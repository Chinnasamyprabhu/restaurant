using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace restaurant.User
{
    public partial class Payment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        protected void lbCardSubmit_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string cardNo = txtCardNo.Text.Trim();
            cardNo = string.Format("************{0}", txtCardNo.Text.Trim().Substring(12, 4));
            string expiryDate = txtExpMonth.Text.Trim() + "/" + txtExpYear.Text.Trim();
            string cvv = txtCvv.Text.Trim();
            string address = txtAddress.Text.Trim();
            string paymentMode = "card";

            if (Session["userId"] != null)
            {
                orderPayment(name, cardNo, expiryDate, cvv, address, paymentMode);
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void lbCodSubmit_Click(object sender, EventArgs e)
        {
            string address = txtCODAddress.Text.Trim();
            string paymentMode = "cod";

            if (Session["userId"] != null)
            {
                orderPayment(string.Empty, string.Empty, string.Empty, string.Empty, address, paymentMode);
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        void orderPayment(string name, string cardNo, string expiryDate, string cvv, string address, string paymentMode)
        {
            int paymentId;

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[7]
            {
        new DataColumn("OrderNo", typeof(string)),
        new DataColumn("ProductId", typeof(int)),
        new DataColumn("Quantity", typeof(int)),
        new DataColumn("UserId", typeof(int)),
        new DataColumn("Status", typeof(string)),
        new DataColumn("PaymentId", typeof(int)),
        new DataColumn("OrderDate", typeof(DateTime)),
            });

            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Save Payment Details
                        using (SqlCommand cmd = new SqlCommand("Save_Payment", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@CardNo", cardNo);
                            cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                            cmd.Parameters.AddWithValue("@Cvv", cvv);
                            cmd.Parameters.AddWithValue("@Address", address);
                            cmd.Parameters.AddWithValue("@PaymentMode", paymentMode);
                            cmd.Parameters.Add("@InsertedId", SqlDbType.Int).Direction = ParameterDirection.Output;

                            cmd.ExecuteNonQuery();
                            paymentId = Convert.ToInt32(cmd.Parameters["@InsertedId"].Value);
                        }

                        // Get Cart Items
                        List<CartItem> cartItems = new List<CartItem>();
                        using (SqlCommand cmd = new SqlCommand("Carts_crud", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Action", "SELECT");
                            cmd.Parameters.AddWithValue("@UserId", Session["userId"]);
                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    cartItems.Add(new CartItem
                                    {
                                        ProductId = (int)dr["ProductId"],
                                        Quantity = (int)dr["Quantity"]
                                    });
                                }
                            }
                        }

                        // Process each cart item
                        foreach (var item in cartItems)
                        {
                            // Update Product Quantity
                            UpdateQuantity(item.ProductId, item.Quantity, transaction, con);

                            // Delete Cart Item
                            DeleteCartItem(item.ProductId, transaction, con);

                            // Add order to DataTable
                            dt.Rows.Add(Utils.GetUniqueid(), item.ProductId, item.Quantity, (int)Session["userId"], "Pending", paymentId, DateTime.Now);
                        }

                        // Save Order Details
                        if (dt.Rows.Count > 0)
                        {
                            using (SqlCommand cmd = new SqlCommand("Save_Orders", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@tblOrders", dt);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        lblMsg.Visible = true;
                        lblMsg.Text = "Your item ordered successfully!!!";
                        lblMsg.CssClass = "alert alert-success";
                        Response.AddHeader("REFRESH", "1;URL=Invoice.aspx?id=" + paymentId);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            Response.Write("<script>alert('Error during rollback: " + rollbackEx.Message + "');</script>");
                        }
                        Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }
        void UpdateQuantity(int _productId, int _quantity, SqlTransaction sqlTransaction, SqlConnection sqlConnection)
        {
            int dbQuantity = 0;
            using (SqlCommand cmd = new SqlCommand("Product_crud", sqlConnection, sqlTransaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@ProductId", _productId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        dbQuantity = (int)dr["Quantity"];
                    }
                    dr.Close();
                }

                if (dbQuantity >= _quantity && dbQuantity > 2)
                {
                    dbQuantity -= _quantity;
                    using (SqlCommand sqlCommand = new SqlCommand("Product_crud", sqlConnection, sqlTransaction))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@Action", "QTYUPDATE");
                        sqlCommand.Parameters.AddWithValue("@Quantity", dbQuantity);
                        sqlCommand.Parameters.AddWithValue("@ProductId", _productId);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        void DeleteCartItem(int _productId, SqlTransaction sqlTransaction, SqlConnection sqlConnection)
        {
            using (SqlCommand cmd = new SqlCommand("Carts_crud", sqlConnection, sqlTransaction))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ProductId", _productId);
                cmd.Parameters.AddWithValue("@UserId", Session["userId"]);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    Response.Write("<script>alert('Error - " + ex.Message + "');</script>");
                }
            }
        }
        public class CartItem
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
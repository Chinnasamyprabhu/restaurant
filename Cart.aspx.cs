using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace restaurant.User
{
    public partial class Cart : System.Web.UI.Page
    {
        private decimal grandTotal;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    getCartItems();
                }
            }
        }

        private void getCartItems()
        {
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Carts_crud", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("UserId", Session["userId"]);

                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sqlDataAdapter.Fill(dt);
                        rCartItem.DataSource = dt;
                        rCartItem.DataBind();

                        if (dt.Rows.Count == 0)
                        {
                            var footerItem = new RepeaterItem(0, ListItemType.Footer);
                            rCartItem.Controls.Add(footerItem);
                            var footerTemplate = new CustomTemplate(ListItemType.Footer);
                            footerTemplate.InstantIn(footerItem);
                        }
                    }
                }
            }
        }

        protected void rCartItem_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                RemoveCartItem(Convert.ToInt32(e.CommandArgument));
            }
            else if (e.CommandName == "UpdateCart")
            {
                UpdateCartItems();
                getCartItems();
            }

            if (e.CommandName == "Checkout")
            {
                CheckoutCartItems();
            }
        }

        private void RemoveCartItem(int productId)
        {
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Carts_crud", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@UserId", Session["userId"]);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        getCartItems();
                        UpdateCartCount();
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<script>alert('Error - " + ex.Message + "');</script>");
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        private void UpdateCartItems()
        {
            foreach (RepeaterItem item in rCartItem.Items)
            {
                HiddenField hdnProductId = item.FindControl("hdnProductId") as HiddenField;
                TextBox txtQuantity = item.FindControl("txtQuantity") as TextBox;

                if (hdnProductId != null && txtQuantity != null)
                {
                    int productId = Convert.ToInt32(hdnProductId.Value);
                    int quantity = Convert.ToInt32(txtQuantity.Text);

                    using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("Carts_crud", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Action", "UPDATE");
                            cmd.Parameters.AddWithValue("@ProductId", productId);
                            cmd.Parameters.AddWithValue("@UserId", Session["userId"]);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);

                            try
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Response.Write("<script>alert('Error - " + ex.Message + "');</script>");
                            }
                            finally
                            {
                                con.Close();
                            }
                        }
                    }
                }
            }
        }

        private void CheckoutCartItems()
        {
            bool isTrue = false;
            string pName = string.Empty;

            for (int item = 0; item < rCartItem.Items.Count; item++)
            {
                if (rCartItem.Items[item].ItemType == ListItemType.Item || rCartItem.Items[item].ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField _productId = rCartItem.Items[item].FindControl("hdnProductId") as HiddenField;
                    HiddenField _cartQuantity = rCartItem.Items[item].FindControl("hdnQuantity") as HiddenField;
                    HiddenField _productQuantity = rCartItem.Items[item].FindControl("hdnPrdQuantity") as HiddenField;
                    Label productName = rCartItem.Items[item].FindControl("lblName") as Label;

                    int productId = Convert.ToInt32(_productId.Value);
                    int cartQuantity = Convert.ToInt32(_cartQuantity.Value);
                    int productQuantity = Convert.ToInt32(_productQuantity.Value);

                    if (productQuantity > cartQuantity && productQuantity > 2)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = false;
                        pName = productName.Text.ToString();
                        break;
                    }
                }
            }

            if (isTrue)
            {
                Response.Redirect("Payment.aspx");
            }
            else
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Item <b>'" + pName + "'</b> is out of stock:(";
                lblMsg.CssClass = "alert alert-warning";
            }
        }

        protected void rCartItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label productPrice = e.Item.FindControl("lblPrice") as Label;
                TextBox quantity = e.Item.FindControl("txtQuantity") as TextBox;
                Label totalPrice = e.Item.FindControl("lblTotalPrice") as Label;

                if (productPrice != null && quantity != null && totalPrice != null)
                {
                    decimal price;
                    int qty;

                    if (decimal.TryParse(productPrice.Text, out price) && int.TryParse(quantity.Text, out qty))
                    {
                        decimal calTotalPrice = price * qty;
                        totalPrice.Text = calTotalPrice.ToString("0.00");
                        grandTotal += calTotalPrice;
                    }
                }
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                Session["grandTotalPrice"] = grandTotal.ToString("0.00");
            }
        }

        private void UpdateCartCount()
        {
            Utils utils = new Utils();
            Session["cartCount"] = utils.cartCount(Convert.ToInt32(Session["userId"]));
        }

        private sealed class CustomTemplate : ITemplate
        {
            private ListItemType ListItemType { get; set; }
            public CustomTemplate(ListItemType type)
            {
                ListItemType = type;
            }
            public void InstantIn(Control container)
            {
                if (ListItemType == ListItemType.Footer)
                {
                    var footer = new LiteralControl("<tr><td colspan='5'><b>Your Cart is empty.</b><a href='menu.aspx' class='badge badge-info ml-2'>Continue Order</a></td></tr></tbody></table>");
                    container.Controls.Add(footer);
                }
            }

            public void InstantiateIn(Control container)
            {
                throw new NotImplementedException();
            }
        }
    }
}
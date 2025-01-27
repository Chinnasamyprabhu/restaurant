using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web;


namespace restaurant.Admin
{
    public partial class OrderStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                // Any initialization code can go here
                Session["breadCrumb"] = "Order Status";
                if (Session["admin"] == null)
                {
                    string originalUrl = HttpContext.Current.Request.Url.PathAndQuery;
                    Response.Redirect($"../User/Login.aspx?ReturnUrl={HttpUtility.UrlEncode(originalUrl)}");
                }
                else
                {
                    getOrderSatus();
                }

            }
            lblMsg.Visible = false;
            pUpdateOrderStatus.Visible = false;
        }
        private void getOrderSatus()
        {
            // Initialize the connection
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                // Create the command
                using (SqlCommand cmd = new SqlCommand("Invoice", con))
                {
                    // Specify that the command is a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the parameter
                    cmd.Parameters.AddWithValue("@Action", "GETSTATUS");

                    // Create the SqlDataAdapter and DataTable
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();

                        // Fill the DataTable
                        sqlDataAdapter.Fill(dt);

                        // Bind the data to the control
                        rOrderStatus.DataSource = dt;
                        rOrderStatus.DataBind();
                    }
                }
            }
        }

        protected void rOrderStatus_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;

            if (e.CommandName == "edit")
            {
                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("Invoice", con))
                    {
                        cmd.Parameters.AddWithValue("@Action", "STATUSBYID");
                        cmd.Parameters.AddWithValue("@OrderDetailsId", e.CommandArgument);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sqlDataAdapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                ddlorderStatus.SelectedValue = dt.Rows[0]["Status"].ToString();
                                hdnId.Value = dt.Rows[0]["OrderDetailsId"].ToString();
                                pUpdateOrderStatus.Visible = true;

                                LinkButton btn = e.Item.FindControl("InkEdit") as LinkButton;
                                if (btn != null)
                                {
                                    btn.CssClass = "badge badge-warning";
                                }
                            }
                        }
                    }
                }

            }
        }

            protected void btnCancel_Click(object sender, EventArgs e)
        {
            pUpdateOrderStatus.Visible = false;
        }
        

        protected void btnUpdate_Click(object sender, EventArgs e)
         {
            int OrderDetailsId = Convert.ToInt32(hdnId.Value);

            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Invoice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "UPDSTATUS");
                    cmd.Parameters.AddWithValue("@OrderDetailsId", OrderDetailsId);
                    cmd.Parameters.AddWithValue("@Status", ddlorderStatus.SelectedValue);
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        lblMsg.Visible = true;
                        lblMsg.Text = "Order Updated successfully";
                        lblMsg.CssClass = "alert alert-success";
                        getOrderSatus();
                        }
                        catch (Exception ex)
                        {
                            lblMsg.Visible = true;
                            lblMsg.Text = "Error: " + ex.Message;
                            lblMsg.CssClass = "alert alert-danger";
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
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
    public partial class Contacts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Any initialization code can go here
                Session["breadCrumb"] = "Contact Users";
                if (Session["admin"] == null)
                {
                    string originalUrl = HttpContext.Current.Request.Url.PathAndQuery;
                    Response.Redirect($"../User/Login.aspx?ReturnUrl={HttpUtility.UrlEncode(originalUrl)}");
                }
                else
                {
                    getcontact();
                }
            }
            lblMsg.Visible = true;

        }

        private void getcontact()
        {
            // Initialize the connection
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                // Create the command
                using (SqlCommand cmd = new SqlCommand("Contact_Crud", con))
                {
                    // Specify that the command is a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the parameter
                    cmd.Parameters.AddWithValue("@Action", "SELECT");

                    // Create the SqlDataAdapter and DataTable
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();

                        // Fill the DataTable
                        sqlDataAdapter.Fill(dt);

                        // Bind the data to the control
                        rContact.DataSource = dt;
                        rContact.DataBind();
                    }
                }
            }
        }

        protected void rContact_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("Contact_Crud", con))
                    {
                        cmd.Parameters.AddWithValue("@Action", "DELETE");
                        cmd.Parameters.AddWithValue("@ContactId", e.CommandArgument);
                        cmd.CommandType = CommandType.StoredProcedure;

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            lblMsg.Visible = true;
                            lblMsg.Text = "Record deleted Successfull!";
                            lblMsg.CssClass = "alert alert-success";
                            getcontact();
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
}
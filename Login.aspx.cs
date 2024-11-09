using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace restaurant.User
{

    public partial class Login : System.Web.UI.Page
    {
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userId"] != null)
            {
                Response.Redirect("Default.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim() == "admin" && txtPassword.Text.Trim() == "123")
            {
                Session["admin"] = txtUsername.Text.Trim();
                Response.Redirect("../Admin/Dashborad.aspx");
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    try
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("User_Curd", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Action", "SELECT4LOGIN");
                            cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                            cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());

                            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                sqlDataAdapter.Fill(dt);

                                if (dt.Rows.Count == 1)
                                {
                                    Session["username"] = txtUsername.Text.Trim();
                                    Session["userId"] = dt.Rows[0]["userId"];
                                    Response.Redirect("Default.aspx");
                                }
                                else
                                {
                                    DisplayInvalidCredentialsMessage();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception (e.g., log the error)
                        lblMsg.Text = "An error occurred while processing your request. Please try again later." + ex.Message;
                        lblMsg.Visible = true;
                        lblMsg.CssClass = "alert alert-danger top-right";
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        private void RedirectAfterLogin()
        {
            string returnUrl = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                Response.Redirect(returnUrl);
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }

        private void DisplayInvalidCredentialsMessage()
        {
            lblMsg.Text = "User ID and password mismatch. Please enter correct ones.";
            lblMsg.Visible = true;
            lblMsg.CssClass = "alert alert-danger top-right";
        }
    }
}
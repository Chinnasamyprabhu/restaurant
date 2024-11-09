using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace restaurant.User
{
    public partial class Registration : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                    if (Request.QueryString["id"] != null)
                    {
                        getUserDetails();
                    }
                    else if (Session["userId"] != null)
                    {
                        Response.Redirect("Default.aspx");
                    }
                
                
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;
            int userId = Convert.ToInt32(Request.QueryString["id"]);

            // Check if the username already exists
            if (UsernameExists(txtUserName.Text.Trim(), userId))
            {
                // Display the error message in the specific field
                lblUsernameError.Visible = true;
                lblUsernameError.Text = $"<b>{txtUserName.Text.Trim()}</b> username already exists, try a new one!";
                return; // Exit the method to prevent further processing
            }

            con = new SqlConnection(Connection.GetConnectionString());
            cmd = new SqlCommand("User_Curd", con);
            cmd.Parameters.AddWithValue("@Action", userId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());
            cmd.Parameters.AddWithValue("@Mobile", txtMoblie.Text.Trim());
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@Address", txtaddress.Text.Trim());
            cmd.Parameters.AddWithValue("@PostCode", txtPostCode.Text.Trim());
            cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());

            if (fuUserImage.HasFile)
            {
                if (Utils.IsValidExtension(fuUserImage.FileName))
                {
                    Guid obj = Guid.NewGuid();
                    fileExtension = Path.GetExtension(fuUserImage.FileName);
                    imagePath = "Images/User/" + obj.ToString() + fileExtension;
                    fuUserImage.PostedFile.SaveAs(Server.MapPath("~/Images/User/") + obj.ToString() + fileExtension);
                    cmd.Parameters.AddWithValue("@ImageUrl", imagePath);
                    isValidToExecute = true;
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Please select .jpg, .jpeg or .png image";
                    lblMsg.CssClass = "alert alert-danger";
                    isValidToExecute = false;
                }
            }
            else
            {
                isValidToExecute = true;
            }

            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    if (userId == 0)
                    {
                        actionName = $"Registration is successful! Welcome, {txtUserName.Text.Trim()}, <b><a href='Login.aspx'>Click here</a></b> fro login";
                    }
                    else
                    {
                        actionName = $"Details updated successfully! Welcome, {txtUserName.Text.Trim()}<b><a href='Profile.aspx'>Can check here</a></b>";
                    }

                    lblSuccessMsg.Visible = true;
                    lblSuccessMsg.Text = actionName;
                    Clear();
                }
                catch (SqlException ex)
                {
                    if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = $"<b>{txtUserName.Text.Trim()}</b> username already exists, try a new one!";
                        lblMsg.CssClass = "alert alert-danger";
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Error--" + ex.Message;
                    lblMsg.CssClass = "alert alert-danger";
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private bool UsernameExists(string username, int userId)
        {
            bool exists = false;
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Username = @Username AND UserId != @UserId", con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();
                    exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    con.Close();
                }
            }
            return exists;
        }

        void getUserDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("User_Curd", con))
                    {
                        cmd.Parameters.AddWithValue("@Action", "SELECT4PROFILE");
                        cmd.Parameters.AddWithValue("@UserId", Request.QueryString["id"]);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            if (dt.Rows.Count == 1)
                            {
                                DataRow row = dt.Rows[0];
                                txtName.Text = row["Name"].ToString();
                                txtUserName.Text = row["Username"].ToString();
                                txtMoblie.Text = row["Mobile"].ToString();
                                txtEmail.Text = row["Email"].ToString();
                                txtaddress.Text = row["Address"].ToString();
                                txtPostCode.Text = row["PostCode"].ToString();

                                string imageUrl = row["ImageUrl"].ToString();
                                imgUser.ImageUrl = string.IsNullOrEmpty(imageUrl) ? "../Images/No_image.png" : "/" + imageUrl;
                                imgUser.Height = 200;
                                imgUser.Width = 200;

                                txtPassword.TextMode = TextBoxMode.SingleLine;
                                txtPassword.ReadOnly = true;
                                txtPassword.Text = row["Password"].ToString();
                            }

                            lblHeaderMsg.Text = "<h2>Edit Profile</h2>";
                            btnRegister.Text = "Update";
                            lblAlreadyUser.Text = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, show a message, etc.)
                lblMsg.Visible = true;
                lblMsg.Text = "Error retrieving user details: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }

        private void Clear()
        {
            txtName.Text = string.Empty;
            txtUserName.Text = string.Empty;
            txtMoblie.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtaddress.Text = string.Empty;
            txtPostCode.Text = string.Empty;
            txtPassword.Text = string.Empty;
            fuUserImage.Attributes.Clear(); // Clears the file upload control (if needed)
            lblMsg.Text = string.Empty; // Clear any message labels
            lblMsg.Visible = false; // Hide the message label
            lblUsernameError.Visible = false; // Hide username error label
        }
    }
}
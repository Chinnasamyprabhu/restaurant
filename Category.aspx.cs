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
    public partial class Category : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Any initialization code can go here
                Session["breadCrumb"] = "Category";
                if (Session["admin"] == null)
                {
                    string originalUrl = HttpContext.Current.Request.Url.PathAndQuery;
                    Response.Redirect($"../User/Login.aspx?ReturnUrl={HttpUtility.UrlEncode(originalUrl)}");
                }
                else
                {
                    getCategories();
                }
              
            }
            lblMsg.Visible = false;
        }

        protected void btnAddorUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagepath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;
            int CategoryId = Convert.ToInt32(hdnId.Value);

            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Category_Curd", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", CategoryId == 0 ? "INSERT" : "UPDATE");
                    cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@IsActive", cbIsActive.Checked);

                    if (fuCategoryImage.HasFile)
                    {
                        if (Utils.IsValidExtension(fuCategoryImage.FileName))
                        {
                            Guid obj = Guid.NewGuid();
                            fileExtension = Path.GetExtension(fuCategoryImage.FileName);
                            imagepath = "/Images/Category/" + obj.ToString() + fileExtension;
                            fuCategoryImage.PostedFile.SaveAs(Server.MapPath("~/Images/Category") + obj.ToString() + fileExtension);
                            string serverPath = Server.MapPath("~/Images/Category/");

                            // Ensure the directory exists
                            if (!Directory.Exists(serverPath))
                            {
                                Directory.CreateDirectory(serverPath);
                            }

                            string fullPath = Path.Combine(serverPath, obj.ToString() + fileExtension);
                            fuCategoryImage.PostedFile.SaveAs(fullPath);
                            cmd.Parameters.AddWithValue("@ImageUrl", imagepath);
                            isValidToExecute = true;
                        }
                        else
                        {
                            lblMsg.Visible = true;
                            lblMsg.Text = "Please select .jpg, .jpeg, .png images";
                            lblMsg.CssClass = "alert alert-danger";
                            isValidToExecute = false;
                        }
                    }
                    else
                    {
                        isValidToExecute = true; // proceed even if there's no image
                    }

                    if (isValidToExecute)
                    {
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            actionName = CategoryId == 0 ? "inserted" : "updated";
                            lblMsg.Visible = true;
                            lblMsg.Text = "Category " + actionName + " successfully";
                            lblMsg.CssClass = "alert alert-success";
                            getCategories();
                            Clear();
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

        private void getCategories()
        {
            // Initialize the connection
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                // Create the command
                using (SqlCommand cmd = new SqlCommand("Category_Curd", con))
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
                        rCategory.DataSource = dt;
                        rCategory.DataBind();
                    }
                }
            }
        }

        private void Clear()
        {
            txtName.Text = string.Empty;
            cbIsActive.Checked = false;
            hdnId.Value = "0";
            imgCategory.ImageUrl = "~/Images/No_image.png"; // Reset image
            btnAddorUpdate.Text = "Add";
            imgCategory.ImageUrl = string.Empty;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        protected void rCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;

            if (e.CommandName == "edit")
            {
                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("Category_Curd", con))
                    {
                        cmd.Parameters.AddWithValue("@Action", "GETBYID");
                        cmd.Parameters.AddWithValue("@CategoryId", e.CommandArgument);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sqlDataAdapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                txtName.Text = dt.Rows[0]["Name"].ToString();
                                cbIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                                imgCategory.ImageUrl = string.IsNullOrEmpty(dt.Rows[0]["ImageUrl"].ToString()) ? "../Images/No_image.png" : "../" + dt.Rows[0]["ImageUrl"].ToString();
                                imgCategory.Height = 200;
                                imgCategory.Width = 200;
                                hdnId.Value = dt.Rows[0]["CategoryId"].ToString();
                                btnAddorUpdate.Text = "Update";

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
            else if (e.CommandName == "delete")
            {
                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("Category_Curd", con))
                    {
                        cmd.Parameters.AddWithValue("@Action", "DELETE");
                        cmd.Parameters.AddWithValue("@CategoryId", e.CommandArgument);
                        cmd.CommandType = CommandType.StoredProcedure;

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            lblMsg.Visible = true;
                            lblMsg.Text = "Category deleted successfully!";
                            lblMsg.CssClass = "alert alert-success";
                            getCategories();
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

        protected void rCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lbl = e.Item.FindControl("lblIsActive") as Label; // Corrected the control ID
                if (lbl != null)
                {
                    // Optional: Trim and compare text in a case-insensitive manner
                    if (lbl.Text.Trim().Equals("True", StringComparison.OrdinalIgnoreCase))
                    {
                        lbl.Text = "Active";
                        lbl.CssClass = "badge badge-success";
                    }
                    else
                    {
                        lbl.Text = "In-Active";
                        lbl.CssClass = "badge badge-danger";
                    }
                }
            }
        }
    }
}
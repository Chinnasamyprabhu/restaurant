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
    public partial class Product : System.Web.UI.Page
    {
        
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Any initialization code can go here
                Session["breadCrumb"] = "Product";
                if (Session["admin"] == null)
                {
                    string originalUrl = HttpContext.Current.Request.Url.PathAndQuery;
                    Response.Redirect($"../User/Login.aspx?ReturnUrl={HttpUtility.UrlEncode(originalUrl)}");
                }
                else
                {
                    getProducts();
                }
            }

            lblMsg.Visible = false;
        }

        protected void btnAddorUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagepath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;
            int productId = Convert.ToInt32(hdnId.Value);

            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Product_crud", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", productId == 0 ? "INSERT" : "UPDATE");
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", txtprice.Text.Trim());
                    cmd.Parameters.AddWithValue("@Quantity", txtquanity.Text.Trim());
                    cmd.Parameters.AddWithValue("@CategoryId", ddlCategories.SelectedValue);
                    cmd.Parameters.AddWithValue("@IsActive", cbIsActive.Checked);

                    if (fuProductImage.HasFile)
                    {
                        if (Utils.IsValidExtension(fuProductImage.FileName))
                        {
                            Guid obj = Guid.NewGuid();
                            fileExtension = Path.GetExtension(fuProductImage.FileName);
                            imagepath = "/Images/Product" + obj.ToString() + fileExtension;
                            fuProductImage.PostedFile.SaveAs(Server.MapPath("~/Images/Product") + obj.ToString() + fileExtension);
                            string serverPath = Server.MapPath("~/Images/Product/");
                            // Ensure the directory exists
                            if (!Directory.Exists(serverPath))
                            {
                                Directory.CreateDirectory(serverPath);
                            }

                            string fullPath = Path.Combine(serverPath, obj.ToString() + fileExtension);
                            fuProductImage.PostedFile.SaveAs(fullPath);
                            cmd.Parameters.AddWithValue("@ImageUrl", imagepath);
                            isValidToExecute = true;
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
                            actionName = productId == 0 ? "inserted" : "updated";
                            lblMsg.Visible = true;
                            lblMsg.Text = "Product " + actionName + " successfully";
                            lblMsg.CssClass = "alert alert-success";
                            getProducts();
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
        private void getProducts()
        {
            // Initialize the connection
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                // Create the command
                using (SqlCommand cmd = new SqlCommand("Product_crud", con))
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
                        rProduct.DataSource = dt;
                       rProduct.DataBind();
                    }
                }
            }
        }

        private void Clear()
        {
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtquanity.Text = String.Empty;
            txtprice.Text = string.Empty;
            ddlCategories.ClearSelection();
            cbIsActive.Checked = false;
            hdnId.Value = "0";
            imgProduct.ImageUrl = "~/Images/No_image.png"; // Reset image
            btnAddorUpdate.Text = "Add";
            imgProduct.ImageUrl = string.Empty;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        protected void rProduct_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            lblMsg.Visible = false;

            if (e.CommandName == "edit")
            {
                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("Product_crud", con))
                    {
                        cmd.Parameters.AddWithValue("@Action", "GETBYID");
                        cmd.Parameters.AddWithValue("@ProductId", e.CommandArgument);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sqlDataAdapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                txtName.Text = dt.Rows[0]["Name"].ToString();
                                txtDescription.Text = dt.Rows[0]["Description"].ToString();
                                txtprice.Text = dt.Rows[0]["Price"].ToString();
                                txtquanity.Text = dt.Rows[0]["Quantity"].ToString();
                                ddlCategories.SelectedValue = dt.Rows[0]["CategoryId"].ToString();
                                cbIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                                imgProduct.ImageUrl = string.IsNullOrEmpty(dt.Rows[0]["ImageUrl"].ToString()) ? "../Images/No_image.png" : "../" + dt.Rows[0]["ImageUrl"].ToString();
                                imgProduct.Height = 200;
                                imgProduct.Width = 200;
                                hdnId.Value = dt.Rows[0]["ProductId"].ToString();
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
                    using (SqlCommand cmd = new SqlCommand("Product_crud", con))
                    {
                        cmd.Parameters.AddWithValue("@Action", "DELETE");
                        cmd.Parameters.AddWithValue("@ProductId", e.CommandArgument);
                        cmd.CommandType = CommandType.StoredProcedure;

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            lblMsg.Visible = true;
                            lblMsg.Text = "Product deleted successfully!";
                            lblMsg.CssClass = "alert alert-success";
                            getProducts();
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

        protected void rProduct_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblIsActive = e.Item.FindControl("lblIsActive") as Label;
                Label lblQuantity = e.Item.FindControl("lblQuantity") as Label;// Corrected the control ID
                if (lblIsActive != null)
                {
                    // Optional: Trim and compare text in a case-insensitive manner
                    if (lblIsActive.Text.Trim().Equals("True", StringComparison.OrdinalIgnoreCase))
                    {
                        lblIsActive.Text = "Active";
                        lblIsActive.CssClass = "badge badge-success";
                    }
                    else
                    {
                        lblIsActive.Text = "In-Active";
                        lblIsActive.CssClass = "badge badge-danger";
                    }

                    if (Convert.ToInt32(lblQuantity.Text) <=10)
                    {
                        lblQuantity.CssClass = "badge badge-danger";
                        lblQuantity.ToolTip = "Item about to be 'Out of Stock'";
                    }
                }
            }
        }
    }
}
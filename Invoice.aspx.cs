using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web;

namespace restaurant.User
{
    public partial class Invoice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // Ensure the page loads only once
            {
                if (Session["userId"] != null)
                {
                    if (Request.QueryString["id"] != null)
                    {
                        BindOrderDetails(); // Bind the order details to the Repeater
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void BindOrderDetails()
        {
            try
            {
                DataTable dt = OrderDetails; // Fetch order details
                lblMsg.Text = "Rows returned: " + dt.Rows.Count;
                lblMsg.Visible = true;

                if (dt.Rows.Count > 0)
                {
                    rOrderItem.DataSource = dt; // Bind data to the Repeater
                    rOrderItem.DataBind();
                }
                else
                {
                    lblMsg.Text += " - No orders found.";
                    lblMsg.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.Visible = true;
            }
        }

        private DataTable OrderDetails
        {
            get
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("Invoice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "INVOICBYID");
                        cmd.Parameters.AddWithValue("@PaymentId", Convert.ToInt32(Request.QueryString["id"]));
                        cmd.Parameters.AddWithValue("@UserId", Session["userId"]);

                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        sda.Fill(dt);

                        // Calculate grand total and add to DataTable
                        if (dt.Rows.Count > 0)
                        {
                            double grandTotal = 0;
                            foreach (DataRow drow in dt.Rows)
                            {
                                grandTotal += Convert.ToDouble(drow["TotalPrice"]);
                            }

                            // Add a new row to display the grand total
                            DataRow dr = dt.NewRow();
                            dr["OrderNo"] = "Grand Total"; // Ensure "OrderNo" exists in your DataTable
                            dr["TotalPrice"] = grandTotal;
                            dt.Rows.Add(dr);
                        }
                    }
                }
                return dt;
            }
        }

        protected void lbDownloadInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = OrderDetails; // Fetch order details for PDF
                if (dt.Rows.Count > 0)
                {
                    GeneratePDF(dt); // Generate and download PDF
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.Visible = true;
            }
        }

        private void GeneratePDF(DataTable dt)
        {
            string fileName = "Invoice_" + Request.QueryString["id"] + ".pdf";
            string filePath = Server.MapPath("~/Invoices/" + fileName);

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Document doc = new Document();
                    PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Add title and other details
                    doc.Add(new Paragraph("Invoice"));
                    doc.Add(new Paragraph("Date: " + DateTime.Now.ToString()));

                    // Create a table with the necessary columns
                    PdfPTable table = new PdfPTable(6); // Ensure column count matches
                    table.AddCell("SrNo");
                    table.AddCell("Order Number");
                    table.AddCell("Item Name");
                    table.AddCell("Unit Price");
                    table.AddCell("Quantity");
                    table.AddCell("Total Price");

                    // Populate the table with data from the DataTable
                    foreach (DataRow row in dt.Rows)
                    {
                        table.AddCell(row["SrNo"].ToString());
                        table.AddCell(row["OrderNo"].ToString());
                        table.AddCell(row["Name"].ToString());
                        table.AddCell("₹" + row["Price"].ToString());
                        table.AddCell(row["Quantity"].ToString());
                        table.AddCell("₹" + row["TotalPrice"].ToString());
                    }

                    doc.Add(table);
                    doc.Close();
                }

                // Serve the file for download
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(filePath);
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error generating PDF: " + ex.Message;
                lblMsg.Visible = true;
            }
        }
    }
}
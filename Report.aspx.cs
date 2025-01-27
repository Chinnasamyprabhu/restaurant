using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace restaurant.Admin
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Any initialization code can go here
                Session["breadCrumb"] = "Selling Report";
                if (Session["admin"] == null)
                {
                    string originalUrl = HttpContext.Current.Request.Url.PathAndQuery;
                    Response.Redirect($"../User/Login.aspx?ReturnUrl={HttpUtility.UrlEncode(originalUrl)}");
                }
            }
        }

        private void getReportDate(DateTime fromDate, DateTime toDate)
        {
            // Initialize the connection
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                // Create the command
                using (SqlCommand cmd = new SqlCommand("SellingReport", con))
                {
                    // Specify that the command is a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the parameters
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    // Create the SqlDataAdapter and DataTable
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();

                        // Fill the DataTable
                        sqlDataAdapter.Fill(dt);

                        // Check if the DataTable has rows
                        if (dt.Rows.Count > 0)
                        {
                            double grandTotal = 0;
                            foreach (DataRow dr in dt.Rows)
                            {
                                grandTotal += Convert.ToDouble(dr["TotalPrice"]);
                            }
                            lblTotal.Text = "Sold Cost: ₹" + grandTotal.ToString("N2");
                            lblTotal.CssClass = "badge badge-primary";

                            // Bind the data to the Repeater
                            rReport.DataSource = dt;
                            rReport.DataBind();
                        }
                        else
                        {
                            // No data available
                            lblTotal.Text = "No data available for the selected date range.";
                            lblTotal.CssClass = "badge badge-warning";
                            rReport.DataSource = null;
                            rReport.DataBind();
                        }
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime fromDate, toDate;

            // Validate dates
            if (DateTime.TryParse(txtFromDate.Text, out fromDate) && DateTime.TryParse(txtToDate.Text, out toDate))
            {
                if (toDate > DateTime.Now)
                {
                    Response.Write("<script>alert('ToDate cannot be greater than current date!');</script>");
                }
                else if (fromDate > toDate)
                {
                    Response.Write("<script>alert('FromDate cannot be greater than ToDate!');</script>");
                }
                else
                {
                    getReportDate(fromDate, toDate);
                }
            }
            else
            {
                Response.Write("<script>alert('Invalid date format!');</script>");
            }
        }
    }
}
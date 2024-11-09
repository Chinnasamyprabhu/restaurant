using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace restaurant.User
{
    public partial class Profile : Page
    {
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
                    getUserDetails();
                    getPurchaseHistory();
                }
            }
        }

        private void getUserDetails()
        {
            string connectionString = Connection.GetConnectionString(); // Assuming you have a Connection class for connection strings
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("User_Curd", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT4PROFILE");
                    cmd.Parameters.AddWithValue("@UserId", Session["userId"]);

                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        rUserProfile.DataSource = dt;
                        rUserProfile.DataBind();
                        if (dt.Rows.Count == 1)
                        {
                            DataRow row = dt.Rows[0];
                            Session["name"] = row["Name"].ToString();
                            Session["email"] = row["Email"].ToString();
                            Session["imageUrl"] = row["ImageUrl"].ToString();
                            Session["CreatedDate"] = row["CreatedDate"].ToString();
                            Session["username"] = row["Username"].ToString(); // Add this if username is needed

                            // Update UI with session data
                            lblName.Text = Session["name"].ToString();
                            lblUsername.Text = "@" + Session["username"].ToString();
                            lblEmail.Text = Session["email"].ToString();
                            lblCreatedDate.Text = Session["CreatedDate"].ToString();
                        }
                    }
                }
            }
        }

        private void getPurchaseHistory()
        {
            int sr = 1; // Adjusted to start from 1
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Invoice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "ORDHISTORY");
                    cmd.Parameters.AddWithValue("@UserId", Session["userId"]);

                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sqlDataAdapter.Fill(dt);
                        rPurchaseHistory.DataSource = dt;
                        rPurchaseHistory.DataBind();

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dataRow in dt.Rows)
                            {
                                dataRow["SrNo"] = sr;
                                sr++;
                            }
                        }

                        if (dt.Rows.Count == 0)
                        {
                            var footerItem = new RepeaterItem(0, ListItemType.Footer);
                            rPurchaseHistory.Controls.Add(footerItem);
                            var footerTemplate = new CustomTemplate(ListItemType.Footer);
                            footerTemplate.InstantIn(footerItem);
                        }
                    }
                }
            }
        }

        protected void rPurchaseHistory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HiddenField PaymentId = e.Item.FindControl("hdnPaymentId") as HiddenField;
                Repeater repOrders = e.Item.FindControl("rOrders") as Repeater;

                using (SqlConnection con = new SqlConnection(Connection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("Invoice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "INVOICBYID");
                        cmd.Parameters.AddWithValue("@paymentId", Convert.ToInt32(PaymentId.Value));
                        cmd.Parameters.AddWithValue("@UserId", Session["userId"]);

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sqlDataAdapter.Fill(dt);

                            // Calculate grand total and add it to a new row
                            if (dt.Rows.Count > 0)
                            {
                                double grandTotal = 0;
                                foreach (DataRow drow in dt.Rows)
                                {
                                    grandTotal += Convert.ToDouble(drow["TotalPrice"]);
                                }

                                // Add a new row to display the grand total
                                DataRow dr = dt.NewRow();
                                dr["Name"] = "Grand Total"; // Adjust this to match your column
                                dr["TotalPrice"] = grandTotal;
                                dt.Rows.Add(dr);
                            }

                            // Bind the updated DataTable to the repeater
                            repOrders.DataSource = dt;
                            repOrders.DataBind();
                        }
                    }
                }
            }
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
                    var footer = new LiteralControl("<tr><td><b>Your are in Hungry! Why not order food for you.</b><a href='menu.aspx' class='badge badge-info ml-2'>Click to Order</a></td></tr></tbody></table>");
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace restaurant.Admin
{
    public partial class Dashborad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrumb"] = " ";
                if(Session["admin"] == null)
                {
                    string originalUrl = HttpContext.Current.Request.Url.PathAndQuery;
                    Response.Redirect($"../User/Login.aspx?ReturnUrl={HttpUtility.UrlEncode(originalUrl)}");
                }
                else
                {
                    DashboardCount dashboard = new DashboardCount();
                    Session["category"] = dashboard.Count("CATEGORY");
                    Session["product"] = dashboard.Count("PRODUCT");
                    Session["order"] = dashboard.Count("ORDER");
                    Session["delivered"] = dashboard.Count("DELIVERED");
                    Session["pending"] = dashboard.Count("PENDING");
                    Session["user"] = dashboard.Count("USER");
                    Session["soldAmount"] = dashboard.Count("SOLDAMOUNT");
                    Session["conact"] = dashboard.Count("CONTACT");
                }
            }
        }
    }
}
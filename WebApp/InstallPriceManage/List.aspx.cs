using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.InstallPriceManage
{
    public partial class List : BasePage
    {
        public string url = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            url = Request.FilePath;
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime date = DateTime.Now;
                txtMonth.Text = date.ToString("yyyy-MM");

            }
        }
    }
}
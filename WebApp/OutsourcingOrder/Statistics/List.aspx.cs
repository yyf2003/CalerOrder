using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.OutsourcingOrder.Statistics
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                BindCustomerList(ddlCustomer1);
                DateTime date = DateTime.Now;
                txtMonth.Text = date.ToString("yyyy-MM");
            }
        }
    }
}
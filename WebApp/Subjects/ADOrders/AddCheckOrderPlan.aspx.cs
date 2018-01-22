using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Subjects.ADOrders
{
    public partial class AddCheckOrderPlan : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMyCustomerList(ref ddlCustomer);
            }
        }
    }
}
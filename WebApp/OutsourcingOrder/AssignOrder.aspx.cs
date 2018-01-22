using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WebApp.OutsourcingOrder
{
    public partial class AssignOrder : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                BindCustomerList(ddlCustomer);
                DateTime date = DateTime.Now;
                txtMonth.Text = date.ToString("yyyy-MM");
                Session["orderAssign"] = null;
                Session["shopAssign"] = null;
            }
        }
    }
}
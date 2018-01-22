using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Subjects.ADOrders
{
    public partial class CheckOrder : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);

            }
            if (!IsPostBack)
            {
                BindMyCustomerList(ref ddlCustomer);
            }
        }
    }
}
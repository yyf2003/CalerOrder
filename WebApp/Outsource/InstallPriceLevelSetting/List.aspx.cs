using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;

namespace WebApp.Outsource.InstallPriceLevelSetting
{
    public partial class List :BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<string> permissionList = GetPromissionList();
            if (permissionList.Any() && permissionList.Contains("add"))
            {
                operatorToolbar.Style.Add("display","");
            }
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
            }
        }
    }
}
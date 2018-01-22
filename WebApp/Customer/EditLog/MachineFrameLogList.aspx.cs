using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Customer.EditLog
{
    public partial class MachineFrameLogList : BasePage
    {
        public int ShopId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["shopId"] != null)
            {
                ShopId = int.Parse(Request.QueryString["shopId"]);
            }
        }

        
    }
}
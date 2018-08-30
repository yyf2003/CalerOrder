using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;

namespace WebApp.OutsourcingOrder.PayRecord
{
    public partial class List : BasePage
    {
        //int addPromission = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime date = DateTime.Now;
                txtMonth.Text = date.ToString("yyyy-MM");

                string promission = GetPromissionStr();
                if (!string.IsNullOrWhiteSpace(promission))
                {
                    List<string> promissionList = StringHelper.ToStringList(promission,'|');
                    if (promissionList.Contains("add"))
                    {
                        hfAddPromission.Value = "1";
                    }
                }
            }
        }
    }
}
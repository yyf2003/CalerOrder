using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;

namespace WebApp.OutsourcingOrder
{
    public partial class OrderList :BasePage
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

                var orderTypeList = CommonMethod.GetEnumList<OrderTypeEnum>().ToList();
                orderTypeList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s.Name + "&nbsp;";
                    li.Value = s.Value.ToString();
                    rblOrderType.Items.Add(li);
                });
            }

        }
    }
}
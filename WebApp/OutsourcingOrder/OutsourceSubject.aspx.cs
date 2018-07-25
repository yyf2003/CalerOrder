using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using Common;
using System.Configuration;

namespace WebApp.OutsourcingOrder
{
    public partial class OutsourceSubject : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtMonth.Text = now.Year + "-" + now.Month;

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

        protected void lbDownLoadTemplate_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "批量更改外协模板";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }



       
    }
}
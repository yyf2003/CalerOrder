using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using Common;

namespace WebApp.Materials
{
    public partial class ExportCustomerMaterial : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string where = string.Empty;
            if (Request.QueryString["customerId"] != null)
            {
                int customerId = int.Parse(Request.QueryString["customerId"]);
                where += " and CustomerMaterialInfo.CustomerId=" + customerId;
            }
            DataSet ds = new CustomerMaterialInfoBLL().GetDataList(where);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "客户材质", null, "customerMaterial");

            }
            else
            {
                Alert("没有数据！");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Common;
using BLL;

namespace WebApp.Materials
{
    public partial class ExportQuoteMaterial : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string where = string.Empty;
            if (Request.QueryString["customerId"] != null)
            {
                int customerId = int.Parse(Request.QueryString["customerId"]);
                where += " and QuoteMaterial.CustomerId=" + customerId;
            }
            DataSet ds = new QuoteMaterialBLL().Export(where);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "报价材质", null, "quoteMaterial");

            }
            else
            {
                Alert("没有数据！");
            }
        }
    }
}
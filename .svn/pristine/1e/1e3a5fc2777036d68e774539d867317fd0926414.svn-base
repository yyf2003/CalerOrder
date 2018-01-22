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
    public partial class ExportBasicMaterial : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string where = string.Empty;
            if (Request.QueryString["whereStr"] != null)
            {
                where = Request.QueryString["whereStr"];
            }
            DataSet ds = new BasicMaterialBLL().GetDataList(where);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "基础材质", null, "basicMaterial");

            }
            else
            {
                Alert("没有数据！");
            }
        }
    }
}
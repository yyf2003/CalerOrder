using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using Common;

namespace WebApp.Customer
{
    public partial class ExportBasicMachineFrame : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataSet ds = new BasicMachineFrameBLL().Export();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "基础器架信息", null, "BasicMachineFrame");

            }
            else
            {
                Alert("没有数据！");
            }
        }
    }
}
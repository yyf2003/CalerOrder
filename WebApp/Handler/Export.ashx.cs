using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using Common;
using System.Data;

namespace WebApp.Handler
{
    /// <summary>
    /// Export 的摘要说明
    /// </summary>
    public class Export : IHttpHandler,IRequiresSessionState
    {
        HttpContext context1;
        string fileName = string.Empty;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context1 = context;
            if (context.Request["filename"] != null)
            {
                fileName = context.Request["filename"];
            }
            ExportExcel();
        }


        void ExportExcel()
        {
            if (context1.Session["dt"] != null)
            {
                DataTable dt = (DataTable)context1.Session["dt"];
                context1.Session.Contents.Remove("dt");
                if (dt != null)
                {
                    //Session["ExportState"] = "ok";
                    OperateFile.DownLoadFile(dt, fileName);
                }
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
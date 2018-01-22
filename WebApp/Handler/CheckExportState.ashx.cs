using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebApp.Handler
{
    /// <summary>
    /// CheckExportState 的摘要说明
    /// </summary>
    public class CheckExportState : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string state = "on";
            if (context.Session["ExportState"] != null && context.Session["ExportState"].ToString() == "ok")
            {
                state = "ok";
                context.Session.Contents.Remove("ExportState");
            }
            context.Response.Write(state);
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
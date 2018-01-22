using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebApp.Statistics.handler
{
    /// <summary>
    /// CheckExportDetail 的摘要说明
    /// </summary>
    public class CheckExportDetail : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpCookie cookie = context.Request.Cookies["项目费用统计明细"];
            string result = cookie != null ? cookie.Value : "0";
            if (result == "1")
            {
                cookie.Value = "0";
                cookie.Expires = DateTime.Now.AddMinutes(30);
                context.Response.Cookies.Add(cookie);
                result = "ok";

            }
            context.Response.Write(result);
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
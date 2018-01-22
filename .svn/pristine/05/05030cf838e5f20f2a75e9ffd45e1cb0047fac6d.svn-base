using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebApp.Statistics.handler
{
    /// <summary>
    /// CheckExportState 的摘要说明
    /// </summary>
    public class CheckExportState : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            string result = "0";
            switch (type)
            {
                case "allPrice":
                    HttpCookie cookie = context.Request.Cookies["项目费用统计明细"];
                    result = cookie != null ? cookie.Value : "0";
                    if (result == "1")
                    {
                        cookie.Value = "0";
                        cookie.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie);
                        result = "ok";

                    }
                    break;
                case "installPrice":
                    HttpCookie cookie0 = context.Request.Cookies["安装费明细"];
                    result = cookie0 != null ? cookie0.Value : "0";
                    if (result == "1")
                    {
                        cookie0.Value = "0";
                        cookie0.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie0);
                        result = "ok";

                    }
                    break;
                case "freihtlPrice":
                    HttpCookie cookie1 = context.Request.Cookies["发货费明细"];
                    result = cookie1 != null ? cookie1.Value : "0";
                    if (result == "1")
                    {
                        cookie1.Value = "0";
                        cookie1.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie1);
                        result = "ok";

                    }
                    break;
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
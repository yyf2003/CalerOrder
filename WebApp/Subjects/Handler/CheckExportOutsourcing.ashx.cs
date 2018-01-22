using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebApp.Subjects.Handler
{
    /// <summary>
    /// CheckExportOutsourcing 的摘要说明
    /// </summary>
    public class CheckExportOutsourcing : IHttpHandler
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
                case "":
                    HttpCookie cookie = context.Request.Cookies["export"];
                    result = cookie != null ? cookie.Value : "0";
                    if (result == "1")
                    {
                        cookie.Value = "0";
                        cookie.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie);
                        result = "ok";

                    }
                    break;
                case "350":
                    HttpCookie cookie0 = context.Request.Cookies["exportOutsourcing"];
                    result = cookie0 != null ? cookie0.Value : "0";
                    if (result == "1")
                    {
                        cookie0.Value = "0";
                        cookie0.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie0);
                        result = "ok";

                    }
                    break;
                default:
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
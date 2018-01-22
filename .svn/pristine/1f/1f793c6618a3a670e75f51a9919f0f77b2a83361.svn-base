using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebApp.Customer.Handler
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
                case "pop":
                    HttpCookie cookie = context.Request.Cookies["pop"];
                    result = cookie != null ? cookie.Value : "0";
                    if (result == "1")
                    {
                        cookie.Value = "0";
                        cookie.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie);
                        result = "ok";

                    }
                    break;
                case "shop":
                    HttpCookie cookie0 = context.Request.Cookies["shop"];
                    result = cookie0 != null ? cookie0.Value : "0";
                    if (result == "1")
                    {
                        cookie0.Value = "0";
                        cookie0.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie0);
                        result = "ok";

                    }
                    break;
                case "shopPop":
                    HttpCookie cookie1 = context.Request.Cookies["shopPop"];
                    result = cookie1 != null ? cookie1.Value : "0";
                    if (result == "1")
                    {
                        cookie1.Value = "0";
                        cookie1.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie1);
                        result = "ok";

                    }
                    break;
                case "shopFrame":
                    HttpCookie cookie2 = context.Request.Cookies["shopFrame"];
                    result = cookie2 != null ? cookie2.Value : "0";
                    if (result == "1")
                    {
                        cookie2.Value = "0";
                        cookie2.Expires = DateTime.Now.AddMinutes(30);
                        context.Response.Cookies.Add(cookie2);
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebApp.Subjects.Handler
{
    /// <summary>
    /// CheckExportPHWBJ 的摘要说明
    /// </summary>
    public class CheckExportPHWBJ : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpCookie cookie = context.Request.Cookies["exportPHWBJ"];
            string result = cookie != null ? cookie.Value : "0";
            if (result == "1")
            {
                cookie.Value = "0";
                cookie.Expires = DateTime.Now.AddMinutes(-1);
                context.Response.Cookies.Add(cookie);
                result = "ok";

            }
            else if (result == "2")
            {
                cookie.Value = "0";
                cookie.Expires = DateTime.Now.AddMinutes(-1);
                context.Response.Cookies.Add(cookie);
                result = "empty";
            }
            else if (result == "3")
            {
                cookie.Value = "0";
                cookie.Expires = DateTime.Now.AddMinutes(-1);
                context.Response.Cookies.Add(cookie);
                result = "notFinishInstallPrice";
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
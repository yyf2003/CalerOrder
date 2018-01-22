﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;



namespace WebApp.Subjects.SupplementByRegion
{
    /// <summary>
    /// CheckExport 的摘要说明
    /// </summary>
    public class CheckExport : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpCookie cookie = context.Request.Cookies["exportRegionSupplement"];
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
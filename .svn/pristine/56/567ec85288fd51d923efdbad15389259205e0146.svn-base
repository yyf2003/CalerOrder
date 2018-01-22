using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Handler
{
    /// <summary>
    /// CheckPromission 的摘要说明
    /// </summary>
    public class CheckPromission : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string url = string.Empty;
            if (context.Request.QueryString["url"] != null)
                url = context.Request.QueryString["url"];
            string promission = new BasePage().GetPromissionStr(url);
            context.Response.Write(promission);
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
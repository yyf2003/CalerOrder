using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Subjects.ExtraOrder.handler
{
    /// <summary>
    /// AddOrderDetail 的摘要说明
    /// </summary>
    public class AddOrderDetail : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
           
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
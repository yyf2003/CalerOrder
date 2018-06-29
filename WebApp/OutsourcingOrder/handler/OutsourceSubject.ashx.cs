using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Common;
using Models;
using BLL;
using DAL;

namespace WebApp.OutsourcingOrder.handler
{
    /// <summary>
    /// OutsourceSubject 的摘要说明
    /// </summary>
    public class OutsourceSubject : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {

            context1 = context;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Text;


namespace WebApp.Subjects.ChangeMaterialPrice
{
    /// <summary>
    /// CheckUpdateState 的摘要说明
    /// </summary>
    public class CheckUpdateState : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //HttpCookie cookie1 = context.Request.Cookies["updateIncomePriceTotal"];
            //int total = cookie1 != null ? int.Parse(cookie1.Value) : 1;
            //HttpCookie cookie2 = context.Request.Cookies["updateIncomePriceFinish"];
            //int finish = cookie2 != null ? int.Parse(cookie2.Value) : 0;
            int total = 1;
            int finish = 0;
            if (context.Session["updateIncomePriceTotal"] != null)
            {
                total = int.Parse(context.Session["updateIncomePriceTotal"].ToString());
            }
            if (context.Session["updateIncomePriceFinish"] != null)
            {
                finish = int.Parse(context.Session["updateIncomePriceFinish"].ToString());
            }
            StringBuilder json = new StringBuilder();
            json.Append("{\"Total\":\"" + total + "\",\"Finish\":\"" + finish + "\"}");
            
            //if (finish == total)
            //{
            //    cookie1.Value = "1";
            //    cookie1.Expires = DateTime.Now.AddDays(-1);
            //    context.Response.Cookies.Add(cookie1);

            //    cookie2.Value = "0";
            //    cookie2.Expires = DateTime.Now.AddDays(-1);
            //    context.Response.Cookies.Add(cookie2);
            //}
            context.Response.Write("[" + json.ToString() + "]");

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
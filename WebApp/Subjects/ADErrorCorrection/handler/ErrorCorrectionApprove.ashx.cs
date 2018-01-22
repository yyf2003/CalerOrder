using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;


namespace WebApp.Subjects.ADErrorCorrection.handler
{
    /// <summary>
    /// ErrorCorrectionApprove 的摘要说明
    /// </summary>
    public class ErrorCorrectionApprove : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }

        }

        string GetList(int guidanceId,int currPage, int pageSize)
        {
            return "";
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using BLL;

namespace WebApp.Subjects.Handler
{
    /// <summary>
    /// AddGuidance 的摘要说明
    /// </summary>
    public class AddGuidance : IHttpHandler
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
            switch (type)
            {
                case "deleteType":
                    int typeId = 0;
                    if (context.Request.QueryString["typeId"] != null)
                    {
                        typeId = int.Parse(context.Request.QueryString["typeId"]);
                    }
                    result = DeleteType(typeId);
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string DeleteType(int typeId)
        {
            SubjectTypeBLL typeBll = new SubjectTypeBLL();
            SubjectType model = typeBll.GetModel(typeId);
            if (model != null)
            {
                model.IsDelete = true;
                typeBll.Update(model);
                return "ok";
            }
            else
                return "error";
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
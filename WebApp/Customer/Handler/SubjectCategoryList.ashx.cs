using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using Newtonsoft.Json;

namespace WebApp.Customer.Handler
{
    /// <summary>
    /// SubjectCategoryList 的摘要说明
    /// </summary>
    public class SubjectCategoryList : IHttpHandler
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
                case "edit":
                    string jsonString = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = edit(jsonString, optype);
                    break;
            }
            context.Response.Write(result);
        }

        string edit(string jsonString, string optype)
        {
            string result = "提交失败";
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                ADSubjectCategory model = JsonConvert.DeserializeObject<ADSubjectCategory>(jsonString);
                if (model != null)
                {
                    if (optype == "add")
                    {
                        if (!CheckName(model.CategoryName))
                        {
                            new ADSubjectCategoryBLL().Add(model);
                            result = "ok";
                        }
                        else
                        {
                            result = "exist";
                        }
                    }
                    else
                    {
                        if (!CheckName(model.CategoryName, model.Id))
                        {
                            new ADSubjectCategoryBLL().Update(model);
                            result = "ok";
                        }
                        else
                        {
                            result = "exist";
                        }

                    }
                }
            }

            return result;
        }

        bool CheckName(string name, int? id = null)
        {
            var model = new ADSubjectCategoryBLL().GetList(s => s.CategoryName == name && ((id != null) ? (s.Id != id) : 1 == 1) && (s.IsDelete==null || s.IsDelete==false)).FirstOrDefault();
            return model != null;
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
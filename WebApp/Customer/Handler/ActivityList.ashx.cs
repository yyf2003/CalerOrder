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
    /// ActivityList 的摘要说明
    /// </summary>
    public class ActivityList : IHttpHandler
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
                ADOrderActivity model = JsonConvert.DeserializeObject<ADOrderActivity>(jsonString);
                if (model != null)
                {
                    if (optype == "add")
                    {
                        if (!CheckName(model.ActivityName))
                        {
                            new ADOrderActivityBLL().Add(model);
                            result = "ok";
                        }
                        else
                        {
                            result = "该活动已经存在";
                        }
                    }
                    else
                    {
                        if (!CheckName(model.ActivityName,model.ActivityId))
                        {
                            new ADOrderActivityBLL().Update(model);
                            result = "ok";
                        }
                        else
                        {
                            result = "该活动已经存在";
                        }
                        
                    }
                }
            }

            return result;
        }

        bool CheckName(string name,int? id=null)
        {
            var model = new ADOrderActivityBLL().GetList(s=>s.ActivityName==name && ((id!=null)?(s.ActivityId!=id):1==1)).FirstOrDefault();
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
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
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        CustomerBLL customerBll = new CustomerBLL();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request["type"] != null)
            {
                type = context.Request["type"];
            }

            switch (type)
            {
                case "edit":
                    string json = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = Edit(optype, json);
                    break;
                
            }
            context.Response.Write(result);
        }

        string Edit(string opType,string jsonStr)
        {
            string result = "error";
            if (!string.IsNullOrEmpty(jsonStr))
            {
                Models.Customer model = JsonConvert.DeserializeObject<Models.Customer>(jsonStr);
                if (model != null)
                {
                    if (opType == "add")
                    {
                        if (!Check(0, model.CustomerName))
                        {
                            model.IsDelete = false;
                            customerBll.Add(model);
                        }
                        else
                        {
                            result = "exist";
                        }
                    }
                    else
                    {
                        if (!Check(model.Id, model.CustomerName))
                        {
                            customerBll.Update(model);
                        }
                        else
                        {
                            result = "exist";
                        }
                        
                    }
                    result = "ok";
                }
            }
            return result;
        }

        bool Check(int id, string name)
        {
            var list = customerBll.GetList(s => s.CustomerName == name);
            if (list.Any() && id>0)
            {
                list = list.Where(s=>s.Id!=id).ToList();
            }
            return list.Any();
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
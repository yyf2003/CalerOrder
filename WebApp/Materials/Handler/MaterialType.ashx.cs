using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using Newtonsoft.Json;
using System.Text;

namespace WebApp.Materials.Handler
{
    /// <summary>
    /// MaterialType 的摘要说明
    /// </summary>
    public class MaterialType : IHttpHandler
    {
        string type = string.Empty;
        MaterialTypeBLL materialTypeBll = new MaterialTypeBLL();
        
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            if (context.Request["type"] != null)
            {
                type = context.Request["type"];
            }
            switch (type)
            {
                case "getParent":
                    result = GetBigType();
                    break;
               
                case "edit":
                    string jsonString = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = AddType(jsonString, optype);
                    break;
            }
            context.Response.Write(result);
        }

        string GetBigType()
        {
            var list = materialTypeBll.GetList(s=>s.ParentId==0);
            StringBuilder Json = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    Json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.MaterialTypeName + "\",\"ParentId\":\"" + s.ParentId + "\"");
                    
                    Json.Append("},");
                });
            }
            return "[" + Json.ToString().TrimEnd(',') + "]";
        }


        string AddType(string jsonString, string optype)
        {
            string result = "提交失败！";
            if (!string.IsNullOrEmpty(jsonString))
            {
                Models.MaterialType model = JsonConvert.DeserializeObject<Models.MaterialType>(jsonString);
                if (model != null)
                {
                    if (optype == "add")
                    {
                        if (!CheckTypeName(model.MaterialTypeName, 0))
                        {
                            model.AddDate = DateTime.Now;
                            model.IsDelete = false;
                            materialTypeBll.Add(model);
                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                    else
                    {
                        if (!CheckTypeName(model.MaterialTypeName, model.Id))
                        {
                            materialTypeBll.Update(model);
                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                }
            }
            return result;
        }

        bool CheckTypeName(string name, int id)
        {

            var list = materialTypeBll.GetList(s => s.MaterialTypeName == name && (id > 0 ? s.Id != id : 1 == 1));
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
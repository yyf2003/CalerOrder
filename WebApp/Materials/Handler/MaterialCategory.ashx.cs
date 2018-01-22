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
    /// MaterialCategory 的摘要说明
    /// </summary>
    public class MaterialCategory : IHttpHandler
    {
        string type = string.Empty;
        MaterialCategoryBLL categoryBll = new MaterialCategoryBLL();
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
                case "edit":
                    string jsonString = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = AddCategory(jsonString, optype);
                    break;
            }
            context.Response.Write(result);
        }

        string AddCategory(string jsonStr, string optype)
        {
            string result = "提交失败！";
            if (!string.IsNullOrEmpty(jsonStr))
            {
                Models.MaterialCategory model = JsonConvert.DeserializeObject<Models.MaterialCategory>(jsonStr);
                if (model != null)
                {
                    if (optype == "add")
                    {
                        if (!CheckName(model.CategoryName, 0))
                        {
                           
                            model.IsDelete = false;
                            categoryBll.Add(model);
                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                    else
                    {
                        if (!CheckName(model.CategoryName, model.Id))
                        {
                            categoryBll.Update(model);
                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                }
            }
            return result;
        }

        bool CheckName(string name, int id)
        {
            var model = categoryBll.GetList(s => s.CategoryName == name && (id > 0 ? s.Id != id : 1 == 1)).FirstOrDefault();
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
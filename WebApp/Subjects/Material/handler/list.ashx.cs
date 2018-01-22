using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using BLL;
using Newtonsoft.Json;

namespace WebApp.Subjects.Material.handler
{
    /// <summary>
    /// list 的摘要说明
    /// </summary>
    public class list : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string optype = context.Request.QueryString["optype"];
            string jsonstr = context.Request.QueryString["jsonstr"];
            context.Response.Write(Edit(optype, jsonstr));
        }

        string Edit(string opType, string jsonStr)
        {
            string result = "提交失败";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OrderMaterialItem newModel = null;
                try
                {
                    newModel = JsonConvert.DeserializeObject<OrderMaterialItem>(jsonStr);
                    
                }
                catch
                {
                    result = "提交失败：起始时间格式不正确";
                }

                if (newModel != null)
                {
                    OrderMaterialItemBLL bll = new OrderMaterialItemBLL();
                    if (opType == "add")
                    {
                        newModel.IsDelete = false;
                        bll.Add(newModel);
                        result = "ok";
                    }
                    else
                    {
                        OrderMaterialItem model = bll.GetModel(newModel.ItemId);
                        if (model != null)
                        {
                            model.BeginDate = newModel.BeginDate;
                            model.EndDate = newModel.EndDate;
                            model.SubjectId = newModel.SubjectId;
                            bll.Update(model);
                            result = "ok";
                        }
                    }
                }
            }
            return result;
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
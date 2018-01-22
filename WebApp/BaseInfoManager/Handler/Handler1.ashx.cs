using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using Models;
namespace WebApp.BaseInfoManager.Handler
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {

        BaseCategoryBLL categoryBll = new BaseCategoryBLL();
        BaseBLL baseBll = new BaseBLL();
        string type = string.Empty;
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
                case "getList":
                    result = GetBaseList();
                    break;
                case "getParentList":
                    //result = GetBaseCategoryList();
                    break;
                case "edit":
                    //string json = context.Request["jsonString"];
                    //string optype = context.Request["optype"];
                    //result = AddModule(json, optype);
                    break;
            }
            context.Response.Write(result);
        }
        StringBuilder json = new StringBuilder();
        string GetBaseList()
        {
            var categoryList = categoryBll.GetList(s => 1 == 1);
            if (categoryList.Any())
            {
                categoryList.ForEach(s =>
                {
                    json.Append("{\"id\":\"" + (-s.Id) + "\",\"text\":\"" + s.CategoryName + "\",\"BaseCode\":\"" + s.BaseCode + "\",\"ParentId\":\"0\"");
                    GetBaseInfo(s.Id);
                    json.Append("},");
                });
            }
            return "[" + json.ToString().TrimEnd(',') + "]";
        }

        void GetBaseInfo(int parentId)
        {
            var list1 = baseBll.GetList(s => s.BaseCategoryId == parentId);
            if (list1.Any())
            {
                json.Append(",\"state\":\"closed\",\"children\":[");
                int index = 0;
                list1.ForEach(s =>
                {
                    if (index > 0)
                    {
                        json.Append(",");
                    }
                    json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.BaseName + "\",\"BaseCode\":\"" + s.BaseCode + "\",\"ParentId\":\"" + s.BaseCategoryId + "\"}");
                    
                    index++;
                });
                json.Append("]");
            }
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
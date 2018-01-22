using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Common;
using Models;
using BLL;
using Newtonsoft.Json;

namespace WebApp.Subjects.Supplement.handler
{
    /// <summary>
    /// List 的摘要说明
    /// </summary>
    public class List : IHttpHandler
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
                case "add":
                    string optype = context.Request.QueryString["optype"];
                    string jsonstr = context.Request.QueryString["jsonstr"];
                    result=Edit(optype, jsonstr);
                    break;
                case "getModel":
                    int id = 0;
                    if (context.Request.QueryString["id"] != null)
                    {
                        id = int.Parse(context.Request.QueryString["id"]);
                    }
                    result = GetModel(id);
                    break;
            }

            context.Response.Write(result);
        }

        string Edit(string opType, string jsonStr)
        {
            string result = "提交失败";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                SupplementItem newModel = null;
                try
                {
                    newModel = JsonConvert.DeserializeObject<SupplementItem>(jsonStr);

                }
                catch
                {
                    result = "提交失败：起始时间格式不正确";
                }

                if (newModel != null)
                {
                    SupplementItemBLL bll = new SupplementItemBLL();
                    if (opType == "add")
                    {
                        newModel.IsDelete = false;
                        newModel.AddDate = DateTime.Now;
                        newModel.AddUserId = new BasePage().CurrentUser.UserId;
                        newModel.ApproveState = 0;
                        newModel.IsSubmit = 0;
                        bll.Add(newModel);
                        result = "ok";
                    }
                    else
                    {
                        SupplementItem model = bll.GetModel(newModel.SupplementId);
                        if (model != null)
                        {
                            model.BeginDate = newModel.BeginDate;
                            model.EndDate = newModel.EndDate;
                            model.Price = newModel.Price;
                            model.Reason = newModel.Reason;
                            model.SubjectId = newModel.SubjectId;
                            bll.Update(model);
                            result = "ok";
                        }
                    }
                }
            }
            return result;
        }

        string GetModel(int id)
        {
            StringBuilder json = new StringBuilder();
            SupplementItem model = new SupplementItemBLL().GetModel(id);
            if (model != null)
            {
                string begin = model.BeginDate != null ? DateTime.Parse(model.BeginDate.ToString()).ToShortDateString() : "";
                string end = model.EndDate != null ? DateTime.Parse(model.EndDate.ToString()).ToShortDateString() : "";
                json.Append("{\"SupplementId\":\"" + model.SupplementId + "\",\"BeginDate\":\"" + begin + "\",\"EndDate\":\"" + end + "\",\"SubjectId\":\"" + (model.SubjectId ?? 0) + "\",\"Price\":\"" + (model.Price ?? 0) + "\",\"Reason\":\""+model.Reason+"\"}");
                return "["+json.ToString()+"]";
            }
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
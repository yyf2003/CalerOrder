using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using BLL;
using Models;
using System.Text;
using Common;

namespace WebApp.Handler
{
    /// <summary>
    /// index 的摘要说明
    /// </summary>
    public class index : IHttpHandler
    {

        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
                type = context.Request.QueryString["type"];
            else if (context.Request.Form["type"] != null)
                type = context.Request.Form["type"];
            switch (type)
            { 
                case "getList":
                    result = GetSubjectList();
                    break;
                case "deleteMsg":
                    result = DeleteMsg();
                    break;
                case "deleteAll":
                    result = DeleteAll();
                    break;
            }
            context.Response.Write(result);
        }

        string GetSubjectList()
        {
            DateTime nowDate = DateTime.Now;
            DateTime limitDate = nowDate.AddDays(-15);
            int userId=new BasePage().CurrentUser.UserId;
            try
            {
                var list = (from subject in CurrentContext.DbContext.Subject
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on subject.GuidanceId equals guidance.ItemId
                            join customer in CurrentContext.DbContext.Customer
                            on subject.CustomerId equals customer.Id
                            where subject.Status == 4 && subject.ApproveState != null && subject.ApproveState > 0
                            && (subject.IsRead == null || subject.IsRead == false)
                            && subject.AddDate > limitDate
                            && subject.AddUserId == userId
                            select new
                            {
                                customer.CustomerName,
                                subject,
                                guidance.ItemName
                            }).OrderByDescending(s => s.subject.ApproveDate).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        json.Append("{\"Id\":\"" + s.subject.Id + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"SubjectName\":\"" + s.subject.SubjectName + "\",\"GuidanceName\":\"" + s.ItemName + "\",\"ApproveState\":\"" + s.subject.ApproveState + "\",\"AddDate\":\"" + s.subject.AddDate + "\",\"ApproveDate\":\"" + s.subject.ApproveDate + "\"},");
                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                else
                    return "";
            }
            catch (Exception ex) {
                return "";
            }
        }

        string DeleteMsg()
        {
            string result = "error";
            int subjectId=0;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            SubjectBLL bll = new SubjectBLL();
            Subject model = bll.GetModel(subjectId);
            if (model != null)
            {
                model.IsRead = true;
                bll.Update(model);
                result = "ok";
            }
            return result;
        }

        string DeleteAll()
        {
            string result = "ok";
            string subjectIds = string.Empty;
            if (context1.Request.Form["subjectIds"] != null)
            {
                subjectIds = context1.Request.Form["subjectIds"];
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    List<int> idList = StringHelper.ToIntList(subjectIds, ',');
                    SubjectBLL bll = new SubjectBLL();
                    Subject model;
                    idList.ForEach(s =>
                    {
                        model = bll.GetModel(s);
                        if (model != null)
                        {
                            model.IsRead = true;
                            bll.Update(model);
                        }
                    });
                }
            }
            catch
            {
                result = "error";
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using System.Text;

namespace WebApp.Quotation.handler
{
    /// <summary>
    /// Edit 的摘要说明
    /// </summary>
    public class Edit : IHttpHandler
    {
        int id;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["id"] != null)
            {
                id = int.Parse(context.Request.QueryString["id"]);
            }
            switch (type)
            {
                case "category":
                    result = GetQuotationCategory();
                    break;
                case "blongs":
                    result = GetQuotationBlongs();
                    break;
                case "classification":
                    result = GetQuotationClassification();
                    break;
                case "account":
                    result = GetQuotationAccount();
                    break;
                case "taxrate":
                    result = GetQuotationTaxRate();
                    break;

                case "delCategory":
                    result = DeleteQuotationCategory();
                    break;
                case "delBelongs":
                    result = DeleteQuotationBlongs();
                    break;
                case "delClassification":
                    result = DeleteQuotationClassification();
                    break;
                case "delAccount":
                    result = DeleteQuotationAccount();
                    break;
                case "delTaxRate":
                    result = DeleteQuotationTaxRate();
                    break;
                case "getProjectList":
                    int subjectId = 0;
                    string dateStr = string.Empty;
                    if (context.Request.QueryString["subjectId"] != null)
                    {
                        subjectId = int.Parse(context.Request.QueryString["subjectId"]);
                    }
                    if (context.Request.QueryString["dateStr"] != null)
                    {
                        dateStr = context.Request.QueryString["dateStr"];
                    }
                    result = GetProjectList(subjectId, dateStr);
                    break;
            }
            context.Response.Write(result);
        }

        string GetQuotationCategory()
        {
            var list = new QuotationCategoryBLL().GetList(s=>s.Id>0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"Name\":\"" + s.CategoryName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetQuotationBlongs()
        {
            var list = new QuotationBlongsBLL().GetList(s => s.Id > 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"Name\":\"" + s.BlongsName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetQuotationClassification()
        {
            var list = new QuotationClassificationBLL().GetList(s => s.Id > 0).OrderBy(s=>s.ClassificationName).ThenBy(s=>s.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"Name\":\"" + s.ClassificationName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetQuotationAccount()
        {
            var list = new QuotationAccountBLL().GetList(s => s.Id > 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"Name\":\"" + s.AccountName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetQuotationTaxRate()
        {
            var list = new QuotationTaxRateBLL().GetList(s => s.Id > 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"Name\":\"" + s.TaxRateName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string DeleteQuotationCategory()
        {
            QuotationCategoryBLL bll = new QuotationCategoryBLL();
            QuotationCategory model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                return "ok";
            }
            else
                return "error";
        }

        string DeleteQuotationClassification()
        {
            QuotationClassificationBLL bll = new QuotationClassificationBLL();
            QuotationClassification model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                return "ok";
            }
            else
                return "error";
        }

        string DeleteQuotationAccount()
        {
            QuotationAccountBLL bll = new QuotationAccountBLL();
            QuotationAccount model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                return "ok";
            }
            else
                return "error";
        }

        string DeleteQuotationTaxRate()
        {
            QuotationTaxRateBLL bll = new QuotationTaxRateBLL();
            QuotationTaxRate model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                return "ok";
            }
            else
                return "error";
        }

        string DeleteQuotationBlongs()
        {
            QuotationBlongsBLL bll = new QuotationBlongsBLL();
            QuotationBlongs model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
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

        public string GetProjectList(int subjectId, string dateStr)
        {
            int year = 0;
            int month = 0;
            if (!string.IsNullOrWhiteSpace(dateStr))
            {
                DateTime date = DateTime.Parse(dateStr);
                year = date.Year;
                month = date.Month;
            }
            else if (subjectId>0)
            { 
               Subject model = new SubjectBLL().GetModel(subjectId);
               if (model != null && model.BeginDate != null)
               {
                   year = DateTime.Parse(model.BeginDate.ToString()).Year;
                   month = DateTime.Parse(model.BeginDate.ToString()).Month;
               }
            }
            var list = new SubjectBLL().GetList(s => s.BeginDate.Value.Year == year && s.BeginDate.Value.Month == month);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"SubjectName\":\""+s.SubjectName+"\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }
    }
}
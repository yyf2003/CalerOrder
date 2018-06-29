using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using BLL;
using Models;
using System.Text;
using Newtonsoft.Json;

namespace WebApp.QuoteOrderManager.handler
{
    /// <summary>
    /// AddQuotation 的摘要说明
    /// </summary>
    public class AddQuotation : IHttpHandler
    {
        string month = string.Empty;
        string guidanceId = string.Empty;
        string subjectCategory = string.Empty;
        string subjectId = string.Empty;
        string sheet = string.Empty;
        int customerId;
        string rate = string.Empty;
        int quoteItemId;
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context1 = context;
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            if (context.Request.QueryString["month"] != null)
            {
                month = context.Request.QueryString["month"];
            }
            if (context.Request.QueryString["guidanceId"] != null)
            {
                guidanceId = context.Request.QueryString["guidanceId"];
            }
            if (context.Request.QueryString["subjectCategory"] != null)
            {
                subjectCategory = context.Request.QueryString["subjectCategory"];
            }
            if (context.Request.QueryString["subjectId"] != null)
            {
                subjectId = context.Request.QueryString["subjectId"];
            }
            if (context.Request.QueryString["rate"] != null)
            {
                rate = context.Request.QueryString["rate"];
            }
            if (context.Request.QueryString["sheet"] != null)
            {
                sheet = context.Request.QueryString["sheet"];
            }
            if (context.Request.QueryString["quoteItemId"] != null)
            {
                quoteItemId = int.Parse(context.Request.QueryString["quoteItemId"]);
            }
            //ChangeRate();
            switch (type)
            {
                case "addRate":
                    result = ChangeRate();
                    break;
                case "addQuoteDifferenceDetail":
                    result = AddQuoteDifferenceDetail();
                    break;
                case "deleteQuoteDifferenceDetail":
                    result = DeleteQuoteDifferenceDetail();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }
        List<int> guidanceIdList = new List<int>();
        List<int> categoryIdList = new List<int>();
        List<int> subjectIdList = new List<int>();

        string ChangeRate()
        {
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(subjectCategory))
            {
                categoryIdList = StringHelper.ToIntList(subjectCategory, ',');
            }
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
                List<int> hMakeSubjectIdList = new SubjectBLL().GetList(s => subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s=>s.Id).ToList();
                subjectIdList.AddRange(hMakeSubjectIdList);
            }
            else
            {
                List<Subject> subjectList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();
                if (categoryIdList.Any())
                {
                    if (categoryIdList.Contains(0))
                    {
                        categoryIdList.Remove(0);
                        if (categoryIdList.Any())
                        {
                            subjectList = subjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0) || (s.SubjectCategoryId == null || s.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            subjectList = subjectList.Where(s => s.SubjectCategoryId == null || s.SubjectCategoryId == 0).ToList();
                    }
                    else
                        subjectList = subjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0)).ToList();
                }
                subjectIdList = subjectList.Select(s => s.Id).ToList();
                subjectId = StringHelper.ListToString(subjectIdList);
            }
            subjectId = StringHelper.ListToString(subjectIdList);
            if (!string.IsNullOrWhiteSpace(subjectId) && !string.IsNullOrWhiteSpace(sheet) && !string.IsNullOrWhiteSpace(rate) && StringHelper.IsDecimalVal(rate))
            {
                List<string> sheetList = StringHelper.ToStringList(sheet, ',', LowerUpperEnum.ToLower);
                StringBuilder sheetWhere = new StringBuilder();
                sheetList.ForEach(s =>
                {
                    sheetWhere.Append("'" + s + "',");
                });
                subjectId = subjectId.TrimEnd(',');
               
                new QuoteOrderDetailBLL().UpdateRate(sheetWhere.ToString().TrimEnd(','), subjectId, StringHelper.IsDecimal(rate), quoteItemId);
               
            }
            return "ok";
        }

        string GetPOPPrice()
        {
            string result = "0";
           
            
            var orderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId??0) && (s.OrderType==(int)OrderTypeEnum.POP || s.OrderType==(int)OrderTypeEnum.道具));

            if (subjectIdList.Any())
            {
                orderList = orderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
            }
            if (quoteItemId > 0)
            {
                orderList = orderList.Where(s => (s.QuoteItemId ?? 0) == 0 || s.QuoteItemId == quoteItemId).ToList();
            }
            else
            {
                orderList = orderList.Where(s => (s.QuoteItemId ?? 0) == 0).ToList();
            }
            if (orderList.Any())
            {
                decimal price = orderList.Sum(s => ((s.AutoAddTotalPrice ?? 0) - (s.DefaultTotalPrice ?? 0)));
                result = Math.Round(price, 2).ToString();
            }
            return result;
        }

        string AddQuoteDifferenceDetail()
        {
            string result = "error|数据有误，请联系管理员";
            string jsonStr = string.Empty;
            if (context1.Request.QueryString["jsonStr"] != null)
            {
                jsonStr = context1.Request.QueryString["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                QuoteDifferenceDetail model = JsonConvert.DeserializeObject<QuoteDifferenceDetail>(jsonStr);
                if (model != null)
                {
                    if (model.QuoteItemId > 0)
                    {
                        model.AddDate = DateTime.Now;
                        model.AddUserId = new BasePage().CurrentUser.UserId;
                        new QuoteDifferenceDetailBLL().Add(model);
                        var list = new QuoteDifferenceDetailBLL().GetList(s => s.QuoteItemId == model.QuoteItemId && s.Sheet == model.Sheet && s.OrderType == model.OrderType);
                        if (list.Any())
                        {
                            //decimal totalPrice = list.Sum(s => s.AddPrice ?? 0);
                            StringBuilder json = new StringBuilder();
                            list.ForEach(s =>
                            {
                                decimal subPrice = (s.MaterialUnitPrice ?? 0) * (s.AddArea ?? 0);
                                json.Append("{\"Id\":\"" + s.Id + "\",\"AddPriceType\":\"" + s.AddPriceType + "\",\"MaterialName\":\"" + s.MaterialName + "\",\"AddArea\":\"" + s.AddArea + "\",\"SubPrice\":\"" + subPrice + "\",\"Remark\":\"" + s.Remark + "\"},");
                            
                            });


                            result = "[" + json.ToString().TrimEnd(',') + "]";
                        }
                    }
                    else
                        result = "error|请先导入报价单";
                   
                }
            }
            return result;
        }

        string DeleteQuoteDifferenceDetail()
        {
            string result = "error";
            int id = 0;
            if (context1.Request.QueryString["id"] != null)
            {
                id = int.Parse(context1.Request.QueryString["id"]);
            }
            QuoteDifferenceDetailBLL bll = new QuoteDifferenceDetailBLL();
            QuoteDifferenceDetail model = bll.GetModel(id);
            if (model != null)
            {
                int quoteItemId = model.QuoteItemId??0;
                bll.Delete(model);
                //result = "ok";
                var list = bll.GetList(s => s.QuoteItemId == quoteItemId && s.Sheet == model.Sheet && s.OrderType == model.OrderType);
                if (list.Any())
                {
                    //decimal totalPrice = list.Sum(s => s.AddPrice ?? 0);
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        //json.Append("{\"Id\":\"" + s.Id + "\",\"AddPriceType\":\"" + s.AddPriceType + "\",\"AddPrice\":\"" + s.AddPrice + "\",\"Remark\":\"" + s.Remark + "\"},");
                        decimal subPrice = (s.MaterialUnitPrice ?? 0) * (s.AddArea ?? 0);
                        json.Append("{\"Id\":\"" + s.Id + "\",\"AddPriceType\":\"" + s.AddPriceType + "\",\"MaterialName\":\"" + s.MaterialName + "\",\"AddArea\":\"" + s.AddArea + "\",\"SubPrice\":\"" + subPrice + "\",\"Remark\":\"" + s.Remark + "\"},");
                            
                    });


                    result = "[" + json.ToString().TrimEnd(',') + "]";
                }
                else
                    result = "";
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
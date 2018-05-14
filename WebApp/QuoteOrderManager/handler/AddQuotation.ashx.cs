using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using BLL;

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
        int customerId;
        string rate = string.Empty;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
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
            ChangeRate();
            context.Response.Write(GetPOPPrice());
        }

        void ChangeRate()
        {
            if (!string.IsNullOrWhiteSpace(subjectId) && !string.IsNullOrWhiteSpace(rate) && StringHelper.IsDecimalVal(rate))
            {
                subjectId = subjectId.TrimEnd(',');
                decimal newRate = StringHelper.IsDecimal(rate)/100;
                newRate = Math.Round(newRate, 2) + 1;
                new QuoteOrderDetailBLL().UpdateRate(subjectId,newRate);
            }
        }

        string GetPOPPrice()
        {
            string result = "0";
            List<int> guidanceIdList = new List<int>();
            List<int> categoryIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
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
            }
            var orderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId??0) && (s.OrderType==(int)OrderTypeEnum.POP || s.OrderType==(int)OrderTypeEnum.道具));
            if (subjectIdList.Any())
            {
                orderList = orderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
            }
            if (orderList.Any())
            {
                decimal price = orderList.Sum(s => ((s.AutoAddTotalPrice ?? 0) - (s.DefaultTotalPrice ?? 0)));
                result = Math.Round(price, 2).ToString();
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
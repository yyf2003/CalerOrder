using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using Models;
using DAL;
namespace WebApp.Statistics.handler
{
    /// <summary>
    /// SubjectStatistics 的摘要说明
    /// </summary>
    public class SubjectStatistics : IHttpHandler
    {
        int subjectId;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context.Request.QueryString["subjectId"]);
            }
            switch (type)
            { 
                case "getRegion":
                    result = GetRegion();
                    break;
            }
            context.Response.Write(result);
        }

        string GetRegion()
        {
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            where order.SubjectId == subjectId
                            select new
                            {
                                shop.RegionName
                            }).Distinct().ToList();
            if (orderList.Any())
            {
                StringBuilder jsonStr = new StringBuilder();
                orderList.ForEach(s => {
                    jsonStr.Append("{\"RegionName\":\""+s.RegionName+"\"},");
                });
                return "[" + jsonStr.ToString().TrimEnd(',') + "]";
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
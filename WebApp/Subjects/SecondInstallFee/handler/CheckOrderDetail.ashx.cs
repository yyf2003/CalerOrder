using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using System.Text;
using Common;

namespace WebApp.Subjects.SecondInstallFee.handler
{
    /// <summary>
    /// CheckOrderDetail 的摘要说明
    /// </summary>
    public class CheckOrderDetail : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            else if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            switch (type)
            {
                case "getList":
                    result = GetOrderList();
                    break;

                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetOrderList()
        {
            string result = string.Empty;
            int subjectId = 0;
            //int shopId = 0;
            string shopNo = string.Empty;
            string shopName = string.Empty;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }
            if (context1.Request.QueryString["shopName"] != null)
            {
                shopName = context1.Request.QueryString["shopName"];
            }
            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where order.SubjectId == subjectId
                       
                        select new
                        {
                            order,
                            shop

                        }).OrderBy(s => s.order.ShopId).ThenBy(s => s.order.OrderType).ThenBy(s => s.order.Sheet).ToList();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.shop.ShopNo.ToUpper().Contains(shopNo.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(shopName))
            {
                list = list.Where(s => s.shop.ShopName.ToUpper().Contains(shopName.ToUpper())).ToList();
            }
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    string type = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                    string addDate = string.Empty;
                    if (s.order.AddDate != null)
                        addDate = s.order.AddDate.ToString();
                    string Quantity = (s.order.Quantity ?? 1).ToString();

                    json.Append("{\"Id\":\"" + s.order.Id + "\",\"OrderType\":\"" + type + "\",\"ShopId\":\"" + s.order.ShopId + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"RegionName\":\"" + s.shop.RegionName + "\",\"ProvinceName\":\"" + s.shop.ProvinceName + "\",\"CityName\":\"" + s.shop.CityName + "\",\"MaterialSupport\":\"" + s.order.MaterialSupport + "\",\"Channel\":\"" + s.shop.Channel + "\",\"Format\":\"" + s.shop.Format + "\",\"Sheet\":\"" + s.order.Sheet + "\",\"GraphicNo\":\"" + s.order.GraphicNo + "\",\"Gender\":\"" + s.order.Gender + "\",\"Quantity\":\"" + Quantity + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",\"ChooseImg\":\"" + s.order.ChooseImg + "\",\"Remark\":\"" + s.order.Remark + "\",\"AddDate\":\"" + addDate + "\",\"Price\":\"" + s.order.Price + "\",\"PayPrice\":\"" + s.order.PayPrice + "\"},");

                });
                
                result = "[" + json.ToString().TrimEnd(',') + "]";
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
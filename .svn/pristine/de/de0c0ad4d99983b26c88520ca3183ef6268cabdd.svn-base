using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using System.Text;

namespace WebApp.Subjects.NewShopSubject.handler
{
    /// <summary>
    /// AddOrderDetail 的摘要说明
    /// </summary>
    public class AddOrderDetail : IHttpHandler
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
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                       join shop in CurrentContext.DbContext.Shop
                       on order.ShopId equals shop.Id
                       where order.SubjectId == subjectId
                       select new { 
                          order,
                          shop
                       }).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"Id\",\"" + s.order.Id + "\",\"ShopId\":\"" + s.order.ShopId + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"RegionName\":\"" + s.shop.RegionName + "\",\"ProvinceName\":\"" + s.shop.ProvinceName + "\",\"CityName\":\"" + s.shop.CityName + "\",\"MaterialSupport\":\"" + s.order.MaterialSupport + "\",\"POSScale\":\"" + s.order.POSScale + "\",\"Sheet\":\"" + s.order.Sheet + "\",\"" + s.order.Sheet + "\",\"GraphicNo\":\"" + s.order.GraphicNo + "\",\"Gender\":\"" + s.order.Gender + "\",\"Quantity\":\"" + s.order.Quantity + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",\"Remark\":\"" + s.order.Remark + "\"},");

                });
                result = "["+json.ToString().TrimEnd(',')+"]";
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
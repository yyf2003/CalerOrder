using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using DAL;

namespace WebApp.Subjects.Supplement.handler
{
    /// <summary>
    /// EditOrder 的摘要说明
    /// </summary>
    public class EditOrder : IHttpHandler
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
                case "getModel":
                    int id = int.Parse(context.Request.QueryString["id"]);
                    result=GetModel(id);
                    break;
            }
            context.Response.Write(result);
        }

        string GetModel(int id)
        {
            var model = (from order in CurrentContext.DbContext.SupplementDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.Id == id
                        select new { 
                           order,
                           shop.ShopNo
                        }).FirstOrDefault();
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                json.Append("{\"Id\":\"" + model.order.Id + "\",\"ShopId\":\"" + (model.order.ShopId ?? 0) + "\",\"ShopNo\":\"" + model.ShopNo + "\",\"Sheet\":\"" + model.order.Sheet + "\",\"LevelNum\":\""+model.order.LevelNum+"\",\"Gender\":\"" + model.order.Gender + "\",\"Quantity\":\"" + model.order.Quantity + "\",\"GraphicWidth\":\"" + model.order.GraphicWidth + "\",\"GraphicLength\":\"" + model.order.GraphicLength + "\",\"Area\":\"" + model.order.Area + "\",\"ChooseImg\":\"" + model.order.ChooseImg + "\",\"GraphicMaterial\":\"" + model.order.GraphicMaterial + "\",\"UnitPrice\":\"" + model.order.UnitPrice + "\",\"PositionDescription\":\"" + model.order.PositionDescription + "\"}");
                return "[" + json.ToString() + "]";
            }
            else
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
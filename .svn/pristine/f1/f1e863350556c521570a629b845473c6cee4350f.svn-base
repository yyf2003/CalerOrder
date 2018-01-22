using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using System.Text;
using Newtonsoft.Json;

namespace WebApp.Subjects.NewShopSubject.handler
{
    /// <summary>
    /// AddShop 的摘要说明
    /// </summary>
    public class AddShop : IHttpHandler
    {
        HttpContext context1;
        ShopBLL shopBll = new ShopBLL();
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
                case "getShopList":
                    result = GetShopList();
                    break;
                case "editShop":
                    result = EditShop();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetShopList()
        { 
            string result=string.Empty;
            //int subjectId = 0;
            //if (context1.Request.Form["subjectId"] != null)
            //{
            //    subjectId = int.Parse(context1.Request.Form["subjectId"]);
            //}
            //var list = shopBll.GetList(s => s.SubjectId == subjectId);
            //if (list.Any())
            //{
            //    StringBuilder json = new StringBuilder();
            //    list.ForEach(s => {
            //        string opendate = string.Empty;
            //        if (s.OpeningDate != null)
            //        {
            //            opendate = DateTime.Parse(s.OpeningDate.ToString()).ToShortDateString();
            //        }
            //        json.Append("{\"ShopId\":\"" + s.Id + "\",\"ShopNo\":\"" + s.ShopNo + "\",\"ShopName\":\"" + s.ShopName + "\",\"RegionName\":\"" + s.RegionName + "\",\"ProvinceName\":\"" + s.ProvinceName + "\",\"CityName\":\"" + s.CityName + "\",\"CityTier\":\"" + s.CityTier + "\",\"IsInstall\":\"" + s.IsInstall + "\",\"Channel\":\"" + s.Channel + "\",\"Format\":\"" + s.Format + "\",\"OpenDate\":\"" + opendate + "\"},");
            //    });
            //    result = "["+json.ToString().TrimEnd(',')+"]";
            //}
            return result;
        }

        string EditShop()
        {
            string result = "提交失败！";
            string jsonString = string.Empty;
            if (context1.Request.Form["jsonString"] != null)
            {
                jsonString = context1.Request.Form["jsonString"];
                jsonString = HttpUtility.UrlDecode(jsonString);
            }
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                Shop model = JsonConvert.DeserializeObject<Shop>(jsonString);
                if (model != null)
                {
                    if (!CheckShop(model))
                    {
                        if (model.Id > 0)
                        {
                            Shop newModel = shopBll.GetModel(model.Id);
                            if (newModel != null)
                            {
                                model.AddDate = newModel.AddDate;
                                model.AddUserId = newModel.AddUserId;
                                model.CustomerId = newModel.CustomerId;
                                model.IsDelete = newModel.IsDelete;
                                //model.Remark = newModel.Remark;
                                shopBll.Update(model);
                                result = "ok";
                            }
                        }
                        else
                        {
                            model.AddDate = DateTime.Now;
                            model.AddUserId = new BasePage().CurrentUser.UserId;
                            model.IsDelete = false;
                            shopBll.Add(model);
                            result = "ok";
                        }
                    }
                    else
                    {
                        result = "exist";
                    }
                }
            }
            return result;
        }

        bool CheckShop(Shop model)
        {

            var list = shopBll.GetList(s => s.ShopNo.ToLower() == model.ShopNo.ToLower() && (model.Id > 0 ? (s.Id != model.Id) : 1 == 1));
            return list.Any();
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
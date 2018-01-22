using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using System.Text;

namespace WebApp.Customer.Handler
{
    /// <summary>
    /// POPBlackListManage 的摘要说明
    /// </summary>
    public class POPBlackListManage : IHttpHandler
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
            switch (type)
            {
                case "getList":
                    int currPage = 0;
                    int pageSize = 0;
                    if (context.Request.QueryString["currpage"] != null)
                    {
                        currPage = int.Parse(context.Request.QueryString["currpage"]);
                    }
                    if (context.Request.QueryString["pagesize"] != null)
                    {
                        pageSize = int.Parse(context.Request.QueryString["pagesize"]);
                    }
                    result = GetList(currPage, pageSize);
                    break;
                case "edit":
                    string jsonStr = context.Request.QueryString["jsonstr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = jsonStr.Replace("+", "%2B");
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    result = Edit(jsonStr);
                    break;
                case "delete":
                    string ids = string.Empty;
                    if (context.Request.QueryString["ids"] != null)
                        ids = context.Request.QueryString["ids"];
                    result = Delete(ids);
                    break;
                default:
                    break;
            }

            context.Response.Write(result);
        }

        string GetList(int currPage, int pageSize)
        {
            var list = (from pop in CurrentContext.DbContext.POPBlackList
                       join shop in CurrentContext.DbContext.Shop
                       on pop.ShopId equals shop.Id
                       select new {
                           pop,
                          shop
                       }).ToList();
            string shopNo = string.Empty;
            string shopName = string.Empty;
            if (context1.Request.QueryString["shopNo"] != null && !string.IsNullOrWhiteSpace(context1.Request.QueryString["shopNo"]))
            {
                shopNo = context1.Request.QueryString["shopNo"];
                list = list.Where(s => s.shop.ShopNo.ToLower() == shopNo.ToLower()).ToList();
            }
            if (context1.Request.QueryString["shopName"] != null && !string.IsNullOrWhiteSpace(context1.Request.QueryString["shopName"]))
            {
                shopName = context1.Request.QueryString["shopName"];
                list = list.Where(s => s.shop.ShopName.ToLower().Contains(shopName.ToLower())).ToList();
            }
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = list.Count;
                list = list.OrderBy(s => s.shop.Id).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                list.ForEach(s =>
                {
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.pop.Id + "\",\"ShopId\":\"" + s.pop.ShopId + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"RegionName\":\"" + s.shop.RegionName + "\",\"ProvinceName\":\"" + s.shop.ProvinceName + "\",\"CityName\":\"" + s.shop.CityName + "\",\"Sheet\":\"" + s.pop.Sheet + "\",\"GraphicNo\":\"" + s.pop.GraphicNo + "\"},");
                    index++;
                });
                if (json.Length > 0)
                    return "{\"total\":" + totalCount + ",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
                else
                    return "{\"total\":0,\"rows\":[] }";
            }
            else
                return "{\"total\":0,\"rows\":[] }";
        }

        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                POPBlackListBLL bll = new POPBlackListBLL();
                POPBlackList model = JsonConvert.DeserializeObject<POPBlackList>(jsonStr);
                if (model != null && !string.IsNullOrWhiteSpace(model.GraphicNo))
                {
                    string GraphicNo = model.GraphicNo.Replace('，', ',').TrimStart(',').TrimEnd(',').ToUpper();
                    List<string> newGraphicNoList = StringHelper.ToStringList(GraphicNo, ',').OrderBy(s => s).ToList();
                    if (model.Id > 0)
                    {
                        
                        var model1 = bll.GetModel(model.Id);
                        if (model1 != null)
                        {
                            model1.GraphicNo = StringHelper.ListToString(newGraphicNoList);
                            bll.Update(model1);
                        }
                    }
                    else
                    {

                        if (!string.IsNullOrWhiteSpace(model.ShopNo))
                        {
                            Shop shopModel = new ShopBLL().GetList(s => s.ShopNo.ToLower() == model.ShopNo.ToLower()).FirstOrDefault();
                            if (shopModel != null)
                            {
                                var model0 = bll.GetList(s =>s.ShopId==shopModel.Id && s.Sheet == model.Sheet).FirstOrDefault();
                                if (model0 != null)
                                {
                                    
                                    List<string> oldGraphicNoList = StringHelper.ToStringList(model0.GraphicNo, ',');
                                    oldGraphicNoList.AddRange(newGraphicNoList);
                                    oldGraphicNoList = oldGraphicNoList.Distinct().OrderBy(s => s).ToList();
                                    model0.GraphicNo = StringHelper.ListToString(oldGraphicNoList);
                                    bll.Update(model0);
                                }
                                else
                                {
                                    model.ShopNo = shopModel.ShopNo;
                                    model.GraphicNo = StringHelper.ListToString(newGraphicNoList);
                                    model.ShopId = shopModel.Id;
                                    bll.Add(model);
                                }
                                
                            }
                            else
                                result = "店铺编号不存在！";
                        }
                        else
                            result = "店铺编号为空";
                    }
                }
                else
                    result = "提交失败";
            }
            else
                result = "提交失败";
            return result;
        }

        string Delete(string ids)
        {
            string r = "删除失败";
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids,',');
                new POPBlackListBLL().Delete(s => idList.Contains(s.Id));
                r = "ok";
            }
            return r;
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
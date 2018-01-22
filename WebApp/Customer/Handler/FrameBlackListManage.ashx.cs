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
    /// FrameBlackListManage 的摘要说明
    /// </summary>
    public class FrameBlackListManage : IHttpHandler
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
                case "getCornerType":
                    result = GetCornerType();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }


        string GetList(int currPage, int pageSize)
        {
            try
            {
                var list = (from pop in CurrentContext.DbContext.MachineFrameBlackList
                            join shop in CurrentContext.DbContext.Shop
                            on pop.ShopId equals shop.Id
                            select new
                            {
                                pop,
                                shop
                            }).ToList();

                if (context1.Request.QueryString["sheet"] != null && !string.IsNullOrWhiteSpace(context1.Request.QueryString["sheet"]))
                {
                    string sheet = string.Empty;
                    sheet = context1.Request.QueryString["sheet"];
                    list = list.Where(s => s.pop.Sheet.ToLower().Contains(sheet.ToLower())).ToList();
                }
                if (context1.Request.QueryString["shopNo"] != null && !string.IsNullOrWhiteSpace(context1.Request.QueryString["shopNo"]))
                {
                    string shopNo = string.Empty;
                    shopNo = context1.Request.QueryString["shopNo"];
                    list = list.Where(s => s.shop.ShopNo.ToLower() == shopNo.ToLower()).ToList();
                }
                if (context1.Request.QueryString["shopName"] != null && !string.IsNullOrWhiteSpace(context1.Request.QueryString["shopName"]))
                {
                    string shopName = string.Empty;
                    shopName = context1.Request.QueryString["shopName"];
                    list = list.Where(s => s.shop.ShopName.ToLower().Contains(shopName.ToLower())).ToList();
                }
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    int totalCount = list.Count;
                    list = list.OrderBy(s => s.pop.ShopId).ThenBy(s => s.pop.Sheet).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                    int index = 1;
                    list.ForEach(s =>
                    {
                        json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.pop.Id + "\",\"ShopId\":\"" + s.pop.ShopId + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"RegionName\":\"" + s.shop.RegionName + "\",\"ProvinceName\":\"" + s.shop.ProvinceName + "\",\"CityName\":\"" + s.shop.CityName + "\",\"CountyName\":\"" + s.shop.AreaName + "\",\"Sheet\":\"" + s.pop.Sheet + "\",\"Gender\":\"" + s.pop.Gender + "\",\"CornerType\":\""+s.pop.CornerType+"\"},");
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
            catch (Exception ex)
            {
                return "";
            }
        }

        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                MachineFrameBlackList model = JsonConvert.DeserializeObject<MachineFrameBlackList>(jsonStr);
                if (model != null)
                {
                    //string sheet = model.Sheet;
                    //string shopNos = model.ShopNo;
                    //int shopId = 0;
                    MachineFrameBlackListBLL bll = new MachineFrameBlackListBLL();
                    if (model.Id > 0)
                    {
                        var model1 = bll.GetModel(model.Id);
                        if (model1 != null)
                        {
                            model1.Sheet = model.Sheet.ToUpper();
                            model1.Gender = model.Gender;
                            model1.CornerType = model.CornerType;
                            bll.Update(model1);
                        }
                    }
                    else
                    {
                        StringBuilder errorMsg = new StringBuilder();
                        if (!string.IsNullOrWhiteSpace(model.ShopNo))
                        {
                           
                            Shop shopModel = new ShopBLL().GetList(s => s.ShopNo.ToLower() == model.ShopNo.ToLower()).FirstOrDefault();
                            if (shopModel != null)
                            {
                                bool isNew = false;
                                var oldList = bll.GetList(s => s.ShopId == shopModel.Id && s.Sheet.ToUpper() == model.Sheet.ToUpper());
                                if (!string.IsNullOrWhiteSpace(model.CornerType))
                                {
                                    oldList = oldList.Where(s => s.CornerType == model.CornerType).ToList();
                                }
                                else
                                {
                                    oldList = oldList.Where(s => s.CornerType == null || s.CornerType == "").ToList();
                                }
                                if (oldList.Any())
                                {
                                    if (model.Gender != null && model.Gender.Contains("男") && model.Gender.Contains("女"))
                                    {
                                        oldList.ForEach(s =>
                                        {
                                            bll.Delete(s);
                                        });
                                        isNew = true;
                                    }
                                    else
                                    {
                                        var model0 = oldList.Where(s=>s.Gender==model.Gender || (s.Gender.Contains("男") && s.Gender.Contains("女"))).FirstOrDefault();
                                        if (model0 != null)
                                        {
                                            result = "重复添加！";
                                        }
                                        else
                                            isNew = true;

                                    }
                                }
                                else
                                    isNew = true;
                                if (isNew)
                                {
                                    model.ShopId = shopModel.Id;
                                    model.Sheet = model.Sheet.ToUpper();
                                    bll.Add(model);
                                }
                            }
                            else
                                result = "店铺编号不存在！";
                            
                            
                            
                        }
                        else
                            result = "店铺编号为空！";
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
                List<int> idList = StringHelper.ToIntList(ids, ',');
                new MachineFrameBlackListBLL().Delete(s => idList.Contains(s.Id));
                r = "ok";
            }
            return r;
        }

        bool CheckShop(string shopNo,out int shopId)
        {
            shopId = 0;
            Shop shopModel = new ShopBLL().GetList(s => s.ShopNo.ToLower() == shopNo.ToLower()).FirstOrDefault();
            if (shopModel != null)
            {
                shopId = shopModel.Id;
                return true;
            }
            return false;
           
        }

        string GetCornerType()
        {
            var list = new MachineFrameCornerTypeBLL().GetList(s=>s.Id>0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"CornerType\":\""+s.CornerTypeName+"\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
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
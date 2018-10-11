using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Models;
using BLL;
using DAL;
using Common;
using Newtonsoft.Json;

namespace WebApp.Outsource.InstallPriceLevelSetting
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            string type = string.Empty;
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
                case "getRegion":
                    result = GetRegionList();
                    break;
                case "getSettingList":
                    result = GetSettingList();
                    break;
                case "getCity":
                    result = GetCityList();
                    break;
                case "add":
                    result = AddSetting();
                    break;
                case "delete":
                    result = DeleteSetting();
                    break;
                case "edit":
                    result = EditSetting();
                    break;
            }
            context.Response.Write(result);
        }

        string GetRegionList()
        {
            List<Region> list = new RegionBLL().GetList(s=>s.IsDelete==null || s.IsDelete==false);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"RegionId\":\""+s.Id+"\",\"RegionName\":\""+s.RegionName+"\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

        string GetSettingList()
        {
            string result = string.Empty;
            int regionId = 0;
            if (context1.Request.QueryString["regionId"] != null)
            {
                regionId = int.Parse(context1.Request.QueryString["regionId"]);
            }
            List<OutsourceIPSetting> settingList = new List<OutsourceIPSetting>();
            OutsourceIPSetting settingModel;
            var list = (from setting in CurrentContext.DbContext.OutsourceInstallPriceLevel
                       join region in CurrentContext.DbContext.Region
                       on setting.RegionId equals region.Id
                       join customer in CurrentContext.DbContext.Customer
                       on setting.CustomerId equals customer.Id
                       join province1 in CurrentContext.DbContext.Place
                       on setting.ProvinceId equals province1.ID into proTemp
                       from province in proTemp.DefaultIfEmpty()
                       join city1 in CurrentContext.DbContext.Place
                       on setting.CityId equals city1.ID into cityTemp
                       from city in cityTemp.DefaultIfEmpty()
                       where regionId>0?(setting.RegionId == regionId):1==1
                       select new {
                           setting,
                           region.RegionName,
                           province,
                           city,
                           customer.CustomerName
                       }).OrderBy(s => s.setting.RegionId).ThenBy(s => s.setting.ProvinceId).ThenBy(s => s.setting.CityId).ThenBy(s => s.setting.BasicMaterialSupportId).ToList();
            if (list.Any())
            {
                list.ForEach(s => {
                    settingModel = settingList.SingleOrDefault(p => p.RegionId == s.setting.RegionId && p.ProvinceId == (s.setting.ProvinceId??0) && p.CityId == (s.setting.CityId??0));
                    if (settingModel != null)
                    {
                        switch (s.setting.MaterialSupport.ToLower())
                        {
                            case "basic":
                                settingModel.BasicPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "premium":
                                settingModel.PremiumPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "vvip":
                                settingModel.VVIPPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "mcs":
                                settingModel.MCSPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "others":
                                settingModel.OthersPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "generic":
                                settingModel.GenericPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                        }
                    }
                    else
                    {
                        settingModel = new OutsourceIPSetting();
                        settingModel.CustomerId = s.setting.CustomerId ?? 0;
                        settingModel.CustomerName = s.CustomerName;
                        settingModel.BasicMaterialSupportId = s.setting.BasicMaterialSupportId??0;
                        settingModel.City =s.city!=null? s.city.PlaceName:"";
                        settingModel.CityId = s.setting.CityId??0;
                        settingModel.MaterialSupport = s.setting.MaterialSupport;
                        settingModel.Province =s.province!=null? s.province.PlaceName:"";
                        settingModel.ProvinceId = s.setting.ProvinceId ?? 0;
                        settingModel.Region = s.RegionName;
                        settingModel.RegionId = s.setting.RegionId ?? 0;
                        switch (s.setting.MaterialSupport.ToLower())
                        {
                            case "basic":
                                settingModel.BasicPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "premium":
                                settingModel.PremiumPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "vvip":
                                settingModel.VVIPPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "mcs":
                                settingModel.MCSPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "others":
                                settingModel.OthersPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                            case "generic":
                                settingModel.GenericPrice = s.setting.BasicInstallPrice ?? 0;
                                break;
                        }
                        settingList.Add(settingModel);
                    }
                });

            }
            if (settingList.Any())
            {
                StringBuilder json = new StringBuilder();
                int index = 1;
                settingList.ForEach(s => {
                    json.Append("{\"RowIndex\":\"" + index + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"CustomerId\":\""+s.CustomerId+"\",\"RegionId\":\"" + s.RegionId + "\",\"RegionName\":\"" + s.Region + "\",\"ProvinceId\":\"" + s.ProvinceId + "\",\"ProvinceName\":\"" + s.Province + "\",\"CityId\":\"" + s.CityId + "\",\"CityName\":\"" + s.City + "\",\"BasicPrice\":\"" + s.BasicPrice + "\",\"PremiumPrice\":\"" + s.PremiumPrice + "\",\"VVIPPrice\":\"" + s.VVIPPrice + "\",\"MCSPrice\":\"" + s.MCSPrice + "\",\"OthersPrice\":\"" + s.OthersPrice + "\",\"GenericPrice\":\"" + s.GenericPrice + "\"},");
                    index++;
                });
                result = "{\"code\":0,\"msg\":\"\",\"count\":" + settingList.Count + ",\"data\":[" + json.ToString().TrimEnd(',') + "]}";
            }
            if (string.IsNullOrWhiteSpace(result))
            {
                result = "{\"code\":0,\"msg\":\"\",\"count\":0,\"data\":[]}";
            }
            return result;
        }

        string GetCityList()
        {
            List<int> provinceIdList = new List<int>();
            if (context1.Request.QueryString["provinceId"] != null)
            {
                string provinceId = context1.Request.QueryString["provinceId"];
                provinceIdList = StringHelper.ToIntList(provinceId,',');
            }
            List<Place> cityList = new PlaceBLL().GetList(s => provinceIdList.Contains(s.ParentID??0) && s.ParentID>0);
            if (cityList.Any())
            {
                StringBuilder json = new StringBuilder();
                cityList.ForEach(s => {
                    json.Append("{\"PlaceId\":\"" + s.ID + "\",\"PlaceName\":\"" + s.PlaceName + "\",\"ParentId\":\"" + (s.ParentID ?? 0) + "\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }

        string AddSetting()
        {
            string result = "ok";
            int settingType = 1;
            if (context1.Request.Form["settingType"] != null)
            {
                settingType = int.Parse(context1.Request.Form["settingType"]);
            }
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                try
                {
                    OutsourceInstallPriceLevelBLL settingBll = new OutsourceInstallPriceLevelBLL();
                    //OutsourceInstallPriceLevel settingModel;
                    List<OutsourceInstallPriceLevel> settingModelListAdd = JsonConvert.DeserializeObject<List<OutsourceInstallPriceLevel>>(jsonStr);

                    if (settingModelListAdd.Any())
                    {
                        int customerId = settingModelListAdd[0].CustomerId ?? 0;
                        if (settingType == 1)
                        {
                            List<int> provinceIdList = settingModelListAdd.Select(s => s.ProvinceId ?? 0).Distinct().ToList();
                            settingBll.Delete(s => s.CustomerId == customerId && provinceIdList.Contains(s.ProvinceId ?? 0) && (s.CityId ?? 0) == 0);
                            settingModelListAdd.ForEach(s =>
                            {
                                settingBll.Add(s);
                            });
                        }
                        else
                        {
                            List<int> cityIdList = settingModelListAdd.Select(s => s.CityId ?? 0).Distinct().ToList();
                            settingBll.Delete(s => s.CustomerId == customerId && cityIdList.Contains(s.CityId ?? 0) && (s.CityId ?? 0)>0);
                            settingModelListAdd.ForEach(s =>
                            {
                                settingBll.Add(s);
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = "提交失败：" + ex.Message;
                }
            }
            return result;
        }

        string DeleteSetting()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                try
                {
                    List<OutsourceInstallPriceLevel> list = JsonConvert.DeserializeObject<List<OutsourceInstallPriceLevel>>(jsonStr);
                    if (list.Any())
                    {
                        OutsourceInstallPriceLevelBLL bll = new OutsourceInstallPriceLevelBLL();
                        list.ForEach(s =>
                        {
                            int customerId = s.CustomerId ?? 0;
                            int provinceId = s.ProvinceId ?? 0;
                            int cityId = s.CityId ?? 0;
                            if (customerId > 0 && provinceId > 0)
                            {
                                if (cityId > 0)
                                {
                                    bll.Delete(b => b.CustomerId == customerId && b.ProvinceId == provinceId && b.CityId == cityId);
                                }
                                else
                                {
                                    bll.Delete(b => b.CustomerId == customerId && b.ProvinceId == provinceId && (b.CityId ?? 0) == 0);
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    result = "删除失败："+ex.Message;
                }
            }
            return result;
        }

        string EditSetting()
        {
            string result = "更新失败";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OutsourceInstallPriceLevel editModel = JsonConvert.DeserializeObject<OutsourceInstallPriceLevel>(jsonStr);
                if (editModel != null)
                {
                    OutsourceInstallPriceLevelBLL bll = new OutsourceInstallPriceLevelBLL();
                    var list = bll.GetList(s=>s.CustomerId==editModel.CustomerId && s.ProvinceId==editModel.ProvinceId && s.MaterialSupport.ToLower()==editModel.MaterialSupport.ToLower());
                    if ((editModel.CityId ?? 0) > 0)
                    {
                        list = list.Where(s=>s.CityId==editModel.CityId).ToList();
                    }
                    OutsourceInstallPriceLevel model = list.FirstOrDefault();
                    if (model!=null)
                    {
                        model.BasicInstallPrice = editModel.BasicInstallPrice;
                        bll.Update(model);
                        result = "ok";
                    }
                }
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
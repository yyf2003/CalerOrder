using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using System.Text;
using Newtonsoft.Json;
using Common;
using Models;
using System.Transactions;

namespace WebApp.OutsourcingOrder.handler
{
    /// <summary>
    /// AssignConfig 的摘要说明
    /// </summary>
    public class AssignConfig : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
                type = context.Request.QueryString["type"];
            else if (context.Request.Form["type"] != null)
                type = context.Request.Form["type"];
            switch (type)
            {
                case "getList":
                    result = GetList();
                    break;
                case "getConfigType":
                    result = GetConfigTypeList();
                    break;
                case "getCustomer":
                    result = GetCustomerList();
                    break;
                case "getRegion":
                    int customerId = int.Parse(context.Request.QueryString["customerId"].ToString());
                    result = GetRegions(customerId);
                    break;
                case "getProvince":
                    int regionId = 0;
                    if (context.Request.QueryString["regionId"] != null)
                    {
                        regionId = int.Parse(context.Request.QueryString["regionId"].ToString());

                    }
                    result = GetProvince(regionId);
                    break;
                case "getCity":
                    string provinceId = string.Empty;
                    if (context.Request.QueryString["provinceId"] != null)
                    {
                        provinceId = context.Request.QueryString["provinceId"].ToString();
                    }
                    result = GetCity(provinceId);
                    break;
                case "edit":
                    string jsonString = context.Request["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        jsonString = HttpUtility.UrlDecode(jsonString);
                    }
                    result = Edit(jsonString);
                    break;
                case "getChannel":
                    result = GetChannelList();
                    break;
                case "getFormat":
                    string channels = string.Empty;
                    if (context.Request.QueryString["channel"] != null)
                    {
                        channels = context.Request.QueryString["channel"].ToString();
                    }
                    result = GetFormatList(channels);
                    break;
                case "delete":
                    string ids = context.Request["ids"];
                    result = Delete(ids);
                    break;
            }
            context.Response.Write(result);
        }

        string GetList()
        {

           
            var list = (from config in CurrentContext.DbContext.OutsourceOrderAssignConfig
                        join region in CurrentContext.DbContext.Region
                        on config.Region equals region.Id
                        join company in CurrentContext.DbContext.Company
                        on config.ProductOutsourctId equals company.Id into companyTemp
                        from outsource in companyTemp.DefaultIfEmpty()
                        select new
                        {
                            config,
                            outsource.CompanyName,
                            region.RegionName
                        }).OrderBy(s => s.config.ConfigTypeId).ToList();
            if (list.Any())
            {
                OutsourceOrderPlaceConfigBLL placeConfigBll = new OutsourceOrderPlaceConfigBLL();
                StringBuilder json = new StringBuilder();
                PlaceBLL placeBll = new PlaceBLL();
                int index = 1;
                list.ForEach(s =>
                {
                    string cityIds = string.Empty;
                    List<int> provinceIdList = new List<int>();
                    List<int> cityIdList = new List<int>();
                    string cityName = string.Empty;
                    string provinceName = string.Empty;
                    var placeConfigList = placeConfigBll.GetList(p=>p.ConfigId==s.config.Id);
                    if (placeConfigList.Any())
                    {
                        provinceIdList = placeConfigList.Select(p => p.PrivinceId ?? 0).ToList();
                        List<string> cityList = placeConfigList.Select(p => p.CityIds).ToList();
                        if (cityList.Any())
                        {
                            cityIdList = StringHelper.ToIntList(StringHelper.ListToString(cityList),',');
                        }
                    }
                    
                    int typeId = s.config.ConfigTypeId ?? 0;
                    string typeName = CommonMethod.GetEnumDescription<OutsourceOrderConfigType>(typeId.ToString());
                    if (provinceIdList.Any())
                    {
                        List<string> cityNameList = placeBll.GetList(c => provinceIdList.Contains(c.ID)).Select(c => c.PlaceName).ToList();
                        provinceName = StringHelper.ListToString(cityNameList);
                    }
                    if (cityIdList.Any())
                    {
                        List<string> cityNameList = placeBll.GetList(c => cityIdList.Contains(c.ID)).Select(c => c.PlaceName).ToList();
                        cityName = StringHelper.ListToString(cityNameList);
                        cityIds = StringHelper.ListToString(cityIdList);
                    }
                    json.Append("{\"Id\":\"" + s.config.Id + "\",\"RowIndex\":\"" + (index++) + "\",\"CustomerId\":\"" + s.config.CustomerId + "\",\"TypeId\":\"" + typeId + "\",\"TypeName\":\"" + typeName + "\",\"OutsourctId\":\"" + (s.config.ProductOutsourctId ?? 0) + "\",\"OutsourctName\":\"" + s.CompanyName + "\",\"MaterialName\":\"" + s.config.MaterialName + "\",\"RegionId\":\"" + s.config.Region + "\",\"RegionName\":\"" + s.RegionName + "\",\"ProvinceId\":\"" + StringHelper.ListToString(provinceIdList) + "\",\"ProvinceName\":\"" + provinceName + "\",\"CityId\":\"" + cityIds + "\",\"CityName\":\"" + cityName + "\",\"Channel\":\"" + s.config.Channel + "\",\"Format\":\"" + s.config.Format + "\",\"IsFullMatch\":\""+((s.config.IsFullMatch??false)?1:0)+"\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";

        }

        string GetConfigTypeList()
        {
            var list = CommonMethod.GetEnumList<OutsourceOrderConfigType>();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"TypeId\":\"" + s.Value + "\",\"TypeName\":\"" + s.Desction + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetCustomerList()
        {
            StringBuilder customerJson = new StringBuilder();
            var list = new CustomerBLL().GetList(s => s.IsDelete == false || s.IsDelete == null);
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    customerJson.Append("{\"CustomerId\":\"" + s.Id + "\",\"CustomerName\":\"" + s.CustomerName + "\"},");
                });
                return "[" + customerJson.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetRegions(int customerId)
        {
            var list = new RegionBLL().GetList(s => s.CustomerId == customerId).OrderBy(s => s.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"RegionName\":\"" + s.RegionName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetProvince(int regionId)
        {
            var list = (from pr in CurrentContext.DbContext.ProvinceInRegion
                        join region in CurrentContext.DbContext.Region
                        on pr.RegionId equals region.Id
                        join province in CurrentContext.DbContext.Place
                        on pr.ProvinceId equals province.ID
                        where pr.RegionId == regionId
                        select new
                        {
                            province,
                            region
                        }).OrderBy(s => s.region.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"RegionId\":\"" + s.region.Id + "\",\"RegionName\":\"" + s.region.RegionName + "\",\"ProvinceId\":\"" + s.province.ID + "\",\"ProvinceName\":\"" + s.province.PlaceName + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetProvince()
        {
            var list = new PlaceBLL().GetList(s => s.ParentID == 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"ProvinceId\":\"" + s.ID + "\",\"ProvinceName\":\"" + s.PlaceName + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetCity(string parentId)
        {
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                List<int> parentIdList = StringHelper.ToIntList(parentId, ',');
                var list = (from place in CurrentContext.DbContext.Place
                            join parent in CurrentContext.DbContext.Place
                            on place.ParentID equals parent.ID
                            where parentIdList.Contains(place.ParentID ?? 0)
                            select new
                            {
                                place.ParentID,
                                ParentName = parent.PlaceName,
                                place.ID,
                                place.PlaceName
                            }).OrderBy(s => s.ParentID).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        json.Append("{\"ParentId\":\"" + s.ParentID + "\",\"ParentName\":\"" + s.ParentName + "\",\"PlaceId\":\"" + s.ID + "\",\"PlaceName\":\"" + s.PlaceName + "\"},");

                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                return "";
            }
            return "";
        }

        string Edit(string jsonStr)
        {
            string result = "提交失败";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OutsourceOrderAssignConfig model = JsonConvert.DeserializeObject<OutsourceOrderAssignConfig>(jsonStr);
                OutsourceOrderAssignConfigBLL bll = new OutsourceOrderAssignConfigBLL();

                OutsourceOrderPlaceConfig placeConfigModel;
                OutsourceOrderPlaceConfigBLL placeConfigBll = new OutsourceOrderPlaceConfigBLL();

                if (model != null)
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        try
                        {
                            model.AddDate = DateTime.Now;
                            string provinceId = model.ProvinceId;
                            string cityId = model.CityId;
                            model.ProvinceId = "";
                            model.CityId = "";
                            if (model.Id > 0)
                            {
                                bll.Update(model);
                            }
                            else
                                bll.Add(model);
                            int configId = model.Id;
                            placeConfigBll.Delete(s => s.ConfigId == configId);
                            if (!string.IsNullOrWhiteSpace(provinceId))
                            {
                                List<int> provinceIdList = StringHelper.ToIntList(provinceId, ',');
                                List<string> cityIdList = new List<string>();
                                if (!string.IsNullOrWhiteSpace(cityId))
                                {
                                    cityIdList = StringHelper.ToStringList(cityId, ',');
                                }
                                provinceIdList.ForEach(province =>
                                {
                                    List<int> cityIdList0 = new List<int>();
                                    cityIdList.ForEach(city =>
                                    {
                                        int pid = int.Parse(city.Split(':')[0]);
                                        int cid = int.Parse(city.Split(':')[1]);
                                        if (pid == province)
                                            cityIdList0.Add(cid);
                                    });
                                    placeConfigModel = new Models.OutsourceOrderPlaceConfig();
                                    placeConfigModel.ConfigId = configId;
                                    placeConfigModel.PrivinceId = province;
                                    if (cityIdList0.Any())
                                        placeConfigModel.CityIds = StringHelper.ListToString(cityIdList0);
                                    placeConfigBll.Add(placeConfigModel);

                                });
                            }
                            tran.Complete();
                            result = "ok";
                        }
                        catch (Exception ex)
                        {
                            result = "提交失败："+ex.Message;
                        }
                    }
                    
                }
                
            }
            return result;
        }

        string Delete(string ids)
        {
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids, ',');
                new OutsourceOrderAssignConfigBLL().Delete(s => idList.Contains(s.Id));
                return "ok";
            }
            return "删除失败";
        }

        string GetChannelList()
        {
            List<string> list = new ShopBLL().GetList(s => s.IsDelete == null || s.IsDelete == false).Select(s=>s.Channel).Distinct().OrderBy(s=>s).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        json.Append("{\"Channel\":\""+s+"\"},");
                    }
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            else
                return "";
        }

        string GetFormatList(string channels)
        {
            if (!string.IsNullOrWhiteSpace(channels))
            {
                channels = channels.Replace("，", ",");
                List<string> channelList = StringHelper.ToStringList(channels, ',', LowerUpperEnum.ToUpper);
                if (channelList.Any())
                {
                    List<string> list = new ShopBLL().GetList(s => (s.IsDelete == null || s.IsDelete == false) && s.Channel!=null && channelList.Contains(s.Channel.ToUpper())).Select(s => s.Format).Distinct().OrderBy(s => s).ToList();
                    if (list.Any())
                    {
                        StringBuilder json = new StringBuilder();
                        list.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                json.Append("{\"Format\":\"" + s + "\"},");
                            }
                        });
                        return "[" + json.ToString().TrimEnd(',') + "]";
                    }
                    else
                        return "";
                }
                else
                    return "";
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
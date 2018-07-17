using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using Common;

namespace WebApp.Customer.Handler
{
    /// <summary>
    /// Shops 的摘要说明
    /// </summary>
    public class Shops : IHttpHandler
    {
        int shopId;
        ShopBLL shopBll = new ShopBLL();
        //Shop model;
        int customerId;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["customerid"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerid"]);
            }
            switch (type)
            {
                case "checkshop":
                    shopId = int.Parse(context.Request.QueryString["shopid"]);
                    result = GetShopModel(shopId);
                    break;
                case "GetRegion":
                    result = GetRegion();
                    break;
                case "GetProvince":
                    string region = context.Request.QueryString["region"];
                    result = GetProvince(region);
                    break;
                case "GetCity":
                    string province = context.Request.QueryString["province"];
                    result = GetCity(province);
                    break;
                case "edit":
                    string jsonString = context.Request.QueryString["jsonString"];
                    result = UpdateShop(jsonString);
                    break;
                case "add":
                    string jsonString0 = context.Request.QueryString["jsonString"];
                    result = AddShop(jsonString0);
                    break;
                case "GetCSUser"://获取客服
                    string regionName = string.Empty;
                    string provinceName = string.Empty;
                    if (context.Request.QueryString["region"] != null)
                        regionName = context.Request.QueryString["region"];
                    if (context.Request.QueryString["province"] != null)
                        provinceName = context.Request.QueryString["province"];
                    result = GetCSUserList(regionName, provinceName);
                    break;
                case "bindRegion":
                    result = BindRegion();
                    break;
                case "bindProvince":
                    int regionId = 0;
                    if (context.Request.QueryString["regionId"] != null)
                        regionId = int.Parse(context.Request.QueryString["regionId"]);
                    result = BindProvince(regionId);
                    break;
                case "bindCity":
                    int provinceId = 0;
                    if (context.Request.QueryString["provinceId"] != null)
                        provinceId = int.Parse(context.Request.QueryString["provinceId"]);
                    result = BindCity(provinceId);
                    break;
                case "bindArea":
                    int cityId = 0;
                    if (context.Request.QueryString["cityId"] != null)
                        cityId = int.Parse(context.Request.QueryString["cityId"]);
                    result = BindCity(cityId);
                    break;
                case "getOutsource":
                    result = GetOutsourceList();
                    break;
                case "getChannel":
                    result = GetChannel();
                    break;
                case "getFormat":
                    string channel = string.Empty;
                    if (context.Request.QueryString["channel"] != null)
                        channel = context.Request.QueryString["channel"];
                    result = GetFormat(channel);
                    break;
            }
            context.Response.Write(result);
        }

        string GetShopModel(int id)
        {
            var model1 = (from shop in CurrentContext.DbContext.Shop
                          join customer in CurrentContext.DbContext.Customer
                          on shop.CustomerId equals customer.Id
                          join user1 in CurrentContext.DbContext.UserInfo
                          on shop.CSUserId equals user1.UserId into userTemp
                          from user in userTemp.DefaultIfEmpty()
                          join company1 in CurrentContext.DbContext.Company
                          on shop.OutsourceId equals company1.Id into companyTemp
                          from company in companyTemp.DefaultIfEmpty()
                          join company2 in CurrentContext.DbContext.Company
                          on shop.OOHInstallOutsourceId equals company2.Id into companyTemp2
                          from oohcompany in companyTemp2.DefaultIfEmpty()
                          join company3 in CurrentContext.DbContext.Company
                          on shop.BCSOutsourceId equals company3.Id into companyTemp3
                          from bcscompany in companyTemp3.DefaultIfEmpty()
                          join company4 in CurrentContext.DbContext.Company
                          on shop.ProductOutsourceId equals company4.Id into companyTemp4
                          from productcompany in companyTemp4.DefaultIfEmpty()
                          where shop.Id == id
                          select new
                          {
                              shop,
                              customer.CustomerName,
                              CSUserName = user.RealName,
                              OutsourceName = company.CompanyName,
                              OOHOutsourceName = oohcompany.CompanyName,
                              BCSOutsourctName=bcscompany.CompanyName,
                              ProductOutsourctName = productcompany.CompanyName
                          }).FirstOrDefault();
            if (model1 != null)
            {
                StringBuilder json = new StringBuilder();
                string opendate = string.Empty;
                if (model1.shop.OpeningDate != null)
                {
                    opendate = DateTime.Parse(model1.shop.OpeningDate.ToString()).ToShortDateString();
                }
                string Status = !string.IsNullOrWhiteSpace(model1.shop.Status) ? model1.shop.Status : "正常";
                if (new BasePage().CurrentUser.RoleId == 2)
                    Status = "正常";
                json.Append("{\"ShopName\":\"" + model1.shop.ShopName + "\",\"ShopNo\":\"" + model1.shop.ShopNo + "\",\"CustomerId\":\"" + (model1.shop.CustomerId ?? 0) + "\",\"Customer\":\"" + model1.CustomerName + "\",\"RegionName\":\"" + model1.shop.RegionName + "\",\"ProvinceName\":\"" + model1.shop.ProvinceName + "\",\"CityName\":\"" + model1.shop.CityName + "\",\"AreaName\":\"" + model1.shop.AreaName + "\",\"CityTier\":\"" + model1.shop.CityTier + "\",\"IsInstall\":\"" + model1.shop.IsInstall + "\",\"AgentCode\":\"" + model1.shop.AgentCode + "\",\"AgentName\":\"" + model1.shop.AgentName + "\",\"POPAddress\":\"" + model1.shop.POPAddress + "\",\"Contact1\":\"" + model1.shop.Contact1 + "\",\"Tel1\":\"" + model1.shop.Tel1 + "\",\"Contact2\":\"" + model1.shop.Contact2 + "\",\"Tel2\":\"" + model1.shop.Tel2 + "\",\"Channel\":\"" + model1.shop.Channel + "\",\"Format\":\"" + model1.shop.Format + "\",\"LocationType\":\"" + model1.shop.LocationType + "\",\"BusinessModel\":\"" + model1.shop.BusinessModel + "\",\"OpeningDate\":\"" + opendate + "\",\"Status\":\"" + Status + "\",\"CSUserId\":\"" + model1.shop.CSUserId + "\",\"CSUserName\":\"" + model1.CSUserName + "\",\"Remark\":\"" + model1.shop.Remark + "\",\"InstallPrice\":\"" + model1.shop.InstallPrice + "\",\"WindowInstallPrice\":\"" + model1.shop.WindowInstallPrice + "\",\"POSScale\":\"" + model1.shop.POSScale + "\",\"BasicInstallPrice\":\"" + model1.shop.BasicInstallPrice + "\",\"ProvinceId\":\"" + (model1.shop.ProvinceId ?? 0) + "\",\"CityId\":\"" + (model1.shop.CityId ?? 0) + "\",\"AreaId\":\"" + (model1.shop.AreaId ?? 0) + "\",\"ShopType\":\"" + model1.shop.ShopType + "\",\"BCSInstallPrice\":\"" + model1.shop.BCSInstallPrice + "\",\"OutsourceInstallPrice\":\"" + model1.shop.OutsourceInstallPrice + "\",\"OutsourceBCSInstallPrice\":\"" + model1.shop.OutsourceBCSInstallPrice + "\",\"OutsourceName\":\"" + model1.OutsourceName + "\",\"OutsourceId\":\"" + (model1.shop.OutsourceId ?? 0) + "\",\"BCSIsInstall\":\"" + model1.shop.BCSIsInstall + "\",\"OOHOutsourceName\":\"" + model1.OOHOutsourceName + "\",\"OOHInstallOutsourceId\":\"" + (model1.shop.OOHInstallOutsourceId ?? 0) + "\",\"BCSOutsourceId\":\"" + (model1.shop.BCSOutsourceId ?? 0) + "\",\"BCSOutsourctName\":\"" + model1.BCSOutsourctName + "\",\"ProductOutsourceId\":\"" + (model1.shop.ProductOutsourceId ?? 0) + "\",\"ProductOutsourctName\":\"" + model1.ProductOutsourctName + "\",\"GenericIsInstall\":\"" + model1.shop.GenericIsInstall + "\",\"GenericInstallPrice\":\"" + model1.shop.GenericInstallPrice + "\",\"OutsourceGenericInstallPrice\":\"" + model1.shop.OutsourceGenericInstallPrice + "\"}");
                return "[" + json.ToString() + "]";
            }
            return "";
        }

        string GetRegion()
        {
            var list = shopBll.GetList(s => s.CustomerId == customerId).Select(s => s.RegionName).Distinct().ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"RegionName\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetProvince(string region)
        {
            region = region.ToLower();
            var list = shopBll.GetList(s => s.RegionName.ToLower() == region).Select(s => s.ProvinceName).Distinct().ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"ProvinceName\":\"" + s + "\"},");
                });
                return "[" + json.ToString() + "]";
            }
            else
                return "";
        }

        string GetCity(string province)
        {
            province = province.ToLower();
            var list = shopBll.GetList(s => s.ProvinceName.ToLower() == province).Select(s => s.CityName).Distinct().ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"CityName\":\"" + s + "\"},");
                });
                return "[" + json.ToString() + "]";
            }
            else
                return "";
        }

        string UpdateShop(string jsonString)
        {
            string result = "提交失败！";
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    Shop model = JsonConvert.DeserializeObject<Shop>(jsonString);
                    if (model != null)
                    {
                        BaseDataChangeLogBLL changeLogBll = new BaseDataChangeLogBLL();
                        ShopChangeDetailBLL detailBll = new ShopChangeDetailBLL();
                        ShopChangeDetail shopChangeDetailModel;
                        Shop newModel = shopBll.GetModel(model.Id);
                        bool canGo = true;
                        if (newModel.IsDelete == null || newModel.IsDelete == false)
                        {
                            canGo = !CheckShop(model);
                        }
                        if (canGo)
                        {
                            if ((model.ProductOutsourceId ?? 0) > 0 && (model.OutsourceId ?? 0) > 0 && model.ProductOutsourceId == model.OutsourceId)
                            {
                                canGo = false;
                                result = "生产外协与主外协不能相同";
                            }
                        }
                        if (canGo)
                        {

                            if (newModel != null)
                            {
                                model.AddDate = newModel.AddDate;
                                model.AddUserId = newModel.AddUserId;
                                model.CustomerId = newModel.CustomerId;
                                model.IsDelete = newModel.IsDelete;
                                
                                shopBll.Update(model);

                                BaseDataChangeLog logModel = new BaseDataChangeLog();
                                logModel.AddDate = DateTime.Now;
                                logModel.AddUserId = new BasePage().CurrentUser.UserId;
                                logModel.ItemType = (int)BaseDataChangeItemEnum.Shop;
                                logModel.ChangeType = (int)DataChangeTypeEnum.Edit;
                                logModel.ShopId = model.Id;
                                changeLogBll.Add(logModel);


                                shopChangeDetailModel = new ShopChangeDetail();
                                shopChangeDetailModel.ShopId = newModel.Id;
                                shopChangeDetailModel.AgentCode = newModel.AgentCode;
                                shopChangeDetailModel.AgentName = newModel.AgentName;
                                shopChangeDetailModel.AreaName = newModel.AreaName;
                                shopChangeDetailModel.BasicInstallPrice = newModel.BasicInstallPrice;
                                shopChangeDetailModel.BusinessModel = newModel.BusinessModel;
                                shopChangeDetailModel.Category = newModel.Category;
                                shopChangeDetailModel.Channel = newModel.Channel;
                                shopChangeDetailModel.CityName = newModel.CityName;
                                shopChangeDetailModel.CityTier = newModel.CityTier;
                                shopChangeDetailModel.Contact1 = newModel.Contact1;
                                shopChangeDetailModel.Contact2 = newModel.Contact2;
                                shopChangeDetailModel.CustomerId = newModel.CustomerId;
                                shopChangeDetailModel.Format = newModel.Format;
                                shopChangeDetailModel.IsInstall = newModel.IsInstall;
                                shopChangeDetailModel.LocationType = newModel.LocationType;
                                shopChangeDetailModel.LogId = logModel.Id;
                                shopChangeDetailModel.POPAddress = newModel.POPAddress;
                                shopChangeDetailModel.ProvinceName = newModel.ProvinceName;
                                shopChangeDetailModel.RegionName = newModel.RegionName;
                                shopChangeDetailModel.Remark = newModel.Remark;
                                shopChangeDetailModel.ShopName = newModel.ShopName;
                                shopChangeDetailModel.ShopNo = newModel.ShopNo;
                                shopChangeDetailModel.ShopType = newModel.ShopType;
                                shopChangeDetailModel.Status = newModel.Status;
                                shopChangeDetailModel.Tel1 = newModel.Tel1;
                                shopChangeDetailModel.Tel2 = newModel.Tel2;
                                shopChangeDetailModel.CSUserId = newModel.CSUserId;
                                shopChangeDetailModel.ChangeType = "修改前";
                                shopChangeDetailModel.AddDate = DateTime.Now;
                                shopChangeDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                shopChangeDetailModel.BCSInstallPrice = newModel.BCSInstallPrice;
                                detailBll.Add(shopChangeDetailModel);


                                shopChangeDetailModel = new ShopChangeDetail();
                                shopChangeDetailModel.ShopId = model.Id;
                                shopChangeDetailModel.AgentCode = model.AgentCode;
                                shopChangeDetailModel.AgentName = model.AgentName;
                                shopChangeDetailModel.AreaName = model.AreaName;
                                shopChangeDetailModel.BasicInstallPrice = model.BasicInstallPrice;
                                shopChangeDetailModel.BusinessModel = model.BusinessModel;
                                shopChangeDetailModel.Category = model.Category;
                                shopChangeDetailModel.Channel = model.Channel;
                                shopChangeDetailModel.CityName = model.CityName;
                                shopChangeDetailModel.CityTier = model.CityTier;
                                shopChangeDetailModel.Contact1 = model.Contact1;
                                shopChangeDetailModel.Contact2 = model.Contact2;
                                shopChangeDetailModel.CustomerId = model.CustomerId;
                                shopChangeDetailModel.Format = model.Format;
                                shopChangeDetailModel.IsInstall = model.IsInstall;
                                shopChangeDetailModel.LocationType = model.LocationType;
                                shopChangeDetailModel.LogId = logModel.Id;
                                shopChangeDetailModel.POPAddress = model.POPAddress;
                                shopChangeDetailModel.ProvinceName = model.ProvinceName;
                                shopChangeDetailModel.RegionName = model.RegionName;
                                shopChangeDetailModel.Remark = model.Remark;
                                shopChangeDetailModel.ShopName = model.ShopName;
                                shopChangeDetailModel.ShopNo = model.ShopNo;
                                shopChangeDetailModel.ShopType = model.ShopType;
                                shopChangeDetailModel.Status = model.Status;
                                shopChangeDetailModel.Tel1 = model.Tel1;
                                shopChangeDetailModel.Tel2 = model.Tel2;
                                shopChangeDetailModel.CSUserId = model.CSUserId;
                                shopChangeDetailModel.ChangeType = "修改后";
                                shopChangeDetailModel.AddDate = DateTime.Now.AddSeconds(1);
                                shopChangeDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                shopChangeDetailModel.BCSInstallPrice = model.BCSInstallPrice;

                                detailBll.Add(shopChangeDetailModel);
                                result = "ok";
                            }
                        }
                        else
                        {
                            result = "exist";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }

        string AddShop(string jsonString)
        {
            string result = "提交失败！";
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    Shop model = JsonConvert.DeserializeObject<Shop>(jsonString);
                    if (model != null)
                    {
                        BaseDataChangeLogBLL changeLogBll = new BaseDataChangeLogBLL();
                        ShopChangeDetailBLL detailBll = new ShopChangeDetailBLL();
                        ShopChangeDetail shopChangeDetailModel;
                        bool canGo = true;
                        if (!string.IsNullOrWhiteSpace(model.ShopNo))
                        {
                            Shop newModel = shopBll.GetList(s => s.ShopNo.ToLower() == model.ShopNo.ToLower()).FirstOrDefault();
                            if (newModel != null)
                            {
                                canGo = false;
                                result = "exist";
                            }
                            else if ((model.ProductOutsourceId ?? 0) > 0 && (model.OutsourceId ?? 0) > 0 && model.ProductOutsourceId == model.OutsourceId)
                            {
                                canGo = false;
                                result = "生产外协与主外协不能相同";
                            }
                        }
                        else
                        {
                            canGo = false;
                            result = "店铺编号为空";
                        }
                        if (canGo)
                        {

                            model.AddDate = DateTime.Now;
                            model.AddUserId = new BasePage().CurrentUser.UserId;
                            model.CustomerId = model.CustomerId;
                            model.ShopNo = model.ShopNo.ToUpper();
                            shopBll.Add(model);

                            BaseDataChangeLog logModel = new BaseDataChangeLog();
                            logModel.AddDate = DateTime.Now;
                            logModel.AddUserId = new BasePage().CurrentUser.UserId;
                            logModel.ItemType = (int)BaseDataChangeItemEnum.Shop;
                            logModel.ChangeType = (int)DataChangeTypeEnum.Add;
                            logModel.ShopId = model.Id;
                            changeLogBll.Add(logModel);



                            shopChangeDetailModel = new ShopChangeDetail();
                            shopChangeDetailModel.ShopId = model.Id;
                            shopChangeDetailModel.AgentCode = model.AgentCode;
                            shopChangeDetailModel.AgentName = model.AgentName;
                            shopChangeDetailModel.AreaName = model.AreaName;
                            shopChangeDetailModel.BasicInstallPrice = model.BasicInstallPrice;
                            shopChangeDetailModel.BusinessModel = model.BusinessModel;
                            shopChangeDetailModel.Category = model.Category;
                            shopChangeDetailModel.Channel = model.Channel;
                            shopChangeDetailModel.CityName = model.CityName;
                            shopChangeDetailModel.CityTier = model.CityTier;
                            shopChangeDetailModel.Contact1 = model.Contact1;
                            shopChangeDetailModel.Contact2 = model.Contact2;
                            shopChangeDetailModel.CustomerId = model.CustomerId;
                            shopChangeDetailModel.Format = model.Format;
                            shopChangeDetailModel.IsInstall = model.IsInstall;
                            shopChangeDetailModel.LocationType = model.LocationType;
                            shopChangeDetailModel.LogId = logModel.Id;
                            shopChangeDetailModel.POPAddress = model.POPAddress;
                            shopChangeDetailModel.ProvinceName = model.ProvinceName;
                            shopChangeDetailModel.RegionName = model.RegionName;
                            shopChangeDetailModel.Remark = model.Remark;
                            shopChangeDetailModel.ShopName = model.ShopName;
                            shopChangeDetailModel.ShopNo = model.ShopNo;
                            shopChangeDetailModel.ShopType = model.ShopType;
                            shopChangeDetailModel.Status = model.Status;
                            shopChangeDetailModel.Tel1 = model.Tel1;
                            shopChangeDetailModel.Tel2 = model.Tel2;
                            shopChangeDetailModel.CSUserId = model.CSUserId;
                            shopChangeDetailModel.ChangeType = "新增";
                            shopChangeDetailModel.AddDate = DateTime.Now.AddSeconds(1);
                            shopChangeDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            shopChangeDetailModel.BCSInstallPrice = model.BCSInstallPrice;
                            detailBll.Add(shopChangeDetailModel);
                            result = "ok";

                        }

                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }

        bool CheckShop(Shop model)
        {
            var list = shopBll.GetList(s => s.ShopNo.ToLower() == model.ShopNo.ToLower() && s.Id != model.Id && (s.IsDelete == null || s.IsDelete == false));
            return list.Any();
        }

        string GetCSUserList(string regionName, string provinceName)
        {
            var list = (from ur in CurrentContext.DbContext.UserInRegion
                        join user in CurrentContext.DbContext.UserInfo
                        on ur.UserId equals user.UserId
                        join userRole in CurrentContext.DbContext.UserInRole
                        on user.UserId equals userRole.UserId
                        where userRole.RoleId == 5
                        && (user.IsDelete == null || user.IsDelete == false)
                        select new
                        {

                            user,
                            ur.RegionId,
                            ur.ProvinceId,
                            ur.CityId,
                            ur.AreaId
                        }).ToList();
            List<UserInfo> userList = new List<UserInfo>();
            if (!string.IsNullOrWhiteSpace(regionName))
            {
                int regionId = 0;
                var region = new RegionBLL().GetList(s => s.CustomerId == customerId && s.RegionName.ToLower() == regionName.ToLower()).FirstOrDefault();
                if (region != null)
                {
                    regionId = region.Id;
                }

                list.ForEach(s =>
                {
                    bool flag = false;
                    if (!string.IsNullOrWhiteSpace(s.RegionId))
                    {
                        if (StringHelper.ToIntList(s.RegionId, ',').Contains(regionId))
                        {
                            flag = true;
                        }
                    }
                    if (flag && !userList.Contains(s.user))
                        userList.Add(s.user);
                });

            }
            if (!string.IsNullOrWhiteSpace(provinceName))
            {
                int provinceId = 0;
                var province = new PlaceBLL().GetList(s => s.PlaceName == provinceName && s.AreaDeep == 2).FirstOrDefault();
                if (province != null)
                {
                    provinceId = province.ID;
                }
                list.ForEach(s =>
                {
                    bool flag = false;
                    if (!string.IsNullOrWhiteSpace(s.ProvinceId))
                    {
                        if (StringHelper.ToIntList(s.ProvinceId, ',').Contains(provinceId))
                        {
                            flag = true;
                        }
                    }
                    if (flag && !userList.Contains(s.user))
                        userList.Add(s.user);
                });
            }
            if (userList.Any())
            {
                StringBuilder json = new StringBuilder();
                userList.ForEach(s =>
                {
                    json.Append("{\"UserId\":\"" + s.UserId + "\",\"RealName\":\"" + s.RealName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string BindRegion()
        {
            var list = new RegionBLL().GetList(s => s.CustomerId == customerId);
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

        string BindProvince(int regionId)
        {
            var list = (from pir in CurrentContext.DbContext.ProvinceInRegion
                        join province in CurrentContext.DbContext.Place
                        on pir.ProvinceId equals province.ID
                        where pir.RegionId == regionId
                        select province).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.ID + "\",\"ProvinceName\":\"" + s.PlaceName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string BindCity(int provinceId)
        {
            var list = new PlaceBLL().GetList(s => s.ParentID == provinceId);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.ID + "\",\"PlaceName\":\"" + s.PlaceName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetOutsourceList()
        {
            //var list = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.IsDelete == null || s.IsDelete == false));
            var list = new CompanyBLL().GetList(s =>s.ParentId>0 && (s.IsDelete == null || s.IsDelete == false));
            
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"OutsourceName\":\"" + s.CompanyName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetChannel()
        {
            var list = new ShopBLL().GetList(s => s.Id > 0).Select(s => s.Channel).Distinct().OrderBy(s => s).ToList();
            List<string> channelList = new List<string>();
            StringBuilder json = new StringBuilder();
            list.ForEach(s =>
            {
                if (!string.IsNullOrWhiteSpace(s) && !channelList.Contains(s))
                {
                    channelList.Add(s);
                    json.Append("{\"Channel\":\"" + s + "\"},");
                }
            });
            if (json.Length > 0)
            {
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetFormat(string channel)
        {
            var list = new ShopBLL().GetList(s => s.Channel != null && s.Channel.ToUpper() == channel).Select(s => s.Format).Distinct().OrderBy(s => s).ToList();
            List<string> formatList = new List<string>();
            StringBuilder json = new StringBuilder();
            list.ForEach(s =>
            {
                if (!string.IsNullOrWhiteSpace(s) && !formatList.Contains(s))
                {
                    formatList.Add(s);
                    json.Append("{\"Format\":\"" + s + "\"},");
                }
            });
            if (json.Length > 0)
            {
                return "[" + json.ToString().TrimEnd(',') + "]";
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
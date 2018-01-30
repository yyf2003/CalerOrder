﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using BLL;
using Common;
using DAL;
using Models;
using NPOI.SS.UserModel;
using System.Data;
using System.Web.SessionState;


namespace WebApp.Subjects.Handler
{
    /// <summary>
    /// ExportOrders 的摘要说明
    /// </summary>
    /// 
    public class ExportOrders : IHttpHandler, IRequiresSessionState
    {
        string type = string.Empty;
        FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
        int selectType = 1;
        string subjectId = string.Empty;
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        string customerService = string.Empty;
        string isInstall = string.Empty;
        string materialCategory = string.Empty;
        string searchType = string.Empty;
        HttpContext context1;
        int roleId = new BasePage().CurrentUser.RoleId;
        int userId = new BasePage().CurrentUser.UserId;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string result = string.Empty;

            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["selecttype"] != null)
            {
                selectType = int.Parse(context.Request.QueryString["selecttype"]);
            }
            if (context.Request.QueryString["subjectids"] != null)
            {
                subjectId = context.Request.QueryString["subjectids"];
            }
            if (context.Request.QueryString["regions"] != null)
            {
                region = context.Request.QueryString["regions"];
            }
            if (context.Request.QueryString["province"] != null)
            {
                province = context.Request.QueryString["province"];
            }
            if (context.Request.QueryString["city"] != null)
            {
                city = context.Request.QueryString["city"];
            }
            if (context.Request.QueryString["customerService"] != null)
            {
                customerService = context.Request.QueryString["customerService"];
            }
            if (context.Request.QueryString["isInstall"] != null)
            {
                isInstall = context.Request.QueryString["isInstall"];
            }
            if (context.Request.QueryString["materialCategory"] != null)
            {
                materialCategory = context.Request.QueryString["materialCategory"];
            }
            if (context.Request.QueryString["searchType"] != null)
            {
                searchType = context.Request.QueryString["searchType"];
            }
            switch (type)
            {
                case "getregion":
                    result = GetRegions();
                    break;
                case "getprovince":
                    result = GetProvince();
                    break;
                case "getcity":
                    result = GetCity();
                    break;
                case "getlist":
                    int currpage = int.Parse(context.Request.QueryString["currpage"]);
                    int pagesize = int.Parse(context.Request.QueryString["pagesize"]);

                    result = GetOrderDetailList(currpage, pagesize);
                    break;
                case "export":

                    ExportOrder();
                    break;
                case "export350":

                    Export350();
                    break;
                case "exportbjphw":
                    ExportBJPHW();
                    break;
                case "exportotherphw":
                    ExportOtherPHW();
                    break;
                case "checkEmptyInfo":
                    string region = string.Empty;
                    string guidanceId = string.Empty;
                    if (context.Request.QueryString["region"] != null)
                    {
                        region = context.Request.QueryString["region"];
                    }
                    if (context.Request.QueryString["guidanceId"] != null)
                    {
                        guidanceId = context.Request.QueryString["guidanceId"];
                    }
                    result = CheckEmpty(guidanceId, region);
                    break;
                case "getCS"://获取客服
                    result = GetCustomerService();
                    break;
                case "getInstallLevel":
                    result = GetInstallLevel();
                    break;
                case "getMaterialCategory":
                    result = GetMaterialCategory();
                    break;
            }
            context.Response.Write(result);
        }

        string GetRegions()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                try
                {
                    List<int> sidList = StringHelper.ToIntList(subjectId, ',');

                    var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                                && (order.IsDelete == null || order.IsDelete == false)
                                select new
                                {
                                    order.Region
                                }
                                  ).ToList();
                    List<string> regionList = new List<string>();
                    if (list.Any())
                    {

                        regionList = list.Select(s => s.Region).Distinct().ToList();

                    }
                    var materialOrderList = (from order in CurrentContext.DbContext.OrderMaterial
                                             join shop in CurrentContext.DbContext.Shop
                                             on order.ShopId equals shop.Id
                                             where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                                             select new
                                             {
                                                 shop.RegionName
                                             }
                                  ).ToList();
                    if (materialOrderList.Any())
                    {
                        materialOrderList.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s.RegionName) && !regionList.Contains(s.RegionName))
                            {
                                regionList.Add(s.RegionName);
                            }
                        });
                    }
                    List<string> myRegion = new BasePage().GetResponsibleRegion;
                    if (myRegion.Any())
                    {
                        StringHelper.ToUpperOrLowerList(ref myRegion, LowerUpperEnum.ToLower);
                        regionList = regionList.Where(s => myRegion.Contains(s.ToLower())).ToList();
                    }
                    if (regionList.Any())
                    {
                        StringBuilder json = new StringBuilder();
                        regionList.ForEach(s =>
                        {
                            json.Append("{\"RegionName\":\"" + s + "\"},");
                        });
                        return "[" + json.ToString().TrimEnd(',') + "]";
                    }

                    else
                        return "";
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
            else
                return "";
        }

        string GetProvince()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<int> sidList = StringHelper.ToIntList(subjectId, ',');
                List<string> regionList = new List<string>();
                if (!string.IsNullOrWhiteSpace(region))
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                //else
                //{
                //    regionList = new BasePage().GetResponsibleRegion;
                //    if (regionList.Any())
                //    {
                //        StringHelper.ToUpperOrLowerList(ref regionList, LowerUpperEnum.ToLower);

                //    }
                //}

                //var list = orderBll.GetList(s => sidList.Contains(s.SubjectId ?? 0) && regionList.Contains(s.Region)).OrderBy(s => s.Region).ThenBy(s => s.Province).ToList();
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                             && (order.IsDelete == null || order.IsDelete == false)
                            select new
                            {
                                order,
                                shop
                            }).ToList();

                if (regionList.Any())
                {
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();
                        }
                        else
                            list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                    }
                    else
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                }

                List<string> provincelist = new List<string>();
                if (list.Any())
                {
                    provincelist = list.Select(s => s.order.Province).Distinct().ToList();

                }
                var materialOrderList = (from order in CurrentContext.DbContext.OrderMaterial
                                         join shop in CurrentContext.DbContext.Shop
                                         on order.ShopId equals shop.Id
                                         where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                                         select shop).ToList();
                if (regionList.Any())
                {
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            materialOrderList = materialOrderList.Where(s => regionList0.Contains(s.RegionName.ToLower()) || s.RegionName == null || s.RegionName == "").ToList();
                        }
                        else
                            materialOrderList = materialOrderList.Where(s => s.RegionName == null || s.RegionName == "").ToList();
                    }
                    else
                        materialOrderList = materialOrderList.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                }
                if (materialOrderList.Any())
                {
                    provincelist.AddRange(materialOrderList.Select(s => s.ProvinceName).Distinct().ToList());

                }
                if (provincelist.Any())
                {
                    StringBuilder json = new StringBuilder();
                    bool isEmpty = false;
                    provincelist.Distinct().ToList().ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            json.Append("{\"ProvinceName\":\"" + s + "\"},");
                        }
                        else
                            isEmpty = true;
                    });
                    if (isEmpty)
                    {
                        json.Append("{\"ProvinceName\":\"空\"},");
                    }
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                else
                    return "";
            }
            else
                return "";
        }

        string GetCity()
        {
            if (!string.IsNullOrWhiteSpace(subjectId) && !string.IsNullOrWhiteSpace(province))
            {
                List<int> sidList = StringHelper.ToIntList(subjectId, ',');
                List<string> regionList = StringHelper.ToStringList(region, ',');
                List<string> provinceList = StringHelper.ToStringList(province, ',');
                //var list = orderBll.GetList(s => sidList.Contains(s.SubjectId ?? 0) && regionList.Contains(s.Region) && provinceList.Contains(s.Province)).OrderBy(s => s.Region).ThenBy(s => s.Province).ThenBy(s => s.City).ToList();
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                             && (order.IsDelete == null || order.IsDelete == false)
                            select new
                            {
                                order,
                                shop
                            }
                            ).ToList();
                if (provinceList.Any())
                {
                    if (provinceList.Contains("空"))
                    {
                        provinceList.Remove("空");
                        if (provinceList.Any())
                        {
                            list = list.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();
                        }
                        else
                            list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();
                    }
                    else
                        list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }

                List<string> citylist = new List<string>();
                if (list.Any())
                {
                    citylist = list.Select(s => s.order.City).Distinct().ToList();

                }
                var materialOrderList = (from order in CurrentContext.DbContext.OrderMaterial
                                         join shop in CurrentContext.DbContext.Shop
                                         on order.ShopId equals shop.Id
                                         where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                                         select new
                                         {
                                             shop
                                         }
                                  ).ToList();
                if (provinceList.Any())
                {
                    if (provinceList.Contains("空"))
                    {
                        provinceList.Remove("空");
                        if (provinceList.Any())
                        {
                            materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.shop.ProvinceName) || s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                        }
                        else
                            materialOrderList = materialOrderList.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                    }
                    else
                        materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }

                if (materialOrderList.Any())
                {
                    materialOrderList = materialOrderList.Distinct().ToList();
                    materialOrderList.ForEach(s =>
                    {
                        if (!citylist.Contains(s.shop.CityName))
                        {
                            citylist.Add(s.shop.CityName);
                        }
                    });
                }

                if (citylist.Any())
                {
                    StringBuilder json = new StringBuilder();
                    bool isEmpty = false;
                    citylist.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            json.Append("{\"CityName\":\"" + s + "\"},");
                        }
                        else
                            isEmpty = true;
                    });
                    if (isEmpty)
                        json.Append("{\"CityName\":\"空\"},");
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                else
                    return "";
            }
            else
                return "";
        }

        string GetCustomerService()
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<int> sidList = StringHelper.ToIntList(subjectId, ',');

                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join user in CurrentContext.DbContext.UserInfo
                            on shop.CSUserId equals user.UserId into userTemp
                            from customerService in userTemp.DefaultIfEmpty()
                            where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                             && (order.IsDelete == null || order.IsDelete == false)
                            select new
                            {
                                order,
                                shop,
                                CSName = customerService.RealName
                            }
                              ).ToList();


                var materialOrderList = (from order in CurrentContext.DbContext.OrderMaterial
                                         join shop in CurrentContext.DbContext.Shop
                                         on order.ShopId equals shop.Id
                                         join user in CurrentContext.DbContext.UserInfo
                                         on shop.CSUserId equals user.UserId into userTemp
                                         from customerService in userTemp.DefaultIfEmpty()
                                         where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                                         select new
                                         {
                                             shop,
                                             CSName = customerService.RealName
                                         }
                                  ).ToList();

                List<string> regionList = new List<string>();
                if (string.IsNullOrWhiteSpace(region))
                {
                    //regionList = new BasePage().GetResponsibleRegion;
                    //if (regionList.Any())
                    //{
                    //    StringHelper.ToUpperOrLowerList(ref regionList, LowerUpperEnum.ToLower);
                    //}
                }
                else
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }

                if (regionList.Any())
                {
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();
                            materialOrderList = materialOrderList.Where(s => regionList0.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                        }
                        else
                        {
                            list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                            materialOrderList = materialOrderList.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                        }
                    }
                    else
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                        materialOrderList = materialOrderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    }

                    //list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    //materialOrderList = materialOrderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    List<string> provinceList = StringHelper.ToStringList(province, ',');
                    if (provinceList.Any())
                    {
                        if (provinceList.Contains("空"))
                        {
                            provinceList.Remove("空");
                            if (provinceList.Any())
                            {
                                list = list.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();
                                materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.shop.ProvinceName) || s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                            }
                            else
                            {
                                list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();
                                materialOrderList = materialOrderList.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                            }
                        }
                        else
                        {
                            list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();
                            materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                        }

                        //list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                        //materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    List<string> cityList = StringHelper.ToStringList(city, ',');
                    if (cityList.Any())
                    {
                        if (cityList.Contains("空"))
                        {
                            cityList.Remove("空");
                            if (cityList.Any())
                            {
                                list = list.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();
                                materialOrderList = materialOrderList.Where(s => cityList.Contains(s.shop.CityName) || s.shop.CityName == null || s.shop.CityName == "").ToList();
                            }
                            else
                            {
                                list = list.Where(s => s.order.City == null || s.order.City == "").ToList();
                                materialOrderList = materialOrderList.Where(s => s.shop.CityName == null || s.shop.CityName == "").ToList();
                            }
                        }
                        else
                        {
                            list = list.Where(s => cityList.Contains(s.order.City)).ToList();
                            materialOrderList = materialOrderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                        }


                        //list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                        //materialOrderList = materialOrderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    }
                }
                List<int> userIdList = new List<int>();
                StringBuilder json = new StringBuilder();
                bool isEmpty = false;
                if (list.Any())
                {

                    var list1 = list.Select(s => new { s.shop.CSUserId, s.CSName }).Distinct().ToList();

                    list1.ForEach(s =>
                    {
                        if (s.CSUserId != null && s.CSUserId > 0)
                        {
                            if (!userIdList.Contains(s.CSUserId ?? 0))
                            {
                                userIdList.Add(s.CSUserId ?? 0);
                                json.Append("{\"UserId\":\"" + s.CSUserId + "\",\"UserName\":\"" + s.CSName + "\"},");
                            }

                        }
                        else
                            isEmpty = true;
                    });

                }
                if (materialOrderList.Any())
                {
                    var list2 = materialOrderList.Select(s => new { s.shop.CSUserId, s.CSName }).Distinct().ToList();
                    list2.ForEach(s =>
                    {
                        if (s.CSUserId != null && s.CSUserId > 0)
                        {
                            if (!userIdList.Contains(s.CSUserId ?? 0))
                            {
                                userIdList.Add(s.CSUserId ?? 0);
                                json.Append("{\"UserId\":\"" + s.CSUserId + "\",\"UserName\":\"" + s.CSName + "\"},");
                            }

                        }
                        else
                            isEmpty = true;
                    });
                }
                if (isEmpty)
                {
                    json.Append("{\"UserId\":\"0\",\"UserName\":\"无\"}");
                }
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetInstallLevel()
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<int> sidList = StringHelper.ToIntList(subjectId, ',');

                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id

                            where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                             && (order.IsDelete == null || order.IsDelete == false)
                            select new
                            {
                                order,
                                shop

                            }).ToList();


                var materialOrderList = (from order in CurrentContext.DbContext.OrderMaterial
                                         join shop in CurrentContext.DbContext.Shop
                                         on order.ShopId equals shop.Id

                                         where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                                         select

                                             shop


                                  ).ToList();


                List<string> regionList = new List<string>();
                if (string.IsNullOrWhiteSpace(region))
                {
                    //regionList = new BasePage().GetResponsibleRegion;
                    //if (regionList.Any())
                    //{
                    //    StringHelper.ToUpperOrLowerList(ref regionList, LowerUpperEnum.ToLower);
                    //}
                }
                else
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);

                }
                if (regionList.Any())
                {
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();
                            materialOrderList = materialOrderList.Where(s => regionList0.Contains(s.RegionName.ToLower()) || s.RegionName == null || s.RegionName == "").ToList();
                        }
                        else
                        {
                            list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                            materialOrderList = materialOrderList.Where(s => s.RegionName == null || s.RegionName == "").ToList();
                        }
                    }
                    else
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                        materialOrderList = materialOrderList.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                    }
                    //list = list.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                    //materialOrderList = materialOrderList.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    List<string> provinceList = StringHelper.ToStringList(province, ',');
                    if (provinceList.Any())
                    {
                        if (provinceList.Contains("空"))
                        {
                            provinceList.Remove("空");
                            if (provinceList.Any())
                            {
                                list = list.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();
                                materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.ProvinceName) || s.ProvinceName == null || s.ProvinceName == "").ToList();
                            }
                            else
                            {
                                list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();
                                materialOrderList = materialOrderList.Where(s => s.ProvinceName == null || s.ProvinceName == "").ToList();
                            }
                        }
                        else
                        {
                            list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();
                            materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                        }



                        //list = list.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                        //materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    List<string> cityList = StringHelper.ToStringList(city, ',');
                    if (cityList.Any())
                    {
                        if (cityList.Contains("空"))
                        {
                            cityList.Remove("空");
                            if (cityList.Any())
                            {
                                list = list.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();
                                materialOrderList = materialOrderList.Where(s => cityList.Contains(s.CityName) || s.CityName == null || s.CityName == "").ToList();
                            }
                            else
                            {
                                list = list.Where(s => s.order.City == null || s.order.City == "").ToList();
                                materialOrderList = materialOrderList.Where(s => s.CityName == null || s.CityName == "").ToList();
                            }
                        }
                        else
                        {
                            list = list.Where(s => cityList.Contains(s.order.City)).ToList();
                            materialOrderList = materialOrderList.Where(s => cityList.Contains(s.CityName)).ToList();
                        }


                        //list = list.Where(s => cityList.Contains(s.CityName)).ToList();
                        //materialOrderList = materialOrderList.Where(s => cityList.Contains(s.CityName)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(customerService))
                {
                    List<int> customerServiceList = StringHelper.ToIntList(customerService, ',');
                    if (customerServiceList.Any())
                    {
                        if (customerServiceList.Contains(0))
                        {
                            customerServiceList.Remove(0);
                            if (customerServiceList.Any())
                            {
                                list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                                materialOrderList = materialOrderList.Where(s => customerServiceList.Contains(s.CSUserId ?? 0) || (s.CSUserId == null || s.CSUserId == 0)).ToList();
                            }
                            else
                            {
                                list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                                materialOrderList = materialOrderList.Where(s => (s.CSUserId == null || s.CSUserId == 0)).ToList();
                            }
                        }
                        else
                        {
                            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                            materialOrderList = materialOrderList.Where(s => customerServiceList.Contains(s.CSUserId ?? 0)).ToList();
                        }
                    }

                }

                StringBuilder json = new StringBuilder();
                bool isEmpty = false;
                List<string> installList = new List<string>();
                if (list.Any())
                {

                    var list1 = list.Select(s => s.order.IsInstall).Distinct().ToList();
                    list1.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            if (!installList.Contains(s))
                            {
                                json.Append("{\"IsInstall\":\"" + s + "\"},");
                                installList.Add(s);
                            }
                        }
                        else
                            isEmpty = true;
                    });

                }
                if (materialOrderList.Any())
                {
                    var list2 = materialOrderList.Select(s => s.IsInstall).Distinct().ToList();
                    list2.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            if (!installList.Contains(s))
                            {
                                json.Append("{\"IsInstall\":\"" + s + "\"},");
                                installList.Add(s);
                            }
                        }
                        else
                            isEmpty = true;
                    });
                }
                if (isEmpty)
                {
                    json.Append("{\"IsInstall\":\"无\"}");
                }
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetMaterialCategory()
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<int> sidList = StringHelper.ToIntList(subjectId, ',');
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            where selectType == 1 ? sidList.Contains(order.SubjectId ?? 0) : sidList.Contains(order.RegionSupplementId ?? 0)
                             && (order.IsDelete == null || order.IsDelete == false)
                            select new
                            {
                                shop,
                                MaterialName = order.GraphicMaterial != null ? order.GraphicMaterial : "",
                                materialCategory,
                                order
                            }
                              ).ToList();
                list = list.Where(s => (s.order.OrderType == 1 && s.order.GraphicLength != null && s.order.GraphicLength > 0 && s.order.GraphicWidth != null && s.order.GraphicWidth > 0) || (s.order.OrderType > 1)).ToList();
                List<string> regionList = new List<string>();
                if (string.IsNullOrWhiteSpace(region))
                {
                    //regionList = new BasePage().GetResponsibleRegion;
                    //if (regionList.Any())
                    //{
                    //    StringHelper.ToUpperOrLowerList(ref regionList, LowerUpperEnum.ToLower);
                    //}
                }
                else
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                if (regionList.Any())
                {

                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();

                        }
                        else
                        {
                            list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();

                        }
                    }
                    else
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();

                    }
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    List<string> provinceList = StringHelper.ToStringList(province, ',');
                    if (provinceList.Any())
                    {
                        if (provinceList.Contains("空"))
                        {
                            provinceList.Remove("空");
                            if (provinceList.Any())
                            {
                                list = list.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                            }
                            else
                            {
                                list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                            }
                        }
                        else
                        {
                            list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();

                        }
                    }

                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    List<string> cityList = StringHelper.ToStringList(city, ',');
                    if (cityList.Any())
                    {
                        if (cityList.Contains("空"))
                        {
                            cityList.Remove("空");
                            if (cityList.Any())
                            {
                                list = list.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                            }
                            else
                            {
                                list = list.Where(s => s.order.City == null || s.order.City == "").ToList();

                            }
                        }
                        else
                        {
                            list = list.Where(s => cityList.Contains(s.order.City)).ToList();

                        }
                    }
                    //list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(customerService))
                {
                    List<int> customerServiceList = StringHelper.ToIntList(customerService, ',');
                    if (customerServiceList.Any())
                    {
                        if (customerServiceList.Contains(0))
                        {
                            customerServiceList.Remove(0);
                            if (customerServiceList.Any())
                            {
                                list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                            }
                            else
                                list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                        }
                        else
                        {
                            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                        }
                    }

                }
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    List<string> installList = StringHelper.ToStringList(isInstall, ',');
                    if (installList.Any())
                    {
                        if (installList.Contains("无"))
                        {
                            installList.Remove("无");
                            if (installList.Any())
                            {
                                list = list.Where(s => installList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                            }
                            else
                                list = list.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                        }
                        else
                            list = list.Where(s => installList.Contains(s.order.IsInstall)).ToList();
                    }
                }

                if (list.Any())
                {
                    int orderCount = list.Count;
                    var shopCount = list.Select(s => s.shop.Id).Distinct().Count();
                    StringBuilder json = new StringBuilder();
                    List<string> materialList = list.Select(s => s.MaterialName).Distinct().ToList();
                    bool isEmpty = false;
                    if (materialList.Contains("") || materialList.Contains(" "))
                    {
                        isEmpty = true;
                        materialList.Remove("");
                        materialList.Remove(" ");
                    }
                    var list1 = (from material in materialList
                                 join orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                                 on material.ToLower() equals orderMaterial.OrderMaterialName.ToLower()
                                 join mcategory in CurrentContext.DbContext.MaterialCategory
                                 on orderMaterial.BasicCategoryId equals mcategory.Id into categoryTemp
                                 from materialCategory in categoryTemp.DefaultIfEmpty()
                                 select new
                                 {
                                     materialCategory
                                 }).ToList();
                    List<int> idList = new List<int>();

                    list1.ForEach(s =>
                    {
                        if (s.materialCategory != null)
                        {
                            if (!idList.Contains(s.materialCategory.Id))
                            {
                                idList.Add(s.materialCategory.Id);
                                json.Append("{\"CategoryId\":\"" + s.materialCategory.Id + "\",\"CategoryName\":\"" + s.materialCategory.CategoryName + "\"},");
                            }
                        }
                        else
                            isEmpty = true;
                    });
                    if (isEmpty)
                    {
                        json.Append("{\"CategoryId\":\"0\",\"CategoryName\":\"无\"},");
                    }
                    result = "[{\"OrderCount\":\"" + orderCount + "\",\"ShopCount\":\"" + shopCount + "\",\"rows\":[" + json.ToString().TrimEnd(',') + "]}]";
                }
            }
            return result;
        }

        string GetOrderDetailList(int currPage, int pageSize)
        {
            string result = string.Empty;
            int shopCount = 0;
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                try
                {
                    List<int> subjectList = StringHelper.ToIntList(subjectId, ',');
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     join pop1 in CurrentContext.DbContext.POP
                                     on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                                     from pop in popTemp.DefaultIfEmpty()
                                     join company1 in CurrentContext.DbContext.Company
                                     on order.OutsourceId equals company1.Id into companyTemp
                                     from company in companyTemp.DefaultIfEmpty()
                                     //where subjectList.Contains(order.SubjectId ?? 0)
                                     where selectType == 1 ? subjectList.Contains(order.SubjectId ?? 0) : subjectList.Contains(order.RegionSupplementId ?? 0)
                                     && ((order.OrderType != null && order.OrderType == 1 && order.GraphicWidth != null && order.GraphicWidth != 0 && order.GraphicLength != null && order.GraphicLength != 0) || (order.OrderType != null && order.OrderType == 2))
                                     && (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
                                     && (order.IsDelete == null || order.IsDelete == false)
                                     && (shop.Status == null || shop.Status == "" || shop.Status == "正常")
                                     select new
                                     {
                                         order,
                                         pop,
                                         shop,
                                         CompanyName = company.CompanyName
                                     }
                                     ).OrderBy(s => s.shop.Id).ToList();
                    if (orderList.Any())
                    {
                        //if (!string.IsNullOrWhiteSpace(region))
                        //{
                        //    List<string> regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                        //    orderList = orderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                        //}
                        List<string> regionList = new List<string>();
                        if (string.IsNullOrWhiteSpace(region))
                        {
                            //regionList = new BasePage().GetResponsibleRegion;
                            //if (regionList.Any())
                            //{
                            //    StringHelper.ToUpperOrLowerList(ref regionList, LowerUpperEnum.ToLower);
                            //}
                        }
                        else
                        {
                            regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                        }
                        if (regionList.Any())
                        {
                            orderList = orderList.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                        }
                        if (!string.IsNullOrWhiteSpace(province))
                        {
                            List<string> provinceList = StringHelper.ToStringList(province, ',');
                            orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                        }
                        if (!string.IsNullOrWhiteSpace(city))
                        {
                            List<string> cityList = StringHelper.ToStringList(city, ',');
                            orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
                        }

                        if (!string.IsNullOrWhiteSpace(customerService))
                        {
                            List<int> customerServiceList = StringHelper.ToIntList(customerService, ',');
                            if (customerServiceList.Contains(0))
                            {
                                customerServiceList.Remove(0);
                                if (customerServiceList.Any())
                                {
                                    orderList = orderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                                }
                                else
                                    orderList = orderList.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                            }
                            else
                            {
                                orderList = orderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(isInstall))
                        {
                            List<string> installList = StringHelper.ToStringList(isInstall, ',');
                            if (installList.Contains("无"))
                            {
                                installList.Remove("无");
                                if (installList.Any())
                                {
                                    orderList = orderList.Where(s => installList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                                }
                                else
                                    orderList = orderList.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                            }
                            else
                                orderList = orderList.Where(s => installList.Contains(s.order.IsInstall)).ToList();
                        }
                        if (!string.IsNullOrWhiteSpace(materialCategory))
                        {
                            bool hasEmpty = false;
                            List<string> materialList = new List<string>();
                            List<int> categoryList = StringHelper.ToIntList(materialCategory, ',');
                            if (categoryList.Contains(0))
                            {
                                hasEmpty = true;
                                categoryList.Remove(0);
                            }
                            if (categoryList.Any())
                            {
                                materialList = new OrderMaterialMppingBLL().GetList(s => categoryList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                            }
                            if (hasEmpty)
                            {
                                if (materialList.Any())
                                {
                                    orderList = orderList.Where(s => materialList.Contains(s.order.GraphicMaterial.ToLower()) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                                }
                                else
                                    orderList = orderList.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                            }
                            else
                                orderList = orderList.Where(s => materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                        }







                        if (searchType == "1")
                        {
                            orderList = orderList.Where(s => (s.shop.CSUserId == userId)).ToList();
                        }
                        if (orderList.Any())
                        {
                            shopCount = orderList.Select(s => s.shop).Distinct().Count();
                            StringBuilder json = new StringBuilder();
                            int total = orderList.Count;
                            orderList = orderList.OrderBy(s => s.order.ShopId).Skip(currPage * pageSize).Take(pageSize).ToList();

                            orderList.ForEach(s =>
                            {
                                //string status = string.Empty;
                                //if (s.shop.Status != null)
                                //    status = s.shop.Status;
                                //bool isClose = false;
                                //if (!string.IsNullOrWhiteSpace(status) && (status.Contains("关") || status.Contains("闭")))
                                //{
                                //    isClose = true;
                                //}
                                //if (!isClose)
                                //{
                                string orderType = s.order.OrderType != null && s.order.OrderType == 1 ? "POP" : "道具";
                                string levelNum = string.Empty;
                                if (s.order.LevelNum != null && s.order.LevelNum > 0)
                                {

                                    levelNum = CommonMethod.GeEnumName<LevelNumEnum>(s.order.LevelNum.ToString());
                                }
                                decimal area = 0;
                                if (s.order.GraphicWidth != null && s.order.GraphicLength != null)
                                {
                                    area = ((s.order.GraphicLength ?? 0) * (s.order.GraphicWidth ?? 0)) / 1000000;
                                }
                                json.Append("{\"total\":\"" + total + "\",\"Id\":\"" + s.order.Id + "\",\"OrderType\":\"" + orderType + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",");
                                json.Append("\"Region\":\"" + s.shop.RegionName + "\",\"Province\":\"" + s.shop.ProvinceName + "\",\"City\":\"" + s.shop.CityName + "\",");
                                json.Append("\"CityTier\":\"" + s.shop.CityTier + "\",\"Format\":\"" + s.shop.Format + "\",\"MaterialSupport\":\"" + s.order.MaterialSupport + "\",");
                                json.Append("\"POSScale\":\"" + s.order.POSScale + "\",\"GraphicNo\":\"" + s.order.GraphicNo + "\",\"POPName\":\"" + s.order.POPName + "\",");
                                json.Append("\"POPType\":\"" + s.order.POPType + "\",\"Sheet\":\"" + s.order.Sheet + "\",\"Gender\":\"" + s.order.Gender + "\",");
                                json.Append("\"Quantity\":\"" + s.order.Quantity + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",");
                                json.Append("\"MachineFrame\":\"" + s.order.MachineFrame + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",");
                                json.Append("\"Area\":\"" + area + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"WindowWide\":\"" + (s.order.WindowWide != null ? s.order.WindowWide.ToString() : "") + "\",");
                                json.Append("\"WindowHigh\":\"" + (s.order.WindowHigh != null ? s.order.WindowHigh.ToString() : "") + "\",\"WindowDeep\":\"" + (s.order.WindowDeep != null ? s.order.WindowDeep.ToString() : "") + "\",\"WindowSize\":\"" + s.order.WindowSize + "\",");
                                json.Append("\"Style\":\"" + (s.pop != null ? s.pop.Style : "") + "\",\"CornerType\":\"" + (s.pop != null ? s.pop.CornerType : "") + "\",\"Category\":\"" + (s.pop != null ? s.pop.Category : "") + "\",");
                                json.Append("\"StandardDimension\":\"" + (s.pop != null ? s.pop.StandardDimension : "") + "\",\"Modula\":\"" + (s.pop != null ? s.pop.Modula : "") + "\",\"Frame\":\"" + (s.pop != null ? s.pop.Frame : "") + "\",");
                                json.Append("\"DoubleFace\":\"" + (s.pop != null ? s.pop.DoubleFace : "") + "\",\"Glass\":\"" + (s.pop != null ? s.pop.Glass : "") + "\",\"Backdrop\":\"" + (s.pop != null ? s.pop.Backdrop : "") + "\",");
                                json.Append("\"ModulaQuantityWidth\":\"" + (s.pop != null && s.pop.ModulaQuantityWidth != null ? s.pop.ModulaQuantityWidth.ToString() : "") + "\",\"ModulaQuantityHeight\":\"" + (s.pop != null && s.pop.ModulaQuantityHeight != null ? s.pop.ModulaQuantityHeight.ToString() : "") + "\",\"PlatformLength\":\"" + (s.pop != null && s.pop.PlatformLength != null ? s.pop.PlatformLength.ToString() : "") + "\",");
                                json.Append("\"PlatformWidth\":\"" + (s.pop != null && s.pop.PlatformWidth != null ? s.pop.PlatformWidth.ToString() : "") + "\",\"PlatformHeight\":\"" + (s.pop != null && s.pop.PlatformHeight != null ? s.pop.PlatformHeight.ToString() : "") + "\",\"FixtureType\":\"" + (s.pop != null ? s.pop.FixtureType : "") + "\",");
                                json.Append("\"ChooseImg\":\"" + (s.order.ChooseImg) + "\",\"Remark\":\"" + (s.order.Remark) + "\",\"LevelNum\":\"" + levelNum + "\",\"CompanyName\":\"" + s.CompanyName + "\"},");
                                // }
                            });
                            result = "[" + json.ToString().TrimEnd(',') + "]$" + shopCount;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = "";
                }
            }
            return result;
        }

        string ExportOrder()
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                StringBuilder whereSql = new StringBuilder();

                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                    StringBuilder regions = new StringBuilder();
                    regionList.ForEach(s =>
                    {
                        regions.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and Region in({0})", regions.ToString().TrimEnd(','));
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    StringBuilder provinces = new StringBuilder();
                    provinceList.ForEach(s =>
                    {
                        provinces.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and Province in({0})", provinces.ToString().TrimEnd(','));
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    StringBuilder citys = new StringBuilder();
                    cityList.ForEach(s =>
                    {
                        citys.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and City in({0})", citys.ToString().TrimEnd(','));
                }

                if (searchType == "1")
                    whereSql.AppendFormat(" and [客服]={0}", userId);
                System.Data.DataSet ds = orderBll.GetOrderList(subjectId, whereSql.ToString(), "true");
                int colNum = ds.Tables[0].Columns.Count;
                ds.Tables[0].Columns.RemoveAt(colNum - 1);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {

                    #region 物料导出
                    //if (!string.IsNullOrWhiteSpace(subjectId))
                    //{
                    //    List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                    //    var orderMaterialList = (from material in CurrentContext.DbContext.OrderMaterial
                    //                             join subject in CurrentContext.DbContext.Subject
                    //                             on material.SubjectId equals subject.Id
                    //                             join shop in CurrentContext.DbContext.Shop
                    //                             on material.ShopId equals shop.Id
                    //                             where subjectIdList.Contains(material.SubjectId ?? 0)
                    //                             select new
                    //                             {
                    //                                 material,
                    //                                 subject,
                    //                                 shop
                    //                             }).ToList();
                    //    if (roleId == 5)
                    //    {
                    //        orderMaterialList = orderMaterialList.Where(s => s.shop.CSUserId == userId).ToList();
                    //    }
                    //    if (!string.IsNullOrWhiteSpace(region))
                    //    {
                    //        regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                    //        orderMaterialList = orderMaterialList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    //    }
                    //    if (!string.IsNullOrWhiteSpace(province))
                    //    {
                    //        provinceList = StringHelper.ToStringList(province, ',');
                    //        orderMaterialList = orderMaterialList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    //    }
                    //    if (!string.IsNullOrWhiteSpace(city))
                    //    {
                    //        cityList = StringHelper.ToStringList(city, ',');
                    //        orderMaterialList = orderMaterialList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    //    }
                    //    orderMaterialList.ForEach(s =>
                    //    {
                    //        DataRow dr = ds.Tables[0].NewRow();
                    //        dr["订单类型"] = "物料";
                    //        dr["POSCode"] = s.shop.ShopNo;
                    //        dr["POS Name"] = s.shop.ShopName;
                    //        dr["Region"] = s.shop.RegionName;
                    //        dr["Province"] = s.shop.ProvinceName;
                    //        dr["City"] = s.shop.CityName;
                    //        dr["City Tier"] = s.shop.CityTier;
                    //        dr["Customer Code"] = s.shop.AgentCode;
                    //        dr["Customer Name"] = s.shop.AgentName;
                    //        dr["Channel"] = s.shop.Channel;
                    //        dr["Format"] = s.shop.Format;
                    //        dr["Location Type"] = s.shop.LocationType;
                    //        dr["Business Model"] = s.shop.BusinessModel;
                    //        dr["POPAddress"] = s.shop.POPAddress;
                    //        dr["Contact1"] = s.shop.Contact1;
                    //        dr["Tel1"] = s.shop.Tel1;
                    //        dr["Contact2"] = s.shop.Contact2;
                    //        dr["Tel2"] = s.shop.Tel2;
                    //        dr["Opening Date"] = s.shop.OpeningDate;
                    //        dr["Status"] = s.shop.Status;
                    //        dr["Quantity"] = s.material.MaterialCount;
                    //        StringBuilder size = new StringBuilder();
                    //        if (s.material.MaterialLength != null && s.material.MaterialLength > 0 && s.material.MaterialWidth != null && s.material.MaterialWidth > 0)
                    //        {
                    //            size.AppendFormat("({0}*{1}", s.material.MaterialLength, s.material.MaterialWidth);
                    //            if (s.material.MaterialHigh != null && s.material.MaterialHigh > 0)
                    //                size.AppendFormat("*{0}", s.material.MaterialHigh);
                    //            size.Append(")");
                    //        }
                    //        dr["Sheet"] = s.material.MaterialName + size;
                    //        dr["Position Description"] = "";
                    //        dr["活动名称"] = s.subject.SubjectName;
                    //        if (s.material.Price != null && s.material.Price>0)
                    //            dr["UnitPrice"] = decimal.Parse(s.material.Price.ToString());

                    //        ds.Tables[0].Rows.Add(dr);
                    //    });
                    //}

                    #endregion


                    #region 没用的
                    //if (materialItemIds != null)
                    //{
                    //    List<int> ids = StringHelper.ToIntList(materialItemIds, ',');

                    //    ids.ForEach(id =>
                    //    {
                    //        var list = (from material in CurrentContext.DbContext.OrderMaterial
                    //                    join shop in CurrentContext.DbContext.Shop
                    //                    on material.ShopId equals shop.Id
                    //                    join mitem in CurrentContext.DbContext.OrderMaterialItem
                    //                    on id equals mitem.ItemId
                    //                    join subject in CurrentContext.DbContext.Subject
                    //                    on mitem.SubjectId equals subject.Id
                    //                    where material.ItemId == id && (regionList.Any() ? regionList.Contains(shop.RegionName) : 1 == 1)
                    //                    && (provinceList.Any() ? provinceList.Contains(shop.ProvinceName) : 1 == 1)
                    //                    && (cityList.Any() ? cityList.Contains(shop.CityName) : 1 == 1)
                    //                    select new
                    //                    {
                    //                        shop,
                    //                        material,
                    //                        subject
                    //                    }).ToList();

                    //        if (roleId == 5)
                    //        {
                    //            list = list.Where(s=>s.shop.CSUserId==userId).ToList();
                    //        }
                    //        list.ForEach(s =>
                    //        {
                    //            DataRow dr = ds.Tables[0].NewRow();
                    //            dr["订单类型"] = "物料";
                    //            dr["POS Code"] = s.shop.ShopNo;
                    //            dr["POS Name"] = s.shop.ShopName;
                    //            dr["Region"] = s.shop.RegionName;
                    //            dr["Province"] = s.shop.ProvinceName;
                    //            dr["City"] = s.shop.CityName;
                    //            dr["City Tier"] = s.shop.CityTier;
                    //            dr["Customer Code"] = s.shop.AgentCode;
                    //            dr["Customer Name"] = s.shop.AgentName;
                    //            dr["Channel"] = s.shop.Channel;
                    //            dr["Format"] = s.shop.Format;
                    //            dr["Location Type"] = s.shop.LocationType;
                    //            dr["Business Model"] = s.shop.BusinessModel;
                    //            dr["POPAddress"] = s.shop.POPAddress;
                    //            dr["Contact1"] = s.shop.Contact1;
                    //            dr["Tel1"] = s.shop.Tel1;
                    //            dr["Contact2"] = s.shop.Contact2;
                    //            dr["Tel2"] = s.shop.Tel2;
                    //            dr["Opening Date"] = s.shop.OpeningDate;
                    //            dr["Status"] = s.shop.Status;
                    //            dr["Quantity"] = s.material.MaterialCount;
                    //            dr["Position Description"] = s.material.MaterialName;
                    //            dr["活动名称"] = s.subject.SubjectName;
                    //            ds.Tables[0].Rows.Add(dr);
                    //        });
                    //    });
                    //}
                    #endregion
                    string fileName = "POP订单数据";
                    //DataView dv = ds.Tables[0].DefaultView;
                    //dv.Sort = " [POSCode] ";
                    OperateFile.ExportExcel(ds.Tables[0], fileName);

                }
                else
                    result = "没有数据可以导出";
            }
            else
                result = "没有数据可以导出";
            return result;
        }

        void Export350()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join pop1 in CurrentContext.DbContext.POP
                            on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                            from pop in popTemp.DefaultIfEmpty()
                            //pop订单中，pop尺寸为空的不导出
                            where subjectIdList.Contains(order.SubjectId ?? 0)
                            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                                //&& !shop.Format.Contains("Homecourt")
                            && (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
                            select new
                            {
                                subject,
                                shop,
                                order,
                                pop
                            }).ToList();

                if (searchType == "1")
                {
                    list = list.Where(s => s.shop.CSUserId == userId).ToList();
                }
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }
                List<Order350Model> orderList = new List<Order350Model>();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        int level = item.order.LevelNum ?? 0;
                        string levelName = string.Empty;
                        //if(item.order.Sheet.Contains("桌"))
                        //    levelName = CommonMethod.GeEnumName<TableLevelEnum>(level.ToString());
                        if (level > 0)
                            levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());
                        Order350Model model = new Order350Model();
                        model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                        model.Category = item.pop != null ? item.pop.Category : "";
                        model.ChooseImg = item.order.ChooseImg;
                        model.City = item.shop.CityName;
                        model.CityTier = item.shop.CityTier;
                        model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                        model.Format = item.shop.Format;
                        model.Gender = item.order.Gender;
                        model.GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                        string smallMaterial = GetSmallMaterial(item.order.SmallMaterialId ?? 0);
                        string material = item.order.GraphicMaterial;
                        if (!string.IsNullOrWhiteSpace(smallMaterial))
                            material += ("+" + smallMaterial);
                        model.GraphicMaterial = material;
                        model.GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0; ;
                        model.POPAddress = item.shop.POPAddress;
                        model.PositionDescription = item.order.PositionDescription;
                        model.Province = item.shop.ProvinceName;
                        model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                        model.Sheet = item.order.Sheet;
                        model.ShopName = item.order.ShopName;
                        model.ShopNo = item.order.ShopNo;
                        model.SubjectName = item.subject.SubjectName;
                        model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                        model.OtherRemark = levelName;
                        model.ShopLevel = item.shop.ShopLevel;
                        orderList.Add(model);
                    }
                }
                #region 物料导出
                var orderMaterialList = (from material in CurrentContext.DbContext.OrderMaterial
                                         join subject in CurrentContext.DbContext.Subject
                                         on material.SubjectId equals subject.Id
                                         join shop in CurrentContext.DbContext.Shop
                                         on material.ShopId equals shop.Id
                                         where subjectIdList.Contains(material.SubjectId ?? 0)
                                         select new
                                         {
                                             material,
                                             subject,
                                             shop
                                         }).ToList();
                if (searchType == "1")
                {
                    orderMaterialList = orderMaterialList.Where(s => s.shop.CSUserId == userId).ToList();
                }
                if (regionList.Any())
                {
                    orderMaterialList = orderMaterialList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }

                if (provinceList.Any())
                {
                    orderMaterialList = orderMaterialList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }

                if (cityList.Any())
                {
                    orderMaterialList = orderMaterialList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }
                if (orderMaterialList.Any())
                {
                    orderMaterialList.ForEach(s =>
                    {
                        Order350Model model = new Order350Model();
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = s.shop.CityName;
                        model.CityTier = s.shop.CityTier;
                        model.Contacts = s.shop.Contact1 + "/" + s.shop.Contact2;
                        model.Format = s.shop.Format;
                        model.Gender = "";
                        model.GraphicLength = 0;
                        model.GraphicMaterial = "";
                        model.GraphicWidth = 0;
                        model.POPAddress = s.shop.POPAddress;
                        StringBuilder size = new StringBuilder();
                        if (s.material.MaterialLength != null && s.material.MaterialLength > 0 && s.material.MaterialWidth != null && s.material.MaterialWidth > 0)
                        {
                            size.AppendFormat("({0}*{1}", s.material.MaterialLength, s.material.MaterialWidth);
                            if (s.material.MaterialHigh != null && s.material.MaterialHigh > 0)
                                size.AppendFormat("*{0}", s.material.MaterialHigh);
                            size.Append(")");
                        }
                        model.PositionDescription = s.material.MaterialName + size.ToString();
                        model.Province = s.shop.ProvinceName;
                        model.Quantity = s.material.MaterialCount != null ? double.Parse(s.material.MaterialCount.ToString()) : 0;
                        model.Sheet = s.material.Sheet;
                        model.ShopName = s.shop.ShopName;
                        model.ShopNo = s.shop.ShopNo;
                        model.SubjectName = s.subject.SubjectName;
                        model.Tels = s.shop.Tel1 + "/" + s.shop.Tel2;
                        model.OtherRemark = s.material.Remark;
                        model.ShopLevel = s.shop.ShopLevel;
                        orderList.Add(model);
                    });
                }

                #region 没用
                //if (!string.IsNullOrWhiteSpace(materialItemIds))
                //{
                //    List<int> ids = StringHelper.ToIntList(materialItemIds, ',');

                //    ids.ForEach(id =>
                //    {
                //        var materialList = (from material in CurrentContext.DbContext.OrderMaterial
                //                            join shop in CurrentContext.DbContext.Shop
                //                            on material.ShopId equals shop.Id
                //                            join mitem in CurrentContext.DbContext.OrderMaterialItem
                //                            on id equals mitem.ItemId
                //                            join subject in CurrentContext.DbContext.Subject
                //                            on mitem.SubjectId equals subject.Id
                //                            where material.ItemId == id && (regionList.Any() ? regionList.Contains(shop.RegionName) : 1 == 1)
                //                            && (provinceList.Any() ? provinceList.Contains(shop.ProvinceName) : 1 == 1)
                //                            && (cityList.Any() ? cityList.Contains(shop.CityName) : 1 == 1)
                //                            select new
                //                            {
                //                                shop,
                //                                material,
                //                                subject
                //                            }).ToList();

                //        if (roleId == 5)
                //        {
                //            materialList = materialList.Where(s => s.shop.CSUserId == userId).ToList();
                //        }
                //        materialList.ForEach(s =>
                //        {
                //            Order350Model model = new Order350Model();
                //            model.Area = 0;
                //            model.Category = "";
                //            model.ChooseImg = "";
                //            model.City = s.shop.CityName;
                //            model.CityTier = s.shop.CityTier;
                //            model.Contacts = s.shop.Contact1 + "/" + s.shop.Contact2;
                //            model.Format = s.shop.Format;
                //            model.Gender = "";
                //            model.GraphicLength = 0;
                //            model.GraphicMaterial = "";
                //            model.GraphicWidth = 0;
                //            model.POPAddress = s.shop.POPAddress;
                //            model.PositionDescription = s.material.MaterialName;
                //            model.Province = s.shop.ProvinceName;
                //            model.Quantity = s.material.MaterialCount != null ? double.Parse(s.material.MaterialCount.ToString()) : 0;
                //            model.Sheet = "";
                //            model.ShopName = s.shop.ShopName;
                //            model.ShopNo = s.shop.ShopNo;
                //            model.SubjectName = s.subject.SubjectName;
                //            model.Tels = s.shop.Tel1 + "/" + s.shop.Tel2;
                //            model.OtherRemark = "物料";
                //            orderList.Add(model);
                //        });
                //    });
                //}
                #endregion
                #endregion
                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopNo).ToList();
                    string templateFileName = "350Template";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(context1.Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheet("总表");

                    int startRow = 1;
                    string shopno = string.Empty;
                    foreach (var item in orderList)
                    {


                        if (startRow == 1)
                            shopno = item.ShopNo;
                        else
                        {
                            if (shopno != item.ShopNo)
                            {
                                shopno = item.ShopNo;
                                int row = startRow + 1;
                                while (row.ToString().Substring(row.ToString().Length - 1, 1) != "2")
                                {
                                    startRow++;
                                    row = startRow + 1;
                                }

                            }
                        }

                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 30; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(0).SetCellValue(item.ShopNo);
                        dataRow.GetCell(1).SetCellValue(item.ShopName);
                        dataRow.GetCell(2).SetCellValue(item.Province);
                        dataRow.GetCell(3).SetCellValue(item.City);
                        dataRow.GetCell(4).SetCellValue(item.CityTier);
                        dataRow.GetCell(5).SetCellValue(item.Format);
                        dataRow.GetCell(6).SetCellValue(item.ShopLevel);
                        dataRow.GetCell(7).SetCellValue(item.POPAddress);
                        dataRow.GetCell(8).SetCellValue(item.Contacts);
                        dataRow.GetCell(9).SetCellValue(item.Tels);
                        dataRow.GetCell(10).SetCellValue(item.ChooseImg);
                        dataRow.GetCell(11).SetCellValue(item.SubjectName);
                        dataRow.GetCell(12).SetCellValue(item.Gender);
                        dataRow.GetCell(13).SetCellValue(item.Category);
                        dataRow.GetCell(14).SetCellValue(item.Sheet);
                        dataRow.GetCell(15).SetCellValue(item.Quantity);
                        dataRow.GetCell(16).SetCellValue(item.GraphicMaterial);
                        dataRow.GetCell(17).SetCellValue(item.GraphicWidth);
                        dataRow.GetCell(18).SetCellValue(item.GraphicLength);
                        dataRow.GetCell(19).SetCellValue(item.Area);
                        //其他备注
                        dataRow.GetCell(20).SetCellValue(item.OtherRemark);
                        dataRow.GetCell(21).SetCellValue(item.PositionDescription);
                        startRow++;

                    }

                    HttpCookie cookie = context1.Request.Cookies["export350"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("export350");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    context1.Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();

                        sheet = null;

                        workBook = null;

                        OperateFile.DownLoadFile(ms, "350总表");
                        //OperateFile.DownLoadFile(path);

                    }
                }





            }
        }

        void ExportBJPHW()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join pop1 in CurrentContext.DbContext.POP
                            on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                            from pop in popTemp.DefaultIfEmpty()
                            //pop订单中，pop尺寸为空的不导出
                            //where subjectIdList.Contains(order.SubjectId ?? 0)
                            where selectType == 1 ? subjectIdList.Contains(order.SubjectId ?? 0) : subjectIdList.Contains(order.RegionSupplementId ?? 0)
                            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                                //&& !shop.Format.Contains("Homecourt")
                            && (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
                            && shop.ProvinceName == "北京"
                            select new
                            {
                                subject,
                                shop,
                                order,
                                pop
                            }).ToList();


                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }

                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }

                List<Order350Model> orderList = new List<Order350Model>();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        int level = item.order.LevelNum ?? 0;
                        string levelName = string.Empty;
                        //if(item.order.Sheet.Contains("桌"))
                        //    levelName = CommonMethod.GeEnumName<TableLevelEnum>(level.ToString());
                        if (level > 0)
                            levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());
                        Order350Model model = new Order350Model();
                        model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                        model.Category = item.pop != null ? item.pop.Category : "";
                        model.ChooseImg = item.order.ChooseImg;
                        model.City = item.shop.CityName;
                        model.CityTier = item.shop.CityTier;
                        model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                        model.Format = item.shop.Format;
                        model.Gender = item.order.Gender;
                        model.GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                        model.GraphicMaterial = item.order.GraphicMaterial;
                        model.GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0;
                        model.POPAddress = item.shop.POPAddress;
                        model.PositionDescription = item.order.PositionDescription;
                        model.Province = item.shop.ProvinceName;
                        model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                        model.Sheet = item.order.Sheet;
                        model.ShopName = item.order.ShopName;
                        model.ShopNo = item.order.ShopNo;
                        model.SubjectName = item.subject.SubjectName;
                        model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                        model.OtherRemark = levelName;
                        model.IsPOPMaterial = item.order.IsPOPMaterial;
                        model.CornerType = item.order.CornerType;
                        orderList.Add(model);
                    }
                }


                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopNo).ToList();
                    string templateFileName = "phwTemplate";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(context1.Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheet("Sheet1");

                    int startRow = 1;
                    string shopno = string.Empty;
                    foreach (var item in orderList)
                    {


                        if (startRow == 1)
                            shopno = item.ShopNo;
                        else
                        {
                            if (shopno != item.ShopNo)
                            {
                                shopno = item.ShopNo;
                                startRow++;


                            }
                        }

                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 14; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        //基础材质（喷绘王材质）
                        string material = item.GraphicMaterial;
                        if (item.IsPOPMaterial != null && item.IsPOPMaterial == 1)
                            material = GetBasicMaterial(material);
                        //if (item.PositionDescription.Contains("台卡") && item.CornerType != "三叶草")
                        //    material += "蝴蝶支架";
                        dataRow.GetCell(0).SetCellValue(material);
                        dataRow.GetCell(1).SetCellValue(item.PositionDescription);
                        dataRow.GetCell(2).SetCellValue(Math.Round(item.GraphicWidth / 1000, 2));
                        dataRow.GetCell(3).SetCellValue(Math.Round(item.GraphicLength / 1000, 2));
                        dataRow.GetCell(4).SetCellValue(item.Quantity);
                        dataRow.GetCell(5).SetCellValue("");
                        dataRow.GetCell(6).SetCellValue("");
                        dataRow.GetCell(7).SetCellValue("");
                        dataRow.GetCell(8).SetCellValue("");
                        dataRow.GetCell(9).SetCellValue("");
                        dataRow.GetCell(10).SetCellValue(item.SubjectName);
                        dataRow.GetCell(11).SetCellValue("");
                        dataRow.GetCell(12).SetCellValue(item.ShopName);
                        dataRow.GetCell(13).SetCellValue(item.GraphicMaterial);

                        startRow++;

                    }

                    HttpCookie cookie = context1.Request.Cookies["exportPHWBJ"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportPHWBJ");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    context1.Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();

                        sheet = null;

                        workBook = null;

                        OperateFile.DownLoadFile(ms, "喷绘王模板(北京)");
                        //OperateFile.DownLoadFile(path);

                    }
                }





            }
        }

        void ExportOtherPHW()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join pop1 in CurrentContext.DbContext.POP
                            on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                            from pop in popTemp.DefaultIfEmpty()
                            //pop订单中，pop尺寸为空的不导出
                            //where subjectIdList.Contains(order.SubjectId ?? 0)
                            where selectType == 1 ? subjectIdList.Contains(order.SubjectId ?? 0) : subjectIdList.Contains(order.RegionSupplementId ?? 0)
                            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                                //&& !shop.Format.Contains("Homecourt")
                            && (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
                            && shop.ProvinceName != "北京"
                            select new
                            {
                                subject,
                                shop,
                                order,
                                pop
                            }).ToList();

                //if (roleId == 5)
                //{
                //    list = list.Where(s => s.shop.CSUserId == userId).ToList();
                //}
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }

                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }

                List<Order350Model> orderList = new List<Order350Model>();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        int level = item.order.LevelNum ?? 0;
                        string levelName = string.Empty;
                        //if(item.order.Sheet.Contains("桌"))
                        //    levelName = CommonMethod.GeEnumName<TableLevelEnum>(level.ToString());
                        if (level > 0)
                            levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());
                        Order350Model model = new Order350Model();
                        model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                        model.Category = item.pop != null ? item.pop.Category : "";
                        model.ChooseImg = item.order.ChooseImg;
                        model.City = item.shop.CityName;
                        model.CityTier = item.shop.CityTier;
                        model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                        model.Format = item.shop.Format;
                        model.Gender = item.order.Gender;
                        model.GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                        model.GraphicMaterial = item.order.GraphicMaterial;
                        model.GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0;
                        model.POPAddress = item.shop.POPAddress;
                        model.PositionDescription = item.order.PositionDescription;
                        model.Province = item.shop.ProvinceName;
                        model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                        model.Sheet = item.order.Sheet;
                        model.ShopName = item.order.ShopName;
                        model.ShopNo = item.order.ShopNo;
                        model.SubjectName = item.subject.SubjectName;
                        model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                        model.OtherRemark = levelName;
                        model.IsPOPMaterial = item.order.IsPOPMaterial;
                        orderList.Add(model);
                    }
                }
                //if (!string.IsNullOrWhiteSpace(materialItemIds))
                //{
                //    List<int> ids = StringHelper.ToIntList(materialItemIds, ',');

                //    ids.ForEach(id =>
                //    {
                //        var materialList = (from material in CurrentContext.DbContext.OrderMaterial
                //                            join shop in CurrentContext.DbContext.Shop
                //                            on material.ShopId equals shop.Id
                //                            join mitem in CurrentContext.DbContext.OrderMaterialItem
                //                            on id equals mitem.ItemId
                //                            join subject in CurrentContext.DbContext.Subject
                //                            on mitem.SubjectId equals subject.Id
                //                            where material.ItemId == id && (regionList.Any() ? regionList.Contains(shop.RegionName) : 1 == 1)
                //                            && (provinceList.Any() ? provinceList.Contains(shop.ProvinceName) : 1 == 1)
                //                            && (cityList.Any() ? cityList.Contains(shop.CityName) : 1 == 1)
                //                            select new
                //                            {
                //                                shop,
                //                                material,
                //                                subject
                //                            }).ToList();

                //        if (roleId == 5)
                //        {
                //            materialList = materialList.Where(s => s.shop.CSUserId == userId).ToList();
                //        }
                //        materialList.ForEach(s =>
                //        {
                //            Order350Model model = new Order350Model();
                //            model.Area = 0;
                //            model.Category = "";
                //            model.ChooseImg = "";
                //            model.City = s.shop.CityName;
                //            model.CityTier = s.shop.CityTier;
                //            model.Contacts = s.shop.Contact1 + "/" + s.shop.Contact2;
                //            model.Format = s.shop.Format;
                //            model.Gender = "";
                //            model.GraphicLength = 0;
                //            model.GraphicMaterial = "";
                //            model.GraphicWidth = 0;
                //            model.POPAddress = s.shop.POPAddress;
                //            model.PositionDescription = s.material.MaterialName;
                //            model.Province = s.shop.ProvinceName;
                //            model.Quantity = s.material.MaterialCount != null ? double.Parse(s.material.MaterialCount.ToString()) : 0;
                //            model.Sheet = "";
                //            model.ShopName = s.shop.ShopName;
                //            model.ShopNo = s.shop.ShopNo;
                //            model.SubjectName = s.subject.SubjectName;
                //            model.Tels = s.shop.Tel1 + "/" + s.shop.Tel2;
                //            model.OtherRemark = "物料";
                //            orderList.Add(model);
                //        });
                //    });
                //}

                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopNo).ToList();
                    string templateFileName = "phwTemplate";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(context1.Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheet("Sheet1");

                    int startRow = 1;
                    string shopno = string.Empty;
                    foreach (var item in orderList)
                    {


                        //if (startRow == 1)
                        //    shopno = item.ShopNo;
                        //else
                        //{
                        //    if (shopno != item.ShopNo)
                        //    {
                        //        shopno = item.ShopNo;
                        //        startRow++;
                        //        //int row = startRow + 1;
                        //        //while (row.ToString().Substring(row.ToString().Length - 1, 1) != "2")
                        //        //{
                        //        //    startRow++;
                        //        //    row = startRow + 1;
                        //        //}

                        //    }
                        //}

                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 14; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        //基础材质（喷绘王材质）
                        string material = item.GraphicMaterial;
                        if (item.IsPOPMaterial != null && item.IsPOPMaterial == 1)
                            material = GetBasicMaterial(material);
                        dataRow.GetCell(0).SetCellValue(material);
                        dataRow.GetCell(1).SetCellValue(item.PositionDescription);
                        dataRow.GetCell(2).SetCellValue(Math.Round(item.GraphicWidth / 1000, 2));
                        dataRow.GetCell(3).SetCellValue(Math.Round(item.GraphicLength / 1000, 2));
                        dataRow.GetCell(4).SetCellValue(item.Quantity);
                        dataRow.GetCell(5).SetCellValue("");
                        dataRow.GetCell(6).SetCellValue("");
                        dataRow.GetCell(7).SetCellValue("");
                        dataRow.GetCell(8).SetCellValue("");
                        dataRow.GetCell(9).SetCellValue("");
                        dataRow.GetCell(10).SetCellValue(item.SubjectName);
                        dataRow.GetCell(11).SetCellValue("");
                        dataRow.GetCell(12).SetCellValue(item.ShopName);
                        dataRow.GetCell(13).SetCellValue(item.GraphicMaterial);

                        startRow++;

                    }

                    HttpCookie cookie = context1.Request.Cookies["exportPHWOther"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportPHWOther");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    context1.Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();

                        sheet = null;

                        workBook = null;

                        OperateFile.DownLoadFile(ms, "喷绘王模板(外协)");
                        //OperateFile.DownLoadFile(path);

                    }
                }





            }
        }

        /// <summary>
        /// 获取基础材质，从订单材质对照表找，如果是拆单材质就不需要做这一步
        /// </summary>
        Dictionary<string, string> BasicMaterialDic = new Dictionary<string, string>();
        string GetBasicMaterial(string orderMaterial)
        {

            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(orderMaterial))
            {
                orderMaterial = orderMaterial.ToLower();
                if (BasicMaterialDic.Keys.Contains(orderMaterial))
                {
                    result = BasicMaterialDic[orderMaterial];
                }
                else
                {
                    var model = (from orderMM in CurrentContext.DbContext.OrderMaterialMpping
                                 join curstomerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                                 on orderMM.CustomerMaterialId equals curstomerMaterial.Id
                                 join basicMaterial in CurrentContext.DbContext.BasicMaterial
                                 on curstomerMaterial.BasicMaterialId equals basicMaterial.Id
                                 where orderMM.OrderMaterialName.ToLower() == orderMaterial
                                 select new
                                 {
                                     basicMaterial.MaterialName
                                 }).FirstOrDefault();
                    if (model != null)
                    {
                        result = model.MaterialName;
                        if (!BasicMaterialDic.Keys.Contains(orderMaterial))
                            BasicMaterialDic.Add(orderMaterial, result);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 获取辅料名称
        /// </summary>
        /// <param name="smId"></param>
        /// <returns></returns>
        /// 
        SmallMaterialBLL smmBll = new SmallMaterialBLL();
        Dictionary<int, string> smmDic = new Dictionary<int, string>();
        string GetSmallMaterial(int smId)
        {
            string restule = string.Empty;
            if (smmDic.Keys.Contains(smId))
            {
                restule = smmDic[smId];
            }
            else
            {
                var model = smmBll.GetModel(smId);
                if (model != null)
                {
                    restule = model.SmallMaterialName;
                    if (!smmDic.Keys.Contains(smId))
                    {
                        smmDic.Add(smId, restule);
                    }
                }
            }
            return restule;
        }

        string CheckEmpty(string guidanceId, string region)
        {
            List<int> guidanceIdList = new List<int>();
            List<string> regionList = new List<string>();
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                            &&
                            (
                               shop.RegionName == null || shop.RegionName == "" || shop.ProvinceName == null || shop.ProvinceName == ""
                               || shop.CityName == null || shop.CityName == "" || shop.CityTier == null || shop.CityTier == ""
                               || shop.IsInstall == null || shop.IsInstall == "" || shop.POPAddress == null || shop.POPAddress == ""
                                //|| ((order.POSScale==null || order.POSScale=="")&&(shop.POSScale==null || shop.POSScale==""))
                                //|| ((order.MaterialSupport == null || order.MaterialSupport == "") && (shop.MaterialSupport == null || shop.MaterialSupport == ""))
                            )
                            select new
                            {
                                order,
                                shop
                            }).ToList();
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                    list = list.Where(s => regionList.Contains(s.shop.RegionName) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                }
                if (list.Any())
                {
                    result = "error";
                    List<Shop> shopList = new List<Shop>();
                    Shop shopModel;
                    List<int> shopIdList = new List<int>();
                    list.ForEach(s =>
                    {
                        if (!shopIdList.Contains(s.shop.Id))
                        {
                            shopModel = new Shop();
                            shopModel = s.shop;
                            if (string.IsNullOrWhiteSpace(shopModel.POSScale) && !string.IsNullOrWhiteSpace(s.order.POSScale))
                            {
                                shopModel.POSScale = s.order.POSScale;
                            }
                            if (string.IsNullOrWhiteSpace(shopModel.MaterialSupport) && !string.IsNullOrWhiteSpace(s.order.MaterialSupport))
                            {
                                shopModel.MaterialSupport = s.order.MaterialSupport;
                            }
                            shopList.Add(shopModel);
                            shopIdList.Add(s.shop.Id);
                        }
                        else
                        {
                            for (int i = 0; i < shopList.Count; i++)
                            {
                                if (shopList[i].Id == s.shop.Id)
                                {
                                    if (string.IsNullOrWhiteSpace(shopList[i].POSScale) && !string.IsNullOrWhiteSpace(s.order.POSScale))
                                    {
                                        shopList[i].POSScale = s.order.POSScale;
                                    }
                                    if (string.IsNullOrWhiteSpace(shopList[i].MaterialSupport) && !string.IsNullOrWhiteSpace(s.order.MaterialSupport))
                                    {
                                        shopList[i].MaterialSupport = s.order.MaterialSupport;
                                    }
                                }
                            }
                        }
                    });
                    context1.Session["emptyOrderShop"] = shopList;
                }
                else
                {
                    context1.Session["emptyOrderShop"] = null;
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
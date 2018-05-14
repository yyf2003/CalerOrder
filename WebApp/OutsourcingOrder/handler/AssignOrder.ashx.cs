using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using DAL;
using Models;
using Common;
using System.Transactions;
using System.Configuration;
using System.Web.SessionState;

namespace WebApp.OutsourcingOrder.handler
{
    /// <summary>
    /// AssignOrder 的摘要说明
    /// </summary>
    public class AssignOrder : IHttpHandler, IRequiresSessionState
    {
        HttpContext context1;
        string material0 = "背胶PP+";
        string material1 = "雪弗板";
        string material3 = "蝴蝶支架";
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
                case "getRegion":
                    result = GetRegion();
                    break;
                case "getProvince":
                    result = GetProvince();
                    break;
                case "getCity":
                    result = GetCity();
                    break;
                case "getCityTier":
                    result = GetCityTier();
                    break;
                case "getCS":
                    result = GetCustomerService();
                    break;
                case "getInstallLevel":
                    result = GetInstallLevel();
                    break;
                case "getChannel":
                    result = GetChannel();
                    break;
                case "getFormat":
                    result = GetFormat();
                    break;
                case "getSheet":
                    result = GetSheet();
                    break;
                case "getMaterial":
                    result = GetMaterial();
                    break;
                case "getShopList":
                    result = GetShopList();
                    break;
                case "GetOutsourcing":
                    result = GetOutsourcing();
                    break;
                case "update":
                    result = update();
                    break;
                case "clean":
                    result = cleanOutsorcing();
                    break;
                case "assign":
                    result = Assign();
                    break;
                case "cancelAssign":
                    result = CancelAssign();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }


        string GetRegion()
        {
            context1.Session["orderAssign"] = null;
            context1.Session["shopAssign"] = null;
            int guidanceId = 0;
            string subjectIds = string.Empty;
            List<int> subjectIdList = new List<int>();
            if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            {
                guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            }
            if (context1.Request.Form["subjectIds"] != null)
            {
                subjectIds = context1.Request.Form["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        //join guidance in CurrentContext.DbContext.SubjectGuidance
                        //on subject.GuidanceId equals guidance.ItemId
                        where
                        order.GuidanceId == guidanceId

                        && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType > 1)
                        && (order.IsValid == null || order.IsValid == true)
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())

                        && (order.OrderType != (int)OrderTypeEnum.物料)
                        select new
                        {
                            order,
                            shop

                        }).ToList();
            if (subjectIdList.Any())
            {
                list = list.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
            }
            if (list.Any())
            {
                //int roleId = new BasePage().CurrentUser.RoleId;
                //if (roleId == 5)
                //{
                //    int userId = new BasePage().CurrentUser.UserId;
                //    list = list.Where(s => s.order.CSUserId == userId).ToList();
                //}
                context1.Session["orderAssign"] = list.Select(s => s.order).ToList();
                context1.Session["shopAssign"] = list.Select(s => s.shop).Distinct().ToList();
                List<string> regionList = list.Select(s => s.order.Region).Distinct().OrderBy(s => s).ToList();
                StringBuilder json = new StringBuilder();
                bool isEmpty = false;
                regionList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        json.Append("{\"RegionName\":\"" + s + "\"},");
                    }
                    else
                        isEmpty = true;

                });
                if (isEmpty)
                    json.Append("{\"RegionName\":\"空\"},");
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetProvince()
        {
            string result = string.Empty;
            string regions = string.Empty;
            List<string> regionList = new List<string>();
            if (context1.Request.Form["region"] != null)
            {
                regions = context1.Request.Form["region"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();
            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }
            if (orderList.Any() && regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        orderList = orderList.Where(s => regionList.Contains(s.Region.ToLower()) || s.Region == null || s.Region == "").ToList();

                    }
                    else
                    {
                        orderList = orderList.Where(s => s.Region == null || s.Region == "").ToList();

                    }
                }
                else
                {
                    orderList = orderList.Where(s => regionList.Contains(s.Region.ToLower())).ToList();

                }
                if (orderList.Any())
                {
                    List<string> provinceList = orderList.Select(s => s.Province).Distinct().OrderBy(s => s).ToList();
                    StringBuilder json = new StringBuilder();
                    bool isEmpty = false;
                    provinceList.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            json.Append("{\"ProvinceName\":\"" + s + "\"},");
                        }
                        else
                            isEmpty = true;

                    });
                    if (isEmpty)
                        json.Append("{\"ProvinceName\":\"空\"},");
                    result = "[" + json.ToString().TrimEnd(',') + "]";
                }
            }
            return result;
        }

        string GetCity()
        {
            string result = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            if (context1.Request.Form["region"] != null)
            {
                regions = context1.Request.Form["region"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["province"] != null)
            {
                provinces = context1.Request.Form["province"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();
            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }
            if (orderList.Any() && regionList.Any() && provinceList.Any())
            {

                if (regionList.Any())
                {

                    if (regionList.Contains("空"))
                    {
                        regionList.Remove("空");
                        if (regionList.Any())
                        {
                            orderList = orderList.Where(s => regionList.Contains(s.Region.ToLower()) || s.Region == null || s.Region == "").ToList();

                        }
                        else
                        {
                            orderList = orderList.Where(s => s.Region == null || s.Region == "").ToList();

                        }
                    }
                    else
                    {
                        orderList = orderList.Where(s => regionList.Contains(s.Region.ToLower())).ToList();

                    }
                }
                if (provinceList.Any())
                {

                    if (provinceList.Contains("空"))
                    {
                        provinceList.Remove("空");
                        if (provinceList.Any())
                        {
                            orderList = orderList.Where(s => provinceList.Contains(s.Province) || s.Province == null || s.Province == "").ToList();

                        }
                        else
                        {
                            orderList = orderList.Where(s => s.Province == null || s.Province == "").ToList();

                        }
                    }
                    else
                    {
                        orderList = orderList.Where(s => provinceList.Contains(s.Province)).ToList();

                    }
                }

                if (orderList.Any())
                {
                    List<string> cityList = orderList.Select(s => s.City).Distinct().OrderBy(s => s).ToList();
                    StringBuilder json = new StringBuilder();
                    bool isEmpty = false;
                    cityList.ForEach(s =>
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
                    result = "[" + json.ToString().TrimEnd(',') + "]";
                }
            }
            return result;
        }

        string GetCustomerService()
        {
            string result = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string city = string.Empty;
            string subjectIds = string.Empty;
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> subjectIdList = new List<int>();
            if (context1.Request.Form["region"] != null)
            {
                regions = context1.Request.Form["region"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["province"] != null)
            {
                provinces = context1.Request.Form["province"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["city"] != null)
            {
                city = context1.Request.Form["city"];
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                }
            }
            if (context1.Request.Form["subjectIds"] != null)
            {
                subjectIds = context1.Request.Form["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();

            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }

            var orders = (from order in orderList
                          //join shop in shopList
                          //on order.ShopId equals shop.Id
                          join user1 in CurrentContext.DbContext.UserInfo
                          on order.CSUserId equals user1.UserId into userTemp
                          from user in userTemp.DefaultIfEmpty()
                          select new
                          {
                              order,
                              //shop,
                              user
                              //CSName = user.RealName
                          }).ToList();
            //var materialOrderList = (from order in CurrentContext.DbContext.OrderMaterial
            //                         join shop in CurrentContext.DbContext.Shop
            //                         on order.ShopId equals shop.Id
            //                         join user in CurrentContext.DbContext.UserInfo
            //                         on shop.CSUserId equals user.UserId into userTemp
            //                         from customerService in userTemp.DefaultIfEmpty()
            //                         where subjectIdList.Contains(order.SubjectId ?? 0)
            //                         select new
            //                         {
            //                             shop,
            //                             customerService
            //                             //CSName = customerService.RealName
            //                         }
            //                      ).ToList();
            if (subjectIdList.Any())
            {
                orders = orders.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
            }
            if (regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();
                        //materialOrderList = materialOrderList.Where(s => regionList.Contains(s.shop.RegionName) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                        //materialOrderList = materialOrderList.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                    }
                }
                else
                {
                    orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                    //materialOrderList = materialOrderList.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
                }
            }
            if (provinceList.Any())
            {

                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orders = orders.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();
                        //materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.shop.ProvinceName) || s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Province == null || s.order.Province == "").ToList();
                        //materialOrderList = materialOrderList.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                    }
                }
                else
                {
                    orders = orders.Where(s => provinceList.Contains(s.order.Province)).ToList();
                    //materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        orders = orders.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();
                        //materialOrderList = materialOrderList.Where(s => cityList.Contains(s.shop.CityName) || s.shop.CityName == null || s.shop.CityName == "").ToList();
                    }
                    else
                    {
                        orders = orders.Where(s => s.order.City == null || s.order.City == "").ToList();
                        //materialOrderList = materialOrderList.Where(s => s.shop.CityName == null || s.shop.CityName == "").ToList();
                    }
                }
                else
                {
                    orders = orders.Where(s => cityList.Contains(s.order.City)).ToList();
                    //materialOrderList = materialOrderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }
            }
            List<int> userIdList = new List<int>();
            StringBuilder json = new StringBuilder();
            bool isEmpty = false;
            if (orders.Any())
            {
                var list1 = orders.Where(s => s.user != null).Select(s => new { s.order.CSUserId, s.user }).Distinct().OrderBy(s => s.user.RealName).ToList();
                list1.ForEach(s =>
                {
                    if (s.CSUserId != null && s.CSUserId > 0)
                    {
                        if (!userIdList.Contains(s.CSUserId ?? 0))
                        {
                            userIdList.Add(s.CSUserId ?? 0);
                            json.Append("{\"UserId\":\"" + s.CSUserId + "\",\"UserName\":\"" + s.user.RealName + "\"},");
                        }

                    }
                    else
                        isEmpty = true;
                });
            }
            //if (materialOrderList.Any())
            //{
            //    var list2 = materialOrderList.Select(s => new { s.shop.CSUserId, s.customerService }).Distinct().ToList();
            //    list2.ForEach(s =>
            //    {
            //        if (s.CSUserId != null && s.CSUserId > 0)
            //        {
            //            if (!userIdList.Contains(s.CSUserId ?? 0))
            //            {
            //                userIdList.Add(s.CSUserId ?? 0);
            //                json.Append("{\"UserId\":\"" + s.CSUserId + "\",\"UserName\":\"" + s.customerService.RealName + "\"},");
            //            }

            //        }
            //        else
            //            isEmpty = true;
            //    });
            //}
            if (isEmpty)
            {
                json.Append("{\"UserId\":\"0\",\"UserName\":\"无\"}");
            }
            if (json.Length > 0)
                result = "[" + json.ToString().TrimEnd(',') + "]";
            return result;
        }

        string GetOutsourcing()
        {
            string r = string.Empty;
            List<int> provinceIdList = new List<int>();
            int roleId = new BasePage().CurrentUser.RoleId;

            var companyList = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.IsDelete == null || s.IsDelete == false)).OrderBy(s => s.ProvinceId).ToList();

            if (roleId == 5)
            {
                int userId = new BasePage().CurrentUser.UserId;
                List<int> outsourceList = new OutsourceInUserBLL().GetList(s => s.UserId == userId).Select(s => s.OutsourceId ?? 0).ToList();
                companyList = companyList.Where(s => outsourceList.Contains(s.Id)).ToList();
            }
            if (companyList.Any())
            {
                StringBuilder json = new StringBuilder();
                companyList.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CompanyName\":\"" + s.CompanyName + "\"},");
                });
                r = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return r;
        }

        string GetCityTier()
        {
            //int guidanceId = 0;
            //string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerService = string.Empty;
            //List<int> subjectList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            //if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            //{
            //guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            //}
            //if (context1.Request.Form["subjectIds"] != null)
            //{
            //    subjectIds = context1.Request.Form["subjectIds"];
            //    if (!string.IsNullOrWhiteSpace(subjectIds))
            //    {
            //        subjectList = StringHelper.ToIntList(subjectIds, ',');
            //    }
            //}
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerService = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerService))
                {
                    customerServiceList = StringHelper.ToIntList(customerService, ',');
                }
            }
            //var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //            join shop in CurrentContext.DbContext.Shop
            //            on order.ShopId equals shop.Id
            //            join subject in CurrentContext.DbContext.Subject
            //            on order.SubjectId equals subject.Id
            //            join guidance in CurrentContext.DbContext.SubjectGuidance
            //            on subject.GuidanceId equals guidance.ItemId
            //            where
            //            guidance.ItemId == guidanceId

            //            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 3)

            //            && (order.IsValid == null || order.IsValid == true)
            //            && (order.IsDelete == null || order.IsDelete == false)
            //            && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
            //            && (order.IsProduce == null || order.IsProduce == true)
            //            //&& (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)

            //            select new
            //            {
            //                order,
            //                shop

            //            }).ToList();
            //if (subjectList.Any())
            //{
            //    list = list.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
            //}
            //if (regionList.Any())
            //{
            //    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            //}
            //if (provinceList.Any())
            //{
            //    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            //}
            //if (cityList.Any())
            //{
            //    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            //}

            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();

            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }

            var orders = (from order in orderList
                          join shop in shopList
                          on order.ShopId equals shop.Id
                          select new
                          {
                              order,
                              shop,

                          }).ToList();
            if (regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Region == null || s.order.Region == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();

                }
            }
            if (provinceList.Any())
            {

                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orders = orders.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => provinceList.Contains(s.order.Province)).ToList();

                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        orders = orders.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.City == null || s.order.City == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => cityList.Contains(s.order.City)).ToList();

                }
            }


            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                }
                else
                {
                    orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                }
            }
            if (orders.Any())
            {
                List<string> cityTierList = orders.Select(s => s.order.CityTier).Distinct().OrderBy(s => s).ToList();
                StringBuilder json = new StringBuilder();
                bool isEmpty = false;
                cityTierList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        json.Append("{\"CityTier\":\"" + s + "\"},");
                    }
                    else
                        isEmpty = true;

                });
                if (isEmpty)
                    json.Append("{\"CityTier\":\"空\"},");
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }


        string GetInstallLevel()
        {
            //int guidanceId = 0;
            //string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerService = string.Empty;
            string cityTier = string.Empty;
            //List<int> subjectList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            List<string> cityTierList = new List<string>();
            //if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            //{
            //    guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            //}
            //if (context1.Request.Form["subjectIds"] != null)
            //{
            //    subjectIds = context1.Request.Form["subjectIds"];
            //    if (!string.IsNullOrWhiteSpace(subjectIds))
            //    {
            //        subjectList = StringHelper.ToIntList(subjectIds, ',');
            //    }
            //}
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerService = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerService))
                {
                    customerServiceList = StringHelper.ToIntList(customerService, ',');
                }
            }
            if (context1.Request.Form["cityTier"] != null)
            {
                cityTier = context1.Request.Form["cityTier"];
                if (!string.IsNullOrWhiteSpace(cityTier))
                {
                    cityTierList = StringHelper.ToStringList(cityTier, ',');
                }
            }
            //var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //            join shop in CurrentContext.DbContext.Shop
            //            on order.ShopId equals shop.Id
            //            join subject in CurrentContext.DbContext.Subject
            //            on order.SubjectId equals subject.Id
            //            join guidance in CurrentContext.DbContext.SubjectGuidance
            //            on subject.GuidanceId equals guidance.ItemId
            //            where
            //            guidance.ItemId == guidanceId

            //            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 3)

            //            && (order.IsValid == null || order.IsValid == true)
            //            && (order.IsDelete == null || order.IsDelete == false)
            //            && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
            //            && (order.IsProduce == null || order.IsProduce == true)
            //            //&& (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)

            //            select new
            //            {
            //                order,
            //                shop

            //            }).ToList();
            //if (subjectList.Any())
            //{
            //    list = list.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
            //}
            //if (regionList.Any())
            //{
            //    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            //}
            //if (provinceList.Any())
            //{
            //    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            //}
            //if (cityList.Any())
            //{
            //    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            //}
            //if (customerServiceList.Any())
            //{
            //    if (customerServiceList.Contains(0))
            //    {
            //        customerServiceList.Remove(0);
            //        if (customerServiceList.Any())
            //        {
            //            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
            //        }
            //        else
            //            list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
            //    }
            //    else
            //    {
            //        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
            //    }
            //}
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();

            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }

            var orders = (from order in orderList
                          join shop in shopList
                          on order.ShopId equals shop.Id
                          select new
                          {
                              order,
                              shop,

                          }).ToList();
            if (regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Region == null || s.order.Region == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();

                }
            }
            if (provinceList.Any())
            {

                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orders = orders.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => provinceList.Contains(s.order.Province)).ToList();

                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        orders = orders.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.City == null || s.order.City == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => cityList.Contains(s.order.City)).ToList();

                }
            }


            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                }
                else
                {
                    orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                }
            }
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        orders = orders.Where(s => cityTierList.Contains(s.order.CityTier) || (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                }
                else
                {
                    orders = orders.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                }
            }
            if (orders.Any())
            {
                List<string> isInstallList = orders.Select(s => s.order.IsInstall).Distinct().OrderBy(s => s).ToList();
                StringBuilder json = new StringBuilder();
                bool isEmpty = false;
                isInstallList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        json.Append("{\"IsInstall\":\"" + s + "\"},");
                    }
                    else
                        isEmpty = true;

                });
                if (isEmpty)
                    json.Append("{\"IsInstall\":\"空\"},");
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }


        string GetChannel()
        {
            //int guidanceId = 0;
            //string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerService = string.Empty;
            string cityTier = string.Empty;
            string isInstall = string.Empty;
            List<string> isInstallList = new List<string>();
            //List<int> subjectList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            List<string> cityTierList = new List<string>();
            //if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            //{
            //    guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            //}
            //if (context1.Request.Form["subjectIds"] != null)
            //{
            //    subjectIds = context1.Request.Form["subjectIds"];
            //    if (!string.IsNullOrWhiteSpace(subjectIds))
            //    {
            //        subjectList = StringHelper.ToIntList(subjectIds, ',');
            //    }
            //}
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerService = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerService))
                {
                    customerServiceList = StringHelper.ToIntList(customerService, ',');
                }
            }
            if (context1.Request.Form["cityTier"] != null)
            {
                cityTier = context1.Request.Form["cityTier"];
                if (!string.IsNullOrWhiteSpace(cityTier))
                {
                    cityTierList = StringHelper.ToStringList(cityTier, ',');
                }
            }
            if (context1.Request.Form["isInstall"] != null)
            {
                isInstall = context1.Request.Form["isInstall"];
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    isInstallList = StringHelper.ToStringList(isInstall, ',');
                }
            }
            //var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //            join shop in CurrentContext.DbContext.Shop
            //            on order.ShopId equals shop.Id
            //            join subject in CurrentContext.DbContext.Subject
            //            on order.SubjectId equals subject.Id
            //            join guidance in CurrentContext.DbContext.SubjectGuidance
            //            on subject.GuidanceId equals guidance.ItemId
            //            where
            //            guidance.ItemId == guidanceId

            //            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 3)

            //            && (order.IsValid == null || order.IsValid == true)
            //            && (order.IsDelete == null || order.IsDelete == false)
            //            && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
            //            && (order.IsProduce == null || order.IsProduce == true)
            //            //&& (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)

            //            select new
            //            {
            //                order,
            //                shop

            //            }).ToList();
            //if (subjectList.Any())
            //{
            //    list = list.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
            //}
            //if (regionList.Any())
            //{
            //    list = list.Where(s => s.shop.RegionName != null && regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            //}
            //if (provinceList.Any())
            //{
            //    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            //}
            //if (cityList.Any())
            //{
            //    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            //}
            //if (customerServiceList.Any())
            //{
            //    if (customerServiceList.Contains(0))
            //    {
            //        customerServiceList.Remove(0);
            //        if (customerServiceList.Any())
            //        {
            //            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
            //        }
            //        else
            //            list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
            //    }
            //    else
            //    {
            //        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
            //    }
            //}
            //if (cityTierList.Any())
            //{
            //    if (cityTierList.Contains("空"))
            //    {
            //        cityTierList.Remove("空");
            //        if (cityTierList.Any())
            //        {
            //            list = list.Where(s => cityTierList.Contains(s.shop.CityTier) || (s.shop.CityTier == null || s.shop.CityTier == "")).ToList();
            //        }
            //        else
            //            list = list.Where(s => (s.shop.CityTier == null || s.shop.CityTier == "")).ToList();
            //    }
            //    else
            //    {
            //        list = list.Where(s => cityTierList.Contains(s.shop.CityTier)).ToList();
            //    }
            //}
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();

            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }

            var orders = (from order in orderList
                          join shop in shopList
                          on order.ShopId equals shop.Id
                          select new
                          {
                              order,
                              shop,

                          }).ToList();
            if (regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Region == null || s.order.Region == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();

                }
            }
            if (provinceList.Any())
            {

                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orders = orders.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => provinceList.Contains(s.order.Province)).ToList();

                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        orders = orders.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.City == null || s.order.City == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => cityList.Contains(s.order.City)).ToList();

                }
            }
            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                }
                else
                {
                    orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                }
            }
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        orders = orders.Where(s => cityTierList.Contains(s.order.CityTier) || (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                }
                else
                {
                    orders = orders.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                }
            }
            if (isInstallList.Any())
            {
                if (isInstallList.Contains("空"))
                {
                    isInstallList.Remove("空");
                    if (isInstallList.Any())
                    {
                        orders = orders.Where(s => isInstallList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                    }
                }
                else
                {
                    orders = orders.Where(s => isInstallList.Contains(s.order.IsInstall)).ToList();

                }
            }
            if (orders.Any())
            {
                List<string> channelList = orders.Select(s => s.order.Channel).Distinct().OrderBy(s => s).ToList();
                StringBuilder json = new StringBuilder();
                bool isEmpty = false;

                channelList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        json.Append("{\"Channel\":\"" + s + "\"},");
                    }
                    else
                        isEmpty = true;

                });

                if (isEmpty)
                    json.Append("{\"Channel\":\"空\"},");

                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetFormat()
        {
            //int guidanceId = 0;
            //string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerService = string.Empty;
            string cityTier = string.Empty;
            string isInstall = string.Empty;
            string channel = string.Empty;
            List<string> isInstallList = new List<string>();
            //List<int> subjectList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            List<string> cityTierList = new List<string>();
            List<string> channelList = new List<string>();
            //if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            //{
            //    guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            //}
            //if (context1.Request.Form["subjectIds"] != null)
            //{
            //    subjectIds = context1.Request.Form["subjectIds"];
            //    if (!string.IsNullOrWhiteSpace(subjectIds))
            //    {
            //        subjectList = StringHelper.ToIntList(subjectIds, ',');
            //    }
            //}
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerService = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerService))
                {
                    customerServiceList = StringHelper.ToIntList(customerService, ',');
                }
            }
            if (context1.Request.Form["cityTier"] != null)
            {
                cityTier = context1.Request.Form["cityTier"];
                if (!string.IsNullOrWhiteSpace(cityTier))
                {
                    cityTierList = StringHelper.ToStringList(cityTier, ',');
                }
            }
            if (context1.Request.Form["isInstall"] != null)
            {
                isInstall = context1.Request.Form["isInstall"];
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    isInstallList = StringHelper.ToStringList(isInstall, ',');
                }
            }
            if (context1.Request.Form["channel"] != null)
            {
                channel = context1.Request.Form["channel"];
                if (!string.IsNullOrWhiteSpace(channel))
                {
                    channelList = StringHelper.ToStringList(channel, ',');
                }
            }
            //var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //            join shop in CurrentContext.DbContext.Shop
            //            on order.ShopId equals shop.Id
            //            join subject in CurrentContext.DbContext.Subject
            //            on order.SubjectId equals subject.Id
            //            join guidance in CurrentContext.DbContext.SubjectGuidance
            //            on subject.GuidanceId equals guidance.ItemId
            //            where
            //            guidance.ItemId == guidanceId

            //            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 3)

            //            && (order.IsValid == null || order.IsValid == true)
            //            && (order.IsDelete == null || order.IsDelete == false)
            //            && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
            //            && (order.IsProduce == null || order.IsProduce == true)
            //            //&& (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)

            //            select new
            //            {
            //                order,
            //                shop

            //            }).ToList();
            //if (subjectList.Any())
            //{
            //    list = list.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
            //}
            //if (regionList.Any())
            //{
            //    list = list.Where(s => s.shop.RegionName != null && regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            //}
            //if (provinceList.Any())
            //{
            //    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            //}
            //if (cityList.Any())
            //{
            //    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            //}
            //if (customerServiceList.Any())
            //{
            //    if (customerServiceList.Contains(0))
            //    {
            //        customerServiceList.Remove(0);
            //        if (customerServiceList.Any())
            //        {
            //            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
            //        }
            //        else
            //            list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
            //    }
            //    else
            //    {
            //        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
            //    }
            //}
            //if (cityTierList.Any())
            //{
            //    if (cityTierList.Contains("空"))
            //    {
            //        cityTierList.Remove("空");
            //        if (cityTierList.Any())
            //        {
            //            list = list.Where(s => cityTierList.Contains(s.shop.CityTier) || (s.shop.CityTier == null || s.shop.CityTier == "")).ToList();
            //        }
            //        else
            //            list = list.Where(s => (s.shop.CityTier == null || s.shop.CityTier == "")).ToList();
            //    }
            //    else
            //    {
            //        list = list.Where(s => cityTierList.Contains(s.shop.CityTier)).ToList();
            //    }
            //}
            //if (isInstallList.Any())
            //{
            //    if (isInstallList.Contains("空"))
            //    {
            //        isInstallList.Remove("空");
            //        if (isInstallList.Any())
            //        {
            //            list = list.Where(s => isInstallList.Contains(s.shop.IsInstall) || (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();

            //        }
            //        else
            //        {
            //            list = list.Where(s => (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();

            //        }
            //    }
            //    else
            //    {
            //        list = list.Where(s => isInstallList.Contains(s.shop.IsInstall)).ToList();

            //    }
            //}
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();

            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }

            var orders = (from order in orderList
                          join shop in shopList
                          on order.ShopId equals shop.Id
                          select new
                          {
                              order,
                              shop,

                          }).ToList();
            if (regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Region == null || s.order.Region == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();

                }
            }
            if (provinceList.Any())
            {

                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orders = orders.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => provinceList.Contains(s.order.Province)).ToList();

                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        orders = orders.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.City == null || s.order.City == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => cityList.Contains(s.order.City)).ToList();

                }
            }
            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                }
                else
                {
                    orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                }
            }
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        orders = orders.Where(s => cityTierList.Contains(s.order.CityTier) || (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                }
                else
                {
                    orders = orders.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                }
            }
            if (isInstallList.Any())
            {
                if (isInstallList.Contains("空"))
                {
                    isInstallList.Remove("空");
                    if (isInstallList.Any())
                    {
                        orders = orders.Where(s => isInstallList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                    }
                }
                else
                {
                    orders = orders.Where(s => isInstallList.Contains(s.order.IsInstall)).ToList();

                }
            }
            if (channelList.Any())
            {
                if (channelList.Contains("空"))
                {
                    channelList.Remove("空");
                    if (channelList.Any())
                    {
                        orders = orders.Where(s => channelList.Contains(s.order.Channel) || (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => channelList.Contains(s.order.Channel)).ToList();

                }
            }
            if (orders.Any())
            {
                List<string> formatList = orders.Select(s => s.order.Format).Distinct().OrderBy(s => s).ToList();
                StringBuilder json = new StringBuilder();
                bool isEmpty = false;
                formatList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        json.Append("{\"Format\":\"" + s + "\"},");
                    }
                    else
                        isEmpty = true;

                });

                if (isEmpty)
                    json.Append("{\"Format\":\"空\"},");
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetSheet()
        {
            //int guidanceId = 0;
            //string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerService = string.Empty;
            string cityTier = string.Empty;
            string isInstall = string.Empty;
            string channel = string.Empty;
            string format = string.Empty;
            List<string> isInstallList = new List<string>();
            //List<int> subjectList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            List<string> cityTierList = new List<string>();
            List<string> channelList = new List<string>();
            List<string> formatList = new List<string>();

            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerService = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerService))
                {
                    customerServiceList = StringHelper.ToIntList(customerService, ',');
                }
            }
            if (context1.Request.Form["cityTier"] != null)
            {
                cityTier = context1.Request.Form["cityTier"];
                if (!string.IsNullOrWhiteSpace(cityTier))
                {
                    cityTierList = StringHelper.ToStringList(cityTier, ',');
                }
            }
            if (context1.Request.Form["isInstall"] != null)
            {
                isInstall = context1.Request.Form["isInstall"];
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    isInstallList = StringHelper.ToStringList(isInstall, ',');
                }
            }
            if (context1.Request.Form["channel"] != null)
            {
                channel = context1.Request.Form["channel"];
                if (!string.IsNullOrWhiteSpace(channel))
                {
                    channelList = StringHelper.ToStringList(channel, ',');
                }
            }
            if (context1.Request.Form["format"] != null)
            {
                format = context1.Request.Form["format"];
                if (!string.IsNullOrWhiteSpace(format))
                {
                    formatList = StringHelper.ToStringList(format, ',');
                }
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();

            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }

            var orders = (from order in orderList
                          join shop in shopList
                          on order.ShopId equals shop.Id
                          select new
                          {
                              order,
                              shop,

                          }).ToList();
            if (regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Region == null || s.order.Region == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();

                }
            }
            if (provinceList.Any())
            {

                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orders = orders.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => provinceList.Contains(s.order.Province)).ToList();

                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        orders = orders.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => s.order.City == null || s.order.City == "").ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => cityList.Contains(s.order.City)).ToList();

                }
            }
            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                }
                else
                {
                    orders = orders.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                }
            }
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        orders = orders.Where(s => cityTierList.Contains(s.order.CityTier) || (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                    }
                    else
                        orders = orders.Where(s => (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                }
                else
                {
                    orders = orders.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                }
            }
            if (isInstallList.Any())
            {
                if (isInstallList.Contains("空"))
                {
                    isInstallList.Remove("空");
                    if (isInstallList.Any())
                    {
                        orders = orders.Where(s => isInstallList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                    }
                }
                else
                {
                    orders = orders.Where(s => isInstallList.Contains(s.order.IsInstall)).ToList();

                }
            }
            if (channelList.Any())
            {
                if (channelList.Contains("空"))
                {
                    channelList.Remove("空");
                    if (channelList.Any())
                    {
                        orders = orders.Where(s => channelList.Contains(s.order.Channel) || (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => channelList.Contains(s.order.Channel)).ToList();

                }
            }
            if (formatList.Any())
            {
                if (formatList.Contains("空"))
                {
                    formatList.Remove("空");
                    if (formatList.Any())
                    {
                        orders = orders.Where(s => formatList.Contains(s.order.Format) || (s.order.Format == null || s.order.Format == "")).ToList();

                    }
                    else
                    {
                        orders = orders.Where(s => (s.order.Format == null || s.order.Format == "")).ToList();

                    }
                }
                else
                {
                    orders = orders.Where(s => formatList.Contains(s.order.Format)).ToList();

                }
            }
            if (orders.Any())
            {
                List<string> sheetList = orders.Select(s => s.order.Sheet).Distinct().OrderBy(s => s).ToList();
                StringBuilder json = new StringBuilder();
                bool isEmpty = false;
                sheetList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        json.Append("{\"Sheet\":\"" + s + "\"},");
                    }
                    else
                        isEmpty = true;

                });
                if (isEmpty)
                    json.Append("{\"Sheet\":\"空\"},");
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetMaterial()
        {
            int guidanceId = 0;
            string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerService = string.Empty;
            string cityTier = string.Empty;
            string isInstall = string.Empty;
            List<string> isInstallList = new List<string>();
            List<int> subjectList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            List<string> cityTierList = new List<string>();
            if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            {
                guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            }
            if (context1.Request.Form["subjectIds"] != null)
            {
                subjectIds = context1.Request.Form["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerService = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerService))
                {
                    customerServiceList = StringHelper.ToIntList(customerService, ',');
                }
            }
            if (context1.Request.Form["cityTier"] != null)
            {
                cityTier = context1.Request.Form["cityTier"];
                if (!string.IsNullOrWhiteSpace(cityTier))
                {
                    cityTierList = StringHelper.ToStringList(cityTier, ',');
                }
            }
            if (context1.Request.Form["isInstall"] != null)
            {
                isInstall = context1.Request.Form["isInstall"];
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    isInstallList = StringHelper.ToStringList(isInstall, ',');
                }
            }
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance.ItemId
                        where
                        guidance.ItemId == guidanceId

                        && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 1)

                        && (order.IsValid == null || order.IsValid == true)
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                        && (order.IsProduce == null || order.IsProduce == true)
                            //&& (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                        && order.OrderType != (int)OrderTypeEnum.物料
                        select new
                        {
                            order,
                            shop

                        }).ToList();
            if (subjectList.Any())
            {
                list = list.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
            }
            if (regionList.Any())
            {
                list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
            }
            if (provinceList.Any())
            {
                list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();
            }
            if (cityList.Any())
            {
                list = list.Where(s => cityList.Contains(s.order.City)).ToList();
            }
            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        list = list.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                    }
                    else
                        list = list.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                }
                else
                {
                    list = list.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                }
            }
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        list = list.Where(s => cityTierList.Contains(s.order.CityTier) || (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                    }
                    else
                        list = list.Where(s => (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                }
                else
                {
                    list = list.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                }
            }
            if (isInstallList.Any())
            {
                if (isInstallList.Contains("空"))
                {
                    isInstallList.Remove("空");
                    if (isInstallList.Any())
                    {
                        list = list.Where(s => isInstallList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();

                    }
                    else
                    {
                        list = list.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => isInstallList.Contains(s.order.IsInstall)).ToList();

                }
            }
            if (list.Any())
            {
                List<string> materialList = list.Select(s => s.order.GraphicMaterial).Distinct().ToList();
                StringBuilder json = new StringBuilder();
                bool isEmpty = false;
                List<string> mList = new List<string>();
                materialList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        string material = new BasePage().GetBasicMaterial(s);
                        if (string.IsNullOrWhiteSpace(material))
                            material = s;
                        if (!mList.Contains(material))
                        {
                            mList.Add(material);
                        }

                    }
                    else
                        isEmpty = true;

                });
                mList.OrderBy(s => s);
                if (isEmpty)
                    mList.Add("空");
                mList.ForEach(s =>
                {
                    json.Append("{\"GraphicMaterial\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string update()
        {
            string result = "提交失败";
            int guidanceId = -1;
            string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string cities = string.Empty;
            string orderIds = string.Empty;
            int companyId = 0;
            if (context1.Request.Form["subjectids"] != null)
            {
                subjectIds = context1.Request.Form["subjectids"];
            }
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
            }
            if (context1.Request.Form["province"] != null)
            {
                provinces = context1.Request.Form["province"];
            }
            if (context1.Request.Form["city"] != null)
            {
                cities = context1.Request.Form["city"];
            }
            if (context1.Request.Form["orderIds"] != null)
            {
                orderIds = context1.Request.Form["orderIds"];
            }
            if (context1.Request.Form["companyId"] != null)
            {
                companyId = int.Parse(context1.Request.Form["companyId"]);
            }
            if (!string.IsNullOrWhiteSpace(subjectIds) && companyId > 0)
            {
                List<int> subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 join pop1 in CurrentContext.DbContext.POP
                                 on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                                 from pop in popTemp.DefaultIfEmpty()
                                 where subjectIdList.Contains(order.SubjectId ?? 0)
                                 && ((order.OrderType != null && order.OrderType == 1 && order.GraphicWidth != null && order.GraphicWidth != 0 && order.GraphicLength != null && order.GraphicLength != 0) || (order.OrderType != null && order.OrderType == 2))
                                 && (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
                                 && (order.IsDelete == null || order.IsDelete == false)
                                 select new
                                 {
                                     order,
                                     shop
                                 }).ToList();
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    List<string> regionList = StringHelper.ToStringList(regions, ',');
                    orderList = orderList.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    List<string> provinceList = StringHelper.ToStringList(provinces, ',');
                    orderList = orderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(cities))
                {
                    List<string> cityList = StringHelper.ToStringList(cities, ',');
                    orderList = orderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(orderIds))
                {
                    List<int> orderIdList = StringHelper.ToIntList(orderIds, ',');
                    orderList = orderList.Where(s => orderIdList.Contains(s.order.Id)).ToList();
                }
                if (orderList.Any())
                {
                    FinalOrderDetailTemp orderModel;
                    FinalOrderDetailTempBLL bll = new FinalOrderDetailTempBLL();
                    StringBuilder materialMsg = new StringBuilder();
                    int successNum = 0;
                    orderList.ForEach(s =>
                    {
                        decimal materialPrice = 0;
                        if (!string.IsNullOrWhiteSpace(s.order.GraphicMaterial))
                            materialPrice = GetMaterialPrice(s.order.GraphicMaterial, companyId);
                        if (materialPrice == -1)
                        {
                            materialMsg.Append(s.order.GraphicMaterial + ",");
                        }
                        else
                        {
                            orderModel = new FinalOrderDetailTemp();
                            orderModel = s.order;
                            orderModel.OutsourceId = companyId;
                            //orderModel.OutsourcePrice = materialPrice;
                            bll.Update(orderModel);
                            successNum++;
                        }

                    });
                    if (materialMsg.Length > 0)
                    {
                        if (successNum > 0)
                            result = "部分提交失败，以下材质没提交报价：" + materialMsg.ToString().TrimEnd(',');
                        else
                            result = "提交失败，以下材质没提交报价：" + materialMsg.ToString().TrimEnd(',');
                    }
                    else
                        result = "ok";
                }

            }

            return result;
        }

        /// <summary>
        /// 清除已分配的外协
        /// </summary>
        /// <returns></returns>
        string cleanOutsorcing()
        {
            string result = "清空失败";
            string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string cities = string.Empty;
            string orderIds = string.Empty;

            if (context1.Request.Form["subjectids"] != null)
            {
                subjectIds = context1.Request.Form["subjectids"];
            }
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
            }
            if (context1.Request.Form["province"] != null)
            {
                provinces = context1.Request.Form["province"];
            }
            if (context1.Request.Form["city"] != null)
            {
                cities = context1.Request.Form["city"];
            }
            if (context1.Request.Form["orderIds"] != null)
            {
                orderIds = context1.Request.Form["orderIds"];
            }

            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                List<int> subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 join pop1 in CurrentContext.DbContext.POP
                                 on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                                 from pop in popTemp.DefaultIfEmpty()
                                 where subjectIdList.Contains(order.SubjectId ?? 0)
                                 && ((order.OrderType != null && order.OrderType == 1 && order.GraphicWidth != null && order.GraphicWidth != 0 && order.GraphicLength != null && order.GraphicLength != 0) || (order.OrderType != null && order.OrderType == 2))
                                 && (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
                                 && (order.IsDelete == null || order.IsDelete == false)
                                 select new
                                 {
                                     order,
                                     shop
                                 }).ToList();
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    List<string> regionList = StringHelper.ToStringList(regions, ',');
                    orderList = orderList.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    List<string> provinceList = StringHelper.ToStringList(provinces, ',');
                    orderList = orderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(cities))
                {
                    List<string> cityList = StringHelper.ToStringList(cities, ',');
                    orderList = orderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(orderIds))
                {
                    List<int> orderIdList = StringHelper.ToIntList(orderIds, ',');
                    orderList = orderList.Where(s => orderIdList.Contains(s.order.Id)).ToList();
                }
                if (orderList.Any())
                {
                    FinalOrderDetailTemp orderModel;
                    FinalOrderDetailTempBLL bll = new FinalOrderDetailTempBLL();
                    orderList.ForEach(s =>
                    {

                        orderModel = new FinalOrderDetailTemp();
                        orderModel = s.order;
                        //orderModel.OutsourcePrice = null;
                        orderModel.OutsourceId = 0;
                        bll.Update(orderModel);

                    });
                    result = "ok";
                }

            }

            return result;
        }

        /// <summary>
        /// 获取外协材质报价
        /// </summary>
        /// <param name="materialName"></param>
        /// <returns></returns>
        Dictionary<string, decimal> dic = new Dictionary<string, decimal>();
        OutsourceMaterialPriceBLL outsourcePriceBll = new OutsourceMaterialPriceBLL();
        decimal GetMaterialPrice(string materialName, int companyId)
        {
            decimal price = 0;
            try
            {
                if (dic.Keys.Contains(materialName.ToLower()))
                {
                    price = dic[materialName.ToLower()];
                }
                else
                {
                    int basicMaterialId = (from mpping in CurrentContext.DbContext.OrderMaterialMpping
                                           join customerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                                           on mpping.CustomerMaterialId equals customerMaterial.Id
                                           where mpping.OrderMaterialName.ToLower() == materialName.ToLower()
                                           select customerMaterial.BasicMaterialId
                                           ).FirstOrDefault() ?? 0;
                    price = outsourcePriceBll.GetList(s => s.BasicMaterialId == basicMaterialId && s.OutsourceId == companyId).Select(s => s.InstallPrice).FirstOrDefault() ?? -1;
                    dic.Add(materialName.ToLower(), price);
                }
            }
            catch
            {
                price = -1;
            }
            return price;
        }


        List<int> subjectList = new List<int>();
        string GetShopList()
        {
            string result = string.Empty;
            int currPage = int.Parse(context1.Request.Form["currpage"]);
            int pageSize = int.Parse(context1.Request.Form["pagesize"]);
            int guidanceId = -1;

            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            List<string> shopNoList = new List<string>();
            List<string> cityTierList = new List<string>();
            List<string> isInstallList = new List<string>();
            List<string> channelList = new List<string>();
            List<string> formatList = new List<string>();
            List<string> sheetList = new List<string>();
            //string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerServiceIds = string.Empty;
            string cityTier = string.Empty;
            string isInstall = string.Empty;
            string channel = string.Empty;
            string format = string.Empty;
            string sheet = string.Empty;
            int isSearch = 0;
            string assignState = "";
            string shopNo = string.Empty;
            int ruanmo = 0;
            string materialAssign = string.Empty;
            string materialPlan = string.Empty;
            string otherMaterial = string.Empty;
            if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            {
                guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            }
            if (context1.Request.Form["subjectIds"] != null)
            {
                subjectIds = context1.Request.Form["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerServiceIds = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerServiceIds))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceIds, ',');
                }
            }
            if (context1.Request.Form["cityTier"] != null)
            {
                cityTier = context1.Request.Form["cityTier"];
                if (!string.IsNullOrWhiteSpace(cityTier))
                {
                    cityTierList = StringHelper.ToStringList(cityTier, ',');
                }
            }
            if (context1.Request.Form["isInstall"] != null)
            {
                isInstall = context1.Request.Form["isInstall"];
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    isInstallList = StringHelper.ToStringList(isInstall, ',');
                }
            }
            if (context1.Request.Form["channel"] != null)
            {
                channel = context1.Request.Form["channel"];
                if (!string.IsNullOrWhiteSpace(channel))
                {
                    channelList = StringHelper.ToStringList(channel, ',');
                }
            }
            if (context1.Request.Form["format"] != null)
            {
                format = context1.Request.Form["format"];
                if (!string.IsNullOrWhiteSpace(format))
                {
                    formatList = StringHelper.ToStringList(format, ',');
                }
            }
            if (context1.Request.Form["sheet"] != null)
            {
                sheet = context1.Request.Form["sheet"];
                if (!string.IsNullOrWhiteSpace(sheet))
                {
                    sheetList = StringHelper.ToStringList(sheet, ',');
                }
            }
            if (context1.Request.Form["isSearch"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["isSearch"]))
            {
                isSearch = int.Parse(context1.Request.Form["isSearch"]);

            }
            if (context1.Request.Form["assignState"] != null)
            {
                assignState = context1.Request.Form["assignState"];
            }
            if (context1.Request.Form["shopNo"] != null)
            {
                shopNo = context1.Request.Form["shopNo"];
                if (!string.IsNullOrWhiteSpace(shopNo))
                {
                    shopNoList = StringHelper.ToStringList(shopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["ruanmo"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["ruanmo"]))
            {
                ruanmo = int.Parse(context1.Request.Form["ruanmo"]);
            }
            if (context1.Request.Form["materialAssign"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["materialAssign"]))
            {
                materialAssign = context1.Request.Form["materialAssign"];
            }
            if (context1.Request.Form["materialPlan"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["materialPlan"]))
            {
                materialPlan = context1.Request.Form["materialPlan"];
            }
            if (context1.Request.Form["otherMaterial"] != null)
            {
                otherMaterial = context1.Request.Form["otherMaterial"];
            }
            List<string> myRegionList = new BasePage().GetResponsibleRegion;
            if (myRegionList.Any())
            {
                StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
            }

            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();
            if (context1.Session["orderAssign"] != null && context1.Session["shopAssign"] != null)
            {
                orderList = context1.Session["orderAssign"] as List<FinalOrderDetailTemp>;
                shopList = context1.Session["shopAssign"] as List<Shop>;
            }
            var list = (from order in orderList
                        join shop1 in shopList
                        on order.ShopId equals shop1.Id into temp
                        from shop in temp.DefaultIfEmpty()
                        select new
                        {
                            order,
                            shop,

                        }).ToList();
            OutsourceOrderDetailBLL assignOrderDetalBll = new OutsourceOrderDetailBLL();
            //var assignOrderDetalList= assignOrderDetalBll.GetList(a => a.GuidanceId == guidanceId);
            var assignOrderDetalList = (from assignOrder in CurrentContext.DbContext.OutsourceOrderDetail
                                        join shop in CurrentContext.DbContext.Shop
                                        on assignOrder.ShopId equals shop.Id
                                        join outsource in CurrentContext.DbContext.Company
                                        on assignOrder.OutsourceId equals outsource.Id
                                        where assignOrder.GuidanceId == guidanceId
                                        select new
                                        {
                                            assignOrder,
                                            outsource,
                                            shop
                                        }).ToList();
            if (subjectList.Any())
            {
                list = list.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
                assignOrderDetalList = assignOrderDetalList.Where(s => subjectList.Contains(s.assignOrder.SubjectId ?? 0)).ToList();

            }

            if (regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => regionList.Contains(s.assignOrder.Region.ToLower()) || s.assignOrder.Region == null || s.assignOrder.Region == "").ToList();
                    }
                    else
                    {
                        list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.Region == null || s.assignOrder.Region == "").ToList();
                    }
                }
                else
                {
                    list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => regionList.Contains(s.assignOrder.Region.ToLower())).ToList();
                }
            }
            if (provinceList.Any())
            {

                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        list = list.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => provinceList.Contains(s.assignOrder.Province) || s.assignOrder.Province == null || s.assignOrder.Province == "").ToList();
                    }
                    else
                    {
                        list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.Province == null || s.assignOrder.Province == "").ToList();
                    }
                }
                else
                {
                    list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => provinceList.Contains(s.assignOrder.Province)).ToList();
                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        list = list.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => cityList.Contains(s.assignOrder.City) || s.assignOrder.City == null || s.assignOrder.City == "").ToList();
                    }
                    else
                    {
                        list = list.Where(s => s.order.City == null || s.order.City == "").ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.City == null || s.assignOrder.City == "").ToList();
                    }
                }
                else
                {
                    list = list.Where(s => cityList.Contains(s.order.City)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => cityList.Contains(s.assignOrder.City)).ToList();
                }
            }
            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        list = list.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => customerServiceList.Contains(s.assignOrder.CSUserId ?? 0) || (s.assignOrder.CSUserId == null || s.assignOrder.CSUserId == 0)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => (s.assignOrder.CSUserId == null || s.assignOrder.CSUserId == 0)).ToList();
                    }
                }
                else
                {
                    list = list.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => customerServiceList.Contains(s.assignOrder.CSUserId ?? 0)).ToList();
                }
            }
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        list = list.Where(s => cityTierList.Contains(s.order.CityTier) || (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => cityTierList.Contains(s.assignOrder.CityTier) || (s.assignOrder.CityTier == null || s.assignOrder.CityTier == "")).ToList();
                    }
                    else
                    {
                        list = list.Where(s => (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => (s.assignOrder.CityTier == null || s.assignOrder.CityTier == "")).ToList();
                    }
                }
                else
                {
                    list = list.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => cityTierList.Contains(s.assignOrder.CityTier)).ToList();
                }
            }
            if (isInstallList.Any())
            {
                if (isInstallList.Contains("空"))
                {
                    isInstallList.Remove("空");
                    if (isInstallList.Any())
                    {
                        list = list.Where(s => isInstallList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => isInstallList.Contains(s.assignOrder.IsInstall) || (s.assignOrder.IsInstall == null || s.assignOrder.IsInstall == "")).ToList();
                    }
                    else
                    {
                        list = list.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => (s.assignOrder.IsInstall == null || s.assignOrder.IsInstall == "")).ToList();
                    }
                }
                else
                {
                    list = list.Where(s => isInstallList.Contains(s.order.IsInstall)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => isInstallList.Contains(s.assignOrder.IsInstall)).ToList();
                }
            }

            if (channelList.Any())
            {
                if (channelList.Contains("空"))
                {
                    channelList.Remove("空");
                    if (channelList.Any())
                    {
                        list = list.Where(s => channelList.Contains(s.order.Channel) || (s.order.Channel == null || s.order.Channel == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => channelList.Contains(s.assignOrder.Channel) || (s.assignOrder.Channel == null || s.assignOrder.Channel == "")).ToList();
                    }
                    else
                    {
                        list = list.Where(s => (s.order.Channel == null || s.order.Channel == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => (s.assignOrder.Channel == null || s.assignOrder.Channel == "")).ToList();
                    }
                }
                else
                {
                    list = list.Where(s => channelList.Contains(s.order.Channel)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => channelList.Contains(s.assignOrder.Channel)).ToList();
                }
            }
            if (formatList.Any())
            {
                if (formatList.Contains("空"))
                {
                    formatList.Remove("空");
                    if (formatList.Any())
                    {
                        list = list.Where(s => formatList.Contains(s.order.Format) || (s.order.Format == null || s.order.Format == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => formatList.Contains(s.assignOrder.Format) || (s.assignOrder.Format == null || s.assignOrder.Format == "")).ToList();
                    }
                    else
                    {
                        list = list.Where(s => (s.order.Format == null || s.order.Format == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => (s.assignOrder.Format == null || s.assignOrder.Format == "")).ToList();
                    }
                }
                else
                {
                    list = list.Where(s => formatList.Contains(s.order.Format)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => formatList.Contains(s.assignOrder.Format)).ToList();
                }
            }
            if (sheetList.Any())
            {
                if (sheetList.Contains("空"))
                {
                    sheetList.Remove("空");
                    if (sheetList.Any())
                    {
                        list = list.Where(s => sheetList.Contains(s.order.Sheet) || (s.order.Sheet == null || s.order.Sheet == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => sheetList.Contains(s.assignOrder.Sheet) || (s.assignOrder.Sheet == null || s.assignOrder.Sheet == "")).ToList();
                    }
                    else
                    {
                        list = list.Where(s => (s.order.Sheet == null || s.order.Sheet == "")).ToList();
                        assignOrderDetalList = assignOrderDetalList.Where(s => (s.assignOrder.Sheet == null || s.assignOrder.Sheet == "")).ToList();
                    }
                }
                else
                {
                    list = list.Where(s => sheetList.Contains(s.order.Sheet)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => sheetList.Contains(s.assignOrder.Sheet)).ToList();
                }
            }

            if (shopNoList.Any())
            {
                list = list.Where(s => shopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
                assignOrderDetalList = assignOrderDetalList.Where(s => shopNoList.Contains(s.assignOrder.ShopNo.ToLower())).ToList();
            }
            if (ruanmo > 0)
            {
                if (ruanmo == 1)
                {
                    //只查询软膜的订单
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.GraphicMaterial != null && s.assignOrder.GraphicMaterial.Contains("软膜") && s.assignOrder.SubjectId > 0).ToList();

                }
                else if (ruanmo == 2)
                {
                    list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.GraphicMaterial == null || s.assignOrder.GraphicMaterial == "" || (s.assignOrder.GraphicMaterial != null && !s.assignOrder.GraphicMaterial.Contains("软膜"))).ToList();

                }
            }
            if (!string.IsNullOrWhiteSpace(materialAssign))
            {
                assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.SubjectId > 0).ToList();
                if (materialAssign == "背胶")
                {
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains(material0) && s.order.GraphicMaterial.Contains(material1) && !s.order.GraphicMaterial.Contains(material3)).ToList();
                    if (!string.IsNullOrWhiteSpace(materialPlan))
                    {
                        if (materialPlan == "雪弗板")
                            materialPlan = "3mmPVC";
                        assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.GraphicMaterial != null && s.assignOrder.GraphicMaterial.ToLower().Contains(materialPlan.ToLower()) && (s.assignOrder.OrderGraphicMaterial.ToLower().Contains(material0.ToLower()) && s.assignOrder.OrderGraphicMaterial.ToLower().Contains(material1.ToLower()) && !s.assignOrder.OrderGraphicMaterial.Contains(material3))).ToList();
                    }
                    else
                    {
                        assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.GraphicMaterial != null && (s.assignOrder.GraphicMaterial.ToLower().Contains(material0.ToLower()) && s.assignOrder.GraphicMaterial.ToLower().Contains(material1.ToLower()) && !s.assignOrder.OrderGraphicMaterial.Contains(material3))).ToList();
                    }
                }
                else if (materialAssign == "非背胶")
                {
                    list = list.Where(s => (s.order.GraphicMaterial != null && !(s.order.GraphicMaterial.Contains(material0) && s.order.GraphicMaterial.Contains(material1))) || s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || s.order.GraphicMaterial.Contains(material3)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => (s.assignOrder.OrderGraphicMaterial != null && !(s.assignOrder.OrderGraphicMaterial.ToLower().Contains(material0.ToLower()) && s.assignOrder.OrderGraphicMaterial.ToLower().Contains(material1.ToLower()))) || s.assignOrder.OrderGraphicMaterial == null || s.assignOrder.OrderGraphicMaterial == "" || s.assignOrder.OrderGraphicMaterial.Contains(material3)).ToList();

                }
                else
                {
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains(materialAssign)).ToList();
                    assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.GraphicMaterial != null && s.assignOrder.GraphicMaterial.Contains(materialAssign)).ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(otherMaterial))
            {
                list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.ToLower().Contains(otherMaterial.ToLower())).ToList();
                assignOrderDetalList = assignOrderDetalList.Where(s => s.assignOrder.GraphicMaterial != null && s.assignOrder.GraphicMaterial.ToLower().Contains(otherMaterial.ToLower())).ToList();
            }
            if (list.Any())
            {

                int orderCount = 0;
                int repeatCount = 0;
                int total = 0;
                int assignTotalOrderCount = 0;
                int notAssignShopCount = 0;
                int notAssignTotalOrderCount = 0;
                var shopList0 = list.Where(s => (s.order.IsProduce == null || s.order.IsProduce == true)).Select(s => s.shop).Distinct().OrderBy(s => s.ShopNo).ToList();

                if (shopList0.Any())
                {
                    List<int> allShopIdList = shopList0.Select(s => s.Id).Distinct().ToList();
                    StringBuilder json = new StringBuilder();
                    List<int> shopIdList = new List<int>();
                    if (!string.IsNullOrWhiteSpace(assignState))
                    {
                        allShopIdList.ForEach(s =>
                        {
                            int shopOrderCount = list.Where(o => o.order.ShopId == s).Distinct().Count();
                            var oneShopAssginOrderList = assignOrderDetalList.Where(a => a.assignOrder.ShopId == s).ToList();
                            int assignOrderCount = oneShopAssginOrderList.Count;
                            bool canShow = true;
                            if (assignState == "0")//未完成分配
                            {
                                canShow = shopOrderCount > assignOrderCount;

                            }
                            else//已完成分配
                            {
                                canShow = (shopOrderCount <= assignOrderCount);
                            }
                            if (canShow)
                                shopIdList.Add(s);
                        });
                        shopList0 = shopList0.Where(s => shopIdList.Contains(s.Id)).ToList();
                    }
                    if (shopList0.Any())
                    {
                        orderCount = list.Select(s => s.order).Count();//订单总数量
                        repeatCount = 0;
                        total = shopList0.Count;//店铺总数量
                        int assignShopCount = assignOrderDetalList.Select(s => s.assignOrder.ShopId ?? 0).Distinct().Count();//以分配店铺数量
                        assignTotalOrderCount = assignOrderDetalList.Where(s=>s.assignOrder.SubjectId>0).Select(s => s.assignOrder).Count();//已分配订单数量
                        notAssignShopCount = total - assignShopCount;//未分配店铺数量
                        notAssignTotalOrderCount = orderCount - assignTotalOrderCount;//未分配订单数量
                        notAssignTotalOrderCount = notAssignTotalOrderCount < 0 ? 0 : notAssignTotalOrderCount;
                        shopList0 = shopList0.OrderBy(s => s.ShopNo).Skip(currPage * pageSize).Take(pageSize).ToList();
                        List<string> nameList = new List<string>();
                        string orderTypeName = string.Empty;
                        shopList0.ForEach(s =>
                        {
                            int shopOrderCount = list.Where(o => o.order.ShopId == s.Id).Distinct().Count();
                            var oneShopAssginOrderList = assignOrderDetalList.Where(a => a.assignOrder.ShopId == s.Id).ToList();
                            int assignOrderCount = oneShopAssginOrderList.Count;
                            orderTypeName = string.Empty;
                            //StringBuilder outsourceName = new StringBuilder();
                            nameList.Clear();
                            oneShopAssginOrderList.ForEach(c =>
                            {
                                //orderTypeName=
                                orderTypeName = CommonMethod.GetEnumDescription<OutsourceOrderTypeEnum>((c.assignOrder.AssignType ?? 1).ToString());
                                orderTypeName = c.outsource.CompanyName + "(" + orderTypeName + ")";
                                if (!nameList.Contains(orderTypeName))
                                {
                                    nameList.Add(orderTypeName);
                                }
                            });
                            string outsourceName = StringHelper.ListToString(nameList, "/");


                            //string outsourceName = GetAssignOutsourceName(guidanceId, s.Id, out orderTypeName);

                            json.Append("{\"total\":\"" + total + "\",\"AssignShopCount\":\"" + assignShopCount + "\",\"NotAssignShopCount\":\"" + notAssignShopCount + "\",\"OutsourceName\":\"" + outsourceName.ToString().TrimEnd('/') + "\",\"ShopId\":\"" + s.Id + "\",\"ShopNo\":\"" + s.ShopNo + "\",\"ShopName\":\"" + s.ShopName + "\",\"Region\":\"" + s.RegionName + "\",\"Province\":\"" + s.ProvinceName + "\",\"City\":\"" + s.CityName + "\",\"CityTier\":\"" + s.CityTier + "\",\"Format\":\"" + s.Format + "\",\"Address\":\"" + s.POPAddress + "\",\"ShopOrderCount\":\"" + shopOrderCount + "\",\"AssignOrderCount\":\"" + assignOrderCount + "\",\"IsInstall\":\"" + s.IsInstall + "\"},");

                        });
                        result = "[" + json.ToString().TrimEnd(',') + "]";


                        int editProduce = 0;
                        if (isSearch == 1)
                        {
                            var editProduceList = list.Select(s => s.order).Where(s => s.IsProduce == false).ToList();
                            if (editProduceList.Any())
                                editProduce = 1;
                            //检查是否有重复订单（一个相同编号，相同尺寸有多条）
                            var orderList0 = list.Select(s => s.order).Distinct().Where(s => (s.IsProduce == null || s.IsProduce == true) && s.GraphicNo != null && s.GraphicNo != "" && s.GraphicLength != null && s.GraphicWidth != null).ToList();
                            var newList = from order in orderList0
                                          group order by new { order.ShopId, order.Sheet, order.GraphicNo, order.GraphicLength, order.GraphicWidth, order.PositionDescription }
                                              into g
                                              where g.Count() > 1
                                              select new
                                              {
                                                  g.Key.ShopId,
                                                  //g.Key.Sheet,
                                                  //g.Key.GraphicNo,
                                                  //g.Key.GraphicLength,
                                                  //g.Key.GraphicWidth
                                                  orderIdList = g.Select(s => s.Id).ToList()
                                              };
                            repeatCount = newList.Count();
                            if (newList.Any())
                            {
                                List<int> idList = new List<int>();
                                newList.ToList().ForEach(s =>
                                {
                                    idList.AddRange(s.orderIdList);
                                });
                                repeatCount = idList.Count - repeatCount;
                            }
                        }

                        result += "|" + orderCount + "|" + repeatCount + "|" + editProduce + "|" + assignTotalOrderCount + "|" + notAssignTotalOrderCount;

                    }


                }
            }
            return result;
        }


        List<string> ChangePOPCountSheetList = new List<string>();
        OutsourceAssignShopBLL assignShopBll = new OutsourceAssignShopBLL();
        List<ExpressPriceConfig> expressPriceConfigList = new List<ExpressPriceConfig>();
        List<ExpressPriceDetail> expressPriceDetailList = new List<ExpressPriceDetail>();
        InstallPriceTempBLL installPriceTempBll = new InstallPriceTempBLL();
        List<InstallPriceTemp> installPriceTempList = new List<InstallPriceTemp>();
        OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
        OutsourceOrderDetail outsourceOrderDetailModel;
        ExpressPriceDetailBLL expressPriceDetailBll = new ExpressPriceDetailBLL();
        //

        List<OutsourceOrderAssignConfig> configList = new List<OutsourceOrderAssignConfig>();
        List<Place> placeList = new List<Place>();

        List<POP> oohPOPList = new List<POP>();
        int customerId = 0;
        int guidanceType = 0;

        string Assign()
        {
            string result = "ok";
            int guidanceId = -1;
            //List<int> subjectList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            List<int> selectShopIdList = new List<int>();
            List<string> shopNoList = new List<string>();
            List<string> cityTierList = new List<string>();
            List<string> isInstallList = new List<string>();
            List<string> channelList = new List<string>();
            List<string> formatList = new List<string>();
            List<string> sheetList = new List<string>();
            //string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerServiceIds = string.Empty;
            string cityTier = string.Empty;
            string isInstall = string.Empty;
            string channel = string.Empty;
            string format = string.Empty;
            string sheet = string.Empty;
            string shopNo = string.Empty;
            string shopIds = string.Empty;
            string assignState = "";
            int companyId = 0;
            int installOutsourceId = 0;
            int orderType = 0;
            int ruanmo = 0;
            string materialAssign = string.Empty;
            string materialPlan = string.Empty;
            string otherMaterial = string.Empty;
            string noInstallPrice = string.Empty;
            if (context1.Request.Form["companyId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["companyId"]))
            {
                companyId = int.Parse(context1.Request.Form["companyId"]);
            }
            if (context1.Request.Form["installOutsourceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["installOutsourceId"]))
            {
                installOutsourceId = int.Parse(context1.Request.Form["installOutsourceId"]);
            }
            if (context1.Request.Form["orderType"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["orderType"]))
            {
                orderType = int.Parse(context1.Request.Form["orderType"]);
            }
            if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            {
                guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            }
            if (context1.Request.Form["subjectIds"] != null)
            {
                subjectIds = context1.Request.Form["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerServiceIds = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerServiceIds))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceIds, ',');
                }
            }
            if (context1.Request.Form["cityTier"] != null)
            {
                cityTier = context1.Request.Form["cityTier"];
                if (!string.IsNullOrWhiteSpace(cityTier))
                {
                    cityTierList = StringHelper.ToStringList(cityTier, ',');
                }
            }
            if (context1.Request.Form["isInstall"] != null)
            {
                isInstall = context1.Request.Form["isInstall"];
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    isInstallList = StringHelper.ToStringList(isInstall, ',');
                }
            }
            if (context1.Request.Form["channel"] != null)
            {
                channel = context1.Request.Form["channel"];
                if (!string.IsNullOrWhiteSpace(channel))
                {
                    channelList = StringHelper.ToStringList(channel, ',');
                }
            }
            if (context1.Request.Form["format"] != null)
            {
                format = context1.Request.Form["format"];
                if (!string.IsNullOrWhiteSpace(format))
                {
                    formatList = StringHelper.ToStringList(format, ',');
                }
            }
            if (context1.Request.Form["sheet"] != null)
            {
                sheet = context1.Request.Form["sheet"];
                if (!string.IsNullOrWhiteSpace(sheet))
                {
                    sheetList = StringHelper.ToStringList(sheet, ',');
                }
            }
            if (context1.Request.Form["shopIds"] != null)
            {
                shopIds = context1.Request.Form["shopIds"];
                if (!string.IsNullOrWhiteSpace(shopIds))
                {
                    selectShopIdList = StringHelper.ToIntList(shopIds, ',');
                }
            }
            if (context1.Request.Form["assignState"] != null)
            {
                assignState = context1.Request.Form["assignState"];
            }
            if (context1.Request.Form["shopNo"] != null)
            {
                shopNo = context1.Request.Form["shopNo"];
                if (!string.IsNullOrWhiteSpace(shopNo))
                {
                    shopNoList = StringHelper.ToStringList(shopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["ruanmo"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["ruanmo"]))
            {
                ruanmo = int.Parse(context1.Request.Form["ruanmo"]);
            }
            if (context1.Request.Form["materialAssign"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["materialAssign"]))
            {
                materialAssign = context1.Request.Form["materialAssign"];
            }
            if (context1.Request.Form["materialPlan"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["materialPlan"]))
            {
                materialPlan = context1.Request.Form["materialPlan"];
            }
            if (context1.Request.Form["otherMaterial"] != null)
            {
                otherMaterial = context1.Request.Form["otherMaterial"];
            }
            if (context1.Request.Form["noInstallPrice"] != null)
            {
                noInstallPrice = context1.Request.Form["noInstallPrice"];
            }
            List<string> myRegionList = new BasePage().GetResponsibleRegion;
            if (myRegionList.Any())
            {
                StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
            }
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on order.GuidanceId equals guidance.ItemId
                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                        on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                        from subjectCategory in categortTemp.DefaultIfEmpty()
                        where
                        order.GuidanceId == guidanceId
                        && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType > 1)

                        && (order.IsValid == null || order.IsValid == true)
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                        && (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower())) : 1 == 1)
                        && (order.IsProduce == null || order.IsProduce == true)
                        && (order.IsValidFromAssign == null || order.IsValidFromAssign == true)
                        && (order.OrderType != (int)OrderTypeEnum.物料)
                        select new
                        {
                            order,
                            shop,
                            subject,
                            guidance,
                            CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                        }).ToList();
            var totalOrderList = list.Where(s => s.subject.SubjectType != (int)SubjectTypeEnum.二次安装 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单 && s.subject.SubjectType != (int)SubjectTypeEnum.新开店安装费).ToList();
            //int roleId = new BasePage().CurrentUser.RoleId;
            //if (roleId == 5)
            //{
            //    int userId = new BasePage().CurrentUser.UserId;
            //    list = list.Where(s => s.order.CSUserId == userId).ToList();
            //}
            //已分订单
            var assignOrderList0 = outsourceOrderDetailBll.GetList(s => s.GuidanceId == guidanceId);
            var assignOrderList = assignOrderList0;

            if (subjectList.Any())
            {
                list = list.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
                assignOrderList = assignOrderList.Where(s => subjectList.Contains(s.SubjectId ?? 0)).ToList();
            }
            // var list0 = list;

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
            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        list = list.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                    }
                    else
                        list = list.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                }
                else
                {
                    list = list.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                }
            }
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        list = list.Where(s => cityTierList.Contains(s.order.CityTier) || (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                    }
                    else
                        list = list.Where(s => (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                }
                else
                {
                    list = list.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                }
            }
            if (isInstallList.Any())
            {
                if (isInstallList.Contains("空"))
                {
                    isInstallList.Remove("空");
                    if (isInstallList.Any())
                    {
                        list = list.Where(s => isInstallList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                    }
                    else
                        list = list.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                }
                else
                {
                    list = list.Where(s => isInstallList.Contains(s.order.IsInstall)).ToList();
                }
            }
            if (channelList.Any())
            {
                if (channelList.Contains("空"))
                {
                    channelList.Remove("空");
                    if (channelList.Any())
                    {
                        list = list.Where(s => channelList.Contains(s.order.Channel) || (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                    else
                    {
                        list = list.Where(s => (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => channelList.Contains(s.order.Channel)).ToList();

                }
            }
            if (formatList.Any())
            {
                if (formatList.Contains("空"))
                {
                    formatList.Remove("空");
                    if (formatList.Any())
                    {
                        list = list.Where(s => formatList.Contains(s.order.Format) || (s.order.Format == null || s.order.Format == "")).ToList();

                    }
                    else
                    {
                        list = list.Where(s => (s.order.Format == null || s.order.Format == "")).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => formatList.Contains(s.order.Format)).ToList();

                }
            }
            if (sheetList.Any())
            {
                if (sheetList.Contains("空"))
                {
                    sheetList.Remove("空");
                    if (sheetList.Any())
                    {
                        list = list.Where(s => sheetList.Contains(s.order.Sheet) || (s.order.Sheet == null || s.order.Sheet == "")).ToList();
                        assignOrderList = assignOrderList.Where(s => sheetList.Contains(s.Sheet) || (s.Sheet == null || s.Sheet == "")).ToList();
                    }
                    else
                    {
                        list = list.Where(s => (s.order.Sheet == null || s.order.Sheet == "")).ToList();
                        assignOrderList = assignOrderList.Where(s => (s.Sheet == null || s.Sheet == "")).ToList();
                    }
                }
                else
                {
                    list = list.Where(s => sheetList.Contains(s.order.Sheet)).ToList();
                    assignOrderList = assignOrderList.Where(s => sheetList.Contains(s.Sheet)).ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(assignState))
            {
                List<int> assignShopList = new OutsourceAssignShopBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).ToList();
                if (assignState == "0")
                {
                    //未分配
                    list = list.Where(s => !assignShopList.Contains(s.shop.Id)).ToList();
                }
                else
                {
                    //已分配
                    list = list.Where(s => assignShopList.Contains(s.shop.Id)).ToList();
                }
            }
            //bool canUpdateOutsource = true;
            if (ruanmo > 0)
            {
                if (ruanmo == 1)
                {
                    //只查询软膜的订单
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                    assignOrderList = assignOrderList.Where(s => s.GraphicMaterial != null && s.GraphicMaterial.Contains("软膜")).ToList();

                }
                else if (ruanmo == 2)
                {
                    list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
                    assignOrderList = assignOrderList.Where(s => s.GraphicMaterial == null || s.GraphicMaterial == "" || (s.GraphicMaterial != null && !s.GraphicMaterial.Contains("软膜"))).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(materialAssign))
            {
                if (materialAssign == "背胶")
                {
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains(material0) && s.order.GraphicMaterial.Contains(material1) && !s.order.GraphicMaterial.Contains(material3)).ToList();
                    assignOrderList = assignOrderList.Where(s => s.OrderGraphicMaterial != null && s.OrderGraphicMaterial.Contains(material0) && s.OrderGraphicMaterial.Contains(material1) && !s.OrderGraphicMaterial.Contains(material3)).ToList();
                    if (!string.IsNullOrWhiteSpace(materialPlan))
                    {
                        if (materialPlan == "雪弗板")
                        {
                            assignOrderList = assignOrderList.Where(s => s.GraphicMaterial != null && s.GraphicMaterial.ToLower() == "3mmpvc").ToList();
                        }
                        else
                            assignOrderList = assignOrderList.Where(s => s.GraphicMaterial != null && s.GraphicMaterial == materialPlan).ToList();
                    }
                }
                else if (materialAssign == "非背胶")
                {
                    list = list.Where(s => (s.order.GraphicMaterial != null && !(s.order.GraphicMaterial.Contains(material0) && s.order.GraphicMaterial.Contains(material1))) || s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || s.order.GraphicMaterial.Contains(material3)).ToList();
                    assignOrderList = assignOrderList.Where(s => (s.OrderGraphicMaterial != null && !(s.OrderGraphicMaterial.Contains(material0) && s.OrderGraphicMaterial.Contains(material1))) || s.OrderGraphicMaterial == null || s.OrderGraphicMaterial == "" || s.OrderGraphicMaterial.Contains(material3)).ToList();
                }
                else
                {
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.ToLower().Contains(materialAssign.ToLower())).ToList();
                    assignOrderList = assignOrderList.Where(s => s.GraphicMaterial != null && s.GraphicMaterial.Contains(materialAssign)).ToList();
                }
            }
            if (shopNoList.Any())
            {
                list = list.Where(s => shopNoList.Contains(s.order.ShopNo.ToLower())).ToList();

            }
            if (selectShopIdList.Any())
            {
                list = list.Where(s => selectShopIdList.Contains(s.shop.Id)).ToList();
            }
            var shopList = list.Select(s => s.shop).Distinct().OrderBy(s => s.ShopNo).ToList();

            if (shopList.Any() && guidanceId > 0)
            {
                customerId = list[0].subject.CustomerId ?? 0;
                List<int> shopIdList = shopList.Select(s => s.Id).ToList();
                oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet == "户外" || s.Sheet.ToLower() == "ooh") && (s.OOHInstallPrice ?? 0) > 0);
                expressPriceConfigList = new ExpressPriceConfigBLL().GetList(s => s.Id > 0);
                ExpressPriceDetailBLL expressPriceDetailBll = new ExpressPriceDetailBLL();

                List<string> BCSCityTierList = new List<string>() { "T1", "T2", "T3" };
                placeList = new PlaceBLL().GetList(s => s.ID > 0);
                configList = new OutsourceOrderAssignConfigBLL().GetList(s => s.Id > 0);
                if (list[0].guidance.ActivityTypeId == (int)GuidanceTypeEnum.Install && orderType == (int)OutsourceOrderTypeEnum.Install)
                {
                    installPriceTempList = installPriceTempBll.GetList(p => p.GuidanceId == guidanceId && shopIdList.Contains(p.ShopId ?? 0));
                }
                else if ((list[0].guidance.ActivityTypeId == (int)GuidanceTypeEnum.Delivery || list[0].guidance.ActivityTypeId == (int)GuidanceTypeEnum.Promotion) && (list[0].guidance.HasExperssFees ?? true))
                {
                    expressPriceDetailList = expressPriceDetailBll.GetList(p => p.GuidanceId == guidanceId && shopIdList.Contains(p.ShopId ?? 0));
                }
                string changePOPCountSheetStr = string.Empty;
                try
                {
                    changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                }
                catch
                {

                }
                if (!string.IsNullOrWhiteSpace(changePOPCountSheetStr))
                {
                    ChangePOPCountSheetList = StringHelper.ToStringList(changePOPCountSheetStr, '|');
                }


                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {

                        shopList.ForEach(s =>
                        {

                            decimal oohInstallPrice = 0;

                            OutsourceAssignOrderModel assignModel = new OutsourceAssignOrderModel();
                            assignModel.AssignType = orderType;
                            assignModel.GuidanceId = guidanceId;
                            assignModel.MaterialAssign = materialAssign;
                            assignModel.MaterialPlan = materialPlan;
                            var orderList0 = list.Where(l => l.order.ShopId == s.Id).Select(l => l.order).ToList();
                            assignModel.OrderList = orderList0;
                            assignModel.OutsourceId = companyId;
                            assignModel.Shop = s;
                            assignModel.InstallOutsourceId = installOutsourceId;
                            assignModel.BCSCityTierList = BCSCityTierList;
                           
                            var oneShopAssinList = assignOrderList.Where(a => a.ShopId == s.Id).ToList();
                            var oneShopAssinList0 = assignOrderList0.Where(a => a.ShopId == s.Id && a.SubjectId > 0).ToList();
                            List<int> assignOrderIdList = new List<int>();
                            if (oneShopAssinList.Any())
                            {
                                assignOrderIdList = oneShopAssinList.Select(a => a.Id).ToList();
                                outsourceOrderDetailBll.Delete(a => assignOrderIdList.Contains(a.Id) && a.SubjectId>0);
                                oneShopAssinList0 = oneShopAssinList0.Where(a => !assignOrderIdList.Contains(a.Id)).ToList();
                            }
                            assignModel.AssignedOrderList = oneShopAssinList0;
                            assignModel.AddInstallPrice = false;
                            //安装
                            if (list[0].guidance.ActivityTypeId == (int)GuidanceTypeEnum.Install && (list[0].guidance.HasInstallFees??true) && orderType == (int)OutsourceOrderTypeEnum.Install)
                            {
                                
                                if (noInstallPrice == "1")
                                {
                                    assignModel.AddInstallPrice = false;
                                }
                                else
                                {
                                    //删除安装费
                                    outsourceOrderDetailBll.Delete(a => (a.GuidanceId == guidanceId && a.ShopId == s.Id && a.SubjectId == 0));
                                    var oneShopTotalOrderList = totalOrderList.Where(a => a.order.ShopId == s.Id).ToList();

                                    bool isBCSSubject = true;
                                    bool isGeneric = true;
                                    oneShopTotalOrderList.ForEach(order =>
                                    {
                                        if (order.subject.CornerType != "三叶草")
                                            isBCSSubject = false;
                                        if (!order.CategoryName.Contains("常规-非活动"))
                                            isGeneric = false;
                                    });
                                    if (isBCSSubject)
                                    {
                                        assignModel.AddInstallPrice = s.BCSIsInstall == "Y";
                                    }
                                    else
                                    {
                                        assignModel.AddInstallPrice = s.IsInstall == "Y";
                                    }
                                    if (assignModel.AddInstallPrice)
                                    {

                                        //如果是安装店铺
                                        assignModel.IsBCSSubject = isBCSSubject;
                                        assignModel.IsGenericSubject = isGeneric;
                                        #region 高空安装费
                                        var oohList = oneShopTotalOrderList.Where(a => (a.order.Sheet != null && (a.order.Sheet.Contains("户外") || a.order.Sheet.ToLower() == "ooh"))).ToList();
                                        if (oohList.Any())
                                        {
                                            decimal price = 0;
                                            oohList.ForEach(a =>
                                            {

                                                if (!string.IsNullOrWhiteSpace(a.order.GraphicNo))
                                                {
                                                    price = oohPOPList.Where(p => p.ShopId == s.Id && p.GraphicNo.ToLower() == a.order.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                                }
                                                else
                                                    price = oohPOPList.Where(p => p.ShopId == s.Id && p.GraphicLength == a.order.GraphicLength && p.GraphicWidth == a.order.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                                if (oohInstallPrice < price)
                                                    oohInstallPrice = price;

                                            });

                                        }
                                        assignModel.OOHInstallPrice = oohInstallPrice;
                                        #endregion
                                    }
                                }
                            }
                            else if ((list[0].guidance.ActivityTypeId == (int)GuidanceTypeEnum.Delivery || list[0].guidance.ActivityTypeId == (int)GuidanceTypeEnum.Promotion) && (list[0].guidance.HasExperssFees ?? true))
                            {
                                //促销或者发货，
                                //var experssPrice = assignOrderList0.Where(a => a.ShopId == s.Id && a.OrderType == (int)OrderTypeEnum.发货费 && a.SubjectId == 0);
                                assignModel.AddExperssPrice = true;

                            }
                            SaveOutsourceOrder(assignModel);

                            #region
                            //assignShopModel = assignShopBll.GetList(l => l.GuidanceId == guidanceId && l.ShopId == s.Id && l.OutsourceId == companyId).FirstOrDefault();

                            //bool isAddInstallPrice = false;
                            //if (assignShopModel != null)
                            //{
                            //    //assignShopModel.OutsourceId = companyId;
                            //    assignShopModel.AssignType = orderType;

                            //    //如果是分配软膜，就不更新外协
                            //    if (orderType == (int)OutsourceOrderTypeEnum.Install && guidanceType == (int)GuidanceTypeEnum.Install && (s.IsInstall == "Y" || (s.BCSInstallPrice ?? 0) > 0))
                            //    {

                            //        decimal receiveInstallPrice = 0;
                            //        var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == guidanceId && sh.ShopId == s.Id);
                            //        if (installShopList.Any())
                            //        {
                            //            installShopList.ForEach(sh =>
                            //            {
                            //                receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                            //            });
                            //        }
                            //        assignShopModel.ReceiveInstallPrice = receiveInstallPrice;
                            //        if (receiveInstallPrice > 0)
                            //        {
                            //            isAddInstallPrice = true;
                            //            if ((s.OutsourceInstallPrice ?? 0) > 0)
                            //            {
                            //                basicInstallPrice = s.OutsourceInstallPrice ?? 0;
                            //            }
                            //            if (subjectCornerType == "三叶草")
                            //            {
                            //                if ((s.OutsourceBCSInstallPrice ?? 0) > 0)
                            //                {
                            //                    basicInstallPrice = s.OutsourceBCSInstallPrice ?? 0;
                            //                }
                            //                else if (BCSCityTierList.Contains(s.CityTier.ToUpper()))
                            //                {
                            //                    basicInstallPrice = 150;
                            //                }
                            //                else
                            //                {
                            //                    basicInstallPrice = 0;
                            //                }
                            //            }
                            //            if (subjectCategoryName.Contains("常规-非活动"))
                            //            {
                            //                if (s.CityName=="包头市" && (s.OutsourceInstallPrice ?? 0) > 0)
                            //                {
                            //                    basicInstallPrice = s.OutsourceInstallPrice ?? 0;
                            //                }
                            //                else if (BCSCityTierList.Contains(s.CityTier.ToUpper()))
                            //                {
                            //                    basicInstallPrice = 150;
                            //                }
                            //                else
                            //                {
                            //                    basicInstallPrice = 0;
                            //                }
                            //            }
                            //            assignShopModel.PayInstallPrice = oohInstallPrice + basicInstallPrice;
                            //        }
                            //        else
                            //            assignShopModel.PayInstallPrice = 0;
                            //    }
                            //    else
                            //    {
                            //        assignShopModel.ReceiveInstallPrice = 0;
                            //        assignShopModel.PayInstallPrice = 0;
                            //    }
                            //    if (guidanceType == (int)GuidanceTypeEnum.Promotion && hasExpressPrice)
                            //    {
                            //        assignShopModel.OutsourceId = companyId;
                            //        assignShopModel.AssignType = orderType;
                            //        //快递费
                            //        decimal rExpressPrice = 0;
                            //        if (expressPriceDetailModel != null && (expressPriceDetailModel.ExpressPrice ?? 0) > 0)
                            //        {
                            //            rExpressPrice = expressPriceDetailModel.ExpressPrice ?? 0;
                            //        }
                            //        else
                            //            rExpressPrice = 35;
                            //        assignShopModel.ReceiveExpresslPrice = rExpressPrice;
                            //        ExpressPriceConfig eM = expressPriceConfigList.Where(e => e.ReceivePrice == rExpressPrice).FirstOrDefault();
                            //        if (eM != null)
                            //            assignShopModel.PayExpressPrice = eM.PayPrice;
                            //        else
                            //            assignShopModel.PayExpressPrice = 22;
                            //    }
                            //    assignShopBll.Update(assignShopModel);
                            //}
                            //else
                            //{
                            //    assignShopModel = new OutsourceAssignShop();
                            //    assignShopModel.AddDate = DateTime.Now;
                            //    assignShopModel.AddUserId = new BasePage().CurrentUser.UserId;
                            //    assignShopModel.GuidanceId = guidanceId;
                            //    assignShopModel.OutsourceId = companyId;
                            //    assignShopModel.AssignType = orderType;
                            //    assignShopModel.ShopId = s.Id;

                            //    if (orderType == (int)OutsourceOrderTypeEnum.Install && guidanceType == (int)GuidanceTypeEnum.Install && (s.IsInstall == "Y" || (s.BCSInstallPrice ?? 0) > 0))
                            //    {
                            //        decimal receiveInstallPrice = 0;
                            //        var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == guidanceId && sh.ShopId == s.Id);
                            //        if (installShopList.Any())
                            //        {
                            //            installShopList.ForEach(sh =>
                            //            {
                            //                receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                            //            });
                            //        }
                            //        assignShopModel.ReceiveInstallPrice = receiveInstallPrice;
                            //        //if (receiveInstallPrice > 0)
                            //        //{

                            //            if ((s.OutsourceInstallPrice ?? 0) > 0)
                            //            {
                            //                basicInstallPrice = s.OutsourceInstallPrice ?? 0;
                            //            }
                            //            if (subjectCornerType == "三叶草")
                            //            {
                            //                if ((s.OutsourceBCSInstallPrice ?? 0) > 0)
                            //                {
                            //                    basicInstallPrice = s.OutsourceBCSInstallPrice ?? 0;
                            //                }
                            //                else if (BCSCityTierList.Contains(s.CityTier.ToUpper()))
                            //                {
                            //                    basicInstallPrice = 150;
                            //                }
                            //                else
                            //                {
                            //                    basicInstallPrice = 0;
                            //                }

                            //            }
                            //            if (subjectCategoryName.Contains("常规-非活动"))
                            //            {
                            //                if (s.CityName == "包头市" && (s.OutsourceInstallPrice ?? 0) > 0)
                            //                {
                            //                    basicInstallPrice = s.OutsourceInstallPrice ?? 0;
                            //                }
                            //                else if (BCSCityTierList.Contains(s.CityTier.ToUpper()))
                            //                {
                            //                    basicInstallPrice = 150;
                            //                }
                            //                else
                            //                {
                            //                    basicInstallPrice = 0;
                            //                }
                            //            }
                            //            assignShopModel.PayInstallPrice = oohInstallPrice + basicInstallPrice;
                            //            isAddInstallPrice = basicInstallPrice > 0;
                            //        //}
                            //        //else
                            //        //    assignShopModel.PayInstallPrice = 0;

                            //    }

                            //    if (guidanceType == (int)GuidanceTypeEnum.Promotion && hasExpressPrice)
                            //    {
                            //        //快递费
                            //        decimal rExpressPrice = 0;
                            //        if (expressPriceDetailModel != null && (expressPriceDetailModel.ExpressPrice ?? 0) > 0)
                            //        {
                            //            rExpressPrice = expressPriceDetailModel.ExpressPrice ?? 0;
                            //        }
                            //        else
                            //            rExpressPrice = 35;
                            //        assignShopModel.ReceiveExpresslPrice = rExpressPrice;

                            //        ExpressPriceConfig eM = expressPriceConfigList.Where(e => e.ReceivePrice == rExpressPrice).FirstOrDefault();
                            //        if (eM != null)
                            //            assignShopModel.PayExpressPrice = eM.PayPrice;
                            //        else
                            //            assignShopModel.PayExpressPrice = 22;
                            //    }
                            //    assignShopBll.Add(assignShopModel);


                            //}
                            //if (isAddInstallPrice)
                            //{
                            //    outsourceOrderDetailBll.Delete(o => o.ShopId == s.Id && o.GuidanceId == guidanceId && o.OrderType == (int)OrderTypeEnum.安装费);
                            //}
                            #endregion
                        });

                        tran.Complete();
                        StringBuilder assginState = new StringBuilder();
                        assginState.Append(totalOrderCount);
                        assginState.Append(",");
                        assginState.Append(assignOrderCount);
                        assginState.Append(",");
                        assginState.Append(repeatOrderCount);
                        result = "ok|" + assginState.ToString();
                    }
                    catch (Exception ex)
                    {
                        result = "error|" + ex.Message;
                    }
                }

            }
            return result;
        }


        string GetAssignOutsourceName(int guidanceId, int shopId, out string orderTypeName)
        {
            orderTypeName = string.Empty;
            var model = (from assign in CurrentContext.DbContext.OutsourceAssignShop
                         join outsource in CurrentContext.DbContext.Company
                         on assign.OutsourceId equals outsource.Id
                         where assign.GuidanceId == guidanceId && assign.ShopId == shopId
                         select new
                         {
                             assign.AssignType,
                             outsource.CompanyName
                         }).FirstOrDefault();
            if (model != null)
            {
                orderTypeName = CommonMethod.GetEnumDescription<OutsourceOrderTypeEnum>((model.AssignType ?? 1).ToString());
                return model.CompanyName;
            }
            else
                return "";
        }

        POPBLL popBll = new POPBLL();
        // List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
        // List<Shop> shopList = new List<Shop>();


        int totalOrderCount = 0;
        int assignOrderCount = 0;
        int repeatOrderCount = 0;

        //, out decimal basicInstallPrice, out decimal oohInstallPrice



        void SaveOutsourceOrder(OutsourceAssignOrderModel assignOrderModel)
        {

            if (assignOrderModel != null && assignOrderModel.Shop != null)
            {
                decimal promotionInstallPrice = 0;
                decimal basicInstallPrice = 0;
                decimal oohInstallPrice = 0;
                string materialSupport = string.Empty;
                List<string> materialSupportList = new List<string>();
                string posScale = string.Empty;
                bool addInstallPrice = false;
                bool hasInstallPrice = false;
                int outsourceId = assignOrderModel.OutsourceId;
                int guidanceId = assignOrderModel.GuidanceId;
                int shopId = assignOrderModel.Shop.Id;
                string materialAssign = assignOrderModel.MaterialAssign;
                string materialPlan = assignOrderModel.MaterialPlan;
                int outsourceType = assignOrderModel.AssignType;
                var OutsourceOrderList = assignOrderModel.OrderList;
                var AssignedOrderList = assignOrderModel.AssignedOrderList;
                int subjectId = assignOrderModel.OrderList.Any() ? assignOrderModel.OrderList[0].SubjectId ?? 0 : 0;
                oohInstallPrice = assignOrderModel.OOHInstallPrice;
                totalOrderCount = OutsourceOrderList.Count();
                if (OutsourceOrderList.Any())
                {

                    List<Order350Model> savedList = new List<Order350Model>();
                    //已分配的订单，取出来，用来判断是否有重复订单
                    var assignedList = outsourceOrderDetailBll.GetList(s => s.GuidanceId == guidanceId && s.ShopId == shopId);

                    var priceOrderList = OutsourceOrderList.Where(s => s.OrderType != (int)OrderTypeEnum.POP && s.OrderType != (int)OrderTypeEnum.道具).ToList();
                    var popOrderList = OutsourceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具).ToList();

                    #region 费用订单
                    if (priceOrderList.Any())
                    {
                        priceOrderList.ForEach(s =>
                        {
                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                            outsourceOrderDetailModel.AssignType = outsourceType;
                            if (s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费)
                            {
                                if (s.OrderType == (int)OrderTypeEnum.安装费)
                                {
                                    Subject subjectModel = new SubjectBLL().GetModel(s.SubjectId ?? 0);
                                    if (subjectModel != null && subjectModel.SubjectType != (int)SubjectTypeEnum.费用订单)
                                    {
                                        hasInstallPrice = true;
                                    }
                                }
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                if (outsourceType == (int)OutsourceOrderTypeEnum.Send && assignOrderModel.InstallOutsourceId > 0)
                                    outsourceOrderDetailModel.OutsourceId = assignOrderModel.InstallOutsourceId;
                            }
                            int Quantity = s.Quantity ?? 1;
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = s.AgentCode;
                            outsourceOrderDetailModel.AgentName = s.AgentName;
                            outsourceOrderDetailModel.Area = s.Area;
                            outsourceOrderDetailModel.BusinessModel = s.BusinessModel;
                            outsourceOrderDetailModel.Channel = s.Channel;
                            outsourceOrderDetailModel.ChooseImg = s.ChooseImg;
                            outsourceOrderDetailModel.City = s.City;
                            outsourceOrderDetailModel.CityTier = s.CityTier;
                            outsourceOrderDetailModel.Contact = s.Contact;
                            outsourceOrderDetailModel.CornerType = s.CornerType;
                            outsourceOrderDetailModel.Format = s.Format;
                            outsourceOrderDetailModel.Gender = string.Empty;
                            outsourceOrderDetailModel.GraphicLength = s.GraphicLength;
                            outsourceOrderDetailModel.OrderGraphicMaterial = s.GraphicMaterial;
                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                            outsourceOrderDetailModel.GraphicNo = s.GraphicNo;
                            outsourceOrderDetailModel.GraphicWidth = s.GraphicWidth;
                            outsourceOrderDetailModel.GuidanceId = s.GuidanceId;
                            outsourceOrderDetailModel.IsInstall = s.IsInstall;
                            outsourceOrderDetailModel.LocationType = s.LocationType;
                            outsourceOrderDetailModel.MachineFrame = s.MachineFrame;
                            outsourceOrderDetailModel.MaterialSupport = s.MaterialSupport;
                            outsourceOrderDetailModel.OrderGender = s.OrderGender;
                            outsourceOrderDetailModel.OrderType = s.OrderType;
                            outsourceOrderDetailModel.POPAddress = s.POPAddress;
                            outsourceOrderDetailModel.POPName = s.POPName;
                            outsourceOrderDetailModel.POPType = s.POPType;
                            outsourceOrderDetailModel.PositionDescription = s.PositionDescription;
                            outsourceOrderDetailModel.POSScale = s.POSScale;
                            outsourceOrderDetailModel.PriceBlongRegion = s.PriceBlongRegion;
                            outsourceOrderDetailModel.Province = s.Province;
                            outsourceOrderDetailModel.Quantity = Quantity;
                            outsourceOrderDetailModel.Region = s.Region;
                            outsourceOrderDetailModel.Remark = s.Remark;
                            outsourceOrderDetailModel.Sheet = s.Sheet;
                            outsourceOrderDetailModel.ShopId = s.ShopId;
                            outsourceOrderDetailModel.ShopName = s.ShopName;
                            outsourceOrderDetailModel.ShopNo = s.ShopNo;
                            outsourceOrderDetailModel.ShopStatus = s.ShopStatus;
                            outsourceOrderDetailModel.SubjectId = s.SubjectId;
                            outsourceOrderDetailModel.Tel = s.Tel;
                            outsourceOrderDetailModel.TotalArea = s.TotalArea;
                            outsourceOrderDetailModel.WindowDeep = s.WindowDeep;
                            outsourceOrderDetailModel.WindowHigh = s.WindowHigh;
                            outsourceOrderDetailModel.WindowSize = s.WindowSize;
                            outsourceOrderDetailModel.WindowWide = s.WindowWide;
                            outsourceOrderDetailModel.ReceiveOrderPrice = s.OrderPrice;
                            outsourceOrderDetailModel.PayOrderPrice = s.PayOrderPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.InstallPriceMaterialSupport;
                            outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                            outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                            outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                            outsourceOrderDetailModel.CSUserId = s.CSUserId;
                            outsourceOrderDetailModel.FinalOrderId = s.Id;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                            assignOrderCount++;
                        });
                    }
                    #endregion

                    #region POP订单
                    if (popOrderList.Any())
                    {
                        List<FinalOrderDetailTemp> tempOrderList = new List<FinalOrderDetailTemp>();
                        //去重
                        popOrderList.ForEach(s =>
                        {
                            bool canGo = true;

                            if (!string.IsNullOrWhiteSpace(s.GraphicNo) && !string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                            {
                                //string gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                //去掉重复的（同一个编号下多次的）
                                var checkList = tempOrderList.Where(sl => sl.Sheet == s.Sheet && sl.PositionDescription == s.PositionDescription && sl.GraphicNo == s.GraphicNo && sl.GraphicLength == s.GraphicLength && sl.GraphicWidth == s.GraphicWidth && (sl.Gender == s.Gender || sl.OrderGender == s.OrderGender)).ToList();
                                var checkAssignedList = AssignedOrderList.Where(sl => sl.Sheet == s.Sheet && sl.PositionDescription == s.PositionDescription && sl.GraphicNo == s.GraphicNo && sl.GraphicLength == s.GraphicLength && sl.GraphicWidth == s.GraphicWidth && (sl.Gender == s.Gender || sl.Gender == s.OrderGender)).ToList();
                                if (checkList.Any() || checkAssignedList.Any())
                                {
                                    canGo = false;
                                    repeatOrderCount++;
                                }

                            }
                            if (canGo)
                            {
                                tempOrderList.Add(s);
                                if (!string.IsNullOrWhiteSpace(s.InstallPriceMaterialSupport) && !materialSupportList.Contains(s.InstallPriceMaterialSupport.ToLower()))
                                {
                                    materialSupportList.Add(s.InstallPriceMaterialSupport.ToLower());
                                }
                                if (string.IsNullOrWhiteSpace(posScale))
                                    posScale = s.InstallPricePOSScale;

                            }


                        });
                        popOrderList = tempOrderList;
                        if (popOrderList.Any())
                        {

                            addInstallPrice = true;
                            popOrderList.ForEach(s =>
                            {

                                double GraphicLength = s.GraphicLength != null ? double.Parse(s.GraphicLength.ToString()) : 0;
                                double GraphicWidth = s.GraphicWidth != null ? double.Parse(s.GraphicWidth.ToString()) : 0;
                                string material0 = s.GraphicMaterial;
                                if (!string.IsNullOrWhiteSpace(materialPlan))
                                {
                                    if (materialPlan == "雪弗板")
                                        material0 = "3mmPVC";
                                    else
                                        material0 = materialPlan;
                                }
                                Order350Model model = new Order350Model();
                                model.SubjectId = s.SubjectId ?? 0;
                                model.ShopId = s.ShopId ?? 0;
                                model.Sheet = s.Sheet;
                                model.Gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                model.PositionDescription = s.PositionDescription;
                                model.GraphicNo = s.GraphicNo;
                                model.GraphicLength = GraphicLength;
                                model.GraphicWidth = GraphicWidth;
                                model.GraphicMaterial = material0;
                                savedList.Add(model);
                                int Quantity = s.Quantity ?? 1;
                                if (!string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                {
                                    Quantity = Quantity > 0 ? 1 : 0;
                                }

                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AssignType = outsourceType;
                                if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.Sheet != null && (s.Sheet.Contains("橱窗") || s.Sheet.Contains("窗贴")) && GraphicLength > 1 && GraphicWidth > 1 && material0.Contains("全透贴"))
                                {
                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                    promotionInstallPrice = 150;
                                    assignOrderModel.AddInstallPrice = true;
                                    assignOrderModel.AddExperssPrice = false;
                                }
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = s.AgentCode;
                                outsourceOrderDetailModel.AgentName = s.AgentName;
                                outsourceOrderDetailModel.Area = s.Area;
                                outsourceOrderDetailModel.BusinessModel = s.BusinessModel;
                                outsourceOrderDetailModel.Channel = s.Channel;
                                outsourceOrderDetailModel.ChooseImg = s.ChooseImg;
                                outsourceOrderDetailModel.City = s.City;
                                outsourceOrderDetailModel.CityTier = s.CityTier;
                                outsourceOrderDetailModel.Contact = s.Contact;
                                outsourceOrderDetailModel.CornerType = s.CornerType;
                                outsourceOrderDetailModel.Format = s.Format;
                                outsourceOrderDetailModel.Gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                outsourceOrderDetailModel.GraphicLength = s.GraphicLength;
                                outsourceOrderDetailModel.OrderGraphicMaterial = s.GraphicMaterial;
                                string material = string.Empty;
                                if (!string.IsNullOrWhiteSpace(material0))
                                    material = new BasePage().GetBasicMaterial(material0);
                                if (string.IsNullOrWhiteSpace(material))
                                    material = s.GraphicMaterial;
                                outsourceOrderDetailModel.GraphicMaterial = material;
                                outsourceOrderDetailModel.GraphicNo = s.GraphicNo;
                                outsourceOrderDetailModel.GraphicWidth = s.GraphicWidth;
                                outsourceOrderDetailModel.GuidanceId = s.GuidanceId;
                                outsourceOrderDetailModel.IsInstall = s.IsInstall;
                                outsourceOrderDetailModel.LocationType = s.LocationType;
                                outsourceOrderDetailModel.MachineFrame = s.MachineFrame;
                                outsourceOrderDetailModel.MaterialSupport = s.MaterialSupport;
                                outsourceOrderDetailModel.OrderGender = s.OrderGender;
                                outsourceOrderDetailModel.OrderType = s.OrderType;
                                outsourceOrderDetailModel.OutsourceId = outsourceId;
                                outsourceOrderDetailModel.POPAddress = s.POPAddress;
                                outsourceOrderDetailModel.POPName = s.POPName;
                                outsourceOrderDetailModel.POPType = s.POPType;
                                outsourceOrderDetailModel.PositionDescription = s.PositionDescription;
                                outsourceOrderDetailModel.POSScale = s.POSScale;
                                outsourceOrderDetailModel.PriceBlongRegion = s.PriceBlongRegion;
                                outsourceOrderDetailModel.Province = s.Province;
                                outsourceOrderDetailModel.Quantity = Quantity;
                                outsourceOrderDetailModel.Region = s.Region;
                                outsourceOrderDetailModel.Remark = s.Remark;
                                outsourceOrderDetailModel.Sheet = s.Sheet;
                                outsourceOrderDetailModel.ShopId = s.ShopId;
                                outsourceOrderDetailModel.ShopName = s.ShopName;
                                outsourceOrderDetailModel.ShopNo = s.ShopNo;
                                outsourceOrderDetailModel.ShopStatus = s.ShopStatus;
                                outsourceOrderDetailModel.SubjectId = s.SubjectId;
                                outsourceOrderDetailModel.Tel = s.Tel;
                                outsourceOrderDetailModel.TotalArea = s.TotalArea;
                                outsourceOrderDetailModel.WindowDeep = s.WindowDeep;
                                outsourceOrderDetailModel.WindowHigh = s.WindowHigh;
                                outsourceOrderDetailModel.WindowSize = s.WindowSize;
                                outsourceOrderDetailModel.WindowWide = s.WindowWide;
                                outsourceOrderDetailModel.ReceiveOrderPrice = s.OrderPrice;
                                outsourceOrderDetailModel.PayOrderPrice = s.PayOrderPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = s.InstallPriceMaterialSupport;
                                decimal unitPrice = 0;
                                decimal totalPrice = 0;
                                if (!string.IsNullOrWhiteSpace(material))
                                {
                                    POP pop = new POP();
                                    pop.GraphicMaterial = material;
                                    pop.GraphicLength = s.GraphicLength;
                                    pop.GraphicWidth = s.GraphicWidth;
                                    pop.Quantity = Quantity;
                                    pop.CustomerId = customerId;
                                    pop.OutsourceType = outsourceType;
                                    new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                    outsourceOrderDetailModel.UnitPrice = unitPrice;
                                    outsourceOrderDetailModel.TotalPrice = totalPrice;
                                }
                                outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                                outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                                if (material0 == "3mmPVC")
                                {
                                    outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                    outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                }
                                outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                                outsourceOrderDetailModel.CSUserId = s.CSUserId;
                                outsourceOrderDetailModel.FinalOrderId = s.Id;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                assignOrderCount++;
                            });
                        }

                    }
                    #endregion

                    #region 安装费和快递费
                    if (popOrderList.Any())
                    {
                        if (addInstallPrice && !hasInstallPrice && assignOrderModel.AddInstallPrice)
                        {
                            decimal installPrice = 0;
                            decimal receiveInstallPrice = 0;
                            string remark = "活动安装费";
                            if (outsourceType == (int)GuidanceTypeEnum.Install)
                            {
                                //按照级别，获取基础安装费

                                var installShopList = installPriceTempList.Where(sh => sh.ShopId == shopId).ToList();
                                if (installShopList.Any())
                                {
                                    installShopList.ForEach(sh =>
                                    {
                                        receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                                    });
                                }
                                if (assignOrderModel.IsBCSSubject)
                                {
                                    if ((assignOrderModel.Shop.OutsourceBCSInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = assignOrderModel.Shop.OutsourceBCSInstallPrice ?? 0;
                                    }
                                    else if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                                    {
                                        basicInstallPrice = 150;
                                    }
                                    else
                                    {
                                        basicInstallPrice = 0;
                                    }

                                }
                                else if (assignOrderModel.IsGenericSubject)
                                {

                                    if (assignOrderModel.Shop.CityName == "包头市" && (assignOrderModel.Shop.OutsourceInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = assignOrderModel.Shop.OutsourceInstallPrice ?? 0;
                                    }
                                    else if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                                    {
                                        basicInstallPrice = 150;
                                    }
                                    else
                                    {
                                        basicInstallPrice = 0;
                                    }
                                }
                                else
                                {
                                    if ((assignOrderModel.Shop.OutsourceInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = assignOrderModel.Shop.OutsourceInstallPrice ?? 0;
                                    }
                                    else
                                    {
                                        materialSupportList.ForEach(ma =>
                                        {
                                            decimal basicInstallPrice0 = new BasePage().GetOutsourceBasicInstallPrice(ma);
                                            if (basicInstallPrice0 > basicInstallPrice)
                                            {
                                                basicInstallPrice = basicInstallPrice0;
                                                materialSupport = ma;
                                            }
                                        });
                                    }
                                }
                                installPrice = oohInstallPrice + basicInstallPrice;
                            }
                            else if (promotionInstallPrice > 0)
                            {
                                installPrice = promotionInstallPrice;
                                receiveInstallPrice = promotionInstallPrice;
                                remark = "促销窗贴安装费";
                            }

                            #region 添加安装费
                            if (installPrice > 0)
                            {

                                if (oohInstallPrice > 0 && (assignOrderModel.Shop.OOHInstallOutsourceId ?? 0) > 0)
                                {
                                    //如果有单独的户外安装外协
                                    outsourceOrderDetailModel = new OutsourceOrderDetail();
                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                    outsourceOrderDetailModel.AddDate = DateTime.Now;
                                    outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    outsourceOrderDetailModel.AgentCode = popOrderList[0].AgentCode;
                                    outsourceOrderDetailModel.AgentName = popOrderList[0].AgentName;
                                    outsourceOrderDetailModel.BusinessModel = popOrderList[0].BusinessModel;
                                    outsourceOrderDetailModel.Channel = popOrderList[0].Channel;
                                    outsourceOrderDetailModel.City = popOrderList[0].City;
                                    outsourceOrderDetailModel.CityTier = popOrderList[0].CityTier;
                                    outsourceOrderDetailModel.Contact = popOrderList[0].Contact;
                                    outsourceOrderDetailModel.Format = popOrderList[0].Format;
                                    outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                    outsourceOrderDetailModel.GraphicNo = string.Empty;
                                    outsourceOrderDetailModel.GraphicWidth = 0;
                                    outsourceOrderDetailModel.GuidanceId = guidanceId;
                                    outsourceOrderDetailModel.IsInstall = popOrderList[0].IsInstall;
                                    outsourceOrderDetailModel.BCSIsInstall = popOrderList[0].BCSIsInstall;
                                    outsourceOrderDetailModel.LocationType = popOrderList[0].LocationType;
                                    outsourceOrderDetailModel.MachineFrame = string.Empty;
                                    outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                    outsourceOrderDetailModel.OrderGender = string.Empty;
                                    outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                    outsourceOrderDetailModel.POPAddress = popOrderList[0].POPAddress;
                                    outsourceOrderDetailModel.POPName = string.Empty;
                                    outsourceOrderDetailModel.POPType = string.Empty;
                                    outsourceOrderDetailModel.PositionDescription = string.Empty;
                                    outsourceOrderDetailModel.POSScale = posScale;
                                    outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                                    outsourceOrderDetailModel.Quantity = 1;
                                    outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                                    outsourceOrderDetailModel.Remark = "户外安装费";
                                    outsourceOrderDetailModel.Sheet = string.Empty;
                                    outsourceOrderDetailModel.ShopId = shopId;
                                    outsourceOrderDetailModel.ShopName = popOrderList[0].ShopName;
                                    outsourceOrderDetailModel.ShopNo = popOrderList[0].ShopNo;
                                    outsourceOrderDetailModel.ShopStatus = popOrderList[0].ShopStatus;
                                    outsourceOrderDetailModel.SubjectId = 0;
                                    outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                                    outsourceOrderDetailModel.TotalArea = 0;
                                    outsourceOrderDetailModel.WindowDeep = 0;
                                    outsourceOrderDetailModel.WindowHigh = 0;
                                    outsourceOrderDetailModel.WindowSize = string.Empty;
                                    outsourceOrderDetailModel.WindowWide = 0;
                                    outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                    outsourceOrderDetailModel.PayOrderPrice = oohInstallPrice;
                                    outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                    outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                    outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                    outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                    outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                    outsourceOrderDetailModel.CSUserId = popOrderList[0].CSUserId;
                                    outsourceOrderDetailModel.OutsourceId = assignOrderModel.Shop.OOHInstallOutsourceId;
                                    outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                    installPrice = installPrice - oohInstallPrice;
                                    oohInstallPrice = 0;
                                }
                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = popOrderList[0].AgentCode;
                                outsourceOrderDetailModel.AgentName = popOrderList[0].AgentName;
                                outsourceOrderDetailModel.BusinessModel = popOrderList[0].BusinessModel;
                                outsourceOrderDetailModel.Channel = popOrderList[0].Channel;
                                outsourceOrderDetailModel.City = popOrderList[0].City;
                                outsourceOrderDetailModel.CityTier = popOrderList[0].CityTier;
                                outsourceOrderDetailModel.Contact = popOrderList[0].Contact;
                                outsourceOrderDetailModel.Format = popOrderList[0].Format;
                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                outsourceOrderDetailModel.GraphicWidth = 0;
                                outsourceOrderDetailModel.GuidanceId = guidanceId;
                                outsourceOrderDetailModel.IsInstall = popOrderList[0].IsInstall;
                                outsourceOrderDetailModel.BCSIsInstall = popOrderList[0].BCSIsInstall;
                                outsourceOrderDetailModel.LocationType = popOrderList[0].LocationType;
                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                outsourceOrderDetailModel.POPAddress = popOrderList[0].POPAddress;
                                outsourceOrderDetailModel.POPName = string.Empty;
                                outsourceOrderDetailModel.POPType = string.Empty;
                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                outsourceOrderDetailModel.POSScale = posScale;
                                outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                                outsourceOrderDetailModel.Quantity = 1;
                                outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                                outsourceOrderDetailModel.Remark = remark;
                                outsourceOrderDetailModel.Sheet = string.Empty;
                                outsourceOrderDetailModel.ShopId = shopId;
                                outsourceOrderDetailModel.ShopName = popOrderList[0].ShopName;
                                outsourceOrderDetailModel.ShopNo = popOrderList[0].ShopNo;
                                outsourceOrderDetailModel.ShopStatus = popOrderList[0].ShopStatus;
                                outsourceOrderDetailModel.SubjectId = 0;
                                outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                                outsourceOrderDetailModel.TotalArea = 0;
                                outsourceOrderDetailModel.WindowDeep = 0;
                                outsourceOrderDetailModel.WindowHigh = 0;
                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                outsourceOrderDetailModel.WindowWide = 0;
                                outsourceOrderDetailModel.ReceiveOrderPrice = receiveInstallPrice;
                                outsourceOrderDetailModel.PayOrderPrice = installPrice;
                                outsourceOrderDetailModel.PayBasicInstallPrice = basicInstallPrice;
                                outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                outsourceOrderDetailModel.CSUserId = popOrderList[0].CSUserId;
                                outsourceOrderDetailModel.OutsourceId = assignOrderModel.InstallOutsourceId > 0 ? assignOrderModel.InstallOutsourceId : outsourceId;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                            }
                            #endregion
                        }
                        else if (assignOrderModel.AddExperssPrice)
                        {
                            ExpressPriceDetail expressPriceDetailModel = expressPriceDetailList.Where(s => s.ShopId == shopId).FirstOrDefault();
                            //快递费
                            decimal rExpressPrice = 0;
                            decimal payExpressPrice = 0;
                            if (expressPriceDetailModel != null && (expressPriceDetailModel.ExpressPrice ?? 0) > 0)
                            {
                                rExpressPrice = expressPriceDetailModel.ExpressPrice ?? 0;
                            }
                            else
                                rExpressPrice = 35;

                            ExpressPriceConfig eM = expressPriceConfigList.Where(price => price.ReceivePrice == rExpressPrice).FirstOrDefault();
                            if (eM != null)
                                payExpressPrice = eM.PayPrice ?? 0;
                            else
                                payExpressPrice = 22;
                            if (assignOrderModel.Shop.ProvinceName == "内蒙古" && !assignOrderModel.Shop.CityName.Contains("通辽"))
                            {
                                payExpressPrice = 0;
                            }

                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = popOrderList[0].AgentCode;
                            outsourceOrderDetailModel.AgentName = popOrderList[0].AgentName;
                            outsourceOrderDetailModel.BusinessModel = popOrderList[0].BusinessModel;
                            outsourceOrderDetailModel.Channel = popOrderList[0].Channel;
                            outsourceOrderDetailModel.City = popOrderList[0].City;
                            outsourceOrderDetailModel.CityTier = popOrderList[0].CityTier;
                            outsourceOrderDetailModel.Contact = assignOrderModel.Shop.Contact1;
                            outsourceOrderDetailModel.Format = popOrderList[0].Format;
                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                            outsourceOrderDetailModel.GraphicWidth = 0;
                            outsourceOrderDetailModel.GuidanceId = guidanceId;
                            outsourceOrderDetailModel.IsInstall = popOrderList[0].IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = popOrderList[0].BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = popOrderList[0].LocationType;
                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                            outsourceOrderDetailModel.MaterialSupport = string.Empty;
                            outsourceOrderDetailModel.OrderGender = string.Empty;
                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.发货费;
                            outsourceOrderDetailModel.POPAddress = assignOrderModel.Shop.POPAddress;
                            outsourceOrderDetailModel.POPName = string.Empty;
                            outsourceOrderDetailModel.POPType = string.Empty;
                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                            outsourceOrderDetailModel.POSScale = posScale;
                            outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                            outsourceOrderDetailModel.Quantity = 1;
                            outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                            outsourceOrderDetailModel.Remark = string.Empty;
                            outsourceOrderDetailModel.Sheet = string.Empty;
                            outsourceOrderDetailModel.ShopId = shopId;
                            outsourceOrderDetailModel.ShopName = popOrderList[0].ShopName;
                            outsourceOrderDetailModel.ShopNo = popOrderList[0].ShopNo;
                            outsourceOrderDetailModel.ShopStatus = popOrderList[0].ShopStatus;
                            outsourceOrderDetailModel.SubjectId = 0;
                            outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                            outsourceOrderDetailModel.TotalArea = 0;
                            outsourceOrderDetailModel.WindowDeep = 0;
                            outsourceOrderDetailModel.WindowHigh = 0;
                            outsourceOrderDetailModel.WindowSize = string.Empty;
                            outsourceOrderDetailModel.WindowWide = 0;
                            outsourceOrderDetailModel.ReceiveOrderPrice = rExpressPrice;
                            outsourceOrderDetailModel.PayOrderPrice = payExpressPrice;
                            outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                            outsourceOrderDetailModel.PayOOHInstallPrice = 0;
                            outsourceOrderDetailModel.PayExpressPrice = payExpressPrice;
                            outsourceOrderDetailModel.ReceiveExpresslPrice = rExpressPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = string.Empty;
                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                            outsourceOrderDetailModel.CSUserId = popOrderList[0].CSUserId;
                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                        }
                    }
                    #endregion
                }
            }

        }


        string CancelAssign()
        {
            string result = "ok";
            int guidanceId = -1;
            List<int> subjectList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            List<int> selectShopIdList = new List<int>();
            List<string> shopNoList = new List<string>();
            List<string> cityTierList = new List<string>();
            List<string> isInstallList = new List<string>();
            List<string> channelList = new List<string>();
            List<string> formatList = new List<string>();
            //string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            string regions = string.Empty;
            string provinces = string.Empty;
            string citys = string.Empty;
            string customerServiceIds = string.Empty;
            string cityTier = string.Empty;
            string isInstall = string.Empty;
            string channel = string.Empty;
            string format = string.Empty;
            string shopNo = string.Empty;
            string shopIds = string.Empty;
            string assignState = "";
            int companyId = 0;
            int orderType = 0;
            int ruanmo = 0;
            //if (context1.Request.Form["companyId"] != null)
            //{
            //    companyId = int.Parse(context1.Request.Form["companyId"]);
            //}
            //if (context1.Request.Form["orderType"] != null)
            //{
            //    orderType = int.Parse(context1.Request.Form["orderType"]);
            //}
            if (context1.Request.Form["guidanceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["guidanceId"]))
            {
                guidanceId = int.Parse(context1.Request.Form["guidanceId"]);

            }
            if (context1.Request.Form["subjectIds"] != null)
            {
                subjectIds = context1.Request.Form["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            if (context1.Request.Form["regions"] != null)
            {
                regions = context1.Request.Form["regions"];
                if (!string.IsNullOrWhiteSpace(regions))
                {
                    regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.Form["provinces"] != null)
            {
                provinces = context1.Request.Form["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["citys"] != null)
            {
                citys = context1.Request.Form["citys"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["customerServiceId"] != null)
            {
                customerServiceIds = context1.Request.Form["customerServiceId"];
                if (!string.IsNullOrWhiteSpace(customerServiceIds))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceIds, ',');
                }
            }
            if (context1.Request.Form["cityTier"] != null)
            {
                cityTier = context1.Request.Form["cityTier"];
                if (!string.IsNullOrWhiteSpace(cityTier))
                {
                    cityTierList = StringHelper.ToStringList(cityTier, ',');
                }
            }
            if (context1.Request.Form["isInstall"] != null)
            {
                isInstall = context1.Request.Form["isInstall"];
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    isInstallList = StringHelper.ToStringList(isInstall, ',');
                }
            }
            if (context1.Request.Form["channel"] != null)
            {
                channel = context1.Request.Form["channel"];
                if (!string.IsNullOrWhiteSpace(channel))
                {
                    channelList = StringHelper.ToStringList(channel, ',');
                }
            }
            if (context1.Request.Form["format"] != null)
            {
                format = context1.Request.Form["format"];
                if (!string.IsNullOrWhiteSpace(format))
                {
                    formatList = StringHelper.ToStringList(format, ',');
                }
            }
            if (context1.Request.Form["shopIds"] != null)
            {
                shopIds = context1.Request.Form["shopIds"];
                if (!string.IsNullOrWhiteSpace(shopIds))
                {
                    selectShopIdList = StringHelper.ToIntList(shopIds, ',');
                }
            }
            if (context1.Request.Form["ruanmo"] != null && !string.IsNullOrWhiteSpace(context1.Request.Form["ruanmo"]))
            {
                ruanmo = int.Parse(context1.Request.Form["ruanmo"]);
            }
           
            List<string> myRegionList = new BasePage().GetResponsibleRegion;
            if (myRegionList.Any())
            {
                StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
            }
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance.ItemId
                        //join category1 in CurrentContext.DbContext.ADSubjectCategory
                        // on subject.SubjectCategoryId equals category1.Id into temp1
                        //from category in temp1.DefaultIfEmpty()
                        where
                        guidance.ItemId == guidanceId

                        && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 3)

                        && (order.IsValid == null || order.IsValid == true)
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                            //&& (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                        && (order.IsProduce == null || order.IsProduce == true)
                        && (order.IsValidFromAssign == null || order.IsValidFromAssign == true)
                        select new
                        {
                            order,
                            shop,
                            subject
                        }).ToList();
            //int roleId = new BasePage().CurrentUser.RoleId;
            //if (roleId == 5)
            //{
            //    int userId = new BasePage().CurrentUser.UserId;
            //    list = list.Where(s => s.order.CSUserId == userId).ToList();
            //}
            if (subjectList.Any())
            {
                list = list.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
            }
            if (regionList.Any())
            {
                list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            }
            if (provinceList.Any())
            {
                list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            }
            if (cityList.Any())
            {
                list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            }
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
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        list = list.Where(s => cityTierList.Contains(s.shop.CityTier) || (s.shop.CityTier == null || s.shop.CityTier == "")).ToList();
                    }
                    else
                        list = list.Where(s => (s.shop.CityTier == null || s.shop.CityTier == "")).ToList();
                }
                else
                {
                    list = list.Where(s => cityTierList.Contains(s.shop.CityTier)).ToList();
                }
            }
            if (isInstallList.Any())
            {
                if (isInstallList.Contains("空"))
                {
                    isInstallList.Remove("空");
                    if (isInstallList.Any())
                    {
                        list = list.Where(s => isInstallList.Contains(s.shop.IsInstall) || (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                    }
                    else
                        list = list.Where(s => (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                }
                else
                {
                    list = list.Where(s => isInstallList.Contains(s.shop.IsInstall)).ToList();
                }
            }
            if (channelList.Any())
            {
                if (channelList.Contains("空"))
                {
                    channelList.Remove("空");
                    if (channelList.Any())
                    {
                        list = list.Where(s => channelList.Contains(s.order.Channel) || (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                    else
                    {
                        list = list.Where(s => (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => channelList.Contains(s.order.Channel)).ToList();

                }
            }
            if (formatList.Any())
            {
                if (formatList.Contains("空"))
                {
                    formatList.Remove("空");
                    if (formatList.Any())
                    {
                        list = list.Where(s => formatList.Contains(s.order.Format) || (s.order.Format == null || s.order.Format == "")).ToList();

                    }
                    else
                    {
                        list = list.Where(s => (s.order.Format == null || s.order.Format == "")).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => formatList.Contains(s.order.Format)).ToList();

                }
            }
            //if (ruanmo > 0)
            //{
            //    if (ruanmo == 1)
            //    {
            //        //只查询软膜的订单
            //        list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
            //    }
            //    else if (ruanmo == 2)
            //    {
            //        list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
            //    }
            //}
            //if (!string.IsNullOrWhiteSpace(assignState))
            //{
            //    List<int> assignShopList = new OutsourceAssignShopBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).ToList();
            //    if (assignState == "0")
            //    {
            //        未分配
            //        list = list.Where(s => !assignShopList.Contains(s.shop.Id)).ToList();
            //    }
            //    else
            //    {
            //        已分配
            //        list = list.Where(s => assignShopList.Contains(s.shop.Id)).ToList();
            //    }
            //}

            //if (shopNoList.Any())
            //{
            //    list = list.Where(s => shopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
            //}
            if (selectShopIdList.Any())
            {
                list = list.Where(s => selectShopIdList.Contains(s.shop.Id)).ToList();
            }
            //orderList = list.Select(s => s.order).ToList();
            //customerId = list[0].subject.CustomerId ?? 0;
            List<int> shopIdList = list.Select(s => s.shop.Id).Distinct().ToList();
            if (shopIdList.Any())
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        //OutsourceAssignShopBLL assignShopBll = new OutsourceAssignShopBLL();
                        //OutsourceOrderDetailBLL assignOrderDetailBll = new OutsourceOrderDetailBLL();
                        new OutsourceOrderDetailBLL().Delete(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0));
                        new OutsourceAssignShopBLL().Delete(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0));
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
            }
            return result;
        }

        public class AssignOrderModel
        {
            public int GuidanceId { get; set; }
            /// <summary>
            /// 生产外协
            /// </summary>
            public int OutsourceId { get; set; }
            /// <summary>
            /// 安装外协
            /// </summary>
            public int InstallOutsourceId { get; set; }
            public int CustomerId { get; set; }
            public int AssignType { get; set; }
            public int RuanMo { get; set; }
            public string MaterialAssign { get; set; }
            public int SubjectId { get; set; }
            public string MaterialPlan { get; set; }
            public string POSScale { get; set; }
            public string MaterialSupport { get; set; }
            public string SubjectCornerType { get; set; }
            public string SubjectCategoryName { get; set; }
            public Shop Shop { get; set; }
            public List<FinalOrderDetailTemp> OrderList { get; set; }
            public List<string> BCSCityTierList { get; set; }

            public List<string> ChannelList { get; set; }
            public List<string> FormatList { get; set; }
            public List<string> SheetList { get; set; }
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
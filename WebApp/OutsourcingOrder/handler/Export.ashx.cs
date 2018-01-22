using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using DAL;
using BLL;
using System.Text;
using Common;

namespace WebApp.OutsourcingOrder.handler
{
    /// <summary>
    /// Export 的摘要说明
    /// </summary>
    public class Export : IHttpHandler
    {
        //FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL(); 
        HttpContext context1;
        int roleId = new BasePage().CurrentUser.RoleId;
        //int companyId = new BasePage().CurrentUser.CompanyId;
        //string subjectId = string.Empty;
        //string region = string.Empty;
        //string province = string.Empty;
        //string city = string.Empty;
        int customerId;
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
            //if (context.Request.QueryString["subjectids"] != null)
            //{
            //    subjectId = context.Request.QueryString["subjectids"];
            //}
            //else if (context.Request.Form["subjectids"] != null)
            //{
            //    subjectId = context.Request.Form["subjectids"];
            //}
            //if (context.Request.QueryString["regions"] != null)
            //{
            //    region = context.Request.QueryString["regions"];
            //}
            //else if (context.Request.Form["regions"] != null)
            //{
            //    region = context.Request.Form["regions"];
            //}
            //if (context.Request.QueryString["province"] != null)
            //{
            //    province = context.Request.QueryString["province"];
            //}
            //else if (context.Request.Form["province"] != null)
            //{
            //    province = context.Request.Form["province"];
            //}
            //if (context.Request.QueryString["city"] != null)
            //{
            //    city = context.Request.QueryString["city"];
            //}
            //else if (context.Request.Form["city"] != null)
            //{
            //    city = context.Request.Form["city"];
            //}

            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            switch (type)
            {
                case "getGuidance":
                    //string beginDate1 = string.Empty;
                    //string endDate1 = string.Empty;
                    //if (context.Request.QueryString["beginDate"] != null)
                    //    beginDate1 = context.Request.QueryString["beginDate"];
                    //if (context.Request.QueryString["endDate"] != null)
                    //    endDate1 = context.Request.QueryString["endDate"];
                    //result = GetGuidance();
                    break;
                
                case "getregion":
                    result = GetRegions();
                    break;
                case "getoutsource":
                    result = GetOutsourcing();
                    break;
                case "getshoplist":
                    result = GetAssignShopList();
                    break;
                
                default:
                    break;
            }
            context.Response.Write(result);
        }

       

        //string GetProjectList(string guidanceIds)
        //{
        //    List<int> guidanceIdList = new List<int>();
        //    if (!string.IsNullOrWhiteSpace(guidanceIds))
        //        guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
        //    string result = string.Empty;
        //    StringBuilder subjectJsonStr = new StringBuilder();
        //    StringBuilder subjectTypeJsonStr = new StringBuilder();
        //    StringBuilder activityJsonStr = new StringBuilder();
        //    //var projectList = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false));
        //    var projectList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
        //                       join subject in CurrentContext.DbContext.Subject
        //                       on order.SubjectId equals subject.Id
        //                       join subjectType1 in CurrentContext.DbContext.SubjectType
        //                       on subject.SubjectTypeId equals subjectType1.Id into typeTemp
        //                       join category1 in CurrentContext.DbContext.ADSubjectCategory
        //                       on subject.SubjectCategoryId equals category1.Id into categoryTemp
        //                       from subjectType in typeTemp.DefaultIfEmpty()
        //                       from category in categoryTemp.DefaultIfEmpty()
        //                       where
        //                       order.OutsourceId == companyId
        //                       && guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
        //                       && subject.ApproveState == 1
        //                       select new
        //                       {
        //                           subject.Id,
        //                           subject.SubjectCategoryId,
        //                           subject.SubjectName,
        //                           subject.SubjectTypeId,
        //                           subject.ActivityId,
        //                           subjectType.SubjectTypeName,
        //                           category.CategoryName
        //                       }).OrderBy(s => s.SubjectTypeId).Distinct().ToList();
        //    if (projectList.Any())
        //    {
        //        Dictionary<int, string> typeDic = new Dictionary<int, string>();
        //        Dictionary<int, string> activityDic = new Dictionary<int, string>();
        //        bool typeEmpty = false;
        //        bool activityEmpty = false;
        //        projectList.ForEach(s =>
        //        {
        //            if ((s.SubjectTypeId ?? 0) == 0)
        //                typeEmpty = true;
        //            if ((s.SubjectCategoryId ?? 0) == 0)
        //                activityEmpty = true;
        //            if (!typeDic.Keys.Contains(s.SubjectTypeId ?? 0))
        //                typeDic.Add(s.SubjectTypeId ?? 0, s.SubjectTypeName);
        //            if (!activityDic.Keys.Contains(s.SubjectCategoryId ?? 0))
        //                activityDic.Add(s.SubjectCategoryId ?? 0, s.CategoryName);
        //            subjectJsonStr.Append("{\"Id\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
        //        });
        //        foreach (KeyValuePair<int, string> item in typeDic)
        //        {
        //            if (!string.IsNullOrWhiteSpace(item.Value))
        //                subjectTypeJsonStr.Append("{\"Id\":\"" + item.Key + "\",\"TypeName\":\"" + item.Value + "\"},");

        //        }
        //        if (typeEmpty)
        //        {
        //            subjectTypeJsonStr.Append("{\"Id\":\"0\",\"TypeName\":\"空\"},");
        //        }
        //        foreach (KeyValuePair<int, string> item in activityDic)
        //        {
        //            if (!string.IsNullOrWhiteSpace(item.Value))
        //                activityJsonStr.Append("{\"Id\":\"" + item.Key + "\",\"ActivityName\":\"" + item.Value + "\"},");
        //        }
        //        if (activityEmpty)
        //        {
        //            activityJsonStr.Append("{\"Id\":\"0\",\"ActivityName\":\"空\"},");
        //        }
        //        result = "[" + subjectJsonStr.ToString().TrimEnd(',') + "]" + "|[" + subjectTypeJsonStr.ToString().TrimEnd(',') + "]" + "|[" + activityJsonStr.ToString().TrimEnd(',') + "]";
        //    }
        //    return result;
        //}

        //string ScreenProject(string guidanceIds, string activityIds, string typeIds)
        //{
        //    string result = string.Empty;
        //    List<int> guidanceIdList = new List<int>();
        //    if (!string.IsNullOrWhiteSpace(guidanceIds))
        //        guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
        //    List<int> activityList = new List<int>();
        //    List<int> typeList = new List<int>();
        //    if (!string.IsNullOrWhiteSpace(activityIds))
        //    {
        //        activityList = StringHelper.ToIntList(activityIds, ',');
        //    }
        //    if (!string.IsNullOrWhiteSpace(typeIds))
        //    {
        //        typeList = StringHelper.ToIntList(typeIds, ',');
        //    }
        //    var projectList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp 
        //                       join  subject in CurrentContext.DbContext.Subject
        //                       on order.SubjectId equals subject.Id
        //                       join subjectType1 in CurrentContext.DbContext.SubjectType
        //                       on subject.SubjectTypeId equals subjectType1.Id into typeTemp
        //                       join category1 in CurrentContext.DbContext.ADSubjectCategory
        //                       on subject.SubjectCategoryId equals category1.Id into categoryTemp
        //                       from subjectType in typeTemp.DefaultIfEmpty()
        //                       from category in categoryTemp.DefaultIfEmpty()
        //                       where order.OutsourceId == companyId 
        //                       && guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
        //                       && subject.ApproveState == 1
        //                       select subject).OrderBy(s => s.SubjectTypeId).Distinct().ToList();
        //    if (activityList.Any())
        //    {
        //        projectList = projectList.Where(s => activityList.Contains(s.SubjectCategoryId ?? 0)).ToList();
        //    }
        //    if (typeList.Any())
        //    {
        //        projectList = projectList.Where(s => typeList.Contains(s.SubjectTypeId ?? 0)).ToList();
        //    }
        //    if (projectList.Any())
        //    {
        //        StringBuilder subjectJsonStr = new StringBuilder();
        //        projectList.ForEach(s =>
        //        {
        //            subjectJsonStr.Append("{\"Id\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
        //        });
        //        result = "[" + subjectJsonStr.ToString().TrimEnd(',') + "]";
        //    }
        //    return result;
        //}

        string GetRegions()
        {
            int guidanceId = -1;
            if (context1.Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(context1.Request.QueryString["guidanceId"]);
            }

            var list = (from outsourceShop in CurrentContext.DbContext.OutsourceAssignShop
                        join shop in CurrentContext.DbContext.Shop
                        on outsourceShop.ShopId equals shop.Id
                        where outsourceShop.GuidanceId == guidanceId
                        select shop).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                var list1 = list.Select(s => s.RegionName).Distinct().ToList();
                list1.ForEach(s =>
                {
                    json.Append("{\"RegionName\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
           
           return "";
        }

        //string GetProvince()
        //{
        //    if (!string.IsNullOrWhiteSpace(subjectId) && !string.IsNullOrWhiteSpace(region))
        //    {
        //        List<int> sidList = StringHelper.ToIntList(subjectId, ',');
        //        List<string> regionList = StringHelper.ToStringList(region, ',');
        //        //var list = orderBll.GetList(s => sidList.Contains(s.SubjectId ?? 0) && regionList.Contains(s.Region)).OrderBy(s => s.Region).ThenBy(s => s.Province).ToList();
        //        var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
        //                    join shop in CurrentContext.DbContext.Shop
        //                    on order.ShopId equals shop.Id
        //                    where sidList.Contains(order.SubjectId ?? 0) && regionList.Contains(shop.RegionName)
        //                    && order.OutsourceId == companyId
        //                    select new
        //                    {
        //                        shop.ProvinceName
        //                    }
        //                          ).ToList();
        //        if (list.Any())
        //        {
        //            var list1 = list.Select(s => s.ProvinceName).Distinct().ToList();
        //            StringBuilder json = new StringBuilder();
        //            list1.ForEach(s =>
        //            {
        //                json.Append("{\"ProvinceName\":\"" + s + "\"},");
        //            });
        //            return "[" + json.ToString().TrimEnd(',') + "]";
        //        }
        //        else
        //            return "";
        //    }
        //    else
        //        return "";
        //}

        //string GetCity()
        //{
        //    if (!string.IsNullOrWhiteSpace(subjectId) && !string.IsNullOrWhiteSpace(region) && !string.IsNullOrWhiteSpace(province))
        //    {
        //        List<int> sidList = StringHelper.ToIntList(subjectId, ',');
        //        List<string> regionList = StringHelper.ToStringList(region, ',');
        //        List<string> provinceList = StringHelper.ToStringList(province, ',');
        //        //var list = orderBll.GetList(s => sidList.Contains(s.SubjectId ?? 0) && regionList.Contains(s.Region) && provinceList.Contains(s.Province)).OrderBy(s => s.Region).ThenBy(s => s.Province).ThenBy(s => s.City).ToList();
        //        var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
        //                    join shop in CurrentContext.DbContext.Shop
        //                    on order.ShopId equals shop.Id
        //                    where sidList.Contains(order.SubjectId ?? 0) && regionList.Contains(shop.RegionName) && provinceList.Contains(shop.ProvinceName)
        //                    && order.OutsourceId == companyId
        //                    select new
        //                    {
        //                        shop.CityName
        //                    }
        //                          ).ToList();
        //        if (list.Any())
        //        {
        //            var list1 = list.Select(s => s.CityName).Distinct().ToList();
        //            StringBuilder json = new StringBuilder();
        //            list1.ForEach(s =>
        //            {
        //                json.Append("{\"CityName\":\"" + s + "\"},");
        //            });
        //            return "[" + json.ToString().TrimEnd(',') + "]";
        //        }
        //        else
        //            return "";
        //    }
        //    else
        //        return "";
        //}

        //string GetOrderDetailList(int currPage, int pageSize)
        //{
        //    string result = string.Empty;
        //    int shopCount = 0;
        //    if (!string.IsNullOrWhiteSpace(subjectId))
        //    {
        //        List<int> subjectList = StringHelper.ToIntList(subjectId, ',');
        //        var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
        //                         join shop in CurrentContext.DbContext.Shop
        //                         on order.ShopId equals shop.Id
        //                         join pop1 in CurrentContext.DbContext.POP
        //                         on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
        //                         from pop in popTemp.DefaultIfEmpty()
        //                         join company1 in CurrentContext.DbContext.Company
        //                         on order.OutsourceId equals company1.Id into companyTemp
        //                         from company in companyTemp.DefaultIfEmpty()
        //                         join subject in CurrentContext.DbContext.Subject
        //                         on order.SubjectId equals subject.Id
        //                         join guidance in CurrentContext.DbContext.SubjectGuidance
        //                         on subject.GuidanceId equals guidance.ItemId
        //                         where subjectList.Contains(order.SubjectId ?? 0)
        //                         //&& ((order.OrderType != null && order.OrderType == 1 && order.GraphicWidth != null && order.GraphicWidth != 0 && order.GraphicLength != null && order.GraphicLength != 0) || (order.OrderType != null && order.OrderType == 2))
        //                         //&& (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
        //                         //&& (order.IsDelete == null || order.IsDelete == false)
        //                         && order.OutsourceId == companyId
        //                         select new
        //                         {
        //                             order,
        //                             pop,
        //                             shop,
        //                             CompanyName = company.CompanyName,
        //                             SubjectName = subject.SubjectName,
        //                             GuidanceName = guidance.ItemName
        //                         }
        //                         ).OrderBy(s => s.shop.Id).ToList();
        //        if (orderList.Any())
        //        {
        //            if (!string.IsNullOrWhiteSpace(region))
        //            {
        //                List<string> regionList = StringHelper.ToStringList(region, ',');
        //                orderList = orderList.Where(s => regionList.Contains(s.order.Region)).ToList();
        //            }
        //            if (!string.IsNullOrWhiteSpace(province))
        //            {
        //                List<string> provinceList = StringHelper.ToStringList(province, ',');
        //                orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
        //            }
        //            if (!string.IsNullOrWhiteSpace(city))
        //            {
        //                List<string> cityList = StringHelper.ToStringList(city, ',');
        //                orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
        //            }
                    
        //            if (orderList.Any())
        //            {
        //                shopCount = orderList.Select(s => s.shop).Distinct().Count();
        //                StringBuilder json = new StringBuilder();
        //                int total = orderList.Count;
        //                orderList = orderList.OrderBy(s => s.order.ShopId).Skip(currPage * pageSize).Take(pageSize).ToList();

        //                orderList.ForEach(s =>
        //                {

        //                    string orderType = s.order.OrderType != null && s.order.OrderType == 1 ? "POP" : "道具";
        //                    string levelNum = string.Empty;
        //                    if (s.order.LevelNum != null && s.order.LevelNum > 0)
        //                    {

        //                        levelNum = CommonMethod.GeEnumName<LevelNumEnum>(s.order.LevelNum.ToString());
        //                    }

        //                    json.Append("{\"total\":\"" + total + "\",\"Id\":\"" + s.order.Id + "\",\"SubjectName\":\""+s.SubjectName+"\",\"GuidanceName\":\""+s.GuidanceName+"\",\"OrderType\":\"" + orderType + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",");
        //                    json.Append("\"Region\":\"" + s.shop.RegionName + "\",\"Province\":\"" + s.shop.ProvinceName + "\",\"City\":\"" + s.shop.CityName + "\",");
        //                    json.Append("\"CityTier\":\"" + s.shop.CityTier + "\",\"Format\":\"" + s.shop.Format + "\",\"MaterialSupport\":\"" + s.order.MaterialSupport + "\",");
        //                    json.Append("\"POSScale\":\"" + s.order.POSScale + "\",\"GraphicNo\":\"" + s.order.GraphicNo + "\",\"POPName\":\"" + s.order.POPName + "\",");
        //                    json.Append("\"POPType\":\"" + s.order.POPType + "\",\"Sheet\":\"" + s.order.Sheet + "\",\"Gender\":\"" + s.order.Gender + "\",");
        //                    json.Append("\"Quantity\":\"" + s.order.Quantity + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",");
        //                    json.Append("\"MachineFrame\":\"" + s.order.MachineFrame + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",");
        //                    json.Append("\"Area\":\"" + s.order.Area + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"WindowWide\":\"" + (s.order.WindowWide != null ? s.order.WindowWide.ToString() : "") + "\",");
        //                    json.Append("\"WindowHigh\":\"" + (s.order.WindowHigh != null ? s.order.WindowHigh.ToString() : "") + "\",\"WindowDeep\":\"" + (s.order.WindowDeep != null ? s.order.WindowDeep.ToString() : "") + "\",\"WindowSize\":\"" + s.order.WindowSize + "\",");
        //                    json.Append("\"Style\":\"" + (s.pop != null ? s.pop.Style : "") + "\",\"CornerType\":\"" + (s.pop != null ? s.pop.CornerType : "") + "\",\"Category\":\"" + (s.pop != null ? s.pop.Category : "") + "\",");
        //                    json.Append("\"StandardDimension\":\"" + (s.pop != null ? s.pop.StandardDimension : "") + "\",\"Modula\":\"" + (s.pop != null ? s.pop.Modula : "") + "\",\"Frame\":\"" + (s.pop != null ? s.pop.Frame : "") + "\",");
        //                    json.Append("\"DoubleFace\":\"" + (s.pop != null ? s.pop.DoubleFace : "") + "\",\"Glass\":\"" + (s.pop != null ? s.pop.Glass : "") + "\",\"Backdrop\":\"" + (s.pop != null ? s.pop.Backdrop : "") + "\",");
        //                    json.Append("\"ModulaQuantityWidth\":\"" + (s.pop != null && s.pop.ModulaQuantityWidth != null ? s.pop.ModulaQuantityWidth.ToString() : "") + "\",\"ModulaQuantityHeight\":\"" + (s.pop != null && s.pop.ModulaQuantityHeight != null ? s.pop.ModulaQuantityHeight.ToString() : "") + "\",\"PlatformLength\":\"" + (s.pop != null && s.pop.PlatformLength != null ? s.pop.PlatformLength.ToString() : "") + "\",");
        //                    json.Append("\"PlatformWidth\":\"" + (s.pop != null && s.pop.PlatformWidth != null ? s.pop.PlatformWidth.ToString() : "") + "\",\"PlatformHeight\":\"" + (s.pop != null && s.pop.PlatformHeight != null ? s.pop.PlatformHeight.ToString() : "") + "\",\"FixtureType\":\"" + (s.pop != null ? s.pop.FixtureType : "") + "\",");
        //                    json.Append("\"ChooseImg\":\"" + (s.order.ChooseImg) + "\",\"Remark\":\"" + (s.order.Remark) + "\",\"LevelNum\":\"" + levelNum + "\",\"CompanyName\":\"" + s.CompanyName + "\"},");
        //                });
        //                result = "[" + json.ToString().TrimEnd(',') + "]$" + shopCount;
        //            }
        //        }
        //    }
        //    return result;
        //}

        string GetOutsourcing()
        {
            string r = string.Empty;
            
            List<string> regionList = new List<string>();
            int guidanceId = -1;
            int customerId = 0;
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            if (context1.Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(context1.Request.QueryString["guidanceId"]);
            }
            if (context1.Request.QueryString["regions"] != null)
            {
                string regions = string.Empty;
                regions = context1.Request.QueryString["regions"];
                regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
            }
            var companyList = (from assign in CurrentContext.DbContext.OutsourceAssignShop
                               join company in CurrentContext.DbContext.Company
                               on assign.OutsourceId equals company.Id
                               where assign.GuidanceId == guidanceId
                               && company.TypeId == (int)CompanyTypeEnum.Outsource 
                               && (company.IsDelete == null || company.IsDelete == false)
                               select company).Distinct().ToList();
            //var companyList = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.IsDelete == null || s.IsDelete == false) && (provinceIdList.Any() ? (provinceIdList.Contains(s.ProvinceId ?? 0)) : 1 == 1)).OrderBy(s => s.ProvinceId).ToList();
            if (companyList.Any())
            {
                if (regionList.Any())
                {
                    List<int> provinceIdList = new List<int>();
                    provinceIdList = (from pir in CurrentContext.DbContext.ProvinceInRegion
                                      join region in CurrentContext.DbContext.Region
                                      on pir.RegionId equals region.Id
                                      where regionList.Contains(region.RegionName.ToLower())
                                      && region.CustomerId == customerId
                                      select pir.ProvinceId ?? 0).Distinct().ToList();
                    companyList = companyList.Where(s => provinceIdList.Contains(s.ProvinceId ?? 0)).ToList();
                }
                StringBuilder json = new StringBuilder();
                companyList.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CompanyName\":\"" + s.CompanyName + "\"},");
                });
                r = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return r;
        }


        string GetAssignShopList()
        {
            string result = string.Empty;
            int currPage = 0;
            int pageSize = 20;
            int guidanceId = 0;
            int outsourceId = 0;
            int orderType = 0;
            if (context1.Request.QueryString["currpage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            if (context1.Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(context1.Request.QueryString["guidanceId"]);
            }
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                outsourceId = int.Parse(context1.Request.QueryString["outsourceId"]);
            }
            if (context1.Request.QueryString["orderType"] != null)
            {
                orderType = int.Parse(context1.Request.QueryString["orderType"]);
            }
            var shopList = (from assign in CurrentContext.DbContext.OutsourceAssignShop
                        join shop in CurrentContext.DbContext.Shop
                        on assign.ShopId equals shop.Id
                        where assign.GuidanceId == guidanceId
                        && assign.OutsourceId == outsourceId
                        && assign.AssignType == orderType
                        select shop).ToList();
            if (shopList.Any())
            {
                StringBuilder json = new StringBuilder();
                int total = shopList.Count;
                shopList = shopList.OrderBy(s => s.ShopNo).Skip(currPage * pageSize).Take(pageSize).ToList();
                shopList.ForEach(s =>
                {
                    json.Append("{\"total\":\"" + total + "\",\"ShopId\":\"" + s.Id + "\",\"ShopNo\":\"" + s.ShopNo + "\",\"ShopName\":\"" + s.ShopName + "\",\"Region\":\"" + s.RegionName + "\",\"Province\":\"" + s.ProvinceName + "\",\"City\":\"" + s.CityName + "\",\"CityTier\":\"" + s.CityTier + "\",\"Format\":\"" + s.Format + "\",\"Address\":\"" + s.POPAddress + "\"},");
                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
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
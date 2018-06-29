using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using DAL;
using Models;
using Common;

using Newtonsoft.Json;
using System.Transactions;

namespace WebApp.Subjects.Handler
{
    /// <summary>
    /// CheckOrder 的摘要说明
    /// </summary>
    public class CheckOrder : IHttpHandler
    {
        string type = string.Empty;
        int customerId;
        int companyId;
        int planId;
        string subjectIds = string.Empty;
        List<int> companyList = new List<int>();
        OrderPlanBLL orderPlanBll = new OrderPlanBLL();
        //OrderPlan planModel;
        CheckOrderPlanDetailBLL checkOrderDetailBll = new CheckOrderPlanDetailBLL();
        //CheckOrderPlanDetail checkOrderModel;
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            companyId = new BasePage().CurrentUser.CompanyId;
            companyList = new BasePage().MySonCompanyList.Select(s => s.Id).ToList();
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            if (context.Request.QueryString["planId"] != null)
            {
                planId = int.Parse(context.Request.QueryString["planId"]);
            }
            if (context.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context.Request.QueryString["subjectIds"];
            }
            switch (type)
            {
                case "getProjects":
                    string begin = context.Request.QueryString["beginDate"];
                    string end = context.Request.QueryString["endDate"];
                    result = GetProjects(begin, end);
                    break;
                case "getMaterials":
                    string begin1 = context.Request.QueryString["beginDate"];
                    string end1 = context.Request.QueryString["endDate"];
                    result = GetMaterials(begin1, end1);
                    break;
                case "getContents":
                    result = GetContents();
                    break;
                case "getPosition":
                    //获取位置
                    result = GetPositions();
                    break;
                case "add":
                    string optype = context.Request.QueryString["optype"];
                    string jsonstr = context.Request.Form["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(jsonstr))
                    {
                        jsonstr = HttpUtility.UrlDecode(jsonstr);
                    }
                    result = AddPlan(optype, jsonstr);
                    break;
                case "getList":
                    string Ids = null;
                    string beginDate = string.Empty;
                    string endDate = string.Empty;
                    if (context.Request.QueryString["Ids"] != null)
                    {
                        Ids = context.Request.QueryString["Ids"];
                    }
                    if (context.Request.QueryString["beginDate"] != null)
                    {
                        beginDate = context.Request.QueryString["beginDate"];
                    }
                    if (context.Request.QueryString["endDate"] != null)
                    {
                        endDate = context.Request.QueryString["endDate"];
                    }
                    result = GetPlanList(beginDate, endDate, Ids);
                    break;
                case "getDetail":
                    result = GetDetailList();
                    break;
                case "deleteDetail":
                    int detailId = int.Parse(context.Request.QueryString["detailId"]);
                    result = DeleteDetail(detailId);
                    break;
                case "deletePlan":
                    result = DeletePlan();
                    break;
                case "deletePlans":
                    string planIds0 = context.Request.QueryString["planIds"];
                    result = DeletePlan(planIds0);
                    break;
                case "checkOrder":
                    //开始拆单
                    string planIds = context.Request.QueryString["planIds"];
                    string subjectIds = context.Request.QueryString["subjectIds"];
                    result = GoCheckOrder(planIds, subjectIds);
                    break;
                case "getGuidance":
                    string guidanceMonth = string.Empty;
                    if (context.Request.QueryString["guidanceMonth"] != null)
                    {
                        guidanceMonth = context.Request.QueryString["guidanceMonth"];
                    }
                    //string beginDate1 = string.Empty;
                    //string endDate1 = string.Empty;
                    //if (context.Request.QueryString["beginDate"] != null)
                    //    beginDate1 = context.Request.QueryString["beginDate"];
                    //if (context.Request.QueryString["endDate"] != null)
                    //    endDate1 = context.Request.QueryString["endDate"];
                    result = GetGuidance(guidanceMonth);
                    break;
                case "getProjectList":
                    //int guidanceId = int.Parse(context.Request.QueryString["guidanceId"]);
                    string guidanceIds = context.Request.QueryString["guidanceIds"];
                    result = GetProjectList(guidanceIds);
                    break;
                case "getRegionProjectList":
                    //int guidanceId = int.Parse(context.Request.QueryString["guidanceId"]);
                    string guidanceIds0 = context.Request.QueryString["guidanceIds"];
                    result = GetRegionProjectList(guidanceIds0);
                    break;
                case "screenProject":
                    string region = context.Request.QueryString["region"];
                    string guidanceIds1 = context.Request.QueryString["guidanceIds"];
                    string activityId = context.Request.QueryString["activityId"];
                    string typeId = context.Request.QueryString["typeId"];
                    result = ScreenProject(guidanceIds1,region, activityId, typeId);
                    break;

            }
            context.Response.Write(result);
        }

        string GetGuidance(string guidanceMonth)
        {
            SubjectGuidanceBLL guidanceBll = new SubjectGuidanceBLL();
            string result = string.Empty;
            var guidanceList = new SubjectBLL().GetList(s => s.ApproveState == 1 && (s.IsDelete == null || s.IsDelete == false)).Select(s => s.GuidanceId ?? 0).Distinct().ToList();
            var list = guidanceBll.GetList(s => s.CustomerId == customerId && guidanceList.Contains(s.ItemId) && (s.IsDelete==null || s.IsDelete==false));
            
            DateTime date = DateTime.Now;
            int year = date.Year;
            int month = date.Month;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date0 = DateTime.Parse(guidanceMonth);
                year = date0.Year;
                month = date0.Month;

            }
            list = list.Where(s => s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
            if (list.Any())
            {
                StringBuilder jsonStr = new StringBuilder();
                list.ForEach(s =>
                {
                    jsonStr.Append("{\"Id\":\"" + s.ItemId + "\",\"GuidanceName\":\"" + s.ItemName + "\"},");
                });
                result = "[" + jsonStr.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetProjectList(string guidanceIds)
        {
            List<int> guidanceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
            string result = string.Empty;
            StringBuilder subjectJsonStr = new StringBuilder();
            StringBuilder subjectTypeJsonStr = new StringBuilder();
            StringBuilder activityJsonStr = new StringBuilder();
            //var projectList = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false));
            List<string> regionList = new List<string>();
            List<string> myRegion = new BasePage().GetResponsibleRegion;
            if (myRegion.Any())
            {
                StringHelper.ToUpperOrLowerList(ref myRegion, LowerUpperEnum.ToLower);

            }
            //POP订单
            var popOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                where guidanceIdList.Contains(subject.GuidanceId ?? 0) 
                                && (subject.IsDelete == null || subject.IsDelete == false)
                                && subject.ApproveState == 1
                                //&& subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                                //&& (myRegion.Any() ? (myRegion.Contains(shop.RegionName.ToLower())) : 1 == 1)
                                && (myRegion.Any()?((subject.PriceBlongRegion!=null && subject.PriceBlongRegion!="")?(myRegion.Contains(subject.PriceBlongRegion.ToLower())):(myRegion.Contains(shop.RegionName.ToLower()))):1==1)
                                && (order.IsDelete == null || order.IsDelete==false)
                                select new {
                                    SubjectId= subject.Id,
                                    shop
                                }).ToList();

            regionList = popOrderList.Select(s => s.shop.RegionName).Distinct().ToList();
            List<int> orderSubjectIdList = popOrderList.Select(s => s.SubjectId).Distinct().ToList();
            
           
            //物料订单

            var materialOrderList = (from materialOrder in CurrentContext.DbContext.OrderMaterial
                                     join shop in CurrentContext.DbContext.Shop
                                     on materialOrder.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on materialOrder.SubjectId equals subject.Id
                                     where guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
                                     && subject.ApproveState == 1
                                     //&& (myRegion.Any() ? (myRegion.Contains(shop.RegionName.ToLower())) : 1 == 1)
                                     && (myRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? (myRegion.Contains(subject.PriceBlongRegion.ToLower())) : (myRegion.Contains(shop.RegionName.ToLower()))) : 1 == 1)
                                     select new {
                                         SubjectId = subject.Id,
                                         shop
                                     }).Distinct().ToList();
            List<string> regionList1 = materialOrderList.Select(s => s.shop.RegionName).Distinct().ToList();
            List<int> materialOrderSubjectIdList = materialOrderList.Select(s => s.SubjectId).Distinct().ToList();
           

            regionList.AddRange(regionList1);
            orderSubjectIdList.AddRange(materialOrderSubjectIdList);
            //获取项目信息
            var projectList = (from subject in CurrentContext.DbContext.Subject
                               join subjectType1 in CurrentContext.DbContext.SubjectType
                               on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                               join category1 in CurrentContext.DbContext.ADSubjectCategory
                               on subject.SubjectCategoryId equals category1.Id into categoryTemp
                               from subjectType in typeTemp.DefaultIfEmpty()
                               from category in categoryTemp.DefaultIfEmpty()
                               where orderSubjectIdList.Contains(subject.Id)

                               select new
                               {
                                   subject.Id,
                                   subject.SubjectCategoryId,
                                   subject.SubjectName,
                                   subject.SubjectTypeId,
                                   subject.ActivityId,
                                   subject.Remark,
                                   subjectType.SubjectTypeName,
                                   category.CategoryName
                               }).OrderBy(s => s.SubjectTypeId).ToList();

            
            if (projectList.Any())
            {


                Dictionary<int, string> typeDic = new Dictionary<int, string>();
                Dictionary<int, string> activityDic = new Dictionary<int, string>();
                bool typeEmpty = false;
                bool activityEmpty = false;
                projectList.ForEach(s =>
                {
                    if ((s.SubjectTypeId ?? 0) == 0)
                        typeEmpty = true;
                    if ((s.SubjectCategoryId ?? 0) == 0)
                        activityEmpty = true;
                    if (!typeDic.Keys.Contains(s.SubjectTypeId ?? 0))
                        typeDic.Add(s.SubjectTypeId ?? 0, s.SubjectTypeName);
                    if (!activityDic.Keys.Contains(s.SubjectCategoryId ?? 0))
                        activityDic.Add(s.SubjectCategoryId ?? 0, s.CategoryName);
                    string subjectName = s.SubjectName;
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(s.Remark))
                        subjectName += "(" + s.Remark + ")";
                    subjectJsonStr.Append("{\"Id\":\"" + s.Id + "\",\"SubjectName\":\"" + subjectName + "\"},");
                });
                foreach (KeyValuePair<int, string> item in typeDic)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value))
                        subjectTypeJsonStr.Append("{\"Id\":\"" + item.Key + "\",\"TypeName\":\"" + item.Value + "\"},");

                }
                if (typeEmpty)
                {
                    subjectTypeJsonStr.Append("{\"Id\":\"0\",\"TypeName\":\"空\"},");
                }
                foreach (KeyValuePair<int, string> item in activityDic)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value))
                        activityJsonStr.Append("{\"Id\":\"" + item.Key + "\",\"ActivityName\":\"" + item.Value + "\"},");
                }
                if (activityEmpty)
                {
                    activityJsonStr.Append("{\"Id\":\"0\",\"ActivityName\":\"空\"},");
                }

                //区域信息
                StringBuilder regionJsonStr = new StringBuilder();
                //if (myRegion.Any())
                //{
                //    regionList = myRegion;
                //}
                if (regionList.Any())
                {
                    bool isEmpty = false;
                    regionList.Distinct().ToList().ForEach(s => {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            regionJsonStr.Append("{\"RegionName\":\"" + s + "\"},");
                        }
                        else
                            isEmpty = true;
                    });
                    if(isEmpty)
                        regionJsonStr.Append("{\"RegionName\":\"空\"},");
                }


                result = "[" + subjectJsonStr.ToString().TrimEnd(',') + "]" + "|[" + subjectTypeJsonStr.ToString().TrimEnd(',') + "]" + "|[" + activityJsonStr.ToString().TrimEnd(',') + "]|[" + regionJsonStr.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetRegionProjectList(string guidanceIds)
        {
            List<int> guidanceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
            string result = string.Empty;
            StringBuilder subjectJsonStr = new StringBuilder();
            StringBuilder subjectTypeJsonStr = new StringBuilder();
            StringBuilder activityJsonStr = new StringBuilder();
            //var projectList = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false));
            //List<string> regionList = new List<string>();
            List<string> myRegion = new BasePage().GetResponsibleRegion;
            if (myRegion.Any())
            {
                StringHelper.ToUpperOrLowerList(ref myRegion, LowerUpperEnum.ToLower);

            }
            //POP订单
            var popOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                join subject in CurrentContext.DbContext.Subject
                                on order.RegionSupplementId equals subject.Id
                                where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                && (subject.IsDelete == null || subject.IsDelete == false)
                                && subject.ApproveState == 1
                                && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                                && order.IsFromRegion==true
                                && (myRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? (myRegion.Contains(subject.PriceBlongRegion.ToLower())) : (myRegion.Contains(shop.RegionName.ToLower()))) : 1 == 1)
                                && (order.IsDelete == null || order.IsDelete == false)
                                select new
                                {
                                    SubjectId = subject.Id,
                                    //shop
                                }).ToList();

            //regionList = popOrderList.Select(s => s.shop.RegionName).Distinct().ToList();
            List<int> orderSubjectIdList = popOrderList.Select(s => s.SubjectId).Distinct().ToList();

            //获取项目信息
            var projectList = (from subject in CurrentContext.DbContext.Subject
                               join subjectType1 in CurrentContext.DbContext.SubjectType
                               on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                               join category1 in CurrentContext.DbContext.ADSubjectCategory
                               on subject.SubjectCategoryId equals category1.Id into categoryTemp
                               from subjectType in typeTemp.DefaultIfEmpty()
                               from category in categoryTemp.DefaultIfEmpty()
                               where orderSubjectIdList.Contains(subject.Id)
                               select new
                               {
                                   subject.Id,
                                   subject.SubjectCategoryId,
                                   subject.SubjectName,
                                   subject.SubjectTypeId,
                                   subject.ActivityId,
                                   subject.Remark,
                                   subjectType.SubjectTypeName,
                                   category.CategoryName
                               }).OrderBy(s => s.SubjectTypeId).ToList();
            if (projectList.Any())
            {
                projectList.ForEach(s =>
                {

                    string subjectName = s.SubjectName;
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(s.Remark))
                        subjectName += "(" + s.Remark + ")";
                    subjectJsonStr.Append("{\"Id\":\"" + s.Id + "\",\"SubjectName\":\"" + subjectName + "\"},");
                });
                result = "[" + subjectJsonStr.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string ScreenProject(string guidanceIds,string regions, string activityIds, string typeIds)
        {
            string result = string.Empty;
            List<int> guidanceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
            List<string> regionList = new List<string>();
            List<int> activityList = new List<int>();
            List<int> typeList = new List<int>();
            if (!string.IsNullOrWhiteSpace(regions))
            {
                regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
            }
            if (!string.IsNullOrWhiteSpace(activityIds))
            {
                activityList = StringHelper.ToIntList(activityIds, ',');
            }
            if (!string.IsNullOrWhiteSpace(typeIds))
            {
                typeList = StringHelper.ToIntList(typeIds, ',');
            }
            
            List<string> myRegion = new BasePage().GetResponsibleRegion;
            if (myRegion.Any())
            {
                StringHelper.ToUpperOrLowerList(ref myRegion, LowerUpperEnum.ToLower);

            }
            //POP订单
            var popOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                where guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
                                && subject.ApproveState == 1
                                //&& subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                                //&& (myRegion.Any() ? (myRegion.Contains(shop.RegionName.ToLower())) : 1 == 1)
                                && (order.IsDelete == null || order.IsDelete == false)
                                && (myRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? (myRegion.Contains(subject.PriceBlongRegion.ToLower())) : (myRegion.Contains(shop.RegionName.ToLower()))) : 1 == 1)
                                select new {
                                    shop,
                                    subject
                                }).ToList();
            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    List<string> regionList0 = regionList;
                    regionList0.Remove("空");
                    if (regionList0.Any())
                    {
                        popOrderList = popOrderList.Where(s => (regionList0.Contains(s.shop.RegionName.ToLower())) || (s.shop.RegionName == null || s.shop.RegionName == "")).ToList();
                    }
                    else
                        popOrderList = popOrderList.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                }
                else
                {
                    popOrderList = popOrderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    //popOrderList = popOrderList.Where(s => ((s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? (regionList.Contains(s.subject.PriceBlongRegion.ToLower())) : (regionList.Contains(s.shop.RegionName.ToLower())))).ToList();
                }
            }
            List<Subject> orderSubjectIdList = popOrderList.Select(s => s.subject).ToList();
            
            //List<Subject> orderSubjectIdList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //                                join shop in CurrentContext.DbContext.Shop
            //                                on order.ShopId equals shop.Id
            //                                join subject in CurrentContext.DbContext.Subject
            //                                on order.SubjectId equals subject.Id
            //                                where guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
            //                                && subject.ApproveState == 1
            //                                && subject.SubjectType != (int)SubjectTypeEnum.二次安装费
            //                                && subject.SubjectType != (int)SubjectTypeEnum.费用订单
            //                                && (myRegion.Any() ? (myRegion.Contains(shop.RegionName.ToLower())) : 1 == 1)
            //                                && (regionList.Any() ? (regionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
            //                                select subject

            //                            ).ToList();

            //物料订单
            var materialOrderList = (from materialOrder in CurrentContext.DbContext.OrderMaterial
                                     join shop in CurrentContext.DbContext.Shop
                                     on materialOrder.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on materialOrder.SubjectId equals subject.Id
                                     where guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
                                     && subject.ApproveState == 1
                                     //&& (myRegion.Any() ? (myRegion.Contains(shop.RegionName.ToLower())) : 1 == 1)
                                     && (myRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? (myRegion.Contains(subject.PriceBlongRegion.ToLower())) : (myRegion.Contains(shop.RegionName.ToLower()))) : 1 == 1)
                                     select new {
                                         shop,
                                         subject
                                     }).ToList();
            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    List<string> regionList0 = regionList;
                    regionList0.Remove("空");
                    if (regionList0.Any())
                    {
                        materialOrderList = materialOrderList.Where(s => regionList0.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                        //materialOrderList = materialOrderList.Where(s => ((s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? (regionList0.Contains(s.subject.PriceBlongRegion.ToLower())) : (regionList0.Contains(s.shop.RegionName.ToLower()))) || (s.shop.RegionName == null || s.shop.RegionName == "")).ToList();
                    }
                    else
                        materialOrderList = materialOrderList.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                }
                else
                {
                    materialOrderList = materialOrderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    //materialOrderList = materialOrderList.Where(s => ((s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? (regionList.Contains(s.subject.PriceBlongRegion.ToLower())) : (regionList.Contains(s.shop.RegionName.ToLower())))).ToList();
                }
            }
            List<Subject> materialOrderSubjectList = materialOrderList.Select(s => s.subject).ToList();

            //List<Subject> materialOrderSubjectList = (from materialOrder in CurrentContext.DbContext.OrderMaterial
            //                                      join shop in CurrentContext.DbContext.Shop
            //                                      on materialOrder.ShopId equals shop.Id
            //                                      join subject in CurrentContext.DbContext.Subject
            //                                      on materialOrder.SubjectId equals subject.Id
            //                                      where guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
            //                                      && subject.ApproveState == 1
            //                                      && (myRegion.Any() ? (myRegion.Contains(shop.RegionName.ToLower())) : 1 == 1)
            //                                      && (regionList.Any() ? (regionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
            //                                      select subject).ToList();

            
            if (activityList.Any())
            {
                orderSubjectIdList = orderSubjectIdList.Where(s => activityList.Contains(s.SubjectCategoryId ?? 0)).ToList();
                materialOrderSubjectList = materialOrderSubjectList.Where(s => activityList.Contains(s.SubjectCategoryId ?? 0)).ToList();
            }
            if (typeList.Any())
            {
                orderSubjectIdList = orderSubjectIdList.Where(s => typeList.Contains(s.SubjectTypeId ?? 0)).ToList();
                materialOrderSubjectList = materialOrderSubjectList.Where(s => typeList.Contains(s.SubjectTypeId ?? 0)).ToList();
            }
            List<int> subjectIdlist = orderSubjectIdList.Select(s => s.Id).Distinct().ToList();
            List<int> materialSubjectIdlist = materialOrderSubjectList.Select(s => s.Id).Distinct().ToList();
            subjectIdlist.AddRange(materialSubjectIdlist);
            //获取项目信息
            var projectList = (from subject in CurrentContext.DbContext.Subject
                               join subjectType1 in CurrentContext.DbContext.SubjectType
                               on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                               join category1 in CurrentContext.DbContext.ADSubjectCategory
                               on subject.SubjectCategoryId equals category1.Id into categoryTemp
                               from subjectType in typeTemp.DefaultIfEmpty()
                               from category in categoryTemp.DefaultIfEmpty()
                               where subjectIdlist.Contains(subject.Id)
                               select new
                               {
                                   subject.Id,
                                   subject.SubjectCategoryId,
                                   subject.SubjectName,
                                   subject.SubjectTypeId,
                                   subject.ActivityId,
                                   subject.Remark,
                                   subjectType.SubjectTypeName,
                                   category.CategoryName
                               }).OrderBy(s => s.SubjectTypeId).ToList();
            if (projectList.Any())
            {
                StringBuilder subjectJsonStr = new StringBuilder();
                projectList.ForEach(s =>
                {
                    string subjectName = s.SubjectName;
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(s.Remark))
                        subjectName += "(" + s.Remark + ")";
                    subjectJsonStr.Append("{\"Id\":\"" + s.Id + "\",\"SubjectName\":\"" + subjectName + "\"},");
                });
                result = "[" + subjectJsonStr.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetProjects(string beginDate, string endDate)
        {
            string result = string.Empty;
            if (StringHelper.IsDateTime(beginDate) && StringHelper.IsDateTime(endDate))
            {
                DateTime begin = DateTime.Parse(beginDate);
                DateTime end = DateTime.Parse(endDate).AddDays(1);
                bool approved = false;
                bool export = false;
                if (context1.Request.QueryString["approveState"] != null)
                {
                    approved = true;
                }
                if (context1.Request.QueryString["export"] != null)
                {
                    export = true;
                }
                //var list = new SubjectBLL().GetList(s => s.BeginDate >= begin && s.BeginDate < end && s.CustomerId == customerId && s.CompanyId == companyId);
                var list = new SubjectBLL().GetList(s => s.BeginDate >= begin && s.BeginDate < end && s.CustomerId == customerId && (export ? (1 == 1) : companyList.Contains(s.CompanyId ?? 0)) && (s.IsDelete == null || s.IsDelete == false) && (approved ? (s.ApproveState == 1) : 1 == 1));
                if (list.Any())
                {
                    StringBuilder jsonStr = new StringBuilder();
                    list.ForEach(s =>
                    {
                        string subjectName = s.SubjectName;
                        if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(s.Remark))
                            subjectName += "(" + s.Remark + ")";
                        jsonStr.Append("{\"Id\":\"" + s.Id + "\",\"SubjectName\":\"" + subjectName + "\"},");
                    });
                    result = "[" + jsonStr.ToString().TrimEnd(',') + "]";
                }
            }
            return result;
        }

        string GetContents()
        {
            #region
            //StringBuilder json = new StringBuilder();
            //List<string> format = new List<string>();
            //StringBuilder formatsb = new StringBuilder();

            //List<string> install = new List<string>();
            //StringBuilder installsb = new StringBuilder();

            //List<string> CityTier = new List<string>();
            //StringBuilder CityTiersb = new StringBuilder();

            //var shoplist = new ShopBLL().GetList(s => s.CustomerId == customerId);


            //List<int> shopIdList = new List<int>();
            //if (shoplist.Any())
            //{

            //    shoplist.ToList().ForEach(s =>
            //    {
            //        if (!shopIdList.Contains(s.Id))
            //            shopIdList.Add(s.Id);
            //        if (!string.IsNullOrWhiteSpace(s.Format) && !format.Contains(s.Format))
            //        {
            //            format.Add(s.Format);
            //            formatsb.Append(s.Format);
            //            formatsb.Append(',');
            //        }

            //        if (!string.IsNullOrWhiteSpace(s.IsInstall) && !install.Contains(s.IsInstall))
            //        {
            //            install.Add(s.IsInstall);
            //            installsb.Append(s.IsInstall);
            //            installsb.Append(',');
            //        }

            //        if (!string.IsNullOrWhiteSpace(s.CityTier) && !CityTier.Contains(s.CityTier))
            //        {
            //            CityTier.Add(s.CityTier);
            //            CityTiersb.Append(s.CityTier);
            //            CityTiersb.Append(',');
            //        }
            //    });


            //}
            ////获取当月的第一天
            //DateTime now = DateTime.Now;
            //DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            //var orderList = (from order in CurrentContext.DbContext.MergeOriginalOrder
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on order.ShopId equals shop.Id
            //                 where shop.CustomerId == customerId && order.AddDate > firstDayOfMonth
            //                 select order).ToList();
            //List<string> materialSupport = new List<string>();
            //StringBuilder mssb = new StringBuilder();
            //List<string> POSScale = new List<string>();
            //StringBuilder scalesb = new StringBuilder();
            //List<string> gender = new List<string>();
            //StringBuilder gendersb = new StringBuilder();
            //if (orderList.Any())
            //{
            //    orderList.ForEach(s =>
            //    {
            //        if (!string.IsNullOrWhiteSpace(s.MaterialSupport) && !materialSupport.Contains(s.MaterialSupport))
            //        {
            //            materialSupport.Add(s.MaterialSupport);
            //            mssb.Append(s.MaterialSupport);
            //            mssb.Append(',');
            //        }
            //        if (!string.IsNullOrWhiteSpace(s.POSScale) && !POSScale.Contains(s.POSScale))
            //        {
            //            POSScale.Add(s.POSScale);
            //            scalesb.Append(s.POSScale);
            //            scalesb.Append(',');
            //        }
            //        if (!string.IsNullOrWhiteSpace(s.Gender) && !gender.Contains(s.Gender))
            //        {
            //            gender.Add(s.Gender);
            //            gendersb.Append(s.Gender);
            //            gendersb.Append(',');
            //        }
            //    });
            //}
            //json.Append("{\"Format\":\"" + formatsb.ToString().TrimEnd(',') + "\",\"MaterialSupport\":\"" + mssb.ToString().TrimEnd(',') + "\",\"IsInstall\":\"" + installsb.ToString().TrimEnd(',') + "\",\"POSScale\":\"" + scalesb.ToString().TrimEnd(',') + "\",\"Gender\":\"" + gendersb.ToString().TrimEnd(',') + "\",\"CityTier\":\"" + CityTiersb.ToString().TrimEnd(',') + "\"}");
            //return "[" + json.ToString() + "]";
            #endregion

            List<int> subjdectIdList = StringHelper.ToIntList(subjectIds, ',');
            StringBuilder json = new StringBuilder();
            List<string> cityTier = new List<string>();
            StringBuilder cityTiersb = new StringBuilder();
            List<string> format = new List<string>();
            StringBuilder formatsb = new StringBuilder();
            List<string> install = new List<string>();
            StringBuilder installsb = new StringBuilder();
            List<string> materialSupport = new List<string>();
            StringBuilder mssb = new StringBuilder();
            List<string> POSScale = new List<string>();
            StringBuilder scalesb = new StringBuilder();
            List<string> sheet = new List<string>();
            StringBuilder sheetsb = new StringBuilder();
            List<string> gender = new List<string>();
            StringBuilder gendersb = new StringBuilder();
            List<string> ChooseImg = new List<string>();
            StringBuilder ChooseImgsb = new StringBuilder();
            var shoplist = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                            join shop in CurrentContext.DbContext.Shop
                            on merge.ShopId equals shop.Id
                            where subjdectIdList.Contains(merge.SubjectId ?? 0)
                            && (merge.IsDelete == null || merge.IsDelete == false)
                            select new
                            {
                                merge,
                                shop

                            }).OrderBy(s => s.shop.CityTier).ToList();
            List<int> shopIdList = new List<int>();
            if (shoplist.Any())
            {

                shoplist.ToList().ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s.shop.CityTier) && !cityTier.Contains(s.shop.CityTier))
                    {
                        cityTier.Add(s.shop.CityTier);
                        cityTiersb.Append(s.shop.CityTier);
                        cityTiersb.Append(',');
                    }
                    if (!string.IsNullOrWhiteSpace(s.shop.Format) && !format.Contains(s.shop.Format))
                    {
                        format.Add(s.shop.Format);
                        formatsb.Append(s.shop.Format);
                        formatsb.Append(',');
                    }
                    if (!string.IsNullOrWhiteSpace(s.merge.MaterialSupport) && !materialSupport.Contains(s.merge.MaterialSupport))
                    {
                        materialSupport.Add(s.merge.MaterialSupport);
                        mssb.Append(s.merge.MaterialSupport);
                        mssb.Append(',');
                    }
                    if (!string.IsNullOrWhiteSpace(s.shop.IsInstall) && !install.Contains(s.shop.IsInstall))
                    {
                        install.Add(s.shop.IsInstall);
                        installsb.Append(s.shop.IsInstall);
                        installsb.Append(',');
                    }
                    if (!string.IsNullOrWhiteSpace(s.merge.POSScale) && !POSScale.Contains(s.merge.POSScale))
                    {
                        POSScale.Add(s.merge.POSScale);
                        scalesb.Append(s.merge.POSScale);
                        scalesb.Append(',');
                    }

                    if (!string.IsNullOrWhiteSpace(s.merge.Gender) && !gender.Contains(s.merge.Gender))
                    {
                        gender.Add(s.merge.Gender);
                        gendersb.Append(s.merge.Gender);
                        gendersb.Append(',');
                    }
                    if (!string.IsNullOrWhiteSpace(s.merge.Sheet) && !sheet.Contains(s.merge.Sheet))
                    {
                        sheet.Add(s.merge.Sheet);
                        sheetsb.Append(s.merge.Sheet);
                        sheetsb.Append(',');
                    }
                    if (!string.IsNullOrWhiteSpace(s.merge.ChooseImg) && !ChooseImg.Contains(s.merge.ChooseImg))
                    {
                        ChooseImg.Add(s.merge.ChooseImg);
                        ChooseImgsb.Append(s.merge.ChooseImg);
                        ChooseImgsb.Append(',');
                    }
                });


            }

            json.Append("{\"CityTier\":\"" + cityTiersb.ToString().TrimEnd(',') + "\",\"Format\":\"" + formatsb.ToString().TrimEnd(',') + "\",\"MaterialSupport\":\"" + mssb.ToString().TrimEnd(',') + "\",\"IsInstall\":\"" + installsb.ToString().TrimEnd(',') + "\",\"POSScale\":\"" + scalesb.ToString().TrimEnd(',') + "\",\"Sheet\":\"" + sheetsb.ToString().TrimEnd(',') + "\",\"Gender\":\"" + gendersb.ToString().TrimEnd(',') + "\",\"ChooseImg\":\"" + ChooseImgsb.ToString().TrimEnd(',') + "\"}");
            return "[" + json.ToString() + "]";
        }

        /// <summary>
        /// 获取位置信息
        /// </summary>
        /// <returns></returns>
        string GetPositions()
        {
            var list = new PositionBLL().GetList(s => 1 == 1);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"PositionName\":\"" + s.PositionName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }


        string AddPlan(string optype, string jsonString)
        {
            string result = "提交失败！";
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(jsonString);
                        ProvinceInOrderPlanBLL planProvinceBll = new ProvinceInOrderPlanBLL();
                        ProvinceInOrderPlan planProvinceModel;
                        CityInOrderPlanBLL planCityBll = new CityInOrderPlanBLL();
                        CityInOrderPlan planCityModel;
                        if (planModel != null)
                        {
                            if (optype == "add")
                            {
                                planModel.AddDate = DateTime.Now;
                                planModel.AddUserId = new BasePage().CurrentUser.UserId;
                                planModel.PlanType = 2;
                                orderPlanBll.Add(planModel);
                                if (planModel.CheckOrderPlanDetail.Any())
                                {
                                    string provinces = string.Empty;
                                    string cities = string.Empty;
                                    planModel.CheckOrderPlanDetail.ForEach(s =>
                                    {
                                        CheckOrderPlanDetail checkModel = s;
                                        if (checkModel != null)
                                        {
                                            if (!string.IsNullOrWhiteSpace(checkModel.ProvinceId))
                                            {
                                                provinces = checkModel.ProvinceId;
                                                checkModel.ProvinceId = null;
                                            }
                                            if (!string.IsNullOrWhiteSpace(checkModel.CityId))
                                            {
                                                cities = checkModel.CityId;
                                                checkModel.CityId = null;
                                            }
                                            checkModel.PlanId = planModel.Id;
                                            checkOrderDetailBll.Add(checkModel);
                                            //保存省份
                                            StringHelper.ToStringList(provinces, ',').ForEach(p =>
                                            {
                                                planProvinceModel = new ProvinceInOrderPlan() { ProvinceName = p, PlanId = planModel.Id, PlanType = 2 };
                                                planProvinceBll.Add(planProvinceModel);
                                            });
                                            //保存城市
                                            StringHelper.ToStringList(cities, ',').ForEach(c =>
                                            {
                                                planCityModel = new CityInOrderPlan() { CityName = c, PlanId = checkModel.Id, PlanType = 2 };
                                                planCityBll.Add(planCityModel);
                                            });
                                        }
                                    });
                                }
                                result = "ok";
                            }
                            else
                            {
                                if (planModel.Id > 0)
                                {
                                    OrderPlan planModel1 = orderPlanBll.GetModel(planModel.Id);
                                    if (planModel1 != null)
                                    {
                                        planModel.AddDate = planModel1.AddDate;
                                        planModel.AddUserId = planModel1.AddUserId;
                                        planModel.PlanType = planModel1.PlanType;
                                        orderPlanBll.Update(planModel);
                                        planProvinceBll.Delete(p => p.PlanId == planModel.Id && p.PlanType == 2);
                                        //先删除原来的城市
                                        planCityBll.Delete(c => c.PlanId == planModel.Id && c.PlanType == 2);
                                        //删除原来的内容
                                        checkOrderDetailBll.Delete(s => s.PlanId == planModel.Id);

                                        if (planModel.CheckOrderPlanDetail.Any())
                                        {
                                            string provinces = string.Empty;
                                            string cities = string.Empty;
                                            //添加内容
                                            planModel.CheckOrderPlanDetail.ForEach(s =>
                                            {
                                                CheckOrderPlanDetail checkModel = s;
                                                if (checkModel != null)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(checkModel.ProvinceId))
                                                    {
                                                        provinces = checkModel.ProvinceId;
                                                        checkModel.ProvinceId = null;
                                                    }
                                                    if (!string.IsNullOrWhiteSpace(checkModel.CityId))
                                                    {
                                                        cities = checkModel.CityId;
                                                        checkModel.CityId = null;
                                                    }
                                                    checkModel.PlanId = planModel.Id;
                                                    checkOrderDetailBll.Add(checkModel);
                                                    //保存省份
                                                    StringHelper.ToStringList(provinces, ',').ForEach(p =>
                                                    {
                                                        planProvinceModel = new ProvinceInOrderPlan() { ProvinceName = p, PlanId = planModel.Id, PlanType = 2 };
                                                        planProvinceBll.Add(planProvinceModel);
                                                    });
                                                    //保存城市
                                                    StringHelper.ToStringList(cities, ',').ForEach(c =>
                                                    {
                                                        planCityModel = new CityInOrderPlan() { CityName = c, PlanId = checkModel.Id, PlanType = 2 };
                                                        planCityBll.Add(planCityModel);
                                                    });
                                                }
                                            });
                                        }
                                    }
                                    result = "ok";
                                }
                                else
                                {
                                    result = "更新失败！";
                                }
                            }

                        }
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

        string GetPlanList(string begin1, string end1, string ids = null)
        {
            List<int> idList = new List<int>();
            if (ids != null)
            {
                idList = StringHelper.ToIntList(ids, ',');
            }
            DateTime now = DateTime.Now;
            DateTime firstday = new DateTime(now.Year, now.Month, 1);
            DateTime lastday = firstday.AddMonths(1);
            //var list = orderPlanBll.GetList(s => idList.Any() ? idList.Contains(s.Id) : (s.CustomerId == customerId && s.AddDate >= firstday && s.AddDate < lastday && s.PlanType == 2));
            var list = (from plan in CurrentContext.DbContext.OrderPlan
                        join customer in CurrentContext.DbContext.Customer
                        on plan.CustomerId equals customer.Id
                        //where idList.Any() ? idList.Contains(plan.Id) : (plan.CustomerId == customerId && plan.AddDate >= firstday && plan.AddDate < lastday && plan.PlanType == 2)
                        where idList.Any() ? idList.Contains(plan.Id) : (plan.CustomerId == customerId && plan.PlanType == 2 && (plan.AddDate >= firstday && plan.AddDate < lastday))
                        select new
                        {
                            plan,
                            customer.CustomerName
                        }).ToList();

            if (!string.IsNullOrWhiteSpace(begin1))
            {
                DateTime beginDate = DateTime.Parse(begin1);
                list = list.Where(s => s.plan.AddDate >= beginDate).ToList();
            }
            if (!string.IsNullOrWhiteSpace(end1))
            {
                DateTime endDate = DateTime.Parse(end1).AddDays(1);
                list = list.Where(s => s.plan.AddDate < endDate).ToList();
            }
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    //string addDate = s.plan.AddDate != null ? DateTime.Parse(s.plan.AddDate.ToString()).ToShortDateString() : "";
                    string begin = s.plan.BeginDate != null ? DateTime.Parse(s.plan.BeginDate.ToString()).ToShortDateString() : "";
                    string end = s.plan.EndDate != null ? DateTime.Parse(s.plan.EndDate.ToString()).ToShortDateString() : "";
                    json.Append("{\"Id\":\"" + s.plan.Id + "\",\"CustomerId\":\"" + s.plan.CustomerId + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"AddDate\":\"" + s.plan.AddDate + "\",\"BeginDate\":\"" + begin + "\",\"EndDate\":\"" + end + "\",\"ProjectId\":\"" + s.plan.ProjectId + "\",\"ProjectName\":\"" + s.plan.ProjectName + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

        string GetDetailList()
        {
            var list = checkOrderDetailBll.GetList(s => s.PlanId == planId).OrderBy(s => s.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    string ProvinceName = string.Empty;
                    string CityName = string.Empty;
                    ProvinceName = GetProvinceNames(s.Id);
                    CityName = GetCityNames(s.Id);
                    json.Append("{\"Id\":\"" + s.Id + "\",\"RegionNames\":\"" + s.RegionNames + "\",\"ProvinceName\":\"" + ProvinceName + "\",\"CityName\":\"" + CityName + "\",\"CityTier\":\"" + s.CityTier + "\",\"IsInstall\":\"" + s.IsInstall + "\",\"PositionId\":\"" + s.PositionId + "\",\"PositionName\":\"" + s.PositionName + "\",\"Format\":\"" + s.Format + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"POSScale\":\"" + s.POSScale + "\",\"Gender\":\"" + s.Gender + "\",\"ChooseImg\":\"" + s.ChooseImg + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

        string DeletePlan(string ids = null)
        {
            if (ids == null)
            {
                deletePlan(planId);
                return "ok";
            }
            else
            {
                List<int> planIdList = StringHelper.ToIntList(ids, ',');
                planIdList.ForEach(id =>
                {
                    deletePlan(id);
                });
                return "ok";
            }
        }

        string DeleteDetail(int detailId)
        {
            CheckOrderPlanDetail model = checkOrderDetailBll.GetModel(detailId);
            if (model != null)
            {
                checkOrderDetailBll.Delete(model);
                return "ok";
            }
            return "error";
        }

        CityInOrderPlanBLL planCityBll = new CityInOrderPlanBLL();
        //CityInOrderPlan planCityModel;
        ProvinceInOrderPlanBLL planProvinceBll = new ProvinceInOrderPlanBLL();
        //ProvinceInOrderPlan planProvinceModel;
        string GetProvinceNames(int planId)
        {
            var list = planProvinceBll.GetList(p => p.PlanId == planId && p.PlanType == 2);
            if (list.Any())
            {
                StringBuilder province = new StringBuilder();
                list.ForEach(s =>
                {
                    province.Append(s.ProvinceName);
                    province.Append(',');

                });

                return province.ToString().TrimEnd(',');
            }
            return "";
        }

        string GetCityNames(int detailId)
        {
            var list = planCityBll.GetList(p => p.PlanId == planId && p.PlanType == 2);
            if (list.Any())
            {
                StringBuilder city = new StringBuilder();
                list.ForEach(s =>
                {
                    city.Append(s.CityName);
                    city.Append(',');

                });

                return city.ToString().TrimEnd(',');
            }
            return "";
        }

        /// <summary>
        /// 执行核单
        /// </summary>
        /// <param name="planIds"></param>
        /// <param name="subjectIds"></param>
        /// <returns></returns>
        string GoCheckOrder(string planIds, string subjectIds)
        {
            string result = "-1";
            List<int> planIdList = StringHelper.ToIntList(planIds, ',');
            List<int> subjectIdList = StringHelper.ToIntList(subjectIds, ',');
            //订单
            var orderList = (from list in CurrentContext.DbContext.MergeOriginalOrder
                             join shop in CurrentContext.DbContext.Shop
                             on list.ShopId equals shop.Id
                             where subjectIdList.Contains(list.SubjectId ?? 0)
                             && (list.IsDelete == null || list.IsDelete == false)
                             select new
                             {
                                 list,
                                 shop

                             }).ToList();

            //方案
            var planList = new OrderPlanBLL().GetList(s => s.CustomerId == customerId && s.PlanType == 2 && planIdList.Contains(s.Id));
            //已完成核单的订单ID
            //List<int> finishCheckId = new List<int>();
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {

                    if (planList.Any() && orderList.Any())
                    {
                        ProvinceInOrderPlanBLL planProvinceBll = new ProvinceInOrderPlanBLL();
                        CityInOrderPlanBLL planCityBll = new CityInOrderPlanBLL();
                        //核单总表
                        CheckOrderBLL checkOrderBll = new CheckOrderBLL();
                        Models.CheckOrder checkOrderModel;
                        //核单明细表
                        CheckOrderResultBLL checkResultBll = new CheckOrderResultBLL();
                        CheckOrderResult checkResultModel;
                        //添加核单总表
                        checkOrderModel = new Models.CheckOrder();
                        if (context1.Request.QueryString["begin"] != null)
                            checkOrderModel.Begindate = DateTime.Parse(context1.Request.QueryString["begin"]);
                        if (context1.Request.QueryString["end"] != null)
                            checkOrderModel.EndDate = DateTime.Parse(context1.Request.QueryString["end"]);
                        checkOrderModel.AddDate = DateTime.Now;
                        checkOrderModel.AddUserId = new BasePage().CurrentUser.UserId;
                        checkOrderModel.CompanyId = new BasePage().CurrentUser.CompanyId;
                        checkOrderModel.CustomerId = customerId;
                        checkOrderModel.PlanIds = planIds;
                        checkOrderModel.SubjectId = subjectIds;
                        checkOrderModel.IsDelete = false;
                        checkOrderBll.Add(checkOrderModel);
                        result = checkOrderModel.Id.ToString();
                        planList.ForEach(p =>
                        {
                            List<int> subjectList = StringHelper.ToIntList(p.ProjectId, ',');
                            var planDetailList = checkOrderDetailBll.GetList(d => d.PlanId == p.Id);
                            //循环项目(活动)
                            foreach (int subjectid in subjectList)
                            {

                                if (planDetailList.Any())
                                {
                                    //循环方案内容
                                    planDetailList.ForEach(d =>
                                    {
                                        //List<int> positionList = StringHelper.ToIntList(d.PositionId, ',');
                                        List<string> positionNameList = StringHelper.ToStringList(d.PositionId, ',');
                                        List<string> regionList = new List<string>() { "0" };
                                        List<string> provinceList = new List<string>() { "0" };
                                        List<string> cityList = new List<string>() { "0" };
                                        List<string> CityTierList = new List<string>() { "0" };
                                        List<string> formatList = new List<string>() { "0" };
                                        List<string> materialSupportList = new List<string>() { "0" };
                                        List<string> POSScaleList = new List<string>() { "0" };
                                        List<string> GenderList = new List<string>() { "0" };
                                        List<string> ChooseImgList = new List<string>() { "0" };



                                        //区域
                                        if (!string.IsNullOrWhiteSpace(d.RegionNames))
                                        {
                                            regionList = StringHelper.ToStringList(d.RegionNames, ',');
                                        }
                                        //省份
                                        var plist = planProvinceBll.GetList(c => c.PlanId == d.Id && c.PlanType == 2).Select(c => c.ProvinceName);
                                        if (plist.Any())
                                            provinceList = plist.ToList();
                                        //城市
                                        var cList = planCityBll.GetList(c => c.PlanId == d.Id && c.PlanType == 2).Select(c => c.CityName);
                                        if (cList.Any())
                                            cityList = cList.ToList();


                                        //城市级别
                                        if (!string.IsNullOrWhiteSpace(d.CityTier))
                                        {
                                            CityTierList = StringHelper.ToStringList(d.CityTier, ',');
                                        }

                                        //店铺类型
                                        if (!string.IsNullOrWhiteSpace(d.Format))
                                        {
                                            formatList = StringHelper.ToStringList(d.Format, ',');
                                        }

                                        //物料支持
                                        if (!string.IsNullOrWhiteSpace(d.MaterialSupport))
                                        {
                                            materialSupportList = StringHelper.ToStringList(d.MaterialSupport, ',');
                                        }

                                        //店铺规模
                                        if (!string.IsNullOrWhiteSpace(d.POSScale))
                                        {
                                            POSScaleList = StringHelper.ToStringList(d.POSScale, ',');
                                        }

                                        //性别
                                        if (!string.IsNullOrWhiteSpace(d.Gender))
                                        {
                                            GenderList = StringHelper.ToStringList(d.Gender, ',');
                                        }
                                        //性别
                                        if (!string.IsNullOrWhiteSpace(d.ChooseImg))
                                        {
                                            ChooseImgList = StringHelper.ToStringList(d.ChooseImg, ',');
                                        }
                                        //按照planDetail里的条件过滤基础数据
                                        //按项目
                                        var newOrderList = orderList.Where(f => f.list.SubjectId == subjectid);
                                        if (regionList.Any())
                                        {
                                            regionList.ForEach(r =>
                                            {
                                                var newOrderList1 = newOrderList.Where(l => r != "0" ? l.shop.RegionName == r : 1 == 1).ToList();
                                                provinceList.ForEach(pr =>
                                                {
                                                    var newOrderList2 = newOrderList1.Where(l => pr != "0" ? l.shop.ProvinceName == pr : 1 == 1).ToList();
                                                    cityList.ForEach(ci =>
                                                    {
                                                        var newOrderList3 = newOrderList2.Where(l => ci != "0" ? l.shop.CityName == ci : 1 == 1).ToList();
                                                        CityTierList.ForEach(ct =>
                                                        {
                                                            var newOrderList4 = newOrderList3.Where(l => ct != "0" ? l.shop.CityTier == ct : 1 == 1).ToList();
                                                            formatList.ForEach(fo =>
                                                            {
                                                                var newOrderList5 = newOrderList4.Where(l => fo != "0" ? l.shop.Format == fo : 1 == 1).ToList();
                                                                materialSupportList.ForEach(ma =>
                                                                {
                                                                    var newOrderList6 = newOrderList5.Where(l => ma != "0" ? l.list.MaterialSupport == ma : 1 == 1).ToList();
                                                                    POSScaleList.ForEach(po =>
                                                                    {
                                                                        var newOrderList7 = newOrderList6.Where(l => po != "0" ? l.list.POSScale == po : 1 == 1).ToList();
                                                                        GenderList.ForEach(gn =>
                                                                        {
                                                                            var newOrderList8 = newOrderList7.Where(l => gn != "0" ? l.list.Gender == gn : 1 == 1).ToList();
                                                                            ChooseImgList.ForEach(ch =>
                                                                            {
                                                                                var newOrderList9 = newOrderList8.Where(l => ch != "0" ? l.list.ChooseImg == ch : 1 == 1).ToList();
                                                                                if (newOrderList9.Any())
                                                                                {

                                                                                    List<int> shopIdList = newOrderList9.Select(n => n.shop.Id).ToList();

                                                                                    //pop基础数据
                                                                                    var popBasicList = from pop in CurrentContext.DbContext.POP
                                                                                                       where shopIdList.Contains(pop.ShopId ?? 0) && pop.Gender == gn
                                                                                                       select pop;

                                                                                    //循环位置,合计每个位置的数量
                                                                                    foreach (string position in positionNameList)
                                                                                    {

                                                                                        int basicCount = popBasicList.Where(b => b.Sheet == position).Sum(b => b.Quantity) ?? 0;
                                                                                        int orderCount = newOrderList9.Where(b => b.list.Sheet == position).Sum(b => b.list.Quantity) ?? 0;
                                                                                        if (basicCount == 0)
                                                                                        {
                                                                                            //如果在pop表没有找到，就到店铺器架类型表（ShopMachineFrame）找
                                                                                            basicCount = (from shopMF in CurrentContext.DbContext.ShopMachineFrame
                                                                                                          where shopIdList.Contains(shopMF.ShopId ?? 0) && shopMF.PositionName == position
                                                                                                          && shopMF.Gender == gn
                                                                                                          select shopMF.Count).Sum(s => s) ?? 0;
                                                                                        }
                                                                                        checkResultModel = new CheckOrderResult();
                                                                                        checkResultModel.BasicPositionCount = basicCount;
                                                                                        checkResultModel.CheckOrderId = checkOrderModel.Id;
                                                                                        checkResultModel.CityId = ci;
                                                                                        checkResultModel.CityTier = ct;
                                                                                        checkResultModel.Format = fo;
                                                                                        checkResultModel.Gender = gn;
                                                                                        checkResultModel.IsInstall = d.IsInstall;
                                                                                        checkResultModel.MaterialSupport = ma;
                                                                                        checkResultModel.OrderPositionCount = orderCount;
                                                                                        checkResultModel.PlanDetailId = d.Id;
                                                                                        //checkResultModel.PositionId = positionId;
                                                                                        checkResultModel.PositionName = position;
                                                                                        checkResultModel.POSScale = po;
                                                                                        checkResultModel.ProvinceId = pr;
                                                                                        checkResultModel.RegionId = r;
                                                                                        checkResultModel.RegionNames = r;
                                                                                        checkResultModel.SubjectId = subjectid;
                                                                                        checkResultBll.Add(checkResultModel);

                                                                                    }
                                                                                }
                                                                            });

                                                                        });
                                                                    });
                                                                });
                                                            });
                                                        });
                                                    });
                                                });
                                            });
                                        }




                                        #region
                                        ////区域
                                        //if (!string.IsNullOrWhiteSpace(d.RegionId))
                                        //{

                                        //    newOrderList = newOrderList.Where(l => regionList.Contains(l.shop.RegionId ?? 0)).ToList();

                                        //}
                                        ////省份
                                        //if (d.ProvinceId != "-1")
                                        //{

                                        //    if (provinceList.Any())
                                        //       newOrderList = newOrderList.Where(l => provinceList.Contains(l.shop.ProvinceName)).ToList();
                                        //}
                                        ////城市
                                        //if (d.CityId != "-1")
                                        //{
                                        //    cityList = planCityBll.GetList(c => c.PlanId == d.Id && c.PlanType == 2).Select(c => c.CityName).ToList();
                                        //    if (cityList.Any())
                                        //       newOrderList = newOrderList.Where(l => cityList.Contains(l.shop.CityName)).ToList();
                                        //}
                                        ////城市级别
                                        //if (!string.IsNullOrWhiteSpace(d.CityTier))
                                        //{
                                        //    CityTierList = StringHelper.ToStringList(d.CityTier, ',');
                                        //    newOrderList = newOrderList.Where(l => CityTierList.Contains(l.shop.CityTier)).ToList();
                                        //}
                                        ////是否安装
                                        //if (!string.IsNullOrWhiteSpace(d.IsInstall))
                                        //{
                                        //    List<string> installList = StringHelper.ToStringList(d.IsInstall, ',');
                                        //    newOrderList = newOrderList.Where(l => installList.Contains(l.shop.IsInstall)).ToList();
                                        //}
                                        ////店铺类型
                                        //if (!string.IsNullOrWhiteSpace(d.Format))
                                        //{
                                        //    formatList = StringHelper.ToStringList(d.Format, ',');
                                        //    newOrderList = newOrderList.Where(l => formatList.Contains(l.shop.Format)).ToList();
                                        //}
                                        ////物料支持
                                        //if (!string.IsNullOrWhiteSpace(d.MaterialSupport))
                                        //{
                                        //    materialSupportList = StringHelper.ToStringList(d.MaterialSupport, ',');
                                        //    newOrderList = newOrderList.Where(l => materialSupportList.Contains(l.list.MaterialSupport)).ToList();
                                        //}
                                        ////店铺规模
                                        //if (!string.IsNullOrWhiteSpace(d.POSScale))
                                        //{
                                        //    POSScaleList = StringHelper.ToStringList(d.POSScale, ',');
                                        //    newOrderList = newOrderList.Where(l => POSScaleList.Contains(l.list.POSScale)).ToList();
                                        //}
                                        ////性别
                                        //if (!string.IsNullOrWhiteSpace(d.Gender))
                                        //{
                                        //    GenderList = StringHelper.ToStringList(d.Gender, ',');
                                        //    newOrderList = newOrderList.Where(l => GenderList.Contains(l.list.Gender)).ToList();
                                        //}
                                        #endregion


                                    });
                                }
                            }

                        });
                    }
                    tran.Complete();
                    //result= "ok";
                }
                catch (Exception ex)
                {
                    //return ex.Message;
                    result = "-1";
                }

                return result;
            }
        }

        void deletePlan(int id)
        {
            OrderPlan planModel = orderPlanBll.GetModel(id);
            if (planModel != null)
            {
                if (orderPlanBll.Delete(planModel))
                {
                    checkOrderDetailBll.Delete(s => s.PlanId == id);
                }

            }
        }

        string GetMaterials(string beginDate, string endDate)
        {
            OrderMaterialItemBLL materialItemBll = new OrderMaterialItemBLL();
            string result = string.Empty;
            if (StringHelper.IsDateTime(beginDate) && StringHelper.IsDateTime(endDate))
            {
                DateTime begin = DateTime.Parse(beginDate);
                DateTime end = DateTime.Parse(endDate).AddDays(1);

                //var list = materialItemBll.GetList(s => s.BeginDate >= begin && s.BeginDate < end);
                var list = (from mitem in CurrentContext.DbContext.OrderMaterialItem
                            join subject in CurrentContext.DbContext.Subject
                            on mitem.SubjectId equals subject.Id
                            where subject.BeginDate >= begin && subject.BeginDate < end
                            && (subject.IsDelete == null || subject.IsDelete == false) && subject.ApproveState == 1
                            select new
                            {
                                mitem.ItemId,
                                subject.SubjectName
                            }).ToList();
                if (list.Any())
                {
                    StringBuilder jsonStr = new StringBuilder();
                    list.ForEach(s =>
                    {
                        //string name = DateTime.Parse(s.BeginDate.ToString()).ToShortDateString() + "至" + DateTime.Parse(s.EndDate.ToString()).ToShortDateString();
                        jsonStr.Append("{\"Id\":\"" + s.ItemId + "\",\"Name\":\"" + s.SubjectName + "\"},");
                    });
                    result = "[" + jsonStr.ToString().TrimEnd(',') + "]";
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
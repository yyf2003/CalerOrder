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
using System.Configuration;
using System.Web.SessionState;

namespace WebApp.OutsourcingOrder.handler
{
    /// <summary>
    /// OrderList 的摘要说明
    /// </summary>
    public class OrderList : IHttpHandler, IRequiresSessionState
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
            else if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            switch (type)
            {
                case "getOutsource":
                    result = GetOutsourceList();
                    break;
                case "getOrderOutsource":
                    result = GetOutsourceFromOrder();
                    break;
                case "getGuidanceList":
                    result = GetGuidanceList();
                    break;
                case "getRegion":
                    result = GetRegion();
                    break;
                case "getSubjectCategory":
                    result = GetSubjectCategory();
                    break;
                case "getOrderList":
                    result = GetOrderList();
                    break;
                case "getMaterialCategory":
                    result = GetMaterialCategory();
                    break;
                case "getProvince":
                    result = GetProvinceList();
                    break;
                case "getCity":
                    result = GetCityList();
                    break;
                case "getSubjectList":
                    result = GetSubjectList();
                    break;
                case "getOrder":
                    result = GetModel();
                    break;
                case "edit":
                    result = Edit();
                    break;
                case "delete":
                    result = DeleteOrder();
                    break;
                case "recover":
                    result = RecoverOrder();
                    break;
                case "changeOutsource":
                    result = ChangeOutsource();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        List<int> guidanceIdList = new List<int>();
        void ReLoadSession()
        {

            if (context1.Session["ChangeOutsourceGuidanceIdList="] != null)
            {
                guidanceIdList = context1.Session["ChangeOutsourceGuidanceIdList="] as List<int>;
            }
            
            List<OutsourceOrderDetail> orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                                     from gid in guidanceIdList
                                                     where order.GuidanceId == gid
                                                     select order).ToList();
            if (orderList0.Any())
            {
                List<Subject> subjectList = (from order in orderList0
                                             join subject in CurrentContext.DbContext.Subject
                                             on order.SubjectId equals subject.Id
                                             where subject.IsDelete == null || subject.IsDelete == false
                                             select subject).Distinct().ToList();
                context1.Session["ChangeOutsourceOrderList="] = orderList0;
                context1.Session["ChangeOutsourceSubjectList="] = subjectList;
            }
            else
            {
                context1.Session["ChangeOutsourceOrderList="] = null;
                context1.Session["ChangeOutsourceSubjectList="] = null;
            }
        }


        string GetGuidanceList()
        {
            context1.Session["ChangeOutsourceOrderList="] = null;
            context1.Session["ChangeOutsourceSubjectList="] = null;
            context1.Session["ChangeOutsourceGuidanceIdList="] = null;
            string result = string.Empty;
            string outsourceId = string.Empty;
            int customerId = 0;
            string guidanceMonth = string.Empty;
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            if (context1.Request.QueryString["guidanceMonth"] != null)
            {
                guidanceMonth = context1.Request.QueryString["guidanceMonth"];
            }
            DateTime date = DateTime.Now;
            int year = date.Year;
            int month = date.Month;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date0 = DateTime.Parse(guidanceMonth);
                year = date0.Year;
                month = date0.Month;

            }
            var list = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on order.GuidanceId equals guidance.ItemId
                        where guidance.CustomerId == customerId
                        && (outsourceList.Any() ? outsourceList.Contains(order.OutsourceId ?? 0) : 1 == 1)
                        && guidance.GuidanceYear == year
                        && guidance.GuidanceMonth == month
                        && (order.IsDelete == null || order.IsDelete == false)
                        select guidance).Distinct().ToList();

            //list = list.Where(s => s.GuidanceYear == year && s.GuidanceMonth == month).OrderBy(s=>s.ItemName).ToList();

            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"GuidanceId\":\"" + s.ItemId + "\",\"GuidanceName\":\"" + s.ItemName + "\"},");
                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetRegion()
        {

            context1.Session["ChangeOutsourceOrderList="] = null;
            context1.Session["ChangeOutsourceSubjectList="] = null;
            context1.Session["ChangeOutsourceGuidanceIdList="] = null;
            List<int> outsourceIdList = new List<int>();
            List<int> guidanceIdList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                string outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                string guidanceIds = context1.Request.QueryString["guidanceIds"];
                if (!string.IsNullOrWhiteSpace(guidanceIds))
                {
                    guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
                    context1.Session["ChangeOutsourceGuidanceIdList="] = guidanceIdList;
                }
            }

            List<OutsourceOrderDetail> orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                                     from gid in guidanceIdList
                                                     where order.GuidanceId == gid
                                                         //&& (order.IsDelete == null || order.IsDelete == false)
                                                     && (outsourceIdList.Any() ? outsourceIdList.Contains(order.OutsourceId ?? 0) : 1 == 1)
                                                     select order).ToList();
            if (orderList0.Any())
            {
                List<Subject> subjectList = (from order in orderList0
                                             join subject in CurrentContext.DbContext.Subject
                                             on order.SubjectId equals subject.Id
                                             where subject.IsDelete == null || subject.IsDelete == false
                                             select subject).Distinct().ToList();
                context1.Session["ChangeOutsourceOrderList="] = orderList0;
                context1.Session["ChangeOutsourceSubjectList="] = subjectList;

                List<string> regionList = orderList0.Select(s => s.Region ?? "").Distinct().ToList();
                bool hasEmpty = regionList.Contains("");
                regionList.Remove("");
                StringBuilder json = new StringBuilder();
                regionList.ForEach(s =>
                {
                    json.Append("{\"RegionName\":\"" + s + "\"},");
                });
                if (hasEmpty)
                {
                    json.Append("{\"RegionName\":\"空\"},");
                }
                if (json.Length > 0)
                {
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                else
                    return "";
            }
            else
                return "";
        }

        string GetSubjectCategory()
        {
            List<int> outsourceIdList = new List<int>();
            //List<int> guidanceIdList = new List<int>();
            List<string> regionList = new List<string>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                string outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
                }
            }

            if (context1.Request.QueryString["region"] != null)
            {
                string region = context1.Request.QueryString["region"];
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                }
            }
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<Subject> subjectList = new List<Subject>();
            if (context1.Session["ChangeOutsourceOrderList="] != null)
            {
                orderList = context1.Session["ChangeOutsourceOrderList="] as List<OutsourceOrderDetail>;
            }
            if (context1.Session["ChangeOutsourceSubjectList="] != null)
            {
                subjectList = context1.Session["ChangeOutsourceSubjectList="] as List<Subject>;
            }
            var slist = (from order in orderList
                         join subject in subjectList
                         on order.SubjectId equals subject.Id
                         where (outsourceIdList.Any() ? (outsourceIdList.Contains(order.OutsourceId ?? 0)) : 1 == 1)
                         //&& (regionList.Any() ? (regionList.Contains(order.Region)) : 1 == 1)
                         select new { order, subject }).ToList();
            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        slist = slist.Where(s => regionList.Contains(s.order.Region) || s.order.Region == null || s.order.Region == "").ToList();
                    }
                    else
                    {
                        slist = slist.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                    }
                }
                else
                    slist = slist.Where(s => regionList.Contains(s.order.Region)).ToList();
            }
            var slist0 = slist.Select(s => s.subject).Distinct().ToList();
            var categoryList = (from subject in slist0
                                join category1 in CurrentContext.DbContext.ADSubjectCategory
                                on subject.SubjectCategoryId equals category1.Id into categoryTemp
                                from category in categoryTemp.DefaultIfEmpty()
                                select category).Distinct().ToList();
            if (categoryList.Any())
            {
                StringBuilder json = new StringBuilder();
                bool hasEmpty = false;
                categoryList.ForEach(s =>
                {
                    if (s != null)
                    {
                        json.Append("{\"CategoryId\":\"" + s.Id + "\",\"CategoryName\":\"" + s.CategoryName + "\"},");
                    }
                });
                if (hasEmpty)
                {
                    json.Append("{\"CategoryId\":\"0\",\"CategoryName\":\"空\"},");
                }
                if (json.Length > 0)
                {
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                else
                    return "";
            }
            else
                return "";
        }

        string GetSubjectList()
        {

            List<int> outsourceIdList = new List<int>();
            List<string> regionList = new List<string>();
            List<int> categoryIdList = new List<int>();
            List<int> guidanceIdList = new List<int>();
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                string guidanceIds = context1.Request.QueryString["guidanceIds"];
                if (!string.IsNullOrWhiteSpace(guidanceIds))
                {
                    guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
                    context1.Session["ChangeOutsourceGuidanceIdList="] = guidanceIdList;
                }
            }
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                string outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["region"] != null)
            {
                string region = context1.Request.QueryString["region"];
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
            }
            if (context1.Request.QueryString["category"] != null)
            {
                string category = context1.Request.QueryString["category"];
                if (!string.IsNullOrWhiteSpace(category))
                {
                    categoryIdList = StringHelper.ToIntList(category, ',');
                }
            }
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<Subject> subjectList = new List<Subject>();
            //if (context1.Session["ChangeOutsourceOrderList="]== null)
            ReLoadSession();
            if (context1.Session["ChangeOutsourceOrderList="] != null)
            {
                orderList = context1.Session["ChangeOutsourceOrderList="] as List<OutsourceOrderDetail>;
            }
            
            if (context1.Session["ChangeOutsourceSubjectList="] != null)
            {
                subjectList = context1.Session["ChangeOutsourceSubjectList="] as List<Subject>;
            }
            if (context1.Session["ChangeOutsourceGuidanceIdList="] != null)
            {
                guidanceIdList = context1.Session["ChangeOutsourceGuidanceIdList="] as List<int>;
            }
            var sList = (from order in orderList
                         join subject in subjectList
                         on order.SubjectId equals subject.Id
                         where (outsourceIdList.Any() ? (outsourceIdList.Contains(order.OutsourceId ?? 0)) : 1 == 1)
                             //&& (regionList.Any() ? (order.Region != null && (regionList.Contains(order.Region.ToLower()))) : 1 == 1)
                         && (categoryIdList.Any() ? (categoryIdList.Contains(subject.SubjectCategoryId ?? 0)) : 1 == 1)
                         select new { order, subject }).ToList();

            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        sList = sList.Where(s => (s.order.Region != null && regionList.Contains(s.order.Region.ToLower())) || s.order.Region == null || s.order.Region == "").ToList();
                    }
                    else
                    {
                        sList = sList.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                    }
                }
                else
                    sList = sList.Where(s => s.order.Region != null && regionList.Contains(s.order.Region.ToLower())).ToList();
            }
            if (sList.Any())
            {
                List<int> allSubjectIdList = sList.Select(s => s.subject.Id).ToList();
                List<Subject> emptyPOPSubjectList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && !allSubjectIdList.Contains(s.Id) && s.Region != null && s.Region != "" && s.ApproveState == 1 && (s.IsDelete == null || s.IsDelete == false));
                //if (regionList.Any())
                //{
                //    emptyPOPSubjectList = emptyPOPSubjectList.Where(s => regionList.Contains(s.Region.ToLower())).ToList();
                //}
                if (regionList.Any())
                {
                    if (regionList.Contains("空"))
                    {
                        regionList.Remove("空");
                        if (regionList.Any())
                        {
                            emptyPOPSubjectList = emptyPOPSubjectList.Where(s => (s.Region != null && regionList.Contains(s.Region.ToLower())) || s.Region == null || s.Region == "").ToList();
                        }
                        else
                        {
                            emptyPOPSubjectList = emptyPOPSubjectList.Where(s => s.Region == null || s.Region == "").ToList();
                        }
                    }
                    else
                        emptyPOPSubjectList = emptyPOPSubjectList.Where(s => s.Region != null && regionList.Contains(s.Region.ToLower())).ToList();
                }
                if (categoryIdList.Any())
                {
                    emptyPOPSubjectList = emptyPOPSubjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0)).ToList();
                }
                var sList0 = sList.Select(s => s.subject).Distinct().ToList();
                sList0.AddRange(emptyPOPSubjectList);
                StringBuilder json = new StringBuilder();
                sList0.Where(s => (s.HandMakeSubjectId ?? 0) == 0).OrderBy(s => s.SubjectName).ToList().ForEach(s =>
                {
                    json.Append("{\"SubjectId\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetOutsourceList()
        {
            string result = string.Empty;
            var companyList = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.IsDelete == null || s.IsDelete == false)).OrderBy(s => s.ProvinceId).ToList();
            if (companyList.Any())
            {
                if (new BasePage().CurrentUser.RoleId == 5)
                {
                    int userId = new BasePage().CurrentUser.UserId;
                    List<int> outsourceList = new OutsourceInUserBLL().GetList(s => s.UserId == userId).Select(s => s.OutsourceId ?? 0).ToList();
                    companyList = companyList.Where(s => outsourceList.Contains(s.Id)).ToList();
                }
                StringBuilder json = new StringBuilder();
                companyList.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CompanyName\":\"" + s.CompanyName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

        string GetOutsourceFromOrder()
        {
            string result = string.Empty;
            List<int> subjectCategoryIdList = new List<int>();
            if (context1.Request.QueryString["subjectCategoryIds"] != null)
            {
                string subjectCategoryIds = context1.Request.QueryString["subjectCategoryIds"];
                if (!string.IsNullOrWhiteSpace(subjectCategoryIds))
                {
                    subjectCategoryIdList = StringHelper.ToIntList(subjectCategoryIds, ',');
                }
            }
            List<int> subjectIdList = new List<int>();
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                string subjectIds = context1.Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }

            }
            List<string> regionList = new List<string>();
            if (context1.Request.QueryString["region"] != null)
            {
                string region = context1.Request.QueryString["region"];
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                }
            }
            List<string> provinceList = new List<string>();
            if (context1.Request.QueryString["province"] != null)
            {
                string province = context1.Request.QueryString["province"];
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                }
            }
            List<string> cityList = new List<string>();
            if (context1.Request.QueryString["city"] != null)
            {
                string city = context1.Request.QueryString["city"];
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                }
            }
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<Subject> subjectList = new List<Subject>();
            List<int> guidanceIdList = new List<int>();
            if (context1.Session["ChangeOutsourceOrderList="] != null)
            {
                orderList = context1.Session["ChangeOutsourceOrderList="] as List<OutsourceOrderDetail>;
            }
            if (context1.Session["ChangeOutsourceSubjectList="] != null)
            {
                subjectList = context1.Session["ChangeOutsourceSubjectList="] as List<Subject>;
            }
            if (context1.Session["ChangeOutsourceGuidanceIdList="] != null)
            {
                guidanceIdList = context1.Session["ChangeOutsourceGuidanceIdList="] as List<int>;
            }
            if (subjectIdList.Any())
            {
                //orderList = orderList.Where(s => subjectList.Contains(s.SubjectId ?? 0) || s.SubjectId == 0).ToList();
                //百丽订单项目
                Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                subjectIdList.AddRange(hMSubjectIdList);
            }
            var sList = (from order in orderList
                         join subject1 in subjectList
                         on order.SubjectId equals subject1.Id into subjectTemp
                         join outsource in CurrentContext.DbContext.Company
                         on order.OutsourceId equals outsource.Id
                         from subject in subjectTemp.DefaultIfEmpty()
                         where (subjectCategoryIdList.Any() ? (subjectCategoryIdList.Contains(subject.SubjectCategoryId ?? 0)) : 1 == 1)
                         && (subjectIdList.Any() ? (subjectIdList.Contains(order.SubjectId ?? 0) || subjectIdList.Contains(order.BelongSubjectId ?? 0)) : 1 == 1)
                         && (provinceList.Any() ? provinceList.Contains(order.Province) : 1 == 1)
                         && (cityList.Any() ? cityList.Contains(order.City) : 1 == 1)
                         select new { order, outsource }).ToList();
            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        sList = sList.Where(s => (regionList.Contains(s.order.Region)) || s.order.Region == null || s.order.Region == "").ToList();
                    }
                    else
                    {
                        sList = sList.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                    }
                }
                else
                    sList = sList.Where(s => (regionList.Contains(s.order.Region))).ToList();
            }
            if (sList.Any())
            {
                var outsourceList = sList.Select(s => s.outsource).Distinct().OrderBy(s => s.CompanyName).ToList();
                StringBuilder json = new StringBuilder();
                outsourceList.ForEach(s =>
                {
                    json.Append("{\"OutsourceId\":\"" + s.Id + "\",\"OutsourceName\":\"" + s.CompanyName + "\"},");
                });
                if (json.Length > 0)
                {
                    result = "[" + json.ToString().TrimEnd(',') + "]";
                }
            }
            return result;
        }

        string GetOrderList_old()
        {
            string result = string.Empty;
            int currPage = 0, pageSize = 0;
            if (context1.Request.QueryString["currpage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            string outsourceType = string.Empty;
            string shopNo = string.Empty;
            string exportType = string.Empty;
            string materialName = string.Empty;
            string materialCategoryId = string.Empty;
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.QueryString["outsourceId"]))
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = context1.Request.QueryString["guidanceIds"];
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context1.Request.QueryString["subjectIds"];
            }
            if (context1.Request.QueryString["outsourceType"] != null)
            {
                outsourceType = context1.Request.QueryString["outsourceType"];
            }
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }
            if (context1.Request.QueryString["materialCategoryId"] != null)
            {
                materialCategoryId = context1.Request.QueryString["materialCategoryId"];
            }

            if (context1.Request.QueryString["exportType"] != null)
            {
                exportType = context1.Request.QueryString["exportType"];
            }
            if (context1.Request.QueryString["materialName"] != null)
            {
                materialName = context1.Request.QueryString["materialName"];
            }
            List<int> guidanceList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                guidanceList = StringHelper.ToIntList(guidanceIds, ',');
            }
            List<int> subjectList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectList = StringHelper.ToIntList(subjectIds, ',');
            }
            List<int> outsourceTypeList = new List<int>();
            if (!string.IsNullOrWhiteSpace(outsourceType))
            {
                outsourceTypeList = StringHelper.ToIntList(outsourceType, ',');
            }
            List<string> shopNoList = new List<string>();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                shopNoList = StringHelper.ToStringList(shopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower);
            }
            List<int> materialCategoryIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(materialCategoryId))
            {
                materialCategoryIdList = StringHelper.ToIntList(materialCategoryId, ',');
            }
            var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                             where outsourceList.Contains(order.OutsourceId ?? 0)
                            && guidanceList.Contains(order.GuidanceId ?? 0)
                             select
                                 //new {
                                 order
                //assign
                //}
                            ).ToList();
            if (subjectList.Any())
            {
                orderList = orderList.Where(s => subjectList.Contains(s.SubjectId ?? 0) || s.SubjectId == 0).ToList();
            }
            if (!string.IsNullOrWhiteSpace(exportType))
            {
                if (exportType == "nohc")
                {
                    orderList = orderList.Where(s => (s.Format != null && s.Format != "") ? (s.Format.ToLower().IndexOf("hc") == -1 && s.Format.ToLower().IndexOf("homecourt") == -1 && s.Format.ToLower().IndexOf("homecore") == -1 && s.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                }
                else if (exportType == "hc")
                {
                    orderList = orderList.Where(s => (s.Format != null && s.Format != "") ? (s.Format.ToLower().IndexOf("hc") != -1 || s.Format.ToLower().IndexOf("homecourt") != -1 || s.Format.ToLower().IndexOf("homecore") != -1 || s.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (materialName == "软膜")
                {
                    orderList = orderList.Where(s => s.GraphicMaterial != null && s.GraphicMaterial.Contains("软膜")).ToList();
                }
                else if (materialName == "非软膜")
                {
                    orderList = orderList.Where(s => s.GraphicMaterial == null || s.GraphicMaterial == "" || (s.GraphicMaterial != null && !s.GraphicMaterial.Contains("软膜"))).ToList();
                }
            }
            if (materialCategoryIdList.Any())
            {
                bool hasEmpty = false;
                List<string> materialList = new List<string>();
                if (materialCategoryIdList.Contains(0))
                {
                    hasEmpty = true;
                    materialCategoryIdList.Remove(0);
                }
                if (materialCategoryIdList.Any())
                {
                    materialList = new OrderMaterialMppingBLL().GetList(s => materialCategoryIdList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                }
                if (hasEmpty)
                {
                    if (materialList.Any())
                    {
                        orderList = orderList.Where(s => materialList.Contains(s.GraphicMaterial.ToLower()) || (s.GraphicMaterial == null || s.GraphicMaterial == "")).ToList();

                    }
                    else
                        orderList = orderList.Where(s => (s.GraphicMaterial == null || s.GraphicMaterial == "")).ToList();
                }
                else
                    orderList = orderList.Where(s => materialList.Contains(s.GraphicMaterial.ToLower())).ToList();
            }

            if (outsourceTypeList.Any())
            {
                orderList = orderList.Where(s => outsourceTypeList.Contains(s.AssignType ?? 0)).ToList();
            }
            if (shopNoList.Any())
            {
                orderList = orderList.Where(s => shopNoList.Contains(s.ShopNo.ToLower())).ToList();
            }
            if (orderList.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = orderList.Count;
                orderList = orderList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).ThenBy(s => s.Sheet).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                orderList.ForEach(s =>
                {
                    string gender = s.Gender;
                    string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.OrderType ?? 1).ToString());
                    string orderPrice = string.Empty;
                    string receiveOrderPrice = string.Empty;
                    int Quantity = s.Quantity ?? 1;
                    if ((s.PayOrderPrice ?? 0) > 0)
                    {
                        orderPrice = (s.PayOrderPrice ?? 0).ToString();
                        receiveOrderPrice = ((s.ReceiveOrderPrice ?? 0) * Quantity).ToString();
                        //Quantity = 1;
                    }
                    json.Append("{\"rowIndex\":\"" + index + "\",\"OrderType\":\"" + orderType + "\",\"Id\":\"" + s.Id + "\",\"ShopNo\":\"" + s.ShopNo + "\",\"ShopName\":\"" + s.ShopName + "\",\"Region\":\"" + s.Region + "\",\"Province\":\"" + s.Province + "\",\"City\":\"" + s.City + "\",\"CityTier\":\"" + s.CityTier + "\",\"Channel\":\"" + s.Channel + "\",\"Format\":\"" + s.Format + "\",\"Sheet\":\"" + s.Sheet + "\",\"Gender\":\"" + gender + "\",\"Quantity\":\"" + Quantity + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicMaterial\":\"" + s.OrderGraphicMaterial + "\",\"ChooseImg\":\"" + s.ChooseImg + "\",\"OrderPrice\":\"" + orderPrice + "\",\"ReceiveOrderPrice\":\"" + receiveOrderPrice + "\",\"IsDelete\":\"" + ((s.IsDelete ?? false) ? 1 : 0) + "\"},");
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

        string GetOrderList()
        {
            string result = string.Empty;
            int currPage = 0, pageSize = 0;
            if (context1.Request.QueryString["currpage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }

            List<int> outsourceIdList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                string outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            List<int> subjectCategoryIdList = new List<int>();
            if (context1.Request.QueryString["subjectCategoryIds"] != null)
            {
                string subjectCategoryIds = context1.Request.QueryString["subjectCategoryIds"];
                if (!string.IsNullOrWhiteSpace(subjectCategoryIds))
                {
                    subjectCategoryIdList = StringHelper.ToIntList(subjectCategoryIds, ',');
                }
            }

            List<int> subjectIdList = new List<int>();
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                string subjectIds = context1.Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }
            }

            List<string> regionList = new List<string>();
            if (context1.Request.QueryString["region"] != null)
            {
                string region = context1.Request.QueryString["region"];
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                }
            }

            List<string> provinceList = new List<string>();
            if (context1.Request.QueryString["province"] != null)
            {
                string province = context1.Request.QueryString["province"];
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                }
            }

            List<string> cityList = new List<string>();
            if (context1.Request.QueryString["city"] != null)
            {
                string city = context1.Request.QueryString["city"];
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                }
            }

            List<int> outsourceTypeList = new List<int>();
            if (context1.Request.QueryString["outsourceType"] != null)
            {
                string outsourceType = context1.Request.QueryString["outsourceType"];
                if (!string.IsNullOrWhiteSpace(outsourceType))
                {
                    outsourceTypeList = StringHelper.ToIntList(outsourceType, ',');
                }
            }

            List<string> shopNoList = new List<string>();
            if (context1.Request.QueryString["shopNo"] != null)
            {
                string shopNo = context1.Request.QueryString["shopNo"];
                if (!string.IsNullOrWhiteSpace(shopNo))
                {
                    shopNoList = StringHelper.ToStringList(shopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower);
                }
            }

            List<int> materialCategoryIdList = new List<int>();
            if (context1.Request.QueryString["materialCategoryId"] != null)
            {
                string materialCategoryId = context1.Request.QueryString["materialCategoryId"];
                if (!string.IsNullOrWhiteSpace(materialCategoryId))
                {
                    materialCategoryIdList = StringHelper.ToIntList(materialCategoryId, ',');
                }
            }
            string exportType = string.Empty;
            if (context1.Request.QueryString["exportType"] != null)
            {
                exportType = context1.Request.QueryString["exportType"];
            }

            string materialName = string.Empty;
            if (context1.Request.QueryString["materialName"] != null)
            {
                materialName = context1.Request.QueryString["materialName"];
            }

            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<Subject> subjectList = new List<Subject>();
            List<int> guidanceIdList = new List<int>();



            if (context1.Session["ChangeOutsourceOrderList="] != null)
            {
                orderList = context1.Session["ChangeOutsourceOrderList="] as List<OutsourceOrderDetail>;
            }
            if (context1.Session["ChangeOutsourceSubjectList="] != null)
            {
                subjectList = context1.Session["ChangeOutsourceSubjectList="] as List<Subject>;
            }
            if (context1.Session["ChangeOutsourceGuidanceIdList="] != null)
            {
                guidanceIdList = context1.Session["ChangeOutsourceGuidanceIdList="] as List<int>;
            }

            if (subjectIdList.Any())
            {
                //orderList = orderList.Where(s => subjectList.Contains(s.SubjectId ?? 0) || s.SubjectId == 0).ToList();
                //百丽订单项目
                Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                subjectIdList.AddRange(hMSubjectIdList);
            }

            var sList = (from order in orderList
                         join subject1 in subjectList
                         on order.SubjectId equals subject1.Id into subjectTemp
                         join outsource in CurrentContext.DbContext.Company
                         on order.OutsourceId equals outsource.Id
                         from subject in subjectTemp.DefaultIfEmpty()
                         where (outsourceIdList.Any() ? (outsourceIdList.Contains(order.OutsourceId ?? 0)) : 1 == 1)
                         && (subjectCategoryIdList.Any() ? (subjectCategoryIdList.Contains(subject.SubjectCategoryId ?? 0)) : 1 == 1)
                         && (subjectIdList.Any() ? (subjectIdList.Contains(order.SubjectId ?? 0) || subjectIdList.Contains(order.BelongSubjectId ?? 0)) : 1 == 1)
                         && (provinceList.Any() ? provinceList.Contains(order.Province) : 1 == 1)
                         && (cityList.Any() ? cityList.Contains(order.City) : 1 == 1)
                         select new { order, subject, outsource }).ToList();

            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        sList = sList.Where(s => (regionList.Contains(s.order.Region)) || s.order.Region == null || s.order.Region == "").ToList();
                    }
                    else
                    {
                        sList = sList.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                    }
                }
                else
                    sList = sList.Where(s => (regionList.Contains(s.order.Region))).ToList();
            }
            if (!string.IsNullOrWhiteSpace(exportType))
            {
                if (exportType == "nohc")
                {
                    sList = sList.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                }
                else if (exportType == "hc")
                {
                    sList = sList.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") != -1 || s.order.Format.ToLower().IndexOf("homecourt") != -1 || s.order.Format.ToLower().IndexOf("homecore") != -1 || s.order.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (materialName == "软膜")
                {
                    sList = sList.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                }
                else if (materialName == "非软膜")
                {
                    sList = sList.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
                }
            }
            if (materialCategoryIdList.Any())
            {
                bool hasEmpty = false;
                List<string> materialList = new List<string>();
                if (materialCategoryIdList.Contains(0))
                {
                    hasEmpty = true;
                    materialCategoryIdList.Remove(0);
                }
                if (materialCategoryIdList.Any())
                {
                    materialList = new OrderMaterialMppingBLL().GetList(s => materialCategoryIdList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                }
                if (hasEmpty)
                {
                    if (materialList.Any())
                    {
                        sList = sList.Where(s => materialList.Contains(s.order.GraphicMaterial.ToLower()) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                    }
                    else
                        sList = sList.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                }
                else
                    sList = sList.Where(s => materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
            }

            if (outsourceTypeList.Any())
            {
                sList = sList.Where(s => outsourceTypeList.Contains(s.order.AssignType ?? 0)).ToList();
            }
            if (shopNoList.Any())
            {
                sList = sList.Where(s => shopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
            }
            if (sList.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = orderList.Count;
                sList = sList.OrderBy(s => s.order.ShopId).ThenBy(s => s.order.OrderType).ThenBy(s => s.order.Sheet).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                sList.ForEach(s =>
                {
                    string gender = s.order.Gender;
                    string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                    string orderPrice = string.Empty;
                    string receiveOrderPrice = string.Empty;
                    string assignType = CommonMethod.GetEnumDescription<OutsourceOrderTypeEnum>((s.order.AssignType??1).ToString());
                    int Quantity = s.order.Quantity ?? 1;
                    if ((s.order.PayOrderPrice ?? 0) > 0)
                    {
                        orderPrice = (s.order.PayOrderPrice ?? 0).ToString();
                        receiveOrderPrice = ((s.order.ReceiveOrderPrice ?? 0) * Quantity).ToString();
                        //Quantity = 1;
                    }
                    json.Append("{\"rowIndex\":\"" + index + "\",\"AssignType\":\"" + assignType + "\",\"OutsourceName\":\"" + s.outsource.CompanyName + "\",\"OrderType\":\"" + orderType + "\",\"Id\":\"" + s.order.Id + "\",\"ShopNo\":\"" + s.order.ShopNo + "\",\"ShopName\":\"" + s.order.ShopName + "\",\"Region\":\"" + s.order.Region + "\",\"Province\":\"" + s.order.Province + "\",\"City\":\"" + s.order.City + "\",\"CityTier\":\"" + s.order.CityTier + "\",\"IsInstall\":\"" + s.order.IsInstall + "\",\"Channel\":\"" + s.order.Channel + "\",\"Format\":\"" + s.order.Format + "\",\"Sheet\":\"" + s.order.Sheet + "\",\"Gender\":\"" + gender + "\",\"Quantity\":\"" + Quantity + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"ChooseImg\":\"" + s.order.ChooseImg + "\",\"OrderPrice\":\"" + orderPrice + "\",\"ReceiveOrderPrice\":\"" + receiveOrderPrice + "\",\"IsDelete\":\"" + ((s.order.IsDelete ?? false) ? 1 : 0) + "\",\"UnitPrice\":\"" + s.order.UnitPrice + "\"},");
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

        string GetMaterialCategory_old()
        {
            string result = string.Empty;
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = context1.Request.QueryString["guidanceIds"];
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context1.Request.QueryString["subjectIds"];
            }
            List<int> guidanceList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                guidanceList = StringHelper.ToIntList(guidanceIds, ',');
            }
            List<int> subjectList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectList = StringHelper.ToIntList(subjectIds, ',');
            }
            List<string> orderMaterialList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                              //join assign in CurrentContext.DbContext.OutsourceAssignShop
                                              //on order.ShopId equals assign.ShopId
                                              where outsourceList.Contains(order.OutsourceId ?? 0)
                                              && guidanceList.Contains(order.GuidanceId ?? 0)
                                              && (subjectList.Any() ? subjectList.Contains(order.SubjectId ?? 0) : 1 == 1)
                                              select
                                                  order.GraphicMaterial
                                              ).Distinct().ToList();

            if (orderMaterialList.Any())
            {
                bool isEmpty = false;
                if (orderMaterialList.Contains("") || orderMaterialList.Contains(" ") || orderMaterialList.Contains(null))
                {
                    isEmpty = true;
                    orderMaterialList.Remove("");
                    orderMaterialList.Remove(" ");
                    orderMaterialList.Remove(null);
                }
                var list1 = (from material in orderMaterialList
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
                StringBuilder json = new StringBuilder();
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
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;
        }



        string GetProvinceList()
        {
            string result = string.Empty;

            List<int> guidanceIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
            List<int> outsourceIdList = new List<int>();
            List<string> regionList = new List<string>();
            List<int> categoryIdList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                string outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["region"] != null)
            {
                string region = context1.Request.QueryString["region"];
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                }
            }
            if (context1.Request.QueryString["category"] != null)
            {
                string category = context1.Request.QueryString["category"];
                if (!string.IsNullOrWhiteSpace(category))
                {
                    categoryIdList = StringHelper.ToIntList(category, ',');
                }
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                string subjectIds = context1.Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<Subject> subjectList = new List<Subject>();
            if (context1.Session["ChangeOutsourceOrderList="] != null)
            {
                orderList = context1.Session["ChangeOutsourceOrderList="] as List<OutsourceOrderDetail>;
            }
            if (context1.Session["ChangeOutsourceSubjectList="] != null)
            {
                subjectList = context1.Session["ChangeOutsourceSubjectList="] as List<Subject>;
            }
            if (context1.Session["ChangeOutsourceGuidanceIdList="] != null)
            {
                guidanceIdList = context1.Session["ChangeOutsourceGuidanceIdList="] as List<int>;
            }

            if (subjectIdList.Any())
            {
                //百丽订单项目
                Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                subjectIdList.AddRange(hMSubjectIdList);
                //sList = sList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0) || subjectIdList.Contains(s.order.BelongSubjectId ?? 0)).ToList();
            }

            var sList = (from order in orderList
                         join subject in subjectList
                         on order.SubjectId equals subject.Id
                         where (outsourceIdList.Any() ? (outsourceIdList.Contains(order.OutsourceId ?? 0)) : 1 == 1)
                             //&& (order.Region != null && (regionList.Contains(order.Region.ToLower())))
                         && (categoryIdList.Any() ? (categoryIdList.Contains(subject.SubjectCategoryId ?? 0)) : 1 == 1)
                             //&& (subjectIdList.Any() ? subjectIdList.Contains(order.SubjectId ?? 0) : 1 == 1)
                         && (subjectIdList.Any() ? (subjectIdList.Contains(order.SubjectId ?? 0) || subjectIdList.Contains(order.BelongSubjectId ?? 0)) : 1 == 1)
                         select new { order, subject }).ToList();
            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        sList = sList.Where(s => (regionList.Contains(s.order.Region)) || s.order.Region == null || s.order.Region == "").ToList();
                    }
                    else
                    {
                        sList = sList.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                    }
                }
                else
                    sList = sList.Where(s => (regionList.Contains(s.order.Region))).ToList();

                if (sList.Any())
                {
                    StringBuilder json = new StringBuilder();
                    var provinceList = sList.Select(s => s.order.Province).Distinct().OrderBy(s => s).ToList();
                    provinceList.ForEach(s =>
                    {
                        json.Append("{\"Province\":\"" + s + "\"},");
                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                return "";
            }
            return "";

        }

        string GetCityList_old()
        {
            string result = string.Empty;
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string provinces = string.Empty;
            string subjectIds = string.Empty;
            List<int> guidanceList = new List<int>();
            List<int> subjectIdList = new List<int>();
            List<string> provinceList = new List<string>();
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = context1.Request.QueryString["guidanceIds"];
                if (!string.IsNullOrWhiteSpace(guidanceIds))
                {
                    guidanceList = StringHelper.ToIntList(guidanceIds, ',');
                }
            }
            if (context1.Request.QueryString["provinces"] != null)
            {
                provinces = context1.Request.QueryString["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context1.Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            List<Shop> shopList = new List<Shop>();
            var orderList = (from assign in CurrentContext.DbContext.OutsourceOrderDetail
                             join shop in CurrentContext.DbContext.Shop
                             on assign.ShopId equals shop.Id
                             where outsourceList.Contains(assign.OutsourceId ?? 0)
                             && guidanceList.Contains(assign.GuidanceId ?? 0)
                                 //&& (subjectList.Any() ? subjectList.Contains(assign.SubjectId ?? 0) : 1 == 1)
                             && provinceList.Contains(shop.ProvinceName)
                             select new { assign, shop }).ToList();
            if (subjectIdList.Any())
            {
                //百丽订单项目
                Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                subjectIdList.AddRange(hMSubjectIdList);
                orderList = orderList.Where(s => subjectIdList.Contains(s.assign.SubjectId ?? 0) || subjectIdList.Contains(s.assign.BelongSubjectId ?? 0)).ToList();
            }
            shopList = orderList.Select(s => s.shop).Distinct().ToList();
            if (shopList.Any())
            {
                StringBuilder json = new StringBuilder();
                var cityList = shopList.Select(s => s.CityName).Distinct().OrderBy(s => s).ToList();
                cityList.ForEach(s =>
                {
                    json.Append("{\"City\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetCityList()
        {
            string result = string.Empty;

            List<int> guidanceIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
            List<int> outsourceIdList = new List<int>();
            //List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<int> categoryIdList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                string outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            //if (context1.Request.QueryString["region"] != null)
            //{
            //    string region = context1.Request.QueryString["region"];
            //    if (!string.IsNullOrWhiteSpace(region))
            //    {
            //        regionList = StringHelper.ToStringList(region, ',');
            //    }
            //}
            if (context1.Request.QueryString["category"] != null)
            {
                string category = context1.Request.QueryString["category"];
                if (!string.IsNullOrWhiteSpace(category))
                {
                    categoryIdList = StringHelper.ToIntList(category, ',');
                }
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                string subjectIds = context1.Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            if (context1.Request.QueryString["province"] != null)
            {
                string province = context1.Request.QueryString["province"];
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                }
            }
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<Subject> subjectList = new List<Subject>();
            if (context1.Session["ChangeOutsourceOrderList="] != null)
            {
                orderList = context1.Session["ChangeOutsourceOrderList="] as List<OutsourceOrderDetail>;
            }
            if (context1.Session["ChangeOutsourceSubjectList="] != null)
            {
                subjectList = context1.Session["ChangeOutsourceSubjectList="] as List<Subject>;
            }
            if (context1.Session["ChangeOutsourceGuidanceIdList="] != null)
            {
                guidanceIdList = context1.Session["ChangeOutsourceGuidanceIdList="] as List<int>;
            }

            if (subjectIdList.Any())
            {
                //百丽订单项目
                Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                subjectIdList.AddRange(hMSubjectIdList);
                //sList = sList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0) || subjectIdList.Contains(s.order.BelongSubjectId ?? 0)).ToList();
            }

            var sList = (from order in orderList
                         join subject in subjectList
                         on order.SubjectId equals subject.Id
                         where
                         (outsourceIdList.Any() ? (outsourceIdList.Contains(order.OutsourceId ?? 0)) : 1 == 1)
                         && (subjectIdList.Any() ? (subjectIdList.Contains(order.SubjectId ?? 0) || subjectIdList.Contains(order.BelongSubjectId ?? 0)) : 1 == 1)
                             //&& (subjectIdList.Any() ? subjectIdList.Contains(order.SubjectId ?? 0) : 1 == 1)
                             //&& order.Region != null && (regionList.Contains(order.Region.ToLower()))
                         && (categoryIdList.Any() ? (categoryIdList.Contains(subject.SubjectCategoryId ?? 0)) : 1 == 1)
                         && provinceList.Contains(order.Province)
                         select new { order, subject }).ToList();
            if (sList.Any())
            {
                StringBuilder json = new StringBuilder();
                var cityList = sList.OrderBy(s => s.order.Province).Select(s => s.order.City).Distinct().ToList();
                cityList.ForEach(s =>
                {
                    json.Append("{\"City\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";

        }

        string GetMaterialCategory()
        {
            string result = string.Empty;

            List<int> guidanceIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
            List<int> outsourceIdList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> categoryIdList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                string outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["region"] != null)
            {
                string region = context1.Request.QueryString["region"];
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                }
            }
            if (context1.Request.QueryString["category"] != null)
            {
                string category = context1.Request.QueryString["category"];
                if (!string.IsNullOrWhiteSpace(category))
                {
                    categoryIdList = StringHelper.ToIntList(category, ',');
                }
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                string subjectIds = context1.Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            if (context1.Request.QueryString["province"] != null)
            {
                string province = context1.Request.QueryString["province"];
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                }
            }
            if (context1.Request.QueryString["city"] != null)
            {
                string city = context1.Request.QueryString["city"];
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                }
            }
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<Subject> subjectList = new List<Subject>();
            if (context1.Session["ChangeOutsourceOrderList="] != null)
            {
                orderList = context1.Session["ChangeOutsourceOrderList="] as List<OutsourceOrderDetail>;
            }
            if (context1.Session["ChangeOutsourceSubjectList="] != null)
            {
                subjectList = context1.Session["ChangeOutsourceSubjectList="] as List<Subject>;
            }
            if (context1.Session["ChangeOutsourceGuidanceIdList="] != null)
            {
                guidanceIdList = context1.Session["ChangeOutsourceGuidanceIdList="] as List<int>;
            }
            if (subjectIdList.Any())
            {
                //百丽订单项目
                Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                subjectIdList.AddRange(hMSubjectIdList);
                //sList = sList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0) || subjectIdList.Contains(s.order.BelongSubjectId ?? 0)).ToList();
            }
            var sList = (from order in orderList
                         join subject in subjectList
                         on order.SubjectId equals subject.Id
                         where
                         (outsourceIdList.Any() ? (outsourceIdList.Contains(order.OutsourceId ?? 0)) : 1 == 1)
                         && (subjectIdList.Any() ? subjectIdList.Contains(order.SubjectId ?? 0) : 1 == 1)
                         && (categoryIdList.Any() ? (categoryIdList.Contains(subject.SubjectCategoryId ?? 0)) : 1 == 1)
                         && (provinceList.Any() ? provinceList.Contains(order.Province) : 1 == 1)
                         && (cityList.Any() ? cityList.Contains(order.City) : 1 == 1)
                         select order).ToList();
            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        sList = sList.Where(s => (regionList.Contains(s.Region)) || s.Region == null || s.Region == "").ToList();
                    }
                    else
                    {
                        sList = sList.Where(s => s.Region == null || s.Region == "").ToList();
                    }
                }
                else
                    sList = sList.Where(s => (regionList.Contains(s.Region))).ToList();
            }
            if (sList.Any())
            {
                List<string> orderMaterialList = sList.Select(s => s.GraphicMaterial).Distinct().ToList();
                if (orderMaterialList.Any())
                {
                    bool isEmpty = false;
                    if (orderMaterialList.Contains("") || orderMaterialList.Contains(" ") || orderMaterialList.Contains(null))
                    {
                        isEmpty = true;
                        orderMaterialList.Remove("");
                        orderMaterialList.Remove(" ");
                        orderMaterialList.Remove(null);
                    }
                    var list1 = (from material in orderMaterialList
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
                    StringBuilder json = new StringBuilder();
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
                    result = "[" + json.ToString().TrimEnd(',') + "]";
                }

            }
            return result;
        }

        string GetModel()
        {
            string result = string.Empty;
            int orderId = 0;
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = int.Parse(context1.Request.QueryString["orderId"]);
            }
            OutsourceOrderDetail orderModel = new OutsourceOrderDetailBLL().GetModel(orderId);
            if (orderModel != null)
            {
                StringBuilder json = new StringBuilder();
                int materialCategoryId = 0;
                if (!string.IsNullOrWhiteSpace(orderModel.OrderGraphicMaterial))
                {
                    OrderMaterialMpping materialModel = new OrderMaterialMppingBLL().GetList(s => s.OrderMaterialName.ToLower() == orderModel.OrderGraphicMaterial.ToLower()).FirstOrDefault();
                    if (materialModel != null)
                    {
                        materialCategoryId = materialModel.BasicCategoryId ?? 0;
                    }
                }
                int orderType = orderModel.OrderType ?? 1;
                string orderTypeName = CommonMethod.GeEnumName<OrderTypeEnum>(orderType.ToString());
                json.Append("{\"Id\":\"" + orderId + "\",\"OrderType\":\"" + orderType + "\",\"OrderTypeName\":\"" + orderTypeName + "\",\"SubjectId\":\"" + orderModel.SubjectId + "\",\"ShopId\":\"" + orderModel.ShopId + "\",\"ShopName\":\"" + orderModel.ShopName + "\",\"ShopNo\":\"" + orderModel.ShopNo + "\",\"Sheet\":\"" + orderModel.Sheet + "\",\"POSScale\":\"" + orderModel.POSScale + "\",\"MaterialSupport\":\"" + orderModel.MaterialSupport + "\",\"MachineFrame\":\"" + orderModel.MachineFrame + "\",\"PositionDescription\":\"" + orderModel.PositionDescription + "\",\"Gender\":\"" + (!string.IsNullOrWhiteSpace(orderModel.OrderGender) ? orderModel.OrderGender : orderModel.Gender) + "\",\"Quantity\":\"" + (orderModel.Quantity ?? 1) + "\",\"GraphicLength\":\"" + orderModel.GraphicLength + "\",\"GraphicWidth\":\"" + orderModel.GraphicWidth + "\",\"MaterialCategoryId\":\"" + materialCategoryId + "\",\"GraphicMaterial\":\"" + orderModel.OrderGraphicMaterial + "\",\"ChooseImg\":\"" + orderModel.ChooseImg + "\",\"Remark\":\"" + orderModel.Remark + "\",\"Channel\":\"" + orderModel.Channel + "\",\"Format\":\"" + orderModel.Format + "\",\"CityTier\":\"" + orderModel.CityTier + "\",\"IsInstall\":\"" + orderModel.IsInstall + "\",\"PayOrderPrice\":\"" + (orderModel.PayOrderPrice ?? 0) + "\",\"ReceiveOrderPrice\":\"" + (orderModel.ReceiveOrderPrice ?? 0) + "\"}");
                result = "[" + json.ToString() + "]";
            }
            return result;
        }

        string Edit()
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
                    bool isPriceOrder = false;
                    OutsourceOrderDetail orderModel = JsonConvert.DeserializeObject<OutsourceOrderDetail>(jsonStr);
                    if (orderModel != null)
                    {
                        int orderType = orderModel.OrderType ?? 1;
                        string orderTypeDes = CommonMethod.GetEnumDescription<OrderTypeEnum>(orderType.ToString());
                        if (orderTypeDes.Contains("费用订单"))
                        {
                            isPriceOrder = true;
                        }

                        OutsourceOrderDetailBLL orderBll = new OutsourceOrderDetailBLL();

                        if (orderModel.Id > 0)
                        {
                            OutsourceOrderDetail newOrderModel = orderBll.GetModel(orderModel.Id);
                            if (newOrderModel != null)
                            {

                                List<string> ChangePOPCountSheetList = new List<string>();
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

                                if (isPriceOrder)
                                {
                                    newOrderModel.ReceiveOrderPrice = orderModel.ReceiveOrderPrice;
                                    newOrderModel.PayOrderPrice = orderModel.PayOrderPrice;

                                }
                                else
                                {
                                    int Quantity = orderModel.Quantity ?? 1;
                                    if (!string.IsNullOrWhiteSpace(orderModel.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(orderModel.Sheet.ToUpper()))
                                    {
                                        Quantity = Quantity > 0 ? 1 : 0;
                                    }
                                    decimal width = orderModel.GraphicWidth ?? 0;
                                    decimal length = orderModel.GraphicLength ?? 0;
                                    newOrderModel.Area = (width * length) / 1000000;
                                    newOrderModel.MaterialSupport = orderModel.MaterialSupport;
                                    newOrderModel.InstallPriceMaterialSupport = orderModel.MaterialSupport;
                                    newOrderModel.POSScale = orderModel.POSScale;
                                    newOrderModel.ChooseImg = orderModel.ChooseImg;
                                    newOrderModel.OrderGender = orderModel.Gender;
                                    newOrderModel.GraphicLength = length;

                                    newOrderModel.GraphicWidth = width;
                                    newOrderModel.MachineFrame = orderModel.MachineFrame;
                                    newOrderModel.OrderType = orderModel.OrderType;
                                    newOrderModel.PositionDescription = orderModel.PositionDescription;
                                    newOrderModel.POSScale = orderModel.POSScale;

                                    newOrderModel.Sheet = orderModel.Sheet;
                                    decimal unitPrice = 0;
                                    decimal totalPrice = 0;

                                    string material = string.Empty;
                                    string material0 = orderModel.OrderGraphicMaterial;
                                    if (!string.IsNullOrWhiteSpace(material0))
                                        material = new BasePage().GetBasicMaterial(material0);
                                    if (string.IsNullOrWhiteSpace(material))
                                        material = orderModel.OrderGraphicMaterial;
                                    newOrderModel.OrderGraphicMaterial = orderModel.OrderGraphicMaterial;
                                    newOrderModel.GraphicMaterial = material;
                                    if (!string.IsNullOrWhiteSpace(material))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = material0;
                                        pop.GraphicLength = orderModel.GraphicLength;
                                        pop.GraphicWidth = orderModel.GraphicWidth;
                                        pop.Quantity = Quantity;

                                        int subjectId = newOrderModel.SubjectId ?? 0;
                                        var guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                                             on subject.GuidanceId equals guidance.ItemId
                                                             where subject.Id == subjectId
                                                             select new { guidance, subject }).FirstOrDefault();
                                        if (guidanceModel != null)
                                        {
                                            pop.CustomerId = guidanceModel.subject.CustomerId;
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                            if (guidanceModel.guidance.ActivityTypeId == (int)GuidanceTypeEnum.Delivery)
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
                                            new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                            newOrderModel.UnitPrice = unitPrice;
                                            newOrderModel.TotalPrice = totalPrice;
                                        }
                                        else
                                        {
                                            throw new Exception("获取项目失败");
                                        }
                                    }
                                    newOrderModel.UnitPrice = unitPrice;
                                    newOrderModel.TotalPrice = totalPrice;
                                }
                                newOrderModel.Quantity = orderModel.Quantity;
                                newOrderModel.Remark = orderModel.Remark;
                                orderBll.Update(newOrderModel);
                            }

                        }
                    }
                    ReLoadSession();
                }
                catch (Exception ex)
                {
                    result = "提交失败：" + ex.Message;
                }

            }
            else
            {
                result = "提交失败！";
            }
            return result;
        }

        string DeleteOrder()
        {
            string result = "ok";
            string orderId = string.Empty;
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = context1.Request.QueryString["orderId"];
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(orderId))
                {
                    List<int> idList = StringHelper.ToIntList(orderId, ',');
                    OutsourceOrderDetailBLL bll = new OutsourceOrderDetailBLL();
                    var list = bll.GetList(s => idList.Contains(s.Id));
                    if (list.Any())
                    {
                        OutsourceOrderDetail model = null;
                        list.ForEach(s =>
                        {
                            model = new OutsourceOrderDetail();
                            model = s;
                            model.IsDelete = true;
                            model.ModifyType = "删除";
                            model.ModifyUserId = new BasePage().CurrentUser.UserId;
                            model.ModifyDate = DateTime.Now;
                            bll.Update(s);
                        });
                        ReLoadSession();
                    }

                }
            }
            catch (Exception ex)
            {
                result = "删除失败：" + ex.Message;
            }
            return result;
        }

        string RecoverOrder()
        {
            string result = "ok";
            string orderId = string.Empty;
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = context1.Request.QueryString["orderId"];
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(orderId))
                {
                    List<int> idList = StringHelper.ToIntList(orderId, ',');
                    OutsourceOrderDetailBLL bll = new OutsourceOrderDetailBLL();
                    var list = bll.GetList(s => idList.Contains(s.Id));
                    if (list.Any())
                    {
                        OutsourceOrderDetail model = null;
                        list.ForEach(s =>
                        {
                            model = new OutsourceOrderDetail();
                            model = s;
                            model.IsDelete = false;
                            model.ModifyType = "恢复";
                            model.ModifyUserId = new BasePage().CurrentUser.UserId;
                            model.ModifyDate = DateTime.Now;
                            bll.Update(s);
                        });
                        ReLoadSession();
                    }

                }
            }
            catch (Exception ex)
            {
                result = "恢复失败：" + ex.Message;
            }
            return result;
        }

        OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();
        string ChangeOutsource()
        {
            string result = "ok";
            string subjectId = string.Empty;
            string orderId = string.Empty;
            int newOutsourceId = 0;
            int changeType = 1;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = context1.Request.QueryString["subjectId"];
            }
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = context1.Request.QueryString["orderId"];
            }
            if (context1.Request.QueryString["newOutsourceId"] != null)
            {
                newOutsourceId = int.Parse(context1.Request.QueryString["newOutsourceId"]);
            }
            if (context1.Request.QueryString["changeType"] != null)
            {
                changeType = int.Parse(context1.Request.QueryString["changeType"]);
            }
            try
            {
                if (newOutsourceId > 0)
                {
                    
                    if (changeType == 1 && !string.IsNullOrWhiteSpace(subjectId))//按项目更改
                    {
                        List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                        if (subjectIdList.Any())
                        {
                            List<OutsourceOrderDetail> outsourceOrderList = new OutsourceOrderDetailBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0) || subjectIdList.Contains(s.BelongSubjectId ?? 0));
                            if (outsourceOrderList.Any())
                            {
                                List<int> guidanceIdList = outsourceOrderList.Select(s=>s.GuidanceId??0).Distinct().ToList();
                                guidanceIdList.ForEach(gid => {
                                    SubjectGuidance guidanceModel = guidanceBll.GetModel(gid);
                                    if (guidanceModel != null)
                                    {
                                        if (!guidanceDic.Keys.Contains(gid))
                                        {
                                            guidanceDic.Add(gid, guidanceModel);
                                        }
                                        var orderList = outsourceOrderList.Where(s=>s.GuidanceId==gid).ToList();
                                        var popOrderList = orderList.Where(s =>(s.SubjectId??0)>0).ToList();
                                        List<int> shopIdList = popOrderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                                        //更新pop
                                        popOrderList.ForEach(order => {
                                            UpdatePOPOrder(order, newOutsourceId);
                                        });
                                        if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion)
                                        {
                                            //快递费
                                            var expressPriceOrderList = orderList.Where(s => (s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.快递费) && (s.SubjectId ?? 0) == 0 && shopIdList.Contains(s.ShopId ?? 0)).ToList();
                                            expressPriceOrderList.ForEach(order =>
                                            {
                                                UpdatePOPOrder(order, newOutsourceId);
                                            });
                                        }
                                    }
                                });
                            }
                        }
                    }
                    else if (changeType == 2 && !string.IsNullOrWhiteSpace(orderId))//只更新选择的pop
                    {
                        List<int> idList = StringHelper.ToIntList(orderId, ',');

                        var list = outsourceOrderBll.GetList(s => idList.Contains(s.Id));

                        list.ForEach(order =>
                        {
                            UpdatePOPOrder(order, newOutsourceId);
                        });
                    }
                }
                ReLoadSession();
            }
            catch (Exception ex)
            {
                result = "更新失败：" + ex.Message;
            }
            return result;
        }

        Dictionary<int, int> customerIdDic = new Dictionary<int, int>();
        SubjectBLL subjectBll = new SubjectBLL();
        SubjectGuidanceBLL guidanceBll = new SubjectGuidanceBLL();
        Dictionary<int, SubjectGuidance> guidanceDic = new Dictionary<int, SubjectGuidance>();
        void UpdatePOPOrder(OutsourceOrderDetail order, int newOutsourceId)
        {
            int oldOutsourceId = order.OutsourceId ?? 0;
            order.OutsourceId = newOutsourceId;
            SubjectGuidance guidanceModel = new SubjectGuidance();
            if (guidanceDic.Keys.Contains(order.GuidanceId ?? 0))
            {
                guidanceModel = guidanceDic[order.GuidanceId ?? 0];
            }
            else
            {
                guidanceModel = guidanceBll.GetModel(order.GuidanceId ?? 0);
                if (guidanceModel != null)
                {
                    guidanceDic.Add(order.GuidanceId ?? 0, guidanceModel);
                }
            }
            if (order.OrderType == ((int)OrderTypeEnum.POP) && guidanceModel.ActivityTypeId==(int)GuidanceTypeEnum.Install && order.IsInstall == "Y")
            {

                if (oldOutsourceId != newOutsourceId)
                {
                    int customerId = 0;
                    if (customerIdDic.Keys.Contains(order.SubjectId ?? 0))
                    {
                        customerId = customerIdDic[order.SubjectId ?? 0];
                    }
                    else
                    {
                        Subject sModel = subjectBll.GetModel(order.SubjectId ?? 0);
                        if (sModel != null)
                        {
                            customerId = sModel.CustomerId ?? 0;
                            customerIdDic.Add(order.SubjectId ?? 0, customerId);
                        }
                    }
                    //判断新外协是否和主外协一样，如果一样使用 生产+安装 价格，如果不一样，使用 安装价格
                    int shopOutsourceId = GetShopOutsourceId(order.ShopId ?? 0);
                    if (shopOutsourceId == newOutsourceId)
                    {
                        POP pop = new POP();
                        pop.GraphicLength = order.GraphicLength;
                        pop.GraphicWidth = order.GraphicWidth;
                        string material = string.Empty;
                        if (!string.IsNullOrWhiteSpace(order.OrderGraphicMaterial))
                            material = new BasePage().GetBasicMaterial(order.OrderGraphicMaterial);
                        if (string.IsNullOrWhiteSpace(material))
                            material = order.OrderGraphicMaterial;
                        pop.GraphicMaterial = material;
                        pop.Quantity = order.Quantity;
                        pop.CustomerId = customerId;
                        pop.OutsourceType = (int)OutsourceOrderTypeEnum.InstallAndProduce;
                        
                        decimal unitPrice = 0;
                        decimal totalPrice = 0;
                        new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                        order.UnitPrice = unitPrice;
                        order.TotalPrice = totalPrice;
                        order.AssignType = (int)OutsourceOrderTypeEnum.Install;
                        outsourceOrderBll.Update(order);
                    }
                    else
                    {
                        POP pop = new POP();
                        pop.GraphicLength = order.GraphicLength;
                        pop.GraphicWidth = order.GraphicWidth;
                        string material = string.Empty;
                        if (!string.IsNullOrWhiteSpace(order.OrderGraphicMaterial))
                            material = new BasePage().GetBasicMaterial(order.OrderGraphicMaterial);
                        if (string.IsNullOrWhiteSpace(material))
                            material = order.OrderGraphicMaterial;
                        pop.GraphicMaterial = material;
                        pop.Quantity = order.Quantity;
                        pop.CustomerId = customerId;
                        pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                        decimal unitPrice = 0;
                        decimal totalPrice = 0;
                        new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                        order.UnitPrice = unitPrice;
                        order.TotalPrice = totalPrice;
                        order.AssignType = (int)OutsourceOrderTypeEnum.Send;
                        outsourceOrderBll.Update(order);
                    }
                }
            }
            else
            {
                outsourceOrderBll.Update(order);
            }
        }

        /// <summary>
        /// 获取店铺主外协
        /// </summary>
        Dictionary<int, int> shopOutsourceIdDic = new Dictionary<int, int>();
        ShopBLL shopBll = new ShopBLL();
        int GetShopOutsourceId(int shopId)
        {
            if (shopOutsourceIdDic.Keys.Contains(shopId))
            {
                return shopOutsourceIdDic[shopId];
            }
            else
            {
                Shop model = shopBll.GetModel(shopId);
                int osId = 0;
                if (model != null)
                {
                    osId = model.OutsourceId ?? 0;
                    shopOutsourceIdDic.Add(shopId, osId);
                }
                return osId;
            }
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
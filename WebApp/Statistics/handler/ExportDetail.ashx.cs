using System;
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

namespace WebApp.Statistics.handler
{
    /// <summary>
    /// ExportDetail 的摘要说明
    /// </summary>
    public class ExportDetail : IHttpHandler
    {
        string guidanceIds = string.Empty;
        string subjectIds = string.Empty;
        string regions = string.Empty;
        string provinces = string.Empty;
        string citys = string.Empty;
        int isExportInstall = 0;
        //string installPrice = string.Empty;
        string freight = string.Empty;
        HttpContext context1;
        string status = string.Empty;//0 表示正常，1 表示闭店信息
        int subjectChannel = 0;
        string shopType = string.Empty;
        string customerServiceIds = string.Empty;
        string subjectCategory = string.Empty;
        string exportType = string.Empty;
        string begin = string.Empty;
        string end = string.Empty;
        int dateSearchType = 1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["guidanceIds"] != null)
                guidanceIds = context.Request.QueryString["guidanceIds"];
            if (context.Request.QueryString["subjectIds"] != null)
                subjectIds = context.Request.QueryString["subjectIds"];
            if (context.Request.QueryString["regions"] != null)
                regions = context.Request.QueryString["regions"];
            if (context.Request.QueryString["provinces"] != null)
                provinces = context.Request.QueryString["provinces"];
            if (context.Request.QueryString["citys"] != null)
                citys = context.Request.QueryString["citys"];
            if (context.Request.QueryString["isExportInstall"] != null)
                isExportInstall = int.Parse(context.Request.QueryString["isExportInstall"]);
            
            if (context.Request.QueryString["freight"] != null)
                freight = context.Request.QueryString["freight"];

            if (context.Request.QueryString["status"] != null)
                status = context.Request.QueryString["status"];
            if (context.Request.QueryString["subjectChannel"] != null)
                subjectChannel = int.Parse(context.Request.QueryString["subjectChannel"]);
            if (context.Request.QueryString["shopType"] != null)
            {
                shopType = context.Request.QueryString["shopType"];
            }
            if (context.Request.QueryString["customerServiceIds"] != null)
            {
                customerServiceIds = context.Request.QueryString["customerServiceIds"];
            }
            if (context.Request.QueryString["subjectCategory"] != null)
            {
                subjectCategory = context.Request.QueryString["subjectCategory"];
            }
            if (context.Request.QueryString["exportType"] != null)
            {
                exportType = context.Request.QueryString["exportType"];
            }
            if (context.Request.QueryString["beginDate"] != null)
            {
                begin = context.Request.QueryString["beginDate"];
            }
            if (context.Request.QueryString["endDate"] != null)
            {
                end = context.Request.QueryString["endDate"];
            }
            if (exportType == "byShop")
                ExportByShop();
            else
                Export();
        }

        void Export()
        {
            string fileName = "项目费用统计明细";
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                List<int> guidanceIdList = new List<int>();
                List<int> subjectIdList = new List<int>();
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<string> shopTypeList = new List<string>();
                List<int> customerServiceList = new List<int>();
                List<int> subjectCategoryList = new List<int>();
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
                if (!string.IsNullOrWhiteSpace(subjectIds))
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');

                if (!string.IsNullOrWhiteSpace(customerServiceIds))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceIds, ',');
                }
                if (!string.IsNullOrWhiteSpace(subjectCategory))
                {
                    subjectCategoryList = StringHelper.ToIntList(subjectCategory, ',');
                }

                string guidanceName = string.Empty;
                List<OrderPriceDetail> newOrderList = new List<OrderPriceDetail>();
                OrderPriceDetail orderModel;
                if (guidanceIdList.Any())
                {
                    #region pop订单费用
                    if (!string.IsNullOrWhiteSpace(regions))
                    {
                        regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                    }
                    else
                    {
                        new BasePage().GetResponsibleRegion.ForEach(s =>
                        {
                            regionList.Add(s.ToLower());
                        });

                    }
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join guindance in CurrentContext.DbContext.SubjectGuidance
                                     on order.GuidanceId equals guindance.ItemId
                                     join user1 in CurrentContext.DbContext.UserInfo
                                     on order.CSUserId equals user1.UserId into userTemp
                                     from user in userTemp.DefaultIfEmpty()
                                     where guidanceIdList.Contains(order.GuidanceId ?? 0)
                                    && (subject.IsDelete == null || subject.IsDelete == false)
                                    && (subject.ApproveState == 1)
                                    && (order.IsDelete == null || order.IsDelete == false)   
                                     && (regionList.Any() ? regionList.Contains(order.Region.ToLower()) : 1 == 1)

                                     select new
                                     {
                                         order,
                                         shop,
                                         subject,
                                         guindance,
                                         CSName=user!=null?user.RealName:""

                                     }).ToList();
                    if (!string.IsNullOrWhiteSpace(begin))
                    {
                        DateTime beginDate = DateTime.Parse(begin);
                        if (dateSearchType == 1)
                        {
                            orderList = orderList.Where(s => s.subject.AddDate >= beginDate).ToList();
                            if (!string.IsNullOrWhiteSpace(end))
                            {
                                DateTime endDate = DateTime.Parse(end).AddDays(1);
                                orderList = orderList.Where(s => s.subject.AddDate < endDate).ToList();
                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => s.order.AddDate >= beginDate).ToList();
                            if (!string.IsNullOrWhiteSpace(end))
                            {
                                DateTime endDate = DateTime.Parse(end).AddDays(1);
                                orderList = orderList.Where(s => s.order.AddDate < endDate).ToList();
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(shopType))
                    {
                        shopTypeList = StringHelper.ToStringList(shopType, ',');

                    }
                    if (subjectCategoryList.Any())
                    {
                        if (subjectCategoryList.Contains(0))
                        {
                            subjectCategoryList.Remove(0);
                            if (subjectCategoryList.Any())
                            {
                                orderList = orderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                                //materialOrderList = materialOrderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                            }
                            else
                            {
                                orderList = orderList.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();
                                //materialOrderList = materialOrderList.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();
                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                            //materialOrderList = materialOrderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                        }
                    }
                    if (subjectChannel == 1)
                    {
                        //上海系统单
                        orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                        //materialOrderList = materialOrderList.Where(s => s.order.RegionSupplementId == null).ToList();
                    }
                    else if (subjectChannel == 2)
                    {
                        //分区订单
                        orderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();
                        //materialOrderList = materialOrderList.Where(s => s.order.RegionSupplementId != null && s.order.RegionSupplementId > 0).ToList();
                    }

                    if (shopTypeList.Any())
                    {
                        if (shopTypeList.Contains("空"))
                        {
                            shopTypeList.Remove("空");
                            if (shopTypeList.Any())
                            {
                                orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                                //materialOrderList = materialOrderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                            }
                            else
                            {
                                orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                                //materialOrderList = materialOrderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                            //materialOrderList = materialOrderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                        }
                    }
                    List<int> allSubjectIdList = new List<int>();
                    if (subjectIdList.Any())
                    {
                        //orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId??0)).ToList();
                        //materialOrderList = materialOrderList.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
                        List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).Distinct().ToList();
                        subjectIdList.AddRange(hMSubjectIdList);
                        allSubjectIdList = subjectIdList;
                    }
                    else
                    {
                        allSubjectIdList = orderList.Select(s => s.order.SubjectId??0).Distinct().ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(provinces))
                    {
                        provinceList = StringHelper.ToStringList(provinces, ',');
                        if (provinceList.Any())
                        {
                            orderList = orderList.Where(s => provinceList.Contains(s.order.Province.Trim())).ToList();
                            //materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                            if (!string.IsNullOrWhiteSpace(citys))
                            {
                                cityList = StringHelper.ToStringList(citys, ',');
                                if (cityList.Any())
                                {
                                    orderList = orderList.Where(s => cityList.Contains(s.order.City.Trim())).ToList();
                                    //materialOrderList = materialOrderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                                }
                            }
                        }
                    }
                    if (customerServiceList.Any())
                    {
                        if (customerServiceList.Contains(0))
                        {
                            customerServiceList.Remove(0);
                            if (customerServiceList.Any())
                            {
                                orderList = orderList.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                                //materialOrderList = materialOrderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                            }
                            else
                            {
                                orderList = orderList.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();
                                //materialOrderList = materialOrderList.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();
                            //materialOrderList = materialOrderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        if (status == "0")
                        {
                            orderList = orderList.Where(s => s.order.ShopStatus == null || s.order.ShopStatus == "" || s.order.ShopStatus == "正常").ToList();
                            //materialOrderList = materialOrderList.Where(s => s.shop.Status == null || s.shop.Status == "" || s.shop.Status == "正常").ToList();
                        }
                        else if (status == "1")
                        {
                            orderList = orderList.Where(s => s.order.ShopStatus != null && s.order.ShopStatus.Contains("闭")).ToList();
                            //materialOrderList = materialOrderList.Where(s => s.shop.Status != null && s.shop.Status.Contains("闭")).ToList();
                        }
                    }
                    var popOrder = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.POP || s.order.OrderType == (int)OrderTypeEnum.道具 || s.order.OrderType == (int)OrderTypeEnum.物料).ToList();
                    //var popOrder = orderList.Select(s=>s).ToList();
                    try
                    {
                        #region pop费用
                        if (popOrder.Any())
                        {

                            var nameList = popOrder.Select(s => s.guindance.ItemName).Distinct().ToList(); ;
                            guidanceName = StringHelper.ListToString(nameList, "+");

                            //decimal totalPrice = 0;
                            //decimal totalArea = 0;
                            List<int> shopList = new List<int>();

                            InstallBLL installBll = new InstallBLL();

                            if (popOrder.Any())
                            {
                                popOrder = popOrder.OrderBy(s => s.order.ShopNo).ToList();

                                popOrder.ForEach(s =>
                                {
                                    #region 旧的
                                    //totalPrice = 0;
                                    //totalArea = 0;
                                    //decimal newPOPArea = 0;
                                    //if (s.order.GraphicWidth != null && s.order.GraphicLength != null)
                                    //{
                                    //    newPOPArea = ((s.order.GraphicWidth ?? 0) * (s.order.GraphicLength ?? 0)) / 1000000;
                                    //}
                                    //if (!string.IsNullOrWhiteSpace(s.order.GraphicMaterial))
                                    //{
                                    //    unitName = string.Empty;
                                    //    decimal defaultUnitPrice = new BasePage().GetMaterialPrice(s.order.GraphicMaterial, out unitName);
                                    //    if (s.order.UnitPrice != null && s.order.UnitPrice > 0)
                                    //    {

                                    //        if (unitName == "个")
                                    //        {
                                    //            totalPrice = (s.order.UnitPrice ?? 0) * (s.order.Quantity ?? 1);
                                    //        }
                                    //        else
                                    //        {
                                    //            totalArea = newPOPArea * (s.order.Quantity ?? 1);
                                    //            totalPrice = newPOPArea * (s.order.UnitPrice ?? 0) * (s.order.Quantity ?? 1);
                                    //        }
                                    //        orderModel = new OrderPriceDetail();
                                    //        if (s.subject.AddDate != null)
                                    //            orderModel.AddDate = DateTime.Parse(s.subject.AddDate.ToString());
                                    //        orderModel.PriceType = s.order.OrderType == 2 ? "道具" : "POP";
                                    //        orderModel.Area = double.Parse(totalArea.ToString());
                                    //        orderModel.Gender = s.order.Gender;
                                    //        orderModel.GraphicLength = double.Parse((s.order.GraphicLength ?? 0).ToString());
                                    //        orderModel.GraphicWidth = double.Parse((s.order.GraphicWidth ?? 0).ToString());
                                    //        orderModel.GraphicMaterial = s.order.GraphicMaterial;
                                    //        orderModel.PositionDescription = s.order.PositionDescription;
                                    //        orderModel.Quantity = s.order.Quantity ?? 1;
                                    //        orderModel.Sheet = s.order.Sheet;
                                    //        orderModel.ShopName = s.shop.ShopName;
                                    //        orderModel.ShopNo = s.shop.ShopNo;
                                    //        orderModel.SubjectName = s.subject.SubjectName;
                                    //        orderModel.SubjectNo = s.subject.SubjectNo;
                                    //        orderModel.TotalPrice = double.Parse(totalPrice.ToString());
                                    //        orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                                    //        newOrderList.Add(orderModel);
                                    //    }
                                    //    else
                                    //    {
                                    //        if (s.order.GraphicMaterial.IndexOf("+挂轴") != -1)
                                    //        {
                                    //            string material0 = s.order.GraphicMaterial.Replace("+挂轴", "").Replace("+上挂轴", "").Replace("+下挂轴", "");
                                    //            decimal unitPrice0 = new BasePage().GetMaterialPrice(material0, out unitName);
                                    //            decimal unitPrice1 = new BasePage().GetMaterialPrice("挂轴", out unitName);
                                    //            decimal popPrice0 = newPOPArea * unitPrice0 * (s.order.Quantity ?? 1);
                                    //            decimal popPrice1 = (s.order.GraphicWidth ?? 0) / 1000 * 2 * unitPrice1 * (s.order.Quantity ?? 1);
                                    //            if (s.order.GraphicMaterial.IndexOf("上挂轴") != -1 || s.order.GraphicMaterial.IndexOf("下挂轴") != -1)
                                    //            {
                                    //                popPrice1 = (s.order.GraphicWidth ?? 0) / 1000 * unitPrice1 * (s.order.Quantity ?? 1);
                                    //            }
                                    //            //totalPrice = (popPrice0 + popPrice1);
                                    //            totalArea = newPOPArea * (s.order.Quantity ?? 1);

                                    //            orderModel = new OrderPriceDetail();
                                    //            if (s.subject.AddDate != null)
                                    //                orderModel.AddDate = DateTime.Parse(s.subject.AddDate.ToString());
                                    //            orderModel.PriceType = s.order.OrderType == 2 ? "道具" : "POP";
                                    //            orderModel.Area = double.Parse(totalArea.ToString());
                                    //            orderModel.Gender = s.order.Gender;
                                    //            orderModel.GraphicLength = double.Parse((s.order.GraphicLength ?? 0).ToString());
                                    //            orderModel.GraphicWidth = double.Parse((s.order.GraphicWidth ?? 0).ToString());
                                    //            orderModel.GraphicMaterial = material0;
                                    //            orderModel.PositionDescription = s.order.PositionDescription;
                                    //            orderModel.Quantity = s.order.Quantity ?? 1;
                                    //            orderModel.Sheet = s.order.Sheet;
                                    //            orderModel.ShopName = s.shop.ShopName;
                                    //            orderModel.ShopNo = s.shop.ShopNo;
                                    //            orderModel.SubjectName = s.subject.SubjectName;
                                    //            orderModel.SubjectNo = s.subject.SubjectNo;
                                    //            orderModel.UnitPrice = double.Parse(unitPrice0.ToString());
                                    //            orderModel.TotalPrice = double.Parse(popPrice0.ToString());
                                    //            newOrderList.Add(orderModel);

                                    //            orderModel = new OrderPriceDetail();
                                    //            if (s.subject.AddDate != null)
                                    //                orderModel.AddDate = DateTime.Parse(s.subject.AddDate.ToString());
                                    //            orderModel.PriceType = s.order.OrderType == 2 ? "道具" : "POP";
                                    //            orderModel.Area = double.Parse(totalArea.ToString());
                                    //            orderModel.Gender = s.order.Gender;
                                    //            orderModel.GraphicLength = double.Parse((s.order.GraphicLength ?? 0).ToString());
                                    //            orderModel.GraphicWidth = double.Parse((s.order.GraphicWidth ?? 0).ToString());
                                    //            orderModel.PositionDescription = s.order.PositionDescription;
                                    //            orderModel.Quantity = s.order.Quantity ?? 1;
                                    //            orderModel.Sheet = s.order.Sheet;
                                    //            orderModel.ShopName = s.shop.ShopName;
                                    //            orderModel.ShopNo = s.shop.ShopNo;
                                    //            orderModel.SubjectName = s.subject.SubjectName;
                                    //            orderModel.SubjectNo = s.subject.SubjectNo;
                                    //            orderModel.GraphicMaterial = "挂轴";
                                    //            if (s.order.GraphicMaterial.IndexOf("上挂轴") != -1)
                                    //            {
                                    //                orderModel.GraphicMaterial = "上挂轴";
                                    //            }
                                    //            if (s.order.GraphicMaterial.IndexOf("下挂轴") != -1)
                                    //            {
                                    //                orderModel.GraphicMaterial = "下挂轴";
                                    //            }
                                    //            orderModel.UnitPrice = double.Parse(unitPrice1.ToString());
                                    //            orderModel.TotalPrice = double.Parse(popPrice1.ToString());
                                    //            newOrderList.Add(orderModel);
                                    //        }
                                    //        else
                                    //        {
                                    //            if (unitName == "个")
                                    //            {
                                    //                totalPrice = defaultUnitPrice * (s.order.Quantity ?? 1);
                                    //            }
                                    //            else
                                    //            {
                                    //                totalArea = newPOPArea * (s.order.Quantity ?? 1);
                                    //                totalPrice = newPOPArea * defaultUnitPrice * (s.order.Quantity ?? 1);
                                    //            }
                                    //            orderModel = new OrderPriceDetail();
                                    //            if (s.subject.AddDate != null)
                                    //                orderModel.AddDate = DateTime.Parse(s.subject.AddDate.ToString());
                                    //            orderModel.PriceType = s.order.OrderType == 2 ? "道具" : "POP";
                                    //            orderModel.Area = double.Parse(totalArea.ToString());
                                    //            orderModel.Gender = s.order.Gender;
                                    //            orderModel.GraphicLength = double.Parse((s.order.GraphicLength ?? 0).ToString());
                                    //            orderModel.GraphicWidth = double.Parse((s.order.GraphicWidth ?? 0).ToString());
                                    //            orderModel.GraphicMaterial = s.order.GraphicMaterial;
                                    //            orderModel.PositionDescription = s.order.PositionDescription;
                                    //            orderModel.Quantity = s.order.Quantity ?? 1;
                                    //            orderModel.Sheet = s.order.Sheet;
                                    //            orderModel.ShopName = s.shop.ShopName;
                                    //            orderModel.ShopNo = s.shop.ShopNo;
                                    //            orderModel.SubjectName = s.subject.SubjectName;
                                    //            orderModel.SubjectNo = s.subject.SubjectNo;
                                    //            orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                                    //            orderModel.TotalPrice = double.Parse(totalPrice.ToString());
                                    //            newOrderList.Add(orderModel);

                                    //        }

                                    //    }
                                    //}
                                    #endregion
                                    string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                                    decimal width = s.order.GraphicWidth ?? 0;
                                    decimal length = s.order.GraphicLength ?? 0;
                                    int Quantity = s.order.Quantity ?? 0;
                                    string GraphicMaterial = s.order.GraphicMaterial;
                                    if (!string.IsNullOrWhiteSpace(GraphicMaterial))
                                    {
                                        #region
                                        //if (GraphicMaterial.Contains("挂轴"))
                                        //{
                                        //    if (GraphicMaterial.Contains("+挂轴") || GraphicMaterial.Contains("+上挂轴") || GraphicMaterial.Contains("+下挂轴"))
                                        //    {

                                        //        string materialName1 = GraphicMaterial.Substring(0, GraphicMaterial.LastIndexOf('+'));

                                        //        string unitName = string.Empty;
                                        //        decimal unitPrice1 = new BasePage().GetMaterialPrice(s.guindance.PriceItemId ?? 0, materialName1, out unitName);

                                        //        decimal totalPrice1 = 0;
                                        //        if (unitName == "个")
                                        //        {
                                        //            totalPrice1 = unitPrice1 * Quantity;
                                        //        }
                                        //        else
                                        //        {
                                        //            totalPrice1 = ((width * length) / 1000000) * unitPrice1 * Quantity;
                                        //        }
                                        //        totalArea = (width * length) / 1000000 * Quantity;
                                        //        orderModel = new OrderPriceDetail();
                                        //        orderModel.AddDate = s.subject.AddDate;
                                        //        //orderModel.PriceType = s.order.OrderType == 2 ? "道具" : "POP";
                                        //        orderModel.PriceType = orderType;
                                        //        orderModel.Area = double.Parse(totalArea.ToString());
                                        //        orderModel.Gender = s.order.Gender;
                                        //        orderModel.GraphicWidth = double.Parse(width.ToString());
                                        //        orderModel.GraphicLength = double.Parse(length.ToString());
                                        //        orderModel.GraphicMaterial = materialName1;
                                        //        orderModel.PositionDescription = s.order.PositionDescription;
                                        //        orderModel.Quantity = Quantity;
                                        //        orderModel.Sheet = s.order.Sheet;
                                        //        orderModel.ShopName = s.shop.ShopName;
                                        //        orderModel.ShopNo = s.shop.ShopNo;
                                        //        orderModel.Region = s.shop.RegionName;
                                        //        orderModel.Province = s.shop.ProvinceName;
                                        //        orderModel.City = s.shop.CityName;
                                        //        orderModel.SubjectName = s.subject.SubjectName;
                                        //        orderModel.SubjectNo = s.subject.SubjectNo;
                                        //        orderModel.TotalPrice = double.Parse(totalPrice1.ToString());
                                        //        orderModel.UnitPrice = double.Parse(unitPrice1.ToString());
                                        //        newOrderList.Add(orderModel);

                                        //        //添加挂轴的pop
                                        //        string materialName2 = GraphicMaterial.Replace(materialName1 + "+", "");//挂轴
                                        //        unitPrice1 = new BasePage().GetMaterialPrice(s.guindance.PriceItemId ?? 0, "挂轴", out unitName);
                                        //        totalArea = (width / 1000) * 2 * Quantity;
                                        //        if (materialName2.Contains("上挂轴") || materialName2.Contains("下挂轴"))
                                        //        {
                                        //            totalArea = (width / 1000) * Quantity;
                                        //        }
                                        //        totalPrice1 = totalArea * unitPrice1;
                                        //        orderModel = new OrderPriceDetail();
                                        //        orderModel.AddDate = s.subject.AddDate;
                                        //        orderModel.PriceType = orderType;
                                        //        orderModel.Area = double.Parse(totalArea.ToString());
                                        //        orderModel.Gender = s.order.Gender;
                                        //        orderModel.GraphicWidth = double.Parse(width.ToString());
                                        //        orderModel.GraphicLength = double.Parse(length.ToString());
                                        //        orderModel.GraphicMaterial = materialName2;
                                        //        orderModel.PositionDescription = s.order.PositionDescription;
                                        //        orderModel.Quantity = Quantity;
                                        //        orderModel.Sheet = s.order.Sheet;
                                        //        orderModel.ShopName = s.shop.ShopName;
                                        //        orderModel.ShopNo = s.shop.ShopNo;
                                        //        orderModel.Region = s.shop.RegionName;
                                        //        orderModel.Province = s.shop.ProvinceName;
                                        //        orderModel.City = s.shop.CityName;
                                        //        orderModel.SubjectName = s.subject.SubjectName;
                                        //        orderModel.SubjectNo = s.subject.SubjectNo;
                                        //        orderModel.TotalPrice = double.Parse(totalPrice1.ToString());
                                        //        orderModel.UnitPrice = double.Parse(unitPrice1.ToString());
                                        //        newOrderList.Add(orderModel);
                                        //    }
                                        //    else
                                        //    {
                                        //        totalArea = (width / 1000) * 2 * Quantity;
                                        //        orderModel = new OrderPriceDetail();
                                        //        orderModel.AddDate = s.subject.AddDate;
                                        //        orderModel.PriceType = orderType;
                                        //        orderModel.Area = double.Parse(totalArea.ToString());
                                        //        orderModel.Gender = s.order.Gender;
                                        //        orderModel.GraphicWidth = double.Parse(width.ToString());
                                        //        orderModel.GraphicLength = double.Parse(length.ToString());
                                        //        orderModel.GraphicMaterial = s.order.GraphicMaterial;
                                        //        orderModel.PositionDescription = s.order.PositionDescription;
                                        //        orderModel.Quantity = Quantity;
                                        //        orderModel.Sheet = s.order.Sheet;
                                        //        orderModel.ShopName = s.shop.ShopName;
                                        //        orderModel.ShopNo = s.shop.ShopNo;
                                        //        orderModel.Region = s.shop.RegionName;
                                        //        orderModel.Province = s.shop.ProvinceName;
                                        //        orderModel.City = s.shop.CityName;
                                        //        orderModel.SubjectName = s.subject.SubjectName;
                                        //        orderModel.SubjectNo = s.subject.SubjectNo;
                                        //        orderModel.TotalPrice = double.Parse((s.order.TotalPrice ?? 0).ToString());
                                        //        orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                                        //        newOrderList.Add(orderModel);
                                        //    }
                                        //}
                                        //else
                                        //{

                                        //    totalArea = (width * length) / 1000000 * Quantity;
                                        //    orderModel = new OrderPriceDetail();
                                        //    orderModel.AddDate = s.subject.AddDate;
                                        //    orderModel.PriceType = orderType;
                                        //    orderModel.Area = double.Parse(totalArea.ToString());
                                        //    orderModel.Gender = s.order.Gender;
                                        //    orderModel.GraphicLength = double.Parse(length.ToString());
                                        //    orderModel.GraphicWidth = double.Parse(width.ToString());
                                        //    orderModel.GraphicMaterial = s.order.GraphicMaterial;
                                        //    orderModel.PositionDescription = s.order.PositionDescription;
                                        //    orderModel.Quantity = Quantity;
                                        //    orderModel.Sheet = s.order.Sheet;
                                        //    orderModel.ShopName = s.shop.ShopName;
                                        //    orderModel.ShopNo = s.shop.ShopNo;
                                        //    orderModel.Region = s.shop.RegionName;
                                        //    orderModel.Province = s.shop.ProvinceName;
                                        //    orderModel.City = s.shop.CityName;
                                        //    orderModel.SubjectName = s.subject.SubjectName;
                                        //    orderModel.SubjectNo = s.subject.SubjectNo;
                                        //    orderModel.TotalPrice = double.Parse((s.order.TotalPrice ?? 0).ToString());
                                        //    orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                                        //    newOrderList.Add(orderModel);
                                        //}
                                        #endregion
                                        ////totalArea = (width * length) / 1000000 * Quantity;
                                        decimal area = ((width * length) / 1000000) * Quantity;
                                        if (GraphicMaterial.Contains("挂轴"))
                                        {

                                            if (GraphicMaterial.Contains("+挂轴") || GraphicMaterial.Contains("+上挂轴") || GraphicMaterial.Contains("+下挂轴"))
                                            {
                                                //如果是+挂轴，要加上挂轴面积
                                                area = ((width * length) / 1000000) * Quantity;
                                                if (GraphicMaterial.Contains("+挂轴"))
                                                    area += (width / 1000) * 2 * Quantity;
                                                else
                                                    area += (width / 1000) * Quantity;
                                            }
                                            else
                                            {
                                                //只有挂轴
                                                area = (width / 1000) * 2 * Quantity;
                                            }

                                        }

                                        orderModel = new OrderPriceDetail();
                                        orderModel.AddDate = s.order.AddDate;
                                        orderModel.PriceType = orderType;
                                        orderModel.Area = double.Parse(area.ToString());
                                        orderModel.Gender = s.order.Gender;
                                        orderModel.GraphicLength = double.Parse(length.ToString());
                                        orderModel.GraphicWidth = double.Parse(width.ToString());
                                        orderModel.GraphicMaterial = s.order.GraphicMaterial;
                                        orderModel.PositionDescription = s.order.PositionDescription;
                                        orderModel.Quantity = Quantity;
                                        orderModel.Sheet = s.order.Sheet;
                                        orderModel.ShopName = s.shop.ShopName;
                                        orderModel.ShopNo = s.shop.ShopNo;
                                        orderModel.Region = s.shop.RegionName;
                                        orderModel.Province = s.shop.ProvinceName;
                                        orderModel.City = s.shop.CityName;
                                        orderModel.SubjectName = s.subject.SubjectName;
                                        orderModel.SubjectNo = s.subject.SubjectNo;
                                        orderModel.TotalPrice = double.Parse((s.order.TotalPrice ?? 0).ToString());
                                        orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                                        orderModel.Channel = s.order.Channel;
                                        orderModel.Format = s.order.Format;
                                        orderModel.CustomServiceName = s.CSName;
                                        newOrderList.Add(orderModel);
                                    }
                                    else
                                    {

                                        orderModel = new OrderPriceDetail();
                                        orderModel.AddDate = s.order.AddDate;
                                        orderModel.PriceType = orderType;
                                        orderModel.Area = 0;
                                        orderModel.Gender = string.Empty;
                                        orderModel.GraphicLength = 0;
                                        orderModel.GraphicWidth = 0;
                                        orderModel.GraphicMaterial = string.Empty;
                                        orderModel.PositionDescription = string.Empty;
                                        orderModel.Quantity = s.order.Quantity ?? 1;
                                        orderModel.Sheet = string.Empty;
                                        orderModel.ShopName = s.shop.ShopName;
                                        orderModel.ShopNo = s.shop.ShopNo;
                                        orderModel.Region = s.shop.RegionName;
                                        orderModel.Province = s.shop.ProvinceName;
                                        orderModel.City = s.shop.CityName;
                                        orderModel.SubjectName = s.subject.SubjectName;
                                        orderModel.SubjectNo = s.subject.SubjectNo;
                                        orderModel.UnitPrice = 0;
                                        orderModel.TotalPrice = double.Parse(((s.order.OrderPrice ?? 0) * (s.order.Quantity ?? 1)).ToString());
                                        orderModel.Remark = s.order.Remark;
                                        orderModel.Channel = s.order.Channel;
                                        orderModel.Format = s.order.Format;
                                        orderModel.CustomServiceName = s.CSName;
                                        newOrderList.Add(orderModel);
                                    }
                                });
                            }
                        }
                        #endregion
                        #region 安装费
                        //if (isExportInstall == 1)
                        //{
                            var installPriceOrderList = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用 || s.order.OrderType == (int)OrderTypeEnum.印刷费).ToList();
                            installPriceOrderList.ForEach(s =>
                            {
                                string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                                orderModel = new OrderPriceDetail();
                                orderModel.AddDate = s.subject.AddDate;
                                orderModel.PriceType = orderType;
                                orderModel.Area = 0;
                                orderModel.Gender = string.Empty;
                                orderModel.GraphicLength = 0;
                                orderModel.GraphicWidth = 0;
                                orderModel.GraphicMaterial = string.Empty;
                                orderModel.PositionDescription = string.Empty;
                                orderModel.Quantity = s.order.Quantity ?? 1;
                                orderModel.Sheet = string.Empty;
                                orderModel.ShopName = s.shop.ShopName;
                                orderModel.ShopNo = s.shop.ShopNo;
                                orderModel.Region = s.shop.RegionName;
                                orderModel.Province = s.shop.ProvinceName;
                                orderModel.City = s.shop.CityName;
                                orderModel.SubjectName = s.subject.SubjectName;
                                orderModel.SubjectNo = s.subject.SubjectNo;
                                orderModel.UnitPrice = double.Parse((s.order.OrderPrice ?? 0).ToString()); ;
                                orderModel.TotalPrice = double.Parse(((s.order.OrderPrice ?? 0) * (s.order.Quantity ?? 1)).ToString());
                                orderModel.Remark = s.order.Remark;
                                orderModel.Channel = s.shop.Channel;
                                orderModel.Format = s.shop.Format;
                                newOrderList.Add(orderModel);
                            });
                            decimal totalInstallPrice = 0;
                            guidanceIdList.ForEach(gid =>
                            {
                                List<int> installShopIdList = orderList.Where(s => s.order.GuidanceId == gid && s.order.OrderType != (int)OrderTypeEnum.物料 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单).Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                                var installPriceDetailList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                                              join guidance in CurrentContext.DbContext.SubjectGuidance
                                                              on installShop.GuidanceId equals guidance.ItemId
                                                              join subject in CurrentContext.DbContext.Subject
                                                              on installShop.SubjectId equals subject.Id
                                                              where
                                                              installShop.GuidanceId == gid
                                                              && installShopIdList.Contains(installShop.ShopId ?? 0)
                                                              && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)//不统计分区增补的安装费
                                                              && (installShop.BasicPrice ?? 0) > 0
                                                              && allSubjectIdList.Contains(installShop.SubjectId ?? 0)
                                                              select installShop).ToList();

                                if (installPriceDetailList.Any())
                                {
                                    totalInstallPrice += (installPriceDetailList.Sum(s => ((s.BasicPrice ?? 0) + (s.WindowPrice ?? 0) + (s.OOHPrice ?? 0))));
                                }
                            });
                            if (totalInstallPrice > 0)
                            {
                                orderModel = new OrderPriceDetail();
                                orderModel.AddDate = null;
                                orderModel.PriceType = "活动安装费";
                                orderModel.Area = 0;
                                orderModel.Gender = string.Empty;
                                orderModel.GraphicLength = 0;
                                orderModel.GraphicWidth = 0;
                                orderModel.GraphicMaterial = string.Empty;
                                orderModel.PositionDescription = string.Empty;
                                orderModel.Quantity = 1;
                                orderModel.Sheet = string.Empty;
                                orderModel.ShopName = string.Empty;
                                orderModel.ShopNo = string.Empty;
                                orderModel.Region = string.Empty;
                                orderModel.Province = string.Empty;
                                orderModel.City = string.Empty;
                                orderModel.SubjectName = string.Empty;
                                orderModel.SubjectNo = string.Empty;
                                orderModel.UnitPrice = 0;
                                orderModel.TotalPrice = double.Parse(totalInstallPrice.ToString());

                                newOrderList.Add(orderModel);
                            }

                            //新开店装修费
                            var newShopInstallList = (from order in CurrentContext.DbContext.PriceOrderDetail
                                                      join subject in CurrentContext.DbContext.Subject
                                                      on order.SubjectId equals subject.Id
                                                      where subjectIdList.Contains(order.SubjectId ?? 0)
                                                      && (subject.SubjectType == (int)SubjectTypeEnum.新开店安装费 || subject.SubjectType == (int)SubjectTypeEnum.运费)
                                                      select new
                                                      {
                                                          order,
                                                          subject
                                                      }).ToList();
                            if (!string.IsNullOrWhiteSpace(regions))
                            {
                                regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                            }
                            else
                            {
                                new BasePage().GetResponsibleRegion.ForEach(s =>
                                {
                                    regionList.Add(s.ToLower());
                                });

                            }
                            if (regionList.Any())
                            {

                                newShopInstallList = newShopInstallList.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                                if (!string.IsNullOrWhiteSpace(provinces))
                                {
                                    provinceList = StringHelper.ToStringList(provinces, ',');
                                    if (provinceList.Any())
                                    {
                                        newShopInstallList = newShopInstallList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                                        if (!string.IsNullOrWhiteSpace(citys))
                                        {
                                            cityList = StringHelper.ToStringList(citys, ',');
                                            if (cityList.Any())
                                                newShopInstallList = newShopInstallList.Where(s => cityList.Contains(s.order.City)).ToList();
                                        }
                                    }
                                }
                            }

                            if (newShopInstallList.Any())
                            {

                                newShopInstallList.ForEach(s =>
                                {
                                    orderModel = new OrderPriceDetail();
                                    if (s.subject.AddDate != null)
                                        orderModel.AddDate = DateTime.Parse(s.subject.AddDate.ToString());
                                    orderModel.PriceType = CommonMethod.GeEnumName<SubjectTypeEnum>((s.subject.SubjectType ?? 0).ToString());
                                    orderModel.Area = 0;
                                    orderModel.Gender = string.Empty;
                                    orderModel.GraphicLength = 0;
                                    orderModel.GraphicWidth = 0;
                                    orderModel.GraphicMaterial = string.Empty;
                                    orderModel.PositionDescription = string.Empty;
                                    orderModel.Quantity = 1;
                                    orderModel.Sheet = string.Empty;
                                    orderModel.ShopName = s.order.ShopName;
                                    orderModel.ShopNo = "";
                                    orderModel.Region = s.order.Region;
                                    orderModel.Province = s.order.Province;
                                    orderModel.City = s.order.City;
                                    orderModel.SubjectName = s.subject.SubjectName;
                                    orderModel.SubjectNo = s.subject.SubjectNo;
                                    orderModel.UnitPrice = 0;
                                    orderModel.Remark = s.order.Remark;
                                    orderModel.TotalPrice = double.Parse((s.order.Amount ?? 0).ToString());
                                    newOrderList.Add(orderModel);
                                });
                            }
                        //}
                        #endregion
                        #region 快递费
                        var expressPriceOrderList = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费).ToList();
                        expressPriceOrderList.ForEach(s =>
                        {
                            string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = s.subject.AddDate;
                            orderModel.PriceType = orderType;
                            orderModel.Area = 0;
                            orderModel.Gender = string.Empty;
                            orderModel.GraphicLength = 0;
                            orderModel.GraphicWidth = 0;
                            orderModel.GraphicMaterial = string.Empty;
                            orderModel.PositionDescription = string.Empty;
                            orderModel.Quantity = 1;
                            orderModel.Sheet = string.Empty;
                            orderModel.ShopName = s.shop.ShopName;
                            orderModel.ShopNo = s.shop.ShopNo;
                            orderModel.Region = s.shop.RegionName;
                            orderModel.Province = s.shop.ProvinceName;
                            orderModel.City = s.shop.CityName;
                            orderModel.SubjectName = s.subject.SubjectName;
                            orderModel.SubjectNo = s.subject.SubjectNo;
                            orderModel.UnitPrice = 0;
                            orderModel.TotalPrice = double.Parse((s.order.OrderPrice ?? 0).ToString());
                            orderModel.Remark = s.order.Remark;
                            newOrderList.Add(orderModel);
                        });
                        decimal totalExpressPrice = 0;
                        List<int> totalShopIdList = popOrder.Select(s => s.shop.Id).Distinct().ToList();
                        var expressPriceDetailList = new ExpressPriceDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && totalShopIdList.Contains(s.ShopId ?? 0));
                        if (expressPriceDetailList.Any())
                        {
                            totalExpressPrice = expressPriceDetailList.Sum(s => s.ExpressPrice ?? 0);
                        }

                        if (totalExpressPrice > 0)
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
                            orderModel.PriceType = "活动快递费";
                            orderModel.Area = 0;
                            orderModel.Gender = string.Empty;
                            orderModel.GraphicLength = 0;
                            orderModel.GraphicWidth = 0;
                            orderModel.GraphicMaterial = string.Empty;
                            orderModel.PositionDescription = string.Empty;
                            orderModel.Quantity = 1;
                            orderModel.Sheet = string.Empty;
                            orderModel.ShopName = string.Empty;
                            orderModel.ShopNo = string.Empty;
                            orderModel.Region = string.Empty;
                            orderModel.Province = string.Empty;
                            orderModel.City = string.Empty;
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.TotalPrice = double.Parse(totalExpressPrice.ToString());
                            newOrderList.Add(orderModel);
                        }



                        #endregion
                    #endregion

                        #region 道具订单费用
                        var propOrderList = (from order in CurrentContext.DbContext.PropOrderDetail
                                             join subject in CurrentContext.DbContext.Subject
                                             on order.SubjectId equals subject.Id
                                             where guidanceIdList.Contains(order.GuidanceId ?? 0)
                                             select new { order, subject }).ToList();
                        if (!string.IsNullOrWhiteSpace(begin))
                        {
                            DateTime beginDate = DateTime.Parse(begin);
                            propOrderList = propOrderList.Where(s => s.subject.AddDate >= beginDate).ToList();
                            if (!string.IsNullOrWhiteSpace(end))
                            {
                                DateTime endDate = DateTime.Parse(end).AddDays(1);
                                propOrderList = propOrderList.Where(s => s.subject.AddDate < endDate).ToList();
                            }
                        }
                        if (subjectIdList.Any())
                        {
                            propOrderList = propOrderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                        }
                        if (propOrderList.Any())
                        {
                            propOrderList.ForEach(s =>
                            {
                                orderModel = new OrderPriceDetail();
                                orderModel.AddDate = s.subject.AddDate;
                                orderModel.PriceType = "道具费";
                                //orderModel.Area = double.Parse(totalArea.ToString());
                                //orderModel.Gender = s.order.Gender;
                                //orderModel.GraphicLength = double.Parse(length.ToString());
                                //orderModel.GraphicWidth = double.Parse(width.ToString());
                                orderModel.GraphicMaterial = s.order.MaterialType;
                                orderModel.PositionDescription = s.order.MaterialName;
                                orderModel.Quantity = s.order.Quantity ?? 1;
                                orderModel.Sheet = s.order.Sheet;
                                //orderModel.ShopName = s.shop.ShopName;
                                //orderModel.ShopNo = s.shop.ShopNo;
                                //orderModel.Region = s.shop.RegionName;
                                //orderModel.Province = s.shop.ProvinceName;
                                //orderModel.City = s.shop.CityName;
                                orderModel.SubjectName = s.subject.SubjectName;
                                orderModel.SubjectNo = s.subject.SubjectNo;
                                orderModel.TotalPrice = double.Parse(((s.order.Quantity ?? 1) * (s.order.UnitPrice ?? 0)).ToString());
                                orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                                newOrderList.Add(orderModel);
                            });
                        }
                        #endregion
                    }
                    catch(Exception ex)
                    { 
                    
                    }
                    #region 导出
                    if (newOrderList.Any())
                    {
                        string templateFileName = "项目费用统计明细";
                        string path = ConfigurationManager.AppSettings["ExportTemplate"];
                        path = path.Replace("fileName", templateFileName);
                        FileStream outFile = new FileStream(context1.Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                        IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                        ISheet sheet = workBook.GetSheet("Sheet1");
                        int startRow = 1;
                        newOrderList.ForEach(s =>
                        {
                            IRow dataRow = sheet.GetRow(startRow);
                            if (dataRow == null)
                                dataRow = sheet.CreateRow(startRow);
                            for (int i = 0; i < 25; i++)
                            {
                                ICell cell = dataRow.GetCell(i);
                                if (cell == null)
                                    cell = dataRow.CreateCell(i);

                            }
                            dataRow.GetCell(0).SetCellValue(s.PriceType);

                            dataRow.GetCell(1).SetCellValue(s.SubjectNo);
                            if (s.AddDate != null)
                                dataRow.GetCell(2).SetCellValue(DateTime.Parse(s.AddDate.ToString()).ToShortDateString());
                            else
                                dataRow.GetCell(2).SetCellValue("");
                            dataRow.GetCell(3).SetCellValue(s.ShopNo);
                            dataRow.GetCell(4).SetCellValue(s.ShopName);

                            dataRow.GetCell(5).SetCellValue(s.Region);
                            dataRow.GetCell(6).SetCellValue(s.Province);
                            dataRow.GetCell(7).SetCellValue(s.City);

                            dataRow.GetCell(8).SetCellValue(s.Channel);
                            dataRow.GetCell(9).SetCellValue(s.Format);
                            dataRow.GetCell(10).SetCellValue(s.CustomServiceName);

                            dataRow.GetCell(11).SetCellValue(s.Sheet);
                            dataRow.GetCell(12).SetCellValue(s.PositionDescription);
                            dataRow.GetCell(13).SetCellValue(s.Gender);
                            dataRow.GetCell(14).SetCellValue(s.GraphicMaterial);

                            dataRow.GetCell(15).SetCellValue(s.GraphicWidth);
                            dataRow.GetCell(16).SetCellValue(s.GraphicLength);
                            dataRow.GetCell(17).SetCellValue(s.Area);
                            dataRow.GetCell(18).SetCellValue(s.UnitPrice);
                            dataRow.GetCell(19).SetCellValue(s.Quantity);
                            dataRow.GetCell(20).SetCellValue(s.TotalPrice);
                            dataRow.GetCell(21).SetCellValue(s.SubjectName);
                            dataRow.GetCell(22).SetCellValue(s.Remark);
                            startRow++;
                        });
                        HttpCookie cookie = context1.Request.Cookies["项目费用统计明细"];
                        if (cookie == null)
                        {
                            cookie = new HttpCookie("项目费用统计明细");
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
                            OperateFile.DownLoadFile(ms, guidanceName + fileName);

                        }
                    }

                    else
                    {
                        HttpCookie cookie = context1.Request.Cookies["项目费用统计明细"];
                        if (cookie == null)
                        {
                            cookie = new HttpCookie("项目费用统计明细");
                        }
                        cookie.Value = "1";
                        cookie.Expires = DateTime.Now.AddMinutes(30);
                        context1.Response.Cookies.Add(cookie);
                    }
                    #endregion
                }
                else
                {
                    HttpCookie cookie = context1.Request.Cookies["项目费用统计明细"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("项目费用统计明细");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    context1.Response.Cookies.Add(cookie);
                }
            }


        }

        //按店统计导出
        void ExportByShop()
        {
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                List<int> guidanceIdList = new List<int>();
                List<int> subjectIdList = new List<int>();
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<string> shopTypeList = new List<string>();
                List<int> customerServiceList = new List<int>();
                List<int> subjectCategoryList = new List<int>();
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
                if (!string.IsNullOrWhiteSpace(subjectIds))
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');

                if (!string.IsNullOrWhiteSpace(customerServiceIds))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceIds, ',');
                }
                if (!string.IsNullOrWhiteSpace(subjectCategory))
                {
                    subjectCategoryList = StringHelper.ToIntList(subjectCategory, ',');
                }

                string guidanceName = string.Empty;
                //List<OrderPriceDetail> newOrderList = new List<OrderPriceDetail>();
                //OrderPriceDetail orderModel;
                if (guidanceIdList.Any())
                {

                    if (!string.IsNullOrWhiteSpace(regions))
                    {
                        regionList = StringHelper.ToStringList(regions, ',', LowerUpperEnum.ToLower);
                    }
                    else
                    {
                        new BasePage().GetResponsibleRegion.ForEach(s =>
                        {
                            regionList.Add(s.ToLower());
                        });

                    }
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join guindance in CurrentContext.DbContext.SubjectGuidance
                                     on order.GuidanceId equals guindance.ItemId
                                     
                                     where guidanceIdList.Contains(order.GuidanceId ?? 0)
                                    && (subject.IsDelete == null || subject.IsDelete == false)
                                    && (subject.ApproveState == 1)
                                    && (order.IsDelete == null || order.IsDelete == false)
                                     && (regionList.Any() ? regionList.Contains(order.Region.ToLower()) : 1 == 1)
                                     select new
                                     {
                                         order,
                                         shop,
                                         subject,
                                         guindance,
                                        
                                     }).ToList();

                    if (!string.IsNullOrWhiteSpace(begin))
                    {
                        DateTime beginDate = DateTime.Parse(begin);
                        if (dateSearchType == 1)
                        {
                            orderList = orderList.Where(s => s.subject.AddDate >= beginDate).ToList();
                            if (!string.IsNullOrWhiteSpace(end))
                            {
                                DateTime endDate = DateTime.Parse(end).AddDays(1);
                                orderList = orderList.Where(s => s.subject.AddDate < endDate).ToList();
                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => s.order.AddDate >= beginDate).ToList();
                            if (!string.IsNullOrWhiteSpace(end))
                            {
                                DateTime endDate = DateTime.Parse(end).AddDays(1);
                                orderList = orderList.Where(s => s.order.AddDate < endDate).ToList();
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(shopType))
                    {
                        shopTypeList = StringHelper.ToStringList(shopType, ',');

                    }
                    if (subjectCategoryList.Any())
                    {
                        if (subjectCategoryList.Contains(0))
                        {
                            subjectCategoryList.Remove(0);
                            if (subjectCategoryList.Any())
                            {
                                orderList = orderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();

                            }
                            else
                            {
                                orderList = orderList.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();

                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();

                        }
                    }
                    if (subjectChannel == 1)
                    {
                        //上海系统单
                        orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();

                    }
                    else if (subjectChannel == 2)
                    {
                        //分区订单
                        orderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();

                    }

                    if (shopTypeList.Any())
                    {
                        if (shopTypeList.Contains("空"))
                        {
                            shopTypeList.Remove("空");
                            if (shopTypeList.Any())
                            {
                                orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();

                            }
                            else
                            {
                                orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();

                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();

                        }
                    }
                    if (subjectIdList.Any())
                    {
                        orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId??0)).ToList();

                    }


                    if (regionList.Any())
                    {
                        if (!string.IsNullOrWhiteSpace(provinces))
                        {
                            provinceList = StringHelper.ToStringList(provinces, ',');
                            if (provinceList.Any())
                            {
                                orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();

                                if (!string.IsNullOrWhiteSpace(citys))
                                {
                                    cityList = StringHelper.ToStringList(citys, ',');
                                    if (cityList.Any())
                                    {
                                        orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();

                                    }
                                }
                            }
                        }
                    }
                    if (customerServiceList.Any())
                    {
                        if (customerServiceList.Contains(0))
                        {
                            customerServiceList.Remove(0);
                            if (customerServiceList.Any())
                            {
                                orderList = orderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                            }
                            else
                            {
                                orderList = orderList.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();

                        }
                    }
                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        if (status == "0")
                        {
                            orderList = orderList.Where(s => s.order.ShopStatus == null || s.order.ShopStatus == "" || s.order.ShopStatus == "正常").ToList();

                        }
                        else if (status == "1")
                        {
                            orderList = orderList.Where(s => s.order.ShopStatus != null && s.order.ShopStatus.Contains("闭")).ToList();

                        }
                    }
                    if (orderList.Any())
                    {
                        List<Shop> shopList = orderList.Select(s => s.shop).Distinct().OrderBy(s => s.ShopNo).ToList();
                        List<int> shopIdList = shopList.Select(s => s.Id).ToList();
                        List<InstallPriceShopInfo> installPriceList = new InstallPriceShopInfoBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (subjectIdList.Any()?subjectIdList.Contains(s.SubjectId??0):1==1) && shopIdList.Contains(s.ShopId ?? 0));
                        List<Models.ExpressPriceDetail> expressPriceList=new ExpressPriceDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && shopIdList.Contains(s.ShopId??0));
                        Dictionary<int, string> userDic = new Dictionary<int, string>();
                        UserBLL userBll = new UserBLL();
                        for (int i = 0; i < shopList.Count; i++)
                        {
                            Shop model = shopList[i];
                            var shopOrderList = orderList.Where(s => s.order.ShopId == model.Id).Select(s => s.order).ToList();
                            decimal popPrice = 0;
                            decimal area = 0;
                            decimal installPrice = 0;
                            decimal measurePrice = 0;
                            decimal expressPrice = 0;
                            decimal materialPrice = 0;
                            decimal otherPrice = 0;

                            new BasePage().StatisticPOPTotalPrice(shopOrderList, out popPrice, out area);
                            model.POPPrice = popPrice;
                            model.POPArea = area;
                            var installOrderList = shopOrderList.Where(s=>s.OrderType==(int)OrderTypeEnum.安装费 && (s.OrderPrice??0)>0).ToList();
                            if (installOrderList.Any())
                            {
                                installPrice += (installOrderList.Sum(s => s.OrderPrice ?? 0));
                            }
                            var oneShopInstallPriceList = installPriceList.Where(s => s.ShopId == model.Id).ToList();
                            if (oneShopInstallPriceList.Any())
                            {
                                installPrice += (oneShopInstallPriceList.Sum(s => ((s.BasicPrice ?? 0)) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0)));
                            }
                            model.InstallPrice = installPrice;
                            measurePrice = shopOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费 && (s.OrderPrice ?? 0) > 0).Sum(s => s.OrderPrice ?? 0);
                            model.MeasurePrice = measurePrice;
                            expressPrice = shopOrderList.Where(s => (s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.快递费 || s.OrderType == (int)OrderTypeEnum.运费) && (s.OrderPrice ?? 0) > 0).Sum(s => s.OrderPrice ?? 0);
                            var oneExpressPriceList = expressPriceList.Where(s => s.ShopId == model.Id).ToList();
                            if (oneExpressPriceList.Any())
                            {
                                expressPrice += (oneExpressPriceList.Sum(s =>s.ExpressPrice??0));
                            }
                            model.ExpressPrice=expressPrice;
                            materialPrice = shopOrderList.Where(s => (s.OrderType == (int)OrderTypeEnum.物料 || s.OrderType == (int)OrderTypeEnum.道具) && (s.OrderPrice ?? 0) > 0).Sum(s => s.OrderPrice ?? 0);
                            model.MaterialPrice = materialPrice;
                            otherPrice = shopOrderList.Where(s => (s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费) && (s.OrderPrice ?? 0) > 0).Sum(s => ((s.OrderPrice ?? 0)*(s.Quantity??1)));
                            model.OtherPrice = otherPrice;
                            int csUserId = model.CSUserId ?? 0;
                            string csUserName = string.Empty;
                            if (userDic.Keys.Contains(csUserId))
                            {
                                csUserName = userDic[csUserId];
                            }
                            else
                            {
                                UserInfo userModel = userBll.GetModel(csUserId);
                                if (userModel != null)
                                {
                                    csUserName = userModel.RealName;
                                    userDic.Add(csUserId, csUserName);
                                }
                            }
                            model.CSUserName = csUserName;
                        }
                        string templateFileName = "项目统计-按店统计";
                        string path = ConfigurationManager.AppSettings["ExportTemplate"];
                        path = path.Replace("fileName", templateFileName);
                        FileStream outFile = new FileStream(context1.Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                        IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                        ISheet sheet = workBook.GetSheet("Sheet1");
                        int startRow = 1;
                        shopList.ForEach(s =>
                        {
                            IRow dataRow = sheet.GetRow(startRow);
                            if (dataRow == null)
                                dataRow = sheet.CreateRow(startRow);
                            for (int i = 0; i < 20; i++)
                            {
                                ICell cell = dataRow.GetCell(i);
                                if (cell == null)
                                    cell = dataRow.CreateCell(i);

                            }
                            dataRow.GetCell(0).SetCellValue(s.ShopNo);
                            dataRow.GetCell(1).SetCellValue(s.ShopName);
                            dataRow.GetCell(2).SetCellValue(s.RegionName);
                            dataRow.GetCell(3).SetCellValue(s.ProvinceName);
                            dataRow.GetCell(4).SetCellValue(s.CityName);
                            dataRow.GetCell(5).SetCellValue(s.CityTier);
                            dataRow.GetCell(6).SetCellValue(s.IsInstall);
                            dataRow.GetCell(7).SetCellValue(s.CSUserName);
                            dataRow.GetCell(8).SetCellValue(s.Channel);
                            dataRow.GetCell(9).SetCellValue(s.Format);
                            dataRow.GetCell(10).SetCellValue(double.Parse(s.POPArea.ToString()));
                            dataRow.GetCell(11).SetCellValue(double.Parse(s.POPPrice.ToString()));
                            dataRow.GetCell(12).SetCellValue(double.Parse(s.InstallPrice.ToString()));
                            dataRow.GetCell(13).SetCellValue(double.Parse(s.MeasurePrice.ToString()));
                            dataRow.GetCell(14).SetCellValue(double.Parse(s.ExpressPrice.ToString()));
                            dataRow.GetCell(15).SetCellValue(double.Parse(s.MaterialPrice.ToString()));
                            dataRow.GetCell(16).SetCellValue(double.Parse(s.OtherPrice.ToString()));
                            decimal total = s.POPPrice + (s.InstallPrice??0) + s.MeasurePrice + s.ExpressPrice + s.MaterialPrice + s.OtherPrice;
                            total = Math.Round(total, 2);
                            dataRow.GetCell(17).SetCellValue(double.Parse(total.ToString()));
                            startRow++;
                        });
                        HttpCookie cookie = context1.Request.Cookies["项目费用统计-按店统计"];
                        if (cookie == null)
                        {
                            cookie = new HttpCookie("项目费用统计-按店统计");
                        }
                        cookie.Value = "1";
                        cookie.Expires = DateTime.Now.AddMinutes(30);
                        context1.Response.Cookies.Add(cookie);
                        string fileName = "项目费用统计-按店统计";
                        using (MemoryStream ms = new MemoryStream())
                        {
                            workBook.Write(ms);
                            ms.Flush();
                            sheet = null;
                            workBook = null;
                            OperateFile.DownLoadFile(ms,fileName);

                        }
                    }
                }
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;
using System.Text;
using Models;
using System.Configuration;
using System.IO;
using NPOI.SS.UserModel;

namespace WebApp.OutsourcingOrder.Statistics
{
    public partial class ExportHelper : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string type = string.Empty;
            if (Request.QueryString["type"] != null)
            {
                type = Request.QueryString["type"];
            }
            if (type == "bydate")
            {
                ExportDetailByDate();
            }
            else
            {
                ExportDetail();
            }
        }

        void ExportDetail()
        {
            
            //int outsourceId = 0;
            string guidanceIds = string.Empty;
            string outsourceId = string.Empty;
            string subjectIds = string.Empty;
            string outsourceType = string.Empty;
            string province = string.Empty;
            string city = string.Empty;
            string guidanceMonth = string.Empty;
            string outsourceName = string.Empty;
            List<int> outsourceList = new List<int>();
            if (Request.QueryString["outsourceId"] != null)
            {
                outsourceId = Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = Request.QueryString["guidanceIds"];
            }
            if (Request.QueryString["subjectIds"] != null)
            {
                subjectIds = Request.QueryString["subjectIds"];
            }
            if (Request.QueryString["outsourceType"] != null)
            {
                outsourceType =Request.QueryString["outsourceType"];
            }
            if (Request.QueryString["province"] != null)
            {
                province = Request.QueryString["province"];
            }
            if (Request.QueryString["city"] != null)
            {
                city = Request.QueryString["city"];
            }
            if (Request.QueryString["outsourceName"] != null)
            {
                outsourceName = Request.QueryString["outsourceName"];
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
            List<string> provinceList = new List<string>();
            if (!string.IsNullOrWhiteSpace(province))
            {
                provinceList = StringHelper.ToStringList(province, ',');
            }
            List<string> cityList = new List<string>();
            if (!string.IsNullOrWhiteSpace(city))
            {
                cityList = StringHelper.ToStringList(city, ',');
            }
            List<OrderPriceDetail> newOrderList = new List<OrderPriceDetail>();
            #region
            guidanceList.ForEach(gid => {

                //是否全部三叶草
                bool isBCSSubject = false;

                var orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                  join guidance in CurrentContext.DbContext.SubjectGuidance
                                  on order.GuidanceId equals guidance.ItemId
                                  join outsource in CurrentContext.DbContext.Company
                                  on order.OutsourceId equals outsource.Id
                                  join subject1 in CurrentContext.DbContext.Subject
                                  on order.SubjectId equals subject1.Id into subjectTemp
                                  from subject in subjectTemp.DefaultIfEmpty()
                                 where outsourceList.Contains(order.OutsourceId ?? 0)
                                 && order.GuidanceId==gid
                                 && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength!=null && order.GraphicLength>1 && order.GraphicWidth!=null && order.GraphicWidth>1) || order.OrderType>1)
                                 && (order.IsDelete == null || order.IsDelete == false)
                                  select new
                                 {
                                     order,
                                     //assign,
                                     subject,
                                     guidance,
                                     outsource
                                 }).ToList();
                var orderList = orderList0;
                if (subjectList.Any())
                {
                    orderList = orderList.Where(s => subjectList.Contains(s.order.SubjectId ?? 0)).ToList();
                    var NotBCSSubjectList = new SubjectBLL().GetList(s => subjectList.Contains(s.Id) && (s.CornerType == null || !s.CornerType.Contains("三叶草")));
                    isBCSSubject = !NotBCSSubjectList.Any();
                }
                if (provinceList.Any())
                {
                    orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
                if (cityList.Any())
                {
                    orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
                }
                if (outsourceTypeList.Any())
                {
                    orderList = orderList.Where(s => outsourceTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                }
                

                if (orderList.Any())
                {
                    List<int> shopIdList = orderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();


                    List<int> installShopIdList = shopIdList;
                    if (isBCSSubject)
                    {
                        //如果是三叶草，把出现在大货订单里面的店铺去掉
                        List<int> totalOrderShopIdList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                                          join subject in CurrentContext.DbContext.Subject
                                                          on order.SubjectId equals subject.Id
                                                          where order.GuidanceId == gid
                                                          && !subjectList.Contains(order.SubjectId ?? 0)
                                                          && subject.ApproveState == 1
                                                          && (subject.IsDelete == null || subject.IsDelete == false)
                                                          && (order.IsDelete == null || order.IsDelete == false)
                                                          && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                                                          select order.ShopId ?? 0).Distinct().ToList();
                        installShopIdList = installShopIdList.Except(totalOrderShopIdList).ToList();

                    }


                    OrderPriceDetail orderModel;
                   
                    orderList = orderList.Where(s => s.order.SubjectId > 0).ToList();
                    orderList.ForEach(s =>
                    {
                        string gender = s.order.Gender;
                        string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                        string orderPrice = string.Empty;
                        int Quantity = s.order.Quantity ?? 1;

                        decimal width = s.order.GraphicWidth ?? 0;
                        decimal length = s.order.GraphicLength ?? 0;
                        decimal totalArea = (width * length) / 1000000 * Quantity;
                        orderModel = new OrderPriceDetail();
                        if (s.subject != null && s.subject.AddDate != null)
                        {
                            orderModel.AddDate = s.subject.AddDate;
                        }
                        orderModel.PriceType = orderType;
                        orderModel.Area = double.Parse(totalArea.ToString());
                        orderModel.Gender =s.order.Gender;
                        orderModel.GraphicLength = double.Parse(length.ToString());
                        orderModel.GraphicWidth = double.Parse(width.ToString());
                        orderModel.GraphicMaterial = s.order.GraphicMaterial;
                        orderModel.PositionDescription = s.order.PositionDescription;
                        orderModel.Quantity = Quantity;
                        orderModel.Sheet = s.order.Sheet;
                        orderModel.ShopName = s.order.ShopName;
                        orderModel.ShopNo = s.order.ShopNo;
                        orderModel.SubjectName = s.subject.SubjectName;
                        orderModel.SubjectNo = s.subject.SubjectNo;
                        if ((s.order.PayOrderPrice ?? 0) > 0)
                        {
                            //orderPrice = (s.order.OrderPrice ?? 0).ToString();
                            orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.Gender = string.Empty;
                            orderModel.Sheet = string.Empty;
                            orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                        }
                        else
                        {
                            orderModel.TotalPrice = double.Parse((s.order.TotalPrice ?? 0).ToString());
                            orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                            orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveUnitPrice ?? 0).ToString());
                            orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveTotalPrice ?? 0).ToString());
                        }
                        orderModel.Remark = s.order.Remark;
                        orderModel.GuidanceName = s.guidance.ItemName;
                        orderModel.OutsourceName = s.outsource.CompanyName;
                        newOrderList.Add(orderModel);

                    });
                    var assignShopList = (from assign in CurrentContext.DbContext.OutsourceAssignShop
                                          join shop in CurrentContext.DbContext.Shop
                                          on assign.ShopId equals shop.Id
                                          where assign.GuidanceId==gid
                                          && shopIdList.Contains(assign.ShopId ?? 0)
                                          && outsourceList.Contains(assign.OutsourceId ?? 0)
                                          && ((assign.PayInstallPrice ?? 0) > 0 || (assign.PayExpressPrice ?? 0) > 0)
                                          select new
                                          {
                                              assign,
                                              shop
                                          }).ToList();
                    if (assignShopList.Any())
                    {
                        assignShopList.ForEach(s =>
                        {
                            if ((s.assign.PayInstallPrice ?? 0) > 0)
                            {
                                orderModel = new OrderPriceDetail();
                                orderModel.AddDate = null;
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
                                orderModel.SubjectName = string.Empty;
                                orderModel.SubjectNo = string.Empty;
                                orderModel.UnitPrice = 0;


                                orderModel.PriceType = "安装费";
                                orderModel.TotalPrice = double.Parse((s.assign.PayInstallPrice ?? 0).ToString());
                                orderModel.UnitPrice = double.Parse((s.assign.PayInstallPrice ?? 0).ToString());
                                orderModel.ReceiveUnitPrice = double.Parse((s.assign.ReceiveInstallPrice ?? 0).ToString());
                                orderModel.ReceiveTotalPrice = double.Parse((s.assign.ReceiveInstallPrice ?? 0).ToString());
                                newOrderList.Add(orderModel);
                            }
                            if ((s.assign.PayExpressPrice ?? 0) > 0)
                            {
                                orderModel = new OrderPriceDetail();
                                orderModel.AddDate = null;
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
                                orderModel.SubjectName = string.Empty;
                                orderModel.SubjectNo = string.Empty;
                                orderModel.UnitPrice = 0;
                                orderModel.PriceType = "发货费";
                                orderModel.TotalPrice = double.Parse((s.assign.PayExpressPrice ?? 0).ToString());
                                orderModel.UnitPrice = double.Parse((s.assign.PayExpressPrice ?? 0).ToString());
                                orderModel.ReceiveUnitPrice = double.Parse((s.assign.ReceiveExpresslPrice ?? 0).ToString());
                                orderModel.ReceiveTotalPrice = double.Parse((s.assign.ReceiveExpresslPrice ?? 0).ToString());
                                newOrderList.Add(orderModel);
                            }
                           
                        });
                    }

                    var installPriceList = orderList0.Where(s => s.order.SubjectId == 0 && installShopIdList.Contains(s.order.ShopId ?? 0) && s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                    if (outsourceTypeList.Any())
                    {
                        installPriceList = installPriceList.Where(s => outsourceTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                    }
                    if (installPriceList.Any())
                    {
                        installPriceList.ForEach(s =>
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
                            orderModel.Area = 0;
                            orderModel.Gender = string.Empty;
                            orderModel.GraphicLength = 0;
                            orderModel.GraphicWidth = 0;
                            orderModel.GraphicMaterial = string.Empty;
                            orderModel.PositionDescription = string.Empty;
                            orderModel.Quantity = 1;
                            orderModel.Sheet = string.Empty;
                            orderModel.ShopName = s.order.ShopName;
                            orderModel.ShopNo = s.order.ShopNo;
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.PriceType = "安装费";
                            orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            orderModel.GuidanceName = s.guidance.ItemName;
                            orderModel.OutsourceName = s.outsource.CompanyName;
                            newOrderList.Add(orderModel);
                        });
                    }
                    //var expressPriceList = new OutsourceOrderDetailBLL().GetList(s => s.GuidanceId == gid && outsourceList.Contains(s.OutsourceId ?? 0) && s.SubjectId == 0 && shopIdList.Contains(s.ShopId ?? 0) && s.OrderType == (int)OrderTypeEnum.发货费).ToList();
                    var expressPriceList = orderList0.Where(s => s.order.SubjectId == 0 && shopIdList.Contains(s.order.ShopId ?? 0) && s.order.OrderType == (int)OrderTypeEnum.发货费).ToList();
                    if (outsourceTypeList.Any())
                    {
                        expressPriceList = expressPriceList.Where(s => outsourceTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                    }
                    if (expressPriceList.Any())
                    {
                        expressPriceList.ForEach(s =>
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
                            orderModel.Area = 0;
                            orderModel.Gender = string.Empty;
                            orderModel.GraphicLength = 0;
                            orderModel.GraphicWidth = 0;
                            orderModel.GraphicMaterial = string.Empty;
                            orderModel.PositionDescription = string.Empty;
                            orderModel.Quantity = 1;
                            orderModel.Sheet = string.Empty;
                            orderModel.ShopName = s.order.ShopName;
                            orderModel.ShopNo = s.order.ShopNo;
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.PriceType = "发货费";
                            orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            orderModel.GuidanceName = s.guidance.ItemName;
                            orderModel.OutsourceName = s.outsource.CompanyName;
                            newOrderList.Add(orderModel);
                        });
                    }


                }
            });
            #endregion
            #region 特殊外协费用
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                decimal installPrice = 0;
                decimal measurePrice = 0;
                decimal expressPrice = 0;
                decimal otherPrice = 0;
                DateTime date0 = DateTime.Parse(guidanceMonth);
                int year = date0.Year;
                int month = date0.Month;
                var outsourcePriceOrderList = new OutsourcePriceOrderBLL().GetList(s => outsourceList.Contains(s.OutsourceId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                if (outsourcePriceOrderList.Any())
                {
                    installPrice = outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).Sum(s => s.PayPrice ?? 0);
                    measurePrice = outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).Sum(s => s.PayPrice ?? 0);
                    expressPrice = outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费).Sum(s => s.PayPrice ?? 0);
                    otherPrice = outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用).Sum(s => s.PayPrice ?? 0);
                }
                if (installPrice > 0)
                {
                    OrderPriceDetail orderModel = new OrderPriceDetail();
                    orderModel.AddDate = null;
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
                    orderModel.SubjectName = string.Empty;
                    orderModel.SubjectNo = string.Empty;
                    orderModel.UnitPrice = 0;
                    orderModel.PriceType = "安装费-实报实销";
                    orderModel.TotalPrice = double.Parse(installPrice.ToString());
                    orderModel.UnitPrice = double.Parse(installPrice.ToString());
                    orderModel.ReceiveUnitPrice =0;
                    orderModel.ReceiveTotalPrice = 0;
                    newOrderList.Add(orderModel);
                }
                if (measurePrice > 0)
                {
                    OrderPriceDetail orderModel = new OrderPriceDetail();
                    orderModel.AddDate = null;
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
                    orderModel.SubjectName = string.Empty;
                    orderModel.SubjectNo = string.Empty;
                    orderModel.UnitPrice = 0;
                    orderModel.PriceType = "测量费-实报实销";
                    orderModel.TotalPrice = double.Parse(measurePrice.ToString());
                    orderModel.UnitPrice = double.Parse(measurePrice.ToString());
                    orderModel.ReceiveUnitPrice = 0;
                    orderModel.ReceiveTotalPrice = 0;
                    newOrderList.Add(orderModel);
                }
                if (expressPrice > 0)
                {
                    OrderPriceDetail orderModel = new OrderPriceDetail();
                    orderModel.AddDate = null;
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
                    orderModel.SubjectName = string.Empty;
                    orderModel.SubjectNo = string.Empty;
                    orderModel.UnitPrice = 0;
                    orderModel.PriceType = "发货费-实报实销";
                    orderModel.TotalPrice = double.Parse(expressPrice.ToString());
                    orderModel.UnitPrice = double.Parse(expressPrice.ToString());
                    orderModel.ReceiveUnitPrice = 0;
                    orderModel.ReceiveTotalPrice = 0;
                    newOrderList.Add(orderModel);
                }
                if (otherPrice > 0)
                {
                    OrderPriceDetail orderModel = new OrderPriceDetail();
                    orderModel.AddDate = null;
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
                    orderModel.SubjectName = string.Empty;
                    orderModel.SubjectNo = string.Empty;
                    orderModel.UnitPrice = 0;
                    orderModel.PriceType = "其他费用-实报实销";
                    orderModel.TotalPrice = double.Parse(otherPrice.ToString());
                    orderModel.UnitPrice = double.Parse(otherPrice.ToString());
                    orderModel.ReceiveUnitPrice = 0;
                    orderModel.ReceiveTotalPrice = 0;
                    newOrderList.Add(orderModel);
                }
            }
            #endregion
            //
            if (newOrderList.Any())
            {
                string templateFileName = "外协项目费用统计明细";
                //StringBuilder templateFileName = new StringBuilder("外协项目费用统计明细");

                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                ISheet sheet = workBook.GetSheet("Sheet1");
                int startRow = 1;
                newOrderList.OrderBy(s => s.ShopNo).ToList().ForEach(s =>
                {
                    IRow dataRow = sheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = sheet.CreateRow(startRow);
                    for (int i = 0; i < 21; i++)
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
                    dataRow.GetCell(5).SetCellValue(s.Sheet);
                    dataRow.GetCell(6).SetCellValue(s.PositionDescription);
                    dataRow.GetCell(7).SetCellValue(s.Gender);
                    dataRow.GetCell(8).SetCellValue(s.GraphicWidth);
                    dataRow.GetCell(9).SetCellValue(s.GraphicLength);
                    dataRow.GetCell(10).SetCellValue(s.Area);
                    dataRow.GetCell(11).SetCellValue(s.GraphicMaterial);
                    dataRow.GetCell(12).SetCellValue(s.UnitPrice);
                    dataRow.GetCell(13).SetCellValue(s.ReceiveUnitPrice);
                    dataRow.GetCell(14).SetCellValue(s.Quantity);
                    dataRow.GetCell(15).SetCellValue(s.TotalPrice);
                    dataRow.GetCell(16).SetCellValue(s.ReceiveTotalPrice);
                    dataRow.GetCell(17).SetCellValue(s.GuidanceName);
                    dataRow.GetCell(18).SetCellValue(s.SubjectName);
                    dataRow.GetCell(19).SetCellValue(s.Remark);
                    dataRow.GetCell(20).SetCellValue(s.OutsourceName);
                    startRow++;
                });
                //HttpCookie cookie = Request.Cookies["项目费用统计明细"];
                //if (cookie == null)
                //{
                //    cookie = new HttpCookie("项目费用统计明细");
                //}
                //cookie.Value = "1";
                //cookie.Expires = DateTime.Now.AddMinutes(30);
                //Response.Cookies.Add(cookie);
                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    sheet = null;
                    workBook = null;
                    StringBuilder fileName = new StringBuilder("外协费用统计明细");
                    if (!string.IsNullOrWhiteSpace(outsourceName))
                    {
                        fileName.Append("-");
                        fileName.Append(outsourceName);
                    }
                    OperateFile.DownLoadFile(ms, fileName.ToString());

                }
            }

            //else
            //{
            //    HttpCookie cookie = Request.Cookies["项目费用统计明细"];
            //    if (cookie == null)
            //    {
            //        cookie = new HttpCookie("项目费用统计明细");
            //    }
            //    cookie.Value = "1";
            //    cookie.Expires = DateTime.Now.AddMinutes(30);
            //    Response.Cookies.Add(cookie);
            //}
        }

        void ExportDetailByDate()
        {
            string beginDateStr = string.Empty;
            string endDateStr = string.Empty;
            string provinces = string.Empty;
            List<string> provinceList = new List<string>();
            string citys = string.Empty;
            List<string> cityList = new List<string>();
            string assignType = string.Empty;
            List<int> assignTypeList = new List<int>();
            int customerId = 0;
            int outsourceId = 0;
            string outsourceName = string.Empty;
            if (Request.QueryString["beginDate"] != null)
            {
                beginDateStr = Request.QueryString["beginDate"];
            }
            if (Request.QueryString["endDate"] != null)
            {
                endDateStr = Request.QueryString["endDate"];
            }
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);
            }
            if (Request.QueryString["outsourceId"] != null)
            {
                outsourceId = int.Parse(Request.QueryString["outsourceId"]);
            }
            if (Request.QueryString["province"] != null)
            {
                provinces = Request.QueryString["province"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (Request.QueryString["city"] != null)
            {
                citys = Request.QueryString["city"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (Request.QueryString["assignType"] != null)
            {
                assignType = Request.QueryString["assignType"];
                if (!string.IsNullOrWhiteSpace(assignType))
                {
                    assignTypeList = StringHelper.ToIntList(assignType, ',');
                }
            }
            if (Request.QueryString["outsourceName"] != null)
            {
                outsourceName = Request.QueryString["outsourceName"];
            }
            var orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                              join subject in CurrentContext.DbContext.Subject
                              on order.SubjectId equals subject.Id
                              where (subject.IsDelete == null || subject.IsDelete == false)
                              && order.OutsourceId == outsourceId
                              && subject.CustomerId == customerId
                              && (order.IsDelete == null || order.IsDelete == false)
                              select new
                              {
                                  order,
                                  subject
                              }).ToList();
            if (!string.IsNullOrWhiteSpace(beginDateStr) && StringHelper.IsDateTime(beginDateStr))
            {
                DateTime beginDate = DateTime.Parse(beginDateStr);
                orderList0 = orderList0.Where(s => s.subject.AddDate >= beginDate).ToList();
            }
            if (!string.IsNullOrWhiteSpace(endDateStr) && StringHelper.IsDateTime(endDateStr))
            {
                DateTime endDate = DateTime.Parse(endDateStr);
                orderList0 = orderList0.Where(s => s.subject.AddDate < endDate).ToList();
            }
            if (provinceList.Any())
            {
                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orderList0 = orderList0.Where(s => provinceList.Contains(s.order.Province) || (s.order.Province == null || s.order.Province == "")).ToList();
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s => (s.order.Province == null || s.order.Province == "")).ToList();
                    }
                }
                else
                {
                    orderList0 = orderList0.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        orderList0 = orderList0.Where(s => cityList.Contains(s.order.City) || (s.order.City == null || s.order.City == "")).ToList();
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s => (s.order.City == null || s.order.City == "")).ToList();
                    }
                }
                else
                {
                    orderList0 = orderList0.Where(s => cityList.Contains(s.order.City)).ToList();
                }
            }
            if (assignTypeList.Any())
            {
                orderList0 = orderList0.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
            }
            if (orderList0.Any())
            {
                List<int> guidanceIdList = orderList0.Select(s => s.order.GuidanceId ?? 0).ToList();
                List<OrderPriceDetail> newOrderList = new List<OrderPriceDetail>();
                guidanceIdList.ForEach(gid =>
                {
                    var orderList = orderList0.Where(s => s.order.GuidanceId == gid).ToList();
                    List<int> shopIdList = orderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                    OrderPriceDetail orderModel;
                    var popOrderList = orderList.Where(s => s.order.SubjectId > 0).ToList();
                    var priceOrderList = orderList.Where(s => s.order.SubjectId == 0).ToList();
                    //pop订单
                    popOrderList.ForEach(s =>
                    {
                        string gender = s.order.Gender;
                        string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                        string orderPrice = string.Empty;
                        int Quantity = s.order.Quantity ?? 1;

                        decimal width = s.order.GraphicWidth ?? 0;
                        decimal length = s.order.GraphicLength ?? 0;
                        decimal totalArea = (width * length) / 1000000 * Quantity;
                        orderModel = new OrderPriceDetail();
                        if (s.subject != null && s.subject.AddDate != null)
                        {
                            orderModel.AddDate = s.subject.AddDate;
                        }
                        orderModel.PriceType = orderType;
                        orderModel.Area = double.Parse(totalArea.ToString());
                        orderModel.Gender = s.order.Gender;
                        orderModel.GraphicLength = double.Parse(length.ToString());
                        orderModel.GraphicWidth = double.Parse(width.ToString());
                        orderModel.GraphicMaterial = s.order.GraphicMaterial;
                        orderModel.PositionDescription = s.order.PositionDescription;
                        orderModel.Quantity = Quantity;
                        orderModel.Sheet = s.order.Sheet;
                        orderModel.ShopName = s.order.ShopName;
                        orderModel.ShopNo = s.order.ShopNo;
                        orderModel.SubjectName = s.subject.SubjectName;
                        orderModel.SubjectNo = s.subject.SubjectNo;
                        if ((s.order.PayOrderPrice ?? 0) > 0)
                        {
                            //orderPrice = (s.order.OrderPrice ?? 0).ToString();
                            orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.Gender = string.Empty;
                            orderModel.Sheet = string.Empty;
                            orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                        }
                        else
                        {
                            orderModel.TotalPrice = double.Parse((s.order.TotalPrice ?? 0).ToString());
                            orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                            orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveUnitPrice ?? 0).ToString());
                            orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveTotalPrice ?? 0).ToString());
                        }
                        orderModel.Remark = s.order.Remark;
                        newOrderList.Add(orderModel);

                    });
                    var assignShopList = (from assign in CurrentContext.DbContext.OutsourceAssignShop
                                          join shop in CurrentContext.DbContext.Shop
                                          on assign.ShopId equals shop.Id
                                          where assign.GuidanceId == gid
                                          && shopIdList.Contains(assign.ShopId ?? 0)
                                          && assign.OutsourceId==outsourceId
                                          && ((assign.PayInstallPrice ?? 0) > 0 || (assign.PayExpressPrice ?? 0) > 0)
                                          select new
                                          {
                                              assign,
                                              shop
                                          }).ToList();
                    if (assignShopList.Any())
                    {
                        assignShopList.ForEach(s =>
                        {

                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
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
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            if ((s.assign.PayInstallPrice ?? 0) > 0)
                            {
                                orderModel.PriceType = "安装费";
                                orderModel.TotalPrice = double.Parse((s.assign.PayInstallPrice ?? 0).ToString());
                                orderModel.UnitPrice = double.Parse((s.assign.PayInstallPrice ?? 0).ToString());
                                orderModel.ReceiveUnitPrice = double.Parse((s.assign.ReceiveInstallPrice ?? 0).ToString());
                                orderModel.ReceiveTotalPrice = double.Parse((s.assign.ReceiveInstallPrice ?? 0).ToString());
                            }
                            else if ((s.assign.PayExpressPrice ?? 0) > 0)
                            {
                                orderModel.PriceType = "发货费";
                                orderModel.TotalPrice = double.Parse((s.assign.PayExpressPrice ?? 0).ToString());
                                orderModel.UnitPrice = double.Parse((s.assign.PayExpressPrice ?? 0).ToString());
                                orderModel.ReceiveUnitPrice = double.Parse((s.assign.ReceiveExpresslPrice ?? 0).ToString());
                                orderModel.ReceiveTotalPrice = double.Parse((s.assign.ReceiveExpresslPrice ?? 0).ToString());
                            }
                            newOrderList.Add(orderModel);
                        });
                    }

                    var installPriceList = priceOrderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                    if (assignTypeList.Any())
                    {
                        installPriceList = installPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                    }
                    if (installPriceList.Any())
                    {
                        installPriceList.ForEach(s =>
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
                            orderModel.Area = 0;
                            orderModel.Gender = string.Empty;
                            orderModel.GraphicLength = 0;
                            orderModel.GraphicWidth = 0;
                            orderModel.GraphicMaterial = string.Empty;
                            orderModel.PositionDescription = string.Empty;
                            orderModel.Quantity = 1;
                            orderModel.Sheet = string.Empty;
                            orderModel.ShopName = s.order.ShopName;
                            orderModel.ShopNo = s.order.ShopNo;
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.PriceType = "安装费";
                            orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            newOrderList.Add(orderModel);
                        });
                    }
                    //var expressPriceList = new OutsourceOrderDetailBLL().GetList(s => s.GuidanceId == gid && outsourceList.Contains(s.OutsourceId ?? 0) && s.SubjectId == 0 && shopIdList.Contains(s.ShopId ?? 0) && s.OrderType == (int)OrderTypeEnum.发货费).ToList();
                    var expressPriceList = priceOrderList.Where(s =>  s.order.OrderType == (int)OrderTypeEnum.发货费).ToList();
                    if (assignTypeList.Any())
                    {
                        expressPriceList = expressPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                    }
                    if (expressPriceList.Any())
                    {
                        expressPriceList.ForEach(s =>
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
                            orderModel.Area = 0;
                            orderModel.Gender = string.Empty;
                            orderModel.GraphicLength = 0;
                            orderModel.GraphicWidth = 0;
                            orderModel.GraphicMaterial = string.Empty;
                            orderModel.PositionDescription = string.Empty;
                            orderModel.Quantity = 1;
                            orderModel.Sheet = string.Empty;
                            orderModel.ShopName = s.order.ShopName;
                            orderModel.ShopNo = s.order.ShopNo;
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.PriceType = "发货费";
                            orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                            orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                            newOrderList.Add(orderModel);
                        });
                    }
                    //特殊外协费用
                    Models.SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(gid);
                    if (guidanceModel != null)
                    {

                        var outsourcePriceOrderList = new OutsourcePriceOrderBLL().GetList(s => s.OutsourceId == gid && s.GuidanceYear == guidanceModel.GuidanceYear && s.GuidanceMonth == guidanceModel.GuidanceMonth).ToList();
                        
                        decimal installPrice = 0;
                        decimal measurePrice = 0;
                        decimal expressPrice = 0;
                        decimal otherPrice = 0;
                        
                        if (outsourcePriceOrderList.Any())
                        {
                            installPrice = outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).Sum(s => s.PayPrice ?? 0);
                            measurePrice = outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).Sum(s => s.PayPrice ?? 0);
                            expressPrice = outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费).Sum(s => s.PayPrice ?? 0);
                            otherPrice = outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用).Sum(s => s.PayPrice ?? 0);
                        }
                        if (installPrice > 0)
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
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
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.PriceType = "安装费-实报实销";
                            orderModel.TotalPrice = double.Parse(installPrice.ToString());
                            orderModel.UnitPrice = double.Parse(installPrice.ToString());
                            orderModel.ReceiveUnitPrice = 0;
                            orderModel.ReceiveTotalPrice = 0;
                            newOrderList.Add(orderModel);
                        }
                        if (measurePrice > 0)
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
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
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.PriceType = "测量费-实报实销";
                            orderModel.TotalPrice = double.Parse(measurePrice.ToString());
                            orderModel.UnitPrice = double.Parse(measurePrice.ToString());
                            orderModel.ReceiveUnitPrice = 0;
                            orderModel.ReceiveTotalPrice = 0;
                            newOrderList.Add(orderModel);
                        }
                        if (expressPrice > 0)
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
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
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.PriceType = "发货费-实报实销";
                            orderModel.TotalPrice = double.Parse(expressPrice.ToString());
                            orderModel.UnitPrice = double.Parse(expressPrice.ToString());
                            orderModel.ReceiveUnitPrice = 0;
                            orderModel.ReceiveTotalPrice = 0;
                            newOrderList.Add(orderModel);
                        }
                        if (otherPrice > 0)
                        {
                            orderModel = new OrderPriceDetail();
                            orderModel.AddDate = null;
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
                            orderModel.SubjectName = string.Empty;
                            orderModel.SubjectNo = string.Empty;
                            orderModel.UnitPrice = 0;
                            orderModel.PriceType = "其他费用-实报实销";
                            orderModel.TotalPrice = double.Parse(otherPrice.ToString());
                            orderModel.UnitPrice = double.Parse(otherPrice.ToString());
                            orderModel.ReceiveUnitPrice = 0;
                            orderModel.ReceiveTotalPrice = 0;
                            newOrderList.Add(orderModel);
                        }
                    }
                });
                if (newOrderList.Any())
                {
                    string templateFileName = "外协项目费用统计明细";
                   
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                    ISheet sheet = workBook.GetSheet("Sheet1");
                    int startRow = 1;
                    newOrderList.OrderBy(s => s.ShopNo).ToList().ForEach(s =>
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
                        dataRow.GetCell(0).SetCellValue(s.PriceType);

                        dataRow.GetCell(1).SetCellValue(s.SubjectNo);
                        if (s.AddDate != null)
                            dataRow.GetCell(2).SetCellValue(DateTime.Parse(s.AddDate.ToString()).ToShortDateString());
                        else
                            dataRow.GetCell(2).SetCellValue("");
                        dataRow.GetCell(3).SetCellValue(s.ShopNo);
                        dataRow.GetCell(4).SetCellValue(s.ShopName);
                        dataRow.GetCell(5).SetCellValue(s.Sheet);
                        dataRow.GetCell(6).SetCellValue(s.PositionDescription);
                        dataRow.GetCell(7).SetCellValue(s.Gender);
                        dataRow.GetCell(8).SetCellValue(s.GraphicWidth);
                        dataRow.GetCell(9).SetCellValue(s.GraphicLength);
                        dataRow.GetCell(10).SetCellValue(s.Area);
                        dataRow.GetCell(11).SetCellValue(s.GraphicMaterial);
                        dataRow.GetCell(12).SetCellValue(s.UnitPrice);
                        dataRow.GetCell(13).SetCellValue(s.ReceiveUnitPrice);
                        dataRow.GetCell(14).SetCellValue(s.Quantity);
                        dataRow.GetCell(15).SetCellValue(s.TotalPrice);
                        dataRow.GetCell(16).SetCellValue(s.ReceiveTotalPrice);
                        dataRow.GetCell(17).SetCellValue(s.SubjectName);
                        dataRow.GetCell(18).SetCellValue(s.Remark);
                        startRow++;
                    });
                    HttpCookie cookie = Request.Cookies["项目费用统计明细"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("项目费用统计明细");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();
                        sheet = null;
                        workBook = null;
                        StringBuilder fileName = new StringBuilder("外协费用统计明细");
                        if (!string.IsNullOrWhiteSpace(outsourceName))
                        {
                            fileName.Append("-");
                            fileName.Append(outsourceName);
                            if (!string.IsNullOrWhiteSpace(beginDateStr) && !string.IsNullOrWhiteSpace(endDateStr))
                            {
                                fileName.AppendFormat("({0}-{1})", beginDateStr, endDateStr);
                            }
                        }
                        OperateFile.DownLoadFile(ms, fileName.ToString());

                    }
                }

                else
                {
                    HttpCookie cookie = Request.Cookies["项目费用统计明细"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("项目费用统计明细");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);
                }
            }
        }
    }
}
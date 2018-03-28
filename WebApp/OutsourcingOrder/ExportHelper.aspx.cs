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
using System.Data;
using NPOI;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
namespace WebApp.OutsourcingOrder
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

            switch (type)
            {
                case "exportbjphw":
                    ExportBJPHW();
                    break;
                case "exportotherphw":
                    ExportOtherPHW();
                    break;
                case "export350":
                    Export350();
                    break;
                default:
                    break;
            }
        }


        void ExportBJPHW()
        {
            string fileName = string.Empty;
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            string outsourceType = string.Empty;
            string shopNo = string.Empty;
            string materialCategoryId = string.Empty;
            string exportType = string.Empty;
            string materialName = string.Empty;
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
                outsourceType = Request.QueryString["outsourceType"];
            }
            if (Request.QueryString["shopNo"] != null)
            {
                shopNo = Request.QueryString["shopNo"];
            }
            if (Request.QueryString["materialCategoryId"] != null)
            {
                materialCategoryId = Request.QueryString["materialCategoryId"];
            }
            if (Request.QueryString["fileName"] != null)
            {
                fileName = Request.QueryString["fileName"];
            }

            if (Request.QueryString["exportType"] != null)
            {
                exportType = Request.QueryString["exportType"];
            }
            if (Request.QueryString["materialName"] != null)
            {
                materialName = Request.QueryString["materialName"];
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
            var orderList = (from assignOrder in CurrentContext.DbContext.OutsourceOrderDetail
                            //join assign in CurrentContext.DbContext.OutsourceAssignShop
                            //on order.ShopId equals assign.ShopId
                            join shop in CurrentContext.DbContext.Shop
                            on assignOrder.ShopId equals shop.Id
                            join subject1 in CurrentContext.DbContext.Subject
                            on assignOrder.SubjectId equals subject1.Id into subjectTemp
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on assignOrder.GuidanceId equals guidance.ItemId
                             from subject in subjectTemp.DefaultIfEmpty()
                            where 
                            //assign.OutsourceId == outsourceId
                            outsourceList.Contains(assignOrder.OutsourceId??0)
                            && guidanceList.Contains(assignOrder.GuidanceId ?? 0)
                            //&& guidanceList.Contains(order.GuidanceId ?? 0)
                            && (subjectList.Any() ? (subjectList.Contains(assignOrder.SubjectId ?? 0)) : 1 == 1)
                            && (outsourceTypeList.Any() ? (outsourceTypeList.Contains(assignOrder.AssignType ?? 0)) : 1 == 1)
                            && (shopNoList.Any() ? (shopNoList.Contains(shop.ShopNo.ToLower())) : 1 == 1)
                            //&& (assignOrder.OrderType != (int)OrderTypeEnum.物料)
                            && assignOrder.SubjectId>0
                            && (assignOrder.IsDelete == null || assignOrder.IsDelete==false)
                            select new {
                                assignOrder,
                                //order,
                                shop,
                                subject,
                                guidance
                            }).ToList();
            //var orderList0 = orderList;
            //if (subjectList.Any())
            //{
            //    orderList = orderList.Where(s => subjectList.Contains(s.assignOrder.SubjectId??0)).ToList();
            //}
            orderList = orderList.Where(s => ((s.assignOrder.Sheet!=null && s.assignOrder.Sheet.Contains("光盘")) || (s.assignOrder.PositionDescription != null && s.assignOrder.PositionDescription.Contains("光盘"))) ? ((s.assignOrder.Format != null && s.assignOrder.Format != "") && s.assignOrder.Format.ToLower().IndexOf("hc") == -1 && s.assignOrder.Format.ToLower().IndexOf("homecourt") == -1 && s.assignOrder.Format.ToLower().IndexOf("homecore") == -1 && s.assignOrder.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
            if (!string.IsNullOrWhiteSpace(exportType))
            {
                if (exportType == "nohc")
                {
                    orderList = orderList.Where(s => (s.assignOrder.Format != null && s.assignOrder.Format != "") ? (s.assignOrder.Format.ToLower().IndexOf("hc") == -1 && s.assignOrder.Format.ToLower().IndexOf("homecourt") == -1 && s.assignOrder.Format.ToLower().IndexOf("homecore") == -1 && s.assignOrder.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                }
                else if (exportType == "hc")
                {
                    orderList = orderList.Where(s => (s.assignOrder.Format != null && s.assignOrder.Format != "") ? (s.assignOrder.Format.ToLower().IndexOf("hc") != -1 || s.assignOrder.Format.ToLower().IndexOf("homecourt") != -1 || s.assignOrder.Format.ToLower().IndexOf("homecore") != -1 || s.assignOrder.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (materialName == "软膜")
                {
                    orderList = orderList.Where(s => s.assignOrder.GraphicMaterial != null && s.assignOrder.GraphicMaterial.Contains("软膜")).ToList();
                }
                else if (materialName == "非软膜")
                {
                    orderList = orderList.Where(s => s.assignOrder.GraphicMaterial == null || s.assignOrder.GraphicMaterial == "" || (s.assignOrder.GraphicMaterial != null && !s.assignOrder.GraphicMaterial.Contains("软膜"))).ToList();
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
                        orderList = orderList.Where(s =>(s.assignOrder.GraphicMaterial!=null && materialList.Contains(s.assignOrder.GraphicMaterial.ToLower())) || (s.assignOrder.GraphicMaterial == null || s.assignOrder.GraphicMaterial == "")).ToList();

                    }
                    else
                        orderList = orderList.Where(s => (s.assignOrder.GraphicMaterial == null || s.assignOrder.GraphicMaterial == "")).ToList();
                }
                else
                    orderList = orderList.Where(s => s.assignOrder.GraphicMaterial != null && materialList.Contains(s.assignOrder.GraphicMaterial.ToLower())).ToList();
            }
            List<Order350Model> modelList = new List<Order350Model>();
            if (orderList.Any())
            {
                List<int> assignShopIdList = orderList.Select(s => s.shop.Id).Distinct().ToList();
                List<int> exportInstallPriceShopId = new List<int>();
                List<int> ExperssShopIdList = new List<int>();
                orderList.OrderBy(s=>s.shop.Id).ToList().ForEach(s => {
                    Order350Model model = new Order350Model();
                    string orderTypeDescription = CommonMethod.GetEnumDescription<OrderTypeEnum>((s.assignOrder.OrderType ?? 1).ToString());
                    string orderTypeName = CommonMethod.GeEnumName<OrderTypeEnum>((s.assignOrder.OrderType ?? 1).ToString());
                    if (orderTypeDescription == "费用订单")
                    {
                        model.UnitPrice = double.Parse((s.assignOrder.PayOrderPrice ?? 0).ToString());
                        model.ShopId = s.shop.Id;
                        model.SubjectId = 0;
                        model.GraphicNo = "";
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = "";
                        model.CityTier = "";
                        model.Contacts = "";
                        model.Format = "";
                        model.Gender = "";
                        //model.GraphicLength = 0;
                        model.GraphicMaterial = orderTypeName;
                        //model.GraphicWidth = 0;
                        model.POPAddress = "";
                        model.PositionDescription = "";
                        model.Province = "";
                        // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                        model.Quantity = 1;
                        model.Sheet = "";
                        model.ShopName = s.shop.ShopName;
                        model.ShopNo = s.shop.ShopNo;
                        model.SubjectName = "";
                        model.Tels = "";
                        model.POSScale = "";
                        model.MaterialSupport = "";
                        model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                        model.OrderType = orderTypeName;
                        modelList.Add(model);
                    }
                    else
                    {
                        double GraphicLength = s.assignOrder.GraphicLength != null ? double.Parse(s.assignOrder.GraphicLength.ToString()) : 0;
                        double GraphicWidth = s.assignOrder.GraphicWidth != null ? double.Parse(s.assignOrder.GraphicWidth.ToString()) : 0;
                        model.ShopId = s.shop.Id;
                        model.SubjectId = s.assignOrder.SubjectId??0;
                        model.City = s.assignOrder.City;
                        model.Format = s.assignOrder.Format;
                        model.GraphicNo = s.assignOrder.GraphicNo;
                        decimal area = 0;
                        if (s.assignOrder.GraphicWidth != null && s.assignOrder.GraphicLength != null)
                        {
                            area = ((s.assignOrder.GraphicWidth ?? 0) * (s.assignOrder.GraphicLength ?? 0)) / 1000000;
                        }
                        model.Area = double.Parse(area.ToString());
                        //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                        //model.Category = s.order.Category;
                        model.ChooseImg = s.assignOrder.ChooseImg;

                        //model.County = item.shop.AreaName;
                        model.CityTier = s.assignOrder.CityTier;
                        model.Contacts = s.shop.Contact1 + "/" + s.shop.Contact2;
                        model.Quantity = s.assignOrder.Quantity != null ? double.Parse(s.assignOrder.Quantity.ToString()) : 0;
                        model.Gender = s.assignOrder.Gender;
                        model.GraphicLength = GraphicLength;
                        model.GraphicMaterial = s.assignOrder.GraphicMaterial;
                        model.GraphicWidth = GraphicWidth;
                        model.POPAddress = s.assignOrder.POPAddress;
                        model.PositionDescription = s.assignOrder.PositionDescription;
                        model.Province = s.assignOrder.Province;

                        model.Sheet = s.assignOrder.Sheet;
                        model.ShopName = s.assignOrder.ShopName;
                        model.ShopNo = s.assignOrder.ShopNo;
                        string subjectName = string.Empty;
                        if (s.subject != null)
                        {
                            subjectName = s.subject.SubjectName;
                            if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(s.subject.Remark))
                                subjectName += "(" + s.subject.Remark + ")";
                        }
                        model.SubjectName = subjectName;
                        model.Tels = s.shop.Tel1 + "/" + s.shop.Tel2;
                        //model.OtherRemark = levelName;
                        //model.IsPOPMaterial = item.order.IsPOPMaterial;
                        model.POSScale = s.assignOrder.POSScale;
                        model.MaterialSupport = s.assignOrder.MaterialSupport;
                        model.GuidanceName = s.guidance.ItemName;
                        model.OrderType = orderTypeName;
                        modelList.Add(model);

                    }
                });
                //var installPriceList = new OutsourceOrderDetailBLL().GetList(s => guidanceList.Contains(s.GuidanceId ?? 0) && outsourceList.Contains(s.OutsourceId ?? 0) && s.SubjectId == 0 && assignShopIdList.Contains(s.ShopId ?? 0) && s.OrderType == (int)OrderTypeEnum.安装费).ToList();
                var installPriceList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                        join guidance in CurrentContext.DbContext.SubjectGuidance
                                        on order.GuidanceId equals guidance.ItemId
                                        where guidanceList.Contains(order.GuidanceId ?? 0)
                                        && outsourceList.Contains(order.OutsourceId ?? 0)
                                        && order.SubjectId == 0
                                        && assignShopIdList.Contains(order.ShopId ?? 0)
                                        && order.OrderType == (int)OrderTypeEnum.安装费
                                        select new { order, guidance.ItemName }
                                     ).ToList();
                if (installPriceList.Any())
                {
                    installPriceList.ForEach(s => {
                        Order350Model model = new Order350Model();
                        model.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                        model.ShopId = s.order.ShopId ?? 0;
                        model.SubjectId = 0;
                        model.GraphicNo = "";
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = "";
                        model.CityTier = "";
                        model.Contacts = "";
                        model.Format = "";
                        model.Gender = "";
                        //model.GraphicLength = 0;
                        model.GraphicMaterial = "安装费";
                        //model.GraphicWidth = 0;
                        model.POPAddress = "";
                        model.PositionDescription = "";
                        model.Province = "";
                       
                        model.Quantity = 1;
                        model.Sheet = "";
                        model.ShopName = s.order.ShopName;
                        model.ShopNo = s.order.ShopNo;
                        model.SubjectName = "";
                        model.Tels = "";
                        model.POSScale = "";
                        model.MaterialSupport = "";
                        model.GuidanceName = s.ItemName;
                        model.OrderType = "安装费";
                        modelList.Add(model);
                    });
                }
                var expressPriceList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                        join guidance in CurrentContext.DbContext.SubjectGuidance
                                        on order.GuidanceId equals guidance.ItemId
                                        where guidanceList.Contains(order.GuidanceId ?? 0)
                                        && outsourceList.Contains(order.OutsourceId ?? 0)
                                        && order.SubjectId == 0
                                        && assignShopIdList.Contains(order.ShopId ?? 0)
                                        && order.OrderType == (int)OrderTypeEnum.发货费
                                        select new { order, guidance.ItemName }
                                     ).ToList();
                if (expressPriceList.Any())
                {
                    expressPriceList.ForEach(s =>
                    {
                        Order350Model model = new Order350Model();
                        model.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                        model.ShopId = s.order.ShopId ?? 0;
                        model.SubjectId = 0;
                        model.GraphicNo = "";
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = "";
                        model.CityTier = "";
                        model.Contacts = "";
                        model.Format = "";
                        model.Gender = "";
                        //model.GraphicLength = 0;
                        model.GraphicMaterial = "发货费";
                        //model.GraphicWidth = 0;
                        model.POPAddress = "";
                        model.PositionDescription = "";
                        model.Province = "";

                        model.Quantity = 1;
                        model.Sheet = "";
                        model.ShopName = s.order.ShopName;
                        model.ShopNo = s.order.ShopNo;
                        model.SubjectName = "";
                        model.Tels = "";
                        model.POSScale = "";
                        model.MaterialSupport = "";
                        model.GuidanceName = s.ItemName;
                        model.OrderType = "发货费";
                        modelList.Add(model);
                    });
                }


                var assignShopList = (from assign in CurrentContext.DbContext.OutsourceAssignShop
                                      join shop in CurrentContext.DbContext.Shop
                                      on assign.ShopId equals shop.Id
                                      join guidance in CurrentContext.DbContext.SubjectGuidance
                                      on assign.GuidanceId equals guidance.ItemId
                                      where outsourceList.Contains(assign.OutsourceId??0)
                                      && guidanceList.Contains(assign.GuidanceId ?? 0)
                                      && assignShopIdList.Contains(assign.ShopId ?? 0)
                                      select new
                                      {
                                          assign,
                                          shop,
                                          guidance.ItemName
                                      }).ToList();
                if (assignShopList.Any())
                {
                    //安装费
                    var installShopList = assignShopList.Where(s => (s.assign.PayInstallPrice ?? 0) > 0).ToList();
                    if (installShopList.Any())
                    {
                        installShopList.ForEach(s =>
                        {
                            Order350Model model = new Order350Model();
                            model.UnitPrice = double.Parse((s.assign.PayInstallPrice ?? 0).ToString());
                            model.ShopId = s.shop.Id;
                            model.SubjectId = 0;
                            model.GraphicNo = "";
                            model.Area = 0;
                            model.Category = "";
                            model.ChooseImg = "";
                            model.City = "";
                            model.CityTier = "";
                            model.Contacts = "";
                            model.Format = "";
                            model.Gender = "";
                            model.GraphicMaterial = "安装费";
                            model.POPAddress = "";
                            model.PositionDescription = "";
                            model.Province = "";
                            model.Quantity = 1;
                            model.Sheet = "";
                            model.ShopName = s.shop.ShopName;
                            model.ShopNo = s.shop.ShopNo;
                            model.SubjectName = "";
                            model.Tels = "";
                            model.POSScale = "";
                            model.MaterialSupport = "";
                            model.GuidanceName = s.ItemName;
                            model.OrderType = "安装费";
                            modelList.Add(model);

                        });
                    }
                    //快递费
                    var expressShopList = assignShopList.Where(s => (s.assign.PayExpressPrice ?? 0) > 0).ToList();
                    if (expressShopList.Any())
                    {
                        expressShopList.ForEach(s =>
                        {
                            Order350Model model = new Order350Model();
                            model.UnitPrice = double.Parse((s.assign.PayExpressPrice ?? 0).ToString());
                            model.ShopId = s.shop.Id;
                            model.SubjectId = 0;
                            model.GraphicNo = "";
                            model.Area = 0;
                            model.Category = "";
                            model.ChooseImg = "";
                            model.City = "";
                            model.CityTier = "";
                            model.Contacts = "";
                            model.Format = "";
                            model.Gender = "";
                            model.GraphicMaterial = "发货费";
                            model.POPAddress = "";
                            model.PositionDescription = "";
                            model.Province = "";
                            model.Quantity = 1;
                            model.Sheet = "";
                            model.ShopName = s.shop.ShopName;
                            model.ShopNo = s.shop.ShopNo;
                            model.SubjectName = "";
                            model.Tels = "";
                            model.POSScale = "";
                            model.MaterialSupport = "";
                            model.GuidanceName = s.ItemName;
                            model.OrderType = "发货费";
                            modelList.Add(model);

                        });
                    }
                }
                

                
               
                modelList.Add(new Order350Model() { ShopId = 9999999, ShopNo = "P9999999" });

            }
            if (modelList.Any())
            {
                modelList = modelList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).ToList();
                string templateFileName = "phwTemplate";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                ISheet sheet = workBook.GetSheet("Sheet1");

                int startRow = 1;
               
                Order350Model lastItem = new Order350Model();
                #region
                foreach (var item in modelList)
                {
                    if (startRow == 1)
                    {
                        lastItem = item;
                    }
                    else
                    {

                        if (lastItem.ShopId != item.ShopId)
                        {

                            lastItem = item;
                            startRow++;
                        }
                    }
                    if (item.ShopId > 0 && item.ShopId < 9999999)
                    {
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
                        string material = string.Empty;
                        if (!string.IsNullOrWhiteSpace(item.GraphicMaterial))
                            material = GetBasicMaterial(item.GraphicMaterial);
                        if (string.IsNullOrWhiteSpace(material))
                            material = item.GraphicMaterial;
                        dataRow.GetCell(0).SetCellValue(material);
                        //dataRow.GetCell(1).SetCellValue(item.ChooseImg);
                        dataRow.GetCell(1).SetCellValue(item.SubjectName);
                        //dataRow.GetCell(2).SetCellValue(Math.Round(item.GraphicWidth / 1000, 3));
                        //dataRow.GetCell(3).SetCellValue(Math.Round(item.GraphicLength / 1000, 3));
                        dataRow.GetCell(2).SetCellValue(item.GraphicWidth / 1000);
                        dataRow.GetCell(3).SetCellValue(item.GraphicLength / 1000);
                        dataRow.GetCell(4).SetCellValue(item.Quantity);
                        dataRow.GetCell(5).SetCellValue("");
                        dataRow.GetCell(6).SetCellValue("");
                        dataRow.GetCell(7).SetCellValue("");
                        dataRow.GetCell(8).SetCellValue("");
                        if (item.UnitPrice > 0)
                            dataRow.GetCell(9).SetCellValue(item.UnitPrice);
                        //dataRow.GetCell(10).SetCellValue(item.SubjectName);
                        dataRow.GetCell(10).SetCellValue(item.GuidanceName);
                        dataRow.GetCell(11).SetCellValue("");
                        dataRow.GetCell(12).SetCellValue(item.ShopName);
                        dataRow.GetCell(13).SetCellValue(item.GraphicMaterial);

                        startRow++;
                    }

                }
                #endregion


                HttpCookie cookie = Request.Cookies["exportPHWBJOS"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("exportPHWBJOS");
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
                    if (string.IsNullOrWhiteSpace(fileName))
                        fileName = "喷绘王模板(北京)";
                    OperateFile.DownLoadFile(ms, fileName);
                   

                }
            }
            else
            {
                HttpCookie cookie = Request.Cookies["exportPHWBJOS"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("exportPHWBJOS");
                }
                cookie.Value = "2";//没有数据可以导出
                cookie.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Add(cookie);
            }
        }


        void ExportOtherPHW()
        {
            string fileName = string.Empty;
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            string outsourceType = string.Empty;
            string shopNo = string.Empty;
            string materialCategoryId = string.Empty;

            string exportType = string.Empty;
            string materialName = string.Empty;
            List<int> outsourceList = new List<int>();
            if (Request.QueryString["outsourceId"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["outsourceId"]))
            {
                outsourceId =Request.QueryString["outsourceId"];
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
                outsourceType = Request.QueryString["outsourceType"];
            }
            if (Request.QueryString["shopNo"] != null)
            {
                shopNo = Request.QueryString["shopNo"];
            }
            if (Request.QueryString["materialCategoryId"] != null)
            {
                materialCategoryId = Request.QueryString["materialCategoryId"];
            }
            if (Request.QueryString["fileName"] != null)
            {
                fileName = Request.QueryString["fileName"];
            }
            if (Request.QueryString["exportType"] != null)
            {
                exportType = Request.QueryString["exportType"];
            }
            if (Request.QueryString["materialName"] != null)
            {
                materialName = Request.QueryString["materialName"];
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
                             //join assign in CurrentContext.DbContext.OutsourceAssignShop
                             //on order.ShopId equals assign.ShopId
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subject1 in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject1.Id into subjectTemp
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on order.GuidanceId equals guidance.ItemId
                             from subject in subjectTemp.DefaultIfEmpty()
                             where outsourceList.Contains(order.OutsourceId??0)
                             && guidanceList.Contains(order.GuidanceId ?? 0)
                             && (subjectList.Any() ? (subjectList.Contains(order.SubjectId ?? 0)) : 1 == 1)
                             && (outsourceTypeList.Any() ? (outsourceTypeList.Contains(order.AssignType ?? 0)) : 1 == 1)
                             && (shopNoList.Any() ? (shopNoList.Contains(shop.ShopNo.ToLower())) : 1 == 1)
                             && (order.OrderType!=(int)OrderTypeEnum.物料)
                             && order.SubjectId > 0
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                
                                 order,
                                 shop,
                                 subject,
                                 guidance
                             }).ToList();
            orderList = orderList.Where(s => ((s.order.Sheet!=null && s.order.Sheet.Contains("光盘")) || (s.order.PositionDescription != null && s.order.PositionDescription.Contains("光盘"))) ? ((s.order.Format != null && s.order.Format != "") && s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
            if (!string.IsNullOrWhiteSpace(exportType))
            {
                if (exportType == "nohc")
                {
                    orderList = orderList.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                }
                else if (exportType == "hc")
                {
                    orderList = orderList.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") != -1 || s.order.Format.ToLower().IndexOf("homecourt") != -1 || s.order.Format.ToLower().IndexOf("homecore") != -1 || s.order.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (materialName == "软膜")
                {
                    orderList = orderList.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                }
                else if (materialName == "非软膜")
                {
                    orderList = orderList.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
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
                        orderList = orderList.Where(s => (s.order.GraphicMaterial!=null && materialList.Contains(s.order.GraphicMaterial.ToLower())) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                    }
                    else
                        orderList = orderList.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                }
                else
                    orderList = orderList.Where(s => s.order.GraphicMaterial != null && materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
            }
            List<Order350Model> modelList = new List<Order350Model>();
            if (orderList.Any())
            {
                List<int> assignShopIdList = orderList.Select(s => s.shop.Id).Distinct().ToList();
                List<int> exportInstallPriceShopId = new List<int>();
                List<int> ExperssShopIdList = new List<int>();
                orderList.ForEach(s =>
                {
                    Order350Model model = new Order350Model();
                    string orderTypeDescription = CommonMethod.GetEnumDescription<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                    string orderTypeName = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                    if (orderTypeDescription == "费用订单")
                    {
                        model.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                        model.ShopId = s.shop.Id;
                        model.SubjectId = 0;
                        model.GraphicNo = "";
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = "";
                        model.CityTier = "";
                        model.Contacts = "";
                        model.Format = "";
                        model.Gender = "";
                        //model.GraphicLength = 0;
                        model.GraphicMaterial = orderTypeName;
                        //model.GraphicWidth = 0;
                        model.POPAddress = "";
                        model.PositionDescription = "";
                        model.Province = "";
                        // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                        model.Quantity = 1;
                        model.Sheet = "";
                        model.ShopName = s.shop.ShopName;
                        model.ShopNo = s.shop.ShopNo;
                        model.SubjectName = "";
                        model.Tels = "";
                        model.POSScale = "";
                        model.MaterialSupport = "";
                        model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                        model.OrderType = orderTypeName;
                        modelList.Add(model);
                    }
                    else
                    {
                        double GraphicLength = s.order.GraphicLength != null ? double.Parse(s.order.GraphicLength.ToString()) : 0;
                        double GraphicWidth = s.order.GraphicWidth != null ? double.Parse(s.order.GraphicWidth.ToString()) : 0;
                        model.ShopId = s.shop.Id;
                        model.SubjectId = s.order.SubjectId??0;
                        model.City = s.order.City;
                        model.Format = s.order.Format;
                        model.GraphicNo = s.order.GraphicNo;
                        decimal area = 0;
                        if (s.order.GraphicWidth != null && s.order.GraphicLength != null)
                        {
                            area = ((s.order.GraphicWidth ?? 0) * (s.order.GraphicLength ?? 0)) / 1000000;
                        }
                        model.Area = double.Parse(area.ToString());
                        //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                        //model.Category = s.order.Category;
                        model.ChooseImg = s.order.ChooseImg;

                        //model.County = item.shop.AreaName;
                        model.CityTier = s.order.CityTier;
                        model.Contacts = s.shop.Contact1 + "/" + s.shop.Contact2;
                        model.Quantity = s.order.Quantity != null ? double.Parse(s.order.Quantity.ToString()) : 0;
                        model.Gender = s.order.Gender;
                        model.GraphicLength = GraphicLength;
                        model.GraphicMaterial = s.order.GraphicMaterial;
                        model.GraphicWidth = GraphicWidth;
                        model.POPAddress = s.order.POPAddress;
                        model.PositionDescription = s.order.PositionDescription;
                        model.Province = s.order.Province;

                        model.Sheet = s.order.Sheet;
                        model.ShopName = s.order.ShopName;
                        model.ShopNo = s.order.ShopNo;
                        string subjectName = string.Empty;
                        if (s.subject != null)
                        {
                            subjectName = s.subject.SubjectName;
                            if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(s.subject.Remark))
                                subjectName += "(" + s.subject.Remark + ")";
                        }
                        model.SubjectName = subjectName;
                        model.Tels = s.shop.Tel1 + "/" + s.shop.Tel2;
                        //model.OtherRemark = levelName;
                        //model.IsPOPMaterial = item.order.IsPOPMaterial;
                        model.POSScale = s.order.POSScale;
                        model.MaterialSupport = s.order.MaterialSupport;
                        model.GuidanceName = s.guidance.ItemName;
                        model.OrderType = orderTypeName;
                        modelList.Add(model);

                       
                    }
                });

                var installPriceList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                        join guidance in CurrentContext.DbContext.SubjectGuidance
                                        on order.GuidanceId equals guidance.ItemId
                                        where guidanceList.Contains(order.GuidanceId ?? 0)
                                        && outsourceList.Contains(order.OutsourceId ?? 0)
                                        && order.SubjectId == 0
                                        && assignShopIdList.Contains(order.ShopId ?? 0)
                                        && order.OrderType == (int)OrderTypeEnum.安装费
                                        select new { order, guidance.ItemName }
                                     ).ToList();
                if (installPriceList.Any())
                {
                    installPriceList.ForEach(s =>
                    {
                        Order350Model model = new Order350Model();
                        model.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                        model.ShopId = s.order.ShopId ?? 0;
                        model.SubjectId = 0;
                        model.GraphicNo = "";
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = "";
                        model.CityTier = "";
                        model.Contacts = "";
                        model.Format = "";
                        model.Gender = "";
                        //model.GraphicLength = 0;
                        model.GraphicMaterial = "安装费";
                        //model.GraphicWidth = 0;
                        model.POPAddress = "";
                        model.PositionDescription = "";
                        model.Province = "";

                        model.Quantity = 1;
                        model.Sheet = "";
                        model.ShopName = s.order.ShopName;
                        model.ShopNo = s.order.ShopNo;
                        model.SubjectName = "";
                        model.Tels = "";
                        model.POSScale = "";
                        model.MaterialSupport = "";
                        model.GuidanceName = s.ItemName;
                        model.OrderType = "安装费";
                        modelList.Add(model);
                    });
                }
                var expressPriceList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                        join guidance in CurrentContext.DbContext.SubjectGuidance
                                        on order.GuidanceId equals guidance.ItemId
                                        where guidanceList.Contains(order.GuidanceId ?? 0)
                                        && outsourceList.Contains(order.OutsourceId ?? 0)
                                        && order.SubjectId == 0
                                        && assignShopIdList.Contains(order.ShopId ?? 0)
                                        && order.OrderType == (int)OrderTypeEnum.发货费
                                        select new { order, guidance.ItemName }
                                     ).ToList();
                if (expressPriceList.Any())
                {
                    expressPriceList.ForEach(s =>
                    {
                        Order350Model model = new Order350Model();
                        model.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                        model.ShopId = s.order.ShopId ?? 0;
                        model.SubjectId = 0;
                        model.GraphicNo = "";
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = "";
                        model.CityTier = "";
                        model.Contacts = "";
                        model.Format = "";
                        model.Gender = "";
                        //model.GraphicLength = 0;
                        model.GraphicMaterial = "发货费";
                        //model.GraphicWidth = 0;
                        model.POPAddress = "";
                        model.PositionDescription = "";
                        model.Province = "";

                        model.Quantity = 1;
                        model.Sheet = "";
                        model.ShopName = s.order.ShopName;
                        model.ShopNo = s.order.ShopNo;
                        model.SubjectName = "";
                        model.Tels = "";
                        model.POSScale = "";
                        model.MaterialSupport = "";
                        model.GuidanceName = s.ItemName;
                        model.OrderType = "发货费";
                        modelList.Add(model);
                    });
                }
                var assignShopList = (from assign in CurrentContext.DbContext.OutsourceAssignShop
                                      join shop in CurrentContext.DbContext.Shop
                                      on assign.ShopId equals shop.Id
                                      join guidance in CurrentContext.DbContext.SubjectGuidance
                                      on assign.GuidanceId equals guidance.ItemId
                                      where outsourceList.Contains(assign.OutsourceId??0)
                                      && guidanceList.Contains(assign.GuidanceId ?? 0)
                                      && assignShopIdList.Contains(assign.ShopId ?? 0)
                                      select new
                                      {
                                          assign,
                                          shop,
                                          guidance.ItemName
                                      }).ToList();
                if (assignShopList.Any())
                {
                    //安装费
                    var installShopList = assignShopList.Where(s => (s.assign.PayInstallPrice ?? 0) > 0).ToList();
                    if (installShopList.Any())
                    {
                        installShopList.ForEach(s =>
                        {
                            Order350Model model = new Order350Model();
                            model.UnitPrice = double.Parse((s.assign.PayInstallPrice ?? 0).ToString());
                            model.ShopId = s.shop.Id;
                            model.SubjectId = 0;
                            model.GraphicNo = "";
                            model.Area = 0;
                            model.Category = "";
                            model.ChooseImg = "";
                            model.City = "";
                            model.CityTier = "";
                            model.Contacts = "";
                            model.Format = "";
                            model.Gender = "";
                            model.GraphicMaterial = "安装费";
                            model.POPAddress = "";
                            model.PositionDescription = "";
                            model.Province = "";
                            model.Quantity = 1;
                            model.Sheet = "";
                            model.ShopName = s.shop.ShopName;
                            model.ShopNo = s.shop.ShopNo;
                            model.SubjectName = "";
                            model.Tels = "";
                            model.POSScale = "";
                            model.MaterialSupport = "";
                            model.GuidanceName = s.ItemName;
                            model.OrderType = "安装费";
                            modelList.Add(model);

                        });
                    }
                    //快递费
                    var expressShopList = assignShopList.Where(s => (s.assign.PayExpressPrice ?? 0) > 0).ToList();
                    if (expressShopList.Any())
                    {
                        expressShopList.ForEach(s =>
                        {
                            Order350Model model = new Order350Model();
                            model.UnitPrice = double.Parse((s.assign.PayExpressPrice ?? 0).ToString());
                            model.ShopId = s.shop.Id;
                            model.SubjectId = 0;
                            model.GraphicNo = "";
                            model.Area = 0;
                            model.Category = "";
                            model.ChooseImg = "";
                            model.City = "";
                            model.CityTier = "";
                            model.Contacts = "";
                            model.Format = "";
                            model.Gender = "";
                            model.GraphicMaterial = "发货费";
                            model.POPAddress = "";
                            model.PositionDescription = "";
                            model.Province = "";
                            model.Quantity = 1;
                            model.Sheet = "";
                            model.ShopName = s.shop.ShopName;
                            model.ShopNo = s.shop.ShopNo;
                            model.SubjectName = "";
                            model.Tels = "";
                            model.POSScale = "";
                            model.MaterialSupport = "";
                            model.GuidanceName = s.ItemName;
                            model.OrderType = "发货费";
                            modelList.Add(model);

                        });
                    }
                }

                modelList.Add(new Order350Model() { ShopId = 9999999, ShopNo = "P9999999" });

            }
            if (modelList.Any())
            {
                modelList = modelList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).ToList();
                string templateFileName = "phwTemplate";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                ISheet sheet = workBook.GetSheet("Sheet1");

                int startRow = 1;

                Order350Model lastItem = new Order350Model();
                #region
                foreach (var item in modelList)
                {
                    if (startRow == 1)
                    {
                        lastItem = item;
                    }
                    else
                    {

                        if (lastItem.ShopId != item.ShopId)
                        {

                            lastItem = item;
                            //startRow++;
                        }
                    }
                    if (item.ShopId > 0 && item.ShopId < 9999999)
                    {
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
                        string material = string.Empty;
                        if (!string.IsNullOrWhiteSpace(item.GraphicMaterial))
                            material = GetBasicMaterial(item.GraphicMaterial);
                        if (string.IsNullOrWhiteSpace(material))
                            material = item.GraphicMaterial;
                        dataRow.GetCell(0).SetCellValue(material);
                        //dataRow.GetCell(1).SetCellValue(item.ChooseImg);
                        dataRow.GetCell(1).SetCellValue(item.SubjectName);
                        //dataRow.GetCell(2).SetCellValue(Math.Round(item.GraphicWidth / 1000, 2));
                        //dataRow.GetCell(3).SetCellValue(Math.Round(item.GraphicLength / 1000, 2));

                        dataRow.GetCell(2).SetCellValue(item.GraphicWidth / 1000);
                        dataRow.GetCell(3).SetCellValue(item.GraphicLength / 1000);

                        dataRow.GetCell(4).SetCellValue(item.Quantity);
                        dataRow.GetCell(5).SetCellValue("");
                        dataRow.GetCell(6).SetCellValue("");
                        dataRow.GetCell(7).SetCellValue("");
                        dataRow.GetCell(8).SetCellValue("");
                        if (item.UnitPrice > 0)
                            dataRow.GetCell(9).SetCellValue(item.UnitPrice);
                        //dataRow.GetCell(10).SetCellValue(item.SubjectName);
                        dataRow.GetCell(10).SetCellValue(item.GuidanceName);
                        dataRow.GetCell(11).SetCellValue("");
                        dataRow.GetCell(12).SetCellValue(item.ShopName);
                        dataRow.GetCell(13).SetCellValue(item.GraphicMaterial);

                        startRow++;
                    }

                }
                #endregion


                HttpCookie cookie = Request.Cookies["exportPHWOtherOS"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("exportPHWOtherOS");
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
                    if (string.IsNullOrWhiteSpace(fileName))
                        fileName = "喷绘王模板(外协)";
                    OperateFile.DownLoadFile(ms, fileName);


                }
            }
            else
            {
                HttpCookie cookie = Request.Cookies["exportPHWOtherOS"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("exportPHWOtherOS");
                }
                cookie.Value = "2";//没有数据可以导出
                cookie.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Add(cookie);
            }
        }

        void Export350()
        {
            string fileName = string.Empty;
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            string outsourceType = string.Empty;
            string shopNo = string.Empty;
            string materialCategoryId = string.Empty;

            string exportType = string.Empty;
            string materialName = string.Empty;
            List<int> outsourceList = new List<int>();
            if (Request.QueryString["outsourceId"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["outsourceId"]))
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
                outsourceType = Request.QueryString["outsourceType"];
            }
            if (Request.QueryString["shopNo"] != null)
            {
                shopNo = Request.QueryString["shopNo"];
            }
            if (Request.QueryString["materialCategoryId"] != null)
            {
                materialCategoryId = Request.QueryString["materialCategoryId"];
            }
            if (Request.QueryString["fileName"] != null)
            {
                fileName = Request.QueryString["fileName"];
            }
            if (Request.QueryString["exportType"] != null)
            {
                exportType = Request.QueryString["exportType"];
            }
            if (Request.QueryString["materialName"] != null)
            {
                materialName = Request.QueryString["materialName"];
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
                             //join assign in CurrentContext.DbContext.OutsourceAssignShop
                             //on order.ShopId equals assign.ShopId
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on subject.GuidanceId equals guidance.ItemId
                             join category1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals category1.Id into temp1
                             from category in temp1.DefaultIfEmpty()
                             where outsourceList.Contains(order.OutsourceId??0)
                            
                              && guidanceList.Contains(order.GuidanceId ?? 0)
                              && (subjectList.Any() ? (subjectList.Contains(order.SubjectId ?? 0)) : 1 == 1)
                             && (outsourceTypeList.Any() ? (outsourceTypeList.Contains(order.AssignType ?? 0)) : 1 == 1)
                             && (shopNoList.Any() ? (shopNoList.Contains(shop.ShopNo.ToLower())) : 1 == 1)
                             && (order.OrderType<3)
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 
                                 order,
                                 shop,
                                 subject,
                                 guidance,
                                 category.CategoryName
                             }).ToList();
            orderList = orderList.Where(s => ((s.order.Sheet!=null && s.order.Sheet.Contains("光盘")) || (s.order.PositionDescription != null && s.order.PositionDescription.Contains("光盘"))) ? ((s.order.Format != null && s.order.Format != "") && s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
            if (!string.IsNullOrWhiteSpace(exportType))
            {
                if (exportType == "nohc")
                {
                    orderList = orderList.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                }
                else if (exportType == "hc")
                {
                    orderList = orderList.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") != -1 || s.order.Format.ToLower().IndexOf("homecourt") != -1 || s.order.Format.ToLower().IndexOf("homecore") != -1 || s.order.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (materialName == "软膜")
                {
                    orderList = orderList.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                }
                else if (materialName == "非软膜")
                {
                    orderList = orderList.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
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
                        orderList = orderList.Where(s =>(s.order.GraphicMaterial!=null && materialList.Contains(s.order.GraphicMaterial.ToLower())) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                    }
                    else
                        orderList = orderList.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                }
                else
                    orderList = orderList.Where(s => s.order.GraphicMaterial != null && materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
            }
            List<Order350Model> modelList = new List<Order350Model>();
            if (orderList.Any())
            {
                #region
                var HCSmallSizeList = new HCSmallGraphicSizeBLL().GetList(s => s.Id > 0);
                foreach (var item in orderList)
                {
                    
                    bool isTakeSmallSize = false;
                    string format = item.order.Format ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(format) && (format.ToUpper().Contains("TERREX") || format.ToUpper().Contains("YA") || format.ToUpper().Contains("HCE")) && item.order.Sheet.Contains("鞋墙"))
                    {
                        var parentSize = HCSmallSizeList.Where(s => s.ParentId == 0 && format.ToUpper().Contains(s.Format.ToUpper()) && s.BigGraphicWidth == item.order.GraphicWidth && s.BigGraphicLength == item.order.GraphicLength).FirstOrDefault();
                        if (parentSize != null)
                        {
                            var hasSmallList = HCSmallSizeList.Where(s => s.ParentId == parentSize.Id).ToList();
                            if (hasSmallList.Any())
                            {
                                isTakeSmallSize = true;
                                hasSmallList.ForEach(size =>
                                {
                                    Order350Model model = new Order350Model();
                                    decimal area = 0;
                                    if (size.SmallGraphicWidth != null && size.SmallGraphicLength != null)
                                    {
                                        area = ((size.SmallGraphicWidth ?? 0) * (size.SmallGraphicLength ?? 0)) / 1000000;
                                    }
                                    model.Area = double.Parse(area.ToString());

                                    //model.Category = (item.order.Category != null && item.order.Category != "") ? item.order.Category : item.pop != null ? item.pop.Category : "";
                                    //model.Category = item.order.Category;
                                    model.ChooseImg = item.order.ChooseImg;

                                    model.City = item.shop.CityName;
                                    model.County = item.shop.AreaName;
                                    model.CityTier = item.shop.CityTier;
                                    model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                                    model.Format = item.order.Format;
                                    model.Gender = !string.IsNullOrWhiteSpace(item.order.OrderGender) ? item.order.OrderGender : item.order.Gender;
                                    //model.Gender = item.order.Gender;
                                    //model.GraphicLength = size.SmallGraphicLength != null ? double.Parse(size.SmallGraphicLength.ToString()) : 0;
                                    //string smallMaterial = GetSmallMaterial(item.order.SmallMaterialId ?? 0);
                                    //string material = item.order.GraphicMaterial;
                                    //if (!string.IsNullOrWhiteSpace(smallMaterial))
                                    //    material += ("+" + smallMaterial);
                                    model.GraphicMaterial = item.order.OrderGraphicMaterial;
                                    model.GraphicWidth = size.SmallGraphicWidth != null ? double.Parse(size.SmallGraphicWidth.ToString()) : 0; ;
                                    model.POPAddress = item.shop.POPAddress;
                                    model.PositionDescription = item.order.PositionDescription;
                                    model.Province = item.shop.ProvinceName;
                                    model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                    model.Sheet = item.order.Sheet;
                                    model.ShopName = item.order.ShopName;
                                    model.ShopNo = item.order.ShopNo;
                                    string subjectName = item.subject.SubjectName;
                                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                                        subjectName += "(" + item.subject.Remark + ")";
                                    model.SubjectName = subjectName;
                                    model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                                    if (item.CategoryName != null && item.CategoryName.Contains("童店"))//童店
                                        model.OtherRemark =item.order.Remark;
                                    model.POSScale = item.order.POSScale;
                                    model.MaterialSupport = item.order.MaterialSupport;
                                    modelList.Add(model);
                                });
                            }
                        }
                    }
                    if (!isTakeSmallSize)
                    {
                        double GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                        double GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0;
                        bool canGo = true;
                       
                        if (canGo)
                        {
                            string subjectName = item.subject.SubjectName;
                            if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                                subjectName += "(" + item.subject.Remark + ")";

                            Order350Model model = new Order350Model();
                            model.ShopId = item.shop.Id;
                            model.SubjectId = item.subject.Id;
                            model.GraphicNo = item.order.GraphicNo;
                            decimal area = 0;
                            if (item.order.GraphicWidth != null && item.order.GraphicLength != null)
                            {
                                area = ((item.order.GraphicWidth ?? 0) * (item.order.GraphicLength ?? 0)) / 1000000;
                            }
                            model.Area = double.Parse(area.ToString());

                            //model.Category = (item.order.Category != null && item.order.Category != "") ? item.order.Category : item.pop != null ? item.pop.Category : "";
                            //model.Category = item.order.Category;
                            model.ChooseImg = item.order.ChooseImg;

                            model.City = item.shop.CityName;
                            model.County = item.shop.AreaName;
                            model.CityTier = item.shop.CityTier;
                            model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                            model.Format = item.order.Format;
                            model.Gender = !string.IsNullOrWhiteSpace(item.order.OrderGender) ? item.order.OrderGender : item.order.Gender;
                            //model.Gender = item.order.Gender;
                            model.GraphicLength = GraphicLength;

                            model.GraphicMaterial = item.order.OrderGraphicMaterial;
                            model.GraphicWidth = GraphicWidth;
                            model.POPAddress = item.shop.POPAddress;
                            model.PositionDescription = item.order.PositionDescription;
                            model.Province = item.shop.ProvinceName;
                            model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                            model.Sheet = item.order.Sheet;
                            model.ShopName = item.order.ShopName;
                            model.ShopNo = item.order.ShopNo;

                            model.SubjectName = subjectName;
                            model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                            if (item.CategoryName != null && item.CategoryName.Contains("童店"))//童店
                                model.OtherRemark = item.order.Remark;
                            model.POSScale = item.order.POSScale;
                            model.MaterialSupport = item.order.MaterialSupport;

                            modelList.Add(model);
                        }
                    }
                }
                #endregion
            }
            if (modelList.Any())
            {
                modelList = modelList.OrderBy(s => s.ShopNo).ThenBy(s => s.Sheet).ToList();
                string templateFileName = "350模板";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);

                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read);


                ExcelPackage package = new ExcelPackage(outFile);
                ExcelWorksheet sheet = package.Workbook.Worksheets[1];

                int startRow = 2;
                string shopno = string.Empty;
                foreach (var item in modelList)
                {


                    if (startRow == 2)
                        shopno = item.ShopNo;
                    else
                    {
                        if (shopno != item.ShopNo)
                        {
                            shopno = item.ShopNo;

                            while (startRow.ToString().Substring(startRow.ToString().Length - 1, 1) != "2")
                            {
                                startRow++;

                            }

                        }
                    }


                    sheet.Cells[startRow, 1].Value = item.ShopNo;
                    sheet.Cells[startRow, 2].Value = item.ShopName;
                    sheet.Cells[startRow, 3].Value = item.Province;
                    sheet.Cells[startRow, 4].Value = item.City;
                    sheet.Cells[startRow, 5].Value = item.CityTier;
                    sheet.Cells[startRow, 6].Value = item.Format;
                    sheet.Cells[startRow, 7].Value = item.POPAddress;
                    sheet.Cells[startRow, 8].Value = item.Contacts;
                    sheet.Cells[startRow, 9].Value = item.Tels;
                    sheet.Cells[startRow, 10].Value = "";
                    sheet.Cells[startRow, 11].Value = item.SubjectName;
                    sheet.Cells[startRow, 12].Value = item.Gender;
                    sheet.Cells[startRow, 13].Value = item.ChooseImg;
                    sheet.Cells[startRow, 14].Value = item.Sheet;
                    sheet.Cells[startRow, 15].Value = item.Quantity;
                    sheet.Cells[startRow, 16].Value = item.GraphicMaterial;
                    sheet.Cells[startRow, 17].Value = item.GraphicWidth;
                    sheet.Cells[startRow, 18].Value = item.GraphicLength;
                    sheet.Cells[startRow, 19].Value = item.Area;
                    sheet.Cells[startRow, 20].Value = item.PositionDescription;
                    sheet.Cells[startRow, 21].Value = item.OtherRemark;

                    startRow++;

                }

                HttpCookie cookie = Request.Cookies["export350OS"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("export350OS");
                }
                cookie.Value = "1";
                cookie.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Add(cookie);

                using (MemoryStream ms = new MemoryStream())
                {
                    //workBook.Write(ms);
                    package.SaveAs(ms);
                    ms.Flush();
                    sheet = null;
                    
                    OperateFile.DownLoadFile(ms, fileName + "350订单表");

                }
            }
            else
            {


                HttpCookie cookie = Request.Cookies["export350OS"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("export350OS");
                }
                cookie.Value = "2";//没有数据可以导出
                cookie.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Add(cookie);

            }
        }
    }
}
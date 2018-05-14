using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;
using NPOI.SS.UserModel;
using System.Web;

namespace WebApp.OutsourcingOrder.Statistics
{
    public partial class ExpressPriceDetail : System.Web.UI.Page
    {
        string outsourceId = string.Empty;
        string guidanceId = string.Empty;
        string region = string.Empty;
        string subjectId = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["outsourceId"] != null)
            {
                outsourceId = Request.QueryString["outsourceId"];
            }
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = Request.QueryString["subjectId"];
            }
            if (Request.QueryString["region"] != null)
            {
                region = Request.QueryString["region"];
            }
            if (Request.QueryString["province"] != null)
            {
                province = Request.QueryString["province"];
            }
            if (Request.QueryString["city"] != null)
            {
                city = Request.QueryString["city"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        List<Shop> shopList = new List<Shop>();
        void GetData()
        {
            List<int> outsourceIdList = new List<int>();
            List<int> guidanceIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            if (!string.IsNullOrWhiteSpace(outsourceId))
            {
                outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
            }
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            if (!string.IsNullOrWhiteSpace(province))
            {
                provinceList = StringHelper.ToStringList(province, ',');
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                cityList = StringHelper.ToStringList(city, ',');
            }

            var orderList0 = (from orderDetail in CurrentContext.DbContext.OutsourceOrderDetail
                              join guidance in CurrentContext.DbContext.SubjectGuidance
                              on orderDetail.GuidanceId equals guidance.ItemId
                              where outsourceIdList.Contains(orderDetail.OutsourceId ?? 0)
                              && guidanceIdList.Contains(orderDetail.GuidanceId ?? 0)
                              && (orderDetail.IsDelete == null || orderDetail.IsDelete == false)
                              //&& (orderDetail.OrderType == (int)OrderTypeEnum.发货费 || orderDetail.OrderType == (int)OrderTypeEnum.运费)
                              select new
                              {
                                  orderDetail,
                                  guidance
                              }).ToList();
            var orderList = orderList0.Where(s => s.orderDetail.SubjectId > 0).ToList();
            if (subjectIdList.Any())
            {
                orderList = orderList.Where(s => subjectIdList.Contains(s.orderDetail.SubjectId ?? 0)).ToList();

            }
            if (regionList.Any())
            {
                orderList = orderList.Where(s => s.orderDetail.Region != null && regionList.Contains(s.orderDetail.Region.ToLower())).ToList();
            }
            if (provinceList.Any())
            {
                orderList = orderList.Where(s => provinceList.Contains(s.orderDetail.Province)).ToList();
            }
            if (cityList.Any())
            {
                orderList = orderList.Where(s => cityList.Contains(s.orderDetail.City)).ToList();
            }
            if (orderList.Any())
            {
                List<int> shopIdList = orderList.Select(s => s.orderDetail.ShopId ?? 0).ToList();
                var experssOrderList = orderList0.Where(s => shopIdList.Contains(s.orderDetail.ShopId ?? 0) && (s.orderDetail.OrderType == (int)OrderTypeEnum.发货费 || s.orderDetail.OrderType == (int)OrderTypeEnum.快递费 || s.orderDetail.OrderType == (int)OrderTypeEnum.运费)).ToList();
                int shopid = 1;
                experssOrderList.ForEach(s =>
                     {
                         decimal pPrice = s.orderDetail.PayOrderPrice ?? 0;
                         decimal rlPrice = s.orderDetail.ReceiveOrderPrice ?? 0;
                         Shop shopModel = new Shop();
                         shopModel.Id = shopid++;
                         shopModel.ShopNo = s.orderDetail.ShopNo;
                         shopModel.ShopName = s.orderDetail.ShopName;
                         shopModel.RegionName = s.orderDetail.Region;
                         shopModel.ProvinceName = s.orderDetail.Province;
                         shopModel.CityName = s.orderDetail.City;
                         shopModel.ExpressPrice = pPrice;
                         shopModel.ReceiveExpressPrice = rlPrice;
                         shopModel.GuidanceName = s.guidance.ItemName;
                         shopModel.Remark = s.orderDetail.Remark;
                         shopList.Add(shopModel);
                     });
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text.Trim()))
            {
                string shopNo = txtShopNo.Text.Trim();
                shopList = shopList.Where(s => s.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
        }

        void BindData() {
            GetData();
            labShopCount.Text = shopList.Select(s => s.Id).Distinct().Count().ToString();
            decimal totalPrice = shopList.Sum(s => s.ExpressPrice);
            decimal rTotalPrice = shopList.Sum(s => s.ReceiveExpressPrice);
            labTotalPrice.Text = Math.Round(totalPrice, 2).ToString();
            labRTotalPrice.Text = Math.Round(rTotalPrice, 2).ToString();
            var list = shopList;
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPrice.DataSource = list.OrderBy(s => s.GuidanceId).ThenBy(s => s.ShopNo).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPrice.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            GetData();
            var list = shopList.OrderBy(s => s.GuidanceId).ThenBy(s => s.ShopNo).ToList();

            if (list.Any())
            {
                string templateFileName = "OSInstallPriceTemplate";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                ISheet sheet = workBook.GetSheet("Sheet1");

                int startRow = 1;
                string shopno = string.Empty;
                foreach (var item in list)
                {



                    IRow dataRow = sheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = sheet.CreateRow(startRow);
                    for (int i = 0; i < 30; i++)
                    {
                        ICell cell = dataRow.GetCell(i);
                        if (cell == null)
                            cell = dataRow.CreateCell(i);

                    }

                    double pPrice = double.Parse((item.ExpressPrice).ToString());
                    double rPrice = double.Parse((item.ReceiveExpressPrice).ToString());


                    dataRow.GetCell(0).SetCellValue(item.GuidanceName);
                    dataRow.GetCell(1).SetCellValue(item.SubjectName);
                    dataRow.GetCell(2).SetCellValue(item.ShopNo);
                    dataRow.GetCell(3).SetCellValue(item.ShopName);
                    dataRow.GetCell(4).SetCellValue(item.POPAddress);
                    dataRow.GetCell(5).SetCellValue(item.RegionName);
                    dataRow.GetCell(6).SetCellValue(item.ProvinceName);
                    dataRow.GetCell(7).SetCellValue(item.CityName);
                    dataRow.GetCell(8).SetCellValue(item.CityTier);
                    dataRow.GetCell(9).SetCellValue(item.IsInstall);
                    dataRow.GetCell(10).SetCellValue(item.POSScale);
                    dataRow.GetCell(11).SetCellValue(item.MaterialSupport);
                    dataRow.GetCell(12).SetCellValue(pPrice);
                    dataRow.GetCell(13).SetCellValue(rPrice);

                    startRow++;

                }
               

                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();

                    sheet = null;

                    workBook = null;
                    OperateFile.DownLoadFile(ms, "外协快递费明细");


                }
            }
            else
            {

            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
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

namespace WebApp.Statistics
{
    public partial class FreightDetail : BasePage
    {
        string subjectIds = string.Empty;
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectIds"] != null)
            {
                subjectIds = Request.QueryString["subjectIds"];
            }
            if (Request.QueryString["regions"] != null)
            {
                region = Request.QueryString["regions"];
            }
            if (Request.QueryString["provinces"] != null)
            {
                province = Request.QueryString["provinces"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        List<Shop> shopList = new List<Shop>();
        void GetData()
        { 
            List<int> subjectIdList = new List<int>();
            
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on subject.GuidanceId equals guidance.ItemId
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 where subjectIdList.Contains(subject.Id) && (guidance.ActivityTypeId ?? 1) == 3//=3是快递的活动（35块发货费）
                                 //&& (shop.Status == null || shop.Status == "" || shop.Status == "正常")
                                 select new
                                 {
                                     order,
                                     shop,
                                     guidance
                                 }).ToList();
                List<string> regionList1 = new List<string>();
                List<string> provinceList1 = new List<string>();
                List<string> cityList1 = new List<string>();
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList1 = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                else
                {
                    GetResponsibleRegion.ForEach(s =>
                    {
                        regionList1.Add(s.ToLower());
                    });

                }
                if (regionList1.Any())
                {

                    orderList = orderList.Where(s => regionList1.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (!string.IsNullOrWhiteSpace(province))
                    {
                        provinceList1 = StringHelper.ToStringList(province, ',');
                        if (provinceList1.Any())
                        {
                            orderList = orderList.Where(s => provinceList1.Contains(s.shop.ProvinceName)).ToList();
                            //if (!string.IsNullOrWhiteSpace(city))
                            //{
                            //    cityList1 = StringHelper.ToStringList(city, ',');
                            //    if (cityList1.Any())
                            //        list1 = list1.Where(s => provinceList1.Contains(s.shop.ProvinceName) && cityList1.Contains(s.shop.CityName)).ToList();
                            //}
                        }

                    }


                }
                if (orderList.Any())
                {
                    shopList = orderList.Select(s => s.shop).Distinct().ToList();
                    if (labShopCount.Text == "0")
                    {
                        labShopCount.Text = shopList.Count.ToString();
                        decimal totalPrice = shopList.Count * 35;
                        labTotalPrice.Text = Math.Round(totalPrice,2).ToString();
                    }
                }
            }
        }

        void BindData()
        {
            GetData();
            AspNetPager1.RecordCount = shopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPrice.DataSource = shopList.OrderBy(s => s.ShopNo).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPrice.DataBind();     
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            GetData();
            if (shopList.Any())
            {
                string templateFileName = "ExpressPriceTemplate";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                ISheet sheet = workBook.GetSheet("Sheet1");

                int startRow = 1;
                string shopno = string.Empty;
                foreach (var item in shopList)
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

                    

                    dataRow.GetCell(0).SetCellValue(item.ShopNo);
                    dataRow.GetCell(1).SetCellValue(item.ShopName);
                    dataRow.GetCell(2).SetCellValue(item.POPAddress);
                    dataRow.GetCell(3).SetCellValue(item.RegionName);
                    dataRow.GetCell(4).SetCellValue(item.ProvinceName);
                    dataRow.GetCell(5).SetCellValue(item.CityName);
                    dataRow.GetCell(6).SetCellValue(item.CityTier);
                    dataRow.GetCell(7).SetCellValue(35);
                    startRow++;

                }


                HttpCookie cookie = Request.Cookies["发货费明细"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("发货费明细");
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
                    OperateFile.DownLoadFile(ms, "发货费明细");


                }
            }
            else
            {

            }
        }
    }
}
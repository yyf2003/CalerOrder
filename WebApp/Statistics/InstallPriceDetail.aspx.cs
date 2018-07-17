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
    public partial class InstallPriceDetail : BasePage
    {
        string guidanceIds = string.Empty;
        string subjectIds = string.Empty;
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        int subjectChannel = 0;
        string customerServiceIds = string.Empty;
        string subjectCategory = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = Request.QueryString["guidanceIds"];
            }
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
            if (Request.QueryString["subjectChannel"] != null)
            {
                subjectChannel = int.Parse(Request.QueryString["subjectChannel"]);
            }
            if (Request.QueryString["customerServiceIds"] != null)
            {
                customerServiceIds = Request.QueryString["customerServiceIds"];
            }
            if (Request.QueryString["subjectCategory"] != null)
            {
                subjectCategory = Request.QueryString["subjectCategory"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        List<Shop> shopList = new List<Shop>();
        void GetData()
        {
            List<int> guidanceIdList = new List<int>();
            List<int> subjectCategoryList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
            }
            List<int> subjectIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectIdList = StringHelper.ToIntList(subjectIds, ',');

            }
            List<int> customerServiceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(customerServiceIds))
            {
                customerServiceIdList = StringHelper.ToIntList(customerServiceIds, ',');
            }
            if (!string.IsNullOrWhiteSpace(subjectCategory))
            {
                subjectCategoryList = StringHelper.ToIntList(subjectCategory, ',');
            }
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
            List<int> installShopIdList = new List<int>();
            decimal totalPrice = 0;
            shopList = new List<Shop>();
            List<FinalOrderDetailTemp> fOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                                     from gid in guidanceIdList
                                                     where order.GuidanceId == gid
                                                     && (order.IsDelete == null || order.IsDelete == false)
                                                     select order).ToList();
            List<Subject> subjectList = (from subject in CurrentContext.DbContext.Subject
                                         from gid in guidanceIdList
                                         where subject.GuidanceId == gid
                                         && subject.ApproveState == 1
                                         && (subject.IsDelete == null || subject.IsDelete == false)
                                         select subject).ToList();
            List<SubjectGuidance> guidanceList = new SubjectGuidanceBLL().GetList(s => guidanceIdList.Contains(s.ItemId));
            guidanceIdList.ForEach(gid =>
            {
                List<int> currSubjectIdList = subjectIdList;
                var orderDetailList = (from order in fOrderList
                                       join subject in subjectList
                                       on order.SubjectId equals subject.Id
                                       join guidance in guidanceList
                                       on order.GuidanceId equals guidance.ItemId
                                       where order.GuidanceId == gid
                                       && order.OrderType != (int)OrderTypeEnum.物料
                                           //&& (order.IsDelete==null || order.IsDelete==false)
                                           //&& (subject.IsDelete==null || subject.IsDelete==false)
                                           //&& subject.ApproveState==1
                                           //&& subject.SubjectType!=(int)SubjectTypeEnum.费用订单
                                           //&& (regionList1.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList1.Contains(subject.PriceBlongRegion.ToLower()) : regionList1.Contains(order.Region.ToLower())) : 1 == 1)
                                       && (regionList1.Any() ? regionList1.Contains(order.Region.ToLower()) : 1 == 1)

                                       select new
                                       {
                                           order,
                                           subject,
                                           guidance
                                       }).ToList();
                if (subjectCategoryList.Any())
                {
                    if (subjectCategoryList.Contains(0))
                    {
                        subjectCategoryList.Remove(0);
                        if (subjectCategoryList.Any())
                        {
                            orderDetailList = orderDetailList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderDetailList = orderDetailList.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();
                    }
                    else
                        orderDetailList = orderDetailList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (subjectIdList.Any())
                {
                    orderDetailList = orderDetailList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId ?? 0)).ToList();
                }
                else
                {
                    currSubjectIdList = orderDetailList.Select(s => s.subject.Id).Distinct().ToList();
                }
                if (subjectChannel == 1)
                {
                    //上海系统单
                    orderDetailList = orderDetailList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel == 2)
                {
                    //分区订单
                    orderDetailList = orderDetailList.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (customerServiceIdList.Any())
                {
                    //orderDetailList = orderDetailList.Where(s => customerServiceIdList.Contains(s.order.CSUserId??0)).ToList();
                    if (customerServiceIdList.Contains(0))
                    {
                        customerServiceIds.Remove(0);
                        if (customerServiceIds.Any())
                        {
                            orderDetailList = orderDetailList.Where(s => customerServiceIdList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                        }
                        else
                        {
                            orderDetailList = orderDetailList.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                        }
                    }
                    else
                    {
                        orderDetailList = orderDetailList.Where(s => customerServiceIdList.Contains(s.order.CSUserId ?? 0)).ToList();

                    }
                }

                if (regionList1.Any())
                {

                    //orderDetailList = orderDetailList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList1.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList1.Contains(s.order.Region.ToLower())).ToList();

                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList1 = StringHelper.ToStringList(province, ',');

                    if (provinceList1.Any())
                    {
                        orderDetailList = orderDetailList.Where(s => provinceList1.Contains(s.order.Province)).ToList();
                        //orderList = orderList.Where(s => s.shop.ProvinceName=="黑龙江").ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(txtShopNo.Text.Trim()))
                {
                    string shopNo = txtShopNo.Text.Trim();
                    orderDetailList = orderDetailList.Where(s => s.order.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
                }
                List<int> shopIdList = orderDetailList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();

                try
                {
                    var orderList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                     join shop in CurrentContext.DbContext.Shop
                                     on installShop.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on installShop.SubjectId equals subject.Id
                                     join guidance in CurrentContext.DbContext.SubjectGuidance
                                     on installShop.GuidanceId equals guidance.ItemId
                                     where installShop.GuidanceId == gid
                                     && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)//不统计分区增补的安装费
                                     && (installShop.BasicPrice ?? 0) > 0
                                     && shopIdList.Contains(installShop.ShopId ?? 0)
                                     && currSubjectIdList.Contains(installShop.SubjectId ?? 0)
                                     select new
                                     {
                                         installShop,
                                         shop,
                                         subject,
                                         guidance.ItemName
                                     }).ToList();



                    bool isWindow = false;
                    bool isOOH = false;
                    foreach (ListItem li in cblFeeType.Items)
                    {
                        if (li.Selected)
                        {
                            if (li.Value == "window")
                                isWindow = true;
                            if (li.Value == "ooh")
                                isOOH = true;
                        }
                    }
                    if (isWindow)
                    {
                        orderList = orderList.Where(s => (s.installShop.WindowPrice ?? 0) > 0).ToList();

                    }
                    if (isOOH)
                    {
                        orderList = orderList.Where(s => (s.installShop.OOHPrice ?? 0) > 0).ToList();
                    }


                    if (orderList.Any())
                    {
                        orderList.ForEach(s =>
                        {

                            Shop shopModel = new Shop();
                            shopModel.ShopNo = s.shop.ShopNo;
                            shopModel.ShopName = s.shop.ShopName;
                            shopModel.RegionName = s.shop.RegionName;
                            shopModel.ProvinceName = s.shop.ProvinceName;
                            shopModel.CityName = s.shop.CityName;
                            shopModel.CityTier = s.shop.CityTier;
                            shopModel.IsInstall = s.shop.IsInstall;
                            shopModel.BasicInstallPrice = s.installShop.BasicPrice ?? 0;
                            shopModel.WindowInstallPrice = s.installShop.WindowPrice ?? 0;
                            shopModel.OOHInstallPrice = s.installShop.OOHPrice ?? 0;
                            shopModel.POSScale = s.installShop.POSScale;
                            shopModel.MaterialSupport = s.installShop.MaterialSupport;
                            shopModel.GuidanceName = s.ItemName;
                            shopModel.SubjectName = s.subject.SubjectName;
                            shopList.Add(shopModel);
                            totalPrice += ((s.installShop.BasicPrice ?? 0) + (s.installShop.WindowPrice ?? 0) + (s.installShop.OOHPrice ?? 0));
                            installShopIdList.Add(s.installShop.ShopId ?? 0);
                        });
                    }
                    // var priceOrderList = orderDetailList.Where(s => s.order.OrderType == (int)OrderTypeEnum.安装费 &&  && (s.order.OrderPrice ?? 0) > 0).ToList();
                    var priceOrderList = orderDetailList.Where(s => (s.guidance.ActivityTypeId ?? 1) != (int)GuidanceTypeEnum.Others && s.subject.SubjectType != (int)SubjectTypeEnum.二次安装 && s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                    if (priceOrderList.Any())
                    {
                        priceOrderList.ForEach(s =>
                        {
                            Shop shopModel = new Shop();
                            shopModel.ShopNo = s.order.ShopNo;
                            shopModel.ShopName = s.order.ShopName;
                            shopModel.RegionName = s.order.Region;
                            shopModel.ProvinceName = s.order.Province;
                            shopModel.CityName = s.order.City;
                            shopModel.CityTier = s.order.CityTier;
                            shopModel.IsInstall = s.order.IsInstall;
                            shopModel.BasicInstallPrice = s.order.OrderPrice ?? 0;
                            shopModel.WindowInstallPrice = 0;
                            shopModel.OOHInstallPrice = 0;
                            shopModel.POSScale = s.order.POSScale;
                            shopModel.MaterialSupport = s.order.MaterialSupport;
                            shopModel.GuidanceName = s.guidance.ItemName; ;
                            shopModel.SubjectName = s.subject.SubjectName;
                            shopList.Add(shopModel);
                            totalPrice += (s.order.OrderPrice ?? 0);
                            installShopIdList.Add(s.order.ShopId ?? 0);
                        });
                    }
                }
                catch (Exception ex)
                {

                }
            });




            labShopCount.Text = installShopIdList.Distinct().Count().ToString();
            labTotalPrice.Text = Math.Round(totalPrice, 2).ToString();



        }

        void BindData()
        {
            GetData();
            AspNetPager1.RecordCount = shopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPrice.DataSource = shopList.OrderBy(s => s.GuidanceId).ThenBy(s => s.ShopNo).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPrice.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gvPrice_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                Shop shop = e.Item.DataItem as Shop;
                if (shop != null)
                {
                    Label labTotal = (Label)e.Item.FindControl("labTotal");
                    decimal total = (shop.BasicInstallPrice ?? 0) + (shop.WindowInstallPrice ?? 0) + shop.OOHInstallPrice;
                    labTotal.Text = Math.Round(total, 2).ToString();
                }
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            GetData();
            var list = shopList.OrderBy(s => s.GuidanceId).ThenBy(s => s.ShopNo).ToList();

            if (list.Any())
            {
                string templateFileName = "InstallPriceTemplate";
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

                    double basicInstallPrice = double.Parse((item.BasicInstallPrice ?? 0).ToString());
                    double windowInstallPrice = double.Parse((item.WindowInstallPrice ?? 0).ToString());
                    double oohInstallPrice = double.Parse(item.OOHInstallPrice.ToString());

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
                    dataRow.GetCell(12).SetCellValue(basicInstallPrice);
                    dataRow.GetCell(13).SetCellValue(windowInstallPrice);
                    dataRow.GetCell(14).SetCellValue(oohInstallPrice);
                    dataRow.GetCell(15).SetCellValue(basicInstallPrice + windowInstallPrice + oohInstallPrice);

                    startRow++;

                }
                HttpCookie cookie = Request.Cookies["安装费明细"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("安装费明细");
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
                    OperateFile.DownLoadFile(ms, "安装费明细");


                }
            }
            else
            {

            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
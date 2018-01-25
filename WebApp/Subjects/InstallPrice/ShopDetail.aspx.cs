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

namespace WebApp.Subjects.InstallPrice
{
    public partial class ShopDetail : BasePage
    {
        int installDetailId;
        string subjectIds = string.Empty;
        string region = string.Empty;
        string status = string.Empty;
        string customerServiceIds = string.Empty;
        int fromSubmit = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["InstallDetailId"] != null)
            {
                installDetailId = int.Parse(Request.QueryString["InstallDetailId"]);
            }
            if (Request.QueryString["subjectIds"] != null)
            {
                subjectIds = Request.QueryString["subjectIds"];
            }
            if (Request.QueryString["region"] != null)
            {
                region = Request.QueryString["region"];
            }
            if (Request.QueryString["status"] != null)
            {
                status = Request.QueryString["status"];
            }
            if (Request.QueryString["fromSubmit"] != null)
            {
                fromSubmit = int.Parse(Request.QueryString["fromSubmit"]);
            }
            if (Request.QueryString["customerServiceIds"] != null)
            {
                customerServiceIds = Request.QueryString["customerServiceIds"];
            }
            if (!IsPostBack)
            {
                //BindData();
                GetInstallShop();
            }
        }

        List<Shop> shopList = new List<Shop>();
        void GetOrderData()
        {
            //InstallPriceDetail detailModel = new InstallPriceDetailBLL().GetModel(installDetailId);
            List<int> subjectIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectIdList = StringHelper.ToIntList(subjectIds,',');
            }

            var detailList = (from detail in CurrentContext.DbContext.InstallPriceDetail
                              join guidance in CurrentContext.DbContext.SubjectGuidance
                              on detail.GuidanceId equals guidance.ItemId
                              join subject in CurrentContext.DbContext.Subject
                              on detail.SubjectId equals subject.Id
                              where (installDetailId>0? (detail.Id == installDetailId):1==1)
                              && (subjectIdList.Any()?subjectIdList.Contains(detail.SubjectId??0):1==1)
                              select new
                              {
                                  detail,
                                  guidance.ItemName,
                                  subject.SubjectName
                              }).ToList();

            

            if (detailList.Any())
            {

                //labGuidanceName.Text = detailList[0].ItemName;
                //labSubjectName.Text = detailList[0].SubjectName;
                
                //int subjectId = detailModel.detail.SubjectId ?? 0;
                //int guidanceId = detailModel.detail.GuidanceId ?? 0;
                shopList = new List<Shop>();
                if (!subjectIdList.Any())
                {
                    subjectIdList = detailList.Select(s => s.detail.SubjectId ?? 0).Distinct().ToList();
                }
                detailList.ForEach(detail => {
                    if (!string.IsNullOrWhiteSpace(detail.detail.ShopIds))
                    {
                        List<int> shopIdList = StringHelper.ToIntList(detail.detail.ShopIds, ',');
                        if (shopIdList.Any())
                        {
                            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                             join shop in CurrentContext.DbContext.Shop
                                             on order.ShopId equals shop.Id
                                             join subject in CurrentContext.DbContext.Subject
                                             on order.SubjectId equals subject.Id
                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                             on subject.GuidanceId equals guidance.ItemId
                                             join pop1 in CurrentContext.DbContext.POP
                                             on new { order.ShopId, order.Sheet, order.GraphicNo } equals new { pop1.ShopId, pop1.Sheet, pop1.GraphicNo } into popTemp
                                             from pop in popTemp.DefaultIfEmpty()
                                             where //subject.GuidanceId == guidanceId
                                             (subjectIdList.Contains(subject.Id) || subjectIdList.Contains(subject.HandMakeSubjectId??0))
                                             && (order.IsDelete == null || order.IsDelete == false)
                                             && shopIdList.Contains(shop.Id)
                                             select new { guidance, order, shop, subject, pop }).ToList();

                            //if (subjectTypeIdList.Any())
                            //{
                            //    orderList = orderList.Where(s => subjectTypeIdList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                            //}
                            //if (subjectIdList.Any())
                            //{
                            //    orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
                            //}
                            if (orderList.Any())
                            {

                                //计算基础安装费的店铺Id,所有项目中，相同店铺只算一次
                                List<int> basicInstallPriceShop = new List<int>();

                                //计算橱窗安装费的店铺Id,所有项目中，相同店铺只算一次
                                List<int> windowInstallPriceShop = new List<int>();
                                //橱窗安装费(同一个店，无论有多少个橱窗订单，都算一次)
                                List<int> windowSheetShopIdList = orderList.Where(s => (s.order.Sheet == "橱窗" || s.order.Sheet.ToLower() == "window")).Select(s => s.shop.Id).Distinct().ToList();
                                Shop shopModel;
                                orderList.ForEach(sub =>
                                {
                                    shopModel = new Shop();
                                    shopModel.Id = sub.shop.Id;
                                    shopModel.ShopName = sub.shop.ShopName;
                                    shopModel.ShopNo = sub.shop.ShopNo;
                                    shopModel.RegionName = sub.shop.RegionName;
                                    shopModel.ProvinceName = sub.shop.ProvinceName;
                                    shopModel.CityName = sub.shop.CityName;
                                    shopModel.CityTier = sub.shop.CityTier;
                                    shopModel.Format = sub.shop.Format;
                                    shopModel.AddDate = sub.shop.AddDate;
                                    shopModel.AgentCode = sub.shop.AgentCode;
                                    shopModel.AgentName = sub.shop.AgentName;
                                    shopModel.Channel = sub.shop.Channel;
                                    shopModel.GuidanceId = sub.guidance.ItemId;
                                    shopModel.SubjectId = sub.order.SubjectId;
                                    shopModel.POSScale = sub.order.InstallPricePOSScale;
                                    shopModel.MaterialSupport = sub.order.MaterialSupport;
                                    shopModel.GuidanceName = detail.ItemName;
                                    shopModel.SubjectName = detail.SubjectName;
                                    shopModel.IsInstall = sub.shop.IsInstall;
                                    //decimal basicInstallPrice = 0;
                                    //decimal windowInstallPrice = 0;

                                    //基础安装费
                                    if (!basicInstallPriceShop.Contains(sub.shop.Id))
                                    {
                                        basicInstallPriceShop.Add(sub.shop.Id);
                                        if ((sub.shop.BasicInstallPrice ?? 0) > 0)
                                        {
                                            shopModel.BasicInstallPrice = (sub.shop.BasicInstallPrice ?? 0);
                                        }
                                        else
                                            shopModel.BasicInstallPrice = GetBasicInstallPrice(sub.order.MaterialSupport);
                                    }
                                    else
                                    {
                                        shopModel.BasicInstallPrice = 0;
                                    }
                                    //橱窗安装费
                                    if (windowSheetShopIdList.Contains(sub.shop.Id) && !windowInstallPriceShop.Contains(sub.shop.Id))
                                    {
                                        windowInstallPriceShop.Add(sub.shop.Id);

                                        shopModel.WindowInstallPrice = GetWindowInstallPrice(sub.order.MaterialSupport); ;
                                    }
                                    else
                                    {
                                        shopModel.WindowInstallPrice = 0;
                                    }
                                    if ((shopModel.BasicInstallPrice + shopModel.WindowInstallPrice) > 0)
                                        shopList.Add(shopModel);
                                });

                                //户外安装费
                                //户外订单(同一个店，如果有2个以上的户外位置订单，按最高算)
                                var oohOrderList = orderList.Where(s => s.pop != null && (s.order.Sheet == "户外" || s.order.Sheet.ToLower() == "ooh") && (s.pop.OOHInstallPrice ?? 0) > 0).Select(s => new { ShopId = s.shop.Id, SubjectId = s.subject.Id, OOHInstallPrice = s.pop.OOHInstallPrice ?? 0 }).ToList();
                                //.Join(oohOrderList, a => new { a.ShopId, a.OOHInstallPrice }, b => new { b.shop.Id, b.pop.OOHInstallPrice }, (a, b) => new {b.subject.Id,ShopId=b.shop.Id,a.OOHInstallPrice });
                                if (oohOrderList.Any())
                                {
                                    var oohList = (from order in oohOrderList
                                                   group order by new
                                                   {
                                                       order.ShopId
                                                   } into item
                                                   select new
                                                   {
                                                       item.Key.ShopId,
                                                       OOHInstallPrice = item.Max(s => s.OOHInstallPrice)
                                                   }).ToList();
                                    var finaloohList = (from order in oohOrderList
                                                        join ooh in oohList
                                                        on new { order.ShopId, order.OOHInstallPrice } equals new { ooh.ShopId, ooh.OOHInstallPrice }
                                                        select new
                                                        {
                                                            order.SubjectId,
                                                            ooh.ShopId,
                                                            ooh.OOHInstallPrice
                                                        }).Distinct().ToList();

                                    if (finaloohList.Any())
                                    {
                                        finaloohList.ForEach(f =>
                                        {
                                            var model = shopList.Where(sh => sh.Id == f.ShopId && sh.SubjectId == f.SubjectId).FirstOrDefault();
                                            if (model != null)
                                            {
                                                int index = shopList.IndexOf(model);
                                                model.OOHInstallPrice = f.OOHInstallPrice;
                                                shopList[index] = model;
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                });
                
            }
        }

        void GetData()
        {
            string province = hfProvinceAndCity.Value;
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> customerServiceList = new List<int>();
            if (!string.IsNullOrWhiteSpace(province))
            {
                string[] place = province.Split('$');
                if (!string.IsNullOrWhiteSpace(place[0]))
                {
                    provinceList = StringHelper.ToStringList(place[0],',');
                }
                if (!string.IsNullOrWhiteSpace(place[1]))
                {
                    cityList = StringHelper.ToStringList(place[1], ',');
                }
                if (!string.IsNullOrWhiteSpace(customerServiceIds))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceIds, ',');
                }
            }

            List<int> subjectIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectIdList = StringHelper.ToIntList(subjectIds,',');
            }
            shopList = new List<Shop>();

            //var list = (from detail in CurrentContext.DbContext.InstallPriceDetail
            //            join installShop in CurrentContext.DbContext.InstallPriceShopInfo
            //            on detail.Id equals installShop.InstallDetailId
            //            join shop in CurrentContext.DbContext.Shop
            //            on installShop.ShopId equals shop.Id
            //            join guidance in CurrentContext.DbContext.SubjectGuidance
            //            on detail.GuidanceId equals guidance.ItemId
            //            join subject in CurrentContext.DbContext.Subject
            //            on detail.SubjectId equals subject.Id
            //            where (installDetailId > 0 ? (detail.Id == installDetailId) : 1 == 1)
            //            && (subjectIdList.Any() ? subjectIdList.Contains(detail.SubjectId ?? 0) : 1 == 1)
            //            select new
            //            {
            //                installShop,
            //                shop,
            //                detail,
            //                guidance.ItemName,
            //                subject
            //                //subject.SubjectName
            //            }).ToList();
            var list = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                        join shop in CurrentContext.DbContext.Shop
                        on installShop.ShopId equals shop.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on installShop.GuidanceId equals guidance.ItemId
                        join subject in CurrentContext.DbContext.Subject
                        on installShop.SubjectId equals subject.Id
                        where subjectIdList.Contains(installShop.SubjectId ?? 0)

                        select new
                        {
                            installShop,
                            shop,
                            guidance.ItemName,
                            subject
                            //subject.SubjectName
                        }).ToList();
            if (fromSubmit==1)
            {
                int guidanceId = list[0].subject.GuidanceId ?? 0;
                List<int> myInstallShopIdList = new List<int>();
               
                List<string> cityCierList = new List<string>() { "T1", "T2", "T3" };
                myInstallShopIdList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                       join subject in CurrentContext.DbContext.Subject
                                       on order.SubjectId equals subject.Id
                                       join shop in CurrentContext.DbContext.Shop
                                       on order.ShopId equals shop.Id
                                       where subject.GuidanceId == guidanceId
                                       && (order.IsDelete == null || order.IsDelete == false)
                                       //&& ((shop.IsInstall != null && shop.IsInstall == "Y") || (subject.CornerType == "三叶草" && shop.BCSInstallPrice != null && shop.BCSInstallPrice > 0))
                                       && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                                       && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                                       //&& (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                                      select shop.Id).Distinct().ToList();
                list = list.Where(s => myInstallShopIdList.Contains(s.shop.Id)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "0")
                {
                    list = list.Where(s => s.shop.Status == null || s.shop.Status == "" || s.shop.Status == "正常").ToList();
                }
                else if (status == "1")
                {
                    list = list.Where(s => s.shop.Status != null && s.shop.Status.Contains("闭")).ToList();
                }
            }
            List<string> regionList = new List<string>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            if (!regionList.Any() && GetResponsibleRegion.Any())
            {
                GetResponsibleRegion.ForEach(s =>
                {
                    regionList.Add(s.ToLower());
                });
            }
            if (regionList.Any())
            {
                //list = list.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
                list = list.Where(s => (!string.IsNullOrWhiteSpace(s.subject.PriceBlongRegion)) ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())).ToList();
               
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
                    {
                        list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();

                }
            }
            decimal totalPrice = 0;
            if (list.Any())
            {
                //if (!subjectIdList.Any())
                //{
                //    subjectIdList = list.Select(s => s.detail.SubjectId ?? 0).Distinct().ToList();
                //}
                list.ForEach(s => {

                    Shop shopModel = s.shop;
                    shopModel.BasicInstallPrice = s.installShop.BasicPrice;
                    shopModel.WindowInstallPrice = s.installShop.WindowPrice;
                    shopModel.OOHInstallPrice = s.installShop.OOHPrice ?? 0;
                    shopModel.POSScale = s.installShop.POSScale;
                    shopModel.MaterialSupport = s.installShop.MaterialSupport;
                    shopModel.GuidanceName = s.ItemName;
                    shopModel.SubjectName = s.subject.SubjectName;
                    shopList.Add(shopModel);
                    totalPrice += ((s.installShop.BasicPrice ?? 0) + (s.installShop.WindowPrice ?? 0) + (s.installShop.OOHPrice ?? 0));
                });
            }
            if (labShopCount.Text == "0")
            {

                labShopCount.Text = shopList.Count.ToString();
                labTotalPrice.Text = Math.Round(totalPrice, 2).ToString();
            }
        }

        void BindData()
        {
            GetData();
            var list = shopList;
            
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPrice.DataSource = list.OrderBy(s => s.GuidanceId).ThenBy(s => s.SubjectName).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPrice.DataBind();
        }


        void GetInstallShop() {
            var list = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                        join shop in CurrentContext.DbContext.Shop
                        on installShop.ShopId equals shop.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on installShop.GuidanceId equals guidance.ItemId
                        join subject in CurrentContext.DbContext.Subject
                        on installShop.SubjectId equals subject.Id
                        where installShop.InstallDetailId == installDetailId
                        select new
                        {
                            installShop,
                            shop,
                            guidance.ItemName,
                            subject,
                            installShop.BasicPrice,
                            installShop.WindowPrice,
                            installShop.OOHPrice
                        }).ToList();
            if (!IsPostBack)
            {
                if (list.Any())
                {
                    List<string> provinceList0 = list.Select(s=>s.shop.ProvinceName).Distinct().OrderBy(s=>s).ToList();
                    if (provinceList0.Any())
                    {
                        provinceList0.ForEach(s => {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s+"&nbsp;";
                            cblProvince.Items.Add(li);
                        });
                    }
                }
            }
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                {
                    provinceList.Add(li.Value);
                }
            }
            if (provinceList.Any())
            {
                list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text.Trim()))
            {
                string shopNo = txtShopNo.Text.Trim().ToLower();
                list = list.Where(s => s.shop.ShopNo != null && s.shop.ShopNo.ToLower().Contains(shopNo)).ToList();
            }
            int totalShopCount = list.Select(s => s.shop.Id).Distinct().Count();
            decimal totalPrice = list.Sum(s=>(s.BasicPrice+s.WindowPrice+s.OOHPrice))??0;
            labShopCount.Text = totalShopCount.ToString();
            labTotalPrice.Text = Math.Round(totalPrice, 2).ToString();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPrice.DataSource = list.OrderBy(s => s.subject.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPrice.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            //GetData();
            //var list = shopList.OrderBy(s => s.GuidanceId).ThenBy(s => s.ShopNo).ToList();
            var list = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                        join shop in CurrentContext.DbContext.Shop
                        on installShop.ShopId equals shop.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on installShop.GuidanceId equals guidance.ItemId
                        join subject in CurrentContext.DbContext.Subject
                        on installShop.SubjectId equals subject.Id
                        where installShop.InstallDetailId == installDetailId
                        select new
                        {
                            installShop,
                            shop,
                            guidance.ItemName,
                            subject,
                        }).ToList();
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

                    double basicInstallPrice = double.Parse((item.installShop.BasicPrice ?? 0).ToString());
                    double windowInstallPrice = double.Parse((item.installShop.WindowPrice ?? 0).ToString());
                    double oohInstallPrice = double.Parse((item.installShop.OOHPrice??0).ToString());

                    dataRow.GetCell(0).SetCellValue(item.ItemName);
                    dataRow.GetCell(1).SetCellValue(item.subject.SubjectName);
                    dataRow.GetCell(2).SetCellValue(item.shop.ShopNo);
                    dataRow.GetCell(3).SetCellValue(item.shop.ShopName);
                    dataRow.GetCell(4).SetCellValue(item.shop.POPAddress);
                    dataRow.GetCell(5).SetCellValue(item.shop.RegionName);
                    dataRow.GetCell(6).SetCellValue(item.shop.ProvinceName);
                    dataRow.GetCell(7).SetCellValue(item.shop.CityName);
                    dataRow.GetCell(8).SetCellValue(item.shop.CityTier);
                    dataRow.GetCell(9).SetCellValue(item.shop.IsInstall);
                    dataRow.GetCell(10).SetCellValue(item.installShop.POSScale);
                    dataRow.GetCell(11).SetCellValue(item.installShop.MaterialSupport);
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

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            //BindData();
            GetInstallShop();
        }

        protected void gvPrice_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objBasicPrice = item.GetType().GetProperty("BasicPrice").GetValue(item,null);
                    object objWindowPrice = item.GetType().GetProperty("WindowPrice").GetValue(item, null);
                    object objOOHPrice = item.GetType().GetProperty("OOHPrice").GetValue(item, null);

                    decimal basicPrice = objBasicPrice != null ? decimal.Parse(objBasicPrice.ToString()) : 0;
                    decimal windowPrice = objWindowPrice != null ? decimal.Parse(objWindowPrice.ToString()) : 0;
                    decimal oohPrice = objOOHPrice != null ? decimal.Parse(objOOHPrice.ToString()) : 0;
                    Label labTotal = (Label)e.Item.FindControl("labTotal");
                    labTotal.Text = Math.Round((basicPrice + windowPrice + oohPrice), 2).ToString();
                }
                //Shop shop = e.Item.DataItem as Shop;
                //if (shop != null)
                //{
                //    Label labTotal = (Label)e.Item.FindControl("labTotal");
                //    decimal total = (shop.BasicInstallPrice ?? 0) + (shop.WindowInstallPrice ?? 0) + shop.OOHInstallPrice;
                //    labTotal.Text = Math.Round(total, 2).ToString();
                //}
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            GetInstallShop();
        }
    }
}
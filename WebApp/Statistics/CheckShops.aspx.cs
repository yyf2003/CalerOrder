using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using Common;
using NPOI.SS.UserModel;
using System.Configuration;
using System.IO;

namespace WebApp.Statistics
{
    public partial class CheckShops : BasePage
    {
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        string customerServiceIds = string.Empty;
        string guidanceIds = string.Empty;
        string subjectIds = string.Empty;
        string status = string.Empty;
        int subjectChannel = 0;
        string shopType = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["regions"] != null)
            {
                region = Request.QueryString["regions"];
            }
            if (Request.QueryString["provinces"] != null)
            {
                province = Request.QueryString["provinces"];
            }
            if (Request.QueryString["citys"] != null)
            {
                city = Request.QueryString["citys"];
            }
            if (Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = Request.QueryString["guidanceIds"];
            }
            if (Request.QueryString["subjectids"] != null)
            {
                subjectIds = Request.QueryString["subjectids"];
            }
            if (Request.QueryString["status"] != null)
            {
                status = Request.QueryString["status"];
            }
            if (Request.QueryString["subjectChannel"] != null)
            {
                subjectChannel = int.Parse(Request.QueryString["subjectChannel"]);
            }
            if (Request.QueryString["shopType"] != null)
            {
                shopType = Request.QueryString["shopType"];
            }
            if (Request.QueryString["customerServiceIds"] != null)
            {
                customerServiceIds = Request.QueryString["customerServiceIds"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }
        List<int> guidanceIdList = new List<int>();
        List<int> subjectIdList = new List<int>();
        List<string> regionList = new List<string>();
        List<string> provinceList = new List<string>();
        List<string> cityList = new List<string>();
        List<string> shopTypeList = new List<string>();
        List<int> customerServiceList = new List<int>();
        void GetCondition()
        {
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
                if (!string.IsNullOrWhiteSpace(subjectIds))
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');

                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                else
                {
                    GetResponsibleRegion.ForEach(s =>
                    {
                        regionList.Add(s.ToLower());
                    });
                }

                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                }
                if (!string.IsNullOrWhiteSpace(shopType))
                {
                    shopTypeList = StringHelper.ToStringList(shopType, ',');
                }
                if (!string.IsNullOrWhiteSpace(customerServiceIds))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceIds, ',');
                }
            }
        }

        List<Shop> GetShopList()
        {
            List<Shop> shopList = new List<Shop>();
            GetCondition();
            if (guidanceIdList.Any())
            {
               var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                              join shop in CurrentContext.DbContext.Shop
                              on order.ShopId equals shop.Id
                              join subject in CurrentContext.DbContext.Subject
                              on order.SubjectId equals subject.Id
                                where guidanceIdList.Contains(order.GuidanceId ?? 0)
                              && (subject.IsDelete == null || subject.IsDelete==false)
                              && subject.ApproveState==1
                              && (order.IsDelete == null || order.IsDelete == false)
                              //&& ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                              //&& (shop.Status == null || shop.Status == "" || shop.Status == "正常")
                              //&& (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower())) : 1 == 1)
                              && (regionList.Any() ? regionList.Contains(order.Region.ToLower()) : 1 == 1)
                              
                              select new { order, shop, subject }).ToList();
               if (subjectChannel == 1)
               {
                   //上海系统单
                   orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
               }
               else if (subjectChannel == 2)
               {
                   //分区订单
                   orderList = orderList.Where(s => s.order.IsFromRegion==true).ToList();
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
                           orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                   }
                   else
                       orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
               }
               if (!string.IsNullOrWhiteSpace(status))
               {
                   if (status == "0")
                   {
                       orderList = orderList.Where(s => s.shop.Status == null || s.shop.Status == "" || s.shop.Status == "正常").ToList();
                   }
                   else if (status == "1")
                   {
                       orderList = orderList.Where(s => s.shop.Status != null && s.shop.Status.Contains("闭")).ToList();
                   }
               }
               if (subjectIdList.Any())
                   orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId??0)).ToList();
               //if (regionList.Any())
               //{
               //    //orderList = orderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
               //    orderList = orderList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())).ToList();
               //}
                if (provinceList.Any())
                    orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                if (cityList.Any())
                    orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
                if (customerServiceList.Any())
                {
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            orderList = orderList.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                        }
                        else
                        {
                            orderList = orderList.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                        }
                    }
                    else
                    {
                        orderList = orderList.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();

                    }
                }
                if (orderList.Any())
                {
                    orderList.ForEach(s => {
                        if (!shopList.Exists(sh => sh.Id == s.shop.Id))
                        {
                            s.shop.POSScale = s.order.POSScale;
                            s.shop.MaterialSupport = s.order.MaterialSupport;
                            shopList.Add(s.shop);
                        }
                    });
                }

                var materialList = (from materialOrder in CurrentContext.DbContext.OrderMaterial
                                    join shop in CurrentContext.DbContext.Shop
                                    on materialOrder.ShopId equals shop.Id
                                    join subject in CurrentContext.DbContext.Subject
                                    on materialOrder.SubjectId equals subject.Id
                                    where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                     && (subject.IsDelete == null || subject.IsDelete == false)
                                    && (subject.ApproveState == 1)
                                    select new
                                    {
                                        materialOrder,
                                        shop
                                    }).ToList();
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status == "0")
                    {
                        materialList = materialList.Where(s => s.shop.Status == null || s.shop.Status == "" || s.shop.Status == "正常").ToList();
                    }
                    else if (status == "1")
                    {
                        materialList = materialList.Where(s => s.shop.Status != null && s.shop.Status.Contains("闭")).ToList();
                    }
                }
                if (subjectIdList.Any())
                    materialList = materialList.Where(s => subjectIdList.Contains(s.materialOrder.SubjectId??0)).ToList();
                if (regionList.Any())
                    materialList = materialList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                if (provinceList.Any())
                    materialList = materialList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                if (cityList.Any())
                    materialList = materialList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                //if (customerServiceList.Any())
                //{
                //    if (customerServiceList.Contains(0))
                //    {
                //        customerServiceList.Remove(0);
                //        if (customerServiceList.Any())
                //        {
                //            materialList = materialList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                //        }
                //        else
                //        {
                //            materialList = materialList.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                //        }
                //    }
                //    else
                //    {
                //        materialList = materialList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();

                //    }
                //}
                if (materialList.Any())
                {
                    materialList.ForEach(s =>
                    {
                        if (!shopList.Exists(sh => sh.Id == s.shop.Id))
                        {
                            
                            shopList.Add(s.shop);
                        }
                    });
                }
            }
            if (!IsPostBack)
            {
                if (shopList.Any())
                {
                    List<string> IsInstallList = shopList.Select(s => s.IsInstall).Distinct().ToList();
                    if (IsInstallList.Any())
                    {
                        bool isEmpty = false;
                        //if (IsInstallList.Contains(""))
                        //{
                        //    isEmpty = true;
                        //    IsInstallList.Remove("");
                        //}
                        IsInstallList.ForEach(s => {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                ListItem li = new ListItem();
                                li.Value = s;
                                li.Text = s + "&nbsp;&nbsp;";
                                cblIsInstall.Items.Add(li);
                            }
                            else
                                isEmpty = true;
                        });
                        if (isEmpty)
                        {
                            ListItem li = new ListItem();
                            li.Value = "";
                            li.Text = "空";
                            cblIsInstall.Items.Add(li);
                        }
                    }
                }
            }


            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                shopList = shopList.Where(s => s.ShopNo.ToLower().Contains(txtShopNo.Text.Trim().ToLower())).ToList();
            }
            List<string> isInstallSelectList = new List<string>();
            foreach (ListItem li in cblIsInstall.Items)
            {
                if (li.Selected && !isInstallSelectList.Contains(li.Value))
                {
                    isInstallSelectList.Add(li.Value);
                }
            }
            if (isInstallSelectList.Any())
            {
                if (isInstallSelectList.Contains(""))
                {
                    isInstallSelectList.Remove("");
                    if (isInstallSelectList.Any())
                    {
                        shopList = shopList.Where(s => isInstallSelectList.Contains(s.IsInstall) || s.IsInstall == null || s.IsInstall == "").ToList();
                    }
                    else
                    {
                        shopList = shopList.Where(s => s.IsInstall == null || s.IsInstall == "").ToList();
                    }
                }
                else
                    shopList = shopList.Where(s => isInstallSelectList.Contains(s.IsInstall)).ToList();
            }
            return shopList;
        }

        void BindData()
        {
            List<Shop> shopList = GetShopList();
            labShopCount.Text = shopList.Count.ToString();
            AspNetPager1.RecordCount =shopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = shopList.OrderBy(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvList.DataBind();

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }


        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<Shop> shopList = GetShopList();
            if (shopList.Any())
            {
                shopList = shopList.OrderBy(s => s.Id).ToList();
                string templateFileName = "ShopTemplate";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                ISheet sheet = workBook.GetSheet("Sheet1");

                int startRow = 1;
                //string shopno = string.Empty;
                foreach (var item in shopList)
                {



                    IRow dataRow = sheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = sheet.CreateRow(startRow);
                    for (int i = 0; i < 23; i++)
                    {
                        ICell cell = dataRow.GetCell(i);
                        if (cell == null)
                            cell = dataRow.CreateCell(i);

                    }
                    dataRow.GetCell(0).SetCellValue(item.ShopNo);
                    dataRow.GetCell(1).SetCellValue(item.ShopName);
                    dataRow.GetCell(2).SetCellValue(item.POSScale);
                    dataRow.GetCell(3).SetCellValue(item.MaterialSupport);
                    dataRow.GetCell(4).SetCellValue(item.RegionName);
                    dataRow.GetCell(5).SetCellValue(item.ProvinceName);
                    dataRow.GetCell(6).SetCellValue(item.CityName);
                    dataRow.GetCell(7).SetCellValue(item.AreaName);
                    dataRow.GetCell(8).SetCellValue(item.CityTier);
                    dataRow.GetCell(9).SetCellValue(item.IsInstall);
                    dataRow.GetCell(10).SetCellValue(item.AgentCode);
                    dataRow.GetCell(11).SetCellValue(item.AgentName);
                    dataRow.GetCell(12).SetCellValue(item.Channel);
                    dataRow.GetCell(13).SetCellValue(item.Format);
                    dataRow.GetCell(14).SetCellValue(item.LocationType);
                    dataRow.GetCell(15).SetCellValue(item.BusinessModel);
                    dataRow.GetCell(16).SetCellValue(item.POPAddress);
                    dataRow.GetCell(17).SetCellValue(item.Contact1);
                    dataRow.GetCell(18).SetCellValue(item.Tel1);
                    dataRow.GetCell(19).SetCellValue(item.Contact2);
                    dataRow.GetCell(20).SetCellValue(item.Tel2);
                    if (item.OpeningDate != null)
                        dataRow.GetCell(21).SetCellValue(DateTime.Parse(item.OpeningDate.ToString()).ToShortDateString());
                    dataRow.GetCell(22).SetCellValue(item.Status);
                    startRow++;

                }



                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    sheet = null;
                    workBook = null;
                    OperateFile.DownLoadFile(ms, "活动店铺信息");

                }
            }
        }
    }
}
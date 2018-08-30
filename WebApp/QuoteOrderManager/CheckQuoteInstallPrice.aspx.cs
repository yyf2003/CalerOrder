using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using DAL;
using Common;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using System.Configuration;


namespace WebApp.QuoteOrderManager
{
    public partial class CheckQuoteInstallPrice : BasePage
    {
        string guidanceMonth = string.Empty;
        string guidanceId = string.Empty;
        string subjectCategory = string.Empty;
        string subjectId = string.Empty;
        string region = string.Empty;

        string levelType = string.Empty;
        decimal levelPrice = 0;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["month"] != null)
            {
                guidanceMonth = Request.QueryString["month"];
            }
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["subjectCategory"] != null)
            {
                subjectCategory = Request.QueryString["subjectCategory"];
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = Request.QueryString["subjectId"];
            }
            if (Request.QueryString["region"] != null)
            {
                region = Request.QueryString["region"];
            }
            if (Request.QueryString["levelType"] != null)
            {
                levelType = Request.QueryString["levelType"];
            }
            if (Request.QueryString["price"] != null)
            {
                levelPrice = decimal.Parse(Request.QueryString["price"]);
            }
            if (!IsPostBack)
            {
                //Session["checkQuoteInstallPriceOrderList"] = null;
                //Session["checkQuoteInstallPriceGuidanceList"] = null;
                Session["checkQuoteInstallPriceShopList"] = null;

                BindData();
            }
        }


        void BindData1111()
        {
            List<int> guidanceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            List<string> regionList = new List<string>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            List<int> subjectCategoryList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectCategory))
            {
                subjectCategoryList = StringHelper.ToIntList(subjectCategory, ',');
            }
            List<int> subjectIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
            }

            //活动安装费
            var activityInstallPriceList = (from install in CurrentContext.DbContext.InstallPriceShopInfo
                                            join shop in CurrentContext.DbContext.Shop
                                            on install.ShopId equals shop.Id
                                            join subject in CurrentContext.DbContext.Subject
                                            on install.SubjectId equals subject.Id
                                            join guidance in CurrentContext.DbContext.SubjectGuidance
                                            on install.GuidanceId equals guidance.ItemId
                                            where guidanceIdList.Contains(install.GuidanceId ?? 0)
                                            select new { 
                                                install, shop, subject,guidance,
                                                install.BasicPrice,
                                                install.WindowPrice,
                                                install.OOHPrice
                                            }).ToList();
            if (regionList.Any())
            {
                activityInstallPriceList = activityInstallPriceList.Where(s => s.shop.RegionName != null && regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            }
            if (subjectCategoryList.Any())
            {
                if (subjectCategoryList.Contains(0))
                {
                    subjectCategoryList.Remove(0);
                    if (subjectCategoryList.Any())
                    {
                        activityInstallPriceList = activityInstallPriceList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                    {
                        activityInstallPriceList = activityInstallPriceList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                }
                else
                {
                    activityInstallPriceList = activityInstallPriceList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
            }
            if (subjectIdList.Any())
            {
                activityInstallPriceList = activityInstallPriceList.Where(s => subjectIdList.Contains(s.install.SubjectId??0)).ToList();
            }
            List<decimal> basicList = new List<decimal>();
            List<decimal> windowList = new List<decimal>();
            List<decimal> OOHList = new List<decimal>();
            #region 首次加载
            if (!IsPostBack && activityInstallPriceList.Any())
            {
                basicList = activityInstallPriceList.Where(s => (s.install.BasicPrice ?? 0) > 0).Select(s => (s.install.BasicPrice ?? 0)).OrderBy(s => s).ToList();
                windowList = activityInstallPriceList.Where(s => (s.install.WindowPrice ?? 0) > 0).Select(s => s.install.WindowPrice ?? 0).OrderBy(s => s).ToList();
                OOHList = activityInstallPriceList.Where(s => (s.install.OOHPrice ?? 0) > 0).Select(s => s.install.OOHPrice ?? 0).OrderBy(s => s).ToList();
                
                if (basicList.Any())
                {
                    basicList.Distinct().ToList().ForEach(s => {
                        ListItem li = new ListItem();
                        li.Value = s.ToString();
                        li.Text = s + "&nbsp;&nbsp;";
                        if (levelType == "basic" && levelPrice == s)
                        {
                            li.Selected = true;
                        }
                        cblBasicInstall.Items.Add(li);
                    });
                    

                }
                if (windowList.Any())
                {
                    windowList.Distinct().ToList().ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.ToString();
                        li.Text = s + "&nbsp;&nbsp;";
                        if (levelType == "window" && levelPrice == s)
                        {
                            li.Selected = true;
                        }
                        cblWindowInstall.Items.Add(li);
                    });
                    
                }
                if (OOHList.Any())
                {
                    OOHList.Distinct().ToList().ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.ToString();
                        li.Text = s + "&nbsp;&nbsp;";
                        if (levelType == "ooh" && levelPrice == s)
                        {
                            li.Selected = true;
                        }
                        cblOOHInstall.Items.Add(li);
                    });
                    
                }
                
            }
            #endregion
            List<decimal> basicSelectedList = new List<decimal>();
            List<decimal> windowSelectedList = new List<decimal>();
            List<decimal> oohSelectedList = new List<decimal>();

            foreach (ListItem li in cblBasicInstall.Items)
            {
                if (li.Selected)
                {
                    basicSelectedList.Add(decimal.Parse(li.Value));
                }
            }
            foreach (ListItem li in cblWindowInstall.Items)
            {
                if (li.Selected)
                {
                    windowSelectedList.Add(decimal.Parse(li.Value));
                }
            }
            foreach (ListItem li in cblOOHInstall.Items)
            {
                if (li.Selected)
                {
                    oohSelectedList.Add(decimal.Parse(li.Value));
                }
            }

            if (basicSelectedList.Any())
            {
                activityInstallPriceList = activityInstallPriceList.Where(s => basicSelectedList.Contains(s.BasicPrice??0)).ToList();
            }
            if (windowSelectedList.Any())
            {
                activityInstallPriceList = activityInstallPriceList.Where(s => windowSelectedList.Contains(s.WindowPrice ?? 0)).ToList();
            }
            if (oohSelectedList.Any())
            {
                activityInstallPriceList = activityInstallPriceList.Where(s => oohSelectedList.Contains(s.OOHPrice ?? 0)).ToList();
            }
            int totalShopCount = activityInstallPriceList.Select(s => s.install.ShopId ?? 0).Distinct().Count();
            labTotalShopCount.Text = totalShopCount.ToString();

            decimal totalPrice = activityInstallPriceList.Sum(s => (s.install.BasicPrice ?? 0) + (s.install.WindowPrice ?? 0) + (s.install.OOHPrice ?? 0));

            labTotalPeice.Text = totalPrice.ToString();

            StringBuilder totalStr = new StringBuilder();
            basicList = activityInstallPriceList.Where(s => (s.install.BasicPrice ?? 0) > 0).Select(s => (s.install.BasicPrice ?? 0)).OrderBy(s => s).ToList();
            windowList = activityInstallPriceList.Where(s => (s.install.WindowPrice ?? 0) > 0).Select(s => s.install.WindowPrice ?? 0).OrderBy(s => s).ToList();
            OOHList = activityInstallPriceList.Where(s => (s.install.OOHPrice ?? 0) > 0).Select(s => s.install.OOHPrice ?? 0).OrderBy(s => s).ToList();
                
            if (basicList.Any())
            {
                var basicGroup = (from basic in basicList
                                  group basic by basic into g
                                  select new
                                  {
                                      price = g.Key,
                                      count = g.Count()
                                  }).ToList();
                basicGroup.ForEach(p =>
                {
                    totalStr.AppendFormat("{0}×{1}+", p.price.ToString("0"), p.count);
                });
            }
            if (windowList.Any())
            {
                var windowGroup = (from window in windowList
                                   group window by window into g
                                   select new
                                   {
                                       price = g.Key,
                                       count = g.Count()
                                   }).ToList();

                windowGroup.Distinct().ToList().ForEach(p =>
                {
                    totalStr.AppendFormat("{0}×{1}+", p.price.ToString("0"), p.count);
                });
            }
            if (OOHList.Any())
            {
                var oohGroup = (from ooh in OOHList
                                group ooh by ooh into g
                                select new
                                {
                                    price = g.Key,
                                    count = g.Count()
                                }).ToList();

                oohGroup.ForEach(p =>
                {
                    totalStr.AppendFormat("{0}×{1}+", p.price.ToString("0"), p.count);
                });
            }
            labTotalStr.Text = "(" + totalStr.ToString().TrimEnd('+') + ")";

            Session["checkQuoteInstallPriceOrderList"] = activityInstallPriceList.Select(s=>s.install).ToList();
            Session["checkQuoteInstallPriceGuidanceList"] = activityInstallPriceList.Select(s => s.guidance).Distinct().ToList();
            Session["checkQuoteInstallPriceShopList"] = activityInstallPriceList.Select(s => s.shop).Distinct().ToList();

            AspNetPager1.RecordCount = activityInstallPriceList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvInstallPrice.DataSource = activityInstallPriceList.OrderBy(s => s.install.GuidanceId).ThenBy(s => s.shop.ShopNo).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvInstallPrice.DataBind();
        }

        void BindData()
        {
            List<int> guidanceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            List<string> regionList = new List<string>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            List<int> subjectCategoryList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectCategory))
            {
                subjectCategoryList = StringHelper.ToIntList(subjectCategory, ',');
            }
            List<int> subjectIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
            }
            List<Shop> shopList = new List<Shop>();
            #region 活动安装费
            var activityInstallPriceList = (from install in CurrentContext.DbContext.InstallPriceShopInfo
                                            join shop in CurrentContext.DbContext.Shop
                                            on install.ShopId equals shop.Id
                                            join subject in CurrentContext.DbContext.Subject
                                            on install.SubjectId equals subject.Id
                                            join guidance in CurrentContext.DbContext.SubjectGuidance
                                            on install.GuidanceId equals guidance.ItemId
                                            where guidanceIdList.Contains(install.GuidanceId ?? 0)
                                            select new
                                            {
                                                install,
                                                shop,
                                                subject,
                                                guidance,
                                                install.BasicPrice,
                                                install.WindowPrice,
                                                install.OOHPrice
                                            }).ToList();
            if (regionList.Any())
            {
                activityInstallPriceList = activityInstallPriceList.Where(s => s.shop.RegionName != null && regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            }
            if (subjectCategoryList.Any())
            {
                if (subjectCategoryList.Contains(0))
                {
                    subjectCategoryList.Remove(0);
                    if (subjectCategoryList.Any())
                    {
                        activityInstallPriceList = activityInstallPriceList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                    {
                        activityInstallPriceList = activityInstallPriceList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                }
                else
                {
                    activityInstallPriceList = activityInstallPriceList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
            }
            if (subjectIdList.Any())
            {
                activityInstallPriceList = activityInstallPriceList.Where(s => subjectIdList.Contains(s.install.SubjectId ?? 0)).ToList();
            }
            activityInstallPriceList.ForEach(price => {
                Shop shop = new Shop();
                //shop = price.shop;
                shop.ShopNo = price.shop.ShopNo;
                shop.ShopName = price.shop.ShopName;
                shop.RegionName = price.shop.RegionName;
                shop.ProvinceName = price.shop.ProvinceName;
                shop.CityName = price.shop.CityName;
                shop.CityTier = price.shop.CityTier;
                shop.IsInstall = price.shop.IsInstall;
                shop.GuidanceName = price.guidance.ItemName;
                shop.GuidanceId = price.guidance.ItemId;
                shop.MaterialSupport = price.install.MaterialSupport;
                shop.BasicInstallPrice = price.install.BasicPrice;
                shop.WindowInstallPrice = price.install.WindowPrice;
                shop.OOHInstallPrice = price.install.OOHPrice??0;
                shopList.Add(shop);
            });

            #endregion

            #region 安装费订单
            //var installPriceOrderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && s.OrderType == (int)OrderTypeEnum.安装费 && (s.OrderPrice ?? 0) > 0);
            var installPriceOrderList = (from order in CurrentContext.DbContext.QuoteOrderDetail
                                         join guidance in CurrentContext.DbContext.SubjectGuidance
                                         on order.GuidanceId equals guidance.ItemId
                                         where guidanceIdList.Contains(order.GuidanceId ?? 0)
                                         && order.OrderType == (int)OrderTypeEnum.安装费
                                         && (order.OrderPrice ?? 0) > 0
                                         select new {
                                             order,
                                             guidance
                                         }).ToList();
            if (regionList.Any())
            {
                installPriceOrderList = installPriceOrderList.Where(s => s.order.Region != null && regionList.Contains(s.order.Region.ToLower())).ToList();
            }
            if (subjectIdList.Any())
            {
                installPriceOrderList = installPriceOrderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
            }
            installPriceOrderList.ForEach(price => {
                Shop shop = new Shop();
                shop.IsInstall = price.order.IsInstall;
                shop.ShopName = price.order.ShopName;
                shop.ShopNo = price.order.ShopNo;
                shop.RegionName = price.order.Region;
                shop.ProvinceName = price.order.Province;
                shop.CityName = price.order.City;
                shop.CityTier = price.order.CityTier;
                shop.GuidanceName = price.guidance.ItemName;
                shop.MaterialSupport = price.order.InstallPriceMaterialSupport;
                shop.BasicInstallPrice = price.order.OrderPrice;
                shop.WindowInstallPrice = 0;
                shop.OOHInstallPrice = 0;
                shop.GuidanceId = price.order.GuidanceId;
                shopList.Add(shop);
            });
            #endregion

            List<decimal> basicList = new List<decimal>();
            List<decimal> windowList = new List<decimal>();
            List<decimal> OOHList = new List<decimal>();
            #region 首次加载
            if (!IsPostBack && shopList.Any())
            {
                basicList = shopList.Where(s => (s.BasicInstallPrice ?? 0) > 0).Select(s => (s.BasicInstallPrice ?? 0)).OrderBy(s => s).ToList();
                windowList = shopList.Where(s => (s.WindowInstallPrice ?? 0) > 0).Select(s => s.WindowInstallPrice ?? 0).OrderBy(s => s).ToList();
                OOHList = shopList.Where(s => s.OOHInstallPrice > 0).Select(s => s.OOHInstallPrice).OrderBy(s => s).ToList();

                if (basicList.Any())
                {
                    basicList.Distinct().ToList().ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.ToString();
                        li.Text = s + "&nbsp;&nbsp;";
                        if (levelType == "basic" && levelPrice == s)
                        {
                            li.Selected = true;
                        }
                        cblBasicInstall.Items.Add(li);
                    });


                }
                if (windowList.Any())
                {
                    windowList.Distinct().ToList().ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.ToString();
                        li.Text = s + "&nbsp;&nbsp;";
                        if (levelType == "window" && levelPrice == s)
                        {
                            li.Selected = true;
                        }
                        cblWindowInstall.Items.Add(li);
                    });

                }
                if (OOHList.Any())
                {
                    OOHList.Distinct().ToList().ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.ToString();
                        li.Text = s + "&nbsp;&nbsp;";
                        if (levelType == "ooh" && levelPrice == s)
                        {
                            li.Selected = true;
                        }
                        cblOOHInstall.Items.Add(li);
                    });

                }

            }
            #endregion
            List<decimal> basicSelectedList = new List<decimal>();
            List<decimal> windowSelectedList = new List<decimal>();
            List<decimal> oohSelectedList = new List<decimal>();

            foreach (ListItem li in cblBasicInstall.Items)
            {
                if (li.Selected)
                {
                    basicSelectedList.Add(decimal.Parse(li.Value));
                }
            }
            foreach (ListItem li in cblWindowInstall.Items)
            {
                if (li.Selected)
                {
                    windowSelectedList.Add(decimal.Parse(li.Value));
                }
            }
            foreach (ListItem li in cblOOHInstall.Items)
            {
                if (li.Selected)
                {
                    oohSelectedList.Add(decimal.Parse(li.Value));
                }
            }

            if (basicSelectedList.Any())
            {
                shopList = shopList.Where(s => basicSelectedList.Contains(s.BasicInstallPrice ?? 0)).ToList();
            }
            if (windowSelectedList.Any())
            {
                shopList = shopList.Where(s => windowSelectedList.Contains(s.WindowInstallPrice ?? 0)).ToList();
            }
            if (oohSelectedList.Any())
            {
                shopList = shopList.Where(s => oohSelectedList.Contains(s.OOHInstallPrice)).ToList();
            }
            int totalShopCount = shopList.Select(s => s.Id).Distinct().Count();
            labTotalShopCount.Text = totalShopCount.ToString();

            decimal totalPrice = shopList.Sum(s => (s.BasicInstallPrice ?? 0) + (s.WindowInstallPrice ?? 0) + s.OOHInstallPrice);

            labTotalPeice.Text = totalPrice.ToString();

            StringBuilder totalStr = new StringBuilder();
            basicList = shopList.Where(s => (s.BasicInstallPrice ?? 0) > 0).Select(s => (s.BasicInstallPrice ?? 0)).OrderBy(s => s).ToList();
            windowList = shopList.Where(s => (s.WindowInstallPrice ?? 0) > 0).Select(s => s.WindowInstallPrice ?? 0).OrderBy(s => s).ToList();
            OOHList = shopList.Where(s => s.OOHInstallPrice > 0).Select(s => s.OOHInstallPrice).OrderBy(s => s).ToList();

            if (basicList.Any())
            {
                var basicGroup = (from basic in basicList
                                  group basic by basic into g
                                  select new
                                  {
                                      price = g.Key,
                                      count = g.Count()
                                  }).ToList();
                basicGroup.ForEach(p =>
                {
                    totalStr.AppendFormat("{0}×{1}+", p.price.ToString("0"), p.count);
                });
            }
            if (windowList.Any())
            {
                var windowGroup = (from window in windowList
                                   group window by window into g
                                   select new
                                   {
                                       price = g.Key,
                                       count = g.Count()
                                   }).ToList();

                windowGroup.Distinct().ToList().ForEach(p =>
                {
                    totalStr.AppendFormat("{0}×{1}+", p.price.ToString("0"), p.count);
                });
            }
            if (OOHList.Any())
            {
                var oohGroup = (from ooh in OOHList
                                group ooh by ooh into g
                                select new
                                {
                                    price = g.Key,
                                    count = g.Count()
                                }).ToList();

                oohGroup.ForEach(p =>
                {
                    totalStr.AppendFormat("{0}×{1}+", p.price.ToString("0"), p.count);
                });
            }
            labTotalStr.Text = "(" + totalStr.ToString().TrimEnd('+') + ")";

            Session["checkQuoteInstallPriceShopList"] = shopList;
            AspNetPager1.RecordCount = shopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvInstallPrice.DataSource = shopList.OrderBy(s => s.GuidanceId).ThenBy(s => s.ShopNo).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvInstallPrice.DataBind();
        }

        protected void gvInstallPrice_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object dataItem = e.Item.DataItem;
                if (dataItem != null)
                {
                    object objBasicPrice = dataItem.GetType().GetProperty("BasicInstallPrice").GetValue(dataItem, null);
                    object objWindowPrice = dataItem.GetType().GetProperty("WindowInstallPrice").GetValue(dataItem, null);
                    object objOOHPrice = dataItem.GetType().GetProperty("OOHInstallPrice").GetValue(dataItem, null);

                    decimal basicPrice = objBasicPrice!=null?decimal.Parse(objBasicPrice.ToString()):0;
                    decimal windowPrice = objWindowPrice != null ? decimal.Parse(objWindowPrice.ToString()) : 0;
                    decimal oohPrice = objOOHPrice != null ? decimal.Parse(objOOHPrice.ToString()) : 0;

                    decimal subTotal = basicPrice + windowPrice + oohPrice;

                    ((Label)e.Item.FindControl("labTotal")).Text = subTotal.ToString();

                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void cblBasicInstall_SelectedIndexChanged(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void cblWindowInstall_SelectedIndexChanged(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void cblOOHInstall_SelectedIndexChanged(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport_Click11(object sender, EventArgs e)
        {
            List<InstallPriceShopInfo> installOrderList = new List<InstallPriceShopInfo>();
            List<SubjectGuidance> guidanceList = new List<SubjectGuidance>();
            List<Shop> shopList = new List<Shop>();
            if (Session["checkQuoteInstallPriceOrderList"] != null)
                installOrderList = Session["checkQuoteInstallPriceOrderList"] as List<InstallPriceShopInfo>;
            if (Session["checkQuoteInstallPriceGuidanceList"] != null)
                guidanceList = Session["checkQuoteInstallPriceGuidanceList"] as List<SubjectGuidance>;
            if (Session["checkQuoteInstallPriceShopList"] != null)
                shopList = Session["checkQuoteInstallPriceShopList"] as List<Shop>;

            var list = (from install in installOrderList
                       join shop in shopList
                       on install.ShopId equals shop.Id
                       join guidance in guidanceList
                       on install.GuidanceId equals guidance.ItemId
                       select new {
                           install,
                           shop,
                           guidance
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

                    double basicInstallPrice = double.Parse((item.install.BasicPrice ?? 0).ToString());
                    double windowInstallPrice = double.Parse((item.install.WindowPrice ?? 0).ToString());
                    double oohInstallPrice = double.Parse((item.install.OOHPrice ?? 0).ToString());

                    dataRow.GetCell(0).SetCellValue(item.guidance.ItemName);
                    //dataRow.GetCell(1).SetCellValue(item.subject.SubjectName);
                    dataRow.GetCell(2).SetCellValue(item.shop.ShopNo);
                    dataRow.GetCell(3).SetCellValue(item.shop.ShopName);
                    dataRow.GetCell(4).SetCellValue(item.shop.POPAddress);
                    dataRow.GetCell(5).SetCellValue(item.shop.RegionName);
                    dataRow.GetCell(6).SetCellValue(item.shop.ProvinceName);
                    dataRow.GetCell(7).SetCellValue(item.shop.CityName);
                    dataRow.GetCell(8).SetCellValue(item.shop.CityTier);
                    dataRow.GetCell(9).SetCellValue(item.shop.IsInstall);
                    dataRow.GetCell(10).SetCellValue(item.install.POSScale);
                    dataRow.GetCell(11).SetCellValue(item.install.MaterialSupport);
                    dataRow.GetCell(12).SetCellValue(basicInstallPrice);
                    dataRow.GetCell(13).SetCellValue(windowInstallPrice);
                    dataRow.GetCell(14).SetCellValue(oohInstallPrice);
                    dataRow.GetCell(15).SetCellValue(basicInstallPrice + windowInstallPrice + oohInstallPrice);
                    startRow++;

                }
               

                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();

                    sheet = null;

                    workBook = null;
                    OperateFile.DownLoadFile(ms, "安装费明细");


                }
            }

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<Shop> shopList = new List<Shop>();
            if (Session["checkQuoteInstallPriceShopList"] != null)
            {
                shopList = Session["checkQuoteInstallPriceShopList"] as List<Shop>;
            }
            if (shopList.Any())
            {
                string templateFileName = "InstallPriceTemplate";
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

                    double basicInstallPrice = double.Parse((item.BasicInstallPrice ?? 0).ToString());
                    double windowInstallPrice = double.Parse((item.WindowInstallPrice ?? 0).ToString());
                    double oohInstallPrice = double.Parse((item.OOHInstallPrice).ToString());

                    dataRow.GetCell(0).SetCellValue(item.GuidanceName);
                    //dataRow.GetCell(1).SetCellValue(item.subject.SubjectName);
                    dataRow.GetCell(2).SetCellValue(item.ShopNo);
                    dataRow.GetCell(3).SetCellValue(item.ShopName);
                    //dataRow.GetCell(4).SetCellValue(item.POPAddress);
                    dataRow.GetCell(5).SetCellValue(item.RegionName);
                    dataRow.GetCell(6).SetCellValue(item.ProvinceName);
                    dataRow.GetCell(7).SetCellValue(item.CityName);
                    dataRow.GetCell(8).SetCellValue(item.CityTier);
                    dataRow.GetCell(9).SetCellValue(item.IsInstall);
                    //dataRow.GetCell(10).SetCellValue(item.POSScale);
                    dataRow.GetCell(11).SetCellValue(item.MaterialSupport);
                    dataRow.GetCell(12).SetCellValue(basicInstallPrice);
                    dataRow.GetCell(13).SetCellValue(windowInstallPrice);
                    dataRow.GetCell(14).SetCellValue(oohInstallPrice);
                    dataRow.GetCell(15).SetCellValue(basicInstallPrice + windowInstallPrice + oohInstallPrice);
                    startRow++;

                }


                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();

                    sheet = null;

                    workBook = null;
                    OperateFile.DownLoadFile(ms, "报价安装费明细");


                }
            }
        }
    }
}
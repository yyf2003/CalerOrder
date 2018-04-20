using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using Models;
using DAL;
using System.Text;
using Common;


namespace WebApp.Statistics
{
    public partial class SubjectStatistics : BasePage
    {
        public int subjectId;
        public int isSearch;
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        int subjectChannel = 0;
        string customerServiceId = string.Empty;
        string subjectBeginDate = string.Empty;
        string subjectEndDate = string.Empty;
        string shopType = string.Empty;
        List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
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
            if (Request.QueryString["subjectChannel"] != null)
            {
                subjectChannel = int.Parse(Request.QueryString["subjectChannel"]);
            }
            if (Request.QueryString["shopType"] != null)
            {
                shopType = Request.QueryString["shopType"];
            }
            if (Request.QueryString["customerServiceId"] != null)
            {
                customerServiceId = Request.QueryString["customerServiceId"];
            }
            //if (Request.QueryString["subjectBeginDate"] != null)
            //{
            //    subjectBeginDate = Request.QueryString["subjectBeginDate"];
            //}
            //if (Request.QueryString["subjectEndDate"] != null)
            //{
            //    subjectEndDate = Request.QueryString["subjectEndDate"];
            //}
            if (!IsPostBack)
            {
                Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                if (subjectModel != null)
                    labSubjectName.Text = subjectModel.SubjectName;
                BindData();
                BindRegion();
            }
        }

        void BindRegion()
        {
            List<String> regionList = new List<string>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',');
            }
            else
                regionList = GetResponsibleRegion;
            if (regionList.Any())
            {
                cblRegion.Items.Clear();
                regionList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;";
                    cblRegion.Items.Add(li);
                });
            }
           
        }



        Dictionary<int, decimal> materialPriceDic = new Dictionary<int, decimal>();
        void BindData()
        {
            
            //orderList=new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId);


            var orderList1 = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                        && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType >1))
                       
                              select new { order, shop, subject }).ToList();
            List<string> regionList1 = new List<string>();
            List<string> provinceList1 = new List<string>();
            List<string> cityList1 = new List<string>();
            List<String> regionList = new List<string>();
            List<string> shopTypeList = new List<string>();
            List<int> customerServiceList = new List<int>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList1 = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            else
            {
                GetResponsibleRegion.ForEach(s => {
                    regionList1.Add(s.ToLower());
                });
                
            }
           
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    if (!regionList1.Contains(li.Value.ToLower()))
                    {
                        regionList1.Add(li.Value.ToLower());
                        
                    }
                }
            }
            
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                {
                    if (!provinceList1.Contains(li.Value))
                    {
                        provinceList1.Add(li.Value);

                    }
                }
            }
            if (!provinceList1.Any())
            {
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList1 = StringHelper.ToStringList(province, ',');
                }
            }
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                {
                    if (!cityList1.Contains(li.Value))
                    {
                        cityList1.Add(li.Value);

                    }
                }
            }
            if (!cityList1.Any())
            {
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList1 = StringHelper.ToStringList(city, ',');
                }
            }
            if (subjectChannel == 1)
            {
                //上海系统单
                orderList1 = orderList1.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
            }
            else if (subjectChannel == 2)
            {
                //分区订单
                orderList1 = orderList1.Where(s => s.order.IsFromRegion==true).ToList();
            }
            if (!string.IsNullOrWhiteSpace(shopType))
            {
                shopTypeList = StringHelper.ToStringList(shopType, ',');
                if (shopTypeList.Any())
                {
                    if (shopTypeList.Contains("空"))
                    {
                        shopTypeList.Remove("空");
                        if (shopTypeList.Any())
                        {
                            orderList1 = orderList1.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                        }
                        else
                            orderList1 = orderList1.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                    }
                    else
                        orderList1 = orderList1.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                }
            }
            if (regionList1.Any())
            {
                //orderList1 = orderList1.Where(s => regionList1.Contains(s.shop.RegionName.ToLower())).ToList();
                //orderList1 = orderList1.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList1.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList1.Contains(s.shop.RegionName.ToLower())).ToList();
                orderList1 = orderList1.Where(s => regionList1.Contains(s.shop.RegionName.ToLower())).ToList();
                
                if (provinceList1.Any())
                {
                    orderList1 = orderList1.Where(s => provinceList1.Contains(s.shop.ProvinceName)).ToList();
                    if (cityList1.Any())
                    {
                        orderList1 = orderList1.Where(s => provinceList1.Contains(s.shop.ProvinceName) && cityList1.Contains(s.shop.CityName)).ToList();
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(customerServiceId))
            {
                customerServiceList = StringHelper.ToIntList(customerServiceId, ',');
                if(customerServiceList.Any())
                    orderList1 = orderList1.Where(s => customerServiceList.Contains(s.shop.CSUserId??0)).ToList();
            }
            //if (!string.IsNullOrWhiteSpace(subjectBeginDate))
            //{
            //    DateTime beginDate = DateTime.Parse(subjectBeginDate);
            //    orderList1 = orderList1.Where(s => s.subject.AddDate >= beginDate).ToList();
            //    if (!string.IsNullOrWhiteSpace(subjectEndDate))
            //    {
            //        DateTime endDate = DateTime.Parse(subjectEndDate).AddDays(1);
            //        orderList1 = orderList1.Where(s => s.subject.AddDate < endDate).ToList();
            //    }
            //}
            orderList = orderList1.Select(s => s.order).ToList();
            List<int> shopIdList = orderList1.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
            //decimal installPrice = new ShopBLL().GetList(s => shopIdList.Contains(s.Id)).Select(s => s.InstallPrice ?? 0).Sum();
            
            decimal area = 0;
            decimal popPrice = 0;
            
            StatisticPOPTotalPrice(orderList, out popPrice, out area);
            labArea.Text = area > 0 ? (area + "平方米") : "0";
            
            if (popPrice > 0)
            {
                
                labPOPPrice.Text = Math.Round(popPrice, 2) + "元";
                labPOPPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labPOPPrice.Attributes.Add("data-region", StringHelper.ListToString(regionList1));
                labPOPPrice.Attributes.Add("data-province", StringHelper.ListToString(provinceList1));
                labPOPPrice.Attributes.Add("data-city", StringHelper.ListToString(cityList1));
                labPOPPrice.Attributes.Add("data-subjectchannel", subjectChannel.ToString());
                labPOPPrice.Attributes.Add("data-shoptype", shopType);
                labPOPPrice.Attributes.Add("name","checkMaterial");
            }
            else
            {
                labPOPPrice.Text = "0";
            }
            
           
            var materialOrderList = (from materialOrder in CurrentContext.DbContext.OrderMaterial
                                     join shop in CurrentContext.DbContext.Shop
                                     on materialOrder.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on materialOrder.SubjectId equals subject.Id
                                     where materialOrder.SubjectId == subjectId
                                     select new { shop, materialOrder, subject }).ToList();
            if (shopTypeList.Any())
            {
                if (shopTypeList.Contains("空"))
                {
                    shopTypeList.Remove("空");
                    if (shopTypeList.Any())
                    {
                        materialOrderList = materialOrderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                    }
                    else
                        materialOrderList = materialOrderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                }
                else
                    materialOrderList = materialOrderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
            }
            if (regionList.Any())
            {
                materialOrderList = materialOrderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                if (provinceList1.Any())
                {
                    materialOrderList = materialOrderList.Where(s => provinceList1.Contains(s.shop.ProvinceName)).ToList();
                    if (cityList1.Any())
                    {
                        materialOrderList = materialOrderList.Where(s => cityList1.Contains(s.shop.CityName)).ToList();
                    }
                }
                
            }
            if (customerServiceList.Any())
                materialOrderList = materialOrderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();

            //if (!string.IsNullOrWhiteSpace(subjectBeginDate))
            //{
            //    DateTime beginDate = DateTime.Parse(subjectBeginDate);
            //    materialOrderList = materialOrderList.Where(s => s.subject.AddDate >= beginDate).ToList();
            //    if (!string.IsNullOrWhiteSpace(subjectEndDate))
            //    {
            //        DateTime endDate = DateTime.Parse(subjectEndDate).AddDays(1);
            //        materialOrderList = materialOrderList.Where(s => s.subject.AddDate < endDate).ToList();
            //    }
            //}
            if (materialOrderList.Any())
            {
                
                materialOrderList.ForEach(s => {
                    if (!shopIdList.Contains(s.shop.Id))
                    {
                        shopIdList.Add(s.shop.Id);
                    }
                   decimal materialPrice = ((s.materialOrder.MaterialCount ?? 1) * (s.materialOrder.Price ?? 0));
                   if (materialPriceDic.Keys.Contains(s.shop.Id))
                   {
                       materialPriceDic[s.shop.Id] += materialPrice;
                   }
                   else
                   {
                       materialPriceDic.Add(s.shop.Id, materialPrice);
                   }
                });
                if (materialPriceDic.Keys.Count > 0)
                {
                    labMaterialPrice.Text = Math.Round(materialPriceDic.Values.Sum(), 2) + "元";
                }
            }
           
            var shopList = new ShopBLL().GetList(s => shopIdList.Contains(s.Id));
            labShopCount.Text = shopList.Count.ToString();
            AspNetPager1.RecordCount = shopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = shopList.OrderBy(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvList.DataBind();              
        }



        FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
        
        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object shopIdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int shopId = shopIdObj != null ? int.Parse(shopIdObj.ToString()) : 0;
                    object installPriceObj = item.GetType().GetProperty("InstallPrice").GetValue(item,null);
                    decimal installPrice = installPriceObj != null ? decimal.Parse(installPriceObj.ToString()) : 0;
                    var shopOrderList = orderList.Where(s => s.ShopId == shopId).ToList();

                   
                    Label areaLab = (Label)e.Item.FindControl("labArea");
                    Label POPPriceLab = (Label)e.Item.FindControl("labPOPPrice");
                    Label labMaterialPrice = (Label)e.Item.FindControl("labMaterialPrice");
                    
                    //Label subTotalLab = (Label)e.Item.FindControl("labSubTotal");
                    
                    decimal area = 0;
                    decimal popPrice = 0;
                    

                    
                    //StatisticPOPPrice(shopOrderList, out popPrice, out area);
                    StatisticPOPTotalPrice(shopOrderList, out popPrice, out area);
                    areaLab.Text = area.ToString();
                    

                    if (popPrice > 0)
                    {
                        POPPriceLab.Text = Math.Round(popPrice, 2).ToString();
                        //POPPriceLab.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        //POPPriceLab.Attributes.Add("data-shopid", shopId.ToString());
                        //POPPriceLab.Attributes.Add("name", "checkPOPPrice");
                    }
                    if (materialPriceDic.Keys.Contains(shopId))
                    {
                        labMaterialPrice.Text = Math.Round(materialPriceDic[shopId], 2).ToString();
                        //labMaterialPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        //labMaterialPrice.Attributes.Add("data-shopid", shopId.ToString());
                        //labMaterialPrice.Attributes.Add("name", "checkMaterialOrderPrice");
                    }
                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            isSearch = 1;
            BindData();
        }

        public int ShopId { get; set; }
        public string subjectIds { get; set; }
        protected void gvList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Check")
            {
                ShopId = int.Parse(e.CommandArgument.ToString());
                subjectIds = subjectId + ",";
                
                //string url = "ShopOrderDetail.aspx?subjectIds=" + subjectIds + "&shopId=" + shopId;
                string url = "ShopOrderDetail.aspx";
                Server.Transfer(url, false);
            }
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCity();
        }


        void BindProvince()
        {
            cblProvince.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                    regionList.Add(li.Value.ToLower());
            }
            
            if (regionList.Any())
            {

                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                          on order.ShopId equals shop.Id
                          join subject in CurrentContext.DbContext.Subject
                          on order.SubjectId equals subject.Id
                            where order.SubjectId == subjectId
                            && (order.IsDelete == null || order.IsDelete == false)
                            && regionList.Contains(shop.RegionName.ToLower())
                            //&& ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(shop.RegionName.ToLower()))
                            select new { order,shop }).ToList();
                if (subjectChannel == 1)
                {
                    //上海系统单
                    list = list.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel == 2)
                {
                    //分区订单
                    list = list.Where(s => s.order.IsFromRegion==true).ToList();
                }
                if (!string.IsNullOrWhiteSpace(shopType))
                {
                    List<string> shopTypeList = StringHelper.ToStringList(shopType, ',');
                    if (shopTypeList.Any())
                    {
                        if (shopTypeList.Contains("空"))
                        {
                            shopTypeList.Remove("空");
                            if (shopTypeList.Any())
                            {
                                list = list.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                            }
                            else
                                list = list.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                        }
                        else
                            list = list.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                    }
                }
                var provinceList = list.OrderBy(s => s.shop.RegionName).Select(s => s.shop.ProvinceName).Distinct().ToList();
                if (!string.IsNullOrWhiteSpace(province))
                {
                    List<string> provinceList0 = StringHelper.ToStringList(province,',');
                    provinceList = provinceList.Where(s => provinceList0.Contains(s)).ToList();
                }
                if (provinceList.Any())
                {
                    provinceList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s + "&nbsp;";
                        li.Value = s;
                        cblProvince.Items.Add(li);
                    });
                }
            }
            
        }

        void BindCity()
        {
            cblCity.Items.Clear();
           
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinceList.Contains(li.Value))
                    provinceList.Add(li.Value);
            }
            //IEnumerable<FinalOrderDetailTemp> list = null;
            if (provinceList.Any())
            {
               var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                          on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        && provinceList.Contains(order.Province) && (order.IsDelete == null || order.IsDelete == false)
                           select new { order, shop }).ToList();
               if (subjectChannel == 1)
               {
                   //上海系统单
                   list = list.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
               }
               else if (subjectChannel == 2)
               {
                   //分区订单
                   list = list.Where(s => s.order.IsFromRegion==true).ToList();
               }
               if (!string.IsNullOrWhiteSpace(shopType))
               {
                   List<string> shopTypeList = StringHelper.ToStringList(shopType, ',');
                   if (shopTypeList.Any())
                   {
                       if (shopTypeList.Contains("空"))
                       {
                           shopTypeList.Remove("空");
                           if (shopTypeList.Any())
                           {
                               list = list.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                           }
                           else
                               list = list.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                       }
                       else
                           list = list.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                   }
               }
                var cityList = list.OrderBy(s => s.shop.RegionName).ThenBy(s => s.shop.ProvinceName).Select(s => s.shop.CityName).Distinct().ToList();
                if (!string.IsNullOrWhiteSpace(city))
                {
                    List<string> cityList0 = StringHelper.ToStringList(city, ',');
                    cityList = cityList.Where(s => cityList0.Contains(s)).ToList();
                }
                if (cityList.Any())
                {
                    cityList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s + "&nbsp;";
                        li.Value = s;
                        cblCity.Items.Add(li);
                    });
                }
            }
        }

        Dictionary<string, decimal> priceDic = new Dictionary<string, decimal>();
       
    }
}
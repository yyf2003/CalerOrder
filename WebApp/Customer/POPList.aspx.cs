using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using DAL;
using System.Text;
using System.Data;
using Common;

namespace WebApp.Customer
{
    public partial class POPList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                BindData();
                //BindSheet();
            }
        }

        void BindData()
        {
            var list = (from pop in CurrentContext.DbContext.POP
                       join shop in CurrentContext.DbContext.Shop
                       on pop.ShopId equals shop.Id
                       join customer in CurrentContext.DbContext.Customer
                       on shop.CustomerId equals customer.Id
                       //where shop.IsDelete == null || shop.IsDelete == false
                       select new { 
                           customer.CustomerName,
                          shop,
                          pop
                       }).ToList();
            List<string> myRegions = GetResponsibleRegion.Select(s=>s.ToLower()).ToList();
            if (myRegions.Any())
            {
                list = list.Where(s => myRegions.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
            }
            //popList = list.Select(s=>s.pop).ToList();
            if (ddlCustomer.SelectedValue != "0")
            {
                int customerId = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.shop.CustomerId == customerId).ToList();
            }

            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                    regionList.Add(li.Value.ToLower());
            }
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinceList.Contains(li.Value))
                    provinceList.Add(li.Value);
            }
            List<string> channelList = new List<string>();
            foreach (ListItem li in cblChannel.Items)
            {
                if (li.Selected && !channelList.Contains(li.Value))
                    channelList.Add(li.Value);
            }
            List<string> formatList = new List<string>();
            foreach (ListItem li in cblFormat.Items)
            {
                if (li.Selected && !formatList.Contains(li.Value))
                    formatList.Add(li.Value);
            }
            List<string> shopLevelList = new List<string>();
            foreach (ListItem li in cblShopLevel.Items)
            {
                if (li.Selected && !shopLevelList.Contains(li.Value))
                    shopLevelList.Add(li.Value);
            }
            List<string> sheetList = new List<string>();
            foreach (ListItem li in cblSheet.Items)
            {
                if (li.Selected && !sheetList.Contains(li.Value))
                    sheetList.Add(li.Value);
            }
            List<string> genderList = new List<string>();
            foreach (ListItem li in cblGender.Items)
            {
                if (li.Selected && !genderList.Contains(li.Value))
                    genderList.Add(li.Value);
            }

            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                        list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower()) || (s.shop.RegionName == null || s.shop.RegionName == "")).ToList();
                    else
                        list = list.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                }
                else
                    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();

            }
            if (provinceList.Any())
            {
                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                        list = list.Where(s => provinceList.Contains(s.shop.ProvinceName) || (s.shop.ProvinceName == null || s.shop.ProvinceName == "")).ToList();
                    else
                        list = list.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                }
                else
                    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();

            }

            //if (cityTierList.Any())
            //{
            //    if (cityTierList.Contains("空"))
            //    {
            //        cityTierList.Remove("空");
            //        if (cityTierList.Any())
            //            list = list.Where(s => cityTierList.Contains(s.CityTier) || (s.CityTier == null || s.CityTier == "")).ToList();
            //        else
            //            list = list.Where(s => s.CityTier == null || s.CityTier == "").ToList();
            //    }
            //    else
            //        list = list.Where(s => cityTierList.Contains(s.CityTier)).ToList();
            //}
            if (channelList.Any())
            {
                if (channelList.Contains("空"))
                {
                    channelList.Remove("空");
                    if (channelList.Any())
                        list = list.Where(s => channelList.Contains(s.shop.Channel) || (s.shop.Channel == null || s.shop.Channel == "")).ToList();
                    else
                        list = list.Where(s => s.shop.Channel == null || s.shop.Channel == "").ToList();
                }
                else
                    list = list.Where(s => channelList.Contains(s.shop.Channel)).ToList();

            }
            if (formatList.Any())
            {
                if (formatList.Contains("空"))
                {
                    formatList.Remove("空");
                    if (formatList.Any())
                        list = list.Where(s => formatList.Contains(s.shop.Format) || (s.shop.Format == null || s.shop.Format == "")).ToList();
                    else
                        list = list.Where(s => s.shop.Format == null || s.shop.Format == "").ToList();
                }
                else
                    list = list.Where(s => formatList.Contains(s.shop.Format)).ToList();
            }


            if (shopLevelList.Any())
            {
                if (shopLevelList.Contains("空"))
                {
                    shopLevelList.Remove("空");
                    if (shopLevelList.Any())
                        list = list.Where(s => shopLevelList.Contains(s.shop.ShopLevel) || (s.shop.ShopLevel == null || s.shop.ShopLevel == "")).ToList();
                    else
                        list = list.Where(s => s.shop.ShopLevel == null || s.shop.ShopLevel == "").ToList();
                }
                else
                    list = list.Where(s => shopLevelList.Contains(s.shop.ShopLevel)).ToList();
            }
            if (sheetList.Any())
            {
                if (sheetList.Contains("空"))
                {
                    sheetList.Remove("空");
                    if (sheetList.Any())
                        list = list.Where(s => sheetList.Contains(s.pop.Sheet) || (s.pop.Sheet == null || s.pop.Sheet == "")).ToList();
                    else
                        list = list.Where(s => s.pop.Sheet == null || s.pop.Sheet == "").ToList();
                }
                else
                    list = list.Where(s => sheetList.Contains(s.pop.Sheet)).ToList();
            }
            if (genderList.Any())
            {
                if (genderList.Contains("空"))
                {
                    genderList.Remove("空");
                    if (genderList.Any())
                        list = list.Where(s => genderList.Contains(s.pop.Gender) || (s.pop.Gender == null || s.pop.Gender == "")).ToList();
                    else
                        list = list.Where(s => s.pop.Gender == null || s.pop.Gender == "").ToList();
                }
                else
                    list = list.Where(s => genderList.Contains(s.pop.Gender)).ToList();
            }


            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                list = list.Where(s => s.shop.ShopNo.ToLower()==txtShopNo.Text.Trim().ToLower()).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                list = list.Where(s => s.shop.ShopName.ToLower().Contains(txtShopName.Text.Trim().ToLower())).ToList();
            }
            
           
            
            
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.shop.Id).ThenBy(s => s.pop.Sheet).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            //SetPromission(gv);
        }

        
        void BindRegion()
        {
            cblRegion.Items.Clear();
            cblProvince.Items.Clear();
            cblChannel.Items.Clear();
            cblFormat.Items.Clear();
            cblShopLevel.Items.Clear();
            cblSheet.Items.Clear();
            cblGender.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from pop in CurrentContext.DbContext.POP
                        join shop in CurrentContext.DbContext.Shop
                        on pop.ShopId equals shop.Id
                        join customer in CurrentContext.DbContext.Customer
                        on shop.CustomerId equals customer.Id
                        where shop.CustomerId== customerId
                        select new
                        {
                            customer.CustomerName,
                            shop,
                            pop
                        }).ToList();
            List<string> myRegions = GetResponsibleRegion.Select(s=>s.ToLower()).ToList();
            if (myRegions.Any())
            {
                list = list.Where(s => myRegions.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
            }
            if (list.Any())
            {
                var regionList = list.Select(s => s.shop.RegionName).Distinct().ToList();
                bool hasNull = false;
                regionList.ForEach(s =>
                {

                    
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        hasNull = true;
                    }
                    else
                    {
                        ListItem li = new ListItem();
                        li.Value = s;
                        li.Text = s + "&nbsp;";
                        cblRegion.Items.Add(li);
                    }
                    
                });
                if (hasNull)
                    cblRegion.Items.Add(new ListItem("空","空"));
                Session["pop"] = list.Select(s => s.pop).ToList();
                Session["shop"] = list.Select(s => s.shop).ToList();
            }
            else
            {
                Session["pop"] = null;
                Session["shop"] = null;
            }
        }

        void BindProvince()
        {
            cblProvince.Items.Clear();
            cblChannel.Items.Clear();
            cblFormat.Items.Clear();
            cblShopLevel.Items.Clear();
            cblSheet.Items.Clear();
            cblGender.Items.Clear();
            
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                    regionList.Add(li.Value.ToLower());
            }
            if (Session["shop"] != null)
            {
                //var list = new ShopBLL().GetList(s => s.CustomerId == customerId && regionList.Contains(s.RegionName.ToLower()));
                List<Shop> list = Session["shop"] as List<Shop>;
                if (list.Any())
                {

                    if (regionList.Any())
                    {
                        if (regionList.Contains("空"))
                        {
                            regionList.Remove("空");
                            if (regionList.Any())
                                list = list.Where(s => regionList.Contains(s.RegionName.ToLower()) || (s.RegionName == null || s.RegionName == "")).ToList();
                            else
                                list = list.Where(s => s.RegionName == null || s.RegionName == "").ToList();
                        }
                        else
                            list = list.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                       
                    }
                    


                    var provinceList = list.OrderBy(s=>s.ProvinceName).Select(s => s.ProvinceName).Distinct().ToList();
                    bool hasNull = false;
                    provinceList.ForEach(s =>
                    {

                        
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            hasNull = true;
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;";
                            cblProvince.Items.Add(li);
                        }
                        
                    });
                    if(hasNull)
                        cblProvince.Items.Add(new ListItem("空","空"));
                }
            }
            
            
        }

        void BindChannel()
        {
            cblChannel.Items.Clear();
            cblFormat.Items.Clear();
            cblShopLevel.Items.Clear();
            cblSheet.Items.Clear();
            cblGender.Items.Clear();
            if (Session["shop"] != null)
            {
                
                List<Shop> list = Session["shop"] as List<Shop>;
                if (list.Any())
                {
                    List<string> regionList = new List<string>();
                    foreach (ListItem li in cblRegion.Items)
                    {
                        if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                            regionList.Add(li.Value.ToLower());
                    }
                    List<string> provinceList = new List<string>();
                    foreach (ListItem li in cblProvince.Items)
                    {
                        if (li.Selected && !provinceList.Contains(li.Value))
                            provinceList.Add(li.Value);
                    }
                    if (regionList.Any())
                    {
                        if (regionList.Contains("空"))
                        {
                            regionList.Remove("空");
                            if (regionList.Any())
                                list = list.Where(s => regionList.Contains(s.RegionName.ToLower()) || (s.RegionName == null || s.RegionName == "")).ToList();
                            else
                                list = list.Where(s => s.RegionName == null || s.RegionName == "").ToList();
                        }
                        else
                            list = list.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                        if (provinceList.Any())
                        {
                            if (provinceList.Contains("空"))
                            {
                                provinceList.Remove("空");
                                if (provinceList.Any())
                                    list = list.Where(s => provinceList.Contains(s.ProvinceName) || (s.ProvinceName == null || s.ProvinceName == "")).ToList();
                                else
                                    list = list.Where(s => s.ProvinceName == null || s.ProvinceName == "").ToList();
                            }
                            else
                                list = list.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                        }
                    }
                    
                    var channelList = list.OrderBy(s => s.Channel).Select(s => s.Channel).Distinct().ToList();
                    bool hasNull = false;
                    channelList.ForEach(s =>
                    {

                        
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            hasNull = true;
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;";
                            cblChannel.Items.Add(li);
                        }
                        
                    });
                    if(hasNull)
                        cblChannel.Items.Add(new ListItem("空","空"));
                }
            }
        }

        void BindFormat()
        {
            cblFormat.Items.Clear();
            cblShopLevel.Items.Clear();
            cblSheet.Items.Clear();
            cblGender.Items.Clear();
            if (Session["shop"] != null)
            {
                //var list = new ShopBLL().GetList(s => s.CustomerId == customerId && regionList.Contains(s.RegionName.ToLower()));
                List<Shop> list = Session["shop"] as List<Shop>;
                if (list.Any())
                {
                    List<string> regionList = new List<string>();
                    foreach (ListItem li in cblRegion.Items)
                    {
                        if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                            regionList.Add(li.Value.ToLower());
                    }
                    List<string> provinceList = new List<string>();
                    foreach (ListItem li in cblProvince.Items)
                    {
                        if (li.Selected && !provinceList.Contains(li.Value))
                            provinceList.Add(li.Value);
                    }
                    List<string> channelList = new List<string>();
                    foreach (ListItem li in cblChannel.Items)
                    {
                        if (li.Selected && !channelList.Contains(li.Value))
                            channelList.Add(li.Value);
                    }
                    if (regionList.Any())
                    {
                        if (regionList.Contains("空"))
                        {
                            regionList.Remove("空");
                            if (regionList.Any())
                                list = list.Where(s => regionList.Contains(s.RegionName.ToLower()) || (s.RegionName == null || s.RegionName == "")).ToList();
                            else
                                list = list.Where(s => s.RegionName == null || s.RegionName == "").ToList();
                        }
                        else
                            list = list.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                        if (provinceList.Any())
                        {
                            if (provinceList.Contains("空"))
                            {
                                provinceList.Remove("空");
                                if (provinceList.Any())
                                    list = list.Where(s => provinceList.Contains(s.ProvinceName) || (s.ProvinceName == null || s.ProvinceName == "")).ToList();
                                else
                                    list = list.Where(s => s.ProvinceName == null || s.ProvinceName == "").ToList();
                            }
                            else
                                list = list.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                        }
                    }
                    if (channelList.Any())
                    {
                        if (channelList.Contains("空"))
                        {
                            channelList.Remove("空");
                            if (channelList.Any())
                                list = list.Where(s => channelList.Contains(s.Channel) || (s.Channel == null || s.Channel == "")).ToList();
                            else
                                list = list.Where(s => s.Channel == null || s.Channel == "").ToList();
                        }
                        else
                            list = list.Where(s => channelList.Contains(s.Channel)).ToList();
                    }
                    
                    var formatList = list.OrderBy(s => s.Format).Select(s => s.Format).Distinct().ToList();
                    bool hasNull = false;
                    formatList.ForEach(s =>
                    {

                        
                        if (string.IsNullOrWhiteSpace(s))
                        {
                             hasNull = true;
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;";
                            cblFormat.Items.Add(li);
                        }
                        
                    });
                    if(hasNull)
                        cblFormat.Items.Add(new ListItem("空","空"));
                }
            }
        }


        void BindShopLevel()
        {
            cblShopLevel.Items.Clear();
            cblSheet.Items.Clear();
            cblGender.Items.Clear();
            if (Session["shop"] != null)
            {
                //var list = new ShopBLL().GetList(s => s.CustomerId == customerId && regionList.Contains(s.RegionName.ToLower()));
                List<Shop> list = Session["shop"] as List<Shop>;
                if (list.Any())
                {
                    List<string> regionList = new List<string>();
                    foreach (ListItem li in cblRegion.Items)
                    {
                        if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                            regionList.Add(li.Value.ToLower());
                    }
                    List<string> provinceList = new List<string>();
                    foreach (ListItem li in cblProvince.Items)
                    {
                        if (li.Selected && !provinceList.Contains(li.Value))
                            provinceList.Add(li.Value);
                    }
                    List<string> channelList = new List<string>();
                    foreach (ListItem li in cblChannel.Items)
                    {
                        if (li.Selected && !channelList.Contains(li.Value))
                            channelList.Add(li.Value);
                    }
                    List<string> formatList = new List<string>();
                    foreach (ListItem li in cblFormat.Items)
                    {
                        if (li.Selected && !formatList.Contains(li.Value))
                            formatList.Add(li.Value);
                    }
                    if (regionList.Any())
                    {
                        if (regionList.Contains("空"))
                        {
                            regionList.Remove("空");
                            if (regionList.Any())
                                list = list.Where(s => regionList.Contains(s.RegionName.ToLower()) || (s.RegionName == null || s.RegionName == "")).ToList();
                            else
                                list = list.Where(s => s.RegionName == null || s.RegionName == "").ToList();
                        }
                        else
                            list = list.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                        if (provinceList.Any())
                        {
                            if (provinceList.Contains("空"))
                            {
                                provinceList.Remove("空");
                                if (provinceList.Any())
                                    list = list.Where(s => provinceList.Contains(s.ProvinceName) || (s.ProvinceName == null || s.ProvinceName == "")).ToList();
                                else
                                    list = list.Where(s => s.ProvinceName == null || s.ProvinceName == "").ToList();
                            }
                            else
                                list = list.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                        }
                    }
                    if (channelList.Any())
                    {
                        if (channelList.Contains("空"))
                        {
                            channelList.Remove("空");
                            if (channelList.Any())
                                list = list.Where(s => channelList.Contains(s.Channel) || (s.Channel == null || s.Channel == "")).ToList();
                            else
                                list = list.Where(s => s.Channel == null || s.Channel == "").ToList();
                        }
                        else
                            list = list.Where(s => channelList.Contains(s.Channel)).ToList();
                    }
                    if (formatList.Any())
                    {
                        if (formatList.Contains("空"))
                        {
                            formatList.Remove("空");
                            if (formatList.Any())
                                list = list.Where(s => formatList.Contains(s.Format) || (s.Format == null || s.Format == "")).ToList();
                            else
                                list = list.Where(s => s.Format == null || s.Format == "").ToList();
                        }
                        else
                            list = list.Where(s => formatList.Contains(s.Format)).ToList();
                    }


                    //if (regionList.Any())
                    //{
                    //    list = list.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                    //    if (provinceList.Any())
                    //    {
                    //        list = list.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                    //    }
                    //}
                    //if(channelList.Any())
                    //    list = list.Where(s => channelList.Contains(s.Channel)).ToList();
                    var shopLevelList = list.OrderBy(s => s.ShopLevel).Select(s => s.ShopLevel).Distinct().ToList();
                    bool hasNull = false;
                    shopLevelList.ForEach(s =>
                    {


                        if (string.IsNullOrWhiteSpace(s))
                        {
                            hasNull = true;
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;";
                            cblShopLevel.Items.Add(li);
                        }

                    });
                    if (hasNull)
                        cblShopLevel.Items.Add(new ListItem("空", "空"));
                }
            }
        }

        void BindSheet()
        {
            cblSheet.Items.Clear();
            cblGender.Items.Clear();
            
            if (Session["pop"] != null && Session["Shop"] != null)
            {
                List<POP> popList = Session["pop"] as List<POP>;
                List<Shop> shopList = Session["Shop"] as List<Shop>;
                var list = (from pop in popList
                            join shop in shopList
                            on pop.ShopId equals shop.Id
                            select new
                            {
                                pop,
                                shop
                            }).ToList();
                List<string> regionList = new List<string>();
                foreach (ListItem li in cblRegion.Items)
                {
                    if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                        regionList.Add(li.Value.ToLower());
                }
                List<string> provinceList = new List<string>();
                foreach (ListItem li in cblProvince.Items)
                {
                    if (li.Selected && !provinceList.Contains(li.Value))
                        provinceList.Add(li.Value);
                }
                List<string> channelList = new List<string>();
                foreach (ListItem li in cblChannel.Items)
                {
                    if (li.Selected && !channelList.Contains(li.Value))
                        channelList.Add(li.Value);
                }
                List<string> formatList = new List<string>();
                foreach (ListItem li in cblFormat.Items)
                {
                    if (li.Selected && !formatList.Contains(li.Value))
                        formatList.Add(li.Value);
                }
                List<string> shopLevelList = new List<string>();
                foreach (ListItem li in cblShopLevel.Items)
                {
                    if (li.Selected && !shopLevelList.Contains(li.Value))
                        shopLevelList.Add(li.Value);
                }
                if (regionList.Any())
                {
                    if (regionList.Contains("空"))
                    {
                        regionList.Remove("空");
                        if (regionList.Any())
                            list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower()) || (s.shop.RegionName == null || s.shop.RegionName == "")).ToList();
                        else
                            list = list.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                    }
                    else
                        list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (provinceList.Any())
                    {
                        if (provinceList.Contains("空"))
                        {
                            provinceList.Remove("空");
                            if (provinceList.Any())
                                list = list.Where(s => provinceList.Contains(s.shop.ProvinceName) || (s.shop.ProvinceName == null || s.shop.ProvinceName == "")).ToList();
                            else
                                list = list.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                        }
                        else
                            list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    }
                }
                if (channelList.Any())
                {
                    if (channelList.Contains("空"))
                    {
                        channelList.Remove("空");
                        if (channelList.Any())
                            list = list.Where(s => channelList.Contains(s.shop.Channel) || (s.shop.Channel == null || s.shop.Channel == "")).ToList();
                        else
                            list = list.Where(s => s.shop.Channel == null || s.shop.Channel == "").ToList();
                    }
                    else
                        list = list.Where(s => channelList.Contains(s.shop.Channel)).ToList();
                }
                if (formatList.Any())
                {
                    if (formatList.Contains("空"))
                    {
                        formatList.Remove("空");
                        if (formatList.Any())
                            list = list.Where(s => formatList.Contains(s.shop.Format) || (s.shop.Format == null || s.shop.Format == "")).ToList();
                        else
                            list = list.Where(s => s.shop.Format == null || s.shop.Format == "").ToList();
                    }
                    else
                        list = list.Where(s => formatList.Contains(s.shop.Format)).ToList();
                }

                if (shopLevelList.Any())
                {
                    if (shopLevelList.Contains("空"))
                    {
                        shopLevelList.Remove("空");
                        if (shopLevelList.Any())
                            list = list.Where(s => shopLevelList.Contains(s.shop.ShopLevel) || (s.shop.ShopLevel == null || s.shop.ShopLevel == "")).ToList();
                        else
                            list = list.Where(s => s.shop.ShopLevel == null || s.shop.ShopLevel == "").ToList();
                    }
                    else
                        list = list.Where(s => shopLevelList.Contains(s.shop.ShopLevel)).ToList();
                }


                if (list.Any())
                {
                    var sheetList = list.OrderBy(s=>s.pop.Sheet).Select(s => s.pop.Sheet).Distinct().ToList();
                    bool hasNull = false;
                    sheetList.ForEach(s =>
                    {
                        
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            hasNull = true;
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;";
                            cblSheet.Items.Add(li);
                        }
                        
                    });
                    if(hasNull)
                        cblSheet.Items.Add(new ListItem("空","空"));
                }
            }

        }

        void BindGender()
        {
            cblGender.Items.Clear();
            
            if (Session["pop"] != null && Session["Shop"] != null)
            {

                List<POP> popList = Session["pop"] as List<POP>;
                List<Shop> shopList = Session["Shop"] as List<Shop>;
                var list = (from pop in popList
                            join shop in shopList
                            on pop.ShopId equals shop.Id
                            select new
                            {
                                pop,
                                shop
                            }).ToList();
                List<string> regionList = new List<string>();
                foreach (ListItem li in cblRegion.Items)
                {
                    if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                        regionList.Add(li.Value.ToLower());
                }
                List<string> provinceList = new List<string>();
                foreach (ListItem li in cblProvince.Items)
                {
                    if (li.Selected && !provinceList.Contains(li.Value))
                        provinceList.Add(li.Value);
                }
                List<string> channelList = new List<string>();
                foreach (ListItem li in cblChannel.Items)
                {
                    if (li.Selected && !channelList.Contains(li.Value))
                        channelList.Add(li.Value);
                }
                List<string> formatList = new List<string>();
                foreach (ListItem li in cblFormat.Items)
                {
                    if (li.Selected && !formatList.Contains(li.Value))
                        formatList.Add(li.Value);
                }
                List<string> shopLevelList = new List<string>();
                foreach (ListItem li in cblShopLevel.Items)
                {
                    if (li.Selected && !shopLevelList.Contains(li.Value))
                        shopLevelList.Add(li.Value);
                }
                List<string> sheetList = new List<string>();
                foreach (ListItem li in cblSheet.Items)
                {
                    if (li.Selected && !sheetList.Contains(li.Value))
                        sheetList.Add(li.Value);
                }

                if (regionList.Any())
                {
                    if (regionList.Contains("空"))
                    {
                        regionList.Remove("空");
                        if(regionList.Any())
                            list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower()) || (s.shop.RegionName==null || s.shop.RegionName=="")).ToList();
                        else
                            list = list.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                    }
                    else
                       list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (provinceList.Any())
                    {
                        if (provinceList.Contains("空"))
                        {
                            provinceList.Remove("空");
                            if (provinceList.Any())
                                list = list.Where(s => provinceList.Contains(s.shop.ProvinceName) || (s.shop.ProvinceName == null || s.shop.ProvinceName == "")).ToList();
                            else
                                list = list.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                        }
                        else
                           list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    }
                }
                if (channelList.Any())
                {
                    if (channelList.Contains("空"))
                    {
                        channelList.Remove("空");
                        if (channelList.Any())
                            list = list.Where(s => channelList.Contains(s.shop.Channel) || (s.shop.Channel == null || s.shop.Channel == "")).ToList();
                        else
                            list = list.Where(s => s.shop.Channel == null || s.shop.Channel == "").ToList();
                    }
                    else
                       list = list.Where(s => channelList.Contains(s.shop.Channel)).ToList();
                }
                if (formatList.Any())
                {
                    if (formatList.Contains("空"))
                    {
                        formatList.Remove("空");
                        if (formatList.Any())
                            list = list.Where(s => formatList.Contains(s.shop.Format) || (s.shop.Format == null || s.shop.Format == "")).ToList();
                        else
                            list = list.Where(s => s.shop.Format == null || s.shop.Format == "").ToList();
                    }
                    else
                        list = list.Where(s => formatList.Contains(s.shop.Format)).ToList();
                }
                if (shopLevelList.Any())
                {
                    if (shopLevelList.Contains("空"))
                    {
                        shopLevelList.Remove("空");
                        if (shopLevelList.Any())
                            list = list.Where(s => shopLevelList.Contains(s.shop.ShopLevel) || (s.shop.ShopLevel == null || s.shop.ShopLevel == "")).ToList();
                        else
                            list = list.Where(s => s.shop.ShopLevel == null || s.shop.ShopLevel == "").ToList();
                    }
                    else
                        list = list.Where(s => shopLevelList.Contains(s.shop.ShopLevel)).ToList();
                }
                if (sheetList.Any())
                {
                    if (sheetList.Contains("空"))
                    {
                        sheetList.Remove("空");
                        if (sheetList.Any())
                            list = list.Where(s => sheetList.Contains(s.pop.Sheet) || (s.pop.Sheet == null || s.pop.Sheet == "")).ToList();
                        else
                            list = list.Where(s => s.pop.Sheet == null || s.pop.Sheet == "").ToList();
                    }
                    else
                        list = list.Where(s => sheetList.Contains(s.pop.Sheet)).ToList();
                }

                if (list.Any())
                {
                    var genderList = list.OrderByDescending(s=>s.pop.Gender).Select(s => s.pop.Gender).Distinct().ToList();
                    bool hasNull = false;
                    genderList.ForEach(s =>
                    {
                        
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            hasNull = true;
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;";
                            cblGender.Items.Add(li);
                        }
                        
                    });
                    if(hasNull)
                        cblGender.Items.Add(new ListItem("空","空"));
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
            BindProvince();
            BindChannel();
            BindFormat();
            BindShopLevel();
            BindSheet();
            BindGender();
        }

        

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                int popId = int.Parse(e.CommandArgument.ToString());
                POPBLL popBll = new POPBLL();
                POP model = popBll.GetModel(popId);
                if (model != null)
                {
                    popBll.Delete(model);
                    BindData();
                }
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportShopAndPOP_Click(object sender, EventArgs e)
        {
            string where = GetCondition();
            DataSet ds = new ShopBLL().GetShopAndPOPList(where);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "店铺+POP",null,"pop");

            }
            
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
            BindChannel();
            BindFormat();
            BindShopLevel();
            BindSheet();
            BindGender();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindChannel();
            BindFormat();
            BindShopLevel();
            BindSheet();
            BindGender();
        }

        

        protected void cblChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFormat();
            BindShopLevel();
            BindSheet();
            BindGender();
        }

        protected void cblFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindShopLevel();
            BindSheet();
            BindGender();
        }

        protected void cblShopLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            BindSheet();
            BindGender();
        }

        protected void cblSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGender();
        }


        string GetCondition()
        {
            System.Text.StringBuilder where = new System.Text.StringBuilder();
            if (ddlCustomer.SelectedValue != "0")
            {
                int customerId = int.Parse(ddlCustomer.SelectedValue);
                where.AppendFormat(" and Shop.CustomerId={0}", customerId);
            }
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value))
                    regionList.Add(li.Value);
            }
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinceList.Contains(li.Value))
                    provinceList.Add(li.Value);
            }

            List<string> channelList = new List<string>();
            foreach (ListItem li in cblChannel.Items)
            {
                if (li.Selected && !channelList.Contains(li.Value))
                    channelList.Add(li.Value);
            }
            List<string> formatList = new List<string>();
            foreach (ListItem li in cblFormat.Items)
            {
                if (li.Selected && !formatList.Contains(li.Value))
                    formatList.Add(li.Value);
            }
            List<string> shopLevelList = new List<string>();
            foreach (ListItem li in cblShopLevel.Items)
            {
                if (li.Selected && !shopLevelList.Contains(li.Value))
                    shopLevelList.Add(li.Value);
            }
            List<string> sheetList = new List<string>();
            foreach (ListItem li in cblSheet.Items)
            {
                if (li.Selected)
                    sheetList.Add(li.Value);
            }
            List<string> genderList = new List<string>();
            foreach (ListItem li in cblGender.Items)
            {
                if (li.Selected)
                    genderList.Add(li.Value);
            }
            if (regionList.Any())
            {
                StringBuilder regions = new StringBuilder();

                regionList.ForEach(s =>
                {
                    if (s!="空")
                        regions.Append("'" + s + "',");
                });
                if (regionList.Contains("空"))
                {

                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        where.AppendFormat(" and (Shop.RegionName in ({0}) or (Shop.RegionName is null or Shop.RegionName=''))", regions.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (Shop.RegionName is null or Shop.RegionName='')");
                }
                else
                {
                    where.AppendFormat(" and Shop.RegionName in ({0})", regions.ToString().TrimEnd(','));
                }
            }
            if (provinceList.Any())
            {
                StringBuilder provinces = new StringBuilder();

                provinceList.ForEach(s =>
                {
                    if (s != "空")
                        provinces.Append("'" + s + "',");
                });
                if (provinceList.Contains("空"))
                {

                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        where.AppendFormat(" and (Shop.ProvinceName in ({0}) or (Shop.ProvinceName is null or Shop.ProvinceName=''))", provinces.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (Shop.ProvinceName is null or Shop.ProvinceName='')");
                }
                else
                {
                    where.AppendFormat(" and Shop.ProvinceName in ({0})", provinces.ToString().TrimEnd(','));
                }
            }


            if (channelList.Any())
            {
                System.Text.StringBuilder channels = new System.Text.StringBuilder();
                channelList.ForEach(s =>
                {
                    if (s != "空")
                        channels.Append("'" + s + "',");
                });
                if (channelList.Contains("空"))
                {

                    channelList.Remove("空");
                    if (channelList.Any())
                    {
                        where.AppendFormat(" and (Shop.Channel in ({0}) or (Shop.Channel is null or Shop.Channel=''))", channels.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (Shop.Channel is null or Shop.Channel='')");
                }
                else
                {
                    where.AppendFormat(" and Shop.Channel in ({0})", channels.ToString().TrimEnd(','));
                }
            }
            if (formatList.Any())
            {
                System.Text.StringBuilder formats = new System.Text.StringBuilder();
                formatList.ForEach(s =>
                {
                    if (s != "空")
                        formats.Append("'" + s + "',");
                });
                if (formatList.Contains("空"))
                {

                    formatList.Remove("空");
                    if (formatList.Any())
                    {

                        where.AppendFormat(" and (Shop.Format in ({0}) or (Shop.Format is null or Shop.Format=''))", formats.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (Shop.Format is null or Shop.Format='')");
                }
                else
                {
                    where.AppendFormat(" and Shop.Format in ({0})", formats.ToString().TrimEnd(','));
                }
            }
            if (shopLevelList.Any())
            {
                System.Text.StringBuilder shopLevels = new System.Text.StringBuilder();
                shopLevelList.ForEach(s =>
                {
                    if (s != "空")
                        shopLevels.Append("'" + s + "',");
                });
                if (shopLevelList.Contains("空"))
                {

                    shopLevelList.Remove("空");
                    if (shopLevelList.Any())
                    {

                        where.AppendFormat(" and (Shop.ShopLevel in ({0}) or (Shop.ShopLevel is null or Shop.ShopLevel=''))", shopLevels.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (Shop.ShopLevel is null or Shop.ShopLevel='')");
                }
                else
                {
                    where.AppendFormat(" and Shop.ShopLevel in ({0})", shopLevels.ToString().TrimEnd(','));
                }
            }
            if (sheetList.Any())
            {
                System.Text.StringBuilder sheets = new System.Text.StringBuilder();
                sheetList.ForEach(s =>
                {
                    if (s != "空")
                        sheets.Append("'" + s + "',");
                });
                if (sheetList.Contains("空"))
                {

                    sheetList.Remove("空");
                    if (sheetList.Any())
                    {
                        where.AppendFormat(" and (POP.Sheet in ({0}) or (POP.Sheet is null or POP.Sheet=''))", sheets.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (POP.Sheet is null or POP.Sheet='')");
                }
                else
                {
                    where.AppendFormat(" and POP.Sheet in ({0})", sheets.ToString().TrimEnd(','));
                }
            }
            if (genderList.Any())
            {
                StringBuilder genders = new StringBuilder();
                genderList.ForEach(s =>
                {
                    if (s != "空")
                        genders.Append("'" + s + "',");
                });
                if (genderList.Contains("空"))
                {

                    genderList.Remove("空");
                    if (genderList.Any())
                    {
                        where.AppendFormat(" and (POP.Gender in ({0}) or (POP.Gender is null or POP.Gender=''))", genders.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (POP.Gender is null or POP.Gender='')");
                }
                else
                {
                    where.AppendFormat(" and POP.Gender in ({0})", genders.ToString().TrimEnd(','));
                }
            }

            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                where.AppendFormat(" and Shop.ShopNo='{0}'", txtShopNo.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                where.AppendFormat(" and Shop.ShopName like '%{0}%", txtShopName.Text.Trim());
            }
            return where.ToString();
        }
    }
}
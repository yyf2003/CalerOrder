﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using DAL;
using System.Data;
using Common;

namespace WebApp.Customer
{
    public partial class MachineFrameList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                //BindChannel();
                //BindFormat();
                BindData();
            }
        }

        void BindData()
        {
            var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
                       join shop in CurrentContext.DbContext.Shop
                       on frame.ShopId equals shop.Id
                       where (shop.IsDelete==null || shop.IsDelete==false) && (!shop.Status.Contains("闭") || shop.Status ==null || shop.Status=="")
                       select new { 
                          frame,
                          frame.ShopId,
                          frame.MachineFrame,
                          frame.Gender,
                          shop
                       }).ToList();
            List<string> myRegions = GetResponsibleRegion.Select(s=>s.ToLower()).ToList();
            if (myRegions.Any())
            {
                list = list.Where(s => myRegions.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
            }
            if (ddlCustomer.SelectedValue != "0")
            {
                int customerId=int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.shop.CustomerId == customerId).ToList();
            }
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> channelList = new List<string>();
            List<string> formatList = new List<string>();
            List<string> sheetList = new List<string>();
            List<string> CornerTypeList = new List<string>();
            List<string> frameList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value))
                    regionList.Add(li.Value);
            }
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinceList.Contains(li.Value))
                    provinceList.Add(li.Value);
            }
            foreach (ListItem li in cblChannel.Items)
            {
                if (li.Selected && !channelList.Contains(li.Value))
                    channelList.Add(li.Value);
            }
            foreach (ListItem li in cblFormat.Items)
            {
                if (li.Selected && !formatList.Contains(li.Value))
                    formatList.Add(li.Value);
            }
            foreach (ListItem item in cblSheet.Items)
            {
                if (item.Selected && !sheetList.Contains(item.Value))
                {
                    sheetList.Add(item.Value);
                }
            }
            foreach (ListItem item in cblCornerType.Items)
            {
                if (item.Selected && !CornerTypeList.Contains(item.Value))
                {
                    CornerTypeList.Add(item.Value);
                }
            }
            foreach (ListItem item in cblFrame.Items)
            {
                if (item.Selected && !frameList.Contains(item.Value))
                {
                    frameList.Add(item.Value);
                }
            }

            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                        list = list.Where(s => regionList.Contains(s.shop.RegionName) || (s.shop.RegionName == null || s.shop.RegionName == "")).ToList();
                    else
                        list = list.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                }
                else
                    list = list.Where(s => regionList.Contains(s.shop.RegionName)).ToList();

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
            if (channelList.Any())
            {
                if (channelList.Contains("空"))
                {
                    channelList.Remove("空");
                    if (channelList.Any())
                    {
                        list = list.Where(s => channelList.Contains(s.shop.Channel) || (s.shop.Channel == null || s.shop.Channel == "")).ToList();
                    }
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
                    {
                        list = list.Where(s => formatList.Contains(s.shop.Format) || (s.shop.Format == null || s.shop.Format == "")).ToList();
                    }
                    else
                        list = list.Where(s => s.shop.Format == null || s.shop.Format == "").ToList();
                }
                else
                    list = list.Where(s => formatList.Contains(s.shop.Format)).ToList();
            }
            if (sheetList.Any())
            {
                list = list.Where(s => sheetList.Contains(s.frame.PositionName)).ToList();
            }
            if (CornerTypeList.Any())
            {
                if (CornerTypeList.Contains("空"))
                {
                    CornerTypeList.Remove("空");
                    if (CornerTypeList.Any())
                    {
                        list = list.Where(s => CornerTypeList.Contains(s.frame.CornerType) || (s.frame.CornerType == null || s.frame.CornerType == "")).ToList();
                    }
                    else
                        list = list.Where(s => s.frame.CornerType == null || s.frame.CornerType == "").ToList();
                }
                else
                    list = list.Where(s => CornerTypeList.Contains(s.frame.CornerType)).ToList();
            }
            if (frameList.Any())
            {
                list = list.Where(s => frameList.Contains(s.frame.MachineFrame)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {

                list = list.Where(s => s.shop.ShopNo.ToLower() == txtShopNo.Text.Trim().ToLower()).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                list = list.Where(s => s.shop.ShopName.ToLower().Contains(txtShopName.Text.Trim().ToLower())).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.frame.PositionName).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            //SetPromission(ref gv, new object[] { btnAdd, btnImport });
            //SetPromission(gv);
        }

        void BindRegion()
        {
            //ddlRegion.Items.Clear();
            //int customerId = int.Parse(ddlCustomer.SelectedValue);
            ////var list = new ShopBLL().GetList(s => s.CustomerId == customerId);
            //var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
            //           join shop in CurrentContext.DbContext.Shop
            //           on frame.ShopId equals shop.Id
            //           where shop.CustomerId == customerId
            //           select shop).ToList();
            //if (list.Any())
            //{
            //    List<string> regionList = new List<string>();
            //    list.ForEach(s =>
            //    {
            //        if (!string.IsNullOrWhiteSpace(s.RegionName) && !regionList.Contains(s.RegionName))
            //        {
            //            regionList.Add(s.RegionName);
            //            ddlRegion.Items.Add(new ListItem(s.RegionName, s.RegionName));
            //        }
            //    });
            //}
            //ddlRegion.Items.Insert(0, new ListItem("--请选择--", "0"));
            cblRegion.Items.Clear();
            cblProvince.Items.Clear();
            ////cblCity.Items.Clear();
            //cblCityTier.Items.Clear();
            //cblChannel.Items.Clear();
            //cblFormat.Items.Clear();
            //cblShopLevel.Items.Clear();
            //cblIsInstall.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
                       join shop in CurrentContext.DbContext.Shop
                       on frame.ShopId equals shop.Id
                       where shop.CustomerId == customerId
                       select new { 
                          shop,
                          frame
                       }).ToList();
            List<string> myRegions = GetResponsibleRegion;
            if (myRegions.Any())
            {
                list = list.Where(s => myRegions.Contains(s.shop.RegionName.ToUpper()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
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
                    cblRegion.Items.Add(new ListItem("空", "空"));
                Session["frame"] = list.Select(s => s.frame).ToList();
                Session["shop"] = list.Select(s => s.shop).ToList();
            }
            else
            {
                Session["frame"] = null;
                Session["shop"] = null;
            }
        }

        void BindProvince()
        {
            //ddlProvince.Items.Clear();
            //int customerId = int.Parse(ddlCustomer.SelectedValue);
            //string regionId = ddlRegion.SelectedValue;
            ////var list = new ShopBLL().GetList(s => s.CustomerId == customerId && s.RegionName == regionId);
            //var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
            //            join shop in CurrentContext.DbContext.Shop
            //            on frame.ShopId equals shop.Id
            //            where shop.CustomerId == customerId && shop.RegionName==regionId
            //            select shop).ToList();
            //if (list.Any())
            //{
            //    List<string> provinceList = new List<string>();
            //    list.ForEach(s =>
            //    {
            //        if (!string.IsNullOrWhiteSpace(s.ProvinceName) && !provinceList.Contains(s.ProvinceName))
            //        {
            //            provinceList.Add(s.ProvinceName);
            //            ddlProvince.Items.Add(new ListItem(s.ProvinceName, s.ProvinceName));
            //        }
            //    });
            //}
            //ddlProvince.Items.Insert(0, new ListItem("--请选择--","0"));


            cblProvince.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                    regionList.Add(li.Value.ToLower());
            }
            if (Session["shop"] != null)
            {
                
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
                    var provinceList = list.OrderBy(s => s.ProvinceName).Select(s => s.ProvinceName).Distinct().ToList();
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
                    if (hasNull)
                        cblProvince.Items.Add(new ListItem("空", "空"));
                }
            }


        }

        void BindChannel()
        {
            //cblChannel.Items.Clear();
            //int customerId = int.Parse(ddlCustomer.SelectedValue);
            //string regionName = ddlRegion.SelectedValue;
            //string provinceName = ddlProvince.SelectedValue;
            //var shopList = new ShopBLL().GetList(s => s.CustomerId == customerId&& (s.IsDelete == null || s.IsDelete == false));
            //if (regionName!="0")
            //{
            //    shopList = shopList.Where(s => s.RegionName.ToLower() == regionName.ToLower()).ToList();
            //}
            //if (provinceName!="0")
            //{
            //    shopList = shopList.Where(s => s.ProvinceName == provinceName).ToList();
            //}
            //var channelList = shopList.Select(s => s.Channel).Distinct().OrderByDescending(s=>s).ToList();
            //channelList.ForEach(s => {
            //    ListItem li = new ListItem();
            //    if (!string.IsNullOrWhiteSpace(s))
            //    {
            //        li.Text = s + "&nbsp;";
            //        li.Value = s;
            //    }
            //    else
            //    {
            //        li.Text = "空&nbsp;";
            //        li.Value = "空";
            //    }
            //    if (cblChannel.Items.FindByValue(li.Value)==null)
            //       cblChannel.Items.Add(li);
            //});
            cblChannel.Items.Clear();
            
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
                    if (hasNull)
                        cblChannel.Items.Add(new ListItem("空", "空"));
                }
            }

        }

        void BindFormat()
        {
            //cblFormat.Items.Clear();
            //int customerId = int.Parse(ddlCustomer.SelectedValue);
            //string regionName = ddlRegion.SelectedValue;
            //string provinceName = ddlProvince.SelectedValue;
            //List<string> channelList = new List<string>();
            //foreach (ListItem li in cblChannel.Items)
            //{
            //    if (li.Selected && !channelList.Contains(li.Value))
            //        channelList.Add(li.Value);
            //}
            //var shopList = new ShopBLL().GetList(s => s.CustomerId == customerId&& (s.IsDelete == null || s.IsDelete == false));
            //if (regionName != "0")
            //{
            //    shopList = shopList.Where(s => s.RegionName.ToLower() == regionName.ToLower()).ToList();
            //}
            //if (provinceName != "0")
            //{
            //    shopList = shopList.Where(s => s.ProvinceName == provinceName).ToList();
            //}
            //if (channelList.Any())
            //{
            //    if (channelList.Contains("空"))
            //    {
            //        channelList.Remove("空");
            //        if (channelList.Any())
            //            shopList = shopList.Where(s => channelList.Contains(s.Channel) || (s.Channel == null || s.Channel == "")).ToList();
            //        else
            //            shopList = shopList.Where(s => s.Channel == null || s.Channel == "").ToList();
            //    }
            //    else
            //        shopList = shopList.Where(s => channelList.Contains(s.Channel)).ToList();
            //}
            //var formatList = shopList.Select(s => s.Format).Distinct().OrderByDescending(s => s).ToList();
            //formatList.ForEach(s =>
            //{
            //    ListItem li = new ListItem();
            //    if (!string.IsNullOrWhiteSpace(s))
            //    {
            //        li.Text = s + "&nbsp;";
            //        li.Value = s;
            //    }
            //    else
            //    {
            //        li.Text = "空&nbsp;";
            //        li.Value = "空";
            //    }
            //    if (cblFormat.Items.FindByValue(li.Value) == null)
            //       cblFormat.Items.Add(li);
            //});
            cblFormat.Items.Clear();
           
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
                    if (hasNull)
                        cblFormat.Items.Add(new ListItem("空", "空"));
                }
            }
        }

        void BindSheet()
        {
            //cblSheet.Items.Clear();
            //int customerId = int.Parse(ddlCustomer.SelectedValue);
            //var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
            //            join shop in CurrentContext.DbContext.Shop
            //            on frame.ShopId equals shop.Id
            //            where shop.CustomerId == customerId && (shop.IsDelete == null || shop.IsDelete == false) && !shop.Status.Contains("闭")
            //            select frame).ToList();
            //if (list.Any())
            //{
            //    List<string> sheetList = new List<string>();
            //    list.ForEach(s =>
            //    {
            //        if (!string.IsNullOrWhiteSpace(s.PositionName) && !sheetList.Contains(s.PositionName))
            //        {
            //            sheetList.Add(s.PositionName);
            //            cblSheet.Items.Add(new ListItem(s.PositionName + "&nbsp;", s.PositionName));
            //        }
            //    });
            //}
            cblSheet.Items.Clear();
            if (Session["frame"] != null && Session["Shop"] != null)
            {
                List<ShopMachineFrame> frameList = Session["frame"] as List<ShopMachineFrame>;
                List<Shop> shopList = Session["Shop"] as List<Shop>;
                var list = (from frame in frameList
                            join shop in shopList
                            on frame.ShopId equals shop.Id
                            select new
                            {
                                frame,
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

               


                if (list.Any())
                {
                    var sheetList = list.OrderByDescending(s => s.frame.PositionName).Select(s => s.frame.PositionName).Distinct().ToList();
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
                    if (hasNull)
                        cblSheet.Items.Add(new ListItem("空", "空"));
                }
            }
        }

        void BindCornerType()
        {
            //List<string> sheets = new List<string>();
            //cblCornerType.Items.Clear();
            //foreach (ListItem item in cblSheet.Items)
            //{
            //    if (item.Selected)
            //    {
            //        if (!sheets.Contains(item.Value))
            //            sheets.Add(item.Value);
            //    }
            //}
            //if (sheets.Any())
            //{
            //    int customerId = int.Parse(ddlCustomer.SelectedValue);
            //    var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
            //                join shop in CurrentContext.DbContext.Shop
            //                on frame.ShopId equals shop.Id
            //                where shop.CustomerId == customerId && sheets.Contains(frame.PositionName)
            //                select frame.CornerType).OrderBy(s => s).ToList();
            //    if (list.Any())
            //    {
            //        List<string> CornerTypeList = new List<string>();
            //        list.ForEach(s =>
            //        {
            //            if (!string.IsNullOrWhiteSpace(s) && !CornerTypeList.Contains(s))
            //            {
            //                CornerTypeList.Add(s);
            //                cblCornerType.Items.Add(new ListItem(s + "&nbsp;", s));
            //            }
            //        });
            //    }
            //}
            cblCornerType.Items.Clear();
            if (Session["frame"] != null && Session["Shop"] != null)
            {
                List<ShopMachineFrame> frameList = Session["frame"] as List<ShopMachineFrame>;
                List<Shop> shopList = Session["Shop"] as List<Shop>;
                var list = (from frame in frameList
                            join shop in shopList
                            on frame.ShopId equals shop.Id
                            select new
                            {
                                frame,
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
                List<string> sheetList = new List<string>();
                foreach (ListItem item in cblSheet.Items)
                {
                    if (item.Selected)
                    {
                        if (!sheetList.Contains(item.Value))
                            sheetList.Add(item.Value);
                    }
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
                if (sheetList.Any())
                {
                    if (sheetList.Contains("空"))
                    {
                        sheetList.Remove("空");
                        if (sheetList.Any())
                            list = list.Where(s => sheetList.Contains(s.frame.PositionName) || (s.frame.PositionName == null || s.frame.PositionName == "")).ToList();
                        else
                            list = list.Where(s => s.frame.PositionName == null || s.frame.PositionName == "").ToList();
                    }
                    else
                        list = list.Where(s => sheetList.Contains(s.frame.PositionName)).ToList();
                }



                if (list.Any())
                {
                    var cornerTypeList = list.OrderByDescending(s => s.frame.CornerType).Select(s => s.frame.CornerType).Distinct().ToList();
                    bool hasNull = false;
                    cornerTypeList.ForEach(s =>
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
                            cblCornerType.Items.Add(li);
                        }

                    });
                    if (hasNull)
                        cblCornerType.Items.Add(new ListItem("空", "空"));
                }
            }
        }

        void BindFrame()
        {
            cblFrame.Items.Clear();
            //List<string> sheets = new List<string>();
            //foreach (ListItem item in cblSheet.Items)
            //{
            //    if (item.Selected)
            //    {
            //        if (!sheets.Contains(item.Value))
            //            sheets.Add(item.Value);
            //    }
            //}
            //List<string> CornerTypeList = new List<string>();
            //foreach (ListItem item in cblCornerType.Items)
            //{
            //    if (item.Selected)
            //    {
            //        if (!CornerTypeList.Contains(item.Value))
            //            CornerTypeList.Add(item.Value);
            //    }
            //}
            //if (sheets.Any())
            //{
            //    int customerId = int.Parse(ddlCustomer.SelectedValue);
            //    var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
            //                join shop in CurrentContext.DbContext.Shop
            //                on frame.ShopId equals shop.Id
            //                where shop.CustomerId == customerId && sheets.Contains(frame.PositionName) && (CornerTypeList.Any()?(CornerTypeList.Contains(frame.CornerType)):1==1)
            //                select frame.MachineFrame).OrderBy(s=>s).ToList();
            //    if (list.Any())
            //    {
            //        List<string> frameList = new List<string>();
            //        list.ForEach(s =>
            //        {
            //            if (!string.IsNullOrWhiteSpace(s) && !frameList.Contains(s))
            //            {
            //                frameList.Add(s);
            //                cblFrame.Items.Add(new ListItem(s+"&nbsp;", s));
            //            }
            //        });
            //    }
            //}


            if (Session["frame"] != null && Session["Shop"] != null)
            {
                List<ShopMachineFrame> frameList = Session["frame"] as List<ShopMachineFrame>;
                List<Shop> shopList = Session["Shop"] as List<Shop>;
                var list = (from frame in frameList
                            join shop in shopList
                            on frame.ShopId equals shop.Id
                            select new
                            {
                                frame,
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
                List<string> sheetList = new List<string>();
                foreach (ListItem item in cblSheet.Items)
                {
                    if (item.Selected)
                    {
                        if (!sheetList.Contains(item.Value))
                            sheetList.Add(item.Value);
                    }
                }
                List<string> cornerTypeList = new List<string>();
                foreach (ListItem item in cblCornerType.Items)
                {
                    if (item.Selected)
                    {
                        if (!cornerTypeList.Contains(item.Value))
                            cornerTypeList.Add(item.Value);
                    }
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
                if (sheetList.Any())
                {
                    if (sheetList.Contains("空"))
                    {
                        sheetList.Remove("空");
                        if (sheetList.Any())
                            list = list.Where(s => sheetList.Contains(s.frame.PositionName) || (s.frame.PositionName == null || s.frame.PositionName == "")).ToList();
                        else
                            list = list.Where(s => s.frame.PositionName == null || s.frame.PositionName == "").ToList();
                    }
                    else
                        list = list.Where(s => sheetList.Contains(s.frame.PositionName)).ToList();
                }
                if (cornerTypeList.Any())
                {
                    if (cornerTypeList.Contains("空"))
                    {
                        cornerTypeList.Remove("空");
                        if (cornerTypeList.Any())
                            list = list.Where(s => cornerTypeList.Contains(s.frame.CornerType) || (s.frame.CornerType == null || s.frame.CornerType == "")).ToList();
                        else
                            list = list.Where(s => s.frame.CornerType == null || s.frame.CornerType == "").ToList();
                    }
                    else
                        list = list.Where(s => cornerTypeList.Contains(s.frame.CornerType)).ToList();
                }


                if (list.Any())
                {
                    var frameNameList = list.OrderBy(s => s.frame.MachineFrame).Select(s => s.frame.MachineFrame).Distinct().ToList();
                    bool hasNull = false;
                    frameNameList.ForEach(s =>
                    {

                        if (string.IsNullOrWhiteSpace(s))
                        {
                            hasNull = true;
                        }
                        else
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;&nbsp;";
                            cblFrame.Items.Add(li);
                        }

                    });
                    if (hasNull)
                        cblFrame.Items.Add(new ListItem("空", "空"));
                }
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportShop_Click(object sender, EventArgs e)
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            string where = GetCondition();
            DataSet ds = new ShopMachineFrameBLL().GetDataList(where);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "店铺器架信息", null, "shopFrame");

            }
           
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

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
            BindChannel();
            BindFormat();
            BindSheet();
            BindCornerType();
            BindFrame();
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
            BindChannel();
            BindFormat();
            BindSheet();
            BindCornerType();
            BindFrame();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            BindChannel();
            BindFormat();
            BindSheet();
            BindCornerType();
            BindFrame();
        }

        protected void cblChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFormat();
            BindSheet();
            BindCornerType();
            BindFrame();
        }

        protected void cblFormat_SelectedIndexChanged(object sender, EventArgs e)
        {

            BindSheet();
            BindCornerType();
            BindFrame();
        }

        /// <summary>
        /// 选择位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cblSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCornerType();
            BindFrame();
        }

        protected void cblCornerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFrame();
        }

        string GetCondition()
        {
            System.Text.StringBuilder where = new System.Text.StringBuilder();
            if (ddlCustomer.SelectedValue != "0")
            {
                int customerId = int.Parse(ddlCustomer.SelectedValue);
                where.AppendFormat(" and Shop.CustomerId={0}", customerId);
            }
            
            //if (ddlRegion.SelectedValue != "0")
            //{
            //    where.AppendFormat(" and Shop.RegionName ='{0}'", ddlRegion.SelectedValue);
            //}
            //if (ddlProvince.SelectedValue != "0")
            //{
            //    where.AppendFormat(" and Shop.ProvinceName ='{0}'", ddlProvince.SelectedValue);
            //}
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> channelList = new List<string>();
            List<string> formatList = new List<string>();
            List<string> sheetList = new List<string>();
            List<string> cornerTypeList = new List<string>();
            List<string> frameList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value))
                    regionList.Add(li.Value);
            }
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinceList.Contains(li.Value))
                    provinceList.Add(li.Value);
            }
            foreach (ListItem li in cblChannel.Items)
            {
                if (li.Selected && !channelList.Contains(li.Value))
                    channelList.Add(li.Value);
            }
            foreach (ListItem li in cblFormat.Items)
            {
                if (li.Selected && !formatList.Contains(li.Value))
                    formatList.Add(li.Value);
            }
            foreach (ListItem li in cblSheet.Items)
            {
                if (li.Selected && !sheetList.Contains(li.Value))
                    sheetList.Add(li.Value);
            }
            foreach (ListItem li in cblCornerType.Items)
            {
                if (li.Selected && !cornerTypeList.Contains(li.Value))
                    cornerTypeList.Add(li.Value);
            }
            foreach (ListItem li in cblFrame.Items)
            {
                if (li.Selected && !frameList.Contains(li.Value))
                    frameList.Add(li.Value);
            }
            if (regionList.Any())
            {
                System.Text.StringBuilder regions = new System.Text.StringBuilder();
                regionList.ForEach(s =>
                {
                    if (s != "空")
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
                System.Text.StringBuilder provinces = new System.Text.StringBuilder();
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
                    if(s!="空")
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

                        where.AppendFormat(" and (ShopMachineFrame.PositionName in ({0}) or (ShopMachineFrame.PositionName is null or ShopMachineFrame.PositionName=''))", sheets.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (ShopMachineFrame.PositionName is null or ShopMachineFrame.PositionName='')");
                }
                else
                {
                    where.AppendFormat(" and ShopMachineFrame.PositionName in ({0})", sheets.ToString().TrimEnd(','));
                }
            }
            if (cornerTypeList.Any())
            {
                System.Text.StringBuilder cornerTypes = new System.Text.StringBuilder();
                cornerTypeList.ForEach(s =>
                {
                    if (s != "空")
                        cornerTypes.Append("'" + s + "',");
                });
                if (cornerTypeList.Contains("空"))
                {

                    cornerTypeList.Remove("空");
                    if (cornerTypeList.Any())
                    {

                        where.AppendFormat(" and (ShopMachineFrame.CornerType in ({0}) or (ShopMachineFrame.CornerType is null or ShopMachineFrame.CornerType=''))", cornerTypes.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (ShopMachineFrame.CornerType is null or ShopMachineFrame.CornerType='')");
                }
                else
                {
                    where.AppendFormat(" and ShopMachineFrame.CornerType in ({0})", cornerTypes.ToString().TrimEnd(','));
                }
            }
            if (frameList.Any())
            {
                System.Text.StringBuilder frames = new System.Text.StringBuilder();
                frameList.ForEach(s =>
                {
                    if (s != "空")
                        frames.Append("'" + s + "',");
                });
                if (frameList.Contains("空"))
                {

                    frameList.Remove("空");
                    if (frameList.Any())
                    {

                        where.AppendFormat(" and (ShopMachineFrame.MachineFrame in ({0}) or (ShopMachineFrame.MachineFrame is null or ShopMachineFrame.MachineFrame=''))", frames.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (ShopMachineFrame.MachineFrame is null or ShopMachineFrame.MachineFrame='')");
                }
                else
                {
                    where.AppendFormat(" and ShopMachineFrame.MachineFrame in ({0})", frames.ToString().TrimEnd(','));
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

       

        HCPOPBLL HCPOPBll = new HCPOPBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowIndex != -1)
            //{
            //    object item = e.Row.DataItem;
            //    if (item != null)
            //    {
            //        object shopIdObj = item.GetType().GetProperty("ShopId").GetValue(item, null);
            //        object frameNameObj = item.GetType().GetProperty("MachineFrame").GetValue(item, null);
            //        object genderObj = item.GetType().GetProperty("Gender").GetValue(item, null);
            //        int shopId = shopIdObj != null ? int.Parse(shopIdObj.ToString()) : 0;
            //        string frameName = frameNameObj != null ? frameNameObj.ToString() : "";
            //        string gender = genderObj != null ? genderObj.ToString() : "";
            //        var list = HCPOPBll.GetList(s => s.ShopId == shopId && s.MachineFrame == frameName && s.MachineFrameGender == gender);
            //        if (!list.Any())
            //        {
            //            ((Label)e.Row.FindControl("labCheckHCPOP")).Text = "";   
            //        }
            //    }
            //}
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //if (e.CommandName == "DeleteItem")
            //{
            //    int frameId = int.Parse(e.CommandArgument.ToString());
            //    ShopMachineFrameBLL bll = new ShopMachineFrameBLL();
            //    ShopMachineFrame model = bll.GetModel(frameId);
            //    if (model != null)
            //    {
            //        bll.Delete(model);
            //        BindData();
            //    }
            //}
        }

       

        

       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Data;
using Common;
using System.Transactions;
using System.Text;
using System.Configuration;
using LitJson;
using System.IO;

namespace WebApp.Customer
{
    public partial class ShopList : BasePage
    {
        //string promissionStr = string.Empty;
        ShopBLL shopBll = new ShopBLL();
        public string url = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            url = Request.FilePath;
            //promissionStr = GetPromissionStr(url);
            if (!IsPostBack)
            {

                BindCustomerList(ref ddlCustomer);
                var shopStuts = CommonMethod.GetEnumList<ShopStatusEnum>();
                if (shopStuts.Any())
                {
                    shopStuts.ForEach(s => {
                        ListItem li = new ListItem();
                        li.Value = s.Name;
                        li.Text = s.Name + "&nbsp;";
                        rblStatus.Items.Add(li);
                    });
                }
                BindOutsource();
                BindData();
                GetUserExportChannel();
                //string userExportPermission = string.Empty;
                //try
                //{
                //    userExportPermission = ConfigurationManager.AppSettings["CanExportUser"];
                //}
                //catch
                //{

                //}
                //if (!string.IsNullOrWhiteSpace(userExportPermission))
                //{
                //    string[] permissions = userExportPermission.Split('#');
                //    foreach (string s in permissions)
                //    {
                //        int userId = int.Parse(s.Split('|')[0]);
                //        //string format = s.Split('|')[1];

                //        string channel = s.Split('|')[1];
                //        if (CurrentUser.UserId == userId)
                //        {
                //            btnExportShop.Visible = true;
                //            btnExportShopAndPOP.Visible = true;
                //            btnExportShopAndFrame.Visible = true;
                //            ViewState["ExportChannel"] = channel;
                //            break;
                //        }
                //    }

                    
                //}
            }
        }

        void BindOutsource()
        {
            var list = new CompanyBLL().GetList(s=>s.TypeId==(int)CompanyTypeEnum.Outsource && (s.IsDelete==null || s.IsDelete==false)).ToList();
            ddlOutsource.DataSource = list;
            ddlOutsource.DataTextField = "CompanyName";
            ddlOutsource.DataValueField = "Id";
            ddlOutsource.DataBind();
            ddlOutsource.Items.Insert(0,new ListItem("--请选择外协--","0"));
        }

        void BindRegion()
        {

            cblRegion.Items.Clear();
            cblProvince.Items.Clear();
            //cblCity.Items.Clear();
            cblCityTier.Items.Clear();
            cblChannel.Items.Clear();
            cblFormat.Items.Clear();
            //cblShopLevel.Items.Clear();
            cblIsInstall.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = new ShopBLL().GetList(s => s.CustomerId == customerId);
            List<string> myRegions = GetResponsibleRegion.Select(s=>s.ToLower()).ToList();
            if (myRegions.Any())
            {
                list = list.Where(s => myRegions.Contains(s.RegionName.ToLower()) || s.RegionName == null || s.RegionName == "").ToList();
            }
            if (list.Any())
            {

                var regionList = list.Select(s => s.RegionName).Distinct().ToList();
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
                Session["shopList"] = list;
            }
            else
            {
                Session["shopList"] = null;
            }
        }

        void BindProvince()
        {

            cblProvince.Items.Clear();
            //cblCity.Items.Clear();
            cblCityTier.Items.Clear();
            cblChannel.Items.Clear();
            cblFormat.Items.Clear();
            //cblShopLevel.Items.Clear();
            cblIsInstall.Items.Clear();
            if (Session["shopList"] != null)
            {

                List<Shop> list = Session["shopList"] as List<Shop>;
                if (list.Any())
                {
                    List<string> regionList = new List<string>();
                    foreach (ListItem li in cblRegion.Items)
                    {
                        if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                            regionList.Add(li.Value.ToLower());
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

        //void BindCity()
        //{
        //    cblCity.Items.Clear();
        //    cblCityTier.Items.Clear();
        //    cblChannel.Items.Clear();
        //    cblFormat.Items.Clear();
        //    cblShopLevel.Items.Clear();

        //    if (Session["shop"] != null)
        //    {

        //        List<Shop> list = Session["shop"] as List<Shop>;
        //        if (list.Any())
        //        {
        //            List<string> regionList = new List<string>();
        //            foreach (ListItem li in cblRegion.Items)
        //            {
        //                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
        //                    regionList.Add(li.Value.ToLower());
        //            }
        //            List<string> provinceList = new List<string>();
        //            foreach (ListItem li in cblProvince.Items)
        //            {
        //                if (li.Selected && !provinceList.Contains(li.Value))
        //                    provinceList.Add(li.Value);
        //            }
        //            if (regionList.Any())
        //            {
        //                if (regionList.Contains("空"))
        //                {
        //                    regionList.Remove("空");
        //                    if (regionList.Any())
        //                        list = list.Where(s => regionList.Contains(s.RegionName.ToLower()) || (s.RegionName == null || s.RegionName == "")).ToList();
        //                    else
        //                        list = list.Where(s => s.RegionName == null || s.RegionName == "").ToList();
        //                }
        //                else
        //                    list = list.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
        //                if (provinceList.Any())
        //                {
        //                    if (provinceList.Contains("空"))
        //                    {
        //                        provinceList.Remove("空");
        //                        if (provinceList.Any())
        //                            list = list.Where(s => provinceList.Contains(s.ProvinceName) || (s.ProvinceName == null || s.ProvinceName == "")).ToList();
        //                        else
        //                            list = list.Where(s => s.ProvinceName == null || s.ProvinceName == "").ToList();
        //                    }
        //                    else
        //                        list = list.Where(s => provinceList.Contains(s.ProvinceName)).ToList();

        //                }
        //            }



        //            var cityList = list.OrderBy(s => s.CityName).Select(s => s.CityName).Distinct().ToList();
        //            cityList.ForEach(s =>
        //            {

        //                ListItem li = new ListItem();
        //                if (string.IsNullOrWhiteSpace(s))
        //                {
        //                    li.Value = "空";
        //                    li.Text = "空&nbsp;";
        //                }
        //                else
        //                {
        //                    li.Value = s;
        //                    li.Text = s + "&nbsp;";
        //                }
        //                cblCity.Items.Add(li);
        //            });
        //        }
        //    }
        //}

        void BindCityTier()
        {
            cblCityTier.Items.Clear();
            cblChannel.Items.Clear();
            cblFormat.Items.Clear();
            //cblShopLevel.Items.Clear();
            cblIsInstall.Items.Clear();
            if (Session["shopList"] != null)
            {

                List<Shop> list = Session["shopList"] as List<Shop>;
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
                    //List<string> cityList = new List<string>();
                    //foreach (ListItem li in cblCity.Items)
                    //{
                    //    if (li.Selected && !cityList.Contains(li.Value))
                    //        cityList.Add(li.Value);
                    //}
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
                    //if (cityList.Any())
                    //{
                    //    if (cityList.Contains("空"))
                    //    {
                    //        cityList.Remove("空");
                    //        if (cityList.Any())
                    //            list = list.Where(s => cityList.Contains(s.CityName) || (s.CityName == null || s.CityName == "")).ToList();
                    //        else
                    //            list = list.Where(s => s.CityName == null || s.CityName == "").ToList();
                    //    }
                    //    else
                    //        list = list.Where(s => cityList.Contains(s.CityName)).ToList();

                    //}

                    var cityTierList = list.OrderBy(s => s.CityTier).Select(s => s.CityTier).Distinct().ToList();
                    bool hasNull = false;
                    cityTierList.ForEach(s =>
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
                            cblCityTier.Items.Add(li);
                        }

                    });
                    if (hasNull)
                        cblCityTier.Items.Add(new ListItem("空", "空"));
                }
            }
        }

        void BindChannel()
        {
            cblChannel.Items.Clear();
            cblFormat.Items.Clear();
            //cblShopLevel.Items.Clear();
            cblIsInstall.Items.Clear();

            if (Session["shopList"] != null)
            {

                List<Shop> list = Session["shopList"] as List<Shop>;
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
                    //List<string> cityList = new List<string>();
                    //foreach (ListItem li in cblCity.Items)
                    //{
                    //    if (li.Selected && !cityList.Contains(li.Value))
                    //        cityList.Add(li.Value);
                    //}
                    List<string> cityTierList = new List<string>();
                    foreach (ListItem li in cblCityTier.Items)
                    {
                        if (li.Selected && !cityTierList.Contains(li.Value))
                            cityTierList.Add(li.Value);
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

                    }
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
                    //if (cityList.Any())
                    //{
                    //    if (cityList.Contains("空"))
                    //    {
                    //        cityList.Remove("空");
                    //        if (cityList.Any())
                    //            list = list.Where(s => cityList.Contains(s.CityName) || (s.CityName == null || s.CityName == "")).ToList();
                    //        else
                    //            list = list.Where(s => s.CityName == null || s.CityName == "").ToList();
                    //    }
                    //    else
                    //        list = list.Where(s => cityList.Contains(s.CityName)).ToList();

                    //}
                    if (cityTierList.Any())
                    {
                        if (cityTierList.Contains("空"))
                        {
                            cityTierList.Remove("空");
                            if (cityTierList.Any())
                                list = list.Where(s => cityTierList.Contains(s.CityTier) || (s.CityTier == null || s.CityTier == "")).ToList();
                            else
                                list = list.Where(s => s.CityTier == null || s.CityTier == "").ToList();
                        }
                        else
                            list = list.Where(s => cityTierList.Contains(s.CityTier)).ToList();
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

            cblFormat.Items.Clear();
            //cblShopLevel.Items.Clear();
            cblIsInstall.Items.Clear();
            if (Session["shopList"] != null)
            {

                List<Shop> list = Session["shopList"] as List<Shop>;
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
                    //List<string> cityList = new List<string>();
                    //foreach (ListItem li in cblCity.Items)
                    //{
                    //    if (li.Selected && !cityList.Contains(li.Value))
                    //        cityList.Add(li.Value);
                    //}
                    List<string> cityTierList = new List<string>();
                    foreach (ListItem li in cblCityTier.Items)
                    {
                        if (li.Selected && !cityTierList.Contains(li.Value))
                            cityTierList.Add(li.Value);
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

                    }
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
                    //if (cityList.Any())
                    //{
                    //    if (cityList.Contains("空"))
                    //    {
                    //        cityList.Remove("空");
                    //        if (cityList.Any())
                    //            list = list.Where(s => cityList.Contains(s.CityName) || (s.CityName == null || s.CityName == "")).ToList();
                    //        else
                    //            list = list.Where(s => s.CityName == null || s.CityName == "").ToList();
                    //    }
                    //    else
                    //        list = list.Where(s => cityList.Contains(s.CityName)).ToList();

                    //}
                    if (cityTierList.Any())
                    {
                        if (cityTierList.Contains("空"))
                        {
                            cityTierList.Remove("空");
                            if (cityTierList.Any())
                                list = list.Where(s => cityTierList.Contains(s.CityTier) || (s.CityTier == null || s.CityTier == "")).ToList();
                            else
                                list = list.Where(s => s.CityTier == null || s.CityTier == "").ToList();
                        }
                        else
                            list = list.Where(s => cityTierList.Contains(s.CityTier)).ToList();
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

        //void BindShopLevel()
        //{

        //    cblShopLevel.Items.Clear();
        //    cblIsInstall.Items.Clear();
        //    if (Session["shop"] != null)
        //    {

        //        List<Shop> list = Session["shop"] as List<Shop>;
        //        if (list.Any())
        //        {
        //            List<string> regionList = new List<string>();
        //            foreach (ListItem li in cblRegion.Items)
        //            {
        //                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
        //                    regionList.Add(li.Value.ToLower());
        //            }
        //            List<string> provinceList = new List<string>();
        //            foreach (ListItem li in cblProvince.Items)
        //            {
        //                if (li.Selected && !provinceList.Contains(li.Value))
        //                    provinceList.Add(li.Value);
        //            }
        //            //List<string> cityList = new List<string>();
        //            //foreach (ListItem li in cblCity.Items)
        //            //{
        //            //    if (li.Selected && !cityList.Contains(li.Value))
        //            //        cityList.Add(li.Value);
        //            //}
        //            List<string> cityTierList = new List<string>();
        //            foreach (ListItem li in cblCityTier.Items)
        //            {
        //                if (li.Selected && !cityTierList.Contains(li.Value))
        //                    cityTierList.Add(li.Value);
        //            }
        //            List<string> channelList = new List<string>();
        //            foreach (ListItem li in cblChannel.Items)
        //            {
        //                if (li.Selected && !channelList.Contains(li.Value))
        //                    channelList.Add(li.Value);
        //            }
        //            List<string> formatList = new List<string>();
        //            foreach (ListItem li in cblFormat.Items)
        //            {
        //                if (li.Selected && !formatList.Contains(li.Value))
        //                    formatList.Add(li.Value);
        //            }
        //            if (regionList.Any())
        //            {
        //                if (regionList.Contains("空"))
        //                {
        //                    regionList.Remove("空");
        //                    if (regionList.Any())
        //                        list = list.Where(s => regionList.Contains(s.RegionName.ToLower()) || (s.RegionName == null || s.RegionName == "")).ToList();
        //                    else
        //                        list = list.Where(s => s.RegionName == null || s.RegionName == "").ToList();
        //                }
        //                else
        //                    list = list.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();

        //            }
        //            if (provinceList.Any())
        //            {
        //                if (provinceList.Contains("空"))
        //                {
        //                    provinceList.Remove("空");
        //                    if (provinceList.Any())
        //                        list = list.Where(s => provinceList.Contains(s.ProvinceName) || (s.ProvinceName == null || s.ProvinceName == "")).ToList();
        //                    else
        //                        list = list.Where(s => s.ProvinceName == null || s.ProvinceName == "").ToList();
        //                }
        //                else
        //                    list = list.Where(s => provinceList.Contains(s.ProvinceName)).ToList();

        //            }
        //            //if (cityList.Any())
        //            //{
        //            //    if (cityList.Contains("空"))
        //            //    {
        //            //        cityList.Remove("空");
        //            //        if (cityList.Any())
        //            //            list = list.Where(s => cityList.Contains(s.CityName) || (s.CityName == null || s.CityName == "")).ToList();
        //            //        else
        //            //            list = list.Where(s => s.CityName == null || s.CityName == "").ToList();
        //            //    }
        //            //    else
        //            //        list = list.Where(s => cityList.Contains(s.CityName)).ToList();

        //            //}
        //            if (cityTierList.Any())
        //            {
        //                if (cityTierList.Contains("空"))
        //                {
        //                    cityTierList.Remove("空");
        //                    if (cityTierList.Any())
        //                        list = list.Where(s => cityTierList.Contains(s.CityTier) || (s.CityTier == null || s.CityTier == "")).ToList();
        //                    else
        //                        list = list.Where(s => s.CityTier == null || s.CityTier == "").ToList();
        //                }
        //                else
        //                    list = list.Where(s => cityTierList.Contains(s.CityTier)).ToList();
        //            }
        //            if (channelList.Any())
        //            {
        //                if (channelList.Contains("空"))
        //                {
        //                    channelList.Remove("空");
        //                    if (channelList.Any())
        //                        list = list.Where(s => channelList.Contains(s.Channel) || (s.Channel == null || s.Channel == "")).ToList();
        //                    else
        //                        list = list.Where(s => s.Channel == null || s.Channel == "").ToList();
        //                }
        //                else
        //                    list = list.Where(s => channelList.Contains(s.Channel)).ToList();

        //            }
        //            if (formatList.Any())
        //            {
        //                if (formatList.Contains("空"))
        //                {
        //                    formatList.Remove("空");
        //                    if (formatList.Any())
        //                        list = list.Where(s => formatList.Contains(s.Format) || (s.Format == null || s.Format == "")).ToList();
        //                    else
        //                        list = list.Where(s => s.Format == null || s.Format == "").ToList();
        //                }
        //                else
        //                    list = list.Where(s => formatList.Contains(s.Format)).ToList();
        //            }
        //            var shopLevelList = list.OrderBy(s => s.POSScale).Select(s => s.POSScale).Distinct().ToList();
        //            bool hasNull = false;
        //            shopLevelList.ForEach(s =>
        //            {


        //                if (string.IsNullOrWhiteSpace(s))
        //                {
        //                    hasNull = true;
        //                }
        //                else
        //                {
        //                    ListItem li = new ListItem();
        //                    li.Value = s;
        //                    li.Text = s + "&nbsp;";
        //                    cblShopLevel.Items.Add(li);
        //                }

        //            });
        //            if (hasNull)
        //                cblShopLevel.Items.Add(new ListItem("空", "空"));
        //        }
        //    }
        //}

        void BindIsInstall()
        {
            cblIsInstall.Items.Clear();
            if (Session["shopList"] != null)
            {

                List<Shop> list = Session["shopList"] as List<Shop>;
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

                    List<string> cityTierList = new List<string>();
                    foreach (ListItem li in cblCityTier.Items)
                    {
                        if (li.Selected && !cityTierList.Contains(li.Value))
                            cityTierList.Add(li.Value);
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
                    //List<string> shopLevelList = new List<string>();
                    //foreach (ListItem li in cblShopLevel.Items)
                    //{
                    //    if (li.Selected && !shopLevelList.Contains(li.Value))
                    //        shopLevelList.Add(li.Value);
                    //}
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

                    if (cityTierList.Any())
                    {
                        if (cityTierList.Contains("空"))
                        {
                            cityTierList.Remove("空");
                            if (cityTierList.Any())
                                list = list.Where(s => cityTierList.Contains(s.CityTier) || (s.CityTier == null || s.CityTier == "")).ToList();
                            else
                                list = list.Where(s => s.CityTier == null || s.CityTier == "").ToList();
                        }
                        else
                            list = list.Where(s => cityTierList.Contains(s.CityTier)).ToList();
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
                    //if (shopLevelList.Any())
                    //{
                    //    if (shopLevelList.Contains("空"))
                    //    {
                    //        shopLevelList.Remove("空");
                    //        if (shopLevelList.Any())
                    //            list = list.Where(s => shopLevelList.Contains(s.POSScale) || (s.POSScale == null || s.POSScale == "")).ToList();
                    //        else
                    //            list = list.Where(s => s.POSScale == null || s.POSScale == "").ToList();
                    //    }
                    //    else
                    //        list = list.Where(s => shopLevelList.Contains(s.POSScale)).ToList();
                    //}

                    var installList = list.OrderBy(s => s.IsInstall).Select(s => s.IsInstall).Distinct().ToList();
                    bool hasNull = false;
                    installList.ForEach(s =>
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
                            cblIsInstall.Items.Add(li);
                        }

                    });
                    if (hasNull)
                        cblIsInstall.Items.Add(new ListItem("空", "空"));
                }
            }
        }

        void BindShopType()
        {
            cblShopType.Items.Clear();
            if (Session["shopList"] != null)
            {

                List<Shop> list = Session["shopList"] as List<Shop>;
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

                    List<string> cityTierList = new List<string>();
                    foreach (ListItem li in cblCityTier.Items)
                    {
                        if (li.Selected && !cityTierList.Contains(li.Value))
                            cityTierList.Add(li.Value);
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
                    
                    List<string> installList = new List<string>();
                    foreach (ListItem li in cblIsInstall.Items)
                    {
                        if (li.Selected && !installList.Contains(li.Value))
                            installList.Add(li.Value);
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

                    }
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

                    if (cityTierList.Any())
                    {
                        if (cityTierList.Contains("空"))
                        {
                            cityTierList.Remove("空");
                            if (cityTierList.Any())
                                list = list.Where(s => cityTierList.Contains(s.CityTier) || (s.CityTier == null || s.CityTier == "")).ToList();
                            else
                                list = list.Where(s => s.CityTier == null || s.CityTier == "").ToList();
                        }
                        else
                            list = list.Where(s => cityTierList.Contains(s.CityTier)).ToList();
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
                   
                    if (installList.Any())
                    {
                        if (installList.Contains("空"))
                        {
                            installList.Remove("空");
                            if (installList.Any())
                                list = list.Where(s => installList.Contains(s.IsInstall) || (s.IsInstall == null || s.IsInstall == "")).ToList();
                            else
                                list = list.Where(s => s.IsInstall == null || s.IsInstall == "").ToList();
                        }
                        else
                            list = list.Where(s => installList.Contains(s.IsInstall)).ToList();
                    }
                    var shopTypeList = list.OrderBy(s => s.ShopType).Select(s => s.ShopType).Distinct().ToList();
                    bool hasNull = false;
                    shopTypeList.ForEach(s =>
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
                            cblShopType.Items.Add(li);
                        }

                    });
                    if (hasNull)
                        cblShopType.Items.Add(new ListItem("空", "空"));
                }
            }
        }

        /// <summary>
        /// 客服
        /// </summary>
        void BindCS()
        {
            cblCS.Items.Clear();
            if (Session["shopList"] != null)
            {

                List<Shop> list = Session["shopList"] as List<Shop>;
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

                    List<string> cityTierList = new List<string>();
                    foreach (ListItem li in cblCityTier.Items)
                    {
                        if (li.Selected && !cityTierList.Contains(li.Value))
                            cityTierList.Add(li.Value);
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

                    List<string> installList = new List<string>();
                    foreach (ListItem li in cblIsInstall.Items)
                    {
                        if (li.Selected && !installList.Contains(li.Value))
                            installList.Add(li.Value);
                    }
                    List<string> shopTypeList = new List<string>();
                    foreach (ListItem li in cblShopType.Items)
                    {
                        if (li.Selected && !shopTypeList.Contains(li.Value))
                            shopTypeList.Add(li.Value);
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

                    }
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

                    if (cityTierList.Any())
                    {
                        if (cityTierList.Contains("空"))
                        {
                            cityTierList.Remove("空");
                            if (cityTierList.Any())
                                list = list.Where(s => cityTierList.Contains(s.CityTier) || (s.CityTier == null || s.CityTier == "")).ToList();
                            else
                                list = list.Where(s => s.CityTier == null || s.CityTier == "").ToList();
                        }
                        else
                            list = list.Where(s => cityTierList.Contains(s.CityTier)).ToList();
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

                    if (installList.Any())
                    {
                        if (installList.Contains("空"))
                        {
                            installList.Remove("空");
                            if (installList.Any())
                                list = list.Where(s => installList.Contains(s.IsInstall) || (s.IsInstall == null || s.IsInstall == "")).ToList();
                            else
                                list = list.Where(s => s.IsInstall == null || s.IsInstall == "").ToList();
                        }
                        else
                            list = list.Where(s => installList.Contains(s.IsInstall)).ToList();
                    }
                    if (shopTypeList.Any())
                    {
                        if (shopTypeList.Contains("空"))
                        {
                            shopTypeList.Remove("空");
                            if (shopTypeList.Any())
                                list = list.Where(s => shopTypeList.Contains(s.ShopType) || (s.ShopType == null || s.ShopType == "")).ToList();
                            else
                                list = list.Where(s => s.ShopType == null || s.ShopType == "").ToList();
                        }
                        else
                            list = list.Where(s => shopTypeList.Contains(s.ShopType)).ToList();
                    }
                    var shopList = (from shop in list
                                    join user1 in CurrentContext.DbContext.UserInfo
                                    on shop.CSUserId equals user1.UserId into temp
                                    from user in temp.DefaultIfEmpty()
                                    select new
                                    {
                                        shop,
                                        user
                                    }).ToList();
                    var userList = shopList.Select(s => s.user).Distinct().ToList();
                    bool hasNull = false;
                    userList.ForEach(s =>
                    {


                        if (s!=null)
                        {
                            ListItem li = new ListItem();
                            li.Value = s.UserId.ToString();
                            li.Text = s.RealName + "&nbsp;";
                            cblCS.Items.Add(li);
                        }
                        else
                        {
                            hasNull = true;
                        }

                    });
                    if (hasNull)
                        cblCS.Items.Add(new ListItem("空", "0"));
                }
            }
        }

        void BindData()
        {
            List<int> curstomerList = new List<int>();
            foreach (ListItem item in ddlCustomer.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }
            var list = (from shop in CurrentContext.DbContext.Shop
                        join customer in CurrentContext.DbContext.Customer
                        on shop.CustomerId equals customer.Id
                        join user1 in CurrentContext.DbContext.UserInfo
                        on shop.CSUserId equals user1.UserId into userTemp
                        from user in userTemp.DefaultIfEmpty()
                        where curstomerList.Contains(shop.CustomerId ?? 0)
                        select new
                        {
                            shop.Id,
                            customer.CustomerName,
                            shop.CustomerId,
                            shop.ShopNo,
                            shop.ShopName,
                            shop.RegionName,
                            shop.ProvinceName,
                            shop.CityName,
                            shop.AreaName,
                            shop.CityTier,
                            shop.Channel,
                            shop.Format,
                            shop.IsInstall,
                            shop.AgentCode,
                            shop.AgentName,
                            shop.Status,
                            shop.IsDelete,
                            CSUserName = user.RealName,
                            shop.POSScale,
                            shop.ShopType,
                            shop.OutsourceId,
                            shop.CSUserId
                        }).ToList();

            List<string> myRegions = GetResponsibleRegion.Select(s=>s.ToLower()).ToList();
            if (myRegions.Any())
            {
                list = list.Where(s => myRegions.Contains(s.RegionName.ToLower()) || s.RegionName == null || s.RegionName == "").ToList();
            }

            if (ddlCustomer.SelectedValue != "0")
            {
                int customerId = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == customerId).ToList();
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

            List<string> cityTierList = new List<string>();
            foreach (ListItem li in cblCityTier.Items)
            {
                if (li.Selected && !cityTierList.Contains(li.Value))
                    cityTierList.Add(li.Value);
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
            
            List<string> installList = new List<string>();
            foreach (ListItem li in cblIsInstall.Items)
            {
                if (li.Selected && !installList.Contains(li.Value))
                    installList.Add(li.Value);
            }
            List<string> shopTypeList = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected && !shopTypeList.Contains(li.Value))
                    shopTypeList.Add(li.Value);
            }
            List<int> csList = new List<int>();
            foreach (ListItem li in cblCS.Items)
            {
                if (li.Selected && !csList.Contains(int.Parse(li.Value)))
                    csList.Add(int.Parse(li.Value));
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

            }
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

            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                        list = list.Where(s => cityTierList.Contains(s.CityTier) || (s.CityTier == null || s.CityTier == "")).ToList();
                    else
                        list = list.Where(s => s.CityTier == null || s.CityTier == "").ToList();
                }
                else
                    list = list.Where(s => cityTierList.Contains(s.CityTier)).ToList();
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

            if (installList.Any())
            {
                if (installList.Contains("空"))
                {
                    installList.Remove("空");
                    if (installList.Any())
                        list = list.Where(s => installList.Contains(s.IsInstall) || (s.IsInstall == null || s.IsInstall == "")).ToList();
                    else
                        list = list.Where(s => s.IsInstall == null || s.IsInstall == "").ToList();
                }
                else
                    list = list.Where(s => installList.Contains(s.IsInstall)).ToList();
            }

            if (shopTypeList.Any())
            {
                if (shopTypeList.Contains("空"))
                {
                    shopTypeList.Remove("空");
                    if (shopTypeList.Any())
                        list = list.Where(s => shopTypeList.Contains(s.ShopType) || (s.ShopType == null || s.ShopType == "")).ToList();
                    else
                        list = list.Where(s => s.ShopType == null || s.ShopType == "").ToList();
                }
                else
                    list = list.Where(s => shopTypeList.Contains(s.ShopType)).ToList();
            }
            if (csList.Any())
            {
                if (csList.Contains(0))
                {
                    csList.Remove(0);
                    if (csList.Any())
                        list = list.Where(s => csList.Contains(s.CSUserId ?? 0) || (s.CSUserId == null || s.CSUserId == 0)).ToList();
                    else
                        list = list.Where(s => s.CSUserId == null || s.CSUserId == 0).ToList();
                }
                else
                    list = list.Where(s => csList.Contains(s.CSUserId??0)).ToList();
            }


            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                list = list.Where(s => s.ShopNo.ToLower().Contains(txtShopNo.Text.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                list = list.Where(s => s.ShopName.ToLower().Contains(txtShopName.Text.Trim().ToLower())).ToList();
            }
            int outsourceState = 0;
            foreach (ListItem li in cblOutsourceState.Items)
            {
                if (li.Selected)
                    outsourceState = int.Parse(li.Value);
            }
            if (outsourceState > 0)
            {
                switch (outsourceState)
                {
                    case 1:
                        list = list.Where(s => (s.OutsourceId??0)>0).ToList();
                        break;
                    case 2:
                        list = list.Where(s => (s.OutsourceId ?? 0) ==0).ToList();
                        break;
                    default:
                        break;
                }
            }
            int outsoutceId = int.Parse(ddlOutsource.SelectedValue);
            if (outsoutceId > 0)
            {
                list = list.Where(s => s.OutsourceId == outsoutceId).ToList();
            }
            
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
           
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd, btnExportShop, btnExportShopAndPOP, btnExportShopAndFrame, btnAddCheckChangeLog });
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
            BindProvince();
            //BindCity();
            BindCityTier();
            BindChannel();
            BindFormat();
            //BindShopLevel();
            BindIsInstall();
            BindShopType();
            BindCS();
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
            //BindCity();
            BindCityTier();
            BindChannel();
            BindFormat();
            //BindShopLevel();
            BindIsInstall();
            BindShopType();
            BindCS();
        }


        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindCity();
            BindCityTier();
            BindChannel();
            BindFormat();
            //BindShopLevel();
            BindIsInstall();
            BindShopType();
            BindCS();
        }

        protected void cblCityTier_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindChannel();
            BindFormat();
            //BindShopLevel();
            BindIsInstall();
            BindShopType();
            BindCS();
        }

        protected void cblChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFormat();
            //BindShopLevel();
            BindIsInstall();
            BindShopType();
            
        }

        protected void cblFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindShopLevel();
            BindIsInstall();
            BindShopType();
           
        }

        //protected void cblShopLevel_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //    BindIsInstall();
        //    BindShopType();
        //}

        protected void cblIsInstall_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindShopType();
        }
        /// <summary>
        /// 导出店铺信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportShop_Click(object sender, EventArgs e)
        {
            string where = GetCondition();

            DataSet ds = new ShopBLL().GetShopList(where);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (CurrentUser.RoleId == 2 && dt.Columns.Contains("Status"))
                {
                    dt.Columns.Remove("Status");
                }
                OperateFile.ExportExcel(dt, "店铺信息", null, "shop");

            }
        }

        /// <summary>
        /// 导出店铺信息+POP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportShopAndPOP_Click(object sender, EventArgs e)
        {
            string where = GetCondition();
            DataSet ds = new ShopBLL().GetShopAndPOPList(where);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (CurrentUser.RoleId == 2 && dt.Columns.Contains("Status"))
                {
                    dt.Columns.Remove("Status");
                }
                OperateFile.ExportExcel(dt, "店铺+POP", null, "shopPop");

            }
        }

        /// <summary>
        /// 导出店铺信息+器架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportShopAndFrame_Click(object sender, EventArgs e)
        {
            string where = GetCondition();
            DataSet ds = new ShopMachineFrameBLL().GetDataList(where);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (CurrentUser.RoleId == 2 && dt.Columns.Contains("Status"))
                {
                    dt.Columns.Remove("Status");
                }
                OperateFile.ExportExcel(dt, "店铺+器架", null, "shopFrame");

            }
        }

        string GetCondition()
        {
            bool isCanExportUser = false;
            List<string> exportChannelList = new List<string>();
            //if (ViewState["ExportChannel"] != null)
            //{
            //    string exportChannel = ViewState["ExportChannel"].ToString();
            //    exportChannelList = StringHelper.ToStringList(exportChannel, ',');
            //    isCanExportUser = true;
            //}
            if (Session["UserExportChannel"] != null)
            {
                List<ExportPermissionContent> contentList = Session["UserExportChannel"] as List<ExportPermissionContent>;
                if (contentList.Any())
                {
                    contentList.ForEach(s => {
                        exportChannelList.Add(s.Channel);
                    });
                }
            }


            System.Text.StringBuilder where = new System.Text.StringBuilder();
            if (ddlCustomer.SelectedValue != "0")
            {
                int customerId = int.Parse(ddlCustomer.SelectedValue);
                where.AppendFormat(" and Shop.CustomerId={0}", customerId);
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

            List<string> cityTierList = new List<string>();
            foreach (ListItem li in cblCityTier.Items)
            {
                if (li.Selected && !cityTierList.Contains(li.Value))
                    cityTierList.Add(li.Value);
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
           
            List<string> installList = new List<string>();
            foreach (ListItem li in cblIsInstall.Items)
            {
                if (li.Selected && !installList.Contains(li.Value))
                    installList.Add(li.Value);
            }
            List<string> shopTypeList = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected && !shopTypeList.Contains(li.Value))
                    shopTypeList.Add(li.Value);
            }
            List<int> csList = new List<int>();
            foreach (ListItem li in cblCS.Items)
            {
                if (li.Selected && !csList.Contains(int.Parse(li.Value)))
                    csList.Add(int.Parse(li.Value));
            }
            if (regionList.Any())
            {
                StringBuilder regions = new StringBuilder();
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
            if (cityTierList.Any())
            {
                StringBuilder cityTiers = new StringBuilder();
                cityTierList.ForEach(s =>
                {
                    if (s != "空")
                        cityTiers.Append("'" + s + "',");
                });
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        where.AppendFormat(" and (Shop.CityTier in ({0}) or (Shop.CityTier is null or Shop.CityTier=''))", cityTiers.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (Shop.CityTier is null or Shop.CityTier='')");
                }
                else
                {
                    where.AppendFormat(" and Shop.CityTier in ({0})", cityTiers.ToString().TrimEnd(','));
                }
            }

            if (channelList.Any())
            {
                StringBuilder channels = new StringBuilder();
                if (exportChannelList.Any())
                {
                    for (int i = 0; i < channelList.Count; i++)
                    {

                        string channel = channelList[i].ToLower();
                        bool flag = false;
                        exportChannelList.ForEach(s =>
                        {
                            if (channel.Contains(s.ToLower()))
                                flag = true;
                        });
                        if (!flag)
                            channelList.RemoveAt(i);
                    }
                }
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
            else
            {
                if (exportChannelList.Any())
                {
                    where.Append(" and (");
                    for (int i = 0; i < exportChannelList.Count; i++)
                    {
                        if (i == 0)
                        {
                            where.AppendFormat(" Shop.Channel like '%{0}%'", exportChannelList[i].ToLower());
                        }
                        else
                        {
                            where.AppendFormat(" or Shop.Channel like '%{0}%'", exportChannelList[i].ToLower());
                        }
                    }
                    where.Append(" )");
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
           
           
            if (installList.Any())
            {
                StringBuilder installs = new StringBuilder();
                installList.ForEach(s =>
                {
                    if (s != "空")
                        installs.Append("'" + s + "',");
                });
                if (installList.Contains("空"))
                {
                    installList.Remove("空");
                    if (installList.Any())
                    {
                        where.AppendFormat(" and (Shop.IsInstall in ({0}) or (Shop.IsInstall is null or Shop.IsInstall=''))", installs.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (Shop.IsInstall is null or Shop.IsInstall='')");
                }
                else
                {
                    where.AppendFormat(" and Shop.IsInstall in ({0})", installs.ToString().TrimEnd(','));
                }
            }


            if (shopTypeList.Any())
            {
                StringBuilder shopTypes = new StringBuilder();
                shopTypeList.ForEach(s =>
                {
                    if (s != "空")
                        shopTypes.Append("'" + s + "',");
                });
                if (shopTypeList.Contains("空"))
                {
                    shopTypeList.Remove("空");
                    if (shopTypeList.Any())
                    {
                        where.AppendFormat(" and (Shop.ShopType in ({0}) or (Shop.ShopType is null or Shop.ShopType=''))", shopTypes.ToString().TrimEnd(','));
                    }
                    else
                        where.AppendFormat(" and (Shop.ShopType is null or Shop.ShopType='')");
                }
                else
                {
                    where.AppendFormat(" and Shop.ShopType in ({0})", shopTypes.ToString().TrimEnd(','));
                }
            }
            if (csList.Any())
            {
                
                if (csList.Contains(0))
                {
                    csList.Remove(0);
                    if (csList.Any())
                    {
                        where.AppendFormat(" and (Shop.CSUserId in ({0}) or (Shop.CSUserId is null or Shop.CSUserId=0))", StringHelper.ListToString(csList));
                    }
                    else
                        where.AppendFormat(" and (Shop.CSUserId is null or Shop.CSUserId=0)");
                }
                else
                {
                    where.AppendFormat(" and Shop.CSUserId in ({0})", StringHelper.ListToString(csList));
                }
            }

            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                where.AppendFormat(" and Shop.ShopNo like '%{0}%'", txtShopNo.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                where.AppendFormat(" and Shop.ShopName like '%{0}%'", txtShopName.Text.Trim());
            }
            if (isCanExportUser)
            {
                where.Append(" and (Shop.Format not like '%hc%' and Shop.Format not like '%Homecore%' and Shop.Format not like '%Homecourt%')");
                where.Append(" and ((Shop.status not like '%闭%' and Shop.status !='装修') or Shop.status is null or Shop.status='')");
                where.Append(" and (Shop.ProvinceName!='湖北')");
            }
            return where.ToString();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            BaseDataChangeLogBLL changeLogBll = new BaseDataChangeLogBLL();
            ShopChangeDetailBLL detailBll = new ShopChangeDetailBLL();
            ShopChangeDetail shopChangeDetailModel;

            int shopId = int.Parse(e.CommandArgument.ToString());
            ShopBLL shopBll = new ShopBLL();
            if (e.CommandName == "DeleteItem")
            {
                Shop model = shopBll.GetModel(shopId);
                if (model != null)
                {
                    bool canUpdate = false;
                    string changeType = "标记删除";
                    if (model.IsDelete == true)
                    {
                        changeType = "恢复";
                        var shopList1 = shopBll.GetList(s => s.ShopNo.ToUpper() == model.ShopNo.ToUpper() && s.Id!=model.Id && (s.IsDelete==null || s.IsDelete==false));
                        if (shopList1.Any())
                        {
                            canUpdate = false;
                        }
                        else
                        {
                            model.IsDelete = false;
                            canUpdate = true;
                        }
                    }
                    else
                    {
                        model.IsDelete = true;
                        canUpdate = true;
                    }
                    if (canUpdate)
                    {
                        shopBll.Update(model);
                        BaseDataChangeLog logModel = new BaseDataChangeLog();
                        logModel.AddDate = DateTime.Now;
                        logModel.AddUserId = new BasePage().CurrentUser.UserId;
                        logModel.ItemType = (int)BaseDataChangeItemEnum.Shop;
                        logModel.ChangeType = (int)DataChangeTypeEnum.Delete;
                        logModel.ShopId = shopId;
                        changeLogBll.Add(logModel);


                        shopChangeDetailModel = new ShopChangeDetail();
                        shopChangeDetailModel.ShopId = shopId;
                        shopChangeDetailModel.AgentCode = model.AgentCode;
                        shopChangeDetailModel.AgentName = model.AgentName;
                        shopChangeDetailModel.AreaName = model.AreaName;
                        shopChangeDetailModel.BasicInstallPrice = model.BasicInstallPrice;
                        shopChangeDetailModel.BusinessModel = model.BusinessModel;
                        shopChangeDetailModel.Category = model.Category;
                        shopChangeDetailModel.Channel = model.Channel;
                        shopChangeDetailModel.CityName = model.CityName;
                        shopChangeDetailModel.CityTier = model.CityTier;
                        shopChangeDetailModel.Contact1 = model.Contact1;
                        shopChangeDetailModel.Contact2 = model.Contact2;
                        shopChangeDetailModel.CustomerId = model.CustomerId;
                        shopChangeDetailModel.Format = model.Format;
                        shopChangeDetailModel.IsInstall = model.IsInstall;
                        shopChangeDetailModel.LocationType = model.LocationType;
                        shopChangeDetailModel.LogId = logModel.Id;
                        shopChangeDetailModel.POPAddress = model.POPAddress;
                        shopChangeDetailModel.ProvinceName = model.ProvinceName;
                        shopChangeDetailModel.RegionName = model.RegionName;
                        shopChangeDetailModel.Remark = model.Remark;
                        shopChangeDetailModel.ShopName = model.ShopName;
                        shopChangeDetailModel.ShopNo = model.ShopNo;
                        shopChangeDetailModel.ShopType = model.ShopType;
                        shopChangeDetailModel.Status = model.Status;
                        shopChangeDetailModel.Tel1 = model.Tel1;
                        shopChangeDetailModel.Tel2 = model.Tel2;
                        shopChangeDetailModel.CSUserId = model.CSUserId;
                        shopChangeDetailModel.ChangeType = changeType;
                        shopChangeDetailModel.AddDate = DateTime.Now;
                        shopChangeDetailModel.AddUserId = CurrentUser.UserId;
                        detailBll.Add(shopChangeDetailModel);
                        BindData();
                    }
                    else
                    {
                        Alert("操作失败：存在相同编号,需要把另外一个删除后才能恢复当前店铺");
                    }
                }
            }
            if (e.CommandName == "RealDelete")
            {
                Shop model = shopBll.GetModel(shopId);
                if (model != null)
                {
                    try
                    {
                        new POPBLL().Delete(s => s.ShopId == shopId);
                        new ShopMachineFrameBLL().Delete(s => s.ShopId == shopId);
                        
                        shopBll.Delete(model);

                        

                        BaseDataChangeLog logModel = new BaseDataChangeLog();
                        logModel.AddDate = DateTime.Now;
                        logModel.AddUserId = new BasePage().CurrentUser.UserId;
                        logModel.ItemType = (int)BaseDataChangeItemEnum.Shop;
                        logModel.ChangeType = (int)DataChangeTypeEnum.Delete;
                        logModel.ShopId = shopId;
                        changeLogBll.Add(logModel);


                        shopChangeDetailModel = new ShopChangeDetail();
                        shopChangeDetailModel.ShopId = shopId;
                        shopChangeDetailModel.AgentCode = model.AgentCode;
                        shopChangeDetailModel.AgentName = model.AgentName;
                        shopChangeDetailModel.AreaName = model.AreaName;
                        shopChangeDetailModel.BasicInstallPrice = model.BasicInstallPrice;
                        shopChangeDetailModel.BusinessModel = model.BusinessModel;
                        shopChangeDetailModel.Category = model.Category;
                        shopChangeDetailModel.Channel = model.Channel;
                        shopChangeDetailModel.CityName = model.CityName;
                        shopChangeDetailModel.CityTier = model.CityTier;
                        shopChangeDetailModel.Contact1 = model.Contact1;
                        shopChangeDetailModel.Contact2 = model.Contact2;
                        shopChangeDetailModel.CustomerId = model.CustomerId;
                        shopChangeDetailModel.Format = model.Format;
                        shopChangeDetailModel.IsInstall = model.IsInstall;
                        shopChangeDetailModel.LocationType = model.LocationType;
                        shopChangeDetailModel.LogId = logModel.Id;
                        shopChangeDetailModel.POPAddress = model.POPAddress;
                        shopChangeDetailModel.ProvinceName = model.ProvinceName;
                        shopChangeDetailModel.RegionName = model.RegionName;
                        shopChangeDetailModel.Remark = model.Remark;
                        shopChangeDetailModel.ShopName = model.ShopName;
                        shopChangeDetailModel.ShopNo = model.ShopNo;
                        shopChangeDetailModel.ShopType = model.ShopType;
                        shopChangeDetailModel.Status = model.Status;
                        shopChangeDetailModel.Tel1 = model.Tel1;
                        shopChangeDetailModel.Tel2 = model.Tel2;
                        shopChangeDetailModel.CSUserId = model.CSUserId;
                        shopChangeDetailModel.ChangeType = "彻底删除";
                        shopChangeDetailModel.AddDate = DateTime.Now;
                        shopChangeDetailModel.AddUserId = CurrentUser.UserId;
                        detailBll.Add(shopChangeDetailModel);


                        BindData();
                    }
                    catch
                    {
                        Alert("删除失败！");
                    }

                }
            }

        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object isDeleteObj = item.GetType().GetProperty("IsDelete").GetValue(item, null);
                    bool isDelete = isDeleteObj != null ? (bool.Parse(isDeleteObj.ToString())) : false;
                    Label labState = (Label)e.Row.FindControl("labState");
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");
                    if (isDelete)
                    {
                        labState.Text = "已删除";
                        labState.ForeColor = System.Drawing.Color.Red;
                        lbDelete.Text = "恢复";
                        lbDelete.Attributes.Add("OnClick", "return confirm('确定恢复吗？')");
                    }
                    else
                    {
                        labState.Text = "正常";
                        lbDelete.Text = "删除";
                        lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前用户的导出范围
        /// </summary>
        void GetUserExportChannel()
        {
            Session["UserExportChannel"] = null;
            string filePath = UserExportShopChannelPath;
            if (File.Exists(Server.MapPath(filePath)))
            {
                List<ExportPermissionContent> contentList = new List<ExportPermissionContent>();
                JsonData jsonData = JsonMapper.ToObject(File.ReadAllText(Server.MapPath(filePath)));
                foreach (JsonData item in jsonData)
                {
                    string PermissionType = item["PermissionType"].ToString();
                    JsonData PermissionContents = item["PermissionContent"];

                    if (PermissionType == ConfigPermissionTypeEnum.Export.ToString())
                    {

                        foreach (JsonData subItem in PermissionContents)
                        {
                            int userId = int.Parse(subItem["UserId"].ToString());
                            if (userId == CurrentUser.UserId)
                            {
                                ExportPermissionContent permissionContent = new ExportPermissionContent();
                                permissionContent.Channel = subItem["Channel"].ToString();
                                permissionContent.Format = subItem["Format"].ToString();
                                permissionContent.UserId = userId;
                                contentList.Add(permissionContent);
                            }
                        }
                    }
                }
                if (contentList.Any())
                {
                    btnExportShop.Visible = true;
                    btnExportShopAndPOP.Visible = true;
                    btnExportShopAndFrame.Visible = true;
                    Session["UserExportChannel"] = contentList;
                }
            }
        }
        




    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.Subjects.InstallPrice
{
    public partial class EditPOSScale : BasePage
    {
        int guidanceId;
        string region = string.Empty;
        string subjectTypeId = string.Empty;
        string subjectId = string.Empty;
        string posScale = string.Empty;
        string shopId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId =int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["region"] != null)
            {
                region = Request.QueryString["region"];
            }
            if (Request.QueryString["subjectTypeId"] != null)
            {
                subjectTypeId = Request.QueryString["subjectTypeId"];
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = Request.QueryString["subjectId"];
            }
            if (Request.QueryString["posScale"] != null)
            {
                posScale = Request.QueryString["posScale"];
            }
            if (Request.QueryString["shopId"] != null)
            {
                shopId = Request.QueryString["shopId"];
            }
            if (!IsPostBack)
            {
                BindPOSScaleDDL();
                BindData();
            }
        }
        List<string> POSScaleData = new List<string>();

        void BindPOSScaleDDL()
        {
            if (!POSScaleData.Any())
            {
                POSScaleData = new POSScaleInfoBLL().GetList(s => 1 == 1).Select(s => s.POSScaleName).ToList();
            }
            if (POSScaleData.Any())
            {
                POSScaleData.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s;
                    li.Value = s;
                    //ddlPOSScale0.Items.Add(li);
                });
            }
        }


        void BindData11()
        {
            
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                              on subject.GuidanceId equals guidance.ItemId
                             where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             && (subject.SubjectType != (int)SubjectTypeEnum.二次安装)
                             && order.IsInstall != null && order.IsInstall == "Y"
                             && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                             //&& (shop.Status == null || shop.Status == "" || shop.Status == "正常")
                             select new
                             {
                                 subject,
                                 shop,
                                 POSScale =order.InstallPricePOSScale,
                                 MaterialSupport = order.MaterialSupport,
                                 guidance.ItemName
                             }).ToList();
            
            //var priceDetailList = new InstallPriceDetailBLL().GetList(s => s.GuidanceId == guidanceId);
            List<int> assginShopList = new List<int>();
            assginShopList=new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();
            //if (priceDetailList.Any())
            //{
            //    priceDetailList.ForEach(s =>
            //    {
            //        List<int> shopIds = StringHelper.ToIntList(s.ShopIds, ',');
            //        shopIds.ForEach(sh =>
            //        {
            //            if (!assginShopList.Contains(sh))
            //                assginShopList.Add(sh);
            //        });
            //    });
            //}
            if (assginShopList.Any())
            {
                orderList = orderList.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
            }
            if (orderList.Any())
            {
                labGuidanceName.Text = orderList[0].ItemName;
                List<string> regions = new List<string>();
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regions = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                List<int> subjectTypeList = new List<int>();
                if (!string.IsNullOrWhiteSpace(subjectTypeId))
                {
                    subjectTypeList = StringHelper.ToIntList(subjectTypeId, ',');
                }
                List<int> subjectIdList = new List<int>();
                if (!string.IsNullOrWhiteSpace(subjectId))
                {
                    subjectIdList = StringHelper.ToIntList(subjectId, ',');
                }
                List<string> posScaleList = new List<string>();
                if (!string.IsNullOrWhiteSpace(posScale))
                {
                    posScaleList = StringHelper.ToStringList(posScale, ',');
                }
                if (regions.Any())
                {
                    orderList = orderList.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
                }
                if (subjectTypeList.Any())
                {
                    orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                }
                if (subjectIdList.Any())
                {
                    orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId ?? 0)).ToList();
                }
                if (posScaleList.Any())
                {
                    if (posScaleList.Contains("空"))
                    {
                        posScaleList.Remove("空");
                        if (posScaleList.Any())
                        {
                            orderList = orderList.Where(s => posScaleList.Contains(s.POSScale) || s.POSScale == "" || s.POSScale == null).ToList();
                        }
                        else
                            orderList = orderList.Where(s => s.POSScale == "" || s.POSScale == null).ToList();
                    }
                    else
                        orderList = orderList.Where(s => posScaleList.Contains(s.POSScale)).ToList();
                }
                string shopNo = txtShopNo.Text.Trim();
                string shopName = txtShopName.Text.Trim();
                string provinceName = ddlProvince.SelectedValue;
                string cityName = ddlCity.SelectedValue;
                List<string> cityTierList = new List<string>();
                for (int i = 0; i < cblCityTier.Items.Count; i++)
                {
                    if (cblCityTier.Items[i].Selected)
                    {
                        cityTierList.Add(cblCityTier.Items[i].Value);
                    }
                }
                if (!string.IsNullOrWhiteSpace(shopNo))
                {
                    orderList = orderList.Where(s => s.shop.ShopNo.ToUpper().Contains(shopNo.ToUpper())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(shopName))
                {
                    orderList = orderList.Where(s => s.shop.ShopName.ToUpper().Contains(shopName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(provinceName))
                {
                    orderList = orderList.Where(s => s.shop.ProvinceName == provinceName).ToList();
                }
                if (!string.IsNullOrWhiteSpace(cityName))
                {
                    orderList = orderList.Where(s => s.shop.CityName == cityName).ToList();
                }
                if (cityTierList.Any())
                {
                    orderList = orderList.Where(s => cityTierList.Contains(s.shop.CityTier)).ToList();
                }
            }

            var shopList = (from order in orderList
                            group order by new
                            {
                                order.shop,
                                order.POSScale,
                                order.MaterialSupport
                            } into g
                            select new
                            {
                                g.Key.shop,
                                g.Key.POSScale,
                                g.Key.MaterialSupport
                            }).ToList();
            if (shopList.Any())
            {
                if (ddlProvince.Items.Count == 1)
                {
                    var provinceList = shopList.Select(s => s.shop.ProvinceName).Distinct().OrderBy(s => s).ToList();
                    provinceList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s;
                        li.Value = s;
                        ddlProvince.Items.Add(li);
                    });
                    var cityTierList = shopList.Select(s => s.shop.CityTier).Distinct().OrderBy(s => s).ToList();
                    cityTierList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s + "&nbsp;&nbsp;";
                        li.Value = s;
                        cblCityTier.Items.Add(li);
                    });
                    Dictionary<string, string> cityDic = new Dictionary<string, string>();
                    shopList.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s.shop.CityName) && !cityDic.Keys.Contains(s.shop.CityName))
                        {
                            cityDic.Add(s.shop.CityName, s.shop.ProvinceName);
                        }
                    });
                    if (cityDic.Keys.Count > 0)
                    {
                        ViewState["cityDic"] = cityDic;
                    }
                }

                
            }
            labShopCount.Text = shopList.Select(s=>s.shop.Id).Distinct().Count().ToString();
            AspNetPager1.RecordCount = shopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvShop.DataSource = shopList.OrderBy(s=>s.shop.Id).ThenBy(s => s.POSScale).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvShop.DataBind();
        }

        void BindData()
        {
            ////活动订单
            //List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
            ////常规订单
            //List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();

            //List<Subject> subjectList = new List<Subject>();

            //if (Session["activityOrderListIP"] != null)
            //{
            //    activityOrderList = Session["activityOrderListIP"] as List<FinalOrderDetailTemp>;

            //}
            //if (Session["genericOrderListIP"] != null)
            //{
            //    genericOrderList = Session["genericOrderListIP"] as List<FinalOrderDetailTemp>;
            //}
            //List<FinalOrderDetailTemp> totalOrderList = activityOrderList.Concat(genericOrderList).ToList();
            //if (totalOrderList.Any())
            //{
            //    var orderList = (from order in totalOrderList
            //                     join subject in CurrentContext.DbContext.Subject
            //                     on order.SubjectId equals subject.Id
            //                     join shop in CurrentContext.DbContext.Shop
            //                     on order.ShopId equals shop.Id
            //                     join guidance in CurrentContext.DbContext.SubjectGuidance
            //                      on subject.GuidanceId equals guidance.ItemId
            //                     where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
            //                     && subject.ApproveState == 1
            //                     && (order.IsDelete == null || order.IsDelete == false)
            //                     select new
            //                     {
            //                         subject,
            //                         shop,
            //                         POSScale = order.InstallPricePOSScale,
            //                         MaterialSupport = order.MaterialSupport,
            //                         guidance.ItemName
            //                     }).ToList();
            //    if (!string.IsNullOrWhiteSpace(shopId))
            //    { 
                   
            //    }
            //}
            List<Shop> shopList = new List<Shop>();
            if (!string.IsNullOrWhiteSpace(shopId))
            {
                List<int> shopIdList = StringHelper.ToIntList(shopId, ',');
                shopList = new ShopBLL().GetList(s => shopIdList.Contains(s.Id));
            }
            labShopCount.Text = shopList.Count.ToString();
            AspNetPager1.RecordCount = shopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvShop.DataSource = shopList.OrderBy(s => s.ShopNo).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvShop.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        
        protected void gvShop_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                
                //object item = e.Item.DataItem;
                //if (item != null)
                //{
                //    object posScaleObj = item.GetType().GetProperty("POSScale").GetValue(item, null);
                //    string posScale = posScaleObj != null ? posScaleObj.ToString() : "";
                //    if (string.IsNullOrWhiteSpace(posScale))
                //    {
                //        if (!POSScaleData.Any())
                //        {
                //            POSScaleData = new POSScaleInfoBLL().GetList(s => 1 == 1).Select(s => s.POSScaleName).ToList();
                //        }
                //        DropDownList ddlPOSScale = (DropDownList)e.Item.FindControl("ddlPOSScale");
                //        ddlPOSScale.Visible = true;

                //        Label labPOSScale = (Label)e.Item.FindControl("labPOSScale");
                //        labPOSScale.Visible = false;
                //        if (POSScaleData.Any())
                //        {
                //            POSScaleData.ForEach(s =>
                //            {
                //                ListItem li = new ListItem();
                //                li.Text = s;
                //                li.Value = s;
                //                ddlPOSScale.Items.Add(li);
                //            });
                //        }
                //    }
                    
                //}
            }
        }

        protected void ddlProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCity.Items.Clear();
            string provinceName = (sender as DropDownList).SelectedValue;
            if (!string.IsNullOrWhiteSpace(provinceName))
            {
                if (ViewState["cityDic"] != null)
                {
                    Dictionary<string, string> cityDic = (Dictionary<string, string>)ViewState["cityDic"];
                    if (cityDic != null && cityDic.Keys.Count > 0)
                    {
                        List<string> list = new List<string>();
                        foreach (KeyValuePair<string, string> item in cityDic)
                        {
                            if (item.Value == provinceName)
                                list.Add(item.Key);
                        }
                        list.ForEach(s => {
                            ListItem li = new ListItem();
                            li.Text = s;
                            li.Value = s;
                            ddlCity.Items.Add(li);
                        });
                    }
                }
            }
            ddlCity.Items.Insert(0,new ListItem("--请选择--",""));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        /// <summary>
        /// 按条件更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdateAll_Click(object sender, EventArgs e)
        {
            //string posScale = ddlPOSScale0.SelectedValue;
            string posScale = txtPOSScale.Text.Trim();
            if (!string.IsNullOrWhiteSpace(posScale))
            {
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                  on subject.GuidanceId equals guidance.ItemId
                                 where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                                 && subject.ApproveState == 1
                                 && (order.IsDelete == null || order.IsDelete == false)
                                 && (subject.SubjectType != (int)SubjectTypeEnum.二次安装)
                                 && order.IsInstall != null && order.IsInstall == "Y"
                                 //&& (shop.Status == null || shop.Status == "" || shop.Status == "正常")
                                 && (order.InstallPricePOSScale==null || order.InstallPricePOSScale=="")
                                 select new
                                 {
                                     subject,
                                     shop,
                                     order
                                 }).ToList();

                var priceDetailList = new InstallPriceDetailBLL().GetList(s => s.GuidanceId == guidanceId);
                List<int> assginShopList = new List<int>();
                if (priceDetailList.Any())
                {
                    priceDetailList.ForEach(s =>
                    {
                        List<int> shopIds = StringHelper.ToIntList(s.ShopIds, ',');
                        shopIds.ForEach(sh =>
                        {
                            if (!assginShopList.Contains(sh))
                                assginShopList.Add(sh);
                        });
                    });
                }
                if (assginShopList.Any())
                {
                    orderList = orderList.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
                }
                if (orderList.Any())
                {
                    
                    List<string> regions = new List<string>();
                    if (!string.IsNullOrWhiteSpace(region))
                    {
                        regions = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                    }
                    List<int> subjectTypeList = new List<int>();
                    if (!string.IsNullOrWhiteSpace(subjectTypeId))
                    {
                        subjectTypeList = StringHelper.ToIntList(subjectTypeId, ',');
                    }
                    List<int> subjectIdList = new List<int>();
                    if (!string.IsNullOrWhiteSpace(subjectId))
                    {
                        subjectIdList = StringHelper.ToIntList(subjectId, ',');
                    }
                    List<string> posScaleList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(posScale))
                    {
                        posScaleList = StringHelper.ToStringList(posScale, ',');
                    }
                    if (regions.Any())
                    {
                        orderList = orderList.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
                    }
                    if (subjectTypeList.Any())
                    {
                        orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                    }
                    if (subjectIdList.Any())
                    {
                        orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId ?? 0)).ToList();
                    }
                    //if (posScaleList.Any())
                    //{
                    //    if (posScaleList.Contains("空"))
                    //    {
                    //        posScaleList.Remove("空");
                    //        if (posScaleList.Any())
                    //        {
                    //            orderList = orderList.Where(s => posScaleList.Contains(s.POSScale) || s.POSScale == "" || s.POSScale == null).ToList();
                    //        }
                    //        else
                    //            orderList = orderList.Where(s => s.POSScale == "" || s.POSScale == null).ToList();
                    //    }
                    //    else
                    //        orderList = orderList.Where(s => posScaleList.Contains(s.POSScale)).ToList();
                    //}
                    string shopNo = txtShopNo.Text.Trim();
                    string shopName = txtShopName.Text.Trim();
                    string provinceName = ddlProvince.SelectedValue;
                    string cityName = ddlCity.SelectedValue;
                    List<string> cityTierList = new List<string>();
                    for (int i = 0; i < cblCityTier.Items.Count; i++)
                    {
                        if (cblCityTier.Items[i].Selected)
                        {
                            cityTierList.Add(cblCityTier.Items[i].Value);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(shopNo))
                    {
                        orderList = orderList.Where(s => s.shop.ShopNo.ToUpper().Contains(shopNo.ToUpper())).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(shopName))
                    {
                        orderList = orderList.Where(s => s.shop.ShopName.ToUpper().Contains(shopName)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(provinceName))
                    {
                        orderList = orderList.Where(s => s.shop.ProvinceName == provinceName).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(cityName))
                    {
                        orderList = orderList.Where(s => s.shop.CityName == cityName).ToList();
                    }
                    if (cityTierList.Any())
                    {
                        orderList = orderList.Where(s => cityTierList.Contains(s.shop.CityTier)).ToList();
                    }
                }
                if (orderList.Any())
                {
                    FinalOrderDetailTempBLL bll = new FinalOrderDetailTempBLL();
                    FinalOrderDetailTemp model;
                    orderList.ForEach(s => {
                        model = s.order;
                        model.InstallPricePOSScale = posScale;
                        bll.Update(model);
                    });
                    POSScaleInfoBLL posScaleBll = new POSScaleInfoBLL();
                    var POSScaleList = posScaleBll.GetList(s => s.POSScaleName.ToLower() == posScale.ToLower());
                    if (!POSScaleList.Any())
                    {
                        POSScaleInfo model0 = new POSScaleInfo() { POSScaleName = posScale };
                        posScaleBll.Add(model0);
                    }
                    ExcuteJs("Finish");
                    BindData();
                }
                
            }
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            FinalOrderDetailTempBLL bll=new FinalOrderDetailTempBLL();
            FinalOrderDetailTemp model;
            bool flag = false;
            foreach (RepeaterItem item in gvShop.Items)
            {
                if (item.ItemIndex != -1)
                {
                    DropDownList ddlPOSScale = (DropDownList)item.FindControl("ddlPOSScale");
                    if (ddlPOSScale.Visible == true && ddlPOSScale.SelectedValue != "")
                    { 
                        HiddenField hfShopId=(HiddenField)item.FindControl("hfShopId");
                        int shopId = int.Parse(hfShopId.Value!=""?hfShopId.Value:"0");
                        if (shopId > 0)
                        {
                            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                        join subject in CurrentContext.DbContext.Subject
                                        on order.SubjectId equals subject.Id
                                        where subject.GuidanceId == guidanceId && order.ShopId == shopId
                                        select order).ToList();
                            if (list.Any())
                            {
                                list.ForEach(s => {
                                    model = s;
                                    model.InstallPricePOSScale = ddlPOSScale.SelectedValue;
                                    bll.Update(model);
                                    flag = true;
                                });
                            }
                        }
                       
                    }
                }
            }
            if (flag)
            {
                ExcuteJs("Finish");
                BindData();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using BLL;
using Models;
using System.Text;
using DAL;

namespace WebApp.Statistics
{
    public partial class StatisticsByRegion : BasePage
    {
        string guidanceIds = string.Empty;
        string region = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = Request.QueryString["guidanceIds"];
            }
            if (Request.QueryString["region"] != null)
            {
                region = Request.QueryString["region"];
            }
            if (!IsPostBack)
            {
                BindGuidance();
                BindData();
            }
        }

        void BindGuidance()
        {
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                List<int> guidanceIdList = StringHelper.ToIntList(guidanceIds,',');
                var list = new SubjectGuidanceBLL().GetList(s => guidanceIdList.Contains(s.ItemId));
                if (list.Any())
                {
                    StringBuilder str = new StringBuilder();
                    list.ForEach(s => {
                        str.Append(s.ItemName);
                        str.Append('，');
                    });
                    labGuidanceName.Text = str.ToString().TrimEnd('，');
                    //cblSubjectCategory.Items.Clear();
                    

                }
            }
            labRegion.Text = region;
        }


        void BindData()
        {
            List<int> guidanceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
            }
            var categoryList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                join category1 in CurrentContext.DbContext.ADSubjectCategory
                                on subject.SubjectCategoryId equals category1.Id into categoryTemp
                                from category in categoryTemp.DefaultIfEmpty()
                                where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                && (subject.IsDelete == null || subject.IsDelete == false)
                                && subject.ApproveState == 1
                                && (order.IsDelete == null || order.IsDelete == false)
                                && shop.ProvinceName == region
                                && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                                select category).Distinct().OrderByDescending(s=>s.Id).ToList();
            Repeater1.DataSource = categoryList;
            Repeater1.DataBind();
            //if (categoryList.Any())
            //{

            //    bool isNull = false;
            //    categoryList.ForEach(s =>
            //    {
            //        if (s != null)
            //        {
            //            ListItem li = new ListItem();
            //            li.Text = s.CategoryName + "&nbsp;&nbsp;";
            //            li.Value = s.Id.ToString();
            //            cblSubjectCategory.Items.Add(li);
            //        }
            //        else
            //            isNull = true;
            //    });
            //    if (isNull)
            //    {
            //        cblSubjectCategory.Items.Add(new ListItem("空", "0"));
            //    }
            //}
        }

        int totalShopCount = 0;
        decimal totalPopPrice = 0;
        decimal totalArea = 0;
        decimal totalInstallPrice = 0;
        decimal totalExpressPrice = 0;
        decimal totalFreight = 0;
        decimal totalPrice = 0;
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {

                List<int> guidanceIdList = new List<int>();
                if (!string.IsNullOrWhiteSpace(guidanceIds))
                {
                    guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
                }

                Label labShopCount = (Label)e.Item.FindControl("labShopCount");
                Label labArea = (Label)e.Item.FindControl("labArea");
                Label labPOPPrice = (Label)e.Item.FindControl("labPOPPrice");
                Label labInstallPrice = (Label)e.Item.FindControl("labInstallPrice");
                Label labExpressPrice = (Label)e.Item.FindControl("labExpressPrice");
                Label labFreight = (Label)e.Item.FindControl("labFreight");
                Label labSubPrice = (Label)e.Item.FindControl("labSubPrice");

                ADSubjectCategory categoryModel = (ADSubjectCategory)e.Item.DataItem;
                Label labSubjectCategoryName = (Label)e.Item.FindControl("labSubjectCategoryName");
                int categoryId = 0;
                if (categoryModel != null)
                {
                    labSubjectCategoryName.Text = categoryModel.CategoryName;
                    categoryId = categoryModel.Id;
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                    join subject in CurrentContext.DbContext.Subject
                                    on order.SubjectId equals subject.Id
                                    join shop in CurrentContext.DbContext.Shop
                                    on order.ShopId equals shop.Id
                                    where subject.SubjectCategoryId == categoryId
                                    && guidanceIdList.Contains(subject.GuidanceId??0)
                                    && shop.ProvinceName == region
                                    && (order.IsDelete == null || order.IsDelete == false)
                                    && (subject.IsDelete == null || subject.IsDelete == false)
                                    && subject.ApproveState == 1
                                    && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                                    select order);
                    if (orderList.Any())
                    {
                        List<int> subjectIdList = orderList.Select(s => s.SubjectId ?? 0).Distinct().ToList();
                        int shopCount = orderList.Select(s => s.ShopId ?? 0).Distinct().Count();
                        labShopCount.Text = shopCount.ToString();
                        totalShopCount += shopCount;
                        decimal popPrice = 0;
                        decimal area = 0;
                        decimal installPrice = 0;
                        decimal expressPrice = 0;
                        decimal freight = 0;
                        decimal subPrice = 0;
                        StatisticPOPTotalPrice(orderList, out popPrice, out area);
                        if (area > 0)
                            labArea.Text = Math.Round(area, 2).ToString();
                        if (popPrice > 0)
                            labPOPPrice.Text = Math.Round(popPrice, 2).ToString();
                        totalPopPrice += popPrice;
                        totalArea += area;
                        //安装费
                        var installPriceList = (from price in CurrentContext.DbContext.InstallPriceShopInfo
                                                join shop in CurrentContext.DbContext.Shop
                                                on price.ShopId equals shop.Id
                                                where guidanceIdList.Contains(price.GuidanceId ?? 0)
                                                && shop.ProvinceName ==region
                                                && subjectIdList.Contains(price.SubjectId??0)
                                                select price).ToList();
                        if (installPriceList.Any())
                        {
                            installPriceList.ForEach(s =>
                            {
                                installPrice += ((s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0));
                            });
                        }
                        //快递费统计
                        subjectIdList.ForEach(gid =>
                        {
                            var freightOrderList = (from order in orderList
                                                    join subject in CurrentContext.DbContext.Subject
                                                    on order.SubjectId equals subject.Id
                                                    join guidance in CurrentContext.DbContext.SubjectGuidance
                                                    on subject.GuidanceId equals guidance.ItemId
                                                    where guidance.ItemId == gid
                                                    && (guidance.ActivityTypeId ?? 1) == 3
                                                    select order).ToList();
                            if (freightOrderList.Any())
                            {
                                expressPrice += ((freightOrderList.Select(s => s.ShopId ?? 0).Distinct().Count()) * 35);
                            }

                        });
                        if (installPrice > 0)
                            labInstallPrice.Text = Math.Round(installPrice, 2).ToString();
                        if (expressPrice > 0)
                            labExpressPrice.Text = Math.Round(expressPrice, 2).ToString();

                        totalInstallPrice += installPrice;
                        totalFreight += expressPrice;

                        subPrice = popPrice + installPrice + expressPrice + freight;
                        if (subPrice > 0)
                            labSubPrice.Text = Math.Round(subPrice, 2).ToString();
                        totalPrice += subPrice;
                    }
                }
                else
                {
                    labSubjectCategoryName.Text = "空";
                    //categoryId = categoryModel.Id;
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     where (subject.SubjectCategoryId == null || subject.SubjectCategoryId==0)
                                     && guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                     && shop.ProvinceName == region
                                     && (order.IsDelete == null || order.IsDelete == false)
                                     && (subject.IsDelete == null || subject.IsDelete == false)
                                     && subject.ApproveState == 1
                                     && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                                     select order);
                    if (orderList.Any())
                    {
                        List<int> subjectIdList = orderList.Select(s => s.SubjectId ?? 0).Distinct().ToList();
                        int shopCount = orderList.Select(s => s.ShopId ?? 0).Distinct().Count();
                        labShopCount.Text = shopCount.ToString();
                        totalShopCount += shopCount;
                        decimal popPrice = 0;
                        decimal area = 0;
                        decimal installPrice = 0;
                        decimal expressPrice = 0;
                        decimal freight = 0;
                        decimal subPrice = 0;
                        StatisticPOPTotalPrice(orderList, out popPrice, out area);
                        if (area > 0)
                            labArea.Text = Math.Round(area, 2).ToString();
                        if (popPrice > 0)
                            labPOPPrice.Text = Math.Round(popPrice, 2).ToString();
                        totalPopPrice += popPrice;
                        totalArea += area;
                        //安装费
                        var installPriceList = (from price in CurrentContext.DbContext.InstallPriceShopInfo
                                                join shop in CurrentContext.DbContext.Shop
                                                on price.ShopId equals shop.Id
                                                where guidanceIdList.Contains(price.GuidanceId ?? 0)
                                                && shop.ProvinceName == region
                                                && subjectIdList.Contains(price.SubjectId ?? 0)
                                                select price).ToList();
                        if (installPriceList.Any())
                        {
                            installPriceList.ForEach(s =>
                            {
                                installPrice += ((s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0));
                            });
                        }
                        //快递费统计
                        subjectIdList.ForEach(gid =>
                        {
                            var freightOrderList = (from order in orderList
                                                    join subject in CurrentContext.DbContext.Subject
                                                    on order.SubjectId equals subject.Id
                                                    join guidance in CurrentContext.DbContext.SubjectGuidance
                                                    on subject.GuidanceId equals guidance.ItemId
                                                    where guidance.ItemId == gid
                                                    && (guidance.ActivityTypeId ?? 1) == 3
                                                    select order).ToList();
                            if (freightOrderList.Any())
                            {
                                expressPrice += ((freightOrderList.Select(s => s.ShopId ?? 0).Distinct().Count()) * 35);
                            }

                        });

                        if (installPrice > 0)
                            labInstallPrice.Text = Math.Round(installPrice, 2).ToString();
                        if (expressPrice > 0)
                            labExpressPrice.Text = Math.Round(expressPrice, 2).ToString();

                        totalInstallPrice += installPrice;
                        totalFreight += expressPrice;

                        subPrice = popPrice + installPrice + expressPrice + freight;
                        if (subPrice > 0)
                            labSubPrice.Text = Math.Round(subPrice, 2).ToString();
                        totalPrice += subPrice;
                    }
                }

            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
               // Label labTotalShopCount = (Label)e.Item.FindControl("labTotalShopCount");
                Label labTotalArea = (Label)e.Item.FindControl("labTotalArea");
                Label labTotalPOPPrice = (Label)e.Item.FindControl("labTotalPOPPrice");
                Label labTotalInstallPrice = (Label)e.Item.FindControl("labTotalInstallPrice");
                Label labTotalExpressPrice = (Label)e.Item.FindControl("labTotalExpressPrice");
                Label labTotalPrice = (Label)e.Item.FindControl("labTotalPrice");

                //labTotalShopCount.Text = totalShopCount.ToString();
                if (totalPopPrice > 0)
                    labTotalPOPPrice.Text = Math.Round(totalPopPrice, 2).ToString();
                if (totalArea > 0)
                    labTotalArea.Text = Math.Round(totalArea, 2).ToString();
                if (totalInstallPrice > 0)
                    labTotalInstallPrice.Text = Math.Round(totalInstallPrice, 2).ToString();
                if (totalExpressPrice > 0)
                    labTotalExpressPrice.Text = Math.Round(totalExpressPrice, 2).ToString();
                if (totalPrice > 0)
                    labTotalPrice.Text = Math.Round(totalPrice, 2).ToString();
            }
        }

        
    }
}
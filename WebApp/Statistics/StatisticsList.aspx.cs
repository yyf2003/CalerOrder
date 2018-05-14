using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.Statistics
{
    public partial class StatisticsList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                BindCustomerList(ddlCustomer1);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                txtYear.Text = now.Year.ToString();
                BindGuidance();
                BindRegion();
                BindSubjectTypeList();
                BindRegion1();
            }
        }

        void BindGuidance()
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance
                            
                        }).Distinct().ToList();

            string beginDate = txtBeginDate.Text;
            string endDate = txtEndDate.Text;
            if (!string.IsNullOrWhiteSpace(beginDate) && !string.IsNullOrWhiteSpace(endDate) && StringHelper.IsDateTime(beginDate) && StringHelper.IsDateTime(endDate))
            {
                DateTime begin = DateTime.Parse(beginDate);
                DateTime end = DateTime.Parse(endDate);
                list = list.Where(s => s.guidance.BeginDate >= begin && s.guidance.EndDate < end.AddDays(1)).ToList();
            }
            else
            {
                string guidanceMonth = txtGuidanceMonth.Text;
                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                {
                    DateTime date = DateTime.Parse(guidanceMonth);
                    int year = date.Year;
                    int month = date.Month;
                    list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();
                }
            }
            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    cblGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyGuidance.Visible = !list.Any();
        }

        void BindRegion()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<string> myRegion = GetResponsibleRegion;
            if (myRegion.Any())
            {
                myRegion.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblRegion.Items.Add(li);
                });
            }
            else
                BindRegionByCustomer1(customerId, ref cblRegion);
        }

        void BindRegion1()
        {
            int customerId = int.Parse(ddlCustomer1.SelectedValue);
            List<string> myRegion = GetResponsibleRegion;
            if (myRegion.Any())
            {
                myRegion.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblRegion1.Items.Add(li);
                });
            }
            else
                BindRegionByCustomer1(customerId, ref cblRegion1);
        }

        void BindData()
        {
           
            var shopList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where guidanceSelectIdList.Contains(order.GuidanceId ?? 0)
                            && (order.IsDelete == null || order.IsDelete == false)
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            && subject.ApproveState == 1
                           // && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                            //&& ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 1)
                            //&& (regionSelectList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionSelectList.Contains(subject.PriceBlongRegion.ToLower()) : regionSelectList.Contains(order.Region.ToLower())) : 1 == 1)
                            && (regionSelectList.Any() ? regionSelectList.Contains(order.Region.ToLower()) : 1 == 1)
                            
                            select new { shop, subject,order }).ToList();
            
            List<string> provinceList = new List<string>();
            if (shopList.Any())
            {
                provinceList = shopList.Select(s => s.order.Province).Distinct().ToList();
            }
            list = shopList.Select(s=>s.order).ToList();
            //运费和新开店
            var priceOrderList0=(from order in CurrentContext.DbContext.PriceOrderDetail
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                 where guidanceSelectIdList.Contains(subject.GuidanceId ?? 0)
                                && (subject.IsDelete == null || subject.IsDelete == false)
                                && subject.ApproveState == 1
                                //&& (regionSelectList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionSelectList.Contains(subject.PriceBlongRegion.ToLower()) : regionSelectList.Contains(order.Region.ToLower())) : 1 == 1)
                                && (regionSelectList.Any() ?  regionSelectList.Contains(order.Region.ToLower()) : 1 == 1)
                                
                                && (subject.SubjectType==(int)SubjectTypeEnum.运费 || subject.SubjectType==(int)SubjectTypeEnum.新开店安装费)
                                select order).ToList();
            if (priceOrderList0.Any())
            {
                List<string> proviceList1 = priceOrderList0.Select(s => s.Province).Distinct().ToList();
                proviceList1.ForEach(s => {
                    if (!string.IsNullOrWhiteSpace(s) && !provinceList.Contains(s))
                    {
                        provinceList.Add(s);
                    }
                });
                priceOrderList = priceOrderList0;
            }
            Repeater1.DataSource = provinceList;
            Repeater1.DataBind();
        }


        List<FinalOrderDetailTemp> list = new List<FinalOrderDetailTemp>();
        List<PriceOrderDetail> priceOrderList = new List<PriceOrderDetail>();
        List<int> guidanceSelectIdList = new List<int>();
        List<string> regionSelectList = new List<string>();
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                    guidanceSelectIdList.Add(int.Parse(li.Value));
            }
            if (!guidanceSelectIdList.Any())
            {
                foreach (ListItem li in cblGuidanceList.Items)
                {
                    guidanceSelectIdList.Add(int.Parse(li.Value));
                }
            }
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                    regionSelectList.Add(li.Value.ToLower());
            }
            labSearchMonth.Text = "活动月份："+txtGuidanceMonth.Text.Trim();
            BindData();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text.Trim()))
            {
                BindGuidance();
                
            }
        }

        protected void lbUp_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month <= 1)
                {
                    year--;
                    month = 12;
                }
                else
                    month--;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                
            }
        }

        protected void lbDown_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month >= 12)
                {
                    year++;
                    month = 1;
                }
                else
                    month++;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                
            }
        }

        List<int> shopIdTotalList = new List<int>();
        decimal totalPopPrice = 0;
        decimal totalArea = 0;
        decimal totalInstallPrice = 0;
        decimal totalMeasurePrice = 0;
        decimal totalExpressPrice = 0;
        decimal totalMaterialPrice = 0;
        decimal totalNewShopPrice = 0;
        decimal totalOtherPrice = 0;

        //decimal totalFreight = 0;
        decimal totalPrice = 0;
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                string provinceName = (string)e.Item.DataItem;
                if (!string.IsNullOrWhiteSpace(provinceName))
                {
                    Label labShopCount = (Label)e.Item.FindControl("labShopCount");
                    Label labArea = (Label)e.Item.FindControl("labArea");
                    Label labPOPPrice = (Label)e.Item.FindControl("labPOPPrice");
                    Label labInstallPrice = (Label)e.Item.FindControl("labInstallPrice");
                    Label labMeasurePrice = (Label)e.Item.FindControl("labMeasurePrice");
                    Label labExpressPrice = (Label)e.Item.FindControl("labExpressPrice");
                    Label labMaterialPrice = (Label)e.Item.FindControl("labMaterialPrice");
                    Label labNewShopPrice = (Label)e.Item.FindControl("labNewShopPrice");
                    Label labOtherPrice = (Label)e.Item.FindControl("labOtherPrice");
                    //Label labFreight = (Label)e.Item.FindControl("labFreight");
                    Label labSubPrice = (Label)e.Item.FindControl("labSubPrice");
                    //getOrderList();
                    int shopCount = 0;
                    decimal subPopPrice = 0;
                    decimal subArea = 0;
                    decimal subInstallPrice = 0;
                    decimal subMeasurePrice = 0;
                    decimal subExpressPrice = 0;
                    decimal subMaterialPrice = 0;
                    decimal subNewShopPrice = 0;
                    decimal subOtherPrice = 0;
                    decimal subTotalPrice = 0;
                    List<int> shopIdSubList = new List<int>();
                    if(list.Any())
                    {
                        
                        guidanceSelectIdList.ForEach(guidanceId => {
                            var orderList = list.Where(s => s.Province == provinceName && s.GuidanceId==guidanceId).ToList();
                            if (orderList.Any())
                            {
                                List<int> shopIdList = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                                shopIdSubList.AddRange(shopIdList);
                                shopIdTotalList.AddRange(shopIdList);
                                decimal popPrice = 0;
                                decimal area = 0;
                                //decimal installPrice = 0;
                                //decimal measurePrice = 0;
                                //decimal expressPrice = 0;
                                //decimal materialPrice = 0;
                                //decimal freight = 0;
                                
                                StatisticPOPTotalPrice(orderList, out popPrice, out area);
                                
                                subPopPrice += popPrice;
                                subArea += area;



                                
                                //安装费
                               
                                //var installPriceList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId==guidanceId && shopIdList.Contains(s.ShopId ?? 0) && (s.SubjectId ?? 0) > 0).ToList();
                                var installPriceList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                                       join guidance in CurrentContext.DbContext.SubjectGuidance
                                                       on installShop.GuidanceId equals guidance.ItemId
                                                       where installShop.GuidanceId == guidanceId
                                                       && shopIdList.Contains(installShop.ShopId ?? 0)
                                                       && (installShop.SubjectId ?? 0) > 0
                                                       && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                                                       select installShop).ToList();
                                if (installPriceList.Any())
                                {
                                    installPriceList.ForEach(s =>
                                    {
                                        subInstallPrice += ((s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0));
                                    });
                                }
                                var installPriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).ToList();
                                if (installPriceOrderList.Any())
                                {
                                    subInstallPrice += (installPriceOrderList.Sum(s => s.OrderPrice ?? 0));
                                }
                                //测量费
                                var measurePriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).ToList();
                                if (measurePriceOrderList.Any())
                                {
                                    subMeasurePrice += (measurePriceOrderList.Sum(s => s.OrderPrice ?? 0));
                                }
                                //快递费统计
                                subExpressPrice += new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0)).Sum(s => s.ExpressPrice ?? 0);

                                var expressPriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.快递费||s.OrderType == (int)OrderTypeEnum.运费).ToList();
                                if (expressPriceOrderList.Any())
                                {
                                    subExpressPrice += (expressPriceOrderList.Sum(s => s.OrderPrice ?? 0));
                                }
                                //物料费
                                var materialList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.物料).ToList();
                                if (materialList.Any())
                                {
                                    subMaterialPrice += (materialList.Sum(s => (s.Quantity ?? 1) * (s.UnitPrice ?? 0)));
                                }
                                //var yunFeiOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.运费).ToList();
                                //if (yunFeiOrderList.Any())
                                //{
                                //    subNewShopPrice += (yunFeiOrderList.Sum(s => s.OrderPrice ?? 0));
                                //}
                                var otherPriceiOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费).ToList();
                                if (otherPriceiOrderList.Any())
                                {
                                    subOtherPrice += (otherPriceiOrderList.Sum(s => ((s.OrderPrice ?? 0)*(s.Quantity??1))));
                                }

                                //
                                
                            }
                        });
                        
                    }
                    //运费和新开店安装费
                   
                    if (priceOrderList.Any())
                    {
                        subNewShopPrice += priceOrderList.Where(s=>s.Province==provinceName).Sum(s => (s.Amount ?? 0));
                    }

                    labShopCount.Text = shopIdSubList.Distinct().Count().ToString();
                    if (subArea > 0)
                        labArea.Text = Math.Round(subArea, 2).ToString();
                    if (subPopPrice > 0)
                        labPOPPrice.Text = Math.Round(subPopPrice, 2).ToString();
                    if (subInstallPrice > 0)
                        labInstallPrice.Text = Math.Round(subInstallPrice, 2).ToString();
                    if (subMeasurePrice > 0)
                        labMeasurePrice.Text = Math.Round(subMeasurePrice, 2).ToString();
                    if (subExpressPrice > 0)
                        labExpressPrice.Text = Math.Round(subExpressPrice, 2).ToString();
                    if (subMaterialPrice > 0)
                        labMaterialPrice.Text = Math.Round(subMaterialPrice, 2).ToString();
                    if (subNewShopPrice > 0)
                        labNewShopPrice.Text = Math.Round(subNewShopPrice, 2).ToString();
                    if (subOtherPrice > 0)
                        labOtherPrice.Text = Math.Round(subOtherPrice, 2).ToString();

                    totalPopPrice += subPopPrice;
                    totalArea += subArea;
                    totalInstallPrice += subInstallPrice;
                    totalMeasurePrice += subMeasurePrice;
                    totalExpressPrice += subExpressPrice;
                    totalMaterialPrice += subMaterialPrice;
                    totalNewShopPrice += subNewShopPrice;
                    totalOtherPrice += subOtherPrice;
                    subTotalPrice = subPopPrice + subInstallPrice + subExpressPrice + subMeasurePrice + subMaterialPrice + subNewShopPrice + subOtherPrice;
                    if (subTotalPrice > 0)
                        labSubPrice.Text = Math.Round(subTotalPrice, 2).ToString();
                    totalPrice += subTotalPrice;
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                Label labTotalShopCount = (Label)e.Item.FindControl("labTotalShopCount");
                Label labTotalArea = (Label)e.Item.FindControl("labTotalArea");
                Label labTotalPOPPrice = (Label)e.Item.FindControl("labTotalPOPPrice");
                Label labTotalInstallPrice = (Label)e.Item.FindControl("labTotalInstallPrice");
                Label labTotalMeasurePrice = (Label)e.Item.FindControl("labTotalMeasurePrice");
                Label labTotalExpressPrice = (Label)e.Item.FindControl("labTotalExpressPrice");
                Label labTotalMaterialPrice = (Label)e.Item.FindControl("labTotalMaterialPrice");
                Label labTotalNewShopPrice = (Label)e.Item.FindControl("labTotalNewShopPrice");
                Label labTotalOtherPrice = (Label)e.Item.FindControl("labTotalOtherPrice");
                Label labTotalPrice = (Label)e.Item.FindControl("labTotalPrice");

                labTotalShopCount.Text = shopIdTotalList.Distinct().Count().ToString();
                if(totalPopPrice>0)
                    labTotalPOPPrice.Text = Math.Round(totalPopPrice, 2).ToString();
                if (totalArea > 0)
                    labTotalArea.Text = Math.Round(totalArea, 2).ToString();
                if (totalInstallPrice > 0)
                    labTotalInstallPrice.Text = Math.Round(totalInstallPrice, 2).ToString();
                if (totalMeasurePrice > 0)
                    labTotalMeasurePrice.Text = Math.Round(totalMeasurePrice, 2).ToString();
                if (totalExpressPrice > 0)
                    labTotalExpressPrice.Text = Math.Round(totalExpressPrice, 2).ToString();
                if (totalMaterialPrice > 0)
                    labTotalMaterialPrice.Text = Math.Round(totalMaterialPrice, 2).ToString();
                if (totalNewShopPrice > 0)
                    labTotalNewShopPrice.Text = Math.Round(totalNewShopPrice, 2).ToString();
                if (totalOtherPrice > 0)
                    labTotalOtherPrice.Text = Math.Round(totalOtherPrice, 2).ToString();
                if (totalPrice > 0)
                    labTotalPrice.Text = Math.Round(totalPrice, 2).ToString();
            }
        }

        
        void getOrderList()
        {
            if (!list.Any())
            {
                list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where guidanceSelectIdList.Contains(order.GuidanceId ?? 0)
                        //&& (regionSelectList.Any() ? (regionSelectList.Contains(order.Region.ToLower())) : 1 == 1)
                        && (order.IsDelete == null || order.IsDelete ==false)
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && (subject.ApproveState == 1)
                        && ((order.OrderType==(int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)||order.OrderType>1)
                        select order).ToList();
            }
            //return list;
        }

        void BindSubjectTypeList() {
            cblSubjectCategory.Items.Clear();
            int customerId = int.Parse(ddlCustomer1.SelectedValue);
            var list = new ADSubjectCategoryBLL().GetList(s => s.CustomerId == customerId);
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.CategoryName + "&nbsp;&nbsp;";
                    cblSubjectCategory.Items.Add(li);
                });
            }
        }



        List<int> shopIdTotalList1 = new List<int>();
        decimal totalPopPrice1 = 0;
        decimal totalArea1 = 0;
        decimal totalInstallPrice1 = 0;
        decimal totalMeasurePrice1 = 0;
        decimal totalExpressPrice1 = 0;
        decimal totalMaterialPrice1 = 0;
        decimal totalNewShopPrice1 = 0;
        decimal totalOtherPrice1 = 0;

        //decimal totalFreight = 0;
        decimal totalPrice1 = 0;
        protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                string provinceName = (string)e.Item.DataItem;
                if (!string.IsNullOrWhiteSpace(provinceName))
                {
                    Label labShopCount = (Label)e.Item.FindControl("labShopCount1");
                    Label labArea = (Label)e.Item.FindControl("labArea1");
                    Label labPOPPrice = (Label)e.Item.FindControl("labPOPPrice1");
                    Label labInstallPrice = (Label)e.Item.FindControl("labInstallPrice1");
                    Label labMeasurePrice = (Label)e.Item.FindControl("labMeasurePrice1");
                    Label labExpressPrice = (Label)e.Item.FindControl("labExpressPrice1");
                    Label labMaterialPrice = (Label)e.Item.FindControl("labMaterialPrice1");
                    Label labNewShopPrice = (Label)e.Item.FindControl("labNewShopPrice1");
                    Label labOtherPrice = (Label)e.Item.FindControl("labOtherPrice1");
                    //Label labFreight = (Label)e.Item.FindControl("labFreight");
                    Label labSubPrice = (Label)e.Item.FindControl("labSubPrice1");
                    //getOrderList();
                    int shopCount = 0;
                    decimal subPopPrice = 0;
                    decimal subArea = 0;
                    decimal subInstallPrice = 0;
                    decimal subMeasurePrice = 0;
                    decimal subExpressPrice = 0;
                    decimal subMaterialPrice = 0;
                    decimal subNewShopPrice = 0;
                    decimal subOtherPrice = 0;
                    decimal subTotalPrice = 0;
                    List<int> shopIdSubList = new List<int>();
                    if (list1.Any())
                    {



                        decimal popPrice = 0;
                        decimal area = 0;
                        var orderList00 = list1.Where(s => s.Province == provinceName && guidanceSelectIdList1.Contains(s.GuidanceId??0)).ToList();
                        if (orderList00.Any())
                        {
                            StatisticPOPTotalPrice(orderList00, out popPrice, out area);

                            subPopPrice += popPrice;
                            subArea += area;
                            List<int> shopIdList = orderList00.Select(s => s.ShopId ?? 0).Distinct().ToList();
                                shopIdSubList.AddRange(shopIdList);
                                shopIdTotalList1.AddRange(shopIdList);

                                var installPriceOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).ToList();
                                if (installPriceOrderList.Any())
                                {
                                    subInstallPrice += (installPriceOrderList.Sum(s => s.OrderPrice ?? 0));
                                }
                                //测量费
                                var measurePriceOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).ToList();
                                if (measurePriceOrderList.Any())
                                {
                                    subMeasurePrice += (measurePriceOrderList.Sum(s => s.OrderPrice ?? 0));
                                }
                                //快递费统计
                                subExpressPrice += new ExpressPriceDetailBLL().GetList(s => guidanceSelectIdList1.Contains(s.GuidanceId ?? 0) && shopIdList.Contains(s.ShopId ?? 0)).Sum(s => s.ExpressPrice ?? 0);

                                var expressPriceOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.发货费 ||s.OrderType == (int)OrderTypeEnum.快递费||s.OrderType == (int)OrderTypeEnum.运费).ToList();
                                if (expressPriceOrderList.Any())
                                {
                                    subExpressPrice += (expressPriceOrderList.Sum(s => s.OrderPrice ?? 0));
                                }
                                //物料费
                                var materialList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.物料).ToList();
                                if (materialList.Any())
                                {
                                    subMaterialPrice += (materialList.Sum(s => (s.Quantity ?? 1) * (s.UnitPrice ?? 0)));
                                }
                                //var yunFeiOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.运费).ToList();
                                //if (yunFeiOrderList.Any())
                                //{
                                //    subNewShopPrice += (yunFeiOrderList.Sum(s => s.OrderPrice ?? 0));
                                //}
                                var otherPriceiOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费).ToList();
                                if (otherPriceiOrderList.Any())
                                {
                                    subOtherPrice += (otherPriceiOrderList.Sum(s => ((s.OrderPrice ?? 0)*(s.Quantity??1))));
                                }

                                var installPriceList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                                        join guidance in CurrentContext.DbContext.SubjectGuidance
                                                        on installShop.GuidanceId equals guidance.ItemId
                                                        where 
                                                        guidanceSelectIdList1.Contains(installShop.GuidanceId ?? 0)
                                                        && shopIdList.Contains(installShop.ShopId ?? 0)
                                                        && (installShop.SubjectId ?? 0) > 0
                                                        && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                                                        select installShop).ToList();
                                if (installPriceList.Any())
                                {
                                    installPriceList.ForEach(s =>
                                    {
                                        subInstallPrice += ((s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0));
                                    });
                                }

                            


                        }
                        //guidanceSelectIdList1.ForEach(guidanceId =>
                        //{
                        //    var orderList = list1.Where(s => s.Province == provinceName && s.GuidanceId == guidanceId).ToList();
                        //    if (orderList.Any())
                        //    {
                        //        List<int> shopIdList = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                        //        shopIdSubList.AddRange(shopIdList);
                        //        shopIdTotalList1.AddRange(shopIdList);
                        //        //decimal popPrice = 0;
                        //        //decimal area = 0;
                               

                        //        //StatisticPOPTotalPrice(orderList, out popPrice, out area);

                        //        //subPopPrice += popPrice;
                        //        //subArea += area;




                        //        //安装费

                        //        //var installPriceList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId==guidanceId && shopIdList.Contains(s.ShopId ?? 0) && (s.SubjectId ?? 0) > 0).ToList();
                        //        var installPriceList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                        //                                join guidance in CurrentContext.DbContext.SubjectGuidance
                        //                                on installShop.GuidanceId equals guidance.ItemId
                        //                                where installShop.GuidanceId == guidanceId
                        //                                && shopIdList.Contains(installShop.ShopId ?? 0)
                        //                                && (installShop.SubjectId ?? 0) > 0
                        //                                && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                        //                                select installShop).ToList();
                        //        if (installPriceList.Any())
                        //        {
                        //            installPriceList.ForEach(s =>
                        //            {
                        //                subInstallPrice += ((s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0));
                        //            });
                        //        }
                        //        var installPriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).ToList();
                        //        if (installPriceOrderList.Any())
                        //        {
                        //            subInstallPrice += (installPriceOrderList.Sum(s => s.OrderPrice ?? 0));
                        //        }
                        //        //测量费
                        //        var measurePriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).ToList();
                        //        if (measurePriceOrderList.Any())
                        //        {
                        //            subMeasurePrice += (measurePriceOrderList.Sum(s => s.OrderPrice ?? 0));
                        //        }
                        //        //快递费统计
                        //        subExpressPrice += new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0)).Sum(s => s.ExpressPrice ?? 0);

                        //        var expressPriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费).ToList();
                        //        if (expressPriceOrderList.Any())
                        //        {
                        //            subExpressPrice += (expressPriceOrderList.Sum(s => s.OrderPrice ?? 0));
                        //        }
                        //        //物料费
                        //        var materialList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.物料).ToList();
                        //        if (materialList.Any())
                        //        {
                        //            subMaterialPrice += (materialList.Sum(s => (s.Quantity ?? 1) * (s.UnitPrice ?? 0)));
                        //        }
                        //        var yunFeiOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.运费).ToList();
                        //        if (yunFeiOrderList.Any())
                        //        {
                        //            subNewShopPrice += (yunFeiOrderList.Sum(s => s.OrderPrice ?? 0));
                        //        }
                        //        var otherPriceiOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用).ToList();
                        //        if (otherPriceiOrderList.Any())
                        //        {
                        //            subOtherPrice += (otherPriceiOrderList.Sum(s => s.OrderPrice ?? 0));
                        //        }

                        //        //

                        //    }
                        //});

                    }
                    //运费和新开店安装费

                    if (priceOrderList1.Any())
                    {
                        subNewShopPrice += priceOrderList1.Where(s => s.Province == provinceName).Sum(s => (s.Amount ?? 0));
                    }

                    labShopCount.Text = shopIdSubList.Distinct().Count().ToString();
                    if (subArea > 0)
                        labArea.Text = Math.Round(subArea, 2).ToString();
                    if (subPopPrice > 0)
                        labPOPPrice.Text = Math.Round(subPopPrice, 2).ToString();
                    if (subInstallPrice > 0)
                        labInstallPrice.Text = Math.Round(subInstallPrice, 2).ToString();
                    if (subMeasurePrice > 0)
                        labMeasurePrice.Text = Math.Round(subMeasurePrice, 2).ToString();
                    if (subExpressPrice > 0)
                        labExpressPrice.Text = Math.Round(subExpressPrice, 2).ToString();
                    if (subMaterialPrice > 0)
                        labMaterialPrice.Text = Math.Round(subMaterialPrice, 2).ToString();
                    if (subNewShopPrice > 0)
                        labNewShopPrice.Text = Math.Round(subNewShopPrice, 2).ToString();
                    if (subOtherPrice > 0)
                        labOtherPrice.Text = Math.Round(subOtherPrice, 2).ToString();

                    totalPopPrice1 += subPopPrice;
                    totalArea1 += subArea;
                    totalInstallPrice1 += subInstallPrice;
                    totalMeasurePrice1 += subMeasurePrice;
                    totalExpressPrice1 += subExpressPrice;
                    totalMaterialPrice1 += subMaterialPrice;
                    totalNewShopPrice1 += subNewShopPrice;
                    totalOtherPrice1 += subOtherPrice;
                    subTotalPrice = subPopPrice + subInstallPrice + subExpressPrice + subMeasurePrice + subMaterialPrice + subNewShopPrice + subOtherPrice;
                    if (subTotalPrice > 0)
                        labSubPrice.Text = Math.Round(subTotalPrice, 2).ToString();
                    totalPrice1 += subTotalPrice;
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                Label labTotalShopCount = (Label)e.Item.FindControl("labTotalShopCount1");
                Label labTotalArea = (Label)e.Item.FindControl("labTotalArea1");
                Label labTotalPOPPrice = (Label)e.Item.FindControl("labTotalPOPPrice1");
                Label labTotalInstallPrice = (Label)e.Item.FindControl("labTotalInstallPrice1");
                Label labTotalMeasurePrice = (Label)e.Item.FindControl("labTotalMeasurePrice1");
                Label labTotalExpressPrice = (Label)e.Item.FindControl("labTotalExpressPrice1");
                Label labTotalMaterialPrice = (Label)e.Item.FindControl("labTotalMaterialPrice1");
                Label labTotalNewShopPrice = (Label)e.Item.FindControl("labTotalNewShopPrice1");
                Label labTotalOtherPrice = (Label)e.Item.FindControl("labTotalOtherPrice1");
                Label labTotalPrice = (Label)e.Item.FindControl("labTotalPrice1");

                labTotalShopCount.Text = shopIdTotalList1.Distinct().Count().ToString();
                if (totalPopPrice1 > 0)
                    labTotalPOPPrice.Text = Math.Round(totalPopPrice1, 2).ToString();
                if (totalArea1 > 0)
                    labTotalArea.Text = Math.Round(totalArea1, 2).ToString();
                if (totalInstallPrice1 > 0)
                    labTotalInstallPrice.Text = Math.Round(totalInstallPrice1, 2).ToString();
                if (totalMeasurePrice1 > 0)
                    labTotalMeasurePrice.Text = Math.Round(totalMeasurePrice1, 2).ToString();
                if (totalExpressPrice1 > 0)
                    labTotalExpressPrice.Text = Math.Round(totalExpressPrice1, 2).ToString();
                if (totalMaterialPrice1 > 0)
                    labTotalMaterialPrice.Text = Math.Round(totalMaterialPrice1, 2).ToString();
                if (totalNewShopPrice1 > 0)
                    labTotalNewShopPrice.Text = Math.Round(totalNewShopPrice1, 2).ToString();
                if (totalOtherPrice1 > 0)
                    labTotalOtherPrice.Text = Math.Round(totalOtherPrice1, 2).ToString();
                if (totalPrice1 > 0)
                    labTotalPrice.Text = Math.Round(totalPrice1, 2).ToString();
            }
        }

        List<FinalOrderDetailTemp> list1 = new List<FinalOrderDetailTemp>();
        List<string> regionSelectList1 = new List<string>();
        List<int> subjectCategorySelectList1 = new List<int>();
        List<int> guidanceSelectIdList1 = new List<int>();
        List<PriceOrderDetail> priceOrderList1 = new List<PriceOrderDetail>();
        protected void btnSearch1_Click(object sender, EventArgs e)
        {
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategorySelectList1.Add(int.Parse(li.Value));
            }
            if (!subjectCategorySelectList1.Any())
            {
                foreach (ListItem li in cblSubjectCategory.Items)
                {
                    subjectCategorySelectList1.Add(int.Parse(li.Value));
                }
            }
            foreach (ListItem li in cblRegion1.Items)
            {
                if (li.Selected)
                    regionSelectList1.Add(li.Value.ToLower());
            }
            string year = txtYear.Text.Trim();
            if (!string.IsNullOrWhiteSpace(year)&&StringHelper.IsIntVal(year)) {
                int yearInt = int.Parse(year);
                guidanceSelectIdList1 = new SubjectGuidanceBLL().GetList(s => s.GuidanceYear == yearInt && (s.IsDelete == null || s.IsDelete == false)).Select(s=>s.ItemId).ToList();
            }
            labYear.Text = "统计年度：" + txtYear.Text.Trim();
            BindData1();
        }

       

        protected void lbUp1_Click(object sender, EventArgs e)
        {
            string year = txtYear.Text.Trim();
            if (!string.IsNullOrWhiteSpace(year) && StringHelper.IsIntVal(year))
            {
                txtYear.Text = (int.Parse(year)-1).ToString();
              
            }
        }

        protected void lbDown1_Click(object sender, EventArgs e)
        {
            string year = txtYear.Text.Trim();
            if (!string.IsNullOrWhiteSpace(year) && StringHelper.IsIntVal(year))
            {

                txtYear.Text = (int.Parse(year) + 1).ToString();

            }
        }

        protected void ddlCustomer1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectTypeList();
            BindRegion1();
        }

        void BindData1() {
            var shopList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where guidanceSelectIdList1.Contains(order.GuidanceId ?? 0)
                            && (order.IsDelete == null || order.IsDelete == false)
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            && subject.ApproveState == 1
                                // && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                                //&& ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 1)
                                //&& (regionSelectList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionSelectList.Contains(subject.PriceBlongRegion.ToLower()) : regionSelectList.Contains(order.Region.ToLower())) : 1 == 1)
                            && (regionSelectList1.Any() ? regionSelectList1.Contains(order.Region.ToLower()) : 1 == 1)

                            select new { shop, subject, order }).ToList();
            if (subjectCategorySelectList1.Any()) {
                shopList = shopList.Where(s => subjectCategorySelectList1.Contains(s.subject.SubjectCategoryId??0)).ToList();
            }
            List<string> provinceList = new List<string>();
            if (shopList.Any())
            {
                provinceList = shopList.Select(s => s.order.Province).Distinct().ToList();
            }
            list1 = shopList.Select(s => s.order).ToList();
            //运费和新开店
            var priceOrderList0 = (from order in CurrentContext.DbContext.PriceOrderDetail
                                   join subject in CurrentContext.DbContext.Subject
                                   on order.SubjectId equals subject.Id
                                   where guidanceSelectIdList1.Contains(subject.GuidanceId ?? 0)
                                  && (subject.IsDelete == null || subject.IsDelete == false)
                                  && subject.ApproveState == 1
                                       //&& (regionSelectList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionSelectList.Contains(subject.PriceBlongRegion.ToLower()) : regionSelectList.Contains(order.Region.ToLower())) : 1 == 1)
                                  && (regionSelectList1.Any() ? regionSelectList1.Contains(order.Region.ToLower()) : 1 == 1)

                                  && (subject.SubjectType == (int)SubjectTypeEnum.运费 || subject.SubjectType == (int)SubjectTypeEnum.新开店安装费)
                                   select order).ToList();
            if (priceOrderList0.Any())
            {
                List<string> proviceList1 = priceOrderList0.Select(s => s.Province).Distinct().ToList();
                proviceList1.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s) && !provinceList.Contains(s))
                    {
                        provinceList.Add(s);
                    }
                });
                priceOrderList1 = priceOrderList0;
            }
            Repeater2.DataSource = provinceList;
            Repeater2.DataBind();


            //闭店
            list2 = list1.Where(s =>s.ShopStatus!=null && s.ShopStatus.Contains("闭")).ToList();
            if (list2.Any())
            {
                Panel_Shut.Visible = true;
                List<string> provinceList2 = new List<string>();
                provinceList2 = list2.Select(s => s.Province).Distinct().ToList();
                Repeater3.DataSource = provinceList2;
                Repeater3.DataBind();

            }
            else
                Panel_Shut.Visible = false;

        }

        void getOrderList1()
        {
            if (!list1.Any())
            {
                list1 = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where guidanceSelectIdList1.Contains(order.GuidanceId ?? 0)
                            //&& (regionSelectList.Any() ? (regionSelectList.Contains(order.Region.ToLower())) : 1 == 1)
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && (subject.ApproveState == 1)
                        && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 1)
                        select order).ToList();
            }
            //return list;
        }


        List<FinalOrderDetailTemp> list2 = new List<FinalOrderDetailTemp>();

        List<int> shopIdTotalList2 = new List<int>();
        decimal totalPopPrice2 = 0;
        decimal totalArea2 = 0;
        decimal totalInstallPrice2 = 0;
        decimal totalMeasurePrice2 = 0;
        decimal totalExpressPrice2 = 0;
        decimal totalMaterialPrice2 = 0;
        decimal totalNewShopPrice2= 0;
        decimal totalOtherPrice2 = 0;

        decimal totalPrice2 = 0;
        protected void Repeater3_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                string provinceName = (string)e.Item.DataItem;
                if (!string.IsNullOrWhiteSpace(provinceName))
                {
                    Label labShopCount = (Label)e.Item.FindControl("labShopCount2");
                    Label labArea = (Label)e.Item.FindControl("labArea2");
                    Label labPOPPrice = (Label)e.Item.FindControl("labPOPPrice2");
                    Label labInstallPrice = (Label)e.Item.FindControl("labInstallPrice2");
                    Label labMeasurePrice = (Label)e.Item.FindControl("labMeasurePrice2");
                    Label labExpressPrice = (Label)e.Item.FindControl("labExpressPrice2");
                    Label labMaterialPrice = (Label)e.Item.FindControl("labMaterialPrice2");
                    Label labNewShopPrice = (Label)e.Item.FindControl("labNewShopPrice2");
                    Label labOtherPrice = (Label)e.Item.FindControl("labOtherPrice2");
                    //Label labFreight = (Label)e.Item.FindControl("labFreight");
                    Label labSubPrice = (Label)e.Item.FindControl("labSubPrice2");
                    //getOrderList();
                    int shopCount = 0;
                    decimal subPopPrice = 0;
                    decimal subArea = 0;
                    decimal subInstallPrice = 0;
                    decimal subMeasurePrice = 0;
                    decimal subExpressPrice = 0;
                    decimal subMaterialPrice = 0;
                    decimal subNewShopPrice = 0;
                    decimal subOtherPrice = 0;
                    decimal subTotalPrice = 0;
                    List<int> shopIdSubList = new List<int>();
                    if (list2.Any())
                    {



                        decimal popPrice = 0;
                        decimal area = 0;
                        var orderList00 = list2.Where(s => s.Province == provinceName && guidanceSelectIdList1.Contains(s.GuidanceId ?? 0)).ToList();
                        if (orderList00.Any())
                        {
                            StatisticPOPTotalPrice(orderList00, out popPrice, out area);

                            subPopPrice += popPrice;
                            subArea += area;
                            List<int> shopIdList = orderList00.Select(s => s.ShopId ?? 0).Distinct().ToList();
                            shopIdSubList.AddRange(shopIdList);
                            shopIdTotalList2.AddRange(shopIdList);

                            var installPriceOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).ToList();
                            if (installPriceOrderList.Any())
                            {
                                subInstallPrice += (installPriceOrderList.Sum(s => s.OrderPrice ?? 0));
                            }
                            //测量费
                            var measurePriceOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).ToList();
                            if (measurePriceOrderList.Any())
                            {
                                subMeasurePrice += (measurePriceOrderList.Sum(s => s.OrderPrice ?? 0));
                            }
                            //快递费统计
                            subExpressPrice += new ExpressPriceDetailBLL().GetList(s => guidanceSelectIdList1.Contains(s.GuidanceId ?? 0) && shopIdList.Contains(s.ShopId ?? 0)).Sum(s => s.ExpressPrice ?? 0);

                            var expressPriceOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.快递费 || s.OrderType == (int)OrderTypeEnum.运费).ToList();
                            if (expressPriceOrderList.Any())
                            {
                                subExpressPrice += (expressPriceOrderList.Sum(s => s.OrderPrice ?? 0));
                            }
                            //物料费
                            var materialList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.物料).ToList();
                            if (materialList.Any())
                            {
                                subMaterialPrice += (materialList.Sum(s => (s.Quantity ?? 1) * (s.UnitPrice ?? 0)));
                            }
                            //var yunFeiOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.运费).ToList();
                            //if (yunFeiOrderList.Any())
                            //{
                            //    subNewShopPrice += (yunFeiOrderList.Sum(s => s.OrderPrice ?? 0));
                            //}
                            var otherPriceiOrderList = orderList00.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费).ToList();
                            if (otherPriceiOrderList.Any())
                            {
                                subOtherPrice += (otherPriceiOrderList.Sum(s => ((s.OrderPrice ?? 0)*(s.Quantity??1))));
                            }

                            var installPriceList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                                    join guidance in CurrentContext.DbContext.SubjectGuidance
                                                    on installShop.GuidanceId equals guidance.ItemId
                                                    where
                                                    guidanceSelectIdList1.Contains(installShop.GuidanceId ?? 0)
                                                    && shopIdList.Contains(installShop.ShopId ?? 0)
                                                    && (installShop.SubjectId ?? 0) > 0
                                                    && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                                                    select installShop).ToList();
                            if (installPriceList.Any())
                            {
                                installPriceList.ForEach(s =>
                                {
                                    subInstallPrice += ((s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0));
                                });
                            }




                        }
                       

                    }
                   

                    labShopCount.Text = shopIdSubList.Distinct().Count().ToString();
                    if (subArea > 0)
                        labArea.Text = Math.Round(subArea, 2).ToString();
                    if (subPopPrice > 0)
                        labPOPPrice.Text = Math.Round(subPopPrice, 2).ToString();
                    if (subInstallPrice > 0)
                        labInstallPrice.Text = Math.Round(subInstallPrice, 2).ToString();
                    if (subMeasurePrice > 0)
                        labMeasurePrice.Text = Math.Round(subMeasurePrice, 2).ToString();
                    if (subExpressPrice > 0)
                        labExpressPrice.Text = Math.Round(subExpressPrice, 2).ToString();
                    if (subMaterialPrice > 0)
                        labMaterialPrice.Text = Math.Round(subMaterialPrice, 2).ToString();
                    if (subNewShopPrice > 0)
                        labNewShopPrice.Text = Math.Round(subNewShopPrice, 2).ToString();
                    if (subOtherPrice > 0)
                        labOtherPrice.Text = Math.Round(subOtherPrice, 2).ToString();

                    totalPopPrice2 += subPopPrice;
                    totalArea2 += subArea;
                    totalInstallPrice2 += subInstallPrice;
                    totalMeasurePrice2 += subMeasurePrice;
                    totalExpressPrice2 += subExpressPrice;
                    totalMaterialPrice2 += subMaterialPrice;
                    totalNewShopPrice2 += subNewShopPrice;
                    totalOtherPrice2 += subOtherPrice;
                    subTotalPrice = subPopPrice + subInstallPrice + subExpressPrice + subMeasurePrice + subMaterialPrice + subNewShopPrice + subOtherPrice;
                    if (subTotalPrice > 0)
                        labSubPrice.Text = Math.Round(subTotalPrice, 2).ToString();
                    totalPrice2 += subTotalPrice;
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                Label labTotalShopCount = (Label)e.Item.FindControl("labTotalShopCount2");
                Label labTotalArea = (Label)e.Item.FindControl("labTotalArea2");
                Label labTotalPOPPrice = (Label)e.Item.FindControl("labTotalPOPPrice2");
                Label labTotalInstallPrice = (Label)e.Item.FindControl("labTotalInstallPrice2");
                Label labTotalMeasurePrice = (Label)e.Item.FindControl("labTotalMeasurePrice2");
                Label labTotalExpressPrice = (Label)e.Item.FindControl("labTotalExpressPrice2");
                Label labTotalMaterialPrice = (Label)e.Item.FindControl("labTotalMaterialPrice2");
                Label labTotalNewShopPrice = (Label)e.Item.FindControl("labTotalNewShopPrice2");
                Label labTotalOtherPrice = (Label)e.Item.FindControl("labTotalOtherPrice2");
                Label labTotalPrice = (Label)e.Item.FindControl("labTotalPrice2");

                labTotalShopCount.Text = shopIdTotalList2.Distinct().Count().ToString();
                if (totalPopPrice2 > 0)
                    labTotalPOPPrice.Text = Math.Round(totalPopPrice2, 2).ToString();
                if (totalArea2 > 0)
                    labTotalArea.Text = Math.Round(totalArea2, 2).ToString();
                if (totalInstallPrice2 > 0)
                    labTotalInstallPrice.Text = Math.Round(totalInstallPrice2, 2).ToString();
                if (totalMeasurePrice2 > 0)
                    labTotalMeasurePrice.Text = Math.Round(totalMeasurePrice2, 2).ToString();
                if (totalExpressPrice2 > 0)
                    labTotalExpressPrice.Text = Math.Round(totalExpressPrice2, 2).ToString();
                if (totalMaterialPrice2 > 0)
                    labTotalMaterialPrice.Text = Math.Round(totalMaterialPrice2, 2).ToString();
                if (totalNewShopPrice2 > 0)
                    labTotalNewShopPrice.Text = Math.Round(totalNewShopPrice2, 2).ToString();
                if (totalOtherPrice2 > 0)
                    labTotalOtherPrice.Text = Math.Round(totalOtherPrice2, 2).ToString();
                if (totalPrice2 > 0)
                    labTotalPrice.Text = Math.Round(totalPrice2, 2).ToString();
            }
        }

        //protected void txtBeginDate_TextChanged(object sender, EventArgs e)
        //{

        //}
        
        protected void txtEndDate_TextChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void txtBeginDate_TextChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void lbClearDate_Click(object sender, EventArgs e)
        {
            txtBeginDate.Text = "";
            txtEndDate.Text = "";
            BindGuidance();
        }
    }
}
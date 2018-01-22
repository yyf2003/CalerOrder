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
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                BindGuidance();
                BindRegion();
            }
        }

        void BindGuidance()
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);

            // var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId).ToList();
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        //join type in CurrentContext.DbContext.ADOrderActivity
                        //on guidance.ActivityTypeId equals type.ActivityId
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance
                            //type
                        }).Distinct().ToList();


            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();
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
                            && (regionSelectList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionSelectList.Contains(subject.PriceBlongRegion.ToLower()) : regionSelectList.Contains(order.Region.ToLower())) : 1 == 1)
                            select new { shop, subject,order }).ToList();
            //if (regionSelectList.Any())
            //{
            //    shopList = shopList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionSelectList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionSelectList.Contains(s.shop.RegionName.ToLower())).ToList();
            //}
            List<string> provinceList = new List<string>();
            if (shopList.Any())
            {
                provinceList = shopList.Select(s => s.order.Province).Distinct().ToList();
            }
            list = shopList.Select(s=>s.order).ToList();
            Repeater1.DataSource = provinceList;
            Repeater1.DataBind();
        }


        List<FinalOrderDetailTemp> list = new List<FinalOrderDetailTemp>();
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
                    //Label labFreight = (Label)e.Item.FindControl("labFreight");
                    Label labSubPrice = (Label)e.Item.FindControl("labSubPrice");
                    //getOrderList();
                    if(list.Any())
                    {
                        int shopCount = 0;
                        decimal subPopPrice = 0;
                        decimal subArea = 0;
                        decimal subInstallPrice = 0;
                        decimal subMeasurePrice = 0;
                        decimal subExpressPrice = 0;
                        decimal subMaterialPrice = 0;
                        decimal subTotalPrice = 0;
                        List<int> shopIdSubList = new List<int>();
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
                                var installPriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.其他费用).ToList();
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

                                var expressPriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费).ToList();
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


                                //
                                
                            }
                        });
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
                        
                        totalPopPrice += subPopPrice;
                        totalArea += subArea;
                        totalInstallPrice += subInstallPrice;
                        totalMeasurePrice += subMeasurePrice;
                        totalExpressPrice += subExpressPrice;
                        totalMaterialPrice += subMaterialPrice;
                        subTotalPrice = subPopPrice + subInstallPrice + subExpressPrice + subMeasurePrice + subMaterialPrice;
                        if (subTotalPrice > 0)
                            labSubPrice.Text = Math.Round(subTotalPrice, 2).ToString();
                        totalPrice += subTotalPrice;
                    }
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
    }
}
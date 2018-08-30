using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;

namespace WebApp.OutsourcingOrder.Statistics
{
    public partial class CheckOtherPriceDetail : System.Web.UI.Page
    {
        string outsourceId = string.Empty;
        string guidanceId = string.Empty;
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        string assignType = string.Empty;
        string beginDateStr = string.Empty;
        string endDateStr = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["outsourceId"] != null)
            {
                outsourceId = Request.QueryString["outsourceId"];
            }
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
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
            if (Request.QueryString["assignType"] != null)
            {
                assignType = Request.QueryString["assignType"];
            }
            if (Request.QueryString["beginDate"] != null)
            {
                beginDateStr = Request.QueryString["beginDate"];
            }
            if (Request.QueryString["endDate"] != null)
            {
                endDateStr = Request.QueryString["endDate"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            List<int> guidanceIdList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> assignTypeList = new List<int>();
            List<int> outsourceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(outsourceId))
            {
                outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            if (!string.IsNullOrWhiteSpace(province))
            {
                provinceList = StringHelper.ToStringList(province, ',');
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                cityList = StringHelper.ToStringList(city, ',');
            }
            if (!string.IsNullOrWhiteSpace(assignType))
            {
                assignTypeList = StringHelper.ToIntList(assignType, ',');
            }

            var assignShopList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                  join shop in CurrentContext.DbContext.Shop
                                  on order.ShopId equals shop.Id
                                  where outsourceIdList.Contains(order.OutsourceId ?? 0)
                                  && guidanceIdList.Contains(order.GuidanceId ?? 0)
                                  && (order.OrderType == (int)OrderTypeEnum.其他费用 || order.OrderType == (int)OrderTypeEnum.印刷费)
                                  && (order.IsDelete == null || order.IsDelete == false)
                                  select new { order,order.OrderType,order.Remark,order.PositionDescription, shop }).ToList();

            //var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.OutsourceId == outsourceId && guidanceIdList.Contains(s.GuidanceId ?? 0));
            if (!string.IsNullOrWhiteSpace(beginDateStr) && StringHelper.IsDateTime(beginDateStr) && !string.IsNullOrWhiteSpace(endDateStr) && StringHelper.IsDateTime(endDateStr))
            {
                DateTime beginDate = DateTime.Parse(beginDateStr);
                DateTime endDate = DateTime.Parse(endDateStr);
                assignShopList = assignShopList.Where(s => s.order.AddDate >= beginDate && s.order.AddDate < endDate.AddDays(1)).ToList();
            }
            if (regionList.Any())
            {
                assignShopList = assignShopList.Where(s => s.shop.RegionName != null && regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            }
            if (provinceList.Any())
            {
                assignShopList = assignShopList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            }
            if (cityList.Any())
            {
                assignShopList = assignShopList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            }
            if (assignTypeList.Any())
            {
                assignShopList = assignShopList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
            }
            //List<int> shopIdList = assignShopList.Select(s => s.shop.Id).Distinct().ToList();
            //var orderList = new OutsourceOrderDetailBLL().GetList(s => s.OutsourceId == outsourceId && guidanceIdList.Contains(s.GuidanceId ?? 0) && shopIdList.Contains(s.ShopId ?? 0) && s.OrderType == (int)OrderTypeEnum.其他费用);
            AspNetPager1.RecordCount = assignShopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = assignShopList.OrderBy(s => s.shop.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvList.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            { 
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object orderTypeObj = item.GetType().GetProperty("OrderType").GetValue(item, null);
                    string orderType = (orderTypeObj ?? string.Empty).ToString();
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);


                    object remarkObj = item.GetType().GetProperty("Remark").GetValue(item, null);
                    string remark = (remarkObj ?? string.Empty).ToString();

                    object positionDescriptionObj = item.GetType().GetProperty("PositionDescription").GetValue(item, null);
                    string positionDescription = (positionDescriptionObj ?? string.Empty).ToString();

                    System.Text.StringBuilder remarksb = new System.Text.StringBuilder(positionDescription);

                    if (!string.IsNullOrWhiteSpace(remark))
                    {
                        remarksb.AppendFormat("({0})", remark);
                    }
                    ((Label)e.Item.FindControl("labRemark")).Text = string.Format("{0}", remarksb.ToString());
                }

            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;
using Models;


namespace WebApp.OutsourcingOrder
{
    public partial class CheckNotAssignOrder : BasePage
    {
        int guidanceId;
        string region = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["region"] != null)
            {
                region = Request.QueryString["region"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            List<string> regionList = new List<string>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            List<int> orderIdList = new List<int>();
            var assignOrderList = new OutsourceOrderDetailBLL().GetList(s => s.GuidanceId == guidanceId && (s.FinalOrderId ?? 0) > 0);
            if (assignOrderList.Any())
            {
                orderIdList = assignOrderList.Select(s => s.FinalOrderId ?? 0).Distinct().ToList();
            }
            List<FinalOrderDetailTemp> list = new FinalOrderDetailTempBLL().GetList(s => s.GuidanceId == guidanceId && (orderIdList.Any() ? (!orderIdList.Contains(s.Id)) : 1 == 1) && (s.IsDelete == null || s.IsDelete == false) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == 1 && s.GraphicLength != null && s.GraphicLength > 0 && s.GraphicWidth != null && s.GraphicWidth > 0) || s.OrderType > 1) && (s.IsValid == null || s.IsValid == true) && (s.IsProduce == null || s.IsProduce == true) && (s.OrderType != (int)OrderTypeEnum.物料));
            var orderList = (from order in list
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where 
                             //order.GuidanceId == guidanceId
                             //&& 
                             (subject.IsDelete == null || subject.IsDelete == false)
                             && (order.IsValidFromAssign == null || order.IsValidFromAssign == true)
                             && subject.ApproveState==1
                             select new
                             {
                                 order,
                                 order.OrderType,
                                 subject
                             }).ToList();
            if (regionList.Any())
            {
                orderList = orderList.Where(s =>s.order.Region!=null && regionList.Contains(s.order.Region.ToLower())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                orderList = orderList.Where(s => txtShopNo.Text.Trim().ToLower().Contains(s.order.ShopNo.ToLower())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                orderList = orderList.Where(s => txtShopName.Text.Trim().ToLower().Contains(s.order.ShopName.ToLower())).ToList();
            }
            AspNetPager1.RecordCount = orderList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            rp_orderList.DataSource = orderList.OrderBy(s=>s.order.ShopId).ThenBy(s => s.subject.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            rp_orderList.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void rp_orderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objOrderType = item.GetType().GetProperty("OrderType").GetValue(item, null);
                    string orderType = objOrderType != null ? objOrderType.ToString() : "1";
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);
                }
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

       

    }
}
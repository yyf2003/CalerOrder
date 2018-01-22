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
using Common;

namespace WebApp.Subjects.RegionApprove
{
    public partial class OrderList :BasePage
    {
        int guidanceId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (!IsPostBack)
            {
                labCSName.Text = CurrentUser.UserName;
                BindSubjectGuidance();
                BindOrder();
            }
        }

        void BindSubjectGuidance()
        {
            var model = (from guidance in CurrentContext.DbContext.SubjectGuidance
                         join type1 in CurrentContext.DbContext.ADOrderActivity
                         on guidance.ActivityTypeId equals type1.ActivityId into typeTemp
                         from type in typeTemp.DefaultIfEmpty()
                         where guidance.ItemId == guidanceId
                         select new
                         {
                             guidance.ItemName,
                             type.ActivityName
                         }).FirstOrDefault();
            if (model != null)
            {
                labItemName.Text = model.ItemName;
                labActivityType.Text = model.ActivityName;
            }
        }

        void BindOrder()
        {
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where subject.GuidanceId == guidanceId
                             && (order.IsCheckByRegion == null || order.IsCheckByRegion == false)
                             && shop.CSUserId == CurrentUser.UserId
                             select new { order,subject }).ToList();
            if (!IsPostBack)
            {
                if (orderList.Any())
                {
                    List<string> shopRegionList = orderList.Select(s => s.order.Province).Distinct().ToList();
                    labShopArea.Text = StringHelper.ListToString(shopRegionList);
                }
            }
            AspNetPager1.RecordCount = orderList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            Repeater1.DataSource = orderList.OrderByDescending(s => s.order.ShopId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            Repeater1.DataBind();

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {

        }
    }
}
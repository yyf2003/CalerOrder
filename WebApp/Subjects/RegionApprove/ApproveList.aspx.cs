using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;

namespace WebApp.Subjects.RegionApprove
{
    public partial class ApproveList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                BindData();
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

            var list = (from item in CurrentContext.DbContext.SubjectGuidance
                        join user1 in CurrentContext.DbContext.UserInfo
                        on item.AddUserId equals user1.UserId into userTemp
                        join activityType1 in CurrentContext.DbContext.ADOrderActivity
                        on item.ActivityTypeId equals activityType1.ActivityId into activityTypeTemp
                        from user in userTemp.DefaultIfEmpty()
                        from activityType in activityTypeTemp.DefaultIfEmpty()
                        where (curstomerList.Any() ? (curstomerList.Contains(item.CustomerId ?? 0)) : 1 == 1)

                        select new
                        {
                            item.ItemId,
                            item.CustomerId,
                            item.BeginDate,
                            item.AddDate,
                            item.AddUserId,
                            item.ItemName,
                            item.Remark,
                            item.SubjectNames,
                            item.EndDate,
                            AddUserName = user.RealName,
                            activityType.ActivityName,
                            item.ActivityTypeId,
                            item.IsFinish,
                            item.IsDelete
                        }).ToList();
            if (CurrentUser.RoleId != 3)
            {
                list = list.Where(s => (s.IsDelete == null || s.IsDelete == false)).ToList();
            }
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtGuidanceName.Text.Trim()))
            {
                list = list.Where(s => s.ItemName.Contains(txtGuidanceName.Text.Trim())).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.ItemId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv);

        }


        protected void Button2_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
            if (e.CommandName == "Approve")
            {
                int guidancId = int.Parse(e.CommandArgument.ToString());
                Response.Redirect("OrderList.aspx?guidanceId=" + guidancId, false);
            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object itemIdObj = item.GetType().GetProperty("ItemId").GetValue(item, null);
                    int guidanceId = itemIdObj != null ? int.Parse(itemIdObj.ToString()) : 0;
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     where subject.GuidanceId == guidanceId
                                     && shop.CSUserId == CurrentUser.UserId
                                     select order).ToList();
                    ((Label)e.Row.FindControl("labRegionCSName")).Text = CurrentUser.UserName;
                    if (orderList.Any())
                    {
                        Label labNotCheckShopCount = (Label)e.Row.FindControl("labNotCheckShopCount");
                        Label labCheckedShopCount = (Label)e.Row.FindControl("labCheckedShopCount");
                        labCheckedShopCount.Text = orderList.Where(s => s.IsCheckByRegion == true).Select(s => s.ShopId).Distinct().Count().ToString();
                        int notCheckCount = orderList.Where(s => s.IsCheckByRegion == null || s.IsCheckByRegion == false).Select(s => s.ShopId).Distinct().Count();
                        if (notCheckCount > 0)
                        {
                            labNotCheckShopCount.Text = notCheckCount.ToString();
                            labNotCheckShopCount.ForeColor = System.Drawing.Color.Red;
                        }

                    }
                    
                }
            }
        }
    }
}
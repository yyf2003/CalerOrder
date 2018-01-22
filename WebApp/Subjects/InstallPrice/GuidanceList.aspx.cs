using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using Common;

namespace WebApp.Subjects.InstallPrice
{
    public partial class GuidanceList : BasePage
    {


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                if (Request.QueryString["comeback"] != null)
                {
                    LoadSearchCondition();
                }
                else
                    CommonMethod.DestoryCookie("InstallPriceSearchCondition");
                BindData();
            }
        }

        bool isRegionOrder = false;
        List<string> cityCierList = new List<string>() { "T1", "T2", "T3" };
        List<string> myRegionList = new List<string>();
        List<int> currPageGuidanceIdList = new List<int>();
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
                        from user in userTemp.DefaultIfEmpty()
                        join customer in CurrentContext.DbContext.Customer
                        on item.CustomerId equals customer.Id
                        where (curstomerList.Any() ? (curstomerList.Contains(item.CustomerId ?? 0)) : 1 == 1)
                        && ((item.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Install)
                        && (item.HasInstallFees??true)
                        && (item.IsDelete == null || item.IsDelete == false)
                        select new
                        {
                            item.ItemId,
                            item.CustomerId,
                            item.BeginDate,
                            item.AddDate,
                            item.AddUserId,
                            item.ItemName,
                            item.EndDate,
                            AddUserName = user.RealName,
                            item.ActivityTypeId,
                            item.IsFinish,
                            item.IsDelete,
                            customer.CustomerName,
                            item.GuidanceYear,
                            item.GuidanceMonth
                        }).ToList();
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }

            if (!string.IsNullOrWhiteSpace(txtGuidanceName.Text.Trim()))
            {
                list = list.Where(s => s.ItemName.ToUpper().Contains(txtGuidanceName.Text.Trim().ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text))
            {
                string guidanceMonth = txtGuidanceMonth.Text;
                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                {
                    DateTime date = DateTime.Parse(guidanceMonth);
                    int year = date.Year;
                    int month = date.Month;
                    list = list.Where(s => s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                }
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            currPageGuidanceIdList = list.Select(s => s.ItemId).OrderByDescending(s => s).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            var installShopOrderList0 = new FinalOrderDetailTempBLL().GetList(s => currPageGuidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength != null && s.GraphicLength > 0 && s.GraphicWidth != null && s.GraphicWidth > 0) || (s.OrderType == (int)OrderTypeEnum.道具)));

            installShopOrderList = (from order in installShopOrderList0
                                    join subject in CurrentContext.DbContext.Subject
                                    on order.SubjectId equals subject.Id
                                    where subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                   && (subject.IsDelete == null || subject.IsDelete == false)
                                   && subject.ApproveState == 1
                                   && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                                   && ((order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && order.BCSIsInstall == "Y"))
                                   select order).ToList();
            installPriceShopList = InstallPriceShopInfoBll.GetList(s => currPageGuidanceIdList.Contains(s.GuidanceId ?? 0));
            gv.DataSource = list.OrderByDescending(s => s.ItemId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();

        }


        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Check")
            {
                SaveSearchCondition();
                int guidanceId = int.Parse(e.CommandArgument.ToString());
                //Response.Redirect(string.Format("Edit.aspx?itemid={0}", guidanceId), false);
                Response.Redirect(string.Format("SubmitInstallPrice.aspx?itemid={0}", guidanceId), false);
            }
        }

        List<FinalOrderDetailTemp> installShopOrderList = new List<FinalOrderDetailTemp>();
        List<InstallPriceShopInfo> installPriceShopList = new List<InstallPriceShopInfo>();
        InstallPriceShopInfoBLL InstallPriceShopInfoBll = new InstallPriceShopInfoBLL();
      
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object IdObj = item.GetType().GetProperty("ItemId").GetValue(item, null);
                    int Id = IdObj != null ? int.Parse(IdObj.ToString()) : 0;
                    object typeIdObj = item.GetType().GetProperty("ActivityTypeId").GetValue(item, null);
                    int typeId = typeIdObj != null ? int.Parse(typeIdObj.ToString()) : 1;

                    Label labActivityName = (Label)e.Row.FindControl("labActivityName");
                    labActivityName.Text = CommonMethod.GetEnumDescription<GuidanceTypeEnum>(typeId.ToString());
                    Label labInstallShopCount = (Label)e.Row.FindControl("labInstallShopCount");
                    Label labFinishCount = (Label)e.Row.FindControl("labFinishCount");
                    
                    List<int> myInstallShopIdList = installShopOrderList.Where(s => s.GuidanceId == Id).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    labInstallShopCount.Text = myInstallShopIdList.Count().ToString();
                    if (installPriceShopList.Any())
                    {
                        int finishShopCount = installPriceShopList.Where(s => s.GuidanceId == Id && myInstallShopIdList.Contains(s.ShopId ?? 0)).Select(s => s.ShopId ?? 0).Distinct().Count();
                        labFinishCount.Text = finishShopCount.ToString();
                    }
                    if (!myInstallShopIdList.Any())
                    {
                        LinkButton lbCheck = (LinkButton)e.Row.FindControl("lbCheck");
                        lbCheck.CommandName = "";
                        lbCheck.Enabled = false;
                        lbCheck.Style.Add("color", "#ccc");
                    }
                }

            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        void SaveSearchCondition()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            string month = txtGuidanceMonth.Text;
            string guidanceName = txtGuidanceName.Text.Trim();
            System.Text.StringBuilder txt = new System.Text.StringBuilder();
            txt.Append(customerId);
            txt.Append("|");
            txt.Append(month);
            txt.Append("|");
            txt.Append(guidanceName);

            HttpCookie conditionCookie = Request.Cookies["InstallPriceSearchCondition"];
            if (conditionCookie == null)
            {
                conditionCookie = new HttpCookie("InstallPriceSearchCondition");
            }
            conditionCookie.Value = txt.ToString();
            conditionCookie.Expires = DateTime.Now.AddMinutes(10);
            Response.Cookies.Add(conditionCookie);
        }

        void LoadSearchCondition()
        {
            HttpCookie conditionCookie = Request.Cookies["InstallPriceSearchCondition"];
            if (conditionCookie != null)
            {
                string conditions = conditionCookie.Value;
                if (!string.IsNullOrWhiteSpace(conditions))
                {
                    List<string> list = StringHelper.ToStringListAllowSpace(conditions, '|');
                    if (list.Any())
                    {
                        ddlCustomer.SelectedValue = list[0];
                        txtGuidanceMonth.Text = list[1];
                        txtGuidanceName.Text = list[2];
                    }
                }
                conditionCookie.Value = "";
                conditionCookie.Expires = DateTime.Now.AddSeconds(-1);
                HttpContext.Current.Response.Cookies.Add(conditionCookie);
            }
        }

    }
}
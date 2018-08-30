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
using WebApp.Base;

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
                BindGuidance();
            }
        }

        //bool isRegionOrder = false;
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
                        && (item.HasInstallFees ?? true)
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

            var guidanceList = list.OrderByDescending(s => s.ItemId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            //currPageGuidanceIdList = guidanceList.Select(s => s.ItemId).ToList();

            //var installShopOrderList0 = new FinalOrderDetailTempBLL().GetList(s => currPageGuidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength != null && s.GraphicLength > 0 && s.GraphicWidth != null && s.GraphicWidth > 0) || (s.OrderType == (int)OrderTypeEnum.道具)));
            ////东区的户外店不自动算安装费，手动下安装费
            //List<int> terrexIdList = installShopOrderList0.Where(s=>s.Channel!=null && s.Channel.ToLower().Contains("terrex") && s.Region!=null && s.Region.ToLower().Contains("east")).Select(s=>s.Id).ToList();
            //installShopOrderList0 = installShopOrderList0.Where(s => !terrexIdList.Contains(s.Id)).ToList();
            //installShopOrderList = (from order in installShopOrderList0
            //                        join subject in CurrentContext.DbContext.Subject
            //                        on order.SubjectId equals subject.Id
            //                        where subject.SubjectType != (int)SubjectTypeEnum.二次安装
            //                       && (subject.IsDelete == null || subject.IsDelete == false)
            //                       && subject.ApproveState == 1
            //                       && subject.SubjectType != (int)SubjectTypeEnum.费用订单
            //                       && (subject.IsSecondInstall??false)==false
            //                       && ((order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && order.BCSIsInstall == "Y"))

            //                       && (subject.IsSecondInstall??false)==false
            //                        select order).ToList();
            //installPriceShopList = InstallPriceShopInfoBll.GetList(s => currPageGuidanceIdList.Contains(s.GuidanceId ?? 0));
            gv.DataSource = guidanceList;
            gv.DataBind();

        }

        void BindGuidance()
        {
            List<int> curstomerList = new List<int>();
            foreach (ListItem item in ddlCustomer.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }

            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        join customer in CurrentContext.DbContext.Customer
                        on guidance.CustomerId equals customer.Id
                        join user1 in CurrentContext.DbContext.UserInfo
                        on guidance.AddUserId equals user1.UserId into userTemp
                        from user in userTemp.DefaultIfEmpty()
                        where (curstomerList.Any() ? (curstomerList.Contains(guidance.CustomerId ?? 0)) : 1 == 1)
                        && ((guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Install)
                        && (guidance.HasInstallFees ?? true)
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance.ItemId,
                            guidance.CustomerId,
                            guidance.BeginDate,
                            guidance.AddDate,
                            guidance.ItemName,
                            guidance.EndDate,
                            AddUserName = user.RealName,
                            guidance.ActivityTypeId,
                            guidance.IsFinish,
                            guidance.IsDelete,
                            customer.CustomerName,
                            guidance.GuidanceYear,
                            guidance.GuidanceMonth
                        }).ToList();
            List<int> guidanceIdTempList = list.Select(s => s.ItemId).OrderByDescending(s => s).Take(30).ToList();
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

            var guidanceList = list.OrderByDescending(s => s.ItemId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            currPageGuidanceIdList = guidanceList.Select(s => s.ItemId).ToList();
            subjectList = new SubjectBLL().GetList(s => currPageGuidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && s.SubjectType != (int)SubjectTypeEnum.二次安装 && s.SubjectType != (int)SubjectTypeEnum.费用订单 && (s.IsSecondInstall ?? false) == false);

            gv.DataSource = guidanceList;
            gv.DataBind();
        }


        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindGuidance();
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

        //List<FinalOrderDetailTemp> installShopOrderList = new List<FinalOrderDetailTemp>();
        List<InstallPriceShopInfo> installPriceShopList = new List<InstallPriceShopInfo>();
        InstallPriceShopInfoBLL InstallPriceShopInfoBll = new InstallPriceShopInfoBLL();
        List<Subject> subjectList = new List<Subject>();

        FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
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

                    List<int> myInstallShopIdList = new List<int>();

                    string redisOrderKey = "InstallPriceOrderList" + Id;
                    List<FinalOrderDetailTemp> orderListSave = null;
                    bool isError = false;
                    try
                    {
                        orderListSave = RedisHelper.Get<List<FinalOrderDetailTemp>>(redisOrderKey);
                        //orderListSave = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        //                 join subject in CurrentContext.DbContext.Subject
                        //                 on order.SubjectId equals subject.Id
                        //                 where (order.IsDelete == null || order.IsDelete == false)
                        //                 && (subject.IsDelete == null || subject.IsDelete == false)
                        //                 && subject.ApproveState == 1
                        //                 && order.GuidanceId == Id
                        //                 && order.OrderType == (int)OrderTypeEnum.POP
                        //                 select order).ToList();
                    }
                    catch (Exception ex)
                    {
                        isError = true;
                    }
                    if (!isError && orderListSave == null)
                    {
                        new DelegateClass().UpdateInstallOrderRedisDataByGuidanceId(Id);
                    }
                    if (orderListSave != null)
                    {
                        var installShopOrderList = (from order in orderListSave
                                                    join subject in subjectList
                                                    on order.SubjectId equals subject.Id
                                                    join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                                    on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                                                    from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                                                    //from subject in subjectList
                                                    where
                                                    order.OrderType == (int)OrderTypeEnum.POP
                                                    && (order.IsDelete == null || order.IsDelete == false)
                                                    //&& ((order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草") && (subjectCategory == null || (subjectCategory != null && !subjectCategory.CategoryName.Contains("常规-非活动")))) || (subject.CornerType == "三叶草" && order.BCSIsInstall == "Y") || (subjectCategory != null && subjectCategory.CategoryName.Contains("常规-非活动") && order.GenericIsInstall == "Y"))

                                                    select new { order, subjectCategory }).ToList();


                        //东区的户外店不自动算安装费，手动下安装费
                        List<int> terrexIdList = installShopOrderList.Where(s => s.order.Region != null && (s.order.Region.ToLower().Contains("east") || s.order.Region.ToLower().Contains("south"))).Select(s => s.order.Id).ToList();
                        installShopOrderList = installShopOrderList.Where(s => !terrexIdList.Contains(s.order.Id)).ToList();

                        //活动订单
                        List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
                        List<int> activityOrderShopIdList = new List<int>();
                        //常规订单
                        List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();
                        List<int> genericOrderShopIdList = new List<int>();

                        genericOrderList = installShopOrderList.Where(s => s.subjectCategory != null && s.subjectCategory.CategoryName.Contains("常规-非活动")).Select(s => s.order).ToList();

                        List<int> genericOrderIdList = new List<int>();
                        if (genericOrderList.Any())
                        {
                            genericOrderIdList = genericOrderList.Select(s => s.Id).ToList();
                            genericOrderList = genericOrderList.Where(s => ((!s.Region.ToLower().Contains("west") && cityCierList.Contains(s.CityTier) && s.IsInstall == "Y") || (s.Region.ToLower().Contains("west") && s.IsInstall == "Y"))).ToList();
                            genericOrderShopIdList = genericOrderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                        }
                        activityOrderList = installShopOrderList.Where(s => !genericOrderIdList.Contains(s.order.Id) && (s.order.IsInstall == "Y" || s.order.BCSIsInstall == "Y")).Select(s => s.order).ToList();
                        activityOrderShopIdList = activityOrderList.Select(s => s.ShopId ?? 0).Distinct().ToList();


                        myInstallShopIdList = activityOrderShopIdList.Concat(genericOrderShopIdList).ToList();
                        labInstallShopCount.Text = myInstallShopIdList.Distinct().Count().ToString();

                        //已提交的店铺
                        var installPriceShopInfoList = InstallPriceShopInfoBll.GetList(s => s.GuidanceId == Id && (s.AddType == null || s.AddType == 1));
                        List<int> genericAssginShopIdList = new List<int>();
                        List<int> activityAssginShopIdList = new List<int>();
                        if (installPriceShopInfoList.Any())
                        {
                            activityAssginShopIdList = installPriceShopInfoList.Where(s => s.SubjectType == null || s.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费).Select(s => s.ShopId ?? 0).Distinct().ToList();

                            genericAssginShopIdList = installPriceShopInfoList.Where(s => s.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费).Select(s => s.ShopId ?? 0).Distinct().ToList();

                        }

                        labFinishCount.Text = activityAssginShopIdList.Concat(genericAssginShopIdList).Distinct().Count().ToString();
                    }

                    //List<FinalOrderDetailTemp> orderList = installShopOrderList.Where(s => s.GuidanceId == Id && (s.InstallPriceAddType ?? 1) == 1).ToList();


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
            BindGuidance();
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
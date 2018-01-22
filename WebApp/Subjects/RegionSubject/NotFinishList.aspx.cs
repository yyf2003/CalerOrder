using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using DAL;

namespace WebApp.Subjects.RegionSubject
{
    public partial class NotFinishList : BasePage
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
            List<string> myRegion = GetResponsibleRegion;
            var list = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on subject.AddUserId equals user.UserId
                        join regionUser1 in CurrentContext.DbContext.UserInfo
                        on subject.SubmitUserId equals regionUser1.UserId into regionUserTemp
                        from regionUser in regionUserTemp.DefaultIfEmpty()
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance.ItemId
                        join subjectType1 in CurrentContext.DbContext.SubjectType
                        on subject.SubjectTypeId equals subjectType1.Id into temp2
                        from subjectType in temp2.DefaultIfEmpty()
                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                        on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                        from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1)
                        && (myRegion.Contains(subject.Region))
                        && subject.SubjectType==(int)SubjectTypeEnum.POP订单
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        && (guidance.IsFinish == null || guidance.IsFinish==false)
                        select new
                        {
                            subject.Id,
                            subject.AddDate,
                            subject.AddUserId,
                            subject.ApproveState,
                            subject.ApproveUserId,
                            subject.BeginDate,
                            subject.Contact,
                            subject.EndDate,
                            subject.IsDelete,
                            subject.OutSubjectName,
                            subject.Remark,
                            subject.Status,
                            subject.SubjectName,
                            subject.SubjectNo,
                            subject.Tel,
                            subject.CustomerId,
                            customer.CustomerName,
                            user.RealName,
                            guidance.ItemName,
                            subjectType.SubjectTypeName,
                            subjectCategory.CategoryName,
                            subject.GuidanceId,
                            subject.SubjectTypeId,
                            subject.SubjectCategoryId,
                            subject.SubjectType,
                            RegionSubmitUser=regionUser.RealName
                        }).ToList();
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.Contains(txtSubjectName.Text)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectNo.Text))
            {
                list = list.Where(s => s.SubjectNo.Contains(txtSubjectNo.Text)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    int subjectType = subjectTypeObj != null ? int.Parse(subjectTypeObj.ToString()) : 1;
                    object statusObj = item.GetType().GetProperty("Status").GetValue(item, null);
                    int status = statusObj != null ? int.Parse(statusObj.ToString()) : 0;
                    Label labSubjectType = (Label)e.Row.FindControl("labSubjectType");
                   
                    labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                    Label labStatus = (Label)e.Row.FindControl("labStatus");

                    LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");




                    string statusMsg = string.Empty;
                    switch (status)
                    {
                        case 1:
                            statusMsg = "<span style='color:red;'>待下单</span>";

                            break;
                        case 2:
                            statusMsg = "<span style='color:blue;'>待拆单</span>";

                            break;
                        case 3:
                            statusMsg = "<span style='color:red;'>待提交</span>";

                            break;
                        case 4:
                            statusMsg = "<span style='color:green;'>提交完成</span>";

                            break;
                    }
                    labStatus.Text = statusMsg;

                    object approveObj = item.GetType().GetProperty("ApproveState").GetValue(item, null);
                    int approve = approveObj != null ? int.Parse(approveObj.ToString()) : 0;

                    Label labApprove = (Label)e.Row.FindControl("labApprove");
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");

                    lbEdit.CommandName = "EditItem";
                    lbEdit.Enabled = true;
                    lbEdit.Style.Add("color", "blue");

                    string approveMsg = string.Empty;
                    switch (approve)
                    {
                        case 0:
                            approveMsg = "<span style='color:blue;'>待审批</span>";
                            if (status < 4)
                            {
                                approveMsg = "--";
                            }

                            break;
                        case 1:
                            approveMsg = "<span style='color:green;'>审批通过</span>";

                            break;
                        case 2:
                            approveMsg = "<span style='color:red;'>审批不通过</span>";
                            
                            break;
                        default:
                            approveMsg = "--";
                            break;
                    }

                    labApprove.Text = approveMsg;

                }
            }
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "EditItem")
            {
                Response.Redirect(string.Format("/Subjects/ADOrders/ImportOrder.aspx?subjectId={0}&fromRegion=1", id), false);

            }
            if (e.CommandName == "Check")
            {
                string url = string.Format("/Subjects/ShowOrderDetail.aspx?subjectId={0}", id);
                Response.Redirect(url, false);
            }
        }
    }
}
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

namespace WebApp.Subjects.OutsourceMaterialOrder
{
    public partial class SubjectList : BasePage
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
            List<int> companyList = MySonCompanyList.Select(s => s.Id).ToList();
            foreach (ListItem item in ddlCustomer.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }
            var list = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on subject.AddUserId equals user.UserId
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance.ItemId
                        join company in CurrentContext.DbContext.Company
                        on subject.OutsourceId equals company.Id
                        //where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1) && companyList.Contains(subject.CompanyId??0)
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1)
                        && (guidance != null ? (guidance.IsDelete == null || guidance.IsDelete == false) : 1 == 1)
                        && subject.SubjectType==(int)SubjectTypeEnum.外协订单
                        select new
                        {
                            subject.Id,
                            subject.AddDate,
                            subject.AddUserId,
                            subject.ApproveState,
                            subject.ApproveUserId,
                            subject.IsDelete,
                            subject.Remark,
                            subject.Status,
                            subject.SubjectName,
                            subject.SubjectNo,
                            subject.CustomerId,
                            customer.CustomerName,
                            user.RealName,
                            guidance.ItemName,
                            subject.GuidanceId,
                            subject.SubjectType,
                            OutsourceName=company.CompanyName
                        }).ToList();
            int approveState = -1;
            if (!string.IsNullOrWhiteSpace(rblApproveState.SelectedValue))
            {
                approveState = int.Parse(rblApproveState.SelectedValue);
            }
            if (CurrentUser.RoleId == 2)
            {
                list = list.Where(s => s.AddUserId == CurrentUser.UserId).ToList();
            }
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidance.Items)
            {
                if (li.Selected)
                    guidanceIdList.Add(int.Parse(li.Value));
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.ToUpper().Contains(txtSubjectName.Text.Trim().ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectNo.Text))
            {
                list = list.Where(s => s.SubjectNo.Contains(txtSubjectNo.Text)).ToList();
            }
           
            if (approveState != -1)
            {
                if (approveState == 0)
                    list = list.Where(s => s.ApproveState == null || s.ApproveState == 0).ToList();
                else
                    list = list.Where(s => s.ApproveState == approveState).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();

            SetPromission(gv, new object[] { btnAdd });
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddSubject.aspx",false);
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {

        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "EditItem")
            {
                
                string url = string.Format("AddSubject.aspx?subjectId={0}", id);
                Response.Redirect(url, false);

            }
            if (e.CommandName == "Check")
            {
                string url = string.Format("SubjectDetail.aspx?subjectId={0}", id);
                Response.Redirect(url, false);
            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    //object guidanceIdObj = item.GetType().GetProperty("GuidanceId").GetValue(item, null);
                    //int guidanceId = guidanceIdObj != null ? int.Parse(guidanceIdObj.ToString()) : 0;
                    //bool SubjectGuidanceIsFinish = false;
                    //SubjectGuidance guidanceModel = guidanceBll.GetModel(guidanceId);
                    //if (guidanceModel != null)
                    //{
                    //    SubjectGuidanceIsFinish = guidanceModel.IsFinish ?? false;
                    //}
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    int subjectType = subjectTypeObj != null ? int.Parse(subjectTypeObj.ToString()) : 1;
                    object statusObj = item.GetType().GetProperty("Status").GetValue(item, null);
                    int status = statusObj != null ? int.Parse(statusObj.ToString()) : 0;
                    Label labSubjectType = (Label)e.Row.FindControl("labSubjectType");
                    //labSubjectType.Text = subjectType == 1 ? "POP订单" : subjectType == 2 ? "费用订单" : subjectType == 3?"补单":subjectType == 4?"二次安装费":"";
                    labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                    Label labStatus = (Label)e.Row.FindControl("labStatus");


                    LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                    LinkButton lbModify = (LinkButton)e.Row.FindControl("lbModifyOrder");

                    

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
                            lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");

                            break;
                        case 1:
                            approveMsg = "<span style='color:green;'>审批通过</span>";

                            lbEdit.CommandName = "";
                            lbEdit.Enabled = false;
                            lbEdit.Style.Add("color", "#ccc");
                            if (CurrentUser.RoleId == 1)
                            {
                                lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");

                            }
                            else
                            {
                                lbModify.CommandName = "";
                                lbModify.Enabled = false;
                                lbModify.Style.Add("color", "#ccc");

                                lbDelete.Enabled = false;
                                lbDelete.CommandName = "";
                                lbDelete.Style.Add("color", "#ccc");
                            }
                            break;
                        case 2:
                            approveMsg = "<span style='color:red;'>审批不通过</span>";
                            lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");

                            break;
                        default:
                            approveMsg = "--";
                            break;
                    }

                    labApprove.Text = approveMsg;
                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
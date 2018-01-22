using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;


namespace WebApp.Subjects.HandMadeOrder
{
    public partial class List : BasePage
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
                        join guidance1 in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance1.ItemId into temp1
                        from guidance in temp1.DefaultIfEmpty()
                        join subjectType1 in CurrentContext.DbContext.SubjectType
                        on subject.SubjectTypeId equals subjectType1.Id into temp2
                        from subjectType in temp2.DefaultIfEmpty()
                        join activityType1 in CurrentContext.DbContext.ADOrderActivity
                        on subject.ActivityId equals activityType1.ActivityId into temp3
                        from activityType in temp3.DefaultIfEmpty()
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1)
                        && subject.IsHandMade==1
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
                            activityType.ActivityName,
                            subject.GuidanceId,
                            subject.SubjectTypeId,
                            subject.ActivityId
                        }).ToList();
            if (CurrentUser.RoleId == 2)
            {
                list = list.Where(s => s.AddUserId == CurrentUser.UserId).ToList();
            }
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }
            
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("Add.aspx",false);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {

        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object statusObj = item.GetType().GetProperty("Status").GetValue(item, null);
                    int status = statusObj != null ? int.Parse(statusObj.ToString()) : 0;
                    object subjectNameObj = item.GetType().GetProperty("SubjectName").GetValue(item, null);
                    string subjectName = subjectNameObj != null ? subjectNameObj.ToString() : "";

                    object remarkObj = item.GetType().GetProperty("Remark").GetValue(item, null);
                    string remark = remarkObj != null ? remarkObj.ToString() : "";

                    Label labStatus = (Label)e.Row.FindControl("labStatus");
                    LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");
                    Label labSubjectName = (Label)e.Row.FindControl("labSubjectName");
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(remark))
                    {
                        subjectName += "(" + remark + ")";
                    }
                    labSubjectName.Text = subjectName;

                    string statusMsg = string.Empty;
                    switch (status)
                    {
                        case 1:
                            statusMsg = "<span style='color:red;'>待下单</span>";
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
                            lbDelete.Enabled = false;
                            lbDelete.CommandName = "";
                            lbDelete.Style.Add("color", "#ccc");
                            
                            
                            lbEdit.CommandName = "";
                            lbEdit.Enabled = false;
                            lbEdit.Style.Add("color", "#ccc");
                           
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

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "EditItem")
            {
                Response.Redirect(string.Format("/Subjects/HandMadeOrder/Add.aspx?subjectId={0}", id), false);

            }
            if (e.CommandName == "Check")
            {
                Response.Redirect(string.Format("/Subjects/HandMadeOrder/CheckDetail.aspx?subjectId={0}", id), false);
                //Response.Redirect(string.Format("/Subjects/ShowOrderDetail.aspx?subjectId={0}", id), false);
            }
            if (e.CommandName == "DeleteItem")
            {
                SubjectBLL subjectBll = new SubjectBLL();
                Models.Subject model = subjectBll.GetModel(id);
                if (model != null)
                {
                    model.IsDelete = true;
                    subjectBll.Update(model);
                    BindData();
                }
            }
            
        }

        
    }
}
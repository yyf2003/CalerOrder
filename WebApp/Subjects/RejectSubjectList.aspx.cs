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


namespace WebApp.Subjects
{
    public partial class RejectSubjectList : BasePage
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

                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                        on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                        from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                        //where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1) && companyList.Contains(subject.CompanyId??0)
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1)
                        && (guidance != null ? (guidance.IsDelete == null || guidance.IsDelete == false) : 1 == 1)
                        && subject.ApproveState==2 && subject.AddUserId==CurrentUser.UserId
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
                            subject.SupplementRegion
                        }).ToList();
            


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
            
            if (guidanceIdList.Any())
            {
                list = list.Where(s => guidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();
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

        void BindGuidance()
        {
            cblGuidance.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && (s.IsDelete==false || s.IsDelete==null));
            var subjectList = new SubjectBLL().GetList(s => s.AddUserId == CurrentUser.UserId && s.GuidanceId != null && s.GuidanceId > 0 && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState==2);
            List<int> guidanceIdList = subjectList.Select(s => s.GuidanceId ?? 0).Distinct().ToList();
            list = list.Where(s => guidanceIdList.Contains(s.ItemId)).ToList();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s.ItemName + "&nbsp;";
                    li.Value = s.ItemId.ToString();
                    cblGuidance.Items.Add(li);
                });
                
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Check")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                string url = string.Format("ShowOrderDetail.aspx?fromReject=1&subjectId={0}", id);
                var model = new SubjectBLL().GetModel(id);
                if (model != null)
                {
                    if (model.SubjectType == (int)SubjectTypeEnum.补单)
                    {
                        url = string.Format("/Subjects/HandMadeOrder/CheckDetail.aspx?fromReject=1&subjectId={0}", id);
                    }
                    else
                    {
                        if (model.SupplementRegion != null && model.SupplementRegion.Length > 0 && model.SubjectType != (int)SubjectTypeEnum.正常单)
                        {

                            url = string.Format("/Subjects/RegionSubject/CheckOrderDetail.aspx?fromReject=1&subjectId={0}", id);
                        }
                        if (model.SubjectType == (int)SubjectTypeEnum.二次安装)
                        {
                            url = string.Format("/Subjects/SecondInstallFee/CheckDetail.aspx?fromReject=1&subjectId={0}", id);
                        }
                    }
                }
                Response.Redirect(url, false);

            }
        }
        SubjectGuidanceBLL guidanceBll = new SubjectGuidanceBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object guidanceIdObj = item.GetType().GetProperty("GuidanceId").GetValue(item, null);
                    int guidanceId = guidanceIdObj != null ? int.Parse(guidanceIdObj.ToString()) : 0;
                    bool SubjectGuidanceIsFinish = false;
                    SubjectGuidance guidanceModel = guidanceBll.GetModel(guidanceId);
                    if (guidanceModel != null)
                    {
                        SubjectGuidanceIsFinish = guidanceModel.IsFinish ?? false;
                    }
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    int subjectType = subjectTypeObj != null ? int.Parse(subjectTypeObj.ToString()) : 1;
                    object statusObj = item.GetType().GetProperty("Status").GetValue(item, null);
                    int status = statusObj != null ? int.Parse(statusObj.ToString()) : 0;
                    Label labSubjectType = (Label)e.Row.FindControl("labSubjectType");
                   
                    labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                    
                    
                    object subjectNameObj = item.GetType().GetProperty("SubjectName").GetValue(item, null);
                    string subjectName = subjectNameObj != null ? subjectNameObj.ToString() : "";

                    object remarkObj = item.GetType().GetProperty("Remark").GetValue(item, null);
                    string remark = remarkObj != null ? remarkObj.ToString() : "";

                    Label labSubjectName = (Label)e.Row.FindControl("labSubjectName");
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(remark))
                    {
                        subjectName += "(" + remark + ")";
                    }
                    labSubjectName.Text = subjectName;

                    
                    
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

       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.Subjects.OutsourceMaterialOrder
{
    public partial class Approve : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindData();
               
            }
        }

        void BindData()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                         join customer in CurrentContext.DbContext.Customer
                         on subject.CustomerId equals customer.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on subject.AddUserId equals user.UserId
                         join outsource in CurrentContext.DbContext.Company
                         on subject.OutsourceId equals outsource.Id
                         join guidance in CurrentContext.DbContext.SubjectGuidance
                         on subject.GuidanceId equals guidance.ItemId
                         where subject.Id == subjectId
                         select new
                         {
                             subject,
                             customer.CustomerName,
                             AddUserName = user.RealName,
                             outsource.CompanyName,
                             guidance.ItemName
                         }).FirstOrDefault();
            List<OutsourceMaterialOrderDetail> materialList = new List<Models.OutsourceMaterialOrderDetail>();
            if (model != null)
            {
                labCustomer.Text = model.CustomerName;
                labGuidance.Text = model.ItemName;
                labSubjectNo.Text = model.subject.SubjectNo;
                labSubjectName.Text = model.subject.SubjectName;
                labOutsource.Text = model.CompanyName;
                labAddUserName.Text = model.AddUserName;
                labRemark.Text = model.subject.Remark;
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>((model.subject.SubjectType ?? 0).ToString());
                if (model.subject.AddDate != null)
                    labAddDate.Text = model.subject.AddDate.ToString();
                
                materialList = new OutsourceMaterialOrderDetailBLL().GetList(s => s.OutsourceSubjectId == subjectId);

            }
            tbMaterialList.DataSource = materialList;
            tbMaterialList.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text.Trim();
            SubjectBLL subjectBll = new SubjectBLL();
            bool isApproveOk = false;
            string msg = string.Empty;
            Models.Subject model = subjectBll.GetModel(subjectId);
            if (model != null)
            {
                model.ApproveState = result;
                model.ApproveDate = DateTime.Now;
                model.ApproveUserId = CurrentUser.UserId;
                subjectBll.Update(model);
                if (!string.IsNullOrWhiteSpace(remark))
                {
                    remark = remark.Replace("\r\n", "<br/>");
                }
                ApproveInfo approveModel = new ApproveInfo() { AddDate = DateTime.Now, AddUserId = CurrentUser.UserId, Remark = remark, Result = result, SubjectId = subjectId };
                new ApproveInfoBLL().Add(approveModel);
                isApproveOk = true;
            }
            if (isApproveOk)
            {
                Alert("审批成功！", "/Subjects/RegionSubject/ApproveList.aspx");
            }
            else
                Alert("审批失败！");
        }
    }
}
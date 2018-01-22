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
    public partial class SubjectDetail : BasePage
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
                GetApproveInfo();
            }
        }

        void BindData() {
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
                        select new {
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
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>((model.subject.SubjectType??0).ToString());
                if (model.subject.AddDate!=null)
                labAddDate.Text = model.subject.AddDate.ToString();
                if ((model.subject.ApproveState ?? 0) == 2 && model.subject.AddUserId == CurrentUser.UserId)
                {
                    btnEdit.Visible = true;
                    btnDelete.Visible = true;
                }
                materialList = new OutsourceMaterialOrderDetailBLL().GetList(s => s.OutsourceSubjectId == subjectId);
                
            }
            tbMaterialList.DataSource = materialList;
            tbMaterialList.DataBind();
        }

        /// <summary>
        /// 显示审批记录
        /// </summary>
        void GetApproveInfo()
        {
            var list = (from approve in CurrentContext.DbContext.ApproveInfo
                        join user in CurrentContext.DbContext.UserInfo
                        on approve.AddUserId equals user.UserId
                        where approve.SubjectId == subjectId
                        select new
                        {
                            approve,
                            UserName = user.RealName,
                        }).ToList();
            if (list.Any())
            {
                Dictionary<int, string> approveStateDic = new Dictionary<int, string>();
                approveStateDic.Add(0, "待审批");
                approveStateDic.Add(1, "<span style='color:green;'>审批通过</span>");
                approveStateDic.Add(2, "<span style='color:red;'>审批不通过</span>");
                StringBuilder tb = new StringBuilder();
                list.ForEach(s =>
                {
                    int approveState = s.approve.Result ?? 0;
                    tb.Append("<table class=\"table\" style=\"margin-bottom:6px;\">");
                    tb.AppendFormat("<tr class=\"tr_hui\"><td style=\"width: 100px;\">审批时间</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", s.approve.AddDate);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批结果</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", approveStateDic[approveState]);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批人</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", s.UserName);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批意见</td><td style=\"text-align: left; padding-left: 5px;height: 60px;\">{0}</td></tr>", s.approve.Remark);
                    tb.Append("</table>");


                });
                approveInfoDiv.InnerHtml = tb.ToString();
                Panel_ApproveInfo.Visible = true;
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {

        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
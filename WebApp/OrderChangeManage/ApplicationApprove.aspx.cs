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
using System.Transactions;

namespace WebApp.OrderChangeManage
{
    public partial class ApplicationApprove :BasePage
    {
        int applicationId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Id"] != null)
            {
                applicationId = int.Parse(Request.QueryString["Id"]);
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            var model = (from application in CurrentContext.DbContext.OrderChangeApplication
                         join guidance in CurrentContext.DbContext.SubjectGuidance
                         on application.SubjectGuidanceId equals guidance.ItemId
                         join user in CurrentContext.DbContext.UserInfo
                         on application.AddUserId equals user.UserId
                         where application.Id == applicationId
                         select new
                         {
                             application.Id,
                             application.AddDate,
                             user.RealName,
                             guidance.ItemName

                         }).FirstOrDefault();
            if (model != null)
            {
                labApplicationUser.Text = model.RealName;
                labApplicationDate.Text = model.AddDate != null ? DateTime.Parse(model.AddDate.ToString()).ToShortDateString() : "";
                labGuidanceName.Text = model.ItemName;
                var subjectList = (from detail in CurrentContext.DbContext.OrderChangeApplicationDetail
                                   join subject in CurrentContext.DbContext.Subject
                                   on detail.SubjectId equals subject.Id
                                   where detail.ApplicationId == applicationId
                                   select new
                                   {
                                       subject.SubjectName,
                                       detail.ChangeType,
                                       detail.Remark,
                                       subject.SubjectNo,
                                       subject.SubjectType
                                   }).ToList();
                Repeater1.DataSource = subjectList;
                Repeater1.DataBind();
            }
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object changeTypeObj = item.GetType().GetProperty("ChangeType").GetValue(item, null);
                    string changeType = changeTypeObj != null ? changeTypeObj.ToString() : "";
                    Label labChangeType = (Label)e.Item.FindControl("labChangeType");
                    labChangeType.Text = CommonMethod.GetEnumDescription<OrderChangeTypeEnum>(changeType);

                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    string subjectType = subjectTypeObj != null ? subjectTypeObj.ToString() : "";
                    ((Label)e.Item.FindControl("labSubjectType")).Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType);
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            OrderChangeApplicationBLL appBll=new OrderChangeApplicationBLL();
            OrderChangeApplication appModel = appBll.GetModel(applicationId);
            bool isSuccess = true;
            string errorMsg = string.Empty;
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    if (appModel != null)
                    {
                        int result = int.Parse(rblApproveResult.SelectedValue);
                        string remark = txtRemark.Text.Trim();
                        if (CurrentUser.RoleId == 4)//财务审批
                        {
                            appModel.FinancialApproveDate = DateTime.Now;
                            appModel.FinancialApproveState = result;
                            appModel.FinancialApproveUID = CurrentUser.UserId;
                            
                        }
                        else
                        {
                            appModel.ManagerApperoveDate = DateTime.Now;
                            appModel.ManagerApperoveState = result;
                            appModel.ManagerApperoveUID = CurrentUser.UserId;
                        }
                        appBll.Update(appModel);
                        if (result == 1)
                        {
                            OrderChangeApplicationDetailBLL detailBll = new OrderChangeApplicationDetailBLL();
                            var detailList = detailBll.GetList(s => s.ApplicationId == applicationId);
                            if (detailList.Any())
                            {
                                detailList.ForEach(s =>
                                {
                                    s.ActivityDate = DateTime.Now;
                                    s.State = 1;
                                    detailBll.Update(s);
                                });
                            }
                        }
                        OrderChangeApproveInfo approveModel = new OrderChangeApproveInfo() { AddDate = DateTime.Now, AddUserId = CurrentUser.UserId, Remark = txtRemark.Text.Trim(), Result = result, SubjectId = applicationId };
                        new OrderChangeApproveInfoBLL().Add(approveModel);
                        tran.Complete();
                        
                    }
                    else
                    {
                        
                        isSuccess = false;
                        errorMsg = "数据错误";
                    }
                }
                catch (Exception ex)
                {
                   
                    isSuccess = false;
                    errorMsg = ex.Message;
                }
            }
            if (isSuccess)
            {
                ExcuteJs("Success");
                //ClientScript.RegisterStartupScript(this.Page.GetType(), "", "<script>Success();</script>", false);
            }
            else
            {
                ExcuteJs("Fail", errorMsg);
                //ClientScript.RegisterStartupScript(this.Page.GetType(), "", "<script>Fail('" + errorMsg + "');</script>", true);
            }
        }
    }
}
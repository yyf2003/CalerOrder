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
using System.Transactions;

namespace WebApp.OrderChangeManage.js
{
    public partial class AddNewApplication : BasePage
    {
        int applicationId;
        List<EnumEntity> ChangeTypeList = new List<EnumEntity>();
        OrderChangeApplicationBLL applicationBll = new OrderChangeApplicationBLL();
        OrderChangeApplicationDetailBLL detailBll = new OrderChangeApplicationDetailBLL();
        List<OrderChangeApplicationDetail> bindDetailList = new List<OrderChangeApplicationDetail>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Id"] != null)
            {
                applicationId = int.Parse(Request.QueryString["Id"]);
            }
            ChangeTypeList = CommonMethod.GetEnumList<OrderChangeTypeEnum>();
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;

                if (applicationId > 0)
                {
                    var model = (from application in CurrentContext.DbContext.OrderChangeApplication
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on application.SubjectGuidanceId equals guidance.ItemId
                                 where application.Id == applicationId
                                 select new
                                 {
                                     application.SubjectGuidanceId,
                                     guidance.GuidanceYear,
                                     guidance.GuidanceMonth,
                                     application.CustomerId
                                     
                                 }).FirstOrDefault();
                    if (model != null && model.GuidanceYear != null && model.GuidanceMonth != null)
                    {
                        if (model.CustomerId != null)
                        {
                            ddlCustomer.SelectedValue = model.CustomerId.ToString();
                            ddlCustomer.Enabled = false;
                        }
                        string month = model.GuidanceYear + "-" + model.GuidanceMonth;
                        if (StringHelper.IsDateTime(month))
                        {
                            txtGuidanceMonth.Text = month;
                            txtGuidanceMonth.Enabled = false;
                            BindGuidance();
                            int guidanceId = model.SubjectGuidanceId ?? 0;
                            foreach (ListItem li in cblGuidanceList.Items)
                            {
                                if (li.Value == guidanceId.ToString())
                                {
                                    li.Selected = true;
                                }
                            }
                            cblGuidanceList.Enabled = false;
                            lbUp.Visible = false;
                            lbDown.Visible = false;
                            BindSubject();
                        }
                    }

                }
                else
                {
                    BindGuidance();
                }
                //BindSubject();
            }
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            //ChangeMonth();
            BindGuidance();
        }

        void ChangeMonth()
        {
            //ddlGuidance.Items.Clear();
            //ddlGuidance.Items.Add(new ListItem("--请选择--", "0"));
            string monthStr = txtGuidanceMonth.Text.Trim();
            if (!string.IsNullOrWhiteSpace(monthStr) && StringHelper.IsDateTime(monthStr))
            {
                DateTime monthDate = DateTime.Parse(monthStr);
                int year = monthDate.Year;
                int month = monthDate.Month;
                var list = new SubjectGuidanceBLL().GetList(s => s.GuidanceYear == year && s.GuidanceMonth == month && (s.IsDelete == false || s.IsDelete == null));
                if (list.Any())
                {
                    list.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.ItemId.ToString();
                        li.Text = s.ItemName;
                        //ddlGuidance.Items.Add(li);
                    });
                }
            }
        }

        void BindGuidance()
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance
                          
                        }).Distinct().ToList();


            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();

            }

            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    //li.Attributes.Add("style", "margin:20px 0px 20px 0px;");
                    cblGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyGuidance.Visible = !list.Any();
        }

        void BindSubject()
        {
           
            int guidanceId = -1;
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                    guidanceId = int.Parse(li.Value);
            }
            bindDetailList = detailBll.GetList(s => s.ApplicationId == applicationId);
            var list = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && (s.SubjectType != (int)SubjectTypeEnum.二次安装 && s.SubjectType != (int)SubjectTypeEnum.运费 && s.SubjectType != (int)SubjectTypeEnum.新开店安装费) && s.AddUserId == CurrentUser.UserId);
           
            Repeater1.DataSource = list;
            Repeater1.DataBind();
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                Object item = e.Item.DataItem;
                if (item != null)
                {
                    object subjectIdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int subjectId = subjectIdObj != null ? int.Parse(subjectIdObj.ToString()) : 0;
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    int subjectType = subjectTypeObj != null ? int.Parse(subjectTypeObj.ToString()) : 1;
                    RadioButtonList rblType = (RadioButtonList)e.Item.FindControl("rblChangeType");

                    ((Label)e.Item.FindControl("labSubjectType")).Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());

                    if (ChangeTypeList.Any())
                    {
                        ChangeTypeList.ForEach(s =>
                        {
                            ListItem li = new ListItem();
                            li.Value = s.Value.ToString();
                            li.Text = s.Desction + "&nbsp;";
                            rblType.Items.Add(li);
                        });
                    }
                    if (bindDetailList.Any())
                    {
                        CheckBox cbOne = (CheckBox)e.Item.FindControl("cbOne");
                        TextBox txtRemark = (TextBox)e.Item.FindControl("txtRemark");
                        var model = bindDetailList.Where(s => s.SubjectId == subjectId).FirstOrDefault();
                        if (model != null)
                        {
                            cbOne.Checked = true;
                            rblType.SelectedValue = model.ChangeType.ToString();
                            txtRemark.Text = model.Remark;
                        }
                    }
                    else
                    {
                        var list = detailBll.GetList(s => s.SubjectId == subjectId && (s.State==null ||s.State < 2));
                        if (list.Any())
                        {
                            ((CheckBox)e.Item.FindControl("cbOne")).Enabled = false;
                        }
                    }
                }
            }
        }

        

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    int selectedNum = 0;
                    string errorMsg = string.Empty;
                    
                    //int guidanceId = int.Parse(ddlGuidance.SelectedValue);
                    int guidanceId = 0;
                    foreach (ListItem li in cblGuidanceList.Items)
                    {
                        if (li.Selected)
                            guidanceId = int.Parse(li.Value);
                    }
                    if (guidanceId > 0)
                    {
                        OrderChangeApplication applicationModel = new OrderChangeApplication();
                        if (applicationId > 0)
                        {
                            detailBll.Delete(s => s.ApplicationId == applicationId);
                            applicationModel = applicationBll.GetModel(applicationId);
                        }
                        applicationModel.SubjectGuidanceId = guidanceId;
                        applicationModel.CustomerId = int.Parse(ddlCustomer.SelectedValue);
                        if (applicationId > 0)
                        {
                            applicationBll.Update(applicationModel);
                        }
                        else
                        {
                            applicationModel.AddDate = DateTime.Now;
                            applicationModel.AddUserId = CurrentUser.UserId;
                            applicationModel.IsDelete = false;
                            applicationBll.Add(applicationModel);
                        }
                        OrderChangeApplicationDetail detailModel;
                        SubjectLogBLL subjectLogBll = new SubjectLogBLL();
                        SubjectLog subjectlogModel;
                        SubjectBLL subjectBll = new SubjectBLL();
                        Subject subjectModel;
                        foreach (RepeaterItem item in Repeater1.Items)
                        {
                            if (item.ItemIndex != -1)
                            {
                                CheckBox cbOne = (CheckBox)item.FindControl("cbOne");
                                if (cbOne.Checked)
                                {
                                    selectedNum++;
                                    HiddenField hfSubjectId = (HiddenField)item.FindControl("hfSubjectId");
                                    int subjectId = int.Parse(hfSubjectId.Value);
                                    RadioButtonList rblChangeType = (RadioButtonList)item.FindControl("rblChangeType");
                                    if (!string.IsNullOrWhiteSpace(rblChangeType.SelectedValue))
                                    {
                                        int changeType = int.Parse(rblChangeType.SelectedValue);
                                        TextBox txtRemark = (TextBox)item.FindControl("txtRemark");
                                        string remark = txtRemark.Text.Trim();
                                        if (string.IsNullOrWhiteSpace(remark))
                                        {
                                            errorMsg = "请填写变更说明";
                                            break;
                                        }
                                        detailModel = new OrderChangeApplicationDetail();
                                        detailModel.ApplicationId = applicationModel.Id;
                                        detailModel.ChangeType = changeType;
                                        detailModel.Remark = remark;
                                        detailModel.SubjectId = subjectId;
                                        detailBll.Add(detailModel);

                                        subjectlogModel = new SubjectLog();
                                        subjectlogModel.OperateDate = DateTime.Now;
                                        subjectlogModel.OperateType = "提交项目修改申请";
                                        subjectlogModel.OperateUserId = CurrentUser.UserId;
                                        subjectlogModel.SubjectId = subjectId;
                                        subjectModel = subjectBll.GetModel(subjectId);
                                        if (subjectModel != null)
                                            subjectlogModel.SubjectType = subjectModel.SubjectType;
                                        var list = subjectLogBll.GetList(s => s.SubjectId == subjectId && s.OperateType == subjectlogModel.OperateType).FirstOrDefault();
                                        if (list == null)
                                        {
                                            subjectLogBll.Add(subjectlogModel);
                                        }
                                    }
                                    else
                                    {
                                        errorMsg = "请选择变更类型";
                                        break;
                                    }
                                }
                            }
                        }
                        if (selectedNum == 0)
                        {
                            errorMsg = "请选择要变更的项目";
                        }
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            throw new Exception(errorMsg);
                        }
                        else
                        {
                            tran.Complete();
                            ScriptManager.RegisterStartupScript(UpdatePanel1, this.GetType(), "js", "Success()", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(UpdatePanel1, this.GetType(), "js", "ShowError('"+ex.Message+"')", true);
                }
            }
            
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ApplicationList.aspx",false);
        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void lbUp_Click(object sender, EventArgs e)
        {
             string guidanceMonth = txtGuidanceMonth.Text;
             if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
             {
                 DateTime date = DateTime.Parse(guidanceMonth);
                 int year = date.Year;
                 int month = date.Month;
                 if (month <= 1)
                 {
                     year--;
                     month = 12;
                 }
                 else
                     month--;
                 txtGuidanceMonth.Text = year + "-" + month;
                 BindGuidance();
             }
        }

        protected void lbDown_Click(object sender, EventArgs e)
        {
             string guidanceMonth = txtGuidanceMonth.Text;
             if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
             {
                 DateTime date = DateTime.Parse(guidanceMonth);
                 int year = date.Year;
                 int month = date.Month;
                 if (month >= 12)
                 {
                     year++;
                     month = 1;
                 }
                 else
                     month++;
                 txtGuidanceMonth.Text = year + "-" + month;
                 BindGuidance();
             }
        }
    }
}
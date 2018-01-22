using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using Common;
using DAL;

namespace WebApp.Subjects.SecondInstallFee
{
    public partial class Add : BasePage
    {
        int subjectId = 0;
        SubjectBLL subjectBll = new SubjectBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                PreviousUrl = "";
                BindGuidanceList();
                BindData();
            }
        }

        void BindData()
        {
            if (subjectId > 0)
            {
                labTitel.Text = "编辑项目-二次安装费";
                Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                if (subjectModel != null)
                {
                    ddlGuidance.SelectedValue = subjectModel.GuidanceId.ToString();
                    ddlGuidance.Enabled = false;
                    BindRegion();
                    txtSubjectName.Text = subjectModel.SubjectName;
                    ddlRegion.SelectedValue = subjectModel.Region;
                    if (subjectModel.SecondInstallPrice != null)
                        txtPrice.Text = subjectModel.SecondInstallPrice.ToString();
                    txtRemark.Text = subjectModel.Remark;
                }
            }
        }

        void BindGuidanceList()
        {
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsFinish == null || guidance.IsFinish == false) 
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select
                        guidance
                        ).Distinct().ToList();
            if (subjectId == 0)
            {
                //string begin = txtBegin.Text.Trim();
                //string end = txtEnd.Text.Trim();
                string begin = "";
                string end = "";
                if (!string.IsNullOrWhiteSpace(begin) && StringHelper.IsDateTime(begin))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    list = list.Where(s => s.BeginDate >= beginDate).ToList();
                    if (!string.IsNullOrWhiteSpace(end) && StringHelper.IsDateTime(end))
                    {
                        DateTime endDate = DateTime.Parse(end).AddDays(1);
                        list = list.Where(s => s.BeginDate < endDate).ToList();
                    }
                }
                else
                {

                    DateTime date = DateTime.Now;
                    DateTime newDate = new DateTime(date.Year, date.Month, 1);
                    DateTime beginDate = newDate.AddMonths(-1);
                    DateTime endDate = newDate.AddMonths(2);
                    list = new SubjectGuidanceBLL().GetList(s => s.BeginDate >= beginDate && s.BeginDate < endDate && (s.IsFinish == null || s.IsFinish == false) && (s.IsDelete == null || s.IsDelete == false));
                }
            }
            if (CurrentUser.RoleId == 2)
            {
                var subjectList = new SubjectBLL().GetList(s => s.AddUserId == CurrentUser.UserId && s.GuidanceId != null && s.GuidanceId > 0 && s.ApproveState == 1 && (s.IsDelete == null || s.IsDelete == false) && s.SubjectType == 1);
                List<int> guidanceIdList = subjectList.Select(s => s.GuidanceId ?? 0).Distinct().ToList();
                list = list.Where(s => guidanceIdList.Contains(s.ItemId)).ToList();
            }
            if (list.Any())
            {
                ddlGuidance.DataSource = list.OrderByDescending(s => s.ItemId).ToList();
                ddlGuidance.DataTextField = "ItemName";
                ddlGuidance.DataValueField = "ItemId";
                ddlGuidance.DataBind();
                ddlGuidance.Items.Insert(0, new ListItem("请选择", "0"));

            }

        }

        void BindRegion()
        {
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(guidanceId);
            if (model != null)
            {
                int customerId = model.CustomerId ?? 0;
                hfCustomerId.Value = customerId.ToString();
                LoadRegion(ref ddlRegion, customerId);
            }
        }


     

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Save();
            Alert("提交成功！", "List.aspx");
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            Save();
            Response.Redirect("ImportOrder.aspx?subjectId=" + subjectId, false);
        }

        string CreateSubjectNo()
        {
            System.Text.StringBuilder code = new System.Text.StringBuilder();
            DateTime date = DateTime.Now;
            code.Append(date.Year).Append(date.Month.ToString().PadLeft(2, '0')).Append(date.Day.ToString().PadLeft(2, '0')).Append(date.Hour.ToString().PadLeft(2, '0')).Append(date.Minute.ToString().PadLeft(2, '0')).Append(date.Second.ToString().PadLeft(2, '0'));
            return code.ToString();
        }

        


        void Save()
        {
            string price = txtPrice.Text;
            if (subjectId > 0)
            {
                //编辑
                Subject model = subjectBll.GetModel(subjectId);
              
                model.Remark = txtRemark.Text.Trim();
                model.SubjectName = txtSubjectName.Text.Trim();
                if (ddlRegion.SelectedValue != "0")
                    model.Region = ddlRegion.SelectedValue;
                model.SecondInstallPrice = StringHelper.IsDecimal(price);
                subjectBll.Update(model);

            }
            else
            {
               
                Subject newModel = new Subject();
                newModel.SubjectType = (int)SubjectTypeEnum.二次安装;//二次安装费用订单
                newModel.ApproveDate = null;
                newModel.ApproveRemark = "";
                newModel.ApproveState = 0;
                newModel.ApproveUserId = 0;
                newModel.Status = 4;
                newModel.SubjectName = txtSubjectName.Text.Trim();
                newModel.SubjectNo = CreateSubjectNo();
                //newModel.OriginalSubjectId = newModel.Id;
                newModel.Remark = txtRemark.Text.Trim();
                newModel.SecondInstallPrice = StringHelper.IsDecimal(price);
                newModel.AddDate = DateTime.Now;
                newModel.AddUserId = CurrentUser.UserId;
                if (ddlRegion.SelectedValue != "0")
                    newModel.Region = ddlRegion.SelectedValue;
                newModel.CompanyId = CurrentUser.CompanyId;
                newModel.CustomerId = int.Parse(hfCustomerId.Value);
                newModel.GuidanceId = int.Parse(ddlGuidance.SelectedValue);
                newModel.IsDelete = false;
                subjectBll.Add(newModel);
                subjectId = newModel.Id;
            }
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGuidanceList();
        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(PreviousUrl,false);
        }
    }
}
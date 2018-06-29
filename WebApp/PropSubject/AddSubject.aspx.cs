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

namespace WebApp.PropSubject
{
    public partial class AddSubject : BasePage
    {
        int subjectId;
        SubjectBLL subjectBll = new SubjectBLL();
        Subject subjectModel;
        int ItemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["itemId"] != null)
            {
                ItemId = int.Parse(Request.QueryString["itemId"]);

            }
            if (!IsPostBack)
            {
                //this.PreviousUrl = "";
                BindMyCustomerList(ddlCustomer);
                BindGuidanceList();
                BindData();
            }
        }

        void BindGuidanceList()
        {
            ddlGuidance.Items.Clear();

            int customerId = int.Parse(ddlCustomer.SelectedValue);
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            DateTime now = DateTime.Now;
            var list = new SubjectGuidanceBLL().GetList(s =>(s.AddType??1)==(int)GuidanceAddTypeEnum.Prop && s.CustomerId == customerId && (s.IsFinish == null || s.IsFinish == false) && (s.IsDelete == null || s.IsDelete == false)).OrderByDescending(s => s.ItemId).ToList();
            
            DateTime date = DateTime.Now;
            list = list.Where(s => s.EndDate >= date).ToList();
            if (list.Any())
            {
                ddlGuidance.DataSource = list;
                ddlGuidance.DataTextField = "ItemName";
                ddlGuidance.DataValueField = "ItemId";
                ddlGuidance.DataBind();

                if (ItemId > 0)
                {
                    ddlGuidance.SelectedValue = ItemId.ToString();
                    ChangeGuidance();
                    ddlGuidance.Enabled = false;
                    ddlCustomer.Enabled = false;
                }
                if (subjectId > 0)
                {
                    ddlCustomer.Enabled = false;
                    ddlGuidance.Enabled = false;
                }
            }
            ddlGuidance.Items.Insert(0, new ListItem("请选择", "0"));
        }

        void BindData()
        {
            Subject model = new SubjectBLL().GetModel(subjectId);
            if (model != null)
            {
                if(model.CustomerId!=null)
                    ddlCustomer.SelectedValue = model.CustomerId.ToString();
                txtSubjectName.Text = model.SubjectName;
                if (model.GuidanceId != null)
                    ddlGuidance.SelectedValue = model.GuidanceId.ToString();
                if (model.BeginDate!=null)
                   txtBeginDate.Text = DateTime.Parse(model.BeginDate.ToString()).ToShortDateString();
                if (model.EndDate != null)
                    txtEndDate.Text = DateTime.Parse(model.EndDate.ToString()).ToShortDateString();
                txtRemark.Text = model.Remark;
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidanceList();
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeGuidance();
        }

        void ChangeGuidance()
        {
            int itemId = int.Parse(ddlGuidance.SelectedValue);
            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(itemId);
            if (model != null)
            {
                if (model.BeginDate != null)
                {
                    txtBeginDate.Text = DateTime.Parse(model.BeginDate.ToString()).ToShortDateString();
                }
                if (model.EndDate != null)
                {
                    txtEndDate.Text = DateTime.Parse(model.EndDate.ToString()).ToShortDateString();
                }
               
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            labMsg.Text = "";
            if (CheckSubject(txtSubjectName.Text.Trim(), subjectId))
            {
                labMsg.Text = "项目名称重复";
                return;
            }
            if (subjectId > 0)
            {
                subjectModel = subjectBll.GetModel(subjectId);
            }
            else
            {
                subjectModel = new Subject();
                subjectModel.AddOrderType = 3;
                subjectModel.AddUserId = CurrentUser.UserId;
                subjectModel.AddDate = DateTime.Now;
                subjectModel.IsDelete = false;
                subjectModel.Status = 1;
                subjectModel.SubjectNo = CreateSubjectNo();
                subjectModel.CompanyId = CurrentUser.CompanyId;
                subjectModel.ApproveState = 0;
                subjectModel.SubjectType = (int)SubjectTypeEnum.道具订单;
            }
            subjectModel.BeginDate = DateTime.Parse(txtBeginDate.Text.Trim());
            subjectModel.CustomerId = int.Parse(ddlCustomer.SelectedValue);
            subjectModel.EndDate = DateTime.Parse(txtEndDate.Text.Trim());
            subjectModel.SubjectName = StringHelper.ReplaceSpecialChar(txtSubjectName.Text.Trim());
            subjectModel.Remark = StringHelper.ReplaceSpecialChar(txtRemark.Text.Trim());
            if (subjectId > 0)
            {
                subjectBll.Update(subjectModel);
            }
            else
            {
                if (ItemId == 0)
                    ItemId = int.Parse(ddlGuidance.SelectedValue);
                subjectModel.GuidanceId = ItemId;
                subjectBll.Add(subjectModel);
            }
            Response.Redirect("AddOrder.aspx?subjectId=" + subjectModel.Id, false);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Subjects/SubjectList.aspx", false);
        }


        /// <summary>
        /// 检查项目名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool CheckSubject(string name, int id)
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            //var list = subjectBll.GetList(s => StringHelper.ReplaceSpace(s.SubjectName) == StringHelper.ReplaceSpace(name) && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1));
            name = StringHelper.ReplaceSpace(name);
            var list = subjectBll.GetList(s => s.SubjectName.Replace(" ", "").ToLower() == name.ToLower() && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1) && (s.IsDelete == null || s.IsDelete == false));

            return list.Any();
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using Common;

namespace WebApp.Subjects.SupplementByRegion
{
    public partial class Add : BasePage
    {
        int subjectId;
        SubjectBLL subjectBll = new SubjectBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindGuidanceList();
                BindMyCustomerList(ddlCustomer);
                BindRegion();
                BindData();
            }
        }

        void BindGuidanceList()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            DateTime now = DateTime.Now;

            var list = new SubjectGuidanceBLL().GetList(s => (s.IsFinish == null || s.IsFinish == false) && (s.IsDelete == null || s.IsDelete == false)).OrderByDescending(s => s.ItemId).ToList();
            if (subjectId == 0)
            {
                DateTime date = DateTime.Now;
                DateTime newDate = new DateTime(date.Year, date.Month, 1);
                DateTime beginDate = newDate.AddMonths(-1);
                DateTime endDate = newDate.AddMonths(2);
                list = list.Where(s => s.BeginDate >= beginDate && s.BeginDate < endDate).ToList();
            }
            if (list.Any())
            {
                ddlGuidance.DataSource = list;
                ddlGuidance.DataTextField = "ItemName";
                ddlGuidance.DataValueField = "ItemId";
                ddlGuidance.DataBind();
                ddlGuidance.Items.Insert(0, new ListItem("请选择", "0"));

                //if (subjectId > 0)
                //    ddlGuidance.Enabled = false;
            }

        }

        void BindData()
        {
            if (subjectId > 0)
            {
                Subject model = subjectBll.GetModel(subjectId);
                if (model != null)
                {
                    if (model.GuidanceId != null)
                    {
                        ddlGuidance.SelectedValue = model.GuidanceId.ToString();
                    }
                    if (model.CustomerId != null)
                    {
                        ddlCustomer.SelectedValue = model.CustomerId.ToString();
                        BindRegion();
                    }
                    if (model.BeginDate != null)
                        txtBeginDate.Text = DateTime.Parse(model.BeginDate.ToString()).ToShortDateString();
                    if (model.EndDate != null)
                        txtEndDate.Text = DateTime.Parse(model.EndDate.ToString()).ToShortDateString();
                    rblRegion.SelectedValue = model.SupplementRegion;
                    txtSubjectName.Text = model.SubjectName;
                    txtRemark.Text = model.Remark;
                }
            }

        }

        void BindRegion()
        {
            rblRegion.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            if (customerId > 0)
            {
                List<string> myRegions = GetResponsibleRegion;
                if (!myRegions.Any())
                {
                    myRegions = new RegionBLL().GetList(s => s.CustomerId == customerId).Select(s=>s.RegionName).Distinct().ToList();
                }
                if (myRegions.Any())
                {
                    myRegions.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s;
                        li.Text = s + "&nbsp;&nbsp;";
                        rblRegion.Items.Add(li);
                    });
                }
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            
            
            labMsg.Text = "";
            Subject subjectModel;
            string subjectName = string.Empty;
            if (CheckSubject(txtSubjectName.Text.Trim(), subjectId))
            {
                labMsg.Text = "项目名称重复";
                return;
            }
            subjectName = txtSubjectName.Text.Trim();
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
                subjectModel.Status = 3;
                subjectModel.SubjectNo = CreateSubjectNo();
                subjectModel.CompanyId = CurrentUser.CompanyId;
                subjectModel.ApproveState = 0;
            }
            subjectModel.BeginDate = DateTime.Parse(txtBeginDate.Text.Trim());
            subjectModel.CustomerId = int.Parse(ddlCustomer.SelectedValue);
            subjectModel.EndDate = DateTime.Parse(txtEndDate.Text.Trim());
            subjectModel.Remark = txtRemark.Text;
            subjectModel.SubjectName = subjectName;
            subjectModel.SupplementRegion = rblRegion.SelectedValue;
            subjectModel.SubjectType = (int)SubjectTypeEnum.分区补单;//
            subjectModel.GuidanceId = int.Parse(ddlGuidance.SelectedValue);
            if (subjectId > 0)
            {
                subjectBll.Update(subjectModel);
            }
            else
            {
                subjectBll.Add(subjectModel);
            }
            Response.Redirect("ImportOrder.aspx?subjectId=" + subjectModel.Id);
        }

        

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
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

        /// <summary>
        /// 检查项目名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool CheckSubject(string name, int id)
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            name = StringHelper.ReplaceSpace(name);
            var list = subjectBll.GetList(s => s.SubjectName.Replace(" ", "").ToLower() == name.ToLower() && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1) && (s.IsDelete == null || s.IsDelete == false));

            return list.Any();
        }
    }
}
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


namespace WebApp.Subjects.NewShopSubject
{
    public partial class AddSubject : BasePage
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
            }
        }

        void BindGuidanceList()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            DateTime now = DateTime.Now;
            
            var list = new SubjectGuidanceBLL().GetList(s => 1 == 1).OrderByDescending(s => s.ItemId).ToList();
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
                
                if (subjectId > 0)
                    ddlGuidance.Enabled = false;
            }

        }

        void BindSubjectType(int itemId)
        {

            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(itemId);
            ddlSubjectType.Items.Clear();
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
                var typeList = new SubjectTypeBLL().GetList(s => s.GuidanceId == itemId && (s.IsDelete == false || s.IsDelete == null));
                if (typeList.Any())
                {
                    typeList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectTypeName;
                        ddlSubjectType.Items.Add(li);
                    });
                }
            }
            ddlSubjectType.Items.Insert(0, new ListItem("请选择", "0"));
        }

        void BindSubjectCategory()
        {
            var List = new ADSubjectCategoryBLL().GetList(s => s.Id > 0);
            if (List.Any())
            {
                List.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.CategoryName;
                    ddlSubjectCategory.Items.Add(li);
                });
            }
        }

        void BindRegion()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<string> myRegions = GetResponsibleRegion;
            if (!myRegions.Any())
            {
                myRegions = new RegionBLL().GetList(s => s.CustomerId == customerId).Select(s => s.RegionName).Distinct().ToList();
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

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNext_Click(object sender, EventArgs e)
        {
            labMsg.Text = "";
            Subject subjectModel;
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
                //subjectModel.AddOrderType = int.Parse(rblOrderType.SelectedValue);
                subjectModel.AddOrderType = 3;
                subjectModel.AddUserId = CurrentUser.UserId;
                subjectModel.AddDate = DateTime.Now;
                subjectModel.IsDelete = false;
                subjectModel.Status = 1;
                subjectModel.SubjectNo = CreateSubjectNo();
                subjectModel.CompanyId = CurrentUser.CompanyId;
                subjectModel.ApproveState = 0;
            }
            subjectModel.BeginDate = DateTime.Parse(txtBeginDate.Text.Trim());
            
            subjectModel.CustomerId = int.Parse(ddlCustomer.SelectedValue);
            subjectModel.EndDate = DateTime.Parse(txtEndDate.Text.Trim());
            subjectModel.Remark = txtRemark.Text;
            subjectModel.SubjectName = txtSubjectName.Text.Trim();
            subjectModel.Region = rblRegion.SelectedValue;
            subjectModel.SubjectCategoryId = int.Parse(ddlSubjectCategory.SelectedValue);
            subjectModel.SubjectTypeId = int.Parse(ddlSubjectType.SelectedValue);
            subjectModel.SubjectType = (int)SubjectTypeEnum.新开店订单;//
            if (subjectId > 0)
            {
                subjectBll.Update(subjectModel);
            }
            else
            {
                subjectModel.GuidanceId = int.Parse(ddlGuidance.SelectedValue);
                subjectBll.Add(subjectModel);
            }
            Response.Redirect("AddShop.aspx?subjectId=" + subjectModel.Id, false);
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("SubjectList.aspx",false);
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            int itemId = int.Parse((sender as DropDownList).SelectedValue);
            BindSubjectType(itemId);
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using Common;
using DAL;

namespace WebApp.Subjects.HandMadeOrder
{
    public partial class Add : BasePage
    {
        int subjectId = 0;
        //string url = string.Empty;
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
                labTitel.Text = "编辑项目";
                Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                if (subjectModel != null)
                {
                    if (subjectModel.GuidanceId != null)
                    {
                        ddlGuidance.SelectedValue = subjectModel.GuidanceId.ToString();
                        BindSubjects();
                        if (subjectModel.HandMakeSubjectId != null)
                            ddlSubject.SelectedValue = subjectModel.HandMakeSubjectId.ToString();
                        txtRemark.Text = subjectModel.Remark;
                    }
                }
            }
        }

        void BindGuidanceList()
        {
            
            
            //var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
            //            join subject in CurrentContext.DbContext.Subject
            //            on guidance.ItemId equals subject.GuidanceId
            //            where (subject.IsDelete == null || subject.IsDelete == false)
            //            && subject.ApproveState == 1
            //            && (guidance.IsFinish == null || guidance.IsFinish == false)
            //            && (guidance.IsDelete == null || guidance.IsDelete == false)
            //            select
            //            guidance
            //            ).Distinct().ToList();
            var list = new SubjectGuidanceBLL().GetList(s => (s.IsFinish == null || s.IsFinish == false) && (s.IsDelete == null || s.IsDelete == false));
            if (subjectId == 0)
            {
                string begin = txtBegin.Text.Trim();
                string end = txtEnd.Text.Trim();
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
                var subjectList = new SubjectBLL().GetList(s => s.AddUserId == CurrentUser.UserId && s.GuidanceId != null && s.GuidanceId > 0 && s.ApproveState == 1 && (s.IsDelete == null || s.IsDelete == false) && (s.SubjectType == (int)SubjectTypeEnum.POP订单 || s.SubjectType == (int)SubjectTypeEnum.手工订单));
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

        void BindSubjects()
        {
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            guidanceId = guidanceId == 0 ? -1 : guidanceId;
            var list = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.SubjectType == 1 || s.SubjectType == 5) && s.ApproveState == 1 && (s.IsDelete == false || s.IsDelete == null));
            if (CurrentUser.RoleId == 2)
            {
                list = list.Where(s=>s.AddUserId==CurrentUser.UserId).ToList();
            }
            ddlSubject.DataSource = list;
            ddlSubject.DataTextField = "SubjectName";
            ddlSubject.DataValueField = "Id";
            ddlSubject.DataBind();
            ddlSubject.Items.Insert(0,new ListItem("请选择", "0"));
        }


        //void BindSubjectType(int itemId)
        //{

        //    SubjectGuidance model = new SubjectGuidanceBLL().GetModel(itemId);
        //    if (model != null)
        //    {

        //        var typeList = new SubjectTypeBLL().GetList(s => s.GuidanceId == itemId && (s.IsDelete == false || s.IsDelete == null));
        //        if (typeList.Any())
        //        {
        //            typeList.ForEach(s =>
        //            {
        //                ListItem li = new ListItem();
        //                li.Value = s.Id.ToString();
        //                li.Text = s.SubjectTypeName;
        //                ddlSubjectType.Items.Add(li);
        //            });
        //        }
        //    }

        //}

        //void BindActivityList()
        //{
        //    var activityList = new ADOrderActivityBLL().GetList(s => s.ActivityId > 0);
        //    if (activityList.Any())
        //    {
        //        activityList.ForEach(s =>
        //        {
        //            ListItem li = new ListItem();
        //            li.Value = s.ActivityId.ToString();
        //            li.Text = s.ActivityName;
        //            ddlActivityType.Items.Add(li);
        //        });
        //    }
        //}

        //string CreateSubjectNo()
        //{
        //    System.Text.StringBuilder code = new System.Text.StringBuilder();
        //    DateTime date = DateTime.Now;
        //    code.Append(date.Year).Append(date.Month.ToString().PadLeft(2, '0')).Append(date.Day.ToString().PadLeft(2, '0')).Append(date.Hour.ToString().PadLeft(2, '0')).Append(date.Minute.ToString().PadLeft(2, '0')).Append(date.Second.ToString().PadLeft(2, '0'));
        //    return code.ToString();
        //}

        /// <summary>
        /// 检查项目名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //bool CheckSubject(string name, int id)
        //{
        //    int customerId = int.Parse(ddlCustomer.SelectedValue);
        //    //var list = subjectBll.GetList(s => StringHelper.ReplaceSpace(s.SubjectName) == StringHelper.ReplaceSpace(name) && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1));
        //    name = StringHelper.ReplaceSpace(name);
        //    var list = subjectBll.GetList(s => s.SubjectName.Replace(" ", "").ToLower() == name.ToLower() && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1) && (s.IsDelete == null || s.IsDelete == false));

        //    return list.Any();
        //}

        protected void btnNext_Click(object sender, EventArgs e)
        {
            
            SubjectBLL subjectBll = new SubjectBLL();
            if (subjectId > 0)
            {
                //编辑
                Subject model = subjectBll.GetModel(subjectId);
                if (model != null)
                {
                    int newSubjectId = int.Parse(ddlSubject.SelectedValue);
                    if (newSubjectId > 0)
                    {
                        Subject newModel = subjectBll.GetModel(newSubjectId);
                        if (newModel != null)
                        {
                            
                            model.SubjectName = newModel.SubjectName + "-补单";
                            model.HandMakeSubjectId = newModel.Id;
                            model.Remark = txtRemark.Text.Trim();
                            subjectBll.Update(model);
                            Response.Redirect("ImportOrder.aspx?subjectId=" + subjectId, false);
                        }
                        else
                        {
                            Alert("提交失败");
                        }
                    }
                    else
                    {
                        Alert("请选择项目名称");
                    }
                }
                else
                {
                    Alert("提交失败");
                }
            }
            else
            {
                //新添加
                int newSubjectId = int.Parse(ddlSubject.SelectedValue);
                if (newSubjectId > 0)
                {
                    Subject newModel = subjectBll.GetModel(newSubjectId);
                    if (newModel != null)
                    {
                        Subject model = new Subject();
                        model.SubjectType = (int)SubjectTypeEnum.补单;
                        model.ApproveDate = null;
                        model.ApproveRemark = "";
                        model.ApproveState = 0;
                        model.ApproveUserId = 0;
                        model.Status = 1;
                        model.SubjectName = newModel.SubjectName + "-补单";
                        model.SubjectNo = CreateSubjectNo();
                        model.HandMakeSubjectId = newModel.Id;
                        model.Remark = txtRemark.Text.Trim();
                        model.AddDate = DateTime.Now;
                        model.AddUserId = CurrentUser.UserId;
                        model.IsHandMade = 1;
                        model.ActivityId = newModel.ActivityId;
                        model.AddOrderType = newModel.AddOrderType;
                        model.CompanyId = newModel.CompanyId;
                        model.CustomerId = newModel.CustomerId;
                        model.GuidanceId = newModel.GuidanceId;
                        model.SubjectCategoryId = newModel.SubjectCategoryId;
                        model.SubjectTypeId = newModel.SubjectTypeId;
                        subjectBll.Add(model);
                        Response.Redirect("ImportOrder.aspx?subjectId=" + model.Id, false);
                    }
                    else
                    {
                        Alert("提交失败");
                    }
                }
                else
                {
                    Alert("请选择项目名称");
                }
            }
            
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int itemId = int.Parse((sender as DropDownList).SelectedValue);
            //BindSubjectType(itemId);
            BindSubjects();
        }

        string CreateSubjectNo()
        {
            System.Text.StringBuilder code = new System.Text.StringBuilder();
            DateTime date = DateTime.Now;
            code.Append(date.Year).Append(date.Month.ToString().PadLeft(2, '0')).Append(date.Day.ToString().PadLeft(2, '0')).Append(date.Hour.ToString().PadLeft(2, '0')).Append(date.Minute.ToString().PadLeft(2, '0')).Append(date.Second.ToString().PadLeft(2, '0'));
            return code.ToString();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //string begin = txtBegin.Text.Trim();
            //string end = txtEnd.Text.Trim();
            //if (!string.IsNullOrWhiteSpace(begin) && StringHelper.IsDateTime(begin))
            //{
            //    DateTime beginDate = DateTime.Parse(begin);
            //    var list = new SubjectGuidanceBLL().GetList(s => s.BeginDate >= beginDate);
            //    if (!string.IsNullOrWhiteSpace(end) && StringHelper.IsDateTime(end))
            //    {
            //        DateTime endDate = DateTime.Parse(end).AddDays(1);
            //        list = list.Where(s => s.BeginDate < endDate).ToList();
            //    }
            //    if (CurrentUser.RoleId == 2)
            //    {
            //        var subjectList = new SubjectBLL().GetList(s => s.AddUserId == CurrentUser.UserId && s.GuidanceId != null && s.GuidanceId > 0 && s.ApproveState == 1 && (s.IsDelete == null || s.IsDelete == false) && s.SubjectType == 1);
            //        List<int> guidanceIdList = subjectList.Select(s => s.GuidanceId ?? 0).Distinct().ToList();
            //        list = list.Where(s => guidanceIdList.Contains(s.ItemId)).ToList();
            //    }
            //    if (list.Any())
            //    {
            //        ddlGuidance.DataSource = list.OrderByDescending(s => s.ItemId).ToList();
            //        ddlGuidance.DataTextField = "ItemName";
            //        ddlGuidance.DataValueField = "ItemId";
            //        ddlGuidance.DataBind();
            //        ddlGuidance.Items.Insert(0, new ListItem("请选择", "0"));

            //    }
            //}
            //else
            //{
            //    BindGuidanceList();
            //}
            BindGuidanceList();
            BindSubjects();
        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(PreviousUrl, false);
        }
    }
}
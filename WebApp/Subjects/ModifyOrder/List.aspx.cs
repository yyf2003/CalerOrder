﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using Common;
using Models;
using BLL;

namespace WebApp.Subjects.ModifyOrder
{
    public partial class List :BasePage
    {
        public int subjectId=0;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindSubject();
                //var orderTypeList = CommonMethod.GetEnumList<OrderTypeEnum>().Where(s => !s.Desction.Contains("费用订单")).ToList();
                var orderTypeList = CommonMethod.GetEnumList<OrderTypeEnum>().ToList();
                
                orderTypeList.ForEach(s => {
                    ListItem li = new ListItem();
                    li.Text = s.Name + "&nbsp;";
                    li.Value = s.Value.ToString();
                    rblOrderType.Items.Add(li);
                });
            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                join user in CurrentContext.DbContext.UserInfo
                                on subject.AddUserId equals user.UserId

                                where subject.Id == subjectId
                                select new
                                {
                                    subject,
                                    customer.CustomerName,
                                    AddUserName = user.UserName
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                labSubjectNo.Text = subjectModel.subject.SubjectNo;
                labSubjectName.Text = subjectModel.subject.SubjectName;
                
                labBeginDate.Text = subjectModel.subject.BeginDate != null ? DateTime.Parse(subjectModel.subject.BeginDate.ToString()).ToShortDateString() : "";
                labEndDate.Text = subjectModel.subject.EndDate != null ? DateTime.Parse(subjectModel.subject.EndDate.ToString()).ToShortDateString() : "";
                labAddUserName.Text = subjectModel.AddUserName;
                labCustomerName.Text = subjectModel.CustomerName;
                int subjectType = subjectModel.subject.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                labRemark.Text = subjectModel.subject.Remark;
                if (subjectModel.subject.SubjectType == (int)SubjectTypeEnum.HC订单 || subjectModel.subject.SubjectType == (int)SubjectTypeEnum.分区补单)
                {
                    hfIsRegionSubject.Value = "1";
                }
                List<Subject> subjectList = new SubjectBLL().GetList(s => s.GuidanceId == subjectModel.subject.GuidanceId && s.ApproveState == 1 && (s.IsDelete == null || s.IsDelete == false) && (s.SubjectType == (int)SubjectTypeEnum.POP订单 || s.SubjectType == (int)SubjectTypeEnum.手工订单)).OrderByDescending(s=>s.AddDate).OrderBy(s=>s.SubjectName).ToList();
                if (subjectList.Any())
                {
                    subjectList.ForEach(s => {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectName;
                        ddlSubjectList.Items.Add(li);
                    });
                }
            }
        }
    }
}
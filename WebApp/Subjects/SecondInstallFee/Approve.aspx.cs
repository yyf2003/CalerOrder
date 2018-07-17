﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Transactions;
using Common;

namespace WebApp.Subjects.SecondInstallFee
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
                BindSubject();
                BindData();
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
                                    AddUserName = user.RealName
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                labSubjectNo.Text = subjectModel.subject.SubjectNo;
                labSubjectName.Text = subjectModel.subject.SubjectName;

                //labBeginDate.Text = subjectModel.subject.BeginDate != null ? DateTime.Parse(subjectModel.subject.BeginDate.ToString()).ToShortDateString() : "";
                //labEndDate.Text = subjectModel.subject.EndDate != null ? DateTime.Parse(subjectModel.subject.EndDate.ToString()).ToShortDateString() : "";
                //labContact.Text = subjectModel.subject.Contact;
                //labTel.Text = subjectModel.subject.Tel;
                labCustomerName.Text = subjectModel.CustomerName;
                labPrice.Text = subjectModel.subject.SecondInstallPrice != null ? subjectModel.subject.SecondInstallPrice.ToString() : "";
                labRemark.Text = subjectModel.subject.Remark;
                labRegion.Text = subjectModel.subject.Region;
                labAddUserName.Text = subjectModel.AddUserName;
            }
        }

        void BindData()
        {
            var list = (from detail in CurrentContext.DbContext.SecondInstallFeeDetail
                        join shop in CurrentContext.DbContext.Shop
                        on detail.ShopId equals shop.Id
                        where detail.SubjectId == subjectId
                        select new
                        {
                            detail,
                            shop
                        }).ToList();

            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            orderList.DataSource = list.OrderBy(s => s.detail.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            orderList.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text.Trim();
            SubjectBLL subjectBll = new SubjectBLL();
            bool isApproveOk = false;
            int subjectType=1;
            int guidanceId = 0;
            string msg = string.Empty;
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    Models.Subject model = subjectBll.GetModel(subjectId);
                    if (model != null)
                    {
                        guidanceId = model.GuidanceId ?? 0;
                        subjectType = model.SubjectType ?? 1;
                        model.ApproveState = result;
                        model.ApproveUserId = CurrentUser.UserId;
                        subjectBll.Update(model);
                        ApproveInfo approveModel = new ApproveInfo() { AddDate = DateTime.Now, AddUserId = CurrentUser.UserId, Remark = txtRemark.Text.Trim(), Result = result, SubjectId = subjectId };
                        new ApproveInfoBLL().Add(approveModel);

                        tran.Complete();
                        isApproveOk = true;
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            if (isApproveOk)
            {
                if (result == 1 && subjectType != (int)SubjectTypeEnum.新开店安装费 && subjectType != (int)SubjectTypeEnum.运费)
                {
                    new WebApp.Base.DelegateClass().SaveOutsourceOrder(guidanceId, subjectId);
                }
                Alert("审批成功！", "/Subjects/ApproveList.aspx");
            }
            else
                Alert("审批失败：" + msg);
        }
    }
}
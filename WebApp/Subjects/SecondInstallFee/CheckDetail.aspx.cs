﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Text;

namespace WebApp.Subjects.SecondInstallFee
{
    public partial class CheckDetail : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["isCheck"] != null)
            {
                Panel1.Visible = false;
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindData();
                GetApproveInfo();
                
            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                join user1 in CurrentContext.DbContext.UserInfo
                                on subject.ApproveUserId equals user1.UserId into userTemp
                                from user in userTemp.DefaultIfEmpty()
                                where subject.Id == subjectId
                                select new
                                {
                                    subject,
                                    customer.CustomerName,
                                    ApproveUserName = user != null ? user.UserName : ""
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                labSubjectNo.Text = subjectModel.subject.SubjectNo;
                labSubjectName.Text = subjectModel.subject.SubjectName;

                labBeginDate.Text = subjectModel.subject.BeginDate != null ? DateTime.Parse(subjectModel.subject.BeginDate.ToString()).ToShortDateString() : "";
                labEndDate.Text = subjectModel.subject.EndDate != null ? DateTime.Parse(subjectModel.subject.EndDate.ToString()).ToShortDateString() : "";
                labContact.Text = subjectModel.subject.Contact;
                labTel.Text = subjectModel.subject.Tel;
                labCustomerName.Text = subjectModel.CustomerName;
                labPrice.Text =subjectModel.subject.SecondInstallPrice!=null ? subjectModel.subject.SecondInstallPrice.ToString():"";
                labRemark.Text = subjectModel.subject.Remark;
                labRegion.Text = subjectModel.subject.Region;
                if (Request.QueryString["fromReject"] != null && subjectModel.subject.ApproveState == 2)
                {
                    btnEdit.Visible = true;
                    btnDelete.Visible = true;
                }
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

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            string url = string.Format("/Subjects/SecondInstallFee/Add.aspx?subjectId={0}", subjectId);
            Response.Redirect(url, false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SubjectBLL bll=new SubjectBLL();
            Subject model = bll.GetModel(subjectId);
            if (model != null)
            {
                model.IsDelete = true;
                model.DeleteDate = DateTime.Now;
                bll.Update(model);
                Alert("删除成功！", "/Subjects/RejectSubjectList.aspx");
            }
        }
    }
}
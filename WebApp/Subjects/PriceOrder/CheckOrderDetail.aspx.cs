﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;
using System.Text;

namespace WebApp.Subjects.PriceOrder
{
    public partial class CheckOrderDetail : BasePage
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
                BindOrderData();
                GetApproveInfo();
            }

        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                where subject.Id == subjectId
                                select new
                                {
                                    subject.SubjectName,
                                    subject.SubjectNo,
                                    customer.CustomerName,
                                    subject.CustomerId,
                                    subject.SubjectType,
                                    subject.GuidanceId
                                }).FirstOrDefault();
            if (subjectModel != null)
            {

                labSubjectName.Text = subjectModel.SubjectName;
                labSubjectNo.Text = subjectModel.SubjectNo;
                labCustomer.Text = subjectModel.CustomerName;
                int subjectType = subjectModel.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());

            }
        }

        void BindOrderData()
        {
            var orderList = new PriceOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
            AspNetPager1.RecordCount = orderList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            orderListRepeater.DataSource = orderList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            orderListRepeater.DataBind();

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

        protected void orderListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                PriceOrderDetail model = (PriceOrderDetail)e.Item.DataItem;
                if (model != null)
                {
                    string orderType = (model.OrderType ?? 0).ToString();
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);
                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindOrderData();
        }
    }
}
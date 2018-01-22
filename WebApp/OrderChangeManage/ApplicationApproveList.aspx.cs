﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;


namespace WebApp.OrderChangeManage
{
    public partial class ApplicationApproveList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                labApproveState.Text = "—未审批";
                BindData();
            }
        }

        void BindData()
        {
            int approveState = int.Parse(rblApproveState.SelectedValue);
            List<int> companyList = MySonCompanyList.Select(s => s.Id).ToList();
            var list = (from application in CurrentContext.DbContext.OrderChangeApplication
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on application.SubjectGuidanceId equals guidance.ItemId
                        join user in CurrentContext.DbContext.UserInfo
                        on application.AddUserId equals user.UserId
                        where (application.IsDelete == null || application.IsDelete == false)
                        select new
                        {
                            application.Id,
                            application.AddDate,
                            user.RealName,
                            user.CompanyId,
                            guidance.ItemName,
                            application.AddUserId,
                            application.FinancialApproveState,
                            application.ManagerApperoveState,
                            application.ManagerApperoveUID

                        }).ToList();
            if (CurrentUser.RoleId == 3 || CurrentUser.RoleId == 6)
            {
                //客服经理或区域经理
                
                if (approveState == 1)
                {
                    //已完成审批
                    list = list.Where(s => s.ManagerApperoveState != null && s.ManagerApperoveState > 0 && s.ManagerApperoveUID==CurrentUser.UserId).ToList();
                }
                else
                    list = list.Where(s => companyList.Contains(s.CompanyId ?? 0) && (s.ManagerApperoveState == null || s.ManagerApperoveState == 0)).ToList();
            }
            else
            {
                //财务
                
                if (approveState == 1)
                {
                    //已完成审批
                    list = list.Where(s => s.FinancialApproveState != null && s.FinancialApproveState > 0).ToList();
                }
                else
                    list = list.Where(s => (s.FinancialApproveState == null || s.FinancialApproveState == 0)).ToList();
            }
            if (approveState == 0)
            {
                labApproveState.Text = "—未审批";
            }
            else
                labApproveState.Text = "—已审批";

            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.AddDate).ThenByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
           
            gv.DataBind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "ApproveItem")
            {
                Response.Redirect("ApplicationApprove.aspx?Id="+id,false);
            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object approveStateObj = item.GetType().GetProperty("ManagerApperoveState").GetValue(item,null);
                    int approveState = approveStateObj != null ? int.Parse(approveStateObj.ToString()) : 0;
                    Label labApproveState = (Label)e.Row.FindControl("labApproveState");
                    switch (approveState)
                    { 
                        case 0:
                            labApproveState.Text = "未审批";
                            //labApproveState.ForeColor = System.Drawing.Color.Red;
                            break;
                        case 1:
                            labApproveState.Text = "审批通过";
                            labApproveState.ForeColor = System.Drawing.Color.Green;
                            break;
                        case 2:
                            labApproveState.Text = "审批不通过";
                            labApproveState.ForeColor = System.Drawing.Color.Red;
                            break;
                    }
                }
            }
        }
    }
}
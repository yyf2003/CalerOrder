using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;

namespace WebApp.Subjects.ADErrorCorrection
{
    public partial class SubjectList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            List<int> curstomerList = GetMyCustomerList().Select(s=>s.Id).ToList();
            
            
            //var list = (from subject in CurrentContext.DbContext.Subject
            //            join customer in CurrentContext.DbContext.Customer
            //            on subject.CustomerId equals customer.Id
            //            join user in CurrentContext.DbContext.UserInfo
            //            on subject.AddUserId equals user.UserId
            //            where (subject.IsDelete == null || subject.IsDelete == false) && subject.Status == 4 && curstomerList.Contains(subject.CustomerId??0)
            //            select new
            //            {
            //                subject.Id,
            //                subject.AddDate,
            //                subject.AddUserId,
            //                subject.ApproveState,
            //                subject.ApproveUserId,
            //                subject.BeginDate,
            //                subject.Contact,
            //                subject.EndDate,
            //                subject.IsDelete,
            //                subject.OutSubjectName,
            //                subject.Remark,
            //                subject.Status,
            //                subject.SubjectName,
            //                subject.SubjectNo,
            //                subject.Tel,
            //                subject.CustomerId,
            //                customer.CustomerName,
            //                user.RealName
            //            }).ToList();
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                       join company in CurrentContext.DbContext.Company
                       on guidance.CustomerId equals company.Id
                       join user in CurrentContext.DbContext.UserInfo
                       on guidance.AddUserId equals user.UserId
                       select new {
                           guidance,
                           CustomerName = company.CompanyName,
                           AddUserName=user.RealName
                       }).ToList();

            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.guidance.ItemId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();

            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Check")
            {
                int itemId = int.Parse(e.CommandArgument.ToString());
                Response.Redirect("ErrorCorrection.aspx?itemId=" + itemId, false);
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;


namespace WebApp.Subjects.Supplement
{
    public partial class ApproveList : BasePage
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
            List<int> customerIds=CurrentUser.Customers.Select(u => u.CustomerId).ToList();
            List<int> companyList = MySonCompanyList.Select(s => s.Id).ToList();
            int approveState = int.Parse(rblApproveState.SelectedValue);
            var list = (from supplement in CurrentContext.DbContext.SupplementItem
                       join subject in CurrentContext.DbContext.Subject
                       on supplement.SubjectId equals subject.Id
                       join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on supplement.AddUserId equals user.UserId
                        where (supplement.IsSubmit != null && supplement.IsSubmit==1) &&  (approveState == 0 ? (supplement.ApproveState == 0 || supplement.ApproveState == null) : supplement.ApproveState > 0)
                       select new {
                           supplement,
                           subject.SubjectName,
                           subject.SubjectNo,
                           customer.CustomerName,
                           AddUserName=user.RealName
                       }).ToList();
            if (!string.IsNullOrWhiteSpace(tbSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.Contains(tbSubjectName.Text.Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(tbSubjectNo.Text))
            {
                list = list.Where(s => s.SubjectNo == tbSubjectNo.Text.Trim()).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.supplement.ApproveState).ThenByDescending(s => s.supplement.SupplementId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            Button btn = new Button();
            SetPromission(gv, new object[] { btn });
            if (approveState == 0)
            {
                gv.Columns[gv.Columns.Count - 2].Visible = false;
                gv.Columns[gv.Columns.Count - 1].Visible = true;
            }
            else
            {
                gv.Columns[gv.Columns.Count - 2].Visible = true;
                gv.Columns[gv.Columns.Count - 1].Visible = false;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button2_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "Approve")
            {
                Response.Redirect(string.Format("Approve.aspx?supplementId={0}", id), false);
            }
            if (e.CommandName == "Check")
            {
                Response.Redirect(string.Format("CheckDetail.aspx?check=1&supplementId={0}", id), false);
            }
        }
    }
}
using System;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;

namespace WebApp.Subjects.Supplement
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                BindCustomerList(ref ddlAddCustomer);
                BindData();
            }
        }

        void BindData()
        {
            var list = (from item in CurrentContext.DbContext.SupplementItem
                        join subject in CurrentContext.DbContext.Subject
                        on item.SubjectId equals subject.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on item.AddUserId equals user.UserId
                        select new
                        {
                            item.BeginDate,
                            item.EndDate,
                            item.IsDelete,
                            item.SupplementId,
                            item.SubjectId,
                            item.Price,
                            item.Reason,
                            item.AddDate,
                            IsSubmit=item.IsSubmit??0,
                            ApproveState=item.ApproveState??0,
                            item.ApproveDate,
                            user.RealName,
                            subject.SubjectName,
                            subject.CustomerId
                        }).ToList();
            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.Contains(txtSubjectName.Text)).ToList();
            }
            
            AspNetPager1.RecordCount = list.Count;
            gv.DataSource = list.OrderByDescending(s => s.SupplementId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }


        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            BindData();
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object approveStateObj = item.GetType().GetProperty("ApproveState").GetValue(item, null);
                    int approveState = approveStateObj != null ? int.Parse(approveStateObj.ToString()) : 0;

                    object IsSubmitObj = item.GetType().GetProperty("IsSubmit").GetValue(item, null);
                    int IsSubmit = approveStateObj != null ? int.Parse(approveStateObj.ToString()) : 0;

                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");
                    //IsSubmit==2表示删除
                    if (approveState == 1 || IsSubmit==2)
                    {
                        Label labedit = (Label)e.Row.FindControl("labEdit");
                        labedit.Text = "编辑";
                        labedit.ForeColor = System.Drawing.Color.Gray;
                        Label labEditOrder = (Label)e.Row.FindControl("labEditOrder");
                        labEditOrder.Text = "添加订单";
                        labEditOrder.ForeColor = System.Drawing.Color.Gray;
                        
                        lbDelete.Enabled = false;
                        lbDelete.CommandName = "";
                        lbDelete.Style.Add("color", "#ccc");
                    }
                    else
                    {
                        lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                    }
                }
            }
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "DeleteItem")
            {
                SupplementItem model = new SupplementItemBLL().GetModel(id);
                model.IsSubmit = 2;
                new SupplementItemBLL().Update(model);
                BindData();
            }
        }
    }
}
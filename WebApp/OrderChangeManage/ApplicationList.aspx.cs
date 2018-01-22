using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;

namespace WebApp.OrderChangeManage
{
    public partial class ApplicationList : BasePage
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
            var list = (from application in CurrentContext.DbContext.OrderChangeApplication
                       join guidance in CurrentContext.DbContext.SubjectGuidance
                       on application.SubjectGuidanceId equals guidance.ItemId
                       join user in CurrentContext.DbContext.UserInfo
                       on application.AddUserId equals user.UserId
                       select new {
                           application.Id,
                           application.AddDate,
                           user.RealName,
                           guidance.ItemName,
                           application.AddUserId,
                           application.FinancialApproveState,
                           application.ManagerApperoveState,
                           application.IsDelete
                       }).ToList();
            //if (CurrentUser.RoleId == 2)
            //{
                
            //}
            list = list.Where(s => s.AddUserId == CurrentUser.UserId).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.AddDate).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv);
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "EditItem")
            {
                Response.Redirect("AddNewApplication.aspx?Id="+id,false);
            }
            if (e.CommandName == "Check")
            {
                Response.Redirect("CheckApplicationDetail.aspx?Id=" + id, false);
            }
            if (e.CommandName == "DeleteItem")
            {
                OrderChangeApplicationBLL bll = new OrderChangeApplicationBLL();
                OrderChangeApplication model = bll.GetModel(id);
                if (model != null)
                {
                    model.IsDelete = true;
                    model.DeleteUserId = CurrentUser.UserId;
                    model.DeleteDate = DateTime.Now;
                    bll.Update(model);
                    BindData();
                }
            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    bool isPassApprove = false;
                    object managerApperoveStateObj = item.GetType().GetProperty("ManagerApperoveState").GetValue(item, null);
                    int managerApperoveState = managerApperoveStateObj != null ? int.Parse(managerApperoveStateObj.ToString()) : 0;
                    //object financialApproveStateObj = item.GetType().GetProperty("FinancialApproveState").GetValue(item, null);
                    //int financialApproveState = financialApproveStateObj != null ? int.Parse(financialApproveStateObj.ToString()) : 0;
                    object isDeleteObj = item.GetType().GetProperty("IsDelete").GetValue(item, null);
                    bool isDelete = isDeleteObj != null ? bool.Parse(isDeleteObj.ToString()) : false;

                    isPassApprove = managerApperoveState == 1;
                    LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");

                    Label labState = (Label)e.Row.FindControl("labState");
                    Label labApproveState = (Label)e.Row.FindControl("labApproveState");

                    switch (managerApperoveState)
                    {
                        case 0:
                            labApproveState.Text = "未审批";
                            //labApproveState.ForeColor = System.Drawing.Color.Green;
                            if (!isDelete)
                               lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");

                            break;
                        case 1:
                            labApproveState.Text = "审批通过";
                            labApproveState.ForeColor = System.Drawing.Color.Green;

                            lbEdit.CommandName = "";
                            lbEdit.Enabled = false;
                            lbEdit.Style.Add("color", "#ccc");

                            lbDelete.Enabled = false;
                            lbDelete.CommandName = "";
                            lbDelete.Style.Add("color", "#ccc");

                            break;
                        case 2:
                            labApproveState.Text = "审批不通过";
                            labApproveState.ForeColor = System.Drawing.Color.Red;
                            if (!isDelete)
                               lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                            break;
                        default:
                            break;
                    }


                    if (!isDelete)
                    {
                        labState.Text = "正常";
                    }
                    else
                    {
                        labState.Text = "已删除";
                        labState.ForeColor = System.Drawing.Color.Red;

                        lbEdit.CommandName = "";
                        lbEdit.Enabled = false;
                        lbEdit.Style.Add("color", "#ccc");

                        lbDelete.Enabled = false;
                        lbDelete.CommandName = "";
                        lbDelete.Style.Add("color", "#ccc");
                    }
                }
            }
        }
    }
}
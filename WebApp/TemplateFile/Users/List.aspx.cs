using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Drawing;

namespace WebApp.Users
{
    public partial class List : BasePage
    {
        UserBLL userBll = new UserBLL();
        UserInfo userModel;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
                GetRoles();
            }
        }

        void BindData()
        {
            var list = (from user in CurrentContext.DbContext.UserInfo
                       join company1 in CurrentContext.DbContext.Company
                       on user.CompanyId equals company1.Id into companyTemp
                       from company in companyTemp.DefaultIfEmpty()
                       select new {
                           user.UserId,
                           user.UserName,
                           user.AddDate,
                           user.CompanyId,
                           user.IsDelete,
                           user.RealName,
                           
                           company.CompanyName
                       }).ToList();

            if (!string.IsNullOrEmpty(txtRealName1.Text))
            {
                list = list.Where(s => s.RealName.Contains(txtRealName1.Text)).ToList();
            }
            if (!string.IsNullOrEmpty(txtUserName1.Text))
            {
                list = list.Where(s => s.UserName.Contains(txtUserName1.Text)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            gv.DataSource = list.OrderByDescending(s => s.UserId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
        }



        void GetRoles()
        {
            hfRoles.Value = "";
            RoleBLL roleBll = new RoleBLL();
            StringBuilder json = new StringBuilder();
            var list = roleBll.GetList(s => s.RoleId > 0);
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    json.Append("{\"RoleId\":\"" + s.RoleId + "\",\"RoleName\":\"" + s.RoleName + "\"},");
                });
                hfRoles.Value = "[" + json.ToString().TrimEnd(',') + "]";
            }
        }

        

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                object userModel = e.Row.DataItem;
                if (userModel != null)
                {
                    object userIdObj = userModel.GetType().GetProperty("UserId").GetValue(userModel,null);
                    int userId = userIdObj != null ? int.Parse(userIdObj.ToString()) : 0;
                    var roles = from ur in CurrentContext.DbContext.UserInRole
                                join role in CurrentContext.DbContext.Role
                                on ur.RoleId equals role.RoleId
                                where ur.UserId == userId
                                select role;
                    if (roles.Any())
                    {
                        string roleNames = string.Empty;
                        string roleIds = string.Empty;
                        roles.ToList().ForEach(s =>
                        {
                            roleNames += s.RoleName + "/";
                            roleIds += s.RoleId + ",";
                        });
                        ((Label)e.Row.FindControl("labRoles")).Text = roleNames.TrimEnd('/');
                        ((HiddenField)e.Row.FindControl("hfRoleIds")).Value = roleIds.TrimEnd(',');
                    }


                    var customers = from userInCustomer in CurrentContext.DbContext.UserInCustomer
                                    join customer in CurrentContext.DbContext.Customer
                                    on userInCustomer.CustomerId equals customer.Id
                                    where userInCustomer.UserId == userId
                                    select customer;
                    if (customers.Any())
                    {
                        
                        StringBuilder customerNames = new StringBuilder();
                        StringBuilder customerIds = new StringBuilder();
                        customers.ToList().ForEach(s => {
                            customerIds.Append(s.Id + ",");
                            customerNames.Append(s.CustomerShortName + "/");
                        });
                        ((Label)e.Row.FindControl("labCustomers")).Text = customerNames.ToString().TrimEnd('/');
                        ((HiddenField)e.Row.FindControl("hfCustomerIds")).Value = customerIds.ToString().TrimEnd(',');
                    }

                    var activies = from userInActivity in CurrentContext.DbContext.UserInActivity
                                   join activity in CurrentContext.DbContext.ADOrderActivity
                                   on userInActivity.ActivityId equals activity.ActivityId
                                   where userInActivity.UserId == userId
                                   select activity;
                    if (activies.Any())
                    {
                        StringBuilder activityNames = new StringBuilder();
                        StringBuilder activityIds = new StringBuilder();
                        activies.ToList().ForEach(s =>
                        {
                            activityIds.Append(s.ActivityId + ",");
                            activityNames.Append(s.ActivityName + "/");
                        });
                        ((Label)e.Row.FindControl("labActivies")).Text = activityNames.ToString().TrimEnd('/');
                        ((HiddenField)e.Row.FindControl("hfAcivityIds")).Value = activityIds.ToString().TrimEnd(',');
                    }



                    object isDeleteObj = userModel.GetType().GetProperty("IsDelete").GetValue(userModel, null);
                    bool isDelete = isDeleteObj != null ? bool.Parse(isDeleteObj.ToString()) : false;
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");
                    if (isDelete)
                    {
                        lbDelete.Text = "恢复";
                        lbDelete.Attributes.Add("OnClick", "return confirm('确定恢复吗？')");
                    }
                    else
                    {
                        lbDelete.Text = "删除";
                        lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                    }
                    
                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int userId = int.Parse(e.CommandArgument.ToString());
            userModel = userBll.GetModel(userId);
            if (userModel != null)
            { 
                bool isDelete=userModel.IsDelete??false;
                userModel.IsDelete = !isDelete;
                userBll.Update(userModel);
                BindData();
            }
        }
    }
}
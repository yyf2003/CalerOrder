using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using DAL;

namespace WebApp.Users
{
    public partial class Edit : BasePage
    {
        UserBLL userBll = new UserBLL();
        UserInfo userModel;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            //userModel = userBll.GetModel(CurrentUser.UserId);
            var userModel1 = (from user in CurrentContext.DbContext.UserInfo
                        join company1 in CurrentContext.DbContext.Company
                        on user.CompanyId equals company1.Id into companyTemp
                        from company in companyTemp.DefaultIfEmpty()
                        where user.UserId == CurrentUser.UserId
                        select new { 
                           user,
                           CompanyName = company!=null?company.CompanyName:""
                        }).FirstOrDefault();
            if (userModel1 != null)
            {
                txtRealName.Text = userModel1.user.RealName;
                txtUserName.Text = userModel1.user.UserName;
                labCompany.Text = userModel1.CompanyName;
                var roleList=from ur in CurrentContext.DbContext.UserInRole
                             join role in CurrentContext.DbContext.Role
                             on ur.RoleId equals role.RoleId
                             where ur.UserId==userModel1.user.UserId
                             select role;
                if (roleList.Any())
                {
                    System.Text.StringBuilder roles = new System.Text.StringBuilder();
                    roleList.ToList().ForEach(s => {
                        roles.Append(s.RoleName);
                        roles.Append("，");
                    });
                    labRole.Text = roles.ToString().TrimEnd('，');
                }
                var customerList = from uc in CurrentContext.DbContext.UserInCustomer
                                   join customer in CurrentContext.DbContext.Customer
                                   on uc.CustomerId equals customer.Id
                                   where uc.UserId == userModel1.user.UserId
                                   select customer;
                if (customerList.Any())
                {
                    System.Text.StringBuilder customers = new System.Text.StringBuilder();
                    customerList.ToList().ForEach(s =>
                    {
                        customers.Append(s.CustomerName);
                        customers.Append("，");
                    });
                    labCustomers.Text = customers.ToString().TrimEnd('，');
                }
            }
        }

        protected void btnSubmit1_Click(object sender, EventArgs e)
        {
            if (!userBll.CheckExist(txtUserName.Text.Trim(), CurrentUser.UserId))
            {
                userModel = userBll.GetModel(CurrentUser.UserId);
                if (userModel != null)
                {
                    userModel.RealName=txtRealName.Text;
                    userModel.UserName=txtUserName.Text;
                    userBll.Update(userModel);
                    ClientScript.RegisterStartupScript(GetType(), "s", "alert('修改成功')", true);
                }

            }
            else
            {
                RequiredFieldValidator2.ErrorMessage = "登陆账号重复";
                RequiredFieldValidator2.IsValid = false;
                return;
            }
            
        }

        protected void btnSubmit2_Click(object sender, EventArgs e)
        {
            string oldPsw = txtOldPsw.Text.Trim();
            string newPsw = txtNewPsw.Text.Trim();
            userModel = userBll.GetModel(CurrentUser.UserId);
            if (userModel != null)
            {
                if (userModel.PassWord == oldPsw)
                {
                    userModel.PassWord = newPsw;
                    userBll.Update(userModel);
                    ClientScript.RegisterStartupScript(GetType(), "s", "alert('密码修改成功');window.location.href='Edit.aspx'", true);
                }
                else
                {
                    RequiredFieldValidator3.ErrorMessage = "原始密码不正确";
                    RequiredFieldValidator3.IsValid = false;
                    return;
                }


            }
            else
                ClientScript.RegisterStartupScript(GetType(), "s", "alert('密码修改失败')", true);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using BLL;
using DAL;
using Models;
using Common;
using System.Web.Security;
using Newtonsoft.Json;

namespace WebApp
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            UserBLL userbll = new UserBLL();
            var list = userbll.GetList(s => s.UserName == txtLoginName.Text && s.PassWord == txtPassword.Text && (s.IsDelete==null || s.IsDelete==false));
            if (list.Any())
            {
                UserInfo user1 = list.First();
                Session["User"] = user1;
                int roleId = 0;
                if (CheckRole(user1.UserId, out roleId)==true)
                {
                    GoLogin(roleId);
                }
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "alert", "LoginFail()", true);
            }
        }

        bool CheckRole(int userId,out int roleId)
        {
            roleId = 0;
            var roleList = (from userInRole in CurrentContext.DbContext.UserInRole
                            join role in CurrentContext.DbContext.Role
                            on userInRole.RoleId equals role.RoleId
                           where userInRole.UserId == userId
                            select role).ToList();
            if (roleList.Count > 1)
            {
                txtLoginName.ReadOnly = true;
                txtPassword.ReadOnly = true;
                ddlRoles.DataSource = roleList;
                ddlRoles.DataTextField = "RoleName";
                ddlRoles.DataValueField = "RoleId";
                ddlRoles.DataBind();
                ddlRoles.Items.Insert(0, new ListItem("--请选择角色--", "0"));
                Panel1.Visible = true;
                Panel2.Visible = false;
                return false;
            }
            else
            {
                roleId = roleList[0].RoleId;;
                return true;
            }
        }


        /// <summary>
        /// 多角色登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLoginWithRole_Click(object sender, EventArgs e)
        {
            int roleId = int.Parse(ddlRoles.SelectedValue);
            if (roleId == 0)
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "alert", "alert('请选择角色！');", true);
            }
            else
            {
                GoLogin(roleId);
            }
        }


        void GoLogin(int roleId)
        {
            UserInfo user1 = Session["User"] as UserInfo;
            if (user1 != null)
            {
                LoginUser loginUserModel = new LoginUser();
                loginUserModel.LoginName = txtLoginName.Text;
                loginUserModel.UserId = user1.UserId;
                loginUserModel.UserName = user1.RealName;
                loginUserModel.UserLevelId = user1.UserLevelId ?? 1;
                if (user1.CompanyId != null)
                {
                    //所属公司
                    Company company = new CompanyBLL().GetModel(user1.CompanyId ?? 0);
                    if (company != null)
                    {
                        loginUserModel.CompanyName = company.CompanyName;
                        loginUserModel.BaseCode = company.BaseCode;
                    }

                }
                loginUserModel.CompanyId = user1.CompanyId ?? 0;
                using (KalerOrderDBEntities dc = new KalerOrderDBEntities())
                {
                    Models.Role roleModel = new RoleBLL().GetModel(roleId);
                    if (roleModel != null)
                    {
                        //暂时是取第一个
                        loginUserModel.RoleId = roleModel.RoleId;
                        loginUserModel.RoleName = roleModel.RoleName;
                    }
                    var list1 = from userInCustomer in dc.UserInCustomer
                                join customer in dc.Customer
                                on userInCustomer.CustomerId equals customer.Id
                                where userInCustomer.UserId == user1.UserId
                                select customer;
                    if (list1.Any())
                    {

                        List<UserCustomer> customers = new List<UserCustomer>();
                        list1.ToList().ForEach(s =>
                        {
                            UserCustomer customer = new UserCustomer();
                            customer.CustomerId = s.Id;
                            customer.CustomerName = s.CustomerName;
                            customers.Add(customer);
                        });
                        loginUserModel.Customers = customers;
                    }
                }


                string userData = JsonConvert.SerializeObject(loginUserModel);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1, txtLoginName.Text, DateTime.Now, DateTime.Now.AddMinutes(60), true, userData
                    );
                string encTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                cookie.Expires = DateTime.Now.AddMinutes(60);
                Response.Cookies.Add(cookie);
                Session["User"] = null;
                Response.Redirect("/main.html");
            }
            
        }



        protected void btnCancel_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session["User"] = null;
            Response.Redirect("login.htm", false);
        }
    }
}
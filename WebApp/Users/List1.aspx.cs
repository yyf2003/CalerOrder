using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Drawing;
using Common;

namespace WebApp.Users
{
    public partial class List1 : BasePage
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
                        join level in CurrentContext.DbContext.UserLevel
                         on user.UserLevelId equals level.LevelId into levelTemp
                        from userLevel in levelTemp.DefaultIfEmpty()
                       select new {
                           user.UserId,
                           user.UserName,
                           user.AddDate,
                           user.CompanyId,
                           user.IsDelete,
                           user.RealName,
                           user.UserLevelId,
                           userLevel.LevelName,
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
            List<int> roleIdList = new List<int>();
            foreach (ListItem li in cblRole.Items)
            {
                if (li.Selected)
                    roleIdList.Add(int.Parse(li.Value));
            }
            if (roleIdList.Any())
            {
                List<int> userList = new UserInRoleBLL().GetList(s => roleIdList.Contains(s.RoleId??0)).Select(s=>s.UserId??0).Distinct().ToList();
                list = list.Where(s => userList.Contains(s.UserId)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            gv.DataSource = list.OrderByDescending(s => s.UserId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd});
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
                    ListItem li = new ListItem();
                    li.Value = s.RoleId.ToString();
                    li.Text = s.RoleName + "&nbsp;&nbsp;";
                    cblRole.Items.Add(li);
                    json.Append("{\"RoleId\":\"" + s.RoleId + "\",\"RoleName\":\"" + s.RoleName + "\"},");
                });
                hfRoles.Value = "[" + json.ToString().TrimEnd(',') + "]";
            }
        }

        

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        UserInRegionBLL userRegionBll = new UserInRegionBLL();
        RegionBLL regionBll = new RegionBLL();
        PlaceBLL placeBll = new PlaceBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                object userModel = e.Row.DataItem;
                if (userModel != null)
                {
                    object userIdObj = userModel.GetType().GetProperty("UserId").GetValue(userModel,null);
                    //object userLevelIdObj = userModel.GetType().GetProperty("UserLevelId").GetValue(userModel, null);

                    int userId = userIdObj != null ? int.Parse(userIdObj.ToString()) : 0;
                    //int userLevelId = userLevelIdObj != null ? int.Parse(userLevelIdObj.ToString()) : 0;
                    var roles = from ur in CurrentContext.DbContext.UserInRole
                                join role in CurrentContext.DbContext.Role
                                on ur.RoleId equals role.RoleId
                                where ur.UserId == userId
                                select role;
                    if (roles.Any())
                    {
                        string roleNames = string.Empty;
                        //string roleIds = string.Empty;
                        roles.ToList().ForEach(s =>
                        {
                            roleNames += s.RoleName + "/";
                            //roleIds += s.RoleId + ",";
                        });
                        ((Label)e.Row.FindControl("labRoles")).Text = roleNames.TrimEnd('/');
                        //((HiddenField)e.Row.FindControl("hfRoleIds")).Value = roleIds.TrimEnd(',');
                    }


                    var customers = from userInCustomer in CurrentContext.DbContext.UserInCustomer
                                    join customer in CurrentContext.DbContext.Customer
                                    on userInCustomer.CustomerId equals customer.Id
                                    where userInCustomer.UserId == userId
                                    select customer;
                    if (customers.Any())
                    {
                        
                        StringBuilder customerNames = new StringBuilder();
                        //StringBuilder customerIds = new StringBuilder();
                        customers.ToList().ForEach(s => {
                            //customerIds.Append(s.Id + ",");
                            customerNames.Append(s.CustomerShortName + "/");
                        });
                        ((Label)e.Row.FindControl("labCustomers")).Text = customerNames.ToString().TrimEnd('/');
                        //((HiddenField)e.Row.FindControl("hfCustomerIds")).Value = customerIds.ToString().TrimEnd(',');
                    }


                    var userRegionModel = userRegionBll.GetList(s => s.UserId == userId).FirstOrDefault();

                    if (userRegionModel != null && !string.IsNullOrWhiteSpace(userRegionModel.RegionId))
                    {
                        List<int> regionIds = StringHelper.ToIntList(userRegionModel.RegionId,',');
                        StringBuilder regionNames = new StringBuilder();
                        var regions = regionBll.GetList(r => regionIds.Contains(r.Id));
                        regions.ForEach(r => {
                            regionNames.Append(r.RegionName + "/");
                        });
                        ((Label)e.Row.FindControl("labRegions")).Text = regionNames.ToString().TrimEnd('/');

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
            if (e.CommandName == "DeleteUser")
            {
                if (userModel != null)
                {
                    bool isDelete = userModel.IsDelete ?? false;
                    userModel.IsDelete = !isDelete;
                    userBll.Update(userModel);
                    BindData();
                }
            }
            if (e.CommandName == "ReSetPassord")
            {
                if (userModel != null)
                {
                    userModel.PassWord = "1";
                    userBll.Update(userModel);
                    Alert("重置成功，当前密码为：1");
                }
            }
        }
    }
}
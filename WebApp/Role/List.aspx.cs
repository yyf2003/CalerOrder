using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using BLL;
using Models;

namespace WebApp.Role
{
    public partial class List : BasePage
    {
        RoleBLL roleBll = new RoleBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {

            List<Models.Role> list = roleBll.GetList(s => s.RoleId > 0);
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                list = list.Where(s => s.RoleName.Contains(txtName.Text)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            gv.DataSource = list.OrderBy(s => s.RoleId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int roleId = int.Parse(e.CommandArgument.ToString());
            Models.Role model = roleBll.GetModel(roleId);
            if (model != null)
            {
                roleBll.Delete(model);
                BindData();
            }
        }
    }
}
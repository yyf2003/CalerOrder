using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;

namespace WebApp.Customer
{
    public partial class CustomerList : BasePage
    {
        CustomerBLL customerBll = new CustomerBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            var list = customerBll.GetList(s=>1==1);
            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                list = list.Where(s => s.CustomerName.Contains(txtName.Text.Trim())).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });

            gv.DataSource = list.OrderBy(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "DeleteItem")
            {
                
                Models.Customer model = customerBll.GetModel(id);
                if (model != null)
                {
                    model.IsDelete = true;
                    customerBll.Update(model);
                    //ClientScript.RegisterClientScriptBlock(GetType(), "alert", "alert('删除成功')", true);
                    BindData();
                }
                
            }
            if (e.CommandName == "EditItem")
            {
                Response.Redirect("AddCustomer.aspx?customerId="+id,false);
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddCustomer.aspx",false);
        }
    }
}
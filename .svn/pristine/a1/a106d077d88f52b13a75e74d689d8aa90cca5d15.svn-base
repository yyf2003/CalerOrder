using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;


namespace WebApp.Materials
{
    public partial class MaterialCategoryList : BasePage
    {
        MaterialCategoryBLL categoryBll = new MaterialCategoryBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            var list = categoryBll.GetList(s=>1==1);
            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                list = list.Where(s => s.CategoryName.Contains(txtName.Text)).ToList();
            }
            if (rblState.SelectedValue != "0")
            {
                int state = int.Parse(rblState.SelectedValue);
                if (state == 1)
                {
                    list = list.Where(s => s.IsDelete==null ||s.IsDelete==false).ToList();
                }
                else
                {
                    list = list.Where(s => s.IsDelete==true).ToList();
                }
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "deleteItem")
            {
                MaterialCategory model = categoryBll.GetModel(id);
                if (model != null)
                {

                    if (model.IsDelete==null || model.IsDelete==false)
                    {
                        model.IsDelete = true;
                    }
                    else
                        model.IsDelete = false;
                    categoryBll.Update(model);
                    BindData();
                }
                else
                   Alert("操作失败！");
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                MaterialCategory model = e.Row.DataItem as MaterialCategory;
                if (model != null)
                {
                    
                    bool isDelete = model.IsDelete ?? false;
                    LinkButton lbIsDelete = (LinkButton)e.Row.FindControl("lbDelete");
                    if (isDelete)
                    {
                        lbIsDelete.Text = "恢复";
                        lbIsDelete.OnClientClick = "return confirm('确定恢复吗？')";
                    }
                    else
                    {
                        lbIsDelete.Text = "删除";
                        lbIsDelete.OnClientClick = "return confirm('确定删除吗？')";
                    }
                }
            }
        }
    }
}
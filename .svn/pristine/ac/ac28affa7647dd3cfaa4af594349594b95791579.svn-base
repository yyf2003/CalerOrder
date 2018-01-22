using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;

namespace WebApp.Materials
{
    public partial class MaterialTypeList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindBigType();
                BindData();
            }
        }

        void BindBigType()
        {
            var list = new MaterialTypeBLL().GetList(s=>s.ParentId==0);
            if (list.Any())
            {
                ddlBigType.DataSource = list;
                ddlBigType.DataTextField = "MaterialTypeName";
                ddlBigType.DataValueField = "Id";
                ddlBigType.DataBind();
            }
            ddlBigType.Items.Insert(0,new ListItem("--请选择--","0"));
        }

        void BindData()
        {
            var list = (from type in CurrentContext.DbContext.MaterialType
                       join bigtype1 in CurrentContext.DbContext.MaterialType
                       on type.ParentId equals bigtype1.Id into temp
                       from bigtype in temp.DefaultIfEmpty()
                       where type.ParentId>0
                       select new {
                           type.Id,
                           type.IsDelete,
                           type.MaterialTypeName,
                           Parent=bigtype.MaterialTypeName,
                           type.ParentId
                       }).ToList();
            if (ddlBigType.SelectedValue != "0")
            {
                int pid = int.Parse(ddlBigType.SelectedValue);
                list = list.Where(s => s.ParentId == pid).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                list = list.Where(s => s.MaterialTypeName.Contains(txtName.Text)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.ParentId).ThenByDescending(s=>s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "deleteItem")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                MaterialTypeBLL bll=new MaterialTypeBLL();
                Models.MaterialType model = bll.GetModel(id);
                if (model != null)
                {

                    model.IsDelete = model.IsDelete!=null?!model.IsDelete:true;
                    bll.Update(model);
                    BindData();
                }

            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object isDeleteObj = item.GetType().GetProperty("IsDelete").GetValue(item, null);
                    bool isDelete = isDeleteObj != null ? bool.Parse(isDeleteObj.ToString()) : false;
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
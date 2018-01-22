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
    public partial class MaterialList : BasePage
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
            ddlBigType.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        void BindData()
        {
            var list = (from material in CurrentContext.DbContext.Material
                        join category in CurrentContext.DbContext.MaterialCategory
                        on material.MaterialCategoryId equals category.Id
                       join smallType in CurrentContext.DbContext.MaterialType
                       on material.SmallTypeId equals smallType.Id
                       join bigType1 in CurrentContext.DbContext.MaterialType
                       on smallType.ParentId equals bigType1.Id into bigTypeTemp
                       join brand1 in CurrentContext.DbContext.MaterialBrand
                       on material.MaterialBrandId equals brand1.Id into brandTemp
                       from brand in brandTemp.DefaultIfEmpty()
                       from bigType in bigTypeTemp.DefaultIfEmpty()
                       select new {
                           material.AddDate,
                           material.Area,
                           material.Id,
                           material.IsDelete,
                           material.Length,
                           material.MaterialName,
                           material.Remark,
                           material.Unit,
                           material.Width,
                           SmallTypeId=material.SmallTypeId,
                           BigTypeId = bigType != null ? bigType.Id : 0,
                           material.MaterialBrandId,
                           smallType.MaterialTypeName,
                           BigTypeName=bigType!=null?bigType.MaterialTypeName:"",
                           BrandName = brand!=null?brand.BrandName:"",
                           material.Specification,
                           category.CategoryName,
                           material.MaterialCategoryId
                       }).ToList();

            if (ddlBigType.SelectedValue != "0")
            {
                int bigId = int.Parse(ddlBigType.SelectedValue);
                list = list.Where(s => s.BigTypeId == bigId).ToList();
            }
            if (hfSmallTypeId.Value != "0" && hfSmallTypeId.Value != "")
            {
                int smallId = int.Parse(hfSmallTypeId.Value);
                list = list.Where(s => s.SmallTypeId == smallId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtMaterialName.Text))
            {
                list = list.Where(s => s.MaterialName.Contains(txtMaterialName.Text)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.SmallTypeId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv,new object[]{btnAdd,btnImport});

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "deleteItem")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                MaterialBLL bll = new MaterialBLL();
                Models.Material model = bll.GetModel(id);
                if (model != null)
                {

                    model.IsDelete = model.IsDelete != null ? !model.IsDelete : true;
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

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Models;
using BLL;
using DAL;

namespace WebApp.Subjects.Material
{
    public partial class MaterialDetail : BasePage 
    {
        OrderMaterialBLL materialBll = new OrderMaterialBLL();
        int itemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemId"] != null)
            {
                itemId = int.Parse(Request.QueryString["itemId"].ToString());
            }
            if (!IsPostBack)
            {
                BindData();
                BindMaterialNames();
            }
        }

        List<string> materialNameList = new List<string>();
        void BindData()
        {
            var list = (from material in CurrentContext.DbContext.OrderMaterial
                       join shop in CurrentContext.DbContext.Shop
                       on material.ShopId equals shop.Id
                       where material.ItemId == itemId
                       select new {
                           material.AddDate,
                           material.ItemId,
                           material.MaterialCount,
                           material.MaterialId,
                           material.MaterialName,
                           material.Remark,
                           material.ShopId,
                           material.Price,
                           shop.ShopNo,
                           shop.ShopName
                       }).ToList();
            materialNameList = list.Select(s => s.MaterialName).Distinct().ToList();
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                list = list.Where(s => s.ShopNo.Contains(txtShopNo.Text)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                list = list.Where(s => s.ShopName.Contains(txtShopName.Text)).ToList();
            }
            List<string> materialname = new List<string>();
            foreach (ListItem li in cblMaterialName.Items)
            {
                if (li.Selected)
                    materialname.Add(li.Value);
            }
            if (materialname.Any())
            {
                list = list.Where(s => materialname.Contains(s.MaterialName)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.ToList().OrderBy(s => s.MaterialId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
        }

        void BindMaterialNames()
        {
            materialNameList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Text = s;
                li.Value = s;
                cblMaterialName.Items.Add(li);
            });
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gv.Rows)
            {
                if (row.RowIndex != -1)
                {
                    CheckBox cb = (CheckBox)row.FindControl("cbOne");
                    if (cb.Checked)
                    {
                        int id = int.Parse(((HiddenField)row.FindControl("hfMaterialId")).Value);
                        OrderMaterial model = materialBll.GetModel(id);
                        if (model != null)
                        {
                            materialBll.Delete(model);
                        }
                    }
                }
            }
            BindData();
        }
    }
}
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
    public partial class SetPrice : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = string.Empty;
            if (Request.QueryString["urlStr"] != null)
            {
                url = Request.QueryString["urlStr"];
                
            }
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                //BindBigType();
                SetSearchVal(url,Panel1);
                BindRegion();
                //BindSmallType();
                SetSearchVal(url, Panel1);
                BindData();
            }
        }

        //void SetSearchVal()
        //{
        //    if (Request.QueryString["urlStr"] != null)
        //    {
        //        string url = Request.QueryString["urlStr"];
        //        if (!string.IsNullOrWhiteSpace(url))
        //        {
        //            string[] arr = url.Split('&');
        //            foreach (string s in arr)
        //            {
        //                if (!string.IsNullOrWhiteSpace(s))
        //                {
        //                    string key = s.Split('=')[0];
        //                    string val = s.Split('=')[1];
        //                    if (!string.IsNullOrWhiteSpace(val))
        //                        SetControlVal(key, val);
        //                }
        //            }
        //        }
        //    }
        //}


        //void SetControlVal(string key,string val)
        //{
        //    foreach (Control control in Panel1.Controls)
        //    {
        //        if (control.ID == key)
        //        {
        //            if (control.GetType().Name == "DropDownList")
        //            {
        //                ((DropDownList)control).SelectedValue = val;
        //            }
        //            if (control.GetType().Name == "TextBox")
        //            {
        //                ((TextBox)control).Text = val;
        //            }
        //        }
        //    }
        //}

        //void BindBigType()
        //{
        //    var list = new MaterialTypeBLL().GetList(s => s.ParentId == 0);
        //    if (list.Any())
        //    {
        //        ddlBigType.DataSource = list;
        //        ddlBigType.DataTextField = "MaterialTypeName";
        //        ddlBigType.DataValueField = "Id";
        //        ddlBigType.DataBind();
        //    }
        //    ddlBigType.Items.Insert(0, new ListItem("--请选择--", "0"));
        //}

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
        }

        void BindRegion()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            BindRegionByCustomer(customerId, ref ddlRegion);
        }

        //protected void ddlBigType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    BindSmallType();
        //}

        //void BindSmallType()
        //{
        //    int bigTypeId = int.Parse(ddlBigType.SelectedValue);
        //    bigTypeId = bigTypeId == 0 ? -1 : bigTypeId;
        //    var list = new MaterialTypeBLL().GetList(s => s.ParentId == bigTypeId);
        //    if (list.Any())
        //    {
        //        ddlSmallType.DataSource = list;
        //        ddlSmallType.DataTextField = "MaterialTypeName";
        //        ddlSmallType.DataValueField = "Id";
        //        ddlSmallType.DataBind();
        //    }
        //    ddlSmallType.Items.Insert(0, new ListItem("--请选择--", "0"));
        //}

        void BindData()
        {
            List<int> myCustomerList = GetMyCustomerList().Select(s => s.Id).ToList();
            var list = (from price in CurrentContext.DbContext.CustomerMaterial
                        join customer in CurrentContext.DbContext.Customer
                        on price.CustomerId equals customer.Id
                        join region1 in CurrentContext.DbContext.Region
                        on price.RegionId equals region1.Id into regionTemp
                        //join type in CurrentContext.DbContext.MaterialType
                        //on price.BigTypeId equals type.Id
                        
                        from region in regionTemp.DefaultIfEmpty()
                        where myCustomerList.Contains(price.CustomerId ?? 0)
                        select new
                        {
                            price.Id,
                            
                            price.CostPrice,
                            price.CustomerId,
                            price.BigTypeId,
                            price.RegionId,
                            price.SalePrice,
                            price.CustomerMaterialName,
                            customer.CustomerName,
                            price.RegionName,
                            //BigTypeName = type.MaterialTypeName,
                            price.IsDelete
                        }).ToList();
            if (ddlCustomer.SelectedValue != "0")
            {
                int customerId = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == customerId).ToList() ;
            }
            if (ddlRegion.SelectedValue != "0")
            {
                int regionId = int.Parse(ddlRegion.SelectedValue);
                list = list.Where(s => s.RegionId == regionId).ToList();
            }
            //if (ddlBigType.SelectedValue != "0")
            //{
            //    int bigTypeId = int.Parse(ddlBigType.SelectedValue);
            //    list = list.Where(s => s.BigTypeId == bigTypeId).ToList();
            //}
            //if (ddlSmallType.SelectedValue != "0")
            //{
            //    int smallTypeId = int.Parse(ddlSmallType.SelectedValue);
            //    list = list.Where(s => s.SmallTypeId == smallTypeId).ToList();
            //}
            //if (!string.IsNullOrWhiteSpace(txtMaterialName.Text))
            //{
            //    list = list.Where(s => s.SmallTypeName.Contains(txtMaterialName.Text)).ToList();
            //}
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.BigTypeId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd,btnImport });
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
                CustomerMaterialBLL bll = new CustomerMaterialBLL();
                Models.CustomerMaterial model = bll.GetModel(id);
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
    }

        
    
}
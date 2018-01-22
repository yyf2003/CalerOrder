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

namespace WebApp.Regions
{
    public partial class List : BasePage
    {
        int customerId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);
            }
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomerSearch);
                BindCustomerList(ref ddlCustomer);
                if (customerId > 0)
                {
                    ddlCustomerSearch.SelectedValue = customerId.ToString();
                    ddlCustomer.SelectedValue = customerId.ToString();
                }
                BindData();
                BindProvince();
            }
        }

        void BindProvince()
        {
            var list = new PlaceBLL().GetList(s => s.ParentID == 0);
            if (list.Any())
            {
                cblProvince.DataSource = list;
                cblProvince.DataTextField = "PlaceName";
                cblProvince.DataValueField = "ID";
                cblProvince.DataBind();

            }
        }

        void BindData()
        {
            List<int> curstomerList = new List<int>();

            foreach (ListItem item in ddlCustomerSearch.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }
            var list = (from region in CurrentContext.DbContext.Region
                        join customer in CurrentContext.DbContext.Customer
                        on region.CustomerId equals customer.Id
                        where customer.IsDelete==null || customer.IsDelete==false
                        && curstomerList.Contains(region.CustomerId??0)
                        select new
                        {
                            region.Id,
                            region.RegionName,
                            customer.CustomerName,
                            region.CustomerId,
                            region.IsDelete
                        }).ToList();
            int customerId1 = int.Parse(ddlCustomerSearch.SelectedValue);
            if (customerId1 > 0)
            {
                list = list.Where(s => s.CustomerId == customerId1).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }



        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object regionIdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int regionId = regionIdObj != null ? int.Parse(regionIdObj.ToString()) : 0;
                    var list = from pr in CurrentContext.DbContext.ProvinceInRegion
                               join province in CurrentContext.DbContext.Place
                               on pr.ProvinceId equals province.ID
                               where pr.RegionId == regionId
                               select province;
                    if (list.Any())
                    {
                        StringBuilder names = new StringBuilder();
                        StringBuilder ids = new StringBuilder();
                        list.ToList().ForEach(s =>
                        {
                            names.Append(s.PlaceName);
                            names.Append("，");
                            ids.Append(s.ID);
                            ids.Append(",");
                        });
                        ((Label)e.Row.FindControl("labProvince")).Text = names.ToString().TrimEnd('，');
                        ((HiddenField)e.Row.FindControl("hfProvinceIds")).Value = ids.ToString().TrimEnd('，');
                    }
                    object isDeleteObj = item.GetType().GetProperty("IsDelete").GetValue(item, null);
                    bool isDelete = isDeleteObj != null ? bool.Parse(isDeleteObj.ToString()) : false;
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");
                    if (isDelete)
                    {
                        lbDelete.Text = "恢复";
                        lbDelete.CommandName = "lbRecover";
                        lbDelete.Attributes.Add("OnClick", "return confirm('确定恢复吗？')");
                    }
                    else
                    {
                        lbDelete.Text = "删除";
                        lbDelete.CommandName = "lbDelete";
                        lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int regionId = int.Parse(e.CommandArgument.ToString());

            RegionBLL regionBll = new RegionBLL();
            Region model = regionBll.GetModel(regionId);
            if (model != null)
            {
                if (e.CommandName == "lbDelete")
                {
                    model.IsDelete = true;
                }
                else if (e.CommandName == "lbRecover")
                    model.IsDelete = false;
                regionBll.Update(model);
            }
            BindData();
        }
    }
}
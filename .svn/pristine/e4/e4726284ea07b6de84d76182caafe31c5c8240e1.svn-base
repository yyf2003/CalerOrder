using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using DAL;
using Models;

namespace WebApp.Customer
{
    public partial class HCPOPList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                BindData();
            }
        }

        void BindData()
        {
            var list = (from hcpop in CurrentContext.DbContext.HCPOP
                       join shop in CurrentContext.DbContext.Shop
                       on hcpop.ShopId equals shop.Id
                       select new { 
                          hcpop.Count,
                          hcpop.GraphicLength,
                          hcpop.GraphicMaterial,
                          hcpop.GraphicWidth,
                          hcpop.Id,
                          hcpop.MachineFrame,
                          hcpop.MachineFrameGender,
                          hcpop.POP,
                          hcpop.POPGender,
                          hcpop.POPType,
                          hcpop.ShopId,
                          shop.ShopName,
                          shop.ShopNo,
                          shop.RegionName,
                          shop.ProvinceName,
                          shop.CityName,
                          shop.CustomerId
                       }).ToList();
            if (ddlCustomer.SelectedValue != "0")
            {
                int customerId = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == customerId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                list = list.Where(s => s.ShopNo.Contains(txtShopNo.Text.Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                list = list.Where(s => s.ShopName.Contains(txtShopName.Text.Trim())).ToList();
            }
            if (ddlRegion.SelectedValue != "0")
            {
                list = list.Where(s => s.RegionName.ToLower() == ddlRegion.SelectedValue.ToLower()).ToList();
            }
            if (ddlProvince.SelectedValue != "0")
            {
                list = list.Where(s => s.ProvinceName == ddlProvince.SelectedValue).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.ShopId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd, btnImport });
        }


        void BindRegion()
        {
            ddlRegion.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            //var list = new ShopBLL().GetList(s => s.CustomerId == customerId);
            var list = (from hcpop in CurrentContext.DbContext.HCPOP
                        join shop in CurrentContext.DbContext.Shop
                        on hcpop.ShopId equals shop.Id
                        where shop.CustomerId == customerId
                        select shop).ToList();
            if (list.Any())
            {
                List<string> regionList = new List<string>();
                list.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s.RegionName) && !regionList.Contains(s.RegionName))
                    {
                        regionList.Add(s.RegionName);
                        ddlRegion.Items.Add(new ListItem(s.RegionName, s.RegionName));
                    }
                });
            }
            ddlRegion.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        void BindProvince()
        {
            ddlProvince.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            string regionId = ddlRegion.SelectedValue;
            //var list = new ShopBLL().GetList(s => s.CustomerId == customerId && s.RegionName == regionId);
            var list = (from hcpop in CurrentContext.DbContext.HCPOP
                        join shop in CurrentContext.DbContext.Shop
                        on hcpop.ShopId equals shop.Id
                        where shop.CustomerId == customerId && shop.RegionName == regionId
                        select shop).ToList();
            if (list.Any())
            {
                List<string> provinceList = new List<string>();
                list.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s.ProvinceName) && !provinceList.Contains(s.ProvinceName))
                    {
                        provinceList.Add(s.ProvinceName);
                        ddlProvince.Items.Add(new ListItem(s.ProvinceName, s.ProvinceName));
                    }
                });
            }
            ddlProvince.Items.Insert(0, new ListItem("--请选择--", "0"));
        }


        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
        }

        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void btnExportShop_Click(object sender, EventArgs e)
        {

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
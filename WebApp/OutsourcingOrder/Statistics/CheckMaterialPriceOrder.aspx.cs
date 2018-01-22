using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using BLL;
using DAL;

namespace WebApp.OutsourcingOrder.Statistics
{
    public partial class CheckMaterialPriceOrder : BasePage
    {
        int customerId = 0;
        string outsourceId = string.Empty;
        
        string guidanceMonth = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["outsourceId"] != null)
            {
                outsourceId = Request.QueryString["outsourceId"];
            }
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);
            }
            if (Request.QueryString["guidanceMonth"] != null)
            {
                guidanceMonth = Request.QueryString["guidanceMonth"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData() {
            int year = 0;
            int month = 0;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                year = date.Year;
                month = date.Month;
            }
            var list = (from order in CurrentContext.DbContext.OutsourceReceivePriceOrder
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join outsource in CurrentContext.DbContext.Company
                        on order.OutsourceId equals outsource.Id
                        where order.CustomerId == customerId
                        && order.GuidanceYear == year
                        && order.GuidanceMonth == month
                        select new
                        {
                            order,
                            shop,
                            outsource.CompanyName
                        }).ToList();
            if (list.Any())
            {
                labShopCount.Text = list.Select(s=>s.order.ShopId??0).Distinct().Count().ToString();
                labTotalPrice.Text = list.Sum(s=>s.order.TotalPrice??0).ToString();
            }
            Repeater1.DataSource = list;
            Repeater1.DataBind();
        }
    }
}
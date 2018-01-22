using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;

namespace WebApp.Quotation
{
    public partial class OrderPrice : BasePage
    {
        int SubjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectid"] != null)
            {
                SubjectId = int.Parse(Request.QueryString["subjectid"]);
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                       where order.SubjectId == SubjectId && order.GraphicMaterial.Length>0
                       group order by new {
                           order.GraphicMaterial,
                           order.UnitPrice
                       }  into g
                       select new {
                          GraphicMaterial=g.Key.GraphicMaterial,
                          UnitPrice=g.Key.UnitPrice,
                          Area=g.Sum(s=>s.Area),
                          Price=g.Sum(s=>(s.Area??0)*(s.UnitPrice??0))
                       }).ToList();
            gvPrice.DataSource = list;
            gvPrice.DataBind();

        }

        decimal totalPrice = 0;
        protected void gvPrice_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object priceObj = item.GetType().GetProperty("Price").GetValue(item, null);
                    decimal price = priceObj != null ? decimal.Parse(priceObj.ToString()) : 0;
                    ((Label)e.Item.FindControl("labPrice")).Text = price > 0 ? Math.Round(price,2).ToString() : "0";
                    totalPrice += price;
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labTotalPrice")).Text = Math.Round(totalPrice, 2).ToString();
            }
        }
    }
}
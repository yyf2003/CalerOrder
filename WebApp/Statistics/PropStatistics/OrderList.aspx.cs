using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using Common;


namespace WebApp.Statistics.PropStatistics
{
    public partial class OrderList : BasePage
    {
        string guidanceId = string.Empty;
        string subjectId = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = Request.QueryString["subjectId"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            List<int> guidanceIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId,',');
            }
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
            }
            var list = (from order in CurrentContext.DbContext.PropOrderDetail
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on order.GuidanceId equals guidance.ItemId
                        where guidanceIdList.Contains(order.GuidanceId ?? 0)
                        && (subjectIdList.Any() ? (subjectIdList.Contains(order.SubjectId ?? 0)) : 1 == 1)
                        select new
                        {
                            order,
                            subject,
                            guidance,
                            Quantity=order.Quantity??1,
                            UnitPrice=order.UnitPrice??0
                        }).ToList();
            Repeater1.DataSource = list;
            Repeater1.DataBind();
            if (list.Any())
            {
                decimal total = list.Sum(s => ((s.order.Quantity ?? 1) * (s.order.UnitPrice ?? 0)));
                labTotalPrice.Text = Math.Round(total, 2).ToString();
            }
        }


        decimal totalPrice = 0;
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objQuantity = item.GetType().GetProperty("Quantity").GetValue(item, null);
                    object objUnitPrice = item.GetType().GetProperty("UnitPrice").GetValue(item, null);

                    int Quantity = objQuantity != null ? int.Parse(objQuantity.ToString()) : 1;
                    decimal UnitPrice = objUnitPrice != null ? decimal.Parse(objUnitPrice.ToString()) : 0;
                    decimal subPrice = Quantity * UnitPrice;
                    ((Label)e.Item.FindControl("labReceiveSub")).Text = Math.Round(subPrice, 2).ToString();
                    totalPrice += subPrice;
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labReceiveTotal")).Text = Math.Round(totalPrice, 2).ToString();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;

namespace WebApp.Statistics.PropStatistics
{
    public partial class OutsourceOrderList : BasePage
    {
        string guidanceId = string.Empty;
        string subjectId = string.Empty;
        string outsourceName = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["outsourceName"] != null)
            {
                outsourceName = Request.QueryString["outsourceName"];
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
            List<string> outsourceNameList = new List<string>();
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(outsourceName))
            {
                outsourceNameList = StringHelper.ToStringList(outsourceName, ',');
            }
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
            }
            var list = (from oOrder in CurrentContext.DbContext.PropOutsourceOrderDetail
                        join rOrder in CurrentContext.DbContext.PropOrderDetail
                        on oOrder.PropOrderId equals rOrder.Id
                        join subject in CurrentContext.DbContext.Subject
                        on oOrder.SubjectId equals subject.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on oOrder.GuidanceId equals guidance.ItemId
                        where guidanceIdList.Contains(oOrder.GuidanceId ?? 0)
                        && (subjectIdList.Any() ? (subjectIdList.Contains(oOrder.SubjectId ?? 0)) : 1 == 1)
                        select new
                        {
                            oOrder,
                            rOrder,
                            subject,
                            guidance,
                            Quantity = oOrder.Quantity ?? 1,
                            UnitPrice = oOrder.UnitPrice ?? 0
                        }).ToList();
            Repeater1.DataSource = list;
            Repeater1.DataBind();
            CombineCell(Repeater1, new List<string> { "tbGuidanceName", "tbSubjectName", "tbMaterialName", "tbSheet", "tbMaterialType", "tbDimension" }, true);
            if (list.Any())
            {
                decimal total = list.Sum(s => ((s.oOrder.Quantity ?? 1) * (s.oOrder.UnitPrice ?? 0)));
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
                    ((Label)e.Item.FindControl("labPaySub")).Text = Math.Round(subPrice, 2).ToString();
                    totalPrice += subPrice;
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labPayTotal")).Text = Math.Round(totalPrice, 2).ToString();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {

        }
    }
}
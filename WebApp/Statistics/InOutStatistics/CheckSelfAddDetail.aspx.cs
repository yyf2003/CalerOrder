using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using System.Text;
using Common;
using System.Web.UI.HtmlControls;

namespace WebApp.Statistics.InOutStatistics
{
    public partial class CheckSelfAddDetail : BasePage
    {
        int quoteItemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["quoteItemId"] != null)
            {
                quoteItemId = int.Parse(Request.QueryString["quoteItemId"]);
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        List<string> sheetList = new List<string>();
        List<QuoteOrderDetail> addSizeRateOrderList = new List<QuoteOrderDetail>();
        List<QuoteDifferenceDetail> quoteDifferenceDetailList = new List<QuoteDifferenceDetail>();
        void BindData()
        {
            addSizeRateOrderList = new QuoteOrderDetailBLL().GetList(s => s.QuoteItemId == quoteItemId && (s.AddSizeRate ?? 0) > 0 && s.Sheet != null);
            if (addSizeRateOrderList.Any())
            {
                sheetList = addSizeRateOrderList.Select(s=>s.Sheet.ToUpper()).Distinct().ToList();
            }
            quoteDifferenceDetailList = new QuoteDifferenceDetailBLL().GetList(s => s.QuoteItemId == quoteItemId && s.OrderType == (int)OrderTypeEnum.POP);
            if (quoteDifferenceDetailList.Any())
            {
                List<string> sheet2 = quoteDifferenceDetailList.Select(s => s.Sheet.ToUpper()).Distinct().ToList();
                sheetList=sheetList.Union(sheet2).OrderBy(s => s).ToList();
            }
            Repeater1.DataSource = sheetList;
            Repeater1.DataBind();
        }

        decimal totalArea = 0;
        decimal totalPrice = 0;
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                string sheet = e.Item.DataItem.ToString();
                if (!string.IsNullOrWhiteSpace(sheet))
                {
                    if (addSizeRateOrderList.Any())
                    {
                        var list0 = addSizeRateOrderList.Where(s=>s.Sheet.ToUpper()==sheet).ToList();
                        if (list0.Any())
                        {
                            decimal rate = list0[0].AddSizeRate ?? 0;
                            decimal rateArea = list0.Sum(s => (((s.TotalGraphicWidth ?? 0) * (s.TotalGraphicLength ?? 0)) / 1000000) - (((s.GraphicWidth ?? 0) * (s.GraphicLength ?? 0)) / 1000000) * (s.Quantity ?? 1));
                            decimal ratePrice = list0.Sum(s => ((s.AutoAddTotalPrice ?? 0) - (s.DefaultTotalPrice ?? 0)));
                            ((Label)e.Item.FindControl("labAddRate")).Text = rate.ToString();
                            ((Label)e.Item.FindControl("labAddRateArea")).Text = Math.Round(rateArea, 2).ToString();
                            ((Label)e.Item.FindControl("labAddRatePrice")).Text = Math.Round(ratePrice, 2).ToString();
                            totalArea += rateArea;
                            totalPrice += ratePrice;
                        }
                    }
                    if (quoteDifferenceDetailList.Any())
                    {
                        var list1 = quoteDifferenceDetailList.Where(s => s.Sheet.ToUpper() == sheet).ToList();
                        if (list1.Any())
                        {
                            HtmlTable AddExtendPOPPriceTable = (HtmlTable)e.Item.FindControl("AddExtendPOPPriceTable");
                            HtmlTableRow headrow = new HtmlTableRow();
                            headrow.Attributes.Add("class", "tr_hui");
                            headrow.Style.Add("font-weight", "bold");
                            HtmlTableCell cell0 = new HtmlTableCell();
                            cell0.InnerHtml = "类型";
                            cell0.Style.Add("width","80px");
                            headrow.Cells.Add(cell0);
                            cell0 = new HtmlTableCell();
                            cell0.InnerHtml = "折算材质";
                            cell0.Style.Add("width", "180px");
                            headrow.Cells.Add(cell0);
                            cell0 = new HtmlTableCell();
                            cell0.InnerHtml = "面积㎡";
                            cell0.Style.Add("width", "80px");
                            headrow.Cells.Add(cell0);
                            cell0 = new HtmlTableCell();
                            cell0.InnerHtml = "金额";
                            cell0.Style.Add("width", "120px");
                            headrow.Cells.Add(cell0);
                            cell0 = new HtmlTableCell();
                            cell0.InnerHtml = "备注";
                            headrow.Cells.Add(cell0);
                            AddExtendPOPPriceTable.Rows.Add(headrow);
                            decimal otherTotalPrice = 0;
                            list1.ForEach(s =>
                            {

                                HtmlTableRow row = new HtmlTableRow();

                                HtmlTableCell cell = new HtmlTableCell();
                                cell.InnerHtml = s.AddPriceType;
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                cell.InnerHtml = s.MaterialName;
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                cell.InnerHtml = (s.AddArea ?? 0).ToString();
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                decimal price = (s.AddArea ?? 0) * (s.MaterialUnitPrice ?? 0);
                                cell.InnerHtml = Math.Round(price, 2).ToString();
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                cell.InnerHtml = s.Remark;
                                row.Cells.Add(cell);


                                row.Cells.Add(cell);
                                AddExtendPOPPriceTable.Rows.Add(row);
                                otherTotalPrice += price;
                                totalArea += (s.AddArea ?? 0);
                                totalPrice += price;

                            });
                            HtmlTableRow row1 = new HtmlTableRow();
                            HtmlTableCell cell1 = new HtmlTableCell();
                            cell1.ColSpan = 3;
                            cell1.InnerHtml = "合计：";
                            cell1.Style.Add("text-align", "right");
                            row1.Cells.Add(cell1);

                            cell1 = new HtmlTableCell();
                            cell1.InnerHtml = Math.Round(otherTotalPrice, 2).ToString();
                            cell1.Style.Add("color", "green");
                            row1.Cells.Add(cell1);

                            cell1 = new HtmlTableCell();
                            cell1.InnerHtml = "";
                            row1.Cells.Add(cell1);
                            AddExtendPOPPriceTable.Rows.Add(row1);
                        }
                    }
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labTotalArea")).Text = Math.Round(totalArea, 2).ToString();
                ((Label)e.Item.FindControl("labTotalPrice")).Text = Math.Round(totalPrice, 2).ToString();
            }
        }
    }
}
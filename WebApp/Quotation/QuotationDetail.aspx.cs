using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BLL;
using DAL;
using Models;

namespace WebApp.Quotation
{
    public partial class QuotationDetail : BasePage
    {
        int quotationId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["quotationid"] != null)
            {
                quotationId = int.Parse(Request.QueryString["quotationid"]);
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        { 
                QuotationsBLL QuotationsBll = new QuotationsBLL();
                ADSubjectPriceBLL priceBll = new ADSubjectPriceBLL();
                Quotations model = QuotationsBll.GetModel(quotationId);
                if (model != null)
                {
                    labCategory.Text = model.Category;
                    labBelongs.Text = model.Blongs;
                    labClassification.Text = model.Classification;
                    labAdidasContact.Text = model.AdidasContact;
                    labTaxRate.Text = model.TaxRate;
                    if (model.OfferPrice != null)
                        labOfferPrice.Text = model.OfferPrice.ToString();
                    labAccount.Text = model.Account;
                    if (model.OtherPrice != null && model.OtherPrice > 0)
                        labOtherPrice.Text = model.OtherPrice.ToString();
                    labOtherPriceRemark.Text = model.OtherPriceRemark;
                    labRemark.Text = model.Remark;

                    var priceList = priceBll.GetList(s => s.QuotationsId == model.Id);
                    if (priceList.Any())
                    { 
                       var shareList = priceList.Where(s => s.PriceType == 1).ToList();
                       gvShareList.DataSource = shareList;
                       gvShareList.DataBind();
                       var bingList = priceList.Where(s => s.PriceType == 2).ToList();
                       gvBingList.DataSource = bingList;
                       gvBingList.DataBind();
                       CombinCol();
                    }
                }
        }

        decimal shareTotal=0;
        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
               
                ADSubjectPrice model = (ADSubjectPrice)e.Item.DataItem;
                if (model != null)
                {
                    shareTotal += model.Price ?? 0;
                }
               
            }
        }

        decimal bingTotal = 0;
        protected void gvBingList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {

                ADSubjectPrice model = (ADSubjectPrice)e.Item.DataItem;
                if (model != null)
                {
                    bingTotal += model.Price ?? 0;
                }
            }
        }

        void CombinCol()
        {
            if (gvShareList.Items.Count > 1)
            {
                for (int i = gvShareList.Items.Count - 1; i > 0; i--)
                {
                    HtmlTableCell cell0 = (HtmlTableCell)gvShareList.Items[i - 1].FindControl("shareTotalTd");
                    HtmlTableCell cell1 = (HtmlTableCell)gvShareList.Items[i].FindControl("shareTotalTd");
                    cell1.RowSpan = (cell1.RowSpan == -1) ? 1 : cell1.RowSpan;
                    cell0.RowSpan = (cell0.RowSpan == -1) ? 1 : cell0.RowSpan;
                    cell1.Visible = false;
                    cell0.RowSpan += cell1.RowSpan;
                    if (i == 1)
                    {
                        cell0.InnerHtml = shareTotal.ToString();
                    }
                }
            }
            else if (gvShareList.Items.Count > 0)
            {
                ((HtmlTableCell)gvShareList.Items[0].FindControl("shareTotalTd")).InnerHtml = shareTotal.ToString();
            }

            if (gvBingList.Items.Count > 1)
            {
                for (int i = gvBingList.Items.Count - 1; i > 0; i--)
                {
                    HtmlTableCell cell00 = (HtmlTableCell)gvBingList.Items[i - 1].FindControl("bingTotalTd");
                    HtmlTableCell cell11 = (HtmlTableCell)gvBingList.Items[i].FindControl("bingTotalTd");
                    cell11.RowSpan = (cell11.RowSpan == -1) ? 1 : cell11.RowSpan;
                    cell00.RowSpan = (cell00.RowSpan == -1) ? 1 : cell00.RowSpan;
                    cell11.Visible = false;
                    cell00.RowSpan += cell11.RowSpan;
                    if (i == 1)
                    {
                        cell00.InnerHtml = bingTotal.ToString();
                    }
                }
            }
            else if (gvBingList.Items.Count > 0)
            {
                ((HtmlTableCell)gvBingList.Items[0].FindControl("bingTotalTd")).InnerHtml = bingTotal.ToString();
            }
        }
    }
}
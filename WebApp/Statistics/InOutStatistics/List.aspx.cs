using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.Statistics.InOutStatistics
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                BindGuidance();
                BindData();
            }
        }

        void BindGuidance(int? onDateSearch = null)
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);

            // var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId).ToList();
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        //join type in CurrentContext.DbContext.ADOrderActivity
                        //on guidance.ActivityTypeId equals type.ActivityId
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance
                            //type
                        }).Distinct().ToList();


            if (onDateSearch != null)
            {
                string begin = txtGuidanceBegin.Text;
                string end = txtGuidanceEnd.Text;
                if (!string.IsNullOrWhiteSpace(begin) && !string.IsNullOrWhiteSpace(end))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    DateTime endDate = DateTime.Parse(end).AddDays(1);

                    list = list.Where(s => s.guidance.BeginDate >= beginDate && s.guidance.BeginDate < endDate).ToList();
                }
            }
            else
            {


                string guidanceMonth = txtGuidanceMonth.Text;
                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                {
                    DateTime date = DateTime.Parse(guidanceMonth);
                    int year = date.Year;
                    int month = date.Month;
                    list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();

                }

            }

            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    cblGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyGuidance.Visible = !list.Any();
        }

        void BindQuoteSubject()
        {
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            //var quoteList=new QuotationItemBLL().GetList(s=>guidanceIdList.Contains(s.g))
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnGetGuidance_Click(object sender, EventArgs e)
        {

        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            BindGuidance();
            BindData();
        }

        protected void lbUp_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month <= 1)
                {
                    year--;
                    month = 12;
                }
                else
                    month--;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                //BindSubjectCategory();
                BindData();
                
            }
        }

        protected void lbDown_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month >= 12)
                {
                    year++;
                    month = 1;
                }
                else
                    month++;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                BindData();
                //BindSubjectCategory();
               
            }
        }

        protected void btnCheckAllGuidance_Click(object sender, EventArgs e)
        {

        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cblSubjectCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cblQuoteSubjectList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void BindData()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            string month = txtGuidanceMonth.Text.Trim();
            int guidanceYear = 0;
            int guidanceMonth = 0;
            if (!string.IsNullOrWhiteSpace(month) && StringHelper.IsDateTime(month))
            {
                DateTime date = DateTime.Parse(month);
                guidanceYear = date.Year;
                guidanceMonth = date.Month;
            }
            var quoteList = new QuotationItemBLL().GetList(s =>s.CustomerId==customerId && s.GuidanceYear == guidanceYear && s.GuidanceMonth == guidanceMonth);
            Repeater_List.DataSource = quoteList;
            Repeater_List.DataBind();
        }

        SubjectGuidanceBLL guidanceBll = new SubjectGuidanceBLL();
        SubjectBLL subjectBll = new SubjectBLL();
        ADSubjectCategoryBLL subjectCategoryBll = new ADSubjectCategoryBLL();
        QuoteOrderDetailBLL quoteOrderBll = new QuoteOrderDetailBLL();
        ImportQuoteOrderBLL importQuoteOrderBll = new ImportQuoteOrderBLL();
        OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();

        protected void Repeater_List_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                QuotationItem model = e.Item.DataItem as QuotationItem;
                if (model != null)
                {
                    List<int> guidanceIdList = new List<int>();
                    if (!string.IsNullOrWhiteSpace(model.GuidanceId))
                    {
                        guidanceIdList = StringHelper.ToIntList(model.GuidanceId, ',');
                        List<string> guidanceNameList = guidanceBll.GetList(s => guidanceIdList.Contains(s.ItemId)).Select(s=>s.ItemName).ToList();
                        if (guidanceNameList.Any())
                        {
                            Label labGuidanceName = (Label)e.Item.FindControl("labGuidanceName");
                            string link = string.Format("<a href='StatisticDetail.aspx?itemId={0}' style='color:blue;text-decoration:underline;'>{1}</a>", model.Id, StringHelper.ListToString(guidanceNameList));
                            labGuidanceName.Text = link;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(model.SubjectCategoryId))
                    {
                        List<int> subjectCategoryIdList = StringHelper.ToIntList(model.SubjectCategoryId, ',');
                        List<string> categoryNameList = subjectCategoryBll.GetList(s => subjectCategoryIdList.Contains(s.Id)).Select(s=>s.CategoryName).ToList();
                        if (categoryNameList.Any())
                        {
                            Label labSubjectCategory = (Label)e.Item.FindControl("labSubjectCategory");
                            labSubjectCategory.Text = StringHelper.ListToString(categoryNameList);
                        }
                    }
                    List<int> subjectIdList = new List<int>();
                    if (!string.IsNullOrWhiteSpace(model.SubjectIds))
                    {
                        subjectIdList = StringHelper.ToIntList(model.SubjectIds,',');
                        List<int> hMakeSubjectIdList = subjectBll.GetList(s => subjectIdList.Contains(s.HandMakeSubjectId ?? 0) && guidanceIdList.Contains(s.GuidanceId??0) && s.ApproveState==1 &&(s.IsDelete==null||s.IsDelete==false)).Select(s=>s.Id).ToList();
                        subjectIdList.AddRange(hMakeSubjectIdList);
                    }
                    List<int> quoteShopIdList = new List<int>();
                    var quoteOrderList = quoteOrderBll.GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.SubjectId ?? 0) && s.QuoteItemId == model.Id);
                    
                    //系统订单
                    if (quoteOrderList.Any())
                    {
                        quoteShopIdList = quoteOrderList.Select(s=>s.ShopId??0).Distinct().ToList();
                        decimal totalArea = quoteOrderList.Sum(s => (s.Area ?? 0) * (s.Quantity ?? 1));
                        decimal shutArea = quoteOrderList.Where(s => s.ShopStatus != null && s.ShopStatus.Contains("闭")).Sum(s => (s.Area ?? 0) * (s.Quantity ?? 1));
                        ((Label)e.Item.FindControl("labTotalArea")).Text = Math.Round(totalArea, 2).ToString();
                        ((Label)e.Item.FindControl("labShutArea")).Text = Math.Round(shutArea, 2).ToString();
                    }
                    #region 统计应收（收入）
                    decimal quoteTotalPrice = 0;
                    var importQuoteList = importQuoteOrderBll.GetList(s=>s.ItemId==model.Id);
                    if (importQuoteList.Any())
                    {
                        var popPriceList = importQuoteList.Where(s=>s.OrderType==(int)OrderTypeEnum.POP).ToList();
                        if (popPriceList.Any())
                        {
                            decimal popPrice = popPriceList.Sum(s=>s.POPPrice??0);
                            ((Label)e.Item.FindControl("labQuotePOPPrice")).Text = popPrice.ToString();
                            decimal area = popPriceList.Sum(s => s.POPArea ?? 0);
                            ((Label)e.Item.FindControl("labQuoteArea")).Text = area.ToString();
                            quoteTotalPrice += popPrice;
                        }
                        var installPriceList = importQuoteList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).ToList();
                        if (installPriceList.Any())
                        {
                            decimal installPrice = installPriceList.Sum(s=>(s.Quantity??1)*(s.InstallUnitPrice??0));
                            ((Label)e.Item.FindControl("labQuoteInstallPrice")).Text = installPrice.ToString();
                            quoteTotalPrice += installPrice;
                        }
                        var expressPriceList = importQuoteList.Where(s => s.OrderType == (int)OrderTypeEnum.快递费).ToList();
                        if (expressPriceList.Any())
                        {
                            decimal expressPrice = expressPriceList.Sum(s => (s.Quantity ?? 1) * (s.ExpressUnitPrice ?? 0));
                            ((Label)e.Item.FindControl("labQuoteExpressPrice")).Text = expressPrice.ToString();
                            quoteTotalPrice += expressPrice;
                        }
                        var materialPriceList = importQuoteList.Where(s => s.OrderType == (int)OrderTypeEnum.物料).ToList();
                        if (materialPriceList.Any())
                        {
                            decimal materialPrice = materialPriceList.Sum(s => (s.Quantity ?? 1) * (s.UnitPrice ?? 0));
                            ((Label)e.Item.FindControl("labQuoteOtherPrice")).Text = materialPrice.ToString();
                            quoteTotalPrice += materialPrice;
                        }
                        ((Label)e.Item.FindControl("labQuoteTotalPrice")).Text = quoteTotalPrice.ToString();
                    }
                    #endregion

                    #region 以下是统计应付（支出）

                    var outsourceOrderList = outsourceOrderBll.GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0));
                    decimal payTotalPrice = 0;
                    //pop
                    var popOrderList_o = outsourceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.POP && subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
                    if (popOrderList_o.Any())
                    {
                        decimal popPrice = popOrderList_o.Sum(s => s.TotalPrice ?? 0);
                        decimal popArea = popOrderList_o.Sum(s => (s.Area ?? 0) * (s.Quantity ?? 1));
                        Label labPayPOPPrice = (Label)e.Item.FindControl("labPayPOPPrice");
                        labPayPOPPrice.Text = Math.Round(popPrice,2).ToString();
                        ((Label)e.Item.FindControl("labPayArea")).Text = Math.Round(popArea, 2).ToString();

                        payTotalPrice += popPrice;
                    }
                    //安装费
                    var installPriceList_o = outsourceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费 && (subjectIdList.Contains(s.SubjectId ?? 0) || subjectIdList.Contains(s.BelongSubjectId ?? 0))).ToList();
                    if (installPriceList_o.Any())
                    {
                        decimal installPrice = installPriceList_o.Sum(s => s.PayOrderPrice ?? 0);
                        Label labPayInstallPrice = (Label)e.Item.FindControl("labPayInstallPrice");
                        labPayInstallPrice.Text = Math.Round(installPrice, 2).ToString();
                        payTotalPrice += installPrice;
                    }
                    //快递费
                    var expressPriceList_o = outsourceOrderList.Where(s => (s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.快递费 || s.OrderType == (int)OrderTypeEnum.运费) && (subjectIdList.Contains(s.SubjectId??0) || quoteShopIdList.Contains(s.ShopId??0))).ToList();
                    if (expressPriceList_o.Any())
                    {
                        decimal expressPrice = expressPriceList_o.Sum(s => s.PayOrderPrice ?? 0);
                        Label labPayExpressPrice = (Label)e.Item.FindControl("labPayExpressPrice");
                        labPayExpressPrice.Text = Math.Round(expressPrice, 2).ToString();
                        payTotalPrice += expressPrice;
                    }
                    //其他费用
                    var otherPriceList_o = outsourceOrderList.Where(s => (s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费) && subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
                    if (otherPriceList_o.Any())
                    {
                        decimal otherPrice = otherPriceList_o.Sum(s => s.PayOrderPrice ?? 0);
                        Label labPayOtherPrice = (Label)e.Item.FindControl("labPayOtherPrice");
                        labPayOtherPrice.Text = Math.Round(otherPrice, 2).ToString();
                        payTotalPrice += otherPrice;
                    }
                    if (payTotalPrice > 0)
                    {
                        Label labPayTotalPrice = (Label)e.Item.FindControl("labPayTotalPrice");
                        labPayTotalPrice.Text = Math.Round(payTotalPrice, 2).ToString();
                    }
                    #endregion
                }
            }
        }

        
    }
}
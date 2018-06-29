using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using BLL;
using Models;
using Common;
using System.Text;
using System.Configuration;

namespace WebApp.Statistics.InOutStatistics
{
    public partial class StatisticDetail : BasePage
    {
        public int quoteItemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemId"] != null)
            {
                quoteItemId = int.Parse(Request.QueryString["itemId"]);
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindQuote();
                BindSystemOrder();
                BindSupplement();
                BindAdd();

            }
        }
        List<QuoteOrderDetail> QuoteOrderDetailList = new List<QuoteOrderDetail>();
        List<int> guidanceIdList = new List<int>();
        List<int> subjectIdList = new List<int>();
        QuotationItemBLL quotationItemBll = new QuotationItemBLL();
        QuoteOrderDetailBLL quoteOrderBll = new QuoteOrderDetailBLL();
        void BindSubject()
        {
            QuotationItem itemModel = quotationItemBll.GetModel(quoteItemId);
            if (itemModel != null)
            {
                if (!string.IsNullOrWhiteSpace(itemModel.GuidanceId))
                {
                    guidanceIdList = StringHelper.ToIntList(itemModel.GuidanceId, ',');
                    List<string> guidanceNameList = new SubjectGuidanceBLL().GetList(s => guidanceIdList.Contains(s.ItemId)).Select(s=>s.ItemName).OrderBy(s=>s).ToList();
                    if (guidanceNameList.Any())
                    {
                        labGuidanceName.Text = StringHelper.ListToString(guidanceNameList,",");
                    }
                }
                if (!string.IsNullOrWhiteSpace(itemModel.SubjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(itemModel.SubjectIds, ',');
                    
                }
                if (itemModel.QuoteSubjectCategoryId != null)
                {
                    ADSubjectCategory subjectCategoryModel = new ADSubjectCategoryBLL().GetModel(itemModel.QuoteSubjectCategoryId ?? 0);
                    if (subjectCategoryModel != null)
                    {
                        labQuoteSubjectCategoryName.Text = subjectCategoryModel.CategoryName;
                    }
                }
                labQuoteSubjectName.Text = itemModel.QuoteSubjectName;
            }
        }


        decimal InstallQuoutPrice = 0;
        decimal ExpressQuotePrice = 0;

        decimal InstallSystemPrice = 0;
        decimal ExpressSystemPrice = 0;

        //decimal InstallSupplementPrice = 0;
        //decimal ExpressSupplementPrice = 0;
        /// <summary>
        /// 实际报价
        /// </summary>
        void BindQuote()
        {

            var importQuoteList = new ImportQuoteOrderBLL().GetList(s=>s.ItemId==quoteItemId);
            if (importQuoteList.Any())
            {
                decimal totalQuote = 0;
                var popPriceList = importQuoteList.Where(s => s.OrderType == (int)OrderTypeEnum.POP).ToList();
                if (popPriceList.Any())
                {
                    decimal popPrice = popPriceList.Sum(s => s.POPPrice ?? 0);
                    labTotalPOPPrice.Text = popPrice.ToString();
                    decimal area = popPriceList.Sum(s => s.POPArea ?? 0);
                    labTotalArea.Text = area.ToString();
                    totalQuote += popPrice;
                }
                var installPriceList = importQuoteList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).ToList();
                if (installPriceList.Any())
                {
                    decimal installPrice = installPriceList.Sum(s => (s.Quantity ?? 1) * (s.InstallUnitPrice ?? 0));
                    labTotalInstallPrice.Text = installPrice.ToString();
                    totalQuote += installPrice;
                    InstallQuoutPrice = installPrice;
                }
                var expressPriceList = importQuoteList.Where(s => s.OrderType == (int)OrderTypeEnum.快递费).ToList();
                if (expressPriceList.Any())
                {
                    decimal expressPrice = expressPriceList.Sum(s => (s.Quantity ?? 1) * (s.ExpressUnitPrice ?? 0));
                    labTotalExpressPrice.Text = expressPrice.ToString();
                    totalQuote += expressPrice;
                    ExpressQuotePrice = expressPrice;
                }
                var materialPriceList = importQuoteList.Where(s => s.OrderType == (int)OrderTypeEnum.物料).ToList();
                if (materialPriceList.Any())
                {
                    decimal materialPrice = materialPriceList.Sum(s => (s.Quantity ?? 1) * (s.UnitPrice ?? 0));
                    labTotalMaterialPrice.Text = materialPrice.ToString();
                    totalQuote += materialPrice;
                }
                labTotalSub.Text = totalQuote.ToString();
            }
        }

        //系统单数据
        void BindSystemOrder()
        {
            if (!QuoteOrderDetailList.Any())
            {
                QuoteOrderDetailList = quoteOrderBll.GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.SubjectId ?? 0) && s.QuoteItemId == quoteItemId).ToList();

            }
            #region 全部订单
            if (QuoteOrderDetailList.Any())
            {
                List<int> orderShopIdList = QuoteOrderDetailList.Select(s=>s.ShopId??0).Distinct().ToList();
                decimal totalPrice = 0;
                decimal totalArea = QuoteOrderDetailList.Sum(s => (s.Area ?? 0) * (s.Quantity ?? 1));
                decimal totalPOPPrice = QuoteOrderDetailList.Sum(s=>(s.DefaultTotalPrice??0));
                labOrderArea.Text = Math.Round(totalArea, 2).ToString();
                labOrderPOPPrice.Text = Math.Round(totalPOPPrice, 2).ToString();
                totalPrice += totalPOPPrice;
                //安装费
                var installList = new InstallPriceShopInfoBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId??0) && subjectIdList.Contains(s.SubjectId??0));
                if (installList.Any())
                {
                    decimal installPrice = installList.Sum(s=>((s.BasicPrice??0)+(s.OOHPrice??0)+(s.WindowPrice??0)));
                    labOrderInstall.Text = Math.Round(installPrice, 2).ToString();
                    totalPrice += installPrice;
                    InstallSystemPrice += installPrice;
                }
                //安装费订单
                var installOrderList = QuoteOrderDetailList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费).ToList();
                if (installOrderList.Any())
                {
                    decimal installPrice1 = installOrderList.Sum(s=>s.OrderPrice??0);
                    InstallSystemPrice += installPrice1;
                }
                //快递费
                decimal expressPrice = 0;
                var expressList = new ExpressPriceDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && orderShopIdList.Contains(s.ShopId??0)).ToList();
                if (expressList.Any())
                {
                    expressPrice = expressList.Sum(s=>s.ExpressPrice??0);
                    
                }
                //快递费订单
                var expressOrderList = QuoteOrderDetailList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.快递费 || s.OrderType == (int)OrderTypeEnum.运费).ToList();
                if (expressOrderList.Any())
                {
                    decimal expressPrice1 = expressOrderList.Sum(s => s.OrderPrice ?? 0);
                    expressPrice += expressPrice1;
                }
                //物料费用
                var materialPriceOrderList = QuoteOrderDetailList.Where(s => s.OrderType == (int)OrderTypeEnum.物料).ToList();
                if (materialPriceOrderList.Any())
                {
                    decimal materialPrice = materialPriceOrderList.Sum(s=>((s.UnitPrice??0)*(s.Quantity??1)));
                    labOrderMaterial.Text = Math.Round(materialPrice, 2).ToString();
                    totalPrice += materialPrice;
                }

                //其他费
                decimal otherPrice = 0;
                var otherPriceOrderList = QuoteOrderDetailList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费).ToList();
                if (otherPriceOrderList.Any())
                {
                     otherPrice = otherPriceOrderList.Sum(s=>s.OrderPrice??0);
                }

                //运费/新开店装修费（非与店铺关联）
                var yunFeiOrderList = (from order in CurrentContext.DbContext.PriceOrderDetail
                                       join subject in CurrentContext.DbContext.Subject
                                       on order.SubjectId equals subject.Id
                                       where guidanceIdList.Contains(order.GuidanceId ?? 0)
                                       && subjectIdList.Contains(order.SubjectId ?? 0)
                                       && (subject.SubjectType == (int)SubjectTypeEnum.运费 || subject.SubjectType == (int)SubjectTypeEnum.新开店安装费)
                                       select new { order, subject }).ToList();
                if (yunFeiOrderList.Any())
                {
                    decimal yunfei = yunFeiOrderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.运费).Sum(s=>s.order.Amount??0);
                    decimal newShopPrice = yunFeiOrderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.新开店安装费).Sum(s => s.order.Amount ?? 0);
                    expressPrice += yunfei;
                    otherPrice += newShopPrice;
                }
                ExpressSystemPrice = expressPrice;
                labOrderExpress.Text = Math.Round(expressPrice, 2).ToString();
                totalPrice += expressPrice;
                labOrderOther.Text = Math.Round(otherPrice, 2).ToString();
                totalPrice += otherPrice;

                labOrderSub.Text = Math.Round(totalPrice, 2).ToString();


                #region 闭店
                var shutOrderList = QuoteOrderDetailList.Where(s => s.ShopStatus != null && s.ShopStatus.Contains("闭")).ToList();
                List<int> shutOrderIdList=new List<int>();
                if (shutOrderList.Any())
                {
                    shutOrderIdList=shutOrderList.Select(s=>s.Id).ToList();
                    List<int> shutShopIdList = shutOrderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                    decimal shutTotalPrice = 0;
                    decimal shutTotalArea = shutOrderList.Sum(s => (s.Area ?? 0) * (s.Quantity ?? 1));
                    decimal shutTotalPOPPrice = shutOrderList.Sum(s => (s.DefaultTotalPrice ?? 0));
                    labShutArea.Text = Math.Round(shutTotalArea, 2).ToString();
                    labShutPOPPrice.Text = Math.Round(shutTotalPOPPrice, 2).ToString();
                    shutTotalPrice += shutTotalPOPPrice;


                    //安装费
                    var shutInstallList = installList.Where(s =>shutShopIdList.Contains(s.ShopId ?? 0));
                    if (shutInstallList.Any())
                    {
                        decimal shutInstallPrice = shutInstallList.Sum(s => ((s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0)));
                        labShutInstall.Text = Math.Round(shutInstallPrice, 2).ToString();
                        shutTotalPrice += shutInstallPrice;
                    }
                    //快递费
                    var shutExpressList = expressList.Where(s => shutShopIdList.Contains(s.ShopId??0)).ToList();
                    if (shutExpressList.Any())
                    {
                        decimal shutExpressPrice = shutExpressList.Sum(s => s.ExpressPrice ?? 0);
                        labShutExpress.Text = Math.Round(shutExpressPrice, 2).ToString();
                        shutTotalPrice += shutExpressPrice;
                    }
                    //物料费用
                    var shutMaterialPriceList = materialPriceOrderList.Where(s => shutShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    if (shutMaterialPriceList.Any())
                    {
                        decimal shutMaterialPrice = shutMaterialPriceList.Sum(s => ((s.UnitPrice ?? 0) * (s.Quantity ?? 1)));
                        labShutMaterial.Text = Math.Round(shutMaterialPrice, 2).ToString();
                        shutTotalPrice += shutMaterialPrice;
                    }

                    //其他费

                    var shutOtherPriceList = otherPriceOrderList.Where(s => shutShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    if (shutOtherPriceList.Any())
                    {
                       decimal shutOtherPrice = shutOtherPriceList.Sum(s => s.OrderPrice ?? 0);
                       labShutOther.Text = Math.Round(shutOtherPrice, 2).ToString();
                       shutTotalPrice += shutOtherPrice;
                    }
                    labShutSub.Text = Math.Round(shutTotalPrice, 2).ToString();
                }
                #endregion

                #region 非闭店不生产
                decimal nonProduceTotalPrice = 0;
                decimal nonProduceTotalArea = 0;
                //无位置的
                var noPositionList = QuoteOrderDetailList.Where(s => !shutOrderIdList.Contains(s.Id) && (s.IsValid ?? true) == false).ToList();
                if (noPositionList.Any())
                {
                    decimal noPositionArea = noPositionList.Sum(s=>((s.Area??0)*(s.Quantity??1)));
                    decimal noPositionPOPPrice = noPositionList.Sum(s => s.DefaultTotalPrice ?? 0);
                    nonProduceTotalArea += noPositionArea;
                    nonProduceTotalPrice += noPositionPOPPrice;
                }
                //sum和橱窗，数量>1的不生产
                string changePOPCountSheetStr = string.Empty;
                List<string> ChangePOPCountSheetList = new List<string>();
                try
                {
                    changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                    
                }
                catch
                {

                }
                if (ChangePOPCountSheetList.Any())
                {
                    var changeNumOrderList = QuoteOrderDetailList.Where(s => !shutOrderIdList.Contains(s.Id) && (s.IsValid ?? true) == true && s.Sheet!=null && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()) && (s.Quantity??1)>1).ToList();
                    if (changeNumOrderList.Any())
                    {
                        decimal changeNumPrice = changeNumOrderList.Sum(s=>(s.DefaultTotalPrice??0)*(((s.Quantity??1)-1)/(s.Quantity??1)));
                        decimal changeNumArea = changeNumOrderList.Sum(s => (s.Area ?? 0) * (((s.Quantity ?? 1) - 1) / (s.Quantity ?? 1)));
                        nonProduceTotalArea += changeNumArea;
                        nonProduceTotalPrice += changeNumPrice;
                    }
                }
                labNonProduceArea.Text = Math.Round(nonProduceTotalArea, 2).ToString();
                labNonProducePOPPrice.Text = Math.Round(nonProduceTotalPrice, 2).ToString();
                labNonProduceSub.Text = Math.Round(nonProduceTotalPrice, 2).ToString();
                #endregion
            }
            #endregion
            #region 实际生产
            decimal produceTotalPrice = 0;
            var outsourceOrderList = new OutsourceOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId??0));
            List<int> produceShopIdList=new List<int>();
            //pop
            var outpopOrderList = outsourceOrderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具)).ToList();
            if (outpopOrderList.Any())
            {
                produceShopIdList = outpopOrderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                decimal outArea = outpopOrderList.Sum(s => ((s.Area ?? 0) * (s.Quantity ?? 1)));
                decimal outPOPPrice = outpopOrderList.Sum(s => s.TotalPrice ?? 0);
                labProduceArea.Text = Math.Round(outArea, 2).ToString();
                labProducePOPPrice.Text = Math.Round(outPOPPrice, 2).ToString();
                produceTotalPrice += outPOPPrice;
            }
            //安装费
            var outInstallOrderList = outsourceOrderList.Where(s => (s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费) && (subjectIdList.Contains(s.SubjectId ?? 0) || subjectIdList.Contains(s.BelongSubjectId??0))).ToList();
            if (outInstallOrderList.Any())
            {
                decimal installPrice = outInstallOrderList.Sum(s => s.PayOrderPrice ?? 0);
                labProduceInstall.Text = Math.Round(installPrice, 2).ToString();
                produceTotalPrice += installPrice;
            }
            //快递费
            var outExpressOrderList = outsourceOrderList.Where(s => produceShopIdList.Contains(s.ShopId ?? 0) && (s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.快递费 || s.OrderType == (int)OrderTypeEnum.运费)).ToList();
            if (outExpressOrderList.Any())
            {
                decimal expressPrice = outExpressOrderList.Sum(s => s.PayOrderPrice ?? 0);
                labProduceExpress.Text = Math.Round(expressPrice, 2).ToString();
                produceTotalPrice += expressPrice;
            }
            //其他费用
            var outOtherPriceOrderList = outsourceOrderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0) && (s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费)).ToList();
            if (outOtherPriceOrderList.Any())
            {
                decimal otherPrice = outOtherPriceOrderList.Sum(s => s.PayOrderPrice ?? 0);
                labProduceOther.Text = Math.Round(otherPrice, 2).ToString();
                produceTotalPrice += otherPrice;
            }
            labProduceSub.Text = Math.Round(produceTotalPrice, 2).ToString();
            #endregion

        }

        /// <summary>
        /// 增补订单
        /// </summary>
        void BindSupplement()
        {
            //增补
            var supplementOrderList = (from order in CurrentContext.DbContext.QuoteOrderDetail
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where (subject.SupplementSubjectId ?? 0) > 0
                             && subjectIdList.Contains(order.SubjectId ?? 0)
                             && order.QuoteItemId==quoteItemId
                             select order).ToList();
            if (supplementOrderList.Any())
            {
                decimal totalPrice = 0;
                List<int> orderIdList = supplementOrderList.Select(s => s.Id).ToList();
                //非增补店铺
                List<int> notSupplementShopIdList = quoteOrderBll.GetList(s => !orderIdList.Contains(s.Id) && subjectIdList.Contains(s.SubjectId ?? 0) && s.QuoteItemId==quoteItemId).Select(s=>s.ShopId??0).Distinct().ToList();

                List<int> supplementShopIdList = supplementOrderList.Select(s => s.ShopId ?? 0).ToList();
                decimal area = supplementOrderList.Sum(s => (s.Area ?? 0) * (s.Quantity ?? 1));
                decimal popPrice = supplementOrderList.Sum(s => s.DefaultTotalPrice ?? 0);

                decimal installPrice = 0;
                //decimal expressPrice = 0;
                List<int> supplementInstallShopIdList = supplementShopIdList.Except(notSupplementShopIdList).ToList();
                if (supplementInstallShopIdList.Any())
                {
                    //增补安装费
                    var supplementInstallPriceList = new InstallPriceShopInfoBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0) && supplementInstallShopIdList.Contains(s.ShopId??0)).ToList();
                    if (supplementInstallPriceList.Any())
                    {
                        installPrice = supplementInstallPriceList.Sum(s => ((s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0)));
                        totalPrice += installPrice;
                        //InstallSupplementPrice = installPrice;
                    }
                    //增补快递费
                }
                //增补物料费
                var supplementMaterialList = supplementOrderList.Where(s=>s.OrderType==(int)OrderTypeEnum.物料).ToList();
                if (supplementMaterialList.Any())
                {
                    decimal materialPrice = supplementMaterialList.Sum(s => ((s.UnitPrice ?? 0) * (s.Quantity ?? 1)));
                    labSupplementMaterialPrice.Text = Math.Round(materialPrice, 2).ToString();
                    totalPrice += materialPrice;
                }
                labSupplementArea.Text = Math.Round(area, 2).ToString();
                labSupplementPOPPrice.Text = Math.Round(popPrice, 2).ToString();
                labSupplementInstallPrice.Text = Math.Round(installPrice, 2).ToString();
                totalPrice += popPrice;
                labSupplementSub.Text = Math.Round(totalPrice, 2).ToString();
            }
        }

        /// <summary>
        /// 卡乐添加的
        /// </summary>
        void BindAdd()
        {
            //增补
            var supplementOrderList = (from order in CurrentContext.DbContext.QuoteOrderDetail
                                       join subject in CurrentContext.DbContext.Subject
                                       on order.SubjectId equals subject.Id
                                       where subjectIdList.Contains(order.SubjectId ?? 0)
                                       && order.QuoteItemId == quoteItemId
                                       && (order.AddSizeRate??0)>0
                                       select order).ToList();
            if (supplementOrderList.Any())
            {

                decimal totalArea = supplementOrderList.Sum(s=>(((s.TotalGraphicWidth??0)*(s.TotalGraphicLength??0))/1000000)-(((s.GraphicWidth??0)*(s.GraphicLength??0))/1000000)*(s.Quantity??1));
                decimal totalPrice = supplementOrderList.Sum(s => ((s.AutoAddTotalPrice ?? 0) - (s.DefaultTotalPrice ?? 0)));
                
                var otherQuoteList = new QuoteDifferenceDetailBLL().GetList(s => s.QuoteItemId == quoteItemId && s.OrderType==(int)OrderTypeEnum.POP);
                if (otherQuoteList.Any())
                {
                    totalArea += otherQuoteList.Sum(s => s.AddArea ?? 0);
                    totalPrice += otherQuoteList.Sum(s=>((s.AddArea??0)*(s.MaterialUnitPrice??0)));
                }
                if (totalArea > 0)
                {
                    StringBuilder url = new StringBuilder();
                    url.AppendFormat("<span name='checkSelfAddDetailSpan' style=' text-decoration:underline;cursor:pointer;color:blue;'>{0}</span>", Math.Round(totalArea, 2));
                    labSelfAddArea.Text = url.ToString();
                }
                
                labSelfAddPOPPrice.Text = Math.Round(totalPrice, 2).ToString();


                decimal installPrice=InstallQuoutPrice-InstallSystemPrice;
                if (installPrice > 0)
                {
                    labSelfAddInstallPrice.Text = Math.Round(installPrice, 2).ToString();
                    totalPrice += installPrice;
                }

                decimal expressPrice = ExpressQuotePrice - ExpressSystemPrice;
                if (expressPrice > 0)
                {
                    labSelfAddExpressPrice.Text = Math.Round(expressPrice, 2).ToString();
                    totalPrice += expressPrice;
                }


                labSelfAddSub.Text = Math.Round(totalPrice, 2).ToString();
            }
        }

       
    }
}
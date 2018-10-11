using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;
using Newtonsoft.Json;

namespace WebApp.QuoteOrderManager
{
    public partial class SubmitAuotation : BasePage
    {
        public string month = string.Empty;
        public string guidanceId = string.Empty;
        public string subjectCategory = string.Empty;
        public string subjectIdSelected = string.Empty;//不含百丽项目
        public string subjectId = string.Empty;//包含百丽项目
        public string region = string.Empty;
        public int customerId;
        public int quoteItemId;

        List<string> popSheetList = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemId"] != null)
            {
                quoteItemId = int.Parse(Request.QueryString["itemId"]);
            }
            if (Request.QueryString["month"] != null)
            {
                month = Request.QueryString["month"];
            }
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["subjectCategory"] != null)
            {
                subjectCategory = Request.QueryString["subjectCategory"];
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectIdSelected = Request.QueryString["subjectId"];
            }
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);
            }
            if (Request.QueryString["region"] != null)
            {
                region = Request.QueryString["region"];
            }
            if (ViewState["subjectId"] != null)
            {
                subjectId = ViewState["subjectId"].ToString();
            }
            if (ViewState["subjectIdSelected"] != null)
            {
                subjectIdSelected = ViewState["subjectIdSelected"].ToString();
            }
            if (ViewState["guidanceId"] != null)
            {
                guidanceId = ViewState["guidanceId"].ToString();
            }
            if (!IsPostBack)
            {
                BindData();
                BindGuidanceName();
                BindPOPList();
            }
        }


        List<int> guidanceIdList = new List<int>();
        List<int> categoryIdList = new List<int>();
        List<int> subjectIdList = new List<int>();
        List<string> regionList = new List<string>();
        QuotationItemBLL quotationBll = new QuotationItemBLL();
        List<SpecialPriceQuoteDetail> specialPriceQuoteDetailList = new List<SpecialPriceQuoteDetail>();
        int selectQuoteSubjectId = 0;
        void BindData()
        {
            QuotationItem model = quotationBll.GetModel(quoteItemId);
            if (model != null)
            {
                customerId = model.CustomerId ?? 0;
                if (model.GuidanceYear != null && model.GuidanceMonth != null)
                {
                    month = model.GuidanceYear + "-" + model.GuidanceMonth;
                }
                guidanceId = model.GuidanceId;
                subjectCategory = model.SubjectCategoryId;
                subjectId = model.SubjectIds;
                selectQuoteSubjectId = model.QuoteSubjectId ?? 0;

                specialPriceQuoteDetailList = new SpecialPriceQuoteDetailBLL().GetList(s => s.ItemId == quoteItemId);
                List<SpecialPriceQuoteDetail> popQuoteList = specialPriceQuoteDetailList.Where(s => s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.POP).ToList();

                //hfPOPQuoteJson.Value = JsonConvert.SerializeObject(popQuoteList);
                
                ViewState["guidanceId"] = guidanceId;
               
                ViewState["subjectId"] = subjectId;

            }
        }

        void BindGuidanceName()
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            labGuidanceMonth.Text = month;
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
                List<string> list = new SubjectGuidanceBLL().GetList(s => guidanceIdList.Contains(s.ItemId)).Select(s => s.ItemName).ToList();
                if (list.Any())
                {
                    labGuidanceName.Text = StringHelper.ListToString(list);
                }
            }
            if (!string.IsNullOrWhiteSpace(subjectCategory))
            {
                categoryIdList = StringHelper.ToIntList(subjectCategory, ',');
                List<string> list = new ADSubjectCategoryBLL().GetList(s => categoryIdList.Contains(s.Id)).Select(s => s.CategoryName).ToList();
                if (list.Any())
                {
                    labSubjectCategory.Text = StringHelper.ListToString(list);
                }
            }
            //List<string> subjectNameList = new List<string>();
            List<Subject> subjectList = new List<Subject>();
            if (!string.IsNullOrWhiteSpace(subjectIdSelected))
            {
                subjectIdList = StringHelper.ToIntList(subjectIdSelected, ',');
                List<int> hMakeSubjectIdList = new SubjectBLL().GetList(s => subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                subjectIdList.AddRange(hMakeSubjectIdList);
                subjectIdList = subjectIdList.Distinct().ToList();
                subjectId = StringHelper.ListToString(subjectIdList);
                ViewState["subjectId"] = subjectId;

                //subjectNameList = new SubjectBLL().GetList(s => subjectIdList.Contains(s.Id)).Select(s => s.SubjectName).OrderBy(s => s).ToList();

            }
            else if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
            }
            subjectList = new SubjectBLL().GetList(s => subjectIdList.Contains(s.Id) && (s.HandMakeSubjectId ?? 0) == 0).OrderBy(s => s.SubjectName).ToList();


            if (subjectList.Any())
            {

                if (!IsPostBack)
                {
                    StringBuilder subjectNameSr = new StringBuilder();
                    subjectList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectName;
                        ddlQuoteSubject.Items.Add(li);
                        subjectNameSr.Append(s.SubjectName);
                        subjectNameSr.Append(",");
                    });
                    labSubjectNames.Text = subjectNameSr.ToString().TrimEnd(',');
                    if (subjectList.Count == 1)
                    {
                        ddlQuoteSubject.Items[1].Selected = true;
                    }
                    else
                    {
                        if (ddlQuoteSubject.Items.FindByValue(selectQuoteSubjectId.ToString()) != null)
                        {
                            ddlQuoteSubject.SelectedValue = selectQuoteSubjectId.ToString();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 绑定POP数据
        /// </summary>
        void BindPOPList()
        {

            Session["QuoteModel"] = null;
            var orderList = (from order in CurrentContext.DbContext.QuoteOrderDetail
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where (order.IsDelete == null || order.IsDelete == false)
                             && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || (order.OrderType > (int)OrderTypeEnum.POP))
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             where guidanceIdList.Contains(order.GuidanceId ?? 0)
                             //&& (categoryIdList.Any() ? categoryIdList.Contains(subject.SubjectCategoryId ?? 0) : 1 == 1)
                             select new
                             {
                                 order,
                                 subject
                             }).ToList();
            if (regionList.Any())
            {
                orderList = orderList.Where(s => s.order.Region != null && regionList.Contains(s.order.Region.ToLower())).ToList();
            }
            if (subjectIdList.Any())
            {
                orderList = orderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
            }
            List<QuoteModel> quoteOrderList = new List<QuoteModel>();
            List<QuoteOrderDetail> popOrderList = new List<QuoteOrderDetail>();
            //保存pop的订单
            Session["AddQuoteOrderList"] = null;
            if (orderList.Any())
            {
                popOrderList = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.POP || s.order.OrderType == (int)OrderTypeEnum.道具).Select(s => s.order).ToList();
                Session["AddQuoteOrderList"] = popOrderList;
                //VVIP店铺
                var vvipList = orderList.Where(s => s.order.MaterialSupport != null && s.order.MaterialSupport.ToLower() == "vvip").ToList();
                List<int> vvipShopList = vvipList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                //非VVIP店铺
                List<int> notVVIPShopList = orderList.Where(s => !vvipShopList.Contains(s.order.ShopId ?? 0)).Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                labVVIPShop.Text = vvipShopList.Count.ToString();
                labNormalShop.Text = notVVIPShopList.Count.ToString();

                List<QuoteOrderDetail> popOrderListSave = new List<QuoteOrderDetail>();
                List<int> selectedIdList = new List<int>();

                string currSheet = string.Empty;
                string currentPosition = string.Empty;
                #region 橱窗区域
                //橱窗背景订单
                currSheet = "Window橱窗";
                currentPosition = "橱窗";
                var windowOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") || s.Sheet.Contains("窗贴")).ToList();
                if (windowOrderList.Any())
                {
                    popSheetList.Add(currentPosition);
                    popOrderListSave.AddRange(windowOrderList);
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(windowOrderList, ref windowList, currSheet, currentPosition);
                    if (windowList.Any())
                    {

                        decimal area = windowList.Sum(s => s.Amount);
                        decimal price = windowList.Sum(s => s.TotalPrice);
                        var model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                        if (model0 != null)
                        {
                            model0.Amount += area;
                            model0.TotalPrice += price;
                        }
                        else
                        {
                            QuoteModel model1 = new QuoteModel();
                            model1.Amount = area;
                            model1.Sheet = currSheet;
                            model1.TotalPrice = price;
                            model1.PositionDescription = currentPosition;
                            quoteOrderList.Add(model1);
                        }
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session[currSheet] = popOrderListSave;
                }
                #endregion
                #region 陈列桌区域
                popOrderListSave = new List<QuoteOrderDetail>();
                //陈列桌订单
                currSheet = "WOW陈列桌区域";
                currentPosition = "陈列桌";
                var tableOrderList = popOrderList.Where(s => s.Sheet.Contains("陈列桌") || s.Sheet.Contains("展桌")).ToList();
                if (tableOrderList.Any())
                {
                    popSheetList.Add(currentPosition);
                    popOrderListSave.AddRange(tableOrderList);
                    List<QuoteModel> tableList = new List<QuoteModel>();//
                    selectedIdList.AddRange(tableOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(tableOrderList, ref tableList, currSheet, currentPosition);
                    if (tableList.Any())
                    {

                        decimal area = tableList.Sum(s => s.Amount);
                        decimal price = tableList.Sum(s => s.TotalPrice);
                        var model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                        if (model0 != null)
                        {
                            model0.Amount += area;
                            model0.TotalPrice += price;
                        }
                        else
                        {
                            QuoteModel model1 = new QuoteModel();
                            model1.Amount = area;
                            model1.Sheet = currSheet;
                            model1.TotalPrice = price;
                            model1.PositionDescription = currentPosition;
                            quoteOrderList.Add(model1);
                        }
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session[currSheet] = popOrderListSave;
                }
                #endregion
                #region 服装墙区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "AppWall服装墙";
                currentPosition = "服装墙";
                var appOrderList = popOrderList.Where(s => s.Sheet.Contains(currentPosition)).ToList();
                if (appOrderList.Any())
                {
                    popSheetList.Add(currentPosition);
                    popOrderListSave.AddRange(appOrderList);
                    List<QuoteModel> appList = new List<QuoteModel>();//
                    selectedIdList.AddRange(appOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(appOrderList, ref appList, currSheet, currentPosition);
                    if (appList.Any())
                    {

                        decimal area = appList.Sum(s => s.Amount);
                        decimal price = appList.Sum(s => s.TotalPrice);
                        var model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                        if (model0 != null)
                        {
                            model0.Amount += area;
                            model0.TotalPrice += price;
                        }
                        else
                        {
                            QuoteModel model1 = new QuoteModel();
                            model1.Amount = area;
                            model1.Sheet = currSheet;
                            model1.TotalPrice = price;
                            model1.PositionDescription = currentPosition;
                            quoteOrderList.Add(model1);
                        }
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session[currSheet] = popOrderListSave;
                }
                #endregion
                #region 鞋墙区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "FTWWall鞋墙";
                currentPosition = "鞋墙";
                List<QuoteOrderDetail> ftwOrderList = new List<QuoteOrderDetail>();
                List<string> shoeWareSheetList = new List<string>() { "鞋墙", "凹槽", "圆吧", "弧形", "鞋吧", "圆桌", "吧台", "鞋砖", "鞋柱" };
                string shoeSheetStr = string.Empty;
                try
                {
                    shoeSheetStr = ConfigurationManager.AppSettings["QuoteOrderTemplateShoeSheet"];
                }
                catch
                {

                }
                if (!string.IsNullOrWhiteSpace(shoeSheetStr))
                {
                    shoeWareSheetList = StringHelper.ToStringList(shoeSheetStr, ',');
                }
                if (shoeWareSheetList.Any())
                {
                    List<int> orderIdListTemp = new List<int>();
                    shoeWareSheetList.ForEach(sh =>
                    {
                        var ftwOrderList0 = popOrderList.Where(s => s.Sheet.Contains(sh) && !orderIdListTemp.Contains(s.Id)).ToList();
                        if (ftwOrderList0.Any())
                        {
                            popOrderListSave.AddRange(ftwOrderList0);
                            ftwOrderList.AddRange(ftwOrderList0);
                            orderIdListTemp.AddRange(ftwOrderList0.Select(s => s.Id).ToList());
                        }
                    });
                }
                if (ftwOrderList.Any())
                {
                    popSheetList.Add(currentPosition);
                    List<QuoteModel> shoeList = new List<QuoteModel>();//
                    selectedIdList.AddRange(ftwOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(ftwOrderList, ref shoeList, currSheet, currentPosition);
                    if (shoeList.Any())
                    {

                        decimal area = shoeList.Sum(s => s.Amount);
                        decimal price = shoeList.Sum(s => s.TotalPrice);
                        var model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                        if (model0 != null)
                        {
                            model0.Amount += area;
                            model0.TotalPrice += price;
                        }
                        else
                        {
                            QuoteModel model1 = new QuoteModel();
                            model1.Amount = area;
                            model1.Sheet = currSheet;
                            model1.TotalPrice = price;
                            model1.PositionDescription = currentPosition;
                            quoteOrderList.Add(model1);
                        }
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session[currSheet] = popOrderListSave;
                }
                #endregion
                #region SMU区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "In-storeSMU店内其它位置SMU";
                currentPosition = "SMU";
                var smuOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("smu")).ToList();
                if (smuOrderList.Any())
                {
                    popSheetList.Add(currentPosition);
                    popOrderListSave.AddRange(smuOrderList);
                    selectedIdList.AddRange(smuOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> smuList = new List<QuoteModel>();//
                    StatisticMaterial(smuOrderList, ref smuList, currSheet, currentPosition);
                    if (smuList.Any())
                    {
                        //quoteOrderList.AddRange(smuList);
                        decimal area = smuList.Sum(s => s.Amount);
                        decimal price = smuList.Sum(s => s.TotalPrice);
                        QuoteModel model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                        if (model0 != null)
                        {
                            model0.Amount += area;
                            model0.TotalPrice += price;
                        }
                        else
                        {
                            QuoteModel model1 = new QuoteModel();
                            model1.Amount = area;
                            model1.Sheet = currSheet;
                            model1.TotalPrice = price;
                            model1.PositionDescription = currentPosition;
                            quoteOrderList.Add(model1);
                        }
                    }
                }
                #endregion
                #region 中岛
                var zdOrderList = popOrderList.Where(s => s.Sheet.Contains("中岛")).ToList();
                if (zdOrderList.Any())
                {
                    popOrderListSave.AddRange(zdOrderList);
                    selectedIdList.AddRange(zdOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> smuList = new List<QuoteModel>();//
                    StatisticMaterial(zdOrderList, ref smuList, currSheet, "中岛");
                    if (smuList.Any())
                    {
                        //quoteOrderList.AddRange(smuList);
                        decimal area = smuList.Sum(s => s.Amount);
                        decimal price = smuList.Sum(s => s.TotalPrice);
                        QuoteModel model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                        if (model0 != null)
                        {
                            model0.Amount += area;
                            model0.TotalPrice += price;
                        }
                        else
                        {
                            QuoteModel model1 = new QuoteModel();
                            model1.Amount = area;
                            model1.Sheet = currSheet;
                            model1.TotalPrice = price;
                            model1.PositionDescription = currentPosition;
                            quoteOrderList.Add(model1);
                        }
                    }
                }
                //if (popOrderListSave.Any())
                //{
                //    Session[currSheet] = popOrderListSave;
                //}
                #endregion

                #region 收银台/OOH订单
                var cashierOrderList = popOrderList.Where(s => s.Sheet == "收银台").ToList();
                if (cashierOrderList.Any())
                {
                    selectedIdList.AddRange(cashierOrderList.Select(s => s.Id).ToList());


                }

                var oohOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("ooh")).ToList();
                if (oohOrderList.Any())
                {
                    selectedIdList.AddRange(oohOrderList.Select(s => s.Id).ToList());

                }
                #endregion
                #region 除了以上常规位置外的其他位置(放SMU里面)
                var otherSheetOrderList = popOrderList.Where(s => !selectedIdList.Contains(s.Id)).ToList();
                if (otherSheetOrderList.Any())
                {
                    popOrderListSave.AddRange(otherSheetOrderList);
                    List<string> sheets = otherSheetOrderList.Select(s => s.Sheet).Distinct().ToList();
                    sheets.ForEach(sheet =>
                    {
                        var orderList0 = otherSheetOrderList.Where(s => s.Sheet == sheet).ToList();
                        List<QuoteModel> smuList = new List<QuoteModel>();//
                        StatisticMaterial(orderList0, ref smuList, currSheet, sheet);
                        if (smuList.Any())
                        {
                            decimal area = smuList.Sum(s => s.Amount);
                            decimal price = smuList.Sum(s => s.TotalPrice);
                            QuoteModel model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                            if (model0 != null)
                            {
                                model0.Amount += area;
                                model0.TotalPrice += price;
                            }
                            else
                            {
                                QuoteModel model1 = new QuoteModel();
                                model1.Amount = area;
                                model1.Sheet = currSheet;
                                model1.TotalPrice = price;
                                model1.PositionDescription = currentPosition;
                                quoteOrderList.Add(model1);
                            }

                        }
                    });

                }
                if (popOrderListSave.Any())
                {
                    Session[currSheet] = popOrderListSave;
                }
                #endregion
                #region 收银台区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "CashierDesk收银台区域";
                currentPosition = "收银台";
                //var cashierOrderList = popOrderList.Where(s => s.Sheet.Contains(currentPosition)).ToList();
                if (cashierOrderList.Any())
                {
                    popOrderListSave.AddRange(cashierOrderList);
                    selectedIdList.AddRange(cashierOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> cashierList = new List<QuoteModel>();//
                    StatisticMaterial(cashierOrderList, ref cashierList, currSheet, currentPosition);
                    if (cashierList.Any())
                    {
                        //quoteOrderList.AddRange(cashierList);
                        decimal area = cashierList.Sum(s => s.Amount);
                        decimal price = cashierList.Sum(s => s.TotalPrice);
                        QuoteModel model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                        if (model0 != null)
                        {
                            model0.Amount += area;
                            model0.TotalPrice += price;
                        }
                        else
                        {
                            QuoteModel model1 = new QuoteModel();
                            model1.Amount = area;
                            model1.Sheet = currSheet;
                            model1.TotalPrice = price;
                            model1.PositionDescription = currentPosition;
                            quoteOrderList.Add(model1);
                        }
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session[currSheet] = popOrderListSave;
                }
                #endregion
                #region OOH区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "OOH店外非橱窗位置";
                currentPosition = "OOH";
                //var oohOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("ooh")).ToList();
                if (oohOrderList.Any())
                {
                    popOrderListSave.AddRange(oohOrderList);
                    selectedIdList.AddRange(oohOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> oohList = new List<QuoteModel>();//
                    StatisticMaterial(oohOrderList, ref oohList, currSheet, currentPosition);
                    if (oohList.Any())
                    {
                        //quoteOrderList.AddRange(oohList);
                        decimal area = oohList.Sum(s => s.Amount);
                        decimal price = oohList.Sum(s => s.TotalPrice);
                        QuoteModel model0 = quoteOrderList.Where(s => s.Sheet == currSheet).FirstOrDefault();
                        if (model0 != null)
                        {
                            model0.Amount += area;
                            model0.TotalPrice += price;
                        }
                        else
                        {
                            QuoteModel model1 = new QuoteModel();
                            model1.Amount = area;
                            model1.Sheet = currSheet;
                            model1.TotalPrice = price;
                            model1.PositionDescription = currentPosition;
                            quoteOrderList.Add(model1);
                        }
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session[currSheet] = popOrderListSave;
                }
                #endregion

            }
            popList.DataSource = quoteOrderList;
            popList.DataBind();
           
            if (orderList.Any())
            {
                BindInstallPrice(orderList.Select(s => s.order).ToList());
                BindeExpressPrice(orderList.Select(s => s.order).ToList());
                BindOtherPriceOrderList(orderList.Select(s => s.order).ToList());
            }


        }

        decimal installPriceTotal = 0;//安装费合计
        decimal installPriceStandard = 0;//标准安装费合计
        List<InstallPriceQuoteModel> InstallPriceQuoteModelList = new List<InstallPriceQuoteModel>();
        List<Shop> oohInstalShopList = new List<Shop>();
        void BindInstallPrice(List<QuoteOrderDetail> orderList)
        {
            //List<string> cityTierList = new List<string>() { "T1", "T2", "T3" };
            List<int> OOHPriceList = new List<int>() { 5000, 2700, 1800, 600 };
            List<int> BasicPriceList = new List<int>() { 800, 400, 150 };
            List<OtherInstallPriceClass> otherInstallPriceList = new List<OtherInstallPriceClass>();
            OtherInstallPriceClass otherInstallPrice;
            int oohLevelOneCount = 0;//5000级别
            int oohLevelOnePrice = 0;
            int oohLevelTwoCount = 0;//2700级别
            int oohLevelTwoPrice = 0;
            int oohLevelThreeCount = 0;//1800级别
            int oohLevelThreePrice = 0;
            int oohLevelFourCount = 0;//600级别
            int oohLevelFourPrice = 0;

            //decimal otherOOHInstallPrice = 0;

            int windowLevelOneCount = 0;
            int windowLevelOnePrice = 0;
            int windowLevelTwoCount = 0;
            int windowLevelTwoPrice = 0;
            int windowLevelThreeCount = 0;
            int windowLevelThreePrice = 0;
            int kidsWindowCount = 0;
            int kidsWindowPrice = 0;

            int basicLevelOneCount = 0;
            int basicLevelOnePrice = 0;
            int basicLevelTwoCount = 0;
            int basicLevelTwoPrice = 0;
            int basicLevelThreeCount = 0;
            int basicLevelThreePrice = 0;
            int kidsBasicCount = 0;
            int kidsBasicPrice = 0;

            int genericLevelCount = 0;
            int genericLevelPrice = 0;

            //decimal otherBasicInstallPrice = 0;
            List<int> installShopList = new List<int>();
            guidanceIdList.ForEach(gid =>
            {
                #region 活动安装费（归类）
                List<int> installShopIdList = orderList.Where(s => s.GuidanceId == gid).Select(s => s.ShopId ?? 0).Distinct().ToList();

                var installPriceList = (from install in CurrentContext.DbContext.InstallPriceShopInfo
                                        join shop in CurrentContext.DbContext.Shop
                                        on install.ShopId equals shop.Id
                                        where install.GuidanceId == gid
                                        && (subjectIdList.Any() ? subjectIdList.Contains(install.SubjectId ?? 0) : 1 == 1)
                                        //&& installShopIdList.Contains(install.ShopId ?? 0)
                                        select new
                                        {
                                            install,
                                            shop
                                        }).ToList();
                if (quoteItemId == 0)
                {
                    installPriceList = installPriceList.Where(s => (s.install.QuoteItemId ?? 0) == 0).ToList();
                }
                if (installPriceList.Any())
                {
                    installShopList.AddRange(installPriceList.Select(s => s.install.ShopId ?? 0).Distinct().ToList());
                    installPriceTotal = installPriceList.Sum(s => ((s.install.BasicPrice ?? 0) + (s.install.OOHPrice ?? 0) + (s.install.WindowPrice ?? 0)));
                    installPriceList.ForEach(s =>
                    {
                        InstallPriceQuoteModel installQuoteModel;
                        string materialSupport = (s.install.MaterialSupport != null) ? (s.install.MaterialSupport.ToLower()) : string.Empty;
                        if (materialSupport == "others")
                        {
                            //童店安装费
                            if ((s.install.WindowPrice ?? 0) > 0)
                            {
                                kidsWindowCount++;
                                kidsWindowPrice += 500;
                                installPriceStandard += 500;
                            }
                            else if ((s.install.BasicPrice ?? 0) > 0)
                            {
                                kidsBasicCount++;
                                kidsBasicPrice += 150;
                                installPriceStandard += 150;
                            }
                            bool flag = false;
                            for (int i = 0; i < InstallPriceQuoteModelList.Count; i++)
                            {
                                installQuoteModel = InstallPriceQuoteModelList[i];
                                if (installQuoteModel.ChargeItem == "BasicInstall" && installQuoteModel.ChargeType == "kids" && installQuoteModel.UnitPrice == (s.install.BasicPrice ?? 0))
                                {
                                    installQuoteModel.Amount++;
                                    flag = true;
                                }

                            }
                            if (!flag)
                            {
                                installQuoteModel = new InstallPriceQuoteModel();
                                installQuoteModel.Amount = 1;
                                installQuoteModel.ChargeItem = "BasicInstall";
                                installQuoteModel.ChargeType = "kids";
                                installQuoteModel.UnitPrice = (s.install.BasicPrice ?? 0);
                                InstallPriceQuoteModelList.Add(installQuoteModel);
                            }
                        }
                        else
                        {
                            #region 大货/常规
                            //OOH
                            if ((s.install.OOHPrice ?? 0) > 0)
                            {
                                //installPriceTotal += (s.install.OOHPrice ?? 0);
                                bool isExist0 = false;
                                for (int i = 0; i < oohInstalShopList.Count; i++)
                                {
                                    if (oohInstalShopList[i].Id == s.install.ShopId)
                                    {
                                        oohInstalShopList[i].OOHInstallPrice += (s.install.OOHPrice ?? 0);
                                        isExist0 = true;
                                    }
                                }
                                if (!isExist0)
                                {
                                    Shop shop = new Shop();
                                    shop = s.shop;
                                    shop.OOHInstallPrice = s.install.OOHPrice ?? 0;
                                    oohInstalShopList.Add(shop);
                                }

                                if (s.install.OOHPrice == 5000)
                                {
                                    oohLevelOneCount++;
                                    oohLevelOnePrice += 5000;
                                    installPriceStandard += 5000;

                                }
                                else if (s.install.OOHPrice == 2700)
                                {
                                    oohLevelTwoCount++;
                                    oohLevelTwoPrice += 2700;
                                    installPriceStandard += 2700;
                                }
                                else if (s.install.OOHPrice == 1800)
                                {
                                    oohLevelThreeCount++;
                                    oohLevelThreePrice += 1800;
                                    installPriceStandard += 1800;
                                }
                                else if (s.install.OOHPrice == 600)
                                {
                                    oohLevelFourCount++;
                                    oohLevelFourPrice += 600;
                                    installPriceStandard += 600;
                                }
                                else//其他高空安装费
                                {
                                    //otherOOHInstallPrice += (s.install.OOHPrice ?? 0);
                                    bool isExist = false;
                                    int index = 0;

                                    foreach (var oh in otherInstallPriceList)
                                    {
                                        if (oh.PriceType == "户外安装费" && oh.Price == (s.install.OOHPrice ?? 0))
                                        {
                                            isExist = true;
                                            break;
                                        }
                                        index++;
                                    }
                                    if (isExist)
                                    {
                                        otherInstallPriceList[index].Count++;
                                    }
                                    else
                                    {
                                        otherInstallPrice = new OtherInstallPriceClass();
                                        otherInstallPrice.Count = 1;
                                        otherInstallPrice.Price = (s.install.OOHPrice ?? 0);
                                        otherInstallPrice.PriceType = "户外安装费";
                                        otherInstallPriceList.Add(otherInstallPrice);
                                    }
                                }
                                bool flag = false;
                                for (int i = 0; i < InstallPriceQuoteModelList.Count; i++)
                                {
                                    installQuoteModel = InstallPriceQuoteModelList[i];
                                    if (installQuoteModel.ChargeItem == "OOHInstall" && installQuoteModel.ChargeType == (s.install.OOHPrice ?? 0).ToString() && installQuoteModel.UnitPrice == (s.install.OOHPrice ?? 0))
                                    {
                                        installQuoteModel.Amount++;
                                        flag = true;
                                    }

                                }
                                if (!flag)
                                {
                                    installQuoteModel = new InstallPriceQuoteModel();
                                    installQuoteModel.Amount = 1;
                                    installQuoteModel.ChargeItem = "OOHInstall";
                                    installQuoteModel.ChargeType = (s.install.OOHPrice ?? 0).ToString();
                                    installQuoteModel.UnitPrice = (s.install.OOHPrice ?? 0);
                                    InstallPriceQuoteModelList.Add(installQuoteModel);
                                }
                            }
                            if ((s.install.WindowPrice ?? 0) > 0)
                            {
                                //installPriceTotal += (s.install.WindowPrice ?? 0);
                                //橱窗
                                if (s.install.WindowPrice == 1000)
                                {
                                    windowLevelOneCount++;
                                    windowLevelOnePrice += 1000;
                                    installPriceStandard += 1000;
                                }
                                else if (s.install.WindowPrice == 500)
                                {
                                    windowLevelTwoCount++;
                                    windowLevelTwoPrice += 500;
                                    installPriceStandard += 500;
                                }
                                else if (s.install.WindowPrice == 200)
                                {
                                    windowLevelThreeCount++;
                                    windowLevelThreePrice += 200;
                                    installPriceStandard += 200;
                                }
                                bool flag = false;
                                for (int i = 0; i < InstallPriceQuoteModelList.Count; i++)
                                {
                                    installQuoteModel = InstallPriceQuoteModelList[i];
                                    if (installQuoteModel.ChargeItem == "WindowInstall" && installQuoteModel.ChargeType == (s.install.WindowPrice ?? 0).ToString() && installQuoteModel.UnitPrice == (s.install.WindowPrice ?? 0))
                                    {
                                        installQuoteModel.Amount++;
                                        flag = true;
                                    }

                                }
                                if (!flag)
                                {
                                    installQuoteModel = new InstallPriceQuoteModel();
                                    installQuoteModel.Amount = 1;
                                    installQuoteModel.ChargeItem = "WindowInstall";
                                    installQuoteModel.ChargeType = (s.install.WindowPrice ?? 0).ToString();
                                    installQuoteModel.UnitPrice = (s.install.WindowPrice ?? 0);
                                    InstallPriceQuoteModelList.Add(installQuoteModel);
                                }
                            }
                            if ((s.install.BasicPrice ?? 0) > 0)
                            {
                                //installPriceTotal += (s.install.BasicPrice ?? 0);
                                //店内
                                if (materialSupport.Contains("generic"))
                                {
                                    genericLevelCount++;
                                    genericLevelPrice += 150;
                                    installPriceStandard += 150;
                                    bool flag = false;
                                    for (int i = 0; i < InstallPriceQuoteModelList.Count; i++)
                                    {
                                        installQuoteModel = InstallPriceQuoteModelList[i];
                                        if (installQuoteModel.ChargeItem == "BasicInstall" && installQuoteModel.ChargeType == "generic" && installQuoteModel.UnitPrice == (s.install.BasicPrice ?? 0))
                                        {
                                            installQuoteModel.Amount++;
                                            flag = true;
                                        }

                                    }
                                    if (!flag)
                                    {
                                        installQuoteModel = new InstallPriceQuoteModel();
                                        installQuoteModel.Amount = 1;
                                        installQuoteModel.ChargeItem = "BasicInstall";
                                        installQuoteModel.ChargeType = "generic";
                                        installQuoteModel.UnitPrice = (s.install.BasicPrice ?? 0);
                                        InstallPriceQuoteModelList.Add(installQuoteModel);
                                    }
                                }
                                else
                                {
                                    if (s.install.BasicPrice == 800)
                                    {
                                        basicLevelOneCount++;
                                        basicLevelOnePrice += 800;
                                        installPriceStandard += 800;
                                    }
                                    else if (s.install.BasicPrice == 400)
                                    {
                                        basicLevelTwoCount++;
                                        basicLevelTwoPrice += 400;
                                        installPriceStandard += 400;
                                    }
                                    else if (s.install.BasicPrice == 150)
                                    {
                                        basicLevelThreeCount++;
                                        basicLevelThreePrice += 150;
                                        installPriceStandard += 150;
                                    }
                                    else//其他级别的店铺安装费
                                    {
                                        //otherBasicInstallPrice += (s.install.BasicPrice ?? 0);
                                        bool isExist = false;
                                        int index = 0;
                                        foreach (var oh in otherInstallPriceList)
                                        {
                                            if (oh.PriceType == "店铺安装费" && oh.Price == (s.install.BasicPrice ?? 0))
                                            {
                                                isExist = true;
                                                break;
                                            }
                                            index++;
                                        }
                                        if (isExist)
                                        {
                                            otherInstallPriceList[index].Count++;
                                        }
                                        else
                                        {
                                            otherInstallPrice = new OtherInstallPriceClass();
                                            otherInstallPrice.Count = 1;
                                            otherInstallPrice.Price = (s.install.BasicPrice ?? 0);
                                            otherInstallPrice.PriceType = "店铺安装费";
                                            otherInstallPriceList.Add(otherInstallPrice);
                                        }
                                    }
                                    bool flag = false;
                                    for (int i = 0; i < InstallPriceQuoteModelList.Count; i++)
                                    {
                                        installQuoteModel = InstallPriceQuoteModelList[i];
                                        if (installQuoteModel.ChargeItem == "BasicInstall" && installQuoteModel.ChargeType == (s.install.BasicPrice ?? 0).ToString() && installQuoteModel.UnitPrice == (s.install.BasicPrice ?? 0))
                                        {
                                            installQuoteModel.Amount++;
                                            flag = true;
                                        }

                                    }
                                    if (!flag)
                                    {
                                        installQuoteModel = new InstallPriceQuoteModel();
                                        installQuoteModel.Amount = 1;
                                        installQuoteModel.ChargeItem = "BasicInstall";
                                        installQuoteModel.ChargeType = (s.install.BasicPrice ?? 0).ToString();
                                        installQuoteModel.UnitPrice = (s.install.BasicPrice ?? 0);
                                        InstallPriceQuoteModelList.Add(installQuoteModel);
                                    }
                                }
                            }
                            #endregion
                        }
                    });
                }


                #endregion

                #region 安装费订单
                var installnstallPriceOrderList = orderList.Where(s => s.GuidanceId == gid && (subjectIdList.Any() ? subjectIdList.Contains(s.SubjectId ?? 0) : 1 == 1) && s.OrderType == (int)OrderTypeEnum.安装费 && (s.OrderPrice ?? 0) > 0).ToList();
                if (quoteItemId == 0)
                {
                    installnstallPriceOrderList = installnstallPriceOrderList.Where(s => (s.QuoteItemId ?? 0) == 0).ToList();
                }
                if (installnstallPriceOrderList.Any())
                {
                    List<decimal> orderInstallPriceList = installnstallPriceOrderList.Select(s => s.OrderPrice ?? 0).ToList();
                    orderInstallPriceList.ForEach(price =>
                    {
                        if (price == 800)
                        {
                            basicLevelOneCount++;
                            basicLevelOnePrice += 800;
                            installPriceStandard += 800;
                        }
                        else if (price == 400)
                        {
                            basicLevelTwoCount++;
                            basicLevelTwoPrice += 400;
                            installPriceStandard += 400;
                        }
                        else if (price == 150)
                        {
                            basicLevelThreeCount++;
                            basicLevelThreePrice += 150;
                            installPriceStandard += 150;
                        }
                        else
                        {
                            bool isExist = false;
                            int index = 0;
                            foreach (var oh in otherInstallPriceList)
                            {
                                if (oh.PriceType == "店铺安装费" && oh.Price == price)
                                {
                                    isExist = true;
                                    break;
                                }
                                index++;
                            }
                            if (isExist)
                            {
                                otherInstallPriceList[index].Count++;
                            }
                            else
                            {
                                otherInstallPrice = new OtherInstallPriceClass();
                                otherInstallPrice.Count = 1;
                                otherInstallPrice.Price = price;
                                otherInstallPrice.PriceType = "店铺安装费";
                                otherInstallPriceList.Add(otherInstallPrice);
                            }
                        }
                    });
                }
                #endregion
            });
            #region 赋值
            labInstallShop.Text = installShopList.Distinct().Count().ToString();

            string OOHLevel1 = "0";
            if (oohLevelOneCount > 0)
            {
                OOHLevel1 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='ooh' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 5000, oohLevelOneCount);
            }
            labOOHLevel1Count.Text = OOHLevel1;
            labOOHLevel1.Text = oohLevelOnePrice.ToString();


            string OOHLevel2 = "0";
            if (oohLevelTwoCount > 0)
            {
                OOHLevel2 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='ooh' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 2700, oohLevelTwoCount);
            }
            labOOHLevel2Count.Text = OOHLevel2;
            labOOHLevel2.Text = oohLevelTwoPrice.ToString();


            string OOHLevel3 = "0";
            if (oohLevelThreeCount > 0)
            {
                OOHLevel3 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='ooh' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 1800, oohLevelThreeCount);
            }
            labOOHLevel3Count.Text = OOHLevel3;
            labOOHLevel3.Text = oohLevelThreePrice.ToString();


            string OOHLevel4 = "0";
            if (oohLevelFourCount > 0)
            {
                OOHLevel4 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='ooh' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 600, oohLevelFourCount);
            }
            labOOHLevel4Count.Text = OOHLevel4;
            labOOHLevel4.Text = oohLevelFourPrice.ToString();


            string WindowLevel1 = "0";
            if (windowLevelOneCount > 0)
            {
                WindowLevel1 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='window' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 1000, windowLevelOneCount);
            }
            labWindowLevel1Count.Text = WindowLevel1;
            labWindowLevel1.Text = windowLevelOnePrice.ToString();


            string BasicLevel1 = "0";
            if (basicLevelOneCount > 0)
            {
                BasicLevel1 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='basic' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 800, basicLevelOneCount);
            }
            labBasicLevel1Count.Text = BasicLevel1;
            labBasicLevel1.Text = basicLevelOnePrice.ToString();


            string WindowLevel2 = "0";
            if (windowLevelTwoCount > 0)
            {
                WindowLevel2 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='window' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 500, windowLevelTwoCount);
            }
            labWindowLevel2Count.Text = WindowLevel2;
            labWindowLevel2.Text = windowLevelTwoPrice.ToString();


            string BasicLevel2 = "0";
            if (basicLevelTwoCount > 0)
            {
                BasicLevel2 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='basic' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 400, basicLevelTwoCount);
            }
            labBasicLevel2Count.Text = BasicLevel2;
            labBasicLevel2.Text = basicLevelTwoPrice.ToString();

            string WindowLevel3 = "0";
            if (windowLevelThreeCount > 0)
            {
                WindowLevel3 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='window' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 200, windowLevelThreeCount);
            }
            labWindowLevel3Count.Text = WindowLevel3;
            labWindowLevel3.Text = windowLevelThreePrice.ToString();

            string BasicLevel3 = "0";
            if (basicLevelThreeCount > 0)
            {
                BasicLevel3 = string.Format("<span name='checkQuoteInstallPriceSpan' data-leveltype='basic' data-price='{0}' style='color:blue;cursor:pointer;text-decoration:underline;'>{1}</span>", 150, basicLevelThreeCount);
            }
            labBasicLevel3Count.Text = BasicLevel3;
            labBasicLevel3.Text = basicLevelThreePrice.ToString();


            labKidsWindowLevelCount.Text = kidsWindowCount.ToString();
            labKidsWindowLevel.Text = kidsWindowPrice.ToString();



            labKidsBasicLevelCount.Text = kidsBasicCount.ToString();
            labKidsBasicLevel.Text = kidsBasicPrice.ToString();


            labGenericLevelCount.Text = genericLevelCount.ToString();
            labGenericLevel.Text = genericLevelPrice.ToString();


            labInstallPriceTotal.Text = installPriceStandard.ToString();

           

            #endregion

            #region 其他级别安装费
            otherInstallPriceRepeater.DataSource = otherInstallPriceList.OrderByDescending(s => s.PriceType).ToList();
            otherInstallPriceRepeater.DataBind();
            OtherInstallPriceCombineCell();
            #endregion
            if (InstallPriceQuoteModelList.Any())
            {
                Session["InstallPriceQuoteModel"] = InstallPriceQuoteModelList;
            }
            if (oohInstalShopList.Any())
            {
                Session["AddQuoteOOHInstalShopList"] = oohInstalShopList;
            }
           
        }

        void OtherInstallPriceCombineCell()
        {
            for (int i = otherInstallPriceRepeater.Items.Count - 1; i > 0; i--)
            {
                HtmlTableCell cell1 = otherInstallPriceRepeater.Items[i - 1].FindControl("priceType") as HtmlTableCell;
                HtmlTableCell cell2 = otherInstallPriceRepeater.Items[i].FindControl("priceType") as HtmlTableCell;
                cell1.RowSpan = (cell1.RowSpan == -1) ? 1 : cell1.RowSpan;
                cell2.RowSpan = (cell2.RowSpan == -1) ? 1 : cell2.RowSpan;
                if (cell2.InnerText == cell1.InnerText)
                {
                    cell2.Visible = false;
                    cell1.RowSpan += cell2.RowSpan;
                    HtmlTableCell cell3 = otherInstallPriceRepeater.Items[i - 1].FindControl("subPrice") as HtmlTableCell;
                    HtmlTableCell cell4 = otherInstallPriceRepeater.Items[i].FindControl("subPrice") as HtmlTableCell;
                    cell3.RowSpan = (cell3.RowSpan == -1) ? 1 : cell3.RowSpan;
                    cell4.RowSpan = (cell4.RowSpan == -1) ? 1 : cell4.RowSpan;

                    string price1Str = ((Label)cell3.FindControl("labSubPrice")).Text;
                    string price2Str = ((Label)cell4.FindControl("labSubPrice")).Text;

                    decimal price1 = !string.IsNullOrWhiteSpace(price1Str) ? decimal.Parse(price1Str) : 0;
                    decimal price2 = !string.IsNullOrWhiteSpace(price2Str) ? decimal.Parse(price2Str) : 0;

                    cell4.Visible = false;
                    cell3.RowSpan += cell4.RowSpan;
                    ((Label)cell3.FindControl("labSubPrice")).Text = (price1 + price2).ToString();


                    HtmlTableCell cell33 = otherInstallPriceRepeater.Items[i - 1].FindControl("subPriceLeft") as HtmlTableCell;
                    HtmlTableCell cell44 = otherInstallPriceRepeater.Items[i].FindControl("subPriceLeft") as HtmlTableCell;
                    cell33.RowSpan = (cell33.RowSpan == -1) ? 1 : cell33.RowSpan;
                    cell44.RowSpan = (cell44.RowSpan == -1) ? 1 : cell44.RowSpan;

                    string price11Str = ((Label)cell33.FindControl("labsubPriceLeft")).Text;
                    string price22Str = ((Label)cell44.FindControl("labsubPriceLeft")).Text;

                    decimal price11 = !string.IsNullOrWhiteSpace(price1Str) ? decimal.Parse(price1Str) : 0;
                    decimal price22 = !string.IsNullOrWhiteSpace(price2Str) ? decimal.Parse(price2Str) : 0;

                    cell44.Visible = false;
                    cell33.RowSpan += cell44.RowSpan;
                    ((Label)cell33.FindControl("labsubPriceLeft")).Text = (price11 + price22).ToString();


                    HtmlTableCell cell5 = otherInstallPriceRepeater.Items[i - 1].FindControl("operatePrice") as HtmlTableCell;
                    HtmlTableCell cell6 = otherInstallPriceRepeater.Items[i].FindControl("operatePrice") as HtmlTableCell;
                    cell5.RowSpan = (cell5.RowSpan == -1) ? 1 : cell5.RowSpan;
                    cell6.RowSpan = (cell6.RowSpan == -1) ? 1 : cell6.RowSpan;
                    cell6.Visible = false;
                    cell5.RowSpan += cell6.RowSpan;
                }
            }
        }

        /// <summary>
        /// 系统订单快递费和物料费
        /// </summary>
        /// <param name="orderList"></param>
        void BindeExpressPrice(List<QuoteOrderDetail> orderList)
        {
            List<ExpressPriceClass> expressPriceList = new List<ExpressPriceClass>();
            ExpressPriceClass expressModel;
            //快递费订单
            var expressPriceOrderList = orderList.Where(s => (s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.快递费 || s.OrderType == (int)OrderTypeEnum.运费) && (s.OrderPrice ?? 0) > 0).ToList();
            if (expressPriceOrderList.Any())
            {
                expressPriceOrderList.ForEach(s =>
                {
                    //expressTotalPrice += (s.OrderPrice ?? 0);
                    bool isExist = false;
                    int index = 0;
                    foreach (var ex in expressPriceList)
                    {
                        if (ex.PriceType == "快递费" && ex.Price == (s.OrderPrice ?? 0))
                        {
                            isExist = true;
                            break;
                        }
                        index++;
                    }
                    if (isExist)
                    {
                        expressPriceList[index].Count++;
                    }
                    else
                    {
                        expressModel = new ExpressPriceClass();
                        expressModel.Count = 1;
                        expressModel.Price = (s.OrderPrice ?? 0);
                        expressModel.PriceType = OrderTypeEnum.快递费.ToString();
                        expressModel.OrderType = OrderTypeEnum.快递费.ToString();
                        expressPriceList.Add(expressModel);
                    }

                });
            }
            //活动里面的快递费（系统自动生成的）
            guidanceIdList.ForEach(gid =>
            {
                SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(gid);
                if (guidanceModel != null && (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion || guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery))
                {
                    //活动快递费
                    List<int> expressPriceShopIdList = orderList.Where(s => s.GuidanceId == gid).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    var expressList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == gid && expressPriceShopIdList.Contains(s.ShopId ?? 0));
                    if (quoteItemId == 0)
                    {
                        expressList = expressList.Where(s => (s.QuoteItemId ?? 0) == 0).ToList();
                    }
                    if (expressList.Any())
                    {
                        expressList.ForEach(s =>
                        {
                            //expressTotalPrice += (s.ExpressPrice ?? 0);
                            bool isExist = false;
                            int index = 0;
                            foreach (var ex in expressPriceList)
                            {
                                if (ex.PriceType == "快递费" && ex.Price == (s.ExpressPrice ?? 0))
                                {
                                    isExist = true;
                                    break;
                                }
                                index++;
                            }
                            if (isExist)
                            {
                                expressPriceList[index].Count++;
                            }
                            else
                            {
                                expressModel = new ExpressPriceClass();
                                expressModel.Count = 1;
                                expressModel.Price = (s.ExpressPrice ?? 0);
                                expressModel.PriceType = OrderTypeEnum.快递费.ToString();
                                expressModel.OrderType = OrderTypeEnum.快递费.ToString();
                                expressPriceList.Add(expressModel);
                            }
                        });
                    }
                }
            });
            //物料费
            var materailOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.物料 && (s.OrderPrice ?? 0) > 0).ToList();
            if (materailOrderList.Any())
            {
                materailOrderList.ForEach(s =>
                {
                    //expressTotalPrice += ((s.UnitPrice ?? 0) * (s.Quantity ?? 1));
                    bool isExist = false;
                    int index = 0;
                    foreach (var ex in expressPriceList)
                    {
                        if (ex.PriceType == s.Sheet && ex.Price == (s.UnitPrice ?? 0))
                        {
                            isExist = true;
                            break;
                        }
                        index++;
                    }
                    if (isExist)
                    {
                        expressPriceList[index].Count = expressPriceList[index].Count + (s.Quantity ?? 1);
                    }
                    else
                    {
                        expressModel = new ExpressPriceClass();
                        expressModel.Count = s.Quantity ?? 1;
                        expressModel.Price = (s.UnitPrice ?? 0);
                        expressModel.PriceType = s.Sheet;
                        expressModel.OrderType = OrderTypeEnum.物料.ToString();
                        expressPriceList.Add(expressModel);
                    }

                });
            }
            expressPriceListRepeater.DataSource = expressPriceList;
            expressPriceListRepeater.DataBind();
            if (expressPriceList.Any())
            {
                Session["ExpressPriceQuoteModel"] = expressPriceList;
            }
        }



        /// <summary>
        /// 其他费用订单/印刷费订单
        /// </summary>
        void BindOtherPriceOrderList(List<QuoteOrderDetail> orderList)
        {
            List<OtherInstallPriceClass> otherOrderlist = new List<OtherInstallPriceClass>();
            //List<QuoteOrderDetail> orderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费));
            //if()
            orderList = orderList.Where(s => (s.OrderType == (int)OrderTypeEnum.其他费用 || s.OrderType == (int)OrderTypeEnum.印刷费)).ToList();
            if (subjectIdList.Any())
            {
                orderList = orderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
            }
            if (orderList.Any())
            {

                OtherInstallPriceClass model;

                var list = (from order in orderList
                            group order by order.OrderType
                                into g
                                select new
                                {
                                    PriceType = g.Key,
                                    Count = g.Select(s => s.ShopId ?? 0).Distinct().Count(),
                                    TotalPrice = g.Sum(s => (s.OrderPrice ?? 0) * (s.Quantity ?? 1))
                                }).ToList();
                if (list.Any())
                {
                    list.ForEach(s =>
                    {
                        model = new OtherInstallPriceClass();
                        model.Count = s.Count;
                        model.ChangeType = s.PriceType ?? 0;
                        model.PriceType = CommonMethod.GeEnumName<OrderTypeEnum>((s.PriceType ?? 0).ToString());
                        model.TotalPrice = s.TotalPrice;
                        otherOrderlist.Add(model);
                        installPriceTotal += s.TotalPrice;
                    });
                }
            }
            otherPriceOrderRepeater.DataSource = otherOrderlist;
            otherPriceOrderRepeater.DataBind();
        }


        protected void btnExport_Click(object sender, EventArgs e)
        {

        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        decimal popTotalPrice = 0;
        decimal popTotalArea = 0;
        protected void popList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                QuoteModel model = e.Item.DataItem as QuoteModel;
                if (model != null)
                {
                    popTotalPrice += model.TotalPrice;
                    popTotalArea += model.Amount;
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labPOPTotalArea")).Text = Math.Round(popTotalArea, 2).ToString();
                ((Label)e.Item.FindControl("labPOPTotalPrice")).Text = Math.Round(popTotalPrice, 2).ToString();
            }
        }

        decimal expressTotalPrice = 0;
        protected void expressPriceListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                ExpressPriceClass item = e.Item.DataItem as ExpressPriceClass;
                if (item != null)
                {
                    decimal subPrice = item.Count * item.Price;
                    expressTotalPrice += subPrice;
                    ((Label)e.Item.FindControl("labSubPrice")).Text = subPrice.ToString();

                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labExpressPriceTotal")).Text = expressTotalPrice.ToString();
            }
        }


        int specialTotal = 0;
        int oohIndex = 0;
        int basicIndex = 0;
        protected void otherInstallPriceRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                OtherInstallPriceClass item = e.Item.DataItem as OtherInstallPriceClass;
                if (item != null)
                {
                    decimal subPrice = item.Count * item.Price;
                    ((Label)e.Item.FindControl("labPrice")).Text = subPrice.ToString();
                    ((Label)e.Item.FindControl("labSubPrice")).Text = subPrice.ToString();
                    ((Label)e.Item.FindControl("labSubPriceLeft")).Text = subPrice.ToString();

                    if (popSheetList.Any())
                    {
                        DropDownList ddlSheet = (DropDownList)e.Item.FindControl("ddlSheet");
                        popSheetList.ForEach(s =>
                        {
                            ListItem li = new ListItem();
                            li.Text = s;
                            li.Value = s;
                            ddlSheet.Items.Add(li);
                        });
                    }
                    if (!materialList.Any())
                    {
                        GetCustomerMaterial();
                    }
                    DropDownList ddlMaterial = (DropDownList)e.Item.FindControl("ddlMaterial");
                    materialList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s.MaterialName + "(" + s.Price + ")";
                        li.Value = s.MaterialName + "-" + s.Price;
                        ddlMaterial.Items.Add(li);
                    });

                    Panel panelOOH = (Panel)e.Item.FindControl("PanelOOH");
                    Panel panelBasic = (Panel)e.Item.FindControl("PanelBasic");
                    HiddenField hfChangeType = (HiddenField)e.Item.FindControl("hfChangeType");
                    if (item.PriceType == "户外安装费")
                    {
                        hfChangeType.Value = ((int)QuoteInstallPriceChangeTypeEnum.OOH).ToString();
                        panelBasic.Visible = false;
                        var specialPriceDetailList0 = specialPriceQuoteDetailList.Where(s => s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.OOH).ToList();
                        if (specialPriceDetailList0.Any())
                        {
                            TextBox txtOOHPriceCount1 = (TextBox)e.Item.FindControl("txtOOHPriceCount1");
                            TextBox txtOOHPriceCount2 = (TextBox)e.Item.FindControl("txtOOHPriceCount2");
                            TextBox txtOOHPriceCount3 = (TextBox)e.Item.FindControl("txtOOHPriceCount3");
                            TextBox txtOOHPriceCount4 = (TextBox)e.Item.FindControl("txtOOHPriceCount4");
                            Label labOOHTotal = (Label)e.Item.FindControl("labOOHTotal");
                            int total = 0;
                            var level1 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 5000).FirstOrDefault();
                            if (level1 != null)
                            {
                                txtOOHPriceCount1.Text = (level1.Quantity ?? 0).ToString();
                                txtOOHPriceCount1.ForeColor = System.Drawing.Color.Red;
                                total += (level1.Quantity ?? 0) * (level1.InstallPriceLevel ?? 0); ;
                            }
                            var level2 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 2700).FirstOrDefault();
                            if (level2 != null)
                            {
                                txtOOHPriceCount2.Text = (level2.Quantity ?? 0).ToString();
                                txtOOHPriceCount2.ForeColor = System.Drawing.Color.Red;
                                total += (level2.Quantity ?? 0) * (level2.InstallPriceLevel ?? 0);
                            }
                            var level3 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 1800).FirstOrDefault();
                            if (level3 != null)
                            {
                                txtOOHPriceCount3.Text = (level3.Quantity ?? 0).ToString();
                                txtOOHPriceCount3.ForeColor = System.Drawing.Color.Red;
                                total += (level3.Quantity ?? 0) * (level3.InstallPriceLevel ?? 0);
                            }
                            var level4 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 600).FirstOrDefault();
                            if (level4 != null)
                            {
                                txtOOHPriceCount4.Text = (level4.Quantity ?? 0).ToString();
                                txtOOHPriceCount4.ForeColor = System.Drawing.Color.Red;
                                total += (level4.Quantity ?? 0) * (level4.InstallPriceLevel ?? 0);
                            }
                            if (total > 0)
                            {
                                labOOHTotal.Text = total.ToString();
                                labOOHTotal.ForeColor = System.Drawing.Color.Blue;
                            }
                            if (oohIndex == 0)
                            {
                                specialTotal += total;
                                oohIndex++;
                            }

                        }
                    }
                    else
                    {
                        hfChangeType.Value = ((int)QuoteInstallPriceChangeTypeEnum.Basic).ToString();
                        panelOOH.Visible = false;
                        var specialPriceDetailList0 = specialPriceQuoteDetailList.Where(s => s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.Basic).ToList();
                        if (specialPriceDetailList0.Any())
                        {
                            TextBox txtBasicPriceCount1 = (TextBox)e.Item.FindControl("txtBasicPriceCount1");
                            TextBox txtBasicPriceCount2 = (TextBox)e.Item.FindControl("txtBasicPriceCount2");
                            TextBox txtBasicPriceCount3 = (TextBox)e.Item.FindControl("txtBasicPriceCount3");

                            Label labBasicTotal = (Label)e.Item.FindControl("labBasicTotal");
                            int total = 0;
                            var level1 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 800).FirstOrDefault();
                            if (level1 != null)
                            {
                                txtBasicPriceCount1.Text = (level1.Quantity ?? 0).ToString();
                                txtBasicPriceCount1.ForeColor = System.Drawing.Color.Red;
                                total += (level1.Quantity ?? 0) * (level1.InstallPriceLevel ?? 0);
                            }
                            var level2 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 400).FirstOrDefault();
                            if (level2 != null)
                            {
                                txtBasicPriceCount2.Text = (level2.Quantity ?? 0).ToString();
                                txtBasicPriceCount2.ForeColor = System.Drawing.Color.Red;
                                total += (level2.Quantity ?? 0) * (level2.InstallPriceLevel ?? 0);
                            }
                            var level3 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 150).FirstOrDefault();
                            if (level3 != null)
                            {
                                txtBasicPriceCount3.Text = (level3.Quantity ?? 0).ToString();
                                txtBasicPriceCount3.ForeColor = System.Drawing.Color.Red;
                                total += (level3.Quantity ?? 0) * (level3.InstallPriceLevel ?? 0);
                            }
                            if (total > 0)
                            {
                                labBasicTotal.Text = total.ToString();
                                labBasicTotal.ForeColor = System.Drawing.Color.Blue;
                            }
                            if (basicIndex == 0)
                            {
                                specialTotal += total;
                                basicIndex++;
                            }

                           
                        }
                    }
                }
            }
        }



        List<CustomerMaterialInfo> materialList = new List<CustomerMaterialInfo>();
        void GetCustomerMaterial()
        {
            int priceItemId = 0;
            var guidanceModel = new SubjectGuidanceBLL().GetList(s => guidanceIdList.Contains(s.ItemId)).FirstOrDefault();
            if (guidanceModel != null)
            {
                priceItemId = guidanceModel.PriceItemId ?? 0;
            }
            var mList = (from customerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                         join basicMaterial in CurrentContext.DbContext.BasicMaterial
                         on customerMaterial.BasicMaterialId equals basicMaterial.Id
                         where customerMaterial.CustomerId == customerId && (customerMaterial.IsDelete == false || customerMaterial.IsDelete == null)
                         && customerMaterial.PriceItemId == priceItemId
                         select new { basicMaterial.MaterialName, customerMaterial }).OrderBy(s => s.MaterialName).ToList();

            mList.ForEach(s =>
            {
                CustomerMaterialInfo model = s.customerMaterial;
                model.MaterialName = s.MaterialName;
                materialList.Add(model);
            });

        }

        protected void otherPriceOrderRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                if (popSheetList.Any())
                {
                    DropDownList ddlSheet = (DropDownList)e.Item.FindControl("ddlSheet_OtherPriceOrder");
                    popSheetList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s;
                        li.Value = s;
                        ddlSheet.Items.Add(li);
                    });
                }
                if (!materialList.Any())
                {
                    GetCustomerMaterial();
                }
                DropDownList ddlMaterial = (DropDownList)e.Item.FindControl("ddlMaterial_OtherPriceOrder");
                materialList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s.MaterialName + "(" + s.Price + ")";
                    li.Value = s.MaterialName + "-" + s.Price;
                    ddlMaterial.Items.Add(li);
                });
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string msg = SubmitQuote();
            ExcuteJs("finish", msg);
        }

        string SubmitQuote()
        {
            //string total = hfQuoteTotalPrice1.Value;
            string total = "0";
            QuotationItem model = new QuotationItem();
            string msg = "ok";
            bool isUpdate = false;
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    SpecialPriceQuoteDetailBLL specialDetailBll = new SpecialPriceQuoteDetailBLL();

                    //if (ViewState["quoteId"] != null)
                    //{
                    //    itemId = StringHelper.IsInt(ViewState["quoteId"].ToString());

                    //}
                    if (quoteItemId > 0)
                    {
                        model = quotationBll.GetModel(quoteItemId);
                        specialDetailBll.Delete(s => s.ItemId == quoteItemId);
                    }
                    model.TotalPrice = StringHelper.IsDecimal(total);

                    model.QuoteSubjectName = ddlQuoteSubject.SelectedItem.Text;
                    int quoteSubjectId = int.Parse(ddlQuoteSubject.SelectedValue);
                    Subject subjectModel = new SubjectBLL().GetModel(quoteSubjectId);
                    if (subjectModel != null)
                    {
                        model.QuoteSubjectCategoryId = subjectModel.SubjectCategoryId;
                    }
                    model.QuoteSubjectId = quoteSubjectId;

                    if (quoteItemId > 0)
                    {

                        quotationBll.Update(model);
                        isUpdate = true;
                    }
                    else
                    {

                        int guidanceYear = 0;
                        int guidanceMonth = 0;
                        if (!string.IsNullOrWhiteSpace(month) && StringHelper.IsDateTime(month))
                        {
                            DateTime date = DateTime.Parse(month);
                            guidanceYear = date.Year;
                            guidanceMonth = date.Month;
                        }
                        model.GuidanceYear = guidanceYear;
                        model.GuidanceMonth = guidanceMonth;
                        model.SubjectCategoryId = subjectCategory;

                        model.SubjectIds = subjectId;
                        model.GuidanceId = guidanceId;

                        model.AddDate = DateTime.Now;
                        model.AddUserId = CurrentUser.UserId;
                        model.CustomerId = customerId;
                        model.IsDelete = false;
                        quotationBll.Add(model);
                    }
                    quoteItemId = model.Id;
                    SpecialPriceQuoteDetail specialDetailModel;
                    foreach (RepeaterItem item in otherInstallPriceRepeater.Items)
                    {
                        if (item.ItemIndex != -1)
                        {
                            TextBox txtOOHPriceCount1 = (TextBox)item.FindControl("txtOOHPriceCount1");
                            TextBox txtOOHPriceCount2 = (TextBox)item.FindControl("txtOOHPriceCount2");
                            TextBox txtOOHPriceCount3 = (TextBox)item.FindControl("txtOOHPriceCount3");
                            TextBox txtOOHPriceCount4 = (TextBox)item.FindControl("txtOOHPriceCount4");

                            TextBox txtBasicPriceCount1 = (TextBox)item.FindControl("txtBasicPriceCount1");
                            TextBox txtBasicPriceCount2 = (TextBox)item.FindControl("txtBasicPriceCount2");
                            TextBox txtBasicPriceCount3 = (TextBox)item.FindControl("txtBasicPriceCount3");

                            if (!string.IsNullOrWhiteSpace(txtOOHPriceCount1.Text) && StringHelper.IsInt(txtOOHPriceCount1.Text) > 0)
                            {
                                specialDetailModel = new SpecialPriceQuoteDetail();
                                specialDetailModel.ItemId = model.Id;
                                specialDetailModel.AddDate = DateTime.Now;
                                specialDetailModel.AddUserId = CurrentUser.UserId;
                                specialDetailModel.ChangeType = (int)QuoteInstallPriceChangeTypeEnum.OOH;
                                specialDetailModel.InstallPriceLevel = 5000;
                                specialDetailModel.Quantity = StringHelper.IsInt(txtOOHPriceCount1.Text);
                                specialDetailBll.Add(specialDetailModel);
                            }
                            if (!string.IsNullOrWhiteSpace(txtOOHPriceCount2.Text) && StringHelper.IsInt(txtOOHPriceCount2.Text) > 0)
                            {
                                specialDetailModel = new SpecialPriceQuoteDetail();
                                specialDetailModel.ItemId = model.Id;
                                specialDetailModel.AddDate = DateTime.Now;
                                specialDetailModel.AddUserId = CurrentUser.UserId;
                                specialDetailModel.ChangeType = (int)QuoteInstallPriceChangeTypeEnum.OOH;
                                specialDetailModel.InstallPriceLevel = 2700;
                                specialDetailModel.Quantity = StringHelper.IsInt(txtOOHPriceCount2.Text);
                                specialDetailBll.Add(specialDetailModel);
                            }
                            if (!string.IsNullOrWhiteSpace(txtOOHPriceCount3.Text) && StringHelper.IsInt(txtOOHPriceCount3.Text) > 0)
                            {
                                specialDetailModel = new SpecialPriceQuoteDetail();
                                specialDetailModel.ItemId = model.Id;
                                specialDetailModel.AddDate = DateTime.Now;
                                specialDetailModel.AddUserId = CurrentUser.UserId;
                                specialDetailModel.ChangeType = (int)QuoteInstallPriceChangeTypeEnum.OOH;
                                specialDetailModel.InstallPriceLevel = 1800;
                                specialDetailModel.Quantity = StringHelper.IsInt(txtOOHPriceCount3.Text);
                                specialDetailBll.Add(specialDetailModel);
                            }
                            if (!string.IsNullOrWhiteSpace(txtOOHPriceCount4.Text) && StringHelper.IsInt(txtOOHPriceCount4.Text) > 0)
                            {
                                specialDetailModel = new SpecialPriceQuoteDetail();
                                specialDetailModel.ItemId = model.Id;
                                specialDetailModel.AddDate = DateTime.Now;
                                specialDetailModel.AddUserId = CurrentUser.UserId;
                                specialDetailModel.ChangeType = (int)QuoteInstallPriceChangeTypeEnum.OOH;
                                specialDetailModel.InstallPriceLevel = 600;
                                specialDetailModel.Quantity = StringHelper.IsInt(txtOOHPriceCount4.Text);
                                specialDetailBll.Add(specialDetailModel);
                            }

                            if (!string.IsNullOrWhiteSpace(txtBasicPriceCount1.Text) && StringHelper.IsInt(txtBasicPriceCount1.Text) > 0)
                            {
                                specialDetailModel = new SpecialPriceQuoteDetail();
                                specialDetailModel.ItemId = model.Id;
                                specialDetailModel.AddDate = DateTime.Now;
                                specialDetailModel.AddUserId = CurrentUser.UserId;
                                specialDetailModel.ChangeType = (int)QuoteInstallPriceChangeTypeEnum.Basic;
                                specialDetailModel.InstallPriceLevel = 800;
                                specialDetailModel.Quantity = StringHelper.IsInt(txtBasicPriceCount1.Text);
                                specialDetailBll.Add(specialDetailModel);
                            }
                            if (!string.IsNullOrWhiteSpace(txtBasicPriceCount2.Text) && StringHelper.IsInt(txtBasicPriceCount2.Text) > 0)
                            {
                                specialDetailModel = new SpecialPriceQuoteDetail();
                                specialDetailModel.ItemId = model.Id;
                                specialDetailModel.AddDate = DateTime.Now;
                                specialDetailModel.AddUserId = CurrentUser.UserId;
                                specialDetailModel.ChangeType = (int)QuoteInstallPriceChangeTypeEnum.Basic;
                                specialDetailModel.InstallPriceLevel = 400;
                                specialDetailModel.Quantity = StringHelper.IsInt(txtBasicPriceCount2.Text);
                                specialDetailBll.Add(specialDetailModel);
                            }
                            if (!string.IsNullOrWhiteSpace(txtBasicPriceCount3.Text) && StringHelper.IsInt(txtBasicPriceCount3.Text) > 0)
                            {
                                specialDetailModel = new SpecialPriceQuoteDetail();
                                specialDetailModel.ItemId = model.Id;
                                specialDetailModel.AddDate = DateTime.Now;
                                specialDetailModel.AddUserId = CurrentUser.UserId;
                                specialDetailModel.ChangeType = (int)QuoteInstallPriceChangeTypeEnum.Basic;
                                specialDetailModel.InstallPriceLevel = 150;
                                specialDetailModel.Quantity = StringHelper.IsInt(txtBasicPriceCount3.Text);
                                specialDetailBll.Add(specialDetailModel);
                            }
                        }
                    }
                    string POPQuoteJson = hfPOPQuoteJson.Value;
                    //specialDetailBll.Delete(s => s.ItemId == model.Id && s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.POP);
                    if (!string.IsNullOrWhiteSpace(POPQuoteJson))
                    {
                        List<SpecialPriceQuoteDetail> specialPriceList = JsonConvert.DeserializeObject<List<SpecialPriceQuoteDetail>>(POPQuoteJson);
                        if (specialPriceList.Any())
                        {
                            specialPriceList.ForEach(s =>
                            {
                                specialDetailModel = new SpecialPriceQuoteDetail();
                                specialDetailModel.ItemId = model.Id;
                                specialDetailModel.AddDate = DateTime.Now;
                                specialDetailModel.AddUserId = CurrentUser.UserId;
                                specialDetailModel.ChangeType = (int)QuoteInstallPriceChangeTypeEnum.POP;
                                specialDetailModel.PriceType = s.PriceType;
                                specialDetailModel.GraphicMaterial = s.GraphicMaterial;
                                specialDetailModel.GraphicMaterialUnitPrice = s.GraphicMaterialUnitPrice;
                                specialDetailModel.Sheet = s.Sheet;
                                specialDetailModel.AddArea = s.AddArea;
                                //specialDetailModel.InstallPriceLevel = 150;
                                specialDetailModel.Quantity = 1;
                                specialDetailBll.Add(specialDetailModel);
                            });
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(subjectId) && ViewState["subjectId"] != null)
                    {
                        subjectId = ViewState["subjectId"].ToString();
                    }
                    tran.Complete();


                }
                catch (Exception ex)
                {
                    msg = "操作失败：" + ex.Message;
                }
            }
            if (msg == "ok" && !isUpdate)
            {
                new QuoteOrderDetailBLL().UpdateQuoteItemId(model.GuidanceId.TrimEnd(','), subjectId.TrimEnd(','), model.Id, "edit");

            }
            return msg;
        }
    }
}
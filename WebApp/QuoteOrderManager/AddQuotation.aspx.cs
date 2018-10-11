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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
//using NPOI.HSSF.UserModel;
//using OfficeOpenXml;


namespace WebApp.QuoteOrderManager
{
    public partial class AddQuotation : BasePage
    {
        public string month = string.Empty;
        public string guidanceId = string.Empty;
        public string subjectCategory = string.Empty;
        public string subjectIdSelected = string.Empty;//不含百丽项目
        public string subjectId = string.Empty;//包含百丽项目
        public string region = string.Empty;
        public int customerId;
        public int itemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemId"] != null)
            {
                itemId = int.Parse(Request.QueryString["itemId"]);
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
            //else
            //{

            //    if (ViewState["customerId"] != null)
            //    {
            //        customerId = int.Parse(ViewState["customerId"].ToString());
            //    }
            //    if (ViewState["month"] != null)
            //    {
            //        month = ViewState["month"].ToString();
            //    }

            //    if (ViewState["subjectCategory"] != null)
            //    {
            //        subjectCategory = ViewState["subjectCategory"].ToString();
            //    }

            //}
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
                BindeExpressPriceImportQuote();
                SetTotalPrice();
            }
        }



        List<int> guidanceIdList = new List<int>();
        List<int> categoryIdList = new List<int>();
        List<int> subjectIdList = new List<int>();
        List<string> regionList = new List<string>();
        QuotationItemBLL quotationBll = new QuotationItemBLL();
        List<SpecialPriceQuoteDetail> specialPriceQuoteDetailList = new List<SpecialPriceQuoteDetail>();
        //decimal addRate = 0;

        int selectQuoteSubjectId = 0;
        void BindData()
        {
            QuotationItem model = quotationBll.GetModel(itemId);
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

                specialPriceQuoteDetailList = new SpecialPriceQuoteDetailBLL().GetList(s => s.ItemId == itemId);
                List<SpecialPriceQuoteDetail> popQuoteList = specialPriceQuoteDetailList.Where(s => s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.POP).ToList();

                hfPOPQuoteJson.Value = JsonConvert.SerializeObject(popQuoteList);
                //ViewState["customerId"] = customerId;
                //ViewState["month"] = month;
                ViewState["guidanceId"] = guidanceId;
                //ViewState["subjectCategory"] = subjectCategory;
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
        /// 不用了
        /// </summary>
        void BindPOPList1()
        {

            var orderList = (from order in CurrentContext.DbContext.QuoteOrderDetail
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where (order.IsDelete == null || order.IsDelete == false)
                             && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || (order.OrderType > (int)OrderTypeEnum.POP))
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             where guidanceIdList.Contains(order.GuidanceId ?? 0)
                                 //&& (categoryIdList.Any()?categoryIdList.Contains(subject.SubjectCategoryId??0):1==1)
                             && (subjectIdList.Any() ? subjectIdList.Contains(order.SubjectId ?? 0) : 1 == 1)
                             select new
                             {
                                 order,
                                 subject
                             }).ToList();
            if (itemId == 0)
            {
                orderList = orderList.Where(s => (s.order.QuoteItemId ?? 0) == 0).ToList();
            }
            List<QuoteModel> quoteOrderList = new List<QuoteModel>();
            List<QuoteOrderDetail> popOrderList = new List<QuoteOrderDetail>();
            //保存每个位置区域的订单

            if (orderList.Any())
            {
                popOrderList = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.POP || s.order.OrderType == (int)OrderTypeEnum.道具).Select(s => s.order).ToList();
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
                var windowOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && !s.Sheet.Contains("地铺") && !s.Sheet.Contains("贴") && !s.PositionDescription.Contains("窗贴") && s.PositionDescription != "左侧贴" && s.PositionDescription != "右侧贴" && s.PositionDescription != "地贴" && s.PositionDescription != "地铺").ToList();
                if (windowOrderList.Any())
                {
                    popOrderListSave.AddRange(windowOrderList);
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(windowOrderList, ref windowList, "Window橱窗", "橱窗背景");
                    if (windowList.Any())
                    {
                        //quoteOrderList.AddRange(windowList);
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
                //橱窗侧贴订单
                var windowSizeStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("侧贴") || s.Sheet.Contains("侧贴"))).ToList();
                if (windowSizeStickOrderList.Any())
                {
                    popOrderListSave.AddRange(windowSizeStickOrderList);
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowSizeStickOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(windowSizeStickOrderList, ref windowList, "Window橱窗", "橱窗侧贴");
                    if (windowList.Any())
                    {
                        //quoteOrderList.AddRange(windowList);
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
                //橱窗地贴订单
                var windowFlootStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("地贴") || s.PositionDescription.Contains("地铺") || s.Sheet.Contains("地贴") || s.Sheet.Contains("地铺"))).ToList();
                if (windowFlootStickOrderList.Any())
                {
                    popOrderListSave.AddRange(windowFlootStickOrderList);
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowFlootStickOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(windowFlootStickOrderList, ref windowList, "Window橱窗", "橱窗地贴");
                    if (windowList.Any())
                    {
                        //quoteOrderList.AddRange(windowList);
                        decimal area = windowList.Sum(s => s.Amount);
                        decimal price = windowList.Sum(s => s.TotalPrice);
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
                //橱窗窗贴订单
                var windowStickOrderList = popOrderList.Where(s => (s.Sheet.Contains("橱窗") && s.PositionDescription == "窗贴") || (s.Sheet.Contains("窗贴"))).ToList();
                if (windowStickOrderList.Any())
                {
                    popOrderListSave.AddRange(windowStickOrderList);
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowStickOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(windowStickOrderList, ref windowList, "Window橱窗", "窗贴");
                    if (windowList.Any())
                    {
                        //quoteOrderList.AddRange(windowList);
                        decimal area = windowList.Sum(s => s.Amount);
                        decimal price = windowList.Sum(s => s.TotalPrice);
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
                    Session["Window橱窗"] = popOrderListSave;
                }
                #endregion
                #region 陈列桌区域
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "WOW 陈列桌区域";
                currentPosition = "陈列桌";
                //陈列桌订单
                var tableOrderList = popOrderList.Where(s => s.Sheet.Contains("陈列桌") || s.Sheet.Contains("展桌")).ToList();
                if (tableOrderList.Any())
                {
                    //先统计台卡
                    List<int> tkSelectedIdList = new List<int>();
                    var frameMachineTypeList = new MachineFrameTypeBLL().GetList(s => s.Sheet == "台卡").ToList();
                    if (frameMachineTypeList.Any())
                    {
                        List<QuoteOrderDetail> tableQuoteOrderList = new List<QuoteOrderDetail>();
                        List<int> frameMachineTypeIdList = frameMachineTypeList.Select(s => s.Id).ToList();
                        var sizeTotalList = new MachineFrameSizeBLL().GetList(s => frameMachineTypeIdList.Contains(s.MachineFrameTypeId ?? 0)).ToList();

                        //台卡
                        if (frameMachineTypeList.Any())
                        {
                            frameMachineTypeList.ForEach(ft =>
                            {
                                var sizeList = sizeTotalList.Where(s => s.MachineFrameTypeId == ft.Id).ToList();
                                if (sizeList.Any())
                                {
                                    sizeList.ForEach(size =>
                                    {
                                        if (string.IsNullOrWhiteSpace(size.Channel))
                                        {
                                            List<QuoteOrderDetail> order1 = tableOrderList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                            if (order1.Any())
                                            {
                                                popOrderListSave.AddRange(order1);
                                                tkSelectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                tableQuoteOrderList.AddRange(order1);
                                            }
                                        }
                                    });
                                }
                                List<QuoteModel> tableList = new List<QuoteModel>();//
                                StatisticMaterial(tableQuoteOrderList, ref tableList, "WOW 陈列桌区域", ft.MachineFrameTypeName);
                                if (tableList.Any())
                                {
                                    //quoteOrderList.AddRange(tableList);
                                    decimal area = tableList.Sum(s => s.Amount);
                                    decimal price = tableList.Sum(s => s.TotalPrice);
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
                    }
                    //除了台卡以外的陈列桌订单 
                    var otherTBOrderList = tableOrderList.Where(s => !tkSelectedIdList.Contains(s.Id)).ToList();
                    if (otherTBOrderList.Any())
                    {
                        List<int> otherTBIdList = new List<int>();
                        //陈列桌静态展订单
                        var tableJTZOrderList = tableOrderList.Where(s => s.Sheet.Contains("静态展") || s.PositionDescription.Contains("静态展")).ToList();
                        if (tableJTZOrderList.Any())
                        {
                            popOrderListSave.AddRange(tableJTZOrderList);
                            otherTBIdList.AddRange(tableJTZOrderList.Select(s => s.Id).ToList());
                            selectedIdList.AddRange(tableJTZOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> tableJTZList = new List<QuoteModel>();
                            StatisticMaterial(tableJTZOrderList, ref tableJTZList, "WOW 陈列桌区域", "陈列桌静态展");
                            if (tableJTZList.Any())
                            {
                                //quoteOrderList.AddRange(tableJTZList);
                                decimal area = tableJTZList.Sum(s => s.Amount);
                                decimal price = tableJTZList.Sum(s => s.TotalPrice);
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
                        //陈列桌模特阵订单
                        var tableModuleOrderList = tableOrderList.Where(s => s.Sheet.Contains("模特阵") || s.PositionDescription.Contains("模特阵")).ToList();
                        if (tableModuleOrderList.Any())
                        {
                            popOrderListSave.AddRange(tableModuleOrderList);
                            otherTBIdList.AddRange(tableModuleOrderList.Select(s => s.Id).ToList());
                            selectedIdList.AddRange(tableModuleOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> tableModuleList = new List<QuoteModel>();
                            StatisticMaterial(tableJTZOrderList, ref tableModuleList, "WOW 陈列桌区域", "陈列桌模特阵");
                            if (tableModuleList.Any())
                            {
                                //quoteOrderList.AddRange(tableModuleList);
                                decimal area = tableModuleList.Sum(s => s.Amount);
                                decimal price = tableModuleList.Sum(s => s.TotalPrice);
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
                        //陈列桌桌铺
                        var tableList = otherTBOrderList.Where(s => !otherTBIdList.Contains(s.Id)).ToList();
                        if (tableList.Any())
                        {
                            popOrderListSave.AddRange(tableList);
                            selectedIdList.AddRange(tableList.Select(s => s.Id).ToList());
                            List<QuoteModel> tbList = new List<QuoteModel>();//
                            StatisticMaterial(tableList, ref tbList, "WOW 陈列桌区域", "陈列桌桌铺");
                            if (tbList.Any())
                            {
                                //quoteOrderList.AddRange(tbList);
                                decimal area = tbList.Sum(s => s.Amount);
                                decimal price = tbList.Sum(s => s.TotalPrice);
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
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["WOW陈列桌区域"] = popOrderListSave;
                }
                #endregion
                #region 服装墙区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "App Wall 服装墙";
                currentPosition = "服装墙";
                var appOrderList = popOrderList.Where(s => s.Sheet.Contains("服装墙")).ToList();
                if (appOrderList.Any())
                {
                    List<int> appSelectedIdList = new List<int>();
                    //List<string> channelList = appOrderList.Select(s => s.Channel != null ? s.Channel.ToUpper() : "").ToList();
                    var frameMachineTypeList = new MachineFrameTypeBLL().GetList(s => s.Sheet == "台卡").ToList();
                    if (frameMachineTypeList.Any())
                    {
                        List<QuoteOrderDetail> appQuoteOrderList = new List<QuoteOrderDetail>();
                        List<int> frameMachineTypeIdList = frameMachineTypeList.Select(s => s.Id).ToList();
                        var sizeTotalList = new MachineFrameSizeBLL().GetList(s => frameMachineTypeIdList.Contains(s.MachineFrameTypeId ?? 0)).ToList();

                        //台卡
                        if (frameMachineTypeList.Any())
                        {
                            frameMachineTypeList.ForEach(ft =>
                            {
                                var sizeList = sizeTotalList.Where(s => s.MachineFrameTypeId == ft.Id).ToList();
                                if (sizeList.Any())
                                {
                                    sizeList.ForEach(size =>
                                    {
                                        if (string.IsNullOrWhiteSpace(size.Channel))
                                        {
                                            List<QuoteOrderDetail> order1 = appOrderList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                            if (order1.Any())
                                            {
                                                popOrderListSave.AddRange(order1);
                                                appSelectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                appQuoteOrderList.AddRange(order1);
                                            }
                                        }
                                    });
                                }
                                List<QuoteModel> appList = new List<QuoteModel>();//
                                StatisticMaterial(appQuoteOrderList, ref appList, "App Wall 服装墙", ft.MachineFrameTypeName);
                                if (appList.Any())
                                {

                                    //quoteOrderList.AddRange(appList);
                                    decimal area = appList.Sum(s => s.Amount);
                                    decimal price = appList.Sum(s => s.TotalPrice);
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
                    }
                    //除了台卡以外的
                    var otherOrderList = appOrderList.Where(s => !appSelectedIdList.Contains(s.Id)).ToList();
                    if (otherOrderList.Any())
                    {
                        popOrderListSave.AddRange(otherOrderList);
                        //要按店铺类型统计服装墙
                        List<string> channelList = otherOrderList.Select(s => s.Channel).Distinct().ToList();
                        channelList.ForEach(ch =>
                        {
                            var list0 = otherOrderList.Where(s => s.Channel == ch).ToList();
                            selectedIdList.AddRange(list0.Select(s => s.Id).ToList());
                            List<QuoteModel> oList = new List<QuoteModel>();//
                            StatisticMaterial(list0, ref oList, "App Wall 服装墙", ch + "服装墙");
                            if (oList.Any())
                            {
                                //quoteOrderList.AddRange(oList);
                                decimal area = oList.Sum(s => s.Amount);
                                decimal price = oList.Sum(s => s.TotalPrice);
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
                }
                if (popOrderListSave.Any())
                {
                    Session["AppWall服装墙"] = popOrderListSave;
                }
                #endregion
                #region 鞋墙区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "FTW Wall 鞋墙";
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
                    shoeWareSheetList.ForEach(sh =>
                    {
                        var ftwOrderList0 = popOrderList.Where(s => s.Sheet.Contains(sh)).ToList();
                        if (ftwOrderList0.Any())
                        {
                            popOrderListSave.AddRange(ftwOrderList0);
                            ftwOrderList.AddRange(ftwOrderList0);
                        }
                    });
                }
                if (ftwOrderList.Any())
                {


                    var frameMachineTypeList = new MachineFrameTypeBLL().GetList(s => s.Sheet == "鞋墙");
                    if (frameMachineTypeList.Any())
                    {

                        //HC
                        var HCList = ftwOrderList.Where(s => s.Channel != null && (s.Channel.ToUpper() == "HC" || s.Channel.ToUpper() == "HOMECOURT")).ToList();
                        List<int> hcOrderIdList = new List<int>();
                        #region HC
                        if (HCList.Any())
                        {
                            hcOrderIdList = HCList.Select(s => s.Id).ToList();
                            List<int> hcNormalIdList = new List<int>();
                            //主KV
                            List<QuoteOrderDetail> ftwQuoteOrderListKV = HCList.Where(s => s.Sheet.Contains("鞋墙")).ToList();
                            if (ftwQuoteOrderListKV.Any())
                            {
                                popOrderListSave.AddRange(ftwQuoteOrderListKV);
                                hcNormalIdList = ftwQuoteOrderListKV.Select(s => s.Id).ToList();
                                selectedIdList.AddRange(ftwQuoteOrderListKV.Select(s => s.Id).ToList());
                                List<QuoteModel> ftwList = new List<QuoteModel>();//
                                StatisticMaterial(ftwQuoteOrderListKV, ref ftwList, "FTW Wall 鞋墙", "HC鞋墙主KV");
                                if (ftwList.Any())
                                {
                                    //quoteOrderList.AddRange(ftwList);
                                    decimal area = ftwList.Sum(s => s.Amount);
                                    decimal price = ftwList.Sum(s => s.TotalPrice);
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
                            //主灯槽
                            List<QuoteOrderDetail> ftwQuoteOrderList = HCList.Where(s => s.Sheet.Contains("凹槽") || s.Sheet.Contains("灯槽")).ToList();
                            if (ftwQuoteOrderList.Any())
                            {
                                popOrderListSave.AddRange(ftwQuoteOrderList);
                                var hcNormalIdList0 = ftwQuoteOrderList.Select(s => s.Id).ToList();
                                selectedIdList.AddRange(ftwQuoteOrderList.Select(s => s.Id).ToList());
                                if (hcNormalIdList0.Any())
                                {
                                    hcNormalIdList.AddRange(hcNormalIdList0);
                                }
                                List<QuoteModel> ftwList = new List<QuoteModel>();//
                                StatisticMaterial(ftwQuoteOrderList, ref ftwList, "FTW Wall 鞋墙", "HC鞋墙灯槽");
                                if (ftwList.Any())
                                {
                                    //quoteOrderList.AddRange(ftwList);
                                    decimal area = ftwList.Sum(s => s.Amount);
                                    decimal price = ftwList.Sum(s => s.TotalPrice);
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
                            //鞋吧，弧形吧的其他位置
                            List<QuoteOrderDetail> ftwQuoteOrderListOthers = HCList.Where(s => !hcNormalIdList.Contains(s.Id)).ToList();
                            if (ftwQuoteOrderListOthers.Any())
                            {
                                popOrderListSave.AddRange(ftwQuoteOrderListOthers);
                                selectedIdList.AddRange(ftwQuoteOrderListOthers.Select(s => s.Id).ToList());
                                List<QuoteModel> ftwList = new List<QuoteModel>();//
                                StatisticMaterial(ftwQuoteOrderListOthers, ref ftwList, "FTW Wall 鞋墙", "HC圆桌+科技+鞋吧");
                                if (ftwList.Any())
                                {
                                    //quoteOrderList.AddRange(ftwList);
                                    decimal area = ftwList.Sum(s => s.Amount);
                                    decimal price = ftwList.Sum(s => s.TotalPrice);
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
                        }
                        #endregion

                        #region 非HC
                        var notHCList = ftwOrderList.Where(s => !hcOrderIdList.Contains(s.Id)).ToList();
                        List<int> NotHCOrderIdList = new List<int>();
                        if (notHCList.Any())
                        {
                            var sizeTotalList = new MachineFrameSizeBLL().GetList(s => s.Id > 0);
                            var frameMachineTypeList2 = frameMachineTypeList.Where(s => !s.MachineFrameTypeName.Contains("HC")).ToList();
                            frameMachineTypeList2.ForEach(frameType =>
                            {
                                List<QuoteOrderDetail> ftwQuoteOrderList = new List<QuoteOrderDetail>();
                                //主KV
                                var sizelist1 = sizeTotalList.Where(s => s.MachineFrameTypeId == frameType.Id && s.FrameType == 1).ToList();
                                sizelist1.ForEach(size =>
                                {
                                    List<QuoteOrderDetail> order1 = notHCList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                    if (order1.Any())
                                    {
                                        popOrderListSave.AddRange(order1);
                                        selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        NotHCOrderIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        ftwQuoteOrderList.AddRange(order1);
                                    }
                                });
                                List<QuoteModel> ftwList = new List<QuoteModel>();//
                                StatisticMaterial(ftwQuoteOrderList, ref ftwList, "FTW Wall 鞋墙", "鞋墙主KV");
                                if (ftwList.Any())
                                {
                                    //quoteOrderList.AddRange(ftwList);
                                    decimal area = ftwList.Sum(s => s.Amount);
                                    decimal price = ftwList.Sum(s => s.TotalPrice);
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

                                //灯槽
                                ftwQuoteOrderList = new List<QuoteOrderDetail>();
                                var sizelist2 = sizeTotalList.Where(s => s.MachineFrameTypeId == frameType.Id && s.FrameType == 2).ToList();
                                sizelist2.ForEach(size =>
                                {
                                    List<QuoteOrderDetail> order1 = notHCList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                    if (order1.Any())
                                    {
                                        popOrderListSave.AddRange(order1);
                                        selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        NotHCOrderIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        ftwQuoteOrderList.AddRange(order1);
                                    }
                                });
                                ftwList = new List<QuoteModel>();
                                StatisticMaterial(ftwQuoteOrderList, ref ftwList, "FTW Wall 鞋墙", "灯槽");
                                if (ftwList.Any())
                                {
                                    //quoteOrderList.AddRange(ftwList);
                                    decimal area = ftwList.Sum(s => s.Amount);
                                    decimal price = ftwList.Sum(s => s.TotalPrice);
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
                        //非大货器架的鞋墙
                        var otherSubjectOrderList = notHCList.Where(s => !NotHCOrderIdList.Contains(s.Id)).ToList();
                        if (otherSubjectOrderList.Any())
                        {
                            popOrderListSave.AddRange(otherSubjectOrderList);
                            selectedIdList.AddRange(otherSubjectOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> ftwList = new List<QuoteModel>();//
                            StatisticMaterial(otherSubjectOrderList, ref ftwList, "FTW Wall 鞋墙", "鞋墙");
                            if (ftwList.Any())
                            {
                                //quoteOrderList.AddRange(ftwList);
                                decimal area = ftwList.Sum(s => s.Amount);
                                decimal price = ftwList.Sum(s => s.TotalPrice);
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
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["FTWWall鞋墙"] = popOrderListSave;
                }
                #endregion
                #region SMU区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "In-store SMU 店内其它位置SMU";
                currentPosition = "SMU";
                var smuOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("smu")).ToList();
                if (smuOrderList.Any())
                {
                    popOrderListSave.AddRange(smuOrderList);
                    selectedIdList.AddRange(smuOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> smuList = new List<QuoteModel>();//
                    StatisticMaterial(smuOrderList, ref smuList, "In-store SMU 店内其它位置SMU", "SMU");
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
                    StatisticMaterial(zdOrderList, ref smuList, "In-store SMU 店内其它位置SMU", "中岛");
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
                if (popOrderListSave.Any())
                {
                    Session["In-storeSMU店内其它位置SMU"] = popOrderListSave;
                }
                #endregion
                #region 收银台区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "Cashier Desk 收银台区域";
                currentPosition = "收银台";
                var cashierOrderList = popOrderList.Where(s => s.Sheet == "收银台").ToList();
                if (cashierOrderList.Any())
                {
                    popOrderListSave.AddRange(cashierOrderList);
                    selectedIdList.AddRange(cashierOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> cashierList = new List<QuoteModel>();//
                    StatisticMaterial(cashierOrderList, ref cashierList, "Cashier Desk 收银台区域", "收银台");
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
                    Session["CashierDesk收银台区域"] = popOrderListSave;
                }
                #endregion
                #region OOH区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "OOH 店外非橱窗位置";
                currentPosition = "OOH";
                var oohOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("ooh")).ToList();
                if (oohOrderList.Any())
                {
                    popOrderListSave.AddRange(oohOrderList);
                    selectedIdList.AddRange(oohOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> oohList = new List<QuoteModel>();//
                    StatisticMaterial(oohOrderList, ref oohList, "OOH 店外非橱窗位置", "OOH");
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
                    Session["OOH店外非橱窗位置"] = popOrderListSave;
                }
                #endregion
                #region 除了以上常规位置外的其他位置
                popOrderListSave = new List<QuoteOrderDetail>();
                currSheet = "其他区域";
                currentPosition = "其他区域";
                var otherSheetOrderList = popOrderList.Where(s => !selectedIdList.Contains(s.Id)).ToList();
                if (otherSheetOrderList.Any())
                {
                    popOrderListSave.AddRange(otherSheetOrderList);
                    List<string> sheets = otherSheetOrderList.Select(s => s.Sheet).Distinct().ToList();
                    sheets.ForEach(sheet =>
                    {
                        var orderList0 = otherSheetOrderList.Where(s => s.Sheet == sheet).ToList();
                        List<QuoteModel> smuList = new List<QuoteModel>();//
                        StatisticMaterial(orderList0, ref smuList, "其他区域", sheet);
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
                    });

                }
                if (popOrderListSave.Any())
                {
                    Session["其他区域"] = popOrderListSave;
                }
                //popOrderListSave = new List<QuoteOrderDetail>();
                #endregion

            }
            popList.DataSource = quoteOrderList;
            popList.DataBind();
            //合并单元格
            //string combinFirstCol = "sheet";
            //List<string> otherColList = new List<string>() { "systemTotal", "importArea", "importPrice" };
            //Dictionary<string, bool> otherColListDic = new Dictionary<string, bool>();
            ////key:是列名，value：是否需要累计单元格里面的数值
            //otherColListDic.Add("systemTotal",true);
            //otherColListDic.Add("importArea", false);
            //otherColListDic.Add("importPrice", false);
            //Dictionary<string, Dictionary<string, bool>> combinDic = new Dictionary<string, Dictionary<string, bool>>();
            //combinDic.Add(combinFirstCol, otherColListDic);
            //CombineCell(popList, combinDic);

            if (orderList.Any())
            {
                BindInstallPrice(orderList.Select(s => s.order).ToList());
                BindeExpressPrice(orderList.Select(s => s.order).ToList());
                BindOtherPriceOrderList(orderList.Select(s => s.order).ToList());
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
                var windowOrderList = popOrderList.Where(s => s.Sheet.Contains(currentPosition)).ToList();
                if (windowOrderList.Any())
                {
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
            //CombineCell(popList, new List<string> { "sheet" });
            //BindInstallPrice(popOrderList);
            //Session["QuoteModel"] = quoteOrderList;
            if (orderList.Any())
            {
                BindInstallPrice(orderList.Select(s => s.order).ToList());
                BindeExpressPrice(orderList.Select(s => s.order).ToList());
                BindOtherPriceOrderList(orderList.Select(s => s.order).ToList());
            }


        }

        /// <summary>
        /// 绑定安装费
        /// </summary>
        /// <param name="orderList"></param>
        /// 
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
            List<ImportQuoteOrder> importInstallQuoteList = new List<ImportQuoteOrder>();
            importInstallQuoteList = importQuoteOrderBll.GetList(s => s.ItemId == itemId && s.OrderType == (int)OrderTypeEnum.安装费);
            guidanceIdList.ForEach(gid =>
            {
                #region 活动安装费（自动归类）
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
                if (itemId == 0)
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
                if (itemId == 0)
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

            #region 导入的安装费报价
            if (importInstallQuoteList.Any())
            {
                decimal importInstallQuote = importInstallQuoteList.Sum(s => (s.InstallUnitPrice ?? 0) * (s.Quantity ?? 1));
                popImportTotalPrice += importInstallQuote;
                var oohquote1 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 5000).FirstOrDefault();
                decimal oohquote1Price = 0;
                if (oohquote1 != null)
                {
                    labOOHLevel1QuoteCount.Text = (oohquote1.Quantity ?? 1).ToString();
                    oohquote1Price = (oohquote1.Quantity ?? 1) * (oohquote1.InstallUnitPrice ?? 0);
                    labOOHLevel1Quote.Text = oohquote1Price.ToString();

                }
                decimal d1 = oohquote1Price - oohLevelOnePrice;
                string d1Str = d1.ToString();
                if (d1 > 0)
                {
                    d1Str = "+" + d1;
                }
                labOOHLevel1Difference.Text = d1Str;

                var oohquote2 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 2700).FirstOrDefault();
                decimal oohquote2Price = 0;
                if (oohquote2 != null)
                {
                    labOOHLevel2QuoteCount.Text = (oohquote2.Quantity ?? 1).ToString();
                    oohquote2Price = (oohquote2.Quantity ?? 1) * (oohquote2.InstallUnitPrice ?? 0);
                    labOOHLevel2Quote.Text = oohquote2Price.ToString();

                }
                decimal d2 = oohquote2Price - oohLevelTwoPrice;
                string d2Str = d2.ToString();
                if (d2 > 0)
                {
                    d2Str = "+" + d2;
                }
                labOOHLevel2Difference.Text = d2Str;

                var oohquote3 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 1800).FirstOrDefault();
                decimal oohquote3Price = 0;
                if (oohquote3 != null)
                {
                    labOOHLevel3QuoteCount.Text = (oohquote3.Quantity ?? 1).ToString();
                    oohquote3Price = (oohquote3.Quantity ?? 1) * (oohquote3.InstallUnitPrice ?? 0);
                    labOOHLevel3Quote.Text = oohquote3Price.ToString();

                }
                decimal d3 = oohquote3Price - oohLevelThreePrice;
                string d3Str = d3.ToString();
                if (d3 > 0)
                {
                    d3Str = "+" + d3;
                }
                labOOHLevel3Difference.Text = d3Str;

                var oohquote4 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 600).FirstOrDefault();
                decimal oohquote4Price = 0;
                if (oohquote4 != null)
                {
                    labOOHLevel4QuoteCount.Text = (oohquote4.Quantity ?? 1).ToString();
                    oohquote4Price = (oohquote4.Quantity ?? 1) * (oohquote4.InstallUnitPrice ?? 0);
                    labOOHLevel4Quote.Text = oohquote4Price.ToString();

                }
                decimal d4 = oohquote4Price - oohLevelFourPrice;
                string d4Str = d4.ToString();
                if (d4 > 0)
                {
                    d4Str = "+" + d4;
                }
                labOOHLevel4Difference.Text = d4Str;

                var wquote1 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 1000).FirstOrDefault();
                decimal wquote1Price = 0;
                if (wquote1 != null)
                {
                    labWindowLevel1QuoteCount.Text = (wquote1.Quantity ?? 1).ToString();
                    wquote1Price = (wquote1.Quantity ?? 1) * (wquote1.InstallUnitPrice ?? 0);
                    labWindowLevel1Quote.Text = wquote1Price.ToString();

                }
                decimal d5 = wquote1Price - windowLevelOnePrice;
                string d5Str = d5.ToString();
                if (d5 > 0)
                {
                    d5Str = "+" + d5;
                }
                labWindowLevel1Difference.Text = d5Str;

                var bquote1 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 800).FirstOrDefault();
                decimal bquote1Price = 0;
                if (bquote1 != null)
                {
                    labBasicLevel1QuoteCount.Text = (bquote1.Quantity ?? 1).ToString();
                    bquote1Price = (bquote1.Quantity ?? 1) * (bquote1.InstallUnitPrice ?? 0);
                    labBasicLevel1Quote.Text = bquote1Price.ToString();

                }
                decimal d6 = bquote1Price - basicLevelOnePrice;
                string d6Str = d6.ToString();
                if (d6 > 0)
                {
                    d6Str = "+" + d6;
                }
                labBasicLevel1Difference.Text = d6Str;

                var wquote2 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 500 && s.MaterialSupport == "basic").FirstOrDefault();
                decimal wquote2Price = 0;
                if (wquote2 != null)
                {
                    labWindowLevel2QuoteCount.Text = (wquote2.Quantity ?? 1).ToString();
                    wquote2Price = (wquote2.Quantity ?? 1) * (wquote2.InstallUnitPrice ?? 0);
                    labWindowLevel2Quote.Text = wquote2Price.ToString();

                }
                decimal d7 = wquote2Price - windowLevelTwoPrice;
                string d7Str = d7.ToString();
                if (d7 > 0)
                {
                    d7Str = "+" + d7;
                }
                labWindowLevel2Difference.Text = d7Str;

                var bquote2 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 400).FirstOrDefault();
                decimal bquote2Price = 0;
                if (bquote2 != null)
                {
                    labBasicLevel2QuoteCount.Text = (bquote2.Quantity ?? 1).ToString();
                    bquote2Price = (bquote2.Quantity ?? 1) * (bquote2.InstallUnitPrice ?? 0);
                    labBasicLevel2Quote.Text = bquote2Price.ToString();

                }
                decimal d8 = bquote2Price - basicLevelTwoPrice;
                string d8Str = d8.ToString();
                if (d8 > 0)
                {
                    d8Str = "+" + d8;
                }
                labBasicLevel2Difference.Text = d8Str;

                var wquote3 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 200).FirstOrDefault();
                decimal wquote3Price = 0;
                if (wquote3 != null)
                {
                    labWindowLevel3QuoteCount.Text = (wquote3.Quantity ?? 1).ToString();
                    wquote3Price = (wquote3.Quantity ?? 1) * (wquote3.InstallUnitPrice ?? 0);
                    labWindowLevel3Quote.Text = wquote3Price.ToString();

                }
                decimal d9 = wquote3Price - windowLevelThreePrice;
                string d9Str = d9.ToString();
                if (d9 > 0)
                {
                    d9Str = "+" + d9;
                }
                labWindowLevel3Difference.Text = d9Str;

                var bquote3 = importInstallQuoteList.Where(s => s.InstallUnitPrice == 150 && s.MaterialSupport == "basic").FirstOrDefault();
                decimal bquote3Price = 0;
                if (bquote3 != null)
                {
                    labBasicLevel3QuoteCount.Text = (bquote3.Quantity ?? 1).ToString();
                    bquote3Price = (bquote3.Quantity ?? 1) * (bquote3.InstallUnitPrice ?? 0);
                    labBasicLevel3Quote.Text = bquote3Price.ToString();

                }
                decimal d10 = bquote3Price - basicLevelThreePrice;
                string d10Str = d10.ToString();
                if (d10 > 0)
                {
                    d10Str = "+" + d10;
                }
                labBasicLevel3Difference.Text = d10Str;

                var kwquote = importInstallQuoteList.Where(s => s.InstallUnitPrice == 500 && s.MaterialSupport == "others").FirstOrDefault();
                decimal kwquotePrice = 0;
                if (kwquote != null)
                {
                    labKidsWindowLevelQuoteCount.Text = (kwquote.Quantity ?? 1).ToString();
                    kwquotePrice = (kwquote.Quantity ?? 1) * (kwquote.InstallUnitPrice ?? 0);
                    labKidsWindowLevelQuote.Text = kwquotePrice.ToString();

                }
                decimal d11 = kwquotePrice - kidsWindowPrice;
                string d11Str = d11.ToString();
                if (d11 > 0)
                {
                    d11Str = "+" + d11;
                }
                labKidsWindowLevelDifference.Text = d11Str;

                var kbquote = importInstallQuoteList.Where(s => s.InstallUnitPrice == 150 && s.MaterialSupport == "others").FirstOrDefault();
                decimal kbquotePrice = 0;
                if (kbquote != null)
                {
                    labKidsBasicLevelQuoteCount.Text = (kbquote.Quantity ?? 1).ToString();
                    kbquotePrice = (kbquote.Quantity ?? 1) * (kbquote.InstallUnitPrice ?? 0);
                    labKidsBasicLevelQuote.Text = kbquotePrice.ToString();

                }
                decimal d12 = kbquotePrice - kidsBasicPrice;
                string d12Str = d12.ToString();
                if (d12 > 0)
                {
                    d12Str = "+" + d12;
                }
                labKidsBasicLevelDifference.Text = d12Str;

                var gquote = importInstallQuoteList.Where(s => s.InstallUnitPrice == 150 && s.MaterialSupport == "generic").FirstOrDefault();
                decimal gquotePrice = 0;
                if (gquote != null)
                {
                    labGenericLevelQuoteCount.Text = (gquote.Quantity ?? 1).ToString();
                    gquotePrice = (gquote.Quantity ?? 1) * (gquote.InstallUnitPrice ?? 0);
                    labGenericLevelQuote.Text = gquotePrice.ToString();

                }
                decimal d13 = gquotePrice - genericLevelPrice;
                string d13Str = d13.ToString();
                if (d13 > 0)
                {
                    d13Str = "+" + d13;
                }
                labGenericLevelDifference.Text = d13Str;

                if (importInstallQuote > 0)
                {
                    labInstallPriceImport.Text = importInstallQuote.ToString();
                }
            }
            #endregion

            #endregion

            otherInstallPriceRepeater.DataSource = otherInstallPriceList.OrderByDescending(s => s.PriceType).ToList();
            otherInstallPriceRepeater.DataBind();
            OtherInstallPriceCombineCell();
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
                    if (itemId == 0)
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
        /// 快递费和物料导入的实际报价
        /// </summary>
        void BindeExpressPriceImportQuote()
        {
            var quoteList = importQuoteOrderBll.GetList(s => s.ItemId == itemId && (s.OrderType == (int)OrderTypeEnum.快递费 || s.OrderType == (int)OrderTypeEnum.物料)).OrderBy(s => s.OrderType).ToList();
            if (quoteList.Any())
            {
                List<ExpressPriceClass> expressPriceList = new List<ExpressPriceClass>();
                ExpressPriceClass expressModel;
                quoteList.ForEach(s =>
                {
                    expressModel = new ExpressPriceClass();
                    expressModel.Count = s.Quantity ?? 1;
                    expressModel.OrderType = CommonMethod.GeEnumName<OrderTypeEnum>(s.OrderType.ToString());
                    expressModel.PriceType = s.Sheet;
                    if (s.OrderType == (int)OrderTypeEnum.快递费)
                    {
                        expressModel.Price = s.ExpressUnitPrice ?? 0;

                    }
                    else
                    {
                        expressModel.Price = s.UnitPrice ?? 0;
                    }
                    expressPriceList.Add(expressModel);
                });
                popImportTotalPrice += expressPriceList.Sum(s => s.Count * s.Price);
                expressPriceListRepeater1.DataSource = expressPriceList;
                expressPriceListRepeater1.DataBind();
                Panel_QuoteExpress.Visible = true;
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

        /// <summary>
        /// 导入的实际报价
        /// </summary>
        decimal expressTotalPrice1 = 0;
        protected void expressPriceListRepeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                ExpressPriceClass item = e.Item.DataItem as ExpressPriceClass;
                if (item != null)
                {

                    decimal subPrice = item.Count * item.Price;
                    expressTotalPrice1 += subPrice;
                    ((Label)e.Item.FindControl("labSubPrice1")).Text = subPrice.ToString();

                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labExpressPriceTotal1")).Text = expressTotalPrice1.ToString();
            }
        }

        decimal installPriceTotal = 0;//安装费合计
        decimal installPriceStandard = 0;//标准安装费合计
        decimal popTotalPrice = 0;
        decimal popTotalArea = 0;
        decimal popImportTotalPrice = 0;
        decimal popImportTotalArea = 0;
        decimal addRateTotalPrie = 0;

        List<ImportQuoteOrder> importPOPQuoteList = new List<ImportQuoteOrder>();
        List<QuoteOrderDetail> quoteOrderDetailList = new List<QuoteOrderDetail>();
        List<QuoteDifferenceDetail> differenceDetailList = new List<QuoteDifferenceDetail>();
        List<QuoteOrderDetail> supplementOrderDetailList = new List<QuoteOrderDetail>();


        protected void popList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                QuoteModel model = e.Item.DataItem as QuoteModel;
                if (model != null)
                {
                    popTotalPrice += model.TotalPrice;
                    popTotalArea += model.Amount;
                    //增补金额和面积
                    //if (!supplementOrderDetailList.Any())
                    //{

                    //    supplementOrderDetailList = (from order in CurrentContext.DbContext.QuoteOrderDetail
                    //                                 join subject in CurrentContext.DbContext.Subject
                    //                                 on order.SubjectId equals subject.Id
                    //                                 where subject.SubjectItemType == (int)SubjectItemTypeEnum.Supplement
                    //                                 && subjectIdList.Contains(order.SubjectId ?? 0)
                    //                                 && (order.OrderType == (int)OrderTypeEnum.POP || order.OrderType == (int)OrderTypeEnum.道具)
                    //                                 select order).ToList();

                    //}
                    //if (supplementOrderDetailList.Any())
                    //{
                    //    var supplementList = supplementOrderDetailList.Where(s => s.Sheet != null && s.Sheet.ToLower().Contains(model.PositionDescription.ToLower())).ToList();
                    //    if (supplementList.Any())
                    //    {
                    //        decimal supplementArea = supplementList.Sum(s => s.Area ?? 0);
                    //        decimal supplementPrice = supplementList.Sum(s => s.DefaultTotalPrice ?? 0);
                    //        popSupplementTotalArea += supplementArea;
                    //        popSupplementTotalPrice += supplementPrice;
                    //        ((Label)e.Item.FindControl("labSupplementArea")).Text = Math.Round(supplementArea, 2).ToString();
                    //        ((Label)e.Item.FindControl("labSupplementPrice")).Text = Math.Round(supplementPrice, 2).ToString();
                    //    }
                    //}
                    //导入的报价
                    if (!importPOPQuoteList.Any())
                    {
                        importPOPQuoteList = importQuoteOrderBll.GetList(s => s.ItemId == itemId && s.OrderType == (int)OrderTypeEnum.POP);
                    }
                    if (importPOPQuoteList.Any())
                    {
                        string sheet = StringHelper.ReplaceSpace(model.Sheet).ToLower();
                        decimal importArea = importPOPQuoteList.Where(s => s.Sheet == sheet).Sum(s => s.POPArea ?? 0);
                        decimal importPrice = importPOPQuoteList.Where(s => s.Sheet == sheet).Sum(s => s.POPPrice ?? 0);
                        ((Label)e.Item.FindControl("labImportArea")).Text = Math.Round(importArea, 2).ToString();
                        ((Label)e.Item.FindControl("labImportPrice")).Text = Math.Round(importPrice, 2).ToString();
                        decimal differenceP = Math.Round((importPrice - model.TotalPrice), 2);
                        string differencePStr = differenceP.ToString();
                        if (differenceP > 0)
                        {
                            differencePStr = "+" + differenceP;
                        }

                        ((Label)e.Item.FindControl("labDifference")).Text = differencePStr;
                        popImportTotalArea += importArea;
                        popImportTotalPrice += importPrice;
                    }
                    //QuoteAddRateAtSize quoteAddRateAtSizeModel = quoteAddRateAtSizeBll.GetList(s=>s.QuoutItemId==itemId && s.Sheet==model.PositionDescription.ToLower()).FirstOrDefault();
                    //if (quoteAddRateAtSizeModel != null)
                    //{
                    //    ((Label)e.Item.FindControl("labAddRate")).Text = (quoteAddRateAtSizeModel.Rate??0).ToString();
                    //    ((Label)e.Item.FindControl("labAddRatePrice")).Text = Math.Round((quoteAddRateAtSizeModel.Price??0), 2).ToString();
                    //}
                    if (!quoteOrderDetailList.Any())
                    {
                        quoteOrderDetailList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.SubjectId ?? 0) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具) && s.Sheet != null);

                    }
                    var orderList = quoteOrderDetailList.Where(s => s.Sheet.ToLower() == model.PositionDescription.ToLower()).ToList();
                    if (subjectIdList.Any())
                    {
                        orderList = orderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
                    }
                    if (itemId > 0)
                    {
                        orderList = orderList.Where(s => (s.QuoteItemId ?? 0) == 0 || s.QuoteItemId == itemId).ToList();
                    }
                    else
                    {
                        orderList = orderList.Where(s => (s.QuoteItemId ?? 0) == 0).ToList();
                    }
                    if (orderList.Any())
                    {
                        decimal price = orderList.Sum(s => ((s.AutoAddTotalPrice ?? 0) - (s.DefaultTotalPrice ?? 0)));
                        decimal rate = orderList[0].AddSizeRate ?? 0;
                        Label labAddRate = (Label)e.Item.FindControl("labAddRate");
                        labAddRate.Text = rate.ToString();
                        string priceStr = "0";
                        if (price > 0)
                        {
                            addRateTotalPrie += price;
                            priceStr = "+" + Math.Round(price, 2);
                            Label labAddRatePrice = (Label)e.Item.FindControl("labAddRatePrice");
                            labAddRatePrice.Text = priceStr;
                            labAddRatePrice.ForeColor = System.Drawing.Color.Green;
                            labAddRate.ForeColor = System.Drawing.Color.Green;
                        }

                    }
                    //给其他报价项目绑定材质选择
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

                    if (!differenceDetailList.Any())
                    {
                        differenceDetailList = new QuoteDifferenceDetailBLL().GetList(s => s.QuoteItemId == itemId);
                    }
                    if (differenceDetailList.Any())
                    {
                        var diffList = differenceDetailList.Where(s => s.Sheet.ToLower() == model.PositionDescription.ToLower()).ToList();
                        if (diffList.Any())
                        {
                            //StringBuilder tr = new StringBuilder();

                            HtmlTable AddExtendPOPPriceTable = (HtmlTable)e.Item.FindControl("AddExtendPOPPriceTable");
                            decimal totalPrice = 0;
                            diffList.ForEach(s =>
                            {

                                HtmlTableRow row = new HtmlTableRow();
                                row.Style.Add("color", "blue");
                                HtmlTableCell cell = new HtmlTableCell();
                                cell.InnerHtml = "类型：";
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                cell.InnerHtml = s.AddPriceType;
                                cell.Style.Add("text-align", "left");
                                cell.Style.Add("padding-left", "5px");
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                cell.InnerHtml = "材质：" + s.MaterialName;
                                cell.Style.Add("text-align", "left");
                                cell.Style.Add("padding-left", "5px");
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                cell.InnerHtml = "面积：" + s.AddArea;
                                cell.Style.Add("text-align", "left");
                                cell.Style.Add("padding-left", "5px");
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                cell.InnerHtml = "备注：" + s.Remark;
                                cell.Style.Add("text-align", "left");
                                cell.Style.Add("padding-left", "5px");
                                row.Cells.Add(cell);

                                cell = new HtmlTableCell();
                                cell.InnerHtml = "<span data-id='" + s.Id + "' name='spanDeleteDifference' style='cursor:pointer;color:red;'>×</span>";

                                row.Cells.Add(cell);
                                AddExtendPOPPriceTable.Rows.Add(row);
                                totalPrice += ((s.AddArea ?? 0) * (s.MaterialUnitPrice ?? 0));
                            });
                            HtmlTableRow row1 = new HtmlTableRow();
                            HtmlTableCell cell1 = new HtmlTableCell();
                            cell1.ColSpan = 2;
                            cell1.InnerHtml = "合计：";
                            cell1.Style.Add("text-align", "right");
                            row1.Cells.Add(cell1);

                            cell1 = new HtmlTableCell();
                            cell1.InnerHtml = Math.Round(totalPrice, 2).ToString();
                            cell1.Style.Add("text-align", "left");
                            cell1.Style.Add("padding-left", "5px");
                            cell1.Style.Add("color", "green");
                            row1.Cells.Add(cell1);

                            cell1 = new HtmlTableCell();
                            cell1.ColSpan = 3;
                            row1.Cells.Add(cell1);
                            AddExtendPOPPriceTable.Rows.Add(row1);
                        }
                    }
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labPOPTotalArea")).Text = Math.Round(popTotalArea, 2).ToString();
                ((Label)e.Item.FindControl("labPOPTotalPrice")).Text = Math.Round(popTotalPrice, 2).ToString();

                ((Label)e.Item.FindControl("labAddRateTotalPrice")).Text = Math.Round(addRateTotalPrie, 2).ToString();

                ((Label)e.Item.FindControl("labPOPImportTotalArea")).Text = Math.Round(popImportTotalArea, 2).ToString();
                ((Label)e.Item.FindControl("labPOPImportTotalPrice")).Text = Math.Round(popImportTotalPrice, 2).ToString();

                //((Label)e.Item.FindControl("labPOPSupplementTotalArea")).Text = Math.Round(popSupplementTotalArea, 2).ToString();
                //((Label)e.Item.FindControl("labPOPSupplementTotalPrice")).Text = Math.Round(popSupplementTotalPrice, 2).ToString();



                //hfQuoteTotalArea.Value = popTotalArea.ToString();
                //TextBox txtAddRate = (TextBox)e.Item.FindControl("txtAddRate");
                //txtAddRate.Text = addRate.ToString();

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

                            //labQuoteTotalPrice.Text = Math.Round((popTotalPrice + installPriceTotal + expressTotalPrice + specialTotal), 2).ToString();
                            //hfQuoteTotalPrice1.Value = Math.Round((popTotalPrice + installPriceTotal + expressTotalPrice + specialTotal), 2).ToString();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string msg = SubmitQuote();
            ExcuteJs("finish", msg);

        }

        /// <summary>
        /// 显示总金额
        /// </summary>
        void SetTotalPrice()
        {
            labSystemTotalPrice.Text = Math.Round((popTotalPrice + installPriceTotal + expressTotalPrice), 2).ToString();
            labQuoteTotalPrice.Text = Math.Round(popImportTotalPrice, 2).ToString();
            //hfQuoteTotalPrice1.Value = Math.Round((popTotalPrice + installPriceTotal + expressTotalPrice + specialTotal), 2).ToString();
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

                    if (ViewState["quoteId"] != null)
                    {
                        itemId = StringHelper.IsInt(ViewState["quoteId"].ToString());

                    }
                    if (itemId > 0)
                    {
                        model = quotationBll.GetModel(itemId);
                        specialDetailBll.Delete(s => s.ItemId == itemId);
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

                    if (itemId > 0)
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
                    itemId = model.Id;
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
                    specialDetailBll.Delete(s => s.ItemId == model.Id && s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.POP);
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

        /// <summary>
        /// 导入报价单
        /// </summary>
        ImportQuoteOrderBLL importQuoteOrderBll = new ImportQuoteOrderBLL();
        ImportQuoteOrder importQuoteOrderModel;
        protected void btnImportQuoteOrder_Click(object sender, EventArgs e)
        {

            if (FileUpload1.HasFile)
            {
                if (itemId == 0)
                    SubmitQuote();
                if (itemId > 0)
                {
                    //ViewState["quoteId"] = itemId;
                    StringBuilder folder = new StringBuilder("QuoteFile/");
                    QuotationItem quoteModel = quotationBll.GetModel(itemId);
                    if (quoteModel != null)
                    {
                        string guidanceYear = quoteModel.GuidanceYear + "-" + quoteModel.GuidanceMonth;
                        string subjectName = ddlQuoteSubject.SelectedItem.Text;
                        folder.AppendFormat("{0}/{1}/", guidanceYear, subjectName);
                    }

                    string path = OperateFile.UpLoadFile(FileUpload1.PostedFile, folder.ToString());
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        importQuoteOrderBll.Delete(s => s.ItemId == itemId);
                        OleDbConnection conn;
                        OleDbDataAdapter da;
                        DataSet ds = null;
                        string newPath = Server.MapPath(path);
                        string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                        ExcelConnStr = ExcelConnStr.Replace("ExcelPath", newPath);
                        conn = new OleDbConnection(ExcelConnStr);
                        conn.Open();
                        //获取excel文件的sheet列表
                        DataTable dt = conn.GetSchema("Tables");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string sheetName = dt.Rows[i]["TABLE_NAME"].ToString();
                            if (sheetName.IndexOf("FilterDatabase") == -1 && sheetName.IndexOf("Print_Area") == -1 && sheetName.IndexOf("Print_Titles") == -1)
                            {
                                //string sql = "select * from [PO V4$]";
                                string sql = "select * from [" + sheetName + "]";
                                da = new OleDbDataAdapter(sql, conn);
                                ds = new DataSet();
                                da.Fill(ds);
                                //string newSheetName = sheetName.Replace("\'","").Replace("\"","");
                                if (sheetName.ToLower().Contains("po v4"))
                                {
                                    SaveSheet1(ds);
                                }
                                else if (sheetName.ToLower().Contains("rfq"))
                                {
                                    SaveSheet2(ds);
                                }
                                da.Dispose();

                            }
                        }

                        conn.Dispose();
                        conn.Close();

                        if (quoteModel != null)
                        {
                            quoteModel.QuoteFileUrl = path;
                            quotationBll.Update(quoteModel);
                        }
                        ExcuteJs("Changed");
                        Response.Redirect("AddQuotation.aspx?itemId=" + itemId, false);

                        //BindGuidanceName();
                        //BindData();
                        //BindPOPList();
                        //BindeExpressPriceImportQuote();
                    }
                }
            }
        }

        /// <summary>
        /// 保存第一个表
        /// </summary>
        /// <param name="ds"></param>
        void SaveSheet1(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                int startIndex = 11;
                int rowIndex = 11;
                string sheetTemp = string.Empty;
                string sheet = string.Empty;
                string sheetDescription = string.Empty;
                string material = string.Empty;
                string unitName = string.Empty;
                string POPArea = string.Empty;
                string unitPrice = string.Empty;
                string POPPrice = string.Empty;
                #region pop
                for (int i = startIndex; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    sheet = string.Empty;
                    sheetDescription = string.Empty;
                    material = string.Empty;
                    unitName = string.Empty;
                    POPArea = string.Empty;
                    unitPrice = string.Empty;
                    POPPrice = string.Empty;
                    sheet = dr[0].ToString();
                    sheetDescription = dr[1].ToString();
                    material = dr[2].ToString();
                    unitName = dr[3].ToString();
                    POPArea = dr[4].ToString();
                    unitPrice = dr[5].ToString();
                    POPPrice = dr[6].ToString();
                    if (i == startIndex)
                    {
                        sheetTemp = sheet;
                    }
                    if (!string.IsNullOrWhiteSpace(sheet) && sheetTemp != sheet)
                    {
                        sheetTemp = sheet;
                    }
                    if (!string.IsNullOrWhiteSpace(POPArea) && !string.IsNullOrWhiteSpace(POPPrice))
                    {
                        importQuoteOrderModel = new ImportQuoteOrder();
                        importQuoteOrderModel.AddDate = DateTime.Now;
                        importQuoteOrderModel.AddUserId = CurrentUser.UserId;
                        importQuoteOrderModel.ItemId = itemId;
                        importQuoteOrderModel.OrderType = (int)OrderTypeEnum.POP;
                        importQuoteOrderModel.POPArea = StringHelper.IsDecimal(POPArea);
                        importQuoteOrderModel.POPPrice = StringHelper.IsDecimal(POPPrice);
                        importQuoteOrderModel.Quantity = 1;
                        importQuoteOrderModel.Sheet = StringHelper.ReplaceSpace(sheetTemp.ToLower());
                        importQuoteOrderModel.SheetDescription = sheetDescription;
                        importQuoteOrderModel.Material = material;
                        importQuoteOrderModel.UnitName = unitName;
                        if (!string.IsNullOrWhiteSpace(unitPrice))
                        {
                            unitPrice = System.Text.RegularExpressions.Regex.Replace(unitPrice, @"[^\d.\d]+", "");
                        }
                        importQuoteOrderModel.UnitPrice = StringHelper.IsDecimal(unitPrice);
                        importQuoteOrderBll.Add(importQuoteOrderModel);
                    }
                    rowIndex++;
                    if (sheetTemp.ToLower().Contains("ooh") && string.IsNullOrWhiteSpace(POPArea))
                    {
                        break;
                    }
                }
                #endregion

                #region 安装费
                sheet = ds.Tables[0].Rows[rowIndex][0].ToString();
                while (!sheet.Contains("安装及服务费"))
                {
                    rowIndex++;
                    sheet = ds.Tables[0].Rows[rowIndex][0].ToString();
                }
                //rowIndex += 2;
                string priceName = string.Empty;
                string installUnitPrice = string.Empty;
                string materialSupport = string.Empty;
                string quantity = string.Empty;
                string priceType = string.Empty;
                for (int i = rowIndex + 2; i < ds.Tables[0].Rows.Count; i++)
                {
                    bool isEnd = false;
                    priceName = string.Empty;
                    installUnitPrice = string.Empty;
                    materialSupport = "basic";
                    quantity = string.Empty;
                    priceType = string.Empty;

                    DataRow dr = ds.Tables[0].Rows[i];
                    priceName = dr[0].ToString();
                    priceType = dr[1].ToString();
                    installUnitPrice = dr[5].ToString();
                    quantity = dr[6].ToString();

                    if (!string.IsNullOrWhiteSpace(priceName) && priceName.ToLower().Contains("generic"))
                    {
                        materialSupport = "generic";
                        isEnd = true;
                    }
                    else if (!string.IsNullOrWhiteSpace(priceType) && priceType.Contains("其它类型"))
                    {
                        materialSupport = "others";
                    }
                    if (!string.IsNullOrWhiteSpace(installUnitPrice))
                    {
                        installUnitPrice = System.Text.RegularExpressions.Regex.Replace(installUnitPrice, @"[^\d.\d]+", "");
                    }
                    if (!string.IsNullOrWhiteSpace(quantity))
                    {
                        importQuoteOrderModel = new ImportQuoteOrder();
                        importQuoteOrderModel.ItemId = itemId;
                        importQuoteOrderModel.AddDate = DateTime.Now;
                        importQuoteOrderModel.AddUserId = CurrentUser.UserId;
                        importQuoteOrderModel.InstallUnitPrice = StringHelper.IsDecimal(installUnitPrice);
                        importQuoteOrderModel.MaterialSupport = materialSupport;
                        importQuoteOrderModel.OrderType = (int)OrderTypeEnum.安装费;
                        importQuoteOrderModel.Quantity = StringHelper.IsInt(quantity);
                        importQuoteOrderBll.Add(importQuoteOrderModel);
                    }
                    if (isEnd)
                        break;
                }
                #endregion

            }
        }

        /// <summary>
        /// 保存第二张表，快递费和物料
        /// </summary>
        /// <param name="ds"></param>
        void SaveSheet2(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                string materialName = string.Empty;
                string typeName = string.Empty;
                string quantityStr = string.Empty;
                string unitPriceStr = string.Empty;

                int startIndex = 11;
                //bool hasData = false;
                //bool isEnd = false;
                for (int i = startIndex; i < ds.Tables[0].Rows.Count; i++)
                {
                    materialName = string.Empty;
                    typeName = string.Empty;
                    quantityStr = string.Empty;
                    unitPriceStr = string.Empty;
                    DataRow dr = ds.Tables[0].Rows[i];
                    materialName = dr[0].ToString();
                    typeName = dr[1].ToString();
                    quantityStr = dr[9].ToString();
                    unitPriceStr = dr[14].ToString();
                    if (!string.IsNullOrWhiteSpace(materialName) && !string.IsNullOrWhiteSpace(quantityStr))
                    {
                        //hasData = true;
                        int quantity = StringHelper.IsInt(quantityStr);
                        if (!string.IsNullOrWhiteSpace(unitPriceStr))
                        {
                            unitPriceStr = System.Text.RegularExpressions.Regex.Replace(unitPriceStr, @"[^\d.\d]+", "");
                        }
                        decimal unitPrice = StringHelper.IsDecimal(unitPriceStr);
                        if (quantity > 0 && unitPrice > 0)
                        {

                            importQuoteOrderModel = new ImportQuoteOrder();
                            importQuoteOrderModel.AddDate = DateTime.Now;
                            importQuoteOrderModel.AddUserId = CurrentUser.UserId;
                            importQuoteOrderModel.ItemId = itemId;
                            importQuoteOrderModel.Quantity = quantity;
                            importQuoteOrderModel.Sheet = materialName;

                            if (typeName.Contains("物流") || typeName.Contains("运费") || typeName.Contains("快递") || typeName.Contains("促销"))
                            {
                                importQuoteOrderModel.ExpressUnitPrice = unitPrice;
                                importQuoteOrderModel.OrderType = (int)OrderTypeEnum.快递费;
                            }
                            else
                            {
                                importQuoteOrderModel.UnitPrice = unitPrice;
                                importQuoteOrderModel.OrderType = (int)OrderTypeEnum.物料;
                            }
                            importQuoteOrderBll.Add(importQuoteOrderModel);
                        }
                    }
                    else
                    {
                        break;
                    }

                }

            }
        }

        QuoteAddRateAtSizeBLL quoteAddRateAtSizeBll = new QuoteAddRateAtSizeBLL();
        void ChangeSizeRate()
        {
            string changeSheet = hfAddRateSheet.Value;
            string rate = hfAddRate.Value;
            if (!string.IsNullOrWhiteSpace(changeSheet) && !string.IsNullOrWhiteSpace(rate) && StringHelper.IsDecimalVal(rate))
            {
                if (!string.IsNullOrWhiteSpace(subjectId))
                {
                    subjectIdList = StringHelper.ToIntList(subjectId, ',');
                }
                List<string> sheetList = StringHelper.ToStringList(changeSheet, ',', LowerUpperEnum.ToLower);
                StringBuilder sheetWhere = new StringBuilder();
                sheetList.ForEach(s =>
                {
                    sheetWhere.Append("'" + s + "',");
                });
                decimal newRate = StringHelper.IsDecimal(rate) / 100;
                newRate = Math.Round(newRate, 2) + 1;
                new QuoteOrderDetailBLL().UpdateRate(sheetWhere.ToString().TrimEnd(','), subjectId.TrimEnd(','), newRate, itemId);
                //QuoteOrderDetailBLL quoteOrderBll = new QuoteOrderDetailBLL();

                QuoteAddRateAtSize quoteAddRateAtSizeModel;
                var orderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具) && s.Sheet != null && sheetList.Contains(s.Sheet.ToLower()));
                if (subjectIdList.Any())
                {
                    orderList = orderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
                }
                sheetList.ForEach(s =>
                {
                    var list0 = orderList.Where(order => order.Sheet.ToLower() == s).ToList();
                    decimal price = list0.Sum(order => ((order.AutoAddTotalPrice ?? 0) - (order.DefaultTotalPrice ?? 0)));
                    quoteAddRateAtSizeModel = new QuoteAddRateAtSize();
                    quoteAddRateAtSizeModel.AddDate = DateTime.Now;
                    quoteAddRateAtSizeModel.AddUserId = CurrentUser.UserId;
                    quoteAddRateAtSizeModel.Price = price;
                    quoteAddRateAtSizeModel.QuoutItemId = itemId;
                    quoteAddRateAtSizeModel.Rate = StringHelper.IsDecimal(rate);
                    quoteAddRateAtSizeModel.Sheet = s;
                    quoteAddRateAtSizeBll.Add(quoteAddRateAtSizeModel);
                });
            }
        }

        //protected void txtAddRate_TextChanged(object sender, EventArgs e)
        //{
        //    //ChangeSizeRate();
        //    BindGuidanceName();
        //    BindData();
        //    BindPOPList();
        //}

        protected void btnRefresh_Click(object sender, EventArgs e)
        {

            BindData();
            BindGuidanceName();
            BindPOPList();
        }



        List<QuoteModel> exportQuoteOrderList = new List<QuoteModel>();
        void ExportQuotePOP()
        {
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
            }
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }

            var orderList = (from order in CurrentContext.DbContext.QuoteOrderDetail
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where (order.IsDelete == null || order.IsDelete == false)
                             && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || (order.OrderType > (int)OrderTypeEnum.POP))
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             where guidanceIdList.Contains(order.GuidanceId ?? 0)
                             && (subjectIdList.Any() ? subjectIdList.Contains(order.SubjectId ?? 0) : 1 == 1)
                             select new
                             {
                                 order,
                                 subject
                             }).ToList();
            if (regionList.Any())
            {
                orderList = orderList.Where(s => s.order.Region != null && regionList.Contains(s.order.Region.ToLower())).ToList();
            }
            if (itemId == 0)
            {
                orderList = orderList.Where(s => (s.order.QuoteItemId ?? 0) == 0).ToList();
            }

            List<QuoteOrderDetail> popOrderList = new List<QuoteOrderDetail>();
            //保存每个位置区域的订单

            if (orderList.Any())
            {
                popOrderList = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.POP || s.order.OrderType == (int)OrderTypeEnum.道具).Select(s => s.order).ToList();
                //VVIP店铺
                var vvipList = orderList.Where(s => s.order.MaterialSupport != null && s.order.MaterialSupport.ToLower() == "vvip").ToList();
                List<int> vvipShopList = vvipList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                //非VVIP店铺
                List<int> notVVIPShopList = orderList.Where(s => !vvipShopList.Contains(s.order.ShopId ?? 0)).Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                labVVIPShop.Text = vvipShopList.Count.ToString();
                labNormalShop.Text = notVVIPShopList.Count.ToString();

                List<int> selectedIdList = new List<int>();

                string currSheet = string.Empty;
                //string currentPosition = string.Empty;
                #region 橱窗区域
                //橱窗背景订单
                currSheet = "Window橱窗";
                //currentPosition = "橱窗";
                var windowOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && !s.Sheet.Contains("地铺") && !s.Sheet.Contains("贴") && !s.PositionDescription.Contains("窗贴") && s.PositionDescription != "左侧贴" && s.PositionDescription != "右侧贴" && s.PositionDescription != "地贴" && s.PositionDescription != "地铺").ToList();
                if (windowOrderList.Any())
                {
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowOrderList.Select(s => s.Id).ToList());
                    StatisticMaterialWithSizeRate(windowOrderList, ref windowList, "Window橱窗", "橱窗背景");
                    if (windowList.Any())
                    {
                        exportQuoteOrderList.AddRange(windowList);
                    }
                }
                //橱窗侧贴订单
                var windowSizeStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("侧贴") || s.Sheet.Contains("侧贴"))).ToList();
                if (windowSizeStickOrderList.Any())
                {

                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowSizeStickOrderList.Select(s => s.Id).ToList());
                    StatisticMaterialWithSizeRate(windowSizeStickOrderList, ref windowList, "Window橱窗", "橱窗侧贴");
                    if (windowList.Any())
                    {
                        exportQuoteOrderList.AddRange(windowList);
                    }
                }
                //橱窗地贴订单
                var windowFlootStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("地贴") || s.PositionDescription.Contains("地铺") || s.Sheet.Contains("地贴") || s.Sheet.Contains("地铺"))).ToList();
                if (windowFlootStickOrderList.Any())
                {

                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowFlootStickOrderList.Select(s => s.Id).ToList());
                    StatisticMaterialWithSizeRate(windowFlootStickOrderList, ref windowList, "Window橱窗", "橱窗地贴");
                    if (windowList.Any())
                    {
                        exportQuoteOrderList.AddRange(windowList);

                    }
                }
                //橱窗窗贴订单
                var windowStickOrderList = popOrderList.Where(s => (s.Sheet.Contains("橱窗") && s.PositionDescription == "窗贴") || (s.Sheet.Contains("窗贴"))).ToList();
                if (windowStickOrderList.Any())
                {

                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowStickOrderList.Select(s => s.Id).ToList());
                    StatisticMaterialWithSizeRate(windowStickOrderList, ref windowList, "Window橱窗", "窗贴");
                    if (windowList.Any())
                    {
                        exportQuoteOrderList.AddRange(windowList);

                    }
                }

                #endregion
                #region 陈列桌区域
                currSheet = "WOW 陈列桌区域";
                //currentPosition = "陈列桌";
                //陈列桌订单
                var tableOrderList = popOrderList.Where(s => s.Sheet.Contains("陈列桌") || s.Sheet.Contains("展桌")).ToList();
                if (tableOrderList.Any())
                {

                    #region 先统计台卡
                    List<int> tkSelectedIdList = new List<int>();
                    //var taiKaList = tableOrderList.Where(s => s.Sheet.Contains("台卡") || s.GraphicNo.Contains("台卡") || s.PositionDescription.Contains("台卡")).ToList();
                    var taiKaList = tableOrderList.Where(s => (s.Sheet != null && s.Sheet.Contains("台卡")) || (s.GraphicNo != null && s.GraphicNo.Contains("台卡")) || (s.PositionDescription != null && s.PositionDescription.Contains("台卡"))).ToList();

                    if (taiKaList.Any())
                    {
                        tkSelectedIdList.AddRange(taiKaList.Select(s => s.Id).ToList());
                        selectedIdList.AddRange(taiKaList.Select(s => s.Id).ToList());
                        List<QuoteModel> tkList = new List<QuoteModel>();//
                        StatisticMaterialWithSizeRate(taiKaList, ref tkList, currSheet, "陈列桌台卡");
                        if (tkList.Any())
                        {
                            exportQuoteOrderList.AddRange(tkList);

                        }
                    }
                    #endregion

                    #region 静态展
                    List<int> jtzSelectedIdList = new List<int>();
                    var jtzOrderList = tableOrderList.Where(s => (s.Sheet.Contains("静态展") || (s.GraphicNo != null && s.GraphicNo.Contains("静态展")) || (s.PositionDescription != null && s.PositionDescription.Contains("静态展"))) && !tkSelectedIdList.Contains(s.Id)).ToList();
                    if (jtzOrderList.Any())
                    {
                        jtzSelectedIdList.AddRange(jtzOrderList.Select(s => s.Id).ToList());
                        selectedIdList.AddRange(jtzOrderList.Select(s => s.Id).ToList());
                        List<QuoteModel> jtzList = new List<QuoteModel>();//
                        StatisticMaterialWithSizeRate(jtzOrderList, ref jtzList, currSheet, "陈列桌静态展");
                        if (jtzList.Any())
                        {
                            exportQuoteOrderList.AddRange(jtzList);

                        }
                    }
                    #endregion

                    #region 模特阵
                    List<int> mtzSelectedIdList = new List<int>();
                    var mtzOrderList = tableOrderList.Where(s => (s.Sheet.Contains("模特阵") || (s.GraphicNo != null && s.GraphicNo.Contains("模特阵")) || (s.PositionDescription != null && s.PositionDescription.Contains("模特阵"))) && !tkSelectedIdList.Contains(s.Id) && !jtzSelectedIdList.Contains(s.Id)).ToList();
                    if (mtzOrderList.Any())
                    {
                        mtzSelectedIdList.AddRange(mtzOrderList.Select(s => s.Id).ToList());
                        selectedIdList.AddRange(mtzOrderList.Select(s => s.Id).ToList());
                        List<QuoteModel> mtzList = new List<QuoteModel>();//
                        StatisticMaterialWithSizeRate(mtzOrderList, ref mtzList, currSheet, "陈列桌静态展");
                        if (mtzList.Any())
                        {
                            exportQuoteOrderList.AddRange(mtzList);

                        }
                    }
                    #endregion

                    #region 陈列桌桌铺
                    List<int> zpSelectedIdList = new List<int>();
                    var zpOrderList = tableOrderList.Where(s => !tkSelectedIdList.Contains(s.Id) && !jtzSelectedIdList.Contains(s.Id) && !mtzSelectedIdList.Contains(s.Id)).ToList();
                    if (zpOrderList.Any())
                    {
                        zpSelectedIdList.AddRange(zpOrderList.Select(s => s.Id).ToList());
                        selectedIdList.AddRange(zpOrderList.Select(s => s.Id).ToList());
                        List<QuoteModel> zpList = new List<QuoteModel>();//
                        StatisticMaterialWithSizeRate(zpOrderList, ref zpList, currSheet, "陈列桌桌铺");
                        if (zpList.Any())
                        {
                            exportQuoteOrderList.AddRange(zpList);

                        }
                    }
                    #endregion
                }

                #endregion
                #region 服装墙区域订单
                currSheet = "App Wall 服装墙";
                //currentPosition = "服装墙";
                var appOrderList = popOrderList.Where(s => s.Sheet.Contains("服装墙")).ToList();
                if (appOrderList.Any())
                {
                    #region 先统计台卡
                    List<int> tkSelectedIdList = new List<int>();
                    var taiKaList = appOrderList.Where(s => (s.Sheet != null && s.Sheet.Contains("台卡")) || (s.GraphicNo != null && s.GraphicNo.Contains("台卡")) || (s.PositionDescription != null && s.PositionDescription.Contains("台卡"))).ToList();

                    if (taiKaList.Any())
                    {
                        tkSelectedIdList.AddRange(taiKaList.Select(s => s.Id).ToList());
                        selectedIdList.AddRange(taiKaList.Select(s => s.Id).ToList());
                        List<QuoteModel> tkList = new List<QuoteModel>();//
                        StatisticMaterialWithSizeRate(taiKaList, ref tkList, currSheet, "服装墙台卡");
                        if (tkList.Any())
                        {
                            exportQuoteOrderList.AddRange(tkList);

                        }
                    }
                    #endregion

                    #region 除了台卡以外的
                    var otherOrderList = appOrderList.Where(s => !tkSelectedIdList.Contains(s.Id)).ToList();
                    if (otherOrderList.Any())
                    {

                        //要按店铺类型统计服装墙
                        List<string> channelList = otherOrderList.Select(s => s.Channel).Distinct().ToList();
                        channelList.ForEach(ch =>
                        {
                            var list0 = otherOrderList.Where(s => s.Channel == ch).ToList();
                            selectedIdList.AddRange(list0.Select(s => s.Id).ToList());
                            List<QuoteModel> oList = new List<QuoteModel>();//
                            StatisticMaterialWithSizeRate(list0, ref oList, "App Wall 服装墙", ch + "服装墙");
                            if (oList.Any())
                            {
                                exportQuoteOrderList.AddRange(oList);
                            }
                        });


                    }
                    #endregion
                }

                #endregion
                #region 鞋墙区域订单
                currSheet = "FTW Wall 鞋墙";
                //currentPosition = "鞋墙";
                List<QuoteOrderDetail> ftwOrderList = new List<QuoteOrderDetail>();
                List<string> shoeWareSheetList = new List<string>() { "鞋墙", "凹槽", "圆吧", "弧形", "鞋吧", "圆桌", "吧台", "鞋砖", "鞋柱", "灯槽" };
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
                            ftwOrderList.AddRange(ftwOrderList0);
                            orderIdListTemp.AddRange(ftwOrderList0.Select(s => s.Id).ToList());
                        }
                    });
                }
                if (ftwOrderList.Any())
                {
                    var frameMachineTypeList = new MachineFrameTypeBLL().GetList(s => s.Sheet == "鞋墙");
                    if (frameMachineTypeList.Any())
                    {

                        //HC
                        var HCList = ftwOrderList.Where(s => s.Channel != null && (s.Channel.ToUpper() == "HC" || s.Channel.ToUpper() == "HOMECOURT")).ToList();
                        List<int> hcOrderIdList = new List<int>();
                        #region HC
                        if (HCList.Any())
                        {
                            hcOrderIdList = HCList.Select(s => s.Id).ToList();
                            List<int> hcNormalIdList = new List<int>();
                            //主KV
                            List<QuoteOrderDetail> ftwQuoteOrderListKV = HCList.Where(s => s.Sheet.Contains("鞋墙")).ToList();
                            if (ftwQuoteOrderListKV.Any())
                            {

                                hcNormalIdList = ftwQuoteOrderListKV.Select(s => s.Id).ToList();
                                selectedIdList.AddRange(ftwQuoteOrderListKV.Select(s => s.Id).ToList());
                                List<QuoteModel> ftwList = new List<QuoteModel>();//
                                StatisticMaterialWithSizeRate(ftwQuoteOrderListKV, ref ftwList, currSheet, "HC鞋墙主KV");
                                if (ftwList.Any())
                                {
                                    exportQuoteOrderList.AddRange(ftwList);

                                }
                            }
                            //主灯槽
                            List<QuoteOrderDetail> ftwQuoteOrderList = HCList.Where(s => s.Sheet.Contains("凹槽") || s.Sheet.Contains("灯槽")).ToList();
                            if (ftwQuoteOrderList.Any())
                            {

                                var hcNormalIdList0 = ftwQuoteOrderList.Select(s => s.Id).ToList();
                                selectedIdList.AddRange(ftwQuoteOrderList.Select(s => s.Id).ToList());
                                if (hcNormalIdList0.Any())
                                {
                                    hcNormalIdList.AddRange(hcNormalIdList0);
                                }
                                List<QuoteModel> ftwList = new List<QuoteModel>();//
                                StatisticMaterialWithSizeRate(ftwQuoteOrderList, ref ftwList, currSheet, "HC鞋墙灯槽");
                                if (ftwList.Any())
                                {
                                    exportQuoteOrderList.AddRange(ftwList);

                                }

                            }
                            //鞋吧，弧形吧的其他位置
                            List<QuoteOrderDetail> ftwQuoteOrderListOthers = HCList.Where(s => !hcNormalIdList.Contains(s.Id)).ToList();
                            if (ftwQuoteOrderListOthers.Any())
                            {

                                selectedIdList.AddRange(ftwQuoteOrderListOthers.Select(s => s.Id).ToList());
                                List<QuoteModel> ftwList = new List<QuoteModel>();//
                                StatisticMaterialWithSizeRate(ftwQuoteOrderListOthers, ref ftwList, currSheet, "HC圆桌+科技+鞋吧");
                                if (ftwList.Any())
                                {
                                    exportQuoteOrderList.AddRange(ftwList);
                                }

                            }
                        }
                        #endregion

                        #region 非HC
                        var notHCList = ftwOrderList.Where(s => !hcOrderIdList.Contains(s.Id)).ToList();
                        List<int> NotHCOrderIdList = new List<int>();
                        if (notHCList.Any())
                        {
                            var sizeTotalList = new MachineFrameSizeBLL().GetList(s => s.Id > 0);
                            var frameMachineTypeList2 = frameMachineTypeList.Where(s => !s.MachineFrameTypeName.Contains("HC")).ToList();
                            frameMachineTypeList2.ForEach(frameType =>
                            {
                                List<QuoteOrderDetail> ftwQuoteOrderList = new List<QuoteOrderDetail>();
                                //主KV
                                var sizelist1 = sizeTotalList.Where(s => s.MachineFrameTypeId == frameType.Id && s.FrameType == 1).ToList();
                                sizelist1.ForEach(size =>
                                {
                                    List<QuoteOrderDetail> order1 = notHCList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                    if (order1.Any())
                                    {

                                        selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        NotHCOrderIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        ftwQuoteOrderList.AddRange(order1);
                                    }
                                });
                                List<QuoteModel> ftwList = new List<QuoteModel>();//
                                StatisticMaterialWithSizeRate(ftwQuoteOrderList, ref ftwList, currSheet, "鞋墙主KV");
                                if (ftwList.Any())
                                {
                                    exportQuoteOrderList.AddRange(ftwList);
                                }

                                //灯槽
                                ftwQuoteOrderList = new List<QuoteOrderDetail>();
                                var sizelist2 = sizeTotalList.Where(s => s.MachineFrameTypeId == frameType.Id && s.FrameType == 2).ToList();
                                sizelist2.ForEach(size =>
                                {
                                    List<QuoteOrderDetail> order1 = notHCList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                    if (order1.Any())
                                    {

                                        selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        NotHCOrderIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        ftwQuoteOrderList.AddRange(order1);
                                    }
                                });
                                ftwList = new List<QuoteModel>();
                                StatisticMaterialWithSizeRate(ftwQuoteOrderList, ref ftwList, currSheet, "灯槽");
                                if (ftwList.Any())
                                {
                                    exportQuoteOrderList.AddRange(ftwList);
                                }
                            });
                        }
                        //非大货器架的鞋墙
                        var otherSubjectOrderList = notHCList.Where(s => !NotHCOrderIdList.Contains(s.Id)).ToList();
                        if (otherSubjectOrderList.Any())
                        {

                            selectedIdList.AddRange(otherSubjectOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> ftwList = new List<QuoteModel>();//
                            StatisticMaterialWithSizeRate(otherSubjectOrderList, ref ftwList, currSheet, "鞋墙");
                            if (ftwList.Any())
                            {
                                exportQuoteOrderList.AddRange(ftwList);
                            }

                        }
                        #endregion
                    }
                }

                #endregion
                #region SMU区域订单
                currSheet = "In-store SMU 店内其它位置SMU";
                var smuOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("smu")).ToList();
                if (smuOrderList.Any())
                {

                    selectedIdList.AddRange(smuOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> smuList = new List<QuoteModel>();//
                    StatisticMaterialWithSizeRate(smuOrderList, ref smuList, currSheet, "SMU");
                    if (smuList.Any())
                    {
                        exportQuoteOrderList.AddRange(smuList);
                    }
                }
                #endregion
                #region 中岛
                var zdOrderList = popOrderList.Where(s => s.Sheet.Contains("中岛")).ToList();
                if (zdOrderList.Any())
                {

                    selectedIdList.AddRange(zdOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> smuList = new List<QuoteModel>();//
                    StatisticMaterialWithSizeRate(zdOrderList, ref smuList, currSheet, "中岛");
                    if (smuList.Any())
                    {
                        exportQuoteOrderList.AddRange(smuList);

                    }
                }

                #endregion
                #region 收银台区域订单/OOH区域订单
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

                    List<string> sheets = otherSheetOrderList.Select(s => s.Sheet).Distinct().ToList();
                    sheets.ForEach(sheet =>
                    {
                        var orderList0 = otherSheetOrderList.Where(s => s.Sheet == sheet).ToList();
                        List<QuoteModel> smuList = new List<QuoteModel>();//
                        StatisticMaterialWithSizeRate(orderList0, ref smuList, currSheet, sheet);
                        if (smuList.Any())
                        {
                            exportQuoteOrderList.AddRange(smuList);

                        }
                    });

                }

                #endregion

                #region 收银台区域订单
                currSheet = "Cashier Desk 收银台区域";
                if (cashierOrderList.Any())
                {
                    List<QuoteModel> cashierList = new List<QuoteModel>();//
                    StatisticMaterialWithSizeRate(cashierOrderList, ref cashierList, currSheet, "收银台");
                    exportQuoteOrderList.AddRange(cashierList);
                }
                #endregion
                #region OOH区域订单
                currSheet = "OOH 店外非橱窗位置";
                if (oohOrderList.Any())
                {
                    List<QuoteModel> oohList = new List<QuoteModel>();//
                    StatisticMaterialWithSizeRate(oohOrderList, ref oohList, currSheet, "OOH");
                    exportQuoteOrderList.AddRange(oohList);
                }
                #endregion
            }


        }

        /// <summary>
        /// 导出报价单模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            ExportQuotePOP();
            if (exportQuoteOrderList.Any())
            {
                string templateFileName = "项目报价模板";
                string path = ConfigurationManager.AppSettings["ExportTemplate2003"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                ISheet sheet = workBook.GetSheetAt(0);

                int startRow = 12;
                int oneSheetRowCount = 0;
                int insertRowTotalCount = 0;
                string currSheet = string.Empty;
                string lastSheet = string.Empty;

                string subjectName = ddlQuoteSubject.SelectedItem.Text;

                string vvipShopCount = labVVIPShop.Text;
                string normalShopCount = labNormalShop.Text;

                IRow dataRowSubjectName0 = sheet.GetRow(5);
                dataRowSubjectName0.GetCell(5).SetCellValue(vvipShopCount);

                IRow dataRowSubjectName1 = sheet.GetRow(7);
                dataRowSubjectName1.GetCell(1).SetCellValue(subjectName);
                dataRowSubjectName1.GetCell(5).SetCellValue(normalShopCount);


                #region 导出POP

                
                exportQuoteOrderList.ToList().ForEach(s =>
                {
                    bool isGo = false;
                    if (startRow == 12)
                    {

                        currSheet = s.Sheet;
                        string cellVal = string.Empty;
                        IRow row0 = sheet.GetRow(startRow);
                        if (row0 != null)
                        {
                            cellVal = row0.Cells[0].ToString();
                        }
                        while (string.IsNullOrWhiteSpace(cellVal) || (!string.IsNullOrWhiteSpace(cellVal) && StringHelper.ReplaceSpace(cellVal).ToLower() != StringHelper.ReplaceSpace(currSheet).ToLower()))
                        {
                            startRow++;
                            row0 = sheet.GetRow(startRow);
                            if (row0 != null)
                            {
                                cellVal = row0.Cells[0].ToString();
                            }
                            if (!string.IsNullOrWhiteSpace(cellVal) && cellVal.Contains("材质成本统计"))
                                break;
                        }

                        if (!string.IsNullOrWhiteSpace(cellVal) && StringHelper.ReplaceSpace(cellVal).ToLower() == StringHelper.ReplaceSpace(currSheet).ToLower())
                        {
                            isGo = true;
                            int listCount = exportQuoteOrderList.Where(q => q.Sheet == currSheet).Count();
                            if (listCount > 3)
                            {
                                IRow sourceRow = sheet.GetRow(startRow + 1);
                                int insertRowCount = listCount - 3;
                                insertRowTotalCount += insertRowCount;
                                InsertRow(sheet, startRow + 2, insertRowCount, sourceRow);
                            }
                        }
                    }
                    else
                    {
                        if (currSheet == s.Sheet)
                        {
                            oneSheetRowCount++;
                            isGo = true;
                        }
                        else
                        {
                            string cellVal = string.Empty;
                            IRow row0 = sheet.GetRow(startRow);
                            if (row0 != null)
                            {
                                cellVal = row0.Cells[0].ToString();
                            }
                            while (string.IsNullOrWhiteSpace(cellVal) || (!string.IsNullOrWhiteSpace(cellVal) && StringHelper.ReplaceSpace(cellVal).ToLower() != StringHelper.ReplaceSpace(s.Sheet).ToLower()))
                            {
                                startRow++;
                                row0 = sheet.GetRow(startRow);
                                if (row0 != null)
                                {
                                    cellVal = row0.Cells[0].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(cellVal) && cellVal.Contains("材质成本统计"))
                                    break;
                            }
                            if (!string.IsNullOrWhiteSpace(cellVal) && StringHelper.ReplaceSpace(cellVal).ToLower() == StringHelper.ReplaceSpace(s.Sheet).ToLower())
                            {
                                isGo = true;

                                oneSheetRowCount = 0;
                                currSheet = s.Sheet;
                                int listCount = exportQuoteOrderList.Where(q => q.Sheet == currSheet).Count();
                                if (listCount > 3)
                                {
                                    IRow sourceRow = sheet.GetRow(startRow + 1);
                                    int insertRowCount = listCount - 3;
                                    insertRowTotalCount += insertRowCount;
                                    InsertRow(sheet, startRow + 2, insertRowCount, sourceRow);

                                }
                            }
                        }
                    }

                    if (isGo)
                    {
                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(1).SetCellValue(s.PositionDescription);
                        dataRow.GetCell(2).SetCellValue(s.QuoteGraphicMaterial);
                        dataRow.GetCell(4).SetCellValue(double.Parse(s.Amount.ToString()));
                        
                        startRow++;
                    }

                });
                //int formulaBeginRow = 34 + insertRowTotalCount;
                //int formulaEndRow = 59 + insertRowTotalCount;
                int totalRow = 33 + insertRowTotalCount;
                int formulaBeginRow = totalRow;
                int formulaEndRow = 26 + totalRow;
                string prvPosition = string.Empty;
                for (int popRowIndex = 12; popRowIndex < totalRow; popRowIndex++)
                {
                    
                    IRow dataRow1 = sheet.GetRow(popRowIndex);
                    if (dataRow1 == null)
                        dataRow1 = sheet.CreateRow(popRowIndex);
                    for (int i = 0; i < 20; i++)
                    {
                        ICell cell = dataRow1.GetCell(i);
                        if (cell == null)
                            cell = dataRow1.CreateCell(i);

                    }
                    if (popRowIndex == 0)
                    {
                        prvPosition = dataRow1.Cells[0].ToString();
                    }
                    else
                    {
                        if (dataRow1.Cells[0].ToString() == prvPosition)
                            sheet.AddMergedRegion(new CellRangeAddress(popRowIndex - 2, popRowIndex, 0, 0));
                        else
                            prvPosition = dataRow1.Cells[0].ToString();
                    }
                }

                //下拉材质设置
                HSSFDataValidation validate = SetValidata(12,
                   totalRow, 2, 2, formulaBeginRow, formulaEndRow);
                // 设定规则  
                sheet.AddValidationData(validate);
                #endregion


                #region 安装费
                decimal oohLevel1Count = 0;
                decimal oohLevel2Count = 0;
                decimal oohLevel3Count = 0;
                decimal oohLevel4Count = 0;
                decimal basicLevel1Count = 0;
                decimal basicLevel2Count = 0;
                decimal basicLevel3Count = 0;


                if (Session["InstallPriceQuoteModel"] != null)
                {
                    InstallPriceQuoteModelList = Session["InstallPriceQuoteModel"] as List<InstallPriceQuoteModel>;
                }
                if (InstallPriceQuoteModelList.Any())
                {
                    //startRow += insertRowTotalCount;
                    var oohInstallList = InstallPriceQuoteModelList.Where(s => s.ChargeItem == "OOHInstall").ToList();
                    if (oohInstallList.Any())
                    {
                        oohInstallList.ForEach(s =>
                        {
                            int rowIndex = 0;
                            if (s.UnitPrice == 5000)
                            {
                                //sheet.Cells[46 + addRowCount, 7].Value = s.Amount.ToString();
                                rowIndex = 46 + insertRowTotalCount;
                                oohLevel1Count += s.Amount;

                            }
                            if (s.UnitPrice == 2700)
                            {
                                //sheet.Cells[47 + addRowCount, 7].Value = s.Amount.ToString();
                                rowIndex = 47 + insertRowTotalCount;
                                oohLevel2Count += s.Amount;
                            }
                            if (s.UnitPrice == 1800)
                            {
                                //sheet.Cells[48 + addRowCount, 7].Value = s.Amount.ToString();
                                rowIndex = 48 + insertRowTotalCount;
                                oohLevel3Count += s.Amount;
                            }
                            if (s.UnitPrice == 600)
                            {
                                //sheet.Cells[49 + addRowCount, 7].Value = s.Amount.ToString();
                                rowIndex = 49 + insertRowTotalCount;
                                oohLevel4Count += s.Amount;
                            }
                            if (rowIndex > 0)
                            {
                                IRow dataRow = sheet.GetRow(rowIndex);
                                if (dataRow == null)
                                    dataRow = sheet.CreateRow(rowIndex);
                                for (int i = 0; i < 20; i++)
                                {
                                    ICell cell = dataRow.GetCell(i);
                                    if (cell == null)
                                        cell = dataRow.CreateCell(i);

                                }
                                dataRow.GetCell(6).SetCellValue(double.Parse(s.Amount.ToString()));
                            }
                        });
                    }
                    var windowInstallList = InstallPriceQuoteModelList.Where(s => s.ChargeItem == "WindowInstall").ToList();
                    if (windowInstallList.Any())
                    {
                        windowInstallList.ForEach(s =>
                        {
                            int rowIndex = 0;
                            if (s.UnitPrice == 1000)
                            {
                                //sheet.Cells[50 + addRowCount, 7].Value = s.Amount.ToString();
                                rowIndex = 50 + insertRowTotalCount;
                            }
                            if (s.UnitPrice == 500)
                            {
                                //sheet.Cells[52 + addRowCount, 7].Value = s.Amount.ToString();
                                rowIndex = 52 + insertRowTotalCount;
                            }
                            if (s.UnitPrice == 200)
                            {
                                //sheet.Cells[54 + addRowCount, 7].Value = s.Amount.ToString();
                                rowIndex = 54 + insertRowTotalCount;
                            }
                            if (rowIndex > 0)
                            {
                                IRow dataRow = sheet.GetRow(rowIndex);
                                if (dataRow == null)
                                    dataRow = sheet.CreateRow(rowIndex);
                                for (int i = 0; i < 20; i++)
                                {
                                    ICell cell = dataRow.GetCell(i);
                                    if (cell == null)
                                        cell = dataRow.CreateCell(i);

                                }
                                dataRow.GetCell(6).SetCellValue(double.Parse(s.Amount.ToString()));
                            }
                        });
                    }
                    var basicInstallList = InstallPriceQuoteModelList.Where(s => s.ChargeItem == "BasicInstall").ToList();
                    if (basicInstallList.Any())
                    {
                        basicInstallList.ForEach(s =>
                        {
                            int rowIndex = 0;
                            if (s.ChargeType == "generic")
                            {
                                //sheet.Cells[58 + addRowCount, 7].Value = s.Amount.ToString();
                                rowIndex = 58 + insertRowTotalCount;
                            }
                            else if (s.ChargeType == "kids")
                            {
                                if (s.UnitPrice == 500)
                                {
                                    //sheet.Cells[56 + addRowCount, 7].Value = s.Amount.ToString();
                                    rowIndex = 56 + insertRowTotalCount;
                                }
                                if (s.UnitPrice == 150)
                                {
                                    //sheet.Cells[57 + addRowCount, 7].Value = s.Amount.ToString();
                                    rowIndex = 57 + insertRowTotalCount;
                                }
                            }
                            else
                            {
                                if (s.UnitPrice == 800)
                                {
                                    //sheet.Cells[51 + addRowCount, 7].Value = s.Amount.ToString();
                                    rowIndex = 51 + insertRowTotalCount;
                                    basicLevel1Count += s.Amount;
                                }
                                if (s.UnitPrice == 400)
                                {
                                    //sheet.Cells[53 + addRowCount, 7].Value = s.Amount.ToString();
                                    rowIndex = 53 + insertRowTotalCount;
                                    basicLevel2Count += s.Amount;
                                }
                                if (s.UnitPrice == 150)
                                {
                                    //sheet.Cells[55 + addRowCount, 7].Value = s.Amount.ToString();
                                    rowIndex = 55 + insertRowTotalCount;
                                    basicLevel3Count += s.Amount;
                                }
                            }
                            if (rowIndex > 0)
                            {
                                IRow dataRow = sheet.GetRow(rowIndex);
                                if (dataRow == null)
                                    dataRow = sheet.CreateRow(rowIndex);
                                for (int i = 0; i < 20; i++)
                                {
                                    ICell cell = dataRow.GetCell(i);
                                    if (cell == null)
                                        cell = dataRow.CreateCell(i);

                                }
                                dataRow.GetCell(6).SetCellValue(double.Parse(s.Amount.ToString()));
                            }
                        });
                    }
                }
                //折算的安装费
                List<SpecialPriceQuoteDetail> otherInstallPriceList = new SpecialPriceQuoteDetailBLL().GetList(s => s.ItemId == itemId);
                var otherOohList = otherInstallPriceList.Where(s => s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.OOH).ToList();
                otherOohList.ForEach(s =>
                {
                    int rowIndex = 0;
                    if (s.InstallPriceLevel == 5000)
                    {
                        //sheet.Cells[46 + addRowCount, 7].Value = s.Amount.ToString();
                        rowIndex = 46 + insertRowTotalCount;
                        oohLevel1Count += (s.Quantity ?? 0);
                        IRow dataRow = sheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(rowIndex);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(6).SetCellValue(double.Parse(oohLevel1Count.ToString()));
                    }
                    if (s.InstallPriceLevel == 2700)
                    {
                        //sheet.Cells[47 + addRowCount, 7].Value = s.Amount.ToString();
                        rowIndex = 47 + insertRowTotalCount;
                        oohLevel2Count += (s.Quantity ?? 0);

                        IRow dataRow = sheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(rowIndex);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(6).SetCellValue(double.Parse(oohLevel2Count.ToString()));
                    }
                    if (s.InstallPriceLevel == 1800)
                    {
                        //sheet.Cells[48 + addRowCount, 7].Value = s.Amount.ToString();
                        rowIndex = 48 + insertRowTotalCount;
                        oohLevel3Count += (s.Quantity ?? 0);

                        IRow dataRow = sheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(rowIndex);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(6).SetCellValue(double.Parse(oohLevel3Count.ToString()));
                    }
                    if (s.InstallPriceLevel == 600)
                    {
                        //sheet.Cells[49 + addRowCount, 7].Value = s.Amount.ToString();
                        rowIndex = 49 + insertRowTotalCount;
                        oohLevel4Count += (s.Quantity ?? 0);

                        IRow dataRow = sheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(rowIndex);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(6).SetCellValue(double.Parse(oohLevel4Count.ToString()));
                    }

                });
                var otherBasicList = otherInstallPriceList.Where(s => s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.Basic).ToList();
                otherBasicList.ForEach(s =>
                {
                    int rowIndex = 0;
                    if (s.InstallPriceLevel == 800)
                    {
                        //sheet.Cells[51 + addRowCount, 7].Value = s.Amount.ToString();
                        rowIndex = 51 + insertRowTotalCount;
                        basicLevel1Count += (s.Quantity ?? 0);

                        IRow dataRow = sheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(rowIndex);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(6).SetCellValue(double.Parse(basicLevel1Count.ToString()));
                    }
                    if (s.InstallPriceLevel == 400)
                    {
                        //sheet.Cells[53 + addRowCount, 7].Value = s.Amount.ToString();
                        rowIndex = 53 + insertRowTotalCount;
                        basicLevel2Count += (s.Quantity ?? 0);

                        IRow dataRow = sheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(rowIndex);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(6).SetCellValue(double.Parse(basicLevel2Count.ToString()));

                    }
                    if (s.InstallPriceLevel == 150)
                    {
                        //sheet.Cells[55 + addRowCount, 7].Value = s.Amount.ToString();
                        rowIndex = 55 + insertRowTotalCount;
                        basicLevel3Count += (s.Quantity ?? 0);

                        IRow dataRow = sheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(rowIndex);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(6).SetCellValue(double.Parse(basicLevel3Count.ToString()));
                    }

                });
                #endregion
               
                sheet.ForceFormulaRecalculation = true;



                //HSSFWorkbook book11 = new HSSFWorkbook();


                #region 导出快递费

                List<ExpressPriceClass> expressPriceList = new List<ExpressPriceClass>();
                if (Session["ExpressPriceQuoteModel"] != null)
                {
                    expressPriceList = Session["ExpressPriceQuoteModel"] as List<ExpressPriceClass>;
                }
                if (expressPriceList.Any())
                {
                    ISheet expressSheet = workBook.GetSheetAt(1);
                    int rowIndex = 12;
                    expressPriceList.ForEach(s =>
                    {
                        IRow dataRow = expressSheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = expressSheet.CreateRow(rowIndex);
                        for (int i = 0; i < 20; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(0).SetCellValue(s.OrderType);
                        dataRow.GetCell(1).SetCellValue(s.PriceType);
                        dataRow.GetCell(9).SetCellValue(double.Parse(s.Count.ToString()));
                        dataRow.GetCell(12).SetCellValue(double.Parse(s.Price.ToString()));
                        rowIndex++;
                    });
                    expressSheet = null;
                }
                #endregion

                #region 高空安装店铺明细
                List<Shop> OOHInstallShopList = new List<Shop>();
                if (Session["AddQuoteOOHInstalShopList"] != null)
                {
                    OOHInstallShopList = Session["AddQuoteOOHInstalShopList"] as List<Shop>;
                }
                if (OOHInstallShopList.Any())
                {
                    ISheet oohInstallShopSheet = workBook.GetSheetAt(2);
                    int rowIndex = 11;
                    OOHInstallShopList.ForEach(ooh =>
                    {
                        IRow dataRow = oohInstallShopSheet.GetRow(rowIndex);
                        if (dataRow == null)
                            dataRow = oohInstallShopSheet.CreateRow(rowIndex);
                        for (int i = 0; i < 10; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(1).SetCellValue(ooh.ShopNo);
                        dataRow.GetCell(2).SetCellValue(ooh.ShopName);
                        dataRow.GetCell(6).SetCellValue(double.Parse(ooh.OOHInstallPrice.ToString()));
                        rowIndex++;
                    });
                    oohInstallShopSheet = null;
                }
                #endregion

                #region POP订单
                if (Session["AddQuoteOrderList"] != null)
                {
                    List<QuoteOrderDetail> quoteOrderDetailList = Session["AddQuoteOrderList"] as List<QuoteOrderDetail>;
                    if (quoteOrderDetailList.Any())
                    {
                        var list = (from order in quoteOrderDetailList
                                    join guidance in CurrentContext.DbContext.SubjectGuidance
                                    on order.GuidanceId equals guidance.ItemId
                                    join subject in CurrentContext.DbContext.Subject
                                    on order.SubjectId equals subject.Id
                                    select new
                                    {
                                        order,
                                        subject,
                                        guidance
                                    }).ToList();
                        ISheet orderSheet = workBook.GetSheetAt(3);
                        int rowIndex = 1;
                        list.ForEach(s =>
                        {
                            IRow dataRow = orderSheet.GetRow(rowIndex);
                            if (dataRow == null)
                                dataRow = orderSheet.CreateRow(rowIndex);
                            for (int i = 0; i < 25; i++)
                            {
                                ICell cell = dataRow.GetCell(i);
                                if (cell == null)
                                    cell = dataRow.CreateCell(i);

                            }
                            dataRow.GetCell(0).SetCellValue(s.guidance.ItemName);
                            dataRow.GetCell(1).SetCellValue(s.subject.SubjectName);
                            dataRow.GetCell(2).SetCellValue("POP");
                            if (s.order.AddDate != null)
                                dataRow.GetCell(3).SetCellValue(DateTime.Parse(s.order.AddDate.ToString()).ToShortDateString());
                            dataRow.GetCell(4).SetCellValue(s.order.ShopNo);
                            dataRow.GetCell(5).SetCellValue(s.order.ShopName);
                            dataRow.GetCell(6).SetCellValue(s.order.Province);
                            dataRow.GetCell(7).SetCellValue(s.order.City);
                            dataRow.GetCell(8).SetCellValue(s.order.CityTier);
                            dataRow.GetCell(9).SetCellValue(s.order.Channel);
                            dataRow.GetCell(10).SetCellValue(s.order.Format);
                            dataRow.GetCell(11).SetCellValue(s.order.Gender);
                            dataRow.GetCell(12).SetCellValue(s.order.ChooseImg);
                            dataRow.GetCell(13).SetCellValue(s.order.Sheet);
                            dataRow.GetCell(14).SetCellValue(s.order.MachineFrame);
                            dataRow.GetCell(15).SetCellValue(s.order.PositionDescription);
                            int quantity = int.Parse((s.order.Quantity ?? 0).ToString());
                            dataRow.GetCell(16).SetCellValue(quantity);
                            dataRow.GetCell(17).SetCellValue(s.order.GraphicMaterial);
                            dataRow.GetCell(18).SetCellValue(s.order.QuoteGraphicMaterial);
                            dataRow.GetCell(19).SetCellValue(double.Parse((s.order.UnitPrice ?? 0).ToString()));
                            double width = double.Parse((s.order.TotalGraphicWidth ?? 0).ToString());
                            double length = double.Parse((s.order.TotalGraphicLength ?? 0).ToString());
                            double area = (width * length) / 1000000 * quantity;
                            dataRow.GetCell(20).SetCellValue(width);
                            dataRow.GetCell(21).SetCellValue(length);
                            dataRow.GetCell(22).SetCellValue(area);
                            rowIndex++;
                        });
                        orderSheet = null;
                    }
                }
                #endregion
                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    sheet = null;
                    workBook = null;
                    OperateFile.DownLoadFile(ms, "活动报价模板", ".xls");

                }
            }
        }

        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportOrder_Click(object sender, EventArgs e)
        {
            List<QuoteOrderDetail> orderList0 = new List<QuoteOrderDetail>();
            if (Session["AddQuoteOrderList"] != null)
            {
                orderList0 = Session["AddQuoteOrderList"] as List<QuoteOrderDetail>;
            }
            if (orderList0.Any())
            {
                var orderList = (from order in orderList0
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on order.GuidanceId equals guidance.ItemId
                                 select new
                                 {
                                     order,
                                     subject,
                                     guidance
                                 }).ToList();
                string templateFileName = "350Template";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                ISheet tableSheet = workBook.GetSheetAt(0);

                int startRow = 1;
                string shopno = string.Empty;
                foreach (var item in orderList)
                {

                    IRow dataRow = tableSheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = tableSheet.CreateRow(startRow);
                    for (int i = 0; i < 30; i++)
                    {
                        ICell cell = dataRow.GetCell(i);
                        if (cell == null)
                            cell = dataRow.CreateCell(i);

                    }

                    dataRow.GetCell(0).SetCellValue("POP");
                    if (item.order.AddDate != null)
                        dataRow.GetCell(1).SetCellValue(DateTime.Parse(item.order.AddDate.ToString()).ToShortDateString());
                    dataRow.GetCell(2).SetCellValue(item.order.ShopNo);
                    dataRow.GetCell(3).SetCellValue(item.order.ShopName);
                    dataRow.GetCell(4).SetCellValue(item.order.Province);
                    dataRow.GetCell(5).SetCellValue(item.order.City);
                    dataRow.GetCell(6).SetCellValue(item.order.CityTier);
                    dataRow.GetCell(7).SetCellValue(item.order.Channel);
                    dataRow.GetCell(8).SetCellValue(item.order.Format);
                    dataRow.GetCell(9).SetCellValue(item.order.POPAddress);
                    dataRow.GetCell(10).SetCellValue(item.order.Contact);
                    dataRow.GetCell(11).SetCellValue(item.order.Tel);

                    dataRow.GetCell(12).SetCellValue(item.order.POSScale);
                    dataRow.GetCell(13).SetCellValue(item.order.MaterialSupport);
                    dataRow.GetCell(14).SetCellValue(item.subject.SubjectName);
                    dataRow.GetCell(15).SetCellValue(item.order.Gender);
                    dataRow.GetCell(16).SetCellValue(item.order.ChooseImg);


                    //dataRow.GetCell(12).SetCellValue(item.Category);
                    dataRow.GetCell(17).SetCellValue(item.order.Sheet);
                    dataRow.GetCell(18).SetCellValue(item.order.MachineFrame);
                    dataRow.GetCell(19).SetCellValue(item.order.PositionDescription);
                    dataRow.GetCell(20).SetCellValue(item.order.Quantity ?? 0);
                    dataRow.GetCell(21).SetCellValue(item.order.GraphicMaterial);
                    dataRow.GetCell(22).SetCellValue(item.order.QuoteGraphicMaterial);
                    dataRow.GetCell(23).SetCellValue(double.Parse((item.order.UnitPrice ?? 0).ToString()));
                    dataRow.GetCell(24).SetCellValue(double.Parse((item.order.TotalGraphicWidth ?? 0).ToString()));
                    dataRow.GetCell(25).SetCellValue(double.Parse((item.order.TotalGraphicLength ?? 0).ToString()));
                    decimal area = ((item.order.TotalGraphicWidth ?? 0) * (item.order.TotalGraphicLength ?? 0)) / 1000000;
                    dataRow.GetCell(26).SetCellValue(double.Parse(area.ToString()));
                    dataRow.GetCell(27).SetCellValue(double.Parse((item.order.AutoAddTotalPrice ?? 0).ToString()));
                    //其他备注
                    dataRow.GetCell(28).SetCellValue(item.order.Remark);
                    dataRow.GetCell(29).SetCellValue(item.order.IsInstall);
                    //dataRow.GetCell(27).SetCellValue(item.NewFormat);
                    startRow++;

                }

                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    tableSheet = null;
                    workBook = null;
                    OperateFile.DownLoadFile(ms, "报价订单明细");
                }
            }
        }

        /// <summary>
        /// 插入行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="insertRowIndex"></param>
        /// <param name="insertRowCount"></param>
        /// <param name="formatRow"></param>
        private void InsertRow(ISheet sheet, int insertRowIndex, int insertRowCount, IRow formatRow)
        {


            //for (int i = 0; i < insertRowCount; i++)
            //{
            //   sheet.ShiftRows(i, sheet.LastRowNum, 1, true, false);
            //   insertRowIndex += i;
            //}
            sheet.ShiftRows(insertRowIndex, sheet.LastRowNum, insertRowCount);
            //sheet.ShiftRows(insertRowIndex + insertRowIndex, sheet.LastRowNum + insertRowIndex, insertRowCount, true, false);
            //sheet.ShiftRows(insertRowIndex, sheet.LastRowNum + insertRowIndex, insertRowCount, true, false);
            for (int i = insertRowIndex; i < insertRowIndex + insertRowCount; i++)
            {
                IRow targetRow = null;
                ICell sourceCell = null;
                ICell targetCell = null;
                targetRow = sheet.CreateRow(i);
                for (int m = formatRow.FirstCellNum; m < formatRow.LastCellNum; m++)
                {
                    sourceCell = formatRow.GetCell(m);
                    if (sourceCell == null)
                    {
                        continue;
                    }
                    targetCell = targetRow.CreateCell(m);
                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);
                    try
                    {
                        //if (m == 3)
                        //{
                        //    targetCell.SetCellFormula(string.Format("=IF(ISBLANK(C14),\"\",VLOOKUP(C14,X:Z,3,0))",i));
                        //}
                        //if (m == 5)
                        //{
                        //    targetCell.SetCellFormula(string.Format("=IF(ISBLANK(C14),,VLOOKUP(C14,X:Y,2,0))", i));
                        //}
                        //if (m == 6)
                        //{
                        //    targetCell.SetCellFormula(string.Format("=F14*E14", i));
                        //}
                    }
                    catch { }
                }
                //ICell targetCell11 = targetRow.CreateCell(8);
                //targetCell11.SetCellValue("new");
            }

            for (int i = insertRowIndex; i < insertRowIndex + insertRowCount; i++)
            {
                IRow firstTargetRow = sheet.GetRow(i);
                ICell firstSourceCell = null;
                ICell firstTargetCell = null;

                for (int m = formatRow.FirstCellNum; m < formatRow.LastCellNum; m++)
                {
                    firstSourceCell = formatRow.GetCell(m, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    if (firstSourceCell == null)
                    {
                        continue;
                    }
                    firstTargetCell = firstTargetRow.GetCell(m, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    firstTargetCell.CellStyle = firstSourceCell.CellStyle;
                    firstTargetCell.SetCellType(firstSourceCell.CellType);
                    try
                    {
                        if (m == 3)
                        {
                            firstTargetCell.SetCellFormula(string.Format("IF(ISBLANK(C{0}),\"\",VLOOKUP(C{0},X:Z,3,0))", i + 1));
                        }
                        if (m == 5)
                        {
                            firstTargetCell.SetCellFormula(string.Format("IF(ISBLANK(C{0}),,VLOOKUP(C{0},X:Y,2,0))", i + 1));
                        }
                        if (m == 6)
                        {
                            firstTargetCell.SetCellFormula(string.Format("F{0}*E{0}", i + 1));
                        }
                        if (m == 6)
                        {
                            firstTargetCell.SetCellFormula(string.Format("F{0}*E{0}", i + 1));
                        }
                    }
                    catch { }
                }
            }



        }

        /// <summary>
        /// 设置单元格有效性(下拉)
        /// </summary>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <param name="firstCell"></param>
        /// <param name="lastCell"></param>
        /// <param name="formulaBeginRow"></param>
        /// <param name="formulaEndRow"></param>
        /// <returns></returns>
        HSSFDataValidation SetValidata(int firstRow, int lastRow, int firstCell, int lastCell, int formulaBeginRow, int formulaEndRow)
        {

            //DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(new String[] { "aa", "bb", "cc" });
            DVConstraint constraint = DVConstraint.CreateFormulaListConstraint(string.Format("$X${0}:$X${1}", formulaBeginRow, formulaEndRow));
            // 设定在哪个单元格生效  
            CellRangeAddressList regions = new CellRangeAddressList(firstRow, lastRow,
                    firstCell, lastCell);
            // 创建规则对象  
            HSSFDataValidation ret = new HSSFDataValidation(regions, constraint);
            return ret;
        }
    }

    public class OtherInstallPriceClass
    {
        public int ChangeType { get; set; }
        public string PriceType { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class ExpressPriceClass
    {
        public string PriceType { get; set; }
        public string OrderType { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }

    }


}
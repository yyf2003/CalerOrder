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
using System.Configuration;
using System.Web.UI.HtmlControls;
using NPOI.SS.UserModel;
using System.Data;
using NPOI;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.IO;

namespace WebApp.QuoteOrderManager
{
    public partial class CheckQuotation : BasePage
    {
        int itemId;
        string month = string.Empty;
        string guidanceId = string.Empty;
        string subjectCategory = string.Empty;
        int customerId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemId"] != null)
            {
                itemId = int.Parse(Request.QueryString["itemId"]);
            }
            if (!IsPostBack)
            {
                BindData();
                BindGuidanceName();
                BindPOPList();
            }
        }

        decimal expressTotalPrice = 0;
        decimal installPriceTotal = 0;
        decimal popTotalPrice = 0;
        decimal popTotalArea = 0;
        int specialTotal = 0;
        int oohIndex = 0;
        int basicIndex = 0;

        List<int> guidanceIdList = new List<int>();
        List<int> categoryIdList = new List<int>();

        QuotationItemBLL quotationBll = new QuotationItemBLL();
        List<SpecialPriceQuoteDetail> specialPriceQuoteDetailList = new List<SpecialPriceQuoteDetail>();
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
                specialPriceQuoteDetailList = new SpecialPriceQuoteDetailBLL().GetList(s => s.ItemId == itemId);
            }
        }

        void BindGuidanceName()
        {
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
        }

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
                             && (categoryIdList.Any() ? categoryIdList.Contains(subject.SubjectCategoryId ?? 0) : 1 == 1)
                             select new
                             {
                                 order,
                                 subject
                             }).ToList();
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
                #region 橱窗区域
                //橱窗背景订单

                var windowOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && !s.Sheet.Contains("地铺") && !s.Sheet.Contains("贴") && !s.PositionDescription.Contains("窗贴") && s.PositionDescription != "左侧贴" && s.PositionDescription != "右侧贴" && s.PositionDescription != "地贴" && s.PositionDescription != "地铺").ToList();
                if (windowOrderList.Any())
                {
                    popOrderListSave.AddRange(windowOrderList);
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(windowOrderList, ref windowList, "Window橱窗", "橱窗背景");
                    if (windowList.Any())
                    {
                        quoteOrderList.AddRange(windowList);
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
                        quoteOrderList.AddRange(windowList);
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
                        quoteOrderList.AddRange(windowList);
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
                        quoteOrderList.AddRange(windowList);
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["Window橱窗"] = popOrderListSave;
                }
                #endregion
                #region 陈列桌区域
                popOrderListSave = new List<QuoteOrderDetail>();
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
                                    quoteOrderList.AddRange(tableList);
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
                                quoteOrderList.AddRange(tableJTZList);
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
                                quoteOrderList.AddRange(tableModuleList);
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
                                quoteOrderList.AddRange(tbList);
                            }

                        }
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["WOW 陈列桌区域"] = popOrderListSave;
                }
                #endregion
                #region 服装墙区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
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

                                    quoteOrderList.AddRange(appList);
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
                                quoteOrderList.AddRange(oList);
                            }
                        });


                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["App Wall 服装墙"] = popOrderListSave;
                }
                #endregion
                #region 鞋墙区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
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
                                    quoteOrderList.AddRange(ftwList);
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
                                    quoteOrderList.AddRange(ftwList);
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
                                    quoteOrderList.AddRange(ftwList);
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
                                    quoteOrderList.AddRange(ftwList);
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
                                    quoteOrderList.AddRange(ftwList);
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
                                quoteOrderList.AddRange(ftwList);
                            }

                        }
                        #endregion
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["FTW Wall 鞋墙"] = popOrderListSave;
                }
                #endregion
                #region SMU区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                var smuOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("smu")).ToList();
                if (smuOrderList.Any())
                {
                    popOrderListSave.AddRange(smuOrderList);
                    selectedIdList.AddRange(smuOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> smuList = new List<QuoteModel>();//
                    StatisticMaterial(smuOrderList, ref smuList, "In-store SMU 店内其它位置SMU", "SMU");
                    if (smuList.Any())
                    {
                        quoteOrderList.AddRange(smuList);
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
                        quoteOrderList.AddRange(smuList);
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["In-store SMU 店内其它位置SMU"] = popOrderListSave;
                }
                #endregion
                #region 收银台区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                var cashierOrderList = popOrderList.Where(s => s.Sheet == "收银台").ToList();
                if (cashierOrderList.Any())
                {
                    popOrderListSave.AddRange(cashierOrderList);
                    selectedIdList.AddRange(cashierOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> cashierList = new List<QuoteModel>();//
                    StatisticMaterial(cashierOrderList, ref cashierList, "Cashier Desk 收银台区域", "收银台");
                    if (cashierList.Any())
                    {
                        quoteOrderList.AddRange(cashierList);
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["Cashier Desk 收银台区域"] = popOrderListSave;
                }
                #endregion
                #region OOH区域订单
                popOrderListSave = new List<QuoteOrderDetail>();
                var oohOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("ooh")).ToList();
                if (oohOrderList.Any())
                {
                    popOrderListSave.AddRange(oohOrderList);
                    selectedIdList.AddRange(oohOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> oohList = new List<QuoteModel>();//
                    StatisticMaterial(oohOrderList, ref oohList, "OOH 店外非橱窗位置", "OOH");
                    if (oohList.Any())
                    {
                        quoteOrderList.AddRange(oohList);
                    }
                }
                if (popOrderListSave.Any())
                {
                    Session["OOH 店外非橱窗位置"] = popOrderListSave;
                }
                #endregion
                #region 除了以上常规位置外的其他位置
                popOrderListSave = new List<QuoteOrderDetail>();
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
                            quoteOrderList.AddRange(smuList);
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
            CombineCell(popList, new List<string> { "sheet" });
            BindInstallPrice(popOrderList);
            Session["QuoteModel"] = quoteOrderList;

        }
        /// <summary>
        /// 缓存安装费
        /// </summary>
        List<InstallPriceQuoteModel> InstallPriceQuoteModelList = new List<InstallPriceQuoteModel>();
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
                //安装费
                List<int> installShopIdList = orderList.Where(s => s.GuidanceId == gid).Select(s => s.ShopId ?? 0).Distinct().ToList();
                List<int> subjectIdList = new SubjectBLL().GetList(s => s.GuidanceId == gid && (categoryIdList.Any() ? categoryIdList.Contains(s.SubjectCategoryId ?? 0) : 1 == 1)).Select(s => s.Id).ToList();
                //var installPriceList = installShopBll.GetList(s => s.GuidanceId == gid && installShopIdList.Contains(s.ShopId ?? 0)).ToList();
                var installPriceList = (from install in CurrentContext.DbContext.InstallPriceShopInfo
                                        join shop in CurrentContext.DbContext.Shop
                                        on install.ShopId equals shop.Id
                                        where install.GuidanceId == gid
                                        && subjectIdList.Contains(install.SubjectId ?? 0)
                                        && installShopIdList.Contains(install.ShopId ?? 0)
                                        select new
                                        {
                                            install,
                                            shop
                                        }).ToList();
                if (installPriceList.Any())
                {
                    installShopList.AddRange(installPriceList.Select(s => s.install.ShopId ?? 0).Distinct().ToList());
                    InstallPriceQuoteModel installQuoteModel;
                    installPriceList.ForEach(s =>
                    {
                        string materialSupport = (s.install.MaterialSupport != null) ? (s.install.MaterialSupport.ToLower()) : string.Empty;
                        if (materialSupport == "others")
                        {
                            //童店安装费
                            if ((s.install.WindowPrice ?? 0) > 0)
                            {
                                kidsWindowCount++;
                                kidsWindowPrice += 500;
                                installPriceTotal += 500;
                                
                            }
                            else if ((s.install.BasicPrice ?? 0) > 0)
                            {
                                kidsBasicCount++;
                                kidsBasicPrice += 150;
                                installPriceTotal += 150;
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
                                Shop shop = new Shop();
                                shop = s.shop;
                                shop.OOHInstallPrice = s.install.OOHPrice ?? 0;
                                //OOHInstallPriceList.Add(shop);
                                if (s.install.OOHPrice == 5000)
                                {
                                    oohLevelOneCount++;
                                    oohLevelOnePrice += 5000;
                                    installPriceTotal += 5000;
                                    
                                }
                                else if (s.install.OOHPrice == 2700)
                                {
                                    oohLevelTwoCount++;
                                    oohLevelTwoPrice += 2700;
                                    installPriceTotal += 2700;
                                }
                                else if (s.install.OOHPrice == 1800)
                                {
                                    oohLevelThreeCount++;
                                    oohLevelThreePrice += 1800;
                                    installPriceTotal += 1800;
                                }
                                else if (s.install.OOHPrice == 600)
                                {
                                    oohLevelFourCount++;
                                    oohLevelFourPrice += 600;
                                    installPriceTotal += 600;
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
                                //橱窗
                                if (s.install.WindowPrice == 1000)
                                {
                                    windowLevelOneCount++;
                                    windowLevelOnePrice += 1000;
                                    installPriceTotal += 1000;
                                }
                                else if (s.install.WindowPrice == 500)
                                {
                                    windowLevelTwoCount++;
                                    windowLevelTwoPrice += 500;
                                    installPriceTotal += 500;
                                }
                                else if (s.install.WindowPrice == 200)
                                {
                                    windowLevelThreeCount++;
                                    windowLevelThreePrice += 200;
                                    installPriceTotal += 200;
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
                                //店内
                                if (materialSupport.Contains("generic"))
                                {
                                    genericLevelCount++;
                                    genericLevelPrice += 150;
                                    installPriceTotal += 150;
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
                                        installPriceTotal += 800;
                                    }
                                    else if (s.install.BasicPrice == 400)
                                    {
                                        basicLevelTwoCount++;
                                        basicLevelTwoPrice += 400;
                                        installPriceTotal += 400;
                                    }
                                    else if (s.install.BasicPrice == 150)
                                    {
                                        basicLevelThreeCount++;
                                        basicLevelThreePrice += 150;
                                        installPriceTotal += 150;
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



            });
            #region 赋值
            labInstallShop.Text = installShopList.Distinct().Count().ToString();

            labOOHLevel1Count.Text = oohLevelOneCount.ToString();
            labOOHLevel1.Text = oohLevelOnePrice.ToString();

            labOOHLevel2Count.Text = oohLevelTwoCount.ToString();
            labOOHLevel2.Text = oohLevelTwoPrice.ToString();

            labOOHLevel3Count.Text = oohLevelThreeCount.ToString();
            labOOHLevel3.Text = oohLevelThreePrice.ToString();

            labOOHLevel4Count.Text = oohLevelFourCount.ToString();
            labOOHLevel4.Text = oohLevelFourPrice.ToString();

            labWindowLevel1Count.Text = windowLevelOneCount.ToString();
            labWindowLevel1.Text = windowLevelOnePrice.ToString();

            labBasicLevel1Count.Text = basicLevelOneCount.ToString();
            labBasicLevel1.Text = basicLevelOnePrice.ToString();

            labWindowLevel2Count.Text = windowLevelTwoCount.ToString();
            labWindowLevel2.Text = windowLevelTwoPrice.ToString();

            labBasicLevel2Count.Text = basicLevelTwoCount.ToString();
            labBasicLevel2.Text = basicLevelTwoPrice.ToString();

            labWindowLevel3Count.Text = windowLevelThreeCount.ToString();
            labWindowLevel3.Text = windowLevelThreePrice.ToString();

            labBasicLevel3Count.Text = basicLevelThreeCount.ToString();
            labBasicLevel3.Text = basicLevelThreePrice.ToString();

            labKidsWindowLevelCount.Text = kidsWindowCount.ToString();
            labKidsWindowLevel.Text = kidsWindowPrice.ToString();

            labKidsBasicLevelCount.Text = kidsBasicCount.ToString();
            labKidsBasicLevel.Text = kidsBasicPrice.ToString();

            labGenericLevelCount.Text = genericLevelCount.ToString();
            labGenericLevel.Text = genericLevelPrice.ToString();
            labInstallPriceTotal.Text = installPriceTotal.ToString();





            //labQuoteTotalPrice.Text = Math.Round((popTotalPrice + installPriceTotal), 2).ToString();

            //hfQuoteTotalPrice1.Value = Math.Round((popTotalPrice + installPriceTotal), 2).ToString();
            #endregion
            //快递费/物料费
            BindeExpressPrice(orderList);
            hfQuoteTotalPrice.Value = Math.Round((popTotalPrice + installPriceTotal + expressTotalPrice), 2).ToString();
            //
            otherInstallPriceRepeater.DataSource = otherInstallPriceList.OrderByDescending(s => s.PriceType).ToList();
            otherInstallPriceRepeater.DataBind();
            OtherInstallPriceCombineCell();
            SetTotalPrice();
            if (InstallPriceQuoteModelList.Any())
            {
                Session["InstallPriceQuoteModel"] = InstallPriceQuoteModelList;
            }
        }

        void BindeExpressPrice(List<QuoteOrderDetail> orderList)
        {
            List<ExpressPriceClass> expressPriceList = new List<ExpressPriceClass>();
            ExpressPriceClass expressModel;
            //快递费订单
            var expressPriceOrderList = orderList.Where(s => (s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.运费) && (s.OrderPrice ?? 0) > 0).ToList();
            if (expressPriceOrderList.Any())
            {
                expressPriceOrderList.ForEach(s =>
                {
                    bool isExist = false;
                    int index = 0;
                    foreach (var ex in expressPriceList)
                    {
                        if (ex.PriceType == "发货费" && ex.Price == (s.OrderPrice ?? 0))
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
                        expressModel.PriceType = "发货费";
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
                    if (expressList.Any())
                    {
                        expressList.ForEach(s =>
                        {
                            bool isExist = false;
                            int index = 0;
                            foreach (var ex in expressPriceList)
                            {
                                if (ex.PriceType == "发货费" && ex.Price == (s.ExpressPrice ?? 0))
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
                                expressModel.PriceType = "发货费";
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
                    bool isExist = false;
                    int index = 0;
                    foreach (var ex in expressPriceList)
                    {
                        if (ex.PriceType == s.Sheet && ex.Price == (s.OrderPrice ?? 0))
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
                        expressModel.PriceType = s.Sheet;
                        expressPriceList.Add(expressModel);
                    }

                });
            }
            expressPriceListRepeater.DataSource = expressPriceList;
            expressPriceListRepeater.DataBind();
        }

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
                hfQuoteTotalArea.Value = popTotalArea.ToString();
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

                    HtmlTableCell cell5 = otherInstallPriceRepeater.Items[i - 1].FindControl("operatePrice") as HtmlTableCell;
                    HtmlTableCell cell6 = otherInstallPriceRepeater.Items[i].FindControl("operatePrice") as HtmlTableCell;
                    cell5.RowSpan = (cell5.RowSpan == -1) ? 1 : cell5.RowSpan;
                    cell6.RowSpan = (cell6.RowSpan == -1) ? 1 : cell6.RowSpan;
                    cell6.Visible = false;
                    cell5.RowSpan += cell6.RowSpan;
                }
            }
        }


        void SetTotalPrice()
        {
            labQuoteTotalPrice.Text = Math.Round((popTotalPrice + installPriceTotal + expressTotalPrice + specialTotal), 2).ToString();
            hfQuoteTotalPrice1.Value = Math.Round((popTotalPrice + installPriceTotal + expressTotalPrice + specialTotal), 2).ToString();
        }

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

                    Panel panelOOH = (Panel)e.Item.FindControl("PanelOOH");
                    Panel panelBasic = (Panel)e.Item.FindControl("PanelBasic");
                    if (item.PriceType == "户外安装费")
                    {
                        panelBasic.Visible = false;
                        var specialPriceDetailList0 = specialPriceQuoteDetailList.Where(s => s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.OOH).ToList();
                        if (specialPriceDetailList0.Any())
                        {
                            InstallPriceQuoteModel installQuoteModel;
                            int total = 0;
                            Label labOOHPriceCount1 = (Label)e.Item.FindControl("labOOHPriceCount1");
                            Label labOOHPriceCount2 = (Label)e.Item.FindControl("labOOHPriceCount2");
                            Label labOOHPriceCount3 = (Label)e.Item.FindControl("labOOHPriceCount3");
                            Label labOOHPriceCount4 = (Label)e.Item.FindControl("labOOHPriceCount4");
                            Label labOOHTotal = (Label)e.Item.FindControl("labOOHTotal");

                            specialPriceDetailList0.ForEach(s => {
                                if (s.InstallPriceLevel == 5000)
                                {
                                    labOOHPriceCount1.Text = (s.Quantity ?? 0).ToString();
                                }
                                if (s.InstallPriceLevel == 2700)
                                {
                                    labOOHPriceCount2.Text = (s.Quantity ?? 0).ToString();
                                }
                                if (s.InstallPriceLevel == 1800)
                                {
                                    labOOHPriceCount3.Text = (s.Quantity ?? 0).ToString();
                                }
                                if (s.InstallPriceLevel == 600)
                                {
                                    labOOHPriceCount4.Text = (s.Quantity ?? 0).ToString();
                                }
                                total += (s.Quantity ?? 0) * (s.InstallPriceLevel??0);

                                if (oohIndex == 0)
                                {
                                    bool flag = false;
                                    for (int i = 0; i < InstallPriceQuoteModelList.Count; i++)
                                    {
                                        installQuoteModel = InstallPriceQuoteModelList[i];
                                        if (installQuoteModel.ChargeItem == "OOHInstall" && installQuoteModel.ChargeType == (s.InstallPriceLevel ?? 0).ToString() && installQuoteModel.UnitPrice == (s.InstallPriceLevel ?? 0))
                                        {
                                            installQuoteModel.Amount += (s.Quantity ?? 0);
                                            flag = true;
                                        }

                                    }
                                    if (!flag)
                                    {
                                        installQuoteModel = new InstallPriceQuoteModel();
                                        installQuoteModel.Amount = (s.Quantity ?? 0);
                                        installQuoteModel.ChargeItem = "OOHInstall";
                                        installQuoteModel.ChargeType = (s.InstallPriceLevel ?? 0).ToString();
                                        installQuoteModel.UnitPrice = (s.InstallPriceLevel ?? 0);
                                        InstallPriceQuoteModelList.Add(installQuoteModel);
                                    }
                                }
                            });

                            #region 废除
                            //var level1 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 5000).FirstOrDefault();
                            //if (level1 != null)
                            //{
                            //    labOOHPriceCount1.Text = (level1.Quantity ?? 0).ToString();
                            //    total += (level1.Quantity ?? 0) * 5000;
                                
                               
                            //}
                            //var level2 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 2700).FirstOrDefault();
                            //if (level2 != null)
                            //{
                            //    labOOHPriceCount2.Text = (level2.Quantity ?? 0).ToString();
                            //    total += (level2.Quantity ?? 0) * 2700;

                            //}
                            //var level3 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 1800).FirstOrDefault();
                            //if (level3 != null)
                            //{
                            //    labOOHPriceCount3.Text = (level3.Quantity ?? 0).ToString();
                            //    total += (level3.Quantity ?? 0) * 1800;
                            //}
                            //var level4 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 600).FirstOrDefault();
                            //if (level4 != null)
                            //{
                            //    labOOHPriceCount4.Text = (level4.Quantity ?? 0).ToString();
                            //    total += (level4.Quantity ?? 0) * 600;
                            //}
                            #endregion
                            labOOHTotal.Text = total.ToString();
                            if (oohIndex == 0)
                            {
                                specialTotal += total;
                                oohIndex++;
                            }

                        }
                    }
                    else
                    {
                        panelOOH.Visible = false;
                        var specialPriceDetailList0 = specialPriceQuoteDetailList.Where(s => s.ChangeType == (int)QuoteInstallPriceChangeTypeEnum.Basic).ToList();
                        if (specialPriceDetailList0.Any())
                        {
                            InstallPriceQuoteModel installQuoteModel;
                            int total = 0;
                            Label labBasicPriceCount1 = (Label)e.Item.FindControl("labBasicPriceCount1");
                            Label labBasicPriceCount2 = (Label)e.Item.FindControl("labBasicPriceCount2");
                            Label labBasicPriceCount3 = (Label)e.Item.FindControl("labBasicPriceCount3");

                            Label labBasicTotal = (Label)e.Item.FindControl("labBasicTotal");

                            specialPriceDetailList0.ForEach(s => {
                                total += (s.Quantity ?? 0) * (s.InstallPriceLevel??0);
                                if (s.InstallPriceLevel == 800)
                                {
                                    labBasicPriceCount1.Text = (s.Quantity ?? 0).ToString();
                                }
                                if (s.InstallPriceLevel == 400)
                                {
                                    labBasicPriceCount2.Text = (s.Quantity ?? 0).ToString();
                                }
                                if (s.InstallPriceLevel == 150)
                                {
                                    labBasicPriceCount3.Text = (s.Quantity ?? 0).ToString();
                                }

                                if (basicIndex == 0)
                                {
                                    bool flag = false;
                                    for (int i = 0; i < InstallPriceQuoteModelList.Count; i++)
                                    {
                                        installQuoteModel = InstallPriceQuoteModelList[i];
                                        if (installQuoteModel.ChargeItem == "BasicInstall" && installQuoteModel.ChargeType == (s.InstallPriceLevel ?? 0).ToString() && installQuoteModel.UnitPrice == (s.InstallPriceLevel ?? 0))
                                        {
                                            installQuoteModel.Amount += (s.Quantity ?? 0);
                                            flag = true;
                                        }

                                    }
                                    if (!flag)
                                    {
                                        installQuoteModel = new InstallPriceQuoteModel();
                                        installQuoteModel.Amount = s.Quantity ?? 0;
                                        installQuoteModel.ChargeItem = "BasicInstall";
                                        installQuoteModel.ChargeType = (s.InstallPriceLevel ?? 0).ToString();
                                        installQuoteModel.UnitPrice = (s.InstallPriceLevel ?? 0);
                                        InstallPriceQuoteModelList.Add(installQuoteModel);
                                    }
                                }
                            });

                            #region 废除
                            //var level1 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 800).FirstOrDefault();
                            //if (level1 != null)
                            //{
                            //    labBasicPriceCount1.Text = (level1.Quantity ?? 0).ToString();
                            //    total += (level1.Quantity ?? 0) * 800;
                            //}
                            //var level2 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 400).FirstOrDefault();
                            //if (level2 != null)
                            //{
                            //    labBasicPriceCount2.Text = (level2.Quantity ?? 0).ToString();
                            //    total += (level2.Quantity ?? 0) * 400;
                            //}
                            //var level3 = specialPriceDetailList0.Where(s => s.InstallPriceLevel == 150).FirstOrDefault();
                            //if (level3 != null)
                            //{
                            //    labBasicPriceCount3.Text = (level3.Quantity ?? 0).ToString();
                            //    total += (level3.Quantity ?? 0) * 150;
                            //}
                            #endregion
                            labBasicTotal.Text = total.ToString();
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

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<QuoteModel> quoteOrderList = new List<QuoteModel>();
            List<InstallPriceQuoteModel> installPriceQuoteList = new List<InstallPriceQuoteModel>();
            if (Session["QuoteModel"] != null)
            {
                quoteOrderList = Session["QuoteModel"] as List<QuoteModel>;
            }
            if (Session["InstallPriceQuoteModel"] != null)
            {
                installPriceQuoteList = Session["InstallPriceQuoteModel"] as List<InstallPriceQuoteModel>;
            }
            if (quoteOrderList.Any())
            {
                string fileName = "活动报价模板";
                string templateFileName = "报价模板";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                ExcelPackage package = new ExcelPackage(outFile);
                ExcelWorksheet sheet = package.Workbook.Worksheets[1];

                string vvipShopCount = labVVIPShop.Text;
                string installShopCount = labInstallShop.Text;
                string normalShopCount = labNormalShop.Text;
                sheet.Cells[6, 6].Value = vvipShopCount;
                sheet.Cells[8, 6].Value = normalShopCount;
                int rowIndex = 13;
                quoteOrderList.ForEach(s => {
                    while (StringHelper.ReplaceSpace(sheet.Cells[rowIndex, 1].Text) != StringHelper.ReplaceSpace(s.Sheet))
                    {
                        rowIndex++;
                    }
                    sheet.Cells[rowIndex, 2].Value = s.PositionDescription;
                    sheet.Cells[rowIndex, 3].Value = s.QuoteGraphicMaterial;
                    sheet.Cells[rowIndex, 5].Value = s.Amount;
                    rowIndex++;
                });
                if (installPriceQuoteList.Any())
                {
                    var oohInstallList = installPriceQuoteList.Where(s => s.ChargeItem == "OOHInstall").ToList();
                    if (oohInstallList.Any())
                    {
                        oohInstallList.ForEach(s =>
                        {
                            if (s.UnitPrice == 5000)
                            {
                                sheet.Cells[156, 7].Value = s.Amount.ToString();
                            }
                            if (s.UnitPrice == 2700)
                            {
                                sheet.Cells[157, 7].Value = s.Amount.ToString();
                            }
                            if (s.UnitPrice == 1800)
                            {
                                sheet.Cells[158, 7].Value = s.Amount.ToString();
                            }
                            if (s.UnitPrice == 600)
                            {
                                sheet.Cells[159, 7].Value = s.Amount.ToString();
                            }
                        });
                    }
                    var windowInstallList = installPriceQuoteList.Where(s => s.ChargeItem == "WindowInstall").ToList();
                    if (windowInstallList.Any())
                    {
                        windowInstallList.ForEach(s => {
                            if (s.UnitPrice==1000)
                                sheet.Cells[160, 7].Value = s.Amount.ToString();
                            if (s.UnitPrice == 500)
                                sheet.Cells[162, 7].Value = s.Amount.ToString();
                            if (s.UnitPrice == 200)
                                sheet.Cells[164, 7].Value = s.Amount.ToString();
                        });
                    }
                    var basicInstallList = installPriceQuoteList.Where(s => s.ChargeItem == "BasicInstall").ToList();
                    if (basicInstallList.Any())
                    {
                        basicInstallList.ForEach(s => {
                            if (s.ChargeType == "generic")
                            {
                                sheet.Cells[168, 7].Value = s.Amount.ToString();
                            }
                            else if (s.ChargeType == "kids")
                            {
                                if (s.UnitPrice == 500)
                                    sheet.Cells[166, 7].Value = s.Amount.ToString();
                                if (s.UnitPrice == 150)
                                    sheet.Cells[167, 7].Value = s.Amount.ToString();
                            }
                            else
                            {
                                if (s.UnitPrice == 800)
                                    sheet.Cells[161, 7].Value = s.Amount.ToString();
                                if (s.UnitPrice == 400)
                                    sheet.Cells[163, 7].Value = s.Amount.ToString();
                                if (s.UnitPrice == 150)
                                    sheet.Cells[165, 7].Value = s.Amount.ToString();
                            }
                        });
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    ms.Flush();
                    sheet = null;
                    OperateFile.DownLoadFile(ms, fileName);

                }
            }
        }



    }


    
}
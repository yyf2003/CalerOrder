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


namespace WebApp.QuoteOrderManager
{
    public partial class AddQuotation : BasePage
    {
        string month = string.Empty;
        string guidanceName = string.Empty;
        string subjectCategory = string.Empty;



        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["month"] != null) {
                month = Request.QueryString["month"];
            }
            if (Request.QueryString["guidanceName"] != null)
            {
                guidanceName = Request.QueryString["guidanceName"];
            }
            if (Request.QueryString["subjectCategory"] != null)
            {
                subjectCategory = Request.QueryString["subjectCategory"];
            }
            if (!IsPostBack)
            {
                BindGuidanceName();
                BindPOPList();
            }
        }
        List<int> guidanceIdList = new List<int>();
        List<int> categoryIdList = new List<int>();
        void BindGuidanceName()
        {
            labGuidanceMonth.Text = month;
            if (!string.IsNullOrWhiteSpace(guidanceName))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceName,',');
                List<string> list = new SubjectGuidanceBLL().GetList(s => guidanceIdList.Contains(s.ItemId)).Select(s=>s.ItemName).ToList();
                if (list.Any())
                {
                    labGuidanceName.Text = StringHelper.ListToString(list);
                }
            }
            if (!string.IsNullOrWhiteSpace(subjectCategory))
            {
                categoryIdList = StringHelper.ToIntList(subjectCategory, ',');
                List<string> list = new ADSubjectCategoryBLL().GetList(s => categoryIdList.Contains(s.Id)).Select(s=>s.CategoryName).ToList();
                if (list.Any())
                {
                    labSubjectCategory.Text = StringHelper.ListToString(list);
                }
            }
        }

        void BindPOPList()
        {
           
            //if (!string.IsNullOrWhiteSpace(guidanceName))
            //{
            //    guidanceIdList = StringHelper.ToIntList(guidanceName, ',');
            //}
            //if (!string.IsNullOrWhiteSpace(subjectCategory))
            //{
            //    categoryIdList = StringHelper.ToIntList(subjectCategory, ',');
            //}
            //var orderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == false || s.IsDelete == null) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength != null && s.GraphicLength > 1 && s.GraphicWidth != null && s.GraphicWidth > 1) || (s.OrderType > (int)OrderTypeEnum.POP)));
            var orderList=(from order in CurrentContext.DbContext.QuoteOrderDetail
                          join subject in CurrentContext.DbContext.Subject
                          on order.SubjectId equals subject.Id
                          where (order.IsDelete==null || order.IsDelete==false)
                          && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || (order.OrderType > (int)OrderTypeEnum.POP))
                          && (subject.IsDelete==null || subject.IsDelete==false)
                          && subject.ApproveState==1
                          where guidanceIdList.Contains(order.GuidanceId??0)
                          && (categoryIdList.Any()?categoryIdList.Contains(subject.SubjectCategoryId??0):1==1)
                          select new {
                            order,
                            subject
                          }).ToList();
            List<QuoteModel> quoteOrderList = new List<QuoteModel>();
            if (orderList.Any()) {
                var popOrderList = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.POP || s.order.OrderType == (int)OrderTypeEnum.道具).Select(s=>s.order).ToList();
                //VVIP店铺
                var vvipList = orderList.Where(s => s.order.MaterialSupport != null && s.order.MaterialSupport.ToLower() == "vvip").ToList();
                List<int> vvipShopList = vvipList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                //非VVIP店铺
                List<int> notVVIPShopList = orderList.Where(s => !vvipShopList.Contains(s.order.ShopId ?? 0)).Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                labVVIPShop.Text = vvipShopList.Count.ToString();
                labNormalShop.Text = notVVIPShopList.Count.ToString();

                
                List<int> selectedIdList = new List<int>();
                #region 橱窗区域
                //橱窗背景订单
                var windowOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && !s.Sheet.Contains("地铺") && !s.Sheet.Contains("贴") && !s.PositionDescription.Contains("窗贴") && s.PositionDescription != "左侧贴" && s.PositionDescription != "右侧贴" && s.PositionDescription != "地贴" && s.PositionDescription != "地铺").ToList();
                if (windowOrderList.Any())
                {
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(windowOrderList, ref windowList, "Window橱窗", "橱窗背景");
                    if (windowList.Any()) {
                        quoteOrderList.AddRange(windowList);
                    }
                }
                //橱窗侧贴订单
                var windowSizeStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("侧贴") || s.Sheet.Contains("侧贴"))).ToList();
                if (windowSizeStickOrderList.Any())
                {
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
                    List<QuoteModel> windowList = new List<QuoteModel>();//
                    selectedIdList.AddRange(windowStickOrderList.Select(s => s.Id).ToList());
                    StatisticMaterial(windowStickOrderList, ref windowList, "Window橱窗", "窗贴");
                    if (windowList.Any())
                    {
                        quoteOrderList.AddRange(windowList);
                    }
                }
                #endregion

                #region 陈列桌区域

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

                #endregion

                #region 服装墙区域订单
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
                        selectedIdList.AddRange(otherOrderList.Select(s => s.Id).ToList());
                        List<QuoteModel> oList = new List<QuoteModel>();//
                        StatisticMaterial(otherOrderList, ref oList, "App Wall 服装墙", "服装墙");
                        if (oList.Any())
                        {
                            quoteOrderList.AddRange(oList);
                        }
                       
                    }
                }
                #endregion

                #region 鞋墙区域订单
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
                                ftwQuoteOrderList.Clear();
                                var sizelist2 = sizeTotalList.Where(s => s.MachineFrameTypeId == frameType.Id && s.FrameType == 2).ToList();
                                sizelist2.ForEach(size =>
                                {
                                    List<QuoteOrderDetail> order1 = notHCList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                    if (order1.Any())
                                    {
                                        selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                        ftwQuoteOrderList.AddRange(order1);
                                    }
                                });
                                ftwList.Clear();//
                                StatisticMaterial(ftwQuoteOrderList, ref ftwList, "FTW Wall 鞋墙", "灯槽");
                                if (ftwList.Any())
                                {
                                    quoteOrderList.AddRange(ftwList);
                                }
                            });
                        }
                        #endregion
                    }
                }
                #endregion

                #region SMU区域订单
                var smuOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("smu")).ToList();
                if (smuOrderList.Any())
                {
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
                    selectedIdList.AddRange(zdOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> smuList = new List<QuoteModel>();//
                    StatisticMaterial(zdOrderList, ref smuList, "In-store SMU 店内其它位置SMU", "中岛");
                    if (smuList.Any())
                    {
                        quoteOrderList.AddRange(smuList);
                    }
                }
                #endregion
                #region 收银台区域订单
                var cashierOrderList = popOrderList.Where(s => s.Sheet == "收银台").ToList();
                if (cashierOrderList.Any())
                {
                    selectedIdList.AddRange(cashierOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> cashierList = new List<QuoteModel>();//
                    StatisticMaterial(cashierOrderList, ref cashierList, "Cashier Desk 收银台区域", "收银台");
                    if (cashierList.Any())
                    {
                        quoteOrderList.AddRange(cashierList);
                    }
                }
                #endregion
                #region OOH区域订单
                var oohOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("ooh")).ToList();
                if (oohOrderList.Any())
                {
                    selectedIdList.AddRange(oohOrderList.Select(s => s.Id).ToList());
                    List<QuoteModel> oohList = new List<QuoteModel>();//
                    StatisticMaterial(oohOrderList, ref oohList, "OOH 店外非橱窗位置", "OOH");
                    if (oohList.Any())
                    {
                        quoteOrderList.AddRange(oohList);
                    }
                }
                #endregion
            }
            popList.DataSource = quoteOrderList;
            popList.DataBind();
            CombineCell(popList, new List<string> { "sheet" });
        }

        protected void popList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }


    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using BLL;
using Common;
using DAL;
using Models;
using NPOI.SS.UserModel;
using System.Data;
using NPOI;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;

namespace WebApp.QuoteOrderManager.handler
{
    /// <summary>
    /// ExportDetail 的摘要说明
    /// </summary>
    public class ExportDetail : IHttpHandler
    {
        HttpContext context1;
        string guidanceIds = string.Empty;
        string subjectIds = string.Empty;
        int templateType = 1;//模板类型：1 大货，2 三叶草，3 童店 4 Terrex
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";

            if (context.Request.QueryString["guidanceIds"] != null)
                guidanceIds = context.Request.QueryString["guidanceIds"];
            if (context.Request.QueryString["subjectIds"] != null)
                subjectIds = context.Request.QueryString["subjectIds"];
            if (context.Request.QueryString["templateType"] != null)
                templateType = int.Parse(context.Request.QueryString["templateType"]);

            ExportQuoteModel();
        }

        void ExportQuoteModel()
        {


            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                List<int> guidanceIdList = new List<int>();
                List<int> subjectIdList = new List<int>();
                guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
                if (!string.IsNullOrWhiteSpace(subjectIds))
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                var orderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (subjectIdList.Any() ? subjectIdList.Contains(s.SubjectId ?? 0) : 1 == 1) && (s.IsDelete == false || s.IsDelete == null) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength != null && s.GraphicLength > 1 && s.GraphicWidth != null && s.GraphicWidth > 1) || (s.OrderType>(int)OrderTypeEnum.POP)));
                
                if (orderList.Any())
                {
                    var popOrderList = orderList.Where(s=>s.OrderType==(int)OrderTypeEnum.POP || s.OrderType==(int)OrderTypeEnum.道具).ToList();
                    //VVIP店铺
                    var vvipList = orderList.Where(s => s.MaterialSupport != null && s.MaterialSupport.ToLower() == "vvip").ToList();
                    List<int> vvipShopList = vvipList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                    //非VVIP店铺
                    List<int> notVVIPShopList = orderList.Where(s => !vvipShopList.Contains(s.ShopId ?? 0)).Select(s => s.ShopId ?? 0).Distinct().ToList();


                    string fileName = "活动报价模板";
                    string templateFileName = "报价模板";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(context1.Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    ExcelPackage package = new ExcelPackage(outFile);
                    ExcelWorksheet sheet = package.Workbook.Worksheets[1];

                    sheet.Cells[6, 6].Value = vvipShopList.Count.ToString();
                    sheet.Cells[8, 6].Value = notVVIPShopList.Count.ToString();

                    //订单统计
                    int windowStartRow = 13;
                    int tableStartRow = windowStartRow + 20;
                    int appWallStartRow = tableStartRow + 20;
                    int ftwWallStartRow = appWallStartRow + 20;
                    int smuStartRow = ftwWallStartRow + 20;
                    int cashierDeskStartRow = smuStartRow + 20;
                    int oohStartRow = cashierDeskStartRow + 10;

                    if (templateType == (int)QuoteOrderTemplateEnum.DaHuo)
                    {
                        #region 大货格式

                        List<int> selectedIdList = new List<int>();

                        

                       

                        
                        
                       
                        #region 橱窗区域
                        List<QuoteModel> windowList = new List<QuoteModel>();//橱窗背景
                        List<QuoteModel> windowSizeStickList = new List<QuoteModel>();//橱窗侧贴
                        List<QuoteModel> windowFlootStickList = new List<QuoteModel>();//橱窗地贴
                        List<QuoteModel> windowStickList = new List<QuoteModel>();//窗贴
                        //橱窗背景订单
                        var windowOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && !s.Sheet.Contains("地铺") && !s.Sheet.Contains("贴") && !s.PositionDescription.Contains("窗贴") && s.PositionDescription != "左侧贴" && s.PositionDescription != "右侧贴" && s.PositionDescription != "地贴" && s.PositionDescription != "地铺").ToList();
                        if (windowOrderList.Any())
                        {
                            selectedIdList.AddRange(windowOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowOrderList, ref windowList);
                        }
                        //橱窗侧贴订单
                        var windowSizeStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("侧贴") || s.Sheet.Contains("侧贴"))).ToList();
                        if (windowSizeStickOrderList.Any())
                        {
                            selectedIdList.AddRange(windowSizeStickOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowSizeStickOrderList, ref windowSizeStickList);
                        }
                        //橱窗地贴订单
                        var windowFlootStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("地贴") || s.PositionDescription.Contains("地铺") || s.Sheet.Contains("地贴") || s.Sheet.Contains("地铺"))).ToList();
                        if (windowFlootStickOrderList.Any())
                        {
                            selectedIdList.AddRange(windowFlootStickOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowFlootStickOrderList, ref windowFlootStickList);
                        }
                        //橱窗窗贴订单
                        var windowStickOrderList = popOrderList.Where(s => (s.Sheet.Contains("橱窗") && s.PositionDescription == "窗贴") || (s.Sheet.Contains("窗贴"))).ToList();
                        if (windowStickOrderList.Any())
                        {
                            selectedIdList.AddRange(windowStickOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowStickOrderList, ref windowStickList);
                        }
                        if (windowList.Any())
                        {
                            windowList.ForEach(s =>
                            {
                                sheet.Cells[windowStartRow, 2].Value = "橱窗背景";
                                sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                windowStartRow++;
                            });

                        }
                        if (windowSizeStickList.Any())
                        {
                            windowSizeStickList.ForEach(s =>
                            {
                                sheet.Cells[windowStartRow, 2].Value = "橱窗侧贴";
                                sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                windowStartRow++;
                            });

                        }
                        if (windowFlootStickList.Any())
                        {
                            windowFlootStickList.ForEach(s =>
                            {
                                sheet.Cells[windowStartRow, 2].Value = "橱窗地贴";
                                sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                windowStartRow++;
                            });

                        }
                        if (windowStickList.Any())
                        {
                            windowStickList.ForEach(s =>
                            {
                                sheet.Cells[windowStartRow, 2].Value = "窗贴";
                                sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                windowStartRow++;
                            });

                        }
                        #endregion
                        #region 陈列桌区域
                        //List<QuoteModel> tableList = new List<QuoteModel>();//HC陈列桌
                        //List<QuoteModel> tableTKList = new List<QuoteModel>();//陈列桌台卡
                        //List<QuoteModel> tableJTZList = new List<QuoteModel>();//陈列桌静态展
                        //List<QuoteModel> tableModuleList = new List<QuoteModel>();//陈列桌模特阵
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
                                        StatisticMaterial(tableQuoteOrderList, ref tableList);
                                        if (tableList.Any())
                                        {

                                            tableList.ForEach(s =>
                                            {
                                                sheet.Cells[tableStartRow, 2].Value = ft.MachineFrameTypeName;
                                                sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                sheet.Cells[tableStartRow, 5].Value = s.Amount;
                                                tableStartRow++;
                                            });
                                        }
                                    });
                                }
                            }
                            //除了台卡以为i的陈列桌订单 
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
                                   StatisticMaterial(tableJTZOrderList, ref tableJTZList);
                                   if (tableJTZList.Any())
                                   {
                                       tableJTZList.ForEach(s =>
                                       {
                                           sheet.Cells[tableStartRow, 2].Value = "陈列桌静态展";
                                           sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                                           sheet.Cells[tableStartRow, 5].Value = s.Amount;
                                           tableStartRow++;
                                       });
                                   }
                               }

                           }
                        }
                        #region
                        //var tableOrderList = popOrderList.Where(s => s.Sheet.Contains("陈列桌") && !s.Sheet.Contains("静态展") && !s.Sheet.Contains("台卡") && !s.Sheet.Contains("模特阵") && !s.PositionDescription.Contains("台卡") && !s.PositionDescription.Contains("静态展") && !s.PositionDescription.Contains("模特阵")).ToList();
                        //if (tableOrderList.Any())
                        //{
                        //    selectedIdList.AddRange(tableOrderList.Select(s => s.Id).ToList());
                        //    StatisticMaterial(tableOrderList, ref tableList);
                        //}
                        ////陈列桌台卡订单
                        //var tableTKOrderList = popOrderList.Where(s => (s.Sheet.Contains("陈列桌") && s.PositionDescription.Contains("台卡")) || (s.Sheet.Contains("陈列桌") && s.Sheet.Contains("台卡"))).ToList();
                        //if (tableTKOrderList.Any())
                        //{
                        //    selectedIdList.AddRange(tableTKOrderList.Select(s => s.Id).ToList());
                        //    StatisticMaterial(tableTKOrderList, ref tableTKList);
                        //}
                        ////陈列桌静态展订单
                        //var tableJTZOrderList = popOrderList.Where(s => (s.Sheet.Contains("陈列桌") && s.PositionDescription.Contains("静态展")) || (s.Sheet.Contains("陈列桌") && s.Sheet.Contains("静态展"))).ToList();
                        //if (tableJTZOrderList.Any())
                        //{
                        //    selectedIdList.AddRange(tableJTZOrderList.Select(s => s.Id).ToList());
                        //    StatisticMaterial(tableJTZOrderList, ref tableJTZList);
                        //}
                        ////陈列桌模特阵订单
                        //var tableModuleOrderList = popOrderList.Where(s => (s.Sheet.Contains("陈列桌") && s.PositionDescription.Contains("模特阵")) || (s.Sheet.Contains("陈列桌") && s.Sheet.Contains("模特阵"))).ToList();
                        //if (tableModuleOrderList.Any())
                        //{
                        //    selectedIdList.AddRange(tableModuleOrderList.Select(s => s.Id).ToList());
                        //    StatisticMaterial(tableModuleOrderList, ref tableModuleList);
                        //}
                       
                        //if (tableList.Any())
                        //{
                        //    tableList.ForEach(s =>
                        //    {
                        //        sheet.Cells[tableStartRow, 2].Value = "陈列桌";
                        //        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                        //        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                        //        tableStartRow++;
                        //    });
                        //}
                        //if (tableTKList.Any())
                        //{
                        //    tableTKList.ForEach(s =>
                        //    {
                        //        sheet.Cells[tableStartRow, 2].Value = "陈列桌台卡";
                        //        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                        //        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                        //        tableStartRow++;
                        //    });
                        //}
                        //if (tableJTZList.Any())
                        //{
                        //    tableJTZList.ForEach(s =>
                        //    {
                        //        sheet.Cells[tableStartRow, 2].Value = "陈列桌静态展";
                        //        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                        //        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                        //        tableStartRow++;
                        //    });
                        //}
                        //if (tableModuleList.Any())
                        //{
                        //    tableModuleList.ForEach(s =>
                        //    {
                        //        sheet.Cells[tableStartRow, 2].Value = "陈列桌模特阵";
                        //        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                        //        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                        //        tableStartRow++;
                        //    });
                        //}
                        #endregion
                        #endregion
                        #region 服装墙区域订单
                        var appOrderList = popOrderList.Where(s => s.Sheet.Contains("服装墙") && !s.Sheet.Contains("台卡") && !s.PositionDescription.Contains("台卡")).ToList();
                        if (appOrderList.Any())
                        {
                            selectedIdList.AddRange(appOrderList.Select(s => s.Id).ToList());
                            List<string> shopChannelList = appOrderList.Select(s => s.Channel).Distinct().ToList();
                            List<QuoteModel> channelQuoteModelList = new List<QuoteModel>();//
                            foreach (string channel in shopChannelList)
                            {
                                if (!string.IsNullOrWhiteSpace(channel))
                                {
                                    var channelOrderList = appOrderList.Where(s => s.Channel == channel).ToList();
                                    channelQuoteModelList = new List<QuoteModel>();
                                    StatisticMaterial(channelOrderList, ref channelQuoteModelList);
                                    if (channelQuoteModelList.Any())
                                    {
                                        channelQuoteModelList.ForEach(s =>
                                        {
                                            sheet.Cells[appWallStartRow, 2].Value = channel + "服装墙";
                                            sheet.Cells[appWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                            sheet.Cells[appWallStartRow, 5].Value = s.Amount;
                                            appWallStartRow++;
                                        });
                                    }
                                }
                            }
                        }
                        //服装墙台卡区域订单
                        var appTKOrderList = popOrderList.Where(s => s.Sheet.Contains("服装墙") && (s.PositionDescription.Contains("台卡") || s.Sheet.Contains("台卡"))).ToList();
                        if (appTKOrderList.Any())
                        {
                            selectedIdList.AddRange(appTKOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> appTKList = new List<QuoteModel>();//
                            StatisticMaterial(appTKOrderList, ref appTKList);
                            if (appTKList.Any())
                            {
                                appTKList.ForEach(s =>
                                {
                                    sheet.Cells[appWallStartRow, 2].Value = "服装墙台卡";
                                    sheet.Cells[appWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[appWallStartRow, 5].Value = s.Amount;
                                    appWallStartRow++;
                                });
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

                            selectedIdList.AddRange(ftwOrderList.Select(s => s.Id).ToList());
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
                                        List<QuoteModel> ftwList = new List<QuoteModel>();//
                                        StatisticMaterial(ftwQuoteOrderListKV, ref ftwList);
                                        if (ftwList.Any())
                                        {
                                            ftwList.ForEach(s =>
                                            {
                                                sheet.Cells[ftwWallStartRow, 2].Value = "HC鞋墙主KV";
                                                sheet.Cells[ftwWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                sheet.Cells[ftwWallStartRow, 5].Value = s.Amount;
                                                ftwWallStartRow++;
                                            });
                                        }
                                    }
                                    //主灯槽
                                    List<QuoteOrderDetail> ftwQuoteOrderList = HCList.Where(s => s.Sheet.Contains("凹槽") || s.Sheet.Contains("灯槽")).ToList();
                                    if (ftwQuoteOrderList.Any())
                                    {
                                        var hcNormalIdList0 = ftwQuoteOrderList.Select(s => s.Id).ToList();
                                        if (hcNormalIdList0.Any())
                                        {
                                            hcNormalIdList.AddRange(hcNormalIdList0);
                                        }
                                        List<QuoteModel> ftwList = new List<QuoteModel>();//
                                        StatisticMaterial(ftwQuoteOrderList, ref ftwList);
                                        if (ftwList.Any())
                                        {
                                            ftwList.ForEach(s =>
                                            {
                                                sheet.Cells[ftwWallStartRow, 2].Value = "HC鞋墙灯槽";
                                                sheet.Cells[ftwWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                sheet.Cells[ftwWallStartRow, 5].Value = s.Amount;
                                                ftwWallStartRow++;
                                            });
                                        }
                                    }
                                    //鞋吧，弧形吧的其他位置
                                    List<QuoteOrderDetail> ftwQuoteOrderListOthers = HCList.Where(s => !hcNormalIdList.Contains(s.Id)).ToList();
                                    if (ftwQuoteOrderListOthers.Any())
                                    {
                                        List<QuoteModel> ftwList = new List<QuoteModel>();//
                                        StatisticMaterial(ftwQuoteOrderListOthers, ref ftwList);
                                        if (ftwList.Any())
                                        {
                                            ftwList.ForEach(s =>
                                            {
                                                sheet.Cells[ftwWallStartRow, 2].Value = "HC圆桌+科技+鞋吧";
                                                sheet.Cells[ftwWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                sheet.Cells[ftwWallStartRow, 5].Value = s.Amount;
                                                ftwWallStartRow++;
                                            });
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
                                                ftwQuoteOrderList.AddRange(order1);
                                            }
                                        });
                                        List<QuoteModel> ftwList = new List<QuoteModel>();//
                                        StatisticMaterial(ftwQuoteOrderList, ref ftwList);
                                        if (ftwList.Any())
                                        {
                                            ftwList.ForEach(s =>
                                            {
                                                sheet.Cells[ftwWallStartRow, 2].Value = frameType.MachineFrameTypeName + "鞋墙主KV";
                                                sheet.Cells[ftwWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                sheet.Cells[ftwWallStartRow, 5].Value = s.Amount;
                                                ftwWallStartRow++;
                                            });
                                        }

                                        //灯槽
                                        ftwQuoteOrderList.Clear();
                                        var sizelist2 = sizeTotalList.Where(s => s.MachineFrameTypeId == frameType.Id && s.FrameType == 2).ToList();
                                        sizelist2.ForEach(size =>
                                        {
                                            List<QuoteOrderDetail> order1 = notHCList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                            if (order1.Any())
                                            {
                                                ftwQuoteOrderList.AddRange(order1);
                                            }
                                        });
                                        ftwList.Clear();//
                                        StatisticMaterial(ftwQuoteOrderList, ref ftwList);
                                        if (ftwList.Any())
                                        {
                                            ftwList.ForEach(s =>
                                            {
                                                sheet.Cells[ftwWallStartRow, 2].Value = frameType.MachineFrameTypeName + "灯槽";
                                                sheet.Cells[ftwWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                sheet.Cells[ftwWallStartRow, 5].Value = s.Amount;
                                                ftwWallStartRow++;
                                            });
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
                            StatisticMaterial(smuOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = "SMU";
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region 中岛
                        var zdOrderList = popOrderList.Where(s => s.Sheet.Contains("中岛")).ToList();
                        if (zdOrderList.Any())
                        {
                            selectedIdList.AddRange(zdOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> smuList = new List<QuoteModel>();//
                            StatisticMaterial(zdOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = "中岛";
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region 收银台区域订单
                        var cashierOrderList = popOrderList.Where(s => s.Sheet == "收银台").ToList();
                        if (cashierOrderList.Any())
                        {
                            selectedIdList.AddRange(cashierOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> cashierList = new List<QuoteModel>();//
                            StatisticMaterial(cashierOrderList, ref cashierList);
                            if (cashierList.Any())
                            {
                                cashierList.ForEach(s =>
                                {
                                    sheet.Cells[cashierDeskStartRow, 2].Value = "收银台";
                                    sheet.Cells[cashierDeskStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[cashierDeskStartRow, 5].Value = s.Amount;
                                    cashierDeskStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region OOH区域订单
                        var oohOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("ooh")).ToList();
                        if (oohOrderList.Any())
                        {
                            selectedIdList.AddRange(oohOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> oohList = new List<QuoteModel>();//
                            StatisticMaterial(oohOrderList, ref oohList);
                            if (oohList.Any())
                            {
                                oohList.ForEach(s =>
                                {
                                    sheet.Cells[oohStartRow, 2].Value = "OOH";
                                    sheet.Cells[oohStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[oohStartRow, 5].Value = s.Amount;
                                    oohStartRow++;
                                });
                            }
                        }
                        #endregion

                        #region 除了以上常规位置外的其他位置
                        var otherOrderList = popOrderList.Where(s => !selectedIdList.Contains(s.Id)).ToList();
                        if (otherOrderList.Any())
                        {
                            List<string> sheets = otherOrderList.Select(s=>s.Sheet).Distinct().ToList();
                            string sheetStr = StringHelper.ListToString(sheets,"/");
                            List<QuoteModel> smuList = new List<QuoteModel>();//
                            StatisticMaterial(otherOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = sheetStr;
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion

                        #endregion
                    }
                    else if (templateType == (int)QuoteOrderTemplateEnum.SanYeCao)
                    {
                        #region 三叶草格式

                        List<int> selectedIdList = new List<int>();

                       
                        #region 橱窗区域
                        List<QuoteModel> windowList = new List<QuoteModel>();//橱窗背景
                        List<QuoteModel> windowSizeStickList = new List<QuoteModel>();//橱窗侧贴
                        List<QuoteModel> windowFlootStickList = new List<QuoteModel>();//橱窗地贴
                        List<QuoteModel> windowStickList = new List<QuoteModel>();//窗贴
                        //橱窗背景订单
                        var windowOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && !s.Sheet.Contains("贴") && !s.PositionDescription.Contains("窗贴") && s.PositionDescription.Contains("左侧贴") && s.PositionDescription.Contains("右侧贴") && s.PositionDescription.Contains("地贴")).ToList();
                        if (windowOrderList.Any())
                        {
                            selectedIdList.AddRange(windowOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowOrderList, ref windowList);
                        }

                        //橱窗侧贴订单
                        var windowSizeStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("侧贴") || s.Sheet.Contains("侧贴"))).ToList();
                        if (windowSizeStickOrderList.Any())
                        {
                            selectedIdList.AddRange(windowSizeStickOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowSizeStickOrderList, ref windowSizeStickList);
                        }
                        //橱窗地贴订单
                        var windowFlootStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("地贴") || s.PositionDescription.Contains("地铺") || s.Sheet.Contains("地贴") || s.Sheet.Contains("地铺"))).ToList();
                        if (windowFlootStickOrderList.Any())
                        {
                            selectedIdList.AddRange(windowFlootStickOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowFlootStickOrderList, ref windowFlootStickList);
                        }
                        //橱窗窗贴订单
                        var windowStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("窗贴") || s.Sheet.Contains("窗贴"))).ToList();
                        if (windowStickOrderList.Any())
                        {
                            selectedIdList.AddRange(windowStickOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowStickOrderList, ref windowStickList);
                        }
                        if (windowList.Any())
                        {
                           
                            windowList.ForEach(s =>
                            {
                                sheet.Cells[windowStartRow, 2].Value = "橱窗背景";
                                sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                windowStartRow++;
                            });

                        }
                        if (windowSizeStickList.Any())
                        {
                            windowSizeStickList.ForEach(s =>
                            {
                                sheet.Cells[windowStartRow, 2].Value = "橱窗侧贴";
                                sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                windowStartRow++;
                            });

                        }
                        if (windowFlootStickList.Any())
                        {
                            windowFlootStickList.ForEach(s =>
                            {
                                sheet.Cells[windowStartRow, 2].Value = "橱窗地贴";
                                sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                windowStartRow++;
                            });

                        }
                        if (windowStickList.Any())
                        {
                            windowStickList.ForEach(s =>
                            {
                                sheet.Cells[windowStartRow, 2].Value = "窗贴";
                                sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                windowStartRow++;
                            });

                        }
                        #endregion
                        #region 陈列桌区域
                        //List<QuoteModel> tableList = new List<QuoteModel>();//陈列桌
                        //List<QuoteModel> tableTKList = new List<QuoteModel>();//陈列桌台卡
                        //List<QuoteModel> tableJTZList = new List<QuoteModel>();//陈列桌静态展
                        //List<QuoteModel> tableModuleList = new List<QuoteModel>();//陈列桌模特阵
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
                                       StatisticMaterial(tableQuoteOrderList, ref tableList);
                                       if (tableList.Any())
                                       {

                                           tableList.ForEach(s =>
                                           {
                                               sheet.Cells[tableStartRow, 2].Value = ft.MachineFrameTypeName;
                                               sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                                               sheet.Cells[tableStartRow, 5].Value = s.Amount;
                                               tableStartRow++;
                                           });
                                       }
                                   });
                               }
                           }
                            //除了台卡以外的
                           var otherOrderList = tableOrderList.Where(s => !tkSelectedIdList.Contains(s.Id)).ToList();
                           if (otherOrderList.Any())
                           {
                               selectedIdList.AddRange(otherOrderList.Select(s => s.Id).ToList());
                               List<QuoteModel> oList = new List<QuoteModel>();//
                               StatisticMaterial(otherOrderList, ref oList);
                               if (oList.Any())
                               {
                                   oList.ForEach(s =>
                                   {
                                       sheet.Cells[tableStartRow, 2].Value = "陈列桌桌铺";
                                       sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                                       sheet.Cells[tableStartRow, 5].Value = s.Amount;
                                       tableStartRow++;
                                   });
                               }
                           }
                        }
                        //if (tableList.Any())
                        //{
                        //    tableList.ForEach(s =>
                        //    {
                        //        sheet.Cells[tableStartRow, 2].Value = "陈列桌";
                        //        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                        //        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                        //        tableStartRow++;
                        //    });
                        //}
                        //if (tableTKList.Any())
                        //{
                        //    tableTKList.ForEach(s =>
                        //    {
                        //        sheet.Cells[tableStartRow, 2].Value = "陈列桌台卡";
                        //        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                        //        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                        //        tableStartRow++;
                        //    });
                        //}
                        //if (tableJTZList.Any())
                        //{
                        //    tableJTZList.ForEach(s =>
                        //    {
                        //        sheet.Cells[tableStartRow, 2].Value = "陈列桌静态展";
                        //        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                        //        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                        //        tableStartRow++;
                        //    });
                        //}
                        //if (tableModuleList.Any())
                        //{
                        //    tableModuleList.ForEach(s =>
                        //    {
                        //        sheet.Cells[tableStartRow, 2].Value = "陈列桌模特阵";
                        //        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                        //        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                        //        tableStartRow++;
                        //    });
                        //}
                        #endregion
                        #region 服装墙区域
                        var appOrderList = popOrderList.Where(s => s.Sheet.Contains("服装墙")).ToList();
                        if (appOrderList.Any())
                        {
                            List<int> appTKSelectedIdList = new List<int>();
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
                                                        appTKSelectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                        selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                        appQuoteOrderList.AddRange(order1);
                                                    }
                                                }
                                            });
                                        }
                                        List<QuoteModel> appList = new List<QuoteModel>();//
                                        StatisticMaterial(appQuoteOrderList, ref appList);
                                        if (appList.Any())
                                        {
                                            appList.ForEach(s =>
                                            {
                                                sheet.Cells[appWallStartRow, 2].Value = ft.MachineFrameTypeName;
                                                sheet.Cells[appWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                sheet.Cells[appWallStartRow, 5].Value = s.Amount;
                                                appWallStartRow++;
                                            });
                                        }
                                    });
                                }
                            }
                            //除了台卡以外的
                            var otherOrderList = appOrderList.Where(s => !appTKSelectedIdList.Contains(s.Id)).ToList();
                            if (otherOrderList.Any())
                            {
                                selectedIdList.AddRange(otherOrderList.Select(s => s.Id).ToList());
                                List<QuoteModel> oList = new List<QuoteModel>();//
                                StatisticMaterial(otherOrderList, ref oList);
                                if (oList.Any())
                                {
                                    oList.ForEach(s =>
                                    {
                                        sheet.Cells[appWallStartRow, 2].Value = "服装墙";
                                        sheet.Cells[appWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                        sheet.Cells[appWallStartRow, 5].Value = s.Amount;
                                        appWallStartRow++;
                                    });
                                }
                            }
                        }
                        #endregion
                        #region 鞋墙区域
                        
                        #endregion

                        #region SMU区域订单
                        var smuOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("smu")).ToList();
                        if (smuOrderList.Any())
                        {
                            selectedIdList.AddRange(smuOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> smuList = new List<QuoteModel>();//
                            StatisticMaterial(smuOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = "SMU";
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region 中岛
                        var zdOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("中岛")).ToList();
                        if (zdOrderList.Any())
                        {
                            selectedIdList.AddRange(zdOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> smuList = new List<QuoteModel>();//
                            StatisticMaterial(zdOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = "中岛";
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region 收银台区域订单
                        var cashierOrderList = popOrderList.Where(s => s.Sheet == "收银台").ToList();
                        if (cashierOrderList.Any())
                        {
                            selectedIdList.AddRange(cashierOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> cashierList = new List<QuoteModel>();//
                            StatisticMaterial(cashierOrderList, ref cashierList);
                            if (cashierList.Any())
                            {
                                cashierList.ForEach(s =>
                                {
                                    sheet.Cells[cashierDeskStartRow, 2].Value = "收银台";
                                    sheet.Cells[cashierDeskStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[cashierDeskStartRow, 5].Value = s.Amount;
                                    cashierDeskStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region OOH区域订单
                        var oohOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("ooh")).ToList();
                        if (oohOrderList.Any())
                        {
                            selectedIdList.AddRange(oohOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> oohList = new List<QuoteModel>();//
                            StatisticMaterial(oohOrderList, ref oohList);
                            if (oohList.Any())
                            {
                                oohList.ForEach(s =>
                                {
                                    sheet.Cells[oohStartRow, 2].Value = "OOH";
                                    sheet.Cells[oohStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[oohStartRow, 5].Value = s.Amount;
                                    oohStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region 除了以上常规位置外的其他位置
                        var otherSheetOrderList = popOrderList.Where(s => !selectedIdList.Contains(s.Id)).ToList();
                        if (otherSheetOrderList.Any())
                        {
                            List<string> sheets = otherSheetOrderList.Select(s => s.Sheet).Distinct().ToList();
                            string sheetStr = StringHelper.ListToString(sheets, "/");
                            List<QuoteModel> smuList = new List<QuoteModel>();//
                            StatisticMaterial(otherSheetOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = sheetStr;
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else if (templateType == (int)QuoteOrderTemplateEnum.TongDian)
                    {
                        #region 童店格式
                        List<int> selectedIdList = new List<int>();
                        #region 橱窗区域
                        List<QuoteModel> windowList = new List<QuoteModel>();//橱窗背景
                        //List<QuoteModel> windowSizeStickList = new List<QuoteModel>();//橱窗侧贴
                        //List<QuoteModel> windowFlootStickList = new List<QuoteModel>();//橱窗地贴
                        //List<QuoteModel> windowStickList = new List<QuoteModel>();//窗贴

                        //橱窗背景订单
                        var windowOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗")).ToList();
                        if (windowOrderList.Any())
                        {
                            selectedIdList.AddRange(windowOrderList.Select(s => s.Id).ToList());
                            StatisticMaterial(windowOrderList, ref windowList);
                            if (windowList.Any())
                            {
                                windowList.ForEach(s =>
                                {
                                    sheet.Cells[windowStartRow, 2].Value = "橱窗背景";
                                    sheet.Cells[windowStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[windowStartRow, 5].Value = s.Amount;
                                    windowStartRow++;
                                });

                            }
                        }

                        ////橱窗侧贴订单
                        //var windowSizeStickOrderList = popOrderList.Where(s => (s.Sheet.Contains("橱窗") && s.PositionDescription.Contains("侧贴")) || (s.Sheet.Contains("橱窗") && s.Sheet.Contains("侧贴"))).ToList();
                        //if (windowSizeStickOrderList.Any())
                        //{
                        //    selectedIdList.AddRange(windowSizeStickOrderList.Select(s => s.Id).ToList());
                        //    StatisticMaterial(windowSizeStickOrderList, ref windowSizeStickList);
                        //}
                        ////橱窗地贴订单
                        //var windowFlootStickOrderList = popOrderList.Where(s => s.Sheet.Contains("橱窗") && (s.PositionDescription.Contains("地贴") || s.PositionDescription.Contains("地铺") || s.Sheet.Contains("地贴") || s.Sheet.Contains("地铺"))).ToList();
                        //if (windowFlootStickOrderList.Any())
                        //{
                        //    selectedIdList.AddRange(windowFlootStickOrderList.Select(s => s.Id).ToList());
                        //    StatisticMaterial(windowFlootStickOrderList, ref windowFlootStickList);
                        //}
                        ////橱窗窗贴订单
                        //var windowStickOrderList = popOrderList.Where(s => (s.Sheet.Contains("橱窗") && s.PositionDescription == "窗贴") || (s.Sheet.Contains("窗贴"))).ToList();
                        //if (windowStickOrderList.Any())
                        //{
                        //    selectedIdList.AddRange(windowStickOrderList.Select(s => s.Id).ToList());
                        //    StatisticMaterial(windowStickOrderList, ref windowStickList);
                        //}

                        #endregion
                        #region 陈列桌区域
                        var tableOrderList = popOrderList.Where(s => s.Sheet.Contains("陈列桌") || s.Sheet.Contains("展桌")).ToList();
                        if (tableOrderList.Any())
                        {
                            List<int> tkSelectedIdList = new List<int>();
                            List<string> channelList = tableOrderList.Select(s => s.Channel != null ? s.Channel.ToUpper() : "").ToList();
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
                                                 if (!string.IsNullOrWhiteSpace(size.Channel))
                                                 {
                                                     List<string> sizeChannelList = StringHelper.ToStringList(size.Channel, ',', LowerUpperEnum.ToUpper);
                                                     bool flag = sizeChannelList.Any(b => channelList.Any(a => a.Equals(b)));
                                                     if (flag)
                                                     {
                                                         List<QuoteOrderDetail> order1 = tableOrderList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                                         if (order1.Any())
                                                         {
                                                             tkSelectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                             selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                             tableQuoteOrderList.AddRange(order1);
                                                         }
                                                     }
                                                 }
                                             });
                                         }
                                         List<QuoteModel> tableList = new List<QuoteModel>();//
                                         StatisticMaterial(tableQuoteOrderList, ref tableList);
                                         if (tableList.Any())
                                         {
                                            
                                             tableList.ForEach(s =>
                                             {
                                                 sheet.Cells[tableStartRow, 2].Value = ft.MachineFrameTypeName;
                                                 sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                 sheet.Cells[tableStartRow, 5].Value = s.Amount;
                                                 tableStartRow++;
                                             });
                                         }
                                     });
                                }
                            }
                            //除了台卡以外的
                            var otherOrderList = tableOrderList.Where(s => !tkSelectedIdList.Contains(s.Id)).ToList();
                            if (otherOrderList.Any())
                            {
                                selectedIdList.AddRange(otherOrderList.Select(s => s.Id).ToList());
                                List<QuoteModel> oList = new List<QuoteModel>();//
                                StatisticMaterial(otherOrderList, ref oList);
                                if (oList.Any())
                                {
                                    oList.ForEach(s =>
                                    {
                                        sheet.Cells[tableStartRow, 2].Value = "陈列桌桌铺";
                                        sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                                        sheet.Cells[tableStartRow, 5].Value = s.Amount;
                                        tableStartRow++;
                                    });
                                }
                            }
                        }
                        #endregion
                        #region 服装墙区域
                        var appOrderList = popOrderList.Where(s => s.Sheet.Contains("服装墙")).ToList();
                        if (appOrderList.Any())
                        {
                            List<int> appSelectedIdList = new List<int>();
                            List<string> channelList = appOrderList.Select(s => s.Channel != null ? s.Channel.ToUpper() : "").ToList();
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
                                                if (!string.IsNullOrWhiteSpace(size.Channel))
                                                {
                                                    List<string> sizeChannelList = StringHelper.ToStringList(size.Channel, ',', LowerUpperEnum.ToUpper);
                                                    bool flag = sizeChannelList.Any(b => channelList.Any(a => a.Equals(b)));
                                                    if (flag)
                                                    {
                                                        List<QuoteOrderDetail> order1 = appOrderList.Where(s => s.GraphicLength == size.Height && s.GraphicWidth == size.Width).ToList();
                                                        if (order1.Any())
                                                        {
                                                            appSelectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                            selectedIdList.AddRange(order1.Select(s => s.Id).ToList());
                                                            appQuoteOrderList.AddRange(order1);
                                                        }
                                                    }
                                                }
                                            });
                                        }
                                        List<QuoteModel> appList = new List<QuoteModel>();//
                                        StatisticMaterial(appQuoteOrderList, ref appList);
                                        if (appList.Any())
                                        {
                                            appList.ForEach(s =>
                                            {
                                                sheet.Cells[appWallStartRow, 2].Value = ft.MachineFrameTypeName;
                                                sheet.Cells[appWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                                sheet.Cells[appWallStartRow, 5].Value = s.Amount;
                                                appWallStartRow++;
                                            });
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
                                StatisticMaterial(otherOrderList, ref oList);
                                if (oList.Any())
                                {
                                    oList.ForEach(s =>
                                    {
                                        sheet.Cells[appWallStartRow, 2].Value = "服装墙";
                                        sheet.Cells[appWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                        sheet.Cells[appWallStartRow, 5].Value = s.Amount;
                                        appWallStartRow++;
                                    });
                                }
                            }
                        }
                        #endregion
                        #region 鞋墙区域
                        var ftwOrderList = popOrderList.Where(s => s.Sheet.Contains("鞋墙")).ToList();
                        if (ftwOrderList.Any())
                        {
                            selectedIdList.AddRange(ftwOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> ftwList = new List<QuoteModel>();//
                            StatisticMaterial(ftwOrderList, ref ftwList);
                            if (ftwList.Any())
                            {
                                ftwList.ForEach(s =>
                                {
                                    sheet.Cells[ftwWallStartRow, 2].Value = "鞋墙";
                                    sheet.Cells[ftwWallStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[ftwWallStartRow, 5].Value = s.Amount;
                                    ftwWallStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region SMU区域订单
                        var smuOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("smu")).ToList();
                        if (smuOrderList.Any())
                        {
                            selectedIdList.AddRange(smuOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> smuList = new List<QuoteModel>();//
                            StatisticMaterial(smuOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = "SMU";
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region 中岛
                        var zdOrderList = popOrderList.Where(s => s.Sheet.Contains("中岛")).ToList();
                        if (zdOrderList.Any())
                        {
                            selectedIdList.AddRange(zdOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> smuList = new List<QuoteModel>();//
                            StatisticMaterial(zdOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = "中岛";
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region 收银台区域订单
                        var cashierOrderList = popOrderList.Where(s => s.Sheet.Contains("收银台")).ToList();
                        if (cashierOrderList.Any())
                        {
                            selectedIdList.AddRange(cashierOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> cashierList = new List<QuoteModel>();//
                            StatisticMaterial(cashierOrderList, ref cashierList);
                            if (cashierList.Any())
                            {
                                cashierList.ForEach(s =>
                                {
                                    sheet.Cells[cashierDeskStartRow, 2].Value = "收银台";
                                    sheet.Cells[cashierDeskStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[cashierDeskStartRow, 5].Value = s.Amount;
                                    cashierDeskStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region OOH区域订单
                        var oohOrderList = popOrderList.Where(s => s.Sheet.ToLower().Contains("ooh")|| s.Sheet.Contains("户外")).ToList();
                        if (oohOrderList.Any())
                        {
                            selectedIdList.AddRange(oohOrderList.Select(s => s.Id).ToList());
                            List<QuoteModel> oohList = new List<QuoteModel>();//
                            StatisticMaterial(oohOrderList, ref oohList);
                            if (oohList.Any())
                            {
                                oohList.ForEach(s =>
                                {
                                    sheet.Cells[oohStartRow, 2].Value = "OOH";
                                    sheet.Cells[oohStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[oohStartRow, 5].Value = s.Amount;
                                    oohStartRow++;
                                });
                            }
                        }
                        #endregion
                        #region 除了以上常规位置外的其他位置
                        var otherSheetOrderList = popOrderList.Where(s => !selectedIdList.Contains(s.Id)).ToList();
                        if (otherSheetOrderList.Any())
                        {
                            List<string> sheets = otherSheetOrderList.Select(s => s.Sheet).Distinct().ToList();
                            string sheetStr = StringHelper.ListToString(sheets, "/");
                            List<QuoteModel> smuList = new List<QuoteModel>();//
                            StatisticMaterial(otherSheetOrderList, ref smuList);
                            if (smuList.Any())
                            {
                                smuList.ForEach(s =>
                                {
                                    sheet.Cells[smuStartRow, 2].Value = sheetStr;
                                    sheet.Cells[smuStartRow, 3].Value = s.QuoteGraphicMaterial;
                                    sheet.Cells[smuStartRow, 5].Value = s.Amount;
                                    smuStartRow++;
                                });
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else if (templateType == (int)QuoteOrderTemplateEnum.Terrex)
                    {
                        #region 户外店格式

                        #endregion
                    }
                    //安装费
                    List<string> cityTierList = new List<string>() { "T1", "T2", "T3" };
                    InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                    int oohInstallLevelOne = 0;//5000级别
                    int oohInstallLevelTwo = 0;//2700级别
                    int oohInstallLevelThree = 0;//1800级别
                    int oohInstallLevelFour = 0;//600级别

                    decimal otherOOHInstallPrice = 0;

                    int windowInstallLevelOne = 0;
                    int windowInstallLevelTwo = 0;
                    int windowInstallLevelThree = 0;
                    int windowInstallKidsLevel = 0;

                    //decimal otherWindowInstallPrice = 0;

                    int basicInstallLevelOne = 0;
                    int basicInstallLevelTwo = 0;
                    int basicInstallLevelThree = 0;

                    decimal otherBasicInstallPrice = 0;

                    List<ExpressPriceDetail> expressPriceList = new List<ExpressPriceDetail>();
                    //快递费订单
                    var expressPriceOrderList = orderList.Where(s=>s.OrderType==(int)OrderTypeEnum.发货费).ToList();
                    if (expressPriceOrderList.Any())
                    {
                        expressPriceOrderList.ForEach(s => {
                            ExpressPriceDetail model = new ExpressPriceDetail();
                            model.ShopId = s.ShopId;
                            model.ExpressPrice = s.PayOrderPrice;
                            expressPriceList.Add(model);
                        });
                    }
                    //安装费订单
                    var installPriceOrderList = orderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).ToList();
                    if (installPriceOrderList.Any())
                    {
                        installPriceOrderList.ForEach(s => {
                            if ((s.OrderPrice ?? 0) > 0)
                            {
                                //店内
                                if (s.OrderPrice == 800)
                                {
                                    basicInstallLevelOne++;
                                }
                                else if (s.OrderPrice == 400)
                                {
                                    basicInstallLevelTwo++;
                                }
                                else if (s.OrderPrice == 150)
                                {
                                    basicInstallLevelThree++;
                                }
                                else
                                    otherBasicInstallPrice += (s.OrderPrice ?? 0);
                            }
                        });
                        
                    }

                    guidanceIdList.ForEach(gid =>
                    {
                        //安装费
                        List<int> installShopIdList = orderList.Where(s => s.GuidanceId == gid && cityTierList.Contains(s.CityTier)).Select(s => s.ShopId ?? 0).Distinct().ToList();
                        var installPriceList = installShopBll.GetList(s => s.GuidanceId == gid && installShopIdList.Contains(s.ShopId ?? 0)).ToList();
                        if (installPriceList.Any())
                        {
                            installPriceList.ForEach(s =>
                            {
                                //OOH
                                if ((s.OOHPrice ?? 0) > 0)
                                {
                                    if (s.OOHPrice == 5000)
                                    {
                                        oohInstallLevelOne++;
                                    }
                                    else if (s.OOHPrice == 2700)
                                    {
                                        oohInstallLevelTwo++;
                                    }
                                    else if (s.OOHPrice == 1800)
                                    {
                                        oohInstallLevelThree++;
                                    }
                                    else if (s.OOHPrice == 600)
                                    {
                                        oohInstallLevelFour++;
                                    }
                                    else
                                        otherOOHInstallPrice += (s.OOHPrice ?? 0);
                                }
                                if ((s.WindowPrice ?? 0) > 0)
                                {
                                    //橱窗
                                    if (s.WindowPrice == 1000)
                                    {
                                        windowInstallLevelOne++;
                                    }
                                    else if (s.WindowPrice == 500)
                                    {
                                        windowInstallLevelTwo++;
                                    }
                                    else if (s.WindowPrice == 200)
                                    {
                                        windowInstallLevelThree++;
                                    }
                                    else if (s.WindowPrice == 350)
                                    {
                                        windowInstallKidsLevel++;
                                    }

                                }
                                if ((s.BasicPrice ?? 0) > 0)
                                {
                                    //店内
                                    if (s.BasicPrice == 800)
                                    {
                                        basicInstallLevelOne++;
                                    }
                                    else if (s.BasicPrice == 400)
                                    {
                                        basicInstallLevelTwo++;
                                    }
                                    else if (s.BasicPrice == 150)
                                    {
                                        basicInstallLevelThree++;
                                    }
                                    else
                                        otherBasicInstallPrice += (s.BasicPrice ?? 0);
                                }
                            });
                        }
                        SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(gid);
                        if (guidanceModel != null && (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion || guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery))
                        {
                            //活动快递费
                            List<int> expressPriceShopIdList = orderList.Where(s => s.GuidanceId == gid).Select(s => s.ShopId ?? 0).Distinct().ToList();
                            var expressList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == gid && expressPriceShopIdList.Contains(s.ShopId??0));
                            if (expressList.Any())
                            {
                                expressPriceList.AddRange(expressList);
                            }
                        }
                        


                    });
                    
                    if (oohInstallLevelOne > 0)
                        sheet.Cells[96, 7].Value = oohInstallLevelOne.ToString();
                    if (oohInstallLevelTwo > 0)
                        sheet.Cells[97, 7].Value = oohInstallLevelTwo.ToString();
                    if (oohInstallLevelThree > 0)
                        sheet.Cells[98, 7].Value = oohInstallLevelThree.ToString();
                    if (oohInstallLevelFour > 0)
                        sheet.Cells[99, 7].Value = oohInstallLevelFour.ToString();


                    if (windowInstallLevelOne > 0)
                        sheet.Cells[100, 7].Value = windowInstallLevelOne.ToString();
                    if (windowInstallLevelTwo > 0)
                        sheet.Cells[102, 7].Value = windowInstallLevelTwo.ToString();
                    if (windowInstallLevelThree > 0)
                        sheet.Cells[104, 7].Value = windowInstallLevelThree.ToString();

                    if (basicInstallLevelOne > 0)
                        sheet.Cells[101, 7].Value = basicInstallLevelOne.ToString();
                    if (basicInstallLevelTwo > 0)
                        sheet.Cells[103, 7].Value = basicInstallLevelTwo.ToString();
                    if (basicInstallLevelThree > 0)
                        sheet.Cells[105, 7].Value = basicInstallLevelThree.ToString();

                    if (otherBasicInstallPrice > 0)
                    {
                        //如果不在正常的级别（T4-T7）
                        { 
                           
                        }
                    }


                    //第二张表
                    ExcelWorksheet sheet2 = package.Workbook.Worksheets[2];
                    int secondSheetRowIndex = 13;
                    //快递费
                    if (expressPriceList.Any())
                    {
                        
                        var groupList = (from order in expressPriceList
                                         group order by order.ExpressPrice
                                             into g
                                             select new
                                             {
                                                 ExpressPrice = g.Key,
                                                 ShopCount = g.Select(s => s.ShopId??0).Count()
                                             }).ToList();
                        
                        groupList.ForEach(s => {
                            sheet2.Cells[secondSheetRowIndex, 1].Value = "快递费";
                            sheet2.Cells[secondSheetRowIndex, 10].Value = s.ShopCount;
                            sheet2.Cells[secondSheetRowIndex, 14].Value = s.ExpressPrice;
                            secondSheetRowIndex++;
                        });

                    }
                    //物料
                    var materialList = orderList.Where(s=>s.OrderType==(int)OrderTypeEnum.物料).ToList();
                    if (materialList.Any())
                    {
                        var mgroupList = (from order in materialList
                                         group order by new
                                         {
                                             order.Sheet,
                                             order.UnitPrice
                                         } into g
                                         select new { 
                                             MaterialName=g.Key.Sheet,
                                             UnitPrice=g.Key.UnitPrice,
                                             Count=g.Sum(s=>s.Quantity??0)
                                         }).ToList();
                        mgroupList.ForEach(s => {
                            sheet2.Cells[secondSheetRowIndex, 1].Value = s.MaterialName;
                            sheet2.Cells[secondSheetRowIndex, 10].Value = s.Count;
                            sheet2.Cells[secondSheetRowIndex, 12].Value = s.UnitPrice;
                            secondSheetRowIndex++;
                        });
                    }
                    //第三张表
                    ExcelWorksheet sheet3 = package.Workbook.Worksheets[3];
                    int thirdSheetRowIndex = 12;


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

        /// <summary>
        /// 统计材质
        /// </summary>
        /// <param name="orderList"></param>
        /// <param name="quoteModelList"></param>
        void StatisticMaterial(List<QuoteOrderDetail> orderList, ref List<QuoteModel> quoteModelList)
        {
            if (orderList.Any())
            {
                List<MaterialClass> materialList = new List<MaterialClass>();
                orderList.ForEach(s =>
                {
                    int Quantity = s.Quantity ?? 1;
                    decimal width = s.TotalGraphicWidth ?? 0;
                    decimal length = s.TotalGraphicLength ?? 0;
                    if (!string.IsNullOrWhiteSpace(s.QuoteGraphicMaterial))
                    {
                        MaterialClass mc = new MaterialClass();
                        if (s.UnitName == "平米")
                        {
                            mc.Area = (width * length) / 1000000 * Quantity;
                        }
                        else if (s.UnitName == "米")
                        {
                            mc.Area = (width / 1000) * 2 * Quantity;
                        }
                        else
                        {
                            mc.Area = Quantity;
                        }
                        mc.MaterialName = s.QuoteGraphicMaterial;
                        materialList.Add(mc);
                    }
                });
                var list0 = (from material in materialList
                             group material by
                             material.MaterialName
                                 into g
                                 select new
                                 {
                                     GraphicMaterial = g.Key,
                                     Area = g.Sum(s => s.Area > 0 ? s.Area : 0),

                                 }).ToList();
                if (list0.Any())
                {
                    List<QuoteModel> quoteModelList1 = new List<QuoteModel>();
                    list0.ForEach(s =>
                    {
                        QuoteModel model = new QuoteModel();
                        model.Amount = s.Area;
                        model.QuoteGraphicMaterial = s.GraphicMaterial;
                        quoteModelList1.Add(model);
                    });
                    quoteModelList = quoteModelList1;
                }
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
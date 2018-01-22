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
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";

            if (context.Request.QueryString["guidanceIds"] != null)
                guidanceIds = context.Request.QueryString["guidanceIds"];
            if (context.Request.QueryString["subjectIds"] != null)
                subjectIds = context.Request.QueryString["subjectIds"];

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
                var orderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (subjectIdList.Any() ? subjectIdList.Contains(s.SubjectId ?? 0) : 1 == 1) && (s.IsDelete == false || s.IsDelete == null) && s.Sheet != null && (s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength != null && s.GraphicLength > 1 && s.GraphicWidth != null && s.GraphicWidth > 1));
                if (orderList.Any())
                {
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


                    List<QuoteModel> windowList = new List<QuoteModel>();//橱窗背景
                    List<QuoteModel> windowSizeStickList = new List<QuoteModel>();//橱窗侧贴
                    List<QuoteModel> windowFlootStickList = new List<QuoteModel>();//橱窗地贴
                    List<QuoteModel> windowStickList = new List<QuoteModel>();//窗贴

                    List<QuoteModel> tableList = new List<QuoteModel>();//HC陈列桌
                    List<QuoteModel> tableTKList = new List<QuoteModel>();//陈列桌台卡
                    List<QuoteModel> tableJTZList = new List<QuoteModel>();//陈列桌静态展
                    List<QuoteModel> tableModuleList = new List<QuoteModel>();//陈列桌模特阵

                    //橱窗背景订单
                    var windowOrderList = orderList.Where(s => s.Sheet == "橱窗" && !s.PositionDescription.Contains("窗贴") && s.PositionDescription != "左侧贴" && s.PositionDescription != "右侧贴" && s.PositionDescription != "地贴").ToList();
                    StatisticMaterial(windowOrderList, ref windowList);
                    //橱窗侧贴订单
                    var windowSizeStickOrderList = orderList.Where(s => s.Sheet == "橱窗" && s.PositionDescription.Contains("侧贴")).ToList();
                    StatisticMaterial(windowSizeStickOrderList, ref windowSizeStickList);
                    //橱窗地贴订单
                    var windowFlootStickOrderList = orderList.Where(s => s.Sheet == "橱窗" && s.PositionDescription == "地贴").ToList();
                    StatisticMaterial(windowFlootStickOrderList, ref windowFlootStickList);
                    //橱窗地贴订单
                    var windowStickOrderList = orderList.Where(s => (s.Sheet.Contains("橱窗") && s.PositionDescription == "窗贴") || (s.Sheet.Contains("窗贴"))).ToList();
                    StatisticMaterial(windowStickOrderList, ref windowStickList);

                    //陈列桌订单
                    var tableOrderList = orderList.Where(s => s.Sheet == "陈列桌" && !s.PositionDescription.Contains("台卡") && !s.PositionDescription.Contains("静态展") && !s.PositionDescription.Contains("模特阵")).ToList();
                    StatisticMaterial(tableOrderList, ref tableList);
                    //陈列桌台卡订单
                    var tableTKOrderList = orderList.Where(s => s.Sheet == "陈列桌" && s.PositionDescription.Contains("台卡")).ToList();
                    StatisticMaterial(tableTKOrderList, ref tableTKList);
                    //陈列桌静态展订单
                    var tableJTZOrderList = orderList.Where(s => s.Sheet == "陈列桌" && s.PositionDescription.Contains("静态展")).ToList();
                    StatisticMaterial(tableJTZOrderList, ref tableJTZList);
                    //陈列桌模特阵订单
                    var tableModuleOrderList = orderList.Where(s => s.Sheet == "陈列桌" && s.PositionDescription.Contains("模特阵")).ToList();
                    StatisticMaterial(tableModuleOrderList, ref tableModuleList);





                    int windowStartRow = 13;
                    int tableStartRow = windowStartRow + 10;
                    int appWallStartRow = tableStartRow + 10;
                    int ftwWallStartRow = appWallStartRow + 10;
                    int smuStartRow = ftwWallStartRow + 10;
                    int cashierDeskStartRow = smuStartRow + 10;
                    int oohStartRow = cashierDeskStartRow + 10;
                    //橱窗区域
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
                    //陈列桌区域
                    if (tableList.Any())
                    {
                        tableList.ForEach(s =>
                        {
                            sheet.Cells[tableStartRow, 2].Value = "陈列桌";
                            sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                            sheet.Cells[tableStartRow, 5].Value = s.Amount;
                            tableStartRow++;
                        });
                    }
                    if (tableTKList.Any())
                    {
                        tableTKList.ForEach(s =>
                        {
                            sheet.Cells[tableStartRow, 2].Value = "陈列桌台卡";
                            sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                            sheet.Cells[tableStartRow, 5].Value = s.Amount;
                            tableStartRow++;
                        });
                    }
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
                    if (tableModuleList.Any())
                    {
                        tableModuleList.ForEach(s =>
                        {
                            sheet.Cells[tableStartRow, 2].Value = "陈列桌模特阵";
                            sheet.Cells[tableStartRow, 3].Value = s.QuoteGraphicMaterial;
                            sheet.Cells[tableStartRow, 5].Value = s.Amount;
                            tableStartRow++;
                        });
                    }

                    //服装墙区域订单
                    var appOrderList = orderList.Where(s => s.Sheet.Contains("服装墙") && !s.PositionDescription.Contains("台卡")).ToList();
                    if (appOrderList.Any())
                    {
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
                    var appTKOrderList = orderList.Where(s => s.Sheet.Contains("服装墙") && s.PositionDescription.Contains("台卡")).ToList();
                    if (appTKOrderList.Any())
                    {
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


                    //鞋墙区域订单
                    var ftwOrderList = orderList.Where(s => s.Sheet.Contains("鞋墙")).ToList();
                    if (ftwOrderList.Any())
                    {

                    }

                    //SMU区域订单
                    var smuOrderList = orderList.Where(s => s.Sheet.ToLower().Contains("smu")).ToList();
                    if (smuOrderList.Any())
                    {
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

                    //收银台区域订单
                    var cashierOrderList = orderList.Where(s => s.Sheet == "收银台").ToList();
                    if (cashierOrderList.Any())
                    {
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

                    //OOH区域订单
                    var oohOrderList = orderList.Where(s => s.Sheet.ToLower().Contains("ooh")).ToList();
                    if (oohOrderList.Any())
                    {
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
                    //安装费
                    List<string> cityTierList = new List<string>() { "T1", "T2", "T3" };
                    InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                    int oohInstallLevelOne = 0;//5000级别
                    int oohInstallLevelTwo = 0;//2700级别
                    int oohInstallLevelThree = 0;//1800级别
                    int oohInstallLevelFour = 0;//600级别

                    int windowInstallLevelOne = 0;
                    int windowInstallLevelTwo = 0;
                    int windowInstallLevelThree = 0;

                    int basicInstallLevelOne = 0;
                    int basicInstallLevelTwo = 0;
                    int basicInstallLevelThree = 0;

                    guidanceIdList.ForEach(gid =>
                    {
                        List<int> shopIdList = orderList.Where(s => s.GuidanceId == gid && cityTierList.Contains(s.CityTier)).Select(s => s.ShopId ?? 0).Distinct().ToList();
                        var installPriceList = installShopBll.GetList(s => s.GuidanceId == gid && shopIdList.Contains(s.ShopId ?? 0)).ToList();
                        if (installPriceList.Any())
                        {
                            installPriceList.ForEach(s =>
                            {
                                //OOH
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
                            });
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
                    decimal width = s.GraphicWidth ?? 0;
                    decimal length = s.GraphicLength ?? 0;
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
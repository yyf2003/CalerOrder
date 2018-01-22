using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Common;
using NPOI.SS.UserModel;
using System.Configuration;
using System.IO;

namespace WebApp.Subjects.ADOrders
{
    public partial class ExportEmptyFrameShop : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int subjectId = 0;
            string sheetName = string.Empty;
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["sheet"] != null)
            {
                sheetName = Request.QueryString["sheet"];
            }
            List<int> shopIdList = new MergeOriginalOrderBLL().GetList(s => s.SubjectId == subjectId && s.Sheet.ToLower() == sheetName.ToLower()).Select(s => s.ShopId ?? 0).Distinct().ToList();
            if (shopIdList.Any())
            {
                List<int> frameShopList = new ShopMachineFrameBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && s.PositionName.ToLower() == sheetName.ToLower()).Select(s => s.ShopId ?? 0).Distinct().ToList();
                var emptyShopList = shopIdList.Except(frameShopList);
                var shopList = new ShopBLL().GetList(s => emptyShopList.Contains(s.Id));
                if (shopList.Any())
                {
                    shopList = shopList.OrderBy(s => s.Id).ToList();
                    string templateFileName = "EmptyFrameShopTemplate";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheet("Sheet1");

                    int startRow = 1;
                    string shopno = string.Empty;
                    foreach (var item in shopList)
                    {



                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 23; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(0).SetCellValue(item.ShopNo);
                        dataRow.GetCell(1).SetCellValue(item.ShopName);
                        dataRow.GetCell(2).SetCellValue(item.RegionName);
                        dataRow.GetCell(3).SetCellValue(item.ProvinceName);
                        dataRow.GetCell(4).SetCellValue(item.CityName);
                        dataRow.GetCell(5).SetCellValue(item.CityTier);
                        dataRow.GetCell(6).SetCellValue(item.Channel);
                        dataRow.GetCell(7).SetCellValue(item.Format);
                        dataRow.GetCell(8).SetCellValue(sheetName);
                        
                        startRow++;

                    }



                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();
                        sheet = null;
                        workBook = null;
                        OperateFile.DownLoadFile(ms, "器架为空的店铺信息");

                    }
                }
            }
        }
    }
}
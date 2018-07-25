using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using Models;
using BLL;
using NPOI.SS.UserModel;
using System.Configuration;
using System.IO;
using Common;

namespace WebApp
{
    public partial class web1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            List<FinalOrderDetailTemp> popList = new List<FinalOrderDetailTemp>();
            FinalOrderDetailTemp pop;
            //按器架
            var qijiaList = (from frame in CurrentContext.DbContext.ShopMachineFrame
                             join shop in CurrentContext.DbContext.Shop
                             on frame.ShopId equals shop.Id
                             where shop.Channel != null
                             && shop.Channel.ToLower().Contains("terrex")
                             && frame.PositionName.Contains("鞋墙")
                             && shop.IsInstall == "Y"
                             select new
                             {
                                 shop,
                                 frame
                             }).ToList();
            qijiaList.ForEach(s =>
            {
                if (s.frame.MachineFrame.Contains("4仓") || s.frame.MachineFrame.Contains("3仓") || s.frame.MachineFrame.Contains("2仓") || s.frame.MachineFrame.Contains("1仓"))
                {
                    //横板
                    pop = new FinalOrderDetailTemp();
                    pop.ShopNo = s.shop.ShopNo;
                    pop.ShopName = s.shop.ShopName;
                    pop.Region = s.shop.RegionName;
                    pop.Province = s.shop.ProvinceName;
                    pop.City = s.shop.CityName;
                    pop.CityTier = s.shop.CityTier;
                    pop.IsInstall = s.shop.IsInstall;
                    pop.POPAddress = s.shop.POPAddress;
                    pop.Contact = s.shop.Contact1;
                    pop.Tel = s.shop.Tel1;
                    pop.Channel = s.shop.Channel;
                    pop.Format = s.shop.Format;
                    pop.Sheet = s.frame.PositionName;
                    pop.MachineFrame = s.frame.MachineFrame;
                    pop.GraphicWidth = 800;
                    pop.GraphicLength = 315;
                    pop.GraphicMaterial = "背胶PP+3mm雪弗板";
                    if (s.frame.MachineFrame.Contains("4仓"))
                        pop.Quantity = 4;
                    if (s.frame.MachineFrame.Contains("3仓"))
                        pop.Quantity = 3;
                    if (s.frame.MachineFrame.Contains("2仓"))
                        pop.Quantity = 2;
                    if (s.frame.MachineFrame.Contains("1仓"))
                        pop.Quantity = 1;
                    popList.Add(pop);


                    //竖板
                    pop = new FinalOrderDetailTemp();
                    pop.ShopNo = s.shop.ShopNo;
                    pop.ShopName = s.shop.ShopName;
                    pop.Region = s.shop.RegionName;
                    pop.Province = s.shop.ProvinceName;
                    pop.City = s.shop.CityName;
                    pop.CityTier = s.shop.CityTier;
                    pop.IsInstall = s.shop.IsInstall;
                    pop.POPAddress = s.shop.POPAddress;
                    pop.Contact = s.shop.Contact1;
                    pop.Tel = s.shop.Tel1;
                    pop.Channel = s.shop.Channel;
                    pop.Format = s.shop.Format;
                    pop.Sheet = s.frame.PositionName;
                    pop.MachineFrame = s.frame.MachineFrame;
                    pop.GraphicWidth = 310;
                    pop.GraphicLength = 765;
                    pop.GraphicMaterial = "背胶PP+3mm雪弗板";
                    if (s.frame.MachineFrame.Contains("4仓"))
                        pop.Quantity = 4;
                    if (s.frame.MachineFrame.Contains("3仓"))
                        pop.Quantity = 3;
                    if (s.frame.MachineFrame.Contains("2仓"))
                        pop.Quantity = 2;
                    if (s.frame.MachineFrame.Contains("1仓"))
                        pop.Quantity = 1;
                    popList.Add(pop);


                }

            });

            //每家店都加
            List<Shop> shopList = new ShopBLL().GetList(s => s.Channel != null && s.Channel.ToLower().Contains("terrex") && s.IsInstall == "Y");
            shopList.ForEach(s =>
            {
                pop = new FinalOrderDetailTemp();
                pop.ShopNo = s.ShopNo;
                pop.ShopName = s.ShopName;
                pop.Region = s.RegionName;
                pop.Province = s.ProvinceName;
                pop.City = s.CityName;
                pop.CityTier = s.CityTier;
                pop.IsInstall = s.IsInstall;
                pop.POPAddress = s.POPAddress;
                pop.Contact = s.Contact1;
                pop.Tel = s.Tel1;
                pop.Channel = s.Channel;
                pop.Format = s.Format;
                pop.Sheet = "鞋中岛";
                pop.GraphicWidth = 320;
                pop.GraphicLength = 195;
                pop.Quantity = 2;
                pop.GraphicMaterial = "背胶PP+3mm雪弗板";
                popList.Add(pop);

                pop = new FinalOrderDetailTemp();
                pop.ShopNo = s.ShopNo;
                pop.ShopName = s.ShopName;
                pop.Region = s.RegionName;
                pop.Province = s.ProvinceName;
                pop.City = s.CityName;
                pop.CityTier = s.CityTier;
                pop.IsInstall = s.IsInstall;
                pop.POPAddress = s.POPAddress;
                pop.Contact = s.Contact1;
                pop.Tel = s.Tel1;
                pop.Channel = s.Channel;
                pop.Format = s.Format;
                pop.Sheet = "鞋中岛";
                pop.GraphicWidth = 320;
                pop.GraphicLength = 265;
                pop.Quantity = 2;
                pop.GraphicMaterial = "背胶PP+3mm雪弗板";
                popList.Add(pop);

                pop = new FinalOrderDetailTemp();
                pop.ShopNo = s.ShopNo;
                pop.ShopName = s.ShopName;
                pop.Region = s.RegionName;
                pop.Province = s.ProvinceName;
                pop.City = s.CityName;
                pop.CityTier = s.CityTier;
                pop.IsInstall = s.IsInstall;
                pop.POPAddress = s.POPAddress;
                pop.Contact = s.Contact1;
                pop.Tel = s.Tel1;
                pop.Channel = s.Channel;
                pop.Format = s.Format;
                pop.Sheet = "陈列桌台卡";
                pop.GraphicWidth = 148;
                pop.GraphicLength = 230;
                pop.Quantity = 2;
                pop.GraphicMaterial = "背胶PP+3mm雪弗板";
                popList.Add(pop);

            });

            if (popList.Any())
            {
                string templateFileName = "安装店铺清单统计模板";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                ISheet sheet = workBook.GetSheetAt(0);
                int startRow = 1;
                foreach (var item in popList)
                {
                    IRow dataRow = sheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = sheet.CreateRow(startRow);
                    for (int i = 0; i < 31; i++)
                    {
                        ICell cell = dataRow.GetCell(i);
                        if (cell == null)
                            cell = dataRow.CreateCell(i);

                    }
                    dataRow.GetCell(0).SetCellValue(item.Channel);
                    dataRow.GetCell(1).SetCellValue(item.ShopNo);
                    dataRow.GetCell(2).SetCellValue(item.ShopName);
                    dataRow.GetCell(3).SetCellValue(item.Region);
                    dataRow.GetCell(4).SetCellValue(item.Province);
                    dataRow.GetCell(5).SetCellValue(item.City);
                    dataRow.GetCell(6).SetCellValue(item.CityTier);
                    dataRow.GetCell(7).SetCellValue(item.IsInstall);
                    dataRow.GetCell(8).SetCellValue(item.POPAddress);

                    dataRow.GetCell(9).SetCellValue(item.Contact);
                    dataRow.GetCell(10).SetCellValue(item.Tel);
                    dataRow.GetCell(11).SetCellValue(item.Format);
                    //dataRow.GetCell(12).SetCellValue(item.Channel);
                    dataRow.GetCell(13).SetCellValue(item.Sheet);
                    dataRow.GetCell(14).SetCellValue(item.MachineFrame);
                    dataRow.GetCell(15).SetCellValue(item.Channel);
                    dataRow.GetCell(16).SetCellValue("男女不限");
                    dataRow.GetCell(17).SetCellValue(double.Parse((item.Quantity ?? 1).ToString()));

                    //dataRow.GetCell(18).SetCellValue(item.ShopNo);
                    //dataRow.GetCell(19).SetCellValue(item.ShopName);
                    //dataRow.GetCell(20).SetCellValue(item.Channel);
                    dataRow.GetCell(21).SetCellValue(double.Parse((item.GraphicWidth ?? 0).ToString()));
                    dataRow.GetCell(22).SetCellValue(double.Parse((item.GraphicLength ?? 0).ToString()));
                    //dataRow.GetCell(23).SetCellValue(item.Channel);
                    dataRow.GetCell(24).SetCellValue(item.GraphicMaterial);

                    startRow++;

                }
                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    sheet = null;
                    workBook = null;
                    OperateFile.DownLoadFile(ms, "安装店铺清单统计模板-Terrex");
                    //OperateFile.DownLoadFile(path);

                }
            }

        }

        protected void Button2_Click(object sender, EventArgs e)
        {

        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            string begin = "2018-3-1";
            string end = "2018-8-1";
            DateTime beginDate = DateTime.Parse(begin);
            DateTime endDate = DateTime.Parse(end).AddDays(1);
            List<Subject> subjectList = new SubjectBLL().GetList(s => (s.IsDelete == null || s.IsDelete == false) && s.AddDate >= beginDate && s.AddDate < endDate);
            if (subjectList.Any())
            {
                ApproveInfoBLL approveBll = new ApproveInfoBLL();
                FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                subjectList.ForEach(s =>
                {
                    int subjectId = s.Id;
                    int subjectType = s.SubjectType ?? 1;
                    var approve = approveBll.GetList(a => a.SubjectId == subjectId).OrderBy(a => a.AddDate).FirstOrDefault();
                    if (approve != null)
                    {
                       
                        if (approve.AddDate != null)
                        {
                            string addDate = approve.AddDate.ToString();
                            int type = 1;
                            if (subjectType == (int)SubjectTypeEnum.HC订单 || subjectType == (int)SubjectTypeEnum.分区补单 || subjectType == (int)SubjectTypeEnum.分区增补 || subjectType == (int)SubjectTypeEnum.新开店订单)
                                type = 2;
                            orderBll.UpdateAddDate(subjectId, addDate, type);
                        }
                    }
                });
            }
        }
    }
}
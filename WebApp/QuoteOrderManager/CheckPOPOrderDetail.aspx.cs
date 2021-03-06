﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using NPOI.SS.UserModel;
using System.Data;
using NPOI;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.Configuration;
using System.IO;
using Common;

namespace WebApp.QuoteOrderManager
{
    public partial class CheckPOPOrderDetail : BasePage
    {
        string sheet = string.Empty;
        string guidanceId = string.Empty;
        string subjectCategoryId = string.Empty;
        string subjectId = string.Empty;
        int quoteItemId;
        string sessonKey = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["sheet"] != null)
            {
                sheet = Request.QueryString["sheet"];
            }
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["subjectCategoryId"] != null)
            {
                subjectCategoryId = Request.QueryString["subjectCategoryId"];
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = Request.QueryString["subjectId"];
            }
            if (Request.QueryString["quoteItemId"] != null)
            {
                quoteItemId = int.Parse(Request.QueryString["quoteItemId"]);
            }
            if (!string.IsNullOrWhiteSpace(sheet))
            {
                sessonKey = StringHelper.ReplaceSpace(sheet);
            }
            if (!IsPostBack) {
                if (!string.IsNullOrWhiteSpace(sheet) && sheet.ToLower().Contains("smu"))
                {
                    btnExportNew.Visible = true;
                }
                labSheet.Text = sheet;
                BindData();
            }
        }

        void BindData()
        {
            List<QuoteOrderDetail> popOrderList = new List<QuoteOrderDetail>();
            if (Session[sessonKey] != null)
                popOrderList = Session[sessonKey] as List<QuoteOrderDetail>;
            var list = (from order in popOrderList
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on order.GuidanceId equals guidance.ItemId
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        select new
                        {
                            order,
                            guidance.ItemName,
                            subject.SubjectName,
                            Gender = !string.IsNullOrWhiteSpace(order.OrderGender)?order.OrderGender:order.Gender
                        }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            CheckPOPOrderRepeater.DataSource = list.OrderBy(s => s.order.ShopId).ThenBy(s => s.order.Sheet).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            CheckPOPOrderRepeater.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<QuoteOrderDetail> popOrderList = new List<QuoteOrderDetail>();
            if (Session[sessonKey] != null)
            {
                popOrderList = Session[sessonKey] as List<QuoteOrderDetail>;
                if (popOrderList.Any())
                {
                    popOrderList = popOrderList.OrderBy(s => s.ShopId).ThenBy(s => s.Sheet).ToList();
                }
            }
            if (popOrderList.Any())
            {
                var list = (from order in popOrderList
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on order.GuidanceId equals guidance.ItemId
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            select new
                            {
                                order,
                                guidance.ItemName,
                                subject.SubjectName,
                                subject.AddDate,
                                Gender = (order.OrderGender != null && order.OrderGender!="") ? order.OrderGender : order.Gender
                            }).ToList();
                string templateFileName = "350Template";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                ISheet tableSheet = workBook.GetSheetAt(0);

                int startRow = 1;
                string shopno = string.Empty;
                foreach (var item in list)
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
                    if (item.AddDate != null)
                        dataRow.GetCell(1).SetCellValue(DateTime.Parse(item.AddDate.ToString()).ToShortDateString());
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
                    dataRow.GetCell(14).SetCellValue(item.SubjectName);
                    dataRow.GetCell(15).SetCellValue(item.Gender);
                    dataRow.GetCell(16).SetCellValue(item.order.ChooseImg);


                    //dataRow.GetCell(12).SetCellValue(item.Category);
                    dataRow.GetCell(17).SetCellValue(item.order.Sheet);
                    dataRow.GetCell(18).SetCellValue(item.order.MachineFrame);
                    dataRow.GetCell(19).SetCellValue(item.order.PositionDescription);
                    dataRow.GetCell(20).SetCellValue(item.order.Quantity??0);
                    dataRow.GetCell(21).SetCellValue(item.order.GraphicMaterial);
                    dataRow.GetCell(22).SetCellValue(item.order.QuoteGraphicMaterial);
                    dataRow.GetCell(23).SetCellValue(double.Parse((item.order.UnitPrice ?? 0).ToString()));
                    dataRow.GetCell(24).SetCellValue(double.Parse((item.order.GraphicWidth??0).ToString()));
                    dataRow.GetCell(25).SetCellValue(double.Parse((item.order.GraphicLength??0).ToString()));
                    dataRow.GetCell(26).SetCellValue(double.Parse((item.order.Area??0).ToString()));
                    dataRow.GetCell(27).SetCellValue(double.Parse((item.order.DefaultTotalPrice??0).ToString()));
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
                    OperateFile.DownLoadFile(ms, sessonKey + "原始订单明细");
                }
            }
        }

        protected void btnExportNew_Click(object sender, EventArgs e)
        {
            List<QuoteOrderDetail> popOrderList = new List<QuoteOrderDetail>();
            if (Session[sessonKey] != null)
            {
                popOrderList = Session[sessonKey] as List<QuoteOrderDetail>;
            }
            if (popOrderList.Any())
            {
                List<int> orderIdList = popOrderList.Select(s=>s.Id).ToList();
                var list = (from order in CurrentContext.DbContext.QuoteOrderDetail
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on order.GuidanceId equals guidance.ItemId
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where orderIdList.Contains(order.Id)
                            select new
                            {
                                order,
                                guidance.ItemName,
                                subject.SubjectName,
                                subject.AddDate,
                                Gender = (order.OrderGender != null && order.OrderGender != "") ? order.OrderGender : order.Gender
                            }).OrderBy(s=>s.order.Sheet).ToList();
                string templateFileName = "350Template";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                ISheet tableSheet = workBook.GetSheetAt(0);

                int startRow = 1;
                string shopno = string.Empty;
                foreach (var item in list)
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
                    if (item.AddDate != null)
                        dataRow.GetCell(1).SetCellValue(DateTime.Parse(item.AddDate.ToString()).ToShortDateString());
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
                    dataRow.GetCell(14).SetCellValue(item.SubjectName);
                    dataRow.GetCell(15).SetCellValue(item.Gender);
                    dataRow.GetCell(16).SetCellValue(item.order.ChooseImg);


                    //dataRow.GetCell(12).SetCellValue(item.Category);
                    dataRow.GetCell(17).SetCellValue(item.order.Sheet);
                    dataRow.GetCell(18).SetCellValue(item.order.MachineFrame);
                    dataRow.GetCell(19).SetCellValue(item.order.PositionDescription);
                    dataRow.GetCell(20).SetCellValue(item.order.Quantity ?? 0);
                    dataRow.GetCell(21).SetCellValue(item.order.GraphicMaterial);
                    dataRow.GetCell(22).SetCellValue(item.order.QuoteGraphicMaterial);
                    dataRow.GetCell(23).SetCellValue(double.Parse((item.order.UnitPrice ?? 0).ToString()));
                    dataRow.GetCell(24).SetCellValue(double.Parse((item.order.TotalGraphicWidth??0).ToString()));
                    dataRow.GetCell(25).SetCellValue(double.Parse((item.order.TotalGraphicLength??0).ToString()));
                    decimal area = ((item.order.TotalGraphicWidth ?? 0) * (item.order.TotalGraphicLength ?? 0)) / 1000000;
                    dataRow.GetCell(26).SetCellValue(double.Parse(area.ToString()));
                    dataRow.GetCell(27).SetCellValue(double.Parse((item.order.AutoAddTotalPrice??0).ToString()));
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
                    OperateFile.DownLoadFile(ms, sessonKey + "变更尺寸后订单明细");
                }
            }
        }
    }
}
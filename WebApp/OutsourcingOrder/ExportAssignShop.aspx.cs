using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;
using System.Text;
using Models;
using System.Configuration;
using System.IO;
using NPOI.SS.UserModel;

namespace WebApp.OutsourcingOrder
{
    public partial class ExportAssignShop : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //void ExportDetail()
        //{
        //    string guidanceIds = string.Empty;
        //    string province = string.Empty;
        //    string city = string.Empty;
        //    if (Request.QueryString["guidanceIds"] != null)
        //    {
        //        guidanceIds = Request.QueryString["guidanceIds"];
        //    }
        //    if (Request.QueryString["province"] != null)
        //    {
        //        province = Request.QueryString["province"];
        //    }
        //    if (Request.QueryString["city"] != null)
        //    {
        //        city = Request.QueryString["city"];
        //    }
        //    List<int> guidanceList = new List<int>();
        //    if (!string.IsNullOrWhiteSpace(guidanceIds))
        //    {
        //        guidanceList = StringHelper.ToIntList(guidanceIds, ',');
        //    }
        //    List<string> provinceList = new List<string>();
        //    if (!string.IsNullOrWhiteSpace(province))
        //    {
        //        provinceList = StringHelper.ToStringList(province, ',');
        //    }
        //    List<string> cityList = new List<string>();
        //    if (!string.IsNullOrWhiteSpace(city))
        //    {
        //        cityList = StringHelper.ToStringList(city, ',');
        //    }
        //    var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
        //                     join subject in CurrentContext.DbContext.Subject
        //                     on order.SubjectId equals subject.Id
        //                     join outsource in CurrentContext.DbContext.Company
        //                      on order.OutsourceId equals outsource.Id
        //                     where
        //                     guidanceList.Contains(order.GuidanceId ?? 0)
        //                     select new
        //                     {
        //                         order,
        //                         subject,
        //                         outsource.CompanyName
        //                     }).ToList();
        //    if (provinceList.Any())
        //    {
        //        orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
        //    }
        //    if (cityList.Any())
        //    {
        //        orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
        //    }

        //    if (orderList.Any())
        //    {
        //        string templateFileName = "外协订单明细";
        //        string path = ConfigurationManager.AppSettings["ExportTemplate"];
        //        path = path.Replace("fileName", templateFileName);
        //        FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
        //        IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
        //        ISheet sheet = workBook.GetSheet("Sheet1");
        //        int startRow = 1;
        //        orderList.ForEach(s =>
        //        {
        //            decimal width = s.order.GraphicWidth ?? 0;
        //            decimal length = s.order.GraphicLength ?? 0;
        //            decimal totalArea = (width * length) / 1000000 * (s.order.Quantity??1);
        //            IRow dataRow = sheet.GetRow(startRow);
        //            if (dataRow == null)
        //                dataRow = sheet.CreateRow(startRow);
        //            for (int i = 0; i < 16; i++)
        //            {
        //                ICell cell = dataRow.GetCell(i);
        //                if (cell == null)
        //                    cell = dataRow.CreateCell(i);

        //            }
        //            dataRow.GetCell(0).SetCellValue(s.order.ShopNo);

        //            dataRow.GetCell(1).SetCellValue(s.order.ShopName);
                    
        //            dataRow.GetCell(3).SetCellValue(s.order.Province);
        //            dataRow.GetCell(4).SetCellValue(s.order.City);
        //            dataRow.GetCell(5).SetCellValue(s.order.CityTier);
        //            dataRow.GetCell(6).SetCellValue(s.order.IsInstall);
        //            dataRow.GetCell(7).SetCellValue(s.subject.SubjectName);
        //            dataRow.GetCell(8).SetCellValue(s.order.Sheet);

        //            dataRow.GetCell(9).SetCellValue(s.order.PositionDescription);
        //            dataRow.GetCell(10).SetCellValue(s.order.Gender);
        //            dataRow.GetCell(11).SetCellValue(s.order.ChooseImg);
        //            dataRow.GetCell(12).SetCellValue(s.order.GraphicMaterial);
        //            dataRow.GetCell(13).SetCellValue(s.Quantity);
        //            dataRow.GetCell(14).SetCellValue(s.TotalPrice);
        //            dataRow.GetCell(15).SetCellValue(s.SubjectName);
        //            startRow++;
        //        });
        //        HttpCookie cookie = Request.Cookies["项目费用统计明细"];
        //        if (cookie == null)
        //        {
        //            cookie = new HttpCookie("项目费用统计明细");
        //        }
        //        cookie.Value = "1";
        //        cookie.Expires = DateTime.Now.AddMinutes(30);
        //        Response.Cookies.Add(cookie);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            workBook.Write(ms);
        //            ms.Flush();
        //            sheet = null;
        //            workBook = null;
        //            OperateFile.DownLoadFile(ms, templateFileName);

        //        }


        //    }
        //    else
        //    {
        //        HttpCookie cookie = Request.Cookies["项目费用统计明细"];
        //        if (cookie == null)
        //        {
        //            cookie = new HttpCookie("项目费用统计明细");
        //        }
        //        cookie.Value = "2";
        //        cookie.Expires = DateTime.Now.AddMinutes(30);
        //        Response.Cookies.Add(cookie);
        //    }
        //}
    }
}
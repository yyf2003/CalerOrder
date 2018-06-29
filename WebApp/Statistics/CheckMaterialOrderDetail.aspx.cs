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
using NPOI.SS.UserModel;
using System.Configuration;
using System.IO;

namespace WebApp.Statistics
{
    public partial class CheckMaterialOrderDetail : BasePage
    {
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        string guidanceIds = string.Empty;
        string subjectIds = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["regions"] != null)
            {
                region = Request.QueryString["regions"];
            }
            if (Request.QueryString["provinces"] != null)
            {
                province = Request.QueryString["provinces"];
            }
            if (Request.QueryString["citys"] != null)
            {
                city = Request.QueryString["citys"];
            }
            if (Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = Request.QueryString["guidanceIds"];
            }
            if (Request.QueryString["subjectids"] != null)
            {
                subjectIds = Request.QueryString["subjectids"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }



        List<int> guidanceIdList = new List<int>();
        List<int> subjectIdList = new List<int>();
        List<string> regionList = new List<string>();
        List<string> provinceList = new List<string>();
        List<string> cityList = new List<string>();

        void GetCondition()
        {
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                //guidanceIdList = StringHelper.ToIntList(guidanceIds, ',');
                if (!string.IsNullOrWhiteSpace(subjectIds))
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');

                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                else
                {
                    GetResponsibleRegion.ForEach(s =>
                    {
                        regionList.Add(s.ToLower());
                    });
                }

                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                }
            }
        }


        void BindData()
        {
            
            GetCondition();
            var orderList = (from order in CurrentContext.DbContext.OrderMaterial
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where
                             (subjectIdList.Contains(subject.Id) || subjectIdList.Contains(subject.HandMakeSubjectId??0))
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1

                             select new { order, shop, subject }).ToList();
            
            if (regionList.Any())
                orderList = orderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            if (provinceList.Any())
                orderList = orderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            if (cityList.Any())
                orderList = orderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            if (labTotalPrice.Text == "")
            {
                decimal totalPrice = 0;
                orderList.ForEach(s => {
                    totalPrice += ((s.order.MaterialCount ?? 1) * (s.order.Price ?? 0));
                });
                if (totalPrice > 0)
                {
                    labTotalPrice.Text = Math.Round(totalPrice, 2).ToString();
                }
                else
                {
                    labTotalPrice.Text = "0";
                }
            }
            AspNetPager1.RecordCount = orderList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = orderList.OrderBy(s => s.shop.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvList.DataBind();

        }


        protected void btnExport_Click(object sender, EventArgs e)
        {
            GetCondition();
            var orderList = (from order in CurrentContext.DbContext.OrderMaterial
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where
                             (subjectIdList.Contains(subject.Id) || subjectIdList.Contains(subject.HandMakeSubjectId ?? 0))
                             && (subject.IsDelete == null || subject.IsDelete == false)
                           
                             && subject.ApproveState == 1

                             select new { order, shop, subject }).ToList();
            //if (subjectIdList.Any())
            //    orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
            if (regionList.Any())
                orderList = orderList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            if (provinceList.Any())
                orderList = orderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            if (cityList.Any())
                orderList = orderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            if (orderList.Any())
            {
                orderList = orderList.OrderBy(s => s.shop.Id).ToList();
                string templateFileName = "MaterialOrderTemplate";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                ISheet sheet = workBook.GetSheet("Sheet1");

                int startRow = 1;
                string shopno = string.Empty;
                foreach (var item in orderList)
                {

                    IRow dataRow = sheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = sheet.CreateRow(startRow);
                    for (int i = 0; i < 11; i++)
                    {
                        ICell cell = dataRow.GetCell(i);
                        if (cell == null)
                            cell = dataRow.CreateCell(i);

                    }
                    dataRow.GetCell(0).SetCellValue(item.subject.SubjectName);
                    dataRow.GetCell(1).SetCellValue(item.shop.ShopNo);
                    dataRow.GetCell(2).SetCellValue(item.shop.ShopName);
                    dataRow.GetCell(3).SetCellValue(item.order.Sheet);
                    dataRow.GetCell(4).SetCellValue(item.order.MaterialName);
                    if (item.order.MaterialCount != null)
                        dataRow.GetCell(5).SetCellValue(item.order.MaterialCount ?? 0);
                    if (item.order.MaterialLength != null)
                        dataRow.GetCell(6).SetCellValue(double.Parse((item.order.MaterialLength ?? 0).ToString()));
                    if (item.order.MaterialWidth != null)
                        dataRow.GetCell(7).SetCellValue(double.Parse((item.order.MaterialWidth ?? 0).ToString()));
                    if (item.order.MaterialHigh != null)
                        dataRow.GetCell(8).SetCellValue(double.Parse((item.order.MaterialHigh ?? 0).ToString()));
                    if (item.order.Price != null)
                        dataRow.GetCell(9).SetCellValue(double.Parse((item.order.Price ?? 0).ToString()));
                    dataRow.GetCell(10).SetCellValue(item.order.Remark);

                    startRow++;

                }



                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    sheet = null;
                    workBook = null;
                    OperateFile.DownLoadFile(ms, "物料订单信息");

                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
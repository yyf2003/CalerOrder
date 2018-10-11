using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using BLL;
using DAL;
using Models;
using Common;
using NPOI.SS.UserModel;
using System.IO;

namespace WebApp.Subjects
{
    public partial class ExportOrder2 : BasePage
    {
        int itemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                itemId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (!IsPostBack)
            {
                SubjectGuidance guidance = new SubjectGuidanceBLL().GetModel(itemId);
                if (guidance != null)
                {
                    labGuidanceName.Text = guidance.ItemName;
                }
                BindSubjectCategory();
            }
        }

        void BindSubjectCategory()
        {
            cblCategory.Items.Clear();
            cblSubject.Items.Clear();
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                join category1 in CurrentContext.DbContext.ADSubjectCategory
                                on subject.SubjectCategoryId equals category1.Id into categoryTemp
                                from category in categoryTemp.DefaultIfEmpty()
                                where order.GuidanceId == itemId
                                && (subject.IsDelete == null || subject.IsDelete == false)
                                && subject.Status == 4
                                select new { subject, category }).ToList();
            if (orderList.Any())
            {
                var categoryList = orderList.Select(s=>s.category).Distinct().ToList();
                bool isNull = false;
                categoryList.ForEach(s => {
                    if (s != null)
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.CategoryName + "&nbsp;";
                        cblCategory.Items.Add(li);
                    }
                    else
                        isNull = true;
                });
                if (isNull)
                {
                    ListItem li = new ListItem();
                    li.Value = "0";
                    li.Text = "空";
                    cblCategory.Items.Add(li);
                }

                var subjectList = orderList.Select(s => s.subject).OrderBy(s=>s.SubjectName).Distinct().ToList();
                if (subjectList.Any())
                {
                    subjectList.ForEach(s => {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectName + "&nbsp;";
                        cblSubject.Items.Add(li);
                    });
                }
            }
            

        }

        void BindSubject()
        {
            cblSubject.Items.Clear();
            List<int> categoryIdList = new List<int>();
            foreach (ListItem li in cblCategory.Items)
            {
                if (li.Selected)
                {
                    categoryIdList.Add(int.Parse(li.Value));
                }
            }
            List<Subject> subjectList = new SubjectBLL().GetList(s=>s.GuidanceId==itemId && (s.IsDelete==null || s.IsDelete==false) && s.Status==4).OrderBy(s=>s.SubjectName).ToList();
            if (categoryIdList.Any())
            {
                if (categoryIdList.Contains(0))
                {
                    categoryIdList.Remove(0);
                    if (categoryIdList.Any())
                    {
                        subjectList = subjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0) && (s.SubjectCategoryId ?? 0) == 0).ToList();
                    }
                    else
                    {
                        subjectList = subjectList.Where(s => (s.SubjectCategoryId ?? 0) == 0).ToList();
                    }
                }
                else
                {
                    subjectList = subjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0)).ToList();
                }
            }
            if (subjectList.Any())
            {
                subjectList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.SubjectName + "&nbsp;";
                    cblSubject.Items.Add(li);
                });
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<int> subjectIdList = new List<int>();
            foreach (ListItem li in cblSubject.Items)
            {
                if (li.Selected)
                {
                    subjectIdList.Add(int.Parse(li.Value));
                }
            }
            //if (!subjectIdList.Any())
            //{
            //    foreach (ListItem li in cblSubject.Items)
            //    {
            //        subjectIdList.Add(int.Parse(li.Value));
            //    }
            //}
            
            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
            //List<FinalOrderDetailTemp> totalOrderList = orderBll.GetList(s =>s.GuidanceId==itemId && (s.IsDelete==null || s.IsDelete==false));
            var totalOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                  join subject in CurrentContext.DbContext.Subject
                                  on order.SubjectId equals subject.Id
                                  join subject1 in CurrentContext.DbContext.Subject
                                  on order.RegionSupplementId equals subject1.Id into temp
                                  from SupplementSubject in temp.DefaultIfEmpty()
                                  where order.GuidanceId == itemId && (order.IsDelete == null || order.IsDelete == false)
                                  select new { order, subject.SubjectName, SupplementSubjectName = SupplementSubject!=null?SupplementSubject.SubjectName:"" }).ToList();
            if (subjectIdList.Any())
            {
                SubjectBLL subjectBll = new SubjectBLL();
                List<Subject> subjectList = subjectBll.GetList(s => s.GuidanceId == itemId && (s.IsDelete == null || s.IsDelete == false) && s.Status == 4);
                List<int> regionSubjectIdList = subjectList.Where(s => subjectIdList.Contains(s.Id) && ((s.SubjectType ?? 1) == (int)SubjectTypeEnum.HC订单 || (s.SubjectType ?? 1) == (int)SubjectTypeEnum.分区补单)).Select(s => s.Id).ToList();
                var newOrderList = totalOrderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0))
                                   .Union(totalOrderList.Where(s => regionSubjectIdList.Contains(s.order.RegionSupplementId ?? 0))).ToList();
                totalOrderList = newOrderList;
            }
            if (totalOrderList.Any())
            {
                string templateFileName = "350Template";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                ISheet sheet = workBook.GetSheetAt(0);

                int startRow = 1;
                string shopno = string.Empty;
                totalOrderList.OrderBy(s => s.order.ShopId).ToList().ForEach(s => {

                    IRow dataRow = sheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = sheet.CreateRow(startRow);
                    for (int i = 0; i < 32; i++)
                    {
                        ICell cell = dataRow.GetCell(i);
                        if (cell == null)
                            cell = dataRow.CreateCell(i);

                    }
                    string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                    dataRow.GetCell(0).SetCellValue(orderType);
                    if (s.order.AddDate != null)
                        dataRow.GetCell(1).SetCellValue(DateTime.Parse(s.order.AddDate.ToString()).ToShortDateString());
                    dataRow.GetCell(2).SetCellValue(s.order.ShopNo);
                    dataRow.GetCell(3).SetCellValue(s.order.ShopName);
                    dataRow.GetCell(4).SetCellValue(s.order.Region);
                    dataRow.GetCell(5).SetCellValue(s.order.Province);
                    dataRow.GetCell(6).SetCellValue(s.order.City);
                    dataRow.GetCell(7).SetCellValue(s.order.CityTier);
                    dataRow.GetCell(8).SetCellValue(s.order.Channel);
                    dataRow.GetCell(9).SetCellValue(s.order.Format);
                    dataRow.GetCell(10).SetCellValue(s.order.POPAddress);
                    dataRow.GetCell(11).SetCellValue(s.order.Contact);
                    dataRow.GetCell(12).SetCellValue(s.order.Tel);
                    dataRow.GetCell(13).SetCellValue(s.order.POSScale);
                    dataRow.GetCell(14).SetCellValue(s.order.MaterialSupport);
                    dataRow.GetCell(15).SetCellValue(s.SubjectName);
                    dataRow.GetCell(16).SetCellValue(s.order.Gender);
                    dataRow.GetCell(17).SetCellValue(s.order.ChooseImg);


                    //dataRow.GetCell(12).SetCellValue(item.Category);
                    dataRow.GetCell(18).SetCellValue(s.order.Sheet);
                    dataRow.GetCell(19).SetCellValue(s.order.MachineFrame);
                    dataRow.GetCell(20).SetCellValue(s.order.PositionDescription);
                    dataRow.GetCell(21).SetCellValue(s.order.Quantity??1);
                    dataRow.GetCell(22).SetCellValue(s.order.GraphicMaterial);
                    dataRow.GetCell(23).SetCellValue(s.order.GraphicMaterial);
                    dataRow.GetCell(24).SetCellValue(double.Parse((s.order.UnitPrice ?? 0).ToString()));
                    dataRow.GetCell(25).SetCellValue(double.Parse((s.order.GraphicWidth??0).ToString()));
                    dataRow.GetCell(26).SetCellValue(double.Parse((s.order.GraphicLength??0).ToString()));
                    dataRow.GetCell(27).SetCellValue(double.Parse((s.order.Area??0).ToString()));
                    //其他备注
                    dataRow.GetCell(29).SetCellValue(s.order.Remark);
                    dataRow.GetCell(30).SetCellValue(s.order.IsInstall);

                    dataRow.GetCell(31).SetCellValue(s.SupplementSubjectName);
                    //dataRow.GetCell(27).SetCellValue(item.NewFormat);
                    startRow++;
                });
                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    sheet = null;
                    workBook = null;
                    string fileName = labGuidanceName.Text;
                    OperateFile.DownLoadFile(ms, fileName + "订单表");

                }
            }
        }

        protected void cblCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
        }


    }
}
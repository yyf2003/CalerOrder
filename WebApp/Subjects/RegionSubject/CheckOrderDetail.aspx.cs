using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using BLL;
using Common;
using DAL;
using Models;
using NPOI.SS.UserModel;

namespace WebApp.Subjects.RegionSubject
{
    public partial class CheckOrderDetail : BasePage
    {
        public int SubjectId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                SubjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                PreviousUrl = "";
                BindSubject();
                GetApproveInfo();
            }
        }

        void BindSubject()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                         join customer in CurrentContext.DbContext.Customer
                         on subject.CustomerId equals customer.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on subject.AddUserId equals user.UserId
                         where subject.Id == SubjectId
                         select new
                         {
                             subject,
                             customer.CustomerName,
                             user.RealName

                         }).FirstOrDefault();
            if (model != null)
            {
                labSubjectNo.Text = model.subject.SubjectNo;
                labSubjectName.Text = model.subject.SubjectName;
                labAddUserName.Text = model.RealName;
                labCustomerName.Text = model.CustomerName;
                int subjectType = model.subject.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());
                labRegion.Text = model.subject.SupplementRegion;
                if ((model.subject.ApproveState ?? 0) == 2 && model.subject.AddUserId==CurrentUser.UserId)
                {
                    spanEdit.Style.Remove("display");
                    btnDelete.Visible = true;
                }
            }
        }

        /// <summary>
        /// 显示审批记录
        /// </summary>
        void GetApproveInfo()
        {
            var list = (from approve in CurrentContext.DbContext.ApproveInfo
                        join user in CurrentContext.DbContext.UserInfo
                        on approve.AddUserId equals user.UserId
                        where approve.SubjectId == SubjectId
                        select new
                        {
                            approve,
                            UserName = user.RealName,
                        }).ToList();
            if (list.Any())
            {
                Dictionary<int, string> approveStateDic = new Dictionary<int, string>();
                approveStateDic.Add(0, "待审批");
                approveStateDic.Add(1, "<span style='color:green;'>审批通过</span>");
                approveStateDic.Add(2, "<span style='color:red;'>审批不通过</span>");
                StringBuilder tb = new StringBuilder();
                list.ForEach(s =>
                {
                    int approveState = s.approve.Result ?? 0;
                    tb.Append("<table class=\"table\" style=\"margin-bottom:6px;\">");
                    tb.AppendFormat("<tr class=\"tr_hui\"><td style=\"width: 100px;\">审批时间</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", s.approve.AddDate);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批结果</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", approveStateDic[approveState]);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批人</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", s.UserName);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批意见</td><td style=\"text-align: left; padding-left: 5px;height: 60px;\">{0}</td></tr>", s.approve.Remark);
                    tb.Append("</table>");


                });
                approveInfoDiv.InnerHtml = tb.ToString();
                Panel_ApproveInfo.Visible = true;
            }
        }

        /// <summary>
        /// 导出POP订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportPOPOrder_Click(object sender, EventArgs e)
        {
            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.HandMakeSubjectId equals subject.Id into subjectTemp
                        from subject1 in subjectTemp.DefaultIfEmpty()
                        where order.SubjectId == SubjectId
                        select new
                        {
                            order,
                            shop,
                            subject1.SubjectName
                        }).OrderBy(s=>s.shop.Id).ThenBy(s=>s.order.HandMakeSubjectId).ToList();
            if (list.Any())
            {
                string templateFileName = "350Template";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                ISheet sheet = workBook.GetSheetAt(0);
                int startRow = 1;
                list.ForEach(s =>
                {
                    IRow dataRow = sheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = sheet.CreateRow(startRow);
                    for (int i = 0; i < 30; i++)
                    {
                        ICell cell = dataRow.GetCell(i);
                        if (cell == null)
                            cell = dataRow.CreateCell(i);

                    }
                    decimal area = 0;
                    decimal width1 = s.order.GraphicWidth ?? 0;
                    decimal length1 = s.order.GraphicLength ?? 0;
                    area = (width1 * length1) / 1000000;
                    
                    dataRow.GetCell(0).SetCellValue(s.shop.ShopNo);
                    dataRow.GetCell(1).SetCellValue(s.shop.ShopName);
                    dataRow.GetCell(2).SetCellValue(s.shop.ProvinceName);
                    dataRow.GetCell(3).SetCellValue(s.shop.CityName);
                    dataRow.GetCell(4).SetCellValue(s.shop.CityTier);
                    dataRow.GetCell(5).SetCellValue(s.shop.Format);
                    dataRow.GetCell(6).SetCellValue(s.shop.POPAddress);
                    dataRow.GetCell(7).SetCellValue(s.shop.Contact1);
                    dataRow.GetCell(8).SetCellValue(s.shop.Tel1);

                    dataRow.GetCell(9).SetCellValue(s.order.POSScale);
                    dataRow.GetCell(10).SetCellValue(s.order.MaterialSupport);
                    dataRow.GetCell(11).SetCellValue(s.SubjectName);
                    dataRow.GetCell(12).SetCellValue(s.order.Gender);
                    dataRow.GetCell(13).SetCellValue(s.order.ChooseImg);

                    dataRow.GetCell(14).SetCellValue(s.order.Sheet);
                    dataRow.GetCell(15).SetCellValue(s.order.MachineFrame);
                    dataRow.GetCell(16).SetCellValue(s.order.PositionDescription);
                    dataRow.GetCell(17).SetCellValue(s.order.Quantity ?? 1);
                    dataRow.GetCell(18).SetCellValue(s.order.GraphicMaterial);

                    dataRow.GetCell(21).SetCellValue(double.Parse(width1.ToString()));
                    dataRow.GetCell(22).SetCellValue(double.Parse(length1.ToString()));
                    dataRow.GetCell(23).SetCellValue(double.Parse(area.ToString()));
                    //其他备注
                    dataRow.GetCell(23).SetCellValue(s.order.Remark);

                    startRow++;
                });

                //HttpCookie cookie = Request.Cookies["exportRegionSupplement"];
                //if (cookie == null)
                //{
                //    cookie = new HttpCookie("exportRegionSupplement");
                //}
                //cookie.Value = "1";
                //cookie.Expires = DateTime.Now.AddMinutes(10);
                //Response.Cookies.Add(cookie);

                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    sheet = null;
                    workBook = null;
                    string fileName = labSubjectName.Text;
                    OperateFile.DownLoadFile(ms, fileName + "350表");


                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddSubject.aspx?subjectId="+SubjectId,false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SubjectBLL bll = new SubjectBLL();
            Subject model = bll.GetModel(SubjectId);
            if (model != null)
            {
                model.DeleteDate = DateTime.Now;
                model.IsDelete = true;
                bll.Update(model);
                Alert("删除成功！", PreviousUrl);
            }
        }
    }
}
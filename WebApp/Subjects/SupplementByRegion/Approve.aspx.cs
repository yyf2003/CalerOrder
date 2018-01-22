using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using Common;
using BLL;
using System.Transactions;
using Models;
using NPOI.SS.UserModel;
using System.Configuration;
using System.IO;

namespace WebApp.Subjects.SupplementByRegion
{
    public partial class Approve : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindData();
            }
        }

        void BindSubject()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on subject.AddUserId equals user.UserId
                        where subject.Id == subjectId
                        select new { 
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
                int subjectType = model.subject.SubjectType ?? 0;
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                labRegion.Text= model.subject.SupplementRegion;
            }
        }

        void BindData()
        {
            var list = (from order in CurrentContext.DbContext.RegionSupplementDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.ItemId == subjectId
                        select new
                        {
                            shop,
                            order
                        }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            orderList.DataSource = list.OrderByDescending(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            orderList.DataBind();
            
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text.Trim();
            SubjectBLL subjectBll = new SubjectBLL();
            bool isApproveOk = false;
            string msg = string.Empty;
            int subjectType = 1;
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {

                    var list = from order in CurrentContext.DbContext.RegionSupplementDetail
                               join shop in CurrentContext.DbContext.Shop
                               on order.ShopId equals shop.Id
                               where order.ItemId == subjectId
                               select new
                               {
                                   order,
                                   shop
                               };
                    if (list.Any())
                    {
                        string unitName = string.Empty;
                        Models.Subject model = subjectBll.GetModel(subjectId);
                        if (model != null)
                        {

                            FinalOrderDetailTempBLL finalOrderTempBll = new FinalOrderDetailTempBLL();
                            finalOrderTempBll.Delete(s => s.RegionSupplementId == subjectId);
                            FinalOrderDetailTemp finalOrderTempModel;
                            list.ToList().ForEach(o =>
                            {
                                finalOrderTempModel = new FinalOrderDetailTemp();
                                finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                finalOrderTempModel.AgentName = o.shop.AgentName;
                                finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                finalOrderTempModel.Channel = o.shop.Channel;
                                finalOrderTempModel.City = o.shop.CityName;
                                finalOrderTempModel.CityTier = o.shop.CityTier;
                                finalOrderTempModel.Contact = o.shop.Contact1;
                                finalOrderTempModel.Format = o.shop.Format;
                                finalOrderTempModel.Gender = o.order.Gender;
                                //finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                finalOrderTempModel.LocationType = o.shop.LocationType;
                                finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                finalOrderTempModel.OrderType = 1;
                                finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                finalOrderTempModel.POSScale = !string.IsNullOrWhiteSpace(o.order.POSScale) ? o.order.POSScale : o.shop.POSScale;
                                finalOrderTempModel.InstallPricePOSScale = finalOrderTempModel.POSScale;
                                finalOrderTempModel.Province = o.shop.ProvinceName;
                                finalOrderTempModel.Quantity = o.order.Quantity;
                                finalOrderTempModel.Region = o.shop.RegionName;
                                finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                                finalOrderTempModel.Remark = o.order.Remark;
                                //finalOrderTempModel.Remark = "分区补单";
                                finalOrderTempModel.Sheet = o.order.Sheet;
                                finalOrderTempModel.ShopId = o.shop.Id;
                                finalOrderTempModel.ShopName = o.shop.ShopName;
                                finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                finalOrderTempModel.SubjectId = o.order.SubjectId;
                                finalOrderTempModel.Tel = o.shop.Tel1;
                                finalOrderTempModel.MachineFrame = o.order.MachineFrame;
                                finalOrderTempModel.LevelNum = o.order.LevelNum;
                                decimal width = o.order.GraphicWidth ?? 0;
                                decimal length = o.order.GraphicLength ?? 0;
                                finalOrderTempModel.GraphicLength = length;
                                finalOrderTempModel.GraphicMaterial = o.order.GraphicMaterial;
                                finalOrderTempModel.GraphicWidth = width;
                                //finalOrderTempModel.UnitPrice = GetMaterialPrice(o.order.GraphicMaterial);
                                finalOrderTempModel.PositionDescription = o.order.PositionDescription;
                                finalOrderTempModel.Area = (width * length) / 1000000;
                                finalOrderTempModel.RegionSupplementId = subjectId;
                                finalOrderTempModel.InstallPositionDescription = o.order.InstallPositionDescription;

                                decimal unitPrice = 0;
                                decimal totalPrice = 0;
                                if (!string.IsNullOrWhiteSpace(o.order.GraphicMaterial))
                                {
                                    POP pop = new POP();
                                    pop.GraphicMaterial = o.order.GraphicMaterial;
                                    pop.GraphicLength = o.order.GraphicLength;
                                    pop.GraphicWidth = o.order.GraphicWidth;
                                    pop.Quantity = o.order.Quantity;
                                    unitPrice = GetMaterialUnitPrice(pop, out totalPrice, out unitName);
                                }
                                finalOrderTempModel.UnitPrice = unitPrice;
                                finalOrderTempModel.TotalPrice = totalPrice;
                                finalOrderTempModel.CSUserId = o.shop.CSUserId;

                                finalOrderTempBll.Add(finalOrderTempModel);
                            });




                            subjectType = model.SubjectType ?? 1;
                            model.ApproveState = result;
                            model.ApproveUserId = CurrentUser.UserId;
                            subjectBll.Update(model);
                            ApproveInfo approveModel = new ApproveInfo() { AddDate = DateTime.Now, AddUserId = CurrentUser.UserId, Remark = txtRemark.Text.Trim(), Result = result, SubjectId = subjectId };
                            new ApproveInfoBLL().Add(approveModel);
                            tran.Complete();
                            isApproveOk = true;
                        }
                    }

                    
                    
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            if (isApproveOk)
            {
                //if (subjectType!=2)
                //Sent();
                Alert("审批成功！", "ApproveList.aspx");
            }
            else
                Alert("审批失败：" + msg);
        }

        protected void lbExport_Click(object sender, EventArgs e)
        {
            var list = (from order in CurrentContext.DbContext.RegionSupplementDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where order.ItemId == subjectId
                        select new
                        {
                            subject,
                            shop,
                            order
                        }).ToList();
            if (list.Any())
            {
                string templateFileName = "350Template";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                ISheet sheet = workBook.GetSheetAt(0);
                int startRow = 1;
                list.ForEach(s => {
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
                    decimal width1 = s.order.GraphicWidth??0;
                    decimal length1 = s.order.GraphicLength??0;
                    area = (width1 * length1) / 1000000;
                    //dataRow.GetCell(0).SetCellValue(s.shop.ShopNo);
                    //dataRow.GetCell(1).SetCellValue(s.shop.ShopName);
                    //dataRow.GetCell(2).SetCellValue(s.shop.ProvinceName);
                    //dataRow.GetCell(3).SetCellValue(s.shop.CityName);
                    //dataRow.GetCell(4).SetCellValue(s.shop.CityTier);
                    //dataRow.GetCell(5).SetCellValue(s.shop.Format);
                    //dataRow.GetCell(6).SetCellValue(s.shop.POPAddress);
                    //dataRow.GetCell(7).SetCellValue(s.shop.Contact1);
                    //dataRow.GetCell(8).SetCellValue(s.shop.Tel1);

                    //dataRow.GetCell(9).SetCellValue(s.order.POSScale);
                    //dataRow.GetCell(10).SetCellValue(s.order.MaterialSupport);
                    //dataRow.GetCell(11).SetCellValue(s.subject.SubjectName);
                    //dataRow.GetCell(12).SetCellValue(s.order.Gender);
                    //dataRow.GetCell(13).SetCellValue(s.order.ChooseImg);

                    //dataRow.GetCell(14).SetCellValue(s.order.Sheet);
                    //dataRow.GetCell(15).SetCellValue(s.order.PositionDescription);
                    //dataRow.GetCell(16).SetCellValue(s.order.Quantity??1);
                    //dataRow.GetCell(17).SetCellValue(s.order.GraphicMaterial);
                    //dataRow.GetCell(18).SetCellValue(double.Parse(width1.ToString()));
                    //dataRow.GetCell(19).SetCellValue(double.Parse(length1.ToString()));
                    //dataRow.GetCell(20).SetCellValue(double.Parse(area.ToString()));
                    ////其他备注
                    //dataRow.GetCell(21).SetCellValue(s.order.Remark);
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
                    dataRow.GetCell(11).SetCellValue(s.order.SubjectName);
                    dataRow.GetCell(12).SetCellValue(s.order.Gender);
                    dataRow.GetCell(13).SetCellValue(s.order.ChooseImg);

                    dataRow.GetCell(14).SetCellValue(s.order.Sheet);
                    dataRow.GetCell(15).SetCellValue(s.order.MachineFrame);
                    dataRow.GetCell(16).SetCellValue(s.order.PositionDescription);
                    dataRow.GetCell(17).SetCellValue(s.order.Quantity ?? 1);
                    dataRow.GetCell(18).SetCellValue(s.order.GraphicMaterial);
                    dataRow.GetCell(19).SetCellValue(double.Parse(width1.ToString()));
                    dataRow.GetCell(20).SetCellValue(double.Parse(length1.ToString()));
                    dataRow.GetCell(21).SetCellValue(double.Parse(area.ToString()));
                    //其他备注
                    dataRow.GetCell(22).SetCellValue(s.order.Remark);
                    
                    startRow++;
                });

                HttpCookie cookie = Request.Cookies["exportRegionSupplement"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("exportRegionSupplement");
                }
                cookie.Value = "1";
                cookie.Expires = DateTime.Now.AddMinutes(10);
                Response.Cookies.Add(cookie);

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
    }
}
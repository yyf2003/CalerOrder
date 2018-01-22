using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using BLL;
using Common;
using DAL;
using NPOI.SS.UserModel;
using System.Web.UI.WebControls;


namespace WebApp.Subjects
{
    public partial class ExportOrder : BasePage
    {
        public int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindOrder();
                
            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                join user1 in CurrentContext.DbContext.UserInfo
                                on subject.ApproveUserId equals user1.UserId into userTemp
                                from user in userTemp.DefaultIfEmpty()
                                where subject.Id == subjectId
                                select new
                                {
                                    subject,
                                    customer.CustomerName
                                    
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                labSubjectNo.Text = subjectModel.subject.SubjectNo;
                labSubjectName.Text = subjectModel.subject.SubjectName;
                labOutSubjectName.Text = subjectModel.subject.OutSubjectName;
                labBeginDate.Text = subjectModel.subject.BeginDate != null ? DateTime.Parse(subjectModel.subject.BeginDate.ToString()).ToShortDateString() : "";
                labEndDate.Text = subjectModel.subject.EndDate != null ? DateTime.Parse(subjectModel.subject.EndDate.ToString()).ToShortDateString() : "";
                labContact.Text = subjectModel.subject.Contact;
                labTel.Text = subjectModel.subject.Tel;
                labCustomerName.Text = subjectModel.CustomerName;
                
                labRemark.Text = subjectModel.subject.Remark;
                
            }
        }

        void BindOrder()
        {


            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                       on order.ShopId equals shop.Id
                       join pop1 in CurrentContext.DbContext.POP
                       on new { order.ShopId, order.GraphicNo,order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo,pop1.Sheet } into popTemp
                       from pop in popTemp.DefaultIfEmpty()
                        where order.SubjectId == subjectId
                        && ((order.OrderType != null && order.OrderType == 1 && order.GraphicWidth != null && order.GraphicWidth != 0 && order.GraphicLength != null && order.GraphicLength != 0) || (order.OrderType != null && order.OrderType == 2))
                        select new
                       {
                           order,
                           shop,
                           Sheet=order.Sheet,
                           LevelNum=order.LevelNum,
                           pop
                       }).ToList();
            if (!string.IsNullOrWhiteSpace(hfRegion.Value))
            {
                List<string> region = StringHelper.ToStringList(hfRegion.Value,',');
                list = list.Where(s => region.Contains(s.shop.RegionName)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(hfProvince.Value))
            {
                List<string> province = StringHelper.ToStringList(hfProvince.Value, ',');
                list = list.Where(s => province.Contains(s.shop.ProvinceName)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(hfCity.Value))
            {
                List<string> city = StringHelper.ToStringList(hfCity.Value, ',');
                list = list.Where(s => city.Contains(s.shop.CityName)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPOP.DataSource = list.OrderBy(s => s.order.ShopId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPOP.DataBind();

        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindOrder();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataSet ds = new FinalOrderDetailTempBLL().GetOrderList(subjectId.ToString(),"","true");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                
                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable dt = CommonMethod.CreateTB(cols);
                var list = ds.Tables[0].AsEnumerable();
                if (!string.IsNullOrWhiteSpace(hfRegion.Value))
                {
                    List<string> region = StringHelper.ToStringList(hfRegion.Value, ',');
                    list = list.Where(s => region.Contains(s.Field<string>("Region")));
                    

                }
                if (!string.IsNullOrWhiteSpace(hfProvince.Value))
                {
                    List<string> province = StringHelper.ToStringList(hfProvince.Value, ',');
                    list = list.Where(s => province.Contains(s.Field<string>("Province")));
                }
                if (!string.IsNullOrWhiteSpace(hfCity.Value))
                {
                    List<string> city = StringHelper.ToStringList(hfCity.Value, ',');
                    list = list.Where(s => city.Contains(s.Field<string>("City")));
                }
                if (list.Any())
                {
                    
                    list.ToList().ForEach(s =>
                    {
                        //DataRow dr = dt.NewRow();
                        dt.LoadDataRow(s.ItemArray, LoadOption.OverwriteChanges);
                        //dt.Rows.Add(dr);
                    });
                    
                    OperateFile.ExportExcel(dt, "POP订单");
                }
                else
                    Alert("没有数据可以导出");
            }
            else
                Alert("没有数据可以导出");
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindOrder();
        }

        /// <summary>
        /// 导出350
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport350_Click(object sender, EventArgs e)
        {
            
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join pop1 in CurrentContext.DbContext.POP
                        on new { order.ShopId, order.GraphicNo,order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo,pop1.Sheet } into popTemp
                        from pop in popTemp.DefaultIfEmpty()
                        //pop订单中，pop尺寸为空的不导出
                        where order.SubjectId == subjectId && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                        select new
                        {
                            subject,
                            shop,
                            order,
                            pop
                        }).ToList();
            if (!string.IsNullOrWhiteSpace(hfRegion.Value))
            {
                List<string> region = StringHelper.ToStringList(hfRegion.Value, ',');
                list = list.Where(s => region.Contains(s.order.Region)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(hfProvince.Value))
            {
                List<string> province = StringHelper.ToStringList(hfProvince.Value, ',');
                list = list.Where(s => province.Contains(s.order.Province)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(hfCity.Value))
            {
                List<string> city = StringHelper.ToStringList(hfCity.Value, ',');
                list = list.Where(s => city.Contains(s.order.City)).ToList();
            }
            if (list.Any())
            {
                string templateFileName = "350Template";
                string path = ConfigurationManager.AppSettings["ExportTemplate"];
                path = path.Replace("fileName", templateFileName);
                FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read,FileShare.Read);
                IWorkbook workBook = WorkbookFactory.Create(outFile,ImportOption.All);
                
                ISheet sheet = workBook.GetSheet("总表");

                int startRow = 1;
                string shopno = string.Empty;
                foreach (var item in list)
                {


                    if (startRow == 1)
                        shopno = item.order.ShopNo;
                    else
                    {
                        if (shopno != item.order.ShopNo)
                        {
                            shopno = item.order.ShopNo;
                            int row = startRow + 1;
                            while (row.ToString().Substring(row.ToString().Length - 1, 1) != "2")
                            {
                                startRow++;
                                row = startRow + 1;
                            }

                        }
                    }

                    IRow dataRow = sheet.GetRow(startRow);
                    if (dataRow == null)
                        dataRow = sheet.CreateRow(startRow);
                    for (int i = 0; i < 23; i++)
                    {
                        ICell cell = dataRow.GetCell(i);
                        if (cell == null)
                            cell = dataRow.CreateCell(i);

                    }
                    dataRow.GetCell(0).SetCellValue(item.order.ShopNo);
                    dataRow.GetCell(1).SetCellValue(item.order.ShopName);
                    dataRow.GetCell(2).SetCellValue(item.order.Province);
                    dataRow.GetCell(3).SetCellValue(item.order.City);
                    dataRow.GetCell(4).SetCellValue(item.order.CityTier);
                    dataRow.GetCell(5).SetCellValue(item.order.Format);
                    dataRow.GetCell(6).SetCellValue(item.order.POPAddress);
                    dataRow.GetCell(7).SetCellValue(item.shop.Contact1 + "/" + item.shop.Contact2);
                    dataRow.GetCell(8).SetCellValue(item.shop.Tel1 + "/" + item.shop.Tel2);
                    dataRow.GetCell(10).SetCellValue(item.order.ChooseImg);
                    dataRow.GetCell(10).SetCellValue(item.subject.SubjectName);
                    dataRow.GetCell(11).SetCellValue(item.order.Gender);
                    dataRow.GetCell(12).SetCellValue(item.pop.Category);
                    dataRow.GetCell(13).SetCellValue(item.order.Sheet);
                    dataRow.GetCell(14).SetCellValue(double.Parse(item.order.Quantity.ToString()));
                    dataRow.GetCell(15).SetCellValue(item.order.GraphicMaterial);
                    dataRow.GetCell(16).SetCellValue(double.Parse(item.order.GraphicWidth.ToString()));
                    dataRow.GetCell(17).SetCellValue(double.Parse(item.order.GraphicLength.ToString()));
                    dataRow.GetCell(18).SetCellValue(double.Parse(item.order.Area.ToString()));
                    dataRow.GetCell(19).SetCellValue(item.order.PositionDescription);


                    startRow++;

                }

                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    
                    sheet = null;
                    
                    workBook = null;
                    
                    OperateFile.DownLoadFile(ms, "350总表");
                    //OperateFile.DownLoadFile(path);

                }
               
               
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "alert", "NoInfo()", true);
            }
        }

        protected void gvPOP_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objLevel = item.GetType().GetProperty("LevelNum").GetValue(item, null);
                    object objSheet = item.GetType().GetProperty("Sheet").GetValue(item, null);
                    if (objSheet != null && objSheet.ToString().Contains("桌"))
                    {
                        string level = objLevel != null ? objLevel.ToString() : "1";
                        ((Label)e.Item.FindControl("labLevel")).Text = CommonMethod.GeEnumName<TableLevelEnum>(level);
                    }

                }
            }
        }
    }
}
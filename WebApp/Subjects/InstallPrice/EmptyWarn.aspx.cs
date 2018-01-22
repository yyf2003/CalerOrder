using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Common;
using Models;
using System.Configuration;
using NPOI.SS.UserModel;
using System.Data;
using System.Data.OleDb;
using BLL;
using DAL;
using System.Text;

namespace WebApp.Subjects.InstallPrice
{
    public partial class EmptyWarn : System.Web.UI.Page
    {
        int guidanceId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId =int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["import"] != null)
            {
                int successNum = 0;
                int failNum = 0;
                if (Request.QueryString["successNum"] != null)
                {
                    successNum = int.Parse(Request.QueryString["successNum"]);
                }
                if (Request.QueryString["failNum"] != null)
                {
                    failNum = int.Parse(Request.QueryString["failNum"]);
                }
                if ((successNum + failNum) > 0)
                {
                    
                    updateResultTR.Visible = true;
                    labResult.Text = "导入成功！";
                    if (failNum > 0)
                    {
                        labResult.Text = "部分数据更新失败！";
                        lbDownLoadErrorMsg.Visible = true;
                    }

                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            if (Session["emptyOrder"] != null)
            {
                List<Shop> shopList = (List<Shop>)Session["emptyOrder"];
                //Session["emptyOrderShop"] = null;
                if (shopList.Any())
                {
                    shopList = shopList.OrderBy(s => s.Id).ToList();
                    string templateFileName = "ShopTemplate";
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
                        dataRow.GetCell(2).SetCellValue(item.POSScale);
                        dataRow.GetCell(3).SetCellValue(item.MaterialSupport);
                        dataRow.GetCell(4).SetCellValue(item.RegionName);
                        dataRow.GetCell(5).SetCellValue(item.ProvinceName);
                        dataRow.GetCell(6).SetCellValue(item.CityName);
                        dataRow.GetCell(7).SetCellValue(item.AreaName);
                        dataRow.GetCell(8).SetCellValue(item.CityTier);
                        dataRow.GetCell(9).SetCellValue(item.IsInstall);
                        dataRow.GetCell(10).SetCellValue(item.AgentCode);
                        dataRow.GetCell(11).SetCellValue(item.AgentName);
                        dataRow.GetCell(12).SetCellValue(item.Channel);
                        dataRow.GetCell(13).SetCellValue(item.Format);
                        dataRow.GetCell(14).SetCellValue(item.LocationType);
                        dataRow.GetCell(15).SetCellValue(item.BusinessModel);
                        dataRow.GetCell(16).SetCellValue(item.POPAddress);
                        dataRow.GetCell(17).SetCellValue(item.Contact1);
                        dataRow.GetCell(18).SetCellValue(item.Tel1);
                        dataRow.GetCell(19).SetCellValue(item.Contact2);
                        dataRow.GetCell(20).SetCellValue(item.Tel2);
                        if (item.OpeningDate != null)
                            dataRow.GetCell(21).SetCellValue(DateTime.Parse(item.OpeningDate.ToString()).ToShortDateString());
                        dataRow.GetCell(22).SetCellValue(item.Status);

                        startRow++;

                    }



                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();
                        sheet = null;
                        workBook = null;
                        OperateFile.DownLoadFile(ms, "店铺信息");

                    }
                }
            }
        }

        protected void lbDownLoadErrorMsg_Click(object sender, EventArgs e)
        {

        }
        ShopBLL shopBll = new ShopBLL();
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                OleDbConnection conn;
                OleDbDataAdapter da;
                DataSet ds = null;
                path = Server.MapPath(path);
                ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                conn = new OleDbConnection(ExcelConnStr);
                conn.Open();
                string sql = "select * from [Sheet1$]";
                da = new OleDbDataAdapter(sql, conn);
                ds = new DataSet();
                da.Fill(ds);
                da.Dispose();
                int successNum = 0;
                int failNum = 0;
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataColumnCollection cols = ds.Tables[0].Columns;
                    DataTable errorTB = Common.CommonMethod.CreateErrorTB(cols);
                    int shopId = 0;
                   
                    string shopNo = string.Empty;
                    
                    string posScale = string.Empty;
                    string materialSupport = string.Empty;
                    Shop shopModel;
                    
                    FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                    FinalOrderDetailTemp orderModel;
                    
                    List<FinalOrderDetailTemp> emptyDataOrderList = new List<FinalOrderDetailTemp>();
                    if (guidanceId>0)
                    {
                        
                        emptyDataOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                              join subject in CurrentContext.DbContext.Subject
                                              on order.SubjectId equals subject.Id
                                              join shop in CurrentContext.DbContext.Shop
                                              on order.ShopId equals shop.Id
                                              join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                              on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                                              from subjectCategory in categoryTemp.DefaultIfEmpty()
                                              where subject.GuidanceId == guidanceId
                                              && shop.IsInstall!=null && shop.IsInstall=="Y"
                                              //&& ((shop.Format != null && shop.Format != "") ? (shop.Format.ToLower().IndexOf("kids") == -1 && shop.Format.ToLower().IndexOf("infant") == -1) : 1 == 1)
                                              && ((order.InstallPricePOSScale == null || order.InstallPricePOSScale == "") || (order.InstallPriceMaterialSupport == null || order.InstallPriceMaterialSupport == ""))
                                              && ((subjectCategory != null) ? (subjectCategory.CategoryName != "童店" && subjectCategory.CategoryName != "三叶草") : 1 == 1)
                                              select order).ToList();
                    }
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        shopId = 0;
                        
                        shopNo = string.Empty;
                        posScale = string.Empty;
                        materialSupport = string.Empty;
                        

                        bool canSave = true;
                        StringBuilder errorMsg = new StringBuilder();

                        if (cols.Contains("POS Code"))
                            shopNo = dr["POS Code"].ToString().Trim();
                        else if (cols.Contains("POSCode"))
                            shopNo = dr["POSCode"].ToString().Trim();
                        else if (cols.Contains("店铺编号"))
                            shopNo = dr["店铺编号"].ToString().Trim();

                        

                        if (cols.Contains("店铺大小"))
                            posScale = dr["店铺大小"].ToString().Trim();
                        else if (cols.Contains("店铺规模大小"))
                            posScale = dr["店铺规模大小"].ToString().Trim();
                        else if (cols.Contains("店铺规模"))
                            posScale = dr["店铺规模"].ToString().Trim();

                        if (cols.Contains("店铺级别"))
                            materialSupport = dr["店铺级别"].ToString().Trim();
                        else if (cols.Contains("物料级别"))
                            materialSupport = dr["物料级别"].ToString().Trim();
                        else if (cols.Contains("物料支持级别"))
                            materialSupport = dr["物料支持级别"].ToString().Trim();
                        else if (cols.Contains("物料支持"))
                            materialSupport = dr["物料支持"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(shopNo))
                        {
                            canSave = false;
                            errorMsg.Append("店铺编号为空 ；");
                        }
                        if (canSave)
                        {
                            CheckShop(shopNo, out shopId);
                            shopModel = new Models.Shop();
                            shopModel = shopBll.GetModel(shopId);
                            if (shopModel != null)
                            {
                                if (!string.IsNullOrWhiteSpace(materialSupport) || !string.IsNullOrWhiteSpace(posScale))
                                {
                                    
                                    if (emptyDataOrderList.Any())
                                    {
                                        var shopOrderList = emptyDataOrderList.Where(order => order.ShopId == shopId).ToList();
                                        if (shopOrderList.Any())
                                        {
                                            shopOrderList.ForEach(order =>
                                            {
                                                orderModel = new FinalOrderDetailTemp();
                                                orderModel = order;
                                                if (string.IsNullOrWhiteSpace(orderModel.POSScale))
                                                {
                                                    orderModel.POSScale = posScale;
                                                }
                                                if (string.IsNullOrWhiteSpace(orderModel.InstallPricePOSScale))
                                                {
                                                    orderModel.InstallPricePOSScale = posScale;
                                                }
                                                if (string.IsNullOrWhiteSpace(orderModel.MaterialSupport))
                                                {
                                                    orderModel.MaterialSupport = materialSupport;
                                                    
                                                }
                                                if (string.IsNullOrWhiteSpace(orderModel.InstallPriceMaterialSupport))
                                                {
                                                    
                                                    orderModel.InstallPriceMaterialSupport = materialSupport;
                                                }
                                                orderBll.Update(orderModel);
                                                successNum++;
                                            });
                                        }
                                    }
                                    //shopBll.Update(shopModel);
                                    //successNum++;
                                }


                            }
                            
                        }
                        else
                        {
                            DataRow dr1 = errorTB.NewRow();
                            for (int i = 0; i < cols.Count; i++)
                            {
                                dr1["" + cols[i] + ""] = dr[cols[i]];
                            }
                            dr1["错误信息"] = errorMsg.ToString();
                            errorTB.Rows.Add(dr1);
                            failNum++;
                        }
                    }
                    if (errorTB.Rows.Count > 0)
                    {
                        Session["updatErrorTb"] = errorTB;
                    }
                    else
                        Session["updatErrorTb"] = null;
                }
                conn.Dispose();
                conn.Close();
                Response.Redirect(string.Format("EmptyWarn.aspx?import=1&guidanceId={0}&successNum={1}&failNum={2}", guidanceId, successNum, failNum), false);
            }
        }

        bool CheckShop(string shopNo, out int shopId)
        {
            shopId = 0;
            var list = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper());
            if (list.Any())
            {
                shopId = list[0].Id;
                return true;
            }
            return false;
        }
    }
}
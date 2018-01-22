using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using BLL;
using Common;
using DAL;
using Models;
using System.Configuration;
using System.Text;

namespace WebApp.Subjects.SecondInstallFee
{
    public partial class ImportOrder : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["import"] != null)
            {
                
                Panel1.Visible = true;
                if (Session["failMsg"] != null)
                {
                    labState.Text = "部分数据导入失败！";
                    ExportFailMsg.Style.Add("display", "block");
                }
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindDateil();
            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                where subject.Id == subjectId
                                select new
                                {
                                    CustomerId = subject.CustomerId ?? 0,
                                    subject.SubjectName,
                                    subject.SubjectNo,
                                    customer.CustomerName
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
               
                labSubjectName.Text = subjectModel.SubjectName;
                labSubjectNo.Text = subjectModel.SubjectNo;
                labCustomer.Text = subjectModel.CustomerName;

            }
        }

        void BindDateil()
        {
            var list = (from detail in CurrentContext.DbContext.SecondInstallFeeDetail
                        join shop in CurrentContext.DbContext.Shop
                        on detail.ShopId equals shop.Id
                        where detail.SubjectId == subjectId
                        select new
                        {
                            shop,
                            detail
                        }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            repeater_PriceList.DataSource = list.OrderBy(s => s.detail.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            repeater_PriceList.DataBind();
            Panel2.Visible = list.Any();
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (Session["failMsg"] != null)
            {
                Session["failMsg"] = null;
            }
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {

                    OleDbConnection conn;
                    OleDbDataAdapter da;
                    DataSet ds = null;
                    path = Server.MapPath(path);
                    string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                    ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                    conn = new OleDbConnection(ExcelConnStr);
                    conn.Open();
                    string sql = "select * from [Sheet1$]";
                    da = new OleDbDataAdapter(sql, conn);
                    ds = new DataSet();
                    da.Fill(ds);
                    da.Dispose();

                    SecondInstallFeeDetailBLL detailBll = new SecondInstallFeeDetailBLL();
                    detailBll.Delete(s => s.SubjectId == subjectId);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        DataTable errorTB = CommonMethod.CreateErrorTB(cols);
                        int shopId = 0;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringBuilder msg = new StringBuilder();
                            bool canSave = true;
                            int IsInvalid = 0;
                            string shopNo = string.Empty;

                            string sheet = string.Empty;
                            string width = string.Empty;
                            string length = string.Empty;
                            string quantity = string.Empty;
                            string remark = string.Empty;
                            if (cols.Contains("店铺编号"))
                                shopNo = dr["店铺编号"].ToString();
                            else if (cols.Contains("POSCode"))
                                shopNo = dr["POSCode"].ToString();
                            else if (cols.Contains("POS Code"))
                                shopNo = dr["POS Code"].ToString();

                            if (cols.Contains("POP位置"))
                                sheet = dr["POP位置"].ToString();
                            else if (cols.Contains("pop位置"))
                                sheet = dr["pop位置"].ToString();
                            else if (cols.Contains("位置"))
                                sheet = dr["位置"].ToString();
                            else if (cols.Contains("Sheet"))
                                sheet = dr["Sheet"].ToString();
                            else if (cols.Contains("sheet"))
                                sheet = dr["sheet"].ToString();

                            if (cols.Contains("POP宽"))
                                width = dr["POP宽"].ToString();
                            else if (cols.Contains("pop宽"))
                                width = dr["pop宽"].ToString();
                            else if (cols.Contains("宽"))
                                width = dr["宽"].ToString();

                            if (cols.Contains("POP高"))
                                length = dr["POP高"].ToString();
                            else if (cols.Contains("pop高"))
                                length = dr["pop高"].ToString();
                            else if (cols.Contains("高"))
                                length = dr["高"].ToString();

                            if (cols.Contains("数量"))
                                quantity = dr["数量"].ToString();
                            if (cols.Contains("备注"))
                                remark = dr["备注"].ToString();
                            if (string.IsNullOrWhiteSpace(shopNo))
                            {
                                canSave = false;
                                msg.Append("店铺编号 为空；");
                            }
                            else
                                if (!CheckShop(shopNo, out shopId, out IsInvalid))
                                {
                                    canSave = false;
                                    if (shopId > 0)
                                        msg.Append("店铺已经关闭；");
                                    else
                                        msg.Append("店铺编号不存在；");
                                }
                            
                            if (canSave)
                            {
                                SecondInstallFeeDetail detail = new SecondInstallFeeDetail();
                                detail.GraphicLength = StringHelper.IsDecimal(length);
                                detail.GraphicWidth = StringHelper.IsDecimal(width);
                                detail.Quantity = StringHelper.IsInt(quantity);
                                detail.Remark = remark;
                                detail.Sheet = sheet;
                                detail.ShopId = shopId;
                                detail.SubjectId = subjectId;
                                detailBll.Add(detail);
                            }
                            if (msg.Length > 0)
                            {
                                DataRow dr1 = errorTB.NewRow();
                                for (int ii = 0; ii < cols.Count; ii++)
                                {
                                    dr1["" + cols[ii] + ""] = dr[ii].ToString();
                                }
                                dr1["错误信息"] = msg.ToString();
                                errorTB.Rows.Add(dr1);
                            }
                        }
                        if (errorTB.Rows.Count > 0)
                        {
                            Session["failMsg"] = errorTB;
                        }
                        Response.Redirect("ImportOrder.aspx?import=1&subjectId=" + subjectId, false);
                    }
                }
            }
        }



        ShopBLL shopBll = new ShopBLL();
        Dictionary<string, int> shopNoList = new Dictionary<string, int>();
        List<string> CloseShopList = new List<string>();
        Dictionary<string, int> InvalidShops = new Dictionary<string, int>();
        bool CheckShop(string shopNo, out int shopId, out int IsInvalid)
        {
            shopId = 0;
            IsInvalid = 0;
            bool flag = true;
            if (CloseShopList.Contains(shopNo.ToUpper()))
            {
                flag = false;
                shopId = 1;
            }

            else
            {
                if (shopNoList.Keys.Contains(shopNo.ToUpper()))
                {
                    shopId = shopNoList[shopNo.ToUpper()];
                    IsInvalid = InvalidShops[shopNo.ToUpper()];
                }
                else
                {
                    var shop = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).FirstOrDefault();
                    if (shop != null)
                    {
                        shopId = shop.Id;
                        IsInvalid = shop.IsInvalid ?? 0;
                        if (!string.IsNullOrWhiteSpace(shop.Status) && (shop.Status.Contains("关") || shop.Status.Contains("闭")))
                        {

                            CloseShopList.Add(shopNo.ToUpper());
                            flag = false;
                        }
                        else
                        {
                            if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                                shopNoList.Add(shopNo.ToUpper(), shopId);
                            if (!InvalidShops.Keys.Contains(shopNo.ToUpper()))
                                InvalidShops.Add(shopNo.ToUpper(), IsInvalid);
                        }

                    }
                    else
                        flag = false;
                }
            }
            return flag;
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindDateil();
        }

        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "二次安装费明细模板";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void lbExportError_Click(object sender, EventArgs e)
        {
            if (Session["failMsg"] != null)
            {
               
                Dictionary<string, DataTable> dts = (Dictionary<string, DataTable>)Session["failMsg"];
                OperateFile.ExportTables(dts, "导入失败信息");
            }
            
        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("List.aspx", false);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Add.aspx?subjectId=" + subjectId, false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using BLL;
using Common;
using Models;

namespace WebApp.Quotation
{
    public partial class Import : BasePage
    {
        
        int SubjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectid"] != null)
            {
                SubjectId = int.Parse(Request.QueryString["subjectid"]);
            }

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
                ImportState.Style.Add("display", "block");
                labState.Text = "导入成功！";
                labTotalNum.Text = (successNum + failNum) + "条";
                labSuccessNum.Text = successNum + "条";
                labFailNum.Text = failNum + "条";
                if (failNum > 0)
                {
                    labState.Text = "部分数据导入失败！";
                    failMsgDiv.Style.Add("display", "block");
                    labFailMsg.Visible = true;
                    lbExportErrorMsg.Visible = true;
                }
                if (successNum > 0)
                    ExcuteJs("Finish");
            }
             
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        QuotationsBLL quotationBll = new QuotationsBLL();
        ADSubjectPriceBLL priceBll = new ADSubjectPriceBLL();
        protected void btnImport_Click(object sender, EventArgs e)
        {
             int successNum = 0;
            int failNum = 0;
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
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
                    conn.Dispose();
                    conn.Close();
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        DataTable ErrorTB = CommonMethod.CreateErrorTB(cols);
                        string CRNumber = string.Empty;
                        string AccountCheckDate = string.Empty;
                        string QuotationsDate = string.Empty;
                        string SubjectName = string.Empty;
                        string Category = string.Empty;
                        string Blongs = string.Empty;
                        string Classification = string.Empty;
                        string AdidasContact = string.Empty;
                        string ProjectContact = string.Empty;
                        string TaxRate = string.Empty;
                        string OfferPrice = string.Empty;
                        string Account = string.Empty;
                        string Invoices = string.Empty;
                        string Remark = string.Empty;
                        Quotations quotationModel;
                        ADSubjectPrice priceModel;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringBuilder errorMsg = new StringBuilder();
                            bool canSave = true;
                            
                            //CRNumber = dr["CR号"].ToString();
                            //AccountCheckDate = dr["AD对账日期"].ToString();
                            //QuotationsDate = dr["报价日期"].ToString();
                            //SubjectName = dr["活动中文名称"].ToString();
                            Category = dr["类别"].ToString();
                            Blongs = dr["AD费用归属"].ToString();
                            Classification = dr["分类"].ToString();
                            AdidasContact = dr["Adidas负责人"].ToString();
                            //ProjectContact = dr["立项负责人"].ToString();
                            TaxRate = dr["税率"].ToString();
                            OfferPrice = dr["报价金额"].ToString();
                            Account = dr["报价账户"].ToString();
                            //Invoices = dr["发票类型"].ToString();
                            Remark = dr["备注"].ToString();
                            if (string.IsNullOrWhiteSpace(Category))
                            {
                                canSave = false;
                                errorMsg.Append("类别 为空");
                            }
                            if (string.IsNullOrWhiteSpace(OfferPrice))
                            {
                                canSave = false;
                                errorMsg.Append("报价金额 为空；");
                            }
                            else if (!StringHelper.IsDecimalVal(OfferPrice))
                            {
                                canSave = false;
                                errorMsg.Append("报价金额 必须为数字；");
                            }
                            if (canSave)
                            {
                                quotationModel = new Quotations();
                                quotationModel.Account = Account;
                                //if (!string.IsNullOrWhiteSpace(AccountCheckDate) && !StringHelper.IsDateTime(AccountCheckDate))
                                    //quotationModel.AccountCheckDate = DateTime.Parse(AccountCheckDate);
                                quotationModel.AddDate = DateTime.Now;
                                quotationModel.AddUserId = CurrentUser.UserId;
                                quotationModel.AdidasContact = AdidasContact;
                                quotationModel.Blongs = Blongs;
                                quotationModel.Category = Category;
                                quotationModel.Classification = Classification;
                                //quotationModel.CRNumber = CRNumber;
                                //quotationModel.Invoices = Invoices;
                                quotationModel.OfferPrice = StringHelper.IsDecimal(OfferPrice);
                                //quotationModel.ProjectContact = ProjectContact;
                                //if (!string.IsNullOrWhiteSpace(QuotationsDate) && !StringHelper.IsDateTime(QuotationsDate))
                                    //quotationModel.QuotationsDate = DateTime.Parse(QuotationsDate);
                                quotationModel.Remark = Remark;
                                quotationModel.SubjectId = SubjectId;
                                quotationModel.TaxRate = TaxRate;
                                quotationBll.Add(quotationModel);
                                for (int i = 8; i < cols.Count; i++)
                                {
                                    if (cols[i] != null)
                                    {
                                        string priceName = cols[i].ColumnName;
                                        string price = dr["" + priceName + ""].ToString();
                                        if (!string.IsNullOrWhiteSpace(price) && StringHelper.IsDecimalVal(price))
                                        {
                                            int priceType = 1;
                                            if (priceName.IndexOf("分摊金额") != -1)
                                            {
                                                priceName = priceName.Replace("分摊金额-", "").Replace("分摊金额", "");
                                            }
                                            if (priceName.IndexOf("合并金额") != -1)
                                            {
                                                priceName = priceName.Replace("合并金额-", "").Replace("合并金额", "");
                                                priceType = 2;
                                            }
                                            priceModel = new ADSubjectPrice();
                                            priceModel.Price = StringHelper.IsDecimal(price);
                                            priceModel.PriceName = priceName;
                                            priceModel.PriceType = priceType;
                                            priceModel.QuotationsId = quotationModel.Id;
                                            priceBll.Add(priceModel);
                                        }
                                    }
                                }
                                successNum++;
                            }
                            else
                            {
                                DataRow dr1 = ErrorTB.NewRow();
                                for (int i = 0; i < cols.Count; i++)
                                {
                                    dr1["" + cols[i] + ""] = dr[cols[i]];
                                }
                                dr1["错误信息"] = errorMsg.ToString();
                                ErrorTB.Rows.Add(dr1);
                                failNum++;
                            }
                        }
                        if (ErrorTB.Rows.Count > 0)
                        {
                            Session["MaterialErrorTb"] = ErrorTB;
                        }
                        Response.Redirect(string.Format("Import.aspx?successNum={0}&failNum={1}", successNum, failNum), false);
                    }
                }
            }
        }

        

        /// <summary>
        /// 导出失败信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportErrorMsg_Click(object sender, EventArgs e)
        {
            if (Session["MaterialErrorTb"] != null)
            {
                DataTable dt = (DataTable)Session["MaterialErrorTb"];
                OperateFile.ExportExcel(dt, "项目报价导入失败信息");
            }
        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoadShop_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "ImportQuotationsTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }
    }
}
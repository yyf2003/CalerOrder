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

namespace WebApp.Subjects.Material
{
    public partial class Import  : BasePage
    {
        int ItemId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemid"] != null)
            {
                ItemId = int.Parse(Request.QueryString["itemid"].ToString());
            }
            if (Request.QueryString["import"] != null)
            {
                Panel1.Visible = true;
                if (Session["fail"] != null)
                {
                    labState.Text = "部分数据导入失败！";
                    ExportFailMsg.Style.Add("display", "block");
                }
                int successNum1 = 0;
                if (Request.QueryString["successNum"] != null)
                {
                    successNum1 = int.Parse(Request.QueryString["successNum"]);
                }
                if (successNum1>0)
                    ExcuteJs("Finish");
            }
        }

        protected void lbExportErrorMsg_Click(object sender, EventArgs e)
        {

        }

        int successNum;
        OrderMaterialBLL materialBll = new OrderMaterialBLL();
        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile && ItemId>0)
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
                        if (!cbAdd.Checked)
                            materialBll.Delete(s => s.ItemId == ItemId);


                        DataColumnCollection cols = ds.Tables[0].Columns;
                        DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                        OrderMaterial model;
                        int shopId = 0;
                        DataTable dt1 = ds.Tables[0];
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            bool canSave = true;
                            StringBuilder msg = new StringBuilder();
                            string shopNo = string.Empty;
                            
                            if (cols.Contains("POSCode"))
                            {
                                shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["POSCode"].ToString());
                            }
                            else if (cols.Contains("POS Code"))
                            {
                                shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["POS Code"].ToString());
                            }
                            else if (cols.Contains("店铺编号"))
                            {
                                shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["店铺编号"].ToString());
                            }
                            //if (cols.Contains("销售价格"))
                            //{
                            //    price = dt1.Rows[i]["销售价格"].ToString();
                            //}
                            //else if (cols.Contains("价格"))
                            //{
                            //    price = dt1.Rows[i]["价格"].ToString();
                            //}
                            if (string.IsNullOrWhiteSpace(shopNo))
                            {
                                canSave = false;
                                msg.Append("店铺编号 为空；");
                            }
                            if (!string.IsNullOrWhiteSpace(shopNo) && !CheckShop(shopNo, out shopId))
                            {
                                canSave = false;
                                msg.Append("店铺编号不存在；");
                            }
                            
                            
                            if (canSave)
                            {
                                string materialName = string.Empty;
                                string count = string.Empty;
                                string price = string.Empty;
                                for (int col = 1; col < cols.Count; col++)
                                {
                                    if (cols[col] != null)
                                    {
                                        bool canGo = true;
                                        price = string.Empty;
                                        materialName = cols[col].ToString();
                                        count = dt1.Rows[i][col].ToString();
                                        count = (count.IndexOf("无") != -1 || count.IndexOf("空") != -1) ? "0" : count;
                                        count = string.IsNullOrWhiteSpace(count) ? "0" : count;
                                        if (!StringHelper.IsIntVal(count))
                                        {
                                            canGo = false;
                                            msg.Append(materialName+"的数量填写不正确；");
                                        }
                                        if (col < cols.Count - 1 && cols[col + 1].ToString().Contains("销售价格"))
                                        {
                                            price = dt1.Rows[i][col + 1].ToString();
                                            col += 1;

                                        }
                                        else if (col < cols.Count - 1 && cols[col + 1].ToString().Contains("价格"))
                                        {
                                            price = dt1.Rows[i][col + 1].ToString();
                                            col += 1;

                                        }
                                        if (!string.IsNullOrWhiteSpace(materialName) && count != "0")
                                        {
                                            if (!string.IsNullOrWhiteSpace(price) && !StringHelper.IsDecimalVal(price))
                                            {
                                                canGo = false;
                                                msg.Append(materialName+"的价格填写不正确；");
                                            }
                                            if (canGo)
                                            {
                                                model = new OrderMaterial();
                                                model.AddDate = DateTime.Now;
                                                model.ItemId = ItemId;
                                                model.MaterialCount = int.Parse(count != "" ? count : "0");
                                                model.MaterialName = materialName;
                                                if (!string.IsNullOrWhiteSpace(price))
                                                    model.Price = decimal.Parse(price);
                                                model.ShopId = shopId;
                                                materialBll.Add(model);
                                                successNum++;
                                            }
                                        }
                                    }
                                }
                            }
                            //
                            if (msg.Length > 0)
                            {
                                DataRow dr1 = errorTB.NewRow();
                                for (int ii = 0; ii < cols.Count; ii++)
                                {
                                    dr1["" + cols[ii] + ""] = dt1.Rows[i][ii].ToString();
                                }
                                dr1["错误信息"] = msg.ToString();
                                errorTB.Rows.Add(dr1);
                            }
                        }
                        if (errorTB != null && errorTB.Rows.Count > 0)
                        {
                            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
                            dic.Add("Material", errorTB);
                            Session["fail"] = dic;
                        }
                        else
                            Session["fail"] = null
                                ;
                        Response.Redirect("Import.aspx?import=1&itemid=" + ItemId + "&successNum=" + successNum, false);
                    }
                    else
                    {
                        Panel1.Visible = true;
                        labState.Text = "导入失败：导入文件里没有有效数据";
                        return;
                    }
                }
            }
        }

        ShopBLL shopBll = new ShopBLL();
        Dictionary<string, int> shopNoList = new Dictionary<string, int>();
        bool CheckShop(string shopNo, out int shopId)
        {
            shopId = 0;
            if (shopNoList.Keys.Contains(shopNo))
            {
                shopId = shopNoList[shopNo];
            }
            else
            {
                var shop = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).FirstOrDefault();
                if (shop != null)
                {
                    shopId = shop.Id;
                    shopNoList.Add(shopNo, shopId);
                }
            }
            return shopId > 0;
        }




        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "ADOrderMaterialTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void lbExportError_Click(object sender, EventArgs e)
        {
            if (Session["fail"] != null)
            {
                
                Dictionary<string, DataTable> dts = (Dictionary<string, DataTable>)Session["fail"];
                OperateFile.ExportTables(dts, "导入失败信息");
            }
        }
    }
}
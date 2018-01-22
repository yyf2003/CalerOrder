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

namespace WebApp.Customer
{
    public partial class ImportHCPOP : BasePage
    {
        int customerId = 0;
        HCPOPBLL bll = new HCPOPBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            int successNum = 0;
            int failNum = 0;
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);
            }
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

        protected void btnImport_Click(object sender, EventArgs e)
        {
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
                        DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                        int shopId = 0;
                        HCPOP model;
                        int successNum=0;
                        int failNum = 0;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            bool canSave = true;
                            StringBuilder errorMsg = new StringBuilder();
                            string shopNo = string.Empty;
                            if (cols.Contains("店铺编号"))
                                shopNo = dr["店铺编号"].ToString().Trim();
                            else if (cols.Contains("POS Code"))
                                shopNo = dr["POS Code"].ToString().Trim();
                            else if (cols.Contains("POSCode"))
                                shopNo = dr["POSCode"].ToString().Trim();

                            string  frameName = dr["器架名称"].ToString().Trim();
                            string popName=dr["POP名称"].ToString().Trim();
                            string gender = dr["性别"].ToString().Trim();
                            string count = dr["数量"].ToString().Trim();
                            string material = dr["材质"].ToString().Trim();
                            string width = dr["POP宽"].ToString().Trim();
                            string length = dr["POP高"].ToString().Trim();
                            if (string.IsNullOrWhiteSpace(shopNo))
                            {
                                canSave = false;
                                errorMsg.Append("店铺编号 为空；");
                            }
                            else if (!CheckShop(shopNo, out shopId))
                            {
                                canSave = false;
                                errorMsg.Append("店铺编号 不存在；");
                            }
                            if (string.IsNullOrWhiteSpace(frameName))
                            {
                                canSave = false;
                                errorMsg.Append("器架名称 为空；");
                            }
                            else if (!CheckFrame(shopId, frameName))
                            {
                                canSave = false;
                                errorMsg.Append("该店铺不没有这个器架；");
                            }

                            if (string.IsNullOrWhiteSpace(popName))
                            {
                                canSave = false;
                                errorMsg.Append("POP名称 为空；");
                            }
                            if (string.IsNullOrWhiteSpace(gender))
                            {
                                canSave = false;
                                errorMsg.Append("性别 为空；");
                            }
                            
                            if (string.IsNullOrWhiteSpace(count))
                            {

                                count = "1";
                            }
                            if (StringHelper.IsInt(count) == 0)
                            {

                                count = "1";
                            }
                            if (string.IsNullOrWhiteSpace(width))
                            {
                                canSave = false;
                                errorMsg.Append("POP宽 为空；");
                            }
                            else if (!StringHelper.IsDecimalVal(width))
                            {
                                canSave = false;
                                errorMsg.Append("POP宽填写不正确；");
                            }
                            if (string.IsNullOrWhiteSpace(length))
                            {
                                canSave = false;
                                errorMsg.Append("POP高 为空；");
                            }
                            else if (!StringHelper.IsDecimalVal(length))
                            {
                                canSave = false;
                                errorMsg.Append("POP高填写不正确；");
                            }
                            if (canSave)
                            {
                                int id = 0;
                                if (CheckIsExist(shopId, frameName,popName, gender, out id))
                                {
                                    model = bll.GetModel(id);
                                    if (model != null)
                                    {
                                        model.Count = int.Parse(count);
                                        model.GraphicLength = decimal.Parse(length);
                                        model.GraphicWidth = decimal.Parse(width);
                                        model.GraphicMaterial = material;
                                        
                                        bll.Update(model);
                                    }
                                }
                                else
                                {
                                    model = new HCPOP() { Count = int.Parse(count),POPGender = gender, MachineFrame = frameName.ToUpper(), POP = popName.ToUpper(), ShopId = shopId,GraphicMaterial=material,GraphicLength=decimal.Parse(length),GraphicWidth=decimal.Parse(width) };
                                    bll.Add(model);
                                }

                                successNum++;
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
                            Session["errorTb"] = errorTB;
                        }
                        Response.Redirect(string.Format("ImportHCPOP.aspx?successNum={0}&failNum={1}&customerId={2}", successNum, failNum, customerId), false);
                    
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否已存在，不存在就新增，存在就更新
        /// </summary>
        /// <param name="shopName"></param>
        /// <returns></returns>
        /// 
        List<Shop> shopList = new List<Shop>();
        bool CheckShop(string shopNo, out int shopId)
        {
            shopId = 0;

            if (!shopList.Any())
            {
                shopList = new ShopBLL().GetList(s => s.CustomerId == customerId);
            }
            var list = shopList.Where(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).ToList();
            if (list.Any())
            {
                shopId = list[0].Id;
                return true;
            }
            return false;
        }

        List<ShopMachineFrame> frameList = new List<ShopMachineFrame>();
        bool CheckFrame(int shopId, string frameName)
        {
            if (!frameList.Any())
            {
                frameList = new ShopMachineFrameBLL().GetList(s=>s.ShopId==shopId);

            }
            var model = frameList.FirstOrDefault(s => s.MachineFrame.ToUpper() == frameName.ToUpper() && s.PositionName == "鞋墙");
            return model != null;
        }

        bool CheckIsExist(int shopId, string frame, string popName, string gender,out int id)
        {
            id = 0;
            var list = bll.GetList(s => s.ShopId == shopId && s.MachineFrame.ToUpper() == frame.ToUpper() && s.POP.ToUpper() == popName.ToUpper() && s.POPGender == gender);
            if (list.Any())
            {
                id = list[0].Id;
                return true;
            }
            else
                return false;
        }

        protected void lbExportErrorMsg_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }

        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "HCPOPTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }
    }
}
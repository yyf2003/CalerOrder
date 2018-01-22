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
    public partial class ImportFrameBlackList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["isImport"] != null && Request.QueryString["isImport"] == "1")
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
                    ImportState.Style.Add("display", "block");
                    labState.Text = "导入成功！";
                    labTotalNum.Text = (successNum + failNum) + "条";
                    labSuccessNum.Text = successNum + "条";
                    labFailNum.Text = failNum + "条";
                    if (failNum > 0)
                    {
                        labState.Text = "部分数据导入失败！";
                        failMsgDiv.Style.Add("display", "block");
                        lbExportErrorMsg.Visible = true;
                    }
                    if (successNum > 0)
                        ExcuteJs("Finish");
                }
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
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        int successNum = 0;
                        int failNum = 0;
                        MachineFrameBlackListBLL listBll = new MachineFrameBlackListBLL();
                        MachineFrameBlackList listModel;
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        DataTable errorTB = Common.CommonMethod.CreateErrorTB(cols);
                        int shopId = 0;
                        string shopNo = string.Empty;
                        string sheet = string.Empty;
                        string gender = string.Empty;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                             shopId = 0;
                             shopNo = string.Empty;
                             sheet = string.Empty;
                             gender = string.Empty;
                             bool canSave = true;
                             StringBuilder errorMsg = new StringBuilder();
                             if (cols.Contains("POS Code"))
                                 shopNo = dr["POS Code"].ToString().Trim();
                             else if (cols.Contains("POSCode"))
                                 shopNo = dr["POSCode"].ToString().Trim();
                             else if (cols.Contains("店铺编号"))
                                 shopNo = dr["店铺编号"].ToString().Trim();
                             if (cols.Contains("Sheet"))
                                 sheet = dr["Sheet"].ToString().Trim();
                             else if (cols.Contains("POP位置"))
                                 sheet = dr["POP位置"].ToString().Trim();
                             else if (cols.Contains("pop位置"))
                                 sheet = dr["pop位置"].ToString().Trim();

                            if (cols.Contains("性别"))
                                gender = dr["性别"].ToString().Trim();
                             else if (cols.Contains("男/女"))
                                gender = dr["男/女"].ToString().Trim();
                            else if (cols.Contains("M/W"))
                                gender = dr["M/W"].ToString().Trim();

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
                            if (string.IsNullOrWhiteSpace(sheet))
                            {
                                canSave = false;
                                errorMsg.Append("POP位置 为空；");
                            }
                            if (canSave)
                            {
                                var list = listBll.GetList(s=>s.ShopId==shopId && s.Sheet==sheet.ToUpper());
                                if (list.Any())
                                {
                                    var list0 = list.Where(s => s.Gender == gender || (s.Gender == null || s.Gender == "") || (s.Gender.Contains("男") && s.Gender.Contains("女"))).ToList();
                                    if (list0.Any())
                                    {
                                        canSave = false;
                                        errorMsg.Append("已存在；");
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(gender) || (gender.Contains("男") && gender.Contains("女")))
                                        {
                                            list = list.Where(s => s.Gender != null && (s.Gender == "男" || s.Gender == "女")).ToList();
                                            if (list.Any())
                                            {
                                                List<int> idList = list.Select(s => s.Id).ToList();
                                                listBll.Delete(s => idList.Contains(s.Id));
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            if (canSave)
                            {
                                listModel = new MachineFrameBlackList();
                                listModel.Gender = gender;
                                listModel.Sheet = sheet;
                                listModel.ShopId = shopId;
                                listModel.ShopNo = shopNo;
                                listBll.Add(listModel);
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
                        else
                        {
                            Session["errorTb"] = null;
                        }
                        conn.Dispose();
                        conn.Close();
                        Response.Redirect(string.Format("ImportFrameBlackList.aspx?isImport=1&successNum={0}&failNum={1}", successNum, failNum), false);
                    }
                }
            }
        }

        protected void lbExportErrorMsg_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }


        ShopBLL shopBll = new ShopBLL();
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
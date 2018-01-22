using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using BLL;
using Common;
using DAL;
using Models;
using System.Web;


namespace WebApp.Subjects.CheckDataBase
{
    public partial class CheckData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Request.QueryString["isImport"] != null && Request.QueryString["isImport"] == "1")
            {
                ImportState.Style.Add("display", "block");
                labState.Text = "检查完成！";
                string importType = string.Empty;
                if (Request.QueryString["fileType"] != null)
                {
                    importType = Request.QueryString["fileType"];
                    rblType.SelectedValue = importType;
                }
                if (Request.QueryString["failNum"] != null)
                {
                    int fail = int.Parse(Request.QueryString["failNum"]);

                    if (fail > 0)
                    {
                        labResult.Text = "部分数据有问题！";
                        failMsgDiv.Style.Add("display", "block");
                        lbExportErrorMsg.Visible = true;
                    }
                    else
                    {
                        labResult.Text = "一切正常！";
                    }
                }
            }
        }

        int failNum = 0;
        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (Session["shopErrorTb"] != null)
                Session["shopErrorTb"] = null;
            if (Session["POPErrorTb"] != null)
                Session["POPErrorTb"] = null;
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
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        string fileType = rblType.SelectedValue;
                        switch (fileType)
                        {
                            case "1":
                                CheckShop(ds);
                                break;
                            case "2":
                                CheckPOP(ds);
                                break;
                            case "3":

                                break;

                        }

                        conn.Dispose();
                        conn.Close();
                        Response.Redirect(string.Format("CheckData.aspx?isImport=1&fileType={0}&failNum={1}", fileType,failNum), false);
                    }
                    else
                    {
                        ImportState.Style.Add("display", "block");
                        labState.Text = "检查失败：";
                        labResult.Text = "导入文件里面没有数据！";
                        failMsgDiv.Style.Add("display", "none");
                        lbExportErrorMsg.Visible = false;
                    }
                }
            }
        }

        DataTable errorTB = null;
        //DataTable POPErrorTB = null;
        //DataTable FrameErrorTB = null;
        void CheckShop(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                string shopNo = string.Empty;
                DataColumnCollection cols = ds.Tables[0].Columns;
                errorTB = CommonMethod.CreateErrorTB(cols);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    shopNo = string.Empty;
                    StringBuilder msg = new StringBuilder();
                    if (cols.Contains("POSCode"))
                    {
                        shopNo = dr["POSCode"].ToString().Trim();
                    }
                    else if (cols.Contains("POS Code"))
                    {
                        shopNo = dr["POS Code"].ToString().Trim();
                    }
                    else if (cols.Contains("店铺编号"))
                    {
                        shopNo = dr["店铺编号"].ToString().Trim();
                    }
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        msg.Append("店铺编号没填写；");
                    }
                    else
                    {
                        Shop shopModel = null;
                        if (GetShopFromDB(shopNo, ref shopModel))
                        {
                            if (shopModel != null)
                            {
                                if (!string.IsNullOrWhiteSpace(shopModel.Format))
                                {
                                    string format = shopModel.Format.ToUpper();
                                    if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE") || format.Contains("YA"))
                                    {
                                        msg.Append("HC店铺；");
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(shopModel.IsInstall))
                                        {
                                            msg.Append("安装级别为空；");
                                        }
                                    }
                                }
                                else
                                {
                                    msg.Append("店铺类型为空；");
                                }
                            }
                            else
                            {
                                msg.Append("店铺编号不存在；");
                            }
                        }
                        else
                        {
                            msg.Append("店铺编号不存在；");
                        }
                    }
                    if (msg.Length > 0)
                    {
                        DataRow dr1 = errorTB.NewRow();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            dr1["" + cols[i] + ""] = dr[cols[i]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        errorTB.Rows.Add(dr1);
                        failNum++;
                    }
                }
                if (errorTB.Rows.Count > 0)
                {
                    Session["errorTb"] = errorTB;
                }
            }
        }

        void CheckPOP(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                int shopId = 0;
                string shopNo = string.Empty;
                string sheet = string.Empty;
                string graphicNo = string.Empty;
                DataColumnCollection cols = ds.Tables[0].Columns;
                errorTB = CommonMethod.CreateErrorTB(cols);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    shopId = 0;
                    shopNo = string.Empty;
                    sheet = string.Empty;
                    graphicNo = string.Empty;
                    StringBuilder msg = new StringBuilder();
                    bool canGo = true;
                    if (cols.Contains("POSCode"))
                    {
                        shopNo = dr["POSCode"].ToString().Trim();
                    }
                    else if (cols.Contains("POS Code"))
                    {
                        shopNo = dr["POS Code"].ToString().Trim();
                    }
                    else if (cols.Contains("店铺编号"))
                    {
                        shopNo = dr["店铺编号"].ToString().Trim();
                    }
                    if (cols.Contains("pop编号"))
                    {
                        graphicNo = dr["pop编号"].ToString().Trim();
                    }
                    else if (cols.Contains("POP编号"))
                    {
                        graphicNo = dr["POP编号"].ToString().Trim();
                    }
                    else if (cols.Contains("Graphic No"))
                    {
                        graphicNo = dr["Graphic No"].ToString().Trim();
                    }
                    else if (cols.Contains("GraphicNo"))
                    {
                        graphicNo = dr["GraphicNo"].ToString().Trim();
                    }
                    else if (cols.Contains("Graphic No#"))
                    {
                        graphicNo = dr["Graphic No#"].ToString().Trim();
                    }
                    else if (cols.Contains("GraphicNo#"))
                    {
                        graphicNo = dr["GraphicNo#"].ToString().Trim();
                    }
                    if (cols.Contains("Sheet"))
                        sheet = dr["Sheet"].ToString().Trim();
                    else if (cols.Contains("位置"))
                        sheet = dr["位置"].ToString().Trim();
                    else if (cols.Contains("pop位置"))
                        sheet = dr["pop位置"].ToString().Trim();

                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        msg.Append("店铺编号 为空；");
                        canGo = false;
                    }
                    if (string.IsNullOrWhiteSpace(graphicNo))
                    {
                        msg.Append("pop编号 为空；");
                        canGo = false;
                    }
                    if (string.IsNullOrWhiteSpace(sheet))
                    {
                        msg.Append("位置 为空；");
                        canGo = false;
                    }
                    else
                    {
                        sheet = sheet == "户外" ? "OOH" : sheet;
                        if (sheet.ToLower() == "t-stand" || sheet.ToLower() == "tstand")
                        {
                            sheet = "中岛";
                        }
                        sheet = sheet.ToUpper();
                    }
                    if (canGo)
                    {
                        Shop shopModel = null;
                        if (GetShopFromDB(shopNo, ref shopModel))
                        {
                            if (shopModel != null)
                            {
                                shopId = shopModel.Id;
                                if (!string.IsNullOrWhiteSpace(shopModel.Format))
                                {
                                    string format = shopModel.Format.ToUpper();
                                    if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE") || format.Contains("YA"))
                                    {
                                        msg.Append("HC店铺；");
                                        canGo = false;
                                    }

                                }
                                else
                                {
                                    msg.Append("店铺类型为空；");
                                    canGo = false;
                                }
                            }
                            else
                            {
                                msg.Append("店铺编号不存在；");
                                canGo = false;
                            }
                        }
                    }
                    if (canGo)
                    {
                        POP popFromDB = null;
                        string errorMsg = string.Empty;
                        if (!GetPOPFromDB(shopId, sheet, graphicNo, ref popFromDB, out errorMsg))
                        {
                            msg.Append(errorMsg);
                        }
                    }
                    if (msg.Length > 0)
                    {
                        DataRow dr1 = errorTB.NewRow();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            dr1["" + cols[i] + ""] = dr[cols[i]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        errorTB.Rows.Add(dr1);
                        failNum++;
                    }

                }
                if (errorTB.Rows.Count > 0)
                {
                    Session["errorTb"] = errorTB;
                }
            }
        }


        Dictionary<string, Shop> shopDic = new Dictionary<string, Shop>();
        ShopBLL shopBll = new ShopBLL();
        bool GetShopFromDB(string shopNo, ref Shop shopFromDb)
        {
            bool flag = false;
            shopNo = shopNo.ToUpper();
            if (shopDic.Keys.Contains(shopNo))
            {
                shopFromDb = shopDic[shopNo];
                flag = true;
            }
            else
            {
                var shopModel = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo).FirstOrDefault();
                if (shopModel != null)
                {
                    shopFromDb = shopModel;
                    shopDic.Add(shopNo, shopFromDb);
                    flag = true;
                }
            }

            return flag;
        }

        POPBLL popBll = new POPBLL();
        POPBlackListBLL POPBlackBll = new POPBlackListBLL();
        bool GetPOPFromDB(int shopId, string sheet, string graphicNo, ref POP popFromDB, out string errorMsg)
        {
            bool flag = false;
            errorMsg = string.Empty;
            var popModel = popBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet && s.GraphicNo.ToUpper() == graphicNo.ToUpper()).FirstOrDefault();
            if (popModel != null)
            {
                popFromDB = popModel;
                flag = true;
            }
            else
            {
                var model1 = POPBlackBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet).FirstOrDefault();
                if (model1 != null)
                {
                    if (!string.IsNullOrWhiteSpace(model1.GraphicNo))
                    {
                        List<string> list = StringHelper.ToStringList(model1.GraphicNo, ',');
                        if (list.Contains(graphicNo.ToUpper()))
                            errorMsg = "该店铺不含该位置的POP，分区已确认；";
                        else
                            errorMsg = "该店铺缺少该位置的POP，请更新基础数据；";
                    }

                }
                else
                    errorMsg = "该店铺缺少该位置的POP，请更新基础数据；";
            }
            return flag;
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

        }

    }
}
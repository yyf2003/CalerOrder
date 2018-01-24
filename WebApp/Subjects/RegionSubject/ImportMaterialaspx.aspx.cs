using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.OleDb;
using Common;
using BLL;
using Models;
using System.Data;
using System.Text;
using DAL;

namespace WebApp.Subjects.RegionSubject
{
    public partial class ImportMaterialaspx : BasePage
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
                if (Session["errorTb"] != null)
                {
                    labState.Text = "部分数据导入失败！";
                    ExportFailMsg.Style.Add("display", "block");
                }
            }
        }

        protected void lbExportError_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }

        protected void lbDownLoadTemplate_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "OrderMaterialTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    OrderMaterialBLL materialBll = new OrderMaterialBLL();
                    DataTable MaterialErrorTB = null;
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
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        MaterialErrorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                        OrderMaterial model;
                        if (!cbAdd.Checked)
                        {
                            materialBll.Delete(s=>s.RegionSupplementId==subjectId);
                        }
                        DataTable dt1 = ds.Tables[0];
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            bool canSave = true;
                            int shopId = 0;
                            int realSubjectId = 0;
                            StringBuilder msg = new StringBuilder();
                            string shopNo = string.Empty;
                            string subjectName = string.Empty;
                            string sheet = string.Empty;
                            string materialName = string.Empty;
                            string num = string.Empty;
                            string length = string.Empty;
                            string width = string.Empty;
                            string high = string.Empty;
                            string price = string.Empty;
                            string remark = string.Empty;
                            if (cols.Contains("POSCode"))
                            {
                                shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POSCode"].ToString().Trim());
                            }
                            else if (cols.Contains("POS Code"))
                            {
                                shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POS Code"].ToString().Trim());
                            }
                            else if (cols.Contains("店铺编号"))
                            {
                                shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["店铺编号"].ToString().Trim());
                            }
                            if (cols.Contains("项目"))
                            {
                                subjectName = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["项目"].ToString().Trim());
                            }
                            else if (cols.Contains("项目名称"))
                            {
                                subjectName = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["项目名称"].ToString().Trim());
                            }
                            if (cols.Contains("Sheet"))
                            {
                                sheet = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["Sheet"].ToString().Trim());
                            }
                            else if (cols.Contains("POP位置"))
                            {
                                sheet = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POP位置"].ToString().Trim());
                            }
                            if (cols.Contains("物料名称"))
                            {
                                materialName = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["物料名称"].ToString().Trim());
                            }
                            else if (cols.Contains("物料"))
                            {
                                materialName = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["物料"].ToString().Trim());
                            }
                            if (cols.Contains("数量"))
                            {
                                num = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["数量"].ToString().Trim());
                            }
                            if (cols.Contains("长"))
                            {
                                length = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["长"].ToString().Trim());
                            }
                            if (cols.Contains("宽"))
                            {
                                width = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["宽"].ToString().Trim());
                            }
                            if (cols.Contains("高"))
                            {
                                high = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["高"].ToString().Trim());
                            }
                            if (cols.Contains("单价"))
                            {
                                price = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["单价"].ToString().Trim());
                            }
                            else if (cols.Contains("价格"))
                            {
                                price = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["价格"].ToString().Trim());
                            }
                            if (cols.Contains("备注"))
                            {
                                remark = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["备注"].ToString().Trim());
                            }
                            if (string.IsNullOrWhiteSpace(shopNo))
                            {
                                canSave = false;
                                msg.Append("店铺编号 为空；");
                            }
                            int IsInvalid = 0;
                            bool IsHC = false;
                            bool EmptyInstallLevel = false;
                            bool EmptyFormat = false;
                            if (!string.IsNullOrWhiteSpace(shopNo) && !CheckShop(shopNo, out shopId, out IsInvalid, out IsHC, out EmptyInstallLevel, out EmptyFormat))
                            {
                                canSave = false;
                                msg.Append("店铺编号不存在；");
                            }
                            if (string.IsNullOrWhiteSpace(subjectName))
                            {
                                canSave = false;
                                msg.Append("项目名称 为空；");
                            }
                            else if (!CheckSubject(subjectName, out realSubjectId))
                            {
                                canSave = false;
                                msg.Append("项目名称不正确；");
                            }
                            if (string.IsNullOrWhiteSpace(materialName))
                            {
                                canSave = false;
                                msg.Append("物料名称 为空；");
                            }
                            
                            if (string.IsNullOrWhiteSpace(num))
                            {
                                canSave = false;
                                msg.Append("数量 为空；");
                            }
                            else if (!StringHelper.IsIntVal(num))
                            {
                                canSave = false;
                                msg.Append("数量填写不正确；");
                            }
                            if (!string.IsNullOrWhiteSpace(length) && !StringHelper.IsDecimalVal(length))
                            {
                                canSave = false;
                                msg.Append("长度填写不正确；");
                            }
                            if (!string.IsNullOrWhiteSpace(width) && !StringHelper.IsDecimalVal(width))
                            {
                                canSave = false;
                                msg.Append("宽度填写不正确；");
                            }
                            if (!string.IsNullOrWhiteSpace(high) && !StringHelper.IsDecimalVal(high))
                            {
                                canSave = false;
                                msg.Append("高度填写不正确；");
                            }
                            if (!string.IsNullOrWhiteSpace(price) && !StringHelper.IsDecimalVal(price))
                            {
                                canSave = false;
                                msg.Append("单价填写不正确；");
                            }
                            if (canSave)
                            {
                                sheet = sheet == "户外" ? "OOH" : sheet;
                                if (int.Parse(num) > 0)
                                {
                                    model = new OrderMaterial();
                                    model.AddDate = DateTime.Now;
                                    model.MaterialCount = int.Parse(num);
                                    if (!string.IsNullOrWhiteSpace(length))
                                        model.MaterialLength = decimal.Parse(length);
                                    if (!string.IsNullOrWhiteSpace(width))
                                        model.MaterialWidth = decimal.Parse(width);
                                    if (!string.IsNullOrWhiteSpace(high))
                                        model.MaterialHigh = decimal.Parse(high);
                                    model.MaterialName = materialName;
                                    if (!string.IsNullOrWhiteSpace(price))
                                        model.Price = decimal.Parse(price);
                                    model.Remark = remark;
                                    model.Sheet = sheet;
                                    model.ShopId = shopId;
                                    model.SubjectId = realSubjectId;
                                    model.RegionSupplementId = subjectId;
                                    materialBll.Add(model);
                                   
                                }
                            }
                            if (msg.Length > 0)
                            {
                                DataRow dr1 = MaterialErrorTB.NewRow();
                                for (int ii = 0; ii < cols.Count; ii++)
                                {
                                    dr1["" + cols[ii] + ""] = dt1.Rows[i][ii].ToString();
                                }
                                dr1["错误信息"] = msg.ToString();
                                MaterialErrorTB.Rows.Add(dr1);
                            }
                        }
                        if (MaterialErrorTB.Rows.Count > 0)
                        {
                            Session["errorTb"] = MaterialErrorTB;
                        }
                        else
                        {
                            Session["errorTb"] = null;
                        }
                        conn.Dispose();
                        conn.Close();
                        Response.Redirect(string.Format("ImportMaterialaspx.aspx?import=1&subjectId={0}", subjectId), false);
                    }
                    else
                    {
                        Panel1.Visible = true;
                        labState.Text = "导入失败：表格里面没有数据！";
                    }
                }
            }
        }




        ShopBLL shopBll = new ShopBLL();
        Dictionary<string, int> shopNoList = new Dictionary<string, int>();
      
        Dictionary<string, int> InvalidShops = new Dictionary<string, int>();
        List<string> HCList = new List<string>();
        List<string> EmptyInstallLevelList = new List<string>();
        List<string> EmptyFormatList = new List<string>();
        int guidanceType = 0;
        bool CheckShop(string shopNo, out int shopId, out int IsInvalid, out bool IsHc, out bool EmptyInstallLevel, out bool EmptyFormat)
        {
            IsHc = false;
            EmptyInstallLevel = false;
            EmptyFormat = false;
            shopId = 0;
            IsInvalid = 0;//关闭店铺
            bool flag = true;

            if (shopNoList.Keys.Contains(shopNo.ToUpper()))
            {
                shopId = shopNoList[shopNo.ToUpper()];
                if (InvalidShops.Keys.Contains(shopNo.ToUpper()))
                    IsInvalid = InvalidShops[shopNo.ToUpper()];
                if (HCList.Contains(shopNo.ToUpper()))
                    IsHc = true;
                if (EmptyInstallLevelList.Contains(shopNo.ToUpper()))
                    EmptyInstallLevel = true;
                if (EmptyFormatList.Contains(shopNo.ToUpper()))
                    EmptyFormat = true;
            }
            else
            {
                var shop = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper() && (s.IsDelete==null || s.IsDelete==false)).FirstOrDefault();
                if (shop != null)
                {
                    shopId = shop.Id;
                    //IsInvalid = shop.IsInvalid ?? 0;
                    //关闭店铺
                    if (!string.IsNullOrWhiteSpace(shop.Status) && (shop.Status.Contains("关") || shop.Status.Contains("闭")))
                    {
                        IsInvalid = shopId;
                        if (!InvalidShops.Keys.Contains(shopNo.ToUpper()))
                            InvalidShops.Add(shopNo.ToUpper(), IsInvalid);
                    }
                    if (guidanceType == 0)
                    {
                        var guidance = (from subject in CurrentContext.DbContext.Subject
                                        join g in CurrentContext.DbContext.SubjectGuidance
                                        on subject.GuidanceId equals g.ItemId
                                        where subject.Id == subjectId
                                        select g).FirstOrDefault();
                        if (guidance != null)
                            guidanceType = guidance.ActivityTypeId ?? 1;//活动类型：1安装，2发货，3促销
                    }
                    if (guidanceType != 3)
                    {
                        if (!string.IsNullOrWhiteSpace(shop.Format))
                        {
                            //非促销的活动要检查是不是HC店铺，如果是，就不导入Homecourt
                            string format = shop.Format.ToUpper();

                            if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE"))
                            {
                                IsHc = true;
                                if (!HCList.Contains(shopNo.ToUpper()))
                                    HCList.Add(shopNo.ToUpper());
                            }
                        }
                        if (string.IsNullOrWhiteSpace(shop.IsInstall))
                        {
                            EmptyInstallLevel = true;
                            if (!EmptyInstallLevelList.Contains(shopNo.ToUpper()))
                                EmptyInstallLevelList.Add(shopNo.ToUpper());
                        }
                        if (string.IsNullOrWhiteSpace(shop.Format))
                        {
                            EmptyFormat = true;
                            if (!EmptyFormatList.Contains(shopNo.ToUpper()))
                                EmptyFormatList.Add(shopNo.ToUpper());
                        }
                    }
                    
                    if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                        shopNoList.Add(shopNo.ToUpper(), shopId);

                }
                else
                    flag = false;
            }

            return flag;
        }

        SubjectBLL subjectBll = new SubjectBLL();
        Dictionary<string, int> subjectList = new Dictionary<string, int>();
        bool CheckSubject(string subjectName, out int supplementSubjectId)
        {
            supplementSubjectId = 0;
            bool flag = true;
            subjectName = subjectName.Trim().ToLower();
            if (subjectList.Keys.Contains(subjectName))
            {
                supplementSubjectId = subjectList[subjectName];
            }
            else
            {
                var model = subjectBll.GetList(s => s.SubjectName.Trim().ToLower() == subjectName && (s.SubjectType ?? 1) == (int)SubjectTypeEnum.POP订单 && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState==1).FirstOrDefault();
                if (model != null)
                {
                    supplementSubjectId = model.Id;

                }
                else
                    flag = false;
            }
            return flag;
        }
    }
}
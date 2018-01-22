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


namespace WebApp.Materials
{
    public partial class ImportBasicMaterial : BasePage
    {
        BasicMaterialBLL materialBll = new BasicMaterialBLL();
        MaterialCategoryBLL categoryBll = new MaterialCategoryBLL();
        UnitInfoBLL unitBll = new UnitInfoBLL();
        protected void Page_Load(object sender, EventArgs e)
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
                        BasicMaterial materialModel;
                        int successNum = 0;
                        int failNum = 0;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            int categoryId = 0;
                            int unitId = 0;
                            string categoryName = string.Empty;
                            if(cols.Contains("材料类别"))
                                categoryName = dr["材料类别"].ToString().Trim();
                            else if (cols.Contains("材质类别"))
                                categoryName = dr["材质类别"].ToString().Trim();
                            else if (cols.Contains("类别"))
                                categoryName = dr["类别"].ToString().Trim();
                            string materialName = string.Empty;
                            if (cols.Contains("材料名称"))
                                materialName = dr["材料名称"].ToString().Trim();
                            else if (cols.Contains("材质名称"))
                                materialName = dr["材质名称"].ToString().Trim();
                            else if (cols.Contains("名称"))
                                materialName = dr["名称"].ToString().Trim();
                            string unitName = string.Empty;
                            if (cols.Contains("单位名称"))
                                unitName = dr["单位名称"].ToString().Trim();
                            else if (cols.Contains("单位"))
                                unitName = dr["单位"].ToString().Trim();
                            bool canSave = true;
                            
                            StringBuilder errorMsg = new StringBuilder();
                            if (string.IsNullOrWhiteSpace(categoryName))
                            {
                                canSave = false;
                                errorMsg.Append("材料类别 为空；");
                            }
                            else if (!CheckCategory(categoryName, out categoryId))
                            {
                                canSave = false;
                                errorMsg.Append("材料类别 不存在，请先添加；");
                            }
                            if (string.IsNullOrWhiteSpace(materialName))
                            {
                                canSave = false;
                                errorMsg.Append("材料名称 为空；");
                            }
                            else if (CheckMaterialName(materialName))
                            {
                                canSave = false;
                                errorMsg.Append("材料名称重复；");
                            }
                            if (!string.IsNullOrWhiteSpace(unitName))
                            {
                                CheckUnit(unitName, out unitId);
                            }
                            if (canSave)
                            {
                                materialModel = new BasicMaterial();
                                materialModel.IsDelete = false;
                                materialModel.MaterialCategoryId = categoryId;
                                materialModel.MaterialName = materialName;
                                materialModel.UnitId = unitId;
                                materialBll.Add(materialModel);
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
                        Response.Redirect(string.Format("ImportBasicMaterial.aspx?successNum={0}&failNum={1}", successNum, failNum), false);
                    }
                }
            }
        }

        
        Dictionary<string, int> categoryDic = new Dictionary<string, int>();
        bool CheckCategory(string categoryName, out int categoryId)
        {
            categoryId = 0;
            bool flag = false;
            if (categoryDic.Keys.Contains(categoryName))
            {
                categoryId = categoryDic[categoryName];
                flag = true;
            }
            else
            {
                var model = categoryBll.GetList(s => s.CategoryName == categoryName).FirstOrDefault();
                if (model != null)
                {
                    categoryId = model.Id;
                    categoryDic.Add(categoryName, categoryId);
                    flag = true;
                }
            }
            return flag;
        }

        
        bool CheckMaterialName(string materialName)
        {
           
            var list = materialBll.GetList(s => s.MaterialName.ToLower() == materialName.ToLower());
            return list.Any();
        }

        Dictionary<string, int> unitDic = new Dictionary<string, int>();
        bool CheckUnit(string unitName, out int unitId)
        {
            unitId = 0;
            bool flag = false;
            if (unitDic.Keys.Contains(unitName))
            {
                unitId = unitDic[unitName];
                flag = true;

            }
            else
            {
                var model = unitBll.GetList(s => s.UnitName.ToLower() == unitName.ToLower()).FirstOrDefault();
                if (model != null)
                {
                    unitId = model.Id;
                    unitDic.Add(unitName, unitId);
                    flag = true;
                }
                else
                {
                    UnitInfo unitModel = new UnitInfo() { UnitName = unitName };
                    unitBll.Add(unitModel);
                    unitId = unitModel.Id;
                    unitDic.Add(unitName, unitId);
                    flag = true;
                }
            }
            return flag;
        }

        protected void lbDownLoad_Click(object sender, EventArgs e)
        {

        }

        protected void lbExportErrorMsg_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "基础材质导入失败信息");
            }
        }
    }
}
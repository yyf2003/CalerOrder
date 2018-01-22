using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using Models;
using BLL;
using Common;
using System.Text;

namespace WebApp.Materials
{
    public partial class ImportMaterial : BasePage
    {
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

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoadShop_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "MaterialTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        int successNum = 0;
        int failNum = 0;
        MaterialBLL materialBll = new MaterialBLL();
        Material materialModel;
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
                        DataTable ErrorTB = CommonMethod.CreateErrorTB(cols);

                        string bigType = string.Empty;
                        string smallType = string.Empty;
                        string brand = string.Empty;
                        
                        string category = string.Empty;
                        string name = string.Empty;
                        string width = string.Empty;
                        string length = string.Empty;
                        string area = string.Empty;
                        string unit = string.Empty;
                        string remark = string.Empty;
                        List<string> finish = new List<string>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringBuilder errorMsg = new StringBuilder();
                            bool canSave = true;
                            bigType = dr["大类"].ToString();
                            smallType = dr["小类"].ToString();
                            brand = dr["品牌名称"].ToString();
                            category = dr["材质类型"].ToString();
                            name = dr["材质名称"].ToString();
                            width = dr["宽"].ToString();
                            length = dr["长"].ToString();
                            area = dr["面积"].ToString();
                            unit = dr["单位"].ToString();
                            remark = dr["备注"].ToString();
                            int categoryId = 0;
                            if (string.IsNullOrWhiteSpace(bigType))
                            {
                                canSave = false;
                                errorMsg.Append("大类 为空；");
                            }
                            if (string.IsNullOrWhiteSpace(smallType))
                            {
                                canSave = false;
                                errorMsg.Append("小类 为空；");
                            }
                            if (string.IsNullOrWhiteSpace(category))
                            {
                                canSave = false;
                                errorMsg.Append("材质类型 为空；");
                            }
                            else if ((categoryId = GetCategoryId(category)) == 0)
                            {
                                canSave = false;
                                errorMsg.Append("材质类型 不存在；");
                            }
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                canSave = false;
                                errorMsg.Append("材质名称 为空；");
                            }
                            string data = bigType + smallType + category + name + brand;
                            if (finish.Contains(data))
                            {
                                errorMsg.Append("重复；");
                                canSave = false;
                            }
                            

                            if (canSave)
                            {
                                int smallTypeId = 0;
                                int bigTypeId = 0;
                                int brandId = 0;
                                int materialId = 0;
                                GetTypeId(bigType, smallType, out bigTypeId, out smallTypeId);
                                brandId = GetBrandId(brand);
                                materialModel = new Material();
                                materialModel.Area = StringHelper.IsDecimal(area);
                                materialModel.IsDelete = false;
                                materialModel.Length = StringHelper.IsDecimal(length);
                                materialModel.MaterialBrandId = brandId;
                                //materialModel.Specification = name;
                                materialModel.MaterialCategoryId = categoryId;
                                materialModel.MaterialName = name;
                                materialModel.BigTypeId = bigTypeId;
                                materialModel.SmallTypeId = smallTypeId;
                                materialModel.Remark = remark;
                                materialModel.Unit = unit;
                                materialModel.Width = StringHelper.IsDecimal(width);
                                if (CheckMaterial(bigTypeId, smallTypeId, categoryId, name, brandId, out materialId))
                                {
                                    //已存在相同的材质，就直接更新
                                    materialModel.Id = materialId;
                                    materialBll.Update(materialModel);
                                    
                                }
                                else
                                {
                                    materialModel.AddDate = DateTime.Now;
                                    materialBll.Add(materialModel);

                                }
                                finish.Add(data);
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
                        Response.Redirect(string.Format("ImportMaterial.aspx?successNum={0}&failNum={1}", successNum, failNum), false);

                    }
                }
            }
        }

        MaterialTypeBLL materialTypeBll = new MaterialTypeBLL();
        //MaterialType typeModel;
        Dictionary<string, int> bigTypeDic = new Dictionary<string, int>();
        Dictionary<string, int> smallTypeDic = new Dictionary<string, int>();
        void GetTypeId(string bigType, string smallType,out int bigTypeId,out int samllTypeId)
        {
            bigTypeId = 0;
            samllTypeId = 0;
             ;
            if (bigTypeDic.Keys.Contains(bigType))
            {
                bigTypeId = bigTypeDic[bigType];
            }
            else
            {
                MaterialType Model1 = materialTypeBll.GetList(s => s.MaterialTypeName == bigType && s.ParentId == 0).FirstOrDefault();
                if (Model1 == null)
                {
                    Model1 = new MaterialType { AddDate = DateTime.Now, IsDelete = false, MaterialTypeName = bigType, ParentId = 0 };
                    materialTypeBll.Add(Model1);
                }
                bigTypeId = Model1.Id;
                bigTypeDic.Add(bigType, bigTypeId);
            }
            if (smallTypeDic.Keys.Contains(smallType))
            {
                samllTypeId = smallTypeDic[smallType];
            }
            else
            {
                int bigid = bigTypeId;
                MaterialType Model2 = materialTypeBll.GetList(s => s.MaterialTypeName == smallType && s.ParentId == bigid).FirstOrDefault();
                if (Model2 == null)
                {
                    Model2 = new MaterialType { AddDate = DateTime.Now, IsDelete = false, MaterialTypeName = smallType, ParentId = bigTypeId };
                    materialTypeBll.Add(Model2);
                }
                samllTypeId = Model2.Id;
                smallTypeDic.Add(smallType, samllTypeId);
            }
           
            
        }

        bool CheckMaterial(int bigTypeId,int smallTypeId,int categoryId,string guige,int brandId,out int materialId)
        {
            materialId = 0;
            var list = materialBll.GetList(s => s.BigTypeId == bigTypeId && s.SmallTypeId == smallTypeId && s.MaterialCategoryId == categoryId && s.Specification == guige && s.MaterialBrandId==brandId);
            if (list.Any())
            {
                materialId = list[0].Id;
                return true;
            }
            return false;
        }

        MaterialBrandBLL brandBll = new MaterialBrandBLL();
        MaterialBrand brandModel;
        Dictionary<string, int> brandDic = new Dictionary<string,int>();
        int GetBrandId(string brand)
        {
            int brandId = 0;
            if (!string.IsNullOrWhiteSpace(brand))
            {
                if (brandDic.Keys.Contains(brand))
                {
                    brandId = brandDic[brand];
                }
                else
                {
                    brandModel = brandBll.GetList(s => s.BrandName == brand).FirstOrDefault();
                    if (brandModel == null)
                    {
                        brandModel = new MaterialBrand { AddDate = DateTime.Now, BrandName = brand, IsDelete = false };
                        brandBll.Add(brandModel);
                    }
                    brandId = brandModel.Id;
                    brandDic.Add(brand, brandId);
                }
            }
           
            return brandId;
        }

        MaterialCategoryBLL categoryBll = new MaterialCategoryBLL();
        IDictionary<string, int> categoryDic = new Dictionary<string, int>();
        int GetCategoryId(string category)
        {
            int id = 0;
            if (categoryDic.ContainsKey(category))
            {
                id = categoryDic[category];
            }
            else
            {
                var list = categoryBll.GetList(s => s.CategoryName == category);
                if (list.Any())
                {
                    id = list[0].Id;
                    categoryDic.Add(category, id);
                }
            }
            return id;
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
                OperateFile.ExportExcel(dt, "数据导入失败信息");
            }
        }
    }
}
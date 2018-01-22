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
    public partial class ImportCustomerMaterial : BasePage
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
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
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
            string fileName = "CustomerMaterialTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        CustomerMaterialBLL customerMaterialBll = new CustomerMaterialBLL();
        CustomerMaterial customerMaterialModel;
        CustomerMaterialDetailBLL detailBll = new CustomerMaterialDetailBLL();
        CustomerMaterialDetail detailModel;
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
                        int customerId = int.Parse(ddlCustomer.SelectedValue);
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        DataTable ErrorTB = CommonMethod.CreateErrorTB(cols);
                        string region = string.Empty;
                        string materialName = string.Empty;
                        //string printPrice = string.Empty;
                        string costPrice = string.Empty;
                        string salePrice = string.Empty;
                        string contents = string.Empty;
                        
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringBuilder errorMsg = new StringBuilder();
                            bool canSave = true;
                            region = dr["区域"].ToString();
                            materialName = dr["材质名称"].ToString();
                            //printPrice = dr["机器打印价"].ToString();
                            costPrice = dr["成本价"].ToString();
                            salePrice = dr["销售价"].ToString();
                            contents = dr["材料"].ToString();
                            int regionId = 0;
                            string newRegion=string.Empty;
                            if (string.IsNullOrWhiteSpace(region))
                            {
                                canSave = false;
                                errorMsg.Append("区域 为空；");
                            }
                            else
                            { 
                               GetRegion(region,out regionId,out newRegion);
                               if (regionId == 0)
                               {
                                   canSave = false;
                                   errorMsg.Append("所选客户不存在该区域；");
                               }
                            }
                            if (string.IsNullOrWhiteSpace(materialName))
                            {
                                canSave = false;
                                errorMsg.Append("材质名称 为空；");
                            }
                            
                            if (string.IsNullOrWhiteSpace(costPrice))
                            {
                                costPrice = "0";

                            }
                            if (string.IsNullOrWhiteSpace(salePrice))
                            {
                                salePrice = "0";

                            }
                            //List<int> materialIds = new List<int>();
                            Dictionary<int, string> categoryIdDic = new Dictionary<int, string>();
                            if (string.IsNullOrWhiteSpace(contents))
                            {
                                canSave = false;
                                errorMsg.Append("材料 为空；");
                            }
                            else
                            {
                                string[] materials = contents.Split('/');
                                StringBuilder msg1 = new StringBuilder();

                                foreach (string s in materials)
                                {
                                    if (!string.IsNullOrWhiteSpace(s))
                                    {
                                        int id = GetMaterialCategoryId(s);
                                        if (id == 0)
                                        {
                                            canSave = false;
                                            msg1.Append(s + "，");
                                        }
                                        else
                                        {
                                            //materialIds.Add(id);
                                            categoryIdDic.Add(id,s);
                                        }
                                    }
                                }
                                if (msg1.Length > 0)
                                {
                                    canSave = false;
                                    errorMsg.Append("材料：" + msg1.ToString().TrimEnd('，')+" 不存在；");
                                }
                            }
                            if (canSave)
                            {
                                customerMaterialModel = new CustomerMaterial();
                                customerMaterialModel.AddDate = DateTime.Now;
                                customerMaterialModel.AddUserId = CurrentUser.UserId;
                                customerMaterialModel.CostPrice = StringHelper.IsDecimal(costPrice);
                                customerMaterialModel.CustomerId = customerId;
                                customerMaterialModel.CustomerMaterialName = materialName;
                                customerMaterialModel.IsDelete = false;
                                //customerMaterialModel.PrintPrice = StringHelper.IsDecimal(printPrice);
                                customerMaterialModel.RegionName =newRegion;
                                customerMaterialModel.RegionId = regionId;
                                customerMaterialModel.SalePrice = StringHelper.IsDecimal(salePrice);
                                int id = 0;
                                if (CheckCustomerMaterial(customerId, region, materialName, out id))
                                {
                                    customerMaterialModel.Id = id;
                                    customerMaterialBll.Update(customerMaterialModel);
                                    detailBll.Delete(s => s.CustomerMaterialId == id);
                                }
                                else
                                {
                                    customerMaterialModel.IsDelete = false;
                                    customerMaterialBll.Add(customerMaterialModel);
                                    id = customerMaterialModel.Id;
                                }
                                //materialIds.ForEach(mid =>
                                //{
                                    //detailModel = new CustomerMaterialDetail { CustomerMaterialId = id,MarterialCategoryId = mid };
                                    //detailBll.Add(detailModel);
                                //});
                                foreach (var item in categoryIdDic)
                                {
                                    detailModel = new CustomerMaterialDetail { CustomerMaterialId = id, MarterialCategoryId = item.Key,MarterialCategoryName=item.Value };
                                    detailBll.Add(detailModel);
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
                        Response.Redirect(string.Format("ImportCustomerMaterial.aspx?successNum={0}&failNum={1}", successNum, failNum), false);
                    }
                }
            }
        }

        Dictionary<string, Dictionary<int, string>> regionDic = new Dictionary<string, Dictionary<int, string>>();
        RegionBLL regionBll = new RegionBLL();
        void GetRegion(string region,out int regionId,out string newRegion)
        {
            regionId = 0;
            newRegion = string.Empty;
            if (regionDic.ContainsKey(region.ToLower()))
            {
                var item = regionDic[region.ToLower()];
                foreach (var item1 in item)
                {
                    regionId = item1.Key;
                    newRegion = item1.Value;
                }
            }
            else
            {
                int customerId = int.Parse(ddlCustomer.SelectedValue);
                var model = regionBll.GetList(s => s.CustomerId == customerId && s.RegionName.ToLower() == region.ToLower() && (s.IsDelete==false || s.IsDelete==null)).FirstOrDefault();
                if (model != null)
                {
                    regionId = model.Id;
                    newRegion = model.RegionName;
                    Dictionary<int, string> newDic = new Dictionary<int, string>();
                    newDic.Add(regionId, newRegion);
                    regionDic.Add(region.ToLower(), newDic);
                }
            }
        }

        //MaterialBLL materialBll = new MaterialBLL();
        MaterialCategoryBLL categoryBll = new MaterialCategoryBLL();
        Dictionary<string, int> materialDic = new Dictionary<string, int>();
        int GetMaterialCategoryId(string categoryName)
        {
            int id = 0;
            if (materialDic.Keys.Contains(categoryName))
            {
                id = materialDic[categoryName];
            }
            else
            {
                var model = categoryBll.GetList(s => s.CategoryName == categoryName).FirstOrDefault();
                if (model != null)
                {
                    id = model.Id;
                    materialDic.Add(categoryName, id);
                }

            }
            return id;
        }

        /// <summary>
        /// 判定客户材质是不是已经存在，存在就更新，
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckCustomerMaterial(int customerId,string region,string name, out int id)
        {
            id = 0;
            var model = customerMaterialBll.GetList(s => s.CustomerMaterialName == name && s.CustomerId==customerId && s.RegionName==region).FirstOrDefault();
            if (model != null)
            {
                id = model.Id;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 导出失败的信息
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
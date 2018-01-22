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
    public partial class ImportCustomerMaterialInfo : BasePage
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
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        CustomerMaterialInfoBLL materialBll = new CustomerMaterialInfoBLL();
        CustomerMaterialInfo materialModel;
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
                        string unit = string.Empty;
                        string price = string.Empty;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringBuilder errorMsg = new StringBuilder();
                            bool canSave = true;
                            //region = dr["区域"].ToString();
                            materialName = dr["材质名称"].ToString();
                            unit = dr["单位"].ToString();
                            price = dr["价格"].ToString();
                            //int regionId = 0;
                            string newRegion = string.Empty;
                            //if (string.IsNullOrWhiteSpace(region))
                            //{
                            //    canSave = false;
                            //    errorMsg.Append("区域 为空；");
                            //}
                            //else
                            //{
                            //    GetRegion(region, out regionId, out newRegion);
                            //    if (regionId == 0)
                            //    {
                            //        canSave = false;
                            //        errorMsg.Append("所选客户不存在该区域；");
                            //    }
                            //}
                            if (string.IsNullOrWhiteSpace(materialName))
                            {
                                canSave = false;
                                errorMsg.Append("材质名称 为空；");
                            }

                            if (string.IsNullOrWhiteSpace(price))
                            {
                                price = "0";
                            }
                            else
                            { 
                                double a=0;
                                if (!double.TryParse(price, out a))
                                {
                                    canSave = false;
                                    errorMsg.Append("价格填写不正确；");
                                }
                            }
                            if (canSave)
                            {
                                materialModel = new CustomerMaterialInfo();
                                materialModel.MaterialName = materialName;
                                materialModel.Price = decimal.Parse(price);
                                //materialModel.Region = region;
                                materialModel.Unit = unit;
                                int id = 0;
                                if (CheckMaterial(customerId, materialName, out id))
                                {
                                    //更新
                                    materialModel.Id = id;
                                    materialBll.Update(materialModel);

                                }
                                else
                                {
                                    materialModel.AddDate = DateTime.Now;
                                    materialModel.AddUserId = CurrentUser.UserId;
                                    materialModel.CustomerId = customerId;
                                    materialModel.IsDelete = false;
                                    materialBll.Add(materialModel);
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
                        Response.Redirect(string.Format("ImportCustomerMaterialInfo.aspx?successNum={0}&failNum={1}", successNum, failNum), false);
                    }

                    
                }
            }
        }



        Dictionary<string, Dictionary<int, string>> regionDic = new Dictionary<string, Dictionary<int, string>>();
        RegionBLL regionBll = new RegionBLL();
        void GetRegion(string region, out int regionId, out string newRegion)
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
                var model = regionBll.GetList(s => s.CustomerId == customerId && s.RegionName.ToLower() == region.ToLower() && (s.IsDelete == false || s.IsDelete == null)).FirstOrDefault();
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

        bool CheckMaterial(int customerId,string name, out int id)
        {
            id = 0;
            var model = materialBll.GetList(s => s.MaterialName.ToUpper() == name.ToUpper() && s.CustomerId == customerId).FirstOrDefault();
            if (model != null)
            {
                id = model.Id;
                return true;
            }
            return false;
        }

        protected void lbDownLoadShop_Click(object sender, EventArgs e)
        {

        }

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
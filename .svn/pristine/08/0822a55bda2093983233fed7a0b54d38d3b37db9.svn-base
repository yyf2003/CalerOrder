using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using BLL;
using Common;
using Models;

namespace WebApp.Subjects.ADOrders
{
    public partial class ImportSplitPlanDetail : BasePage
    {
        public int customerId;
        public int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);

            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);

            }
            if (Request.QueryString["import"] != null)
            {
                Panel1.Visible = true;
                if (Session["failMsg"] != null)
                {
                    labState.Text = "部分数据导入失败！";
                    ExportFailMsg.Style.Add("display", "block");
                }
            }
        }

        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "SplitOrderPlanTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            #region
            //if (!string.IsNullOrWhiteSpace(hfPlanId.Value))
            //{
            //    List<int> planIdList = StringHelper.ToIntList(hfPlanId.Value, ',');
            //    if (planIdList.Any())
            //    {
            //        if (FileUpload1.HasFile)
            //        {
            //            string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
            //            if (path != "")
            //            {
            //                OleDbConnection conn;
            //                OleDbDataAdapter da;
            //                DataSet ds = null;
            //                string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
            //                path = Server.MapPath(path);
            //                ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
            //                conn = new OleDbConnection(ExcelConnStr);
            //                conn.Open();
            //                string sql = "select * from [Sheet1$]";
            //                da = new OleDbDataAdapter(sql, conn);
            //                ds = new DataSet();
            //                da.Fill(ds);
            //                da.Dispose();
            //                conn.Close();
            //                if (ds != null && ds.Tables[0].Rows.Count > 0)
            //                {
            //                    DataColumnCollection cols = ds.Tables[0].Columns;
            //                    DataTable errorTB = CommonMethod.CreateErrorTB(cols);
            //                    SplitOrderPlanDetailBLL detailBll = new SplitOrderPlanDetailBLL();
            //                    SplitOrderPlanDetail model;
            //                    foreach (int id in planIdList)
            //                    {
            //                        foreach (DataRow dr in ds.Tables[0].Rows)
            //                        {
            //                            string orderType = dr["订单类型"].ToString();
            //                            string width = dr["宽"].ToString();
            //                            string length = dr["高"].ToString();
            //                            string material = dr["材质"].ToString();
            //                            string supplier = dr["供货方"].ToString();
            //                            string price = dr["单价"].ToString();
            //                            string count = dr["数量"].ToString();
            //                            string remark = dr["备注"].ToString();
            //                            orderType = orderType == "道具" ? "2" : "1";
            //                            int num = StringHelper.IsInt(count);
            //                            num = num == 0 ? 1 : num;
            //                            model = new SplitOrderPlanDetail();
            //                            model.AddDate = DateTime.Now;
            //                            model.AddUserId = CurrentUser.UserId;
            //                            if (!string.IsNullOrWhiteSpace(width))
            //                            {
            //                                model.GraphicLength = StringHelper.IsDecimal(width);
            //                            }
            //                            if (!string.IsNullOrWhiteSpace(length))
            //                            {
            //                                model.GraphicWidth = StringHelper.IsDecimal(length);
            //                            }
            //                            model.GraphicMaterial = material;
            //                            model.OrderType = int.Parse(orderType);
            //                            model.Quantity = num;
            //                            if (!string.IsNullOrWhiteSpace(price))
            //                                model.RackSalePrice = StringHelper.IsDecimal(price);
            //                            model.Remark = remark;
            //                            model.Supplier = supplier;
            //                            model.PlanId = id;
            //                            detailBll.Add(model);
            //                        }
            //                    }

                                
            //                }
            //            }
            //        }
            //        Response.Redirect("ImportSplitPlanDetail.aspx?import=1&customerId=" + customerId + "&subjectId=" + subjectId, false);
                
            //    }

            //}
            #endregion
            Import();
        }

        protected void lbExportError_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }


        OrderPlanBLL orderPlanBll = new OrderPlanBLL();
        SplitOrderPlanDetailBLL splitPlanBll = new SplitOrderPlanDetailBLL();
        void Import()
        {
            if (FileUpload1.HasFile)
            { 
               string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
               if (path != "")
               {
                   OleDbConnection conn;
                   OleDbDataAdapter da;
                   DataSet ds = null;
                   string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                   path = Server.MapPath(path);
                   ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                   conn = new OleDbConnection(ExcelConnStr);
                   conn.Open();

                   DataTable dt = conn.GetSchema("Tables");
                   for (int t = 0; t < dt.Rows.Count; t++)
                   {
                       string sheetName = dt.Rows[t]["TABLE_NAME"].ToString();
                       if (sheetName.IndexOf("FilterDatabase") == -1)
                       {
                           string sql = "select * from [" + sheetName + "]";
                           da = new OleDbDataAdapter(sql, conn);
                           ds = new DataSet();
                           da.Fill(ds);
                           da.Dispose();
                           conn.Close();
                           if (ds.Tables[0].Rows.Count > 0)
                           {
                               DataColumnCollection cols = ds.Tables[0].Columns;
                               //获取提条件信息
                               DataRow dr = ds.Tables[0].Rows[0];
                               string region = dr["区域"].ToString().Trim();
                               string province = dr["省份"].ToString().Trim();
                               string city = dr["城市"].ToString().Trim();
                               string positionName = dr["POP位置"].ToString().Trim();
                               string machineFrameNames = dr["器架类型"].ToString().Trim();
                               string gender = dr["性别"].ToString().Trim();
                               string cityTier = dr["城市级别"].ToString().Trim();
                               string shopNos = dr["店铺编号"].ToString().Trim();
                               string format = dr["店铺类型"].ToString().Trim();
                               string materialSupport = dr["物料支持"].ToString().Trim();
                               string POSScale = dr["店铺规模"].ToString().Trim();
                               string install = dr["安装级别"].ToString().Trim();
                               string quantity = dr["数量"].ToString().Trim();
                               string graphicNo = dr["POP编号"].ToString().Trim();
                               string graphicMaterial = dr["POP材质"].ToString().Trim();
                               string POPSize = dr["POP尺寸"].ToString().Trim();
                               string windowSize = dr["位置尺寸"].ToString().Trim();
                               string chooseImg = dr["选图"].ToString().Trim();
                               string keepPOPSize = dr["是否保留POP原尺寸"].ToString().Trim();
                               OrderPlan planModel = new OrderPlan();
                               planModel.AddDate = DateTime.Now;
                               planModel.AddUserId = CurrentUser.UserId;
                               planModel.PlanType = 1;
                               planModel.ChooseImg = chooseImg;
                               planModel.CityId = city;
                               planModel.CityTier = cityTier;
                               planModel.CustomerId = customerId;
                               planModel.Format = format;
                               planModel.Gender = gender;
                               planModel.GraphicMaterial = graphicMaterial;
                               planModel.GraphicNo = graphicNo;
                               planModel.IsInstall = install;
                               planModel.KeepPOPSize = keepPOPSize == "是" ? true : false;
                               planModel.MachineFrameNames = machineFrameNames;
                               planModel.MaterialSupport = materialSupport;
                               planModel.POPSize = POPSize;
                               planModel.PositionName = positionName;
                               planModel.POSScale = POSScale;
                               planModel.ProvinceId = province;
                               planModel.Quantity = quantity;
                               planModel.RegionNames = region;
                               planModel.ShopNos = shopNos;
                               planModel.SubjectId = subjectId;
                               planModel.WindowSize = windowSize;
                               orderPlanBll.Add(planModel);

                               for (int i = 4; i < ds.Tables[0].Rows.Count; i++)
                               {
                                   string orderType = ds.Tables[0].Rows[i][1].ToString();
                                   string width = ds.Tables[0].Rows[i][2].ToString();
                                   string length = ds.Tables[0].Rows[i][3].ToString();
                                   string material = ds.Tables[0].Rows[i][4].ToString();
                                   string supplier = ds.Tables[0].Rows[i][5].ToString();
                                   string price = ds.Tables[0].Rows[i][6].ToString();
                                   string count = ds.Tables[0].Rows[i][7].ToString();
                                   string newChooseImg = ds.Tables[0].Rows[i][8].ToString();
                                   string remark = ds.Tables[0].Rows[i][9].ToString();
                                   orderType = orderType == "道具" ? "2" : "1";
                                   int num = StringHelper.IsInt(count);
                                   num = num == 0 ? 1 : num;
                                   SplitOrderPlanDetail model = new SplitOrderPlanDetail();
                                   model.AddDate = DateTime.Now;
                                   model.AddUserId = CurrentUser.UserId;
                                   if (!string.IsNullOrWhiteSpace(width))
                                   {
                                       model.GraphicLength = StringHelper.IsDecimal(width);
                                   }
                                   if (!string.IsNullOrWhiteSpace(length))
                                   {
                                       model.GraphicWidth = StringHelper.IsDecimal(length);
                                   }
                                   model.GraphicMaterial = material;
                                   model.OrderType = int.Parse(orderType);
                                   model.Quantity = num;
                                   if (!string.IsNullOrWhiteSpace(price))
                                       model.RackSalePrice = StringHelper.IsDecimal(price);
                                   model.Remark = remark;
                                   model.Supplier = supplier;
                                   model.PlanId = planModel.Id;
                                   splitPlanBll.Add(model);
                               }
                               

                           }
                       }
                   }
                   Response.Redirect("ImportSplitPlanDetail.aspx?import=1&customerId=" + customerId + "&subjectId=" + subjectId, false);
                  
                   
               }
            
            }
        }
    }
}
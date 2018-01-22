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
    public partial class ImportMachineFrame : BasePage
    {
        int customerId = 0;
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

        /// <summary>
        /// 导出那些导入失败的信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportErrorMsg_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "店铺器架类型模板";
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
        ShopMachineFrameBLL bll = new ShopMachineFrameBLL();
        DataColumnCollection cols;
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
                        cols = ds.Tables[0].Columns;
                        DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                        ShopMachineFrame model;

                        int shopId = 0;
                        List<int> DeleteShopId = new List<int>();
                        bool isDeleteOldDate = cbDeleteOld.Checked;
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

                            string sheet = string.Empty;
                            if (cols.Contains("POP位置"))
                                sheet = dr["POP位置"].ToString().Trim();
                            else if (cols.Contains("器架类型"))
                                sheet = dr["器架类型"].ToString().Trim();

                            string frameName = string.Empty;
                            if (cols.Contains("器架名称"))
                                frameName = dr["器架名称"].ToString().Trim();
                            else if (cols.Contains("名称"))
                                frameName = dr["名称"].ToString().Trim();
                            string gender = dr["性别"].ToString().Trim();
                            string count = dr["数量"].ToString().Trim();
                            string cornerType = string.Empty;
                            if (cols.Contains("角落类型"))
                                cornerType = dr["角落类型"].ToString().Trim();
                            string levelNum = string.Empty;
                            if (cols.Contains("级别"))
                                levelNum = dr["级别"].ToString().Trim();

                            if (string.IsNullOrWhiteSpace(shopNo))
                            {
                                canSave = false;
                                errorMsg.Append("店铺编号 为空；");
                            }
                            else if (!CheckShop(shopNo,dr, out shopId))
                            {
                                canSave = false;
                                errorMsg.Append("店铺编号 不存在；");
                            }
                            if (string.IsNullOrWhiteSpace(sheet))
                            {
                                canSave = false;
                                errorMsg.Append("POP位置 为空；");
                            }
                            if (string.IsNullOrWhiteSpace(frameName))
                            {
                                canSave = false;
                                errorMsg.Append("器架名称 为空；");
                            }
                            if (!string.IsNullOrWhiteSpace(sheet) && !string.IsNullOrWhiteSpace(frameName) && frameName!="无")
                            {
                                if (!CheckFrame(sheet, frameName))
                                {
                                    canSave = false;
                                    errorMsg.Append("器架名称 填写不正确；");
                                }
                            }
                            if (string.IsNullOrWhiteSpace(count))
                            {

                                count = "1";
                            }
                            if (StringHelper.IsInt(count) == 0)
                            {

                                count = "1";
                            }
                            if (!string.IsNullOrWhiteSpace(levelNum) && !StringHelper.IsIntVal(levelNum))
                            {
                                canSave = false;
                                errorMsg.Append("级别填写不正确；");
                            }
                            if (canSave)
                            {
                                if (isDeleteOldDate && !DeleteShopId.Contains(shopId))
                                {
                                    DeleteOldMachineFrame(shopId);
                                    DeleteShopId.Add(shopId);
                                }

                                int id = 0;
                                frameName=frameName.Replace("（", "(").Replace("）", ")").ToUpper();
                                if (CheckFrameIsExist(shopId, sheet, frameName, gender, cornerType, out id))
                                {
                                    model = bll.GetModel(id);
                                    if (model != null)
                                    {
                                        model.Count = int.Parse(count);
                                        if (!string.IsNullOrWhiteSpace(levelNum))
                                            model.LevelNum = int.Parse(levelNum);
                                        else
                                            model.LevelNum = null;
                                        bll.Update(model);
                                    }
                                }
                                else
                                {
                                    model = new ShopMachineFrame() { Count = int.Parse(count), Gender = gender, MachineFrame = frameName, PositionName = sheet, ShopId = shopId, CornerType = cornerType };
                                    if (!string.IsNullOrWhiteSpace(levelNum))
                                        model.LevelNum = int.Parse(levelNum);
                                    else
                                        model.LevelNum = null;
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
                        Response.Redirect(string.Format("ImportMachineFrame.aspx?successNum={0}&failNum={1}&customerId={2}", successNum, failNum, customerId), false);

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
        Dictionary<string, int> shopDic = new Dictionary<string, int>();
        ShopBLL shopBll = new ShopBLL();
        bool CheckShop(string shopNo,DataRow dr, out int shopId)
        {
            shopId = 0;
            bool flag = false;
            if (shopDic.Keys.Contains(shopNo.ToUpper()))
            {
                shopId = shopDic[shopNo.ToUpper()];
                flag = true;
            }
            else
            {
                var model = shopBll.GetList(s => s.ShopNo == shopNo.ToUpper()).FirstOrDefault();
                if (model != null)
                    shopId = model.Id;
                else
                {
                    //shopId = AddShop(dr);
                }
                if (shopId > 0)
                {
                    shopDic.Add(shopNo.ToUpper(), shopId);
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 判定是否已存在，
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="sheet"></param>
        /// <param name="frame"></param>
        /// <param name="gender"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckFrameIsExist(int shopId, string sheet, string frame, string gender, string cornerType, out int id)
        {
            id = 0;
            var list = bll.GetList(s => s.ShopId == shopId && s.PositionName == sheet && s.MachineFrame.ToUpper() == frame && s.Gender == gender && s.CornerType == cornerType);
            if (list.Any())
            {
                id = list[0].Id;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 检查器架名称是否填写正确
        /// </summary>
        List<string> correctFrameList = new List<string>();
        bool CheckFrame(string sheet, string frameName)
        {
            bool flag = false;
            if (correctFrameList.Contains(sheet.ToUpper()+"&"+frameName.ToUpper()))
            {
                flag = true;
            }
            else
            {
                var list = new BasicMachineFrameBLL().GetList(s => s.Sheet == sheet.ToUpper() && s.MachineFrame == frameName.ToUpper());
                if (list.Any())
                {
                    correctFrameList.Add(sheet.ToUpper() + "&" + frameName.ToUpper());
                    flag = true;
                }
            }
            return flag;
        }

        int AddShop(DataRow dr)
        {
            int shopId = 0;
            string shopNo = string.Empty;
            string shopName = string.Empty;
            string region = string.Empty;
            string province = string.Empty;
            string city = string.Empty;
            string cityLevel = string.Empty;
            string isInstall = string.Empty;
            string angentCode = string.Empty;
            string angentName = string.Empty;
            string adress = string.Empty;
            string contact1 = string.Empty;
            string tel1 = string.Empty;
            string contact2 = string.Empty;
            string tel2 = string.Empty;
            string channel = string.Empty;
            string format = string.Empty;
            string locationType = string.Empty;
            string customerModel = string.Empty;
            string shopLevel = string.Empty;
            //bool canSave = true;
            StringBuilder errorMsg = new StringBuilder();
            if (cols.Contains("POS Code"))
                shopNo = dr["POS Code"].ToString().Trim();
            else if (cols.Contains("POSCode"))
                shopNo = dr["POSCode"].ToString().Trim();
            else if (cols.Contains("店铺编号"))
                shopNo = dr["店铺编号"].ToString().Trim();
            if (cols.Contains("POS Name"))
                shopName = dr["POS Name"].ToString().Trim();
            else if (cols.Contains("POSName"))
                shopName = dr["POSName"].ToString().Trim();
            else if (cols.Contains("店铺名称"))
                shopName = dr["店铺名称"].ToString().Trim();
            if (cols.Contains("Region"))
                region = dr["Region"].ToString().Trim();
            else if (cols.Contains("区域"))
                region = dr["区域"].ToString().Trim();
            if (cols.Contains("Province"))
                province = dr["Province"].ToString().Trim();
            else if (cols.Contains("省份"))
                province = dr["省份"].ToString().Trim();
            if (cols.Contains("City"))
                city = dr["City"].ToString().Trim();
            else if (cols.Contains("城市"))
                city = dr["城市"].ToString().Trim();
            if (cols.Contains("City Tier"))
                cityLevel = dr["City Tier"].ToString().Trim();
            else if (cols.Contains("CityTier"))
                cityLevel = dr["CityTier"].ToString().Trim();
            else if (cols.Contains("城市级别"))
                cityLevel = dr["城市级别"].ToString().Trim();
            if (cols.Contains("是否安装"))
                isInstall = dr["是否安装"].ToString().Trim();
            else if (cols.Contains("IsInstall"))
                isInstall = dr["IsInstall"].ToString().Trim();

            if (cols.Contains("Customer Code"))
                angentCode = dr["Customer Code"].ToString().Trim();
            else if (cols.Contains("CustomerCode"))
                angentCode = dr["CustomerCode"].ToString().Trim();
            else if (cols.Contains("经销商编号"))
                angentCode = dr["经销商编号"].ToString().Trim();

            if (cols.Contains("Customer Name"))
                angentName = dr["Customer Name"].ToString().Trim();
            else if (cols.Contains("CustomerName"))
                angentName = dr["CustomerName"].ToString().Trim();
            else if (cols.Contains("经销商名称"))
                angentName = dr["经销商名称"].ToString().Trim();
            if (cols.Contains("POPAddress"))
                adress = dr["POPAddress"].ToString().Trim();
            else if (cols.Contains("店铺地址"))
                adress = dr["店铺地址"].ToString().Trim();
            else if (cols.Contains("Address"))
                adress = dr["Address"].ToString().Trim();

            if (cols.Contains("popContact1"))
                contact1 = dr["popContact1"].ToString().Trim();
            else if (cols.Contains("Contact1"))
                contact1 = dr["Contact1"].ToString().Trim();
            else if (cols.Contains("Contact"))
                contact1 = dr["Contact"].ToString().Trim();
            else if (cols.Contains("联系人1"))
                contact1 = dr["联系人1"].ToString().Trim();

            if (cols.Contains("popTel1"))
                tel1 = dr["popTel1"].ToString().Trim();
            else if (cols.Contains("Tel1"))
                tel1 = dr["Tel1"].ToString().Trim();
            else if (cols.Contains("Tel"))
                tel1 = dr["Tel"].ToString().Trim();
            else if (cols.Contains("联系电话1"))
                tel1 = dr["联系电话1"].ToString().Trim();

            if (cols.Contains("popContact2"))
                contact2 = dr["popContact2"].ToString().Trim();
            else if (cols.Contains("Contact2"))
                contact2 = dr["Contact2"].ToString().Trim();
            else if (cols.Contains("联系人2"))
                contact2 = dr["联系人2"].ToString().Trim();

            if (cols.Contains("popTel2"))
                tel2 = dr["popTel2"].ToString().Trim();
            else if (cols.Contains("Tel2"))
                tel2 = dr["Tel2"].ToString().Trim();
            else if (cols.Contains("联系电话2"))
                tel2 = dr["联系电话2"].ToString().Trim();

            if (cols.Contains("Channel"))
                channel = dr["Channel"].ToString().Trim();
            else if (cols.Contains("产品类型"))
                channel = dr["产品类型"].ToString().Trim();

            if (cols.Contains("Format"))
                format = dr["Format"].ToString().Trim();
            else if (cols.Contains("店铺类型"))
                format = dr["店铺类型"].ToString().Trim();

            if(cols.Contains("店铺级别"))
                shopLevel = dr["店铺级别"].ToString().Trim();
            if (cols.Contains("Location Type"))
                locationType = dr["Location Type"].ToString().Trim();
            else if (cols.Contains("LocationType"))
                locationType = dr["LocationType"].ToString().Trim();
            else if (cols.Contains("店铺属性"))
                locationType = dr["店铺属性"].ToString().Trim();

            if (cols.Contains("Business Model"))
                customerModel = dr["Business Model"].ToString().Trim();
            else if (cols.Contains("BusinessModel"))
                customerModel = dr["BusinessModel"].ToString().Trim();
            else if (cols.Contains("客户类别"))
                customerModel = dr["客户类别"].ToString().Trim();


            Shop shopModel = new Models.Shop();

            shopModel.RegionName = region;
            shopModel.ProvinceName = province;
            shopModel.CityName = city;
            shopModel.AgentCode = angentCode;
            shopModel.AgentName = angentName;
            shopModel.BusinessModel = customerModel;
            shopModel.Channel = channel;
            shopModel.CityTier = cityLevel;
            shopModel.Contact1 = contact1;
            shopModel.Tel1 = tel1;
            shopModel.Contact2 = contact2;
            shopModel.Tel2 = tel2;
            shopModel.CustomerId = customerId;
            shopModel.Format = format.Replace("（", "(").Replace("）", ")"); ;
            shopModel.IsInstall = isInstall;
            shopModel.LocationType = locationType;
            shopModel.POPAddress = adress;
            shopModel.ShopName = shopName;
            shopModel.AddDate = DateTime.Now;
            shopModel.ShopNo = shopNo.ToUpper();
            shopModel.IsDelete = false;
            shopModel.ShopLevel = shopLevel;
            shopBll.Add(shopModel);
            shopId = shopModel.Id;
            return shopId;
        }


        void DeleteOldMachineFrame(int shopId)
        {
            bll.Delete(s => s.ShopId == shopId);
        }
    }
}
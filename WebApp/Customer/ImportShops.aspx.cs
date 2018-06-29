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
    public partial class ImportShops : BasePage
    {
        int customerId;
        ShopBLL shopBll = new ShopBLL();
        POPBLL popBll = new POPBLL();
        ShopMachineFrameBLL frameBll = new ShopMachineFrameBLL();
        int fileType;

        BaseDataChangeLogBLL changeLogBll = new BaseDataChangeLogBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Request.QueryString["customerId"] != null)
            //{
            //    customerId = int.Parse(Request.QueryString["customerId"]);
            //}
            int successNum = 0;
            int failNum = 0;
            if (Request.QueryString["isImport"] != null && Request.QueryString["isImport"] == "1")
            {
                if (Request.QueryString["successNum"] != null)
                {
                    successNum = int.Parse(Request.QueryString["successNum"]);
                }
                if (Request.QueryString["failNum"] != null)
                {
                    failNum = int.Parse(Request.QueryString["failNum"]);
                }
                if (Request.QueryString["fileType"] != null)
                {
                    fileType = int.Parse(Request.QueryString["fileType"]);
                }
                if (Request.QueryString["isWrong"] != null && Request.QueryString["isWrong"] == "1")
                {
                    ImportState.Style.Add("display", "block");
                    labState.Text = Request.QueryString["wrongMsg"];
                    labTotalNum.Text = (successNum + failNum) + "条";
                    labSuccessNum.Text = successNum + "条";
                    labFailNum.Text = failNum + "条";
                }
                else
                {
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
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
            }
        }

        int successNum = 0;
        int failNum = 0;
        string wrongMsg = string.Empty;
        int isWrong = 0;
        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    if (Session["errorTb"] != null)
                        Session["errorTb"] = null;
                    customerId = int.Parse(ddlCustomer.SelectedValue);
                    string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                    OleDbConnection conn;
                    OleDbDataAdapter da;
                    DataSet ds = null;
                    path = Server.MapPath(path);
                    ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                    conn = new OleDbConnection(ExcelConnStr);
                    conn.Open();
                    
                    
                    string fileType = rblImportType.SelectedValue;
                    switch (fileType)
                    {
                        case "1"://店铺信息

                            string sql1 = "select * from [pop$]";
                            da = new OleDbDataAdapter(sql1, conn);
                            ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            ImportShop(ds);
                            regionList.Clear();
                            placeList.Clear();
                            break;
                        case "2":
                            string sql2 = "select * from [pop$]";
                            da = new OleDbDataAdapter(sql2, conn);
                            ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            ImportPOP(ds);
                            positionList.Clear();
                            break;
                        case "3":
                            string sql3 = "select * from [器架$]";
                            da = new OleDbDataAdapter(sql3, conn);
                            ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            ImportMachineFrame(ds);
                            break;
                        case "4":
                            string sql4 = "select * from [Sheet1$]";
                            da = new OleDbDataAdapter(sql4, conn);
                            ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            ImportShopCharges(ds);
                            break;
                        case "5":
                            string sql5 = "select * from [Sheet1$]";
                            da = new OleDbDataAdapter(sql5, conn);
                            ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            ImportOutsourceInstallPrice(ds);
                            break;
                        case "6":
                            string sql6 = "select * from [Sheet1$]";
                            da = new OleDbDataAdapter(sql6, conn);
                            ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            UpdateCustomerServiceId(ds);
                            break;
                    }
                    
                    conn.Dispose();
                    conn.Close();
                    Response.Redirect(string.Format("ImportShops.aspx?isImport=1&customerId={0}&fileType={1}&successNum={2}&failNum={3}&isWrong={4}&wrongMsg={5}", customerId, fileType, successNum, failNum,isWrong,wrongMsg), false);
                }
            }
        }

        //导入店铺信息
        void ImportShop(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {


                Dictionary<string, string> regionChangeDic = new Dictionary<string, string>();
                string tempStr = string.Empty;
                try
                {
                    tempStr = ConfigurationManager.AppSettings["RegionChangeLanguage"];
                }
                catch { }
                if (!string.IsNullOrWhiteSpace(tempStr))
                {
                    List<string> list1 = StringHelper.ToStringList(tempStr,'|');
                    if (list1.Any())
                    {
                        list1.ForEach(s => {
                            string eng = s.Split(':')[0];
                            string ch = s.Split(':')[1];
                            if (!regionChangeDic.Keys.Contains(eng))
                            {
                                regionChangeDic.Add(eng, ch);
                            }
                        });
                    }
                }
                BaseDataChangeLogBLL changeLogBll = new BaseDataChangeLogBLL();
                ShopChangeDetailBLL detailBll = new ShopChangeDetailBLL();
                ShopChangeDetail shopChangeDetailModel;

                BaseDataChangeLog logModel = new BaseDataChangeLog();
                logModel.AddDate = DateTime.Now;
                logModel.AddUserId = new BasePage().CurrentUser.UserId;
                logModel.ItemType = (int)BaseDataChangeItemEnum.Shop;
                logModel.ChangeType = (int)DataChangeTypeEnum.Add;
                changeLogBll.Add(logModel);


                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable errorTB = Common.CommonMethod.CreateErrorTB(cols);
                List<string> shopNosFinished = new List<string>();
                Models.Shop shopModel;
                bool isexist = false;
                int shopId = 0;
                int regionId = 0;
                int provinceId = 0;
                int cityId = 0;
                int countyId = 0;
                int csUserId = 0;
                int outsourceId = 0;//外协
                string shopNo = string.Empty;
                string newShopNo = string.Empty;
                string shopName = string.Empty;
                string region = string.Empty;
                string province = string.Empty;
                string city = string.Empty;
                string county = string.Empty;
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
                string openDate = string.Empty;
                string status = string.Empty;
                string remark = string.Empty;
                string csUserName = string.Empty;
                string shopLevel = string.Empty;
                string installPrice = string.Empty;//SP特殊安装费（T4-T7）
                string osInstallPrice = string.Empty;//SP外协特殊安装费（T4-T7）
                string bcsInstallPrice = string.Empty;//三叶草特殊安装费（T4-T7）
                string osBCSInstallPrice = string.Empty;//三叶草外协特殊安装费（T4-T7）
                string category = string.Empty;
                string shopType = string.Empty;
                string outsourceName = string.Empty;//外协名称
                //string windowInstallPrice = string.Empty;
                List<string> shopNoList = new List<string>();
                bool isNoCheckType = cbNoCheckType.Checked;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    isexist = false;
                    shopId = 0;
                    regionId = 0;
                    provinceId = 0;
                    cityId = 0;
                    countyId = 0;
                    csUserId = 0;
                    outsourceId = 0;
                    shopNo = string.Empty;
                    newShopNo = string.Empty;
                    shopName = string.Empty;
                    region = string.Empty;
                    province = string.Empty;
                    city = string.Empty;
                    county = string.Empty;
                    cityLevel = string.Empty;
                    isInstall = string.Empty;
                    angentCode = string.Empty;
                    angentName = string.Empty;
                    adress = string.Empty;
                    contact1 = string.Empty;
                    tel1 = string.Empty;
                    contact2 = string.Empty;
                    tel2 = string.Empty;
                    channel = string.Empty;
                    format = string.Empty;
                    locationType = string.Empty;
                    customerModel = string.Empty;
                    openDate = string.Empty;
                    status = string.Empty;
                    remark = string.Empty;
                    csUserName = string.Empty;
                    shopLevel = string.Empty;
                    installPrice = string.Empty;

                    osInstallPrice = string.Empty;
                    bcsInstallPrice = string.Empty;
                    osBCSInstallPrice = string.Empty;
                    outsourceName = string.Empty;
                    shopType = string.Empty;
                    bool canSave = true;
                    StringBuilder errorMsg = new StringBuilder();

                    if (cols.Contains("POS Code"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POS Code"].ToString().Trim());
                    else if (cols.Contains("POSCode"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POSCode"].ToString().Trim());
                    else if (cols.Contains("店铺编号"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["店铺编号"].ToString().Trim());

                    if (cols.Contains("新店铺编号"))
                        newShopNo = StringHelper.ReplaceSpecialChar(dr["新店铺编号"].ToString().Trim());

                    if (cols.Contains("客服名称"))
                        csUserName = StringHelper.ReplaceSpecialChar(dr["客服名称"].ToString().Trim());
                    else if (cols.Contains("客服"))
                        csUserName = StringHelper.ReplaceSpecialChar(dr["客服"].ToString().Trim());
                    else if (cols.Contains("客服姓名"))
                        csUserName = StringHelper.ReplaceSpecialChar(dr["客服姓名"].ToString().Trim());

                    if (cols.Contains("POS Name"))
                        shopName = StringHelper.ReplaceSpecialChar(dr["POS Name"].ToString().Trim());
                    else if (cols.Contains("POSName"))
                        shopName = StringHelper.ReplaceSpecialChar(dr["POSName"].ToString().Trim());
                    else if (cols.Contains("店铺名称"))
                        shopName = StringHelper.ReplaceSpecialChar(dr["店铺名称"].ToString().Trim());
                    if (cols.Contains("Region"))
                        region = StringHelper.ReplaceSpecialChar(dr["Region"].ToString().Trim());
                    else if (cols.Contains("区域"))
                        region = StringHelper.ReplaceSpecialChar(dr["区域"].ToString().Trim());
                    if (cols.Contains("Province"))
                        province = StringHelper.ReplaceSpecialChar(dr["Province"].ToString().Trim());
                    else if (cols.Contains("省份"))
                        province = StringHelper.ReplaceSpecialChar(dr["省份"].ToString().Trim());
                    if (cols.Contains("City"))
                        city = StringHelper.ReplaceSpecialChar(dr["City"].ToString().Trim());
                    else if (cols.Contains("城市"))
                        city = StringHelper.ReplaceSpecialChar(dr["城市"].ToString().Trim());
                    if (cols.Contains("County"))
                        county = StringHelper.ReplaceSpecialChar(dr["County"].ToString().Trim());
                    else if (cols.Contains("区"))
                        county = StringHelper.ReplaceSpecialChar(dr["区"].ToString().Trim());
                    else if (cols.Contains("区县"))
                        county = StringHelper.ReplaceSpecialChar(dr["区县"].ToString().Trim());
                    else if (cols.Contains("区/县"))
                        county = StringHelper.ReplaceSpecialChar(dr["区/县"].ToString().Trim());
                    else if (cols.Contains("县"))
                        county = StringHelper.ReplaceSpecialChar(dr["县"].ToString().Trim());
                    if (cols.Contains("City Tier"))
                        cityLevel = StringHelper.ReplaceSpecialChar(dr["City Tier"].ToString().Trim());
                    else if (cols.Contains("CityTier"))
                        cityLevel = StringHelper.ReplaceSpecialChar(dr["CityTier"].ToString().Trim());
                    else if (cols.Contains("城市级别"))
                        cityLevel = StringHelper.ReplaceSpecialChar(dr["城市级别"].ToString().Trim());
                    if (cols.Contains("是否安装"))
                        isInstall = StringHelper.ReplaceSpecialChar(dr["是否安装"].ToString().Trim());
                    else if (cols.Contains("IsInstall"))
                        isInstall = StringHelper.ReplaceSpecialChar(dr["IsInstall"].ToString().Trim());
                    else if (cols.Contains("安装级别"))
                        isInstall = StringHelper.ReplaceSpecialChar(dr["安装级别"].ToString().Trim());
                    if (cols.Contains("Customer Code"))
                        angentCode = StringHelper.ReplaceSpecialChar(dr["Customer Code"].ToString().Trim());
                    else if (cols.Contains("CustomerCode"))
                        angentCode = StringHelper.ReplaceSpecialChar(dr["CustomerCode"].ToString().Trim());
                    else if (cols.Contains("经销商编号"))
                        angentCode = StringHelper.ReplaceSpecialChar(dr["经销商编号"].ToString().Trim());
                    if (cols.Contains("Customer Name"))
                        angentName = StringHelper.ReplaceSpecialChar(dr["Customer Name"].ToString().Trim());
                    else if (cols.Contains("CustomerName"))
                        angentName = StringHelper.ReplaceSpecialChar(dr["CustomerName"].ToString().Trim());
                    else if (cols.Contains("经销商名称"))
                        angentName = StringHelper.ReplaceSpecialChar(dr["经销商名称"].ToString().Trim());
                    if (cols.Contains("POPAddress"))
                        adress = StringHelper.ReplaceSpecialChar(dr["POPAddress"].ToString().Trim());
                    else if (cols.Contains("店铺地址"))
                        adress = StringHelper.ReplaceSpecialChar(dr["店铺地址"].ToString().Trim());
                    else if (cols.Contains("Address"))
                        adress = StringHelper.ReplaceSpecialChar(dr["Address"].ToString().Trim());

                    if (cols.Contains("popContact1"))
                        contact1 = StringHelper.ReplaceSpecialChar(dr["popContact1"].ToString().Trim());
                    else if (cols.Contains("Contact1"))
                        contact1 = StringHelper.ReplaceSpecialChar(dr["Contact1"].ToString().Trim());
                    else if (cols.Contains("Contact"))
                        contact1 = StringHelper.ReplaceSpecialChar(dr["Contact"].ToString().Trim());
                    else if (cols.Contains("联系人1"))
                        contact1 = StringHelper.ReplaceSpecialChar(dr["联系人1"].ToString().Trim());

                    if (cols.Contains("popTel1"))
                        tel1 = StringHelper.ReplaceSpecialChar(dr["popTel1"].ToString().Trim());
                    else if (cols.Contains("Tel1"))
                        tel1 = StringHelper.ReplaceSpecialChar(dr["Tel1"].ToString().Trim());
                    else if (cols.Contains("Tel"))
                        tel1 = StringHelper.ReplaceSpecialChar(dr["Tel"].ToString().Trim());
                    else if (cols.Contains("联系电话1"))
                        tel1 = StringHelper.ReplaceSpecialChar(dr["联系电话1"].ToString().Trim());

                    if (cols.Contains("popContact2"))
                        contact2 = StringHelper.ReplaceSpecialChar(dr["popContact2"].ToString().Trim());
                    else if (cols.Contains("Contact2"))
                        contact2 = StringHelper.ReplaceSpecialChar(dr["Contact2"].ToString().Trim());
                    else if (cols.Contains("联系人2"))
                        contact2 = StringHelper.ReplaceSpecialChar(dr["联系人2"].ToString().Trim());

                    if (cols.Contains("popTel2"))
                        tel2 = StringHelper.ReplaceSpecialChar(dr["popTel2"].ToString().Trim());
                    else if (cols.Contains("Tel2"))
                        tel2 = StringHelper.ReplaceSpecialChar(dr["Tel2"].ToString().Trim());
                    else if (cols.Contains("联系电话2"))
                        tel2 = StringHelper.ReplaceSpecialChar(dr["联系电话2"].ToString().Trim());

                    if (cols.Contains("Channel"))
                        channel = StringHelper.ReplaceSpecialChar(dr["Channel"].ToString().Trim());
                    else if (cols.Contains("产品类型"))
                        channel = StringHelper.ReplaceSpecialChar(dr["产品类型"].ToString().Trim());

                    if (cols.Contains("Format"))
                        format = StringHelper.ReplaceSpecialChar(dr["Format"].ToString().Trim());
                    else if (cols.Contains("店铺类型"))
                        format = StringHelper.ReplaceSpecialChar(dr["店铺类型"].ToString().Trim());
                    if (cols.Contains("店铺级别"))
                        shopLevel = StringHelper.ReplaceSpecialChar(dr["店铺级别"].ToString().Trim());
                    else if (cols.Contains("ShopLevel"))
                        shopLevel = StringHelper.ReplaceSpecialChar(dr["ShopLevel"].ToString().Trim());
                    else if (cols.Contains("店铺规模大小"))
                        shopLevel = StringHelper.ReplaceSpecialChar(dr["店铺规模大小"].ToString().Trim());
                    else if (cols.Contains("店铺规模"))
                        shopLevel = StringHelper.ReplaceSpecialChar(dr["店铺规模"].ToString().Trim());
                    if (cols.Contains("Location Type"))
                        locationType = StringHelper.ReplaceSpecialChar(dr["Location Type"].ToString().Trim());
                    else if (cols.Contains("LocationType"))
                        locationType = StringHelper.ReplaceSpecialChar(dr["LocationType"].ToString().Trim());
                    else if (cols.Contains("店铺属性"))
                        locationType = StringHelper.ReplaceSpecialChar(dr["店铺属性"].ToString().Trim());

                    if (cols.Contains("Business Model"))
                        customerModel = StringHelper.ReplaceSpecialChar(dr["Business Model"].ToString().Trim());
                    else if (cols.Contains("BusinessModel"))
                        customerModel = StringHelper.ReplaceSpecialChar(dr["BusinessModel"].ToString().Trim());
                    else if (cols.Contains("客户类别"))
                        customerModel = StringHelper.ReplaceSpecialChar(dr["客户类别"].ToString().Trim());


                    if (cols.Contains("Opening Date"))
                        openDate = dr["Opening Date"].ToString().Trim();
                    else if (cols.Contains("OpeningDate"))
                        openDate = dr["OpeningDate"].ToString().Trim();
                    else if (cols.Contains("开店日期"))
                        openDate = dr["开店日期"].ToString().Trim();
                    if (openDate.Contains("0:00:00"))
                        openDate = openDate.Replace("0:00:00", "");
                    if (cols.Contains("Status"))
                        status = StringHelper.ReplaceSpecialChar(dr["Status"].ToString().Trim());
                    else if (cols.Contains("店铺状态"))
                        status = StringHelper.ReplaceSpecialChar(dr["店铺状态"].ToString().Trim());
                   
                    
                    
                    if (cols.Contains("特殊安装费"))
                        installPrice = dr["特殊安装费"].ToString().Trim();
                    else if (cols.Contains("特殊基础安装费"))
                        installPrice = dr["特殊基础安装费"].ToString().Trim();
                    else if (cols.Contains("SP特殊基础安装费"))
                        installPrice = dr["SP特殊基础安装费"].ToString().Trim();
                    else if (cols.Contains("SP特殊安装费"))
                        installPrice = dr["SP特殊安装费"].ToString().Trim();
                    //if (cols.Contains("橱窗安装费"))
                    //    windowInstallPrice = dr["橱窗安装费"].ToString().Trim();

                    if (cols.Contains("外协特殊安装费"))
                        osInstallPrice = dr["外协特殊安装费"].ToString().Trim();
                    else if (cols.Contains("SP外协安装费"))
                        osInstallPrice = dr["SP外协安装费"].ToString().Trim();
                    else if (cols.Contains("SP外协特殊安装费"))
                        osInstallPrice = dr["SP外协特殊安装费"].ToString().Trim();

                    if (cols.Contains("三叶草特殊安装费"))
                        bcsInstallPrice = dr["三叶草特殊安装费"].ToString().Trim();
                    else if (cols.Contains("三叶草安装费"))
                        bcsInstallPrice = dr["三叶草安装费"].ToString().Trim();

                    if (cols.Contains("三叶草外协特殊安装费"))
                        osBCSInstallPrice = dr["三叶草外协特殊安装费"].ToString().Trim();
                    else if (cols.Contains("三叶草外协安装费"))
                        osBCSInstallPrice = dr["三叶草外协安装费"].ToString().Trim();


                    if (cols.Contains("活动常规"))
                        category = StringHelper.ReplaceSpecialChar(dr["活动常规"].ToString().Trim());

                    if (cols.Contains("店铺渠道"))
                        shopType = StringHelper.ReplaceSpecialChar(dr["店铺渠道"].ToString().Trim());
                    else if (cols.Contains("渠道"))
                        shopType = StringHelper.ReplaceSpecialChar(dr["渠道"].ToString().Trim());

                    if (cols.Contains("外协名称"))
                        outsourceName = StringHelper.ReplaceSpecialChar(dr["外协名称"].ToString().Trim());
                    else if (cols.Contains("外协"))
                        outsourceName = StringHelper.ReplaceSpecialChar(dr["外协"].ToString().Trim());

                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号为空 ；");
                    }
                    else if (shopNoList.Contains(shopNo.ToUpper()))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号重复 ；");
                    }
                    if (!string.IsNullOrWhiteSpace(newShopNo) && CheckNewShopNo(newShopNo))
                    {
                        canSave = false;
                        errorMsg.Append("新店铺编号已存在 ；");
                    }
                    if (string.IsNullOrWhiteSpace(shopName))
                    {
                        //canSave = false;
                        //errorMsg.Append("店铺名称为空 ；");
                    }
                    
                    if (string.IsNullOrWhiteSpace(region))
                    {
                        canSave = false;
                        errorMsg.Append("区域为空 ；");
                    }
                    else
                    {
                        if (regionChangeDic.Keys.Count > 0 && regionChangeDic.Keys.Contains(region))
                        {
                            region = regionChangeDic[region];
                        }
                        region = region.Substring(0, 1).ToUpper() + region.Substring(1).ToLower(); 
                    }
                    
                    if (!string.IsNullOrWhiteSpace(province))
                    {
                        provinceId = GetProvinceId(province);
                        if (provinceId > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(city))
                            {
                                cityId = GetCityId(city, provinceId);
                                if (cityId > 0)
                                {
                                    if (!string.IsNullOrWhiteSpace(county))
                                    {
                                        countyId = GetCityId(county, cityId);
                                        if (countyId == 0)
                                        {
                                            //canSave = false;
                                            errorMsg.Append("区县填写填写不正确(已导入) ；");
                                        }
                                    }
                                }
                                else
                                {
                                    string newCity = string.Empty;
                                    GetCity(city, out newCity, out cityId, out countyId);
                                    if (cityId == 0)
                                    {
                                        canSave = false;
                                        errorMsg.Append("城市填写填写不正确 ；");
                                    }
                                    else
                                    {
                                        county = city;
                                        city = newCity;
                                    }
                                }
                            }
                        }
                        else
                        {
                            canSave = false;
                            errorMsg.Append("省份填写填写不正确 ；");
                        }
                    }
                    //if (!string.IsNullOrWhiteSpace(installPrice) && !StringHelper.IsDecimalVal(installPrice))
                    //{
                    //    canSave = false;
                    //    errorMsg.Append("基础安装费填写不正确 ；");
                    //}
                    //if (!string.IsNullOrWhiteSpace(windowInstallPrice) && !StringHelper.IsDecimalVal(windowInstallPrice))
                    //{
                    //    canSave = false;
                    //    errorMsg.Append("橱窗安装费填写不正确 ；");
                    //}
                    
                    if (!isNoCheckType)
                    {
                        if (string.IsNullOrWhiteSpace(channel))
                        {
                            canSave = false;
                            errorMsg.Append("channel为空 ；");
                        }
                        if (string.IsNullOrWhiteSpace(format))
                        {
                            canSave = false;
                            errorMsg.Append("format为空；");
                        }
                        if (string.IsNullOrWhiteSpace(cityLevel))
                        {
                            canSave = false;
                            errorMsg.Append("城市级别为空；");
                        }
                        if (string.IsNullOrWhiteSpace(isInstall))
                        {
                            canSave = false;
                            errorMsg.Append("是否安装为空；");
                        }
                    }
                    if (isInstall == "是" || isInstall == "安装")
                    {
                        isInstall = "Y";
                    }
                    if (isInstall == "否" || isInstall == "不安装" || isInstall == "不" || isInstall == "不是")
                    {
                        isInstall = "N";
                    }
                    if (!string.IsNullOrWhiteSpace(csUserName))
                    {
                        if (!CheckUserId(csUserName, out csUserId))
                        {
                            //canSave = false;
                            errorMsg.AppendFormat("客服{0}不存在，请先创建用户；", csUserName);
                        }
                    }
                    
                    bool isShut = false;
                    bool isInstall0 = false;
                    isexist = CheckShop(shopNo, out shopId, out isShut, out isInstall0);
                    if (isexist)
                    {

                    }
                    else if (region.ToLower()!="west" && string.IsNullOrWhiteSpace(outsourceName))
                    {
                        canSave = false;
                        errorMsg.Append("请填写外协名称；");
                    }
                    if (!string.IsNullOrWhiteSpace(outsourceName))
                    {
                        if (!GetOutsourceName(outsourceName, out outsourceId))
                        {
                            canSave = false;
                            errorMsg.Append("外协名称填写不正确；");
                        }
                    }
                    if (!isexist && !string.IsNullOrWhiteSpace(shopName) && ShopNameIsExist(shopName))
                    {
                        canSave = false;
                        errorMsg.Append("店铺名称已存在；");
                    }
                    if (canSave)
                    {
                        Shop oldModel = null;
                        shopNoList.Add(shopNo.ToUpper());
                        
                        if (isShut)
                            errorMsg.Append("该店铺已经删除；");
                        shopModel = new Models.Shop();
                        if (isexist)
                        {
                            shopModel = shopBll.GetModel(shopId);
                            oldModel = shopModel;
                            if (shopModel == null)
                            {
                                isexist = false;
                                shopModel = new Models.Shop();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(region))
                            shopModel.RegionName = region;
                        if (provinceId > 0)
                        {
                            shopModel.ProvinceId = provinceId;
                            shopModel.ProvinceName = province;
                        }
                        if (cityId > 0)
                        {
                            shopModel.CityId = cityId;
                            shopModel.CityName = city;
                        }
                        if (countyId > 0)
                        {
                            shopModel.AreaId = countyId;
                            shopModel.AreaName = county;
                        }

                        if (!string.IsNullOrWhiteSpace(newShopNo))
                            shopModel.ShopNo = newShopNo.ToUpper();

                        if (!string.IsNullOrWhiteSpace(angentCode))
                            shopModel.AgentCode = angentCode;
                        if (!string.IsNullOrWhiteSpace(angentName))
                            shopModel.AgentName = angentName;
                        if (!string.IsNullOrWhiteSpace(customerModel))
                            shopModel.BusinessModel = customerModel;
                        if (!string.IsNullOrWhiteSpace(channel))
                            shopModel.Channel = channel;
                        if (!string.IsNullOrWhiteSpace(cityLevel))
                            shopModel.CityTier = cityLevel;
                        if (!string.IsNullOrWhiteSpace(contact1))
                            shopModel.Contact1 = contact1;
                        if (!string.IsNullOrWhiteSpace(tel1))
                            shopModel.Tel1 = tel1;
                        if (!string.IsNullOrWhiteSpace(contact2))
                            shopModel.Contact2 = contact2;
                        if (!string.IsNullOrWhiteSpace(tel2))
                            shopModel.Tel2 = tel2;
                        shopModel.CustomerId = customerId;
                        if (!string.IsNullOrWhiteSpace(format))
                            shopModel.Format = format.Replace("（", "(").Replace("）", ")");
                        if (!string.IsNullOrWhiteSpace(isInstall))
                            shopModel.IsInstall = isInstall.ToUpper();
                        if (!string.IsNullOrWhiteSpace(locationType))
                            shopModel.LocationType = locationType;
                        if (StringHelper.IsDateTime(openDate))
                            shopModel.OpeningDate = DateTime.Parse(openDate);
                        if (!string.IsNullOrWhiteSpace(adress))
                            shopModel.POPAddress = adress;
                        if (!string.IsNullOrWhiteSpace(remark))
                            shopModel.Remark = remark;
                        if (!string.IsNullOrWhiteSpace(shopName))
                            shopModel.ShopName = shopName;
                        if (!string.IsNullOrWhiteSpace(status))
                        {
                            if (status.Contains("闭"))
                                status = "关闭";
                            shopModel.Status = status;
                        }
                        if (csUserId > 0)
                            shopModel.CSUserId = csUserId;
                        if (!string.IsNullOrWhiteSpace(shopLevel))
                            shopModel.POSScale = shopLevel;
                        if (!string.IsNullOrWhiteSpace(installPrice) && StringHelper.IsDecimalVal(installPrice))
                            shopModel.BasicInstallPrice = StringHelper.IsDecimal(installPrice);


                        if (!string.IsNullOrWhiteSpace(osInstallPrice) && StringHelper.IsDecimalVal(osInstallPrice))
                            shopModel.OutsourceInstallPrice = StringHelper.IsDecimal(osInstallPrice);

                        if (!string.IsNullOrWhiteSpace(bcsInstallPrice) && StringHelper.IsDecimalVal(bcsInstallPrice))
                            shopModel.BCSInstallPrice = StringHelper.IsDecimal(bcsInstallPrice);

                        if (!string.IsNullOrWhiteSpace(osBCSInstallPrice) && StringHelper.IsDecimalVal(osBCSInstallPrice))
                            shopModel.OutsourceBCSInstallPrice = StringHelper.IsDecimal(osBCSInstallPrice);
                        if (!string.IsNullOrWhiteSpace(category))
                            shopModel.Category = category;
                        if (!string.IsNullOrWhiteSpace(shopType))
                            shopModel.ShopType = shopType;
                        if (outsourceId > 0)
                            shopModel.OutsourceId = outsourceId;
                        if (isexist)
                        {
                            shopBll.Update(shopModel);


                            shopChangeDetailModel = new ShopChangeDetail();
                            shopChangeDetailModel.ShopId = oldModel.Id;
                            shopChangeDetailModel.AgentCode = oldModel.AgentCode;
                            shopChangeDetailModel.AgentName = oldModel.AgentName;
                            shopChangeDetailModel.AreaName = oldModel.AreaName;
                            shopChangeDetailModel.BasicInstallPrice = oldModel.BasicInstallPrice;
                            shopChangeDetailModel.BusinessModel = oldModel.BusinessModel;
                            shopChangeDetailModel.Category = oldModel.Category;
                            shopChangeDetailModel.Channel = oldModel.Channel;
                            shopChangeDetailModel.CityName = oldModel.CityName;
                            shopChangeDetailModel.CityTier = oldModel.CityTier;
                            shopChangeDetailModel.Contact1 = oldModel.Contact1;
                            shopChangeDetailModel.Contact2 = oldModel.Contact2;
                            shopChangeDetailModel.CustomerId = oldModel.CustomerId;
                            shopChangeDetailModel.Format = oldModel.Format;
                            shopChangeDetailModel.IsInstall = oldModel.IsInstall;
                            shopChangeDetailModel.LocationType = oldModel.LocationType;
                            shopChangeDetailModel.LogId = logModel.Id;
                            shopChangeDetailModel.POPAddress = oldModel.POPAddress;
                            shopChangeDetailModel.ProvinceName = oldModel.ProvinceName;
                            shopChangeDetailModel.RegionName = oldModel.RegionName;
                            shopChangeDetailModel.Remark = oldModel.Remark;
                            shopChangeDetailModel.ShopName = oldModel.ShopName;
                            shopChangeDetailModel.ShopNo = oldModel.ShopNo;
                            shopChangeDetailModel.ShopType = oldModel.ShopType;
                            shopChangeDetailModel.Status = oldModel.Status;
                            shopChangeDetailModel.Tel1 = oldModel.Tel1;
                            shopChangeDetailModel.Tel2 = oldModel.Tel2;
                            shopChangeDetailModel.CSUserId = oldModel.CSUserId;
                            shopChangeDetailModel.ChangeType = "修改前";
                            shopChangeDetailModel.AddDate = DateTime.Now;
                            shopChangeDetailModel.AddUserId = CurrentUser.UserId;
                            detailBll.Add(shopChangeDetailModel);

                            //修改后
                            shopChangeDetailModel = new ShopChangeDetail();
                            shopChangeDetailModel.ShopId = shopModel.Id;
                            shopChangeDetailModel.AgentCode = shopModel.AgentCode;
                            shopChangeDetailModel.AgentName = shopModel.AgentName;
                            shopChangeDetailModel.AreaName = shopModel.AreaName;
                            shopChangeDetailModel.BasicInstallPrice = shopModel.BasicInstallPrice;
                            shopChangeDetailModel.BusinessModel = shopModel.BusinessModel;
                            shopChangeDetailModel.Category = shopModel.Category;
                            shopChangeDetailModel.Channel = shopModel.Channel;
                            shopChangeDetailModel.CityName = shopModel.CityName;
                            shopChangeDetailModel.CityTier = shopModel.CityTier;
                            shopChangeDetailModel.Contact1 = shopModel.Contact1;
                            shopChangeDetailModel.Contact2 = shopModel.Contact2;
                            shopChangeDetailModel.CustomerId = shopModel.CustomerId;
                            shopChangeDetailModel.Format = shopModel.Format;
                            shopChangeDetailModel.IsInstall = shopModel.IsInstall;
                            shopChangeDetailModel.LocationType = shopModel.LocationType;
                            shopChangeDetailModel.LogId = logModel.Id;
                            shopChangeDetailModel.POPAddress = shopModel.POPAddress;
                            shopChangeDetailModel.ProvinceName = shopModel.ProvinceName;
                            shopChangeDetailModel.RegionName = shopModel.RegionName;
                            shopChangeDetailModel.Remark = shopModel.Remark;
                            shopChangeDetailModel.ShopName = shopModel.ShopName;
                            shopChangeDetailModel.ShopNo = shopModel.ShopNo;
                            shopChangeDetailModel.ShopType = shopModel.ShopType;
                            shopChangeDetailModel.Status = shopModel.Status;
                            shopChangeDetailModel.Tel1 = shopModel.Tel1;
                            shopChangeDetailModel.Tel2 = shopModel.Tel2;
                            shopChangeDetailModel.CSUserId = shopModel.CSUserId;
                            shopChangeDetailModel.ChangeType = "修改后";
                            shopChangeDetailModel.AddDate = DateTime.Now;
                            shopChangeDetailModel.AddUserId = CurrentUser.UserId;
                            detailBll.Add(shopChangeDetailModel);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(shopName))
                            {
                                shopModel.AddDate = DateTime.Now;
                                shopModel.ShopNo = shopNo.ToUpper();
                                shopModel.IsDelete = false;
                                shopBll.Add(shopModel);
                                shopChangeDetailModel = new ShopChangeDetail();
                                shopChangeDetailModel.ShopId = shopModel.Id;
                                shopChangeDetailModel.AgentCode = shopModel.AgentCode;
                                shopChangeDetailModel.AgentName = shopModel.AgentName;
                                shopChangeDetailModel.AreaName = shopModel.AreaName;
                                shopChangeDetailModel.BasicInstallPrice = shopModel.BasicInstallPrice;
                                shopChangeDetailModel.BusinessModel = shopModel.BusinessModel;
                                shopChangeDetailModel.Category = shopModel.Category;
                                shopChangeDetailModel.Channel = shopModel.Channel;
                                shopChangeDetailModel.CityName = shopModel.CityName;
                                shopChangeDetailModel.CityTier = shopModel.CityTier;
                                shopChangeDetailModel.Contact1 = shopModel.Contact1;
                                shopChangeDetailModel.Contact2 = shopModel.Contact2;
                                shopChangeDetailModel.CustomerId = shopModel.CustomerId;
                                shopChangeDetailModel.Format = shopModel.Format;
                                shopChangeDetailModel.IsInstall = shopModel.IsInstall;
                                shopChangeDetailModel.LocationType = shopModel.LocationType;
                                shopChangeDetailModel.LogId = logModel.Id;
                                shopChangeDetailModel.POPAddress = shopModel.POPAddress;
                                shopChangeDetailModel.ProvinceName = shopModel.ProvinceName;
                                shopChangeDetailModel.RegionName = shopModel.RegionName;
                                shopChangeDetailModel.Remark = shopModel.Remark;
                                shopChangeDetailModel.ShopName = shopModel.ShopName;
                                shopChangeDetailModel.ShopNo = shopModel.ShopNo;
                                shopChangeDetailModel.ShopType = shopModel.ShopType;
                                shopChangeDetailModel.Status = shopModel.Status;
                                shopChangeDetailModel.Tel1 = shopModel.Tel1;
                                shopChangeDetailModel.Tel2 = shopModel.Tel2;
                                shopChangeDetailModel.CSUserId = shopModel.CSUserId;
                                shopChangeDetailModel.ChangeType = "新增";
                                shopChangeDetailModel.AddDate = DateTime.Now;
                                shopChangeDetailModel.AddUserId = CurrentUser.UserId;
                                detailBll.Add(shopChangeDetailModel);

                            }



                        }
                        AddRegion(region);
                        successNum++;
                    }
                    if (errorMsg.Length>0)
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
            }
        }
        //导入pop信息
        void ImportPOP(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                BaseDataChangeLogBLL changeLogBll = new BaseDataChangeLogBLL();
                POPChangeDetailBLL detailBll = new POPChangeDetailBLL();
                POPChangeDetail changeDetailModel;

                BaseDataChangeLog logModel = new BaseDataChangeLog();
                logModel.AddDate = DateTime.Now;
                logModel.AddUserId = new BasePage().CurrentUser.UserId;
                logModel.ItemType = (int)BaseDataChangeItemEnum.POP;
                logModel.ChangeType = (int)DataChangeTypeEnum.Add;
                changeLogBll.Add(logModel);

                string shopNo = string.Empty;
                string Sheet = string.Empty;
                string GraphicNo = string.Empty;
                string POPName = string.Empty;
                string POPType = string.Empty;
                string Style = string.Empty;
                string CornerType = string.Empty;
                string Category = string.Empty;
                string StandardDimension = string.Empty;
                string Modula = string.Empty;
                string Gender = string.Empty;
                string Quantity = string.Empty;
                string WindowWide = string.Empty;
                string WindowHigh = string.Empty;
                string WindowDeep = string.Empty;
                string WindowSize = string.Empty;
                string GraphicWidth = string.Empty;
                string GraphicLength = string.Empty;
                string DoubleFace = string.Empty;
                string GraphicMaterial = string.Empty;
                string Glass = string.Empty;
                string Backdrop = string.Empty;
                string ModulaQuantityWidth = string.Empty;
                string Frame = string.Empty;
                string ModulaQuantityHeight = string.Empty;
                string PositionDescription = string.Empty;
                string PlatformLength = string.Empty;
                string PlatformWidth = string.Empty;
                string PlatformHeight = string.Empty;
                string Area = string.Empty;
                string UnitPrice = string.Empty;
                string ExpireDate = string.Empty;
                string FixtureType = string.Empty;
                string Remark = string.Empty;
                //通电否
                string IsElectricity = string.Empty;
                //左侧贴
                string LeftSideStick = string.Empty;
                //右侧贴
                string RightSideStick = string.Empty;
                //地铺
                string Floor = string.Empty;
                //窗贴
                string WindowStick = string.Empty;
                //悬挂否
                string IsHang = string.Empty;
                //门位置
                string DoorPosition = string.Empty;
                //户外安装费
                string oohInstallPrice = string.Empty;
                //外协户外安装费
                string OSOOHInstallPrice = string.Empty;
                //器架名称
                string frameName = string.Empty;
                int shopId = 0;
                int positionId = 0;


                Models.POP popModel;
                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                List<int> DeletePOPShopId = new List<int>();
                List<int> DeleteFrameShopId = new List<int>();
                bool isDeleteOldPOP = cbDeleteOldPOP.Checked;
                bool isDeleteOldFrame = cbDeleteOldFrame.Checked;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    bool canSave = true;
                    bool isExist = false;
                    StringBuilder errorMsg = new StringBuilder();

                    if (cols.Contains("POS Code"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POS Code"].ToString().Trim());
                    else if (cols.Contains("POSCode"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POSCode"].ToString().Trim());
                    else if (cols.Contains("店铺编号"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["店铺编号"].ToString().Trim());
                    if (cols.Contains("Sheet"))
                        Sheet = StringHelper.ReplaceSpecialChar(dr["Sheet"].ToString().Trim());
                    else if (cols.Contains("POP位置"))
                        Sheet = StringHelper.ReplaceSpecialChar(dr["POP位置"].ToString().Trim());

                    if (cols.Contains("Graphic No"))
                        GraphicNo = StringHelper.ReplaceSpecialChar(dr["Graphic No"].ToString().Trim());
                    else if (cols.Contains("GraphicNo"))
                        GraphicNo = StringHelper.ReplaceSpecialChar(dr["GraphicNo"].ToString().Trim());
                    else if (cols.Contains("GraphicNo."))
                        GraphicNo = StringHelper.ReplaceSpecialChar(dr["GraphicNo."].ToString().Trim());
                    else if (cols.Contains("Graphic No."))
                        GraphicNo = StringHelper.ReplaceSpecialChar(dr["Graphic No."].ToString().Trim());
                    else if (cols.Contains("POP编号"))
                        GraphicNo = StringHelper.ReplaceSpecialChar(dr["POP编号"].ToString().Trim());
                    else if (cols.Contains("Graphic No#"))//字段名称(Graphic No.)含有（.）的导入的时候变成了#
                        GraphicNo = StringHelper.ReplaceSpecialChar(dr["Graphic No#"].ToString());
                    else if (cols.Contains("GraphicNo#"))
                        GraphicNo = StringHelper.ReplaceSpecialChar(dr["GraphicNo#"].ToString());

                    if (cols.Contains("Graphic Name"))
                        POPName = StringHelper.ReplaceSpecialChar(dr["Graphic Name"].ToString().Trim());
                    else if (cols.Contains("GraphicName"))
                        POPName = StringHelper.ReplaceSpecialChar(dr["GraphicName"].ToString().Trim());
                    else if (cols.Contains("POP Name"))
                        POPName = StringHelper.ReplaceSpecialChar(dr["POP Name"].ToString().Trim());
                    else if (cols.Contains("POPName"))
                        POPName = StringHelper.ReplaceSpecialChar(dr["POPName"].ToString().Trim());
                    else if (cols.Contains("POP名称"))
                        POPName = StringHelper.ReplaceSpecialChar(dr["POP名称"].ToString().Trim());

                    if (cols.Contains("Corner Type"))
                        CornerType = StringHelper.ReplaceSpecialChar(dr["Corner Type"].ToString().Trim());
                    else if (cols.Contains("CornerType"))
                        CornerType = StringHelper.ReplaceSpecialChar(dr["CornerType"].ToString().Trim());
                    else if (cols.Contains("角落类型"))
                        CornerType = StringHelper.ReplaceSpecialChar(dr["角落类型"].ToString().Trim());

                    if (cols.Contains("Category"))
                        Category = StringHelper.ReplaceSpecialChar(dr["Category"].ToString().Trim());
                    else if (cols.Contains("系列"))
                        Category = StringHelper.ReplaceSpecialChar(dr["系列"].ToString().Trim());

                    if (cols.Contains("Standard Dimension"))
                        StandardDimension = StringHelper.ReplaceSpecialChar(dr["Standard Dimension"].ToString().Trim());
                    else if (cols.Contains("StandardDimension"))
                        StandardDimension = StringHelper.ReplaceSpecialChar(dr["StandardDimension"].ToString().Trim());
                    else if (cols.Contains("是否标准规格"))
                        StandardDimension = StringHelper.ReplaceSpecialChar(dr["是否标准规格"].ToString().Trim());

                    if (cols.Contains("Modula"))
                        Modula = StringHelper.ReplaceSpecialChar(dr["Modula"].ToString().Trim());
                    else if (cols.Contains("是否格栅"))
                        Modula = StringHelper.ReplaceSpecialChar(dr["是否格栅"].ToString().Trim());

                    if (cols.Contains("M/W"))
                        Gender = StringHelper.ReplaceSpecialChar(dr["M/W"].ToString().Trim());
                    else if (cols.Contains("性别"))
                        Gender = StringHelper.ReplaceSpecialChar(dr["性别"].ToString().Trim());
                    else if (cols.Contains("Sex"))
                        Gender = StringHelper.ReplaceSpecialChar(dr["Sex"].ToString().Trim());

                    if (cols.Contains("Quantity"))
                        Quantity = dr["Quantity"].ToString().Trim();
                    else if (cols.Contains("数量"))
                        Quantity = dr["数量"].ToString().Trim();

                    if (cols.Contains("Window Wide(mm)"))
                        WindowWide = dr["Window Wide(mm)"].ToString().Trim();
                    else if (cols.Contains("WindowWide(mm)"))
                        WindowWide = dr["WindowWide(mm)"].ToString().Trim();
                    else if (cols.Contains("WindowWide（mm）"))
                        WindowWide = dr["WindowWide（mm）"].ToString().Trim();
                    else if (cols.Contains("位置宽"))
                        WindowWide = dr["位置宽"].ToString().Trim();

                    if (cols.Contains("Window High(mm)"))
                        WindowHigh = dr["Window High(mm)"].ToString().Trim();
                    else if (cols.Contains("WindowHigh(mm)"))
                        WindowHigh = dr["WindowHigh(mm)"].ToString().Trim();
                    else if (cols.Contains("WindowHigh（mm）"))
                        WindowHigh = dr["WindowHigh（mm）"].ToString().Trim();
                    else if (cols.Contains("位置高"))
                        WindowHigh = dr["位置高"].ToString().Trim();

                    if (cols.Contains("Window Deep(mm)"))
                        WindowDeep = dr["Window Deep(mm)"].ToString().Trim();
                    else if (cols.Contains("WindowDeep(mm)"))
                        WindowDeep = dr["WindowDeep(mm)"].ToString().Trim();
                    else if (cols.Contains("WindowDeep（mm）"))
                        WindowDeep = dr["WindowDeep（mm）"].ToString().Trim();
                    else if (cols.Contains("位置深"))
                        WindowDeep = dr["位置深"].ToString().Trim();

                    if (cols.Contains("Window Size"))
                        WindowSize = dr["Window Size"].ToString().Trim();
                    else if (cols.Contains("WindowSize"))
                        WindowSize = dr["WindowSize"].ToString().Trim();
                    else if (cols.Contains("位置规模"))
                        WindowSize = dr["位置规模"].ToString().Trim();

                    if (cols.Contains("Graphic Width(mm)"))
                        GraphicWidth = dr["Graphic Width(mm)"].ToString().Trim();
                    else if (cols.Contains("GraphicWidth(mm)"))
                        GraphicWidth = dr["GraphicWidth(mm)"].ToString().Trim();
                    else if (cols.Contains("GraphicWidth（mm）"))
                        GraphicWidth = dr["GraphicWidth（mm）"].ToString().Trim();
                    else if (cols.Contains("POP宽"))
                        GraphicWidth = dr["POP宽"].ToString().Trim();


                    if (cols.Contains("Graphic Length(mm)"))
                        GraphicLength = dr["Graphic Length(mm)"].ToString().Trim();
                    else if (cols.Contains("GraphicLength(mm)"))
                        GraphicLength = dr["GraphicLength(mm)"].ToString().Trim();
                    else if (cols.Contains("GraphicLength（mm）"))
                        GraphicLength = dr["GraphicLength（mm）"].ToString().Trim();
                    else if (cols.Contains("POP高"))
                        GraphicLength = dr["POP高"].ToString().Trim();

                    if (cols.Contains("Double-Face"))
                        DoubleFace = StringHelper.ReplaceSpecialChar(dr["Double-Face"].ToString().Trim());
                    else if (cols.Contains("单双面"))
                        DoubleFace = StringHelper.ReplaceSpecialChar(dr["单双面"].ToString().Trim());

                    if (cols.Contains("Graphic Material"))
                        GraphicMaterial = StringHelper.ReplaceSpecialChar(dr["Graphic Material"].ToString().Trim());
                    else if (cols.Contains("GraphicMaterial"))
                        GraphicMaterial = StringHelper.ReplaceSpecialChar(dr["GraphicMaterial"].ToString().Trim());
                    else if (cols.Contains("POP材质"))
                        GraphicMaterial = StringHelper.ReplaceSpecialChar(dr["POP材质"].ToString().Trim());

                    if (cols.Contains("Glass"))
                        Glass = StringHelper.ReplaceSpecialChar(dr["Glass"].ToString().Trim());
                    else if (cols.Contains("是否有玻璃"))
                        Glass = StringHelper.ReplaceSpecialChar(dr["是否有玻璃"].ToString().Trim());

                    if (cols.Contains("Backdrop"))
                        Backdrop = StringHelper.ReplaceSpecialChar(dr["Backdrop"].ToString().Trim());
                    else if (cols.Contains("背景"))
                        Backdrop = StringHelper.ReplaceSpecialChar(dr["背景"].ToString().Trim());

                    if (cols.Contains("Frame"))
                        Frame = StringHelper.ReplaceSpecialChar(dr["Frame"].ToString().Trim());
                    else if (cols.Contains("是否框架"))
                        Frame = StringHelper.ReplaceSpecialChar(dr["是否框架"].ToString().Trim());

                    if (cols.Contains("Modula Quantity Width"))
                        ModulaQuantityWidth = StringHelper.ReplaceSpecialChar(dr["Modula Quantity Width"].ToString().Trim());
                    else if (cols.Contains("ModulaQuantityWidth"))
                        ModulaQuantityWidth = StringHelper.ReplaceSpecialChar(dr["ModulaQuantityWidth"].ToString().Trim());
                    else if (cols.Contains("格栅横向数量"))
                        ModulaQuantityWidth = StringHelper.ReplaceSpecialChar(dr["格栅横向数量"].ToString().Trim());


                    if (cols.Contains("Modula Quantity Height"))
                        ModulaQuantityHeight = StringHelper.ReplaceSpecialChar(dr["Modula Quantity Height"].ToString().Trim());
                    else if (cols.Contains("ModulaQuantityHeight"))
                        ModulaQuantityHeight = StringHelper.ReplaceSpecialChar(dr["ModulaQuantityHeight"].ToString().Trim());
                    else if (cols.Contains("格栅纵向数量"))
                        ModulaQuantityHeight = StringHelper.ReplaceSpecialChar(dr["格栅纵向数量"].ToString().Trim());


                    if (cols.Contains("Position Description"))
                        PositionDescription = StringHelper.ReplaceSpecialChar(dr["Position Description"].ToString().Trim());
                    else if (cols.Contains("PositionDescription"))
                        PositionDescription = StringHelper.ReplaceSpecialChar(dr["PositionDescription"].ToString().Trim());
                    else if (cols.Contains("位置描述"))
                        PositionDescription = StringHelper.ReplaceSpecialChar(dr["位置描述"].ToString().Trim());

                    if (cols.Contains("Platform Length(mm)"))
                        PlatformLength = dr["Platform Length(mm)"].ToString().Trim();
                    else if (cols.Contains("PlatformLength(mm)"))
                        PlatformLength = dr["PlatformLength(mm)"].ToString().Trim();
                    else if (cols.Contains("PlatformLength（mm）"))
                        PlatformLength = dr["PlatformLength（mm）"].ToString().Trim();
                    else if (cols.Contains("平台长"))
                        PlatformLength = dr["平台长"].ToString().Trim();

                    if (cols.Contains("Platform Width(mm)"))
                        PlatformWidth = dr["Platform Width(mm)"].ToString().Trim();
                    else if (cols.Contains("PlatformWidth(mm)"))
                        PlatformWidth = dr["PlatformWidth(mm)"].ToString().Trim();
                    else if (cols.Contains("PlatformWidth（mm）"))
                        PlatformWidth = dr["PlatformWidth（mm）"].ToString().Trim();
                    else if (cols.Contains("平台宽"))
                        PlatformWidth = dr["平台宽"].ToString().Trim();

                    if (cols.Contains("Platform Height(mm)"))
                        PlatformHeight = dr["Platform Height(mm)"].ToString().Trim();
                    else if (cols.Contains("PlatformHeight(mm)"))
                        PlatformHeight = dr["PlatformHeight(mm)"].ToString().Trim();
                    else if (cols.Contains("PlatformHeight（mm）"))
                        PlatformHeight = dr["PlatformHeight（mm）"].ToString().Trim();
                    else if (cols.Contains("平台高"))
                        PlatformHeight = dr["平台高"].ToString().Trim();

                    if (cols.Contains("UnitPrice"))
                        UnitPrice = dr["UnitPrice"].ToString().Trim();
                    if (cols.Contains("器架名称"))
                        frameName = StringHelper.ReplaceSpecialChar(dr["器架名称"].ToString().Trim());

                    
                    if (!string.IsNullOrWhiteSpace(GraphicLength) && !string.IsNullOrWhiteSpace(GraphicWidth))
                    {
                        decimal h = StringHelper.IsDecimal(GraphicLength);
                        decimal w = StringHelper.IsDecimal(GraphicWidth);
                        decimal area = (h * w) / 1000000;
                        Area = area.ToString();
                    }



                    if (cols.Contains("Expire Date"))
                        ExpireDate = dr["Expire Date"].ToString().Trim();
                    else if (cols.Contains("ExpireDate"))
                        ExpireDate = dr["ExpireDate"].ToString().Trim();
                    else if (cols.Contains("有效期限"))
                        ExpireDate = dr["有效期限"].ToString().Trim();


                    if (cols.Contains("Fixture Type"))
                        FixtureType = StringHelper.ReplaceSpecialChar(dr["Fixture Type"].ToString().Trim());
                    else if (cols.Contains("FixtureType"))
                        FixtureType = StringHelper.ReplaceSpecialChar(dr["FixtureType"].ToString().Trim());
                    else if (cols.Contains("设备类别"))
                        FixtureType = StringHelper.ReplaceSpecialChar(dr["设备类别"].ToString().Trim());

                    if (cols.Contains("Comments 1"))
                        Remark = StringHelper.ReplaceSpecialChar(dr["Comments 1"].ToString().Trim());
                    else if (cols.Contains("Comments1"))
                        Remark = StringHelper.ReplaceSpecialChar(dr["Comments1"].ToString().Trim());
                    else if (cols.Contains("备注"))
                        Remark = StringHelper.ReplaceSpecialChar(dr["备注"].ToString().Trim());

                    if (cols.Contains("通电否"))
                        IsElectricity = StringHelper.ReplaceSpecialChar(dr["通电否"].ToString().Trim());
                    if (cols.Contains("左侧贴"))
                        LeftSideStick = StringHelper.ReplaceSpecialChar(dr["左侧贴"].ToString().Trim());
                    if (cols.Contains("右侧贴"))
                        RightSideStick = StringHelper.ReplaceSpecialChar(dr["右侧贴"].ToString().Trim());
                    if (cols.Contains("地铺"))
                        Floor = StringHelper.ReplaceSpecialChar(dr["地铺"].ToString().Trim());
                    if (cols.Contains("窗贴"))
                        WindowStick = StringHelper.ReplaceSpecialChar(dr["窗贴"].ToString().Trim());
                    if (cols.Contains("悬挂否"))
                        IsHang = StringHelper.ReplaceSpecialChar(dr["悬挂否"].ToString().Trim());
                    if (cols.Contains("门位置"))
                        DoorPosition = StringHelper.ReplaceSpecialChar(dr["门位置"].ToString().Trim());
                    if (cols.Contains("户外安装费"))
                        oohInstallPrice = StringHelper.ReplaceSpecialChar(dr["户外安装费"].ToString().Trim());
                    if (cols.Contains("外协户外安装费"))
                        OSOOHInstallPrice = StringHelper.ReplaceSpecialChar(dr["外协户外安装费"].ToString().Trim());
                    else if (cols.Contains("应付户外安装费"))
                          OSOOHInstallPrice = dr["应付户外安装费"].ToString().Trim();
                    else if (cols.Contains("户外应付"))
                        OSOOHInstallPrice = dr["户外应付"].ToString().Trim();

                    bool isShut = false;
                    bool isInstall = false;
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号 为空；");
                    }
                    else if (!CheckShop(shopNo, out shopId, out isShut, out isInstall))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号 不存在；");
                    }
                    if (isShut)
                        errorMsg.Append("该店铺编号已经标志删除；");
                    if (string.IsNullOrWhiteSpace(Sheet))
                    {
                        canSave = false;
                        errorMsg.Append("POP位置 为空；");
                    }
                    if (string.IsNullOrWhiteSpace(GraphicNo))
                    {
                        canSave = false;
                        errorMsg.Append("GraphicNo 为空；");
                    }
                    if (string.IsNullOrWhiteSpace(Gender))
                    {
                        canSave = false;
                        errorMsg.Append("性别 为空；");
                    }
                    //GraphicWidth，GraphicLength
                    if (string.IsNullOrWhiteSpace(GraphicWidth))
                    {
                        canSave = false;
                        errorMsg.Append("POP宽 为空；");
                    }
                    else if (!StringHelper.IsDecimalVal(GraphicWidth))
                    {
                        canSave = false;
                        errorMsg.Append("POP宽填写不正确 ；");
                    }
                    if (string.IsNullOrWhiteSpace(GraphicLength))
                    {
                        canSave = false;
                        errorMsg.Append("POP高 为空；");
                    }
                    else if (!StringHelper.IsDecimalVal(GraphicLength))
                    {
                        canSave = false;
                        errorMsg.Append("POP高填写不正确 ；");
                    }
                    int categoryId = 0, materialId = 0;
                    GraphicMaterial = StringHelper.ReplaceSpace(GraphicMaterial);
                    if (string.IsNullOrWhiteSpace(GraphicMaterial))
                    {
                        canSave = false;
                        errorMsg.Append("材质 为空；");
                    }
                    else if (!CheckMaterial(GraphicMaterial, out categoryId, out materialId))
                    {
                        canSave = false;
                        errorMsg.Append("材质不存在；");
                    }
                    if (string.IsNullOrWhiteSpace(Quantity))
                    {

                        Quantity = "1";
                    }
                    if (StringHelper.IsInt(Quantity) == 0)
                    {

                        Quantity = "1";
                    }
                    if (Sheet == "户外")
                        Sheet = "OOH";
                    if (Sheet.ToLower() == "window" || Sheet.ToLower() == "windows")
                        Sheet = "橱窗";
                    int popId = 0;
                    if (canSave && !string.IsNullOrWhiteSpace(GraphicNo) && !GraphicNo.Contains("无") && CheckPOP(shopId, Sheet, GraphicNo, out popId))
                    {

                        //errorMsg.Append("POP编号重复；");
                        //canSave = false;
                        isExist = true;
                    }
                    if (Sheet == "OOH" && isInstall)
                    {
                        if (string.IsNullOrWhiteSpace(oohInstallPrice))
                        {
                            canSave = false;
                            errorMsg.Append("请填写户外安装费(可以填写0)；");
                        }
                        if (!string.IsNullOrWhiteSpace(oohInstallPrice) && !StringHelper.IsDecimalVal(oohInstallPrice))
                        {
                            canSave = false;
                            errorMsg.Append("户外安装费填写不正确 ；");
                        }
                        if (!string.IsNullOrWhiteSpace(OSOOHInstallPrice) && !StringHelper.IsDecimalVal(OSOOHInstallPrice))
                        {
                            canSave = false;
                            errorMsg.Append("外协户外安装费填写不正确 ；");
                        }
                    }
                    decimal wdeep = StringHelper.IsDecimal(WindowDeep);
                    decimal wHigh = StringHelper.IsDecimal(WindowHigh);
                    decimal wWide = StringHelper.IsDecimal(WindowWide);
                    //if (Sheet == "橱窗")
                    //{
                    //    if (wdeep == 0)
                    //    {
                    //        canSave = false;
                    //        errorMsg.Append("请填写位置深 ；");
                    //    }
                    //    if (wHigh == 0)
                    //    {
                    //        canSave = false;
                    //        errorMsg.Append("请填写位置高 ；");
                    //    }
                    //    if (wWide == 0)
                    //    {
                    //        canSave = false;
                    //        errorMsg.Append("请填写位置宽 ；");
                    //    }
                    //}
                    frameName = frameName.ToUpper();
                    if (!string.IsNullOrWhiteSpace(frameName) && !CheckPOPFrameName(shopId,Sheet,frameName,Gender,CornerType))
                    {
                        canSave = false;
                        errorMsg.Append("器架名称不正确 ；");
                    }
                    if (canSave)
                    {
                        popModel = new Models.POP();
                        if (isDeleteOldPOP && !DeletePOPShopId.Contains(shopId))
                        {
                            DeleteOldPOP(shopId);
                            DeletePOPShopId.Add(shopId);

                        }
                        if (isDeleteOldFrame && !DeleteFrameShopId.Contains(shopId))
                        {
                            frameBll.Delete(s => s.ShopId == shopId);
                            DeleteFrameShopId.Add(shopId);
                        }
                        if (!isDeleteOldPOP && isExist)
                        {
                            popModel = popBll.GetModel(popId);

                        }
                        if (!string.IsNullOrWhiteSpace(UnitPrice))
                            popModel.UnitPrice = StringHelper.IsDecimal(UnitPrice);
                        if (!string.IsNullOrWhiteSpace(Area))
                            popModel.Area = Math.Round(StringHelper.IsDecimal(Area), 2);
                        popModel.Backdrop = Backdrop;
                        popModel.Category = Category;
                        popModel.CornerType = CornerType;
                        popModel.DoorPosition = DoorPosition;
                        popModel.DoubleFace = DoubleFace;
                        if (StringHelper.IsDateTime(ExpireDate))
                            popModel.ExpireDate = DateTime.Parse(ExpireDate);
                        popModel.FixtureType = FixtureType;

                        popModel.Frame = Frame;
                        popModel.Gender = Gender;
                        popModel.Glass = Glass;
                        if (!string.IsNullOrWhiteSpace(GraphicLength))
                            popModel.GraphicLength = StringHelper.IsDecimal(GraphicLength);
                        if (!string.IsNullOrWhiteSpace(GraphicWidth))
                            popModel.GraphicWidth = StringHelper.IsDecimal(GraphicWidth);
                        if (!string.IsNullOrWhiteSpace(GraphicMaterial))
                            popModel.GraphicMaterial = GraphicMaterial;
                        if (!string.IsNullOrWhiteSpace(GraphicNo))
                            popModel.GraphicNo = GraphicNo.ToUpper();
                        if (!string.IsNullOrWhiteSpace(POPName))
                            popModel.POPName = POPName.ToUpper();

                        popModel.IsElectricity = IsElectricity;
                        popModel.IsHang = IsHang;

                        popModel.Modula = Modula;
                        popModel.ModulaQuantityHeight = StringHelper.IsInt(ModulaQuantityHeight);
                        popModel.ModulaQuantityWidth = StringHelper.IsInt(ModulaQuantityWidth);
                        popModel.PlatformHeight = StringHelper.IsInt(PlatformHeight);
                        popModel.PlatformLength = StringHelper.IsInt(PlatformLength);
                        popModel.PlatformWidth = StringHelper.IsInt(PlatformWidth);
                        popModel.POPName = POPName;
                        popModel.POPType = POPType;

                        popModel.PositionDescription = PositionDescription;
                        popModel.PositionId = positionId;
                        popModel.Quantity = StringHelper.IsInt(Quantity);
                        popModel.Remark = Remark;
                        popModel.Sheet = Sheet.ToUpper();
                        popModel.StandardDimension = StandardDimension;
                        popModel.Style = Style;
                        
                        popModel.WindowDeep = wdeep;
                        popModel.WindowHigh = wHigh;
                        popModel.WindowWide = wWide;
                        popModel.WindowSize = WindowSize;
                        popModel.WindowStick = WindowStick;

                        popModel.IsElectricity = IsElectricity;
                        popModel.LeftSideStick = LeftSideStick;
                        popModel.RightSideStick = RightSideStick;
                        popModel.Floor = Floor;
                        popModel.WindowStick = WindowStick;
                        popModel.IsHang = IsHang;
                        popModel.DoorPosition = DoorPosition;
                        if (Sheet == "OOH")
                        {
                            if (!string.IsNullOrWhiteSpace(oohInstallPrice))
                                popModel.OOHInstallPrice = StringHelper.IsDecimal(oohInstallPrice);
                            if (!string.IsNullOrWhiteSpace(OSOOHInstallPrice))
                                popModel.OSOOHInstallPrice = StringHelper.IsDecimal(OSOOHInstallPrice);
                        }
                        popModel.MaterialCategoryId = categoryId;
                        popModel.OrderGraphicMaterialId = materialId;
                        popModel.MachineFrameName = frameName;
                        //左侧贴：橱窗位置深*橱窗位置高
                        //popModel.LeftSideStick = wdeep + "×" + wHigh;
                        //右侧贴：橱窗位置深*橱窗位置高
                        //popModel.RightSideStick = wdeep + "×" + wHigh;
                        //地铺：橱窗位置宽*橱窗位置深
                        //popModel.Floor = wWide + "×" + wdeep;
                        if (!isDeleteOldPOP && isExist)
                        {
                            popBll.Update(popModel);
                        }
                        else
                        {
                            popModel.AddDate = DateTime.Now;
                            popModel.AddUserId = new BasePage().CurrentUser.UserId;
                            popModel.ShopId = shopId;
                            popBll.Add(popModel);

                            changeDetailModel = new POPChangeDetail();
                            changeDetailModel.AddDate = DateTime.Now;
                            changeDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            changeDetailModel.Category = popModel.Category;
                            changeDetailModel.ChangeType = "新增";
                            changeDetailModel.CornerType = popModel.CornerType;
                            changeDetailModel.Gender = popModel.Gender;
                            changeDetailModel.GraphicLength = popModel.GraphicLength;
                            changeDetailModel.GraphicMaterial = popModel.GraphicMaterial;
                            changeDetailModel.GraphicNo = popModel.GraphicNo;
                            changeDetailModel.GraphicWidth = popModel.GraphicWidth;
                            changeDetailModel.IsValid = popModel.IsValid;
                            changeDetailModel.OOHInstallPrice = popModel.OOHInstallPrice;
                            changeDetailModel.PositionDescription = popModel.PositionDescription;
                            changeDetailModel.Quantity = popModel.Quantity;
                            changeDetailModel.Remark = popModel.Remark;
                            changeDetailModel.Sheet = popModel.Sheet;
                            changeDetailModel.ShopId = popModel.ShopId;
                            changeDetailModel.WindowDeep = popModel.WindowDeep;
                            changeDetailModel.WindowHigh = popModel.WindowHigh;
                            changeDetailModel.WindowSize = popModel.WindowSize;
                            changeDetailModel.WindowWide = popModel.WindowWide;
                            changeDetailModel.LogId = logModel.Id;
                            detailBll.Add(changeDetailModel);


                        }
                        AddSheet(Sheet);
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
            }
        }

        //导入器架

        void ImportMachineFrame(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                //添加变更日志
                BaseDataChangeLog logModel = new BaseDataChangeLog();
                logModel.AddDate = DateTime.Now;
                logModel.AddUserId = new BasePage().CurrentUser.UserId;
                logModel.ItemType = (int)BaseDataChangeItemEnum.ShopMachineFrame;
                logModel.ChangeType = (int)DataChangeTypeEnum.Add;
                //logModel.ShopId = shopId;
                changeLogBll.Add(logModel);

                ShopMachineFrameChangeDetailBLL changeDetailBll = new ShopMachineFrameChangeDetailBLL();
                ShopMachineFrameChangeDetail changeDetailModel;

                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                Models.ShopMachineFrame model;
                //ShopMachineFrameBLL bll = new ShopMachineFrameBLL();
                int shopId = 0;
                List<int> DeleteFrameShopId = new List<int>();
                bool isDeleteOldFrame = cbDeleteOldFrame.Checked;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    bool canSave = true;
                    StringBuilder errorMsg = new StringBuilder();
                    string shopNo = string.Empty;
                    if (cols.Contains("店铺编号"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["店铺编号"].ToString().Trim());
                    else if (cols.Contains("POS Code"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POS Code"].ToString().Trim());
                    else if (cols.Contains("POSCode"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POSCode"].ToString().Trim());

                    string sheet = string.Empty;
                    if (cols.Contains("POP位置"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["POP位置"].ToString().Trim());
                    else if (cols.Contains("器架类型"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["器架类型"].ToString().Trim());
                    else if (cols.Contains("Sheet"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["Sheet"].ToString().Trim());

                    string frameName = string.Empty;
                    if (cols.Contains("器架名称"))
                        frameName = StringHelper.ReplaceSpecialChar(dr["器架名称"].ToString().Trim());
                    else if (cols.Contains("名称"))
                        frameName = StringHelper.ReplaceSpecialChar(dr["名称"].ToString().Trim());

                    string gender = string.Empty;
                    if (cols.Contains("性别"))
                        gender = StringHelper.ReplaceSpecialChar(dr["性别"].ToString().Trim());
                    else if (cols.Contains("男女"))
                        gender = StringHelper.ReplaceSpecialChar(dr["男女"].ToString().Trim());
                    else if (cols.Contains("男/女"))
                        gender = StringHelper.ReplaceSpecialChar(dr["男/女"].ToString().Trim());
                    else if (cols.Contains("M/W"))
                        gender = StringHelper.ReplaceSpecialChar(dr["M/W"].ToString().Trim());

                    string count = string.Empty;
                    if (cols.Contains("数量"))
                        count = dr["数量"].ToString().Trim();
                    else if (cols.Contains("Quantity"))
                        count = dr["Quantity"].ToString().Trim();

                    string cornerType = string.Empty;
                    if (cols.Contains("角落类型"))
                        cornerType = StringHelper.ReplaceSpecialChar(dr["角落类型"].ToString().Trim());
                    string levelNum = string.Empty;
                    if (cols.Contains("级别"))
                        levelNum = StringHelper.ReplaceSpecialChar(dr["级别"].ToString().Trim());
                    bool isShut = false;
                    bool isInstall = false;
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号 为空；");
                    }
                    else if (!CheckShop(shopNo, out shopId, out isShut, out isInstall))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号 不存在；");
                    }
                    if (isShut)
                        errorMsg.Append("该店铺编号已经标志删除；");
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
                    if (!string.IsNullOrWhiteSpace(cornerType) && !CheckCornerType(sheet, cornerType))
                    {
                        canSave = false;
                        errorMsg.Append("角落类型 填写不正确；");
                    }
                    if (canSave)
                    {
                        if (!CheckFrame(sheet,cornerType, frameName))
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
                        if (isDeleteOldFrame && !DeleteFrameShopId.Contains(shopId))
                        {
                            frameBll.Delete(s => s.ShopId == shopId);
                            DeleteFrameShopId.Add(shopId);
                        }

                        int id = 0;
                        frameName = frameName.Replace("（", "(").Replace("）", ")").ToUpper();
                        if (!isDeleteOldFrame && CheckFrameIsExist(shopId, sheet, frameName, gender, cornerType, out id))
                        {
                            model = frameBll.GetModel(id);
                            if (model != null)
                            {
                                model.Count = int.Parse(count);
                                if (!string.IsNullOrWhiteSpace(levelNum))
                                    model.LevelNum = int.Parse(levelNum);
                                else
                                    model.LevelNum = null;
                                frameBll.Update(model);
                            }
                        }
                        else
                        {
                            model = new Models.ShopMachineFrame() { Count = int.Parse(count), Gender = gender, MachineFrame = frameName, PositionName = sheet.ToUpper(), ShopId = shopId, CornerType = cornerType };
                            if (!string.IsNullOrWhiteSpace(levelNum))
                                model.LevelNum = int.Parse(levelNum);
                            else
                                model.LevelNum = null;
                            frameBll.Add(model);
                            //添加变更明细
                             changeDetailModel = new ShopMachineFrameChangeDetail();
                            changeDetailModel.CornerType = model.CornerType;
                            changeDetailModel.Count = model.Count;
                            changeDetailModel.Gender = model.Gender;
                            changeDetailModel.LevelNum = model.LevelNum;
                            changeDetailModel.LogId = logModel.Id;
                            changeDetailModel.MachineFrame = model.MachineFrame;
                            changeDetailModel.PositionName = model.PositionName;
                            changeDetailModel.Remark = "新增";
                            changeDetailModel.ShopId = model.ShopId;
                            changeDetailModel.AddDate = DateTime.Now;
                            changeDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            changeDetailBll.Add(changeDetailModel);
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
                
            }
        }


        //更新特殊店铺基础安装费
        void ImportShopCharges(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                string shopNo = string.Empty;
                int shopId = 0;
                string installPrice = string.Empty;
                string windowInstallPrice = string.Empty;
                string isInstall = string.Empty;
                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                Models.Shop shopModel;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    bool canSave = true;
                    StringBuilder errorMsg = new StringBuilder();
                    if (cols.Contains("POS Code"))
                        shopNo = dr["POS Code"].ToString().Trim();
                    else if (cols.Contains("POSCode"))
                        shopNo = dr["POSCode"].ToString().Trim();
                    else if (cols.Contains("POSCODE"))
                        shopNo = dr["POSCODE"].ToString().Trim();
                    else if (cols.Contains("POS CODE"))
                        shopNo = dr["POS CODE"].ToString().Trim();
                    else if (cols.Contains("店铺编号"))
                        shopNo = dr["店铺编号"].ToString().Trim();
                    if (cols.Contains("基础安装费"))
                        installPrice = dr["基础安装费"].ToString().Trim();
                    if (cols.Contains("安装费"))
                        installPrice = dr["安装费"].ToString().Trim();
                    //if (cols.Contains("橱窗安装费"))
                    //    windowInstallPrice = dr["橱窗安装费"].ToString().Trim();
                    if (cols.Contains("是否安装"))
                        isInstall = dr["是否安装"].ToString().Trim();
                    else if (cols.Contains("IsInstall"))
                        isInstall = dr["IsInstall"].ToString().Trim();
                    else if (cols.Contains("安装级别"))
                        isInstall = dr["安装级别"].ToString().Trim();

                    bool isShut = false;
                    bool isInstall0 = false;
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号为空；");
                    }
                    else if (!CheckShop(shopNo, out shopId, out isShut, out isInstall0))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号不存在；");
                    }
                    if (isShut)
                        errorMsg.Append("该店铺编号已经标志删除；");
                    installPrice = string.IsNullOrWhiteSpace(installPrice) ? "0" : installPrice;
                    //windowInstallPrice = string.IsNullOrWhiteSpace(windowInstallPrice) ? "0" : windowInstallPrice;
                    if (!StringHelper.IsDecimalVal(installPrice))
                    {
                        canSave = false;
                        errorMsg.Append("基础安装费填写不正确；");
                    }
                    //if (!StringHelper.IsDecimalVal(windowInstallPrice))
                    //{
                    //    canSave = false;
                    //    errorMsg.Append("橱窗安装费填写不正确；");
                    //}
                    if (isInstall == "是" || isInstall == "安装")
                    {
                        isInstall = "Y";
                    }
                    if (isInstall == "否" || isInstall == "不安装" || isInstall == "不" || isInstall == "不是")
                    {
                        isInstall = "N";
                    }
                    if (canSave)
                    {
                        shopModel = shopBll.GetModel(shopId);
                        if (shopModel != null)
                        {
                            //bool update = false;
                            if (decimal.Parse(installPrice) > 0)
                            {
                                shopModel.BasicInstallPrice = decimal.Parse(installPrice);
                                if (!string.IsNullOrWhiteSpace(isInstall))
                                {
                                    shopModel.IsInstall = isInstall;
                                }
                                shopBll.Update(shopModel);
                                successNum++;
                            }
                           
                            
                        }
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
            }
        }

        /// <summary>
        /// 新增鞋墙凹槽
        /// </summary>
        void ImprotAOCaoMachineFrame(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                ShopMachineFrameBLL frameBll = new ShopMachineFrameBLL();
                ShopMachineFrame frameModel;
                ShopBLL shopBll = new ShopBLL();
                Shop shopModel;
                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                List<int> addShopList = new List<int>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StringBuilder errorMsg = new StringBuilder();
                    string shopNo = string.Empty;
                    if (cols.Contains("店铺编号"))
                        shopNo = dr["店铺编号"].ToString().Trim();
                    else if (cols.Contains("POS Code"))
                        shopNo = dr["POS Code"].ToString().Trim();
                    else if (cols.Contains("POSCode"))
                        shopNo = dr["POSCode"].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(shopNo))
                    {
                        shopModel = shopBll.GetList(s => s.ShopNo.ToLower() == shopNo.ToLower()).FirstOrDefault();
                        if (shopModel != null)
                        {
                            if (!addShopList.Contains(shopModel.Id))
                            {
                                var frameList = frameBll.GetList(s => s.ShopId == shopModel.Id && s.PositionName == "鞋墙" && (s.CornerType == null || s.CornerType == ""));
                                frameList.ForEach(s =>
                                {
                                   
                                    s.PositionName = "凹槽";
                                    frameBll.Add(s);
                                });
                                addShopList.Add(shopModel.Id);
                                successNum++;
                            }
                        }
                        else
                        {
                            DataRow dr1 = errorTB.NewRow();
                            for (int i = 0; i < cols.Count; i++)
                            {
                                dr1["" + cols[i] + ""] = dr[cols[i]];
                            }
                            dr1["错误信息"] = "店铺不存在";
                            errorTB.Rows.Add(dr1);
                            failNum++;
                        }
                    }
                }
                if (errorTB.Rows.Count > 0)
                {
                    Session["errorTb"] = errorTB;
                }

            }
        }

        void UpdatePOPNo(DataSet ds)
        {
            POPBLL popbll = new POPBLL();
            POP popModel;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string id0 = dr["Id"].ToString();
                    string shopId0 = dr["ShopId"].ToString();
                    string sheet = dr["Sheet"].ToString();
                    if (!string.IsNullOrWhiteSpace(id0) && !string.IsNullOrWhiteSpace(shopId0) && !string.IsNullOrWhiteSpace(sheet))
                    {
                        
                        int id = int.Parse(id0);
                        popModel = popbll.GetModel(id);
                        if (popModel != null)
                        {
                            int shopid = int.Parse(shopId0);
                            int count = popbll.GetList(s => s.ShopId == shopid && s.Sheet == sheet && s.GraphicNo != null && s.GraphicNo != "" && s.GraphicNo != "无").Count;
                            string popNo = sheet.Contains("陈列桌") ? "TABLE" : sheet;
                            StringBuilder popNO1 = new StringBuilder();
                            popNO1.Append(popNo).Append('-').Append((count + 1).ToString().PadLeft(2, '0'));
                            popModel.GraphicNo = popNO1.ToString();
                            popbll.Update(popModel);
                            successNum++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新客服
        /// </summary>
        /// <param name="ds"></param>
        void UpdateCustomerServiceId(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                string shopNo = string.Empty;
                int shopId = 0;
                int csUserId = 0;
                string csUserName = string.Empty;
                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                Models.Shop shopModel;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    bool canSave = true;
                    shopId = 0;
                    csUserId = 0;
                    csUserName = string.Empty;
                    StringBuilder errorMsg = new StringBuilder();
                    if (cols.Contains("POS Code"))
                        shopNo = dr["POS Code"].ToString().Trim();
                    else if (cols.Contains("POSCode"))
                        shopNo = dr["POSCode"].ToString().Trim();
                    else if (cols.Contains("POSCODE"))
                        shopNo = dr["POSCODE"].ToString().Trim();
                    else if (cols.Contains("POS CODE"))
                        shopNo = dr["POS CODE"].ToString().Trim();
                    else if (cols.Contains("店铺编号"))
                        shopNo = dr["店铺编号"].ToString().Trim();

                    if (cols.Contains("客服名称"))
                        csUserName = dr["客服名称"].ToString().Trim();
                    else if (cols.Contains("客服"))
                        csUserName = dr["客服"].ToString().Trim();
                    else if (cols.Contains("客服姓名"))
                        csUserName = dr["客服姓名"].ToString().Trim();

                    bool isShut = false;
                    bool isInsatll = false;
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号 为空；");
                    }
                    else if (!CheckShop(shopNo, out shopId, out isShut, out isInsatll))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号 不存在；");
                    }
                    if (string.IsNullOrWhiteSpace(csUserName))
                    {
                        canSave = false;
                        errorMsg.Append("客服姓名不能为空；");
                    }
                    else if (!CheckUserId(csUserName, out csUserId))
                    {
                        canSave = false;
                        errorMsg.AppendFormat("客服{0}不存在，请先创建用户；", csUserName);
                    }

                    if (canSave)
                    {
                        shopModel = shopBll.GetModel(shopId);
                        shopModel.CSUserId = csUserId;
                        shopBll.Update(shopModel);
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
            }
        }

        /// <summary>
        /// 导入外协安装费
        /// </summary>
        /// <param name="ds"></param>
        void ImportOutsourceInstallPrice(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable errorTB = Common.CommonMethod.CreateErrorTB(cols);
                string shopNo = string.Empty;
                string outsourceInstallPrice = string.Empty;
                int shopId=0;
                if (!cols.Contains("外协安装费") && !cols.Contains("外协费用"))
                {
                    isWrong = 1;
                    //ImportState.Style.Add("display", "block");
                    wrongMsg= "导入失败：没找到‘外协安装费’列";
                    return;
                }
                List<string> shopNoList = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    bool canSave = true;
                    StringBuilder errorMsg = new StringBuilder();
                    if (cols.Contains("POS Code"))
                        shopNo = dr["POS Code"].ToString().Trim();
                    else if (cols.Contains("POSCode"))
                        shopNo = dr["POSCode"].ToString().Trim();
                    else if (cols.Contains("店铺编号"))
                        shopNo = dr["店铺编号"].ToString().Trim();
                    if (cols.Contains("外协安装费"))
                        outsourceInstallPrice = dr["外协安装费"].ToString().Trim();
                    else if (cols.Contains("外协费用"))
                        outsourceInstallPrice = dr["外协费用"].ToString().Trim();
                    bool isShut = false;
                    bool isInsatll = false;
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号 为空；");
                    }
                    else if (shopNoList.Contains(shopNo.ToUpper()))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号重复 ；");
                    }
                    else if (!CheckShop(shopNo, out shopId, out isShut,out isInsatll))
                    {
                        canSave = false;
                        errorMsg.Append("店铺编号 不存在；");
                    }
                    if (!string.IsNullOrWhiteSpace(outsourceInstallPrice) && !StringHelper.IsDecimalVal(outsourceInstallPrice))
                    {
                        canSave = false;
                        errorMsg.Append("外协安装费填写不正确；");
                    }
                    Models.Shop shopModel;
                    if (canSave)
                    {
                        shopNoList.Add(shopNo.ToUpper());
                        decimal installPrice = StringHelper.IsDecimal(outsourceInstallPrice);
                        if (installPrice > 0)
                        {
                            shopModel = shopBll.GetModel(shopId);
                            if (shopModel != null)
                            {
                                shopModel.OutsourceInstallPrice = installPrice;
                                shopBll.Update(shopModel);
                            }
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
            }
        }

        RegionBLL regionBll = new RegionBLL();
        Models.Region regionModel;
        List<string> regions = new List<string>();
        void AddRegion(string region)
        {
            if (!string.IsNullOrWhiteSpace(region) && !regions.Contains(region.ToLower()))
            {
                regions.Add(region.ToLower());
                var list = regionBll.GetList(s => s.RegionName.ToLower() == region.ToLower() && s.CustomerId == customerId);
                if (!list.Any())
                {
                    regionModel = new Models.Region { CustomerId = customerId, IsDelete = false, RegionName = region };
                    regionBll.Add(regionModel);
                }
            }

        }

        /// <summary>
        /// 检查是否已存在，不存在就新增，存在就更新
        /// </summary>
        /// <param name="shopName"></param>
        /// <returns></returns>
        bool CheckShop(string shopNo, out int shopId, out bool isShut, out bool isInstall)
        {
            shopId = 0;
            isShut = false; isInstall = false;
            var list = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper());
            if (list.Any())
            {
                var model0 = list.Where(s=>s.IsDelete==null || s.IsDelete==false).FirstOrDefault();
                if (model0 != null)
                {
                    isShut = false;
                    shopId = model0.Id;
                    isInstall = (model0.IsInstall != null && model0.IsInstall.ToLower() == "y");
                }
                else
                {
                    model0 = list.Where(s => s.IsDelete == true).FirstOrDefault();
                    if (model0 != null)
                    {
                        isShut = true;
                        shopId = model0.Id;
                        isInstall = (model0.IsInstall != null && model0.IsInstall.ToLower() == "y");
                    }
                }
                return true;
            }
            return false;
        }

        bool CheckNewShopNo(string newShopNo)
        {
            var list = shopBll.GetList(s => s.ShopNo.ToUpper() == newShopNo.ToUpper() && (s.IsDelete==null || s.IsDelete==false));
            return list.Any();
        }


        bool ShopNameIsExist(string shopName)
        {

            var list = shopBll.GetList(s => s.ShopName.ToUpper() == shopName.ToUpper() && (s.IsDelete==null || s.IsDelete==false));
            if (list.Any())
            {
                return true;
            }
            return false;
        }

        List<Models.Region> regionList = new List<Models.Region>();
        List<Models.Place> placeList = new List<Models.Place>();
        //RegionBLL regionBll = new RegionBLL();
        PlaceBLL placeBll = new PlaceBLL();
        /// <summary>
        /// 区域Id
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        int GetRegionId(string regionName)
        {
            int id = 0;
            if (!regionList.Any())
            {
                regionList = regionBll.GetList(s => s.CustomerId == customerId);
            }
            var list = regionList.Where(s => s.RegionName.ToLower() == regionName.ToLower()).ToList();
            if (list.Any())
            {
                id = list[0].Id;
            }
            return id;
        }
        /// <summary>
        /// 省份ID
        /// </summary>
        /// <param name="provinceName"></param>
        /// <returns></returns>
        int GetProvinceId(string provinceName)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => (s.PlaceName == provinceName || s.PlaceName == provinceName + "省") && s.ParentID == 0).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }
        /// <summary>
        /// 城市ID 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        int GetCityId(string cityName, int provinceId)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => s.PlaceName == cityName && s.ParentID == provinceId).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }

        /// <summary>
        /// 县ID 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        int GetCountyId(string countyName, int cityId)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => s.PlaceName == countyName && s.ParentID == cityId).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }

        /// <summary>
        /// 通过县名称获取城市Id
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="cityId"></param>
        /// <param name="areaId"></param>
        void GetCity(string areaName, out string cityName, out int cityId, out int areaId)
        {
            cityId = 0; areaId = 0;
            cityName = string.Empty;
            Models.Place placeModel = placeBll.GetList(s => s.PlaceName == areaName).FirstOrDefault();
            if (placeModel != null)
            {
                cityId = placeModel.ParentID ?? 0;
                areaId = placeModel.ID;
                var model1 = placeBll.GetModel(cityId);
                if (model1 != null)
                    cityName = model1.PlaceName;
            }
        }
        /// <summary>
        /// 获取pop位置ID
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        /// 
        List<Models.Position> positionList = new List<Models.Position>();
        List<string> newSheet = new List<string>();
        PositionBLL positionBll = new PositionBLL();
        void AddSheet(string sheet)
        {


            if (!positionList.Any())
            {
                positionList = positionBll.GetList(s => 1 == 1);
            }
            var list = positionList.Where(s => s.PositionName.ToUpper() == sheet.ToUpper());
            if (!list.Any() && !newSheet.Contains(sheet))
            {
                newSheet.Add(sheet);
                Models.Position model = new Models.Position() { PositionName = sheet };
                positionBll.Add(model);

            }

        }
        /// <summary>
        /// 下载导入模板(shop)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoadShop_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "ShopImportTemplate";
            
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }
        /// <summary>
        /// 下载导入模板(POP)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoadPOP_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "POPImportTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void lbDownLoadFrame_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "店铺器架类型模板";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void lbDownLoadShopAndPOP_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "ShopAndPOPImportTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }
        /// <summary>
        /// 检查pop是否存在
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="popNo"></param>
        /// <returns></returns>
        bool CheckPOP(int shopId, string sheet, string popNo, out int popId)
        {
            popId = 0;
            var list = popBll.GetList(s => s.ShopId == shopId && s.Sheet.ToUpper() == sheet.ToUpper() && s.GraphicNo.ToUpper() == popNo.ToUpper());
            if (list.Any())
            {
                popId = list[0].Id;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 导出失败信息
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

        Dictionary<string, int> userDic = new Dictionary<string, int>();
        UserBLL userBll = new UserBLL();
        bool CheckUserId(string userName, out int userId)
        {
            userId = 0;
            bool flag = true;
            if (userDic.Keys.Contains(userName))
                userId = userDic[userName];
            else
            {
                var model = userBll.GetList(s => s.RealName == userName).FirstOrDefault();
                if (model != null)
                {
                    userId = model.UserId;
                    userDic.Add(userName, userId);
                }
                else
                    flag = false;
            }
            return flag;
        }

        void DeleteOldPOP(int shopId)
        {
            popBll.Delete(s => s.ShopId == shopId);
        }

        List<string> ExistMaterialList = new List<string>();
        Dictionary<string, string> dicMateril = new Dictionary<string, string>();
        OrderMaterialMppingBLL orderMaterialBll = new OrderMaterialMppingBLL();
        /// <summary>
        /// 检查材质是否正确
        /// </summary>
        /// <param name="materialName"></param>
        /// <returns></returns>
        bool CheckMaterial(string materialName, out int categoryId, out int materialId)
        {
            bool flag = false;
            categoryId = 0;
            materialId = 0;
            if (dicMateril.Keys.Contains(materialName.ToLower()))
            {
                flag = true;
                string ids = dicMateril[materialName.ToLower()];
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    categoryId = int.Parse(ids.Split('|')[0]);
                    materialId = int.Parse(ids.Split('|')[1]);
                }
            }
            else
            {
                var model = orderMaterialBll.GetList(s => s.OrderMaterialName.ToLower() == materialName.ToLower()).FirstOrDefault();
                if (model != null)
                {
                    flag = true;
                    categoryId = model.BasicCategoryId ?? 0;
                    materialId = model.Id;
                    dicMateril.Add(materialName.ToLower(), categoryId + "|" + materialId);
                    //ExistMaterialList.Add(materialName.ToLower());
                }
            }
            return flag;
        }

        protected void lbDownLoadInstallPrice_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "特殊店铺基础安装费";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }


        bool CheckFrameIsExist(int shopId, string sheet, string frame, string gender, string cornerType, out int id)
        {
            id = 0;
            var list = frameBll.GetList(s => s.ShopId == shopId && s.PositionName.ToUpper() == sheet.ToUpper() && s.MachineFrame.ToUpper() == frame && s.Gender == gender && s.CornerType == cornerType);
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
        bool CheckFrame(string sheet,string cornerType, string frameName)
        {
            bool flag = false;
            if (correctFrameList.Contains(sheet.ToUpper() + "&" + cornerType + "&" + frameName.ToUpper()))
            {
                flag = true;
            }
            else
            {
                var list = new BasicMachineFrameBLL().GetList(s => s.Sheet == sheet.ToUpper() && s.MachineFrame == frameName.ToUpper());
                if (!string.IsNullOrWhiteSpace(cornerType))
                {
                    list = list.Where(s => s.CornerType == cornerType).ToList();
                }
                else
                {
                    list = list.Where(s => s.CornerType == null || s.CornerType=="").ToList();
                }
                if (list.Any())
                {
                    correctFrameList.Add(sheet.ToUpper() + "&" + cornerType + "&" + frameName.ToUpper());
                    flag = true;
                }
            }
            return flag;
        }

        List<string> correctCornerTypeList = new List<string>();
        bool CheckCornerType(string sheet, string cornerType)
        {
            bool flag = false;
            if (correctCornerTypeList.Contains(sheet.ToUpper() + "&" + cornerType))
            {
                flag = true;
            }
            else
            {
                var list = new BasicMachineFrameBLL().GetList(s => s.Sheet == sheet.ToUpper() && s.CornerType == cornerType);
                
                if (list.Any())
                {
                    correctCornerTypeList.Add(sheet.ToUpper() + "&" + cornerType);
                    flag = true;
                }
            }
            return flag;
        }


        bool CheckPOPFrameName(int shopId, string sheet, string frameName, string gender, string cornerType)
        {
            
            var list = frameBll.GetList(s => s.ShopId == shopId && s.PositionName.ToUpper() == sheet.ToUpper() && s.MachineFrame.ToUpper() == frameName && (s.Gender == gender || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女"))));
            if (!string.IsNullOrWhiteSpace(cornerType))
            {
                list = list.Where(s => s.CornerType == cornerType).ToList();
            }
            else
            {
                list = list.Where(s => s.CornerType == null || s.CornerType=="").ToList();
            }
            return list.Any();
        }

        Dictionary<string, int> outsourceDic = new Dictionary<string, int>();
        CompanyBLL companyBll = new CompanyBLL();
        bool GetOutsourceName(string outsourceName, out int outsourceId)
        {
            bool flag = false;
            outsourceId=0;
            if (!string.IsNullOrWhiteSpace(outsourceName))
            {
                outsourceName = outsourceName.ToLower();
            }
            if (outsourceDic.Keys.Contains(outsourceName))
            {
                outsourceId = outsourceDic[outsourceName];
                flag = true;
            }
            else
            {
                Company model = companyBll.GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.CompanyName.ToLower() == outsourceName || (s.ShortName != null && s.ShortName.ToLower() == outsourceName)) && (s.IsDelete == null || s.IsDelete == false)).FirstOrDefault();
                if (model != null)
                {
                    outsourceId = model.Id;
                    outsourceDic.Add(outsourceName, outsourceId);
                    flag = true;
                }
            }
            return flag;
        }

        //string GetGraphicNo(int shopId, string sheet)
        //{
        //    string code = string.Empty;
        //    var 
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Models;
using NPOI.SS.UserModel;
using System.Configuration;
using System.IO;
using Common;
using System.Data;
using System.Data.OleDb;
using System.Text;
using BLL;
using DAL;

namespace WebApp.Subjects
{
    public partial class EmptyDateWarn : System.Web.UI.Page
    {
        string guidanceId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["import"] != null)
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
                    //updateResultTR.Style.Add("display", "block");
                    updateResultTR.Visible = true;
                    labResult.Text = "导入成功！";
                    if (failNum > 0)
                    {
                        labResult.Text = "部分数据更新失败！";
                        lbDownLoadErrorMsg.Visible = true;
                    }

                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            if (Session["emptyOrderShop"] != null)
            {
                List<Shop> shopList = (List<Shop>)Session["emptyOrderShop"];
                //Session["emptyOrderShop"] = null;
                if (shopList.Any())
                {
                    shopList = shopList.OrderBy(s => s.Id).ToList();
                    string templateFileName = "ShopTemplate";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheet("Sheet1");

                    int startRow = 1;
                    string shopno = string.Empty;
                    foreach (var item in shopList)
                    {



                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 23; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(0).SetCellValue(item.ShopNo);
                        dataRow.GetCell(1).SetCellValue(item.ShopName);
                        dataRow.GetCell(2).SetCellValue(item.POSScale);
                        dataRow.GetCell(3).SetCellValue(item.MaterialSupport);
                        dataRow.GetCell(4).SetCellValue(item.RegionName);
                        dataRow.GetCell(5).SetCellValue(item.ProvinceName);
                        dataRow.GetCell(6).SetCellValue(item.CityName);
                        dataRow.GetCell(7).SetCellValue(item.AreaName);
                        dataRow.GetCell(8).SetCellValue(item.CityTier);
                        dataRow.GetCell(9).SetCellValue(item.IsInstall);
                        dataRow.GetCell(10).SetCellValue(item.AgentCode);
                        dataRow.GetCell(11).SetCellValue(item.AgentName);
                        dataRow.GetCell(12).SetCellValue(item.Channel);
                        dataRow.GetCell(13).SetCellValue(item.Format);
                        dataRow.GetCell(14).SetCellValue(item.LocationType);
                        dataRow.GetCell(15).SetCellValue(item.BusinessModel);
                        dataRow.GetCell(16).SetCellValue(item.POPAddress);
                        dataRow.GetCell(17).SetCellValue(item.Contact1);
                        dataRow.GetCell(18).SetCellValue(item.Tel1);
                        dataRow.GetCell(19).SetCellValue(item.Contact2);
                        dataRow.GetCell(20).SetCellValue(item.Tel2);
                        if (item.OpeningDate != null)
                            dataRow.GetCell(21).SetCellValue(DateTime.Parse(item.OpeningDate.ToString()).ToShortDateString());
                        dataRow.GetCell(22).SetCellValue(item.Status);

                        startRow++;

                    }



                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();
                        sheet = null;
                        workBook = null;
                        OperateFile.DownLoadFile(ms, "店铺信息");

                    }
                }
            }
        }

        ShopBLL shopBll = new ShopBLL();
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
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
                int successNum = 0;
                int failNum = 0;
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataColumnCollection cols = ds.Tables[0].Columns;
                    DataTable errorTB = Common.CommonMethod.CreateErrorTB(cols);
                    int shopId = 0;
                    int provinceId = 0;
                    int cityId = 0;
                    int countyId = 0;

                    string shopNo = string.Empty;
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
                    string posScale = string.Empty;
                    string materialSupport = string.Empty;
                    Shop shopModel;
                    bool isexist = false;

                    FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                    FinalOrderDetailTemp orderModel;
                    List<int> guidanceIdList=new List<int>();
                    List<FinalOrderDetailTemp> emptyDataOrderList = new List<FinalOrderDetailTemp>();
                    if (!string.IsNullOrWhiteSpace(guidanceId))
                    {
                        guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
                        emptyDataOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                         join subject in CurrentContext.DbContext.Subject
                                         on order.SubjectId equals subject.Id
                                         where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                         && (order.POSScale == null || order.POSScale == ""
                                         || order.MaterialSupport == null || order.MaterialSupport == "")
                                         select order).ToList();
                    }
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        shopId = 0;
                        provinceId = 0;
                        cityId = 0;
                        countyId = 0;
                        shopNo = string.Empty;
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
                        posScale = string.Empty;
                        materialSupport = string.Empty;
                        bool canSave = true;
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

                        if (cols.Contains("County"))
                            county = dr["County"].ToString().Trim();
                        else if (cols.Contains("区"))
                            county = dr["区"].ToString().Trim();
                        else if (cols.Contains("区县"))
                            county = dr["区县"].ToString().Trim();
                        else if (cols.Contains("区/县"))
                            county = dr["区/县"].ToString().Trim();
                        else if (cols.Contains("县"))
                            county = dr["县"].ToString().Trim();

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
                        else if (cols.Contains("安装级别"))
                            isInstall = dr["安装级别"].ToString().Trim();

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

                        if (cols.Contains("店铺大小"))
                            posScale = dr["店铺大小"].ToString().Trim();
                        else if (cols.Contains("店铺规模大小"))
                            posScale = dr["店铺规模大小"].ToString().Trim();
                        else if (cols.Contains("店铺规模"))
                            posScale = dr["店铺规模"].ToString().Trim();

                        if (cols.Contains("店铺级别"))
                            materialSupport = dr["店铺级别"].ToString().Trim();
                        else if (cols.Contains("物料级别"))
                            materialSupport = dr["物料级别"].ToString().Trim();
                        else if (cols.Contains("物料支持级别"))
                            materialSupport = dr["物料支持级别"].ToString().Trim();
                        else if (cols.Contains("物料支持"))
                            materialSupport = dr["物料支持"].ToString().Trim();

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


                        if (cols.Contains("Opening Date"))
                            openDate = dr["Opening Date"].ToString().Trim();
                        else if (cols.Contains("OpeningDate"))
                            openDate = dr["OpeningDate"].ToString().Trim();
                        else if (cols.Contains("开店日期"))
                            openDate = dr["开店日期"].ToString().Trim();
                        if (openDate.Contains("0:00:00"))
                            openDate = openDate.Replace("0:00:00", "");
                        if (cols.Contains("Status"))
                            status = dr["Status"].ToString().Trim();
                        else if (cols.Contains("店铺状态"))
                            status = dr["店铺状态"].ToString().Trim();
                        else
                            status = "正常";
                        

                        if (string.IsNullOrWhiteSpace(shopNo))
                        {
                            canSave = false;
                            errorMsg.Append("店铺编号为空 ；");
                        }
                        if (string.IsNullOrWhiteSpace(shopName))
                        {
                            canSave = false;
                            errorMsg.Append("店铺名称为空 ；");
                        }
                        if (string.IsNullOrWhiteSpace(region))
                        {
                            canSave = false;
                            errorMsg.Append("区域为空 ；");
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


                        if (canSave)
                        {
                            isexist = CheckShop(shopNo, out shopId);
                            shopModel = new Models.Shop();
                            if (isexist)
                            {
                                shopModel = shopBll.GetModel(shopId);
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

                            if (!string.IsNullOrWhiteSpace(format))
                                shopModel.Format = format.Replace("（", "(").Replace("）", ")");
                            if (!string.IsNullOrWhiteSpace(isInstall))
                                shopModel.IsInstall = isInstall == "是" ? "Y" : isInstall == "否" ? "N" : isInstall;
                            if (!string.IsNullOrWhiteSpace(locationType))
                                shopModel.LocationType = locationType;
                            if (StringHelper.IsDateTime(openDate))
                                shopModel.OpeningDate = DateTime.Parse(openDate);
                            if (!string.IsNullOrWhiteSpace(adress))
                                shopModel.POPAddress = adress;

                            if (!string.IsNullOrWhiteSpace(shopName))
                                shopModel.ShopName = shopName;
                            if (!string.IsNullOrWhiteSpace(status))
                            {
                                if (status.Contains("闭"))
                                    status = "关闭";
                                shopModel.Status = status;
                            }
                            if (!string.IsNullOrWhiteSpace(materialSupport) || !string.IsNullOrWhiteSpace(posScale))
                            {
                                if (!string.IsNullOrWhiteSpace(materialSupport))
                                    shopModel.MaterialSupport = materialSupport;
                                if (!string.IsNullOrWhiteSpace(posScale))
                                    shopModel.POSScale = posScale;

                                if (emptyDataOrderList.Any())
                                {
                                    var shopOrderList = emptyDataOrderList.Where(order => order.ShopId==shopId).ToList();
                                    if (shopOrderList.Any())
                                    {
                                        shopOrderList.ForEach(order =>
                                        {
                                            orderModel = new FinalOrderDetailTemp();
                                            orderModel = order;
                                            if (string.IsNullOrWhiteSpace(orderModel.POSScale))
                                            {
                                                orderModel.POSScale = posScale;
                                                orderModel.InstallPricePOSScale = posScale;
                                            }
                                            if (string.IsNullOrWhiteSpace(orderModel.MaterialSupport))
                                                orderModel.MaterialSupport = materialSupport;
                                            orderBll.Update(orderModel);
                                        });
                                    }
                                }
                            }

                            if (isexist)
                            {
                                shopBll.Update(shopModel);
                            }
                            //else
                            //{
                            //    shopModel.AddDate = DateTime.Now;
                            //    shopModel.ShopNo = shopNo.ToUpper();
                            //    shopModel.IsDelete = false;
                            //    shopBll.Add(shopModel);
                            //}

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
                        Session["updatErrorTb"] = errorTB;
                    }
                    else
                        Session["updatErrorTb"] = null;
                }
                conn.Dispose();
                conn.Close();
                Response.Redirect(string.Format("EmptyDateWarn.aspx?import=1&guidanceId={0}&successNum={1}&failNum={2}", guidanceId, successNum, failNum), false);
            }
        }


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


        List<Models.Place> placeList = new List<Models.Place>();
        PlaceBLL placeBll = new PlaceBLL();
        /// <summary>
        /// 区域Id
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>

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

        protected void lbDownLoadErrorMsg_Click(object sender, EventArgs e)
        {

        }


    }
}
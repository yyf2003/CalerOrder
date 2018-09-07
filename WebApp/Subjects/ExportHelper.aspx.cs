using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using BLL;
using Common;
using DAL;
using Models;
using NPOI.SS.UserModel;
using System.Data;
using NPOI;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;

//using NPOI.HSSF.UserModel;
//using NPOI.HSSF.UserModel;

//using NPOI.HPSF;
//using NPOI.HSSF;
//using NPOI.HSSF.UserModel;
//using NPOI.POIFS;
//using NPOI.Util;
//using NPOI.HSSF.Util;
//using NPOI.HSSF.Extractor;
//using NPOI.XSSF.UserModel;


namespace WebApp.Subjects
{
    public partial class ExportHelper : BasePage
    {
        string type = string.Empty;
        int selectType = 1;
        string subjectId = string.Empty;
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        string customerServiceId = string.Empty;
        string isInstall = string.Empty;
        string materialCategoryId = string.Empty;
        string exportType = string.Empty;
        string exportShopNo = string.Empty;
        string isNotInclude = string.Empty;
        string materialName = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["type"] != null)
            {
                type = Request.QueryString["type"];
            }
            if (Request.QueryString["selecttype"] != null)
            {
                selectType = int.Parse(Request.QueryString["selecttype"]);
            }
            if (Request.QueryString["subjectids"] != null)
            {
                subjectId = Request.QueryString["subjectids"];
            }
            if (Request.QueryString["regions"] != null)
            {
                region = Request.QueryString["regions"];
            }
            if (Request.QueryString["province"] != null)
            {
                province = Request.QueryString["province"];
            }

            if (Request.QueryString["customerService"] != null)
            {
                customerServiceId = Request.QueryString["customerService"];
            }
            if (Request.QueryString["isInstall"] != null)
            {
                isInstall = Request.QueryString["isInstall"];
            }
            if (Request.QueryString["materialCategory"] != null)
            {
                materialCategoryId = Request.QueryString["materialCategory"];
            }

            if (Request.QueryString["exportType"] != null)
            {
                exportType =Request.QueryString["exportType"];
            }
            if (Request.QueryString["exportShopNo"] != null)
            {
                exportShopNo = Request.QueryString["exportShopNo"];
            }
            if (Request.QueryString["isNotInclude"] != null)
            {
                isNotInclude = Request.QueryString["isNotInclude"];
            }
            if (Request.QueryString["materialName"] != null)
            {
                materialName = Request.QueryString["materialName"];
            }
            HttpCookie cookie = Request.Cookies["emptyData"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddSeconds(-1);
                Response.Cookies.Add(cookie);
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            city = hfCityData.Value;
            switch (type)
            {

                case "export":
                    ExportOrder();
                    break;
                case "export350":
                    Export350();
                    break;
                case "export350withempty":
                    Export350(true);
                    break;
                case "exportbjphw":
                    ExportBJPHW();
                    break;
                case "exportotherphw":
                    ExportOtherPHW();
                    break;
                case "exportOutsourcingOrder"://导出外协竖版订单
                    ExportOutsourcingOrder();
                    break;
                case "exportOutsourcingOrder350"://导出外协350订单
                    ExportOutsourcingOrder350();
                    break;
                case "exportQuote350"://导出报价350订单
                    ExportQuote350();
                    break;
                //case "exportPackingList"://导出装箱单
                //    ExportPackingList();
                //    break;
                case "exportNew350"://导出350订单(系统订单)
                    ExportNew350();
                    break;
            }
        }


        string ExportOrder()
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                StringBuilder whereSql = new StringBuilder();

                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                    StringBuilder regions = new StringBuilder();
                    regionList.ForEach(s =>
                    {
                        regions.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and Region in({0})", regions.ToString().TrimEnd(','));
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    StringBuilder provinces = new StringBuilder();
                    provinceList.ForEach(s =>
                    {
                        provinces.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and Province in({0})", provinces.ToString().TrimEnd(','));
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    StringBuilder citys = new StringBuilder();
                    cityList.ForEach(s =>
                    {
                        citys.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and City in({0})", citys.ToString().TrimEnd(','));
                }

                //if (searchType == "1")
                    //whereSql.AppendFormat(" and [客服]={0}", CurrentUser.UserId);
                FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                System.Data.DataSet ds = orderBll.GetOrderList(subjectId, whereSql.ToString(), "true");
                int colNum = ds.Tables[0].Columns.Count;
                ds.Tables[0].Columns.RemoveAt(colNum - 1);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {





                    string fileName = "POP订单数据";

                    OperateFile.ExportExcel(ds.Tables[0], fileName);

                }
                else
                    result = "没有数据可以导出";
            }
            else
                result = "没有数据可以导出";
            return result;
        }

        void Export350(bool? withEmpty = null)
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> customerServiceList = new List<int>();
                List<string> installList = new List<string>();
                List<int> categoryList = new List<int>();

                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join pop1 in CurrentContext.DbContext.POP
                            on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                            from pop in popTemp.DefaultIfEmpty()
                            //pop订单中，pop尺寸为空的不导出
                            where subjectIdList.Contains(order.SubjectId ?? 0)

                                //&& !shop.Format.Contains("Homecourt")
                            && (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
                            && (order.IsDelete == null || order.IsDelete == false)

                            select new
                            {
                                subject,
                                shop,
                                order,
                                pop
                            }).ToList();

                if (withEmpty == null)
                {
                    list = list.Where(s => (s.order.OrderType == 1 && s.order.GraphicLength != null && s.order.GraphicLength > 0 && s.order.GraphicWidth != null && s.order.GraphicWidth > 0) || (s.order.OrderType == 2)).ToList();
                }
                //if (searchType == "1")
                //{
                //    list = list.Where(s => s.shop.CSUserId == CurrentUser.UserId).ToList();
                //}
                //if (!string.IsNullOrWhiteSpace(region))
                //{
                //    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                //    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                //}
                //List<string> regionList = new List<string>();
                if (string.IsNullOrWhiteSpace(region))
                {
                    regionList = new BasePage().GetResponsibleRegion;
                    if (regionList.Any())
                    {
                        StringHelper.ToUpperOrLowerList(ref regionList, LowerUpperEnum.ToLower);
                    }
                }
                else
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                if (regionList.Any())
                {
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                        }
                        else
                            list = list.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                    }
                    else
                        list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    //list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    //list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    if (provinceList.Contains("空"))
                    {
                        List<string> provinceList0 = provinceList;
                        provinceList0.Remove("空");
                        if (provinceList0.Any())
                        {
                            list = list.Where(s => provinceList0.Contains(s.shop.ProvinceName) || s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                            
                        }
                        else
                        {
                            list = list.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                           
                        }
                    }
                    else
                    {
                        list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                        
                    }
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    //list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();

                    if (cityList.Contains("空"))
                    {
                        List<string> cityList0 = cityList;
                        cityList0.Remove("空");
                        if (cityList0.Any())
                        {
                            list = list.Where(s => cityList0.Contains(s.shop.CityName) || s.shop.CityName == null || s.shop.CityName == "").ToList();
                            
                        }
                        else
                        {
                            list = list.Where(s => s.shop.CityName == null || s.shop.CityName == "").ToList();
                            
                        }
                    }
                    else
                    {
                        list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                       
                    }

                }
                if (!string.IsNullOrWhiteSpace(customerServiceId))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceId, ',');
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                        }
                        else
                            list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    installList = StringHelper.ToStringList(isInstall, ',');
                    if (installList.Contains("无"))
                    {
                        installList.Remove("无");
                        if (installList.Any())
                        {
                            list = list.Where(s => installList.Contains(s.shop.IsInstall) || (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                        }
                        else
                            list = list.Where(s => (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                    }
                    else
                        list = list.Where(s => installList.Contains(s.shop.IsInstall)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(materialCategoryId))
                {
                    bool hasEmpty = false;
                    List<string> materialList = new List<string>();
                    categoryList = StringHelper.ToIntList(materialCategoryId, ',');
                    if (categoryList.Contains(0))
                    {
                        hasEmpty = true;
                        categoryList.Remove(0);
                    }
                    if (categoryList.Any())
                    {
                        materialList = new OrderMaterialMppingBLL().GetList(s => categoryList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                    }
                    if (hasEmpty)
                    {
                        if (materialList.Any())
                        {
                            list = list.Where(s => materialList.Contains(s.order.GraphicMaterial.ToLower()) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                        }
                        else
                            list = list.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                    }
                    else
                        list = list.Where(s => materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                }

                List<Order350Model> orderList = new List<Order350Model>();
                int guidanceId = list.Select(s => s.subject.GuidanceId ?? 0).FirstOrDefault();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        string status = string.Empty;
                        if (item.shop.Status != null)
                            status = item.shop.Status;
                        bool isClose = false;
                        if (!string.IsNullOrWhiteSpace(status) && (status.Contains("关") || status.Contains("闭")))
                        {
                            isClose = true;
                        }
                        if (!isClose)
                        {
                            int level = item.order.LevelNum ?? 0;
                            string levelName = string.Empty;

                            if (level > 0)
                                levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());
                            Order350Model model = new Order350Model();
                            decimal area = 0;
                            if (item.order.GraphicWidth != null && item.order.GraphicLength != null)
                            {
                                area = ((item.order.GraphicWidth ?? 0) * (item.order.GraphicLength ?? 0)) / 1000000;
                            }
                            model.Area = double.Parse(area.ToString());
                            //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                            model.Category = (item.order.Category != null && item.order.Category != "") ? item.order.Category : item.pop != null ? item.pop.Category : "";
                            model.ChooseImg = item.order.ChooseImg;
                            model.City = item.shop.CityName;
                            model.County = item.shop.AreaName;
                            model.CityTier = item.shop.CityTier;
                            model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                            model.Format = item.shop.Format;
                            model.Gender = item.order.Gender;
                            model.GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                            string smallMaterial = GetSmallMaterial(item.order.SmallMaterialId ?? 0);
                            string material = item.order.GraphicMaterial;
                            if (!string.IsNullOrWhiteSpace(smallMaterial))
                                material += ("+" + smallMaterial);
                            model.GraphicMaterial = material;
                            model.GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0; ;
                            model.POPAddress = item.shop.POPAddress;
                            model.PositionDescription = item.order.PositionDescription;
                            model.Province = item.shop.ProvinceName;
                            model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                            model.Sheet = item.order.Sheet;
                            model.ShopName = item.shop.ShopName;
                            model.ShopNo = item.shop.ShopNo;
                            string subjectName = item.subject.SubjectName;
                            if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                                subjectName += "(" + item.subject.Remark + ")";
                            model.SubjectName = subjectName;
                            model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                            model.OtherRemark = levelName + " " + item.order.Remark;
                            model.POSScale = item.order.POSScale;
                            model.MaterialSupport = item.order.MaterialSupport;
                            //model.ShopLevel = item.shop.ShopLevel;
                            orderList.Add(model);
                        }
                    }
                }
                #region 物料导出

                var orderMaterialList = (from material in CurrentContext.DbContext.OrderMaterial
                                         join subject in CurrentContext.DbContext.Subject
                                         on material.SubjectId equals subject.Id
                                         join shop in CurrentContext.DbContext.Shop
                                         on material.ShopId equals shop.Id
                                         where subjectIdList.Contains(material.SubjectId ?? 0)
                                         select new
                                         {
                                             material,
                                             subject,
                                             shop
                                         }).ToList();
                //if (searchType == "1")
                //{
                //    orderMaterialList = orderMaterialList.Where(s => s.shop.CSUserId == CurrentUser.UserId).ToList();
                //}
                if (regionList.Any())
                {
                    //orderMaterialList = orderMaterialList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => regionList0.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                        }
                        else
                            orderMaterialList = orderMaterialList.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                    }
                    else
                        orderMaterialList = orderMaterialList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }

                if (provinceList.Any())
                {
                    //orderMaterialList = orderMaterialList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    if (provinceList.Contains("空"))
                    {
                        List<string> provinceList0 = provinceList;
                        provinceList0.Remove("空");
                        if (provinceList0.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => provinceList0.Contains(s.shop.ProvinceName) || s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                            
                        }
                        else
                        {
                            orderMaterialList = orderMaterialList.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                           
                        }
                    }
                    else
                    {
                        orderMaterialList = orderMaterialList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                        
                    }
                }
                if (cityList.Any())
                {
                    //orderMaterialList = orderMaterialList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    if (cityList.Contains("空"))
                    {
                        List<string> cityList0 = cityList;
                        cityList0.Remove("空");
                        if (cityList0.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => cityList0.Contains(s.shop.CityName) || s.shop.CityName == null || s.shop.CityName == "").ToList();
                            
                        }
                        else
                        {
                            orderMaterialList = orderMaterialList.Where(s => s.shop.CityName == null || s.shop.CityName == "").ToList();
                            
                        }
                    }
                    else
                    {
                        orderMaterialList = orderMaterialList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                       
                    }
                }
                if (customerServiceList.Any())
                {
                   if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                        }
                        else
                            orderMaterialList = orderMaterialList.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                    }
                    else
                    {
                        orderMaterialList = orderMaterialList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                    }
                }
                if (installList.Any())
                {
                   if (installList.Contains("无"))
                    {
                        installList.Remove("无");
                        if (installList.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => installList.Contains(s.shop.IsInstall) || (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                        }
                        else
                            orderMaterialList = orderMaterialList.Where(s => (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                    }
                    else
                       orderMaterialList = orderMaterialList.Where(s => installList.Contains(s.shop.IsInstall)).ToList();
                }

                if (orderMaterialList.Any())
                {
                    orderMaterialList.ForEach(s =>
                    {
                        Order350Model model = new Order350Model();
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = s.shop.CityName;
                        model.County = s.shop.AreaName;
                        model.CityTier = s.shop.CityTier;
                        model.Contacts = s.shop.Contact1 + "/" + s.shop.Contact2;
                        model.Format = s.shop.Format;
                        model.Gender = "";
                        model.GraphicLength = 0;
                        model.GraphicMaterial = "";
                        model.GraphicWidth = 0;
                        model.POPAddress = s.shop.POPAddress;
                        StringBuilder size = new StringBuilder();
                        if (s.material.MaterialLength != null && s.material.MaterialLength > 0 && s.material.MaterialWidth != null && s.material.MaterialWidth > 0)
                        {
                            size.AppendFormat("({0}*{1}", s.material.MaterialLength, s.material.MaterialWidth);
                            if (s.material.MaterialHigh != null && s.material.MaterialHigh > 0)
                                size.AppendFormat("*{0}", s.material.MaterialHigh);
                            size.Append(")");
                        }
                        model.PositionDescription = s.material.MaterialName + size.ToString();
                        model.Province = s.shop.ProvinceName;
                        model.Quantity = s.material.MaterialCount != null ? double.Parse(s.material.MaterialCount.ToString()) : 0;
                        model.Sheet = s.material.Sheet;
                        model.ShopName = s.shop.ShopName;
                        model.ShopNo = s.shop.ShopNo;
                        model.SubjectName = s.subject.SubjectName;
                        model.Tels = s.shop.Tel1 + "/" + s.shop.Tel2;
                        model.OtherRemark = s.material.Remark;
                        model.ShopLevel = s.shop.ShopLevel;
                        orderList.Add(model);
                    });
                }



                #endregion
                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopNo).ToList();
                    string templateFileName = "Export350Template";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);

                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read);

                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                    ISheet sheet = workBook.GetSheetAt(0);
                   
                    int startRow = 1;
                    string shopno = string.Empty;
                    foreach (var item in orderList)
                    {


                        if (startRow == 1)
                            shopno = item.ShopNo;
                        else
                        {
                            if (shopno != item.ShopNo)
                            {
                                shopno = item.ShopNo;
                                int row = startRow + 1;
                                while (startRow.ToString().Substring(startRow.ToString().Length - 1, 1) != "2")
                                {
                                    startRow++;
                                    row = startRow + 1;
                                }

                            }
                        }




                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 30; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);
                        }
                        dataRow.GetCell(0).SetCellValue(item.ShopNo);
                        dataRow.GetCell(1).SetCellValue(item.ShopName);
                        dataRow.GetCell(2).SetCellValue(item.Province);
                        dataRow.GetCell(3).SetCellValue(item.City);
                        dataRow.GetCell(4).SetCellValue(item.CityTier);
                        dataRow.GetCell(5).SetCellValue(item.Format);
                        dataRow.GetCell(6).SetCellValue(item.POPAddress);
                        dataRow.GetCell(7).SetCellValue(item.Contacts);
                        dataRow.GetCell(8).SetCellValue(item.Tels);
                        dataRow.GetCell(9).SetCellValue(item.ChooseImg);
                        dataRow.GetCell(10).SetCellValue(item.SubjectName);
                        dataRow.GetCell(11).SetCellValue(item.Gender);
                        dataRow.GetCell(12).SetCellValue(item.Category);
                        dataRow.GetCell(13).SetCellValue(item.Sheet);
                        dataRow.GetCell(14).SetCellValue(item.Quantity);
                        dataRow.GetCell(15).SetCellValue(item.GraphicMaterial);
                        dataRow.GetCell(16).SetCellValue(item.GraphicWidth);
                        dataRow.GetCell(17).SetCellValue(item.GraphicLength);
                        dataRow.GetCell(18).SetCellValue(item.Area);
                        dataRow.GetCell(19).SetCellValue(item.OtherRemark);
                        //其他备注
                        dataRow.GetCell(20).SetCellValue(item.PositionDescription);
                        startRow++;

                    }
                  
                    HttpCookie cookie = Request.Cookies["export350"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("export350");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();
                        sheet = null;
                        workBook = null;
                        string fileName = string.Empty;
                        SubjectGuidance model = new SubjectGuidanceBLL().GetModel(guidanceId);
                        if (model != null)
                        {
                            fileName = model.ItemName + "-";
                        }
                        OperateFile.DownLoadFile(ms, fileName + "350总表");

                    }
                }
                else
                {
                   

                    HttpCookie cookie = Request.Cookies["export350"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("export350");
                    }
                    cookie.Value = "2";//没有数据可以导出
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                }




            }
        }

        void ExportBJPHW()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                List<int> guidanceIdList = new List<int>();
                //List<int> promotionShopList = GetFreightShopList(subjectIdList);
                List<string> myRegionList = new BasePage().GetResponsibleRegion;
                if (myRegionList.Any())
                {
                    StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
                }
                
               
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on subject.GuidanceId equals guidance.ItemId
                            join category1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals category1.Id into temp1
                            from category in temp1.DefaultIfEmpty()
                            //where subjectIdList.Contains(order.SubjectId ?? 0)
                            where (selectType == 1 ? subjectIdList.Contains(order.SubjectId ?? 0) : subjectIdList.Contains(order.RegionSupplementId ?? 0))
                            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType>1)
                            
                            && (order.IsValid == null || order.IsValid == true)
                           // && ((order.Province == "北京" || order.Province == "天津"))
                            && (order.IsDelete == null || order.IsDelete == false)
                            && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                            && (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                            && order.OrderType!=(int)OrderTypeEnum.物料
                            select new
                            {
                                subject,
                                shop,
                                order,
                                CategoryName=category!=null?category.CategoryName:"",
                               guidance
                            }).ToList();
                list = list.Where(s => ((s.order.Sheet!=null && s.order.Sheet.Contains("光盘")) || (s.order.PositionDescription != null && s.order.PositionDescription.Contains("光盘"))) ? ((s.order.Format != null && s.order.Format != "") && s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                if (!string.IsNullOrWhiteSpace(exportType))
                {
                    if (exportType == "nohc")
                    {
                        list = list.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                    }
                    else if (exportType == "hc")
                    {
                        list = list.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") != -1 || s.order.Format.ToLower().IndexOf("homecourt") != -1 || s.order.Format.ToLower().IndexOf("homecore") != -1 || s.order.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                    }
                }

                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    if (materialName == "软膜")
                    {
                        list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                    }
                    else if (materialName == "非软膜")
                    {
                        list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
                    }
                }


                if (string.IsNullOrWhiteSpace(region))
                {
                    //regionList = new BasePage().GetResponsibleRegion;
                    //if (regionList.Any())
                    //{
                    //    StringHelper.ToUpperOrLowerList(ref regionList, LowerUpperEnum.ToLower);
                    //}
                }
                else
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                if (regionList.Any())
                {
                    //list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();
                        }
                        else
                            list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                    }
                    else
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                        //list = list.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    //list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    if (provinceList.Contains("空"))
                    {
                        List<string> provinceList0 = provinceList;
                        provinceList0.Remove("空");
                        if (provinceList0.Any())
                        {
                            list = list.Where(s => provinceList0.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                        }
                        else
                        {
                            list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                        }
                    }
                    else
                    {
                        list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();

                    }
                }
                else
                {
                    //list = list.Where(s => s.shop.ProvinceName == "北京" || s.shop.ProvinceName == "天津").ToList();
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    
                    if (cityList.Contains("空"))
                    {
                        List<string> cityList0 = cityList;
                        cityList0.Remove("空");
                        if (cityList0.Any())
                        {
                            list = list.Where(s => cityList0.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                        }
                        else
                        {
                            list = list.Where(s => s.order.City == null || s.order.City == "").ToList();

                        }
                    }
                    else
                    {
                        list = list.Where(s => cityList.Contains(s.order.City)).ToList();

                    }
                }
                
                if (!string.IsNullOrWhiteSpace(customerServiceId))
                {
                    List<int> customerServiceList = StringHelper.ToIntList(customerServiceId, ',');
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                        }
                        else
                            list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    List<string> installList = StringHelper.ToStringList(isInstall, ',');
                    if (installList.Contains("无"))
                    {
                        installList.Remove("无");
                        if (installList.Any())
                        {
                            list = list.Where(s => installList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                        }
                        else
                            list = list.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                    }
                    else
                        list = list.Where(s => installList.Contains(s.order.IsInstall)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(materialCategoryId))
                {
                    bool hasEmpty = false;
                    List<string> materialList = new List<string>();
                    List<int> categoryList = StringHelper.ToIntList(materialCategoryId, ',');
                    if (categoryList.Contains(0))
                    {
                        hasEmpty = true;
                        categoryList.Remove(0);
                    }
                    if (categoryList.Any())
                    {
                        materialList = new OrderMaterialMppingBLL().GetList(s => categoryList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                    }
                    if (hasEmpty)
                    {
                        if (materialList.Any())
                        {
                            list = list.Where(s => (s.order.GraphicMaterial!=null && materialList.Contains(s.order.GraphicMaterial.ToLower())) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                        }
                        else
                            list = list.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                    }
                    else
                        list = list.Where(s => s.order.GraphicMaterial != null && materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(exportShopNo))
                {
                    List<string> exportShopNoList = StringHelper.ToStringList(exportShopNo.Replace("，",","), ',', LowerUpperEnum.ToLower); ;
                    if (exportShopNoList.Any())
                    {
                        if (isNotInclude == "1")
                        {
                            list = list.Where(s => !exportShopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
                        }
                        else
                        {
                            list = list.Where(s => exportShopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
                        }
                    }
                }
                List<Order350Model> orderList = new List<Order350Model>();
                if (list.Any())
                {
                   
                    guidanceIdList = list.Select(s=>s.guidance.ItemId).Distinct().ToList();
                    List<int> shopIdList = list.Select(s => s.shop.Id).Distinct().ToList();
                    string changePOPCountSheetStr = string.Empty;
                    List<string> ChangePOPCountSheetList = new List<string>();
                    try
                    {
                        changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrWhiteSpace(changePOPCountSheetStr))
                    {
                        ChangePOPCountSheetList = StringHelper.ToStringList(changePOPCountSheetStr, '|');
                    }
                    //var HCSmallSizeList = new HCSmallGraphicSizeBLL().GetList(s => s.Id > 0);
                    foreach (var item in list)
                    {

                        int level = item.order.LevelNum ?? 0;
                        string levelName = string.Empty;

                        if (level > 0)
                            levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());

                        double GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                        double GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0;
                        bool canGo = true;
                        if (!string.IsNullOrWhiteSpace(item.order.GraphicNo) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(item.order.Sheet.ToUpper()))
                        {
                            //去掉重复的（同一个编号下多次的）
                            var checkList = orderList.Where(s => s.SubjectId == item.subject.Id && s.ShopId == item.shop.Id && s.Sheet == item.order.Sheet && s.PositionDescription == item.order.PositionDescription && s.GraphicNo == item.order.GraphicNo && s.GraphicLength == GraphicLength && s.GraphicWidth == GraphicWidth && s.Gender==item.order.Gender).ToList();
                            if (checkList.Any())
                                canGo = false;
                        }
                        if (canGo)
                        {
                            Order350Model model = new Order350Model();

                            string orderTypeName = CommonMethod.GetEnumDescription<OrderTypeEnum>((item.order.OrderType ?? 1).ToString());
                            if (orderTypeName == "费用订单")
                            {
                                model.UnitPrice = double.Parse((item.order.OrderPrice ?? 0).ToString());
                                model.ShopId = item.shop.Id;
                                model.SubjectId = 0;
                                model.GraphicNo = "";

                                model.Area = 0;
                                //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                model.Category = "";
                                model.ChooseImg = "";
                                model.City = "";
                                //model.County = item.shop.AreaName;
                                model.CityTier = "";
                                model.Contacts = "";
                                model.Format = "";
                                model.Gender = "";
                                //model.GraphicLength = 0;
                                model.GraphicMaterial = CommonMethod.GeEnumName<OrderTypeEnum>((item.order.OrderType ?? 1).ToString());
                                //model.GraphicWidth = 0;
                                model.POPAddress = "";
                                model.PositionDescription = "";
                                model.Province = "";
                                // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                model.Quantity = 1;
                                model.Sheet = "";
                                model.ShopName = item.order.ShopName;
                                model.ShopNo = item.order.ShopNo;

                                model.SubjectName = "";
                                model.Tels = "";
                                //model.OtherRemark = levelName;

                                model.POSScale = "";
                                model.MaterialSupport = "";
                                model.GuidanceName = item.guidance != null ? item.guidance.ItemName : "";
                                model.OrderType = CommonMethod.GeEnumName<OrderTypeEnum>((item.order.OrderType??1).ToString());
                                orderList.Add(model);
                            }
                            else
                            {
                                model.ShopId = item.shop.Id;
                                model.SubjectId = item.subject.Id;
                                model.City = item.order.City;
                                model.Format = item.order.Format;
                                model.GraphicNo = item.order.GraphicNo;
                                decimal area = 0;
                                if (item.order.GraphicWidth != null && item.order.GraphicLength != null)
                                {
                                    area = ((item.order.GraphicWidth ?? 0) * (item.order.GraphicLength ?? 0)) / 1000000;
                                }
                                model.Area = double.Parse(area.ToString());
                                //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                model.Category = item.order.Category;
                                model.ChooseImg = item.order.ChooseImg;

                                //model.County = item.shop.AreaName;
                                model.CityTier = item.order.CityTier;
                                model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;

                                model.Gender = !string.IsNullOrWhiteSpace(item.order.OrderGender) ? item.order.OrderGender : item.order.Gender;
                                model.GraphicLength = GraphicLength;
                                model.GraphicMaterial = item.order.GraphicMaterial;
                                model.GraphicWidth = GraphicWidth;
                                model.POPAddress = item.order.POPAddress;
                                model.PositionDescription = item.order.PositionDescription;
                                model.Province = item.order.Province;
                                // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                if (item.order.Sheet!=null&&ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(item.order.Sheet.ToUpper()))
                                {
                                    model.Quantity = (item.order.Quantity ?? 0) > 0 ? 1 : 0;
                                }
                                else
                                {
                                    model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                }
                                model.Sheet = item.order.Sheet;
                                model.ShopName = item.order.ShopName;
                                model.ShopNo = item.order.ShopNo;
                                string subjectName = item.subject.SubjectName;
                                if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                                    subjectName += "(" + item.subject.Remark + ")";
                                model.SubjectName = subjectName;
                                model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                                //model.OtherRemark = levelName;
                                model.IsPOPMaterial = item.order.IsPOPMaterial;
                                model.POSScale = item.order.POSScale;
                                model.MaterialSupport = item.order.MaterialSupport;
                                model.GuidanceName = item.guidance.ItemName;
                                model.OrderType = CommonMethod.GeEnumName<OrderTypeEnum>((item.order.OrderType ?? 1).ToString());
                                orderList.Add(model);
                            }
                        }
                    }



                    //var installList = new InstallPriceShopInfoBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
                    //非分区活动安装费
                    //var installList = (from install in CurrentContext.DbContext.InstallPriceShopInfo
                    //                  join shop in CurrentContext.DbContext.Shop
                    //                  on install.ShopId equals shop.Id
                    //                  join subject in CurrentContext.DbContext.Subject
                    //                  on install.SubjectId equals subject.Id
                    //                  join guidance in CurrentContext.DbContext.SubjectGuidance
                    //                  on subject.GuidanceId equals guidance.ItemId
                    //                  //from guidance in temp.DefaultIfEmpty()
                    //                  where subjectIdList.Contains(install.SubjectId??0)
                    //                  && shopIdList.Contains(install.ShopId??0)
                    //                  && guidance.ActivityTypeId  != (int)GuidanceTypeEnum.Others
                    //                  select new {
                    //                      install,
                    //                      shop,
                    //                      guidance
                    //                  }).ToList();
                    #region 新

                    List<int> genericShopIdList = list.Where(s => s.CategoryName != null && s.CategoryName.Contains("常规-非活动")).Select(s => s.order.ShopId ?? 0).ToList();
                    List<int> normalShopIdList = list.Where(s => s.CategoryName == null || (s.CategoryName != null && !s.CategoryName.Contains("常规-非活动"))).Select(s => s.order.ShopId ?? 0).ToList();

                    var installList = (from install in CurrentContext.DbContext.InstallPriceTemp
                                       join shop in CurrentContext.DbContext.Shop
                                       on install.ShopId equals shop.Id
                                       join guidance in CurrentContext.DbContext.SubjectGuidance
                                       on install.GuidanceId equals guidance.ItemId
                                       //from guidance in temp.DefaultIfEmpty()
                                       where guidanceIdList.Contains(guidance.ItemId)
                                       && shopIdList.Contains(install.ShopId ?? 0)
                                       && guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others
                                       select new
                                       {
                                           install,
                                           shop,
                                           guidance
                                       }).ToList();

                    #endregion
                    if (installList.Any())
                    {

                        var genericInstallList = installList.Where(s => genericShopIdList.Contains(s.install.ShopId ?? 0) && s.install.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费).ToList();
                        var notGenericInstallList = installList.Where(s => normalShopIdList.Contains(s.install.ShopId ?? 0) && s.install.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费).ToList();

                        genericInstallList.ForEach(s =>
                        {
                            decimal installPrice = (s.install.BasicPrice ?? 0) + (s.install.OOHPrice ?? 0) + (s.install.WindowPrice ?? 0);
                            //decimal installPrice = s.install.TotalPrice ?? 0;
                            if (installPrice > 0)
                            {
                                Order350Model model = new Order350Model();
                                model.UnitPrice = double.Parse(installPrice.ToString());
                                model.ShopId = s.shop.Id;
                                model.SubjectId = 0;
                                model.GraphicNo = "";

                                model.Area = 0;
                                //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                model.Category = "";
                                model.ChooseImg = "";
                                model.City = "";
                                //model.County = item.shop.AreaName;
                                model.CityTier = "";
                                model.Contacts = "";
                                model.Format = "";
                                model.Gender = "";
                                //model.GraphicLength = 0;
                                model.GraphicMaterial = "安装费";
                                //model.GraphicWidth = 0;
                                model.POPAddress = "";
                                model.PositionDescription = "";
                                model.Province = "";
                                // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                model.Quantity = 1;
                                model.Sheet = "";
                                model.ShopName = s.shop.ShopName;
                                model.ShopNo = s.shop.ShopNo;

                                model.SubjectName = "";
                                model.Tels = "";
                                //model.OtherRemark = levelName;

                                model.POSScale = "";
                                model.MaterialSupport = "";
                                model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                                model.OrderType = "安装费";
                                orderList.Add(model);
                            }

                        });
                        notGenericInstallList.ForEach(s =>
                        {
                            decimal installPrice = (s.install.BasicPrice ?? 0) + (s.install.OOHPrice ?? 0) + (s.install.WindowPrice ?? 0);
                            //decimal installPrice = s.install.TotalPrice ?? 0;
                            if (installPrice > 0)
                            {
                                Order350Model model = new Order350Model();
                                model.UnitPrice = double.Parse(installPrice.ToString());
                                model.ShopId = s.shop.Id;
                                model.SubjectId = 0;
                                model.GraphicNo = "";

                                model.Area = 0;
                                //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                model.Category = "";
                                model.ChooseImg = "";
                                model.City = "";
                                //model.County = item.shop.AreaName;
                                model.CityTier = "";
                                model.Contacts = "";
                                model.Format = "";
                                model.Gender = "";
                                //model.GraphicLength = 0;
                                model.GraphicMaterial = "安装费";
                                //model.GraphicWidth = 0;
                                model.POPAddress = "";
                                model.PositionDescription = "";
                                model.Province = "";
                                // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                model.Quantity = 1;
                                model.Sheet = "";
                                model.ShopName = s.shop.ShopName;
                                model.ShopNo = s.shop.ShopNo;

                                model.SubjectName = "";
                                model.Tels = "";
                                //model.OtherRemark = levelName;

                                model.POSScale = "";
                                model.MaterialSupport = "";
                                model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                                model.OrderType = "安装费";
                                orderList.Add(model);
                            }

                        });
                    }

                    //发货费
                    var ExperssShopList = list.Where(s => s.guidance.ActivityTypeId == (int)GuidanceTypeEnum.Promotion || s.guidance.ActivityTypeId == (int)GuidanceTypeEnum.Delivery).ToList();
                    if (ExperssShopList.Any())
                    {
                        List<int> expressGuidanceIdList = ExperssShopList.Select(s=>s.guidance.ItemId).Distinct().ToList();
                        //var priceOrderList = new PriceOrderDetailBLL().GetList(s =>s.GuidanceId!=null && expressGuidanceIdList.Contains(s.GuidanceId??0)).ToList();
                        var priceOrderList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId != null && expressGuidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();

                        List<int> ExperssShopIdList = new List<int>();
                        ExperssShopList.ForEach(s => {
                            if (!ExperssShopIdList.Contains(s.shop.Id))
                            {
                                decimal expressPrice = 0;
                                var priceModel = priceOrderList.Where(p => p.ShopId == s.shop.Id).FirstOrDefault();
                                if (priceModel != null)
                                {
                                    expressPrice = priceModel.ExpressPrice ?? 0;
                                }
                                if (expressPrice > 0)
                                {
                                    Order350Model model = new Order350Model();

                                    model.ShopId = s.shop.Id;
                                    model.SubjectId = 0;
                                    model.GraphicNo = "";

                                    model.Area = 0;
                                    //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                    model.Category = "";
                                    model.ChooseImg = "";
                                    model.City = "";
                                    //model.County = item.shop.AreaName;
                                    model.CityTier = "";
                                    model.Contacts = "";
                                    model.Format = "";
                                    model.Gender = "";
                                    //model.GraphicLength = 0;
                                    model.GraphicMaterial = "发货费";
                                    //model.GraphicWidth = 0;
                                    model.POPAddress = "";
                                    model.PositionDescription = "";
                                    model.Province = "";
                                    // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                    model.Quantity = 1;
                                    model.Sheet = "";
                                    model.ShopName = s.shop.ShopName;
                                    model.ShopNo = s.shop.ShopNo;

                                    model.SubjectName = "";
                                    model.Tels = "";
                                    //model.OtherRemark = levelName;

                                    model.POSScale = "";
                                    model.MaterialSupport = "";
                                    model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                                    model.OrderType = "发货费";
                                    model.UnitPrice = double.Parse(expressPrice.ToString());
                                    orderList.Add(model);
                                    ExperssShopIdList.Add(s.shop.Id);
                                }
                            }
                        });
                    }
                    //物料，除了光盘
                    var orderMaterialList = (from material in CurrentContext.DbContext.OrderMaterial
                                             join subject in CurrentContext.DbContext.Subject
                                             on material.SubjectId equals subject.Id
                                             join shop in CurrentContext.DbContext.Shop
                                             on material.ShopId equals shop.Id
                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                            on subject.GuidanceId equals guidance.ItemId
                                             where subjectIdList.Contains(material.SubjectId ?? 0)
                                             && (shop.Status == null || shop.Status == "" || shop.Status == ShopStatusEnum.正常.ToString())
                                             && (!material.MaterialName.ToLower().Contains("光盘"))
                                             && shopIdList.Contains(material.ShopId ?? 0)
                                             select new
                                             {
                                                 material,
                                                 subject,
                                                 shop,
                                                 guidance
                                             }).OrderBy(s => s.shop.Id).ToList();
                    if (orderMaterialList.Any())
                    {
                        orderMaterialList.ForEach(s =>
                        {
                            Order350Model model = new Order350Model();

                            model.ShopId = s.shop.Id;
                            model.SubjectId = 0;
                            model.GraphicNo = "";

                            model.Area = 0;
                            //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                            model.Category = "";
                            model.ChooseImg = "";
                            model.City = "";
                            //model.County = item.shop.AreaName;
                            model.CityTier = "";
                            model.Contacts = "";
                            model.Format = "";
                            model.Gender = "";
                            //model.GraphicLength = 0;
                            model.GraphicMaterial = s.material.MaterialName;
                            //model.GraphicWidth = 0;
                            model.POPAddress = "";
                            model.PositionDescription = "";
                            model.Province = "";
                            model.Quantity = s.material.MaterialCount != null ? double.Parse(s.material.MaterialCount.ToString()) : 0;
                            //model.Quantity = 1;
                            model.Sheet = "";
                            model.ShopName = s.shop.ShopName;
                            model.ShopNo = s.shop.ShopNo;

                            model.SubjectName = "";
                            model.Tels = "";
                            //model.OtherRemark = levelName;

                            model.POSScale = "";
                            model.MaterialSupport = "";
                            model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                            model.OrderType = "物料";
                            orderList.Add(model);
                        });
                    }


                    orderList.Add(new Order350Model() { ShopId = 9999999, ShopNo = "P9999999" });
                }
               



                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).ToList();
                    string templateFileName = "phwTemplate";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheet("Sheet1");

                    int startRow = 1;
                    //int shopId = 0;
                    //string shopno = string.Empty;
                    Order350Model lastItem = new Order350Model();
                    #region
                    foreach (var item in orderList)
                    {

                        //if (item.ShopId == 5223)
                        //{
                        //   int a=1;
                        //}
                        if (startRow == 1)
                        {
                            //shopno = item.ShopNo;
                            //shopId = item.ShopId;
                            lastItem = item;
                        }
                        else
                        {
                            
                            if (lastItem.ShopId != item.ShopId)
                            {
                                
                                lastItem = item;
                                startRow++;
                            }
                        }
                        if (item.ShopId>0 && item.ShopId < 9999999)
                        {
                            IRow dataRow = sheet.GetRow(startRow);
                            if (dataRow == null)
                                dataRow = sheet.CreateRow(startRow);
                            for (int i = 0; i < 14; i++)
                            {
                                ICell cell = dataRow.GetCell(i);
                                if (cell == null)
                                    cell = dataRow.CreateCell(i);

                            }
                            //基础材质（喷绘王材质）
                            string material = string.Empty;
                            if (!string.IsNullOrWhiteSpace(item.GraphicMaterial))
                                material = GetBasicMaterial(item.GraphicMaterial);
                            if (string.IsNullOrWhiteSpace(material))
                                material = item.GraphicMaterial;
                            dataRow.GetCell(0).SetCellValue(material);
                            //dataRow.GetCell(1).SetCellValue(item.ChooseImg);
                            dataRow.GetCell(1).SetCellValue(item.SubjectName);
                            //dataRow.GetCell(2).SetCellValue(Math.Round(item.GraphicWidth / 1000, 2));
                            //dataRow.GetCell(3).SetCellValue(Math.Round(item.GraphicLength / 1000, 2));

                            dataRow.GetCell(2).SetCellValue(item.GraphicWidth / 1000);
                            dataRow.GetCell(3).SetCellValue(item.GraphicLength / 1000);
                            dataRow.GetCell(4).SetCellValue(item.Quantity);
                            dataRow.GetCell(5).SetCellValue("");
                            dataRow.GetCell(6).SetCellValue("");
                            dataRow.GetCell(7).SetCellValue("");
                            dataRow.GetCell(8).SetCellValue("");
                            if (item.UnitPrice>0)
                              dataRow.GetCell(9).SetCellValue(item.UnitPrice);
                            //dataRow.GetCell(10).SetCellValue(item.SubjectName);
                            dataRow.GetCell(10).SetCellValue(item.GuidanceName);
                            dataRow.GetCell(11).SetCellValue("");
                            dataRow.GetCell(12).SetCellValue(item.ShopName);
                            dataRow.GetCell(13).SetCellValue(item.GraphicMaterial);

                            startRow++;
                        }

                    }
                    #endregion

                   
                    HttpCookie cookie = Request.Cookies["exportPHWBJ"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportPHWBJ");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();

                        sheet = null;

                        workBook = null;

                        OperateFile.DownLoadFile(ms, "喷绘王模板(北京)");
                        //OperateFile.DownLoadFile(path);

                    }
                }
                else
                {
                    HttpCookie cookie = Request.Cookies["exportPHWBJ"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportPHWBJ");
                    }
                    cookie.Value = "2";//没有数据可以导出
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);
                }



            }
        }

        void ExportOtherPHW()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                //List<int> promotionShopList = GetFreightShopList(subjectIdList);
                List<int> guidanceIdList = new List<int>();
                List<string> myRegionList = new BasePage().GetResponsibleRegion;
                if (myRegionList.Any())
                {
                    StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
                }
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on subject.GuidanceId equals guidance.ItemId
                            join category1 in CurrentContext.DbContext.ADSubjectCategory
                              on subject.SubjectCategoryId equals category1.Id into temp1
                            from category in temp1.DefaultIfEmpty()
                            //where subjectIdList.Contains(order.SubjectId ?? 0)
                            where (selectType == 1 ? subjectIdList.Contains(order.SubjectId ?? 0) : subjectIdList.Contains(order.RegionSupplementId ?? 0))
                            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType>1)
                            
                            && (order.IsValid == null || order.IsValid == true)
                            //&& (!order.Province.Contains("北京") && !order.Province.Contains("天津"))
                            && (order.IsDelete == null || order.IsDelete == false)
                            && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                            && (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                            && order.OrderType != (int)OrderTypeEnum.物料
                            select new
                            {
                                subject,
                                shop,
                                order,
                                guidance,
                                CategoryName = category != null ? category.CategoryName : "",
                            }).ToList();
                list = list.Where(s => ((s.order.Sheet!=null && s.order.Sheet.Contains("光盘")) || (s.order.PositionDescription != null && s.order.PositionDescription.Contains("光盘"))) ? ((s.order.Format != null && s.order.Format != "") && s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                if (!string.IsNullOrWhiteSpace(exportType))
                {
                    if (exportType == "nohc")
                    {
                        list = list.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                    }
                    else if (exportType == "hc")
                    {
                        list = list.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") != -1 || s.order.Format.ToLower().IndexOf("homecourt") != -1 || s.order.Format.ToLower().IndexOf("homecore") != -1 || s.order.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    if (materialName == "软膜")
                    {
                        list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                    }
                    else if (materialName == "非软膜")
                    {
                        list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
                    }
                }
                if (string.IsNullOrWhiteSpace(region))
                {
                    //regionList = new BasePage().GetResponsibleRegion;
                    //if (regionList.Any())
                    //{
                    //    StringHelper.ToUpperOrLowerList(ref regionList, LowerUpperEnum.ToLower);
                    //}
                }
                else
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                if (regionList.Any())
                {
                    //list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();
                        }
                        else
                            list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                    }
                    else
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                        //list = list.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    //list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    if (provinceList.Contains("空"))
                    {
                        List<string> provinceList0 = provinceList;
                        provinceList0.Remove("空");
                        if (provinceList0.Any())
                        {
                            list = list.Where(s => provinceList0.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                        }
                        else
                        {
                            list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                        }
                    }
                    else
                    {
                        list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();

                    }
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    //list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    if (cityList.Contains("空"))
                    {
                        List<string> cityList0 = cityList;
                        cityList0.Remove("空");
                        if (cityList0.Any())
                        {
                            list = list.Where(s => cityList0.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                        }
                        else
                        {
                            list = list.Where(s => s.order.City == null || s.order.City == "").ToList();

                        }
                    }
                    else
                    {
                        list = list.Where(s => cityList.Contains(s.order.City)).ToList();

                    }
                }
                if (!string.IsNullOrWhiteSpace(customerServiceId))
                {
                    List<int> customerServiceList = StringHelper.ToIntList(customerServiceId, ',');
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                        }
                        else
                            list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    List<string> installList = StringHelper.ToStringList(isInstall, ',');
                    if (installList.Contains("无"))
                    {
                        installList.Remove("无");
                        if (installList.Any())
                        {
                            list = list.Where(s => installList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                        }
                        else
                            list = list.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                    }
                    else
                        list = list.Where(s => installList.Contains(s.order.IsInstall)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(materialCategoryId))
                {
                    bool hasEmpty = false;
                    List<string> materialList = new List<string>();
                    List<int> categoryList = StringHelper.ToIntList(materialCategoryId, ',');
                    if (categoryList.Contains(0))
                    {
                        hasEmpty = true;
                        categoryList.Remove(0);
                    }
                    if (categoryList.Any())
                    {
                        materialList = new OrderMaterialMppingBLL().GetList(s => categoryList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                    }
                    if (hasEmpty)
                    {
                        if (materialList.Any())
                        {
                            list = list.Where(s => (s.order.GraphicMaterial!=null && materialList.Contains(s.order.GraphicMaterial.ToLower())) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                        }
                        else
                            list = list.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                    }
                    else
                        list = list.Where(s => s.order.GraphicMaterial != null && materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(exportShopNo))
                {
                    List<string> exportShopNoList = StringHelper.ToStringList(exportShopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower); ;
                    if (exportShopNoList.Any())
                    {
                        if (isNotInclude == "1")
                        {
                            list = list.Where(s => !exportShopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
                        }
                        else
                        {
                            list = list.Where(s => exportShopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
                        }
                    }
                }
                List<Order350Model> orderList = new List<Order350Model>();
                if (list.Any())
                {


                    //InstallPriceShopInfoBLL installPriceBll = new InstallPriceShopInfoBLL();
                    //bool SubmitInstallPrice = true;
                    ////检查是否已经提交安装费
                    //var installShopList = list.Where(s => s.guidance.ActivityTypeId == (int)GuidanceTypeEnum.Install && s.subject.SubjectType != (int)SubjectTypeEnum.二次安装).ToList();
                    //if (installShopList.Any())
                    //{
                    //    List<int> guidaceIdList = installShopList.Select(s => s.guidance.ItemId).Distinct().ToList();

                    //    List<int> FinishInstallPriceShopIdList = installPriceBll.GetList(s => guidaceIdList.Contains(s.GuidanceId ?? 0)).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    //    //三叶草安装费
                    //    List<string> cityTierList = new List<string> { "T1", "T2", "T3" };
                    //    List<int> installShopIdList1 = installShopList.Where(s => s.subject.CornerType == "三叶草" && cityTierList.Contains(s.shop.CityTier) && (s.shop.BCSInstallPrice ?? 0) > 0).Select(s => s.shop.Id).Distinct().ToList();
                    //    if (installShopIdList1.Any())
                    //    {

                    //        //List<int> c = FinishInstallPriceShopIdList.Except(installShopIdList).ToList();
                    //        SubmitInstallPrice = StringHelper.IsContainsAll(FinishInstallPriceShopIdList, installShopIdList1);
                    //    }
                    //    //非三叶草
                    //    List<int> installShopIdList2 = installShopList.Where(s => s.shop.IsInstall == "Y" && (string.IsNullOrWhiteSpace(s.subject.CornerType) || s.subject.CornerType != "三叶草")).Select(s => s.shop.Id).Distinct().ToList();
                    //    if (installShopIdList2.Any())
                    //    {
                    //        if (!StringHelper.IsContainsAll(FinishInstallPriceShopIdList, installShopIdList2))
                    //        {
                    //            SubmitInstallPrice = false;
                    //        }
                    //    }
                    //}
                    //if (!SubmitInstallPrice)
                    //{
                    //    HttpCookie cookie = Request.Cookies["exportPHWOther"];
                    //    if (cookie == null)
                    //    {
                    //        cookie = new HttpCookie("exportPHWOther");
                    //    }
                    //    cookie.Value = "3";//没有提交完安装费
                    //    cookie.Expires = DateTime.Now.AddMinutes(30);
                    //    Response.Cookies.Add(cookie);
                    //    return;
                    //}

                    guidanceIdList = list.Select(s => s.guidance.ItemId).Distinct().ToList();
                    List<int> shopIdList = list.Select(s => s.shop.Id).Distinct().ToList();
                    string changePOPCountSheetStr = string.Empty;
                    List<string> ChangePOPCountSheetList = new List<string>();
                    try
                    {
                        changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrWhiteSpace(changePOPCountSheetStr))
                    {
                        ChangePOPCountSheetList = StringHelper.ToStringList(changePOPCountSheetStr, '|');
                    }
                    var HCSmallSizeList = new HCSmallGraphicSizeBLL().GetList(s => s.Id > 0);
                    foreach (var item in list)
                    {
                        int level = item.order.LevelNum ?? 0;
                        string levelName = string.Empty;
                        
                        if (level > 0)
                            levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());
                        
                        double GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                        double GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0;
                        bool canGo = true;
                        if (!string.IsNullOrWhiteSpace(item.order.GraphicNo) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(item.order.Sheet.ToUpper()))
                        {
                            //去掉重复订单（同一个编号下多次）
                            var checkList = orderList.Where(s => s.SubjectId == item.subject.Id && s.ShopId == item.shop.Id && s.Sheet == item.order.Sheet && s.PositionDescription == item.order.PositionDescription && s.GraphicNo == item.order.GraphicNo && s.GraphicLength == GraphicLength && s.GraphicWidth == GraphicWidth && s.Gender == item.order.Gender).ToList();
                            if (checkList.Any())
                                canGo = false;
                        }
                        if (canGo)
                        {
                            Order350Model model = new Order350Model();
                            string orderTypeName = CommonMethod.GetEnumDescription<OrderTypeEnum>((item.order.OrderType ?? 1).ToString());
                            if (orderTypeName == "费用订单")
                            {
                                model.UnitPrice = double.Parse((item.order.OrderPrice ?? 0).ToString());
                                model.ShopId = item.shop.Id;
                                model.SubjectId = 0;
                                model.GraphicNo = "";

                                model.Area = 0;
                                //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                model.Category = "";
                                model.ChooseImg = "";
                                model.City = "";
                                //model.County = item.shop.AreaName;
                                model.CityTier = "";
                                model.Contacts = "";
                                model.Format = "";
                                model.Gender = "";
                                //model.GraphicLength = 0;
                                model.GraphicMaterial = CommonMethod.GeEnumName<OrderTypeEnum>((item.order.OrderType ?? 1).ToString());
                                //model.GraphicWidth = 0;
                                model.POPAddress = "";
                                model.PositionDescription = "";
                                model.Province = "";
                                // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                model.Quantity = 1;
                                model.Sheet = "";
                                model.ShopName = item.order.ShopName;
                                model.ShopNo = item.order.ShopNo;

                                model.SubjectName = "";
                                model.Tels = "";
                                //model.OtherRemark = levelName;

                                model.POSScale = "";
                                model.MaterialSupport = "";
                                model.GuidanceName = item.guidance != null ? item.guidance.ItemName : "";
                                model.OrderType = CommonMethod.GeEnumName<OrderTypeEnum>((item.order.OrderType??1).ToString());
                                orderList.Add(model);
                            }
                            else
                            {
                                model.ShopId = item.shop.Id;
                                model.GraphicNo = item.order.GraphicNo;
                                model.SubjectId = item.subject.Id;
                                decimal area = 0;
                                if (item.order.GraphicWidth != null && item.order.GraphicLength != null)
                                {
                                    area = ((item.order.GraphicWidth ?? 0) * (item.order.GraphicLength ?? 0)) / 1000000;
                                }
                                model.Area = double.Parse(area.ToString());
                                //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                model.Category = item.order.Category;
                                model.ChooseImg = item.order.ChooseImg;
                                model.City = item.order.City;
                                model.County = item.shop.AreaName;
                                model.CityTier = item.order.CityTier;
                                model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                                model.Format = item.order.Format;
                                model.Gender = !string.IsNullOrWhiteSpace(item.order.OrderGender) ? item.order.OrderGender : item.order.Gender;
                                model.GraphicLength = GraphicLength;
                                model.GraphicMaterial = item.order.GraphicMaterial;
                                model.GraphicWidth = GraphicWidth;
                                model.POPAddress = item.shop.POPAddress;
                                model.PositionDescription = item.order.PositionDescription;
                                model.Province = item.order.Province;
                                //model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                if (item.order.Sheet != null && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(item.order.Sheet.ToUpper()))
                                {
                                    model.Quantity = (item.order.Quantity ?? 0) > 0 ? 1 : 0;
                                }
                                else
                                {
                                    model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                }
                                model.Sheet = item.order.Sheet;
                                model.ShopName = item.order.ShopName;
                                model.ShopNo = item.order.ShopNo;
                                string subjectName = item.subject.SubjectName;
                                if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                                    subjectName += "(" + item.subject.Remark + ")";
                                model.SubjectName = subjectName;
                                model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                                //model.OtherRemark = levelName;
                                model.IsPOPMaterial = item.order.IsPOPMaterial;
                                model.GuidanceName = item.guidance.ItemName;
                                model.OrderType = CommonMethod.GeEnumName<OrderTypeEnum>((item.order.OrderType ?? 1).ToString());
                                orderList.Add(model);
                            }
                        }
                    }


                    //var installList = (from install in CurrentContext.DbContext.InstallPriceShopInfo
                    //                   join shop in CurrentContext.DbContext.Shop
                    //                   on install.ShopId equals shop.Id
                    //                   join subject in CurrentContext.DbContext.Subject
                    //                   on install.SubjectId equals subject.Id
                    //                   join guidance1 in CurrentContext.DbContext.SubjectGuidance
                    //                   on subject.GuidanceId equals guidance1.ItemId into temp
                    //                   from guidance in temp.DefaultIfEmpty()
                    //                   where subjectIdList.Contains(install.SubjectId ?? 0)
                    //                   && shopIdList.Contains(install.ShopId ?? 0)
                    //                   && (guidance != null && guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                    //                   select new
                    //                   {
                    //                       install,
                    //                       shop,
                    //                       guidance
                    //                   }).ToList();

                    List<int> genericShopIdList = list.Where(s => s.CategoryName != null && s.CategoryName.Contains("常规-非活动")).Select(s => s.order.ShopId ?? 0).ToList();
                    List<int> normalShopIdList = list.Where(s => s.CategoryName == null || (s.CategoryName != null && !s.CategoryName.Contains("常规-非活动"))).Select(s => s.order.ShopId ?? 0).ToList();


                    var installList = (from install in CurrentContext.DbContext.InstallPriceTemp
                                       join shop in CurrentContext.DbContext.Shop
                                       on install.ShopId equals shop.Id
                                       join guidance in CurrentContext.DbContext.SubjectGuidance
                                       on install.GuidanceId equals guidance.ItemId
                                       where guidanceIdList.Contains(guidance.ItemId)
                                       && shopIdList.Contains(install.ShopId ?? 0)
                                       && (guidance != null && guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                                       select new
                                       {
                                           install,
                                           shop,
                                           guidance
                                       }).ToList();
                    if (installList.Any())
                    {
                        var genericInstallList = installList.Where(s => genericShopIdList.Contains(s.install.ShopId ?? 0) && s.install.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费).ToList();
                        var notGenericInstallList = installList.Where(s => normalShopIdList.Contains(s.install.ShopId ?? 0) && s.install.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费).ToList();

                        genericInstallList.ForEach(s =>
                        {
                            decimal installPrice = (s.install.BasicPrice ?? 0) + (s.install.OOHPrice ?? 0) + (s.install.WindowPrice ?? 0);
                            
                            if (installPrice > 0)
                            {
                                Order350Model model = new Order350Model();
                                model.UnitPrice = double.Parse(installPrice.ToString());
                                model.ShopId = s.shop.Id;
                                model.SubjectId = 0;
                                model.GraphicNo = "";

                                //model.Area = 0;
                                //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                model.Category = "";
                                model.ChooseImg = "";
                                model.City = "";
                                //model.County = item.shop.AreaName;
                                model.CityTier = "";
                                model.Contacts = "";
                                model.Format = "";
                                model.Gender = "";
                                //model.GraphicLength = 0;
                                model.GraphicMaterial = "安装费";
                                //model.GraphicWidth = 0;
                                model.POPAddress = "";
                                model.PositionDescription = "";
                                model.Province = "";
                                // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                model.Quantity = 1;
                                model.Sheet = "";
                                model.ShopName = s.shop.ShopName;
                                model.ShopNo = s.shop.ShopNo;

                                model.SubjectName = "";
                                model.Tels = "";
                                //model.OtherRemark = levelName;

                                model.POSScale = "";
                                model.MaterialSupport = "";
                                model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                                model.OrderType = "安装费";
                                orderList.Add(model);
                            }
                        });
                        notGenericInstallList.ForEach(s =>
                        {
                            decimal installPrice = (s.install.BasicPrice ?? 0) + (s.install.OOHPrice ?? 0) + (s.install.WindowPrice ?? 0);

                            if (installPrice > 0)
                            {
                                Order350Model model = new Order350Model();
                                model.UnitPrice = double.Parse(installPrice.ToString());
                                model.ShopId = s.shop.Id;
                                model.SubjectId = 0;
                                model.GraphicNo = "";

                                //model.Area = 0;
                                //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                model.Category = "";
                                model.ChooseImg = "";
                                model.City = "";
                                //model.County = item.shop.AreaName;
                                model.CityTier = "";
                                model.Contacts = "";
                                model.Format = "";
                                model.Gender = "";
                                //model.GraphicLength = 0;
                                model.GraphicMaterial = "安装费";
                                //model.GraphicWidth = 0;
                                model.POPAddress = "";
                                model.PositionDescription = "";
                                model.Province = "";
                                // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                model.Quantity = 1;
                                model.Sheet = "";
                                model.ShopName = s.shop.ShopName;
                                model.ShopNo = s.shop.ShopNo;

                                model.SubjectName = "";
                                model.Tels = "";
                                //model.OtherRemark = levelName;

                                model.POSScale = "";
                                model.MaterialSupport = "";
                                model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                                model.OrderType = "安装费";
                                orderList.Add(model);
                            }
                        });
                    }

                    //发货费
                    var ExperssShopList = list.Where(s => s.guidance.ActivityTypeId == (int)GuidanceTypeEnum.Promotion || s.guidance.ActivityTypeId == (int)GuidanceTypeEnum.Delivery).ToList();
                    if (ExperssShopList.Any())
                    {
                        List<int> expressGuidanceIdList = ExperssShopList.Select(s => s.guidance.ItemId).Distinct().ToList();
                        var priceOrderList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId != null && expressGuidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();

                        List<int> ExperssShopIdList = new List<int>();
                        ExperssShopList.ForEach(s =>
                        {
                            if (!ExperssShopIdList.Contains(s.shop.Id))
                            {
                                decimal expressPrice = 0;
                                var priceModel = priceOrderList.Where(p => p.ShopId == s.shop.Id).FirstOrDefault();
                                if (priceModel != null)
                                {
                                    expressPrice = priceModel.ExpressPrice ?? 0;
                                }
                                if (expressPrice > 0)
                                {
                                    Order350Model model = new Order350Model();
                                    model.ShopId = s.shop.Id;
                                    model.SubjectId = 0;
                                    model.GraphicNo = "";

                                    model.Area = 0;
                                    //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                                    model.Category = "";
                                    model.ChooseImg = "";
                                    model.City = "";
                                    //model.County = item.shop.AreaName;
                                    model.CityTier = "";
                                    model.Contacts = "";
                                    model.Format = "";
                                    model.Gender = "";
                                    //model.GraphicLength = 0;
                                    model.GraphicMaterial = "发货费";
                                    //model.GraphicWidth = 0;
                                    model.POPAddress = "";
                                    model.PositionDescription = "";
                                    model.Province = "";
                                    // model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                    model.Quantity = 1;
                                    model.Sheet = "";
                                    model.ShopName = s.shop.ShopName;
                                    model.ShopNo = s.shop.ShopNo;

                                    model.SubjectName = "";
                                    model.Tels = "";
                                    //model.OtherRemark = levelName;

                                    model.POSScale = "";
                                    model.MaterialSupport = "";
                                    model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                                    model.OrderType = "发货费";
                                    model.UnitPrice = double.Parse(expressPrice.ToString());
                                    orderList.Add(model);
                                    ExperssShopIdList.Add(s.shop.Id);
                                }
                            }
                        });
                    }
                    //物料，除了光盘
                    var orderMaterialList = (from material in CurrentContext.DbContext.OrderMaterial
                                             join subject in CurrentContext.DbContext.Subject
                                             on material.SubjectId equals subject.Id
                                             join shop in CurrentContext.DbContext.Shop
                                             on material.ShopId equals shop.Id
                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                            on subject.GuidanceId equals guidance.ItemId
                                             where subjectIdList.Contains(material.SubjectId ?? 0)
                                             && (shop.Status == null || shop.Status == "" || shop.Status == ShopStatusEnum.正常.ToString())
                                             && (!material.MaterialName.ToLower().Contains("光盘"))
                                             && shopIdList.Contains(material.ShopId ?? 0)
                                             select new
                                             {
                                                 material,
                                                 subject,
                                                 shop,
                                                 guidance
                                             }).OrderBy(s => s.shop.Id).ToList();
                    if (orderMaterialList.Any())
                    {
                        orderMaterialList.ForEach(s =>
                        {
                            Order350Model model = new Order350Model();

                            model.ShopId = s.shop.Id;
                            model.SubjectId = 0;
                            model.GraphicNo = "";

                            model.Area = 0;
                            //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                            model.Category = "";
                            model.ChooseImg = "";
                            model.City = "";
                            //model.County = item.shop.AreaName;
                            model.CityTier = "";
                            model.Contacts = "";
                            model.Format = "";
                            model.Gender = "";
                            //model.GraphicLength = 0;
                            model.GraphicMaterial = s.material.MaterialName;
                            //model.GraphicWidth = 0;
                            model.POPAddress = "";
                            model.PositionDescription = "";
                            model.Province = "";
                            model.Quantity = s.material.MaterialCount != null ? double.Parse(s.material.MaterialCount.ToString()) : 0;
                            //model.Quantity = 1;
                            model.Sheet = "";
                            model.ShopName = s.shop.ShopName;
                            model.ShopNo = s.shop.ShopNo;

                            model.SubjectName = "";
                            model.Tels = "";
                            //model.OtherRemark = levelName;

                            model.POSScale = "";
                            model.MaterialSupport = "";
                            model.GuidanceName = s.guidance != null ? s.guidance.ItemName : "";
                            model.OrderType = "物料";
                            orderList.Add(model);
                        });
                    }

                    orderList.Add(new Order350Model() { ShopId =9999999, ShopNo = "P9999999" });
                }

                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopId).ThenBy(s=>s.OrderType).ToList();
                    string templateFileName = "phwTemplate";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheet("Sheet1");

                    int startRow = 1;
                    Order350Model lastItem = new Order350Model();
                    //string shopno = string.Empty;
                    foreach (var item in orderList)
                    {
                        if (startRow == 1)
                        {
                            lastItem = item;
                        }
                        else
                        {
                            if (lastItem.ShopId != item.ShopId)
                            {
                              
                                lastItem = item;
                            }
                        }


                        if (item.ShopId > 0 && item.ShopId < 9999999)
                        {
                            IRow dataRow = sheet.GetRow(startRow);
                            if (dataRow == null)
                                dataRow = sheet.CreateRow(startRow);
                            for (int i = 0; i < 14; i++)
                            {
                                ICell cell = dataRow.GetCell(i);
                                if (cell == null)
                                    cell = dataRow.CreateCell(i);

                            }
                            //基础材质（喷绘王材质）
                            string material = string.Empty;
                            if (!string.IsNullOrWhiteSpace(item.GraphicMaterial))
                                material = GetBasicMaterial(item.GraphicMaterial);
                            if (string.IsNullOrWhiteSpace(material))
                                material = item.GraphicMaterial;
                            dataRow.GetCell(0).SetCellValue(material);
                            dataRow.GetCell(1).SetCellValue(item.SubjectName);

                            dataRow.GetCell(2).SetCellValue(item.GraphicWidth / 1000);
                            dataRow.GetCell(3).SetCellValue(item.GraphicLength / 1000);
                            dataRow.GetCell(4).SetCellValue(item.Quantity);
                            dataRow.GetCell(5).SetCellValue("");
                            dataRow.GetCell(6).SetCellValue("");
                            dataRow.GetCell(7).SetCellValue("");
                            dataRow.GetCell(8).SetCellValue("");
                            if (item.UnitPrice > 0)
                                dataRow.GetCell(9).SetCellValue(item.UnitPrice);
                            //dataRow.GetCell(9).SetCellValue("");
                            dataRow.GetCell(10).SetCellValue(item.GuidanceName);
                            dataRow.GetCell(11).SetCellValue("");
                            dataRow.GetCell(12).SetCellValue(item.ShopName);
                            dataRow.GetCell(13).SetCellValue(item.GraphicMaterial);

                            startRow++;
                        }

                    }

                    HttpCookie cookie = Request.Cookies["exportPHWOther"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportPHWOther");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();

                        sheet = null;

                        workBook = null;

                        OperateFile.DownLoadFile(ms, "喷绘王模板(外协)");
                        //OperateFile.DownLoadFile(path);

                    }
                }
                else
                {
                    HttpCookie cookie = Request.Cookies["exportPHWOther"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportPHWOther");
                    }
                    cookie.Value = "2";//没有数据可以导出
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);
                }




            }
        }

        /// <summary>
        /// 导出外协订单350
        /// </summary>
        void ExportOutsourcingOrder350()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                //List<int> promotionShopList = GetFreightShopList(subjectIdList);
                int companyId = new BasePage().CurrentUser.CompanyId;
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join pop1 in CurrentContext.DbContext.POP
                            on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                            from pop in popTemp.DefaultIfEmpty()
                            //pop订单中，pop尺寸为空的不导出
                            where subjectIdList.Contains(order.SubjectId ?? 0)
                            && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                                //&& !shop.Format.Contains("Homecourt")
                            && (pop != null ? (pop.IsValid == null || pop.IsValid == true) : 1 == 1)
                            && order.OutsourceId == companyId
                            && (order.IsDelete == null || order.IsDelete == false)
                            select new
                            {
                                subject,
                                shop,
                                order,
                                pop
                            }).ToList();


                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }

                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }

                List<Order350Model> orderList = new List<Order350Model>();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        string status = string.Empty;
                        if (item.shop.Status != null)
                            status = item.shop.Status;
                        bool isClose = false;
                        if (!string.IsNullOrWhiteSpace(status) && (status.Contains("关") || status.Contains("闭")))
                        {
                            isClose = true;
                        }
                        if (!isClose)
                        {
                            int level = item.order.LevelNum ?? 0;
                            string levelName = string.Empty;
                            //if(item.order.Sheet.Contains("桌"))
                            //    levelName = CommonMethod.GeEnumName<TableLevelEnum>(level.ToString());
                            if (level > 0)
                                levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());
                            Order350Model model = new Order350Model();
                            model.ShopId = item.shop.Id;
                            decimal area = 0;
                            if (item.order.GraphicWidth != null && item.order.GraphicLength != null)
                            {
                                area = ((item.order.GraphicWidth ?? 0) * (item.order.GraphicLength ?? 0)) / 1000000;
                            }
                            model.Area = double.Parse(area.ToString());
                            model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                            model.Category = (item.order.Category != null && item.order.Category != "") ? item.order.Category : item.pop != null ? item.pop.Category : "";
                            model.ChooseImg = item.order.ChooseImg;
                            model.City = item.shop.CityName;
                            model.County = item.shop.AreaName;
                            model.CityTier = item.shop.CityTier;
                            model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                            model.Format = item.shop.Format;
                            model.Gender = item.order.Gender;
                            model.GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                            model.GraphicMaterial = item.order.GraphicMaterial;
                            model.GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0;
                            model.POPAddress = item.shop.POPAddress;
                            model.PositionDescription = item.order.PositionDescription;
                            model.Province = item.shop.ProvinceName;
                            model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                            model.Sheet = item.order.Sheet;
                            model.ShopName = item.shop.ShopName;
                            model.ShopNo = item.shop.ShopNo;

                            string subjectName = item.subject.SubjectName;
                            if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                                subjectName += "(" + item.subject.Remark + ")";
                            model.SubjectName = subjectName;
                            model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                            model.OtherRemark = levelName;
                            model.IsPOPMaterial = item.order.IsPOPMaterial;
                            model.POSScale = item.order.POSScale;
                            model.MaterialSupport = item.order.MaterialSupport;
                            orderList.Add(model);
                        }
                    }
                  
                }

                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopNo).ToList();
                    string templateFileName = "350Template";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheetAt(0);

                    int startRow = 1;
                    Order350Model lastItem = new Order350Model();
                    //string shopno = string.Empty;
                    foreach (var item in orderList)
                    {
                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 30; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(0).SetCellValue(item.ShopNo);
                        dataRow.GetCell(1).SetCellValue(item.ShopName);
                        dataRow.GetCell(2).SetCellValue(item.Province);
                        dataRow.GetCell(3).SetCellValue(item.City);
                        dataRow.GetCell(4).SetCellValue(item.CityTier);
                        dataRow.GetCell(5).SetCellValue(item.Format);
                        dataRow.GetCell(6).SetCellValue(item.POPAddress);
                        dataRow.GetCell(7).SetCellValue(item.Contacts);
                        dataRow.GetCell(8).SetCellValue(item.Tels);
                        dataRow.GetCell(9).SetCellValue(item.ChooseImg);
                        dataRow.GetCell(10).SetCellValue(item.SubjectName);
                        dataRow.GetCell(11).SetCellValue(item.Gender);
                        dataRow.GetCell(12).SetCellValue(item.Category);
                        dataRow.GetCell(13).SetCellValue(item.Sheet);
                        dataRow.GetCell(14).SetCellValue(item.Quantity);
                        dataRow.GetCell(15).SetCellValue(item.GraphicMaterial);
                        dataRow.GetCell(16).SetCellValue(item.GraphicWidth);
                        dataRow.GetCell(17).SetCellValue(item.GraphicLength);
                        dataRow.GetCell(18).SetCellValue(item.Area);
                        //其他备注
                        dataRow.GetCell(19).SetCellValue(item.OtherRemark);
                        dataRow.GetCell(20).SetCellValue(item.PositionDescription);
                        dataRow.GetCell(22).SetCellValue(item.County);
                        startRow++;

                    }

                    HttpCookie cookie = Request.Cookies["exportOutsourcing"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportOutsourcing");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();

                        sheet = null;

                        workBook = null;

                        OperateFile.DownLoadFile(ms, "外协350订单");

                    }
                }





            }
        }

        string ExportOutsourcingOrder()
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                StringBuilder whereSql = new StringBuilder();
                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                StringBuilder subjectIds = new StringBuilder();
                subjectIdList.ForEach(s =>
                    {
                        subjectIds.Append(s + ",");
                    });
                whereSql.AppendFormat(" and Subject.Id in({0})", subjectIds.ToString().TrimEnd(','));
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',');
                    StringBuilder regions = new StringBuilder();
                    regionList.ForEach(s =>
                    {
                        regions.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and Region in({0})", regions.ToString().TrimEnd(','));
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    StringBuilder provinces = new StringBuilder();
                    provinceList.ForEach(s =>
                    {
                        provinces.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and Province in({0})", provinces.ToString().TrimEnd(','));
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    StringBuilder citys = new StringBuilder();
                    cityList.ForEach(s =>
                    {
                        citys.Append("'" + s + "',");
                    });
                    whereSql.AppendFormat(" and City in({0})", citys.ToString().TrimEnd(','));
                }
                whereSql.AppendFormat(" and orderTemp.CompanyId={0}", CurrentUser.CompanyId);
                FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                System.Data.DataSet ds = orderBll.ExportOutsourceOrderList(whereSql.ToString());
                int colNum = ds.Tables[0].Columns.Count;
                ds.Tables[0].Columns.RemoveAt(colNum - 1);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    string fileName = "外协竖版订单";

                    OperateFile.ExportExcel(ds.Tables[0], fileName);

                }
                else
                    result = "没有数据可以导出";
            }
            else
                result = "没有数据可以导出";
            return result;
        }

       


        /// <summary>
        /// 获取辅料名称
        /// </summary>
        /// <param name="smId"></param>
        /// <returns></returns>
        /// 
        SmallMaterialBLL smmBll = new SmallMaterialBLL();
        Dictionary<int, string> smmDic = new Dictionary<int, string>();
        string GetSmallMaterial(int smId)
        {
            string restule = string.Empty;
            if (smmDic.Keys.Contains(smId))
            {
                restule = smmDic[smId];
            }
            else
            {
                var model = smmBll.GetModel(smId);
                if (model != null)
                {
                    restule = model.SmallMaterialName;
                    if (!smmDic.Keys.Contains(smId))
                    {
                        smmDic.Add(smId, restule);
                    }
                }
            }
            return restule;
        }

        /// <summary>
        /// 获取促销店铺Id
        /// </summary>
        /// <param name="subjectIdList"></param>
        /// <returns></returns>
        List<int> GetFreightShopList(List<int> subjectIdList)
        {
            List<int> shopList = new List<int>();
            if (subjectIdList.Any())
            {
                shopList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on subject.GuidanceId equals guidance.ItemId
                            where subjectIdList.Contains(order.SubjectId ?? 0) && guidance.ActivityTypeId == 3
                            select order.ShopId ?? 0).Distinct().ToList();

            }
            return shopList;
        }

        /// <summary>
        /// 导出报价350（不含空）
        /// </summary>
        void ExportQuote350()
        {
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');
                List<string> myRegionList = new BasePage().GetResponsibleRegion;
                if (myRegionList.Any())
                {
                    StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
                }
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            //join pop1 in CurrentContext.DbContext.POP
                            //on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                            //from pop in popTemp.DefaultIfEmpty()
                            join supplimentsubject1 in CurrentContext.DbContext.Subject
                            on order.RegionSupplementId equals supplimentsubject1.Id into supplimentsubjectTemp
                            from supplimentsubject in supplimentsubjectTemp.DefaultIfEmpty()
                            //pop订单中，pop尺寸为空的不导出
                            where subjectIdList.Contains(order.SubjectId ?? 0)
                           
                            && (order.IsDelete == null || order.IsDelete == false)
                            && (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                            select new
                            {
                                subject,
                                shop,
                                order,
                                //pop,
                                supplimentsubject,
                            }).ToList();


                list = list.Where(s => (s.order.OrderType == 1 && s.order.GraphicLength != null && s.order.GraphicLength > 0 && s.order.GraphicWidth != null && s.order.GraphicWidth > 0) || (s.order.OrderType >1)).ToList();


                if (!string.IsNullOrWhiteSpace(region))
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                    //list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                        }
                        else
                            list = list.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                    }
                    else
                        list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    //list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    if (provinceList.Contains("空"))
                    {
                        List<string> provinceList0 = provinceList;

                        provinceList0.Remove("空");
                        if (provinceList0.Any())
                        {
                            list = list.Where(s => provinceList0.Contains(s.shop.ProvinceName) || s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                           
                        }
                        else
                        {
                            list = list.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();
                            
                        }
                    }
                    else
                    {
                        list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                        
                    }

                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    //list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    if (cityList.Contains("空"))
                    {
                        List<string> cityList0 = cityList;
                        cityList0.Remove("空");
                        if (cityList0.Any())
                        {
                            list = list.Where(s => cityList0.Contains(s.shop.CityName) || s.shop.CityName == null || s.shop.CityName == "").ToList();
                            
                        }
                        else
                        {
                            list = list.Where(s => s.shop.CityName == null || s.shop.CityName == "").ToList();
                            
                        }
                    }
                    else
                    {
                        list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                       
                    }

                }

                if (!string.IsNullOrWhiteSpace(customerServiceId))
                {
                    List<int> customerServiceList = StringHelper.ToIntList(customerServiceId, ',');
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                        }
                        else
                            list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    List<string> installList = StringHelper.ToStringList(isInstall, ',');
                    if (installList.Contains("无"))
                    {
                        installList.Remove("无");
                        if (installList.Any())
                        {
                            list = list.Where(s => installList.Contains(s.shop.IsInstall) || (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                        }
                        else
                            list = list.Where(s => (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                    }
                    else
                        list = list.Where(s => installList.Contains(s.shop.IsInstall)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(materialCategoryId))
                {
                    bool hasEmpty = false;
                    List<string> materialList = new List<string>();
                    List<int> categoryList = StringHelper.ToIntList(materialCategoryId, ',');
                    if (categoryList.Contains(0))
                    {
                        hasEmpty = true;
                        categoryList.Remove(0);
                    }
                    if (categoryList.Any())
                    {
                        materialList = new OrderMaterialMppingBLL().GetList(s => categoryList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                    }
                    if (hasEmpty)
                    {
                        if (materialList.Any())
                        {
                            list = list.Where(s => (s.order.GraphicMaterial!=null && materialList.Contains(s.order.GraphicMaterial.ToLower())) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                        }
                        else
                            list = list.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                    }
                    else
                        list = list.Where(s => s.order.GraphicMaterial != null && materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                }

                List<Order350Model> orderList = new List<Order350Model>();
                int guidanceId = list.Select(s => s.subject.GuidanceId ?? 0).FirstOrDefault();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        
                        int level = item.order.LevelNum ?? 0;
                        string levelName = string.Empty;

                        if (level > 0)
                            levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());
                        Order350Model model = new Order350Model();
                        decimal area = 0;
                        if (item.order.GraphicWidth != null && item.order.GraphicLength != null)
                        {
                            area = (((item.order.GraphicWidth ?? 0) * (item.order.GraphicLength ?? 0)) / 1000000) * (item.order.Quantity ?? 1);
                        }
                        model.AddDate = item.subject.AddDate;
                        model.Area = double.Parse(area.ToString());
                        //model.Area = item.order.Area != null ? double.Parse(StringHelper.IsDecimal(item.order.Area.ToString()).ToString()) : 0;
                        model.Category = item.order.Category;
                        model.ChooseImg = item.order.ChooseImg;
                        model.City = item.shop.CityName;
                        model.County = item.shop.AreaName;
                        model.CityTier = item.order.CityTier;
                        model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                        model.Format = item.order.Channel;
                        model.Format = item.order.Format;
                        model.NewFormat = item.shop.Format;
                        model.Gender =!string.IsNullOrWhiteSpace(item.order.OrderGender)?item.order.OrderGender: item.order.Gender;
                        //model.Gender = item.order.Gender;
                        model.GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                        //string smallMaterial = GetSmallMaterial(item.order.SmallMaterialId ?? 0);
                        //string material = item.order.GraphicMaterial;
                        //if (!string.IsNullOrWhiteSpace(smallMaterial))
                        //    material += ("+" + smallMaterial);
                        string quoteMaterial = GetQuoteMaterial(item.order.GraphicMaterial);
                        model.QuoteGraphicMaterial = !string.IsNullOrWhiteSpace(quoteMaterial) ? quoteMaterial : item.order.GraphicMaterial;
                        model.GraphicMaterial =item.order.GraphicMaterial;
                        model.GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0; ;
                        model.POPAddress = item.shop.POPAddress;
                        model.PositionDescription = item.order.PositionDescription;
                        model.Province = item.shop.ProvinceName;
                        model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                        model.Sheet = item.order.Sheet;
                        model.ShopName = item.order.ShopName;
                        model.ShopNo = item.order.ShopNo;
                        string subjectName = item.subject.SubjectName;
                        if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                            subjectName += "(" + item.subject.Remark + ")";
                        model.SubjectName = subjectName;
                        model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                       
                        model.OtherRemark = item.order.Remark;
                        model.POSScale = item.order.POSScale;
                        model.MaterialSupport = item.order.MaterialSupport;
                        model.MachineFrame = item.order.MachineFrame;
                        model.IsInstall = item.order.IsInstall;
                        model.UnitPrice = item.order.UnitPrice != null ? double.Parse(item.order.UnitPrice.ToString()) : 0;
                        if (item.supplimentsubject != null)
                            model.SupplimentSubjectName = ("分区订单(" + item.supplimentsubject.SubjectName + ")");
                        model.OrderType = CommonMethod.GeEnumName<OrderTypeEnum>((item.order.OrderType ?? 1).ToString());
                        if ((item.order.OrderPrice ?? 0) > 0)
                            model.ReceivePrice = double.Parse((item.order.OrderPrice ?? 0).ToString());
                        else
                            model.ReceivePrice = double.Parse((item.order.TotalPrice ?? 0).ToString());
                        model.Region = item.order.Region;
                        orderList.Add(model);
                    }
                }
  

                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopNo).ToList();
                    string templateFileName = "350Template";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);

                    ISheet sheet = workBook.GetSheetAt(0);

                    int startRow = 1;
                    string shopno = string.Empty;
                    foreach (var item in orderList)
                    {

                        IRow dataRow = sheet.GetRow(startRow);
                        if (dataRow == null)
                            dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < 32; i++)
                        {
                            ICell cell = dataRow.GetCell(i);
                            if (cell == null)
                                cell = dataRow.CreateCell(i);

                        }
                        dataRow.GetCell(0).SetCellValue(item.OrderType);
                        if (item.AddDate!=null)
                            dataRow.GetCell(1).SetCellValue(DateTime.Parse(item.AddDate.ToString()).ToShortDateString());
                        dataRow.GetCell(2).SetCellValue(item.ShopNo);
                        dataRow.GetCell(3).SetCellValue(item.ShopName);
                        dataRow.GetCell(4).SetCellValue(item.Region);
                        dataRow.GetCell(5).SetCellValue(item.Province);
                        dataRow.GetCell(6).SetCellValue(item.City);
                        dataRow.GetCell(7).SetCellValue(item.CityTier);
                        dataRow.GetCell(8).SetCellValue(item.Channel);
                        dataRow.GetCell(9).SetCellValue(item.Format);
                        dataRow.GetCell(10).SetCellValue(item.POPAddress);
                        dataRow.GetCell(11).SetCellValue(item.Contacts);
                        dataRow.GetCell(12).SetCellValue(item.Tels);

                        dataRow.GetCell(13).SetCellValue(item.POSScale);
                        dataRow.GetCell(14).SetCellValue(item.MaterialSupport);
                        dataRow.GetCell(15).SetCellValue(item.SubjectName);
                        dataRow.GetCell(16).SetCellValue(item.Gender);
                        dataRow.GetCell(17).SetCellValue(item.ChooseImg);


                        //dataRow.GetCell(12).SetCellValue(item.Category);
                        dataRow.GetCell(18).SetCellValue(item.Sheet);
                        dataRow.GetCell(19).SetCellValue(item.MachineFrame);
                        dataRow.GetCell(20).SetCellValue(item.PositionDescription);
                        dataRow.GetCell(21).SetCellValue(item.Quantity);
                        dataRow.GetCell(22).SetCellValue(item.GraphicMaterial);
                        dataRow.GetCell(23).SetCellValue(item.QuoteGraphicMaterial);
                        dataRow.GetCell(24).SetCellValue(item.UnitPrice);
                        dataRow.GetCell(25).SetCellValue(item.GraphicWidth);
                        dataRow.GetCell(26).SetCellValue(item.GraphicLength);
                        dataRow.GetCell(27).SetCellValue(item.Area);
                        if (item.ReceivePrice>0)
                          dataRow.GetCell(28).SetCellValue(item.ReceivePrice);
                        //其他备注
                        dataRow.GetCell(29).SetCellValue(item.OtherRemark);
                        dataRow.GetCell(30).SetCellValue(item.IsInstall);
                        dataRow.GetCell(31).SetCellValue(item.SupplimentSubjectName);
                        //dataRow.GetCell(27).SetCellValue(item.NewFormat);
                        startRow++;

                    }

                    HttpCookie cookie = Request.Cookies["exportQuote350"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportQuote350");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workBook.Write(ms);
                        ms.Flush();

                        sheet = null;

                        workBook = null;
                        string fileName = string.Empty;
                        SubjectGuidance model = new SubjectGuidanceBLL().GetModel(guidanceId);
                        if (model != null)
                        {
                            fileName = model.ItemName + "-";
                        }
                        OperateFile.DownLoadFile(ms, fileName + "报价350表");
                        //OperateFile.DownLoadFile(path);

                    }
                }
                else
                {

                    HttpCookie cookie = Request.Cookies["exportQuote350"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportQuote350");
                    }
                    cookie.Value = "2";//没有数据可以导出
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                }



            }
        }

        

        //系统单350
        void ExportNew350()
        {

            if (!string.IsNullOrWhiteSpace(subjectId))
            {

                List<string> regionList = new List<string>();
                List<string> provinceList = new List<string>();
                List<string> cityList = new List<string>();
                List<int> customerServiceList = new List<int>();
                List<string> installList = new List<string>();
                List<int> categoryList = new List<int>();

                List<int> subjectIdList = StringHelper.ToIntList(subjectId, ',');

                List<string> myRegionList = new BasePage().GetResponsibleRegion;
                if (myRegionList.Any())
                {
                    StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
                }

                List<int> priceOrderTypeList = CommonMethod.GetEnumList<OrderTypeEnum>().Where(s => s.Desction.Contains("费用订单")).Select(s => s.Value).ToList();

                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join category1 in CurrentContext.DbContext.ADSubjectCategory
                            on subject.SubjectCategoryId equals category1.Id into temp1
                            from category in temp1.DefaultIfEmpty()
                           // where subjectIdList.Contains(order.SubjectId ?? 0)
                            where (selectType == 1 ? subjectIdList.Contains(order.SubjectId ?? 0) : subjectIdList.Contains(order.RegionSupplementId ?? 0))
                            && (order.IsValid == null || order.IsValid == true)
                            && (order.IsDelete == null || order.IsDelete == false)
                            && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                            && (myRegionList.Any()?((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())):1==1)
                            && (order.OrderType!=(int)OrderTypeEnum.物料)
                            && !priceOrderTypeList.Contains(order.OrderType??1)//不导出费用订单
                            select new
                            {
                                subject,
                                shop,
                                order,
                                category.CategoryName,
                                //pop
                            }).ToList();

                list = list.Where(s => (s.order.OrderType == 1 && s.order.GraphicLength != null && s.order.GraphicLength > 0 && s.order.GraphicWidth != null && s.order.GraphicWidth > 0) || (s.order.OrderType > 1)).ToList();
                //HC店铺的光盘不导出
                list = list.Where(s => ((s.order.Sheet!=null && s.order.Sheet.Contains("光盘")) || (s.order.PositionDescription != null && s.order.PositionDescription.Contains("光盘"))) ? ((s.order.Format != null && s.order.Format != "") && s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                if (!string.IsNullOrWhiteSpace(exportType))
                {
                    if (exportType == "nohc")
                    {
                        list = list.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") == -1 && s.order.Format.ToLower().IndexOf("homecourt") == -1 && s.order.Format.ToLower().IndexOf("homecore") == -1 && s.order.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                    }
                    else if (exportType == "hc")
                    {
                        list = list.Where(s => (s.order.Format != null && s.order.Format != "") ? (s.order.Format.ToLower().IndexOf("hc") != -1 || s.order.Format.ToLower().IndexOf("homecourt") != -1 || s.order.Format.ToLower().IndexOf("homecore") != -1 || s.order.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    if (materialName == "软膜")
                    {
                        list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                    }
                    else if (materialName == "非软膜")
                    {
                        list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
                    }
                }
                if (string.IsNullOrWhiteSpace(region))
                {
                    
                }
                else
                {
                    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                }
                if (regionList.Any())
                {
                    //list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            list = list.Where(s => regionList0.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();
                        }
                        else
                        {
                            list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();
                            //list = list.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                        }
                    }
                    else
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();
                        //list = list.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                    //list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    if (provinceList.Contains("空"))
                    {
                        List<string> provinceList0 = provinceList;
                        provinceList0.Remove("空");
                        if (provinceList0.Any())
                        {
                            list = list.Where(s => provinceList0.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                        }
                        else
                        {
                            list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                        }
                    }
                    else
                    {
                        list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();

                    }
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                    //list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    if (cityList.Contains("空"))
                    {
                        List<string> cityList0 = cityList;
                        cityList0.Remove("空");
                        if (cityList0.Any())
                        {
                            list = list.Where(s => cityList0.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                        }
                        else
                        {
                            list = list.Where(s => s.order.City == null || s.order.City == "").ToList();

                        }
                    }
                    else
                    {
                        list = list.Where(s => cityList.Contains(s.order.City)).ToList();

                    }
                }
                if (!string.IsNullOrWhiteSpace(customerServiceId))
                {
                    customerServiceList = StringHelper.ToIntList(customerServiceId, ',');
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                        }
                        else
                            list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(isInstall))
                {
                    installList = StringHelper.ToStringList(isInstall, ',');
                    if (installList.Contains("无"))
                    {
                        installList.Remove("无");
                        if (installList.Any())
                        {
                            list = list.Where(s => installList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                        }
                        else
                            list = list.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                    }
                    else
                        list = list.Where(s => installList.Contains(s.order.IsInstall)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(materialCategoryId))
                {
                    bool hasEmpty = false;
                    List<string> materialList = new List<string>();
                    categoryList = StringHelper.ToIntList(materialCategoryId, ',');
                    if (categoryList.Contains(0))
                    {
                        hasEmpty = true;
                        categoryList.Remove(0);
                    }
                    if (categoryList.Any())
                    {
                        materialList = new OrderMaterialMppingBLL().GetList(s => categoryList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                    }
                    if (hasEmpty)
                    {
                        if (materialList.Any())
                        {
                            list = list.Where(s =>(s.order.GraphicMaterial!=null && materialList.Contains(s.order.GraphicMaterial.ToLower())) || (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();

                        }
                        else
                            list = list.Where(s => (s.order.GraphicMaterial == null || s.order.GraphicMaterial == "")).ToList();
                    }
                    else
                        list = list.Where(s => s.order.GraphicMaterial != null && materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(exportShopNo))
                {
                    List<string> exportShopNoList = StringHelper.ToStringList(exportShopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower); ;
                    if (exportShopNoList.Any())
                    {
                        if (isNotInclude == "1")
                        {
                            list = list.Where(s => !exportShopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
                        }
                        else
                        {
                            list = list.Where(s => exportShopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
                        }
                    }
                }
                
                List<Order350Model> orderList = new List<Order350Model>();
                int guidanceId = list.Select(s => s.subject.GuidanceId ?? 0).FirstOrDefault();
                if (list.Any())
                {
                    string changePOPCountSheetStr = string.Empty;
                    List<string> ChangePOPCountSheetList = new List<string>();
                    try
                    {
                        changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrWhiteSpace(changePOPCountSheetStr))
                    {
                        ChangePOPCountSheetList = StringHelper.ToStringList(changePOPCountSheetStr,'|');
                    }
                    #region
                    var HCSmallSizeList = new HCSmallGraphicSizeBLL().GetList(s=>s.Id>0);
                    foreach (var item in list)
                    {
                        //string status = string.Empty;
                        //if (item.shop.Status != null)
                        //    status = item.shop.Status;
                        
                        int level = item.order.LevelNum ?? 0;
                        string levelName = string.Empty;

                        if (level > 0)
                            levelName = CommonMethod.GeEnumName<LevelNumEnum>(level.ToString());
                        bool isTakeSmallSize = false;
                        string format = item.order.Format??string.Empty;
                        if (item.order.IsSplit == null || item.order.IsSplit == false && !string.IsNullOrWhiteSpace(format) && (format.ToUpper().Contains("TERREX") || format.ToUpper().Contains("YA") || format.ToUpper().Contains("HCE")) && item.order.Sheet.Contains("鞋墙"))
                        {
                            var parentSize = HCSmallSizeList.Where(s => s.ParentId == 0 && format.ToUpper().Contains(s.Format.ToUpper()) && s.BigGraphicWidth == item.order.GraphicWidth && s.BigGraphicLength == item.order.GraphicLength).FirstOrDefault();
                            if (parentSize != null)
                            {
                                var hasSmallList = HCSmallSizeList.Where(s => s.ParentId == parentSize.Id).ToList();
                                if (hasSmallList.Any())
                                {
                                    isTakeSmallSize = true;
                                    hasSmallList.ForEach(size =>
                                    {
                                        Order350Model model = new Order350Model();
                                        decimal area = 0;
                                        if (size.SmallGraphicWidth != null && size.SmallGraphicLength != null)
                                        {
                                            area = ((size.SmallGraphicWidth ?? 0) * (size.SmallGraphicLength ?? 0)) / 1000000;
                                        }
                                        model.Area = double.Parse(area.ToString());

                                        //model.Category = (item.order.Category != null && item.order.Category != "") ? item.order.Category : item.pop != null ? item.pop.Category : "";
                                        model.Category = item.order.Category;
                                        model.ChooseImg = item.order.ChooseImg;

                                        model.City = item.order.City;
                                        model.County = item.shop.AreaName;
                                        model.CityTier = item.order.CityTier;
                                        model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                                        model.Format = item.order.Format;
                                        model.Gender = !string.IsNullOrWhiteSpace(item.order.OrderGender) ? item.order.OrderGender : item.order.Gender;
                                        //model.Gender = item.order.Gender;
                                        model.GraphicLength = size.SmallGraphicLength != null ? double.Parse(size.SmallGraphicLength.ToString()) : 0;
                                        string smallMaterial = GetSmallMaterial(item.order.SmallMaterialId ?? 0);
                                        string material = item.order.GraphicMaterial;
                                        if (!string.IsNullOrWhiteSpace(smallMaterial))
                                            material += ("+" + smallMaterial);
                                        model.GraphicMaterial = material;
                                        model.GraphicWidth = size.SmallGraphicWidth != null ? double.Parse(size.SmallGraphicWidth.ToString()) : 0; ;
                                        model.POPAddress = item.shop.POPAddress;
                                        model.PositionDescription = item.order.PositionDescription;
                                        model.Province = item.shop.ProvinceName;
                                        if (item.order.Sheet!=null && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(item.order.Sheet.ToUpper()))
                                        {
                                            model.Quantity = (item.order.Quantity ?? 0) > 0 ? 1 : 0;
                                        }
                                        else
                                        {
                                            model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                        }
                                        model.Sheet = item.order.Sheet;
                                        model.ShopName = item.order.ShopName;
                                        model.ShopNo = item.order.ShopNo;
                                        string subjectName = item.subject.SubjectName;
                                        if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                                            subjectName += "(" + item.subject.Remark + ")";
                                        model.SubjectName = subjectName;
                                        model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                                        //if (item.CategoryName!=null && item.CategoryName.Contains("童店"))//童店
                                        model.OtherRemark = item.order.Remark;
                                        model.POSScale = item.order.POSScale;
                                        model.MaterialSupport = item.order.MaterialSupport;
                                        orderList.Add(model);
                                    });
                                }
                            }
                        }
                        if (!isTakeSmallSize)
                        {
                            double GraphicLength = item.order.GraphicLength != null ? double.Parse(item.order.GraphicLength.ToString()) : 0;
                            double GraphicWidth = item.order.GraphicWidth != null ? double.Parse(item.order.GraphicWidth.ToString()) : 0;
                            bool canGo = true;
                            if (!string.IsNullOrWhiteSpace(item.order.GraphicNo) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(item.order.Sheet.ToUpper()))
                            {
                                var checkList = orderList.Where(s => s.SubjectId == item.subject.Id && s.ShopId == item.shop.Id && s.Sheet == item.order.Sheet && s.PositionDescription == item.order.PositionDescription && s.GraphicNo == item.order.GraphicNo && s.GraphicLength == GraphicLength && s.GraphicWidth == GraphicWidth && s.Gender==item.order.Gender).ToList();
                                if (checkList.Any())
                                    canGo = false;
                            }
                            if (canGo)
                            {
                                string subjectName = item.subject.SubjectName;
                                if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(item.subject.Remark))
                                    subjectName += "(" + item.subject.Remark + ")";

                                Order350Model model = new Order350Model();
                                model.ShopId = item.shop.Id;
                                model.SubjectId = item.subject.Id;
                                model.GraphicNo = item.order.GraphicNo;
                                decimal area = 0;
                                if (item.order.GraphicWidth != null && item.order.GraphicLength != null)
                                {
                                    area = ((item.order.GraphicWidth ?? 0) * (item.order.GraphicLength ?? 0)) / 1000000;
                                }
                                model.Area = double.Parse(area.ToString());

                                //model.Category = (item.order.Category != null && item.order.Category != "") ? item.order.Category : item.pop != null ? item.pop.Category : "";
                                model.Category = item.order.Category;
                                model.ChooseImg = item.order.ChooseImg;

                                model.City = item.order.City;
                                model.County = item.shop.AreaName;
                                model.CityTier = item.order.CityTier;
                                model.Contacts = item.shop.Contact1 + "/" + item.shop.Contact2;
                                model.Format = item.order.Format;
                                model.Gender = !string.IsNullOrWhiteSpace(item.order.OrderGender) ? item.order.OrderGender : item.order.Gender;
                                //model.Gender = item.order.Gender;
                                model.GraphicLength = GraphicLength;
                                string smallMaterial = GetSmallMaterial(item.order.SmallMaterialId ?? 0);
                                string material = item.order.GraphicMaterial;
                                if (!string.IsNullOrWhiteSpace(smallMaterial))
                                    material += ("+" + smallMaterial);
                                model.GraphicMaterial = material;
                                model.GraphicWidth = GraphicWidth;
                                model.POPAddress = item.shop.POPAddress;
                                model.PositionDescription = item.order.PositionDescription;
                                model.Province = item.order.Province;
                                if (item.order.Sheet!=null&&ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(item.order.Sheet.ToUpper()))
                                {
                                    model.Quantity = (item.order.Quantity ?? 0) > 0 ? 1 : 0;
                                }
                                else
                                {
                                    model.Quantity = item.order.Quantity != null ? double.Parse(item.order.Quantity.ToString()) : 0;
                                }
                                model.Sheet = item.order.Sheet;
                                model.ShopName = item.order.ShopName;
                                model.ShopNo = item.order.ShopNo;

                                model.SubjectName = subjectName;
                                model.Tels = item.shop.Tel1 + "/" + item.shop.Tel2;
                                //if (item.CategoryName != null && item.CategoryName.Contains("童店"))//童店
                                model.OtherRemark = item.order.Remark;
                                model.POSScale = item.order.POSScale;
                                model.MaterialSupport = item.order.MaterialSupport;

                                orderList.Add(model);
                            }
                        }
                    }
                    #endregion
                }
                #region 物料导出

                var orderMaterialList = (from material in CurrentContext.DbContext.OrderMaterial
                                         join subject in CurrentContext.DbContext.Subject
                                         on material.SubjectId equals subject.Id
                                         join shop in CurrentContext.DbContext.Shop
                                         on material.ShopId equals shop.Id
                                         where subjectIdList.Contains(material.SubjectId ?? 0)
                                         && (shop.Status == null || shop.Status == "" || shop.Status == ShopStatusEnum.正常.ToString())
                                         && (material.MaterialName.ToLower().Contains("光盘") ? ((shop.Format != null && shop.Format != "") && shop.Format.ToLower().IndexOf("hc") == -1 && shop.Format.ToLower().IndexOf("homecourt") == -1 && shop.Format.ToLower().IndexOf("homecore") == -1 && shop.Format.ToLower().IndexOf("ya") == -1) : 1 == 1)
                                         select new
                                         {
                                             material,
                                             subject,
                                             shop
                                         }).ToList();
                if (!string.IsNullOrWhiteSpace(exportType))
                {
                    if (exportType == "nohc")
                    {
                        list = list.Where(s => (s.shop.Format != null && s.shop.Format != "") ? (s.shop.Format.ToLower().IndexOf("hc") == -1 && s.shop.Format.ToLower().IndexOf("homecourt") == -1 && s.shop.Format.ToLower().IndexOf("homecore") == -1) : 1 == 1).ToList();
                    }
                    else if (exportType == "hc")
                    {
                        list = list.Where(s => (s.shop.Format != null && s.shop.Format != "") ? (s.shop.Format.ToLower().IndexOf("hc") != -1 || s.shop.Format.ToLower().IndexOf("homecourt") != -1 || s.shop.Format.ToLower().IndexOf("homecore") != -1) : 1 == 1).ToList();
                    }
                }
                if (regionList.Any())
                {
                    //orderMaterialList = orderMaterialList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (regionList.Contains("空"))
                    {
                        List<string> regionList0 = regionList;
                        regionList0.Remove("空");
                        if (regionList0.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => regionList0.Contains(s.shop.RegionName.ToLower()) || s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                        }
                        else
                            orderMaterialList = orderMaterialList.Where(s => s.shop.RegionName == null || s.shop.RegionName == "").ToList();
                    }
                    else
                        orderMaterialList = orderMaterialList.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                }

                if (provinceList.Any())
                {
                    //orderMaterialList = orderMaterialList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    if (provinceList.Contains("空"))
                    {
                        List<string> provinceList0 = provinceList;
                        provinceList0.Remove("空");
                        if (provinceList0.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => provinceList0.Contains(s.shop.ProvinceName) || s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();

                        }
                        else
                        {
                            orderMaterialList = orderMaterialList.Where(s => s.shop.ProvinceName == null || s.shop.ProvinceName == "").ToList();

                        }
                    }
                    else
                    {
                        orderMaterialList = orderMaterialList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();

                    }
                }
                if (cityList.Any())
                {
                    //orderMaterialList = orderMaterialList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    if (cityList.Contains("空"))
                    {
                        List<string> cityList0 = cityList;
                        cityList0.Remove("空");
                        if (cityList0.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => cityList0.Contains(s.shop.CityName) || s.shop.CityName == null || s.shop.CityName == "").ToList();

                        }
                        else
                        {
                            orderMaterialList = orderMaterialList.Where(s => s.shop.CityName == null || s.shop.CityName == "").ToList();

                        }
                    }
                    else
                    {
                        orderMaterialList = orderMaterialList.Where(s => cityList.Contains(s.shop.CityName)).ToList();

                    }
                }
                if (customerServiceList.Any())
                {
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                        }
                        else
                            orderMaterialList = orderMaterialList.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                    }
                    else
                    {
                        orderMaterialList = orderMaterialList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                    }
                }
                if (installList.Any())
                {
                    if (installList.Contains("无"))
                    {
                        installList.Remove("无");
                        if (installList.Any())
                        {
                            orderMaterialList = orderMaterialList.Where(s => installList.Contains(s.shop.IsInstall) || (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                        }
                        else
                            orderMaterialList = orderMaterialList.Where(s => (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                    }
                    else
                        orderMaterialList = orderMaterialList.Where(s => installList.Contains(s.shop.IsInstall)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(exportShopNo))
                {
                    List<string> exportShopNoList = StringHelper.ToStringList(exportShopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower); ;
                    if (exportShopNoList.Any())
                    {
                        if (isNotInclude == "1")
                        {
                            orderMaterialList = orderMaterialList.Where(s => !exportShopNoList.Contains(s.shop.ShopNo.ToLower())).ToList();
                        }
                        else
                        {
                            orderMaterialList = orderMaterialList.Where(s => exportShopNoList.Contains(s.shop.ShopNo.ToLower())).ToList();
                        }
                    }
                }
                if (orderMaterialList.Any())
                {
                    orderMaterialList.ForEach(s =>
                    {
                        Order350Model model = new Order350Model();
                        model.Area = 0;
                        model.Category = "";
                        model.ChooseImg = "";
                        model.City = s.shop.CityName;
                        model.County = s.shop.AreaName;
                        model.CityTier = s.shop.CityTier;
                        model.Contacts = s.shop.Contact1 + "/" + s.shop.Contact2;
                        model.Format = s.shop.Format;
                        model.Gender = "";
                        model.GraphicLength = 0;
                        model.GraphicMaterial = "";
                        model.GraphicWidth = 0;
                        model.POPAddress = s.shop.POPAddress;
                        StringBuilder size = new StringBuilder();
                        if (s.material.MaterialLength != null && s.material.MaterialLength > 0 && s.material.MaterialWidth != null && s.material.MaterialWidth > 0)
                        {
                            size.AppendFormat("({0}*{1}", s.material.MaterialLength, s.material.MaterialWidth);
                            if (s.material.MaterialHigh != null && s.material.MaterialHigh > 0)
                                size.AppendFormat("*{0}", s.material.MaterialHigh);
                            size.Append(")");
                        }
                        model.PositionDescription = size.ToString();
                        model.Province = s.shop.ProvinceName;
                        model.Quantity = s.material.MaterialCount != null ? double.Parse(s.material.MaterialCount.ToString()) : 0;
                        model.Sheet = s.material.MaterialName;
                        model.ShopName = s.shop.ShopName;
                        model.ShopNo = s.shop.ShopNo;
                        model.SubjectName = s.subject.SubjectName;
                        model.Tels = s.shop.Tel1 + "/" + s.shop.Tel2;
                        model.OtherRemark = s.material.Remark;
                        model.ShopLevel = s.shop.ShopLevel;
                        orderList.Add(model);
                    });
                }



                #endregion
                if (orderList.Any())
                {
                    orderList = orderList.OrderBy(s => s.ShopNo).ThenBy(s=>s.Sheet).ToList();
                    string templateFileName = "350模板";
                    string path = ConfigurationManager.AppSettings["ExportTemplate"];
                    path = path.Replace("fileName", templateFileName);
                    FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read);
                    ExcelPackage package = new ExcelPackage(outFile);
                    ExcelWorksheet sheet = package.Workbook.Worksheets[1];

                    int startRow = 2;
                    string shopno = string.Empty;
                    foreach (var item in orderList)
                    {


                        if (startRow == 2)
                            shopno = item.ShopNo;
                        else
                        {
                            if (shopno != item.ShopNo)
                            {
                                shopno = item.ShopNo;
                               
                                while (startRow.ToString().Substring(startRow.ToString().Length - 1, 1) != "2")
                                {
                                    startRow++;
                                  
                                }

                            }
                        }


                        sheet.Cells[startRow, 1].Value = item.ShopNo;
                        sheet.Cells[startRow, 2].Value = item.ShopName;
                        sheet.Cells[startRow, 3].Value = item.Province;
                        sheet.Cells[startRow, 4].Value = item.City;
                        sheet.Cells[startRow, 5].Value = item.CityTier;
                        sheet.Cells[startRow, 6].Value = item.Format;
                        sheet.Cells[startRow, 7].Value = item.POPAddress;
                        sheet.Cells[startRow, 8].Value = item.Contacts;
                        sheet.Cells[startRow, 9].Value = item.Tels;
                        sheet.Cells[startRow, 10].Value = "";
                        sheet.Cells[startRow, 11].Value = item.SubjectName;
                        sheet.Cells[startRow, 12].Value = item.Gender;
                        sheet.Cells[startRow, 13].Value = item.ChooseImg;
                        sheet.Cells[startRow, 14].Value = item.Sheet;
                        sheet.Cells[startRow, 15].Value = item.Quantity;
                        sheet.Cells[startRow, 16].Value = item.GraphicMaterial;
                        sheet.Cells[startRow, 17].Value = item.GraphicWidth;
                        sheet.Cells[startRow, 18].Value = item.GraphicLength;
                        sheet.Cells[startRow, 19].Value = item.Area;
                        sheet.Cells[startRow, 20].Value = item.OtherRemark;
                        sheet.Cells[startRow, 21].Value = item.PositionDescription;

                        startRow++;

                    }
                   
                    HttpCookie cookie = Request.Cookies["exportNew350"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportNew350");
                    }
                    cookie.Value = "1";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        //workBook.Write(ms);
                        package.SaveAs(ms);
                        ms.Flush();
                        sheet = null;
                        //workBook = null;
                        string fileName = string.Empty;
                        SubjectGuidance model = new SubjectGuidanceBLL().GetModel(guidanceId);
                        if (model != null)
                        {
                            fileName = model.ItemName + "-";
                        }
                        OperateFile.DownLoadFile(ms, fileName + "350订单表");

                    }
                }
                else
                {


                    HttpCookie cookie = Request.Cookies["exportNew350"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("exportNew350");
                    }
                    cookie.Value = "2";//没有数据可以导出
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);

                }




            }
        }
    }
}
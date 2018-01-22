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
using DAL;

namespace WebApp
{
    public partial class importPlace : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ok"] != null)
                Label1.Text = "导入完成";
        }

        protected void Button1_Click(object sender, EventArgs e)
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
                    PlaceBLL placeBll = new PlaceBLL();
                    Place model;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int id =int.Parse(dr["Id"].ToString().Trim());
                        string placeName = dr["PlaceName"].ToString().Trim();
                        int ParentId = int.Parse(dr["ParentId"].ToString());
                        //string AreaSort = dr["AreaSort"].ToString();
                        //int AreaDeep = int.Parse(dr["AreaDeep"].ToString());
                        string AreaRegion = dr["Region"].ToString();
                        model = new Place() { ID = id,AreaRegion = AreaRegion, ParentID = ParentId, PlaceName = placeName };
                        placeBll.Insert(model);
                    }
                    conn.Dispose();
                    conn.Close();
                    Response.Redirect("importPlace.aspx?ok=1", false);
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on order.GuidanceId equals guidance.ItemId
                             where subject.ApproveState == 1
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && (guidance.IsDelete == null || guidance.IsDelete == false)
                             && (order.IsDelete == null || order.IsDelete == false)
                             && (order.IsValid == null || order.IsValid == true)
                             && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                             && guidance.ActivityTypeId == (int)GuidanceTypeEnum.Promotion
                             //&& order.GuidanceId >10
                             //&& order.GuidanceId<=50
                             select new
                             {
                                 order.GuidanceId,
                                 order.ShopId
                             }).ToList();
            List<string> list = new List<string>();
            ExpressPriceDetailBLL priceBll = new ExpressPriceDetailBLL();
            ExpressPriceDetail priceModel;
            int shopCount = 0;
            orderList.ForEach(s => {
                string shop = s.GuidanceId + "|" + s.ShopId;
                if (!list.Contains(shop))
                {
                    priceModel = new ExpressPriceDetail();
                    priceModel.AddDate = DateTime.Now;
                    priceModel.ExpressPrice = 35;
                    priceModel.GuidanceId = s.GuidanceId;
                    priceModel.ShopId = s.ShopId;
                    priceBll.Add(priceModel);
                    shopCount++;
                    list.Add(shop);
                }
                
            });
            Label2.Text = "更新成功！店铺数量：" + shopCount;
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            var materialOrderList = (from material in CurrentContext.DbContext.OrderMaterial
                                     join shop in CurrentContext.DbContext.Shop
                                     on material.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on material.SubjectId equals subject.Id
                                     
                                     select new
                                     {
                                         subject,
                                         material,
                                         shop
                                     }).ToList();
            int count = 0;
            if (materialOrderList.Any())
            {
                FinalOrderDetailTempBLL finalOrderTempBll = new FinalOrderDetailTempBLL();
                FinalOrderDetailTemp finalOrderTempModel;
                materialOrderList.ForEach(o =>
                {
                    count++;
                    finalOrderTempModel = new FinalOrderDetailTemp();
                    finalOrderTempModel.AgentCode = o.shop.AgentCode;
                    finalOrderTempModel.AgentName = o.shop.AgentName;
                    finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                    finalOrderTempModel.Channel = o.shop.Channel;
                    finalOrderTempModel.City = o.shop.CityName;
                    finalOrderTempModel.CityTier = o.shop.CityTier;
                    finalOrderTempModel.Contact = o.shop.Contact1;
                    finalOrderTempModel.Format = o.shop.Format;
                    //finalOrderTempModel.Gender = o.order.Gender;
                    //finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                    finalOrderTempModel.IsInstall = o.shop.IsInstall;
                    finalOrderTempModel.LocationType = o.shop.LocationType;
                    //finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                    finalOrderTempModel.OrderType = (int)OrderTypeEnum.物料;
                    finalOrderTempModel.POPAddress = o.shop.POPAddress;
                    //finalOrderTempModel.POSScale = o.order.POSScale;
                    //finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                    //finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                    //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                    //if (subjectCategoryName == "童店")
                    //finalOrderTempModel.InstallPriceMaterialSupport = "Others";
                    finalOrderTempModel.Province = o.shop.ProvinceName;
                    finalOrderTempModel.Quantity = o.material.MaterialCount;
                    finalOrderTempModel.Region = o.shop.RegionName;
                    //finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                    finalOrderTempModel.Sheet = o.material.MaterialName;
                    StringBuilder size = new StringBuilder();
                    if (o.material.MaterialLength != null && o.material.MaterialLength > 0 && o.material.MaterialWidth != null && o.material.MaterialWidth > 0)
                    {
                        size.AppendFormat("({0}*{1}", o.material.MaterialLength, o.material.MaterialWidth);
                        if (o.material.MaterialHigh != null && o.material.MaterialHigh > 0)
                            size.AppendFormat("*{0}", o.material.MaterialHigh);
                        size.Append(")");
                    }
                    finalOrderTempModel.PositionDescription = size.ToString();
                    finalOrderTempModel.Remark = o.material.Remark;
                    finalOrderTempModel.ShopId = o.shop.Id;
                    finalOrderTempModel.ShopName = o.shop.ShopName;
                    finalOrderTempModel.ShopNo = o.shop.ShopNo;
                    finalOrderTempModel.SubjectId = o.subject.Id;
                    finalOrderTempModel.Tel = o.shop.Tel1;
                    finalOrderTempModel.MachineFrame = "";
                    if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                    {
                        finalOrderTempModel.IsFromRegion = true;
                    }
                    finalOrderTempModel.ShopStatus = o.shop.Status;
                    finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                    if ((o.material.Price ?? 0) > 0)
                    {
                        finalOrderTempModel.UnitPrice = o.material.Price;
                        finalOrderTempModel.OrderPrice = (o.material.Price ?? 0) * (o.material.MaterialCount ?? 0);
                        finalOrderTempModel.PayOrderPrice = (o.material.PayPrice ?? 0) * (o.material.MaterialCount ?? 0);
                    }
                    finalOrderTempModel.CSUserId = o.shop.CSUserId;
                    finalOrderTempBll.Add(finalOrderTempModel);
                });
            }
            Label3.Text = "更新成功！订单数量：" + count;
        }
    }
}
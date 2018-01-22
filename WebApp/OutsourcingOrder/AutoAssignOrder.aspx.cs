using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Transactions;
using System.Text;
using Common;
using System.Configuration;

namespace WebApp.OutsourcingOrder
{
    public partial class AutoAssignOrder : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                BindGuidance();
                Region();
                BindAssignOrder();
            }
        }




        void BindGuidance()
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance

                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance

                        }).Distinct().ToList();


            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();

            }

            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    cblGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyGuidance.Visible = !list.Any();
        }

        void Region()
        {
            cblRegion.Items.Clear();
            //List<int> guidanceList = new List<int>();
            //foreach (ListItem li in cblGuidanceList.Items)
            //{
            //    if (li.Selected)
            //    {
            //        guidanceList.Add(int.Parse(li.Value));
            //    }
            //}
            //if (!guidanceList.Any())
            //{
            //    foreach (ListItem li in cblGuidanceList.Items)
            //    {
            //        guidanceList.Add(int.Parse(li.Value));
            //    }
            //}

            //if (guidanceList.Any())
            //{
            //    //List<FinalOrderDetailTemp> list = new FinalOrderDetailTempBLL().GetList(s => guidanceList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == 1 && s.GraphicLength != null && s.GraphicLength > 0 && s.GraphicWidth != null && s.GraphicWidth > 0) || s.OrderType > 1) && (s.IsValid == null || s.IsValid == true) && (s.IsProduce == null || s.IsProduce == true) && (s.OrderType != (int)OrderTypeEnum.物料) && (s.IsValidFromAssign == null || s.IsValidFromAssign == true));
                
            //}
            if (!finalOrderList0.Any())
            {
                LoadFinalOrderList();
            }
            var orderList = (from order in finalOrderList0
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where 
                             //guidanceList.Contains(order.GuidanceId ?? 0)
                            (subject.IsDelete == null || subject.IsDelete == false)
                             select order.Region).Distinct().ToList();
            orderList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s;
                li.Text = s + "&nbsp;&nbsp;";
                cblRegion.Items.Add(li);
            });
        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Region();
            //BindAssignOrder();
        }




        protected void txtMonth_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text.Trim()))
            {
                BindGuidance();
                Region();
                finalOrderList0.Clear();
                BindAssignOrder();
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
            Region();
        }

        protected void lbUp_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month <= 1)
                {
                    year--;
                    month = 12;
                }
                else
                    month--;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                Region();
                finalOrderList0.Clear();
                BindAssignOrder();
            }
        }

        protected void lbDown_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month >= 12)
                {
                    year++;
                    month = 1;
                }
                else
                    month++;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                Region();
                finalOrderList0.Clear();
                BindAssignOrder();
            }
        }


        int totalOrderCount = 0;
        int assignOrderCount = 0;
        int repeatOrderCount = 0;
        List<POP> oohPOPList = new List<POP>();
        InstallPriceTempBLL installShopPriceBll = new InstallPriceTempBLL();
        List<string> ChangePOPCountSheetList = new List<string>();
        List<ExpressPriceConfig> expressPriceConfigList = new List<ExpressPriceConfig>();
        ExpressPriceDetailBLL expressPriceDetailBll = new ExpressPriceDetailBLL();
        ExpressPriceDetail expressPriceDetailModel;

        List<OutsourceOrderAssignConfig> configList = new List<OutsourceOrderAssignConfig>();
        List<Place> placeList = new List<Place>();
        int calerOutsourceId = 8;
        /// <summary>
        /// 开始分单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<int> guidanceList = new List<int>();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                {
                    guidanceList.Add(int.Parse(li.Value));
                }
            }
            //if (!guidanceList.Any())
            //{
            //    foreach (ListItem li in cblGuidanceList.Items)
            //    {
            //        guidanceList.Add(int.Parse(li.Value));
            //    }
            //}
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value);
                }
            }
            if (guidanceList.Any())
            {
                string changePOPCountSheetStr = string.Empty;
                string beiJingCalerOutsourceName = string.Empty;
                try
                {
                    changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                    beiJingCalerOutsourceName = ConfigurationManager.AppSettings["CalerOutsourceName"];
                }
                catch
                {

                }
                if (!string.IsNullOrWhiteSpace(changePOPCountSheetStr))
                {
                    ChangePOPCountSheetList = StringHelper.ToStringList(changePOPCountSheetStr, '|');
                }
                if (!string.IsNullOrWhiteSpace(beiJingCalerOutsourceName))
                {
                    Company companyModel = new CompanyBLL().GetList(s => (s.CompanyName == beiJingCalerOutsourceName || s.ShortName == beiJingCalerOutsourceName) && s.TypeId==(int)CompanyTypeEnum.Outsource).FirstOrDefault();
                    if(companyModel!=null)
                        calerOutsourceId = companyModel.Id;
                }
                FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                OutsourceAssignShopBLL outsourceAssignShopBll = new OutsourceAssignShopBLL();
               
                int guidanceNum = 0;
                string shopno = string.Empty;
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        expressPriceConfigList = new ExpressPriceConfigBLL().GetList(s => s.Id > 0);
                        List<string> BCSCityTierList = new List<string>() { "T1", "T2", "T3" };
                        guidanceList.ForEach(guidanceId =>
                        {
                            outsourceAssignShopBll.Delete(s => s.GuidanceId == guidanceId);
                            
                            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                             join shop in CurrentContext.DbContext.Shop
                                             on order.ShopId equals shop.Id
                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                             on order.GuidanceId equals guidance.ItemId
                                             where order.GuidanceId == guidanceId
                                             && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength >1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType > 1)
                                             && (order.IsValid == null || order.IsValid == true)
                                             && (order.IsDelete == null || order.IsDelete == false)
                                             && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                                             && (order.IsProduce == null || order.IsProduce == true)
                                             && (order.IsValidFromAssign == null || order.IsValidFromAssign == true)
                                             && (order.OrderType != (int)OrderTypeEnum.物料)
                                             select new
                                             {
                                                 order,
                                                 shop,
                                                 guidance
                                             }).ToList();
                            if (regionList.Any())
                            {
                                orderList = orderList.Where(s => regionList.Contains(s.order.Region)).ToList();
                            }
                            if (orderList.Any())
                            {
                                
                                placeList = new PlaceBLL().GetList(s => s.ID > 0);
                                configList = new OutsourceOrderAssignConfigBLL().GetList(s => s.Id > 0);
                                outsourceOrderDetailBll.Delete(s => s.GuidanceId == guidanceId);
                                
                                List<Shop> shopList = orderList.Select(order => order.shop).Distinct().ToList();
                                List<int> shopIdList = shopList.Select(shop => shop.Id).ToList();
                                oohPOPList = new POPBLL().GetList(pop => shopIdList.Contains(pop.ShopId ?? 0) && (pop.Sheet == "户外" || pop.Sheet.ToLower() == "ooh") && (pop.OOHInstallPrice ?? 0) > 0);
                                shopList.ForEach(shop =>
                                {

                                    expressPriceDetailModel = null;
                                   
                                    expressPriceDetailModel = expressPriceDetailBll.GetList(price => price.GuidanceId == guidanceId && price.ShopId == shop.Id).FirstOrDefault();
                                    OutsourceAssignOrderModel assignModel = new OutsourceAssignOrderModel();
                                  
                                    assignModel.Guidance = orderList[0].guidance;

                                    var orderList0 = orderList.Where(l => l.order.ShopId == shop.Id).Select(l => l.order).ToList();
                                    assignModel.OrderList = orderList0;
                                    assignModel.Shop = shop;
                                    assignModel.BCSCityTierList = BCSCityTierList;
                                   
                                    SaveAssignOrderNew(assignModel);
                                  
                                });
                            }
                            guidanceNum++;
                        });
                        tran.Complete();
                        string msg = string.Format("分单成功：活动数量 {0}，订单数量 {1}", guidanceNum, assignOrderCount);
                        labAssignState.Text = msg;
                        ScriptManager.RegisterClientScriptBlock(UpdatePanel1, GetType(), "Refresh", "Refresh()", true);  
                    }
                    catch (Exception ex)
                    {

                        string msg = string.Format("分单失败：" + ex.Message);
                        labAssignState.Text = msg;
                    }
                }
            }
        }



        OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
        OutsourceOrderDetail outsourceOrderDetailModel;
        void SaveOutsourceOrder(OutsourceAssignOrderModel assignOrderModel, out decimal basicInstallPrice, out decimal oohInstallPrice)
        {
            basicInstallPrice = 0;
            oohInstallPrice = 0;
            //hasExtraInstallPrice = false;
            string materialSupport = string.Empty;
            string posScale = string.Empty;
            if (assignOrderModel != null && assignOrderModel.Shop != null)
            {

                int customerId = assignOrderModel.CustomerId;
                int outsourceId = assignOrderModel.Shop.OutsourceId ?? 0;
                int guidanceId = assignOrderModel.GuidanceId;
                int guidanceType = assignOrderModel.GuidanceType;
                int shopId = assignOrderModel.Shop.Id;
                int assignType = assignOrderModel.AssignType;
                var OutsourceOrderList = assignOrderModel.OrderList;
                int extraInstallPriceSubjectId = 0;
                //int subjectId = assignOrderModel.OrderList.Any() ? assignOrderModel.OrderList[0].SubjectId ?? 0 : 0;
                if (OutsourceOrderList.Any())
                {
                    if (assignOrderModel.Shop.ProvinceName.Contains("内蒙古") && !assignOrderModel.Shop.CityName.Contains("通辽"))
                    {
                        assignType = (int)OutsourceOrderTypeEnum.Install;
                    }
                    //物料支持级别
                    int index = 0;
                    materialSupport = OutsourceOrderList[index].InstallPriceMaterialSupport;
                    posScale = OutsourceOrderList[index].POSScale;
                    while (string.IsNullOrWhiteSpace(materialSupport))
                    {
                        index++;
                        if (index == OutsourceOrderList.Count)
                            break;
                        materialSupport = OutsourceOrderList[index].InstallPriceMaterialSupport;
                    }
                    int index1 = 0;
                    while (string.IsNullOrWhiteSpace(posScale))
                    {
                        index1++;
                        if (index1 == OutsourceOrderList.Count)
                            break;
                        posScale = OutsourceOrderList[index1].POSScale;
                    }

                    if (guidanceType == (int)GuidanceTypeEnum.Install && (assignOrderModel.Shop.IsInstall == "Y" || assignOrderModel.Shop.BCSIsInstall == "Y"))
                    {
                        //按照级别，获取基础安装费
                        basicInstallPrice = new BasePage().GetOutsourceBasicInstallPrice(materialSupport);
                        //获取户外安装费

                        var oohList = OutsourceOrderList.Where(s => (s.Sheet != null && (s.Sheet.Contains("户外") || s.Sheet.ToLower() == "ooh"))).ToList();
                        if (oohList.Any())
                        {

                            Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                            oohList.ForEach(s =>
                            {
                                decimal price = 0;
                                if (!string.IsNullOrWhiteSpace(s.GraphicNo))
                                {
                                    price = oohPOPList.Where(p => p.ShopId == shopId && p.GraphicNo.ToLower() == s.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                }
                                else
                                    price = oohPOPList.Where(p => p.ShopId == shopId && p.GraphicLength == s.GraphicLength && p.GraphicWidth == s.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                if (oohPriceDic.Keys.Contains(shopId))
                                {
                                    if (oohPriceDic[shopId] < price)
                                    {
                                        oohPriceDic[shopId] = price;
                                    }
                                }
                                else
                                    oohPriceDic.Add(shopId, price);
                            });

                            if (oohPriceDic.Keys.Count > 0)
                            {
                                foreach (KeyValuePair<int, decimal> item in oohPriceDic)
                                {
                                    oohInstallPrice = item.Value;
                                }
                            }
                        }

                    }
                    List<Order350Model> savedList = new List<Order350Model>();
                    bool hasInstallPrice = false;
                    bool addInstallPrice = false;
                    bool hasPOP = false;
                    #region
                    OutsourceOrderList.ForEach(s =>
                    {
                        if (s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费 || s.OrderType == (int)OrderTypeEnum.其他费用)
                        {
                            hasInstallPrice = true;
                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                            int Quantity = s.Quantity ?? 1;
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = s.AgentCode;
                            outsourceOrderDetailModel.AgentName = s.AgentName;
                            outsourceOrderDetailModel.Area = s.Area;
                            outsourceOrderDetailModel.BusinessModel = s.BusinessModel;
                            outsourceOrderDetailModel.Channel = s.Channel;
                            outsourceOrderDetailModel.ChooseImg = s.ChooseImg;
                            outsourceOrderDetailModel.City = s.City;
                            outsourceOrderDetailModel.CityTier = s.CityTier;
                            outsourceOrderDetailModel.Contact = s.Contact;
                            outsourceOrderDetailModel.CornerType = s.CornerType;
                            outsourceOrderDetailModel.Format = s.Format;
                            outsourceOrderDetailModel.Gender = string.Empty;
                            outsourceOrderDetailModel.GraphicLength = s.GraphicLength;
                            outsourceOrderDetailModel.OrderGraphicMaterial = s.GraphicMaterial;
                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                            outsourceOrderDetailModel.GraphicNo = s.GraphicNo;
                            outsourceOrderDetailModel.GraphicWidth = s.GraphicWidth;
                            outsourceOrderDetailModel.GuidanceId = s.GuidanceId;
                            outsourceOrderDetailModel.IsInstall = s.IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = s.BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = s.LocationType;
                            outsourceOrderDetailModel.MachineFrame = s.MachineFrame;
                            outsourceOrderDetailModel.MaterialSupport = s.MaterialSupport;
                            outsourceOrderDetailModel.OrderGender = s.OrderGender;
                            outsourceOrderDetailModel.OrderType = s.OrderType;
                            outsourceOrderDetailModel.POPAddress = s.POPAddress;
                            outsourceOrderDetailModel.POPName = s.POPName;
                            outsourceOrderDetailModel.POPType = s.POPType;
                            outsourceOrderDetailModel.PositionDescription = s.PositionDescription;
                            outsourceOrderDetailModel.POSScale = s.POSScale;
                            outsourceOrderDetailModel.Province = s.Province;
                            outsourceOrderDetailModel.Quantity = Quantity;
                            outsourceOrderDetailModel.Region = s.Region;
                            outsourceOrderDetailModel.Remark = s.Remark;
                            outsourceOrderDetailModel.Sheet = s.Sheet;
                            outsourceOrderDetailModel.ShopId = s.ShopId;
                            outsourceOrderDetailModel.ShopName = s.ShopName;
                            outsourceOrderDetailModel.ShopNo = s.ShopNo;
                            outsourceOrderDetailModel.ShopStatus = s.ShopStatus;
                            outsourceOrderDetailModel.SubjectId = s.SubjectId;
                            outsourceOrderDetailModel.Tel = s.Tel;
                            outsourceOrderDetailModel.TotalArea = s.TotalArea;
                            outsourceOrderDetailModel.WindowDeep = s.WindowDeep;
                            outsourceOrderDetailModel.WindowHigh = s.WindowHigh;
                            outsourceOrderDetailModel.WindowSize = s.WindowSize;
                            outsourceOrderDetailModel.WindowWide = s.WindowWide;
                            outsourceOrderDetailModel.ReceiveOrderPrice = s.OrderPrice;
                            outsourceOrderDetailModel.PayOrderPrice = s.PayOrderPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.InstallPriceMaterialSupport;
                            outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                            outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                            outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                            outsourceOrderDetailModel.CSUserId = s.CSUserId;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                            assignOrderCount++;

                        }
                        else
                        {
                            double GraphicLength = s.GraphicLength != null ? double.Parse(s.GraphicLength.ToString()) : 0;
                            double GraphicWidth = s.GraphicWidth != null ? double.Parse(s.GraphicWidth.ToString()) : 0;
                            bool canGo = true;
                            if (!string.IsNullOrWhiteSpace(s.GraphicNo) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                            {
                                string gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                //去掉重复的（同一个编号下多次的）
                                var checkList = savedList.Where(sl => sl.ShopId == s.ShopId && sl.Sheet == s.Sheet && sl.PositionDescription == s.PositionDescription && sl.GraphicNo == s.GraphicNo && sl.GraphicLength == GraphicLength && sl.GraphicWidth == GraphicWidth && sl.Gender == gender).ToList();

                                if (checkList.Any())
                                {
                                    canGo = false;
                                    repeatOrderCount++;
                                }
                            }
                            if (canGo)
                            {
                                Order350Model savedModel = new Order350Model();
                                savedModel.SubjectId = s.SubjectId ?? 0;
                                savedModel.ShopId = s.ShopId ?? 0;
                                savedModel.Sheet = s.Sheet;
                                savedModel.Gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                savedModel.PositionDescription = s.PositionDescription;
                                savedModel.GraphicNo = s.GraphicNo;
                                savedModel.GraphicLength = GraphicLength;
                                savedModel.GraphicWidth = GraphicWidth;
                                savedModel.GraphicMaterial = s.GraphicMaterial;
                                savedList.Add(savedModel);



                                string material0 = s.GraphicMaterial;
                                if (s.Province == "天津")
                                {
                                    if (material0.Contains("背胶PP+") && material0.Contains("雪弗板"))
                                    {
                                        string material1 = "背胶PP";

                                        int Quantity = s.Quantity ?? 1;
                                        if (!string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                        {
                                            Quantity = Quantity > 0 ? 1 : 0;
                                        }

                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        outsourceOrderDetailModel.AgentCode = s.AgentCode;
                                        outsourceOrderDetailModel.AgentName = s.AgentName;
                                        outsourceOrderDetailModel.Area = s.Area;
                                        outsourceOrderDetailModel.BusinessModel = s.BusinessModel;
                                        outsourceOrderDetailModel.Channel = s.Channel;
                                        outsourceOrderDetailModel.ChooseImg = s.ChooseImg;
                                        outsourceOrderDetailModel.City = s.City;
                                        outsourceOrderDetailModel.CityTier = s.CityTier;
                                        outsourceOrderDetailModel.Contact = s.Contact;
                                        outsourceOrderDetailModel.CornerType = s.CornerType;
                                        outsourceOrderDetailModel.Format = s.Format;
                                        outsourceOrderDetailModel.Gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                        outsourceOrderDetailModel.GraphicLength = s.GraphicLength;
                                        outsourceOrderDetailModel.OrderGraphicMaterial = s.GraphicMaterial;

                                        outsourceOrderDetailModel.GraphicMaterial = material1;
                                        outsourceOrderDetailModel.GraphicNo = s.GraphicNo;
                                        outsourceOrderDetailModel.GraphicWidth = s.GraphicWidth;
                                        outsourceOrderDetailModel.GuidanceId = s.GuidanceId;
                                        outsourceOrderDetailModel.IsInstall = s.IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = s.BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = s.LocationType;
                                        outsourceOrderDetailModel.MachineFrame = s.MachineFrame;
                                        outsourceOrderDetailModel.MaterialSupport = s.MaterialSupport;
                                        outsourceOrderDetailModel.OrderGender = s.OrderGender;
                                        outsourceOrderDetailModel.OrderType = s.OrderType;
                                        outsourceOrderDetailModel.POPAddress = s.POPAddress;
                                        outsourceOrderDetailModel.POPName = s.POPName;
                                        outsourceOrderDetailModel.POPType = s.POPType;
                                        outsourceOrderDetailModel.PositionDescription = s.PositionDescription;
                                        outsourceOrderDetailModel.POSScale = s.POSScale;
                                        outsourceOrderDetailModel.Province = s.Province;
                                        outsourceOrderDetailModel.Quantity = Quantity;
                                        outsourceOrderDetailModel.Region = s.Region;
                                        outsourceOrderDetailModel.Remark = s.Remark;
                                        outsourceOrderDetailModel.Sheet = s.Sheet;
                                        outsourceOrderDetailModel.ShopId = s.ShopId;
                                        outsourceOrderDetailModel.ShopName = s.ShopName;
                                        outsourceOrderDetailModel.ShopNo = s.ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = s.ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = s.SubjectId;
                                        outsourceOrderDetailModel.Tel = s.Tel;
                                        outsourceOrderDetailModel.TotalArea = s.TotalArea;
                                        outsourceOrderDetailModel.WindowDeep = s.WindowDeep;
                                        outsourceOrderDetailModel.WindowHigh = s.WindowHigh;
                                        outsourceOrderDetailModel.WindowSize = s.WindowSize;
                                        outsourceOrderDetailModel.WindowWide = s.WindowWide;
                                        outsourceOrderDetailModel.ReceiveOrderPrice = s.OrderPrice;
                                        outsourceOrderDetailModel.PayOrderPrice = s.PayOrderPrice;
                                        outsourceOrderDetailModel.InstallPriceMaterialSupport = s.InstallPriceMaterialSupport;
                                        decimal unitPrice = 0;
                                        decimal totalPrice = 0;
                                        if (!string.IsNullOrWhiteSpace(material1))
                                        {
                                            POP pop = new POP();
                                            pop.GraphicMaterial = material1;
                                            pop.GraphicLength = s.GraphicLength;
                                            pop.GraphicWidth = s.GraphicWidth;
                                            pop.Quantity = Quantity;
                                            pop.CustomerId = customerId;
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                            new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                            outsourceOrderDetailModel.UnitPrice = unitPrice;
                                            outsourceOrderDetailModel.TotalPrice = totalPrice;
                                        }
                                        outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                                        outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                                        outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                                        outsourceOrderDetailModel.CSUserId = s.CSUserId;
                                        outsourceOrderDetailModel.OutsourceId = 8;//北京卡乐
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                        material1 = "3mmPVC";

                                        int Quantity1 = s.Quantity ?? 1;
                                        if (!string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                        {
                                            Quantity1 = Quantity1 > 0 ? 1 : 0;
                                        }

                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        outsourceOrderDetailModel.AgentCode = s.AgentCode;
                                        outsourceOrderDetailModel.AgentName = s.AgentName;
                                        outsourceOrderDetailModel.Area = s.Area;
                                        outsourceOrderDetailModel.BusinessModel = s.BusinessModel;
                                        outsourceOrderDetailModel.Channel = s.Channel;
                                        outsourceOrderDetailModel.ChooseImg = s.ChooseImg;
                                        outsourceOrderDetailModel.City = s.City;
                                        outsourceOrderDetailModel.CityTier = s.CityTier;
                                        outsourceOrderDetailModel.Contact = s.Contact;
                                        outsourceOrderDetailModel.CornerType = s.CornerType;
                                        outsourceOrderDetailModel.Format = s.Format;
                                        outsourceOrderDetailModel.Gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                        outsourceOrderDetailModel.GraphicLength = s.GraphicLength;
                                        outsourceOrderDetailModel.OrderGraphicMaterial = s.GraphicMaterial;

                                        outsourceOrderDetailModel.GraphicMaterial = material1;
                                        outsourceOrderDetailModel.GraphicNo = s.GraphicNo;
                                        outsourceOrderDetailModel.GraphicWidth = s.GraphicWidth;
                                        outsourceOrderDetailModel.GuidanceId = s.GuidanceId;
                                        outsourceOrderDetailModel.IsInstall = s.IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = s.BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = s.LocationType;
                                        outsourceOrderDetailModel.MachineFrame = s.MachineFrame;
                                        outsourceOrderDetailModel.MaterialSupport = s.MaterialSupport;
                                        outsourceOrderDetailModel.OrderGender = s.OrderGender;
                                        outsourceOrderDetailModel.OrderType = s.OrderType;
                                        outsourceOrderDetailModel.POPAddress = s.POPAddress;
                                        outsourceOrderDetailModel.POPName = s.POPName;
                                        outsourceOrderDetailModel.POPType = s.POPType;
                                        outsourceOrderDetailModel.PositionDescription = s.PositionDescription;
                                        outsourceOrderDetailModel.POSScale = s.POSScale;
                                        outsourceOrderDetailModel.Province = s.Province;
                                        outsourceOrderDetailModel.Quantity = Quantity1;
                                        outsourceOrderDetailModel.Region = s.Region;
                                        outsourceOrderDetailModel.Remark = s.Remark;
                                        outsourceOrderDetailModel.Sheet = s.Sheet;
                                        outsourceOrderDetailModel.ShopId = s.ShopId;
                                        outsourceOrderDetailModel.ShopName = s.ShopName;
                                        outsourceOrderDetailModel.ShopNo = s.ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = s.ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = s.SubjectId;
                                        outsourceOrderDetailModel.Tel = s.Tel;
                                        outsourceOrderDetailModel.TotalArea = s.TotalArea;
                                        outsourceOrderDetailModel.WindowDeep = s.WindowDeep;
                                        outsourceOrderDetailModel.WindowHigh = s.WindowHigh;
                                        outsourceOrderDetailModel.WindowSize = s.WindowSize;
                                        outsourceOrderDetailModel.WindowWide = s.WindowWide;
                                        outsourceOrderDetailModel.ReceiveOrderPrice = s.OrderPrice;
                                        outsourceOrderDetailModel.PayOrderPrice = s.PayOrderPrice;
                                        outsourceOrderDetailModel.InstallPriceMaterialSupport = s.InstallPriceMaterialSupport;
                                        decimal unitPrice1 = 0;
                                        decimal totalPrice1 = 0;
                                        if (!string.IsNullOrWhiteSpace(material1))
                                        {
                                            POP pop = new POP();
                                            pop.GraphicMaterial = material1;
                                            pop.GraphicLength = s.GraphicLength;
                                            pop.GraphicWidth = s.GraphicWidth;
                                            pop.Quantity = Quantity1;
                                            pop.CustomerId = customerId;
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                            new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                            outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                            outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                        }
                                        outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                                        outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                                        outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                                        outsourceOrderDetailModel.CSUserId = s.CSUserId;
                                        outsourceOrderDetailModel.OutsourceId = outsourceId;
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                        addInstallPrice = true;

                                        if (extraInstallPriceSubjectId == 0)
                                            extraInstallPriceSubjectId = s.SubjectId ?? 0;
                                    }
                                    else
                                    {

                                        int Quantity = s.Quantity ?? 1;
                                        if (!string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                        {
                                            Quantity = Quantity > 0 ? 1 : 0;
                                        }

                                        outsourceOrderDetailModel = new OutsourceOrderDetail();

                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        outsourceOrderDetailModel.AgentCode = s.AgentCode;
                                        outsourceOrderDetailModel.AgentName = s.AgentName;
                                        outsourceOrderDetailModel.Area = s.Area;
                                        outsourceOrderDetailModel.BusinessModel = s.BusinessModel;
                                        outsourceOrderDetailModel.Channel = s.Channel;
                                        outsourceOrderDetailModel.ChooseImg = s.ChooseImg;
                                        outsourceOrderDetailModel.City = s.City;
                                        outsourceOrderDetailModel.CityTier = s.CityTier;
                                        outsourceOrderDetailModel.Contact = s.Contact;
                                        outsourceOrderDetailModel.CornerType = s.CornerType;
                                        outsourceOrderDetailModel.Format = s.Format;
                                        outsourceOrderDetailModel.Gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                        outsourceOrderDetailModel.GraphicLength = s.GraphicLength;
                                        outsourceOrderDetailModel.OrderGraphicMaterial = s.GraphicMaterial;
                                        string material = string.Empty;
                                        if (!string.IsNullOrWhiteSpace(material0))
                                            material = new BasePage().GetBasicMaterial(material0);
                                        if (string.IsNullOrWhiteSpace(material))
                                            material = s.GraphicMaterial;
                                        outsourceOrderDetailModel.GraphicMaterial = material;
                                        outsourceOrderDetailModel.GraphicNo = s.GraphicNo;
                                        outsourceOrderDetailModel.GraphicWidth = s.GraphicWidth;
                                        outsourceOrderDetailModel.GuidanceId = s.GuidanceId;
                                        outsourceOrderDetailModel.IsInstall = s.IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = s.BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = s.LocationType;
                                        outsourceOrderDetailModel.MachineFrame = s.MachineFrame;
                                        outsourceOrderDetailModel.MaterialSupport = s.MaterialSupport;
                                        outsourceOrderDetailModel.OrderGender = s.OrderGender;
                                        outsourceOrderDetailModel.OrderType = s.OrderType;
                                        outsourceOrderDetailModel.POPAddress = s.POPAddress;
                                        outsourceOrderDetailModel.POPName = s.POPName;
                                        outsourceOrderDetailModel.POPType = s.POPType;
                                        outsourceOrderDetailModel.PositionDescription = s.PositionDescription;
                                        outsourceOrderDetailModel.POSScale = s.POSScale;
                                        outsourceOrderDetailModel.Province = s.Province;
                                        outsourceOrderDetailModel.Quantity = Quantity;
                                        outsourceOrderDetailModel.Region = s.Region;
                                        outsourceOrderDetailModel.Remark = s.Remark;
                                        outsourceOrderDetailModel.Sheet = s.Sheet;
                                        outsourceOrderDetailModel.ShopId = s.ShopId;
                                        outsourceOrderDetailModel.ShopName = s.ShopName;
                                        outsourceOrderDetailModel.ShopNo = s.ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = s.ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = s.SubjectId;
                                        outsourceOrderDetailModel.Tel = s.Tel;
                                        outsourceOrderDetailModel.TotalArea = s.TotalArea;
                                        outsourceOrderDetailModel.WindowDeep = s.WindowDeep;
                                        outsourceOrderDetailModel.WindowHigh = s.WindowHigh;
                                        outsourceOrderDetailModel.WindowSize = s.WindowSize;
                                        outsourceOrderDetailModel.WindowWide = s.WindowWide;
                                        outsourceOrderDetailModel.ReceiveOrderPrice = s.OrderPrice;
                                        outsourceOrderDetailModel.PayOrderPrice = s.PayOrderPrice;
                                        outsourceOrderDetailModel.InstallPriceMaterialSupport = s.InstallPriceMaterialSupport;
                                        decimal unitPrice = 0;
                                        decimal totalPrice = 0;
                                        if (!string.IsNullOrWhiteSpace(material))
                                        {
                                            POP pop = new POP();
                                            pop.GraphicMaterial = material;
                                            pop.GraphicLength = s.GraphicLength;
                                            pop.GraphicWidth = s.GraphicWidth;
                                            pop.Quantity = Quantity;
                                            pop.CustomerId = customerId;
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                            new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                            outsourceOrderDetailModel.UnitPrice = unitPrice;
                                            outsourceOrderDetailModel.TotalPrice = totalPrice;
                                        }
                                        outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                                        outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                                        outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                                        outsourceOrderDetailModel.CSUserId = s.CSUserId;
                                        outsourceOrderDetailModel.OutsourceId = 8;
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                        addInstallPrice = true;

                                    }

                                }
                                else
                                {

                                    int Quantity = s.Quantity ?? 1;
                                    if (!string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                    {
                                        Quantity = Quantity > 0 ? 1 : 0;
                                    }

                                    outsourceOrderDetailModel = new OutsourceOrderDetail();

                                    outsourceOrderDetailModel.AddDate = DateTime.Now;
                                    outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    outsourceOrderDetailModel.AgentCode = s.AgentCode;
                                    outsourceOrderDetailModel.AgentName = s.AgentName;
                                    outsourceOrderDetailModel.Area = s.Area;
                                    outsourceOrderDetailModel.BusinessModel = s.BusinessModel;
                                    outsourceOrderDetailModel.Channel = s.Channel;
                                    outsourceOrderDetailModel.ChooseImg = s.ChooseImg;
                                    outsourceOrderDetailModel.City = s.City;
                                    outsourceOrderDetailModel.CityTier = s.CityTier;
                                    outsourceOrderDetailModel.Contact = s.Contact;
                                    outsourceOrderDetailModel.CornerType = s.CornerType;
                                    outsourceOrderDetailModel.Format = s.Format;
                                    outsourceOrderDetailModel.Gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                    outsourceOrderDetailModel.GraphicLength = s.GraphicLength;
                                    outsourceOrderDetailModel.OrderGraphicMaterial = s.GraphicMaterial;
                                    string material = string.Empty;
                                    if (!string.IsNullOrWhiteSpace(material0))
                                        material = new BasePage().GetBasicMaterial(material0);
                                    if (string.IsNullOrWhiteSpace(material))
                                        material = s.GraphicMaterial;
                                    outsourceOrderDetailModel.GraphicMaterial = material;
                                    outsourceOrderDetailModel.GraphicNo = s.GraphicNo;
                                    outsourceOrderDetailModel.GraphicWidth = s.GraphicWidth;
                                    outsourceOrderDetailModel.GuidanceId = s.GuidanceId;
                                    outsourceOrderDetailModel.IsInstall = s.IsInstall;
                                    outsourceOrderDetailModel.BCSIsInstall = s.BCSIsInstall;
                                    outsourceOrderDetailModel.LocationType = s.LocationType;
                                    outsourceOrderDetailModel.MachineFrame = s.MachineFrame;
                                    outsourceOrderDetailModel.MaterialSupport = s.MaterialSupport;
                                    outsourceOrderDetailModel.OrderGender = s.OrderGender;
                                    outsourceOrderDetailModel.OrderType = s.OrderType;
                                    outsourceOrderDetailModel.POPAddress = s.POPAddress;
                                    outsourceOrderDetailModel.POPName = s.POPName;
                                    outsourceOrderDetailModel.POPType = s.POPType;
                                    outsourceOrderDetailModel.PositionDescription = s.PositionDescription;
                                    outsourceOrderDetailModel.POSScale = s.POSScale;
                                    outsourceOrderDetailModel.Province = s.Province;
                                    outsourceOrderDetailModel.Quantity = Quantity;
                                    outsourceOrderDetailModel.Region = s.Region;
                                    outsourceOrderDetailModel.Remark = s.Remark;
                                    outsourceOrderDetailModel.Sheet = s.Sheet;
                                    outsourceOrderDetailModel.ShopId = s.ShopId;
                                    outsourceOrderDetailModel.ShopName = s.ShopName;
                                    outsourceOrderDetailModel.ShopNo = s.ShopNo;
                                    outsourceOrderDetailModel.ShopStatus = s.ShopStatus;
                                    outsourceOrderDetailModel.SubjectId = s.SubjectId;
                                    outsourceOrderDetailModel.Tel = s.Tel;
                                    outsourceOrderDetailModel.TotalArea = s.TotalArea;
                                    outsourceOrderDetailModel.WindowDeep = s.WindowDeep;
                                    outsourceOrderDetailModel.WindowHigh = s.WindowHigh;
                                    outsourceOrderDetailModel.WindowSize = s.WindowSize;
                                    outsourceOrderDetailModel.WindowWide = s.WindowWide;
                                    outsourceOrderDetailModel.ReceiveOrderPrice = s.OrderPrice;
                                    outsourceOrderDetailModel.PayOrderPrice = s.PayOrderPrice;
                                    outsourceOrderDetailModel.InstallPriceMaterialSupport = s.InstallPriceMaterialSupport;
                                    decimal unitPrice = 0;
                                    decimal totalPrice = 0;
                                    if (!string.IsNullOrWhiteSpace(material))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = material;
                                        pop.GraphicLength = s.GraphicLength;
                                        pop.GraphicWidth = s.GraphicWidth;
                                        pop.Quantity = Quantity;
                                        pop.CustomerId = customerId;
                                        pop.OutsourceType = assignType;
                                        new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                        outsourceOrderDetailModel.UnitPrice = unitPrice;
                                        outsourceOrderDetailModel.TotalPrice = totalPrice;
                                    }
                                    outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                                    outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                                    outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                                    outsourceOrderDetailModel.CSUserId = s.CSUserId;
                                    outsourceOrderDetailModel.OutsourceId = outsourceId;
                                    outsourceOrderDetailModel.AssignType = assignType;
                                    if (material0.Contains("软膜") && !s.Province.Contains("吉林"))
                                    {
                                        //软膜默认北京生产
                                        outsourceOrderDetailModel.OutsourceId = 8;
                                        if (s.Province == "北京")
                                        {
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                        }
                                        else
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                    }
                                    if (material0.Contains("即时贴") && s.Province.Contains("内蒙古") && !s.City.Contains("通辽"))
                                    {
                                        //内蒙古即时贴默认北京生产
                                        outsourceOrderDetailModel.OutsourceId = 8;
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                    }
                                    if (material0.Contains("网格布") && !string.IsNullOrWhiteSpace(s.Channel) && s.Channel.ToLower().Contains("terrex"))
                                    {
                                        //户外店的网格布默认北京生产
                                        outsourceOrderDetailModel.OutsourceId = 8;
                                        if (s.Province == "北京")
                                        {
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                        }
                                        else
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                    }

                                    outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                }
                                hasPOP = true;
                                assignOrderCount++;
                            }
                        }

                    });
                    #endregion

                    #region 安装费和快递费
                    if (hasPOP)
                    {
                        if (guidanceType == (int)GuidanceTypeEnum.Install && assignType == (int)OutsourceOrderTypeEnum.Install)
                        {
                            addInstallPrice = true;
                        }
                        if (addInstallPrice && !hasInstallPrice && assignType == (int)OutsourceOrderTypeEnum.Install)
                        {
                            //hasExtraInstallPrice = true;
                            //添加安装费
                            decimal receiveInstallPrice = 0;
                            decimal installPrice = 0;
                            var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == guidanceId && sh.ShopId == shopId);
                            if (installShopList.Any())
                            {
                                installShopList.ForEach(sh =>
                                {
                                    receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                                });
                            }

                            if ((assignOrderModel.Shop.OutsourceInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = assignOrderModel.Shop.OutsourceInstallPrice ?? 0;
                            }
                            if (assignOrderModel.SubjectCornerType == "三叶草")
                            {
                                if ((assignOrderModel.Shop.OutsourceBCSInstallPrice ?? 0) > 0)
                                {
                                    basicInstallPrice = assignOrderModel.Shop.OutsourceBCSInstallPrice ?? 0;
                                }
                                else if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                                {
                                    basicInstallPrice = 150;
                                }
                                else
                                {
                                    basicInstallPrice = 0;
                                }

                            }
                            if (assignOrderModel.SubjectCategoryName.Contains("常规-非活动"))
                            {
                                //if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                                //{
                                //    basicInstallPrice = 150;
                                //}
                                //else
                                //    basicInstallPrice = 0;
                                if (assignOrderModel.Shop.CityName == "包头市" && (assignOrderModel.Shop.OutsourceInstallPrice ?? 0) > 0)
                                {
                                    basicInstallPrice = assignOrderModel.Shop.OutsourceInstallPrice ?? 0;
                                }
                                else if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                                {
                                    basicInstallPrice = 150;
                                }
                                else
                                {
                                    basicInstallPrice = 0;
                                }
                            }
                            installPrice = oohInstallPrice + basicInstallPrice;

                            if (installPrice > 0)
                            {

                                if (oohInstallPrice > 0 && (assignOrderModel.Shop.OOHInstallOutsourceId ?? 0) > 0)
                                {
                                    //如果有单独的户外安装外协
                                    outsourceOrderDetailModel = new OutsourceOrderDetail();
                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                    outsourceOrderDetailModel.AddDate = DateTime.Now;
                                    outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].AgentCode;
                                    outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].AgentName;
                                    outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].BusinessModel;
                                    outsourceOrderDetailModel.Channel = OutsourceOrderList[0].Channel;
                                    outsourceOrderDetailModel.City = OutsourceOrderList[0].City;
                                    outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].CityTier;
                                    outsourceOrderDetailModel.Contact = OutsourceOrderList[0].Contact;
                                    outsourceOrderDetailModel.Format = OutsourceOrderList[0].Format;
                                    outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                    outsourceOrderDetailModel.GraphicNo = string.Empty;
                                    outsourceOrderDetailModel.GraphicWidth = 0;
                                    outsourceOrderDetailModel.GuidanceId = guidanceId;
                                    outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].IsInstall;
                                    outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].BCSIsInstall;
                                    outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].LocationType;
                                    outsourceOrderDetailModel.MachineFrame = string.Empty;
                                    outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                    outsourceOrderDetailModel.OrderGender = string.Empty;
                                    outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                    outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].POPAddress;
                                    outsourceOrderDetailModel.POPName = string.Empty;
                                    outsourceOrderDetailModel.POPType = string.Empty;
                                    outsourceOrderDetailModel.PositionDescription = string.Empty;
                                    outsourceOrderDetailModel.POSScale = posScale;
                                    outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                                    outsourceOrderDetailModel.Quantity = 1;
                                    outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                                    outsourceOrderDetailModel.Remark = string.Empty;
                                    outsourceOrderDetailModel.Sheet = string.Empty;
                                    outsourceOrderDetailModel.ShopId = shopId;
                                    outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].ShopName;
                                    outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].ShopNo;
                                    outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].ShopStatus;
                                    outsourceOrderDetailModel.SubjectId = OutsourceOrderList[0].SubjectId;
                                    outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                                    outsourceOrderDetailModel.TotalArea = 0;
                                    outsourceOrderDetailModel.WindowDeep = 0;
                                    outsourceOrderDetailModel.WindowHigh = 0;
                                    outsourceOrderDetailModel.WindowSize = string.Empty;
                                    outsourceOrderDetailModel.WindowWide = 0;
                                    outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                    outsourceOrderDetailModel.PayOrderPrice = oohInstallPrice;
                                    outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                    outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                    outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                    outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                    outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                    outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].CSUserId;
                                    outsourceOrderDetailModel.OutsourceId = assignOrderModel.Shop.OOHInstallOutsourceId;
                                    outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                    installPrice = installPrice - oohInstallPrice;
                                    oohInstallPrice = 0;
                                }
                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].AgentCode;
                                outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].AgentName;
                                outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].BusinessModel;
                                outsourceOrderDetailModel.Channel = OutsourceOrderList[0].Channel;
                                outsourceOrderDetailModel.City = OutsourceOrderList[0].City;
                                outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].CityTier;
                                outsourceOrderDetailModel.Contact = OutsourceOrderList[0].Contact;
                                outsourceOrderDetailModel.Format = OutsourceOrderList[0].Format;
                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                outsourceOrderDetailModel.GraphicWidth = 0;
                                outsourceOrderDetailModel.GuidanceId = guidanceId;
                                outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].IsInstall;
                                outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].BCSIsInstall;
                                outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].LocationType;
                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].POPAddress;
                                outsourceOrderDetailModel.POPName = string.Empty;
                                outsourceOrderDetailModel.POPType = string.Empty;
                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                outsourceOrderDetailModel.POSScale = posScale;
                                outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                                outsourceOrderDetailModel.Quantity = 1;
                                outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                                outsourceOrderDetailModel.Remark = string.Empty;
                                outsourceOrderDetailModel.Sheet = string.Empty;
                                outsourceOrderDetailModel.ShopId = shopId;
                                outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].ShopName;
                                outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].ShopNo;
                                outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].ShopStatus;
                                outsourceOrderDetailModel.SubjectId = OutsourceOrderList[0].SubjectId;
                                outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                                outsourceOrderDetailModel.TotalArea = 0;
                                outsourceOrderDetailModel.WindowDeep = 0;
                                outsourceOrderDetailModel.WindowHigh = 0;
                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                outsourceOrderDetailModel.WindowWide = 0;
                                outsourceOrderDetailModel.ReceiveOrderPrice = receiveInstallPrice;
                                outsourceOrderDetailModel.PayOrderPrice = installPrice;
                                outsourceOrderDetailModel.PayBasicInstallPrice = basicInstallPrice;
                                outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].CSUserId;
                                outsourceOrderDetailModel.OutsourceId = outsourceId;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                            }
                        }
                        else if (guidanceType == (int)GuidanceTypeEnum.Promotion && assignOrderModel.HasExpressPrice)
                        {
                            //快递费
                            decimal rExpressPrice = 0;
                            decimal payExpressPrice = 0;
                            if (expressPriceDetailModel != null && (expressPriceDetailModel.ExpressPrice ?? 0) > 0)
                            {
                                rExpressPrice = expressPriceDetailModel.ExpressPrice ?? 0;
                            }
                            else
                                rExpressPrice = 35;

                            ExpressPriceConfig eM = expressPriceConfigList.Where(price => price.ReceivePrice == rExpressPrice).FirstOrDefault();
                            if (eM != null)
                                payExpressPrice = eM.PayPrice ?? 0;
                            else
                                payExpressPrice = 22;
                            if (assignOrderModel.Shop.ProvinceName == "内蒙古" && !assignOrderModel.Shop.CityName.Contains("通辽"))
                            {
                                payExpressPrice = 0;
                            }
                            if (payExpressPrice > 0)
                            {
                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].AgentCode;
                                outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].AgentName;
                                outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].BusinessModel;
                                outsourceOrderDetailModel.Channel = OutsourceOrderList[0].Channel;
                                outsourceOrderDetailModel.City = OutsourceOrderList[0].City;
                                outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].CityTier;
                                outsourceOrderDetailModel.Contact = OutsourceOrderList[0].Contact;
                                outsourceOrderDetailModel.Format = OutsourceOrderList[0].Format;
                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                outsourceOrderDetailModel.GraphicWidth = 0;
                                outsourceOrderDetailModel.GuidanceId = guidanceId;
                                outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].IsInstall;
                                outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].BCSIsInstall;
                                outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].LocationType;
                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.发货费;
                                outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].POPAddress;
                                outsourceOrderDetailModel.POPName = string.Empty;
                                outsourceOrderDetailModel.POPType = string.Empty;
                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                outsourceOrderDetailModel.POSScale = posScale;
                                outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                                outsourceOrderDetailModel.Quantity = 1;
                                outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                                outsourceOrderDetailModel.Remark = string.Empty;
                                outsourceOrderDetailModel.Sheet = string.Empty;
                                outsourceOrderDetailModel.ShopId = shopId;
                                outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].ShopName;
                                outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].ShopNo;
                                outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].ShopStatus;
                                outsourceOrderDetailModel.SubjectId = extraInstallPriceSubjectId;
                                outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                                outsourceOrderDetailModel.TotalArea = 0;
                                outsourceOrderDetailModel.WindowDeep = 0;
                                outsourceOrderDetailModel.WindowHigh = 0;
                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                outsourceOrderDetailModel.WindowWide = 0;
                                outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                outsourceOrderDetailModel.PayOrderPrice = 0;
                                outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                outsourceOrderDetailModel.PayOOHInstallPrice = 0;
                                outsourceOrderDetailModel.PayExpressPrice = payExpressPrice;
                                outsourceOrderDetailModel.ReceiveExpresslPrice = rExpressPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].CSUserId;
                                outsourceOrderDetailModel.OutsourceId = outsourceId;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        //int orderId = 0;
        void SaveAssignOrder(OutsourceAssignOrderModel assignOrderModel)
        {

            //hasExtraInstallPrice = false;
            string materialSupport = string.Empty;
            string posScale = string.Empty;
            if (assignOrderModel != null && assignOrderModel.Shop != null)
            {
                decimal promotionInstallPrice = 0;
                int customerId = assignOrderModel.CustomerId;
                int outsourceId = assignOrderModel.Shop.OutsourceId ?? 0;
                int guidanceId = assignOrderModel.GuidanceId;
                int guidanceType = assignOrderModel.GuidanceType;
                int shopId = assignOrderModel.Shop.Id;
                int assignType = assignOrderModel.AssignType;
                var OutsourceOrderList = assignOrderModel.OrderList;
                int extraInstallPriceSubjectId = 0;
                
                //去重
                List<FinalOrderDetailTemp> tempOrderList = new List<FinalOrderDetailTemp>(); ;
                OutsourceOrderList.ForEach(s => {
                    bool canGo = true;
                    if (!string.IsNullOrWhiteSpace(s.GraphicNo) && !string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                    {
                        //string gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                        //去掉重复的（同一个编号下多次的）
                        var checkList = tempOrderList.Where(sl => sl.Sheet == s.Sheet && sl.PositionDescription == s.PositionDescription && sl.GraphicNo == s.GraphicNo && sl.GraphicLength == s.GraphicLength && sl.GraphicWidth == s.GraphicWidth && (sl.Gender==s.Gender || sl.OrderGender==s.OrderGender)).ToList();

                        if (checkList.Any())
                        {
                            canGo = false;
                        }
                    }
                    if (canGo)
                    {
                        tempOrderList.Add(s);
                    }
                });
                OutsourceOrderList = tempOrderList;
                if (OutsourceOrderList.Any())
                {
                    if (assignOrderModel.Shop.ProvinceName.Contains("内蒙古") && !assignOrderModel.Shop.CityName.Contains("通辽"))
                    {
                        assignType = (int)OutsourceOrderTypeEnum.Install;
                    }
                    int index = 0;
                    materialSupport = OutsourceOrderList[index].InstallPriceMaterialSupport;
                    posScale = OutsourceOrderList[index].POSScale;
                    while (string.IsNullOrWhiteSpace(materialSupport))
                    {
                        index++;
                        if (index == OutsourceOrderList.Count)
                            break;
                        materialSupport = OutsourceOrderList[index].InstallPriceMaterialSupport;
                    }
                    bool addInstallPrice = false;
                    bool hasInstallPrice = false;
                    decimal basicInstallPrice = 0;
                    decimal oohInstallPrice = 0;
                    #region 订单明细
                    if (!assignOrderModel.Shop.ProvinceName.Contains("北京"))
                    {
                        #region 非北京订单
                        List<int> assignedOrderIdList = new List<int>();
                        //按材质分
                        var materialConfig = configList.Where(s => s.ConfigTypeId == (int)OutsourceOrderConfigType.Material).ToList();
                        if (materialConfig.Any())
                        {
                            
                            materialConfig.ForEach(config =>
                            {
                                OutsourceOrderPlaceConfigBLL placeConfigBll = new OutsourceOrderPlaceConfigBLL();
                                if (!string.IsNullOrWhiteSpace(config.MaterialName))
                                {
                                    var orderList = OutsourceOrderList.Where(order => !assignedOrderIdList.Contains(order.Id) && order.GraphicMaterial != null && order.GraphicMaterial.ToLower().Contains(config.MaterialName.ToLower()) &&  (order.OrderType == (int)OrderTypeEnum.POP || order.OrderType == (int)OrderTypeEnum.道具)).ToList();
                                   
                                    List<int> cityIdList = new List<int>();
                                   
                                    List<string> cityNameList = new List<string>();
                                    bool flag = false;
                                    var placeConfigList = (from placeConfin in CurrentContext.DbContext.OutsourceOrderPlaceConfig
                                                          join place in CurrentContext.DbContext.Place
                                                          on placeConfin.PrivinceId equals place.ID
                                                           where placeConfin.ConfigId == config.Id
                                                           select new {
                                                              placeConfin.CityIds,
                                                              place.PlaceName
                                                          }).ToList();
                                    if (placeConfigList.Any())
                                    {
                                        placeConfigList.ForEach(pc => {
                                            if (pc.PlaceName == assignOrderModel.Shop.ProvinceName)
                                            {
                                                orderList = orderList.Where(order => order.Province == pc.PlaceName).ToList();
                                                if (!string.IsNullOrWhiteSpace(pc.CityIds))
                                                {
                                                    cityIdList = StringHelper.ToIntList(pc.CityIds, ',');
                                                    cityNameList = placeList.Where(p => cityIdList.Contains(p.ID)).Select(p => p.PlaceName).ToList();
                                                    orderList = orderList.Where(order => cityNameList.Contains(order.City)).ToList();
                                                }
                                                flag = true;
                                                
                                            }
                                        });
                                    }

                                    if (!flag)
                                        orderList = new List<FinalOrderDetailTemp>();
                                    if (!string.IsNullOrWhiteSpace(config.Channel))
                                    {
                                        List<string> channelList = StringHelper.ToStringList(config.Channel, ',', LowerUpperEnum.ToLower);
                                        if (channelList.Any())
                                        {
                                            orderList = orderList.Where(order => order.Channel != null && channelList.Contains(order.Channel.ToLower())).ToList();
                                        }
                                    }
                                    if (orderList.Any())
                                    {
                                        orderList.ForEach(order =>
                                        {
                                            string material0 = order.GraphicMaterial;
                                            int Quantity = order.Quantity ?? 1;
                                            if (!string.IsNullOrWhiteSpace(order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(order.Sheet.ToUpper()))
                                            {
                                                Quantity = Quantity > 0 ? 1 : 0;
                                            }
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = order.AgentName;
                                            outsourceOrderDetailModel.Area = order.Area;
                                            outsourceOrderDetailModel.BusinessModel = order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = order.ChooseImg;
                                            outsourceOrderDetailModel.City = order.City;
                                            outsourceOrderDetailModel.CityTier = order.CityTier;
                                            outsourceOrderDetailModel.Contact = order.Contact;
                                            outsourceOrderDetailModel.CornerType = order.CornerType;
                                            outsourceOrderDetailModel.Format = order.Format;
                                            outsourceOrderDetailModel.Gender = (order.OrderGender != null && order.OrderGender != "") ? order.OrderGender : order.Gender;
                                            outsourceOrderDetailModel.GraphicLength = order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = order.GraphicMaterial;
                                            string material = string.Empty;
                                            if (!string.IsNullOrWhiteSpace(material0))
                                                material = new BasePage().GetBasicMaterial(material0);
                                            if (string.IsNullOrWhiteSpace(material))
                                                material = order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material;
                                            outsourceOrderDetailModel.GraphicNo = order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = order.POPAddress;
                                            outsourceOrderDetailModel.POPName = order.POPName;
                                            outsourceOrderDetailModel.POPType = order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = order.POSScale;
                                            outsourceOrderDetailModel.Province = order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = order.Region;
                                            outsourceOrderDetailModel.Remark = order.Remark;
                                            outsourceOrderDetailModel.Sheet = order.Sheet;
                                            outsourceOrderDetailModel.ShopId = order.ShopId;
                                            outsourceOrderDetailModel.ShopName = order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = order.SubjectId;
                                            outsourceOrderDetailModel.Tel = order.Tel;
                                            outsourceOrderDetailModel.TotalArea = order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = order.InstallPriceMaterialSupport;
                                            decimal unitPrice = 0;
                                            decimal totalPrice = 0;
                                            if (!string.IsNullOrWhiteSpace(material))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material;
                                                pop.GraphicLength = order.GraphicLength;
                                                pop.GraphicWidth = order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = customerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = order.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = order.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = config.ProductOutsourctId;
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            outsourceOrderDetailModel.FinalOrderId =order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                            assignedOrderIdList.Add(order.Id);
                                            assignOrderCount++;
                                        });
                                    }
                                }
                            });
                        }
                        //
                        //var orderList1 = OutsourceOrderList.Where(order => !assignedOrderIdList.Contains(order.Id)).ToList();
                        var orderList1 = (from order in OutsourceOrderList
                                         join subject in CurrentContext.DbContext.Subject
                                         on order.SubjectId equals subject.Id
                                         where !assignedOrderIdList.Contains(order.Id)
                                         select new {
                                             order,
                                             subject

                                         }).ToList();
                        if (orderList1.Any())
                        {
                            orderList1.ForEach(s =>
                            {

                               
                                string material0 = s.order.GraphicMaterial??"";
                                if (s.order.Province == "天津")
                                {
                                    if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)//促销
                                    {
                                        
                                        int Quantity = s.order.Quantity ?? 1;
                                        if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                        {
                                            Quantity = Quantity > 0 ? 1 : 0;
                                        }

                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                        outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                        outsourceOrderDetailModel.Area = s.order.Area;
                                        outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                        outsourceOrderDetailModel.Channel = s.order.Channel;
                                        outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                        outsourceOrderDetailModel.City = s.order.City;
                                        outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                        outsourceOrderDetailModel.Contact = s.order.Contact;
                                        outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                        outsourceOrderDetailModel.Format = s.order.Format;
                                        outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                        outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                        outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                        string material = string.Empty;
                                        if (!string.IsNullOrWhiteSpace(material0))
                                            material = new BasePage().GetBasicMaterial(material0);
                                        if (string.IsNullOrWhiteSpace(material))
                                            material = s.order.GraphicMaterial;
                                        outsourceOrderDetailModel.GraphicMaterial = material;
                                        outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                        outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                        outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                        outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                        outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                        outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                        outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                        outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                        outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                        outsourceOrderDetailModel.POPName = s.order.POPName;
                                        outsourceOrderDetailModel.POPType = s.order.POPType;
                                        outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                        outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                        outsourceOrderDetailModel.Province = s.order.Province;
                                        outsourceOrderDetailModel.Quantity = Quantity;
                                        outsourceOrderDetailModel.Region = s.order.Region;
                                        outsourceOrderDetailModel.Remark = s.order.Remark;
                                        outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                        outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                        outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                        outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                        outsourceOrderDetailModel.Tel = s.order.Tel;
                                        outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                        outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                        outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                        outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                        outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                        outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                        outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                        outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                        decimal unitPrice = 0;
                                        decimal totalPrice = 0;
                                        if (!string.IsNullOrWhiteSpace(material))
                                        {
                                            POP pop = new POP();
                                            pop.GraphicMaterial = material0;
                                            pop.GraphicLength = s.order.GraphicLength;
                                            pop.GraphicWidth = s.order.GraphicWidth;
                                            pop.Quantity = Quantity;
                                            pop.CustomerId = customerId;
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                            new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                            outsourceOrderDetailModel.UnitPrice = unitPrice;
                                            outsourceOrderDetailModel.TotalPrice = totalPrice;
                                        }
                                        outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                        outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                        outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                        outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                        outsourceOrderDetailModel.OutsourceId = calerOutsourceId;//北京卡乐
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.order.Sheet == "橱窗" && s.order.GraphicLength > 1 && s.order.GraphicWidth > 1 && material0.Contains("全透贴"))
                                        {
                                           
                                            promotionInstallPrice = 150;
                                           
                                        }
                                        outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                       


                                    }
                                    else
                                    {
                                        string material = string.Empty;
                                        if (!string.IsNullOrWhiteSpace(material0))
                                            material = new BasePage().GetBasicMaterial(material0);
                                        if (string.IsNullOrWhiteSpace(material))
                                            material = s.order.GraphicMaterial;
                                        if (material.Contains("背胶PP+") && material.Contains("雪弗板") && !material.Contains("蝴蝶支架"))
                                        {
                                            string material1 = "背胶PP";

                                            int Quantity = s.order.Quantity ?? 1;
                                            if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                            {
                                                Quantity = Quantity > 0 ? 1 : 0;
                                            }

                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                            outsourceOrderDetailModel.Area = s.order.Area;
                                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = s.order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                            outsourceOrderDetailModel.City = s.order.City;
                                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                            outsourceOrderDetailModel.Contact = s.order.Contact;
                                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                            outsourceOrderDetailModel.Format = s.order.Format;
                                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material1;
                                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                            outsourceOrderDetailModel.POPName = s.order.POPName;
                                            outsourceOrderDetailModel.POPType = s.order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                            outsourceOrderDetailModel.Province = s.order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = s.order.Region;
                                            outsourceOrderDetailModel.Remark = s.order.Remark + ",天津裱板";
                                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                            outsourceOrderDetailModel.Tel = s.order.Tel;
                                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                            decimal unitPrice = 0;
                                            decimal totalPrice = 0;
                                            if (!string.IsNullOrWhiteSpace(material1))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material1;
                                                pop.GraphicLength = s.order.GraphicLength;
                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = customerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = calerOutsourceId;//北京卡乐
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                            material1 = "3mmPVC";

                                            Quantity = s.order.Quantity ?? 1;
                                            if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                            {
                                                Quantity = Quantity > 0 ? 1 : 0;
                                            }
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                            outsourceOrderDetailModel.Area = s.order.Area;
                                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = s.order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                            outsourceOrderDetailModel.City = s.order.City;
                                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                            outsourceOrderDetailModel.Contact = s.order.Contact;
                                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                            outsourceOrderDetailModel.Format = s.order.Format;
                                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : s.order.Gender;
                                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material1;
                                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                            outsourceOrderDetailModel.POPName = s.order.POPName;
                                            outsourceOrderDetailModel.POPType = s.order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                            outsourceOrderDetailModel.Province = s.order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = s.order.Region;
                                            outsourceOrderDetailModel.Remark = s.order.Remark;
                                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                            outsourceOrderDetailModel.Tel = s.order.Tel;
                                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                            decimal unitPrice1 = 0;
                                            decimal totalPrice1 = 0;
                                            if (!string.IsNullOrWhiteSpace(material1))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material1;
                                                pop.GraphicLength = s.order.GraphicLength;
                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = customerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                            }
                                            //outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                            //outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                            addInstallPrice = true;
                                            if (extraInstallPriceSubjectId == 0)
                                                extraInstallPriceSubjectId = s.order.SubjectId ?? 0;
                                        }
                                        else
                                        {

                                            int Quantity = s.order.Quantity ?? 1;
                                            if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                            {
                                                Quantity = Quantity > 0 ? 1 : 0;
                                            }

                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                            outsourceOrderDetailModel.Area = s.order.Area;
                                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = s.order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                            outsourceOrderDetailModel.City = s.order.City;
                                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                            outsourceOrderDetailModel.Contact = s.order.Contact;
                                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                            outsourceOrderDetailModel.Format = s.order.Format;
                                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                            //string material = string.Empty;
                                            //if (!string.IsNullOrWhiteSpace(material0))
                                            //    material = new BasePage().GetBasicMaterial(material0);
                                            //if (string.IsNullOrWhiteSpace(material))
                                            //    material = s.order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material;
                                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                            outsourceOrderDetailModel.POPName = s.order.POPName;
                                            outsourceOrderDetailModel.POPType = s.order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                            outsourceOrderDetailModel.Province = s.order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = s.order.Region;
                                            outsourceOrderDetailModel.Remark = s.order.Remark;
                                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                            outsourceOrderDetailModel.Tel = s.order.Tel;
                                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                            decimal unitPrice = 0;
                                            decimal totalPrice = 0;
                                            if (!string.IsNullOrWhiteSpace(material))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material;
                                                pop.GraphicLength = s.order.GraphicLength;
                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = customerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                            {
                                                outsourceOrderDetailModel.OutsourceId = outsourceId;
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                if (s.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                    hasInstallPrice = true;
                                            }
                                            else
                                                addInstallPrice = true;
                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                        }
                                    }
                                }
                                else
                                {
                                    //非天津
                                    int Quantity = s.order.Quantity ?? 1;
                                    if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                    {
                                        Quantity = Quantity > 0 ? 1 : 0;
                                    }
                                    outsourceOrderDetailModel = new OutsourceOrderDetail();
                                    outsourceOrderDetailModel.AddDate = DateTime.Now;
                                    outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                    outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                    outsourceOrderDetailModel.Area = s.order.Area;
                                    outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                    outsourceOrderDetailModel.Channel = s.order.Channel;
                                    outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                    outsourceOrderDetailModel.City = s.order.City;
                                    outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                    outsourceOrderDetailModel.Contact = s.order.Contact;
                                    outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                    outsourceOrderDetailModel.Format = s.order.Format;
                                    outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : s.order.Gender;
                                    outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                    outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                    string material = string.Empty;
                                    if (!string.IsNullOrWhiteSpace(material0))
                                        material = new BasePage().GetBasicMaterial(material0);
                                    if (string.IsNullOrWhiteSpace(material))
                                        material = s.order.GraphicMaterial;
                                    outsourceOrderDetailModel.GraphicMaterial = material;
                                    outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                    outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                    outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                    outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                    outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                    outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                    outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                    outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                    outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                    outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                    outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                    outsourceOrderDetailModel.POPName = s.order.POPName;
                                    outsourceOrderDetailModel.POPType = s.order.POPType;
                                    outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                    outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                    outsourceOrderDetailModel.Province = s.order.Province;
                                    outsourceOrderDetailModel.Quantity = Quantity;
                                    outsourceOrderDetailModel.Region = s.order.Region;
                                    outsourceOrderDetailModel.Remark = s.order.Remark;
                                    outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                    outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                    outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                    outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                    outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                    outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                    outsourceOrderDetailModel.Tel = s.order.Tel;
                                    outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                    outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                    outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                    outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                    outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                    outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                    outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                    outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                    decimal unitPrice1 = 0;
                                    decimal totalPrice1 = 0;
                                    
                                    if (!string.IsNullOrWhiteSpace(material))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = material;
                                        pop.GraphicLength = s.order.GraphicLength;
                                        pop.GraphicWidth = s.order.GraphicWidth;
                                        pop.Quantity = Quantity;
                                        pop.CustomerId = customerId;
                                        pop.OutsourceType = assignType;
                                        if ((!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south")) || guidanceType == (int)GuidanceTypeEnum.Promotion)
                                        {
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                        }
                                        new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                        outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                        outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                    }
                                    outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                    outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                    outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                    outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                    outsourceOrderDetailModel.OutsourceId = outsourceId;
                                    outsourceOrderDetailModel.AssignType = assignType;
                                    if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                    {
                                        //outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                        //if(s.subject.SubjectType!=(int)SubjectTypeEnum.费用订单)
                                        //hasInstallPrice = true;
                                        if (s.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                            hasInstallPrice = true;
                                        if (!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south"))
                                        {
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            outsourceOrderDetailModel.PayOrderPrice = 0;

                                        }
                                        else
                                        {
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                        }
                                    }
                                    if (s.order.OrderType == (int)OrderTypeEnum.发货费 && s.order.Province == "内蒙古" && !s.order.City.Contains("通辽"))
                                    {
                                        outsourceOrderDetailModel.PayOrderPrice = 0;
                                    }
                                    //if (!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south"))
                                    //{
                                    //    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                    //    if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                    //    {
                                    //        outsourceOrderDetailModel.PayOrderPrice = 0;
                                    //    }
                                    //}
                                    if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                    {
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.order.Sheet == "橱窗" && s.order.GraphicLength > 1 && s.order.GraphicWidth > 1 && material0.Contains("全透贴"))
                                        {
                                           
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            promotionInstallPrice = 150;
                                        }
                                    }
                                    outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                    outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                }
                                assignOrderCount++;
                            });
                        }
                        #endregion
                    }
                    else
                    {
                        #region 北京订单
                        var orderList1 = (from order in OutsourceOrderList
                                         join subject in CurrentContext.DbContext.Subject
                                         on order.SubjectId equals subject.Id
                                         select new {
                                             order,
                                             subject
                                         }).ToList();
                        orderList1.ForEach(s =>
                        {
                            int Quantity = s.order.Quantity ?? 1;
                            if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                            {
                                Quantity = Quantity > 0 ? 1 : 0;
                            }
                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                            outsourceOrderDetailModel.Area = s.order.Area;
                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                            outsourceOrderDetailModel.Channel = s.order.Channel;
                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                            outsourceOrderDetailModel.City = s.order.City;
                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                            outsourceOrderDetailModel.Contact = s.order.Contact;
                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                            outsourceOrderDetailModel.Format = s.order.Format;
                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : s.order.Gender;
                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                            string material = string.Empty;
                            if (!string.IsNullOrWhiteSpace(s.order.GraphicMaterial))
                                material = new BasePage().GetBasicMaterial(s.order.GraphicMaterial);
                            if (string.IsNullOrWhiteSpace(material))
                                material = s.order.GraphicMaterial;
                            outsourceOrderDetailModel.GraphicMaterial = s.order.GraphicMaterial;
                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                            outsourceOrderDetailModel.POPName = s.order.POPName;
                            outsourceOrderDetailModel.POPType = s.order.POPType;
                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                            outsourceOrderDetailModel.Province = s.order.Province;
                            outsourceOrderDetailModel.Quantity = Quantity;
                            outsourceOrderDetailModel.Region = s.order.Region;
                            outsourceOrderDetailModel.Remark = s.order.Remark;
                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                            outsourceOrderDetailModel.Tel = s.order.Tel;
                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                            decimal unitPrice1 = 0;
                            decimal totalPrice1 = 0;
                            if (!string.IsNullOrWhiteSpace(material))
                            {
                                POP pop = new POP();
                                pop.GraphicMaterial = material;
                                pop.GraphicLength = s.order.GraphicLength;
                                pop.GraphicWidth = s.order.GraphicWidth;
                                pop.Quantity = Quantity;
                                pop.CustomerId = customerId;
                                pop.OutsourceType = assignType;
                                if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install; ;
                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                outsourceOrderDetailModel.TotalPrice = totalPrice1;
                            }
                            outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                            outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                            outsourceOrderDetailModel.AssignType = assignType;
                            if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                            {
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                if(s.subject.SubjectType!=(int)SubjectTypeEnum.费用订单)
                                    hasInstallPrice = true;

                            }
                            if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                            {
                                
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.order.Sheet == "橱窗" && s.order.GraphicLength > 1 && s.order.GraphicWidth > 1 && s.order.GraphicMaterial.Contains("全透贴"))
                                {
                                   
                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                    promotionInstallPrice = 150;
                                }
                            }
                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                            assignOrderCount++;
                        });
                        #endregion
                    }
                    #endregion

                    //if (assignOrderModel.Shop.ShopNo == "P22047")
                    //{
                    //    int a = 1;
                    //}
                    #region 安装费和快递费
                    var popList = OutsourceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具);
                    if (guidanceType == (int)GuidanceTypeEnum.Promotion)
                    {
                        if (promotionInstallPrice > 0)
                        {
                            addInstallPrice = true;
                            assignOrderModel.HasExpressPrice = false;
                        }
                        else
                        {
                            addInstallPrice = false;
                        }
                    }
                    else if (guidanceType == (int)GuidanceTypeEnum.Install && assignType == (int)OutsourceOrderTypeEnum.Install && popList.Any())
                    {
                        addInstallPrice = true;
                    }
                    
                    if (addInstallPrice && !hasInstallPrice)
                    {


                        //按照级别，获取基础安装费
                        basicInstallPrice = new BasePage().GetOutsourceBasicInstallPrice(materialSupport);
                        //获取户外安装费

                        var oohList = OutsourceOrderList.Where(s => (s.Sheet != null && (s.Sheet.Contains("户外") || s.Sheet.ToLower() == "ooh"))).ToList();
                        if (oohList.Any())
                        {

                            Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                            oohList.ForEach(s =>
                            {
                                decimal price = 0;
                                if (!string.IsNullOrWhiteSpace(s.GraphicNo))
                                {
                                    price = oohPOPList.Where(p => p.ShopId == shopId && p.GraphicNo.ToLower() == s.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                }
                                else
                                    price = oohPOPList.Where(p => p.ShopId == shopId && p.GraphicLength == s.GraphicLength && p.GraphicWidth == s.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                if (oohPriceDic.Keys.Contains(shopId))
                                {
                                    if (oohPriceDic[shopId] < price)
                                    {
                                        oohPriceDic[shopId] = price;
                                    }
                                }
                                else
                                    oohPriceDic.Add(shopId, price);
                            });

                            if (oohPriceDic.Keys.Count > 0)
                            {
                                foreach (KeyValuePair<int, decimal> item in oohPriceDic)
                                {
                                    oohInstallPrice = item.Value;
                                }
                            }
                        }


                        //hasExtraInstallPrice = true;
                        //添加安装费
                        decimal receiveInstallPrice = 0;
                        decimal installPrice = 0;
                        var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == guidanceId && sh.ShopId == shopId);
                        if (installShopList.Any())
                        {
                            installShopList.ForEach(sh =>
                            {
                                receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                            });
                        }
                        if ((assignOrderModel.Shop.OutsourceInstallPrice ?? 0) > 0)
                        {
                            basicInstallPrice = assignOrderModel.Shop.OutsourceInstallPrice ?? 0;
                        }
                        if (assignOrderModel.SubjectCornerType == "三叶草")
                        {
                            if ((assignOrderModel.Shop.OutsourceBCSInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = assignOrderModel.Shop.OutsourceBCSInstallPrice ?? 0;
                            }
                            else if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                            {
                                basicInstallPrice = 150;
                            }
                            else
                            {
                                basicInstallPrice = 0;
                            }

                        }
                        if (assignOrderModel.SubjectCategoryName.Contains("常规-非活动"))
                        {
                           
                            if (assignOrderModel.Shop.CityName == "包头市" && (assignOrderModel.Shop.OutsourceInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = assignOrderModel.Shop.OutsourceInstallPrice ?? 0;
                            }
                            else if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                            {
                                basicInstallPrice = 150;
                            }
                            else
                            {
                                basicInstallPrice = 0;
                            }
                        }
                        if (promotionInstallPrice > 0)
                            basicInstallPrice = promotionInstallPrice;
                        installPrice = oohInstallPrice + basicInstallPrice;

                        if (installPrice > 0)
                        {

                            if (oohInstallPrice > 0 && (assignOrderModel.Shop.OOHInstallOutsourceId ?? 0) > 0)
                            {
                                //如果有单独的户外安装外协
                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].AgentCode;
                                outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].AgentName;
                                outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].BusinessModel;
                                outsourceOrderDetailModel.Channel = OutsourceOrderList[0].Channel;
                                outsourceOrderDetailModel.City = OutsourceOrderList[0].City;
                                outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].CityTier;
                                outsourceOrderDetailModel.Contact = OutsourceOrderList[0].Contact;
                                outsourceOrderDetailModel.Format = OutsourceOrderList[0].Format;
                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                outsourceOrderDetailModel.GraphicWidth = 0;
                                outsourceOrderDetailModel.GuidanceId = guidanceId;
                                outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].IsInstall;
                                outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].BCSIsInstall;
                                outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].LocationType;
                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].POPAddress;
                                outsourceOrderDetailModel.POPName = string.Empty;
                                outsourceOrderDetailModel.POPType = string.Empty;
                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                outsourceOrderDetailModel.POSScale = posScale;
                                outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                                outsourceOrderDetailModel.Quantity = 1;
                                outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                                outsourceOrderDetailModel.Remark = string.Empty;
                                outsourceOrderDetailModel.Sheet = string.Empty;
                                outsourceOrderDetailModel.ShopId = shopId;
                                outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].ShopName;
                                outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].ShopNo;
                                outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].ShopStatus;
                                outsourceOrderDetailModel.SubjectId =0;
                                outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                                outsourceOrderDetailModel.TotalArea = 0;
                                outsourceOrderDetailModel.WindowDeep = 0;
                                outsourceOrderDetailModel.WindowHigh = 0;
                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                outsourceOrderDetailModel.WindowWide = 0;
                                outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                outsourceOrderDetailModel.PayOrderPrice = oohInstallPrice;
                                outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].CSUserId;
                                outsourceOrderDetailModel.OutsourceId = assignOrderModel.Shop.OOHInstallOutsourceId;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                installPrice = installPrice - oohInstallPrice;
                                oohInstallPrice = 0;
                            }
                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].AgentCode;
                            outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].AgentName;
                            outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].BusinessModel;
                            outsourceOrderDetailModel.Channel = OutsourceOrderList[0].Channel;
                            outsourceOrderDetailModel.City = OutsourceOrderList[0].City;
                            outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].CityTier;
                            outsourceOrderDetailModel.Contact = OutsourceOrderList[0].Contact;
                            outsourceOrderDetailModel.Format = OutsourceOrderList[0].Format;
                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                            outsourceOrderDetailModel.GraphicWidth = 0;
                            outsourceOrderDetailModel.GuidanceId = guidanceId;
                            outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].LocationType;
                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                            outsourceOrderDetailModel.MaterialSupport = materialSupport;
                            outsourceOrderDetailModel.OrderGender = string.Empty;
                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                            outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].POPAddress;
                            outsourceOrderDetailModel.POPName = string.Empty;
                            outsourceOrderDetailModel.POPType = string.Empty;
                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                            outsourceOrderDetailModel.POSScale = posScale;
                            outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                            outsourceOrderDetailModel.Quantity = 1;
                            outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                            outsourceOrderDetailModel.Remark = string.Empty;
                            outsourceOrderDetailModel.Sheet = string.Empty;
                            outsourceOrderDetailModel.ShopId = shopId;
                            outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].ShopName;
                            outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].ShopNo;
                            outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].ShopStatus;
                            outsourceOrderDetailModel.SubjectId = 0;
                            outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                            outsourceOrderDetailModel.TotalArea = 0;
                            outsourceOrderDetailModel.WindowDeep = 0;
                            outsourceOrderDetailModel.WindowHigh = 0;
                            outsourceOrderDetailModel.WindowSize = string.Empty;
                            outsourceOrderDetailModel.WindowWide = 0;
                            outsourceOrderDetailModel.ReceiveOrderPrice = receiveInstallPrice;
                            outsourceOrderDetailModel.PayOrderPrice = installPrice;
                            outsourceOrderDetailModel.PayBasicInstallPrice = basicInstallPrice;
                            outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                            outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].CSUserId;
                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                        }
                    }
                    else if ((guidanceType == (int)GuidanceTypeEnum.Promotion && assignOrderModel.HasExpressPrice) || (guidanceType == (int)GuidanceTypeEnum.Delivery))
                    {
                        //快递费
                        decimal rExpressPrice = 0;
                        decimal payExpressPrice = 0;
                        if (expressPriceDetailModel != null && (expressPriceDetailModel.ExpressPrice ?? 0) > 0)
                        {
                            rExpressPrice = expressPriceDetailModel.ExpressPrice ?? 0;
                        }
                        else
                            rExpressPrice = 35;

                        ExpressPriceConfig eM = expressPriceConfigList.Where(price => price.ReceivePrice == rExpressPrice).FirstOrDefault();
                        if (eM != null)
                            payExpressPrice = eM.PayPrice ?? 0;
                        else
                            payExpressPrice = 22;
                        if (assignOrderModel.Shop.ProvinceName == "内蒙古" && !assignOrderModel.Shop.CityName.Contains("通辽"))
                        {
                            payExpressPrice = 0;
                        }
                        //if (payExpressPrice > 0)
                        //{
                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].AgentCode;
                            outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].AgentName;
                            outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].BusinessModel;
                            outsourceOrderDetailModel.Channel = OutsourceOrderList[0].Channel;
                            outsourceOrderDetailModel.City = OutsourceOrderList[0].City;
                            outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].CityTier;
                            outsourceOrderDetailModel.Contact = OutsourceOrderList[0].Contact;
                            outsourceOrderDetailModel.Format = OutsourceOrderList[0].Format;
                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                            outsourceOrderDetailModel.GraphicWidth = 0;
                            outsourceOrderDetailModel.GuidanceId = guidanceId;
                            outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].LocationType;
                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                            outsourceOrderDetailModel.MaterialSupport = materialSupport;
                            outsourceOrderDetailModel.OrderGender = string.Empty;
                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.发货费;
                            outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].POPAddress;
                            outsourceOrderDetailModel.POPName = string.Empty;
                            outsourceOrderDetailModel.POPType = string.Empty;
                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                            outsourceOrderDetailModel.POSScale = posScale;
                            outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                            outsourceOrderDetailModel.Quantity = 1;
                            outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                            outsourceOrderDetailModel.Remark = string.Empty;
                            outsourceOrderDetailModel.Sheet = string.Empty;
                            outsourceOrderDetailModel.ShopId = shopId;
                            outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].ShopName;
                            outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].ShopNo;
                            outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].ShopStatus;
                            outsourceOrderDetailModel.SubjectId = 0;
                            outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                            outsourceOrderDetailModel.TotalArea = 0;
                            outsourceOrderDetailModel.WindowDeep = 0;
                            outsourceOrderDetailModel.WindowHigh = 0;
                            outsourceOrderDetailModel.WindowSize = string.Empty;
                            outsourceOrderDetailModel.WindowWide = 0;
                            outsourceOrderDetailModel.ReceiveOrderPrice = rExpressPrice;
                            outsourceOrderDetailModel.PayOrderPrice = payExpressPrice;
                            outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                            outsourceOrderDetailModel.PayOOHInstallPrice = 0;
                            outsourceOrderDetailModel.PayExpressPrice = payExpressPrice;
                            outsourceOrderDetailModel.ReceiveExpresslPrice = rExpressPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                            outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].CSUserId;
                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                            if (assignOrderModel.Shop.ProvinceName == "天津")
                                outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                        //}
                    }
                    #endregion
                }
            }

        }



        //int orderIdNew = 0;
        int orderid = 0;
        void SaveAssignOrderNew(OutsourceAssignOrderModel assignOrderModel)
        {

            
            string posScale = string.Empty;
            if (assignOrderModel != null && assignOrderModel.Shop != null && assignOrderModel.OrderList.Any())
            {
                var OutsourceOrderList = (from order in assignOrderModel.OrderList
                                         join subject in CurrentContext.DbContext.Subject
                                         on order.SubjectId equals subject.Id
                                         join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                         on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                         from subjectCategory in categortTemp.DefaultIfEmpty()
                                         where (subject.IsDelete==null || subject.IsDelete==false)
                                         && subject.ApproveState==1
                                         select new {
                                             order,
                                             subject,
                                             CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                         }).ToList();
                decimal promotionInstallPrice = 0;
                //decimal PVCInstallPrice = 0;
                int customerId = assignOrderModel.Guidance.CustomerId??0;
                int outsourceId = assignOrderModel.Shop.OutsourceId ?? 0;
                int guidanceId = assignOrderModel.Guidance.ItemId;
                int guidanceType = assignOrderModel.Guidance.ActivityTypeId??0;
                int shopId = assignOrderModel.Shop.Id;
                int assignType = 0;
                bool isBCSSubject = true;
                bool isGeneric = true;
                bool addInstallPrice = false;
                bool hasInstallPrice = false;
                bool inInstallShop = false;
                List<string> materialSupportList = new List<string>();

                //去重
                List<FinalOrderDetailTemp> tempOrderList = new List<FinalOrderDetailTemp>();
                List<int> repeatOrderIdList = new List<int>();
                OutsourceOrderList.ForEach(s =>
                {
                    
                    bool canGo = true;
                    if (!string.IsNullOrWhiteSpace(s.order.GraphicNo) && !string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                    {
                        //string gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                        //去掉重复的（同一个编号下多次的）
                        var checkList = tempOrderList.Where(sl => sl.Sheet == s.order.Sheet && sl.PositionDescription == s.order.PositionDescription && sl.GraphicNo == s.order.GraphicNo && sl.GraphicLength == s.order.GraphicLength && sl.GraphicWidth == s.order.GraphicWidth && (sl.Gender == s.order.Gender || sl.OrderGender == s.order.OrderGender)).ToList();

                        if (checkList.Any())
                        {
                            canGo = false;
                            repeatOrderIdList.Add(s.order.Id);
                        }
                    }
                    if (canGo)
                    {
                        if (!string.IsNullOrWhiteSpace(s.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(s.order.InstallPriceMaterialSupport.ToLower()))
                        {
                            materialSupportList.Add(s.order.InstallPriceMaterialSupport.ToLower());
                        }
                        tempOrderList.Add(s.order);
                        if (s.subject.CornerType != "三叶草")
                            isBCSSubject = false;
                        if (!s.CategoryName.Contains("常规-非活动"))
                            isGeneric = false;
                    }
                    
                });
                if (repeatOrderIdList.Any())
                {
                    OutsourceOrderList = OutsourceOrderList.Where(s => !repeatOrderIdList.Contains(s.order.Id)).ToList();
                }
                if (OutsourceOrderList.Any())
                {
                    if (guidanceType == (int)GuidanceTypeEnum.Install)
                    {
                        if (isBCSSubject)
                        {
                            if (assignOrderModel.Shop.BCSIsInstall == "Y")
                            {
                                assignType = (int)OutsourceOrderTypeEnum.Install;
                                inInstallShop = true;
                            }
                            else
                            {
                                assignType = (int)OutsourceOrderTypeEnum.Send;
                            }
                        }
                        else 
                        {
                            if (assignOrderModel.Shop.IsInstall == "Y")
                            {
                                assignType = (int)OutsourceOrderTypeEnum.Install;
                                inInstallShop = true;
                            }
                            else
                            {
                                assignType = (int)OutsourceOrderTypeEnum.Send;
                            }
                        }
                    }
                    else
                    {
                        //assignType = (int)OutsourceOrderTypeEnum.Install;
                        if (assignOrderModel.Shop.IsInstall == "Y")
                        {
                            assignType = (int)OutsourceOrderTypeEnum.Install;
                            inInstallShop = true;
                        }
                        else
                        {
                            assignType = (int)OutsourceOrderTypeEnum.Send;
                        }
                    }
                    if (assignOrderModel.Shop.ProvinceName.Contains("内蒙古") && !assignOrderModel.Shop.CityName.Contains("通辽"))
                    {
                        assignType = (int)OutsourceOrderTypeEnum.Install;
                    }
                   
                   
                    decimal basicInstallPrice = 0;
                    decimal oohInstallPrice = 0;
                    #region 订单明细
                    if (!assignOrderModel.Shop.ProvinceName.Contains("北京"))
                    {
                        #region 非北京订单
                        List<int> assignedOrderIdList = new List<int>();
                        #region 按材质分
                        var materialConfig = configList.Where(s => s.ConfigTypeId == (int)OutsourceOrderConfigType.Material).ToList();
                        if (materialConfig.Any())
                        {

                            materialConfig.ForEach(config =>
                            {
                                OutsourceOrderPlaceConfigBLL placeConfigBll = new OutsourceOrderPlaceConfigBLL();
                                if (!string.IsNullOrWhiteSpace(config.MaterialName))
                                {
                                    var orderList = OutsourceOrderList.Where(order => !assignedOrderIdList.Contains(order.order.Id) && order.order.GraphicMaterial != null && order.order.GraphicMaterial.ToLower().Contains(config.MaterialName.ToLower()) && (order.order.OrderType == (int)OrderTypeEnum.POP || order.order.OrderType == (int)OrderTypeEnum.道具)).Select(order=>order.order).ToList();

                                    List<int> cityIdList = new List<int>();

                                    List<string> cityNameList = new List<string>();
                                    bool flag = false;
                                    var placeConfigList = (from placeConfin in CurrentContext.DbContext.OutsourceOrderPlaceConfig
                                                           join place in CurrentContext.DbContext.Place
                                                           on placeConfin.PrivinceId equals place.ID
                                                           where placeConfin.ConfigId == config.Id
                                                           select new
                                                           {
                                                               placeConfin.CityIds,
                                                               place.PlaceName
                                                           }).ToList();
                                    if (placeConfigList.Any())
                                    {
                                        placeConfigList.ForEach(pc =>
                                        {
                                            if (pc.PlaceName == assignOrderModel.Shop.ProvinceName)
                                            {
                                                orderList = orderList.Where(order => order.Province == pc.PlaceName).ToList();
                                                if (!string.IsNullOrWhiteSpace(pc.CityIds))
                                                {
                                                    cityIdList = StringHelper.ToIntList(pc.CityIds, ',');
                                                    cityNameList = placeList.Where(p => cityIdList.Contains(p.ID)).Select(p => p.PlaceName).ToList();
                                                    orderList = orderList.Where(order => cityNameList.Contains(order.City)).ToList();
                                                }
                                                flag = true;

                                            }
                                        });
                                    }

                                    if (!flag)
                                        orderList = new List<FinalOrderDetailTemp>();
                                    if (!string.IsNullOrWhiteSpace(config.Channel))
                                    {
                                        List<string> channelList = StringHelper.ToStringList(config.Channel, ',', LowerUpperEnum.ToLower);
                                        if (channelList.Any())
                                        {
                                            orderList = orderList.Where(order => order.Channel != null && channelList.Contains(order.Channel.ToLower())).ToList();
                                        }
                                    }
                                    if (orderList.Any())
                                    {
                                        orderList.ForEach(order =>
                                        {
                                            string material0 = order.GraphicMaterial;
                                            int Quantity = order.Quantity ?? 1;
                                            if (!string.IsNullOrWhiteSpace(order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(order.Sheet.ToUpper()))
                                            {
                                                Quantity = Quantity > 0 ? 1 : 0;
                                            }
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = order.AgentName;
                                            outsourceOrderDetailModel.Area = order.Area;
                                            outsourceOrderDetailModel.BusinessModel = order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = order.ChooseImg;
                                            outsourceOrderDetailModel.City = order.City;
                                            outsourceOrderDetailModel.CityTier = order.CityTier;
                                            outsourceOrderDetailModel.Contact = order.Contact;
                                            outsourceOrderDetailModel.CornerType = order.CornerType;
                                            outsourceOrderDetailModel.Format = order.Format;
                                            outsourceOrderDetailModel.Gender = (order.OrderGender != null && order.OrderGender != "") ? order.OrderGender : order.Gender;
                                            outsourceOrderDetailModel.GraphicLength = order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = order.GraphicMaterial;
                                            string material = string.Empty;
                                            if (!string.IsNullOrWhiteSpace(material0))
                                                material = new BasePage().GetBasicMaterial(material0);
                                            if (string.IsNullOrWhiteSpace(material))
                                                material = order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material;
                                            outsourceOrderDetailModel.GraphicNo = order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = order.POPAddress;
                                            outsourceOrderDetailModel.POPName = order.POPName;
                                            outsourceOrderDetailModel.POPType = order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = order.POSScale;
                                            outsourceOrderDetailModel.Province = order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = order.Region;
                                            outsourceOrderDetailModel.Remark = order.Remark;
                                            outsourceOrderDetailModel.Sheet = order.Sheet;
                                            outsourceOrderDetailModel.ShopId = order.ShopId;
                                            outsourceOrderDetailModel.ShopName = order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = order.SubjectId;
                                            outsourceOrderDetailModel.Tel = order.Tel;
                                            outsourceOrderDetailModel.TotalArea = order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = order.InstallPriceMaterialSupport;
                                            decimal unitPrice = 0;
                                            decimal totalPrice = 0;
                                            if (!string.IsNullOrWhiteSpace(material))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material;
                                                pop.GraphicLength = order.GraphicLength;
                                                pop.GraphicWidth = order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = customerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = order.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = order.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = config.ProductOutsourctId;
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            outsourceOrderDetailModel.FinalOrderId = order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                            assignedOrderIdList.Add(order.Id);
                                            assignOrderCount++;
                                        });
                                    }
                                }
                            });
                        }
                        #endregion
                        var orderList1 = (from order in OutsourceOrderList
                                          where !assignedOrderIdList.Contains(order.order.Id)
                                          select order).ToList();
                        if (orderList1.Any())
                        {
                            orderList1.ForEach(s =>
                            {
                                int Quantity = s.order.Quantity ?? 1;
                                if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                {
                                    Quantity = Quantity > 0 ? 1 : 0;
                                }
                                orderid = s.order.Id;
                                
                                string material0 = s.order.GraphicMaterial ?? "";
                                if (s.order.Province == "天津")
                                {
                                    if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)//促销
                                    {
                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                        outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                        outsourceOrderDetailModel.Area = s.order.Area;
                                        outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                        outsourceOrderDetailModel.Channel = s.order.Channel;
                                        outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                        outsourceOrderDetailModel.City = s.order.City;
                                        outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                        outsourceOrderDetailModel.Contact = s.order.Contact;
                                        outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                        outsourceOrderDetailModel.Format = s.order.Format;
                                        outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                        outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                        outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                        string material = string.Empty;
                                        if (!string.IsNullOrWhiteSpace(material0))
                                            material = new BasePage().GetBasicMaterial(material0);
                                        if (string.IsNullOrWhiteSpace(material))
                                            material = s.order.GraphicMaterial;
                                        outsourceOrderDetailModel.GraphicMaterial = material;
                                        outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                        outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                        outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                        outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                        outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                        outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                        outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                        outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                        outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                        outsourceOrderDetailModel.POPName = s.order.POPName;
                                        outsourceOrderDetailModel.POPType = s.order.POPType;
                                        outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                        outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                        outsourceOrderDetailModel.Province = s.order.Province;
                                        outsourceOrderDetailModel.Quantity = Quantity;
                                        outsourceOrderDetailModel.Region = s.order.Region;
                                        outsourceOrderDetailModel.Remark = s.order.Remark;
                                        outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                        outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                        outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                        outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                        outsourceOrderDetailModel.Tel = s.order.Tel;
                                        outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                        outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                        outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                        outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                        outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                        outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                        outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                        outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                        decimal unitPrice = 0;
                                        decimal totalPrice = 0;
                                        if (!string.IsNullOrWhiteSpace(material))
                                        {
                                            POP pop = new POP();
                                            pop.GraphicMaterial = material0;
                                            pop.GraphicLength = s.order.GraphicLength;
                                            pop.GraphicWidth = s.order.GraphicWidth;
                                            pop.Quantity = Quantity;
                                            pop.CustomerId = customerId;
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                            new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                            outsourceOrderDetailModel.UnitPrice = unitPrice;
                                            outsourceOrderDetailModel.TotalPrice = totalPrice;
                                        }
                                        outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                        outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                        outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                        outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                        outsourceOrderDetailModel.OutsourceId = calerOutsourceId;//北京卡乐
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.order.Sheet != null && (s.order.Sheet.Contains("橱窗") || s.order.Sheet.Contains("窗贴")) && s.order.GraphicLength > 1 && s.order.GraphicWidth > 1 && material0.Contains("全透贴"))
                                        {

                                            promotionInstallPrice = 150;

                                        }
                                        outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);





                                    }
                                    else
                                    {
                                        string material = string.Empty;
                                        if (!string.IsNullOrWhiteSpace(material0))
                                            material = new BasePage().GetBasicMaterial(material0);
                                        if (string.IsNullOrWhiteSpace(material))
                                            material = s.order.GraphicMaterial??string.Empty;
                                        if (material.Contains("背胶PP+") && material.Contains("雪弗板") && !material.Contains("蝴蝶支架"))
                                        {
                                            string material1 = "背胶PP";
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                            outsourceOrderDetailModel.Area = s.order.Area;
                                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = s.order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                            outsourceOrderDetailModel.City = s.order.City;
                                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                            outsourceOrderDetailModel.Contact = s.order.Contact;
                                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                            outsourceOrderDetailModel.Format = s.order.Format;
                                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material1;
                                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                            outsourceOrderDetailModel.POPName = s.order.POPName;
                                            outsourceOrderDetailModel.POPType = s.order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                            outsourceOrderDetailModel.Province = s.order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = s.order.Region;
                                            outsourceOrderDetailModel.Remark = s.order.Remark + ",天津裱板";
                                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                            outsourceOrderDetailModel.Tel = s.order.Tel;
                                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                            decimal unitPrice = 0;
                                            decimal totalPrice = 0;
                                            if (!string.IsNullOrWhiteSpace(material1))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material1;
                                                pop.GraphicLength = s.order.GraphicLength;
                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = customerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = calerOutsourceId;//北京卡乐
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                            material1 = "3mmPVC";

                                            
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                            outsourceOrderDetailModel.Area = s.order.Area;
                                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = s.order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                            outsourceOrderDetailModel.City = s.order.City;
                                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                            outsourceOrderDetailModel.Contact = s.order.Contact;
                                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                            outsourceOrderDetailModel.Format = s.order.Format;
                                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : s.order.Gender;
                                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material1;
                                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                            outsourceOrderDetailModel.POPName = s.order.POPName;
                                            outsourceOrderDetailModel.POPType = s.order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                            outsourceOrderDetailModel.Province = s.order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = s.order.Region;
                                            outsourceOrderDetailModel.Remark = s.order.Remark;
                                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                            outsourceOrderDetailModel.Tel = s.order.Tel;
                                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                            decimal unitPrice1 = 0;
                                            decimal totalPrice1 = 0;
                                            if (!string.IsNullOrWhiteSpace(material1))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material1;
                                                pop.GraphicLength = s.order.GraphicLength;
                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = customerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                            }
                                            //outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                            //outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);

                                            //PVCInstallPrice = 150;
                                            
                                        }
                                        else
                                        {
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                            outsourceOrderDetailModel.Area = s.order.Area;
                                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = s.order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                            outsourceOrderDetailModel.City = s.order.City;
                                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                            outsourceOrderDetailModel.Contact = s.order.Contact;
                                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                            outsourceOrderDetailModel.Format = s.order.Format;
                                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                            //string material = string.Empty;
                                            //if (!string.IsNullOrWhiteSpace(material0))
                                            //    material = new BasePage().GetBasicMaterial(material0);
                                            //if (string.IsNullOrWhiteSpace(material))
                                            //    material = s.order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material;
                                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                            outsourceOrderDetailModel.POPName = s.order.POPName;
                                            outsourceOrderDetailModel.POPType = s.order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                            outsourceOrderDetailModel.Province = s.order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = s.order.Region;
                                            outsourceOrderDetailModel.Remark = s.order.Remark;
                                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                            outsourceOrderDetailModel.Tel = s.order.Tel;
                                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                            decimal unitPrice = 0;
                                            decimal totalPrice = 0;
                                            if (!string.IsNullOrWhiteSpace(material))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material;
                                                pop.GraphicLength = s.order.GraphicLength;
                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = customerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                            {
                                                outsourceOrderDetailModel.OutsourceId = outsourceId;
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                if (s.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                    hasInstallPrice = true;
                                            }
                                            else
                                                addInstallPrice = true;
                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                        }
                                    }
                                }
                                else
                                {
                                    //非天津
                                    
                                    outsourceOrderDetailModel = new OutsourceOrderDetail();
                                    outsourceOrderDetailModel.AddDate = DateTime.Now;
                                    outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                    outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                    outsourceOrderDetailModel.Area = s.order.Area;
                                    outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                    outsourceOrderDetailModel.Channel = s.order.Channel;
                                    outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                    outsourceOrderDetailModel.City = s.order.City;
                                    outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                    outsourceOrderDetailModel.Contact = s.order.Contact;
                                    outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                    outsourceOrderDetailModel.Format = s.order.Format;
                                    outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : s.order.Gender;
                                    outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                    outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                    string material = string.Empty;
                                    if (!string.IsNullOrWhiteSpace(material0))
                                        material = new BasePage().GetBasicMaterial(material0);
                                    if (string.IsNullOrWhiteSpace(material))
                                        material = s.order.GraphicMaterial;
                                    outsourceOrderDetailModel.GraphicMaterial = material;
                                    outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                    outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                    outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                    outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                    outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                    outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                    outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                    outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                    outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                    outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                    outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                    outsourceOrderDetailModel.POPName = s.order.POPName;
                                    outsourceOrderDetailModel.POPType = s.order.POPType;
                                    outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                    outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                    outsourceOrderDetailModel.Province = s.order.Province;
                                    outsourceOrderDetailModel.Quantity = Quantity;
                                    outsourceOrderDetailModel.Region = s.order.Region;
                                    outsourceOrderDetailModel.Remark = s.order.Remark;
                                    outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                    outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                    outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                    outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                    outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                    outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                    outsourceOrderDetailModel.Tel = s.order.Tel;
                                    outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                    outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                    outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                    outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                    outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                    outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                    outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                    outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                    decimal unitPrice1 = 0;
                                    decimal totalPrice1 = 0;

                                    if (!string.IsNullOrWhiteSpace(material))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = material;
                                        pop.GraphicLength = s.order.GraphicLength;
                                        pop.GraphicWidth = s.order.GraphicWidth;
                                        pop.Quantity = Quantity;
                                        pop.CustomerId = customerId;
                                        pop.OutsourceType = assignType;
                                        if ((!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south")) || guidanceType == (int)GuidanceTypeEnum.Promotion)
                                        {
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                        }
                                        new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                        outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                        outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                    }
                                    outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                    outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                    outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                    outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                    outsourceOrderDetailModel.OutsourceId = outsourceId;
                                    outsourceOrderDetailModel.AssignType = assignType;
                                    if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                    {
                                       
                                        if (s.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                            hasInstallPrice = true;
                                        if (!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south"))
                                        {
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            outsourceOrderDetailModel.PayOrderPrice = 0;

                                        }
                                        else
                                        {
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                        }
                                    }
                                    if (s.order.OrderType == (int)OrderTypeEnum.发货费)
                                    {
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        if (s.order.Province == "内蒙古" && !s.order.City.Contains("通辽"))
                                            outsourceOrderDetailModel.PayOrderPrice = 0;
                                    }
                                   
                                    if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                    {
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.order.Sheet != null && (s.order.Sheet.Contains("橱窗") || s.order.Sheet.Contains("窗贴")) && s.order.GraphicLength > 1 && s.order.GraphicWidth > 1 && material0.Contains("全透贴"))
                                        {

                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            promotionInstallPrice = 150;
                                        }
                                    }
                                    outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                    outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                }
                                assignOrderCount++;
                            });
                        }
                        #endregion
                    }
                    else
                    {
                        #region 北京订单
                       
                        OutsourceOrderList.ForEach(s =>
                        {
                            int Quantity = s.order.Quantity ?? 1;
                            if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                            {
                                Quantity = Quantity > 0 ? 1 : 0;
                            }
                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                            outsourceOrderDetailModel.Area = s.order.Area;
                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                            outsourceOrderDetailModel.Channel = s.order.Channel;
                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                            outsourceOrderDetailModel.City = s.order.City;
                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                            outsourceOrderDetailModel.Contact = s.order.Contact;
                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                            outsourceOrderDetailModel.Format = s.order.Format;
                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : s.order.Gender;
                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                            string material = string.Empty;
                            if (!string.IsNullOrWhiteSpace(s.order.GraphicMaterial))
                                material = new BasePage().GetBasicMaterial(s.order.GraphicMaterial);
                            if (string.IsNullOrWhiteSpace(material))
                                material = s.order.GraphicMaterial;
                            outsourceOrderDetailModel.GraphicMaterial = s.order.GraphicMaterial;
                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                            outsourceOrderDetailModel.POPName = s.order.POPName;
                            outsourceOrderDetailModel.POPType = s.order.POPType;
                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                            outsourceOrderDetailModel.Province = s.order.Province;
                            outsourceOrderDetailModel.Quantity = Quantity;
                            outsourceOrderDetailModel.Region = s.order.Region;
                            outsourceOrderDetailModel.Remark = s.order.Remark;
                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                            outsourceOrderDetailModel.Tel = s.order.Tel;
                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                            decimal unitPrice1 = 0;
                            decimal totalPrice1 = 0;
                            if (!string.IsNullOrWhiteSpace(material))
                            {
                                POP pop = new POP();
                                pop.GraphicMaterial = material;
                                pop.GraphicLength = s.order.GraphicLength;
                                pop.GraphicWidth = s.order.GraphicWidth;
                                pop.Quantity = Quantity;
                                pop.CustomerId = customerId;
                                pop.OutsourceType = assignType;
                                if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install; ;
                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                outsourceOrderDetailModel.TotalPrice = totalPrice1;
                            }
                            outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                            outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                            outsourceOrderDetailModel.AssignType = assignType;
                            if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                            {
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                if (s.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                    hasInstallPrice = true;

                            }
                            if (s.order.OrderType == (int)OrderTypeEnum.发货费)
                            {
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                            }
                            if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                            {

                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.order.Sheet != null && (s.order.Sheet.Contains("橱窗") || s.order.Sheet.Contains("窗贴")) && s.order.GraphicLength > 1 && s.order.GraphicWidth > 1 && s.order.GraphicMaterial.Contains("全透贴"))
                                {

                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                    promotionInstallPrice = 150;
                                }
                            }
                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                            assignOrderCount++;
                        });
                        #endregion
                    }
                    #endregion

                    //if (assignOrderModel.Shop.ShopNo == "P22047")
                    //{
                    //    int a = 1;
                    //}
                    #region 安装费和快递费
                    var popList = OutsourceOrderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.POP || s.order.OrderType == (int)OrderTypeEnum.道具);
                    if (guidanceType == (int)GuidanceTypeEnum.Others)
                    {
                        addInstallPrice = false;
                    }
                    else if (guidanceType == (int)GuidanceTypeEnum.Promotion && inInstallShop)
                    {
                        if (promotionInstallPrice > 0)
                        {
                            addInstallPrice = true;
                            assignOrderModel.HasExpressPrice = false;
                        }
                        else
                        {
                            addInstallPrice = false;
                        }
                    }
                    else if (guidanceType == (int)GuidanceTypeEnum.Install && (assignOrderModel.Guidance.HasInstallFees??true) && inInstallShop && popList.Any())
                    {
                        addInstallPrice = true;
                    }
                    
                    if (addInstallPrice && !hasInstallPrice)
                    {

                        string materialSupport = string.Empty;
                        //按照级别，获取基础安装费
                        materialSupportList.ForEach(ma => {
                            decimal basicInstallPrice0 = new BasePage().GetOutsourceBasicInstallPrice(ma);
                            if (basicInstallPrice0 > basicInstallPrice)
                            {
                                basicInstallPrice = basicInstallPrice0;
                                materialSupport = ma;
                            }
                        });
                        
                        //获取户外安装费

                        var oohList = OutsourceOrderList.Where(s => (s.order.Sheet != null && (s.order.Sheet.Contains("户外") || s.order.Sheet.ToLower() == "ooh"))).ToList();
                        if (oohList.Any())
                        {

                            Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                            oohList.ForEach(s =>
                            {
                                decimal price = 0;
                                if (!string.IsNullOrWhiteSpace(s.order.GraphicNo))
                                {
                                    price = oohPOPList.Where(p => p.ShopId == shopId && p.GraphicNo.ToLower() == s.order.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                }
                                else
                                    price = oohPOPList.Where(p => p.ShopId == shopId && p.GraphicLength == s.order.GraphicLength && p.GraphicWidth == s.order.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                if (price > oohInstallPrice)
                                {
                                    oohInstallPrice = price;
                                }
                            });

                        }


                        //hasExtraInstallPrice = true;
                        //添加安装费
                        decimal receiveInstallPrice = 0;
                        decimal installPrice = 0;
                        var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == guidanceId && sh.ShopId == shopId);
                        if (installShopList.Any())
                        {
                            installShopList.ForEach(sh =>
                            {
                                receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                            });
                        }
                        if ((assignOrderModel.Shop.OutsourceInstallPrice ?? 0) > 0)
                        {
                            basicInstallPrice = assignOrderModel.Shop.OutsourceInstallPrice ?? 0;
                        }
                        if (isBCSSubject)
                        {
                            basicInstallPrice = 0;
                            if ((assignOrderModel.Shop.OutsourceBCSInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = assignOrderModel.Shop.OutsourceBCSInstallPrice ?? 0;
                            }
                            else if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                            {
                                basicInstallPrice = 150;
                            }
                            

                        }
                        if (isGeneric)
                        {
                            basicInstallPrice = 0;
                            if (assignOrderModel.Shop.CityName == "包头市" && (assignOrderModel.Shop.OutsourceInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = assignOrderModel.Shop.OutsourceInstallPrice ?? 0;
                            }
                            else if (assignOrderModel.BCSCityTierList.Contains(assignOrderModel.Shop.CityTier.ToUpper()))
                            {
                                basicInstallPrice = 150;
                            }
                            
                        }
                        string remark = "活动安装费";
                        if (promotionInstallPrice > 0)
                        {
                            installPrice = promotionInstallPrice;
                            receiveInstallPrice = promotionInstallPrice;
                            remark = "促销窗贴安装费";
                        }
                        else
                            installPrice = oohInstallPrice + basicInstallPrice;

                        if (installPrice > 0)
                        {

                            if (oohInstallPrice > 0 && (assignOrderModel.Shop.OOHInstallOutsourceId ?? 0) > 0)
                            {
                                //如果有单独的户外安装外协
                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].order.AgentCode;
                                outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].order.AgentName;
                                outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].order.BusinessModel;
                                outsourceOrderDetailModel.Channel = OutsourceOrderList[0].order.Channel;
                                outsourceOrderDetailModel.City = OutsourceOrderList[0].order.City;
                                outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].order.CityTier;
                                outsourceOrderDetailModel.Contact = OutsourceOrderList[0].order.Contact;
                                outsourceOrderDetailModel.Format = OutsourceOrderList[0].order.Format;
                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                outsourceOrderDetailModel.GraphicWidth = 0;
                                outsourceOrderDetailModel.GuidanceId = guidanceId;
                                outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].order.IsInstall;
                                outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].order.BCSIsInstall;
                                outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].order.LocationType;
                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].order.POPAddress;
                                outsourceOrderDetailModel.POPName = string.Empty;
                                outsourceOrderDetailModel.POPType = string.Empty;
                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                outsourceOrderDetailModel.POSScale = posScale;
                                outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                                outsourceOrderDetailModel.Quantity = 1;
                                outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                                outsourceOrderDetailModel.Remark = "户外安装费";
                                outsourceOrderDetailModel.Sheet = string.Empty;
                                outsourceOrderDetailModel.ShopId = shopId;
                                outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].order.ShopName;
                                outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].order.ShopNo;
                                outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].order.ShopStatus;
                                outsourceOrderDetailModel.SubjectId = 0;
                                outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                                outsourceOrderDetailModel.TotalArea = 0;
                                outsourceOrderDetailModel.WindowDeep = 0;
                                outsourceOrderDetailModel.WindowHigh = 0;
                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                outsourceOrderDetailModel.WindowWide = 0;
                                outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                outsourceOrderDetailModel.PayOrderPrice = oohInstallPrice;
                                outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].order.CSUserId;
                                outsourceOrderDetailModel.OutsourceId = assignOrderModel.Shop.OOHInstallOutsourceId;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                installPrice = installPrice - oohInstallPrice;
                                oohInstallPrice = 0;
                            }
                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].order.AgentCode;
                            outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].order.AgentName;
                            outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].order.BusinessModel;
                            outsourceOrderDetailModel.Channel = OutsourceOrderList[0].order.Channel;
                            outsourceOrderDetailModel.City = OutsourceOrderList[0].order.City;
                            outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].order.CityTier;
                            outsourceOrderDetailModel.Contact = OutsourceOrderList[0].order.Contact;
                            outsourceOrderDetailModel.Format = OutsourceOrderList[0].order.Format;
                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                            outsourceOrderDetailModel.GraphicWidth = 0;
                            outsourceOrderDetailModel.GuidanceId = guidanceId;
                            outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].order.IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].order.BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].order.LocationType;
                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                            outsourceOrderDetailModel.MaterialSupport = materialSupport;
                            outsourceOrderDetailModel.OrderGender = string.Empty;
                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                            outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].order.POPAddress;
                            outsourceOrderDetailModel.POPName = string.Empty;
                            outsourceOrderDetailModel.POPType = string.Empty;
                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                            outsourceOrderDetailModel.POSScale = posScale;
                            outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                            outsourceOrderDetailModel.Quantity = 1;
                            outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                            outsourceOrderDetailModel.Remark = remark;
                            outsourceOrderDetailModel.Sheet = string.Empty;
                            outsourceOrderDetailModel.ShopId = shopId;
                            outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].order.ShopName;
                            outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].order.ShopNo;
                            outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].order.ShopStatus;
                            outsourceOrderDetailModel.SubjectId = 0;
                            outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                            outsourceOrderDetailModel.TotalArea = 0;
                            outsourceOrderDetailModel.WindowDeep = 0;
                            outsourceOrderDetailModel.WindowHigh = 0;
                            outsourceOrderDetailModel.WindowSize = string.Empty;
                            outsourceOrderDetailModel.WindowWide = 0;
                            outsourceOrderDetailModel.ReceiveOrderPrice = receiveInstallPrice;
                            outsourceOrderDetailModel.PayOrderPrice = installPrice;
                            outsourceOrderDetailModel.PayBasicInstallPrice = basicInstallPrice;
                            outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                            outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].order.CSUserId;
                            outsourceOrderDetailModel.OutsourceId = outsourceId;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                        }
                    }
                    else if ((guidanceType == (int)GuidanceTypeEnum.Promotion && (assignOrderModel.Guidance.HasExperssFees??true)) || (guidanceType == (int)GuidanceTypeEnum.Delivery))
                    {
                        //快递费
                        decimal rExpressPrice = 0;
                        decimal payExpressPrice = 0;
                        if (expressPriceDetailModel != null && (expressPriceDetailModel.ExpressPrice ?? 0) > 0)
                        {
                            rExpressPrice = expressPriceDetailModel.ExpressPrice ?? 0;
                        }
                        else
                            rExpressPrice = 35;

                        ExpressPriceConfig eM = expressPriceConfigList.Where(price => price.ReceivePrice == rExpressPrice).FirstOrDefault();
                        if (eM != null)
                            payExpressPrice = eM.PayPrice ?? 0;
                        else
                            payExpressPrice = 22;
                        if (assignOrderModel.Shop.ProvinceName == "内蒙古" && !assignOrderModel.Shop.CityName.Contains("通辽"))
                        {
                            payExpressPrice = 0;
                        }
                        //if (payExpressPrice > 0)
                        //{
                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                        outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                        outsourceOrderDetailModel.AgentCode = OutsourceOrderList[0].order.AgentCode;
                        outsourceOrderDetailModel.AgentName = OutsourceOrderList[0].order.AgentName;
                        outsourceOrderDetailModel.BusinessModel = OutsourceOrderList[0].order.BusinessModel;
                        outsourceOrderDetailModel.Channel = OutsourceOrderList[0].order.Channel;
                        outsourceOrderDetailModel.City = OutsourceOrderList[0].order.City;
                        outsourceOrderDetailModel.CityTier = OutsourceOrderList[0].order.CityTier;
                        outsourceOrderDetailModel.Contact = OutsourceOrderList[0].order.Contact;
                        outsourceOrderDetailModel.Format = OutsourceOrderList[0].order.Format;
                        outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                        outsourceOrderDetailModel.GraphicNo = string.Empty;
                        outsourceOrderDetailModel.GraphicWidth = 0;
                        outsourceOrderDetailModel.GuidanceId = guidanceId;
                        outsourceOrderDetailModel.IsInstall = OutsourceOrderList[0].order.IsInstall;
                        outsourceOrderDetailModel.BCSIsInstall = OutsourceOrderList[0].order.BCSIsInstall;
                        outsourceOrderDetailModel.LocationType = OutsourceOrderList[0].order.LocationType;
                        outsourceOrderDetailModel.MachineFrame = string.Empty;
                        outsourceOrderDetailModel.MaterialSupport = string.Empty;
                        outsourceOrderDetailModel.OrderGender = string.Empty;
                        outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.发货费;
                        outsourceOrderDetailModel.POPAddress = OutsourceOrderList[0].order.POPAddress;
                        outsourceOrderDetailModel.POPName = string.Empty;
                        outsourceOrderDetailModel.POPType = string.Empty;
                        outsourceOrderDetailModel.PositionDescription = string.Empty;
                        outsourceOrderDetailModel.POSScale = posScale;
                        outsourceOrderDetailModel.Province = assignOrderModel.Shop.ProvinceName;
                        outsourceOrderDetailModel.Quantity = 1;
                        outsourceOrderDetailModel.Region = assignOrderModel.Shop.RegionName;
                        outsourceOrderDetailModel.Remark = string.Empty;
                        outsourceOrderDetailModel.Sheet = string.Empty;
                        outsourceOrderDetailModel.ShopId = shopId;
                        outsourceOrderDetailModel.ShopName = OutsourceOrderList[0].order.ShopName;
                        outsourceOrderDetailModel.ShopNo = OutsourceOrderList[0].order.ShopNo;
                        outsourceOrderDetailModel.ShopStatus = OutsourceOrderList[0].order.ShopStatus;
                        outsourceOrderDetailModel.SubjectId = 0;
                        outsourceOrderDetailModel.Tel = assignOrderModel.Shop.Tel1;
                        outsourceOrderDetailModel.TotalArea = 0;
                        outsourceOrderDetailModel.WindowDeep = 0;
                        outsourceOrderDetailModel.WindowHigh = 0;
                        outsourceOrderDetailModel.WindowSize = string.Empty;
                        outsourceOrderDetailModel.WindowWide = 0;
                        outsourceOrderDetailModel.ReceiveOrderPrice = rExpressPrice;
                        outsourceOrderDetailModel.PayOrderPrice = payExpressPrice;
                        outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                        outsourceOrderDetailModel.PayOOHInstallPrice = 0;
                        outsourceOrderDetailModel.PayExpressPrice = payExpressPrice;
                        outsourceOrderDetailModel.ReceiveExpresslPrice = rExpressPrice;
                        outsourceOrderDetailModel.InstallPriceMaterialSupport = string.Empty;
                        outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                        outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                        outsourceOrderDetailModel.CSUserId = OutsourceOrderList[0].order.CSUserId;
                        outsourceOrderDetailModel.OutsourceId = outsourceId;
                        if (assignOrderModel.Shop.ProvinceName == "天津")
                            outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                        //}
                    }
                    #endregion
                }
            }

        }




        List<string> regionList = new List<string>();
        void BindAssignOrder()
        {
            regionList.Clear();
            List<int> guidanceList = new List<int>();
            //foreach (ListItem li in cblGuidanceList.Items)
            //{
            //    if (li.Selected)
            //        guidanceList.Add(int.Parse(li.Value));
            //}
            //if (!guidanceList.Any())
            //{
            //    foreach (ListItem li in cblGuidanceList.Items)
            //    {
            //        guidanceList.Add(int.Parse(li.Value));
            //    }
            //}
            foreach (ListItem li in cblGuidanceList.Items)
            {
                guidanceList.Add(int.Parse(li.Value));
            }
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                    regionList.Add(li.Value);
            }
            if (regionList.Any())
            {
                labRegion.Text = StringHelper.ListToString(regionList, ",");
            }
            else
                labRegion.Text = "全部";

            var list = new SubjectGuidanceBLL().GetList(s => guidanceList.Contains(s.ItemId));
            Repeater1.DataSource = list;
            Repeater1.DataBind();
            Panel1.Visible = true;
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAssignOrder();
        }

        FinalOrderDetailTempBLL finalOrderBll = new FinalOrderDetailTempBLL();
        List<FinalOrderDetailTemp> finalOrderList0 = new List<FinalOrderDetailTemp>();
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                SubjectGuidance guidanceModel = (SubjectGuidance)e.Item.DataItem;
                if (guidanceModel != null)
                {
                    if (!finalOrderList0.Any())
                    {
                        LoadFinalOrderList();
                    }
                    var orderList = from order in finalOrderList0
                                    join subject in CurrentContext.DbContext.Subject
                                    on order.SubjectId equals subject.Id
                                    where  (subject.IsDelete == null || subject.IsDelete == false)
                                    && order.GuidanceId == guidanceModel.ItemId
                                    && subject.ApproveState==1
                                    select order;
                    if (regionList.Any())
                    {
                        orderList = orderList.Where(s => regionList.Contains(s.Region));
                    }
                    ((Label)e.Item.FindControl("labTotalShopCount")).Text = orderList.Select(s => s.ShopId ?? 0).Distinct().Count().ToString();
                    int totalOrderCount= orderList.Count();
                    ((Label)e.Item.FindControl("labTotalOrderCount")).Text = totalOrderCount.ToString();

                    var assignOrderList = outsourceOrderDetailBll.GetList(s => s.GuidanceId == guidanceModel.ItemId);
                    if (regionList.Any())
                    {
                        assignOrderList = assignOrderList.Where(s => regionList.Contains(s.Region)).ToList();
                    }
                    ((Label)e.Item.FindControl("labAssignShopCount")).Text = assignOrderList.Select(s => s.ShopId ?? 0).Distinct().Count().ToString();
                    int assignOrderCount = assignOrderList.Where(s => (s.FinalOrderId ?? 0) > 0).Select(s=>s.FinalOrderId).Distinct().Count();
                    ((Label)e.Item.FindControl("labAssignOrderCount")).Text = assignOrderCount.ToString();
                    int notAssignOrderCount = totalOrderCount - assignOrderCount;
                    if (notAssignOrderCount>0)
                        ((Label)e.Item.FindControl("labNotAssignOrderCount")).Text = "<span onclick='CheckNotAssignOrder(" + guidanceModel.ItemId + ")' style='color:red;cursor:pointer;text-decoration:underline;'>" + notAssignOrderCount + "</span>";
                    else
                        ((Label)e.Item.FindControl("labNotAssignOrderCount")).Text = notAssignOrderCount.ToString();
                }
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BindAssignOrder();
        }

        void LoadFinalOrderList()
        {
            List<int> guidanceList = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                //if (li.Selected)
                //{
                //    guidanceList.Add(int.Parse(li.Value));
                //}
                guidanceList.Add(int.Parse(li.Value));
            }
            //if (!guidanceList.Any())
            //{
            //    foreach (ListItem li in cblGuidanceList.Items)
            //    {
            //        guidanceList.Add(int.Parse(li.Value));
            //    }
            //}
            finalOrderList0 = new FinalOrderDetailTempBLL().GetList(s => guidanceList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == 1 && s.GraphicLength != null && s.GraphicLength > 1 && s.GraphicWidth != null && s.GraphicWidth > 1) || s.OrderType > 1) && (s.IsValid == null || s.IsValid == true) && (s.IsProduce == null || s.IsProduce == true) && (s.OrderType != (int)OrderTypeEnum.物料) && (s.IsValidFromAssign == null || s.IsValidFromAssign == true));
        }

    }
}
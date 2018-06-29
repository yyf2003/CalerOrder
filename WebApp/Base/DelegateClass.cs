using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using Models;
using BLL;
using System.Text;
using DAL;
using System.Data;
using System.Configuration;
using System.Transactions;

namespace WebApp.Base
{
    public class DelegateClass
    {
        int currUserId = new BasePage().CurrentUser.UserId;
        //自动保存外协订单
        public delegate void DeleSaveOutsourceOrder(int guidanceId, int subjectId);




        public void SaveOutsourceOrder(int guidanceId, int subjectId)
        {
            DeleSaveOutsourceOrder dele = new DeleSaveOutsourceOrder(SaveOutsourceOrderHandler);
            AsyncCallback callback = new AsyncCallback(CallBackMethod);
            dele.BeginInvoke(guidanceId, subjectId, callback, dele);
        }

        /// <summary>
        /// 回调方法
        /// </summary>
        /// <param name="ia"></param>
        void CallBackMethod(IAsyncResult ia)
        {
            DeleSaveOutsourceOrder dele = ia.AsyncState as DeleSaveOutsourceOrder;
            dele.EndInvoke(ia);
        }

        void SaveOutsourceOrderHandler(int guidanceId, int subjectId)
        {
            Subject subjectModel = new SubjectBLL().GetModel(subjectId);
            if (subjectModel != null)
            {
                if (subjectModel.IsSecondInstall ?? false)
                {
                    //二次安装
                    SaveHandler2(guidanceId, subjectId, subjectModel.SubjectType ?? 1);
                }
                else
                {
                    //非二次安装
                    SaveHandler1(guidanceId, subjectId, subjectModel.SubjectType ?? 1);
                }
            }
        }




        void SaveHandler2(int guidanceId, int subjectId, int subjectType)
        {

            SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(guidanceId);
            if (guidanceModel != null)
            {
                bool isSave = true;
                string errorMsg = string.Empty;
                //using (TransactionScope tran = new TransactionScope())
                //{
                try
                {

                    if ((subjectType != (int)SubjectTypeEnum.二次安装 && subjectType != (int)SubjectTypeEnum.费用订单))
                    {
                        if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guidanceModel.HasInstallFees ?? true))
                        {
                            new BasePage().SaveSecondInstallPrice(subjectId, subjectType);
                        }
                        //else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guidanceModel.HasExperssFees ?? true))
                        //{
                        //    new BasePage().SaveExpressPrice(guidanceId, subjectId, subjectType);
                        //}
                        //else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery)
                        //{
                        //    new BasePage().SaveExpressPriceForDelivery(guidanceId, subjectId, subjectType, guidanceModel.ExperssPrice);
                        //}
                    }
                    //保存报价单
                    new BasePage().SaveQuotationOrder(guidanceId, subjectId, subjectType);

                    var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                        on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                        from subjectCategory in categortTemp.DefaultIfEmpty()
                                        join gudiance in CurrentContext.DbContext.SubjectGuidance
                                        on subject.GuidanceId equals gudiance.ItemId
                                        where subject.Id == subjectId
                                        select new
                                        {
                                            subject,
                                            gudiance,
                                            CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                        }).FirstOrDefault();
                    if (subjectModel != null)
                    {

                        int guidanceType = subjectModel.gudiance.ActivityTypeId ?? 0;
                        List<FinalOrderDetailTemp> orderList0 = new List<FinalOrderDetailTemp>();
                        FinalOrderDetailTempBLL OrderDetailTempBll = new FinalOrderDetailTempBLL();
                        OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
                        OutsourceOrderDetail outsourceOrderDetailModel;
                        if (subjectType == (int)SubjectTypeEnum.HC订单 || subjectType == (int)SubjectTypeEnum.分区补单 || subjectType == (int)SubjectTypeEnum.分区增补 || subjectType == (int)SubjectTypeEnum.新开店订单)
                        {
                            orderList0 = OrderDetailTempBll.GetList(s => s.RegionSupplementId == subjectId && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType > 1)) && (s.OrderType != (int)OrderTypeEnum.物料));
                            //删除旧POP订单数据
                            outsourceOrderDetailBll.Delete(s => s.RegionSupplementId == subjectId);
                        }
                        else
                        {
                            orderList0 = OrderDetailTempBll.GetList(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0 && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType > 1)) && (s.OrderType != (int)OrderTypeEnum.物料));
                            //删除旧POP订单数据
                            outsourceOrderDetailBll.Delete(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0);
                        }
                        if (orderList0.Any())
                        {




                            List<ExpressPriceConfig> expressPriceConfigList = new ExpressPriceConfigBLL().GetList(s => s.Id > 0);
                            ExpressPriceDetailBLL expressPriceDetailBll = new ExpressPriceDetailBLL();
                            ExpressPriceDetail expressPriceDetailModel;

                            List<OutsourceOrderAssignConfig> configList = new OutsourceOrderAssignConfigBLL().GetList(s => s.Id > 0);
                            List<Place> placeList = new PlaceBLL().GetList(s => s.ID > 0);
                            List<int> shopIdList = orderList0.Select(s => s.ShopId ?? 0).ToList();
                            List<Shop> shopList = new ShopBLL().GetList(s => shopIdList.Contains(s.Id)).ToList();
                            //List<POP> oohPOPList = new POPBLL().GetList(pop => shopIdList.Contains(pop.ShopId ?? 0) && (pop.Sheet == "户外" || pop.Sheet.ToLower() == "ooh") && (pop.OOHInstallPrice ?? 0) > 0);
                            List<string> BCSCityTierList = new List<string>() { "T1", "T2", "T3" };
                            //List<FinalOrderDetailTemp> totalOrderList = OrderDetailTempBll.GetList(s => s.GuidanceId == subjectModel.gudiance.ItemId && shopIdList.Contains(s.ShopId ?? 0) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType == (int)OrderTypeEnum.道具)) && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true));
                            List<FinalOrderDetailTemp> totalOrderList = new List<FinalOrderDetailTemp>();
                            totalOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                              join subject in CurrentContext.DbContext.Subject
                                              on order.SubjectId equals subject.Id
                                              where order.GuidanceId == subjectModel.gudiance.ItemId
                                              && shopIdList.Contains(order.ShopId ?? 0)
                                              && ((order.OrderType == (int)OrderTypeEnum.POP
                                              && order.GraphicLength > 1 && order.GraphicWidth > 1) || (order.OrderType == (int)OrderTypeEnum.道具))
                                              && (order.IsDelete == null || order.IsDelete == false)
                                              && (order.IsValid == null || order.IsValid == true)
                                              && (subject.IsSecondInstall ?? false) == true
                                              select order).ToList();

                            string changePOPCountSheetStr = string.Empty;
                            string beiJingCalerOutsourceName = string.Empty;
                            string OOHInstallSheetName = string.Empty;
                            int calerOutsourceId = 8;
                            List<string> ChangePOPCountSheetList = new List<string>();
                            List<string> OOHInstallSheetList = new List<string>();
                            try
                            {
                                changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                                beiJingCalerOutsourceName = ConfigurationManager.AppSettings["CalerOutsourceName"];
                                OOHInstallSheetName = ConfigurationManager.AppSettings["OOHInstallSheet"];
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
                                Company companyModel = new CompanyBLL().GetList(s => (s.CompanyName == beiJingCalerOutsourceName || s.ShortName == beiJingCalerOutsourceName) && s.TypeId == (int)CompanyTypeEnum.Outsource).FirstOrDefault();
                                if (companyModel != null)
                                    calerOutsourceId = companyModel.Id;
                            }
                            if (!string.IsNullOrWhiteSpace(OOHInstallSheetName))
                            {
                                OOHInstallSheetList = StringHelper.ToStringList(OOHInstallSheetName, ',', LowerUpperEnum.ToUpper);
                            }
                            List<POP> oohPOPList = new POPBLL().GetList(pop => shopIdList.Contains(pop.ShopId ?? 0) && (OOHInstallSheetList.Any() ? (OOHInstallSheetList.Contains(pop.Sheet.ToUpper())) : (pop.Sheet == "户外" || pop.Sheet.ToLower() == "ooh")) && (pop.OSOOHInstallPrice ?? 0) > 0);

                            //删除安装费和发货费
                            outsourceOrderDetailBll.Delete(s => s.GuidanceId == subjectModel.gudiance.ItemId && shopIdList.Contains(s.ShopId ?? 0) && s.SubjectId == 0 && s.InstallPriceAddType == 2);
                            //var assignedList = outsourceOrderDetailBll.GetList(s => s.GuidanceId == subjectModel.gudiance.ItemId && s.SubjectId > 0);
                            var assignedList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                                join subject in CurrentContext.DbContext.Subject
                                                on order.SubjectId equals subject.Id
                                                where order.GuidanceId == subjectModel.gudiance.ItemId
                                                && order.SubjectId > 0
                                                && shopIdList.Contains(order.ShopId ?? 0)
                                                && ((order.OrderType == (int)OrderTypeEnum.POP
                                                && order.GraphicLength > 1 && order.GraphicWidth > 1) || (order.OrderType == (int)OrderTypeEnum.道具))
                                                && (order.IsDelete == null || order.IsDelete == false)
                                                && (subject.IsSecondInstall ?? false) == true
                                                select order).ToList();
                            shopList.ForEach(shop =>
                            {

                                bool isInstallShop = shop.IsInstall == "Y";
                                //if (shop.Id == 709)
                                //{
                                //int iddd = 0;
                                //}
                                bool hasInstallPrice = false;
                                bool addInstallPrice = false;

                                //List<string> materialSupportList = new List<string>();
                                string materialSupport = string.Empty;
                                string posScale = string.Empty;
                                decimal promotionInstallPrice = 0;
                                bool hasExpressPrice = subjectModel.gudiance.HasExperssFees ?? true;
                                //bool hasInstallPrice=subjectModel.gudiance.HasInstallFees ?? true;
                                var oneShopOrderList = orderList0.Where(order => order.ShopId == shop.Id).ToList();
                                //去重
                                List<FinalOrderDetailTemp> tempOrderList = new List<FinalOrderDetailTemp>();
                                oneShopOrderList.ForEach(s =>
                                {

                                    bool canGo = true;

                                    if (!string.IsNullOrWhiteSpace(s.GraphicNo) && !string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                    {
                                        //string gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                        //去掉重复的（同一个编号下多次的）
                                        var checkList = tempOrderList.Where(sl => sl.Sheet == s.Sheet && sl.PositionDescription == s.PositionDescription && sl.GraphicNo == s.GraphicNo && sl.GraphicLength == s.GraphicLength && sl.GraphicWidth == s.GraphicWidth && (sl.Gender == s.Gender || sl.OrderGender == s.OrderGender)).ToList();

                                        if (checkList.Any())
                                        {
                                            canGo = false;
                                        }
                                        else
                                        {
                                            var oneShopAssignedList = assignedList.Where(assign => assign.ShopId == s.ShopId).ToList();
                                            foreach (var assign in oneShopAssignedList)
                                            {
                                                if (assign.Sheet == s.Sheet && assign.PositionDescription == s.PositionDescription && assign.GraphicNo == s.GraphicNo && assign.GraphicLength == s.GraphicLength && assign.GraphicWidth == s.GraphicWidth && (assign.Gender == s.Gender || assign.Gender == s.OrderGender))
                                                {
                                                    canGo = false;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                    if (canGo)
                                    {
                                        tempOrderList.Add(s);

                                    }


                                });
                                oneShopOrderList = tempOrderList;
                                if (oneShopOrderList.Any())
                                {
                                    var popList = totalOrderList.Where(s => s.ShopId == shop.Id);
                                    //单店全部订单
                                    var totalOrderList0 = (from order in popList
                                                           join subject in CurrentContext.DbContext.Subject
                                                           on order.SubjectId equals subject.Id
                                                           join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                                          on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                                           from subjectCategory in categortTemp.DefaultIfEmpty()
                                                           where (subject.IsDelete == null || subject.IsDelete == false)
                                                           && subject.ApproveState == 1
                                                           && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                                           && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                                                           && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                                                           select new
                                                           {
                                                               order,
                                                               subject,
                                                               CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                                           }).ToList();
                                    bool isBCSSubject = true;
                                    bool isGeneric = true;
                                    totalOrderList0.ForEach(order =>
                                    {
                                        if (order.subject.CornerType != "三叶草")
                                            isBCSSubject = false;
                                        if (!order.CategoryName.Contains("常规-非活动"))
                                            isGeneric = false;
                                        //if (!string.IsNullOrWhiteSpace(order.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(order.order.InstallPriceMaterialSupport.ToLower()))
                                        //{
                                        //    materialSupportList.Add(order.order.InstallPriceMaterialSupport.ToLower());
                                        //}
                                        if (string.IsNullOrWhiteSpace(materialSupport))
                                            materialSupport = order.order.InstallPriceMaterialSupport;
                                        if (string.IsNullOrWhiteSpace(posScale))
                                            posScale = order.order.InstallPricePOSScale;
                                    });
                                    int assignType = 0;
                                    if (shop.ProvinceName == "内蒙古" && !shop.CityName.Contains("通辽"))
                                    {
                                        assignType = (int)OutsourceOrderTypeEnum.Install;

                                    }
                                    else
                                    {
                                        if (guidanceType == (int)GuidanceTypeEnum.Install)
                                        {
                                            if (isBCSSubject)
                                            {
                                                assignType = shop.BCSIsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                                                isInstallShop = shop.BCSIsInstall == "Y";
                                            }
                                            else
                                            {
                                                assignType = shop.IsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                                            }
                                        }
                                        else
                                        {
                                            assignType = shop.IsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                                        }
                                    }
                                    var oneShopOrderListNew = (from order in oneShopOrderList
                                                               join subject in CurrentContext.DbContext.Subject
                                                               on order.SubjectId equals subject.Id
                                                               select new
                                                               {
                                                                   order,
                                                                   subject
                                                               }).ToList();
                                    if (!shop.ProvinceName.Contains("北京"))
                                    {
                                        #region 非北京订单
                                        List<int> assignedOrderIdList = new List<int>();
                                        #region 按设置好的材质
                                        var materialConfig = configList.Where(s => s.ConfigTypeId == (int)OutsourceOrderConfigType.Material).ToList();
                                        if (materialConfig.Any())
                                        {

                                            materialConfig.ForEach(config =>
                                            {
                                                OutsourceOrderPlaceConfigBLL placeConfigBll = new OutsourceOrderPlaceConfigBLL();
                                                if (!string.IsNullOrWhiteSpace(config.MaterialName))
                                                {
                                                    //var orderList = oneShopOrderListNew.Where(order => !assignedOrderIdList.Contains(order.order.Id) && order.order.GraphicMaterial != null && order.order.GraphicMaterial.ToLower().Contains(config.MaterialName.ToLower()) && (order.order.OrderType == (int)OrderTypeEnum.POP || order.order.OrderType == (int)OrderTypeEnum.道具)).ToList();
                                                    var orderList = oneShopOrderListNew.Where(order => !assignedOrderIdList.Contains(order.order.Id) && order.order.GraphicMaterial != null && ((config.IsFullMatch ?? false) ? (order.order.GraphicMaterial.ToLower() == config.MaterialName.ToLower()) : (order.order.GraphicMaterial.ToLower().Contains(config.MaterialName.ToLower()))) && (order.order.OrderType == (int)OrderTypeEnum.POP || order.order.OrderType == (int)OrderTypeEnum.道具)).ToList();

                                                    List<int> cityIdList = new List<int>();

                                                    List<string> cityNameList = new List<string>();
                                                    bool hasProvince = false;
                                                    bool canGo = false;
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
                                                        hasProvince = true;
                                                        placeConfigList.ForEach(pc =>
                                                        {
                                                            if (pc.PlaceName == shop.ProvinceName)
                                                            {
                                                                orderList = orderList.Where(order => order.order.Province == pc.PlaceName).ToList();
                                                                if (!string.IsNullOrWhiteSpace(pc.CityIds))
                                                                {
                                                                    cityIdList = StringHelper.ToIntList(pc.CityIds, ',');
                                                                    cityNameList = placeList.Where(p => cityIdList.Contains(p.ID)).Select(p => p.PlaceName).ToList();
                                                                    orderList = orderList.Where(order => cityNameList.Contains(order.order.City)).ToList();
                                                                }
                                                                canGo = true;
                                                            }
                                                        });
                                                    }
                                                    if (hasProvince && !canGo)
                                                        orderList.Clear();
                                                    if (!string.IsNullOrWhiteSpace(config.Channel))
                                                    {
                                                        List<string> channelList = StringHelper.ToStringList(config.Channel, ',', LowerUpperEnum.ToLower);
                                                        if (channelList.Any())
                                                        {
                                                            orderList = orderList.Where(order => order.order.Channel != null && channelList.Contains(order.order.Channel.ToLower())).ToList();
                                                        }
                                                    }
                                                    if (orderList.Any())
                                                    {
                                                        orderList.ForEach(order =>
                                                        {
                                                            string material0 = order.order.GraphicMaterial;
                                                            int Quantity = order.order.Quantity ?? 1;
                                                            if (!string.IsNullOrWhiteSpace(order.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(order.order.Sheet.ToUpper()))
                                                            {
                                                                Quantity = Quantity > 0 ? 1 : 0;
                                                            }
                                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                            outsourceOrderDetailModel.AddUserId = currUserId;
                                                            outsourceOrderDetailModel.AgentCode = order.order.AgentCode;
                                                            outsourceOrderDetailModel.AgentName = order.order.AgentName;
                                                            outsourceOrderDetailModel.Area = order.order.Area;
                                                            outsourceOrderDetailModel.BusinessModel = order.order.BusinessModel;
                                                            outsourceOrderDetailModel.Channel = order.order.Channel;
                                                            outsourceOrderDetailModel.ChooseImg = order.order.ChooseImg;
                                                            outsourceOrderDetailModel.City = order.order.City;
                                                            outsourceOrderDetailModel.CityTier = order.order.CityTier;
                                                            outsourceOrderDetailModel.Contact = order.order.Contact;
                                                            outsourceOrderDetailModel.CornerType = order.order.CornerType;
                                                            outsourceOrderDetailModel.Format = order.order.Format;
                                                            outsourceOrderDetailModel.Gender = (order.order.OrderGender != null && order.order.OrderGender != "") ? order.order.OrderGender : order.order.Gender;
                                                            outsourceOrderDetailModel.GraphicLength = order.order.GraphicLength;
                                                            outsourceOrderDetailModel.OrderGraphicMaterial = order.order.GraphicMaterial;
                                                            string material = string.Empty;
                                                            if (!string.IsNullOrWhiteSpace(material0))
                                                                material = new BasePage().GetBasicMaterial(material0);
                                                            if (string.IsNullOrWhiteSpace(material))
                                                                material = order.order.GraphicMaterial;
                                                            outsourceOrderDetailModel.GraphicMaterial = material;
                                                            outsourceOrderDetailModel.GraphicNo = order.order.GraphicNo;
                                                            outsourceOrderDetailModel.GraphicWidth = order.order.GraphicWidth;
                                                            outsourceOrderDetailModel.GuidanceId = order.order.GuidanceId;
                                                            outsourceOrderDetailModel.IsInstall = order.order.IsInstall;
                                                            outsourceOrderDetailModel.BCSIsInstall = order.order.BCSIsInstall;
                                                            outsourceOrderDetailModel.LocationType = order.order.LocationType;
                                                            outsourceOrderDetailModel.MachineFrame = order.order.MachineFrame;
                                                            outsourceOrderDetailModel.MaterialSupport = order.order.MaterialSupport;
                                                            outsourceOrderDetailModel.OrderGender = order.order.OrderGender;
                                                            outsourceOrderDetailModel.OrderType = order.order.OrderType;
                                                            outsourceOrderDetailModel.POPAddress = order.order.POPAddress;
                                                            outsourceOrderDetailModel.POPName = order.order.POPName;
                                                            outsourceOrderDetailModel.POPType = order.order.POPType;
                                                            outsourceOrderDetailModel.PositionDescription = order.order.PositionDescription;
                                                            outsourceOrderDetailModel.POSScale = order.order.POSScale;
                                                            outsourceOrderDetailModel.Province = order.order.Province;
                                                            outsourceOrderDetailModel.Quantity = Quantity;
                                                            outsourceOrderDetailModel.Region = order.order.Region;
                                                            outsourceOrderDetailModel.Remark = order.order.Remark;
                                                            outsourceOrderDetailModel.Sheet = order.order.Sheet;
                                                            outsourceOrderDetailModel.ShopId = order.order.ShopId;
                                                            outsourceOrderDetailModel.ShopName = order.order.ShopName;
                                                            outsourceOrderDetailModel.ShopNo = order.order.ShopNo;
                                                            outsourceOrderDetailModel.ShopStatus = order.order.ShopStatus;
                                                            outsourceOrderDetailModel.SubjectId = order.order.SubjectId;
                                                            outsourceOrderDetailModel.Tel = order.order.Tel;
                                                            outsourceOrderDetailModel.TotalArea = order.order.TotalArea;
                                                            outsourceOrderDetailModel.WindowDeep = order.order.WindowDeep;
                                                            outsourceOrderDetailModel.WindowHigh = order.order.WindowHigh;
                                                            outsourceOrderDetailModel.WindowSize = order.order.WindowSize;
                                                            outsourceOrderDetailModel.WindowWide = order.order.WindowWide;
                                                            outsourceOrderDetailModel.ReceiveOrderPrice = order.order.OrderPrice;
                                                            outsourceOrderDetailModel.PayOrderPrice = order.order.PayOrderPrice;
                                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = order.order.InstallPriceMaterialSupport;
                                                            decimal unitPrice = 0;
                                                            decimal totalPrice = 0;
                                                            if (!string.IsNullOrWhiteSpace(material))
                                                            {
                                                                POP pop = new POP();
                                                                pop.GraphicMaterial = material;
                                                                pop.GraphicLength = order.order.GraphicLength;
                                                                pop.GraphicWidth = order.order.GraphicWidth;
                                                                pop.Quantity = Quantity;
                                                                pop.CustomerId = subjectModel.subject.CustomerId;
                                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                                            }
                                                            outsourceOrderDetailModel.ReceiveUnitPrice = order.order.UnitPrice;
                                                            outsourceOrderDetailModel.ReceiveTotalPrice = order.order.TotalPrice;
                                                            outsourceOrderDetailModel.RegionSupplementId = order.order.RegionSupplementId;
                                                            outsourceOrderDetailModel.CSUserId = order.order.CSUserId;
                                                            outsourceOrderDetailModel.OutsourceId = config.ProductOutsourctId;
                                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                            outsourceOrderDetailModel.FinalOrderId = order.order.Id;
                                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                            assignedOrderIdList.Add(order.order.Id);
                                                            //assignOrderCount++;
                                                        });
                                                    }
                                                }
                                            });
                                        }
                                        #endregion

                                        #region 其他材质订单
                                        var orderList1 = (from order in oneShopOrderListNew
                                                          //join subject in CurrentContext.DbContext.Subject
                                                          //on order.SubjectId equals subject.Id
                                                          where !assignedOrderIdList.Contains(order.order.Id)
                                                          select new
                                                          {
                                                              order = order.order,
                                                              subject = order.subject

                                                          }).ToList();
                                        if (orderList1.Any())
                                        {
                                            orderList1.ForEach(s =>
                                            {

                                                int Quantity = s.order.Quantity ?? 1;
                                                if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                                {
                                                    Quantity = Quantity > 0 ? 1 : 0;
                                                }
                                                string material0 = s.order.GraphicMaterial ?? "";
                                                if (s.order.Province == "天津")
                                                {
                                                    if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)//促销或发货
                                                    {


                                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                        outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                            pop.CustomerId = subjectModel.subject.CustomerId;
                                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                            if (guidanceType == (int)GuidanceTypeEnum.Delivery)
                                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
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
                                                            material = s.order.GraphicMaterial ?? string.Empty;


                                                        if (material.Contains("背胶PP+") && material.Contains("雪弗板") && !material.Contains("蝴蝶支架"))
                                                        {
                                                            string material1 = "背胶PP";


                                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                            outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                            if (!string.IsNullOrWhiteSpace(material1))
                                                            {
                                                                POP pop = new POP();
                                                                pop.GraphicMaterial = material1;
                                                                pop.GraphicLength = s.order.GraphicLength;
                                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                                pop.Quantity = Quantity;
                                                                pop.CustomerId = subjectModel.subject.CustomerId;
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
                                                            outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                                pop.CustomerId = subjectModel.subject.CustomerId;
                                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                                outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                                outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                                            }
                                                            //outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                                            //outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                                            //不算应收（算作北京）
                                                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                                            outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                            //addInstallPrice = true;

                                                        }
                                                        else
                                                        {



                                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                            outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                                pop.CustomerId = subjectModel.subject.CustomerId;
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
                                                                outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                                //if (s.order.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                                //hasInstallPrice = true;
                                                            }
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
                                                    outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                        pop.CustomerId = subjectModel.subject.CustomerId;
                                                        pop.OutsourceType = assignType;
                                                        if ((!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south")) || guidanceType == (int)GuidanceTypeEnum.Promotion)
                                                        {
                                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                        }
                                                        else if (guidanceType == (int)GuidanceTypeEnum.Delivery)
                                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
                                                        new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                        outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                        outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                                    }
                                                    outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                                    outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                                    outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                                    outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                                    if ((shop.BCSOutsourceId ?? 0) > 0 && s.subject.CornerType == "三叶草")
                                                    {
                                                        outsourceOrderDetailModel.OutsourceId = shop.BCSOutsourceId ?? 0;
                                                    }
                                                    else
                                                        outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;

                                                    outsourceOrderDetailModel.AssignType = assignType;
                                                    if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                                    {
                                                        //if (s.order.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                        //hasInstallPrice = true;
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
                                                //assignOrderCount++;
                                            });
                                        }
                                        #endregion
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 北京订单
                                        var orderList1 = oneShopOrderList;
                                        orderList1.ForEach(s =>
                                        {
                                            int Quantity = s.Quantity ?? 1;
                                            if (!string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                            {
                                                Quantity = Quantity > 0 ? 1 : 0;
                                            }
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = currUserId;
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
                                            if (!string.IsNullOrWhiteSpace(s.GraphicMaterial))
                                                material = new BasePage().GetBasicMaterial(s.GraphicMaterial);
                                            if (string.IsNullOrWhiteSpace(material))
                                                material = s.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = s.GraphicMaterial;
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
                                            decimal unitPrice1 = 0;
                                            decimal totalPrice1 = 0;
                                            if (!string.IsNullOrWhiteSpace(material))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material;
                                                pop.GraphicLength = s.GraphicLength;
                                                pop.GraphicWidth = s.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = subjectModel.subject.CustomerId;
                                                pop.OutsourceType = assignType;
                                                if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                            outsourceOrderDetailModel.AssignType = assignType;
                                            if (s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费 || s.OrderType == (int)OrderTypeEnum.其他费用)
                                            {
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                //if (s.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                //hasInstallPrice = true;

                                            }
                                            if (s.OrderType == (int)OrderTypeEnum.发货费)
                                            {
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            }
                                            if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                            {

                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.Sheet != null && (s.Sheet.Contains("橱窗") || s.Sheet.Contains("窗贴")) && s.GraphicLength > 1 && s.GraphicWidth > 1 && s.GraphicMaterial.Contains("全透贴"))
                                                {

                                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                    promotionInstallPrice = 150;
                                                }
                                            }
                                            outsourceOrderDetailModel.FinalOrderId = s.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                            //assignOrderCount++;
                                        });
                                        #endregion
                                    }

                                    #region 安装费和快递费
                                    if (guidanceType == (int)GuidanceTypeEnum.Others)
                                    {
                                        addInstallPrice = false;
                                    }
                                    else if (guidanceType == (int)GuidanceTypeEnum.Promotion && assignType == (int)OutsourceOrderTypeEnum.Install)
                                    {
                                        if (promotionInstallPrice > 0)
                                        {
                                            addInstallPrice = true;
                                            hasExpressPrice = false;
                                        }
                                        else
                                        {
                                            addInstallPrice = false;
                                        }
                                    }
                                    else if (guidanceType == (int)GuidanceTypeEnum.Install && (subjectModel.gudiance.HasInstallFees ?? true) && assignType == (int)OutsourceOrderTypeEnum.Install && popList.Any())
                                    {
                                        addInstallPrice = true;
                                    }
                                    if (shop.Channel != null && shop.Channel.ToLower().Contains("terrex"))
                                    {
                                        isInstallShop = false;
                                    }
                                    if (addInstallPrice && !hasInstallPrice && isInstallShop)
                                    {
                                        decimal receiveInstallPrice = 0;
                                        decimal installPrice = 0;
                                        string remark = "活动安装费";
                                        decimal oohInstallPrice = 0;
                                        decimal basicInstallPrice = 150;

                                        if (promotionInstallPrice > 0)
                                        {
                                            installPrice = promotionInstallPrice;
                                            receiveInstallPrice = promotionInstallPrice;
                                            remark = "促销窗贴安装费";
                                        }
                                        else
                                        {

                                            //按照级别，获取基础安装费
                                            //decimal basicInstallPrice = new BasePage().GetOutsourceBasicInstallPrice(materialSupport);

                                            //materialSupportList.ForEach(ma =>
                                            //{
                                            //    decimal basicInstallPrice0 = new BasePage().GetOutsourceBasicInstallPrice(ma);
                                            //    if (basicInstallPrice0 > basicInstallPrice)
                                            //    {
                                            //        basicInstallPrice = basicInstallPrice0;
                                            //        materialSupport = ma;
                                            //    }
                                            //});

                                            //var oohList = totalOrderList0.Where(s => (s.order.Sheet != null && (s.order.Sheet.Contains("户外") || s.order.Sheet.ToLower() == "ooh"))).ToList();
                                            var oohList = totalOrderList0.Where(s => (s.order.Sheet != null && (OOHInstallSheetList.Any() ? (OOHInstallSheetList.Contains(s.order.Sheet.ToUpper())) : (s.order.Sheet.Contains("户外") || s.order.Sheet.ToLower() == "ooh")))).ToList();

                                            if (oohList.Any())
                                            {

                                                Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                                                oohList.ForEach(s =>
                                                {
                                                    decimal price = 0;
                                                    if (!string.IsNullOrWhiteSpace(s.order.GraphicNo))
                                                    {
                                                        price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.order.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                                    }
                                                    else
                                                        price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.order.GraphicLength && p.GraphicWidth == s.order.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();
                                                    if (price > oohInstallPrice)
                                                    {
                                                        oohInstallPrice = price;
                                                    }
                                                });


                                            }

                                            //hasExtraInstallPrice = true;
                                            //添加安装费

                                            InstallPriceTempBLL installShopPriceBll = new InstallPriceTempBLL();
                                            var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == subjectModel.gudiance.ItemId && sh.ShopId == shop.Id);
                                            if (installShopList.Any())
                                            {
                                                installShopList.ForEach(sh =>
                                                {
                                                    receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                                                });
                                            }
                                            if ((shop.OutsourceInstallPrice ?? 0) > 0)
                                            {
                                                basicInstallPrice = shop.OutsourceInstallPrice ?? 0;
                                            }
                                            if (isBCSSubject)
                                            {
                                                if ((shop.OutsourceBCSInstallPrice ?? 0) > 0)
                                                {
                                                    basicInstallPrice = shop.OutsourceBCSInstallPrice ?? 0;
                                                }
                                                else if (BCSCityTierList.Contains(shop.CityTier.ToUpper()))
                                                {
                                                    basicInstallPrice = 150;
                                                }
                                                else
                                                {
                                                    basicInstallPrice = 0;
                                                }

                                            }
                                            if (isGeneric)
                                            {

                                                if (shop.CityName == "包头市" && (shop.OutsourceInstallPrice ?? 0) > 0)
                                                {
                                                    basicInstallPrice = shop.OutsourceInstallPrice ?? 0;
                                                }
                                                else if (BCSCityTierList.Contains(shop.CityTier.ToUpper()))
                                                {
                                                    basicInstallPrice = 150;
                                                }
                                                else
                                                {
                                                    basicInstallPrice = 0;
                                                }
                                            }
                                            installPrice = oohInstallPrice + basicInstallPrice;
                                        }
                                        if (installPrice > 0)
                                        {

                                            if (oohInstallPrice > 0 && (shop.OOHInstallOutsourceId ?? 0) > 0)
                                            {
                                                //如果有单独的户外安装外协
                                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                outsourceOrderDetailModel.AddUserId = currUserId;
                                                outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                                outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                                outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                                outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                                outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                                outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                                outsourceOrderDetailModel.Contact = shop.Contact1;
                                                outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                                outsourceOrderDetailModel.GraphicWidth = 0;
                                                outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                                outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                                outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                                outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                                outsourceOrderDetailModel.MaterialSupport = string.Empty;
                                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                                outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                                outsourceOrderDetailModel.POPName = string.Empty;
                                                outsourceOrderDetailModel.POPType = string.Empty;
                                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                                outsourceOrderDetailModel.POSScale = posScale;
                                                outsourceOrderDetailModel.Province = shop.ProvinceName;
                                                outsourceOrderDetailModel.Quantity = 1;
                                                outsourceOrderDetailModel.Region = shop.RegionName;
                                                outsourceOrderDetailModel.Remark = "户外安装费";
                                                outsourceOrderDetailModel.Sheet = string.Empty;
                                                outsourceOrderDetailModel.ShopId = shop.Id;
                                                outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                                outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                                outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                                outsourceOrderDetailModel.SubjectId = 0;
                                                outsourceOrderDetailModel.Tel = shop.Tel1;
                                                outsourceOrderDetailModel.TotalArea = 0;
                                                outsourceOrderDetailModel.WindowDeep = 0;
                                                outsourceOrderDetailModel.WindowHigh = 0;
                                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                                outsourceOrderDetailModel.WindowWide = 0;
                                                outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                                outsourceOrderDetailModel.PayOrderPrice = oohInstallPrice;
                                                outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                                outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                                outsourceOrderDetailModel.InstallPriceMaterialSupport = string.Empty;
                                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                                outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                                outsourceOrderDetailModel.OutsourceId = shop.OOHInstallOutsourceId;
                                                outsourceOrderDetailModel.InstallPriceAddType = 2;
                                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                installPrice = installPrice - oohInstallPrice;
                                                oohInstallPrice = 0;
                                            }
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = currUserId;
                                            outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                            outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                            outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                            outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                            outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                            outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                            outsourceOrderDetailModel.Contact = oneShopOrderList[0].Contact;
                                            outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                                            outsourceOrderDetailModel.GraphicWidth = 0;
                                            outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                            outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                                            outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                            outsourceOrderDetailModel.OrderGender = string.Empty;
                                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                            outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                            outsourceOrderDetailModel.POPName = string.Empty;
                                            outsourceOrderDetailModel.POPType = string.Empty;
                                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                                            outsourceOrderDetailModel.POSScale = posScale;
                                            outsourceOrderDetailModel.Province = shop.ProvinceName;
                                            outsourceOrderDetailModel.Quantity = 1;
                                            outsourceOrderDetailModel.Region = shop.RegionName;
                                            outsourceOrderDetailModel.Remark = remark;
                                            outsourceOrderDetailModel.Sheet = string.Empty;
                                            outsourceOrderDetailModel.ShopId = shop.Id;
                                            outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                            outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = 0;
                                            outsourceOrderDetailModel.Tel = shop.Tel1;
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
                                            outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                            outsourceOrderDetailModel.InstallPriceAddType = 2;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                        }
                                    }
                                    else if ((guidanceType == (int)GuidanceTypeEnum.Promotion && hasExpressPrice) || (guidanceType == (int)GuidanceTypeEnum.Delivery) && popList.Any())
                                    {
                                        expressPriceDetailModel = expressPriceDetailBll.GetList(price => price.GuidanceId == subjectModel.gudiance.ItemId && price.ShopId == shop.Id).FirstOrDefault();
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
                                        if (shop.ProvinceName == "内蒙古" && !shop.CityName.Contains("通辽"))
                                        {
                                            payExpressPrice = 0;
                                        }

                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = currUserId;
                                        outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                        outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                        outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                        outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                        outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                        outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                        outsourceOrderDetailModel.Contact = shop.Contact1;
                                        outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                        outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                        outsourceOrderDetailModel.GraphicNo = string.Empty;
                                        outsourceOrderDetailModel.GraphicWidth = 0;
                                        outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                        outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                        outsourceOrderDetailModel.MachineFrame = string.Empty;
                                        outsourceOrderDetailModel.MaterialSupport = string.Empty;
                                        outsourceOrderDetailModel.OrderGender = string.Empty;
                                        outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.发货费;
                                        outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                        outsourceOrderDetailModel.POPName = string.Empty;
                                        outsourceOrderDetailModel.POPType = string.Empty;
                                        outsourceOrderDetailModel.PositionDescription = string.Empty;
                                        outsourceOrderDetailModel.POSScale = posScale;
                                        outsourceOrderDetailModel.Province = shop.ProvinceName;
                                        outsourceOrderDetailModel.Quantity = 1;
                                        outsourceOrderDetailModel.Region = shop.RegionName;
                                        outsourceOrderDetailModel.Remark = string.Empty;
                                        outsourceOrderDetailModel.Sheet = string.Empty;
                                        outsourceOrderDetailModel.ShopId = shop.Id;
                                        outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                        outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = 0;
                                        outsourceOrderDetailModel.Tel = shop.Tel1;
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
                                        outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                        outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                        if (shop.ProvinceName == "天津")
                                            outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);

                                    }
                                    #endregion
                                }

                            });

                        }
                    }
                    //tran.Complete();
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    isSave = false;
                }
                //}
                if (!isSave)
                {
                    OutsourceOrderSavingLogBLL logBll = new OutsourceOrderSavingLogBLL();
                    OutsourceOrderSavingLog logModel = new OutsourceOrderSavingLog();
                    logModel.AddDate = DateTime.Now;
                    string msg = errorMsg;
                    logModel.ErrorMsg = msg.Length > 400 ? (msg.Substring(0, 400)) : msg;
                    logModel.GuidanceId = guidanceId;
                    logModel.State = "操作失败";
                    logModel.SubjectId = subjectId;
                    logBll.Add(logModel);
                }
            }
        }

        void SaveHandler1_old(int guidanceId, int subjectId, int subjectType)
        {

            SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(guidanceId);
            if (guidanceModel != null)
            {
                InstallPriceShopInfoBLL installPriceShopInfoBll = new InstallPriceShopInfoBLL();
                List<InstallPriceShopInfo> installPriceShopInfoList = installPriceShopInfoBll.GetList(s => s.GuidanceId == guidanceId);
                bool isSave = true;
                string errorMsg = string.Empty;
                //using (TransactionScope tran = new TransactionScope())
                //{
                try
                {
                    if ((subjectType != (int)SubjectTypeEnum.二次安装 && subjectType != (int)SubjectTypeEnum.费用订单))
                    {
                        if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guidanceModel.HasInstallFees ?? true))
                        {
                            new BasePage().SaveInstallPrice(guidanceId, subjectId, subjectType);
                            UpdateInstallOrderRedisDataHandler(subjectId);

                        }
                        else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guidanceModel.HasExperssFees ?? true))
                        {
                            new BasePage().SaveExpressPrice(guidanceId, subjectId, subjectType);
                        }
                        else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery)
                        {
                            new BasePage().SaveExpressPriceForDelivery(guidanceId, subjectId, subjectType, guidanceModel.ExperssPrice);
                        }
                    }
                    //保存报价单
                    new BasePage().SaveQuotationOrder(guidanceId, subjectId, subjectType);

                    #region 保存外协订单
                    var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                        on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                        from subjectCategory in categortTemp.DefaultIfEmpty()
                                        join gudiance in CurrentContext.DbContext.SubjectGuidance
                                        on subject.GuidanceId equals gudiance.ItemId
                                        where subject.Id == subjectId
                                        select new
                                        {
                                            subject,
                                            gudiance,
                                            CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                        }).FirstOrDefault();
                    if (subjectModel != null)
                    {
                        //保存外协订单
                        int guidanceType = subjectModel.gudiance.ActivityTypeId ?? 0;
                        List<FinalOrderDetailTemp> orderList0 = new List<FinalOrderDetailTemp>();
                        FinalOrderDetailTempBLL OrderDetailTempBll = new FinalOrderDetailTempBLL();
                        OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
                        OutsourceOrderDetail outsourceOrderDetailModel;
                        if (subjectType == (int)SubjectTypeEnum.HC订单 || subjectType == (int)SubjectTypeEnum.分区补单 || subjectType == (int)SubjectTypeEnum.分区增补 || subjectType == (int)SubjectTypeEnum.新开店订单)
                        {
                            orderList0 = OrderDetailTempBll.GetList(s => s.RegionSupplementId == subjectId && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType > 1)) && (s.OrderType != (int)OrderTypeEnum.物料));
                            //删除旧POP订单数据
                            outsourceOrderDetailBll.Delete(s => s.RegionSupplementId == subjectId);
                        }
                        else
                        {
                            orderList0 = OrderDetailTempBll.GetList(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0 && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType > 1)) && (s.OrderType != (int)OrderTypeEnum.物料));
                            //删除旧POP订单数据
                            outsourceOrderDetailBll.Delete(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0);
                        }
                        if (orderList0.Any())
                        {

                            bool isProductInNorth = false;
                            if (subjectModel.subject.PriceBlongRegion != null && subjectModel.subject.PriceBlongRegion.ToLower() == "north")
                            {
                                isProductInNorth = true;
                            }


                            List<ExpressPriceConfig> expressPriceConfigList = new ExpressPriceConfigBLL().GetList(s => s.Id > 0);
                            ExpressPriceDetailBLL expressPriceDetailBll = new ExpressPriceDetailBLL();
                            ExpressPriceDetail expressPriceDetailModel;

                            List<OutsourceOrderAssignConfig> configList = new OutsourceOrderAssignConfigBLL().GetList(s => s.Id > 0);
                            List<Place> placeList = new PlaceBLL().GetList(s => s.ID > 0);
                            List<int> shopIdList = orderList0.Select(s => s.ShopId ?? 0).ToList();
                            List<Shop> shopList = new ShopBLL().GetList(s => shopIdList.Contains(s.Id)).ToList();
                            //List<POP> oohPOPList = new POPBLL().GetList(pop => shopIdList.Contains(pop.ShopId ?? 0) && (pop.Sheet == "户外" || pop.Sheet.ToLower() == "ooh") && (pop.OOHInstallPrice ?? 0) > 0);
                            List<string> BCSCityTierList = new List<string>() { "T1", "T2", "T3" };
                            //List<FinalOrderDetailTemp> totalOrderList = OrderDetailTempBll.GetList(s => s.GuidanceId == subjectModel.gudiance.ItemId && shopIdList.Contains(s.ShopId ?? 0) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType == (int)OrderTypeEnum.道具)) && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true));
                            List<FinalOrderDetailTemp> totalOrderList = new List<FinalOrderDetailTemp>();
                            totalOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                              join subject in CurrentContext.DbContext.Subject
                                              on order.SubjectId equals subject.Id
                                              where order.GuidanceId == subjectModel.gudiance.ItemId
                                              && shopIdList.Contains(order.ShopId ?? 0)
                                              && ((order.OrderType == (int)OrderTypeEnum.POP
                                              && order.GraphicLength > 1 && order.GraphicWidth > 1) || (order.OrderType == (int)OrderTypeEnum.道具))
                                              && (order.IsDelete == null || order.IsDelete == false)
                                              && (order.IsValid == null || order.IsValid == true)
                                              && (subject.IsSecondInstall ?? false) == false
                                              select order).ToList();



                            string changePOPCountSheetStr = string.Empty;
                            string beiJingCalerOutsourceName = string.Empty;
                            string OOHInstallSheetName = string.Empty;
                            int calerOutsourceId = 8;
                            List<string> ChangePOPCountSheetList = new List<string>();
                            List<string> OOHInstallSheetList = new List<string>();
                            try
                            {
                                changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                                beiJingCalerOutsourceName = ConfigurationManager.AppSettings["CalerOutsourceName"];
                                OOHInstallSheetName = ConfigurationManager.AppSettings["OOHInstallSheet"];
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
                                Company companyModel = new CompanyBLL().GetList(s => (s.CompanyName == beiJingCalerOutsourceName || s.ShortName == beiJingCalerOutsourceName) && s.TypeId == (int)CompanyTypeEnum.Outsource).FirstOrDefault();
                                if (companyModel != null)
                                    calerOutsourceId = companyModel.Id;
                            }
                            if (!string.IsNullOrWhiteSpace(OOHInstallSheetName))
                            {
                                OOHInstallSheetList = StringHelper.ToStringList(OOHInstallSheetName, ',', LowerUpperEnum.ToUpper);
                            }
                            List<POP> oohPOPList = new POPBLL().GetList(pop => shopIdList.Contains(pop.ShopId ?? 0) && (OOHInstallSheetList.Any() ? (OOHInstallSheetList.Contains(pop.Sheet.ToUpper())) : (pop.Sheet == "户外" || pop.Sheet.ToLower() == "ooh")) && (pop.OSOOHInstallPrice ?? 0) > 0);

                            //删除安装费和发货费
                            outsourceOrderDetailBll.Delete(s => s.GuidanceId == subjectModel.gudiance.ItemId && shopIdList.Contains(s.ShopId ?? 0) && s.SubjectId == 0 && (s.InstallPriceAddType ?? 1) == 1);
                            //var assignedList = outsourceOrderDetailBll.GetList(s => s.GuidanceId == subjectModel.gudiance.ItemId && s.SubjectId > 0);
                            var assignedList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                                join subject in CurrentContext.DbContext.Subject
                                                on order.SubjectId equals subject.Id
                                                where order.GuidanceId == subjectModel.gudiance.ItemId
                                                && order.SubjectId > 0
                                                && shopIdList.Contains(order.ShopId ?? 0)
                                                && ((order.OrderType == (int)OrderTypeEnum.POP
                                                && order.GraphicLength > 1 && order.GraphicWidth > 1) || (order.OrderType == (int)OrderTypeEnum.道具))
                                                && (order.IsDelete == null || order.IsDelete == false)
                                                && (subject.IsSecondInstall ?? false) == false
                                                select order).ToList();
                            shopList.ForEach(shop =>
                            {

                                bool isInstallShop = shop.IsInstall == "Y";
                                bool isBCSInstallShop = shop.BCSIsInstall == "Y";//三叶草是否安装
                                //if (shop.Id == 709)
                                //{
                                //int iddd = 0;
                                //}
                                bool hasInstallPrice = false;
                                bool addInstallPrice = false;

                                List<string> materialSupportList = new List<string>();
                                string posScale = string.Empty;
                                decimal promotionInstallPrice = 0;
                                bool hasExpressPrice = subjectModel.gudiance.HasExperssFees ?? true;
                                //bool hasInstallPrice=subjectModel.gudiance.HasInstallFees ?? true;
                                var oneShopOrderList = orderList0.Where(order => order.ShopId == shop.Id).ToList();
                                //去重
                                List<FinalOrderDetailTemp> tempOrderList = new List<FinalOrderDetailTemp>();
                                oneShopOrderList.ForEach(s =>
                                {

                                    bool canGo = true;

                                    if (!string.IsNullOrWhiteSpace(s.GraphicNo) && !string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                    {
                                        //string gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                        //去掉重复的（同一个编号下多次的）
                                        var checkList = tempOrderList.Where(sl => sl.Sheet == s.Sheet && sl.PositionDescription == s.PositionDescription && sl.GraphicNo == s.GraphicNo && sl.GraphicLength == s.GraphicLength && sl.GraphicWidth == s.GraphicWidth && (sl.Gender == s.Gender || sl.OrderGender == s.OrderGender)).ToList();

                                        if (checkList.Any())
                                        {
                                            canGo = false;
                                        }
                                        else
                                        {
                                            var oneShopAssignedList = assignedList.Where(assign => assign.ShopId == s.ShopId).ToList();
                                            foreach (var assign in oneShopAssignedList)
                                            {
                                                if (assign.Sheet == s.Sheet && assign.PositionDescription == s.PositionDescription && assign.GraphicNo == s.GraphicNo && assign.GraphicLength == s.GraphicLength && assign.GraphicWidth == s.GraphicWidth && (assign.Gender == s.Gender || assign.Gender == s.OrderGender))
                                                {
                                                    canGo = false;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                    if (canGo)
                                    {
                                        tempOrderList.Add(s);

                                    }


                                });
                                oneShopOrderList = tempOrderList;
                                if (oneShopOrderList.Any())
                                {
                                    var popList = totalOrderList.Where(s => s.ShopId == shop.Id);
                                    //单店全部订单
                                    var totalOrderList0 = (from order in popList
                                                           join subject in CurrentContext.DbContext.Subject
                                                           on order.SubjectId equals subject.Id
                                                           join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                                          on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                                           from subjectCategory in categortTemp.DefaultIfEmpty()
                                                           where (subject.IsDelete == null || subject.IsDelete == false)
                                                           && subject.ApproveState == 1
                                                           && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                                           && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                                                           && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                                                           select new
                                                           {
                                                               order,
                                                               subject,
                                                               CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                                           }).ToList();
                                    bool isBCSSubject = true;
                                    bool isContainsNotGeneric = false;
                                    bool isGeneric = false;
                                    totalOrderList0.ForEach(order =>
                                    {
                                        if (order.CategoryName.Contains("常规-非活动"))
                                            isGeneric = true;
                                        else
                                        {
                                            isContainsNotGeneric = true;
                                            if (isBCSInstallShop && !isInstallShop)
                                            {
                                                if (order.subject.CornerType == "三叶草")
                                                    isBCSSubject = true;
                                            }
                                            else
                                            {
                                                if (order.subject.CornerType != "三叶草")
                                                    isBCSSubject = false;
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(order.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(order.order.InstallPriceMaterialSupport.ToLower()))
                                        {
                                            materialSupportList.Add(order.order.InstallPriceMaterialSupport.ToLower());
                                        }
                                        if (string.IsNullOrWhiteSpace(posScale))
                                            posScale = order.order.InstallPricePOSScale;
                                    });
                                    int assignType = 0;
                                    if (shop.ProvinceName == "内蒙古" && !shop.CityName.Contains("通辽"))
                                    {
                                        assignType = (int)OutsourceOrderTypeEnum.Install;

                                    }
                                    else
                                    {
                                        if (guidanceType == (int)GuidanceTypeEnum.Install)
                                        {
                                            if (isBCSSubject)
                                            {
                                                assignType = shop.BCSIsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                                                isInstallShop = shop.BCSIsInstall == "Y";
                                            }
                                            else
                                            {
                                                assignType = shop.IsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                                            }
                                        }
                                        else
                                        {
                                            assignType = shop.IsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                                        }
                                    }
                                    var oneShopOrderListNew = (from order in oneShopOrderList
                                                               join subject in CurrentContext.DbContext.Subject
                                                               on order.SubjectId equals subject.Id
                                                               select new
                                                               {
                                                                   order,
                                                                   subject
                                                               }).ToList();
                                    if (!shop.ProvinceName.Contains("北京"))
                                    {
                                        #region 非北京订单
                                        List<int> assignedOrderIdList = new List<int>();
                                        #region 按设置好的材质
                                        var materialConfig = configList.Where(s => s.ConfigTypeId == (int)OutsourceOrderConfigType.Material).ToList();
                                        if (materialConfig.Any())
                                        {

                                            materialConfig.ForEach(config =>
                                            {
                                                OutsourceOrderPlaceConfigBLL placeConfigBll = new OutsourceOrderPlaceConfigBLL();
                                                if (!string.IsNullOrWhiteSpace(config.MaterialName))
                                                {
                                                    var orderList = oneShopOrderListNew.Where(order => !assignedOrderIdList.Contains(order.order.Id) && order.order.GraphicMaterial != null && ((config.IsFullMatch ?? false) ? (order.order.GraphicMaterial.ToLower() == config.MaterialName.ToLower()) : (order.order.GraphicMaterial.ToLower().Contains(config.MaterialName.ToLower()))) && (order.order.OrderType == (int)OrderTypeEnum.POP || order.order.OrderType == (int)OrderTypeEnum.道具)).ToList();

                                                    List<int> cityIdList = new List<int>();

                                                    List<string> cityNameList = new List<string>();
                                                    bool hasProvince = false;
                                                    bool canGo = false;
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
                                                        hasProvince = true;
                                                        placeConfigList.ForEach(pc =>
                                                        {
                                                            if (pc.PlaceName == shop.ProvinceName)
                                                            {
                                                                orderList = orderList.Where(order => order.order.Province == pc.PlaceName).ToList();
                                                                if (!string.IsNullOrWhiteSpace(pc.CityIds))
                                                                {
                                                                    cityIdList = StringHelper.ToIntList(pc.CityIds, ',');
                                                                    cityNameList = placeList.Where(p => cityIdList.Contains(p.ID)).Select(p => p.PlaceName).ToList();
                                                                    orderList = orderList.Where(order => cityNameList.Contains(order.order.City)).ToList();
                                                                }
                                                                canGo = true;
                                                            }
                                                        });
                                                    }
                                                    if (hasProvince && !canGo)
                                                        orderList.Clear();
                                                    if (!string.IsNullOrWhiteSpace(config.Channel))
                                                    {
                                                        List<string> channelList = StringHelper.ToStringList(config.Channel, ',', LowerUpperEnum.ToLower);
                                                        if (channelList.Any())
                                                        {
                                                            orderList = orderList.Where(order => order.order.Channel != null && channelList.Contains(order.order.Channel.ToLower())).ToList();
                                                        }
                                                    }
                                                    if (orderList.Any())
                                                    {
                                                        orderList.ForEach(order =>
                                                        {
                                                            string material0 = order.order.GraphicMaterial;
                                                            int Quantity = order.order.Quantity ?? 1;
                                                            if (!string.IsNullOrWhiteSpace(order.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(order.order.Sheet.ToUpper()))
                                                            {
                                                                Quantity = Quantity > 0 ? 1 : 0;
                                                            }
                                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                            outsourceOrderDetailModel.AddUserId = currUserId;
                                                            outsourceOrderDetailModel.AgentCode = order.order.AgentCode;
                                                            outsourceOrderDetailModel.AgentName = order.order.AgentName;
                                                            outsourceOrderDetailModel.Area = order.order.Area;
                                                            outsourceOrderDetailModel.BusinessModel = order.order.BusinessModel;
                                                            outsourceOrderDetailModel.Channel = order.order.Channel;
                                                            outsourceOrderDetailModel.ChooseImg = order.order.ChooseImg;
                                                            outsourceOrderDetailModel.City = order.order.City;
                                                            outsourceOrderDetailModel.CityTier = order.order.CityTier;
                                                            outsourceOrderDetailModel.Contact = order.order.Contact;
                                                            outsourceOrderDetailModel.CornerType = order.order.CornerType;
                                                            outsourceOrderDetailModel.Format = order.order.Format;
                                                            outsourceOrderDetailModel.Gender = (order.order.OrderGender != null && order.order.OrderGender != "") ? order.order.OrderGender : order.order.Gender;
                                                            outsourceOrderDetailModel.GraphicLength = order.order.GraphicLength;
                                                            outsourceOrderDetailModel.OrderGraphicMaterial = order.order.GraphicMaterial;
                                                            string material = string.Empty;
                                                            if (!string.IsNullOrWhiteSpace(material0))
                                                                material = new BasePage().GetBasicMaterial(material0);
                                                            if (string.IsNullOrWhiteSpace(material))
                                                                material = order.order.GraphicMaterial;
                                                            outsourceOrderDetailModel.GraphicMaterial = material;
                                                            outsourceOrderDetailModel.GraphicNo = order.order.GraphicNo;
                                                            outsourceOrderDetailModel.GraphicWidth = order.order.GraphicWidth;
                                                            outsourceOrderDetailModel.GuidanceId = order.order.GuidanceId;
                                                            outsourceOrderDetailModel.IsInstall = order.order.IsInstall;
                                                            outsourceOrderDetailModel.BCSIsInstall = order.order.BCSIsInstall;
                                                            outsourceOrderDetailModel.LocationType = order.order.LocationType;
                                                            outsourceOrderDetailModel.MachineFrame = order.order.MachineFrame;
                                                            outsourceOrderDetailModel.MaterialSupport = order.order.MaterialSupport;
                                                            outsourceOrderDetailModel.OrderGender = order.order.OrderGender;
                                                            outsourceOrderDetailModel.OrderType = order.order.OrderType;
                                                            outsourceOrderDetailModel.POPAddress = order.order.POPAddress;
                                                            outsourceOrderDetailModel.POPName = order.order.POPName;
                                                            outsourceOrderDetailModel.POPType = order.order.POPType;
                                                            outsourceOrderDetailModel.PositionDescription = order.order.PositionDescription;
                                                            outsourceOrderDetailModel.POSScale = order.order.POSScale;
                                                            outsourceOrderDetailModel.Province = order.order.Province;
                                                            outsourceOrderDetailModel.Quantity = Quantity;
                                                            outsourceOrderDetailModel.Region = order.order.Region;
                                                            outsourceOrderDetailModel.Remark = order.order.Remark;
                                                            outsourceOrderDetailModel.Sheet = order.order.Sheet;
                                                            outsourceOrderDetailModel.ShopId = order.order.ShopId;
                                                            outsourceOrderDetailModel.ShopName = order.order.ShopName;
                                                            outsourceOrderDetailModel.ShopNo = order.order.ShopNo;
                                                            outsourceOrderDetailModel.ShopStatus = order.order.ShopStatus;
                                                            outsourceOrderDetailModel.SubjectId = order.order.SubjectId;
                                                            outsourceOrderDetailModel.Tel = order.order.Tel;
                                                            outsourceOrderDetailModel.TotalArea = order.order.TotalArea;
                                                            outsourceOrderDetailModel.WindowDeep = order.order.WindowDeep;
                                                            outsourceOrderDetailModel.WindowHigh = order.order.WindowHigh;
                                                            outsourceOrderDetailModel.WindowSize = order.order.WindowSize;
                                                            outsourceOrderDetailModel.WindowWide = order.order.WindowWide;
                                                            outsourceOrderDetailModel.ReceiveOrderPrice = order.order.OrderPrice;
                                                            outsourceOrderDetailModel.PayOrderPrice = order.order.PayOrderPrice;
                                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = order.order.InstallPriceMaterialSupport;
                                                            decimal unitPrice = 0;
                                                            decimal totalPrice = 0;
                                                            if (!string.IsNullOrWhiteSpace(material))
                                                            {
                                                                POP pop = new POP();
                                                                pop.GraphicMaterial = material;
                                                                pop.GraphicLength = order.order.GraphicLength;
                                                                pop.GraphicWidth = order.order.GraphicWidth;
                                                                pop.Quantity = Quantity;
                                                                pop.CustomerId = subjectModel.subject.CustomerId;
                                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                                            }
                                                            outsourceOrderDetailModel.ReceiveUnitPrice = order.order.UnitPrice;
                                                            outsourceOrderDetailModel.ReceiveTotalPrice = order.order.TotalPrice;
                                                            outsourceOrderDetailModel.RegionSupplementId = order.order.RegionSupplementId;
                                                            outsourceOrderDetailModel.CSUserId = order.order.CSUserId;
                                                            outsourceOrderDetailModel.OutsourceId = config.ProductOutsourctId;
                                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                            outsourceOrderDetailModel.FinalOrderId = order.order.Id;
                                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                            assignedOrderIdList.Add(order.order.Id);
                                                            //assignOrderCount++;
                                                        });
                                                    }
                                                }
                                            });
                                        }
                                        #endregion

                                        #region 其他材质订单
                                        var orderList1 = (from order in oneShopOrderListNew
                                                          //join subject in CurrentContext.DbContext.Subject
                                                          //on order.SubjectId equals subject.Id
                                                          where !assignedOrderIdList.Contains(order.order.Id)
                                                          select new
                                                          {
                                                              order = order.order,
                                                              subject = order.subject

                                                          }).ToList();
                                        if (orderList1.Any())
                                        {
                                            orderList1.ForEach(s =>
                                            {

                                                int Quantity = s.order.Quantity ?? 1;
                                                if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                                {
                                                    Quantity = Quantity > 0 ? 1 : 0;
                                                }
                                                string material0 = s.order.GraphicMaterial ?? "";
                                                if (s.order.Province == "天津")
                                                {
                                                    if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)//促销或发货
                                                    {


                                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                        outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                            pop.GraphicMaterial = material;
                                                            pop.GraphicLength = s.order.GraphicLength;
                                                            pop.GraphicWidth = s.order.GraphicWidth;
                                                            pop.Quantity = Quantity;
                                                            pop.CustomerId = subjectModel.subject.CustomerId;
                                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                            if (guidanceType == (int)GuidanceTypeEnum.Delivery)
                                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
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
                                                            material = s.order.GraphicMaterial ?? string.Empty;


                                                        if (material.Contains("背胶PP+") && material.Contains("雪弗板") && !material.Contains("蝴蝶支架"))
                                                        {
                                                            string material1 = "背胶PP";


                                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                            outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                            if (!string.IsNullOrWhiteSpace(material1))
                                                            {
                                                                POP pop = new POP();
                                                                pop.GraphicMaterial = material1;
                                                                pop.GraphicLength = s.order.GraphicLength;
                                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                                pop.Quantity = Quantity;
                                                                pop.CustomerId = subjectModel.subject.CustomerId;
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
                                                            outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                                pop.CustomerId = subjectModel.subject.CustomerId;
                                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                                outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                                outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                                            }
                                                            //outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                                            //outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                                            //不算应收（算作北京）
                                                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                                            outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                            //addInstallPrice = true;

                                                        }
                                                        else
                                                        {



                                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                            outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                                pop.CustomerId = subjectModel.subject.CustomerId;
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
                                                                outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                                //if (s.order.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                                //hasInstallPrice = true;
                                                            }
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
                                                    outsourceOrderDetailModel.AddUserId = currUserId;
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
                                                        pop.CustomerId = subjectModel.subject.CustomerId;
                                                        pop.OutsourceType = assignType;
                                                        if ((!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south")) || guidanceType == (int)GuidanceTypeEnum.Promotion)
                                                        {
                                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                        }
                                                        else if (guidanceType == (int)GuidanceTypeEnum.Delivery)
                                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
                                                        new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                        outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                        outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                                    }
                                                    outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                                    outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                                    outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                                    outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                                    if ((shop.BCSOutsourceId ?? 0) > 0 && s.subject.CornerType == "三叶草")
                                                    {
                                                        outsourceOrderDetailModel.OutsourceId = shop.BCSOutsourceId ?? 0;
                                                    }
                                                    else
                                                        outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;

                                                    outsourceOrderDetailModel.AssignType = assignType;
                                                    if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                                    {
                                                        //if (s.order.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                        //hasInstallPrice = true;
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
                                                    if ((shop.ProductOutsourceId ?? 0) > 0)
                                                    {
                                                        outsourceOrderDetailModel.OutsourceId = shop.ProductOutsourceId;
                                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                    }
                                                    if (isProductInNorth && shop.RegionName != null && shop.RegionName.ToLower() != subjectModel.subject.PriceBlongRegion.ToLower())
                                                    {
                                                        outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                    }

                                                    outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                                    outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                }
                                                //assignOrderCount++;
                                            });
                                        }
                                        #endregion
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 北京订单
                                        var orderList1 = oneShopOrderList;
                                        orderList1.ForEach(s =>
                                        {
                                            int Quantity = s.Quantity ?? 1;
                                            if (!string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                            {
                                                Quantity = Quantity > 0 ? 1 : 0;
                                            }
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = currUserId;
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
                                            if (!string.IsNullOrWhiteSpace(s.GraphicMaterial))
                                                material = new BasePage().GetBasicMaterial(s.GraphicMaterial);
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
                                            decimal unitPrice1 = 0;
                                            decimal totalPrice1 = 0;
                                            if (!string.IsNullOrWhiteSpace(material))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material;
                                                pop.GraphicLength = s.GraphicLength;
                                                pop.GraphicWidth = s.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = subjectModel.subject.CustomerId;
                                                pop.OutsourceType = assignType;
                                                if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                            outsourceOrderDetailModel.AssignType = assignType;
                                            if (s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费 || s.OrderType == (int)OrderTypeEnum.其他费用)
                                            {
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                //if (s.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                //hasInstallPrice = true;

                                            }
                                            if (s.OrderType == (int)OrderTypeEnum.发货费)
                                            {
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            }
                                            if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                            {

                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.Sheet != null && (s.Sheet.Contains("橱窗") || s.Sheet.Contains("窗贴")) && s.GraphicLength > 1 && s.GraphicWidth > 1 && s.GraphicMaterial.Contains("全透贴"))
                                                {

                                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                    promotionInstallPrice = 150;
                                                }
                                            }
                                            outsourceOrderDetailModel.FinalOrderId = s.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                            //assignOrderCount++;
                                        });
                                        #endregion
                                    }

                                    #region 安装费和快递费
                                    if (guidanceType == (int)GuidanceTypeEnum.Others)
                                    {
                                        addInstallPrice = false;
                                    }
                                    else if (guidanceType == (int)GuidanceTypeEnum.Promotion && assignType == (int)OutsourceOrderTypeEnum.Install)
                                    {
                                        if (promotionInstallPrice > 0)
                                        {
                                            addInstallPrice = true;
                                            hasExpressPrice = false;
                                        }
                                        else
                                        {
                                            addInstallPrice = false;
                                        }
                                    }
                                    else if (guidanceType == (int)GuidanceTypeEnum.Install && (subjectModel.gudiance.HasInstallFees ?? true) && assignType == (int)OutsourceOrderTypeEnum.Install && popList.Any())
                                    {
                                        addInstallPrice = true;
                                    }
                                    //户外店不计算安装费，统一导入
                                    if (shop.Channel != null && shop.Channel.ToLower().Contains("terrex") && shop.RegionName != null && shop.RegionName.ToLower().Contains("east"))
                                    {
                                        isInstallShop = false;
                                    }
                                    if (addInstallPrice && !hasInstallPrice && isInstallShop)
                                    {
                                        decimal receiveInstallPrice = 0;
                                        decimal genericReceiveInstallPrice = 0;
                                        decimal installPrice = 0;
                                        string remark = "活动安装费";
                                        decimal oohInstallPrice = 0;
                                        decimal basicInstallPrice = 0;
                                        decimal genericBasicInstallPrice = 0;
                                        string materialSupport = string.Empty;
                                        if (promotionInstallPrice > 0)
                                        {
                                            installPrice = promotionInstallPrice;
                                            receiveInstallPrice = promotionInstallPrice;
                                            remark = "促销窗贴安装费";
                                        }
                                        else if (isContainsNotGeneric)
                                        {

                                            //按照级别，获取基础安装费
                                            //decimal basicInstallPrice = new BasePage().GetOutsourceBasicInstallPrice(materialSupport);

                                            materialSupportList.ForEach(ma =>
                                            {
                                                decimal basicInstallPrice0 = new BasePage().GetOutsourceBasicInstallPrice(ma);
                                                if (basicInstallPrice0 > basicInstallPrice)
                                                {
                                                    basicInstallPrice = basicInstallPrice0;
                                                    materialSupport = ma;
                                                }
                                            });

                                            //var oohList = totalOrderList0.Where(s => (s.order.Sheet != null && (s.order.Sheet.Contains("户外") || s.order.Sheet.ToLower() == "ooh"))).ToList();
                                            var oohList = totalOrderList0.Where(s => (s.order.Sheet != null && (OOHInstallSheetList.Any() ? (OOHInstallSheetList.Contains(s.order.Sheet.ToUpper())) : (s.order.Sheet.Contains("户外") || s.order.Sheet.ToLower() == "ooh")))).ToList();

                                            if (oohList.Any())
                                            {

                                                Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                                                oohList.ForEach(s =>
                                                {
                                                    decimal price = 0;
                                                    if (!string.IsNullOrWhiteSpace(s.order.GraphicNo))
                                                    {
                                                        price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.order.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                                    }
                                                    else
                                                        price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.order.GraphicLength && p.GraphicWidth == s.order.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();
                                                    if (price > oohInstallPrice)
                                                    {
                                                        oohInstallPrice = price;
                                                    }
                                                });


                                            }
                                            InstallPriceTempBLL installShopPriceBll = new InstallPriceTempBLL();
                                            var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == subjectModel.gudiance.ItemId && sh.ShopId == shop.Id);
                                            if (installShopList.Any())
                                            {
                                                installShopList.Where(sh => sh.SubjectType == null || sh.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费).ToList().ForEach(sh =>
                                                {
                                                    receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                                                });
                                                installShopList.Where(sh => sh.SubjectType != null && sh.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费).ToList().ForEach(sh =>
                                                {
                                                    genericReceiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                                                });
                                            }
                                            if ((shop.OutsourceInstallPrice ?? 0) > 0)
                                            {
                                                basicInstallPrice = shop.OutsourceInstallPrice ?? 0;
                                            }
                                            if (isBCSSubject)
                                            {
                                                if ((shop.OutsourceBCSInstallPrice ?? 0) > 0)
                                                {
                                                    basicInstallPrice = shop.OutsourceBCSInstallPrice ?? 0;
                                                }
                                                else if (BCSCityTierList.Contains(shop.CityTier.ToUpper()))
                                                {
                                                    basicInstallPrice = 150;
                                                }
                                                else
                                                {
                                                    basicInstallPrice = 0;
                                                }

                                            }

                                            installPrice = oohInstallPrice + basicInstallPrice;
                                        }
                                        if (isGeneric)
                                        {

                                            if (shop.CityName == "包头市" && (shop.OutsourceInstallPrice ?? 0) > 0)
                                            {
                                                genericBasicInstallPrice = shop.OutsourceInstallPrice ?? 0;
                                            }
                                            else if (BCSCityTierList.Contains(shop.CityTier.ToUpper()))
                                            {
                                                genericBasicInstallPrice = 150;
                                            }
                                            else
                                            {
                                                genericBasicInstallPrice = 0;
                                            }
                                        }
                                        if (installPrice > 0)
                                        {

                                            if (oohInstallPrice > 0 && (shop.OOHInstallOutsourceId ?? 0) > 0)
                                            {
                                                //如果有单独的户外安装外协
                                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                outsourceOrderDetailModel.AddUserId = currUserId;
                                                outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                                outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                                outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                                outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                                outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                                outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                                outsourceOrderDetailModel.Contact = shop.Contact1;
                                                outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                                outsourceOrderDetailModel.GraphicWidth = 0;
                                                outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                                outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                                outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                                outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                                outsourceOrderDetailModel.MaterialSupport = string.Empty;
                                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                                outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                                outsourceOrderDetailModel.POPName = string.Empty;
                                                outsourceOrderDetailModel.POPType = string.Empty;
                                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                                outsourceOrderDetailModel.POSScale = posScale;
                                                outsourceOrderDetailModel.Province = shop.ProvinceName;
                                                outsourceOrderDetailModel.Quantity = 1;
                                                outsourceOrderDetailModel.Region = shop.RegionName;
                                                outsourceOrderDetailModel.Remark = "户外安装费";
                                                outsourceOrderDetailModel.Sheet = string.Empty;
                                                outsourceOrderDetailModel.ShopId = shop.Id;
                                                outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                                outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                                outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                                outsourceOrderDetailModel.SubjectId = 0;
                                                outsourceOrderDetailModel.Tel = shop.Tel1;
                                                outsourceOrderDetailModel.TotalArea = 0;
                                                outsourceOrderDetailModel.WindowDeep = 0;
                                                outsourceOrderDetailModel.WindowHigh = 0;
                                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                                outsourceOrderDetailModel.WindowWide = 0;
                                                outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                                outsourceOrderDetailModel.PayOrderPrice = oohInstallPrice;
                                                outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                                outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                                outsourceOrderDetailModel.InstallPriceMaterialSupport = string.Empty;
                                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                                outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                                outsourceOrderDetailModel.OutsourceId = shop.OOHInstallOutsourceId;
                                                outsourceOrderDetailModel.InstallPriceAddType = 1;
                                                outsourceOrderDetailModel.InstallPriceSubjectType = (int)InstallPriceSubjectTypeEnum.活动安装费;
                                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                installPrice = installPrice - oohInstallPrice;
                                                oohInstallPrice = 0;
                                            }
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = currUserId;
                                            outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                            outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                            outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                            outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                            outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                            outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                            outsourceOrderDetailModel.Contact = oneShopOrderList[0].Contact;
                                            outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                                            outsourceOrderDetailModel.GraphicWidth = 0;
                                            outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                            outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                                            outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                            outsourceOrderDetailModel.OrderGender = string.Empty;
                                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                            outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                            outsourceOrderDetailModel.POPName = string.Empty;
                                            outsourceOrderDetailModel.POPType = string.Empty;
                                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                                            outsourceOrderDetailModel.POSScale = posScale;
                                            outsourceOrderDetailModel.Province = shop.ProvinceName;
                                            outsourceOrderDetailModel.Quantity = 1;
                                            outsourceOrderDetailModel.Region = shop.RegionName;
                                            outsourceOrderDetailModel.Remark = remark;
                                            outsourceOrderDetailModel.Sheet = string.Empty;
                                            outsourceOrderDetailModel.ShopId = shop.Id;
                                            outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                            outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = 0;
                                            outsourceOrderDetailModel.Tel = shop.Tel1;
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
                                            outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                            if (isBCSSubject)
                                                outsourceOrderDetailModel.OutsourceId = (shop.BCSOutsourceId ?? 0) > 0 ? (shop.BCSOutsourceId ?? 0) : (shop.OutsourceId ?? 0);
                                            else
                                                outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                            outsourceOrderDetailModel.InstallPriceAddType = 1;
                                            outsourceOrderDetailModel.InstallPriceSubjectType = (int)InstallPriceSubjectTypeEnum.活动安装费;

                                            if (installPriceShopInfoList.Any())
                                            {
                                                var installShopModel = installPriceShopInfoList.Where(i => i.ShopId == shop.Id && i.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费).FirstOrDefault();
                                                if (installShopModel != null)
                                                    outsourceOrderDetailModel.BelongSubjectId = installShopModel.SubjectId;

                                            }
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                        }
                                        //常规安装费
                                        if (genericBasicInstallPrice > 0)
                                        {
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = currUserId;
                                            outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                            outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                            outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                            outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                            outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                            outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                            outsourceOrderDetailModel.Contact = oneShopOrderList[0].Contact;
                                            outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                                            outsourceOrderDetailModel.GraphicWidth = 0;
                                            outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                            outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                                            outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                            outsourceOrderDetailModel.OrderGender = string.Empty;
                                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                            outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                            outsourceOrderDetailModel.POPName = string.Empty;
                                            outsourceOrderDetailModel.POPType = string.Empty;
                                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                                            outsourceOrderDetailModel.POSScale = posScale;
                                            outsourceOrderDetailModel.Province = shop.ProvinceName;
                                            outsourceOrderDetailModel.Quantity = 1;
                                            outsourceOrderDetailModel.Region = shop.RegionName;
                                            outsourceOrderDetailModel.Remark = "常规安装费";
                                            outsourceOrderDetailModel.Sheet = string.Empty;
                                            outsourceOrderDetailModel.ShopId = shop.Id;
                                            outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                            outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = 0;
                                            outsourceOrderDetailModel.Tel = shop.Tel1;
                                            outsourceOrderDetailModel.TotalArea = 0;
                                            outsourceOrderDetailModel.WindowDeep = 0;
                                            outsourceOrderDetailModel.WindowHigh = 0;
                                            outsourceOrderDetailModel.WindowSize = string.Empty;
                                            outsourceOrderDetailModel.WindowWide = 0;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = genericReceiveInstallPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = genericBasicInstallPrice;
                                            outsourceOrderDetailModel.PayBasicInstallPrice = genericBasicInstallPrice;
                                            outsourceOrderDetailModel.PayOOHInstallPrice = 0;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                            outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                            outsourceOrderDetailModel.InstallPriceAddType = 1;
                                            outsourceOrderDetailModel.InstallPriceSubjectType = (int)InstallPriceSubjectTypeEnum.常规安装费;
                                            if (installPriceShopInfoList.Any())
                                            {
                                                var installShopModel = installPriceShopInfoList.Where(i => i.ShopId == shop.Id && i.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费).FirstOrDefault();
                                                if (installShopModel != null)
                                                    outsourceOrderDetailModel.BelongSubjectId = installShopModel.SubjectId;

                                            }
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                        }
                                    }
                                    else if ((guidanceType == (int)GuidanceTypeEnum.Promotion && hasExpressPrice) || (guidanceType == (int)GuidanceTypeEnum.Delivery) && popList.Any())
                                    {
                                        expressPriceDetailModel = expressPriceDetailBll.GetList(price => price.GuidanceId == subjectModel.gudiance.ItemId && price.ShopId == shop.Id).FirstOrDefault();
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
                                        if (shop.ProvinceName == "内蒙古" && !shop.CityName.Contains("通辽"))
                                        {
                                            payExpressPrice = 0;
                                        }

                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = currUserId;
                                        outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                        outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                        outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                        outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                        outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                        outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                        outsourceOrderDetailModel.Contact = shop.Contact1;
                                        outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                        outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                        outsourceOrderDetailModel.GraphicNo = string.Empty;
                                        outsourceOrderDetailModel.GraphicWidth = 0;
                                        outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                        outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                        outsourceOrderDetailModel.MachineFrame = string.Empty;
                                        outsourceOrderDetailModel.MaterialSupport = string.Empty;
                                        outsourceOrderDetailModel.OrderGender = string.Empty;
                                        outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.发货费;
                                        outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                        outsourceOrderDetailModel.POPName = string.Empty;
                                        outsourceOrderDetailModel.POPType = string.Empty;
                                        outsourceOrderDetailModel.PositionDescription = string.Empty;
                                        outsourceOrderDetailModel.POSScale = posScale;
                                        outsourceOrderDetailModel.Province = shop.ProvinceName;
                                        outsourceOrderDetailModel.Quantity = 1;
                                        outsourceOrderDetailModel.Region = shop.RegionName;
                                        outsourceOrderDetailModel.Remark = string.Empty;
                                        outsourceOrderDetailModel.Sheet = string.Empty;
                                        outsourceOrderDetailModel.ShopId = shop.Id;
                                        outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                        outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = 0;
                                        outsourceOrderDetailModel.Tel = shop.Tel1;
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
                                        outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                        //outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;

                                        if (shop.ProvinceName == "天津")
                                            outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                                        else
                                        {
                                            if (isBCSSubject)
                                                outsourceOrderDetailModel.OutsourceId = (shop.BCSOutsourceId ?? 0) > 0 ? (shop.BCSOutsourceId ?? 0) : (shop.OutsourceId ?? 0);
                                            else
                                                outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                        }
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);

                                    }
                                    #endregion
                                }

                            });

                        }
                    }
                    #endregion
                    //tran.Complete();
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    isSave = false;
                }
                //}
                if (!isSave)
                {
                    OutsourceOrderSavingLogBLL logBll = new OutsourceOrderSavingLogBLL();
                    OutsourceOrderSavingLog logModel = new OutsourceOrderSavingLog();
                    logModel.AddDate = DateTime.Now;
                    string msg = errorMsg;
                    logModel.ErrorMsg = msg.Length > 400 ? (msg.Substring(0, 400)) : msg;
                    logModel.GuidanceId = guidanceId;
                    logModel.State = "操作失败";
                    logModel.SubjectId = subjectId;
                    logBll.Add(logModel);
                }
            }
        }

        void SaveHandler1(int guidanceId, int subjectId, int subjectType)
        { 
           SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(guidanceId);
           if (guidanceModel != null)
           {
               InstallPriceShopInfoBLL installPriceShopInfoBll = new InstallPriceShopInfoBLL();
               List<InstallPriceShopInfo> installPriceShopInfoList = installPriceShopInfoBll.GetList(s => s.GuidanceId == guidanceId);
               bool isSave = true;
               string errorMsg = string.Empty;

               try
               {
                   if ((subjectType != (int)SubjectTypeEnum.二次安装 && subjectType != (int)SubjectTypeEnum.费用订单))
                   {
                       if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guidanceModel.HasInstallFees ?? true))
                       {
                           new BasePage().SaveInstallPrice(guidanceId, subjectId, subjectType);
                           UpdateInstallOrderRedisDataHandler(subjectId);

                       }
                       else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guidanceModel.HasExperssFees ?? true))
                       {
                           new BasePage().SaveExpressPrice(guidanceId, subjectId, subjectType);
                       }
                       else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery)
                       {
                           new BasePage().SaveExpressPriceForDelivery(guidanceId, subjectId, subjectType, guidanceModel.ExperssPrice);
                       }
                   }
                   //保存报价单
                   new BasePage().SaveQuotationOrder(guidanceId, subjectId, subjectType);

                   //保存外协订单
                   new BasePage().SaveOutsourceOrder(subjectId, subjectType);
               }
               catch (Exception ex)
               {
                   errorMsg = ex.Message;
                   isSave = false;
               }
               
               if (!isSave)
               {
                   OutsourceOrderSavingLogBLL logBll = new OutsourceOrderSavingLogBLL();
                   OutsourceOrderSavingLog logModel = new OutsourceOrderSavingLog();
                   logModel.AddDate = DateTime.Now;
                   string msg = errorMsg;
                   logModel.ErrorMsg = msg.Length > 400 ? (msg.Substring(0, 400)) : msg;
                   logModel.GuidanceId = guidanceId;
                   logModel.State = "操作失败";
                   logModel.SubjectId = subjectId;
                   logBll.Add(logModel);
               }
           }
        }


        #region 在提交安装费归类的时候同时修改外协安装费归类
        public delegate void DeleUpdateOutsourceInstallPriceSubject(int guidanceId, int installSubjectId);

        public void UpdateOutsourceInstallPriceSubject(int guidanceId, int installSubjectId)
        {
            DeleUpdateOutsourceInstallPriceSubject dele = new DeleUpdateOutsourceInstallPriceSubject(UpdateOutsourceInstallPriceSubjectHandler);
            AsyncCallback callback = new AsyncCallback(CallBackMethod2);
            dele.BeginInvoke(guidanceId, installSubjectId, callback, dele);
        }

        void CallBackMethod2(IAsyncResult ia)
        {
            DeleUpdateOutsourceInstallPriceSubject dele = ia.AsyncState as DeleUpdateOutsourceInstallPriceSubject;
            dele.EndInvoke(ia);
        }

        void UpdateOutsourceInstallPriceSubjectHandler(int guidanceId, int installSubjectId)
        {
            InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
            var list = installShopBll.GetList(s => s.GuidanceId == guidanceId && s.SubjectId == installSubjectId);
            if (list.Any())
            {
                List<int> shopIdList = list.Select(s => s.ShopId ?? 0).Distinct().ToList();
                OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();
                //OutsourceOrderDetail model;
                var outsourceInstallOrderList = outsourceOrderBll.GetList(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0) && s.OrderType == (int)OrderTypeEnum.安装费 && (s.SubjectId ?? 0) == 0);
                if (outsourceInstallOrderList.Any())
                {
                    outsourceInstallOrderList.ForEach(s =>
                    {
                        s.BelongSubjectId = installSubjectId;
                        outsourceOrderBll.Update(s);
                    });
                }
            }
        }

        #endregion

        #region 更新Redis缓存（安装活动订单的缓存）
        public delegate void DeleUpdateInstallOrderRedisData(int subjectId);
        void CallBackMethod3(IAsyncResult ia)
        {
            DeleUpdateInstallOrderRedisData dele = ia.AsyncState as DeleUpdateInstallOrderRedisData;
            dele.EndInvoke(ia);
        }

        public void UpdateInstallOrderRedisData(int subjectId)
        {
            DeleUpdateInstallOrderRedisData dele = new DeleUpdateInstallOrderRedisData(UpdateInstallOrderRedisDataHandler);
            AsyncCallback callback = new AsyncCallback(CallBackMethod3);
            dele.BeginInvoke(subjectId, callback, dele);
        }

        void UpdateInstallOrderRedisDataHandler(int subjectId)
        {
            
            List<FinalOrderDetailTemp> orderList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId);
            if (orderList.Any())
            {
                int currGuidanceId = orderList[0].GuidanceId ?? 0;
                string redisOrderKey = "InstallPriceOrderList" + currGuidanceId;
                List<FinalOrderDetailTemp> orderListSave = RedisHelper.Get<List<FinalOrderDetailTemp>>(redisOrderKey);
                if (orderListSave == null || !orderListSave.Any())
                {
                    //List<int> guidanceIdTempList = (from guidance in CurrentContext.DbContext.SubjectGuidance
                    //                                where ((guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Install)
                    //                                && (guidance.HasInstallFees ?? true)
                    //                                && (guidance.IsDelete == null || guidance.IsDelete == false)
                    //                                select guidance.ItemId).OrderByDescending(s => s).Take(30).ToList();
                    orderListSave = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     where order.GuidanceId == currGuidanceId
                                     && order.OrderType == (int)OrderTypeEnum.POP
                                     && (subject.IsDelete==null || subject.IsDelete==false)
                                     && subject.ApproveState==1
                                     select order).ToList();
                    RedisHelper.Set(redisOrderKey, orderListSave);
                }
                else
                {
                    orderListSave.RemoveAll(s => s.SubjectId == subjectId);
                    orderListSave.AddRange(orderList);
                    RedisHelper.Set(redisOrderKey, orderListSave);
                }
            }
        }

        #endregion

    }
}
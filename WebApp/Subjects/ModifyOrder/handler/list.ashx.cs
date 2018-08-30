using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using BLL;
using Models;
using Newtonsoft.Json;
using System.Text;
using Common;
using System.Transactions;

namespace WebApp.Subjects.ModifyOrder.handler
{
    /// <summary>
    /// list 的摘要说明
    /// </summary>
    public class list : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            else if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }

            switch (type)
            {
                case "getList":
                    result = GetOrderList();
                    break;
                case "deleteOrder":
                    result = DeleteOrders();
                    break;
                case "recoverOrder":
                    result = RecoverOrders();
                    break;
                case "checkShop":
                    result = CheckShopNo();
                    break;
                case "edit":
                    result = Edit();
                    break;
                case "getOrder":
                    result = GetModel();
                    break;
                case "editShopInfo":
                    result = EditShopInfo();
                    break;
                case "getSheetList":
                    result = GetSheetList();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetSheetList()
        {
            int subjectId = 0;
            string shopNo = string.Empty;
            string shopName = string.Empty;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            Subject subjectModel = new SubjectBLL().GetModel(subjectId);
            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
            if (subjectModel != null)
            {
                if (subjectModel.SubjectType == (int)SubjectTypeEnum.HC订单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区补单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区增补 || subjectModel.SubjectType == (int)SubjectTypeEnum.新开店订单)
                {
                    orderList = orderBll.GetList(s => s.RegionSupplementId == subjectId);
                }
                else
                {
                    orderList = orderBll.GetList(s => s.SubjectId == subjectId && (s.RegionSupplementId??0)==0);
                }

            }
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                orderList = orderList.Where(s => s.ShopNo.ToLower() == shopNo.ToLower()).ToList();
            }
            List<string> sheetList = new List<string>();
            List<string> genderList = new List<string>();
            List<int> orderTypeIdList = new List<int>();
            if (orderList.Any())
            {
                bool isEmpty0 = false;
                orderList.ForEach(order =>
                {
                    string gender = !string.IsNullOrWhiteSpace(order.OrderGender) ? order.OrderGender : order.Gender;
                    if (!string.IsNullOrWhiteSpace(gender) && !genderList.Contains(gender))
                    {
                        genderList.Add(gender);
                    }
                    if (!string.IsNullOrWhiteSpace(order.Sheet))
                    {
                        if (!sheetList.Contains(order.Sheet.ToUpper()))
                            sheetList.Add(order.Sheet.ToUpper());
                    }
                    else
                        isEmpty0 = true;

                    if (!orderTypeIdList.Contains(order.OrderType ?? 1))
                    {
                        orderTypeIdList.Add(order.OrderType ?? 1);
                    }
                });
                if (isEmpty0)
                    sheetList.Add("空");
            }
            StringBuilder sheetJson = new StringBuilder();
            StringBuilder genderJson = new StringBuilder();
            StringBuilder orderTypeJson = new StringBuilder();
            sheetList.ForEach(sheet => {
                sheetJson.Append("{\"SheetName\":\""+sheet+"\"},");
            });
            genderList.ForEach(gender =>
            {
                genderJson.Append("{\"GenderName\":\"" + gender + "\"},");
            });
            orderTypeIdList.ForEach(ot => {
                string s = CommonMethod.GeEnumName<OrderTypeEnum>(ot.ToString());
                orderTypeJson.Append("{\"OrderTypeId\":\""+ot+"\",\"OrderTypeName\":\"" + s + "\"},");
            });
            string sheetStr = "[]";
            string genderStr = "[]";
            string otStr = "[]";
            if (orderTypeJson.Length > 0)
            {
                otStr = "[" + (orderTypeJson.ToString().TrimEnd(',')) + "]";
            }
            if (sheetJson.Length > 0)
            {
                sheetStr = "[" + (sheetJson.ToString().TrimEnd(',')) + "]";
            }
            if (genderJson.Length > 0)
            {
                genderStr = "[" + (genderJson.ToString().TrimEnd(',')) + "]";
            }

            return "[{\"Sheet\":" + sheetStr + ",\"Gender\":" + genderStr + ",\"OrderType\":" + otStr + "}]";
        }

        string GetOrderList()
        {
            string result = string.Empty;
            int subjectId = 0;
            int currPage = 1;
            int pageSize = 0;
            int orderTypeId = 0;
            string shopNo = string.Empty;
            string sheet = string.Empty;
            string gender = string.Empty;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            if (context1.Request.QueryString["currPage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currPage"]);
            }
            if (context1.Request.QueryString["pageSize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pageSize"]);
            }
            if (context1.Request.QueryString["orderType"] != null)
            {
                orderTypeId = int.Parse(context1.Request.QueryString["orderType"]);
            }
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }

            if (context1.Request.QueryString["sheet"] != null)
            {
                sheet = context1.Request.QueryString["sheet"];
            }
            if (context1.Request.QueryString["gender"] != null)
            {
                gender = context1.Request.QueryString["gender"];
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            Subject subjectModel = new SubjectBLL().GetModel(subjectId);
            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
            if (subjectModel != null)
            {
                // || subjectModel.SubjectType == (int)SubjectTypeEnum.分区增补 || subjectModel.SubjectType == (int)SubjectTypeEnum.新开店订单
                if (subjectModel.SubjectType == (int)SubjectTypeEnum.HC订单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区补单)
                {
                    orderList = orderBll.GetList(s => s.RegionSupplementId == subjectId);
                }
                else
                {
                    orderList = orderBll.GetList(s => s.SubjectId == subjectId);
                }
            }
            if (orderTypeId > 0)
            {
                orderList = orderList.Where(s => s.OrderType == orderTypeId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                orderList = orderList.Where(s => s.ShopNo.ToLower() == shopNo.ToLower()).ToList();
            }
            if (!string.IsNullOrWhiteSpace(sheet))
            {
                if (sheet == "空")
                {
                    orderList = orderList.Where(s => s.Sheet == "" || s.Sheet == null).ToList();
                }
                else
                {
                    orderList = orderList.Where(s => s.Sheet != "" && s.Sheet != null && s.Sheet.ToUpper()==sheet).ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(gender))
            {
                orderList = orderList.Where(s => (s.OrderGender!=null&&s.OrderGender!="")?(s.OrderGender==gender):s.Gender==gender).ToList();
            }
            int recordCount = orderList.Count;
            orderList = orderList.OrderBy(s => s.ShopId).ThenBy(s => s.Sheet).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
            StringBuilder json = new StringBuilder();
            int index = 1;
            SubjectBLL subjectBll = new SubjectBLL();
            orderList.ForEach(s =>
            {
                int rowIndex = (pageSize * (currPage - 1)) + index;
                index++;
                string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.OrderType ?? 1).ToString());
                int isDelete = s.IsDelete == true ? 1 : 0;
                string subjectName = string.Empty;
                Subject smodel = subjectBll.GetModel(s.SubjectId??0);
                if (smodel != null)
                    subjectName = smodel.SubjectName;
                json.Append("{\"rowIndex\":\"" + rowIndex + "\",\"Id\":\"" + s.Id + "\",\"SubjectId\":\"" + s.SubjectId + "\",\"SubjectName\":\"" + subjectName + "\",\"OrderType\":\"" + (s.OrderType ?? 1) + "\",\"OrderTypeName\":\"" + orderType + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"POSScale\":\"" + s.POSScale + "\",\"ShopNo\":\"" + s.ShopNo + "\",\"ShopName\":\"" + s.ShopName + "\",\"Region\":\"" + s.Region + "\",\"Province\":\"" + s.Province + "\",\"City\":\"" + s.City + "\",\"CityTier\":\"" + s.CityTier + "\",\"IsInstall\":\"" + s.IsInstall + "\",\"Channel\":\"" + s.Channel + "\",\"Format\":\"" + s.Format + "\",\"Sheet\":\"" + s.Sheet + "\",\"MachineFrame\":\"" + s.MachineFrame + "\",\"Gender\":\"" + (!string.IsNullOrWhiteSpace(s.OrderGender) ? s.OrderGender : s.Gender) + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"ChooseImg\":\"" + s.ChooseImg + "\",\"Remark\":\"" + s.Remark + "\",\"IsDelete\":\"" + isDelete + "\",\"ReceivePrice\":\"" + s.OrderPrice + "\",\"PayPrice\":\"" + s.PayOrderPrice + "\",\"ShopStatus\":\"" + (!string.IsNullOrWhiteSpace(s.ShopStatus) ? s.ShopStatus : "正常") + "\"},");
            });
            result = "{\"pageCount\":\"" + recordCount + "\",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
            return result;
        }

        string DeleteOrders()
        {
            string result = "删除失败";
            if (context1.Request.QueryString["ids"] != null)
            {
                string ids = context1.Request.QueryString["ids"];
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        List<int> idList = StringHelper.ToIntList(ids, ',');
                        try
                        {
                            int guidancid = 0;
                            int subjectId = 0;
                            List<int> oohShopIdList = new List<int>();
                            List<int> windowShopIdList = new List<int>();
                            List<int> shopIdList = new List<int>();
                            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                            var orderList = orderBll.GetList(s => idList.Contains(s.Id));
                            orderList.ForEach(s =>
                            {
                                if (guidancid == 0)
                                    guidancid = s.GuidanceId ?? 0;
                                if (subjectId == 0)
                                    subjectId = (s.RegionSupplementId ?? 0) > 0 ? (s.RegionSupplementId??0) : (s.SubjectId ?? 0);
                                s.IsDelete = true;
                                s.DeleteDate = DateTime.Now;
                                s.DeleteUserId = new BasePage().CurrentUser.UserId;
                                orderBll.Update(s);
                                if (!shopIdList.Contains(s.ShopId ?? 0))
                                    shopIdList.Add(s.ShopId ?? 0);
                                if (s.Sheet != null && s.Sheet.ToUpper() == "OOH")
                                {
                                    if (!oohShopIdList.Contains(s.ShopId ?? 0))
                                        oohShopIdList.Add(s.ShopId ?? 0);
                                }
                                if (s.Sheet != null && s.Sheet.Contains("橱窗"))
                                {
                                    if (!windowShopIdList.Contains(s.ShopId ?? 0))
                                        windowShopIdList.Add(s.ShopId ?? 0);
                                }
                            });
                            Subject subjectModel0 = new SubjectBLL().GetModel(subjectId);
                            if (subjectModel0 != null)
                            {
                                SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(guidancid);
                                if (guidanceModel != null)
                                {

                                    InstallPriceTempBLL installPriceTempBll = new InstallPriceTempBLL();
                                    InstallPriceShopInfoBLL installPriceShopInfoBll = new InstallPriceShopInfoBLL();
                                    ExpressPriceDetailBLL expressPriceBll = new ExpressPriceDetailBLL();
                                    var newOrderList = from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                                       join subject in CurrentContext.DbContext.Subject
                                                       on order.SubjectId equals subject.Id
                                                       where order.GuidanceId == guidancid
                                                       && shopIdList.Contains(order.ShopId ?? 0)
                                                       && (order.IsDelete == null || order.IsDelete == false)
                                                       && (subject.IsDelete == null || subject.IsDelete == false)
                                                       && subject.ApproveState == 1
                                                       && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                                       && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                                                       select order;
                                    List<int> newShopIdList0 = new List<int>();
                                    shopIdList.ForEach(shopid =>
                                    {
                                        var shopOrderList = newOrderList.Where(s => s.ShopId == shopid);
                                        if (shopOrderList.Any())
                                        {
                                            if (oohShopIdList.Contains(shopid) || windowShopIdList.Contains(shopid))
                                            {
                                                newShopIdList0.Add(shopid);
                                            }
                                        }
                                        else
                                        {
                                            installPriceTempBll.Delete(s => s.GuidanceId == guidancid && s.ShopId == shopid);
                                            installPriceShopInfoBll.Delete(s => s.GuidanceId == guidancid && s.ShopId == shopid && s.AddType == 1);
                                            expressPriceBll.Delete(s => s.GuidanceId == guidancid && s.ShopId == shopid);
                                        }
                                    });
                                    //if (newShopIdList0.Any() && (guidanceModel.HasInstallFees??true))
                                    //{
                                    //    //重新计算活动安装费
                                    //    new BasePage().RecountInstallPrice(guidancid, newShopIdList0);
                                    //}



                                    if (guidanceModel != null && (subjectModel0.SubjectType != (int)SubjectTypeEnum.二次安装 && subjectModel0.SubjectType != (int)SubjectTypeEnum.费用订单 && subjectModel0.SubjectType != (int)SubjectTypeEnum.新开店安装费))
                                    {
                                        if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guidanceModel.HasInstallFees ?? true))
                                        {
                                            new BasePage().RecountInstallPrice(guidanceModel.ItemId, newShopIdList0);
                                        }
                                        else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guidanceModel.HasExperssFees ?? true))
                                        {
                                            new BasePage().SaveExpressPrice(guidanceModel.ItemId, subjectId, subjectModel0.SubjectType ?? 1);
                                        }
                                        else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery)
                                        {
                                            new BasePage().SaveExpressPriceForDelivery(guidanceModel.ItemId, subjectId, subjectModel0.SubjectType ?? 1, guidanceModel.ExperssPrice);
                                        }
                                    }

                                    //处理外协订单
                                    OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();
                                    outsourceOrderBll.Delete(s => idList.Contains(s.FinalOrderId ?? 0));
                                    List<int> newShopIdList = new List<int>();//需要更新安装费的店铺
                                    shopIdList.ForEach(shopid =>
                                    {
                                        var priceOrder = outsourceOrderBll.GetList(s => s.GuidanceId == guidancid && s.ShopId == shopid && s.SubjectId == 0);
                                        if (priceOrder.Any())
                                        {
                                            var shopOrderlist = outsourceOrderBll.GetList(s => s.GuidanceId == guidancid && s.ShopId == shopid && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));
                                            if (shopOrderlist.Any())
                                            {
                                                if (priceOrder.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).Any() && oohShopIdList.Contains(shopid))
                                                {
                                                    newShopIdList.Add(shopid);
                                                }
                                            }
                                            else
                                            {
                                                outsourceOrderBll.Delete(s => s.GuidanceId == guidancid && s.ShopId == shopid && s.SubjectId == 0);
                                            }
                                        }
                                    });
                                    if (newShopIdList.Any() && (guidanceModel.HasInstallFees ?? true))
                                    {
                                        //重新计算外协安装费
                                        new BasePage().ResetOutsourceInstallPrice(guidancid, newShopIdList);
                                    }
                                }
                            }
                            result = "ok";
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            result = "删除失败："+ex.Message;
                        }
                    }
                }
            }
            return result;
        }

        string RecoverOrders()
        {
            string result = "恢复失败";
            if (context1.Request.QueryString["ids"] != null)
            {
                string ids = context1.Request.QueryString["ids"];
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    List<int> idList = StringHelper.ToIntList(ids, ',');
                    using (TransactionScope tran = new TransactionScope())
                    {
                        try
                        {
                            int guidancid = 0;
                            int subjectId = 0;
                            List<int> shopIdList = new List<int>();
                            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                            var orderList = orderBll.GetList(s => idList.Contains(s.Id));
                            
                            orderList.ForEach(s =>
                            {
                                s.IsDelete = false;
                                orderBll.Update(s);
                                if (guidancid == 0)
                                    guidancid = s.GuidanceId ?? 0;
                                if (subjectId == 0)
                                    subjectId = (s.RegionSupplementId ?? 0) > 0 ? (s.RegionSupplementId ?? 0) : (s.SubjectId ?? 0);
                                if (!shopIdList.Contains(s.ShopId ?? 0))
                                    shopIdList.Add(s.ShopId ?? 0);
                                
                            });
                            Subject subjectModel0 = new SubjectBLL().GetModel(subjectId);
                            if (subjectModel0 != null)
                            {
                                SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(guidancid);
                                if (shopIdList.Any() && guidanceModel != null && (subjectModel0.SubjectType != (int)SubjectTypeEnum.二次安装 && subjectModel0.SubjectType != (int)SubjectTypeEnum.费用订单 && subjectModel0.SubjectType != (int)SubjectTypeEnum.新开店安装费))
                                {
                                    if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guidanceModel.HasInstallFees ?? true))
                                    {
                                        new BasePage().RecountInstallPrice(guidanceModel.ItemId, shopIdList);
                                    }
                                    else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guidanceModel.HasExperssFees ?? true))
                                    {
                                        new BasePage().SaveExpressPrice(guidanceModel.ItemId, subjectId, subjectModel0.SubjectType ?? 1);
                                    }
                                    else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery)
                                    {
                                        new BasePage().SaveExpressPriceForDelivery(guidanceModel.ItemId, subjectId, subjectModel0.SubjectType ?? 1, guidanceModel.ExperssPrice);
                                    }
                                }
                                //重新分外协订单
                                new BasePage().AutoAssignOutsourceOrder(subjectId, subjectModel0.SubjectType ?? 1);
                            }
                            result = "ok";
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            result = "恢复失败："+ex.Message;
                        }
                    
                    }
                    
                }
            }
            return result;
        }

        string GetModel()
        {
            string result = string.Empty;
            int orderId = 0;
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = int.Parse(context1.Request.QueryString["orderId"]);
            }
            FinalOrderDetailTemp orderModel = new FinalOrderDetailTempBLL().GetModel(orderId);
            if (orderModel != null)
            {
                StringBuilder json = new StringBuilder();
                int materialCategoryId = 0;
                if (!string.IsNullOrWhiteSpace(orderModel.GraphicMaterial))
                {
                    OrderMaterialMpping materialModel = new OrderMaterialMppingBLL().GetList(s => s.OrderMaterialName.ToLower() == orderModel.GraphicMaterial.ToLower()).FirstOrDefault();
                    if (materialModel != null)
                    {
                        materialCategoryId = materialModel.BasicCategoryId ?? 0;
                    }
                }
                json.Append("{\"Id\":\"" + orderId + "\",\"OrderType\":\"" + (orderModel.OrderType ?? 1) + "\",\"SubjectId\":\"" + orderModel.SubjectId + "\",\"ShopId\":\"" + orderModel.ShopId + "\",\"ShopName\":\"" + orderModel.ShopName + "\",\"ShopNo\":\"" + orderModel.ShopNo + "\",\"Sheet\":\"" + orderModel.Sheet + "\",\"POSScale\":\"" + orderModel.POSScale + "\",\"MaterialSupport\":\"" + orderModel.MaterialSupport + "\",\"MachineFrame\":\"" + orderModel.MachineFrame + "\",\"PositionDescription\":\"" + orderModel.PositionDescription + "\",\"Gender\":\"" + (!string.IsNullOrWhiteSpace(orderModel.OrderGender) ? orderModel.OrderGender : orderModel.Gender) + "\",\"Quantity\":\"" + orderModel.Quantity + "\",\"GraphicLength\":\"" + orderModel.GraphicLength + "\",\"GraphicWidth\":\"" + orderModel.GraphicWidth + "\",\"MaterialCategoryId\":\"" + materialCategoryId + "\",\"GraphicMaterial\":\"" + orderModel.GraphicMaterial + "\",\"ChooseImg\":\"" + orderModel.ChooseImg + "\",\"Remark\":\"" + orderModel.Remark + "\",\"Channel\":\"" + orderModel.Channel + "\",\"Format\":\"" + orderModel.Format + "\",\"CityTier\":\"" + orderModel.CityTier + "\",\"IsInstall\":\"" + orderModel.IsInstall + "\",\"ShopStatus\":\"" + (!string.IsNullOrWhiteSpace(orderModel.ShopStatus) ? orderModel.ShopStatus : "正常") + "\",\"OrderPrice\":\"" + (orderModel.OrderPrice ?? 0) + "\",\"PayOrderPrice\":\"" + (orderModel.PayOrderPrice ?? 0) + "\"}");
                result = "[" + json.ToString() + "]";
            }
            return result;
        }

        string CheckShopNo()
        {
            string result = string.Empty;
            int subjectId = 0;
            string shopNo = string.Empty;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }
            int guidanceId = 0;
            SubjectBLL subjectBll = new SubjectBLL();
            Subject subject = subjectBll.GetModel(subjectId);
            List<int> subjectIdList = new List<int>();
            if (subject != null)
            {
                guidanceId = subject.GuidanceId ?? 0;
                subjectIdList = subjectBll.GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false)).Select(s => s.Id).ToList();
            }
            if (!string.IsNullOrWhiteSpace(shopNo) && guidanceId > 0)
            {
                Shop shop = new ShopBLL().GetList(s => s.ShopNo.ToLower() == shopNo.ToLower()).FirstOrDefault();
                if (shop != null)
                {
                    int shopId = shop.Id;
                    var orderList = new FinalOrderDetailTempBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0)).Select(s => new { s.MaterialSupport, s.POSScale }).ToList();
                    if (orderList.Any())
                    {
                        string MaterialSupport = string.Empty;
                        string POSScale = string.Empty;
                        orderList.ForEach(s =>
                        {
                            if (string.IsNullOrWhiteSpace(MaterialSupport) && !string.IsNullOrWhiteSpace(s.MaterialSupport))
                                MaterialSupport = s.MaterialSupport.ToLower();
                            if (string.IsNullOrWhiteSpace(POSScale) && !string.IsNullOrWhiteSpace(s.POSScale))
                                POSScale = s.POSScale;
                        });
                        if (!string.IsNullOrWhiteSpace(MaterialSupport) || !string.IsNullOrWhiteSpace(POSScale))
                            result = MaterialSupport + "|" + POSScale;
                    }
                }
            }
            return result;
        }

        string Edit()
        {
            string result = "ok";
            bool isOk = true;
            string jsonStr = string.Empty;
            //int subjectId = 0;
            int currSubjectId = 0;
            Subject subjectModel0 = null;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                //using (TransactionScope tran = new TransactionScope())
                //{
                    try
                    {
                        
                       // bool isPriceOrder = false;
                        FinalOrderDetailTemp orderModel = JsonConvert.DeserializeObject<FinalOrderDetailTemp>(jsonStr);
                        if (orderModel != null)
                        {
                           
                            //int orderType= orderModel.OrderType ?? 1;
                            //string orderTypeDes = CommonMethod.GetEnumDescription<OrderTypeEnum>(orderType.ToString());
                            //if (orderTypeDes.Contains("费用订单"))
                            //{
                            //    isPriceOrder = true;
                            //}

                            currSubjectId = (orderModel.RegionSupplementId ?? 0) > 0 ? orderModel.RegionSupplementId ?? 0 : (orderModel.SubjectId ?? 0);
                            subjectModel0 = new SubjectBLL().GetModel(currSubjectId);


                            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                            OrderChangeLogBLL logBll = new OrderChangeLogBLL();
                            OrderChangeLog logModel;
                            if (orderModel.Id > 0)
                            {
                                FinalOrderDetailTemp newOrderModel = orderBll.GetModel(orderModel.Id);
                                if (newOrderModel != null)
                                {

                                    logModel = new OrderChangeLog();
                                    logModel.AddDate = DateTime.Now;
                                    logModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    logModel.Area = newOrderModel.Area;
                                    logModel.BCSIsInstall = newOrderModel.BCSIsInstall;
                                    logModel.Category = newOrderModel.Category;
                                    logModel.Channel = newOrderModel.Channel;
                                    logModel.ChooseImg = newOrderModel.ChooseImg;
                                    logModel.City = newOrderModel.City;
                                    logModel.CityTier = newOrderModel.CityTier;
                                    logModel.Contact = newOrderModel.Contact;
                                    logModel.CornerType = newOrderModel.CornerType;
                                    logModel.CSUserId = newOrderModel.CSUserId;
                                    logModel.EditRemark = "修改前";
                                    logModel.EditType = "修改前";
                                    logModel.FinalOrderId = newOrderModel.Id;
                                    logModel.Format = newOrderModel.Format;
                                    logModel.Gender = newOrderModel.Gender;
                                    logModel.GraphicLength = newOrderModel.GraphicLength;
                                    logModel.GraphicMaterial = newOrderModel.GraphicMaterial;
                                    logModel.GraphicNo = newOrderModel.GraphicNo;
                                    logModel.GraphicWidth = newOrderModel.GraphicWidth;
                                    logModel.GuidanceId = newOrderModel.GuidanceId;
                                    logModel.IsFromRegion = newOrderModel.IsFromRegion;
                                    logModel.IsInstall = newOrderModel.IsInstall;
                                    logModel.IsValid = newOrderModel.IsValid;
                                    logModel.MachineFrame = newOrderModel.MachineFrame;
                                    logModel.MaterialSupport = newOrderModel.MaterialSupport;
                                    logModel.OrderGender = newOrderModel.OrderGender;
                                    logModel.OrderPrice = newOrderModel.OrderPrice;
                                    logModel.OrderType = newOrderModel.OrderType;
                                    logModel.PayOrderPrice = newOrderModel.PayOrderPrice;
                                    logModel.POPAddress = newOrderModel.POPAddress;
                                    logModel.POPName = newOrderModel.POPName;
                                    logModel.POPType = newOrderModel.POPType;
                                    logModel.PositionDescription = newOrderModel.PositionDescription;
                                    logModel.POSScale = newOrderModel.POSScale;
                                    logModel.PriceBlongRegion = newOrderModel.PriceBlongRegion;
                                    logModel.Province = newOrderModel.Province;
                                    logModel.Quantity = newOrderModel.Quantity;
                                    logModel.Region = newOrderModel.Region;
                                    logModel.RegionSupplementId = newOrderModel.RegionSupplementId;
                                    logModel.Remark = newOrderModel.Remark;
                                    logModel.Sheet = newOrderModel.Sheet;
                                    logModel.ShopId = newOrderModel.ShopId;
                                    logModel.ShopName = newOrderModel.ShopName;
                                    logModel.ShopNo = newOrderModel.ShopNo;
                                    logModel.ShopStatus = newOrderModel.ShopStatus;
                                    logModel.SubjectId = newOrderModel.SubjectId;
                                    logModel.Tel = newOrderModel.Tel;
                                    logModel.TotalArea = newOrderModel.TotalArea;
                                    logModel.TotalPrice = newOrderModel.TotalPrice;
                                    logModel.UnitName = newOrderModel.UnitName;
                                    logModel.UnitPrice = newOrderModel.UnitPrice;
                                    logBll.Add(logModel);



                                    newOrderModel.EditDate = DateTime.Now;
                                    newOrderModel.EditUserId = new BasePage().CurrentUser.UserId;
                                    if (newOrderModel.OrderType > 3)
                                    {
                                        newOrderModel.OrderPrice = orderModel.OrderPrice;
                                        newOrderModel.PayOrderPrice = orderModel.PayOrderPrice;
                                        newOrderModel.Quantity = orderModel.Quantity;
                                    }
                                    else
                                    {
                                        decimal width = orderModel.GraphicWidth ?? 0;
                                        decimal length = orderModel.GraphicLength ?? 0;
                                        newOrderModel.Area = (width * length) / 1000000;
                                        newOrderModel.MaterialSupport = orderModel.MaterialSupport;
                                        newOrderModel.InstallPriceMaterialSupport = orderModel.MaterialSupport;
                                        newOrderModel.POSScale = orderModel.POSScale;
                                        newOrderModel.ChooseImg = orderModel.ChooseImg;
                                        newOrderModel.OrderGender = orderModel.Gender;
                                        newOrderModel.GraphicLength = length;
                                        newOrderModel.GraphicMaterial = orderModel.GraphicMaterial;
                                        newOrderModel.GraphicWidth = width;
                                        newOrderModel.MachineFrame = orderModel.MachineFrame;
                                        newOrderModel.OrderType = orderModel.OrderType;
                                        newOrderModel.PositionDescription = orderModel.PositionDescription;
                                        newOrderModel.POSScale = orderModel.POSScale;
                                        newOrderModel.InstallPricePOSScale = orderModel.POSScale;
                                        newOrderModel.Quantity = orderModel.Quantity;

                                        newOrderModel.Sheet = orderModel.Sheet;

                                        decimal unitPrice = 0;
                                        decimal totalPrice = 0;
                                        if (!string.IsNullOrWhiteSpace(orderModel.GraphicMaterial))
                                        {
                                            POP pop = new POP();
                                            pop.GraphicMaterial = orderModel.GraphicMaterial;
                                            pop.GraphicLength = orderModel.GraphicLength;
                                            pop.GraphicWidth = orderModel.GraphicWidth;
                                            pop.Quantity = orderModel.Quantity;
                                            //int subjectId = orderModel.SubjectId ?? 0;
                                            SubjectGuidance guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                                                             on subject.GuidanceId equals guidance.ItemId
                                                                             where subject.Id == currSubjectId
                                                                             select guidance).FirstOrDefault();
                                            if (guidanceModel != null)
                                            {
                                                newOrderModel.GuidanceId = guidanceModel.ItemId;
                                                pop.PriceItemId = guidanceModel.PriceItemId ?? 0;
                                                string unitName = string.Empty;
                                                unitPrice = new BasePage().GetMaterialUnitPrice(pop, out totalPrice, out unitName);
                                            }

                                        }
                                        newOrderModel.UnitPrice = unitPrice;
                                        newOrderModel.TotalPrice = totalPrice;
                                    }
                                    newOrderModel.SubjectId = orderModel.SubjectId;
                                    if ((orderModel.RegionSupplementId??0)>0)
                                      newOrderModel.RegionSupplementId = orderModel.RegionSupplementId;
                                    newOrderModel.Remark = orderModel.Remark;
                                    orderBll.Update(newOrderModel);

                                    //记录修改后
                                    logModel = new OrderChangeLog();
                                    logModel.AddDate = DateTime.Now;
                                    logModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    logModel.Area = newOrderModel.Area;
                                    logModel.BCSIsInstall = newOrderModel.BCSIsInstall;
                                    logModel.Category = newOrderModel.Category;
                                    logModel.Channel = newOrderModel.Channel;
                                    logModel.ChooseImg = newOrderModel.ChooseImg;
                                    logModel.City = newOrderModel.City;
                                    logModel.CityTier = newOrderModel.CityTier;
                                    logModel.Contact = newOrderModel.Contact;
                                    logModel.CornerType = newOrderModel.CornerType;
                                    logModel.CSUserId = newOrderModel.CSUserId;
                                    logModel.EditRemark = "修改后";
                                    logModel.EditType = "修改后";
                                    logModel.FinalOrderId = newOrderModel.Id;
                                    logModel.Format = newOrderModel.Format;
                                    logModel.Gender = newOrderModel.Gender;
                                    logModel.GraphicLength = newOrderModel.GraphicLength;
                                    logModel.GraphicMaterial = newOrderModel.GraphicMaterial;
                                    logModel.GraphicNo = newOrderModel.GraphicNo;
                                    logModel.GraphicWidth = newOrderModel.GraphicWidth;
                                    logModel.GuidanceId = newOrderModel.GuidanceId;
                                    logModel.IsFromRegion = newOrderModel.IsFromRegion;
                                    logModel.IsInstall = newOrderModel.IsInstall;
                                    logModel.IsValid = newOrderModel.IsValid;
                                    logModel.MachineFrame = newOrderModel.MachineFrame;
                                    logModel.MaterialSupport = newOrderModel.MaterialSupport;
                                    logModel.OrderGender = newOrderModel.OrderGender;
                                    logModel.OrderPrice = newOrderModel.OrderPrice;
                                    logModel.OrderType = newOrderModel.OrderType;
                                    logModel.PayOrderPrice = newOrderModel.PayOrderPrice;
                                    logModel.POPAddress = newOrderModel.POPAddress;
                                    logModel.POPName = newOrderModel.POPName;
                                    logModel.POPType = newOrderModel.POPType;
                                    logModel.PositionDescription = newOrderModel.PositionDescription;
                                    logModel.POSScale = newOrderModel.POSScale;
                                    logModel.PriceBlongRegion = newOrderModel.PriceBlongRegion;
                                    logModel.Province = newOrderModel.Province;
                                    logModel.Quantity = newOrderModel.Quantity;
                                    logModel.Region = newOrderModel.Region;
                                    logModel.RegionSupplementId = newOrderModel.RegionSupplementId;
                                    logModel.Remark = newOrderModel.Remark;
                                    logModel.Sheet = newOrderModel.Sheet;
                                    logModel.ShopId = newOrderModel.ShopId;
                                    logModel.ShopName = newOrderModel.ShopName;
                                    logModel.ShopNo = newOrderModel.ShopNo;
                                    logModel.ShopStatus = newOrderModel.ShopStatus;
                                    logModel.SubjectId = newOrderModel.SubjectId;
                                    logModel.Tel = newOrderModel.Tel;
                                    logModel.TotalArea = newOrderModel.TotalArea;
                                    logModel.TotalPrice = newOrderModel.TotalPrice;
                                    logModel.UnitName = newOrderModel.UnitName;
                                    logModel.UnitPrice = newOrderModel.UnitPrice;
                                    logBll.Add(logModel);



                                }
                            }
                            else
                            {
                                string shopNo = orderModel.ShopNo;
                                Shop shopModel = new ShopBLL().GetList(s => s.ShopNo.ToLower() == shopNo.ToLower()).FirstOrDefault();
                                if (shopModel != null)
                                {

                                    //int subjectId = orderModel.SubjectId ?? 0;

                                    SubjectGuidance guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                                                     join guidance in CurrentContext.DbContext.SubjectGuidance
                                                                     on subject.GuidanceId equals guidance.ItemId
                                                                     where subject.Id == currSubjectId
                                                                     select guidance).FirstOrDefault();



                                    FinalOrderDetailTemp newOrderModel = new FinalOrderDetailTemp();
                                    newOrderModel.AddDate = DateTime.Now;
                                    newOrderModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    decimal width = orderModel.GraphicWidth ?? 0;
                                    decimal length = orderModel.GraphicLength ?? 0;
                                    newOrderModel.Area = (width * length) / 1000000;
                                    newOrderModel.AgentCode = shopModel.AgentCode;
                                    newOrderModel.AgentName = shopModel.AgentName;
                                    newOrderModel.Channel = shopModel.Channel;
                                    newOrderModel.ChooseImg = orderModel.ChooseImg;
                                    newOrderModel.City = shopModel.CityName;
                                    newOrderModel.CityTier = shopModel.CityTier;
                                    newOrderModel.Contact = shopModel.Contact1;
                                    newOrderModel.Format = shopModel.Format;
                                    newOrderModel.Gender = orderModel.Gender;
                                    newOrderModel.OrderGender = orderModel.Gender;
                                    newOrderModel.GraphicLength = length;
                                    newOrderModel.GraphicMaterial = orderModel.GraphicMaterial;
                                    newOrderModel.GraphicWidth = width;
                                    newOrderModel.IsInstall = shopModel.IsInstall;
                                    newOrderModel.BCSIsInstall = shopModel.BCSIsInstall;
                                    newOrderModel.MachineFrame = orderModel.MachineFrame;
                                    newOrderModel.MaterialSupport = orderModel.MaterialSupport;
                                    newOrderModel.InstallPriceMaterialSupport = orderModel.MaterialSupport;
                                    newOrderModel.OrderType = orderModel.OrderType;
                                    newOrderModel.POPAddress = shopModel.POPAddress;
                                    newOrderModel.PositionDescription = orderModel.PositionDescription;
                                    newOrderModel.POSScale = orderModel.POSScale;
                                    newOrderModel.InstallPricePOSScale = orderModel.POSScale;
                                    newOrderModel.Province = shopModel.ProvinceName;
                                    newOrderModel.Quantity = orderModel.Quantity;
                                    newOrderModel.Region = shopModel.RegionName;
                                    newOrderModel.Remark = orderModel.Remark + "(手工新增)";
                                    newOrderModel.Sheet = orderModel.Sheet;
                                    newOrderModel.ShopId = shopModel.Id;
                                    newOrderModel.ShopName = shopModel.ShopName;
                                    newOrderModel.ShopNo = shopModel.ShopNo;
                                    newOrderModel.SubjectId = orderModel.SubjectId;
                                    newOrderModel.GuidanceId = orderModel.GuidanceId;
                                    newOrderModel.Tel = shopModel.Tel1;
                                    newOrderModel.CSUserId = shopModel.CSUserId;
                                    decimal unitPrice = 0;
                                    decimal totalPrice = 0;
                                    if (!string.IsNullOrWhiteSpace(orderModel.GraphicMaterial))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = orderModel.GraphicMaterial;
                                        pop.GraphicLength = orderModel.GraphicLength;
                                        pop.GraphicWidth = orderModel.GraphicWidth;
                                        pop.Quantity = orderModel.Quantity;

                                       
                                        if (guidanceModel != null)
                                        {
                                            newOrderModel.GuidanceId = guidanceModel.ItemId;
                                            pop.PriceItemId = guidanceModel.PriceItemId ?? 0;
                                            string unitName = string.Empty;
                                            unitPrice = new BasePage().GetMaterialUnitPrice(pop, out totalPrice, out unitName);
                                        }

                                    }
                                    newOrderModel.UnitPrice = unitPrice;
                                    newOrderModel.TotalPrice = totalPrice;
                                    newOrderModel.OrderPrice = orderModel.OrderPrice;
                                    newOrderModel.PayOrderPrice = orderModel.PayOrderPrice;
                                    //Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                                    if (subjectModel0 != null)
                                    {
                                        newOrderModel.GuidanceId = subjectModel0.GuidanceId;
                                    }
                                    if ((orderModel.RegionSupplementId??0)>0)
                                    {
                                        newOrderModel.IsFromRegion = true;
                                        newOrderModel.RegionSupplementId = orderModel.RegionSupplementId;
                                    }
                                    orderBll.Add(newOrderModel);

                                    //记录新增记录
                                    logModel = new OrderChangeLog();
                                    logModel.AddDate = DateTime.Now;
                                    logModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    logModel.Area = newOrderModel.Area;
                                    logModel.BCSIsInstall = newOrderModel.BCSIsInstall;
                                    logModel.Category = newOrderModel.Category;
                                    logModel.Channel = newOrderModel.Channel;
                                    logModel.ChooseImg = newOrderModel.ChooseImg;
                                    logModel.City = newOrderModel.City;
                                    logModel.CityTier = newOrderModel.CityTier;
                                    logModel.Contact = newOrderModel.Contact;
                                    logModel.CornerType = newOrderModel.CornerType;
                                    logModel.CSUserId = newOrderModel.CSUserId;
                                    logModel.EditRemark = "新增订单信息";
                                    logModel.EditType = "新增";
                                    logModel.FinalOrderId = newOrderModel.Id;
                                    logModel.Format = newOrderModel.Format;
                                    logModel.Gender = newOrderModel.Gender;
                                    logModel.GraphicLength = newOrderModel.GraphicLength;
                                    logModel.GraphicMaterial = newOrderModel.GraphicMaterial;
                                    logModel.GraphicNo = newOrderModel.GraphicNo;
                                    logModel.GraphicWidth = newOrderModel.GraphicWidth;
                                    logModel.GuidanceId = newOrderModel.GuidanceId;
                                    logModel.IsFromRegion = newOrderModel.IsFromRegion;
                                    logModel.IsInstall = newOrderModel.IsInstall;
                                    logModel.IsValid = newOrderModel.IsValid;
                                    logModel.MachineFrame = newOrderModel.MachineFrame;
                                    logModel.MaterialSupport = newOrderModel.MaterialSupport;
                                    logModel.OrderGender = newOrderModel.OrderGender;
                                    logModel.OrderPrice = newOrderModel.OrderPrice;
                                    logModel.OrderType = newOrderModel.OrderType;
                                    logModel.PayOrderPrice = newOrderModel.PayOrderPrice;
                                    logModel.POPAddress = newOrderModel.POPAddress;
                                    logModel.POPName = newOrderModel.POPName;
                                    logModel.POPType = newOrderModel.POPType;
                                    logModel.PositionDescription = newOrderModel.PositionDescription;
                                    logModel.POSScale = newOrderModel.POSScale;
                                    logModel.PriceBlongRegion = newOrderModel.PriceBlongRegion;
                                    logModel.Province = newOrderModel.Province;
                                    logModel.Quantity = newOrderModel.Quantity;
                                    logModel.Region = newOrderModel.Region;
                                    logModel.RegionSupplementId = newOrderModel.RegionSupplementId;
                                    logModel.Remark = newOrderModel.Remark;
                                    logModel.Sheet = newOrderModel.Sheet;
                                    logModel.ShopId = newOrderModel.ShopId;
                                    logModel.ShopName = newOrderModel.ShopName;
                                    logModel.ShopNo = newOrderModel.ShopNo;
                                    logModel.ShopStatus = newOrderModel.ShopStatus;
                                    logModel.SubjectId = newOrderModel.SubjectId;
                                    logModel.Tel = newOrderModel.Tel;
                                    logModel.TotalArea = newOrderModel.TotalArea;
                                    logModel.TotalPrice = newOrderModel.TotalPrice;
                                    logModel.UnitName = newOrderModel.UnitName;
                                    logModel.UnitPrice = newOrderModel.UnitPrice;
                                    logBll.Add(logModel);
                                    //if (!isPriceOrder)
                                    //{
                                    //    if (guidanceModel != null && (subjectModel0.SubjectType != (int)SubjectTypeEnum.二次安装 && subjectModel0.SubjectType != (int)SubjectTypeEnum.费用订单 && subjectModel0.SubjectType != (int)SubjectTypeEnum.新开店安装费))
                                    //    {
                                    //        if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guidanceModel.HasInstallFees ?? true))
                                    //        {
                                    //            new BasePage().RecountInstallPrice(guidanceModel.ItemId, new List<int>() { shopModel.Id });
                                    //        }
                                    //        else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guidanceModel.HasExperssFees ?? true))
                                    //        {
                                    //            new BasePage().SaveExpressPrice(guidanceModel.ItemId, subjectId, subjectModel0.SubjectType ?? 1);
                                    //        }
                                    //        else if (guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery)
                                    //        {
                                    //            new BasePage().SaveExpressPriceForDelivery(guidanceModel.ItemId, subjectId, subjectModel0.SubjectType ?? 1, guidanceModel.ExperssPrice);
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                   // result = "提交失败：店铺不存在";

                                    throw new Exception("店铺不存在");
                                    
                                }
                            }
                           
                           
                        }
                        //tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        result = "提交失败：" + ex.Message;
                        isOk = false;
                    }
                //}
            }
            else
            {
                result = "提交失败！";
                isOk = false;
            }
            if (isOk)
            {
                //Subject sModel = new SubjectBLL().GetModel(subjectId);
                if (subjectModel0 != null)
                {
                    if (subjectModel0.SubjectType != (int)SubjectTypeEnum.新开店安装费 && subjectModel0.SubjectType != (int)SubjectTypeEnum.运费)
                    {
                        //重新计算安装费，分配外协订单
                        new WebApp.Base.DelegateClass().SaveOutsourceOrder(subjectModel0.GuidanceId ?? 0, subjectModel0.Id);
                    }
                }
            }
            return result;
        }

        string EditShopInfo()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                try
                {
                    FinalOrderDetailTemp orderModel = JsonConvert.DeserializeObject<FinalOrderDetailTemp>(jsonStr);
                    if (orderModel != null)
                    {
                        FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                        if (orderModel.Id > 0)
                        {
                            FinalOrderDetailTemp order = orderBll.GetModel(orderModel.Id);
                            if (order != null)
                            {


                                var list = orderBll.GetList(s => s.ShopId == order.ShopId && s.GuidanceId == order.GuidanceId);
                                list.ForEach(s =>
                                {
                                    s.POSScale = orderModel.POSScale;
                                    s.MaterialSupport = orderModel.MaterialSupport;
                                    s.Channel = orderModel.Channel;
                                    s.Format = orderModel.Format;
                                    s.CityTier = orderModel.CityTier;
                                    s.IsInstall = orderModel.IsInstall;
                                    s.ShopStatus = orderModel.ShopStatus;
                                    orderBll.Update(s);
                                });
                                if (!string.IsNullOrWhiteSpace(orderModel.ShopStatus) && orderModel.ShopStatus.Contains("闭"))
                                {
                                    new InstallPriceTempBLL().Delete(s => s.GuidanceId == order.GuidanceId && s.ShopId == order.ShopId);
                                    new InstallPriceShopInfoBLL().Delete(s => s.GuidanceId == order.GuidanceId && s.ShopId == order.ShopId);
                                    new ExpressPriceDetailBLL().Delete(s => s.GuidanceId == order.GuidanceId && s.ShopId == order.ShopId);
                                    //删除外协订单
                                    new OutsourceOrderDetailBLL().Delete(s => s.GuidanceId == order.GuidanceId && s.ShopId == order.ShopId);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
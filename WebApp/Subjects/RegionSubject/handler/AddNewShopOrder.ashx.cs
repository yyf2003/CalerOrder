using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using System.Text;
using Common;
using System.Transactions;

namespace WebApp.Subjects.RegionSubject.handler
{
    /// <summary>
    /// AddNewShopOrder 的摘要说明
    /// </summary>
    public class AddNewShopOrder : IHttpHandler
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
                case "getOrderShopList":
                    result = GetOrderShopList();
                    break;
               
                case "getPOP":
                    result = GetPOPList();
                    break;
                case "addOrder":
                    result = AddOrder();
                    break;
                case "deleteOrder":
                    result = DeleteOrder();
                    break;
                
                case "getOrder":
                    result = GetOrder();
                    break;
                case "editOrder":
                    result = EditOrder();
                    break;
                case "getGuidanceList":
                    result = GetSubjectGuidanceList();
                    break;
                case "getSubjectList":
                    result = GetSubjectList();
                    break;
                case "getPropList":
                    result = GetPropList();
                    break;
                case "editProp":
                    result = EditProp();
                    break;
                case "deleteProp":
                    result = DeleteProp();
                    break;
                case "getProp":
                    result = GetProp();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetOrderList()
        {
            string result = string.Empty;
            int subjectId = 0;
            int shopId = 0;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            if (context1.Request.QueryString["shopId"] != null)
            {
                shopId = int.Parse(context1.Request.QueryString["shopId"]);
            }
            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join outsource1 in CurrentContext.DbContext.Company
                        on order.OutsourceId equals outsource1.Id into temp
                        from outsource in temp.DefaultIfEmpty()
                        where order.SubjectId == subjectId
                        && (shopId > 0 ? order.ShopId == shopId : 1 == 1)
                        select new
                        {
                            order,
                            shop,
                            OutsourceName = outsource.CompanyName
                        }).OrderBy(s => s.order.OrderType).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    
                    string type = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                    string addDate = string.Empty;
                    if (s.order.AddDate != null)
                        addDate = s.order.AddDate.ToString();

                    json.Append("{\"Id\":\"" + s.order.Id + "\",\"OrderType\":\"" + (s.order.OrderType ?? 1) + "\",\"OrderTypeName\":\"" + type + "\",\"ShopId\":\"" + s.order.ShopId + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"RegionName\":\"" + s.shop.RegionName + "\",\"ProvinceName\":\"" + s.shop.ProvinceName + "\",\"CityName\":\"" + s.shop.CityName + "\",\"MaterialSupport\":\"" + s.order.MaterialSupport + "\",\"POSScale\":\"" + s.order.POSScale + "\",\"Channel\":\"" + s.shop.Channel + "\",\"Format\":\"" + s.shop.Format + "\",\"Sheet\":\"" + s.order.Sheet + "\",\"GraphicNo\":\"" + s.order.GraphicNo + "\",\"Gender\":\"" + s.order.Gender + "\",\"Quantity\":\"" + s.order.Quantity + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",\"ChooseImg\":\"" + s.order.ChooseImg + "\",\"Remark\":\"" + s.order.Remark + "\",\"IsApprove\":\"" + (s.order.ApproveState ?? 0) + "\",\"AddDate\":\"" + addDate + "\",\"Price\":\"" + s.order.Price + "\",\"PayPrice\":\"" + s.order.PayPrice + "\",\"OutsourceName\":\"" + s.OutsourceName + "\"},");
                   

                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetOrderShopList()
        {
            string result = string.Empty;
            int subjectId = 0;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            shop
                        }).OrderByDescending(s => s.order.AddDate).ToList();
            var shopList = (from shop in list
                            group shop by new
                            {
                                shop.shop
                            }
                                into temp
                                select new
                                {
                                    temp.Key.shop,
                                    POPCount = temp.Count(),
                                    ApprovePOPCount = temp.Count(s => s.order.ApproveState == 1)
                                }).ToList();
            if (shopList.Any())
            {
                StringBuilder json = new StringBuilder();
                shopList.ForEach(s =>
                {
                    int approveState = 0;
                    if (s.POPCount > 0 && s.ApprovePOPCount == 0)
                    {
                        approveState = 0;//待审批
                    }
                    if (s.POPCount > s.ApprovePOPCount)
                    {
                        approveState = 1;//未完成审批
                    }
                    if (s.POPCount == s.ApprovePOPCount)
                    {
                        approveState = 2;//完成审批
                    }
                    json.Append("{\"ShopId\":\"" + s.shop.Id + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"Region\":\"" + s.shop.RegionName + "\",\"Province\":\"" + s.shop.ProvinceName + "\",\"City\":\"" + s.shop.CityName + "\",\"Channel\":\"" + s.shop.Channel + "\",\"Format\":\"" + s.shop.Format + "\",\"ShopType\":\"" + s.shop.ShopType + "\",\"POPCount\":\"" + s.POPCount + "\",\"ApproveState\":\"" + approveState + "\"},");

                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }

            return result;
        }

        string GetPOPList()
        {
            string result = string.Empty;
            string shopNo = string.Empty;
            string materialSupport = string.Empty;
            bool flag = true;
            StringBuilder errorMsg = new StringBuilder();
            int subjectId = 0;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }
            if (context1.Request.QueryString["materialSupport"] != null)
            {
                materialSupport = context1.Request.QueryString["materialSupport"];
            }
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                var shopModel = new ShopBLL().GetList(s => s.ShopNo.ToLower() == shopNo.ToLower() && (s.IsDelete==null || s.IsDelete==false)).FirstOrDefault();
                if (shopModel != null)
                {
                    if (string.IsNullOrWhiteSpace(shopModel.CityTier))
                    {
                        flag = false;
                        errorMsg.Append("城市级别为空，");
                    }
                    if (string.IsNullOrWhiteSpace(shopModel.IsInstall))
                    {
                        flag = false;
                        errorMsg.Append("是否安装为空");
                    }
                    bool isAddNewShop = false;
                    Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                    if (subjectModel != null && subjectModel.SubjectType==(int)SubjectTypeEnum.新开店订单)
                    {
                        isAddNewShop = true;
                    }
                    int shopId = shopModel.Id;
                    bool isKids = false;
                    string newShopGraphicNo = "signage";
                    if (!string.IsNullOrWhiteSpace(shopModel.Format))
                    {
                        string format = shopModel.Format.ToUpper();
                        if (format.Contains("KIDS") || format.Contains("INFANT") || format.Contains("YA"))
                        {
                            isKids = true;
                        }
                    }
                    //获取这个店铺已下完的订单
                    var orderList = new RegionOrderDetailBLL().GetList(s => s.SubjectId == subjectId && s.ShopId == shopId);

                    var  popList= new POPBLL().GetList(s => s.ShopId == shopId);
                    if (isAddNewShop && isKids)
                    { 
                        popList = popList.Where(s => s.GraphicNo != null && s.GraphicNo.ToLower().Contains(newShopGraphicNo)).ToList();

                    }
                    if (popList.Any())
                    {
                        StringBuilder popJson = new StringBuilder();

                        popList.ForEach(s =>
                        {
                            bool canWork = true;
                            if (orderList.Any())
                            {
                                var existOrder = orderList.Where(order => order.Sheet!=null && order.Sheet.ToLower() == s.Sheet.ToLower() && order.GraphicNo == s.GraphicNo).FirstOrDefault();
                                canWork = existOrder == null;
                            }
                            if (canWork)
                            {
                                int isValid = (s.IsValid ?? true) ? 1 : 0;

                                popJson.Append("{\"Id\":\"" + s.Id + "\",\"ShopId\":\"" + s.ShopId + "\",\"Sheet\":\"" + s.Sheet + "\",\"GraphicNo\":\"" + s.GraphicNo + "\",\"Gender\":\"" + s.Gender + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"Remark\":\"" + s.Remark + "\",\"IsValid\":\"" + isValid + "\",\"OrderType\":\"1\",\"OrderTypeName\":\"POP\"},");
                            }
                        });
                        if (isKids && popJson.Length > 0 && !string.IsNullOrWhiteSpace(materialSupport))
                        {
                            var propList = new KidsNewShopPropBLL().GetList(s => s.MaterialSupport == materialSupport.ToUpper());
                            if (propList.Any())
                            {
                                propList.ForEach(s =>
                                {
                                    popJson.Append("{\"Id\":\"" + s.Id + "\",\"ShopId\":\"" + shopModel.Id + "\",\"Sheet\":\"" + s.PropName + "\",\"GraphicNo\":\"\",\"Gender\":\"" + s.Gender + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicWidth\":\"\",\"GraphicLength\":\"\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"Remark\":\"" + s.Remark + "\",\"IsValid\":\"1\",\"OrderType\":\"2\",\"OrderTypeName\":\"道具\"},");
                                });
                            }
                        }
                        result = "[" + popJson.ToString().TrimEnd(',') + "]";

                    }
                }
                else
                {
                    flag = false;
                    errorMsg.Append("店铺不存在");
                }
            }
            if (flag)
            {
                result = "ok|" + result;
            }
            else
            {
                result = "error|" + errorMsg.ToString();
            }
            return result;
        }

        string GetSubjectGuidanceList()
        {
            string result = string.Empty;
            //默认获取前三个月活动
            string activityMonth = string.Empty;
            if (context1.Request.QueryString["month"] != null)
            {
                activityMonth = context1.Request.QueryString["month"];

            }
            var guidanceList = new SubjectGuidanceBLL().GetList(s =>s.ActivityTypeId!=(int)GuidanceTypeEnum.Others && (s.IsDelete == null || s.IsDelete == false)).OrderByDescending(s => s.AddDate).ToList();
            if (!string.IsNullOrWhiteSpace(activityMonth) && StringHelper.IsDateTime(activityMonth))
            {
                DateTime date = DateTime.Parse(activityMonth);
                int year = date.Year;
                int month = date.Month;
                guidanceList = guidanceList.Where(s => s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
            }
            else
            {
                //DateTime date1 = DateTime.Now.AddMonths(-1);
                //DateTime date = new DateTime(date1.Year, date1.Month, 1);
                DateTime date = DateTime.Now;
                int year = date.Year;
                int month = date.Month;
                guidanceList = guidanceList.Where(s => s.GuidanceYear == year && s.GuidanceMonth == month).ToList();

            }
            if (guidanceList.Any())
            {
                StringBuilder guidanceJson = new StringBuilder();
                guidanceList.ForEach(s =>
                {
                    guidanceJson.Append("{\"Id\":\"" + s.ItemId + "\",\"GuidanceName\":\"" + s.ItemName + "\"},");
                });
                result = "[" + guidanceJson.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetSubjectList()
        {
            int guidanceId = 0;
            if (context1.Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(context1.Request.QueryString["guidanceId"]);
            }
            int subjectId = 0;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }

            string result = string.Empty;
            StringBuilder subjectJson = new StringBuilder();
            SubjectBLL subjectBll = new SubjectBLL();
            Subject subjectModel = subjectBll.GetModel(subjectId);
            if (subjectModel != null)
            {
               
                List<string> regionList = new List<string>();
                if (!string.IsNullOrWhiteSpace(subjectModel.SupplementRegion))
                {
                    regionList = StringHelper.ToStringList(subjectModel.SupplementRegion, ',', LowerUpperEnum.ToLower);

                }
                var subjectList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                   join shop in CurrentContext.DbContext.Shop
                                   on order.ShopId equals shop.Id
                                   join subject in CurrentContext.DbContext.Subject
                                   on order.SubjectId equals subject.Id
                                   where subject.GuidanceId==guidanceId
                                   && subject.ApproveState == 1
                                   && (subject.IsDelete == null || subject.IsDelete == false)
                                   && (subject.SubjectType ?? 1) == (int)SubjectTypeEnum.POP订单
                                   && (regionList.Any() ? (regionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                                   
                                   select subject).Distinct().OrderBy(s => s.Id).ToList();
                if (subjectList.Any())
                {
                    subjectList.ForEach(s =>
                    {
                        subjectJson.Append("{\"SubjectId\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                    });

                }
                //空POP项目
                var notPOPSubject = subjectBll.GetList(s => (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && s.RegionOrderType == 1 && s.GuidanceId == guidanceId);
                if (regionList.Any())
                {
                    notPOPSubject = notPOPSubject.Where(s => (s.Region != null && s.Region != "") ? (regionList.Contains(s.Region.ToLower())) : 1 == 1).ToList();
                }
                notPOPSubject.ForEach(s =>
                {
                    subjectJson.Append("{\"SubjectId\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                });
                if (subjectJson.Length > 0)
                {
                    result = "[" + subjectJson.ToString().TrimEnd(',') + "]";
                }
            }

            return result;
        }

        string AddOrder()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    jsonStr = jsonStr.Replace("+", "%2B");
                    jsonStr = HttpUtility.UrlDecode(jsonStr);
                }
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        List<RegionOrderDetail> orderList = JsonConvert.DeserializeObject<List<RegionOrderDetail>>(jsonStr);
                        if (orderList.Any())
                        {
                            RegionOrderDetail orderModel;
                            RegionOrderDetailBLL orderBll = new RegionOrderDetailBLL();
                            //RegionOrderPriceBLL orderPriceBll = new RegionOrderPriceBLL();
                            //RegionOrderPrice orderPriceModel;
                            int shopId = orderList[0].ShopId ?? 0;
                            int subjectId = orderList[0].SubjectId ?? 0;
                            int customerId = 0;
                            Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                            if (subjectModel != null)
                                customerId = subjectModel.CustomerId ?? 0;
                            StringBuilder msg = new StringBuilder();
                            int successNum = 0;
                            orderList.ForEach(s =>
                            {
                                bool canSave = true;
                                if (s.OrderType == (int)OrderTypeEnum.POP)
                                {
                                    canSave = CheckMaterial(customerId,s.GraphicMaterial);
                                }
                                if (canSave)
                                {
                                    
                                    //if (s.OrderType == (int)OrderTypeEnum.费用)
                                    //{
                                    //    orderPriceModel = new RegionOrderPrice();
                                    //    orderPriceModel.AddDate = DateTime.Now;
                                    //    orderPriceModel.Price = s.OrderPrice;
                                    //    orderPriceModel.PriceTypeId = s.PriceType;
                                    //    orderPriceModel.Remark = s.Remark;
                                    //    orderPriceModel.ShopId = shopId;
                                    //    orderPriceModel.SubjectId = subjectId;
                                    //    orderPriceBll.Add(orderPriceModel);
                                    //}
                                    //else
                                    //{
                                        
                                    //}
                                    orderModel = new RegionOrderDetail();
                                    orderModel.OrderType = s.OrderType;
                                    orderModel.AddDate = DateTime.Now;
                                    orderModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    orderModel.CornerType = s.CornerType;
                                    orderModel.Gender = s.Gender;
                                    orderModel.GraphicLength = s.GraphicLength;
                                    orderModel.GraphicMaterial = s.GraphicMaterial;
                                    orderModel.GraphicNo = s.GraphicNo;
                                    orderModel.GraphicWidth = s.GraphicWidth;
                                    orderModel.MaterialSupport = s.MaterialSupport;
                                    orderModel.PositionDescription = s.PositionDescription;
                                    orderModel.POSScale = s.POSScale;
                                    orderModel.Quantity = s.Quantity;
                                    orderModel.Remark = s.Remark;
                                    orderModel.Sheet = !string.IsNullOrWhiteSpace(s.Sheet)?s.Sheet:"";
                                    orderModel.ShopId = s.ShopId;
                                    orderModel.SubjectId = s.SubjectId;
                                    orderModel.ChooseImg = s.ChooseImg;
                                    orderModel.IsValid = s.IsValid;
                                    if (s.OrderType > 2)
                                    {
                                        orderModel.Price = s.Price;
                                        orderModel.PayPrice = s.PayPrice;
                                    }
                                    orderBll.Add(orderModel);
                                    successNum++;
                                }
                                else
                                {
                                    msg.Append(s.GraphicMaterial + "，");
                                }
                            });
                            if (msg.Length > 0)
                            {
                                
                                result = "材质 '" + msg.ToString().TrimEnd('，') + "' 不正确，请先更新基础数据库再提交！";
                                throw new Exception(result);
                            }
                            else
                            {
                                tran.Complete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result =  ex.Message;
                    }
                }
            }
            else
            {
                result = "没有可提交的数据";
            }
            return result;
        }

        string GetOrder()
        {
            int id = 0;
            string result = string.Empty;
            if (context1.Request.QueryString["id"] != null)
            {
                id = int.Parse(context1.Request.QueryString["id"]);
            }
            RegionOrderDetail model = new RegionOrderDetailBLL().GetModel(id);
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((model.OrderType ?? 0).ToString());
                json.Append("{\"Id\":\"" + model.Id + "\",\"ChooseImg\":\"" + model.ChooseImg + "\",\"Gender\":\"" + model.Gender + "\",\"GraphicLength\":\"" + model.GraphicLength + "\",\"GraphicMaterial\":\"" + model.GraphicMaterial + "\",\"GraphicWidth\":\"" + model.GraphicWidth + "\",\"HandMakeSubjectId\":\"" + model.HandMakeSubjectId + "\",\"MaterialSupport\":\"" + model.MaterialSupport + "\",\"OrderType\":\"" + orderType + "\",\"PositionDescription\":\"" + model.PositionDescription + "\",\"POSScale\":\"" + model.POSScale + "\",\"Quantity\":\"" + model.Quantity + "\",\"Remark\":\"" + model.Remark + "\",\"Sheet\":\"" + model.Sheet + "\",\"GraphicNo\":\"" + model.GraphicNo + "\",\"Price\":\""+model.Price+"\",\"PayPrice\":\""+model.PayPrice+"\"},");
                result = "[" + json.ToString().TrimEnd(',') + "]";

            }
            return result;
        }

        string EditOrder()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];

                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    jsonStr = jsonStr.Replace("+", "%2B");
                    jsonStr = HttpUtility.UrlDecode(jsonStr);
                }
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                try
                {
                    RegionOrderDetail newOrder = JsonConvert.DeserializeObject<RegionOrderDetail>(jsonStr);
                    RegionOrderDetailBLL orderBll = new RegionOrderDetailBLL();
                    if (newOrder != null && newOrder.Id > 0)
                    {

                        RegionOrderDetail orderModel = orderBll.GetModel(newOrder.Id);
                        if (orderModel != null)
                        {
                            string orderType = CommonMethod.GetEnumDescription<OrderTypeEnum>((orderModel.OrderType ?? 1).ToString());
                            if (orderType == "费用订单")
                            {
                                orderModel.OrderType = (int)Enum.Parse(typeof(OrderTypeEnum), newOrder.OrderTypeName);
                                orderModel.Price = newOrder.Price;
                                if ((newOrder.PayPrice ?? 0) == 0)
                                    orderModel.PayPrice = 150;
                                else
                                    orderModel.PayPrice = newOrder.PayPrice;
                                orderModel.Remark = newOrder.Remark;
                            }
                            else
                            {
                                orderModel.Gender = newOrder.Gender;
                                orderModel.MaterialSupport = newOrder.MaterialSupport;
                                orderModel.PositionDescription = newOrder.PositionDescription;
                                orderModel.POSScale = newOrder.POSScale;
                                orderModel.Quantity = newOrder.Quantity;
                                orderModel.Remark = newOrder.Remark;
                                orderModel.Sheet = !string.IsNullOrWhiteSpace(newOrder.Sheet) ? newOrder.Sheet : ""; ;

                                orderModel.ChooseImg = newOrder.ChooseImg;
                                
                            }
                            orderBll.Update(orderModel);
                        }
                        else
                            result = "提交失败：请重试";
                    }
                    else
                        result = "提交失败：请重试";
                }
                catch (Exception ex)
                {
                    result = "提交失败：" + ex.Message;
                }
            }
            return result;
        }

        string DeleteOrder()
        {
            string result = "ok";
            string ids = string.Empty;
            if (context1.Request.QueryString["ids"] != null)
            {
                ids = context1.Request.QueryString["ids"];
                List<int> idList = StringHelper.ToIntList(ids, ',');
                if (idList.Any())
                {
                    try
                    {
                        new RegionOrderDetailBLL().Delete(s => idList.Contains(s.Id));
                    }
                    catch (Exception ex)
                    {
                        result = "删除失败：" + ex.Message;
                    }
                }
            }
            return result;
        }

        List<string> materialList = new List<string>();
        bool CheckMaterial(int customerId,string materialName)
        {
            bool flag = true;
            materialName = materialName.Trim().ToLower();
            if (materialList.Contains(materialName))
            {
                flag = true;
            }
            else
            {
                OrderMaterialMpping materialModel = new OrderMaterialMppingBLL().GetList(s =>s.CustomerId==customerId && s.OrderMaterialName.ToLower() == materialName).FirstOrDefault();
                if (materialModel != null)
                    materialList.Add(materialName);
                else
                    flag = false;
            }
            return flag;
        }


        string GetPropList()
        {
            string materialSupport = string.Empty;
            if (context1.Request.QueryString["MaterialSupport"] != null)
            {
                materialSupport = context1.Request.QueryString["MaterialSupport"];
            }
            string result = string.Empty;
            var list = new KidsNewShopPropBLL().GetList(s => s.Id > 0);

            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"PropName\":\"" + s.PropName + "\",\"Quantity\":\"" + s.Quantity + "\",\"Gender\":\"" + s.Gender + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"Remark\":\"" + s.Remark + "\"},");
                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string EditProp()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    jsonStr = jsonStr.Replace("+", "%2B");
                    jsonStr = HttpUtility.UrlDecode(jsonStr);
                }
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                KidsNewShopPropBLL propBll = new KidsNewShopPropBLL();
                KidsNewShopProp propModel = JsonConvert.DeserializeObject<KidsNewShopProp>(jsonStr);
                if (propModel != null)
                {
                    if (propModel.Id > 0)
                    {
                        KidsNewShopProp oldModel = propBll.GetModel(propModel.Id);
                        if (oldModel != null)
                        {
                            
                            var list0 = propBll.GetList(s => s.MaterialSupport == propModel.MaterialSupport.ToUpper() && s.PropName.ToLower() == propModel.PropName.ToLower() && s.Id != propModel.Id);
                            if (list0.Any())
                            {
                                result = "道具名称已存在";
                            }
                            else
                            {
                                oldModel.GraphicMaterial = propModel.GraphicMaterial;
                                oldModel.MaterialSupport = propModel.MaterialSupport.ToUpper();
                                oldModel.PositionDescription = propModel.PositionDescription;
                                oldModel.PropName = propModel.PropName;
                                oldModel.Quantity = propModel.Quantity;
                                oldModel.Remark = propModel.Remark;
                                oldModel.Gender = propModel.Gender;
                                propBll.Update(oldModel);
                            }
                        }
                        else
                            result = "提交失败";
                    }
                    else
                    {
                        var list0 = propBll.GetList(s => s.MaterialSupport == propModel.MaterialSupport.ToUpper() && s.PropName.ToLower() == propModel.PropName.ToLower());
                        if (list0.Any())
                        {
                            result = "道具名称已存在";
                        }
                        else
                        {
                            KidsNewShopProp newModel = new KidsNewShopProp();
                            newModel.GraphicMaterial = propModel.GraphicMaterial;
                            newModel.MaterialSupport = propModel.MaterialSupport.ToUpper();
                            newModel.PositionDescription = propModel.PositionDescription;
                            newModel.PropName = propModel.PropName;
                            newModel.Quantity = propModel.Quantity;
                            newModel.Remark = propModel.Remark;
                            newModel.Gender = propModel.Gender;
                            newModel.AddDate = DateTime.Now;
                            newModel.AddUserId = new BasePage().CurrentUser.UserId;
                            propBll.Add(newModel);
                        }
                    }
                }
            }
            return result;
        }

        string DeleteProp()
        {
            string result = "ok";
            int id = 0;
            if (context1.Request.QueryString["id"] != null)
            {
                id = int.Parse(context1.Request.QueryString["id"]);
                KidsNewShopPropBLL propBll = new KidsNewShopPropBLL();
                KidsNewShopProp model = propBll.GetModel(id);
                if (model != null)
                {
                    propBll.Delete(model);
                }
                else
                    result = "删除失败";
            }
            else
                result = "删除失败";
            return result;
        }

        string GetProp()
        {
            string materialSupport = string.Empty;
            if (context1.Request.QueryString["MaterialSupport"] != null)
            {
                materialSupport = context1.Request.QueryString["MaterialSupport"];
            }
            string result = string.Empty;
            var list = new KidsNewShopPropBLL().GetList(s => s.Id > 0 && s.MaterialSupport == materialSupport.ToUpper());

            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"PropName\":\"" + s.PropName + "\",\"Quantity\":\"" + s.Quantity + "\",\"Gender\":\"" + s.Gender + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"Remark\":\"" + s.Remark + "\"},");
                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
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
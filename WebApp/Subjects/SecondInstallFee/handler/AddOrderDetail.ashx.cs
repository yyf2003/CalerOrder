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

namespace WebApp.Subjects.SecondInstallFee.handler
{
    /// <summary>
    /// AddOrderDetail 的摘要说明
    /// </summary>
    public class AddOrderDetail : IHttpHandler
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
                case "getPOPList":
                    result = GetPOPList();
                    break;
                case "deleteOrder":
                    result = DeleteOrder();
                    break;
                case "addOrder":
                    result = AddOrder();
                    break;
                case "getOrder":
                    result = GetOrder();
                    break;
                case "editOrder":
                    result = EditOrder();
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
            if (context1.Request.QueryString["shopName"] != null)
            {
                shopName = context1.Request.QueryString["shopName"];
            }
            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            shop,
                        }).OrderBy(s => s.order.ShopId).ThenBy(s => s.order.OrderType).ThenBy(s=>s.order.Sheet).ToList();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.shop.ShopNo.ToUpper().Contains(shopNo.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(shopName))
            {
                list = list.Where(s => s.shop.ShopName.ToUpper().Contains(shopName.ToUpper())).ToList();
            }
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    int orderType = s.order.OrderType ?? 1;
                    string type = CommonMethod.GeEnumName<OrderTypeEnum>(orderType.ToString());
                    string addDate = string.Empty;
                    if (s.order.AddDate != null)
                        addDate = s.order.AddDate.ToString();
                    json.Append("{\"Id\":\"" + s.order.Id + "\",\"OrderType\":\"" + type + "\",\"ShopId\":\"" + s.order.ShopId + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"RegionName\":\"" + s.shop.RegionName + "\",\"ProvinceName\":\"" + s.shop.ProvinceName + "\",\"CityName\":\"" + s.shop.CityName + "\",\"MaterialSupport\":\"" + s.order.MaterialSupport + "\",\"POSScale\":\"" + s.order.POSScale + "\",\"Channel\":\"" + s.shop.Channel + "\",\"Format\":\"" + s.shop.Format + "\",\"Sheet\":\"" + s.order.Sheet + "\",\"GraphicNo\":\"" + s.order.GraphicNo + "\",\"Gender\":\"" + s.order.Gender + "\",\"Quantity\":\"" + s.order.Quantity + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",\"ChooseImg\":\"" + s.order.ChooseImg + "\",\"Remark\":\"" + s.order.Remark + "\",\"IsApprove\":\"" + (s.order.ApproveState ?? 0) + "\",\"AddDate\":\"" + addDate + "\",\"Price\":\"" + s.order.Price + "\",\"PayPrice\":\"" + s.order.PayPrice + "\"},");

                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
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

        string GetPOPList()
        {
            string result = string.Empty;
            string shopNo = string.Empty;
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
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                var shopModel = new ShopBLL().GetList(s => s.ShopNo.ToLower() == shopNo.ToLower() && (s.IsDelete == null || s.IsDelete == false)).FirstOrDefault();
                if (shopModel != null)
                {
                    int shopId = shopModel.Id;
                    //获取这个店铺已下完的订单
                    var orderList = new ExtraOrderDetailBLL().GetList(s => s.SubjectId == subjectId && s.ShopId == shopId && s.Sheet!=null && s.GraphicNo!=null);
                    var popList = new POPBLL().GetList(s => s.ShopId == shopId);
                    if (popList.Any())
                    {
                        StringBuilder popJson = new StringBuilder();
                        popList.ForEach(s =>
                        {
                            bool canWork = true;
                            if (orderList.Any())
                            {
                                var existOrder = orderList.Where(order => order.Sheet.ToLower() == s.Sheet.ToLower() && order.GraphicNo== s.GraphicNo).FirstOrDefault();
                                canWork = existOrder == null;
                            }
                            if (canWork)
                            {
                                int isValid = (s.IsValid ?? true) ? 1 : 0;
                                popJson.Append("{\"Id\":\"" + s.Id + "\",\"ShopId\":\"" + s.ShopId + "\",\"ShopName\":\"" + shopModel.ShopName+ "\",\"Format\":\""+shopModel.Format+"\",\"Sheet\":\"" + s.Sheet + "\",\"GraphicNo\":\"" + s.GraphicNo + "\",\"Gender\":\"" + s.Gender + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"Remark\":\"" + s.Remark + "\",\"IsValid\":\"" + isValid + "\",\"OrderType\":\"1\",\"OrderTypeName\":\"POP\"},");
                            }
                        });
                        result = "[" + popJson.ToString().TrimEnd(',') + "]";
                    }
                }
                else
                {
                    flag = false;
                    errorMsg.Append("店铺不存在");
                }
            }
            if (!flag)
            {
                result = "error|" + errorMsg.ToString();
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
                                    canSave = new BasePage().CheckMaterial(customerId,s.GraphicMaterial);
                                }
                                if (canSave)
                                {

                                    
                                    orderModel = new RegionOrderDetail();
                                    orderModel.OrderType = (int)Enum.Parse(typeof(OrderTypeEnum),s.OrderTypeName);
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
                                    orderModel.Sheet = s.Sheet;
                                    orderModel.ShopId = s.ShopId;
                                    orderModel.SubjectId = s.SubjectId;
                                    orderModel.ChooseImg = s.ChooseImg;
                                    orderModel.IsValid = s.IsValid;
                                    if ((s.Price ?? 0) > 0)
                                    {
                                        orderModel.Price = s.Price;
                                        if ((s.PayPrice ?? 0) > 0)
                                            orderModel.PayPrice = s.PayPrice;
                                        else
                                            orderModel.PayPrice = 150;
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
                        result = ex.Message;
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
            //ExtraOrderDetail model = new ExtraOrderDetailBLL().GetModel(id);
            var model = (from order in CurrentContext.DbContext.RegionOrderDetail
                         join shop in CurrentContext.DbContext.Shop
                         on order.ShopId equals shop.Id
                         where order.Id == id
                         select new
                         {
                             order,
                             shop
                         }).FirstOrDefault();
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((model.order.OrderType ?? 0).ToString());
                string price = model.order.Price != null ? model.order.Price.ToString() : "";
                string payPrice = model.order.PayPrice != null ? model.order.PayPrice.ToString() : "";
                json.Append("{\"Id\":\"" + model.order.Id + "\",\"ShopNo\":\"" + model.shop.ShopNo + "\",\"ShopName\":\"" + model.shop.ShopName + "\",\"ChooseImg\":\"" + model.order.ChooseImg + "\",\"Gender\":\"" + model.order.Gender + "\",\"GraphicLength\":\"" + model.order.GraphicLength + "\",\"GraphicMaterial\":\"" + model.order.GraphicMaterial + "\",\"GraphicWidth\":\"" + model.order.GraphicWidth + "\",\"HandMakeSubjectId\":\"" + model.order.HandMakeSubjectId + "\",\"MaterialSupport\":\"" + model.order.MaterialSupport + "\",\"OrderTypeName\":\"" + orderType + "\",\"PositionDescription\":\"" + model.order.PositionDescription + "\",\"POSScale\":\"" + model.order.POSScale + "\",\"Quantity\":\"" + model.order.Quantity + "\",\"Remark\":\"" + model.order.Remark + "\",\"Sheet\":\"" + model.order.Sheet + "\",\"GraphicNo\":\"" + (model.order.GraphicNo ?? "") + "\",\"Price\":\"" + price + "\",\"PayPrice\":\"" + payPrice + "\"},");
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
                            string orderType = CommonMethod.GetEnumDescription<OrderTypeEnum>((orderModel.OrderType??1).ToString());
                            if (orderType == "费用订单")
                            {
                                orderModel.OrderType = (int)Enum.Parse(typeof(OrderTypeEnum), newOrder.OrderTypeName);
                                orderModel.Price = newOrder.Price;
                                if ((newOrder.PayPrice??0)==0)
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
                                orderModel.Sheet = newOrder.Sheet;
                                orderModel.GraphicLength = newOrder.GraphicLength;
                                orderModel.GraphicWidth = newOrder.GraphicWidth;
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
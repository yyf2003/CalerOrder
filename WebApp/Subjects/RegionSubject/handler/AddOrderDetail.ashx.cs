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
                case "getOrderShopList":
                    result = GetOrderShopList();
                    break;
                case "getMaterialList":
                    result = GetMaterialList();
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
                case "deleteMaterail":
                    result = DeleteMaterial();
                    break;
                case "getCustomerMaterail":
                    result = GetCustomerMaterial();
                    break;
                case "getSheetList":
                    result = GetSheetList();
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
            int shopId = 0;
            string shopNo = string.Empty;
            string shopName = string.Empty;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            if (context1.Request.QueryString["shopId"] != null)
            {
                shopId = int.Parse(context1.Request.QueryString["shopId"]);
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
                        join subject in CurrentContext.DbContext.Subject
                        on order.HandMakeSubjectId equals subject.Id into subjectTemp
                        from subject1 in subjectTemp.DefaultIfEmpty()
                        where order.SubjectId == subjectId
                        && (shopId > 0 ? order.ShopId == shopId : 1 == 1)
                        select new
                        {
                            order,
                            shop,

                            subject1.SubjectName
                        }).OrderByDescending(s => s.order.AddDate).ThenBy(s => s.shop.ShopNo).ToList();
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
                    string type = orderType == 1 ? "POP" : "道具";
                    string addDate = string.Empty;
                    if (s.order.AddDate != null)
                        addDate = s.order.AddDate.ToString();
                    json.Append("{\"Id\":\"" + s.order.Id + "\",\"OrderType\":\"" + type + "\",\"SubjectName\":\"" + s.SubjectName + "\",\"ShopId\":\"" + s.order.ShopId + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"RegionName\":\"" + s.shop.RegionName + "\",\"ProvinceName\":\"" + s.shop.ProvinceName + "\",\"CityName\":\"" + s.shop.CityName + "\",\"MaterialSupport\":\"" + s.order.MaterialSupport + "\",\"POSScale\":\"" + s.order.POSScale + "\",\"Channel\":\"" + s.shop.Channel + "\",\"Format\":\"" + s.shop.Format + "\",\"Sheet\":\"" + s.order.Sheet + "\",\"GraphicNo\":\"" + s.order.GraphicNo + "\",\"Gender\":\"" + s.order.Gender + "\",\"Quantity\":\"" + s.order.Quantity + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",\"ChooseImg\":\"" + s.order.ChooseImg + "\",\"Remark\":\"" + s.order.Remark + "\",\"IsApprove\":\"" + (s.order.ApproveState ?? 0) + "\",\"AddDate\":\"" + addDate + "\"},");

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
                        join subject in CurrentContext.DbContext.Subject
                        on order.HandMakeSubjectId equals subject.Id into subjectTemp
                        from subject1 in subjectTemp.DefaultIfEmpty()
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
                    bool isHc = false;
                    if (!string.IsNullOrWhiteSpace(shopModel.Format))
                    {
                        string format = shopModel.Format.ToUpper();
                        if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE") || format.Contains("YA"))
                        {
                            isHc = true;
                        }
                    }
                    if (isHc)
                    {

                        int shopId = shopModel.Id;
                        string materialSupport = string.Empty;
                        string posScale = string.Empty;
                        SubjectBLL subjectBll = new SubjectBLL();
                        Subject subjectModel = subjectBll.GetModel(subjectId);
                        bool isOk = true;
                        if (subjectModel != null)
                        {
                            int guidanceId = subjectModel.GuidanceId ?? 0;
                            var orderList = from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                            join subject in CurrentContext.DbContext.Subject
                                            on order.SubjectId equals subject.Id
                                            where order.ShopId == shopId
                                            && subject.GuidanceId == guidanceId
                                            && (order.IsDelete==null || order.IsDelete==false)
                                            select order;
                            if (orderList.Any())
                            {
                                var notHcList = orderList.Where(s => s.Format != null && !s.Format.ToUpper().Contains("HC") && !s.Format.ToUpper().Contains("YA")).ToList();
                                if (notHcList.Any())
                                {
                                    isOk = false;
                                    result = "error|该店铺已下非HC订单";
                                }
                                //else
                                //{
                                //    isOk = false;
                                //    result = "error|该店铺订单已下完";
                                //}
                            }
                        }
                        if (isOk)
                        {
                            materialSupport = new BasePage().GetShopMaterialSupport(subjectModel.GuidanceId ?? 0, shopId);
                            //获取这个店铺已下完的订单
                            var orderList = new RegionOrderDetailBLL().GetList(s => s.SubjectId == subjectId && s.ShopId == shopId);

                            var popList = new POPBLL().GetList(s => s.ShopId == shopId);
                            string popJosnStr = string.Empty;
                            string subjectJosnStr = string.Empty;
                            if (popList.Any())
                            {
                                StringBuilder popJson = new StringBuilder();
                                popList.ForEach(s =>
                                {
                                    bool canWork = true;
                                    if (orderList.Any())
                                    {
                                        var existOrder = orderList.Where(order => order.Sheet.ToLower() == s.Sheet.ToLower() && order.GraphicNo == s.GraphicNo).FirstOrDefault();
                                        canWork = existOrder == null;
                                    }
                                    if (canWork)
                                        popJson.Append("{\"Id\":\"" + s.Id + "\",\"ShopId\":\"" + s.ShopId + "\",\"Sheet\":\"" + s.Sheet + "\",\"GraphicNo\":\"" + s.GraphicNo + "\",\"Gender\":\"" + s.Gender + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"Remark\":\"" + s.Remark + "\",\"MaterialSupport\":\"" + materialSupport.ToUpper() + "\",\"POSScale\":\"" + posScale + "\"},");

                                });
                                popJosnStr = "[" + popJson.ToString().TrimEnd(',') + "]";

                                
                            }
                            if (subjectModel != null)
                            {
                                StringBuilder subjectJson = new StringBuilder();
                                //var subjectList = subjectBll.GetList(s=>s.GuidanceId==subjectModel.GuidanceId && s.ApproveState==1 && (s.IsDelete==null || s.IsDelete==false) && (s.SubjectType??1)==(int)SubjectTypeEnum.POP订单);
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
                                                   where subject.GuidanceId == subjectModel.GuidanceId
                                                   && subject.ApproveState == 1
                                                   && (subject.IsDelete == null || subject.IsDelete == false)
                                                   && (((subject.SubjectType ?? 1) == (int)SubjectTypeEnum.POP订单) || ((subject.SubjectType ?? 1) == (int)SubjectTypeEnum.手工订单))
                                                   && (regionList.Any() ? (regionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                                                   select subject).Distinct().OrderBy(s => s.Id).ToList();
                                if (subjectList.Any())
                                {
                                    subjectList.ForEach(s =>
                                    {
                                        subjectJson.Append("{\"ShopId\":\"" + shopId + "\",\"SubjectId\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                                    });

                                }
                                var notPOPSubject = subjectBll.GetList(s => (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && s.RegionOrderType == 1 && s.GuidanceId == subjectModel.GuidanceId);
                                if (regionList.Any())
                                {
                                    notPOPSubject = notPOPSubject.Where(s => (s.Region != null && s.Region != "") ? (regionList.Contains(s.Region.ToLower())) : 1 == 1).ToList();
                                }
                                notPOPSubject.ForEach(s =>
                                {
                                    subjectJson.Append("{\"ShopId\":\"" + shopId + "\",\"SubjectId\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                                });
                                subjectJosnStr = "[" + subjectJson.ToString().TrimEnd(',') + "]";
                            }
                            result = popJosnStr + "|" + subjectJosnStr;
                        }
                    }
                    else
                        result = "error|该店铺不是HC店";
                }
                else
                    result = "error|店铺不存在";
            }
            return result;
        }

        string GetMaterialList()
        {
            string result = string.Empty;
            int subjectId = 0;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            var list = (from material in CurrentContext.DbContext.OrderMaterial
                        join shop in CurrentContext.DbContext.Shop
                        on material.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on material.SubjectId equals subject.Id

                        where material.RegionSupplementId == subjectId
                        select new
                        {
                            shop,
                            material,
                            subject.SubjectName

                        }).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.material.MaterialId + "\",\"SubjectName\":\"" + s.SubjectName + "\",\"ShopId\":\"" + s.material.ShopId + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"RegionName\":\"" + s.shop.RegionName + "\",\"ProvinceName\":\"" + s.shop.ProvinceName + "\",\"CityName\":\"" + s.shop.CityName + "\",\"Sheet\":\"" + s.material.Sheet + "\",\"MaterialName\":\"" + s.material.MaterialName + "\",\"MaterialCount\":\"" + s.material.MaterialCount + "\",\"MaterialLength\":\"" + s.material.MaterialLength + "\",\"MaterialWidth\":\"" + s.material.MaterialWidth + "\",\"MaterialHigh\":\"" + s.material.MaterialHigh + "\",\"Price\":\"" + s.material.Price + "\",\"Remark\":\"" + s.material.Remark + "\"},");

                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;

        }
        RegionOrderDetailBLL orderBll = new RegionOrderDetailBLL();
        string AddOrder()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
                //jsonStr = HttpUtility.UrlDecode(jsonStr);
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
                            int shopId = orderList[0].ShopId ?? 0;
                            int subjectId = orderList[0].SubjectId ?? 0;

                            StringBuilder msg = new StringBuilder();
                            int successNum = 0;
                            orderList.ForEach(s =>
                            {
                                bool canSave = true;
                                if (s.OrderType == 1)
                                {
                                    canSave = CheckMaterial(s.GraphicMaterial);
                                }

                                if (canSave)
                                {
                                    if (!CheckPOPIsExist(subjectId, s.ShopId ?? 0, s.Sheet, s.GraphicNo))
                                    {


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
                                        orderModel.Sheet = s.Sheet;
                                        orderModel.ShopId = s.ShopId;
                                        orderModel.SubjectId = s.SubjectId;
                                        orderModel.HandMakeSubjectId = s.HandMakeSubjectId;
                                        orderModel.ChooseImg = s.ChooseImg;
                                        orderBll.Add(orderModel);
                                        successNum++;
                                    }
                                }
                                else
                                {
                                    msg.Append(s.GraphicMaterial + "，");
                                }
                            });
                            if (msg.Length > 0)
                            {
                                //if (successNum > 0)
                                //{
                                //    result = "提交失败：以下材质不正确："+msg.ToString();
                                //}
                                //else
                                //    result = "提交失败：以下材质不正确：" + msg.ToString();
                                //result += "请先更新基础数据库再提交！";
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
                        result = "提交失败：<br/>" + ex.Message;
                    }
                }
            }
            else
            {
                result = "提交失败：没有可提交的数据";
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
                    if (newOrder != null && newOrder.Id > 0)
                    {

                        RegionOrderDetail orderModel = orderBll.GetModel(newOrder.Id);
                        if (orderModel != null)
                        {

                            orderModel.Gender = newOrder.Gender;
                            orderModel.MaterialSupport = newOrder.MaterialSupport;
                            orderModel.PositionDescription = newOrder.PositionDescription;
                            orderModel.POSScale = newOrder.POSScale;
                            orderModel.Quantity = newOrder.Quantity;
                            orderModel.Remark = newOrder.Remark;
                            orderModel.Sheet = newOrder.Sheet;
                            orderModel.HandMakeSubjectId = newOrder.HandMakeSubjectId;
                            orderModel.ChooseImg = newOrder.ChooseImg;
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

        string DeleteMaterial()
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
                        new OrderMaterialBLL().Delete(s => idList.Contains(s.MaterialId));
                    }
                    catch (Exception ex)
                    {
                        result = "删除失败：" + ex.Message;
                    }
                }
            }
            return result;
        }

        string GetCustomerMaterial()
        {
            string result = string.Empty;
            int subjectId = 0;
            if (context1.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context1.Request.QueryString["subjectId"]);
            }
            //Subject subjectModel = new SubjectBLL().GetModel(subjectId);
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join guidance in CurrentContext.DbContext.SubjectGuidance
                                on subject.GuidanceId equals guidance.ItemId
                                where subject.Id == subjectId
                                select new
                                {
                                    subject,
                                    guidance.PriceItemId
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                int customerId = subjectModel.subject.CustomerId ?? 0;
                int priceItemId = subjectModel.PriceItemId ?? 0;
                //var list = new CustomerMaterialInfoBLL().GetList(s => s.CustomerId == customerId).OrderBy(s=>s.BasicCategoryId).ToList();
                var list = (from cm in CurrentContext.DbContext.CustomerMaterialInfo
                            join bm in CurrentContext.DbContext.BasicMaterial
                            on cm.BasicMaterialId equals bm.Id
                            where cm.CustomerId == customerId && cm.PriceItemId == priceItemId
                            select bm).ToList();

                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        json.Append("{\"MaterialName\":\"" + s.MaterialName + "\"},");
                    });
                    result = "[" + json.ToString().TrimEnd(',') + "]";
                }
            }
            return result;
        }

        string GetSheetList()
        {
            var list = (from pop in CurrentContext.DbContext.POP
                        join shop in CurrentContext.DbContext.Shop
                        on pop.ShopId equals shop.Id
                        where shop.Format.ToLower().Contains("hc")
                        select pop.Sheet).Distinct().OrderBy(s => s).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"SheetName\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        List<string> materialList = new List<string>();
        bool CheckMaterial(string materialName)
        {
            bool flag = true;
            materialName = materialName.Trim().ToLower();
            if (materialList.Contains(materialName))
            {
                flag = true;
            }
            else
            {
                OrderMaterialMpping materialModel = new OrderMaterialMppingBLL().GetList(s => s.OrderMaterialName.ToLower() == materialName).FirstOrDefault();
                if (materialModel != null)
                    materialList.Add(materialName);
                else
                    flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 检查是否下重复了
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="shopId"></param>
        /// <param name="sheet"></param>
        /// <param name="graphicNo"></param>
        /// <returns></returns>
        bool CheckPOPIsExist(int subjectId, int shopId, string sheet, string graphicNo)
        {
            if (!string.IsNullOrWhiteSpace(graphicNo))
            {
                var list = orderBll.GetList(s => s.SubjectId == subjectId && s.ShopId == shopId && s.Sheet.ToUpper() == sheet.ToUpper() && (s.GraphicNo != null && s.GraphicNo != "" && s.GraphicNo.ToUpper() == graphicNo.ToUpper()));
                return list.Any();
            }
            else
                return false;
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
                json.Append("{\"Id\":\"" + model.Id + "\",\"ChooseImg\":\"" + model.ChooseImg + "\",\"Gender\":\"" + model.Gender + "\",\"GraphicLength\":\"" + model.GraphicLength + "\",\"GraphicMaterial\":\"" + model.GraphicMaterial + "\",\"GraphicWidth\":\"" + model.GraphicWidth + "\",\"HandMakeSubjectId\":\"" + model.HandMakeSubjectId + "\",\"MaterialSupport\":\"" + model.MaterialSupport + "\",\"OrderType\":\"" + orderType + "\",\"PositionDescription\":\"" + model.PositionDescription + "\",\"POSScale\":\"" + model.POSScale + "\",\"Quantity\":\"" + model.Quantity + "\",\"Remark\":\"" + model.Remark + "\",\"Sheet\":\"" + model.Sheet + "\",\"GraphicNo\":\"" + model.GraphicNo + "\"},");
                //获取项目List信息
                SubjectBLL subjectBll = new SubjectBLL();
                Subject subjectModel = subjectBll.GetModel(model.SubjectId ?? 0);
                if (subjectModel != null)
                {
                    StringBuilder subjectJson = new StringBuilder();
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
                                       where subject.GuidanceId == subjectModel.GuidanceId
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
                        result = "[" + json.ToString().TrimEnd(',') + "]";
                    }
                    var notPOPSubject = subjectBll.GetList(s => (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && s.RegionOrderType == 1);
                    notPOPSubject.ForEach(s =>
                    {
                        subjectJson.Append("{\"SubjectId\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                    });
                    result += "|[" + subjectJson.ToString().TrimEnd(',') + "]";
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
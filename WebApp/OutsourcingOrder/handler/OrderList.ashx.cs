using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using DAL;
using Models;
using Common;
using Newtonsoft.Json;
using System.Configuration;

namespace WebApp.OutsourcingOrder.handler
{
    /// <summary>
    /// OrderList 的摘要说明
    /// </summary>
    public class OrderList : IHttpHandler
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
                case "getOutsource":
                    result = GetOutsourceList();
                    break;
                case "getGuidanceList":
                    result = GetGuidanceList();
                    break;
                case "getOrderList":
                    result = GetOrderList();
                    break;
                case "getMaterialCategory":
                    result = GetMaterialCategory();
                    break;
                case "getProvince":
                    result = GetProvinceList();
                    break;
                case "getCity":
                    result = GetCityList();
                    break;
                case "getSubjectList":
                    result = GetSubjectList();
                    break;
                case "getOrder":
                    result = GetModel();
                    break;
                case "edit":
                    result = Edit();
                    break;
                case "delete":
                    result = DeleteOrder();
                    break;
                case "recover":
                    result = RecoverOrder();
                    break;
                case "changeOutsource":
                    result = ChangeOutsource();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetGuidanceList()
        {
            string result = string.Empty;
            string outsourceId = string.Empty;
            int customerId = 0;
            string guidanceMonth = string.Empty;
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            if (context1.Request.QueryString["guidanceMonth"] != null)
            {
                guidanceMonth = context1.Request.QueryString["guidanceMonth"];
            }
            var list = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on order.GuidanceId equals guidance.ItemId
                        where guidance.CustomerId == customerId
                        && outsourceList.Contains(order.OutsourceId ?? 0)
                        select guidance).Distinct().ToList();
            DateTime date = DateTime.Now;
            int year = date.Year;
            int month = date.Month;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date0 = DateTime.Parse(guidanceMonth);
                year = date0.Year;
                month = date0.Month;

            }
            list = list.Where(s => s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"GuidanceId\":\"" + s.ItemId + "\",\"GuidanceName\":\"" + s.ItemName + "\"},");
                });
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetSubjectList()
        {
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.QueryString["outsourceId"]))
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = context1.Request.QueryString["guidanceIds"];
            }
            List<int> guidanceList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                guidanceList = StringHelper.ToIntList(guidanceIds, ',');
            }
            var subjectList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                               join subject in CurrentContext.DbContext.Subject
                               on order.SubjectId equals subject.Id
                               where outsourceList.Contains(order.OutsourceId ?? 0)
                             && guidanceList.Contains(subject.GuidanceId ?? 0)
                               select subject).Distinct().ToList();
            if (subjectList.Any())
            {
                StringBuilder json = new StringBuilder();
                subjectList.ForEach(s =>
                {
                    json.Append("{\"SubjectId\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetOutsourceList()
        {
            string result = string.Empty;
            var companyList = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.IsDelete == null || s.IsDelete == false)).OrderBy(s => s.ProvinceId).ToList();
            if (companyList.Any())
            {
                if (new BasePage().CurrentUser.RoleId == 5)
                {
                    int userId = new BasePage().CurrentUser.UserId;
                    List<int> outsourceList = new OutsourceInUserBLL().GetList(s => s.UserId == userId).Select(s => s.OutsourceId ?? 0).ToList();
                    companyList = companyList.Where(s => outsourceList.Contains(s.Id)).ToList();
                }
                StringBuilder json = new StringBuilder();
                companyList.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CompanyName\":\"" + s.CompanyName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

        string GetOrderList()
        {
            string result = string.Empty;
            int currPage = 0, pageSize = 0;
            if (context1.Request.QueryString["currpage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            string outsourceType = string.Empty;
            string shopNo = string.Empty;
            string exportType = string.Empty;
            string materialName = string.Empty;
            string materialCategoryId = string.Empty;
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null && !string.IsNullOrWhiteSpace(context1.Request.QueryString["outsourceId"]))
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = context1.Request.QueryString["guidanceIds"];
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context1.Request.QueryString["subjectIds"];
            }
            if (context1.Request.QueryString["outsourceType"] != null)
            {
                outsourceType = context1.Request.QueryString["outsourceType"];
            }
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }
            if (context1.Request.QueryString["materialCategoryId"] != null)
            {
                materialCategoryId = context1.Request.QueryString["materialCategoryId"];
            }

            if (context1.Request.QueryString["exportType"] != null)
            {
                exportType = context1.Request.QueryString["exportType"];
            }
            if (context1.Request.QueryString["materialName"] != null)
            {
                materialName = context1.Request.QueryString["materialName"];
            }
            List<int> guidanceList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                guidanceList = StringHelper.ToIntList(guidanceIds, ',');
            }
            List<int> subjectList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectList = StringHelper.ToIntList(subjectIds, ',');
            }
            List<int> outsourceTypeList = new List<int>();
            if (!string.IsNullOrWhiteSpace(outsourceType))
            {
                outsourceTypeList = StringHelper.ToIntList(outsourceType, ',');
            }
            List<string> shopNoList = new List<string>();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                shopNoList = StringHelper.ToStringList(shopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower);
            }
            List<int> materialCategoryIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(materialCategoryId))
            {
                materialCategoryIdList = StringHelper.ToIntList(materialCategoryId, ',');
            }
            var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                             //join assign in CurrentContext.DbContext.OutsourceAssignShop
                             //on order.ShopId equals assign.ShopId
                             where outsourceList.Contains(order.OutsourceId ?? 0)
                                 //&& order.OutsourceId == outsourceId
                                 // && guidanceList.Contains(assign.GuidanceId ?? 0)
                            && guidanceList.Contains(order.GuidanceId ?? 0)
                             select
                                 //new {
                                 order
                //assign
                //}
                            ).ToList();
            if (subjectList.Any())
            {
                orderList = orderList.Where(s => subjectList.Contains(s.SubjectId ?? 0) || s.SubjectId == 0).ToList();
            }
            if (!string.IsNullOrWhiteSpace(exportType))
            {
                if (exportType == "nohc")
                {
                    orderList = orderList.Where(s => (s.Format != null && s.Format != "") ? (s.Format.ToLower().IndexOf("hc") == -1 && s.Format.ToLower().IndexOf("homecourt") == -1 && s.Format.ToLower().IndexOf("homecore") == -1 && s.Format.ToLower().IndexOf("ya") == -1) : 1 == 1).ToList();
                }
                else if (exportType == "hc")
                {
                    orderList = orderList.Where(s => (s.Format != null && s.Format != "") ? (s.Format.ToLower().IndexOf("hc") != -1 || s.Format.ToLower().IndexOf("homecourt") != -1 || s.Format.ToLower().IndexOf("homecore") != -1 || s.Format.ToLower().IndexOf("ya") != -1) : 1 == 1).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (materialName == "软膜")
                {
                    orderList = orderList.Where(s => s.GraphicMaterial != null && s.GraphicMaterial.Contains("软膜")).ToList();
                }
                else if (materialName == "非软膜")
                {
                    orderList = orderList.Where(s => s.GraphicMaterial == null || s.GraphicMaterial == "" || (s.GraphicMaterial != null && !s.GraphicMaterial.Contains("软膜"))).ToList();
                }
            }
            if (materialCategoryIdList.Any())
            {
                bool hasEmpty = false;
                List<string> materialList = new List<string>();
                if (materialCategoryIdList.Contains(0))
                {
                    hasEmpty = true;
                    materialCategoryIdList.Remove(0);
                }
                if (materialCategoryIdList.Any())
                {
                    materialList = new OrderMaterialMppingBLL().GetList(s => materialCategoryIdList.Contains(s.BasicCategoryId ?? 0)).Select(s => s.OrderMaterialName.ToLower()).ToList();

                }
                if (hasEmpty)
                {
                    if (materialList.Any())
                    {
                        orderList = orderList.Where(s => materialList.Contains(s.GraphicMaterial.ToLower()) || (s.GraphicMaterial == null || s.GraphicMaterial == "")).ToList();

                    }
                    else
                        orderList = orderList.Where(s => (s.GraphicMaterial == null || s.GraphicMaterial == "")).ToList();
                }
                else
                    orderList = orderList.Where(s => materialList.Contains(s.GraphicMaterial.ToLower())).ToList();
            }

            if (outsourceTypeList.Any())
            {
                orderList = orderList.Where(s => outsourceTypeList.Contains(s.AssignType ?? 0)).ToList();
            }
            if (shopNoList.Any())
            {
                orderList = orderList.Where(s => shopNoList.Contains(s.ShopNo.ToLower())).ToList();
            }
            if (orderList.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = orderList.Count;
                orderList = orderList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).ThenBy(s => s.Sheet).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                orderList.ForEach(s =>
                {
                    string gender = s.Gender;
                    string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.OrderType ?? 1).ToString());
                    string orderPrice = string.Empty;
                    string receiveOrderPrice = string.Empty;
                    int Quantity = s.Quantity ?? 1;
                    if ((s.PayOrderPrice ?? 0) > 0)
                    {
                        orderPrice = (s.PayOrderPrice ?? 0).ToString();
                        receiveOrderPrice = ((s.ReceiveOrderPrice ?? 0) * Quantity).ToString();
                        //Quantity = 1;
                    }
                    json.Append("{\"rowIndex\":\"" + index + "\",\"OrderType\":\"" + orderType + "\",\"Id\":\"" + s.Id + "\",\"ShopNo\":\"" + s.ShopNo + "\",\"ShopName\":\"" + s.ShopName + "\",\"Region\":\"" + s.Region + "\",\"Province\":\"" + s.Province + "\",\"City\":\"" + s.City + "\",\"CityTier\":\"" + s.CityTier + "\",\"Channel\":\"" + s.Channel + "\",\"Format\":\"" + s.Format + "\",\"Sheet\":\"" + s.Sheet + "\",\"Gender\":\"" + gender + "\",\"Quantity\":\"" + Quantity + "\",\"PositionDescription\":\"" + s.PositionDescription + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicMaterial\":\"" + s.OrderGraphicMaterial + "\",\"ChooseImg\":\"" + s.ChooseImg + "\",\"OrderPrice\":\"" + orderPrice + "\",\"ReceiveOrderPrice\":\"" + receiveOrderPrice + "\",\"IsDelete\":\"" + ((s.IsDelete ?? false) ? 1 : 0) + "\"},");
                    index++;
                });
                if (json.Length > 0)
                    return "{\"total\":" + totalCount + ",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
                else
                    return "{\"total\":0,\"rows\":[] }";
            }
            else
                return "{\"total\":0,\"rows\":[] }";
        }

        string GetMaterialCategory()
        {
            string result = string.Empty;
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = context1.Request.QueryString["guidanceIds"];
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context1.Request.QueryString["subjectIds"];
            }
            List<int> guidanceList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceIds))
            {
                guidanceList = StringHelper.ToIntList(guidanceIds, ',');
            }
            List<int> subjectList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectList = StringHelper.ToIntList(subjectIds, ',');
            }
            List<string> orderMaterialList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                              //join assign in CurrentContext.DbContext.OutsourceAssignShop
                                              //on order.ShopId equals assign.ShopId
                                              where outsourceList.Contains(order.OutsourceId ?? 0)
                                              && guidanceList.Contains(order.GuidanceId ?? 0)
                                              && (subjectList.Any() ? subjectList.Contains(order.SubjectId ?? 0) : 1 == 1)
                                              select
                                                  order.GraphicMaterial
                                              ).Distinct().ToList();

            if (orderMaterialList.Any())
            {
                bool isEmpty = false;
                if (orderMaterialList.Contains("") || orderMaterialList.Contains(" ") || orderMaterialList.Contains(null))
                {
                    isEmpty = true;
                    orderMaterialList.Remove("");
                    orderMaterialList.Remove(" ");
                    orderMaterialList.Remove(null);
                }
                var list1 = (from material in orderMaterialList
                             join orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                             on material.ToLower() equals orderMaterial.OrderMaterialName.ToLower()
                             join mcategory in CurrentContext.DbContext.MaterialCategory
                             on orderMaterial.BasicCategoryId equals mcategory.Id into categoryTemp
                             from materialCategory in categoryTemp.DefaultIfEmpty()
                             select new
                             {
                                 materialCategory
                             }).ToList();
                List<int> idList = new List<int>();
                StringBuilder json = new StringBuilder();
                list1.ForEach(s =>
                {
                    if (s.materialCategory != null)
                    {
                        if (!idList.Contains(s.materialCategory.Id))
                        {
                            idList.Add(s.materialCategory.Id);
                            json.Append("{\"CategoryId\":\"" + s.materialCategory.Id + "\",\"CategoryName\":\"" + s.materialCategory.CategoryName + "\"},");
                        }
                    }
                    else
                        isEmpty = true;
                });
                if (isEmpty)
                {
                    json.Append("{\"CategoryId\":\"0\",\"CategoryName\":\"无\"},");
                }
                result = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return result;
        }

        string GetProvinceList()
        {
            string result = string.Empty;
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string subjectIds = string.Empty;
            List<int> guidanceList = new List<int>();
            List<int> subjectList = new List<int>();
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = context1.Request.QueryString["guidanceIds"];
                if (!string.IsNullOrWhiteSpace(guidanceIds))
                {
                    guidanceList = StringHelper.ToIntList(guidanceIds, ',');
                }
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context1.Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            var shopList = (from assign in CurrentContext.DbContext.OutsourceOrderDetail
                            join shop in CurrentContext.DbContext.Shop
                            on assign.ShopId equals shop.Id
                            where outsourceList.Contains(assign.OutsourceId ?? 0)
                            && guidanceList.Contains(assign.GuidanceId ?? 0)
                            && (subjectList.Any() ? subjectList.Contains(assign.SubjectId ?? 0) : 1 == 1)
                            select shop).ToList();

            if (shopList.Any())
            {
                StringBuilder json = new StringBuilder();
                var provinceList = shopList.Select(s => s.ProvinceName).Distinct().OrderBy(s => s).ToList();
                provinceList.ForEach(s =>
                {
                    json.Append("{\"Province\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetCityList()
        {
            string result = string.Empty;
            string outsourceId = string.Empty;
            string guidanceIds = string.Empty;
            string provinces = string.Empty;
            string subjectIds = string.Empty;
            List<int> guidanceList = new List<int>();
            List<int> subjectList = new List<int>();
            List<string> provinceList = new List<string>();
            List<int> outsourceList = new List<int>();
            if (context1.Request.QueryString["outsourceId"] != null)
            {
                outsourceId = context1.Request.QueryString["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = context1.Request.QueryString["guidanceIds"];
                if (!string.IsNullOrWhiteSpace(guidanceIds))
                {
                    guidanceList = StringHelper.ToIntList(guidanceIds, ',');
                }
            }
            if (context1.Request.QueryString["provinces"] != null)
            {
                provinces = context1.Request.QueryString["provinces"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context1.Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            var shopList = (from assign in CurrentContext.DbContext.OutsourceOrderDetail
                            join shop in CurrentContext.DbContext.Shop
                            on assign.ShopId equals shop.Id
                            where outsourceList.Contains(assign.OutsourceId ?? 0)
                            && guidanceList.Contains(assign.GuidanceId ?? 0)
                            && (subjectList.Any() ? subjectList.Contains(assign.SubjectId ?? 0) : 1 == 1)
                            && provinceList.Contains(shop.ProvinceName)
                            select shop).ToList();
            if (shopList.Any())
            {
                StringBuilder json = new StringBuilder();
                var cityList = shopList.Select(s => s.CityName).Distinct().OrderBy(s => s).ToList();
                cityList.ForEach(s =>
                {
                    json.Append("{\"City\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetModel()
        {
            string result = string.Empty;
            int orderId = 0;
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = int.Parse(context1.Request.QueryString["orderId"]);
            }
            OutsourceOrderDetail orderModel = new OutsourceOrderDetailBLL().GetModel(orderId);
            if (orderModel != null)
            {
                StringBuilder json = new StringBuilder();
                int materialCategoryId = 0;
                if (!string.IsNullOrWhiteSpace(orderModel.OrderGraphicMaterial))
                {
                    OrderMaterialMpping materialModel = new OrderMaterialMppingBLL().GetList(s => s.OrderMaterialName.ToLower() == orderModel.OrderGraphicMaterial.ToLower()).FirstOrDefault();
                    if (materialModel != null)
                    {
                        materialCategoryId = materialModel.BasicCategoryId ?? 0;
                    }
                }
                int orderType = orderModel.OrderType ?? 1;
                string orderTypeName = CommonMethod.GeEnumName<OrderTypeEnum>(orderType.ToString());
                json.Append("{\"Id\":\"" + orderId + "\",\"OrderType\":\"" + orderType + "\",\"OrderTypeName\":\"" + orderTypeName + "\",\"SubjectId\":\"" + orderModel.SubjectId + "\",\"ShopId\":\"" + orderModel.ShopId + "\",\"ShopName\":\"" + orderModel.ShopName + "\",\"ShopNo\":\"" + orderModel.ShopNo + "\",\"Sheet\":\"" + orderModel.Sheet + "\",\"POSScale\":\"" + orderModel.POSScale + "\",\"MaterialSupport\":\"" + orderModel.MaterialSupport + "\",\"MachineFrame\":\"" + orderModel.MachineFrame + "\",\"PositionDescription\":\"" + orderModel.PositionDescription + "\",\"Gender\":\"" + (!string.IsNullOrWhiteSpace(orderModel.OrderGender) ? orderModel.OrderGender : orderModel.Gender) + "\",\"Quantity\":\"" + (orderModel.Quantity??1) + "\",\"GraphicLength\":\"" + orderModel.GraphicLength + "\",\"GraphicWidth\":\"" + orderModel.GraphicWidth + "\",\"MaterialCategoryId\":\"" + materialCategoryId + "\",\"GraphicMaterial\":\"" + orderModel.OrderGraphicMaterial + "\",\"ChooseImg\":\"" + orderModel.ChooseImg + "\",\"Remark\":\"" + orderModel.Remark + "\",\"Channel\":\"" + orderModel.Channel + "\",\"Format\":\"" + orderModel.Format + "\",\"CityTier\":\"" + orderModel.CityTier + "\",\"IsInstall\":\"" + orderModel.IsInstall + "\",\"PayOrderPrice\":\"" + (orderModel.PayOrderPrice ?? 0) + "\",\"ReceiveOrderPrice\":\"" + (orderModel.ReceiveOrderPrice ?? 0) + "\"}");
                result = "[" + json.ToString() + "]";
            }
            return result;
        }

        string Edit()
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
                    bool isPriceOrder = false;
                    OutsourceOrderDetail orderModel = JsonConvert.DeserializeObject<OutsourceOrderDetail>(jsonStr);
                    if (orderModel != null)
                    {
                        int orderType = orderModel.OrderType ?? 1;
                        string orderTypeDes = CommonMethod.GetEnumDescription<OrderTypeEnum>(orderType.ToString());
                        if (orderTypeDes.Contains("费用订单"))
                        {
                            isPriceOrder = true;
                        }

                        OutsourceOrderDetailBLL orderBll = new OutsourceOrderDetailBLL();
                       
                        if (orderModel.Id > 0)
                        {
                            OutsourceOrderDetail newOrderModel = orderBll.GetModel(orderModel.Id);
                            if (newOrderModel != null)
                            {

                                List<string> ChangePOPCountSheetList = new List<string>();
                                string changePOPCountSheetStr = string.Empty;
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
                                
                                if (isPriceOrder)
                                {
                                    newOrderModel.ReceiveOrderPrice = orderModel.ReceiveOrderPrice;
                                    newOrderModel.PayOrderPrice = orderModel.PayOrderPrice;
                                    
                                }
                                else
                                {
                                    int Quantity = orderModel.Quantity ?? 1;
                                    if (!string.IsNullOrWhiteSpace(orderModel.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(orderModel.Sheet.ToUpper()))
                                    {
                                        Quantity = Quantity > 0 ? 1 : 0;
                                    }
                                    decimal width = orderModel.GraphicWidth ?? 0;
                                    decimal length = orderModel.GraphicLength ?? 0;
                                    newOrderModel.Area = (width * length) / 1000000;
                                    newOrderModel.MaterialSupport = orderModel.MaterialSupport;
                                    newOrderModel.InstallPriceMaterialSupport = orderModel.MaterialSupport;
                                    newOrderModel.POSScale = orderModel.POSScale;
                                    newOrderModel.ChooseImg = orderModel.ChooseImg;
                                    newOrderModel.OrderGender = orderModel.Gender;
                                    newOrderModel.GraphicLength = length;
                                    
                                    newOrderModel.GraphicWidth = width;
                                    newOrderModel.MachineFrame = orderModel.MachineFrame;
                                    newOrderModel.OrderType = orderModel.OrderType;
                                    newOrderModel.PositionDescription = orderModel.PositionDescription;
                                    newOrderModel.POSScale = orderModel.POSScale;
                                    
                                    newOrderModel.Sheet = orderModel.Sheet;
                                    decimal unitPrice = 0;
                                    decimal totalPrice = 0;
                                   
                                    string material = string.Empty;
                                    string material0 = orderModel.OrderGraphicMaterial;
                                    if (!string.IsNullOrWhiteSpace(material0))
                                        material = new BasePage().GetBasicMaterial(material0);
                                    if (string.IsNullOrWhiteSpace(material))
                                        material = orderModel.OrderGraphicMaterial;
                                    newOrderModel.OrderGraphicMaterial = orderModel.OrderGraphicMaterial;
                                    newOrderModel.GraphicMaterial = material;
                                    if (!string.IsNullOrWhiteSpace(material))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = material0;
                                        pop.GraphicLength = orderModel.GraphicLength;
                                        pop.GraphicWidth = orderModel.GraphicWidth;
                                        pop.Quantity = Quantity;

                                        int subjectId = newOrderModel.SubjectId ?? 0;
                                        var guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                                             on subject.GuidanceId equals guidance.ItemId
                                                             where subject.Id == subjectId
                                                             select new { guidance, subject }).FirstOrDefault();
                                        if (guidanceModel != null)
                                        {
                                            pop.CustomerId = guidanceModel.subject.CustomerId;
                                            pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                            if (guidanceModel.guidance.ActivityTypeId == (int)GuidanceTypeEnum.Delivery)
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
                                            new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                            newOrderModel.UnitPrice = unitPrice;
                                            newOrderModel.TotalPrice = totalPrice;
                                        }
                                        else
                                        {
                                            throw new Exception("获取项目失败");
                                        }
                                    }
                                    newOrderModel.UnitPrice = unitPrice;
                                    newOrderModel.TotalPrice = totalPrice;
                                }
                                newOrderModel.Quantity = orderModel.Quantity;
                                newOrderModel.Remark = orderModel.Remark;
                                orderBll.Update(newOrderModel);
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    result = "提交失败：" + ex.Message;
                }

            }
            else
            {
                result = "提交失败！";
            }
            return result;
        }

        string DeleteOrder()
        {
            string result = "ok";
            string orderId = string.Empty;
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = context1.Request.QueryString["orderId"];
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(orderId))
                {
                    List<int> idList = StringHelper.ToIntList(orderId, ',');
                    OutsourceOrderDetailBLL bll = new OutsourceOrderDetailBLL();
                    var list = bll.GetList(s => idList.Contains(s.Id));
                    if (list.Any())
                    {
                        OutsourceOrderDetail model = null;
                        list.ForEach(s => {
                            model = new OutsourceOrderDetail();
                            model = s;
                            model.IsDelete = true;
                            model.ModifyType = "删除";
                            model.ModifyUserId = new BasePage().CurrentUser.UserId;
                            model.ModifyDate = DateTime.Now;
                            bll.Update(s);
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                result = "删除失败："+ex.Message;
            }
            return result;
        }

        string RecoverOrder()
        {
            string result = "ok";
            string orderId = string.Empty;
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = context1.Request.QueryString["orderId"];
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(orderId))
                {
                    List<int> idList = StringHelper.ToIntList(orderId, ',');
                    OutsourceOrderDetailBLL bll = new OutsourceOrderDetailBLL();
                    var list = bll.GetList(s => idList.Contains(s.Id));
                    if (list.Any())
                    {
                        OutsourceOrderDetail model = null;
                        list.ForEach(s =>
                        {
                            model = new OutsourceOrderDetail();
                            model = s;
                            model.IsDelete = false;
                            model.ModifyType = "恢复";
                            model.ModifyUserId = new BasePage().CurrentUser.UserId;
                            model.ModifyDate = DateTime.Now;
                            bll.Update(s);
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                result = "恢复失败：" + ex.Message;
            }
            return result;
        }

        string ChangeOutsource() {
            string result = "ok";
            string orderId = string.Empty;
            int newOutsourceId = 0;
            if (context1.Request.QueryString["orderId"] != null)
            {
                orderId = context1.Request.QueryString["orderId"];
            }
            if (context1.Request.QueryString["newOutsourceId"] != null)
            {
                newOutsourceId = int.Parse(context1.Request.QueryString["newOutsourceId"]);
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(orderId) && newOutsourceId>0)
                {
                    List<int> idList = StringHelper.ToIntList(orderId, ',');
                    OutsourceOrderDetailBLL bll = new OutsourceOrderDetailBLL();
                    var list = bll.GetList(s => idList.Contains(s.Id));
                    if (list.Any())
                    {
                        OutsourceOrderDetail model = null;
                        list.ForEach(s =>
                        {
                            model = new OutsourceOrderDetail();
                            model = s;
                            model.OutsourceId = newOutsourceId;
                            bll.Update(s);
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                result = "更新失败：" + ex.Message;
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
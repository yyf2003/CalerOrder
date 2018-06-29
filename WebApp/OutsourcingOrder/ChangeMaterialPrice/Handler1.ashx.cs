using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using DAL;
using BLL;
using Common;

namespace WebApp.OutsourcingOrder.ChangeMaterialPrice
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
           
            context.Response.ContentType = "text/plain";
            context1 = context;
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            } 
            switch (type)
            {
                case "updateUnitPrice":
                    result = updateUnitPrice();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string updateUnitPrice()
        {
            string result = "ok";
            int priceItemId = 0;
            List<int> guidanceIdList = new List<int>();
            List<string> orderMaterialList = new List<string>();
            if (context1.Request.Form["priceItemId"] != null)
            {
                priceItemId = int.Parse(context1.Request.Form["priceItemId"]);
            }
            if (context1.Request.Form["guidanceId"] != null)
            {
                string guidanceId = context1.Request.Form["guidanceId"];
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (context1.Request.Form["material"] != null)
            {
                string material = context1.Request.Form["material"];
                orderMaterialList = StringHelper.ToStringList(material, ',', LowerUpperEnum.ToLower);
            }
            string errorMsg = string.Empty;

            try
            {
                var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on order.GuidanceId equals guidance.ItemId
                                 where subject.ApproveState == 1
                                 && (subject.IsDelete == null || subject.IsDelete == false)
                                 && guidanceIdList.Contains(order.GuidanceId ?? 0)
                                 && (order.OrderType == (int)OrderTypeEnum.POP || order.OrderType == (int)OrderTypeEnum.道具)
                                 && order.GraphicMaterial != null && order.GraphicMaterial != ""
                                 select new { guidance.ItemName,guidance.CustomerId, order }).ToList();
                if (orderMaterialList.Any())
                {
                    orderList = orderList.Where(s => orderMaterialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                }
                if (orderList.Any())
                {

                    POP popModel;
                    string unitName = string.Empty;
                    OutsourceOrderDetailBLL orderBll = new OutsourceOrderDetailBLL();
                    Dictionary<int, SubjectGuidance> guidanceDic = new Dictionary<int, SubjectGuidance>();
                    SubjectGuidanceBLL guidanceBll = new SubjectGuidanceBLL();
                    SubjectGuidance guidanceModel;
                    int customerId = orderList[0].CustomerId ?? 0;
                    orderList.ForEach(s =>
                    {
                        popModel = new POP();
                        popModel.Quantity = s.order.Quantity;
                        popModel.PriceItemId = priceItemId;
                        popModel.GraphicLength = s.order.GraphicLength;
                        popModel.GraphicWidth = s.order.GraphicWidth;
                        popModel.GraphicMaterial = s.order.GraphicMaterial;
                        popModel.CustomerId = customerId;
                        int guidanceType = 0;
                        if (guidanceDic.Keys.Contains(s.order.GuidanceId ?? 0))
                        {
                            guidanceModel = guidanceDic[s.order.GuidanceId ?? 0];
                            guidanceType = guidanceModel.ActivityTypeId ?? 0;
                        }
                        else
                        {
                            guidanceModel = guidanceBll.GetModel(s.order.GuidanceId ?? 0);
                            if (guidanceModel != null)
                            {
                                guidanceDic.Add(s.order.GuidanceId ?? 0, guidanceModel);
                                guidanceType = guidanceModel.ActivityTypeId ?? 0;
                            }
                        }
                        if (guidanceType == (int)GuidanceTypeEnum.Promotion)
                        {
                            popModel.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                        }
                        else if (guidanceType == (int)GuidanceTypeEnum.Delivery)
                        {
                            if (s.order.Province == "北京")
                            {
                                popModel.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                            }
                            else
                                popModel.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
                        }
                        else
                        {
                            popModel.OutsourceType = s.order.AssignType;
                        }
                        decimal totalPrice = 0;
                        decimal unitPrice = 0;
                        new BasePage().GetOutsourceOrderMaterialPrice(popModel, out unitPrice, out totalPrice);
                        s.order.UnitPrice = unitPrice;
                        s.order.TotalPrice = totalPrice;
                        orderBll.Update(s.order);


                    });

                }
                else
                {
                    result = "empty";
                }
            }
            catch (Exception ex)
            {

                result = ex.Message;
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
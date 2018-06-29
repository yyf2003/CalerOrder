using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using DAL;
using BLL;
using Common;
using System.Web.SessionState;

namespace WebApp.Subjects.ChangeMaterialPrice
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler,IRequiresSessionState
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
            //bool isOk = true;
            //string errorMsg = string.Empty;
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
                guidanceIdList = StringHelper.ToIntList(guidanceId,',');
            }
            if (context1.Request.Form["material"] != null)
            {
                string material = context1.Request.Form["material"];
                List<string> materialList = StringHelper.ToStringList(material, ',');
                materialList.ForEach(m =>
                {
                    orderMaterialList.AddRange(new BasePage().GetOrderMaterialByBasicMaterial(m, LowerUpperEnum.ToLower));
                });
            }
            try
            {
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on order.GuidanceId equals guidance.ItemId
                                 where subject.ApproveState == 1
                                 && (subject.IsDelete == null || subject.IsDelete == false)
                                 && guidanceIdList.Contains(order.GuidanceId ?? 0)
                                 && (order.OrderType == (int)OrderTypeEnum.POP || order.OrderType == (int)OrderTypeEnum.道具)
                                 && order.GraphicMaterial != null && order.GraphicMaterial != ""
                                 select new { guidance.ItemName, order }).ToList();
                if (orderMaterialList.Any())
                {
                    orderList = orderList.Where(s => orderMaterialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                }
                if (orderList.Any())
                {
                    int finishOrderCount = 0;
                    int totalOrderCount = orderList.Count;
                    //HttpCookie cookie = context1.Request.Cookies["updateIncomePriceTotal"];
                    //if (cookie == null)
                    //{
                    //    cookie = new HttpCookie("updateIncomePriceTotal");
                    //}
                    //cookie.Value = totalOrderCount.ToString();
                    //cookie.Expires = DateTime.Now.AddMinutes(60);
                    //context1.Response.Cookies.Add(cookie);
                    //context1.Session["updateIncomePriceTotal"] = totalOrderCount.ToString();


                    POP popModel;
                    string unitName = string.Empty;
                    FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                    OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();
                    OutsourceOrderDetail outsourceOrder;
                    orderList.ForEach(s =>
                    {
                        popModel = new POP();
                        popModel.Quantity = s.order.Quantity;
                        popModel.PriceItemId = priceItemId;
                        popModel.GraphicLength = s.order.GraphicLength;
                        popModel.GraphicWidth = s.order.GraphicWidth;
                        popModel.GraphicMaterial = s.order.GraphicMaterial;

                        decimal totalPrice = 0;
                        decimal unitPrice =new BasePage().GetMaterialUnitPrice(popModel, out totalPrice, out unitName);
                        s.order.UnitPrice = unitPrice;
                        s.order.TotalPrice = totalPrice;
                        orderBll.Update(s.order);

                        outsourceOrder = outsourceOrderBll.GetList(o => o.FinalOrderId == s.order.Id).FirstOrDefault();
                        if (outsourceOrder != null)
                        {
                            outsourceOrder.ReceiveUnitPrice = unitPrice;
                            outsourceOrder.ReceiveTotalPrice = totalPrice;
                            outsourceOrderBll.Update(outsourceOrder);
                        }
                        finishOrderCount++;
                        //HttpCookie cookie1 = context1.Request.Cookies["updateIncomePriceFinish"];
                        //if (cookie1 == null)
                        //{
                        //    cookie1 = new HttpCookie("updateIncomePriceFinish");
                        //}
                        //cookie1.Value = finishOrderCount.ToString();
                        //cookie1.Expires = DateTime.Now.AddMinutes(60);
                        //context1.Response.Cookies.Add(cookie1);
                        //context1.Session["updateIncomePriceFinish"] = finishOrderCount.ToString();
                    });
                    
                }

            }
            catch (Exception ex)
            {
                //isOk = false;
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
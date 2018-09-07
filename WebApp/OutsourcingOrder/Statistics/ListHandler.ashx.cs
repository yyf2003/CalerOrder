using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Common;


namespace WebApp.OutsourcingOrder.Statistics
{
    /// <summary>
    /// ListHandler 的摘要说明
    /// </summary>
    public class ListHandler : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            string type = string.Empty;
            if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            else if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            switch (type)
            { 
                case "getProvinceByDate":
                    result = GetProvinceBySelectDate();
                    break;
                case "getCityByDate":
                    result = GetCityBySelectDate();
                    break;
                case "getSearchByDate":
                    result = GetDataBySelectDate();
                    break;
                default:
                    result = GetDataNew();
                    break;
            }
            context.Response.Write(result);
        }

        
        string GetData()
        {
            string result = string.Empty;
            string outsourceId = string.Empty;
            string guidanceMonth = string.Empty;
            string guidanceId = string.Empty;
            string subjectId = string.Empty;
            string province = string.Empty;
            string city = string.Empty;
            string assignType = string.Empty;
            List<int> guidanceIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> assignTypeList = new List<int>();
            List<int> outsourceList = new List<int>();
            if (context1.Request.Form["outsourceId"] != null)
            {
                outsourceId = context1.Request.Form["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.Form["guidanceMonth"] != null)
            {
                guidanceMonth = context1.Request.Form["guidanceMonth"];
            }
            if (context1.Request.Form["guidanceId"] != null)
            {
                guidanceId = context1.Request.Form["guidanceId"];
                if (!string.IsNullOrWhiteSpace(guidanceId))
                {
                    guidanceIdList = StringHelper.ToIntList(guidanceId,',');
                }
            }
            if (context1.Request.Form["subjectId"] != null)
            {
                subjectId = context1.Request.Form["subjectId"];
                if (!string.IsNullOrWhiteSpace(subjectId))
                {
                    subjectIdList = StringHelper.ToIntList(subjectId, ',');
                    Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                    List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                    subjectIdList.AddRange(hMSubjectIdList);
                }
            }
            if (context1.Request.Form["province"] != null)
            {
                province = context1.Request.Form["province"];
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                }
            }
            if (context1.Request.Form["city"] != null)
            {
                city = context1.Request.Form["city"];
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                }
            }
            if (context1.Request.Form["assignType"] != null)
            {
                assignType = context1.Request.Form["assignType"];
                if (!string.IsNullOrWhiteSpace(assignType))
                {
                    assignTypeList = StringHelper.ToIntList(assignType, ',');
                }
            }
            int totalShopCount = 0;
            decimal totalArea = 0;

            decimal popPrice = 0;
            decimal receivePOPPrice = 0;

            decimal installPrice = 0;
            decimal receiveinstallPrice = 0;

            decimal expressPrice = 0;
            decimal receiveExpressPrice = 0;

            decimal measurePrice = 0;
            decimal receiveMeasurePrice = 0;
            decimal otherPrice = 0;
            decimal receiveOtherPrice = 0;
            List<int> TotalShopCountList = new List<int>();
            guidanceIdList.ForEach(gid => {
                //是否全部三叶草
                bool isBCSSubject = false;
                var orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                 
                                  where outsourceList.Contains(order.OutsourceId ?? 0)
                                  
                                  && gid==order.GuidanceId
                                  && (order.IsDelete == null || order.IsDelete == false)
                                  && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength!=null && order.GraphicLength>1 && order.GraphicWidth!=null && order.GraphicWidth>1) || order.OrderType > 1)
                                  select new
                                  {
                                      order
                                     
                                  }).ToList();

                //var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.OutsourceId == outsourceId && guidanceIdList.Contains(s.GuidanceId ?? 0));
                var orderList = orderList0;

                if (subjectIdList.Any())
                {
                    orderList = orderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                    
                    var NotBCSSubjectList = new SubjectBLL().GetList(s => subjectIdList.Contains(s.Id) && (s.CornerType==null || !s.CornerType.Contains("三叶草")));
                    isBCSSubject = !NotBCSSubjectList.Any();
                    
                }

                if (provinceList.Any())
                {
                    orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
                if (cityList.Any())
                {
                    orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
                }
                if (assignTypeList.Any())
                {
                    orderList = orderList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                }
                
                if (orderList.Any())
                {
                    List<int> shopIdList = orderList.Select(s => s.order.ShopId??0).Distinct().ToList();
                    TotalShopCountList.AddRange(shopIdList);
                    List<int> installShopIdList = shopIdList;
                    if (isBCSSubject)
                    {
                        //如果是三叶草，把出现在大货订单里面的店铺去掉
                        List<int> totalOrderShopIdList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                                          join subject in CurrentContext.DbContext.Subject
                                                          on order.SubjectId equals subject.Id
                                                          where order.GuidanceId == gid
                                                          && !subjectIdList.Contains(order.SubjectId??0)
                                                          && subject.ApproveState == 1
                                                          && (subject.IsDelete == null || subject.IsDelete == false)
                                                          && (order.IsDelete == null || order.IsDelete == false)
                                                          && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                                                          select order.ShopId ?? 0).Distinct().ToList();
                        installShopIdList = installShopIdList.Except(totalOrderShopIdList).ToList();

                    }

                    //安装费
                    var installOrderPriceList = orderList0.Where(s => installShopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                    if (installOrderPriceList.Any())
                    {
                        if (assignTypeList.Any())
                        {
                            installOrderPriceList = installOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                        }
                        installPrice += installOrderPriceList.Sum(s => (s.order.PayOrderPrice ?? 0)*(s.order.Quantity??1));
                        receiveinstallPrice += installOrderPriceList.Sum(s => (s.order.ReceiveOrderPrice ?? 0) *(s.order.Quantity ?? 1));
                    }
                    //快递费
                    var expressOrderPriceList = orderList0.Where(s => shopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)).ToList();
                    if (expressOrderPriceList.Any())
                    {
                        if (assignTypeList.Any())
                        {
                            expressOrderPriceList = expressOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                        }
                        expressPrice += expressOrderPriceList.Sum(s => (s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                        receiveExpressPrice += expressOrderPriceList.Sum(s => (s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                    }
                    var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.GuidanceId == gid && installShopIdList.Contains(s.ShopId ?? 0) && outsourceList.Contains(s.OutsourceId ?? 0)).ToList();
                    assignShopList.ForEach(s =>
                    {
                        if ((s.PayInstallPrice ?? 0) > 0)
                            installPrice += (s.PayInstallPrice ?? 0);

                        if ((s.ReceiveInstallPrice ?? 0) > 0)
                            receiveinstallPrice += (s.ReceiveInstallPrice ?? 0);

                        if ((s.PayExpressPrice ?? 0) > 0)
                            expressPrice += (s.PayExpressPrice ?? 0);

                        if ((s.ReceiveExpresslPrice ?? 0) > 0)
                            receiveExpressPrice += (s.ReceiveExpresslPrice ?? 0);
                    });
                    if (orderList.Any())
                    {
                        decimal installPrice1 = 0;
                        decimal receiveInstallPrice1 = 0;
                        orderList = orderList.Where(s=>s.order.SubjectId>0).ToList();
                        orderList.ForEach(s =>
                        {
                            if (s.order.GraphicLength != null && s.order.GraphicWidth != null)
                            {
                                if (s.order.GraphicMaterial == "挂轴")
                                {
                                    totalArea += ((s.order.GraphicWidth ?? 0) / 1000) * 2 * (s.order.Quantity ?? 1);
                                }
                                else
                                {
                                    totalArea += ((s.order.GraphicLength ?? 0) * (s.order.GraphicWidth ?? 0) / 1000000) * (s.order.Quantity ?? 1);
                                }
                            }
                            popPrice += (s.order.TotalPrice ?? 0);
                            receivePOPPrice += (s.order.ReceiveTotalPrice ?? 0);
                            
                            if (s.order.OrderType == (int)OrderTypeEnum.测量费)
                            {
                                measurePrice += ((s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                receiveMeasurePrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                            }
                            if (s.order.OrderType == (int)OrderTypeEnum.其他费用 || s.order.OrderType == (int)OrderTypeEnum.印刷费)
                            {
                                otherPrice += ((s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                receiveOtherPrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                            }
                            if (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)
                            {
                                expressPrice += ((s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                receiveExpressPrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                            }
                        });
                        //安装费单独算
                        var installOrderList = orderList.Where(s => installShopIdList.Contains(s.order.ShopId??0)).ToList();
                        installOrderList.ForEach(s => {
                            if (s.order.OrderType == (int)OrderTypeEnum.安装费)
                            {
                                installPrice1 += (s.order.PayOrderPrice ?? 0);
                                receiveInstallPrice1 += (s.order.ReceiveOrderPrice ?? 0);
                            }
                        });

                        if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                        {
                            DateTime date0 = DateTime.Parse(guidanceMonth);
                            int year = date0.Year;
                            int month = date0.Month;
                            //应收外协费用（裱板费）
                            var pvcOrderList = new OutsourceReceivePriceOrderBLL().GetList(s => outsourceList.Contains(s.OutsourceId??0) && shopIdList.Contains(s.ShopId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                            if (pvcOrderList.Any())
                            {
                                decimal materialPrice = pvcOrderList.Sum(s => s.TotalPrice ?? 0);
                                popPrice = popPrice - materialPrice;
                                popPrice = popPrice > 0 ? popPrice : 0;
                            }
                        }

                        installPrice += installPrice1;
                        receiveinstallPrice += receiveInstallPrice1;

                    }

                }
            });
            //特殊外协费用
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date0 = DateTime.Parse(guidanceMonth);
                int year = date0.Year;
                int month = date0.Month;
                var outsourcePriceOrderList = new OutsourcePriceOrderBLL().GetList(s => outsourceList.Contains(s.OutsourceId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                if (outsourcePriceOrderList.Any())
                {
                    installPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).Sum(s => s.PayPrice ?? 0);
                    measurePrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).Sum(s => s.PayPrice ?? 0);
                    expressPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费).Sum(s => s.PayPrice ?? 0);
                    otherPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用).Sum(s => s.PayPrice ?? 0);
                }
            }
            decimal totalPrice = (popPrice + installPrice + expressPrice + measurePrice + otherPrice);
            decimal receiveTotalPrice = (receivePOPPrice + receiveinstallPrice + receiveExpressPrice + receiveMeasurePrice + receiveOtherPrice);
            if (totalPrice > 0)
                totalPrice = Math.Round(totalPrice, 2);
            if (popPrice > 0)
                popPrice = Math.Round(popPrice, 2);
            if (installPrice > 0)
                installPrice = Math.Round(installPrice, 2);
            if (expressPrice > 0)
                expressPrice = Math.Round(expressPrice, 2);
            if (receiveExpressPrice > 0)
                receiveExpressPrice = Math.Round(receiveExpressPrice, 2);
            if (measurePrice > 0)
                measurePrice = Math.Round(measurePrice, 2);
            if (receiveMeasurePrice > 0)
                receiveMeasurePrice = Math.Round(receiveMeasurePrice, 2);
            if (otherPrice > 0)
                otherPrice = Math.Round(otherPrice, 2);
            if (receiveOtherPrice > 0)
                receiveOtherPrice = Math.Round(receiveOtherPrice, 2);
            if (totalArea > 0)
                totalArea = Math.Round(totalArea, 2);

            if (receiveinstallPrice > 0)
                receiveinstallPrice = Math.Round(receiveinstallPrice, 2);
            if (receivePOPPrice > 0)
                receivePOPPrice = Math.Round(receivePOPPrice, 2);
            if (receiveTotalPrice > 0)
                receiveTotalPrice = Math.Round(receiveTotalPrice, 2);
            System.Text.StringBuilder json = new System.Text.StringBuilder();
            totalShopCount = TotalShopCountList.Distinct().Count();
            json.Append("{\"ShopCount\":\"" + totalShopCount + "\",\"TotalArea\":\"" + totalArea + "\",\"POPPrice\":\"" + popPrice + "\",\"InstallPrice\":\"" + installPrice + "\",\"ExpressPrice\":\"" + expressPrice + "\",\"ReceiveExpressPrice\":\"" + receiveExpressPrice + "\",\"MeasurePrice\":\"" + measurePrice + "\",\"OtherPrice\":\"" + otherPrice + "\",\"TotalPrice\":\"" + totalPrice + "\",\"ReceivePOPPrice\":\"" + receivePOPPrice + "\",\"ReceiveInstallPrice\":\"" + receiveinstallPrice + "\",\"ReceiveTotalPrice\":\"" + receiveTotalPrice + "\",\"ReceiveMeasurePrice\":\"" + receiveMeasurePrice + "\",\"ReceiveOtherPrice\":\"" + receiveOtherPrice + "\"}");
            result = "[" + json.ToString() + "]";
            return result;
        }

        string GetDataNew()
        {
            string result = string.Empty;
            string outsourceId = string.Empty;
            string guidanceMonth = string.Empty;
            string guidanceId = string.Empty;
            string subjectId = string.Empty;
            string province = string.Empty;
            string city = string.Empty;
            string assignType = string.Empty;
            List<int> guidanceIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> assignTypeList = new List<int>();
            List<int> outsourceList = new List<int>();
            if (context1.Request.Form["outsourceId"] != null)
            {
                outsourceId = context1.Request.Form["outsourceId"];
                if (!string.IsNullOrWhiteSpace(outsourceId))
                {
                    outsourceList = StringHelper.ToIntList(outsourceId, ',');
                }
            }
            if (context1.Request.Form["guidanceMonth"] != null)
            {
                guidanceMonth = context1.Request.Form["guidanceMonth"];
            }
            if (context1.Request.Form["guidanceId"] != null)
            {
                guidanceId = context1.Request.Form["guidanceId"];
                if (!string.IsNullOrWhiteSpace(guidanceId))
                {
                    guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
                }
            }
            bool selectedSubject = false;
            if (context1.Request.Form["subjectId"] != null)
            {
                subjectId = context1.Request.Form["subjectId"];
                if (!string.IsNullOrWhiteSpace(subjectId))
                {
                    subjectIdList = StringHelper.ToIntList(subjectId, ',');
                    Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                    List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                    subjectIdList.AddRange(hMSubjectIdList);
                    selectedSubject = true;
                }
            }
            if (context1.Request.Form["province"] != null)
            {
                province = context1.Request.Form["province"];
                if (!string.IsNullOrWhiteSpace(province))
                {
                    provinceList = StringHelper.ToStringList(province, ',');
                }
            }
            if (context1.Request.Form["city"] != null)
            {
                city = context1.Request.Form["city"];
                if (!string.IsNullOrWhiteSpace(city))
                {
                    cityList = StringHelper.ToStringList(city, ',');
                }
            }
            if (context1.Request.Form["assignType"] != null)
            {
                assignType = context1.Request.Form["assignType"];
                if (!string.IsNullOrWhiteSpace(assignType))
                {
                    assignTypeList = StringHelper.ToIntList(assignType, ',');
                }
            }
            int totalShopCount = 0;
            decimal totalArea = 0;

            decimal popPrice = 0;
            decimal receivePOPPrice = 0;

            decimal installPrice = 0;
            decimal receiveinstallPrice = 0;

            decimal expressPrice = 0;
            decimal receiveExpressPrice = 0;

            decimal measurePrice = 0;
            decimal receiveMeasurePrice = 0;
            decimal otherPrice = 0;
            decimal receiveOtherPrice = 0;
            List<int> TotalShopCountList = new List<int>();
            guidanceIdList.ForEach(gid =>
            {
                
                var orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail

                                  where outsourceList.Contains(order.OutsourceId ?? 0)

                                  && gid == order.GuidanceId
                                  && (order.IsDelete == null || order.IsDelete == false)
                                  && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType > 1)
                                  select new
                                  {
                                      order

                                  }).ToList();

                //var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.OutsourceId == outsourceId && guidanceIdList.Contains(s.GuidanceId ?? 0));
                

                if (provinceList.Any())
                {
                    orderList0 = orderList0.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
                if (cityList.Any())
                {
                    orderList0 = orderList0.Where(s => cityList.Contains(s.order.City)).ToList();
                }
                if (assignTypeList.Any())
                {
                    orderList0 = orderList0.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                }

                List<int> shopIdList = new List<int>();
                if (orderList0.Any())
                {
                    var orderList = orderList0.Where(s=>(s.order.SubjectId??0)>0).ToList();
                    //活动安装费
                    var activeInstallOrder = orderList0.Where(s => (s.order.SubjectId ?? 0) == 0 && s.order.OrderType==(int)OrderTypeEnum.安装费).ToList();
                    if (subjectIdList.Any())
                    {
                        orderList = orderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                        if (selectedSubject && activeInstallOrder.Any(s => (s.order.BelongSubjectId ?? 0) > 0))
                        {
                            activeInstallOrder = activeInstallOrder.Where(s => subjectIdList.Contains(s.order.BelongSubjectId ?? 0)).ToList();
                        }
                        else
                        {
                            shopIdList = orderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                            if (shopIdList.Any())
                               activeInstallOrder = activeInstallOrder.Where(s => shopIdList.Contains(s.order.ShopId??0)).ToList();
                        }
                    }
                    if (!shopIdList.Any())
                        shopIdList = orderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                    TotalShopCountList.AddRange(shopIdList);
                    //TotalShopCountList.AddRange(activeInstallOrder.Select(s=>s.order.ShopId??0).ToList());
                    
                    

                    //安装费
                    //var installOrderPriceList = orderList0.Where(s => installShopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                    if (activeInstallOrder.Any())
                    {
                        if (assignTypeList.Any())
                        {
                            activeInstallOrder = activeInstallOrder.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                        }
                        installPrice += activeInstallOrder.Sum(s => (s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                        receiveinstallPrice += activeInstallOrder.Sum(s => (s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                    }
                    //快递费
                    var expressOrderPriceList = orderList0.Where(s => shopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)).ToList();
                    if (expressOrderPriceList.Any())
                    {
                        if (assignTypeList.Any())
                        {
                            expressOrderPriceList = expressOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                        }
                        expressPrice += expressOrderPriceList.Sum(s => (s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                        receiveExpressPrice += expressOrderPriceList.Sum(s => (s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                    }
                    var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.GuidanceId == gid && shopIdList.Contains(s.ShopId ?? 0) && outsourceList.Contains(s.OutsourceId ?? 0)).ToList();
                    assignShopList.ForEach(s =>
                    {
                        if ((s.PayInstallPrice ?? 0) > 0)
                            installPrice += (s.PayInstallPrice ?? 0);

                        if ((s.ReceiveInstallPrice ?? 0) > 0)
                            receiveinstallPrice += (s.ReceiveInstallPrice ?? 0);

                        if ((s.PayExpressPrice ?? 0) > 0)
                            expressPrice += (s.PayExpressPrice ?? 0);

                        if ((s.ReceiveExpresslPrice ?? 0) > 0)
                            receiveExpressPrice += (s.ReceiveExpresslPrice ?? 0);
                    });
                    if (orderList.Any())
                    {
                        //decimal installPrice1 = 0;
                        //decimal receiveInstallPrice1 = 0;
                        //orderList = orderList.Where(s => s.order.SubjectId > 0).ToList();
                        orderList.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s.order.GraphicMaterial))
                            {
                                if (s.order.GraphicLength != null && s.order.GraphicWidth != null)
                                {
                                    if (s.order.GraphicMaterial == "挂轴")
                                    {
                                        totalArea += ((s.order.GraphicWidth ?? 0) / 1000) * 2 * (s.order.Quantity ?? 1);
                                    }
                                    else
                                    {
                                        totalArea += ((s.order.GraphicLength ?? 0) * (s.order.GraphicWidth ?? 0) / 1000000) * (s.order.Quantity ?? 1);
                                    }
                                }
                                popPrice += (s.order.TotalPrice ?? 0);
                                receivePOPPrice += (s.order.ReceiveTotalPrice ?? 0);
                            }
                            if (s.order.OrderType == (int)OrderTypeEnum.测量费)
                            {
                                measurePrice += ((s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                receiveMeasurePrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                            }
                            if (s.order.OrderType == (int)OrderTypeEnum.安装费)
                            {
                                installPrice += ((s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                receiveinstallPrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                            }
                            if (s.order.OrderType == (int)OrderTypeEnum.其他费用 || s.order.OrderType == (int)OrderTypeEnum.印刷费)
                            {
                                otherPrice += ((s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                receiveOtherPrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                            }
                            if (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)
                            {
                                expressPrice += ((s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                receiveExpressPrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                            }
                        });
                       

                        if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                        {
                            DateTime date0 = DateTime.Parse(guidanceMonth);
                            int year = date0.Year;
                            int month = date0.Month;
                            //应收外协费用（裱板费）
                            var pvcOrderList = new OutsourceReceivePriceOrderBLL().GetList(s => outsourceList.Contains(s.OutsourceId ?? 0) && shopIdList.Contains(s.ShopId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                            if (pvcOrderList.Any())
                            {
                                decimal materialPrice = pvcOrderList.Sum(s => s.TotalPrice ?? 0);
                                popPrice = popPrice - materialPrice;
                                popPrice = popPrice > 0 ? popPrice : 0;
                            }
                        }

                        //installPrice += installPrice1;
                        //receiveinstallPrice += receiveInstallPrice1;

                    }

                }
            });
            //特殊外协费用
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date0 = DateTime.Parse(guidanceMonth);
                int year = date0.Year;
                int month = date0.Month;
                var outsourcePriceOrderList = new OutsourcePriceOrderBLL().GetList(s => outsourceList.Contains(s.OutsourceId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                if (outsourcePriceOrderList.Any())
                {
                    installPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).Sum(s => s.PayPrice ?? 0);
                    measurePrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).Sum(s => s.PayPrice ?? 0);
                    expressPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费).Sum(s => s.PayPrice ?? 0);
                    otherPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用).Sum(s => s.PayPrice ?? 0);
                }
            }
            decimal totalPrice = (popPrice + installPrice + expressPrice + measurePrice + otherPrice);
            decimal receiveTotalPrice = (receivePOPPrice + receiveinstallPrice + receiveExpressPrice + receiveMeasurePrice + receiveOtherPrice);
            if (totalPrice > 0)
                totalPrice = Math.Round(totalPrice, 2);
            if (popPrice > 0)
                popPrice = Math.Round(popPrice, 2);
            if (installPrice > 0)
                installPrice = Math.Round(installPrice, 2);
            if (expressPrice > 0)
                expressPrice = Math.Round(expressPrice, 2);
            if (receiveExpressPrice > 0)
                receiveExpressPrice = Math.Round(receiveExpressPrice, 2);
            if (measurePrice > 0)
                measurePrice = Math.Round(measurePrice, 2);
            if (receiveMeasurePrice > 0)
                receiveMeasurePrice = Math.Round(receiveMeasurePrice, 2);
            if (otherPrice > 0)
                otherPrice = Math.Round(otherPrice, 2);
            if (receiveOtherPrice > 0)
                receiveOtherPrice = Math.Round(receiveOtherPrice, 2);
            if (totalArea > 0)
                totalArea = Math.Round(totalArea, 2);

            if (receiveinstallPrice > 0)
                receiveinstallPrice = Math.Round(receiveinstallPrice, 2);
            if (receivePOPPrice > 0)
                receivePOPPrice = Math.Round(receivePOPPrice, 2);
            if (receiveTotalPrice > 0)
                receiveTotalPrice = Math.Round(receiveTotalPrice, 2);
            System.Text.StringBuilder json = new System.Text.StringBuilder();
            totalShopCount = TotalShopCountList.Distinct().Count();
            json.Append("{\"ShopCount\":\"" + totalShopCount + "\",\"TotalArea\":\"" + totalArea + "\",\"POPPrice\":\"" + popPrice + "\",\"InstallPrice\":\"" + installPrice + "\",\"ExpressPrice\":\"" + expressPrice + "\",\"ReceiveExpressPrice\":\"" + receiveExpressPrice + "\",\"MeasurePrice\":\"" + measurePrice + "\",\"OtherPrice\":\"" + otherPrice + "\",\"TotalPrice\":\"" + totalPrice + "\",\"ReceivePOPPrice\":\"" + receivePOPPrice + "\",\"ReceiveInstallPrice\":\"" + receiveinstallPrice + "\",\"ReceiveTotalPrice\":\"" + receiveTotalPrice + "\",\"ReceiveMeasurePrice\":\"" + receiveMeasurePrice + "\",\"ReceiveOtherPrice\":\"" + receiveOtherPrice + "\"}");
            result = "[" + json.ToString() + "]";
            return result;
        }

        string GetProvinceBySelectDate()
        {
            string result = string.Empty;
            string beginDateStr = string.Empty;
            string endDateStr = string.Empty;
            int customerId = 0;
            int outsourceId = 0;
            if (context1.Request.Form["beginDate"] != null)
            {
                beginDateStr = context1.Request.Form["beginDate"];
            }
            if (context1.Request.Form["endDate"] != null)
            {
                endDateStr = context1.Request.Form["endDate"];
            }
            if (context1.Request.Form["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.Form["customerId"]);
            }
            if (context1.Request.Form["outsourceId"] != null)
            {
                outsourceId = int.Parse(context1.Request.Form["outsourceId"]);
            }
            if (!string.IsNullOrWhiteSpace(beginDateStr) && StringHelper.IsDateTime(beginDateStr))
            {
                DateTime beginDate = DateTime.Parse(beginDateStr);
                var orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                  join subject in CurrentContext.DbContext.Subject
                                  on order.SubjectId equals subject.Id
                                  where (subject.IsDelete == null || subject.IsDelete == false)
                                  && order.OutsourceId == outsourceId
                                  && subject.CustomerId == customerId
                                  && subject.AddDate >= beginDate
                                   && (order.IsDelete == null || order.IsDelete == false)
                                  select new
                                  {
                                      order,
                                      subject
                                  }).ToList();
                if (!string.IsNullOrWhiteSpace(endDateStr) && StringHelper.IsDateTime(endDateStr))
                {
                    DateTime endDate = DateTime.Parse(endDateStr);
                    orderList0 = orderList0.Where(s => s.subject.AddDate < endDate.AddDays(1)).ToList();
                    if (orderList0.Any())
                    {
                        List<string> provinceList = orderList0.Select(s => s.order.Province).Distinct().ToList();
                        if (provinceList.Any())
                        {
                            System.Text.StringBuilder json = new System.Text.StringBuilder();
                            bool isEmpty = false;
                            provinceList.ForEach(s => {
                                if (!string.IsNullOrWhiteSpace(s))
                                {
                                    json.Append("{\"Province\":\"" + s + "\"},");
                                }
                                else
                                {
                                    isEmpty = true;
                                }
                            });
                            if (isEmpty)
                            {
                                json.Append("{\"Province\":\"空\"},");
                            }
                            result = "[" + json.ToString().TrimEnd(',') + "]";
                        }
                    }
                }
            }
            return result;
        }

        string GetCityBySelectDate()
        {
            string result = string.Empty;
            string beginDateStr = string.Empty;
            string endDateStr = string.Empty;
            string provinces = string.Empty;
            List<string> provinceList = new List<string>();
            int customerId = 0;
            int outsourceId = 0;
            if (context1.Request.Form["beginDate"] != null)
            {
                beginDateStr = context1.Request.Form["beginDate"];
            }
            if (context1.Request.Form["endDate"] != null)
            {
                endDateStr = context1.Request.Form["endDate"];
            }
            if (context1.Request.Form["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.Form["customerId"]);
            }
            if (context1.Request.Form["outsourceId"] != null)
            {
                outsourceId = int.Parse(context1.Request.Form["outsourceId"]);
            }
            if (context1.Request.Form["province"] != null)
            {
                provinces = context1.Request.Form["province"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces,',');
                }
            }
            if (provinceList.Any())
            {
                
                var orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                  join subject in CurrentContext.DbContext.Subject
                                  on order.SubjectId equals subject.Id
                                  where (subject.IsDelete == null || subject.IsDelete == false)
                                  && order.OutsourceId == outsourceId
                                  && subject.CustomerId == customerId
                                  && (order.IsDelete == null || order.IsDelete == false)
                                  select new
                                  {
                                      order,
                                      subject
                                  }).ToList();
                if (!string.IsNullOrWhiteSpace(beginDateStr) && StringHelper.IsDateTime(beginDateStr))
                {
                    DateTime beginDate = DateTime.Parse(beginDateStr);
                    orderList0 = orderList0.Where(s => s.subject.AddDate >= beginDate).ToList();
                }
                if (!string.IsNullOrWhiteSpace(endDateStr) && StringHelper.IsDateTime(endDateStr))
                {
                    DateTime endDate = DateTime.Parse(endDateStr);
                    orderList0 = orderList0.Where(s => s.subject.AddDate < endDate.AddDays(1)).ToList();
                }
                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orderList0 = orderList0.Where(s => provinceList.Contains(s.order.Province) || (s.order.Province==null || s.order.Province=="")).ToList();
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s =>(s.order.Province == null || s.order.Province == "")).ToList();
                    }
                }
                else
                {
                    orderList0 = orderList0.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
                
                if (orderList0.Any())
                {
                    List<string> cityList = orderList0.Select(s => s.order.City).Distinct().ToList();
                    if (cityList.Any())
                    {
                        System.Text.StringBuilder json = new System.Text.StringBuilder();
                        bool isEmpty = false;
                        cityList.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                json.Append("{\"City\":\"" + s + "\"},");
                            }
                            else
                            {
                                isEmpty = true;
                            }
                        });
                        if (isEmpty)
                        {
                            json.Append("{\"City\":\"空\"},");
                        }
                        result = "[" + json.ToString().TrimEnd(',') + "]";
                    }
                }
            }
            return result;
        }

        string GetDataBySelectDate()
        {

            string result = string.Empty;
            string beginDateStr = string.Empty;
            string endDateStr = string.Empty;
            string provinces = string.Empty;
            List<string> provinceList = new List<string>();
            string citys = string.Empty;
            List<string> cityList = new List<string>();
            string assignType = string.Empty;
            List<int> assignTypeList = new List<int>();
            int customerId = 0;
            int outsourceId = 0;
            if (context1.Request.Form["beginDate"] != null)
            {
                beginDateStr = context1.Request.Form["beginDate"];
            }
            if (context1.Request.Form["endDate"] != null)
            {
                endDateStr = context1.Request.Form["endDate"];
            }
            if (context1.Request.Form["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.Form["customerId"]);
            }
            if (context1.Request.Form["outsourceId"] != null)
            {
                outsourceId = int.Parse(context1.Request.Form["outsourceId"]);
            }
            if (context1.Request.Form["province"] != null)
            {
                provinces = context1.Request.Form["province"];
                if (!string.IsNullOrWhiteSpace(provinces))
                {
                    provinceList = StringHelper.ToStringList(provinces, ',');
                }
            }
            if (context1.Request.Form["city"] != null)
            {
                citys = context1.Request.Form["city"];
                if (!string.IsNullOrWhiteSpace(citys))
                {
                    cityList = StringHelper.ToStringList(citys, ',');
                }
            }
            if (context1.Request.Form["assignType"] != null)
            {
                assignType = context1.Request.Form["assignType"];
                if (!string.IsNullOrWhiteSpace(assignType))
                {
                    assignTypeList = StringHelper.ToIntList(assignType, ',');
                }
            }
            var orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                              join subject in CurrentContext.DbContext.Subject
                              on order.SubjectId equals subject.Id
                              where (subject.IsDelete == null || subject.IsDelete == false)
                              && order.OutsourceId == outsourceId
                              && subject.CustomerId == customerId
                              && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType > 1)
                              && (order.IsDelete == null || order.IsDelete == false)
                              select new
                              {
                                  order,
                                  subject
                              }).ToList();
            if (!string.IsNullOrWhiteSpace(beginDateStr) && StringHelper.IsDateTime(beginDateStr))
            {
                DateTime beginDate = DateTime.Parse(beginDateStr);
                orderList0 = orderList0.Where(s => s.subject.AddDate >= beginDate).ToList();
            }
            if (!string.IsNullOrWhiteSpace(endDateStr) && StringHelper.IsDateTime(endDateStr))
            {
                DateTime endDate = DateTime.Parse(endDateStr);
                orderList0 = orderList0.Where(s => s.subject.AddDate < endDate.AddDays(1)).ToList();
            }
            if (provinceList.Any())
            {
                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        orderList0 = orderList0.Where(s => provinceList.Contains(s.order.Province) || (s.order.Province == null || s.order.Province == "")).ToList();
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s => (s.order.Province == null || s.order.Province == "")).ToList();
                    }
                }
                else
                {
                    orderList0 = orderList0.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        orderList0 = orderList0.Where(s => cityList.Contains(s.order.City) || (s.order.City == null || s.order.City == "")).ToList();
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s => (s.order.City == null || s.order.City == "")).ToList();
                    }
                }
                else
                {
                    orderList0 = orderList0.Where(s => cityList.Contains(s.order.City)).ToList();
                }
            }
            if (assignTypeList.Any())
            {
                orderList0 = orderList0.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
            }
            if (orderList0.Any())
            {
                List<int> guidanceIdList = orderList0.Select(s => s.order.GuidanceId ?? 0).Distinct().ToList();
                int totalShopCount = 0;
                decimal totalArea = 0;

                decimal popPrice = 0;
                decimal receivePOPPrice = 0;

                decimal installPrice = 0;
                decimal receiveinstallPrice = 0;

                decimal expressPrice = 0;
                decimal receiveExpressPrice = 0;

                decimal measurePrice = 0;
                decimal receiveMeasurePrice = 0;
                decimal otherPrice = 0;
                decimal receiveOtherPrice = 0;
                List<int> TotalShopCountList = new List<int>();
                guidanceIdList.ForEach(gid =>
                {
                    var orderList = orderList0.Where(s=>s.order.GuidanceId==gid).ToList();
                   
                    if (orderList.Any())
                    {
                        List<int> shopIdList = orderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                        TotalShopCountList.AddRange(shopIdList);


                        //安装费
                        var installOrderPriceList = orderList.Where(s => s.order.SubjectId == 0 && s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                        if (installOrderPriceList.Any())
                        {
                            if (assignTypeList.Any())
                            {
                                installOrderPriceList = installOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                            }
                            installPrice += installOrderPriceList.Sum(s => s.order.PayOrderPrice ?? 0);
                            receiveinstallPrice += installOrderPriceList.Sum(s => s.order.ReceiveOrderPrice ?? 0);
                        }
                        //快递费
                        var expressOrderPriceList = orderList.Where(s => s.order.SubjectId == 0 && (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)).ToList();
                        if (expressOrderPriceList.Any())
                        {
                            if (assignTypeList.Any())
                            {
                                expressOrderPriceList = expressOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                            }
                            expressPrice += expressOrderPriceList.Sum(s => s.order.PayOrderPrice ?? 0);
                            receiveExpressPrice += expressOrderPriceList.Sum(s => s.order.ReceiveOrderPrice ?? 0);
                        }
                        var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.GuidanceId == gid && shopIdList.Contains(s.ShopId ?? 0) && s.OutsourceId==gid).ToList();
                        assignShopList.ForEach(s =>
                        {
                            if ((s.PayInstallPrice ?? 0) > 0)
                                installPrice += (s.PayInstallPrice ?? 0);

                            if ((s.ReceiveInstallPrice ?? 0) > 0)
                                receiveinstallPrice += (s.ReceiveInstallPrice ?? 0);

                            if ((s.PayExpressPrice ?? 0) > 0)
                                expressPrice += (s.PayExpressPrice ?? 0);

                            if ((s.ReceiveExpresslPrice ?? 0) > 0)
                                receiveExpressPrice += (s.ReceiveExpresslPrice ?? 0);
                        });
                        if (orderList.Any())
                        {
                            decimal installPrice1 = 0;
                            decimal receiveInstallPrice1 = 0;
                            orderList = orderList.Where(s => s.order.SubjectId > 0).ToList();
                            orderList.ForEach(s =>
                            {
                                if (s.order.GraphicLength != null && s.order.GraphicWidth != null)
                                {
                                    if (s.order.GraphicMaterial == "挂轴")
                                    {
                                        totalArea += ((s.order.GraphicWidth ?? 0) / 1000) * 2 * (s.order.Quantity ?? 1);
                                    }
                                    else
                                    {
                                        totalArea += ((s.order.GraphicLength ?? 0) * (s.order.GraphicWidth ?? 0) / 1000000) * (s.order.Quantity ?? 1);
                                    }
                                }
                                popPrice += (s.order.TotalPrice ?? 0);
                                receivePOPPrice += (s.order.ReceiveTotalPrice ?? 0);
                                if (s.order.OrderType == (int)OrderTypeEnum.安装费)
                                {
                                    installPrice1 += (s.order.PayOrderPrice ?? 0);
                                    receiveInstallPrice1 += (s.order.ReceiveOrderPrice ?? 0);
                                }
                                if (s.order.OrderType == (int)OrderTypeEnum.测量费)
                                {
                                    measurePrice += (s.order.PayOrderPrice ?? 0);
                                    receiveMeasurePrice += (s.order.ReceiveOrderPrice ?? 0);
                                }
                                if (s.order.OrderType == (int)OrderTypeEnum.其他费用 || s.order.OrderType == (int)OrderTypeEnum.印刷费)
                                {
                                    otherPrice += ((s.order.PayOrderPrice ?? 0)*(s.order.Quantity??1));
                                    receiveOtherPrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                }
                                if (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)
                                {
                                    expressPrice += (s.order.PayOrderPrice ?? 0);
                                    receiveExpressPrice += (s.order.ReceiveOrderPrice ?? 0);
                                }
                            });
                            //if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                            //{
                            //    DateTime date0 = DateTime.Parse(guidanceMonth);
                            //    int year = date0.Year;
                            //    int month = date0.Month;
                            //    //应收外协费用（裱板费）
                            //    var pvcOrderList = new OutsourceReceivePriceOrderBLL().GetList(s => outsourceList.Contains(s.OutsourceId ?? 0) && shopIdList.Contains(s.ShopId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                            //    if (pvcOrderList.Any())
                            //    {
                            //        decimal materialPrice = pvcOrderList.Sum(s => s.TotalPrice ?? 0);
                            //        popPrice = popPrice - materialPrice;
                            //        popPrice = popPrice > 0 ? popPrice : 0;
                            //    }


                            //}

                            installPrice += installPrice1;
                            receiveinstallPrice += receiveInstallPrice1;

                        }

                    }
                    //特殊外协费用
                    Models.SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(gid);
                    if (guidanceModel != null)
                    {
                        
                        var outsourcePriceOrderList = new OutsourcePriceOrderBLL().GetList(s => s.OutsourceId == gid && s.GuidanceYear == guidanceModel.GuidanceYear && s.GuidanceMonth == guidanceModel.GuidanceMonth).ToList();
                        if (outsourcePriceOrderList.Any())
                        {
                            installPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).Sum(s => s.PayPrice ?? 0);
                            measurePrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).Sum(s => s.PayPrice ?? 0);
                            expressPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费).Sum(s => s.PayPrice ?? 0);
                            otherPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用).Sum(s => s.PayPrice ?? 0);
                        }
                    }
                });
               
                decimal totalPrice = (popPrice + installPrice + expressPrice + measurePrice + otherPrice);
                decimal receiveTotalPrice = (receivePOPPrice + receiveinstallPrice + receiveExpressPrice + receiveMeasurePrice + receiveOtherPrice);
                if (totalPrice > 0)
                    totalPrice = Math.Round(totalPrice, 2);
                if (popPrice > 0)
                    popPrice = Math.Round(popPrice, 2);
                if (installPrice > 0)
                    installPrice = Math.Round(installPrice, 2);
                if (expressPrice > 0)
                    expressPrice = Math.Round(expressPrice, 2);
                if (receiveExpressPrice > 0)
                    receiveExpressPrice = Math.Round(receiveExpressPrice, 2);
                if (measurePrice > 0)
                    measurePrice = Math.Round(measurePrice, 2);
                if (receiveMeasurePrice > 0)
                    receiveMeasurePrice = Math.Round(receiveMeasurePrice, 2);
                if (otherPrice > 0)
                    otherPrice = Math.Round(otherPrice, 2);
                if (receiveOtherPrice > 0)
                    receiveOtherPrice = Math.Round(receiveOtherPrice, 2);
                if (totalArea > 0)
                    totalArea = Math.Round(totalArea, 2);

                if (receiveinstallPrice > 0)
                    receiveinstallPrice = Math.Round(receiveinstallPrice, 2);
                if (receivePOPPrice > 0)
                    receivePOPPrice = Math.Round(receivePOPPrice, 2);
                if (receiveTotalPrice > 0)
                    receiveTotalPrice = Math.Round(receiveTotalPrice, 2);
                System.Text.StringBuilder json = new System.Text.StringBuilder();
                totalShopCount = TotalShopCountList.Distinct().Count();
                json.Append("{\"ShopCount\":\"" + totalShopCount + "\",\"TotalArea\":\"" + totalArea + "\",\"POPPrice\":\"" + popPrice + "\",\"InstallPrice\":\"" + installPrice + "\",\"ExpressPrice\":\"" + expressPrice + "\",\"ReceiveExpressPrice\":\"" + receiveExpressPrice + "\",\"MeasurePrice\":\"" + measurePrice + "\",\"OtherPrice\":\"" + otherPrice + "\",\"TotalPrice\":\"" + totalPrice + "\",\"ReceivePOPPrice\":\"" + receivePOPPrice + "\",\"ReceiveInstallPrice\":\"" + receiveinstallPrice + "\",\"ReceiveTotalPrice\":\"" + receiveTotalPrice + "\",\"ReceiveMeasurePrice\":\"" + receiveMeasurePrice + "\",\"ReceiveOtherPrice\":\"" + receiveOtherPrice + "\"}");
                result = "[" + json.ToString() + "]";
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using System.Text;
using DAL;
using Common;
using Newtonsoft.Json;

namespace WebApp.InstallPriceManage.handler
{
    /// <summary>
    /// List 的摘要说明
    /// </summary>
    public class List : IHttpHandler
    {

        string type = string.Empty;
        int customerId;
        string guidanceId = string.Empty;
        string region = string.Empty;
        string shopNo = string.Empty;
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context1 = context;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            if (context.Request.QueryString["guidanceId"] != null)
            {
                guidanceId = context.Request.QueryString["guidanceId"];
            }
            if (context.Request.QueryString["region"] != null)
            {
                region = context.Request.QueryString["region"];
            }
            if (context.Request.QueryString["shopNo"] != null)
            {
                shopNo = context.Request.QueryString["shopNo"];
            }
            switch (type)
            {
                case "getRegion":
                    result = GetRegion();
                    break;
                case "getInstallShopList":
                    result = GetInstallShopList();
                    break;
                case "getExpressShopList":
                    result = GetExpressShopList();
                    break;
                case "deleteInstallPrice":
                    result = DeleteInstallPrice();
                    break;
                case "deleteExpressPrice":
                    result = DeleteExpressPrice();
                    break;
                case "recoverInstallPrice":
                    result = RecoverInstallPrice();
                    break;
                case "recoverExpressPrice":
                    result = RecoverExpressPrice();
                    break;
                case "getInstallSubject":
                    result = GetInstallSubjectList();
                    break;
                case "editInstallPrice":
                    result = EditInstallPrice();
                    break;
                case "editExpressPrice":
                    result = EditExpressPrice();
                    break;
                case "getProvince":
                    result = GetProvince();
                    break;
                case "getCity":
                    result = GetCity();
                    break;
                default:
                    break;

            }
            context.Response.Write(result);
        }

        string GetRegion()
        {
            var list = new RegionBLL().GetList(s => s.CustomerId == customerId);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"RegionName\":\"" + s.RegionName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetProvince()
        {
            string result = string.Empty;
            List<int> guidanceIdList = new List<int>();
            List<string> regionList = new List<string>();
            if (context1.Request.QueryString["guidanceId"] != null)
            {
                string guidance = context1.Request.QueryString["guidanceId"];
                guidanceIdList = StringHelper.ToIntList(guidance, ',');
            }
            if (context1.Request.QueryString["region"] != null)
            {
                string region = context1.Request.QueryString["region"];
                regionList = StringHelper.ToStringList(region,',', LowerUpperEnum.ToLower);
            }
            if (guidanceIdList.Any() && regionList.Any())
            {
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where (order.IsDelete == null || order.IsDelete == false)
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            && subject.ApproveState == 1
                            && guidanceIdList.Contains(order.GuidanceId ?? 0)
                            && order.Region != null && regionList.Contains(order.Region.ToLower())
                            select order.Province).Distinct().OrderBy(s => s).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s => {
                        json.Append("{\"ProvinceName\":\""+s+"\"},");
                    });
                    result = "["+json.ToString().TrimEnd(',')+"]";
                }
            }

            return result;
        }

        string GetCity()
        {
            string result = string.Empty;
            List<int> guidanceIdList = new List<int>();
            //List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            if (context1.Request.QueryString["guidanceId"] != null)
            {
                string guidance = context1.Request.QueryString["guidanceId"];
                guidanceIdList = StringHelper.ToIntList(guidance, ',');
            }
            //if (context1.Request.QueryString["region"] != null)
            //{
            //    string region = context1.Request.QueryString["region"];
            //    regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            //}
            if (context1.Request.QueryString["province"] != null)
            {
                string province = context1.Request.QueryString["province"];
                provinceList = StringHelper.ToStringList(province, ',');
            }
            if (guidanceIdList.Any() && provinceList.Any())
            {
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where (order.IsDelete == null || order.IsDelete == false)
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            && subject.ApproveState == 1
                            && guidanceIdList.Contains(order.GuidanceId ?? 0)
                            && provinceList.Contains(order.Province)
                            select order.City).Distinct().OrderBy(s => s).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        json.Append("{\"CityName\":\"" + s + "\"},");
                    });
                    result = "[" + json.ToString().TrimEnd(',') + "]";
                }
            }

            return result;
        }

        string GetInstallShopList()
        {
            int pageIndex = 0;
            int pageSize = 10;
            string shopNo = string.Empty;
            if (context1.Request.QueryString["currpage"] != null)
            {
                pageIndex = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }
            List<int> guidanceIdList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> shopNoList = new List<string>();
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                shopNo = shopNo.Replace("，", ",");
                shopNoList = StringHelper.ToStringList(shopNo, ',', LowerUpperEnum.ToLower);
            }
            var shopList = (from install in CurrentContext.DbContext.InstallPriceShopInfo
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on install.GuidanceId equals guidance.ItemId
                            join shop in CurrentContext.DbContext.Shop
                            on install.ShopId equals shop.Id
                            where guidanceIdList.Contains(install.GuidanceId ?? 0)
                            select new
                            {
                                shop,
                                install,
                                guidance
                            }).ToList();
            if (regionList.Any())
            {
                shopList = shopList.Where(s => s.shop.RegionName != null && regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            }
            if (shopNoList.Any())
            {
                shopList = shopList.Where(s => s.shop.ShopNo != null && shopNoList.Contains(s.shop.ShopNo.ToLower())).ToList();
            }
            if (shopList.Any())
            {
                int totalCount = shopList.Count;
                StringBuilder json = new StringBuilder();
                int index = 1;
                OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();
                shopList.OrderBy(s => s.install.ShopId).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList().ForEach(s =>
                {
                    decimal total = (s.install.BasicPrice ?? 0) + (s.install.OOHPrice ?? 0) + (s.install.WindowPrice ?? 0);
                    decimal payPrice = 0;
                    OutsourceOrderDetail osModel = outsourceOrderBll.GetList(o => o.GuidanceId == s.guidance.ItemId && o.ShopId == s.shop.Id && o.OrderType == (int)OrderTypeEnum.安装费 && (o.SubjectId ?? 0) == 0 && o.InstallPriceSubjectType == s.install.InstallPriceSubjectType).FirstOrDefault();
                    if (osModel != null)
                    {
                        payPrice = osModel.PayOrderPrice ?? 0;
                    }
                    int isDelete = (s.install.IsDelete ?? false) ? 1 : 0;
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.install.Id + "\",\"GuidanceId\":\"" + s.install.GuidanceId + "\",\"GuidanceName\":\"" + s.guidance.ItemName + "\",\"SubjectId\":\"" + (s.install.SubjectId ?? 0) + "\",\"ShopId\":\"" + s.shop.Id + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"Region\":\"" + s.shop.RegionName + "\",\"Province\":\"" + s.shop.ProvinceName + "\",\"MaterialSupport\":\"" + s.install.MaterialSupport + "\",\"BasicInstallPrice\":\"" + (s.install.BasicPrice ?? 0) + "\",\"WindowInstallPrice\":\"" + (s.install.WindowPrice ?? 0) + "\",\"OOHInstallPrice\":\"" + (s.install.OOHPrice ?? 0) + "\",\"Remark\":\"" + s.install.Remark + "\",\"TotalPrice\":\"" + total + "\",\"PayPrice\":\"" + payPrice + "\",\"IsDelete\":\"" + isDelete + "\"},");
                    index++;
                });
                //return "["+json.ToString().TrimEnd(',')+"]";
                if (json.Length > 0)
                    return "{\"total\":" + totalCount + ",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
                else
                    return "{\"total\":0,\"rows\":[] }";
            }
            return "{\"total\":0,\"rows\":[] }";
        }

        string GetExpressShopList()
        {
            int pageIndex = 0;
            int pageSize = 10;
            string shopNo = string.Empty;
            if (context1.Request.QueryString["currpage"] != null)
            {
                pageIndex = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNo = context1.Request.QueryString["shopNo"];
            }
            List<int> guidanceIdList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> shopNoList = new List<string>();
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                shopNo = shopNo.Replace("，", ",");
                shopNoList = StringHelper.ToStringList(shopNo, ',', LowerUpperEnum.ToLower);
            }

            var shopList = (from express in CurrentContext.DbContext.ExpressPriceDetail
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on express.GuidanceId equals guidance.ItemId
                            join shop in CurrentContext.DbContext.Shop
                            on express.ShopId equals shop.Id
                            where guidanceIdList.Contains(express.GuidanceId ?? 0)
                            select new
                            {
                                shop,
                                express,
                                guidance
                            }).ToList();
            if (regionList.Any())
            {
                shopList = shopList.Where(s => s.shop.RegionName != null && regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            }
            if (shopNoList.Any())
            {
                shopList = shopList.Where(s => s.shop.ShopNo != null && shopNoList.Contains(s.shop.ShopNo.ToLower())).ToList();
            }
            if (shopList.Any())
            {
                int totalCount = shopList.Count;
                StringBuilder json = new StringBuilder();
                int index = 1;
                OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();
                shopList.OrderBy(s => s.express.ShopId).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList().ForEach(s =>
                {
                    decimal payPrice = 0;
                    OutsourceOrderDetail osModel = outsourceOrderBll.GetList(o => o.GuidanceId == s.guidance.ItemId && o.ShopId == s.shop.Id && o.OrderType == (int)OrderTypeEnum.发货费 && (o.SubjectId ?? 0) == 0).FirstOrDefault();
                    if (osModel != null)
                    {
                        payPrice = osModel.PayExpressPrice ?? 0;
                    }
                    int isDelete = (s.express.IsDelete ?? false) ? 1 : 0;
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.express.Id + "\",\"GuidanceId\":\"" + s.express.GuidanceId + "\",\"GuidanceName\":\"" + s.guidance.ItemName + "\",\"ShopId\":\"" + s.shop.Id + "\",\"ShopNo\":\"" + s.shop.ShopNo + "\",\"ShopName\":\"" + s.shop.ShopName + "\",\"Region\":\"" + s.shop.RegionName + "\",\"Province\":\"" + s.shop.ProvinceName + "\",\"ReceivePrice\":\"" + (s.express.ExpressPrice ?? 0) + "\",\"PayPrice\":\"" + payPrice + "\",\"IsDelete\":\"" + isDelete + "\"},");
                    index++;
                });
                //return "["+json.ToString().TrimEnd(',')+"]";
                if (json.Length > 0)
                    return "{\"total\":" + totalCount + ",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
                else
                    return "{\"total\":0,\"rows\":[] }";
            }
            return "{\"total\":0,\"rows\":[] }";

        }

        string DeleteInstallPrice()
        {
            string r = "ok";
            string ids = string.Empty;
            //int isDeletePayPrice = 0;
            if (context1.Request.QueryString["ids"] != null)
            {
                ids = context1.Request.QueryString["ids"];
            }
            //if (context1.Request.QueryString["isDeletePayPrice"] != null)
            //{
            //    isDeletePayPrice = int.Parse(context1.Request.QueryString["isDeletePayPrice"]);
            //}
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids, ',');
                InstallPriceShopInfoBLL iBll = new InstallPriceShopInfoBLL();
                OutsourceOrderDetailBLL oBll = new OutsourceOrderDetailBLL();
                List<InstallPriceShopInfo> list = iBll.GetList(s => idList.Contains(s.Id));
                if (list.Any())
                {
                    list.ForEach(s =>
                    {
                        s.IsDelete = true;
                        iBll.Update(s);

                        var oModel = oBll.GetList(o => o.GuidanceId == s.GuidanceId && o.ShopId == s.ShopId && o.SubjectId == 0 && o.BelongSubjectId == s.SubjectId && o.OrderType == (int)OrderTypeEnum.安装费).FirstOrDefault();
                        if (oModel != null)
                        {
                            oModel.IsDelete = true;
                            oBll.Update(oModel);
                        }
                    });

                }
                else
                {
                    r = "删除失败：没有找到符合条件的数据";
                }
            }
            else
                r = "删除失败";
            return r;
        }

        string DeleteExpressPrice()
        {
            string r = "ok";
            string ids = string.Empty;
            //int isDeletePayPrice = 0;
            if (context1.Request.QueryString["ids"] != null)
            {
                ids = context1.Request.QueryString["ids"];
            }
            //if (context1.Request.QueryString["isDeletePayPrice"] != null)
            //{
            //    isDeletePayPrice = int.Parse(context1.Request.QueryString["isDeletePayPrice"]);
            //}
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids, ',');
                ExpressPriceDetailBLL eBll = new ExpressPriceDetailBLL();
                OutsourceOrderDetailBLL oBll = new OutsourceOrderDetailBLL();
                List<ExpressPriceDetail> list = eBll.GetList(s => idList.Contains(s.Id));
                if (list.Any())
                {
                    list.ForEach(s =>
                    {
                        s.IsDelete = true;
                        eBll.Update(s);

                        var oModel = oBll.GetList(o => o.GuidanceId == s.GuidanceId && o.ShopId == s.ShopId && o.SubjectId == 0 && o.OrderType == (int)OrderTypeEnum.发货费).FirstOrDefault();
                        if (oModel != null)
                        {
                            oModel.IsDelete = true;
                            oBll.Update(oModel);
                        }
                    });

                }
                else
                {
                    r = "删除失败：没有找到符合条件的数据";
                }
            }
            else
                r = "删除失败";
            return r;
        }

        string RecoverInstallPrice()
        {
            string r = "ok";
            string ids = string.Empty;
            //int isDeletePayPrice = 0;
            if (context1.Request.QueryString["ids"] != null)
            {
                ids = context1.Request.QueryString["ids"];
            }
            //if (context1.Request.QueryString["isDeletePayPrice"] != null)
            //{
            //    isDeletePayPrice = int.Parse(context1.Request.QueryString["isDeletePayPrice"]);
            //}
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids, ',');
                InstallPriceShopInfoBLL iBll = new InstallPriceShopInfoBLL();
                OutsourceOrderDetailBLL oBll = new OutsourceOrderDetailBLL();
                List<InstallPriceShopInfo> list = iBll.GetList(s => idList.Contains(s.Id));
                if (list.Any())
                {
                    list.ForEach(s =>
                    {
                        s.IsDelete = false;
                        iBll.Update(s);
                        var oModel = oBll.GetList(o => o.GuidanceId == s.GuidanceId && o.ShopId == s.ShopId && o.SubjectId == 0 && o.BelongSubjectId == s.SubjectId && o.OrderType == (int)OrderTypeEnum.安装费).FirstOrDefault();
                        if (oModel != null)
                        {
                            oModel.IsDelete = false;
                            oBll.Update(oModel);
                        }
                    });

                }
                else
                {
                    r = "恢复失败：没有找到符合条件的数据";
                }
            }
            else
                r = "恢复失败";
            return r;
        }

        string RecoverExpressPrice()
        {
            string r = "ok";
            string ids = string.Empty;
            //int isDeletePayPrice = 0;
            if (context1.Request.QueryString["ids"] != null)
            {
                ids = context1.Request.QueryString["ids"];
            }
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids, ',');
                ExpressPriceDetailBLL eBll = new ExpressPriceDetailBLL();
                OutsourceOrderDetailBLL oBll = new OutsourceOrderDetailBLL();
                List<ExpressPriceDetail> list = eBll.GetList(s => idList.Contains(s.Id));
                if (list.Any())
                {
                    list.ForEach(s =>
                    {
                        s.IsDelete = false;
                        eBll.Update(s);
                        var oModel = oBll.GetList(o => o.GuidanceId == s.GuidanceId && o.ShopId == s.ShopId && o.SubjectId == 0 && o.OrderType == (int)OrderTypeEnum.发货费).FirstOrDefault();
                        if (oModel != null)
                        {
                            oModel.IsDelete = false;
                            oBll.Update(oModel);
                        }
                    });

                }
                else
                {
                    r = "恢复失败：没有找到符合条件的数据";
                }
            }
            else
                r = "恢复失败";
            return r;
        }

        string GetInstallSubjectList()
        {
            int guidanceId = 0;
            if (context1.Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(context1.Request.QueryString["guidanceId"]);
            }
            List<Subject> subjectList = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && (s.SubjectType != (int)SubjectTypeEnum.二次安装 && s.SubjectType != (int)SubjectTypeEnum.补单) && ((s.HandMakeSubjectId ?? 0) == 0));
            if (subjectList.Any())
            {
                StringBuilder json = new StringBuilder();
                subjectList.OrderBy(s => s.SubjectName).ToList().ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string EditInstallPrice()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.QueryString["jsonStr"] != null)
            {
                jsonStr = context1.Request.QueryString["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                jsonStr = HttpUtility.UrlDecode(jsonStr);
                InstallPriceShopInfo newModel = JsonConvert.DeserializeObject<InstallPriceShopInfo>(jsonStr);
                if (newModel != null)
                {
                    InstallPriceShopInfoBLL iBll = new InstallPriceShopInfoBLL();
                    if (newModel.Id > 0)
                    {
                        InstallPriceShopInfo model = iBll.GetModel(newModel.Id);
                        if (model != null)
                        {
                            model.BasicPrice = newModel.BasicPrice;
                            model.MaterialSupport = newModel.MaterialSupport;
                            model.OOHPrice = newModel.OOHPrice;
                            model.Remark = newModel.Remark;
                            model.SubjectId = newModel.SubjectId;
                            model.WindowPrice = newModel.WindowPrice;
                            iBll.Update(model);

                            //外协安装费
                            OutsourceOrderDetailBLL oBll = new OutsourceOrderDetailBLL();
                            var oModel = oBll.GetList(s => s.GuidanceId == model.GuidanceId && s.BelongSubjectId == model.SubjectId && s.ShopId == model.ShopId && s.SubjectId == 0 && s.OrderType == (int)OrderTypeEnum.安装费).FirstOrDefault();
                            if (oModel != null && (oModel.PayOrderPrice != newModel.PayPrice || oModel.BelongSubjectId != newModel.SubjectId))
                            {
                                oModel.PayOrderPrice = newModel.PayPrice;
                                oModel.BelongSubjectId = newModel.SubjectId;
                                oModel.ReceiveOrderPrice = (newModel.BasicPrice ?? 0) + (newModel.WindowPrice ?? 0) + (newModel.OOHPrice ?? 0);
                                oBll.Update(oModel);
                            }
                        }
                    }
                    else
                    {
                        result = "操作失败！";
                    }

                }
            }
            return result;
        }

        string EditExpressPrice()
        { 
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.QueryString["jsonStr"] != null)
            {
                jsonStr = context1.Request.QueryString["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                jsonStr = HttpUtility.UrlDecode(jsonStr);
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                ExpressPriceDetailBLL eBll = new ExpressPriceDetailBLL();
                ExpressPriceDetail newModel = JsonConvert.DeserializeObject<ExpressPriceDetail>(jsonStr);
                if (newModel != null)
                {
                    if (newModel.Id > 0)
                    {
                        ExpressPriceDetail model = eBll.GetModel(newModel.Id);
                        model.ExpressPrice = newModel.ExpressPrice;
                        eBll.Update(model);

                        OutsourceOrderDetailBLL oBll = new OutsourceOrderDetailBLL();
                        var oModel = oBll.GetList(s => s.GuidanceId == model.GuidanceId && s.ShopId == model.ShopId && s.SubjectId == 0 && s.OrderType == (int)OrderTypeEnum.发货费).FirstOrDefault();
                        if (oModel != null && oModel.PayExpressPrice != newModel.PayPrice)
                        {
                            oModel.PayOrderPrice = newModel.PayPrice;
                            oModel.PayExpressPrice = newModel.PayPrice;
                            oModel.ReceiveOrderPrice = newModel.ExpressPrice;
                            oModel.ReceiveExpresslPrice = newModel.ExpressPrice;
                            oBll.Update(oModel);
                        }
                    }
                    else
                    {
                        result = "操作失败！";
                    }
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
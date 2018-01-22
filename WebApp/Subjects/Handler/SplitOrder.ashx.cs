using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.SessionState;
using BLL;
using Common;
using DAL;
using Models;
using Newtonsoft.Json;

namespace WebApp.Subjects.Handler
{
    /// <summary>
    /// SplitOrder 的摘要说明
    /// </summary>
    public class SplitOrder : IHttpHandler, IRequiresSessionState
    {
        string type = string.Empty;
        int customerId;
        int subjectId;
        int planId;
        string subjectIds = string.Empty;
        OrderPlanBLL orderPlanBll = new OrderPlanBLL();
        SplitOrderPlanDetailBLL splitPlanBll = new SplitOrderPlanDetailBLL();
        ShopMachineFrameBLL frameBll = new ShopMachineFrameBLL();
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            if (context.Request.QueryString["SubjectId"] != null)
            {
                subjectId = int.Parse(context.Request.QueryString["SubjectId"]);
            }
            if (context.Request.QueryString["planId"] != null)
            {
                planId = int.Parse(context.Request.QueryString["planId"]);
            }
            if (context.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context.Request.QueryString["subjectIds"];
            }
            if (context.Request.Form["subjectIds"] != null)
            {
                subjectIds = context.Request.Form["subjectIds"];
            }
            string result = string.Empty;
            switch (type)
            {
                case "getCondition":
                    //获取店铺类型
                    string positionName2 = context.Request.QueryString["positionName"];
                    string shopNos0 = string.Empty;
                    if (context.Request.QueryString["shopNo"] != null)
                    {
                        shopNos0 = context.Request.QueryString["shopNo"];
                    }
                    result = GetConditions(positionName2, shopNos0);
                    break;
                case "getPosition":
                    //获取位置
                    result = GetPositions();
                    break;
                case "getCornerType":
                    //获取器架角落类型
                    string positionName0 = context.Request.Form["positionName"];
                    string shopNos = string.Empty;
                    if (context.Request.Form["shopNo"] != null)
                    {
                        shopNos = context.Request.Form["shopNo"];
                    }
                    string gender0 = string.Empty;
                    if (context.Request.Form["gender"] != null)
                    {
                        gender0 = context.Request.Form["gender"];
                    }
                    result = GetCornerType(positionName0, shopNos, gender0);
                    break;
                case "getMachineFrame":
                    //获取器架类型
                    string positionName = context.Request.Form["positionName"];
                    string cornerType = context.Request.Form["cornerType"];
                    string shopNos1 = string.Empty;
                    if (context.Request.Form["shopNo"] != null)
                    {
                        shopNos1 = context.Request.Form["shopNo"];
                    }
                    string gender1 = string.Empty;
                    if (context.Request.Form["gender"] != null)
                    {
                        gender1 = context.Request.Form["gender"];
                    }
                    result = GetMachineFrame(positionName, cornerType, shopNos1, gender1);
                    break;
                case "add":
                    string optype = context.Request.QueryString["optype"];
                    //string jsonstr = context.Request.QueryString["jsonStr"];
                    string jsonstr = context.Request.Form["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(jsonstr))
                    {
                        jsonstr = jsonstr.Replace("+", "%2B");

                        jsonstr = HttpUtility.UrlDecode(jsonstr);

                        jsonstr = jsonstr.Replace("乘", "×");
                    }
                    result = AddPlan(optype, jsonstr);
                    break;
                case "getList":

                    string Ids = null;

                    if (context.Request.QueryString["Ids"] != null)
                    {
                        Ids = context.Request.QueryString["Ids"];
                    }
                    result = GetPlanList(Ids);
                    break;
                case "getDetail":

                    result = GetDetailList();
                    break;
                case "deleteDetail":
                    int detailId = int.Parse(context.Request.QueryString["detailId"]);
                    result = DeleteDetail(detailId);
                    break;
                case "deletePlan":
                    result = DeletePlan();
                    break;
                case "splitOrder":
                    //开始拆单
                    string planIds = context.Request.QueryString["planIds"];
                    result = GoSplitOrder(planIds);
                    break;
                
                case "getCustomerMaterial":
                    string region1 = context.Request.QueryString["region"];
                    //int bigTypeId = int.Parse(context.Request.QueryString["bigTypeId"]);
                    result = GetCustomerMaterial(region1);
                    break;
                case "checkMaterial":
                    string materialName = context.Request.QueryString["materialName"];
                    if (!string.IsNullOrWhiteSpace(materialName))
                    {
                        materialName = HttpUtility.UrlDecode(materialName);
                    }
                    string region2 = context.Request.QueryString["region"];
                    result = CheckCustomerMaterial(materialName, region2);
                    break;
                case "getPOPBaseInfo":
                    string jsonstr1 = context.Request.Form["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(jsonstr1))
                    {
                        jsonstr1 = HttpUtility.UrlDecode(jsonstr1);
                    }
                    result = GetPOPBaseInfo(jsonstr1);
                    break;
                case "changeCondition":
                    string str = context.Request.Form["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        str = HttpUtility.UrlDecode(str);
                    }
                    result = ChangeConditions(str);
                    break;
                case "checkSplit":
                    result = CheckPlanData(subjectId);
                    break;
                case "getShops"://设置单店拆单的店铺
                    string str1 = context.Request.Form["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(str1))
                    {
                        str1 = str1.Replace("+", "%2B");

                        str1 = HttpUtility.UrlDecode(str1);


                    }
                    result = GetShops(str1);
                    break;
                case "addPerShop":
                    string str2 = context.Request.Form["jsonStr"];

                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        str2 = str2.Replace("+", "%2B");

                        str2 = HttpUtility.UrlDecode(str2);


                    }
                    result = AddPerShopInfo(str2);
                    break;
                case "addHCPOP":
                    string str3 = context.Request.Form["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(str3))
                    {
                        str3 = str3.Replace("+", "%2B");

                        str3 = HttpUtility.UrlDecode(str3);
                    }
                    result = AddHCPOP(str3);
                    break;

            }
            context.Response.Write(result);
        }

        string GetConditions(string sheetName = null,string shopNos=null)
        {
            List<int> subjdectIdList = StringHelper.ToIntList(subjectIds, ',');
            StringBuilder json = new StringBuilder();
            List<string> cityTier = new List<string>();
            StringBuilder cityTiersb = new StringBuilder();
            List<string> format = new List<string>();
            StringBuilder formatsb = new StringBuilder();
            List<string> install = new List<string>();
            StringBuilder installsb = new StringBuilder();
            List<string> materialSupport = new List<string>();
            StringBuilder mssb = new StringBuilder();
            List<string> POSScale = new List<string>();
            StringBuilder scalesb = new StringBuilder();
            List<string> sheet = new List<string>();
            StringBuilder sheetsb = new StringBuilder();
            List<string> gender = new List<string>();
            StringBuilder gendersb = new StringBuilder();
            List<int> count = new List<int>();
            StringBuilder countsb = new StringBuilder();
            List<string> material = new List<string>();
            StringBuilder materialsb = new StringBuilder();
            List<decimal> GraphicWidth = new List<decimal>();
            StringBuilder GraphicWidthsb = new StringBuilder();
            List<decimal> GraphicLength = new List<decimal>();
            StringBuilder GraphicLengthsb = new StringBuilder();
            List<decimal> WindowWide = new List<decimal>();
            StringBuilder WindowWidesb = new StringBuilder();
            List<decimal> WindowHigh = new List<decimal>();
            StringBuilder WindowHighsb = new StringBuilder();
            List<decimal> WindowDeep = new List<decimal>();
            StringBuilder WindowDeepsb = new StringBuilder();

            List<string> ChooseImg = new List<string>();
            StringBuilder ChooseImgsb = new StringBuilder();


            List<string> POPSize = new List<string>();
            StringBuilder POPSizesb = new StringBuilder();

            List<string> WindowSize = new List<string>();
            StringBuilder WindowSizesb = new StringBuilder();

            List<string> IsElectricity = new List<string>();
            StringBuilder IsElectricitysb = new StringBuilder();

            var shoplist = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                            join shop in CurrentContext.DbContext.Shop
                            on merge.ShopId equals shop.Id
                            join pop1 in CurrentContext.DbContext.POP
                            on new { merge.ShopId, merge.GraphicNo } equals new { pop1.ShopId, pop1.GraphicNo } into popTemp
                            from pop in popTemp.DefaultIfEmpty()
                            where subjdectIdList.Contains(merge.SubjectId ?? 0) && (sheetName != null && sheetName != "0" ? merge.Sheet == sheetName : 1 == 1)
                            && (merge.IsDelete == null || merge.IsDelete == false)
                            select new
                            {
                                merge,
                                shop,
                                pop

                            }).OrderBy(s => s.shop.CityTier).ToList();
            List<int> shopIdList = new List<int>();
            List<int> newShopIds = new List<int>();
            bool hasShopNo = false;
            if (!string.IsNullOrWhiteSpace(shopNos))
            {
                shopNos = shopNos.Replace("，", ",").TrimEnd(',');
                string notExist = string.Empty;
                newShopIds = GetShopId(shopNos, out notExist);
                hasShopNo = true;
            }
            if (shoplist.Any())
            {
                if (hasShopNo)
                {
                    shoplist = shoplist.Where(s => newShopIds.Contains(s.shop.Id)).ToList();
                }
                shoplist.ToList().ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s.shop.CityTier) && !cityTier.Contains(s.shop.CityTier))
                    {
                        cityTier.Add(s.shop.CityTier);
                        cityTiersb.Append(s.shop.CityTier);
                        cityTiersb.Append(',');
                    }
                    if (!string.IsNullOrWhiteSpace(s.shop.Format) && !format.Contains(s.shop.Format))
                    {
                        format.Add(s.shop.Format);
                        formatsb.Append(s.shop.Format);
                        formatsb.Append(',');
                    }
                    string support = !string.IsNullOrWhiteSpace(s.merge.MaterialSupport) ? s.merge.MaterialSupport : "空";
                    if (!materialSupport.Contains(support))
                    {
                        materialSupport.Add(support);
                        mssb.Append(support);
                        mssb.Append(',');
                    }
                    string isinstall = !string.IsNullOrWhiteSpace(s.shop.IsInstall) ? s.shop.IsInstall : "空";
                    if (!install.Contains(isinstall))
                    {
                        install.Add(isinstall);
                        installsb.Append(isinstall);
                        installsb.Append(',');
                    }
                    string scale = !string.IsNullOrWhiteSpace(s.merge.POSScale) ? s.merge.POSScale : "空";
                    if (!POSScale.Contains(scale))
                    {
                        POSScale.Add(scale);
                        scalesb.Append(scale);
                        scalesb.Append(',');
                    }
                    if (!string.IsNullOrWhiteSpace(s.merge.Sheet) && !sheet.Contains(s.merge.Sheet))
                    {
                        sheet.Add(s.merge.Sheet);
                        sheetsb.Append(s.merge.Sheet);
                        sheetsb.Append(',');
                    }

                    if (!string.IsNullOrWhiteSpace(s.merge.Gender) && !gender.Contains(s.merge.Gender))
                    {
                        gender.Add(s.merge.Gender);
                        gendersb.Append(s.merge.Gender);
                        gendersb.Append(',');
                    }
                    //
                    if (s.merge.Quantity != null && !count.Contains(s.merge.Quantity ?? 0))
                    {
                        count.Add(s.merge.Quantity ?? 0);
                        //countsb.Append(s.merge.Quantity.ToString());
                        //countsb.Append(',');
                    }
                    if (s.pop != null && !string.IsNullOrWhiteSpace(s.pop.GraphicMaterial) && !material.Contains(s.pop.GraphicMaterial))
                    {
                        material.Add(s.pop.GraphicMaterial);
                        materialsb.Append(s.pop.GraphicMaterial);
                        materialsb.Append(',');
                    }

                    string popsize = string.Empty;
                    if (s.pop != null)
                    {
                        if ((s.pop.GraphicWidth == null || s.pop.GraphicWidth == 0) || (s.pop.GraphicLength == null || s.pop.GraphicLength == 0))
                        {
                            popsize = "空";
                        }
                        else
                        {
                            popsize = s.pop.GraphicWidth + "*" + s.pop.GraphicLength;
                        }
                        if (!POPSize.Contains(popsize))
                        {
                            POPSize.Add(popsize);
                            POPSizesb.Append(popsize);
                            POPSizesb.Append(',');
                        }
                    }
                    else
                    {
                        string popsize0 = "空";
                        if (!POPSize.Contains(popsize0))
                        {
                            POPSize.Add(popsize0);
                            POPSizesb.Append(popsize0);
                            POPSizesb.Append(',');
                        }
                    }
                    string windowsize = string.Empty;
                    if (s.pop != null)
                    {
                        if ((s.pop.WindowWide == null || s.pop.WindowWide == 0) || (s.pop.WindowHigh == null || s.pop.WindowHigh == 0) || (s.pop.WindowDeep == null || s.pop.WindowDeep == 0))
                        {
                            windowsize = "空";
                        }
                        else
                        {
                            windowsize = s.pop.WindowWide + "*" + s.pop.WindowHigh + "*" + s.pop.WindowDeep;
                        }
                        if (!WindowSize.Contains(windowsize))
                        {
                            WindowSize.Add(windowsize);
                            WindowSizesb.Append(windowsize);
                            WindowSizesb.Append(',');
                        }
                    }
                    else
                    {
                        string windowsize0 = "空";
                        if (!WindowSize.Contains(windowsize0))
                        {
                            WindowSize.Add(windowsize0);
                            WindowSizesb.Append(windowsize0);
                            WindowSizesb.Append(',');
                        }
                    }





                    if (!string.IsNullOrWhiteSpace(s.merge.ChooseImg) && !ChooseImg.Contains(s.merge.ChooseImg))
                    {
                        ChooseImg.Add(s.merge.ChooseImg);
                        ChooseImgsb.Append(s.merge.ChooseImg);
                        ChooseImgsb.Append(',');
                    }

                    //通电否
                    if (s.pop != null)
                    {
                        string electricity = string.IsNullOrWhiteSpace(s.pop.IsElectricity) ? "空" : s.pop.IsElectricity;
                        if (!IsElectricity.Contains(electricity))
                        {
                            IsElectricity.Add(electricity);
                            IsElectricitysb.Append(electricity);
                            IsElectricitysb.Append(',');
                        }
                    }

                });


            }

            //GraphicWidth.Sort();
            //GraphicLength.Sort();
            //WindowWide.Sort();
            //WindowHigh.Sort();
            //WindowDeep.Sort();
            count.Sort();
            count.ForEach(s =>
            {
                countsb.Append(s);
                countsb.Append(',');
            });
            //sheetsb.Append("其他");
            //sheetsb.Append(',');
            //POPSizesb.Append("空");
            //POPSizesb.Append(',');
            //WindowSizesb.Append("空");
            //WindowSizesb.Append(',');

            json.Append("{\"CityTier\":\"" + cityTiersb.ToString().TrimEnd(',') + "\",\"Format\":\"" + formatsb.ToString().TrimEnd(',') + "\",\"MaterialSupport\":\"" + mssb.ToString().TrimEnd(',') + "\",\"IsInstall\":\"" + installsb.ToString().TrimEnd(',') + "\",\"POSScale\":\"" + scalesb.ToString().TrimEnd(',') + "\",\"Sheet\":\"" + sheetsb.ToString().TrimEnd(',') + "\",\"Gender\":\"" + gendersb.ToString().TrimEnd(',') + "\",");
            //json.Append("\"GraphicMaterial\":\"" + materialsb.ToString().TrimEnd(',') + "\",\"Quantity\":\"" + countsb.ToString().TrimEnd(',') + "\",\"GraphicWidth\":\"" + GraphicWidthsb.ToString().TrimEnd(',') + "\",\"GraphicLength\":\"" + GraphicLengthsb.ToString().TrimEnd(',') + "\",\"WindowWide\":\"" + WindowWidesb.ToString().TrimEnd(',') + "\",\"WindowHigh\":\"" + WindowHighsb.ToString().TrimEnd(',') + "\",\"WindowDeep\":\"" + WindowDeepsb.ToString().TrimEnd(',') + "\",\"ChooseImg\":\"" + ChooseImgsb.ToString().TrimEnd(',')+ "\"}");

            json.Append("\"GraphicMaterial\":\"" + materialsb.ToString().TrimEnd(',') + "\",\"Quantity\":\"" + countsb.ToString().TrimEnd(',') + "\",\"POPSize\":\"" + POPSizesb.ToString().TrimEnd(',') + "\",\"ChooseImg\":\"" + ChooseImgsb.ToString().TrimEnd(',') + "\",\"IsElectricity\":\"" + IsElectricitysb.ToString().TrimEnd(',') + "\",\"WindowSize\":\"" + WindowSizesb.ToString().TrimEnd(',') + "\"}");

            //json.Append("\"CornerType\":\"" + CornerTypesb.ToString().TrimEnd(',')+ "\"}");
            return "[" + json.ToString() + "]";
        }

        /// <summary>
        /// 获取位置信息
        /// </summary>
        /// <returns></returns>
        string GetPositions()
        {
            var list = new PositionBLL().GetList(s => 1 == 1);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"PositionName\":\"" + s.PositionName + "\"},");
                });
                //json.Append("{\"Id\":\"0\",\"PositionName\":\"其他\"},");
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        /// <summary>
        /// 获取订单里面的角落类型信息
        /// </summary>
        /// <returns></returns>
        string GetCornerType(string positionName,string shopNos=null,string gender=null)
        {

            List<int> newShopIds = new List<int>();
            bool hasShopNo = false;
            if (!string.IsNullOrWhiteSpace(shopNos))
            {
                shopNos=shopNos.Replace("，", ",").TrimEnd(',');
                string notExist=string.Empty;
                newShopIds = GetShopId(shopNos,out notExist);
                hasShopNo = true;
            }
            List<string> genderList = new List<string>();
            if (!string.IsNullOrWhiteSpace(gender))
            {
                gender = gender.TrimEnd(',');
                genderList = StringHelper.ToStringList(gender,',');
                
            }
            List<int> subjdectIdList = StringHelper.ToIntList(subjectIds, ',');
            string rusult = string.Empty;

            //订单里，所选择位置的店铺
            //List<int> shopList = new MergeOriginalOrderBLL().GetList(s => subjdectIdList.Contains(s.SubjectId ?? 0) && s.Sheet == positionName && (s.IsDelete == null || s.IsDelete == false)).Select(s => s.ShopId ?? 0).ToList();
            var shopList = new MergeOriginalOrderBLL().GetList(s => subjdectIdList.Contains(s.SubjectId ?? 0) && s.Sheet == positionName && (s.IsDelete == null || s.IsDelete == false)).ToList();
            
            if (hasShopNo)
            {
                shopList = shopList.Where(s => newShopIds.Contains(s.Id)).ToList();
             
            }
            if (genderList.Any())
            {

                shopList = shopList.Where(s => genderList.Contains(s.Gender)).ToList();
            }
            if (shopList.Any())
            {
                var shopIds = shopList.Select(s => s.ShopId??0).Distinct().ToList();
                StringBuilder json = new StringBuilder();
                List<string> cornerTypes = new List<string>();
                var list = new ShopMachineFrameBLL().GetList(s => shopIds.Contains(s.ShopId ?? 0) && s.PositionName == positionName);
                if (genderList.Any())
                {
                    bool allGender = false;
                    genderList.ForEach(s =>
                    {
                        if (s.Contains("男") && s.Contains("女"))
                            allGender = true;
                    });
                    if (allGender)
                    {
                        genderList = new List<string>() { "男", "女" };
                    }
                    list = list.Where(s => genderList.Contains(s.Gender) || (s.Gender.Contains("男") && s.Gender.Contains("女"))).ToList();
                }
                list.ToList().ForEach(s =>
                {
                    string cornerType = !string.IsNullOrWhiteSpace(s.CornerType) ? s.CornerType : "空";
                    if (!cornerTypes.Contains(cornerType))
                    {
                        cornerTypes.Add(cornerType);
                        json.Append("{\"CornerType\":\"" + cornerType + "\"},");
                    }

                });
                if (json.Length > 0)
                {
                   
                    rusult = "[" + json.ToString().TrimEnd(',') + "]";
                }

            }


            return rusult;
        }


        string GetMachineFrame(string positionName, string cornerType, string shopNos = null, string gender = null)
        {
            List<int> newShopIds = new List<int>();
            bool hasShopNo = false;
            if (!string.IsNullOrWhiteSpace(shopNos))
            {
                shopNos = shopNos.Replace("，", ",").TrimEnd(',');
                string notExist = string.Empty;
                newShopIds = GetShopId(shopNos, out notExist);
                hasShopNo = true;
            }
            List<string> genderList = new List<string>();
            if (!string.IsNullOrWhiteSpace(gender))
            {
                gender = gender.TrimEnd(',');
                genderList = StringHelper.ToStringList(gender, ',');
            }

            StringBuilder json = new StringBuilder();
            List<int> subjdectIdList = StringHelper.ToIntList(subjectIds, ',');
            List<MergeOriginalOrder> orderList = new List<MergeOriginalOrder>();
            if (context1.Session["frameOrderlist"] != null)
            {
                orderList = context1.Session["frameOrderlist"] as List<MergeOriginalOrder>;
            }
            else
            {
                orderList = (from merge in CurrentContext.DbContext.MergeOriginalOrder

                             where subjdectIdList.Contains(merge.SubjectId ?? 0)
                             && (merge.IsDelete == null || merge.IsDelete == false)
                             select merge).ToList();
                context1.Session["frameOrderlist"] = orderList;
            }
            //var shoplist = (from merge in CurrentContext.DbContext.MergeOriginalOrder

            //                where subjdectIdList.Contains(merge.SubjectId ?? 0) && merge.Sheet == positionName
            //                && (merge.IsDelete == null || merge.IsDelete == false)
            //                select merge).ToList();
            var shoplist = orderList.Where(s => s.Sheet == positionName).ToList();
            if (hasShopNo)
            {
                //shoplist = newShopIds;
                shoplist = shoplist.Where(s => newShopIds.Contains(s.ShopId??0)).ToList();
                
            }
            if (genderList.Any())
            {
                shoplist = shoplist.Where(s => genderList.Contains(s.Gender)).ToList();
            }
            var shopIds = shoplist.Select(s => s.ShopId??0).Distinct().ToList();
            List<ShopMachineFrame> shopFrameList = new List<ShopMachineFrame>();
            if (context1.Session["framelist"] != null)
            {
                shopFrameList = context1.Session["framelist"] as List<ShopMachineFrame>;
            }
            else
            {
                shopFrameList = frameBll.GetList(s => s.Id > 0);
                context1.Session["framelist"] = shopFrameList;
            }
            var frames = shopFrameList.Where(f => shopIds.Contains(f.ShopId ?? 0) && f.PositionName == positionName).ToList();
            bool cornerTypeIsNull = false;
            if (!string.IsNullOrWhiteSpace(cornerType))
            {
                if (cornerType.Contains("空"))
                {
                    List<string> cornerTypes0 = StringHelper.ToStringList(cornerType.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                    if (cornerTypes0.Any())
                    {
                        frames = frames.Where(f => cornerTypes0.Contains(f.CornerType) || (f.CornerType == null || f.CornerType == "")).OrderBy(s => s.MachineFrame).ToList();
                    }
                    else
                    {
                        cornerTypeIsNull = true;
                        frames = frames.Where(f => f.CornerType == null || f.CornerType == "").OrderBy(s => s.MachineFrame).ToList();
                    }
                }
                else
                {
                    List<string> cornerTypes = StringHelper.ToStringList(cornerType, ',');
                    frames = frames.Where(f => cornerTypes.Contains(f.CornerType)).OrderBy(s => s.MachineFrame).ToList();
                }

            }
            else
            {
                frames = frames.Where(f => f.CornerType == null || f.CornerType == "").OrderBy(s => s.MachineFrame).ToList();
            }
            if (genderList.Any())
            {
                bool allGender = false;
                genderList.ForEach(s =>
                {
                    if (s.Contains("男") && s.Contains("女"))
                        allGender = true;
                });
                if (allGender)
                {
                    genderList = new List<string>() { "男", "女" };
                }
                frames = frames.Where(s => genderList.Contains(s.Gender) || (s.Gender.Contains("男") && s.Gender.Contains("女"))).ToList();
            }
            List<string> frameList = new List<string>();
            var frameShop = frames.Select(s => s.ShopId ?? 0).Distinct().ToList();
            shopIds.AddRange(frameShop);
            List<int> result1 = shopIds.Union(frameShop).ToList();

            if (frames.Any())
            {


                frames.OrderBy(s=>s.MachineFrame).ToList().ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s.MachineFrame) && !frameList.Contains(s.MachineFrame))
                    {
                        frameList.Add(s.MachineFrame);
                        json.Append("{\"Id\":\"" + s.Id + "\",\"FrameName\":\"" + s.MachineFrame + "\"},");
                    }

                });

            }
            if ((string.IsNullOrWhiteSpace(cornerType) || cornerTypeIsNull) && result1.Count > frameShop.Count)
            {
                frameList.Add("空");
                json.Append("{\"Id\":\"0\",\"FrameName\":\"空\"}");
            }
            if (json.Length > 0)
                return "[" + json.ToString().TrimEnd(',') + "]";
            else
                return "";

        }


        CityInOrderPlanBLL planCityBll = new CityInOrderPlanBLL();
        CityInOrderPlan planCityModel;
        ProvinceInOrderPlanBLL planProvinceBll = new ProvinceInOrderPlanBLL();
        ProvinceInOrderPlan planProvinceModel;
        string AddPlan(string optype, string jsonString)
        {
            string result = "提交失败！";
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(jsonString);

                        if (planModel != null)
                        {
                            string provinces = string.Empty;
                            string cities = string.Empty;
                            SplitOrderDetailPerShopBLL perShopBll = new SplitOrderDetailPerShopBLL();
                            if (optype == "add")
                            {
                                planModel.AddDate = DateTime.Now;
                                planModel.AddUserId = new BasePage().CurrentUser.UserId;
                                planModel.PlanType = 1;
                                
                                if (!string.IsNullOrWhiteSpace(planModel.ProvinceId))
                                {
                                    provinces = planModel.ProvinceId;
                                    planModel.ProvinceId = null;
                                }

                                if (!string.IsNullOrWhiteSpace(planModel.CityId))
                                {
                                    cities = planModel.CityId;
                                    planModel.CityId = null;
                                }
                                if (!string.IsNullOrWhiteSpace(planModel.ShopNos))
                                {
                                    planModel.ShopNos = planModel.ShopNos.Replace("，", ",").TrimEnd(',');
                                }
                                //planModel.Quantity = planModel.Quantity != null ? planModel.Quantity == 0 ? null : planModel.Quantity : planModel.Quantity;
                                orderPlanBll.Add(planModel);
                                //保存省份
                                StringHelper.ToStringList(provinces, ',').ForEach(p =>
                                {
                                    planProvinceModel = new ProvinceInOrderPlan() { ProvinceName = p, PlanId = planModel.Id, PlanType = 1 };
                                    planProvinceBll.Add(planProvinceModel);
                                });
                                //保存城市
                                StringHelper.ToStringList(cities, ',').ForEach(c =>
                                {
                                    planCityModel = new CityInOrderPlan() { CityName = c, PlanId = planModel.Id, PlanType = 1 };
                                    planCityBll.Add(planCityModel);
                                });
                                if (planModel.SplitOrderPlanDetail.Any())
                                {
                                    planModel.SplitOrderPlanDetail.ForEach(s =>
                                    {
                                        SplitOrderPlanDetail splitModel = s;
                                        if (splitModel != null)
                                        {
                                            decimal price = splitModel.RackSalePrice ?? 0;

                                            splitModel.RackSalePrice = price;
                                            splitModel.AddDate = DateTime.Now;
                                            splitModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            splitModel.PlanId = planModel.Id;
                                            splitPlanBll.Add(splitModel);
                                            if (splitModel.IsPerShop != true)
                                            {
                                                perShopBll.Delete(p => p.Sheet == planModel.PositionName && p.SubjectId == planModel.SubjectId && p.Remark == s.Remark);
                                            }
                                        }
                                    });
                                }
                                result = "ok";
                            }
                            else
                            {
                                if (planModel.Id > 0)
                                {
                                    OrderPlan planModel1 = orderPlanBll.GetModel(planModel.Id);
                                    if (planModel1 != null)
                                    {
                                        planModel.AddDate = planModel1.AddDate;
                                        planModel.AddUserId = planModel1.AddUserId;
                                        planModel.PlanType = planModel1.PlanType;
                                        
                                        if (!string.IsNullOrWhiteSpace(planModel.ProvinceId))
                                        {
                                            provinces = planModel.ProvinceId;
                                            planModel.ProvinceId = null;
                                        }

                                        if (!string.IsNullOrWhiteSpace(planModel.CityId))
                                        {
                                            cities = planModel.CityId;
                                            planModel.CityId = null;
                                        }
                                        if (!string.IsNullOrWhiteSpace(planModel.ShopNos))
                                        {
                                            planModel.ShopNos = planModel.ShopNos.Replace("，", ",").TrimEnd(',');
                                        }
                                        orderPlanBll.Update(planModel);
                                        splitPlanBll.Delete(s => s.PlanId == planModel.Id);
                                        //先删除原来的省份和城市
                                        planProvinceBll.Delete(p => p.PlanId == planModel.Id && p.PlanType == 1);
                                        planCityBll.Delete(c => c.PlanId == planModel.Id && c.PlanType == 1);
                                        //保存省份
                                        StringHelper.ToStringList(provinces, ',').ForEach(p =>
                                        {
                                            planProvinceModel = new ProvinceInOrderPlan() { ProvinceName = p, PlanId = planModel.Id, PlanType = 1 };
                                            planProvinceBll.Add(planProvinceModel);
                                        });
                                        //保存城市
                                        StringHelper.ToStringList(cities, ',').ForEach(c =>
                                        {
                                            planCityModel = new CityInOrderPlan() { CityName = c, PlanId = planModel.Id, PlanType = 1 };
                                            planCityBll.Add(planCityModel);
                                        });
                                        if (planModel.SplitOrderPlanDetail.Any())
                                        {
                                            planModel.SplitOrderPlanDetail.ForEach(s =>
                                            {
                                                SplitOrderPlanDetail splitModel = s;
                                                if (splitModel != null)
                                                {
                                                    splitModel.RackSalePrice = splitModel.RackSalePrice ?? 0;
                                                    splitModel.AddDate = DateTime.Now;
                                                    splitModel.AddUserId = new BasePage().CurrentUser.UserId;
                                                    splitModel.PlanId = planModel.Id;
                                                    splitPlanBll.Add(splitModel);
                                                    if (splitModel.IsPerShop != true)
                                                    {
                                                        perShopBll.Delete(p => p.Sheet == planModel.PositionName && p.SubjectId == planModel.SubjectId && p.Remark == s.Remark);
                                                    }
                                                }
                                            });
                                        }
                                    }
                                    result = "ok";
                                }
                                else
                                {
                                    result = "更新失败！";
                                }
                            }

                        }
                        tran.Complete();

                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
            }
            return result;
        }

        string GetPlanList(string ids = null)
        {
            try
            {


                List<int> idList = new List<int>();
                if (ids != null)
                {
                    idList = StringHelper.ToIntList(ids, ',');
                }
                var list = orderPlanBll.GetList(s => idList.Any() ? idList.Contains(s.Id) : (s.CustomerId == customerId && s.SubjectId == subjectId && s.PlanType == 1));
                string sort = string.Empty;
                string order = string.Empty;
                if (context1.Request.QueryString["sort"] != null)
                {
                    sort = context1.Request.QueryString["sort"];
                }
                if (context1.Request.QueryString["order"] != null)
                {
                    order = context1.Request.QueryString["order"];
                }
                if (!string.IsNullOrWhiteSpace(sort))
                {
                    if (sort == "PositionName")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.PositionName).ToList();
                        else
                            list = list.OrderByDescending(s => s.PositionName).ToList();
                    }
                    else if (sort == "CornerType")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.CornerType).ToList();
                        else
                            list = list.OrderByDescending(s => s.CornerType).ToList();
                    }
                    else if (sort == "MachineFrameNames")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.MachineFrameNames).ToList();
                        else
                            list = list.OrderByDescending(s => s.MachineFrameNames).ToList();
                    }
                }
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        string ProvinceName = string.Empty;
                        string CityName = string.Empty;
                        ProvinceName = GetProvinceNames(s.Id);
                        CityName = GetCityNames(s.Id);
                        string keeppopsize = s.KeepPOPSize != null && bool.Parse(s.KeepPOPSize.ToString()) ? "是" : "";
                        int isExcept = s.IsExcept == true ? 1 : 0;
                        //json.Append("{\"Id\":\"" + s.Id + "\",\"RegionNames\":\"" + s.RegionNames + "\",\"ProvinceId\":\"" + s.ProvinceId + "\",\"CityId\":\"" + s.CityId + "\",\"ProvinceName\":\"" + ProvinceName + "\",\"CityName\":\"" + CityName + "\",\"CityTier\":\"" + s.CityTier + "\",\"IsInstall\":\"" + s.IsInstall + "\",\"Format\":\"" + s.Format + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"POSScale\":\"" + s.POSScale + "\",\"Gender\":\"" + s.Gender + "\",\"ShopNos\":\"" + s.ShopNos + "\",\"PositionId\":\"" + s.PositionId + "\",\"PositionName\":\"" + s.PositionName + "\",\"MachineFrameIds\":\"" + s.MachineFrameIds + "\",\"MachineFrameNames\":\"" + s.MachineFrameNames + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicNo\":\"" + s.GraphicNo + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"WindowWidth\":\"" + s.WindowWidth + "\",\"WindowHigh\":\"" + s.WindowHigh + "\",\"WindowDeep\":\"" + s.WindowDeep + "\",\"ChooseImg\":\"" + s.ChooseImg + "\",\"CornerType\":\"" + s.CornerType + "\",\"KeepPOPSize\":\"" + keeppopsize + "\"},");
                        json.Append("{\"Id\":\"" + s.Id + "\",\"RegionNames\":\"" + s.RegionNames + "\",\"ProvinceId\":\"" + s.ProvinceId + "\",\"CityId\":\"" + s.CityId + "\",\"ProvinceName\":\"" + ProvinceName + "\",\"CityName\":\"" + CityName + "\",\"CityTier\":\"" + s.CityTier + "\",\"IsInstall\":\"" + s.IsInstall + "\",\"Format\":\"" + s.Format + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"POSScale\":\"" + s.POSScale + "\",\"Gender\":\"" + s.Gender + "\",\"ShopNos\":\"" + s.ShopNos + "\",\"IsExcept\":\"" + isExcept + "\",\"PositionId\":\"" + s.PositionId + "\",\"PositionName\":\"" + s.PositionName + "\",\"MachineFrameIds\":\"" + s.MachineFrameIds + "\",\"MachineFrameNames\":\"" + s.MachineFrameNames + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicNo\":\"" + s.GraphicNo + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"POPSize\":\"" + s.POPSize + "\",\"WindowSize\":\"" + s.WindowSize + "\",\"ChooseImg\":\"" + s.ChooseImg + "\",\"CornerType\":\"" + s.CornerType + "\",\"KeepPOPSize\":\"" + keeppopsize + "\",\"IsElectricity\":\"" + s.IsElectricity + "\"},");

                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                return "[]";
            }
            catch (Exception ex)
            {
                return "[]";
            }
        }

        string GetDetailList()
        {
            var list = splitPlanBll.GetList(s => s.PlanId == planId).OrderBy(s => s.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    string orderType = s.OrderType != null ? s.OrderType == 1 ? "pop" : "道具" : "pop";
                    string isInSet = s.IsInSet != null && s.IsInSet == true ? "是" : "";
                    string isPerShop = s.IsPerShop != null && s.IsPerShop == true ? "是" : "";
                    int isHCPOP = s.IsHCPOP == true ? 1 : 0;
                    json.Append("{\"Id\":\"" + s.Id + "\",\"OrderType\":\"" + orderType + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"CustomerMaterialId\":\"" + s.CustomerMaterialId + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"Quantity\":\"" + s.Quantity + "\",\"RackSalePrice\":\"" + s.RackSalePrice + "\",\"Remark\":\"" + s.Remark + "\",\"Supplier\":\"" + s.Supplier + "\",\"NewChooseImg\":\"" + s.NewChooseImg + "\",\"IsInSet\":\"" + isInSet + "\",\"IsPerShop\":\"" + isPerShop + "\",\"NewGender\":\"" + s.NewGender + "\",\"IsHCPOP\":\"" + isHCPOP + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

        string DeleteDetail(int detailId)
        {
            SplitOrderPlanDetail model = splitPlanBll.GetModel(detailId);
            if (model != null)
            {
                splitPlanBll.Delete(model);
                return "ok";
            }
            return "error";
        }

        string DeletePlan()
        {
            OrderPlan planModel = orderPlanBll.GetModel(planId);
            if (planModel != null)
            {
                if (orderPlanBll.Delete(planModel))
                {
                    splitPlanBll.Delete(s => s.PlanId == planId);
                }
                return "ok";
            }
            return "error";
        }

        /// <summary>
        /// 开始拆单
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="customerId"></param>
        /// <param name="planIds"></param>
        /// <returns></returns>
        string GoSplitOrder(string planIds)
        {
            SplitOrderPlanDetailBLL planDetailBll = new SplitOrderPlanDetailBLL();
            FinalOrderDetailTempBLL finalOrderTempBll = new FinalOrderDetailTempBLL();
            FinalOrderDetailTemp finalOrderTempModel;
            List<int> planIdList = StringHelper.ToIntList(planIds, ',');
            var planList = new OrderPlanBLL().GetList(s => s.CustomerId == customerId && s.PlanType == 1 && planIdList.Contains(s.Id));
            //已完成拆单的订单ID
            List<int> finishSplitId = new List<int>();
            //保持原来尺寸的orderid;
            List<int> keepId = new List<int>();
            int planNum = 0;
            TransactionOptions tOpt = new TransactionOptions();

            tOpt.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted; //设置TransactionOptions模式

            tOpt.Timeout = new TimeSpan(0, 5, 0); // 设置超时时间为5分钟        
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, tOpt))
            {
                try
                {
                    //先删除所有之前已拆完的订单
                    finalOrderTempBll.Delete(s => s.SubjectId == subjectId);

                    #region 从合并订单表（MergeOriginalOrder）进行拆单
                    var listOrder = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     join pop1 in CurrentContext.DbContext.POP
                                     on new { shopid = order.ShopId, graphicNo = order.GraphicNo } equals new { shopid = pop1.ShopId, graphicNo = pop1.GraphicNo } into popTemp
                                     from pop in popTemp.DefaultIfEmpty()
                                     where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                                     select new
                                     {
                                         order,
                                         shop,
                                         pop
                                     }).ToList();
                    #region 按方案拆单
                    if (listOrder.Any() && planList.Any())
                    {
                        List<string> InSetList = new List<string>();
                        HCPOPBLL hcPOPBll = new HCPOPBLL();
                        planList.ForEach(s =>
                        {
                            List<string> shopNoListMJ = new List<string>();//发毛巾的店铺（临时）
                            //Dictionary<string, Shop> dicMJ = new Dictionary<string, Shop>();//发毛巾的店铺（临时）
                            planNum++;
                            //var shopList = listOrder.Where(l => !finishSplitId.Contains(l.order.Id)).ToList();
                            var shopList = listOrder;
                            if (!string.IsNullOrWhiteSpace(s.ShopNos))
                            {
                                List<string> shopNoList = StringHelper.ToStringList(s.ShopNos, ',');
                                if (s.IsExcept != null && s.IsExcept == true)
                                {
                                    shopList = shopList.Where(l => !shopNoList.Contains(l.shop.ShopNo)).ToList();
                                }
                                else
                                    shopList = shopList.Where(l => shopNoList.Contains(l.shop.ShopNo)).ToList();
                            }


                            List<string> regionList = new List<string>();
                            regionList = StringHelper.ToStringList(s.RegionNames, ',');

                            //筛选区域
                            if (regionList.Any())
                            {
                                shopList = shopList.Where(l => regionList.Contains(l.shop.RegionName)).ToList();
                            }
                            //省份
                            string provinces = GetProvinceNames(s.Id);
                            if (!string.IsNullOrWhiteSpace(provinces))
                            {
                                List<string> pList = StringHelper.ToStringList(provinces, ',');
                                shopList = shopList.Where(l => pList.Contains(l.shop.ProvinceName)).ToList();
                            }
                            //城市
                            string cities = GetCityNames(s.Id);
                            if (!string.IsNullOrWhiteSpace(cities))
                            {
                                List<string> cList = StringHelper.ToStringList(cities, ',');
                                shopList = shopList.Where(l => cList.Contains(l.shop.CityName)).ToList();
                            }
                            //城市级别
                            if (!string.IsNullOrWhiteSpace(s.CityTier))
                            {
                                List<string> CityTierList = StringHelper.ToStringList(s.CityTier, ',');
                                shopList = shopList.Where(l => CityTierList.Contains(l.shop.CityTier)).ToList();
                            }
                            //是否安装
                            if (!string.IsNullOrWhiteSpace(s.IsInstall))
                            {
                                if (s.IsInstall.Contains("空"))
                                {
                                    List<string> installList0 = StringHelper.ToStringList(s.IsInstall.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (installList0.Any())
                                        shopList = shopList.Where(l => installList0.Contains(l.shop.IsInstall) || ((l.shop.IsInstall == null || l.shop.IsInstall == ""))).ToList();
                                    else
                                        shopList = shopList.Where(l => (l.shop.IsInstall == null || l.shop.IsInstall == "")).ToList();
                                }
                                else
                                {
                                    List<string> installList = StringHelper.ToStringList(s.IsInstall, ',');
                                    shopList = shopList.Where(l => installList.Contains(l.shop.IsInstall)).ToList();
                                }
                            }
                            //店铺类型
                            if (!string.IsNullOrWhiteSpace(s.Format))
                            {
                                List<string> FormatList = StringHelper.ToStringList(s.Format, ',');
                                shopList = shopList.Where(l => FormatList.Contains(l.shop.Format)).ToList();
                            }
                            //pop位置
                            if (!string.IsNullOrWhiteSpace(s.PositionName))
                            {
                                List<string> List = StringHelper.ToStringList(s.PositionName, ',');
                                shopList = shopList.Where(l => List.Contains(l.order.Sheet)).ToList();
                            }
                            //物料支持
                            if (!string.IsNullOrWhiteSpace(s.MaterialSupport))
                            {

                                if (s.MaterialSupport.Contains("空"))
                                {
                                    List<string> scaleList0 = StringHelper.ToStringList(s.MaterialSupport.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (scaleList0.Any())
                                        shopList = shopList.Where(l => scaleList0.Contains(l.order.MaterialSupport) || ((l.order.MaterialSupport == null || l.order.MaterialSupport == ""))).ToList();
                                    else
                                        shopList = shopList.Where(l => (l.order.MaterialSupport == null || l.order.MaterialSupport == "")).ToList();

                                }
                                else
                                {
                                    List<string> scaleList = StringHelper.ToStringList(s.MaterialSupport, ',');
                                    shopList = shopList.Where(l => scaleList.Contains(l.order.MaterialSupport)).ToList();
                                }
                            }
                            //店铺规模
                            if (!string.IsNullOrWhiteSpace(s.POSScale))
                            {

                                if (s.POSScale.Contains("空"))
                                {
                                    List<string> scaleList0 = StringHelper.ToStringList(s.POSScale.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (scaleList0.Any())
                                        shopList = shopList.Where(l => scaleList0.Contains(l.order.POSScale) || ((l.order.POSScale == null || l.order.POSScale == ""))).ToList();
                                    else
                                        shopList = shopList.Where(l => (l.order.POSScale == null || l.order.POSScale == "")).ToList();

                                }
                                else
                                {
                                    List<string> scaleList = StringHelper.ToStringList(s.POSScale, ',');
                                    shopList = shopList.Where(l => scaleList.Contains(l.order.POSScale)).ToList();
                                }
                            }

                            //性别
                            if (!string.IsNullOrWhiteSpace(s.Gender))
                            {
                                List<string> list = StringHelper.ToStringList(s.Gender, ',');
                                shopList = shopList.Where(l => list.Contains(l.order.Gender) || (l.order.Gender.Contains("男") && l.order.Gender.Contains("女"))).ToList();
                            }
                            //数量
                            if (!string.IsNullOrWhiteSpace(s.Quantity))
                            {
                                List<int> Quantitylist = StringHelper.ToIntList(s.Quantity, ',');
                                shopList = shopList.Where(l => Quantitylist.Contains(l.order.Quantity ?? 0)).ToList();
                            }
                            //POP编号
                            if (!string.IsNullOrWhiteSpace(s.GraphicNo))
                            {
                                List<string> list = StringHelper.ToStringList(s.GraphicNo, ',');
                                shopList = shopList.Where(l => list.Contains(l.order.GraphicNo)).ToList();
                            }
                            //pop材质
                            if (!string.IsNullOrWhiteSpace(s.GraphicMaterial))
                            {
                                List<string> list = StringHelper.ToStringList(s.GraphicMaterial, ',');
                                shopList = shopList.Where(l => l.pop != null && list.Contains(l.pop.GraphicMaterial)).ToList();
                            }
                            //POP尺寸
                            if (!string.IsNullOrWhiteSpace(s.POPSize))
                            {
                                if (s.POPSize.Contains("空"))
                                {
                                    List<string> list0 = StringHelper.ToStringList(s.POPSize.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (list0.Any())
                                    {
                                        //shopList = shopList.Where(l => l.pop != null && (list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) && (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))).ToList();
                                        shopList = shopList.Where(l => (l.pop != null && (list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) || (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))) || l.pop==null).ToList();

                                    }
                                    else
                                    {
                                        //shopList = shopList.Where(l => l.pop != null && l.pop.GraphicLength == null && l.pop.GraphicWidth == null).ToList();
                                        shopList = shopList.Where(l => (l.pop != null && ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) || (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0))) || l.pop==null).ToList();

                                    }

                                }
                                else
                                {
                                    List<string> list = StringHelper.ToStringList(s.POPSize, ',');
                                    shopList = shopList.Where(l => l.pop != null && list.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength)).ToList();
                                }

                            }
                            //Window尺寸
                            if (!string.IsNullOrWhiteSpace(s.WindowSize))
                            {
                                if (s.WindowSize.Contains("空"))
                                {
                                    List<string> list0 = StringHelper.ToStringList(s.WindowSize.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (list0.Any())
                                    {
                                        //shopList = shopList.Where(l => l.pop != null && (list0.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep) || ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) && (l.pop.WindowWide == null || l.pop.WindowWide == 0) && (l.pop.WindowDeep == null || l.pop.WindowDeep == 0)))).ToList();
                                        shopList = shopList.Where(l => (l.pop != null && (list0.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep) || ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) || (l.pop.WindowWide == null || l.pop.WindowWide == 0) || (l.pop.WindowDeep == null || l.pop.WindowDeep == 0)))) || l.pop==null).ToList();

                                    }
                                    else
                                    {
                                        //shopList = shopList.Where(l => l.pop != null && l.pop.WindowHigh == null && l.pop.WindowWide == null && l.pop.WindowDeep == null).ToList();
                                        shopList = shopList.Where(l => (l.pop != null && ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) || (l.pop.WindowWide == null || l.pop.WindowWide == 0) || (l.pop.WindowDeep == null || l.pop.WindowDeep == 0))) || l.pop==null).ToList();

                                    }
                                }
                                else
                                {
                                    List<string> list = StringHelper.ToStringList(s.WindowSize, ',');
                                    shopList = shopList.Where(l => l.pop != null && list.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep)).ToList();
                                }

                            }



                            //如果是橱窗，判断是否通电 
                            if (!string.IsNullOrWhiteSpace(s.PositionName) && s.PositionName == "橱窗" && !string.IsNullOrWhiteSpace(s.IsElectricity))
                            {
                                if (s.IsElectricity.Contains("空"))
                                {
                                    List<string> elcList0 = StringHelper.ToStringList(s.IsElectricity.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (elcList0.Any())
                                    {
                                        //shopList = shopList.Where(l => l.pop != null && (elcList0.Contains(l.pop.IsElectricity) || ((l.pop.IsElectricity == null || l.pop.IsElectricity == "")))).ToList();
                                        shopList = shopList.Where(l => l.pop != null ? (elcList0.Contains(l.pop.IsElectricity) || ((l.pop.IsElectricity == null || l.pop.IsElectricity == ""))) : (1 == 1)).ToList();

                                    }
                                    else
                                    {
                                        //shopList = shopList.Where(l => l.pop != null && ((l.pop.IsElectricity == null || l.pop.IsElectricity == ""))).ToList();
                                        shopList = shopList.Where(l => l.pop != null ? (l.pop.IsElectricity == null || l.pop.IsElectricity == "") : (1 == 1)).ToList();

                                    }
                                }
                                else
                                {
                                    List<string> elcList = StringHelper.ToStringList(s.IsElectricity, ',');
                                    shopList = shopList.Where(l => l.pop != null && elcList.Contains(l.pop.IsElectricity)).ToList();
                                }

                            }

                            //选图
                            if (!string.IsNullOrWhiteSpace(s.ChooseImg))
                            {
                                List<string> list = StringHelper.ToStringList(s.ChooseImg, ',');
                                shopList = shopList.Where(l => list.Contains(l.order.ChooseImg)).ToList();
                            }

                            //角落类型
                            if (!string.IsNullOrWhiteSpace(s.CornerType))
                            {

                                if (s.CornerType.Contains("空"))
                                {
                                    List<string> list0 = StringHelper.ToStringList(s.CornerType.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (list0.Any())
                                    {
                                        List<int> cornerShopList = frameBll.GetList(f => (list0.Contains(f.CornerType) || (f.CornerType == "" || f.CornerType == null)) && f.PositionName == s.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                        shopList = shopList.Where(l => cornerShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                    }
                                    else
                                    {
                                        List<int> cornerShopList = frameBll.GetList(f => (f.CornerType == "" || f.CornerType == null) && f.PositionName == s.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                        shopList = shopList.Where(l => cornerShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                    }
                                }
                                else
                                {
                                    List<string> cornerList = StringHelper.ToStringList(s.CornerType, ',');
                                    List<int> cornerShopList = frameBll.GetList(f => cornerList.Contains(f.CornerType) && f.PositionName == s.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                    shopList = shopList.Where(l => cornerShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                }


                            }


                            var planDetail = planDetailBll.GetList(d => d.PlanId == s.Id && (d.IsPerShop == null || d.IsPerShop == false));
                            #region 有器架类型
                            if (!string.IsNullOrWhiteSpace(s.MachineFrameNames))
                            {
                                List<string> frames = new List<string>();
                                int isKong = 0;
                                if (s.MachineFrameNames.Contains("空"))
                                {
                                    List<string> list0 = StringHelper.ToStringList(s.MachineFrameNames.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (list0.Any())//除了空，还有其他的器架
                                    {

                                        //所选择的器架类型的店铺
                                        List<int> frameShopList = frameBll.GetList(f => f.PositionName == s.PositionName && list0.Contains(f.MachineFrame)).Select(f => f.ShopId ?? 0).ToList();
                                        //当前位置的所有器架类型的店铺
                                        List<int> frameShopList1 = frameBll.GetList(f => f.PositionName == s.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                        //当前位置没有器架类型的店铺
                                        List<int> shopListNoFrame = shopList.Where(l => !frameShopList1.Contains(l.order.ShopId ?? 0)).Select(l => l.order.ShopId ?? 0).ToList();
                                        frameShopList.AddRange(shopListNoFrame);
                                        shopList = shopList.Where(l => frameShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                        frames = list0;
                                    }
                                    else//只有空
                                    {
                                        //当前位置的所有器架类型的店铺
                                        List<int> frameShopList1 = frameBll.GetList(f => f.PositionName == s.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                        shopList = shopList.Where(l => !frameShopList1.Contains(l.order.ShopId ?? 0)).ToList();
                                    }
                                    isKong = 1;
                                }
                                else
                                {
                                    frames = StringHelper.ToStringList(s.MachineFrameNames, ',');
                                    var frameShopList = frameBll.GetList(f => f.PositionName == s.PositionName && frames.Contains(f.MachineFrame)).Select(f => f.ShopId ?? 0).ToList();
                                    shopList = shopList.Where(l => frameShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                }
                                #region 保留POP原尺寸
                                //if ((s.KeepPOPSize ?? false) == true)
                                //{
                                //    shopList.ForEach(o =>
                                //    {
                                //        bool canGo = false;
                                //        string frameName = string.Empty;
                                //        if (isKong == 0)
                                //        {
                                //            var frameList = frameBll.GetList(f => f.ShopId == o.shop.Id && f.PositionName == o.order.Sheet && frames.Contains(f.MachineFrame) && f.Gender == o.order.Gender).FirstOrDefault();
                                //            if (frameList != null)
                                //            {
                                //                frameName = frameList.MachineFrame;
                                //                canGo = true;
                                //            }
                                //        }
                                //        else
                                //        {
                                //            if (frames.Any())
                                //            {
                                //                var frameList = frameBll.GetList(f => f.ShopId == o.shop.Id && f.PositionName == o.order.Sheet && frames.Contains(f.MachineFrame) && f.Gender == o.order.Gender).FirstOrDefault();
                                //                if (frameList != null)
                                //                {
                                //                    frameName = frameList.MachineFrame;

                                //                }
                                //                canGo = true;
                                //            }
                                //            else
                                //            {
                                //                canGo = true;
                                //            }

                                //        }
                                //        if (canGo && o.pop != null && !keepId.Contains(o.order.Id))
                                //        {
                                //            finalOrderTempModel = new FinalOrderDetailTemp();
                                //            finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                //            finalOrderTempModel.AgentName = o.shop.AgentName;
                                //            finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                //            finalOrderTempModel.Channel = o.shop.Channel;
                                //            finalOrderTempModel.City = o.shop.CityName;
                                //            finalOrderTempModel.CityTier = o.shop.CityTier;
                                //            finalOrderTempModel.Contact = o.shop.Contact1;
                                //            finalOrderTempModel.Format = o.shop.Format;
                                //            finalOrderTempModel.Gender = o.order.Gender;
                                //            finalOrderTempModel.GraphicNo = o.order.GraphicNo;

                                //            if (o.pop != null && o.pop.GraphicLength != null && o.pop.GraphicWidth != null)
                                //            {
                                //                decimal area = (o.pop.GraphicLength ?? 0 * o.pop.GraphicWidth ?? 0) / 1000000;
                                //                finalOrderTempModel.Area = area;
                                //            }
                                //            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                //            finalOrderTempModel.LocationType = o.shop.LocationType;
                                //            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                //            finalOrderTempModel.OrderType = 1;
                                //            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                //            finalOrderTempModel.POSScale = o.order.POSScale;
                                //            finalOrderTempModel.Province = o.shop.ProvinceName;
                                //            finalOrderTempModel.Quantity = o.order.Quantity;
                                //            finalOrderTempModel.Region = o.shop.RegionName;
                                //            finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                                //            //finalOrderTempModel.Remark = o.order.Remark;
                                //            finalOrderTempModel.Remark = "保持原尺寸";
                                //            finalOrderTempModel.Sheet = o.order.Sheet;
                                //            finalOrderTempModel.ShopId = o.shop.Id;
                                //            finalOrderTempModel.ShopName = o.shop.ShopName;
                                //            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                //            finalOrderTempModel.SubjectId = subjectId;
                                //            finalOrderTempModel.Tel = o.shop.Tel1;
                                //            finalOrderTempModel.MachineFrame = frameName;
                                //            finalOrderTempModel.LevelNum = o.order.LevelNum;
                                //            if (o.pop != null)
                                //            {
                                //                finalOrderTempModel.GraphicLength = o.pop.GraphicLength;
                                //                finalOrderTempModel.GraphicMaterial = o.pop.GraphicMaterial;
                                //                finalOrderTempModel.GraphicWidth = o.pop.GraphicWidth;
                                //                finalOrderTempModel.UnitPrice = o.pop.UnitPrice ?? 0;
                                //                finalOrderTempModel.PositionDescription = o.pop.PositionDescription;
                                //                finalOrderTempModel.POPName = o.pop.POPName;
                                //                finalOrderTempModel.POPType = o.pop.POPType;
                                //                finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                //                finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                //                finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                //                finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                //                finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                //            }
                                //            finalOrderTempBll.Add(finalOrderTempModel);
                                //            keepId.Add(o.order.Id);
                                //        }

                                //    });
                                //}
                                #endregion


                                #region 执行方案内容
                                if (shopList.Any() && planDetail.Any())
                                {
                                    shopList.ForEach(o =>
                                    {

                                        //var frameList = frameBll.GetList(f => f.ShopId == o.shop.Id && f.PositionName == o.order.Sheet && frames.Contains(f.MachineFrame) && f.Gender == o.order.Gender).FirstOrDefault();
                                        var frameList = frameBll.GetList(f => f.ShopId == o.shop.Id && f.PositionName == o.order.Sheet && frames.Contains(f.MachineFrame) && f.Gender == o.order.Gender).Select(f => f.MachineFrame).ToList();
                                        if (!frameList.Any() && isKong == 1)
                                        {
                                            frameList.Add("");
                                        }
                                        if (frameList.Any())
                                        {


                                            frameList.ForEach(f =>
                                            {


                                                #region 保留POP原尺寸
                                                if ((s.KeepPOPSize ?? false) == true)
                                                {

                                                    finalOrderTempModel = new FinalOrderDetailTemp();
                                                    finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                                    finalOrderTempModel.AgentName = o.shop.AgentName;
                                                    finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                                    finalOrderTempModel.Channel = o.shop.Channel;
                                                    finalOrderTempModel.City = o.shop.CityName;
                                                    finalOrderTempModel.CityTier = o.shop.CityTier;
                                                    finalOrderTempModel.Contact = o.shop.Contact1;
                                                    finalOrderTempModel.Format = o.shop.Format;
                                                    finalOrderTempModel.Gender = o.order.Gender;
                                                    finalOrderTempModel.GraphicNo = o.order.GraphicNo;


                                                    finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                    finalOrderTempModel.LocationType = o.shop.LocationType;
                                                    finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                    finalOrderTempModel.OrderType = 1;
                                                    finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                    finalOrderTempModel.POSScale = o.order.POSScale;
                                                    finalOrderTempModel.Province = o.shop.ProvinceName;
                                                    finalOrderTempModel.Quantity = o.order.Quantity;
                                                    finalOrderTempModel.Region = o.shop.RegionName;
                                                    finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                                                    //finalOrderTempModel.Remark = o.order.Remark;
                                                    finalOrderTempModel.Remark = "保持原尺寸";
                                                    finalOrderTempModel.Sheet = o.order.Sheet;
                                                    finalOrderTempModel.ShopId = o.shop.Id;
                                                    finalOrderTempModel.ShopName = o.shop.ShopName;
                                                    finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                                    finalOrderTempModel.SubjectId = subjectId;
                                                    finalOrderTempModel.Tel = o.shop.Tel1;
                                                    finalOrderTempModel.MachineFrame = f;
                                                    finalOrderTempModel.LevelNum = o.order.LevelNum;
                                                    if (o.pop != null)
                                                    {
                                                        decimal width = o.pop.GraphicWidth ?? 0;
                                                        decimal length = o.pop.GraphicLength ?? 0;
                                                        if (o.order.Sheet == "橱窗")
                                                        {
                                                            width = o.pop.WindowWide ?? 0;
                                                            length = o.pop.WindowHigh ?? 0;

                                                        }
                                                        finalOrderTempModel.GraphicLength = length;
                                                        finalOrderTempModel.GraphicMaterial = o.pop.GraphicMaterial;
                                                        finalOrderTempModel.GraphicWidth = width;
                                                        finalOrderTempModel.UnitPrice = o.pop.UnitPrice ?? 0;
                                                        finalOrderTempModel.PositionDescription = o.pop.PositionDescription;
                                                        finalOrderTempModel.POPName = o.pop.POPName;
                                                        finalOrderTempModel.POPType = o.pop.POPType;
                                                        finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                                        finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                                        finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                                        finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                                        finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                                        finalOrderTempModel.Area =Math.Round((width * length) / 1000000,2);
                                                    }
                                                    finalOrderTempBll.Add(finalOrderTempModel);
                                                    //keepId.Add(o.order.Id);
                                                }
                                                #endregion


                                                #region
                                                planDetail.ForEach(p =>
                                                {
                                                    #region 拆成男女一套的
                                                    string inSetModel = o.shop.Id + "|" + o.order.Sheet + "|" + f;
                                                    if (p.IsInSet != null && p.IsInSet == true)
                                                    {
                                                        if (!InSetList.Contains(inSetModel))
                                                        {
                                                            InSetList.Add(inSetModel);

                                                            finalOrderTempModel = new FinalOrderDetailTemp();
                                                            finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                                            finalOrderTempModel.AgentName = o.shop.AgentName;
                                                            finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                                            finalOrderTempModel.Channel = o.shop.Channel;
                                                            finalOrderTempModel.City = o.shop.CityName;
                                                            finalOrderTempModel.CityTier = o.shop.CityTier;
                                                            finalOrderTempModel.Contact = o.shop.Contact1;
                                                            finalOrderTempModel.Format = o.shop.Format;
                                                            finalOrderTempModel.Gender = o.order.Gender;
                                                            finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                                            finalOrderTempModel.GraphicLength = p.GraphicLength;
                                                            finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                                                            finalOrderTempModel.GraphicMaterial = p.GraphicMaterial;
                                                            finalOrderTempModel.GraphicWidth = p.GraphicWidth;
                                                            if (p.GraphicLength != null && p.GraphicWidth != null)
                                                            {
                                                                decimal area = (p.GraphicLength ?? 0 * p.GraphicWidth ?? 0) / 1000000;
                                                                finalOrderTempModel.Area =Math.Round(area,2);
                                                            }
                                                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                            finalOrderTempModel.LocationType = o.shop.LocationType;
                                                            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                            finalOrderTempModel.OrderType = p.OrderType;
                                                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                            finalOrderTempModel.POSScale = o.order.POSScale;
                                                            finalOrderTempModel.Province = o.shop.ProvinceName;
                                                            finalOrderTempModel.Quantity = p.Quantity;
                                                            finalOrderTempModel.Region = o.shop.RegionName;
                                                            finalOrderTempModel.ChooseImg = !string.IsNullOrWhiteSpace(p.NewChooseImg) ? p.NewChooseImg : o.order.ChooseImg;
                                                            finalOrderTempModel.PositionDescription = p.Remark;
                                                            finalOrderTempModel.Sheet = o.order.Sheet;
                                                            finalOrderTempModel.ShopId = o.shop.Id;
                                                            finalOrderTempModel.ShopName = o.shop.ShopName;
                                                            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                                            finalOrderTempModel.SubjectId = subjectId;
                                                            finalOrderTempModel.Tel = o.shop.Tel1;
                                                            finalOrderTempModel.UnitPrice = p.RackSalePrice ?? 0;
                                                            finalOrderTempModel.MachineFrame = f;
                                                            finalOrderTempModel.MachineFrameNum = frameList.Count;
                                                            finalOrderTempModel.LevelNum = o.order.LevelNum;
                                                            if (frameList.Count < p.Quantity)
                                                            {
                                                                finalOrderTempModel.AlertMsg = "订单数量大于系统器架类型数量";
                                                            }

                                                            if (o.pop != null)
                                                            {
                                                                finalOrderTempModel.POPName = o.pop.POPName;
                                                                finalOrderTempModel.POPType = o.pop.POPType;
                                                                finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                                                finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                                                finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                                                finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                                                finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                                            }

                                                            finalOrderTempBll.Add(finalOrderTempModel);
                                                        }
                                                    }
                                                    #endregion
                                                    else
                                                    {
                                                        InSetList.Add(inSetModel);

                                                        decimal popWidth = p.GraphicWidth ?? 0;
                                                        decimal popHeight = p.GraphicLength ?? 0;
                                                        int count = p.Quantity ?? 0;
                                                        string material = p.GraphicMaterial;

                                                        if (p.IsHCPOP != null && p.IsHCPOP == true)
                                                        {
                                                            //如果是HC器架POP
                                                            var hcPOP = hcPOPBll.GetList(h => h.MachineFrame.ToUpper() == s.MachineFrameNames.ToUpper() && h.ShopId == o.shop.Id && h.POP.ToUpper() == p.Remark.ToUpper() && ((p.NewGender != null && p.NewGender != "") ? (h.POPGender == p.NewGender) : (1 == 1))).FirstOrDefault();

                                                            if (hcPOP != null)
                                                            {
                                                                count = count == 0 ? hcPOP.Count ?? 0 : count;
                                                                popWidth = hcPOP.GraphicWidth ?? 0;
                                                                popHeight = hcPOP.GraphicLength ?? 0;
                                                                material = !string.IsNullOrWhiteSpace(material) ? material : hcPOP.GraphicMaterial;

                                                            }

                                                        }
                                                        else if (o.order.Sheet == "橱窗" && o.pop != null)
                                                        {
                                                            if (p.Remark == "左侧贴")
                                                            {
                                                                popWidth = o.pop.WindowDeep ?? 0;
                                                                popHeight = o.pop.WindowHigh ?? 0;
                                                            }
                                                            if (p.Remark == "右侧贴")
                                                            {
                                                                popWidth = o.pop.WindowDeep ?? 0;
                                                                popHeight = o.pop.WindowHigh ?? 0;
                                                            }
                                                            if (p.Remark == "地铺")
                                                            {
                                                                popWidth = o.pop.WindowWide ?? 0;
                                                                popHeight = o.pop.WindowDeep ?? 0;
                                                            }
                                                        }


                                                        finalOrderTempModel = new FinalOrderDetailTemp();
                                                        finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                                        finalOrderTempModel.AgentName = o.shop.AgentName;
                                                        finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                                        finalOrderTempModel.Channel = o.shop.Channel;
                                                        finalOrderTempModel.City = o.shop.CityName;
                                                        finalOrderTempModel.CityTier = o.shop.CityTier;
                                                        finalOrderTempModel.Contact = o.shop.Contact1;
                                                        finalOrderTempModel.Format = o.shop.Format;
                                                        finalOrderTempModel.Gender = !string.IsNullOrWhiteSpace(p.NewGender) ? p.NewGender : o.order.Gender;
                                                        finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                                        finalOrderTempModel.GraphicLength = popHeight;
                                                        finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                                                        finalOrderTempModel.GraphicMaterial = material;
                                                        finalOrderTempModel.GraphicWidth = popWidth;
                                                        if (popHeight > 0 && popWidth > 0)
                                                        {

                                                            finalOrderTempModel.Area =Math.Round((popHeight * popWidth) / 1000000,2);
                                                        }
                                                        finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                        finalOrderTempModel.LocationType = o.shop.LocationType;
                                                        finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                        finalOrderTempModel.OrderType = p.OrderType;
                                                        finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                        finalOrderTempModel.POSScale = o.order.POSScale;
                                                        finalOrderTempModel.Province = o.shop.ProvinceName;
                                                        finalOrderTempModel.Quantity = count;
                                                        finalOrderTempModel.Region = o.shop.RegionName;
                                                        finalOrderTempModel.ChooseImg = !string.IsNullOrWhiteSpace(p.NewChooseImg) ? p.NewChooseImg : o.order.ChooseImg;
                                                        finalOrderTempModel.PositionDescription = p.Remark;
                                                        finalOrderTempModel.Sheet = o.order.Sheet;
                                                        finalOrderTempModel.ShopId = o.shop.Id;
                                                        finalOrderTempModel.ShopName = o.shop.ShopName;
                                                        finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                                        finalOrderTempModel.SubjectId = subjectId;
                                                        finalOrderTempModel.Tel = o.shop.Tel1;
                                                        finalOrderTempModel.UnitPrice = p.RackSalePrice ?? 0;
                                                        finalOrderTempModel.MachineFrame = f;
                                                        finalOrderTempModel.MachineFrameNum = frameList.Count;
                                                        finalOrderTempModel.LevelNum = o.order.LevelNum;
                                                        if (frameList.Count < p.Quantity)
                                                        {
                                                            finalOrderTempModel.AlertMsg = "订单数量大于系统器架类型数量";
                                                        }

                                                        if (o.pop != null)
                                                        {
                                                            finalOrderTempModel.POPName = o.pop.POPName;
                                                            finalOrderTempModel.POPType = o.pop.POPType;
                                                            finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                                            finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                                            finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                                            finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                                            finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                                        }
                                                        finalOrderTempBll.Add(finalOrderTempModel);
                                                    }
                                                    finishSplitId.Add(o.order.Id);

                                                });
                                                #endregion
                                            });
                                        }
                                    });
                                }
                                #endregion
                            }
                            #endregion
                            #region 以下是没有“器架类型”条件
                            else
                            {

                                #region 保留POP原尺寸
                                if ((s.KeepPOPSize ?? false) == true)
                                {
                                    shopList.ForEach(o =>
                                    {
                                        if (o.pop != null && !keepId.Contains(o.order.Id))
                                        {
                                            finalOrderTempModel = new FinalOrderDetailTemp();
                                            finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                            finalOrderTempModel.AgentName = o.shop.AgentName;
                                            finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                            finalOrderTempModel.Channel = o.shop.Channel;
                                            finalOrderTempModel.City = o.shop.CityName;
                                            finalOrderTempModel.CityTier = o.shop.CityTier;
                                            finalOrderTempModel.Contact = o.shop.Contact1;
                                            finalOrderTempModel.Format = o.shop.Format;
                                            finalOrderTempModel.Gender = o.order.Gender;
                                            finalOrderTempModel.GraphicNo = o.order.GraphicNo;


                                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                            finalOrderTempModel.LocationType = o.shop.LocationType;
                                            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                            finalOrderTempModel.OrderType = 1;//默认是pop
                                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                            finalOrderTempModel.POSScale = o.order.POSScale;
                                            finalOrderTempModel.Province = o.shop.ProvinceName;
                                            finalOrderTempModel.Quantity = o.order.Quantity;
                                            finalOrderTempModel.Region = o.shop.RegionName;
                                            finalOrderTempModel.ChooseImg = o.order.ChooseImg;

                                            finalOrderTempModel.Remark = "保持原尺寸";
                                            finalOrderTempModel.Sheet = o.order.Sheet;
                                            finalOrderTempModel.ShopId = o.shop.Id;
                                            finalOrderTempModel.ShopName = o.shop.ShopName;
                                            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                            finalOrderTempModel.SubjectId = subjectId;
                                            finalOrderTempModel.Tel = o.shop.Tel1;
                                            finalOrderTempModel.LevelNum = o.order.LevelNum;
                                            if (o.pop != null)
                                            {
                                                decimal width = o.pop.GraphicWidth ?? 0;
                                                decimal length = o.pop.GraphicLength ?? 0;
                                                if (o.order.Sheet == "橱窗")
                                                {
                                                    width = o.pop.WindowWide ?? 0;
                                                    length = o.pop.WindowHigh ?? 0;

                                                }
                                                finalOrderTempModel.GraphicLength = length;
                                                finalOrderTempModel.GraphicMaterial = o.pop.GraphicMaterial;
                                                finalOrderTempModel.GraphicWidth = width;
                                                finalOrderTempModel.UnitPrice = o.pop.UnitPrice ?? 0;
                                                finalOrderTempModel.PositionDescription = o.pop.PositionDescription;
                                                finalOrderTempModel.POPName = o.pop.POPName;
                                                finalOrderTempModel.POPType = o.pop.POPType;
                                                finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                                finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                                finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                                finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                                finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                                finalOrderTempModel.Area =Math.Round((length * width) / 1000000,2);
                                            }


                                            finalOrderTempBll.Add(finalOrderTempModel);
                                            keepId.Add(o.order.Id);
                                        }

                                    });
                                }
                                #endregion
                                #region 执行方案内容
                                if (shopList.Any() && planDetail.Any())
                                {
                                    shopList.ForEach(o =>
                                    {

                                        planDetail.ForEach(p =>
                                        {
                                            string inSetModel = o.shop.Id + "|" + o.order.Sheet + "|" + "";
                                            if (p.IsInSet != null && p.IsInSet == true)
                                            {
                                                if (!InSetList.Contains(inSetModel))
                                                {
                                                    InSetList.Add(inSetModel);
                                                    finalOrderTempModel = new FinalOrderDetailTemp();
                                                    finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                                    finalOrderTempModel.AgentName = o.shop.AgentName;
                                                    finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                                    finalOrderTempModel.Channel = o.shop.Channel;
                                                    finalOrderTempModel.City = o.shop.CityName;
                                                    finalOrderTempModel.CityTier = o.shop.CityTier;
                                                    finalOrderTempModel.Contact = o.shop.Contact1;
                                                    finalOrderTempModel.Format = o.shop.Format;
                                                    finalOrderTempModel.Gender = o.order.Gender;
                                                    finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                                    finalOrderTempModel.GraphicLength = p.GraphicLength;
                                                    finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                                                    finalOrderTempModel.GraphicMaterial = p.GraphicMaterial;
                                                    finalOrderTempModel.GraphicWidth = p.GraphicWidth;
                                                    if (p.GraphicLength != null && p.GraphicWidth != null)
                                                    {
                                                        decimal area = (p.GraphicLength ?? 0 * p.GraphicWidth ?? 0) / 1000000;
                                                        finalOrderTempModel.Area = area;
                                                    }
                                                    finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                    finalOrderTempModel.LocationType = o.shop.LocationType;
                                                    finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                    finalOrderTempModel.OrderType = p.OrderType;
                                                    finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                    finalOrderTempModel.POSScale = o.order.POSScale;
                                                    finalOrderTempModel.Province = o.shop.ProvinceName;
                                                    finalOrderTempModel.Quantity = p.Quantity;
                                                    finalOrderTempModel.Region = o.shop.RegionName;
                                                    finalOrderTempModel.ChooseImg = !string.IsNullOrWhiteSpace(p.NewChooseImg) ? p.NewChooseImg : o.order.ChooseImg;
                                                    finalOrderTempModel.PositionDescription = p.Remark;
                                                    finalOrderTempModel.Sheet = o.order.Sheet;
                                                    finalOrderTempModel.ShopId = o.shop.Id;
                                                    finalOrderTempModel.ShopName = o.shop.ShopName;
                                                    finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                                    finalOrderTempModel.SubjectId = subjectId;
                                                    finalOrderTempModel.Tel = o.shop.Tel1;

                                                    finalOrderTempModel.UnitPrice = p.RackSalePrice ?? 0;
                                                    finalOrderTempModel.MachineFrame = s.MachineFrameNames;
                                                    finalOrderTempModel.LevelNum = o.order.LevelNum;

                                                    if (o.pop != null)
                                                    {
                                                        finalOrderTempModel.POPName = o.pop.POPName;
                                                        finalOrderTempModel.POPType = o.pop.POPType;
                                                        finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                                        finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                                        finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                                        finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                                        finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                                    }
                                                    finalOrderTempBll.Add(finalOrderTempModel);
                                                }
                                            }
                                            else
                                            {
                                                decimal popWidth = p.GraphicWidth ?? 0;
                                                decimal popHeight = p.GraphicLength ?? 0;
                                                if (o.order.Sheet == "橱窗" && o.pop != null)
                                                {
                                                    if (p.Remark == "左侧贴")
                                                    {
                                                        popWidth = o.pop.WindowDeep ?? 0;
                                                        popHeight = o.pop.WindowHigh ?? 0;
                                                    }
                                                    if (p.Remark == "右侧贴")
                                                    {
                                                        popWidth = o.pop.WindowDeep ?? 0;
                                                        popHeight = o.pop.WindowHigh ?? 0;
                                                    }
                                                    if (p.Remark == "地铺")
                                                    {
                                                        popWidth = o.pop.WindowWide ?? 0;
                                                        popHeight = o.pop.WindowDeep ?? 0;
                                                    }
                                                }
                                                finalOrderTempModel = new FinalOrderDetailTemp();
                                                finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                                finalOrderTempModel.AgentName = o.shop.AgentName;
                                                finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                                finalOrderTempModel.Channel = o.shop.Channel;
                                                finalOrderTempModel.City = o.shop.CityName;
                                                finalOrderTempModel.CityTier = o.shop.CityTier;
                                                finalOrderTempModel.Contact = o.shop.Contact1;
                                                finalOrderTempModel.Format = o.shop.Format;
                                                finalOrderTempModel.Gender = o.order.Gender;
                                                finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                                finalOrderTempModel.GraphicLength = popHeight;
                                                finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                                                finalOrderTempModel.GraphicMaterial = p.GraphicMaterial;
                                                finalOrderTempModel.GraphicWidth = popWidth;
                                                if (popWidth > 0 && popHeight > 0)
                                                {
                                                    decimal area = (popHeight * popWidth) / 1000000;
                                                    finalOrderTempModel.Area = area;
                                                }
                                                finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                finalOrderTempModel.LocationType = o.shop.LocationType;
                                                finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                finalOrderTempModel.OrderType = p.OrderType;
                                                finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                finalOrderTempModel.POSScale = o.order.POSScale;
                                                finalOrderTempModel.Province = o.shop.ProvinceName;
                                                finalOrderTempModel.Quantity = p.Quantity;
                                                finalOrderTempModel.Region = o.shop.RegionName;
                                                finalOrderTempModel.ChooseImg = !string.IsNullOrWhiteSpace(p.NewChooseImg) ? p.NewChooseImg : o.order.ChooseImg;
                                                finalOrderTempModel.PositionDescription = p.Remark;
                                                finalOrderTempModel.Sheet = o.order.Sheet;
                                                finalOrderTempModel.ShopId = o.shop.Id;
                                                finalOrderTempModel.ShopName = o.shop.ShopName;
                                                finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                                finalOrderTempModel.SubjectId = subjectId;
                                                finalOrderTempModel.Tel = o.shop.Tel1;

                                                finalOrderTempModel.UnitPrice = p.RackSalePrice ?? 0;
                                                finalOrderTempModel.MachineFrame = s.MachineFrameNames;
                                                finalOrderTempModel.LevelNum = o.order.LevelNum;

                                                if (o.pop != null)
                                                {
                                                    finalOrderTempModel.POPName = o.pop.POPName;
                                                    finalOrderTempModel.POPType = o.pop.POPType;
                                                    finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                                    finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                                    finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                                    finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                                    finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                                }
                                                finalOrderTempBll.Add(finalOrderTempModel);
                                            }

                                        });
                                        finishSplitId.Add(o.order.Id);
                                    });
                                }
                                #endregion
                                //}

                            }
                            #endregion
                        });
                        //finishSplitId.Clear();

                        //将单店设置表（SplitOrderDetailPerShop）的数据添加到结果表
                        var perList = (from per in CurrentContext.DbContext.SplitOrderDetailPerShop
                                       join shop in CurrentContext.DbContext.Shop
                                       on per.ShopId equals shop.Id
                                       join order in CurrentContext.DbContext.MergeOriginalOrder
                                       on per.OrderId equals order.Id
                                       where per.SubjectId == subjectId
                                       select new { per, shop, order }).ToList();
                        perList.ForEach(o =>
                        {
                            finalOrderTempModel = new FinalOrderDetailTemp();
                            finalOrderTempModel.AgentCode = o.shop.AgentCode;
                            finalOrderTempModel.AgentName = o.shop.AgentName;
                            finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                            finalOrderTempModel.Channel = o.shop.Channel;
                            finalOrderTempModel.City = o.shop.CityName;
                            finalOrderTempModel.CityTier = o.shop.CityTier;
                            finalOrderTempModel.Contact = o.shop.Contact1;
                            finalOrderTempModel.Format = o.shop.Format;
                            finalOrderTempModel.Gender = o.per.Gender;
                            finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                            finalOrderTempModel.GraphicLength = o.per.GraphicLength;
                            //finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                            finalOrderTempModel.GraphicMaterial = o.per.GraphicMaterial;
                            finalOrderTempModel.GraphicWidth = o.per.GraphicWidth;
                            if (o.per.GraphicWidth != null && o.per.GraphicLength != null)
                            {
                                decimal area = (o.per.GraphicLength ?? 0 * o.per.GraphicWidth ?? 0) / 1000000;
                                finalOrderTempModel.Area =Math.Round(area,2);
                            }
                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                            finalOrderTempModel.LocationType = o.shop.LocationType;
                            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                            finalOrderTempModel.OrderType = o.per.OrderType;
                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                            finalOrderTempModel.POSScale = o.order.POSScale;
                            finalOrderTempModel.Province = o.shop.ProvinceName;
                            finalOrderTempModel.Quantity = o.per.Quantity;
                            finalOrderTempModel.Region = o.shop.RegionName;
                            finalOrderTempModel.ChooseImg = o.per.NewChooseImg;
                            finalOrderTempModel.PositionDescription = o.per.Remark;
                            finalOrderTempModel.Sheet = o.per.Sheet;
                            finalOrderTempModel.ShopId = o.shop.Id;
                            finalOrderTempModel.ShopName = o.shop.ShopName;
                            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                            finalOrderTempModel.SubjectId = subjectId;
                            finalOrderTempModel.Tel = o.shop.Tel1;

                            finalOrderTempModel.UnitPrice = o.per.RackSalePrice ?? 0;
                            finalOrderTempModel.MachineFrame = o.per.MachineFrame;
                            //finalOrderTempModel.LevelNum = o.order.LevelNum;

                            finalOrderTempBll.Add(finalOrderTempModel);
                        });
                    }
                    #endregion
                    #region 以下是没有在方案的订单信息
                    var shopList1 = listOrder.Where(l => !finishSplitId.Contains(l.order.Id)).ToList();
                    if (shopList1.Any())
                    {
                        shopList1.ForEach(o =>
                        {

                            finalOrderTempModel = new FinalOrderDetailTemp();
                            finalOrderTempModel.AgentCode = o.shop.AgentCode;
                            finalOrderTempModel.AgentName = o.shop.AgentName;
                            finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                            finalOrderTempModel.Channel = o.shop.Channel;
                            finalOrderTempModel.City = o.shop.CityName;
                            finalOrderTempModel.CityTier = o.shop.CityTier;
                            finalOrderTempModel.Contact = o.shop.Contact1;
                            finalOrderTempModel.Format = o.shop.Format;
                            finalOrderTempModel.Gender = o.order.Gender;
                            finalOrderTempModel.GraphicNo = o.order.GraphicNo;


                            finalOrderTempModel.CustomerMaterialId = 0;

                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                            finalOrderTempModel.LocationType = o.shop.LocationType;
                            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                            finalOrderTempModel.OrderType = 1;//默认pop
                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                            finalOrderTempModel.POSScale = o.order.POSScale;
                            finalOrderTempModel.Province = o.shop.ProvinceName;
                            finalOrderTempModel.Quantity = o.order.Quantity;
                            finalOrderTempModel.Region = o.shop.RegionName;
                            finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                            //finalOrderTempModel.Remark = o.order.Remark;
                            finalOrderTempModel.Remark = "方案外";
                            finalOrderTempModel.Sheet = o.order.Sheet;
                            finalOrderTempModel.ShopId = o.shop.Id;
                            finalOrderTempModel.ShopName = o.shop.ShopName;
                            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                            finalOrderTempModel.SubjectId = subjectId;
                            finalOrderTempModel.Tel = o.shop.Tel1;
                            finalOrderTempModel.LevelNum = o.order.LevelNum;
                            if (o.pop != null)
                            {
                                decimal width = o.pop.GraphicWidth ?? 0;
                                decimal length = o.pop.GraphicLength ?? 0;
                                if (o.order.Sheet == "橱窗")
                                {
                                    width = o.pop.WindowWide ?? 0;
                                    length = o.pop.WindowHigh ?? 0;

                                }
                                decimal area = (width * length) / 1000000;
                                finalOrderTempModel.GraphicMaterial = o.pop.GraphicMaterial;
                                finalOrderTempModel.GraphicLength = length;
                                finalOrderTempModel.GraphicWidth = width;
                                finalOrderTempModel.Area =Math.Round(area,2);
                                finalOrderTempModel.UnitPrice = o.pop.UnitPrice ?? 0;
                                finalOrderTempModel.PositionDescription = o.pop.PositionDescription;
                                finalOrderTempModel.POPName = o.pop.POPName;
                                finalOrderTempModel.POPType = o.pop.POPType;
                                finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                            }
                            try
                            {
                                finalOrderTempBll.Add(finalOrderTempModel);
                            }
                            catch (Exception ex)
                            {
                                string s = ex.Message;
                            }
                        });
                    }
                    #endregion
                    #endregion

                    SubjectBLL subjectBll = new SubjectBLL();
                    Models.Subject subjectModel = subjectBll.GetModel(subjectId);
                    if (subjectModel != null)
                    {
                        subjectModel.Status = 3;//拆单完成
                        subjectModel.ApproveState = 0;
                        subjectModel.SplitPlanIds = planIds;
                        subjectBll.Update(subjectModel);
                    }
                    tran.Complete();
                    return "ok";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

        }


        //List<int> Get

        string GetProvinceNames(int planId)
        {
            var list = planProvinceBll.GetList(p => p.PlanId == planId && p.PlanType == 1);
            if (list.Any())
            {
                StringBuilder province = new StringBuilder();
                list.ForEach(s =>
                {
                    province.Append(s.ProvinceName);
                    province.Append(',');

                });

                return province.ToString().TrimEnd(',');
            }
            return "";
        }

        string GetCityNames(int planId)
        {
            var list = planCityBll.GetList(p => p.PlanId == planId && p.PlanType == 1);
            if (list.Any())
            {
                StringBuilder city = new StringBuilder();
                list.ForEach(s =>
                {
                    city.Append(s.CityName);
                    city.Append(',');

                });

                return city.ToString().TrimEnd(',');
            }
            return "";
        }

        /// <summary>
        /// 获取客户的材质报价
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        decimal GetMaterialPrice(string material, int regionId)
        {
            //decimal mprice = (from price in CurrentContext.DbContext.CustomerMaterial
            //           join mType in CurrentContext.DbContext.MaterialType
            //           on price.MaterialTypeId equals mType.Id
            //           where price.CustomerId == customerId && price.RegionId == regionId
            //           && mType.MaterialTypeName == material
            //           select price.SalePrice).FirstOrDefault()??0;
            //return mprice;
            var model = new CustomerMaterialBLL().GetList(s => s.CustomerMaterialName == material).FirstOrDefault();
            if (model != null)
            {
                return model.SalePrice ?? 0;
            }
            return 0;
        }

        /// <summary>
        /// 获取客户材质的大类
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        string GetMaterialBigType(string region)
        {
            var list = from cm in CurrentContext.DbContext.CustomerMaterial
                       join mt in CurrentContext.DbContext.MaterialType
                       on cm.BigTypeId equals mt.Id
                       where cm.CustomerId == customerId && cm.RegionName == region
                       select new
                       {
                           MaterialTypeId = mt.Id,
                           mt.MaterialTypeName
                       };
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                List<int> types = new List<int>();
                list.ToList().ForEach(s =>
                {
                    if (!types.Contains(s.MaterialTypeId))
                    {
                        json.Append("{\"MaterialTypeId\":\"" + s.MaterialTypeId + "\",\"MaterialTypeName\":\"" + s.MaterialTypeName + "\"},");
                        types.Add(s.MaterialTypeId);
                    }

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        /// <summary>
        /// 获取客户材质
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="bigTypeId"></param>
        /// <returns></returns>
        string GetCustomerMaterial(string region)
        {
            ////var list = new CustomerMaterialBLL().GetList(s => s.CustomerId == customerId && s.RegionName == region && (s.IsDelete == false || s.IsDelete == null));
            //var list = new CustomerMaterialInfoBLL().GetList(s => s.CustomerId == customerId && s.Region == region && (s.IsDelete == false || s.IsDelete == null));
            //if (list.Any())
            //{
            //    StringBuilder json = new StringBuilder();
            //    list.ForEach(s =>
            //    {
            //        //json.Append("{\"MaterialId\":\"" + s.Id + "\",\"MaterialName\":\"" + s.CustomerMaterialName + "\",\"UnitPrice\":\"" + s.SalePrice + "\"},");
            //        json.Append("{\"MaterialName\":\"" + s.MaterialName + "\",\"Price\":\"" + s.Price + "\"},");
            //    });
            //    return "[" + json.ToString().TrimEnd(',') + "]";
            //}
            return "";
        }

        /// <summary>
        /// 检查客户材质是否正确
        /// </summary>
        /// <param name="material"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        string CheckCustomerMaterial(string material, string region)
        {
            List<string> regionList = new List<string>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',');
            }
            var list = new CustomerMaterialBLL().GetList(s => (regionList.Any() ? regionList.Contains(s.RegionName) : 1 == 1) && s.CustomerId == customerId && s.CustomerMaterialName == material);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"MaterialId\":\"" + s.Id + "\",\"MaterialName\":\"" + s.CustomerMaterialName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "error";
        }


        CustomerMaterialBLL customerMaterialBll = new CustomerMaterialBLL();
        bool GetMaterialPrice(string region, int customerId, int customerMaterialId, string materialName, out decimal price)
        {
            price = 0;
            List<string> regions = StringHelper.ToStringList(region, ',');
            var model = customerMaterialBll.GetList(s => s.CustomerId == customerId && regions.Contains(s.RegionName) && (customerMaterialId > 0 ? s.Id == customerMaterialId : s.CustomerMaterialName == materialName)).FirstOrDefault();
            if (model != null)
            {
                price = model.SalePrice ?? 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 按照所选条件获取pop订单里面的基础数据，生成方案内容
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        string GetPOPBaseInfo(string jsonStr)
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(jsonStr);
                if (planModel != null)
                {
                    var shopList = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                    join shop in CurrentContext.DbContext.Shop
                                    on order.ShopId equals shop.Id
                                    where order.SubjectId == planModel.SubjectId && order.GraphicNo != null && order.GraphicNo != ""

                                    select new
                                    {
                                        order,
                                        shop
                                    }).ToList();
                    List<string> regionList = new List<string>();
                    regionList = StringHelper.ToStringList(planModel.RegionNames, ',');

                    //筛选区域
                    if (regionList.Any())
                    {
                        shopList = shopList.Where(l => regionList.Contains(l.shop.RegionName)).ToList();
                    }
                    ////省份
                    //string provinces = GetProvinceNames(planModel.Id);
                    //if (!string.IsNullOrWhiteSpace(provinces))
                    //{
                    //    List<string> pList = StringHelper.ToStringList(provinces, ',');
                    //    shopList = shopList.Where(l => pList.Contains(l.shop.ProvinceName)).ToList();
                    //}
                    ////城市
                    //string cities = GetCityNames(s.Id);
                    //if (!string.IsNullOrWhiteSpace(cities))
                    //{
                    //    List<string> cList = StringHelper.ToStringList(cities, ',');
                    //    shopList = shopList.Where(l => cList.Contains(l.shop.CityName)).ToList();
                    //}
                    //城市级别
                    if (!string.IsNullOrWhiteSpace(planModel.CityTier))
                    {
                        List<string> CityTierList = StringHelper.ToStringList(planModel.CityTier, ',');
                        shopList = shopList.Where(l => CityTierList.Contains(l.shop.CityTier)).ToList();
                    }
                    //是否安装
                    if (!string.IsNullOrWhiteSpace(planModel.IsInstall))
                    {
                        List<string> installList = StringHelper.ToStringList(planModel.IsInstall, ',');
                        shopList = shopList.Where(l => installList.Contains(l.shop.IsInstall)).ToList();
                    }
                    //店铺类型
                    if (!string.IsNullOrWhiteSpace(planModel.Format))
                    {
                        List<string> FormatList = StringHelper.ToStringList(planModel.Format, ',');
                        shopList = shopList.Where(l => FormatList.Contains(l.shop.Format)).ToList();
                    }
                    //pop位置
                    if (!string.IsNullOrWhiteSpace(planModel.PositionName))
                    {
                        List<string> List = StringHelper.ToStringList(planModel.PositionName, ',');
                        shopList = shopList.Where(l => List.Contains(l.order.Sheet)).ToList();
                    }



                    //物料支持
                    if (!string.IsNullOrWhiteSpace(planModel.MaterialSupport))
                    {
                        List<string> list = StringHelper.ToStringList(planModel.MaterialSupport, ',');
                        shopList = shopList.Where(l => list.Contains(l.order.MaterialSupport)).ToList();
                    }
                    //店铺规模
                    if (!string.IsNullOrWhiteSpace(planModel.POSScale))
                    {
                        List<string> list = StringHelper.ToStringList(planModel.POSScale, ',');
                        shopList = shopList.Where(l => list.Contains(l.order.POSScale)).ToList();
                    }
                    //性别
                    if (!string.IsNullOrWhiteSpace(planModel.Gender))
                    {
                        List<string> list = StringHelper.ToStringList(planModel.Gender, ',');
                        shopList = shopList.Where(l => list.Contains(l.order.Gender)).ToList();
                    }
                    //器架类型
                    if (!string.IsNullOrWhiteSpace(planModel.MachineFrameNames))
                    {
                        List<string> list1 = StringHelper.ToStringList(planModel.MachineFrameNames, ',');
                        List<int> frameShopList = (from frame in CurrentContext.DbContext.ShopMachineFrame
                                                   where list1.Contains(frame.MachineFrame)
                                                   select frame.ShopId ?? 0).ToList();

                        shopList = shopList.Where(l => frameShopList.Contains(l.shop.Id)).ToList();
                    }
                    //数量
                    if (!string.IsNullOrWhiteSpace(planModel.Quantity))
                    {
                        List<int> Quantitylist = StringHelper.ToIntList(planModel.Quantity, ',');
                        shopList = shopList.Where(l => Quantitylist.Contains(l.order.Quantity ?? 0)).ToList();
                    }
                    //POP编号
                    if (!string.IsNullOrWhiteSpace(planModel.GraphicNo))
                    {
                        List<string> list = StringHelper.ToStringList(planModel.GraphicNo, ',');
                        shopList = shopList.Where(l => list.Contains(l.order.GraphicNo)).ToList();
                    }
                    //选图
                    if (!string.IsNullOrWhiteSpace(planModel.ChooseImg))
                    {
                        List<string> list = StringHelper.ToStringList(planModel.ChooseImg, ',');
                        shopList = shopList.Where(l => list.Contains(l.order.ChooseImg)).ToList();
                    }

                    if (shopList.Any())
                    {
                        List<string> info = new List<string>();
                        var list = (from order1 in shopList
                                    join pop in CurrentContext.DbContext.POP
                                    on new { order1.order.ShopId, order1.order.Sheet, order1.order.GraphicNo } equals new { pop.ShopId, pop.Sheet, pop.GraphicNo }
                                    select new
                                    {
                                        pop,
                                        order1
                                    }).ToList();
                        if (list.Any())
                        {

                            StringBuilder json = new StringBuilder();
                            list.ForEach(s =>
                            {
                                string key = s.pop.Sheet + "-" + s.pop.GraphicWidth + "-" + s.pop.GraphicLength + "-" + s.pop.GraphicMaterial;
                                if (!info.Contains(key))
                                {
                                    json.Append("{\"Sheet\":\"" + s.pop.Sheet + "\",\"Width\":\"" + s.pop.GraphicWidth + "\",\"Length\":\"" + s.pop.GraphicLength + "\",\"GraphicMaterial\":\"" + s.pop.GraphicMaterial + "\",\"Quantity\":\"" + s.order1.order.Quantity + "\",\"UnitPrice\":\"" + s.pop.UnitPrice + "\"},");
                                    info.Add(key);
                                }


                            });
                            result = "[" + json.ToString().TrimEnd(',') + "]";
                        }
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        /// 

        string ChangeConditions(string jsonStr)
        {
            string restult = string.Empty;
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {


                OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(jsonStr);
                if (planModel != null)
                {

                    

                    if (context1.Session["shoplist"] == null)
                    {
                        context1.Session["shoplist"] = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                                                        join shop in CurrentContext.DbContext.Shop
                                                        on merge.ShopId equals shop.Id
                                                        where merge.SubjectId == planModel.SubjectId
                                                        && (merge.IsDelete == null || merge.IsDelete == false)
                                                        select shop).ToList();
                        context1.Session["orderlist"] = new MergeOriginalOrderBLL().GetList(s => s.SubjectId == planModel.SubjectId && (s.IsDelete == null || s.IsDelete == false));
                        context1.Session["poplist"] = new POPBLL().GetList(s=>s.Id>0);

                    }
                    List<Shop> shops = new List<Shop>();
                    List<MergeOriginalOrder> orders = new List<MergeOriginalOrder>();
                    List<POP> pops = new List<POP>();
                    if (context1.Session["shoplist"] != null && context1.Session["orderlist"] != null && context1.Session["poplist"] != null)
                    {
                        shops = context1.Session["shoplist"] as List<Shop>;
                        orders = context1.Session["orderlist"] as List<MergeOriginalOrder>;
                        pops = context1.Session["poplist"] as List<POP>;
                    }
                    var shoplist = (from merge in orders
                                    join shop in shops
                                    on merge.ShopId equals shop.Id
                                    join pop1 in pops
                                    on new { merge.ShopId, merge.GraphicNo } equals new { pop1.ShopId, pop1.GraphicNo } into popTemp
                                    from pop in popTemp.DefaultIfEmpty()
                                    where merge.SubjectId == planModel.SubjectId
                                    && (merge.IsDelete == null || merge.IsDelete == false)
                                    select new
                                    {
                                        merge,
                                        shop,
                                        pop

                                    }).OrderBy(s => s.shop.CityTier).ToList();
                    //var shoplist = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                    //            join shop in CurrentContext.DbContext.Shop
                    //            on merge.ShopId equals shop.Id
                    //            join pop1 in CurrentContext.DbContext.POP
                    //            on new { merge.ShopId, merge.GraphicNo } equals new { pop1.ShopId, pop1.GraphicNo } into popTemp
                    //            from pop in popTemp.DefaultIfEmpty()
                    //            where merge.SubjectId == planModel.SubjectId
                    //            && (merge.IsDelete == null || merge.IsDelete == false)
                    //            select new
                    //            {
                    //                merge,
                    //                shop,
                    //                pop

                    //            }).OrderBy(s => s.shop.CityTier).ToList();
                    
                    if (!string.IsNullOrWhiteSpace(planModel.RegionNames))
                    {
                        List<string> regionList = StringHelper.ToStringList(planModel.RegionNames, ',');
                        shoplist = shoplist.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(planModel.ProvinceId))
                    {
                        List<string> provinceList = StringHelper.ToStringList(planModel.ProvinceId, ',');
                        shoplist = shoplist.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(planModel.CityId))
                    {
                        List<string> cityList = StringHelper.ToStringList(planModel.CityId, ',');
                        shoplist = shoplist.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    }
                    //位置
                    if (!string.IsNullOrWhiteSpace(planModel.PositionName))
                    {
                        //List<string> cityList = StringHelper.ToStringList(planModel.CityId, ',');
                        shoplist = shoplist.Where(s => s.merge.Sheet == planModel.PositionName).ToList();
                    }
                    //店铺编号
                    if (!string.IsNullOrWhiteSpace(planModel.ShopNos))
                    {
                        List<string> shopNos = StringHelper.ToStringList(planModel.ShopNos, ',');
                        shoplist = shoplist.Where(s => shopNos.Contains(s.shop.ShopNo)).ToList();
                    }


                    //角落类型
                    if (!string.IsNullOrWhiteSpace(planModel.CornerType))
                    {
                        if (planModel.CornerType.Contains("空"))
                        {
                            List<string> list0 = StringHelper.ToStringList(planModel.CornerType.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (list0.Any())
                            {
                                List<int> cornerShopList = frameBll.GetList(f => (list0.Contains(f.CornerType) || (f.CornerType == "" || f.CornerType == null)) && f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                shoplist = shoplist.Where(s => cornerShopList.Contains(s.shop.Id)).ToList();
                            }
                            else
                            {
                                List<int> cornerShopList = frameBll.GetList(f => (f.CornerType == "" || f.CornerType == null) && f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                shoplist = shoplist.Where(s => cornerShopList.Contains(s.shop.Id)).ToList();
                            }
                        }
                        else
                        {
                            List<string> cornerList = StringHelper.ToStringList(planModel.CornerType, ',');
                            List<int> cornerShopList = frameBll.GetList(f => cornerList.Contains(f.CornerType) && f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                            shoplist = shoplist.Where(s => cornerShopList.Contains(s.shop.Id)).ToList();
                        }
                    }
                    //器架类型
                    if (!string.IsNullOrWhiteSpace(planModel.MachineFrameNames))
                    {

                        if (planModel.MachineFrameNames.Contains("空"))
                        {
                            List<string> list0 = StringHelper.ToStringList(planModel.MachineFrameNames.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (list0.Any())//除了空，还有其他的器架
                            {

                                //所选择的器架类型的店铺
                                List<int> frameShopList = frameBll.GetList(f => f.PositionName == planModel.PositionName && list0.Contains(f.MachineFrame)).Select(f => f.ShopId ?? 0).ToList();
                                //当前位置的所有器架类型的店铺
                                List<int> frameShopList1 = frameBll.GetList(f => f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                //当前位置没有器架类型的店铺
                                List<int> shopListNoFrame = shoplist.Where(l => !frameShopList1.Contains(l.merge.ShopId ?? 0)).Select(l => l.merge.ShopId ?? 0).ToList();
                                frameShopList.AddRange(shopListNoFrame);
                                shoplist = shoplist.Where(l => frameShopList.Contains(l.merge.ShopId ?? 0)).ToList();
                                //frames = list0;
                            }
                            else//只有空
                            {
                                //当前位置的所有器架类型的店铺
                                List<int> frameShopList1 = frameBll.GetList(f => f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                shoplist = shoplist.Where(l => !frameShopList1.Contains(l.merge.ShopId ?? 0)).ToList();
                            }

                        }
                        else
                        {
                            List<string> frames = StringHelper.ToStringList(planModel.MachineFrameNames, ',');
                            var frameShopList = frameBll.GetList(f => f.PositionName == planModel.PositionName && frames.Contains(f.MachineFrame)).Select(f => f.ShopId ?? 0).ToList();
                            shoplist = shoplist.Where(l => frameShopList.Contains(l.merge.ShopId ?? 0)).ToList();
                        }
                    }
                    //性别
                    if (!string.IsNullOrWhiteSpace(planModel.Gender))
                    {
                        List<string> generList = StringHelper.ToStringList(planModel.Gender, ',');
                        shoplist = shoplist.Where(s => generList.Contains(s.merge.Gender)).ToList();
                    }
                    //城市级别
                    if (!string.IsNullOrWhiteSpace(planModel.CityTier))
                    {
                        List<string> cityTierList = StringHelper.ToStringList(planModel.CityTier, ',');
                        shoplist = shoplist.Where(s => cityTierList.Contains(s.shop.CityTier)).ToList();
                    }






                    //店铺级别
                    if (!string.IsNullOrWhiteSpace(planModel.Format))
                    {
                        List<string> formatList = StringHelper.ToStringList(planModel.Format, ',');
                        shoplist = shoplist.Where(s => formatList.Contains(s.shop.Format)).ToList();
                    }
                    //物料支持
                    if (!string.IsNullOrWhiteSpace(planModel.MaterialSupport))
                    {
                        List<string> supportList = StringHelper.ToStringListAllowSpace(planModel.MaterialSupport.Replace("空", ""), ',');
                        shoplist = shoplist.Where(s => supportList.Contains(s.merge.MaterialSupport)).ToList();
                    }
                    //店铺规模
                    if (!string.IsNullOrWhiteSpace(planModel.POSScale))
                    {
                        List<string> scaleList = StringHelper.ToStringListAllowSpace(planModel.POSScale.Replace("空", ""), ',');
                        shoplist = shoplist.Where(s => scaleList.Contains(s.merge.POSScale)).ToList();
                    }
                    //安装级别
                    if (!string.IsNullOrWhiteSpace(planModel.IsInstall))
                    {
                        //List<string> installList = StringHelper.ToStringList(planModel.IsInstall, ',');
                        //shoplist = shoplist.Where(s => installList.Contains(s.shop.IsInstall)).ToList();
                        if (planModel.IsInstall.Contains("空"))
                        {
                            List<string> installList0 = StringHelper.ToStringList(planModel.IsInstall.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (installList0.Any())
                                shoplist = shoplist.Where(l => installList0.Contains(l.shop.IsInstall) || ((l.shop.IsInstall == null || l.shop.IsInstall == ""))).ToList();
                            else
                                shoplist = shoplist.Where(l => (l.shop.IsInstall == null || l.shop.IsInstall == "")).ToList();
                        }
                        else
                        {
                            List<string> installList = StringHelper.ToStringList(planModel.IsInstall, ',');
                            shoplist = shoplist.Where(l => installList.Contains(l.shop.IsInstall)).ToList();
                        }
                    }
                    //数量
                    if (!string.IsNullOrWhiteSpace(planModel.Quantity))
                    {
                        List<int> quantityList = StringHelper.ToIntList(planModel.Quantity, ',');
                        shoplist = shoplist.Where(s => quantityList.Contains(s.merge.Quantity ?? 0)).ToList();
                    }
                    //材质
                    if (!string.IsNullOrWhiteSpace(planModel.GraphicMaterial))
                    {
                        List<string> materialList = StringHelper.ToStringList(planModel.GraphicMaterial, ',');
                        shoplist = shoplist.Where(s => s.pop != null && materialList.Contains(s.pop.GraphicMaterial)).ToList();
                    }
                    //pop尺寸
                    if (!string.IsNullOrWhiteSpace(planModel.POPSize))
                    {

                        if (planModel.POPSize.Contains("空"))
                        {
                            List<string> list0 = StringHelper.ToStringList(planModel.POPSize.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (list0.Any())
                            {
                                //shopList = shopList.Where(l => l.pop != null && (list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) && (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))).ToList();
                                shoplist = shoplist.Where(l => (l.pop != null && (list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) || (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))) || l.pop==null).ToList();

                            }
                            else
                            {
                                //shopList = shopList.Where(l => l.pop != null && l.pop.GraphicLength == null && l.pop.GraphicWidth == null).ToList();
                                shoplist = shoplist.Where(l => (l.pop != null && ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) || (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0))) || l.pop == null).ToList();

                            }

                        }
                        else
                        {
                            List<string> list = StringHelper.ToStringList(planModel.POPSize, ',');
                            shoplist = shoplist.Where(l => l.pop != null && list.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength)).ToList();
                        }
                    }
                    //Window尺寸
                    if (!string.IsNullOrWhiteSpace(planModel.WindowSize))
                    {

                        if (planModel.WindowSize.Contains("空"))
                        {
                            List<string> list0 = StringHelper.ToStringList(planModel.WindowSize.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (list0.Any())
                            {
                                //shopList = shopList.Where(l => l.pop != null && (list0.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep) || ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) && (l.pop.WindowWide == null || l.pop.WindowWide == 0) && (l.pop.WindowDeep == null || l.pop.WindowDeep == 0)))).ToList();
                                shoplist = shoplist.Where(l => (l.pop != null && (list0.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep) || ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) || (l.pop.WindowWide == null || l.pop.WindowWide == 0) || (l.pop.WindowDeep == null || l.pop.WindowDeep == 0)))) || l.pop==null).ToList();

                            }
                            else
                            {
                                //shopList = shopList.Where(l => l.pop != null && l.pop.WindowHigh == null && l.pop.WindowWide == null && l.pop.WindowDeep == null).ToList();
                                shoplist = shoplist.Where(l => (l.pop != null && ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) || (l.pop.WindowWide == null || l.pop.WindowWide == 0) || (l.pop.WindowDeep == null || l.pop.WindowDeep == 0))) || l.pop == null).ToList();

                            }
                        }
                        else
                        {
                            List<string> list = StringHelper.ToStringList(planModel.WindowSize, ',');
                            shoplist = shoplist.Where(l => l.pop != null && list.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep)).ToList();
                        }
                    }
                    //通电否
                    if (!string.IsNullOrWhiteSpace(planModel.IsElectricity))
                    {

                        if (planModel.IsElectricity.Contains("空"))
                        {
                            List<string> elcList0 = StringHelper.ToStringList(planModel.IsElectricity.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (elcList0.Any())
                            {
                                //shopList = shopList.Where(l => l.pop != null && (elcList0.Contains(l.pop.IsElectricity) || ((l.pop.IsElectricity == null || l.pop.IsElectricity == "")))).ToList();
                                shoplist = shoplist.Where(l => l.pop != null ? (elcList0.Contains(l.pop.IsElectricity) || ((l.pop.IsElectricity == null || l.pop.IsElectricity == ""))) : (1 == 1)).ToList();

                            }
                            else
                            {
                                //shopList = shopList.Where(l => l.pop != null && ((l.pop.IsElectricity == null || l.pop.IsElectricity == ""))).ToList();
                                shoplist = shoplist.Where(l => l.pop != null ? (l.pop.IsElectricity == null || l.pop.IsElectricity == "") : (1 == 1)).ToList();

                            }
                        }
                        else
                        {
                            List<string> elcList = StringHelper.ToStringList(planModel.IsElectricity, ',');
                            shoplist = shoplist.Where(l => l.pop != null && elcList.Contains(l.pop.IsElectricity)).ToList();
                        }
                    }
                    if (shoplist.Any())
                    {
                        StringBuilder json = new StringBuilder();
                        List<string> cityTier = new List<string>();
                        StringBuilder cityTiersb = new StringBuilder();
                        List<string> format = new List<string>();
                        StringBuilder formatsb = new StringBuilder();
                        List<string> install = new List<string>();
                        StringBuilder installsb = new StringBuilder();
                        List<string> materialSupport = new List<string>();
                        StringBuilder mssb = new StringBuilder();
                        List<string> POSScale = new List<string>();
                        StringBuilder scalesb = new StringBuilder();
                        List<string> sheet = new List<string>();
                        StringBuilder sheetsb = new StringBuilder();
                        List<string> gender = new List<string>();
                        StringBuilder gendersb = new StringBuilder();
                        List<int> count = new List<int>();
                        StringBuilder countsb = new StringBuilder();
                        List<string> material = new List<string>();
                        StringBuilder materialsb = new StringBuilder();
                        List<decimal> GraphicWidth = new List<decimal>();
                        StringBuilder GraphicWidthsb = new StringBuilder();
                        List<decimal> GraphicLength = new List<decimal>();
                        StringBuilder GraphicLengthsb = new StringBuilder();
                        List<decimal> WindowWide = new List<decimal>();
                        StringBuilder WindowWidesb = new StringBuilder();
                        List<decimal> WindowHigh = new List<decimal>();
                        StringBuilder WindowHighsb = new StringBuilder();
                        List<decimal> WindowDeep = new List<decimal>();
                        StringBuilder WindowDeepsb = new StringBuilder();
                        List<string> ChooseImg = new List<string>();
                        StringBuilder ChooseImgsb = new StringBuilder();
                        List<string> POPSize = new List<string>();
                        StringBuilder POPSizesb = new StringBuilder();
                        List<string> WindowSize = new List<string>();
                        StringBuilder WindowSizesb = new StringBuilder();
                        List<string> IsElectricity = new List<string>();
                        StringBuilder IsElectricitysb = new StringBuilder();
                        shoplist.ToList().ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s.shop.CityTier) && !cityTier.Contains(s.shop.CityTier))
                            {
                                cityTier.Add(s.shop.CityTier);
                                cityTiersb.Append(s.shop.CityTier);
                                cityTiersb.Append(',');
                            }
                            if (!string.IsNullOrWhiteSpace(s.shop.Format) && !format.Contains(s.shop.Format))
                            {
                                format.Add(s.shop.Format);
                                formatsb.Append(s.shop.Format);
                                formatsb.Append(',');
                            }
                            string materialSupport1 = !string.IsNullOrWhiteSpace(s.merge.MaterialSupport) ? s.merge.MaterialSupport : "空";
                            //if (!string.IsNullOrWhiteSpace(s.merge.MaterialSupport) && !materialSupport.Contains(s.merge.MaterialSupport))
                            if (!materialSupport.Contains(materialSupport1))
                            {
                                materialSupport.Add(materialSupport1);
                                mssb.Append(materialSupport1);
                                mssb.Append(',');
                            }
                            string isinstall = !string.IsNullOrWhiteSpace(s.shop.IsInstall) ? s.shop.IsInstall : "空";
                            if (!install.Contains(isinstall))
                            {
                                install.Add(isinstall);
                                installsb.Append(isinstall);
                                installsb.Append(',');
                            }
                            if (!string.IsNullOrWhiteSpace(s.merge.POSScale) && !POSScale.Contains(s.merge.POSScale))
                            {
                                POSScale.Add(s.merge.POSScale);
                                scalesb.Append(s.merge.POSScale);
                                scalesb.Append(',');
                            }
                            if (!string.IsNullOrWhiteSpace(s.merge.Sheet) && !sheet.Contains(s.merge.Sheet))
                            {
                                sheet.Add(s.merge.Sheet);
                                sheetsb.Append(s.merge.Sheet);
                                sheetsb.Append(',');
                            }
                            if (!string.IsNullOrWhiteSpace(s.merge.Gender) && !gender.Contains(s.merge.Gender))
                            {
                                gender.Add(s.merge.Gender);
                                gendersb.Append(s.merge.Gender);
                                gendersb.Append(',');
                            }
                            //
                            if (s.merge.Quantity != null && !count.Contains(s.merge.Quantity ?? 0))
                            {
                                count.Add(s.merge.Quantity ?? 0);

                            }
                            if (s.pop != null && !string.IsNullOrWhiteSpace(s.pop.GraphicMaterial) && !material.Contains(s.pop.GraphicMaterial))
                            {
                                material.Add(s.pop.GraphicMaterial);
                                materialsb.Append(s.pop.GraphicMaterial);
                                materialsb.Append(',');
                            }

                            string popsize = string.Empty;
                            if (s.pop != null)
                            {
                                if ((s.pop.GraphicWidth == null || s.pop.GraphicWidth == 0) || (s.pop.GraphicLength == null || s.pop.GraphicLength == 0))
                                {
                                    popsize = "空";
                                }
                                else
                                {
                                    popsize = s.pop.GraphicWidth + "*" + s.pop.GraphicLength;
                                }
                                if (!POPSize.Contains(popsize))
                                {
                                    POPSize.Add(popsize);
                                    POPSizesb.Append(popsize);
                                    POPSizesb.Append(',');
                                }
                            }
                            else
                            {
                                string popsize0 = "空";
                                if (!POPSize.Contains(popsize0))
                                {
                                    POPSize.Add(popsize0);
                                    POPSizesb.Append(popsize0);
                                    POPSizesb.Append(',');
                                }
                            }
                            string windowsize = string.Empty;
                            if (s.pop != null)
                            {
                                if ((s.pop.WindowWide == null || s.pop.WindowWide == 0) || (s.pop.WindowHigh == null || s.pop.WindowHigh == 0) || (s.pop.WindowDeep == null || s.pop.WindowDeep == 0))
                                {
                                    windowsize = "空";
                                }
                                else
                                {
                                    windowsize = s.pop.WindowWide + "*" + s.pop.WindowHigh + "*" + s.pop.WindowDeep;
                                }
                                if (!WindowSize.Contains(windowsize))
                                {
                                    WindowSize.Add(windowsize);
                                    WindowSizesb.Append(windowsize);
                                    WindowSizesb.Append(',');
                                }
                            }
                            else
                            {
                                string windowsize0 = "空";
                                if (!WindowSize.Contains(windowsize0))
                                {
                                    WindowSize.Add(windowsize0);
                                    WindowSizesb.Append(windowsize0);
                                    WindowSizesb.Append(',');
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(s.merge.ChooseImg) && !ChooseImg.Contains(s.merge.ChooseImg))
                            {
                                ChooseImg.Add(s.merge.ChooseImg);
                                ChooseImgsb.Append(s.merge.ChooseImg);
                                ChooseImgsb.Append(',');
                            }

                            //通电否
                            if (s.pop != null)
                            {
                                string electricity = string.IsNullOrWhiteSpace(s.pop.IsElectricity) ? "空" : s.pop.IsElectricity;
                                if (!IsElectricity.Contains(electricity))
                                {
                                    IsElectricity.Add(electricity);
                                    IsElectricitysb.Append(electricity);
                                    IsElectricitysb.Append(',');
                                }
                            }


                        });
                        count.Sort();
                        count.ForEach(s =>
                        {
                            countsb.Append(s);
                            countsb.Append(',');
                        });

                        json.Append("{\"CityTier\":\"" + cityTiersb.ToString().TrimEnd(',') + "\",\"Format\":\"" + formatsb.ToString().TrimEnd(',') + "\",\"MaterialSupport\":\"" + mssb.ToString().TrimEnd(',') + "\",\"IsInstall\":\"" + installsb.ToString().TrimEnd(',') + "\",\"POSScale\":\"" + scalesb.ToString().TrimEnd(',') + "\",\"Sheet\":\"" + sheetsb.ToString().TrimEnd(',') + "\",\"Gender\":\"" + gendersb.ToString().TrimEnd(',') + "\",");

                        json.Append("\"GraphicMaterial\":\"" + materialsb.ToString().TrimEnd(',') + "\",\"Quantity\":\"" + countsb.ToString().TrimEnd(',') + "\",\"POPSize\":\"" + POPSizesb.ToString().TrimEnd(',') + "\",\"ChooseImg\":\"" + ChooseImgsb.ToString().TrimEnd(',') + "\",\"IsElectricity\":\"" + IsElectricitysb.ToString().TrimEnd(',') + "\",\"WindowSize\":\"" + WindowSizesb.ToString().TrimEnd(',') + "\"}");
                        restult = "[" + json.ToString() + "]";
                    }
                    else
                    {
                        StringBuilder json1 = new StringBuilder();
                        json1.Append("{\"CityTier\":\"\",\"Format\":\"\",\"MaterialSupport\":\"\",\"IsInstall\":\"\",\"POSScale\":\"\",\"Sheet\":\"\",\"Gender\":\"\",");

                        json1.Append("\"GraphicMaterial\":\"\",\"Quantity\":\"\",\"POPSize\":\"\",\"ChooseImg\":\"\",\"IsElectricity\":\"\",\"WindowSize\":\"\"}");
                        restult = "[" + json1.ToString() + "]";
                    }
                }

            }
            return restult;
        }


        /// <summary>
        /// 检查拆单方案是否满足所有订单数据
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        string CheckPlanData(int subjectId)
        {
            var planList = new OrderPlanBLL().GetList(s => s.SubjectId == subjectId && s.PlanType == 1);
            string result = "ok";
            if (planList.Any())
            {
                var listOrder = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 join pop1 in CurrentContext.DbContext.POP
                                 on new { shopid = order.ShopId, graphicNo = order.GraphicNo } equals new { shopid = pop1.ShopId, graphicNo = pop1.GraphicNo } into popTemp
                                 from pop in popTemp.DefaultIfEmpty()
                                 where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                                 select new
                                 {
                                     order,
                                     shop,
                                     pop
                                 }).ToList();
                if (listOrder.Any())
                {
                    List<int> finishSplitId = new List<int>();
                    int SplitCount = 0;
                    List<string> CRegionList = new List<string>();
                    List<string> CProvinceList = new List<string>();
                    List<string> CCityList = new List<string>();
                    List<string> CCityTierList = new List<string>();

                    List<string> CIsInstallList = new List<string>();
                    List<string> CFormatList = new List<string>();
                    List<string> CPositionList = new List<string>();
                    List<string> CMaterialSupportList = new List<string>();

                    List<string> CPOSScaleList = new List<string>();
                    List<string> CGenderList = new List<string>();
                    List<int> CQuantityList = new List<int>();
                    List<string> CGraphicMaterialList = new List<string>();
                    List<string> CPOPSizeList = new List<string>();

                    List<string> CWindowSizeList = new List<string>();
                    List<string> CChooseImgList = new List<string>();
                    List<string> CCornerTypeList = new List<string>();
                    List<string> CMachineFrameList = new List<string>();

                    planList.ForEach(s =>
                         {
                             var shopList = listOrder.Where(l => !finishSplitId.Contains(l.order.Id)).ToList();
                             List<string> regionList = new List<string>();
                             regionList = StringHelper.ToStringList(s.RegionNames, ',');

                             //筛选区域
                             if (regionList.Any())
                             {
                                 shopList = shopList.Where(l => regionList.Contains(l.shop.RegionName)).ToList();
                                 regionList.ForEach(r =>
                                 {
                                     if (!CRegionList.Contains(r))
                                     {
                                         CRegionList.Add(r);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CRegionList.Contains("无"))
                                 {
                                     CRegionList.Add("无");
                                 }
                             }
                             //省份
                             string provinces = GetProvinceNames(s.Id);
                             if (!string.IsNullOrWhiteSpace(provinces))
                             {
                                 List<string> pList = StringHelper.ToStringList(provinces, ',');
                                 shopList = shopList.Where(l => pList.Contains(l.shop.ProvinceName)).ToList();
                                 pList.ForEach(p =>
                                 {
                                     if (!CProvinceList.Contains(p))
                                     {
                                         CProvinceList.Add(p);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CProvinceList.Contains("无"))
                                 {
                                     CProvinceList.Add("无");
                                 }
                             }
                             //城市
                             string cities = GetCityNames(s.Id);
                             if (!string.IsNullOrWhiteSpace(cities))
                             {
                                 List<string> cList = StringHelper.ToStringList(cities, ',');
                                 shopList = shopList.Where(l => cList.Contains(l.shop.CityName)).ToList();
                                 cList.ForEach(c =>
                                 {
                                     if (!CCityList.Contains(c))
                                     {
                                         CCityList.Add(c);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CCityList.Contains("无"))
                                 {
                                     CCityList.Add("无");
                                 }
                             }
                             //城市级别
                             if (!string.IsNullOrWhiteSpace(s.CityTier))
                             {
                                 List<string> CityTierList = StringHelper.ToStringList(s.CityTier, ',');
                                 shopList = shopList.Where(l => CityTierList.Contains(l.shop.CityTier)).ToList();
                                 CityTierList.ForEach(c =>
                                 {
                                     if (!CCityTierList.Contains(c))
                                     {
                                         CCityTierList.Add(c);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CCityTierList.Contains("无"))
                                 {
                                     CCityTierList.Add("无");
                                 }
                             }
                             //是否安装
                             if (!string.IsNullOrWhiteSpace(s.IsInstall))
                             {
                                 List<string> installList = StringHelper.ToStringList(s.IsInstall, ',');
                                 shopList = shopList.Where(l => installList.Contains(l.shop.IsInstall)).ToList();
                                 installList.ForEach(i =>
                                 {
                                     if (!CIsInstallList.Contains(i))
                                     {
                                         CIsInstallList.Add(i);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CIsInstallList.Contains("无"))
                                 {
                                     CIsInstallList.Add("无");
                                 }
                             }
                             //店铺类型
                             if (!string.IsNullOrWhiteSpace(s.Format))
                             {
                                 List<string> FormatList = StringHelper.ToStringList(s.Format, ',');
                                 shopList = shopList.Where(l => FormatList.Contains(l.shop.Format)).ToList();
                                 FormatList.ForEach(f =>
                                 {
                                     if (!CFormatList.Contains(f))
                                     {
                                         CFormatList.Add(f);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CFormatList.Contains("无"))
                                 {
                                     CFormatList.Add("无");
                                 }
                             }
                             //pop位置
                             if (!string.IsNullOrWhiteSpace(s.PositionName))
                             {
                                 List<string> List = StringHelper.ToStringList(s.PositionName, ',');
                                 shopList = shopList.Where(l => List.Contains(l.order.Sheet)).ToList();
                                 List.ForEach(p =>
                                 {
                                     if (!CPositionList.Contains(p))
                                     {
                                         CPositionList.Add(p);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CPositionList.Contains("无"))
                                 {
                                     CPositionList.Add("无");
                                 }
                             }
                             //物料支持
                             if (!string.IsNullOrWhiteSpace(s.MaterialSupport))
                             {

                                 if (s.MaterialSupport.Contains("空"))
                                 {
                                     List<string> scaleList0 = StringHelper.ToStringList(s.MaterialSupport.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                     if (scaleList0.Any())
                                         shopList = shopList.Where(l => scaleList0.Contains(l.order.MaterialSupport) || ((l.order.MaterialSupport == null || l.order.MaterialSupport == ""))).ToList();
                                     else
                                         shopList = shopList.Where(l => (l.order.MaterialSupport == null || l.order.MaterialSupport == "")).ToList();

                                 }
                                 else
                                 {
                                     List<string> scaleList = StringHelper.ToStringList(s.MaterialSupport, ',');
                                     shopList = shopList.Where(l => scaleList.Contains(l.order.MaterialSupport)).ToList();
                                 }
                                 List<string> List = StringHelper.ToStringList(s.MaterialSupport, ',');
                                 List.ForEach(m =>
                                 {
                                     if (!CMaterialSupportList.Contains(m))
                                     {
                                         CMaterialSupportList.Add(m);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CMaterialSupportList.Contains("无"))
                                 {
                                     CMaterialSupportList.Add("无");
                                 }
                             }
                             //店铺规模
                             if (!string.IsNullOrWhiteSpace(s.POSScale))
                             {

                                 if (s.POSScale.Contains("空"))
                                 {
                                     List<string> scaleList0 = StringHelper.ToStringList(s.POSScale.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                     if (scaleList0.Any())
                                         shopList = shopList.Where(l => scaleList0.Contains(l.order.POSScale) || ((l.order.POSScale == null || l.order.POSScale == ""))).ToList();
                                     else
                                         shopList = shopList.Where(l => (l.order.POSScale == null || l.order.POSScale == "")).ToList();

                                 }
                                 else
                                 {
                                     List<string> scaleList = StringHelper.ToStringList(s.POSScale, ',');
                                     shopList = shopList.Where(l => scaleList.Contains(l.order.POSScale)).ToList();
                                 }
                                 List<string> List = StringHelper.ToStringList(s.POSScale, ',');
                                 List.ForEach(sc =>
                                 {
                                     if (!CPOSScaleList.Contains(sc))
                                     {
                                         CPOSScaleList.Add(sc);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CPOSScaleList.Contains("无"))
                                 {
                                     CPOSScaleList.Add("无");
                                 }
                             }
                             //性别
                             if (!string.IsNullOrWhiteSpace(s.Gender))
                             {
                                 List<string> list = StringHelper.ToStringList(s.Gender, ',');
                                 shopList = shopList.Where(l => list.Contains(l.order.Gender)).ToList();
                                 list.ForEach(g =>
                                 {
                                     if (!CGenderList.Contains(g))
                                     {
                                         CGenderList.Add(g);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CGenderList.Contains("无"))
                                 {
                                     CGenderList.Add("无");
                                 }
                             }
                             //数量
                             if (!string.IsNullOrWhiteSpace(s.Quantity))
                             {
                                 List<int> Quantitylist = StringHelper.ToIntList(s.Quantity, ',');
                                 shopList = shopList.Where(l => Quantitylist.Contains(l.order.Quantity ?? 0)).ToList();
                                 Quantitylist.ForEach(q =>
                                 {
                                     if (!CQuantityList.Contains(q))
                                     {
                                         CQuantityList.Add(q);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CQuantityList.Contains(0))
                                 {
                                     CQuantityList.Add(0);
                                 }
                             }
                             //POP编号
                             if (!string.IsNullOrWhiteSpace(s.GraphicNo))
                             {
                                 List<string> list = StringHelper.ToStringList(s.GraphicNo, ',');
                                 shopList = shopList.Where(l => list.Contains(l.order.GraphicNo)).ToList();
                             }
                             //pop材质
                             if (!string.IsNullOrWhiteSpace(s.GraphicMaterial))
                             {
                                 List<string> list = StringHelper.ToStringList(s.GraphicMaterial, ',');
                                 shopList = shopList.Where(l => l.pop != null && list.Contains(l.pop.GraphicMaterial)).ToList();
                                 list.ForEach(g =>
                                 {
                                     if (!CGraphicMaterialList.Contains(g))
                                     {
                                         CGraphicMaterialList.Add(g);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CGraphicMaterialList.Contains("无"))
                                 {
                                     CGraphicMaterialList.Add("无");
                                 }
                             }
                             //POP尺寸
                             if (!string.IsNullOrWhiteSpace(s.POPSize))
                             {
                                 if (s.POPSize.Contains("空"))
                                 {
                                     List<string> list0 = StringHelper.ToStringList(s.POPSize.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                     if (list0.Any())
                                     {
                                         //shopList = shopList.Where(l => l.pop != null && (list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) && (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))).ToList();
                                         shopList = shopList.Where(l => l.pop != null ? ((list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) && (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))) : (1 == 1)).ToList();

                                     }
                                     else
                                     {
                                         //shopList = shopList.Where(l => l.pop != null && l.pop.GraphicLength == null && l.pop.GraphicWidth == null).ToList();
                                         shopList = shopList.Where(l => l.pop != null ? (l.pop.GraphicLength == null && l.pop.GraphicWidth == null) : (1 == 1)).ToList();

                                     }

                                 }
                                 else
                                 {
                                     List<string> list = StringHelper.ToStringList(s.POPSize, ',');
                                     shopList = shopList.Where(l => l.pop != null && list.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength)).ToList();
                                 }
                                 List<string> list1 = StringHelper.ToStringList(s.POPSize, ',');
                                 list1.ForEach(p =>
                                 {
                                     if (!CPOPSizeList.Contains(p))
                                     {
                                         CPOPSizeList.Add(p);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CPOPSizeList.Contains("无"))
                                 {
                                     CPOPSizeList.Add("无");
                                 }
                             }
                             //Window尺寸
                             if (!string.IsNullOrWhiteSpace(s.WindowSize))
                             {
                                 if (s.WindowSize.Contains("空"))
                                 {
                                     List<string> list0 = StringHelper.ToStringList(s.WindowSize.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                     if (list0.Any())
                                     {
                                         //shopList = shopList.Where(l => l.pop != null && (list0.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep) || ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) && (l.pop.WindowWide == null || l.pop.WindowWide == 0) && (l.pop.WindowDeep == null || l.pop.WindowDeep == 0)))).ToList();
                                         shopList = shopList.Where(l => l.pop != null ? ((list0.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep) || ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) && (l.pop.WindowWide == null || l.pop.WindowWide == 0) && (l.pop.WindowDeep == null || l.pop.WindowDeep == 0)))) : (1 == 1)).ToList();

                                     }
                                     else
                                     {
                                         //shopList = shopList.Where(l => l.pop != null && l.pop.WindowHigh == null && l.pop.WindowWide == null && l.pop.WindowDeep == null).ToList();
                                         shopList = shopList.Where(l => l.pop != null ? (l.pop.WindowHigh == null && l.pop.WindowWide == null && l.pop.WindowDeep == null) : (1 == 1)).ToList();

                                     }
                                 }
                                 else
                                 {
                                     List<string> list = StringHelper.ToStringList(s.WindowSize, ',');
                                     shopList = shopList.Where(l => l.pop != null && list.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep)).ToList();
                                 }
                                 List<string> list1 = StringHelper.ToStringList(s.WindowSize, ',');
                                 list1.ForEach(w =>
                                 {
                                     if (!CWindowSizeList.Contains(w))
                                     {
                                         CWindowSizeList.Add(w);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CWindowSizeList.Contains("无"))
                                 {
                                     CWindowSizeList.Add("无");
                                 }
                             }


                             //如果是橱窗，判断是否通电 
                             if (!string.IsNullOrWhiteSpace(s.PositionName) && s.PositionName == "橱窗" && !string.IsNullOrWhiteSpace(s.IsElectricity))
                             {
                                 if (s.IsElectricity.Contains("空"))
                                 {
                                     List<string> elcList0 = StringHelper.ToStringList(s.IsElectricity.Replace(",空", "").Replace("空", ""), ',');
                                     if (elcList0.Any())
                                     {
                                         //shopList = shopList.Where(l => l.pop != null && (elcList0.Contains(l.pop.IsElectricity) || ((l.pop.IsElectricity == null || l.pop.IsElectricity == "")))).ToList();
                                         shopList = shopList.Where(l => l.pop != null ? ((elcList0.Contains(l.pop.IsElectricity) || ((l.pop.IsElectricity == null || l.pop.IsElectricity == "")))) : (1 == 1)).ToList();

                                     }
                                     else
                                     {
                                         //shopList = shopList.Where(l => l.pop != null && ((l.pop.IsElectricity == null || l.pop.IsElectricity == ""))).ToList();
                                         shopList = shopList.Where(l => l.pop != null ? (((l.pop.IsElectricity == null || l.pop.IsElectricity == ""))) : (1 == 1)).ToList();

                                     }
                                 }
                                 else
                                 {
                                     List<string> elcList = StringHelper.ToStringList(s.IsElectricity, ',');
                                     shopList = shopList.Where(l => l.pop != null && elcList.Contains(l.pop.IsElectricity)).ToList();
                                 }

                             }

                             //选图
                             if (!string.IsNullOrWhiteSpace(s.ChooseImg))
                             {
                                 List<string> list = StringHelper.ToStringList(s.ChooseImg, ',');
                                 shopList = shopList.Where(l => list.Contains(l.order.ChooseImg)).ToList();
                                 list.ForEach(c =>
                                 {
                                     if (!CChooseImgList.Contains(c))
                                     {
                                         CChooseImgList.Add(c);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CChooseImgList.Contains("无"))
                                 {
                                     CChooseImgList.Add("无");
                                 }
                             }
                             //角落类型
                             if (!string.IsNullOrWhiteSpace(s.CornerType))
                             {

                                 if (s.CornerType.Contains("空"))
                                 {
                                     List<string> list0 = StringHelper.ToStringList(s.CornerType.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                     if (list0.Any())
                                     {
                                         List<int> cornerShopList = frameBll.GetList(f => (list0.Contains(f.CornerType) || (f.CornerType == "" || f.CornerType == null)) && f.PositionName == s.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                         shopList = shopList.Where(l => cornerShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                     }
                                     else
                                     {
                                         List<int> cornerShopList = frameBll.GetList(f => (f.CornerType == "" || f.CornerType == null) && f.PositionName == s.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                         shopList = shopList.Where(l => cornerShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                     }
                                 }
                                 else
                                 {
                                     List<string> cornerList = StringHelper.ToStringList(s.CornerType, ',');
                                     List<int> cornerShopList = frameBll.GetList(f => cornerList.Contains(f.CornerType) && f.PositionName == s.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                     shopList = shopList.Where(l => cornerShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                 }
                                 List<string> list = StringHelper.ToStringList(s.CornerType, ',');
                                 list.ForEach(c =>
                                 {
                                     if (!CCornerTypeList.Contains(c))
                                     {
                                         CCornerTypeList.Add(c);
                                     }
                                 });
                             }
                             else
                             {
                                 if (!CCornerTypeList.Contains("无"))
                                 {
                                     CCornerTypeList.Add("无");
                                 }
                             }
                             //器架
                             if (!string.IsNullOrWhiteSpace(s.MachineFrameNames))
                             {
                                 string frameNew = s.MachineFrameNames.Replace(",空", "").Replace("空,", "").Replace("空", "");
                                 List<string> frames = StringHelper.ToStringList(frameNew, ',');
                                 if (frames.Any())
                                 {
                                     var frameShopList = frameBll.GetList(f => f.PositionName == s.PositionName && frames.Contains(f.MachineFrame)).Select(f => f.ShopId ?? 0).ToList();
                                     shopList = shopList.Where(l => frameShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                     if (shopList.Any())
                                     {
                                         shopList.ForEach(o =>
                                         {

                                             var frameList = frameBll.GetList(f => f.ShopId == o.shop.Id && f.PositionName == o.order.Sheet && frames.Contains(f.MachineFrame) && f.Gender == o.order.Gender).FirstOrDefault();

                                             if (frameList != null)
                                             {

                                                 finishSplitId.Add(o.order.Id);

                                             }
                                         });
                                     }
                                     frames.ForEach(f =>
                                     {
                                         if (!CMachineFrameList.Contains(f))
                                         {
                                             CMachineFrameList.Add(f);
                                         }
                                     });
                                 }

                             }
                             else
                             {
                                 if (!CMachineFrameList.Contains("无"))
                                 {
                                     CMachineFrameList.Add("无");
                                 }
                                 if (shopList.Any())
                                 {
                                     shopList.ForEach(o =>
                                     {
                                         finishSplitId.Add(o.order.Id);

                                     });
                                 }
                             }

                         });
                    //不符合条件的订单
                    var shopList1 = listOrder.Where(l => !finishSplitId.Contains(l.order.Id)).ToList();

                    if (shopList1.Any())
                    {

                        CreateExportTB();
                        foreach (var item in shopList1)
                        {
                            //var list = listOrder.Where(l => l.shop.Id == item.shop.Id);
                            if (CRegionList.Any())
                            {
                                //var list1 = listOrder.Where(l => CRegionList.Contains(l.shop.RegionName) && l.shop.Id==item.shop.Id);
                                if (!CRegionList.Contains(item.shop.RegionName))
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CRegionList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该店铺不包含区域：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CProvinceList.Any())
                            {
                                //var list1 = listOrder.Where(l => CProvinceList.Contains(l.shop.ProvinceName) && l.shop.Id == item.shop.Id);
                                bool flag = true;
                                if (CProvinceList.Contains("无"))
                                {

                                }
                                else
                                {
                                    flag = CProvinceList.Contains(item.shop.ProvinceName);
                                }
                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CProvinceList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该店铺不包含省份：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CCityList.Any())
                            {
                                //var list1 = listOrder.Where(l => CCityList.Contains(l.shop.CityName) && l.shop.Id == item.shop.Id);
                                bool flag = true;
                                if (CCityList.Contains("无"))
                                {

                                }
                                else
                                {
                                    flag = CCityList.Contains(item.shop.CityName);
                                }
                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CCityList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该店铺不包含城市：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CCityTierList.Any())
                            {
                                //var list1 = listOrder.Where(l => CCityTierList.Contains(l.shop.CityTier) && l.shop.Id == item.shop.Id);
                                bool flag = true;
                                if (CCityTierList.Contains("无"))
                                {

                                }
                                else
                                {
                                    flag = CCityTierList.Contains(item.shop.CityTier);
                                }
                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CCityTierList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该店铺不包含城市基本：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CIsInstallList.Any())
                            {
                                //var list1 = listOrder.Where(l => CIsInstallList.Contains(l.shop.IsInstall) && l.shop.Id == item.shop.Id);
                                bool flag = true;
                                if (CIsInstallList.Contains("无"))
                                {

                                }
                                else
                                {
                                    flag = CIsInstallList.Contains(item.shop.IsInstall);
                                }
                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CIsInstallList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该店铺不包含安装级别：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CFormatList.Any())
                            {
                                //var list1 = listOrder.Where(l => CFormatList.Contains(l.shop.Format) && l.shop.Id == item.shop.Id);
                                bool flag = true;
                                if (CFormatList.Contains("无"))
                                {

                                }
                                else
                                {
                                    flag = CFormatList.Contains(item.shop.Format);
                                }
                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CFormatList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该店铺不包含Format：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CPositionList.Any())
                            {
                                //string sheet1 = item.order.Sheet;
                                //var list1 = listOrder.Where(l => CPositionList.Contains(l.order.Sheet) && l.shop.Id == item.shop.Id);

                                if (!CPositionList.Contains(item.order.Sheet))
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CPositionList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该条订单不包含位置：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CMaterialSupportList.Any())
                            {
                                //var list1 = listOrder.Where(l => CMaterialSupportList.Contains(l.order.MaterialSupport) && l.shop.Id == item.shop.Id);
                                bool flag = true;
                                if (CMaterialSupportList.Contains("无"))
                                {

                                }
                                else
                                {
                                    flag = CMaterialSupportList.Contains(item.order.MaterialSupport);
                                }

                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CMaterialSupportList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该条订单不包含物料支持：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CPOSScaleList.Any())
                            {
                                //var list1 = listOrder.Where(l => CPOSScaleList.Contains(l.order.POSScale) && l.shop.Id == item.shop.Id);
                                bool flag = true;
                                if (CPOSScaleList.Contains("无"))
                                {

                                }
                                else
                                {
                                    flag = CPOSScaleList.Contains(item.order.POSScale);
                                }
                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CPOSScaleList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该条订单不包含店铺规模大小：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CChooseImgList.Any())
                            {
                                //var list1 = listOrder.Where(l => CChooseImgList.Contains(l.order.ChooseImg) && l.shop.Id == item.shop.Id);
                                bool flag = true;
                                if (CChooseImgList.Contains("无"))
                                {

                                }
                                else
                                {
                                    flag = CChooseImgList.Contains(item.order.ChooseImg);
                                }
                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CChooseImgList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该条订单不包含选图：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CCornerTypeList.Any())
                            {
                                //list = list.Where(l => CPOSScaleList.Contains(l.order.POSScale));
                                bool flag = true;
                                if (CCornerTypeList.Contains("无"))
                                {

                                }
                                else
                                {
                                    if (CCornerTypeList.Contains("空"))
                                    {
                                        CCornerTypeList.Remove("空");
                                        if (CCornerTypeList.Any())
                                        {
                                            var frameList1 = frameBll.GetList(f => f.CornerType != null && f.CornerType != "" && f.ShopId == item.shop.Id).Select(f => f.CornerType).ToList();
                                            flag = frameList1.Where(f => CCornerTypeList.Contains(f)).Any();
                                        }
                                        else
                                        {
                                            flag = frameBll.GetList(f => (f.CornerType != null || f.CornerType != "") && f.ShopId == item.shop.Id).Any();
                                        }
                                    }
                                    else
                                    {
                                        var cornerShopList = frameBll.GetList(f => CCornerTypeList.Contains(f.CornerType) && f.ShopId == item.shop.Id);
                                        flag = cornerShopList.Any();
                                    }
                                }

                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CCornerTypeList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该店铺不包含角落类型：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }
                            if (CMachineFrameList.Any())
                            {
                                //list = list.Where(l => CPOSScaleList.Contains(l.order.POSScale));

                                bool flag = true;
                                if (CMachineFrameList.Contains("无"))
                                {

                                }
                                else
                                {
                                    var ShopList = frameBll.GetList(f => CMachineFrameList.Contains(f.MachineFrame) && f.ShopId == item.shop.Id);
                                    flag = ShopList.Any();

                                }
                                if (flag == false)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    CMachineFrameList.ForEach(s =>
                                    {
                                        sb.Append(s + ",");
                                    });
                                    DataRow dr = exportTB.NewRow();
                                    dr["POS Code"] = item.shop.ShopNo;
                                    dr["POS Name"] = item.shop.ShopName;
                                    dr["物料支持"] = item.order.MaterialSupport;
                                    dr["POS Type"] = item.order.POSScale;
                                    dr["Region"] = item.shop.RegionName;
                                    dr["Province"] = item.shop.ProvinceName;
                                    dr["City"] = item.shop.CityName;
                                    dr["City Tier"] = item.shop.CityTier;
                                    dr["Customer Code"] = item.shop.AgentCode;

                                    dr["Customer Name"] = item.shop.AgentName;
                                    dr["Channel"] = item.shop.Channel;
                                    dr["Format"] = item.shop.Format;
                                    dr["Location Type"] = item.shop.LocationType;
                                    dr["Business Model"] = item.shop.BusinessModel;
                                    dr["POPAddress"] = item.shop.POPAddress;
                                    dr["Contact1"] = item.shop.Contact1;
                                    dr["Tel1"] = item.shop.Tel1;
                                    dr["Contact2"] = item.shop.Contact2;
                                    dr["Tel2"] = item.shop.Tel2;
                                    dr["Opening Date"] = item.shop.OpeningDate != null ? DateTime.Parse(item.shop.OpeningDate.ToString()).ToShortDateString() : "";
                                    dr["Status"] = item.shop.Status;
                                    dr["Sheet"] = item.order.Sheet;
                                    dr["M/W"] = item.order.Gender;
                                    dr["Quantity"] = item.order.Quantity != null ? item.order.Quantity.ToString() : "";
                                    dr["Graphic No"] = item.order.GraphicNo;
                                    dr["Corner Type"] = item.pop != null ? item.pop.CornerType : "";
                                    dr["Category"] = item.pop != null ? item.pop.Category : "";
                                    dr["Standard Dimension"] = item.pop != null ? item.pop.StandardDimension : "";
                                    dr["Modula"] = item.pop != null ? item.pop.Modula : "";
                                    dr["Window Wide(mm)"] = item.pop != null && item.pop.WindowWide != null ? item.pop.WindowWide.ToString() : "";
                                    dr["Window High(mm)"] = item.pop != null && item.pop.WindowHigh != null ? item.pop.WindowHigh.ToString() : "";
                                    dr["Window Deep(mm)"] = item.pop != null && item.pop.WindowDeep != null ? item.pop.WindowDeep.ToString() : "";
                                    dr["Window Size"] = item.pop != null ? item.pop.WindowSize : "";
                                    dr["Graphic Width(mm)"] = item.pop != null && item.pop.GraphicWidth != null ? item.pop.GraphicWidth.ToString() : "";
                                    dr["Graphic Length(mm)"] = item.pop != null && item.pop.GraphicLength != null ? item.pop.GraphicLength.ToString() : "";
                                    dr["Graphic Material"] = item.pop != null ? item.pop.GraphicMaterial : "";
                                    dr["Position Description"] = item.pop != null ? item.pop.PositionDescription : "";
                                    dr["选图"] = item.order.ChooseImg;
                                    dr["备注"] = "不符合拆单条件";
                                    dr["不符合条件内容"] = "该店铺不包含器架类型：" + sb.ToString().TrimEnd(',');
                                    exportTB.Rows.Add(dr);
                                    SplitCount++;
                                    continue;
                                }
                            }


                        }
                        if (exportTB.Rows.Count > 0)
                        {
                            result = exportTB.Rows.Count.ToString();
                            context1.Session["exportTb"] = exportTB;

                        }
                    }
                }
            }
            return result;
        }

        DataTable exportTB;
        void CreateExportTB()
        {
            exportTB = new DataTable();
            exportTB.Columns.AddRange(new DataColumn[]{
               new DataColumn("POS Code", Type.GetType("System.String")), 
               new DataColumn("POS Name", Type.GetType("System.String")),
               new DataColumn("物料支持", Type.GetType("System.String")),
               new DataColumn("POS Type", Type.GetType("System.String")),
               new DataColumn("Region", Type.GetType("System.String")),
               new DataColumn("Province", Type.GetType("System.String")),
               new DataColumn("City", Type.GetType("System.String")),
               new DataColumn("City Tier", Type.GetType("System.String")),
               new DataColumn("Customer Code", Type.GetType("System.String")),
               new DataColumn("Customer Name", Type.GetType("System.String")),
               new DataColumn("Channel", Type.GetType("System.String")),
               new DataColumn("Format", Type.GetType("System.String")),
               new DataColumn("Location Type", Type.GetType("System.String")),
               new DataColumn("Business Model", Type.GetType("System.String")),
               new DataColumn("POPAddress", Type.GetType("System.String")),
               new DataColumn("Contact1", Type.GetType("System.String")),
               new DataColumn("Tel1", Type.GetType("System.String")),
               new DataColumn("Contact2", Type.GetType("System.String")),
               new DataColumn("Tel2", Type.GetType("System.String")),
               new DataColumn("Opening Date", Type.GetType("System.String")),
               new DataColumn("Status", Type.GetType("System.String")),
               new DataColumn("Sheet", Type.GetType("System.String")),
               new DataColumn("M/W", Type.GetType("System.String")),
               new DataColumn("Quantity", Type.GetType("System.String")),
               new DataColumn("Graphic No", Type.GetType("System.String")),
               //new DataColumn("Graphic Name", Type.GetType("System.String")),
               //new DataColumn("Graphic Type", Type.GetType("System.String")),
               new DataColumn("Corner Type", Type.GetType("System.String")),
               new DataColumn("Category", Type.GetType("System.String")),
               new DataColumn("Standard Dimension", Type.GetType("System.String")),
               new DataColumn("Modula", Type.GetType("System.String")),
               new DataColumn("Window Wide(mm)", Type.GetType("System.String")),
               new DataColumn("Window High(mm)", Type.GetType("System.String")),
               new DataColumn("Window Deep(mm)", Type.GetType("System.String")),
               new DataColumn("Window Size", Type.GetType("System.String")),
               new DataColumn("Graphic Width(mm)", Type.GetType("System.String")),
               new DataColumn("Graphic Length(mm)", Type.GetType("System.String")),
               //new DataColumn("Size(M2)", Type.GetType("System.String")),
               //new DataColumn("Double-Face", Type.GetType("System.String")),
               new DataColumn("Graphic Material", Type.GetType("System.String")),
               //new DataColumn("Glass", Type.GetType("System.String")),
               //new DataColumn("Backdrop", Type.GetType("System.String")),
               new DataColumn("Position Description", Type.GetType("System.String")),
               //new DataColumn("Platform Length(mm)", Type.GetType("System.String")),
               //new DataColumn("Platform Width(mm)", Type.GetType("System.String")),
               //new DataColumn("Platform Height(mm)", Type.GetType("System.String")),
               new DataColumn("选图", Type.GetType("System.String")),
               new DataColumn("备注", Type.GetType("System.String")),
               new DataColumn("不符合条件内容", Type.GetType("System.String"))
              }
            );
        }

        string GetShops(string jsonStr)
        {
            string restult = string.Empty;
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(jsonStr);
                if (planModel != null)
                {
                    SplitOrderPlanDetail splitModel = new SplitOrderPlanDetail();
                    if (planModel.SplitOrderPlanDetail.Any())
                    {
                        splitModel = planModel.SplitOrderPlanDetail.FirstOrDefault();
                    }
                    var shoplist = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                                    join shop in CurrentContext.DbContext.Shop
                                    on merge.ShopId equals shop.Id
                                    join pop1 in CurrentContext.DbContext.POP
                                    on new { merge.ShopId, merge.GraphicNo } equals new { pop1.ShopId, pop1.GraphicNo } into popTemp
                                    from pop in popTemp.DefaultIfEmpty()
                                    where merge.SubjectId == planModel.SubjectId
                                    && (merge.IsDelete == null || merge.IsDelete == false)
                                    select new
                                    {
                                        merge,
                                        shop,
                                        pop

                                    }).OrderBy(s => s.shop.Id).ToList();


                    //位置
                    if (!string.IsNullOrWhiteSpace(planModel.PositionName))
                    {
                        //List<string> cityList = StringHelper.ToStringList(planModel.CityId, ',');
                        shoplist = shoplist.Where(s => s.merge.Sheet == planModel.PositionName).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(planModel.RegionNames))
                    {
                        List<string> regionList = StringHelper.ToStringList(planModel.RegionNames, ',');
                        shoplist = shoplist.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(planModel.ProvinceId))
                    {
                        List<string> provinceList = StringHelper.ToStringList(planModel.ProvinceId, ',');
                        shoplist = shoplist.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(planModel.CityId))
                    {
                        List<string> cityList = StringHelper.ToStringList(planModel.CityId, ',');
                        shoplist = shoplist.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    }
                    //角落类型
                    if (!string.IsNullOrWhiteSpace(planModel.CornerType))
                    {
                        if (planModel.CornerType.Contains("空"))
                        {
                            List<string> list0 = StringHelper.ToStringList(planModel.CornerType.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (list0.Any())
                            {
                                List<int> cornerShopList = frameBll.GetList(f => (list0.Contains(f.CornerType) || (f.CornerType == "" || f.CornerType == null)) && f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                shoplist = shoplist.Where(s => cornerShopList.Contains(s.shop.Id)).ToList();
                            }
                            else
                            {
                                List<int> cornerShopList = frameBll.GetList(f => (f.CornerType == "" || f.CornerType == null) && f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                shoplist = shoplist.Where(s => cornerShopList.Contains(s.shop.Id)).ToList();
                            }
                        }
                        else
                        {
                            List<string> cornerList = StringHelper.ToStringList(planModel.CornerType, ',');
                            List<int> cornerShopList = frameBll.GetList(f => cornerList.Contains(f.CornerType) && f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                            shoplist = shoplist.Where(s => cornerShopList.Contains(s.shop.Id)).ToList();
                        }
                    }
                    //器架类型
                    if (!string.IsNullOrWhiteSpace(planModel.MachineFrameNames))
                    {
                        if (planModel.MachineFrameNames.Contains("空"))
                        {
                            List<string> list0 = StringHelper.ToStringList(planModel.MachineFrameNames.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (!list0.Any())
                            {
                                //List<string> frameList = StringHelper.ToStringList(planModel.MachineFrameNames, ',');
                                List<int> frameShopList = frameBll.GetList(f => f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                shoplist = shoplist.Where(s => !frameShopList.Contains(s.shop.Id)).ToList();
                            }
                        }
                        else
                        {
                            List<string> frameList = StringHelper.ToStringList(planModel.MachineFrameNames, ',');
                            List<int> frameShopList = frameBll.GetList(f => frameList.Contains(f.MachineFrame) && f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                            shoplist = shoplist.Where(s => frameShopList.Contains(s.shop.Id)).ToList();
                        }
                    }
                    //性别
                    if (!string.IsNullOrWhiteSpace(planModel.Gender))
                    {
                        List<string> generList = StringHelper.ToStringList(planModel.Gender, ',');
                        shoplist = shoplist.Where(s => generList.Contains(s.merge.Gender)).ToList();
                    }
                    //城市级别
                    if (!string.IsNullOrWhiteSpace(planModel.CityTier))
                    {
                        List<string> cityTierList = StringHelper.ToStringList(planModel.CityTier, ',');
                        shoplist = shoplist.Where(s => cityTierList.Contains(s.shop.CityTier)).ToList();
                    }
                    //店铺级别
                    if (!string.IsNullOrWhiteSpace(planModel.Format))
                    {
                        List<string> formatList = StringHelper.ToStringList(planModel.Format, ',');
                        shoplist = shoplist.Where(s => formatList.Contains(s.shop.Format)).ToList();
                    }
                    //物料支持
                    if (!string.IsNullOrWhiteSpace(planModel.MaterialSupport))
                    {
                        List<string> supportList = StringHelper.ToStringListAllowSpace(planModel.MaterialSupport.Replace("空", ""), ',');
                        shoplist = shoplist.Where(s => supportList.Contains(s.merge.MaterialSupport)).ToList();
                    }
                    //店铺规模
                    if (!string.IsNullOrWhiteSpace(planModel.POSScale))
                    {
                        List<string> scaleList = StringHelper.ToStringListAllowSpace(planModel.POSScale.Replace("空", ""), ',');
                        shoplist = shoplist.Where(s => scaleList.Contains(s.merge.POSScale)).ToList();
                    }
                    //安装级别
                    if (!string.IsNullOrWhiteSpace(planModel.IsInstall))
                    {
                        List<string> installList = StringHelper.ToStringList(planModel.IsInstall, ',');
                        shoplist = shoplist.Where(s => installList.Contains(s.shop.IsInstall)).ToList();
                    }
                    //数量
                    if (!string.IsNullOrWhiteSpace(planModel.Quantity))
                    {
                        List<int> quantityList = StringHelper.ToIntList(planModel.Quantity, ',');
                        shoplist = shoplist.Where(s => quantityList.Contains(s.merge.Quantity ?? 0)).ToList();
                    }
                    //材质
                    if (!string.IsNullOrWhiteSpace(planModel.GraphicMaterial))
                    {
                        List<string> materialList = StringHelper.ToStringList(planModel.GraphicMaterial, ',');
                        shoplist = shoplist.Where(s => s.pop != null && materialList.Contains(s.pop.GraphicMaterial)).ToList();
                    }
                    //pop尺寸
                    if (!string.IsNullOrWhiteSpace(planModel.POPSize))
                    {
                        if (planModel.POPSize.Contains("空"))
                        {
                            List<string> list0 = StringHelper.ToStringList(planModel.POPSize.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (list0.Any())
                                shoplist = shoplist.Where(l => l.pop != null && (list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) && (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))).ToList();
                            else
                                shoplist = shoplist.Where(l => l.pop != null && l.pop.GraphicLength == null && l.pop.GraphicWidth == null).ToList();

                        }
                        else
                        {
                            List<string> list = StringHelper.ToStringList(planModel.POPSize, ',');
                            shoplist = shoplist.Where(l => l.pop != null && list.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength)).ToList();
                        }

                    }
                    //Window尺寸
                    if (!string.IsNullOrWhiteSpace(planModel.WindowSize))
                    {
                        if (planModel.WindowSize.Contains("空"))
                        {
                            List<string> list0 = StringHelper.ToStringList(planModel.WindowSize.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (list0.Any())
                                shoplist = shoplist.Where(l => l.pop != null && (list0.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep) || ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) && (l.pop.WindowWide == null || l.pop.WindowWide == 0) && (l.pop.WindowDeep == null || l.pop.WindowDeep == 0)))).ToList();
                            else
                                shoplist = shoplist.Where(l => l.pop != null && l.pop.WindowHigh == null && l.pop.WindowWide == null && l.pop.WindowDeep == null).ToList();

                        }
                        else
                        {
                            List<string> list = StringHelper.ToStringList(planModel.WindowSize, ',');
                            shoplist = shoplist.Where(l => l.pop != null && list.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep)).ToList();
                        }
                    }
                    //通电否
                    if (!string.IsNullOrWhiteSpace(planModel.IsElectricity))
                    {
                        if (planModel.IsElectricity.Contains("空"))
                        {
                            List<string> elcList0 = StringHelper.ToStringList(planModel.IsElectricity.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                            if (elcList0.Any())
                                shoplist = shoplist.Where(l => l.pop != null && (elcList0.Contains(l.pop.IsElectricity) || ((l.pop.IsElectricity == null || l.pop.IsElectricity == "")))).ToList();
                            else
                                shoplist = shoplist.Where(l => l.pop != null && ((l.pop.IsElectricity == null || l.pop.IsElectricity == ""))).ToList();

                        }
                        else
                        {
                            List<string> elcList = StringHelper.ToStringList(planModel.IsElectricity, ',');
                            shoplist = shoplist.Where(l => l.pop != null && elcList.Contains(l.pop.IsElectricity)).ToList();
                        }
                    }
                    //选图
                    if (!string.IsNullOrWhiteSpace(planModel.ChooseImg))
                    {
                        List<string> ChooseImgList = StringHelper.ToStringList(planModel.ChooseImg, ',');
                        shoplist = shoplist.Where(s => ChooseImgList.Contains(s.merge.ChooseImg)).ToList();
                    }
                    if (shoplist.Any())
                    {
                        StringBuilder json = new StringBuilder();
                        var newList = (from shop in shoplist
                                       join per in CurrentContext.DbContext.SplitOrderDetailPerShop
                                       on new { shop.merge.ShopId, shop.merge.Sheet, shop.merge.Gender, shop.merge.SubjectId, splitModel.Remark } equals new { per.ShopId, per.Sheet, per.Gender, per.SubjectId, per.Remark } into perTemp

                                       //on new {shop.merge.Id, splitModel.Remark } equals new {per.OrderId,per.Remark } into perTemp
                                       from per1 in perTemp.DefaultIfEmpty()

                                       select new
                                       {
                                           shop,
                                           per1
                                       });
                        newList.ToList().ForEach(sh =>
                        {
                            int Quantity = sh.per1 != null ? (sh.per1.Quantity ?? 0) : (splitModel.Quantity ?? 0);
                            decimal GraphicWidth = sh.per1 != null ? (sh.per1.GraphicWidth ?? 0) : 0;
                            decimal GraphicLength = sh.per1 != null ? (sh.per1.GraphicLength ?? 0) : 0;
                            string GraphicMaterial = (sh.per1 != null && (string.IsNullOrWhiteSpace(sh.per1.GraphicMaterial) == false)) ? sh.per1.GraphicMaterial : splitModel.GraphicMaterial;
                            string orderType = splitModel.OrderType == 2 ? "道具" : "pop";
                            string chooseImg = string.IsNullOrWhiteSpace(splitModel.NewChooseImg) == false ? splitModel.NewChooseImg : sh.shop.merge.ChooseImg;
                            json.Append("{\"OrderId\":\"" + sh.shop.merge.Id + "\",\"OrderType\":\"" + orderType + "\",\"ShopId\":\"" + sh.shop.shop.Id + "\",\"ShopName\":\"" + sh.shop.shop.ShopName + "\",\"ShopNo\":\"" + sh.shop.shop.ShopNo + "\",\"Sheet\":\"" + planModel.PositionName + "\",\"Gender\":\"" + sh.shop.merge.Gender + "\",\"Quantity\":\"" + Quantity + "\",\"GraphicWidth\":\"" + GraphicWidth + "\",\"GraphicLength\":\"" + GraphicLength + "\",\"Remark\":\"" + splitModel.Remark + "\",\"GraphicMaterial\":\"" + GraphicMaterial + "\",\"NewChooseImg\":\"" + chooseImg + "\",\"MachineFrame\":\"" + planModel.MachineFrameNames + "\"},");

                        });
                        restult = "[" + json.ToString().TrimEnd(',') + "]";
                    }
                }
            }
            return restult;
        }


        string AddPerShopInfo(string jsonStr)
        {
            string result = "提交失败!";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        List<SplitOrderDetailPerShop> list = JsonConvert.DeserializeObject<List<SplitOrderDetailPerShop>>(jsonStr);
                        if (list.Any())
                        {

                            int subjectId = list[0].SubjectId ?? 0;
                            string sheet = list[0].Sheet;
                            string remark = list[0].Remark;
                            SplitOrderDetailPerShopBLL bll = new SplitOrderDetailPerShopBLL();
                            bll.Delete(s => s.SubjectId == subjectId && s.Sheet == sheet && s.Remark == remark);
                            //
                            list.ForEach(s =>
                            {
                                SplitOrderDetailPerShop model = new SplitOrderDetailPerShop() { ShopId = s.ShopId, SubjectId = subjectId, GraphicLength = s.GraphicLength, GraphicWidth = s.GraphicWidth, Quantity = s.Quantity, Remark = remark, Sheet = sheet, Gender = s.Gender, GraphicMaterial = s.GraphicMaterial, OrderType = s.OrderType, OrderId = s.OrderId, MachineFrame = s.MachineFrame, NewChooseImg = s.NewChooseImg };
                                bll.Add(model);
                            });
                        }
                        tran.Complete();
                        result = "ok";
                    }
                    catch (Exception ex)
                    {
                        result = "提交失败：" + ex.Message;
                    }
                }

            }
            return result;
        }


        string AddHCPOP(string jsonStr)
        {
            string restult = string.Empty;
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(jsonStr);
                if (planModel != null)
                {
                    string frameName = planModel.MachineFrameNames;
                    if (!string.IsNullOrWhiteSpace(frameName))
                    {
                        string gender = planModel.Gender;
                        List<string> genderList = new List<string>();
                        if (!string.IsNullOrWhiteSpace(gender))
                        {
                            genderList = StringHelper.ToStringList(gender, ',');

                        }
                        var list = new HCPOPBLL().GetList(s => s.MachineFrame == frameName && (genderList.Any() ? (genderList.Contains(s.MachineFrameGender)) : (1 == 1)));
                        if (list.Any())
                        {
                            List<string> popList = new List<string>();
                            StringBuilder json = new StringBuilder();
                            list.ForEach(s =>
                            {
                                string pop = s.POP + "-" + s.POPGender;
                                if (!popList.Contains(pop))
                                {
                                    popList.Add(pop);
                                    string poptype = (s.POPType != null && s.POPType.ToString() == "2") ? "道具" : "pop";
                                    json.Append("{\"POPType\":\"" + poptype + "\",\"POP\":\"" + s.POP + "\",\"POPGender\":\"" + s.POPGender + "\"},");

                                }
                            });
                            restult = "[" + json.ToString().TrimEnd(',') + "]";
                        }
                    }
                }
            }
            return restult;
        }

        List<int> GetShopId(string shopNos,out string notExistShopNo)
        {
            notExistShopNo = string.Empty;
            List<int> ids = new List<int>();
            List<string> shopNoList = StringHelper.ToStringList(shopNos,',').Distinct().ToList();
            ShopBLL shopBll = new ShopBLL();
            StringBuilder str=new StringBuilder();
            shopNoList.ForEach(s => {
                var model = shopBll.GetList(l=>l.ShopNo==s).FirstOrDefault();
                if (model != null)
                    ids.Add(model.Id);
                else
                {
                    str.Append(s);
                    str.Append(",");
                }
            });
            notExistShopNo = str.ToString();
            return ids;
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


/// <summary>
/// 橱窗类
/// </summary>
public class WindowClass
{
    public string Name { get; set; }
    public int OrderType { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }


}
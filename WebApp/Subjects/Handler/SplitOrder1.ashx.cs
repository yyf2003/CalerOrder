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
    /// SplitOrder1 的摘要说明
    /// </summary>
    public class SplitOrder1 : IHttpHandler, IRequiresSessionState
    {
        HttpContext context1;
        ShopMachineFrameBLL frameBll = new ShopMachineFrameBLL();
        int planId;
        int customerId;
        int subjectId;
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
            if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            if (context.Request.QueryString["planId"] != null)
            {
                planId = int.Parse(context.Request.QueryString["planId"]);
            }
            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            if (context.Request.QueryString["SubjectId"] != null)
            {
                subjectId = int.Parse(context.Request.QueryString["SubjectId"]);
            }
            switch (type)
            {
                case "getCondition":
                    //获取店铺类型
                    int subjectId = 0;
                    if (context.Request.QueryString["subjectId"] != null)
                    {
                        subjectId = int.Parse(context.Request.QueryString["subjectId"]);
                    }
                    string shopNos = string.Empty;
                    if (context.Request.QueryString["shopNos"] != null)
                    {
                        shopNos = context.Request.QueryString["shopNos"];
                    }
                    result = GetConditions(subjectId, shopNos);
                    break;
                case "changeCondition":
                    string str = context.Request.Form["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        str = HttpUtility.UrlDecode(str);
                    }
                    result = ChangeConditions(str);
                    break;
                case "getCustomerMaterial":
                    //string region1 = context.Request.QueryString["region"];
                    //int customerId = int.Parse(context.Request.QueryString["customerId"]);
                    result = GetCustomerMaterial();
                    break;
                case "add":
                    string optype = context.Request.Form["optype"];
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
                case "deletePlan":
                    result = DeletePlan();
                    break;
                case "splitOrder":
                    //开始拆单
                    string planIds = context.Request.QueryString["planIds"];
                    result = GoSplitOrder(planIds);
                    break;
                case "getEmptyFrameShopCount":
                    int subjectId0 = 0;
                    string sheet = string.Empty;
                    if (context.Request.QueryString["subjectId"] != null)
                        subjectId0 = int.Parse(context.Request.QueryString["subjectId"]);
                    if (context.Request.QueryString["sheet"] != null)
                        sheet = context.Request.QueryString["sheet"];
                    result = GetEmptyFrameShopCount(subjectId0, sheet);
                    break;
                case "getTableSize":
                    string sheetName = string.Empty;
                    if (context.Request.QueryString["sheet"] != null)
                    {
                        sheetName = context.Request.QueryString["sheet"];
                    }
                    string frameName = string.Empty;
                    if (context.Request.QueryString["frameName"] != null)
                    {
                        frameName = context.Request.QueryString["frameName"];
                    }
                    result = GetTableSize(sheetName, frameName);
                    break;
            }
            context.Response.Write(result);
        }
        /// <summary>
        /// 首次加载获取所有条件信息
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="shopNos"></param>
        /// <returns></returns>
        string GetConditions(int subjectId, string shopNos = null)
        {

            StringBuilder json = new StringBuilder();

            List<string> sheet = new List<string>();
            StringBuilder sheetsb = new StringBuilder();

            List<string> region = new List<string>();
            StringBuilder regionsb = new StringBuilder();

            List<string> gender = new List<string>();
            StringBuilder gendersb = new StringBuilder();

            List<string> cityTier = new List<string>();
            StringBuilder cityTiersb = new StringBuilder();

            List<string> format = new List<string>();
            StringBuilder formatsb = new StringBuilder();

            //List<string> shopLevel = new List<string>();
            //StringBuilder shopLevelsb = new StringBuilder();

            List<string> materialSupport = new List<string>();
            StringBuilder mssb = new StringBuilder();

            List<string> POSScale = new List<string>();
            StringBuilder scalesb = new StringBuilder();

            List<string> install = new List<string>();
            StringBuilder installsb = new StringBuilder();

            List<int> count = new List<int>();
            StringBuilder countsb = new StringBuilder();

            List<string> material = new List<string>();
            StringBuilder materialsb = new StringBuilder();

            List<string> POPSize = new List<string>();
            StringBuilder POPSizesb = new StringBuilder();

            List<string> WindowSize = new List<string>();
            StringBuilder WindowSizesb = new StringBuilder();

            List<string> IsElectricity = new List<string>();
            StringBuilder IsElectricitysb = new StringBuilder();

            List<string> ChooseImg = new List<string>();
            StringBuilder ChooseImgsb = new StringBuilder();



            var shoplist = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                            join shop in CurrentContext.DbContext.Shop
                            on merge.ShopId equals shop.Id
                            join pop1 in CurrentContext.DbContext.POP
                            on new { merge.ShopId, merge.GraphicNo } equals new { pop1.ShopId, pop1.GraphicNo } into popTemp
                            from pop in popTemp.DefaultIfEmpty()
                            where merge.SubjectId == subjectId && (merge.IsDelete == null || merge.IsDelete == false)
                            select new
                            {
                                merge,
                                shop,
                                pop

                            }).OrderBy(s => s.shop.CityTier).ToList();
            List<int> newShopIds = new List<int>();
            bool hasShopNo = false;
            string notExist = string.Empty;
            if (!string.IsNullOrWhiteSpace(shopNos))
            {
                shopNos = shopNos.Replace("，", ",").TrimEnd(',');

                newShopIds = GetShopId(shopNos, out notExist);
                hasShopNo = true;
            }
            if (hasShopNo)
            {
                if (!string.IsNullOrWhiteSpace(notExist))
                {
                    return "店铺编号：" + notExist + " 不存在！";
                }
                shoplist = shoplist.Where(s => newShopIds.Contains(s.shop.Id)).ToList();
            }
            if (shoplist.Any())
            {

                shoplist.ToList().ForEach(s =>
                {

                    if (!string.IsNullOrWhiteSpace(s.merge.Sheet) && !sheet.Contains(s.merge.Sheet))
                    {
                        sheet.Add(s.merge.Sheet);
                        sheetsb.Append(s.merge.Sheet);
                        sheetsb.Append(',');
                    }

                    if (!string.IsNullOrWhiteSpace(s.shop.RegionName) && !region.Contains(s.shop.RegionName))
                    {
                        region.Add(s.shop.RegionName);
                        regionsb.Append(s.shop.RegionName);
                        regionsb.Append(',');
                    }

                    if (!string.IsNullOrWhiteSpace(s.merge.Gender) && !gender.Contains(s.merge.Gender))
                    {
                        gender.Add(s.merge.Gender);
                        //gendersb.Append(s.merge.Gender);
                        //gendersb.Append(',');
                    }


                    if (!string.IsNullOrWhiteSpace(s.shop.CityTier) && !cityTier.Contains(s.shop.CityTier))
                    {
                        cityTier.Add(s.shop.CityTier);
                        //cityTiersb.Append(s.shop.CityTier);
                        //cityTiersb.Append(',');
                    }

                    if (!string.IsNullOrWhiteSpace(s.shop.Format) && !format.Contains(s.shop.Format))
                    {
                        format.Add(s.shop.Format);
                        //formatsb.Append(s.shop.Format);
                        //formatsb.Append(',');
                    }

                    //if (!string.IsNullOrWhiteSpace(s.shop.POSScale) && !shopLevel.Contains(s.shop.POSScale))
                    //{
                    //    shopLevel.Add(s.shop.POSScale);
                    //}

                    string support = !string.IsNullOrWhiteSpace(s.merge.MaterialSupport) ? s.merge.MaterialSupport : "空";
                    if (!materialSupport.Contains(support))
                    {
                        materialSupport.Add(support);
                        //mssb.Append(support);
                        //mssb.Append(',');
                    }

                    string scale = !string.IsNullOrWhiteSpace(s.merge.POSScale) ? s.merge.POSScale : "空";

                    if (!POSScale.Contains(scale))
                    {
                        POSScale.Add(scale);
                        //scalesb.Append(scale);
                        //scalesb.Append(',');
                    }


                    string isinstall = !string.IsNullOrWhiteSpace(s.shop.IsInstall) ? s.shop.IsInstall : "空";
                    if (!install.Contains(isinstall))
                    {
                        install.Add(isinstall);
                        //installsb.Append(isinstall);
                        //installsb.Append(',');
                    }

                    if (s.merge.Quantity != null && !count.Contains(s.merge.Quantity ?? 0))
                    {
                        count.Add(s.merge.Quantity ?? 0);

                    }
                    if (s.pop != null && !string.IsNullOrWhiteSpace(s.pop.GraphicMaterial) && !material.Contains(s.pop.GraphicMaterial))
                    {
                        material.Add(s.pop.GraphicMaterial);
                        //materialsb.Append(s.pop.GraphicMaterial);
                        //materialsb.Append(',');
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
                            //POPSizesb.Append(popsize);
                            //POPSizesb.Append(',');
                        }
                    }
                    else
                    {
                        string popsize0 = "空";
                        if (!POPSize.Contains(popsize0))
                        {
                            POPSize.Add(popsize0);
                            //POPSizesb.Append(popsize0);
                            //POPSizesb.Append(',');
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
                            //WindowSizesb.Append(windowsize);
                            //WindowSizesb.Append(',');
                        }
                    }
                    else
                    {
                        string windowsize0 = "空";
                        if (!WindowSize.Contains(windowsize0))
                        {
                            WindowSize.Add(windowsize0);
                            //WindowSizesb.Append(windowsize0);
                            //WindowSizesb.Append(',');
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

            gender.Sort();
            gender.ForEach(s =>
            {
                gendersb.Append(s);
                gendersb.Append(',');
            });
            cityTier.Sort();
            cityTier.ForEach(s =>
            {
                cityTiersb.Append(s);
                cityTiersb.Append(',');
            });
            format.Sort();
            format.ForEach(s =>
            {
                formatsb.Append(s);
                formatsb.Append(',');
            });
            //shopLevel.Sort();
            //shopLevel.ForEach(s =>
            //{
            //    shopLevelsb.Append(s);
            //    shopLevelsb.Append(',');
            //});
            materialSupport.Sort();
            materialSupport.ForEach(s =>
            {
                mssb.Append(s);
                mssb.Append(',');
            });
            install.Sort();
            install.ForEach(s =>
            {
                installsb.Append(s);
                installsb.Append(',');
            });
            POSScale.Sort();
            POSScale.ForEach(s =>
            {
                scalesb.Append(s);
                scalesb.Append(',');
            });

            material.Sort();
            material.ForEach(s =>
            {
                materialsb.Append(s);
                materialsb.Append(',');
            });
            count.Sort();
            count.ForEach(s =>
            {
                countsb.Append(s);
                countsb.Append(',');
            });
            POPSize.Sort();
            POPSize.ForEach(s =>
            {
                POPSizesb.Append(s);
                POPSizesb.Append(',');
            });
            WindowSize.Sort();
            WindowSize.ForEach(s =>
            {
                WindowSizesb.Append(s);
                WindowSizesb.Append(',');
            });
            json.Append("{\"RegionNames\":\"" + regionsb.ToString().TrimEnd(',') + "\",\"CityTier\":\"" + cityTiersb.ToString().TrimEnd(',') + "\",\"Format\":\"" + formatsb.ToString().TrimEnd(',') + "\",\"MaterialSupport\":\"" + mssb.ToString().TrimEnd(',') + "\",\"IsInstall\":\"" + installsb.ToString().TrimEnd(',') + "\",\"POSScale\":\"" + scalesb.ToString().TrimEnd(',') + "\",\"Sheet\":\"" + sheetsb.ToString().TrimEnd(',') + "\",\"Gender\":\"" + gendersb.ToString().TrimEnd(',') + "\",");

            json.Append("\"GraphicMaterial\":\"" + materialsb.ToString().TrimEnd(',') + "\",\"Quantity\":\"" + countsb.ToString().TrimEnd(',') + "\",\"POPSize\":\"" + POPSizesb.ToString().TrimEnd(',') + "\",\"ChooseImg\":\"" + ChooseImgsb.ToString().TrimEnd(',') + "\",\"IsElectricity\":\"" + IsElectricitysb.ToString().TrimEnd(',') + "\",\"WindowSize\":\"" + WindowSizesb.ToString().TrimEnd(',') + "\"}");

            return "[" + json.ToString() + "]";
        }
        /// <summary>
        /// 选择条件变更是，更新所有信息
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        string ChangeConditions(string jsonStr)
        {
            string restult = string.Empty;
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {


                OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(jsonStr);
                if (planModel != null)
                {
                    string subjectCategory = string.Empty;
                    string subjectCornerType = string.Empty;
                    if ((planModel.SubjectId ?? 0) > 0)
                    {
                        var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                            join category1 in CurrentContext.DbContext.ADSubjectCategory
                                            on subject.SubjectCategoryId equals category1.Id into categoryTemp
                                            from category in categoryTemp.DefaultIfEmpty()
                                            where subject.Id == (planModel.SubjectId ?? 0)
                                            select new { subject, category.CategoryName }).FirstOrDefault();
                        if (subjectModel != null)
                        {
                            subjectCategory = subjectModel.CategoryName;
                            subjectCornerType = subjectModel.subject.CornerType;
                        }
                    }


                    if (context1.Session["shoplist"] == null)
                    {
                        var shopList0 = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                                         join shop in CurrentContext.DbContext.Shop
                                         on merge.ShopId equals shop.Id
                                         where merge.SubjectId == planModel.SubjectId
                                         && (merge.IsDelete == null || merge.IsDelete == false)
                                         select shop).ToList();
                        var shopids = shopList0.Select(s => s.Id).Distinct().ToList();
                        context1.Session["shoplist"] = shopList0;
                        context1.Session["orderlist"] = new MergeOriginalOrderBLL().GetList(s => s.SubjectId == planModel.SubjectId && (s.IsDelete == null || s.IsDelete == false));
                        context1.Session["poplist"] = new POPBLL().GetList(s => shopids.Contains(s.ShopId ?? 0));
                        context1.Session["frames"] = frameBll.GetList(s => shopids.Contains(s.ShopId ?? 0));

                    }
                    List<Shop> shops = new List<Shop>();
                    List<MergeOriginalOrder> orders = new List<MergeOriginalOrder>();
                    List<POP> pops = new List<POP>();
                    List<ShopMachineFrame> frameLists = new List<ShopMachineFrame>();
                    if (context1.Session["shoplist"] != null && context1.Session["orderlist"] != null && context1.Session["poplist"] != null && context1.Session["frames"] != null)
                    {
                        shops = context1.Session["shoplist"] as List<Shop>;
                        orders = context1.Session["orderlist"] as List<MergeOriginalOrder>;
                        pops = context1.Session["poplist"] as List<POP>;
                        frameLists = context1.Session["frames"] as List<ShopMachineFrame>;
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

                    //店铺编号
                    if (!string.IsNullOrWhiteSpace(planModel.ShopNos))
                    {
                        List<string> shopNos = StringHelper.ToStringList(planModel.ShopNos, ',');
                        shoplist = shoplist.Where(s => shopNos.Contains(s.shop.ShopNo)).ToList();
                    }
                    //位置
                    if (!string.IsNullOrWhiteSpace(planModel.PositionName))
                    {
                        shoplist = shoplist.Where(s => s.merge.Sheet == planModel.PositionName).ToList();
                    }
                    List<string> regionList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(planModel.RegionNames))
                    {
                        regionList = StringHelper.ToStringList(planModel.RegionNames, ',');
                        shoplist = shoplist.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
                    }
                    List<string> provinceList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(planModel.ProvinceId))
                    {
                        provinceList = StringHelper.ToStringList(planModel.ProvinceId, ',');
                        shoplist = shoplist.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(planModel.CityId))
                    {
                        List<string> cityList = StringHelper.ToStringList(planModel.CityId, ',');
                        shoplist = shoplist.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    }
                    //性别
                    List<string> genderList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(planModel.Gender))
                    {
                        genderList = StringHelper.ToStringList(planModel.Gender, ',');
                        shoplist = shoplist.Where(s => genderList.Contains(s.merge.Gender)).ToList();
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
                                List<int> frameShopList = frameLists.Where(f => f.PositionName == planModel.PositionName && list0.Contains(f.MachineFrame)).Select(f => f.ShopId ?? 0).ToList();
                                //当前位置的所有器架类型的店铺
                                List<int> frameShopList1 = frameLists.Where(f => f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                //当前位置没有器架类型的店铺
                                List<int> shopListNoFrame = shoplist.Where(l => !frameShopList1.Contains(l.merge.ShopId ?? 0)).Select(l => l.merge.ShopId ?? 0).ToList();
                                frameShopList.AddRange(shopListNoFrame);
                                shoplist = shoplist.Where(l => frameShopList.Contains(l.merge.ShopId ?? 0)).ToList();
                                //frames = list0;
                                var emptyPOPSizeList = shoplist.Where(s => s.pop == null).ToList();
                                var notEmptyPOPSizeList = shoplist.Where(s => s.pop != null).ToList();
                                if (notEmptyPOPSizeList.Any())
                                {
                                    notEmptyPOPSizeList = notEmptyPOPSizeList.Where(s => (s.pop.MachineFrameName != null && s.pop.MachineFrameName != "") ? (list0.Contains(s.pop.MachineFrameName)) : 1 == 1).ToList();
                                    if (notEmptyPOPSizeList.Any())
                                    {
                                        shoplist = emptyPOPSizeList.Union(notEmptyPOPSizeList).ToList();
                                    }
                                }
                            }
                            else//只有空
                            {
                                //当前位置的所有器架类型的店铺
                                List<int> frameShopList1 = frameLists.Where(f => f.PositionName == planModel.PositionName).Select(f => f.ShopId ?? 0).ToList();
                                shoplist = shoplist.Where(l => !frameShopList1.Contains(l.merge.ShopId ?? 0)).ToList();
                            }

                        }
                        else
                        {
                            List<string> frames = StringHelper.ToStringList(planModel.MachineFrameNames, ',');
                            var frameShopList = frameLists.Where(f => f.PositionName.Trim() == planModel.PositionName.Trim() && frames.Contains(f.MachineFrame)).Select(f => f.ShopId ?? 0).ToList();
                            shoplist = shoplist.Where(l => frameShopList.Contains(l.merge.ShopId ?? 0)).ToList();
                            var emptyPOPSizeList = shoplist.Where(s => s.pop == null).ToList();//没有POP数据的订单
                            var notEmptyPOPSizeList = shoplist.Where(s => s.pop != null).ToList();//待POP编号的订单
                            if (notEmptyPOPSizeList.Any())
                            {
                                //有器架关联的POP订单
                                var hasFramePOPList = notEmptyPOPSizeList.Where(s => s.pop.MachineFrameName != null && s.pop.MachineFrameName != "" && frames.Contains(s.pop.MachineFrameName)).ToList();
                                //有POP订单，但是没有器架关联的
                                var noFramePOPList = notEmptyPOPSizeList.Where(s => s.pop.MachineFrameName == null || s.pop.MachineFrameName == "").ToList();
                                if (hasFramePOPList.Any())
                                {
                                    shoplist = emptyPOPSizeList.Union(hasFramePOPList).Union(noFramePOPList).ToList();
                                }
                               
                            }
                        }
                    }


                    //城市级别
                    if (!string.IsNullOrWhiteSpace(planModel.CityTier))
                    {
                        List<string> cityTierList = StringHelper.ToStringList(planModel.CityTier, ',');
                        shoplist = shoplist.Where(s => cityTierList.Contains(s.shop.CityTier)).ToList();
                    }
                    //店铺类型
                    if (!string.IsNullOrWhiteSpace(planModel.Format))
                    {
                        List<string> formatList = StringHelper.ToStringList(planModel.Format, ',');
                        shoplist = shoplist.Where(s => formatList.Contains(s.shop.Format)).ToList();
                    }
                    //if (!string.IsNullOrWhiteSpace(planModel.ShopLevel))
                    //{
                    //    List<string> shopLevelList = StringHelper.ToStringList(planModel.ShopLevel, ',');
                    //    shoplist = shoplist.Where(s => shopLevelList.Contains(s.shop.POSScale)).ToList();
                    //}
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
                                shoplist = shoplist.Where(l => (l.pop != null && (list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) || (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))) || l.pop == null).ToList();

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
                                shoplist = shoplist.Where(l => (l.pop != null && (list0.Contains(l.pop.WindowWide + "*" + l.pop.WindowHigh + "*" + l.pop.WindowDeep) || ((l.pop.WindowHigh == null || l.pop.WindowHigh == 0) || (l.pop.WindowWide == null || l.pop.WindowWide == 0) || (l.pop.WindowDeep == null || l.pop.WindowDeep == 0)))) || l.pop == null).ToList();

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

                        List<string> region = new List<string>();
                        StringBuilder regionsb = new StringBuilder();

                        List<string> province = new List<string>();
                        StringBuilder provincesb = new StringBuilder();

                        List<string> city = new List<string>();
                        StringBuilder citysb = new StringBuilder();

                        List<string> gender = new List<string>();
                        StringBuilder gendersb = new StringBuilder();

                        List<string> cornerType = new List<string>();
                        StringBuilder cornerTypesb = new StringBuilder();

                        List<string> frameName = new List<string>();
                        StringBuilder frameNamesb = new StringBuilder();

                        List<string> cityTier = new List<string>();
                        StringBuilder cityTiersb = new StringBuilder();

                        List<string> shopLevel = new List<string>();
                        StringBuilder shopLevelsb = new StringBuilder();

                        List<string> format = new List<string>();
                        StringBuilder formatsb = new StringBuilder();

                        List<string> materialSupport = new List<string>();
                        StringBuilder mssb = new StringBuilder();

                        List<string> POSScale = new List<string>();
                        StringBuilder scalesb = new StringBuilder();

                        List<string> install = new List<string>();
                        StringBuilder installsb = new StringBuilder();

                        List<int> count = new List<int>();
                        StringBuilder countsb = new StringBuilder();

                        List<string> material = new List<string>();
                        StringBuilder materialsb = new StringBuilder();

                        List<string> POPSize = new List<string>();
                        StringBuilder POPSizesb = new StringBuilder();

                        List<string> WindowSize = new List<string>();
                        StringBuilder WindowSizesb = new StringBuilder();

                        List<string> IsElectricity = new List<string>();
                        StringBuilder IsElectricitysb = new StringBuilder();

                        List<string> ChooseImg = new List<string>();
                        StringBuilder ChooseImgsb = new StringBuilder();
                        //符合条件的所有shopid
                        var AllShopIdList = shoplist.Select(s => s.shop.Id).Distinct().ToList();
                        shoplist.ToList().ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s.shop.RegionName) && !region.Contains(s.shop.RegionName))
                            {
                                region.Add(s.shop.RegionName);
                                regionsb.Append(s.shop.RegionName);
                                regionsb.Append(',');
                            }
                            if (!string.IsNullOrWhiteSpace(s.shop.ProvinceName) && !province.Contains(s.shop.ProvinceName))
                            {
                                province.Add(s.shop.ProvinceName);
                                //provincesb.Append(s.shop.ProvinceName);
                                //provincesb.Append(',');
                            }
                            if (!string.IsNullOrWhiteSpace(s.shop.CityName) && !city.Contains(s.shop.ProvinceName + "_" + s.shop.CityName))
                            {
                                city.Add(s.shop.ProvinceName + "_" + s.shop.CityName);
                                //citysb.Append(s.shop.ProvinceName+"_"+s.shop.CityName);
                                //citysb.Append(',');
                            }

                            if (!string.IsNullOrWhiteSpace(s.merge.Gender) && !gender.Contains(s.merge.Gender))
                            {
                                gender.Add(s.merge.Gender);
                                //gendersb.Append(s.merge.Gender);
                                //gendersb.Append(',');
                            }



                            if (!string.IsNullOrWhiteSpace(s.shop.CityTier) && !cityTier.Contains(s.shop.CityTier))
                            {
                                cityTier.Add(s.shop.CityTier);
                                //cityTiersb.Append(s.shop.CityTier);
                                //cityTiersb.Append(',');
                            }



                            if (!string.IsNullOrWhiteSpace(s.shop.Format) && !format.Contains(s.shop.Format))
                            {
                                format.Add(s.shop.Format);
                                //formatsb.Append(s.shop.Format);
                                //formatsb.Append(',');
                            }
                            string materialSupport1 = !string.IsNullOrWhiteSpace(s.merge.MaterialSupport) ? s.merge.MaterialSupport : "空";

                            if (!materialSupport.Contains(materialSupport1))
                            {
                                materialSupport.Add(materialSupport1);
                                //mssb.Append(materialSupport1);
                                //mssb.Append(',');
                            }
                            string isinstall = !string.IsNullOrWhiteSpace(s.shop.IsInstall) ? s.shop.IsInstall : "空";
                            if (!install.Contains(isinstall))
                            {
                                install.Add(isinstall);
                                //installsb.Append(isinstall);
                                //installsb.Append(',');
                            }
                            if (!string.IsNullOrWhiteSpace(s.merge.POSScale) && !POSScale.Contains(s.merge.POSScale))
                            {
                                POSScale.Add(s.merge.POSScale);
                                //scalesb.Append(s.merge.POSScale);
                                //scalesb.Append(',');
                            }

                            if (s.merge.Quantity != null && !count.Contains(s.merge.Quantity ?? 0))
                            {
                                count.Add(s.merge.Quantity ?? 0);

                            }
                            if (s.pop != null && !string.IsNullOrWhiteSpace(s.pop.GraphicMaterial) && !material.Contains(s.pop.GraphicMaterial))
                            {
                                material.Add(s.pop.GraphicMaterial);
                                //materialsb.Append(s.pop.GraphicMaterial);
                                //materialsb.Append(',');
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
                                    //POPSizesb.Append(popsize);
                                    //POPSizesb.Append(',');
                                }
                            }
                            else
                            {
                                string popsize0 = "空";
                                if (!POPSize.Contains(popsize0))
                                {
                                    POPSize.Add(popsize0);
                                    //POPSizesb.Append(popsize0);
                                    //POPSizesb.Append(',');
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
                                    //WindowSizesb.Append(windowsize);
                                    //WindowSizesb.Append(',');
                                }
                            }
                            else
                            {
                                string windowsize0 = "空";
                                if (!WindowSize.Contains(windowsize0))
                                {
                                    WindowSize.Add(windowsize0);
                                    //WindowSizesb.Append(windowsize0);
                                    //WindowSizesb.Append(',');
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
                        //取角落类型和器架
                        if (!string.IsNullOrWhiteSpace(planModel.PositionName))
                        {
                            List<ShopMachineFrame> framelist = new List<ShopMachineFrame>();
                            framelist = frameLists.Where(f => AllShopIdList.Contains(f.ShopId ?? 0) && f.PositionName.Trim() == planModel.PositionName.Trim()).ToList();
                            if (genderList.Any())
                            {
                                bool allGender = false;
                                genderList.ForEach(g =>
                                {
                                    if (g.Contains("男") && g.Contains("女"))
                                        allGender = true;
                                });
                                if (allGender)
                                {
                                    genderList = new List<string>() { "男", "女" };
                                }
                                framelist = framelist.Where(f => genderList.Contains(f.Gender) || (f.Gender.Contains("男") && f.Gender.Contains("女"))).ToList();
                            }
                            else
                            {
                                framelist = (from frame in framelist
                                             join shop in shoplist
                                             on new { frame.Gender, frame.ShopId } equals new { shop.merge.Gender, shop.merge.ShopId }
                                             select frame).Union(
                                           from frame1 in framelist
                                           join shop1 in shoplist
                                           on frame1.ShopId equals shop1.shop.Id
                                           where frame1.Gender.Contains("男") && frame1.Gender.Contains("女")
                                           select frame1
                                          ).ToList();


                            }
                            if (!string.IsNullOrWhiteSpace(subjectCornerType))
                            {
                                //framelist = framelist.Where(s => s.CornerType.Contains("三叶草")).ToList();
                                framelist = framelist.Where(s => s.CornerType == subjectCornerType).ToList();
                            }
                            else
                            {
                                framelist = framelist.Where(s => (s.CornerType == null || s.CornerType == "")).ToList();
                            }
                            //获取角落类型
                            framelist.ForEach(f =>
                            {
                                string cornerType1 = !string.IsNullOrWhiteSpace(f.CornerType) ? f.CornerType : "空";
                                if (!cornerType.Contains(cornerType1))
                                {
                                    cornerType.Add(cornerType1);
                                    cornerTypesb.Append(cornerType1);
                                    cornerTypesb.Append(',');
                                }
                            });

                            if (subjectCategory == "童店")
                            {
                                framelist = (from frame in framelist
                                             join shop in CurrentContext.DbContext.Shop
                                             on frame.ShopId equals shop.Id
                                             where shop.Channel != null && (shop.Channel.ToLower().Contains("kids") || shop.Channel.ToLower().Contains("infant"))
                                             select frame).ToList();
                            }
                            else
                            {
                                framelist = (from frame in framelist
                                             join shop in CurrentContext.DbContext.Shop
                                             on frame.ShopId equals shop.Id
                                             where shop.Channel == null || shop.Channel == "" || (!shop.Channel.ToLower().Contains("kids") && !shop.Channel.ToLower().Contains("infant"))
                                             select frame).ToList();
                            }

                            if (!string.IsNullOrWhiteSpace(planModel.CornerType))
                            {
                                if (planModel.CornerType.Contains("空"))
                                {
                                    List<string> cornerTypes0 = StringHelper.ToStringList(planModel.CornerType.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (cornerTypes0.Any())
                                    {
                                        framelist = framelist.Where(f => cornerTypes0.Contains(f.CornerType) || (f.CornerType == null || f.CornerType == "")).OrderBy(f => f.MachineFrame).ToList();
                                    }
                                    else
                                    {
                                        //cornerTypeIsNull = true;
                                        framelist = framelist.Where(f => f.CornerType == null || f.CornerType == "").OrderBy(f => f.MachineFrame).ToList();
                                    }
                                }
                                else
                                {
                                    List<string> cornerTypes = StringHelper.ToStringList(planModel.CornerType, ',');
                                    framelist = framelist.Where(f => cornerTypes.Contains(f.CornerType)).OrderBy(f => f.MachineFrame).ToList();
                                }
                            }
                            else
                            {
                                //framelist = framelist.Where(f => f.CornerType == null || f.CornerType == "").OrderBy(f => f.MachineFrame).ToList();
                                framelist = framelist.OrderBy(f => f.MachineFrame).ToList();

                            }
                            framelist.ForEach(f =>
                            {
                                if (!frameName.Contains(f.MachineFrame))
                                {
                                    frameName.Add(f.MachineFrame);
                                    frameNamesb.Append(f.MachineFrame);
                                    frameNamesb.Append(',');
                                }
                            });
                            //有器架的店铺
                            var frameShopIdList = framelist.Select(s => s.ShopId ?? 0).Distinct().ToList();
                            //将有器架的店合在所有店铺里，再把有器架的店铺去掉，剩下的就是无器架的店铺
                            AllShopIdList.AddRange(frameShopIdList);
                            List<int> result1 = AllShopIdList.Union(frameShopIdList).ToList();

                        }






                        if (regionList.Any())
                        {
                            province.Sort();
                            province.ForEach(s =>
                            {
                                provincesb.Append(s);
                                provincesb.Append(',');
                            });
                        }
                        else
                        {
                            provincesb = new StringBuilder();
                        }
                        if (provinceList.Any())
                        {
                            city.Sort();
                            city.ForEach(s =>
                            {
                                citysb.Append(s);
                                citysb.Append(',');
                            });
                        }
                        else
                        {
                            citysb = new StringBuilder();
                        }

                        gender.Sort();
                        gender.ForEach(s =>
                        {
                            gendersb.Append(s);
                            gendersb.Append(',');
                        });
                        cityTier.Sort();
                        cityTier.ForEach(s =>
                        {
                            cityTiersb.Append(s);
                            cityTiersb.Append(',');
                        });
                        format.Sort();
                        format.ForEach(s =>
                        {
                            formatsb.Append(s);
                            formatsb.Append(',');
                        });
                        shopLevel.Sort();
                        shopLevel.ForEach(s =>
                        {
                            shopLevelsb.Append(s);
                            shopLevelsb.Append(',');
                        });

                        materialSupport.Sort();
                        materialSupport.ForEach(s =>
                        {
                            mssb.Append(s);
                            mssb.Append(',');
                        });
                        install.Sort();
                        install.ForEach(s =>
                        {
                            installsb.Append(s);
                            installsb.Append(',');
                        });
                        POSScale.Sort();
                        POSScale.ForEach(s =>
                        {
                            scalesb.Append(s);
                            scalesb.Append(',');
                        });
                        count.Sort();
                        count.ForEach(s =>
                        {
                            countsb.Append(s);
                            countsb.Append(',');
                        });
                        material.Sort();
                        material.ForEach(s =>
                        {
                            materialsb.Append(s);
                            materialsb.Append(',');
                        });

                        POPSize.Sort();
                        POPSize.ForEach(s =>
                        {
                            POPSizesb.Append(s);
                            POPSizesb.Append(',');
                        });
                        WindowSize.Sort();
                        WindowSize.ForEach(s =>
                        {
                            WindowSizesb.Append(s);
                            WindowSizesb.Append(',');
                        });
                        json.Append("{\"RegionNames\":\"" + regionsb.ToString().TrimEnd(',') + "\",\"ProvinceName\":\"" + provincesb.ToString().TrimEnd(',') + "\",\"CityName\":\"" + citysb.ToString().TrimEnd(',') + "\",\"CornerType\":\"" + cornerTypesb.ToString().TrimEnd(',') + "\",\"MachineFrameNames\":\"" + frameNamesb.ToString().TrimEnd(',') + "\",\"CityTier\":\"" + cityTiersb.ToString().TrimEnd(',') + "\",\"Format\":\"" + formatsb.ToString().TrimEnd(',') + "\",\"MaterialSupport\":\"" + mssb.ToString().TrimEnd(',') + "\",\"IsInstall\":\"" + installsb.ToString().TrimEnd(',') + "\",\"POSScale\":\"" + scalesb.ToString().TrimEnd(',') + "\",\"Gender\":\"" + gendersb.ToString().TrimEnd(',') + "\",");

                        json.Append("\"GraphicMaterial\":\"" + materialsb.ToString().TrimEnd(',') + "\",\"Quantity\":\"" + countsb.ToString().TrimEnd(',') + "\",\"POPSize\":\"" + POPSizesb.ToString().TrimEnd(',') + "\",\"ChooseImg\":\"" + ChooseImgsb.ToString().TrimEnd(',') + "\",\"IsElectricity\":\"" + IsElectricitysb.ToString().TrimEnd(',') + "\",\"WindowSize\":\"" + WindowSizesb.ToString().TrimEnd(',') + "\",\"ShopLevel\":\"" + shopLevelsb.ToString().TrimEnd(',') + "\"}");
                        restult = "[" + json.ToString() + "]";
                    }
                    else
                    {
                        StringBuilder json1 = new StringBuilder();
                        json1.Append("{\"RegionNames\":\"\",\"ProvinceName\":\"\",\"CityName\":\"\",\"CornerType\":\"\",\"MachineFrameNames\":\"\",\"CityTier\":\"\",\"Format\":\"\",\"MaterialSupport\":\"\",\"IsInstall\":\"\",\"POSScale\":\"\",\"Gender\":\"\",");

                        json1.Append("\"GraphicMaterial\":\"\",\"Quantity\":\"\",\"POPSize\":\"\",\"ChooseImg\":\"\",\"IsElectricity\":\"\",\"WindowSize\":\"\",\"ShopLevel\":\"\"}");
                        restult = "[" + json1.ToString() + "]";
                    }
                }

            }
            return restult;
        }

        List<int> GetShopId(string shopNos, out string notExistShopNo)
        {
            notExistShopNo = string.Empty;
            List<int> ids = new List<int>();
            List<string> shopNoList = StringHelper.ToStringList(shopNos, ',').Distinct().ToList();
            ShopBLL shopBll = new ShopBLL();
            StringBuilder str = new StringBuilder();
            shopNoList.ForEach(s =>
            {
                var model = shopBll.GetList(l => l.ShopNo == s).FirstOrDefault();
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

        /// <summary>
        /// 获取客户材质
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="bigTypeId"></param>
        /// <returns></returns>
        string GetCustomerMaterial()
        {

            //var list = new CustomerMaterialInfoBLL().GetList(s => s.CustomerId == customerId && (s.IsDelete == false || s.IsDelete == null));
            int priceItemId = 0;
            var guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on subject.GuidanceId equals guidance.ItemId
                                 where subject.Id == subjectId
                                 select new
                                 {
                                     guidance
                                 }).FirstOrDefault();
            if (guidanceModel != null)
            {
                priceItemId = guidanceModel.guidance.PriceItemId ?? 0;
            }
            var list = (from customerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                        join basicMaterial in CurrentContext.DbContext.BasicMaterial
                        on customerMaterial.BasicMaterialId equals basicMaterial.Id
                        where customerMaterial.CustomerId == customerId && (customerMaterial.IsDelete == false || customerMaterial.IsDelete == null)
                        && customerMaterial.PriceItemId == priceItemId
                        select new
                        {
                            basicMaterial.MaterialName,
                            customerMaterial.Price
                        }).OrderBy(s => s.MaterialName).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {

                    json.Append("{\"MaterialName\":\"" + s.MaterialName + "\",\"Price\":\"" + s.Price + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        CityInOrderPlanBLL planCityBll = new CityInOrderPlanBLL();
        CityInOrderPlan planCityModel;
        ProvinceInOrderPlanBLL planProvinceBll = new ProvinceInOrderPlanBLL();
        ProvinceInOrderPlan planProvinceModel;
        OrderPlanBLL orderPlanBll = new OrderPlanBLL();
        SplitOrderPlanDetailBLL splitPlanBll = new SplitOrderPlanDetailBLL();

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
                    if (sort == "Gender")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.Gender).ToList();
                        else
                            list = list.OrderByDescending(s => s.Gender).ToList();
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
                    else if (sort == "CityTier")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.CityTier).ToList();
                        else
                            list = list.OrderByDescending(s => s.CityTier).ToList();
                    }
                    else if (sort == "IsInstall")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.IsInstall).ToList();
                        else
                            list = list.OrderByDescending(s => s.IsInstall).ToList();
                    }
                    else if (sort == "Format")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.Format).ToList();
                        else
                            list = list.OrderByDescending(s => s.Format).ToList();
                    }
                    else if (sort == "MaterialSupport")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.MaterialSupport).ToList();
                        else
                            list = list.OrderByDescending(s => s.MaterialSupport).ToList();
                    }
                    else if (sort == "POSScale")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.POSScale).ToList();
                        else
                            list = list.OrderByDescending(s => s.POSScale).ToList();
                    }
                    else if (sort == "POPSize")
                    {
                        if (order == "asc")
                            list = list.OrderBy(s => s.POPSize).ToList();
                        else
                            list = list.OrderByDescending(s => s.POPSize).ToList();
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
                        string noMainKV = s.NoMainKV != null && bool.Parse(s.NoMainKV.ToString()) ? "是" : "";
                        int isExcept = s.IsExcept == true ? 1 : 0;

                        json.Append("{\"Id\":\"" + s.Id + "\",\"RegionNames\":\"" + s.RegionNames + "\",\"ProvinceId\":\"" + s.ProvinceId + "\",\"CityId\":\"" + s.CityId + "\",\"ProvinceName\":\"" + ProvinceName + "\",\"CityName\":\"" + CityName + "\",\"CityTier\":\"" + s.CityTier + "\",\"IsInstall\":\"" + s.IsInstall + "\",\"Format\":\"" + s.Format + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"POSScale\":\"" + s.POSScale + "\",\"Gender\":\"" + s.Gender + "\",\"ShopNos\":\"" + s.ShopNos + "\",\"PositionId\":\"" + s.PositionId + "\",\"PositionName\":\"" + s.PositionName + "\",\"MachineFrameIds\":\"" + s.MachineFrameIds + "\",\"MachineFrameNames\":\"" + s.MachineFrameNames + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"POPSize\":\"" + s.POPSize + "\",\"WindowSize\":\"" + s.WindowSize + "\",\"ChooseImg\":\"" + s.ChooseImg + "\",\"CornerType\":\"" + s.CornerType + "\",\"KeepPOPSize\":\"" + keeppopsize + "\",\"IsElectricity\":\"" + s.IsElectricity + "\",\"NotInvolveShopNos\":\"" + s.NotInvolveShopNos + "\",\"NoMainKV\":\"" + noMainKV + "\"},");

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
                    json.Append("{\"Id\":\"" + s.Id + "\",\"OrderType\":\"" + orderType + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"CustomerMaterialId\":\"" + s.CustomerMaterialId + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"Quantity\":\"" + s.Quantity + "\",\"RackSalePrice\":\"" + s.RackSalePrice + "\",\"Remark\":\"" + s.Remark + "\",\"Supplier\":\"" + s.Supplier + "\",\"NewChooseImg\":\"" + s.NewChooseImg + "\",\"IsInSet\":\"" + isInSet + "\",\"IsPerShop\":\"" + isPerShop + "\",\"NewGender\":\"" + s.NewGender + "\",\"IsHCPOP\":\"" + isHCPOP + "\",\"WindowType\":\"" + s.WindowType + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

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

        string DeletePlan()
        {
            string ids = string.Empty;
            if (context1.Request.QueryString["planIds"] != null)
            {
                ids = context1.Request.QueryString["planIds"].ToString().TrimEnd(',');
            }
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids, ',');
                orderPlanBll.Delete(s => idList.Contains(s.Id));
                splitPlanBll.Delete(s => idList.Contains(s.PlanId ?? 0));
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
            int MaterialPriceId = 0;
            string subjectCategoryName = string.Empty;
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
            List<string> subjectCornerTypeList = new List<string>();
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, tOpt))
            {
                try
                {
                    //用来保存已拆单的数据，每次拆单都检查是否有重复
                    List<FinalOrderDetailTemp> FinishOrderList = new List<FinalOrderDetailTemp>();
                    //先删除所有之前已拆完的订单
                    Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                    if (subjectModel != null)
                    {
                        if (subjectModel.SubjectType == (int)SubjectTypeEnum.HC订单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区补单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区增补 || subjectModel.SubjectType == (int)SubjectTypeEnum.新开店订单)
                        {
                            finalOrderTempBll.Delete(s => s.RegionSupplementId == subjectId);
                            new QuoteOrderDetailBLL().Delete(s => s.RegionSupplementId == subjectId);
                        }
                        else
                        {
                            finalOrderTempBll.Delete(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0);
                            new QuoteOrderDetailBLL().Delete(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0);
                        }
                    }
                    string unitName = string.Empty;
                    #region 从合并订单表（MergeOriginalOrder）进行拆单
                    var listOrder = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join guidance in CurrentContext.DbContext.SubjectGuidance
                                     on subject.GuidanceId equals guidance.ItemId
                                     join pop1 in CurrentContext.DbContext.POP
                                     on new { shopid = order.ShopId,sheet=order.Sheet, graphicNo = order.GraphicNo } equals new { shopid = pop1.ShopId,sheet=pop1.Sheet, graphicNo = pop1.GraphicNo } into popTemp
                                     from pop in popTemp.DefaultIfEmpty()
                                     join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                     on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                                     from subjectCategory in categoryTemp.DefaultIfEmpty()
                                     where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                                     select new
                                     {
                                         subject,
                                         order,
                                         shop,
                                         pop,
                                         guidance.PriceItemId,
                                         subjectCategory
                                     }).ToList();
                    if (listOrder.Any())
                    {
                        MaterialPriceId = listOrder[0].PriceItemId ?? 0;
                        subjectCategoryName = listOrder[0].subjectCategory != null ? listOrder[0].subjectCategory.CategoryName : string.Empty;
                    }
                    if (MaterialPriceId == 0)
                    {
                        throw new Exception("该活动指引没有价格方案");
                    }
                    #region 按方案拆单
                    if (listOrder.Any() && planList.Any())
                    {
                        //户外店鞋墙主kv
                        List<HCSmallGraphicSize> terrexMainKvList = new HCSmallGraphicSizeBLL().GetList(s=>s.Format.ToLower()=="terrex");

                        if (!string.IsNullOrWhiteSpace(listOrder[0].subject.CornerType))
                            subjectCornerTypeList.Add(listOrder[0].subject.CornerType);
                        List<string> InSetList = new List<string>();
                        HCPOPBLL hcPOPBll = new HCPOPBLL();
                        planList.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s.CornerType))
                            {
                                subjectCornerTypeList.AddRange(StringHelper.ToStringList(s.CornerType, ','));
                            }
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
                                shopList = shopList.Where(l => List.Contains(l.order.Sheet.Trim())).ToList();
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
                            List<string> PlanGenderlist = new List<string>();
                            //性别
                            if (!string.IsNullOrWhiteSpace(s.Gender))
                            {
                                PlanGenderlist = StringHelper.ToStringList(s.Gender, ',');
                                shopList = shopList.Where(l => PlanGenderlist.Contains(l.order.OrderGender) || (l.order.OrderGender.Contains("男") && l.order.OrderGender.Contains("女"))).ToList();
                            }
                            //数量
                            if (!string.IsNullOrWhiteSpace(s.Quantity))
                            {
                                List<int> Quantitylist = StringHelper.ToIntList(s.Quantity, ',');
                                shopList = shopList.Where(l => Quantitylist.Contains(l.order.Quantity ?? 0)).ToList();
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
                                        shopList = shopList.Where(l => (l.pop != null && (list0.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength) || ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) || (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0)))) || l.pop == null).ToList();

                                    }
                                    else
                                    {
                                        //shopList = shopList.Where(l => l.pop != null && l.pop.GraphicLength == null && l.pop.GraphicWidth == null).ToList();
                                        shopList = shopList.Where(l => (l.pop != null && ((l.pop.GraphicLength == null || l.pop.GraphicLength == 0) || (l.pop.GraphicWidth == null || l.pop.GraphicWidth == 0))) || l.pop == null).ToList();

                                    }

                                }
                                else
                                {
                                    List<string> list = StringHelper.ToStringList(s.POPSize, ',');
                                    shopList = shopList.Where(l => l.pop != null && list.Contains(l.pop.GraphicWidth + "*" + l.pop.GraphicLength)).ToList();
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


                            if (!string.IsNullOrWhiteSpace(s.NotInvolveShopNos))
                            {
                                List<string> notInvolveList = StringHelper.ToStringList(s.NotInvolveShopNos, ',');
                                shopList = shopList.Where(l => !notInvolveList.Contains(l.shop.ShopNo)).ToList();
                            }

                            var planDetail = planDetailBll.GetList(d => d.PlanId == s.Id && (d.IsPerShop == null || d.IsPerShop == false));
                            #region 有器架类型
                            if (!string.IsNullOrWhiteSpace(s.MachineFrameNames))
                            {
                                // List<int> orderShopIdList = shopList.Select(sh => sh.shop.Id).Distinct().ToList();
                                List<string> frames = new List<string>();
                                int isKong = 0;
                                //判断角落类型
                                var frameList = frameBll.GetList(f => f.PositionName.Trim() == s.PositionName.Trim());
                                if (s.MachineFrameNames.Contains("空"))
                                {

                                    List<string> list0 = StringHelper.ToStringList(s.MachineFrameNames.Replace(",空", "").Replace("空,", "").Replace("空", ""), ',');
                                    if (list0.Any())//除了空，还有其他的器架
                                    {

                                        frameList = frameList.Where(f => list0.Contains(f.MachineFrame)).ToList();
                                        
                                        if (subjectCornerTypeList.Any())
                                        {
                                            if (subjectCornerTypeList.Contains("空"))
                                            {
                                                subjectCornerTypeList.Remove("空");
                                                if (subjectCornerTypeList.Any())
                                                {
                                                    frameList = frameList.Where(f => (f.CornerType == null || f.CornerType == "") || subjectCornerTypeList.Contains(f.CornerType)).ToList();
                                                }
                                                else
                                                    frameList = frameList.Where(f => (f.CornerType == null || f.CornerType == "")).ToList();
                                            }
                                            else
                                            {
                                                frameList = frameList.Where(f => subjectCornerTypeList.Contains(f.CornerType)).ToList();
                                            }
                                        }
                                        else
                                            frameList = frameList.Where(f => (f.CornerType == null || f.CornerType == "")).ToList();
                                        //所选择的器架类型的店铺
                                        List<int> frameShopList = frameList.Select(f => f.ShopId ?? 0).ToList();
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
                                        List<int> frameShopList1 = frameList.Select(f => f.ShopId ?? 0).ToList();
                                        shopList = shopList.Where(l => !frameShopList1.Contains(l.order.ShopId ?? 0)).ToList();
                                    }
                                    isKong = 1;
                                }
                                else
                                {
                                    frames = StringHelper.ToStringList(s.MachineFrameNames, ',');
                                    frameList = frameList.Where(f => frames.Contains(f.MachineFrame)).ToList();
                                   
                                    if (subjectCornerTypeList.Any())
                                    {
                                        if (subjectCornerTypeList.Contains("空"))
                                        {
                                            subjectCornerTypeList.Remove("空");
                                            if (subjectCornerTypeList.Any())
                                            {
                                                frameList = frameList.Where(f => (f.CornerType == null || f.CornerType == "") || subjectCornerTypeList.Contains(f.CornerType)).ToList();
                                            }
                                            else
                                                frameList = frameList.Where(f => (f.CornerType == null || f.CornerType == "")).ToList();
                                        }
                                        else
                                        {
                                            frameList = frameList.Where(f => subjectCornerTypeList.Contains(f.CornerType)).ToList();
                                        }
                                    }
                                    else
                                        frameList = frameList.Where(f => (f.CornerType == null || f.CornerType == "")).ToList();
                                    var frameShopList = frameList.Select(f => f.ShopId ?? 0).ToList();
                                    shopList = shopList.Where(l => frameShopList.Contains(l.order.ShopId ?? 0)).ToList();
                                    var emptyPOPSizeList = shopList.Where(sh => sh.pop == null).ToList();
                                    var notEmptyPOPSizeList = shopList.Where(sh => sh.pop != null).ToList();
                                    if (notEmptyPOPSizeList.Any())
                                    {
                                        var notEmptyFramePOPSizeList = notEmptyPOPSizeList.Where(sh => (sh.pop.MachineFrameName != null && sh.pop.MachineFrameName != "") ? (frames.Contains(sh.pop.MachineFrameName)) : 1 == 1).ToList();
                                        var emptyFramePOPSizeList = notEmptyPOPSizeList.Where(sh => sh.pop.MachineFrameName == null || sh.pop.MachineFrameName == "").ToList();

                                        if (notEmptyFramePOPSizeList.Any())
                                        {
                                            shopList = emptyPOPSizeList.Union(notEmptyFramePOPSizeList).Union(emptyFramePOPSizeList).ToList();
                                        }
                                    }
                                }

                                #region 执行方案内容
                                if (shopList.Any())
                                {
                                    
                                    if (subjectCategoryName == "户外")
                                    {
                                        #region 判断是否去掉主KV（主KV延续）
                                        List<HCSmallGraphicSize> mainKvList = terrexMainKvList.Where(kv => kv.Sheet == s.PositionName).ToList();
                                        List<HCSmallGraphicSize> noMainKvList = new List<HCSmallGraphicSize>();
                                        List<decimal> GraphicWidthList = new List<decimal>();
                                        if (s.NoMainKV ?? false)
                                        {
                                            frames.ForEach(f => {
                                                noMainKvList.AddRange(mainKvList.Where(kv => f.Contains(kv.Remark)).ToList());
                                            });

                                            GraphicWidthList = noMainKvList.Select(kv => kv.BigGraphicWidth ?? 0).ToList();
                                        }
                                        #endregion
                                        POPBLL popBll=new POPBLL();
                                        //如果是户外项目，先取出对应该器架的POP作为拆单结果
                                        shopList.ForEach(shop1 => {
                                            var shopframeListOOH = frameList.Where(f => f.ShopId == shop1.shop.Id && (f.Gender == shop1.order.Gender || (f.Gender.Contains("男") && f.Gender.Contains("女")) || (shop1.order.Gender.Contains("男") && shop1.order.Gender.Contains("女")))).ToList();
                                            shopframeListOOH.ForEach(frame =>
                                            {
                                                bool frameIsValid = frame.IsValid ?? true;
                                                var shopPOPList = popBll.GetList(pop => pop.ShopId == shop1.shop.Id && pop.Sheet.ToUpper() == s.PositionName.ToUpper() && pop.MachineFrameName != null && pop.MachineFrameName.ToUpper() == frame.MachineFrame.ToUpper() && (pop.Gender == shop1.order.Gender || (pop.Gender.Contains("男") && pop.Gender.Contains("女")) || (shop1.order.Gender.Contains("男") && shop1.order.Gender.Contains("女"))));
                                                //如果是主KV延续，去掉符合条件的主KV
                                                if (GraphicWidthList.Any())
                                                {
                                                    var mainKVPOPIdList = shopPOPList.Where(pop => PlanGenderlist.Contains(pop.Gender) && GraphicWidthList.Contains(pop.GraphicWidth ?? 0)).Select(pop => pop.Id).ToList();
                                                    shopPOPList = shopPOPList.Where(pop => !mainKVPOPIdList.Contains(pop.Id)).ToList();
                                                }
                                                if (shopPOPList.Any())
                                                {
                                                    shopPOPList.ForEach(pop => {
                                                        finalOrderTempModel = new FinalOrderDetailTemp();
                                                        finalOrderTempModel.AgentCode = shop1.shop.AgentCode;
                                                        finalOrderTempModel.AgentName = shop1.shop.AgentName;
                                                        finalOrderTempModel.BusinessModel = shop1.shop.BusinessModel;
                                                        finalOrderTempModel.Channel = shop1.shop.Channel;
                                                        finalOrderTempModel.City = shop1.shop.CityName;
                                                        finalOrderTempModel.CityTier = shop1.shop.CityTier;
                                                        finalOrderTempModel.Contact = shop1.shop.Contact1;
                                                        finalOrderTempModel.Format = shop1.shop.Format;
                                                        finalOrderTempModel.Gender = shop1.order.Gender;
                                                        finalOrderTempModel.OrderGender = shop1.order.OrderGender;
                                                        finalOrderTempModel.GraphicNo = shop1.order.GraphicNo;


                                                        finalOrderTempModel.IsInstall = shop1.shop.IsInstall;
                                                        finalOrderTempModel.BCSIsInstall = shop1.shop.BCSIsInstall;
                                                        finalOrderTempModel.LocationType = shop1.shop.LocationType;
                                                        finalOrderTempModel.MaterialSupport = shop1.order.MaterialSupport;
                                                        finalOrderTempModel.OrderType = 1;
                                                        finalOrderTempModel.POPAddress = shop1.shop.POPAddress;
                                                        finalOrderTempModel.POSScale = shop1.order.POSScale;
                                                        finalOrderTempModel.InstallPricePOSScale = shop1.order.POSScale;
                                                        finalOrderTempModel.InstallPriceMaterialSupport = shop1.order.MaterialSupport;

                                                        finalOrderTempModel.Province = shop1.shop.ProvinceName;
                                                        finalOrderTempModel.Quantity = shop1.order.Quantity;
                                                        finalOrderTempModel.Region = shop1.shop.RegionName;
                                                        finalOrderTempModel.ChooseImg = shop1.order.ChooseImg;

                                                        finalOrderTempModel.Sheet = shop1.order.Sheet;
                                                        finalOrderTempModel.ShopId = shop1.shop.Id;
                                                        finalOrderTempModel.ShopName = shop1.shop.ShopName;
                                                        finalOrderTempModel.ShopNo = shop1.shop.ShopNo;
                                                        finalOrderTempModel.SubjectId = subjectId;
                                                        finalOrderTempModel.Tel = shop1.shop.Tel1;
                                                        finalOrderTempModel.MachineFrame = frame.MachineFrame;
                                                        finalOrderTempModel.CornerType = frame.CornerType;
                                                        finalOrderTempModel.LevelNum = shop1.order.LevelNum;
                                                        decimal width = pop.GraphicWidth ?? 0;
                                                        decimal length = pop.GraphicLength ?? 0;

                                                        decimal unitPrice = 0;
                                                        decimal area = (width * length) / 1000000;
                                                        decimal totalPrice = 0;
                                                        if (!string.IsNullOrWhiteSpace(pop.GraphicMaterial))
                                                        {
                                                            //unitPrice = new BasePage().GetMaterialPrice(o.pop.GraphicMaterial);
                                                            pop.Quantity = shop1.order.Quantity;
                                                            pop.PriceItemId = MaterialPriceId;
                                                            unitPrice = new BasePage().GetMaterialUnitPrice(pop, out totalPrice, out unitName);
                                                        }

                                                        finalOrderTempModel.GraphicLength = length;
                                                        finalOrderTempModel.GraphicMaterial = pop.GraphicMaterial;
                                                        finalOrderTempModel.GraphicWidth = width;
                                                        finalOrderTempModel.UnitPrice = unitPrice;
                                                        finalOrderTempModel.PositionDescription = pop.PositionDescription;
                                                        finalOrderTempModel.POPName = pop.POPName;
                                                        finalOrderTempModel.POPType = pop.POPType;
                                                        finalOrderTempModel.WindowDeep = pop.WindowDeep;
                                                        finalOrderTempModel.WindowHigh = pop.WindowHigh;
                                                        finalOrderTempModel.WindowSize = pop.WindowSize;
                                                        finalOrderTempModel.WindowWide = pop.WindowWide;
                                                        finalOrderTempModel.IsElectricity = pop.IsElectricity;

                                                        finalOrderTempModel.Area = Math.Round(area, 6);
                                                        finalOrderTempModel.IsPOPMaterial = 1;
                                                        finalOrderTempModel.TotalPrice = totalPrice;
                                                        finalOrderTempModel.Remark = pop.Remark;
                                                        finalOrderTempModel.IsValid = pop.IsValid;
                                                        if ((finalOrderTempModel.IsValid ?? true) && !frameIsValid)
                                                        {
                                                            finalOrderTempModel.IsValid = frameIsValid;
                                                        } 
                                                        finalOrderTempModel.IsSplit = false;
                                                        if (shop1.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                                                        {
                                                            finalOrderTempModel.IsFromRegion = true;
                                                        }
                                                        finalOrderTempModel.ShopStatus = shop1.shop.Status;
                                                        finalOrderTempModel.GuidanceId = shop1.subject.GuidanceId;
                                                        finalOrderTempModel.CSUserId = shop1.shop.CSUserId;
                                                        finalOrderTempModel.UnitName = unitName;
                                                        finalOrderTempModel.ProduceOutsourceId =pop.ProduceOutsourceId;
                                                        finalOrderTempModel.AddDate = DateTime.Now;
                                                        finalOrderTempBll.Add(finalOrderTempModel);
                                                        
                                                    });
                                                    
                                                }
                                            });
                                            
                                            finishSplitId.Add(shop1.order.Id);
                                        });
                                    }
                                    if (planDetail.Any())
                                    {
                                        var list12 = shopList.Where(ss=>ss.shop.Id==173).ToList();
                                        //MaterialPriceId = shopList[0].PriceItemId??0;
                                        shopList.ForEach(o =>
                                        {
                                            //if (o.shop.Id == 173)
                                            //{
                                            //    int ddd = 1;
                                            //}
                                            var shopframeList = frameList.Where(f => f.ShopId == o.shop.Id && (f.Gender == o.order.Gender || (f.Gender.Contains("男") && f.Gender.Contains("女")) || (o.order.Gender.Contains("男") && o.order.Gender.Contains("女")))).ToList();
                                            if (o.pop != null && o.pop.MachineFrameName != null && o.pop.MachineFrameName != "")
                                            {
                                                shopframeList = shopframeList.Where(fr => fr.MachineFrame == o.pop.MachineFrameName).ToList();
                                            }
                                            if (shopframeList.Any())
                                            {
                                                if (o.order.LevelNum != null && o.order.LevelNum > 1)//订单里面的器架级别大于1
                                                {
                                                    shopframeList = shopframeList.Where(f => f.LevelNum == o.order.LevelNum).ToList();
                                                }
                                                else
                                                {
                                                    //默认是空级别或者1级别
                                                    shopframeList = shopframeList.Where(f => f.LevelNum == null || f.LevelNum == 1).ToList();
                                                }
                                            }
                                            else
                                            {
                                                if (isKong == 1)
                                                {
                                                    ShopMachineFrame newFrame = new ShopMachineFrame();
                                                    newFrame.Gender = "男女";
                                                    newFrame.LevelNum = 1;
                                                    newFrame.MachineFrame = "空";
                                                    shopframeList.Add(newFrame);
                                                }
                                            }
                                            if (shopframeList.Any())
                                            {

                                                List<string> frame = new List<string>();
                                                shopframeList.ForEach(f =>
                                                {
                                                    bool frameIsValid = f.IsValid ?? true;
                                                    if (!frame.Contains(f.MachineFrame))
                                                    {
                                                        frame.Add(f.MachineFrame);

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
                                                            finalOrderTempModel.OrderGender = o.order.OrderGender;
                                                            finalOrderTempModel.GraphicNo = o.order.GraphicNo;


                                                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                            finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                                            finalOrderTempModel.LocationType = o.shop.LocationType;
                                                            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                            finalOrderTempModel.OrderType = 1;
                                                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                            finalOrderTempModel.POSScale = o.order.POSScale;
                                                            finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                                                            finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                                                            //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                                                            if (subjectCategoryName == "童店")
                                                                finalOrderTempModel.InstallPriceMaterialSupport = "Others";
                                                            finalOrderTempModel.Province = o.shop.ProvinceName;
                                                            finalOrderTempModel.Quantity = o.order.Quantity;
                                                            finalOrderTempModel.Region = o.shop.RegionName;
                                                            finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                                                            //finalOrderTempModel.Remark = o.order.Remark;
                                                            finalOrderTempModel.SplitOrderRemark = "保持原尺寸";
                                                            finalOrderTempModel.Sheet = o.order.Sheet;
                                                            finalOrderTempModel.ShopId = o.shop.Id;
                                                            finalOrderTempModel.ShopName = o.shop.ShopName;
                                                            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                                            finalOrderTempModel.SubjectId = subjectId;
                                                            finalOrderTempModel.Tel = o.shop.Tel1;
                                                            finalOrderTempModel.MachineFrame = f.MachineFrame;
                                                            finalOrderTempModel.CornerType = f.CornerType;
                                                            finalOrderTempModel.LevelNum = o.order.LevelNum;
                                                            if (o.pop != null)
                                                            {
                                                                decimal width = o.pop.GraphicWidth ?? 0;
                                                                decimal length = o.pop.GraphicLength ?? 0;

                                                                decimal unitPrice = 0;
                                                                decimal area = (width * length) / 1000000;
                                                                decimal totalPrice = 0;

                                                                if (!string.IsNullOrWhiteSpace(o.pop.GraphicMaterial))
                                                                {
                                                                    //unitPrice = new BasePage().GetMaterialPrice(o.pop.GraphicMaterial);
                                                                    o.pop.Quantity = o.order.Quantity;
                                                                    o.pop.PriceItemId = MaterialPriceId;
                                                                    unitPrice = new BasePage().GetMaterialUnitPrice(o.pop, out totalPrice, out unitName);
                                                                }

                                                                finalOrderTempModel.GraphicLength = length;
                                                                finalOrderTempModel.GraphicMaterial = o.pop.GraphicMaterial;
                                                                finalOrderTempModel.GraphicWidth = width;
                                                                finalOrderTempModel.UnitPrice = unitPrice;
                                                                finalOrderTempModel.PositionDescription = o.pop.PositionDescription;
                                                                finalOrderTempModel.POPName = o.pop.POPName;
                                                                finalOrderTempModel.POPType = o.pop.POPType;
                                                                finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                                                finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                                                finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                                                finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                                                finalOrderTempModel.IsElectricity = o.pop.IsElectricity;

                                                                finalOrderTempModel.Area = Math.Round(area, 6);
                                                                finalOrderTempModel.IsPOPMaterial = 1;
                                                                finalOrderTempModel.TotalPrice = totalPrice;
                                                                finalOrderTempModel.Remark = o.pop.Remark;
                                                                finalOrderTempModel.IsValid = o.pop.IsValid;
                                                                finalOrderTempModel.ProduceOutsourceId = o.pop.ProduceOutsourceId;
                                                            }
                                                            if ((finalOrderTempModel.IsValid ?? true) && !frameIsValid)
                                                            {
                                                                finalOrderTempModel.IsValid = frameIsValid;
                                                            }
                                                            finalOrderTempModel.IsSplit = false;
                                                            if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                                                            {
                                                                finalOrderTempModel.IsFromRegion = true;
                                                            }
                                                            finalOrderTempModel.ShopStatus = o.shop.Status;
                                                            finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                                                            finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                                            finalOrderTempModel.UnitName = unitName;
                                                            finalOrderTempModel.AddDate = DateTime.Now;
                                                            finalOrderTempBll.Add(finalOrderTempModel);
                                                           
                                                        }
                                                        #endregion
                                                        #region
                                                        planDetail.ForEach(p =>
                                                        {
                                                            #region 拆成男女一套的
                                                            string inSetModel = o.shop.Id + "|" + o.order.Sheet + "|" + f.MachineFrame;
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
                                                                    finalOrderTempModel.OrderGender = o.order.OrderGender;
                                                                    finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                                                    finalOrderTempModel.GraphicLength = p.GraphicLength;
                                                                    finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                                                                    finalOrderTempModel.GraphicMaterial = p.GraphicMaterial;
                                                                    finalOrderTempModel.GraphicWidth = p.GraphicWidth;
                                                                    decimal area = 0;
                                                                    if (p.GraphicLength != null && p.GraphicWidth != null)
                                                                    {
                                                                        area = (p.GraphicLength ?? 0 * p.GraphicWidth ?? 0) / 1000000;
                                                                        finalOrderTempModel.Area = Math.Round(area, 6);
                                                                    }
                                                                    finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                                    finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                                                    finalOrderTempModel.LocationType = o.shop.LocationType;
                                                                    finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                                    finalOrderTempModel.OrderType = p.OrderType;
                                                                    finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                                    finalOrderTempModel.POSScale = o.order.POSScale;
                                                                    finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                                                                    finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                                                                    //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                                                                    if (subjectCategoryName == "童店")
                                                                        finalOrderTempModel.InstallPriceMaterialSupport = "Others";
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
                                                                    finalOrderTempModel.MachineFrame = f.MachineFrame;
                                                                    finalOrderTempModel.CornerType = f.CornerType;
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
                                                                        finalOrderTempModel.Remark = o.pop.Remark;
                                                                        finalOrderTempModel.IsValid = o.pop.IsValid;
                                                                        finalOrderTempModel.ProduceOutsourceId = o.pop.ProduceOutsourceId;
                                                                    }
                                                                    if ((finalOrderTempModel.IsValid ?? true) && !frameIsValid)
                                                                    {
                                                                        finalOrderTempModel.IsValid = frameIsValid;
                                                                    }
                                                                    finalOrderTempModel.SmallMaterialId = GetSmallMaterial(o.order.Sheet, f.MachineFrame);

                                                                    decimal totalPrice = 0;
                                                                    POP popModel = new POP();
                                                                    popModel.GraphicLength = p.GraphicLength;
                                                                    popModel.GraphicWidth = p.GraphicWidth;
                                                                    popModel.GraphicMaterial = p.GraphicMaterial;
                                                                    popModel.Quantity = p.Quantity;
                                                                    popModel.PriceItemId = MaterialPriceId;
                                                                    new BasePage().GetMaterialUnitPrice(popModel, out totalPrice, out unitName);

                                                                    finalOrderTempModel.TotalPrice = totalPrice;
                                                                    finalOrderTempModel.IsSplit = true;
                                                                    if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                                                                    {
                                                                        finalOrderTempModel.IsFromRegion = true;
                                                                    }
                                                                    finalOrderTempModel.ShopStatus = o.shop.Status;
                                                                    finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                                                                    finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                                                    finalOrderTempModel.UnitName = unitName;
                                                                    finalOrderTempModel.AddDate = DateTime.Now;
                                                                    finalOrderTempBll.Add(finalOrderTempModel);
                                                                   
                                                                }
                                                            }
                                                            #endregion
                                                            else
                                                            {
                                                                finalOrderTempModel = new FinalOrderDetailTemp();
                                                                InSetList.Add(inSetModel);

                                                                decimal popWidth = p.GraphicWidth ?? 0;
                                                                decimal popHeight = p.GraphicLength ?? 0;
                                                                int count = p.Quantity ?? 0;

                                                                decimal unitPrice = p.RackSalePrice ?? 0;
                                                                string remark = "";
                                                                string material = p.GraphicMaterial;
                                                                if (o.pop != null)
                                                                {
                                                                    remark = o.pop.Remark;
                                                                    if (o.order.Sheet == "橱窗" && !string.IsNullOrWhiteSpace(p.WindowType))
                                                                    {
                                                                        if (string.IsNullOrWhiteSpace(material))
                                                                        {
                                                                            material = o.pop.GraphicMaterial;
                                                                            finalOrderTempModel.IsPOPMaterial = 1;
                                                                        }

                                                                        if (p.WindowType == "LeftSideStick")
                                                                        {
                                                                            if (o.pop.LeftSideStick != "N")
                                                                            {
                                                                                popWidth = o.pop.WindowDeep ?? 0;
                                                                                popHeight = o.pop.WindowHigh ?? 0;
                                                                            }
                                                                            else
                                                                            {
                                                                                popWidth = 0;
                                                                                popHeight = 0;
                                                                            }

                                                                        }
                                                                        if (p.WindowType == "RightSideStick")
                                                                        {

                                                                            if (o.pop.RightSideStick != "N")
                                                                            {
                                                                                popWidth = o.pop.WindowDeep ?? 0;
                                                                                popHeight = o.pop.WindowHigh ?? 0;
                                                                            }
                                                                            else
                                                                            {
                                                                                popWidth = 0;
                                                                                popHeight = 0;
                                                                            }
                                                                        }
                                                                        if (p.WindowType == "Floor")
                                                                        {

                                                                            if (o.pop.Floor != "N")
                                                                            {
                                                                                popWidth = o.pop.WindowWide ?? 0;
                                                                                popHeight = o.pop.WindowDeep ?? 0;
                                                                            }
                                                                            else
                                                                            {
                                                                                popWidth = 0;
                                                                                popHeight = 0;
                                                                            }
                                                                        }
                                                                        if (p.WindowType == "WindowStick")
                                                                        {
                                                                            popWidth = 1;
                                                                            popHeight = 1;
                                                                            remark = remark + "(需要分区确认尺寸)";
                                                                        }
                                                                        //popWidth = popWidth == 0 ? 1 : popWidth;
                                                                        //popHeight = popHeight == 0 ? 1 : popHeight;
                                                                    }
                                                                }


                                                                finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                                                finalOrderTempModel.AgentName = o.shop.AgentName;
                                                                finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                                                finalOrderTempModel.Channel = o.shop.Channel;
                                                                finalOrderTempModel.City = o.shop.CityName;
                                                                finalOrderTempModel.CityTier = o.shop.CityTier;
                                                                finalOrderTempModel.Contact = o.shop.Contact1;
                                                                finalOrderTempModel.Format = o.shop.Format;
                                                                finalOrderTempModel.Gender = !string.IsNullOrWhiteSpace(p.NewGender) ? p.NewGender : o.order.Gender;
                                                                finalOrderTempModel.OrderGender = o.order.OrderGender;
                                                                finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                                                finalOrderTempModel.GraphicLength = popHeight;
                                                                finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                                                                finalOrderTempModel.GraphicMaterial = material;
                                                                finalOrderTempModel.GraphicWidth = popWidth;
                                                                if (popHeight > 0 && popWidth > 0)
                                                                {

                                                                    finalOrderTempModel.Area = Math.Round((popHeight * popWidth) / 1000000, 6);
                                                                }
                                                                finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                                finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                                                finalOrderTempModel.LocationType = o.shop.LocationType;
                                                                finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                                finalOrderTempModel.OrderType = p.OrderType;
                                                                finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                                finalOrderTempModel.POSScale = o.order.POSScale;
                                                                finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                                                                finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                                                                //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                                                                if (subjectCategoryName == "童店")
                                                                    finalOrderTempModel.InstallPriceMaterialSupport = "Others";
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
                                                                finalOrderTempModel.Remark = remark;

                                                                finalOrderTempModel.MachineFrame = f.MachineFrame;
                                                                finalOrderTempModel.CornerType = f.CornerType;
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
                                                                    finalOrderTempModel.IsValid = o.pop.IsValid;
                                                                    finalOrderTempModel.ProduceOutsourceId = o.pop.ProduceOutsourceId;
                                                                }
                                                                if ((finalOrderTempModel.IsValid ?? true) && !frameIsValid)
                                                                {
                                                                    finalOrderTempModel.IsValid = frameIsValid;
                                                                }
                                                                decimal totalPrice = 0;
                                                                POP popModel = new POP();
                                                                popModel.GraphicLength = popHeight;
                                                                popModel.GraphicWidth = popWidth;
                                                                popModel.GraphicMaterial = material;
                                                                popModel.Quantity = p.Quantity;
                                                                popModel.PriceItemId = MaterialPriceId;
                                                                unitPrice = new BasePage().GetMaterialUnitPrice(popModel, out totalPrice, out unitName);

                                                                finalOrderTempModel.UnitPrice = unitPrice;
                                                                finalOrderTempModel.TotalPrice = totalPrice;
                                                                finalOrderTempModel.SmallMaterialId = GetSmallMaterial(o.order.Sheet, f.MachineFrame);
                                                                finalOrderTempModel.IsSplit = true;
                                                                if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                                                                {
                                                                    finalOrderTempModel.IsFromRegion = true;
                                                                }
                                                                finalOrderTempModel.ShopStatus = o.shop.Status;
                                                                finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                                                                finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                                                finalOrderTempModel.UnitName = unitName;
                                                                finalOrderTempModel.AddDate = DateTime.Now;
                                                                bool isOk = true;
                                                                
                                                                if (isOk)
                                                                {
                                                                    finalOrderTempBll.Add(finalOrderTempModel);
                                                                    FinishOrderList.Add(finalOrderTempModel);
                                                                   
                                                                }
                                                            }
                                                            finishSplitId.Add(o.order.Id);

                                                        });
                                                        #endregion
                                                    }
                                                });
                                            }
                                        });
                                    }
                                    
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
                                        if (!keepId.Contains(o.order.Id))
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
                                            finalOrderTempModel.OrderGender = o.order.OrderGender;
                                            finalOrderTempModel.GraphicNo = o.order.GraphicNo;


                                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                            finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                            finalOrderTempModel.LocationType = o.shop.LocationType;
                                            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                            finalOrderTempModel.OrderType = 1;//默认是pop
                                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                            finalOrderTempModel.POSScale = o.order.POSScale;
                                            finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                                            finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                                            //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                                            if (subjectCategoryName == "童店")
                                                finalOrderTempModel.InstallPriceMaterialSupport = "Others";
                                            finalOrderTempModel.Province = o.shop.ProvinceName;
                                            finalOrderTempModel.Quantity = o.order.Quantity;
                                            finalOrderTempModel.Region = o.shop.RegionName;
                                            finalOrderTempModel.ChooseImg = o.order.ChooseImg;

                                            finalOrderTempModel.SplitOrderRemark = "保持原尺寸";
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
                                               
                                                decimal unitPrice = 0;
                                                decimal totalPrice = 0;
                                                if (!string.IsNullOrWhiteSpace(o.pop.GraphicMaterial))
                                                {
                                                    //unitPrice = new BasePage().GetMaterialPrice(o.pop.GraphicMaterial);
                                                    o.pop.Quantity = o.order.Quantity;
                                                    o.pop.PriceItemId = MaterialPriceId;
                                                    unitPrice = new BasePage().GetMaterialUnitPrice(o.pop, out totalPrice, out unitName);
                                                }
                                                finalOrderTempModel.GraphicLength = length;
                                                finalOrderTempModel.GraphicMaterial = o.pop.GraphicMaterial;
                                                finalOrderTempModel.GraphicWidth = width;
                                                finalOrderTempModel.UnitPrice = unitPrice;
                                               
                                                finalOrderTempModel.PositionDescription = o.pop.PositionDescription;
                                                finalOrderTempModel.POPName = o.pop.POPName;
                                                finalOrderTempModel.POPType = o.pop.POPType;
                                                finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                                finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                                finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                                finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                                finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                                finalOrderTempModel.Area = Math.Round((length * width) / 1000000, 6);
                                                finalOrderTempModel.TotalPrice = totalPrice;
                                                finalOrderTempModel.IsPOPMaterial = 1;
                                                finalOrderTempModel.Remark = o.pop.Remark;
                                                finalOrderTempModel.IsValid = o.pop.IsValid;
                                                finalOrderTempModel.ProduceOutsourceId = o.pop.ProduceOutsourceId;
                                            }

                                            finalOrderTempModel.IsSplit = false;
                                            if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                                            {
                                                finalOrderTempModel.IsFromRegion = true;
                                            }
                                            finalOrderTempModel.ShopStatus = o.shop.Status;
                                            finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                                            finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                            finalOrderTempModel.UnitName = unitName;
                                            finalOrderTempModel.AddDate = DateTime.Now;
                                            finalOrderTempBll.Add(finalOrderTempModel);

                                           
                                            keepId.Add(o.order.Id);
                                        }

                                    });
                                }
                                #endregion
                                #region 执行方案内容
                                if (shopList.Any() && planDetail.Any())
                                {
                                    //MaterialPriceId = shopList[0].PriceItemId ?? 0;
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
                                                    finalOrderTempModel.OrderGender = o.order.OrderGender;
                                                    finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                                    finalOrderTempModel.GraphicLength = p.GraphicLength;
                                                    finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                                                    finalOrderTempModel.GraphicMaterial = p.GraphicMaterial;
                                                    finalOrderTempModel.GraphicWidth = p.GraphicWidth;
                                                    if (p.GraphicLength != null && p.GraphicWidth != null)
                                                    {
                                                        decimal area = (p.GraphicLength ?? 0 * p.GraphicWidth ?? 0) / 1000000;
                                                        finalOrderTempModel.Area = Math.Round(area, 6);
                                                    }
                                                    finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                    finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                                    finalOrderTempModel.LocationType = o.shop.LocationType;
                                                    finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                    finalOrderTempModel.OrderType = p.OrderType;
                                                    finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                    finalOrderTempModel.POSScale = o.order.POSScale;
                                                    finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                                                    finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                                                    //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                                                    if (subjectCategoryName == "童店")
                                                        finalOrderTempModel.InstallPriceMaterialSupport = "Others";
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
                                                        finalOrderTempModel.Remark = o.pop.Remark;
                                                        finalOrderTempModel.IsValid = o.pop.IsValid;
                                                        finalOrderTempModel.ProduceOutsourceId = o.pop.ProduceOutsourceId;
                                                    }


                                                    decimal totalPrice = 0;
                                                    POP popModel = new POP();
                                                    popModel.GraphicLength = p.GraphicLength;
                                                    popModel.GraphicWidth = p.GraphicWidth;
                                                    popModel.GraphicMaterial = p.GraphicMaterial;
                                                    popModel.Quantity = p.Quantity;
                                                    popModel.PriceItemId = MaterialPriceId;
                                                    new BasePage().GetMaterialUnitPrice(popModel, out totalPrice, out unitName);

                                                    finalOrderTempModel.TotalPrice = totalPrice;
                                                    finalOrderTempModel.IsSplit = true;
                                                    if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                                                    {
                                                        finalOrderTempModel.IsFromRegion = true;
                                                    }
                                                    finalOrderTempModel.ShopStatus = o.shop.Status;
                                                    finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                                                    finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                                    finalOrderTempModel.UnitName = unitName;
                                                    finalOrderTempModel.AddDate = DateTime.Now;
                                                    finalOrderTempBll.Add(finalOrderTempModel);
                                                   
                                                }
                                            }
                                            else
                                            {
                                                finalOrderTempModel = new FinalOrderDetailTemp();
                                                decimal popWidth = p.GraphicWidth ?? 0;
                                                decimal popHeight = p.GraphicLength ?? 0;
                                                string material = p.GraphicMaterial;
                                                decimal unitPrice = p.RackSalePrice ?? 0;
                                                string remark = "";
                                                if (o.pop != null)
                                                {
                                                    remark = o.pop.Remark;
                                                    if (o.order.Sheet == "橱窗" && !string.IsNullOrWhiteSpace(p.WindowType))
                                                    {
                                                        if (string.IsNullOrWhiteSpace(material))
                                                        {
                                                            material = o.pop.GraphicMaterial;
                                                            finalOrderTempModel.IsPOPMaterial = 1;
                                                        }

                                                        //if (p.WindowType == "LeftSideStick")
                                                        //{
                                                        //    popWidth = o.pop.WindowDeep ?? 0;
                                                        //    popHeight = o.pop.WindowHigh ?? 0;
                                                        //}
                                                        //if (p.WindowType == "RightSideStick")
                                                        //{
                                                        //    popWidth = o.pop.WindowDeep ?? 0;
                                                        //    popHeight = o.pop.WindowHigh ?? 0;
                                                        //}
                                                        //if (p.WindowType == "Floor")
                                                        //{
                                                        //    popWidth = o.pop.WindowWide ?? 0;
                                                        //    popHeight = o.pop.WindowDeep ?? 0;
                                                        //}
                                                        if (p.WindowType == "LeftSideStick")
                                                        {
                                                            if (o.pop.LeftSideStick != "N")
                                                            {
                                                                popWidth = o.pop.WindowDeep ?? 0;
                                                                popHeight = o.pop.WindowHigh ?? 0;
                                                            }
                                                            else
                                                            {
                                                                popWidth = 0;
                                                                popHeight = 0;
                                                            }

                                                        }
                                                        if (p.WindowType == "RightSideStick")
                                                        {

                                                            if (o.pop.RightSideStick != "N")
                                                            {
                                                                popWidth = o.pop.WindowDeep ?? 0;
                                                                popHeight = o.pop.WindowHigh ?? 0;
                                                            }
                                                            else
                                                            {
                                                                popWidth = 0;
                                                                popHeight = 0;
                                                            }
                                                        }
                                                        if (p.WindowType == "Floor")
                                                        {

                                                            if (o.pop.Floor !="N")
                                                            {
                                                                popWidth = o.pop.WindowWide ?? 0;
                                                                popHeight = o.pop.WindowDeep ?? 0;
                                                            }
                                                            else
                                                            {
                                                                popWidth = 0;
                                                                popHeight = 0;
                                                            }
                                                        }
                                                        if (p.WindowType == "WindowStick")
                                                        {
                                                            popWidth = 1;
                                                            popHeight = 1;
                                                            remark = remark + "(需要分区确认尺寸)";
                                                        }
                                                        //popWidth = popWidth == 0 ? 1 : popWidth;
                                                        //popHeight = popHeight == 0 ? 1 : popHeight;

                                                        if (unitPrice == 0 && !string.IsNullOrWhiteSpace(material))
                                                        {
                                                            unitPrice = new BasePage().GetMaterialPrice(material);
                                                        }
                                                    }
                                                }


                                                finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                                finalOrderTempModel.AgentName = o.shop.AgentName;
                                                finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                                finalOrderTempModel.Channel = o.shop.Channel;
                                                finalOrderTempModel.City = o.shop.CityName;
                                                finalOrderTempModel.CityTier = o.shop.CityTier;
                                                finalOrderTempModel.Contact = o.shop.Contact1;
                                                finalOrderTempModel.Format = o.shop.Format;
                                                finalOrderTempModel.Gender = o.order.Gender;
                                                finalOrderTempModel.OrderGender = o.order.OrderGender;
                                                finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                                finalOrderTempModel.GraphicLength = popHeight;
                                                finalOrderTempModel.CustomerMaterialId = p.CustomerMaterialId ?? 0;
                                                finalOrderTempModel.GraphicMaterial = material;
                                                finalOrderTempModel.GraphicWidth = popWidth;
                                                if (popWidth > 0 && popHeight > 0)
                                                {
                                                    decimal area = (popHeight * popWidth) / 1000000;
                                                    finalOrderTempModel.Area = Math.Round(area, 6);
                                                }
                                                finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                                finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                                finalOrderTempModel.LocationType = o.shop.LocationType;
                                                finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                                finalOrderTempModel.OrderType = p.OrderType;
                                                finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                                finalOrderTempModel.POSScale = o.order.POSScale;
                                                finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                                                finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                                                //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                                                if (subjectCategoryName == "童店")   
                                                    finalOrderTempModel.InstallPriceMaterialSupport = "Others";
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

                                                //finalOrderTempModel.UnitPrice = p.RackSalePrice ?? 0;
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
                                                    finalOrderTempModel.IsValid = o.pop.IsValid;
                                                    finalOrderTempModel.ProduceOutsourceId = o.pop.ProduceOutsourceId;
                                                }

                                                decimal totalPrice = 0;
                                                POP popModel = new POP();
                                                popModel.GraphicLength = popHeight;
                                                popModel.GraphicWidth = popWidth;
                                                popModel.GraphicMaterial = material;
                                                popModel.Quantity = p.Quantity;
                                                popModel.PriceItemId = MaterialPriceId;
                                                unitPrice = new BasePage().GetMaterialUnitPrice(popModel, out totalPrice, out unitName);

                                                finalOrderTempModel.UnitPrice = unitPrice;
                                                finalOrderTempModel.TotalPrice = totalPrice;
                                                finalOrderTempModel.Remark = remark;
                                                finalOrderTempModel.IsSplit = true;
                                                if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                                                {
                                                    finalOrderTempModel.IsFromRegion = true;
                                                }
                                                finalOrderTempModel.ShopStatus = o.shop.Status;
                                                finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                                                finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                                finalOrderTempModel.AddDate = DateTime.Now;
                                                finalOrderTempModel.UnitName = unitName;
                                                bool isOk = true;
                                               
                                                if (isOk)
                                                {
                                                    finalOrderTempBll.Add(finalOrderTempModel);
                                                    FinishOrderList.Add(finalOrderTempModel);
                                                   
                                                }
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

                    }
                    #endregion
                    #region 以下是没有在方案的订单信息
                    var shopList1 = listOrder.Where(l => !finishSplitId.Contains(l.order.Id)).ToList();
                    if (shopList1.Any())
                    {
                        //MaterialPriceId = shopList1[0].PriceItemId ?? 0;
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
                            finalOrderTempModel.OrderGender = o.order.OrderGender;
                            finalOrderTempModel.GraphicNo = o.order.GraphicNo;


                            finalOrderTempModel.CustomerMaterialId = 0;

                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                            finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                            finalOrderTempModel.LocationType = o.shop.LocationType;
                            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                            finalOrderTempModel.OrderType = 1;//默认pop
                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                            finalOrderTempModel.POSScale = o.order.POSScale;
                            finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                            finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                            //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                            if (subjectCategoryName == "童店")   
                                finalOrderTempModel.InstallPriceMaterialSupport = "Others";
                            finalOrderTempModel.Province = o.shop.ProvinceName;
                            finalOrderTempModel.Quantity = o.order.Quantity;
                            finalOrderTempModel.Region = o.shop.RegionName;
                            finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                            //finalOrderTempModel.Remark = o.order.Remark;
                            finalOrderTempModel.SplitOrderRemark = "方案外";
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
                                
                                decimal area = (width * length) / 1000000;
                                decimal unitPrice = 0;
                                decimal totalPrice = 0;
                                if (!string.IsNullOrWhiteSpace(o.pop.GraphicMaterial))
                                {
                                    o.pop.Quantity = o.order.Quantity;
                                    o.pop.PriceItemId = MaterialPriceId;
                                    unitPrice = new BasePage().GetMaterialUnitPrice(o.pop, out totalPrice, out unitName);
                                }
                                finalOrderTempModel.GraphicMaterial = o.pop.GraphicMaterial;
                                finalOrderTempModel.GraphicLength = length;
                                finalOrderTempModel.GraphicWidth = width;
                                finalOrderTempModel.Area = Math.Round(area, 6);
                                finalOrderTempModel.UnitPrice = unitPrice;
                                finalOrderTempModel.PositionDescription = o.pop.PositionDescription;
                                finalOrderTempModel.POPName = o.pop.POPName;
                                finalOrderTempModel.POPType = o.pop.POPType;
                                finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                finalOrderTempModel.IsPOPMaterial = 1;
                                finalOrderTempModel.TotalPrice = totalPrice;
                                finalOrderTempModel.Remark = o.pop.Remark;
                                finalOrderTempModel.IsSplit = false;
                                finalOrderTempModel.IsValid = o.pop.IsValid;
                                finalOrderTempModel.ProduceOutsourceId = o.pop.ProduceOutsourceId;
                            }
                            finalOrderTempModel.ShopStatus = o.shop.Status;
                            finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                            finalOrderTempModel.CSUserId = o.shop.CSUserId;
                            finalOrderTempModel.UnitName = unitName;
                            finalOrderTempModel.AddDate = DateTime.Now;
                            if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                            {
                                finalOrderTempModel.IsFromRegion = true;
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

                    #region 物料信息

                    //
                    var materialOrderList = (from material in CurrentContext.DbContext.OrderMaterial
                                             join shop in CurrentContext.DbContext.Shop
                                             on material.ShopId equals shop.Id
                                             join subject in CurrentContext.DbContext.Subject
                                             on material.SubjectId equals subject.Id
                                             where material.SubjectId == subjectId
                                             select new
                                             {
                                                 subject,
                                                 material,
                                                 shop
                                             }).ToList();
                    if (materialOrderList.Any())
                    {
                        materialOrderList.ForEach(o =>
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
                            //finalOrderTempModel.Gender = o.order.Gender;
                            //finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                            finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                            finalOrderTempModel.LocationType = o.shop.LocationType;
                            //finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                            finalOrderTempModel.OrderType = (int)OrderTypeEnum.物料;
                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                            finalOrderTempModel.Province = o.shop.ProvinceName;
                            finalOrderTempModel.Quantity = o.material.MaterialCount;
                            finalOrderTempModel.Region = o.shop.RegionName;
                            //finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                            finalOrderTempModel.Sheet = o.material.MaterialName;
                            StringBuilder size = new StringBuilder();
                            if (o.material.MaterialLength != null && o.material.MaterialLength > 0 && o.material.MaterialWidth != null && o.material.MaterialWidth > 0)
                            {
                                size.AppendFormat("({0}*{1}", o.material.MaterialLength, o.material.MaterialWidth);
                                if (o.material.MaterialHigh != null && o.material.MaterialHigh > 0)
                                    size.AppendFormat("*{0}", o.material.MaterialHigh);
                                size.Append(")");
                            }
                            finalOrderTempModel.PositionDescription = size.ToString();
                            finalOrderTempModel.Remark = o.material.Remark;
                            finalOrderTempModel.ShopId = o.shop.Id;
                            finalOrderTempModel.ShopName = o.shop.ShopName;
                            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                            finalOrderTempModel.SubjectId = subjectId;
                            finalOrderTempModel.Tel = o.shop.Tel1;
                            finalOrderTempModel.MachineFrame = "";
                            if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                            {
                                finalOrderTempModel.IsFromRegion = true;
                            }
                            finalOrderTempModel.ShopStatus = o.shop.Status;
                            finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                            if ((o.material.Price ?? 0) > 0)
                            {
                                finalOrderTempModel.UnitPrice =o.material.Price;
                                finalOrderTempModel.OrderPrice = (o.material.Price ?? 0) * (o.material.MaterialCount ?? 0);
                                finalOrderTempModel.PayOrderPrice = (o.material.PayPrice ?? 0) * (o.material.MaterialCount ?? 0);
                            }
                            finalOrderTempModel.CSUserId = o.shop.CSUserId;
                            finalOrderTempModel.UnitName = unitName;
                            finalOrderTempModel.AddDate = DateTime.Now;
                            finalOrderTempBll.Add(finalOrderTempModel);
                            
                        });
                    }

                    #endregion




                    SubjectBLL subjectBll = new SubjectBLL();
                    //Models.Subject subjectModel = subjectBll.GetModel(subjectId);
                    if (subjectModel != null)
                    {
                        subjectModel.Status = 3;//拆单完成
                        subjectModel.SplitPlanIds = planIds;
                        subjectBll.Update(subjectModel);
                    }
                    CheckSplict();
                    tran.Complete();
                    return "ok";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

        }


        /// <summary>
        /// 获取辅料Id
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="frameName"></param>
        /// <returns></returns>
        /// 
        Dictionary<string, int> smDic = new Dictionary<string, int>();
        SmallMaterialMappingBLL smmBll = new SmallMaterialMappingBLL();
        int GetSmallMaterial(string sheet, string frameName)
        {
            int smId = 0;
            if (!string.IsNullOrWhiteSpace(frameName))
            {
                if (smDic.Keys.Contains(sheet + "-" + frameName))
                {
                    smId = smDic[sheet + "-" + frameName];
                }
                else
                {
                    var list = smmBll.GetList(s => s.Sheet == sheet && s.MachineFrame == frameName);
                    if (list.Any())
                    {
                        smId = list[0].SmallMaterialId ?? 0;
                        smDic.Add(sheet + "-" + frameName, smId);
                    }
                }
            }
            return smId;
        }

        /// <summary>
        /// 拆单的是，获取所选位置，器架为空的店铺数量
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        string GetEmptyFrameShopCount(int subjectId, string sheet)
        {
            int count = 0;
            List<int> shopIdList = new MergeOriginalOrderBLL().GetList(s => s.SubjectId == subjectId && s.Sheet.ToLower() == sheet.ToLower()).Select(s => s.ShopId ?? 0).Distinct().ToList();
            if (shopIdList.Any())
            {
                List<int> frameShopList = new ShopMachineFrameBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && s.PositionName.ToLower() == sheet.ToLower()).Select(s => s.ShopId ?? 0).Distinct().ToList();
                count = shopIdList.Count - frameShopList.Count;
            }
            return count.ToString();
        }

        /// <summary>
        /// 获取陈列桌包边尺寸
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        string GetTableSize(string sheet, string frame)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(frame))
            {
                List<string> frameList = StringHelper.ToStringList(frame, ',');

                int priceItemId = 0;
                var guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                     join guidance in CurrentContext.DbContext.SubjectGuidance
                                     on subject.GuidanceId equals guidance.ItemId
                                     where subject.Id == subjectId
                                     select guidance).FirstOrDefault();
                if (guidanceModel != null)
                    priceItemId = guidanceModel.PriceItemId ?? 0;
                var list = (from table in CurrentContext.DbContext.TableSize
                            join orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                            on table.OrderMaterialId equals orderMaterial.Id
                            join cm in CurrentContext.DbContext.CustomerMaterialInfo
                            on orderMaterial.BasicMaterialId equals cm.BasicMaterialId
                            //join bm in CurrentContext.DbContext.BasicMaterial
                            //on cm.BasicMaterialId equals bm.Id
                            where table.Sheet == sheet && frameList.Contains(table.MachineFrame)
                            && cm.PriceItemId == priceItemId
                            select new
                            {
                                table,
                                MaterialName = orderMaterial.OrderMaterialName,
                                cm.Price
                            }).OrderBy(s => s.table.MachineFrame).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        json.Append("{\"NormalLength\":\"" + s.table.NormalLength + "\",\"NormalWidth\":\"" + s.table.NormalWidth + "\",\"WithEdgeLength\":\"" + s.table.WithEdgeLength + "\",\"WithEdgeWidth\":\"" + s.table.WithEdgeWidth + "\",\"Quantity\":\"" + s.table.Quantity + "\",\"Remark\":\"" + s.table.Remark + "\",\"MaterialName\":\"" + s.MaterialName + "\",\"Price\":\"" + s.Price + "\"},");
                    });
                    result = "[" + json.ToString().TrimEnd(',') + "]";
                }
            }
            return result;
        }

        void CheckSplict()
        {
            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
            FinalOrderDetailTemp orderModel;
            
            var orderList = orderBll.GetList(s => s.SubjectId == subjectId && s.IsSplit == true && s.Sheet == "鞋墙");
            if (orderList.Any())
            {
                var standardList = orderList.Where(s => s.PositionDescription != null && s.PositionDescription.Contains("标配") && s.Sheet == "鞋墙").ToList();
                if (standardList.Any())
                {
                    standardList.ForEach(s => {
                        var list = orderList.Where(o => o.ShopId == s.ShopId && o.MachineFrame == s.MachineFrame && o.GraphicNo != null && o.GraphicNo != "" && s.Sheet == "鞋墙" && o.OrderGender==s.OrderGender);
                        if (list.Any())
                        {
                            orderModel = new FinalOrderDetailTemp();
                            orderModel = s;
                            orderBll.Delete(orderModel);
                        }
                    });
                }

               
            }
            var orderList1 = orderBll.GetList(s => s.SubjectId == subjectId && s.Sheet == "服装墙");
            var list2 = orderList1.Where(s => (s.GraphicNo == null || s.GraphicNo == "") && (s.GraphicLength != null && s.GraphicWidth != null && s.GraphicLength > 0 && s.GraphicWidth > 0)).ToList();
            if (list2.Any())
            {
                list2.ForEach(s =>
                {
                    var list = orderList1.Where(o => o.ShopId == s.ShopId && o.GraphicNo != null && o.GraphicNo != "" && (s.GraphicLength == o.GraphicLength && s.GraphicWidth == o.GraphicWidth && s.OrderGender == o.OrderGender));
                    if (list.Any())
                    {
                        orderModel = new FinalOrderDetailTemp();
                        orderModel = s;
                        orderBll.Delete(orderModel);
                    }
                });
            }
            //var orderlist = orderBll.GetList(s=>s.SubjectId==subjectId);
            //orderlist.ForEach(s => {
            //    if (s.OrderType == (int)OrderTypeEnum.物料)
            //    {
            //        new BasePage().SaveQuotationOrder(s, false);
            //    }
            //    else
            //    {
            //        new BasePage().SaveQuotationOrder(s);
            //    }
            //});

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
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
    /// SplitPlan 的摘要说明
    /// </summary>
    public class SplitPlan : IHttpHandler, IRequiresSessionState
    {
        HttpContext context1;
        ShopMachineFrameBLL frameBll = new ShopMachineFrameBLL();

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
            else if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            else if (context.Request.Form["customerId"] != null)
            {
                customerId = int.Parse(context.Request.Form["customerId"]);
            }
            if (context.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context.Request.QueryString["subjectId"]);
            }
            else if (context.Request.Form["subjectId"] != null)
            {
                customerId = int.Parse(context.Request.Form["subjectId"]);
            }
            switch (type)
            {
                case "initConditions":
                    result = GetConditions();
                    break;
                case "changeCondition":
                    result = ChangeCondition();
                    break;
                case "getCustomerMaterial":
                    result = GetCustomerMaterial();
                    break;
                case "getPlanList":
                    result = GetPlanList();
                    break;
                case "getPlanDetail":
                    result = GetDetailList();
                    break;
                case "add":
                    result = AddPlan();
                    break;
                case "deletePlan":
                    result = DeletePlan();
                    break;
                case "checkShopNo":
                    result = CheckShopNos();
                    break;
                case "getTableSize":
                    result = GetTableSize();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetConditions()
        {
            string shopNos = string.Empty;
            if (context1.Request.QueryString["shopNos"] != null)
            {
                shopNos = context1.Request.QueryString["shopNos"];
                shopNos = shopNos.Trim();
            }
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

            //List<string> material = new List<string>();
            //StringBuilder materialsb = new StringBuilder();

            List<string> POPSize = new List<string>();
            StringBuilder POPSizesb = new StringBuilder();

            //List<string> WindowSize = new List<string>();
            //StringBuilder WindowSizesb = new StringBuilder();

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
                newShopIds = GetShopId(shopNos, out notExist);
                hasShopNo = true;
            }
            if (hasShopNo)
            {
                if (!string.IsNullOrWhiteSpace(notExist))
                {
                    return "店铺编号：" + notExist.TrimEnd(',') + " 不存在或者不属于当前项目！";
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

                    if (!string.IsNullOrWhiteSpace(s.merge.OrderGender) && !gender.Contains(s.merge.OrderGender))
                    {
                        gender.Add(s.merge.OrderGender);

                    }


                    if (!string.IsNullOrWhiteSpace(s.shop.CityTier) && !cityTier.Contains(s.shop.CityTier))
                    {
                        cityTier.Add(s.shop.CityTier);

                    }

                    if (!string.IsNullOrWhiteSpace(s.shop.Format) && !format.Contains(s.shop.Format))
                    {
                        format.Add(s.shop.Format);

                    }



                    string support = !string.IsNullOrWhiteSpace(s.merge.MaterialSupport) ? s.merge.MaterialSupport : "空";
                    if (!materialSupport.Contains(support))
                    {
                        materialSupport.Add(support);

                    }

                    string scale = !string.IsNullOrWhiteSpace(s.merge.POSScale) ? s.merge.POSScale : "空";

                    if (!POSScale.Contains(scale))
                    {
                        POSScale.Add(scale);

                    }


                    string isinstall = !string.IsNullOrWhiteSpace(s.shop.IsInstall) ? s.shop.IsInstall : "空";
                    if (!install.Contains(isinstall))
                    {
                        install.Add(isinstall);

                    }

                    if (s.merge.Quantity != null && !count.Contains(s.merge.Quantity ?? 0))
                    {
                        count.Add(s.merge.Quantity ?? 0);

                    }
                    //if (s.pop != null && !string.IsNullOrWhiteSpace(s.pop.GraphicMaterial) && !material.Contains(s.pop.GraphicMaterial))
                    //{
                    //    material.Add(s.pop.GraphicMaterial);

                    //}

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

                        }
                    }
                    else
                    {
                        string popsize0 = "空";
                        if (!POPSize.Contains(popsize0))
                        {
                            POPSize.Add(popsize0);

                        }
                    }
                    //string windowsize = string.Empty;
                    //if (s.pop != null)
                    //{
                    //    if ((s.pop.WindowWide == null || s.pop.WindowWide == 0) || (s.pop.WindowHigh == null || s.pop.WindowHigh == 0) || (s.pop.WindowDeep == null || s.pop.WindowDeep == 0))
                    //    {
                    //        windowsize = "空";
                    //    }
                    //    else
                    //    {
                    //        windowsize = s.pop.WindowWide + "*" + s.pop.WindowHigh + "*" + s.pop.WindowDeep;
                    //    }
                    //    if (!WindowSize.Contains(windowsize))
                    //    {
                    //        WindowSize.Add(windowsize);

                    //    }
                    //}
                    //else
                    //{
                    //    string windowsize0 = "空";
                    //    if (!WindowSize.Contains(windowsize0))
                    //    {
                    //        WindowSize.Add(windowsize0);

                    //    }
                    //}
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

            //material.Sort();
            //material.ForEach(s =>
            //{
            //    materialsb.Append(s);
            //    materialsb.Append(',');
            //});
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
            //WindowSize.Sort();
            //WindowSize.ForEach(s =>
            //{
            //    WindowSizesb.Append(s);
            //    WindowSizesb.Append(',');
            //});
            json.Append("{\"RegionNames\":\"" + regionsb.ToString().TrimEnd(',') + "\",\"CityTier\":\"" + cityTiersb.ToString().TrimEnd(',') + "\",\"Format\":\"" + formatsb.ToString().TrimEnd(',') + "\",\"MaterialSupport\":\"" + mssb.ToString().TrimEnd(',') + "\",\"IsInstall\":\"" + installsb.ToString().TrimEnd(',') + "\",\"POSScale\":\"" + scalesb.ToString().TrimEnd(',') + "\",\"Sheet\":\"" + sheetsb.ToString().TrimEnd(',') + "\",\"Gender\":\"" + gendersb.ToString().TrimEnd(',') + "\",");

            json.Append("\"Quantity\":\"" + countsb.ToString().TrimEnd(',') + "\",\"POPSize\":\"" + POPSizesb.ToString().TrimEnd(',') + "\",\"ChooseImg\":\"" + ChooseImgsb.ToString().TrimEnd(',') + "\",\"IsElectricity\":\"" + IsElectricitysb.ToString().TrimEnd(',') + "\"}");

            return "[" + json.ToString() + "]";
        }

        string ChangeCondition()
        {
            string restult = string.Empty;

            string str = context1.Request.Form["jsonStr"];
            if (!string.IsNullOrWhiteSpace(str))
            {
                str = HttpUtility.UrlDecode(str);
            }
            if (!string.IsNullOrWhiteSpace(str))
            {
                OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(str);
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
                    else
                    {
                        shops = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                                         join shop in CurrentContext.DbContext.Shop
                                         on merge.ShopId equals shop.Id
                                         where merge.SubjectId == planModel.SubjectId
                                         && (merge.IsDelete == null || merge.IsDelete == false)
                                         select shop).ToList();
                        var shopids = shops.Select(s => s.Id).Distinct().ToList();
                        context1.Session["shoplist"] = shops;
                        orders = new MergeOriginalOrderBLL().GetList(s => s.SubjectId == planModel.SubjectId && (s.IsDelete == null || s.IsDelete == false));
                        pops = new POPBLL().GetList(s => shopids.Contains(s.ShopId ?? 0));
                        frameLists = frameBll.GetList(s => shopids.Contains(s.ShopId ?? 0));
                        context1.Session["orderlist"] = orders;
                        context1.Session["poplist"] = pops;
                        context1.Session["frames"] = frameLists;

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
                        shoplist = shoplist.Where(s => genderList.Contains(s.merge.OrderGender)).ToList();
                    }
                    else
                    {
                        genderList = shoplist.Select(s => s.merge.OrderGender).Distinct().ToList();
                    }
                    List<string> realGenderList = shoplist.Select(s => s.merge.Gender).Distinct().ToList();
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
                                List<int> frameShopList = frameLists.Where(f => f.PositionName == planModel.PositionName && list0.Contains(f.MachineFrame) && (genderList.Any() ? (genderList.Contains(f.Gender) || f.Gender.Contains("男") && f.Gender.Contains("女")) : 1 == 1)).Select(f => f.ShopId ?? 0).ToList();
                                //当前位置的所有器架类型的店铺
                                List<int> frameShopList1 = frameLists.Where(f => f.PositionName == planModel.PositionName && (realGenderList.Any() ? (realGenderList.Contains(f.Gender) || f.Gender.Contains("男") && f.Gender.Contains("女")) : 1 == 1)).Select(f => f.ShopId ?? 0).ToList();
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
                                List<int> frameShopList1 = frameLists.Where(f => f.PositionName == planModel.PositionName && (realGenderList.Any() ? (realGenderList.Contains(f.Gender) || f.Gender.Contains("男") && f.Gender.Contains("女")) : 1 == 1)).Select(f => f.ShopId ?? 0).ToList();
                                shoplist = shoplist.Where(l => !frameShopList1.Contains(l.merge.ShopId ?? 0)).ToList();
                            }

                        }
                        else
                        {
                            List<string> frames = StringHelper.ToStringList(planModel.MachineFrameNames, ',');
                            var frameShopList = frameLists.Where(f => f.PositionName.Trim() == planModel.PositionName.Trim() && frames.Contains(f.MachineFrame) && (realGenderList.Any() ? (realGenderList.Contains(f.Gender) || f.Gender.Contains("男") && f.Gender.Contains("女")) : 1 == 1)).Select(f => f.ShopId ?? 0).ToList();
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

                            }
                            if (!string.IsNullOrWhiteSpace(s.shop.CityName) && !city.Contains(s.shop.ProvinceName + "_" + s.shop.CityName))
                            {
                                city.Add(s.shop.ProvinceName + "_" + s.shop.CityName);

                            }
                            if (!string.IsNullOrWhiteSpace(s.merge.OrderGender) && !gender.Contains(s.merge.OrderGender))
                            {
                                gender.Add(s.merge.OrderGender);

                            }
                            if (!string.IsNullOrWhiteSpace(s.shop.CityTier) && !cityTier.Contains(s.shop.CityTier))
                            {
                                cityTier.Add(s.shop.CityTier);

                            }
                            if (!string.IsNullOrWhiteSpace(s.shop.Format) && !format.Contains(s.shop.Format))
                            {
                                format.Add(s.shop.Format);

                            }
                            string materialSupport1 = !string.IsNullOrWhiteSpace(s.merge.MaterialSupport) ? s.merge.MaterialSupport : "空";

                            if (!materialSupport.Contains(materialSupport1))
                            {
                                materialSupport.Add(materialSupport1);

                            }
                            string isinstall = !string.IsNullOrWhiteSpace(s.shop.IsInstall) ? s.shop.IsInstall : "空";
                            if (!install.Contains(isinstall))
                            {
                                install.Add(isinstall);

                            }
                            if (!string.IsNullOrWhiteSpace(s.merge.POSScale) && !POSScale.Contains(s.merge.POSScale))
                            {
                                POSScale.Add(s.merge.POSScale);

                            }

                            if (s.merge.Quantity != null && !count.Contains(s.merge.Quantity ?? 0))
                            {
                                count.Add(s.merge.Quantity ?? 0);

                            }
                            //if (s.pop != null && !string.IsNullOrWhiteSpace(s.pop.GraphicMaterial) && !material.Contains(s.pop.GraphicMaterial))
                            //{
                            //    material.Add(s.pop.GraphicMaterial);

                            //}

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

                                }
                            }
                            else
                            {
                                string popsize0 = "空";
                                if (!POPSize.Contains(popsize0))
                                {
                                    POPSize.Add(popsize0);

                                }
                            }
                            //string windowsize = string.Empty;
                            //if (s.pop != null)
                            //{
                            //    if ((s.pop.WindowWide == null || s.pop.WindowWide == 0) || (s.pop.WindowHigh == null || s.pop.WindowHigh == 0) || (s.pop.WindowDeep == null || s.pop.WindowDeep == 0))
                            //    {
                            //        windowsize = "空";
                            //    }
                            //    else
                            //    {
                            //        windowsize = s.pop.WindowWide + "*" + s.pop.WindowHigh + "*" + s.pop.WindowDeep;
                            //    }
                            //    if (!WindowSize.Contains(windowsize))
                            //    {
                            //        WindowSize.Add(windowsize);

                            //    }
                            //}
                            //else
                            //{
                            //    string windowsize0 = "空";
                            //    if (!WindowSize.Contains(windowsize0))
                            //    {
                            //        WindowSize.Add(windowsize0);

                            //    }
                            //}
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
                            //if (genderList.Any())
                            if (realGenderList.Any())
                            {

                                bool allGender = false;
                                realGenderList.ForEach(g =>
                                {
                                    if (g.Contains("男") && g.Contains("女"))
                                        allGender = true;
                                });
                                if (allGender)
                                {
                                    realGenderList = new List<string>() { "男", "女" };
                                }
                                framelist = framelist.Where(f => realGenderList.Contains(f.Gender) || (f.Gender.Contains("男") && f.Gender.Contains("女"))).ToList();
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
                        json.Append("{\"RegionNames\":\"" + regionsb.ToString().TrimEnd(',') + "\",\"ProvinceName\":\"" + provincesb.ToString().TrimEnd(',') + "\",\"CityName\":\"" + citysb.ToString().TrimEnd(',') + "\",\"CornerType\":\"" + cornerTypesb.ToString().TrimEnd(',') + "\",\"MachineFrameNames\":\"" + frameNamesb.ToString().TrimEnd(',') + "\",\"CityTier\":\"" + cityTiersb.ToString().TrimEnd(',') + "\",\"Format\":\"" + formatsb.ToString().TrimEnd(',') + "\",\"MaterialSupport\":\"" + mssb.ToString().TrimEnd(',') + "\",\"IsInstall\":\"" + installsb.ToString().TrimEnd(',') + "\",\"POSScale\":\"" + scalesb.ToString().TrimEnd(',') + "\",\"Gender\":\"" + gendersb.ToString().TrimEnd(',') + "\",");

                        json.Append("\"Quantity\":\"" + countsb.ToString().TrimEnd(',') + "\",\"POPSize\":\"" + POPSizesb.ToString().TrimEnd(',') + "\",\"ChooseImg\":\"" + ChooseImgsb.ToString().TrimEnd(',') + "\",\"IsElectricity\":\"" + IsElectricitysb.ToString().TrimEnd(',') + "\"}");
                        restult = "[" + json.ToString() + "]";
                    }
                    else
                    {
                        StringBuilder json1 = new StringBuilder();
                        json1.Append("{\"RegionNames\":\"\",\"ProvinceName\":\"\",\"CityName\":\"\",\"CornerType\":\"\",\"MachineFrameNames\":\"\",\"CityTier\":\"\",\"Format\":\"\",\"MaterialSupport\":\"\",\"IsInstall\":\"\",\"POSScale\":\"\",\"Gender\":\"\",");

                        json1.Append("\"Quantity\":\"\",\"POPSize\":\"\",\"ChooseImg\":\"\",\"IsElectricity\":\"\"}");
                        restult = "[" + json1.ToString() + "]";
                    }
                }
            }
            return restult;
        }

        string GetCustomerMaterial()
        {
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

        string GetPlanList()
        {
            try
            {
                var list = new OrderPlanBLL().GetList(s =>  (s.CustomerId == customerId && s.SubjectId == subjectId && s.PlanType == 1));
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
                        int isExcept = s.IsExcept == true ? 1 : 0;

                        json.Append("{\"Id\":\"" + s.Id + "\",\"RegionNames\":\"" + s.RegionNames + "\",\"ProvinceId\":\"" + s.ProvinceId + "\",\"CityId\":\"" + s.CityId + "\",\"ProvinceName\":\"" + ProvinceName + "\",\"CityName\":\"" + CityName + "\",\"CityTier\":\"" + s.CityTier + "\",\"IsInstall\":\"" + s.IsInstall + "\",\"Format\":\"" + s.Format + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"POSScale\":\"" + s.POSScale + "\",\"Gender\":\"" + s.Gender + "\",\"ShopNos\":\"" + s.ShopNos + "\",\"PositionId\":\"" + s.PositionId + "\",\"PositionName\":\"" + s.PositionName + "\",\"MachineFrameNames\":\"" + s.MachineFrameNames + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"POPSize\":\"" + s.POPSize + "\",\"ChooseImg\":\"" + s.ChooseImg + "\",\"CornerType\":\"" + s.CornerType + "\",\"KeepPOPSize\":\"" + keeppopsize + "\",\"IsElectricity\":\"" + s.IsElectricity + "\",\"NotInvolveShopNos\":\"" + s.NotInvolveShopNos + "\"},");

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
            int planId = 0;
            if (context1.Request.QueryString["planId"] != null)
            {
                planId = int.Parse(context1.Request.QueryString["planId"]);
            }
            var list = new SplitOrderPlanDetailBLL().GetList(s => s.PlanId == planId).OrderBy(s => s.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    string orderType = s.OrderType != null ? s.OrderType == 1 ? "pop" : "道具" : "pop";
                   
                    json.Append("{\"Id\":\"" + s.Id + "\",\"OrderType\":\"" + orderType + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"Quantity\":\"" + s.Quantity + "\",\"RackSalePrice\":\"" + s.RackSalePrice + "\",\"Remark\":\"" + s.Remark + "\",\"NewChooseImg\":\"" + s.NewChooseImg + "\",\"WindowType\":\"" + s.WindowType + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }


        string AddPlan()
        {
            OrderPlanBLL orderPlanBll = new OrderPlanBLL();
            SplitOrderPlanDetailBLL splitPlanBll = new SplitOrderPlanDetailBLL();
            CityInOrderPlanBLL planCityBll = new CityInOrderPlanBLL();
            CityInOrderPlan planCityModel;
            ProvinceInOrderPlanBLL planProvinceBll = new ProvinceInOrderPlanBLL();
            ProvinceInOrderPlan planProvinceModel;
            string optype = context1.Request.Form["optype"];
            string jsonstr = context1.Request.Form["jsonStr"];
            if (!string.IsNullOrWhiteSpace(jsonstr))
            {
                jsonstr = jsonstr.Replace("+", "%2B");

                jsonstr = HttpUtility.UrlDecode(jsonstr);

                jsonstr = jsonstr.Replace("乘", "×");
            }
            string result = "提交失败！";
            if (!string.IsNullOrWhiteSpace(jsonstr))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        OrderPlan planModel = JsonConvert.DeserializeObject<OrderPlan>(jsonstr);

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
                                            //if (splitModel.IsPerShop != true)
                                            //{
                                            //    perShopBll.Delete(p => p.Sheet == planModel.PositionName && p.SubjectId == planModel.SubjectId && p.Remark == s.Remark);
                                            //}
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
                                            planModel.ShopNos = planModel.ShopNos.Replace("，", ",").Replace(".", ",").Replace("。", ",").Replace("、", ",").TrimEnd(',');
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
                new OrderPlanBLL().Delete(s => idList.Contains(s.Id));
                new SplitOrderPlanDetailBLL().Delete(s => idList.Contains(s.PlanId ?? 0));
                return "ok";
            }

            return "error";
        }

        string CheckShopNos()
        {
            string shopNos = string.Empty;
            if (context1.Request.QueryString["shopNo"] != null)
            {
                shopNos = context1.Request.QueryString["shopNo"];
            }
            string msg = string.Empty;
            if (!string.IsNullOrWhiteSpace(shopNos))
            {
                GetShopId(shopNos, out msg);
            }
            if (!string.IsNullOrWhiteSpace(msg))
            {
                msg = "店铺编号：" + msg.TrimEnd(',') + " 不存在或者不属于当前项目！";
            }
            return msg;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        List<int> GetShopId(string shopNos, out string notExistShopNo)
        {
            notExistShopNo = string.Empty;
            List<int> ids = new List<int>();
            if (!string.IsNullOrWhiteSpace(shopNos))
            {
                shopNos = shopNos.Replace("，", ",").Replace(".", ",").Replace("。", ",").Replace("、", ",").TrimEnd(',');
                List<string> shopNoList = StringHelper.ToStringList(shopNos, ',').Distinct().ToList();
                ShopBLL shopBll = new ShopBLL();
                StringBuilder str = new StringBuilder();
                shopNoList.ForEach(s =>
                {
                    //var model = shopBll.GetList(l => l.ShopNo.ToUpper() == s.ToUpper()).FirstOrDefault();
                    var model = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 where shop.ShopNo.ToUpper() == s.ToUpper()
                                 && order.SubjectId == subjectId
                                 select shop).FirstOrDefault();
                    if (model != null)
                        ids.Add(model.Id);
                    else
                    {
                        str.Append(s);
                        str.Append(",");
                    }
                });
                notExistShopNo = str.ToString();
            }
            return ids;
        }


        CityInOrderPlanBLL planCityBll = new CityInOrderPlanBLL();
        ProvinceInOrderPlanBLL planProvinceBll = new ProvinceInOrderPlanBLL();
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



        string GetTableSize()
        {
            string sheetName = string.Empty;
            if (context1.Request.QueryString["sheet"] != null)
            {
                sheetName = context1.Request.QueryString["sheet"];
            }
            string frameName = string.Empty;
            if (context1.Request.QueryString["frameName"] != null)
            {
                frameName = context1.Request.QueryString["frameName"];
            }
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(frameName))
            {
                List<string> frameList = StringHelper.ToStringList(frameName, ',');

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
                            where table.Sheet == sheetName && frameList.Contains(table.MachineFrame)
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
    }
}
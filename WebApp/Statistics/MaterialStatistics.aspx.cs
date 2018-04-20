using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using Common;

namespace WebApp.Statistics
{
    public partial class MaterialStatistics : BasePage
    {
        int shopId;
        int subjectId;
        //是否查询
        string isSearch = string.Empty;
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        string guidanceIds = string.Empty;
        string subjectIds = string.Empty;
        string status = string.Empty;
        int subjectChannel = 0;
        string shopType = string.Empty;
        string customerServiceIds = string.Empty;
        string subjectCategory = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["shopId"] != null)
            {
                shopId = int.Parse(Request.QueryString["shopId"]);
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["isSearch"] != null)
            {
                isSearch = Request.QueryString["isSearch"];
            }
            if (Request.QueryString["regions"] != null)
            {
                region = Request.QueryString["regions"];
            }
            if (Request.QueryString["provinces"] != null)
            {
                province = Request.QueryString["provinces"];
            }
            if (Request.QueryString["citys"] != null)
            {
                city = Request.QueryString["citys"];
            }
            if (Request.QueryString["guidanceIds"] != null)
            {
                guidanceIds = Request.QueryString["guidanceIds"];
            }

            if (Request.QueryString["subjectIds"] != null)
            {
                subjectIds = Request.QueryString["subjectIds"];
            }
            if (Request.QueryString["status"] != null)
            {
                status = Request.QueryString["status"];
            }
            if (Request.QueryString["subjectChannel"] != null)
            {
                subjectChannel = int.Parse(Request.QueryString["subjectChannel"].ToString());
            }
            if (Request.QueryString["shopType"] != null)
            {
                shopType = Request.QueryString["shopType"];
            }
            if (Request.QueryString["customerServiceIds"] != null)
            {
                customerServiceIds = Request.QueryString["customerServiceIds"];
            }
            if (Request.QueryString["subjectCategory"] != null)
            {
                subjectCategory = Request.QueryString["subjectCategory"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            List<int> customerServiceList = new List<int>();
            if (!string.IsNullOrWhiteSpace(customerServiceIds))
            {
                customerServiceList = StringHelper.ToIntList(customerServiceIds, ',');
            }
            List<int> subjectCategoryList = new List<int>();
            List<string> regionList1 = new List<string>();
            List<string> provinceList1 = new List<string>();
            List<string> cityList1 = new List<string>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList1 = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            else
            {
                GetResponsibleRegion.ForEach(s =>
                {
                    regionList1.Add(s.ToLower());
                });

            }
            if (!string.IsNullOrWhiteSpace(subjectCategory))
            {
                subjectCategoryList = StringHelper.ToIntList(subjectCategory, ',');
            }
            List<FinalOrderDetailTemp> list = new List<FinalOrderDetailTemp>();
            if (subjectId > 0)
            {
                //list = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false) && ((s.OrderType == 1 && s.GraphicLength != null && s.GraphicLength > 0 && s.GraphicWidth != null && s.GraphicWidth > 0) || (s.OrderType == 2)));
                var orderList0 = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 
                                 where
                                 order.SubjectId == subjectId
                                 && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                                 && (order.IsDelete == null || order.IsDelete == false)
                                
                                 select new
                                 {
                                     order,
                                     shop,
                                     subject

                                 }).ToList();

                if (customerServiceList.Any())
                {
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            orderList0 = orderList0.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                        }
                        else
                        {
                            orderList0 = orderList0.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                        }
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s => customerServiceList.Contains(s.order.CSUserId ?? 0)).ToList();

                    }
                }
                list = orderList0.Select(s=>s.order).ToList();
            }
            else
            {
                List<int> guidanceList = new List<int>();
                List<int> subjectList = new List<int>();
                if (!string.IsNullOrWhiteSpace(guidanceIds))
                {
                    guidanceList = StringHelper.ToIntList(guidanceIds, ',');
                }
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectList = StringHelper.ToIntList(subjectIds, ',');

                }
                
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 //join guidance in CurrentContext.DbContext.SubjectGuidance
                                 //on subject.GuidanceId equals guidance.ItemId
                                 where (guidanceList.Any() ? guidanceList.Contains(order.GuidanceId ?? 0) : 1 == 1)
                                 && (subjectList.Any() ? subjectList.Contains(subject.Id) : 1 == 1)
                                 && (subject.IsDelete == null || subject.IsDelete == false)
                                 && subject.ApproveState == 1
                                 && (order.IsDelete == null || order.IsDelete == false)
                                 //&& ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                                 //&& (shop.Status == null || shop.Status == "" || shop.Status == "正常")
                                 //&& (regionList1.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList1.Contains(subject.PriceBlongRegion.ToLower()) : regionList1.Contains(order.Region.ToLower())) : 1 == 1)
                                 && (regionList1.Any() ?  regionList1.Contains(order.Region.ToLower()) : 1 == 1)
                                 
                                 select new
                                 {
                                     order,
                                     shop,
                                     subject

                                 }).ToList();
                if (subjectCategoryList.Any())
                {
                    if (subjectCategoryList.Contains(0))
                    {
                        subjectCategoryList.Remove(0);
                        if (subjectCategoryList.Any())
                        {
                            orderList = orderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderList = orderList.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();
                    }
                    else
                        orderList = orderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (subjectChannel == 1)
                {
                    //上海系统单
                    orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel == 2)
                {
                    //分区订单
                    orderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();
                }
                
                    if (customerServiceList.Any())
                    {
                        if (customerServiceList.Contains(0))
                        {
                            customerServiceList.Remove(0);
                            if (customerServiceList.Any())
                            {
                                orderList = orderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                            }
                            else
                            {
                                orderList = orderList.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                            }
                        }
                        else
                        {
                            orderList = orderList.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();

                        }
                    }
                

                list = orderList.Select(s => s.order).ToList();
            }

            if (!string.IsNullOrWhiteSpace(isSearch) && isSearch == "1")
            {
                var list1 = (from order in list
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                            
                             select new { order, shop, subject }).ToList();

                if (!string.IsNullOrWhiteSpace(shopType))
                {
                    List<string> shopTypeList = StringHelper.ToStringList(shopType, ',');
                    if (shopTypeList.Any())
                    {
                        if (shopTypeList.Contains("空"))
                        {
                            shopTypeList.Remove("空");
                            if (shopTypeList.Any())
                            {
                                list1 = list1.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                            }
                            else
                                list1 = list1.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                        }
                        else
                            list1 = list1.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status == "0")
                    {
                        list1 = list1.Where(s => s.shop.Status == null || s.shop.Status == "" || s.shop.Status == "正常").ToList();
                    }
                    else if (status == "1")
                    {
                        list1 = list1.Where(s => s.shop.Status != null && s.shop.Status.Contains("闭")).ToList();
                    }
                }


               
                if (regionList1.Any())
                {

                    //list1 = list1.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList1.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList1.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (!string.IsNullOrWhiteSpace(province))
                    {
                        provinceList1 = StringHelper.ToStringList(province, ',');
                        if (provinceList1.Any())
                        {
                            list1 = list1.Where(s => provinceList1.Contains(s.order.Province)).ToList();
                            if (!string.IsNullOrWhiteSpace(city))
                            {
                                cityList1 = StringHelper.ToStringList(city, ',');
                                if (cityList1.Any())
                                    list1 = list1.Where(s => provinceList1.Contains(s.order.Province) && cityList1.Contains(s.order.City)).ToList();
                            }
                        }

                    }
                    

                }

                list = list1.Select(s => s.order).ToList();
            }

           



            if (shopId > 0)
            {
                list = list.Where(s => s.ShopId == shopId).ToList();
            }
            List<MaterialClass> materialList = new List<MaterialClass>();
            
            //新的统计方法
            if (list.Any())
            {
                
                list.ForEach(s =>
                {
                    int Quantity = s.Quantity ?? 1;
                    decimal width = s.GraphicWidth ?? 0;
                    decimal length = s.GraphicLength ?? 0;
                    if (!string.IsNullOrWhiteSpace(s.GraphicMaterial))
                    {
                        string GraphicMaterial=s.GraphicMaterial;
                        MaterialClass mc = new MaterialClass();
                        if (GraphicMaterial.Contains("挂轴"))
                        {
                            int PriceItemId = (from subject in CurrentContext.DbContext.Subject
                                               join guidance in CurrentContext.DbContext.SubjectGuidance
                                               on subject.GuidanceId equals guidance.ItemId
                                               where subject.Id == (s.SubjectId ?? 0)
                                               select (guidance.PriceItemId ?? 0)).FirstOrDefault();
                            
                            string unitName = string.Empty;
                            if (GraphicMaterial.Contains("+挂轴") || GraphicMaterial.Contains("+上挂轴") || GraphicMaterial.Contains("+下挂轴"))
                            {

                                
                                string materialName1 = GraphicMaterial.Substring(0, GraphicMaterial.LastIndexOf('+'));
                                string materialName2 = GraphicMaterial.Replace(materialName1 + "+", "");//挂轴
                                
                                decimal unitPrice1 = GetMaterialPrice(PriceItemId, materialName1, out unitName);

                                decimal totalPrice1 = 0;
                                
                                if (unitName == "平米")
                                {
                                    totalPrice1 = ((width * length) / 1000000) * unitPrice1 * Quantity;
                                }
                                else if (unitName == "米")
                                {
                                    totalPrice1 = ((width/1000) * 2) * unitPrice1 * Quantity;
                                }
                                else
                                {
                                    totalPrice1 = unitPrice1 * Quantity;
                                }
                                mc.Area = (width * length) / 1000000 * Quantity;
                                mc.MaterialName = materialName1;
                                mc.Quantity = Quantity;
                                mc.UnitPrice = unitPrice1;
                                mc.TotalPrice = totalPrice1;
                                materialList.Add(mc);

                                //统计挂轴价格
                                mc = new MaterialClass();
                                unitPrice1 = GetMaterialPrice(PriceItemId, "挂轴", out unitName);
                                mc.MaterialName = materialName2;
                                mc.UnitPrice = unitPrice1;
                                mc.Area = (width / 1000) * 2 * Quantity;
                                if (materialName2.Contains("上挂轴") || materialName2.Contains("下挂轴"))
                                {
                                    mc.Area = (width / 1000) * Quantity;
                                }
                                totalPrice1 = mc.Area * unitPrice1;
                                mc.TotalPrice = totalPrice1;
                                mc.Quantity = Quantity;
                                materialList.Add(mc);
                            }
                            else
                            {
                                mc = new MaterialClass();
                                decimal unitPrice1 = GetMaterialPrice(PriceItemId, "挂轴", out unitName);
                                mc.MaterialName = GraphicMaterial;
                                mc.UnitPrice = unitPrice1;
                                mc.Area = (width / 1000) * 2 * Quantity;
                                
                                mc.TotalPrice = mc.Area * unitPrice1;
                                mc.Quantity = Quantity;
                                materialList.Add(mc);
                            }
                        }
                        else
                        {

                            mc.Area = (width * length) / 1000000 * Quantity;
                           
                            mc.MaterialName = s.GraphicMaterial;
                            mc.Quantity = Quantity;
                            mc.UnitPrice = s.UnitPrice ?? 0;
                            mc.TotalPrice = s.TotalPrice ?? 0;
                            materialList.Add(mc);
                        }
                    }
                });
            }
            var list0 = (from material in materialList
                         group material by new
                         {
                             material.MaterialName,
                             material.UnitPrice
                         } into g
                         select new
                         {
                             GraphicMaterial = g.Key.MaterialName,
                             UnitPrice = g.Key.UnitPrice,
                             Area = g.Sum(s => s.Area > 0 ? s.Area : 0),
                             Count = g.Sum(s => s.Quantity),
                             Price = g.Sum(s => s.TotalPrice)

                         }).ToList();
            gvPrice.DataSource = list0;
            gvPrice.DataBind();
        }
        decimal totalArea = 0;
        decimal totalPrice = 0;
        protected void gvPrice_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object areaObj = item.GetType().GetProperty("Area").GetValue(item, null);
                    decimal area = areaObj != null ? decimal.Parse(areaObj.ToString()) : 0;
                    object priceObj = item.GetType().GetProperty("Price").GetValue(item, null);
                    decimal price = priceObj != null ? decimal.Parse(priceObj.ToString()) : 0;

                    ((Label)e.Item.FindControl("labArea")).Text = area.ToString();
                    ((Label)e.Item.FindControl("labPrice")).Text = price > 0 ? Math.Round(price, 2).ToString() : "0";
                    //((Label)e.Item.FindControl("labPrice")).Text = price.ToString();
                    totalArea += area;
                    totalPrice += price;

                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("labTotalArea")).Text = totalArea.ToString();
                ((Label)e.Item.FindControl("labTotalPrice")).Text = Math.Round(totalPrice, 2).ToString();
            }
        }

        /// <summary>
        /// 获取材质价格和单位
        /// </summary>
        //Dictionary<string, Dictionary<decimal, string>> priceDic = new Dictionary<string, Dictionary<decimal, string>>();
        //decimal GetMaterialPrice(string materialName, out string unitName)
        //{
        //    unitName = string.Empty;
        //    decimal price = 0;
        //    if (priceDic.Keys.Contains(materialName.ToLower()))
        //    {
        //        Dictionary<decimal, string> dic = priceDic[materialName.ToLower()];
        //        int index = 0;
        //        foreach (KeyValuePair<decimal, string> item in dic)
        //        {
        //            if (index == 0)
        //            {
        //                unitName = item.Value;
        //                price = item.Key;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        string name = materialName.Replace("—", "-").Replace("（", "(").Replace("）", ")").ToLower();
        //        //var model = materialBll.GetList(s => s.MaterialName.ToLower() == name).FirstOrDefault();
        //        var model = (from orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
        //                     join customerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
        //                     on orderMaterial.CustomerMaterialId equals customerMaterial.Id
        //                     join basicM in CurrentContext.DbContext.BasicMaterial
        //                     on customerMaterial.BasicMaterialId equals basicM.Id
        //                     join unit in CurrentContext.DbContext.UnitInfo
        //                     on basicM.UnitId equals unit.Id
        //                     where orderMaterial.OrderMaterialName.ToLower() == name
        //                     select new
        //                     {
        //                         customerMaterial.Price,
        //                         unit.UnitName
        //                     }).FirstOrDefault();
        //        if (model != null)
        //        {
        //            price = model.Price ?? 0;
        //            unitName = model.UnitName;
        //            Dictionary<decimal, string> dic = new Dictionary<decimal, string>();
        //            dic.Add(price, unitName);
        //            priceDic.Add(materialName.ToLower(), dic);

        //        }
        //    }
        //    return price;
        //}
    }

    
}
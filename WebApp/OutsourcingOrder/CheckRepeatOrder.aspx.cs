using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;

namespace WebApp.OutsourcingOrder
{
    public partial class CheckRepeatOrder : BasePage
    {
        string material0 = "背胶PP+";
        string material1 = "雪弗板";
        string material3 = "蝴蝶支架";
        int guidanceId=0;
        string subjectIds = string.Empty;
        string regions = string.Empty;
        string provinces = string.Empty;
        string citys = string.Empty;
        string customerService = string.Empty;
        string cityTier = string.Empty;
        string isInstall = string.Empty;
        string channel = string.Empty;
        string format = string.Empty;
        string sheet = string.Empty;
        string shopNo = string.Empty;
        int ruanmo = 0;
        string materialAssign = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["guidanceId"]))
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["subjectIds"] != null)
            {
                subjectIds = Request.QueryString["subjectIds"];
            }
            if (Request.QueryString["regions"] != null)
            {
                regions = Request.QueryString["regions"];
            }
            if (Request.QueryString["provinces"] != null)
            {
                provinces = Request.QueryString["provinces"];
            }
            if (Request.QueryString["citys"] != null)
            {
                citys = Request.QueryString["citys"];
            }
            if (Request.QueryString["customerService"] != null)
            {
                customerService = Request.QueryString["customerService"];
            }
            if (Request.QueryString["cityTier"] != null)
            {
                cityTier = Request.QueryString["cityTier"];
            }
            if (Request.QueryString["isInstall"] != null)
            {
                isInstall = Request.QueryString["isInstall"];
            }
            if (Request.QueryString["ruanmo"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ruanmo"]))
            {
                ruanmo = int.Parse(Request.QueryString["ruanmo"]);
            }
            if (Request.QueryString["materialAssign"] != null)
            {
                materialAssign = Request.QueryString["materialAssign"];
            }
            if (Request.QueryString["channel"] != null)
            {
                channel = Request.QueryString["channel"];
            }
            if (Request.QueryString["format"] != null)
            {
                format = Request.QueryString["format"];
            }
            if (Request.QueryString["sheet"] != null)
            {
                sheet = Request.QueryString["sheet"];
            }
            if (Request.QueryString["shopNo"] != null)
            {
                shopNo = Request.QueryString["shopNo"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            List<int> subjectIdList = new List<int>();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();

            List<int> customerServiceList = new List<int>();
            List<string> cityTierList = new List<string>();
            List<string> isInstallList = new List<string>();
            List<string> channelList = new List<string>();
            List<string> formatList = new List<string>();
            List<string> sheetList = new List<string>();
            List<string> shopNoList = new List<string>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectIdList = StringHelper.ToIntList(subjectIds,',');
            }
            if (!string.IsNullOrWhiteSpace(regions))
            {
                regionList = StringHelper.ToStringList(regions,',', LowerUpperEnum.ToLower);
            }
            if (!string.IsNullOrWhiteSpace(provinces))
            {
                provinceList = StringHelper.ToStringList(provinces, ',');
            }
            if (!string.IsNullOrWhiteSpace(citys))
            {
                cityList = StringHelper.ToStringList(citys, ',');
            }

            if (!string.IsNullOrWhiteSpace(customerService))
            {
                customerServiceList = StringHelper.ToIntList(customerService, ',');
            }
            if (!string.IsNullOrWhiteSpace(cityTier))
            {
                cityTierList = StringHelper.ToStringList(cityTier, ',');
            }
            if (!string.IsNullOrWhiteSpace(isInstall))
            {
                isInstallList = StringHelper.ToStringList(isInstall, ',');
            }
            if (!string.IsNullOrWhiteSpace(channel))
            {
                channelList = StringHelper.ToStringList(channel, ',');
            }
            if (!string.IsNullOrWhiteSpace(format))
            {
                formatList = StringHelper.ToStringList(format, ',');
            }
            if (!string.IsNullOrWhiteSpace(sheet))
            {
                sheetList = StringHelper.ToStringList(sheet, ',');
            }
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                shopNoList = StringHelper.ToStringList(shopNo.Replace("，", ","), ',', LowerUpperEnum.ToLower);
            }
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance.ItemId
                        //join category1 in CurrentContext.DbContext.ADSubjectCategory
                        // on subject.SubjectCategoryId equals category1.Id into temp1
                        //from category in temp1.DefaultIfEmpty()
                        where
                        guidance.ItemId == guidanceId
                        && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 3)

                        && (order.IsValid == null || order.IsValid == true)
                        && (order.IsValidFromAssign == null || order.IsValidFromAssign == true)
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                        //&& (myRegionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? myRegionList.Contains(subject.PriceBlongRegion.ToLower()) : myRegionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                        && (order.OrderType != (int)OrderTypeEnum.物料)
                        select new
                        {
                            order,
                            shop

                        }).ToList();

            if (subjectIdList.Any())
            {
                list = list.Where(s => subjectIdList.Contains(s.order.SubjectId??0)).ToList();
            }
            //if (regionList.Any())
            //{
            //    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            //}
            //if (provinceList.Any())
            //{
            //    list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            //}
            //if (cityList.Any())
            //{
            //    list = list.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            //}
            if (regionList.Any())
            {

                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region.ToLower()) || s.order.Region == null || s.order.Region == "").ToList();

                    }
                    else
                    {
                        list = list.Where(s => s.order.Region == null || s.order.Region == "").ToList();

                    }
                }
                else
                {
                    list = list.Where(s => regionList.Contains(s.order.Region.ToLower())).ToList();

                }
            }
            if (provinceList.Any())
            {

                if (provinceList.Contains("空"))
                {
                    provinceList.Remove("空");
                    if (provinceList.Any())
                    {
                        list = list.Where(s => provinceList.Contains(s.order.Province) || s.order.Province == null || s.order.Province == "").ToList();

                    }
                    else
                    {
                        list = list.Where(s => s.order.Province == null || s.order.Province == "").ToList();

                    }
                }
                else
                {
                    list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();

                }
            }
            if (cityList.Any())
            {
                if (cityList.Contains("空"))
                {
                    cityList.Remove("空");
                    if (cityList.Any())
                    {
                        list = list.Where(s => cityList.Contains(s.order.City) || s.order.City == null || s.order.City == "").ToList();

                    }
                    else
                    {
                        list = list.Where(s => s.order.City == null || s.order.City == "").ToList();

                    }
                }
                else
                {
                    list = list.Where(s => cityList.Contains(s.order.City)).ToList();

                }
            }
            if (customerServiceList.Any())
            {
                if (customerServiceList.Contains(0))
                {
                    customerServiceList.Remove(0);
                    if (customerServiceList.Any())
                    {
                        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                    }
                    else
                        list = list.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();
                }
                else
                {
                    list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
                }
            }
            if (cityTierList.Any())
            {
                if (cityTierList.Contains("空"))
                {
                    cityTierList.Remove("空");
                    if (cityTierList.Any())
                    {
                        list = list.Where(s => cityTierList.Contains(s.order.CityTier) || (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                    }
                    else
                        list = list.Where(s => (s.order.CityTier == null || s.order.CityTier == "")).ToList();
                }
                else
                {
                    list = list.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                }
            }
            if (isInstallList.Any())
            {
                if (isInstallList.Contains("空"))
                {
                    isInstallList.Remove("空");
                    if (isInstallList.Any())
                    {
                        list = list.Where(s => isInstallList.Contains(s.order.IsInstall) || (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                    }
                    else
                        list = list.Where(s => (s.order.IsInstall == null || s.order.IsInstall == "")).ToList();
                }
                else
                {
                    list = list.Where(s => isInstallList.Contains(s.order.IsInstall)).ToList();
                }
            }
            if (channelList.Any())
            {
                if (channelList.Contains("空"))
                {
                    channelList.Remove("空");
                    if (channelList.Any())
                    {
                        list = list.Where(s => channelList.Contains(s.order.Channel) || (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                    else
                    {
                        list = list.Where(s => (s.order.Channel == null || s.order.Channel == "")).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => channelList.Contains(s.order.Channel)).ToList();

                }
            }
            if (formatList.Any())
            {
                if (formatList.Contains("空"))
                {
                    formatList.Remove("空");
                    if (formatList.Any())
                    {
                        list = list.Where(s => formatList.Contains(s.order.Format) || (s.order.Format == null || s.order.Format == "")).ToList();

                    }
                    else
                    {
                        list = list.Where(s => (s.order.Format == null || s.order.Format == "")).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => formatList.Contains(s.order.Format)).ToList();

                }
            }

            if (sheetList.Any())
            {
                if (sheetList.Contains("空"))
                {
                    sheetList.Remove("空");
                    if (sheetList.Any())
                    {
                        list = list.Where(s => sheetList.Contains(s.order.Sheet) || (s.order.Sheet == null || s.order.Sheet == "")).ToList();

                    }
                    else
                    {
                        list = list.Where(s => (s.order.Sheet == null || s.order.Sheet == "")).ToList();

                    }
                }
                else
                {
                    list = list.Where(s => sheetList.Contains(s.order.Sheet)).ToList();

                }
            }
            if (shopNoList.Any())
            {
                list = list.Where(s => shopNoList.Contains(s.order.ShopNo.ToLower())).ToList();
            }
            if (ruanmo > 0)
            {
                if (ruanmo == 1)
                {
                    //只查询软膜的订单
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                }
                else if (ruanmo == 2)
                {
                    list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(materialAssign))
            {
                if (materialAssign == "非背胶雪弗板")
                {
                    list = list.Where(s => (s.order.GraphicMaterial != null && !(s.order.GraphicMaterial.Contains(material0) && s.order.GraphicMaterial.Contains(material1))) || s.order.GraphicMaterial == null || s.order.GraphicMaterial == "").ToList();


                }
                else
                   list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.ToLower().Contains(material0.ToLower()) && s.order.GraphicMaterial.ToLower().Contains(material1.ToLower())).ToList();

            }
            List<int> idList = new List<int>();
            var orderList = list.Where(s => s.order.GraphicNo != null && s.order.GraphicNo != "" && s.order.GraphicLength != null && s.order.GraphicWidth != null).ToList();
            var newList = from order in orderList
                          group order by new { order.order.ShopId, order.order.Sheet, order.order.GraphicNo, order.order.GraphicLength, order.order.GraphicWidth }
                              into g
                              where g.Count() > 1
                              select new
                              {
                                  orderIdList=g.Select(s=>s.order.Id).ToList()
                              };
            if (newList.Any())
            {
               
                newList.ToList().ForEach(s => {
                    idList.AddRange(s.orderIdList);
                });

            }
            var orderList1 = (from order in list
                             join subject in CurrentContext.DbContext.Subject
                             on order.order.SubjectId equals subject.Id
                             where idList.Contains(order.order.Id)
                             select new { 
                                subject.SubjectName,
                                order.order,
                                order.order.IsProduce
                             }).OrderBy(s=>s.order.ShopId).ThenBy(s=>s.order.Sheet).ToList();
            if (!IsPostBack)
            {
                List<string> sheet0List = orderList1.Select(s=>s.order.Sheet).Distinct().ToList();
                if (sheet0List.Any())
                {
                    sheet0List.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s;
                        li.Text = s + "&nbsp;";
                        cblSheet.Items.Add(li);
                    });
                }
            }
            List<string> selectSheetList = new List<string>();
            foreach (ListItem li in cblSheet.Items)
            {
                if (li.Selected)
                {
                    selectSheetList.Add(li.Value);
                }
            }
            if (selectSheetList.Any())
            {
                orderList1 = orderList1.Where(s => selectSheetList.Contains(s.order.Sheet)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                orderList1 = orderList1.Where(s => s.order.ShopNo.ToLower().Contains(txtShopNo.Text.ToLower())).ToList();
            }
            rp_orderList.DataSource = orderList1;
            rp_orderList.DataBind();
        }

        protected void btnSearchServer_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void cblSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void rp_orderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object dataItem = e.Item.DataItem;
                if (dataItem != null)
                {
                    object objIsProduce = dataItem.GetType().GetProperty("IsProduce").GetValue(dataItem, null);
                    bool IsProduce = objIsProduce != null ? bool.Parse(objIsProduce.ToString()) : true;
                    Label labState = (Label)e.Item.FindControl("labState");
                    
                    if (IsProduce)
                    {
                        labState.Text = "正常";
                    }
                    else
                    {
                        labState.Text = "不生产";
                        labState.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }

        protected void btnNoProduce_Click(object sender, EventArgs e)
        {
            List<int> orderIdList = new List<int>();
            foreach (RepeaterItem item in rp_orderList.Items)
            {
                if (item.ItemIndex != -1)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbOne");
                    if (cb.Checked)
                    {
                        HiddenField hcOrderId = (HiddenField)item.FindControl("hcOrderId");
                        if (!string.IsNullOrWhiteSpace(hcOrderId.Value))
                           orderIdList.Add(int.Parse(hcOrderId.Value));
                    }
                }
            }
            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
            var list = orderBll.GetList(s => orderIdList.Contains(s.Id)).ToList();
            if (list.Any())
            {
                list.ForEach(s => {
                    s.IsProduce = false;
                    orderBll.Update(s);
                });
                BindData();
            }
        }

        protected void btnProduce_Click(object sender, EventArgs e)
        {
            List<int> orderIdList = new List<int>();
            foreach (RepeaterItem item in rp_orderList.Items)
            {
                if (item.ItemIndex != -1)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbOne");
                    if (cb.Checked)
                    {
                        HiddenField hcOrderId = (HiddenField)item.FindControl("hcOrderId");
                        if (!string.IsNullOrWhiteSpace(hcOrderId.Value))
                            orderIdList.Add(int.Parse(hcOrderId.Value));
                    }
                }
            }
            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
            var list = orderBll.GetList(s => orderIdList.Contains(s.Id)).ToList();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    s.IsProduce = true;
                    orderBll.Update(s);
                });
                BindData();
            }
        }


    }
}
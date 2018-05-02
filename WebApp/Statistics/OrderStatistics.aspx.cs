using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.Statistics
{
    public partial class OrderStatistics : BasePage
    {
        InstallBLL installBll = new InstallBLL();
        List<int> searchSubjectIdList = new List<int>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //UpdateOrderUnitPrice();
                if (Session["shopTypeSelected"] != null)
                    Session["shopTypeSelected"] = null;
                if (Session["userSelected"] != null)
                    Session["userSelected"] = null;
                if (Session["subjectCategorySelected"] != null)
                    Session["subjectCategorySelected"] = null;
                if (Session["priceSubjectSelected"] != null)
                    Session["priceSubjectSelected"] = null;
                if (Session["subjectSelected"] != null)
                    Session["subjectSelected"] = null;
                if (Session["secondInstallSubjectSelected"] != null)
                    Session["secondInstallSubjectSelected"] = null;
                if (Session["provinceSelected"] != null)
                    Session["provinceSelected"] = null;
                if (Session["citySelected"] != null)
                    Session["citySelected"] = null;


                if (Session["orderDetailStatistics"] != null)
                    Session["orderDetailStatistics"] = null;
                if (Session["shopStatistics"] != null)
                    Session["shopStatistics"] = null;
                if (Session["subjectStatistics"] != null)
                    Session["subjectStatistics"] = null;
                if (Session["guidanceStatistics"] != null)
                    Session["guidanceStatistics"] = null;


                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;


                BindGuidance();
                BindRegion();
                gvList.DataSource = new List<Subject>();
                gvList.DataBind();
            }
        }

        void InitStatisticsValue()
        {
            Panel1.Visible = false;
            labSubjectCount.Text = "0";
            labPOPPrice.Text = "0";
            labPOPPrice.Attributes.Remove("style");
            labPOPPrice.Attributes.Remove("name");

            labShopCount.Text = "0";
            labShopCount.Attributes.Remove("style");
            labShopCount.Attributes.Remove("name");

            labInstallPrice.Text = "0";
            labInstallPrice.Attributes.Remove("style");
            labInstallPrice.Attributes.Remove("name");

            labMaterialPrice.Text = "0";
            labMaterialPrice.Attributes.Remove("style");
            labMaterialPrice.Attributes.Remove("name");

            labNewShopInstallPrice.Text = "0";
            labNewShopInstallPrice.Attributes.Remove("style");
            labNewShopInstallPrice.Attributes.Remove("name");


            labShopCount1.Text = "0";
            labShopCount1.Attributes.Remove("style");
            labShopCount1.Attributes.Remove("name");


            labPOPPrice1.Text = "0";
            labPOPPrice1.Attributes.Remove("style");
            labPOPPrice1.Attributes.Remove("name");


            labInstallPrice1.Text = "0";
            labInstallPrice1.Attributes.Remove("style");
            labInstallPrice1.Attributes.Remove("name");

            labMaterialPrice1.Text = "0";
            labMaterialPrice1.Attributes.Remove("style");
            labMaterialPrice1.Attributes.Remove("name");
        }

        void LoadSessionData()
        {
            List<int> guidanceIdList = GetGuidanceSelected();
            List<FinalOrderDetailTemp> finalOrderDetailTempList = new FinalOrderDetailTempBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false));

            var orderList = (from order in finalOrderDetailTempList
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on order.GuidanceId equals guidance.ItemId
                             where (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1

                             select new { order, shop, subject, guidance }).ToList();
            if (orderList.Any())
            {
                Session["orderDetailStatistics"] = orderList.Select(s => s.order).ToList();
                Session["shopStatistics"] = orderList.Select(s => s.shop).Distinct().ToList();
                List<Subject> subjectList = orderList.Select(s => s.subject).Distinct().ToList();
                List<int> subjectIdList = subjectList.Select(s=>s.Id).ToList();
                List<Subject> subjectList1 = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && !subjectIdList.Contains(s.Id));
                if (subjectList1.Any())
                {
                    subjectList.AddRange(subjectList1);
                }
                Session["subjectStatistics"] = subjectList;
                Session["guidanceStatistics"] = orderList.Select(s => s.guidance).Distinct().ToList();
            }
            else
            {
                Session["orderDetailStatistics"] = null;
                Session["shopStatistics"] = null;
                Session["subjectStatistics"] = null;
                Session["guidanceStatistics"] = null;
            }
        }

        void BindGuidance(int? onDateSearch = null)
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);

            // var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId).ToList();
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        //join type in CurrentContext.DbContext.ADOrderActivity
                        //on guidance.ActivityTypeId equals type.ActivityId
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance
                            //type
                        }).Distinct().ToList();


            if (onDateSearch != null)
            {
                string begin = txtGuidanceBegin.Text;
                string end = txtGuidanceEnd.Text;
                if (!string.IsNullOrWhiteSpace(begin) && !string.IsNullOrWhiteSpace(end))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    DateTime endDate = DateTime.Parse(end).AddDays(1);

                    list = list.Where(s => s.guidance.BeginDate >= beginDate && s.guidance.BeginDate < endDate).ToList();
                }
            }
            else
            {


                string guidanceMonth = txtGuidanceMonth.Text;
                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                {
                    DateTime date = DateTime.Parse(guidanceMonth);
                    int year = date.Year;
                    int month = date.Month;
                    list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();

                }

            }

            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    cblGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyGuidance.Visible = !list.Any();
        }

        void BindShopType()
        {
            cblShopType.Items.Clear();
            //List<int> guidanceIdList = GetGuidanceSelected();
            //var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //                 join subject in CurrentContext.DbContext.Subject
            //                 on order.SubjectId equals subject.Id
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on order.ShopId equals shop.Id
            //                 where guidanceIdList.Contains(subject.GuidanceId ?? 0)
            //                 && (subject.IsDelete == null || subject.IsDelete == false)
            //                 && subject.ApproveState == 1
            //                 && (order.IsDelete == null || order.IsDelete == false)
            //                 select new { order, shop }).ToList();
            int subjectChannel1 = 0;
            if (Session["orderDetailStatistics"] == null)
            {
                LoadSessionData();
            }
            List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();
            if (Session["orderDetailStatistics"] != null)
                finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
            if (Session["shopStatistics"] != null)
                shopList = Session["shopStatistics"] as List<Shop>;

            foreach (ListItem li in cblSubjectChannel.Items)
            {
                if (li.Selected)
                {
                    subjectChannel1 = int.Parse(li.Value);
                    break;
                }
            }
            if (finalOrderDetailTempList.Any())
            {
                var orderList = (from order in finalOrderDetailTempList
                                 join shop1 in shopList
                                 on order.ShopId equals shop1.Id into temp
                                 from shop in temp.DefaultIfEmpty()
                                 select new
                                 {
                                     order,
                                     ShopType = shop != null ? shop.ShopType : ""
                                 }).ToList();
                if (subjectChannel1 == 1)
                {
                    //上海系统单
                    orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel1 == 2)
                {
                    //分区订单
                    orderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (orderList.Any())
                {
                    bool isNull = false;
                    List<string> selectedList = new List<string>();
                    if (Session["shopTypeSelected"] != null)
                    {
                        selectedList = Session["shopTypeSelected"] as List<string>;
                    }
                    //var shopList = orderList.Select(s => s.shop).Distinct().ToList();
                    orderList.Select(s => s.ShopType).Distinct().OrderBy(s => s).ToList().ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            ListItem li = new ListItem();
                            li.Text = s + "&nbsp;&nbsp;";
                            li.Value = s;
                            if (selectedList.Contains(s))
                                li.Selected = true;
                            cblShopType.Items.Add(li);
                        }
                        else
                            isNull = true;
                    });
                    if (isNull)
                    {
                        cblShopType.Items.Add(new ListItem("空", "空"));
                    }
                }
            }
        }

        void BindAddUser()
        {
            //int customerId = int.Parse(ddlCustomer.SelectedValue);
            cblAddUser.Items.Clear();
            //List<int> guidanceIdList = GetGuidanceSelected();
            //var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //                 join subject in CurrentContext.DbContext.Subject
            //                 on order.SubjectId equals subject.Id
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on order.ShopId equals shop.Id
            //                 join user in CurrentContext.DbContext.UserInfo
            //                 on subject.AddUserId equals user.UserId
            //                 where guidanceIdList.Contains(subject.GuidanceId ?? 0)
            //                 && (subject.IsDelete == null || subject.IsDelete == false)
            //                 && subject.ApproveState == 1
            //                 && (order.IsDelete == null || order.IsDelete == false)
            //                 select new { order, shop, user }).ToList();
            if (Session["orderDetailStatistics"] == null)
            {
                LoadSessionData();
            }
            List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailStatistics"] != null)
                finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
            if (Session["shopStatistics"] != null)
                shopList = Session["shopStatistics"] as List<Shop>;
            if (Session["subjectStatistics"] != null)
                subjectList = Session["subjectStatistics"] as List<Subject>;
            if (finalOrderDetailTempList.Any())
            {
                var orderList = (from order in finalOrderDetailTempList
                                 join shop in shopList
                                 on order.ShopId equals shop.Id
                                 join subject in subjectList
                                 on order.SubjectId equals subject.Id
                                 join user in CurrentContext.DbContext.UserInfo
                                 on subject.AddUserId equals user.UserId
                                 select new
                                 {
                                     order,
                                     shop,
                                     user
                                 }).ToList();

                int subjectChannel1 = 0;
                foreach (ListItem li in cblSubjectChannel.Items)
                {
                    if (li.Selected)
                    {
                        subjectChannel1 = int.Parse(li.Value);
                        break;
                    }
                }
                List<string> shopTypeList = new List<string>();
                foreach (ListItem li in cblShopType.Items)
                {
                    if (li.Selected)
                    {
                        shopTypeList.Add(li.Value);
                    }
                }

                if (subjectChannel1 == 1)
                {
                    //上海系统单
                    orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel1 == 2)
                {
                    //分区订单
                    orderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (shopTypeList.Any())
                {
                    if (shopTypeList.Contains("空"))
                    {
                        shopTypeList.Remove("空");
                        if (shopTypeList.Any())
                        {
                            orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                    }
                    else
                        orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                }

                var userList = orderList.Select(s => s.user).Distinct().ToList();
                if (userList.Any())
                {
                    List<int> selectedList = new List<int>();
                    if (Session["userSelected"] != null)
                    {
                        selectedList = Session["userSelected"] as List<int>;
                    }
                    userList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s.RealName + "&nbsp;&nbsp;";
                        li.Value = s.UserId.ToString();
                        if (selectedList.Contains(s.UserId))
                            li.Selected = true;
                        cblAddUser.Items.Add(li);
                    });
                }
            }
        }

        void BindSubjectCategory()
        {
            cblSubjectCategory.Items.Clear();
            //List<int> guidanceIdList = GetGuidanceSelected();

            //var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //                 join subject in CurrentContext.DbContext.Subject
            //                 on order.SubjectId equals subject.Id
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on order.ShopId equals shop.Id
            //                 join category1 in CurrentContext.DbContext.ADSubjectCategory
            //                 on subject.SubjectCategoryId equals category1.Id into categoryTemp
            //                 from category in categoryTemp.DefaultIfEmpty()
            //                 where guidanceIdList.Contains(subject.GuidanceId ?? 0)
            //                 && (subject.IsDelete == null || subject.IsDelete == false)
            //                 && subject.ApproveState == 1
            //                 && (order.IsDelete == null || order.IsDelete == false)
            //                 select new
            //                 {
            //                     shop,
            //                     order,
            //                     subject.AddUserId,
            //                     category
            //                 }).ToList();
            if (Session["orderDetailStatistics"] == null)
            {
                LoadSessionData();
            }
            List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailStatistics"] != null)
                finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
            if (Session["shopStatistics"] != null)
                shopList = Session["shopStatistics"] as List<Shop>;
            if (Session["subjectStatistics"] != null)
                subjectList = Session["subjectStatistics"] as List<Subject>;

            var orderList = (from order in finalOrderDetailTempList
                             join shop in shopList
                             on order.ShopId equals shop.Id
                             join subject in subjectList
                             on order.SubjectId equals subject.Id
                             join user in CurrentContext.DbContext.UserInfo
                             on subject.AddUserId equals user.UserId
                             join category1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals category1.Id into categoryTemp
                             from category in categoryTemp.DefaultIfEmpty()
                             select new
                             {
                                 order,
                                 shop,
                                 subject.AddUserId,
                                 category
                             }).ToList();

            if (orderList.Any())
            {
                int subjectChannel1 = 0;
                foreach (ListItem li in cblSubjectChannel.Items)
                {
                    if (li.Selected)
                    {
                        subjectChannel1 = int.Parse(li.Value);
                        break;
                    }
                }
                List<string> shopTypeList = new List<string>();
                foreach (ListItem li in cblShopType.Items)
                {
                    if (li.Selected)
                    {
                        shopTypeList.Add(li.Value);
                    }
                }
                List<int> addUsers = new List<int>();
                foreach (ListItem li in cblAddUser.Items)
                {
                    if (li.Selected)
                    {
                        addUsers.Add(int.Parse(li.Value));
                    }
                }
                if (subjectChannel1 == 1)
                {
                    //上海系统单
                    orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel1 == 2)
                {
                    //分区订单
                    orderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (shopTypeList.Any())
                {
                    if (shopTypeList.Contains("空"))
                    {
                        shopTypeList.Remove("空");
                        if (shopTypeList.Any())
                        {
                            orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                    }
                    else
                        orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                }

                if (addUsers.Any())
                {
                    orderList = orderList.Where(s => addUsers.Contains(s.AddUserId ?? 0)).ToList();
                }

                if (orderList.Any())
                {
                    var categoryList = orderList.Select(s => s.category).Distinct().ToList();
                    List<int> selectedList = new List<int>();
                    if (Session["subjectCategorySelected"] != null)
                    {
                        selectedList = Session["subjectCategorySelected"] as List<int>;
                    }
                    bool isNull = false;
                    categoryList.ForEach(s =>
                    {
                        if (s != null)
                        {
                            ListItem li = new ListItem();
                            li.Text = s.CategoryName + "&nbsp;&nbsp;";
                            li.Value = s.Id.ToString();
                            if (selectedList.Contains(s.Id))
                                li.Selected = true;
                            cblSubjectCategory.Items.Add(li);
                        }
                        else
                            isNull = true;
                    });
                    if (isNull)
                    {
                        cblSubjectCategory.Items.Add(new ListItem("空", "0"));
                    }
                }
            }
        }

        List<string> regionList = new List<string>();
        List<string> provinceList = new List<string>();
        List<string> cityList = new List<string>();
        List<int> addUserList = new List<int>();
        List<int> subjectCategoryList = new List<int>();
        List<string> shopTypeList = new List<string>();
        List<int> customerServiceIds = new List<int>();

        List<int> guidanceIdList = new List<int>();
        List<int> subjectIdList = new List<int>();
        List<int> priceSubjectIdList = new List<int>();

        int subjectChannel = 0;//订单渠道
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //StringBuilder subjectIds = new StringBuilder();
        protected void Button1_Click(object sender, EventArgs e)
        {
            InitStatisticsValue();
            Statistic();
            ShowSubjectList();
        }

        void CleanLab()
        {
            labShopCount.Text = "0";
            labShopCount.Attributes.Remove("style");
            labShopCount.Attributes.Remove("name");

            labArea.Text = "0";

            labPOPPrice.Text = "0";
            labPOPPrice.Attributes.Remove("style");
            labPOPPrice.Attributes.Remove("name");

            labExpressPrice.Text = "0";
            labExpressPrice.Attributes.Remove("style");
            labExpressPrice.Attributes.Remove("name");

            labInstallPrice.Text = "0";
            labInstallPrice.Attributes.Remove("style");
            labInstallPrice.Attributes.Remove("name");

            labFreight.Text = "0";

            labMeasurePrice.Text = "0";
            labRegionInstallPrice.Text = "0";
            labRegionExpressPrice.Text = "0";
            labRegionOtherPrice.Text = "0";
            labSecondInstallPrice.Text = "0";
            labSecondExpressPrice.Text = "0";

        }


        //统计
        //项目安装费字典：<项目Id，费用合计>(按店铺统计)
        Dictionary<int, decimal> subjectInstallPriceDic = new Dictionary<int, decimal>();

        Dictionary<int, decimal> areaDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> areaDic_r = new Dictionary<int, decimal>();

        Dictionary<int, decimal> popPriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> popPriceDic_r = new Dictionary<int, decimal>();

        Dictionary<int, decimal> subjectInstallPriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> subjectInstallPriceDic_r = new Dictionary<int, decimal>();
       
        //二次安装费字典：<项目Id，费用合计>
        Dictionary<int, decimal> secondInstallPriceDic = new Dictionary<int, decimal>();
        Dictionary<int, decimal> secondInstallPriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> secondInstallPriceDic_r = new Dictionary<int, decimal>();
        //二次发货费字典：<项目Id，费用合计>
        Dictionary<int, decimal> secondExpressPriceDic = new Dictionary<int, decimal>();
        Dictionary<int, decimal> secondExpressPriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> secondExpressPriceDic_r = new Dictionary<int, decimal>();

        //项目快递费字典：<项目Id，费用合计>
        Dictionary<int, decimal> expressPriceDic = new Dictionary<int, decimal>();
        Dictionary<int, decimal> expressPriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> expressPriceDic_r = new Dictionary<int, decimal>();
        //物料（道具）费用
        Dictionary<int, decimal> materialPriceDic = new Dictionary<int, decimal>();
        Dictionary<int, decimal> materialPriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> materialPriceDic_r = new Dictionary<int, decimal>();
        //新开店安装费
        Dictionary<int, decimal> newShopInstallPriceDic = new Dictionary<int, decimal>();
        Dictionary<int, decimal> newShopInstallPriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> newShopInstallPriceDic_r = new Dictionary<int, decimal>();
        //其他费用
        Dictionary<int, decimal> otherPriceDic = new Dictionary<int, decimal>();
        Dictionary<int, decimal> otherPriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> otherPriceDic_r = new Dictionary<int, decimal>();
        //分区活动安装费
        Dictionary<int, decimal> regionInstallPriceDic = new Dictionary<int, decimal>();
        //分区活动发货费
        Dictionary<int, decimal> regionExpressPriceDic = new Dictionary<int, decimal>();
        //分区新开店测量费
        Dictionary<int, decimal> measurePriceDic = new Dictionary<int, decimal>();
        Dictionary<int, decimal> measurePriceDic_s = new Dictionary<int, decimal>();
        Dictionary<int, decimal> measurePriceDic_r = new Dictionary<int, decimal>();
        //分区其他费
        Dictionary<int, decimal> regionOtherPriceDic = new Dictionary<int, decimal>();

        void Statistic()
        {
            CleanLab();
            ClearSeesion();
            guidanceIdList = GetGuidanceSelected();
            subjectIdList = GetSubjectSelected();
            priceSubjectIdList = GetPriceSubjectSelected();

            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                {
                    shopTypeList.Add(li.Value);
                }
            }
            foreach (ListItem li in cblAddUser.Items)
            {
                if (li.Selected)
                    addUserList.Add(int.Parse(li.Value));
            }

            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategoryList.Add(int.Parse(li.Value));
            }
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                {
                    regionList.Add(li.Value.ToLower());
                }
            }
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinceList.Contains(li.Value.Trim()))
                {
                    provinceList.Add(li.Value.Trim());
                }
            }
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected && !cityList.Contains(li.Value.Trim()))
                {
                    cityList.Add(li.Value.Trim());
                }
            }
            foreach (ListItem li in cblCustomerService.Items)
            {
                if (li.Selected)
                    customerServiceIds.Add(int.Parse(li.Value));
            }
            if (!regionList.Any() && GetResponsibleRegion.Any())
            {
                GetResponsibleRegion.ForEach(s =>
                {
                    regionList.Add(s.ToLower());
                });
            }

            foreach (ListItem li in cblSubjectChannel.Items)
            {
                if (li.Selected)
                {
                    subjectChannel = int.Parse(li.Value);
                    break;
                }
            }

            if (Session["orderDetailStatistics"] == null)
            {
                LoadSessionData();
            }
            List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            List<SubjectGuidance> guidanceList = new List<SubjectGuidance>();
            if (Session["orderDetailStatistics"] != null)
                finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
            if (Session["shopStatistics"] != null)
                shopList = Session["shopStatistics"] as List<Shop>;
            if (Session["subjectStatistics"] != null)
                subjectList = Session["subjectStatistics"] as List<Subject>;
            if (Session["guidanceStatistics"] != null)
                guidanceList = Session["guidanceStatistics"] as List<SubjectGuidance>;

            #region
            var orderList = (from order in finalOrderDetailTempList
                             join shop in shopList
                             on order.ShopId equals shop.Id
                             join subject in subjectList
                             on order.SubjectId equals subject.Id
                             join guidance in guidanceList
                             on order.GuidanceId equals guidance.ItemId
                             //where (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower())) : 1 == 1)
                             where (regionList.Any() ? regionList.Contains(order.Region.ToLower()) : 1 == 1)

                             select new
                             {
                                 order,
                                 shop,
                                 subject,
                                 guidance

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
            if (shopTypeList.Any())
            {
                if (shopTypeList.Contains("空"))
                {
                    shopTypeList.Remove("空");
                    if (shopTypeList.Any())
                    {
                        orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                    }
                    else
                        orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                }
                else
                    orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
            }
            if (addUserList.Any())
            {
                orderList = orderList.Where(s => addUserList.Contains(s.subject.AddUserId ?? 0)).ToList();
            }
            if (subjectCategoryList.Any())
            {
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

            if (provinceList.Any())
            {
                orderList = orderList.Where(s => provinceList.Contains(s.order.Province.Trim())).ToList();
                if (cityList.Any())
                    orderList = orderList.Where(s => cityList.Contains(s.order.City.Trim())).ToList();
            }

            if ((subjectIdList.Any() || priceSubjectIdList.Any()) == false)
            {
                foreach (ListItem li in cblSubjects.Items)
                    subjectIdList.Add(int.Parse(li.Value));

                foreach (ListItem li in cblPriceSubjects.Items)
                {
                    priceSubjectIdList.Add(int.Parse(li.Value));
                }

            }
            subjectIdList.AddRange(priceSubjectIdList);
            string begin = txtSubjectBegin.Text.Trim();
            string end = txtSubjectEnd.Text.Trim();
            if (!string.IsNullOrWhiteSpace(begin))
            {
                DateTime beginDate = DateTime.Parse(begin);
                orderList = orderList.Where(s => s.subject.AddDate >= beginDate).ToList();
                if (!string.IsNullOrWhiteSpace(end))
                {
                    DateTime endDate = DateTime.Parse(end).AddDays(1);
                    orderList = orderList.Where(s => s.subject.AddDate < endDate).ToList();
                }
            }

            if (customerServiceIds.Any())
            {
                if (customerServiceIds.Contains(0))
                {
                    customerServiceIds.Remove(0);
                    if (customerServiceIds.Any())
                    {
                        orderList = orderList.Where(s => customerServiceIds.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                    }
                    else
                    {
                        orderList = orderList.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                    }
                }
                else
                {
                    orderList = orderList.Where(s => customerServiceIds.Contains(s.order.CSUserId ?? 0)).ToList();

                }
            }
            orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
            decimal installPrice = 0;//安装费
            decimal secondInstallPrice = 0;//二次安装费
            decimal secondExperssPrice = 0;//二次安装费
            decimal expressPrice = 0;//促销快递费
            decimal area = 0;//面积
            decimal popPrice = 0;//POP制作费
            decimal newShopInstallPrice = 0;//新开店安装费
            //decimal freightPrice = 0;//运费
            decimal otherPrice = 0;//其他费用
            decimal measurePrice = 0;//新开店测量费
            decimal regionInstallPrice = 0;//增补/新开店安装费
            decimal regionOtherPrice = 0;//分区活动其他装费
            decimal regionExpressPrice = 0;//分区活动快递/发货装费



            //decimal installPrice_s = 0;//安装费-上海
            decimal secondInstallPrice_s = 0;//二次安装费-上海
            decimal secondExperssPrice_s = 0;//二次快递费-上海
            decimal expressPrice_s = 0;//促销快递费-上海
            //decimal area_s = 0;//面积-上海
            //decimal popPrice_s = 0;//POP制作费-上海
            decimal newShopInstallPrice_s = 0;//新开店安装费-上海
            //decimal freightPrice = 0;//运费-上海
            decimal otherPrice_s = 0;//其他费用-上海
            decimal measurePrice_s = 0;//新开店测量费-上海
            

            //decimal installPrice_r = 0;//安装费-分区
            decimal secondInstallPrice_r = 0;//二次安装费-分区
            decimal secondExperssPrice_r = 0;//二次安装费-分区
            decimal expressPrice_r = 0;//促销快递费-分区
            //decimal area_r = 0;//面积-分区
            //decimal popPrice_r = 0;//POP制作费-分区
            decimal newShopInstallPrice_r = 0;//新开店安装费-分区
            //decimal freightPrice = 0;//运费-分区
            decimal otherPrice_r = 0;//其他费用-分区
            decimal measurePrice_r = 0;//新开店测量费-分区
            


            decimal shutShopExpressPrice = 0;//闭店促销快递费

            //正常店铺订单
            //var normalOrder=orderList0.Where(s => (s.shop.Status == null || s.shop.Status == "" || s.shop.Status == "正常")).ToList();
            //闭店订单
            var shutShopOrder = orderList.Where(s => (s.order.ShopStatus != null && s.order.ShopStatus.Contains("闭"))).ToList();

            //上海（系统）订单
            var systemOrderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
            //分区订单
            var regionOrderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();

            List<int> shopIdList = new List<int>();
            List<int> regionShopIdList = new List<int>();

            if (orderList.Any())
            {

                //shopIdList = orderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                shopIdList = systemOrderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                regionShopIdList = regionOrderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                labSubjectCount.Text = (subjectIdList.Count).ToString();
                //StatisticPOPTotalPrice(orderList.Select(s => s.order), out popPrice, out area);
                //StatisticPOPTotalPrice(systemOrderList.Select(s => s.order), out popPrice_s, out area_s);
                //StatisticPOPTotalPrice(regionOrderList.Select(s => s.order), out popPrice_r, out area_r);

                var systemOrderListPrice1 = StatisticPOPTotalPrice(systemOrderList.Select(s => s.order));
                var regionOrderListPrice1 = StatisticPOPTotalPrice(regionOrderList.Select(s => s.order));

                if (systemOrderListPrice1.Any())
                {
                    systemOrderListPrice1.ForEach(s => {
                        area+=(s.Area??0);
                        popPrice += (s.TotalPrice ?? 0);
                        if (!areaDic_s.Keys.Contains(s.SubjectId ?? 0))
                        {
                            areaDic_s.Add(s.SubjectId ?? 0, s.Area??0);
                        }
                        else
                        {
                            areaDic_s[s.SubjectId ?? 0] = areaDic_s[s.SubjectId ?? 0] + (s.Area ?? 0);
                        }
                        if (!popPriceDic_s.Keys.Contains(s.SubjectId ?? 0))
                        {
                            popPriceDic_s.Add(s.SubjectId ?? 0, s.TotalPrice ?? 0);
                        }
                        else
                        {
                            popPriceDic_s[s.SubjectId ?? 0] = popPriceDic_s[s.SubjectId ?? 0] + (s.TotalPrice ?? 0);
                        }
                    });
                    Session["area_s"] = areaDic_s;
                    Session["popPrice_s"] = popPriceDic_s;
                }
                if (regionOrderListPrice1.Any())
                {
                    regionOrderListPrice1.ForEach(s =>
                    {
                        area += (s.Area ?? 0);
                        popPrice += (s.TotalPrice ?? 0);
                        if (!areaDic_r.Keys.Contains(s.SubjectId ?? 0))
                        {
                            areaDic_r.Add(s.SubjectId ?? 0, s.Area ?? 0);
                        }
                        else
                        {
                            areaDic_r[s.SubjectId ?? 0] = areaDic_r[s.SubjectId ?? 0] + (s.Area ?? 0);
                        }
                        if (!popPriceDic_r.Keys.Contains(s.SubjectId ?? 0))
                        {
                            popPriceDic_r.Add(s.SubjectId ?? 0, s.TotalPrice ?? 0);
                        }
                        else
                        {
                            popPriceDic_r[s.SubjectId ?? 0] = popPriceDic_r[s.SubjectId ?? 0] + (s.TotalPrice ?? 0);
                        }
                    });
                    Session["area_r"] = areaDic_r;
                    Session["popPrice_r"] = popPriceDic_r;
                }
                //area = area_s + area_r;
                //popPrice = popPrice_s + popPrice_r;
                labArea.Text = area > 0 ? (area + "平方米") : "0";
                if (popPrice > 0)
                {
                    labPOPPrice.Text = Math.Round(popPrice, 2) + "元";
                    labPOPPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                    labPOPPrice.Attributes.Add("name", "checkMaterial");
                }
                
            }

            #region 快递费用（促销）
            //var saleGuidanceList=new SubjectGuidanceBLL().GetList(s=>)
            guidanceIdList.ForEach(gid =>
            {
                //var freightOrderShopList = orderList.Where(s => s.order.GuidanceId == gid && (((s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Promotion && (s.guidance.HasExperssFees ?? false) == true) || ((s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Delivery))).ToList();
                //if (freightOrderShopList.Any())
                //{
                    
                //    List<int> expressShopIdList = freightOrderShopList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                //    var expressPriceList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == gid && expressShopIdList.Contains(s.ShopId ?? 0)).ToList();
                //    if (expressPriceList.Any())
                //    {
                //        expressPrice += expressPriceList.Sum(s => s.ExpressPrice ?? 0);
                //    }


                //}

                List<int> expressShopIdList_s = new List<int>();
                var freightOrderShopList_s = systemOrderList.Where(s => s.order.GuidanceId == gid && (((s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Promotion && (s.guidance.HasExperssFees ?? false) == true) || ((s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Delivery))).ToList();
                if (freightOrderShopList_s.Any())
                {

                    expressShopIdList_s = freightOrderShopList_s.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                    var expressPriceList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == gid && expressShopIdList_s.Contains(s.ShopId ?? 0)).ToList();
                    if (expressPriceList.Any())
                    {
                        expressPrice_s += expressPriceList.Sum(s => s.ExpressPrice ?? 0);
                    }


                }


                var freightOrderShopList_r = regionOrderList.Where(s => s.order.GuidanceId == gid && (((s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Promotion && (s.guidance.HasExperssFees ?? false) == true) || ((s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Delivery)) && !expressShopIdList_s.Contains(s.order.ShopId??0)).ToList();
                if (freightOrderShopList_r.Any())
                {

                    List<int> expressShopIdList_r = freightOrderShopList_r.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                    var expressPriceList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == gid && expressShopIdList_r.Contains(s.ShopId ?? 0)).ToList();
                    if (expressPriceList.Any())
                    {
                        expressPrice_r += expressPriceList.Sum(s => s.ExpressPrice ?? 0);
                    }


                }
            });
            #endregion



            //Dictionary<int, List<int>> installShopIdDic = new Dictionary<int, List<int>>(); 
            #region 正常安装费
            guidanceIdList.ForEach(gid =>
            {
                //List<int> installShopIdList = orderList.Where(s => s.order.GuidanceId == gid && s.order.OrderType != (int)OrderTypeEnum.物料 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单).Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                
                //var installPriceDetailList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                //                              join subject in CurrentContext.DbContext.Subject
                //                              on installShop.SubjectId equals subject.Id
                //                              join guidance in CurrentContext.DbContext.SubjectGuidance
                //                              on installShop.GuidanceId equals guidance.ItemId
                //                              where
                //                              installShop.GuidanceId == gid
                //                              && installShopIdList.Contains(installShop.ShopId ?? 0)
                //                              && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)//不统计分区增补的安装费
                //                              && (installShop.BasicPrice ?? 0) > 0
                //                              select new
                //                              {
                //                                  installShop
                                                
                //                              }).ToList();

                //if (installPriceDetailList.Any())
                //{
                //    decimal installPrice0 = 0;
                //    installPriceDetailList.ForEach(s =>
                //    {
                //        installPrice0 = (s.installShop.BasicPrice ?? 0) + (s.installShop.WindowPrice ?? 0) + (s.installShop.OOHPrice ?? 0);
                //        if (!subjectInstallPriceDic.Keys.Contains(s.installShop.SubjectId ?? 0))
                //        {
                //            subjectInstallPriceDic.Add(s.installShop.SubjectId ?? 0, installPrice0);
                //        }
                //        else
                //        {
                //            subjectInstallPriceDic[s.installShop.SubjectId ?? 0] = subjectInstallPriceDic[s.installShop.SubjectId ?? 0] + installPrice0;

                //        }
                //    });
                //}

                //上海
                List<int> installShopIdList_s = systemOrderList.Where(s => s.order.GuidanceId == gid && s.order.OrderType != (int)OrderTypeEnum.物料 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单).Select(s => s.order.ShopId ?? 0).Distinct().ToList();

                var installPriceDetailList_s = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                              join subject in CurrentContext.DbContext.Subject
                                              on installShop.SubjectId equals subject.Id
                                              join guidance in CurrentContext.DbContext.SubjectGuidance
                                              on installShop.GuidanceId equals guidance.ItemId
                                              where
                                              installShop.GuidanceId == gid
                                              && installShopIdList_s.Contains(installShop.ShopId ?? 0)
                                              && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)//不统计分区增补的安装费
                                              && (installShop.BasicPrice ?? 0) > 0
                                              select new
                                              {
                                                  installShop

                                              }).ToList();

                if (installPriceDetailList_s.Any())
                {
                    decimal installPrice0 = 0;
                    installPriceDetailList_s.ForEach(s =>
                    {
                        installPrice0 = (s.installShop.BasicPrice ?? 0) + (s.installShop.WindowPrice ?? 0) + (s.installShop.OOHPrice ?? 0);
                        if (!subjectInstallPriceDic_s.Keys.Contains(s.installShop.SubjectId ?? 0))
                        {
                            subjectInstallPriceDic_s.Add(s.installShop.SubjectId ?? 0, installPrice0);
                        }
                        else
                        {
                            subjectInstallPriceDic_s[s.installShop.SubjectId ?? 0] = subjectInstallPriceDic_s[s.installShop.SubjectId ?? 0] + installPrice0;

                        }
                    });
                }

                //分区
                List<int> installShopIdList_r = regionOrderList.Where(s => s.order.GuidanceId == gid && s.order.OrderType != (int)OrderTypeEnum.物料 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单).Select(s => s.order.ShopId ?? 0).Distinct().ToList();

                var installPriceDetailList_r = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                                join subject in CurrentContext.DbContext.Subject
                                                on installShop.SubjectId equals subject.Id
                                                join guidance in CurrentContext.DbContext.SubjectGuidance
                                                on installShop.GuidanceId equals guidance.ItemId
                                                where
                                                installShop.GuidanceId == gid
                                                && installShopIdList_r.Contains(installShop.ShopId ?? 0)
                                                && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)//不统计分区增补的安装费
                                                && (installShop.BasicPrice ?? 0) > 0
                                                select new
                                                {
                                                    installShop

                                                }).ToList();

                if (installPriceDetailList_r.Any())
                {
                    decimal installPrice0 = 0;
                    installPriceDetailList_r.ForEach(s =>
                    {
                        installPrice0 = (s.installShop.BasicPrice ?? 0) + (s.installShop.WindowPrice ?? 0) + (s.installShop.OOHPrice ?? 0);
                        if (!subjectInstallPriceDic_r.Keys.Contains(s.installShop.SubjectId ?? 0))
                        {
                            subjectInstallPriceDic_r.Add(s.installShop.SubjectId ?? 0, installPrice0);
                        }
                        else
                        {
                            subjectInstallPriceDic_r[s.installShop.SubjectId ?? 0] = subjectInstallPriceDic_r[s.installShop.SubjectId ?? 0] + installPrice0;
                        }
                    });
                }
            });


            

            //var orderInstallList = orderList.Where(s => (s.guidance.ActivityTypeId ?? 1) != (int)GuidanceTypeEnum.Others && s.subject.SubjectType != (int)SubjectTypeEnum.二次安装 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单 && s.order.OrderType > (int)OrderTypeEnum.道具).ToList();
            //if (orderInstallList.Any())
            //{
            //    subjectIdList.ForEach(s =>
            //    {
            //        decimal installPrice0 = orderInstallList.Where(r => r.order.OrderType == (int)OrderTypeEnum.安装费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
            //        if (!subjectInstallPriceDic.Keys.Contains(s))
            //        {
            //            subjectInstallPriceDic.Add(s, installPrice0);
            //        }
            //        else
            //        {
            //            subjectInstallPriceDic[s] = subjectInstallPriceDic[s] + installPrice0;
            //        }
                   
            //        decimal otherPrice0 = orderInstallList.Where(r => r.order.OrderType == (int)OrderTypeEnum.其他费用 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
            //        otherPrice += otherPrice0;
            //        if (!otherPriceDic.Keys.Contains(s))
            //        {
            //            otherPriceDic.Add(s, otherPrice0);
            //        }
            //        else
            //        {
            //            otherPriceDic[s] += otherPrice0;
            //        }

            //        decimal regionExpressPrice0 = orderInstallList.Where(r => r.order.OrderType == (int)OrderTypeEnum.发货费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    
            //        expressPrice += regionExpressPrice0;

            //        decimal measurePrice0 = orderInstallList.Where(r => r.order.OrderType == (int)OrderTypeEnum.测量费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
            //        if (measurePriceDic.Keys.Contains(s))
            //        {
            //            measurePriceDic[s] = measurePriceDic[s] + measurePrice0;
            //        }
            //        else
            //        {
            //            measurePriceDic.Add(s, measurePrice0);
            //        }
            //        measurePrice += measurePrice0;

            //    });
            //}

            //上海
            var orderInstallList_s = systemOrderList.Where(s => (s.guidance.ActivityTypeId ?? 1) != (int)GuidanceTypeEnum.Others && s.subject.SubjectType != (int)SubjectTypeEnum.二次安装 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单 && s.order.OrderType > (int)OrderTypeEnum.道具).ToList();
            if (orderInstallList_s.Any())
            {
                subjectIdList.ForEach(s =>
                {
                    decimal installPrice0 = orderInstallList_s.Where(r => r.order.OrderType == (int)OrderTypeEnum.安装费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (!subjectInstallPriceDic_s.Keys.Contains(s))
                    {
                        subjectInstallPriceDic_s.Add(s, installPrice0);
                    }
                    else
                    {
                        subjectInstallPriceDic_s[s] = subjectInstallPriceDic_s[s] + installPrice0;
                    }

                    decimal otherPrice0 = orderInstallList_s.Where(r => r.order.OrderType == (int)OrderTypeEnum.其他费用 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    otherPrice_s += otherPrice0;
                    if (!otherPriceDic_s.Keys.Contains(s))
                    {
                        otherPriceDic_s.Add(s, otherPrice0);
                    }
                    else
                    {
                        otherPriceDic_s[s] += otherPrice0;
                    }

                    decimal regionExpressPrice0 = orderInstallList_s.Where(r => r.order.OrderType == (int)OrderTypeEnum.发货费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);

                    expressPrice_s += regionExpressPrice0;

                    decimal measurePrice0 = orderInstallList_s.Where(r => r.order.OrderType == (int)OrderTypeEnum.测量费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (measurePriceDic_s.Keys.Contains(s))
                    {
                        measurePriceDic_s[s] = measurePriceDic_s[s] + measurePrice0;
                    }
                    else
                    {
                        measurePriceDic_s.Add(s, measurePrice0);
                    }
                    measurePrice_s += measurePrice0;

                });
            }
            //分区
            var orderInstallList_r = regionOrderList.Where(s => (s.guidance.ActivityTypeId ?? 1) != (int)GuidanceTypeEnum.Others && s.subject.SubjectType != (int)SubjectTypeEnum.二次安装 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单 && s.order.OrderType > (int)OrderTypeEnum.道具).ToList();
            if (orderInstallList_r.Any())
            {
                subjectIdList.ForEach(s =>
                {
                    decimal installPrice0 = orderInstallList_r.Where(r => r.order.OrderType == (int)OrderTypeEnum.安装费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (!subjectInstallPriceDic_r.Keys.Contains(s))
                    {
                        subjectInstallPriceDic_r.Add(s, installPrice0);
                    }
                    else
                    {
                        subjectInstallPriceDic_r[s] = subjectInstallPriceDic_r[s] + installPrice0;
                    }

                    decimal otherPrice0 = orderInstallList_r.Where(r => r.order.OrderType == (int)OrderTypeEnum.其他费用 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    otherPrice_r += otherPrice0;
                    if (!otherPriceDic_r.Keys.Contains(s))
                    {
                        otherPriceDic_r.Add(s, otherPrice0);
                    }
                    else
                    {
                        otherPriceDic_r[s] += otherPrice0;
                    }

                    decimal regionExpressPrice0 = orderInstallList_r.Where(r => r.order.OrderType == (int)OrderTypeEnum.发货费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);

                    expressPrice_r += regionExpressPrice0;

                    decimal measurePrice0 = orderInstallList_r.Where(r => r.order.OrderType == (int)OrderTypeEnum.测量费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (measurePriceDic_r.Keys.Contains(s))
                    {
                        measurePriceDic_r[s] = measurePriceDic_r[s] + measurePrice0;
                    }
                    else
                    {
                        measurePriceDic_r.Add(s, measurePrice0);
                    }
                    measurePrice_r += measurePrice0;

                });
            }

            #endregion

            #region 物料费
            //
            //var materialList = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.物料).ToList();
            //if (materialList.Any())
            //{
                
            //    materialList.ForEach(s =>
            //    {
            //        int num = s.order.Quantity ?? 0;
            //        decimal price = s.order.UnitPrice ?? 0;
            //        decimal subPrice = num * price;
            //        if (materialPriceDic.Keys.Contains(s.order.SubjectId ?? 0))
            //        {
            //            materialPriceDic[s.order.SubjectId ?? 0] += subPrice;
            //        }
            //        else
            //            materialPriceDic.Add(s.order.SubjectId ?? 0, subPrice);
            //    });
            //}
            //上海
            var materialList_s = systemOrderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.物料).ToList();
            if (materialList_s.Any())
            {
                materialList_s.ForEach(s =>
                {
                    int num = s.order.Quantity ?? 0;
                    decimal price = s.order.UnitPrice ?? 0;
                    decimal subPrice = num * price;
                    if (materialPriceDic_s.Keys.Contains(s.order.SubjectId ?? 0))
                    {
                        materialPriceDic_s[s.order.SubjectId ?? 0] += subPrice;
                    }
                    else
                        materialPriceDic_s.Add(s.order.SubjectId ?? 0, subPrice);
                });
            }
            //分区
            var materialList_r = regionOrderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.物料).ToList();
            if (materialList_r.Any())
            {
                materialList_r.ForEach(s =>
                {
                    int num = s.order.Quantity ?? 0;
                    decimal price = s.order.UnitPrice ?? 0;
                    decimal subPrice = num * price;
                    if (materialPriceDic_r.Keys.Contains(s.order.SubjectId ?? 0))
                    {
                        materialPriceDic_r[s.order.SubjectId ?? 0] += subPrice;
                    }
                    else
                        materialPriceDic_r.Add(s.order.SubjectId ?? 0, subPrice);
                });
            }

            #endregion
            #region 二次安装费
            //var secondInstallList = orderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.二次安装 && (s.order.OrderType ?? 1) == (int)OrderTypeEnum.安装费);
            //if (secondInstallList.Any())
            //{
            //    secondInstallList.ToList().ForEach(s =>
            //    {
            //        if (!secondInstallPriceDic.Keys.Contains((s.subject.Id)))
            //        {
            //            secondInstallPriceDic.Add(s.subject.Id, (s.order.OrderPrice ?? 0));
            //        }
            //        else
            //        {
            //            secondInstallPriceDic[s.subject.Id] += (s.order.OrderPrice ?? 0);
            //        }
            //        secondInstallPrice += (s.order.OrderPrice ?? 0);
            //    });
            //}
            //上海
            var secondInstallList_s = systemOrderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.二次安装 && (s.order.OrderType ?? 1) == (int)OrderTypeEnum.安装费);
            if (secondInstallList_s.Any())
            {
                secondInstallList_s.ToList().ForEach(s =>
                {
                    if (!secondInstallPriceDic_s.Keys.Contains((s.subject.Id)))
                    {
                        secondInstallPriceDic_s.Add(s.subject.Id, (s.order.OrderPrice ?? 0));
                    }
                    else
                    {
                        secondInstallPriceDic_s[s.subject.Id] += (s.order.OrderPrice ?? 0);
                    }
                    secondInstallPrice_s += (s.order.OrderPrice ?? 0);
                });
            }
            //分区
            var secondInstallList_r = regionOrderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.二次安装 && (s.order.OrderType ?? 1) == (int)OrderTypeEnum.安装费);
            if (secondInstallList_r.Any())
            {
                secondInstallList_r.ToList().ForEach(s =>
                {
                    if (!secondInstallPriceDic_r.Keys.Contains((s.subject.Id)))
                    {
                        secondInstallPriceDic_r.Add(s.subject.Id, (s.order.OrderPrice ?? 0));
                    }
                    else
                    {
                        secondInstallPriceDic_r[s.subject.Id] += (s.order.OrderPrice ?? 0);
                    }
                    secondInstallPrice_r += (s.order.OrderPrice ?? 0);
                });
            }
            #endregion
            #region 二次发货费
            //var secondExpressList = orderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.二次安装 && (s.order.OrderType ?? 1) == (int)OrderTypeEnum.发货费);
            //if (secondExpressList.Any())
            //{
            //    secondExpressList.ToList().ForEach(s =>
            //    {
            //        if (!secondExpressPriceDic.Keys.Contains((s.subject.Id)))
            //        {
            //            secondExpressPriceDic.Add(s.subject.Id, (s.order.OrderPrice ?? 0));
            //        }
            //        else
            //        {
            //            secondExpressPriceDic[s.subject.Id] += (s.order.OrderPrice ?? 0);
            //        }
            //        secondExperssPrice += (s.order.OrderPrice ?? 0);
            //    });
            //}
            //Session["secondInstallPriceDicStatistics"] = secondInstallPriceDic;
            //上海
            var secondExpressList_s = systemOrderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.二次安装 && (s.order.OrderType ?? 1) == (int)OrderTypeEnum.发货费);
            if (secondExpressList_s.Any())
            {
                secondExpressList_s.ToList().ForEach(s =>
                {
                    if (!secondExpressPriceDic_s.Keys.Contains((s.subject.Id)))
                    {
                        secondExpressPriceDic_s.Add(s.subject.Id, (s.order.OrderPrice ?? 0));
                    }
                    else
                    {
                        secondExpressPriceDic_s[s.subject.Id] += (s.order.OrderPrice ?? 0);
                    }
                    secondExperssPrice_s += (s.order.OrderPrice ?? 0);
                });
            }
            //分区
            var secondExpressList_r = regionOrderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.二次安装 && (s.order.OrderType ?? 1) == (int)OrderTypeEnum.发货费);
            if (secondExpressList_r.Any())
            {
                secondExpressList_r.ToList().ForEach(s =>
                {
                    if (!secondExpressPriceDic_r.Keys.Contains((s.subject.Id)))
                    {
                        secondExpressPriceDic_r.Add(s.subject.Id, (s.order.OrderPrice ?? 0));
                    }
                    else
                    {
                        secondExpressPriceDic_r[s.subject.Id] += (s.order.OrderPrice ?? 0);
                    }
                    secondExperssPrice_r += (s.order.OrderPrice ?? 0);
                });
            }
            #endregion
            #region 统计其他费用（新开安装费/运费）
            if (subjectChannel < 2)
            {
                //统计分区的时候不计算这个
                var orderDetailList = new PriceOrderDetailBLL().GetList(s => priceSubjectIdList.Contains(s.SubjectId ?? 0) && ((s.SubjectType ?? 1) == (int)SubjectTypeEnum.新开店安装费 || (s.SubjectType ?? 1) == (int)SubjectTypeEnum.运费));
                if (regionList.Any())
                {
                    orderDetailList = orderDetailList.Where(s => regionList.Contains(s.Region.ToLower())).ToList();
                    if (provinceList.Any())
                    {
                        orderDetailList = orderDetailList.Where(s => provinceList.Contains(s.Province)).ToList();
                        if (cityList.Any())
                            orderDetailList = orderDetailList.Where(s => cityList.Contains(s.City)).ToList();
                    }
                }
                if (orderDetailList.Any())
                {
                    orderDetailList.ForEach(s =>
                    {
                        if (!newShopInstallPriceDic_s.Keys.Contains((s.SubjectId ?? 0)))
                        {
                            newShopInstallPriceDic_s.Add((s.SubjectId ?? 0), (s.Amount ?? 0));
                        }
                        else
                        {
                            newShopInstallPriceDic_s[(s.SubjectId ?? 0)] += (s.Amount ?? 0);
                        }
                        newShopInstallPrice_s += (s.Amount ?? 0);
                    });
                    
                }
            }
            //Session["newShopInstallDicStatistics"] = newShopInstallPriceDic;
            

            #endregion
            #region 分区活动费（安装费，测量费，其他费用）
            var regionPriceOrderList = regionOrderList.Where(s => (s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Others && (s.order.OrderType ?? 1) > 1).ToList();
            if (regionPriceOrderList.Any())
            {
                
                subjectIdList.ForEach(s =>
                {
                    decimal regionInstallPrice0 = regionPriceOrderList.Where(r => r.order.OrderType == (int)OrderTypeEnum.安装费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (regionInstallPriceDic.Keys.Contains(s))
                    {
                        regionInstallPriceDic[s] = regionInstallPriceDic[s] + regionInstallPrice0;
                    }
                    else
                    {
                        regionInstallPriceDic.Add(s, regionInstallPrice0);
                    }
                    regionInstallPrice += regionInstallPrice0;

                    decimal regionOtherPrice0 = regionPriceOrderList.Where(r => r.order.OrderType == (int)OrderTypeEnum.其他费用 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (regionOtherPriceDic.Keys.Contains(s))
                    {
                        regionOtherPriceDic[s] = regionOtherPriceDic[s] + regionOtherPrice0;
                    }
                    else
                    {
                        regionOtherPriceDic.Add(s, regionOtherPrice0);
                    }
                    regionOtherPrice += regionOtherPrice0;

                    decimal regionExpressPrice0 = regionPriceOrderList.Where(r => r.order.OrderType == (int)OrderTypeEnum.发货费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (regionExpressPriceDic.Keys.Contains(s))
                    {
                        regionExpressPriceDic[s] = regionExpressPriceDic[s] + regionExpressPrice0;
                    }
                    else
                    {
                        regionExpressPriceDic.Add(s, regionExpressPrice0);
                    }
                    regionExpressPrice += regionExpressPrice0;

                    decimal measurePrice0 = regionPriceOrderList.Where(r => r.order.OrderType == (int)OrderTypeEnum.测量费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (measurePriceDic_r.Keys.Contains(s))
                    {
                        measurePriceDic_r[s] = measurePriceDic_r[s] + measurePrice0;
                    }
                    else
                    {
                        measurePriceDic_r.Add(s, measurePrice0);
                    }
                    measurePrice_r += measurePrice0;

                });


            }

            #endregion
            #region 费用订单
            //var priceOrderList = orderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.费用订单).ToList();
            //if (priceOrderList.Any())
            //{
            //    priceOrderList.ForEach(s =>
            //    {
            //        decimal price0 = s.order.OrderPrice ?? 0;
            //        if (s.order.OrderType == (int)OrderTypeEnum.安装费)
            //        {

            //            if (!subjectInstallPriceDic.Keys.Contains(s.order.SubjectId ?? 0))
            //            {
            //                subjectInstallPriceDic.Add(s.order.SubjectId ?? 0, price0);
            //            }
            //            else
            //            {
            //                subjectInstallPriceDic[s.order.SubjectId ?? 0] = subjectInstallPriceDic[s.order.SubjectId ?? 0] + price0;
            //            }
            //        }
            //        else if (s.order.OrderType == (int)OrderTypeEnum.发货费)
            //        {
            //            expressPrice += price0;

            //        }
            //        else if (s.order.OrderType == (int)OrderTypeEnum.其他费用)
            //        {
            //            otherPrice += price0;
            //            if (!otherPriceDic.Keys.Contains((s.order.SubjectId ?? 0)))
            //            {
            //                otherPriceDic.Add((s.order.SubjectId ?? 0), price0);
            //            }
            //            else
            //            {
            //                otherPriceDic[(s.order.SubjectId ?? 0)] += price0;
            //            }
            //        }
            //        else if (s.order.OrderType == (int)OrderTypeEnum.运费)
            //        {
            //            //运费和新开的费用放一起
            //            newShopInstallPrice += price0;
            //            if (!newShopInstallPriceDic.Keys.Contains((s.order.SubjectId ?? 0)))
            //            {
            //                newShopInstallPriceDic.Add((s.order.SubjectId ?? 0), price0);
            //            }
            //            else
            //            {
            //                newShopInstallPriceDic[(s.order.SubjectId ?? 0)] += price0;
            //            }

            //        }
            //    });
            //}
            //Session["otherPriceDicStatistics"] = otherPriceDic;
            //上海
            var priceOrderList_s = systemOrderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.费用订单).ToList();
            if (priceOrderList_s.Any())
            {
                priceOrderList_s.ForEach(s =>
                {
                    decimal price0 = s.order.OrderPrice ?? 0;
                    if (s.order.OrderType == (int)OrderTypeEnum.安装费)
                    {

                        if (!subjectInstallPriceDic_s.Keys.Contains(s.order.SubjectId ?? 0))
                        {
                            subjectInstallPriceDic_s.Add(s.order.SubjectId ?? 0, price0);
                        }
                        else
                        {
                            subjectInstallPriceDic_s[s.order.SubjectId ?? 0] = subjectInstallPriceDic_s[s.order.SubjectId ?? 0] + price0;
                        }
                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.发货费)
                    {
                        expressPrice_s += price0;

                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.其他费用)
                    {
                        otherPrice_s += price0;
                        if (!otherPriceDic_s.Keys.Contains((s.order.SubjectId ?? 0)))
                        {
                            otherPriceDic_s.Add((s.order.SubjectId ?? 0), price0);
                        }
                        else
                        {
                            otherPriceDic_s[(s.order.SubjectId ?? 0)] += price0;
                        }
                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.运费)
                    {
                        //运费和新开的费用放一起
                        newShopInstallPrice_s += price0;
                        if (!newShopInstallPriceDic_s.Keys.Contains((s.order.SubjectId ?? 0)))
                        {
                            newShopInstallPriceDic_s.Add((s.order.SubjectId ?? 0), price0);
                        }
                        else
                        {
                            newShopInstallPriceDic_s[(s.order.SubjectId ?? 0)] += price0;
                        }

                    }
                });
            }

            //分区
            var priceOrderList_r = regionOrderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.费用订单).ToList();
            if (priceOrderList_r.Any())
            {
                priceOrderList_r.ForEach(s =>
                {
                    decimal price0 = s.order.OrderPrice ?? 0;
                    if (s.order.OrderType == (int)OrderTypeEnum.安装费)
                    {

                        if (!subjectInstallPriceDic_r.Keys.Contains(s.order.SubjectId ?? 0))
                        {
                            subjectInstallPriceDic_r.Add(s.order.SubjectId ?? 0, price0);
                        }
                        else
                        {
                            subjectInstallPriceDic_r[s.order.SubjectId ?? 0] = subjectInstallPriceDic_r[s.order.SubjectId ?? 0] + price0;
                        }
                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.发货费)
                    {
                        expressPrice_r += price0;

                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.其他费用)
                    {
                        otherPrice_r += price0;
                        if (!otherPriceDic_r.Keys.Contains((s.order.SubjectId ?? 0)))
                        {
                            otherPriceDic_r.Add((s.order.SubjectId ?? 0), price0);
                        }
                        else
                        {
                            otherPriceDic_r[(s.order.SubjectId ?? 0)] += price0;
                        }
                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.运费)
                    {
                        //运费和新开的费用放一起
                        newShopInstallPrice_r += price0;
                        if (!newShopInstallPriceDic_r.Keys.Contains((s.order.SubjectId ?? 0)))
                        {
                            newShopInstallPriceDic_r.Add((s.order.SubjectId ?? 0), price0);
                        }
                        else
                        {
                            newShopInstallPriceDic_r[(s.order.SubjectId ?? 0)] += price0;
                        }

                    }
                });
            }
            #endregion
            #endregion
            
            if (subjectInstallPriceDic_s.Keys.Count > 0)
            {
                Session["subjectInstallPrice_s"] = subjectInstallPriceDic_s;
                installPrice += (subjectInstallPriceDic_s.Sum(s => s.Value));

            }
            if (subjectInstallPriceDic_r.Keys.Count > 0)
            {
                Session["subjectInstallPrice_r"] = subjectInstallPriceDic_r;
                installPrice += (subjectInstallPriceDic_r.Sum(s => s.Value));

            }
           
            if (expressPriceDic_s.Keys.Count > 0)
            {
                if (subjectIdList.Any())
                {
                    subjectIdList.ForEach(s =>
                    {
                        if (expressPriceDic_s.Keys.Contains(s))
                            expressPrice += expressPriceDic_s[s];
                    });
                }
                else
                {
                    foreach (KeyValuePair<int, decimal> item in expressPriceDic_s)
                    {
                        expressPrice += item.Value;
                    }
                }
                Session["expressPrice_s"] = expressPriceDic_s;
            }
            if (expressPriceDic_r.Keys.Count > 0)
            {
                if (subjectIdList.Any())
                {
                    subjectIdList.ForEach(s =>
                    {
                        if (expressPriceDic_r.Keys.Contains(s))
                            expressPrice += expressPriceDic_r[s];
                    });
                }
                else
                {
                    foreach (KeyValuePair<int, decimal> item in expressPriceDic_r)
                    {
                        expressPrice += item.Value;
                    }
                }
                Session["expressPrice_r"] = expressPriceDic_r;
            }
          
            if (shopIdList.Any())
            {
                labShopCount.Text = shopIdList.Distinct().Count().ToString();
                labShopCount.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labShopCount.Attributes.Add("name", "checkShop");
            }
            expressPrice = expressPrice_s + expressPrice_r;
            if (expressPrice > 0)
            {
                labExpressPrice.Text = Math.Round(expressPrice, 2) + "元";
            }
            secondExperssPrice = secondExperssPrice_s + secondExperssPrice_r;
            if (secondExperssPrice > 0)
            {
                labSecondExpressPrice.Text = Math.Round(secondExperssPrice, 2) + "元";
            }
            secondInstallPrice = secondInstallPrice_s + secondInstallPrice_r;
            if (secondInstallPrice > 0)
            {
                labSecondInstallPrice.Text = Math.Round(secondInstallPrice, 2) + "元";
            }
            if (installPrice > 0)
            {
                labInstallPrice.Text = Math.Round(installPrice, 2) + "元";
                labInstallPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labInstallPrice.Attributes.Add("name", "checkInstallPrice");
            }

            decimal materialPrice = 0;
            if (materialPriceDic_s.Keys.Count > 0)
            {
                if (subjectIdList.Any())
                {
                    subjectIdList.ForEach(s =>
                    {
                        if (materialPriceDic_s.Keys.Contains(s))
                            materialPrice += materialPriceDic_s[s];
                    });
                }
                else
                {
                    foreach (KeyValuePair<int, decimal> item in materialPriceDic_s)
                    {
                        materialPrice += item.Value;
                    }
                }
                Session["materialPrice_s"] = materialPriceDic_s;
            }
            if (materialPriceDic_r.Keys.Count > 0)
            {
                if (subjectIdList.Any())
                {
                    subjectIdList.ForEach(s =>
                    {
                        if (materialPriceDic_r.Keys.Contains(s))
                            materialPrice += materialPriceDic_r[s];
                    });
                }
                else
                {
                    foreach (KeyValuePair<int, decimal> item in materialPriceDic_r)
                    {
                        materialPrice += item.Value;
                    }
                }
                Session["materialPrice_r"] = materialPriceDic_r;
            }
           
            if (materialPrice > 0)
            {
                labMaterialPrice.Text = Math.Round(materialPrice, 2) + "元";
                labMaterialPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labMaterialPrice.Attributes.Add("name", "checkMaterialOrderPrice");
            }
           
            if (newShopInstallPriceDic_s.Keys.Count > 0)
            {
                Session["newShopInstallPrice_s"] = newShopInstallPriceDic_s;
            }
            if (newShopInstallPriceDic_r.Keys.Count > 0)
            {
                Session["newShopInstallPrice_r"] = newShopInstallPriceDic_r;
            }
            newShopInstallPrice = newShopInstallPrice_s + newShopInstallPrice_r;
            if (newShopInstallPrice > 0)
            {
                labNewShopInstallPrice.Text = Math.Round(newShopInstallPrice, 2) + "元";
                labNewShopInstallPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labNewShopInstallPrice.Attributes.Add("name", "checkNewShopInstallPrice");

            }
            if (otherPriceDic_s.Keys.Count > 0)
            {
                Session["otherPrice_s"] = otherPriceDic_s;
            }
            if (otherPriceDic_r.Keys.Count > 0)
            {
                Session["otherPrice_r"] = otherPriceDic_r;
            }
            otherPrice = otherPrice_s + otherPrice_r;
            if (otherPrice > 0)
            {
                labFreight.Text = Math.Round(otherPrice, 2) + "元";
                labFreight.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labFreight.Attributes.Add("name", "checkOtherPrice");
            }

            if (regionInstallPrice > 0)
            {
                labRegionInstallPrice.Text = Math.Round(regionInstallPrice, 2) + "元";

            }
            if (regionExpressPrice > 0)
            {
                labRegionExpressPrice.Text = Math.Round(regionExpressPrice, 2) + "元";

            }
            
            measurePrice = measurePrice_s + measurePrice_r;
            if (measurePrice > 0)
            {
                labMeasurePrice.Text = Math.Round(measurePrice, 2) + "元";

            }
            if (regionOtherPrice > 0)
            {
                labRegionOtherPrice.Text = Math.Round(regionOtherPrice, 2) + "元";

            }

            //decimal measurePrice = 0;//新开店测量费
            //decimal regionInstallPrice = 0;//增补/新开店安装费
            //decimal regionOtherPrice = 0;//分区活动其他装费
            //decimal regionExpressPrice = 0;//分区活动快递/发货装费


            decimal total = popPrice + installPrice + expressPrice + secondInstallPrice + materialPrice + newShopInstallPrice + otherPrice + measurePrice + regionInstallPrice + regionOtherPrice + regionExpressPrice + secondExperssPrice;
            labTotalPrice.Text = total > 0 ? (Math.Round(total, 2) + "元") : "0";


            #region 统计闭店
            if (shutShopOrder.Any())
            {
                List<int> shutDownShopIdList = shutShopOrder.Select(s => s.shop.Id).Distinct().ToList();


                if (shutDownShopIdList.Any())
                {
                    labShopCount1.Text = shutDownShopIdList.Count.ToString();
                    labShopCount1.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                    labShopCount1.Attributes.Add("name", "checkShop");
                    labShopCount1.Attributes.Add("data-status", "1");
                }

                decimal shutShopPopPrice = 0;
                decimal shutShopArea = 0;
                //StatisticPOPPrice(shutShopOrder.Select(s => s.order), out shutShopPopPrice, out shutShopArea);
                StatisticPOPTotalPrice(shutShopOrder.Select(s => s.order), out shutShopPopPrice, out shutShopArea);
                //闭店POP费用
                labArea1.Text = shutShopArea > 0 ? (shutShopArea + "平方米") : "0";
                if (shutShopPopPrice > 0)
                {
                    labPOPPrice1.Text = Math.Round(shutShopPopPrice, 2) + "元";
                    labPOPPrice1.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                    labPOPPrice1.Attributes.Add("name", "checkMaterial");
                    labPOPPrice1.Attributes.Add("data-status", "1");
                }

                decimal shutShopInstallPrice = 0;
                //闭店的安装费统计
                guidanceIdList.ForEach(gid =>
                {
                    var shutList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                    join shop in CurrentContext.DbContext.Shop
                                    on installShop.ShopId equals shop.Id
                                    join subject in CurrentContext.DbContext.Subject
                                    on installShop.SubjectId equals subject.Id
                                    join guidance in CurrentContext.DbContext.SubjectGuidance
                                    on subject.GuidanceId equals guidance.ItemId
                                    where
                                    installShop.GuidanceId == gid
                                    && shutDownShopIdList.Contains(installShop.ShopId ?? 0)

                                    && (installShop.BasicPrice ?? 0) > 0
                                    select installShop).ToList();
                    shutList.ForEach(s =>
                        {
                            shutShopInstallPrice += (s.BasicPrice ?? 0) + (s.WindowPrice ?? 0) + (s.OOHPrice ?? 0);
                        });
                });
                var priceOrderList2 = shutShopOrder.Where(s => s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.其他费用).ToList();
                if (priceOrderList2.Any())
                {
                    shutShopInstallPrice += priceOrderList2.Sum(s => s.order.OrderPrice ?? 0);
                }
                //闭店快递费
                guidanceIdList.ForEach(gid =>
                {
                    var expressPriceList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == gid && shutDownShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    if (expressPriceList.Any())
                    {
                        shutShopExpressPrice += expressPriceList.Sum(s => s.ExpressPrice ?? 0);
                    }
                });
                var priceOrderList3 = shutShopOrder.Where(s => s.order.OrderType == (int)OrderTypeEnum.发货费).ToList();
                if (priceOrderList3.Any())
                {
                    shutShopExpressPrice += priceOrderList3.Sum(s => s.order.OrderPrice ?? 0);
                }
                if (shutShopInstallPrice > 0)
                {
                    labInstallPrice1.Text = Math.Round(shutShopInstallPrice, 2) + "元";
                    labInstallPrice1.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                    labInstallPrice1.Attributes.Add("name", "checkInstallPrice");
                    labInstallPrice1.Attributes.Add("data-status", "1");
                }

                decimal shutShopMaterialPrice = 0;
                //闭店物料费用
                if (materialList_s.Any() || materialList_r.Any())
                {
                    var materialList = materialList_s.Concat(materialList_r);
                    var shutList = materialList.Where(s => s.shop.Status != null && s.shop.Status.Contains("闭")).ToList();
                    if (shutList.Any())
                    {
                        shutList.ForEach(s =>
                        {
                            shutShopMaterialPrice += ((s.order.Quantity ?? 1) * (s.order.UnitPrice ?? 0));
                        });

                    }
                }
                if (shutShopMaterialPrice > 0)
                {
                    labMaterialPrice1.Text = Math.Round(shutShopMaterialPrice, 2) + "元";
                    labMaterialPrice1.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                    labMaterialPrice1.Attributes.Add("name", "checkShutShopMaterialOrderPrice");
                }



                decimal total1 = shutShopPopPrice + shutShopInstallPrice + shutShopMaterialPrice + shutShopExpressPrice;
                labTotalPrice1.Text = total1 > 0 ? (Math.Round(total1, 2) + "元") : "0";

                if (CurrentUser.UserLevelId == (int)UserLevelEnum.总部)
                {
                    Panel1.Visible = true;
                }
            #endregion
            }
        }


        void ClearSeesion()
        {
            Session["area_s"] = null;
            Session["area_r"] = null;
            Session["popPrice_s"] = null;
            Session["popPrice_r"] = null;
            Session["subjectInstallPrice_s"] = null;
            Session["subjectInstallPrice_r"] = null;
            Session["expressPrice_s"] = null;
            Session["expressPrice_r"] = null;
            Session["materialPrice_s"] = null;
            Session["materialPrice_r"] = null;
            Session["newShopInstallPrice_s"] = null;
            Session["newShopInstallPrice_r"] = null;
            Session["otherPrice_s"] = null;
            Session["otherPrice_r"] = null;
            Session["measurePrice_s"] = null;
            Session["measurePrice_r"] = null;
        }
       

        /// <summary>
        /// 获取已选择的活动
        /// </summary>
        /// <returns></returns>
        List<int> GetGuidanceSelected()
        {
            List<int> list = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected && !list.Contains(int.Parse(li.Value)))
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取已选择的正常项目
        /// </summary>
        /// <returns></returns>
        List<int> GetSubjectSelected()
        {
            List<int> list = new List<int>();
            foreach (ListItem li in cblSubjects.Items)
            {
                if (li.Selected && !list.Contains(int.Parse(li.Value)))
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取已选择的新开店安装项目
        /// </summary>
        /// <returns></returns>
        List<int> GetPriceSubjectSelected()
        {
            List<int> list = new List<int>();
            foreach (ListItem li in cblPriceSubjects.Items)
            {
                if (li.Selected && !list.Contains(int.Parse(li.Value)))
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取已选择的二次安装费项目
        /// </summary>
        /// <returns></returns>
        List<int> GetSecondInstallSubjectSelected()
        {
            List<int> list = new List<int>();
            //foreach (ListItem li in cblSecondInstallSubjects.Items)
            //{
            //    if (li.Selected && !list.Contains(int.Parse(li.Value)))
            //    {
            //        list.Add(int.Parse(li.Value));
            //    }
            //}
            return list;
        }

        FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
        //int totalShopCount = 0;

        //decimal totalArea = 0;
        //decimal totalPopPrice = 0;



        int index = 0;
        /// <summary>
        /// 统计每个项目的费用、面积信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        protected void gvList_ItemDataBound1(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    
                    object subjectIdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int subjectId = subjectIdObj != null ? int.Parse(subjectIdObj.ToString()) : 0;
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    int subjectType = subjectTypeObj != null ? int.Parse(subjectTypeObj.ToString()) : 1;
                    object priceBlongRegionObj = item.GetType().GetProperty("PriceBlongRegion").GetValue(item, null);
                    string priceBlongRegion = priceBlongRegionObj != null ? priceBlongRegionObj.ToString() : string.Empty;

                    ((Label)e.Item.FindControl("labSubjectType")).Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());

                    Label shopCountLab = (Label)e.Item.FindControl("labShopCount");
                    Label areaLab = (Label)e.Item.FindControl("labArea");
                    Label POPPriceLab = (Label)e.Item.FindControl("labPOPPrice");
                    Label InstallPriceLab = (Label)e.Item.FindControl("labInstallPrice");
                    Label ExpressPriceLab = (Label)e.Item.FindControl("labExpressPrice");


                    object subjectNameObj = item.GetType().GetProperty("SubjectName").GetValue(item, null);
                    string subjectName = subjectNameObj != null ? subjectNameObj.ToString() : "";

                    object remarkObj = item.GetType().GetProperty("Remark").GetValue(item, null);
                    string remark = remarkObj != null ? remarkObj.ToString() : "";

                    Label labSubjectName = (Label)e.Item.FindControl("labSubjectName");
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(remark))
                    {
                        subjectName += "-" + remark + "";
                    }
                    if (!string.IsNullOrWhiteSpace(priceBlongRegion))
                    {
                        subjectName += "-" + priceBlongRegion + "实施";
                    }
                    labSubjectName.Text = subjectName;


                    if (subjectType != 2)//pop订单
                    {

                        List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
                        List<Shop> shopList = new List<Shop>();
                        List<Subject> subjectList = new List<Subject>();
                        List<SubjectGuidance> guidanceList = new List<SubjectGuidance>();
                        if (Session["orderDetailStatistics"] != null)
                            finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
                        if (Session["shopStatistics"] != null)
                            shopList = Session["shopStatistics"] as List<Shop>;
                        if (Session["subjectStatistics"] != null)
                            subjectList = Session["subjectStatistics"] as List<Subject>;


                        var orderList = (from order in finalOrderDetailTempList
                                         join shop in shopList
                                         on order.ShopId equals shop.Id
                                         join subject in subjectList
                                         on order.SubjectId equals subject.Id
                                         where subject.Id == subjectId
                                         //&& (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower())) : 1 == 1)
                                         && (regionList.Any() ?  regionList.Contains(order.Region.ToLower()) : 1 == 1)
                                         
                                         select new
                                         {
                                             order,
                                             shop,
                                             subject


                                         }).ToList();



                        if (shopTypeList.Any())
                        {
                            if (shopTypeList.Contains("空"))
                            {
                                shopTypeList.Remove("空");
                                if (shopTypeList.Any())
                                {
                                    orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                                }
                                else
                                    orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                            }
                            else
                                orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                        }
                        if (addUserList.Any())
                        {
                            orderList = orderList.Where(s => addUserList.Contains(s.subject.AddUserId ?? 0)).ToList();
                        }

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
                        if (regionList.Any())
                        {
                            //orderList = orderList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                            if (provinceList.Any())
                            {
                                orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                                if (cityList.Any())
                                {
                                    orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
                                }
                            }
                        }
                        string begin = txtSubjectBegin.Text.Trim();
                        string end = txtSubjectEnd.Text.Trim();
                        if (!string.IsNullOrWhiteSpace(begin))
                        {
                            DateTime beginDate = DateTime.Parse(begin);
                            orderList = orderList.Where(s => s.subject.AddDate >= beginDate).ToList();
                            if (!string.IsNullOrWhiteSpace(end))
                            {
                                DateTime endDate = DateTime.Parse(end).AddDays(1);
                                orderList = orderList.Where(s => s.subject.AddDate < endDate).ToList();
                            }
                        }
                        if (customerServiceIds.Any())
                        {
                            if (customerServiceIds.Contains(0))
                            {
                                customerServiceIds.Remove(0);
                                if (customerServiceIds.Any())
                                {
                                    orderList = orderList.Where(s => customerServiceIds.Contains(s.order.CSUserId ?? 0) || (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                                }
                                else
                                {
                                    orderList = orderList.Where(s => (s.order.CSUserId == null || s.order.CSUserId == 0)).ToList();

                                }
                            }
                            else
                            {
                                orderList = orderList.Where(s => customerServiceIds.Contains(s.order.CSUserId ?? 0)).ToList();

                            }
                        }
                        if (orderList.Any())
                        {


                            int shopCount = orderList.Select(s => s.shop).Distinct().Count();
                            decimal area = 0;
                            decimal popPrice = 0;
                            //StatisticPOPPrice(orderList.Select(s => s.order), out popPrice, out area);
                            StatisticPOPTotalPrice(orderList.Select(s => s.order), out popPrice, out area);
                            shopCountLab.Text = shopCount.ToString();
                            //areaLab.Text = area > 0 ? Math.Round(area, 2).ToString() : "0";
                            areaLab.Text = area > 0 ? area.ToString() : "0";
                            StringBuilder labText = new StringBuilder();
                            labText.AppendFormat("<a href='javascript:void(0);' onclick='CheckMaterialPrice({0})' style='text-decoration: underline;'>", subjectId);
                            labText.AppendFormat("{0}", popPrice > 0 ? Math.Round(popPrice, 2).ToString() : "0");
                            labText.Append("</a>");
                            //POPPriceLab.Text = popPrice > 0 ? Math.Round(popPrice, 2).ToString() : "0";
                            POPPriceLab.Text = labText.ToString();
                        }
                        Dictionary<int, decimal> installPriceDic = new Dictionary<int, decimal>();
                        if (Session["installPriceDicStatistics"] != null)
                        {
                            installPriceDic = Session["installPriceDicStatistics"] as Dictionary<int, decimal>;
                        }
                        if (installPriceDic.Keys.Count > 0 && installPriceDic.Keys.Contains(subjectId))
                        {
                            InstallPriceLab.Text = installPriceDic[subjectId].ToString();
                        }

                        Dictionary<int, decimal> expressPriceDicNew = new Dictionary<int, decimal>();
                        if (Session["expressPriceDicStatistics"] != null)
                        {
                            expressPriceDicNew = Session["expressPriceDicStatistics"] as Dictionary<int, decimal>;
                        }
                        if (expressPriceDicNew.Keys.Count > 0 && expressPriceDicNew.Keys.Contains(subjectId))
                        {
                            ExpressPriceLab.Text = expressPriceDicNew[subjectId].ToString();
                        }
                    }

                    Dictionary<int, decimal> materialPriceDicNew = new Dictionary<int, decimal>();
                    if (Session["materialPriceDicStatistics"] != null)
                    {
                        materialPriceDicNew = Session["materialPriceDicStatistics"] as Dictionary<int, decimal>;
                    }
                    if (materialPriceDicNew.Keys.Count > 0)
                    {
                        Label labMaterial = (Label)e.Item.FindControl("labMaterial");
                        if (materialPriceDicNew.Keys.Contains(subjectId))
                        {
                            var materialOrderList = (from materialOrder in CurrentContext.DbContext.OrderMaterial
                                                     join shop in CurrentContext.DbContext.Shop
                                                     on materialOrder.ShopId equals shop.Id
                                                     where materialOrder.SubjectId == subjectId
                                                     select shop).ToList();
                            if (regionList.Any())
                            {
                                materialOrderList = materialOrderList.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();
                                if (provinceList.Any())
                                {
                                    materialOrderList = materialOrderList.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                                    if (cityList.Any())
                                    {
                                        materialOrderList = materialOrderList.Where(s => cityList.Contains(s.CityName)).ToList();
                                    }
                                }
                            }
                            shopCountLab.Text = materialOrderList.Distinct().Count().ToString();
                            if (materialPriceDicNew[subjectId] > 0)
                            {
                                StringBuilder labText = new StringBuilder();
                                labText.AppendFormat("<a href='javascript:void(0);' onclick='CheckMaterialOrderPrice({0})' style='text-decoration: underline;'>", subjectId);
                                labText.AppendFormat("{0}", Math.Round(materialPriceDicNew[subjectId], 2).ToString());
                                labText.Append("</a>");
                                labMaterial.Text = labText.ToString();

                            }

                        }
                    }
                    ////新开店安装费
                    //if (newShopInstallPriceDic.Keys.Count > 0 && newShopInstallPriceDic.Keys.Contains(subjectId))
                    //{
                    //    Label labNewShopInstallPrice = (Label)e.Item.FindControl("labNewShopInstallPrice");
                    //    if (newShopInstallPriceDic[subjectId] > 0)
                    //    {

                    //        labNewShopInstallPrice.Text = Math.Round(newShopInstallPriceDic[subjectId], 2).ToString();

                    //    }
                    //}
                    ////运费
                    //Dictionary<int, decimal> freightDicNew = new Dictionary<int, decimal>();
                    //if (Session["freightDicStatistics"] != null)
                    //{
                    //    freightDicNew = Session["freightDicStatistics"] as Dictionary<int, decimal>;
                    //}

                    //if (freightDicNew.Keys.Count > 0 && freightDicNew.Keys.Contains(subjectId))
                    //{
                    //    Label labFreight = (Label)e.Item.FindControl("labFreight");
                    //    if (freightDicNew[subjectId] > 0)
                    //    {

                    //        labFreight.Text = Math.Round(freightDicNew[subjectId], 2).ToString();

                    //    }
                    //}
                    //新开店安装费/运费
                    Dictionary<int, decimal> newShopInstallPriceDicNew = new Dictionary<int, decimal>();
                    if (Session["newShopInstallDicStatistics"] != null)
                    {
                        newShopInstallPriceDicNew = Session["newShopInstallDicStatistics"] as Dictionary<int, decimal>;
                    }
                    if (newShopInstallPriceDicNew.Keys.Count > 0 && newShopInstallPriceDicNew.Keys.Contains(subjectId))
                    {
                        Label labNewShopInstallPrice = (Label)e.Item.FindControl("labNewShopInstallPrice");
                        labNewShopInstallPrice.Text = newShopInstallPriceDicNew[subjectId].ToString();
                    }

                    //二次安装费
                    Dictionary<int, decimal> secondInstallPriceDicNew = new Dictionary<int, decimal>();
                    if (Session["secondInstallPriceDicStatistics"] != null)
                    {
                        secondInstallPriceDicNew = Session["secondInstallPriceDicStatistics"] as Dictionary<int, decimal>;
                    }
                    if (secondInstallPriceDicNew.Keys.Count > 0 && secondInstallPriceDicNew.Keys.Contains(subjectId))
                    {
                        Label labSecondInstallPrice = (Label)e.Item.FindControl("labSecondInstallPrice");
                        labSecondInstallPrice.Text = secondInstallPriceDicNew[subjectId].ToString();
                    }

                    //其他费用
                    Dictionary<int, decimal> otherPriceDicNew = new Dictionary<int, decimal>();
                    if (Session["otherPriceDicStatistics"] != null)
                    {
                        otherPriceDicNew = Session["otherPriceDicStatistics"] as Dictionary<int, decimal>;
                    }
                    if (otherPriceDicNew.Keys.Count > 0 && otherPriceDicNew.Keys.Contains(subjectId))
                    {
                        Label labOtherPrice = (Label)e.Item.FindControl("labOtherPrice");
                        labOtherPrice.Text = otherPriceDicNew[subjectId].ToString();
                    }
                }
            }

        }

        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {

                    object subjectIdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int subjectId = subjectIdObj != null ? int.Parse(subjectIdObj.ToString()) : 0;
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    int subjectType = subjectTypeObj != null ? int.Parse(subjectTypeObj.ToString()) : 1;
                    object priceBlongRegionObj = item.GetType().GetProperty("PriceBlongRegion").GetValue(item, null);
                    string priceBlongRegion = priceBlongRegionObj != null ? priceBlongRegionObj.ToString() : string.Empty;

                    ((Label)e.Item.FindControl("labSubjectType")).Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());

                    Label shopCountLab = (Label)e.Item.FindControl("labShopCount");
                    
                    Label POPPriceLab = (Label)e.Item.FindControl("labPOPPrice");
                    Label InstallPriceLab = (Label)e.Item.FindControl("labInstallPrice");
                    Label ExpressPriceLab = (Label)e.Item.FindControl("labExpressPrice");


                    object subjectNameObj = item.GetType().GetProperty("SubjectName").GetValue(item, null);
                    string subjectName = subjectNameObj != null ? subjectNameObj.ToString() : "";

                    object remarkObj = item.GetType().GetProperty("Remark").GetValue(item, null);
                    string remark = remarkObj != null ? remarkObj.ToString() : "";

                    Label labSubjectName = (Label)e.Item.FindControl("labSubjectName");
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(remark))
                    {
                        subjectName += "-" + remark + "";
                    }
                    if (!string.IsNullOrWhiteSpace(priceBlongRegion))
                    {
                        subjectName += "-" + priceBlongRegion + "实施";
                    }
                    labSubjectName.Text = subjectName;


                    if (Session["area_s"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["area_s"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labArea");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId],2).ToString();
                        }
                    }
                    if (Session["area_r"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["area_r"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labArea1");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["popPrice_s"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["popPrice_s"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labPOPPrice");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["popPrice_r"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["popPrice_r"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labPOPPrice1");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["subjectInstallPrice_s"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["subjectInstallPrice_s"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labInstallPrice");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["subjectInstallPrice_r"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["subjectInstallPrice_r"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labInstallPrice1");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["materialPrice_s"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["materialPrice_s"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labMaterial");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["materialPrice_r"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["materialPrice_r"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labMaterial1");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["newShopInstallPrice_s"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["newShopInstallPrice_s"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labNewShopInstallPrice");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["newShopInstallPrice_r"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["newShopInstallPrice_r"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labNewShopInstallPrice1");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["otherPrice_s"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["otherPrice_s"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labOtherPrice");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                    if (Session["otherPrice_r"] != null)
                    {
                        Dictionary<int, decimal> newDic = new Dictionary<int, decimal>();
                        newDic = Session["otherPrice_r"] as Dictionary<int, decimal>;
                        Label lab = (Label)e.Item.FindControl("labOtherPrice1");
                        if (newDic.Keys.Count > 0 && newDic.Keys.Contains(subjectId))
                        {
                            lab.Text = Math.Round(newDic[subjectId], 2).ToString();
                        }
                    }
                }
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
            BindRegion();

        }


        /// <summary>
        /// 按时间获取项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGetProject_Click(object sender, EventArgs e)
        {

            BindSubjects();
            BindPriceSubjects();
            BindSecondInstallSubjects();
        }

        void GetInstallPrice(int subjectId, out decimal installPrice)
        {
            installPrice = 0;
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                        group order by order.ShopId into g
                        join shop in CurrentContext.DbContext.Shop
                        on g.Key equals shop.Id
                        select shop).ToList();
            decimal price = 0;
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    price += (s.InstallPrice ?? 0);
                });
            }
            installPrice = price;

        }



        void BindRegion()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<string> myRegion = GetResponsibleRegion;
            if (myRegion.Any())
            {
                myRegion.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblRegion.Items.Add(li);
                });
            }
            else
                BindRegionByCustomer1(customerId, ref cblRegion);
        }

        void BindProvince()
        {
            cblProvince.Items.Clear();
            cblCity.Items.Clear();
            int subjectChannel1 = 0;
            List<string> shopTypeList = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                {
                    shopTypeList.Add(li.Value);
                }
            }
            foreach (ListItem li in cblSubjectChannel.Items)
            {
                if (li.Selected)
                {
                    subjectChannel1 = int.Parse(li.Value);
                    break;
                }
            }
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                    regionList.Add(li.Value.ToLower());
            }
            List<int> addUsers = new List<int>();
            foreach (ListItem li in cblAddUser.Items)
            {
                if (li.Selected)
                    addUsers.Add(int.Parse(li.Value));
            }
            List<int> subjectCategorys = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategorys.Add(int.Parse(li.Value));
            }
            if (regionList.Any())
            {
              
                if (Session["orderDetailStatistics"] == null)
                {
                    LoadSessionData();
                }
                List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList = new List<Subject>();
                if (Session["orderDetailStatistics"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
                if (Session["shopStatistics"] != null)
                    shopList = Session["shopStatistics"] as List<Shop>;
                if (Session["subjectStatistics"] != null)
                    subjectList = Session["subjectStatistics"] as List<Subject>;

                var list1 = (from order in finalOrderDetailTempList
                             join shop in shopList
                             on order.ShopId equals shop.Id
                             join subject in subjectList
                             on order.SubjectId equals subject.Id
                             //where ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower()))
                             where regionList.Contains(order.Region.ToLower())

                             select new
                             {
                                 order,
                                 shop,
                                 subject

                             }).ToList();
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
                if (subjectChannel1 == 1)
                {
                    //上海系统单
                    list1 = list1.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel1 == 2)
                {
                    //分区订单
                    list1 = list1.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (addUsers.Any())
                {
                    list1 = list1.Where(s => addUsers.Contains(s.subject.AddUserId ?? 0)).ToList();
                }
                if (subjectCategorys.Any())
                {
                    if (subjectCategorys.Contains(0))
                    {
                        subjectCategorys.Remove(0);
                        if (subjectCategorys.Any())
                        {
                            list1 = list1.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            list1 = list1.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();
                    }
                    else
                        list1 = list1.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (list1.Any())
                {

                    var provinceList = list1.OrderBy(s => s.order.Region).Select(s => s.order.Province).Distinct().ToList();
                    if (provinceList.Any())
                    {
                        List<string> selectedList = new List<string>();
                        if (Session["provinceSelected"] != null)
                        {
                            selectedList = Session["provinceSelected"] as List<string>;
                        }
                        bool isSelected = false;
                        provinceList.ForEach(s =>
                        {
                            ListItem li = new ListItem();
                            li.Text = s + "&nbsp;";
                            li.Value = s;
                            if (selectedList.Contains(s))
                            {
                                li.Selected = true;
                                isSelected = true;
                            }
                            cblProvince.Items.Add(li);
                        });
                        if (isSelected)
                        {
                            BindCity();
                        }
                    }
                }

            }
            BindPriceSubjects();
            BindSubjects();

        }

        void BindCity()
        {

            cblCity.Items.Clear();
            int subjectChannel1 = 0;
            List<string> shopTypeList = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                {
                    shopTypeList.Add(li.Value);
                }
            }
            foreach (ListItem li in cblSubjectChannel.Items)
            {
                if (li.Selected)
                {
                    subjectChannel1 = int.Parse(li.Value);
                    break;
                }
            }
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                {
                    regionList.Add(li.Value.ToLower());
                }
            }
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinceList.Contains(li.Value))
                    provinceList.Add(li.Value);
            }
            List<int> addUsers = new List<int>();
            foreach (ListItem li in cblAddUser.Items)
            {
                if (li.Selected)
                    addUsers.Add(int.Parse(li.Value));
            }
            List<int> subjectCategorys = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategorys.Add(int.Parse(li.Value));
            }
            if (regionList.Any() && provinceList.Any())
            {
                //List<int> guidanceIdList = GetGuidanceSelected();
                //var list1 = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                //             join subject in CurrentContext.DbContext.Subject
                //             on order.SubjectId equals subject.Id
                //             join shop in CurrentContext.DbContext.Shop
                //             on order.ShopId equals shop.Id
                //             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                //                 //&& (regionList.Contains(shop.RegionName.ToLower()) || regionList.Contains(subject.PriceBlongRegion.ToLower()))
                //             && ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(shop.RegionName.ToLower()))
                //             
                //               && (order.IsDelete == null || order.IsDelete == false)
                //             && (subject.IsDelete == null || subject.IsDelete == false)
                //             && subject.ApproveState == 1
                //             && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                //             select new { subject, order, shop }).ToList();
                if (Session["orderDetailStatistics"] == null)
                {
                    LoadSessionData();
                }
                List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList = new List<Subject>();
                if (Session["orderDetailStatistics"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
                if (Session["shopStatistics"] != null)
                    shopList = Session["shopStatistics"] as List<Shop>;
                if (Session["subjectStatistics"] != null)
                    subjectList = Session["subjectStatistics"] as List<Subject>;

                var list1 = (from order in finalOrderDetailTempList
                             join shop in shopList
                             on order.ShopId equals shop.Id
                             join subject in subjectList
                             on order.SubjectId equals subject.Id
                             //where ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower()))
                             where regionList.Contains(order.Region.ToLower())
                                          
                             select new
                             {
                                 order,
                                 shop,
                                 subject

                             }).ToList();
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
                if (subjectChannel1 == 1)
                {
                    //上海系统单
                    list1 = list1.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel1 == 2)
                {
                    //分区订单
                    list1 = list1.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (addUsers.Any())
                {
                    list1 = list1.Where(s => addUsers.Contains(s.subject.AddUserId ?? 0)).ToList();
                }
                //if (subjectCategorys.Any())
                //{
                //    list1 = list1.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                //}
                if (subjectCategorys.Any())
                {
                    if (subjectCategorys.Contains(0))
                    {
                        subjectCategorys.Remove(0);
                        if (subjectCategorys.Any())
                        {
                            list1 = list1.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            list1 = list1.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();
                    }
                    else
                        list1 = list1.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (provinceList.Any())
                {
                    list1 = list1.Where(s => provinceList.Contains(s.order.Province)).ToList();

                    var cityList = list1.OrderBy(s => s.order.Region).ThenBy(s => s.order.Province).Select(s => s.order.City).Distinct().ToList();
                    if (cityList.Any())
                    {
                        List<string> selectedList = new List<string>();
                        if (Session["citySelected"] != null)
                        {
                            selectedList = Session["citySelected"] as List<string>;
                        }
                        cityList.ForEach(s =>
                        {
                            ListItem li = new ListItem();
                            li.Text = s + "&nbsp;";
                            li.Value = s;
                            if (selectedList.Contains(s))
                                li.Selected = true;
                            cblCity.Items.Add(li);
                        });
                    }
                }

            }
            //BindPriceSubjects();
            //BindSubjects();

        }

        void CustomerService()
        {
            cblCustomerService.Items.Clear();
            int subjectChannel1 = 0;
            List<string> shopTypeList = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                {
                    shopTypeList.Add(li.Value);
                }
            }
            foreach (ListItem li in cblSubjectChannel.Items)
            {
                if (li.Selected)
                {
                    subjectChannel1 = int.Parse(li.Value);
                    break;
                }
            }
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                {
                    regionList.Add(li.Value.ToLower());
                }
            }
            if (!regionList.Any() && GetResponsibleRegion.Any())
            {
                GetResponsibleRegion.ForEach(s =>
                {
                    regionList.Add(s.ToLower());
                });
            }
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinceList.Contains(li.Value))
                    provinceList.Add(li.Value);
            }
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected && !cityList.Contains(li.Value))
                    cityList.Add(li.Value);
            }
            List<int> addUsers = new List<int>();
            foreach (ListItem li in cblAddUser.Items)
            {
                if (li.Selected)
                    addUsers.Add(int.Parse(li.Value));
            }
            List<int> subjectCategorys = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategorys.Add(int.Parse(li.Value));
            }

            //List<int> guidanceIdList = GetGuidanceSelected();
            //var list1 = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //             join subject in CurrentContext.DbContext.Subject
            //             on order.SubjectId equals subject.Id
            //             join shop in CurrentContext.DbContext.Shop
            //             on order.ShopId equals shop.Id
            //             join user in CurrentContext.DbContext.UserInfo
            //             on shop.CSUserId equals user.UserId into userTemp
            //             from customerService in userTemp.DefaultIfEmpty()
            //             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
            //                 //&& (regionList.Contains(shop.RegionName.ToLower()) || regionList.Contains(subject.PriceBlongRegion.ToLower()))
            //                 //&& ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(shop.RegionName.ToLower()))
            //             && (order.IsDelete == null || order.IsDelete == false)
            //             && (subject.IsDelete == null || subject.IsDelete == false)
            //             && subject.ApproveState == 1
            //             && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
            //             select new { subject, order, shop, customerService }).ToList();
            if (Session["orderDetailStatistics"] == null)
            {
                LoadSessionData();
            }
            List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailStatistics"] != null)
                finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
            if (Session["shopStatistics"] != null)
                shopList = Session["shopStatistics"] as List<Shop>;
            if (Session["subjectStatistics"] != null)
                subjectList = Session["subjectStatistics"] as List<Subject>;

            var list1 = (from order in finalOrderDetailTempList
                         join shop in shopList
                         on order.ShopId equals shop.Id
                         join subject in subjectList
                         on order.SubjectId equals subject.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on order.CSUserId equals user.UserId into userTemp
                         from customerService in userTemp.DefaultIfEmpty()
                         //where (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower())) : 1 == 1)
                         where (regionList.Any() ? regionList.Contains(order.Region.ToLower()) : 1 == 1)
                        
                         select new
                         {
                             order,
                             shop,
                             subject,
                             customerService
                         }).ToList();
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
            if (subjectChannel1 == 1)
            {
                //上海系统单
                list1 = list1.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
            }
            else if (subjectChannel1 == 2)
            {
                //分区订单
                list1 = list1.Where(s => s.order.IsFromRegion == true).ToList();
            }
            if (addUsers.Any())
            {
                list1 = list1.Where(s => addUsers.Contains(s.subject.AddUserId ?? 0)).ToList();
            }
            //if (subjectCategorys.Any())
            //{
            //    list1 = list1.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
            //}
            if (subjectCategorys.Any())
            {
                if (subjectCategorys.Contains(0))
                {
                    subjectCategorys.Remove(0);
                    if (subjectCategorys.Any())
                    {
                        list1 = list1.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        list1 = list1.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();
                }
                else
                    list1 = list1.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
            }
            //if (regionList.Any())
            //{
            //list1 = list1.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            //}
            if (provinceList.Any())
            {
                list1 = list1.Where(s => provinceList.Contains(s.order.Province)).ToList();
            }
            if (cityList.Any())
            {
                list1 = list1.Where(s => cityList.Contains(s.order.City)).ToList();
            }
            bool isEmpty = false;
            if (list1.Any())
            {
                var list = list1.Select(s => new { s.order.CSUserId, s.customerService }).Distinct().ToList();
                list.ForEach(s =>
                {
                    if (s.CSUserId != null && s.CSUserId > 0)
                    {

                        ListItem li = new ListItem();
                        li.Text = s.customerService.RealName + "&nbsp;";
                        li.Value = s.CSUserId.ToString();

                        cblCustomerService.Items.Add(li);
                    }
                    else
                        isEmpty = true;
                });
                if (isEmpty)
                {
                    ListItem li = new ListItem();
                    li.Text = "空&nbsp;";
                    li.Value = "0";
                    cblCustomerService.Items.Add(li);
                }
            }
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
            BindSecondInstallSubjects();
            CustomerService();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> provinceSelected = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                    provinceSelected.Add(li.Value);
            }
            Session["provinceSelected"] = provinceSelected;
            BindCity();
            //BindSubjects();
            CustomerService();
            BindSubjectNameList();
        }

        protected void cblCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> citySelected = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                    citySelected.Add(li.Value);
            }
            Session["citySelected"] = citySelected;
            BindSubjectNameList();
            //BindSubjects();
            //CustomerService();
        }

        protected void gvList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int subjectId = int.Parse(e.CommandArgument.ToString());
            Subject model = new SubjectBLL().GetModel(subjectId);
            if (model != null)
            {
                if (e.CommandName == "Check")
                {
                    string url = "SubjectStatistics.aspx";
                    int subjectType = model.SubjectType ?? 1;
                    if (subjectType == (int)SubjectTypeEnum.新开店安装费)
                    {
                        url = "PriceSubjectStatistic.aspx";
                    }
                    if (subjectType == (int)SubjectTypeEnum.二次安装)
                    {
                        url = "/Subjects/SecondInstallFee/CheckOrderDetail.aspx";
                    }
                    StringBuilder regions = new StringBuilder();
                    StringBuilder provinces = new StringBuilder();
                    StringBuilder citys = new StringBuilder();
                    StringBuilder customerServiceIds = new StringBuilder();
                    foreach (ListItem li in cblRegion.Items)
                    {
                        if (li.Selected)
                        {
                            regions.Append(li.Value);
                            regions.Append(",");
                        }
                    }
                    foreach (ListItem li in cblProvince.Items)
                    {
                        if (li.Selected)
                        {
                            provinces.Append(li.Value);
                            provinces.Append(",");
                        }
                    }
                    foreach (ListItem li in cblCity.Items)
                    {
                        if (li.Selected)
                        {
                            citys.Append(li.Value);
                            citys.Append(",");
                        }
                    }
                    foreach (ListItem li in cblCustomerService.Items)
                    {
                        if (li.Selected)
                        {
                            customerServiceIds.Append(li.Value);
                            customerServiceIds.Append(",");
                        }
                    }
                    url = string.Format("{0}?subjectId={1}&region={2}&province={3}&city={4}&customerServiceId={5}", url, subjectId, regions.ToString().TrimEnd(','), provinces.ToString().TrimEnd(','), citys.ToString().TrimEnd(','), customerServiceIds.ToString().TrimEnd(','));
                    if (subjectType == (int)SubjectTypeEnum.二次安装)
                    {
                        url += "&isCheck=1";
                    }
                    Response.Redirect(url, false);
                }
            }
        }

        /// <summary>
        /// 按时间获取活动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGetGuidance_Click(object sender, EventArgs e)
        {
            string begin = txtGuidanceBegin.Text.Trim();
            string end = txtGuidanceEnd.Text.Trim();
            if (!string.IsNullOrWhiteSpace(begin) && !string.IsNullOrWhiteSpace(end))
            {
                labBeginDate.Text = begin;
                labEndDate.Text = end;
                labSeparator.Visible = true;
                BindGuidance(1);
                BindShopType();
                BindAddUser();
                BindSubjectCategory();
                BindProvince();
                BindPriceSubjects();
                BindSubjects();
                BindSecondInstallSubjects();
            }
            else
            {
                labBeginDate.Text = "";
                labEndDate.Text = "";
                labSeparator.Visible = false;
            }

        }

        /// <summary>
        /// 选择活动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            GuidanceSelectChange();
            //BindSecondInstallSubjects();
        }

        protected void cblAddUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<int> userSelected = new List<int>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                    userSelected.Add(int.Parse(li.Value));
            }
            Session["userSelected"] = userSelected;
            BindSubjectCategory();
            //BindProvince();
            BindPriceSubjects();
            //BindSubjects();
            //BindSecondInstallSubjects();
            BindSubjectNameList();
        }

        protected void cblSubjectCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<int> subjectCategorySelected = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategorySelected.Add(int.Parse(li.Value));
            }
            Session["subjectCategorySelected"] = subjectCategorySelected;
            BindPriceSubjects();
            //BindSubjects();
            //BindSecondInstallSubjects();
            BindSubjectNameList();
        }

        protected void cblSubjectChannel_SelectedIndexChanged(object sender, EventArgs e)
        {

            BindShopType();
            BindAddUser();
            BindSubjectCategory();
            //BindSubjects();
            //BindSecondInstallSubjects();
            //BindPriceSubjects();
            BindSubjectNameList();
        }

        protected void cblShopType_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> shopTypeSelected = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                    shopTypeSelected.Add(li.Value);
            }
            Session["shopTypeSelected"] = shopTypeSelected;
            BindAddUser();
            BindSubjectCategory();
            BindProvince();
            //BindPriceSubjects();
            //BindSubjects();
            //BindSecondInstallSubjects();
            BindSubjectNameList();
        }

        protected void cblSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<int> subjectSelected = new List<int>();
            foreach (ListItem li in cblSubjects.Items)
            {
                if (li.Selected)
                    subjectSelected.Add(int.Parse(li.Value));
            }
            Session["subjectSelected"] = subjectSelected;
        }

        protected void cblSecondInstallSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            //List<int> secondInstallSubjectSelected = new List<int>();
            //foreach (ListItem li in cblSecondInstallSubjects.Items)
            //{
            //    if (li.Selected)
            //        secondInstallSubjectSelected.Add(int.Parse(li.Value));
            //}
            //Session["secondInstallSubjectSelected"] = secondInstallSubjectSelected;
        }

        protected void cblPriceSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<int> priceSubjectSelected = new List<int>();
            foreach (ListItem li in cblPriceSubjects.Items)
            {
                if (li.Selected)
                    priceSubjectSelected.Add(int.Parse(li.Value));
            }
            Session["priceSubjectSelected"] = priceSubjectSelected;
        }

        protected void lbUp_Click(object sender, EventArgs e)
        {
            //string begin = labBeginDate.Text;
            //if (!string.IsNullOrWhiteSpace(begin) && StringHelper.IsDateTime(begin))
            //{
            //    DateTime date = DateTime.Parse(begin);
            //    int year = date.Year;
            //    int month = date.Month;
            //    if (month <= 1)
            //    {
            //        year--;
            //        month = 12;
            //    }
            //    else
            //        month--;
            //    //DateTime newDate = new DateTime(year, month, 1);
            //    int customerId = int.Parse(ddlCustomer.SelectedValue);
            //    List<SubjectGuidance> guidanceList = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && s.BeginDate.Value.Year == year && s.BeginDate.Value.Month == month);
            //    if (guidanceList.Any())
            //    {

            //        labBeginDate.Text = DateTime.Parse(guidanceList.OrderBy(s=>s.BeginDate).ToList()[0].BeginDate.ToString()).ToShortDateString();
            //        labEndDate.Text = DateTime.Parse(guidanceList.OrderByDescending(s => s.EndDate).ToList()[0].EndDate.ToString()).ToShortDateString();
            //        BindGuidance();
            //        BindShopType();
            //        BindAddUser();
            //        BindSubjectCategory();
            //        BindProvince();
            //        BindPriceSubjects();
            //        BindSubjects();
            //        BindSecondInstallSubjects();
            //    }



            //}
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month <= 1)
                {
                    year--;
                    month = 12;
                }
                else
                    month--;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                BindShopType();
                BindAddUser();
                BindSubjectCategory();
                //BindProvince();
                //BindPriceSubjects();
                //BindSubjects();
                //BindSecondInstallSubjects();
            }
        }

        protected void lbDown_Click(object sender, EventArgs e)
        {
            //string end = labEndDate.Text;
            //if (!string.IsNullOrWhiteSpace(end) && StringHelper.IsDateTime(end))
            //{
            //    DateTime date = DateTime.Parse(end);
            //    int year = date.Year;
            //    int month = date.Month;
            //    if (month >=12)
            //    {
            //        year++;
            //        month = 1;
            //    }
            //    else
            //        month++;
            //    //DateTime newDate = new DateTime(year, month, 1);
            //    int customerId = int.Parse(ddlCustomer.SelectedValue);
            //    List<SubjectGuidance> guidanceList = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && s.BeginDate.Value.Year == year && s.BeginDate.Value.Month == month);
            //    if (guidanceList.Any())
            //    {

            //        labBeginDate.Text = DateTime.Parse(guidanceList.OrderBy(s => s.BeginDate).ToList()[0].BeginDate.ToString()).ToShortDateString();
            //        labEndDate.Text = DateTime.Parse(guidanceList.OrderByDescending(s => s.EndDate).ToList()[0].EndDate.ToString()).ToShortDateString();

            //        BindGuidance();
            //        BindShopType();
            //        BindAddUser();
            //        BindSubjectCategory();
            //        BindProvince();
            //        BindPriceSubjects();
            //        BindSubjects();
            //        BindSecondInstallSubjects();
            //    }
            //}
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month >= 12)
                {
                    year++;
                    month = 1;
                }
                else
                    month++;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                BindShopType();
                BindAddUser();
                BindSubjectCategory();
                //BindProvince();
                //BindPriceSubjects();
                //BindSubjects();
                //BindSecondInstallSubjects();
            }
        }

        protected void cblShopType_SelectedIndexChanged1(object sender, EventArgs e)
        {
            List<string> shopTypeSelected = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                    shopTypeSelected.Add(li.Value);
            }
            Session["shopTypeSelected"] = shopTypeSelected;
            BindAddUser();
            BindSubjectCategory();
            BindProvince();
            BindPriceSubjects();
            BindSubjects();
            //BindSecondInstallSubjects();
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text.Trim()))
            {
                BindGuidance();
                BindShopType();
                BindAddUser();
                BindSubjectCategory();
                BindProvince();
                BindPriceSubjects();
                BindSubjects();
                //BindSecondInstallSubjects();
            }
        }

        //protected void cbCheckAllGuidance_CheckedChanged(object sender, EventArgs e)
        //{
        //    bool isChecked=cbCheckAllGuidance.Checked;
        //    if (cblGuidanceList.Items.Count > 0)
        //    {
        //        foreach (ListItem li in cblGuidanceList.Items)
        //        {
        //            li.Selected = isChecked;
        //        }
        //        GuidanceSelectChange();
        //    }

        //}

        void GuidanceSelectChange()
        {
            LoadSessionData();
            BindShopType();
            BindAddUser();
            BindSubjectCategory();
            BindProvince();
            CustomerService();
            //BindPriceSubjects();
            //BindSubjects();
            BindSubjectNameList();
        }

        protected void btnCheckAllGuidance_Click(object sender, EventArgs e)
        {
            GuidanceSelectChange();
        }

        void BindSubjectList()
        {
            //try
            //{
                guidanceIdList = GetGuidanceSelected();
                subjectIdList = GetSubjectSelected();
                priceSubjectIdList = GetPriceSubjectSelected();

                foreach (ListItem li in cblShopType.Items)
                {
                    if (li.Selected)
                    {
                        shopTypeList.Add(li.Value);
                    }
                }
                foreach (ListItem li in cblAddUser.Items)
                {
                    if (li.Selected)
                        addUserList.Add(int.Parse(li.Value));
                }

                foreach (ListItem li in cblSubjectCategory.Items)
                {
                    if (li.Selected)
                        subjectCategoryList.Add(int.Parse(li.Value));
                }
                foreach (ListItem li in cblRegion.Items)
                {
                    if (li.Selected && !regionList.Contains(li.Value.ToLower()))
                    {
                        regionList.Add(li.Value.ToLower());
                    }
                }
                foreach (ListItem li in cblProvince.Items)
                {
                    if (li.Selected && !provinceList.Contains(li.Value.Trim()))
                    {
                        provinceList.Add(li.Value.Trim());
                    }
                }
                foreach (ListItem li in cblCity.Items)
                {
                    if (li.Selected && !cityList.Contains(li.Value.Trim()))
                    {
                        cityList.Add(li.Value.Trim());
                    }
                }
                foreach (ListItem li in cblCustomerService.Items)
                {
                    if (li.Selected)
                        customerServiceIds.Add(int.Parse(li.Value));
                }
                if (!regionList.Any() && GetResponsibleRegion.Any())
                {
                    GetResponsibleRegion.ForEach(s =>
                    {
                        regionList.Add(s.ToLower());
                    });
                }

                foreach (ListItem li in cblSubjectChannel.Items)
                {
                    if (li.Selected)
                    {
                        subjectChannel = int.Parse(li.Value);
                        break;
                    }
                }
                List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList = new List<Subject>();
                if (Session["orderDetailStatistics"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
                if (Session["shopStatistics"] != null)
                    shopList = Session["shopStatistics"] as List<Shop>;
                if (Session["subjectStatistics"] != null)
                    subjectList = Session["subjectStatistics"] as List<Subject>;

                var orderList0 = (from order in finalOrderDetailTempList
                                  join shop in shopList
                                  on order.ShopId equals shop.Id
                                  join subject in subjectList
                                  on order.SubjectId equals subject.Id
                                  select new
                                  {
                                      order,
                                      shop,
                                      subject

                                  }).ToList();

                if (addUserList.Any())
                {
                    orderList0 = orderList0.Where(s => addUserList.Contains(s.subject.AddUserId ?? 0)).ToList();
                }

                if (subjectCategoryList.Any())
                {
                    if (subjectCategoryList.Contains(0))
                    {
                        subjectCategoryList.Remove(0);
                        if (subjectCategoryList.Any())
                        {
                            orderList0 = orderList0.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderList0 = orderList0.Where(s => s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (subjectChannel == 1)
                {
                    //上海系统单
                    orderList0 = orderList0.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel == 2)
                {
                    //分区订单
                    orderList0 = orderList0.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (regionList.Any())
                {
                    orderList0 = orderList0.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
                    if (provinceList.Any())
                    {
                        orderList0 = orderList0.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                        if (cityList.Any())
                            orderList0 = orderList0.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                    }
                }
                string begin = txtSubjectBegin.Text.Trim();
                string end = txtSubjectEnd.Text.Trim();
                if (!string.IsNullOrWhiteSpace(begin))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    orderList0 = orderList0.Where(s => s.subject.AddDate >= beginDate).ToList();
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        DateTime endDate = DateTime.Parse(end).AddDays(1);
                        orderList0 = orderList0.Where(s => s.subject.AddDate < endDate).ToList();
                    }
                }
                if ((subjectIdList.Any() || priceSubjectIdList.Any()) == false)
                {
                    foreach (ListItem li in cblSubjects.Items)
                        subjectIdList.Add(int.Parse(li.Value));

                    foreach (ListItem li in cblPriceSubjects.Items)
                        priceSubjectIdList.Add(int.Parse(li.Value));
                }

                orderList0 = orderList0.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
                if (customerServiceIds.Any())
                {
                    if (customerServiceIds.Contains(0))
                    {
                        customerServiceIds.Remove(0);
                        if (customerServiceIds.Any())
                        {
                            orderList0 = orderList0.Where(s => customerServiceIds.Contains(s.shop.CSUserId ?? 0) || (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                        }
                        else
                        {
                            orderList0 = orderList0.Where(s => (s.shop.CSUserId == null || s.shop.CSUserId == 0)).ToList();

                        }
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s => customerServiceIds.Contains(s.shop.CSUserId ?? 0)).ToList();

                    }
                }
                List<int> subjectIds = new List<int>();
                subjectIds = orderList0.Select(s => s.subject.Id).Distinct().ToList();
                if (priceSubjectIdList.Any())
                {
                    subjectIds.AddRange(priceSubjectIdList);
                }

                subjectIdList.ForEach(s =>
                {
                    if (!subjectIds.Contains(s))
                        subjectIds.Add(s);
                });

                var list = (from subject in subjectList
                            join customer in CurrentContext.DbContext.Customer
                            on subject.CustomerId equals customer.Id
                            join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                            on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                            from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                            where subjectIds.Contains(subject.Id)
                            select new
                            {
                                subject,
                                subject.Id,
                                subject.SubjectType,
                                subject.SubjectCategoryId,
                                customer.CustomerName,
                                subject.SubjectName,
                                subject.Remark,
                                subject.PriceBlongRegion,
                                CategoryName = subjectCategory!=null?subjectCategory.CategoryName:"空"
                            }
                                ).ToList();
                gvList.DataSource = list;
                gvList.DataBind();
            //}
            //catch (Exception ex)
            //{ 
            
            //}
        }

        void ShowSubjectList()
        {
            if (cbShopSubjectList.Checked)
            {
                gvList.Visible = true;
                BindSubjectList();
            }
            else
            {
                gvList.Visible = false;
            }
        }

        void BindSubjectNameList()
        {
            BindPriceSubjects();
            BindSubjects();
            if (cbShowSubjectNameList.Checked)
            {
                //Panel_SubjectNameList.Visible = true;
                Panel_SubjectNameList.Style.Add("display","block");
                
            }
            else
            {
                Panel_SubjectNameList.Style.Add("display", "none");
                //Panel_SubjectNameList.Visible = false;
            }
        }

        /// <summary>
        /// 绑定费用项目
        /// </summary>
        void BindPriceSubjects()
        {
            cbAll0.Checked = false;
            cblPriceSubjects.Items.Clear();
            
            List<int> guidanceIdList = GetGuidanceSelected();
            int subjectChannel1 = 0;
            foreach (ListItem li in cblSubjectChannel.Items)
            {
                if (li.Selected)
                {
                    subjectChannel1 = int.Parse(li.Value);
                    break;
                }
            }
            List<string> regions = new List<string>();
            List<string> provinces = new List<string>();
            List<string> citys = new List<string>();
            List<int> addUsers = new List<int>();
            List<int> subjectCategorys = new List<int>();
            List<string> shopTypeList = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                {
                    shopTypeList.Add(li.Value);
                }
            }
            foreach (ListItem li in cblAddUser.Items)
            {
                if (li.Selected)
                    addUsers.Add(int.Parse(li.Value));
            }

            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategorys.Add(int.Parse(li.Value));
            }
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinces.Contains(li.Value))
                    provinces.Add(li.Value);
            }
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected && !citys.Contains(li.Value))
                {
                    citys.Add(li.Value);
                }
            }
            if (!regions.Any() && GetResponsibleRegion.Any())
            {
                GetResponsibleRegion.ForEach(s =>
                {
                    regions.Add(s.ToLower());
                });
            }
            try
            {

                if (Session["orderDetail"] == null)
                {
                    LoadSessionData();
                }
                List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList1 = new List<Subject>();
                if (Session["orderDetailStatistics"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
                if (Session["shopStatistics"] != null)
                    shopList = Session["shopStatistics"] as List<Shop>;
                if (Session["subjectStatistics"] != null)
                    subjectList1 = Session["subjectStatistics"] as List<Subject>;

                var orderList = (from order in finalOrderDetailTempList
                                 join shop in shopList
                                 on order.ShopId equals shop.Id
                                 join subject in subjectList1
                                 on order.SubjectId equals subject.Id
                                 join user in CurrentContext.DbContext.UserInfo
                                 on subject.AddUserId equals user.UserId

                                 select new
                                 {
                                     order,
                                     shop,
                                     subject

                                 }).ToList();
                List<int> subjectIdInOrderList = orderList.Select(s => s.subject.Id).ToList();
                if (shopTypeList.Any())
                {
                    if (shopTypeList.Contains("空"))
                    {
                        shopTypeList.Remove("空");
                        if (shopTypeList.Any())
                        {
                            orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                    }
                    else
                        orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                }
                if (addUsers.Any())
                {
                    orderList = orderList.Where(s => addUsers.Contains(s.subject.AddUserId ?? 0)).ToList();
                }

                if (subjectCategorys.Any())
                {
                    if (subjectCategorys.Contains(0))
                    {
                        subjectCategorys.Remove(0);
                        if (subjectCategorys.Any())
                        {
                            orderList = orderList.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        orderList = orderList.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (subjectChannel1 == 1)
                {
                    //上海系统单
                    orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel1 == 2)
                {
                    //分区订单
                    orderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (regions.Any())
                {
                    //orderList = orderList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regions.Contains(s.subject.PriceBlongRegion.ToLower()) : regions.Contains(s.order.Region.ToLower())).ToList();
                    orderList = orderList.Where(s => regions.Contains(s.order.Region.ToLower())).ToList();
                    
                    if (provinces.Any())
                    {
                        orderList = orderList.Where(s => provinces.Contains(s.order.Province)).ToList();
                        if (citys.Any())
                            orderList = orderList.Where(s => citys.Contains(s.order.City)).ToList();
                    }
                }
                string begin = txtSubjectBegin.Text.Trim();
                string end = txtSubjectEnd.Text.Trim();
                if (!string.IsNullOrWhiteSpace(begin))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    orderList = orderList.Where(s => s.subject.AddDate >= beginDate).ToList();
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        DateTime endDate = DateTime.Parse(end).AddDays(1);
                        orderList = orderList.Where(s => s.subject.AddDate < endDate).ToList();
                    }
                }

                var subjectList = orderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.费用订单).Select(s => s.subject).Distinct().OrderBy(s => s.GuidanceId).ThenBy(s => s.SubjectName).ToList();

                if (subjectList.Any())
                {
                    cbAllDiv.Style.Add("display", "block");
                }
                else
                {
                    cbAllDiv.Style.Add("display", "none");
                }
                List<int> selectedList = new List<int>();
                if (Session["subjectSelected"] != null)
                {
                    selectedList = Session["subjectSelected"] as List<int>;
                }
                subjectList.OrderBy(s => s.Id).ToList().ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string subjectName = s.SubjectName;
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(s.Remark))
                    {
                        subjectName += "-" + s.Remark;
                    }
                    if (s.PriceBlongRegion != null && s.PriceBlongRegion != "")
                    {
                        subjectName += "-" + s.PriceBlongRegion + "实施";
                    }
                    //li.Text = subjectName + "&nbsp;&nbsp;";
                    li.Text = "<span name='spanCheckSubject' data-subjectid='" + s.Id + "' style='cursor:pointer;'>" + subjectName + "</span>&nbsp;&nbsp;";
                    li.Value = s.Id.ToString();
                    if (selectedList.Contains(s.Id))
                        li.Selected = true;
                    cblPriceSubjects.Items.Add(li);
                });

                var newShopInstallPriceOrderList = (from order in CurrentContext.DbContext.PriceOrderDetail
                                                   join subject in CurrentContext.DbContext.Subject
                                                   on order.SubjectId equals subject.Id
                                                   where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                                   && (subject.IsDelete == null || subject.IsDelete == false)
                                                   && (subject.SubjectType == (int)SubjectTypeEnum.新开店安装费 || subject.SubjectType == (int)SubjectTypeEnum.运费)
                                                   && subject.ApproveState == 1
                                                   select new {
                                                       order,
                                                       subject
                                                   }).ToList();

                if (regions.Any())
                {
                    //newShopInstallPriceOrderList = newShopInstallPriceOrderList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regions.Contains(s.subject.PriceBlongRegion.ToLower()) : regions.Contains(s.order.Region.ToLower())).ToList();
                    newShopInstallPriceOrderList = newShopInstallPriceOrderList.Where(s => regions.Contains(s.order.Region.ToLower())).ToList();
                    
                    if (provinces.Any())
                    {
                        newShopInstallPriceOrderList = newShopInstallPriceOrderList.Where(s => provinces.Contains(s.order.Province)).ToList();
                        if (citys.Any())
                            newShopInstallPriceOrderList = newShopInstallPriceOrderList.Where(s => citys.Contains(s.order.City)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(begin))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    newShopInstallPriceOrderList = newShopInstallPriceOrderList.Where(s => s.subject.AddDate >= beginDate).ToList();
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        DateTime endDate = DateTime.Parse(end).AddDays(1);
                        newShopInstallPriceOrderList = newShopInstallPriceOrderList.Where(s => s.subject.AddDate < endDate).ToList();
                    }
                }

                //var newShopInstallPriceSList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && (s.SubjectType == (int)SubjectTypeEnum.新开店安装费 || s.SubjectType == (int)SubjectTypeEnum.运费) && s.ApproveState == 1);
                var newShopInstallPriceSubjectList = newShopInstallPriceOrderList.Select(s=>s.subject).Distinct().ToList();
                if (newShopInstallPriceSubjectList.Any())
                {
                    newShopInstallPriceSubjectList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        string subjectName = s.SubjectName;
                        li.Text = "<span name='spanCheckSubject' data-subjectid='" + s.Id + "' style='cursor:pointer;'>" + subjectName + "</span>&nbsp;&nbsp;";
                        if (selectedList.Contains(s.Id))
                            li.Selected = true;
                        cblPriceSubjects.Items.Add(li);
                    });
                }

            }
            catch (Exception ex)
            { }


        }

        /// <summary>
        /// 绑定正常项目
        /// </summary>
        void BindSubjects()
        {
            cbAll.Checked = false;
            cblSubjects.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<int> guidanceIdList = GetGuidanceSelected();
            int subjectChannel1 = 0;
            foreach (ListItem li in cblSubjectChannel.Items)
            {
                if (li.Selected)
                {
                    subjectChannel1 = int.Parse(li.Value);
                    break;
                }
            }
            List<string> regions = new List<string>();
            List<string> provinces = new List<string>();
            List<string> citys = new List<string>();
            List<int> addUsers = new List<int>();
            List<int> subjectCategorys = new List<int>();
            List<string> shopTypeList = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                {
                    shopTypeList.Add(li.Value);
                }
            }
            foreach (ListItem li in cblAddUser.Items)
            {
                if (li.Selected)
                    addUsers.Add(int.Parse(li.Value));
            }

            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategorys.Add(int.Parse(li.Value));
            }
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !provinces.Contains(li.Value))
                    provinces.Add(li.Value);
            }
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected && !citys.Contains(li.Value))
                {
                    citys.Add(li.Value);
                }
            }
            if (!regions.Any() && GetResponsibleRegion.Any())
            {
                GetResponsibleRegion.ForEach(s =>
                {
                    regions.Add(s.ToLower());
                });
            }
            try
            {

                if (Session["orderDetail"] == null)
                {
                    LoadSessionData();
                }
                List<FinalOrderDetailTemp> finalOrderDetailTempList = new List<FinalOrderDetailTemp>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList1 = new List<Subject>();
                if (Session["orderDetailStatistics"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatistics"] as List<FinalOrderDetailTemp>;
                if (Session["shopStatistics"] != null)
                    shopList = Session["shopStatistics"] as List<Shop>;
                if (Session["subjectStatistics"] != null)
                    subjectList1 = Session["subjectStatistics"] as List<Subject>;

                var orderList = (from order in finalOrderDetailTempList
                                 join shop in shopList
                                 on order.ShopId equals shop.Id
                                 join subject in subjectList1
                                 on order.SubjectId equals subject.Id
                                 join user in CurrentContext.DbContext.UserInfo
                                 on subject.AddUserId equals user.UserId

                                 select new
                                 {
                                     order,
                                     shop,
                                     subject

                                 }).ToList();
                List<int> subjectIdInOrderList = orderList.Select(s => s.subject.Id).ToList();
                if (shopTypeList.Any())
                {
                    if (shopTypeList.Contains("空"))
                    {
                        shopTypeList.Remove("空");
                        if (shopTypeList.Any())
                        {
                            orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                    }
                    else
                        orderList = orderList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                }
                if (addUsers.Any())
                {
                    orderList = orderList.Where(s => addUsers.Contains(s.subject.AddUserId ?? 0)).ToList();
                }

                if (subjectCategorys.Any())
                {
                    if (subjectCategorys.Contains(0))
                    {
                        subjectCategorys.Remove(0);
                        if (subjectCategorys.Any())
                        {
                            orderList = orderList.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        orderList = orderList.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (subjectChannel1 == 1)
                {
                    //上海系统单
                    orderList = orderList.Where(s => s.order.IsFromRegion == null || s.order.IsFromRegion == false).ToList();
                }
                else if (subjectChannel1 == 2)
                {
                    //分区订单
                    orderList = orderList.Where(s => s.order.IsFromRegion == true).ToList();
                }
                if (regions.Any())
                {
                    //orderList = orderList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regions.Contains(s.subject.PriceBlongRegion.ToLower()) : regions.Contains(s.order.Region.ToLower())).ToList();
                    orderList = orderList.Where(s => regions.Contains(s.order.Region.ToLower())).ToList();
                    
                    if (provinces.Any())
                    {
                        orderList = orderList.Where(s => provinces.Contains(s.order.Province)).ToList();
                        if (citys.Any())
                            orderList = orderList.Where(s => citys.Contains(s.order.City)).ToList();
                    }
                }
                string begin = txtSubjectBegin.Text.Trim();
                string end = txtSubjectEnd.Text.Trim();
                if (!string.IsNullOrWhiteSpace(begin))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    orderList = orderList.Where(s => s.subject.AddDate >= beginDate).ToList();
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        DateTime endDate = DateTime.Parse(end).AddDays(1);
                        orderList = orderList.Where(s => s.subject.AddDate < endDate).ToList();
                    }
                }

                var subjectList = orderList.Where(s => s.subject.SubjectType != (int)SubjectTypeEnum.费用订单).Select(s => s.subject).Distinct().OrderBy(s => s.GuidanceId).ThenBy(s => s.SubjectName).ToList();

                if (subjectList.Any())
                {
                    cbAllDiv.Style.Add("display", "block");
                }
                else
                {
                    cbAllDiv.Style.Add("display", "none");
                }
                List<int> selectedList = new List<int>();
                if (Session["subjectSelected"] != null)
                {
                    selectedList = Session["subjectSelected"] as List<int>;
                }
                subjectList.OrderBy(s => s.Id).ToList().ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string subjectName = s.SubjectName;
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(s.Remark))
                    {
                        subjectName += "-" + s.Remark;
                    }
                    if (s.PriceBlongRegion != null && s.PriceBlongRegion != "")
                    {
                        subjectName += "-" + s.PriceBlongRegion + "实施";
                    }
                    li.Text = "<span name='spanCheckSubject' data-subjectid='" + s.Id + "' style='cursor:pointer;'>" + subjectName + "</span>&nbsp;&nbsp;";
                    li.Value = s.Id.ToString();
                    if (selectedList.Contains(s.Id))
                        li.Selected = true;
                    cblSubjects.Items.Add(li);
                });

                //List<int> materialSubjectList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1).Select(s => s.Id).ToList();
                //单独的物料项目（不含pop）
                var materialSubjectList = (from materialOrder in CurrentContext.DbContext.OrderMaterial
                                           join subject in CurrentContext.DbContext.Subject
                                           on materialOrder.SubjectId equals subject.Id
                                           join shop in CurrentContext.DbContext.Shop
                                           on materialOrder.ShopId equals shop.Id
                                           where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                           && (subject.IsDelete == null || subject.IsDelete == false)
                                           && subject.ApproveState == 1
                                           select new
                                           {
                                               materialOrder,
                                               subject,
                                               shop
                                           }
                                          ).ToList();
                materialSubjectList = materialSubjectList.Where(s => !subjectIdInOrderList.Contains(s.subject.Id)).ToList();
                if (materialSubjectList.Any())
                {
                    if (shopTypeList.Any())
                    {
                        if (shopTypeList.Contains("空"))
                        {
                            shopTypeList.Remove("空");
                            if (shopTypeList.Any())
                            {
                                materialSubjectList = materialSubjectList.Where(s => shopTypeList.Contains(s.shop.ShopType) || (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                            }
                            else
                                materialSubjectList = materialSubjectList.Where(s => (s.shop.ShopType == null || s.shop.ShopType == "")).ToList();
                        }
                        else
                            materialSubjectList = materialSubjectList.Where(s => shopTypeList.Contains(s.shop.ShopType)).ToList();
                    }
                    if (addUsers.Any())
                    {
                        materialSubjectList = materialSubjectList.Where(s => addUsers.Contains(s.subject.AddUserId ?? 0)).ToList();
                    }
                    if (subjectCategorys.Any())
                    {
                        materialSubjectList = materialSubjectList.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                    }
                    if (regions.Any())
                    {
                        materialSubjectList = materialSubjectList.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
                        if (provinces.Any())
                        {
                            materialSubjectList = materialSubjectList.Where(s => provinces.Contains(s.shop.ProvinceName)).ToList();
                            if (citys.Any())
                                materialSubjectList = materialSubjectList.Where(s => citys.Contains(s.shop.CityName)).ToList();
                        }
                    }
                    string begin0 = txtGuidanceBegin.Text.Trim();
                    string end0 = txtGuidanceEnd.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(begin0) && !string.IsNullOrWhiteSpace(end0))
                    {
                        DateTime beginDate = DateTime.Parse(begin0);
                        DateTime endDate = DateTime.Parse(end0).AddDays(1);
                        materialSubjectList = materialSubjectList.Where(s => s.subject.AddDate >= beginDate && s.subject.AddDate < endDate).ToList();
                    }
                    var subjectList0 = materialSubjectList.Select(s => s.subject).Distinct().OrderBy(s => s.GuidanceId).ToList();
                    subjectList0.OrderBy(s => s.Id).ToList().ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        string subjectName = s.SubjectName;

                        
                        li.Text = "<span name='spanCheckSubject' data-subjectid='" + s.Id + "' style='cursor:pointer;'>" + subjectName + "</span>&nbsp;&nbsp;";
                        li.Value = s.Id.ToString();
                        if (selectedList.Contains(s.Id))
                            li.Selected = true;
                        cblSubjects.Items.Add(li);
                    });
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 绑定二次安装费项目
        /// </summary>
        void BindSecondInstallSubjects()
        {
           
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<int> guidanceIdList = GetGuidanceSelected();

            List<string> regions = new List<string>();
            List<int> addUsers = new List<int>();
            List<int> subjectCategorys = new List<int>();
            int subjectChannel1 = 0;
            foreach (ListItem li in cblSubjectChannel.Items)
            {
                if (li.Selected)
                {
                    subjectChannel1 = int.Parse(li.Value);
                    break;
                }
            }
            foreach (ListItem li in cblAddUser.Items)
            {
                if (li.Selected)
                    addUsers.Add(int.Parse(li.Value));
            }

            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCategorys.Add(int.Parse(li.Value));
            }
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }

            if (!regions.Any() && GetResponsibleRegion.Any())
            {
                GetResponsibleRegion.ForEach(s =>
                {
                    regions.Add(s.ToLower());
                });
            }
            try
            {
                var List = (from subject in CurrentContext.DbContext.Subject
                            where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                            && subject.CustomerId == customerId
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            && subject.ApproveState == 1
                            && subject.SubjectType == 4
                            select new
                            {

                                subject
                                //shop
                            }).ToList();
                if (subjectChannel1 == 1)
                {
                    //上海系统单
                    List = List.Where(s => s.subject.SupplementRegion == null).ToList();
                }
                else if (subjectChannel1 == 2)
                {
                    //分区订单
                    List = List.Where(s => s.subject.SupplementRegion.Length > 0).ToList();
                }
                if (addUsers.Any())
                {
                    List = List.Where(s => addUsers.Contains(s.subject.AddUserId ?? 0)).ToList();
                }
                
                if (subjectCategorys.Any())
                {
                    if (subjectCategorys.Contains(0))
                    {
                        subjectCategorys.Remove(0);
                        if (subjectCategorys.Any())
                        {
                            List = List.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            List = List.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        List = List.Where(s => subjectCategorys.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (regions.Any())
                {
                    List = List.Where(s => regions.Contains(s.subject.Region)).ToList();
                }
                string begin = txtSubjectBegin.Text.Trim();
                string end = txtSubjectEnd.Text.Trim();
                if (!string.IsNullOrWhiteSpace(begin))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    List = List.Where(s => s.subject.AddDate >= beginDate).ToList();
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        DateTime endDate = DateTime.Parse(end).AddDays(1);
                        List = List.Where(s => s.subject.AddDate < endDate).ToList();
                    }
                }

                List<int> selectedList = new List<int>();
                if (Session["secondInstallSubjectSelected"] != null)
                {
                    selectedList = Session["secondInstallSubjectSelected"] as List<int>;
                }
                List.ForEach(s =>
                {
                    string name = s.subject.SubjectName;
                    string region = s.subject.Region;
                    if (!string.IsNullOrWhiteSpace(region))
                    {
                        name += "(" + region + ")";
                    }
                    ListItem li = new ListItem();
                    li.Text = name + "&nbsp;&nbsp;";
                    li.Value = s.subject.Id.ToString();
                    if (selectedList.Contains(s.subject.Id))
                        li.Selected = true;
                    //cblSecondInstallSubjects.Items.Add(li);
                });

            }
            catch (Exception ex)
            {

            }
        }

        protected void cbShopSubjectList_CheckedChanged(object sender, EventArgs e)
        {
            ShowSubjectList();
        }

        protected void cbShowSubjectNameList_CheckedChanged(object sender, EventArgs e)
        {
            BindSubjectNameList();
        }
    }
}
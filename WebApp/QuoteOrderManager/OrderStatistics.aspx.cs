using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.QuoteOrderManager
{
    public partial class OrderStatistics : BasePage
    {
        QuoteOrderDetailBLL orderBll = new QuoteOrderDetailBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                BindGuidance();
                BindRegion();
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
            List<QuoteOrderDetail> quoteOrderDetailList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false));

            var orderList = (from order in quoteOrderDetailList
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on order.GuidanceId equals guidance.ItemId
                             where (subject.IsDelete == null || subject.IsDelete == false)
                             && (order.IsDelete == null || order.IsDelete == false)
                             && subject.ApproveState == 1
                             select new { order, shop, subject, guidance }).ToList();
            if (orderList.Any())
            {
                Session["orderDetailStatisticsQuote"] = orderList.Select(s => s.order).ToList();
                Session["shopStatisticsQuote"] = orderList.Select(s => s.shop).Distinct().ToList();
                Session["subjectStatisticsQuote"] = orderList.Select(s => s.subject).Distinct().ToList();
                Session["guidanceStatisticsQuote"] = orderList.Select(s => s.guidance).Distinct().ToList();
            }
        }

        void BindGuidance(int? onDateSearch = null)
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);

            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance
                           
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

                if (Session["orderDetailStatisticsQuote"] == null)
                {
                    LoadSessionData();
                }
                List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList = new List<Subject>();
                if (Session["orderDetailStatisticsQuote"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
                if (Session["shopStatisticsQuote"] != null)
                    shopList = Session["shopStatisticsQuote"] as List<Shop>;
                if (Session["subjectStatisticsQuote"] != null)
                    subjectList = Session["subjectStatisticsQuote"] as List<Subject>;

                var list1 = (from order in finalOrderDetailTempList
                             join shop in shopList
                             on order.ShopId equals shop.Id
                             join subject in subjectList
                             on order.SubjectId equals subject.Id
                             where ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower()))

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
                        if (Session["provinceSelectedQuote"] != null)
                        {
                            selectedList = Session["provinceSelectedQuote"] as List<string>;
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
            //BindPriceSubjects();
            //BindSubjects();

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
                if (Session["orderDetailStatisticsQuote"] == null)
                {
                    LoadSessionData();
                }
                List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList = new List<Subject>();
                if (Session["orderDetailStatisticsQuote"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
                if (Session["shopStatisticsQuote"] != null)
                    shopList = Session["shopStatisticsQuote"] as List<Shop>;
                if (Session["subjectStatisticsQuote"] != null)
                    subjectList = Session["subjectStatisticsQuote"] as List<Subject>;

                var list1 = (from order in finalOrderDetailTempList
                             join shop in shopList
                             on order.ShopId equals shop.Id
                             join subject in subjectList
                             on order.SubjectId equals subject.Id
                             where ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower()))
                             //             
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
                if (provinceList.Any())
                {
                    list1 = list1.Where(s => provinceList.Contains(s.order.Province)).ToList();

                    var cityList = list1.OrderBy(s => s.order.Region).ThenBy(s => s.order.Province).Select(s => s.order.City).Distinct().ToList();
                    if (cityList.Any())
                    {
                        List<string> selectedList = new List<string>();
                        if (Session["citySelectedQuote"] != null)
                        {
                            selectedList = Session["citySelectedQuote"] as List<string>;
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

            if (Session["orderDetailStatisticsQuote"] == null)
            {
                LoadSessionData();
            }
            List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailStatisticsQuote"] != null)
                finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
            if (Session["shopStatisticsQuote"] != null)
                shopList = Session["shopStatisticsQuote"] as List<Shop>;
            if (Session["subjectStatisticsQuote"] != null)
                subjectList = Session["subjectStatisticsQuote"] as List<Subject>;

            var list1 = (from order in finalOrderDetailTempList
                         join shop in shopList
                         on order.ShopId equals shop.Id
                         join subject in subjectList
                         on order.SubjectId equals subject.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on order.CSUserId equals user.UserId into userTemp
                         from customerService in userTemp.DefaultIfEmpty()
                         where (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower())) : 1 == 1)
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

        void BindShopType()
        {
            cblShopType.Items.Clear();
            
            int subjectChannel1 = 0;
            if (Session["orderDetailStatisticsQuote"] == null)
            {
                LoadSessionData();
            }
            List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
            List<Shop> shopList = new List<Shop>();
            if (Session["orderDetailStatisticsQuote"] != null)
                finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
            if (Session["shopStatisticsQuote"] != null)
                shopList = Session["shopStatisticsQuote"] as List<Shop>;

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
                    if (Session["shopTypeSelectedQuote"] != null)
                    {
                        selectedList = Session["shopTypeSelectedQuote"] as List<string>;
                    }
                    
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
            
            cblAddUser.Items.Clear();

            if (Session["orderDetailStatisticsQuote"] == null)
            {
                LoadSessionData();
            }
            List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailStatisticsQuote"] != null)
                finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
            if (Session["shopStatisticsQuote"] != null)
                shopList = Session["shopStatisticsQuote"] as List<Shop>;
            if (Session["subjectStatisticsQuote"] != null)
                subjectList = Session["subjectStatisticsQuote"] as List<Subject>;
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
                    if (Session["userSelectedQuote"] != null)
                    {
                        selectedList = Session["userSelectedQuote"] as List<int>;
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
            if (Session["orderDetailStatisticsQuote"] == null)
            {
                LoadSessionData();
            }
            List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailStatisticsQuote"] != null)
                finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
            if (Session["shopStatisticsQuote"] != null)
                shopList = Session["shopStatisticsQuote"] as List<Shop>;
            if (Session["subjectStatisticsQuote"] != null)
                subjectList = Session["subjectStatisticsQuote"] as List<Subject>;

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
                    if (Session["subjectCategorySelectedQuote"] != null)
                    {
                        selectedList = Session["subjectCategorySelectedQuote"] as List<int>;
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

                if (Session["orderDetailStatisticsQuote"] == null)
                {
                    LoadSessionData();
                }
                List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList1 = new List<Subject>();
                if (Session["orderDetailStatisticsQuote"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
                if (Session["shopStatisticsQuote"] != null)
                    shopList = Session["shopStatisticsQuote"] as List<Shop>;
                if (Session["subjectStatisticsQuote"] != null)
                    subjectList1 = Session["subjectStatisticsQuote"] as List<Subject>;

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
                    orderList = orderList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regions.Contains(s.subject.PriceBlongRegion.ToLower()) : regions.Contains(s.order.Region.ToLower())).ToList();
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
                if (Session["subjectSelectedQuote"] != null)
                {
                    selectedList = Session["subjectSelectedQuote"] as List<int>;
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

                if (Session["orderDetailStatisticsQuote"] == null)
                {
                    LoadSessionData();
                }
                List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
                List<Shop> shopList = new List<Shop>();
                List<Subject> subjectList1 = new List<Subject>();
                if (Session["orderDetailStatisticsQuote"] != null)
                    finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
                if (Session["shopStatisticsQuote"] != null)
                    shopList = Session["shopStatisticsQuote"] as List<Shop>;
                if (Session["subjectStatisticsQuote"] != null)
                    subjectList1 = Session["subjectStatisticsQuote"] as List<Subject>;

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
                    orderList = orderList.Where(s => (s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regions.Contains(s.subject.PriceBlongRegion.ToLower()) : regions.Contains(s.order.Region.ToLower())).ToList();
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
                if (Session["subjectSelectedQuote"] != null)
                {
                    selectedList = Session["subjectSelectedQuote"] as List<int>;
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
        /// 显示和隐藏项目名称选项
        /// </summary>
        void BindSubjectNameList()
        {
            BindPriceSubjects();
            BindSubjects();
            if (cbShowSubjectNameList.Checked)
            {
                Panel_SubjectNameList.Visible = true;
            }
            else
            {
                Panel_SubjectNameList.Visible = false;
            }
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


        protected void lbUp_Click(object sender, EventArgs e)
        {
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
                
            }
        }

        protected void lbDown_Click(object sender, EventArgs e)
        {
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
               
            }
        }

        protected void btnGetGuidance_Click(object sender, EventArgs e)
        {

        }

        protected void cblSubjectChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindShopType();
            BindAddUser();
            BindSubjectCategory();
            BindSubjectNameList();
        }

        protected void cblShopType_SelectedIndexChanged1(object sender, EventArgs e)
        {
            List<string> shopTypeSelected = new List<string>();
            foreach (ListItem li in cblShopType.Items)
            {
                if (li.Selected)
                    shopTypeSelected.Add(li.Value);
            }
            Session["shopTypeSelectedQuote"] = shopTypeSelected;
            BindAddUser();
            BindSubjectCategory();
            BindProvince();
            BindSubjectNameList();
        }

        protected void cblAddUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<int> userSelected = new List<int>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                    userSelected.Add(int.Parse(li.Value));
            }
            Session["userSelectedQuote"] = userSelected;
            BindSubjectCategory();
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
            Session["subjectCategorySelectedQuote"] = subjectCategorySelected;
            //BindPriceSubjects();
            BindSubjectNameList();
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
            //BindSecondInstallSubjects();
            CustomerService();
            BindSubjectNameList();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> provinceSelected = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                    provinceSelected.Add(li.Value);
            }
            Session["provinceSelectedQuote"] = provinceSelected;
            BindCity();
            //BindSubjects();
            CustomerService();
            BindSubjectNameList();
        }

        protected void btnGetProject_Click(object sender, EventArgs e)
        {

        }

        protected void cblSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cblPriceSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
            BindRegion();
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
                BindSubjectNameList();
            }
        }

        protected void btnCheckAllGuidance_Click(object sender, EventArgs e)
        {
            GuidanceSelectChange();
        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            GuidanceSelectChange();
        }

        protected void cbShowSubjectNameList_CheckedChanged(object sender, EventArgs e)
        {
            BindSubjectNameList();
        }

        void GuidanceSelectChange()
        {
            LoadSessionData();
            BindShopType();
            BindAddUser();
            BindSubjectCategory();
            BindProvince();
            BindSubjectNameList();
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

        

        /// <summary>
        /// 统计方法
        /// </summary>
        /// 
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

        Dictionary<int, decimal> subjectInstallPriceDic = new Dictionary<int, decimal>();
        //二次安装费字典：<项目Id，费用合计>
        Dictionary<int, decimal> secondInstallPriceDic = new Dictionary<int, decimal>();
        //二次发货费字典：<项目Id，费用合计>
        Dictionary<int, decimal> secondExpressPriceDic = new Dictionary<int, decimal>();

        //项目快递费字典：<项目Id，费用合计>
        Dictionary<int, decimal> expressPriceDic = new Dictionary<int, decimal>();
        //物料（道具）费用
        Dictionary<int, decimal> materialPriceDic = new Dictionary<int, decimal>();
        //新开店安装费
        Dictionary<int, decimal> newShopInstallPriceDic = new Dictionary<int, decimal>();
        //运费
        Dictionary<int, decimal> freightDic = new Dictionary<int, decimal>();
        //分区活动安装费
        Dictionary<int, decimal> regionInstallPriceDic = new Dictionary<int, decimal>();
        //分区活动发货费
        Dictionary<int, decimal> regionExpressPriceDic = new Dictionary<int, decimal>();
        //分区新开店测量费
        Dictionary<int, decimal> measurePriceDic = new Dictionary<int, decimal>();
        //分区其他费
        Dictionary<int, decimal> regionOtherPriceDic = new Dictionary<int, decimal>();
        void StatisticData()
        {
            
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

            if (Session["orderDetailStatisticsQuote"] == null)
            {
                LoadSessionData();
            }
            List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            List<SubjectGuidance> guidanceList = new List<SubjectGuidance>();
            if (Session["orderDetailStatisticsQuote"] != null)
                finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
            if (Session["shopStatisticsQuote"] != null)
                shopList = Session["shopStatisticsQuote"] as List<Shop>;
            if (Session["subjectStatisticsQuote"] != null)
                subjectList = Session["subjectStatisticsQuote"] as List<Subject>;
            if (Session["guidanceStatisticsQuote"] != null)
                guidanceList = Session["guidanceStatisticsQuote"] as List<SubjectGuidance>;

            #region
            var orderList = (from order in finalOrderDetailTempList
                             join shop in shopList
                             on order.ShopId equals shop.Id
                             join subject in subjectList
                             on order.SubjectId equals subject.Id
                             join guidance in guidanceList
                             on order.GuidanceId equals guidance.ItemId
                             where (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower())) : 1 == 1)

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
            decimal originalPOPPrice = 0;//原始订单POP费用
            decimal autoAddPOPPrice = 0;//系统报价自动增加POP费用
            //decimal POPPriceAfterAdd = 0;//总POP费用
            decimal newShopInstallPrice = 0;//新开店安装费
            decimal freightPrice = 0;//运费
            decimal otherPrice = 0;//其他费用
            decimal measurePrice = 0;//新开店测量费
            decimal regionInstallPrice = 0;//增补/新开店安装费
            decimal regionOtherPrice = 0;//分区活动其他装费
            decimal regionExpressPrice = 0;//分区活动快递/发货装费

            decimal shutShopExpressPrice = 0;//闭店促销快递费

            //正常店铺订单
            //var normalOrder=orderList0.Where(s => (s.shop.Status == null || s.shop.Status == "" || s.shop.Status == "正常")).ToList();
            //闭店订单
            var shutShopOrder = orderList.Where(s => (s.order.ShopStatus != null && s.order.ShopStatus.Contains("闭"))).ToList();



            List<int> shopIdList = new List<int>();
            if (orderList.Any())
            {

                //统计实际订单费用和调整后的费用（POP）
                originalPOPPrice = orderList.Sum(s => s.order.DefaultTotalPrice ?? 0);
                popPrice = orderList.Sum(s => s.order.TotalPrice ?? 0);
                autoAddPOPPrice = popPrice - originalPOPPrice;
                area = orderList.Sum(s => (s.order.TotalArea ?? 0)*(s.order.Quantity??1));
                shopIdList = orderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                labSubjectCount.Text = (subjectIdList.Count).ToString();
                //StatisticQuotePOPTotalPrice(orderList.Select(s => s.order), out popPrice, out area);
                labArea.Text = area > 0 ? (area + "平方米") : "0";
                if (popPrice > 0)
                {
                    string popPriceStr = string.Format("（原始订单费用：{0}  自动增加费用：{1}）",Math.Round(originalPOPPrice, 2), Math.Round(autoAddPOPPrice, 2));
                    labPOPPrice.Text = Math.Round(popPrice, 2) + "元";
                    labPOPPriceDetail.Text = popPriceStr;
                    labPOPPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                    labPOPPrice.Attributes.Add("name", "checkMaterial");
                }


            }

            //快递费用（促销）
            //var saleGuidanceList=new SubjectGuidanceBLL().GetList(s=>)
            guidanceIdList.ForEach(gid =>
            {
                var freightOrderShopList = orderList.Where(s => s.order.GuidanceId == gid && (((s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Promotion && (s.guidance.HasExperssFees ?? false) == true) || ((s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Delivery))).ToList();
                if (freightOrderShopList.Any())
                {
                   
                    List<int> expressShopIdList = freightOrderShopList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                    var expressPriceList = new ExpressPriceDetailBLL().GetList(s => s.GuidanceId == gid && expressShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    if (expressPriceList.Any())
                    {
                        expressPrice += expressPriceList.Sum(s => s.ExpressPrice ?? 0);
                    }


                }
            });




            #region 正常安装费
            guidanceIdList.ForEach(gid =>
            {
                List<int> installShopIdList = orderList.Where(s => s.order.GuidanceId == gid && s.order.OrderType != (int)OrderTypeEnum.物料 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单).Select(s => s.order.ShopId ?? 0).Distinct().ToList();
              
                var installPriceDetailList = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                              
                                              join subject in CurrentContext.DbContext.Subject
                                              on installShop.SubjectId equals subject.Id
                                              join guidance in CurrentContext.DbContext.SubjectGuidance
                                              on installShop.GuidanceId equals guidance.ItemId
                                              where
                                              installShop.GuidanceId == gid
                                              && installShopIdList.Contains(installShop.ShopId ?? 0)
                                              && (guidance.ActivityTypeId != (int)GuidanceTypeEnum.Others)//不统计分区增补的安装费
                                              && (installShop.BasicPrice ?? 0) > 0
                                              select new
                                              {
                                                  installShop
                                                  // shop,
                                                  // subject
                                              }).ToList();

                if (installPriceDetailList.Any())
                {




                    decimal installPrice0 = 0;
                    installPriceDetailList.ForEach(s =>
                    {
                        installPrice0 = (s.installShop.BasicPrice ?? 0) + (s.installShop.WindowPrice ?? 0) + (s.installShop.OOHPrice ?? 0);
                        if (!subjectInstallPriceDic.Keys.Contains(s.installShop.SubjectId ?? 0))
                        {
                            subjectInstallPriceDic.Add(s.installShop.SubjectId ?? 0, installPrice0);
                        }
                        else
                        {
                            subjectInstallPriceDic[s.installShop.SubjectId ?? 0] = subjectInstallPriceDic[s.installShop.SubjectId ?? 0] + installPrice0;
                        }
                    });
                }
            });


            var orderInstallList = orderList.Where(s => (s.guidance.ActivityTypeId ?? 1) != (int)GuidanceTypeEnum.Others && s.subject.SubjectType != (int)SubjectTypeEnum.二次安装 && s.subject.SubjectType != (int)SubjectTypeEnum.费用订单 && s.order.OrderType > (int)OrderTypeEnum.道具).ToList();
            if (orderInstallList.Any())
            {
                subjectIdList.ForEach(s =>
                {
                    decimal installPrice0 = orderInstallList.Where(r => r.order.OrderType == (int)OrderTypeEnum.安装费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (!subjectInstallPriceDic.Keys.Contains(s))
                    {
                        subjectInstallPriceDic.Add(s, installPrice0);
                    }
                    else
                    {
                        subjectInstallPriceDic[s] = subjectInstallPriceDic[s] + installPrice0;
                    }

                    decimal otherPrice0 = orderInstallList.Where(r => r.order.OrderType == (int)OrderTypeEnum.其他费用 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    freightPrice += otherPrice0;
                    if (!freightDic.Keys.Contains(s))
                    {
                        freightDic.Add(s, otherPrice0);
                    }
                    else
                    {
                        freightDic[s] += otherPrice0;
                    }

                    decimal regionExpressPrice0 = orderInstallList.Where(r => r.order.OrderType == (int)OrderTypeEnum.发货费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);

                    expressPrice += regionExpressPrice0;

                    decimal measurePrice0 = orderInstallList.Where(r => r.order.OrderType == (int)OrderTypeEnum.测量费 && r.subject.Id == s).Sum(r => r.order.OrderPrice ?? 0);
                    if (measurePriceDic.Keys.Contains(s))
                    {
                        measurePriceDic[s] = measurePriceDic[s] + measurePrice0;
                    }
                    else
                    {
                        measurePriceDic.Add(s, measurePrice0);
                    }
                    measurePrice += measurePrice0;

                });
            }

            #endregion

            #region 物料费
            
            var materialList = orderList.Where(s => s.order.OrderType == (int)OrderTypeEnum.物料).ToList();
            

            if (materialList.Any())
            {
               
                materialList.ForEach(s =>
                {
                    int num = s.order.Quantity ?? 0;
                    decimal price = s.order.UnitPrice ?? 0;
                    decimal subPrice = num * price;
                    if (materialPriceDic.Keys.Contains(s.order.SubjectId ?? 0))
                    {
                        materialPriceDic[s.order.SubjectId ?? 0] += subPrice;
                    }
                    else
                        materialPriceDic.Add(s.order.SubjectId ?? 0, subPrice);
                });
            }
            #endregion
            #region 二次安装费
            var secondInstallList = orderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.二次安装 && (s.order.OrderType ?? 1) == (int)OrderTypeEnum.安装费);
            if (secondInstallList.Any())
            {
                secondInstallList.ToList().ForEach(s =>
                {
                    if (!secondInstallPriceDic.Keys.Contains((s.subject.Id)))
                    {
                        secondInstallPriceDic.Add(s.subject.Id, (s.order.OrderPrice ?? 0));
                    }
                    else
                    {
                        secondInstallPriceDic[s.subject.Id] += (s.order.OrderPrice ?? 0);
                    }
                    secondInstallPrice += (s.order.OrderPrice ?? 0);
                });
            }
            #endregion
            #region 二次发货费
            var secondExpressList = orderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.二次安装 && (s.order.OrderType ?? 1) == (int)OrderTypeEnum.发货费);
            if (secondExpressList.Any())
            {
                secondExpressList.ToList().ForEach(s =>
                {
                    if (!secondExpressPriceDic.Keys.Contains((s.subject.Id)))
                    {
                        secondExpressPriceDic.Add(s.subject.Id, (s.order.OrderPrice ?? 0));
                    }
                    else
                    {
                        secondExpressPriceDic[s.subject.Id] += (s.order.OrderPrice ?? 0);
                    }
                    secondExperssPrice += (s.order.OrderPrice ?? 0);
                });
            }
            Session["secondInstallPriceDicStatisticsQuote"] = secondInstallPriceDic;
            //Session["secondExperssDicStatistics"] = secondExperssPrice;
            #endregion
            #region 统计其他费用（新开安装费）
            var orderDetailList = new PriceOrderDetailBLL().GetList(s => priceSubjectIdList.Contains(s.SubjectId ?? 0) && (s.SubjectType ?? 1) == (int)SubjectTypeEnum.新开店安装费);
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
                    if (!newShopInstallPriceDic.Keys.Contains((s.SubjectId ?? 0)))
                    {
                        newShopInstallPriceDic.Add((s.SubjectId ?? 0), (s.Amount ?? 0));
                    }
                    else
                    {
                        newShopInstallPriceDic[(s.SubjectId ?? 0)] += (s.Amount ?? 0);
                    }
                    newShopInstallPrice += (s.Amount ?? 0);
                });
            }
            #endregion
            #region 运费
            if (priceSubjectIdList.Any())
            {

                var freightOrderList = new PriceOrderDetailBLL().GetList(s => priceSubjectIdList.Contains(s.SubjectId ?? 0) && (s.SubjectType ?? 1) == (int)SubjectTypeEnum.运费);
                if (regionList.Any())
                {
                    freightOrderList = freightOrderList.Where(s => regionList.Contains(s.Region.ToLower())).ToList();

                }
                if (freightOrderList.Any())
                {
                    freightOrderList.ForEach(s =>
                    {
                        if (!freightDic.Keys.Contains((s.SubjectId ?? 0)))
                        {
                            freightDic.Add((s.SubjectId ?? 0), (s.Amount ?? 0));
                        }
                        else
                        {
                            freightDic[(s.SubjectId ?? 0)] += (s.Amount ?? 0);
                        }
                        freightPrice += (s.Amount ?? 0);
                    });
                }

            }
            #endregion
            #region 分区活动费（安装费，测量费，其他费用）
            var regionPriceOrderList = orderList.Where(s => (s.guidance.ActivityTypeId ?? 1) == (int)GuidanceTypeEnum.Others && (s.order.OrderType ?? 1) > 1).ToList();
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
                    if (measurePriceDic.Keys.Contains(s))
                    {
                        measurePriceDic[s] = measurePriceDic[s] + measurePrice0;
                    }
                    else
                    {
                        measurePriceDic.Add(s, measurePrice0);
                    }
                    measurePrice += measurePrice0;

                });


            }

            #endregion
            #region 费用订单
            var priceOrderList = orderList.Where(s => s.subject.SubjectType == (int)SubjectTypeEnum.费用订单).ToList();
            if (priceOrderList.Any())
            {
                priceOrderList.ForEach(s =>
                {
                    decimal price0 = s.order.OrderPrice ?? 0;
                    if (s.order.OrderType == (int)OrderTypeEnum.安装费)
                    {

                        if (!subjectInstallPriceDic.Keys.Contains(s.order.SubjectId ?? 0))
                        {
                            subjectInstallPriceDic.Add(s.order.SubjectId ?? 0, price0);
                        }
                        else
                        {
                            subjectInstallPriceDic[s.order.SubjectId ?? 0] = subjectInstallPriceDic[s.order.SubjectId ?? 0] + price0;
                        }
                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.发货费)
                    {
                        expressPrice += price0;

                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.其他费用)
                    {
                        freightPrice += price0;
                        if (!freightDic.Keys.Contains((s.order.SubjectId ?? 0)))
                        {
                            freightDic.Add((s.order.SubjectId ?? 0), price0);
                        }
                        else
                        {
                            freightDic[(s.order.SubjectId ?? 0)] += price0;
                        }
                    }
                    else if (s.order.OrderType == (int)OrderTypeEnum.运费)
                    {
                        freightPrice += price0;
                        if (!freightDic.Keys.Contains((s.order.SubjectId ?? 0)))
                        {
                            freightDic.Add((s.order.SubjectId ?? 0), price0);
                        }
                        else
                        {
                            freightDic[(s.order.SubjectId ?? 0)] += price0;
                        }

                    }
                });
            }
            Session["freightDicStatisticsQuote"] = freightDic;
            #endregion
            #endregion

            if (subjectInstallPriceDic.Keys.Count > 0)
            {

                installPrice += (subjectInstallPriceDic.Sum(s => s.Value));

            }
            Session["installPriceDicStatisticsQuote"] = subjectInstallPriceDic;
            if (expressPriceDic.Keys.Count > 0)
            {
                if (subjectIdList.Any())
                {
                    subjectIdList.ForEach(s =>
                    {
                        if (expressPriceDic.Keys.Contains(s))
                            expressPrice += expressPriceDic[s];
                    });
                }
                else
                {
                    foreach (KeyValuePair<int, decimal> item in expressPriceDic)
                    {
                        expressPrice += item.Value;
                    }
                }
            }
            Session["expressPriceDicStatisticsQuote"] = expressPriceDic;
            if (shopIdList.Any())
            {
                labShopCount.Text = shopIdList.Distinct().Count().ToString();
                labShopCount.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labShopCount.Attributes.Add("name", "checkShop");
            }
            if (expressPrice > 0)
            {
                labExpressPrice.Text = Math.Round(expressPrice, 2) + "元";
            }
            if (secondExperssPrice > 0)
            {
                labSecondExpressPrice.Text = Math.Round(secondExperssPrice, 2) + "元";
            }
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
            if (materialPriceDic.Keys.Count > 0)
            {
                if (subjectIdList.Any())
                {
                    subjectIdList.ForEach(s =>
                    {
                        if (materialPriceDic.Keys.Contains(s))
                            materialPrice += materialPriceDic[s];
                    });
                }
                else
                {
                    foreach (KeyValuePair<int, decimal> item in materialPriceDic)
                    {
                        materialPrice += item.Value;
                    }
                }
            }
            Session["materialPriceDicStatisticsQuote"] = materialPriceDic;
            if (materialPrice > 0)
            {
                labMaterialPrice.Text = Math.Round(materialPrice, 2) + "元";
                labMaterialPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labMaterialPrice.Attributes.Add("name", "checkMaterialOrderPrice");
            }

            if (newShopInstallPrice > 0)
            {
                labNewShopInstallPrice.Text = Math.Round(newShopInstallPrice, 2) + "元";

            }
            if (freightPrice > 0)
            {
                labFreight.Text = Math.Round(freightPrice, 2) + "元";

            }

            if (regionInstallPrice > 0)
            {
                labRegionInstallPrice.Text = Math.Round(regionInstallPrice, 2) + "元";

            }
            if (regionExpressPrice > 0)
            {
                labRegionExpressPrice.Text = Math.Round(regionExpressPrice, 2) + "元";

            }
            if (measurePrice > 0)
            {
                labMeasurePrice.Text = Math.Round(measurePrice, 2) + "元";

            }
            if (regionOtherPrice > 0)
            {
                labRegionOtherPrice.Text = Math.Round(regionOtherPrice, 2) + "元";

            }


            decimal total = popPrice + installPrice + expressPrice + secondInstallPrice + materialPrice + newShopInstallPrice + freightPrice + measurePrice + regionInstallPrice + regionOtherPrice + regionExpressPrice + secondExperssPrice;
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

                StatisticQuotePOPTotalPrice(shutShopOrder.Select(s => s.order), out shutShopPopPrice, out shutShopArea);
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
                if (materialList.Any())
                {
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

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            InitStatisticsValue();
            StatisticData();
            ShowSubjectList();
        }

        protected void cbShopSubjectList_CheckedChanged(object sender, EventArgs e)
        {
            ShowSubjectList();
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

        void BindSubjectList()
        {
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
            List<QuoteOrderDetail> finalOrderDetailTempList = new List<QuoteOrderDetail>();
            List<Shop> shopList = new List<Shop>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailStatisticsQuote"] != null)
                finalOrderDetailTempList = Session["orderDetailStatisticsQuote"] as List<QuoteOrderDetail>;
            if (Session["shopStatisticsQuote"] != null)
                shopList = Session["shopStatisticsQuote"] as List<Shop>;
            if (Session["subjectStatisticsQuote"] != null)
                subjectList = Session["subjectStatisticsQuote"] as List<Subject>;

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
          
            List<int> subjectIds = orderList0.Select(s => s.subject.Id).Distinct().ToList();
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
                        where subjectIds.Contains(subject.Id)
                        select new
                        {
                            subject,
                            subject.Id,
                            subject.SubjectType,
                            customer.CustomerName,
                            subject.SubjectName,
                            subject.Remark,
                            subject.PriceBlongRegion
                        }
                            ).ToList();
            gvList.DataSource = list;
            gvList.DataBind();
        }

        
        protected void gvList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        
    }
}
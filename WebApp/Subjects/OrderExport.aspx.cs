﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using DAL;

namespace WebApp.Subjects
{
    public partial class OrderExport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                BindGuidance();
            }
        }

        void BindGuidance(int? onDateSearch = null)
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);

            // var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId).ToList();
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        join type in CurrentContext.DbContext.ADOrderActivity
                        on guidance.ActivityTypeId equals type.ActivityId
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance,
                            type
                        }).Distinct().ToList();


            if (onDateSearch != null)
            {
                string begin = labBeginDate.Text;
                string end = labEndDate.Text;
                if (!string.IsNullOrWhiteSpace(begin) && !string.IsNullOrWhiteSpace(end))
                {
                    DateTime beginDate = DateTime.Parse(begin);
                    DateTime endDate = DateTime.Parse(end).AddDays(1);

                    list = list.Where(s => s.guidance.BeginDate >= beginDate && s.guidance.EndDate < endDate).ToList();
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

                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + s.type.ActivityName + "&nbsp;&nbsp;";
                    cblGuidanceList.Items.Add(li);
                });
            }
            labEmptyGuidance.Visible = !list.Any();
        }


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
            }
            else
            {
                labBeginDate.Text = "";
                labEndDate.Text = "";
                labSeparator.Visible = false;
            }
        }

        void BindSubjectInfo()
        {
            cblSubjectType.Items.Clear();
            cblActivity.Items.Clear();
            cblSubjects.Items.Clear();
            cblRegion.Items.Clear();
            List<int> guidanceIdList = GetGuidanceSelected();
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join subjectType1 in CurrentContext.DbContext.SubjectType
                             on subject.SubjectTypeId equals subjectType1.Id into subjectTypeTemp
                             from subjectType in subjectTypeTemp.DefaultIfEmpty()
                             join category1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals category1.Id into categoryTemp
                             from category in categoryTemp.DefaultIfEmpty()
                             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 subject,
                                 subjectType,
                                 category,
                                 shop
                             }).ToList();
            if (orderList.Any())
            {
                var typeList = orderList.Select(s => s.subjectType).Distinct().ToList();
                bool isTypeNull = false;
                typeList.ForEach(s =>
                {
                    if (s != null)
                    {
                        ListItem li = new ListItem();
                        li.Text = s.SubjectTypeName + "&nbsp;&nbsp;";
                        li.Value = s.Id.ToString();
                        //if (selectedList.Contains(s.Id))
                        //    li.Selected = true;
                        cblSubjectType.Items.Add(li);
                    }
                    else
                        isTypeNull = true;
                });
                if (isTypeNull)
                {
                    cblSubjectType.Items.Add(new ListItem("空", "0"));
                }



                var categoryList = orderList.Select(s => s.category).Distinct().ToList();
                List<int> selectedList = new List<int>();
                if (Session["subjectCategorySelected"] != null)
                {
                    selectedList = Session["subjectCategorySelected"] as List<int>;
                }
                bool isCategoryNull = false;
                categoryList.ForEach(s =>
                {
                    if (s != null)
                    {
                        ListItem li = new ListItem();
                        li.Text = s.CategoryName + "&nbsp;&nbsp;";
                        li.Value = s.Id.ToString();
                        if (selectedList.Contains(s.Id))
                            li.Selected = true;
                        cblActivity.Items.Add(li);
                    }
                    else
                        isCategoryNull = true;
                });
                if (isCategoryNull)
                {
                    cblActivity.Items.Add(new ListItem("空", "0"));
                }

                

                List<string> regionList = orderList.Select(s => s.shop.RegionName).Distinct().ToList();
                var subjectBlongRegionList = orderList.Select(s => s.subject.PriceBlongRegion).Distinct().ToList();
                if (subjectBlongRegionList.Any())
                {
                    subjectBlongRegionList.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s) && !regionList.Contains(s))
                        {
                            regionList.Add(s);
                        }
                    });
                }
                if (regionList.Any())
                {
                    bool isRegionNull = false;
                    regionList.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            ListItem li = new ListItem();
                            li.Text = s + "&nbsp;&nbsp;";
                            li.Value = s;
                            cblRegion.Items.Add(li);
                        }
                        else
                            isRegionNull = true;
                    });
                    if (isRegionNull)
                    {
                        cblRegion.Items.Add(new ListItem("空", "空"));
                    }
                }
                var subjectList = orderList.Select(s => s.subject).Distinct().ToList();
                subjectList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s.SubjectName + "&nbsp;&nbsp;";
                    li.Value = s.Id.ToString();
                    cblSubjects.Items.Add(li);
                });
                PanelSubject.Visible = subjectList.Any();
            }
            else
                PanelSubject.Visible = false;
        }

        void BindSubjectActivity()
        {
            cblActivity.Items.Clear();
            List<int> guidanceIdList = GetGuidanceSelected();

            List<int> subjectTypeList = new List<int>();
            foreach (ListItem li in cblSubjectType.Items)
            {
                if (li.Selected)
                {
                    subjectTypeList.Add(int.Parse(li.Value));
                }
            }

            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join category1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals category1.Id into categoryTemp
                             from category in categoryTemp.DefaultIfEmpty()
                             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 subject,
                                 category
                             }).ToList();
            if (subjectTypeList.Any())
            {
                if (subjectTypeList.Contains(0))
                {
                    subjectTypeList.Remove(0);
                    if (subjectTypeList.Any())
                    {
                        orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0) || (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0)).ToList();
                    }
                    else
                    {
                        orderList = orderList.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0)).ToList();
                    }
                }
                else
                    orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
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
                        cblActivity.Items.Add(li);
                    }
                    else
                        isNull = true;
                });
                if (isNull)
                {
                    cblActivity.Items.Add(new ListItem("空", "0"));
                }
            }
        }

        void BindRegion()
        {
            cblRegion.Items.Clear();
            List<int> guidanceIdList = GetGuidanceSelected();

            List<int> subjectTypeList = new List<int>();
            foreach (ListItem li in cblSubjectType.Items)
            {
                if (li.Selected)
                {
                    subjectTypeList.Add(int.Parse(li.Value));
                }
            }
            List<int> subjectCategoryList = new List<int>();
            foreach (ListItem li in cblActivity.Items)
            {
                if (li.Selected)
                {
                    subjectCategoryList.Add(int.Parse(li.Value));
                }
            }
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 subject,
                                 shop
                             }).ToList();
            if (subjectTypeList.Any())
            {
                if (subjectTypeList.Contains(0))
                {
                    subjectTypeList.Remove(0);
                    if (subjectTypeList.Any())
                    {
                        orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0) || (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0)).ToList();
                    }
                    else
                    {
                        orderList = orderList.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0)).ToList();
                    }
                }
                else
                    orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
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
                    {
                        orderList = orderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                }
                else
                    orderList = orderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
            }
            if (orderList.Any())
            {
                List<string> regionList = orderList.Select(s => s.shop.RegionName).Distinct().ToList();
                var subjectBlongRegionList = orderList.Select(s => s.subject.PriceBlongRegion).Distinct().ToList();
                if (subjectBlongRegionList.Any())
                {
                    subjectBlongRegionList.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s) && !regionList.Contains(s))
                        {
                            regionList.Add(s);
                        }
                    });
                }
                if (regionList.Any())
                {
                    bool isRegionNull = false;
                    regionList.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            ListItem li = new ListItem();
                            li.Text = s + "&nbsp;&nbsp;";
                            li.Value = s;
                            cblRegion.Items.Add(li);
                        }
                        else
                            isRegionNull = true;
                    });
                    if (isRegionNull)
                    {
                        cblRegion.Items.Add(new ListItem("空", "空"));
                    }
                }
            }
        }

        void BindSubject()
        {
            cblSubjects.Items.Clear();
            cbSubjectAll.Checked = false;
            List<int> guidanceIdList = GetGuidanceSelected();

            List<int> subjectTypeList = new List<int>();
            foreach (ListItem li in cblSubjectType.Items)
            {
                if (li.Selected)
                {
                    subjectTypeList.Add(int.Parse(li.Value));
                }
            }
            List<int> subjectCategoryList = new List<int>();
            foreach (ListItem li in cblActivity.Items)
            {
                if (li.Selected)
                {
                    subjectCategoryList.Add(int.Parse(li.Value));
                }
            }
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value.ToLower());
                }
            }
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 subject,
                                 shop
                             }).ToList();
            if (subjectTypeList.Any())
            {
                if (subjectTypeList.Contains(0))
                {
                    subjectTypeList.Remove(0);
                    if (subjectTypeList.Any())
                    {
                        orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0) || (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0)).ToList();
                    }
                    else
                    {
                        orderList = orderList.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0)).ToList();
                    }
                }
                else
                    orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
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
                    {
                        orderList = orderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                }
                else
                    orderList = orderList.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
            }
            if (regionList.Any())
            {
                if (regionList.Contains("空"))
                {
                    regionList.Remove("空");
                    if (regionList.Any())
                    {
                        orderList = orderList.Where(s => ((s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower())) || (s.shop.RegionName == null || s.shop.RegionName == "")).ToList();
                    }
                    else
                    {
                        orderList = orderList.Where(s => (s.shop.RegionName == null || s.shop.RegionName == "")).ToList();
                    }
                }
                else
                    orderList = orderList.Where(s => ((s.subject.PriceBlongRegion != null && s.subject.PriceBlongRegion != "") ? regionList.Contains(s.subject.PriceBlongRegion.ToLower()) : regionList.Contains(s.shop.RegionName.ToLower()))).ToList();
            }
            if (orderList.Any())
            {
                var subjectList = orderList.Select(s => s.subject).Distinct().ToList();
                subjectList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s.SubjectName + "&nbsp;&nbsp;";
                    li.Value = s.Id.ToString();
                    cblSubjects.Items.Add(li);
                });
                PanelSubject.Visible = subjectList.Any();
            }
            else
                PanelSubject.Visible = false;
        }

        void BindProvince()
        {
            cblProvince.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value.ToLower());
                }
            }
            if (regionList.Any())
            {
                List<int> subjectIdList = new List<int>();
                foreach (ListItem li in cblSubjects.Items)
                {
                    if (li.Selected)
                    {
                        subjectIdList.Add(int.Parse(li.Value));
                    }
                }
                if (subjectIdList.Any())
                {
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     where subjectIdList.Contains(order.SubjectId??0)
                                     && ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(shop.RegionName.ToLower()))
                                     select shop).ToList();
                    var provinceList = orderList.OrderBy(s => s.RegionName).Select(s => s.ProvinceName).Distinct().ToList();
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
                        PanelProvince.Visible = true;
                        if (isSelected)
                        {
                            //BindCity();
                        }
                    }
                    else
                        PanelProvince.Visible = false;
                }
                else
                    PanelProvince.Visible = false;
            }
            else
                PanelProvince.Visible = false;
        }

        void BindCity()
        {
            cblCity.Items.Clear();
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                {
                    provinceList.Add(li.Value);
                }
            }
            if (provinceList.Any())
            {
                List<int> subjectIdList = new List<int>();
                foreach (ListItem li in cblSubjects.Items)
                {
                    if (li.Selected)
                    {
                        subjectIdList.Add(int.Parse(li.Value));
                    }
                }
                if (subjectIdList.Any())
                {
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     where subjectIdList.Contains(order.SubjectId ?? 0)
                                     && provinceList.Contains(shop.ProvinceName)
                                     select shop).ToList();
                    var cityList = orderList.OrderBy(s => s.ProvinceName).Select(s => s.CityName).Distinct().ToList();
                    if (cityList.Any())
                    {
                        List<string> selectedList = new List<string>();
                        //if (Session["provinceSelected"] != null)
                        //{
                        //    selectedList = Session["provinceSelected"] as List<string>;
                        //}
                        bool isSelected = false;
                        cityList.ForEach(s =>
                        {
                            ListItem li = new ListItem();
                            li.Text = s + "&nbsp;";
                            li.Value = s;
                            if (selectedList.Contains(s))
                            {
                                li.Selected = true;
                                isSelected = true;
                            }
                            cblCity.Items.Add(li);
                        });
                        PanelCity.Visible = true;
                       
                    }
                    else
                        PanelCity.Visible = false;
                }
                else
                    PanelCity.Visible = false;
            }
            else
                PanelCity.Visible = false;
        }

        void BindCustomerServiceName()
        {
            cblCustomerService.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value.ToLower());
                }
            }
            if (regionList.Any())
            {
                List<int> subjectIdList = new List<int>();
                foreach (ListItem li in cblSubjects.Items)
                {
                    if (li.Selected)
                    {
                        subjectIdList.Add(int.Parse(li.Value));
                    }
                }
                List<string> provinceList = new List<string>();
                foreach (ListItem li in cblProvince.Items)
                {
                    if (li.Selected)
                    {
                        provinceList.Add(li.Value);
                    }
                }
                if (subjectIdList.Any())
                {
                    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     join user1 in CurrentContext.DbContext.UserInfo
                                     on shop.CSUserId equals user1.UserId into userTemp
                                     from user in userTemp.DefaultIfEmpty()
                                     where subjectIdList.Contains(order.SubjectId ?? 0)
                                     && ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(shop.RegionName.ToLower()))
                                     select new { shop, CSName = user.RealName }).ToList();
                    if (provinceList.Any())
                    {
                        orderList = orderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                    }
                    if (orderList.Any())
                    {

                        var list1 = orderList.Select(s => new { s.shop.CSUserId, s.CSName }).Distinct().ToList();
                        bool isEmpty = false;
                        List<int> userIdList = new List<int>();
                        list1.ForEach(s =>
                        {
                            if (s.CSUserId != null && s.CSUserId > 0)
                            {
                                if (!userIdList.Contains(s.CSUserId ?? 0))
                                {
                                    userIdList.Add(s.CSUserId ?? 0);
                                    ListItem li = new ListItem();
                                    li.Text = s.CSName + "&nbsp;";
                                    li.Value = (s.CSUserId ?? 0).ToString();
                                    cblCustomerService.Items.Add(li);
                                }

                            }
                            else
                                isEmpty = true;
                        });
                        if (isEmpty)
                        {
                            cblCustomerService.Items.Add(new ListItem("空","0"));
                        }
                    }
                }
               
            }
           
        }

        void BindIsInstall()
        {
            cblIsInstall.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value.ToLower());
                }
            }
            List<int> subjectIdList = new List<int>();
            foreach (ListItem li in cblSubjects.Items)
            {
                if (li.Selected)
                {
                    subjectIdList.Add(int.Parse(li.Value));
                }
            }
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                {
                    provinceList.Add(li.Value);
                }
            }
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                {
                    cityList.Add(li.Value);
                }
            }
            List<int> customerServiceList = new List<int>();
            foreach (ListItem li in cblCustomerService.Items)
            {
                if (li.Selected)
                {
                    customerServiceList.Add(int.Parse(li.Value));
                }
            }
            if (subjectIdList.Any())
            {
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 where subjectIdList.Contains(order.SubjectId ?? 0)
                                 && (regionList.Any()? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(shop.RegionName.ToLower())):1==1)
                                 select shop).ToList();
                if (provinceList.Any())
                {
                    orderList = orderList.Where(s => provinceList.Contains(s.ProvinceName)).ToList();
                }
                if (cityList.Any())
                {
                    orderList = orderList.Where(s => cityList.Contains(s.CityName)).ToList();
                }
                if (customerServiceList.Any())
                {
                    if (customerServiceList.Contains(0))
                    {
                        customerServiceList.Remove(0);
                        if (customerServiceList.Any())
                        {
                            orderList = orderList.Where(s => customerServiceList.Contains(s.CSUserId ?? 0) || (s.CSUserId == null || s.CSUserId == 0)).ToList();
                            
                        }
                        else
                        {
                            orderList = orderList.Where(s => (s.CSUserId == null || s.CSUserId == 0)).ToList();
                            
                        }
                    }
                    else
                    {
                        orderList = orderList.Where(s => customerServiceList.Contains(s.CSUserId ?? 0)).ToList();
                       
                    }
                }
                if (orderList.Any())
                {
                    var list1 = orderList.Select(s => s.IsInstall).Distinct().ToList();
                    List<string> installList = new List<string>();
                    bool isEmpty = false;
                    list1.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            if (!installList.Contains(s))
                            {
                                installList.Add(s);
                                ListItem li = new ListItem();
                                li.Text = s + "&nbsp;";
                                li.Value = s;
                                cblIsInstall.Items.Add(li);
                            }
                        }
                        else
                            isEmpty = true;
                    });
                    if (isEmpty)
                    {
                        cblIsInstall.Items.Add(new ListItem("空", "空"));
                    }
                }
            }
        }

        void BindShopCount()
        { 
           List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value.ToLower());
                }
            }
            List<int> subjectIdList = new List<int>();
            foreach (ListItem li in cblSubjects.Items)
            {
                if (li.Selected)
                {
                    subjectIdList.Add(int.Parse(li.Value));
                }
            }
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                {
                    provinceList.Add(li.Value);
                }
            }
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                {
                    cityList.Add(li.Value);
                }
            }
            List<int> customerServiceList = new List<int>();
            foreach (ListItem li in cblCustomerService.Items)
            {
                if (li.Selected)
                {
                    customerServiceList.Add(int.Parse(li.Value));
                }
            }
            List<string> installList = new List<string>();
            foreach (ListItem li in cblIsInstall.Items)
            {
                if (li.Selected)
                {
                    installList.Add(li.Value);
                }
            }
            if (subjectIdList.Any())
            {
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 where subjectIdList.Contains(order.SubjectId ?? 0)
                                 && (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(shop.RegionName.ToLower())) : 1 == 1)
                                 select new {shop,order }).ToList();
                if (provinceList.Any())
                {
                    orderList = orderList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }
                if (cityList.Any())
                {
                    orderList = orderList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
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
                if (installList.Any())
                {
                    
                    if (installList.Contains("无"))
                    {
                        installList.Remove("无");
                        if (installList.Any())
                        {
                            orderList = orderList.Where(s => installList.Contains(s.shop.IsInstall) || (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.shop.IsInstall == null || s.shop.IsInstall == "")).ToList();
                    }
                    else
                        orderList = orderList.Where(s => installList.Contains(s.shop.IsInstall)).ToList();
                }
                if (orderList.Any())
                {
                    int totalShopCount = orderList.Select(s => s.shop).Distinct().Count();
                    int totalOrderCount = orderList.Count();
                    var shutOrderList = orderList.Where(s => s.order.ShopStatus != null && (s.order.ShopStatus.Contains("闭") || s.order.ShopStatus.Contains("装修")));
                    int shutShopCount = shutOrderList.Select(s => s.shop).Distinct().Count();
                    int shutOrderCount = shutOrderList.Count();
                    labShopCount.Text = totalShopCount.ToString();
                    labOrderCount.Text = totalOrderCount.ToString();
                    labShutShopCout.Text = shutShopCount.ToString();
                    labShutShopOrderCout.Text = shutOrderCount.ToString();
                }
                else
                {
                    labShopCount.Text = "0";
                    labOrderCount.Text = "0";
                    labShutShopCout.Text = "0";
                    labShutShopOrderCout.Text = "0";
                }
            }
            else
            {
                labShopCount.Text = "0";
                labOrderCount.Text = "0";
                labShutShopCout.Text = "0";
                labShutShopOrderCout.Text = "0";
            }
        }

        void ClearInfo()
        {
            cblSubjectType.Items.Clear();
            cblActivity.Items.Clear();
            cblRegion.Items.Clear();
            cblSubjects.Items.Clear();
            PanelSubject.Visible = false;
            cblProvince.Items.Clear();
            PanelProvince.Visible = false;
            cblCity.Items.Clear();
            PanelCity.Visible = false;
            cblCustomerService.Items.Clear();
            cblIsInstall.Items.Clear();
            labShopCount.Text = "0";
            labOrderCount.Text = "0";
            labShutShopCout.Text = "0";
            labShutShopOrderCout.Text = "0";
            
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
                //BindSubjectInfo();
                //BindProvince();
                //BindCity();
                //BindCustomerServiceName();
                //BindIsInstall();
                ClearInfo();
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
                ClearInfo();
                //BindSubjectInfo();
                //BindProvince();
                //BindCity();
                //BindCustomerServiceName();
                //BindIsInstall();
                //BindShopCount();
            }
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            BindGuidance();
            ClearInfo();
        }

        /// <summary>
        /// 选择活动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindSubjectActivity();
            BindSubjectInfo();
            BindProvince();
            BindCity();
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        /// <summary>
        /// 选择活动类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cblSubjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectActivity();
            BindRegion();
            BindSubject();
            BindProvince();
            BindCity();
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        /// <summary>
        /// 选择活动分类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cblActivity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
            BindSubject();
            BindProvince();
            BindCity();
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
            BindProvince();
            BindCity();
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        

        protected void cbSubjectAll_CheckedChanged(object sender, EventArgs e)
        {
            BindProvince();
            BindCity();
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        protected void cblSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
            BindCity();
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCity();
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        protected void cbProvinceAll_CheckedChanged(object sender, EventArgs e)
        {
            BindCity();
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        protected void cblCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        protected void cbCityAll_CheckedChanged(object sender, EventArgs e)
        {
            BindCustomerServiceName();
            BindIsInstall();
            BindShopCount();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
            ClearInfo();
        }
    }
}
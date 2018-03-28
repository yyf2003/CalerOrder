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

namespace WebApp.OutsourcingOrder.Statistics
{
    public partial class OrderStatistic : BasePage
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

        List<int> GetSubjectCategorySelected()
        {
            List<int> list = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected && !list.Contains(int.Parse(li.Value)))
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            return list;
        }

        List<string> GetRegionSelected()
        {
            List<string> list = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !list.Contains(li.Value))
                {
                    list.Add(li.Value);
                }
            }
            return list;
        }

        List<string> GetProvinceSelected()
        {
            List<string> list = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected && !list.Contains(li.Value))
                {
                    list.Add(li.Value);
                }
            }
            return list;
        }

        List<string> GetCitySelected()
        {
            List<string> list = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected && !list.Contains(li.Value))
                {
                    list.Add(li.Value);
                }
            }
            return list;
        }

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

        List<int> GetOutSourceSelected()
        {
            List<int> list = new List<int>();
            foreach (ListItem li in cblOutsource.Items)
            {
                if (li.Selected && !list.Contains(int.Parse(li.Value)))
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            return list;
        }


       

        void BindGuidance()
        {
            cblGuidanceList.Items.Clear();
            cblSubjectCategory.Items.Clear();
            cblRegion.Items.Clear();
            cblSubjects.Items.Clear();
            cbAll.Checked = false;
            cbAllDiv.Style.Add("display", "none");
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


            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();
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

        void BindSubjectCategory()
        {
            cblSubjectCategory.Items.Clear();
            List<int> guidanceIdList = GetGuidanceSelected();
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join category1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals category1.Id into categoryTemp
                             from category in categoryTemp.DefaultIfEmpty()
                             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 shop,
                                 order,
                                 subject.AddUserId,
                                 category
                             }).ToList();
            if (orderList.Any())
            {
                var categoryList = orderList.Select(s => s.category).Distinct().ToList();
                List<int> selectedList = new List<int>();
                //if (Session["subjectCategorySelected"] != null)
                //{
                //    selectedList = Session["subjectCategorySelected"] as List<int>;
                //}
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

        void BindRegion()
        {
            cblRegion.Items.Clear();
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 subject,
                                 order
                             }).ToList();
            if (orderList.Any())
            {
                if (subjectCategoryIdList.Any())
                {
                    if (subjectCategoryIdList.Contains(0))
                    {
                        subjectCategoryIdList.Remove(0);
                        if (subjectCategoryIdList.Any())
                        {
                            orderList = orderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        orderList = orderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (orderList.Any())
                {
                    var regionList = orderList.Select(s => s.order.Region).Distinct().ToList();
                    regionList.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            ListItem li = new ListItem();
                            li.Text = s + "&nbsp;&nbsp;";
                            li.Value = s;
                            cblRegion.Items.Add(li);
                        }
                       
                    });
                }
            }
        }

        void BindProvince()
        {
            cblProvince.Items.Clear();
            cblCity.Items.Clear();
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            if (regionList.Any())
            {
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                 && (subject.IsDelete == null || subject.IsDelete == false)
                                 && subject.ApproveState == 1
                                 && (order.IsDelete == null || order.IsDelete == false)
                                 select new
                                 {
                                     subject,
                                     order
                                 }).ToList();
                if (orderList.Any())
                {
                    if (subjectCategoryIdList.Any())
                    {
                        if (subjectCategoryIdList.Contains(0))
                        {
                            subjectCategoryIdList.Remove(0);
                            if (subjectCategoryIdList.Any())
                            {
                                orderList = orderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                            }
                            else
                                orderList = orderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderList = orderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                    }
                    if (regionList.Any())
                    {
                        orderList = orderList.Where(s => regionList.Contains(s.order.Region)).ToList();
                    }
                    if (orderList.Any())
                    {
                        var provinceList = orderList.Select(s => s.order.Province).Distinct().ToList();
                        provinceList.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                ListItem li = new ListItem();
                                li.Text = s + "&nbsp;&nbsp;";
                                li.Value = s;
                                cblProvince.Items.Add(li);
                            }

                        });
                    }
                }
            }
        }

        void BindCity()
        {
            
            cblCity.Items.Clear();
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            if (provinceList.Any())
            {
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                 && (subject.IsDelete == null || subject.IsDelete == false)
                                 && subject.ApproveState == 1
                                 && (order.IsDelete == null || order.IsDelete == false)
                                 select new
                                 {
                                     subject,
                                     order
                                 }).ToList();
                if (orderList.Any())
                {
                    if (subjectCategoryIdList.Any())
                    {
                        if (subjectCategoryIdList.Contains(0))
                        {
                            subjectCategoryIdList.Remove(0);
                            if (subjectCategoryIdList.Any())
                            {
                                orderList = orderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                            }
                            else
                                orderList = orderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderList = orderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                    }
                    if (regionList.Any())
                    {
                        orderList = orderList.Where(s => regionList.Contains(s.order.Region)).ToList();
                    }
                    if (provinceList.Any())
                    {
                        orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                    }
                    if (orderList.Any())
                    {
                        var cityList = orderList.Select(s => s.order.City).Distinct().ToList();
                        cityList.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                ListItem li = new ListItem();
                                li.Text = s + "&nbsp;&nbsp;";
                                li.Value = s;
                                cblCity.Items.Add(li);
                            }

                        });
                    }
                }
            }
        }

        void BindSubject()
        {
            cblSubjects.Items.Clear();
            cbAll.Checked = false;
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            List<string> cityList = GetCitySelected();
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 subject,
                                 order
                                 
                             }).ToList();
            List<int> subjectIdInOrderList = orderList.Select(s => s.subject.Id).ToList();
            if (orderList.Any())
            {
                if (subjectCategoryIdList.Any())
                {
                    if (subjectCategoryIdList.Contains(0))
                    {
                        subjectCategoryIdList.Remove(0);
                        if (subjectCategoryIdList.Any())
                        {
                            orderList = orderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            orderList = orderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        orderList = orderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (regionList.Any())
                {
                    orderList = orderList.Where(s => regionList.Contains(s.order.Region)).ToList();
                }
                if (provinceList.Any())
                {
                    orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
                if (cityList.Any())
                {
                    orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
                }
                var subjectList = orderList.Select(s => s.subject).Distinct().OrderBy(s => s.GuidanceId).ThenBy(s => s.SubjectName).ToList();

                
                //List<int> selectedList = new List<int>();
                //if (Session["subjectSelected"] != null)
                //{
                //    selectedList = Session["subjectSelected"] as List<int>;
                //}
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
                    li.Text = subjectName + "&nbsp;&nbsp;";
                    li.Value = s.Id.ToString();
                    //if (selectedList.Contains(s.Id))
                        //li.Selected = true;
                    cblSubjects.Items.Add(li);
                });
            }
            
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
                                       && !subjectIdInOrderList.Contains(subject.Id)
                                       select new
                                       {
                                           subject,
                                           shop
                                       }
                                      ).ToList();
           
            if (materialSubjectList.Any())
            {
                if (subjectCategoryIdList.Any())
                {
                    if (subjectCategoryIdList.Contains(0))
                    {
                        subjectCategoryIdList.Remove(0);
                        if (subjectCategoryIdList.Any())
                        {
                            materialSubjectList = materialSubjectList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            materialSubjectList = materialSubjectList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        materialSubjectList = materialSubjectList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (regionList.Any())
                {
                    materialSubjectList = materialSubjectList.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
                }
                if (provinceList.Any())
                {
                    materialSubjectList = materialSubjectList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                }
                if (cityList.Any())
                {
                    materialSubjectList = materialSubjectList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                }
                
                var subjectList0 = materialSubjectList.Select(s => s.subject).Distinct().OrderBy(s => s.GuidanceId).ToList();
                subjectList0.OrderBy(s => s.Id).ToList().ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string subjectName = s.SubjectName;

                    li.Text = subjectName + "&nbsp;&nbsp;";
                    li.Value = s.Id.ToString();
                   
                    cblSubjects.Items.Add(li);
                });
            }
            if (orderList.Any() || materialSubjectList.Any())
            {
                cbAllDiv.Style.Add("display", "block");
            }
            else
            {
                cbAllDiv.Style.Add("display", "none");
            }
        }

        void BindOutSource()
        {
            cblOutsource.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            List<string> cityList = GetCitySelected();
            List<int> subjectIdList = GetSubjectSelected();

            var outsourceOrderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join company in CurrentContext.DbContext.Company
                                     on order.OutsourceId equals company.Id
                                     where guidanceIdList.Contains(order.GuidanceId ?? 0)
                                     && (order.IsDelete == null || order.IsDelete==false)
                                     select new {
                                         order,
                                         subject,
                                         company
                                     }).ToList();
            if (outsourceOrderList.Any())
            {
                if (subjectCategoryIdList.Any())
                {
                    if (subjectCategoryIdList.Contains(0))
                    {
                        subjectCategoryIdList.Remove(0);
                        if (subjectCategoryIdList.Any())
                        {
                            outsourceOrderList = outsourceOrderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            outsourceOrderList = outsourceOrderList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        outsourceOrderList = outsourceOrderList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (regionList.Any())
                {
                    outsourceOrderList = outsourceOrderList.Where(s => regionList.Contains(s.order.Region)).ToList();
                }
                if (provinceList.Any())
                {
                    outsourceOrderList = outsourceOrderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
                if (cityList.Any())
                {
                    outsourceOrderList = outsourceOrderList.Where(s => cityList.Contains(s.order.City)).ToList();
                }
                if (subjectIdList.Any())
                {
                    outsourceOrderList = outsourceOrderList.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
                }
                if (outsourceOrderList.Any())
                {
                    var companyList = outsourceOrderList.Select(s => s.company).Distinct().OrderBy(s=>s.Id).ToList();
                    companyList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s.CompanyName + "&nbsp;&nbsp;";
                        li.Value = s.Id.ToString();
                        cblOutsource.Items.Add(li);

                    });
                }
            }
            loadOutsource.Style.Add("display", "none");
        }

        void Search()
        {
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            List<string> cityList = GetCitySelected();
            List<int> subjectIdList = GetSubjectSelected();
            List<int> subjectOutsourceIdList = GetOutSourceSelected();
            if (subjectOutsourceIdList.Any())
            { 
               
            }
        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectCategory();
            BindRegion();
            BindSubject();
            BindOutSource();
        }

        protected void cblSubjectCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
            BindSubject();
            BindOutSource();
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
            BindSubject();
            BindOutSource();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCity();
            BindSubject();
            BindOutSource();
        }

        protected void cblCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
            BindOutSource();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
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
                
            }
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }

        protected void cblSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindOutSource();
        }

        protected void cblOutspurce_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
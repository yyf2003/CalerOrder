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


       
        void LoadData() {
            List<int> guidanceIdList = GetGuidanceSelected();
            var oList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subject1 in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject1.Id into subjectTemp
                             from subject in subjectTemp.DefaultIfEmpty()
                             where guidanceIdList.Contains(order.GuidanceId ?? 0)
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new
                             {
                                 shop,
                                 order,
                                 subject,
                             }).ToList();
            if (oList.Any())
            {
                List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
                List<Subject> subjectList = new List<Subject>();
                List<Shop> shopList = new List<Shop>();
                orderList = oList.Select(s => s.order).ToList();
                subjectList = oList.Where(s => s.subject != null).Select(s => s.subject).Distinct().ToList();
                shopList = oList.Select(s => s.shop).Distinct().ToList();
                Session["OutsourceStatisticOrderList"] = orderList;
                Session["OutsourceStatisticShopList"] = shopList;
                Session["OutsourceStatisticSubjectList"] = subjectList;

            }
            else
            {
                Session["OutsourceStatisticOrderList"] = null;
                Session["OutsourceStatisticShopList"] = null;
                Session["OutsourceStatisticSubjectList"] = null;
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
            foreach (ListItem li in cblOutsourceId.Items)
            {
                if (li.Selected && !list.Contains(int.Parse(li.Value)))
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            if (!list.Any()) {
                foreach (ListItem li in cblOutsourceId.Items)
                {
                    if (!list.Contains(int.Parse(li.Value)))
                    {
                        list.Add(int.Parse(li.Value));
                    }
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
            //List<int> guidanceIdList = GetGuidanceSelected();
            //var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
            //                 join subject in CurrentContext.DbContext.Subject
            //                 on order.SubjectId equals subject.Id
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on order.ShopId equals shop.Id
            //                 join category1 in CurrentContext.DbContext.ADSubjectCategory
            //                 on subject.SubjectCategoryId equals category1.Id into categoryTemp
            //                 from category in categoryTemp.DefaultIfEmpty()
            //                 where guidanceIdList.Contains(order.GuidanceId ?? 0)
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
            //List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<Subject> subjectList = new List<Subject>();
            //List<Shop> shopList = new List<Shop>();
            if (Session["OutsourceStatisticSubjectList"] != null)
            {
                //orderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
                //shopList = Session["OutsourceStatisticShopList"] as List<Shop>;
            }

            if (subjectList.Any())
            {
                var categoryList = (from subject in subjectList
                           join category1 in CurrentContext.DbContext.ADSubjectCategory
                           on subject.SubjectCategoryId equals category1.Id into categoryTemp
                           from category in categoryTemp.DefaultIfEmpty()
                           select category).Distinct().ToList();
                //var categoryList = orderList.Select(s => s.category).Distinct().ToList();
                List<int> selectedList = new List<int>();
               
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
            cblProvince.Items.Clear();
            cblCity.Items.Clear();
            //List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            //var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
            //                 join subject in CurrentContext.DbContext.Subject
            //                 on order.SubjectId equals subject.Id
            //                 where guidanceIdList.Contains(order.GuidanceId ?? 0)
            //                 && (subject.IsDelete == null || subject.IsDelete == false)
            //                 && subject.ApproveState == 1
            //                 && (order.IsDelete == null || order.IsDelete == false)
            //                 select new
            //                 {
            //                     subject,
            //                     order
            //                 }).ToList();

            List<Subject> subjectList = new List<Subject>();
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            if (Session["OutsourceStatisticOrderList"] != null)
            {
                orderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
            }

            if (orderList.Any())
            {
                var list = (from order in orderList
                            join subject in subjectList
                            on order.SubjectId equals subject.Id
                            select new
                            {
                                order,
                                subject
                            }).ToList();
                if (subjectCategoryIdList.Any())
                {
                    if (subjectCategoryIdList.Contains(0))
                    {
                        subjectCategoryIdList.Remove(0);
                        if (subjectCategoryIdList.Any())
                        {
                            list = list.Where(s => (subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0))).ToList();
                        }
                        else
                            list = list.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        list = list.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (list.Any())
                {
                    var regionList = list.Select(s => s.order.Region).Distinct().ToList();
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
            //List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            if (regionList.Any())
            {
                //var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                //                 join subject in CurrentContext.DbContext.Subject
                //                 on order.SubjectId equals subject.Id
                //                 where guidanceIdList.Contains(order.GuidanceId ?? 0)
                //                 && (subject.IsDelete == null || subject.IsDelete == false)
                //                 && subject.ApproveState == 1
                //                 && (order.IsDelete == null || order.IsDelete == false)
                //                 select new
                //                 {
                //                     subject,
                //                     order
                //                 }).ToList();
                List<Subject> subjectList = new List<Subject>();
                List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
                if (Session["OutsourceStatisticOrderList"] != null)
                {
                    orderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                    subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
                }
                if (orderList.Any())
                {
                    var list = (from order in orderList
                                join subject in subjectList
                                on order.SubjectId equals subject.Id
                                select new
                                {
                                    order,
                                    subject
                                }).ToList();
                    if (subjectCategoryIdList.Any())
                    {
                        if (subjectCategoryIdList.Contains(0))
                        {
                            subjectCategoryIdList.Remove(0);
                            if (subjectCategoryIdList.Any())
                            {
                                list = list.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                            }
                            else
                                list = list.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            list = list.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                    }
                    if (regionList.Any())
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region)).ToList();
                    }
                    if (list.Any())
                    {
                        var provinceList = list.Select(s => s.order.Province).Distinct().ToList();
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
            //List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            if (provinceList.Any())
            {
                //var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                //                 join subject in CurrentContext.DbContext.Subject
                //                 on order.SubjectId equals subject.Id
                //                 where guidanceIdList.Contains(order.GuidanceId ?? 0)
                //                 && (subject.IsDelete == null || subject.IsDelete == false)
                //                 && subject.ApproveState == 1
                //                 && (order.IsDelete == null || order.IsDelete == false)
                //                 select new
                //                 {
                //                     subject,
                //                     order
                //                 }).ToList();
                List<Subject> subjectList = new List<Subject>();
                List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
                if (Session["OutsourceStatisticOrderList"] != null)
                {
                    orderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                    subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
                }
                if (orderList.Any())
                {
                    var list = (from order in orderList
                                join subject in subjectList
                                on order.SubjectId equals subject.Id
                                select new
                                {
                                    order,
                                    subject
                                }).ToList();
                    if (subjectCategoryIdList.Any())
                    {
                        if (subjectCategoryIdList.Contains(0))
                        {
                            subjectCategoryIdList.Remove(0);
                            if (subjectCategoryIdList.Any())
                            {
                                list = list.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                            }
                            else
                                list = list.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            list = list.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                    }
                    if (regionList.Any())
                    {
                        list = list.Where(s => regionList.Contains(s.order.Region)).ToList();
                    }
                    if (provinceList.Any())
                    {
                        list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();
                    }
                    if (list.Any())
                    {
                        var cityList = list.Select(s => s.order.City).Distinct().ToList();
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
            //List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            List<string> cityList = GetCitySelected();
            //var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
            //                 join subject in CurrentContext.DbContext.Subject
            //                 on order.SubjectId equals subject.Id
            //                 where guidanceIdList.Contains(order.GuidanceId ?? 0)
            //                 && (subject.IsDelete == null || subject.IsDelete == false)
            //                 && subject.ApproveState == 1
            //                 && (order.IsDelete == null || order.IsDelete == false)
            //                 select new
            //                 {
            //                     subject,
            //                     order
                                 
            //                 }).ToList();

            List<Subject> subjectList = new List<Subject>();
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            if (Session["OutsourceStatisticOrderList"] != null)
            {
                orderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
            }


            //List<int> subjectIdInOrderList = orderList.Select(s => s.subject.Id).ToList();
            if (orderList.Any())
            {
                var list = (from order in orderList
                            join subject in subjectList
                            on order.SubjectId equals subject.Id
                            select new
                            {
                                order,
                                subject
                            }).ToList();
                if (subjectCategoryIdList.Any())
                {
                    if (subjectCategoryIdList.Contains(0))
                    {
                        subjectCategoryIdList.Remove(0);
                        if (subjectCategoryIdList.Any())
                        {
                            list = list.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            list = list.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        list = list.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
                }
                if (regionList.Any())
                {
                    list = list.Where(s => regionList.Contains(s.order.Region)).ToList();
                }
                if (provinceList.Any())
                {
                    list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();
                }
                if (cityList.Any())
                {
                    list = list.Where(s => cityList.Contains(s.order.City)).ToList();
                }
                var subjectList0 = list.Select(s => s.subject).Distinct().OrderBy(s => s.GuidanceId).ThenBy(s => s.SubjectName).ToList();

                
                //List<int> selectedList = new List<int>();
                //if (Session["subjectSelected"] != null)
                //{
                //    selectedList = Session["subjectSelected"] as List<int>;
                //}
                subjectList0.OrderBy(s => s.Id).ToList().ForEach(s =>
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
            
            ////List<int> materialSubjectList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1).Select(s => s.Id).ToList();
            ////单独的物料项目（不含pop）
            //var materialSubjectList = (from materialOrder in CurrentContext.DbContext.OrderMaterial
            //                           join subject in CurrentContext.DbContext.Subject
            //                           on materialOrder.SubjectId equals subject.Id
            //                           join shop in CurrentContext.DbContext.Shop
            //                           on materialOrder.ShopId equals shop.Id
            //                           where guidanceIdList.Contains(subject.GuidanceId ?? 0)
            //                           && (subject.IsDelete == null || subject.IsDelete == false)
            //                           && subject.ApproveState == 1
            //                           && !subjectIdInOrderList.Contains(subject.Id)
            //                           select new
            //                           {
            //                               subject,
            //                               shop
            //                           }
            //                          ).ToList();
           
            //if (materialSubjectList.Any())
            //{
            //    if (subjectCategoryIdList.Any())
            //    {
            //        if (subjectCategoryIdList.Contains(0))
            //        {
            //            subjectCategoryIdList.Remove(0);
            //            if (subjectCategoryIdList.Any())
            //            {
            //                materialSubjectList = materialSubjectList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
            //            }
            //            else
            //                materialSubjectList = materialSubjectList.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
            //        }
            //        else
            //            materialSubjectList = materialSubjectList.Where(s => subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
            //    }
            //    if (regionList.Any())
            //    {
            //        materialSubjectList = materialSubjectList.Where(s => regionList.Contains(s.shop.RegionName)).ToList();
            //    }
            //    if (provinceList.Any())
            //    {
            //        materialSubjectList = materialSubjectList.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            //    }
            //    if (cityList.Any())
            //    {
            //        materialSubjectList = materialSubjectList.Where(s => cityList.Contains(s.shop.CityName)).ToList();
            //    }
                
            //    var subjectList0 = materialSubjectList.Select(s => s.subject).Distinct().OrderBy(s => s.GuidanceId).ToList();
            //    subjectList0.OrderBy(s => s.Id).ToList().ForEach(s =>
            //    {
            //        ListItem li = new ListItem();
            //        string subjectName = s.SubjectName;

            //        li.Text = subjectName + "&nbsp;&nbsp;";
            //        li.Value = s.Id.ToString();
                   
            //        cblSubjects.Items.Add(li);
            //    });
            //}
            //if (orderList.Any() || materialSubjectList.Any())
            //{
            //    cbAllDiv.Style.Add("display", "block");
            //}
            //else
            //{
            //    cbAllDiv.Style.Add("display", "none");
            //}
        }

        void BindOutSource()
        {
            cblOutsourceId.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            List<string> cityList = GetCitySelected();
            List<int> subjectIdList = GetSubjectSelected();

            var outsourceOrderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                     join subject1 in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject1.Id into subjectTemp
                                     join company in CurrentContext.DbContext.Company
                                     on order.OutsourceId equals company.Id
                                      from subject in subjectTemp.DefaultIfEmpty()
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
                            outsourceOrderList = outsourceOrderList.Where(s =>s.subject!=null && subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            outsourceOrderList = outsourceOrderList.Where(s => s.subject != null && (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        outsourceOrderList = outsourceOrderList.Where(s => s.subject != null && subjectCategoryIdList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
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
                    outsourceOrderList = outsourceOrderList.Where(s =>s.subject!=null && subjectIdList.Contains(s.subject.Id)).ToList();
                }
                if (outsourceOrderList.Any())
                {
                    var companyList = outsourceOrderList.Select(s => s.company).Distinct().OrderBy(s=>s.Id).ToList();
                    companyList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Text = s.CompanyName + "&nbsp;&nbsp;";
                        li.Value = s.Id.ToString();
                        cblOutsourceId.Items.Add(li);

                    });
                }
            }
            loadOutsource.Style.Add("display", "none");
        }

        void Search()
        {
            CleanLab();
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            List<string> cityList = GetCitySelected();
            List<int> subjectIdList = GetSubjectSelected();
            List<int> subjectOutsourceIdList = GetOutSourceSelected();
            List<int> assignTypeList = new List<int>();
            foreach (ListItem li in cblOutsourceType.Items) {
                if (li.Selected) {
                    assignTypeList.Add(int.Parse(li.Value));
                }
            }
            string guidanceMonth = string.Empty;
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text)) {
                guidanceMonth = txtGuidanceMonth.Text.Trim();
            }
            if (subjectOutsourceIdList.Any() && guidanceIdList.Any())
            {
                List<OutsourceOrderDetail> totalOrderList = new List<OutsourceOrderDetail>();
                List<Subject> subjectList = new List<Subject>();
                List<Shop> shopList = new List<Shop>();
                if (Session["OutsourceStatisticOrderList"] != null)
                {
                    totalOrderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                    subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
                    shopList = Session["OutsourceStatisticShopList"] as List<Shop>;
                }
                if (totalOrderList.Any())
                {
                    int totalShopCount = 0;
                    decimal totalArea = 0;

                    decimal popPrice = 0;
                    decimal receivePOPPrice = 0;

                    decimal installPrice = 0;
                    decimal receiveinstallPrice = 0;

                    decimal expressPrice = 0;
                    decimal receiveExpressPrice = 0;

                    decimal measurePrice = 0;
                    decimal receiveMeasurePrice = 0;
                    decimal otherPrice = 0;
                    decimal receiveOtherPrice = 0;
                    List<int> TotalShopCountList = new List<int>();
                    #region 
                    guidanceIdList.ForEach(gid =>
                    {
                        //是否全部三叶草
                        bool isBCSSubject = false;
                        var orderList0 = (from order in totalOrderList
                                          where subjectOutsourceIdList.Contains(order.OutsourceId ?? 0)
                                          && gid == order.GuidanceId
                                          && (order.IsDelete == null || order.IsDelete == false)
                                          && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType > 1)
                                          select new
                                          {
                                              order

                                          }).ToList();

                        //var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.OutsourceId == outsourceId && guidanceIdList.Contains(s.GuidanceId ?? 0));
                        var orderList = orderList0;

                        if (subjectIdList.Any())
                        {
                            orderList = orderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();

                            var NotBCSSubjectList = new SubjectBLL().GetList(s => subjectIdList.Contains(s.Id) && (s.CornerType == null || !s.CornerType.Contains("三叶草")));
                            isBCSSubject = !NotBCSSubjectList.Any();

                        }
                        if (regionList.Any())
                        {
                            orderList = orderList.Where(s => s.order.Region != null && regionList.Contains(s.order.Region)).ToList();
                        }
                        if (provinceList.Any())
                        {
                            orderList = orderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                        }
                        if (cityList.Any())
                        {
                            orderList = orderList.Where(s => cityList.Contains(s.order.City)).ToList();
                        }
                        if (assignTypeList.Any())
                        {
                            orderList = orderList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                        }

                        if (orderList.Any())
                        {
                            List<int> shopIdList = orderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                            TotalShopCountList.AddRange(shopIdList);
                            List<int> installShopIdList = shopIdList;
                            if (isBCSSubject)
                            {
                                //如果是三叶草，把出现在大货订单里面的店铺去掉
                                List<int> totalOrderShopIdList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                                                  join subject in CurrentContext.DbContext.Subject
                                                                  on order.SubjectId equals subject.Id
                                                                  where order.GuidanceId == gid
                                                                  && !subjectIdList.Contains(order.SubjectId ?? 0)
                                                                  && subject.ApproveState == 1
                                                                  && (subject.IsDelete == null || subject.IsDelete == false)
                                                                  && (order.IsDelete == null || order.IsDelete == false)
                                                                  && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                                                                  select order.ShopId ?? 0).Distinct().ToList();
                                installShopIdList = installShopIdList.Except(totalOrderShopIdList).ToList();

                            }

                            //安装费
                            var installOrderPriceList = orderList0.Where(s => installShopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                            if (installOrderPriceList.Any())
                            {
                                if (assignTypeList.Any())
                                {
                                    installOrderPriceList = installOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                                }
                                installPrice += installOrderPriceList.Sum(s => s.order.PayOrderPrice ?? 0);
                                receiveinstallPrice += installOrderPriceList.Sum(s => s.order.ReceiveOrderPrice ?? 0);
                            }
                            //快递费
                            var expressOrderPriceList = orderList0.Where(s => shopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.运费)).ToList();
                            if (expressOrderPriceList.Any())
                            {
                                if (assignTypeList.Any())
                                {
                                    expressOrderPriceList = expressOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                                }
                                expressPrice += expressOrderPriceList.Sum(s => s.order.PayOrderPrice ?? 0);
                                receiveExpressPrice += expressOrderPriceList.Sum(s => s.order.ReceiveOrderPrice ?? 0);
                            }
                            var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.GuidanceId == gid && installShopIdList.Contains(s.ShopId ?? 0) && subjectOutsourceIdList.Contains(s.OutsourceId ?? 0)).ToList();
                            assignShopList.ForEach(s =>
                            {
                                if ((s.PayInstallPrice ?? 0) > 0)
                                    installPrice += (s.PayInstallPrice ?? 0);

                                if ((s.ReceiveInstallPrice ?? 0) > 0)
                                    receiveinstallPrice += (s.ReceiveInstallPrice ?? 0);

                                if ((s.PayExpressPrice ?? 0) > 0)
                                    expressPrice += (s.PayExpressPrice ?? 0);

                                if ((s.ReceiveExpresslPrice ?? 0) > 0)
                                    receiveExpressPrice += (s.ReceiveExpresslPrice ?? 0);
                            });
                            if (orderList.Any())
                            {
                                decimal installPrice1 = 0;
                                decimal receiveInstallPrice1 = 0;
                                orderList = orderList.Where(s => s.order.SubjectId > 0).ToList();
                                orderList.ForEach(s =>
                                {
                                    if (s.order.GraphicLength != null && s.order.GraphicWidth != null)
                                    {
                                        if (s.order.GraphicMaterial == "挂轴")
                                        {
                                            totalArea += ((s.order.GraphicWidth ?? 0) / 1000) * 2 * (s.order.Quantity ?? 1);
                                        }
                                        else
                                        {
                                            totalArea += ((s.order.GraphicLength ?? 0) * (s.order.GraphicWidth ?? 0) / 1000000) * (s.order.Quantity ?? 1);
                                        }
                                    }
                                    popPrice += (s.order.TotalPrice ?? 0);
                                    receivePOPPrice += (s.order.ReceiveTotalPrice ?? 0);

                                    if (s.order.OrderType == (int)OrderTypeEnum.测量费)
                                    {
                                        measurePrice += (s.order.PayOrderPrice ?? 0);
                                        receiveMeasurePrice += (s.order.ReceiveOrderPrice ?? 0);
                                    }
                                    if (s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                    {
                                        otherPrice += (s.order.PayOrderPrice ?? 0);
                                        receiveOtherPrice += (s.order.ReceiveOrderPrice ?? 0);
                                    }
                                    if (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.运费)
                                    {
                                        expressPrice += (s.order.PayOrderPrice ?? 0);
                                        receiveExpressPrice += (s.order.ReceiveOrderPrice ?? 0);
                                    }
                                });
                                //安装费单独算
                                var installOrderList = orderList.Where(s => installShopIdList.Contains(s.order.ShopId ?? 0)).ToList();
                                installOrderList.ForEach(s =>
                                {
                                    if (s.order.OrderType == (int)OrderTypeEnum.安装费)
                                    {
                                        installPrice1 += (s.order.PayOrderPrice ?? 0);
                                        receiveInstallPrice1 += (s.order.ReceiveOrderPrice ?? 0);
                                    }
                                });

                                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                                {
                                    DateTime date0 = DateTime.Parse(guidanceMonth);
                                    int year = date0.Year;
                                    int month = date0.Month;
                                    //应收外协费用（裱板费）
                                    var pvcOrderList = new OutsourceReceivePriceOrderBLL().GetList(s => subjectOutsourceIdList.Contains(s.OutsourceId ?? 0) && shopIdList.Contains(s.ShopId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                                    if (pvcOrderList.Any())
                                    {
                                        decimal materialPrice = pvcOrderList.Sum(s => s.TotalPrice ?? 0);
                                        popPrice = popPrice - materialPrice;
                                        popPrice = popPrice > 0 ? popPrice : 0;
                                    }
                                }

                                installPrice += installPrice1;
                                receiveinstallPrice += receiveInstallPrice1;

                            }

                        }
                    });

                    #endregion

                    #region 特殊外协费用
                    
                    if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                    {
                        DateTime date0 = DateTime.Parse(guidanceMonth);
                        int year = date0.Year;
                        int month = date0.Month;
                        var outsourcePriceOrderList = new OutsourcePriceOrderBLL().GetList(s => subjectOutsourceIdList.Contains(s.OutsourceId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                        if (outsourcePriceOrderList.Any())
                        {
                            installPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费).Sum(s => s.PayPrice ?? 0);
                            measurePrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.测量费).Sum(s => s.PayPrice ?? 0);
                            expressPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.发货费).Sum(s => s.PayPrice ?? 0);
                            otherPrice += outsourcePriceOrderList.Where(s => s.OrderType == (int)OrderTypeEnum.其他费用).Sum(s => s.PayPrice ?? 0);
                        }
                    }
                    decimal totalPrice = (popPrice + installPrice + expressPrice + measurePrice + otherPrice);
                    decimal receiveTotalPrice = (receivePOPPrice + receiveinstallPrice + receiveExpressPrice + receiveMeasurePrice + receiveOtherPrice);
                    if (totalPrice > 0)
                        totalPrice = Math.Round(totalPrice, 2);
                    if (popPrice > 0)
                        popPrice = Math.Round(popPrice, 2);
                    if (installPrice > 0)
                        installPrice = Math.Round(installPrice, 2);
                    if (expressPrice > 0)
                        expressPrice = Math.Round(expressPrice, 2);
                    if (receiveExpressPrice > 0)
                        receiveExpressPrice = Math.Round(receiveExpressPrice, 2);
                    if (measurePrice > 0)
                        measurePrice = Math.Round(measurePrice, 2);
                    if (receiveMeasurePrice > 0)
                        receiveMeasurePrice = Math.Round(receiveMeasurePrice, 2);
                    if (otherPrice > 0)
                        otherPrice = Math.Round(otherPrice, 2);
                    if (receiveOtherPrice > 0)
                        receiveOtherPrice = Math.Round(receiveOtherPrice, 2);
                    if (totalArea > 0)
                        totalArea = Math.Round(totalArea, 2);

                    if (receiveinstallPrice > 0)
                        receiveinstallPrice = Math.Round(receiveinstallPrice, 2);
                    if (receivePOPPrice > 0)
                        receivePOPPrice = Math.Round(receivePOPPrice, 2);
                    if (receiveTotalPrice > 0)
                        receiveTotalPrice = Math.Round(receiveTotalPrice, 2);
                    //System.Text.StringBuilder json = new System.Text.StringBuilder();
                    totalShopCount = TotalShopCountList.Distinct().Count();

                    //应付

                    labShopCount.Text = totalShopCount.ToString();
                    labArea.Text = totalArea.ToString();
                    if (popPrice > 0)
                    {
                        labPOPPrice.Text = popPrice + " 元";
                        labPOPPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        labPOPPrice.Attributes.Add("name", "checkPOPOrderPrice");
                    }

                    if (installPrice > 0)
                    {
                        labInstallPrice.Text = installPrice + " 元";
                        labInstallPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        labInstallPrice.Attributes.Add("name", "checkInstallPrice");
                    }

                    if (expressPrice > 0)
                    {
                        labExpressPrice.Text = expressPrice + " 元";
                        labExpressPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        labExpressPrice.Attributes.Add("name", "checkExpressPrice");
                    }

                    labMeasurePrice.Text = measurePrice.ToString();
                    if (otherPrice > 0)
                    {
                        labOtherPrice.Text = otherPrice + " 元";
                        labOtherPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        labOtherPrice.Attributes.Add("name", "checkOtherPrice");
                    }

                    labTotalPrice.Text = totalPrice.ToString();

                    //应收
                    if (receivePOPPrice > 0)
                    {
                        labRPOPPrice.Text = receivePOPPrice + " 元";
                        labRPOPPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        labRPOPPrice.Attributes.Add("name", "checkPOPOrderPrice");
                    }

                    if (receiveinstallPrice > 0)
                    {
                        labRInstallPrice.Text = receiveinstallPrice + " 元";
                        labRInstallPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        labRInstallPrice.Attributes.Add("name", "checkInstallPrice");
                    }

                    if (receiveExpressPrice > 0)
                    {
                        labRExpressPrice.Text = receiveExpressPrice + " 元";
                        labRExpressPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        labRExpressPrice.Attributes.Add("name", "checkExpressPrice");
                    }


                    labRMeasurePrice.Text = receiveMeasurePrice.ToString();
                    if (receiveOtherPrice > 0)
                    {
                        labROtherPrice.Text = receiveOtherPrice + " 元";
                        labROtherPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        labROtherPrice.Attributes.Add("name", "checkOtherPrice");
                    }

                    labRTotalPrice.Text = receiveTotalPrice.ToString();
                    #endregion
                }
            }
        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
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
            Search();
        }

        protected void cblSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindOutSource();
        }

        protected void cblOutspurce_SelectedIndexChanged(object sender, EventArgs e)
        {

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

            labOtherPrice.Text = "0";
            labOtherPrice.Attributes.Remove("style");
            labOtherPrice.Attributes.Remove("name");
            
            labMeasurePrice.Text = "0";

            labRPOPPrice.Text = "0";
            labRPOPPrice.Attributes.Remove("style");
            labRPOPPrice.Attributes.Remove("name");

            labRExpressPrice.Text = "0";
            labRExpressPrice.Attributes.Remove("style");
            labRExpressPrice.Attributes.Remove("name");

            labRInstallPrice.Text = "0";
            labRInstallPrice.Attributes.Remove("style");
            labRInstallPrice.Attributes.Remove("name");

            labROtherPrice.Text = "0";
            labROtherPrice.Attributes.Remove("style");
            labROtherPrice.Attributes.Remove("name");

            labRMeasurePrice.Text = "0";
           

        }
    }
}
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
using System.Configuration;
using System.IO;
using NPOI.SS.UserModel;

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


       
        void LoadData_old() {
            List<int> guidanceIdList = GetGuidanceSelected();
            var oList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on order.GuidanceId equals guidance.ItemId
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
                                 guidance
                             }).ToList();
            if (oList.Any())
            {
                List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
                List<Subject> subjectList = new List<Subject>();
                List<Shop> shopList = new List<Shop>();
                List<SubjectGuidance> guidanceList = new List<SubjectGuidance>();
                orderList = oList.Select(s => s.order).ToList();
                subjectList = oList.Where(s => s.subject != null).Select(s => s.subject).Distinct().ToList();
                List<int> allSujectIdList = subjectList.Select(s => s.Id).ToList();
                List<Subject> emptyPOPSubjectList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && !allSujectIdList.Contains(s.Id) && s.Region!=null && s.Region!="" && s.ApproveState==1 && (s.IsDelete==null || s.IsDelete==false));
                subjectList.AddRange(emptyPOPSubjectList);
                shopList = oList.Select(s => s.shop).Distinct().ToList();
                guidanceList = oList.Select(s=>s.guidance).Distinct().ToList();
                Session["OutsourceStatisticOrderList"] = orderList;
                Session["OutsourceStatisticShopList"] = shopList;
                Session["OutsourceStatisticSubjectList"] = subjectList;
                Session["OutsourceStatisticGuidanceList"] = guidanceList;
            }
            else
            {
                Session["OutsourceStatisticOrderList"] = null;
                Session["OutsourceStatisticShopList"] = null;
                Session["OutsourceStatisticSubjectList"] = null;
                Session["OutsourceStatisticGuidanceList"] = null;
            }
             
        }

        /// <summary>
        /// 设置POP缓存
        /// </summary>
        void LoadData()
        {
            #region POP订单信息
            List<int> guidanceIdList = GetGuidanceSelected();
            var subjectListTemp = (from subject in CurrentContext.DbContext.Subject
                                   join guidance in CurrentContext.DbContext.SubjectGuidance
                                   on subject.GuidanceId equals guidance.ItemId
                                   where guidanceIdList.Contains(subject.GuidanceId ?? 0)
                                   && (subject.IsDelete == null || subject.IsDelete == false)
                                   && subject.ApproveState == 1
                                   select new { subject, guidance }
                              ).ToList();

            //List<int> subjectIdList = subjectListTemp.Select(s => s.subject.Id).ToList();
            var orderList0 = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                              from gid in guidanceIdList
                             where order.GuidanceId==gid
                             && (order.IsDelete==null || order.IsDelete==false)
                             select order).ToList();
            if (orderList0.Any())
            {
                
                List<Subject> subjectList = new List<Subject>();
                List<Shop> shopList = new List<Shop>();
                List<SubjectGuidance> guidanceList = new List<SubjectGuidance>();

                List<int> shopIdList = orderList0.Select(s => s.ShopId ?? 0).Distinct().ToList();
                shopList = new ShopBLL().GetList(s => shopIdList.Contains(s.Id)).ToList();
                
                guidanceList = subjectListTemp.Select(s => s.guidance).Distinct().OrderBy(s => s.ItemId).ThenBy(s => s.ItemName).ToList();
               
                guidanceIdList = guidanceList.Select(s => s.ItemId).ToList();

                subjectList = subjectListTemp.Select(s => s.subject).ToList();
                

                Session["OutsourceStatisticOrderList"] = orderList0;
                Session["OutsourceStatisticShopList"] = shopList;
                Session["OutsourceStatisticSubjectList"] = subjectList;
                Session["OutsourceStatisticGuidanceList"] = guidanceList;

            }
            else
            {
                Session["OutsourceStatisticOrderList"] = null;
                Session["OutsourceStatisticShopList"] = null;
                Session["OutsourceStatisticSubjectList"] = null;
                Session["OutsourceStatisticGuidanceList"] = null;
            }
            #endregion

        }

        /// <summary>
        /// 设置道具缓存
        /// </summary>
        void LoadPropData()
        {
            #region 道具订单信息
            Session["OutsourceStatisticPropOrderList"] = null;
            Session["OutsourceStatisticSubjectList"] = null;
            List<int> propGuidanceIdList = new List<int>();
            foreach (ListItem li in cblPropGuidanceList.Items)
            {
                if (li.Selected)
                {
                    propGuidanceIdList.Add(int.Parse(li.Value));
                }
            }
            if (propGuidanceIdList.Any())
            {
                var propOrderList = (from order in CurrentContext.DbContext.PropOutsourceOrderDetail
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     where propGuidanceIdList.Contains(order.GuidanceId ?? 0)
                                     select new
                                     {
                                         order,
                                         subject
                                     }).ToList();
                if (propOrderList.Any())
                {
                    Session["OutsourceStatisticPropOrderList"] = propOrderList.Select(s => s.order).ToList();
                    Session["OutsourceStatisticSubjectList"] = propOrderList.Select(s => s.subject).Distinct().ToList();
                }
            }
            #endregion
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

        List<int> GetPropSubjectSelected()
        {
            List<int> list = new List<int>();
            foreach (ListItem li in cblPropSubjects.Items)
            {
                if (li.Selected)
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            if (!list.Any())
            {
                foreach (ListItem li in cblPropSubjects.Items)
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            return list;
        }

        //StringBuilder outsourceNames = new StringBuilder();
        List<int> GetOutSourceSelected()
        {
            //outsourceNames.Clear();
            List<int> list = new List<int>();
            foreach (ListItem li in cblOutsourceId.Items)
            {
                if (li.Selected && !list.Contains(int.Parse(li.Value)))
                {
                    list.Add(int.Parse(li.Value));
                    //outsourceNames.Append(li.Text);
                    //outsourceNames.Append('/');
                }
            }
            if (!list.Any()) {
                foreach (ListItem li in cblOutsourceId.Items)
                {
                    if (!list.Contains(int.Parse(li.Value)))
                    {
                        list.Add(int.Parse(li.Value));
                        //outsourceNames.Append(li.Text);
                        //outsourceNames.Append('/');
                    }
                }
            }
            return list;
        }

        List<string> GetMaterialSelected()
        {
            List<string> list = new List<string>();
            foreach (ListItem li in cblMaterial.Items)
            {
                if (li.Selected)
                {
                    list.Add(li.Value.ToLower());
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
            cblMaterial.Items.Clear();
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
            var popGuidanceList = list.Where(s=>(s.guidance.AddType??1)==(int)GuidanceAddTypeEnum.POP).ToList();
            if (popGuidanceList.Any())
            {
                popGuidanceList = popGuidanceList.OrderBy(s => s.guidance.ItemName).ToList();

                popGuidanceList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    cblGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyGuidance.Visible = !popGuidanceList.Any();

            //道具活动
            cblPropGuidanceList.Items.Clear();
            cbAllPropGuidance.Checked = false;
            var propGuidanceList = list.Where(s => (s.guidance.AddType ?? 1) == (int)GuidanceAddTypeEnum.Prop).ToList();
            if (propGuidanceList.Any())
            {
                propGuidanceList = propGuidanceList.OrderBy(s => s.guidance.ItemName).ToList();

                propGuidanceList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    cblPropGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyPropGuidance.Visible = !propGuidanceList.Any();
        }

        

        void BindSubjectCategory()
        {
            cblSubjectCategory.Items.Clear();
           
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
                            where (subject.HandMakeSubjectId??0)==0
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

                subjectList0.OrderBy(s => s.SubjectName).ToList().ForEach(s =>
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
                   
                    cblSubjects.Items.Add(li);
                });
                //空的项目
                List<int> subjectIdAllList = subjectList0.Select(s => s.Id).ToList();
                List<Subject> emptyPOPSubjectList = subjectList.Where(s =>!subjectIdAllList.Contains(s.Id) && s.Region!=null && s.Region!="").ToList();
                if (emptyPOPSubjectList.Any())
                {
                    if (regionList.Any())
                    {
                        emptyPOPSubjectList = emptyPOPSubjectList.Where(s => regionList.Contains(s.Region)).ToList();
                    }

                    if (subjectCategoryIdList.Any())
                    {
                        if (subjectCategoryIdList.Contains(0))
                        {
                            subjectCategoryIdList.Remove(0);
                            if (subjectCategoryIdList.Any())
                            {
                                emptyPOPSubjectList = emptyPOPSubjectList.Where(s => subjectCategoryIdList.Contains(s.SubjectCategoryId ?? 0) || (s.SubjectCategoryId == null || s.SubjectCategoryId == 0)).ToList();
                            }
                            else
                                emptyPOPSubjectList = emptyPOPSubjectList.Where(s => (s.SubjectCategoryId == null || s.SubjectCategoryId == 0)).ToList();
                        }
                        else
                            emptyPOPSubjectList = emptyPOPSubjectList.Where(s => subjectCategoryIdList.Contains(s.SubjectCategoryId ?? 0)).ToList();
                    }
                    emptyPOPSubjectList.OrderBy(s => s.SubjectName).ToList().ForEach(s =>
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

                        cblSubjects.Items.Add(li);
                    });
                }
            }
            
        }

        void BindPropSubject()
        {
            cblPropSubjects.Items.Clear();
            cbAllProp.Checked = false;
            List<Subject> propSubjectList = new List<Subject>();
            if (Session["OutsourceStatisticSubjectList"] != null)
            {
                propSubjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
            }
            if (propSubjectList.Any())
            {
                propSubjectList.OrderBy(s => s.SubjectName).ToList().ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string subjectName = s.SubjectName;
                    li.Text = subjectName + "&nbsp;&nbsp;";
                    li.Value = s.Id.ToString();
                    cblPropSubjects.Items.Add(li);
                });
            }
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

            
            List<Subject> subjectList = new List<Subject>();
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            if (Session["OutsourceStatisticOrderList"] != null)
            {
                orderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
            }
            var outsourceOrderList = (from order in orderList
                                      join subject1 in subjectList
                                      on order.SubjectId equals subject1.Id into subjectTemp
                                      join company in CurrentContext.DbContext.Company
                                      on order.OutsourceId equals company.Id
                                      from subject in subjectTemp.DefaultIfEmpty()
                                      where guidanceIdList.Contains(order.GuidanceId ?? 0)
                                      && (order.IsDelete == null || order.IsDelete == false)
                                      select new
                                      {
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
                    Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                    List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                    subjectIdList.AddRange(hMSubjectIdList);
                    outsourceOrderList = outsourceOrderList.Where(s => (s.subject != null && subjectIdList.Contains(s.subject.Id)) || subjectIdList.Contains(s.order.BelongSubjectId??0)).ToList();
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

        void BindPropOutsource() 
        {
            List<PropOutsourceOrderDetail> orderList = new List<PropOutsourceOrderDetail>();
            if (Session["OutsourceStatisticPropOrderList"] != null)
            {
                orderList = Session["OutsourceStatisticPropOrderList"] as List<PropOutsourceOrderDetail>;
            }
            if (orderList.Any())
            { 
               
            }
        }

        void BindMaterial()
        {
            cblMaterial.Items.Clear();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            List<string> cityList = GetCitySelected();
            List<int> subjectIdList = GetSubjectSelected();
            List<int> subjectOutsourceIdList = new List<int>();
            List<Subject> subjectList = new List<Subject>();
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            List<int> guidanceIdList = new List<int>();
            if (Session["OutsourceStatisticOrderList"] != null)
            {
                orderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
            }
            
            foreach (ListItem li in cblOutsourceId.Items)
            {
                if (li.Selected)
                {
                    subjectOutsourceIdList.Add(int.Parse(li.Value));
                   
                }
            }
            if (orderList.Any())
            {
                guidanceIdList = subjectList.Select(s => s.GuidanceId ?? 0).Distinct().ToList();
                var list = (from order in orderList
                            join subject in subjectList
                            on order.SubjectId equals subject.Id
                            where subjectOutsourceIdList.Contains(order.OutsourceId??0)
                            &&order.GraphicMaterial!=null
                            && order.GraphicMaterial != "" 
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
                if (subjectIdList.Any())
                {
                    Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                    List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                    subjectIdList.AddRange(hMSubjectIdList);
                    list = list.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                }
                if (list.Any())
                {
                    List<string> materialList = list.Select(s=>s.order.GraphicMaterial.ToLower()).Distinct().OrderBy(s=>s).ToList();
                    if (materialList.Any())
                    {
                        materialList.ForEach(s =>
                        {
                            ListItem li = new ListItem();
                            li.Text = s + "&nbsp;&nbsp;";
                            li.Value = s;
                            cblMaterial.Items.Add(li);

                        });
                    }
                }
            }

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
            List<string> materialList = GetMaterialSelected();
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
                        if (materialList.Any())
                        {
                            orderList = orderList.Where(s =>s.order.GraphicMaterial!=null && materialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
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
                            var expressOrderPriceList = orderList0.Where(s => shopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)).ToList();
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
                                    if (s.order.OrderType == (int)OrderTypeEnum.其他费用 || s.order.OrderType == (int)OrderTypeEnum.印刷费)
                                    {
                                        otherPrice += ((s.order.PayOrderPrice ?? 0)*(s.order.Quantity??1));
                                        receiveOtherPrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                    }
                                    if (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)
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

        void SearchNew()
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

            bool selectedSubject = false;
            selectedSubject = subjectIdList.Any();
            foreach (ListItem li in cblOutsourceType.Items)
            {
                if (li.Selected)
                {
                    assignTypeList.Add(int.Parse(li.Value));
                }
            }
            string guidanceMonth = string.Empty;
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text))
            {
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
                      
                        var orderList0 = (from order in totalOrderList
                                          where subjectOutsourceIdList.Contains(order.OutsourceId ?? 0)
                                          && gid == order.GuidanceId
                                          //&& (order.IsDelete == null || order.IsDelete == false)
                                          //&& ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType > 1)
                                          select new
                                          {
                                              order

                                          }).ToList();

                        //var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.OutsourceId == outsourceId && guidanceIdList.Contains(s.GuidanceId ?? 0));
                        var popOrderList = orderList0.Where(s=>(s.order.SubjectId??0)>0).ToList();
                        //活动安装费
                        var activityInstallPriceList = orderList0.Where(s => (s.order.SubjectId ?? 0) == 0 && s.order.OrderType==(int)OrderTypeEnum.安装费).ToList();

                        var expressOrderPriceList = orderList0.Where(s => (s.order.SubjectId ?? 0) == 0 && (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)).ToList();
                        List<int> allSubjectIdList = new List<int>();   
                        if (!subjectIdList.Any())
                        {
                            List<Subject> subjectListTemp = subjectList;
                            if (subjectCategoryIdList.Any())
                            {
                                if (subjectCategoryIdList.Contains(0))
                                {
                                    subjectCategoryIdList.Remove(0);
                                    if (subjectCategoryIdList.Any())
                                    {
                                        subjectListTemp = subjectListTemp.Where(s => subjectCategoryIdList.Contains(s.SubjectCategoryId ?? 0) || (s.SubjectCategoryId == null || s.SubjectCategoryId == 0)).ToList();
                                    }
                                    else
                                        subjectListTemp = subjectListTemp.Where(s => s.SubjectCategoryId == null || s.SubjectCategoryId == 0).ToList();
                                }
                                else
                                    subjectListTemp = subjectListTemp.Where(s => subjectCategoryIdList.Contains(s.SubjectCategoryId ?? 0)).ToList();
                            }
                            subjectIdList = subjectListTemp.Select(s => s.Id).Distinct().ToList();

                        }
                        
                        if (regionList.Any())
                        {
                            popOrderList = popOrderList.Where(s => s.order.Region != null && regionList.Contains(s.order.Region)).ToList();
                            activityInstallPriceList = activityInstallPriceList.Where(s => s.order.Region != null && regionList.Contains(s.order.Region)).ToList();
                            expressOrderPriceList = expressOrderPriceList.Where(s => s.order.Region != null && regionList.Contains(s.order.Region)).ToList();
                        }
                        if (provinceList.Any())
                        {
                            popOrderList = popOrderList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                            activityInstallPriceList = activityInstallPriceList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                            expressOrderPriceList = expressOrderPriceList.Where(s => provinceList.Contains(s.order.Province)).ToList();
                        }
                        if (cityList.Any())
                        {
                            popOrderList = popOrderList.Where(s => cityList.Contains(s.order.City)).ToList();
                            activityInstallPriceList = activityInstallPriceList.Where(s => cityList.Contains(s.order.City)).ToList();
                            expressOrderPriceList = expressOrderPriceList.Where(s => cityList.Contains(s.order.City)).ToList();
                        }
                        if (assignTypeList.Any())
                        {
                            popOrderList = popOrderList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                            activityInstallPriceList = activityInstallPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                            expressOrderPriceList = expressOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                        }
                        List<int> shopIdList = new List<int>();
                        if (subjectIdList.Any())
                        {
                            //百丽
                            Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                            List<int> hMSubjectIdList = new SubjectBLL().GetList(s => s.GuidanceId == gid && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                            subjectIdList.AddRange(hMSubjectIdList);

                            popOrderList = popOrderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                            if (selectedSubject && activityInstallPriceList.Where(s => (s.order.BelongSubjectId ?? 0) > 0).Any())
                            {
                                activityInstallPriceList = activityInstallPriceList.Where(s => subjectIdList.Contains(s.order.BelongSubjectId ?? 0)).ToList();
                            }
                            //else
                            //{
                            //    shopIdList = popOrderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                            //    if (shopIdList.Any())
                            //       activityInstallPriceList = activityInstallPriceList.Where(s => shopIdList.Contains(s.order.ShopId ?? 0)).ToList();
                            //}

                        }
                        if (popOrderList.Any() || activityInstallPriceList.Any() || expressOrderPriceList.Any())
                        {
                            if (!shopIdList.Any())
                               shopIdList = popOrderList.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                            //List<int> shopIdList0 = activityInstallPriceList.Where(s => !shopIdList.Contains(s.order.ShopId??0)).Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                            TotalShopCountList.AddRange(shopIdList);
                            //TotalShopCountList.AddRange(shopIdList0);

                            //安装费
                            //var installOrderPriceList = orderList0.Where(s => installShopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                            if (activityInstallPriceList.Any())
                            {
                                installPrice += activityInstallPriceList.Sum(s => s.order.PayOrderPrice ?? 0);
                                receiveinstallPrice += activityInstallPriceList.Sum(s => s.order.ReceiveOrderPrice ?? 0);
                            }
                            //快递费
                            //var expressOrderPriceList = orderList0.Where(s => shopIdList.Contains(s.order.ShopId ?? 0) && s.order.SubjectId == 0 && (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)).ToList();
                            
                            if (expressOrderPriceList.Any())
                            {
                                //if (assignTypeList.Any())
                                //{
                                //    expressOrderPriceList = expressOrderPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                                //}
                                expressPrice += expressOrderPriceList.Sum(s => s.order.PayOrderPrice ?? 0);
                                receiveExpressPrice += expressOrderPriceList.Sum(s => s.order.ReceiveOrderPrice ?? 0);
                            }
                            
                            if (popOrderList.Any())
                            {
                                decimal installPrice1 = 0;
                                decimal receiveInstallPrice1 = 0;
                                //popOrderList = popOrderList.Where(s => s.order.SubjectId > 0).ToList();
                                popOrderList.ForEach(s =>
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
                                    if (s.order.OrderType == (int)OrderTypeEnum.其他费用 || s.order.OrderType == (int)OrderTypeEnum.印刷费)
                                    {
                                        otherPrice += ((s.order.PayOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                        receiveOtherPrice += ((s.order.ReceiveOrderPrice ?? 0) * (s.order.Quantity ?? 1));
                                    }
                                    if (s.order.OrderType == (int)OrderTypeEnum.发货费 || s.order.OrderType == (int)OrderTypeEnum.快递费 || s.order.OrderType == (int)OrderTypeEnum.运费)
                                    {
                                        expressPrice += (s.order.PayOrderPrice ?? 0);
                                        receiveExpressPrice += (s.order.ReceiveOrderPrice ?? 0);
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
                    #endregion
                    #region 赋值
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
            ChangeGuidanceSelected();
        }

        protected void cblPropGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangePropGuidanceSelected();
        }

        void ChangeGuidanceSelected()
        {
            LoadData();
            BindSubjectCategory();
            BindRegion();
            BindSubject();
            BindOutSource();
        }

        void ChangePropGuidanceSelected()
        {
            LoadPropData();
            BindPropSubject();
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
            //Search();
            SearchNew();
        }

        protected void cblSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindOutSource();
        }

        protected void cblPropSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindOutSource();
        }

        protected void cblOutspurce_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindMaterial();
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

        //protected void cbAllGuidance_CheckedChanged(object sender, EventArgs e)
        //{
        //    ChangeGuidanceSelected();
        //}

        //protected void cbAllGuidance_CheckedChanged(object sender, EventArgs e)
        //{
        //    ChangeGuidanceSelected();
        //}

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<int> guidanceIdList = GetGuidanceSelected();
            List<int> subjectCategoryIdList = GetSubjectCategorySelected();
            List<string> regionList = GetRegionSelected();
            List<string> provinceList = GetProvinceSelected();
            List<string> cityList = GetCitySelected();
            List<int> subjectIdList = GetSubjectSelected();
            List<int> subjectOutsourceIdList = GetOutSourceSelected();
            List<int> assignTypeList = new List<int>();
            foreach (ListItem li in cblOutsourceType.Items)
            {
                if (li.Selected)
                {
                    assignTypeList.Add(int.Parse(li.Value));
                }
            }
            string guidanceMonth = string.Empty;
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text))
            {
                guidanceMonth = txtGuidanceMonth.Text.Trim();
            }
            if (subjectOutsourceIdList.Any() && guidanceIdList.Any())
            {
                List<OutsourceOrderDetail> totalOrderList = new List<OutsourceOrderDetail>();
                List<Subject> subjectList = new List<Subject>();
                List<Shop> shopList = new List<Shop>();
                List<SubjectGuidance> guidanceList = new List<SubjectGuidance>();
                List<Company> outsourceList = new CompanyBLL().GetList(s => subjectOutsourceIdList.Contains(s.Id));
                if (Session["OutsourceStatisticOrderList"] != null)
                {
                    totalOrderList = Session["OutsourceStatisticOrderList"] as List<OutsourceOrderDetail>;
                    subjectList = Session["OutsourceStatisticSubjectList"] as List<Subject>;
                    shopList = Session["OutsourceStatisticShopList"] as List<Shop>;
                    guidanceList = Session["OutsourceStatisticGuidanceList"] as List<SubjectGuidance>;
                }
                if (totalOrderList.Any())
                {
                    List<OrderPriceDetail> newOrderList = new List<OrderPriceDetail>();
                    #region
                    guidanceIdList.ForEach(gid => {
                        //是否全部三叶草
                        bool isBCSSubject = false;

                        var orderList0 = (from order in totalOrderList
                                          join guidance in guidanceList
                                          on order.GuidanceId equals guidance.ItemId
                                          join outsource in outsourceList
                                          on order.OutsourceId equals outsource.Id
                                          join subject1 in subjectList
                                          on order.SubjectId equals subject1.Id into subjectTemp
                                          from subject in subjectTemp.DefaultIfEmpty()
                                          where subjectOutsourceIdList.Contains(order.OutsourceId ?? 0)
                                          && order.GuidanceId == gid
                                          && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType > 1)
                                          && (order.IsDelete == null || order.IsDelete == false)
                                          select new
                                          {
                                              order,
                                              subject,
                                              guidance,
                                              outsource
                                          }).ToList();
                        var orderList = orderList0;
                        if (subjectIdList.Any())
                        {
                            //百丽
                            Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                            List<int> hMSubjectIdList = new SubjectBLL().GetList(s =>s.GuidanceId==gid && subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                            subjectIdList.AddRange(hMSubjectIdList);

                            orderList = orderList.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                            var NotBCSSubjectList = new SubjectBLL().GetList(s => subjectIdList.Contains(s.Id) && (s.CornerType == null || !s.CornerType.Contains("三叶草")));
                            isBCSSubject = !NotBCSSubjectList.Any();
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


                            OrderPriceDetail orderModel;

                            orderList = orderList.Where(s => s.order.SubjectId > 0).ToList();
                            orderList.ForEach(s =>
                            {
                                string gender = s.order.Gender;
                                string orderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                                string orderPrice = string.Empty;
                                int Quantity = s.order.Quantity ?? 1;

                                decimal width = s.order.GraphicWidth ?? 0;
                                decimal length = s.order.GraphicLength ?? 0;
                                decimal totalArea = (width * length) / 1000000 * Quantity;
                                orderModel = new OrderPriceDetail();
                                if (s.subject != null && s.subject.AddDate != null)
                                {
                                    orderModel.AddDate = s.subject.AddDate;
                                }
                                orderModel.PriceType = orderType;
                                orderModel.Area = double.Parse(totalArea.ToString());
                                orderModel.Gender = s.order.Gender;
                                orderModel.GraphicLength = double.Parse(length.ToString());
                                orderModel.GraphicWidth = double.Parse(width.ToString());
                                orderModel.GraphicMaterial = s.order.GraphicMaterial;
                                orderModel.PositionDescription = s.order.PositionDescription;
                                orderModel.Quantity = Quantity;
                                orderModel.Sheet = s.order.Sheet;
                                orderModel.ShopName = s.order.ShopName;
                                orderModel.ShopNo = s.order.ShopNo;
                                orderModel.SubjectName = s.subject.SubjectName;
                                orderModel.SubjectNo = s.subject.SubjectNo;
                                if ((s.order.PayOrderPrice ?? 0) > 0)
                                {
                                    //orderPrice = (s.order.OrderPrice ?? 0).ToString();
                                    orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                                    orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                                    orderModel.Gender = string.Empty;
                                    orderModel.Sheet = string.Empty;
                                    orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                                    orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                                }
                                else
                                {
                                    orderModel.TotalPrice = double.Parse((s.order.TotalPrice ?? 0).ToString());
                                    orderModel.UnitPrice = double.Parse((s.order.UnitPrice ?? 0).ToString());
                                    orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveUnitPrice ?? 0).ToString());
                                    orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveTotalPrice ?? 0).ToString());
                                }
                                orderModel.Remark = s.order.Remark;
                                orderModel.GuidanceName = s.guidance.ItemName;
                                orderModel.OutsourceName = s.outsource.CompanyName;
                                newOrderList.Add(orderModel);

                            });
                            var assignShopList = (from assign in CurrentContext.DbContext.OutsourceAssignShop
                                                  join shop in CurrentContext.DbContext.Shop
                                                  on assign.ShopId equals shop.Id
                                                  where assign.GuidanceId == gid
                                                  && shopIdList.Contains(assign.ShopId ?? 0)
                                                  && subjectOutsourceIdList.Contains(assign.OutsourceId ?? 0)
                                                  && ((assign.PayInstallPrice ?? 0) > 0 || (assign.PayExpressPrice ?? 0) > 0)
                                                  select new
                                                  {
                                                      assign,
                                                      shop
                                                  }).ToList();
                            if (assignShopList.Any())
                            {
                                assignShopList.ForEach(s =>
                                {
                                    if ((s.assign.PayInstallPrice ?? 0) > 0)
                                    {
                                        orderModel = new OrderPriceDetail();
                                        orderModel.AddDate = null;
                                        orderModel.Area = 0;
                                        orderModel.Gender = string.Empty;
                                        orderModel.GraphicLength = 0;
                                        orderModel.GraphicWidth = 0;
                                        orderModel.GraphicMaterial = string.Empty;
                                        orderModel.PositionDescription = string.Empty;
                                        orderModel.Quantity = 1;
                                        orderModel.Sheet = string.Empty;
                                        orderModel.ShopName = s.shop.ShopName;
                                        orderModel.ShopNo = s.shop.ShopNo;
                                        orderModel.SubjectName = string.Empty;
                                        orderModel.SubjectNo = string.Empty;
                                        orderModel.UnitPrice = 0;


                                        orderModel.PriceType = "安装费";
                                        orderModel.TotalPrice = double.Parse((s.assign.PayInstallPrice ?? 0).ToString());
                                        orderModel.UnitPrice = double.Parse((s.assign.PayInstallPrice ?? 0).ToString());
                                        orderModel.ReceiveUnitPrice = double.Parse((s.assign.ReceiveInstallPrice ?? 0).ToString());
                                        orderModel.ReceiveTotalPrice = double.Parse((s.assign.ReceiveInstallPrice ?? 0).ToString());
                                        
                                        newOrderList.Add(orderModel);
                                    }
                                    if ((s.assign.PayExpressPrice ?? 0) > 0)
                                    {
                                        orderModel = new OrderPriceDetail();
                                        orderModel.AddDate = null;
                                        orderModel.Area = 0;
                                        orderModel.Gender = string.Empty;
                                        orderModel.GraphicLength = 0;
                                        orderModel.GraphicWidth = 0;
                                        orderModel.GraphicMaterial = string.Empty;
                                        orderModel.PositionDescription = string.Empty;
                                        orderModel.Quantity = 1;
                                        orderModel.Sheet = string.Empty;
                                        orderModel.ShopName = s.shop.ShopName;
                                        orderModel.ShopNo = s.shop.ShopNo;
                                        orderModel.SubjectName = string.Empty;
                                        orderModel.SubjectNo = string.Empty;
                                        orderModel.UnitPrice = 0;
                                        orderModel.PriceType = "发货费";
                                        orderModel.TotalPrice = double.Parse((s.assign.PayExpressPrice ?? 0).ToString());
                                        orderModel.UnitPrice = double.Parse((s.assign.PayExpressPrice ?? 0).ToString());
                                        orderModel.ReceiveUnitPrice = double.Parse((s.assign.ReceiveExpresslPrice ?? 0).ToString());
                                        orderModel.ReceiveTotalPrice = double.Parse((s.assign.ReceiveExpresslPrice ?? 0).ToString());
                                        newOrderList.Add(orderModel);
                                    }

                                });
                            }

                            var installPriceList = orderList0.Where(s => s.order.SubjectId == 0 && installShopIdList.Contains(s.order.ShopId ?? 0) && s.order.OrderType == (int)OrderTypeEnum.安装费).ToList();
                            if (assignTypeList.Any())
                            {
                                installPriceList = installPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                            }
                            if (installPriceList.Any())
                            {
                                installPriceList.ForEach(s =>
                                {
                                    orderModel = new OrderPriceDetail();
                                    orderModel.AddDate = null;
                                    orderModel.Area = 0;
                                    orderModel.Gender = string.Empty;
                                    orderModel.GraphicLength = 0;
                                    orderModel.GraphicWidth = 0;
                                    orderModel.GraphicMaterial = string.Empty;
                                    orderModel.PositionDescription = string.Empty;
                                    orderModel.Quantity = 1;
                                    orderModel.Sheet = string.Empty;
                                    orderModel.ShopName = s.order.ShopName;
                                    orderModel.ShopNo = s.order.ShopNo;
                                    orderModel.SubjectName = string.Empty;
                                    orderModel.SubjectNo = string.Empty;
                                    orderModel.UnitPrice = 0;
                                    orderModel.PriceType = "安装费";
                                    orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                                    orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                                    orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                                    orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                                    orderModel.GuidanceName = s.guidance.ItemName;
                                    orderModel.OutsourceName = s.outsource.CompanyName;
                                    newOrderList.Add(orderModel);
                                });
                            }
                            //var expressPriceList = new OutsourceOrderDetailBLL().GetList(s => s.GuidanceId == gid && outsourceList.Contains(s.OutsourceId ?? 0) && s.SubjectId == 0 && shopIdList.Contains(s.ShopId ?? 0) && s.OrderType == (int)OrderTypeEnum.发货费).ToList();
                            var expressPriceList = orderList0.Where(s => s.order.SubjectId == 0 && shopIdList.Contains(s.order.ShopId ?? 0) && s.order.OrderType == (int)OrderTypeEnum.发货费).ToList();
                            if (assignTypeList.Any())
                            {
                                expressPriceList = expressPriceList.Where(s => assignTypeList.Contains(s.order.AssignType ?? 0)).ToList();
                            }
                            if (expressPriceList.Any())
                            {
                                expressPriceList.ForEach(s =>
                                {
                                    orderModel = new OrderPriceDetail();
                                    orderModel.AddDate = null;
                                    orderModel.Area = 0;
                                    orderModel.Gender = string.Empty;
                                    orderModel.GraphicLength = 0;
                                    orderModel.GraphicWidth = 0;
                                    orderModel.GraphicMaterial = string.Empty;
                                    orderModel.PositionDescription = string.Empty;
                                    orderModel.Quantity = 1;
                                    orderModel.Sheet = string.Empty;
                                    orderModel.ShopName = s.order.ShopName;
                                    orderModel.ShopNo = s.order.ShopNo;
                                    orderModel.SubjectName = string.Empty;
                                    orderModel.SubjectNo = string.Empty;
                                    orderModel.UnitPrice = 0;
                                    orderModel.PriceType = "发货费";
                                    orderModel.TotalPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                                    orderModel.UnitPrice = double.Parse((s.order.PayOrderPrice ?? 0).ToString());
                                    orderModel.ReceiveUnitPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                                    orderModel.ReceiveTotalPrice = double.Parse((s.order.ReceiveOrderPrice ?? 0).ToString());
                                    orderModel.GuidanceName = s.guidance.ItemName;
                                    orderModel.OutsourceName = s.outsource.CompanyName;
                                    newOrderList.Add(orderModel);
                                });
                            }


                        }
                    });
                    #endregion
                    if (newOrderList.Any())
                    {
                        string templateFileName = "外协项目费用统计明细";
                       
                        string path = ConfigurationManager.AppSettings["ExportTemplate"];
                        path = path.Replace("fileName", templateFileName);
                        FileStream outFile = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                        IWorkbook workBook = WorkbookFactory.Create(outFile, ImportOption.All);
                        ISheet sheet = workBook.GetSheet("Sheet1");
                        int startRow = 1;
                        newOrderList.OrderBy(s => s.ShopNo).ToList().ForEach(s =>
                        {
                            IRow dataRow = sheet.GetRow(startRow);
                            if (dataRow == null)
                                dataRow = sheet.CreateRow(startRow);
                            for (int i = 0; i < 21; i++)
                            {
                                ICell cell = dataRow.GetCell(i);
                                if (cell == null)
                                    cell = dataRow.CreateCell(i);

                            }
                            dataRow.GetCell(0).SetCellValue(s.PriceType);

                            dataRow.GetCell(1).SetCellValue(s.SubjectNo);
                            if (s.AddDate != null)
                                dataRow.GetCell(2).SetCellValue(DateTime.Parse(s.AddDate.ToString()).ToShortDateString());
                            else
                                dataRow.GetCell(2).SetCellValue("");
                            dataRow.GetCell(3).SetCellValue(s.ShopNo);
                            dataRow.GetCell(4).SetCellValue(s.ShopName);
                            dataRow.GetCell(5).SetCellValue(s.Sheet);
                            dataRow.GetCell(6).SetCellValue(s.PositionDescription);
                            dataRow.GetCell(7).SetCellValue(s.Gender);
                            dataRow.GetCell(8).SetCellValue(s.GraphicWidth);
                            dataRow.GetCell(9).SetCellValue(s.GraphicLength);
                            dataRow.GetCell(10).SetCellValue(s.Area);
                            dataRow.GetCell(11).SetCellValue(s.GraphicMaterial);
                            dataRow.GetCell(12).SetCellValue(s.UnitPrice);
                            dataRow.GetCell(13).SetCellValue(s.ReceiveUnitPrice);
                            dataRow.GetCell(14).SetCellValue(s.Quantity);
                            dataRow.GetCell(15).SetCellValue(s.TotalPrice);
                            dataRow.GetCell(16).SetCellValue(s.ReceiveTotalPrice);
                            dataRow.GetCell(17).SetCellValue(s.GuidanceName);
                            dataRow.GetCell(18).SetCellValue(s.SubjectName);
                            dataRow.GetCell(19).SetCellValue(s.Remark);
                            dataRow.GetCell(20).SetCellValue(s.OutsourceName);
                            startRow++;
                        });
                        using (MemoryStream ms = new MemoryStream())
                        {
                            workBook.Write(ms);
                            ms.Flush();
                            sheet = null;
                            workBook = null;
                            StringBuilder fileName = new StringBuilder("外协费用统计明细");
                            //if (outsourceNames.Length>0)
                            //{
                            //    fileName.Append("-");
                            //    fileName.AppendFormat("({0})", outsourceNames.ToString().TrimEnd('/'));
                            //}
                            OperateFile.DownLoadFile(ms, fileName.ToString());

                        }
                    }

                   
                }
            }
        }
    }
}
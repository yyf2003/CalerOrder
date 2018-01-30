using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using DAL;
using Common;
using System.Configuration;
using System.Transactions;

namespace WebApp.Subjects.InstallPrice
{
    public partial class SubmitInstallPrice : BasePage
    {

        public int guidanceId;
        SubjectBLL subjectBll = new SubjectBLL();
        bool isRegionOrder = false;
        List<string> myRegionList = new List<string>();
        List<string> cityCierList = new List<string>() { "T1", "T2", "T3" };
        List<int> myInstallShopIdList = new List<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemid"] != null)
                guidanceId = int.Parse(Request.QueryString["itemid"]);
            if (CurrentUser.RoleId == 6)
            {
                isRegionOrder = true;
                myRegionList = GetResponsibleRegion;
                StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
                labTitle.Text = "—分区订单";
            }
            if (!IsPostBack)
            {
                Session["orderDetailInstallPrice"] = null;
                Session["shopInstallPrice"] = null;
                Session["subjectInstallPrice"] = null;
                Session["assginShopIdInstallPrice"] = null;
                BindGuidance();
                BindRegion();
                BindSubjectType();
                BindSubject();
                BindPOSScale();
                GetShopCount();
                BindSubjectDDL();
                BindData();

            }
        }



        void BindData()
        {
            var list = (from detail in CurrentContext.DbContext.InstallPriceDetail
                        join subject in CurrentContext.DbContext.Subject
                        on detail.SubjectId equals subject.Id
                        where detail.GuidanceId == guidanceId
                        && detail.AddType == 1
                        select new
                        {
                            detail,
                            detail.SelectSubjectTypeId,
                            subject.SubjectName,
                            detail.Id
                        }).ToList();
           
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            RepeaterList.DataSource = list.OrderBy(s => s.detail.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            RepeaterList.DataBind();
        }

        void BindGuidance()
        {

            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(guidanceId);
            if (model != null)
            {
                labGuidanceName.Text = model.ItemName;
                labSecondGuidanceName.Text = model.ItemName;
                //if (model.ItemName.Contains("二次") || model.ItemName.Contains("2次"))
                //{
                //    cbIsSecondInstall.Checked = true;
                //}
                List<FinalOrderDetailTemp> finalOrderDetailTempList = new FinalOrderDetailTempBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength != null && s.GraphicLength > 0 && s.GraphicWidth != null && s.GraphicWidth > 0) || (s.OrderType == (int)OrderTypeEnum.道具)));

                var orderList = (from order in finalOrderDetailTempList
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 where (subject.IsDelete == null || subject.IsDelete == false)
                                 && subject.ApproveState == 1
                                 && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                 && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                                 && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                                 && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && order.BCSIsInstall != null && order.BCSIsInstall == "Y"))

                                 select new { order, shop, subject }).ToList();
                if (orderList.Any())
                {
                    Session["orderDetailInstallPrice"] = orderList.Select(s => s.order).ToList();
                    Session["shopInstallPrice"] = orderList.Select(s => s.shop).Distinct().ToList();
                    Session["subjectInstallPrice"] = orderList.Select(s => s.subject).Distinct().ToList();

                }
                List<int> assginShopIdList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId && (s.AddType==null || s.AddType==1)).Select(s => s.ShopId ?? 0).Distinct().ToList();
                if (assginShopIdList.Any())
                {
                    Session["assginShopIdInstallPrice"] = assginShopIdList;
                }
            }
        }

        void BindRegion()
        {
            cblRegion.Items.Clear();
            cblSecondRegion.Items.Clear();
            
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;

            }
           
            List<Subject> subjectList = new List<Subject>();
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            List<string> regionList1 = new List<string>();
            if (orderList.Any())
            {
                regionList1 = orderList.Select(s => s.Region).Distinct().ToList();
            }
            if (Session["assginShopIdInstallPrice"] != null)
            {
                List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
                if (assginShopIdList.Any())
                {
                    orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId??0)).ToList();
                }
            }
            var orderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              select new
                              {
                                  Region = (subject.PriceBlongRegion!=null&&subject.PriceBlongRegion!="")?subject.PriceBlongRegion:order.Region
                              }).ToList();
            int totalShopCount = orderList.Select(s => s.ShopId ?? 0).Distinct().Count();
            labTotalShopCount.Text = totalShopCount.ToString();
            List<string> regionList = orderList0.Select(s => s.Region).Distinct().ToList();
            regionList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s;
                li.Text = s + "&nbsp;&nbsp;";
                cblRegion.Items.Add(li);

            });
            regionList1.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s;
                li.Text = s + "&nbsp;&nbsp;";
                cblSecondRegion.Items.Add(li);

            });
        }

        void BindSubjectType()
        {
            cblSubjectType.Items.Clear();
           
            List<string> regions = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (orderList.Any())
            {
                if (Session["assginShopIdInstallPrice"] != null)
                {
                    List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
                    if (assginShopIdList.Any())
                    {
                        orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }
                }
            }
            var orderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              join subjectType1 in CurrentContext.DbContext.SubjectType
                             on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                              from subjectType in typeTemp.DefaultIfEmpty()
                              where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                              select new
                              {
                                  order,
                                  subject.SubjectTypeId,
                                  SubjectTypeName = subjectType!=null?subjectType.SubjectTypeName:""
                              }).ToList();

            if (orderList0.Any())
            {
                var list = orderList0.Select(s => new { s.SubjectTypeId, s.SubjectTypeName }).Distinct().ToList();
                List<int> typeIdList = new List<int>();
                bool isEmpty = false;
                list.ForEach(s =>
                {
                    if (s.SubjectTypeId != null && s.SubjectTypeId != 0)
                    {
                        if (!typeIdList.Contains(s.SubjectTypeId ?? 0))
                        {
                            ListItem li = new ListItem();
                            li.Value = (s.SubjectTypeId ?? 0).ToString();
                            li.Text = s.SubjectTypeName + "&nbsp;&nbsp;";
                            cblSubjectType.Items.Add(li);
                        }
                    }
                    else
                        isEmpty = true;
                });
                if (isEmpty)
                {
                    ListItem li = new ListItem();
                    li.Value = "0";
                    li.Text = "空";
                    cblSubjectType.Items.Add(li);
                }

            }

        }

        void BindSubject()
        {
            cblSubjectName.Items.Clear();

           
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (orderList.Any())
            {
                if (Session["assginShopIdInstallPrice"] != null)
                {
                    List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
                    if (assginShopIdList.Any())
                    {
                        orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }
                }
            }
            if (orderList.Any())
            {
                List<string> regions = new List<string>();
                List<int> subjectTypeList = new List<int>();
                foreach (ListItem li in cblRegion.Items)
                {
                    if (li.Selected && !regions.Contains(li.Value.ToLower()))
                        regions.Add(li.Value.ToLower());
                }
                foreach (ListItem li in cblSubjectType.Items)
                {
                    if (li.Selected && !subjectTypeList.Contains(int.Parse(li.Value)))
                        subjectTypeList.Add(int.Parse(li.Value));
                }
                var orderList0 = (from order in orderList
                                  join subject in subjectList
                                  on order.SubjectId equals subject.Id
                                  join subjectType1 in CurrentContext.DbContext.SubjectType
                                 on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                                  from subjectType in typeTemp.DefaultIfEmpty()
                                  where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                                  select new
                                  {
                                      order,
                                      subject
                                  }).ToList();


                if (subjectTypeList.Any())
                {

                    if (subjectTypeList.Contains(0))
                    {
                        subjectTypeList.Remove(0);
                        if (subjectTypeList.Any())
                        {
                            orderList0 = orderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                        }
                        else
                            orderList0 = orderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                }

                if (orderList0.Any())
                {
                    var subjectList0 = orderList0.Select(s => s.subject).Distinct().OrderBy(s => s.SubjectName).ToList();
                    subjectList0.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectName + "&nbsp;&nbsp;";
                        cblSubjectName.Items.Add(li);
                    });



                }
            }


        }

        public void BindPOSScale()
        {
            cblPOSScale.Items.Clear();
            labShopCount.Text = "";

            //var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //                 join subject in CurrentContext.DbContext.Subject
            //                 on order.SubjectId equals subject.Id
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on order.ShopId equals shop.Id
            //                 where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
            //                 && subject.ApproveState == 1
            //                 && (order.IsDelete == null || order.IsDelete == false)
            //                && subject.SubjectType != (int)SubjectTypeEnum.二次安装
            //                && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
            //                 && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == (int)OrderTypeEnum.道具))
            //                 //&& (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
            //                 //&& (shop.Status == null || shop.Status == "" || shop.Status == "正常")
            //                 select new
            //                 {
            //                     subject,
            //                     // subjectType.SubjectTypeName,
            //                     // pop,
            //                     shop,
            //                     //order,
            //                     //ShopId=shop.Id,
            //                     order,
            //                     POSScale = order.InstallPricePOSScale
            //                 }).ToList();


            //List<int> assginShopList = new List<int>();
            //assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();

            //if (assginShopList.Any())
            //{
            //    orderList = orderList.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
            //}
            List<string> regions = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }
            List<int> subjectTypeList = new List<int>();
            foreach (ListItem li in cblSubjectType.Items)
            {
                if (li.Selected && !subjectTypeList.Contains(int.Parse(li.Value)))
                    subjectTypeList.Add(int.Parse(li.Value));
            }
            List<int> subjectIdList = new List<int>();
            foreach (ListItem li in cblSubjectName.Items)
            {
                if (li.Selected && !subjectIdList.Contains(int.Parse(li.Value)))
                    subjectIdList.Add(int.Parse(li.Value));
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (orderList.Any())
            {
                if (Session["assginShopIdInstallPrice"] != null)
                {
                    List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
                    if (assginShopIdList.Any())
                    {
                        orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }
                }
            }
            var orderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              join subjectType1 in CurrentContext.DbContext.SubjectType
                             on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                              from subjectType in typeTemp.DefaultIfEmpty()
                              where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                              select new
                              {
                                  order,
                                  subject
                              }).ToList();

            if (subjectTypeList.Any())
            {

                if (subjectTypeList.Contains(0))
                {
                    subjectTypeList.Remove(0);
                    if (subjectTypeList.Any())
                    {
                        orderList0 = orderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                }
                else
                    orderList0 = orderList0.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
            }
            if (subjectIdList.Any())
            {
                //orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId??0)).ToList();
                orderList0 = orderList0.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
            }

            if (orderList0.Any())
            {


                List<string> POSScaleList = orderList0.Select(s => s.order.InstallPricePOSScale).Distinct().ToList();


                //var notEmptyList = orderList.Where(s => s.POSScale != null && s.POSScale != "").ToList();
                //var EmptyList = orderList.Where(s => s.POSScale == null || s.POSScale == "").ToList();
                //List<int> notEmptyShopIdList = notEmptyList.Select(s => s.shop.Id).Distinct().ToList();
                //EmptyList = EmptyList.Where(s => !notEmptyShopIdList.Contains(s.shop.Id)).ToList();
                //POSScaleList = notEmptyList.Select(s => s.POSScale).Distinct().OrderBy(s => s).ToList();
                bool isEmpty = false;
                POSScaleList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        ListItem li = new ListItem();
                        li.Value = s;
                        li.Text = s + "&nbsp;&nbsp;";
                        cblPOSScale.Items.Add(li);
                    }
                    else
                        isEmpty = true;
                });
                if (isEmpty)
                {
                    cblPOSScale.Items.Add(new ListItem("空", "空"));
                }
            }


        }

        void GetShopCount()
        {
            labShopCount.Text = "";

            //var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //                 join subject in CurrentContext.DbContext.Subject
            //                 on order.SubjectId equals subject.Id
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on order.ShopId equals shop.Id
            //                 //join subjectType1 in CurrentContext.DbContext.SubjectType
            //                 //on subject.SubjectTypeId equals subjectType1.Id into typeTemp
            //                 //from subjectType in typeTemp.DefaultIfEmpty()
            //                 where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
            //                 && subject.ApproveState == 1
            //                 && (order.IsDelete == null || order.IsDelete == false)
            //                 && subject.SubjectType != (int)SubjectTypeEnum.二次安装
            //                && ((order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
            //                && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == (int)OrderTypeEnum.道具))
            //                 //&& (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
            //                 select new
            //                 {
            //                     subject,
            //                     order,
            //                     shop,
            //                     POSScale = order.InstallPricePOSScale
            //                 }).ToList();


            //List<int> assginShopList = new List<int>();
            //assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();

            //if (assginShopList.Any())
            //{
            //    orderList = orderList.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
            //}
            List<string> regions = new List<string>();

            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }

            List<int> subjectTypeList = new List<int>();
            foreach (ListItem li in cblSubjectType.Items)
            {
                if (li.Selected && !subjectTypeList.Contains(int.Parse(li.Value)))
                    subjectTypeList.Add(int.Parse(li.Value));
            }
            List<int> subjectIdList = new List<int>();
            foreach (ListItem li in cblSubjectName.Items)
            {
                if (li.Selected && !subjectIdList.Contains(int.Parse(li.Value)))
                    subjectIdList.Add(int.Parse(li.Value));
            }
            List<string> posScaleList = new List<string>();
            foreach (ListItem li in cblPOSScale.Items)
            {
                if (li.Selected && !posScaleList.Contains(li.Value))
                    posScaleList.Add(li.Value);
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (orderList.Any())
            {
                if (Session["assginShopIdInstallPrice"] != null)
                {
                    List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
                    if (assginShopIdList.Any())
                    {
                        orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }
                }
            }
            var orderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              join subjectType1 in CurrentContext.DbContext.SubjectType
                             on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                              from subjectType in typeTemp.DefaultIfEmpty()
                              where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                              select new
                              {
                                  order,
                                  subject
                              }).ToList();

            if (subjectTypeList.Any())
            {

                if (subjectTypeList.Contains(0))
                {
                    subjectTypeList.Remove(0);
                    if (subjectTypeList.Any())
                    {
                        orderList0 = orderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                }
                else
                    orderList0 = orderList0.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
            }
            if (subjectIdList.Any())
            {
                //orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId??0)).ToList();
                orderList0 = orderList0.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
            }
            if (posScaleList.Any())
            {
                if (posScaleList.Contains("空"))
                {
                    posScaleList.Remove("空");
                    if (posScaleList.Any())
                    {
                        orderList0 = orderList0.Where(s => posScaleList.Contains(s.order.InstallPricePOSScale) || s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                }
                else
                    orderList0 = orderList0.Where(s => posScaleList.Contains(s.order.InstallPricePOSScale)).ToList();
            }
            if (orderList0.Any())
            {
                List<int> shopIdList = orderList0.Select(s => s.order.ShopId ?? 0).Distinct().ToList();
                int shopCount = shopIdList.Count;
                string text = "<span name='spanCheckLeftShop' style=' color:Blue; cursor:pointer; text-decoration:underline;'>" + shopCount + "</span>";
                labShopCount.Text = text;
            }
            else
                labShopCount.Text = "0";

        }

        void BindSubjectDDL()
        {
            ddlSubject.Items.Clear();
            ddlSubject.Items.Add(new ListItem("--请选择--", "0"));
            List<string> regions = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (subject.SubjectType != (int)SubjectTypeEnum.二次安装 && subject.SubjectType != (int)SubjectTypeEnum.补单)
                        && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == (int)OrderTypeEnum.道具))
                        select new { shop, subject }).ToList();
            if (regions.Any())
            {
                list = list.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
            }
            if (list.Any())
            {
                var slist = list.Select(s => s.subject).Distinct().OrderBy(s => s.SubjectTypeId).ToList();
                slist.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.SubjectName;
                    ddlSubject.Items.Add(li);
                });

            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {

        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectType();
            BindSubject();
            BindPOSScale();
            GetShopCount();
            BindSubjectDDL();
        }

        protected void cblSubjectType_SelectedIndexChanged(object sender, EventArgs e)
        {

            BindSubject();
            BindPOSScale();
            GetShopCount();

        }

        protected void cblSubjectName_SelectedIndexChanged(object sender, EventArgs e)
        {

            BindPOSScale();
            GetShopCount();

        }

        protected void cblPOSScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetShopCount();
        }

        /*
        protected void btnSubmit_Click111111(object sender, EventArgs e)
        {
            List<string> selectRegion = new List<string>();
            List<int> selectSubjectType = new List<int>();
            List<int> selectSubject = new List<int>();
            List<string> selectPOSScale = new List<string>();

            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !selectRegion.Contains(li.Value.ToLower()))
                    selectRegion.Add(li.Value.ToLower());
            }

            foreach (ListItem li in cblSubjectType.Items)
            {
                if (li.Selected && !selectSubjectType.Contains(int.Parse(li.Value)))
                    selectSubjectType.Add(int.Parse(li.Value));
            }

            foreach (ListItem li in cblSubjectName.Items)
            {
                if (li.Selected && !selectSubject.Contains(int.Parse(li.Value)))
                    selectSubject.Add(int.Parse(li.Value));
            }

            foreach (ListItem li in cblPOSScale.Items)
            {
                if (li.Selected && !selectPOSScale.Contains(li.Value))
                    selectPOSScale.Add(li.Value);
            }
            if (!selectPOSScale.Any())
            {
                foreach (ListItem li in cblPOSScale.Items)
                {
                    if (!selectPOSScale.Contains(li.Value))
                        selectPOSScale.Add(li.Value);
                }
            }
            //var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //            join subject in CurrentContext.DbContext.Subject
            //            on order.SubjectId equals subject.Id
            //            join shop in CurrentContext.DbContext.Shop
            //            on order.ShopId equals shop.Id
            //            //join subjectType1 in CurrentContext.DbContext.SubjectType
            //            //on subject.SubjectTypeId equals subjectType1.Id into typeTemp
            //            //from subjectType in typeTemp.DefaultIfEmpty()
            //            join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
            //            on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
            //            from subjectCategory in categortTemp.DefaultIfEmpty()
            //            join pop1 in CurrentContext.DbContext.POP
            //            on new { order.ShopId, order.Sheet, order.GraphicNo } equals new { pop1.ShopId, pop1.Sheet, pop1.GraphicNo } into popTemp
            //            from pop in popTemp.DefaultIfEmpty()
            //            where subject.GuidanceId == guidanceId
            //            && subject.SubjectType != (int)SubjectTypeEnum.二次安装
            //            && subject.SubjectType != (int)SubjectTypeEnum.费用订单
            //            && (subject.IsDelete == null || subject.IsDelete == false)
            //            && subject.ApproveState == 1
            //            && (order.IsDelete == null || order.IsDelete == false)
            //                //&& ((shop.IsInstall != null && shop.IsInstall == "Y")||(subject.CornerType=="三叶草" && shop.BCSInstallPrice!=null && shop.BCSInstallPrice>0))
            //            && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && shop.BCSIsInstall=="Y"))
            //            && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == (int)OrderTypeEnum.道具))
            //            //&& (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
            //            select new
            //            {
            //                subject,
            //                pop,
            //                shop,
            //                order,
            //                POSScale = order.InstallPricePOSScale,
            //                CategoryName = subjectCategory!=null?(subjectCategory.CategoryName):""
            //            }).ToList();

            ////去掉已归类的店铺
            ////var priceShopList = ;
            //List<int> assginShopList = new List<int>();
            //assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();

            //if (assginShopList.Any())
            //{
            //    list = list.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
            //}


            //if (selectRegion.Any())
            //{
            //    list = list.Where(s => selectRegion.Contains(s.order.Region)).ToList();
            //}
            //if (selectSubjectType.Any())
            //{
            //    //list = list.Where(s => selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
            //    if (selectSubjectType.Contains(0))
            //    {
            //        selectSubjectType.Remove(0);
            //        if (selectSubjectType.Any())
            //        {
            //            list = list.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
            //        }
            //        else
            //            list = list.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
            //    }
            //    else
            //        list = list.Where(s => selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
            //}
            //if (selectSubject.Any())
            //{
            //    //list = list.Where(s => selectSubject.Contains(s.subject.Id) || selectSubject.Contains(s.subject.HandMakeSubjectId ?? 0)).ToList();
            //    list = list.Where(s => selectSubject.Contains(s.subject.Id)).ToList();
            //}
            //if (selectPOSScale.Any())
            //{
            //    if (selectPOSScale.Contains("空"))
            //    {
            //        List<string> temp = new List<string>();
            //        temp.AddRange(selectPOSScale);
            //        temp.Remove("空");
            //        if (temp.Any())
            //        {
            //            list = list.Where(s => temp.Contains(s.POSScale) || s.POSScale == "" || s.POSScale == null).ToList();
            //        }
            //        else
            //            list = list.Where(s => s.POSScale == "" || s.POSScale == null).ToList();


            //    }
            //    else
            //        list = list.Where(s => selectPOSScale.Contains(s.POSScale)).ToList();
            //}
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            List<Shop> shopList = new List<Shop>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (Session["shopInstallPrice"] != null)
            {
                shopList = Session["shopInstallPrice"] as List<Shop>;
            }
            if (orderList.Any())
            {
                if (Session["assginShopIdInstallPrice"] != null)
                {
                    List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
                    if (assginShopIdList.Any())
                    {
                        orderList = orderList.Where(s => !assginShopIdList.Contains(s.Id)).ToList();
                    }
                }
            }
            var orderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              join subjectType1 in CurrentContext.DbContext.SubjectType
                              on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                              from subjectType in typeTemp.DefaultIfEmpty()
                              join shop in shopList
                              on order.ShopId equals shop.Id
                              join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                              on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                              from subjectCategory in categortTemp.DefaultIfEmpty()
                              where (selectRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? selectRegion.Contains(subject.PriceBlongRegion.ToLower()) : selectRegion.Contains(order.Region.ToLower())) : 1 == 1)
                              select new
                              {
                                  order,
                                  subject,
                                  shop,
                                  CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                              }).ToList();

            if (selectSubjectType.Any())
            {

                if (selectSubjectType.Contains(0))
                {
                    selectSubjectType.Remove(0);
                    if (selectSubjectType.Any())
                    {
                        orderList0 = orderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                }
                else
                    orderList0 = orderList0.Where(s => selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
            }
            if (selectSubject.Any())
            {
                //orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId??0)).ToList();
                orderList0 = orderList0.Where(s => selectSubject.Contains(s.subject.Id)).ToList();
            }
            if (selectPOSScale.Any())
            {
                if (selectPOSScale.Contains("空"))
                {
                    selectPOSScale.Remove("空");
                    if (selectPOSScale.Any())
                    {
                        orderList0 = orderList0.Where(s => selectPOSScale.Contains(s.order.InstallPricePOSScale) || s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                }
                else
                    orderList0 = orderList0.Where(s => selectPOSScale.Contains(s.order.InstallPricePOSScale)).ToList();
            }


            if (orderList0.Any())
            {

                List<int> shopIdList = orderList0.Select(s => s.shop.Id).Distinct().ToList();
                var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);
                //计算基础安装费的店铺Id,所有项目中，相同店铺只算一次
                List<int> basicInstallPriceShop = new List<int>();
                //计算橱窗安装费的店铺Id,所有项目中，相同店铺只算一次
                List<int> windowInstallPriceShop = new List<int>();
                //基础安装费
                decimal basicInstallPrice = 0;
                //橱窗安装费(同一个店，无论有多少个橱窗订单，都算一次)
                decimal windowInstallPrice = 0;
                decimal oohInstallPrice = 0;
                //有橱窗订单店铺
                List<int> windowSheetShopIdList = orderList0.Where(s => (s.order.Sheet != null && (s.order.Sheet.Contains("橱窗") || s.order.Sheet.ToLower() == "window") && (!s.CategoryName.Contains("常规-非活动")))).Select(s => s.shop.Id).Distinct().ToList();

                List<InstallPriceShopInfo> installPriceShopList = new List<InstallPriceShopInfo>();
                InstallPriceShopInfo installPriceShopModel;
                InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                List<string> BCSInstallCityTierList = new List<string>();
                decimal bcsBasicInstallPrice = 150;//三叶草t1-t3安装费
                if (!BCSInstallCityTierList.Any())
                {
                    string BCSInstallCityTier = string.Empty;
                    try
                    {
                        BCSInstallCityTier = ConfigurationManager.AppSettings["BCSBasicInstallPrice"];
                        if (!string.IsNullOrWhiteSpace(BCSInstallCityTier))
                        {
                            string[] str = BCSInstallCityTier.Split(':');
                            BCSInstallCityTierList = StringHelper.ToStringList(str[0], ',', LowerUpperEnum.ToUpper);
                            bcsBasicInstallPrice = StringHelper.IsDecimal(str[1]);
                        }
                    }
                    catch
                    {

                    }
                }
                orderList0.ForEach(s =>
                {
                    installPriceShopModel = new InstallPriceShopInfo();
                    installPriceShopModel.ShopId = s.shop.Id;
                    installPriceShopModel.POSScale = s.order.InstallPricePOSScale;
                    installPriceShopModel.MaterialSupport = s.order.MaterialSupport;
                    installPriceShopModel.GuidanceId = guidanceId;
                    decimal basicPrice = 0;
                    decimal windowPrice = 0;
                    bool isvalid = false;
                    //基础安装费
                    if (!basicInstallPriceShop.Contains(s.shop.Id))
                    {
                        basicInstallPriceShop.Add(s.shop.Id);

                        if (cbIsSecondInstall.Checked)
                        {
                            basicPrice = 150;//如果是二次安装，基础安装费150，其他不算
                        }
                        else
                        {
                            if (s.subject.CornerType == "三叶草")
                            {
                                if ((s.shop.BCSInstallPrice ?? 0) > 0)
                                {
                                    basicPrice = (s.shop.BCSInstallPrice ?? 0);
                                }
                                else if (BCSInstallCityTierList.Contains(s.order.CityTier.ToUpper()))
                                {
                                    basicPrice = bcsBasicInstallPrice;
                                }
                                else
                                {
                                    basicPrice = 0;
                                }
                            }
                            else if (s.CategoryName.Contains("常规-非活动"))
                            {
                                if (BCSInstallCityTierList.Contains(s.order.CityTier.ToUpper()))
                                {
                                    basicPrice = bcsBasicInstallPrice;
                                }
                                else
                                {
                                    basicPrice = 0;
                                }
                            }
                            else
                            {
                                if ((s.shop.BasicInstallPrice ?? 0) > 0)
                                {
                                    basicPrice = (s.shop.BasicInstallPrice ?? 0);
                                }
                                else
                                    basicPrice += GetBasicInstallPrice(s.order.InstallPriceMaterialSupport);
                            }
                        }
                        installPriceShopModel.BasicPrice = basicPrice;
                        basicInstallPrice += basicPrice;
                        isvalid = true;
                    }
                    if (!cbIsSecondInstall.Checked)
                    {
                        //橱窗安装费
                        if (windowSheetShopIdList.Contains(s.shop.Id) && !windowInstallPriceShop.Contains(s.shop.Id))
                        {
                            windowInstallPriceShop.Add(s.shop.Id);
                            windowPrice += GetWindowInstallPrice(s.order.InstallPriceMaterialSupport);
                            windowInstallPrice += windowPrice;
                            installPriceShopModel.WindowPrice = windowPrice;
                        }

                    }
                    if (isvalid)
                        installPriceShopList.Add(installPriceShopModel);
                });
                if (!cbIsSecondInstall.Checked)
                {
                    //户外订单(同一个店，如果有2个以上的户外位置订单，按最高算)
                    //var oohOrderList = list.Where(s => s.pop != null && (s.order.Sheet == "户外" || s.order.Sheet.ToLower() == "ooh") && (s.pop.OOHInstallPrice ?? 0) > 0).Select(s => new { ShopId = s.shop.Id, OOHInstallPrice = s.pop.OOHInstallPrice ?? 0 }).ToList();
                    var oohOrderList0 = orderList0.Where(s => (s.order.Sheet != null && (s.order.Sheet == "户外" || s.order.Sheet.ToLower() == "ooh"))).ToList();
                    List<int> oohOrderShopIdList = oohOrderList0.Select(s => s.shop.Id).Distinct().ToList();
                    oohPOPList = oohPOPList.Where(s => oohOrderShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    if (oohOrderList0.Any())
                    {
                        Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                        oohOrderList0.ForEach(s =>
                        {
                            decimal price = 0;
                            if (!string.IsNullOrWhiteSpace(s.order.GraphicNo))
                            {
                                price = oohPOPList.Where(p => p.ShopId == s.shop.Id && p.GraphicNo.ToLower() == s.order.GraphicNo.ToLower()).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                            }
                            else
                                price = oohPOPList.Where(p => p.ShopId == s.shop.Id && p.GraphicLength == s.order.GraphicLength && p.GraphicWidth == s.order.GraphicWidth).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                            if (oohPriceDic.Keys.Contains(s.shop.Id))
                            {
                                if (oohPriceDic[s.shop.Id] < price)
                                {
                                    oohPriceDic[s.shop.Id] = price;
                                }
                            }
                            else
                                oohPriceDic.Add(s.shop.Id, price);
                        });
                        if (oohPriceDic.Keys.Count > 0)
                        {
                            foreach (KeyValuePair<int, decimal> item in oohPriceDic)
                            {
                                var model0 = installPriceShopList.Where(sh => sh.ShopId == item.Key).FirstOrDefault();
                                if (model0 != null)
                                {
                                    int index = installPriceShopList.IndexOf(model0);
                                    model0.OOHPrice = item.Value;
                                    installPriceShopList[index] = model0;
                                    oohInstallPrice += item.Value;
                                }
                            }
                        }
                    }



                }
                decimal total = basicInstallPrice + windowInstallPrice + oohInstallPrice;
                InstallPriceDetailBLL installPriceBll = new InstallPriceDetailBLL();
                int subjectId = int.Parse(ddlSubject.SelectedValue);
                InstallPriceDetail model = null;
                model = installPriceBll.GetList(s => s.GuidanceId == guidanceId && s.SubjectId == subjectId).FirstOrDefault();
                if (model != null)
                {
                    //model.Price += total;
                    if (selectPOSScale.Any())
                    {
                        List<string> POSScale = new List<string>();
                        if (!string.IsNullOrWhiteSpace(model.SelectPOSScale))
                        {
                            POSScale = StringHelper.ToStringList(model.SelectPOSScale, ',');
                        }
                        selectPOSScale.ForEach(s =>
                        {
                            if (!POSScale.Contains(s))
                            {
                                POSScale.Add(s);
                            }
                        });
                        model.SelectPOSScale = StringHelper.ListToString(POSScale);
                    }

                    if (selectRegion.Any())
                    {

                        List<string> Regions = new List<string>();
                        if (!string.IsNullOrWhiteSpace(model.SelectRegion))
                        {
                            Regions = StringHelper.ToStringList(model.SelectRegion, ',');
                        }
                        selectRegion.ForEach(s =>
                        {
                            if (!Regions.Contains(s))
                            {
                                Regions.Add(s);
                            }
                        });
                        model.SelectRegion = StringHelper.ListToString(Regions);
                    }
                    if (selectSubject.Any())
                    {
                        // model.SelectSubjectId = StringHelper.ListToString(selectSubject) + "," + model.SelectSubjectId;
                        List<int> Subjects = new List<int>();
                        if (!string.IsNullOrWhiteSpace(model.SelectSubjectId))
                        {
                            Subjects = StringHelper.ToIntList(model.SelectSubjectId, ',');
                        }
                        selectSubject.ForEach(s =>
                        {
                            if (!Subjects.Contains(s))
                            {
                                Subjects.Add(s);
                            }
                        });
                        model.SelectSubjectId = StringHelper.ListToString(Subjects);
                    }
                    if (selectSubjectType.Any())
                    {
                        //model.SelectSubjectTypeId = StringHelper.ListToString(selectSubjectType) + "," + model.SelectSubjectTypeId;
                        List<int> SubjectTypes = new List<int>();
                        if (!string.IsNullOrWhiteSpace(model.SelectSubjectTypeId))
                        {
                            SubjectTypes = StringHelper.ToIntList(model.SelectSubjectTypeId, ',');
                        }
                        selectSubjectType.ForEach(s =>
                        {
                            if (!SubjectTypes.Contains(s))
                            {
                                SubjectTypes.Add(s);
                            }
                        });
                        model.SelectSubjectTypeId = StringHelper.ListToString(SubjectTypes);
                    }

                    installPriceBll.Update(model);

                }
                else
                {
                    try
                    {
                        model = new InstallPriceDetail();
                        model.AddType = 1;
                        model.AddDate = DateTime.Now;
                        model.GuidanceId = guidanceId;
                        //model.Price = total;
                        model.SelectPOSScale = StringHelper.ListToString(selectPOSScale);
                        model.SelectRegion = StringHelper.ListToString(selectRegion);
                        model.SelectSubjectId = StringHelper.ListToString(selectSubject);
                        model.SelectSubjectTypeId = StringHelper.ListToString(selectSubjectType);
                        //model.ShopCount = shopIdList.Count;
                        // model.ShopIds = StringHelper.ListToString(shopIdList);
                        model.SubjectId = subjectId;
                        installPriceBll.Add(model);
                    }
                    catch (Exception ex)
                    { }

                }
                if (installPriceShopList.Any())
                {
                    installPriceShopList.ForEach(s =>
                    {
                        var model2 = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == s.ShopId).FirstOrDefault();
                        if (model2 == null)
                        {
                            InstallPriceShopInfo model1 = new InstallPriceShopInfo();
                            model1 = s;
                            model1.InstallDetailId = model.Id;
                            model1.SubjectId = subjectId;
                            model1.AddType = 1;
                            installShopBll.Add(model1);
                        }
                    });
                }

                //BindRegion();
                //BindSubjectType();
                //BindSubject();
                //BindPOSScale();
                //GetShopCount();
                //BindData();
            }

        }
        */


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SubmitPrice();
        }

        /// <summary>
        /// 安装费计算（新）
        /// </summary>
        void SubmitPrice()
        {
            bool isOk = true;
            List<string> selectRegion = new List<string>();
            List<int> selectSubjectType = new List<int>();
            List<int> selectSubject = new List<int>();
            List<string> selectPOSScale = new List<string>();

            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !selectRegion.Contains(li.Value.ToLower()))
                    selectRegion.Add(li.Value.ToLower());
            }

            foreach (ListItem li in cblSubjectType.Items)
            {
                if (li.Selected && !selectSubjectType.Contains(int.Parse(li.Value)))
                    selectSubjectType.Add(int.Parse(li.Value));
            }

            foreach (ListItem li in cblSubjectName.Items)
            {
                if (li.Selected && !selectSubject.Contains(int.Parse(li.Value)))
                    selectSubject.Add(int.Parse(li.Value));
            }

            foreach (ListItem li in cblPOSScale.Items)
            {
                if (li.Selected && !selectPOSScale.Contains(li.Value))
                    selectPOSScale.Add(li.Value);
            }
            if (!selectPOSScale.Any())
            {
                foreach (ListItem li in cblPOSScale.Items)
                {
                    if (!selectPOSScale.Contains(li.Value))
                        selectPOSScale.Add(li.Value);
                }
            }

            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            List<Shop> shopList = new List<Shop>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (Session["shopInstallPrice"] != null)
            {
                shopList = Session["shopInstallPrice"] as List<Shop>;
            }
            if (orderList.Any())
            {
                if (Session["assginShopIdInstallPrice"] != null)
                {
                    List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
                    if (assginShopIdList.Any())
                    {
                        orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }
                }
            }
            var orderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              join subjectType1 in CurrentContext.DbContext.SubjectType
                              on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                              from subjectType in typeTemp.DefaultIfEmpty()
                              join shop in shopList
                              on order.ShopId equals shop.Id
                              join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                              on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                              from subjectCategory in categortTemp.DefaultIfEmpty()
                              where (selectRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? selectRegion.Contains(subject.PriceBlongRegion.ToLower()) : selectRegion.Contains(order.Region.ToLower())) : 1 == 1)
                              select new
                              {
                                  order,
                                  subject,
                                  shop,
                                  CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                              }).ToList();

            if (selectSubjectType.Any())
            {

                if (selectSubjectType.Contains(0))
                {
                    selectSubjectType.Remove(0);
                    if (selectSubjectType.Any())
                    {
                        orderList0 = orderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                }
                else
                    orderList0 = orderList0.Where(s => selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
            }
            if (selectSubject.Any())
            {

                orderList0 = orderList0.Where(s => selectSubject.Contains(s.subject.Id)).ToList();
            }
            if (selectPOSScale.Any())
            {
                if (selectPOSScale.Contains("空"))
                {
                    selectPOSScale.Remove("空");
                    if (selectPOSScale.Any())
                    {
                        orderList0 = orderList0.Where(s => selectPOSScale.Contains(s.order.InstallPricePOSScale) || s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                }
                else
                    orderList0 = orderList0.Where(s => selectPOSScale.Contains(s.order.InstallPricePOSScale)).ToList();
            }
            if (orderList0.Any())
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        #region 保存/更新InstallPriceDetail
                        InstallPriceDetailBLL installPriceBll = new InstallPriceDetailBLL();
                        int subjectId = int.Parse(ddlSubject.SelectedValue);
                        InstallPriceDetail model = installPriceBll.GetList(s => s.GuidanceId == guidanceId && s.SubjectId == subjectId && s.AddType==1).FirstOrDefault();
                        if (model != null)
                        {

                            if (selectPOSScale.Any())
                            {
                                List<string> POSScale = new List<string>();
                                if (!string.IsNullOrWhiteSpace(model.SelectPOSScale))
                                {
                                    POSScale = StringHelper.ToStringList(model.SelectPOSScale, ',');
                                }
                                selectPOSScale.ForEach(s =>
                                {
                                    if (!POSScale.Contains(s))
                                    {
                                        POSScale.Add(s);
                                    }
                                });
                                model.SelectPOSScale = StringHelper.ListToString(POSScale);
                            }

                            if (selectRegion.Any())
                            {

                                List<string> Regions = new List<string>();
                                if (!string.IsNullOrWhiteSpace(model.SelectRegion))
                                {
                                    Regions = StringHelper.ToStringList(model.SelectRegion, ',');
                                }
                                selectRegion.ForEach(s =>
                                {
                                    if (!Regions.Contains(s))
                                    {
                                        Regions.Add(s);
                                    }
                                });
                                model.SelectRegion = StringHelper.ListToString(Regions);
                            }
                            if (selectSubject.Any())
                            {
                                List<int> Subjects = new List<int>();
                                if (!string.IsNullOrWhiteSpace(model.SelectSubjectId))
                                {
                                    Subjects = StringHelper.ToIntList(model.SelectSubjectId, ',');
                                }
                                selectSubject.ForEach(s =>
                                {
                                    if (!Subjects.Contains(s))
                                    {
                                        Subjects.Add(s);
                                    }
                                });
                                model.SelectSubjectId = StringHelper.ListToString(Subjects);
                            }
                            if (selectSubjectType.Any())
                            {
                                List<int> SubjectTypes = new List<int>();
                                if (!string.IsNullOrWhiteSpace(model.SelectSubjectTypeId))
                                {
                                    SubjectTypes = StringHelper.ToIntList(model.SelectSubjectTypeId, ',');
                                }
                                selectSubjectType.ForEach(s =>
                                {
                                    if (!SubjectTypes.Contains(s))
                                    {
                                        SubjectTypes.Add(s);
                                    }
                                });
                                model.SelectSubjectTypeId = StringHelper.ListToString(SubjectTypes);
                            }

                            installPriceBll.Update(model);

                        }
                        else
                        {
                            model = new InstallPriceDetail();
                            model.AddType = 1;
                            model.AddDate = DateTime.Now;
                            model.GuidanceId = guidanceId;
                            model.SelectPOSScale = StringHelper.ListToString(selectPOSScale);
                            model.SelectRegion = StringHelper.ListToString(selectRegion);
                            model.SelectSubjectId = StringHelper.ListToString(selectSubject);
                            model.SelectSubjectTypeId = StringHelper.ListToString(selectSubjectType);
                            model.SubjectId = subjectId;
                            installPriceBll.Add(model);
                        }
                        #endregion
                        List<string> BCSInstallCityTierList = new List<string>();
                        decimal bcsBasicInstallPrice = 150;//三叶草t1-t3安装费
                        if (!BCSInstallCityTierList.Any())
                        {
                            string BCSInstallCityTier = string.Empty;
                            try
                            {
                                BCSInstallCityTier = ConfigurationManager.AppSettings["BCSBasicInstallPrice"];
                                if (!string.IsNullOrWhiteSpace(BCSInstallCityTier))
                                {
                                    string[] str = BCSInstallCityTier.Split(':');
                                    BCSInstallCityTierList = StringHelper.ToStringList(str[0], ',', LowerUpperEnum.ToUpper);
                                    bcsBasicInstallPrice = StringHelper.IsDecimal(str[1]);
                                }
                            }
                            catch
                            {
                                
                            }
                        }
                        List<Shop> shopList0 = orderList0.Select(s => s.shop).Distinct().ToList();
                        List<int> shopIdList = shopList0.Select(s => s.Id).ToList();
                        var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);
                        var allOrderList = (from order in orderList
                                            join subject in subjectList
                                            on order.SubjectId equals subject.Id
                                            join subjectType1 in CurrentContext.DbContext.SubjectType
                                            on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                                            from subjectType in typeTemp.DefaultIfEmpty()
                                            join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                            on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                            from subjectCategory in categortTemp.DefaultIfEmpty()
                                            where shopIdList.Contains(order.ShopId ?? 0)
                                            select new
                                            {
                                                order,
                                                subject,
                                                CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                            }).ToList();
                        shopList0.ForEach(shop =>
                        {
                            
                            bool isBCSSubject = true;
                            bool isGeneric = true;
                            //基础安装费
                            decimal basicInstallPrice = 0;
                            //橱窗安装费
                            decimal windowInstallPrice = 0;
                            //OOH安装费
                            decimal oohInstallPrice = 0;
                            string materialSupport = string.Empty;
                            string POSScale = string.Empty;
                            List<string> materialSupportList = new List<string>();
                            var oneShopOrderList = allOrderList.Where(s => s.order.ShopId == shop.Id).ToList();
                            if (oneShopOrderList.Any())
                            {
                                oneShopOrderList.ForEach(s =>
                                {
                                    if (!string.IsNullOrWhiteSpace(s.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(s.order.InstallPriceMaterialSupport.ToLower()))
                                    {
                                        materialSupportList.Add(s.order.InstallPriceMaterialSupport.ToLower());
                                    }
                                    if (string.IsNullOrWhiteSpace(POSScale) && !string.IsNullOrWhiteSpace(s.order.InstallPricePOSScale))
                                        POSScale = s.order.InstallPricePOSScale;
                                    if (s.subject.CornerType != "三叶草")
                                        isBCSSubject = false;
                                    if (!s.CategoryName.Contains("常规-非活动"))
                                        isGeneric = false;
                                });
                                List<FinalOrderDetailTemp> oohOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower() == "ooh" || s.order.Sheet == "户外")).Select(s => s.order).ToList();
                                List<FinalOrderDetailTemp> windowOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower().Contains("橱窗") || s.order.Sheet.ToLower().Contains("window"))).Select(s => s.order).ToList();


                                #region 店内安装费
                                materialSupportList.ForEach(ma =>
                                {
                                    decimal basicInstallPrice0 = GetBasicInstallPrice(ma);
                                    if (basicInstallPrice0 > basicInstallPrice)
                                    {
                                        basicInstallPrice = basicInstallPrice0;
                                        materialSupport = ma;
                                    }
                                });
                                if (isBCSSubject)
                                {
                                    if ((shop.BCSInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = (shop.BCSInstallPrice ?? 0);
                                    }
                                    else if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                                    {
                                        basicInstallPrice = bcsBasicInstallPrice;
                                    }
                                    else
                                    {
                                        basicInstallPrice = 0;
                                    }
                                }
                                else if (isGeneric)
                                {
                                    if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                                    {
                                        basicInstallPrice = bcsBasicInstallPrice;
                                    }
                                    else
                                    {
                                        basicInstallPrice = 0;
                                    }
                                }
                                else if ((shop.BasicInstallPrice ?? 0) > 0)
                                {
                                    basicInstallPrice = (shop.BasicInstallPrice ?? 0);
                                }
                                //if (cbIsSecondInstall.Checked)
                                //{
                                //    basicInstallPrice = 150;//如果是二次安装，基础安装费150，其他不算
                                //}
                                //else
                                //{

                                    
                                    
                                //}
                                #endregion
                                #region 橱窗安装
                                if (windowOrderList.Any())
                                {
                                    windowInstallPrice = GetWindowInstallPrice(materialSupport);
                                }
                                #endregion
                                #region OOH安装费
                                if (oohOrderList.Any())
                                {

                                    oohOrderList.ForEach(s =>
                                    {
                                        decimal price = 0;
                                        if (!string.IsNullOrWhiteSpace(s.GraphicNo))
                                        {
                                            price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.GraphicNo.ToLower()).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                                        }
                                        else
                                            price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.GraphicLength && p.GraphicWidth == s.GraphicWidth).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                                        if (price > oohInstallPrice)
                                        {
                                            oohInstallPrice = price;
                                        }
                                    });
                                }
                                #endregion
                                #region 保存InstallPriceShopInfo
                                InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                                InstallPriceShopInfo installPriceShopModel = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == shop.Id && i.AddType==1).FirstOrDefault();
                                if (installPriceShopModel == null)
                                {
                                    installPriceShopModel = new InstallPriceShopInfo();
                                    installPriceShopModel.BasicPrice = basicInstallPrice;
                                    installPriceShopModel.OOHPrice = oohInstallPrice;
                                    installPriceShopModel.WindowPrice = windowInstallPrice;
                                    installPriceShopModel.InstallDetailId = model.Id;
                                    installPriceShopModel.GuidanceId = guidanceId;
                                    installPriceShopModel.SubjectId = subjectId;
                                    installPriceShopModel.ShopId = shop.Id;
                                    installPriceShopModel.MaterialSupport = materialSupport;
                                    installPriceShopModel.POSScale = POSScale;
                                    installPriceShopModel.AddType = 1;
                                    installShopBll.Add(installPriceShopModel);
                                }
                                #endregion
                            }
                        });
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        isOk = false;
                    }
                }
                if (isOk)
                {
                    Response.Redirect(string.Format("SubmitInstallPrice.aspx?itemid={0}",guidanceId),false);
                }
            }
        }


        protected void RepeaterList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                InstallPriceDetailBLL bll = new InstallPriceDetailBLL();
                InstallPriceDetail model = bll.GetModel(id);
                if (model != null)
                {
                   
                    installShopBll.Delete(s => s.InstallDetailId == model.Id);
                    bll.Delete(model);
                    Response.Redirect(string.Format("SubmitInstallPrice.aspx?itemid={0}", guidanceId), false);
                   
                }

            }
        }

        SubjectTypeBLL subjectTypeBll = new SubjectTypeBLL();
        InstallPriceShopInfo installShopBll = new InstallPriceShopInfo();

        protected void RepeaterList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object subjectTypeIdObj = item.GetType().GetProperty("SelectSubjectTypeId").GetValue(item, null);
                    string subjectTypeIds = subjectTypeIdObj != null ? subjectTypeIdObj.ToString() : "";
                    if (!string.IsNullOrWhiteSpace(subjectTypeIds))
                    {
                        List<int> typeIdList = StringHelper.ToIntList(subjectTypeIds, ',');
                        List<string> typeList = subjectTypeBll.GetList(s => typeIdList.Contains(s.Id)).Select(s => s.SubjectTypeName).ToList();
                        if (typeList.Any())
                        {
                            ((Label)e.Item.FindControl("labSubjectTypeName")).Text = StringHelper.ListToString(typeList);
                        }
                    }
                    object IdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int id = IdObj != null ? int.Parse(IdObj.ToString()) : 0;
                    var list = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                //join shop in CurrentContext.DbContext.Shop
                                //on installShop.ShopId equals shop.Id
                                where installShop.InstallDetailId == id
                                //&& (installShop.AddType == null || installShop.AddType==1)
                                //&& myInstallShopIdList.Contains(installShop.ShopId ?? 0)
                                && (installShop.BasicPrice ?? 0) > 0
                                select installShop).ToList();
                    if (list.Any())
                    {
                        int shopCount = list.Select(s => s.ShopId ?? 0).Distinct().Count();
                        List<string> posScaleList = list.Select(s => s.POSScale).Distinct().ToList();
                        int index = posScaleList.IndexOf("");
                        if (index != -1)
                            posScaleList[index] = "空";
                        string posScale = StringHelper.ListToString(posScaleList);

                        ((Label)e.Item.FindControl("labSelectPOSScale")).Text = posScale;
                        ((Label)e.Item.FindControl("labShopCount")).Text = shopCount.ToString();
                        decimal price = list.Sum(s => ((s.BasicPrice ?? 0) + (s.WindowPrice ?? 0) + (s.OOHPrice ?? 0)));
                        //list.ForEach(s =>
                        //{
                        //    price += (s.BasicPrice ?? 0) + (s.WindowPrice ?? 0) + (s.OOHPrice ?? 0);
                        //});

                        if (price > 0)
                            ((Label)e.Item.FindControl("labPrice")).Text = Math.Round(price, 2).ToString();
                    }
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("GuidanceList.aspx?comeback=1", false);
        }


        //以下是单独安装费的内容


        void BindSubjectType1()
        {
            cblSecondSubjectType.Items.Clear();

            List<string> regions = new List<string>();
            foreach (ListItem li in cblSecondRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            //if (orderList.Any())
            //{
            //    if (Session["assginShopIdInstallPrice"] != null)
            //    {
            //        List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
            //        if (assginShopIdList.Any())
            //        {
            //            orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
            //        }
            //    }
            //}
            var orderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              join subjectType1 in CurrentContext.DbContext.SubjectType
                             on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                              from subjectType in typeTemp.DefaultIfEmpty()
                              where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                              select new
                              {
                                  order,
                                  subject.SubjectTypeId,
                                  SubjectTypeName = subjectType != null ? subjectType.SubjectTypeName : ""
                              }).ToList();

            if (orderList0.Any())
            {
                var list = orderList0.Select(s => new { s.SubjectTypeId, s.SubjectTypeName }).Distinct().ToList();
                List<int> typeIdList = new List<int>();
                bool isEmpty = false;
                list.ForEach(s =>
                {
                    if (s.SubjectTypeId != null && s.SubjectTypeId != 0)
                    {
                        if (!typeIdList.Contains(s.SubjectTypeId ?? 0))
                        {
                            ListItem li = new ListItem();
                            li.Value = (s.SubjectTypeId ?? 0).ToString();
                            li.Text = s.SubjectTypeName + "&nbsp;&nbsp;";
                            cblSecondSubjectType.Items.Add(li);
                        }
                    }
                    else
                        isEmpty = true;
                });
                if (isEmpty)
                {
                    ListItem li = new ListItem();
                    li.Value = "0";
                    li.Text = "空";
                    cblSecondSubjectType.Items.Add(li);
                }

            }
        }

        void BindSubject1()
        {
            cblSecondSubjectName.Items.Clear();
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
           
            if (orderList.Any())
            {
                List<string> regions = new List<string>();
                List<int> subjectTypeList = new List<int>();
                foreach (ListItem li in cblSecondRegion.Items)
                {
                    if (li.Selected && !regions.Contains(li.Value.ToLower()))
                        regions.Add(li.Value.ToLower());
                }
                foreach (ListItem li in cblSecondSubjectType.Items)
                {
                    if (li.Selected && !subjectTypeList.Contains(int.Parse(li.Value)))
                        subjectTypeList.Add(int.Parse(li.Value));
                }
                var orderList0 = (from order in orderList
                                  join subject in subjectList
                                  on order.SubjectId equals subject.Id
                                  join subjectType1 in CurrentContext.DbContext.SubjectType
                                 on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                                  from subjectType in typeTemp.DefaultIfEmpty()
                                  where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                                  select new
                                  {
                                      order,
                                      subject
                                  }).ToList();


                if (subjectTypeList.Any())
                {

                    if (subjectTypeList.Contains(0))
                    {
                        subjectTypeList.Remove(0);
                        if (subjectTypeList.Any())
                        {
                            orderList0 = orderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                        }
                        else
                            orderList0 = orderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                    }
                    else
                        orderList0 = orderList0.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                }

                if (orderList0.Any())
                {
                    var subjectList0 = orderList0.Select(s => s.subject).Distinct().OrderBy(s => s.SubjectName).ToList();
                    subjectList0.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectName + "&nbsp;&nbsp;";
                        cblSecondSubjectName.Items.Add(li);
                    });
                }
            }
        }

        protected void cblSecondRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectType1();
            BindSubject1();
        }

        protected void cblSecondSubjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject1();
        }

        void BindData1()
        {
            var list = (from detail in CurrentContext.DbContext.InstallPriceDetail
                        join subject in CurrentContext.DbContext.Subject
                        on detail.SubjectId equals subject.Id
                        where detail.GuidanceId == guidanceId
                        && detail.AddType == 2
                        select new
                        {
                            detail,
                            detail.SelectSubjectTypeId,
                            subject.SubjectName,
                            detail.Id
                        }).ToList();
            //if (list.Any())
            //{

            //    myInstallShopIdList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //                           join subject in CurrentContext.DbContext.Subject
            //                           on order.SubjectId equals subject.Id
            //                           join shop in CurrentContext.DbContext.Shop
            //                           on order.ShopId equals shop.Id
            //                           where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
            //                           && subject.ApproveState == 1
            //                           && (order.IsDelete == null || order.IsDelete == false)
            //                           && subject.SubjectType != (int)SubjectTypeEnum.二次安装
            //                           && subject.SubjectType != (int)SubjectTypeEnum.费用订单
            //                           && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
            //                           && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
            //                           //&& (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
            //                           select shop.Id).Distinct().ToList();
            //    List<int> installDetailList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId && myInstallShopIdList.Contains(s.ShopId ?? 0) && s.AddType == 2).Select(s => s.InstallDetailId ?? 0).Distinct().ToList();
            //    list = list.Where(s => installDetailList.Contains(s.Id)).ToList();
            //}

            RepeaterList1.DataSource = list;
            RepeaterList1.DataBind();
        }

        protected void RepeaterList1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object subjectTypeIdObj = item.GetType().GetProperty("SelectSubjectTypeId").GetValue(item, null);
                    string subjectTypeIds = subjectTypeIdObj != null ? subjectTypeIdObj.ToString() : "";
                    if (!string.IsNullOrWhiteSpace(subjectTypeIds))
                    {
                        List<int> typeIdList = StringHelper.ToIntList(subjectTypeIds, ',');
                        List<string> typeList = subjectTypeBll.GetList(s => typeIdList.Contains(s.Id)).Select(s => s.SubjectTypeName).ToList();
                        if (typeList.Any())
                        {
                            ((Label)e.Item.FindControl("labSubjectTypeName")).Text = StringHelper.ListToString(typeList);
                        }
                    }
                    object IdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int id = IdObj != null ? int.Parse(IdObj.ToString()) : 0;
                    var list = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                                where installShop.InstallDetailId == id
                                //&& myInstallShopIdList.Contains(installShop.ShopId ?? 0)
                                && (installShop.BasicPrice ?? 0) > 0
                                select installShop).ToList();
                    if (list.Any())
                    {
                        int shopCount = list.Select(s => s.ShopId ?? 0).Distinct().Count();
                        ((Label)e.Item.FindControl("labShopCount")).Text = shopCount.ToString();
                        decimal price = 0;
                        list.ForEach(s =>
                        {
                            price += (s.BasicPrice ?? 0) + (s.WindowPrice ?? 0) + (s.OOHPrice ?? 0);
                        });
                        if (price > 0)
                            ((Label)e.Item.FindControl("labPrice")).Text = Math.Round(price, 2).ToString();
                    }
                }
            }
        }

        protected void RepeaterList1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                InstallPriceDetailBLL bll = new InstallPriceDetailBLL();
                OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
                InstallPriceDetail model = bll.GetModel(id);
                if (model != null)
                {
                    bool isOk = false;
                    using (TransactionScope tran = new TransactionScope())
                    {
                        try
                        {
                            List<int> shopIdList = installShopBll.GetList(s => s.InstallDetailId == model.Id).Select(s=>s.ShopId??0).ToList();
                            //删除外协安装费
                            outsourceOrderDetailBll.Delete(s => s.GuidanceId == model.GuidanceId && shopIdList.Contains(s.ShopId??0) && s.SubjectId==model.SubjectId && s.OrderType==(int)OrderTypeEnum.安装费);
                            installShopBll.Delete(s => s.InstallDetailId == model.Id);
                            bll.Delete(model);
                            tran.Complete();
                            isOk = true;
                        }
                        catch (Exception ex)
                        { 
                           
                        }
                    }
                   
                    if (isOk)
                    {
                        BindData1();
                    }
                }

            }
        }

        /*
        protected void btnSubmit1_Click1111(object sender, EventArgs e)
        {
            List<string> selectRegion = new List<string>();

            List<int> selectSubject = new List<int>();

            foreach (ListItem li in cblSecondRegion.Items)
            {
                if (li.Selected && !selectRegion.Contains(li.Value))
                    selectRegion.Add(li.Value);
            }

            foreach (ListItem li in cblSecondSubjectName.Items)
            {
                if (li.Selected && !selectSubject.Contains(int.Parse(li.Value)))
                    selectSubject.Add(int.Parse(li.Value));
            }
            if (selectSubject.Any())
            {
                selectSubject.ForEach(sid =>
                {
                    var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                from subjectCategory in categortTemp.DefaultIfEmpty()
                                join pop1 in CurrentContext.DbContext.POP
                                on new { order.ShopId, order.Sheet, order.GraphicNo } equals new { pop1.ShopId, pop1.Sheet, pop1.GraphicNo } into popTemp
                                from pop in popTemp.DefaultIfEmpty()
                                where subject.Id == sid

                                && (order.IsDelete == null || order.IsDelete == false)
                                && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                                && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                                //&& (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                                select new
                                {
                                    subject,
                                    pop,
                                    shop,
                                    order,
                                    POSScale = order.InstallPricePOSScale,
                                    CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                }).ToList();

                    if (selectRegion.Any())
                    {
                        list = list.Where(l => selectRegion.Contains(l.order.Region)).ToList();
                    }
                    if (list.Any())
                    {
                        List<int> shopIdList = list.Select(s => s.shop.Id).Distinct().ToList();
                        var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);
                        //计算基础安装费的店铺Id,所有项目中，相同店铺只算一次
                        List<int> basicInstallPriceShop = new List<int>();
                        //计算橱窗安装费的店铺Id,所有项目中，相同店铺只算一次
                        List<int> windowInstallPriceShop = new List<int>();
                        //基础安装费
                        decimal basicInstallPrice = 0;
                        //橱窗安装费(同一个店，无论有多少个橱窗订单，都算一次)
                        decimal windowInstallPrice = 0;
                        decimal oohInstallPrice = 0;
                        //有橱窗订单店铺
                        List<int> windowSheetShopIdList = list.Where(s => (s.order.Sheet != null && (s.order.Sheet == "橱窗" || s.order.Sheet.ToLower() == "window") && (!s.CategoryName.Contains("常规-非活动")))).Select(s => s.shop.Id).Distinct().ToList();

                        List<InstallPriceShopInfo> installPriceShopList = new List<InstallPriceShopInfo>();
                        InstallPriceShopInfo installPriceShopModel;
                        InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                        List<string> BCSInstallCityTierList = new List<string>();
                        decimal bcsBasicInstallPrice = 150;//三叶草t1-t3安装费
                        if (!BCSInstallCityTierList.Any())
                        {
                            string BCSInstallCityTier = string.Empty;
                            try
                            {
                                BCSInstallCityTier = ConfigurationManager.AppSettings["BCSBasicInstallPrice"];
                                if (!string.IsNullOrWhiteSpace(BCSInstallCityTier))
                                {
                                    string[] str = BCSInstallCityTier.Split(':');
                                    BCSInstallCityTierList = StringHelper.ToStringList(str[0], ',', LowerUpperEnum.ToUpper);
                                    bcsBasicInstallPrice = StringHelper.IsDecimal(str[1]);
                                }
                            }
                            catch
                            {

                            }
                        }
                        list.ForEach(s =>
                        {
                            installPriceShopModel = new InstallPriceShopInfo();
                            installPriceShopModel.ShopId = s.shop.Id;
                            installPriceShopModel.POSScale = s.POSScale;
                            installPriceShopModel.MaterialSupport = s.order.MaterialSupport;
                            installPriceShopModel.GuidanceId = guidanceId;

                            decimal basicPrice = 0;
                            decimal windowPrice = 0;
                            bool isvalid = false;
                            //基础安装费
                            if (!basicInstallPriceShop.Contains(s.shop.Id))
                            {
                                basicInstallPriceShop.Add(s.shop.Id);

                                if (cbIsSecondInstall.Checked)
                                {
                                    basicPrice = 150;//如果是二次安装，基础安装费150，其他不算
                                }
                                else
                                {
                                    if (s.subject.CornerType == "三叶草")
                                    {
                                        if ((s.shop.BCSInstallPrice ?? 0) > 0)
                                        {
                                            basicPrice = (s.shop.BCSInstallPrice ?? 0);
                                        }
                                        else if (BCSInstallCityTierList.Contains(s.order.CityTier.ToUpper()))
                                        {
                                            basicPrice = bcsBasicInstallPrice;
                                        }
                                        else
                                        {
                                            basicPrice = 0;
                                        }
                                    }
                                    else if (s.CategoryName.Contains("常规-非活动"))
                                    {
                                        if (BCSInstallCityTierList.Contains(s.order.CityTier.ToUpper()))
                                        {
                                            basicPrice = bcsBasicInstallPrice;
                                        }
                                        else
                                        {
                                            basicPrice = 0;
                                        }
                                    }
                                    else
                                    {
                                        if ((s.shop.BasicInstallPrice ?? 0) > 0)
                                        {
                                            basicPrice = (s.shop.BasicInstallPrice ?? 0);
                                        }
                                        else
                                            basicPrice += GetBasicInstallPrice(s.order.InstallPriceMaterialSupport);
                                    }
                                }
                                installPriceShopModel.BasicPrice = basicPrice;
                                basicInstallPrice += basicPrice;
                                isvalid = true;
                            }
                            if (!cbIsSecondInstall.Checked)
                            {
                                //橱窗安装费
                                if (windowSheetShopIdList.Contains(s.shop.Id) && !windowInstallPriceShop.Contains(s.shop.Id))
                                {
                                    windowInstallPriceShop.Add(s.shop.Id);
                                    windowPrice += GetWindowInstallPrice(s.order.InstallPriceMaterialSupport);
                                    windowInstallPrice += windowPrice;
                                    installPriceShopModel.WindowPrice = windowPrice;
                                }

                            }
                            if (isvalid)
                                installPriceShopList.Add(installPriceShopModel);
                        });
                        if (!cbIsSecondInstall.Checked)
                        {
                            //户外订单(同一个店，如果有2个以上的户外位置订单，按最高算)
                            //var oohOrderList = list.Where(s => s.pop != null && (s.order.Sheet == "户外" || s.order.Sheet.ToLower() == "ooh") && (s.pop.OOHInstallPrice ?? 0) > 0).Select(s => new { ShopId = s.shop.Id, OOHInstallPrice = s.pop.OOHInstallPrice ?? 0 }).ToList();
                            //if (oohOrderList.Any())
                            //{
                            //    var oohList = (from order in oohOrderList
                            //                   group order by new
                            //                   {
                            //                       order.ShopId
                            //                   } into item
                            //                   select new
                            //                   {
                            //                       item.Key.ShopId,
                            //                       OOHInstallPrice = item.Max(s => s.OOHInstallPrice)
                            //                   }).ToList();
                            //    var finaloohList = (from order in oohOrderList
                            //                        join ooh in oohList
                            //                        on new { order.ShopId, order.OOHInstallPrice } equals new { ooh.ShopId, ooh.OOHInstallPrice }
                            //                        select new
                            //                        {

                            //                            ooh.ShopId,
                            //                            ooh.OOHInstallPrice
                            //                        }).Distinct().ToList();
                            //    if (finaloohList.Any())
                            //    {
                            //        finaloohList.ForEach(f =>
                            //        {
                            //            var model0 = installPriceShopList.Where(sh => sh.ShopId == f.ShopId).FirstOrDefault();
                            //            if (model0 != null)
                            //            {
                            //                int index = installPriceShopList.IndexOf(model0);
                            //                model0.OOHPrice = f.OOHInstallPrice;
                            //                installPriceShopList[index] = model0;
                            //            }
                            //        });
                            //    }
                            //    oohInstallPrice = finaloohList.Sum(s => s.OOHInstallPrice);

                            //}


                            var oohOrderList0 = list.Where(s => (s.order.Sheet != null && (s.order.Sheet == "户外" || s.order.Sheet.ToLower() == "ooh"))).ToList();
                            List<int> oohOrderShopIdList = oohOrderList0.Select(s => s.shop.Id).Distinct().ToList();
                            oohPOPList = oohPOPList.Where(s => oohOrderShopIdList.Contains(s.ShopId ?? 0)).ToList();
                            if (oohOrderList0.Any())
                            {
                                Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                                oohOrderList0.ForEach(s =>
                                {
                                    decimal price = 0;
                                    if (!string.IsNullOrWhiteSpace(s.order.GraphicNo))
                                    {
                                        price = oohPOPList.Where(p => p.ShopId == s.shop.Id && p.GraphicNo.ToLower() == s.order.GraphicNo.ToLower()).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                                    }
                                    else
                                        price = oohPOPList.Where(p => p.ShopId == s.shop.Id && p.GraphicLength == s.order.GraphicLength && p.GraphicWidth == s.order.GraphicWidth).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                                    if (oohPriceDic.Keys.Contains(s.shop.Id))
                                    {
                                        if (oohPriceDic[s.shop.Id] < price)
                                        {
                                            oohPriceDic[s.shop.Id] = price;
                                        }
                                    }
                                    else
                                        oohPriceDic.Add(s.shop.Id, price);
                                });
                                if (oohPriceDic.Keys.Count > 0)
                                {
                                    foreach (KeyValuePair<int, decimal> item in oohPriceDic)
                                    {
                                        var model0 = installPriceShopList.Where(sh => sh.ShopId == item.Key).FirstOrDefault();
                                        if (model0 != null)
                                        {
                                            int index = installPriceShopList.IndexOf(model0);
                                            model0.OOHPrice = item.Value;
                                            installPriceShopList[index] = model0;
                                            oohInstallPrice += item.Value;
                                        }
                                    }
                                }
                            }

                        }
                        decimal total = basicInstallPrice + windowInstallPrice + oohInstallPrice;
                        InstallPriceDetailBLL installPriceBll = new InstallPriceDetailBLL();

                        InstallPriceDetail model = null;
                        model = installPriceBll.GetList(s => s.GuidanceId == guidanceId && s.SubjectId == sid && s.AddType == 2).FirstOrDefault();
                        if (model != null)
                        {

                            if (selectRegion.Any())
                            {
                                model.SelectRegion = StringHelper.ListToString(selectRegion);
                            }
                            else
                                model.SelectRegion = "";
                            model.SelectSubjectId = sid.ToString();
                            installPriceBll.Update(model);

                        }
                        else
                        {
                            try
                            {
                                model = new InstallPriceDetail();
                                model.AddType = 2;
                                model.AddDate = DateTime.Now;
                                model.GuidanceId = guidanceId;
                                //model.Price = total;
                                model.SelectPOSScale = "";
                                model.SelectRegion = StringHelper.ListToString(selectRegion);
                                model.SelectSubjectId = StringHelper.ListToString(selectSubject);
                                model.SelectSubjectTypeId = sid.ToString();
                                model.SubjectId = sid;
                                installPriceBll.Add(model);
                            }
                            catch (Exception ex)
                            { }

                        }
                        if (installPriceShopList.Any())
                        {


                            installPriceShopList.ForEach(l =>
                            {
                                InstallPriceShopInfo model1 = new InstallPriceShopInfo();
                                model1 = l;
                                model1.InstallDetailId = model.Id;
                                model1.SubjectId = sid;
                                model1.AddType = 2;
                                installShopBll.Add(model1);
                            });
                        }



                    }


                });
                BindRegion();
                BindSubjectType1();
                BindSubject1();
                BindData1();
            }
        }
        */
        protected void btnSubmit1_Click(object sender, EventArgs e)
        {
            SubmitBySubjectPrice();
        }
        /// <summary>
        /// 安装费计算（新）
        /// </summary>
        void SubmitBySubjectPrice()
        {
            bool isOk = true;
            List<string> selectRegion = new List<string>();
            List<int> selectSubject = new List<int>();
            
            foreach (ListItem li in cblSecondRegion.Items)
            {
                if (li.Selected && !selectRegion.Contains(li.Value.ToLower()))
                    selectRegion.Add(li.Value.ToLower());
            }


            foreach (ListItem li in cblSecondSubjectName.Items)
            {
                if (li.Selected && !selectSubject.Contains(int.Parse(li.Value)))
                    selectSubject.Add(int.Parse(li.Value));
            }

            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            List<Shop> shopList = new List<Shop>();
            if (Session["orderDetailInstallPrice"] != null)
            {
                orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (Session["shopInstallPrice"] != null)
            {
                shopList = Session["shopInstallPrice"] as List<Shop>;
            }
            var totalOrderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              join subjectType1 in CurrentContext.DbContext.SubjectType
                              on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                              from subjectType in typeTemp.DefaultIfEmpty()
                              join shop in shopList
                              on order.ShopId equals shop.Id
                              join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                              on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                              from subjectCategory in categortTemp.DefaultIfEmpty()
                              where (selectRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? selectRegion.Contains(subject.PriceBlongRegion.ToLower()) : selectRegion.Contains(order.Region.ToLower())) : 1 == 1)
                              && selectSubject.Contains(order.SubjectId??0)
                              select new
                              {
                                  order,
                                  subject,
                                  shop,
                                  CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                              }).ToList();

            if (totalOrderList0.Any())
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                        InstallPriceDetailBLL installPriceBll = new InstallPriceDetailBLL();
                        OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
                        OutsourceOrderDetail outsourceOrderDetailModel;
                        selectSubject.ForEach(sid => {
                            var oneSubjectOrderList = totalOrderList0.Where(s => s.order.SubjectId == sid).ToList();
                            if (oneSubjectOrderList.Any())
                            {
                                #region 保存/更新InstallPriceDetail
                                int subjectId = int.Parse(ddlSubject.SelectedValue);
                                InstallPriceDetail model = installPriceBll.GetList(s => s.GuidanceId == guidanceId && s.SubjectId == sid && s.AddType == 2).FirstOrDefault();
                                if (model != null)
                                {
                                    if (selectRegion.Any())
                                    {

                                        List<string> Regions = new List<string>();
                                        if (!string.IsNullOrWhiteSpace(model.SelectRegion))
                                        {
                                            Regions = StringHelper.ToStringList(model.SelectRegion, ',');
                                        }
                                        selectRegion.ForEach(s =>
                                        {
                                            if (!Regions.Contains(s))
                                            {
                                                Regions.Add(s);
                                            }
                                        });
                                        model.SelectRegion = StringHelper.ListToString(Regions);
                                    }
                                    model.SelectSubjectId = sid.ToString();
                                    installPriceBll.Update(model);

                                }
                                else
                                {
                                    model = new InstallPriceDetail();
                                    model.AddType = 2;
                                    model.AddDate = DateTime.Now;
                                    model.GuidanceId = guidanceId;
                                    model.SelectRegion = StringHelper.ListToString(selectRegion);
                                    model.SelectSubjectId = sid.ToString();
                                    model.SubjectId = sid;
                                    installPriceBll.Add(model);
                                }
                                #endregion
                                List<string> BCSInstallCityTierList = new List<string>();
                                decimal bcsBasicInstallPrice = 150;//三叶草t1-t3安装费
                                if (!BCSInstallCityTierList.Any())
                                {
                                    string BCSInstallCityTier = string.Empty;
                                    try
                                    {
                                        BCSInstallCityTier = ConfigurationManager.AppSettings["BCSBasicInstallPrice"];
                                        if (!string.IsNullOrWhiteSpace(BCSInstallCityTier))
                                        {
                                            string[] str = BCSInstallCityTier.Split(':');
                                            BCSInstallCityTierList = StringHelper.ToStringList(str[0], ',', LowerUpperEnum.ToUpper);
                                            bcsBasicInstallPrice = StringHelper.IsDecimal(str[1]);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                                List<Shop> shopList0 = oneSubjectOrderList.Select(s => s.shop).Distinct().ToList();
                                List<int> shopIdList = shopList0.Select(s => s.Id).ToList();
                                var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);
                                //删除外协订单里面的安装费
                                outsourceOrderDetailBll.Delete(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0) && s.SubjectId==sid && s.OrderType==(int)OrderTypeEnum.安装费);
                                shopList0.ForEach(shop =>
                                {


                                    bool isBCSSubject = true;
                                    bool isGeneric = true;
                                    //基础安装费
                                    decimal basicInstallPrice = 0;
                                    decimal specialBasicInstallPrice = 0;
                                    //橱窗安装费
                                    decimal windowInstallPrice = 0;
                                    //OOH安装费
                                    decimal oohInstallPrice = 0;

                                    //外协基础安装费
                                    decimal outsourceBasicInstallPrice = 0;
                                    decimal outsourceSpecialBasicInstallPrice = 0;
                                    //外协OOH安装费
                                    decimal outsourceOOHInstallPrice = 0;

                                    string materialSupport = string.Empty;
                                    string POSScale = string.Empty;

                                    string outRemark = "单独安装费";

                                    List<string> materialSupportList = new List<string>();
                                    var oneShopOrderList = oneSubjectOrderList.Where(s => s.order.ShopId == shop.Id).ToList();
                                    if (oneShopOrderList.Any())
                                    {
                                        oneShopOrderList.ForEach(s =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(s.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(s.order.InstallPriceMaterialSupport.ToLower()))
                                            {
                                                materialSupportList.Add(s.order.InstallPriceMaterialSupport.ToLower());
                                            }
                                            if (string.IsNullOrWhiteSpace(POSScale) && !string.IsNullOrWhiteSpace(s.order.InstallPricePOSScale))
                                                POSScale = s.order.InstallPricePOSScale;
                                            if (s.subject.CornerType != "三叶草")
                                                isBCSSubject = false;
                                            if (!s.CategoryName.Contains("常规-非活动"))
                                                isGeneric = false;
                                        });
                                        List<FinalOrderDetailTemp> oohOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower() == "ooh" || s.order.Sheet == "户外")).Select(s => s.order).ToList();
                                        List<FinalOrderDetailTemp> windowOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower().Contains("橱窗") || s.order.Sheet.ToLower().Contains("window"))).Select(s => s.order).ToList();


                                        #region 店内安装费
                                        
                                        //按照级别，获取基础安装费
                                        materialSupportList.ForEach(ma =>
                                        {
                                            decimal basicInstallPrice0 = GetBasicInstallPrice(ma);
                                            if (basicInstallPrice0 > basicInstallPrice)
                                            {
                                                basicInstallPrice = basicInstallPrice0;
                                                materialSupport = ma;
                                            }
                                        });
                                        outsourceBasicInstallPrice=GetOutsourceBasicInstallPrice(materialSupport);
                                        if ((shop.BasicInstallPrice ?? 0) > 0)
                                        {
                                            specialBasicInstallPrice = (shop.BasicInstallPrice ?? 0);
                                        }
                                        //外协基础安装费
                                        if ((shop.OutsourceInstallPrice ?? 0) > 0)
                                        {
                                            outsourceSpecialBasicInstallPrice = (shop.OutsourceInstallPrice ?? 0);
                                        }
                                        if (isBCSSubject)
                                        {
                                            if ((shop.BCSInstallPrice ?? 0) > 0)
                                            {
                                                specialBasicInstallPrice = (shop.BCSInstallPrice ?? 0);
                                            }
                                            else if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                                            {
                                                basicInstallPrice = bcsBasicInstallPrice;
                                            }
                                            else
                                            {
                                                basicInstallPrice = 0;
                                            }
                                            //外协三叶草安装费
                                            if ((shop.OutsourceBCSInstallPrice ?? 0) > 0)
                                            {
                                                outsourceSpecialBasicInstallPrice = (shop.OutsourceBCSInstallPrice ?? 0);
                                            }
                                            else if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                                            {
                                                outsourceBasicInstallPrice = bcsBasicInstallPrice;
                                            }
                                            else
                                            {
                                                outsourceBasicInstallPrice = 0;
                                            }
                                        }
                                        else if (isGeneric)
                                        {
                                            if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                                            {
                                                basicInstallPrice = bcsBasicInstallPrice;
                                            }
                                            else
                                            {
                                                basicInstallPrice = 0;
                                            }
                                            outsourceBasicInstallPrice = basicInstallPrice;
                                        }
                                        if (cbSecondIsSecondInstall.Checked)
                                        {
                                            outsourceBasicInstallPrice=basicInstallPrice = 150;//如果是二次安装，基础安装费150，其他不算
                                            outRemark = "二次安装费";
                                        }
                                        basicInstallPrice = specialBasicInstallPrice > 0 ? specialBasicInstallPrice : basicInstallPrice;
                                        
                                        outsourceBasicInstallPrice = outsourceSpecialBasicInstallPrice > 0 ? outsourceSpecialBasicInstallPrice : outsourceBasicInstallPrice;
                                        #endregion
                                        #region 橱窗安装
                                        if (windowOrderList.Any())
                                        {
                                            windowInstallPrice = GetWindowInstallPrice(materialSupport);
                                        }
                                        #endregion
                                        #region OOH安装费
                                        if (oohOrderList.Any())
                                        {

                                            oohOrderList.ForEach(s =>
                                            {
                                                decimal price = 0;
                                                decimal outprice = 0;
                                                if (!string.IsNullOrWhiteSpace(s.GraphicNo))
                                                {
                                                    price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.GraphicNo.ToLower()).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();
                                                    outprice = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();
                                                }
                                                else
                                                {
                                                    price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.GraphicLength && p.GraphicWidth == s.GraphicWidth).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();
                                                    outprice = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.GraphicLength && p.GraphicWidth == s.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();
                                                }
                                                if (price > oohInstallPrice)
                                                {
                                                    oohInstallPrice = price;
                                                }
                                                if (outprice > outsourceOOHInstallPrice)
                                                {
                                                    outsourceOOHInstallPrice = outprice;
                                                }
                                            });
                                        }
                                        #endregion
                                        #region 保存InstallPriceShopInfo
                                       
                                        InstallPriceShopInfo installPriceShopModel = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == shop.Id && i.SubjectId==sid && i.AddType==2).FirstOrDefault();
                                        if (installPriceShopModel == null)
                                        {
                                            installPriceShopModel = new InstallPriceShopInfo();
                                            installPriceShopModel.BasicPrice = basicInstallPrice;
                                            installPriceShopModel.OOHPrice = oohInstallPrice;
                                            installPriceShopModel.WindowPrice = windowInstallPrice;
                                            installPriceShopModel.InstallDetailId = model.Id;
                                            installPriceShopModel.GuidanceId = guidanceId;
                                            installPriceShopModel.SubjectId = sid;
                                            installPriceShopModel.ShopId = shop.Id;
                                            installPriceShopModel.MaterialSupport = materialSupport;
                                            installPriceShopModel.POSScale = POSScale;
                                            installPriceShopModel.AddType = 2;
                                            installShopBll.Add(installPriceShopModel);
                                        }
                                        #endregion

                                        //保存外协安装费
                                        //outsourceOrderDetailModel = outsourceOrderDetailBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == shop.Id && i.SubjectId == sid && i.OrderType==(int)OrderTypeEnum.安装费).FirstOrDefault();
                                        //outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        if (outsourceOOHInstallPrice > 0 && (shop.OOHInstallOutsourceId ?? 0) > 0)
                                        {
                                            //如果有单独的户外安装外协
                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = oneShopOrderList[0].order.AgentName;
                                            outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = oneShopOrderList[0].order.Channel;
                                            outsourceOrderDetailModel.City = oneShopOrderList[0].order.City;
                                            outsourceOrderDetailModel.CityTier = oneShopOrderList[0].order.CityTier;
                                            outsourceOrderDetailModel.Contact = shop.Contact1;
                                            outsourceOrderDetailModel.Format = oneShopOrderList[0].order.Format;
                                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                                            outsourceOrderDetailModel.GraphicWidth = 0;
                                            outsourceOrderDetailModel.GuidanceId = guidanceId;
                                            outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = oneShopOrderList[0].order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                                            outsourceOrderDetailModel.MaterialSupport = string.Empty;
                                            outsourceOrderDetailModel.OrderGender = string.Empty;
                                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                            outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                            outsourceOrderDetailModel.POPName = string.Empty;
                                            outsourceOrderDetailModel.POPType = string.Empty;
                                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                                            outsourceOrderDetailModel.POSScale = POSScale;
                                            outsourceOrderDetailModel.Province = shop.ProvinceName;
                                            outsourceOrderDetailModel.Quantity = 1;
                                            outsourceOrderDetailModel.Region = shop.RegionName;
                                            outsourceOrderDetailModel.Remark = outRemark+"(户外安装费)";
                                            outsourceOrderDetailModel.Sheet = string.Empty;
                                            outsourceOrderDetailModel.ShopId = shop.Id;
                                            outsourceOrderDetailModel.ShopName = oneShopOrderList[0].order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = sid;
                                            outsourceOrderDetailModel.Tel = shop.Tel1;
                                            outsourceOrderDetailModel.TotalArea = 0;
                                            outsourceOrderDetailModel.WindowDeep = 0;
                                            outsourceOrderDetailModel.WindowHigh = 0;
                                            outsourceOrderDetailModel.WindowSize = string.Empty;
                                            outsourceOrderDetailModel.WindowWide = 0;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                            outsourceOrderDetailModel.PayOrderPrice = outsourceOOHInstallPrice;
                                            outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                            outsourceOrderDetailModel.PayOOHInstallPrice = outsourceOOHInstallPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = string.Empty;
                                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                            outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = shop.OOHInstallOutsourceId;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                            outsourceOOHInstallPrice = 0;
                                        }
                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].order.AgentCode;
                                        outsourceOrderDetailModel.AgentName = oneShopOrderList[0].order.AgentName;
                                        outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].order.BusinessModel;
                                        outsourceOrderDetailModel.Channel = oneShopOrderList[0].order.Channel;
                                        outsourceOrderDetailModel.City = oneShopOrderList[0].order.City;
                                        outsourceOrderDetailModel.CityTier = oneShopOrderList[0].order.CityTier;
                                        outsourceOrderDetailModel.Contact = oneShopOrderList[0].order.Contact;
                                        outsourceOrderDetailModel.Format = oneShopOrderList[0].order.Format;
                                        outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                        outsourceOrderDetailModel.GraphicNo = string.Empty;
                                        outsourceOrderDetailModel.GraphicWidth = 0;
                                        outsourceOrderDetailModel.GuidanceId = guidanceId;
                                        outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].order.IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].order.BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = oneShopOrderList[0].order.LocationType;
                                        outsourceOrderDetailModel.MachineFrame = string.Empty;
                                        outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                        outsourceOrderDetailModel.OrderGender = string.Empty;
                                        outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                        outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                        outsourceOrderDetailModel.POPName = string.Empty;
                                        outsourceOrderDetailModel.POPType = string.Empty;
                                        outsourceOrderDetailModel.PositionDescription = string.Empty;
                                        outsourceOrderDetailModel.POSScale = POSScale;
                                        outsourceOrderDetailModel.Province = shop.ProvinceName;
                                        outsourceOrderDetailModel.Quantity = 1;
                                        outsourceOrderDetailModel.Region = shop.RegionName;
                                        outsourceOrderDetailModel.Remark = outRemark;
                                        outsourceOrderDetailModel.Sheet = string.Empty;
                                        outsourceOrderDetailModel.ShopId = shop.Id;
                                        outsourceOrderDetailModel.ShopName = oneShopOrderList[0].order.ShopName;
                                        outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].order.ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].order.ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = sid;
                                        outsourceOrderDetailModel.Tel = shop.Tel1;
                                        outsourceOrderDetailModel.TotalArea = 0;
                                        outsourceOrderDetailModel.WindowDeep = 0;
                                        outsourceOrderDetailModel.WindowHigh = 0;
                                        outsourceOrderDetailModel.WindowSize = string.Empty;
                                        outsourceOrderDetailModel.WindowWide = 0;
                                        outsourceOrderDetailModel.ReceiveOrderPrice = (basicInstallPrice+oohInstallPrice+windowInstallPrice);
                                        outsourceOrderDetailModel.PayOrderPrice = outsourceBasicInstallPrice + outsourceOOHInstallPrice;
                                        outsourceOrderDetailModel.PayBasicInstallPrice = outsourceBasicInstallPrice;
                                        outsourceOrderDetailModel.PayOOHInstallPrice = outsourceOOHInstallPrice;
                                        outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                        outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                        outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                        outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].order.CSUserId;
                                        outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                    }
                                });
                            }

                            
                        });
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        isOk = false;
                    }
                }
                if (isOk)
                {
                    //Response.Redirect(string.Format("SubmitInstallPrice.aspx?itemid={0}", guidanceId), false);
                    BindData1();
                }
            }
        }




        protected void Button1_Click(object sender, EventArgs e)
        {
            BindSubjectType1();
            BindSubject1();
            BindData1();
        }
    }
}
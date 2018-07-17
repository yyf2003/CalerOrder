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
        // bool isRegionOrder = false;
        List<string> myRegionList = new List<string>();
        List<string> cityCierList = new List<string>() { "T1", "T2", "T3" };
        List<int> myInstallShopIdList = new List<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemid"] != null)
                guidanceId = int.Parse(Request.QueryString["itemid"]);
            //if (CurrentUser.RoleId == 6)
            //{
            //    //isRegionOrder = true;
            //    myRegionList = GetResponsibleRegion;
            //    StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
            //    labTitle.Text = "—分区订单";
            //}
            if (!IsPostBack)
            {
                Session["activityOrderListIP"] = null;
                Session["genericOrderListIP"] = null;
                Session["totalOrderListIP"] = null;
                Session["subjectListIP"] = null;
                BindGuidance();
                BindRegion();
                BindSubjectType();
                BindSubject();
                BindCityTier();
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

        void BindGuidance_1()
        {
            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(guidanceId);
            if (model != null)
            {
                labGuidanceName.Text = model.ItemName;
                labSecondGuidanceName.Text = model.ItemName;

                //string redisOrderKey = "InstallPriceOrderList" + guidanceId;

                //List<FinalOrderDetailTemp> finalOrderDetailTempList = RedisHelper.Get<List<FinalOrderDetailTemp>>(redisOrderKey);
                List<FinalOrderDetailTemp> finalOrderDetailTempList = new FinalOrderDetailTempBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength != null && s.GraphicLength > 0 && s.GraphicWidth != null && s.GraphicWidth > 0) || (s.OrderType == (int)OrderTypeEnum.道具)));
                //东区的户外店不自动算安装费，手动下安装费
                List<int> terrexIdList = finalOrderDetailTempList.Where(s => s.Channel != null && s.Channel.ToLower().Contains("terrex") && s.Region != null && s.Region.ToLower().Contains("east")).Select(s => s.Id).ToList();
                finalOrderDetailTempList = finalOrderDetailTempList.Where(s => !terrexIdList.Contains(s.Id)).ToList();

                //活动订单
                List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
                //常规订单
                List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();

                //全部订单
                var orderList = (from order in finalOrderDetailTempList
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                 on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                                 from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                                 select new { order, shop, subject, subjectCategory }).ToList();
                if (orderList.Any())
                {
                    genericOrderList = orderList.Where(s => s.subjectCategory != null && s.subjectCategory.CategoryName.Contains("常规-非活动") && ((!s.shop.RegionName.ToLower().Contains("west") && cityCierList.Contains(s.shop.CityTier)) || (s.shop.RegionName.ToLower().Contains("west") && s.shop.IsInstall == "Y"))).Select(s => s.order).ToList();
                    List<int> genericOrderIdList = new List<int>();
                    if (genericOrderList.Any())
                    {
                        genericOrderIdList = genericOrderList.Select(s => s.Id).ToList();
                    }
                    activityOrderList = orderList.Where(s => !genericOrderIdList.Contains(s.order.Id) && (s.shop.IsInstall == "Y" || s.shop.BCSIsInstall == "Y")).Select(s => s.order).ToList();



                }
                //已提交的安装费
                var installPriceShopInfoList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId && (s.AddType == null || s.AddType == 1));
                if (installPriceShopInfoList.Any())
                {
                    List<int> assginShopIdList = installPriceShopInfoList.Where(s => s.SubjectType == null || s.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    if (assginShopIdList.Any())
                    {
                        Session["assginActivityShopId"] = assginShopIdList;
                        activityOrderList = activityOrderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }
                    assginShopIdList = installPriceShopInfoList.Where(s => s.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    if (assginShopIdList.Any())
                    {
                        Session["assginGenericShopId"] = assginShopIdList;
                        genericOrderList = genericOrderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }

                }
                Session["activityOrderOrderList"] = activityOrderList;
                Session["genericOrderList"] = genericOrderList;
                Session["subjectList"] = orderList.Select(s => s.subject).Distinct().ToList();
            }
        }

        void BindGuidance_old()
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
                //List<FinalOrderDetailTemp> finalOrderDetailTempList = RedisHelper.Get<List<FinalOrderDetailTemp>>("InstallPriceOrderList").Where(s => s.GuidanceId == guidanceId).ToList();
                //东区的户外店不自动算安装费，手动下安装费
                List<int> terrexIdList = finalOrderDetailTempList.Where(s => s.Channel != null && s.Channel.ToLower().Contains("terrex") && s.Region != null && s.Region.ToLower().Contains("east")).Select(s => s.Id).ToList();
                finalOrderDetailTempList = finalOrderDetailTempList.Where(s => !terrexIdList.Contains(s.Id)).ToList();


                List<FinalOrderDetailTemp> totalLeftOrderList = new List<FinalOrderDetailTemp>();
                //活动订单
                List<FinalOrderDetailTemp> notGenericOrderList = new List<FinalOrderDetailTemp>();
                //常规订单
                List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();
                var orderList = (from order in finalOrderDetailTempList
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                 on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                                 from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                                 where
                                 (subject.IsDelete == null || subject.IsDelete == false)
                                 && subject.ApproveState == 1
                                 && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                 && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                                 && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                                 && (subject.IsSecondInstall ?? false) == false
                                 && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && order.BCSIsInstall != null && order.BCSIsInstall == "Y"))
                                 //&& ((order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草") && (subjectCategory == null || (subjectCategory != null && !subjectCategory.CategoryName.Contains("常规-非活动")))) || (subject.CornerType == "三叶草" && order.BCSIsInstall == "Y") || (subjectCategory != null && subjectCategory.CategoryName.Contains("常规-非活动") && order.GenericIsInstall == "Y"))

                                 select new { order, shop, subject, subjectCategory }).ToList();
                if (orderList.Any())
                {
                    genericOrderList = orderList.Where(s => s.subjectCategory != null && s.subjectCategory.CategoryName.Contains("常规-非活动")).Select(s => s.order).ToList();
                    List<int> orderIdList0 = new List<int>();
                    if (genericOrderList.Any())
                    {
                        orderIdList0 = genericOrderList.Select(s => s.Id).ToList();
                    }
                    notGenericOrderList = orderList.Where(s => !orderIdList0.Contains(s.order.Id)).Select(s => s.order).ToList();
                    Session["orderDetailInstallPrice"] = orderList.Select(s => s.order).ToList();
                    Session["notGenericOrderInstallPrice"] = notGenericOrderList;
                    Session["genericOrderInstallPrice"] = genericOrderList;
                    Session["shopInstallPrice"] = orderList.Select(s => s.shop).Distinct().ToList();
                    Session["subjectInstallPrice"] = orderList.Select(s => s.subject).Distinct().ToList();
                }
                //List<int> assginShopIdList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId && (s.AddType==null || s.AddType==1)).Select(s => s.ShopId ?? 0).Distinct().ToList();
                var installPriceShopInfoList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId && (s.AddType == null || s.AddType == 1));
                if (installPriceShopInfoList.Any())
                {
                    List<int> assginShopIdList = installPriceShopInfoList.Where(s => s.SubjectType == null || s.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    if (assginShopIdList.Any())
                    {
                        Session["assginShopIdNotGeneric"] = assginShopIdList;
                        notGenericOrderList = notGenericOrderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }
                    assginShopIdList = installPriceShopInfoList.Where(s => s.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    if (assginShopIdList.Any())
                    {
                        Session["assginShopIdGeneric"] = assginShopIdList;
                        genericOrderList = genericOrderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }

                }
                totalLeftOrderList = notGenericOrderList.Concat(genericOrderList).ToList();
                Session["totalOrderInstallPrice"] = totalLeftOrderList;
            }
        }

        void BindGuidance()
        {
            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(guidanceId);
            if (model != null)
            {
                labGuidanceName.Text = model.ItemName;
                labSecondGuidanceName.Text = model.ItemName;

                string redisOrderKey = "InstallPriceOrderList" + guidanceId;

                List<FinalOrderDetailTemp> finalOrderDetailTempList = RedisHelper.Get<List<FinalOrderDetailTemp>>(redisOrderKey);
                if (finalOrderDetailTempList == null)
                    finalOrderDetailTempList = new List<FinalOrderDetailTemp>();

                //List<FinalOrderDetailTemp> finalOrderDetailTempList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                //                                                       join subject in CurrentContext.DbContext.Subject
                //                                                       on order.SubjectId equals subject.Id
                //                                                       where (order.IsDelete == null || order.IsDelete == false)
                //                                                       && (subject.IsDelete == null || subject.IsDelete == false)
                //                                                       && subject.ApproveState == 1
                //                                                       && order.GuidanceId == guidanceId
                //                                                       && order.OrderType==(int)OrderTypeEnum.POP
                //                                                       select order).ToList();

                //东区的户外店不自动算安装费，手动下安装费
                List<int> terrexIdList = finalOrderDetailTempList.Where(s => s.Channel != null && s.Channel.ToLower().Contains("terrex") && s.Region != null && s.Region.ToLower().Contains("east")).Select(s => s.Id).ToList();
                finalOrderDetailTempList = finalOrderDetailTempList.Where(s => !terrexIdList.Contains(s.Id)).ToList();

                //活动订单
                List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
                //常规订单
                List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();

                List<Subject> subjectList = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && s.SubjectType != (int)SubjectTypeEnum.二次安装 && s.SubjectType != (int)SubjectTypeEnum.费用订单 && s.SubjectType != (int)SubjectTypeEnum.新开店安装费);

                //全部订单
                var orderList = (from order in finalOrderDetailTempList
                                 join subject in subjectList
                                 on order.SubjectId equals subject.Id
                                 //join shop in CurrentContext.DbContext.Shop
                                 //on order.ShopId equals shop.Id
                                 join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                 on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                                 from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                                 where order.GuidanceId == guidanceId
                                
                                 select new { order, subject, subjectCategory }).ToList();
                if (orderList.Any())
                {
                    genericOrderList = orderList.Where(s => s.subjectCategory != null && s.subjectCategory.CategoryName.Contains("常规-非活动")).Select(s => s.order).ToList();
                    
                    List<int> genericOrderIdList = new List<int>();
                    if (genericOrderList.Any())
                    {
                        genericOrderIdList = genericOrderList.Select(s => s.Id).ToList();
                        genericOrderList = genericOrderList.Where(s => ((!s.Region.ToLower().Contains("west") && cityCierList.Contains(s.CityTier) && s.IsInstall == "Y") || (s.Region.ToLower().Contains("west") && s.IsInstall == "Y"))).ToList();
                    
                    }
                    activityOrderList = orderList.Where(s => !genericOrderIdList.Contains(s.order.Id) && (s.order.IsInstall == "Y" || s.order.BCSIsInstall == "Y")).Select(s => s.order).ToList();

                }
                //已提交的安装费
                var installPriceShopInfoList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId && (s.AddType == null || s.AddType == 1));
                if (installPriceShopInfoList.Any())
                {
                    List<int> assginShopIdList = installPriceShopInfoList.Where(s => s.SubjectType == null || s.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    if (assginShopIdList.Any())
                    {
                        Session["assginActivityShopIdIP"] = assginShopIdList;
                        activityOrderList = activityOrderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }
                    assginShopIdList = installPriceShopInfoList.Where(s => s.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费).Select(s => s.ShopId ?? 0).Distinct().ToList();
                    if (assginShopIdList.Any())
                    {
                        Session["assginGenericShopIdIP"] = assginShopIdList;
                        genericOrderList = genericOrderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
                    }

                }
                Session["activityOrderListIP"] = activityOrderList;
                Session["genericOrderListIP"] = genericOrderList;
                Session["totalOrderListIP"] = finalOrderDetailTempList;
                Session["subjectListIP"] = orderList.Select(s => s.subject).Distinct().ToList();
            }
        }

        void BindRegion_old()
        {
            cblRegion.Items.Clear();
            cblSecondRegion.Items.Clear();

            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            if (Session["totalOrderInstallPrice"] != null)
            {
                orderList = Session["totalOrderInstallPrice"] as List<FinalOrderDetailTemp>;

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
            //if (Session["assginShopIdInstallPrice"] != null)
            //{
            //    List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
            //    if (assginShopIdList.Any())
            //    {
            //        orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
            //    }
            //}
            var orderList0 = (from order in orderList
                              join subject in subjectList
                              on order.SubjectId equals subject.Id
                              select new
                              {
                                  Region = (subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? subject.PriceBlongRegion : order.Region
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

        void BindRegion()
        {
            cblRegion.Items.Clear();
            cblSecondRegion.Items.Clear();
            //活动订单
            List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
            //常规订单
            List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();
            
            if (Session["activityOrderListIP"] != null)
            {
                activityOrderList = Session["activityOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["genericOrderListIP"] != null)
            {
                genericOrderList = Session["genericOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            List<string> regionList = new List<string>();
            if (activityOrderList.Any())
            {
                regionList = activityOrderList.Select(s => s.Region ?? "").Distinct().ToList();
            }
            if (genericOrderList.Any())
            {
                List<string> regionList0 = genericOrderList.Select(s => s.Region ?? "").Distinct().ToList();
                regionList = regionList.Union(regionList0).ToList();
            }
            if (regionList.Any())
            {
                bool hasEmpty = false;
                if (regionList.Contains(""))
                {
                    hasEmpty = true;
                    regionList.Remove("");
                }
                regionList.OrderBy(s => s).ToList().ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblRegion.Items.Add(li);

                });
                if (hasEmpty)
                {
                    ListItem li = new ListItem();
                    li.Value = "空";
                    li.Text = "空";
                    cblRegion.Items.Add(li);
                }
            }
        }

        void BindSubjectType_old()
        {
            cblSubjectType.Items.Clear();

            List<string> regions = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }
            #region 旧的
            //List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            //List<Subject> subjectList = new List<Subject>();
            //if (Session["orderDetailInstallPrice"] != null)
            //{
            //    orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            //}
            //if (Session["subjectInstallPrice"] != null)
            //{
            //    subjectList = Session["subjectInstallPrice"] as List<Subject>;
            //}
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
            //var orderList0 = (from order in orderList
            //                  join subject in subjectList
            //                  on order.SubjectId equals subject.Id
            //                  join subjectType1 in CurrentContext.DbContext.SubjectType
            //                 on subject.SubjectTypeId equals subjectType1.Id into typeTemp
            //                  from subjectType in typeTemp.DefaultIfEmpty()
            //                  where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
            //                  select new
            //                  {
            //                      order,
            //                      subject.SubjectTypeId,
            //                      SubjectTypeName = subjectType != null ? subjectType.SubjectTypeName : ""
            //                  }).ToList();

            //if (orderList0.Any())
            //{
            //    var list = orderList0.Select(s => new { s.SubjectTypeId, s.SubjectTypeName }).Distinct().ToList();
            //    List<int> typeIdList = new List<int>();
            //    bool isEmpty = false;
            //    list.ForEach(s =>
            //    {
            //        if (s.SubjectTypeId != null && s.SubjectTypeId != 0)
            //        {
            //            if (!typeIdList.Contains(s.SubjectTypeId ?? 0))
            //            {
            //                ListItem li = new ListItem();
            //                li.Value = (s.SubjectTypeId ?? 0).ToString();
            //                li.Text = s.SubjectTypeName + "&nbsp;&nbsp;";
            //                cblSubjectType.Items.Add(li);
            //            }
            //        }
            //        else
            //            isEmpty = true;
            //    });
            //    if (isEmpty)
            //    {
            //        ListItem li = new ListItem();
            //        li.Value = "0";
            //        li.Text = "空";
            //        cblSubjectType.Items.Add(li);
            //    }

            //}
            #endregion
            List<FinalOrderDetailTemp> totalOrderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["totalOrderInstallPrice"] != null)
            {
                totalOrderList = Session["totalOrderInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (totalOrderList.Any())
            {
                var orderList0 = (from order in totalOrderList
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
            //活动订单
            List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
            //常规订单
            List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();

            List<Subject> subjectList = new List<Subject>();
           
            if (Session["activityOrderListIP"] != null)
            {
                activityOrderList = Session["activityOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["genericOrderListIP"] != null)
            {
                genericOrderList = Session["genericOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectListIP"] != null)
            {
                subjectList = Session["subjectListIP"] as List<Subject>;
            }
            List<FinalOrderDetailTemp> totalOrderList= activityOrderList.Concat(genericOrderList).ToList();
            if (totalOrderList.Any())
            {
                var orderList0 = (from order in totalOrderList
                                  join subject in subjectList
                                  on order.SubjectId equals subject.Id
                                  join subjectType1 in CurrentContext.DbContext.SubjectType
                                  on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                                  from subjectType in typeTemp.DefaultIfEmpty()
                                  where 
                                  //(regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                                  (regions.Any() ? regions.Contains(order.Region.ToLower()) : 1 == 1)
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
        }

        void BindSubject_old()
        {
            cblSubjectName.Items.Clear();
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
            #region 旧的
            //List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            //List<Subject> subjectList = new List<Subject>();
            //if (Session["orderDetailInstallPrice"] != null)
            //{
            //    orderList = Session["orderDetailInstallPrice"] as List<FinalOrderDetailTemp>;
            //}
            //if (Session["subjectInstallPrice"] != null)
            //{
            //    subjectList = Session["subjectInstallPrice"] as List<Subject>;
            //}

            //if (orderList.Any())
            //{
            //if (Session["assginShopIdInstallPrice"] != null)
            //{
            //    List<int> assginShopIdList = Session["assginShopIdInstallPrice"] as List<int>;
            //    if (assginShopIdList.Any())
            //    {
            //        orderList = orderList.Where(s => !assginShopIdList.Contains(s.ShopId ?? 0)).ToList();
            //    }
            //}
            //常规订单


            //}
            //if (orderList.Any())
            //{
            //    List<string> regions = new List<string>();
            //    List<int> subjectTypeList = new List<int>();
            //    foreach (ListItem li in cblRegion.Items)
            //    {
            //        if (li.Selected && !regions.Contains(li.Value.ToLower()))
            //            regions.Add(li.Value.ToLower());
            //    }
            //    foreach (ListItem li in cblSubjectType.Items)
            //    {
            //        if (li.Selected && !subjectTypeList.Contains(int.Parse(li.Value)))
            //            subjectTypeList.Add(int.Parse(li.Value));
            //    }
            //    var orderList0 = (from order in orderList
            //                      join subject in subjectList
            //                      on order.SubjectId equals subject.Id
            //                      join subjectType1 in CurrentContext.DbContext.SubjectType
            //                     on subject.SubjectTypeId equals subjectType1.Id into typeTemp
            //                      from subjectType in typeTemp.DefaultIfEmpty()
            //                      where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
            //                      select new
            //                      {
            //                          order,
            //                          subject
            //                      }).ToList();


            //    if (subjectTypeList.Any())
            //    {

            //        if (subjectTypeList.Contains(0))
            //        {
            //            subjectTypeList.Remove(0);
            //            if (subjectTypeList.Any())
            //            {
            //                orderList0 = orderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
            //            }
            //            else
            //                orderList0 = orderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
            //        }
            //        else
            //            orderList0 = orderList0.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
            //    }

            //    if (orderList0.Any())
            //    {
            //        var subjectList0 = orderList0.Select(s => s.subject).Distinct().OrderBy(s => s.SubjectName).ToList();
            //        subjectList0.ForEach(s =>
            //        {
            //            ListItem li = new ListItem();
            //            li.Value = s.Id.ToString();
            //            li.Text = s.SubjectName + "&nbsp;&nbsp;";
            //            cblSubjectName.Items.Add(li);
            //        });



            //    }
            //}
            #endregion

            List<FinalOrderDetailTemp> totalOrderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            if (Session["totalOrderInstallPrice"] != null)
            {
                totalOrderList = Session["totalOrderInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (totalOrderList.Any())
            {
                var orderList0 = (from order in totalOrderList
                                  join subject in subjectList
                                  on order.SubjectId equals subject.Id
                                  where (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                                  && ((subject.HandMakeSubjectId ?? 0) == 0)
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

        void BindSubject()
        {
            cblSubjectName.Items.Clear();
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
            //活动订单
            List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
            //常规订单
            List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();

            List<Subject> subjectList = new List<Subject>();
            
            if (Session["activityOrderListIP"] != null)
            {
                activityOrderList = Session["activityOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["genericOrderListIP"] != null)
            {
                genericOrderList = Session["genericOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectListIP"] != null)
            {
                subjectList = Session["subjectListIP"] as List<Subject>;
            }
            List<FinalOrderDetailTemp> totalOrderList = activityOrderList.Concat(genericOrderList).ToList();
            if (totalOrderList.Any())
            {
                var subjectList0 = (from order in totalOrderList
                                    join subject in subjectList
                                    on order.SubjectId equals subject.Id
                                    where 
                                    //(regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                                    (regions.Any() ? regions.Contains(order.Region.ToLower()) : 1 == 1)
                                    && (subject.HandMakeSubjectId ?? 0) == 0
                                    select subject).Distinct().ToList();
                if (subjectTypeList.Any())
                {

                    if (subjectTypeList.Contains(0))
                    {
                        subjectTypeList.Remove(0);
                        if (subjectTypeList.Any())
                        {
                            subjectList0 = subjectList0.Where(s => (s.SubjectTypeId == null || s.SubjectTypeId == 0) || (subjectTypeList.Contains(s.SubjectTypeId ?? 0))).ToList();
                        }
                        else
                            subjectList0 = subjectList0.Where(s => s.SubjectTypeId == null || s.SubjectTypeId == 0).ToList();
                    }
                    else
                        subjectList0 = subjectList0.Where(s => subjectTypeList.Contains(s.SubjectTypeId ?? 0)).ToList();
                }
                if (subjectList0.Any())
                {
                    //var subjectList0 = orderList0.Select(s => s.subject).Distinct().OrderBy(s => s.SubjectName).ToList();
                    subjectList0.OrderBy(s => s.SubjectName).ToList().ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectName + "&nbsp;&nbsp;";
                        cblSubjectName.Items.Add(li);
                    });
                }
            }
        }

        void BindCityTier() {
            cblCityTier.Items.Clear();
            //cbCityTier.Checked = false;
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
            //活动订单
            List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
            //常规订单
            List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();

            List<Subject> subjectList = new List<Subject>();

            if (Session["activityOrderListIP"] != null)
            {
                activityOrderList = Session["activityOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["genericOrderListIP"] != null)
            {
                genericOrderList = Session["genericOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectListIP"] != null)
            {
                subjectList = Session["subjectListIP"] as List<Subject>;
            }
            List<FinalOrderDetailTemp> totalOrderList = activityOrderList.Concat(genericOrderList).ToList();
            if (totalOrderList.Any())
            {
                var orderList0 = (from order in totalOrderList
                                  join subject in subjectList
                                  on order.SubjectId equals subject.Id
                                  where order.SubjectId == subject.Id
                                      //&& (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                                  && (regions.Any() ? regions.Contains(order.Region.ToLower()) : 1 == 1)
                                  select new { order, subject }).ToList();
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
                    //百丽项目
                    Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                    List<int> hMSubjectIdList = subjectList.Where(s => subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                    subjectIdList.AddRange(hMSubjectIdList);
                    orderList0 = orderList0.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                }
                if (orderList0.Any())
                {
                    List<string> cityTierList = orderList0.Select(s => s.order.CityTier).Distinct().OrderBy(s=>s).ToList();
                    bool isEmpty = false;
                    cityTierList.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;&nbsp;";
                            cblCityTier.Items.Add(li);
                        }
                        else
                            isEmpty = true;
                    });
                    if (isEmpty)
                    {
                        cblCityTier.Items.Add(new ListItem("空", "空"));
                    }
                }
            }
        }

        void BindPOSScale_old()
        {
            cblPOSScale.Items.Clear();
            labShopCount.Text = "";

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
            if (Session["totalOrderInstallPrice"] != null)
            {
                orderList = Session["totalOrderInstallPrice"] as List<FinalOrderDetailTemp>;
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

        void BindPOSScale()
        {
            cblPOSScale.Items.Clear();
            labShopCount.Text = "";

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
            List<string> cityTierList = new List<string>();
            foreach (ListItem li in cblCityTier.Items)
            {
                if (li.Selected && !cityTierList.Contains(li.Value))
                    cityTierList.Add(li.Value);
            }
            //活动订单
            List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
            //常规订单
            List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();

            List<Subject> subjectList = new List<Subject>();
            
            if (Session["activityOrderListIP"] != null)
            {
                activityOrderList = Session["activityOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["genericOrderListIP"] != null)
            {
                genericOrderList = Session["genericOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectListIP"] != null)
            {
                subjectList = Session["subjectListIP"] as List<Subject>;
            }
            List<FinalOrderDetailTemp> totalOrderList = activityOrderList.Concat(genericOrderList).ToList();
            if (totalOrderList.Any())
            {
                var orderList0 = (from order in totalOrderList
                                  join subject in subjectList
                                  on order.SubjectId equals subject.Id
                                  where order.SubjectId == subject.Id
                                  //&& (regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                                  && (regions.Any() ? regions.Contains(order.Region.ToLower()) : 1 == 1)
                                  select new { order, subject }).ToList();
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
                    //百丽项目
                    Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                    List<int> hMSubjectIdList = subjectList.Where(s => subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                    subjectIdList.AddRange(hMSubjectIdList);
                    orderList0 = orderList0.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                }
                if (cityTierList.Any())
                {
                    if (cityTierList.Contains("空"))
                    {
                        cityTierList.Remove("空");
                        if (cityTierList.Any())
                        {
                            orderList0 = orderList0.Where(s => cityTierList.Contains(s.order.CityTier) || s.order.CityTier == "" || s.order.CityTier == null).ToList();
                        }
                        else
                            orderList0 = orderList0.Where(s => s.order.CityTier == "" || s.order.CityTier == null).ToList();
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                    }
                }
                if (orderList0.Any())
                {
                    List<string> POSScaleList = orderList0.Select(s => s.order.InstallPricePOSScale).Distinct().ToList();
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

        }

        void GetShopCount_old()
        {
            labShopCount.Text = "";
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
            if (Session["totalOrderInstallPrice"] != null)
            {
                orderList = Session["totalOrderInstallPrice"] as List<FinalOrderDetailTemp>;
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

        void GetShopCount()
        {
            labShopCount.Text = "0";
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
            List<string> cityTierList = new List<string>();
            foreach (ListItem li in cblCityTier.Items)
            {
                if (li.Selected && !cityTierList.Contains(li.Value))
                    cityTierList.Add(li.Value);
            }
            List<string> posScaleList = new List<string>();
            foreach (ListItem li in cblPOSScale.Items)
            {
                if (li.Selected && !posScaleList.Contains(li.Value))
                    posScaleList.Add(li.Value);
            }
            //活动订单
            List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
            //常规订单
            List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();

            List<Subject> subjectList = new List<Subject>();
            
            if (Session["activityOrderListIP"] != null)
            {
                activityOrderList = Session["activityOrderListIP"] as List<FinalOrderDetailTemp>;
                
            }
            if (Session["genericOrderListIP"] != null)
            {
                genericOrderList = Session["genericOrderListIP"] as List<FinalOrderDetailTemp>;
                
            }
            if (Session["subjectListIP"] != null)
            {
                subjectList = Session["subjectListIP"] as List<Subject>;
            }
            List<FinalOrderDetailTemp> totalOrderList = activityOrderList.Concat(genericOrderList).ToList();
            
            if (totalOrderList.Any())
            {
                var orderList0 = (from order in totalOrderList
                                  join subject in subjectList
                                  on order.SubjectId equals subject.Id
                                  where 
                                  //(regions.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regions.Contains(subject.PriceBlongRegion.ToLower()) : regions.Contains(order.Region.ToLower())) : 1 == 1)
                                 (regions.Any() ? regions.Contains(order.Region.ToLower()): 1 == 1)
                                 
                                  select new { order, subject }).ToList();
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
                    //百丽项目
                    Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                    List<int> hMSubjectIdList = subjectList.Where(s => subjectIdList.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                    subjectIdList.AddRange(hMSubjectIdList);
                    orderList0 = orderList0.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
                }
                if (cityTierList.Any())
                {
                    if (cityTierList.Contains("空"))
                    {
                        cityTierList.Remove("空");
                        if (cityTierList.Any())
                        {
                            orderList0 = orderList0.Where(s => cityTierList.Contains(s.order.CityTier) || s.order.CityTier == "" || s.order.CityTier == null).ToList();
                        }
                        else
                            orderList0 = orderList0.Where(s => s.order.CityTier == "" || s.order.CityTier == null).ToList();
                    }
                    else
                    {
                        orderList0 = orderList0.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                    }
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
        }

        void BindSubjectDDL_old()
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
                        && (subject.IsSecondInstall ?? false) == false
                        && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == (int)OrderTypeEnum.道具))
                        && ((subject.HandMakeSubjectId ?? 0) == 0)
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

            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();

            List<Subject> subjectList = new List<Subject>();

            if (Session["totalOrderListIP"] != null)
            {
                orderList = Session["totalOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectListIP"] != null)
            {
                subjectList = Session["subjectListIP"] as List<Subject>;
            }
            
            
            var list = (from order in orderList
                        join subject in subjectList
                        on order.SubjectId equals subject.Id
                        where subject.GuidanceId == guidanceId
                        select new { order, subject }).ToList();
            if (regions.Any())
            {
                list = list.Where(s => regions.Contains(s.order.Region.ToLower())).ToList();
            }
            if (list.Any())
            {
                var slist = list.Where(s=>(s.subject.HandMakeSubjectId??0)==0).Select(s => s.subject).Distinct().OrderBy(s => s.SubjectTypeId).ToList();
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
            BindCityTier();
            BindPOSScale();
            GetShopCount();
            BindSubjectDDL();
        }

        protected void cblSubjectType_SelectedIndexChanged(object sender, EventArgs e)
        {

            BindSubject();
            BindCityTier();
            BindPOSScale();
            GetShopCount();

        }

        protected void cblSubjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCityTier();
            BindPOSScale();
            GetShopCount();

        }

        protected void cblCityTier_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPOSScale();
            GetShopCount();
        }

        protected void cblPOSScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetShopCount();
        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SubmitPrice();
        }

        /// <summary>
        /// 安装费计算（新）
        /// </summary>
        void SubmitPrice_old()
        {
            bool isOk = true;
            List<string> selectRegion = new List<string>();
            List<int> selectSubjectType = new List<int>();
            List<int> selectSubject = new List<int>();
            List<string> selectPOSScale = new List<string>();
            int subjectId = int.Parse(ddlSubject.SelectedValue);
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
            if (Session["totalOrderInstallPrice"] != null)
            {
                orderList = Session["totalOrderInstallPrice"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectInstallPrice"] != null)
            {
                subjectList = Session["subjectInstallPrice"] as List<Subject>;
            }
            if (Session["shopInstallPrice"] != null)
            {
                shopList = Session["shopInstallPrice"] as List<Shop>;
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
                //百丽项目
                Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                List<int> hMSubjectIdList = new SubjectBLL().GetList(s => guidanceId == s.GuidanceId && selectSubject.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                selectSubject.AddRange(hMSubjectIdList);

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

                        InstallPriceDetail model = installPriceBll.GetList(s => s.GuidanceId == guidanceId && s.SubjectId == subjectId && s.AddType == 1).FirstOrDefault();
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
                            bool isCanSave = true;
                            if (shop.Channel != null && shop.Channel.ToLower().Contains("terrex") && shop.RegionName != null && shop.RegionName.ToLower().Contains("east"))
                                isCanSave = false;
                            if (isCanSave)
                            {
                                bool isBCSSubject = true;
                                bool isGeneric = false;
                                bool isContainsNotGeneric = false;
                                //基础安装费
                                decimal basicInstallPrice = 0;
                                decimal genericBasicInstallPrice = 0;
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
                                        if (s.CategoryName.Contains("常规-非活动"))
                                            isGeneric = true;
                                        else
                                        {
                                            isContainsNotGeneric = true;
                                            if (s.subject.CornerType != "三叶草")
                                                isBCSSubject = false;
                                        }
                                    });
                                    List<FinalOrderDetailTemp> oohOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower() == "ooh" || s.order.Sheet == "户外")).Select(s => s.order).ToList();
                                    List<FinalOrderDetailTemp> windowOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower().Contains("橱窗") || s.order.Sheet.ToLower().Contains("window"))).Select(s => s.order).ToList();


                                    InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();
                                    #region 活动安装费
                                    if (isContainsNotGeneric)
                                    {

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

                                        else if ((shop.BasicInstallPrice ?? 0) > 0)
                                        {
                                            basicInstallPrice = (shop.BasicInstallPrice ?? 0);
                                        }
                                        #endregion
                                        #region 橱窗安装
                                        if (!isGeneric)
                                        {
                                            if (windowOrderList.Any())
                                            {
                                                windowInstallPrice = GetWindowInstallPrice(materialSupport);
                                            }
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
                                        if (basicInstallPrice > 0)
                                        {
                                            InstallPriceShopInfo installPriceShopModel = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == shop.Id && i.AddType == 1 && (i.SubjectType == null || i.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费)).FirstOrDefault();
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
                                                installPriceShopModel.AddDate = DateTime.Now;
                                                installPriceShopModel.AddUserId = CurrentUser.UserId;
                                                installPriceShopModel.SubjectType = (int)InstallPriceSubjectTypeEnum.活动安装费;
                                                installShopBll.Add(installPriceShopModel);
                                            }
                                        }

                                        #endregion
                                    }
                                    #endregion
                                    #region 常规安装费
                                    if (isGeneric)
                                    {
                                        if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                                        {
                                            genericBasicInstallPrice = bcsBasicInstallPrice;
                                        }
                                        else
                                        {
                                            genericBasicInstallPrice = 0;
                                        }
                                        if (genericBasicInstallPrice > 0)
                                        {
                                            InstallPriceShopInfo installPriceShopModel = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == shop.Id && i.AddType == 1 && (i.SubjectType != null && i.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费)).FirstOrDefault();
                                            if (installPriceShopModel == null)
                                            {
                                                installPriceShopModel = new InstallPriceShopInfo();
                                                installPriceShopModel.BasicPrice = genericBasicInstallPrice;
                                                installPriceShopModel.OOHPrice = 0;
                                                installPriceShopModel.WindowPrice = 0;
                                                installPriceShopModel.InstallDetailId = model.Id;
                                                installPriceShopModel.GuidanceId = guidanceId;
                                                installPriceShopModel.SubjectId = subjectId;
                                                installPriceShopModel.ShopId = shop.Id;
                                                installPriceShopModel.MaterialSupport = "Generic";
                                                installPriceShopModel.POSScale = POSScale;
                                                installPriceShopModel.AddType = 1;
                                                installPriceShopModel.AddDate = DateTime.Now;
                                                installPriceShopModel.AddUserId = CurrentUser.UserId;
                                                installPriceShopModel.SubjectType = (int)InstallPriceSubjectTypeEnum.常规安装费;
                                                installShopBll.Add(installPriceShopModel);
                                            }
                                        }
                                    }
                                    #endregion


                                }
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
                    new WebApp.Base.DelegateClass().UpdateOutsourceInstallPriceSubject(guidanceId, subjectId);
                    Response.Redirect(string.Format("SubmitInstallPrice.aspx?itemid={0}", guidanceId), false);
                }
            }
        }


        void SubmitPrice()
        {
            bool isOk = true;
            List<string> selectRegion = new List<string>();
            List<int> selectSubjectType = new List<int>();
            List<int> selectSubject = new List<int>();
            List<string> selectPOSScale = new List<string>();
            List<string> cityTierList = new List<string>();
            int subjectId = int.Parse(ddlSubject.SelectedValue);
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
            
            foreach (ListItem li in cblCityTier.Items)
            {
                if (li.Selected && !cityTierList.Contains(li.Value))
                    cityTierList.Add(li.Value);
            }
            foreach (ListItem li in cblPOSScale.Items)
            {
                if (li.Selected)
                    selectPOSScale.Add(li.Value);
            }
            if (!selectPOSScale.Any())
            {
                foreach (ListItem li in cblPOSScale.Items)
                {
                   selectPOSScale.Add(li.Value);
                }
            }
            InstallPriceDetailBLL installPriceBll = new InstallPriceDetailBLL();
            InstallPriceDetail installPriceDetailModel = new InstallPriceDetail();
            //活动订单
            List<FinalOrderDetailTemp> activityOrderList = new List<FinalOrderDetailTemp>();
            //常规订单
            List<FinalOrderDetailTemp> genericOrderList = new List<FinalOrderDetailTemp>();
            List<Subject> subjectList = new List<Subject>();
            
            if (Session["activityOrderListIP"] != null)
            {
                activityOrderList = Session["activityOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["genericOrderListIP"] != null)
            {
                genericOrderList = Session["genericOrderListIP"] as List<FinalOrderDetailTemp>;
            }
            if (Session["subjectListIP"] != null)
            {
                subjectList = Session["subjectListIP"] as List<Subject>;
            }
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    if (activityOrderList.Any() || genericOrderList.Any())
                    {
                        #region 保存/更新InstallPriceDetail
                        installPriceDetailModel = installPriceBll.GetList(s => s.GuidanceId == guidanceId && s.SubjectId == subjectId && s.AddType == 1).FirstOrDefault();
                        if (installPriceDetailModel != null)
                        {

                            if (selectPOSScale.Any())
                            {
                                List<string> POSScale = new List<string>();
                                if (!string.IsNullOrWhiteSpace(installPriceDetailModel.SelectPOSScale))
                                {
                                    POSScale = StringHelper.ToStringList(installPriceDetailModel.SelectPOSScale, ',');
                                }
                                selectPOSScale.ForEach(s =>
                                {
                                    if (!POSScale.Contains(s))
                                    {
                                        POSScale.Add(s);
                                    }
                                });
                                installPriceDetailModel.SelectPOSScale = StringHelper.ListToString(POSScale);
                            }

                            if (selectRegion.Any())
                            {

                                List<string> Regions = new List<string>();
                                if (!string.IsNullOrWhiteSpace(installPriceDetailModel.SelectRegion))
                                {
                                    Regions = StringHelper.ToStringList(installPriceDetailModel.SelectRegion, ',');
                                }
                                selectRegion.ForEach(s =>
                                {
                                    if (!Regions.Contains(s))
                                    {
                                        Regions.Add(s);
                                    }
                                });
                                installPriceDetailModel.SelectRegion = StringHelper.ListToString(Regions);
                            }
                            if (selectSubject.Any())
                            {
                                List<int> Subjects = new List<int>();
                                if (!string.IsNullOrWhiteSpace(installPriceDetailModel.SelectSubjectId))
                                {
                                    Subjects = StringHelper.ToIntList(installPriceDetailModel.SelectSubjectId, ',');
                                }
                                selectSubject.ForEach(s =>
                                {
                                    if (!Subjects.Contains(s))
                                    {
                                        Subjects.Add(s);
                                    }
                                });
                                installPriceDetailModel.SelectSubjectId = StringHelper.ListToString(Subjects);
                            }
                            if (selectSubjectType.Any())
                            {
                                List<int> SubjectTypes = new List<int>();
                                if (!string.IsNullOrWhiteSpace(installPriceDetailModel.SelectSubjectTypeId))
                                {
                                    SubjectTypes = StringHelper.ToIntList(installPriceDetailModel.SelectSubjectTypeId, ',');
                                }
                                selectSubjectType.ForEach(s =>
                                {
                                    if (!SubjectTypes.Contains(s))
                                    {
                                        SubjectTypes.Add(s);
                                    }
                                });
                                installPriceDetailModel.SelectSubjectTypeId = StringHelper.ListToString(SubjectTypes);
                            }

                            installPriceBll.Update(installPriceDetailModel);

                        }
                        else
                        {
                            installPriceDetailModel = new InstallPriceDetail();
                            installPriceDetailModel.AddType = 1;
                            installPriceDetailModel.AddDate = DateTime.Now;
                            installPriceDetailModel.GuidanceId = guidanceId;
                            installPriceDetailModel.SelectPOSScale = StringHelper.ListToString(selectPOSScale);
                            installPriceDetailModel.SelectRegion = StringHelper.ListToString(selectRegion);
                            installPriceDetailModel.SelectSubjectId = StringHelper.ListToString(selectSubject);
                            installPriceDetailModel.SelectSubjectTypeId = StringHelper.ListToString(selectSubjectType);
                            installPriceDetailModel.SubjectId = subjectId;
                            installPriceBll.Add(installPriceDetailModel);
                        }
                        #endregion
                    }
                    if (activityOrderList.Any())
                    {
                        #region 活动订单
                        var activityOrderList0 = (from order in activityOrderList
                                                  join subject in subjectList
                                                  on order.SubjectId equals subject.Id
                                                  join shop in CurrentContext.DbContext.Shop
                                                  on order.ShopId equals shop.Id
                                                  where 
                                                  //(selectRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? selectRegion.Contains(subject.PriceBlongRegion.ToLower()) : selectRegion.Contains(order.Region.ToLower())) : 1 == 1)
                                                  (selectRegion.Any() ? selectRegion.Contains(order.Region.ToLower()) : 1 == 1)
                                                  select new { order, shop, subject }).ToList();
                        if (selectSubjectType.Any())
                        {
                            List<int> selectSubjectType0 = new List<int>();
                            selectSubjectType0 = selectSubjectType.Select(s => s).ToList();
                            if (selectSubjectType0.Contains(0))
                            {
                                selectSubjectType0.Remove(0);
                                if (selectSubjectType0.Any())
                                {
                                    activityOrderList0 = activityOrderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (selectSubjectType0.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                                }
                                else
                                    activityOrderList0 = activityOrderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                            }
                            else
                                activityOrderList0 = activityOrderList0.Where(s => selectSubjectType0.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                        }
                        if (selectSubject.Any())
                        {
                            //百丽项目
                            Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                            List<int> hMSubjectIdList = subjectList.Where(s => selectSubject.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                            selectSubject.AddRange(hMSubjectIdList);
                            activityOrderList0 = activityOrderList0.Where(s => selectSubject.Contains(s.subject.Id)).ToList();
                        }
                        if (cityTierList.Any())
                        {
                            List<string> cityTierList0 = new List<string>();
                            cityTierList0 = cityTierList.Select(s => s).ToList();
                            if (cityTierList0.Contains("空"))
                            {
                                cityTierList0.Remove("空");
                                if (cityTierList0.Any())
                                {
                                    activityOrderList0 = activityOrderList0.Where(s => cityTierList0.Contains(s.order.CityTier) || s.order.CityTier == "" || s.order.CityTier == null).ToList();
                                }
                                else
                                    activityOrderList0 = activityOrderList0.Where(s => s.order.CityTier == "" || s.order.CityTier == null).ToList();
                            }
                            else
                            {
                                activityOrderList0 = activityOrderList0.Where(s => cityTierList0.Contains(s.order.CityTier)).ToList();
                            }
                        }
                        if (selectPOSScale.Any())
                        {
                            List<string> selectPOSScale0 = new List<string>();
                            selectPOSScale0 = selectPOSScale.Select(s => s).ToList();
                            if (selectPOSScale0.Contains("空"))
                            {
                                selectPOSScale0.Remove("空");
                                if (selectPOSScale0.Any())
                                {
                                    activityOrderList0 = activityOrderList0.Where(s => selectPOSScale0.Contains(s.order.InstallPricePOSScale) || s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                                }
                                else
                                    activityOrderList0 = activityOrderList0.Where(s => s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                            }
                            else
                                activityOrderList0 = activityOrderList0.Where(s => selectPOSScale0.Contains(s.order.InstallPricePOSScale)).ToList();
                        }
                        //
                        if (activityOrderList0.Any())
                        {
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
                            List<Shop> shopList0 = activityOrderList0.Select(s => s.shop).Distinct().ToList();
                            List<int> shopIdList = shopList0.Select(s => s.Id).ToList();
                            var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);
                            var totalOrderList0 = (from order in activityOrderList
                                                   from subject in subjectList
                                                   where order.SubjectId == subject.Id
                                                   select new { order, subject }).ToList();
                            shopList0.ForEach(shop =>
                            {
                                bool isBCSSubject = true;

                                //基础安装费
                                decimal basicInstallPrice = 0;
                                //橱窗安装费
                                decimal windowInstallPrice = 0;
                                //OOH安装费
                                decimal oohInstallPrice = 0;
                                string materialSupport = string.Empty;
                                string POSScale = string.Empty;
                                List<string> materialSupportList = new List<string>();
                                var oneShopOrderList = totalOrderList0.Where(s => s.order.ShopId == shop.Id).ToList();
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
                                    });
                                    if (!materialSupportList.Any())
                                    {
                                        materialSupportList.Add("Basic");
                                    }
                                    bool isInstall = false;
                                    if (isBCSSubject)
                                    {
                                        isInstall = shop.BCSIsInstall == "Y";
                                    }
                                    else
                                    {
                                        isInstall = shop.IsInstall == "Y";
                                    }
                                    if (isInstall)
                                    {
                                        List<FinalOrderDetailTemp> oohOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower() == "ooh" || s.order.Sheet == "户外")).Select(s => s.order).ToList();
                                        List<FinalOrderDetailTemp> windowOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower().Contains("橱窗") || s.order.Sheet.ToLower().Contains("window"))).Select(s => s.order).ToList();

                                        InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();

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

                                        else if ((shop.BasicInstallPrice ?? 0) > 0)
                                        {
                                            basicInstallPrice = (shop.BasicInstallPrice ?? 0);
                                        }
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
                                        if (basicInstallPrice > 0)
                                        {
                                            InstallPriceShopInfo installPriceShopModel = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == shop.Id && i.AddType == 1 && (i.SubjectType == null || i.SubjectType == (int)InstallPriceSubjectTypeEnum.活动安装费)).FirstOrDefault();
                                            if (installPriceShopModel == null)
                                            {
                                                installPriceShopModel = new InstallPriceShopInfo();
                                                installPriceShopModel.BasicPrice = basicInstallPrice;
                                                installPriceShopModel.OOHPrice = oohInstallPrice;
                                                installPriceShopModel.WindowPrice = windowInstallPrice;
                                                installPriceShopModel.InstallDetailId = installPriceDetailModel.Id;
                                                installPriceShopModel.GuidanceId = guidanceId;
                                                installPriceShopModel.SubjectId = subjectId;
                                                installPriceShopModel.ShopId = shop.Id;
                                                installPriceShopModel.MaterialSupport = materialSupport;
                                                installPriceShopModel.POSScale = POSScale;
                                                installPriceShopModel.AddType = 1;
                                                installPriceShopModel.AddDate = DateTime.Now;
                                                installPriceShopModel.AddUserId = CurrentUser.UserId;
                                                installPriceShopModel.SubjectType = (int)InstallPriceSubjectTypeEnum.活动安装费;
                                                installShopBll.Add(installPriceShopModel);
                                            }
                                        }

                                        #endregion
                                    }

                                }

                            });
                        }

                        #endregion
                    }
                    if (genericOrderList.Any())
                    {
                        #region 常规订单
                        var genericOrderList0 = (from order in genericOrderList
                                                 join subject in subjectList
                                                 on order.SubjectId equals subject.Id
                                                 join shop in CurrentContext.DbContext.Shop
                                                 on order.ShopId equals shop.Id
                                                 where 
                                                 //order.SubjectId == subject.Id
                                                 //&& (selectRegion.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? selectRegion.Contains(subject.PriceBlongRegion.ToLower()) : selectRegion.Contains(order.Region.ToLower())) : 1 == 1)
                                                 (selectRegion.Any() ? selectRegion.Contains(order.Region.ToLower()) : 1 == 1)
                                                 select new { order, shop, subject }).ToList();
                        if (selectSubjectType.Any())
                        {

                            if (selectSubjectType.Contains(0))
                            {
                                selectSubjectType.Remove(0);
                                if (selectSubjectType.Any())
                                {
                                    genericOrderList0 = genericOrderList0.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                                }
                                else
                                    genericOrderList0 = genericOrderList0.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                            }
                            else
                                genericOrderList0 = genericOrderList0.Where(s => selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                        }
                        if (selectSubject.Any())
                        {
                            //百丽项目
                            Dictionary<int, int> handMakeSubjectIdDic = new Dictionary<int, int>();
                            List<int> hMSubjectIdList = subjectList.Where(s => selectSubject.Contains(s.HandMakeSubjectId ?? 0)).Select(s => s.Id).ToList();
                            selectSubject.AddRange(hMSubjectIdList);
                            genericOrderList0 = genericOrderList0.Where(s => selectSubject.Contains(s.subject.Id)).ToList();
                        }
                        if (cityTierList.Any())
                        {
                            if (cityTierList.Contains("空"))
                            {
                                cityTierList.Remove("空");
                                if (cityTierList.Any())
                                {
                                    genericOrderList0 = genericOrderList0.Where(s => cityTierList.Contains(s.order.CityTier) || s.order.CityTier == "" || s.order.CityTier == null).ToList();
                                }
                                else
                                    genericOrderList0 = genericOrderList0.Where(s => s.order.CityTier == "" || s.order.CityTier == null).ToList();
                            }
                            else
                            {
                                genericOrderList0 = genericOrderList0.Where(s => cityTierList.Contains(s.order.CityTier)).ToList();
                            }
                        }
                        if (selectPOSScale.Any())
                        {
                            if (selectPOSScale.Contains("空"))
                            {
                                selectPOSScale.Remove("空");
                                if (selectPOSScale.Any())
                                {
                                    genericOrderList0 = genericOrderList0.Where(s => selectPOSScale.Contains(s.order.InstallPricePOSScale) || s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                                }
                                else
                                    genericOrderList0 = genericOrderList0.Where(s => s.order.InstallPricePOSScale == "" || s.order.InstallPricePOSScale == null).ToList();
                            }
                            else
                                genericOrderList0 = genericOrderList0.Where(s => selectPOSScale.Contains(s.order.InstallPricePOSScale)).ToList();
                        }
                        List<Shop> shopList0 = genericOrderList0.Select(s => s.shop).Distinct().ToList();
                        List<int> shopIdList = shopList0.Select(s => s.Id).ToList();
                        var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);
                        var totalOrderList0 = (from order in genericOrderList
                                               from subject in subjectList
                                               where order.SubjectId == subject.Id
                                               select new { order, subject }).ToList();
                        shopList0.ForEach(shop =>
                        {
                            //基础安装费
                            decimal basicInstallPrice = 0;
                            //橱窗安装费
                            decimal windowInstallPrice = 0;
                            //OOH安装费
                            decimal oohInstallPrice = 0;
                            string materialSupport = string.Empty;
                            string POSScale = string.Empty;
                            List<string> materialSupportList = new List<string>();
                            var oneShopOrderList = totalOrderList0.Where(s => s.order.ShopId == shop.Id).ToList();
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

                                });
                                if (!materialSupportList.Any())
                                {
                                    materialSupportList.Add("generic");
                                }
                                bool isInstall = false;
                                if (shop.RegionName != null && shop.RegionName.ToLower() == "west")
                                {
                                    isInstall = shop.IsInstall == "Y";
                                }
                                else if (cityCierList.Contains(shop.CityTier))
                                {
                                    isInstall = true;
                                }
                                if (isInstall)
                                {
                                    List<FinalOrderDetailTemp> oohOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower() == "ooh" || s.order.Sheet == "户外")).Select(s => s.order).ToList();
                                    //List<FinalOrderDetailTemp> windowOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower().Contains("橱窗") || s.order.Sheet.ToLower().Contains("window"))).Select(s => s.order).ToList();

                                    InstallPriceShopInfoBLL installShopBll = new InstallPriceShopInfoBLL();

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
                                    if ((shop.GenericInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = (shop.GenericInstallPrice ?? 0);
                                    }
                                    if (basicInstallPrice == 0)
                                    {
                                        basicInstallPrice = 150;
                                    }

                                    #endregion
                                    #region 橱窗安装
                                    //if (windowOrderList.Any())
                                    //{
                                    //    windowInstallPrice = GetWindowInstallPrice(materialSupport);
                                    //}
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
                                    if (basicInstallPrice > 0)
                                    {
                                        InstallPriceShopInfo installPriceShopModel = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == shop.Id && i.AddType == 1 && (i.SubjectType == null || i.SubjectType == (int)InstallPriceSubjectTypeEnum.常规安装费)).FirstOrDefault();
                                        if (installPriceShopModel == null)
                                        {
                                            installPriceShopModel = new InstallPriceShopInfo();
                                            installPriceShopModel.BasicPrice = basicInstallPrice;
                                            installPriceShopModel.OOHPrice = oohInstallPrice;
                                            installPriceShopModel.WindowPrice = windowInstallPrice;
                                            installPriceShopModel.InstallDetailId = installPriceDetailModel.Id;
                                            installPriceShopModel.GuidanceId = guidanceId;
                                            installPriceShopModel.SubjectId = subjectId;
                                            installPriceShopModel.ShopId = shop.Id;
                                            installPriceShopModel.MaterialSupport = materialSupport;
                                            installPriceShopModel.POSScale = POSScale;
                                            installPriceShopModel.AddType = 1;
                                            installPriceShopModel.AddDate = DateTime.Now;
                                            installPriceShopModel.AddUserId = CurrentUser.UserId;
                                            installPriceShopModel.SubjectType = (int)InstallPriceSubjectTypeEnum.常规安装费;
                                            installShopBll.Add(installPriceShopModel);
                                        }
                                    }

                                    #endregion
                                }

                            }
                        });

                        #endregion
                    }
                    tran.Complete();
                }
                catch (Exception ex)
                {
                    isOk = false;
                }
            }
            if (isOk)
            {
                new WebApp.Base.DelegateClass().UpdateOutsourceInstallPriceSubject(guidanceId, subjectId);
                Response.Redirect(string.Format("SubmitInstallPrice.aspx?itemid={0}", guidanceId), false);
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
                            List<int> shopIdList = installShopBll.GetList(s => s.InstallDetailId == model.Id).Select(s => s.ShopId ?? 0).ToList();
                            //删除外协安装费
                            outsourceOrderDetailBll.Delete(s => s.GuidanceId == model.GuidanceId && shopIdList.Contains(s.ShopId ?? 0) && s.SubjectId == model.SubjectId && s.OrderType == (int)OrderTypeEnum.安装费);
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
                                   && selectSubject.Contains(order.SubjectId ?? 0)
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
                        selectSubject.ForEach(sid =>
                        {
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
                                outsourceOrderDetailBll.Delete(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0) && s.SubjectId == sid && s.OrderType == (int)OrderTypeEnum.安装费);
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
                                        outsourceBasicInstallPrice = GetOutsourceBasicInstallPrice(materialSupport);
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
                                            outsourceBasicInstallPrice = basicInstallPrice = 150;//如果是二次安装，基础安装费150，其他不算
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

                                        InstallPriceShopInfo installPriceShopModel = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == shop.Id && i.SubjectId == sid && i.AddType == 2).FirstOrDefault();
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
                                            outsourceOrderDetailModel.Remark = outRemark + "(户外安装费)";
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
                                        outsourceOrderDetailModel.ReceiveOrderPrice = (basicInstallPrice + oohInstallPrice + windowInstallPrice);
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
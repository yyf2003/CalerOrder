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

namespace WebApp.Subjects.InstallPrice
{
    public partial class Edit : BasePage
    {
        public int guidanceId;
        SubjectBLL subjectBll = new SubjectBLL();
        bool isRegionOrder = false;
        List<string> myRegionList = new List<string>();
        List<string> cityCierList = new List<string>() { "T1", "T2", "T3" };
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
        List<int> myInstallShopIdList = new List<int>();
        void BindData()
        {
            var list = (from detail in CurrentContext.DbContext.InstallPriceDetail
                        join subject in CurrentContext.DbContext.Subject
                        on detail.SubjectId equals subject.Id
                        where detail.GuidanceId == guidanceId
                        select new
                        {
                            detail,
                            detail.SelectSubjectTypeId,
                            subject.SubjectName,
                            detail.Id
                        }).ToList();
            if (list.Any())
            {
                
                myInstallShopIdList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join shop in CurrentContext.DbContext.Shop
                                 on order.ShopId equals shop.Id
                                 where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                                 && subject.ApproveState == 1
                                 && (order.IsDelete == null || order.IsDelete == false)
                                     //&& shop.IsInstall != null && shop.IsInstall == "Y"
                                 && ((shop.IsInstall != null && shop.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                                 && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                                 && (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                                 select shop.Id).Distinct().ToList();
                List<int> installDetailList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId && myInstallShopIdList.Contains(s.ShopId ?? 0)).Select(s => s.InstallDetailId ?? 0).Distinct().ToList();
                list = list.Where(s => installDetailList.Contains(s.Id)).ToList();
            }
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
                if (model.ItemName.Contains("二次") || model.ItemName.Contains("2次"))
                {
                    cbIsSecondInstall.Checked = true;
                }
               
               

            }
        }

        void BindRegion()
        {
            cblRegion.Items.Clear();
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             //&& shop.IsInstall != null && shop.IsInstall == "Y"
                             && ((shop.IsInstall != null && shop.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                             && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                             && (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                             select shop).ToList();

            List<int> assginShopList = new List<int>();
            assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();
            if (assginShopList.Any())
            {
                orderList = orderList.Where(s => !assginShopList.Contains(s.Id)).ToList();
            }
            List<string> regionList = orderList.Select(s=>s.RegionName).Distinct().ToList();
            regionList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s;
                li.Text = s + "&nbsp;&nbsp;";
                cblRegion.Items.Add(li);
            });
        }

        void BindSubjectType()
        {
            cblSubjectType.Items.Clear();
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join subjectType1 in CurrentContext.DbContext.SubjectType
                             on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                             from subjectType in typeTemp.DefaultIfEmpty()
                             
                             where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             //&& (subject.SubjectType != (int)SubjectTypeEnum.二次安装费)
                             //&& shop.IsInstall != null && shop.IsInstall == "Y"
                             && ((shop.IsInstall != null && shop.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                             && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                             && (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) &&  myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                             select new
                             {
                                 shop,
                                 subject.SubjectTypeId,
                                 subjectType.SubjectTypeName,
                             }).ToList();

            //var priceDetailList = new InstallPriceDetailBLL().GetList(s => s.GuidanceId == guidanceId);
            List<int> assginShopList = new List<int>();
            assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();
            if (assginShopList.Any())
            {
                orderList = orderList.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
            }
            List<string> regions = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regions.Contains(li.Value.ToLower()))
                    regions.Add(li.Value.ToLower());
            }
            if (regions.Any())
            {
                orderList = orderList.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
            }
            if (orderList.Any())
            {
                var list = orderList.Select(s => new {s.SubjectTypeId,s.SubjectTypeName }).Distinct().ToList();
                List<int> typeIdList = new List<int>();
                bool isEmpty = false;
                list.ForEach(s => {
                    if (s.SubjectTypeId != null && s.SubjectTypeId!=0)
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
                //try
                //{
                //    List<string> regions = new List<string>();
                    
                //    foreach (ListItem li in cblRegion.Items)
                //    {
                //        if (li.Selected && !regions.Contains(li.Value.ToLower()))
                //            regions.Add(li.Value.ToLower());
                //    }

                //    if (regions.Any())
                //    {
                //        orderList = orderList.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
                //    }
                //    if (orderList.Any())
                //    {
                //        //labShopCount.Text = orderList.Select(s => s.shop.Id).Distinct().ToList().Count.ToString();

                //        int shopCount = orderList.Select(s => s.shop.Id).Distinct().ToList().Count;
                //        string text = "<span name='spanCheckLeftShop' style=' color:Blue; cursor:pointer; text-decoration:underline;'>" + shopCount + "</span>";
                //        labShopCount.Text = text;


                //        //orderList = orderList.Where(s => s.subject.SubjectType != (int)SubjectTypeEnum.补单).ToList();
                //        Dictionary<int, string> dicSubjectType = new Dictionary<int, string>();
                //        Dictionary<int, string> dicSubject = new Dictionary<int, string>();
                //        List<string> POSScaleList = new List<string>();
                //        orderList.ForEach(s =>
                //        {

                //            if (s.subject.SubjectTypeId != null && s.subject.SubjectTypeId > 0 && !dicSubjectType.Keys.Contains(s.subject.SubjectTypeId ?? 0))
                //            {
                //                dicSubjectType.Add(s.subject.SubjectTypeId ?? 0, s.SubjectTypeName);
                //            }
                            
                //            int subjectId = s.subject.Id;
                //            string subjectName = s.subject.SubjectName;
                //            if (s.subject.SubjectType == (int)SubjectTypeEnum.补单)
                //            {
                //                subjectId = s.subject.HandMakeSubjectId ?? 0;

                //            }
                //            if (!dicSubject.Keys.Contains(subjectId))
                //            {
                //                Subject subjectModel = subjectBll.GetModel(subjectId);
                //                if (subjectModel != null)
                //                {
                //                    subjectName = subjectModel.SubjectName;
                //                }
                //                dicSubject.Add(subjectId, subjectName);
                //            }
                //            if (!POSScaleList.Contains(s.POSScale))
                //            {
                //                POSScaleList.Add(s.POSScale);
                //            }

                //        });

                //        foreach (KeyValuePair<int, string> item in dicSubjectType)
                //        {
                //            ListItem li = new ListItem();
                //            li.Value = item.Key.ToString();
                //            li.Text = item.Value + "&nbsp;&nbsp;";
                //            cblSubjectType.Items.Add(li);
                //        }
                //        foreach (KeyValuePair<int, string> item in dicSubject)
                //        {
                //            ListItem li = new ListItem();
                //            li.Value = item.Key.ToString();
                //            li.Text = item.Value + "&nbsp;&nbsp;";
                //            cblSubjectName.Items.Add(li);
                //        }
                //        bool isEmpty = false;
                //        POSScaleList.OrderBy(s => s).ToList().ForEach(s =>
                //        {
                //            if (string.IsNullOrWhiteSpace(s))
                //            {
                //                isEmpty = true;
                //            }
                //            else
                //            {
                //                ListItem li = new ListItem();
                //                li.Value = s;
                //                li.Text = s + "&nbsp;&nbsp;";
                //                cblPOSScale.Items.Add(li);
                //            }
                //        });
                //        if (isEmpty)
                //        {
                //            //string text = "空（剩余店数：<span name='checkShops' style='cursor:pointer;text-decoration:underline;'>" + emptyPOSScaleShopList.Count + "</span>）";
                //            cblPOSScale.Items.Add(new ListItem("空", "空"));
                //        }
                //    }
                //}

                //catch (Exception ex)
                //{

                //}
            }

        }

        void BindSubject()
        {
            cblSubjectName.Items.Clear();
            //cblPOSScale.Items.Clear();
            //labShopCount.Text = "";
            
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             //&& (subject.SubjectType != (int)SubjectTypeEnum.二次安装费)
                             //&& shop.IsInstall != null && shop.IsInstall == "Y"
                             && ((shop.IsInstall != null && shop.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                             && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                             && (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                             select new
                             {
                                 subject,
                                 shop
                             }).ToList();

            
            List<int> assginShopList = new List<int>();
            assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();
            
            if (assginShopList.Any())
            {
                orderList = orderList.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
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
                if (regions.Any())
                {
                    orderList = orderList.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
                }
                if (subjectTypeList.Any())
                {
                    //orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                    if (subjectTypeList.Contains(0))
                    {
                        subjectTypeList.Remove(0);
                        if (subjectTypeList.Any())
                        {
                            orderList = orderList.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                        }
                        else
                            orderList = orderList.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId==0).ToList();
                    }
                    else
                        orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                }

                if (orderList.Any())
                {
                    var subjectList = orderList.Select(s=>s.subject).Distinct().OrderBy(s=>s.SubjectName).ToList();
                    subjectList.ForEach(s => {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectName + "&nbsp;&nbsp;";
                        cblSubjectName.Items.Add(li);
                    });

                    //int shopCount = orderList.Select(s => s.shop.Id).Distinct().ToList().Count;
                    //string text = "<span name='spanCheckLeftShop' style=' color:Blue; cursor:pointer; text-decoration:underline;'>" + shopCount + "</span>";
                    //labShopCount.Text = text;

                    //Dictionary<int, string> dicSubject = new Dictionary<int, string>();
                    //List<string> POSScaleList = new List<string>();
                    //orderList.ForEach(s =>
                    //{
                        
                    //    int subjectId = s.subject.Id;
                    //    string subjectName = s.subject.SubjectName;
                    //    if (s.subject.SubjectType == (int)SubjectTypeEnum.补单)
                    //    {
                    //        subjectId = s.subject.HandMakeSubjectId ?? 0;

                    //    }
                    //    if (!dicSubject.Keys.Contains(subjectId))
                    //    {
                    //        Subject subjectModel = subjectBll.GetModel(subjectId);
                    //        if (subjectModel != null)
                    //        {
                    //            subjectName = subjectModel.SubjectName;
                    //        }
                    //        dicSubject.Add(subjectId, subjectName);
                    //    }
                    //    if (!POSScaleList.Contains(s.POSScale))
                    //    {
                    //        POSScaleList.Add(s.POSScale);
                    //    }

                    //});
                    //foreach (KeyValuePair<int, string> item in dicSubject)
                    //{
                    //    ListItem li = new ListItem();
                    //    li.Value = item.Key.ToString();
                    //    li.Text = item.Value + "&nbsp;&nbsp;";
                    //    cblSubjectName.Items.Add(li);
                    //}
                    //bool isEmpty = false;
                    //POSScaleList.OrderBy(s => s).ToList().ForEach(s =>
                    //{
                    //    if (string.IsNullOrWhiteSpace(s))
                    //    {
                    //        isEmpty = true;
                    //    }
                    //    else
                    //    {
                    //        ListItem li = new ListItem();
                    //        li.Value = s;
                    //        li.Text = s + "&nbsp;&nbsp;";
                    //        cblPOSScale.Items.Add(li);
                    //    }
                    //});
                    //if (isEmpty)
                    //{
                        
                    //    cblPOSScale.Items.Add(new ListItem("空", "空"));
                    //}
                   

                }
            }
            

        }

        public void BindPOSScale()
        {
            cblPOSScale.Items.Clear();
            labShopCount.Text = "";
            
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             //&& (subject.SubjectType != (int)SubjectTypeEnum.二次安装费)
                             //&& shop.IsInstall != null && shop.IsInstall == "Y"
                            && ((shop.IsInstall != null && shop.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                             && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                             && (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                             //&& (shop.Status == null || shop.Status == "" || shop.Status == "正常")
                             select new
                             {
                                 subject,
                                 // subjectType.SubjectTypeName,
                                 // pop,
                                 shop,
                                 //order,
                                 //ShopId=shop.Id,
                                 POSScale = order.InstallPricePOSScale
                             }).ToList();

           
            List<int> assginShopList = new List<int>();
            assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();
            
            if (assginShopList.Any())
            {
                orderList = orderList.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
            }
            if (orderList.Any())
            {
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

                if (regions.Any())
                {
                    orderList = orderList.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
                }
                if (subjectTypeList.Any())
                {
                    //orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                    if (subjectTypeList.Contains(0))
                    {
                        subjectTypeList.Remove(0);
                        if (subjectTypeList.Any())
                        {
                            orderList = orderList.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                        }
                        else
                            orderList = orderList.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                    }
                    else
                        orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                }
                if (subjectIdList.Any())
                {
                    //orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId??0)).ToList();
                    orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
                }
                
                if (orderList.Any())
                {
                    
                    //int shopCount = orderList.Select(s => s.shop.Id).Distinct().ToList().Count;
                    //string text = "<span name='spanCheckLeftShop' style=' color:Blue; cursor:pointer; text-decoration:underline;'>" + shopCount + "</span>";
                    //labShopCount.Text = text;
                    List<string> POSScaleList = new List<string>();
                    //int emptyPOSScaleShopNum = 0;
                    //bool isEmpty = false;
                    //List<int> shopIdList = new List<int>();
                    //orderList.ForEach(s =>
                    //{
                    //    if (!shopIdList.Contains(s.shop.Id))
                    //    {
                    //        shopIdList.Add(s.shop.Id);
                    //        if (string.IsNullOrWhiteSpace(s.POSScale))
                    //            emptyPOSScaleShopNum++;
                    //        if (!POSScaleList.Contains(s.POSScale))
                    //        {
                    //            POSScaleList.Add(s.POSScale);
                    //        }
                    //    }

                    //});

                    //POSScaleList.OrderBy(s => s).ToList().ForEach(s =>
                    //{
                    //    if (string.IsNullOrWhiteSpace(s))
                    //    {
                    //        isEmpty = true;
                    //    }
                    //    else
                    //    {
                    //        ListItem li = new ListItem();
                    //        li.Value = s;
                    //        li.Text = s + "&nbsp;&nbsp;";
                    //        cblPOSScale.Items.Add(li);
                    //    }
                    //});
                    //if (isEmpty)
                    //{
                       
                    //    cblPOSScale.Items.Add(new ListItem("空", "空"));
                    //}

                    var notEmptyList = orderList.Where(s=>s.POSScale!=null && s.POSScale!="").ToList();
                    var EmptyList = orderList.Where(s => s.POSScale == null || s.POSScale == "").ToList();
                    List<int> notEmptyShopIdList = notEmptyList.Select(s => s.shop.Id).Distinct().ToList();
                    EmptyList = EmptyList.Where(s => !notEmptyShopIdList.Contains(s.shop.Id)).ToList();
                    POSScaleList = notEmptyList.Select(s => s.POSScale).Distinct().OrderBy(s=>s).ToList();
                    POSScaleList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s;
                        li.Text = s + "&nbsp;&nbsp;";
                        cblPOSScale.Items.Add(li);
                    });
                    if (EmptyList.Any())
                    {

                        cblPOSScale.Items.Add(new ListItem("空", "空"));
                    }
                }
            }
            
        }

        void GetShopCount()
        {
            labShopCount.Text = "";
            
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             //join subjectType1 in CurrentContext.DbContext.SubjectType
                             //on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                             //from subjectType in typeTemp.DefaultIfEmpty()
                             where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                             //&& (subject.SubjectType != (int)SubjectTypeEnum.二次安装费)
                             //&& shop.IsInstall != null && shop.IsInstall == "Y"
                            && ((shop.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                             && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                             && (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                             select new
                             {
                                 subject,
                                 shop,
                                 POSScale = order.InstallPricePOSScale
                             }).ToList();

            
            List<int> assginShopList = new List<int>(); 
            assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId ?? 0).Distinct().ToList();
            
            if (assginShopList.Any())
            {
                orderList = orderList.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
            }

            if (orderList.Any())
            {
                int totalShopCount = orderList.Select(s => s.shop).Distinct().Count();
                labTotalShopCount.Text = totalShopCount.ToString();
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
                if (regions.Any())
                {
                    orderList = orderList.Where(s => regions.Contains(s.shop.RegionName.ToLower())).ToList();
                }
                if (subjectTypeList.Any())
                {
                    //orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                    if (subjectTypeList.Contains(0))
                    {
                        subjectTypeList.Remove(0);
                        if (subjectTypeList.Any())
                        {
                            orderList = orderList.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                        }
                        else
                            orderList = orderList.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                    }
                    else
                        orderList = orderList.Where(s => subjectTypeList.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                }
                if (subjectIdList.Any())
                {
                    //orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id) || subjectIdList.Contains(s.subject.HandMakeSubjectId??0)).ToList();
                    orderList = orderList.Where(s => subjectIdList.Contains(s.subject.Id)).ToList();
                }

                if (posScaleList.Any())
                {
                    if (posScaleList.Contains("空"))
                    {
                        posScaleList.Remove("空");
                        if (posScaleList.Any())
                        {
                            orderList = orderList.Where(s => posScaleList.Contains(s.POSScale) || s.POSScale == "" || s.POSScale == null).ToList();
                        }
                        else
                            orderList = orderList.Where(s => s.POSScale == "" || s.POSScale == null).ToList();
                    }
                    else
                        orderList = orderList.Where(s => posScaleList.Contains(s.POSScale)).ToList();
                }
                if (orderList.Any())
                {
                    List<int> shopIdList = orderList.Select(s => s.shop.Id).Distinct().ToList();
                    int shopCount = shopIdList.Count;
                    string text = "<span name='spanCheckLeftShop' style=' color:Blue; cursor:pointer; text-decoration:underline;'>" + shopCount + "</span>";
                    labShopCount.Text = text;
                }
                else
                    labShopCount.Text = "0";
            }
            else
               labTotalShopCount.Text = "0";

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
                        && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                             
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> selectRegion = new List<string>();
            List<int> selectSubjectType = new List<int>();
            List<int> selectSubject = new List<int>();
            List<string> selectPOSScale = new List<string>();

            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !selectRegion.Contains(li.Value))
                    selectRegion.Add(li.Value);
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
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        //join subjectType1 in CurrentContext.DbContext.SubjectType
                        //on subject.SubjectTypeId equals subjectType1.Id into typeTemp
                        //from subjectType in typeTemp.DefaultIfEmpty()
                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                        on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                        from subjectCategory in categortTemp.DefaultIfEmpty()
                        join pop1 in CurrentContext.DbContext.POP
                        on new { order.ShopId, order.Sheet, order.GraphicNo } equals new { pop1.ShopId, pop1.Sheet, pop1.GraphicNo } into popTemp
                        from pop in popTemp.DefaultIfEmpty()
                        where subject.GuidanceId == guidanceId 
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (order.IsDelete == null || order.IsDelete == false)
                        //&& ((shop.IsInstall != null && shop.IsInstall == "Y")||(subject.CornerType=="三叶草" && shop.BCSInstallPrice!=null && shop.BCSInstallPrice>0))
                        && ((shop.IsInstall != null && shop.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && (cityCierList.Contains(shop.CityTier) || (shop.BCSInstallPrice != null || shop.BCSInstallPrice > 0))))
                        && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                        && (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))     
                        select new
                        {
                            subject,
                            pop,
                            shop,
                            order,
                            POSScale = order.InstallPricePOSScale,
                            CategoryName = subjectCategory!=null?(subjectCategory.CategoryName):""
                        }).ToList();
            //去掉已归类的店铺
            //var priceShopList = ;
            List<int> assginShopList = new List<int>();
            assginShopList = new InstallPriceShopInfoBLL().GetList(s => s.GuidanceId == guidanceId).Select(s => s.ShopId??0).Distinct().ToList();
            
            if (assginShopList.Any())
            {
                list = list.Where(s => !assginShopList.Contains(s.shop.Id)).ToList();
            }


            if (selectRegion.Any())
            {
                list = list.Where(s => selectRegion.Contains(s.shop.RegionName)).ToList();
            }
            if (selectSubjectType.Any())
            {
                //list = list.Where(s => selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
                if (selectSubjectType.Contains(0))
                {
                    selectSubjectType.Remove(0);
                    if (selectSubjectType.Any())
                    {
                        list = list.Where(s => (s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0) || (selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0))).ToList();
                    }
                    else
                        list = list.Where(s => s.subject.SubjectTypeId == null || s.subject.SubjectTypeId == 0).ToList();
                }
                else
                    list = list.Where(s => selectSubjectType.Contains(s.subject.SubjectTypeId ?? 0)).ToList();
            }
            if (selectSubject.Any())
            {
                //list = list.Where(s => selectSubject.Contains(s.subject.Id) || selectSubject.Contains(s.subject.HandMakeSubjectId ?? 0)).ToList();
                list = list.Where(s => selectSubject.Contains(s.subject.Id)).ToList();
            }
            if (selectPOSScale.Any())
            {
                if (selectPOSScale.Contains("空"))
                {
                    List<string> temp = new List<string>();
                    temp.AddRange(selectPOSScale);
                    temp.Remove("空");
                    if (temp.Any())
                    {
                        list = list.Where(s => temp.Contains(s.POSScale) || s.POSScale == "" || s.POSScale == null).ToList();
                    }
                    else
                        list = list.Where(s => s.POSScale == "" || s.POSScale == null).ToList();


                }
                else
                    list = list.Where(s => selectPOSScale.Contains(s.POSScale)).ToList();
            }
            if (list.Any())
            {
                //非常规
                //var notGenericList = list.Where(s=>!s.CategoryName.Contains("常规")).ToList();
                //常规
                //var genericList = list.Where(s => s.CategoryName.Contains("常规")).ToList();

                List<int> shopIdList = list.Select(s => s.shop.Id).Distinct().ToList();
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
                List<int> windowSheetShopIdList = list.Where(s => s.order.Sheet == "橱窗" || s.order.Sheet.ToLower() == "window" && (!s.CategoryName.Contains("常规-非活动"))).Select(s => s.shop.Id).Distinct().ToList();

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
                                
                                if (BCSInstallCityTierList.Contains(s.shop.CityTier.ToUpper()))
                                {
                                    basicPrice = bcsBasicInstallPrice;
                                }
                                else
                                {
                                    basicPrice = (s.shop.BCSInstallPrice ?? 0);
                                }
                            }
                            else if (s.CategoryName.Contains("常规-非活动"))
                            {
                                if (BCSInstallCityTierList.Contains(s.shop.CityTier.ToUpper()))
                                {
                                    basicPrice = bcsBasicInstallPrice;
                                }
                                else {
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
                    var oohOrderList = list.Where(s => s.pop != null && (s.order.Sheet == "户外" || s.order.Sheet.ToLower() == "ooh") && (s.pop.OOHInstallPrice ?? 0) > 0).Select(s => new { ShopId = s.shop.Id, OOHInstallPrice = s.pop.OOHInstallPrice ?? 0 }).ToList();
                    if (oohOrderList.Any())
                    {
                        var oohList = (from order in oohOrderList
                                       group order by new
                                       {
                                           order.ShopId
                                       } into item
                                       select new
                                       {
                                           item.Key.ShopId,
                                           OOHInstallPrice = item.Max(s => s.OOHInstallPrice)
                                       }).ToList();
                        var finaloohList = (from order in oohOrderList
                                            join ooh in oohList
                                            on new { order.ShopId, order.OOHInstallPrice } equals new { ooh.ShopId, ooh.OOHInstallPrice }
                                            select new
                                            {

                                                ooh.ShopId,
                                                ooh.OOHInstallPrice
                                            }).Distinct().ToList();
                        if (finaloohList.Any())
                        {
                            finaloohList.ForEach(f =>
                            {
                                var model0 = installPriceShopList.Where(sh => sh.ShopId == f.ShopId).FirstOrDefault();
                                if (model0 != null)
                                {
                                    int index = installPriceShopList.IndexOf(model0);
                                    model0.OOHPrice = f.OOHInstallPrice;
                                    installPriceShopList[index] = model0;
                                }
                            });
                        }
                        oohInstallPrice = finaloohList.Sum(s => s.OOHInstallPrice);

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
                    //model.ShopCount += shopIdList.Count;
                   
                    //model.SubjectId = subjectId;
                    //var shopList = installShopBll.GetList(s=>s.InstallDetailId==model.Id);
                    //if (shopList.Any())
                    //{
                    //    model.ShopCount += shopList.Select(s => s.ShopId ?? 0).Distinct().Count();
                    //    model.Price += (shopList.Sum(s => s.BasicPrice ?? 0) + shopList.Sum(s => s.OOHPrice ?? 0) + shopList.Sum(s => s.WindowPrice ?? 0));
                    //}
                    installPriceBll.Update(model);

                }
                else
                {
                    try
                    {
                        model = new InstallPriceDetail();
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
                    
                    
                    installPriceShopList.ForEach(s => {
                        var model2 = installShopBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == s.ShopId).FirstOrDefault();
                        if (model2 == null)
                        {
                            InstallPriceShopInfo model1 = new InstallPriceShopInfo();
                            model1 = s;
                            model1.InstallDetailId = model.Id;
                            model1.SubjectId = subjectId;
                            installShopBll.Add(model1);
                        }
                    });
                }
                //BindGuidance();
                BindRegion();
                BindSubjectType();
                BindSubject();
                BindPOSScale();
                GetShopCount();
                BindData();
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
                    //new InstallPriceShopInfoBLL().Delete(s=>s.SubjectId==model.SubjectId);
                    
                    List<int> installShopIdList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                         join subject in CurrentContext.DbContext.Subject
                                         on order.SubjectId equals subject.Id
                                         join shop in CurrentContext.DbContext.Shop
                                         on order.ShopId equals shop.Id
                                         where subject.GuidanceId == guidanceId && (subject.IsDelete == null || subject.IsDelete == false)
                                         && subject.ApproveState == 1
                                         && (order.IsDelete == null || order.IsDelete == false)
                                         && ((shop.IsInstall != null && shop.IsInstall == "Y") || (subject.CornerType == "三叶草" && shop.BCSInstallPrice != null && shop.BCSInstallPrice > 0))
                                         && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                                         && (isRegionOrder ? ((order.IsFromRegion != null && order.IsFromRegion == true) && myRegionList.Contains(order.Region.ToLower())) : (order.IsFromRegion == null || order.IsFromRegion == false))
                                         select shop.Id).Distinct().ToList();
                    if (installShopIdList.Any())
                    {

                        installShopBll.Delete(s => s.SubjectId == model.SubjectId && installShopIdList.Contains(s.ShopId ?? 0));
                        var list = installShopBll.GetList(s=>s.SubjectId==model.SubjectId);
                        if(!list.Any())
                           bll.Delete(model);
                    }
                    BindRegion();
                    BindSubjectType();
                    BindSubject();
                    BindPOSScale();
                    GetShopCount();
                    BindData();
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
                                && myInstallShopIdList.Contains(installShop.ShopId??0)
                                select installShop).ToList();
                    if (list.Any())
                    {
                        int shopCount = list.Select(s => s.ShopId ?? 0).Distinct().Count();
                        List<string> posScaleList = list.Select(s => s.POSScale).Distinct().ToList();
                        int index = posScaleList.IndexOf("");
                        if (index!=-1)
                           posScaleList[index] = "空";
                        string posScale = StringHelper.ListToString(posScaleList);
                        
                        ((Label)e.Item.FindControl("labSelectPOSScale")).Text = posScale;
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("GuidanceList.aspx?comeback=1", false);
        }

        
    }
}
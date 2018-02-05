using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;

namespace WebApp.Subjects
{
    public partial class ShowOrderDetail : BasePage
    {
        public int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            
            if (!IsPostBack)
            {
                
                BindSubject();
                GetApproveInfo();
                
            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                join user in CurrentContext.DbContext.UserInfo
                                on subject.AddUserId equals user.UserId
                                join subjectType1 in CurrentContext.DbContext.SubjectType
                                on subject.SubjectTypeId equals subjectType1.Id into subjectTypeTemp
                                from subjectType in subjectTypeTemp.DefaultIfEmpty()
                                join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                                from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                                where subject.Id == subjectId
                                select new
                                {
                                    subject,
                                    customer.CustomerName,
                                    AddUserName = user.UserName,
                                    subjectType.SubjectTypeName,
                                    subjectCategory.CategoryName
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                labSubjectNo.Text = subjectModel.subject.SubjectNo;
                labSubjectName.Text = subjectModel.subject.SubjectName;
                labIsSecondInstall.Text = (subjectModel.subject.IsSecondInstall??false)?"是":"否";
                labBeginDate.Text = subjectModel.subject.BeginDate != null ? DateTime.Parse(subjectModel.subject.BeginDate.ToString()).ToShortDateString() : "";
                labEndDate.Text = subjectModel.subject.EndDate != null ? DateTime.Parse(subjectModel.subject.EndDate.ToString()).ToShortDateString() : "";
                labAddUserName.Text = subjectModel.AddUserName;
                labCustomerName.Text = subjectModel.CustomerName;
                int subjectType = subjectModel.subject.SubjectType ?? 1;
                labOrderType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());
                labRemark.Text = subjectModel.subject.Remark;
                hfSubjectType.Value = subjectType.ToString();
                hfPlanIds.Value = subjectModel.subject.SplitPlanIds;
                labPriceBlong.Text = !string.IsNullOrWhiteSpace(subjectModel.subject.PriceBlongRegion) ? subjectModel.subject.PriceBlongRegion : "默认";
                labSubjectType.Text = subjectModel.SubjectTypeName;
                labSubjectCategory.Text = subjectModel.CategoryName;
                if (Request.QueryString["fromReject"] != null && subjectModel.subject.ApproveState==2)
                {
                    btnEdit.Visible = true;
                    btnDelete.Visible = true;
                }
            }
        }


        void BindList()
        {

            var listOrder = (from list in CurrentContext.DbContext.ListOrderDetail
                             join shop in CurrentContext.DbContext.Shop
                             on list.ShopId equals shop.Id

                             where list.SubjectId == subjectId && (list.IsDelete == null || list.IsDelete == false)
                             select new
                             {
                                 list.LevelNum,
                                 list.Sheet,
                                 list,
                                 shop
                             }).ToList();

            //var listOrder = (from list in CurrentContext.DbContext.ListOrderDetail
                             

            //                 where list.SubjectId == subjectId && (list.IsDelete == null || list.IsDelete == false)
            //                 select new
            //                 {
            //                     list.LevelNum,
            //                     list.Sheet,
            //                     list
                                 
            //                 }).ToList();

            string shopNo = txtListShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                listOrder = listOrder.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPagerList.RecordCount = listOrder.Count;
            this.AspNetPagerList.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerList.CurrentPageIndex, this.AspNetPagerList.PageCount, this.AspNetPagerList.RecordCount, this.AspNetPagerList.PageSize });
            repeater_List.DataSource = listOrder.OrderBy(s => s.list.ShopId).Skip((AspNetPagerList.CurrentPageIndex - 1) * AspNetPagerList.PageSize).Take(AspNetPagerList.PageSize).ToList();

            repeater_List.DataBind();
            
        }

        void BindPOP()
        {
            var popOrder = (from order in CurrentContext.DbContext.POPOrderDetail
                            join pop in CurrentContext.DbContext.POP

                            on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop.ShopId, pop.GraphicNo, pop.Sheet }
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id

                            where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                            select new
                            {
                                order,

                                pop,
                                shop

                            }).ToList();
            //var popOrder = (from order in CurrentContext.DbContext.POPOrderDetail
                            
            //                where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
            //                select new
            //                {
            //                    order
            //                }).ToList();
            string shopNo = txtPOPShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                popOrder = popOrder.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPagerPOP.RecordCount = popOrder.Count;
            this.AspNetPagerPOP.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerPOP.CurrentPageIndex, this.AspNetPagerPOP.PageCount, this.AspNetPagerPOP.RecordCount, this.AspNetPagerPOP.PageSize });
            repeater_POPList.DataSource = popOrder.OrderBy(s => s.order.ShopId).Skip((AspNetPagerPOP.CurrentPageIndex - 1) * AspNetPagerPOP.PageSize).Take(AspNetPagerPOP.PageSize).ToList();

            repeater_POPList.DataBind();
            
        }

        void BindSupplement()
        {


            //补充订单
            var supplementOrder = (from list in CurrentContext.DbContext.SupplementOrderDetail
                                   join shop in CurrentContext.DbContext.Shop
                                   on list.ShopId equals shop.Id

                                   where list.SubjectId == subjectId && (list.IsDelete == null || list.IsDelete == false)
                                   select new
                                   {

                                       list,
                                       shop
                                   }).ToList();
            string shopNo = txtBCShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                supplementOrder = supplementOrder.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPagerBC.RecordCount = supplementOrder.Count;
            this.AspNetPagerBC.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerBC.CurrentPageIndex, this.AspNetPagerBC.PageCount, this.AspNetPagerBC.RecordCount, this.AspNetPagerBC.PageSize });
            repeater_BCList.DataSource = supplementOrder.OrderBy(s => s.list.ShopId).Skip((AspNetPagerBC.CurrentPageIndex - 1) * AspNetPagerBC.PageSize).Take(AspNetPagerBC.PageSize).ToList();

            repeater_BCList.DataBind();

        }

        void BindMerge()
        {
            var MergeList = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                             join pop1 in CurrentContext.DbContext.POP
                             on new { merge.GraphicNo, merge.ShopId, merge.Sheet } equals new { pop1.GraphicNo, pop1.ShopId, pop1.Sheet } into popTemp
                             join shop in CurrentContext.DbContext.Shop
                             on merge.ShopId equals shop.Id
                             from pop in popTemp.DefaultIfEmpty()
                             where merge.SubjectId == subjectId && (merge.IsDelete == null || merge.IsDelete == false)
                             select new
                             {
                                 merge.LevelNum,
                                 merge.Sheet,
                                 merge,
                                 shop,
                                 pop

                             }).ToList();

            ////var MergeList = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                             
            ////                 where merge.SubjectId == subjectId && (merge.IsDelete == null || merge.IsDelete == false)
            ////                 select new
            ////                 {
            ////                     merge.LevelNum,
            ////                     merge.Sheet,
            ////                     merge
                                

            ////                 }).ToList();

            string shopNo = txtMergeShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                MergeList = MergeList.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPagerMerge.RecordCount = MergeList.Count;
            this.AspNetPagerMerge.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerMerge.CurrentPageIndex, this.AspNetPagerMerge.PageCount, this.AspNetPagerMerge.RecordCount, this.AspNetPagerMerge.PageSize });
            repeater_MergeList.DataSource = MergeList.OrderBy(s => s.merge.ShopId).Skip((AspNetPagerMerge.CurrentPageIndex - 1) * AspNetPagerMerge.PageSize).Take(AspNetPagerMerge.PageSize).ToList();

            repeater_MergeList.DataBind();
            
        }

        void BindMaterial()
        {
            var materialList = (from material in CurrentContext.DbContext.OrderMaterial
                                join shop in CurrentContext.DbContext.Shop
                                on material.ShopId equals shop.Id
                                where material.SubjectId == subjectId
                                select new
                                {
                                    material,
                                    shop
                                }).ToList();
            string shopNo = txtMaterialShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                materialList = materialList.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPagerMaterial.RecordCount = materialList.Count;
            this.AspNetPagerMaterial.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerMaterial.CurrentPageIndex, this.AspNetPagerMaterial.PageCount, this.AspNetPagerMaterial.RecordCount, this.AspNetPagerMaterial.PageSize });
            repeater_Material.DataSource = materialList.OrderBy(s => s.material.ShopId).Skip((AspNetPagerMaterial.CurrentPageIndex - 1) * AspNetPagerMaterial.PageSize).Take(AspNetPagerMaterial.PageSize).ToList();

            repeater_Material.DataBind();
        }

        void BindPOPPriceOrder()
        {
            var list = new PriceOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
            string shopNo = txtPriceOrderShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPagerPriceOrder.RecordCount = list.Count;
            this.AspNetPagerPriceOrder.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerPriceOrder.CurrentPageIndex, this.AspNetPagerPriceOrder.PageCount, this.AspNetPagerPriceOrder.RecordCount, this.AspNetPagerPriceOrder.PageSize });
            repeater_OrderPrice.DataSource = list.OrderBy(s => s.ShopId).Skip((AspNetPagerPriceOrder.CurrentPageIndex - 1) * AspNetPagerPriceOrder.PageSize).Take(AspNetPagerPriceOrder.PageSize).ToList();
            repeater_OrderPrice.DataBind();
        }

        protected void orderListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                PriceOrderDetail model = (PriceOrderDetail)e.Item.DataItem;
                if (model != null)
                {
                    string orderType = (model.OrderType ?? 0).ToString();
                    ((Label)e.Item.FindControl("labPriceType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);
                }
            }
        }



        void BindPriceOrder()
        {
            var list = (from order in CurrentContext.DbContext.PriceOrderDetail
                        
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                           
                        }).ToList();

            AspNetPagerPrice.RecordCount = list.Count;
            this.AspNetPagerPrice.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerPrice.CurrentPageIndex, this.AspNetPagerPrice.PageCount, this.AspNetPagerPrice.RecordCount, this.AspNetPagerPrice.PageSize });
            repeater_PriceList.DataSource = list.OrderBy(s => s.order.Id).Skip((AspNetPagerPrice.CurrentPageIndex - 1) * AspNetPagerPrice.PageSize).Take(AspNetPagerPrice.PageSize).ToList();
            repeater_PriceList.DataBind();
            
        }

        void BindPOP1List()
        {
            var list = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            shop
                        }).ToList();
            string shopNo = txtPOP1ShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPagerPOP1.RecordCount = list.Count;
            this.AspNetPagerPOP1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerPOP1.CurrentPageIndex, this.AspNetPagerPOP1.PageCount, this.AspNetPagerPOP1.RecordCount, this.AspNetPagerPOP1.PageSize });
            repeater_POP1List.DataSource = list.OrderBy(s => s.order.Id).Skip((AspNetPagerPOP1.CurrentPageIndex - 1) * AspNetPagerPOP1.PageSize).Take(AspNetPagerPOP1.PageSize).ToList();
            repeater_POP1List.DataBind();
        }

        void BindHCList()
        {
            var list = (from order in CurrentContext.DbContext.HCOrderDetail
                        //join pop in CurrentContext.DbContext.POP
                        //on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop.ShopId, pop.GraphicNo, pop.Sheet }
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                           // pop,
                            shop
                        }).ToList();
            string shopNo = txtHCShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPagerHC.RecordCount = list.Count;
            this.AspNetPagerHC.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerHC.CurrentPageIndex, this.AspNetPagerHC.PageCount, this.AspNetPagerHC.RecordCount, this.AspNetPagerHC.PageSize });
            repeater_HCList.DataSource = list.OrderBy(s => s.order.ShopId).Skip((AspNetPagerHC.CurrentPageIndex - 1) * AspNetPagerHC.PageSize).Take(AspNetPagerHC.PageSize).ToList();

            repeater_HCList.DataBind();
        }

        void BindHCMaterial()
        {
            var materialList = (from material in CurrentContext.DbContext.OrderMaterial
                                join shop in CurrentContext.DbContext.Shop
                                on material.ShopId equals shop.Id
                                where material.SubjectId == subjectId
                                select new
                                {
                                    material,
                                    shop
                                }).ToList();
            string shopNo = txthcmaterialShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                materialList = materialList.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPagerHCMaterial.RecordCount = materialList.Count;
            this.AspNetPagerHCMaterial.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerHCMaterial.CurrentPageIndex, this.AspNetPagerHCMaterial.PageCount, this.AspNetPagerHCMaterial.RecordCount, this.AspNetPagerHCMaterial.PageSize });
            repeater_HCmaterial.DataSource = materialList.OrderBy(s => s.material.ShopId).Skip((AspNetPagerHCMaterial.CurrentPageIndex - 1) * AspNetPagerHCMaterial.PageSize).Take(AspNetPagerHCMaterial.PageSize).ToList();

            repeater_HCmaterial.DataBind();
        }


        void BindFinalOrder()
        {


            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join pop1 in CurrentContext.DbContext.POP
                        on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                        from pop in popTemp.DefaultIfEmpty()
                        //join shop in CurrentContext.DbContext.Shop
                        //on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (order.RegionSupplementId == null || order.RegionSupplementId == 0)
                        select new
                        {
                            order,
                            Sheet = order.Sheet,
                            LevelNum = order.LevelNum,
                            //shop,
                            order.OrderType,
                            pop
                        }).ToList();

            //var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        
            //            where order.SubjectId == subjectId
            //            && (order.IsDelete == null || order.IsDelete == false)
            //            && (order.RegionSupplementId == null || order.RegionSupplementId == 0)
            //            select new
            //            {
            //                order,
            //                Sheet = order.Sheet,
            //                LevelNum = order.LevelNum
            //            }).ToList();

            PanelFinalOrder.Visible = list.Any();
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text.Trim()))
            {
                list = list.Where(s => s.order.ShopNo.ToLower().Contains(txtShopNo.Text.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text.Trim()))
            {
                list = list.Where(s => s.order.ShopName.Contains(txtShopNo.Text.Trim())).ToList();
            }


            AspNetPagerFinal.RecordCount = list.Count;
            this.AspNetPagerFinal.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerFinal.CurrentPageIndex, this.AspNetPagerFinal.PageCount, this.AspNetPagerFinal.RecordCount, this.AspNetPagerFinal.PageSize });
            repeater_FinalOrder.DataSource = list.OrderBy(s => s.order.ShopId).Skip((AspNetPagerFinal.CurrentPageIndex - 1) * AspNetPagerFinal.PageSize).Take(AspNetPagerFinal.PageSize).ToList();
            repeater_FinalOrder.DataBind();

            

        }

        void BindSecondInstallDetail()
        {
            var list = (from detail in CurrentContext.DbContext.SecondInstallFeeDetail
                        join shop in CurrentContext.DbContext.Shop
                        on detail.ShopId equals shop.Id
                        where detail.SubjectId == subjectId
                        select new
                        {
                            detail,
                            shop
                        }).ToList();

            AspNetPagerInstallDetail.RecordCount = list.Count;
            this.AspNetPagerInstallDetail.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerInstallDetail.CurrentPageIndex, this.AspNetPagerInstallDetail.PageCount, this.AspNetPagerInstallDetail.RecordCount, this.AspNetPagerInstallDetail.PageSize });
            detailList.DataSource = list.OrderBy(s => s.detail.Id).Skip((AspNetPagerInstallDetail.CurrentPageIndex - 1) * AspNetPagerInstallDetail.PageSize).Take(AspNetPagerInstallDetail.PageSize).ToList();
            detailList.DataBind();
            PanelSecondInstall.Visible = list.Any();
        }


        void BindSISOrder()
        {
            var list = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            shop
                        }).ToList();

            AspNetPagerSIS.RecordCount = list.Count;
            this.AspNetPagerSIS.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerSIS.CurrentPageIndex, this.AspNetPagerSIS.PageCount, this.AspNetPagerSIS.RecordCount, this.AspNetPagerSIS.PageSize });
            repeaterSIS.DataSource = list.OrderBy(s => s.order.Id).Skip((AspNetPagerSIS.CurrentPageIndex - 1) * AspNetPagerSIS.PageSize).Take(AspNetPagerSIS.PageSize).ToList();
            repeaterSIS.DataBind();
            PanelSISOrder.Visible = list.Any();
        }

        /// <summary>
        /// list订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreachList_Click(object sender, EventArgs e)
        {
            AspNetPagerList.CurrentPageIndex = 1;
            BindList();
        }

        /// <summary>
        /// pop订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreachPOP_Click(object sender, EventArgs e)
        {
            AspNetPagerPOP.CurrentPageIndex = 1;
            BindPOP();
        }

        /// <summary>
        /// 补充订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreachBC_Click(object sender, EventArgs e)
        {
            AspNetPagerBC.CurrentPageIndex = 1;
            BindSupplement();
        }

        /// <summary>
        /// 合并订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreachMerge_Click(object sender, EventArgs e)
        {
            AspNetPagerMerge.CurrentPageIndex = 1;
            BindMerge();
        }

        /// <summary>
        /// 搜索物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreachMaterial_Click(object sender, EventArgs e)
        {
            AspNetPagerMaterial.CurrentPageIndex = 1;
            BindMaterial();
        }

        protected void btnSreachPriceOrder_Click(object sender, EventArgs e)
        {
            AspNetPagerPriceOrder.CurrentPageIndex = 1;
            BindPOPPriceOrder();
        }



        protected void btnSreachPOP1_Click(object sender, EventArgs e)
        {
            AspNetPagerPOP1.CurrentPageIndex = 1;
            BindPOP1List();
        }

        protected void btnSreachHC_Click(object sender, EventArgs e)
        {
            AspNetPagerHC.CurrentPageIndex = 1;
            BindHCList();
        }

        protected void btnSearchFinal_Click(object sender, EventArgs e)
        {
            AspNetPagerFinal.CurrentPageIndex = 1;
            BindFinalOrder();
        }


        protected void AspNetPagerList_PageChanged(object sender, EventArgs e)
        {
            BindList();
        }
        protected void AspNetPagerPOP_PageChanged(object sender, EventArgs e)
        {
            BindPOP();
        }
        protected void AspNetPagerBC_PageChanged(object sender, EventArgs e)
        {
            BindSupplement();
        }
        protected void AspNetPagerMerge_PageChanged(object sender, EventArgs e)
        {
            BindMerge();
        }
        protected void AspNetPagerMaterial_PageChanged(object sender, EventArgs e)
        {
            BindMaterial();
        }

        protected void AspNetPagerPriceOrder_PageChanged(object sender, EventArgs e)
        {
            BindPOPPriceOrder();
        }


        protected void AspNetPagerPrice_PageChanged(object sender, EventArgs e)
        {
            BindPriceOrder();
        }

        protected void AspNetPagerPOP1_PageChanged(object sender, EventArgs e)
        {
            BindPOP1List();
        }

        protected void AspNetPagerHC_PageChanged(object sender, EventArgs e)
        {
            BindHCList();
        }

        protected void AspNetPagerHCMaterial_PageChanged(object sender, EventArgs e)
        {
            BindHCMaterial();
        }

        protected void AspNetPagerFinal_PageChanged(object sender, EventArgs e)
        {
            BindFinalOrder();
        }

        protected void AspNetPagerInstallDetail_PageChanged(object sender, EventArgs e)
        {
            BindSecondInstallDetail();
        }

        protected void AspNetPagerSIS_PageChanged(object sender, EventArgs e)
        {
            BindSISOrder();
        }

        void LoadOrders()
        {
            int subjectType = !string.IsNullOrWhiteSpace(hfSubjectType.Value) ? int.Parse(hfSubjectType.Value) : 0;
            switch (subjectType)
            {
                case (int)SubjectTypeEnum.POP订单:
                default:
                    LoadPOPOrder();
                    break;
                case (int)SubjectTypeEnum.新开店安装费:
                case (int)SubjectTypeEnum.运费:
                    LoadPriceOrder();
                    break;
                case (int)SubjectTypeEnum.手工订单:
                
                    LoadHC();
                    break;
                case (int)SubjectTypeEnum.二次安装:
                    BindSecondInstallDetail();
                    break;
                //case 5:
                //    BindSISOrder();
                //    break;
            }
            Panel1.Visible = false;
        }

        void LoadPOPOrder()
        {
            BindList();
            BindPOP();
            BindSupplement();
            BindMerge();
            BindMaterial();
            BindPOPPriceOrder();
            BindFinalOrder();
            StatisticsInfo();
            PanelPOPOrder.Visible = true;
        }

        void LoadPriceOrder()
        {
            BindPriceOrder();
            PanelPriceOrder.Visible = true;
        }

        void LoadHC()
        {
            BindPOP1List();
            BindHCList();
            BindHCMaterial();
            PanelSupplementOrder.Visible = true;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        /// <summary>
        /// 显示审批记录
        /// </summary>
        void GetApproveInfo()
        {
            var list = (from approve in CurrentContext.DbContext.ApproveInfo
                        join user in CurrentContext.DbContext.UserInfo
                        on approve.AddUserId equals user.UserId
                        where approve.SubjectId == subjectId
                        select new
                        {
                            approve,
                            UserName=user.RealName,
                        }).ToList();
            if (list.Any())
            {
                Dictionary<int, string> approveStateDic = new Dictionary<int, string>();
                approveStateDic.Add(0, "待审批");
                approveStateDic.Add(1, "<span style='color:green;'>审批通过</span>");
                approveStateDic.Add(2, "<span style='color:red;'>审批不通过</span>");
                StringBuilder tb = new StringBuilder();
                list.ForEach(s =>
                {
                    int approveState = s.approve.Result ?? 0;
                    
                    tb.Append("<table class=\"table\" style=\"margin-bottom:6px;\">");
                    tb.AppendFormat("<tr class=\"tr_hui\"><td style=\"width: 100px;\">审批时间</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", s.approve.AddDate);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批结果</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", approveStateDic[approveState]);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批人</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", s.UserName);
                    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批意见</td><td style=\"text-align: left; padding-left: 5px;height: 60px;\">{0}</td></tr>", s.approve.Remark);
                    tb.Append("</table>");


                });
                approveInfoDiv.InnerHtml = tb.ToString();
                Panel_ApproveInfo.Visible = true;
            }
        }

        void StatisticsInfo()
        {
            var orderList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false));
            if (orderList.Any())
            {
                List<int> shopIdList = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                shopIdList.Remove(0);
                labShopCount.Text = shopIdList.Count.ToString();
                decimal totalArea = 0;
                decimal totalPrice = 0;
                StatisticPOPTotalPrice(orderList, out totalPrice, out totalArea);
                if (totalArea > 0)
                {
                    labTotalArea.Text = Math.Round(totalArea, 2) + " 平方米";
                }
                else
                    labTotalArea.Text = "0";
                if (totalPrice > 0)
                {
                    labTotalPrice.Text = Math.Round(totalPrice, 2) + " 元";
                }
                else
                    labTotalPrice.Text = "0";
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            var model = new SubjectBLL().GetModel(subjectId);
            int subjectType = 1;
            if (model != null)
            {
                subjectType = model.SubjectType ?? 1;
            }
            string url = string.Format("AddSubject.aspx?subjectId={0}", subjectId);
            if (subjectType == (int)SubjectTypeEnum.二次安装)
            {
                url = string.Format("/Subjects/SecondInstallFee/Add.aspx?subjectId={0}", subjectId);
            }
            else if (subjectType == (int)SubjectTypeEnum.补单)
            {
                url = string.Format("/Subjects/HandMadeOrder/Add.aspx?subjectId={0}", subjectId);
            }
            Response.Redirect(url, false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SubjectBLL bll = new SubjectBLL();
            Subject model = bll.GetModel(subjectId);
            if (model != null)
            {
                model.IsDelete = true;
                model.DeleteDate = DateTime.Now;
                bll.Update(model);
                Alert("删除成功！", "/Subjects/RejectSubjectList.aspx");
            }
        }

        protected void repeater_FinalOrder_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object orderModel = e.Item.DataItem;
                if (orderModel != null)
                {
                    Label labOrderType = (Label)e.Item.FindControl("labOrderType");
                    object objOrderType = orderModel.GetType().GetProperty("OrderType").GetValue(orderModel,null);
                    if (objOrderType!=null)
                        labOrderType.Text = CommonMethod.GeEnumName<OrderTypeEnum>(objOrderType.ToString());
                }
            }
        }

        protected void btnSreachHCMaterial_Click(object sender, EventArgs e)
        {
            BindHCMaterial();
        }

       
    }
}
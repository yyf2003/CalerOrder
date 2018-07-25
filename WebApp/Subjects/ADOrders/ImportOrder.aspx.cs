using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using BLL;
using Common;
using DAL;
using Models;
using System.Web;
using System.Transactions;
using System.Web.UI.WebControls;

namespace WebApp.Subjects.ADOrders
{
    public partial class ImportOrder : BasePage
    {

        public int subjectId;
        public string fail = "0";
        public int hasOrder = 0;
        MergeOriginalOrderBLL mergeOrderBll = new MergeOriginalOrderBLL();
        List<string> HCShop = new List<string>();
        List<String> ChangePOPCountSheetList = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {


            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);

            }

            if (Request.QueryString["import"] != null)
            {
                Panel1.Visible = true;
                if (Session["failMsg"] != null || Session["errorTb"] != null)
                {
                    labState.Text = "部分数据导入失败！";
                    ExportFailMsg.Style.Add("display", "block");
                }
                if (Session["tips"] != null)
                {
                    List<string> tipsList = Session["tips"] as List<string>;
                    if (tipsList.Any())
                    {
                        string msg = StringHelper.ListToString(tipsList);
                        labTips.Text = "提示：" + msg + " 表中的‘物料支持’列并没发现类似 Basic,Premium,MCS,VVIP 这些，请检查，如果确认没问题，请忽略该提示！";

                    }
                }
                if (Session["ListColumnsError"] != null)
                {
                    labState.Text = "导入失败：List订单模板不正确，请下载最新模板！";
                }
                if (Session["emptyFrame"] != null)
                {
                    ExportEmptyFrame.Style.Add("display", "block");
                }
                if (Session["POPWrongFrame"] != null)
                {
                    ExportPOPEmptyFrame.Style.Add("display", "block");
                }
                if (Session["POPPlaceWarning"] != null)
                {
                    ExportPOPPlaceWarning.Style.Add("display", "block");
                }
                if (Session["errorMaterialSupport"] != null)
                {
                    ErrorMaterialSupportWarning.Style.Add("display", "block");
                    hfHasErrorMaterialSupport.Value = "1";
                }
                else
                {
                    hfHasErrorMaterialSupport.Value = "";
                }
            }
            else
            {
                if (Session["errorMaterialSupport"] != null)
                {
                    ErrorMaterialSupportWarning.Style.Add("display", "block");
                    hfHasErrorMaterialSupport.Value = "1";
                }
                else
                {
                    hfHasErrorMaterialSupport.Value = "";
                }
            }
            if (!IsPostBack)
            {
                BindSubject();
                DataTable errorTB = null;
                if (!CheckMaterialSupport(subjectId, out errorTB))
                {

                    if (errorTB != null && errorTB.Rows.Count > 0)
                    {
                        labState.Text = "警告：";
                        Session["errorMaterialSupport"] = errorTB;
                        hfHasErrorMaterialSupport.Value = "1";
                        Panel1.Visible = true;
                        ErrorMaterialSupportWarning.Style.Add("display", "block");
                    }
                    else
                    {
                        Panel1.Visible = false;
                        labState.Text = "导入完成";
                        Session["errorMaterialSupport"] = null;
                        hfHasErrorMaterialSupport.Value = "";
                    }
                }

            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                where subject.Id == subjectId
                                select new
                                {
                                    subject.GuidanceId,
                                    subject.SubjectName,
                                    subject.SubjectNo,
                                    customer.CustomerName,
                                    subject.CustomerId,
                                    subject.SubjectType
                                }).FirstOrDefault();
            if (subjectModel != null)
            {

                hfCustomerId.Value = (subjectModel.CustomerId ?? 0).ToString();
                labSubjectName.Text = subjectModel.SubjectName;
                labSubjectNo.Text = subjectModel.SubjectNo;
                labCustomer.Text = subjectModel.CustomerName;
                int subjectType = subjectModel.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                hfSubjectType.Value = subjectType.ToString();
                hfGuidanceId.Value = (subjectModel.GuidanceId ?? 0).ToString();
                switch (subjectType)
                {
                    case (int)SubjectTypeEnum.POP订单:
                    default:
                        var list = mergeOrderBll.GetList(s => s.SubjectId == subjectId);
                        if (list.Any())
                            hasOrder = 1;
                        break;
                    case (int)SubjectTypeEnum.新开店安装费:
                    case (int)SubjectTypeEnum.运费:
                        var list1 = new PriceOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
                        if (list1.Any())
                            hasOrder = 1;
                        break;
                    case (int)SubjectTypeEnum.手工订单:
                        var list2 = new HandMadeOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
                        var list3 = new HCOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
                        var list4 = new OrderMaterialBLL().GetList(s => s.SubjectId == subjectId);
                        if (list2.Any() || list3.Any() || list4.Any())
                            hasOrder = 1;
                        break;

                }

            }
        }

        void BindList()
        {

            //var listOrder = (from list in CurrentContext.DbContext.ListOrderDetail
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on list.ShopId equals shop.Id

            //                 where list.SubjectId == subjectId && (list.IsDelete == null || list.IsDelete == false)
            //                 select new
            //                 {
            //                     list.LevelNum,
            //                     list.Sheet,
            //                     list,
            //                     shop
            //                 }).ToList();
            var listOrder = (from list in CurrentContext.DbContext.ListOrderDetail

                             where list.SubjectId == subjectId && (list.IsDelete == null || list.IsDelete == false)
                             select new
                             {
                                 list.LevelNum,
                                 list.Sheet,
                                 list

                             }).ToList();
            string shopNo = txtListShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                listOrder = listOrder.Where(s => s.list.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPagerList.RecordCount = listOrder.Count;
            this.AspNetPagerList.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerList.CurrentPageIndex, this.AspNetPagerList.PageCount, this.AspNetPagerList.RecordCount, this.AspNetPagerList.PageSize });
            repeater_List.DataSource = listOrder.OrderBy(s => s.list.ShopId).Skip((AspNetPagerList.CurrentPageIndex - 1) * AspNetPagerList.PageSize).Take(AspNetPagerList.PageSize).ToList();

            repeater_List.DataBind();

        }

        void BindPOP()
        {
            try
            {
                //var popOrder = (from order in CurrentContext.DbContext.POPOrderDetail
                //                join pop in CurrentContext.DbContext.POP

                //                on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop.ShopId, pop.GraphicNo, pop.Sheet }
                //                join shop in CurrentContext.DbContext.Shop
                //                on order.ShopId equals shop.Id

                //                where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                //                select new
                //                {
                //                    order,

                //                    pop,
                //                    shop

                //                }).ToList();


                var popOrder = (from order in CurrentContext.DbContext.POPOrderDetail

                                where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                                select new
                                {
                                    order


                                }).ToList();

                string shopNo = txtPOPShopNo.Text.Trim();
                if (!string.IsNullOrWhiteSpace(shopNo))
                {
                    popOrder = popOrder.Where(s => s.order.ShopNo.Contains(shopNo)).ToList();
                }
                AspNetPagerPOP.RecordCount = popOrder.Count;
                this.AspNetPagerPOP.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerPOP.CurrentPageIndex, this.AspNetPagerPOP.PageCount, this.AspNetPagerPOP.RecordCount, this.AspNetPagerPOP.PageSize });
                repeater_POPList.DataSource = popOrder.OrderBy(s => s.order.ShopId).Skip((AspNetPagerPOP.CurrentPageIndex - 1) * AspNetPagerPOP.PageSize).Take(AspNetPagerPOP.PageSize).ToList();

                repeater_POPList.DataBind();
            }
            catch (Exception ex)
            {

            }

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
                supplementOrder = supplementOrder.Where(s => s.shop.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPagerBC.RecordCount = supplementOrder.Count;
            this.AspNetPagerBC.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerBC.CurrentPageIndex, this.AspNetPagerBC.PageCount, this.AspNetPagerBC.RecordCount, this.AspNetPagerBC.PageSize });
            repeater_BCList.DataSource = supplementOrder.OrderBy(s => s.list.ShopId).Skip((AspNetPagerBC.CurrentPageIndex - 1) * AspNetPagerBC.PageSize).Take(AspNetPagerBC.PageSize).ToList();

            repeater_BCList.DataBind();

        }

        void BindMerge()
        {
            //var MergeList = (from merge in CurrentContext.DbContext.MergeOriginalOrder
            //                 join pop1 in CurrentContext.DbContext.POP
            //                 on new { merge.GraphicNo, merge.ShopId, merge.Sheet } equals new { pop1.GraphicNo, pop1.ShopId, pop1.Sheet } into popTemp
            //                 join shop in CurrentContext.DbContext.Shop
            //                 on merge.ShopId equals shop.Id
            //                 from pop in popTemp.DefaultIfEmpty()
            //                 where merge.SubjectId == subjectId && (merge.IsDelete == null || merge.IsDelete == false)
            //                 select new
            //                 {
            //                     merge.LevelNum,
            //                     merge.Sheet,
            //                     merge,
            //                     shop,
            //                     pop

            //                 }).ToList();

            var MergeList = (from merge in CurrentContext.DbContext.MergeOriginalOrder

                             where merge.SubjectId == subjectId && (merge.IsDelete == null || merge.IsDelete == false)
                             select new
                             {
                                 merge.LevelNum,
                                 merge.Sheet,
                                 merge

                             }).ToList();
            string shopNo = txtMergeShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                MergeList = MergeList.Where(s => s.merge.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPagerMerge.RecordCount = MergeList.Count;
            this.AspNetPagerMerge.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerMerge.CurrentPageIndex, this.AspNetPagerMerge.PageCount, this.AspNetPagerMerge.RecordCount, this.AspNetPagerMerge.PageSize });
            repeater_MergeList.DataSource = MergeList.OrderBy(s => s.merge.ShopId).Skip((AspNetPagerMerge.CurrentPageIndex - 1) * AspNetPagerMerge.PageSize).Take(AspNetPagerMerge.PageSize).ToList();

            repeater_MergeList.DataBind();
            if (MergeList.Any())
                hasOrder++;

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
                materialList = materialList.Where(s => s.shop.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPagerMaterial.RecordCount = materialList.Count;
            this.AspNetPagerMaterial.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerMaterial.CurrentPageIndex, this.AspNetPagerMaterial.PageCount, this.AspNetPagerMaterial.RecordCount, this.AspNetPagerMaterial.PageSize });
            repeater_Material.DataSource = materialList.OrderBy(s => s.material.ShopId).Skip((AspNetPagerMaterial.CurrentPageIndex - 1) * AspNetPagerMaterial.PageSize).Take(AspNetPagerMaterial.PageSize).ToList();

            repeater_Material.DataBind();
        }

        void BindPOPPriceOrder()
        {
            var list = priceOrderBll.GetList(s=>s.SubjectId==subjectId);
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
                            order

                        }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            repeater_PriceList.DataSource = list.OrderBy(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            repeater_PriceList.DataBind();
            if (list.Any())
            {
                Panel_Price.Visible = true;
                hasOrder++;
            }
        }
        int hasHMOrder = 0;
        void BindHandMakeOrder_POP()
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
            if (list.Any())
                hasHMOrder++;
        }

        void BindHandMakeOrder_HC()
        {
            var list = (from order in CurrentContext.DbContext.HCOrderDetail

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
            if (list.Any())
                hasHMOrder++;
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
            //string shopNo = txtMaterialShopNo.Text.Trim();
            //if (!string.IsNullOrWhiteSpace(shopNo))
            //{
            //    materialList = materialList.Where(s => s.shop.ShopNo.Contains(shopNo)).ToList();
            //}
            AspNetPagerHCMaterial.RecordCount = materialList.Count;
            this.AspNetPagerHCMaterial.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerHCMaterial.CurrentPageIndex, this.AspNetPagerHCMaterial.PageCount, this.AspNetPagerHCMaterial.RecordCount, this.AspNetPagerHCMaterial.PageSize });
            repeater_HCmaterial.DataSource = materialList.OrderBy(s => s.material.ShopId).Skip((AspNetPagerHCMaterial.CurrentPageIndex - 1) * AspNetPagerHCMaterial.PageSize).Take(AspNetPagerHCMaterial.PageSize).ToList();

            repeater_HCmaterial.DataBind();
            if (materialList.Any())
                hasHMOrder++;
        }

        void BindHCPOPPriceOrder()
        {
            var list = priceOrderBll.GetList(s => s.SubjectId == subjectId);
            string shopNo = txtHCPriceOrderShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPagerHCPriceOrder.RecordCount = list.Count;
            this.AspNetPagerHCPriceOrder.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerHCPriceOrder.CurrentPageIndex, this.AspNetPagerHCPriceOrder.PageCount, this.AspNetPagerHCPriceOrder.RecordCount, this.AspNetPagerHCPriceOrder.PageSize });
            repeater_HCPriceOrderList.DataSource = list.OrderBy(s => s.ShopId).Skip((AspNetPagerHCPriceOrder.CurrentPageIndex - 1) * AspNetPagerHCPriceOrder.PageSize).Take(AspNetPagerHCPriceOrder.PageSize).ToList();
            repeater_HCPriceOrderList.DataBind();
        }
        protected void repeater_HCPriceOrderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
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


        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "ADOrderImpormTemplate";
            int subjectType = 0;
            if (!string.IsNullOrWhiteSpace(hfSubjectType.Value))
                subjectType = int.Parse(hfSubjectType.Value);
            switch (subjectType)
            {
                case (int)SubjectTypeEnum.手工订单:
                    fileName = "HandMadeOrderTemplate";
                    break;
                case (int)SubjectTypeEnum.新开店安装费:
                    fileName = "费用订单模板";
                    break;
                case (int)SubjectTypeEnum.运费:
                    fileName = "运费模板";
                    break;
                default:
                    break;
            }

            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        /// <summary>
        /// 导出失败信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportError_Click(object sender, EventArgs e)
        {
            if (Session["failMsg"] != null)
            {
                //List<DataTable> dts = (List<DataTable>)Session["failMsg"];
                Dictionary<string, DataTable> dts = (Dictionary<string, DataTable>)Session["failMsg"];
                OperateFile.ExportTables(dts, "导入失败信息");
            }
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }

        /// <summary>
        /// 导出空器架订单信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportEmptyFrame_Click(object sender, EventArgs e)
        {
            if (Session["emptyFrame"] != null)
            {
                //DataTable dt = (DataTable)Session["emptyFrame"];
                //OperateFile.ExportExcel(dt, "器架为空的订单信息");
                Dictionary<string, DataTable> dts = (Dictionary<string, DataTable>)Session["emptyFrame"];
                OperateFile.ExportTables(dts, "器架为空的订单信息");
            }
        }

        /// <summary>
        /// 导出POP器架对应错误信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportPOPEmptyFrame_Click(object sender, EventArgs e)
        {
            if (Session["POPWrongFrame"] != null)
            {
                DataTable dt = (DataTable)Session["POPWrongFrame"];
                OperateFile.ExportExcel(dt, "POP器架对应错误信息");
            }
        }
        /// <summary>
        /// 导出POP位置警告信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportPOPPlaceWarning_Click(object sender, EventArgs e)
        {
            if (Session["POPPlaceWarning"] != null)
            {
                DataTable dt = (DataTable)Session["POPPlaceWarning"];
                OperateFile.ExportExcel(dt, "POP位置占用警告信息");
            }
        }



        /// <summary>
        /// 导出物料级别不统一警告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportErrorMaterialSupport_Click(object sender, EventArgs e)
        {
            if (Session["errorMaterialSupport"] != null)
            {
                DataTable dt = (DataTable)Session["errorMaterialSupport"];
                OperateFile.ExportExcel(dt, "物料级别不统一警告信息");
            }
        }



        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNext_Click(object sender, EventArgs e)
        {
            var list = mergeOrderBll.GetList(s => s.SubjectId == subjectId);
            if (list.Any())
            {

                Response.Redirect("SplitOrder.aspx?subjectId=" + subjectId, false);
            }
            else
                Alert("请先导入订单！");
        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(hfSubjectType.Value) && int.Parse(hfSubjectType.Value) == (int)SubjectTypeEnum.正常单)
                Response.Redirect("/Subjects/RegionSubject/AddSubject.aspx?subjectId=" + subjectId, false);
            else
                Response.Redirect("/Subjects/AddSubject.aspx?subjectId=" + subjectId, false);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //导出失败信息的Table
        DataTable ListErrorTB = null;
        DataTable ListEmptyFrameTB = null;
        DataTable POPEmptyFrameTB = null;
        DataTable POPErrorTB = null;
        DataTable POPWrongFrameTB = null;
        DataTable SupplementErrorTB = null;
        DataTable MaterialErrorTB = null;
        DataTable PriceOrderErrorTB = null;
        DataTable DisplayTableErrorTB = null;
        DataTable HMPOPErrorTB = new DataTable();
        DataTable HMHCErrorTB = new DataTable();
        DataTable POPPlaceWarningTB = null;
        int successNum = 0;
        List<ImportTables> importTableList = new List<ImportTables>();
        //List<string> materialSupportList = new List<string>() { "basic", "premium", "mcs", "vvip" };
        List<string> materialSupportList = new List<string>();
        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (Session["failMsg"] != null)
            {
                Session["failMsg"] = null;
            }
            if (Session["errorTb"] != null)
            {
                Session["errorTb"] = null;
            }
            if (Session["tips"] != null)
            {
                Session["tips"] = null;
            }
            if (Session["ListColumnsError"] != null)
            {
                Session["ListColumnsError"] = null;
            }
            if (Session["emptyFrame"] != null)
            {
                Session["emptyFrame"] = null;
            }
            if (Session["POPWrongFrame"] != null)
            {
                Session["POPWrongFrame"] = null;
            }
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    string MaterialSupportStr = string.Empty;
                    try
                    {
                        MaterialSupportStr = ConfigurationManager.AppSettings["MaterialSupport"];
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrWhiteSpace(MaterialSupportStr))
                    {
                        materialSupportList = StringHelper.ToStringList(MaterialSupportStr, '|', LowerUpperEnum.ToUpper);
                    }
                    string changePOPCountSheetStr = string.Empty;
                    try
                    {
                        changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrWhiteSpace(changePOPCountSheetStr))
                    {
                        ChangePOPCountSheetList = StringHelper.ToStringList(changePOPCountSheetStr, '|');
                    }

                    OleDbConnection conn;
                    OleDbDataAdapter da;
                    DataSet ds = null;
                    path = Server.MapPath(path);
                    string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                    ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                    conn = new OleDbConnection(ExcelConnStr);
                    conn.Open();
                    int subjectType = 0;
                    if (!string.IsNullOrWhiteSpace(hfSubjectType.Value))
                    {
                        subjectType = int.Parse(hfSubjectType.Value);
                    }
                    switch (subjectType)
                    {
                        case (int)SubjectTypeEnum.POP订单:
                        case (int)SubjectTypeEnum.正常单:
                            //导入pop订单
                            DataTable dt = conn.GetSchema("Tables");
                            StringBuilder sheetMsg = new StringBuilder();
                            List<string> sheetList = new List<string>();
                            string[] sheets = new string[] { "list$", "pop$", "补充订单$" };
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string sheetName = dt.Rows[i]["TABLE_NAME"].ToString();
                                if (sheetName.IndexOf("FilterDatabase") == -1 && sheets.Contains(sheetName.ToLower()))
                                {
                                    if (!sheetList.Contains(sheetName.ToLower()))
                                    {
                                        sheetList.Add(sheetName.ToLower());
                                    }
                                    else
                                    {
                                        sheetMsg.Append("导入文件不能含有相同的Sheet!<br/>");
                                        break;
                                    }
                                }
                            }
                            if (sheetList.Count < 3)
                            {
                                sheetMsg.Append("导入文件必须含有list、pop、补充订单 三个sheet表!<br/>");
                            }
                            if (sheetMsg.Length > 0)
                            {
                                Panel1.Visible = true;
                                labState.Text = "导入失败：" + sheetMsg.ToString();
                                return;

                            }
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string sheetName = dt.Rows[i]["TABLE_NAME"].ToString();
                                if (sheetName.IndexOf("FilterDatabase") == -1)
                                {

                                    string sql = "select * from [" + sheetName + "]";
                                    da = new OleDbDataAdapter(sql, conn);
                                    ds = new DataSet();
                                    da.Fill(ds);
                                    da.Dispose();
                                    //string sheetName1 = string.Empty;
                                    if (sheetName.ToLower() == "list$")
                                    {
                                        ImportTables tab = new ImportTables();
                                        tab.TabName = "list";
                                        tab.Data = ds;
                                        importTableList.Add(tab);

                                    }
                                    else if (sheetName.ToLower() == "pop$")
                                    {
                                        ImportTables tab = new ImportTables();
                                        tab.TabName = "pop";
                                        tab.Data = ds;
                                        importTableList.Add(tab);

                                    }
                                    else if (sheetName.ToLower() == "补充订单$")
                                    {
                                        ImportTables tab = new ImportTables();
                                        tab.TabName = "补充订单";
                                        tab.Data = ds;
                                        importTableList.Add(tab);

                                    }
                                    else if (sheetName.ToLower() == "material$")
                                    {
                                        ImportTables tab = new ImportTables();
                                        tab.TabName = "material";
                                        tab.Data = ds;
                                        importTableList.Add(tab);

                                    }
                                    else if (sheetName.ToLower() == "费用订单$")
                                    {
                                        ImportTables tab = new ImportTables();
                                        tab.TabName = "费用订单";
                                        tab.Data = ds;
                                        importTableList.Add(tab);

                                    }
                                }
                            }
                            if (importTableList.Any())
                            {
                                if (!cbAdd.Checked)
                                {
                                    listBll.Delete(s => s.SubjectId == subjectId);
                                    popOrderBll.Delete(s => s.SubjectId == subjectId);
                                    supplementBll.Delete(s => s.SubjectId == subjectId);
                                    materialBll.Delete(s => s.SubjectId == subjectId);
                                    priceOrderBll.Delete(s => s.SubjectId == subjectId);
                                }
                                int popCount = 0;
                                importTableList.ForEach(s =>
                                {

                                    if (s.TabName == "list" && s.Data.Tables[0].Rows.Count > 1)
                                    {
                                        popCount++;
                                        SaveListOrder(s.Data);
                                    }
                                    if (s.TabName == "pop" && s.Data.Tables[0].Rows.Count > 0)
                                    {
                                        popCount++;
                                        SavePOPOrder(s.Data);
                                    }
                                    if (s.TabName == "补充订单" && s.Data.Tables[0].Rows.Count > 0)
                                    {
                                        popCount++;
                                        SaveSupplementOrder(s.Data);
                                    }
                                    if (s.TabName == "material" && s.Data.Tables[0].Rows.Count > 0)
                                    {
                                        popCount++;
                                        SaveMaterialNew(s.Data);
                                    }
                                    if (s.TabName == "费用订单" && s.Data.Tables[0].Rows.Count > 0)
                                    {
                                        popCount++;
                                        SavePriceOder(s.Data);
                                    }

                                });
                                if (popCount == 0)
                                {
                                    Panel1.Visible = true;
                                    labState.Text = "导入失败：表格里面没有数据";
                                    return;
                                }

                            }

                            conn.Dispose();
                            conn.Close();
                            MergeOrder();
                            CheckImport();

                            break;
                        case (int)SubjectTypeEnum.新开店安装费:
                        case (int)SubjectTypeEnum.运费:
                            //导入费用订单
                            string sql1 = "select * from [Sheet1$]";
                            da = new OleDbDataAdapter(sql1, conn);
                            ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            ImportPriceOrder(ds);
                            break;
                        case (int)SubjectTypeEnum.手工订单:
                            //手工单
                            DataTable dt2 = conn.GetSchema("Tables");
                            StringBuilder sheetMsg2 = new StringBuilder();
                            List<string> sheetList2 = new List<string>();
                            //string[] sheets = new string[] { "pop$", "hc$"};
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                string sheetName = dt2.Rows[i]["TABLE_NAME"].ToString();
                                if (sheetName.IndexOf("FilterDatabase") == -1)
                                {
                                    if (!sheetList2.Contains(sheetName.ToLower()))
                                    {
                                        sheetList2.Add(sheetName.ToLower());
                                    }
                                    else
                                    {
                                        sheetMsg2.Append("导入文件不能含有相同的Sheet!<br/>");
                                        break;
                                    }
                                }
                            }
                            if (!sheetList2.Contains("pop$"))
                            {
                                sheetMsg2.Append("导入文件必须含有名称为‘pop’的Sheet");
                            }
                            if (sheetMsg2.Length > 0)
                            {
                                Panel1.Visible = true;
                                labState.Text = "导入失败：" + sheetMsg2.ToString();
                                return;

                            }
                            else
                            {
                                if (!cbAdd.Checked)
                                {
                                    new HandMadeOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                                    new HCOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                                    new OrderMaterialBLL().Delete(s => s.SubjectId == subjectId);
                                    priceOrderBll.Delete(s => s.SubjectId == subjectId);
                                }
                            }
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                string sheetName = dt2.Rows[i]["TABLE_NAME"].ToString();
                                if (sheetName.IndexOf("FilterDatabase") == -1)
                                {

                                    string sql = "select * from [" + sheetName + "]";
                                    da = new OleDbDataAdapter(sql, conn);
                                    ds = new DataSet();
                                    da.Fill(ds);
                                    da.Dispose();
                                    if (sheetName.ToLower().Contains("pop"))
                                    {
                                        ImportTables tab = new ImportTables();
                                        tab.TabName = "pop";
                                        tab.Data = ds;
                                        importTableList.Add(tab);
                                        ImportHnadMakeOrder_POP(ds);
                                    }
                                    else if (sheetName.ToLower().Contains("material"))
                                    {
                                        ImportTables tab = new ImportTables();
                                        tab.TabName = "material";
                                        tab.Data = ds;
                                        importTableList.Add(tab);
                                        SaveMaterialNew(ds);
                                    }
                                    if (sheetName.ToLower().Contains("费用订单"))
                                    {
                                        ImportTables tab = new ImportTables();
                                        tab.TabName = "费用订单";
                                        tab.Data = ds;
                                        importTableList.Add(tab);
                                        SavePriceOder(ds);
                                    }
                                }
                            }
                            CheckImport();
                            break;
                        default:
                            break;
                    }
                    SubjectBLL subjectBll = new SubjectBLL();
                    Models.Subject model = subjectBll.GetModel(subjectId);
                    if (model != null)
                    {

                        if (successNum > 0)
                        {
                            if (hfSubjectType.Value == "2" || hfSubjectType.Value == "5")
                                model.Status = 3;
                            else
                                model.Status = 2;
                        }
                        subjectBll.Update(model);
                    }
                    Response.Redirect("ImportOrder.aspx?fromRegion=1&import=1&subjectId=" + subjectId, false);

                }
            }
        }

        /// <summary>
        /// 保存list订单
        /// </summary>
        /// <param name="ds"></param>
        /// 
        /// 
        ListOrderDetailBLL listBll = new ListOrderDetailBLL();

        /// <summary>
        /// 如果物料支持导入不正确，把Sheet名称保存在tipsList，并显示提示
        /// </summary>
        List<string> tipsList = new List<string>();
        void SaveListOrder(DataSet ds)
        {
            int shopId = 0;

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                //需要检查器架的位置
                List<string> sheetList = new List<string>() { "服装墙", "鞋墙", "陈列桌", "中岛", "凹槽" };

                ListOrderDetail listModel;
                DataColumnCollection cols = ds.Tables[0].Columns;
                //检查模板表头：
                bool ColumnsPass = true;
                string[] keys = ConfigurationManager.AppSettings.AllKeys;
                if (keys.Contains("ListSheetRequireColumns") && keys.Contains("ListSheetRequireColumnsCount"))
                {
                    string ColumnsName = ConfigurationManager.AppSettings["ListSheetRequireColumns"].ToString();
                    int ColumnsCount = StringHelper.IsInt(ConfigurationManager.AppSettings["ListSheetRequireColumnsCount"].ToString());
                    if (!string.IsNullOrWhiteSpace(ColumnsName) && ColumnsCount > 0)
                    {
                        ColumnsPass = false;
                        int okCount = 0;
                        List<string> ColumnList = StringHelper.ToStringList(ColumnsName, '|');
                        for (int i = 0; i < ColumnsCount; i++)
                        {
                            if (ColumnList.Contains(cols[i].ColumnName))
                            {
                                okCount++;
                                ColumnList.Remove(cols[i].ColumnName);
                            }
                        }
                        if (okCount == ColumnsCount)
                        {
                            ColumnsPass = true;
                        }
                    }
                }
                if (!ColumnsPass)
                {
                    Session["ListColumnsError"] = "导入失败：List订单表模板不正确，请下载最新模板！";
                    Response.Redirect("ImportOrder.aspx?import=1&subjectId=" + subjectId, false);
                    return;
                }
                ListErrorTB = CommonMethod.CreateErrorTB(cols);
                string errorTBColName = "器架状态";
                ListEmptyFrameTB = CommonMethod.CreateErrorTB(cols, errorTBColName);

                DataTable dt1 = ds.Tables[0];
                DataRow dr0 = ListErrorTB.NewRow();
                DataRow dr01 = ListEmptyFrameTB.NewRow();
                for (int c = 0; c < cols.Count; c++)
                {
                    dr0["" + cols[c] + ""] = dt1.Rows[0][c].ToString();
                    dr01["" + cols[c] + ""] = dt1.Rows[0][c].ToString();
                }
                ListErrorTB.Rows.Add(dr0);
                ListEmptyFrameTB.Rows.Add(dr01);
                //bool isMaterialSupport = false;
                for (int i = 1; i < dt1.Rows.Count; i++)
                {
                    //rowIndex=i+1;
                    bool canSave = true;
                    StringBuilder msg = new StringBuilder();
                    StringBuilder emptyFrameMsg = new StringBuilder();
                    string shopNo = string.Empty;
                    if (cols.Contains("POSCode"))
                    {
                        //shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["POSCode"].ToString());
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POSCode"].ToString().Trim());
                    }
                    else if (cols.Contains("POS Code"))
                    {
                        //shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["POS Code"].ToString());
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POS Code"].ToString().Trim());
                    }
                    else if (cols.Contains("店铺编号"))
                    {
                        //shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["店铺编号"].ToString());
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["店铺编号"].ToString().Trim());
                    }
                    string materialSupport = string.Empty;
                    if (cols.Contains("物料支持"))
                    {
                        materialSupport = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["物料支持"].ToString().Trim());
                        materialSupport = StringHelper.ReplaceSpace(materialSupport);
                    }
                    else if (cols.Contains("物料支持级别"))
                    {
                        materialSupport = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["物料支持级别"].ToString().Trim());
                        materialSupport = StringHelper.ReplaceSpace(materialSupport);
                    }
                    string posScale = string.Empty;
                    if (cols.Contains("店铺规模大小"))
                    {
                        posScale = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["店铺规模大小"].ToString().Trim());
                    }
                    else if (cols.Contains("店铺规模"))
                    {
                        posScale = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["店铺规模"].ToString().Trim());
                    }
                    string cornerType = string.Empty;
                    if (cols.Contains("角落类型"))
                    {
                        cornerType = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["角落类型"].ToString().Trim());
                    }
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        msg.Append("店铺编号 为空；");
                    }
                    bool IsHc = false;
                    bool IsShut = false;
                    Shop shopFromDB = null;
                    if (!string.IsNullOrWhiteSpace(shopNo) && GetShopFromDB(shopNo, ref shopFromDB))
                    {
                        if (shopFromDB != null)
                        {
                            if (!string.IsNullOrWhiteSpace(shopFromDB.Status) && shopFromDB.Status.Contains("闭"))
                            {
                                //canSave = false;
                                //msg.Append("关闭店铺；");
                                IsShut = true;
                            }
                            else
                            {

                            }
                            shopId = shopFromDB.Id;
                            if (guidanceType != (int)GuidanceTypeEnum.Promotion)//不是促销
                            {
                                if (shopFromDB.Format == null || shopFromDB.Format == "")
                                {
                                    canSave = false;
                                    msg.Append("店铺类型为空；");
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(shopFromDB.Format))
                                    {
                                        //非促销的活动要检查是不是HC店铺，如果是，就不导入Homecourt
                                        string format = shopFromDB.Format.ToUpper();
                                        if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE"))
                                        {
                                            IsHc = true;
                                            canSave = false;
                                            msg.Append("HC店铺；");
                                        }
                                    }
                                    if (!IsHc)
                                    {

                                        if (shopFromDB.IsInstall == null || shopFromDB.IsInstall == "")
                                        {
                                            canSave = false;
                                            msg.Append("安装级别为空；");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            canSave = false;
                            msg.Append("店铺编号不存在；");
                        }
                    }
                    else
                    {
                        canSave = false;
                        msg.Append("店铺编号不存在；");
                    }


                    string materialSupport1 = materialSupport;
                    if (canSave && !IsHc && shopFromDB != null && shopFromDB.RegionName != null && shopFromDB.RegionName.ToLower() == "north")
                    {
                        if (guidanceType == (int)GuidanceTypeEnum.Install)//安装活动
                        {
                            //if (string.IsNullOrWhiteSpace(materialSupport))
                            //{
                            //    canSave = false;
                            //    msg.Append("物料支持 为空；");
                            //}
                            //else if (!materialSupportList.Contains(materialSupport.ToUpper()))
                            //{
                            //    canSave = false;
                            //    msg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                            //}
                            materialSupport1 = GetShopMaterialSupport(guidanceId, shopId);
                            if (string.IsNullOrWhiteSpace(materialSupport1))
                            {
                                materialSupport1 = materialSupport;
                                if (string.IsNullOrWhiteSpace(materialSupport1))
                                {
                                    canSave = false;
                                    msg.Append("物料支持 为空；");
                                }
                                else if (!materialSupportList.Contains(materialSupport1.ToUpper()))
                                {
                                    canSave = false;
                                    msg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                                }
                            }
                        }
                    }
                    if (canSave)
                    {
                        string sheet = string.Empty;
                        string gender = string.Empty;
                        string count = string.Empty;
                        string chooseImg = string.Empty;
                        string level = string.Empty;
                        for (int col = 8; col < cols.Count; col++)
                        {
                            sheet = string.Empty;
                            gender = string.Empty;
                            count = string.Empty;
                            chooseImg = string.Empty;
                            level = string.Empty;
                            if (cols[col] != null)
                            {
                                bool canGo = true;
                                gender = cols[col].ToString().Trim();
                                sheet = dt1.Rows[0][col].ToString().Trim();
                                count = dt1.Rows[i][col].ToString().Trim();
                                count = (count.IndexOf("无") != -1 || count.IndexOf("空") != -1) ? "0" : count;
                                count = string.IsNullOrWhiteSpace(count) ? "0" : count;
                                if (col < cols.Count - 1)
                                {
                                    int indexNum = 2;

                                    for (int k = 0; k < indexNum; k++)
                                    {
                                        if (col < cols.Count - 1 && dt1.Rows[0][col + 1].ToString().Contains("选图"))
                                        {
                                            chooseImg = dt1.Rows[i][col + 1].ToString().Trim();
                                            col += 1;

                                        }
                                        if (col < cols.Count - 1 && dt1.Rows[0][col + 1].ToString().Contains("级别"))
                                        {
                                            level = dt1.Rows[i][col + 1].ToString().Trim();
                                            col += 1;
                                        }
                                    }


                                }
                                if (gender.Contains("男") || string.IsNullOrWhiteSpace(gender))
                                {
                                    gender = "男";
                                }
                                else
                                    gender = "女";

                                if (!StringHelper.IsIntVal(count))
                                {

                                    msg.AppendFormat("{0}{1}的数量填写不正确；", gender, sheet);
                                    canGo = false;
                                }

                                if (canGo && count != "0" && !string.IsNullOrWhiteSpace(level))
                                {


                                    if (!StringHelper.IsIntVal(level))
                                    {

                                        msg.AppendFormat("{0}{1}的级别填写不正确；", gender, sheet);
                                        canGo = false;
                                    }

                                }


                                if (canGo && !string.IsNullOrWhiteSpace(gender) && !string.IsNullOrWhiteSpace(sheet) && count != "0")
                                {
                                    sheet = sheet == "户外" ? "OOH" : sheet;
                                    if (sheet.ToLower() == "t-stand" || sheet.ToLower() == "tstand")
                                    {
                                        sheet = "中岛";
                                    }
                                    listModel = new ListOrderDetail();
                                    listModel.AddDate = DateTime.Now;
                                    listModel.AddUserId = CurrentUser.UserId;
                                    listModel.Gender = gender;
                                    listModel.OrderGender = gender;
                                    listModel.Quantity = int.Parse(count != "" ? count : "0");
                                    listModel.Sheet = sheet;
                                    listModel.ShopId = shopId;
                                    listModel.SubjectId = subjectId;
                                    listModel.MaterialSupport = materialSupport1;
                                    listModel.POSScale = posScale;
                                    listModel.ChooseImg = chooseImg;
                                    listModel.CornerType = cornerType;
                                    listModel.Channel = shopFromDB.Channel;
                                    listModel.CityName = shopFromDB.CityName;
                                    listModel.CityTier = shopFromDB.CityTier;
                                    listModel.Format = shopFromDB.Format;
                                    listModel.IsInstall = shopFromDB.IsInstall;
                                    listModel.ProvinceName = shopFromDB.ProvinceName;
                                    listModel.RegionName = shopFromDB.RegionName;
                                    listModel.ShopName = shopFromDB.ShopName;
                                    listModel.ShopNo = shopFromDB.ShopNo;



                                    if (!string.IsNullOrWhiteSpace(level))
                                        listModel.LevelNum = int.Parse(level);
                                    listBll.Add(listModel);
                                    successNum++;

                                    //非关闭店铺，检查器架是不是为空
                                    if (!IsHc && sheetList.Contains(sheet))
                                    {
                                        //
                                        string errMsg = string.Empty;
                                        if (!IsExistMachineFrame(shopId, sheet, gender, out errMsg))
                                        {
                                            if (!string.IsNullOrWhiteSpace(errMsg))
                                            {
                                                if (IsShut)
                                                    emptyFrameMsg.Append("闭店：");
                                                emptyFrameMsg.Append(errMsg);
                                            }
                                            else
                                            {
                                                if (IsShut)
                                                    emptyFrameMsg.Append("闭店：");
                                                emptyFrameMsg.AppendFormat("{0}{1}器架为空,请补充；", sheet, gender);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (msg.Length > 0)
                    {
                        DataRow dr1 = ListErrorTB.NewRow();
                        for (int ii = 0; ii < cols.Count; ii++)
                        {
                            dr1["" + cols[ii] + ""] = dt1.Rows[i][ii].ToString();
                        }
                        dr1["错误信息"] = msg.ToString();
                        ListErrorTB.Rows.Add(dr1);
                    }
                    if (emptyFrameMsg.Length > 0)
                    {
                        DataRow dr11 = ListEmptyFrameTB.NewRow();
                        for (int ii = 0; ii < cols.Count; ii++)
                        {
                            dr11["" + cols[ii] + ""] = dt1.Rows[i][ii].ToString();
                        }
                        dr11[errorTBColName] = emptyFrameMsg.ToString();
                        ListEmptyFrameTB.Rows.Add(dr11);
                    }
                }

            }

        }

        /// <summary>
        /// 保存POP订单
        /// </summary>
        /// <param name="ds"></param>
        POPOrderDetailBLL popOrderBll = new POPOrderDetailBLL();
        void SavePOPOrder(DataSet ds)
        {


            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                //需要检查器架的位置
                List<string> sheetList = new List<string>() { "服装墙", "鞋墙", "陈列桌", "中岛", "凹槽" };
                POPOrderDetail popOrderModel;
                DataColumnCollection cols = ds.Tables[0].Columns;
                POPErrorTB = CommonMethod.CreateErrorTB(cols);

                string errorTBColName = "器架状态";
                POPWrongFrameTB = CommonMethod.CreateErrorTB(cols, errorTBColName);
                POPEmptyFrameTB = CommonMethod.CreateErrorTB(cols, errorTBColName);
                string PlaceWarning = "位置占用";
                POPPlaceWarningTB = CommonMethod.CreateErrorTB(cols, PlaceWarning);
                List<string> CheckPlaceSheetList = new List<string>();
                string CheckPlaceIsFreeSheetStr = string.Empty;
                try
                {
                    CheckPlaceIsFreeSheetStr = ConfigurationManager.AppSettings["CheckPlace"];
                }
                catch
                {

                }
                if (!string.IsNullOrWhiteSpace(CheckPlaceIsFreeSheetStr))
                {
                    CheckPlaceSheetList = StringHelper.ToStringList(CheckPlaceIsFreeSheetStr, '|', LowerUpperEnum.ToUpper);
                }

                int shopId = 0;
                //从基础数据取出的
                string realGender = string.Empty;
                //bool isMaterialSupport = false;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    shopId = 0;
                    realGender = string.Empty;
                    string shopNo = string.Empty;
                    if (cols.Contains("POSCode"))
                    {
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POSCode"].ToString().Trim());
                    }
                    else if (cols.Contains("POS Code"))
                    {
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POS Code"].ToString().Trim());
                    }
                    else if (cols.Contains("店铺编号"))
                    {
                        shopNo = StringHelper.ReplaceSpecialChar(dr["店铺编号"].ToString().Trim());
                    }
                    string graphicNo = string.Empty;
                    if (cols.Contains("pop编号"))
                    {
                        graphicNo = StringHelper.ReplaceSpecialChar(dr["pop编号"].ToString().Trim());
                    }
                    else if (cols.Contains("POP编号"))
                    {
                        graphicNo = StringHelper.ReplaceSpecialChar(dr["POP编号"].ToString().Trim());
                    }
                    else if (cols.Contains("Graphic No"))
                    {
                        graphicNo = StringHelper.ReplaceSpecialChar(dr["Graphic No"].ToString().Trim());
                    }
                    else if (cols.Contains("GraphicNo"))
                    {
                        graphicNo = StringHelper.ReplaceSpecialChar(dr["GraphicNo"].ToString().Trim());
                    }
                    else if (cols.Contains("Graphic No#"))
                    {
                        graphicNo = StringHelper.ReplaceSpecialChar(dr["Graphic No#"].ToString().Trim());
                    }
                    else if (cols.Contains("GraphicNo#"))
                    {
                        graphicNo = StringHelper.ReplaceSpecialChar(dr["GraphicNo#"].ToString().Trim());
                    }
                    string count = string.Empty;
                    if (cols.Contains("数量"))
                        count = StringHelper.ReplaceSpecialChar(dr["数量"].ToString().Trim());
                    else if (cols.Contains("Quantity"))
                        count = StringHelper.ReplaceSpecialChar(dr["Quantity"].ToString().Trim());
                    string sheet = string.Empty;
                    if (cols.Contains("Sheet"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["Sheet"].ToString().Trim());
                    else if (cols.Contains("位置"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["位置"].ToString().Trim());
                    else if (cols.Contains("pop位置"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["pop位置"].ToString().Trim());
                    //else if (cols.Contains("POP位置"))
                    //    sheet = dr["POP位置"].ToString().Trim();
                    string gender = string.Empty;
                    if (cols.Contains("性别"))
                        gender = StringHelper.ReplaceSpecialChar(dr["性别"].ToString().Trim());
                    else if (cols.Contains("M/W"))
                        gender = StringHelper.ReplaceSpecialChar(dr["M/W"].ToString().Trim());


                    string MaterialSupport = string.Empty;
                    if (cols.Contains("物料支持"))
                        MaterialSupport = StringHelper.ReplaceSpecialChar(dr["物料支持"].ToString().Trim());
                    else if (cols.Contains("物料支持级别"))
                        MaterialSupport = StringHelper.ReplaceSpecialChar(dr["物料支持级别"].ToString().Trim());
                    string POSScale = string.Empty;
                    if (cols.Contains("店铺规模大小"))
                        POSScale = StringHelper.ReplaceSpecialChar(dr["店铺规模大小"].ToString().Trim());
                    else if (cols.Contains("店铺规模"))
                        POSScale = StringHelper.ReplaceSpecialChar(dr["店铺规模"].ToString().Trim());
                    string ChooseImg = string.Empty;
                    if (cols.Contains("选图"))
                        ChooseImg = StringHelper.ReplaceSpecialChar(dr["选图"].ToString().Trim());
                    

                    int positionId = 0;
                    bool canSave = true;
                    StringBuilder msg = new StringBuilder();
                    StringBuilder emptyFrameMsg = new StringBuilder();
                    StringBuilder wrongFrameMsg = new StringBuilder();
                    StringBuilder placeWarningMsg = new StringBuilder();
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        msg.Append("店铺编号 为空；");
                    }
                    if (string.IsNullOrWhiteSpace(graphicNo))
                    {
                        canSave = false;
                        msg.Append("pop编号 为空；");
                    }
                    if (string.IsNullOrWhiteSpace(sheet))
                    {
                        canSave = false;
                        msg.Append("位置 为空；");
                    }
                    else
                    {
                        sheet = sheet == "户外" ? "OOH" : sheet;
                        if (sheet.ToLower() == "t-stand" || sheet.ToLower() == "tstand")
                        {
                            sheet = "中岛";
                        }
                        sheet = sheet.ToUpper();
                    }
                    if (string.IsNullOrWhiteSpace(gender))
                    {
                        //gender = "男女不限";
                        canSave = false;
                        msg.Append("性别 为空；");
                    }
                    //count = (count.IndexOf("无") != -1 || count.IndexOf("空") != -1) ? "1" : count;
                    if (string.IsNullOrWhiteSpace(count) || !StringHelper.IsIntVal(count))
                    {
                       
                        msg.Append("数量填写不正确；系统自动改成1");
                        count = "1";
                    }
                    if (StringHelper.IsInt(count)<1)
                    {
                        msg.Append("数量小于1；系统自动改成1");
                        count = "1";
                    }
                    bool IsHc = false;
                    bool IsShut = false;
                    Shop shopFromDB = null;
                    if (!string.IsNullOrWhiteSpace(shopNo) && GetShopFromDB(shopNo, ref shopFromDB))
                    {
                        if (shopFromDB != null)
                        {
                            if (!string.IsNullOrWhiteSpace(shopFromDB.Status) && shopFromDB.Status.Contains("闭"))
                            {
                               
                                IsShut = true;
                            }
                            else
                            {

                            }
                            shopId = shopFromDB.Id;

                            if (guidanceType != (int)GuidanceTypeEnum.Promotion)//不是促销
                            {
                                if (shopFromDB.Format == null || shopFromDB.Format == "")
                                {
                                    canSave = false;
                                    msg.Append("店铺类型为空；");
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(shopFromDB.Format))
                                    {
                                        //非促销的活动要检查是不是HC店铺，如果是，就不导入Homecourt
                                        string format = shopFromDB.Format.ToUpper();

                                        if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE"))
                                        {
                                            IsHc = true;
                                            canSave = false;
                                            msg.Append("HC店铺；");
                                        }
                                    }
                                    if (!IsHc)
                                    {

                                        if (shopFromDB.IsInstall == null || shopFromDB.IsInstall == "")
                                        {
                                            canSave = false;
                                            msg.Append("安装级别为空；");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            canSave = false;
                            msg.Append("店铺编号不存在；");
                        }
                    }
                    else
                    {
                        canSave = false;
                        msg.Append("店铺编号不存在；");
                    }
                    POP popFromDB = null;
                    string MaterialSupport1 = MaterialSupport;
                    if (!IsHc && canSave)//非HC
                    {
                        string errorMsg = string.Empty;

                        if (!string.IsNullOrWhiteSpace(shopNo) && !string.IsNullOrWhiteSpace(sheet) && !string.IsNullOrWhiteSpace(graphicNo) && !GetPOPFromDB(shopId, sheet, graphicNo, ref popFromDB, out errorMsg))
                        {
                            canSave = false;
                            if (IsShut)
                            {
                                errorMsg = "闭店：" + errorMsg;
                            }
                            msg.Append(errorMsg);
                        }
                        if (popFromDB != null)
                            realGender = popFromDB.Gender;

                        if (shopFromDB != null && shopFromDB.RegionName != null && shopFromDB.RegionName.ToLower() == "north" && guidanceType == (int)GuidanceTypeEnum.Install)//安装活动
                        {
                            MaterialSupport1 = GetShopMaterialSupport(guidanceId, shopId);
                            if (string.IsNullOrWhiteSpace(MaterialSupport1))
                            {
                                MaterialSupport1 = MaterialSupport;
                                if (string.IsNullOrWhiteSpace(MaterialSupport1))
                                {
                                    canSave = false;
                                    msg.Append("物料支持 为空；");
                                }
                                else if (!materialSupportList.Contains(MaterialSupport1.ToUpper()))
                                {
                                    canSave = false;
                                    msg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                                }
                            }
                        }
                    }

                    if (canSave)
                    {
                        if (realGender.Contains("男") && realGender.Contains("女"))
                            realGender = gender;
                        //要在插入数据库之前检查
                        if (!IsHc && popFromDB != null)
                        {
                            if (!string.IsNullOrWhiteSpace(popFromDB.MachineFrameName))
                            {
                                var frameModel = new ShopMachineFrameBLL().GetList(s => s.ShopId == shopId && s.PositionName == sheet && s.MachineFrame.ToUpper() == popFromDB.MachineFrameName.ToUpper() && (s.Gender == realGender || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (realGender.Contains("男") && realGender.Contains("女"))));
                                if (!string.IsNullOrWhiteSpace(popFromDB.CornerType))
                                {
                                    frameModel = frameModel.Where(s => s.CornerType == popFromDB.CornerType).ToList();
                                }
                                else
                                {
                                    frameModel = frameModel.Where(s => s.CornerType == null || s.CornerType == "").ToList();
                                }
                                if (!frameModel.Any())
                                {
                                    if (IsShut)
                                        wrongFrameMsg.Append("闭店：");
                                    wrongFrameMsg.Append("器架对应关系有错，请查实；");
                                }
                            }
                            string subjectNames = string.Empty;
                            //CheckPlaceSheetList.Contains(sheet) && 
                            if (shopFromDB != null && shopFromDB.RegionName != null && shopFromDB.RegionName.ToLower() == "north" && !POPPlaceIsFree(shopId, sheet, graphicNo.ToUpper(), out subjectNames))
                            {
                                placeWarningMsg.AppendFormat("该位置已被其他项目（{0}）占用", subjectNames);
                            }
                            //检查器架是不是为空
                            if (sheetList.Contains(sheet))
                            {
                                //
                                string errMsg = string.Empty;
                                if (!IsExistMachineFrame(shopId, sheet, realGender, out errMsg))
                                {
                                    if (!string.IsNullOrWhiteSpace(errMsg))
                                    {
                                        if (IsShut)
                                            emptyFrameMsg.Append("闭店：");
                                        emptyFrameMsg.Append(errMsg);
                                    }
                                    else
                                    {
                                        if (IsShut)
                                            emptyFrameMsg.Append("闭店：");
                                        emptyFrameMsg.AppendFormat("{0}{1}器架为空,请补充；", sheet, realGender);
                                    }
                                }
                            }
                        }
                        ChcekSheet(sheet, out positionId);
                        popOrderModel = new POPOrderDetail();
                        popOrderModel.AddDate = DateTime.Now;
                        popOrderModel.AddUserId = CurrentUser.UserId;
                        popOrderModel.Channel = shopFromDB.Channel;
                        popOrderModel.CityName = shopFromDB.CityName;
                        popOrderModel.CityTier = shopFromDB.CityTier;
                        popOrderModel.Format = shopFromDB.Format;
                        popOrderModel.IsInstall = shopFromDB.IsInstall;
                        popOrderModel.ProvinceName = shopFromDB.ProvinceName;
                        popOrderModel.RegionName = shopFromDB.RegionName;
                        popOrderModel.ShopName = shopFromDB.ShopName;
                        popOrderModel.ShopNo = shopFromDB.ShopNo;
                        popOrderModel.GraphicNo = graphicNo.ToUpper();
                        popOrderModel.Sheet = sheet;
                        popOrderModel.PositionId = positionId;

                        popOrderModel.Gender = realGender;
                        popOrderModel.OrderGender = gender;
                        popOrderModel.ShopId = shopId;
                        popOrderModel.SubjectId = subjectId;
                        popOrderModel.MaterialSupport = MaterialSupport1;
                        popOrderModel.POSScale = POSScale;
                        popOrderModel.ChooseImg = ChooseImg;

                        popOrderModel.Quantity = int.Parse(count != "" ? count : "1");
                        popOrderModel.GraphicLength = popFromDB.GraphicLength;
                        popOrderModel.GraphicMaterial = popFromDB.GraphicMaterial;
                        popOrderModel.GraphicWidth = popFromDB.GraphicWidth;
                        popOrderModel.PositionDescription = popFromDB.PositionDescription;
                        popOrderModel.Remark = popFromDB.Remark;
                        popOrderBll.Add(popOrderModel);
                        successNum++;


                    }
                    else
                    {
                        DataRow dr1 = POPErrorTB.NewRow();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            dr1["" + cols[i] + ""] = dr[cols[i]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        POPErrorTB.Rows.Add(dr1);
                    }
                    if (wrongFrameMsg.Length > 0)
                    {
                        DataRow dr11 = POPWrongFrameTB.NewRow();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            dr11["" + cols[i] + ""] = dr[cols[i]];
                        }
                        dr11[errorTBColName] = wrongFrameMsg.ToString();
                        POPWrongFrameTB.Rows.Add(dr11);
                    }
                    if (emptyFrameMsg.Length > 0)
                    {
                        DataRow dr11 = POPEmptyFrameTB.NewRow();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            dr11["" + cols[i] + ""] = dr[cols[i]];
                        }
                        dr11[errorTBColName] = emptyFrameMsg.ToString();
                        POPEmptyFrameTB.Rows.Add(dr11);
                    }
                    if (placeWarningMsg.Length > 0)
                    {
                        DataRow placeWarnTB = POPPlaceWarningTB.NewRow();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            placeWarnTB["" + cols[i] + ""] = dr[cols[i]];
                        }
                        placeWarnTB[PlaceWarning] = placeWarningMsg.ToString();
                        POPPlaceWarningTB.Rows.Add(placeWarnTB);
                    }
                }

            }
        }

        /// <summary>
        /// 保存补充订单
        /// </summary>
        /// <param name="ds"></param>
        SupplementOrderDetailBLL supplementBll = new SupplementOrderDetailBLL();
        void SaveSupplementOrder(DataSet ds)
        {
            int shopId = 0;

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                SupplementOrderDetail supplementModel;
                DataColumnCollection cols = ds.Tables[0].Columns;
                SupplementErrorTB = CommonMethod.CreateErrorTB(cols);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string shopNo = string.Empty;
                    if (cols.Contains("店铺编号"))
                    {
                        shopNo = dr["店铺编号"].ToString().Trim();
                    }
                    else if (cols.Contains("POS Code"))
                    {
                        shopNo = dr["POS Code"].ToString().Trim();
                    }
                    else if (cols.Contains("POSCode"))
                    {
                        shopNo = dr["POSCode"].ToString().Trim();
                    }
                    string materialSupport = string.Empty;
                    if (cols.Contains("物料支持"))
                    {
                        materialSupport = StringHelper.ReplaceSpace(dr["物料支持"].ToString());
                    }
                    else if (cols.Contains("物料支持级别"))
                    {
                        materialSupport = StringHelper.ReplaceSpace(dr["物料支持级别"].ToString());
                    }
                    string posScale = string.Empty;
                    if (cols.Contains("店铺规模大小"))
                    {
                        posScale = dr["店铺规模大小"].ToString().Trim();
                    }
                    else if (cols.Contains("店铺规模"))
                    {
                        posScale = dr["店铺规模"].ToString().Trim();
                    }
                    string sheet = string.Empty;
                    if (cols.Contains("位置"))
                    {
                        sheet = dr["位置"].ToString().Trim();
                    }
                    else if (cols.Contains("pop位置"))
                        sheet = dr["pop位置"].ToString().Trim();
                    else if (cols.Contains("Sheet"))
                        sheet = dr["Sheet"].ToString().Trim();
                    string gender = string.Empty;
                    if (cols.Contains("性别"))
                    {
                        gender = dr["性别"].ToString().Trim();
                    }
                    else if (cols.Contains("M/W"))
                        gender = dr["M/W"].ToString().Trim();
                    string count = string.Empty;
                    if (cols.Contains("数量"))
                    {
                        count = dr["数量"].ToString().Trim();
                    }
                    else if (cols.Contains("Quantity"))
                        count = dr["Quantity"].ToString().Trim();
                    string remark = string.Empty;
                    if (cols.Contains("备注"))
                    {
                        remark = dr["备注"].ToString().Trim();
                    }
                    string chooseImg = string.Empty;
                    if (cols.Contains("选图"))
                    {
                        chooseImg = dr["选图"].ToString().Trim();
                    }
                    int positionId = 0;
                    bool canSave = true;
                    StringBuilder msg = new StringBuilder();
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        msg.Append("店铺编号 为空；");
                    }
                    if (string.IsNullOrWhiteSpace(sheet))
                    {
                        canSave = false;
                        msg.Append("位置 为空；");
                    }
                    if (string.IsNullOrWhiteSpace(gender))
                    {
                        gender = "男女不限";
                    }
                    if (string.IsNullOrWhiteSpace(count) || !StringHelper.IsIntVal(count))
                    {
                        canSave = false;
                        msg.Append("数量填写不正确；");
                    }
                    int isInvalid = 0;
                    bool IsHc = false;
                    bool EmptyInstallLevel = false;
                    bool EmptyFormat = false;
                    if (!string.IsNullOrWhiteSpace(shopNo) && !CheckShop(shopNo, out shopId, out isInvalid, out IsHc, out EmptyInstallLevel, out EmptyFormat))
                    {
                        canSave = false;
                        //if (shopId > 0)
                        //    msg.Append("店铺已经关闭；");
                        //else
                        msg.Append("店铺编号不存在；");

                    }
                    if (IsHc)
                    {
                        canSave = false;
                        msg.Append("HC店铺；");
                    }
                    else
                    {
                        if (EmptyInstallLevel)
                        {
                            canSave = false;
                            msg.Append("安装级别为空；");
                        }
                        if (EmptyFormat)
                        {
                            canSave = false;
                            msg.Append("店铺类型为空；");
                        }
                    }
                    if (canSave)
                    {
                        sheet = sheet == "户外" ? "OOH" : sheet;
                        if (sheet.ToLower() == "t-stand" || sheet.ToLower() == "tstand")
                        {
                            sheet = "中岛";
                        }
                        ChcekSheet(sheet, out positionId);
                        supplementModel = new SupplementOrderDetail();
                        supplementModel.AddDate = DateTime.Now;
                        supplementModel.AddUserId = CurrentUser.UserId;
                        supplementModel.Gender = gender;
                        supplementModel.Quantity = int.Parse(count != "" ? count : "0");
                        supplementModel.Sheet = sheet;
                        supplementModel.PositionId = positionId;
                        supplementModel.ShopId = shopId;
                        supplementModel.SubjectId = subjectId;
                        supplementModel.ChooseImg = chooseImg;
                        supplementModel.Remark = remark;
                        supplementModel.MaterialSupport = materialSupport;
                        supplementModel.POSScale = posScale;
                        supplementBll.Add(supplementModel);
                        successNum++;
                    }
                    else
                    {
                        DataRow dr1 = SupplementErrorTB.NewRow();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            dr1["" + cols[i] + ""] = dr[cols[i]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        SupplementErrorTB.Rows.Add(dr1);
                    }
                }
            }
        }

        OrderMaterialBLL materialBll = new OrderMaterialBLL();

        /// <summary>
        /// 保存物料
        /// </summary>
        /// <param name="ds"></param>
        void SaveMaterialNew(DataSet ds)
        {

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataColumnCollection cols = ds.Tables[0].Columns;
                MaterialErrorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                OrderMaterial model;
                int shopId = 0;
                DataTable dt1 = ds.Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    bool canSave = true;
                    int materialId = 0;
                    StringBuilder msg = new StringBuilder();
                    string shopNo = string.Empty;
                    string sheet = string.Empty;
                    string materialName = string.Empty;
                    string num = string.Empty;
                    string length = string.Empty;
                    string width = string.Empty;
                    string high = string.Empty;
                    string price = string.Empty;
                    string remark = string.Empty;
                    if (cols.Contains("POSCode"))
                    {
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POSCode"].ToString().Trim());
                    }
                    else if (cols.Contains("POS Code"))
                    {
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POS Code"].ToString().Trim());
                    }
                    else if (cols.Contains("店铺编号"))
                    {
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["店铺编号"].ToString().Trim());
                    }
                    if (cols.Contains("Sheet"))
                    {
                        sheet = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["Sheet"].ToString().Trim());
                    }
                    else if (cols.Contains("POP位置"))
                    {
                        sheet = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POP位置"].ToString().Trim());
                    }
                    if (cols.Contains("物料名称"))
                    {
                        materialName = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["物料名称"].ToString().Trim());
                    }
                    else if (cols.Contains("物料"))
                    {
                        materialName = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["物料"].ToString().Trim());
                    }
                    if (cols.Contains("数量"))
                    {
                        num = dt1.Rows[i]["数量"].ToString().Trim();
                    }
                    if (cols.Contains("长"))
                    {
                        length = dt1.Rows[i]["长"].ToString().Trim();
                    }
                    if (cols.Contains("宽"))
                    {
                        width = dt1.Rows[i]["宽"].ToString().Trim();
                    }
                    if (cols.Contains("高"))
                    {
                        high = dt1.Rows[i]["高"].ToString().Trim();
                    }
                    if (cols.Contains("单价"))
                    {
                        price = dt1.Rows[i]["单价"].ToString().Trim();
                    }
                    else if (cols.Contains("价格"))
                    {
                        price = dt1.Rows[i]["价格"].ToString().Trim();
                    }
                    if (cols.Contains("备注"))
                    {
                        remark = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["备注"].ToString().Trim());
                    }
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        msg.Append("店铺编号 为空；");
                    }
                    int IsInvalid = 0;
                    bool IsHC = false;
                    bool EmptyInstallLevel = false;
                    bool EmptyFormat = false;
                    if (!string.IsNullOrWhiteSpace(shopNo) && !CheckShop(shopNo, out shopId, out IsInvalid, out IsHC, out EmptyInstallLevel, out EmptyFormat))
                    {
                        canSave = false;
                        msg.Append("店铺编号不存在；");
                    }
                    if (IsHC && hfSubjectType.Value != "5")
                    {
                        //非手工单不能导入HC
                        canSave = false;
                        msg.Append("HC店铺；");
                    }
                    if (string.IsNullOrWhiteSpace(materialName))
                    {
                        canSave = false;
                        msg.Append("物料名称 为空；");
                    }
                    //num = !string.IsNullOrWhiteSpace(num) ? num : "1";
                    if (string.IsNullOrWhiteSpace(num))
                    {
                        canSave = false;
                        msg.Append("数量 为空；");
                    }
                    else if (!StringHelper.IsIntVal(num))
                    {
                        canSave = false;
                        msg.Append("数量填写不正确；");
                    }
                    if (!string.IsNullOrWhiteSpace(length) && !StringHelper.IsDecimalVal(length))
                    {
                        canSave = false;
                        msg.Append("长度填写不正确；");
                    }
                    if (!string.IsNullOrWhiteSpace(width) && !StringHelper.IsDecimalVal(width))
                    {
                        canSave = false;
                        msg.Append("宽度填写不正确；");
                    }
                    if (!string.IsNullOrWhiteSpace(high) && !StringHelper.IsDecimalVal(high))
                    {
                        canSave = false;
                        msg.Append("高度填写不正确；");
                    }
                    if (!string.IsNullOrWhiteSpace(price) && !StringHelper.IsDecimalVal(price))
                    {
                        canSave = false;
                        msg.Append("单价填写不正确；");
                    }
                    if (canSave)
                    {
                        sheet = sheet == "户外" ? "OOH" : sheet;
                        if (int.Parse(num) > 0)
                        {
                            model = new OrderMaterial();
                            model.AddDate = DateTime.Now;
                            model.MaterialCount = int.Parse(num);
                            if (!string.IsNullOrWhiteSpace(length))
                                model.MaterialLength = decimal.Parse(length);
                            if (!string.IsNullOrWhiteSpace(width))
                                model.MaterialWidth = decimal.Parse(width);
                            if (!string.IsNullOrWhiteSpace(high))
                                model.MaterialHigh = decimal.Parse(high);
                            model.MaterialName = materialName;
                            if (!string.IsNullOrWhiteSpace(price))
                                model.Price = decimal.Parse(price);
                            model.Remark = remark;
                            model.Sheet = sheet;
                            model.ShopId = shopId;
                            model.SubjectId = subjectId;
                            materialBll.Add(model);

                            successNum++;
                        }
                    }
                    if (msg.Length > 0)
                    {
                        DataRow dr1 = MaterialErrorTB.NewRow();
                        for (int ii = 0; ii < cols.Count; ii++)
                        {
                            dr1["" + cols[ii] + ""] = dt1.Rows[i][ii].ToString();
                        }
                        dr1["错误信息"] = msg.ToString();
                        MaterialErrorTB.Rows.Add(dr1);
                    }
                }
            }
        }


        /// <summary>
        /// 保存费用订单
        /// </summary>
        /// <param name="ds"></param>
        PriceOrderDetailBLL priceOrderBll = new PriceOrderDetailBLL();
        void SavePriceOder(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataColumnCollection cols = ds.Tables[0].Columns;
                PriceOrderErrorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);

                DataTable dt1 = ds.Tables[0];
                
                PriceOrderDetail priceModel;
                List<string> orderTypeList = CommonMethod.GetEnumList<OrderTypeEnum>().Where(s => s.Desction.Contains("费用订单")).Select(s => s.Name).ToList();
                int shopId = 0;
                string orderType = string.Empty;
                //店铺编号
                string shopNo = string.Empty;
                //应收费用金额
                string price = string.Empty;
                //应付费用金额
                string payPrice = string.Empty;
                string contents = string.Empty;
                string remark = string.Empty;
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    StringBuilder msg = new StringBuilder();
                    int orderTypeIndex = 0;
                    bool canSave = true;
                    if (cols.Contains("费用类型"))
                        orderType = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["费用类型"].ToString().Trim());

                    if (cols.Contains("店铺编号"))
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["店铺编号"].ToString().Trim());
                    else if (cols.Contains("POSCode"))
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POSCode"].ToString().Trim());
                    else if (cols.Contains("POS Code"))
                        shopNo = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["POS Code"].ToString().Trim());

                    if (cols.Contains("费用金额"))
                        price = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["费用金额"].ToString().Trim());
                    else if (cols.Contains("费用"))
                        price = dt1.Rows[i]["费用"].ToString().Trim();
                    else if (cols.Contains("金额"))
                        price = dt1.Rows[i]["金额"].ToString().Trim();
                    else if (cols.Contains("应收金额"))
                        price = dt1.Rows[i]["应收金额"].ToString().Trim();
                    else if (cols.Contains("应收费用金额"))
                        price = dt1.Rows[i]["应收费用金额"].ToString().Trim();
                    else if (cols.Contains("应收费用"))
                        price = dt1.Rows[i]["应收费用"].ToString().Trim();

                    if (cols.Contains("应付费用金额"))
                        payPrice = dt1.Rows[i]["应付费用金额"].ToString().Trim();
                    else if (cols.Contains("应付费用"))
                        payPrice = dt1.Rows[i]["应付费用"].ToString().Trim();
                    else if (cols.Contains("应付金额"))
                        payPrice = dt1.Rows[i]["应付金额"].ToString().Trim();
                    if (cols.Contains("备注"))
                        remark = StringHelper.ReplaceSpecialChar(dt1.Rows[i]["备注"].ToString().Trim());
                    if (string.IsNullOrWhiteSpace(orderType))
                    {
                        canSave = false;
                        msg.Append("费用类型 为空；");
                    }
                    else if (!orderTypeList.Contains(orderType))
                    {
                        canSave = false;
                        msg.Append("费用类型 填写不正确；");
                    }
                    else
                    {
                        OrderTypeEnum otEnum = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), orderType, true);
                        orderTypeIndex = (int)otEnum;
                    }
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        msg.Append("店铺编号 为空；");
                    }
                    Shop shopFromDB = null;
                    if (!string.IsNullOrWhiteSpace(shopNo) && GetShopFromDB(shopNo, ref shopFromDB))
                    {
                        if (shopFromDB != null)
                        {
                            shopId = shopFromDB.Id;
                        }
                        else
                        {
                            canSave = false;
                            msg.Append("店铺编号不存在；");
                        }
                    }
                    else
                    {
                        canSave = false;
                        msg.Append("店铺编号不存在；");
                    }
                    if (string.IsNullOrWhiteSpace(price))
                    {
                        canSave = false;
                        msg.Append("应收金额 为空；");
                    }
                    else if (!StringHelper.IsDecimalVal(price))
                    {
                        canSave = false;
                        msg.Append("应收金额填写不正确；");
                    }

                    if (!string.IsNullOrWhiteSpace(payPrice) && !StringHelper.IsDecimalVal(payPrice))
                    {
                        canSave = false;
                        msg.Append("应付金额填写不正确；");
                    }
                    if (canSave)
                    {
                        if (string.IsNullOrWhiteSpace(payPrice))
                            payPrice = price;
                        priceModel = new PriceOrderDetail();
                        priceModel.AddDate = DateTime.Now;
                        priceModel.Address = shopFromDB.POPAddress;
                        priceModel.Amount = StringHelper.IsDecimal(price);
                        priceModel.PayAmount = StringHelper.IsDecimal(payPrice);
                        priceModel.City = shopFromDB.CityName;
                        priceModel.Contents = string.Empty;
                        priceModel.Province = shopFromDB.ProvinceName;
                        priceModel.Region = shopFromDB.RegionName;
                        priceModel.Remark = remark;
                        priceModel.ShopId = shopId;
                        priceModel.ShopName = shopFromDB.ShopName;
                        priceModel.SubjectId = subjectId;
                        priceModel.OrderType = orderTypeIndex;
                        priceModel.GuidanceId = int.Parse(hfGuidanceId.Value);
                        priceModel.ShopNo = shopFromDB.ShopNo;
                        priceOrderBll.Add(priceModel);
                        successNum++;
                    }
                    else
                    {

                        DataRow dr1 = PriceOrderErrorTB.NewRow();
                        for (int ii = 0; ii < cols.Count; ii++)
                        {
                            dr1["" + cols[ii] + ""] = dt1.Rows[i][ii].ToString();
                        }
                        dr1["错误信息"] = msg.ToString();
                        PriceOrderErrorTB.Rows.Add(dr1);

                    }
                }
            }
        }


        /// <summary>
        /// 如果导入有失败的数据，就导出失败的信息
        /// </summary>
        void CheckImport()
        {
            bool isFail = false;
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();

            if (ListErrorTB != null && ListErrorTB.Rows.Count > 1)
            {
                isFail = true;
                dic.Add("List", ListErrorTB);
            }
            if (POPErrorTB != null && POPErrorTB.Rows.Count > 0)
            {
                isFail = true;
                dic.Add("POP", POPErrorTB);
            }
            if (SupplementErrorTB != null && SupplementErrorTB.Rows.Count > 0)
            {
                isFail = true;
                dic.Add("补充订单", SupplementErrorTB);
            }
            if (DisplayTableErrorTB != null && DisplayTableErrorTB.Rows.Count > 0)
            {
                isFail = true;
                dic.Add("陈列桌数量不足", DisplayTableErrorTB);
            }
            if (MaterialErrorTB != null && MaterialErrorTB.Rows.Count > 0)
            {
                isFail = true;
                dic.Add("Material", MaterialErrorTB);
            }
            if (PriceOrderErrorTB != null && PriceOrderErrorTB.Rows.Count > 0)
            {
                isFail = true;
                dic.Add("费用订单", PriceOrderErrorTB);
            }
            if (HMPOPErrorTB.Rows.Count > 0 || HMHCErrorTB.Rows.Count > 0)
            {
                isFail = true;
                dic.Add("POP", HMPOPErrorTB);
                dic.Add("HC", HMHCErrorTB);
            }

            if (isFail)
            {
                Session["failMsg"] = dic;
            }
            else
            {
                Session["failMsg"] = null;
                if (tipsList.Any())
                    Session["tips"] = tipsList;
                else
                    Session["tips"] = null;
            }
            bool isEmptyFrame = false;
            Dictionary<string, DataTable> dicEmptyFrame = new Dictionary<string, DataTable>();
            if (ListEmptyFrameTB != null && ListEmptyFrameTB.Rows.Count > 1)
            {
                isEmptyFrame = true;
                dicEmptyFrame.Add("List", ListEmptyFrameTB);
            }
            if (POPEmptyFrameTB != null && POPEmptyFrameTB.Rows.Count > 0)
            {
                isEmptyFrame = true;
                dicEmptyFrame.Add("POP", POPEmptyFrameTB);
            }
            if (isEmptyFrame)
            {
                Session["emptyFrame"] = dicEmptyFrame;
            }
            else
            {
                Session["emptyFrame"] = null;
            }
            if (POPWrongFrameTB != null && POPWrongFrameTB.Rows.Count > 0)
            {
                Session["POPWrongFrame"] = POPWrongFrameTB;
            }
            else
            {
                Session["POPWrongFrame"] = null;
            }
            if (POPPlaceWarningTB != null && POPPlaceWarningTB.Rows.Count > 0)
            {
                Session["POPPlaceWarning"] = POPPlaceWarningTB;
            }
            else
            {
                Session["POPPlaceWarning"] = null;
            }
            //检查物料支持级别是否统一名称
            DataTable errorMaterialSupportTB = null;
            if (!CheckMaterialSupport(subjectId, out errorMaterialSupportTB))
            {
                if (errorMaterialSupportTB != null && errorMaterialSupportTB.Rows.Count > 0)
                {
                    Session["errorMaterialSupport"] = errorMaterialSupportTB;
                }
                else
                    Session["errorMaterialSupport"] = null;
            }
            else
                Session["errorMaterialSupport"] = null;
        }


        ShopBLL shopBll = new ShopBLL();
        Dictionary<string, int> shopNoList = new Dictionary<string, int>();
        List<string> CloseShopList = new List<string>();
        Dictionary<string, int> InvalidShops = new Dictionary<string, int>();
        List<string> HCList = new List<string>();
        List<string> EmptyInstallLevelList = new List<string>();
        List<string> EmptyFormatList = new List<string>();
        int guidanceType = 0;
        bool CheckShop1(string shopNo, out int shopId, out int IsInvalid, out bool IsHc)
        {
            IsHc = false;
            shopId = 0;
            IsInvalid = 0;
            bool flag = true;
            if (CloseShopList.Contains(shopNo.ToUpper()))
            {
                flag = false;
                shopId = 1;
            }
            else
            {
                if (shopNoList.Keys.Contains(shopNo.ToUpper()))
                {
                    shopId = shopNoList[shopNo.ToUpper()];
                    IsInvalid = InvalidShops[shopNo.ToUpper()];
                }
                else
                {
                    var shop = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).FirstOrDefault();
                    if (shop != null)
                    {
                        shopId = shop.Id;
                        IsInvalid = shop.IsInvalid ?? 0;
                        if (!string.IsNullOrWhiteSpace(shop.Status) && (shop.Status.Contains("关") || shop.Status.Contains("闭")))
                        {

                            CloseShopList.Add(shopNo.ToUpper());
                            flag = false;

                        }
                        else
                        {
                            if (guidanceType == 0)
                            {
                                var guidance = (from subject in CurrentContext.DbContext.Subject
                                                join g in CurrentContext.DbContext.SubjectGuidance
                                                on subject.GuidanceId equals g.ItemId
                                                where subject.Id == subjectId
                                                select g).FirstOrDefault();
                                if (guidance != null)
                                    guidanceType = guidance.ActivityTypeId ?? 1;
                            }
                            if (guidanceType != 3 && !string.IsNullOrWhiteSpace(shop.Format))
                            {
                                //非促销的活动要检查是不是HC店铺，如果是，就不导入
                                string format = shop.Format.ToUpper();
                                if (format.Contains("HC") || format.Contains("HOMECOURT-C") || format.Contains("HOMECORE") || format.Contains("HOMECOURT"))
                                {
                                    IsHc = true;
                                }
                            }

                            else
                            {
                                if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                                    shopNoList.Add(shopNo.ToUpper(), shopId);
                                if (!InvalidShops.Keys.Contains(shopNo.ToUpper()))
                                    InvalidShops.Add(shopNo.ToUpper(), IsInvalid);
                            }
                        }

                    }
                    else
                        flag = false;
                }
            }
            return flag;
        }


        bool CheckShop(string shopNo, out int shopId, out int IsInvalid, out bool IsHc, out bool EmptyInstallLevel, out bool EmptyFormat)
        {
            IsHc = false;
            EmptyInstallLevel = false;
            EmptyFormat = false;
            shopId = 0;
            IsInvalid = 0;//关闭店铺
            bool flag = true;

            if (shopNoList.Keys.Contains(shopNo.ToUpper()))
            {
                shopId = shopNoList[shopNo.ToUpper()];
                if (InvalidShops.Keys.Contains(shopNo.ToUpper()))
                    IsInvalid = InvalidShops[shopNo.ToUpper()];
                if (HCList.Contains(shopNo.ToUpper()))
                    IsHc = true;
                if (EmptyInstallLevelList.Contains(shopNo.ToUpper()))
                    EmptyInstallLevel = true;
                if (EmptyFormatList.Contains(shopNo.ToUpper()))
                    EmptyFormat = true;
            }
            else
            {
                var shop = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).FirstOrDefault();
                if (shop != null)
                {
                    shopId = shop.Id;
                    //IsInvalid = shop.IsInvalid ?? 0;
                    //关闭店铺
                    if (!string.IsNullOrWhiteSpace(shop.Status) && (shop.Status.Contains("关") || shop.Status.Contains("闭")))
                    {
                        IsInvalid = shopId;
                        if (!InvalidShops.Keys.Contains(shopNo.ToUpper()))
                            InvalidShops.Add(shopNo.ToUpper(), IsInvalid);
                    }
                    if (guidanceType == 0)
                    {
                        var guidance = (from subject in CurrentContext.DbContext.Subject
                                        join g in CurrentContext.DbContext.SubjectGuidance
                                        on subject.GuidanceId equals g.ItemId
                                        where subject.Id == subjectId
                                        select g).FirstOrDefault();
                        if (guidance != null)
                            guidanceType = guidance.ActivityTypeId ?? 1;//活动类型：1安装，2发货，3促销
                    }
                    if (guidanceType != 3)
                    {
                        if (!string.IsNullOrWhiteSpace(shop.Format))
                        {
                            //非促销的活动要检查是不是HC店铺，如果是，就不导入Homecourt
                            string format = shop.Format.ToUpper();

                            if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE"))
                            {
                                IsHc = true;
                                if (!HCList.Contains(shopNo.ToUpper()))
                                    HCList.Add(shopNo.ToUpper());
                            }
                        }
                        if (string.IsNullOrWhiteSpace(shop.IsInstall))
                        {
                            EmptyInstallLevel = true;
                            if (!EmptyInstallLevelList.Contains(shopNo.ToUpper()))
                                EmptyInstallLevelList.Add(shopNo.ToUpper());
                        }
                        if (string.IsNullOrWhiteSpace(shop.Format))
                        {
                            EmptyFormat = true;
                            if (!EmptyFormatList.Contains(shopNo.ToUpper()))
                                EmptyFormatList.Add(shopNo.ToUpper());
                        }
                    }
                    //if (!IsHc)
                    //{

                    //}
                    if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                        shopNoList.Add(shopNo.ToUpper(), shopId);

                }
                else
                    flag = false;
            }

            return flag;
        }


        Dictionary<string, Shop> shopDic = new Dictionary<string, Shop>();
        bool GetShopFromDB(string shopNo, ref Shop shopFromDb)
        {
            bool flag = false;
            shopNo = shopNo.ToUpper();
            if (shopDic.Keys.Contains(shopNo))
            {
                shopFromDb = shopDic[shopNo];
                flag = true;
            }
            else
            {
                var shopModel = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo && (s.IsDelete == null || s.IsDelete == false)).FirstOrDefault();
                if (shopModel != null)
                {
                    shopFromDb = shopModel;
                    shopDic.Add(shopNo, shopFromDb);
                    flag = true;
                }
            }
            if (guidanceType == 0)
            {
                var guidance = (from subject in CurrentContext.DbContext.Subject
                                join g in CurrentContext.DbContext.SubjectGuidance
                                on subject.GuidanceId equals g.ItemId
                                where subject.Id == subjectId
                                select g).FirstOrDefault();
                if (guidance != null)
                {
                    guidanceType = guidance.ActivityTypeId ?? 1;//活动类型：1安装，2发货，3促销
                    guidanceId = guidance.ItemId;
                }
            }
            return flag;
        }

        bool CheckHandMakeOrderShop11(string shopNo, DataRow dr, out int shopId, out string msg)
        {
            shopId = 0;
            msg = string.Empty;
            bool flag = true;
            if (shopNoList.Keys.Contains(shopNo.ToUpper()))
            {
                shopId = shopNoList[shopNo.ToUpper()];
            }
            else
            {
                var shop = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).FirstOrDefault();
                if (shop != null)
                {
                    shopId = shop.Id;
                    if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                        shopNoList.Add(shopNo.ToUpper(), shopId);

                }
                else
                {
                    shopId = AddShop(dr, out msg);
                    if (shopId > 0)
                    {
                        if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                            shopNoList.Add(shopNo.ToUpper(), shopId);
                    }
                    else
                        flag = false;
                }
            }
            return flag;
        }


        POPBLL popBll = new POPBLL();
        POPBlackListBLL POPBlackBll = new POPBlackListBLL();
        //没用
        bool CheckGraphicNo(int shopId, string sheet, string graphicNo, out string realGender, out string errorMsg)
        {
            realGender = string.Empty;
            errorMsg = string.Empty;
            //return popBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet && s.GraphicNo.ToUpper() == graphicNo.Trim().ToUpper() && ((s.Gender=="")||(s.Gender==null)||(s.Gender == gender) || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女")))).Any();
            var model = popBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet && s.GraphicNo.ToUpper() == graphicNo.Trim().ToUpper()).FirstOrDefault();
            if (model != null)
            {
                realGender = model.Gender;
                return true;
            }
            else
            {
                var model1 = POPBlackBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet).FirstOrDefault();
                if (model1 != null)
                {
                    if (!string.IsNullOrWhiteSpace(model1.GraphicNo))
                    {
                        List<string> list = StringHelper.ToStringList(model1.GraphicNo, ',');
                        if (list.Contains(graphicNo.ToUpper()))
                            errorMsg = "该店铺不含该位置的POP，分区已确认；";
                        else
                            errorMsg = "该店铺缺少该位置的POP，请更新基础数据；";
                    }

                }
                else
                    errorMsg = "该店铺缺少该位置的POP，请更新基础数据；";
                return false;
            }


        }

        bool GetPOPFromDB(int shopId, string sheet, string graphicNo, ref POP popFromDB, out string errorMsg)
        {
            bool flag = false;
            errorMsg = string.Empty;
            var popModel = popBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet && s.GraphicNo.ToUpper() == graphicNo.ToUpper()).FirstOrDefault();
            if (popModel != null)
            {
                popFromDB = popModel;
                flag = true;
            }
            else
            {
                var model1 = POPBlackBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet).FirstOrDefault();
                if (model1 != null)
                {
                    if (!string.IsNullOrWhiteSpace(model1.GraphicNo))
                    {
                        List<string> list = StringHelper.ToStringList(model1.GraphicNo, ',');
                        if (list.Contains(graphicNo.ToUpper()))
                            errorMsg = "该店铺不含该位置的POP，分区已确认；";
                        else
                            errorMsg = "该店铺缺少该位置的POP，请更新基础数据；";
                    }

                }
                else
                    errorMsg = "该店铺缺少该位置的POP，请更新基础数据；";
            }
            return flag;
        }



        List<Models.Position> positionList = new List<Models.Position>();
        bool ChcekSheet(string sheet, out int positionId)
        {
            positionId = 0;
            bool flag = false;
            if (!positionList.Any())
            {
                positionList = new PositionBLL().GetList(s => 1 == 1);
            }
            if (positionList.Any())
            {
                var list = positionList.Where(s => s.PositionName.ToLower() == sheet.ToLower()).ToList();
                if (list.Any())
                {
                    positionId = list[0].Id;
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 检查器架是否为空
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        List<ShopMachineFrame> FrameList = new List<ShopMachineFrame>();
        MachineFrameBlackListBLL mfBlackListBll = new MachineFrameBlackListBLL();
        string cornerType = string.Empty;
        bool isCheckCornerType = false;
        ShopMachineFrameBLL shopFrameBll = new ShopMachineFrameBLL();
        bool IsExistMachineFrame(int shopId, string sheet, string gender, out string errorMsg)
        {
            if (!isCheckCornerType)
            {
                Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                if (subjectModel != null)
                {
                    cornerType = subjectModel.CornerType;
                    isCheckCornerType = true;
                }
            }
            errorMsg = string.Empty;
            bool flag = false;
            if (!FrameList.Any())
            {
                FrameList = shopFrameBll.GetList(s => s.Id > 0);
            }
            if (FrameList.Any())
            {
                var list = FrameList.Where(s => s.ShopId == shopId && s.PositionName.Trim() == sheet.Trim() && ((s.Gender == gender) || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女"))));
                if (!string.IsNullOrWhiteSpace(cornerType))
                {
                    list = list.Where(s => s.CornerType == cornerType.Trim()).ToList();
                }
                else
                {
                    list = list.Where(s => s.CornerType == null || s.CornerType == "").ToList();
                }
                flag = list.Any();
                if (!flag)
                {
                    var blackList = mfBlackListBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet.ToUpper() && ((s.Gender == gender) || (s.Gender == null || s.Gender == "") || (s.Gender != null && s.Gender.Contains("男") && s.Gender.Contains("女"))));
                    if (!string.IsNullOrWhiteSpace(cornerType))
                    {
                        blackList = blackList.Where(s => s.CornerType == cornerType.Trim()).ToList();
                    }
                    else
                    {
                        blackList = blackList.Where(s => s.CornerType == null || s.CornerType == "").ToList();
                    }
                    if (blackList.Any())
                    {

                        errorMsg = string.Format("已确认无{0}{1}的器架；", gender, sheet);
                    }
                    //flag = false;
                }
            }
            return flag;
        }


        bool IsPOPExistMachineFrame(int shopId, string sheet, string frameName, string gender, string cornerType, out string errorMsg)
        {
            errorMsg = string.Empty;
            bool flag = false;
            var list = shopFrameBll.GetList(s => s.ShopId == shopId && s.PositionName == sheet && s.MachineFrame == frameName && ((s.Gender == gender) || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女"))));
            if (!string.IsNullOrWhiteSpace(cornerType))
            {
                list = list.Where(s => s.CornerType == cornerType).ToList();
            }
            else
            {
                list = list.Where(s => s.CornerType == null || s.CornerType == "").ToList();
            }
            if (list.Any())
            {
                flag = true;
            }
            else
            {
                errorMsg = "该POP对应的器架不存在；";
            }
            return flag;
        }

        /// <summary>
        /// 合并订单
        /// </summary>
        /// 
        void MergeOrder()
        {

            MergeOriginalOrder model;
            ListOrderDetail listOrderModel;
            POPOrderDetail popdetail;
            mergeOrderBll.Delete(s => s.SubjectId == subjectId);
            var list = listBll.GetList(s => s.SubjectId == subjectId);
            var pop = popOrderBll.GetList(s => s.SubjectId == subjectId);
            var supplement = supplementBll.GetList(s => s.SubjectId == subjectId);
            //List<MergeOriginalOrder> MergeList = new List<MergeOriginalOrder>();
            #region 先把补充订单数据加到list里面(按店铺编码、位置、性别作为条件，相同的就保存大数量的那个)
            supplement.ForEach(s =>
            {
                var listModel = list.FirstOrDefault(l => l.Sheet == s.Sheet && l.ShopId == s.ShopId && ((l.Gender == s.Gender) || (l.Gender.Contains("男") && l.Gender.Contains("女")) || (s.Gender.Contains("男") && s.Gender.Contains("女"))));
                if (listModel != null)
                {
                    int index = list.IndexOf(listModel);
                    if (listModel.Quantity < s.Quantity)
                    {
                        listModel.Quantity = s.Quantity;
                        list[index] = listModel;
                    }
                }
                else
                {
                    listOrderModel = new ListOrderDetail()
                    {
                        AddDate = s.AddDate,
                        AddUserId = s.AddUserId,
                        ChooseImg = s.ChooseImg,
                        Gender = s.Gender,

                        PositionId = s.PositionId,
                        Quantity = s.Quantity,
                        Remark = s.Remark,
                        Sheet = s.Sheet,
                        ShopId = s.ShopId,
                        SubjectId = s.SubjectId,
                        MaterialSupport = s.MaterialSupport,
                        POSScale = s.POSScale,
                        LevelNum = 1,

                    };
                    list.Add(listOrderModel);
                }
            });
            //再把list合并到pop中（按店铺编码、位置、性别作为条件，相同的就保存大数量的那个）

            if (list.Any())
            {
                list.ForEach(l =>
                {

                    popdetail = new POPOrderDetail();
                    popdetail.AddDate = DateTime.Now;
                    popdetail.AddUserId = CurrentUser.UserId;
                    popdetail.ChooseImg = l.ChooseImg;
                    popdetail.Gender = l.Gender;
                    popdetail.OrderGender = l.Gender;
                    popdetail.MaterialSupport = l.MaterialSupport;
                    popdetail.PositionId = l.PositionId;
                    popdetail.POSScale = l.POSScale;
                    popdetail.Quantity = l.Quantity;
                    popdetail.Remark = l.Remark;
                    popdetail.Sheet = l.Sheet;
                    popdetail.ShopId = l.ShopId;
                    popdetail.SubjectId = l.SubjectId;
                    popdetail.CornerType = l.CornerType;
                    popdetail.LevelNum = l.LevelNum;
                    popdetail.Channel = l.Channel;
                    popdetail.CityName = l.CityName;
                    popdetail.CityTier = l.CityTier;
                    popdetail.Format = l.Format;
                    popdetail.IsInstall = l.IsInstall;
                    popdetail.ProvinceName = l.ProvinceName;
                    popdetail.RegionName = l.RegionName;
                    popdetail.ShopName = l.ShopName;
                    popdetail.ShopNo = l.ShopNo;
                    pop.Add(popdetail);

                });

            }
            #endregion

            #region 更新那些“物料支持”和“规模大小”为空的
            if (pop.Any())
            {
                var emptyList = pop.Where(s => string.IsNullOrWhiteSpace(s.MaterialSupport) == true || string.IsNullOrWhiteSpace(s.POSScale) == true).ToList();
                Dictionary<int, string> dic = new Dictionary<int, string>();
                emptyList.ForEach(s =>
                {
                    int index = pop.IndexOf(s);
                    if (string.IsNullOrWhiteSpace(s.MaterialSupport))
                    {
                        var model1 = pop.Where(m => string.IsNullOrWhiteSpace(m.MaterialSupport) == false && m.ShopId == s.ShopId).FirstOrDefault();
                        if (model1 != null)
                        {
                            s.MaterialSupport = model1.MaterialSupport;
                        }

                    }
                    if (string.IsNullOrWhiteSpace(s.POSScale))
                    {
                        var model1 = pop.Where(m => string.IsNullOrWhiteSpace(m.POSScale) == false && m.ShopId == s.ShopId).FirstOrDefault();
                        if (model1 != null)
                        {
                            s.POSScale = model1.POSScale;
                        }
                        else
                        {
                            if (dic.Keys.Contains(s.ShopId ?? 0))
                                s.POSScale = dic[s.ShopId ?? 0];
                            else
                            {
                                Shop shopModel = new ShopBLL().GetModel(s.ShopId ?? 0);
                                if (shopModel != null && !string.IsNullOrWhiteSpace(shopModel.POSScale))
                                {
                                    s.POSScale = shopModel.POSScale;
                                    dic.Add(shopModel.Id, shopModel.POSScale);
                                }
                            }
                        }
                    }
                    pop[index] = s;
                });
                //dic = null;
            }
            #endregion
            #region 最后保存到合并表中

            if (pop.Any())
            {



                pop.ForEach(p =>
                {
                    model = new MergeOriginalOrder();
                    model.AddDate = DateTime.Now;
                    model.ChooseImg = p.ChooseImg;
                    model.Gender = p.Gender;
                    model.OrderGender = p.OrderGender;
                    model.GraphicNo = p.GraphicNo;
                    model.GraphicLength = p.GraphicLength;
                    model.GraphicWidth = p.GraphicWidth;
                    model.GraphicMaterial = p.GraphicMaterial;
                    model.MaterialSupport = p.MaterialSupport;
                    model.POPName = p.POPName;
                    model.POPType = p.POPType;
                    model.PositionId = p.PositionId;
                    model.POSScale = p.POSScale;
                    model.Quantity = p.Quantity;
                    model.Remark = p.Remark;
                    model.Sheet = p.Sheet;
                    model.ShopId = p.ShopId;
                    model.SubjectId = p.SubjectId;
                    model.CornerType = p.CornerType;
                    model.LevelNum = p.LevelNum;
                    model.Channel = p.Channel;
                    model.CityName = p.CityName;
                    model.CityTier = p.CityTier;
                    model.Format = p.Format;
                    model.IsInstall = p.IsInstall;
                    model.ProvinceName = p.ProvinceName;
                    model.RegionName = p.RegionName;
                    model.ShopName = p.ShopName;
                    model.ShopNo = p.ShopNo;
                    model.PositionDescription = p.PositionDescription;
                    mergeOrderBll.Add(model);

                });

            }
            #endregion
            
           


        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            if (Session["failMsg"] != null)
            {
                //List<DataTable> dts = (List<DataTable>)Session["failMsg"];
                Dictionary<string, DataTable> dts = (Dictionary<string, DataTable>)Session["failMsg"];
                OperateFile.ExportTables(dts, "导入失败信息");
            }
        }


        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGoNext_Click(object sender, EventArgs e)
        {
            var list = mergeOrderBll.GetList(s => s.SubjectId == subjectId);
            if (list.Any())
            {

                Response.Redirect("SplitOrderSuccess.aspx?subjectId=" + subjectId, false);
            }
            else
                Alert("请先导入订单");
        }


        /// <summary>
        /// 如果是陈列桌，检查级别和系统数量，系统数量不足就不导入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        ShopMachineFrameBLL frmaeBll = new ShopMachineFrameBLL();
        bool CheckFrameNumber(int shopid, string gender, int levelNum)
        {
            bool flag = true;
            //if (sheet.Contains("桌"))
            //{

            //}
            int sysCount = frmaeBll.GetList(s => s.ShopId == shopid && s.PositionName == "陈列桌" && ((s.Gender == "") || (s.Gender == null) || (s.Gender == gender) || (s.Gender.Contains("男") && s.Gender.Contains("女")))).Sum(s => s.Count) ?? 0;
            flag = sysCount >= levelNum;
            return flag;
        }

        protected void lbGoSkip_Click(object sender, EventArgs e)
        {

        }

        List<OrderMaterial> materialList = new List<OrderMaterial>();
        bool IsExistOrderMaterial(int subjectId, int shopId, string materialName, out int materialId)
        {
            materialId = 0;
            bool flag = false;
            if (!materialList.Any())
            {
                materialList = materialBll.GetList(s => s.SubjectId == subjectId);
            }

            if (materialList.Any())
            {
                var model = materialList.Where(s => s.ShopId == shopId && s.MaterialName.ToLower() == materialName.ToLower()).FirstOrDefault();
                if (model != null)
                {
                    materialId = model.MaterialId;
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 不需拆单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        protected void lbGoSkip_Click1(object sender, EventArgs e)
        {

            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    int MaterialPriceId = 0;
                    string subjectCategoryName = string.Empty;
                    //
                    int flag = 0;
                    FinalOrderDetailTempBLL finalOrderTempBll = new FinalOrderDetailTempBLL();
                    FinalOrderDetailTemp finalOrderTempModel;
                    //先删除所有之前已拆完的订单
                    Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                    if (subjectModel != null)
                    {
                        if (subjectModel.SubjectType == (int)SubjectTypeEnum.HC订单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区补单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区增补 || subjectModel.SubjectType == (int)SubjectTypeEnum.新开店订单)
                        {
                            finalOrderTempBll.Delete(s => s.RegionSupplementId == subjectId);
                            new QuoteOrderDetailBLL().Delete(s => s.RegionSupplementId == subjectId);
                        }
                        else
                        {
                            finalOrderTempBll.Delete(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0);
                            new QuoteOrderDetailBLL().Delete(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0);
                        }
                    }
                    //finalOrderTempBll.Delete(s => s.SubjectId == subjectId);
                    var listOrder = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                     join shop in CurrentContext.DbContext.Shop
                                     on order.ShopId equals shop.Id
                                     join subject in CurrentContext.DbContext.Subject
                                     on order.SubjectId equals subject.Id
                                     join guidance in CurrentContext.DbContext.SubjectGuidance
                                     on subject.GuidanceId equals guidance.ItemId
                                     join pop1 in CurrentContext.DbContext.POP
                                     on new { shopid = order.ShopId, graphicNo = order.GraphicNo } equals new { shopid = pop1.ShopId, graphicNo = pop1.GraphicNo } into popTemp
                                     from pop in popTemp.DefaultIfEmpty()
                                     join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                     on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                                     from subjectCategory in categoryTemp.DefaultIfEmpty()
                                     where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                                     select new
                                     {
                                         order,
                                         shop,
                                         pop,
                                         guidance.PriceItemId,
                                         subjectCategory,
                                         subject.SubjectType,
                                         subject.GuidanceId
                                     }).ToList();
                    if (listOrder.Any())
                    {
                        flag++;
                        MaterialPriceId = listOrder[0].PriceItemId ?? 0;
                        subjectCategoryName = listOrder[0].subjectCategory != null ? listOrder[0].subjectCategory.CategoryName : string.Empty;
                        string unitName = string.Empty;
                        listOrder.ForEach(o =>
                        {

                            finalOrderTempModel = new FinalOrderDetailTemp();
                            finalOrderTempModel.AgentCode = o.shop.AgentCode;
                            finalOrderTempModel.AgentName = o.shop.AgentName;
                            finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                            finalOrderTempModel.Channel = o.shop.Channel;
                            finalOrderTempModel.City = o.shop.CityName;
                            finalOrderTempModel.CityTier = o.shop.CityTier;
                            finalOrderTempModel.Contact = o.shop.Contact1;
                            finalOrderTempModel.Format = o.shop.Format;
                            finalOrderTempModel.Gender = (o.order.OrderGender!=null && o.order.OrderGender!="")?o.order.OrderGender:o.order.Gender;
                            finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                            finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                            finalOrderTempModel.LocationType = o.shop.LocationType;
                            finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                            finalOrderTempModel.OrderType = 1;
                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                            finalOrderTempModel.POSScale = o.order.POSScale;
                            finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                            finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                            //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                            if (subjectCategoryName == "童店")
                                finalOrderTempModel.InstallPriceMaterialSupport = "Others";
                            finalOrderTempModel.Province = o.shop.ProvinceName;
                            finalOrderTempModel.Quantity = o.order.Quantity;
                            finalOrderTempModel.Region = o.shop.RegionName;
                            finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                            //finalOrderTempModel.Remark = o.order.Remark;
                            finalOrderTempModel.SplitOrderRemark = "不需拆单";
                            finalOrderTempModel.Sheet = o.order.Sheet;
                            finalOrderTempModel.ShopId = o.shop.Id;
                            finalOrderTempModel.ShopName = o.shop.ShopName;
                            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                            finalOrderTempModel.SubjectId = subjectId;
                            finalOrderTempModel.Tel = o.shop.Tel1;
                            finalOrderTempModel.MachineFrame = "";
                            finalOrderTempModel.LevelNum = o.order.LevelNum;
                            if (o.pop != null)
                            {
                                decimal width = o.pop.GraphicWidth ?? 0;
                                decimal length = o.pop.GraphicLength ?? 0;

                                finalOrderTempModel.GraphicLength = length;
                                finalOrderTempModel.GraphicMaterial = o.pop.GraphicMaterial;
                                finalOrderTempModel.GraphicWidth = width;

                                finalOrderTempModel.PositionDescription = o.pop.PositionDescription;
                                finalOrderTempModel.POPName = o.pop.POPName;
                                finalOrderTempModel.POPType = o.pop.POPType;
                                finalOrderTempModel.WindowDeep = o.pop.WindowDeep;
                                finalOrderTempModel.WindowHigh = o.pop.WindowHigh;
                                finalOrderTempModel.WindowSize = o.pop.WindowSize;
                                finalOrderTempModel.WindowWide = o.pop.WindowWide;
                                finalOrderTempModel.IsElectricity = o.pop.IsElectricity;
                                finalOrderTempModel.Area = (width * length) / 1000000;
                                decimal totalPrice = 0;
                                o.pop.Quantity = o.order.Quantity;
                                o.pop.PriceItemId = MaterialPriceId;
                                decimal unitPrice = GetMaterialUnitPrice(o.pop, out totalPrice, out unitName);
                                finalOrderTempModel.UnitPrice = unitPrice;
                                finalOrderTempModel.TotalPrice = totalPrice;
                                finalOrderTempModel.Remark = o.pop.Remark;
                                finalOrderTempModel.IsValid = o.pop.IsValid;
                                finalOrderTempModel.ProduceOutsourceId = o.pop.ProduceOutsourceId;
                            }
                            if (o.SubjectType == (int)SubjectTypeEnum.正常单)
                            {
                                finalOrderTempModel.IsFromRegion = true;
                            }
                            finalOrderTempModel.ShopStatus = o.shop.Status;
                            finalOrderTempModel.GuidanceId = o.GuidanceId;
                            finalOrderTempModel.CSUserId = o.shop.CSUserId;
                            finalOrderTempModel.UnitName = unitName;
                            finalOrderTempModel.AddDate = DateTime.Now;
                            finalOrderTempBll.Add(finalOrderTempModel);
                            //保存报价单
                            
                            //new BasePage().SaveQuotationOrder(finalOrderTempModel);
                        });
                    }
                    //物料信息
                    var materialOrderList = (from material in CurrentContext.DbContext.OrderMaterial
                                             join shop in CurrentContext.DbContext.Shop
                                             on material.ShopId equals shop.Id
                                             join subject in CurrentContext.DbContext.Subject
                                             on material.SubjectId equals subject.Id
                                             where material.SubjectId == subjectId
                                             select new
                                             {
                                                 subject,
                                                 material,
                                                 shop
                                             }).ToList();
                    if (materialOrderList.Any())
                    {
                        materialOrderList.ForEach(o =>
                        {
                            flag++;
                            finalOrderTempModel = new FinalOrderDetailTemp();
                            finalOrderTempModel.AgentCode = o.shop.AgentCode;
                            finalOrderTempModel.AgentName = o.shop.AgentName;
                            finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                            finalOrderTempModel.Channel = o.shop.Channel;
                            finalOrderTempModel.City = o.shop.CityName;
                            finalOrderTempModel.CityTier = o.shop.CityTier;
                            finalOrderTempModel.Contact = o.shop.Contact1;
                            finalOrderTempModel.Format = o.shop.Format;
                            //finalOrderTempModel.Gender = o.order.Gender;
                            //finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                            finalOrderTempModel.IsInstall = o.shop.IsInstall;
                            finalOrderTempModel.LocationType = o.shop.LocationType;
                            //finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                            finalOrderTempModel.OrderType = (int)OrderTypeEnum.物料;
                            finalOrderTempModel.POPAddress = o.shop.POPAddress;
                            //finalOrderTempModel.POSScale = o.order.POSScale;
                            //finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                            //finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                            //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                            //if (subjectCategoryName == "童店")
                            //finalOrderTempModel.InstallPriceMaterialSupport = "Others";
                            finalOrderTempModel.Province = o.shop.ProvinceName;
                            finalOrderTempModel.Quantity = o.material.MaterialCount;
                            finalOrderTempModel.Region = o.shop.RegionName;
                            //finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                            finalOrderTempModel.Sheet = o.material.MaterialName;
                            StringBuilder size = new StringBuilder();
                            if (o.material.MaterialLength != null && o.material.MaterialLength > 0 && o.material.MaterialWidth != null && o.material.MaterialWidth > 0)
                            {
                                size.AppendFormat("({0}*{1}", o.material.MaterialLength, o.material.MaterialWidth);
                                if (o.material.MaterialHigh != null && o.material.MaterialHigh > 0)
                                    size.AppendFormat("*{0}", o.material.MaterialHigh);
                                size.Append(")");
                            }
                            finalOrderTempModel.PositionDescription = size.ToString();
                            finalOrderTempModel.Remark = o.material.Remark;
                            finalOrderTempModel.ShopId = o.shop.Id;
                            finalOrderTempModel.ShopName = o.shop.ShopName;
                            finalOrderTempModel.ShopNo = o.shop.ShopNo;
                            finalOrderTempModel.SubjectId = subjectId;
                            finalOrderTempModel.Tel = o.shop.Tel1;
                            finalOrderTempModel.MachineFrame = "";
                            if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                            {
                                finalOrderTempModel.IsFromRegion = true;
                            }
                            finalOrderTempModel.ShopStatus = o.shop.Status;
                            finalOrderTempModel.GuidanceId = o.subject.GuidanceId;
                            if ((o.material.Price ?? 0) > 0)
                            {
                                finalOrderTempModel.UnitPrice = o.material.Price;
                                finalOrderTempModel.OrderPrice = (o.material.Price ?? 0) * (o.material.MaterialCount ?? 0);
                                finalOrderTempModel.PayOrderPrice = (o.material.PayPrice ?? 0) * (o.material.MaterialCount ?? 0);
                            }
                            finalOrderTempModel.CSUserId = o.shop.CSUserId;
                            finalOrderTempModel.AddDate = DateTime.Now;
                            finalOrderTempBll.Add(finalOrderTempModel);
                            //保存报价单
                            //new BasePage().SaveQuotationOrder(finalOrderTempModel,false);
                        });
                    }

                    if (flag > 0)
                    {
                        SubjectBLL subjectBll = new SubjectBLL();
                        //Subject subjectModel = subjectBll.GetModel(subjectId);
                        if (subjectModel != null)
                        {
                            subjectModel.Status = 3;
                            subjectBll.Update(subjectModel);
                        }
                        tran.Complete();
                        Response.Redirect("SplitOrderSuccess.aspx?subjectId=" + subjectId, false);
                    }
                    else
                        Alert("请先导入订单");
                }
                catch (Exception ex)
                {
                    Alert("提交失败！");
                }

            }



        }

        /// <summary>
        /// 导入费用订单
        /// </summary>
        /// <param name="ds"></param>
        void ImportPriceOrder(DataSet ds)
        {
            int guidanceId = 0;
            if (!string.IsNullOrWhiteSpace(hfGuidanceId.Value))
            {
                guidanceId = int.Parse(hfGuidanceId.Value);
            }
            int subjectType = 0;
            if (!string.IsNullOrWhiteSpace(hfSubjectType.Value))
                subjectType = int.Parse(hfSubjectType.Value);
            PriceOrderDetailBLL priceOrderBll = new PriceOrderDetailBLL();
            if (!cbAdd.Checked)
                priceOrderBll.Delete(s => s.SubjectId == subjectId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                Dictionary<string, string> regionChangeDic = new Dictionary<string, string>();
                string tempStr = string.Empty;
                try
                {
                    tempStr = ConfigurationManager.AppSettings["RegionChangeLanguage"];
                }
                catch { }
                if (!string.IsNullOrWhiteSpace(tempStr))
                {
                    List<string> list1 = StringHelper.ToStringList(tempStr, '|');
                    if (list1.Any())
                    {
                        list1.ForEach(s =>
                        {
                            string eng = s.Split(':')[0];
                            string ch = s.Split(':')[1];
                            if (!regionChangeDic.Keys.Contains(eng))
                            {
                                regionChangeDic.Add(eng, ch);
                            }
                        });
                    }
                }



                DataColumnCollection cols = ds.Tables[0].Columns;
                DataTable errorTB = CommonMethod.CreateErrorTB(cols);
                int shopId = 0;
                int orderType = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StringBuilder msg = new StringBuilder();
                    bool canSave = true;

                    string shopNo = string.Empty;

                    string ShopName = string.Empty;
                    string Region = string.Empty;
                    string Province = string.Empty;
                    string City = string.Empty;
                    string Address = string.Empty;
                    string price = string.Empty;
                    string count = string.Empty;
                    string contents = string.Empty;
                    string remark = string.Empty;
                    if (cols.Contains("店铺编号"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["店铺编号"].ToString());
                    else if (cols.Contains("POSCode"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POSCode"].ToString());
                    else if (cols.Contains("POS Code"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POS Code"].ToString());

                    if (cols.Contains("店铺名称"))
                        ShopName = StringHelper.ReplaceSpecialChar(dr["店铺名称"].ToString());
                    if (cols.Contains("区域"))
                        Region = StringHelper.ReplaceSpecialChar(dr["区域"].ToString());
                    if (cols.Contains("省份"))
                        Province = StringHelper.ReplaceSpecialChar(dr["省份"].ToString());
                    if (cols.Contains("城市"))
                        City = StringHelper.ReplaceSpecialChar(dr["城市"].ToString());
                    if (cols.Contains("店铺地址"))
                        Address = StringHelper.ReplaceSpecialChar(dr["店铺地址"].ToString());

                    if (cols.Contains("金额"))
                        price = StringHelper.ReplaceSpecialChar(dr["金额"].ToString());

                    if (cols.Contains("费用内容"))
                        contents = StringHelper.ReplaceSpecialChar(dr["费用内容"].ToString());
                    else if (cols.Contains("内容"))
                        contents = StringHelper.ReplaceSpecialChar(dr["内容"].ToString());

                    if (cols.Contains("备注"))
                        remark = StringHelper.ReplaceSpecialChar(dr["备注"].ToString());

                    if (string.IsNullOrWhiteSpace(ShopName))
                    {
                        canSave = false;
                        msg.Append("店铺名称不能空；");
                    }
                    if (string.IsNullOrWhiteSpace(Region))
                    {
                        canSave = false;
                        msg.Append("区域不能空；");
                    }
                    if (string.IsNullOrWhiteSpace(Province))
                    {
                        canSave = false;
                        msg.Append("省份不能空；");
                    }
                    else
                    {
                        int provinceId = GetProvinceId(Province);
                        if (provinceId == 0)
                        {
                            canSave = false;
                            msg.Append("省份填写不正确；");
                        }

                    }
                    
                    if (string.IsNullOrWhiteSpace(price))
                    {
                        canSave = false;
                        msg.Append("金额不能空；");
                    }
                    else if (!StringHelper.IsDecimalVal(price))
                    {
                        canSave = false;
                        msg.Append("金额填写不正确；");
                    }
                    if (string.IsNullOrWhiteSpace(remark) && subjectType == (int)SubjectTypeEnum.新开店安装费)
                    {
                        canSave = false;
                        msg.Append("备注不能空；");
                    }
                    if (!string.IsNullOrWhiteSpace(Region))
                    {
                        if (regionChangeDic.Keys.Count > 0 && regionChangeDic.Keys.Contains(Region))
                        {
                            Region = regionChangeDic[Region];
                        }
                        Region = Region.Substring(0, 1).ToUpper() + Region.Substring(1).ToLower();
                    }
                    if (canSave)
                    {
                        PriceOrderDetail order = new PriceOrderDetail();
                        order.AddDate = DateTime.Now;
                        order.Amount = StringHelper.IsDecimal(price);
                        order.Remark = remark;
                        order.ShopId = shopId;
                        order.GuidanceId = guidanceId;
                        order.SubjectId = subjectId;
                        order.Address = Address;
                        order.City = City;
                        order.Province = Province;
                        order.Region = Region;
                        order.ShopName = ShopName;
                        order.Contents = contents;
                        order.SubjectType = subjectType;
                        priceOrderBll.Add(order);
                        successNum++;
                    }
                    if (msg.Length > 0)
                    {
                        DataRow dr1 = errorTB.NewRow();
                        for (int ii = 0; ii < cols.Count; ii++)
                        {
                            dr1["" + cols[ii] + ""] = dr[ii].ToString();
                        }
                        dr1["错误信息"] = msg.ToString();
                        errorTB.Rows.Add(dr1);
                    }
                }
                if (errorTB.Rows.Count > 0)
                {
                    Session["errorTb"] = errorTB;
                }

            }

        }

        /// <summary>
        /// 导入手工订单
        /// </summary>
        /// 
        DataColumnCollection HandMakeOrderCols;
        void ImportHnadMakeOrder_POP(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                HandMadeOrderDetailBLL handOrderBll = new HandMadeOrderDetailBLL();
                Dictionary<string, int> orderTypeDic = new Dictionary<string, int>();
                orderTypeDic.Add("pop", 1);
                orderTypeDic.Add("道具", 2);
                orderTypeDic.Add("物料", 3);

                string orderType = string.Empty;
                string shopNo = string.Empty;
                int shopId = 0;
                string sheet = string.Empty;
                string machineFrame = string.Empty;
                string levelNum = string.Empty;
                string gender = string.Empty;
                string num = string.Empty;
                string width = string.Empty;
                string length = string.Empty;
                string material = string.Empty;
                //string unitPrice = string.Empty;
                string chooseImg = string.Empty;
                string positionDescription = string.Empty;
                string remark = string.Empty;
                string category = string.Empty;
                string materialSupport = string.Empty;
                string posScale = string.Empty;
                string installPositionDescription = string.Empty;
                HandMakeOrderCols = ds.Tables[0].Columns;
                HMPOPErrorTB = CommonMethod.CreateErrorTB(HandMakeOrderCols);
                HandMadeOrderDetail handOrderModel;
                //int successNum = 0;
                //int failNum = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StringBuilder msg = new StringBuilder();
                    if (HandMakeOrderCols.Contains("订单类型"))
                    {
                        orderType = StringHelper.ReplaceSpecialChar(dr["订单类型"].ToString().Trim());
                    }
                    if (HandMakeOrderCols.Contains("店铺编号"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["店铺编号"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("POSCode"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POSCode"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("POS Code"))
                        shopNo = StringHelper.ReplaceSpecialChar(dr["POS Code"].ToString().Trim());
                    if (HandMakeOrderCols.Contains("POP位置"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["POP位置"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("位置"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["位置"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("Sheet"))
                        sheet = StringHelper.ReplaceSpecialChar(dr["Sheet"].ToString().Trim());

                    if (HandMakeOrderCols.Contains("器架名称"))
                        machineFrame = StringHelper.ReplaceSpecialChar(dr["器架名称"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("器架"))
                        machineFrame = StringHelper.ReplaceSpecialChar(dr["器架"].ToString().Trim());


                    if (HandMakeOrderCols.Contains("系列"))
                        category = StringHelper.ReplaceSpecialChar(dr["系列"].ToString().Trim());
                    if (HandMakeOrderCols.Contains("M/W"))
                        gender = StringHelper.ReplaceSpecialChar(dr["M/W"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("Gender"))
                        gender = StringHelper.ReplaceSpecialChar(dr["Gender"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("性别"))
                        gender = StringHelper.ReplaceSpecialChar(dr["性别"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("男女"))
                        gender = StringHelper.ReplaceSpecialChar(dr["男女"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("男/女"))
                        gender = StringHelper.ReplaceSpecialChar(dr["男/女"].ToString().Trim());
                    if (HandMakeOrderCols.Contains("POP数量"))
                        num = StringHelper.ReplaceSpecialChar(dr["POP数量"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("数量"))
                        num = StringHelper.ReplaceSpecialChar(dr["数量"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("Quantity"))
                        num = StringHelper.ReplaceSpecialChar(dr["Quantity"].ToString().Trim());
                    if (HandMakeOrderCols.Contains("POP宽"))
                        width = StringHelper.ReplaceSpecialChar(dr["POP宽"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("宽"))
                        width = StringHelper.ReplaceSpecialChar(dr["宽"].ToString().Trim());
                    if (HandMakeOrderCols.Contains("POP高"))
                        length = StringHelper.ReplaceSpecialChar(dr["POP高"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("高"))
                        length = StringHelper.ReplaceSpecialChar(dr["高"].ToString().Trim());
                    if (HandMakeOrderCols.Contains("POP材质"))
                        material = StringHelper.ReplaceSpecialChar(dr["POP材质"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("材质"))
                        material = StringHelper.ReplaceSpecialChar(dr["材质"].ToString().Trim());

                    //if (HandMakeOrderCols.Contains("材质单价"))
                    //    unitPrice = StringHelper.ReplaceSpecialChar(dr["材质单价"].ToString().Trim());
                    //else if (HandMakeOrderCols.Contains("单价"))
                    //    unitPrice = StringHelper.ReplaceSpecialChar(dr["单价"].ToString().Trim());

                    if (HandMakeOrderCols.Contains("选图"))
                        chooseImg = StringHelper.ReplaceSpecialChar(dr["选图"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("系列/选图"))
                        chooseImg = StringHelper.ReplaceSpecialChar(dr["系列/选图"].ToString().Trim());


                    if (HandMakeOrderCols.Contains("POP位置明细"))
                        positionDescription = StringHelper.ReplaceSpecialChar(dr["POP位置明细"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("pop位置明细"))
                        positionDescription = StringHelper.ReplaceSpecialChar(dr["pop位置明细"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("位置明细"))
                        positionDescription = StringHelper.ReplaceSpecialChar(dr["位置明细"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("位置描述"))
                        positionDescription = StringHelper.ReplaceSpecialChar(dr["位置描述"].ToString().Trim());



                    if (HandMakeOrderCols.Contains("备注"))
                        remark = StringHelper.ReplaceSpecialChar(dr["备注"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("其他备注"))
                        remark =StringHelper.ReplaceSpecialChar( dr["其他备注"].ToString().Trim());



                    if (HandMakeOrderCols.Contains("物料支持"))
                        materialSupport = StringHelper.ReplaceSpecialChar(dr["物料支持"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("物料支持级别"))
                        materialSupport = StringHelper.ReplaceSpecialChar(dr["物料支持级别"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("店铺级别"))
                        materialSupport = StringHelper.ReplaceSpecialChar(dr["店铺级别"].ToString().Trim());


                    if (HandMakeOrderCols.Contains("店铺规模大小"))
                        posScale = StringHelper.ReplaceSpecialChar(dr["店铺规模大小"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("店铺规模"))
                        posScale = StringHelper.ReplaceSpecialChar(dr["店铺规模"].ToString().Trim());
                    else if (HandMakeOrderCols.Contains("店铺大小"))
                        posScale = StringHelper.ReplaceSpecialChar(dr["店铺大小"].ToString().Trim());

                    if (HandMakeOrderCols.Contains("安装位置描述"))
                        installPositionDescription = StringHelper.ReplaceSpecialChar(dr["安装位置描述"].ToString().Trim());

                    bool canSave = true;
                    //decimal materialPrice = 0;
                    //if (string.IsNullOrWhiteSpace(orderType))
                    //{
                    //    canSave = false;
                    //    msg.Append("订单类型 为空；");
                    //}
                    //else if (!orderTypeDic.Keys.Contains(orderType.ToLower()))
                    //{
                    //    canSave = false;
                    //    msg.Append("订单类型填写不正确:必须为POP/道具/物料；");
                    //}
                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        msg.Append("店铺编号 为空；");
                    }

                    Shop shopFromDB = null;
                    if (!string.IsNullOrWhiteSpace(shopNo) && GetShopFromDB(shopNo, ref shopFromDB))
                    {
                        if (shopFromDB != null)
                        {
                            shopId = shopFromDB.Id;
                            if (!string.IsNullOrWhiteSpace(shopFromDB.RegionName) && shopFromDB.RegionName.ToLower() == "north" && guidanceType != (int)GuidanceTypeEnum.Promotion)//不是促销
                            {
                                if (shopFromDB.Format == null || shopFromDB.Format == "")
                                {
                                    canSave = false;
                                    msg.Append("店铺类型为空；");
                                }
                                if (shopFromDB.IsInstall == null || shopFromDB.IsInstall == "")
                                {
                                    canSave = false;
                                    msg.Append("安装级别为空；");
                                }
                            }
                        }
                        else
                        {
                            canSave = false;
                            msg.Append("店铺编号不存在；");
                        }
                    }
                    else
                    {
                        canSave = false;
                        msg.Append("店铺编号不存在；");
                    }

                    if (string.IsNullOrWhiteSpace(sheet))
                    {
                        canSave = false;
                        msg.Append("位置 为空；");
                    }
                    sheet = sheet == "户外" ? "OOH" : sheet;
                    if (sheet.ToLower() == "t-stand" || sheet.ToLower() == "tstand")
                    {
                        sheet = "中岛";
                    }
                    if (string.IsNullOrWhiteSpace(gender))
                    {
                        canSave = false;
                        msg.Append("性别 为空；");
                    }
                    if (string.IsNullOrWhiteSpace(num))
                    {
                        //canSave = false;
                        msg.Append("数量 为空:已自动改成1；");
                        num = "1";
                    }
                    else if (!StringHelper.IsIntVal(num))
                    {
                        //canSave = false;
                        msg.Append("数量填写不正确:已自动改成1；");
                        num = "1";
                    }
                    if (StringHelper.IsInt(num) == 0)
                    {
                        //canSave = false;
                        msg.Append("数量必为0:已自动改成1；");
                        num = "1";
                    }

                    if (string.IsNullOrWhiteSpace(width))
                    {
                        canSave = false;
                        msg.Append("POP宽不能空；");
                    }
                    else if (!StringHelper.IsDecimalVal(width))
                    {
                        canSave = false;
                        msg.Append("POP宽填写不正确；");
                    }
                    else if (StringHelper.IsDecimal(width) == 0)
                    {
                        canSave = false;
                        msg.Append("POP宽必须大于0；");
                    }
                    if (string.IsNullOrWhiteSpace(length))
                    {
                        canSave = false;
                        msg.Append("POP高不能空；");
                    }
                    else if (!StringHelper.IsDecimalVal(length))
                    {
                        canSave = false;
                        msg.Append("POP高填写不正确；");
                    }
                    else if (StringHelper.IsDecimal(length) == 0)
                    {
                        canSave = false;
                        msg.Append("POP高必须大于0；");
                    }
                    if (string.IsNullOrWhiteSpace(material))
                    {
                        canSave = false;
                        msg.Append("材质 为空；");
                    }
                    else if (!CheckMaterial(material))
                    {
                        canSave = false;
                        msg.Append("材质不存在；");
                    }
                    if (!string.IsNullOrWhiteSpace(machineFrame) && !string.IsNullOrWhiteSpace(gender) && !string.IsNullOrWhiteSpace(sheet))
                    {
                        if (!CheckShopMachineFrame(shopId, sheet, machineFrame, gender))
                        {
                            canSave = false;
                            msg.Append("此店铺不含该器架；");
                        }
                    }
                    decimal area = 0;
                    decimal width1 = 0;
                    decimal length1 = 0;
                    if (!string.IsNullOrWhiteSpace(width))
                        width1 = StringHelper.IsDecimal(width);
                    if (!string.IsNullOrWhiteSpace(length))
                        length1 = StringHelper.IsDecimal(length);
                    area = (width1 * length1) / 1000000;
                    bool IsHc = false;
                    string materialSupport1 = materialSupport;
                    if (shopFromDB != null && !string.IsNullOrWhiteSpace(shopFromDB.RegionName) && shopFromDB.RegionName.ToLower() == "north")
                    {
                        if (guidanceType == (int)GuidanceTypeEnum.Install)//安装活动
                        {
                            materialSupport1 = GetShopMaterialSupport(guidanceId, shopId);
                            if (string.IsNullOrWhiteSpace(materialSupport1))
                            {
                                materialSupport1 = materialSupport;
                                if (string.IsNullOrWhiteSpace(materialSupport1))
                                {
                                    canSave = false;
                                    msg.Append("物料支持 为空；");
                                }
                                else if (!materialSupportList.Contains(materialSupport1.ToUpper()))
                                {
                                    canSave = false;
                                    msg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                                }
                            }
                            else
                            {
                                if (shopFromDB != null && shopFromDB.IsInstall != null && shopFromDB.IsInstall.ToLower() == "y" && sheet.ToLower().Contains("ooh"))
                                {
                                    if (!CheckOOH(shopId, "", width1, length1))
                                    {
                                        canSave = false;
                                        msg.Append("该尺寸的POP不存在，请更新数据库；");
                                    }
                                }
                            }
                        }
                        //if (!string.IsNullOrWhiteSpace(shopFromDB.Format))
                        //{
                        ////非促销的活动要检查是不是HC店铺，如果是，就不导入Homecourt
                        //string format = shopFromDB.Format.ToUpper();
                        //if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE"))
                        //{
                        //        IsHc = true;
                        //        canSave = false;
                        //        msg.Append("HC店铺；");
                        //    }
                        //}
                        //if (!IsHc)
                        //{
                        //if (!CheckHandMakeOrder(shopId, sheet, width1, length1))
                        //{
                        //    canSave = false;
                        //    msg.Append("此POP不存在,请更新数据库；");
                        //}
                        //else
                        //{

                        //}

                        //}


                    }
                    if (canSave)
                    {

                        handOrderModel = new HandMadeOrderDetail();

                        handOrderModel.Area = area;
                        handOrderModel.ChooseImg = chooseImg;
                        handOrderModel.Gender = gender;
                        handOrderModel.GraphicLength = length1;
                        handOrderModel.GraphicMaterial = material;
                        handOrderModel.GraphicWidth = width1;
                        if (!string.IsNullOrWhiteSpace(levelNum))
                            handOrderModel.LevelNum = int.Parse(levelNum);
                        handOrderModel.OrderType = 1;
                        handOrderModel.PositionDescription = positionDescription;
                        handOrderModel.Quantity = int.Parse(num);
                        handOrderModel.Remark = remark;
                        handOrderModel.Sheet = sheet;
                        handOrderModel.ShopId = shopId;
                        handOrderModel.SubjectId = subjectId;
                        handOrderModel.MaterialSupport = materialSupport1;
                        handOrderModel.Category = category;
                        handOrderModel.POSScale = posScale;
                        handOrderModel.InstallPositionDescription = installPositionDescription;
                        handOrderModel.MachineFrame = machineFrame;
                        handOrderBll.Add(handOrderModel);
                        successNum++;
                    }
                    else
                    {
                        DataRow dr1 = HMPOPErrorTB.NewRow();
                        for (int i = 0; i < HandMakeOrderCols.Count; i++)
                        {
                            dr1["" + HandMakeOrderCols[i] + ""] = dr[HandMakeOrderCols[i]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        HMPOPErrorTB.Rows.Add(dr1);

                    }
                }

            }
        }

        //不使用了
        void ImportHnadMakeOrder_HC(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                HCOrderDetailBLL hcOrderBll = new HCOrderDetailBLL();
                //hcOrderBll.Delete(s => s.SubjectId == subjectId);
                HCOrderDetail hcOrderModel;
                HandMakeOrderCols = ds.Tables[0].Columns;
                HMHCErrorTB = CommonMethod.CreateErrorTB(HandMakeOrderCols);
                //Dictionary<string, int> orderTypeDic = new Dictionary<string, int>();
                //orderTypeDic.Add("pop", 1);
                //orderTypeDic.Add("道具", 2);
                //orderTypeDic.Add("物料", 3);
                //string orderType = string.Empty;
                string shopNo = string.Empty;
                string posScale = string.Empty;
                string materialSupport = string.Empty;
                int shopId = 0;
                string sheet = string.Empty;
                string machineFrame = string.Empty;
                string graphicNo = string.Empty;
                string levelNum = string.Empty;
                string gender = string.Empty;
                string num = string.Empty;
                string width = string.Empty;
                string length = string.Empty;
                string material = string.Empty;
                string chooseImg = string.Empty;
                string category = string.Empty;
                string positionDescription = string.Empty;
                string remark = string.Empty;
                string installPositionDescription = string.Empty;
                //bool isMaterialSupport = false;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StringBuilder msg = new StringBuilder();
                    //if (cols.Contains("订单类型"))
                    //{
                    //    orderType = dr["订单类型"].ToString().Trim();
                    //}
                    if (HandMakeOrderCols.Contains("店铺编号"))
                        shopNo = dr["店铺编号"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("POSCode"))
                        shopNo = dr["POSCode"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("POS Code"))
                        shopNo = dr["POS Code"].ToString().Trim();

                    if (HandMakeOrderCols.Contains("POP位置"))
                        sheet = dr["POP位置"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("位置"))
                        sheet = dr["位置"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("Sheet"))
                        sheet = dr["Sheet"].ToString().Trim();

                    if (HandMakeOrderCols.Contains("器架名称"))
                        machineFrame = dr["器架名称"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("器架"))
                        machineFrame = dr["器架"].ToString().Trim();
                    //if (cols.Contains("pop编号"))
                    //    graphicNo = dr["pop编号"].ToString().Trim();
                    //else if (cols.Contains("POP编号"))
                    //    graphicNo = dr["POP编号"].ToString().Trim();
                    //else if (cols.Contains("Graphic No"))
                    //    graphicNo = dr["Graphic No"].ToString().Trim();
                    //else if (cols.Contains("GraphicNo"))
                    //    graphicNo = dr["GraphicNo"].ToString().Trim();
                    //else if (cols.Contains("Graphic No#"))
                    //    graphicNo = dr["Graphic No#"].ToString().Trim();
                    //else if (cols.Contains("GraphicNo#"))
                    //    graphicNo = dr["GraphicNo#"].ToString().Trim();

                    if (HandMakeOrderCols.Contains("级别"))
                        levelNum = dr["级别"].ToString().Trim();
                    if (HandMakeOrderCols.Contains("M/W"))
                        gender = dr["M/W"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("Gender"))
                        gender = dr["Gender"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("性别"))
                        gender = dr["性别"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("男女"))
                        gender = dr["男女"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("男/女"))
                        gender = dr["男/女"].ToString().Trim();
                    if (HandMakeOrderCols.Contains("POP数量"))
                        num = dr["POP数量"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("数量"))
                        num = dr["数量"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("Quantity"))
                        num = dr["Quantity"].ToString().Trim();
                    if (HandMakeOrderCols.Contains("POP宽"))
                        width = dr["POP宽"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("宽"))
                        width = dr["宽"].ToString().Trim();
                    if (HandMakeOrderCols.Contains("POP高"))
                        length = dr["POP高"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("高"))
                        length = dr["高"].ToString().Trim();
                    if (HandMakeOrderCols.Contains("POP材质"))
                        material = dr["POP材质"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("材质"))
                        material = dr["材质"].ToString().Trim();

                    if (HandMakeOrderCols.Contains("选图"))
                        chooseImg = dr["选图"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("系列/选图"))
                        chooseImg = dr["系列/选图"].ToString().Trim();





                    //if (HandMakeOrderCols.Contains("安装位置描述"))
                    //    positionDescription = dr["安装位置描述"].ToString().Trim();
                    //else if (HandMakeOrderCols.Contains("位置描述"))
                    //    positionDescription = dr["位置描述"].ToString().Trim();

                    //if (HandMakeOrderCols.Contains("其他备注"))
                    //    remark = dr["其他备注"].ToString().Trim();
                    //else if (HandMakeOrderCols.Contains("备注"))
                    //    remark = dr["备注"].ToString().Trim();

                    //if (HandMakeOrderCols.Contains("店铺规模大小"))
                    //    posScale = dr["店铺规模大小"].ToString().Trim();
                    //if (HandMakeOrderCols.Contains("店铺规模"))
                    //    posScale = dr["店铺规模"].ToString().Trim();

                    //if (HandMakeOrderCols.Contains("物料支持"))
                    //    materialSupport = dr["物料支持"].ToString().Trim();
                    //else if (HandMakeOrderCols.Contains("物料支持级别"))
                    //    materialSupport = dr["物料支持级别"].ToString().Trim();

                    if (HandMakeOrderCols.Contains("POP位置明细"))
                        positionDescription = dr["POP位置明细"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("pop位置明细"))
                        positionDescription = dr["pop位置明细"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("位置明细"))
                        positionDescription = dr["位置明细"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("位置描述"))
                        positionDescription = dr["位置描述"].ToString().Trim();



                    if (HandMakeOrderCols.Contains("备注"))
                        remark = dr["备注"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("其他备注"))
                        remark = dr["其他备注"].ToString().Trim();



                    if (HandMakeOrderCols.Contains("物料支持"))
                        materialSupport = dr["物料支持"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("物料支持级别"))
                        materialSupport = dr["物料支持级别"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("店铺级别"))
                        materialSupport = dr["店铺级别"].ToString().Trim();


                    if (HandMakeOrderCols.Contains("店铺规模大小"))
                        posScale = dr["店铺规模大小"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("店铺规模"))
                        posScale = dr["店铺规模"].ToString().Trim();
                    else if (HandMakeOrderCols.Contains("店铺大小"))
                        posScale = dr["店铺大小"].ToString().Trim();

                    if (HandMakeOrderCols.Contains("安装位置描述"))
                        installPositionDescription = dr["安装位置描述"].ToString().Trim();


                    bool canSave = true;

                    if (string.IsNullOrWhiteSpace(shopNo))
                    {
                        canSave = false;
                        msg.Append("店铺编号 为空；");
                    }
                    Shop shopFromDB = null;
                    if (!string.IsNullOrWhiteSpace(shopNo) && GetShopFromDB(shopNo, ref shopFromDB))
                    {
                        if (shopFromDB != null)
                        {
                            shopId = shopFromDB.Id;
                            if (guidanceType != (int)GuidanceTypeEnum.Promotion)//不是促销
                            {
                                if (shopFromDB.Format == null || shopFromDB.Format == "")
                                {
                                    canSave = false;
                                    msg.Append("店铺类型为空；");
                                }
                                if (shopFromDB.IsInstall == null || shopFromDB.IsInstall == "")
                                {
                                    canSave = false;
                                    msg.Append("安装级别为空；");
                                }
                            }
                        }
                        else
                        {
                            canSave = false;
                            msg.Append("店铺编号不存在；");
                        }
                    }
                    else
                    {
                        canSave = false;
                        msg.Append("店铺编号不存在；");
                    }
                    //string addShopMsg = string.Empty;
                    //if (!string.IsNullOrWhiteSpace(shopNo) && !CheckHandMakeOrderShop(shopNo, dr, out shopId, out addShopMsg))
                    //{
                    //    canSave = false;
                    //    msg.Append("店铺不存在，添加失败：" + addShopMsg + "；");

                    //}
                    //if (string.IsNullOrWhiteSpace(materialSupport))
                    //{
                    //    canSave = false;
                    //    msg.Append("物料支持级别 为空；");
                    //}
                    if (guidanceType == (int)GuidanceTypeEnum.Install)//安装活动
                    {
                        //if (string.IsNullOrWhiteSpace(materialSupport))
                        //{
                        //    canSave = false;
                        //    msg.Append("物料支持 为空；");
                        //}
                        //else if (!materialSupportList.Contains(materialSupport.ToUpper()))
                        //{
                        //    canSave = false;
                        //    msg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                        //}
                    }
                    if (string.IsNullOrWhiteSpace(sheet))
                    {
                        canSave = false;
                        msg.Append("位置 为空；");
                    }

                    sheet = sheet == "户外" ? "OOH" : sheet;
                    if (sheet.ToLower() == "t-stand" || sheet.ToLower() == "tstand")
                    {
                        sheet = "中岛";
                    }

                    if (!string.IsNullOrWhiteSpace(levelNum) && !StringHelper.IsIntVal(levelNum))
                    {
                        canSave = false;
                        msg.Append("级别填写不正确；");
                    }
                    if (string.IsNullOrWhiteSpace(gender))
                    {
                        canSave = false;
                        msg.Append("性别 为空；");
                    }
                    if (string.IsNullOrWhiteSpace(num))
                    {
                        canSave = false;
                        msg.Append("数量 为空；");
                    }
                    else if (!StringHelper.IsIntVal(num))
                    {
                        canSave = false;
                        msg.Append("数量填写不正确；");
                    }

                    //if (!string.IsNullOrWhiteSpace(width) && !StringHelper.IsDecimalVal(width))
                    //{
                    //    canSave = false;
                    //    msg.Append("POP宽填写不正确；");
                    //}
                    //if (!string.IsNullOrWhiteSpace(length) && !StringHelper.IsDecimalVal(length))
                    //{
                    //    canSave = false;
                    //    msg.Append("POP高填写不正确；");
                    //}
                    if (string.IsNullOrWhiteSpace(width))
                    {
                        canSave = false;
                        msg.Append("POP宽不能空；");
                    }
                    else if (!StringHelper.IsDecimalVal(width))
                    {
                        canSave = false;
                        msg.Append("POP宽填写不正确；");
                    }
                    else if (StringHelper.IsDecimal(width) == 0)
                    {
                        canSave = false;
                        msg.Append("POP宽必须大于0；");
                    }
                    if (string.IsNullOrWhiteSpace(length))
                    {
                        canSave = false;
                        msg.Append("POP高不能空；");
                    }
                    else if (!StringHelper.IsDecimalVal(length))
                    {
                        canSave = false;
                        msg.Append("POP高填写不正确；");
                    }
                    else if (StringHelper.IsDecimal(length) == 0)
                    {
                        canSave = false;
                        msg.Append("POP高必须大于0；");
                    }
                    //decimal materialPrice = 0;
                    if (string.IsNullOrWhiteSpace(material))
                    {
                        canSave = false;
                        msg.Append("材质 为空；");
                    }
                    else if (!CheckMaterial(material))
                    {
                        canSave = false;
                        msg.Append("材质不存在；");
                    }
                    if (!string.IsNullOrWhiteSpace(machineFrame) && !string.IsNullOrWhiteSpace(gender) && !string.IsNullOrWhiteSpace(sheet))
                    {
                        if (!CheckShopMachineFrame(shopId, sheet, machineFrame, gender))
                        {
                            canSave = false;
                            msg.Append("此店铺不含该器架；");
                        }
                    }
                    //if (HCOrderIsExist(subjectId, shopId, sheet, gender, StringHelper.IsDecimal(width), StringHelper.IsDecimal(length)))
                    //{
                    //    canSave = false;
                    //    msg.Append("订单重复；");
                    //}
                    decimal area = 0;
                    decimal width1 = 0;
                    decimal length1 = 0;
                    if (!string.IsNullOrWhiteSpace(width))
                        width1 = StringHelper.IsDecimal(width);
                    if (!string.IsNullOrWhiteSpace(length))
                        length1 = StringHelper.IsDecimal(length);
                    area = (width1 * length1) / 1000000;
                    //if (guidanceType != (int)GuidanceTypeEnum.Promotion)
                    //{
                    if (!CheckHandMakeOrder(shopId, sheet, width1, length1))
                    {
                        canSave = false;
                        msg.Append("此POP不存在，请更新数据库；");
                    }
                    //}
                    if (canSave)
                    {

                        hcOrderModel = new HCOrderDetail();

                        hcOrderModel.Area = area;
                        hcOrderModel.ChooseImg = chooseImg;
                        hcOrderModel.Gender = gender;
                        hcOrderModel.GraphicLength = length1;
                        hcOrderModel.GraphicMaterial = material;
                        hcOrderModel.GraphicWidth = width1;
                        if (!string.IsNullOrWhiteSpace(levelNum))
                            hcOrderModel.LevelNum = int.Parse(levelNum);
                        hcOrderModel.OrderType = 1;
                        hcOrderModel.PositionDescription = positionDescription;
                        hcOrderModel.Quantity = int.Parse(num);
                        hcOrderModel.Remark = remark;
                        hcOrderModel.Sheet = sheet;
                        hcOrderModel.ShopId = shopId;
                        hcOrderModel.SubjectId = subjectId;
                        hcOrderModel.GraphicNo = graphicNo;
                        hcOrderModel.MaterialSupport = materialSupport;
                        hcOrderModel.POSScale = posScale;
                        hcOrderModel.Category = category;
                        hcOrderModel.InstallPositionDescription = installPositionDescription;
                        hcOrderModel.MachineFrame = machineFrame;
                        hcOrderBll.Add(hcOrderModel);
                        successNum++;
                    }
                    else
                    {

                        DataRow dr1 = HMHCErrorTB.NewRow();
                        for (int ii = 0; ii < HandMakeOrderCols.Count; ii++)
                        {
                            dr1["" + HandMakeOrderCols[ii] + ""] = dr[HandMakeOrderCols[ii]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        HMHCErrorTB.Rows.Add(dr1);
                    }

                }

            }
        }
        /// <summary>
        /// 提交费用订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            SubjectBLL subjectBll = new SubjectBLL();
            Models.Subject model = subjectBll.GetModel(subjectId);
            if (model != null)
            {
                model.Status = 4;//提交完成
                model.ApproveState = 0;
                subjectBll.Update(model);
                hfSubmitState.Value = "1";
                //Alert("提交成功！", "../SubjectList.aspx");

            }
            else
            {
                //Alert("提交失败！");
                hfSubmitState.Value = "0";
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindPriceOrder();
        }







        void LoadOrders()
        {
            int subjectType = !string.IsNullOrWhiteSpace(hfSubjectType.Value) ? int.Parse(hfSubjectType.Value) : 0;
            switch (subjectType)
            {

                case (int)SubjectTypeEnum.新开店安装费:
                case (int)SubjectTypeEnum.运费:
                    BindPriceOrder();
                    break;
                case (int)SubjectTypeEnum.手工订单:
                    BindHandMakeOrder_POP();
                    BindHandMakeOrder_HC();
                    BindHCMaterial();
                    BindHCPOPPriceOrder();
                    PanelHandMake.Visible = hasHMOrder > 0;
                    break;
                case (int)SubjectTypeEnum.POP订单:
                default:
                    LoadPOPOrder();
                    break;

            }
            //Panel2.Visible = false;
        }

        void LoadPOPOrder()
        {
            BindList();
            BindPOP();
            BindSupplement();
            BindMerge();
            BindMaterial();
            BindPOPPriceOrder();
            Panel_POP.Visible = true;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            LoadOrders();
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
            BindHandMakeOrder_POP();
        }

        protected void AspNetPagerHC_PageChanged(object sender, EventArgs e)
        {
            BindHandMakeOrder_HC();
        }

        protected void AspNetPagerHCMaterial_PageChanged(object sender, EventArgs e)
        {
            BindHCMaterial();
        }

        protected void AspNetPagerHCPriceOrder_PageChanged(object sender, EventArgs e)
        {
            BindHCPOPPriceOrder();
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
            BindHandMakeOrder_POP();
        }

        protected void btnSreachHC_Click(object sender, EventArgs e)
        {
            AspNetPagerHC.CurrentPageIndex = 1;
            BindHandMakeOrder_HC();
        }

        protected void btnSreachHCPriceOrder_Click(object sender, EventArgs e)
        {
            AspNetPagerHCPriceOrder.CurrentPageIndex = 1;
            BindHCPOPPriceOrder();
        }

        /// <summary>
        /// 提交手工订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmitHM_Click(object sender, EventArgs e)
        {
            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
            FinalOrderDetailTemp orderModel;
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    int materialPriceItemId = 0;
                    string subjectCategoryName = string.Empty;
                    //
                    orderBll.Delete(s => s.SubjectId == subjectId);
                    new QuoteOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                    //pop手工单
                    var handOrderList = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                                         join subject in CurrentContext.DbContext.Subject
                                         on order.SubjectId equals subject.Id
                                         join shop in CurrentContext.DbContext.Shop
                                         on order.ShopId equals shop.Id
                                         join guidance in CurrentContext.DbContext.SubjectGuidance
                                         on subject.GuidanceId equals guidance.ItemId
                                         join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                         on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                                         from subjectCategory in categoryTemp.DefaultIfEmpty()
                                         where order.SubjectId == subjectId
                                         select new
                                         {
                                             order,
                                             subject,
                                             shop,
                                             guidance.PriceItemId,
                                             subjectCategory
                                         }).ToList();
                    if (handOrderList.Any())
                    {

                        materialPriceItemId = handOrderList[0].PriceItemId ?? 0;
                        subjectCategoryName = handOrderList[0].subjectCategory != null ? handOrderList[0].subjectCategory.CategoryName : string.Empty;
                        string unitName = string.Empty;
                        handOrderList.ForEach(s =>
                        {

                            orderModel = new FinalOrderDetailTemp();
                            orderModel.AddDate = DateTime.Now;
                            orderModel.ChooseImg = s.order.ChooseImg;
                            orderModel.Gender = s.order.Gender;
                            orderModel.GraphicLength = s.order.GraphicLength;
                            orderModel.GraphicMaterial = s.order.GraphicMaterial;
                            orderModel.GraphicWidth = s.order.GraphicWidth;
                            orderModel.LevelNum = s.order.LevelNum;
                            orderModel.OrderType = s.order.OrderType;
                            orderModel.PositionDescription = s.order.PositionDescription;
                            orderModel.Quantity = s.order.Quantity;
                            orderModel.Remark = s.order.Remark;
                            orderModel.Sheet = s.order.Sheet;
                            orderModel.ShopId = s.order.ShopId;

                            orderModel.AgentCode = s.shop.AgentCode;
                            orderModel.AgentName = s.shop.AgentName;
                            orderModel.BusinessModel = s.shop.BusinessModel;
                            orderModel.Channel = s.shop.Channel;
                            orderModel.City = s.shop.CityName;
                            orderModel.CityTier = s.shop.CityTier;
                            orderModel.Contact = s.shop.Contact1;
                            orderModel.Format = s.shop.Format;
                            orderModel.IsInstall = s.shop.IsInstall;
                            orderModel.BCSIsInstall = s.shop.BCSIsInstall;
                            orderModel.LocationType = s.shop.LocationType;
                            orderModel.POPAddress = s.shop.POPAddress;
                            orderModel.Province = s.shop.ProvinceName;
                            orderModel.Region = s.shop.RegionName;
                            orderModel.ShopName = s.shop.ShopName;
                            orderModel.ShopNo = s.shop.ShopNo;


                            orderModel.SubjectId = subjectId;
                            orderModel.IsPOPMaterial = 1;
                            orderModel.Category = s.order.Category;
                            orderModel.MaterialSupport = s.order.MaterialSupport;
                            decimal unitPrice = 0;
                            decimal totalPrice = 0;
                            if (!string.IsNullOrWhiteSpace(s.order.GraphicMaterial))
                            {
                                POP pop = new POP();
                                pop.GraphicMaterial = s.order.GraphicMaterial;
                                pop.GraphicLength = s.order.GraphicLength;
                                pop.GraphicWidth = s.order.GraphicWidth;
                                pop.Quantity = s.order.Quantity;
                                pop.PriceItemId = materialPriceItemId;
                                unitPrice = GetMaterialUnitPrice(pop, out totalPrice, out unitName);
                            }
                            orderModel.UnitPrice = unitPrice;
                            orderModel.TotalPrice = totalPrice;
                            decimal width = s.order.GraphicWidth ?? 0;
                            decimal length = s.order.GraphicLength ?? 0;
                            orderModel.Area = (width * length) / 1000000;
                            orderModel.POSScale = s.order.POSScale;
                            orderModel.InstallPricePOSScale = s.order.POSScale;
                            orderModel.InstallPriceMaterialSupport = s.order.MaterialSupport;
                            //if (!string.IsNullOrWhiteSpace(s.shop.Format) && (s.shop.Format.ToLower().Contains("kids") || s.shop.Format.ToLower().Contains("infant")))
                            if (subjectCategoryName == "童店")
                                orderModel.InstallPriceMaterialSupport = "Others";
                            orderModel.InstallPositionDescription = s.order.InstallPositionDescription;
                            orderModel.MachineFrame = s.order.MachineFrame;
                            orderModel.ShopStatus = s.shop.Status;
                            orderModel.GuidanceId = s.subject.GuidanceId;
                            orderModel.CSUserId = s.shop.CSUserId;
                            orderBll.Add(orderModel);
                            //保存报价订单
                            //orderModel.UnitName = unitName;
                            //new BasePage().SaveQuotationOrder(orderModel);

                        });
                    }
                    //HC订单
                    //var list = (from order in CurrentContext.DbContext.HCOrderDetail
                    //            join shop in CurrentContext.DbContext.Shop
                    //            on order.ShopId equals shop.Id
                    //            join subject in CurrentContext.DbContext.Subject
                    //            on order.SubjectId equals subject.Id
                    //            join guidance in CurrentContext.DbContext.SubjectGuidance
                    //            on subject.GuidanceId equals guidance.ItemId
                    //            join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                    //            on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                    //            from subjectCategory in categoryTemp.DefaultIfEmpty()
                    //            where order.SubjectId == subjectId
                    //            select new
                    //            {

                    //                order,
                    //                shop,
                    //                guidance.PriceItemId,
                    //                subjectCategory
                    //            }).ToList();
                    //if (list.Any())
                    //{

                    //    materialPriceItemId = list[0].PriceItemId ?? 0;
                    //    subjectCategoryName = list[0].subjectCategory != null ? list[0].subjectCategory.CategoryName : string.Empty;
                    //    list.ForEach(o =>
                    //    {

                    //        orderModel = new FinalOrderDetailTemp();
                    //        orderModel.AgentCode = o.shop.AgentCode;
                    //        orderModel.AgentName = o.shop.AgentName;
                    //        orderModel.BusinessModel = o.shop.BusinessModel;
                    //        orderModel.Channel = o.shop.Channel;
                    //        orderModel.City = o.shop.CityName;
                    //        orderModel.CityTier = o.shop.CityTier;
                    //        orderModel.Contact = o.shop.Contact1;
                    //        orderModel.Format = o.shop.Format;
                    //        orderModel.Gender = o.order.Gender;
                    //        orderModel.GraphicNo = o.order.GraphicNo;
                    //        orderModel.IsInstall = o.shop.IsInstall;
                    //        orderModel.BCSIsInstall = o.shop.BCSIsInstall;
                    //        orderModel.LocationType = o.shop.LocationType;
                    //        orderModel.MaterialSupport = o.order.MaterialSupport;
                    //        orderModel.OrderType = o.order.OrderType;
                    //        orderModel.POPAddress = o.shop.POPAddress;

                    //        orderModel.Province = o.shop.ProvinceName;
                    //        orderModel.Quantity = o.order.Quantity;
                    //        orderModel.Region = o.shop.RegionName;
                    //        orderModel.ChooseImg = o.order.ChooseImg;
                    //        //finalOrderTempModel.Remark = o.order.Remark;
                    //        orderModel.Remark = o.order.Remark;
                    //        orderModel.Sheet = o.order.Sheet;
                    //        orderModel.ShopId = o.shop.Id;
                    //        orderModel.ShopName = o.shop.ShopName;
                    //        orderModel.ShopNo = o.shop.ShopNo;
                    //        orderModel.SubjectId = subjectId;
                    //        orderModel.Tel = o.shop.Tel1;
                    //        orderModel.MachineFrame = o.order.MachineFrame;
                    //        orderModel.LevelNum = o.order.LevelNum;
                    //        decimal width = o.order.GraphicWidth ?? 0;
                    //        decimal length = o.order.GraphicLength ?? 0;
                    //        orderModel.GraphicLength = length;
                    //        orderModel.GraphicMaterial = o.order.GraphicMaterial;
                    //        decimal unitPrice = 0;
                    //        decimal totalPrice = 0;
                    //        if (!string.IsNullOrWhiteSpace(o.order.GraphicMaterial))
                    //        {
                    //            POP pop = new POP();
                    //            pop.GraphicMaterial = o.order.GraphicMaterial;
                    //            pop.GraphicLength = o.order.GraphicLength;
                    //            pop.GraphicWidth = o.order.GraphicWidth;
                    //            pop.Quantity = o.order.Quantity;
                    //            pop.PriceItemId = materialPriceItemId;
                    //            unitPrice = GetMaterialUnitPrice(pop, out totalPrice);
                    //        }
                    //        orderModel.UnitPrice = unitPrice;
                    //        orderModel.TotalPrice = totalPrice;

                    //        orderModel.GraphicWidth = width;
                    //        orderModel.PositionDescription = o.order.PositionDescription;
                    //        orderModel.Area = (width * length) / 1000000;
                    //        orderModel.Category = o.order.Category;
                    //        orderModel.IsHC = true;
                    //        orderModel.POSScale = o.order.POSScale;
                    //        orderModel.InstallPricePOSScale = o.order.POSScale;
                    //        orderModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                    //        //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                    //        if (subjectCategoryName == "童店")
                    //            orderModel.InstallPriceMaterialSupport = "Others";
                    //        orderModel.InstallPositionDescription = o.order.InstallPositionDescription;
                    //        orderModel.ShopStatus = o.shop.Status;
                    //        orderModel.CSUserId = o.shop.CSUserId;
                    //        orderBll.Add(orderModel);

                    //    });
                    //}

                    var materialOrderList = (from material in CurrentContext.DbContext.OrderMaterial
                                             join shop in CurrentContext.DbContext.Shop
                                             on material.ShopId equals shop.Id
                                             join subject in CurrentContext.DbContext.Subject
                                             on material.SubjectId equals subject.Id
                                             where material.SubjectId == subjectId
                                             select new
                                             {
                                                 subject,
                                                 material,
                                                 shop
                                             }).ToList();
                    if (materialOrderList.Any())
                    {
                        materialOrderList.ForEach(o =>
                        {

                            orderModel = new FinalOrderDetailTemp();
                            orderModel.AddDate = DateTime.Now;
                            orderModel.AgentCode = o.shop.AgentCode;
                            orderModel.AgentName = o.shop.AgentName;
                            orderModel.BusinessModel = o.shop.BusinessModel;
                            orderModel.Channel = o.shop.Channel;
                            orderModel.City = o.shop.CityName;
                            orderModel.CityTier = o.shop.CityTier;
                            orderModel.Contact = o.shop.Contact1;
                            orderModel.Format = o.shop.Format;
                            //finalOrderTempModel.Gender = o.order.Gender;
                            //finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                            orderModel.IsInstall = o.shop.IsInstall;
                            orderModel.BCSIsInstall = o.shop.BCSIsInstall;
                            orderModel.LocationType = o.shop.LocationType;
                            //finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                            orderModel.OrderType = (int)OrderTypeEnum.物料;
                            orderModel.POPAddress = o.shop.POPAddress;
                            //finalOrderTempModel.POSScale = o.order.POSScale;
                            //finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                            //finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                            //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                            //if (subjectCategoryName == "童店")
                            //finalOrderTempModel.InstallPriceMaterialSupport = "Others";
                            orderModel.Province = o.shop.ProvinceName;
                            orderModel.Quantity = o.material.MaterialCount;
                            orderModel.Region = o.shop.RegionName;
                            //finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                            orderModel.Sheet = o.material.MaterialName;
                            StringBuilder size = new StringBuilder();
                            if (o.material.MaterialLength != null && o.material.MaterialLength > 0 && o.material.MaterialWidth != null && o.material.MaterialWidth > 0)
                            {
                                size.AppendFormat("({0}*{1}", o.material.MaterialLength, o.material.MaterialWidth);
                                if (o.material.MaterialHigh != null && o.material.MaterialHigh > 0)
                                    size.AppendFormat("*{0}", o.material.MaterialHigh);
                                size.Append(")");
                            }
                            orderModel.PositionDescription = size.ToString();
                            orderModel.Remark = o.material.Remark;
                            orderModel.ShopId = o.shop.Id;
                            orderModel.ShopName = o.shop.ShopName;
                            orderModel.ShopNo = o.shop.ShopNo;
                            orderModel.SubjectId = subjectId;
                            orderModel.Tel = o.shop.Tel1;
                            orderModel.MachineFrame = "";
                            if (o.subject.SubjectType == (int)SubjectTypeEnum.正常单)
                            {
                                orderModel.IsFromRegion = true;
                            }
                            orderModel.ShopStatus = o.shop.Status;
                            orderModel.GuidanceId = o.subject.GuidanceId;
                            if ((o.material.Price ?? 0) > 0)
                            {
                                orderModel.UnitPrice = o.material.Price;
                                orderModel.OrderPrice = (o.material.Price ?? 0) * (o.material.MaterialCount ?? 0);
                                orderModel.PayOrderPrice = (o.material.PayPrice ?? 0) * (o.material.MaterialCount ?? 0);
                            }
                            orderModel.CSUserId = o.shop.CSUserId;
                            orderBll.Add(orderModel);
                            //new BasePage().SaveQuotationOrder(orderModel,false);
                        });
                    }



                    SubjectBLL subjectBll = new SubjectBLL();
                    Subject subjectModel = subjectBll.GetModel(subjectId);
                    bool flag = false;
                    if (subjectModel != null)
                    {
                        subjectModel.Status = 4;//提交完成
                        subjectModel.ApproveState = 0;
                        subjectBll.Update(subjectModel);
                        flag = true;

                    }

                    if (flag)
                    {
                        tran.Complete();
                        hfSubmitState.Value = "1";

                    }
                    else
                    {
                        hfSubmitState.Value = "0";
                        //Alert("提交失败！");
                    }
                }
                catch (Exception ex)
                {
                    hfSubmitState.Value = "0";
                }
            }

        }



        /// <summary>
        /// 导入手工单的时候，店铺不存在直接添加到系统
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        int AddShop(DataRow dr, out string msg)
        {
            msg = string.Empty;
            int shopId = 0;
            int provinceId = 0;
            int cityId = 0;
            int countyId = 0;
            string shopNo = string.Empty;
            string shopName = string.Empty;
            string region = string.Empty;
            string province = string.Empty;
            string city = string.Empty;
            string county = string.Empty;
            string cityLevel = string.Empty;
            string isInstall = string.Empty;
            string angentCode = string.Empty;
            string angentName = string.Empty;
            string adress = string.Empty;
            string contact1 = string.Empty;
            string tel1 = string.Empty;
            string contact2 = string.Empty;
            string tel2 = string.Empty;
            string channel = string.Empty;
            string format = string.Empty;
            string locationType = string.Empty;
            string customerModel = string.Empty;
            string MaterialSupport = string.Empty;
            bool canSave = true;
            StringBuilder errorMsg = new StringBuilder();
            if (HandMakeOrderCols.Contains("POS Code"))
                shopNo = dr["POS Code"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("POSCode"))
                shopNo = dr["POSCode"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("店铺编号"))
                shopNo = dr["店铺编号"].ToString().Trim();
            if (HandMakeOrderCols.Contains("POS Name"))
                shopName = dr["POS Name"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("POSName"))
                shopName = dr["POSName"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("店铺名称"))
                shopName = dr["店铺名称"].ToString().Trim();
            if (HandMakeOrderCols.Contains("Region"))
                region = dr["Region"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("区域"))
                region = dr["区域"].ToString().Trim();
            if (HandMakeOrderCols.Contains("Province"))
                province = dr["Province"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("省份"))
                province = dr["省份"].ToString().Trim();
            if (HandMakeOrderCols.Contains("City"))
                city = dr["City"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("城市"))
                city = dr["城市"].ToString().Trim();

            if (HandMakeOrderCols.Contains("县/区"))
                county = dr["县/区"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("区"))
                county = dr["区"].ToString().Trim();

            if (HandMakeOrderCols.Contains("City Tier"))
                cityLevel = dr["City Tier"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("CityTier"))
                cityLevel = dr["CityTier"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("城市级别"))
                cityLevel = dr["城市级别"].ToString().Trim();
            if (HandMakeOrderCols.Contains("是否安装"))
                isInstall = dr["是否安装"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("IsInstall"))
                isInstall = dr["IsInstall"].ToString().Trim();

            if (HandMakeOrderCols.Contains("Customer Code"))
                angentCode = dr["Customer Code"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("CustomerCode"))
                angentCode = dr["CustomerCode"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("经销商编号"))
                angentCode = dr["经销商编号"].ToString().Trim();

            if (HandMakeOrderCols.Contains("Customer Name"))
                angentName = dr["Customer Name"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("CustomerName"))
                angentName = dr["CustomerName"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("经销商名称"))
                angentName = dr["经销商名称"].ToString().Trim();
            if (HandMakeOrderCols.Contains("POPAddress"))
                adress = dr["POPAddress"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("店铺地址"))
                adress = dr["店铺地址"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("Address"))
                adress = dr["Address"].ToString().Trim();

            if (HandMakeOrderCols.Contains("popContact1"))
                contact1 = dr["popContact1"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("Contact1"))
                contact1 = dr["Contact1"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("Contact"))
                contact1 = dr["Contact"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("联系人1"))
                contact1 = dr["联系人1"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("联系人"))
                contact1 = dr["联系人"].ToString().Trim();

            if (HandMakeOrderCols.Contains("popTel1"))
                tel1 = dr["popTel1"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("Tel1"))
                tel1 = dr["Tel1"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("Tel"))
                tel1 = dr["Tel"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("联系电话1"))
                tel1 = dr["联系电话1"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("联系电话"))
                tel1 = dr["联系电话"].ToString().Trim();

            if (HandMakeOrderCols.Contains("popContact2"))
                contact2 = dr["popContact2"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("Contact2"))
                contact2 = dr["Contact2"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("联系人2"))
                contact2 = dr["联系人2"].ToString().Trim();

            if (HandMakeOrderCols.Contains("popTel2"))
                tel2 = dr["popTel2"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("Tel2"))
                tel2 = dr["Tel2"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("联系电话2"))
                tel2 = dr["联系电话2"].ToString().Trim();

            if (HandMakeOrderCols.Contains("Channel"))
                channel = dr["Channel"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("产品类型"))
                channel = dr["产品类型"].ToString().Trim();

            if (HandMakeOrderCols.Contains("Format"))
                format = dr["Format"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("店铺类型"))
                format = dr["店铺类型"].ToString().Trim();

            if (HandMakeOrderCols.Contains("店铺级别"))
                MaterialSupport = dr["店铺级别"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("物料支持级别"))
                MaterialSupport = dr["物料支持级别"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("物料支持"))
                MaterialSupport = dr["物料支持"].ToString().Trim();
            if (HandMakeOrderCols.Contains("Location Type"))
                locationType = dr["Location Type"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("LocationType"))
                locationType = dr["LocationType"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("店铺属性"))
                locationType = dr["店铺属性"].ToString().Trim();

            if (HandMakeOrderCols.Contains("Business Model"))
                customerModel = dr["Business Model"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("BusinessModel"))
                customerModel = dr["BusinessModel"].ToString().Trim();
            else if (HandMakeOrderCols.Contains("客户类别"))
                customerModel = dr["客户类别"].ToString().Trim();
            if (string.IsNullOrWhiteSpace(shopName))
            {
                canSave = false;
                errorMsg.Append("店铺名称为空 ；");
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                canSave = false;
                errorMsg.Append("区域为空 ；");
            }
            if (!string.IsNullOrWhiteSpace(province))
            {
                provinceId = GetProvinceId(province);
                if (provinceId > 0)
                {
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        cityId = GetCityId(city, provinceId);
                        if (cityId > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(county))
                            {
                                countyId = GetCityId(county, cityId);
                                if (countyId == 0)
                                {
                                    //canSave = false;
                                    //errorMsg.Append("区县填写填写不正确 ；");
                                }
                            }
                        }
                        else
                        {
                            string newCity = string.Empty;
                            GetCity(city, out newCity, out cityId, out countyId);
                            if (cityId == 0)
                            {
                                //canSave = false;
                                //errorMsg.Append("城市填写填写不正确 ；");
                            }
                            else
                            {
                                county = city;
                                city = newCity;
                            }
                        }
                    }
                }
                else
                {
                    canSave = false;
                    errorMsg.Append("省份填写填写不正确 ；");
                }
            }
            if (string.IsNullOrWhiteSpace(cityLevel))
            {
                //canSave = false;
                //errorMsg.Append("城市级别为空 ；");
            }
            if (string.IsNullOrWhiteSpace(isInstall))
            {
                //canSave = false;
                //errorMsg.Append("是否安装为空 ；");
            }
            if (string.IsNullOrWhiteSpace(adress))
            {
                //canSave = false;
                //errorMsg.Append("店铺地址为空 ；");
            }
            if (string.IsNullOrWhiteSpace(format))
            {
                //canSave = false;
                //errorMsg.Append("店铺类型为空 ；");
            }
            if (canSave)
            {
                Shop shopModel = new Models.Shop();
                if (provinceId > 0)
                    shopModel.ProvinceId = provinceId;
                if (cityId > 0)
                    shopModel.CityId = cityId;
                if (countyId > 0)
                    shopModel.AreaId = countyId;
                shopModel.RegionName = region;
                shopModel.ProvinceName = province;
                shopModel.CityName = city;
                shopModel.AreaName = county;
                shopModel.AgentCode = angentCode;
                shopModel.AgentName = angentName;
                shopModel.BusinessModel = customerModel;
                shopModel.Channel = channel;
                shopModel.CityTier = cityLevel;
                shopModel.Contact1 = contact1;
                shopModel.Tel1 = tel1;
                shopModel.Contact2 = contact2;
                shopModel.Tel2 = tel2;
                shopModel.CustomerId = !string.IsNullOrWhiteSpace(hfCustomerId.Value) ? int.Parse(hfCustomerId.Value) : 0;
                shopModel.Format = format.Replace("（", "(").Replace("）", ")"); ;
                shopModel.IsInstall = isInstall;
                shopModel.LocationType = locationType;
                shopModel.POPAddress = adress;
                shopModel.ShopName = shopName;
                shopModel.AddDate = DateTime.Now;
                shopModel.ShopNo = shopNo.ToUpper();
                shopModel.IsDelete = false;
                shopModel.MaterialSupport = MaterialSupport;
                shopModel.Status = "正常";
                shopBll.Add(shopModel);
                shopId = shopModel.Id;
            }
            else
                msg = errorMsg.ToString();
            return shopId;
        }


        List<Models.Place> placeList = new List<Models.Place>();
        //RegionBLL regionBll = new RegionBLL();
        PlaceBLL placeBll = new PlaceBLL();

        /// <summary>
        /// 省份ID
        /// </summary>
        /// <param name="provinceName"></param>
        /// <returns></returns>
        int GetProvinceId(string provinceName)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => (s.PlaceName == provinceName || s.PlaceName == provinceName + "省") && s.ParentID == 0).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }
        /// <summary>
        /// 城市ID 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        int GetCityId(string cityName, int provinceId)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => s.PlaceName == cityName && s.ParentID == provinceId).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }

        /// <summary>
        /// 县ID 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        int GetCountyId(string countyName, int cityId)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => s.PlaceName == countyName && s.ParentID == cityId).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }

        /// <summary>
        /// 通过县名称获取城市Id
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="cityId"></param>
        /// <param name="areaId"></param>
        void GetCity(string areaName, out string cityName, out int cityId, out int areaId)
        {
            cityId = 0; areaId = 0;
            cityName = string.Empty;
            Models.Place placeModel = placeBll.GetList(s => s.PlaceName == areaName).FirstOrDefault();
            if (placeModel != null)
            {
                cityId = placeModel.ParentID ?? 0;
                areaId = placeModel.ID;
                var model1 = placeBll.GetModel(cityId);
                if (model1 != null)
                    cityName = model1.PlaceName;
            }
        }

        /// <summary>
        /// 检查该位置是不是已经被其他项目占用
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="graphicNo"></param>
        /// <returns></returns>
        /// 
        int guidanceId = 0;
        bool POPPlaceIsFree(int shopId, string sheet, string graphicNo, out string subjectName)
        {
            //bool flag = true;
            subjectName = string.Empty;
            if (guidanceId == 0)
            {
                Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                if (subjectModel != null && subjectModel.GuidanceId != null)
                {
                    guidanceId = subjectModel.GuidanceId ?? 0;
                }
            }
            var orderList = from order in CurrentContext.DbContext.POPOrderDetail
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where subject.GuidanceId == guidanceId
                            && order.ShopId == shopId
                            && order.Sheet.ToUpper() == sheet
                            && (order.GraphicNo != null && order.GraphicNo != "" && order.GraphicNo.ToUpper() == graphicNo)
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            select subject;
            if (orderList.Any())
            {

                var subjectNameList = orderList.Select(s => s.SubjectName).Distinct().ToList();
                subjectName = StringHelper.ListToString(subjectNameList);
                return false;
            }
            else
                return true;

        }



        OrderMaterialMppingBLL OrderMaterialMppingBll = new OrderMaterialMppingBLL();
        List<string> materialStrList = new List<string>();
        bool CheckMaterial(string materialName)
        {

            bool flag = false;
            if (materialStrList.Contains(materialName.ToLower()))
            {
                flag = true;
            }
            else
            {
                int customerId = 0;
                if (!string.IsNullOrWhiteSpace(hfCustomerId.Value))
                    customerId = StringHelper.IsInt(hfCustomerId.Value);
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    var model = OrderMaterialMppingBll.GetList(s => s.OrderMaterialName.ToLower() == materialName.ToLower()).FirstOrDefault();
                    if (model != null)
                    {
                        flag = true;
                        materialStrList.Add(materialName.ToLower());
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
            return flag;
        }
    }

    /// <summary>
    /// 导入订单模板类
    /// </summary>
    public class ImportTables
    {
        public string TabName { get; set; }
        public DataSet Data { get; set; }
    }
}
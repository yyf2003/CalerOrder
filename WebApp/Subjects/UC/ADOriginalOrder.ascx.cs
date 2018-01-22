using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using System.Data;
using Common;
using Models;

namespace WebApp.Subjects.UC
{
    public partial class ADOriginalOrder : System.Web.UI.UserControl
    {
        public int subjectId;
        public int isShow = 0;
        public int subjectType = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }


            if (!IsPostBack)
            {
                Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                if (subjectModel != null)
                {
                    subjectType = subjectModel.SubjectType ?? 1;
                    if (subjectType == 1)
                    {
                        BindList();
                        BindPOP();
                        BindSupplement();
                        BindMerge();
                        BindMaterial();
                        BindHCOrderDetail();
                        Panel1.Visible = isShow > 0;
                        Panel2.Visible = false;
                    }
                    else
                    {
                        BindPriceOrderDeatil();
                        Panel1.Visible = false;
                        Panel2.Visible = isShow > 0;
                    }
                }

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
            string shopNo = txtShopNo3.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                supplementOrder = supplementOrder.Where(s => s.shop.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPager3.RecordCount = supplementOrder.Count;
            this.AspNetPager3.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager3.CurrentPageIndex, this.AspNetPager3.PageCount, this.AspNetPager3.RecordCount, this.AspNetPager3.PageSize });
            gvSupplement.DataSource = supplementOrder.OrderBy(s => s.list.ShopId).Skip((AspNetPager3.CurrentPageIndex - 1) * AspNetPager3.PageSize).Take(AspNetPager3.PageSize).ToList();

            gvSupplement.DataBind();
            isShow += supplementOrder.Any() ? 1 : isShow;




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
            string shopNo = txtShopNo1.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                listOrder = listOrder.Where(s => s.shop.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPager1.RecordCount = listOrder.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = listOrder.OrderBy(s => s.list.ShopId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();

            gvList.DataBind();
            isShow += listOrder.Any() ? 1 : isShow;
        }

        void BindPOP()
        {
            var popOrder = (from order in CurrentContext.DbContext.POPOrderDetail
                            join pop in CurrentContext.DbContext.POP
                                //on order.GraphicNo equals pop.GraphicNo
                            on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop.ShopId, pop.GraphicNo, pop.Sheet }
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            //where order.ShopId == pop.ShopId && order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                            where order.SubjectId == subjectId && (order.IsDelete == null || order.IsDelete == false)
                            select new
                            {
                                order,

                                pop,
                                shop

                            }).ToList();
            string shopNo = txtShopNo2.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                popOrder = popOrder.Where(s => s.shop.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPager2.RecordCount = popOrder.Count;
            this.AspNetPager2.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager2.CurrentPageIndex, this.AspNetPager2.PageCount, this.AspNetPager2.RecordCount, this.AspNetPager2.PageSize });
            gvPOP.DataSource = popOrder.OrderBy(s => s.order.ShopId).Skip((AspNetPager2.CurrentPageIndex - 1) * AspNetPager2.PageSize).Take(AspNetPager2.PageSize).ToList();

            gvPOP.DataBind();
            isShow += popOrder.Any() ? 1 : isShow;
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
                                 //GraphicNo = pop != null ? pop.GraphicNo : "",
                                 //POPName = pop != null ? pop.POPName : "",
                                 //POPType = pop != null ? pop.POPType : "",
                                 //PositionDescription = pop != null ? pop.PositionDescription : "",
                                 //WindowWide = pop != null && pop.WindowWide != null ? pop.WindowWide.ToString() : "",
                                 //WindowHigh = pop != null && pop.WindowHigh != null ? pop.WindowHigh.ToString() : "",
                                 //WindowDeep = pop != null && pop.WindowDeep != null ? pop.WindowDeep.ToString() : "",
                                 //WindowSize = pop != null ? pop.WindowSize : "",
                                 //GraphicWidth = pop != null && pop.GraphicWidth != null ? pop.GraphicWidth.ToString() : "",
                                 //GraphicLength = pop != null && pop.GraphicLength != null ? pop.GraphicLength.ToString() : "",
                                 //Area = pop != null && pop.Area != null ? pop.Area.ToString() : "",
                                 //GraphicMaterial = pop != null ? pop.GraphicMaterial : "",
                                 //Style = pop != null ? pop.Style : "",
                                 //CornerType = pop != null ? pop.CornerType : merge.CornerType,
                                 //Category = pop != null ? pop.Category : "",
                                 //StandardDimension = pop != null ? pop.StandardDimension : "",
                                 //Modula = pop != null ? pop.Modula : "",
                                 //Frame = pop != null ? pop.Frame : "",
                                 //DoubleFace = pop != null ? pop.DoubleFace : "",
                                 //Glass = pop != null ? pop.Glass : "",
                                 //Backdrop = pop != null ? pop.Backdrop : "",
                                 //ModulaQuantityWidth = pop != null && pop.ModulaQuantityWidth != null ? pop.ModulaQuantityWidth.ToString() : "",
                                 //ModulaQuantityHeight = pop != null && pop.ModulaQuantityHeight != null ? pop.ModulaQuantityHeight.ToString() : "",
                                 //PlatformLength = pop != null && pop.PlatformLength != null ? pop.PlatformLength.ToString() : "",
                                 //PlatformWidth = pop != null && pop.PlatformWidth != null ? pop.PlatformWidth.ToString() : "",
                                 //PlatformHeight = pop != null && pop.PlatformHeight != null ? pop.PlatformHeight.ToString() : "",
                                 //FixtureType = pop != null ? pop.FixtureType : "",
                                 //Gender = pop != null ? pop.Gender : "",

                             }).ToList();
            string shopNo = txtShopNo4.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                MergeList = MergeList.Where(s => s.shop.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPager4.RecordCount = MergeList.Count;
            this.AspNetPager4.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager4.CurrentPageIndex, this.AspNetPager4.PageCount, this.AspNetPager4.RecordCount, this.AspNetPager4.PageSize });
            gvMerge.DataSource = MergeList.OrderBy(s => s.merge.ShopId).Skip((AspNetPager4.CurrentPageIndex - 1) * AspNetPager4.PageSize).Take(AspNetPager4.PageSize).ToList();

            gvMerge.DataBind();
            if (!MergeList.Any())
            {
                Button1.Visible = false;
            }

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
            string shopNo = txtShopNo5.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                materialList = materialList.Where(s => s.shop.ShopNo.Contains(shopNo)).ToList();
            }
            AspNetPager5.RecordCount = materialList.Count;
            this.AspNetPager5.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager5.CurrentPageIndex, this.AspNetPager5.PageCount, this.AspNetPager5.RecordCount, this.AspNetPager5.PageSize });
            gvMaterial.DataSource = materialList.OrderBy(s => s.material.ShopId).Skip((AspNetPager5.CurrentPageIndex - 1) * AspNetPager5.PageSize).Take(AspNetPager5.PageSize).ToList();

            gvMaterial.DataBind();
        }

        void BindPriceOrderDeatil()
        {
            var list = (from order in CurrentContext.DbContext.PriceOrderDetail
                        //join shop in CurrentContext.DbContext.Shop
                        //on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            //shop
                        }).ToList();
            AspNetPager6.RecordCount = list.Count;
            this.AspNetPager6.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager6.CurrentPageIndex, this.AspNetPager6.PageCount, this.AspNetPager6.RecordCount, this.AspNetPager6.PageSize });
            repeater_List.DataSource = list.OrderBy(s => s.order.Id).Skip((AspNetPager6.CurrentPageIndex - 1) * AspNetPager6.PageSize).Take(AspNetPager6.PageSize).ToList();
            repeater_List.DataBind();
            isShow += list.Any() ? 1 : isShow;
        }

        void BindHCOrderDetail()
        {
            try
            {
                var list = (from order in CurrentContext.DbContext.HCOrderDetail
                            join pop in CurrentContext.DbContext.POP
                            on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop.ShopId, pop.GraphicNo, pop.Sheet }
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            where order.SubjectId == subjectId
                            select new
                            {
                                order,
                                pop,
                                shop
                            }).ToList();
                string shopNo = txtShopNo7.Text.Trim();
                if (!string.IsNullOrWhiteSpace(shopNo))
                {
                    list = list.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
                }
                AspNetPager7.RecordCount = list.Count;
                this.AspNetPager7.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager7.CurrentPageIndex, this.AspNetPager7.PageCount, this.AspNetPager7.RecordCount, this.AspNetPager7.PageSize });
                gvHC.DataSource = list.OrderBy(s => s.order.ShopId).Skip((AspNetPager7.CurrentPageIndex - 1) * AspNetPager7.PageSize).Take(AspNetPager7.PageSize).ToList();

                gvHC.DataBind();
            }
            catch (Exception ex)
            {

            }


        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindList();
        }
        protected void AspNetPager2_PageChanged(object sender, EventArgs e)
        {
            BindPOP();
        }
        protected void AspNetPager3_PageChanged(object sender, EventArgs e)
        {
            BindSupplement();
        }
        protected void AspNetPager4_PageChanged(object sender, EventArgs e)
        {
            BindMerge();
        }
        protected void AspNetPager5_PageChanged(object sender, EventArgs e)
        {
            BindMaterial();
        }
        protected void AspNetPager6_PageChanged(object sender, EventArgs e)
        {
            BindPriceOrderDeatil();
        }
        
        /// <summary>
        /// list订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreach1_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindList();
        }

        /// <summary>
        /// pop订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreach2_Click(object sender, EventArgs e)
        {
            AspNetPager2.CurrentPageIndex = 1;
            BindPOP();
        }

        /// <summary>
        /// 补充订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreach3_Click(object sender, EventArgs e)
        {
            AspNetPager3.CurrentPageIndex = 1;
            BindSupplement();
        }

        /// <summary>
        /// 合并订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreach4_Click(object sender, EventArgs e)
        {
            AspNetPager4.CurrentPageIndex = 1;
            BindMerge();
        }

        /// <summary>
        /// 搜索物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSreach5_Click(object sender, EventArgs e)
        {
            AspNetPager5.CurrentPageIndex = 1;
            BindMaterial();
        }

        
        /// <summary>
        /// 导出list订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport1_Click(object sender, EventArgs e)
        {
            DataSet ds = new ListOrderDetailBLL().GetOrderList(subjectId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "List订单");

            }
        }

        /// <summary>
        /// 导出pop订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport2_Click(object sender, EventArgs e)
        {
            DataSet ds = new POPOrderDetailBLL().GetOrderList(subjectId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "POP订单");

            }
        }

        /// <summary>
        /// 导出补充订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport3_Click(object sender, EventArgs e)
        {
            DataSet ds = new SupplementOrderDetailBLL().GetOrderList(subjectId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "补充订单");

            }
        }


        /// <summary>
        /// 导出合并订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            DataSet ds = new MergeOriginalOrderBLL().GetOrderList(subjectId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "合并订单");

            }
        }

        /// <summary>
        /// 导出物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button3_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 删除合并订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //MergeOriginalOrderBLL mergeBll = new MergeOriginalOrderBLL();
            //List<int> idList = new List<int>();
            //foreach (RepeaterItem item in gvMerge.Items)
            //{
            //    if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
            //    {
            //        CheckBox cb = (CheckBox)item.FindControl("cbOne");
            //        if (cb.Checked)
            //        {
            //            HiddenField hfId = (HiddenField)item.FindControl("hfMergeId");
            //            int id = int.Parse(hfId.Value);
            //            idList.Add(id);

            //        }
            //    }
            //}
            //if (idList.Any())
            //{
            //    idList.ForEach(id =>
            //    {
            //        MergeOriginalOrder model = mergeBll.GetModel(id);
            //        if (model != null)
            //        {
            //            //model.IsDelete = true;
            //            mergeBll.Delete(model);
            //        }
            //    });
            //    BindMerge();
            //}
            //else
            //{
            //    new BasePage().Alert("请选择要删除的订单");
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objLevel = item.GetType().GetProperty("LevelNum").GetValue(item, null);
                    object objSheet = item.GetType().GetProperty("Sheet").GetValue(item, null);
                    string level = objLevel != null ? objLevel.ToString() : "0";
                    //if (objSheet != null && objSheet.ToString().Contains("桌"))
                    //{

                    //    ((Label)e.Item.FindControl("labLevel")).Text = CommonMethod.GeEnumName<TableLevelEnum>(level);
                    //}
                    if (level != "0")
                    {
                        ((Label)e.Item.FindControl("labLevel")).Text = CommonMethod.GeEnumName<LevelNumEnum>(level);
                    }
                }
            }
        }



        protected void gvMerge_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objLevel = item.GetType().GetProperty("LevelNum").GetValue(item, null);
                    object objSheet = item.GetType().GetProperty("Sheet").GetValue(item, null);
                    string level = objLevel != null ? objLevel.ToString() : "0";
                    //if (objSheet != null && objSheet.ToString().Contains("桌"))
                    //{
                    //    string level = objLevel != null ? objLevel.ToString() : "1";
                    //    ((Label)e.Item.FindControl("labLevel")).Text = CommonMethod.GeEnumName<TableLevelEnum>(level);
                    //}
                    if (level != "0")
                    {
                        ((Label)e.Item.FindControl("labLevel")).Text = CommonMethod.GeEnumName<LevelNumEnum>(level);
                    }
                }
            }
        }

        

        /// <summary>
        /// HC订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
    }
}
using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;

namespace WebApp.Subjects.ADErrorCorrection
{
    public partial class ErrorCorrection : BasePage
    {
        //int subjectId;
        public int itemId;
        public int customerId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemId"] != null)
            {
                itemId = int.Parse(Request.QueryString["itemId"]);
            }
            if (!IsPostBack)
            {
                //BindData();
            }
        }

        void BindData()
        {

            //var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //           join shop in CurrentContext.DbContext.Shop
            //           on order.ShopId equals shop.Id
            //           join pop1 in CurrentContext.DbContext.POP
            //           on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
            //           from pop in popTemp.DefaultIfEmpty()
            //           where order.SubjectId == subjectId && shop.CSUserId==CurrentUser.UserId
            //           select new
            //           {
            //               order,
            //               LevelNum = order.LevelNum,
            //               Sheet = order.Sheet,
            //               pop
            //           }).ToList();
            //if (!string.IsNullOrWhiteSpace(txtShopName.Text))
            //{
            //    list = list.Where(s => s.order.ShopName.Contains(txtShopName.Text)).ToList();
            //}
            //if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            //{
            //    list = list.Where(s => s.order.ShopNo.ToLower() == txtShopNo.Text.ToLower()).ToList();
            //}
            //AspNetPager1.RecordCount = list.Count;
            //this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            //gvPOP.DataSource = list.OrderBy(s => s.order.ShopId).ThenBy(s=>s.order.Sheet).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            //gvPOP.DataBind();
        }

        void GetPOPList()
        {
            labMsg.Text = "";
            int shopId = 0;
            string shopNo = txtShopNo.Text.Trim();
            if (CheckShopNo(shopNo, out shopId))
            {
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            //join shop in CurrentContext.DbContext.Shop
                            //on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where subject.GuidanceId == itemId && order.ShopId == shopId
                            //order.SubjectId == subjectId && shop.CSUserId == CurrentUser.UserId
                            select new
                            {
                                order,
                                LevelNum = order.LevelNum,
                                Sheet = order.Sheet,
                                subject
                            }).ToList();
                if (list.Any())
                {
                    customerId = list[0].subject.CustomerId??0;
                }
                AspNetPager1.RecordCount = list.Count;
                this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
                gvPOP.DataSource = list.OrderBy(s => s.order.ShopId).ThenBy(s => s.order.Sheet).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
                gvPOP.DataBind();
                hfShopId.Value = shopId.ToString();
            }
            else
            {
                
                labMsg.Text = "店铺编号不存在";
            }
        }

        ShopBLL shopBll = new ShopBLL();
        bool CheckShopNo(string shopNo, out int shopId)
        {
            bool flag = false;
            shopId = 0;
            var model = shopBll.GetList(s => s.ShopNo.ToLower() == shopNo.ToLower() && (s.IsDelete==null || s.IsDelete==false)).FirstOrDefault();
            if (model != null)
            {
                shopId = model.Id;
                flag = true;
            }
            return flag;
        }
        

        protected void Button1_Click(object sender, EventArgs e)
        {
            GetPOPList();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            GetPOPList();
        }

        protected void gvPOP_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }
    }
}
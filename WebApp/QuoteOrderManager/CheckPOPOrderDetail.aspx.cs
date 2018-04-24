using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;

namespace WebApp.QuoteOrderManager
{
    public partial class CheckPOPOrderDetail : BasePage
    {
        string sheet = string.Empty;
        //string guidanceId = string.Empty;
        //string subjectCategoryId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["sheet"] != null)
            {
                sheet = Request.QueryString["sheet"];
            }
            //if (Request.QueryString["guidanceId"] != null)
            //{
            //    guidanceId = Request.QueryString["guidanceId"];
            //}
            //if (Request.QueryString["subjectCategoryId"] != null)
            //{
            //    subjectCategoryId = Request.QueryString["subjectCategoryId"];
            //}
            if (!IsPostBack) {
                labSheet.Text = sheet;
                BindData();
            }
        }

        void BindData()
        {
            List<QuoteOrderDetail> popOrderList = new List<QuoteOrderDetail>();
            if (!string.IsNullOrWhiteSpace(sheet) && Session[sheet] != null)
            {
                popOrderList = Session[sheet] as List<QuoteOrderDetail>;
                //if (popOrderList.Any())
                //{
                //    popOrderList = popOrderList.OrderBy(s => s.ShopId).ThenBy(s => s.Sheet).ToList();
                //}
            }
            var list = (from order in popOrderList
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on order.GuidanceId equals guidance.ItemId
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        select new
                        {
                            order,
                            guidance.ItemName,
                            subject.SubjectName,
                            Gender = !string.IsNullOrWhiteSpace(order.OrderGender)?order.OrderGender:order.Gender
                        }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            CheckPOPOrderRepeater.DataSource = list.OrderBy(s => s.order.ShopId).ThenBy(s => s.order.Sheet).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            CheckPOPOrderRepeater.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
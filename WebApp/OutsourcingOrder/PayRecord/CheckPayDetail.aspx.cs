using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Models;
using DAL;
using BLL;
using Common;
using System.Text;

namespace WebApp.OutsourcingOrder.PayRecord
{
    public partial class CheckPayDetail : BasePage
    {
        public int guidanceId;
        public int outsourceId;
        public int currUserId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["outsourceId"] != null)
            {
                outsourceId = int.Parse(Request.QueryString["outsourceId"]);
            }
            currUserId = CurrentUser.UserId;
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            Company companyModel = new CompanyBLL().GetModel(outsourceId);
            if (companyModel != null)
            {
                labOutsourceName.Text = companyModel.CompanyName;
            }
            SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(guidanceId);
            if (guidanceModel != null)
            {
                labGuidanceName.Text = guidanceModel.ItemName;
            }
            var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                            where order.GuidanceId == guidanceId
                            && order.OutsourceId == outsourceId
                            && (order.IsDelete == null || order.IsDelete == false)
                            select order).ToList();
            if (orderList.Any())
            {

                decimal totalPrice = 0;
                var popList = orderList.Where(order => order.OrderType == (int)OrderTypeEnum.POP).ToList();
                popList.ForEach(order =>
                {
                    totalPrice += (order.TotalPrice ?? 0);
                });
                var priceOrderList = orderList.Where(order => order.OrderType != (int)OrderTypeEnum.POP).ToList();
                priceOrderList.ForEach(order =>
                {
                    totalPrice += ((order.Quantity ?? 1) * (order.PayOrderPrice ?? 0));
                });

                if (totalPrice > 0)
                {
                    totalPrice = Math.Round(totalPrice, 2);
                }
                labShouldPay.Text = totalPrice.ToString();
                decimal pay = 0;
                var payRecordList = new OutsourcePayRecordBLL().GetList(s => s.GuidanceId == guidanceId && s.OutsourceId == outsourceId);
                if (payRecordList.Any())
                {
                    pay = payRecordList.Sum(s => s.PayAmount ?? 0);
                    
                }
                labPay.Text = pay.ToString();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
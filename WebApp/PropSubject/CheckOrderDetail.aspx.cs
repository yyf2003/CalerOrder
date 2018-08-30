using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using Common;
using BLL;
using Models;
using System.Text;

namespace WebApp.PropSubject
{
    public partial class CheckOrderDetail : BasePage
    {
        int subjectId;
        int isCheck = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["isCheck"] != null)
            {
                isCheck = int.Parse(Request.QueryString["isCheck"]);
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindOrder();
            }
            if (isCheck == 0)
            {
                GetApproveInfo();
                Panel_Container1.Visible = true;
            }
        }

        void BindSubject()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                         join guidance in CurrentContext.DbContext.SubjectGuidance
                         on subject.GuidanceId equals guidance.ItemId
                         join customer in CurrentContext.DbContext.Customer
                         on subject.CustomerId equals customer.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on subject.AddUserId equals user.UserId
                         where subject.Id == subjectId
                         select new
                         {
                             subject,
                             customer.CustomerName,
                             user.RealName,
                             guidance.ItemName

                         }).FirstOrDefault();
            if (model != null)
            {
                labGuidanceName.Text = model.ItemName;
                labSubjectNo.Text = model.subject.SubjectNo;
                labSubjectName.Text = model.subject.SubjectName;
                labAddUserName.Text = model.RealName;
                labCustomerName.Text = model.CustomerName;
                int subjectType = model.subject.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());

            }

        }

        void BindOrder()
        {
            List<PropOrderDetail> orderList = new List<PropOrderDetail>();
            var list = (from iOrder in CurrentContext.DbContext.PropOrderDetail
                        join oOrder1 in CurrentContext.DbContext.PropOutsourceOrderDetail
                        on iOrder.Id equals oOrder1.PropOrderId into temp
                        from oOrder in temp.DefaultIfEmpty()
                        where iOrder.SubjectId == subjectId
                        select new
                        {
                            iOrder,
                            //IQuantity=iOrder.Quantity??1,
                            //IUnitPrice = iOrder.UnitPrice ?? 0,
                            //OQuantity = oOrder.Quantity ?? 1,
                            //OUnitPrice = oOrder.UnitPrice ?? 0,
                            oOrder
                        }).OrderBy(s => s.iOrder.Id).ToList();
            if (list.Any())
            {
                int orderId = 0;
                int index = 0;
                foreach (var item in list)
                {
                    PropOrderDetail model = new PropOrderDetail();
                    if (index == 0)
                    {
                        orderId = item.iOrder.Id;
                        model.Id = item.iOrder.Id;
                        model.BOMCost = item.iOrder.BOMCost;
                        model.Dimension = item.iOrder.Dimension;
                        model.MaterialName = item.iOrder.MaterialName;
                        model.MaterialType = item.iOrder.MaterialType;
                        model.Packaging = item.iOrder.Packaging;
                        model.PackingCost = item.iOrder.PackingCost;
                        model.ProcessingCost = item.iOrder.ProcessingCost;
                        model.Quantity = item.iOrder.Quantity;
                        model.ServiceType = item.iOrder.ServiceType;
                        model.Sheet = item.iOrder.Sheet;
                        model.SubjectId = item.iOrder.SubjectId;
                        model.TransportationCost = item.iOrder.TransportationCost;
                        model.UnitName = item.iOrder.UnitName;
                        model.UnitPrice = item.iOrder.UnitPrice;
                    }
                    else
                    {
                        if (orderId == item.iOrder.Id)
                        {
                            model.Id = 0;
                            model.MaterialName = item.oOrder.MaterialName;

                        }
                        else
                        {

                            orderId = item.iOrder.Id;
                            model.Id = item.iOrder.Id;
                            model.BOMCost = item.iOrder.BOMCost;
                            model.Dimension = item.iOrder.Dimension;
                            model.MaterialName = item.iOrder.MaterialName;
                            model.MaterialType = item.iOrder.MaterialType;
                            model.Packaging = item.iOrder.Packaging;
                            model.PackingCost = item.iOrder.PackingCost;
                            model.ProcessingCost = item.iOrder.ProcessingCost;
                            model.Quantity = item.iOrder.Quantity;
                            model.ServiceType = item.iOrder.ServiceType;
                            model.Sheet = item.iOrder.Sheet;
                            model.SubjectId = item.iOrder.SubjectId;
                            model.TransportationCost = item.iOrder.TransportationCost;
                            model.UnitName = item.iOrder.UnitName;
                            model.UnitPrice = item.iOrder.UnitPrice;
                        }
                    }
                    if (item.oOrder != null)
                    {
                        model.PropOrderId = item.oOrder.PropOrderId;
                        model.OutsourceName = item.oOrder.OutsourceName;
                        model.PayMaterialName = item.oOrder.MaterialName;
                        model.PayPackaging = item.oOrder.Packaging;
                        model.PayQuantity = item.oOrder.Quantity;
                        model.PayRemark = item.oOrder.Remark;
                        model.PayUnitName = item.oOrder.UnitName;
                        model.PayUnitPrice = item.oOrder.UnitPrice;
                    }
                    orderList.Add(model);
                    index++;
                }

            }

            Repeater1.DataSource = orderList;
            Repeater1.DataBind();

        }

        void GetApproveInfo()
        {
            Dictionary<int, string> approveStateDic = new Dictionary<int, string>();
            approveStateDic.Add(0, "待审批");
            approveStateDic.Add(1, "审批通过");
            approveStateDic.Add(2, "审批不通过");
            var list = (from approve in CurrentContext.DbContext.ApproveInfo
                        join user in CurrentContext.DbContext.UserInfo
                        on approve.AddUserId equals user.UserId
                        where approve.SubjectId == subjectId
                        select new
                        {
                            approve,
                            user.UserName,
                        }).ToList();
            if (list.Any())
            {
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
                Panel1.Visible = true;
            }
        }

        decimal ITotalPrice = 0;
        decimal OTotalPrice = 0;
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                Object item = e.Item.DataItem;
                if (item != null)
                {
                    object iQuantityObj = item.GetType().GetProperty("Quantity").GetValue(item, null);
                    object oQuantityObj = item.GetType().GetProperty("PayQuantity").GetValue(item, null);
                    object iUnitPriceObj = item.GetType().GetProperty("UnitPrice").GetValue(item, null);
                    object oUnitPriceObj = item.GetType().GetProperty("PayUnitPrice").GetValue(item, null);

                    int iQuantity = iQuantityObj != null ? int.Parse(iQuantityObj.ToString()) : 1;
                    int oQuantity = oQuantityObj != null ? int.Parse(oQuantityObj.ToString()) : 1;

                    decimal iUnitPrice = iUnitPriceObj != null ? decimal.Parse(iUnitPriceObj.ToString()) : 0;
                    decimal oUnitPrice = oUnitPriceObj != null ? decimal.Parse(oUnitPriceObj.ToString()) : 0;

                    decimal iSub = iQuantity * iUnitPrice;
                    decimal oSub = oQuantity * oUnitPrice;

                    if (iSub > 0)
                    {
                        ITotalPrice += iSub;
                        ((Label)e.Item.FindControl("labReceiveSub")).Text = Math.Round(iSub,2).ToString();
                    }
                    if (oSub > 0)
                    {
                        OTotalPrice += oSub;
                        ((Label)e.Item.FindControl("labPaySub")).Text = Math.Round(oSub, 2).ToString();
                    }
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                if (ITotalPrice > 0)
                {
                    ((Label)e.Item.FindControl("labReceiveTotal")).Text = Math.Round(ITotalPrice, 2).ToString();
                }
                if (OTotalPrice > 0)
                {
                    ((Label)e.Item.FindControl("labPayTotal")).Text = Math.Round(OTotalPrice, 2).ToString();
                }
            }
        }
    }
}
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

namespace WebApp.PropSubject
{
    public partial class Approve : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindOrder();
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
                        join oOrder in CurrentContext.DbContext.PropOutsourceOrderDetail
                        on iOrder.Id equals oOrder.PropOrderId
                        where iOrder.SubjectId == subjectId
                        select new
                        {
                            iOrder,
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
                    model.PropOrderId = item.oOrder.PropOrderId;
                    model.OutsourceName = item.oOrder.OutsourceName;
                    model.PayMaterialName = item.oOrder.MaterialName;
                    model.PayPackaging = item.oOrder.Packaging;
                    model.PayQuantity = item.oOrder.Quantity;
                    model.PayRemark = item.oOrder.Remark;
                    model.PayUnitName = item.oOrder.UnitName;
                    model.PayUnitPrice = item.oOrder.UnitPrice;

                    orderList.Add(model);
                    index++;
                }
               
            }
            
            Repeater1.DataSource = orderList;
            Repeater1.DataBind();
           
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text.Trim();
            SubjectBLL subjectBll = new SubjectBLL();
            bool isApproveOk = true;
            string msg = "ok";
            try
            {
                Models.Subject model = subjectBll.GetModel(subjectId);
                if (result == 1)
                {
                    OrderChangeApplicationDetailBLL changeApplicationBll = new OrderChangeApplicationDetailBLL();
                    OrderChangeApplicationDetail changeModel = changeApplicationBll.GetList(s => s.SubjectId == subjectId && s.State == 1).FirstOrDefault();
                    if (changeModel != null)
                    {
                        changeModel.State = 2;
                        changeModel.FinishDate = DateTime.Now;
                        changeModel.FinishUserId = CurrentUser.UserId;
                        changeApplicationBll.Update(changeModel);
                    }

                }
                model.ApproveState = result;
                model.ApproveDate = DateTime.Now;
                model.ApproveUserId = CurrentUser.UserId;
                subjectBll.Update(model);
                if (!string.IsNullOrWhiteSpace(remark))
                {
                    remark = remark.Replace("\r\n", "<br/>");
                }
                ApproveInfo approveModel = new ApproveInfo() { AddDate = DateTime.Now, AddUserId = CurrentUser.UserId, Remark = remark, Result = result, SubjectId = subjectId };
                new ApproveInfoBLL().Add(approveModel);

            }
            catch (Exception ex)
            {
                isApproveOk = false;
                msg = ex.Message;
            }
            if (isApproveOk)
            {
                ExcuteJs("ApproveStae", msg, "/Subjects/ApproveList.aspx");
            }
            else
            {
                ExcuteJs("ApproveStae", msg, "");
            }
        }
    }
}
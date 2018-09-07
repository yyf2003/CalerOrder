using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using Common;
using DAL;

namespace WebApp.Subjects.RegionSubject
{
    public partial class AddNewShopOrder : BasePage
    {
        public int SubjectId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                SubjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindSubject();
            }
        }

        void BindSubject()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                         join customer in CurrentContext.DbContext.Customer
                         on subject.CustomerId equals customer.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on subject.AddUserId equals user.UserId
                         join outsource1 in CurrentContext.DbContext.Company
                         on subject.OutsourceId equals outsource1.Id into temp
                         from outsource in temp.DefaultIfEmpty()
                         where subject.Id == SubjectId
                         select new
                         {
                             subject,
                             customer.CustomerName,
                             user.RealName,
                             OutsourceName = outsource.CompanyName
                         }).FirstOrDefault();
            if (model != null)
            {
                labSubjectNo.Text = model.subject.SubjectNo;
                labSubjectName.Text = model.subject.SubjectName;
                labAddUserName.Text = model.RealName;
                labCustomerName.Text = model.CustomerName;
                int subjectType = model.subject.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                labRegion.Text = model.subject.SupplementRegion;
                labProduceOutsource.Text = model.OutsourceName;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SubjectBLL subjectBll = new SubjectBLL();
            Subject model = subjectBll.GetModel(SubjectId);
            if (model != null)
            {
                bool submitIsValid = false;
                RegionOrderDetailBLL orderBll = new RegionOrderDetailBLL();
                RegionOrderDetail orderModel;

                RegionOrderPriceBLL priceOrderBll = new RegionOrderPriceBLL();

                var orderList = orderBll.GetList(s => s.SubjectId == SubjectId);
                if (orderList.Any())
                {
                    orderList.ForEach(s =>
                    {
                        orderModel = s;
                        orderModel.IsSubmit = 1;
                        orderModel.ApproveState = 0;
                        orderBll.Update(orderModel);
                    });
                    model.Status = 4;
                    model.ApproveState = 0;
                    subjectBll.Update(model);
                    //submitIsValid = true;
                }
                var priceOrderList = priceOrderBll.GetList(s => s.SubjectId == SubjectId);
                if (priceOrderList.Any())
                {
                    RegionOrderPrice priceModel;
                    priceOrderList.ForEach(s =>
                    {
                        priceModel = s;
                        priceModel.IsSubmit = 1;
                        priceModel.ApproveState = 0;
                        priceOrderBll.Update(priceModel);
                    });
                    model.Status = 4;
                    model.ApproveState = 0;
                    subjectBll.Update(model);
                    //submitIsValid = true;
                }
               
                Alert("提交成功", "List.aspx");
            }
            else
                Alert("提交失败！");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("AddSubject.aspx?subjectId={0}", SubjectId),false);
        }
    }
}
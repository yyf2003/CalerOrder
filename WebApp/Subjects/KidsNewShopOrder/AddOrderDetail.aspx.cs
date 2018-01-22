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

namespace WebApp.Subjects.KidsNewShopOrder
{
    public partial class AddOrderDetail : BasePage
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
                         where subject.Id == SubjectId
                         select new
                         {
                             subject,
                             customer.CustomerName,
                             user.RealName

                         }).FirstOrDefault();
            if (model != null)
            {
                labSubjectNo.Text = model.subject.SubjectNo;
                labSubjectName.Text = model.subject.SubjectName;
                labAddUserName.Text = model.RealName;
                labCustomerName.Text = model.CustomerName;
                int subjectType = model.subject.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                labRegion.Text = !string.IsNullOrWhiteSpace(model.subject.PriceBlongRegion) ? model.subject.PriceBlongRegion : "默认";

            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SubjectBLL subjectBll = new SubjectBLL();
            Subject model = subjectBll.GetModel(SubjectId);
            if (model != null)
            {

                HandMadeOrderDetailBLL orderBll = new HandMadeOrderDetailBLL();
                HandMadeOrderDetail orderModel;
                var orderList = orderBll.GetList(s => s.SubjectId == SubjectId && (s.IsSubmit == null || s.IsSubmit == 0));
                if (orderList.Any())
                {
                    orderList.ForEach(s =>
                    {
                        orderModel = s;
                        orderModel.IsSubmit = 1;
                        orderBll.Update(orderModel);
                    });
                    model.Status = 4;
                    model.ApproveState = 0;
                    subjectBll.Update(model);
                }
                else
                {
                    model.ApproveState = 1;
                    subjectBll.Update(model);
                }
                Alert("提交成功", "/Subjects/SubjectList.aspx");
            }
            else
                Alert("提交失败！");
        }
    }
}
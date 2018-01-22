using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;

namespace WebApp.Statistics
{
    public partial class SubjectDetail : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
                BindData();
        }

        void BindData()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                         join guidance in CurrentContext.DbContext.SubjectGuidance
                         on subject.GuidanceId equals guidance.ItemId
                         join user in CurrentContext.DbContext.UserInfo
                         on subject.AddUserId equals user.UserId
                         join customer in CurrentContext.DbContext.Customer
                         on subject.CustomerId equals customer.Id
                         join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                         on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                         from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                         where subject.Id == subjectId
                         select new
                         {
                             subject,
                             guidance.ItemName,
                             user.RealName,
                             customer.CustomerName,
                             subjectCategory.CategoryName
                         }).FirstOrDefault();
            if (model != null)
            {
                labGuidanceName.Text = model.ItemName;
                labSubjectName.Text = model.subject.SubjectName;
                labSubjectNo.Text = model.subject.SubjectNo;
                int subjectType = model.subject.SubjectType ?? 1;
                labOrderType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());
                labAddUserName.Text = model.RealName;
                labCustomerName.Text = model.CustomerName;
                labRegion.Text = !string.IsNullOrWhiteSpace(model.subject.SupplementRegion) ? model.subject.SupplementRegion : !string.IsNullOrWhiteSpace(model.subject.PriceBlongRegion) ? model.subject.PriceBlongRegion : "默认";
                labRemark.Text = model.subject.Remark;
                labSubjectCategory.Text = model.CategoryName;
                if (model.subject.AddDate != null)
                    labAddDate.Text = model.subject.AddDate.ToString();

            }
        }
    }
}
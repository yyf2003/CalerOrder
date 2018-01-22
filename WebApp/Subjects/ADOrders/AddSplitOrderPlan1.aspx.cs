using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;

namespace WebApp.Subjects.ADOrders
{
    public partial class AddSplitOrderPlan1 : BasePage
    {
        public int customerId;
        public int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);

            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);

            }
            if (!IsPostBack)
            {
                var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                    join customer in CurrentContext.DbContext.Customer
                                    on subject.CustomerId equals customer.Id
                                    join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                   on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                                    from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                                    where subject.Id == subjectId
                                    select new
                                    {
                                        subject.SubjectName,
                                        subject.SubjectNo,
                                        customer.CustomerName,
                                        customer.Id,
                                        subjectCategory.CategoryName
                                    }).FirstOrDefault();
                if (subjectModel != null)
                {
                    labSubjectName.Text = subjectModel.SubjectName;
                    labSubjectNo.Text = subjectModel.SubjectNo;
                    labCustomer.Text = subjectModel.CustomerName;
                    hfSubjectCategoryName.Value = subjectModel.CategoryName;
                }
            }
        }
    }
}
using System;
using System.Data;
using System.Linq;
using Common;
using DAL;

namespace WebApp.Subjects.ADOrders
{
    public partial class AddSplitOrderPlan : BasePage
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
                                    where subject.Id == subjectId
                                    select new
                                    {
                                        subject.SubjectName,
                                        subject.SubjectNo,
                                        customer.CustomerName,
                                        customer.Id
                                    }).FirstOrDefault();
                if (subjectModel != null)
                {
                    labSubjectName.Text = subjectModel.SubjectName;
                    labSubjectNo.Text = subjectModel.SubjectNo;
                    labCustomer.Text = subjectModel.CustomerName;
                    
                }
            }
        }

        protected void lbDownLoadNotSplit_Click(object sender, EventArgs e)
        {
            if (Session["exportTb"] != null)
            {
                DataTable dt = (DataTable)Session["exportTb"];
                OperateFile.ExportExcel(dt, "不符合拆单条件的订单");
            }
        }
    }
}
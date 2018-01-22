using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;

namespace WebApp.Subjects.ADOrders
{
    public partial class SplitOrder : BasePage
    {
        int subjectId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
                
            }
            if (!IsPostBack)
            {
                hfSubjectId.Value = subjectId.ToString();
                BindSubject();
                Session["frameOrderlist"] = null;
                Session["framelist"] = null;
                Session["shoplist"] = null;
                Session["orderlist"] = null;
                Session["poplist"] = null;
                Session["frames"] = null;
            }
        }

        void BindSubject()
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
                hfCustomerId.Value = subjectModel.Id.ToString();
            }
        }

        //protected void lbGoSkip_Click(object sender, EventArgs e)
        //{
           
        //}

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ImportOrder.aspx?subjectId=" + subjectId, false);
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            Response.Redirect("SplitOrderSuccess.aspx?subjectId=" + subjectId, false);
        }
    }
}
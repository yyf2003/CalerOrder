using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using BLL;
using Models;
using DAL;

namespace WebApp.OutsourcingOrder.PriceOrder
{
    public partial class Add : BasePage
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
                BindCustomerList(ddlCustomer);
                BindOutsource();
                BindOrderType();
                if (subjectId > 0)
                    BindData();
            }
        }

        void BindData()
        {
            OutsourcePriceOrder model = new OutsourcePriceOrderBLL().GetModel(subjectId);
            if (model != null)
            {
                if (model.CustomerId != null)
                    ddlCustomer.SelectedValue = model.CustomerId.ToString();
                if (model.GuidanceYear != null && model.GuidanceMonth != null)
                {
                    txtMonth.Text = model.GuidanceYear + "-" + model.GuidanceMonth;
                }
                txtSubjectName.Text = model.SubjectName;
                if (model.OutsourceId != null)
                    ddlOutsource.SelectedValue = model.OutsourceId.ToString();
                if (model.OrderType != null)
                    ddlOrderType.SelectedValue = model.OrderType.ToString();
                if (model.PayPrice != null)
                    txtPrice.Text = model.PayPrice.ToString();
                txtRemark.Text = model.Remark;

            }
        }

        void BindOutsource()
        {
            var list = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource);
            ddlOutsource.DataSource = list;
            ddlOutsource.DataTextField = "CompanyName";
            ddlOutsource.DataValueField = "Id";
            ddlOutsource.DataBind();
            ddlOutsource.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        void BindOrderType()
        {
            var list = CommonMethod.GetEnumList<OrderTypeEnum>().Where(s => s.Desction == "费用订单").ToList();
            ddlOrderType.DataSource = list;
            ddlOrderType.DataTextField = "Name";
            ddlOrderType.DataValueField = "Value";
            ddlOrderType.DataBind();
            ddlOrderType.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("List.aspx", false);
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            string guidanceMonth = txtMonth.Text.Trim();
            string subjectName = txtSubjectName.Text.Trim();
            int outsourceId = int.Parse(ddlOutsource.SelectedValue);
            int orderType = int.Parse(ddlOrderType.SelectedValue);
            string price = txtPrice.Text.Trim();
            string remark = txtRemark.Text.Trim();
            OutsourcePriceOrder model = new OutsourcePriceOrder();
            OutsourcePriceOrderBLL bll = new OutsourcePriceOrderBLL();
            if (subjectId > 0)
            {
                model = bll.GetModel(subjectId);
            }
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                model.GuidanceYear = date.Year;
                model.GuidanceMonth = date.Month;
            }
            model.CustomerId = customerId;
            model.OrderType = orderType;
            model.OutsourceId = outsourceId;
            model.PayPrice = StringHelper.IsDecimal(price);
            model.Remark = remark;
            model.SubjectName = subjectName;
            if (subjectId > 0)
            {
                bll.Update(model);
            }
            else
            {
                model.AddDate = DateTime.Now;
                model.AddUserId = CurrentUser.UserId;
                bll.Add(model);
            }
            Alert("提交成功！", "List.aspx");
        }
    }
}
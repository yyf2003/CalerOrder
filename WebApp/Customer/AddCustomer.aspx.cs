using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;

using System.Text;

namespace WebApp.Customer
{
    public partial class AddCustomer : BasePage
    {
        public int customerId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);
            }
            if (!IsPostBack)
            {
                //InitProvince();
                BindData();
            }
        }

        void InitProvince()
        {
            var list = new PlaceBLL().GetList(s => s.ParentID == 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"ProvinceId\":\"" + s.ID + "\",\"ProvinceName\":\"" + s.PlaceName + "\"},");
                });
                hfProvinces.Value= "[" + json.ToString().TrimEnd(',') + "]";
            }
        }

        void BindData()
        {
            Models.Customer model = new CustomerBLL().GetModel(customerId);
            if (model != null)
            {
                txtCode.Text = model.CustomerCode;
                txtContact.Text = model.Contact;
                txtCustomerName.Text = model.CustomerName;
                txtShortName.Text = model.CustomerShortName;
                txtTel.Text = model.Tel;
            }

            var list = new CustomerBLL().GetList(s=>s.Id!=customerId);
            if (list.Any())
            {
                ddlOtherCustomer.DataSource = list;
                ddlOtherCustomer.DataTextField = "CustomerName";
                ddlOtherCustomer.DataValueField = "Id";
                ddlOtherCustomer.DataBind();
                ddlOtherCustomer.Items.Insert(0,new ListItem("--请选择--","0"));
            }
        }

    }
}
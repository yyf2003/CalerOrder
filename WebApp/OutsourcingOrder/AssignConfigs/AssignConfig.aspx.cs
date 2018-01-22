using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using BLL;

namespace WebApp.OutsourcingOrder.AssignConfigs
{
    public partial class AssignConfig : BasePage
    {
        public string url = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            url = Request.FilePath;
            if (!IsPostBack)
            {
                //BindMyCustomerList(seleCustomer);
                BindCustomerList(seleCustomer);
                var typeList = CommonMethod.GetEnumList<OutsourceOrderConfigType>();
                if (typeList.Any())
                {
                    typeList.ForEach(s => {
                        ListItem li = new ListItem();
                        li.Value = s.Value.ToString();
                        li.Text = s.Desction;
                        seleConfigType.Items.Add(li);
                    });
                }
                var companyList = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.IsDelete == null || s.IsDelete == false)).OrderBy(s => s.ProvinceId).ToList();
                if (companyList.Any())
                {
                    companyList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.CompanyName;
                        seleOutsource.Items.Add(li);
                    });
                }
            }
        }
    }
}
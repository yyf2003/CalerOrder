﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;

namespace WebApp.Outsource
{
    public partial class PriceList : BasePage
    {
        public int companyId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["companyId"] != null)
            {
                companyId = int.Parse(Request.QueryString["companyId"]);
            }

            if (!IsPostBack)
            {
                Company model = new CompanyBLL().GetModel(companyId);
                if (model != null)
                    labName.Text = model.CompanyName;
            }
        }


    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Materials
{
    public partial class QuoteMaterialList : System.Web.UI.Page
    {
        public string url = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            url = Request.FilePath;
        }
    }
}
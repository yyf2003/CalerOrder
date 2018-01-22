using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;

namespace WebApp.Quotation
{
    public partial class CheckFiles : BasePage
    {
        int subjectId;
        public string fileStr = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectid"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectid"]);
            }
            fileStr = GetAttachList(((int)FileCodeEnum.SubjectQuotation).ToString(), subjectId);
        }
    }
}
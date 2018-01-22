using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;

namespace WebApp.Subjects
{
    public partial class ShowSplitPlanList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int subjectId = 0;
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            Subject subjectModel = new SubjectBLL().GetModel(subjectId);
            if (subjectModel != null)
            {
                hfPlanIds.Value = subjectModel.SplitPlanIds;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;

namespace WebApp.Subjects.NewShopSubject
{
    public partial class AddShop : System.Web.UI.Page
    {
        public int SubjectId = 0;
        public string Region = string.Empty;
        public int RegionId = 0;
        public int CustomerId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                SubjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (SubjectId > 0)
            {
                Subject subjectModel = new SubjectBLL().GetModel(SubjectId);
                if (subjectModel != null)
                {
                    Region = subjectModel.Region;
                    CustomerId = subjectModel.CustomerId ?? 0;
                    if (!string.IsNullOrWhiteSpace(Region))
                    {
                        var regionModel = new RegionBLL().GetList(s => s.CustomerId == CustomerId && s.RegionName.ToLower() == Region.ToLower()).FirstOrDefault();
                        if (regionModel != null)
                            RegionId = regionModel.Id;
                    }
                }
            }
        }
    }
}
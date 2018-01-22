using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using Common;

namespace WebApp.Subjects
{
    public partial class transfers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int subjectId = 0;
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
                Subject model = new SubjectBLL().GetModel(subjectId);
                if (model != null)
                {
                    string url = string.Format("ShowOrderDetail.aspx?fromReject=1&subjectId={0}", subjectId);
                    if (model.SubjectType == (int)SubjectTypeEnum.补单)
                    {
                        url = string.Format("/Subjects/HandMadeOrder/CheckDetail.aspx?fromReject=1&subjectId={0}", subjectId);
                    }
                    else
                    {
                        if (model.SupplementRegion != null && model.SupplementRegion.Length > 0 && model.SubjectType != (int)SubjectTypeEnum.正常单)
                        {

                            url = string.Format("/Subjects/RegionSubject/CheckOrderDetail.aspx?fromReject=1&subjectId={0}", subjectId);
                        }
                        if (model.SubjectType == (int)SubjectTypeEnum.二次安装)
                        {
                            url = string.Format("/Subjects/SecondInstallFee/CheckDetail.aspx?fromReject=1&subjectId={0}", subjectId);
                        }
                    }
                    if (model.SubjectType == (int)SubjectTypeEnum.外协订单)
                    {
                        url = string.Format("/Subjects/OutsourceMaterialOrder/SubjectDetail.aspx?subjectId={0}", subjectId);
                    }
                    Response.Redirect(url, false);
                }
            }
        }
    }
}
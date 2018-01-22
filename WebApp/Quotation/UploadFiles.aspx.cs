using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;

namespace WebApp.Quotation
{
    public partial class UploadFiles : BasePage
    {
        public int SubjectId;
       
        public string FileCode = string.Empty;
        public string FileType = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            //UpLoadUC1.FileCode = ((int)FileCodeEnum.SubjectQuotation).ToString();
            //UpLoadUC1.FileType = FileTypeEnum.Files.ToString();
            //UpLoadUC1.SubjectId = Request.QueryString["subjectid"];
            //UpLoadUC1.UserId = CurrentUser.UserId.ToString();

            FileCode = ((int)FileCodeEnum.SubjectQuotation).ToString();
            FileType = FileTypeEnum.Files.ToString();
            if (Request.QueryString["subjectid"]!=null)
            SubjectId =int.Parse(Request.QueryString["subjectid"]);
            
        }
    }
}
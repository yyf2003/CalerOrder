using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;

namespace WebApp.Subjects
{
    public partial class UploadGuidanceAttachment : System.Web.UI.Page
    {
        public int ItemId;
        public string FileCode = string.Empty;
        public string FileType = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            FileCode = ((int)FileCodeEnum.SubjectGuidanceAttach).ToString();
            FileType = FileTypeEnum.Files.ToString();
            if (Request.QueryString["itemid"] != null)
                ItemId = int.Parse(Request.QueryString["itemid"]);
        }
    }
}
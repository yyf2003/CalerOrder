using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using System.Data;
using Newtonsoft.Json;

namespace WebApp.FileUploadUC
{
    public partial class UpLoadUC : System.Web.UI.UserControl
    {
        public string UserId { get; set; }
        public string FileType { get; set; }
        public string FileCode { get; set; }
        public string Code { get; set; }
        public string SubjectId { get; set; }
        
        public string JosnStr = string.Empty;

        AttachmentBLL attachBll = new AttachmentBLL();
        //Attachment attachModel;
        protected void Page_Load(object sender, EventArgs e)
        {
            GetFiles();
        }

        void GetFiles()
        {
            FileCode = FileCode != null ? FileCode : "0";

            int sid = int.Parse(!string.IsNullOrWhiteSpace(SubjectId) ? SubjectId : "0");
            
            var list = attachBll.GetList(s => s.FileCode==FileCode && s.FileType==FileType && s.SubjectId==sid && (s.IsDelete==null || s.IsDelete==false));
            
            if (list.Any())
            {
                JosnStr = JsonConvert.SerializeObject(list);
            }
        }
    }
}
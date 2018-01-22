using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Services;
using BLL;
using Models;
using Common;
using WebApp;

namespace upload.uploadify
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    /// 
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Handler1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
           
            HttpPostedFile file = context.Request.Files["Filedata"];
            int subjectId =int.Parse(context.Request.QueryString["subjectid"]);
            int secItemId = 0;
            if (context.Request.QueryString["secitemid"] != null)
            {
                secItemId = int.Parse(context.Request.QueryString["secitemid"]);
            }
            string fileCode = context.Request.QueryString["filecode"];
            string fileType = context.Request.QueryString["filetype"];
            if (file != null)
            {
                AttachmentBLL attachBll = new AttachmentBLL();
                Attachment attachModel;
                attachModel = new Attachment();
                attachModel.FileType = fileType;
                attachModel.SubjectId = subjectId;
                attachModel.SecItemId = secItemId;
                attachModel.FileCode = fileCode;
                OperateFile.UpFiles(file, ref attachModel);
                attachModel.AddDate = DateTime.Now;
                attachModel.AddUserId = new BasePage().CurrentUser.UserId;
                attachModel.IsDelete = false;
                attachBll.Add(attachModel);
                
            }
        }


        


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
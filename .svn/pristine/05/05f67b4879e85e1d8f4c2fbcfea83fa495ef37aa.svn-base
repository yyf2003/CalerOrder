using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using BLL;
using DAL;
using Models;
using Common;

namespace WebApp.FileUploadUC
{
    public partial class UpLoad : System.Web.UI.Page
    {
        public string fileType = "Files";
        string userId = "0";
        string fileCode = "0";
        string subjectId = "0";
        string code = "1";
        AttachmentBLL attachBll = new AttachmentBLL();
        Attachment attachModel;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["filetype"] != null)
            {
                fileType = Request.QueryString["filetype"].ToString();
            }
            if (Request.QueryString["userid"] != null)
            {
                userId = Request.QueryString["userid"].ToString();
            }
            if (Request.QueryString["filecode"] != null)
            {
                fileCode = Request.QueryString["filecode"].ToString();
            }
            if (Request.QueryString["subjectid"] != null)
            {
                subjectId = Request.QueryString["subjectid"].ToString();
            }
            if (Request.QueryString["code"] != null)
            {
                code = Request.QueryString["code"].ToString();
            }
        }

        protected void btn_UpLoad_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                if (FileUpload1.PostedFile.ContentLength <= 30485760)//<=30m
                {

                    attachModel = new Attachment();
                    attachModel.FileType = fileType;
                    attachModel.SubjectId = int.Parse(subjectId);
                    attachModel.FileCode = fileCode;
                    OperateFile.UpFiles(FileUpload1.PostedFile, ref attachModel);
                    attachModel.AddDate = DateTime.Now;
                    attachModel.AddUserId = int.Parse(userId);
                    attachModel.IsDelete = false;
                    attachBll.Add(attachModel);

                    string jsonStr = JsonConvert.SerializeObject(attachModel);
                    ClientScript.RegisterClientScriptBlock(GetType(), "callback", "callback(" + jsonStr + "," + code + ")", true);

                }
                else
                {
                    ClientScript.RegisterClientScriptBlock(GetType(), "callback", "AlertError()", true);

                }
            }
        }
    }
}
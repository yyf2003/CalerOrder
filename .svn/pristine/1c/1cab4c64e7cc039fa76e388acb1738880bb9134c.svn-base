using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using BLL;
using Models;

namespace WebApp.Subjects.js
{
    /// <summary>
    /// GuidanceDetail 的摘要说明
    /// </summary>
    public class GuidanceDetail : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            if (context.Request.QueryString["type"] != null)
                type = context.Request.QueryString["type"];
            switch (type)
            { 
                case "download":
                    int fileId = int.Parse(context.Request.QueryString["fileId"]);
                    DownLoad(fileId);
                    break;
            }
        }

        void DownLoad(int fileId)
        {
            Attachment model = new AttachmentBLL().GetModel(fileId);
            if (model != null && !string.IsNullOrWhiteSpace(model.FilePath))
            {
                OperateFile.DownLoadFile(model.FilePath);
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
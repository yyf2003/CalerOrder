using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using System.IO;
using Newtonsoft.Json;

namespace WebApp.Handler
{
    /// <summary>
    /// OperateFile 的摘要说明
    /// </summary>
    public class Files : IHttpHandler
    {
        int fileId;
        string OperateType = string.Empty;
        AttachmentBLL attachBll = new AttachmentBLL();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["id"] != null)
            {
                fileId = int.Parse(context.Request.QueryString["id"].ToString());
            }
            if (context.Request.QueryString["type"] != null)
            {
                OperateType = context.Request.QueryString["type"].ToString();
            }
            string r = "";
            switch (OperateType)
            {
                case "download":
                    r = DownLoadFile(context);
                    break;
                case "deletefile":
                    int id = int.Parse(context.Request.QueryString["id"]);
                    r = DeleteFile(id);
                    break;
                case "getfiles":
                    int subjectId = 0;
                    int secItemId = 0;
                    string fileCode=string.Empty;;
                    string fileType = string.Empty;
                    if (context.Request.QueryString["subjectid"]!=null)
                        subjectId = int.Parse(context.Request.QueryString["subjectid"]);
                    if (context.Request.QueryString["secitemid"]!=null)
                        secItemId = int.Parse(context.Request.QueryString["secitemid"]);
                    if (context.Request.QueryString["filecode"]!=null)
                        fileCode = context.Request.QueryString["filecode"];
                    if (context.Request.QueryString["filetype"]!=null)
                        fileType = context.Request.QueryString["filetype"];
                    r = GetFiles(subjectId,secItemId, fileCode, fileType);
                    break;
            }
            context.Response.Write(r);
        }


        string DownLoadFile(HttpContext context)
        {

            Attachment attach = attachBll.GetModel(fileId);
            string path = attach.FilePath;
            if (path != "")
            {
                string path1 = HttpContext.Current.Server.MapPath(path);



                long len = 102400;
                byte[] buffer = new byte[len];
                long read = 0;
                FileStream fs = null;
                fs = new FileStream(path1, FileMode.Open, FileAccess.Read, FileShare.Read);
                read = fs.Length;

                string extent = path.Substring(path.LastIndexOf('.') + 1);

                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlEncode(attach.Title + "." + extent, System.Text.Encoding.UTF8)));

                context.Response.AddHeader("Content-Length", read.ToString());
                context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                while (read > 0)
                {
                    if (context.Response.IsClientConnected)
                    {
                        int length = fs.Read(buffer, 0, Convert.ToInt32(len));
                        context.Response.OutputStream.Write(buffer, 0, length);
                        context.Response.Flush();
                        //context.Response.Clear();
                        read -= length;
                    }
                    else
                    {
                        read = -1;
                    }
                }

                context.Response.Flush();
                context.Response.End();
                return "ok";
            }
            else
            {
                return "downloaderror";

            }
        }

        string DeleteFile(int id)
        {
            Attachment model = attachBll.GetModel(id);
            if (model != null)
            {
                model.IsDelete = true;
                attachBll.Update(model);
                return "ok";
            }
            else
                return "error";
        }


        string GetFiles(int subjectId,int secItemId, string fileCode,string fileType)
        {

            string JosnStr = string.Empty;
            var list = attachBll.GetList(s => s.FileCode == fileCode && s.FileType == fileType && s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false));
            if (secItemId > 0)
            {
                list = list.Where(s => s.SecItemId == secItemId).ToList();
            }
            if (list.Any())
            {
                JosnStr = JsonConvert.SerializeObject(list);
            }
            return JosnStr;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using Common;
using Newtonsoft.Json;

namespace WebApp.Modules
{
    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    public class UploadHandler : IHttpHandler
    {
        ModuleMenuImgBLL bll = new ModuleMenuImgBLL();
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            string result = string.Empty;
            switch (type)
            { 
                case "upload":
                    Upload();
                    break;
                case "getfiles":
                    result = GetImgs();
                    break;
                case "deletefile":
                    int id = 0;
                    if (context.Request.QueryString["id"] != null)
                    {
                        id = int.Parse(context.Request.QueryString["id"]);
                    }
                   result= DeleteImg(id);
                    break;
            }
            context.Response.Write(result);
        }

        void Upload()
        {
            HttpPostedFile file = context1.Request.Files["Filedata"];
            if (file != null)
            {
                string path = OperateFile.UpLoadFile(file, "MenuImg");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    ModuleMenuImg model = new ModuleMenuImg();
                    model.Url = path;
                    bll.Add(model);
                }
            }
        }

        string GetImgs()
        {
            var list = bll.GetList(s=>1==1);
            if (list.Any())
            {
                return JsonConvert.SerializeObject(list);
            }
            else
                return "";
        }

        string DeleteImg(int id)
        {
            ModuleMenuImg model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                return "ok";
            }
            else
                return "error";
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using System.Text;
using Newtonsoft.Json;
using Common;

namespace WebApp.Customer.Handler
{
    /// <summary>
    /// InstallPriceList 的摘要说明
    /// </summary>
    public class InstallPriceList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
                type = context.Request.QueryString["type"];
            else if(context.Request.Form["type"] != null)
                type = context.Request.Form["type"];
            switch (type)
            {
                case "getList":
                    result = GetList();
                    break;
                case "edit":
                    string jsonStr = string.Empty;
                    if (context.Request.Form["jsonStr"] != null)
                    {
                        jsonStr = context.Request.Form["jsonStr"];
                        jsonStr = context.Server.UrlDecode(jsonStr);
                    }
                    result= Edit(jsonStr);
                    break;
                case "delete":
                    string ids = string.Empty;
                    if (context.Request.Form["ids"] != null)
                    {
                        ids = context.Request.Form["ids"];
                    }
                    result = Delete(ids);
                    break;
                case "getMaterialSupportList":
                    result = GetBasicMaterialSupportList();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetBasicMaterialSupportList()
        {
            var list = new BasicMaterialSupportBLL().GetList(s=>s.IsDelete==null || s.IsDelete==false);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"MaterialSupportName\":\"" + s.MaterialSupportName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetList()
        {
            var list = new InstallPriceConfigBLL().GetList(s=>s.Id>0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int rowIndex = 1;
                list.ForEach(s => {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"RowIndex\":\"" + (rowIndex++) + "\",\"MaterialSupport\":\"" + s.MaterialSupport + "\",\"BasicInstallPrice\":\"" + s.BasicInstallPrice + "\",\"WindowInstallPrice\":\"" + s.WindowInstallPrice + "\",\"BasicMaterialSupportId\":\"" + s.BasicMaterialSupportId + "\",\"OutsourceBasicInstallPrice\":\"" + s.OutsourceBasicInstallPrice + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

        string Edit(string jsonStr)
        {
            string r = "提交失败";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                try
                {
                    List<InstallPriceConfig> list = JsonConvert.DeserializeObject<List<InstallPriceConfig>>(jsonStr);
                    InstallPriceConfigBLL bll = new InstallPriceConfigBLL();
                    if (list != null && list.Any())
                    {
                        list.ForEach(s =>
                        {
                            string MaterialSupport = s.MaterialSupport;
                            InstallPriceConfig model = bll.GetList(i =>i.Id==s.Id).FirstOrDefault();
                            if (model != null)
                            {
                                model.BasicMaterialSupportId = s.BasicMaterialSupportId;
                                model.MaterialSupport = s.MaterialSupport;
                                model.BasicInstallPrice = s.BasicInstallPrice;
                                model.WindowInstallPrice = s.WindowInstallPrice;
                                model.OutsourceBasicInstallPrice = s.OutsourceBasicInstallPrice;
                                bll.Update(model);
                            }
                            else
                            {
                                model = new InstallPriceConfig();
                                model.BasicMaterialSupportId = s.BasicMaterialSupportId;
                                model.MaterialSupport = s.MaterialSupport;
                                model.BasicInstallPrice = s.BasicInstallPrice;
                                model.WindowInstallPrice = s.WindowInstallPrice;
                                model.OutsourceBasicInstallPrice = s.OutsourceBasicInstallPrice;
                                model.AddDate = DateTime.Now;
                                bll.Add(model);
                            }
                        });
                    }
                    r = "ok";
                }
                catch(Exception ex)
                {
                    r = ex.Message;
                }
            }
            return r;
        }

        string Delete(string ids)
        {
            string r = "删除失败";
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids,',');
                if (idList.Any())
                {
                    try
                    {
                        new InstallPriceConfigBLL().Delete(s => idList.Contains(s.Id));
                        r = "ok";
                    }
                    catch (Exception ex)
                    {
                        r = ex.Message;
                    }

                }
            }
            return r;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using System.Text;
using Newtonsoft.Json;
using Common;

namespace WebApp.ExpressPriceManage
{
    /// <summary>
    /// List1 的摘要说明
    /// </summary>
    public class List1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
                type = context.Request.QueryString["type"];
            else if (context.Request.Form["type"] != null)
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
                    result=Edit(jsonStr);
                    break;
                case "delete":
                    string ids = string.Empty;
                    if (context.Request.Form["ids"] != null)
                    {
                        ids = context.Request.Form["ids"];
                    }
                    result = Delete(ids);
                    break;
            }
            context.Response.Write(result);
        }


        string GetList() 
        {
            var list = new ExpressPriceConfigBLL().GetList(s => s.Id > 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int rowIndex = 1;
                list.ForEach(s =>
                {
                    //string isDefault = string.Empty;
                    int isDefault = 0;
                    if ((s.IsDefault ?? false))
                        isDefault = 1;
                    json.Append("{\"Id\":\"" + s.Id + "\",\"RowIndex\":\"" + (rowIndex++) + "\",\"ReceivePrice\":\"" + s.ReceivePrice + "\",\"PayPrice\":\"" + s.PayPrice + "\",\"IsDefault\":\"" + isDefault + "\"},");

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
                    List<ExpressPriceConfig> list = JsonConvert.DeserializeObject<List<ExpressPriceConfig>>(jsonStr);
                    ExpressPriceConfigBLL bll = new ExpressPriceConfigBLL();
                    if (list != null && list.Any())
                    {
                        list.ForEach(s =>
                        {
                            
                            ExpressPriceConfig model = bll.GetList(i => i.Id == s.Id).FirstOrDefault();
                            if (model != null)
                            {
                                model.IsDefault = s.IsDefault;
                                model.PayPrice = s.PayPrice;
                                model.ReceivePrice = s.ReceivePrice;
                                bll.Update(model);
                            }
                            else
                            {
                                model = new ExpressPriceConfig();
                                model.IsDefault = s.IsDefault;
                                model.PayPrice = s.PayPrice;
                                model.ReceivePrice = s.ReceivePrice;
                                bll.Add(model);
                            }
                        });
                    }
                    r = "ok";
                }
                catch (Exception ex)
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
                List<int> idList = StringHelper.ToIntList(ids, ',');
                if (idList.Any())
                {
                    try
                    {
                        new ExpressPriceConfigBLL().Delete(s => idList.Contains(s.Id));
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
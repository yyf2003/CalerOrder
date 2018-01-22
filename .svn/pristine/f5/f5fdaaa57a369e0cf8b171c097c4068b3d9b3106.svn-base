using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using System.Text;
using Newtonsoft.Json;

namespace WebApp.MaterialSupportManager.handler
{
    /// <summary>
    /// List 的摘要说明
    /// </summary>
    public class List : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            else if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            switch (type)
            {
                case "getBasic":
                    result = GetBasicMaterialSupportList();
                    break;
                case "getList":
                    int basicId = 0;
                    if (context.Request.QueryString["basicId"] != null)
                        basicId = int.Parse(context.Request.QueryString["basicId"]);
                    result = GetList(basicId);
                    break;
                case "edit":
                    string jsonStr = context.Request.Form["jsonstr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = jsonStr.Replace("+", "%2B");
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    result = Edit(jsonStr);
                    break;
                case "getModel":
                    int id = 0;
                    if (context.Request.QueryString["id"] != null)
                        id = int.Parse(context.Request.QueryString["id"]);
                    result = GetModel(id);
                    break;
                case "delete":
                    string ids = string.Empty;
                    if (context.Request.QueryString["ids"] != null)
                        ids = context.Request.QueryString["ids"];
                    result = Delete(ids);
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetBasicMaterialSupportList()
        {
            var list = new BasicMaterialSupportBLL().GetList(s => s.IsDelete == null || s.IsDelete == false);

            StringBuilder json = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"MaterialSupportName\":\"" + s.MaterialSupportName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";

        }

        string GetList(int basicId)
        {
            var list = (from item in CurrentContext.DbContext.NewMaterialSupport
                       join basic in CurrentContext.DbContext.BasicMaterialSupport
                       on item.BasicMaterialSupportId equals basic.Id
                       where basicId > 0 ? (item.BasicMaterialSupportId == basicId) : 1 == 1
                       select new {
                           item,
                           basic.MaterialSupportName
                       }).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int rowIndex = 0;
                list.ForEach(s => {
                    json.Append("{\"rowIndex\":\"" + (++rowIndex) + "\",\"Id\":\"" + s.item.Id + "\",\"MaterialSupportName\":\"" + s.item.NewMaterialSupportName + "\",\"BasicMaterialSupportId\":\"" + s.item.BasicMaterialSupportId + "\",\"BasicMaterialSupportName\":\"" + s.MaterialSupportName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
        }

        NewMaterialSupportBLL bll = new NewMaterialSupportBLL();
        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                NewMaterialSupport model = JsonConvert.DeserializeObject<NewMaterialSupport>(jsonStr);

                if (model != null)
                {
                    if (model.Id > 0)
                    {
                        if (!CheckExist(model.Id, model.NewMaterialSupportName))
                        {
                            NewMaterialSupport newModel = bll.GetModel(model.Id);
                            if (newModel != null)
                            {
                                newModel.BasicMaterialSupportId = model.BasicMaterialSupportId;
                                newModel.NewMaterialSupportName = model.NewMaterialSupportName;
                                
                                bll.Update(newModel);
                            }
                            else
                            {
                                bll.Add(model);
                            }
                        }
                        else
                            result = "exist";
                    }
                    else
                    {
                        if (!CheckExist(0, model.NewMaterialSupportName))
                            bll.Add(model);
                        else
                            result = "exist";
                    }
                }
            }
            return result;
        }

        bool CheckExist(int id, string name)
        {
            var model = bll.GetList(s => s.NewMaterialSupportName.ToLower() == name.ToLower() && (id > 0 ? (s.Id != id) : 1 == 1));
            return model.Any();
        }

        string GetModel(int id)
        {
            NewMaterialSupport model = bll.GetModel(id);
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                json.Append("{\"Id\":\"" + model.Id + "\",\"BasicMaterialSupportId\":\"" + model.BasicMaterialSupportId + "\",\"NewMaterialSupportName\":\"" + model.NewMaterialSupportName + "\"}");
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string Delete(string ids)
        {
            string r = "删除失败";
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = Common.StringHelper.ToIntList(ids,',');
                if (idList.Any())
                {
                    bll.Delete(s => idList.Contains(s.Id));
                    return "ok";
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
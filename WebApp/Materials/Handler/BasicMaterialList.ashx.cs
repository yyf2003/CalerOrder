using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;

namespace WebApp.Materials.Handler
{
    /// <summary>
    /// BasicMaterialList 的摘要说明
    /// </summary>
    public class BasicMaterialList : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
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
                case "getCategory":
                    result = GetMaterialCategory();
                    break;
                case "getList":
                    int currPage = 0;
                    int pageSize = 0;
                    if (context.Request.QueryString["currpage"] != null)
                    {
                        currPage = int.Parse(context.Request.QueryString["currpage"]);
                    }
                    if (context.Request.QueryString["pagesize"] != null)
                    {
                        pageSize = int.Parse(context.Request.QueryString["pagesize"]);
                    }
                    result=GetMaterialList(currPage, pageSize);
                    break;
                case "getUnit":
                    result = GetUnits();
                    break;
                case "getModel":
                    int id=int.Parse(context.Request.QueryString["id"]);
                    result=GetModel(id);
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
                case "update":
                    int id1 = int.Parse(context.Request.QueryString["id"]);
                    int state = int.Parse(context.Request.QueryString["state"]);
                    result = Update(id1, state);
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetMaterialCategory()
        {
            var list = new MaterialCategoryBLL().GetList(s => s.Id > 0);
            StringBuilder json = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CategoryName\":\"" + s.CategoryName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
        }

        //BasicMaterialBLL materialBll = new BasicMaterialBLL();
        string GetMaterialList(int currPage, int pageSize)
        {
            var list = (from material in CurrentContext.DbContext.BasicMaterial
                        join category in CurrentContext.DbContext.MaterialCategory
                        on material.MaterialCategoryId equals category.Id
                        join unit1 in CurrentContext.DbContext.UnitInfo
                        on material.UnitId equals unit1.Id into unitTemp
                        from unit in unitTemp.DefaultIfEmpty()
                        select new
                        {
                            category.CategoryName,
                            material,
                            unit.UnitName
                        }).ToList();
            int categoryId = 0;
            if (context1.Request.QueryString["categoryId"] != null)
            {
                categoryId = int.Parse(context1.Request.QueryString["categoryId"]);
            }
            if (categoryId > 0)
                list = list.Where(s => s.material.MaterialCategoryId == categoryId).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = list.Count;
                list = list.OrderBy(s => s.material.MaterialCategoryId).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                list.ForEach(s =>
                {
                    string state = s.material.IsDelete != null && s.material.IsDelete == true ? "已删除" : "正常";
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.material.Id + "\",\"MaterialCategoryId\":\"" + s.material.MaterialCategoryId + "\",\"CategoryName\":\"" + s.CategoryName + "\",\"MaterialName\":\"" + s.material.MaterialName + "\",\"Unit\":\"" + s.UnitName + "\",\"State\":\"" + state + "\"},");
                    index++;
                });
                if (json.Length > 0)
                    return "{\"total\":" + totalCount + ",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
                else
                    return "{\"total\":0,\"rows\":[] }";
            }
            else
                return "{\"total\":0,\"rows\":[] }";
        }

        string GetUnits()
        {
            var list = new UnitInfoBLL().GetList(s=>s.Id>0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"UnitName\":\""+s.UnitName+"\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }
        BasicMaterialBLL bll = new BasicMaterialBLL();
        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                BasicMaterial model = JsonConvert.DeserializeObject<BasicMaterial>(jsonStr);
                
                if (model != null)
                {
                    if (model.Id > 0)
                    {
                        if (!CheckExist(model.Id, model.MaterialName))
                        {
                            BasicMaterial newModel = bll.GetModel(model.Id);
                            int categoryId = newModel.MaterialCategoryId??0;
                            int unitId = newModel.UnitId ?? 0;
                            if (newModel != null)
                            {
                                newModel.MaterialCategoryId = model.MaterialCategoryId;
                                newModel.MaterialName = model.MaterialName;
                                newModel.UnitId = model.UnitId;
                                bll.Update(newModel);

                                if (categoryId != (model.MaterialCategoryId ?? 0) || unitId != (model.UnitId ?? 0))
                                {
                                    CustomerMaterialInfoBLL cmBll = new CustomerMaterialInfoBLL();
                                    var list = cmBll.GetList(s => s.BasicMaterialId == model.Id);
                                    if (list.Any())
                                    {
                                        list.ForEach(s => {
                                            s.BasicCategoryId = model.MaterialCategoryId;
                                            s.UnitId = model.UnitId;
                                            cmBll.Update(s);
                                        });
                                    }
                                    if (categoryId != (model.MaterialCategoryId ?? 0))
                                    {
                                        OrderMaterialMppingBLL ommBll = new OrderMaterialMppingBLL();
                                        var list1 = ommBll.GetList(s => s.BasicMaterialId == model.Id);
                                        if (list1.Any())
                                        {
                                            list1.ForEach(s =>
                                            {
                                                s.BasicCategoryId = model.MaterialCategoryId;

                                                ommBll.Update(s);
                                            });
                                        }
                                    }
                                }

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
                        if (!CheckExist(0, model.MaterialName))
                           bll.Add(model);
                        else
                            result = "exist";
                    }
                }
            }
            return result;
        }

        bool CheckExist(int id, string materialName)
        {
            var model = bll.GetList(s => s.MaterialName.ToLower() == materialName.ToLower() && (id>0?(s.Id!=id):1==1));
            return model.Any();
        }

        string GetModel(int id)
        {
            BasicMaterial model = bll.GetModel(id);
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                json.Append("{\"Id\":\"" + model.Id + "\",\"MaterialCategoryId\":\"" + model.MaterialCategoryId + "\",\"MaterialName\":\"" + model.MaterialName + "\",\"UnitId\":\"" + model.UnitId + "\"}");
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }

        string Update(int id,int state)
        {
            string result = "ok";
            BasicMaterial model = bll.GetModel(id);
            if (model != null)
            {
                //1删除，0恢复
                model.IsDelete = state == 1;
                bll.Update(model);
            }
            else
                result = "error";
            return result;
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
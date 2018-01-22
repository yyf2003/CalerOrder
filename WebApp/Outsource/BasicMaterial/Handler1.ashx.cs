using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using DAL;
using BLL;
using Models;
using Newtonsoft.Json;

namespace WebApp.Outsource.BasicMaterial
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        HttpContext context1;
        OutSourceMaterialBLL bll = new OutSourceMaterialBLL();
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            string type = string.Empty;
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
                case "getList":
                    result=GetList();
                    break;
                case "getUnitList":
                    result = GetUnits();
                    break;
                case "edit":
                    result = Edit();
                    break;
                case "delete":
                    result = Delete();
                    break;
                case "recover":
                    result = Recover();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetList()
        {
            
            int currPage = 0;
            int pageSize = 0;
            if (context1.Request.QueryString["currPage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currPage"]);
            }
            if (context1.Request.QueryString["pageSize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pageSize"]);
            }
            var list = (from material in CurrentContext.DbContext.OutSourceMaterial
                       join unit in CurrentContext.DbContext.UnitInfo
                       on material.UnitId equals unit.Id
                       select new {
                           material,
                           unit.UnitName
                       }).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = list.Count;
                list = list.OrderBy(s => s.material.Id).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                list.ForEach(s =>
                {
                    string addDate = s.material.AddDate != null ? DateTime.Parse(s.material.AddDate.ToString()).ToShortDateString() : "";
                    string state = s.material.IsDelete != null && s.material.IsDelete == true ? "0" : "1";
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.material.Id + "\",\"MaterialName\":\"" + s.material.MaterialName + "\",\"UnitName\":\"" + s.UnitName + "\",\"UnitId\":\"" + s.material.UnitId + "\",\"UnitPrice\":\"" + s.material.UnitPrice + "\",\"AddDate\":\"" + addDate + "\",\"State\":\"" + state + "\"},");
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
            var list = new UnitInfoBLL().GetList(s => s.Id > 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"UnitName\":\"" + s.UnitName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string Edit()
        { 
           string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.QueryString["jsonStr"] != null)
            {
                jsonStr = context1.Request.QueryString["jsonStr"];
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    jsonStr = jsonStr.Replace("+", "%2B");
                    jsonStr = HttpUtility.UrlDecode(jsonStr);
                }
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                try
                {
                    
                    OutSourceMaterial newModel = JsonConvert.DeserializeObject<OutSourceMaterial>(jsonStr);
                    if (newModel != null)
                    {
                        if (newModel.Id > 0)
                        {
                            if (!CheckExist(newModel.Id, newModel.MaterialName))
                            {
                                OutSourceMaterial oldModel = bll.GetModel(newModel.Id);
                                if (oldModel != null)
                                {
                                    oldModel.MaterialName = newModel.MaterialName;
                                    oldModel.UnitId = newModel.UnitId;
                                    oldModel.UnitPrice = newModel.UnitPrice;
                                    bll.Update(oldModel);
                                }
                                else
                                {
                                    throw new Exception("数据错误");
                                }

                            }
                            else
                                result = "exist";
                        }
                        else
                        {
                            if (!CheckExist(0, newModel.MaterialName))
                            {
                                newModel.AddDate = DateTime.Now;
                                newModel.IsDelete = false;
                                bll.Add(newModel);

                            }
                            else
                                result = "exist";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }

        string Delete()
        {
            string result = "删除失败！";
            string ids = string.Empty;
            if (context1.Request.Form["ids"] != null)
            {
                ids = context1.Request.Form["ids"];
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    try
                    {
                        ids = ids.TrimEnd(',');
                        List<int> idList = Common.StringHelper.ToIntList(ids, ',');
                        OutSourceMaterial model;
                        idList.ForEach(s =>
                        {
                            model = bll.GetModel(s);
                            if (model != null)
                            {
                                //model.IsDelete = true;
                                bll.Delete(model);
                            }
                        });
                        result = "ok";
                    }
                    catch (Exception ex)
                    {
                        result = "删除失败：" + ex.Message;
                    }
                }
            }
            return result;
        }

        string Recover()
        {
            string result = "恢复失败！";
            string ids = string.Empty;
            if (context1.Request.Form["ids"] != null)
            {
                ids = context1.Request.Form["ids"];
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    try
                    {
                        ids = ids.TrimEnd(',');
                        List<int> idList = Common.StringHelper.ToIntList(ids, ',');
                        OutSourceMaterial model;
                        idList.ForEach(s =>
                        {
                            model = bll.GetModel(s);
                            if (model != null)
                            {
                                model.IsDelete = false;
                                bll.Update(model);
                            }
                        });
                        result = "ok";
                    }
                    catch (Exception ex)
                    {
                        result = "恢复失败：" + ex.Message;
                    }
                }
            }
            return result;
        }

        bool CheckExist(int id, string materialName)
        {
            var model = bll.GetList(s => s.MaterialName.ToLower() == materialName.ToLower() && (id > 0 ? (s.Id != id) : 1 == 1));
            return model.Any();
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
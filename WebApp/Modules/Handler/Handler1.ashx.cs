using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BLL;
using Models;
using Newtonsoft.Json;
using System.Transactions;
using System;

namespace WebApp.Modules.Handler
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        ModuleBLL moduleBll = new ModuleBLL();
        string type = string.Empty;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string result = string.Empty;

            if (context.Request["type"] != null)
            {
                type = context.Request["type"];
            }
            
            switch (type)
            { 
                case "getList":
                    result = GetList();
                    break;
                case "getParentList":
                    result = GetParentModuleList();
                    break;
                case "edit":
                    string json = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = AddModule(json, optype);
                    break;
                case "getOrderNum":
                    int parentId = int.Parse(context.Request["parentId"]);
                    result = GetMaxOrderNum(parentId).ToString();
                    break;
                case "editImg":
                    int moduleId=0;
                    int imgId=0;
                    if (context.Request.QueryString["moduleId"] != null)
                    {
                        moduleId = int.Parse(context.Request.QueryString["moduleId"]);
                    }
                    if (context.Request.QueryString["imgId"] != null)
                    {
                        imgId = int.Parse(context.Request.QueryString["imgId"]);
                    }
                    result= UpdateImg(moduleId, imgId);
                    break;
                case "deleteModule":
                    int moduleId0 = 0;
                    if (context.Request.QueryString["moduleId"] != null)
                    {
                        moduleId0 = int.Parse(context.Request.QueryString["moduleId"]);
                    }
                    result=DeleteModule(moduleId0);
                    break;
            }
            context.Response.Write(result);
        }


        StringBuilder json = new StringBuilder();
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        string GetList()
        {

            var list = moduleBll.GetList(s => 1 == 1);

            if (list.Any())
            {

                var list1 = list.Where(s => s.ParentId == 0).OrderBy(s=>s.OrderNum).ToList();
                list1.ForEach(s =>
                {
                    string isShowOnHome = "";
                    json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\",\"ParentId\":\"" + s.ParentId + "\",\"OrderNum\":\"" + s.OrderNum + "\",\"IsShowOnHome\":\"" + isShowOnHome + "\"");
                    GetModule(list, s.Id);
                    json.Append("},");
                });

            }
            return "[" + json.ToString().TrimEnd(',') + "]";
        }

        void GetModule(IEnumerable<Module> list, int parentId)
        {
            var list1 = list.Where(s => s.ParentId == parentId).OrderBy(s => s.OrderNum).ToList();
            if (list1.Any())
            {
                json.Append(" ,\"state\":\"closed\",\"children\":[");
                int index = 0;
                list1.ForEach(s =>
                {
                    if (index > 0)
                    {
                        json.Append(",");
                    }
                    string isShow = s.IsShow != null ? bool.Parse(s.IsShow.ToString()) ? "是" : "否" : "否";
                    string isShowOnHome = s.IsShowOnHome != null ? bool.Parse(s.IsShowOnHome.ToString()) ? "是" : "否" : "否";
                    json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\",\"ParentId\":\"" + s.ParentId + "\",\"OrderNum\":\"" + s.OrderNum + "\",\"IsShow\":\""+isShow+"\",\"IsShowOnHome\":\"" + isShowOnHome + "\",\"ImgUrl\":\""+s.ImgUrl+"\"");
                    GetModule(list, s.Id);
                    json.Append("}");
                    index++;
                });
                json.Append("]");
            }
        }

        /// <summary>
        /// 获取父级模块下拉
        /// </summary>
        /// <returns></returns>
        public string GetParentModuleList()
        {
            StringBuilder json = new StringBuilder();
            json.Append(GetList());
            string json1 = "[{\"id\":\"-100\",\"text\":\"根目录\",\"children\":";
            json.Insert(0, json1);
            json.Append("}]");
            return json.ToString();
        }

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string AddModule(string jsonString, string optype)
        {
            string result = "error";
            if (!string.IsNullOrEmpty(jsonString))
            {
                Module model = JsonConvert.DeserializeObject<Module>(jsonString);
                if (model != null)
                {
                    if (optype == "add")
                    {
                        model.IsDelete = false;
                        model.ParentId = model.ParentId == -100 ? 0 : model.ParentId;
                        if (model.OrderNum == null)
                        {
                            model.OrderNum = GetMaxOrderNum(model.ParentId ?? 0) + 1;
                        }
                        RoleInModuleBLL roleInModuleBll = new RoleInModuleBLL();
                        moduleBll.Add(model);
                        roleInModuleBll.Delete(s => s.ModuleId == model.Id);
                        var permissionList = roleInModuleBll.GetList(s => s.ModuleId == model.ParentId);
                        if (permissionList.Any())
                        {
                            permissionList.ForEach(s => {
                                s.ModuleId = model.Id;
                                roleInModuleBll.Add(s);
                            });
                        }
                    }
                    else
                    {
                        model.ParentId = model.ParentId == -100 ? 0 : model.ParentId;
                        if (model.OrderNum == null)
                        {
                            model.OrderNum = GetMaxOrderNum(model.ParentId ?? 0) + 1;
                        }
                        moduleBll.Update(model);
                    }
                    result = model.Id.ToString();
                }
            }
            return result;
        }

        int GetMaxOrderNum(int parentId)
        {
            parentId = parentId == -100 ? 0 : parentId;
            return moduleBll.GetList(s => s.ParentId == parentId).Max(s => s.OrderNum)??0;
        }

        string UpdateImg(int moduleId, int imgId)
        {
            string result = string.Empty;
            Module model = moduleBll.GetModel(moduleId);
            if (model != null)
            {
                ModuleMenuImg imgModel = new ModuleMenuImgBLL().GetModel(imgId);
                if (imgModel != null)
                {
                    model.ImgUrl = imgModel.Url;
                    moduleBll.Update(model);
                    result = "ok";
                }
            }
            return result;
        }

        string DeleteModule(int id)
        {
            string result = "ok";
            Module model = moduleBll.GetModel(id);
            if (model != null)
            {

                var childrenList = moduleBll.GetList(s => s.ParentId == id);
                if (childrenList.Any())
                {
                    result = "hasChildren";
                }
                else
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        try
                        {
                            moduleBll.Delete(model);
                            new RoleInModuleBLL().Delete(s => s.ModuleId == id);
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            result = "操作失败："+ex.Message;
                        }
                        
                    }
                }
            }
            else
                result = "操作失败";
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
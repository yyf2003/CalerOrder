using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using BLL;
using Common;
using Models;
using Newtonsoft.Json;

namespace WebApp.Modules.Handler
{
    /// <summary>
    /// AuthorityHandler1 的摘要说明
    /// </summary>
    public class AuthorityHandler1 : IHttpHandler
    {
        string type = string.Empty;
        ModuleBLL moduleBll = new ModuleBLL();
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
                case "getroles":
                    result = GetRoles();
                    break;
                case "getFirstLevelModule":
                    result = GetFirstLevelModule();
                    break;
                case "getModules":
                    int roleId = int.Parse(context.Request["roleId"]);
                    int moduleId = int.Parse(context.Request["moduleId"]);
                    result = GetModules(roleId, moduleId);
                    break;
                case "updatePermission":
                    int parentModuleId = int.Parse(context.Request.Form["parentModuleId"]);
                    int roleId1 = int.Parse(context.Request.Form["roleId"]);
                    string jsonStr = context.Request.Form["jsonStr"];
                    result = UpdatePermission(parentModuleId, roleId1, jsonStr);
                    break;
            }
            context.Response.Write(result);
        }

        string GetRoles()
        {
            RoleBLL roleBll = new RoleBLL();
            var list = roleBll.GetList(s =>s.RoleId>0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"RoleId\":\"" + s.RoleId + "\",\"RoleName\":\"" + s.RoleName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
        }

        /// <summary>
        /// 获取所有parentId=0的模块名称
        /// </summary>
        /// <returns></returns>
        public string GetFirstLevelModule()
        {
            var list = moduleBll.GetList(s => s.ParentId == 0);
            StringBuilder json = new StringBuilder();
            json.Append("{\"id\":\"" + 0 + "\",\"text\":\"全部\",\"Url\":\"\",\"ParentId\":\"-1\"},");
            if (list.Any())
            {
                list.ToList().ForEach(s =>
                {
                    json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\",\"ParentId\":\"" + s.ParentId + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
        }


        List<BaseInfo> operateList = new List<BaseInfo>();
        RoleInModuleBLL roleInModuleBll = new RoleInModuleBLL();
        RoleInModule roleInModuleModel;
        public string GetModules(int roleId,int moduleId)
        {
            GetOperateType();
            var list = moduleBll.GetList(s =>s.Id>0);
            if (moduleId > 0)
                list = moduleBll.GetModulesById(moduleId);
            StringBuilder json = new StringBuilder();
            if (list.Any())
            {
                var permissionList = roleInModuleBll.GetList(s => s.RoleId == roleId);
                list.ToList().ForEach(s => {
                    string open = string.Empty;
                    if (s.ParentId>0)
                        open = "\"open\":true,";
                    json.Append("{\"id\":\"" + s.Id + "\",\"pId\":\"" + s.ParentId + "\",\"name\":\"" + s.ModuleName + "\"," + open + "},");

                    //操作类型!list.Where(s1 => s1.ParentId == s.Id).Any() &&
                    if (s.ParentId>0 && operateList.Any())
                    {
                        roleInModuleModel = permissionList.FirstOrDefault(p => p.ModuleId == s.Id);
                        List<string> permissions = new List<string>();
                        if (roleInModuleModel != null && roleInModuleModel.OperatePermission!=null)
                        {
                            permissions = roleInModuleModel.OperatePermission.Split('|').ToList();

                        }
                        operateList.ForEach(o =>
                        {
                            string check = string.Empty;
                            if (permissions.Contains(o.BaseCode))
                                check = "\"checked\":true,";
                            json.Append("{\"id\":\"" + o.BaseCode + "\",\"pId\":\"" + s.Id + "\",\"name\":\"" + o.BaseName + "\",\"IsOperateType\":\"1\"," + check + " \"open\":true},");
                        });
                    }
                    
                });
                if (json.Length > 0)
                {
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                else
                    return "[]";
            }
            else
                return "[]";
        }

        void GetOperateType()
        {
            BaseBLL baseBll = new BaseBLL();
            operateList = baseBll.GetList(s => s.BaseCategoryId == (int)CategoryTypeEnum.OperateType).ToList();
        }



        public string UpdatePermission(int parentModuleId, int roleId, string jsonStr)
        {
            var permissionList = roleInModuleBll.GetList(s => s.RoleId == roleId);
            if (parentModuleId > 0)
            {
                var list = moduleBll.GetModulesById(parentModuleId).Select(m => m.Id).ToList();
                if (list.Any())
                {
                    permissionList = permissionList.Where(s => list.Contains(s.ModuleId ?? 0)).ToList();

                }
            }
            if (permissionList.Any())
            {
                permissionList.ToList().ForEach(s =>
                {
                    roleInModuleModel = s;
                    roleInModuleBll.Delete(roleInModuleModel);
                });
            }

            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        List<RoleInModule> rmlist = JsonConvert.DeserializeObject<List<RoleInModule>>(jsonStr);
                        if (rmlist.Any())
                        {
                            rmlist.ForEach(s =>
                            {
                                roleInModuleBll.Add(s);
                            });
                            tran.Complete();
                            return "ok";
                        }
                        else
                            return "error";
                    }
                    return "ok";
                }
                catch (Exception ex)
                {
                    return "提交失败：" + ex.Message;
                }
                finally
                {
                    tran.Dispose();
                }
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
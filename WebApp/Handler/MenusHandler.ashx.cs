using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using System.Text;

namespace WebApp.Handler
{
    /// <summary>
    /// MenusHandler 的摘要说明
    /// </summary>
    public class MenusHandler : IHttpHandler
    {

        RoleInModuleBLL roleModuleBll = new RoleInModuleBLL();
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
                case "getTopMenu":
                    result = GetTopMenu();
                    break;
                case "getChildMenu":
                    int pid = int.Parse(context.Request["parentId"]);
                    result = GetChildMenu(pid);
                    break;
            }
            context.Response.Write(result);
        }

        public string GetTopMenu()
        {

            int roleId = new BasePage().CurrentUser.RoleId;
            int userId = new BasePage().CurrentUser.UserId;
            StringBuilder json = new StringBuilder();
            List<int> myModules = roleModuleBll.GetList(s => s.RoleId == roleId).Select(s => s.ModuleId ?? 0).ToList();
            List<int> userInModuules = new UserInModuleBLL().GetList(s => s.RoleId == roleId && s.UserId == userId).Select(s => s.ModuleId ?? 0).ToList();
            myModules.AddRange(userInModuules);
            myModules = myModules.Distinct().ToList();
            using (KalerOrderDBEntities dc = new KalerOrderDBEntities())
            {
                var modules = from module in dc.Module
                              select module;
                var parentids = from module in modules
                                where myModules.Contains(module.Id)
                                select module.ParentId;
                var myMenuList = (from module in modules
                                  where parentids.Contains(module.Id) && module.IsShow==true && module.ParentId==0
                                  select module).OrderBy(s => s.OrderNum);
                if (myMenuList.Any())
                {
                    myMenuList.ToList().ForEach(s =>
                    {
                        json.Append("{\"Id\":\"" + s.Id + "\",\"ModuleName\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\"},");
                    });
                }
            }
            if (json.Length > 0)
                return "[" + json.ToString().TrimEnd(',') + "]";
            else
                return "[]";



        }

        public string GetChildMenu(int parentId)
        {
            StringBuilder json = new StringBuilder();
            int roleId = new BasePage().CurrentUser.RoleId;
            int userId=new BasePage().CurrentUser.UserId;
            using (KalerOrderDBEntities dc = new KalerOrderDBEntities())
            {
                var myModules = roleModuleBll.GetList(s => s.RoleId == roleId).Select(s => s.ModuleId ?? 0);
                var modules = from module in dc.Module
                              where module.IsShow==true
                              select module;
                List<Module> list = modules.Where(s => s.ParentId == parentId && myModules.Contains(s.Id)).ToList();

                var userInModules = new UserInModuleBLL().GetList(s => s.RoleId == roleId && s.UserId==userId).Select(s => s.ModuleId ?? 0);
                var userModules = from module in dc.Module
                              where module.IsShow == true
                              select module;
                List<Module> userMlist = userModules.Where(s => s.ParentId == parentId && userInModules.Contains(s.Id)).ToList();
                list.AddRange(userMlist);
                list = list.Distinct().OrderBy(s=>s.OrderNum).ToList();
                if (list.Any())
                {
                    list.ToList().ForEach(s =>
                    {
                        json.Append("{\"Id\":\"" + s.Id + "\",\"ModuleName\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\"},");
                    });
                }
            }
            if (json.Length > 0)
                return "[" + json.ToString().TrimEnd(',') + "]";
            else
                return "[]";

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
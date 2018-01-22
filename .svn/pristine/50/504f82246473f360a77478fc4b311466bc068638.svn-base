using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using Newtonsoft.Json;

namespace WebApp.Role.Handler
{
    /// <summary>
    /// RoleHandler1 的摘要说明
    /// </summary>
    public class RoleHandler1 : IHttpHandler
    {
        RoleBLL roleBll = new RoleBLL();
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

                case "edit":
                    string jsonString = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = AddRole(jsonString, optype);
                    break;
            }
            context.Response.Write(result);
        }

        string AddRole(string jsonString, string optype)
        {
            string result = "提交失败！";
            if (!string.IsNullOrEmpty(jsonString))
            {
                Models.Role model = JsonConvert.DeserializeObject<Models.Role>(jsonString);
               
                if (model != null)
                {
                    if (optype == "add")
                    {
                        if (!CheckRoleName(model.RoleName, 0))
                        {

                            model.IsDelete = false;
                            roleBll.Add(model);
                            
                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                    else
                    {
                        if (!CheckRoleName(model.RoleName, model.RoleId))
                        {
                            Models.Role rm = roleBll.GetModel(model.RoleId);
                            if (rm != null)
                            {
                                rm.RoleName = model.RoleName;
                                roleBll.Update(rm);
                                
                                result = "ok";
                            }


                        }
                        else
                            result = "exist";

                    }


                }
                
                
            }
            return result;
        }


        bool CheckRoleName(string name, int roleid)
        {

            var list = roleBll.GetList(s => s.RoleName == name && (roleid > 0 ? s.RoleId != roleid : 1 == 1));
            return list.Any();
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
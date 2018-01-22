using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BLL;
using DAL.Models;

namespace WebApp.Handler
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class ModuleOperate : IHttpHandler
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
            if (type == "getList")
            {
                result = GetList();

            }
            context.Response.Write(result);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        StringBuilder json = new StringBuilder();
        string GetList()
        {

            var list = moduleBll.GetList(s => 1 == 1);

            if (list.Any())
            {

                var list1 = list.Where(s => s.ParentId == 0);
                list1.ToList().ForEach(s =>
                {
                    json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\",\"ParentId\":\"" + s.ParentId + "\"");
                    GetModule(list, s.Id);
                    json.Append("},");
                });

            }
            return "[" + json.ToString().TrimEnd(',') + "]";
        }

        void GetModule(IEnumerable<Module> list, int parentId)
        {
            var list1 = list.Where(s => s.ParentId == parentId);
            if (list1.Any())
            {
                json.Append(" ,\"children\":[");
                int index = 0;
                list1.ToList().ForEach(s =>
                {
                    if (index > 0)
                    {
                        json.Append(",");
                    }
                    json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\",\"ParentId\":\"" + s.ParentId + "\"");
                    GetModule(list, s.Id);
                    json.Append("}");
                    index++;
                });
                json.Append("]");
            }
        }

    }
}
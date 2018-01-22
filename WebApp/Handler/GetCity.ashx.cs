using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using BLL;
using System.Text;

namespace WebApp.Handler
{
    /// <summary>
    /// GetCity 的摘要说明
    /// </summary>
    public class GetCity : IHttpHandler
    {
        int provinceId = -1;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request["provinceid"]!=null)
               provinceId = int.Parse(context.Request["provinceid"]);
            provinceId = provinceId == 0 ? -1 : provinceId;
            context.Response.Write(GetCities(provinceId));
        }

        string GetCities(int provinceId)
        {
            var list = new PlaceBLL().GetList(s=>s.ParentID==provinceId);
            StringBuilder json = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s => {
                    json.Append("{\"ID\":\""+s.ID+"\",\"PlaceName\":\""+s.PlaceName+"\"},");
                });
            }
            return json.Length > 0 ? ("[" + json.ToString().Trim(',') + "]") : "";
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
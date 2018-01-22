using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using DAL;
using Models;
using Common;

namespace WebApp.Handler
{
    /// <summary>
    /// Place 的摘要说明
    /// </summary>
    public class Place : IHttpHandler
    {
        string type = string.Empty;
        int customerId;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            switch (type)
            {
                case "getRegion":
                    result = GetRegion();
                    break;
                case "getProvince":
                    string regionId = context.Request.QueryString["regionId"];
                    result = GetProvince(regionId);
                    break;
                case "getCity":
                    string provinceId = context.Request.QueryString["provinceId"];
                    result = GetCity(provinceId);
                    break;
            }
            context.Response.Write(result);
        }


        string GetRegion()
        {
            var list = new RegionBLL().GetList(s => s.CustomerId == customerId && (s.IsDelete == null || s.IsDelete == false));
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"RegionId\":\"" + s.Id + "\",\"RegionName\":\"" + s.RegionName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetProvince(string regionId)
        {
            List<int> regionid = StringHelper.ToIntList(regionId,',');
            var list = (from province in CurrentContext.DbContext.Place
                       join pr in CurrentContext.DbContext.ProvinceInRegion
                       on province.ID equals pr.ProvinceId
                       where regionid.Contains(pr.RegionId??0)
                        select province).OrderBy(s => s.ID).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"ProvinceId\":\"" + s.ID + "\",\"ProvinceName\":\"" + s.PlaceName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetCity(string provinceId)
        {
            List<int> provinceid = StringHelper.ToIntList(provinceId, ',');
            var list = new PlaceBLL().GetList(s => s.ParentID > 0 && provinceid.Contains(s.ParentID??0)).OrderBy(s=>s.ParentID).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"CityId\":\"" + s.ID + "\",\"CityName\":\"" + s.PlaceName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
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
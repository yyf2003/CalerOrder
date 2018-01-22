using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using System.Text;
using Newtonsoft.Json;
using Common;

namespace WebApp.Customer.Handler
{
    /// <summary>
    /// ERPHostManage 的摘要说明
    /// </summary>
    public class ERPHostManage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
                type = context.Request.QueryString["type"];
            else if (context.Request.Form["type"] != null)
                type = context.Request.Form["type"];
            switch (type)
            {
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
                    result = GetList(currPage, pageSize);
                    break;
                case "getModel":
                    int id = 0;
                    if (context.Request.QueryString["id"] != null)
                        id = int.Parse(context.Request.QueryString["id"]);
                    result = GetModel(id);
                    break;
                case "edit":
                    string jsonStr = string.Empty;
                    if (context.Request.Form["jsonString"] != null)
                    {
                        jsonStr = context.Request.Form["jsonString"];
                        jsonStr = context.Server.UrlDecode(jsonStr);
                    }
                    result = Edit(jsonStr);
                    break;
                case "delete":
                    string ids = string.Empty;
                    if (context.Request.Form["ids"] != null)
                    {
                        ids = context.Request.Form["ids"];
                    }
                    result = Delete(ids);
                    break;
                
                default:
                    break;
            }
            context.Response.Write(result);
        }
        ErpHostBLL bll = new ErpHostBLL();
        string GetList(int currPage, int pageSize)
        {
            var list = bll.GetList(s => s.Id > 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = list.Count;
                list = list.OrderByDescending(s => s.Id).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int rowIndex = 1;
                list.ForEach(s =>
                {
                    string state = "未激活";
                    if (s.IsActive ?? true)
                        state = "正常";
                    string provinceName = string.Empty;
                    string cityName = string.Empty;
                    if(!string.IsNullOrWhiteSpace(s.Provinces))
                        provinceName = GetPlace(s.Provinces);
                    if (!string.IsNullOrWhiteSpace(s.Cities))
                        cityName = GetPlace(s.Cities);
                    json.Append("{\"Id\":\"" + s.Id + "\",\"RowIndex\":\"" + (rowIndex++) + "\",\"ClientName\":\"" + s.ClientName + "\",\"ClientNo\":\"" + s.ClientNo + "\",\"ClientHost\":\"" + s.ClientHost + "\",\"ProvinceIds\":\"" + s.Provinces + "\",\"CityIds\":\"" + s.Cities + "\",\"ProvinceName\":\"" + provinceName + "\",\"CityName\":\"" + cityName + "\",\"State\":\"" + state + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }

        string GetModel(int id)
        {
            ErpHost model = bll.GetModel(id);
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                json.Append("{\"Id\":\"" + model.Id + "\",\"ClientName\":\"" + model.ClientName + "\",\"ClientNo\":\"" + model.ClientNo + "\",\"ClientHost\":\"" + model.ClientHost + "\",\"Provinces\":\"" + model.Provinces + "\",\"Cities\":\"" + model.Cities + "\"}");
                return "[" + json.ToString() + "]";
            }
            else
                return "";
        }
        string Edit(string jsonStr)
        {
            string r = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                try
                {
                    ErpHost model = JsonConvert.DeserializeObject<ErpHost>(jsonStr);
                    if (model != null)
                    {
                        if (model.Id > 0)
                        {
                            string msg = CheckExist(model.Id, model.ClientName, model.ClientHost);
                            if (!string.IsNullOrWhiteSpace(msg))
                                r = msg;
                            else
                            {
                                bll.Update(model);
                            }
                        }
                        else
                        {
                            string msg = CheckExist(0, model.ClientName, model.ClientHost);
                            if (!string.IsNullOrWhiteSpace(msg))
                                r = msg;
                            else
                            {
                                model.IsActive = true;
                                bll.Add(model);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    r = ex.Message;
                }
            }

            return r;
        }

        string CheckExist(int id, string name,string host)
        {
            string r=string.Empty;
            var list = bll.GetList(s=>s.Id>0);
            var list1 = list.Where(s => s.ClientName.ToLower() == name.ToLower() && (id > 0 ? (s.Id != id) : 1 == 1));
            if (list1.Any())
                r= "客户端名称重复";
            //else
            //{
            //    var list2 = list.Where(s => s.ClientHost.ToLower() == host.ToLower() && (id > 0 ? (s.Id != id) : 1 == 1));
            //    if (list2.Any())
            //        r = "服务器域名重复";
            //}
            return r;
        }

        List<Place> placeList = new List<Place>();
        string GetPlace(string ids)
        {
            string r = string.Empty;
            if (!string.IsNullOrWhiteSpace(ids))
            {
                if (!placeList.Any())
                {
                    placeList = new PlaceBLL().GetList(s => s.ID > 0);
                }
                if (placeList.Any())
                {
                    List<int> idList = StringHelper.ToIntList(ids,',');
                    var list = placeList.Where(s => idList.Contains(s.ID)).ToList();
                    if (list.Any())
                    {
                        List<string> places = list.Select(s => s.PlaceName).Distinct().ToList();
                        r = StringHelper.ListToString(places);
                    }
                    
                }
            }
            return r;
        }

        string Delete(string ids)
        {
            string r = "删除失败";
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids,',');
                bll.Delete(s=>idList.Contains(s.Id));
                r = "ok";
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
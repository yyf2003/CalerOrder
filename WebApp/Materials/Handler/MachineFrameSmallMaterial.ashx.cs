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
    /// MachineFrameSmallMaterial 的摘要说明
    /// </summary>
    public class MachineFrameSmallMaterial : IHttpHandler
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
                    result = GetMaterialList(currPage, pageSize);
                    break;
                case "getMachineFrame":
                    result=GetMachineFrame();
                    break;
                case "getSmallMaterial":
                    result = GetSmallMaterial();
                    break;
                case "edit":
                    string jsonStr = context.Request["jsonstr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = jsonStr.Replace("+", "%2B");
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    
                    result = Edit(jsonStr);
                    break;
                case "delete":
                    int id = int.Parse(context.Request["Id"]);
                    result = Delete(id);
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetMaterialList(int currPage, int pageSize)
        {
            int customerId = 0;
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            try
            {
                string result = string.Empty;
                var list = (from materialMapp in CurrentContext.DbContext.SmallMaterialMapping
                           join customer in CurrentContext.DbContext.Customer
                           on materialMapp.CustomerId equals customer.Id
                           join material in CurrentContext.DbContext.SmallMaterial
                           on materialMapp.SmallMaterialId equals material.Id
                           where materialMapp.CustomerId == customerId
                           select new {
                               materialMapp,
                               customer.CustomerName,
                               material.SmallMaterialName
                           }).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    int totalCount = list.Count;
                    list = list.OrderBy(s => s.materialMapp.Sheet).ThenBy(s=>s.materialMapp.MachineFrame).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                    int index = 1;
                    list.ForEach(s =>
                    {

                        json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.materialMapp.Id + "\",\"CustomerId\":\"" + s.materialMapp.CustomerId + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"Sheet\":\"" + s.materialMapp.Sheet + "\",\"MachineFrame\":\"" + s.materialMapp.MachineFrame + "\",\"SmallMaterialId\":\"" + s.materialMapp.SmallMaterialId + "\",\"SmallMaterialName\":\"" + s.SmallMaterialName + "\"},");
                        index++;
                    });
                    if (json.Length > 0)
                        result= "{\"total\":" + totalCount + ",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
                    else
                        result= "{\"total\":0,\"rows\":[] }";
                }
                return result;
            }
            catch
            {
                return "{\"total\":0,\"rows\":[] }"; 
            }
        }

        string GetMachineFrame()
        { 
            int customerId = 0;
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            string sheet=string.Empty;
            if (context1.Request.QueryString["sheet"] != null)
            {
                sheet = context1.Request.QueryString["sheet"];
            }
            var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
                        join shop in CurrentContext.DbContext.Shop
                        on frame.ShopId equals shop.Id
                        where shop.CustomerId == customerId && frame.PositionName == sheet
                        select frame.MachineFrame).Distinct().ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"FrameName\":\""+s+"\"},");

                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }

        string GetSmallMaterial()
        {
            var list = new SmallMaterialBLL().GetList(s =>s.Id>0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\""+s.Id+"\",\"SmallMaterialName\":\"" + s.SmallMaterialName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }


        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                SmallMaterialMapping model = JsonConvert.DeserializeObject<SmallMaterialMapping>(jsonStr);
                SmallMaterialMappingBLL bll = new SmallMaterialMappingBLL();
                if (model != null)
                {
                    if (model.Id > 0)
                    {
                        SmallMaterialMapping newModel = bll.GetModel(model.Id);
                        if (newModel != null)
                        {
                            newModel.MachineFrame = model.MachineFrame;
                            newModel.Sheet = model.Sheet;
                            newModel.CustomerId = model.CustomerId;
                            newModel.SmallMaterialId = model.SmallMaterialId;
                            
                            bll.Update(newModel);
                        }
                        else
                        {
                            
                            bll.Add(model);
                        }
                    }
                    else
                    {
                        
                        bll.Add(model);
                    }
                }
            }
            return result;
        }

        string Delete(int id)
        {
            
            SmallMaterialMappingBLL bll = new SmallMaterialMappingBLL();
            SmallMaterialMapping model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                return "ok";
            }
            return "失败";
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
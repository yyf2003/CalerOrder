using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using Newtonsoft.Json;
using System.Text;
using Common;
using DAL;

namespace WebApp.TableSize.handler
{
    /// <summary>
    /// List 的摘要说明
    /// </summary>
    public class List : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
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
                    result = GetList(currPage, pageSize);
                    break;
                case "getFrameList":
                    string sheet = string.Empty;
                    if (context.Request.QueryString["sheet"] != null)
                        sheet = context.Request.QueryString["sheet"];
                    result = GetFrameList(sheet);
                    break;
                case "edit":
                    string jsonStr = context.Request.QueryString["jsonstr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = jsonStr.Replace("+", "%2B");
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    result = Edit(jsonStr);
                    break;
                case "delete":
                    string ids = string.Empty;
                    if (context.Request.QueryString["ids"] != null)
                        ids = context.Request.QueryString["ids"];
                    result = Delete(ids);
                    break;
                case "getMaterialCategory":
                    result = GetMaterialCategory();
                    break;
                case "getOrderMaterial":
                    int customerId = 0;
                    int categoryId = 0;
                    if (context.Request.QueryString["customerId"] != null)
                        customerId = int.Parse(context.Request.QueryString["customerId"]);
                    if (context.Request.QueryString["categoryId"] != null)
                        categoryId = int.Parse(context.Request.QueryString["categoryId"]);
                    result = GetOrderMaterial(categoryId, customerId);
                    break;
            }
            context.Response.Write(result);
        }

        string GetList(int currPage, int pageSize)
        {
            //var list = new TableSizeWithEdgeBLL().GetList(s=>1==1);
            var list = (from table in CurrentContext.DbContext.TableSize
                       join orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                       on table.OrderMaterialId equals orderMaterial.Id
                       join category in CurrentContext.DbContext.MaterialCategory
                       on table.BasicCategoryId equals category.Id
                       
                       select new {
                           table,
                           category.CategoryName,
                           orderMaterial.OrderMaterialName
                       }).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = list.Count;
                list = list.OrderBy(s => s.table.Sheet).ThenBy(s => s.table.MachineFrame).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                list.ForEach(s =>
                {
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.table.Id + "\",\"Sheet\":\"" + s.table.Sheet + "\",\"MachineFrame\":\"" + s.table.MachineFrame + "\",\"NormalLength\":\"" + s.table.NormalLength + "\",\"NormalWidth\":\"" + s.table.NormalWidth + "\",\"WithEdgeLength\":\"" + s.table.WithEdgeLength + "\",\"WithEdgeWidth\":\"" + s.table.WithEdgeWidth + "\",\"Remark\":\"" + s.table.Remark + "\",\"BasicCategoryId\":\"" + s.table.BasicCategoryId + "\",\"OrderMaterialId\":\"" + s.table.OrderMaterialId + "\",\"CategoryName\":\"" + s.CategoryName + "\",\"OrderMaterialName\":\"" + s.OrderMaterialName + "\",\"Quantity\":\"" + s.table.Quantity + "\"},");
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

        string GetFrameList(string sheet)
        {
            var list = new ShopMachineFrameBLL().GetList(s => s.PositionName == sheet).Select(s=>s.MachineFrame).Distinct().OrderBy(s=>s).ToList();
            string result = string.Empty;
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"FrameName\":\""+s+"\"},");
                });
                result = "["+json.ToString().TrimEnd(',')+"]";
            }
            return result;
        }

        TableSizeBLL bll = new TableSizeBLL();
        string Edit(string jsonStr)
        { 
           string result = "ok";
           if (!string.IsNullOrWhiteSpace(jsonStr))
           {
               Models.TableSize model = JsonConvert.DeserializeObject<Models.TableSize>(jsonStr);
               if (model != null)
               {

                   if (model.Id > 0)
                   {
                       var model1 = bll.GetModel(model.Id);
                       if (model1 != null)
                       {
                           model1.Remark = model.Remark;
                           model1.MachineFrame = model.MachineFrame;
                           model1.NormalLength = model.NormalLength;
                           model1.NormalWidth = model.NormalWidth;
                           model1.WithEdgeLength = model.WithEdgeLength;
                           model1.WithEdgeWidth = model.WithEdgeWidth;
                           model1.BasicCategoryId = model.BasicCategoryId;
                           model1.OrderMaterialId = model.OrderMaterialId;
                           model1.Quantity = model.Quantity;
                           bll.Update(model1);
                       }
                       else
                           bll.Add(model);
                   }
                   else
                   {
                       bll.Add(model);
                   }
               }
               else
               {
                   result = "提交失败";
               }
           }
           return result;
        }

        string Delete(string ids)
        {
            string r = "删除失败";
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids, ',');
                bll.Delete(s => idList.Contains(s.Id));
                r = "ok";
            }
            return r;
        }


        string GetMaterialCategory()
        {
            var list = new MaterialCategoryBLL().GetList(s => s.IsDelete == false || s.IsDelete == null);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CategoryName\":\"" + s.CategoryName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetOrderMaterial(int categoryId, int customerId)
        {
            //var list = new OrderMaterialMppingBLL().GetList(s =>s.CustomerId==customerId && s.BasicCategoryId == categoryId).OrderBy(s => s.OrderMaterialName).ToList();
            var list = (from orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                       join cm in CurrentContext.DbContext.CustomerMaterialInfo
                       on orderMaterial.CustomerMaterialId equals cm.Id
                       join basicMaterial in CurrentContext.DbContext.BasicMaterial
                       on cm.BasicMaterialId equals basicMaterial.Id
                       join unit1 in CurrentContext.DbContext.UnitInfo
                       on basicMaterial.UnitId equals unit1.Id into unitTemp
                       from unit in unitTemp.DefaultIfEmpty()
                       select new {
                           orderMaterial.Id,
                           orderMaterial.OrderMaterialName,
                           cm.Price,
                           unit.UnitName

                       }).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"OrderMaterialName\":\"" + s.OrderMaterialName + "\",\"Price\":\"" + s.Price + "\",\"UnitName\":\"" + s.UnitName + "\"},");
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
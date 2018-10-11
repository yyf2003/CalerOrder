using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using System.Text;

namespace WebApp.Materials.Handler
{
    /// <summary>
    /// OrderMaterialMapping 的摘要说明
    /// </summary>
    public class OrderMaterialMapping : IHttpHandler
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
            else if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            else if (context.Request["type"] != null)
            {
                type = context.Request["type"];
            }
            switch (type)
            {
                case "getCustomer":
                    result = GetCustomerList();
                    break;
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
                case "getBasicCategory":
                    result = GetBasicMaterialCategory();
                    break;
                case "getBasicMaterial":
                    int categoryId = int.Parse(context.Request.QueryString["categoryId"]);
                    result = GetBasicMaterial1(categoryId);
                    break;
                case "getUnit":
                    result = GetUnits();
                    break;
                case "edit":
                    string jsonStr = context.Request["jsonstr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    result = Edit(jsonStr);
                    break;
                case "getCustomerMaterial":
                    //int cId = int.Parse(context.Request.QueryString["customerId"]);
                    //int categoryId = int.Parse(context.Request.QueryString["categoryId"]);
                    //result = GetCustomerMaterial(cId, categoryId);
                    break;
                case "delete":
                    int id = int.Parse(context.Request.QueryString["id"]);
                    result = DeleteMaterial(id);
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetCustomerList()
        {
            List<Models.Customer> cusotmerList = new BasePage().GetMyCustomerList();
            if (cusotmerList.Any())
            {
                StringBuilder json = new StringBuilder();
                //if (context1.Request.QueryString["bind"]!=null)
                //   json.Append("{\"CustomerId\":\"0\",\"CustomerName\":\"全部\"},");
                cusotmerList.ForEach(s =>
                {
                    json.Append("{\"CustomerId\":\"" + s.Id + "\",\"CustomerName\":\"" + s.CustomerName + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetCustomerMaterial(int customerId,int categoryId)
        {

            var list = (from customerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                        join basicMaterial in CurrentContext.DbContext.BasicMaterial
                        on customerMaterial.BasicMaterialId equals basicMaterial.Id
                        join unit1 in CurrentContext.DbContext.UnitInfo
                        on basicMaterial.UnitId equals unit1.Id into unitTemp
                        from unit in unitTemp.DefaultIfEmpty()
                        where customerMaterial.CustomerId == customerId && customerMaterial.BasicCategoryId == categoryId
                        select new
                        {
                            customerMaterial,
                            basicMaterial.MaterialName,
                            basicMaterial.UnitId,
                            unit.UnitName
                        }
                     ).ToList();
            StringBuilder json = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.customerMaterial.Id + "\",\"BasicMaterialId\":\""+s.customerMaterial.BasicMaterialId+"\",\"CustomerMaterialName\":\"" + s.MaterialName + "\",\"UnitId\":\""+(s.UnitId??0)+"\",\"Unit\":\""+s.UnitName+"\",\"Price\":\""+(s.customerMaterial.Price??0)+"\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
        }

        string GetBasicMaterial1(int categoryId)
        {
            int customerId = 0;
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            var list = (from basicMaterial in CurrentContext.DbContext.BasicMaterial
                       join unit in CurrentContext.DbContext.UnitInfo
                       on basicMaterial.UnitId equals unit.Id
                       where basicMaterial.MaterialCategoryId == categoryId
                       && (basicMaterial.IsDelete == null || basicMaterial.IsDelete==false)
                       && basicMaterial.CustomerId == customerId
                       select new {
                           basicMaterial.UnitId,
                           basicMaterial.Id,
                           basicMaterial.MaterialName,
                           unit.UnitName
                       }).ToList();
            StringBuilder json = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"BasicMaterialName\":\"" + s.MaterialName + "\",\"UnitId\":\"" + s.UnitId + "\",\"UnitName\":\"" + s.UnitName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
        }

        string GetBasicMaterialCategory()
        {
            var list = new MaterialCategoryBLL().GetList(s => s.Id > 0);
            StringBuilder json = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CategoryName\":\"" + s.CategoryName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
        }

        string GetBasicMaterial(int categoryId)
        {
            //var list = new BasicMaterialBLL().GetList(s => s.MaterialCategoryId == categoryId && (s.IsDelete == null || s.IsDelete == false));
            var list = (from bm in CurrentContext.DbContext.BasicMaterial
                       join category in CurrentContext.DbContext.MaterialCategory
                       on bm.MaterialCategoryId equals category.Id
                       join unit1 in CurrentContext.DbContext.UnitInfo
                       on bm.UnitId equals unit1.Id into unitTemp
                       from unit in unitTemp.DefaultIfEmpty()
                       select new {
                           bm,
                           category,
                           unit
                       }).OrderBy(s=>s.bm.MaterialName).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.bm.Id + "\",\"UnitId\":\"" + s.bm.UnitId + "\",\"UnitName\":\"" + s.unit.UnitName + "\",\"MaterialName\":\"" + s.bm.MaterialName + "\",\"CategoryId\":\"" + s.bm.MaterialCategoryId + "\",\"CategoryName\":\"" + s.category.CategoryName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";

        }

        string GetUnits()
        {
            var list = new UnitInfoBLL().GetList(s => s.Id > 0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"UnitName\":\"" + s.UnitName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetMaterialList(int currPage, int pageSize)
        {
            int customerId = 0;
            int categoryId = 0;
            string orderMaterialName = string.Empty;
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            if (context1.Request.QueryString["categoryId"] != null)
            {
                categoryId = int.Parse(context1.Request.QueryString["categoryId"]);
            }
            if (context1.Request.QueryString["orderMaterialName"] != null)
            {
                orderMaterialName = context1.Request.QueryString["orderMaterialName"];
            }
            
            var list=(from orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                      join customer in CurrentContext.DbContext.Customer
                         on orderMaterial.CustomerId equals customer.Id
                        join category in CurrentContext.DbContext.MaterialCategory
                        on orderMaterial.BasicCategoryId equals category.Id
                        join basicMaterial in CurrentContext.DbContext.BasicMaterial
                        on orderMaterial.BasicMaterialId equals basicMaterial.Id
                        where orderMaterial.CustomerId == customerId
                        select new
                        {
                            orderMaterial,
                            customer.CustomerName,
                            category.CategoryName,
                            basicMaterial.MaterialName
                            
                        }).ToList();
            if (categoryId > 0)
            {
                list = list.Where(s => s.orderMaterial.BasicCategoryId == categoryId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(orderMaterialName))
            {
                list = list.Where(s => s.orderMaterial.OrderMaterialName.ToLower().Contains(orderMaterialName.ToLower())).ToList();
            }
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = list.Count;
                list = list.OrderBy(s => s.orderMaterial.BasicCategoryId).ThenBy(s=>s.orderMaterial.OrderMaterialName).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                list.ForEach(s =>
                {
                    string state = s.orderMaterial.IsDelete != null && s.orderMaterial.IsDelete == true ? "已删除" : "正常";
                    json.Append("{\"rowIndex\":\"" + index + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"Id\":\"" + s.orderMaterial.Id + "\",\"OrderMaterialName\":\"" + s.orderMaterial.OrderMaterialName + "\",\"State\":\"" + state + "\",\"BasicCategoryId\":\"" + s.orderMaterial.BasicCategoryId + "\",\"CategoryName\":\"" + s.CategoryName + "\",\"BasicMaterialName\":\"" + s.MaterialName + "\",\"BasicMaterialId\":\"" + s.orderMaterial.BasicMaterialId + "\"},");
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

        OrderMaterialMppingBLL bll = new OrderMaterialMppingBLL();
        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {

                OrderMaterialMpping model = JsonConvert.DeserializeObject<OrderMaterialMpping>(jsonStr);
                if (model != null)
                {
                    if (model.Id > 0)
                    {
                        if (!CheckExist(model.Id, model.CustomerId ?? 1, model.OrderMaterialName))
                        {
                            OrderMaterialMpping newModel = bll.GetModel(model.Id);
                            if (newModel != null)
                            {
                                newModel.BasicMaterialId = model.BasicMaterialId;
                                newModel.BasicCategoryId = model.BasicCategoryId;
                                //newModel.CustomerId = model.CustomerId;
                                newModel.OrderMaterialName = model.OrderMaterialName;
                                bll.Update(newModel);
                            }
                            else
                            {
                                
                                model.IsDelete = false;
                                bll.Add(model);
                            }
                        }
                        else
                            result = "exist";
                    }
                    else
                    {
                        if (!CheckExist(0,model.CustomerId??1, model.OrderMaterialName))
                        {
                            //model.OrderMaterialName = DateTime.Now;
                            //model.AddUserId = new BasePage().CurrentUser.UserId;
                            model.IsDelete = false;
                            bll.Add(model);
                        }
                        else
                            result = "exist";
                    }
                }
            }
            return result;
        }

        bool CheckExist(int id,int customerId, string name)
        {
            var list = bll.GetList(s =>s.CustomerId==customerId && s.OrderMaterialName.ToLower() == name.ToLower() && (id > 0 ? (s.Id != id) : 1 == 1));
            return list.Any();
        }

        string DeleteMaterial(int id)
        {
            string result = "error";
            OrderMaterialMpping model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                result = "ok";
            }
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
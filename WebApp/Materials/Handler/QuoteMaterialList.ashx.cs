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
    /// QuoteMaterialList 的摘要说明
    /// </summary>
    public class QuoteMaterialList : IHttpHandler
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
                //case "getCustomer":
                //    result = GetCustomerList();
                //    break;
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
                //case "getBasicCategory":
                //    result = GetBasicMaterialCategory();
                //    break;
                case "getBasicMaterial":
                    int categoryId = int.Parse(context.Request.QueryString["categoryId"]);
                    int customerId = int.Parse(context.Request.QueryString["customerId"]);
                    int isAdd = int.Parse(context.Request.QueryString["isAdd"]);
                    result = GetBasicMaterial(categoryId, customerId, isAdd);
                    break;
                //case "getUnit":
                //    result = GetUnits();
                //    break;
                case "edit":
                    string jsonStr = context.Request["jsonstr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = jsonStr.Replace("+", "%2B");
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    result = Edit(jsonStr);
                    break;
                //case "getCustomerMaterial":
                //    int cId = int.Parse(context.Request.QueryString["customerId"]);
                //    int categoryId = int.Parse(context.Request.QueryString["categoryId"]);
                //    result = GetCustomerMaterial(cId, categoryId);
                //    break;
                case "delete":
                    int id = int.Parse(context.Request.QueryString["id"]);
                    result = DeleteMaterial(id);
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetMaterialList(int currPage, int pageSize)
        {
            int customerId = 0;
            string quoteMaterialName = string.Empty;
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            if (context1.Request.QueryString["searchQuoteMaterialName"] != null)
            {
                quoteMaterialName = context1.Request.QueryString["searchQuoteMaterialName"];
            }
            try
            {
                //var list = (from qm in CurrentContext.DbContext.QuoteMaterial
                //            join customer in CurrentContext.DbContext.Customer
                //            on qm.CustomerId equals customer.Id
                //            join cm in CurrentContext.DbContext.CustomerMaterialInfo
                //            on qm.CustomerMaterialId equals cm.Id
                //            join category in CurrentContext.DbContext.MaterialCategory
                //            on qm.BasicCategoryId equals category.Id
                //            join bm in CurrentContext.DbContext.BasicMaterial
                //            on cm.BasicMaterialId equals bm.Id
                //            join unit1 in CurrentContext.DbContext.UnitInfo
                //            on bm.UnitId equals unit1.Id into unitTemp
                //            from unit in unitTemp.DefaultIfEmpty()
                //            where qm.CustomerId == customerId
                //            select new
                //            {
                //                qm,//订单报价材质
                //                customer.CustomerName,//客户名称
                //                bm.UnitId,//单位Id
                //                unit.UnitName,//单位名称
                //                CustomerMaterialName= bm.MaterialName,//客户材质名称
                //                category.CategoryName,//材质类别
                //                cm.Price//客户材质报价
                //            }).ToList();
                var list = (from qm in CurrentContext.DbContext.QuoteMaterial
                           join bm in CurrentContext.DbContext.BasicMaterial
                           on qm.CustomerMaterialId equals bm.Id
                           join customer in CurrentContext.DbContext.Customer
                           on qm.CustomerId equals customer.Id
                           join category in CurrentContext.DbContext.MaterialCategory
                           on qm.BasicCategoryId equals category.Id
                           join unit1 in CurrentContext.DbContext.UnitInfo
                           on bm.UnitId equals unit1.Id into unitTemp
                            from unit in unitTemp.DefaultIfEmpty()
                           where qm.CustomerId == customerId
                           select new {
                               qm,
                               bm,
                               CustomerName = customer.CustomerName,
                               unit.UnitName,
                               category.CategoryName,//材质类别
                           }).ToList();
                if (!string.IsNullOrWhiteSpace(quoteMaterialName))
                {
                    list = list.Where(s => s.qm.QuoteMaterialName.ToLower().Contains(quoteMaterialName.ToLower())).ToList();
                }
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    int totalCount = list.Count;
                    list = list.OrderBy(s => s.qm.BasicCategoryId).ThenBy(s=>s.bm.MaterialName).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                    int index = 1;
                    list.ForEach(s =>
                    {
                        //string state = s.cm.IsDelete != null && s.cm.IsDelete == true ? "已删除" : "正常";
                        json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.qm.Id + "\",\"QuoteMaterialName\":\"" + s.qm.QuoteMaterialName + "\",\"CustomerId\":\"" + s.qm.CustomerId + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"UnitId\":\"" + s.bm.UnitId + "\",\"Unit\":\"" + s.UnitName + "\",\"CustomerMaterialId\":\""+s.qm.CustomerMaterialId+"\",\"CustomerMaterialName\":\"" + s.bm.MaterialName + "\",\"BasicCategoryId\":\"" + s.qm.BasicCategoryId + "\",\"BasicCategoryName\":\"" + s.CategoryName + "\"},");
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
            catch
            {
                return "";
            }
            

        }
        QuoteMaterialBLL bll = new QuoteMaterialBLL();
        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                
                QuoteMaterial model = JsonConvert.DeserializeObject<QuoteMaterial>(jsonStr);
                if (model != null)
                {
                    if (model.Id > 0)
                    {
                        QuoteMaterial newModel = bll.GetModel(model.Id);
                        if (newModel != null)
                        {
                            //if (!CheckMaterialName(newModel.Id, model.CustomerId ?? 0, model.QuoteMaterialName))
                            //{
                                
                            //}
                            //else
                            //    result = "exist";
                            newModel.BasicCategoryId = model.BasicCategoryId;
                            newModel.CustomerId = model.CustomerId;
                            newModel.CustomerMaterialId = model.CustomerMaterialId;
                            newModel.QuoteMaterialName = model.QuoteMaterialName;
                            bll.Update(newModel);
                        }
                        else
                        {
                            //if(!CheckMaterialName(0, model.CustomerId ?? 0, model.QuoteMaterialName))
                            //{
                                
                            //}
                            //else
                            //    result = "exist";
                            bll.Add(model);
                        }
                    }
                    else
                    {

                        //if (!CheckMaterialName(0, model.CustomerId ?? 0, model.QuoteMaterialName))
                        //{
                            
                        //}
                        //else
                        //    result = "exist";
                        bll.Add(model);
                    }
                }
            }
            return result;
        }

        bool CheckMaterialName(int id,int customerId, string materialName)
        {
            var list = bll.GetList(s => s.CustomerId == customerId && s.QuoteMaterialName.ToLower() == materialName.ToLower() && (id>0?(s.Id!=id):1==1));
            return list.Any();
        }

        string DeleteMaterial(int id)
        {
            QuoteMaterial model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                return "ok";
            }
            else
                return "error";
        }


        string GetBasicMaterial(int categoryId, int customerId, int isAdd)
        {
            List<int> QuoteMaterialIdList = new QuoteMaterialBLL().GetList(s => s.CustomerId == customerId).Select(s=>s.CustomerMaterialId??0).ToList();
            var list = (from basicMaterial in CurrentContext.DbContext.BasicMaterial
                        join unit in CurrentContext.DbContext.UnitInfo
                        on basicMaterial.UnitId equals unit.Id
                        where basicMaterial.MaterialCategoryId == categoryId
                        && (basicMaterial.IsDelete == null || basicMaterial.IsDelete == false)
                        
                        select new
                        {
                            basicMaterial.UnitId,
                            basicMaterial.Id,
                            basicMaterial.MaterialName,
                            unit.UnitName
                        }).ToList();
            if (isAdd == 1)
            {
                list = list.Where(s => !QuoteMaterialIdList.Contains(s.Id)).ToList();
            }
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using System.Text;
using Newtonsoft.Json;

namespace WebApp.Materials.Handler
{
    /// <summary>
    /// Material 的摘要说明
    /// </summary>
    public class Material : IHttpHandler
    {
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
                case "getBigType":
                    result = GetType(0);
                    break;
                case "getSmallType":
                    int parentId = int.Parse(context.Request["parentId"]);
                    result = GetType(parentId);
                    break;
                case "getBrand":
                    result = GetBrandList();
                    break;
                case "edit":
                    string jsonString = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = AddMaterial(jsonString, optype);
                    break;
                case "getMaterialBySmallType":
                    int typeId = int.Parse(context.Request["typeId"]);
                    int pageSize = int.Parse(context.Request["pageSize"]);
                    int currPage = int.Parse(context.Request["currPage"]);
                    result = GetMaterialBySmallType(typeId, currPage, pageSize);
                    break;
                case "getMaterialByType":
                    int bigTypeId = int.Parse(context.Request["bigTypeId"]);
                    int smallTypeId = int.Parse(context.Request["smallTypeId"]);
                    int pageSize1 = int.Parse(context.Request["pageSize"]);
                    int currPage1 = int.Parse(context.Request["currPage"]);
                    result = GetMaterialByType(bigTypeId, smallTypeId, currPage1, pageSize1);
                    break;
                case "getCategory":
                    result = GetCategory();
                    break;
            }
            context.Response.Write(result);
        }

        string GetType(int parentId)
        {
            MaterialTypeBLL materialTypeBll = new MaterialTypeBLL();
            var list = materialTypeBll.GetList(s => s.ParentId == parentId);
            StringBuilder Json = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    Json.Append("{\"Id\":\"" + s.Id + "\",\"TypeName\":\"" + s.MaterialTypeName + "\"");
                    Json.Append("},");
                });
                return "[" + Json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetBrandList()
        {
            var list = new MaterialBrandBLL().GetList(s=>1==1);
            if (list.Any())
            {
                StringBuilder Json = new StringBuilder();
                list.ForEach(s => {
                    Json.Append("{\"Id\":\"" + s.Id + "\",\"BrandName\":\"" + s.BrandName + "\"");
                    Json.Append("},");
                });
                return "[" + Json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }
        MaterialBLL materialBll = new MaterialBLL();
        string AddMaterial(string jsonString, string optype)
        {
           
            string result = "提交失败！";
            if (!string.IsNullOrEmpty(jsonString))
            {
                Models.Material model = JsonConvert.DeserializeObject<Models.Material>(jsonString);
                if (model != null)
                {
                    if (optype == "add")
                    {
                        if (!CheckMaterial(model, 0))
                        {
                            model.AddDate = DateTime.Now;
                            model.IsDelete = false;
                            materialBll.Add(model);
                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                    else
                    {
                        if (!CheckMaterial(model, model.Id))
                        {
                            materialBll.Update(model);
                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                }
            }
            return result;
            
        }

        bool CheckName(string name,int id)
        {
            var list = materialBll.GetList(s=>s.MaterialName==name && (id>0?s.Id!=id:1==1));
            return list.Any();
        }

        bool CheckMaterial(Models.Material model,int id)
        {
            var list = materialBll.GetList(s => s.BigTypeId == model.BigTypeId && s.SmallTypeId == model.SmallTypeId && s.MaterialCategoryId == model.MaterialCategoryId && s.MaterialBrandId == model.MaterialBrandId && s.Specification == model.Specification && (id > 0 ? s.Id != id : 1 == 1));
            return list.Any();
        }


        string GetMaterialBySmallType(int typeId,int currPage,int pageSize)
        {
            var list = (from material in CurrentContext.DbContext.Material
                       join type in CurrentContext.DbContext.MaterialType
                       on material.SmallTypeId equals type.Id
                       join brand in CurrentContext.DbContext.MaterialBrand
                       on material.MaterialBrandId equals brand.Id
                       where material.SmallTypeId == typeId
                       select new {
                           material.Id,
                           material.Area,
                           material.Length,
                           material.MaterialName,
                           material.Unit,
                           material.Width,
                           type.MaterialTypeName,
                           brand.BrandName
                       }).ToList();
            int total = list.Count;
            if (list.Any())
            {
                list = list.OrderBy(s => s.Id).Skip((currPage) * pageSize).Take(pageSize).ToList();
                StringBuilder Json = new StringBuilder();
                //Json.Append("{\"total\":\"" + total + "\",\"rows\":[");
                list.ForEach(s =>
                {
                    Json.Append("{\"total\":\"" + total + "\",\"MaterialName\":\"" + s.MaterialName + "\",\"MaterialTypeName\":\"" + s.MaterialTypeName + "\",\"BrandName\":\"" + s.BrandName + "\",\"Width\":\"" + s.Width + "\",\"Length\":\"" + s.Length + "\",\"Area\":\"" + s.Area + "\",\"Unit\":\"" + s.Unit + "\"");
                    Json.Append("},");
                    //Json.Append("{MaterialName:'" + s.MaterialName + "',MaterialTypeName:'" + s.MaterialTypeName + "',BrandName:'" + s.BrandName + "',Width:" + s.Width + ",Length:" + s.Length + ",Area:" + s.Area + ",Unit:'" + s.Unit + "'");
                    //Json.Append("},");
                });
                //return "{total:" + total + ",rows:["+Json.ToString().TrimEnd(',') + "]}";
                return "[" + Json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetMaterialByType(int bitTypeId, int samllTypeId, int currPage, int pageSize)
        {
            var list = (from material in CurrentContext.DbContext.Material
                        join brand in CurrentContext.DbContext.MaterialBrand
                        on material.MaterialBrandId equals brand.Id
                        join bigType in CurrentContext.DbContext.MaterialType
                        on material.BigTypeId equals bigType.Id
                        join smallType in CurrentContext.DbContext.MaterialType
                        on material.SmallTypeId equals smallType.Id
                        where material.BigTypeId == bitTypeId && (samllTypeId>0?(material.SmallTypeId==samllTypeId):1==1)
                        select new
                        {
                            material.Id,
                            material.Area,
                            material.Length,
                            BigTypeName=bigType.MaterialTypeName,
                            SmallTypeName = smallType.MaterialTypeName,
                            material.MaterialName,
                            material.Unit,
                            material.Width,
                            brand.BrandName
                        }).ToList();
            int total = list.Count;
            if (list.Any())
            {
                list = list.OrderBy(s => s.Id).Skip((currPage) * pageSize).Take(pageSize).ToList();
                StringBuilder Json = new StringBuilder();
                
                list.ForEach(s =>
                {
                    Json.Append("{\"total\":\"" + total + "\",\"MaterialId\":\""+s.Id+"\",\"MaterialName\":\"" + s.MaterialName + "\",\"BigTypeName\":\"" + s.BigTypeName + "\",\"SmallTypeName\":\"" + s.SmallTypeName + "\",\"BrandName\":\"" + s.BrandName + "\",\"Width\":\"" + s.Width + "\",\"Length\":\"" + s.Length + "\",\"Area\":\"" + s.Area + "\",\"Unit\":\"" + s.Unit + "\"");
                    Json.Append("},");
                   
                });
               
                return "[" + Json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetCategory()
        {
            var list = new MaterialCategoryBLL().GetList(s=>s.IsDelete==false || s.IsDelete==null);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CategoryName\":\"" + s.CategoryName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Newtonsoft.Json;
using System.Text;

namespace WebApp.Materials.Handler
{
    /// <summary>
    /// SetPrice 的摘要说明
    /// </summary>
    public class SetPrice : IHttpHandler
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
                case "getCustomer":
                    result = GetCustomerList();
                    break;
                case "getRegion":
                    int customerId = int.Parse(context.Request["customerId"]);
                    result = GetRegion(customerId);
                    break;
                case "getPrice":
                    int priceId = int.Parse(context.Request["priceId"]);
                    result = GetPrice(priceId);
                    break;
                case "edit":
                    string jsonString = context.Request["jsonString"];
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        jsonString = HttpUtility.UrlDecode(jsonString);
                    }
                    string optype = context.Request["optype"];
                    result = AddPrice(jsonString, optype);
                    break;
                case "getMaterialCategory":
                    int pageSize = int.Parse(context.Request["pageSize"]);
                    int currPage = int.Parse(context.Request["currPage"]);
                    result = GetMaterialCategory(currPage, pageSize);
                    break;
            }
            context.Response.Write(result);
        }

        string GetPrice(int id)
        {

            //var model = (from price in CurrentContext.DbContext.CustomerMaterial
            //             join type in CurrentContext.DbContext.MaterialType
            //             on price.Id equals type.Id
            //             where price.Id == id
            //             select new
            //             {
            //                 //price.CustomerId,
            //                 //price.RegionId,
            //                 //BigTypeId = type.ParentId ?? 0,
            //                 //SmallTypeId = price.MaterialTypeId,
            //                 //price.SalePrice,
            //                 //price.CostPrice
            //             }).FirstOrDefault();
            Models.CustomerMaterial model = materialPriceBll.GetModel(id);
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                json.Append("{\"CustomerId\":\"" + model.CustomerId + "\",\"RegionId\":\"" + model.RegionId + "\",\"RegionName\":\"" + model.RegionName + "\",\"CustomerMaterialName\":\""+model.CustomerMaterialName+"\",\"BigTypeId\":\"" + model.BigTypeId + "\",\"CostPrice\":\"" + model.CostPrice + "\",\"SalePrice\":\"" + model.SalePrice + "\"");
                var detailList = detailBll.GetList(s=>s.CustomerMaterialId==id);
                if (detailList.Any())
                {
                    StringBuilder detailJson = new StringBuilder();
                    detailList.ForEach(d => {
                        detailJson.Append("{\"MarterialCategoryId\":\"" + d.MarterialCategoryId + "\",\"MarterialCategoryName\":\"" + d.MarterialCategoryName + "\"},");
                    });
                    json.Append(",\"Materials\":[" + detailJson.ToString().TrimEnd(',') + "]");
                }
                json.Append("}");
                return "[" + json.ToString() + "]";
            }
            return "";
        }


        string GetCustomerList()
        {
            var list = new BasePage().GetMyCustomerList();
            if (list.Any())
            {
                StringBuilder Json = new StringBuilder();
                list.ForEach(s =>
                {
                    Json.Append("{\"Id\":\"" + s.Id + "\",\"CustomerName\":\"" + s.CustomerName + "\"");
                    Json.Append("},");
                });
                return "[" + Json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetRegion(int customerId)
        {
            var list = new RegionBLL().GetList(s => s.CustomerId == customerId);
            if (list.Any())
            {
                StringBuilder Json = new StringBuilder();
                list.ForEach(s =>
                {
                    Json.Append("{\"Id\":\"" + s.Id + "\",\"RegionName\":\"" + s.RegionName + "\"");
                    Json.Append("},");
                });
                return "[" + Json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }
        CustomerMaterialBLL materialPriceBll = new CustomerMaterialBLL();
        CustomerMaterialDetailBLL detailBll = new CustomerMaterialDetailBLL();
        string AddPrice(string jsonString, string optype)
        {

            string result = "提交失败！";
            if (!string.IsNullOrEmpty(jsonString))
            {
                Models.CustomerMaterial model = JsonConvert.DeserializeObject<Models.CustomerMaterial>(jsonString);
                if (model != null)
                {
                    List<Models.CustomerMaterialDetail> details = model.Materials;
                    if (optype == "add")
                    {
                        if (!CheckPrice(model, 0))
                        {
                            model.AddDate = DateTime.Now;
                            model.IsDelete = false;
                            model.AddUserId = new BasePage().CurrentUser.UserId;
                            materialPriceBll.Add(model);

                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                    else
                    {
                        if (!CheckPrice(model, 1))
                        {
                            materialPriceBll.Update(model);
                            result = "ok";
                        }
                        else
                            result = "exist";
                    }
                    CustomerMaterialDetailBLL detailBll = new CustomerMaterialDetailBLL();
                    Models.CustomerMaterialDetail detailModel;
                    detailBll.Delete(s=>s.CustomerMaterialId==model.Id);
                    details.ForEach(s => {
                        detailModel = new Models.CustomerMaterialDetail {CustomerMaterialId=model.Id,MarterialCategoryId=s.MarterialCategoryId,MarterialCategoryName=s.MarterialCategoryName };
                        detailBll.Add(detailModel);
                    });
                }
            }
            return result;

        }


        bool CheckPrice(Models.CustomerMaterial model, int id)
        {
            var list = materialPriceBll.GetList(s => s.CustomerId == model.CustomerId && s.RegionId == model.RegionId && s.CustomerMaterialName==model.CustomerMaterialName && (id > 0 ? s.Id != model.Id : 1 == 1));
            return list.Any();
            //return false;
        }

        string GetMaterialCategory(int currPage, int pageSize)
        {
            var list = new MaterialCategoryBLL().GetList(s=>s.Id>0);
            int total = list.Count;
            if (list.Any())
            {
                list = list.OrderBy(s => s.Id).Skip((currPage) * pageSize).Take(pageSize).ToList();
                StringBuilder Json = new StringBuilder();

                list.ForEach(s =>
                {
                    Json.Append("{\"total\":\"" + total + "\",\"CategoryId\":\"" + s.Id + "\",\"CategoryName\":\"" + s.CategoryName + "\"");
                    Json.Append("},");

                });

                return "[" + Json.ToString().TrimEnd(',') + "]";
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
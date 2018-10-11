using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using BLL;
using Models;
using System.Text;
using Newtonsoft.Json;

namespace WebApp.Outsource.handler
{
    /// <summary>
    /// PriceList 的摘要说明
    /// </summary>
    public class PriceList : IHttpHandler
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
            switch (type)
            { 
                case "getList":
                    result = GetMaterialList();
                    break;
                case "edit":
                    result = Edit();
                    break;
                case "addBatch":
                    result = BatchAddPrice();
                    break;
                case "delete":
                    result = Delete();
                    break;
            }
            context.Response.Write(result);
        }

        

        string GetMaterialList_old()
        {
            int currPage=0, pageSize=0;
            int companyId = 0;
            string materialName = string.Empty;
            if (context1.Request.QueryString["currpage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            if (context1.Request.QueryString["companyId"] != null)
            {
                companyId = int.Parse(context1.Request.QueryString["companyId"]);
            }
            if (context1.Request.QueryString["materialName"] != null)
            {
                materialName = context1.Request.QueryString["materialName"];
            }
            try
            {
                var list = (from cm in CurrentContext.DbContext.OutsourceMaterialPrice
                            join company in CurrentContext.DbContext.Company
                            on cm.OutsourceId equals company.Id
                            join bm in CurrentContext.DbContext.BasicMaterial
                            on cm.BasicMaterialId equals bm.Id
                            join category in CurrentContext.DbContext.MaterialCategory
                            on cm.BasicCategoryId equals category.Id
                            join unit1 in CurrentContext.DbContext.UnitInfo
                            on bm.UnitId equals unit1.Id into unitTemp
                            from unit in unitTemp.DefaultIfEmpty()
                            where cm.OutsourceId == companyId
                            select new
                            {
                                cm,
                                company.CompanyName,
                                unit.UnitName,
                                category.CategoryName,
                                bm
                            }).ToList();
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    list = list.Where(s => s.bm.MaterialName.ToLower().Contains(materialName.ToLower())).ToList();
                }
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    int totalCount = list.Count;
                    list = list.OrderBy(s => s.cm.BasicCategoryId).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                    int index = 1;
                    list.ForEach(s =>
                    {

                        json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.cm.Id + "\",\"CompanyId\":\"" + s.cm.OutsourceId + "\",\"CompanyName\":\"" + s.CompanyName + "\",\"UnitId\":\"" + s.bm.UnitId + "\",\"Unit\":\"" + s.UnitName + "\",\"InstallPrice\":\"" + s.cm.InstallPrice + "\",\"SendPrice\":\""+s.cm.SendPrice+"\",\"BasicMaterialName\":\"" + s.bm.MaterialName + "\",\"BasicMaterialId\":\"" + s.cm.BasicMaterialId + "\",\"BasicCategoryId\":\"" + s.cm.BasicCategoryId + "\",\"BasicCategoryName\":\"" + s.CategoryName + "\"},");
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

        string GetMaterialList()
        {
            //int currPage = 0, pageSize = 0;
            int companyId = 0;
            int customerId = 0;
            int priceItemId = 0;
            string materialName = string.Empty;
            //if (context1.Request.QueryString["currpage"] != null)
            //{
            //    currPage = int.Parse(context1.Request.QueryString["currpage"]);
            //}
            //if (context1.Request.QueryString["pagesize"] != null)
            //{
            //    pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            //}
            if (context1.Request.QueryString["companyId"] != null)
            {
                companyId = int.Parse(context1.Request.QueryString["companyId"]);
            }
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            if (context1.Request.QueryString["priceItemId"] != null)
            {
                priceItemId = int.Parse(context1.Request.QueryString["priceItemId"]);
            }
            if (context1.Request.QueryString["materialName"] != null)
            {
                materialName = context1.Request.QueryString["materialName"];
            }
            try
            {

                var list = (from cm in CurrentContext.DbContext.OutsourceMaterialInfo
                            join company in CurrentContext.DbContext.Company
                            on cm.OutsourctId equals company.Id
                            join bm in CurrentContext.DbContext.BasicMaterial
                            on cm.BasicMaterialId equals bm.Id
                            join category in CurrentContext.DbContext.MaterialCategory
                            on cm.BasicCategoryId equals category.Id
                            join unit1 in CurrentContext.DbContext.UnitInfo
                            on bm.UnitId equals unit1.Id into unitTemp
                            from unit in unitTemp.DefaultIfEmpty()
                            where cm.OutsourctId == companyId
                            && cm.CustomerId == customerId
                            && cm.PriceItemId == priceItemId
                            select new
                            {
                                cm,
                                company.CompanyName,
                                unit.UnitName,
                                category.CategoryName,
                                bm
                            }).OrderBy(s => s.cm.BasicCategoryId).ToList();
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    list = list.Where(s => s.bm.MaterialName.ToLower().Contains(materialName.ToLower())).ToList();
                }
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    int totalCount = list.Count;
                    //list = list.OrderBy(s => s.cm.BasicCategoryId).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                    int index = 1;
                    list.ForEach(s =>
                    {

                        json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.cm.Id + "\",\"CompanyId\":\"" + s.cm.OutsourctId + "\",\"CompanyName\":\"" + s.CompanyName + "\",\"UnitId\":\"" + s.bm.UnitId + "\",\"Unit\":\"" + s.UnitName + "\",\"InstallPrice\":\"" + s.cm.InstallPrice + "\",\"SubInstallPrice\":\"" + s.cm.SubInstallPrice + "\",\"SendPrice\":\"" + s.cm.SendPrice + "\",\"BasicMaterialName\":\"" + s.bm.MaterialName + "\",\"BasicMaterialId\":\"" + s.cm.BasicMaterialId + "\",\"BasicCategoryId\":\"" + s.cm.BasicCategoryId + "\",\"BasicCategoryName\":\"" + s.CategoryName + "\"},");
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

        string Edit()
        {
            string result = "提交失败！";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    try
                    {
                        OutsourceMaterialInfoBLL bll = new OutsourceMaterialInfoBLL();
                        //jsonStr = jsonStr.Replace("+", "%2B");
                        //jsonStr = HttpUtility.UrlDecode(jsonStr);
                        OutsourceMaterialInfo model = JsonConvert.DeserializeObject<OutsourceMaterialInfo>(jsonStr);
                        if (model != null)
                        {
                            if (model.Id > 0)
                            {
                                //更新
                                OutsourceMaterialInfo newModel = bll.GetModel(model.Id);
                                if (newModel != null)
                                {
                                    newModel.BasicCategoryId = model.BasicCategoryId;
                                    newModel.BasicMaterialId = model.BasicMaterialId;
                                    newModel.InstallPrice = model.InstallPrice;
                                    newModel.SubInstallPrice = model.SubInstallPrice;
                                    newModel.SendPrice = model.SendPrice;
                                    newModel.UnitId = model.UnitId;
                                    bll.Update(newModel);
                                    result = "ok";
                                }
                                else
                                {
                                    //newModel.BasicCategoryId = model.BasicCategoryId;
                                    //newModel.BasicMaterialId = model.BasicMaterialId;
                                    //newModel.Price = model.Price;
                                    //newModel.OutsourceId = model.OutsourceId;
                                    //newModel.AddDate = DateTime.Now;
                                    //newModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    //bll.Add(newModel);
                                }
                                
                            }
                            else
                            {
                                //新增
                                var list = bll.GetList(s=>s.OutsourctId==model.OutsourctId && s.PriceItemId==model.PriceItemId && s.CustomerId==model.CustomerId && s.BasicMaterialId==model.BasicMaterialId);
                                if (list.Any())
                                {
                                    result = "此材质报价已经存在！";
                                }
                                else
                                {
                                    model.AddDate = DateTime.Now;
                                    model.AddUserId = new BasePage().CurrentUser.UserId;
                                    model.IsDelete = false;
                                    bll.Add(model);
                                    result = "ok";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = "提交失败："+ex.Message;
                    }
                }
            }


            return result;
        }

        string Delete()
        {
            string result = "删除失败！";
            string ids = string.Empty;
            if (context1.Request.Form["ids"] != null)
            {
                ids = context1.Request.Form["ids"];
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    try
                    {
                        ids = ids.TrimEnd(',');
                        List<int> idList = Common.StringHelper.ToIntList(ids, ',');
                        new OutsourceMaterialPriceBLL().Delete(s => idList.Contains(s.Id));
                        result = "ok";
                    }
                    catch (Exception ex)
                    {
                        result = "删除失败："+ex.Message;
                    }
                }
            }
            return result;
        }

        string BatchAddPrice()
        {
            string result = "提交失败！";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    try
                    {
                        OutsourceMaterialInfoBLL bll = new OutsourceMaterialInfoBLL();
                        //jsonStr = jsonStr.Replace("+", "%2B");
                        //jsonStr = HttpUtility.UrlDecode(jsonStr);
                        List<OutsourceMaterialInfo> submitList = JsonConvert.DeserializeObject<List<OutsourceMaterialInfo>>(jsonStr);
                        if (submitList.Any())
                        {
                            submitList.ForEach(s => {
                                var oldModel = bll.GetList(p=>p.OutsourctId==s.OutsourctId && p.PriceItemId==s.PriceItemId && p.CustomerId==s.CustomerId && p.BasicMaterialId==s.BasicMaterialId).FirstOrDefault();
                                if (oldModel != null)
                                {
                                    oldModel.SubInstallPrice = s.SubInstallPrice;
                                    oldModel.InstallPrice = s.InstallPrice;
                                    oldModel.SendPrice = s.SendPrice;
                                    bll.Update(oldModel);
                                }
                                else
                                {
                                    OutsourceMaterialInfo model = new OutsourceMaterialInfo();
                                    model.AddDate = DateTime.Now;
                                    model.AddUserId = new BasePage().CurrentUser.UserId;
                                    model.BasicCategoryId = s.BasicCategoryId;
                                    model.BasicMaterialId = s.BasicMaterialId;
                                    model.CustomerId = s.CustomerId;
                                    model.SubInstallPrice = s.SubInstallPrice;
                                    model.InstallPrice = s.InstallPrice;
                                    model.IsDelete = false;
                                    model.OutsourctId = s.OutsourctId;
                                    model.PriceItemId = s.PriceItemId;
                                    model.SendPrice = s.SendPrice;
                                    model.UnitId = s.UnitId;
                                    bll.Add(model);
                                }
                            });
                        }
                        result = "ok";
                    }
                    catch(Exception ex)
                    {
                        result ="提交失败："+ ex.Message;
                    }
                }
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using System.Transactions;

namespace WebApp.Outsource.MaterialInfo
{
    /// <summary>
    /// ListHandler 的摘要说明
    /// </summary>
    public class ListHandler : IHttpHandler
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
                case "getPriceItem":
                    result = GetPriceItemList();
                    break;
                case "getList":
                    result = GetMaterialList();
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
                    int Id = int.Parse(context.Request["Id"]);
                    result = DeleteMaterial(Id);
                    break;
                case "addPriceItem":
                    result = AddPriceItem();
                    break;
                case "changePriceItemState":
                    result = ChangePriceItemState();
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
                //json.Append("{\"CustomerId\":\"0\",\"CustomerName\":\"全部\"},");
                cusotmerList.ForEach(s =>
                {
                    json.Append("{\"CustomerId\":\"" + s.Id + "\",\"CustomerName\":\"" + s.CustomerName + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetPriceItemList()
        {
            int customerId = 0;
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            var list = new OutsourceMaterialPriceItemBLL().GetList(s => s.CustomerId == customerId).OrderByDescending(s => s.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    string beginDate = string.Empty;
                    if (s.BeginDate != null)
                        beginDate = DateTime.Parse(s.BeginDate.ToString()).ToShortDateString();
                    int isDelete = (s.IsDelete ?? false) ? 1 : 0;

                    json.Append("{\"ItemId\":\"" + s.Id + "\",\"ItemName\":\"" + s.ItemName + "\",\"BeginDate\":\"" + beginDate + "\",\"IsDelete\":\"" + isDelete + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "[]";
        }


        string GetMaterialList()
        {
            int customerId = 0;
            int priceItemId = 0;
            int currPage=0,pageSize=0;
            string materialName = string.Empty;
            if (context1.Request.QueryString["currpage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            if (context1.Request.QueryString["materialName"] != null)
            {
                materialName = context1.Request.QueryString["materialName"];
            }
            if (context1.Request.QueryString["priceItemId"] != null)
            {
                priceItemId = int.Parse(context1.Request.QueryString["priceItemId"]);
            }
            try
            {
                var list = (from om in CurrentContext.DbContext.OutsourceMaterialInfo
                            join customer in CurrentContext.DbContext.Customer
                            on om.CustomerId equals customer.Id
                            join bm in CurrentContext.DbContext.BasicMaterial
                            on om.BasicMaterialId equals bm.Id
                            join unit1 in CurrentContext.DbContext.UnitInfo
                            on bm.UnitId equals unit1.Id into unitTemp
                            from unit in unitTemp.DefaultIfEmpty()
                            where om.CustomerId == customerId
                            && om.PriceItemId == priceItemId
                            && (om.OutsourctId??0)==0
                            select new
                            {
                                om,
                                customer.CustomerName,
                                unit.UnitName,
                                bm.MaterialName
                            }).ToList();
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    list = list.Where(s => s.MaterialName.ToLower().Contains(materialName.ToLower())).ToList();
                }
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    int totalCount = list.Count;
                    list = list.OrderBy(s => s.om.BasicCategoryId).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                    int index = 1;
                    list.ForEach(s =>
                    {
                        string state = s.om.IsDelete != null && s.om.IsDelete == true ? "已删除" : "正常";
                        json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.om.Id + "\",\"MaterialName\":\"" + s.MaterialName + "\",\"CustomerId\":\"" + s.om.CustomerId + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"UnitId\":\"" + s.om.UnitId + "\",\"Unit\":\"" + s.UnitName + "\",\"State\":\"" + state + "\",\"InstallPrice\":\"" + s.om.InstallPrice + "\",\"SubInstallPrice\":\"" + s.om.SubInstallPrice + "\",\"SendPrice\":\"" + s.om.SendPrice + "\",\"BasicMaterialName\":\"" + s.MaterialName + "\",\"BasicMaterialId\":\"" + s.om.BasicMaterialId + "\",\"BasicCategoryId\":\"" + s.om.BasicCategoryId + "\"},");
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

        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OutsourceMaterialInfoBLL bll = new OutsourceMaterialInfoBLL();
                OutsourceMaterialInfo model = JsonConvert.DeserializeObject<OutsourceMaterialInfo>(jsonStr);
                if (model != null)
                {
                    if (model.Id > 0)
                    {
                        var list = bll.GetList(s => s.CustomerId == model.CustomerId && s.PriceItemId == model.PriceItemId && s.BasicMaterialId == model.BasicMaterialId && s.Id != model.Id);
                        if (list.Any())
                        {
                            result = "重复报价！";
                        }
                        else
                        {
                            OutsourceMaterialInfo newModel = bll.GetModel(model.Id);
                            if (newModel != null)
                            {
                                newModel.BasicCategoryId = model.BasicCategoryId;
                                newModel.BasicMaterialId = model.BasicMaterialId;
                                newModel.CustomerId = model.CustomerId;
                                newModel.PriceItemId = model.PriceItemId;
                                newModel.InstallPrice = model.InstallPrice;
                                newModel.SubInstallPrice = model.SubInstallPrice;
                                newModel.SendPrice = model.SendPrice;
                                newModel.UnitId = model.UnitId;
                                bll.Update(newModel);
                            }
                            else
                            {
                                model.AddDate = DateTime.Now;
                                model.AddUserId = new BasePage().CurrentUser.UserId;
                                model.IsDelete = false;
                                bll.Add(model);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var list = bll.GetList(s => s.CustomerId == model.CustomerId && s.PriceItemId == model.PriceItemId && s.BasicMaterialId == model.BasicMaterialId);
                            if (list.Any())
                            {
                                result = "重复报价！";
                            }
                            else
                            {
                                model.AddDate = DateTime.Now;
                                model.AddUserId = new BasePage().CurrentUser.UserId;
                                model.IsDelete = false;
                                bll.Add(model);

                            }
                        }
                        catch(Exception ex)
                        {
                            result = "提交失败："+ex.Message;
                        }
                    }
                }
            }
            return result;
        }

        string DeleteMaterial(int id)
        {
            string result = "error";
            try
            {
                OutsourceMaterialInfoBLL bll = new OutsourceMaterialInfoBLL();
                OutsourceMaterialInfo model = bll.GetModel(id);
                if (model != null)
                {
                    
                    bll.Delete(model);
                    result = "ok";
                }

            }
            catch
            {

            }
            return result;
        }

        string AddPriceItem()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request["jsonStr"] != null)
            {
                jsonStr = context1.Request["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        //jsonStr = HttpUtility.UrlDecode(jsonStr);
                        if (!string.IsNullOrWhiteSpace(jsonStr))
                        {
                            jsonStr = jsonStr.Replace("+", "%2B");
                            jsonStr = HttpUtility.UrlDecode(jsonStr);
                        }
                        OutsourceMaterialPriceItem itemModel = JsonConvert.DeserializeObject<OutsourceMaterialPriceItem>(jsonStr);
                        if (itemModel != null)
                        {
                            OutsourceMaterialPriceItemBLL itemBll = new OutsourceMaterialPriceItemBLL();
                            var itemList = itemBll.GetList(s =>s.CustomerId==itemModel.CustomerId);
                            if (itemList.Any(s => s.ItemName.ToLower() == itemModel.ItemName.Trim().ToLower()))
                            {
                                result = "类型名称重复";
                            }
                            else
                            {
                                List<OutsourceMaterialInfo> materialList = new List<OutsourceMaterialInfo>();
                                if (itemModel.Materials != null && itemModel.Materials.Any())
                                {
                                    materialList = itemModel.Materials;
                                }
                                itemModel.Materials = null;
                                itemModel.ItemName = itemModel.ItemName.Trim();
                                if (itemList.Any(s => (itemModel.IsDelete??false)==false))
                                   itemModel.IsDelete = true;
                                else
                                    itemModel.IsDelete = false;
                                itemBll.Add(itemModel);
                                if (materialList.Any())
                                {
                                    OutsourceMaterialInfo detailModel;
                                    OutsourceMaterialInfoBLL detailBll = new OutsourceMaterialInfoBLL();
                                    materialList.ForEach(s =>
                                    {
                                        detailModel = new OutsourceMaterialInfo();
                                        detailModel.AddDate = DateTime.Now;
                                        detailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        detailModel.BasicCategoryId = s.BasicCategoryId;
                                        detailModel.BasicMaterialId = s.BasicMaterialId;
                                        detailModel.CustomerId = itemModel.CustomerId;
                                        detailModel.IsDelete = false;
                                        detailModel.InstallPrice = s.InstallPrice;
                                        detailModel.SendPrice = s.SendPrice;
                                        detailModel.PriceItemId = itemModel.Id;
                                        detailModel.UnitId = s.UnitId;
                                        detailBll.Add(detailModel);
                                    });
                                }
                                tran.Complete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }

            }
            return result;
        }



        string ChangePriceItemState()
        {
            string result = "ok";
            int itemId = 0;
            if (context1.Request.QueryString["id"] != null)
            {
                itemId = int.Parse(context1.Request.QueryString["id"]);
            }
            OutsourceMaterialPriceItemBLL itemBll = new OutsourceMaterialPriceItemBLL();
            OutsourceMaterialPriceItem model = itemBll.GetModel(itemId);
            if (model != null)
            {
                //原来是禁用的，（不能同时有2个启用）
                if (model.IsDelete ?? false)
                {
                    var list = itemBll.GetList(s =>s.CustomerId==model.CustomerId && (s.IsDelete ?? false) == false);
                    if (list.Any())
                    {
                        result = "更新失败：不能同时有2个启用的项目，请先禁用其他项目！";
                    }
                    else
                    {
                        model.IsDelete = false;
                        itemBll.Update(model);
                    }
                }
                else
                {
                    int count = itemBll.GetList(s => s.CustomerId == model.CustomerId).Count;
                    if (count == 1)
                    {
                        result = "更新失败：当前只有一个项目，所以不能禁用！";
                    }
                    else
                    {
                        model.IsDelete = true;
                        itemBll.Update(model);
                    }
                }
            }
            else
                result = "更新失败！";
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
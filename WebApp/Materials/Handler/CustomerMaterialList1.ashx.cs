using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using System.Text;
using System.Transactions;

namespace WebApp.Materials.Handler
{
    /// <summary>
    /// CustomerMaterialList1 的摘要说明
    /// </summary>
    public class CustomerMaterialList1 : IHttpHandler
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
                    result = GetBasicMaterial(categoryId);
                    break;
                case "getUnit":
                    result = GetUnits();
                    break;
                case "edit":
                    string jsonStr = context.Request["jsonstr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = jsonStr.Replace("+", "%2B");
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    //string opType = context.Request["opType"];
                    result = Edit(jsonStr);
                    break;
                case "delete":
                    int Id = int.Parse(context.Request["Id"]);
                    result=DeleteMaterial(Id);
                    break;
                case "getPriceItem":
                    result = GetPriceItemList();
                    break;
                case "addPriceItem":
                    result = AddPriceItem();
                    break;
                case "changePriceItemState":
                    result = ChangePriceItemState();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetCustomerList()
        {
            List<Models.Customer> cusotmerList =new BasePage().GetMyCustomerList();
            if (cusotmerList.Any())
            {
                StringBuilder json = new StringBuilder();
                //json.Append("{\"CustomerId\":\"0\",\"CustomerName\":\"全部\"},");
                cusotmerList.ForEach(s => {
                    json.Append("{\"CustomerId\":\""+s.Id+"\",\"CustomerName\":\""+s.CustomerName+"\"},");

                });
                return "["+json.ToString().TrimEnd(',')+"]";
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
            var list = new CustomerMaterialPriceItemBLL().GetList(s => s.CustomerId == customerId).OrderByDescending(s=>s.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
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

        string GetMaterialList(int currPage, int pageSize)
        {
            int customerId = 0;
            int priceItemId = 0;
            string materialName = string.Empty;
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
                var list = (from cm in CurrentContext.DbContext.CustomerMaterialInfo
                            join customer in CurrentContext.DbContext.Customer
                            on cm.CustomerId equals customer.Id
                            
                            join bm in CurrentContext.DbContext.BasicMaterial
                            on cm.BasicMaterialId equals bm.Id
                            join unit1 in CurrentContext.DbContext.UnitInfo
                            on bm.UnitId equals unit1.Id into unitTemp
                            from unit in unitTemp.DefaultIfEmpty()
                            where cm.CustomerId == customerId
                            && cm.PriceItemId == priceItemId
                            select new
                            {
                                cm,
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
                    list = list.OrderBy(s => s.cm.BasicCategoryId).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                    int index = 1;
                    OutsourceMaterialInfoBLL outMaterialBll = new OutsourceMaterialInfoBLL();
                    var outsourceMaterialList = (from material in CurrentContext.DbContext.OutsourceMaterialInfo
                                                 join priceItem in CurrentContext.DbContext.OutsourceMaterialPriceItem
                                                 on material.PriceItemId equals priceItem.Id
                                                 where (priceItem.IsDelete == null || priceItem.IsDelete == false)
                                                 && material.CustomerId == customerId
                                                 select material).ToList();
                    list.ForEach(s =>
                    {
                        decimal PayPriceInstall = 0;
                        decimal PayPriceInstallAndProduct = 0;
                        decimal PayPriceSend = 0;
                        int materialId = s.cm.BasicMaterialId ?? 0;
                        int outsourceMaterialId = 0;
                        var omModel = outsourceMaterialList.Where(o => o.BasicMaterialId == materialId).FirstOrDefault();
                        if (omModel != null)
                        {
                            PayPriceInstall = omModel.InstallPrice ?? 0;
                            PayPriceInstallAndProduct = omModel.InstallAndProductPrice ?? 0;
                            PayPriceSend = omModel.SendPrice ?? 0;
                            outsourceMaterialId = omModel.Id;
                        }
                        string state = s.cm.IsDelete != null && s.cm.IsDelete == true ? "已删除" : "正常";
                        json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.cm.Id + "\",\"MaterialName\":\"" + s.MaterialName + "\",\"CustomerId\":\"" + s.cm.CustomerId + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"UnitId\":\"" + s.cm.UnitId + "\",\"Unit\":\"" + s.UnitName + "\",\"State\":\"" + state + "\",\"Price\":\"" + s.cm.Price + "\",\"BasicMaterialName\":\"" + s.MaterialName + "\",\"BasicMaterialId\":\"" + s.cm.BasicMaterialId + "\",\"BasicCategoryId\":\"" + s.cm.BasicCategoryId + "\",\"PayPriceInstall\":\"" + PayPriceInstall + "\",\"PayPriceInstallAndProduct\":\"" + PayPriceInstallAndProduct + "\",\"PayPriceSend\":\"" + PayPriceSend + "\",\"OutsourceMaterialId\":\"" + outsourceMaterialId + "\"},");
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
            var list = new BasicMaterialBLL().GetList(s => s.MaterialCategoryId == categoryId && (s.IsDelete==null || s.IsDelete==false));
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int customerId = 0;
                if (context1.Request.QueryString["customerId"] != null)
                {
                    customerId = int.Parse(context1.Request.QueryString["customerId"]);
                }
                //外协材质报价
                var outsourceMaterialList = (from material in CurrentContext.DbContext.OutsourceMaterialInfo
                                             join priceItem in CurrentContext.DbContext.OutsourceMaterialPriceItem
                                             on material.PriceItemId equals priceItem.Id
                                             where (priceItem.IsDelete == null || priceItem.IsDelete == false)
                                             && material.CustomerId == customerId
                                             select material).ToList();
                list.ForEach(s => {
                    decimal PayPriceInstall = 0;//应付安装价格
                    decimal PayPriceSend = 0;//应付发货价格
                    int outsourceMaterialId = 0;
                    var omModel = outsourceMaterialList.Where(o => o.BasicMaterialId == s.Id).FirstOrDefault();
                    if (omModel != null)
                    {
                        PayPriceInstall = omModel.InstallPrice ?? 0;
                        PayPriceSend = omModel.SendPrice ?? 0;
                        outsourceMaterialId = omModel.Id;
                    }
                    json.Append("{\"Id\":\"" + s.Id + "\",\"UnitId\":\"" + s.UnitId + "\",\"MaterialName\":\"" + s.MaterialName + "\",\"PayPriceInstall\":\"" + PayPriceInstall + "\",\"PayPriceSend\":\"" + PayPriceSend + "\",\"OutsourceMaterialId\":\"" + outsourceMaterialId + "\"},");
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
        CustomerMaterialInfoBLL bll = new CustomerMaterialInfoBLL();
        string Edit(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                
                CustomerMaterialInfo model = JsonConvert.DeserializeObject<CustomerMaterialInfo>(jsonStr);
                if (model != null)
                {
                    OutsourceMaterialInfoBLL outMaterialBll = new OutsourceMaterialInfoBLL();
                    OutsourceMaterialInfo outsourceMaterialInfoModel;
                    if (model.Id > 0)
                    {
                        var list = bll.GetList(s=>s.CustomerId==model.CustomerId && s.PriceItemId==model.PriceItemId && s.BasicMaterialId==model.BasicMaterialId && s.Id!=model.Id);
                        if (list.Any())
                        {
                            result = "重复报价！";
                        }
                        else
                        {
                            CustomerMaterialInfo newModel = bll.GetModel(model.Id);
                            if (newModel != null)
                            {
                                newModel.BasicCategoryId = model.BasicCategoryId;
                                newModel.BasicMaterialId = model.BasicMaterialId;
                                newModel.CustomerId = model.CustomerId;
                                newModel.PriceItemId = model.PriceItemId;
                                newModel.Price = model.Price;
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
                            if ((model.OutsourceMaterialId ?? 0) > 0)
                            {
                                outsourceMaterialInfoModel = outMaterialBll.GetModel(model.OutsourceMaterialId ?? 0);
                                if (outsourceMaterialInfoModel != null)
                                {
                                    if (outsourceMaterialInfoModel.InstallPrice != model.PayPriceInstall || outsourceMaterialInfoModel.SendPrice != model.PayPriceSend || outsourceMaterialInfoModel.InstallAndProductPrice!=model.PayPriceInstallAndProduct)
                                    {
                                        outsourceMaterialInfoModel.InstallPrice = model.PayPriceInstall;
                                        outsourceMaterialInfoModel.InstallAndProductPrice = model.PayPriceInstallAndProduct;
                                        outsourceMaterialInfoModel.SendPrice = model.PayPriceSend;
                                        outMaterialBll.Update(outsourceMaterialInfoModel);
                                    }
                                }
                            }
                            else
                            {
                                //获取启用的外协报价项目
                                OutsourceMaterialPriceItem itemModel = new OutsourceMaterialPriceItemBLL().GetList(s => s.IsDelete == null || s.IsDelete == false).FirstOrDefault();
                                if (itemModel != null)
                                {
                                    outsourceMaterialInfoModel = new OutsourceMaterialInfo();
                                    outsourceMaterialInfoModel.AddDate = DateTime.Now;
                                    outsourceMaterialInfoModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    outsourceMaterialInfoModel.BasicCategoryId = model.BasicCategoryId;
                                    outsourceMaterialInfoModel.BasicMaterialId = model.BasicMaterialId;
                                    outsourceMaterialInfoModel.CustomerId = model.CustomerId;
                                    outsourceMaterialInfoModel.InstallPrice = model.PayPriceInstall;
                                    outsourceMaterialInfoModel.InstallAndProductPrice = model.PayPriceInstallAndProduct;
                                    outsourceMaterialInfoModel.IsDelete = false;
                                    outsourceMaterialInfoModel.PriceItemId = itemModel.Id;
                                    outsourceMaterialInfoModel.SendPrice = model.PayPriceSend;
                                    outsourceMaterialInfoModel.UnitId = model.UnitId;
                                    outMaterialBll.Add(outsourceMaterialInfoModel);
                                }
                            }
                            //outsourceMaterialInfoModel = outMaterialBll.GetList(s=>s.BasicMaterialId==model.BasicMaterialId).FirstOrDefault();
                            //if (outsourceMaterialInfoModel != null)
                            //{
                            //    if (outsourceMaterialInfoModel.InstallPrice != model.PayPriceInstall || outsourceMaterialInfoModel.SendPrice != model.PayPriceSend)
                            //    {
                            //        outsourceMaterialInfoModel.InstallPrice = model.PayPriceInstall;
                            //        outsourceMaterialInfoModel.SendPrice = model.PayPriceSend;
                            //        outMaterialBll.Update(outsourceMaterialInfoModel);
                            //    }
                            //}
                            //else
                            //{
                                
                            //}
                        }
                    }
                    else
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
                            BasicMaterial basicM = new BasicMaterialBLL().GetModel(model.BasicMaterialId??0);
                            if (basicM != null)
                            {
                                OrderMaterialMppingBLL orderMaterialBll = new OrderMaterialMppingBLL();
                                var orderMList = orderMaterialBll.GetList(s => s.OrderMaterialName.ToLower() == basicM.MaterialName.ToLower());
                                if (!orderMList.Any())
                                {
                                    OrderMaterialMpping orderMModel = new OrderMaterialMpping();
                                    orderMModel.BasicMaterialId = model.BasicMaterialId;
                                    orderMModel.BasicCategoryId = model.BasicCategoryId;
                                    orderMModel.CustomerId = model.CustomerId;
                                    orderMModel.OrderMaterialName = basicM.MaterialName;
                                    orderMModel.IsDelete = false;
                                    orderMaterialBll.Add(orderMModel);

                                    if ((model.OutsourceMaterialId ?? 0) > 0)
                                    {
                                        outsourceMaterialInfoModel = outMaterialBll.GetModel(model.OutsourceMaterialId ?? 0);
                                        if (outsourceMaterialInfoModel != null)
                                        {
                                            if (outsourceMaterialInfoModel.InstallPrice != model.PayPriceInstall || outsourceMaterialInfoModel.SendPrice != model.PayPriceSend || outsourceMaterialInfoModel.InstallAndProductPrice != model.PayPriceInstallAndProduct)
                                            {
                                                outsourceMaterialInfoModel.InstallPrice = model.PayPriceInstall;
                                                outsourceMaterialInfoModel.InstallAndProductPrice = model.PayPriceInstallAndProduct;
                                                outsourceMaterialInfoModel.SendPrice = model.PayPriceSend;
                                                outMaterialBll.Update(outsourceMaterialInfoModel);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //获取启用的外协报价项目
                                        OutsourceMaterialPriceItem itemModel = new OutsourceMaterialPriceItemBLL().GetList(s => s.IsDelete == null || s.IsDelete == false).FirstOrDefault();
                                        if (itemModel != null)
                                        {
                                            outsourceMaterialInfoModel = new OutsourceMaterialInfo();
                                            outsourceMaterialInfoModel.AddDate = DateTime.Now;
                                            outsourceMaterialInfoModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceMaterialInfoModel.BasicCategoryId = model.BasicCategoryId;
                                            outsourceMaterialInfoModel.BasicMaterialId = model.BasicMaterialId;
                                            outsourceMaterialInfoModel.CustomerId = model.CustomerId;
                                            outsourceMaterialInfoModel.InstallPrice = model.PayPriceInstall;
                                            outsourceMaterialInfoModel.InstallAndProductPrice = model.PayPriceInstallAndProduct;
                                            outsourceMaterialInfoModel.IsDelete = false;
                                            outsourceMaterialInfoModel.PriceItemId = itemModel.Id;
                                            outsourceMaterialInfoModel.SendPrice = model.PayPriceSend;
                                            outsourceMaterialInfoModel.UnitId = model.UnitId;
                                            outMaterialBll.Add(outsourceMaterialInfoModel);
                                        }
                                    }
                                    //outsourceMaterialInfoModel = outMaterialBll.GetList(s => s.BasicMaterialId == model.BasicMaterialId).FirstOrDefault();
                                    //if (outsourceMaterialInfoModel != null)
                                    //{
                                    //    if (outsourceMaterialInfoModel.InstallPrice != model.PayPriceInstall || outsourceMaterialInfoModel.SendPrice != model.PayPriceSend)
                                    //    {
                                    //        outsourceMaterialInfoModel.InstallPrice = model.PayPriceInstall;
                                    //        outsourceMaterialInfoModel.SendPrice = model.PayPriceSend;
                                    //        outMaterialBll.Update(outsourceMaterialInfoModel);
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    outsourceMaterialInfoModel = new OutsourceMaterialInfo();
                                    //    outsourceMaterialInfoModel.AddDate = DateTime.Now;
                                    //    outsourceMaterialInfoModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    //    outsourceMaterialInfoModel.BasicCategoryId = model.BasicCategoryId;
                                    //    outsourceMaterialInfoModel.BasicMaterialId = model.BasicMaterialId;
                                    //    outsourceMaterialInfoModel.CustomerId = model.CustomerId;
                                    //    outsourceMaterialInfoModel.InstallPrice = model.PayPriceInstall;
                                    //    outsourceMaterialInfoModel.IsDelete = false;
                                    //    outsourceMaterialInfoModel.PriceItemId = 1;
                                    //    outsourceMaterialInfoModel.SendPrice = model.PayPriceSend;
                                    //    outsourceMaterialInfoModel.UnitId = model.UnitId;
                                    //    outMaterialBll.Add(outsourceMaterialInfoModel);
                                    //}
                                }
                                
                            }
                        }
                    }
                }
            }
            return result;
        }

        bool CheckExist(int customerId,int id, string name)
        {
            var list = bll.GetList(s => s.CustomerId == customerId && s.MaterialName.ToLower() == name.ToLower() && (id > 0 ? (s.Id != id) : 1 == 1));
            return list.Any();
        }


        string DeleteMaterial(int id)
        {
            string result = "error";
            try
            {
                CustomerMaterialInfo model = bll.GetModel(id);
                if (model != null)
                {
                    //model.IsDelete = true;
                    bll.Delete (model);
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
            if (context1.Request["jsonstr"] != null)
            {
                jsonStr = context1.Request["jsonstr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                        CustomerMaterialPriceItem itemModel = JsonConvert.DeserializeObject<CustomerMaterialPriceItem>(jsonStr);
                        if (itemModel != null)
                        {
                            CustomerMaterialPriceItemBLL itemBll = new CustomerMaterialPriceItemBLL();
                            var itemList = itemBll.GetList(s => s.ItemName.ToLower() == itemModel.ItemName.Trim().ToLower());
                            if (itemList.Any())
                            {
                                result = "类型名称重复";
                            }
                            else
                            {
                                List<CustomerMaterialInfo> materialList = new List<CustomerMaterialInfo>();
                                if (itemModel.Materials != null && itemModel.Materials.Any())
                                {
                                    materialList = itemModel.Materials;
                                }
                                itemModel.Materials = null;
                                itemModel.ItemName = itemModel.ItemName.Trim();
                                itemBll.Add(itemModel);
                                if (materialList.Any())
                                {
                                    CustomerMaterialInfo detailModel;
                                    CustomerMaterialInfoBLL detailBll = new CustomerMaterialInfoBLL();
                                    materialList.ForEach(s =>
                                    {
                                        detailModel = new CustomerMaterialInfo();
                                        detailModel.AddDate = DateTime.Now;
                                        detailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        detailModel.BasicCategoryId = s.BasicCategoryId;
                                        detailModel.BasicMaterialId = s.BasicMaterialId;
                                        detailModel.CustomerId = itemModel.CustomerId;
                                        detailModel.IsDelete = false;
                                        detailModel.Price = s.Price;
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
            CustomerMaterialPriceItemBLL itemBll = new CustomerMaterialPriceItemBLL();
            CustomerMaterialPriceItem model = itemBll.GetModel(itemId);
            if (model != null)
            {
                model.IsDelete = (model.IsDelete ?? false)==false ? true : false;
                itemBll.Update(model);
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
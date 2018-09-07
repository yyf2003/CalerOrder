using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using BLL;
using Common;
using Models;
using Newtonsoft.Json;

namespace WebApp.Customer.Handler
{
    /// <summary>
    /// AddCustomer 的摘要说明
    /// </summary>
    public class AddCustomer : IHttpHandler
    {
        CustomerBLL customerBll = new CustomerBLL();
        Models.Customer customerModel;
        ProvinceInRegionBLL prBll = new ProvinceInRegionBLL();
        ProvinceInRegion prModel;
        int customerId;
        public void ProcessRequest(HttpContext context)
        {
            string type = string.Empty;
            string result = string.Empty;
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context.Request.QueryString["customerId"]);
            }
            switch (type)
            { 
                case "initProvince":
                    result = InitProvince();
                    break;
                case "add":
                    string jsonStr = context.Request.Form["jsonStr"];
                    string optype = context.Request.QueryString["optype"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    result = Add(optype, jsonStr);
                    break;
                case "getRegion":
                    result = GetRegion();
                    break;

            }
            context.Response.Write(result);
        }

        string InitProvince() {
            PlaceBLL placeBll = new PlaceBLL();
            var list = placeBll.GetList(s=>s.ParentID==0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"ProvinceId\":\""+s.ID+"\",\"ProvinceName\":\""+s.PlaceName+"\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string Add(string optype, string jsonStr)
        {
            string result = "提交失败！";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        customerModel = JsonConvert.DeserializeObject<Models.Customer>(jsonStr);
                        if (customerModel != null)
                        {
                            List<Region> rgionList = new List<Region>();
                            if (customerModel.Regions != null)
                                rgionList = customerModel.Regions;
                            RegionBLL regionBll = new RegionBLL();
                            Region regionModel;
                            if (optype == "add")
                            {
                                //新增
                                customerModel.IsDelete = false;
                                customerModel.AddDate = DateTime.Now;
                                customerModel.AddUserId = new BasePage().CurrentUser.UserId;
                                customerBll.Add(customerModel);
                                if (rgionList.Any())
                                {
                                    rgionList.ForEach(s =>
                                    {
                                        regionModel = s;
                                        regionModel.IsDelete = false;
                                        regionModel.CustomerId = customerModel.Id;
                                        regionBll.Add(regionModel);
                                        if (!string.IsNullOrWhiteSpace(s.ProvinceIds))
                                        {
                                            StringHelper.ToIntList(s.ProvinceIds, ',').ForEach(p =>
                                            {
                                                prModel = new ProvinceInRegion() { ProvinceId = p, RegionId = regionModel.Id };
                                                prBll.Add(prModel);
                                            });
                                        }
                                    });
                                }
                            }
                            else
                            {
                                customerBll.Update(customerModel);
                                //删除区域信息
                                regionBll.GetList(s => s.CustomerId == customerModel.Id).ForEach(s => {
                                    prBll.Delete(p=>p.RegionId==s.Id);
                                    regionBll.Delete(s);
                                });
                                if (rgionList.Any())
                                {
                                    rgionList.ForEach(s =>
                                    {
                                        regionModel = s;
                                        regionModel.IsDelete = false;
                                        regionModel.CustomerId = customerModel.Id;
                                        regionBll.Add(regionModel);
                                        if (!string.IsNullOrWhiteSpace(s.ProvinceIds))
                                        {
                                            StringHelper.ToIntList(s.ProvinceIds, ',').ForEach(p =>
                                            {
                                                prModel = new ProvinceInRegion() { ProvinceId = p, RegionId = regionModel.Id };
                                                prBll.Add(prModel);
                                            });
                                        }
                                    });
                                }
                            }
                            result = "ok";
                            tran.Complete();
                        }
                    }
                    catch (Exception ex)
                    { result = "提交失败"; }
                }
            }
            return result;
        }

        string GetRegion()
        {
            RegionBLL regionBll = new RegionBLL();
            //var regioList = regionBll.GetList(s=>s.CustomerId==customerId && (s.IsDelete==null || s.IsDelete==false));
            var regioList = regionBll.GetList(s => (s.IsDelete == null || s.IsDelete == false));
            if (regioList.Any())
            {
                StringBuilder jsonStr = new StringBuilder();
                regioList.ForEach(s => {
                    StringBuilder pids = new StringBuilder();
                    prBll.GetList(p => p.RegionId == s.Id).ForEach(p => {
                        pids.Append(p.ProvinceId);
                        pids.Append(",");
                    });
                    
                    jsonStr.Append("{\"RegionId\":\""+s.Id+"\",\"RegionName\":\""+s.RegionName+"\",\"ProvinceId\":\""+pids.ToString().TrimEnd(',')+"\"},");
                    
                });
                return "["+jsonStr.ToString().TrimEnd(',')+"]";
            }
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
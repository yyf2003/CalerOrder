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

namespace WebApp.Regions.Handler
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        string type = string.Empty;
        RegionBLL regionBll = new RegionBLL();
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
                case "getProvince":
                    int regionId = int.Parse(context.Request["regionId"]);
                    result = GetProvince(regionId);
                    break;
                case "edit":
                    string jsonString= context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = AddRegion(jsonString, optype);
                    break;
            }
            context.Response.Write(result);
        }

        string GetProvince(int regionId)
        {
            var list = from pr in CurrentContext.DbContext.ProvinceInRegion
                       join province in CurrentContext.DbContext.Place
                       on pr.ProvinceId equals province.ID
                       where pr.RegionId == regionId
                       select province.ID;
            if (list.Any())
            {
                StringBuilder sb = new StringBuilder();
                list.ToList().ForEach(s =>
                {
                    sb.Append("{\"Id\":\""+s+"\"},");
                });
                return "[" + sb.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string AddRegion(string jsonString, string optype)
        {
            string result = "提交失败！";
            if (!string.IsNullOrEmpty(jsonString))
            {
                Models.Region model = JsonConvert.DeserializeObject<Models.Region>(jsonString);
                if (model != null)
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        try
                        {
                            string provinceIds = model.ProvinceIds;
                            if (optype == "add")
                            {
                                if (!CheckName(model.CustomerId ?? 0, model.RegionName, 0))
                                {

                                    regionBll.Add(model);
                                    result = "ok";
                                }
                                else
                                    result = "exist";
                            }
                            else
                            {
                                if (!CheckName(model.CustomerId ?? 0, model.RegionName, model.Id))
                                {
                                    regionBll.Update(model);
                                    result = "ok";
                                }
                                else
                                    result = "exist";
                            }
                            int regionid = model.Id;
                            ProvinceInRegionBLL prBll = new ProvinceInRegionBLL();
                            ProvinceInRegion prModel;
                            prBll.Delete(s => s.RegionId == regionid);
                            if (!string.IsNullOrWhiteSpace(provinceIds))
                            {
                                string[] ids = provinceIds.Split(',');
                                foreach (string s in ids)
                                {
                                    
                                    if (!string.IsNullOrWhiteSpace(s))
                                    {
                                        prModel = new ProvinceInRegion() { ProvinceId = int.Parse(s), RegionId = regionid };
                                        prBll.Add(prModel);
                                    }
                                }
                            }
                            tran.Complete();
                        }
                        catch(Exception ex)
                        {
                            result = ex.Message;
                        }
                        

                    }
                    
                }
            }
            return result;
        }

        bool CheckName(int customerId, string name, int id)
        {
            var list = regionBll.GetList(s =>s.CustomerId==customerId && s.RegionName.ToUpper() == name.ToUpper() && (id > 0 ? s.Id != id : 1 == 1));
            return list.Any();
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
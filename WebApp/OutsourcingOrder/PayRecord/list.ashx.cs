using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using DAL;
using Common;
using System.Text;
using Newtonsoft.Json;

namespace WebApp.OutsourcingOrder.PayRecord
{
    /// <summary>
    /// list 的摘要说明
    /// </summary>
    public class list : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            string type = string.Empty;
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
                case "getOutsourceRegion":
                    result = GetOutsourceRegion();
                    break;
                case "getOutsource":
                    result = GetOutsource();
                    break;
                case "getGuidanceList":
                    result = GetGuidanceList();
                    break;
                case "getPayDetail":
                    result = GetPayDetailList();
                    break;
                case "deleteDetail":
                    result = DeleteDetail();
                    break;
                case "editDetail":
                    result = EditDetail();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetOutsourceRegion()
        {
            string result = string.Empty;
            List<Region> regionList = (from company in CurrentContext.DbContext.Company
                                       join region in CurrentContext.DbContext.Region
                                       on company.RegionId equals region.Id
                                       where company.TypeId == (int)CompanyTypeEnum.Outsource
                                       select region).Distinct().ToList();
            if (regionList.Any())
            {
                StringBuilder json = new StringBuilder();
                regionList.ForEach(s => {
                    json.Append("{\"RegionId\":\""+s.Id+"\",\"RegionName\":\""+s.RegionName+"\"},");
                });
                result = "["+json.ToString().TrimEnd(',')+"]";
            }
            return result; 
        }

        string GetOutsource() {
            
            List<int> regionIdList = new List<int>();
            if (context1.Request.QueryString["regionId"] != null)
            {
                string regionId = context1.Request.QueryString["regionId"];
                if (!string.IsNullOrWhiteSpace(regionId))
                {
                    regionIdList = StringHelper.ToIntList(regionId,',');
                }
            }
            List<Company> list = new CompanyBLL().GetList(s=>s.TypeId==(int)CompanyTypeEnum.Outsource && (s.IsDelete==null || s.IsDelete==false));
            if (regionIdList.Any())
            {
                list = list.Where(s => regionIdList.Contains(s.RegionId??0)).ToList();
            }
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.OrderBy(s=>s.RegionId).ToList().ForEach(s => {
                    json.Append("{\"CompanyId\":\"" + s.Id + "\",\"CompanyName\":\"" + s.CompanyName + "\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "[]";
        }
         
        string GetGuidanceList()
        {
            string result = string.Empty;
            int currPage = 0;
            int pageSize = 0;
            int customerId = 0;
            string guidanceMonth = string.Empty;
            string guidanceId = string.Empty;
            List<int> guidanceIdList = new List<int>();
            int outsourceId = 0;
            if (context1.Request.Form["page"] != null)
            {
                currPage = int.Parse(context1.Request.Form["page"]);
            }
            if (context1.Request.Form["limit"] != null)
            {
                pageSize = int.Parse(context1.Request.Form["limit"]);
            }
            if (context1.Request.Form["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.Form["customerId"]);
            }
            if (context1.Request.Form["guidanceMonth"] != null)
            {
                guidanceMonth = context1.Request.Form["guidanceMonth"];
            }
            if (context1.Request.Form["guidanceId"] != null)
            {
                guidanceId = context1.Request.Form["guidanceId"];
                guidanceIdList = StringHelper.ToIntList(guidanceId,',');
            }
            if (context1.Request.Form["outsourceId"] != null)
            {
                outsourceId = int.Parse(context1.Request.Form["outsourceId"]);
            }
            StringBuilder json = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                var list = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                            join guidance in CurrentContext.DbContext.SubjectGuidance
                            on order.GuidanceId equals guidance.ItemId
                            where guidance.CustomerId == customerId
                            && order.OutsourceId == outsourceId
                            && guidance.GuidanceYear == year
                            && guidance.GuidanceMonth == month
                            && (order.IsDelete == null || order.IsDelete == false)
                            select new { order, guidance }).ToList();
                if (guidanceIdList.Any())
                {
                    list = list.Where(s => guidanceIdList.Contains(s.order.GuidanceId??0)).ToList();
                }
                if (list.Any())
                {
                    var guidanceList = list.Select(s => s.guidance).Distinct().ToList();
                    int index = 1;
                    int rowCount = guidanceList.Count();
                    List<OutsourceOrderDetail> outsourceOrderList = list.Select(s => s.order).ToList();
                    OutsourcePayRecordBLL payRecordBll = new OutsourcePayRecordBLL();
                    guidanceList.OrderBy(s => s.ItemId).Skip((currPage - 1) * pageSize).Take(pageSize).ToList().ForEach(s =>
                    {
                        var orderList = outsourceOrderList.Where(order=>order.GuidanceId==s.ItemId).ToList();
                        decimal totalPrice = 0;
                        var popList = orderList.Where(order => order.OrderType == (int)OrderTypeEnum.POP).ToList();
                        popList.ForEach(order => {
                            totalPrice += (order.TotalPrice ?? 0);
                        });
                        var priceOrderList = orderList.Where(order => order.OrderType != (int)OrderTypeEnum.POP).ToList();
                        priceOrderList.ForEach(order =>
                        {
                            totalPrice += ((order.Quantity??1)*(order.PayOrderPrice??0));
                        });
                        if (totalPrice > 0)
                        {
                            totalPrice = Math.Round(totalPrice, 2);
                        }
                        string gmonth = s.GuidanceYear + "-" + s.GuidanceMonth;
                        decimal totalPay = 0;
                        string lastPayDate = string.Empty;
                        int payRecordCount = 0;
                        List<OutsourcePayRecord> payRecordList = payRecordBll.GetList(p => p.OutsourceId == outsourceId && p.GuidanceId==s.ItemId);
                        if (payRecordList.Any())
                        {
                            payRecordCount = payRecordList.Count;
                            totalPay = payRecordList.Sum(p => p.PayAmount ?? 0);
                            var m = payRecordList.OrderByDescending(p => p.Id).FirstOrDefault();
                            if (m.PayDate != null)
                            {
                                lastPayDate = DateTime.Parse(m.PayDate.ToString()).ToShortDateString();
                            }
                        }

                        json.Append("{\"RowIndex\":\"" + index + "\",\"GuidanceId\":\"" + s.ItemId + "\",\"GuidanceName\":\"" + s.ItemName + "\",\"GuidanceMonth\":\"" + gmonth + "\",\"PayPrice\":\"" + totalPrice + "\",\"Pay\":\"" + totalPay + "\",\"LastPayDate\":\"" + lastPayDate + "\",\"PayRecordCount\":\"" + payRecordCount + "\"},");
                        index++;
                    });
                    result = "{\"code\":0,\"msg\":\"\",\"count\":"+rowCount+",\"data\":["+json.ToString().TrimEnd(',')+"]}";
                }
            }
            if (string.IsNullOrWhiteSpace(result))
            {
                result = "{\"code\":0,\"msg\":\"\",\"count\":0,\"data\":[]}";
            }
            return result;
        }

        string GetPayDetailList()
        {
            string result = string.Empty;
            int guidanceId = 0;
            int outsourceId = 0;
            if (context1.Request.Form["guidanceId"] != null)
            {
                guidanceId = int.Parse(context1.Request.Form["guidanceId"]);
            }
            if (context1.Request.Form["outsourceId"] != null)
            {
                outsourceId = int.Parse(context1.Request.Form["outsourceId"]);
            }
            var list = (from record in CurrentContext.DbContext.OutsourcePayRecord
                       join user in CurrentContext.DbContext.UserInfo
                       on record.AddUserId equals user.UserId
                       where record.GuidanceId == guidanceId
                       && record.OutsourceId == outsourceId
                       select new {
                           record,
                           AddUserName=user.RealName
                       }).OrderBy(s=>s.record.PayDate).ToList();
            if (list.Any())
            {
                int count = list.Count;
                StringBuilder json = new StringBuilder();
                int index = 1;
                list.ForEach(s => {
                    string payDate = string.Empty;
                    if (s.record.PayDate != null)
                    {
                        payDate = DateTime.Parse(s.record.PayDate.ToString()).ToShortDateString();
                    }
                    json.Append("{\"RowIndex\":\"" + index + "\",\"Id\":\"" + s.record.Id + "\",\"AddDate\":\"" + s.record.AddDate + "\",\"PayDate\":\"" + payDate + "\",\"PayAmount\":\"" + s.record.PayAmount + "\",\"AddUserId\":\"" + s.record.AddUserId + "\",\"AddUserName\":\"" + s.AddUserName + "\",\"Remark\":\"" + s.record.Remark + "\"},");
                    index++;
                });
                result = "{\"code\":0,\"msg\":\"\",\"count\":" + count + ",\"data\":[" + json.ToString().TrimEnd(',') + "]}";
            }
            if (string.IsNullOrWhiteSpace(result))
                result = "{\"code\":0,\"msg\":\"\",\"count\":0,\"data\":[]}";
            return result;
        }

        string DeleteDetail()
        {
            string result = "error";
            int id=0;
            if (context1.Request.QueryString["id"] != null)
            {
                id = int.Parse(context1.Request.QueryString["id"]);
            }
            OutsourcePayRecordBLL bll = new OutsourcePayRecordBLL();
            OutsourcePayRecord model = bll.GetModel(id);
            if (model != null)
            {
                bll.Delete(model);
                result = "ok";
            }
            return result;
        }

        string EditDetail()
        {
            string result = "error";
            string jsonStr = string.Empty;
            if (context1.Request.QueryString["jsonStr"] != null)
            {
                jsonStr = context1.Request.QueryString["jsonStr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OutsourcePayRecordBLL bll=new OutsourcePayRecordBLL();
                OutsourcePayRecord newModel = JsonConvert.DeserializeObject<OutsourcePayRecord>(jsonStr);
                OutsourcePayRecord model = bll.GetModel(newModel.Id);
                if (model != null)
                {
                    if (newModel.PayAmount != null && newModel.PayAmount > 0)
                    {
                        model.PayAmount = newModel.PayAmount;
                    }
                    if (newModel.PayDate != null && StringHelper.IsDateTime(newModel.PayDate.ToString()))
                    {
                        model.PayDate = newModel.PayDate;
                    }
                    model.Remark = newModel.Remark;
                    bll.Update(model);
                    result = "ok";
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using DAL;
using Models;
using Common;
using Newtonsoft.Json;

namespace WebApp.Customer.EditLog.handler
{
    /// <summary>
    /// MachineFrameLogList 的摘要说明
    /// </summary>
    public class MachineFrameLogList : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context1 = context;
            string result = string.Empty;
            string type = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            switch (type)
            {
                case "getFrameLogList":
                    result = GetFrameLogList();
                    break;
                case "getPOPLogList":
                    result = GetPOPLogList();
                    break;
                case "getShopLogList":
                    result = GetShopLogList();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetFrameLogList()
        {
            string result = string.Empty;
            int shopId = 0;
            int currPage = 0;
            int pageSize = 0;
            string beginDate = string.Empty;
            string endDate = string.Empty;
            if (context1.Request.QueryString["shopId"] != null)
                shopId = int.Parse(context1.Request.QueryString["shopId"]);
            if (context1.Request.QueryString["beginDate"] != null)
                beginDate = context1.Request.QueryString["beginDate"];
            if (context1.Request.QueryString["endDate"] != null)
                endDate = context1.Request.QueryString["endDate"];
            if (context1.Request.QueryString["currPage"] != null)
                currPage = int.Parse(context1.Request.QueryString["currPage"]);
            if (context1.Request.QueryString["pageSize"] != null)
                pageSize = int.Parse(context1.Request.QueryString["pageSize"]);
            var list = (from log in CurrentContext.DbContext.ShopMachineFrameChangeDetail
                       join user1 in CurrentContext.DbContext.UserInfo
                       on log.AddUserId equals user1.UserId into userTemp
                       from user in userTemp.DefaultIfEmpty()
                       where log.ShopId == shopId
                       select new {
                           log,
                           user.RealName
                       }).ToList();
            if (!string.IsNullOrWhiteSpace(beginDate) && StringHelper.IsDateTime(beginDate))
            {
                DateTime begin = DateTime.Parse(beginDate);
                list = list.Where(s => s.log.AddDate >= begin).ToList();
            }
            if (!string.IsNullOrWhiteSpace(endDate) && StringHelper.IsDateTime(endDate))
            {
                DateTime end = DateTime.Parse(endDate).AddDays(1);
                list = list.Where(s => s.log.AddDate < end).ToList();
            }
            int recordCount = list.Count;
            list = list.OrderByDescending(s => s.log.AddDate).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
            StringBuilder json = new StringBuilder();
            int index = 1;
            list.ForEach(s =>
            {
                int rowIndex = (pageSize * (currPage - 1)) + index;
                index++;
                string addDate = string.Empty;
                if (s.log.AddDate != null)
                    addDate = DateTime.Parse(s.log.AddDate.ToString()).ToString();
                string isValid = (s.log.IsValid??true)?"是":"否";

                json.Append("{\"rowIndex\":\"" + rowIndex + "\",\"Id\":\"" + s.log.Id + "\",\"AddUserName\":\"" + s.RealName + "\",\"AddDate\":\"" + addDate + "\",\"PositionName\":\"" + s.log.PositionName + "\",\"MachineFrame\":\"" + s.log.MachineFrame + "\",\"Gender\":\"" + s.log.Gender + "\",\"Count\":\"" + s.log.Count + "\",\"CornerType\":\"" + s.log.CornerType + "\",\"Remark\":\"" + s.log.Remark + "\",\"IsValid\":\"" + isValid + "\"},");
            });
            result = "{\"pageCount\":\"" + recordCount + "\",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
            return result;
        }

        string GetPOPLogList()
        {
            string result = string.Empty;
            int shopId = 0;
            int currPage = 0;
            int pageSize = 0;
            string beginDate = string.Empty;
            string endDate = string.Empty;
            if (context1.Request.QueryString["shopId"] != null)
                shopId = int.Parse(context1.Request.QueryString["shopId"]);
            if (context1.Request.QueryString["beginDate"] != null)
                beginDate = context1.Request.QueryString["beginDate"];
            if (context1.Request.QueryString["endDate"] != null)
                endDate = context1.Request.QueryString["endDate"];
            if (context1.Request.QueryString["currPage"] != null)
                currPage = int.Parse(context1.Request.QueryString["currPage"]);
            if (context1.Request.QueryString["pageSize"] != null)
                pageSize = int.Parse(context1.Request.QueryString["pageSize"]);
            var list = (from log in CurrentContext.DbContext.POPChangeDetail
                        join user1 in CurrentContext.DbContext.UserInfo
                        on log.AddUserId equals user1.UserId into userTemp
                        from user in userTemp.DefaultIfEmpty()
                        where log.ShopId == shopId
                        select new
                        {
                            log,
                            user.RealName
                        }).ToList();
            if (!string.IsNullOrWhiteSpace(beginDate) && StringHelper.IsDateTime(beginDate))
            {
                DateTime begin = DateTime.Parse(beginDate);
                list = list.Where(s => s.log.AddDate >= begin).ToList();
            }
            if (!string.IsNullOrWhiteSpace(endDate) && StringHelper.IsDateTime(endDate))
            {
                DateTime end = DateTime.Parse(endDate).AddDays(1);
                list = list.Where(s => s.log.AddDate < end).ToList();
            }
            int recordCount = list.Count;
            list = list.OrderByDescending(s => s.log.AddDate).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
            StringBuilder json = new StringBuilder();
            int index = 1;
            list.ForEach(s =>
            {
                int rowIndex = (pageSize * (currPage - 1)) + index;
                index++;
                string addDate = string.Empty;
                if (s.log.AddDate != null)
                    addDate = DateTime.Parse(s.log.AddDate.ToString()).ToString();
                string isValid = (s.log.IsValid ?? true) ? "是" : "否";
                json.Append("{\"rowIndex\":\"" + rowIndex + "\",\"Id\":\"" + s.log.Id + "\",\"AddUserName\":\"" + s.RealName + "\",\"AddDate\":\"" + addDate + "\",\"Sheet\":\"" + s.log.Sheet + "\",\"Category\":\"" + s.log.Category + "\",\"Gender\":\"" + s.log.Gender + "\",\"ChangeType\":\"" + s.log.ChangeType + "\",\"CornerType\":\"" + s.log.CornerType + "\",\"GraphicLength\":\"" + s.log.GraphicLength + "\",\"GraphicMaterial\":\"" + s.log.GraphicMaterial + "\",\"GraphicNo\":\"" + s.log.GraphicNo + "\",\"GraphicWidth\":\"" + s.log.GraphicWidth + "\",\"IsValid\":\"" + isValid + "\",\"OOHInstallPrice\":\"" + s.log.OOHInstallPrice + "\",\"PositionDescription\":\"" + s.log.PositionDescription + "\",\"Quantity\":\"" + s.log.Quantity + "\",\"Remark\":\"" + s.log.Remark + "\",\"WindowDeep\":\"" + s.log.WindowDeep + "\",\"WindowHigh\":\"" + s.log.WindowHigh + "\",\"WindowSize\":\"" + s.log.WindowSize + "\",\"WindowWide\":\"" + s.log.WindowWide + "\"},");
            });
            result = "{\"pageCount\":\"" + recordCount + "\",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
            return result;
        }

        string GetShopLogList()
        {
            string result = string.Empty;
            int shopId = 0;
            int currPage = 0;
            int pageSize = 0;
            string beginDate = string.Empty;
            string endDate = string.Empty;
            string shopNo = string.Empty;
            if (context1.Request.QueryString["shopId"] != null)
                shopId = int.Parse(context1.Request.QueryString["shopId"]);
            if (context1.Request.QueryString["beginDate"] != null)
                beginDate = context1.Request.QueryString["beginDate"];
            if (context1.Request.QueryString["endDate"] != null)
                endDate = context1.Request.QueryString["endDate"];
            if (context1.Request.QueryString["currPage"] != null)
                currPage = int.Parse(context1.Request.QueryString["currPage"]);
            if (context1.Request.QueryString["pageSize"] != null)
                pageSize = int.Parse(context1.Request.QueryString["pageSize"]);
            if (context1.Request.QueryString["shopNo"] != null)
                shopNo = context1.Request.QueryString["shopNo"];

            var list = (from log in CurrentContext.DbContext.ShopChangeDetail
                       join user1 in CurrentContext.DbContext.UserInfo
                       on log.CSUserId equals user1.UserId into userTemp
                       from user in userTemp.DefaultIfEmpty()
                        join user2 in CurrentContext.DbContext.UserInfo
                         on log.AddUserId equals user2.UserId into user2Temp
                        from addUser in user2Temp.DefaultIfEmpty()
                       
                       select new { 
                          log,
                          CSUserName=user.RealName,
                          AddUserName = addUser.RealName
                       }).ToList();
            if (shopId > 0)
            {
                list = list.Where(s=>s.log.ShopId==shopId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(beginDate) && StringHelper.IsDateTime(beginDate))
            {
                DateTime begin = DateTime.Parse(beginDate);
                list = list.Where(s => s.log.AddDate >= begin).ToList();
            }
            if (!string.IsNullOrWhiteSpace(endDate) && StringHelper.IsDateTime(endDate))
            {
                DateTime end = DateTime.Parse(endDate).AddDays(1);
                list = list.Where(s => s.log.AddDate < end).ToList();
            }
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.log.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            int recordCount = list.Count;
            list = list.OrderByDescending(s => s.log.AddDate).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
            StringBuilder json = new StringBuilder();
            int index = 1;
            list.ForEach(s =>
            {
                int rowIndex = (pageSize * (currPage - 1)) + index;
                index++;
                string addDate = string.Empty;
                if (s.log.AddDate != null)
                    addDate = DateTime.Parse(s.log.AddDate.ToString()).ToString();

                json.Append("{\"rowIndex\":\"" + rowIndex + "\",\"Id\":\"" + s.log.Id + "\",\"AddUserName\":\"" + s.AddUserName + "\",\"AddDate\":\"" + addDate + "\",\"ShopName\":\"" + s.log.ShopName + "\",\"ShopNo\":\"" + s.log.ShopNo + "\",\"RegionName\":\"" + s.log.RegionName + "\",\"ProvinceName\":\"" + s.log.ProvinceName + "\",\"CityName\":\"" + s.log.CityName + "\",\"AreaName\":\"" + s.log.AreaName + "\",\"CityTier\":\"" + s.log.CityTier + "\",\"IsInstall\":\"" + s.log.IsInstall + "\",\"AgentCode\":\"" + s.log.AgentCode + "\",\"AgentName\":\"" + s.log.AgentName + "\",\"POPAddress\":\"" + s.log.POPAddress + "\",\"Contact1\":\"" + s.log.Contact1 + "\",\"Tel1\":\"" + s.log.Tel1 + "\",\"Contact2\":\"" + s.log.Contact2 + "\",\"Tel2\":\"" + s.log.Tel2 + "\",\"Channel\":\"" + s.log.Channel + "\",\"Format\":\"" + s.log.Format + "\",\"Status\":\"" + s.log.Status + "\",\"Category\":\"" + s.log.Category + "\",\"ShopType\":\"" + s.log.ShopType + "\",\"BasicInstallPrice\":\"" + s.log.BasicInstallPrice + "\",\"ChangeType\":\"" + s.log.ChangeType + "\",\"Remark\":\"" + s.log.Remark + "\",\"CSUserName\":\"" + s.CSUserName + "\",\"BCSInstallPrice\":\""+s.log.BCSInstallPrice+"\"},");
            });
            result = "{\"pageCount\":\"" + recordCount + "\",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";

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
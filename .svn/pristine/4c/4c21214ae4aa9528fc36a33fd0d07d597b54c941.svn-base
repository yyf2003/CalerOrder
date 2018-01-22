using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;
using Newtonsoft.Json;

namespace WebApp.Subjects.ADErrorCorrection.handler
{
    /// <summary>
    /// ErrorCorrection 的摘要说明
    /// </summary>
    public class ErrorCorrection : IHttpHandler
    {
        
        public void ProcessRequest(HttpContext context)
        {
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
                case "getOriginalOrders":
                    
                    int shopid =int.Parse(context.Request.QueryString["shopId"]);
                    int guidanceId = int.Parse(context.Request.QueryString["guidanceId"]);
                    result = GetOriginalOrders(guidanceId, shopid);
                    break;
                case "updateOrder":
                    string jsonStr = context.Request.Form["jsonStr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = jsonStr.Replace("+", "%2B");
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    result = UpdateOrders(jsonStr);
                    break;
                case "getSubjects":
                    int guidanceId0 = int.Parse(context.Request.QueryString["guidanceId"]);
                    result = GetProjects(guidanceId0);
                    break;
                case "getShopId":
                    string shopNo = context.Request.QueryString["shopNo"];
                    result = GetShopId(shopNo);
                    break;
                case "clearData":
                    int shopid1 =int.Parse(context.Request.QueryString["shopId"]);
                    int guidanceId1 = int.Parse(context.Request.QueryString["guidanceId"]);
                    result = ClearData(guidanceId1, shopid1);
                    break;
            }
            context.Response.Write(result);
        }

        string GetOriginalOrders(int guidanceId, int shopid)
        {

            var editList=(from order in CurrentContext.DbContext.ADOrderErrorCorrection
                         join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where order.GuidanceId == guidanceId && order.ShopId == shopid
                        select new
                        {
                            order,
                            LevelNum = order.LevelNum,
                            Sheet = order.Sheet,
                            subject
                        }).ToList();
            if (editList.Any())
            {
                StringBuilder json = new StringBuilder();
                editList.ForEach(s =>
                {
                    json.Append("{\"OrderId\":\"" + s.order.OrderId + "\",\"SubjectId\":\"" + s.order.SubjectId + "\",\"SubjectName\":\"" + s.subject.SubjectName + "\",\"ShopId\":\"" + s.order.ShopId + "\",\"ShopNo\":\"" + s.order.ShopNo + "\",\"OrderType\":\"" + s.order.OrderType + "\",\"Sheet\":\"" + s.Sheet + "\",\"LevelNum\":\"" + s.order.LevelNum + "\",\"Gender\":\"" + s.order.Gender + "\",\"Quantity\":\"" + s.order.Quantity + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"UnitPrice\":\"" + s.order.UnitPrice + "\",\"ChooseImg\":\"" + s.order.ChooseImg + "\",\"PositionDescription\":\"" + s.order.Remark + "\",\"State\":\""+s.order.State+"\",\"EditRemark\":\""+s.order.EditRemark+"\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
            {
                var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 where subject.GuidanceId == guidanceId && order.ShopId == shopid
                                 select new
                                 {
                                     order,
                                     LevelNum = order.LevelNum,
                                     Sheet = order.Sheet,
                                     subject
                                 }).ToList();
                if (orderList.Any())
                {
                    StringBuilder json = new StringBuilder();
                    orderList.ForEach(s =>
                    {
                        json.Append("{\"OrderId\":\"" + s.order.Id + "\",\"SubjectId\":\"" + s.order.SubjectId + "\",\"SubjectName\":\"" + s.subject.SubjectName + "\",\"ShopId\":\"" + s.order.ShopId + "\",\"ShopNo\":\"" + s.order.ShopNo + "\",\"OrderType\":\"" + s.order.OrderType + "\",\"Sheet\":\"" + s.Sheet + "\",\"LevelNum\":\"" + s.order.LevelNum + "\",\"Gender\":\"" + s.order.Gender + "\",\"Quantity\":\"" + s.order.Quantity + "\",\"GraphicWidth\":\"" + s.order.GraphicWidth + "\",\"GraphicLength\":\"" + s.order.GraphicLength + "\",\"GraphicMaterial\":\"" + s.order.GraphicMaterial + "\",\"UnitPrice\":\"" + s.order.UnitPrice + "\",\"ChooseImg\":\"" + s.order.ChooseImg + "\",\"PositionDescription\":\"" + s.order.PositionDescription + "\",\"State\":\"0\"},");

                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                return "";
            }
        }

        string GetOrder(string Ids)
        {
            if (!string.IsNullOrWhiteSpace(Ids))
            {
                List<int> list = StringHelper.ToIntList(Ids, ',');
                var orderList = new FinalOrderDetailTempBLL().GetList(s => list.Contains(s.Id)).OrderBy(s => s.ShopId).ThenBy(s=>s.Sheet).ToList();
                if (orderList.Any())
                {
                    StringBuilder json = new StringBuilder();
                    orderList.ForEach(s =>
                    {
                        json.Append("{\"Id\":\"" + s.Id + "\",\"SubjectId\":\"" + s.SubjectId + "\",\"ShopId\":\"" + s.ShopId + "\",\"ShopNo\":\"" + s.ShopNo + "\",\"OrderType\":\"" + s.OrderType + "\",\"Sheet\":\"" + s.Sheet + "\",\"Gender\":\"" + s.Gender + "\",\"Quantity\":\"" + s.Quantity + "\",\"GraphicWidth\":\"" + s.GraphicWidth + "\",\"GraphicLength\":\"" + s.GraphicLength + "\",\"GraphicMaterial\":\"" + s.GraphicMaterial + "\",\"UnitPrice\":\"" + s.UnitPrice + "\",\"ChooseImg\":\"" + s.ChooseImg + "\",\"PositionDescription\":\"" + s.PositionDescription + "\"},");

                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                return "";
            }
            else
                return "";
        }

        /// <summary>
        /// 保存修改记录
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        string UpdateOrders(string jsonStr)
        {
            ADOrderErrorCorrectionBLL bll = new ADOrderErrorCorrectionBLL();
            string result = "提交失败";
            try
            {
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    List<ADOrderErrorCorrection> list = JsonConvert.DeserializeObject<List<ADOrderErrorCorrection>>(jsonStr);
                    if (list.Any())
                    {
                        int guidanceId = list[0].GuidanceId ?? 0;
                        int shopId = list[0].ShopId;
                        bll.Delete(s => s.GuidanceId == guidanceId && s.ShopId==shopId);
                        list.ForEach(s =>
                        {
                            s.AddDate = DateTime.Now;
                            s.AddUserId = new BasePage().CurrentUser.UserId;
                            bll.Add(s);
                        });
                        result = "ok";
                    }

                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }


        string GetProjects(int guidanceId)
        {
            
            var list = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete==null || s.IsDelete==false)).OrderBy(s=>s.SubjectName).ToList();
            if (list.Any())
            {
                StringBuilder jsonStr = new StringBuilder();
                list.ForEach(s => {
                    jsonStr.Append("{\"Id\":\"" + s.Id + "\",\"SubjectName\":\"" + s.SubjectName + "\"},");
                });
                return "["+jsonStr.ToString().TrimEnd(',')+"]";
            }
            return "";

        }

        string GetShopId(string shopNo)
        {
            string result = "error";
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                var model = new ShopBLL().GetList(s => s.ShopNo.ToLower() == shopNo.ToLower()).FirstOrDefault();
                if (model != null)
                    result = model.Id.ToString();
            }
            return result;
        }

        string ClearData(int guidanceId, int shopid)
        {
            //string result = string.Empty;
            new ADOrderErrorCorrectionBLL().Delete(s => s.GuidanceId == guidanceId && s.ShopId==shopid);
            return "ok";
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
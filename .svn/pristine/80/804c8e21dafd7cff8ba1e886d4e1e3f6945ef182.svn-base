using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.Subjects.Handler
{
    /// <summary>
    /// EditOrderDetails 的摘要说明
    /// </summary>
    public class EditOrderDetails : IHttpHandler
    {
        string type = string.Empty;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            switch (type)
            {
                case "getOrderInfo":
                    result = GetOrderInfo();
                    break;
            }
            context.Response.Write(result);
        }

        MergeOriginalOrderBLL mergeBll = new MergeOriginalOrderBLL();
        string GetOrderInfo()
        {
            int year = 0;
            int month = 0;
            //获取上一个月的订单，如果没有就获取全部
            DateTime date = DateTime.Now;
            year = date.Year;
            month = date.Month;
            if (month == 1)
            {
                year--;
                month = 12;
            }
            else
                month--;
            List<MergeOriginalOrder> orderList = new List<MergeOriginalOrder>();
            orderList = mergeBll.GetList(s => s.AddDate.Value.Year == year && s.AddDate.Value.Month == month);
            if (!orderList.Any())
            {
                orderList = mergeBll.GetList(s=>s.Id>0);
            }

            if (orderList.Any())
            {
                List<string> SheetList = new List<string>();
                List<string> GenderList = new List<string>();
                List<string> MaterialSupportList = new List<string>();
                List<string> POSScaleList = new List<string>();
                List<string> LevelNumList = new List<string>();
                orderList.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s.Sheet) && !SheetList.Contains(s.Sheet))
                    {
                        SheetList.Add(s.Sheet);
                    }
                    if (!string.IsNullOrWhiteSpace(s.Gender) && !GenderList.Contains(s.Gender))
                    {
                        GenderList.Add(s.Gender);
                    }
                    if (!string.IsNullOrWhiteSpace(s.MaterialSupport) && !MaterialSupportList.Contains(s.MaterialSupport))
                    {
                        MaterialSupportList.Add(s.MaterialSupport);
                    }
                    if (!string.IsNullOrWhiteSpace(s.POSScale) && !POSScaleList.Contains(s.POSScale))
                    {
                        POSScaleList.Add(s.POSScale);
                    }
                    //if (s.LevelNum!=null && !LevelNumList.Contains(s.LevelNum.ToString()))
                    //{
                    //    LevelNumList.Add(s.LevelNum.ToString());
                    //}
                });
                StringBuilder json = new StringBuilder();
                json.Append("{\"Sheet\":\"" + StringHelper.ListToString(SheetList) + "\",\"Gender\":\"" + StringHelper.ListToString(GenderList) + "\",\"MaterialSupport\":\"" + StringHelper.ListToString(MaterialSupportList) + "\",\"POSScale\":\"" + StringHelper.ListToString(POSScaleList.OrderBy(s => s).ToList()) + "\"}");
                return "[" + json.ToString() + "]";
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
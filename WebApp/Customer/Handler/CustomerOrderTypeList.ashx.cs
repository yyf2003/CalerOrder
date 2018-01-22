using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using System.Text;
using Newtonsoft.Json;
using Common;

namespace WebApp.Customer.Handler
{
    /// <summary>
    /// CustomerOrderTypeList 的摘要说明
    /// </summary>
    public class CustomerOrderTypeList : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request["type"] != null)
            {
                type = context.Request["type"];
            }
            switch (type)
            {
                case "getOrderType":
                    result = GetOrderType();
                    break;
                case "edit":
                    result = Eidt();
                    break;
            }
            context.Response.Write(result);
        }

        string GetOrderType()
        {
            int customerId = 0;
            if (context1.Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(context1.Request.QueryString["customerId"]);
            }
            var list = new CustomerOrderTypeBLL().GetList(s => s.CustomerId == customerId);
           
            StringBuilder ids = new StringBuilder();
            if (list.Any())
            {
                list.ForEach(s => {
                    ids.Append(s.OrderTypeId);
                    ids.Append(',');
                });
                return ids.ToString().TrimEnd(',');
            }
            else
                return "";
        }

        string Eidt()
        {
            string result = "ok";
            string jsonString = string.Empty;
            if (context1.Request.QueryString["jsonString"] != null)
            {
                jsonString = context1.Request.QueryString["jsonString"];
            }
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    CustomerOrderType newModel = JsonConvert.DeserializeObject<CustomerOrderType>(jsonString);
                    if (newModel != null)
                    {
                        CustomerOrderTypeBLL bll = new CustomerOrderTypeBLL();
                        CustomerOrderType model;
                        bll.Delete(s => s.CustomerId == newModel.CustomerId);
                        if (!string.IsNullOrWhiteSpace(newModel.OrderTypeIds))
                        {
                            List<int> ids = StringHelper.ToIntList(newModel.OrderTypeIds, ',');
                            ids.ForEach(s =>
                            {
                                model = new CustomerOrderType();
                                model.CustomerId = newModel.CustomerId;
                                model.OrderTypeId = s;
                                bll.Add(model);
                            });
                        }
                    }
                    else
                        result = "提交数据错误";
                }
                catch (Exception ex)
                {
                    result = ex.Message;
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
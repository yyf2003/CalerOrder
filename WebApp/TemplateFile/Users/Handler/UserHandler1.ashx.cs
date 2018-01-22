using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BLL;
using Models;
using Newtonsoft.Json;
using System.Transactions;

namespace WebApp.Users.Handler
{
    /// <summary>
    /// UserHandler1 的摘要说明
    /// </summary>
    public class UserHandler1 : IHttpHandler
    {
        UserBLL user = new UserBLL();
        string type = string.Empty;
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
                case "getCompany":
                    result = GetCompanyList();
                    break;
                case "getCustomer":
                    result = GetCustomerList();
                    break;
                case "getActivity":
                    result = GetActivity();
                    break;
                case "edit":
                    string jsonString = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = AddUser(jsonString, optype);
                    break;
            }
            context.Response.Write(result);
        }
        StringBuilder companyJson = new StringBuilder();
        string GetCompanyList()
        {
            var list = new CompanyBLL().GetList(s=>1==1);
            if (list.Any())
            {
                var parentList = list.Where(s=>s.ParentId==0);
                if (parentList.Any())
                {
                    parentList.ToList().ForEach(s => {
                        companyJson.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.CompanyName + "\",\"ParentId\":\"" + s.ParentId + "\"");
                        GetChildren(list, s.Id);
                        companyJson.Append("},");
                    });
                }
            }
            return "[" + companyJson.ToString().TrimEnd(',') + "]";
        }

        void GetChildren(IEnumerable<Company> list, int parentId)
        {
            var list1 = list.Where(s => s.ParentId == parentId).ToList();
            if (list1.Any())
            {
                companyJson.Append(" ,\"state\":\"closed\",\"children\":[");
                int index = 0;
                list1.ForEach(s =>
                {
                    if (index > 0)
                    {
                        companyJson.Append(",");
                    }
                    companyJson.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.CompanyName + "\",\"ParentId\":\"" + s.ParentId + "\"");
                    GetChildren(list, s.Id);
                    companyJson.Append("}");
                    index++;
                });
                companyJson.Append("]");
            }
        }

        string GetCustomerList()
        {
            StringBuilder customerJson = new StringBuilder();
            var list = new CustomerBLL().GetList(s=>s.IsDelete==false||s.IsDelete==null);
            if (list.Any())
            {
                list.ForEach(s => {
                    customerJson.Append("{\"CustomerId\":\"" + s.Id + "\",\"CustomerName\":\""+s.CustomerName+"\"},");
                });
                return "[" + customerJson.ToString().TrimEnd(',') + "]";
            }
            else
               return "";
        }

        string GetActivity()
        {
            var list = new ADOrderActivityBLL().GetList(s=>1==1);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"ActivityId\":\"" + s.ActivityId + "\",\"ActivityName\":\"" + s.ActivityName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string AddUser(string jsonString, string optype)
        {
            string result = "提交失败！";
            if (!string.IsNullOrEmpty(jsonString))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        UserInfo model = JsonConvert.DeserializeObject<UserInfo>(jsonString);
                        UserInRoleBLL urBll = new UserInRoleBLL();
                        UserInRole urModel;
                        UserInCustomerBLL ucBll = new UserInCustomerBLL();
                        UserInCustomer ucModel;
                        UserInActivityBLL uaBll=new UserInActivityBLL();
                        UserInActivity uaModel;
                        if (model != null)
                        {
                            if (optype == "add")
                            {
                                if (!CheckUserName(model.UserName, 0))
                                {
                                    string roles = model.Roles;
                                    string customers = model.Customers;
                                    string activies = model.Activities;
                                    model.AddDate = DateTime.Now;
                                    model.PassWord = "1";
                                    user.Add(model);
                                    if (!string.IsNullOrEmpty(roles))
                                    {
                                        string[] rolesArr = roles.Split(',');
                                        foreach (string s in rolesArr)
                                        {
                                            if (!string.IsNullOrEmpty(s))
                                            {
                                                urModel = new UserInRole();
                                                urModel.RoleId = int.Parse(s);
                                                urModel.UserId = model.UserId;
                                                urBll.Add(urModel);
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(customers))
                                    {
                                        string[] customersArr = customers.Split(',');
                                        foreach (string s in customersArr)
                                        {
                                            if (!string.IsNullOrEmpty(s))
                                            {
                                                ucModel = new UserInCustomer();
                                                ucModel.CustomerId = int.Parse(s);
                                                ucModel.UserId = model.UserId;
                                                ucBll.Add(ucModel);
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(activies))
                                    {
                                        string[] activiesArr = activies.Split(',');
                                        
                                        foreach (string s in activiesArr)
                                        {
                                            if (!string.IsNullOrEmpty(s))
                                            {
                                                uaModel = new UserInActivity();
                                                uaModel.ActivityId = int.Parse(s);
                                                uaModel.UserId = model.UserId;
                                                uaBll.Add(uaModel);
                                            }
                                        }
                                    }
                                    result = "ok";
                                }
                                else
                                    result = "exist";
                            }
                            else
                            {
                                if (!CheckUserName(model.UserName, model.UserId))
                                {
                                    UserInfo um = user.GetModel(model.UserId);
                                    if (um != null)
                                    {
                                        um.UserName = model.UserName;
                                        um.RealName = model.RealName;
                                        um.CompanyId = model.CompanyId;
                                        user.Update(um);
                                        urBll.Delete(s => s.UserId == model.UserId);
                                        ucBll.Delete(s => s.UserId == model.UserId);
                                        uaBll.Delete(s=>s.UserId==model.UserId);
                                        string roles = model.Roles;
                                        if (!string.IsNullOrEmpty(roles))
                                        {
                                            string[] rolesArr = roles.Split(',');
                                            foreach (string s in rolesArr)
                                            {
                                                if (!string.IsNullOrEmpty(s))
                                                {
                                                    urModel = new UserInRole();
                                                    urModel.RoleId = int.Parse(s);
                                                    urModel.UserId = model.UserId;
                                                    urBll.Add(urModel);
                                                }
                                            }
                                        }
                                        string customers = model.Customers;
                                        if (!string.IsNullOrEmpty(customers))
                                        {
                                            string[] customersArr = customers.Split(',');
                                            foreach (string s in customersArr)
                                            {
                                                if (!string.IsNullOrEmpty(s))
                                                {
                                                    ucModel = new UserInCustomer();
                                                    ucModel.CustomerId = int.Parse(s);
                                                    ucModel.UserId = model.UserId;
                                                    ucBll.Add(ucModel);
                                                }
                                            }
                                        }
                                        string activies = model.Activities;
                                        if (!string.IsNullOrEmpty(activies))
                                        {
                                            string[] activiesArr = activies.Split(',');

                                            foreach (string s in activiesArr)
                                            {
                                                if (!string.IsNullOrEmpty(s))
                                                {
                                                    uaModel = new UserInActivity();
                                                    uaModel.ActivityId = int.Parse(s);
                                                    uaModel.UserId = model.UserId;
                                                    uaBll.Add(uaModel);
                                                }
                                            }
                                        }
                                        result = "ok";
                                    }
                                    
                                    
                                }
                                else
                                    result = "exist";

                            }
                            
                            
                        }
                        tran.Complete();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return result+"错误信息："+ex.Message;
                    }
                }

                
            }
            return result;
        }

        bool CheckUserName(string userName,int userId)
        {

            var list = user.GetList(s => s.UserName == userName && (userId>0?s.UserId!=userId:1==1));
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
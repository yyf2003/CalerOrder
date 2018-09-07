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
using Common;
using System.IO;
using LitJson;

namespace WebApp.Users.Handler
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {

        UserBLL user = new UserBLL();
        string type = string.Empty;
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
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
                case "getCompany":
                    result = GetCompanyList();
                    break;
                case "getCustomer":
                    result = GetCustomerList();
                    break;
                case "edit":
                    string jsonString = context.Request["jsonString"];
                    string optype = context.Request["optype"];
                    result = AddUser(jsonString, optype);
                    break;
                case "getRegion":
                    //int customerId=int.Parse(context.Request.QueryString["customerId"].ToString());
                    result=GetRegions(); 
                    break;
                case "getProvince":
                    if (context.Request.QueryString["regionId"] != null)
                    {
                        string regionIds = context.Request.QueryString["regionId"].ToString();
                        result = GetProvince(regionIds);
                    }
                    else
                        result = GetProvince();
                    break;
                case "getPlace":
                    string parentIds = context.Request.QueryString["parentId"].ToString();
                    result = GetPlace(parentIds); 
                    break;
                case "getUserLevel":
                    result = GetUserLevel();
                    break;
                case "getModel":
                    int userId = int.Parse(context.Request.QueryString["userId"]);
                    result = GetModel(userId);
                    break;
                case "getOutsource":
                    int userid = 0;
                    int provinceId = 0;
                    int cityId = 0;
                    if (context.Request.QueryString["userId"] != null)
                    {
                        userid = int.Parse(context.Request.QueryString["userId"]);
                    }
                    if (context.Request.QueryString["provinceId"] != null)
                    {
                        provinceId = int.Parse(context.Request.QueryString["provinceId"]);
                    }
                    if (context.Request.QueryString["cityId"] != null)
                    {
                        cityId = int.Parse(context.Request.QueryString["cityId"]);
                    }
                    result = GetOutsourceList(userid, provinceId, cityId);
                    break;
                case "editOutsource":
                    string jsonStr = "";
                    if (context.Request.QueryString["jsonStr"] != null)
                    {
                        jsonStr = context.Request.QueryString["jsonStr"];
                    }
                    result=EditOutsource(jsonStr);
                    break;
                case "getUserRole":
                    int userId1 = 0;
                    if (context.Request.QueryString["userId"] != null)
                    {
                        userId1 = int.Parse(context.Request.QueryString["userId"]);
                    }
                    result = GetUserRole(userId1);
                    break;
                case "getModules":
                    int userid0 = 0;
                    if (context.Request.QueryString["userId"] != null)
                    {
                        userid0 = int.Parse(context.Request.QueryString["userId"]);
                    }
                    int roleId0 = 0;
                    if (context.Request.QueryString["roleId"] != null)
                    {
                        roleId0 = int.Parse(context.Request.QueryString["roleId"]);
                    }
                    result = GetModules(userid0, roleId0);
                    break;
                case "updatePermission":
                    string jsonStr0 = context.Request.Form["jsonStr"];
                    string exportChannelJsonStr0 = string.Empty;
                    if (context.Request.Form["exportChannelJsonStr"] != null)
                    {
                        exportChannelJsonStr0 = context.Request.Form["exportChannelJsonStr"];
                    }
                    result = UpdatePermission(jsonStr0, exportChannelJsonStr0);
                    break;
                case "GetChannel":
                    int userId2 = 0;
                    if (context.Request.QueryString["userId"] != null)
                    {
                        userId2 = int.Parse(context.Request.QueryString["userId"]);
                    }
                    result = GetShopChannels(userId2);
                    break;
            }
            context.Response.Write(result);
        }
        StringBuilder companyJson = new StringBuilder();
        string GetCompanyList()
        {
            var list = new CompanyBLL().GetList(s => 1 == 1);
            if (list.Any())
            {
                var parentList = list.Where(s => s.ParentId == 0);
                if (parentList.Any())
                {
                    parentList.ToList().ForEach(s =>
                    {
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
            var list = new CustomerBLL().GetList(s => s.IsDelete == false || s.IsDelete == null);
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    customerJson.Append("{\"CustomerId\":\"" + s.Id + "\",\"CustomerName\":\"" + s.CustomerName + "\"},");
                });
                return "[" + customerJson.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetUserLevel()
        {
            var list = new UserLevelBLL().GetList(s=>s.LevelId>0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"LevelId\":\"" + s.LevelId + "\",\"LevelName\":\"" + s.LevelName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
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
                        UserInRegionBLL userRegionBll = new UserInRegionBLL();
                        UserInRegion userRegionModel;
                        if (model != null)
                        {
                            if (optype == "add")
                            {
                                if (!CheckUserName(model.UserName, 0))
                                {
                                    string roles = model.Roles;
                                    string customers = model.Customers;
                                    string regions = model.Regions;
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
                                                if (!string.IsNullOrEmpty(regions))
                                                {
                                                    string[] regionsArr = regions.Split('|');
                                                    userRegionModel = new UserInRegion();
                                                    userRegionModel.RegionId = regionsArr[0];
                                                    userRegionModel.ProvinceId = regionsArr[1];
                                                    userRegionModel.CityId = regionsArr[2];
                                                    userRegionModel.AreaId = regionsArr[3];
                                                    userRegionModel.UserId = model.UserId;
                                                    userRegionModel.RoleId = int.Parse(s);
                                                    userRegionBll.Add(userRegionModel);
                                                }
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
                                        um.UserLevelId = model.UserLevelId;
                                        user.Update(um);
                                        urBll.Delete(s => s.UserId == model.UserId);
                                        ucBll.Delete(s => s.UserId == model.UserId);
                                        userRegionBll.Delete(s=>s.UserId==model.UserId);
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
                                                    string regions = model.Regions;
                                                    if (!string.IsNullOrEmpty(regions))
                                                    {
                                                        string[] regionsArr = regions.Split('|');
                                                        userRegionModel = new UserInRegion();
                                                        userRegionModel.RegionId = regionsArr[0];
                                                        userRegionModel.ProvinceId = regionsArr[1];
                                                        userRegionModel.CityId = regionsArr[2];
                                                        userRegionModel.AreaId = regionsArr[3];
                                                        userRegionModel.UserId = model.UserId;
                                                        userRegionModel.RoleId = int.Parse(s);
                                                        userRegionBll.Add(userRegionModel);
                                                    }
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
                        return result + "错误信息：" + ex.Message;
                    }
                }


            }
            return result;
        }

        bool CheckUserName(string userName, int userId)
        {

            var list = user.GetList(s => s.UserName == userName && (userId > 0 ? s.UserId != userId : 1 == 1));
            return list.Any();
        }

        string GetRegions() 
        {
            var list = new RegionBLL().GetList(s=>s.IsDelete==null ||s.IsDelete==false).OrderBy(s=>s.Id).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"RegionName\":\""+s.RegionName+"\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }

        string GetProvince(string regionIds)
        {
            if (!string.IsNullOrWhiteSpace(regionIds))
            {
                List<int> regionIdList = StringHelper.ToIntList(regionIds,',');
                var list = (from pr in CurrentContext.DbContext.ProvinceInRegion
                            join region in CurrentContext.DbContext.Region
                            on pr.RegionId equals region.Id
                            join province in CurrentContext.DbContext.Place
                            on pr.ProvinceId equals province.ID
                            where regionIdList.Contains(pr.RegionId??0)
                            select new
                            {
                                province,
                                region
                            }).OrderBy(s => s.region.Id).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        json.Append("{\"RegionId\":\""+s.region.Id+"\",\"RegionName\":\""+s.region.RegionName+"\",\"ProvinceId\":\"" + s.province.ID + "\",\"ProvinceName\":\"" + s.province.PlaceName + "\"},");

                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                return "";
            }
            return "";
        }

        string GetProvince()
        {
            var list = new PlaceBLL().GetList(s=>s.ParentID==0);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"ProvinceId\":\"" + s.ID + "\",\"ProvinceName\":\"" + s.PlaceName + "\"},");

                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetPlace(string parentIds)
        {
            if (!string.IsNullOrWhiteSpace(parentIds))
            {
                List<int> parentIdList = StringHelper.ToIntList(parentIds, ',');
                //var list = new PlaceBLL().GetList(s => parentIdList.Contains(s.ParentID ?? 0)).OrderBy(s=>s.ParentID).ToList();
                var list = (from place in CurrentContext.DbContext.Place
                           join parent in CurrentContext.DbContext.Place
                           on place.ParentID equals parent.ID
                           where parentIdList.Contains(place.ParentID ?? 0)
                           select new { 
                              place.ParentID,
                              ParentName=parent.PlaceName,
                              place.ID,
                              place.PlaceName
                           }).OrderBy(s => s.ParentID).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        json.Append("{\"ParentId\":\""+s.ParentID+"\",\"ParentName\":\""+s.ParentName+"\",\"PlaceId\":\"" + s.ID + "\",\"PlaceName\":\"" + s.PlaceName + "\"},");

                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                return "";
            
            }
            return "";
        }

        string GetModel(int userId)
        {
            var model = user.GetModel(userId);
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                StringBuilder Roles = new StringBuilder();
                StringBuilder Customers = new StringBuilder();
                StringBuilder Regions = new StringBuilder();
                var uRoleList = new UserInRoleBLL().GetList(s=>s.UserId==userId);
                uRoleList.ForEach(s => {
                    Roles.Append(s.RoleId+",");
                });
                var uCustomerList = new UserInCustomerBLL().GetList(s => s.UserId == userId);
                uCustomerList.ForEach(s =>
                {
                    Customers.Append(s.CustomerId + ",");
                });
                var uRegion = new UserInRegionBLL().GetList(s => s.UserId == userId).FirstOrDefault();
                if (uRegion != null)
                {
                    Regions.Append(uRegion.RegionId + "|" + uRegion.ProvinceId + "|" + uRegion.CityId + "|" + uRegion.AreaId);
                }
                //uRegionList.ForEach(s =>
                //{
                //    Regions.Append(s.RegionId + ",");
                //});
                json.Append("{\"UserId\":\"" + model.UserId + "\",\"CompanyId\":\"" + model.CompanyId + "\",\"UserName\":\"" + model.UserName + "\",\"RealName\":\"" + model.RealName + "\",\"UserLevelId\":\"" + model.UserLevelId + "\",\"Roles\":\"" + Roles.ToString().TrimEnd(',') + "\",\"Customers\":\"" + Customers.ToString().TrimEnd(',') + "\",\"Regions\":\"" + Regions.ToString() + "\"}");
                return "["+json.ToString()+"]";
            }
            return "";
        }

        string GetOutsourceList(int userId,int provinceId,int cityId)
        {
            List<int> userOutsourceList = new OutsourceInUserBLL().GetList(s => s.UserId == userId).Select(s=>s.OutsourceId??0).ToList();
            var outsourceList = new CompanyBLL().GetList(s => s.TypeId==(int)CompanyTypeEnum.Outsource &&  (s.IsDelete == null || s.IsDelete == false));
            if (provinceId > 0)
            {
                outsourceList = outsourceList.Where(s => s.ProvinceId == provinceId).ToList();
            }
            if (cityId > 0)
            {
                outsourceList = outsourceList.Where(s => s.CityId == cityId).ToList();
            }
            if (outsourceList.Any())
            {
                StringBuilder json = new StringBuilder();
                outsourceList.OrderBy(s=>s.ProvinceId).ToList().ForEach(s => {
                    int selected = 0;
                    if (userOutsourceList.Contains(s.Id))
                        selected = 1;
                    json.Append("{\"OutsourceId\":\"" + s.Id + "\",\"OutsourceName\":\"" + s.CompanyName + "\",\"Selected\":\"" + selected + "\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }

        string EditOutsource(string jsonStr)
        {
            string result = "ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                OutsourceInUserBLL oiuBll = new OutsourceInUserBLL();
                OutsourceInUser oiuModel;
                List<OutsourceInUser> list = JsonConvert.DeserializeObject<List<OutsourceInUser>>(jsonStr);
                if (list.Any())
                {
                    int userId = list[0].UserId ?? 0;
                    oiuBll.Delete(s => s.UserId == userId);
                    list.ForEach(s =>
                    {
                        oiuModel = new OutsourceInUser();
                        oiuModel.OutsourceId = s.OutsourceId;
                        oiuModel.UserId = s.UserId;
                        oiuBll.Add(oiuModel);
                    });
                }
                else
                    result = "没有可以提交";
            }
            else
                result = "没有可以提交";
            return result;

        }

        string GetOrderType(int userId)
        {
            if (userId > 0)
            {
                OrderApprovePermissionBLL permissionBll = new OrderApprovePermissionBLL();
                List<int> assignList = permissionBll.GetList(s => s.UserId == userId).Select(s=>s.OrderTypeId??0).ToList();
                var orderTypeList = new OrderTypeBLL().GetList(s=>s.IsDelete==null || s.IsDelete==false);
                if (orderTypeList.Any())
                {
                    StringBuilder json = new StringBuilder();
                }
            }
            return "";
        }

        string GetUserRole(int userId)
        {
            var list = (from uRole in CurrentContext.DbContext.UserInRole
                       join role in CurrentContext.DbContext.Role
                       on uRole.RoleId equals role.RoleId
                       where uRole.UserId == userId
                       select role).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int index = 0;
                list.ForEach(s=>{
                    if (index == 0)
                    {
                        json.Append("{\"RoleId\":\"" + s.RoleId + "\",\"RoleName\":\"" + s.RoleName + "\",\"selected\":\"true\"},");
                    }
                    else
                    {
                        json.Append("{\"RoleId\":\"" + s.RoleId + "\",\"RoleName\":\"" + s.RoleName + "\"},");
                    }
                    index++;
                });
                return "["+json.ToString().TrimEnd(',')+"]";
                
            }
            return "";
        }


        ModuleBLL moduleBll = new ModuleBLL();
        UserInModuleBLL userInModuleBll = new UserInModuleBLL();
        //UserInModule userInModuleModel;
        List<BaseInfo> operateList = new List<BaseInfo>();
        //string GetModules(int userId)
        //{
        //    var list = moduleBll.GetList(s => s.Id > 0);
            
        //    StringBuilder json = new StringBuilder();
        //    if (list.Any())
        //    {
        //        BaseBLL baseBll = new BaseBLL();
        //        List<BaseInfo> operateList = baseBll.GetList(s => s.BaseCategoryId == (int)CategoryTypeEnum.OperateType).ToList();

        //        var permissionList = userInModuleBll.GetList(s => s.UserId == userId);
        //        list.ToList().ForEach(s =>
        //        {
        //            string open = string.Empty;
        //            //if (s.ParentId > 0)
        //                //open = "\"open\":true,";
        //            json.Append("{\"id\":\"" + s.Id + "\",\"pId\":\"" + s.ParentId + "\",\"name\":\"" + s.ModuleName + "\"," + open + "},");

        //            //操作类型!list.Where(s1 => s1.ParentId == s.Id).Any() &&
        //            if (s.ParentId > 0 && operateList.Any())
        //            {
        //                userInModuleModel = permissionList.FirstOrDefault(p => p.ModuleId == s.Id);
        //                List<string> permissions = new List<string>();
        //                if (userInModuleModel != null && userInModuleModel.OperatePermission != null)
        //                {
        //                    permissions = userInModuleModel.OperatePermission.Split('|').ToList();

        //                }
        //                StringBuilder operateStr = new StringBuilder();
        //                operateList.ForEach(o =>
        //                {
        //                    string cheked = string.Empty;
        //                    if (permissions.Contains(o.BaseCode))
        //                        cheked = "checked='checked'";
        //                    string checkbox = "<input type='checkbox' " + cheked + "  class='cbPermission' value='" + o.BaseCode + "'/>" + o.BaseName;
        //                    operateStr.Append(checkbox);
        //                    operateStr.Append("&nbsp;&nbsp;");
        //                    string opStr = "<div class='divPermission' data-moduleid='" + s.Id + "'>" + operateStr.ToString() + "</div>";
        //                    json.Append("{\"id\":\"" + o.BaseCode + "\",\"pId\":\"" + s.Id + "\",\"name\":\"" + o.BaseName + "\",\"OperateStr\":\"" + opStr + "\"},");
        //                });
                        
        //            }

        //        });
        //        if (json.Length > 0)
        //        {
        //            return "[" + json.ToString().TrimEnd(',') + "]";
        //        }
        //        else
        //            return "[]";
        //    }
        //    else
        //        return "[]";
        //}
        List<UserInModule> userPermissionList = new List<UserInModule>();
        List<RoleInModule> rolePermissionList = new List<RoleInModule>();

        string GetModules(int userId, int roleId)
        {
            ModuleBLL moduleBll = new ModuleBLL();
            List<Module> list = moduleBll.GetList(s => s.Id > 0);
            List<Module> newList = list;
            if (userId > 0)
            {
                userPermissionList = userInModuleBll.GetList(s => s.UserId == userId && s.RoleId==roleId);
                rolePermissionList = new RoleInModuleBLL().GetList(s => s.RoleId == roleId);
                if (newList.Any())
                {

                    operateList = new BaseBLL().GetList(s => s.BaseCategoryId == (int)CategoryTypeEnum.OperateType).ToList();
                    var list1 = newList.Where(s => s.ParentId == 0).OrderBy(s => s.OrderNum).ToList();
                    list1.ForEach(s =>
                    {
                        json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\",\"ParentId\":\"" + s.ParentId + "\",\"OrderNum\":\"" + s.OrderNum + "\",\"OperateStr\":\"\"");
                        GetModule(list, s.Id);
                        json.Append("},");
                    });

                }
            }
            
            return "[" + json.ToString().TrimEnd(',') + "]";


        }

        StringBuilder json = new StringBuilder();
       
        void GetModule(IEnumerable<Module> list, int parentId)
        {
            var list1 = list.Where(s => s.ParentId == parentId).OrderBy(s => s.OrderNum).ToList();
            if (list1.Any())
            {
                json.Append(" ,\"state\":\"closed\",\"children\":[");

                int index = 0;
                list1.ForEach(s =>
                {
                    if (index > 0)
                    {
                        json.Append(",");
                    }
                    //该用户，该角色的操作权限
                    List<string> roleOperateList = new List<string>();
                    var roleInModuleModel = rolePermissionList.FirstOrDefault(m => m.ModuleId == s.Id);
                    if (roleInModuleModel != null && roleInModuleModel.OperatePermission != null)
                    {
                        roleOperateList = roleInModuleModel.OperatePermission.Split('|').ToList();
                    }
                    var userInModuleModel = userPermissionList.FirstOrDefault(m => m.ModuleId == s.Id);
                    StringBuilder operateStr = new StringBuilder();
                    operateList.ForEach(o =>
                    {
                        string cheked = string.Empty;
                        string disabled = string.Empty;
                        if (roleOperateList.Contains(o.BaseCode))
                        {
                            disabled = "disabled='disabled'";
                        }
                        if (string.IsNullOrWhiteSpace(disabled))
                        {
                            
                            if (userInModuleModel != null && userInModuleModel.OperatePermission != null)
                            {
                                List<string> permissions = new List<string>();
                                permissions = userInModuleModel.OperatePermission.Split('|').ToList();
                                if (permissions.Contains(o.BaseCode))
                                    cheked = "checked='checked'";

                            }
                        }
                        string checkbox = "<input type='checkbox' " + cheked + " " + disabled + " class='cbPermission' value='" + o.BaseCode + "'/>" + o.BaseName;
                        operateStr.Append(checkbox);
                        operateStr.Append("&nbsp;&nbsp;");
                    });
                    string opStr = "<div class='divPermission' data-moduleid='" + s.Id + "'>" + operateStr.ToString() + "</div>";

                    json.Append("{\"id\":\"" + s.Id + "\",\"text\":\"" + s.ModuleName + "\",\"Url\":\"" + s.Url + "\",\"ParentId\":\"" + s.ParentId + "\",\"OrderNum\":\"" + s.OrderNum + "\",\"OperateStr\":\"" + opStr + "\"");
                    GetModule(list, s.Id);
                    json.Append("}");
                    index++;
                });
                json.Append("]");
            }
        }

        string UpdatePermission(string jsonStr, string exportChannelJsonStr0=null)
        {
            string result = "ok";
            int userId = 0;
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        List<UserInModule> moduleList = JsonConvert.DeserializeObject<List<UserInModule>>(jsonStr);
                        if (moduleList.Any())
                        {
                            userId = moduleList[0].UserId??0;
                            int roleId = moduleList[0].RoleId ?? 0;
                            userInModuleBll.Delete(s => s.RoleId == roleId && s.UserId == userId);
                            moduleList.ForEach(s => {
                                if(s.ModuleId!=null && s.ModuleId>0)
                                   userInModuleBll.Add(s);
                            });
                            tran.Complete();
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        result = "提交失败："+ex.Message;
                    }
                }
            }
            else
                result = "提交失败";
            if (result == "ok")
            {
                //string filePath = "/ConfigFile/Config.txt";//配置文件路径
                string filePath = new BasePage().UserExportShopChannelPath;
                if (File.Exists(context1.Server.MapPath(filePath)))
                {
                    JsonData jsonData = JsonMapper.ToObject(File.ReadAllText(context1.Server.MapPath(filePath)));
                    List<ExportPermissionContent> contentList = new List<ExportPermissionContent>();
                    foreach (JsonData item in jsonData)
                    {
                        string PermissionType = item["PermissionType"].ToString();
                        if (PermissionType == ConfigPermissionTypeEnum.Export.ToString())
                        {
                            JsonData PermissionContents = item["PermissionContent"];
                            List<ExportPermissionContent> exportChannelList = JsonConvert.DeserializeObject<List<ExportPermissionContent>>(PermissionContents.ToJson());
                            exportChannelList = exportChannelList.Where(s => s.UserId != userId).ToList();
                            PermissionContents.Clear();
                            if (exportChannelList.Any())
                            {
                                exportChannelList.ForEach(s =>
                                {
                                    JsonData jd = JsonMapper.ToObject(JsonMapper.ToJson(s));// new JsonData(JsonMapper.ToJson(s).Replace(@"\", ""));
                                    PermissionContents.Add(jd);
                                });

                            }
                            if (!string.IsNullOrWhiteSpace(exportChannelJsonStr0))
                            {
                                JsonData jsonDataAdd = JsonMapper.ToObject(exportChannelJsonStr0);
                                foreach (JsonData jd in jsonDataAdd)
                                {
                                    PermissionContents.Add(jd);
                                }
                            }
                        }

                    }
                    string jsond = jsonData.ToJson();
                    StreamWriter sw = new StreamWriter(context1.Server.MapPath(filePath));
                    sw.Write(jsond); //写入数据
                    sw.Close();	//关闭流
                    sw.Dispose();
                }
            }
            return result;
        }

        string GetShopChannels(int userId)
        {
            string result = string.Empty;
            var list = new ShopBLL().GetList(s => s.Channel != null && s.Channel != "");
            if (list.Any())
            {
                var channelList = list.Select(s=>s.Channel).Distinct().OrderBy(s=>s).ToList();
                StringBuilder json = new StringBuilder();
                channelList.ForEach(channel => {
                    StringBuilder formatJson = new StringBuilder();
                    if (!string.IsNullOrWhiteSpace(channel))
                    {
                        json.Append("{\"Channel\":\"" + channel + "\",\"Formats\":");
                        var formatList = list.Where(s => s.Channel == channel).Select(s=>s.Format).Distinct().OrderBy(s=>s).ToList();
                        
                        formatList.ForEach(format => {
                            if (!string.IsNullOrWhiteSpace(format))
                               formatJson.Append("{\"Format\":\"" + format + "\"},");
                        });
                        
                    }
                    json.Append("[" + formatJson.ToString().TrimEnd(',') + "]},");
                });
                result= "[" + json.ToString().TrimEnd(',') + "]";
                string exportPersimisson= GetUserExportPermission(userId);
                if (!string.IsNullOrWhiteSpace(exportPersimisson))
                {
                    result += "|" + exportPersimisson;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取用户导出权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUserExportPermission(int userId)
        {
            string result = string.Empty;
            //string filePath = "/ConfigFile/Config.txt";
            string filePath = new BasePage().UserExportShopChannelPath;

            if (File.Exists(context1.Server.MapPath(filePath)))
            {
                List<ExportPermissionContent> contentList = new List<ExportPermissionContent>();
                JsonData jsonData = JsonMapper.ToObject(File.ReadAllText(context1.Server.MapPath(filePath)));
                foreach (JsonData item in jsonData)
                {
                    string PermissionType = item["PermissionType"].ToString();
                    JsonData PermissionContents = item["PermissionContent"];

                    if (PermissionType == ConfigPermissionTypeEnum.Export.ToString())
                    {

                        foreach (JsonData subItem in PermissionContents)
                        {
                            ExportPermissionContent permissionContent = new ExportPermissionContent();
                            permissionContent.Channel = subItem["Channel"].ToString();
                            permissionContent.Format = subItem["Format"].ToString();
                            permissionContent.UserId = int.Parse(subItem["UserId"].ToString());
                            contentList.Add(permissionContent);
                        }
                    }

                }
                if (contentList.Any())
                {
                    contentList = contentList.Where(s => s.UserId == userId).ToList();
                    if (contentList.Any())
                    {
                        StringBuilder channelJson = new StringBuilder();
                        contentList.ForEach(s =>
                        {
                            channelJson.Append("{\"Channel\":\"" + s.Channel + "\",\"Format\":\"" + s.Format + "\"},");
                        });
                        result = "[" + channelJson.ToString().TrimEnd(',') + "]";
                    }
                }
            }
            else
            {
                JsonData jsonData = new JsonData();
                jsonData.SetJsonType(JsonType.Array);

                JsonData PermissionContent = new JsonData();
                PermissionContent.SetJsonType(JsonType.Array);
                

                JsonData jsonData1 = new JsonData();
                jsonData1["PermissionType"] = "Export";
                jsonData1["PermissionContent"] = PermissionContent;
                jsonData.Add(jsonData1);

                JsonData jsonData2 = new JsonData();
                jsonData2["PermissionType"] = "Approve";
                jsonData2["PermissionContent"] = PermissionContent;
                jsonData.Add(jsonData2);

                


                string jsond = jsonData.ToJson();
                StreamWriter sw = new StreamWriter(context1.Server.MapPath(filePath));
                sw.Write(jsond); //写入数据
                sw.Close();	//关闭流
                sw.Dispose();	
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
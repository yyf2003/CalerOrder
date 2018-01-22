using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using DAL;
using Newtonsoft.Json;
using System.Text;
using Common;

namespace WebApp.Outsource.handler
{
    /// <summary>
    /// List 的摘要说明
    /// </summary>
    public class List : IHttpHandler
    {
        HttpContext context1;
        CompanyBLL companyBll = new CompanyBLL();
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
                case "getList":
                    result = GetList();
                    break;
                case "edit":
                    result = Edit();
                    break;
                case "delete":
                    result = Delete();
                    break;
                case "recover":
                    result = Recover();
                    break;
                case "getCustomerService":
                    result=GetCustomerService();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetList()
        {
            
            int currPage = 0;
            int pageSize = 0;
            //string whereStr = string.Empty;
            if (context1.Request.QueryString["currpage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            #region
            //var list = (from company in CurrentContext.DbContext.OutsourceInfo
            //            join place in CurrentContext.DbContext.Place
            //            on company.ProvinceId equals place.ID into ProvinceTemp
            //            join place1 in CurrentContext.DbContext.Place
            //            on company.CityId equals place1.ID into CityTemp
            //            from Province in ProvinceTemp.DefaultIfEmpty()
            //            from City in CityTemp.DefaultIfEmpty()
            //            select new
            //            {
            //                company,
            //                ProvinceName = Province != null ? Province.PlaceName : "",
            //                CityName = City != null ? City.PlaceName : ""
            //            }).ToList();

            //if (list.Any())
            //{
            //    StringBuilder json = new StringBuilder();
            //    int totalCount = list.Count;
            //    list = list.OrderByDescending(s => s.company.Id).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
            //    int rowIndex = 1;
            //    list.ForEach(s =>
            //    {
            //        //string state = s.company.State == 1 ? "正常" : "未激活";
            //        string provinceName = string.Empty;
            //        string cityName = string.Empty;
            //        if (!string.IsNullOrWhiteSpace(s.company.Provinces))
            //            provinceName = GetPlace(s.company.Provinces);
            //        if (!string.IsNullOrWhiteSpace(s.company.Cities))
            //            cityName = GetPlace(s.company.Cities);
            //        json.Append("{\"Id\":\"" + s.company.Id + "\",\"RowIndex\":\"" + (rowIndex++) + "\",\"OutsourceName\":\"" + s.company.OutsourceName + "\",\"CompanyCode\":\"" + s.company.CompanyCode + "\",\"ShortName\":\"" + s.company.ShortName + "\",\"ProvinceName\":\"" + s.ProvinceName + "\",\"ProvinceId\":\""+s.company.ProvinceId+"\",\"CityName\":\"" + s.CityName + "\",\"CityId\":\""+s.company.CityId+"\",\"InchargeProvinceName\":\""+provinceName+"\",\"InchargeProvinceIds\":\"" + s.company.Provinces + "\",\"InchargeCityName\":\""+cityName+"\",\"InchargeCityIds\":\"" + s.company.Cities + "\",\"JoinDate\":\"" + s.company.JoinDate + "\",\"Contacts\":\"" + s.company.Contacts + "\",\"Tel\":\"" + s.company.Tel + "\",\"Address\":\"" + s.company.Address + "\",\"ERPHost\":\"" + s.company.ERPHost + "\",\"Remark\":\"" + s.company.Remark + "\",\"State\":\"" + (s.company.State??1) + "\"},");

            //    });
            //    return "[" + json.ToString().TrimEnd(',') + "]";
            //}
            #endregion
            int typeId=(int)CompanyTypeEnum.Outsource;
            var list = (from company in CurrentContext.DbContext.Company
                        join province1 in CurrentContext.DbContext.Place
                        on company.ProvinceId equals province1.ID into provinceTemp
                        from province in provinceTemp.DefaultIfEmpty()
                        join city1 in CurrentContext.DbContext.Place
                        on company.CityId equals city1.ID into cityTemp
                        from city in cityTemp.DefaultIfEmpty()
                        join user1 in CurrentContext.DbContext.UserInfo
                        on company.CustomerServiceId equals user1.UserId into userTemp
                        from user in userTemp.DefaultIfEmpty()
                        where company.TypeId == typeId
                        select new
                        {
                            company,
                            Province = province.PlaceName,
                            City = city.PlaceName,
                            user.RealName
                        }).ToList();
            
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = list.Count;
                list = list.OrderBy(s => s.company.TypeId).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                CompanyInRegionBLL companyRegionBll = new CompanyInRegionBLL();
                list.ForEach(s =>
                {
                    string inchargeProvinceIds = string.Empty;
                    string inchargeCityIds = string.Empty;
                    string inchargeProvince = string.Empty;
                    string inchargeCity = string.Empty;
                    var model = companyRegionBll.GetList(r => r.CompanyId == s.company.Id).FirstOrDefault();
                    if (model != null)
                    {
                        inchargeProvinceIds = model.ProvinceId;
                        inchargeCityIds = model.CityId;
                        if (!string.IsNullOrWhiteSpace(inchargeProvinceIds))
                            inchargeProvince = GetPlace(inchargeProvinceIds);
                        if (!string.IsNullOrWhiteSpace(inchargeCityIds))
                            inchargeCity = GetPlace(inchargeCityIds);
                    }
                    string joinDate = s.company.JoinDate != null ? DateTime.Parse(s.company.JoinDate.ToString()).ToShortDateString() : "";
                    string state = s.company.IsDelete != null && s.company.IsDelete == true ? "0" : "1";
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.company.Id + "\",\"TypeId\":\"" + s.company.TypeId + "\",\"CompanyName\":\"" + s.company.CompanyName + "\",\"ShortName\":\"" + s.company.ShortName + "\",\"ParentId\":\"" + s.company.ParentId + "\",\"ProvinceId\":\"" + s.company.ProvinceId + "\",\"CityId\":\"" + s.company.CityId + "\",\"Province\":\"" + s.Province + "\",\"City\":\"" + s.City + "\",\"Contact\":\"" + s.company.Contact + "\",\"Tel\":\"" + s.company.Tel + "\",\"Address\":\"" + s.company.Address + "\",\"State\":\"" + state + "\",\"ProvinceIds\":\"" + inchargeProvinceIds + "\",\"CityIds\":\"" + inchargeCityIds + "\",\"InchargeProvince\":\"" + inchargeProvince + "\",\"InchargeCity\":\"" + inchargeCity + "\",\"JoinDate\":\"" + joinDate + "\",\"CompanyCode\":\"" + s.company.CompanyCode + "\",\"CustomerServiceId\":\"" + s.company.CustomerServiceId + "\",\"CustomerServiceName\":\""+s.RealName+"\"},");
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

        string Edit()
        {
            string r = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.Form["jsonStr"] != null)
            {
                jsonStr = context1.Request.Form["jsonStr"];
               if (!string.IsNullOrWhiteSpace(jsonStr))
               {
                   try
                   {
                       jsonStr = jsonStr.Replace("+", "%2B");
                       jsonStr = HttpUtility.UrlDecode(jsonStr);
                       Company model = JsonConvert.DeserializeObject<Company>(jsonStr);
                       CompanyInRegion companyRegionModel;
                       CompanyInRegionBLL companyInRegionBll = new CompanyInRegionBLL();
                       if (model != null)
                       {
                           if (model.Id > 0)
                           {
                               if (!IsExist(model.Id, model.CompanyName))
                               {
                                   Company newModel = companyBll.GetModel(model.Id);
                                   if (newModel != null)
                                   {
                                       //newModel.CompanyCode = model.CompanyCode;
                                       newModel.Address = model.Address;
                                       newModel.CityId = model.CityId;
                                       newModel.CompanyName = model.CompanyName;
                                       newModel.Contact = model.Contact;
                                       newModel.ProvinceId = model.ProvinceId;
                                       newModel.ShortName = model.ShortName;
                                       newModel.Tel = model.Tel;
                                       newModel.JoinDate = model.JoinDate;
                                       newModel.CustomerServiceId = model.CustomerServiceId;
                                       companyBll.Update(newModel);
                                       companyInRegionBll.Delete(s => s.CompanyId == newModel.Id);
                                       if (!string.IsNullOrEmpty(model.Regions))
                                       {
                                           string[] regionsArr = model.Regions.Split('|');
                                           companyRegionModel = new CompanyInRegion();
                                           companyRegionModel.ProvinceId = regionsArr[0];
                                           companyRegionModel.CityId = regionsArr[1];
                                           companyRegionModel.CompanyId = newModel.Id;
                                           companyInRegionBll.Add(companyRegionModel);
                                       }
                                   }
                               }
                               else
                                   r = "exist";
                           }
                           else
                           {
                               if (!IsExist(0, model.CompanyName))
                               {
                                   model.CompanyCode = CreateCompanyCode();
                                   model.AddDate = DateTime.Now;
                                   model.IsDelete = false;
                                   model.ParentId = 1;
                                   model.TypeId = (int)CompanyTypeEnum.Outsource;
                                  
                                   companyBll.Add(model);
                                   if (!string.IsNullOrEmpty(model.Regions))
                                   {
                                       string[] regionsArr = model.Regions.Split('|');
                                       companyRegionModel = new CompanyInRegion();
                                       companyRegionModel.ProvinceId = regionsArr[0];
                                       companyRegionModel.CityId = regionsArr[1];
                                       companyRegionModel.CompanyId = model.Id;

                                       companyInRegionBll.Add(companyRegionModel);
                                   }

                               }
                               else
                                   r = "exist";
                           }
                       }
                   }
                   catch (Exception ex)
                   {
                       r = "提交失败："+ex.Message;
                   }
               }
            }
            
            return r;
        }

        string Delete()
        {
            string result = "删除失败！";
            string ids = string.Empty;
            if (context1.Request.Form["ids"] != null)
            {
                ids = context1.Request.Form["ids"];
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    try
                    {
                        ids = ids.TrimEnd(',');
                        List<int> idList = Common.StringHelper.ToIntList(ids, ',');
                        Company model;
                        idList.ForEach(s => {
                            model = companyBll.GetModel(s);
                            if (model != null)
                            {
                                model.IsDelete = true;
                                companyBll.Update(model);
                            }
                        });
                        result = "ok";
                    }
                    catch (Exception ex)
                    {
                        result = "删除失败：" + ex.Message;
                    }
                }
            }
            return result;
        }

        string Recover()
        {
            string result = "恢复失败！";
            string ids = string.Empty;
            if (context1.Request.Form["ids"] != null)
            {
                ids = context1.Request.Form["ids"];
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    try
                    {
                        ids = ids.TrimEnd(',');
                        List<int> idList = Common.StringHelper.ToIntList(ids, ',');
                        Company model;
                        idList.ForEach(s =>
                        {
                            model = companyBll.GetModel(s);
                            if (model != null)
                            {
                                model.IsDelete = false;
                                companyBll.Update(model);
                            }
                        });
                        result = "ok";
                    }
                    catch (Exception ex)
                    {
                        result = "恢复失败：" + ex.Message;
                    }
                }
            }
            return result;
        }


        List<Place> placeList = new List<Place>();
        string GetPlace(string ids)
        {
            string r = string.Empty;
            if (!string.IsNullOrWhiteSpace(ids))
            {
                if (!placeList.Any())
                {
                    placeList = new PlaceBLL().GetList(s => s.ID > 0);
                }
                if (placeList.Any())
                {
                    List<int> idList = StringHelper.ToIntList(ids, ',');
                    var list = placeList.Where(s => idList.Contains(s.ID)).ToList();
                    if (list.Any())
                    {
                        List<string> places = list.Select(s => s.PlaceName).Distinct().ToList();
                        r = StringHelper.ListToString(places);
                    }

                }
            }
            return r;
        }

        bool IsExist(int id, string name)
        {
            var list = companyBll.GetList(s =>s.CompanyName.ToLower() == name.ToLower() && (id > 0 ? (s.Id != id) : 1 == 1));
            return list.Any();
        }

        string CreateCompanyCode()
        {
            int index = ((from company in CurrentContext.DbContext.Company
                        select company.Id).Max())+1;
            return "A17" + index.ToString().PadLeft(4, '0');
        }

        string GetCustomerService()
        {
            var list = (
                        from user in CurrentContext.DbContext.UserInfo
                        join userRole in CurrentContext.DbContext.UserInRole
                        on user.UserId equals userRole.UserId
                        where userRole.RoleId == 5
                        && (user.IsDelete == null || user.IsDelete == false)
                        select user).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"UserId\":\""+s.UserId+"\",\"UserName\":\""+s.RealName+"\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
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
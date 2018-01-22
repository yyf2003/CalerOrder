using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using DAL;
using Models;
using System.Text;
using Newtonsoft.Json;
using Common;

namespace WebApp.CompanyManage.handler
{
    /// <summary>
    /// List1 的摘要说明
    /// </summary>
    public class List1 : IHttpHandler
    {
        CompanyBLL companyBll = new CompanyBLL();
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
                type = context.Request.QueryString["type"];
            else if (context.Request.Form["type"] != null)
                type = context.Request.Form["type"];
            switch (type)
            {
                case "getTypeList":
                    result = GetTypeList();
                    break;
                case "getList":
                    int typeId = 0;
                    if (context.Request.QueryString["typeId"] != null)
                        typeId = int.Parse(context.Request.QueryString["typeId"]);
                    int currPage = 0;
                    int pageSize = 0;
                    if (context.Request.QueryString["currpage"] != null)
                    {
                        currPage = int.Parse(context.Request.QueryString["currpage"]);
                    }
                    if (context.Request.QueryString["pagesize"] != null)
                    {
                        pageSize = int.Parse(context.Request.QueryString["pagesize"]);
                    }
                    result = GetList(typeId, currPage, pageSize);
                    break;
                case "getPlace":
                    int parentId = 0;
                    string parentIds = string.Empty;
                    if (context.Request.QueryString["parentId"] != null)
                    {
                        parentId =int.Parse(context.Request.QueryString["parentId"]);
                        result = GetPlace(parentId);
                    }
                    else if (context.Request.QueryString["parentIds"] != null)
                    {
                        parentIds = context.Request.QueryString["parentIds"];
                        result = GetPlace(parentIds);
                    }
                    
                    break;
                case "edit":
                    string jsonStr = context.Request.Form["jsonstr"];
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        jsonStr = jsonStr.Replace("+", "%2B");
                        jsonStr = HttpUtility.UrlDecode(jsonStr);
                    }
                    result = Edit(jsonStr);
                    break;
                case "delete":
                    result = Delete();
                    break;
                case "recover":
                    result = Recover();
                    break;
                default:
                    break;
            }
            context.Response.Write(result);
        }

        string GetTypeList()
        {
            var list = new CompanyTypeBLL().GetList(s => s.Id > 0);
            StringBuilder json = new StringBuilder();
            if (list.Any())
            {
                json.Append("{\"Id\":\"0\",\"TypeName\":\"全部\"},");
                list.ForEach(s =>
                {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"TypeName\":\"" + s.TypeName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
        }

        string GetList(int typeId, int currPage, int pageSize)
        {
            var list = (from company in CurrentContext.DbContext.Company
                        join type in CurrentContext.DbContext.CompanyType
                        on company.TypeId equals type.Id
                        join province1 in CurrentContext.DbContext.Place
                        on company.ProvinceId equals province1.ID into provinceTemp
                        from province in provinceTemp.DefaultIfEmpty()
                        join city1 in CurrentContext.DbContext.Place
                        on company.CityId equals city1.ID into cityTemp
                        from city in cityTemp.DefaultIfEmpty()
                        select new
                        {
                            company,
                            type.TypeName,
                            Province = province.PlaceName,
                            City = city.PlaceName
                        }).ToList();
            if (typeId > 0)
            {
                list = list.Where(s => s.company.TypeId == typeId).ToList();
            }
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                int totalCount = list.Count;
                list = list.OrderBy(s => s.company.TypeId).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                int index = 1;
                CompanyInRegionBLL companyRegionBll = new CompanyInRegionBLL();
                list.ForEach(s =>
                {
                    string inchargeProvince = string.Empty;
                    string inchargeCity = string.Empty;
                    string provinceIds = string.Empty;
                    string cityIds = string.Empty;
                    if ((s.company.TypeId ?? 1) > 1)
                    {
                        
                        var model = companyRegionBll.GetList(r=>r.CompanyId==s.company.Id).FirstOrDefault();
                        if (model != null)
                        {
                            provinceIds = model.ProvinceId;
                            cityIds = model.CityId;
                            if (!string.IsNullOrWhiteSpace(provinceIds))
                                inchargeProvince = GetInchargePlace(provinceIds);
                            if (!string.IsNullOrWhiteSpace(cityIds))
                                inchargeCity = GetInchargePlace(cityIds);
                        }
                    }
                    string joinDate = s.company.JoinDate != null ? DateTime.Parse(s.company.JoinDate.ToString()).ToShortDateString() : "";
                    string state = s.company.IsDelete != null && s.company.IsDelete == true ? "已删除" : "正常";
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.company.Id + "\",\"TypeId\":\"" + s.company.TypeId + "\",\"TypeName\":\"" + s.TypeName + "\",\"CompanyName\":\"" + s.company.CompanyName + "\",\"ShortName\":\"" + s.company.ShortName + "\",\"ParentId\":\"" + s.company.ParentId + "\",\"ProvinceId\":\"" + s.company.ProvinceId + "\",\"CityId\":\"" + s.company.CityId + "\",\"Province\":\"" + s.Province + "\",\"City\":\"" + s.City + "\",\"Contact\":\"" + s.company.Contact + "\",\"Tel\":\"" + s.company.Tel + "\",\"Address\":\"" + s.company.Address + "\",\"State\":\"" + state + "\",\"ProvinceIds\":\"" + provinceIds + "\",\"CityIds\":\"" + cityIds + "\",\"JoinDate\":\"" + joinDate + "\",\"InchargeProvince\":\"" + inchargeProvince + "\",\"InchargeCity\":\"" + inchargeCity + "\",\"CompanyCode\":\""+s.company.CompanyCode+"\"},");
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

        string GetPlace(int parentId)
        {
            var list = new PlaceBLL().GetList(s => s.ParentID == parentId);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"PlaceId\":\"" + s.ID + "\",\"PlaceName\":\"" + s.PlaceName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "[]";
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
                            select new
                            {
                                place.ParentID,
                                ParentName = parent.PlaceName,
                                place.ID,
                                place.PlaceName
                            }).OrderBy(s => s.ParentID).ToList();
                if (list.Any())
                {
                    StringBuilder json = new StringBuilder();
                    list.ForEach(s =>
                    {
                        json.Append("{\"ParentId\":\"" + s.ParentID + "\",\"ParentName\":\"" + s.ParentName + "\",\"PlaceId\":\"" + s.ID + "\",\"PlaceName\":\"" + s.PlaceName + "\"},");

                    });
                    return "[" + json.ToString().TrimEnd(',') + "]";
                }
                return "";

            }
            return "";
        }

        List<Place> placeList = new List<Place>();
        string GetInchargePlace(string ids)
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

        string Edit(string jsonStr)
        {
            string r="ok";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                Company model = JsonConvert.DeserializeObject<Company>(jsonStr);
                CompanyInRegion companyRegionModel;
                CompanyInRegionBLL companyInRegionBll = new CompanyInRegionBLL();
                if (model != null)
                {
                    if (model.Id > 0)
                    {
                        if (!IsExist(model.TypeId ?? 0, model.Id, model.CompanyName))
                        {
                            Company newModel = companyBll.GetModel(model.Id);
                            if (newModel != null)
                            {

                                newModel.Address = model.Address;
                                newModel.CityId = model.CityId;
                                newModel.CompanyName = model.CompanyName;
                                newModel.Contact = model.Contact;
                                newModel.ParentId = newModel.ParentId;
                                newModel.ProvinceId = model.ProvinceId;
                                newModel.ShortName = model.ShortName;
                                newModel.Tel = model.Tel;
                                newModel.TypeId = model.TypeId;
                                newModel.JoinDate = model.JoinDate;
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
                        if (!IsExist(model.TypeId ?? 0, 0, model.CompanyName))
                        {
                            model.AddDate = DateTime.Now;
                            model.IsDelete = false;
                            model.ParentId = 1;
                            model.CompanyCode=CreateCompanyCode();
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
                        idList.ForEach(s =>
                        {
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


        bool IsExist(int typeId, int id, string name)
        {
            var list = companyBll.GetList(s => s.TypeId == typeId && s.CompanyName.ToLower() == name.ToLower() && (id>0?(s.Id!=id):1==1));
            return list.Any();
        }

        string CreateCompanyCode()
        {
            int index = ((from company in CurrentContext.DbContext.Company
                          select company.Id).Max()) + 1;
            return "A17" + index.ToString().PadLeft(4, '0');
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
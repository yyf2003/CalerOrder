using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using System.Text;
//using BLL;
using DAL;
//using Models;
using System.Web.SessionState;
using Common;

namespace WebApp.Subjects.InstallPrice.handler
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler,IRequiresSessionState
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            switch (type)
            { 
                case "getPOSScale":
                    result = GetPOSScale();
                    break;
                case "checkEmpty":
                    int guidanceId = 0;
                    if (context.Request.QueryString["guidanceId"] != null)
                    {
                        guidanceId = int.Parse(context.Request.QueryString["guidanceId"]);
                    }
                    result=CheckEmptyData(guidanceId);
                    break;
            }

            
            context.Response.Write(result);
        }

        string GetPOSScale()
        {
            string r = string.Empty;
            List<string> list = new POSScaleInfoBLL().GetList(s => 1 == 1).Select(s => s.POSScaleName).Distinct().ToList();

            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"POSScale\":\"" + s + "\"},");
                });
                r = "[" + json.ToString().TrimEnd(',') + "]";
            }
            return r;
        }

        string CheckEmptyData(int guidanceId)
        {
            context1.Session["emptyOrder"] = null;
            string result = "ok";
            if (guidanceId>0)
            {
                
                var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                            on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                            from subjectCategory in categoryTemp.DefaultIfEmpty()
                            where subject.GuidanceId == guidanceId
                            && subject.ApproveState==1
                            && (subject.IsDelete == null || subject.IsDelete==false)
                            && (order.IsDelete == null || order.IsDelete == false)
                            && shop.IsInstall !=null && shop.IsInstall == "Y"
                            && ((subjectCategory != null) ? ((subjectCategory.CategoryName != "童店" && subjectCategory.CategoryName!="三叶草") || subjectCategory.CategoryName == "" || subjectCategory.CategoryName == null) : 1 == 1)
                            && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                            select new
                            {
                                order,
                                shop,
                                subject
                            }).ToList();
                //if (new BasePage().CurrentUser.RoleId == 6)
                //{

                //    List<string> myRegionList = new BasePage().GetResponsibleRegion;
                //    StringHelper.ToUpperOrLowerList(ref myRegionList, LowerUpperEnum.ToLower);
                //    list = list.Where(s => s.order.IsFromRegion != null && s.order.IsFromRegion == true && myRegionList.Contains(s.order.Region.ToLower())).ToList();
                //}
                //else
                //{
                //    list = list.Where(s => (s.order.IsFromRegion == null || s.order.IsFromRegion == false)).ToList();
                //}
                if (list.Any())
                {
                   
                    List<Shop> shopList = new List<Shop>();
                    Shop shopModel;
                    List<int> shopIdList = new List<int>();
                    


                    var notEmptyList = list.Where(s => s.order.InstallPricePOSScale != null && s.order.InstallPricePOSScale!="" && s.order.InstallPriceMaterialSupport!=null && s.order.InstallPriceMaterialSupport!="").ToList();
                    var emptyList = list.Where(s => s.order.InstallPricePOSScale == null || s.order.InstallPricePOSScale == "" || s.order.InstallPriceMaterialSupport == null || s.order.InstallPriceMaterialSupport == "").ToList();
                    List<int> notEmptyShopIdList = notEmptyList.Select(s => s.shop.Id).Distinct().ToList();
                    emptyList = emptyList.Where(s => !notEmptyShopIdList.Contains(s.shop.Id)).ToList();
                    if (emptyList.Any())
                    {
                        emptyList.ForEach(s => {
                            if (!shopIdList.Contains(s.shop.Id))
                            {
                                shopModel = new Shop();
                                shopModel = s.shop;
                                shopModel.POSScale = s.order.InstallPricePOSScale;
                                shopModel.MaterialSupport = s.order.InstallPriceMaterialSupport;
                                shopList.Add(shopModel);
                                shopIdList.Add(s.shop.Id);
                            }
                            else
                            {
                                int index = shopList.IndexOf(shopList.Where(sh=>sh.Id==s.shop.Id).FirstOrDefault());
                                shopModel=shopList[index];
                                if (string.IsNullOrWhiteSpace(shopModel.POSScale) && !string.IsNullOrWhiteSpace(s.order.InstallPricePOSScale))
                                    shopModel.POSScale = s.order.InstallPricePOSScale;
                                if (string.IsNullOrWhiteSpace(shopModel.MaterialSupport) && !string.IsNullOrWhiteSpace(s.order.InstallPriceMaterialSupport))
                                    shopModel.MaterialSupport = s.order.InstallPriceMaterialSupport;
                                shopList[index] = shopModel;
                            }
                        });
                        shopList = shopList.Where(s=>s.POSScale==null || s.POSScale=="" || s.MaterialSupport==null || s.MaterialSupport=="").ToList();
                        if (shopList.Any())
                        {
                            context1.Session["emptyOrder"] = shopList;
                            result = "empty";
                        }
                    }
                   
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
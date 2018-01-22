using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;
using Newtonsoft.Json;
using System.Transactions;



namespace WebApp.Subjects.Handler
{
    /// <summary>
    /// Orders 的摘要说明
    /// </summary>
    public class Orders : IHttpHandler
    {
        int subjectId;
        string subjectIds = string.Empty;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(context.Request.QueryString["subjectId"]);
            }
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            if (context.Request.QueryString["subjectIds"] != null)
            {
                subjectIds = context.Request.QueryString["subjectIds"];
            }
            switch (type)
            { 
                case "exportMergeOrder":
                    ExportMergeOrder();
                    break;
                case "getRegion":
                    result = GetRegion();
                    break;
                case "getProvince":
                    string region = context.Request.QueryString["region"];
                    string sheet = "0";
                    if (context.Request.QueryString["sheet"] != null)
                    {
                        sheet = context.Request.QueryString["sheet"];
                    }
                    result = GetProvince(region, sheet);
                    break;
                case "getCity":
                    string province = context.Request.QueryString["province"];
                    string sheet1 = "0";
                    if (context.Request.QueryString["sheet"] != null)
                    {
                        sheet1 = context.Request.QueryString["sheet"];
                    }
                    result = GetCity(province, sheet1);
                    break;
            }
            context.Response.Write(result);
        }

        /// <summary>
        /// 导出合并后的原始订单
        /// </summary>
        void ExportMergeOrder()
        {
            #region
            //var MergeList = from merge in CurrentContext.DbContext.MergeOriginalOrder
            //                join pop1 in CurrentContext.DbContext.POP
            //                on new { merge.GraphicNo, merge.ShopId } equals new { pop1.GraphicNo, pop1.ShopId } into popTemp
            //                join shop in CurrentContext.DbContext.Shop
            //                on merge.ShopId equals shop.Id
            //                from pop in popTemp.DefaultIfEmpty()
            //                where merge.SubjectId == subjectId
            //                select new
            //                {
            //                    merge,

            //                    shop,
            //                    GraphicNo = pop != null ? pop.GraphicNo : "",
            //                    POPName = pop != null ? pop.POPName : "",
            //                    POPType = pop != null ? pop.POPType : "",
            //                    PositionDescription = pop != null ? pop.PositionDescription : "",
            //                    WindowWide = pop != null && pop.WindowWide != null ? pop.WindowWide.ToString() : "",
            //                    WindowHigh = pop != null && pop.WindowHigh != null ? pop.WindowHigh.ToString() : "",
            //                    WindowDeep = pop != null && pop.WindowDeep != null ? pop.WindowDeep.ToString() : "",
            //                    WindowSize = pop != null ? pop.WindowSize : "",
            //                    GraphicWidth = pop != null && pop.GraphicWidth != null ? pop.GraphicWidth.ToString() : "",
            //                    GraphicLength = pop != null && pop.GraphicLength != null ? pop.GraphicLength.ToString() : "",
            //                    Area = pop != null && pop.Area != null ? pop.Area.ToString() : "",
            //                    GraphicMaterial = pop != null ? pop.GraphicMaterial : "",
            //                    Style = pop != null ? pop.Style : "",
            //                    CornerType = pop != null ? pop.CornerType : "",
            //                    Category = pop != null ? pop.Category : "",
            //                    StandardDimension = pop != null ? pop.StandardDimension : "",
            //                    Modula = pop != null ? pop.Modula : "",
            //                    Frame = pop != null ? pop.Frame : "",
            //                    DoubleFace = pop != null ? pop.DoubleFace : "",
            //                    Glass = pop != null ? pop.Glass : "",
            //                    Backdrop = pop != null ? pop.Backdrop : "",
            //                    ModulaQuantityWidth = pop != null && pop.ModulaQuantityWidth != null ? pop.ModulaQuantityWidth.ToString() : "",
            //                    ModulaQuantityHeight = pop != null && pop.ModulaQuantityHeight != null ? pop.ModulaQuantityHeight.ToString() : "",
            //                    PlatformLength = pop != null && pop.PlatformLength != null ? pop.PlatformLength.ToString() : "",

            //                    PlatformWidth = pop != null && pop.PlatformWidth != null ? pop.PlatformWidth.ToString() : "",
            //                    PlatformHeight = pop != null && pop.PlatformHeight != null ? pop.PlatformHeight.ToString() : "",
            //                    FixtureType = pop != null ? pop.FixtureType : "",


            //                };
            //if (MergeList.Any())
            //{

            //}

            #endregion
            try
            {
                DataSet ds = new MergeOriginalOrderBLL().GetOrderList(subjectId);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    OperateFile.ExportExcel(ds.Tables[0], "合并订单");

                }
                
            }
            catch (Exception ex)
            {
                
                //new BasePage().Alert("导出失败：" + ex.Message);
            }
        }


        string GetRegion()
        {
            List<int> subjdectIdList = StringHelper.ToIntList(subjectIds,',');
            var shopList = from merge in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on merge.ShopId equals shop.Id
                            where subjdectIdList.Contains(merge.SubjectId??0)
                            select shop.RegionName;
            List<string> region = new List<string>();
            if (shopList.Any())
            {
                shopList.ToList().ForEach(s => {
                    if (!string.IsNullOrWhiteSpace(s) && !region.Contains(s))
                    {
                        region.Add(s);
                    }
                });
            }
            if (region.Any())
            {
                StringBuilder json = new StringBuilder();
                region.ForEach(r => {
                    json.Append("{\"Region\":\""+r+"\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetProvince(string region,string sheet=null)
        {
            List<int> subjdectIdList = StringHelper.ToIntList(subjectIds, ',');
            List<string> regionList = StringHelper.ToStringList(region,',');
            var shopList = (from merge in CurrentContext.DbContext.FinalOrderDetailTemp
                           join shop in CurrentContext.DbContext.Shop
                           on merge.ShopId equals shop.Id
                            where regionList.Contains(shop.RegionName) && subjdectIdList.Contains(merge.SubjectId ?? 0) && ((sheet!=null && sheet!="" && sheet!="0")?(merge.Sheet==sheet):1==1)
                           select shop.ProvinceName).OrderBy(s=>s);
            List<string> province = new List<string>();
            if (shopList.Any())
            {
                shopList.ToList().ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s) && !province.Contains(s))
                    {
                        province.Add(s);
                    }
                });
            }
            if (province.Any())
            {
                StringBuilder json = new StringBuilder();
                province.ForEach(p =>
                {
                    json.Append("{\"Province\":\"" + p + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

        string GetCity(string province, string sheet = null)
        {
            List<int> subjdectIdList = StringHelper.ToIntList(subjectIds, ',');
            List<string> provinceList = StringHelper.ToStringList(province,',');
            var shopList = (from merge in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on merge.ShopId equals shop.Id
                            where provinceList.Contains(shop.ProvinceName) && subjdectIdList.Contains(merge.SubjectId ?? 0) && ((sheet != null && sheet != "" && sheet != "0") ? (merge.Sheet == sheet) : 1 == 1)
                            select new {shop.ProvinceName, shop.CityName }).OrderBy(s => s.ProvinceName);
            List<string> city = new List<string>();
            if (shopList.Any())
            {
                StringBuilder json = new StringBuilder();
                shopList.ToList().ForEach(c =>
                {
                    if (!string.IsNullOrWhiteSpace(c.CityName) && !city.Contains(c.CityName))
                    {
                        city.Add(c.CityName);
                        json.Append("{\"Province\":\""+c.ProvinceName+"\",\"City\":\"" + c.CityName + "\"},");
                    }
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
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
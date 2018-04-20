using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Common;
using DAL;

namespace WebApp.Statistics
{
    public partial class NewShopInstallPriceDetail : BasePage
    {
        int subjectId;
        string subjectIds = string.Empty;
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectIds"] != null)
            {
                subjectIds = Request.QueryString["subjectIds"];
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
                labTitle.Text = "查看项目明细";
            }
            if (Request.QueryString["region"] != null)
            {
                region = Request.QueryString["region"];
            }
            if (Request.QueryString["province"] != null)
            {
                province = Request.QueryString["province"];
            }
            if (Request.QueryString["city"] != null)
            {
                city = Request.QueryString["city"];
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }


        void BindData()
        {

            List<int> subjectIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(subjectIds))
            {
                subjectIdList = StringHelper.ToIntList(subjectIds, ',');
            }
            else
                subjectIdList.Add(subjectId);
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
            }
            else
            {
                GetResponsibleRegion.ForEach(s =>
                {
                    regionList.Add(s.ToLower());
                });

            }

            var list = (from order in CurrentContext.DbContext.PriceOrderDetail
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where subjectIdList.Contains(order.SubjectId ?? 0)
                        && (subject.SubjectType == (int)SubjectTypeEnum.新开店安装费 || subject.SubjectType == (int)SubjectTypeEnum.运费 || order.OrderType==(int)OrderTypeEnum.运费)
                        //&& (regionList.Any() ? ((subject.PriceBlongRegion != null && subject.PriceBlongRegion != "") ? regionList.Contains(subject.PriceBlongRegion.ToLower()) : regionList.Contains(order.Region.ToLower())) : 1 == 1)
                        && (regionList.Any() ? regionList.Contains(order.Region.ToLower()) : 1 == 1)
                        
                        select new
                        {
                            order,
                            subject,
                            subject.SubjectType
                        }).ToList();

            if (!string.IsNullOrWhiteSpace(province))
            {
                provinceList = StringHelper.ToStringList(province, ',');
                list = list.Where(s => provinceList.Contains(s.order.Province)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                cityList = StringHelper.ToStringList(city, ',');
                list = list.Where(s => cityList.Contains(s.order.City)).ToList();
            }
            
            labShopCount.Text = (list.Select(s => s.order.ShopName).Distinct().Count()).ToString();
            labTotlePrice.Text = (list.Sum(s => s.order.Amount ?? 0)).ToString();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = list.OrderBy(s => s.order.ShopId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvList.DataBind();

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objSubjectType = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    string orderType = objSubjectType != null ? objSubjectType.ToString() : "0";
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<SubjectTypeEnum>(orderType);
                }
            }
        }
    }
}
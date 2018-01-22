using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;

namespace WebApp.OutsourcingOrder.Statistics
{
    public partial class CheckPOPPriceDetail : BasePage
    {
        int customerId = 0;
        string outsourceId = string.Empty;
        string guidanceId = string.Empty;
        string subjectId = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        string assignType = string.Empty;
        string guidanceMonth = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["outsourceId"] != null)
            {
                outsourceId = Request.QueryString["outsourceId"];
            }
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = Request.QueryString["subjectId"];
            }
            if (Request.QueryString["province"] != null)
            {
                province = Request.QueryString["province"];
            }
            if (Request.QueryString["city"] != null)
            {
                city = Request.QueryString["city"];
            }
            if (Request.QueryString["assignType"] != null)
            {
                assignType = Request.QueryString["assignType"];
            }
            if (Request.QueryString["guidanceMonth"] != null)
            {
                guidanceMonth = Request.QueryString["guidanceMonth"];
            }
            if (Request.QueryString["customerId"] != null)
            {
                customerId =int.Parse(Request.QueryString["customerId"]);
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData() {
            List<int> guidanceIdList = new List<int>();
            List<int> subjectIdList = new List<int>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            List<int> assignTypeList = new List<int>();
            List<int> outsourceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(outsourceId))
            {
                outsourceIdList = StringHelper.ToIntList(outsourceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
            }
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                subjectIdList = StringHelper.ToIntList(subjectId, ',');
            }
            if (!string.IsNullOrWhiteSpace(province))
            {
                provinceList = StringHelper.ToStringList(province, ',');
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                cityList = StringHelper.ToStringList(city, ',');
            }
            if (!string.IsNullOrWhiteSpace(assignType))
            {
                assignTypeList = StringHelper.ToIntList(assignType, ',');
            }

            
            var assignShopList = new OutsourceOrderDetailBLL().GetList(s => outsourceIdList.Contains(s.OutsourceId ?? 0) && guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));

            //var assignShopList = new OutsourceAssignShopBLL().GetList(s => s.OutsourceId == outsourceId && guidanceIdList.Contains(s.GuidanceId ?? 0));
            if (subjectIdList.Any())
            {
                assignShopList = assignShopList.Where(s => subjectIdList.Contains(s.SubjectId??0)).ToList();
            }
            if (provinceList.Any())
            {
                assignShopList = assignShopList.Where(s => provinceList.Contains(s.Province)).ToList();
            }
            if (cityList.Any())
            {
                assignShopList = assignShopList.Where(s => cityList.Contains(s.City)).ToList();
            }
            if (assignTypeList.Any())
            {
                assignShopList = assignShopList.Where(s => assignTypeList.Contains(s.AssignType ?? 0)).ToList();
            }
            if (!IsPostBack)
            {
                decimal materialPrice = 0;
                List<int> shopIdList = assignShopList.Select(s => s.ShopId??0).Distinct().ToList();
                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                {
                    DateTime date0 = DateTime.Parse(guidanceMonth);
                    int year = date0.Year;
                    int month = date0.Month;
                    var pvcOrderList = new OutsourceReceivePriceOrderBLL().GetList(s => outsourceIdList.Contains(s.OutsourceId ?? 0) && shopIdList.Contains(s.ShopId ?? 0) && s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                    if (pvcOrderList.Any())
                    {
                        materialPrice = pvcOrderList.Sum(s => s.TotalPrice ?? 0);

                    }
                }

                decimal totalPay = assignShopList.Sum(s => s.TotalPrice ?? 0);
                decimal totalReceive = assignShopList.Sum(s => s.ReceiveTotalPrice ?? 0);
                labShopCount.Text = assignShopList.Select(s => s.ShopId).Distinct().Count().ToString();
                if (materialPrice > 0)
                {
                    
                    labTotalPrice.Text = "POP费用：" + totalPay.ToString();
                    labMaterialPrice.Text = "— 应收外协：<span id='spanCheckMaterialPrice' data-guidancemonth='" + guidanceMonth + "' data-customerid='" + customerId + "' data-outsourceid='" + outsourceId + "' style='color:blue;cursor:pointer;text-decoration:underline;'>" + materialPrice + "</span>";
                    labPOPFinallPrice.Text = "="+(totalPay - materialPrice).ToString();
                    labMaterialPrice.Visible =labPOPFinallPrice.Visible= true;
                }
                else
                   labTotalPrice.Text = totalPay.ToString();

                labRTotalPrice.Text = totalReceive.ToString();
            }
            AspNetPager1.RecordCount = assignShopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPOP.DataSource = assignShopList.OrderBy(s => s.ShopId).ThenBy(s=>s.Sheet).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPOP.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
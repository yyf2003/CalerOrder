﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using Models;
using DAL;
using System.Text;
using Common;

namespace WebApp.Statistics
{
    public partial class PriceSubjectStatistic : System.Web.UI.Page
    {
        public int subjectId;
        
        string region = string.Empty;
        string province = string.Empty;
        string city = string.Empty;
        string customerServiceId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
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
            if (Request.QueryString["customerServiceId"] != null)
            {
                customerServiceId = Request.QueryString["customerServiceId"];
            }
            if (!IsPostBack)
            {
                //Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                //if (subjectModel != null)
                //    labSubjectName.Text = subjectModel.SubjectName;
                BindData();
                //BindRegion();
            }
        }

        void BindData()
        {
            var list = (from order in CurrentContext.DbContext.PriceOrderDetail
                       //join shop in CurrentContext.DbContext.Shop
                       //on order.ShopId equals shop.Id
                       join subject in CurrentContext.DbContext.Subject
                       on order.SubjectId equals subject.Id
                       where order.SubjectId == subjectId
                       select new
                       {
                           order,
                           //shop,
                           subject
                       }).ToList();
            //List<int> customerServiceList = new List<int>();
            //if (!string.IsNullOrWhiteSpace(customerServiceId))
            //{
            //    customerServiceList = StringHelper.ToIntList(customerServiceId, ',');
            //    if (customerServiceList.Any())
            //        list = list.Where(s => customerServiceList.Contains(s.shop.CSUserId ?? 0)).ToList();
            //}
            if (labSubjectName.Text == "")
            {
                if (list.Any())
                {
                    labSubjectName.Text = list[0].subject.SubjectName;
                    labShopCount.Text = list.Select(s=>s.order.ShopName.ToLower()).Distinct().Count().ToString();
                    labTotlePrice.Text = list.Sum(s => s.order.Amount ?? 0).ToString();

                    int subjectType = list[0].subject.SubjectType ?? 0;
                    labTitle.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString()) + "项目明细";
                }
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = list.OrderBy(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvList.DataBind(); 

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {

        }
    }
}
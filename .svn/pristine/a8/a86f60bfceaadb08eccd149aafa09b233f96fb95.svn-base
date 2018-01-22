using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;

namespace WebApp.Subjects.InstallManagement
{
    public partial class InstallList : BasePage
    {
        public int CustomerId { get; set; }
        public int GuidanceId { get; set; }
        public List<string> RegionList = new List<string>();
        public List<string> ProvinceList = new List<string>();
        public List<string> CityList = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                LoadCondition();
                if (CustomerId > 0)
                {
                    ddlCustomer.SelectedValue = CustomerId.ToString();
                    BindGuidance();
                    BindShop();
                }
            }
        }
        void LoadCondition()
        {
            if (Session["Condition"] != null)
            {
                Dictionary<string, string> dic = (Dictionary<string, string>)Session["Condition"];
                foreach (KeyValuePair<string, string> item in dic)
                {
                    if (item.Key == "CustomerId")
                    {
                        CustomerId = int.Parse(item.Value);
                    }
                    if (item.Key == "GuidanceId")
                    {
                        GuidanceId = int.Parse(item.Value);
                    }
                    if (item.Key == "Region")
                    {
                        RegionList = StringHelper.ToStringList(item.Value, ',');
                    }
                    if (item.Key == "Province")
                    {
                        ProvinceList = StringHelper.ToStringList(item.Value, ',');
                    }
                    if (item.Key == "City")
                    {
                        CityList = StringHelper.ToStringList(item.Value, ',');
                    }
                }
                Session["Condition"] = null;
            }
        }

        void BindGuidance()
        {
            ddlGuidance.Items.Clear();
            ddlGuidance.Items.Add(new ListItem("--请选择--", "-1"));
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        join type in CurrentContext.DbContext.ADOrderActivity
                        on guidance.ActivityTypeId equals type.ActivityId
                        where guidance.CustomerId == customerId && guidance.ActivityTypeId == 1
                        select new
                        {
                            guidance,
                            type
                        }).ToList();
            string begin = txtBeginDate.Text.Trim();
            string end = txtEndDate.Text.Trim();
            if (!string.IsNullOrWhiteSpace(begin) && !string.IsNullOrWhiteSpace(end))
            {
                DateTime beginDate = DateTime.Parse(begin);
                DateTime endDate = DateTime.Parse(end).AddDays(1);
                list = list.Where(s => s.guidance.BeginDate >= beginDate && s.guidance.AddDate < endDate).ToList();
            }
            else
            {

                DateTime date = DateTime.Now;
                DateTime newDate = new DateTime(date.Year, date.Month, 1);
                DateTime beginDate = newDate.AddMonths(-2);
                DateTime endDate = newDate.AddMonths(1);
                list = list.Where(s => s.guidance.BeginDate >= beginDate && s.guidance.BeginDate < endDate).ToList();
            }
            bool isChecked = false;
            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {


                    ListItem li = new ListItem();
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + s.type.ActivityName;
                    if (GuidanceId == s.guidance.ItemId)
                    {
                        li.Selected = true;
                        isChecked = true;
                    }
                    ddlGuidance.Items.Add(li);
                });
            }

            if (isChecked)
                BindRegion();
        }

        void BindShop()
        {


            int guidanceId = int.Parse(ddlGuidance.SelectedValue);

            var shopList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where subject.GuidanceId == guidanceId && shop.IsInstall=="Y"
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                            select shop).Distinct().ToList();
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                    regionList.Add(li.Value.ToLower());
            }
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                    provinceList.Add(li.Value);
            }
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                    cityList.Add(li.Value);
            }

            if (regionList.Any())
            {
                shopList = shopList.Where(s => regionList.Contains(s.RegionName.ToLower())).ToList();

            }
            if (provinceList.Any())
            {
                shopList = shopList.Where(s => provinceList.Contains(s.ProvinceName)).ToList();

            }
            if (cityList.Any())
            {
                shopList = shopList.Where(s => cityList.Contains(s.CityName)).ToList();

            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                shopList = shopList.Where(s => s.ShopNo.ToLower() == txtShopNo.Text.Trim().ToLower()).ToList();

            }

            AspNetPager1.RecordCount = shopList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = shopList.OrderBy(s => s.RegionName).ThenBy(s => s.ProvinceName).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
        }

        void BindRegion()
        {
            cblRegion.Items.Clear();
            cblProvince.Items.Clear();
            cblCity.Items.Clear();
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            hfGuidanceId.Value = guidanceId.ToString();
            var shopList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where subject.GuidanceId == guidanceId && shop.IsInstall == "Y"
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                            select shop).Distinct().Select(s => s.RegionName).Distinct().ToList();
            if (shopList.Any())
            {
                bool selected = false;
                shopList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s + "&nbsp;";
                    li.Value = s;
                    RegionList.ForEach(r =>
                    {
                        if (r == s.ToLower())
                        {
                            li.Selected = true;
                            selected = true;
                        }
                    });
                    cblRegion.Items.Add(li);
                });
                if (selected)
                {
                    BindProvince();
                }
            }
        }

        void BindProvince()
        {
            cblProvince.Items.Clear();
            cblCity.Items.Clear();
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value.ToLower());
                }
            }

            var shopList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where subject.GuidanceId == guidanceId && shop.IsInstall == "Y" && regionList.Contains(shop.RegionName.ToLower())
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                            select shop).Distinct().Select(s => s.ProvinceName).Distinct().ToList();
            if (shopList.Any())
            {
                bool selected = false;
                shopList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s + "&nbsp;";
                    li.Value = s;
                    ProvinceList.ForEach(p =>
                    {
                        if (p == s)
                        {
                            li.Selected = true;
                            selected = true;
                        }
                    });
                    cblProvince.Items.Add(li);
                });
                if (selected)
                {
                    BindCity();
                }
            }
        }

        void BindCity()
        {
            cblCity.Items.Clear();
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                {
                    provinceList.Add(li.Value);
                }
            }

            var shopList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where subject.GuidanceId == guidanceId && shop.IsInstall == "Y" && provinceList.Contains(shop.ProvinceName)
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                            select shop).Distinct().Select(s => s.CityName).Distinct().ToList();
            if (shopList.Any())
            {
                shopList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s + "&nbsp;";
                    li.Value = s;
                    CityList.ForEach(c =>
                    {
                        if (c == s)
                        {
                            li.Selected = true;

                        }
                    });
                    cblCity.Items.Add(li);
                });
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            BindShop();
        }

        protected void btnSearchSubject_Click(object sender, EventArgs e)
        {

        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
        }


        InstallBLL installBll = new InstallBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                Shop shopModel = (Shop)e.Row.DataItem;
                if (shopModel != null)
                {
                    int guidanceId = int.Parse(hfGuidanceId.Value);
                    var installModel = installBll.GetList(s => s.GuidanceId == guidanceId && s.ShopId == shopModel.Id).FirstOrDefault();
                    Label installStateLab = (Label)e.Row.FindControl("labInstallState");
                    
                    if (installModel != null)
                    {
                        installStateLab.Text = "已安装";
                        installStateLab.ForeColor = System.Drawing.Color.Green;
                        
                    }
                    else
                    {
                        installStateLab.Text = "未安装";
                        installStateLab.ForeColor = System.Drawing.Color.Red;
                        
                    }
                }
            }
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int shopid = int.Parse(e.CommandArgument.ToString());
            int guidanceId = 0;
            Dictionary<string, string> conditionDic = new Dictionary<string, string>();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            if (!string.IsNullOrWhiteSpace(hfGuidanceId.Value))
                guidanceId = int.Parse(hfGuidanceId.Value);
            conditionDic.Add("CustomerId", customerId.ToString());
            conditionDic.Add("GuidanceId", guidanceId.ToString());
            List<string> regionList = new List<string>();
            List<string> provinceList = new List<string>();
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                    regionList.Add(li.Value.ToLower());
            }
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                    provinceList.Add(li.Value);
            }
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                    cityList.Add(li.Value);
            }
            if (regionList.Any())
            {
                conditionDic.Add("Region", StringHelper.ListToString(regionList));
            }
            if (provinceList.Any())
            {
                conditionDic.Add("Province", StringHelper.ListToString(provinceList));
            }
            if (cityList.Any())
            {
                conditionDic.Add("City", StringHelper.ListToString(cityList));
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                conditionDic.Add("ShopNo", txtShopNo.Text.Trim());
            }
            Session["Condition"] = conditionDic;
            if (e.CommandName == "Check")
            {
                Response.Redirect("CheckDetail.aspx?guidanceId=" + guidanceId + "&shopId=" + shopid, false);

            }
            if (e.CommandName == "Install")
            {
                Response.Redirect("InstallConfirm.aspx?guidanceId=" + guidanceId + "&shopId=" + shopid, false);

            }
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCity();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {

        }
    }
}
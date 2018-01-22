using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;

namespace WebApp.Subjects.Received
{
    public partial class SendList : BasePage
    {
        public int CustomerId { get; set; }
        public int GuidanceId { get; set; }
        //public List<int> GuidanceList = new List<int>();
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
                        //GuidanceList = StringHelper.ToIntList(item.Value, ',');
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
                        where guidance.CustomerId == customerId && guidance.ActivityTypeId != 1
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
            bool isChecked=false;
            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {
                    

                    ListItem li = new ListItem();
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + s.type.ActivityName;
                    if (GuidanceId==s.guidance.ItemId)
                    {
                        li.Selected = true;
                        isChecked = true;
                    }
                    ddlGuidance.Items.Add(li);
                });
            }

            if(isChecked)
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
                            where subject.GuidanceId == guidanceId && (shop.IsInstall != "Y")
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && subject.ApproveState == 1
                             && (order.IsDelete == null || order.IsDelete == false)
                            select shop).OrderBy(s=>s.ProvinceName).Distinct().ToList();
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
            //hfGuidanceId.Value = guidanceId.ToString();
            var shopList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where subject.GuidanceId == guidanceId && (shop.IsInstall != "Y")
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
            cbAllProvince.Checked = false;
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
                            where subject.GuidanceId == guidanceId && regionList.Contains(shop.RegionName.ToLower())
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            && subject.ApproveState == 1
                            && (order.IsDelete == null || order.IsDelete == false)
                            select shop).ToList();
            if (shopList.Any())
            {
                shopList = shopList.OrderBy(s => s.RegionName).ToList();
                var pList = shopList.Select(s => s.ProvinceName).Distinct().ToList();
                bool selected = false;
                pList.ForEach(s =>
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
                cbAllProvinceDiv.Style.Add("display", "block");
            }
            else
            {

                cbAllProvinceDiv.Style.Add("display", "none");
            }
        }

        void BindCity()
        {
            cbAllCity.Checked = false;
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
                            where subject.GuidanceId == guidanceId && provinceList.Contains(shop.ProvinceName)
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            && subject.ApproveState == 1
                            && (order.IsDelete == null || order.IsDelete == false)
                            select shop).ToList();

            if (shopList.Any())
            {
                shopList = shopList.OrderBy(s => s.ProvinceName).ToList();
                var cityList = shopList.Select(s => s.CityName).Distinct().ToList();
                cityList.ForEach(s =>
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
                cbAllCityDiv.Style.Add("display", "block");
            }
            else
            {

                cbAllCityDiv.Style.Add("display", "none");
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            BindShop();
        }

        protected void btnSearchSubject_Click(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindShop();
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCity();
        }

        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProvince();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int shopid = int.Parse(e.CommandArgument.ToString());
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            
            //string guidanceIds=StringHelper.ListToString(guidanceList);
            Dictionary<string, string> conditionDic = new Dictionary<string, string>();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            //if (!string.IsNullOrWhiteSpace(hfGuidanceId.Value))
            //    guidanceId = int.Parse(hfGuidanceId.Value);
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
            if (e.CommandName == "Send")
            {
                Response.Redirect("SendConfirm.aspx?guidanceId=" + guidanceId + "&shopId=" + shopid, false);

            }
            if (e.CommandName == "Receive")
            {
                Response.Redirect("ReceiveConfirm.aspx?guidanceId=" + guidanceId + "&shopId=" + shopid, false);
            }
        }

        SendBLL sendBll = new SendBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                Shop shopModel = (Shop)e.Row.DataItem;
                if (shopModel != null)
                {
                    int guidanceId = int.Parse(ddlGuidance.SelectedValue);
                    var sendModel = sendBll.GetList(s => s.GuidanceId == guidanceId && s.ShopId == shopModel.Id).FirstOrDefault();
                    Label sendStateLab = (Label)e.Row.FindControl("labSendState");
                    Label receiveStateLab = (Label)e.Row.FindControl("labReceiveState");
                    LinkButton lbReceiveConfirm = (LinkButton)e.Row.FindControl("lbReceiveConfirm");
                    if (sendModel != null)
                    {
                        sendStateLab.Text = "已发货";
                        sendStateLab.ForeColor = System.Drawing.Color.Green;
                        if (sendModel.IsReceived == 1)
                        {
                            receiveStateLab.Text = "已收货";
                            receiveStateLab.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            receiveStateLab.Text = "未收货";
                            receiveStateLab.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else
                    {
                        sendStateLab.Text = "未发货";
                        sendStateLab.ForeColor = System.Drawing.Color.Red;
                        lbReceiveConfirm.Enabled = false;
                        lbReceiveConfirm.Style.Add("color", "#ccc");
                    }
                }
            }
        }

        protected void cblGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
        }

        /// <summary>
        /// 获取已选择的活动
        /// </summary>
        /// <returns></returns>
        //List<int> GetGuidanceSelected()
        //{
        //    List<int> list = new List<int>();
        //    foreach (ListItem li in cblGuidanceList.Items)
        //    {
        //        if (li.Selected && !list.Contains(int.Parse(li.Value)))
        //        {
        //            list.Add(int.Parse(li.Value));
        //        }
        //    }
        //    return list;
        //}

        //int GetGuidanceSelected()
        //{
        //    int id = 0;
        //    foreach (ListItem li in cblGuidanceList.Items)
        //    {
        //        if (li.Selected)
        //        {
        //            id=int.Parse(li.Value);
        //        }
        //    }
        //    return id;
        //}
    }
}
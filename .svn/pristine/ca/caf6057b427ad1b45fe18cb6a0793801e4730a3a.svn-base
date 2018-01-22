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
    public partial class CheckDetail : BasePage
    {
        public int guidanceId;
        public int shopId;
        public string FileType = string.Empty;
        public string FileCode = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["shopId"] != null)
            {
                shopId = int.Parse(Request.QueryString["shopId"]);
            }
            FileType = FileTypeEnum.Image.ToString();
            FileCode = FileCodeEnum.AfterInstallImg.ToString();
            if (!IsPostBack)
            {
                BindShop();
                BindOrders();
                BindInstallData();
            }
        }

        void BindShop()
        {
            Shop model = new ShopBLL().GetModel(shopId);
            if (model != null)
            {
                labShopNo.Text = model.ShopNo;
                labShopName.Text = model.ShopName;
                labRegion.Text = model.RegionName;
                labProvince.Text = model.ProvinceName;
                labCity.Text = model.CityName;
                labCityTier.Text = model.CityTier;
                labIsInstall.Text = model.IsInstall;
                labAddress.Text = model.POPAddress;
                if (model.InstallPrice != null)
                    labDefaultPrice.Text = model.InstallPrice.ToString();
            }
        }
        void BindOrders()
        {
            List<FinalOrderDetailTemp> orderlist = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                                    join subject in CurrentContext.DbContext.Subject
                                                    on order.SubjectId equals subject.Id
                                                    where subject.GuidanceId == guidanceId && order.ShopId == shopId
                                                    select order).ToList();
            var materialList = (from material in CurrentContext.DbContext.OrderMaterial
                                join subject in CurrentContext.DbContext.Subject
                                on material.SubjectId equals subject.Id
                                where subject.GuidanceId == guidanceId && material.ShopId == shopId
                                select material).ToList();
            materialList.ForEach(s =>
            {
                FinalOrderDetailTemp model = new FinalOrderDetailTemp();
                model.OrderType = 3;
                model.PositionDescription = s.MaterialName;
                model.Quantity = s.MaterialCount;
                model.Remark = s.Remark;
                model.Sheet = s.Sheet;
                model.SubjectId = s.SubjectId;
                orderlist.Add(model);
            });
            rep_OrderList.DataSource = orderlist;
            rep_OrderList.DataBind();

        }

        void BindInstallData()
        {
            var model = new InstallBLL().GetList(s=>s.GuidanceId==guidanceId && s.ShopId==shopId).FirstOrDefault();
            if (model != null)
            {
                if (model.InstallDate != null)
                    labInstallDate.Text = DateTime.Parse(model.InstallDate.ToString()).ToShortDateString();
                if(model.FinishDate!=null)
                    labFinishDate.Text = DateTime.Parse(model.FinishDate.ToString()).ToShortDateString();
                labInstallUserName.Text = model.InstallUserName;
                if (model.OtherPrice != null)
                    labOtherPrice.Text = model.OtherPrice.ToString();
                labOtherPriceRemark.Text = model.OtherPriceRemark;
                labInstallRemark.Text = model.InstallRemark;
            }
        }



        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("InstallList.aspx", false);
        }

        protected void rep_OrderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                FinalOrderDetailTemp model = (FinalOrderDetailTemp)e.Item.DataItem;
                if (model != null)
                {
                    int orderType = model.OrderType ?? 1;
                    Label lab = (Label)e.Item.FindControl("labOrderType");
                    switch (orderType)
                    {
                        case 1:
                            lab.Text = "POP";
                            break;
                        case 2:
                            lab.Text = "道具";
                            break;
                        case 3:
                            lab.Text = "物料";
                            break;
                    }
                }
            }
        }
    }
}
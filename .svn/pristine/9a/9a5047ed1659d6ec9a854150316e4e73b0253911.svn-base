using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;


namespace WebApp.Subjects.InstallManagement
{
    public partial class InstallConfirm : BasePage
    {
        public int guidanceId;
        public int shopId;
        public string FileType = string.Empty;
        //public string BeforeFileCode = string.Empty;
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
            //BeforeFileCode = FileCodeEnum.BeforeInstallImg.ToString();
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
            var model = (from install in CurrentContext.DbContext.Install
                       join shop in CurrentContext.DbContext.Shop
                       on install.ShopId equals shop.Id
                       where install.GuidanceId == guidanceId && install.ShopId == shopId
                       select new { 
                         install,
                         shop.InstallPrice
                       }).FirstOrDefault();
            if (model!=null)
            {
                if (model.install.InstallDate != null)
                {
                    txtInstallDate.Text = DateTime.Parse(model.install.InstallDate.ToString()).ToShortDateString();

                }
                if (model.install.FinishDate != null)
                {
                    txtFinishDate.Text = DateTime.Parse(model.install.FinishDate.ToString()).ToShortDateString();

                }
                txtInstallUserName.Text = model.install.InstallUserName;
                if (model.InstallPrice != null)
                    labDefaultInstallPrice.Text = model.InstallPrice.ToString();
                if (model.install.OtherPrice != null)
                    txtOtherPrice.Text = model.install.OtherPrice.ToString();
                txtOtherPriceRemark.Text = model.install.OtherPriceRemark;
                txtRemark.Text = model.install.InstallRemark;
                hfInstallId.Value = model.install.Id.ToString();
            }
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string installDate = txtInstallDate.Text.Trim();
            string finishDate = txtFinishDate.Text.Trim();
            string remark = txtRemark.Text.Trim();
            InstallBLL installBll = new InstallBLL();
            Install model = new Install();
            int installId = 0;
            if (!string.IsNullOrWhiteSpace(hfInstallId.Value))
            {
                installId = int.Parse(hfInstallId.Value);
                model = installBll.GetModel(installId);
                model.FinishDate = DateTime.Parse(finishDate);
                model.InstallDate = DateTime.Parse(installDate);
                model.InstallUserName = txtInstallUserName.Text.Trim();
                if (!string.IsNullOrWhiteSpace(txtOtherPrice.Text.Trim()))
                {
                    model.OtherPrice = decimal.Parse(txtOtherPrice.Text.Trim());
                    model.OtherPriceRemark = txtOtherPriceRemark.Text.Trim();
                }
                model.InstallRemark = remark;
                installBll.Update(model);
            }
            else
            {
                model.AddDate = DateTime.Now;
                model.AddUserId = CurrentUser.UserId;
                model.FinishDate = DateTime.Parse(finishDate);
                model.InstallDate = DateTime.Parse(installDate);
                model.GuidanceId = guidanceId;
                model.ShopId = shopId;
                if (!string.IsNullOrWhiteSpace(txtOtherPrice.Text.Trim()))
                {
                    model.OtherPrice = decimal.Parse(txtOtherPrice.Text.Trim());
                    model.OtherPriceRemark = txtOtherPriceRemark.Text.Trim();
                }
                model.InstallUserName = txtInstallUserName.Text.Trim();
                model.InstallRemark = remark;
                installBll.Add(model);
            }
            Alert("提交成功", "InstallList.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("InstallList.aspx", false);
        }
    }
}
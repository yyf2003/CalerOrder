using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;

namespace WebApp.Subjects.InstallPrice
{
    public partial class CheckShopDetail1 : System.Web.UI.Page
    {
        int installDetailId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["InstallDetailId"] != null)
            {
                installDetailId = int.Parse(Request.QueryString["InstallDetailId"]);
            }
        }

        void GetInstallShop()
        {
            var list = (from installShop in CurrentContext.DbContext.InstallPriceShopInfo
                        join shop in CurrentContext.DbContext.Shop
                        on installShop.ShopId equals shop.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on installShop.GuidanceId equals guidance.ItemId
                        join subject in CurrentContext.DbContext.Subject
                        on installShop.SubjectId equals subject.Id
                        where installShop.InstallDetailId == installDetailId
                        select new
                        {
                            installShop,
                            shop,
                            guidance.ItemName,
                            subject,
                            installShop.BasicPrice,
                            installShop.WindowPrice,
                            installShop.OOHPrice
                        }).ToList();
            if (!IsPostBack)
            {
                if (list.Any())
                {
                    List<string> provinceList0 = list.Select(s => s.shop.ProvinceName).Distinct().OrderBy(s => s).ToList();
                    if (provinceList0.Any())
                    {
                        provinceList0.ForEach(s =>
                        {
                            ListItem li = new ListItem();
                            li.Value = s;
                            li.Text = s + "&nbsp;";
                            cblProvince.Items.Add(li);
                        });
                    }
                }
            }
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                {
                    provinceList.Add(li.Value);
                }
            }
            if (provinceList.Any())
            {
                list = list.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text.Trim()))
            {
                string shopNo = txtShopNo.Text.Trim().ToLower();
                list = list.Where(s => s.shop.ShopNo != null && s.shop.ShopNo.ToLower().Contains(shopNo)).ToList();
            }
            int totalShopCount = list.Select(s => s.shop.Id).Distinct().Count();
            decimal totalPrice = list.Sum(s => (s.BasicPrice + s.WindowPrice + s.OOHPrice)) ?? 0;
            labShopCount.Text = totalShopCount.ToString();
            labTotalPrice.Text = Math.Round(totalPrice, 2).ToString();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPrice.DataSource = list.OrderBy(s => s.subject.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPrice.DataBind();
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {

        }

        protected void gvPrice_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objBasicPrice = item.GetType().GetProperty("BasicPrice").GetValue(item, null);
                    object objWindowPrice = item.GetType().GetProperty("WindowPrice").GetValue(item, null);
                    object objOOHPrice = item.GetType().GetProperty("OOHPrice").GetValue(item, null);

                    decimal basicPrice = objBasicPrice != null ? decimal.Parse(objBasicPrice.ToString()) : 0;
                    decimal windowPrice = objWindowPrice != null ? decimal.Parse(objWindowPrice.ToString()) : 0;
                    decimal oohPrice = objOOHPrice != null ? decimal.Parse(objOOHPrice.ToString()) : 0;
                    Label labTotal = (Label)e.Item.FindControl("labTotal");
                    labTotal.Text = Math.Round((basicPrice + windowPrice + oohPrice), 2).ToString();
                }
               
            }
        }
    }
}
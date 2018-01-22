using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using BLL;
using DAL;
using Models;
using Common;

namespace WebApp.Statistics
{
    public partial class ByOrderStatistics : BasePage
    {
        FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                BindCustomerList(ref ddlCustomer);
                //BindRegion();
                BindGuidance();
                BindSubject();
                //BindData(null);

            }
        }

        void BindRegion()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            BindRegionByCustomer(customerId, ref cblRegion);
        }

        void BindGuidance()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            int year = DateTime.Now.Year;
            var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && s.BeginDate.Value.Year == year).OrderBy(s => s.ItemId).ToList();

            ddlGuidance.DataSource = list;
            ddlGuidance.DataTextField = "ItemName";
            ddlGuidance.DataValueField = "ItemId";
            ddlGuidance.DataBind();
            ddlGuidance.Items.Insert(0, new ListItem("请选择", "0"));
        }

        void BindSubject()
        {
            cblSubjects.Items.Clear();
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            var subjectList = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1);
            subjectList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Text = s.SubjectName + "&nbsp;";
                li.Value = s.Id.ToString();
                cblSubjects.Items.Add(li);
            });

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            hfSubjectIds.Value = "";
            StringBuilder subjectIds = new StringBuilder();
            List<int> subjectIdList = new List<int>();
            foreach (ListItem li in cblSubjects.Items)
            {
                if (li.Selected)
                {
                    int id = int.Parse(li.Value);
                    if (!subjectIdList.Contains(id))
                    {
                        subjectIdList.Add(id);
                        subjectIds.Append(id + ",");
                    }
                }
            }
            List<string> regionList = new List<string>();
            StringBuilder regions = new StringBuilder();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected && !regionList.Contains(li.Value))
                {
                    regionList.Add(li.Text.ToLower());
                    regions.Append(li.Text.ToLower());
                    regions.Append(",");
                }

            }
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            if (subjectIdList.Any())
            {
                hfSubjectIds.Value = subjectIds.ToString();
                orderList = new FinalOrderDetailTempBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0));
            }
            if (!string.IsNullOrWhiteSpace(txtBegin.Text))
            {
                DateTime begin = DateTime.Parse(txtBegin.Text);
                var subjectList = new SubjectBLL().GetList(s => s.CustomerId == customerId && s.AddDate >= begin && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1).ToList();
                if (!string.IsNullOrWhiteSpace(txtEnd.Text))
                {
                    DateTime end = DateTime.Parse(txtEnd.Text).AddDays(1);
                    subjectList = subjectList.Where(s => s.AddDate < end).ToList();
                }
                if (subjectList.Any())
                {
                    List<int> subjectIdList1 = subjectList.Select(s => s.Id).ToList();
                    hfSubjectIds.Value = StringHelper.ListToString(subjectIdList1);
                    if (orderList.Any())
                    {
                        orderList = orderList.Where(s => subjectIdList1.Contains(s.SubjectId ?? 0)).ToList();
                    }
                    else
                    {
                        orderList = new FinalOrderDetailTempBLL().GetList(s => subjectIdList1.Contains(s.SubjectId ?? 0));
                    }

                }
                else
                    orderList = new List<FinalOrderDetailTemp>();
            }
            if (regionList.Any())
            {
                orderList = orderList.Where(s => regionList.Contains(s.Region.ToLower())).ToList();
            }
            //var list = new FinalOrderDetailTempBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0) && (regionList.Any()?():1==1));
            List<int> shopIdList = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
            labShopCount.Text = shopIdList.Count.ToString();
            decimal area = 0;
            decimal popPrice = 0;
            decimal installPrice = new ShopBLL().GetList(s => shopIdList.Contains(s.Id)).Select(s => s.InstallPrice ?? 0).Sum();
            decimal fahuoPrice = 0;
            orderList.ForEach(s =>
            {
                area += (s.Area ?? 0) * (s.Quantity ?? 1);
                popPrice += (s.Area ?? 0) * (s.UnitPrice ?? 0) * (s.Quantity ?? 1);
            });
            labArea.Text = area > 0 ? (Math.Round(area, 2) + "平方米") : "0";
            labInstallPrice.Text = installPrice > 0 ? (Math.Round(installPrice, 2) + "元") : "0";
            labFahuoPrice.Text = fahuoPrice > 0 ? (Math.Round(fahuoPrice, 2) + "元") : "0";
            if (popPrice > 0)
            {
                labPOPPrice.Text = Math.Round(popPrice, 2) + "元";
                labPOPPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                labPOPPrice.Attributes.Add("data-region", regions.ToString().TrimEnd(','));
                labPOPPrice.Attributes.Add("data-subjectids", subjectIds.ToString().TrimEnd(','));
                labPOPPrice.Attributes.Add("name", "checkMaterial");
            }
            else
            {
                labPOPPrice.Text = "0";
                labPOPPrice.Attributes.Remove("style");
                labPOPPrice.Attributes.Remove("name");
            }

            decimal totalPrice = popPrice + installPrice + fahuoPrice;
            labTotalPrice.Text = totalPrice > 0 ? (Math.Round(totalPrice, 2) + "元") : "0";



            //Session["order"] = orderList;
            BindData(orderList);
        }


        void BindData(List<FinalOrderDetailTemp> orderList)
        {
            //List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            //if (Session["order"] != null)
            //    orderList = (List<FinalOrderDetailTemp>)Session["order"];
            if (orderList == null || !orderList.Any())
            {

                List<int> subjectIdList = new List<int>();
                if (!string.IsNullOrWhiteSpace(hfSubjectIds.Value))
                    subjectIdList = StringHelper.ToIntList(hfSubjectIds.Value, ',');
                List<string> regionList = new List<string>();
                StringBuilder regions = new StringBuilder();
                foreach (ListItem li in cblRegion.Items)
                {
                    if (li.Selected && !regionList.Contains(li.Value))
                    {
                        regionList.Add(li.Text.ToLower());
                        regions.Append(li.Text.ToLower());
                        regions.Append(",");
                    }

                }

                if (subjectIdList.Any())
                {
                    orderList = new FinalOrderDetailTempBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0));
                }
                if (!string.IsNullOrWhiteSpace(txtBegin.Text))
                {
                    DateTime begin = DateTime.Parse(txtBegin.Text);
                    int customerId = int.Parse(ddlCustomer.SelectedValue);
                    var subjectList = new SubjectBLL().GetList(s =>s.CustomerId==customerId && s.AddDate >= begin && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1).ToList();
                    if (!string.IsNullOrWhiteSpace(txtEnd.Text))
                    {
                        DateTime end = DateTime.Parse(txtEnd.Text).AddDays(1);
                        subjectList = subjectList.Where(s => s.AddDate < end).ToList();
                    }
                    if (subjectList.Any())
                    {
                        List<int> subjectIdList1 = subjectList.Select(s => s.Id).ToList();
                        
                        if (orderList.Any())
                        {
                            orderList = orderList.Where(s => subjectIdList1.Contains(s.SubjectId ?? 0)).ToList();
                        }
                        else
                        {
                            orderList = new FinalOrderDetailTempBLL().GetList(s => subjectIdList1.Contains(s.SubjectId ?? 0));
                        }

                    }
                    else
                        orderList = new List<FinalOrderDetailTemp>();
                }
                if (regionList.Any())
                {
                    orderList = orderList.Where(s => regionList.Contains(s.Region.ToLower())).ToList();
                }
            }
            var list = (from order in orderList
                        group order by new { order.ShopNo, order.ShopName, order.ShopId, order.Region } into g
                        select new
                        {
                            g.Key.ShopId,
                            g.Key.ShopNo,
                            g.Key.ShopName,
                            g.Key.Region
                        }).ToList();

            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = list.OrderBy(s => s.ShopId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvList.DataBind();
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData(null);
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
            BindRegion();
        }

        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object shopIdObj = item.GetType().GetProperty("ShopId").GetValue(item, null);
                    int shopId = shopIdObj != null ? int.Parse(shopIdObj.ToString()) : 0;
                    string subjectIds = hfSubjectIds.Value;
                    List<int> subjectIdList = new List<int>();
                    if (!string.IsNullOrWhiteSpace(subjectIds))
                        subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                    var shopOrderList = orderBll.GetList(s => subjectIdList.Contains(s.SubjectId ?? 0) && s.ShopId == shopId);

                    //var shopOrderList = orderList.Where(s => s.ShopId == shopId).ToList();


                    Label areaLab = (Label)e.Item.FindControl("labArea");
                    Label POPPriceLab = (Label)e.Item.FindControl("labPOPPrice");
                    Label InstallFeeLab = (Label)e.Item.FindControl("labInstallFee");
                    Label FahuoFeeLab = (Label)e.Item.FindControl("labFahuoFee");
                    Label subTotalLab = (Label)e.Item.FindControl("labSubTotal");

                    decimal area = 0;
                    decimal popPrice = 0;
                    decimal installPrice = new ShopBLL().GetModel(shopId).InstallPrice ?? 0;
                    decimal fahuoPrice = 0;

                    shopOrderList.ForEach(s =>
                    {
                        area += (s.Area ?? 0) * (s.Quantity ?? 1);
                        popPrice += (s.Area ?? 0) * (s.UnitPrice ?? 0) * (s.Quantity ?? 1);

                    });
                    areaLab.Text = area > 0 ? Math.Round(area, 2).ToString() : "0";
                    InstallFeeLab.Text = installPrice > 0 ? Math.Round(installPrice, 2).ToString() : "0";
                    FahuoFeeLab.Text = fahuoPrice > 0 ? Math.Round(fahuoPrice, 2).ToString() : "0";
                    if (popPrice > 0)
                    {
                        POPPriceLab.Text = Math.Round(popPrice, 2) + "元";
                        POPPriceLab.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                        POPPriceLab.Attributes.Add("data-shopid", shopId.ToString());
                        POPPriceLab.Attributes.Add("data-subjectids", subjectIds.ToString().TrimEnd(','));
                        POPPriceLab.Attributes.Add("name", "checkShopMaterial");
                    }
                    else
                    {
                        labPOPPrice.Text = "0";
                        labPOPPrice.Attributes.Remove("style");
                        labPOPPrice.Attributes.Remove("name");
                    }
                    decimal totalPrice = popPrice + installPrice + fahuoPrice;
                    subTotalLab.Text = totalPrice > 0 ? Math.Round(totalPrice, 2).ToString() : "0";

                }
            }
        }

        public int ShopId { get; set; }
        public string subjectIds { get; set; }

        protected void gvList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Check")
            {
                ShopId = int.Parse(e.CommandArgument.ToString());
                subjectIds = hfSubjectIds.Value;

                //string url = "ShopOrderDetail.aspx?subjectIds=" + subjectIds + "&shopId=" + shopId;
                string url = "ShopOrderDetail.aspx";
                Server.Transfer(url, false);
            }
        }


    }
}
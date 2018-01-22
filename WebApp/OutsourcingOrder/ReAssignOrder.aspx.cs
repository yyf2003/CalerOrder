using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Transactions;
using System.Text;
using Common;
using System.Configuration;

namespace WebApp.OutsourcingOrder
{
    public partial class ReAssignOrder : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["reAssignOrder"] = null;
                Session["reAssignSubject"] = null;
                Session["reAssignGuidance"] = null;
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                BindGuidance();
                Region();
                BindCityTer();
                BindSPIsInstall();
                BindBCSIsInstall();
                BindSheet();
                Panel1.Visible = false;
                Panel2.Visible = true;
                InstallOutsourceList();
            }
        }

        void InstallOutsourceList()
        {
            var companyList = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.IsDelete == null || s.IsDelete == false)).OrderBy(s => s.ProvinceId).ToList();
            ddlType1InstallOutsource.DataSource = companyList;
            ddlType1InstallOutsource.DataTextField = "CompanyName";
            ddlType1InstallOutsource.DataValueField = "Id";
            ddlType1InstallOutsource.DataBind();
            ddlType1InstallOutsource.Items.Insert(0, new ListItem("--请选择--", "0"));

            ddlType2InstallOutsource.DataSource = companyList;
            ddlType2InstallOutsource.DataTextField = "CompanyName";
            ddlType2InstallOutsource.DataValueField = "Id";
            ddlType2InstallOutsource.DataBind();
            ddlType2InstallOutsource.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        void LoadSession()
        {
            List<int> guidanceList = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                {
                    guidanceList.Add(int.Parse(li.Value));
                }
            }
            if (!guidanceList.Any())
            {
                foreach (ListItem li in cblGuidanceList.Items)
                {
                    guidanceList.Add(int.Parse(li.Value));
                }
            }
            var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on subject.GuidanceId equals guidance.ItemId
                             where guidanceList.Contains(subject.GuidanceId ?? 0)
                             && (order.IsDelete == null || order.IsDelete == false)
                             select new { order, subject, guidance }).ToList();
            if (orderList.Any())
            {
                Session["reAssignOrder"] = orderList.Select(s => s.order).Distinct().ToList();
                Session["reAssignSubject"] = orderList.Select(s => s.subject).Distinct().ToList();
                Session["reAssignGuidance"] = orderList.Select(s => s.guidance).Distinct().ToList();
            }
            else
            {
                Session["reAssignOrder"] = null;
                Session["reAssignSubject"] = null;
                Session["reAssignGuidance"] = null;
            }
        }

        void BindGuidance()
        {
            cblGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance

                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance

                        }).Distinct().ToList();


            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();

            }

            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    cblGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyGuidance.Visible = !list.Any();
            LoadSession();
        }

        void Region()
        {
            cblRegion.Items.Clear();
            cblProvince.Items.Clear();
            cblCity.Items.Clear();
            //List<int> guidanceList = new List<int>();
            //foreach (ListItem li in cblGuidanceList.Items)
            //{
            //    if (li.Selected)
            //    {
            //        guidanceList.Add(int.Parse(li.Value));
            //    }
            //}
            //if (!guidanceList.Any())
            //{
            //    foreach (ListItem li in cblGuidanceList.Items)
            //    {
            //        guidanceList.Add(int.Parse(li.Value));
            //    }
            //}
            //if (guidanceList.Any())
            //{
            //    var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //                     join subject in CurrentContext.DbContext.Subject
            //                     on order.SubjectId equals subject.Id
            //                     join guidance in CurrentContext.DbContext.SubjectGuidance
            //                     on subject.GuidanceId equals guidance.ItemId
            //                     where guidanceList.Contains(order.GuidanceId ?? 0)
            //                     && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 3)
            //                     && (order.IsValid == null || order.IsValid == true)
            //                     && (order.IsDelete == null || order.IsDelete == false)
            //                     && (subject.IsDelete == null || subject.IsDelete == false)
            //                     && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
            //                     && (order.IsProduce == null || order.IsProduce == true)
            //                     && (order.IsValidFromAssign == null || order.IsValidFromAssign == true)
            //                     && (order.OrderType != (int)OrderTypeEnum.物料)
            //                     select order.Region).Distinct().ToList();

            //    orderList.ForEach(s =>
            //    {
            //        ListItem li = new ListItem();
            //        li.Value = s;
            //        li.Text = s + "&nbsp;&nbsp;";
            //        cblRegion.Items.Add(li);
            //    });
            //}

            if (Session["reAssignOrder"] == null)
            {
                LoadSession();
            }
            List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
            if (Session["reAssignOrder"] != null)
                orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
            var regionList = orderDetailList.Select(s => s.Region).Distinct().ToList();
            regionList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s;
                li.Text = s + "&nbsp;&nbsp;";
                cblRegion.Items.Add(li);
            });

        }

        void BingProvince()
        {
            cblProvince.Items.Clear();
            cblCity.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value);
                }
            }
            if (Session["reAssignOrder"] == null)
            {
                LoadSession();
            }
            List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
            if (Session["reAssignOrder"] != null)
                orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
            if (regionList.Any())
            {
                orderDetailList = orderDetailList.Where(s => regionList.Contains(s.Region)).ToList();
            }
            var provinceList = orderDetailList.Select(s => s.Province).Distinct().ToList();
            provinceList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s;
                li.Text = s + "&nbsp;&nbsp;";
                cblProvince.Items.Add(li);
            });
        }

        void BindCity()
        {
            cblCity.Items.Clear();
            //List<string> regionList = new List<string>();
            //foreach (ListItem li in cblRegion.Items)
            //{
            //    if (li.Selected)
            //    {
            //        regionList.Add(li.Value);
            //    }
            //}
            List<string> provinceList = new List<string>();
            foreach (ListItem li in cblProvince.Items)
            {
                if (li.Selected)
                {
                    provinceList.Add(li.Value);
                }
            }
            if (Session["reAssignOrder"] == null)
            {
                LoadSession();
            }
            List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
            if (Session["reAssignOrder"] != null)
                orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
            if (provinceList.Any())
            {
                orderDetailList = orderDetailList.Where(s => provinceList.Contains(s.Province)).ToList();
            }
            var cityList = orderDetailList.Select(s => s.City).Distinct().ToList();
            cityList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s;
                li.Text = s + "&nbsp;&nbsp;";
                cblCity.Items.Add(li);
            });
        }

        void BindCityTer()
        {
            cblCityTier.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value);
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
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                {
                    cityList.Add(li.Value);
                }
            }
            if (Session["reAssignOrder"] == null)
            {
                LoadSession();
            }
            List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
            if (Session["reAssignOrder"] != null)
                orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
            if (regionList.Any())
            {
                orderDetailList = orderDetailList.Where(s => regionList.Contains(s.Region)).ToList();
            }
            if (provinceList.Any())
            {
                orderDetailList = orderDetailList.Where(s => provinceList.Contains(s.Province)).ToList();
            }
            if (cityList.Any())
            {
                orderDetailList = orderDetailList.Where(s => cityList.Contains(s.City)).ToList();
            }
            var cityTierList = orderDetailList.Select(s => s.CityTier).Distinct().OrderBy(s => s).ToList();
            cityTierList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s;
                li.Text = s + "&nbsp;&nbsp;";
                cblCityTier.Items.Add(li);
            });
        }


        void BindSPIsInstall()
        {
            cblSPIsInstall.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value);
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
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                {
                    cityList.Add(li.Value);
                }
            }
            List<string> cityTierList = new List<string>();
            foreach (ListItem li in cblCityTier.Items)
            {
                if (li.Selected)
                {
                    cityTierList.Add(li.Value);
                }
            }
            if (Session["reAssignOrder"] == null)
            {
                LoadSession();
            }
            List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
            if (Session["reAssignOrder"] != null)
                orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
            if (regionList.Any())
            {
                orderDetailList = orderDetailList.Where(s => regionList.Contains(s.Region)).ToList();
            }
            if (provinceList.Any())
            {
                orderDetailList = orderDetailList.Where(s => provinceList.Contains(s.Province)).ToList();
            }
            if (cityList.Any())
            {
                orderDetailList = orderDetailList.Where(s => cityList.Contains(s.City)).ToList();
            }
            if (cityTierList.Any())
            {
                orderDetailList = orderDetailList.Where(s => cityTierList.Contains(s.CityTier)).ToList();
            }
            var SPIsInstallList = orderDetailList.Select(s => s.IsInstall).Distinct().OrderBy(s => s).ToList();
            bool isEmpty = false;
            SPIsInstallList.ForEach(s =>
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblSPIsInstall.Items.Add(li);
                }
            });
            if (isEmpty)
            {
                ListItem li = new ListItem();
                li.Value = "空";
                li.Text = "空";
                cblSPIsInstall.Items.Add(li);
            }
        }


        void BindBCSIsInstall()
        {
            cblBCSIsInstall.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value);
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
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                {
                    cityList.Add(li.Value);
                }
            }
            List<string> cityTierList = new List<string>();
            foreach (ListItem li in cblCityTier.Items)
            {
                if (li.Selected)
                {
                    cityTierList.Add(li.Value);
                }
            }
            if (Session["reAssignOrder"] == null)
            {
                LoadSession();
            }
            List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
            if (Session["reAssignOrder"] != null)
                orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
            if (regionList.Any())
            {
                orderDetailList = orderDetailList.Where(s => regionList.Contains(s.Region)).ToList();
            }
            if (provinceList.Any())
            {
                orderDetailList = orderDetailList.Where(s => provinceList.Contains(s.Province)).ToList();
            }
            if (cityList.Any())
            {
                orderDetailList = orderDetailList.Where(s => cityList.Contains(s.City)).ToList();
            }
            if (cityTierList.Any())
            {
                orderDetailList = orderDetailList.Where(s => cityTierList.Contains(s.CityTier)).ToList();
            }
            var bcsIsInstallList = orderDetailList.Select(s => s.BCSIsInstall).Distinct().OrderBy(s => s).ToList();
            bool isEmpty = false;
            bcsIsInstallList.ForEach(s =>
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblBCSIsInstall.Items.Add(li);
                }
            });
            if (isEmpty)
            {
                ListItem li = new ListItem();
                li.Value = "空";
                li.Text = "空";
                cblBCSIsInstall.Items.Add(li);
            }
        }

        void BindSheet()
        {
            cblSheet.Items.Clear();
            List<string> regionList = new List<string>();
            foreach (ListItem li in cblRegion.Items)
            {
                if (li.Selected)
                {
                    regionList.Add(li.Value);
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
            List<string> cityList = new List<string>();
            foreach (ListItem li in cblCity.Items)
            {
                if (li.Selected)
                {
                    cityList.Add(li.Value);
                }
            }
            List<string> cityTierList = new List<string>();
            foreach (ListItem li in cblCityTier.Items)
            {
                if (li.Selected)
                {
                    cityTierList.Add(li.Value);
                }
            }
            List<string> spIsInstallList = new List<string>();
            foreach (ListItem li in cblSPIsInstall.Items)
            {
                if (li.Selected)
                {
                    spIsInstallList.Add(li.Value);
                }
            }
            List<string> bcsIsInstallList = new List<string>();
            foreach (ListItem li in cblBCSIsInstall.Items)
            {
                if (li.Selected)
                {
                    bcsIsInstallList.Add(li.Value);
                }
            }
            if (Session["reAssignOrder"] == null)
            {
                LoadSession();
            }
            List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
            if (Session["reAssignOrder"] != null)
                orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
            if (regionList.Any())
            {
                orderDetailList = orderDetailList.Where(s => regionList.Contains(s.Region)).ToList();
            }
            if (provinceList.Any())
            {
                orderDetailList = orderDetailList.Where(s => provinceList.Contains(s.Province)).ToList();
            }
            if (cityList.Any())
            {
                orderDetailList = orderDetailList.Where(s => cityList.Contains(s.City)).ToList();
            }
            if (cityTierList.Any())
            {
                orderDetailList = orderDetailList.Where(s => cityTierList.Contains(s.CityTier)).ToList();
            }
            if (spIsInstallList.Any())
            {
                if (spIsInstallList.Contains("空"))
                {
                    spIsInstallList.Remove("空");
                    if (spIsInstallList.Any())
                    {
                        orderDetailList = orderDetailList.Where(s => spIsInstallList.Contains(s.IsInstall) || s.IsInstall == null || s.IsInstall == "").ToList();

                    }
                    else
                    {
                        orderDetailList = orderDetailList.Where(s => s.IsInstall == null || s.IsInstall == "").ToList();

                    }
                }
                else
                {
                    orderDetailList = orderDetailList.Where(s => spIsInstallList.Contains(s.IsInstall)).ToList();
                }
            }
            if (bcsIsInstallList.Any())
            {
                if (bcsIsInstallList.Contains("空"))
                {
                    bcsIsInstallList.Remove("空");
                    if (bcsIsInstallList.Any())
                    {
                        orderDetailList = orderDetailList.Where(s => bcsIsInstallList.Contains(s.BCSIsInstall) || s.BCSIsInstall == null || s.BCSIsInstall == "").ToList();

                    }
                    else
                    {
                        orderDetailList = orderDetailList.Where(s => s.BCSIsInstall == null || s.BCSIsInstall == "").ToList();

                    }
                }
                else
                {
                    orderDetailList = orderDetailList.Where(s => bcsIsInstallList.Contains(s.BCSIsInstall)).ToList();
                }
            }
            var sheetList = orderDetailList.Select(s => s.Sheet).Distinct().OrderBy(s => s).ToList();
            bool isEmpty = false;
            sheetList.ForEach(s =>
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblSheet.Items.Add(li);
                }
            });
            if (isEmpty)
            {
                ListItem li = new ListItem();
                li.Value = "空";
                li.Text = "空";
                cblSheet.Items.Add(li);
            }
        }

        void BindMaterialOrderList()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            string guidanceMonth = txtGuidanceMonth.Text;
            int year = 0;
            int month = 0;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                year = date.Year;
                month = date.Month;
            }
            var list = (from order in CurrentContext.DbContext.OutsourceReceivePriceOrder
                       join shop in CurrentContext.DbContext.Shop
                       on order.ShopId equals shop.Id
                       join outsource in CurrentContext.DbContext.Company
                       on order.OutsourceId equals outsource.Id
                       where order.CustomerId == customerId
                       && order.GuidanceYear == year
                       && order.GuidanceMonth == month
                       select new {
                           order,
                           shop,
                           outsource.CompanyName
                       }).ToList();
            Repeater1.DataSource = list;
            Repeater1.DataBind();
            PanelTypeDataList.Visible = list.Any();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
            Region();

        }

        protected void txtMonth_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text.Trim()))
            {
                BindGuidance();
                Region();
            }
        }

        protected void lbUp_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month <= 1)
                {
                    year--;
                    month = 12;
                }
                else
                    month--;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                Region();
                BindCityTer();
                BindSPIsInstall();
                BindBCSIsInstall();
                BindSheet();
            }
        }

        protected void lbDown_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month >= 12)
                {
                    year++;
                    month = 1;
                }
                else
                    month++;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
                Region();
                BindCityTer();
                BindSPIsInstall();
                BindBCSIsInstall();
                BindSheet();
            }

        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Region();
            BindCityTer();
            BindSPIsInstall();
            BindBCSIsInstall();
            BindSheet();
        }

        
        protected void cblRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BingProvince();
            BindCity();
            BindCityTer();
            BindSPIsInstall();
            BindBCSIsInstall();
            BindSheet();
        }

        protected void cblProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCity();
            BindCityTer();
            BindSPIsInstall();
            BindBCSIsInstall();
            BindSheet();
        }

        protected void cblCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCityTer();
            BindSPIsInstall();
            BindBCSIsInstall();
            BindSheet();
        }

        protected void cblCityTier_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSPIsInstall();
            BindBCSIsInstall();
            BindSheet();
        }

        protected void cblSPIsInstall_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSheet();
        }

        protected void cblBCSIsInstall_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSheet();
        }

        protected void btnResearch_Click(object sender, EventArgs e)
        {
            BindShop();
            BindMaterialOrderList();
        }

        void BindShop()
        {
            if (Session["reAssignOrder"] == null)
            {
                LoadSession();
            }
            List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
            if (Session["reAssignOrder"] != null)
                orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
            if (orderDetailList.Any())
            {
                List<string> regionList = new List<string>();
                foreach (ListItem li in cblRegion.Items)
                {
                    if (li.Selected)
                    {
                        regionList.Add(li.Value);
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
                List<string> cityList = new List<string>();
                foreach (ListItem li in cblCity.Items)
                {
                    if (li.Selected)
                    {
                        cityList.Add(li.Value);
                    }
                }
                List<string> cityTierList = new List<string>();
                foreach (ListItem li in cblCityTier.Items)
                {
                    if (li.Selected)
                    {
                        cityTierList.Add(li.Value);
                    }
                }
                List<string> spIsInstallList = new List<string>();
                foreach (ListItem li in cblSPIsInstall.Items)
                {
                    if (li.Selected)
                    {
                        spIsInstallList.Add(li.Value);
                    }
                }
                List<string> bcsIsInstallList = new List<string>();
                foreach (ListItem li in cblBCSIsInstall.Items)
                {
                    if (li.Selected)
                    {
                        bcsIsInstallList.Add(li.Value);
                    }
                }
                List<string> sheetList = new List<string>();
                foreach (ListItem li in cblSheet.Items)
                {
                    if (li.Selected)
                    {
                        sheetList.Add(li.Value);
                    }
                }
                if (regionList.Any())
                {
                    orderDetailList = orderDetailList.Where(s => regionList.Contains(s.Region)).ToList();
                }
                if (provinceList.Any())
                {
                    orderDetailList = orderDetailList.Where(s => provinceList.Contains(s.Province)).ToList();
                }
                if (cityList.Any())
                {
                    orderDetailList = orderDetailList.Where(s => cityList.Contains(s.City)).ToList();
                }
                if (cityTierList.Any())
                {
                    orderDetailList = orderDetailList.Where(s => cityTierList.Contains(s.CityTier)).ToList();
                }
                if (spIsInstallList.Any())
                {
                    if (spIsInstallList.Contains("空"))
                    {
                        spIsInstallList.Remove("空");
                        if (spIsInstallList.Any())
                        {
                            orderDetailList = orderDetailList.Where(s => spIsInstallList.Contains(s.IsInstall) || s.IsInstall == null || s.IsInstall == "").ToList();

                        }
                        else
                        {
                            orderDetailList = orderDetailList.Where(s => s.IsInstall == null || s.IsInstall == "").ToList();

                        }
                    }
                    else
                    {
                        orderDetailList = orderDetailList.Where(s => spIsInstallList.Contains(s.IsInstall)).ToList();
                    }
                }
                if (bcsIsInstallList.Any())
                {
                    if (bcsIsInstallList.Contains("空"))
                    {
                        bcsIsInstallList.Remove("空");
                        if (bcsIsInstallList.Any())
                        {
                            orderDetailList = orderDetailList.Where(s => bcsIsInstallList.Contains(s.BCSIsInstall) || s.BCSIsInstall == null || s.BCSIsInstall == "").ToList();

                        }
                        else
                        {
                            orderDetailList = orderDetailList.Where(s => s.BCSIsInstall == null || s.BCSIsInstall == "").ToList();

                        }
                    }
                    else
                    {
                        orderDetailList = orderDetailList.Where(s => bcsIsInstallList.Contains(s.BCSIsInstall)).ToList();
                    }
                }
                if (sheetList.Any())
                {
                    if (sheetList.Contains("空"))
                    {
                        sheetList.Remove("空");
                        if (sheetList.Any())
                        {
                            orderDetailList = orderDetailList.Where(s => sheetList.Contains(s.Sheet) || s.Sheet == null || s.Sheet == "").ToList();

                        }
                        else
                        {
                            orderDetailList = orderDetailList.Where(s => s.Sheet == null || s.Sheet == "").ToList();

                        }
                    }
                    else
                    {
                        orderDetailList = orderDetailList.Where(s => sheetList.Contains(s.Sheet)).ToList();
                    }
                }
                if (cbOOHInstallPrice.Checked)
                {
                    orderDetailList = orderDetailList.Where(s=>(s.PayOOHInstallPrice??0)>0).ToList();
                }
                if (!string.IsNullOrWhiteSpace(txtSearchShopNo.Text.Trim()))
                {
                    string shopNos = txtSearchShopNo.Text.Trim().Replace('，', ',');
                    List<string> shopNoList = StringHelper.ToStringList(shopNos, ',', LowerUpperEnum.ToLower);
                    if (shopNoList.Any())
                    {
                        orderDetailList = orderDetailList.Where(s =>s.ShopNo!=null && shopNoList.Contains(s.ShopNo.ToLower())).ToList();
                    }
                }
                string material = string.Empty;
                foreach (ListItem li in cblMaterial.Items)
                {
                    if (li.Selected)
                        material=li.Value.ToLower();
                }
                if (!string.IsNullOrWhiteSpace(material))
                {
                    orderDetailList = orderDetailList.Where(s => s.GraphicMaterial != null && s.GraphicMaterial.ToLower().Contains(material)).ToList();
                }
                if (orderDetailList.Any())
                {

                    Panel1.Visible = true;
                    Panel2.Visible = false;
                    List<int> shopIdList = orderDetailList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                    labShopCount.Text = shopIdList.Count.ToString();
                    labOrderCount.Text = orderDetailList.Count.ToString();
                    var shopList = (from shop in orderDetailList
                                    group shop by new
                                    {
                                        shop.ShopId,
                                        shop.ShopNo,
                                        shop.ShopName,
                                        shop.Region,
                                        shop.Province,
                                        shop.City,
                                        shop.CityTier,
                                        shop.IsInstall,
                                        shop.Format

                                    } into g
                                    select new
                                    {

                                        g.Key.ShopId,
                                        g.Key.ShopNo,
                                        g.Key.ShopName,
                                        g.Key.Region,
                                        g.Key.Province,
                                        g.Key.City,
                                        g.Key.CityTier,
                                        g.Key.IsInstall,
                                        g.Key.Format,
                                        OrderCount = g.Select(s => s.Id).Count()
                                    }).ToList();
                    AspNetPager1.RecordCount = shopList.Count;
                    this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
                    Repeater_shopList.DataSource = shopList.OrderBy(s => s.ShopNo).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
                    Repeater_shopList.DataBind();

                }
                else
                {
                    Panel1.Visible = false;
                    Panel2.Visible = true;
                    labShopCount.Text = "0";
                    labOrderCount.Text = "0";
                }
            }
            else
            {
                Panel1.Visible = false;
                Panel2.Visible = true;
                labShopCount.Text = "0";
                labOrderCount.Text = "0";
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindShop();
        }

        protected void btnSubmitType1_Click(object sender, EventArgs e)
        {
           
            int orderCount = 0;
            if (Repeater_shopList.Items.Count > 0)
            {
                int customerId=int.Parse(ddlCustomer.SelectedValue);
                string guidanceMonth = txtGuidanceMonth.Text;
                int year = 0;
                int month = 0;
                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                {
                    DateTime date = DateTime.Parse(guidanceMonth);
                    year = date.Year;
                    month = date.Month;
                }
                int outsourceId = int.Parse(ddlType1InstallOutsource.SelectedValue);
                int count = 0;
                if (!string.IsNullOrWhiteSpace(txtMaterialNum.Text))
                {
                    count = StringHelper.IsInt(txtMaterialNum.Text.Trim());
                }
                decimal price = 0;
                if (!string.IsNullOrWhiteSpace(txtMaterialTotalPrice.Text))
                {
                    price = StringHelper.IsDecimal(txtMaterialTotalPrice.Text.Trim());
                }
                string materialName = ddlMaterialName.SelectedValue;
                OutsourceReceivePriceOrderBLL bll = new OutsourceReceivePriceOrderBLL();
                OutsourceReceivePriceOrder model;
                //List<int> shopIdList = new List<int>();
                foreach (RepeaterItem item in Repeater_shopList.Items)
                {
                    CheckBox cbOne = (CheckBox)item.FindControl("cbOne");
                    if (cbOne.Checked)
                    {
                        HiddenField hfShopId = (HiddenField)item.FindControl("hfShopId");
                        if (!string.IsNullOrWhiteSpace(hfShopId.Value))
                        {
                            int shopId = int.Parse(hfShopId.Value);
                            model = new OutsourceReceivePriceOrder();
                            model.AddDate = DateTime.Now;
                            model.AddUserId = CurrentUser.UserId;
                            model.Count = count;
                            model.CustomerId = customerId;
                            model.GuidanceMonth = month;
                            model.GuidanceYear = year;
                            model.MaterialName = materialName;
                            model.OutsourceId = outsourceId;
                            model.ShopId = shopId;
                            model.TotalPrice = price;
                            bll.Add(model);
                            orderCount++;
                        }
                          
                    }
                }
                


            }
            if (orderCount > 0)
            {
                BindMaterialOrderList();
            }
            
            


        }

        protected void btnSubmitType2_Click(object sender, EventArgs e)
        {
            int orderCount = 0;
            int outsourceId = int.Parse(ddlType2InstallOutsource.SelectedValue);
            string msg = string.Empty;
            if (outsourceId>0 && Repeater_shopList.Items.Count > 0)
            {
                List<int> shopIdList = new List<int>();
                foreach (RepeaterItem item in Repeater_shopList.Items)
                {
                    CheckBox cbOne = (CheckBox)item.FindControl("cbOne");
                    if (cbOne.Checked)
                    {
                        HiddenField hfShopId = (HiddenField)item.FindControl("hfShopId");
                        if (!string.IsNullOrWhiteSpace(hfShopId.Value))
                            shopIdList.Add(int.Parse(hfShopId.Value));
                    }
                }
                if (shopIdList.Any())
                {
                    OutsourceOrderDetailBLL orderBll = new OutsourceOrderDetailBLL();
                    List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
                    if (Session["reAssignOrder"] != null)
                        orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
                    if (orderDetailList.Any())
                    {
                        orderDetailList = orderDetailList.Where(s => shopIdList.Contains(s.ShopId ?? 0) && s.OrderType==(int)OrderTypeEnum.安装费 && (s.PayOOHInstallPrice??0)>0).ToList();
                        if (orderDetailList.Any())
                        {
                            orderDetailList.ForEach(s =>
                            {
                                if (s.OutsourceId != outsourceId)
                                {
                                    OutsourceOrderDetail newModel = new OutsourceOrderDetail();
                                    newModel = s;
                                    decimal oohInstallPrice = s.PayOOHInstallPrice ?? 0;
                                    s.PayOOHInstallPrice = 0;
                                    s.PayOrderPrice = s.PayOrderPrice - oohInstallPrice;
                                    s.ModifyDate = DateTime.Now;
                                    s.ModifyUserId = CurrentUser.UserId;
                                    s.ModifyRemark = "更改高空安装外协";
                                    orderBll.Update(s);

                                    newModel.OutsourceId = outsourceId;
                                    newModel.PayOrderPrice = oohInstallPrice;
                                    newModel.PayBasicInstallPrice = 0;
                                    newModel.ReceiveOrderPrice = 0;
                                    newModel.ModifyDate = DateTime.Now;
                                    newModel.ModifyUserId = CurrentUser.UserId;
                                    newModel.ModifyRemark = "高空安装新外协，原外协：" + s.OutsourceId;
                                    orderBll.Add(newModel);
                                    orderCount++;
                                }
                            });
                        }
                        else
                            msg = "无OOH安装订单";
                    }
                }
            }
            if (orderCount > 0)
            {
                labType2State.Text = string.Format("更新{0}条数据", orderCount);
                LoadSession();
                BindShop();
            }
            else
                labType2State.Text = "更新0条数据" + (!string.IsNullOrWhiteSpace(msg)?("："+msg):"");

        }

        protected void Repeater_shopList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objShopId = item.GetType().GetProperty("ShopId").GetValue(item, null);
                    int shopId = objShopId != null ? int.Parse(objShopId.ToString()) : 0;
                    List<OutsourceOrderDetail> orderDetailList = new List<OutsourceOrderDetail>();
                    if (Session["reAssignOrder"] != null)
                        orderDetailList = Session["reAssignOrder"] as List<OutsourceOrderDetail>;
                    if (orderDetailList.Any())
                    {
                        var list = (from order in orderDetailList
                                   join company in CurrentContext.DbContext.Company
                                   on order.OutsourceId equals company.Id
                                    where order.ShopId == shopId
                                   select new { 
                                      order,
                                      company
                                   }).ToList();
                        List<string> sheetList = new List<string>();
                        foreach (ListItem li in cblSheet.Items)
                        {
                            if (li.Selected)
                            {
                                sheetList.Add(li.Value);
                            }
                        }
                        string material = string.Empty;
                        foreach (ListItem li in cblMaterial.Items)
                        {
                            if (li.Selected)
                                material = li.Value.ToLower();
                        }
                        if (sheetList.Any())
                        {
                            if (sheetList.Contains("空"))
                            {
                                sheetList.Remove("空");
                                if (sheetList.Any())
                                {
                                    list = list.Where(s => sheetList.Contains(s.order.Sheet) || s.order.Sheet == null || s.order.Sheet == "").ToList();

                                }
                                else
                                {
                                    list = list.Where(s => s.order.Sheet == null || s.order.Sheet == "").ToList();

                                }
                            }
                            else
                            {
                                list = list.Where(s => sheetList.Contains(s.order.Sheet)).ToList();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(material))
                        {
                            list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.ToLower().Contains(material)).ToList();
                        }
                        if (cbOOHInstallPrice.Checked)
                        {
                            list = list.Where(s => (s.order.PayOOHInstallPrice ?? 0) > 0).ToList();
                        }
                        List<string> nameList = new List<string>();
                        string orderTypeName = string.Empty;
                        list.ForEach(s => {
                            orderTypeName = CommonMethod.GetEnumDescription<OutsourceOrderTypeEnum>((s.order.AssignType ?? 1).ToString());
                            orderTypeName = s.company.CompanyName + "(" + orderTypeName + ")";
                            if (!nameList.Contains(orderTypeName))
                            {
                                nameList.Add(orderTypeName);
                            }
                        });
                        string outsourceName = StringHelper.ListToString(nameList, "/");
                        ((Label)e.Item.FindControl("labOutsource")).Text = outsourceName;
                    }
                }
            }
        }

        protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "deleteItem")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                OutsourceReceivePriceOrderBLL bll = new OutsourceReceivePriceOrderBLL();
                OutsourceReceivePriceOrder model = bll.GetModel(id);
                if (model != null)
                {
                    bll.Delete(model);
                    BindMaterialOrderList();
                }
            }
        }

        

        

        
    }
}
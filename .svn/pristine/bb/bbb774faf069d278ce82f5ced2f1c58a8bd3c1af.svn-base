using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using System.Data;
using Common;
using DAL;

namespace WebApp.Quotation
{
    public partial class List1 : BasePage
    {
        QuotationsBLL quotaltionBll = new QuotationsBLL();
        ADSubjectPriceBLL adPriceBll = new ADSubjectPriceBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                //BindGuidance();
                BindADContacts();

            }
        }


        void BindADContacts()
        {
            var list = quotaltionBll.GetList(s => s.Id > 0);
            if (list.Any())
            {
                List<string> contacts = new List<string>();
                list.ForEach(s =>
                {
                    string contact = s.AdidasContact;
                    contact = contact.Replace('，', ',').Replace('\\', ',').Replace('/', ',').Replace('、', ',');
                    string[] arr = contact.Split(',');
                    foreach (string c in arr)
                    {
                        if (!contacts.Contains(c))
                        {
                            contacts.Add(c);
                        }
                    }

                });
                contacts.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;";
                    cbADContact.Items.Add(li);
                });
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<int> curstomerList = new List<int>();
            List<int> companyList = MySonCompanyList.Select(s => s.Id).ToList();
            foreach (ListItem item in ddlCustomer.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }
            var list = (from qu in CurrentContext.DbContext.Quotations
                        join subject in CurrentContext.DbContext.Subject
                        on qu.SubjectId equals subject.Id
                        join supplement1 in CurrentContext.DbContext.AccountCheckSupplement
                       on subject.Id equals supplement1.SubjectId into temp
                        from supplement in temp.DefaultIfEmpty()
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1) && companyList.Contains(subject.CompanyId ?? 0)
                        select new
                        {
                            qu,
                            subject,
                            supplement
                        }).OrderBy(s => s.qu.SubjectId).ToList();
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.subject.CustomerId == cid).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                list = list.Where(s => s.subject.SubjectName.Contains(txtSubjectName.Text)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectNo.Text))
            {
                list = list.Where(s => s.subject.SubjectNo.Contains(txtSubjectNo.Text)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtCRNumber.Text))
            {
                list = list.Where(s => s.supplement != null && s.supplement.CRNumber.Contains(txtCRNumber.Text.Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtPONumber.Text))
            {
                list = list.Where(s => s.supplement != null && s.supplement.PONumber.Contains(txtPONumber.Text.Trim())).ToList();
            }

            int invoiceState = int.Parse(rblIsInvoice.SelectedValue);
            if (invoiceState > 0)
            {
                var invoiceSubjectList = new ADInvoiceBLL().GetList(s => s.Id > 0).Select(s => s.SubjectId ?? 0).ToList();
                if (invoiceSubjectList.Any())
                {
                    if (invoiceState == 1)
                    {
                        list = list.Where(s => invoiceSubjectList.Contains(s.subject.Id)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => !invoiceSubjectList.Contains(s.subject.Id)).ToList();
                    }
                }
                else
                {
                    list.Clear();
                }
            }


            if (!string.IsNullOrWhiteSpace(txtBeginDate.Text))
            {
                DateTime begin = DateTime.Parse(txtBeginDate.Text);
                list = list.Where(s => s.subject.BeginDate >= begin).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtEndDate.Text))
            {
                DateTime end = DateTime.Parse(txtEndDate.Text).AddDays(1);
                list = list.Where(s => s.subject.BeginDate < end).ToList();
            }
            if (list.Any())
            {
                CreateTb();
                List<int> qIdList = list.Select(s => s.qu.Id).ToList();
                var priceList = adPriceBll.GetList(s => qIdList.Contains(s.QuotationsId ?? 0));
                if (priceList.Any())
                {
                    var shareList = priceList.Where(s => s.PriceType == 1).ToList();
                    if (shareList.Any())
                    {
                        shareList.ForEach(s =>
                        {
                            string name = s.PriceName.IndexOf("分摊金额-") != -1 ? s.PriceName : "分摊金额-" + s.PriceName;
                            if (!exportTB.Columns.Contains(name))
                            {
                                exportTB.Columns.Add(new DataColumn(name, Type.GetType("System.String")));
                            }
                        });
                    }

                    var bingList = priceList.Where(s => s.PriceType == 2).ToList();
                    if (bingList.Any())
                    {
                        bingList.ForEach(s =>
                        {
                            string name = s.PriceName.IndexOf("并入金额-") != -1 ? s.PriceName : "并入金额-" + s.PriceName;
                            if (!exportTB.Columns.Contains(name))
                            {
                                exportTB.Columns.Add(new DataColumn(name, Type.GetType("System.String")));
                            }
                        });
                        exportTB.Columns.Add(new DataColumn("并入金额归属", Type.GetType("System.String")));
                    }
                }

                exportTB.Columns.AddRange(new DataColumn[] {
                   new DataColumn("CR名称",Type.GetType("System.String")),
                   new DataColumn("CR号",Type.GetType("System.String")),
                   new DataColumn("PO号",Type.GetType("System.String")),
                   new DataColumn("备注",Type.GetType("System.String"))
                });

                list.ForEach(s =>
                {
                    DataRow dr = exportTB.NewRow();
                    dr["报价日期"] = s.qu.AddDate != null ? DateTime.Parse(s.qu.AddDate.ToString()).ToShortDateString() : "";
                    dr["项目编号"] = s.subject.SubjectNo;
                    dr["项目名称"] = s.subject.SubjectName;

                    dr["CR名称"] = s.supplement != null ? s.supplement.CRName : "";
                    dr["CR号"] = s.supplement != null ? s.supplement.CRNumber : "";
                    dr["PO号"] = s.supplement != null ? s.supplement.PONumber : "";

                    dr["类别"] = s.qu.Category;
                    dr["AD费用归属"] = s.qu.Blongs;
                    dr["分类"] = s.qu.Classification;
                    dr["卡乐项目负责人"] = s.subject.Contact;
                    dr["Adidas负责人"] = s.qu.AdidasContact;
                    dr["税率"] = s.qu.TaxRate;
                    decimal offerPrice = s.qu.OfferPrice ?? 0;
                    decimal otherPrice = s.qu.OtherPrice ?? 0;
                    dr["汇总金额"] = (offerPrice + otherPrice).ToString("0.00");
                    dr["报价金额"] = s.qu.OfferPrice != null ? s.qu.OfferPrice.ToString() : "";
                    dr["挂账金额"] = s.qu.OtherPrice != null ? s.qu.OtherPrice.ToString() : "";
                    dr["挂账说明"] = s.qu.OtherPriceRemark;
                    dr["报价账户"] = s.qu.Account;

                    dr["备注"] = s.qu.Remark;

                    var priceList1 = adPriceBll.GetList(p => p.QuotationsId == s.qu.Id);
                    var shareList1 = priceList1.Where(p => p.PriceType == 1).ToList();
                    if (shareList1.Any())
                    {
                        shareList1.ForEach(sh =>
                        {
                            string name = sh.PriceName.IndexOf("分摊金额-") != -1 ? sh.PriceName : "分摊金额-" + sh.PriceName;
                            if (exportTB.Columns[name] != null)
                            {
                                dr["" + name + ""] = sh.Price != null ? sh.Price.ToString() : "";
                            }
                        });
                    }
                    var bingList1 = priceList1.Where(p => p.PriceType == 2).ToList();
                    if (bingList1.Any())
                    {
                        bingList1.ForEach(sh =>
                        {
                            string name = sh.PriceName.IndexOf("并入金额-") != -1 ? sh.PriceName : "并入金额-" + sh.PriceName;
                            if (exportTB.Columns[name] != null)
                            {
                                dr["" + name + ""] = sh.Price != null ? sh.Price.ToString() : "";
                            }
                        });
                        dr["并入金额归属"] = "总部";
                    }
                    exportTB.Rows.Add(dr);
                });

            }
            if (exportTB != null && exportTB.Rows.Count > 0)
            {
                OperateFile.ExportExcel(exportTB, "AD项目费用信息");
            }
            else
            {
                Alert("没有数据可以导出");
            }
        }

        DataTable exportTB = null;
        void CreateTb()
        {
            exportTB = new DataTable();
            exportTB.Columns.AddRange(new DataColumn[] { 
               new DataColumn("报价日期",Type.GetType("System.String")),
               new DataColumn("项目编号",Type.GetType("System.String")),
               new DataColumn("项目名称",Type.GetType("System.String")),
               new DataColumn("分类",Type.GetType("System.String")),
               new DataColumn("卡乐项目负责人",Type.GetType("System.String")),
               new DataColumn("Adidas负责人",Type.GetType("System.String")),
               new DataColumn("AD费用归属",Type.GetType("System.String")),
               new DataColumn("类别",Type.GetType("System.String")),
               new DataColumn("报价账户",Type.GetType("System.String")),
               new DataColumn("汇总金额",Type.GetType("System.String")),
               new DataColumn("报价金额",Type.GetType("System.String")),
               new DataColumn("挂账金额",Type.GetType("System.String")),
               new DataColumn("挂账说明",Type.GetType("System.String")),
               new DataColumn("税率",Type.GetType("System.String"))
            });
        }

        void BindGuidance()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            int year = DateTime.Now.Year;
            var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && s.BeginDate.Value.Year == year).OrderBy(s => s.ItemId).ToList();
            if (list.Any())
            {
                ddlGuidance.DataSource = list;
                ddlGuidance.DataTextField = "ItemName";
                ddlGuidance.DataValueField = "ItemId";
                ddlGuidance.DataBind();
                ddlGuidance.Items.Add(new ListItem("其他", "0"));
            }
            ddlGuidance.Items.Insert(0, new ListItem("--请选择--", "-1"));

        }

        //void BindRegion()
        //{
        //    cblRegion.Items.Clear();
        //    int guidanceId = int.Parse(ddlGuidance.SelectedValue);
        //    var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
        //                join subject in CurrentContext.DbContext.Subject
        //                on order.SubjectId equals subject.Id
        //                join shop in CurrentContext.DbContext.Shop
        //                on order.ShopId equals shop.Id
        //                where subject.GuidanceId == guidanceId
        //                select shop.RegionName).Distinct().ToList();
        //    if (list.Any())
        //    {
        //        list.ForEach(s => {
        //            ListItem li = new ListItem();
        //            li.Text = s + "&nbsp;";
        //            li.Value = s;
        //            cblRegion.Items.Add(li);
        //        });
        //    }
        //}

        //void BindProvince()
        //{
        //    cblProvince.Items.Clear();
        //    cblCity.Items.Clear();
        //    int guidanceId = int.Parse(ddlGuidance.SelectedValue);
        //    List<string> regionList = new List<string>();
        //    foreach (ListItem li in cblRegion.Items)
        //    {
        //        if (li.Selected)
        //        {
        //            regionList.Add(li.Value.ToLower());
        //        }
        //    }
        //    var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
        //                join subject in CurrentContext.DbContext.Subject
        //                on order.SubjectId equals subject.Id
        //                join shop in CurrentContext.DbContext.Shop
        //                on order.ShopId equals shop.Id
        //                where subject.GuidanceId == guidanceId && regionList.Contains(shop.RegionName.ToLower())
        //                select shop).ToList();
        //    if (list.Any())
        //    {
        //        var provinceList = list.OrderBy(s=>s.RegionName).Select(s=>s.ProvinceName).Distinct().ToList();
        //        provinceList.ForEach(s => {
        //            ListItem li = new ListItem();
        //            li.Text = s + "&nbsp;";
        //            li.Value = s;
        //            cblProvince.Items.Add(li);
        //        });
        //    }
        //}

        //void BindCity()
        //{
        //    cblCity.Items.Clear();
        //    int guidanceId = int.Parse(ddlGuidance.SelectedValue);
        //    List<string> provinceList = new List<string>();
        //    foreach (ListItem li in cblProvince.Items)
        //    {
        //        if (li.Selected)
        //        {
        //            provinceList.Add(li.Value);
        //        }
        //    }
        //    var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
        //                join subject in CurrentContext.DbContext.Subject
        //                on order.SubjectId equals subject.Id
        //                join shop in CurrentContext.DbContext.Shop
        //                on order.ShopId equals shop.Id
        //                where subject.GuidanceId == guidanceId && provinceList.Contains(shop.ProvinceName)
        //                select shop).ToList();
        //    if (list.Any())
        //    {
        //        var cityList = list.OrderBy(s => s.ProvinceName).Select(s => s.CityName).Distinct().ToList();
        //        cityList.ForEach(s =>
        //        {
        //            ListItem li = new ListItem();
        //            li.Text = s + "&nbsp;";
        //            li.Value = s;
        //            cblCity.Items.Add(li);
        //        });
        //    }
        //}

    

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;
using System.Data;

namespace WebApp.Quotation
{
    public partial class List : BasePage
    {
        QuotationsBLL quotaltionBll = new QuotationsBLL();
        ADSubjectPriceBLL adPriceBll = new ADSubjectPriceBLL();
        AttachmentBLL attachBll = new AttachmentBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                BindADContacts();
                BindData();
            }
        }

        


        void BindADContacts()
        {
            var list = quotaltionBll.GetList(s=>s.Id>0);
            if (list.Any())
            {
                List<string> contacts = new List<string>();
                list.ForEach(s => {
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
                contacts.ForEach(s => {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + " ";
                    cbADContact.Items.Add(li);
                });
            }
        }

        void BindData()
        {
            List<int> curstomerList = new List<int>();
            List<int> companyList = MySonCompanyList.Select(s => s.Id).ToList();
            foreach (ListItem item in ddlCustomer.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }
            var list = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on subject.AddUserId equals user.UserId
                        join supplement1 in CurrentContext.DbContext.AccountCheckSupplement
                        on subject.Id equals supplement1.SubjectId into temp
                        from supplement in temp.DefaultIfEmpty()
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1) && companyList.Contains(subject.CompanyId ?? 0)
                        select new
                        {
                            subject.Id,
                            subject.AddDate,
                            subject.ApproveState,
                            subject.ApproveUserId,
                            subject.BeginDate,
                            subject.Contact,
                            subject.EndDate,
                            subject.IsDelete,
                            subject.OutSubjectName,
                            subject.Remark,
                            subject.Status,
                            subject.SubjectName,
                            subject.SubjectNo,
                            subject.Tel,
                            subject.CustomerId,
                            customer.CustomerName,
                            user.RealName,
                            supplement.CRNumber,
                            supplement.PONumber
                        }).ToList();
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }
            List<string> adContacts = new List<string>();
            foreach (ListItem li in cbADContact.Items)
            {
                if (li.Selected)
                {
                    if (!adContacts.Contains(li.Value))
                    {
                        adContacts.Add(li.Value);
                    }
                }
            }
            if (adContacts.Any())
            {
                List<int> subjectIds = quotaltionBll.GetList(s => adContacts.Contains(s.AdidasContact)).Select(s => s.SubjectId??0).ToList();
                list = list.Where(s => subjectIds.Contains(s.Id)).ToList();
            }



            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.Contains(txtSubjectName.Text)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectNo.Text))
            {
                list = list.Where(s => s.SubjectNo.Contains(txtSubjectNo.Text)).ToList();
            }
            //if (!string.IsNullOrWhiteSpace(txtBeginDate.Text))
            //{
            //    DateTime begin = DateTime.Parse(txtBeginDate.Text);
            //    list = list.Where(s => s.BeginDate >= begin).ToList();
            //}
            //if (!string.IsNullOrWhiteSpace(txtEndDate.Text))
            //{
            //    DateTime end = DateTime.Parse(txtEndDate.Text).AddDays(1);
            //    list = list.Where(s => s.BeginDate < end).ToList();
            //}

            if (!string.IsNullOrWhiteSpace(txtCRNumber.Text))
            {
                list = list.Where(s => s.CRNumber != null && s.CRNumber.Contains(txtCRNumber.Text.Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtPONumber.Text))
            {
                list = list.Where(s => s.PONumber != null && s.PONumber.Contains(txtPONumber.Text.Trim())).ToList();
            }

            if (rblSubmitPrice.SelectedValue != "0")
            {
                List<int> subjectIdList = list.Select(s => s.Id).ToList();
                var quList = quotaltionBll.GetList(q => subjectIdList.Contains(q.SubjectId ?? 0));
                if (quList.Any())
                {
                    List<int> submitList = quList.Select(q => q.SubjectId??0).Distinct().ToList();
                    if (rblSubmitPrice.SelectedValue == "1")
                    {
                        list = list.Where(s => submitList.Contains(s.Id)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => !submitList.Contains(s.Id)).ToList();
                    }
                }
                else
                {
                    list.Clear();
                }
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            hfIsFinishImport.Value = "";
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        
        string fileCode = ((int)FileCodeEnum.SubjectQuotation).ToString();
        decimal totalMoney = 0;
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType== DataControlRowType.DataRow)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object IdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int id = IdObj != null ? int.Parse(IdObj.ToString()) : 0;
                    var list = attachBll.GetList(s=>s.SubjectId==id && s.FileCode==fileCode && (s.IsDelete==null || s.IsDelete==false));
                    ((Label)e.Row.FindControl("labFileNum")).Text = list.Count.ToString();
                    System.Text.StringBuilder adContacts = new System.Text.StringBuilder();
                    List<string> contactList = new List<string>();
                    var qList = quotaltionBll.GetList(s=>s.SubjectId==id);
                    if (qList.Any())
                    {
                        decimal total = 0;
                        qList.ForEach(s => {
                            total += s.OfferPrice ?? 0;
                            if (!contactList.Contains(s.AdidasContact))
                            {
                                contactList.Add(s.AdidasContact);
                            }
                        });
                        ((Label)e.Row.FindControl("labTotalPrice")).Text = total > 0 ? total.ToString("0.00") : "0";
                        totalMoney += total;
                    }
                    if (contactList.Any())
                    {
                        contactList.ForEach(s => {
                            adContacts.Append(s+"/");
                        });
                        ((Label)e.Row.FindControl("labADContact")).Text = adContacts.ToString().TrimEnd('/');
                    }
                }
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                ((Label)e.Row.FindControl("labTotal")).Text = totalMoney > 0 ? totalMoney.ToString("0.00") : "0";
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

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
                       select new {
                           qu,
                           subject,
                           supplement
                       }).OrderBy(s=>s.qu.SubjectId).ToList();
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
            //if (!string.IsNullOrWhiteSpace(txtBeginDate.Text))
            //{
            //    DateTime begin = DateTime.Parse(txtBeginDate.Text);
            //    list = list.Where(s => s.subject.BeginDate >= begin).ToList();
            //}
            //if (!string.IsNullOrWhiteSpace(txtEndDate.Text))
            //{
            //    DateTime end = DateTime.Parse(txtEndDate.Text).AddDays(1);
            //    list = list.Where(s => s.subject.BeginDate < end).ToList();
            //}
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
                            string name = s.PriceName.IndexOf("合并金额-") != -1 ? s.PriceName : "合并金额-" + s.PriceName;
                            if (!exportTB.Columns.Contains(name))
                            {
                                exportTB.Columns.Add(new DataColumn(name, Type.GetType("System.String")));
                            }
                        });
                    }
                }

                list.ForEach(s =>
                {
                    DataRow dr = exportTB.NewRow();
                    dr["报价日期"] = s.qu.AddDate != null ? DateTime.Parse(s.qu.AddDate.ToString()).ToShortDateString() : "";
                    dr["项目编号"] = s.subject.SubjectNo;
                    dr["项目名称"] = s.subject.SubjectName;

                    dr["CR名称"] = s.supplement!=null?s.supplement.CRName:"";
                    dr["CR号"] = s.supplement != null ? s.supplement.CRNumber : "";
                    dr["PO号"] = s.supplement != null ? s.supplement.PONumber : "";

                    dr["类别"] = s.qu.Category;
                    dr["AD费用归属"] = s.qu.Blongs;
                    dr["分类"] = s.qu.Classification;
                    dr["卡乐项目负责人"] = s.subject.Contact;
                    dr["Adidas负责人"] = s.qu.AdidasContact;
                    dr["税率"] = s.qu.TaxRate;
                    dr["报价金额"] = s.qu.OfferPrice != null ? s.qu.OfferPrice.ToString() : "";
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
                            string name = sh.PriceName.IndexOf("合并金额-") != -1 ? sh.PriceName : "合并金额-" + sh.PriceName;
                            if (exportTB.Columns[name] != null)
                            {
                                dr["" + name + ""] = sh.Price != null ? sh.Price.ToString() : "";
                            }
                        });
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

               new DataColumn("CR名称",Type.GetType("System.String")),
               new DataColumn("CR号",Type.GetType("System.String")),
               new DataColumn("PO号",Type.GetType("System.String")),

               new DataColumn("类别",Type.GetType("System.String")),
               new DataColumn("AD费用归属",Type.GetType("System.String")),
               new DataColumn("分类",Type.GetType("System.String")),
               new DataColumn("卡乐项目负责人",Type.GetType("System.String")),
               new DataColumn("Adidas负责人",Type.GetType("System.String")),
               new DataColumn("税率",Type.GetType("System.String")),
               new DataColumn("报价金额",Type.GetType("System.String")),
               new DataColumn("报价账户",Type.GetType("System.String")),
               new DataColumn("备注",Type.GetType("System.String")),
            });
        }
    }
}
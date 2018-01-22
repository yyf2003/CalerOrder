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
using System.Data;
using Common;

namespace WebApp.Quotation
{
    public partial class PriceList : BasePage
    {
        public int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectid"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectid"]);
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindData();
            }
        }

        void BindSubject()
        {
            Subject model = new SubjectBLL().GetModel(subjectId);
            if (model != null)
            {
                labSubjectNo.Text = model.SubjectNo;
                labSubjectName.Text = model.SubjectName;
                labContact.Text = model.Contact;
            }
        }

        void BindData()
        {
           
            var list = (from price in CurrentContext.DbContext.Quotations
                       join subject in CurrentContext.DbContext.Subject
                       on price.SubjectId equals subject.Id
                       where price.SubjectId == subjectId
                       select new { 
                          subject.SubjectName,
                          subject.SubjectNo,
                          price.Id,
                          price.Account,
                          price.AdidasContact,
                          price.Blongs,
                          price.Category,
                          price.Classification,
                          price.OfferPrice,
                          price.OtherPrice,
                          price.OtherPriceRemark,
                          price.TaxRate,
                          price.AddDate
                         
                       }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "DeleteItem")
            {
                //using (TransactionScope tran = new TransactionScope())
                //{
                //    try
                //    {
                //new ADSubjectPriceBLL().Delete(p => p.QuotationsId == id);
                //new QuotationsBLL().Delete(q => q.Id == id);
                //tran.Complete();
                //BindData();
                        
                //    }
                //    catch (Exception ex)
                //    {
                //        Alert("删除失败："+ex.Message);
                //    }
                //}
                new ADSubjectPriceBLL().Delete(p => p.QuotationsId == id);
                new QuotationsBLL().Delete(q => q.Id == id);
                BindData();
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        ADSubjectPriceBLL adPriceBll = new ADSubjectPriceBLL();
        protected void btnExport_Click(object sender, EventArgs e)
        {
            string subjectName = string.Empty;
            var list = (from qu in CurrentContext.DbContext.Quotations
                       join subject in CurrentContext.DbContext.Subject
                       on qu.SubjectId equals subject.Id
                       where qu.SubjectId == subjectId
                       select new {
                           subject.SubjectName,
                           subject.SubjectNo,
                           subject.Contact,
                           qu.Id,
                           qu.Account,
                           qu.AdidasContact,
                           qu.Blongs,
                           qu.Category,
                           qu.Classification,
                           qu.OfferPrice,
                           qu.OtherPrice,
                           qu.OtherPriceRemark,
                           qu.TaxRate,
                           qu.AddDate,
                           qu.Remark
                       }).ToList();
            if (list.Any())
            {
                subjectName = list[0].SubjectName;
                CreateTb();
                List<int> qIdList = list.Select(s => s.Id).ToList();
                var priceList =adPriceBll.GetList(s => qIdList.Contains(s.QuotationsId??0));
                if (priceList.Any())
                {
                    var shareList = priceList.Where(s=>s.PriceType==1).ToList();
                    if (shareList.Any())
                    {
                        shareList.ForEach(s => {
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


                
                list.ForEach(s => {

                    DataRow dr = exportTB.NewRow();
                    dr["报价日期"] = s.AddDate != null ? DateTime.Parse(s.AddDate.ToString()).ToShortDateString() : "";
                    dr["项目编号"] = s.SubjectNo;
                    dr["项目名称"] = s.SubjectName;
                    dr["类别"] = s.Category;
                    dr["AD费用归属"] = s.Blongs;
                    dr["分类"] = s.Classification;
                    dr["卡乐项目负责人"] = s.Contact;
                    dr["Adidas负责人"] = s.AdidasContact;
                    dr["税率"] = s.TaxRate;
                    dr["报价金额"] = s.OfferPrice != null ? s.OfferPrice.ToString() : "";
                    dr["报价账户"] = s.Account;
                    dr["挂账金额"] = s.OtherPrice != null ? s.OtherPrice.ToString() : "";
                    dr["挂账说明"] = s.OtherPriceRemark;
                    dr["备注"] = s.Remark;

                    var priceList1 = adPriceBll.GetList(p=>p.QuotationsId==s.Id);
                    var shareList1 = priceList1.Where(p=>p.PriceType==1).ToList();
                    if (shareList1.Any())
                    {
                        shareList1.ForEach(sh => {
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
                OperateFile.ExportExcel(exportTB, "AD项目费用信息", subjectName);
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
               new DataColumn("报价金额",Type.GetType("System.String")),
               new DataColumn("挂账金额",Type.GetType("System.String")),
               new DataColumn("挂账说明",Type.GetType("System.String")),
               new DataColumn("税率",Type.GetType("System.String"))
            });
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("List1.aspx",false);
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object objOfferPrice = item.GetType().GetProperty("OfferPrice").GetValue(item, null);
                    decimal offerPrice = objOfferPrice != null ? decimal.Parse(objOfferPrice.ToString()) : 0;
                    object objOtherPrice = item.GetType().GetProperty("OtherPrice").GetValue(item, null);
                    decimal otherPrice = objOtherPrice != null ? decimal.Parse(objOtherPrice.ToString()) : 0;
                    ((Label)e.Row.FindControl("labTotal")).Text = (offerPrice + otherPrice).ToString("0.00");
                }
            }
        }
    }
}
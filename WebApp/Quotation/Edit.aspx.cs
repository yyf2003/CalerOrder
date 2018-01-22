using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using BLL;
using Common;
using Models;
using Newtonsoft.Json;

namespace WebApp.Quotation
{
    public partial class Edit : BasePage
    {
        public int subjectId;
        int quotationId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectid"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectid"]);
            }
            if (Request.QueryString["quotationid"] != null)
            {
                quotationId = int.Parse(Request.QueryString["quotationid"]);
            }
            if (!IsPostBack)
            {
                BindData();
                BindSubjectName();
            }
        }

        void BindData()
        {
            if (quotationId > 0)
            {
                QuotationsBLL QuotationsBll = new QuotationsBLL();
                ADSubjectPriceBLL priceBll = new ADSubjectPriceBLL();
                Quotations model = QuotationsBll.GetModel(quotationId);
                if (model != null)
                {
                    txtCategory.Text = model.Category;
                    txtBelongs.Text = model.Blongs;
                    txtClassification.Text = model.Classification;
                    txtAdidasContact.Text = model.AdidasContact;
                   
                    txtTaxRate.Text = model.TaxRate;
                    if (model.OfferPrice != null)
                        txtOfferPrice.Text = model.OfferPrice.ToString();
                    txtAccount.Text = model.Account;
                    if (model.OtherPrice != null && model.OtherPrice>0)
                        txtOtherPrice.Text = model.OtherPrice.ToString();
                    txtOtherPriceRemark.Text = model.OtherPriceRemark;
                    txtRemark.Text = model.Remark;

                    var priceList = priceBll.GetList(s => s.QuotationsId == model.Id);
                    if (priceList.Any())
                    {
                        var shareList = priceList.Where(s => s.PriceType == 1).ToList();
                        if (shareList.Any())
                        {
                            StringBuilder json1 = new StringBuilder();
                            shareList.ForEach(s => {
                                json1.Append("{\"PriceName\":\"" + s.PriceName + "\",\"Price\":\""+s.Price+"\"},");
                            });
                            hfSharePrice.Value = "["+json1.ToString().TrimEnd(',')+"]";
                        }
                        var bingList = priceList.Where(s => s.PriceType == 2).ToList();
                        if (bingList.Any())
                        {
                            StringBuilder json2 = new StringBuilder();
                            bingList.ForEach(s =>
                            {
                                json2.Append("{\"PriceName\":\"" + s.PriceName + "\",\"Price\":\"" + s.Price + "\"},");
                            });
                            hfBingPrice.Value = "[" + json2.ToString().TrimEnd(',') + "]";
                        }
                    }
                }
            }
        }

        void BindSubjectName()
        {
            Subject model = new SubjectBLL().GetModel(subjectId);
            if (model != null && model.BeginDate!=null)
            {
                int year = DateTime.Parse(model.BeginDate.ToString()).Year;
                int month =DateTime.Parse(model.BeginDate.ToString()).Month;
                var list = new SubjectBLL().GetList(s=>s.BeginDate.Value.Year==year && s.BeginDate.Value.Month==month);
                if (list.Any())
                {
                    //ddlSubjects.DataSource = list;
                    //ddlSubjects.DataTextField = "SubjectName";
                    //ddlSubjects.DataValueField = "SubjectName";
                    //ddlSubjects.DataBind();
                    //ddlSubjects.Items.Insert(0,new ListItem("--请选择--",""));
                }

            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            QuotationsBLL QuotationsBll = new QuotationsBLL();
            Quotations QuotationsModel;
            ADSubjectPriceBLL priceBll = new ADSubjectPriceBLL();
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    string Category = txtCategory.Text.Trim();
                    string Belongs = txtBelongs.Text.Trim();
                    string Classification = txtClassification.Text.Trim();
                    string AdidasContact = txtAdidasContact.Text.Trim();
                    string TaxRate = txtTaxRate.Text.Trim();
                    string OfferPrice = txtOfferPrice.Text.Trim();
                    string Account = txtAccount.Text.Trim();
                    string OtherPrice = txtOtherPrice.Text.Trim();
                    string OtherPriceRemark = txtOtherPriceRemark.Text.Trim();
                    string Remark = txtRemark.Text.Trim();
                    if (quotationId > 0)
                    {
                        QuotationsModel = QuotationsBll.GetModel(quotationId);
                        if (QuotationsModel == null)
                        {
                            QuotationsModel = new Quotations();
                            quotationId = 0;
                        }
                    }
                    else
                    {
                        QuotationsModel = new Quotations();
                    }
                    QuotationsModel.Account = Account;
                    QuotationsModel.AdidasContact = AdidasContact;
                    QuotationsModel.Blongs = Belongs;
                    QuotationsModel.Category = Category;
                    QuotationsModel.Classification = Classification;
                    QuotationsModel.OfferPrice = StringHelper.IsDecimal(OfferPrice);
                    QuotationsModel.Remark = Remark;
                    QuotationsModel.TaxRate = TaxRate;
                    QuotationsModel.OtherPrice = StringHelper.IsDecimal(OtherPrice);
                    QuotationsModel.OtherPriceRemark = OtherPriceRemark;
                    if (quotationId > 0)
                    {
                        QuotationsBll.Update(QuotationsModel);
                    }
                    else
                    {
                        QuotationsModel.SubjectId = subjectId;
                        QuotationsModel.AddDate = DateTime.Now;
                        QuotationsModel.AddUserId = CurrentUser.UserId;
                        QuotationsBll.Add(QuotationsModel);
                    }
                    SaveCategory(Category);
                    SaveBlongs(Belongs);
                    SaveClassification(Classification);
                    SaveTaxRate(TaxRate);
                    SaveAccount(Account);
                    priceBll.Delete(s => s.QuotationsId == QuotationsModel.Id);
                    if (!string.IsNullOrWhiteSpace(hfSharePrice.Value))
                    {
                        List<ADSubjectPrice> priceList = JsonConvert.DeserializeObject<List<ADSubjectPrice>>(hfSharePrice.Value);
                        if (priceList.Any())
                        {

                            priceList.ForEach(s =>
                            {
                                s.QuotationsId = QuotationsModel.Id;
                                priceBll.Add(s);
                            });

                        }
                    }
                    if (!string.IsNullOrWhiteSpace(hfBingPrice.Value))
                    {
                        List<ADSubjectPrice> priceList = JsonConvert.DeserializeObject<List<ADSubjectPrice>>(hfBingPrice.Value);
                        if (priceList.Any())
                        {

                            priceList.ForEach(s =>
                            {
                                s.QuotationsId = QuotationsModel.Id;
                                priceBll.Add(s);
                            });

                        }
                    }
                    tran.Complete();

                    ExcuteJs("Finish");
                }
                catch (Exception ex)
                {
                    //Alert("提交失败："+ex.Message);
                    ExcuteJs("Fail", ex.Message);
                }
            }
        }

        void SaveCategory(string name)
        {
            QuotationCategoryBLL bll = new QuotationCategoryBLL();
            string name1 = StringHelper.ReplaceSpace(name).ToLower();
            var model = bll.GetList(s => s.CategoryName.Replace(" ", "").ToLower() == name1).FirstOrDefault();
            if (model == null)
            {
                QuotationCategory newModel = new QuotationCategory();
                newModel.CategoryName = name;
                bll.Add(newModel);
            }
        }

        void SaveBlongs(string name)
        {
            QuotationBlongsBLL bll = new QuotationBlongsBLL();
            string name1 = StringHelper.ReplaceSpace(name).ToLower();
            var model = bll.GetList(s => s.BlongsName.Replace(" ", "").ToLower() == name1).FirstOrDefault();
            if (model == null)
            {
                QuotationBlongs newModel = new QuotationBlongs();
                newModel.BlongsName = name;
                bll.Add(newModel);
            }
        }

        void SaveClassification(string name)
        {
            QuotationClassificationBLL bll = new QuotationClassificationBLL();
            string name1 = StringHelper.ReplaceSpace(name).ToLower();
            var model = bll.GetList(s => s.ClassificationName.Replace(" ", "").ToLower() == name1).FirstOrDefault();
            if (model == null)
            {
                QuotationClassification newModel = new QuotationClassification();
                newModel.ClassificationName = name;
                bll.Add(newModel);
            }
        }

        void SaveTaxRate(string name)
        {
            QuotationTaxRateBLL bll = new QuotationTaxRateBLL();
            string name1 = StringHelper.ReplaceSpace(name).ToLower();
            var model = bll.GetList(s => s.TaxRateName.Replace(" ", "").ToLower() == name1).FirstOrDefault();
            if (model == null)
            {
                QuotationTaxRate newModel = new QuotationTaxRate();
                newModel.TaxRateName = name;
                bll.Add(newModel);
            }
        }

        void SaveAccount(string name)
        {
            QuotationAccountBLL bll = new QuotationAccountBLL();
            string name1 = StringHelper.ReplaceSpace(name).ToLower();
            var model = bll.GetList(s => s.AccountName.Replace(" ", "").ToLower() == name1).FirstOrDefault();
            if (model == null)
            {
                QuotationAccount newModel = new QuotationAccount();
                newModel.AccountName = name;
                bll.Add(newModel);
            }
        }
    }
}
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
    public partial class Invoice : BasePage
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
                BindData();
            }
        }

        ADInvoiceBLL bll = new ADInvoiceBLL();
        void BindData()
        {
            var model = bll.GetList(s => s.SubjectId == subjectId).FirstOrDefault();
            if (model != null)
            {
                txtInvoiceMoney.Text = model.InvoiceMoney != null ? model.InvoiceMoney.ToString() : "";
                txtInvoiceNumber.Text = model.InvoiceNumber;
                if (model.InvoiceDate != null)
                    txtInvoiceDate.Text = DateTime.Parse(model.InvoiceDate.ToString()).ToShortDateString();
                if (model.ExpectReceiveDate != null)
                {
                    txtExpectReceiveDate.Text = DateTime.Parse(model.ExpectReceiveDate.ToString()).ToShortDateString();
                    //hfExpectReceiveDate.Value = txtExpectReceiveDate.Text;
                }
                txtRemark.Text = model.Remark;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string money = txtInvoiceMoney.Text.Trim();
            string number = txtInvoiceNumber.Text.Trim();
            string invoiceDate = txtInvoiceDate.Text;
            string expectReceiveDate = txtExpectReceiveDate.Text;
            string remark = txtRemark.Text;
            if (string.IsNullOrWhiteSpace(money))
            {
                Alert("请输入开票金额");
                return;
            }
            else if (!StringHelper.IsDecimalVal(money))
            {
                Alert("开票金额必须是数字");
                return;
            }
            if (string.IsNullOrWhiteSpace(number))
            {
                Alert("请输入发票号码");
                return;
            }
            if (string.IsNullOrWhiteSpace(invoiceDate))
            {
                Alert("请输入开票时间");
                return;
            }
            else if (!StringHelper.IsDateTime(invoiceDate))
            {
                Alert("开票时间格式不正确");
                return;
            }
            if (string.IsNullOrWhiteSpace(expectReceiveDate))
            {
                Alert("请输入预计到款时间");
                return;
            }
            else if (!StringHelper.IsDateTime(expectReceiveDate))
            {
                Alert("预计到款时间格式不正确");
                return;
            }
            var model = bll.GetList(s => s.SubjectId == subjectId).FirstOrDefault();
            if (model != null)
            {
                model.ExpectReceiveDate = DateTime.Parse(expectReceiveDate);
                model.InvoiceDate = DateTime.Parse(invoiceDate);
                model.InvoiceMoney = decimal.Parse(money);
                model.InvoiceNumber = number;
                model.Remark = remark;
                bll.Update(model);
            }
            else
            {
                ADInvoice newModel = new ADInvoice();
                newModel.ExpectReceiveDate = DateTime.Parse(expectReceiveDate);
                newModel.InvoiceDate = DateTime.Parse(invoiceDate);
                newModel.InvoiceMoney = decimal.Parse(money);
                newModel.InvoiceNumber = number;
                newModel.Remark = remark;
                newModel.SubjectId = subjectId;
                bll.Add(newModel);
            }
            ExcuteJs("Finish");
        }
    }
}
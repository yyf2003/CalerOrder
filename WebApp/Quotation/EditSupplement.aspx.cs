using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;

namespace WebApp.Quotation
{
    public partial class EditSupplement : BasePage
    {
        int subjectId;
        
        AccountCheckSupplementBLL bll = new AccountCheckSupplementBLL();
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

        void BindData()
        {
            if (subjectId > 0)
            {
                AccountCheckSupplement model = bll.GetList(s=>s.SubjectId==subjectId).FirstOrDefault();
                if (model != null)
                {
                    txtCRName.Text = model.CRName;
                    txtCRNumber.Text = model.CRNumber;
                    txtPONumber.Text = model.PONumber;
                    txtKPI.Text = model.KPI;
                    txtRemark.Text = model.Remark;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            AccountCheckSupplement model;
            bool isExist = false;
            string CRName = txtCRName.Text.Trim();
            string CRNumber = txtCRNumber.Text.Trim();
            string PONumber = txtPONumber.Text.Trim();
            string KPI = txtKPI.Text.Trim();
            string Remark = txtRemark.Text.Trim();
            model = bll.GetList(s => s.SubjectId == subjectId).FirstOrDefault();
            if (model != null)
            {
                isExist = true;
                
            }
            else
                model = new AccountCheckSupplement();
            model.CRName = CRName;
            model.CRNumber = CRNumber;
            model.KPI = KPI;
            model.PONumber = PONumber;
            model.Remark = Remark;
            try
            {
                if (isExist)
                {
                    bll.Update(model);
                }
                else
                {
                    model.AddDate = DateTime.Now;
                    model.AddUserId = CurrentUser.UserId;
                    model.SubjectId = subjectId;
                    bll.Add(model);
                }
                ExcuteJs("Finish");
            }
            catch (Exception ex)
            {
                ExcuteJs("Fail", ex.Message);
            }
        }
    }
}
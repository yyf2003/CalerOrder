using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using BLL;
using Models;
using Common;
using System.Text;

namespace WebApp.OutsourcingOrder.PayRecord
{
    public partial class AddPayment : BasePage
    {
        string guidanceId;
        int outsourceId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = Request.QueryString["guidanceId"];
            }
            if (Request.QueryString["outsourceId"] != null)
            {
                outsourceId = int.Parse(Request.QueryString["outsourceId"]);
            }
            if (!IsPostBack)
            {
                txtPayDate.Text = DateTime.Now.ToShortDateString();
                BindData();
                BindSubjectList();
            }
        }

        void BindData()
        {
            Session["PayRecordOrderList"] = null;
            Session["PayRecordSubjectList"] = null;
            Company companyModel = new CompanyBLL().GetModel(outsourceId);
            if (companyModel != null)
            {
                labOutsourceName.Text = companyModel.CompanyName;
            }
            //SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(guidanceId);
            //if (guidanceModel != null)
            //{
            //    labGuidanceName.Text = guidanceModel.ItemName;
            //}
            List<int> guidanceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId,',').Distinct().OrderBy(s=>s).ToList();
            }
            if (guidanceIdList.Any())
            {
                List<string> guidanceNameList = new SubjectGuidanceBLL().GetList(s => guidanceIdList.Contains(s.ItemId)).Select(s=>s.ItemName).ToList();
                labGuidanceName.Text = StringHelper.ListToString(guidanceNameList,"，");
                
            }
            var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                            //join subject1 in CurrentContext.DbContext.Subject
                            //on order.SubjectId equals subject1.Id into temp
                            //from subject in temp.DefaultIfEmpty()
                            where guidanceIdList.Contains(order.GuidanceId??0)
                            && order.OutsourceId == outsourceId
                            && (order.IsDelete == null || order.IsDelete == false)
                            select order).ToList();
            if (orderList.Any())
            {
                Session["PayRecordOrderList"] = orderList;
                List<int> popSubjectIdList = orderList.Where(s => (s.SubjectId ?? 0) > 0).Select(s => s.SubjectId ?? 0).Distinct().ToList();
                List<int> activityInstallPricSubjectIdList = orderList.Where(s => (s.SubjectId ?? 0) == 0 && s.OrderType == (int)OrderTypeEnum.安装费).Select(s => s.BelongSubjectId ?? 0).Distinct().ToList();
                popSubjectIdList = popSubjectIdList.Union(activityInstallPricSubjectIdList).ToList();
                //List<Subject> subjectList = orderList.Where(s=>s.subject!=null).Select(s => s.subject).Distinct().ToList();
                List<Subject> subjectList = new SubjectBLL().GetList(s => popSubjectIdList.Contains(s.Id));
                Session["PayRecordSubjectList"] = subjectList;
                var categoryList = (from subject in subjectList
                                    join category1 in CurrentContext.DbContext.ADSubjectCategory
                                    on subject.SubjectCategoryId equals category1.Id into categoryTemp
                                    from category in categoryTemp.DefaultIfEmpty()
                                    select category).Distinct().ToList();
                
                bool isNull = false;
                categoryList.ForEach(s =>
                {
                    if (s != null)
                    {
                        ListItem li = new ListItem();
                        li.Text = s.CategoryName + "&nbsp;&nbsp;";
                        li.Value = s.Id.ToString();
                        cblSubjectCategory.Items.Add(li);
                    }
                    else
                        isNull = true;
                });
                if (isNull)
                {
                    cblSubjectCategory.Items.Add(new ListItem("空", "0"));
                }
                //subjectList.OrderBy(s => s.GuidanceId).ToList().ForEach(s =>
                //{
                //    ListItem li = new ListItem();
                //    li.Text = s.SubjectName + "&nbsp;&nbsp;";
                //    li.Value = s.Id.ToString();
                //    cblSubject.Items.Add(li);
                //});
                decimal totalPrice = 0;
                var payRecordList = new OutsourcePayRecordBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && s.OutsourceId == outsourceId);
                //Dictionary<guidanceId, pay>
                Dictionary<int, decimal> payDic = new Dictionary<int, decimal>();
                guidanceIdList.ForEach(gid => {
                    decimal oneGidShouldPay = 0;
                    var popList = orderList.Where(order =>order.GuidanceId==gid && order.OrderType == (int)OrderTypeEnum.POP).ToList();
                    popList.ForEach(order =>
                    {
                        oneGidShouldPay += (order.TotalPrice ?? 0);
                    });
                    var priceOrderList = orderList.Where(order => order.GuidanceId == gid && order.OrderType != (int)OrderTypeEnum.POP).ToList();
                    priceOrderList.ForEach(order =>
                    {
                        oneGidShouldPay += ((order.Quantity ?? 1) * (order.PayOrderPrice ?? 0));
                    });
                    if (oneGidShouldPay > 0)
                    {
                        oneGidShouldPay = Math.Round(oneGidShouldPay, 2);
                    }
                    var payRecord = payRecordList.Where(s=>s.GuidanceId==gid).ToList();
                    if (payRecord.Any())
                    {
                        decimal pay = payRecord.Sum(s => s.PayAmount ?? 0);
                        oneGidShouldPay = oneGidShouldPay - pay;
                    }
                    payDic.Add(gid, oneGidShouldPay);
                    totalPrice += oneGidShouldPay;
                });
                Session["totalShouldPay"] = payDic;
                
                
                if (totalPrice <= 0)
                {
                    btnSubmit.Enabled = false;
                }
                labShouldPay.Text = totalPrice.ToString();
            }
        }

        void BindSubjectList()
        {
            //cblSubject.Items.Clear();
            List<int> categoryIdList = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                {
                    categoryIdList.Add(int.Parse(li.Value));
                }
            }
            List<Subject> subjectList = new List<Subject>();
            if (Session["PayRecordSubjectList"] != null)
            {
                subjectList = Session["PayRecordSubjectList"] as List<Subject>;
            }
            if (subjectList.Any())
            {
                if (categoryIdList.Any())
                {
                    if (categoryIdList.Contains(0))
                    {
                        categoryIdList.Remove(0);
                        if (categoryIdList.Any())
                        {
                            subjectList = subjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0) || (s.SubjectCategoryId == 0 || s.SubjectCategoryId == null)).ToList();
                        }
                        else
                        {
                            subjectList = subjectList.Where(s => (s.SubjectCategoryId == 0 || s.SubjectCategoryId == null)).ToList();
                        }
                    }
                    else
                    {
                        subjectList = subjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0)).ToList();
                    }
                }
                StringBuilder subjectSb = new StringBuilder();
                subjectList.OrderBy(s=>s.GuidanceId).ToList().ForEach(s => {
                    //ListItem li = new ListItem();
                    //li.Text = s.SubjectName + "&nbsp;&nbsp;";
                    //li.Value = s.Id.ToString();
                    //cblSubject.Items.Add(li);
                    subjectSb.AppendFormat("<div style='float:left; margin-right:10px;'>{0}，</div>",s.SubjectName);

                });
                labSubject.Text = subjectSb.ToString();
            }
        }

        void BindShouldPay()
        {
            List<int> categoryIdList = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                {
                    categoryIdList.Add(int.Parse(li.Value));
                }
            }
            List<OutsourceOrderDetail> orderList = new List<OutsourceOrderDetail>();
            if (Session["PayRecordOrderList"] != null)
            {
                orderList = Session["PayRecordOrderList"] as List<OutsourceOrderDetail>;
            }
            List<Subject> subjectList = new List<Subject>();
            if (Session["PayRecordSubjectList"] != null)
            {
                subjectList = Session["PayRecordSubjectList"] as List<Subject>;
            }
            if (orderList.Any() && subjectList.Any())
            {
                if (categoryIdList.Any())
                {
                    if (categoryIdList.Contains(0))
                    {
                        categoryIdList.Remove(0);
                        if (categoryIdList.Any())
                        {
                            subjectList = subjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0) || (s.SubjectCategoryId == 0 || s.SubjectCategoryId == null)).ToList();
                        }
                        else
                        {
                            subjectList = subjectList.Where(s => (s.SubjectCategoryId == 0 || s.SubjectCategoryId == null)).ToList();
                        }
                    }
                    else
                    {
                        subjectList = subjectList.Where(s => categoryIdList.Contains(s.SubjectCategoryId ?? 0)).ToList();
                    }
                }
                List<int> subjectIdList = subjectList.Select(s=>s.Id).ToList();
                orderList = orderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0) || subjectIdList.Contains(s.BelongSubjectId ?? 0)).ToList();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<int> guidanceIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(guidanceId))
            {
                guidanceIdList = StringHelper.ToIntList(guidanceId,',').Distinct().OrderBy(s=>s).ToList();
            }
            //decimal shouldPay = 0;
            //if (!string.IsNullOrWhiteSpace(labShouldPay.Text))
            //{
            //    shouldPay = StringHelper.IsDecimal(labShouldPay.Text);
            //}
            string payTxt = txtPay.Text.Trim();
            decimal pay = StringHelper.IsDecimal(payTxt);
            DateTime payDate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(txtPayDate.Text.Trim()) && StringHelper.IsDateTime(txtPayDate.Text.Trim()))
            {
                payDate = DateTime.Parse(txtPayDate.Text.Trim());
            }
            Dictionary<int, decimal> dicShouldPay = new Dictionary<int, decimal>();
            if (Session["totalShouldPay"] != null)
            {
                dicShouldPay = Session["totalShouldPay"] as Dictionary<int, decimal>;
            }
            OutsourcePayRecordBLL payBll = new OutsourcePayRecordBLL();
            int payGuidanceCount = 0;
            foreach (int gid in guidanceIdList)
            { 
                decimal oneGidShouldPay=dicShouldPay[gid];
                decimal payAmount = 0;
                if (pay > 0)
                {
                    if (pay > oneGidShouldPay)
                    {
                        payAmount = oneGidShouldPay;
                        pay = pay - oneGidShouldPay;
                    }
                    else
                    {
                        payAmount = pay;
                        pay = 0;
                    }
                    if (payAmount > 0)
                    {
                        OutsourcePayRecord model = new OutsourcePayRecord();
                        model.AddDate = DateTime.Now;
                        model.AddUserId = CurrentUser.UserId;
                        model.GuidanceId = gid;
                        model.OutsourceId = outsourceId;
                        model.PayAmount = payAmount;
                        model.PayDate = payDate;
                        model.Remark = txtRemark.Text.Trim();
                        payBll.Add(model);
                    }
                    payGuidanceCount++;
                    
                }
                else
                    break;
            }
            if (payGuidanceCount>0)
               ExcuteJs("submitSuccess");
        }

        protected void lbShowSubject_Click(object sender, EventArgs e)
        {
            if (PanelSubject.Visible)
            {
                PanelSubject.Visible = false;
                lbShowSubject.Text = "展开";
            }
            else
            {
                PanelSubject.Visible = true;
                lbShowSubject.Text = "收起";
            }
        }

        protected void cblSubjectCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectList();
        }

        protected void cbAllSubject_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
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
            }
        }

        void BindData()
        {
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
                labGuidanceName.Text = StringHelper.ListToString(guidanceNameList);
                
            }
            var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                            join subject1 in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject1.Id into temp
                            from subject in temp.DefaultIfEmpty()
                            where guidanceIdList.Contains(order.GuidanceId??0)
                            && order.OutsourceId == outsourceId
                            && (order.IsDelete == null || order.IsDelete == false)
                            select new {
                                order,
                                subject
                            }).ToList();
            if (orderList.Any())
            {
                List<Subject> subjectList = orderList.Where(s=>s.subject!=null).Select(s => s.subject).Distinct().ToList();
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
                //subjectList.ForEach(s => {
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
                    var popList = orderList.Where(order =>order.order.GuidanceId==gid && order.order.OrderType == (int)OrderTypeEnum.POP).ToList();
                    popList.ForEach(order =>
                    {
                        oneGidShouldPay += (order.order.TotalPrice ?? 0);
                    });
                    var priceOrderList = orderList.Where(order => order.order.GuidanceId == gid && order.order.OrderType != (int)OrderTypeEnum.POP).ToList();
                    priceOrderList.ForEach(order =>
                    {
                        oneGidShouldPay += ((order.order.Quantity ?? 1) * (order.order.PayOrderPrice ?? 0));
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
                    OutsourcePayRecord model = new OutsourcePayRecord();
                    model.AddDate = DateTime.Now;
                    model.AddUserId = CurrentUser.UserId;
                    model.GuidanceId = gid;
                    model.OutsourceId = outsourceId;
                    model.PayAmount = payAmount;
                    model.PayDate = payDate;
                    model.Remark = txtRemark.Text.Trim();
                    //StringBuilder subjectIdsSb = new StringBuilder();
                    //foreach (ListItem li in cblSubject.Items)
                    //{
                    //    if (li.Selected)
                    //    {
                    //        subjectIdsSb.Append(li.Value);
                    //        subjectIdsSb.Append(",");
                    //    }
                    //}
                    //if (subjectIdsSb.Length > 0)
                    //{
                    //    model.SubjectIds = subjectIdsSb.ToString().TrimEnd(',');
                    //}
                    payBll.Add(model);
                    payGuidanceCount++;
                }
                else
                    break;
            }
            if (payGuidanceCount>0)
               ExcuteJs("submitSuccess");
        }
    }
}
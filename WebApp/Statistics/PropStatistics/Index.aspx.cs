using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.Statistics.PropStatistics
{
    public partial class Index : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                ChangeSearchType();
                //BindGuidance();
                BindOutsource();
                BindSubject();
            }
        }

        void ClearSession()
        {
            Session["PropGuidanceStatistics"] = null;
            Session["PropSubjectStatistics"] = null;
            Session["PropOrderStatistics"] = null;
            Session["PropOutsourceOrderStatistics"] = null;
            guidanceSelectAllDiv.Style.Add("display", "none");
        }

        void ChangeSearchType()
        {
            if (rbOnGuidanceSearch.Checked)
            {
                txtSubjectBegin.Text = "";
                txtSubjectEnd.Text = "";
                BindGuidance();
                btnGetProject.Enabled = false;
                lbUp.Enabled = true;
                lbDown.Enabled = true;
               
            }
            else if (rbOnOrderSubjectSearch.Checked)
            {
                btnGetProject.Enabled = true;
                lbUp.Enabled = false;
                lbDown.Enabled = false;
                Session["PropGuidanceStatistics"] = null;
                Session["PropSubjectStatistics"] = null;
                Session["PropOrderStatistics"] = null;
                Session["PropOutsourceOrderStatistics"] = null;
            }
            
        }

        void BindGuidance()
        {
            ClearSession();
            cblPropGuidanceList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from subject in CurrentContext.DbContext.Subject
                                join guidance in CurrentContext.DbContext.SubjectGuidance
                                on subject.GuidanceId equals guidance.ItemId
                                where (guidance.IsDelete == null || guidance.IsDelete == false)
                                && (guidance.AddType ?? 1) == (int)GuidanceAddTypeEnum.Prop
                                && guidance.CustomerId == customerId
                                && (subject.ApproveState==1)
                                && (subject.IsDelete==null || subject.IsDelete==false)
                                select new { guidance, subject }).ToList();
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
                List<int> guidanceIdList = list.Select(s => s.guidance.ItemId).ToList();
                //List<Subject> subjectList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1).OrderBy(s=>s.GuidanceId).ToList();
                List<Subject> subjectList = list.Select(s=>s.subject).ToList();
                List<SubjectGuidance> guidanceList = list.Select(s => s.guidance).Distinct().ToList();
                Session["PropGuidanceStatistics"] = guidanceList;
                Session["PropSubjectStatistics"] = subjectList;
                List<int> subjectIdList = subjectList.Select(s => s.Id).ToList();
                var orderList = new PropOrderDetailBLL().GetList(s => subjectIdList.Contains(s.SubjectId??0) && (s.IsDelete==null || s.IsDelete==false));
                var outsourceOrderList = new PropOutsourceOrderDetailBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0) && (s.IsDelete == null || s.IsDelete == false));
                Session["PropOrderStatistics"] = orderList;
                Session["PropOutsourceOrderStatistics"] = outsourceOrderList;
                guidanceList.ForEach(s => {
                    ListItem li = new ListItem();
                    li.Value = s.ItemId.ToString();
                    li.Text = s.ItemName + "&nbsp;&nbsp;";
                    cblPropGuidanceList.Items.Add(li);
                });
                guidanceSelectAllDiv.Style.Add("display", "");
            }
            Panel_EmptyPropGuidance.Visible = !list.Any();

        }

        

        void BindOutsource()
        {
            cbAllOutsource.Checked = false;
            cblOutsource.Items.Clear();
            List<int> guidanceIdList = GetGuidanceSelected();

            List<PropOutsourceOrderDetail> outsourceOrderList = new List<PropOutsourceOrderDetail>();
            if (Session["PropOutsourceOrderStatistics"] != null)
            {
                outsourceOrderList = Session["PropOutsourceOrderStatistics"] as List<PropOutsourceOrderDetail>;
            }
            if (outsourceOrderList.Any())
            {
                if (guidanceIdList.Any())
                {
                    outsourceOrderList = outsourceOrderList.Where(s => guidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();
                }
                List<string> outsourceList = outsourceOrderList.Select(s => s.OutsourceName).Distinct().OrderBy(s => s).ToList();
                outsourceList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblOutsource.Items.Add(li);
                });
                OutsourceSelectAllDiv.Style.Add("display", "");
            }
            else
            {
                OutsourceSelectAllDiv.Style.Add("display", "none");
            }
        }

        void BindSubject()
        {
            cbAllPropSubject.Checked = false;
            cblPropSubject.Items.Clear();
            List<Subject> subjectList = new List<Subject>();
            List<string> outsourceList = GetOutsourceSelected();
            if (Session["PropSubjectStatistics"] != null)
            {
                subjectList = Session["PropSubjectStatistics"] as List<Subject>;
            }
            if (outsourceList.Any())
            {
                List<PropOutsourceOrderDetail> outsourceOrderList = new List<PropOutsourceOrderDetail>();
                if (Session["PropOutsourceOrderStatistics"] != null)
                {
                    outsourceOrderList = Session["PropOutsourceOrderStatistics"] as List<PropOutsourceOrderDetail>;
                }
                if (outsourceOrderList.Any())
                {
                    outsourceOrderList = outsourceOrderList.Where(s => outsourceList.Contains(s.OutsourceName)).ToList();
                    List<int> sidList = outsourceOrderList.Select(s => s.SubjectId ?? 0).Distinct().ToList();
                    subjectList = subjectList.Where(s => sidList.Contains(s.Id)).ToList();
                }
                else
                {
                    subjectList = new List<Subject>();
                }
            }
            if (subjectList.Any())
            {
                subjectList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.SubjectName + "&nbsp;&nbsp;";
                    cblPropSubject.Items.Add(li);
                });
                SubjectSelectAllDiv.Style.Add("display", "");
            }
            else
            {
                SubjectSelectAllDiv.Style.Add("display", "none");
            }
        }

        void BindGuidanceByDateSearch()
        {
            ClearSession();
            string begin = txtSubjectBegin.Text.Trim();
            string end = txtSubjectEnd.Text.Trim();
            cblPropGuidanceList.Items.Clear();
            if (!string.IsNullOrWhiteSpace(begin) && !string.IsNullOrWhiteSpace(end) && StringHelper.IsDateTime(begin) && StringHelper.IsDateTime(end))
            {
                DateTime beginDate = DateTime.Parse(begin);
                DateTime endDate = DateTime.Parse(end).AddDays(1);

                var list = (from order in CurrentContext.DbContext.PropOrderDetail
                            join subject in CurrentContext.DbContext.Subject
                            on order.SubjectId equals subject.Id
                            where subject.ApproveState == 1
                            && (subject.IsDelete == null || subject.IsDelete == false)
                            && (order.AddDate >= beginDate && order.AddDate < endDate)
                            select new { order, subject }).ToList();
                if (list.Any())
                {
                    List<int> guidanceIdList = list.Select(s => s.order.GuidanceId ?? 0).Distinct().ToList();
                    List<Subject> subjectList = list.Select(s => s.subject).Distinct().ToList();
                    List<int> subjectIdList = subjectList.Select(s => s.Id).ToList();
                    List<SubjectGuidance> guidanceList = new SubjectGuidanceBLL().GetList(s => guidanceIdList.Contains(s.ItemId));
                    List<PropOrderDetail> orderList = list.Select(s => s.order).ToList();
                    List<PropOutsourceOrderDetail> outsourceOrderList = new PropOutsourceOrderDetailBLL().GetList(s => subjectIdList.Contains(s.SubjectId ?? 0));
                    Session["PropGuidanceStatistics"] = guidanceList;
                    Session["PropSubjectStatistics"] = subjectList;
                    Session["PropOrderStatistics"] = orderList;
                    Session["PropOutsourceOrderStatistics"] = outsourceOrderList;
                    guidanceList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.ItemId.ToString();
                        li.Text = s.ItemName + "&nbsp;&nbsp;";
                        cblPropGuidanceList.Items.Add(li);
                    });
                    guidanceSelectAllDiv.Style.Add("display", "");
                }
                
            }
            BindOutsource();
            BindSubject();
        }

        List<int> GetGuidanceSelected()
        {
            List<int> list = new List<int>();
            foreach (ListItem li in cblPropGuidanceList.Items)
            {
                if (li.Selected)
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            return list;
        }

        List<string> GetOutsourceSelected()
        {
            List<string> list = new List<string>();
            foreach (ListItem li in cblOutsource.Items)
            {
                if (li.Selected)
                {
                    list.Add(li.Value);
                }
            }
            return list;
        }

        List<int> GetSubjectSelected()
        {
            List<int> list = new List<int>();
            foreach (ListItem li in cblPropSubject.Items)
            {
                if (li.Selected)
                {
                    list.Add(int.Parse(li.Value));
                }
            }
            return list;
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            rbOnGuidanceSearch.Checked=true;
            ChangeSearchType();
            BindOutsource();
            BindSubject();

        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {

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
                BindOutsource();
                BindSubject();
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
                BindOutsource();
                BindSubject();
            }
        }

        protected void btnGetProject_Click(object sender, EventArgs e)
        {
            BindGuidanceByDateSearch();
        }

        protected void cblPropGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindOutsource();
            BindSubject();
        }

        protected void cblOutsource_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
        }

        protected void rbOnOrderSubjectSearch_CheckedChanged(object sender, EventArgs e)
        {
            ChangeSearchType();
        }

        protected void rbOnGuidanceSearch_CheckedChanged(object sender, EventArgs e)
        {
            ChangeSearchType();
        }

        protected void btnCheckAllPropGuidance_Click(object sender, EventArgs e)
        {
            BindOutsource();
            BindSubject();
        }

        protected void btnCheckAllOutsource_Click(object sender, EventArgs e)
        {
            BindSubject();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        void Search()
        {
            labPayPrice.Text = "0";
            labReceivePrice.Text = "0";
            List<int> guidanceIdList = GetGuidanceSelected();
            List<string> outsourceList = GetOutsourceSelected();
            List<int> subjectIdList = GetSubjectSelected();
            List<PropOrderDetail> orderList = new List<PropOrderDetail>();
            List<PropOutsourceOrderDetail> outsourceOrderList = new List<PropOutsourceOrderDetail>();
            if (Session["PropOrderStatistics"] != null)
            {
                orderList = Session["PropOrderStatistics"] as List<PropOrderDetail>;
            }
            if (Session["PropOutsourceOrderStatistics"] != null)
            {
                outsourceOrderList = Session["PropOutsourceOrderStatistics"] as List<PropOutsourceOrderDetail>;
            }
            if (guidanceIdList.Any())
            {
                orderList = orderList.Where(s => guidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();
                outsourceOrderList = outsourceOrderList.Where(s => guidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();
            }
            if (outsourceList.Any())
            {
                outsourceOrderList = outsourceOrderList.Where(s => outsourceList.Contains(s.OutsourceName)).ToList();
            }
            if (subjectIdList.Any())
            {
                orderList = orderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
                outsourceOrderList = outsourceOrderList.Where(s => subjectIdList.Contains(s.SubjectId ?? 0)).ToList();
            }
            if (orderList.Any())
            {
                decimal receivePrice = orderList.Sum(s => ((s.UnitPrice ?? 0) * (s.Quantity ?? 1)));
                if (receivePrice > 0)
                {
                    receivePrice = Math.Round(receivePrice, 2);
                    labReceivePrice.Text = receivePrice + " 元";
                    labReceivePrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                    labReceivePrice.Attributes.Add("name", "spanReceivePrice");
                }
              
            }
            if (outsourceOrderList.Any())
            {
                decimal payPrice = outsourceOrderList.Sum(s => ((s.UnitPrice ?? 0) * (s.Quantity ?? 1)));
                if (payPrice > 0)
                {
                    payPrice = Math.Round(payPrice, 2);
                    labPayPrice.Text = payPrice + " 元";
                    labPayPrice.Attributes.Add("style", "text-decoration:underline; cursor:pointer;color:blue;");
                    labPayPrice.Attributes.Add("name", "spanOutsourcePrice");
                }
                
            }
        }
    }
}
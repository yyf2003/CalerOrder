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

namespace WebApp.QuoteOrderManager
{
    public partial class QuoteOperate : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                BindGuidance();
                BindRegion();
                GetQuoteList();
            }
        }

        void BindGuidance()
        {
            cblGuidanceList.Items.Clear();
            cblSubjectCategory.Items.Clear();
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
        }

        void BindRegion()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<string> myRegion = GetResponsibleRegion;
            if (myRegion.Any())
            {
                myRegion.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    cblRegion.Items.Add(li);
                });
            }
            else
                BindRegionByCustomer1(customerId, ref cblRegion);
        }

        void BindSubjectCategory()
        {
            cblSubjectCategory.Items.Clear();
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items) {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join user in CurrentContext.DbContext.UserInfo
                             on subject.AddUserId equals user.UserId
                             join category1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals category1.Id into categoryTemp
                             from category in categoryTemp.DefaultIfEmpty()
                             where guidanceIdList.Contains(order.GuidanceId??0)
                             select new
                             {
                                 order,
                                 category
                             }).ToList();

            if (orderList.Any())
            {
               
                if (orderList.Any())
                {
                    var categoryList = orderList.Select(s => s.category).Distinct().ToList();
                   
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
                }
            }
        }

        protected void btnCheckAllGuidance_Click(object sender, EventArgs e)
        {
            BindSubjectCategory();
        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectCategory();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text.Trim()))
            {
                BindGuidance();
                GetQuoteList();
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
                //BindSubjectCategory();
                GetQuoteList();
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
                //BindSubjectCategory();
                GetQuoteList();
            }
        }

        void GetQuoteList()
        {
            string guidanceMonth = txtGuidanceMonth.Text.Trim();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                var list = new QuotationItemBLL().GetList(s => s.CustomerId == customerId && s.GuidanceYear == year && s.GuidanceMonth == month && (s.IsDelete==null || s.IsDelete==false));
                gvList.DataSource = list;
                gvList.DataBind();
            }
        }

        protected void gvList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

       
    }
}
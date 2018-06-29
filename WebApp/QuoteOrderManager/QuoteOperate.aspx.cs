﻿using System;
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
            cblSubjectName.Items.Clear();
            cbAllSubject.Checked = false;
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
            var orderList = (from order in CurrentContext.DbContext.QuoteOrderDetail
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join category1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals category1.Id into categoryTemp
                             from category in categoryTemp.DefaultIfEmpty()
                             where guidanceIdList.Contains(order.GuidanceId??0)
                             && (order.QuoteItemId??0)==0
                             select new
                             {
                                 order,
                                 category
                             }).ToList();

            if (orderList.Any())
            {
                var categoryList = orderList.Select(s => s.category).Distinct().OrderBy(s=>s.CategoryName).ToList();

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

        void BindSubject()
        {
            cblSubjectName.Items.Clear();
            cbAllSubject.Checked = false;
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            List<int> subjectCategoryList = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                {
                    subjectCategoryList.Add(int.Parse(li.Value));
                }
            }
            //获取已提交报价的订单
            List<int> quoteSubjectIdList = new List<int>();
            var quoteOrderList = new QuoteOrderDetailBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId??0) && (s.QuoteItemId??0)>0);
            if (quoteOrderList.Any())
            {
                quoteSubjectIdList = quoteOrderList.Select(s => s.SubjectId ?? 0).Distinct().ToList();
            }
            var subjectList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && s.ApproveState == 1 && (s.IsDelete == null || s.IsDelete == false) && ((s.HandMakeSubjectId ?? 0) == 0 && (s.SupplementRegion ?? "") == "") && !quoteSubjectIdList.Contains(s.Id));
            if (subjectCategoryList.Any())
            {
                if (subjectCategoryList.Contains(0))
                {
                    subjectCategoryList.Remove(0);
                    if (subjectCategoryList.Any())
                    {
                        subjectList = subjectList.Where(s => subjectCategoryList.Contains(s.SubjectCategoryId ?? 0) || (s.SubjectCategoryId == null || s.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        subjectList = subjectList.Where(s => s.SubjectCategoryId == null || s.SubjectCategoryId == 0).ToList();
                }
                else
                    subjectList = subjectList.Where(s => subjectCategoryList.Contains(s.SubjectCategoryId ?? 0)).ToList();
            }

            if (subjectList.Any())
            {
                //var subjectList = orderList.Select(s => s.subject).Distinct().ToList();
                subjectList.OrderBy(s=>s.SubjectName).ToList().ForEach(s => {
                    ListItem li = new ListItem();
                    li.Text = s.SubjectName + "&nbsp;&nbsp;&nbsp;";
                    li.Value = s.Id.ToString();
                    cblSubjectName.Items.Add(li);
                });
            }

        }


        protected void btnCheckAllGuidance_Click(object sender, EventArgs e)
        {
            BindSubjectCategory();
        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectCategory();
            BindSubject();
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
                //var list = new QuotationItemBLL().GetList(s => s.CustomerId == customerId && s.GuidanceYear == year && s.GuidanceMonth == month && (s.IsDelete==null || s.IsDelete==false));
                var list = (from order in CurrentContext.DbContext.QuotationItem
                            join user in CurrentContext.DbContext.UserInfo
                            on order.AddUserId equals user.UserId
                            where order.CustomerId == customerId
                            && order.GuidanceYear == year
                            && order.GuidanceMonth == month
                            && (order.IsDelete == null || order.IsDelete == false)
                            select new {
                                order,
                                order.Id,
                                order.CustomerId,
                                order.GuidanceId,
                                order.QuoteSubjectCategoryId,
                                order.AddUserId,
                                AddUserName= user.RealName
                            }
                         ).ToList();
                gvList.DataSource = list;
                gvList.DataBind();
            }
        }

        SubjectGuidanceBLL guidanceBll = new SubjectGuidanceBLL();
        ADSubjectCategoryBLL subjectCategoryBll = new ADSubjectCategoryBLL();
        protected void gvList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            
            if (e.CommandName == "deleteItem")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                new SpecialPriceQuoteDetailBLL().Delete(s=>s.ItemId==id);
                new QuotationItemBLL().Delete(s=>s.Id==id);
                new QuoteOrderDetailBLL().UpdateQuoteItemId("", "", id, "delete");
                new ImportQuoteOrderBLL().Delete(s => s.ItemId == id);
                BindSubjectCategory();
                BindSubject();
                GetQuoteList();
            }
        }

        ImportQuoteOrderBLL importQuoteOrderBll = new ImportQuoteOrderBLL();
        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object guidanceIdObj = item.GetType().GetProperty("GuidanceId").GetValue(item, null);
                    object subjectCategoryIdObj = item.GetType().GetProperty("QuoteSubjectCategoryId").GetValue(item, null);
                    string guidanceId = guidanceIdObj != null ? guidanceIdObj.ToString() : string.Empty;
                    int subjectCategoryId = subjectCategoryIdObj != null ? int.Parse(subjectCategoryIdObj.ToString()) : 0;




                    if (!string.IsNullOrWhiteSpace(guidanceId))
                    {
                        List<int> guidanceIdList = StringHelper.ToIntList(guidanceId, ',');
                        List<string> guidanceNameList = guidanceBll.GetList(s => guidanceIdList.Contains(s.ItemId)).Select(s => s.ItemName).ToList();
                        if (guidanceNameList.Any())
                        {
                            ((Label)e.Item.FindControl("labGuidanceName")).Text = StringHelper.ListToString(guidanceNameList);
                        }
                    }
                    if (subjectCategoryId > 0)
                    {
                        ADSubjectCategory categoryModel = subjectCategoryBll.GetModel(subjectCategoryId);
                        if (categoryModel != null)
                        {
                            ((Label)e.Item.FindControl("labSubjectCategory")).Text = categoryModel.CategoryName;
                        }
                    }
                    //if (!string.IsNullOrWhiteSpace(subjectCategoryId))
                    //{
                    //    object customerIdObj = item.GetType().GetProperty("CustomerId").GetValue(item, null);
                    //    int customerId = customerIdObj != null ? int.Parse(customerIdObj.ToString()) : 0;
                    //    List<int> subjectCategoryIdList = StringHelper.ToIntList(subjectCategoryId, ',');
                    //    List<string> categoryNameList = subjectCategoryBll.GetList(s => s.CustomerId == customerId && subjectCategoryIdList.Contains(s.Id)).Select(s => s.CategoryName).ToList();
                    //    if (categoryNameList.Any())
                    //    {
                    //        ((Label)e.Item.FindControl("labSubjectCategory")).Text = StringHelper.ListToString(categoryNameList);
                    //    }
                    //}

                    LinkButton lbDelete = (LinkButton)e.Item.FindControl("lbDelete");
                    LinkButton lbEdit = (LinkButton)e.Item.FindControl("lbEdit");

                    object addUserIdObj = item.GetType().GetProperty("AddUserId").GetValue(item, null);
                    int addUserId = addUserIdObj != null ? int.Parse(addUserIdObj.ToString()) : 0;

                    object itemIdObj = item.GetType().GetProperty("Id").GetValue(item, null);
                    int itemId = itemIdObj != null ? int.Parse(itemIdObj.ToString()) : 0;

                    var importQuoteList = importQuoteOrderBll.GetList(s => s.ItemId == itemId);
                    if (importQuoteList.Any())
                    {
                        var popList = importQuoteList.Where(s =>s.OrderType == (int)OrderTypeEnum.POP);
                        decimal popPrice = popList.Sum(s => s.POPPrice ?? 0);
                        decimal popArea = popList.Sum(s => s.POPArea ?? 0);

                        var installList = importQuoteList.Where(s => s.OrderType == (int)OrderTypeEnum.安装费);
                        decimal installPrice = installList.Sum(s => (s.InstallUnitPrice ?? 0) * (s.Quantity ?? 1));

                        var expressList = importQuoteList.Where(s =>s.OrderType == (int)OrderTypeEnum.快递费).ToList();
                        decimal expressPrice = expressList.Sum(s => (s.ExpressUnitPrice ?? 0) * (s.Quantity ?? 1));

                        var materialList = importQuoteList.Where(s =>s.OrderType == (int)OrderTypeEnum.物料).ToList();
                        decimal materialPrice = materialList.Sum(s => (s.UnitPrice ?? 0) * (s.Quantity ?? 1));

                        ((Label)e.Item.FindControl("labTotalArea")).Text = popArea.ToString();
                        ((Label)e.Item.FindControl("labTotalPrice")).Text = (popPrice + installPrice + expressPrice + materialPrice).ToString();

                    }
                    if (addUserId == CurrentUser.UserId)
                    {
                        lbEdit.Enabled = true;
                        lbEdit.Style.Add("color", "blue");
                        lbEdit.Attributes.Add("OnClick", "return editItem('" + itemId + "')");

                        lbDelete.CommandName = "deleteItem";
                        lbDelete.Enabled = true;
                        lbDelete.Attributes.Add("OnClick", "javascript:return confirm('确定删除吗？')");
                        lbDelete.Style.Add("color", "red");
                    }
                    else
                    {
                        lbDelete.CommandName = "";
                        lbDelete.Enabled = false;
                        lbDelete.Style.Add("color", "#ccc");

                        lbEdit.Enabled = false;
                        lbEdit.Style.Add("color", "#ccc");

                    }
                }
            }
        }

        protected void btnRefreshOrder_Click(object sender, EventArgs e)
        {
            
            GetQuoteList();
        }


        protected void btnRefreshGuidance_Click(object sender, EventArgs e)
        {
            BindSubjectCategory();
            BindSubject();
            GetQuoteList();
        }


        protected void cblSubjectCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
        }

       
    }
}
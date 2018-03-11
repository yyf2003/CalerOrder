﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using DAL;
using Common;
using System.Transactions;


namespace WebApp.Subjects.RegionSubject
{
    public partial class List : BasePage
    {
        public string url = string.Empty;
        int operateType = 0;
        int subjectId = 0;
        int applicationDetailId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["operateType"] != null)
            {
                operateType = int.Parse(Request.QueryString["operateType"]);
                if (Request.QueryString["subjectId"] != null)
                    subjectId = int.Parse(Request.QueryString["subjectId"]);
                if (Request.QueryString["applicationDetailId"] != null)
                    applicationDetailId = int.Parse(Request.QueryString["applicationDetailId"]);
                labTitle.Text = "分区订单管理—项目变更操作";
            }
            url = Request.FilePath;
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);

                BindData();
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
                        join guidance1 in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance1.ItemId into temp1
                        from guidance in temp1.DefaultIfEmpty()
                        join subjectType1 in CurrentContext.DbContext.SubjectType
                        on subject.SubjectTypeId equals subjectType1.Id into temp2
                        from subjectType in temp2.DefaultIfEmpty()

                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                        on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                        from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1)
                        && (subject.SupplementRegion.Length > 0)
                        select new
                        {
                            subject.Id,
                            subject.AddDate,
                            subject.AddUserId,
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
                            guidance.ItemName,
                            guidance.IsFinish,
                            subjectType.SubjectTypeName,
                            subjectCategory.CategoryName,
                            subject.GuidanceId,
                            subject.SubjectTypeId,
                            subject.SubjectCategoryId,
                            subject.SubjectType
                        }).ToList();
            if (CurrentUser.RoleId == 5)
            {
                list = list.Where(s => s.AddUserId == CurrentUser.UserId).ToList();
            }
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidance.Items)
            {
                if (li.Selected)
                    guidanceIdList.Add(int.Parse(li.Value));
            }
            List<int> orderTypeList = new List<int>();
            foreach (ListItem li in cblOrderType.Items)
            {
                if (li.Selected)
                    orderTypeList.Add(int.Parse(li.Value));
            }
            if (guidanceIdList.Any())
            {
                list = list.Where(s => guidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();
            }

            if (orderTypeList.Any())
            {
                list = list.Where(s => orderTypeList.Contains(s.SubjectType ?? 1)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.ToUpper().Contains(txtSubjectName.Text.Trim().ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectNo.Text))
            {
                list = list.Where(s => s.SubjectNo.ToUpper().Contains(txtSubjectNo.Text.Trim().ToUpper())).ToList();
            }
            if (subjectId > 0)
            {
                list = list.Where(s => s.Id == subjectId).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }

        void BindGuidance()
        {
            cblGuidance.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && (s.IsDelete == null || s.IsDelete == false));
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text))
            {
                string guidanceMonth = txtGuidanceMonth.Text.Trim();
                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                {
                    DateTime date = DateTime.Parse(guidanceMonth);
                    int year = date.Year;
                    int month = date.Month;
                    list = list.Where(s => s.GuidanceYear == year && s.GuidanceMonth == month).ToList();
                }

            }
            else
            {
                DateTime date = DateTime.Now;
                DateTime newDate = new DateTime(date.Year, date.Month, 1);
                DateTime beginDate = newDate.AddMonths(-1);
                DateTime endDate = newDate.AddMonths(1);
                list = list.Where(s => s.BeginDate >= beginDate && s.BeginDate < endDate).ToList();
            }
            var subjectList = new SubjectBLL().GetList(s => s.GuidanceId != null && s.GuidanceId > 0 && (s.SupplementRegion.Length > 0) && (s.IsDelete == null || s.IsDelete == false));
            if (CurrentUser.RoleId == 5)
            {
                subjectList = subjectList.Where(s => s.AddUserId == CurrentUser.UserId).ToList();
            }
            List<int> guidanceIdList = subjectList.Select(s => s.GuidanceId ?? 0).Distinct().ToList();
            list = list.Where(s => guidanceIdList.Contains(s.ItemId)).ToList();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s.ItemName + "&nbsp;";
                    li.Value = s.ItemId.ToString();
                    cblGuidance.Items.Add(li);
                });
            }
        }

        void BindOrderType()
        {
            cblOrderType.Items.Clear();
            List<int> guidanceIdList = new List<int>();
            List<int> guidanceIdSelectList = new List<int>();
            foreach (ListItem li in cblGuidance.Items)
            {
                if (li.Selected)
                {
                    guidanceIdSelectList.Add(int.Parse(li.Value));
                }
                guidanceIdList.Add(int.Parse(li.Value));
            }
            if (!guidanceIdSelectList.Any())
            {
                guidanceIdSelectList = guidanceIdList;
            }
            var subjectList = new SubjectBLL().GetList(s => guidanceIdSelectList.Contains(s.GuidanceId ?? 0) && (s.SupplementRegion.Length > 0) && (s.IsDelete == false || s.IsDelete == null));
            if (CurrentUser.RoleId == 5)
            {
                subjectList = subjectList.Where(s => s.AddUserId == CurrentUser.UserId).ToList();
            }
            if (subjectList.Any())
            {
                List<int> orderTypeList = subjectList.Select(s => s.SubjectType ?? 1).Distinct().ToList();
                var enumList = CommonMethod.GetEnumList<SubjectTypeEnum>().Where(s => orderTypeList.Contains(s.Value)).ToList();
                if (enumList.Any())
                {
                    enumList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Value.ToString();
                        li.Text = s.Name + "&nbsp;";
                        cblOrderType.Items.Add(li);
                    });
                }

            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddSubject.aspx", false);
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "EditItem")
            {
                Response.Redirect(string.Format("AddSubject.aspx?subjectId={0}", id), false);

            }
            if (e.CommandName == "Check")
            {
                string url = string.Format("CheckOrderDetail.aspx?subjectId={0}", id);
                var model = new SubjectBLL().GetModel(id);
                if (model != null && (model.SubjectType ?? 1) == (int)SubjectTypeEnum.正常单)
                    url = string.Format("/Subjects/ShowOrderDetail.aspx?subjectId={0}", id);
                else if (model.SubjectType == (int)SubjectTypeEnum.费用订单)
                {
                    url = string.Format("/Subjects/PriceOrder/CheckOrderDetail.aspx?subjectId={0}", id);
                }
                Response.Redirect(url, false);
            }
            if (e.CommandName == "DeleteItem")
            {
                SubjectBLL subjectBll = new SubjectBLL();
                Models.Subject model = subjectBll.GetModel(id);
                if (model != null)
                {
                    bool isOk = true;
                    string msg = string.Empty;
                    using (TransactionScope tran = new TransactionScope())
                    {
                        try
                        {
                            model.IsDelete = true;
                            subjectBll.Update(model);
                           

                            FinalOrderDetailTempBLL finalOrderBll = new FinalOrderDetailTempBLL();
                            OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();
                            List<int> shopIdList = new List<int>();
                            if (model.SubjectType == (int)SubjectTypeEnum.HC订单 || model.SubjectType == (int)SubjectTypeEnum.分区补单)
                            {
                                var list0 = finalOrderBll.GetList(s => s.RegionSupplementId == id && (s.IsDelete == null || s.IsDelete == false));
                                if (list0.Any())
                                {
                                    shopIdList = list0.Select(s => s.ShopId ?? 0).ToList();
                                    list0.ForEach(s =>
                                    {
                                        s.DeleteDate = DateTime.Now;
                                        s.DeleteUserId = CurrentUser.UserId;
                                        s.IsDelete = true;
                                        finalOrderBll.Update(s);
                                    });
                                    //从报价订单表删除
                                    new QuoteOrderDetailBLL().Delete(s => s.RegionSupplementId == id);
                                    //删除外协订单
                                    outsourceOrderBll.Delete(s=>s.RegionSupplementId==id);
                                }
                            }
                            else
                            {
                                var list0 = finalOrderBll.GetList(s => s.SubjectId == id && (s.IsDelete == null || s.IsDelete == false));
                                if (list0.Any())
                                {
                                    shopIdList = list0.Select(s => s.ShopId ?? 0).ToList();
                                    list0.ForEach(s =>
                                    {
                                        s.DeleteDate = DateTime.Now;
                                        s.DeleteUserId = CurrentUser.UserId;
                                        s.IsDelete = true;
                                        finalOrderBll.Update(s);
                                    });
                                    //从报价订单表删除
                                    new QuoteOrderDetailBLL().Delete(s => s.SubjectId == id);
                                    //删除外协订单
                                    outsourceOrderBll.Delete(s => s.SubjectId == id);
                                }
                            }
                            //更新项目变更申请
                            if (applicationDetailId > 0)
                            {
                                OrderChangeApplicationDetailBLL changeApplicationDetailBll = new OrderChangeApplicationDetailBLL();
                                OrderChangeApplicationDetail changeModel = changeApplicationDetailBll.GetModel(applicationDetailId);
                                if (changeModel != null)
                                {
                                    changeModel.State = 2;
                                    changeModel.FinishUserId = CurrentUser.UserId;
                                    changeModel.FinishDate = DateTime.Now;
                                    changeApplicationDetailBll.Update(changeModel);
                                }
                            }
                            //重新计算安装费
                            if (shopIdList.Any() && model.SubjectType != (int)SubjectTypeEnum.二次安装 && model.SubjectType != (int)SubjectTypeEnum.费用订单)
                            {
                                SubjectGuidance guianceModel = new SubjectGuidanceBLL().GetModel(model.GuidanceId ?? 0);
                                //if (guianceModel != null && guianceModel.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                                //{
                                    //if ((guianceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guianceModel.HasInstallFees ?? true)))
                                    //{
                                    //    RecountInstallPrice(model.GuidanceId ?? 0, shopIdList);//重新计算活动安装费
                                    //    ResetOutsourceInstallPrice(model.GuidanceId ?? 0, shopIdList);//重新计算外协安装费
                                    //}
                                    //else if (guianceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guianceModel.HasExperssFees ?? true))
                                    //{
                                    //    ReSaveExpressPrice(model.GuidanceId ?? 0);//重新计算活动快递费
                                    //    ResetOutsourceInstallPrice(model.GuidanceId ?? 0, shopIdList);//重新计算外协安装费
                                    //}
                                   
                                //}
                                if (guianceModel != null && guianceModel.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                                {
                                    if ((guianceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guianceModel.HasInstallFees ?? true)))
                                    {
                                        if ((model.IsSecondInstall ?? false))
                                        {
                                            ReSaveSecondInstallPrice(model.GuidanceId ?? 0);//重新计算活动安装费(二次安装)
                                            ResetOutsourceSecondInstallPrice(model.GuidanceId ?? 0, shopIdList);//重新计算外协安装费(二次安装)
                                        }
                                        else
                                        {
                                            RecountInstallPrice(model.GuidanceId ?? 0, shopIdList);//重新计算活动安装费
                                            ResetOutsourceInstallPrice(model.GuidanceId ?? 0, shopIdList);//重新计算外协安装费
                                        }
                                    }
                                    //else if (guianceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guianceModel.HasExperssFees ?? true))
                                    //{
                                    //    ReSaveExpressPrice(model.GuidanceId ?? 0);//重新计算活动快递费
                                    //    ResetOutsourceInstallPrice(model.GuidanceId ?? 0, shopIdList);//重新计算外协安装费
                                    //}
                                }
                            }
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            isOk = false;
                            msg = "删除失败：" + ex.Message;
                        }
                    }
                    if (isOk)
                    {
                        BindData();
                    }
                    else
                    {
                        Alert(msg);
                    }

                }
            }

        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object isFinishObj = item.GetType().GetProperty("IsFinish").GetValue(item, null);
                    bool isFinish = isFinishObj != null ? bool.Parse(isFinishObj.ToString()) : false;
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    int subjectType = subjectTypeObj != null ? int.Parse(subjectTypeObj.ToString()) : 1;
                    object statusObj = item.GetType().GetProperty("Status").GetValue(item, null);
                    int status = statusObj != null ? int.Parse(statusObj.ToString()) : 0;
                    Label labSubjectType = (Label)e.Row.FindControl("labSubjectType");
                    //labSubjectType.Text = subjectType == 1 ? "POP订单" : subjectType == 2 ? "费用订单" : subjectType == 3?"补单":subjectType == 4?"二次安装费":"";
                    labSubjectType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());
                    Label labStatus = (Label)e.Row.FindControl("labStatus");
                    LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                    string statusMsg = string.Empty;
                    switch (status)
                    {
                        case 1:
                            statusMsg = "<span style='color:red;'>待下单</span>";

                            break;
                        case 2:
                            statusMsg = "<span style='color:blue;'>待拆单</span>";

                            break;
                        case 3:
                            statusMsg = "<span style='color:red;'>待提交</span>";

                            break;
                        case 4:
                            statusMsg = "<span style='color:green;'>提交完成</span>";

                            break;
                    }
                    labStatus.Text = statusMsg;
                    object approveObj = item.GetType().GetProperty("ApproveState").GetValue(item, null);
                    int approve = approveObj != null ? int.Parse(approveObj.ToString()) : 0;

                    Label labApprove = (Label)e.Row.FindControl("labApprove");
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");

                    lbEdit.CommandName = "EditItem";
                    lbEdit.Enabled = true;
                    lbEdit.Style.Add("color", "blue");

                    string approveMsg = string.Empty;
                    switch (approve)
                    {
                        case 0:
                            approveMsg = "<span style='color:blue;'>待审批</span>";
                            if (status < 4)
                            {
                                approveMsg = "--";
                            }
                            lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                            break;
                        case 1:
                            approveMsg = "<span style='color:green;'>审批通过</span>";
                            //if ((subjectType == (int)SubjectTypeEnum.HC订单 && isFinish) || subjectType != (int)SubjectTypeEnum.HC订单)
                            //{
                            //    lbEdit.CommandName = "";
                            //    lbEdit.Enabled = false;
                            //    lbEdit.Style.Add("color", "#ccc");
                            //}

                            //lbEdit.CommandName = "";
                            //lbEdit.Enabled = false;
                            //lbEdit.Style.Add("color", "#ccc");


                            //lbDelete.Enabled = false;
                            //lbDelete.CommandName = "";
                            //lbDelete.Style.Add("color", "#ccc");
                            if (operateType > 0)
                            {
                                
                                if (operateType == 1)
                                {
                                    //编辑
                                    lbDelete.Enabled = false;
                                    lbDelete.CommandName = "";
                                    lbDelete.Style.Add("color", "#ccc");
                                }
                                else if (operateType == 2)
                                {
                                    //删除
                                    lbEdit.CommandName = "";
                                    lbEdit.Enabled = false;
                                    lbEdit.Style.Add("color", "#ccc");
                                    lbDelete.Attributes.Add("OnClick", "return LoadDelete(this)");
                                }
                            }
                            else
                            {
                                lbEdit.CommandName = "";
                                lbEdit.Enabled = false;
                                lbEdit.Style.Add("color", "#ccc");

                                lbDelete.Enabled = false;
                                lbDelete.CommandName = "";
                                lbDelete.Style.Add("color", "#ccc");
                            }
                            break;
                        case 2:
                            approveMsg = "<span style='color:red;'>审批不通过</span>";
                            lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");

                            break;
                        default:
                            approveMsg = "--";
                            break;
                    }

                    labApprove.Text = approveMsg;

                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
            BindOrderType();
        }

        protected void cblGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindOrderType();
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }
    }
}
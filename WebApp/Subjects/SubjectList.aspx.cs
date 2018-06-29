using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using Common;
using System.Transactions;

namespace WebApp.Subjects
{
    public partial class SubjectList : BasePage
    {
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

                labTitle.Text = "项目信息—项目变更操作";
            }
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);

                BindSubjectCategory();
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
                        //where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1) && companyList.Contains(subject.CompanyId??0)
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1)
                        && (guidance != null ? (guidance.IsDelete == null || guidance.IsDelete == false) : 1 == 1)
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
                            subjectType.SubjectTypeName,
                            subjectCategory.CategoryName,
                            subject.GuidanceId,
                            subject.SubjectTypeId,
                            subject.SubjectCategoryId,
                            subject.SubjectType,
                            subject.SupplementRegion
                        }).ToList();
            if (CurrentUser.RoleId == 2)
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
            List<int> addUserList = new List<int>();
            foreach (ListItem li in cblAddUserName.Items)
            {
                if (li.Selected)
                {
                    addUserList.Add(int.Parse(li.Value));
                }
            }
            List<int> subjectCagetgoryList = new List<int>();
            foreach (ListItem li in cblSubjectCategory.Items)
            {
                if (li.Selected)
                    subjectCagetgoryList.Add(int.Parse(li.Value));
            }
            List<int> subjectTypeList = new List<int>();
            foreach (ListItem li in cblSubjectType.Items)
            {
                if (li.Selected)
                    subjectTypeList.Add(int.Parse(li.Value));
            }
            int approveState = -1;
            if (!string.IsNullOrWhiteSpace(rblApproveState.SelectedValue))
            {
                approveState = int.Parse(rblApproveState.SelectedValue);
            }

            if (guidanceIdList.Any())
            {
                list = list.Where(s => guidanceIdList.Contains(s.GuidanceId ?? 0)).ToList();
            }
            if (orderTypeList.Any())
            {

                list = list.Where(s => orderTypeList.Contains(s.SubjectType ?? 1)).ToList();
            }
            if (addUserList.Any())
            {

                list = list.Where(s => addUserList.Contains(s.AddUserId ?? 0)).ToList();
            }
            if (subjectTypeList.Any())
            {
                if (subjectTypeList.Contains(0))
                {
                    subjectTypeList.Remove(0);
                    if (subjectTypeList.Any())
                    {
                        list = list.Where(s => subjectTypeList.Contains(s.SubjectTypeId ?? 0) || (s.SubjectTypeId == null || s.SubjectTypeId == 0)).ToList();
                    }
                    else
                        list = list.Where(s => (s.SubjectTypeId == null || s.SubjectTypeId == 0)).ToList();
                }
                else
                    list = list.Where(s => subjectTypeList.Contains(s.SubjectTypeId ?? 0)).ToList();
            }
            if (subjectCagetgoryList.Any())
            {
                if (subjectCagetgoryList.Contains(0))
                {
                    subjectCagetgoryList.Remove(0);
                    if (subjectCagetgoryList.Any())
                    {
                        list = list.Where(s => subjectCagetgoryList.Contains(s.SubjectCategoryId ?? 0) || (s.SubjectCategoryId == null || s.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        list = list.Where(s => (s.SubjectCategoryId == null || s.SubjectCategoryId == 0)).ToList();
                }
                else
                    list = list.Where(s => subjectCagetgoryList.Contains(s.SubjectCategoryId ?? 0)).ToList();
            }


            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.ToUpper().Contains(txtSubjectName.Text.Trim().ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectNo.Text))
            {
                list = list.Where(s => s.SubjectNo.Contains(txtSubjectNo.Text)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text))
            {
                List<int> subjectList = new List<int>();
                Shop shopModel = new ShopBLL().GetList(s => s.ShopNo.ToUpper() == txtShopNo.Text.Trim().ToUpper()).FirstOrDefault();

                if (shopModel != null)
                {
                    var orderList = new FinalOrderDetailTempBLL().GetList(s => s.ShopId == shopModel.Id && (s.IsDelete == null || s.IsDelete == false));
                    if (orderList.Any())
                    {
                        subjectList = orderList.Select(s => s.SubjectId ?? 0).Distinct().ToList();
                    }
                }

                list = list.Where(s => subjectList.Contains(s.Id)).ToList();
            }
            if (approveState != -1)
            {
                if (approveState == 0)
                    list = list.Where(s => s.ApproveState == null || s.ApproveState == 0).ToList();
                else
                    list = list.Where(s => s.ApproveState == approveState).ToList();
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


        SubjectGuidanceBLL guidanceBll = new SubjectGuidanceBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object guidanceIdObj = item.GetType().GetProperty("GuidanceId").GetValue(item, null);
                    int guidanceId = guidanceIdObj != null ? int.Parse(guidanceIdObj.ToString()) : 0;
                    bool SubjectGuidanceIsFinish = false;
                    SubjectGuidance guidanceModel = guidanceBll.GetModel(guidanceId);
                    if (guidanceModel != null)
                    {
                        SubjectGuidanceIsFinish = guidanceModel.IsFinish ?? false;
                    }
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    int subjectType = subjectTypeObj != null ? int.Parse(subjectTypeObj.ToString()) : 1;
                    object statusObj = item.GetType().GetProperty("Status").GetValue(item, null);
                    int status = statusObj != null ? int.Parse(statusObj.ToString()) : 0;
                    Label labSubjectType = (Label)e.Row.FindControl("labSubjectType");
                    //labSubjectType.Text = subjectType == 1 ? "POP订单" : subjectType == 2 ? "费用订单" : subjectType == 3?"补单":subjectType == 4?"二次安装费":"";
                    labSubjectType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());
                    Label labStatus = (Label)e.Row.FindControl("labStatus");


                    LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                    LinkButton lbModify = (LinkButton)e.Row.FindControl("lbModifyOrder");

                    object subjectNameObj = item.GetType().GetProperty("SubjectName").GetValue(item, null);
                    string subjectName = subjectNameObj != null ? subjectNameObj.ToString() : "";

                    object remarkObj = item.GetType().GetProperty("Remark").GetValue(item, null);
                    string remark = remarkObj != null ? remarkObj.ToString() : "";

                    Label labSubjectName = (Label)e.Row.FindControl("labSubjectName");
                    if (subjectName.Contains("补单") && !string.IsNullOrWhiteSpace(remark))
                    {
                        subjectName += "(" + remark + ")";
                    }
                    labSubjectName.Text = subjectName;

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
                            //lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                            lbDelete.Attributes.Add("OnClick", "return LoadDelete(this)");
                            lbModify.CommandName = "";
                            lbModify.Enabled = false;
                            lbModify.Style.Add("color", "#ccc");
                            break;
                        case 1:
                            approveMsg = "<span style='color:green;'>审批通过</span>";

                            if (operateType > 0)
                            {
                                lbModify.CommandName = "";
                                lbModify.Enabled = false;
                                lbModify.Style.Add("color", "#ccc");
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
                                if (CurrentUser.RoleId == 1)
                                {
                                    //lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                                    lbDelete.Attributes.Add("OnClick", "return LoadDelete(this)");
                                }
                                else
                                {
                                    lbModify.CommandName = "";
                                    lbModify.Enabled = false;
                                    lbModify.Style.Add("color", "#ccc");

                                    lbDelete.Enabled = false;
                                    lbDelete.CommandName = "";
                                    lbDelete.Style.Add("color", "#ccc");
                                }
                            }
                            break;
                        case 2:
                            approveMsg = "<span style='color:red;'>审批不通过</span>";
                            //lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                            lbDelete.Attributes.Add("OnClick", "return LoadDelete(this)");
                            lbModify.CommandName = "";
                            lbModify.Enabled = false;
                            lbModify.Style.Add("color", "#ccc");
                            break;
                        default:
                            approveMsg = "--";
                            break;
                    }

                    labApprove.Text = approveMsg;
                }
            }
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "EditItem")
            {
                var model = new SubjectBLL().GetModel(id);
                int subjectType = 1;
                if (model != null)
                {
                    subjectType = model.SubjectType ?? 1;
                }
                string url = string.Format("AddSubject.aspx?subjectId={0}", id);
                
                if (subjectType == (int)SubjectTypeEnum.补单)
                {
                    url = string.Format("/Subjects/HandMadeOrder/Add.aspx?subjectId={0}", id);
                }
                else if (subjectType == (int)SubjectTypeEnum.道具订单)
                {
                    url = string.Format("/PropSubject/AddSubject.aspx?subjectId={0}", id);
                }
                Response.Redirect(url, false);

            }
            if (e.CommandName == "Check")
            {
                string url = string.Format("ShowOrderDetail.aspx?subjectId={0}", id);
                var model = new SubjectBLL().GetModel(id);
                if (model != null)
                {
                    if (model.SubjectType == (int)SubjectTypeEnum.道具订单)
                    {
                        url = string.Format("/PropSubject/CheckOrderDetail.aspx?subjectId={0}", id);
                    }
                    else if (model.SubjectType == (int)SubjectTypeEnum.补单)
                    {
                        url = string.Format("/Subjects/HandMadeOrder/CheckDetail.aspx?subjectId={0}", id);
                    }
                    else if (model.SubjectType == (int)SubjectTypeEnum.费用订单)
                    {
                        url = string.Format("/Subjects/PriceOrder/CheckOrderDetail.aspx?subjectId={0}", id);
                    }
                    else
                    {
                        if (model.SupplementRegion != null && model.SupplementRegion.Length > 0 && model.SubjectType != (int)SubjectTypeEnum.正常单)
                        {
                            url = string.Format("/Subjects/RegionSubject/CheckOrderDetail.aspx?subjectId={0}", id);
                        }
                        if (model.SubjectType == (int)SubjectTypeEnum.二次安装)
                        {
                            url = string.Format("/Subjects/SecondInstallFee/CheckOrderDetail.aspx?subjectId={0}", id);
                        }
                    }
                    if (model.SubjectType == (int)SubjectTypeEnum.外协订单)
                    {
                        url = string.Format("/Subjects/OutsourceMaterialOrder/SubjectDetail.aspx?subjectId={0}", id);
                    }
                    
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
                            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                            var totalOrderList = orderBll.GetList(s => s.SubjectId == id);
                            //if (model.SubjectType == (int)SubjectTypeEnum.HC订单 || model.SubjectType == (int)SubjectTypeEnum.分区补单)
                            //{

                            //}
                            //else
                            //{
                            //    var regionOrderList = totalOrderList.Where(s => (s.RegionSupplementId ?? 0) > 0 && (s.IsDelete == null || s.IsDelete == false));
                            //    if (regionOrderList.Any())
                            //    {
                            //        isOk = false;
                            //        msg = "该项目包含分区订单，请先联系分区客服把分区订单删除后才能删除项目！";
                            //    }
                            //}
                            var regionOrderList = totalOrderList.Where(s => ((s.RegionSupplementId ?? 0) > 0) && s.RegionSupplementId!=s.SubjectId && (s.IsDelete == null || s.IsDelete == false));
                            if (regionOrderList.Any())
                            {
                                isOk = false;
                                msg = "该项目包含分区订单，请先联系分区客服把分区订单删除后才能删除项目！";
                            }
                            if (isOk)
                            {
                                model.IsDelete = true;
                                model.DeleteDate = DateTime.Now;
                                subjectBll.Update(model);

                                if ((model.IsSecondInstall ?? false))
                                {
                                    ReSaveSecondInstallPrice(model.GuidanceId ?? 0);
                                }
                                OutsourceOrderDetailBLL outsourceOrderBll = new OutsourceOrderDetailBLL();

                                List<int> shopIdList = new List<int>();
                                if (model.SubjectType == (int)SubjectTypeEnum.HC订单 || model.SubjectType == (int)SubjectTypeEnum.分区补单)
                                {

                                    var list0 = orderBll.GetList(s => s.RegionSupplementId == id);
                                    if (list0.Any())
                                    {
                                        shopIdList = list0.Select(s => s.ShopId ?? 0).Distinct().ToList();
                                        list0.ForEach(s =>
                                        {
                                            s.DeleteDate = DateTime.Now;
                                            s.DeleteUserId = CurrentUser.UserId;
                                            s.IsDelete = true;
                                            orderBll.Update(s);
                                        });
                                    }
                                    //从报价订单表删除
                                    new QuoteOrderDetailBLL().Delete(s => s.RegionSupplementId == id);
                                    //从外协订单表删除
                                    outsourceOrderBll.Delete(s => s.RegionSupplementId == id);
                                }
                                else
                                {

                                    if (totalOrderList.Any())
                                    {
                                        shopIdList = totalOrderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                                        totalOrderList.ForEach(s =>
                                        {
                                            s.DeleteDate = DateTime.Now;
                                            s.DeleteUserId = CurrentUser.UserId;
                                            s.IsDelete = true;
                                            orderBll.Update(s);
                                        });

                                    }
                                    //从报价订单表删除
                                    new QuoteOrderDetailBLL().Delete(s => s.SubjectId == id);
                                    //从外协订单表删除
                                    outsourceOrderBll.Delete(s => s.SubjectId == id);
                                }
                                //applicationDetailId
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
                                //重新计算外协安装费
                                if (shopIdList.Any() && model.SubjectType != (int)SubjectTypeEnum.二次安装 && model.SubjectType != (int)SubjectTypeEnum.费用订单)
                                {
                                    SubjectGuidance guianceModel = new SubjectGuidanceBLL().GetModel(model.GuidanceId ?? 0);
                                    //if (guianceModel != null && guianceModel.ActivityTypeId != (int)GuidanceTypeEnum.Others)
                                    //{
                                    //    if ((guianceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (guianceModel.HasInstallFees ?? true)) || (guianceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion))
                                    //        ResetOutsourceInstallPrice(model.GuidanceId ?? 0, shopIdList);
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
                                        else if (guianceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guianceModel.HasExperssFees ?? true))
                                        {
                                            ReSaveExpressPrice(model.GuidanceId ?? 0);//重新计算活动快递费
                                            ResetOutsourceInstallPrice(model.GuidanceId ?? 0, shopIdList);//重新计算外协安装费
                                        }
                                    }

                                }
                                tran.Complete();
                            }
                            //Alert("删除成功！");
                        }
                        catch (Exception ex)
                        {
                            isOk = false;
                            msg = "删除失败：" + ex.Message;
                            //Alert("删除失败：" + ex.Message);
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
            if (e.CommandName == "Export")
            {
                Response.Redirect(string.Format("ExportOrder.aspx?subjectId={0}", id), false);
            }
            if (e.CommandName == "ModifyOrder")
            {
                Response.Redirect(string.Format("ModifyOrder/List.aspx?subjectId={0}", id), false);
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void btnSearchSubject_Click(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void cblGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAddUser();
            BindSubjectCategory();
            BindSubjectType();
            BindOrderType();
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
                //DateTime begin = DateTime.Parse(txtBeginDate.Text.Trim());
                //list = list.Where(s => s.AddDate >= begin).ToList();
                //if (!string.IsNullOrWhiteSpace(txtEndDate.Text))
                //{
                //    DateTime end = DateTime.Parse(txtEndDate.Text.Trim()).AddDays(1);
                //    list = list.Where(s => s.AddDate < end).ToList();
                //}
            }
            else
            {
                DateTime date = DateTime.Now;
                DateTime newDate = new DateTime(date.Year, date.Month, 1);
                DateTime beginDate = newDate.AddMonths(-1);
                DateTime endDate = newDate.AddMonths(2);
                list = list.Where(s => s.BeginDate >= beginDate && s.BeginDate < endDate).ToList();
            }
            if (CurrentUser.RoleId == 2)
            {
                var subjectList = new SubjectBLL().GetList(s => s.AddUserId == CurrentUser.UserId && s.GuidanceId != null && s.GuidanceId > 0 && (s.IsDelete == null || s.IsDelete == false));
                List<int> guidanceIdList = subjectList.Select(s => s.GuidanceId ?? 0).Distinct().ToList();
                list = list.Where(s => guidanceIdList.Contains(s.ItemId)).ToList();
            }
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

        void BindSubjectType()
        {
            cblSubjectType.Items.Clear();
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidance.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            List<int> orderTypeList = new List<int>();
            foreach (ListItem li in cblOrderType.Items)
            {
                if (li.Selected)
                {
                    orderTypeList.Add(int.Parse(li.Value));
                }
            }
            List<int> addUserList = new List<int>();
            foreach (ListItem li in cblAddUserName.Items)
            {
                if (li.Selected)
                {
                    addUserList.Add(int.Parse(li.Value));
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
            var list = (from subject in CurrentContext.DbContext.Subject
                        join subjectType1 in CurrentContext.DbContext.SubjectType
                        on subject.SubjectTypeId equals subjectType1.Id into subjectTypeTemp
                        from subjectType in subjectTypeTemp.DefaultIfEmpty()
                        where guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
                       && (orderTypeList.Any() ? (orderTypeList.Contains(subject.SubjectType ?? 1)) : 1 == 1)
                        && (addUserList.Any() ? (addUserList.Contains(subject.AddUserId ?? 0)) : 1 == 1)
                        select new
                        {
                            subject,
                            subjectType
                        }).ToList();
            if (subjectCategoryList.Any())
            {
                if (subjectCategoryList.Contains(0))
                {
                    subjectCategoryList.Remove(0);
                    if (subjectCategoryList.Any())
                    {
                        list = list.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0) || (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                    }
                    else
                        list = list.Where(s => (s.subject.SubjectCategoryId == null || s.subject.SubjectCategoryId == 0)).ToList();
                }
                else
                    list = list.Where(s => subjectCategoryList.Contains(s.subject.SubjectCategoryId ?? 0)).ToList();
            }
            if (list.Any())
            {
                bool isNull = false;
                list.Select(s => new { s.subject.SubjectTypeId, s.subjectType }).Distinct().OrderBy(s => s.SubjectTypeId).ToList().ForEach(s =>
                {
                    if (s.SubjectTypeId != null && s.SubjectTypeId > 0)
                    {
                        ListItem li = new ListItem();
                        li.Value = s.subjectType.Id.ToString();
                        li.Text = s.subjectType.SubjectTypeName + "&nbsp;";
                        cblSubjectType.Items.Add(li);
                    }
                    else
                        isNull = true;
                });
                if (isNull)
                {
                    cblSubjectType.Items.Add(new ListItem("空", "0"));
                }
            }
            //var typeList = new SubjectTypeBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId) && (s.IsDelete == false || s.IsDelete == null));
            //if (typeList.Any())
            //{

            //    typeList.ForEach(s =>
            //    {
            //        ListItem li = new ListItem();
            //        li.Value = s.Id.ToString();
            //        li.Text = s.SubjectTypeName + "&nbsp;";
            //        cblSubjectType.Items.Add(li);
            //    });
            //}
        }

        void BindSubjectCategory()
        {
            cblSubjectCategory.Items.Clear();
            //var List = new ADSubjectCategoryBLL().GetList(s => s.Id > 0);
            //if (List.Any())
            //{
            //    List.ForEach(s =>
            //    {
            //        ListItem li = new ListItem();
            //        li.Value = s.Id.ToString();
            //        li.Text = s.CategoryName + "&nbsp;";
            //        cblSubjectCategory.Items.Add(li);
            //    });
            //}
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidance.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            List<int> orderTypeList = new List<int>();
            foreach (ListItem li in cblOrderType.Items)
            {
                if (li.Selected)
                {
                    orderTypeList.Add(int.Parse(li.Value));
                }
            }
            List<int> addUserList = new List<int>();
            foreach (ListItem li in cblAddUserName.Items)
            {
                if (li.Selected)
                {
                    addUserList.Add(int.Parse(li.Value));
                }
            }
            var list = (from subject in CurrentContext.DbContext.Subject
                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                        on subject.SubjectCategoryId equals subjectCategory1.Id into subjectCategoryTemp
                        from subjectCategory in subjectCategoryTemp.DefaultIfEmpty()
                        where guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == null || subject.IsDelete == false)
                        && (orderTypeList.Any() ? (orderTypeList.Contains(subject.SubjectType ?? 1)) : 1 == 1)
                        && (addUserList.Any() ? (addUserList.Contains(subject.AddUserId ?? 0)) : 1 == 1)
                        select new
                        {
                            subject.SubjectCategoryId,
                            subjectCategory
                        }).Distinct().OrderBy(s => s.SubjectCategoryId).ToList();
            if (list.Any())
            {
                bool isNull = false;
                list.ForEach(s =>
                {
                    if (s.SubjectCategoryId != null && s.SubjectCategoryId > 0)
                    {
                        ListItem li = new ListItem();
                        li.Value = s.subjectCategory.Id.ToString();
                        li.Text = s.subjectCategory.CategoryName + "&nbsp;";
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

        void BindOrderType()
        {
            cblOrderType.Items.Clear();
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidance.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            var subjectList = new SubjectBLL().GetList(s => guidanceIdList.Contains(s.GuidanceId ?? 0) && (s.IsDelete == false || s.IsDelete == null));
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

        void BindAddUser()
        {
            cblAddUserName.Items.Clear();
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidance.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            List<int> orderTypeList = new List<int>();
            foreach (ListItem li in cblOrderType.Items)
            {
                if (li.Selected)
                {
                    orderTypeList.Add(int.Parse(li.Value));
                }
            }
            var userList = (from subject in CurrentContext.DbContext.Subject
                            join user in CurrentContext.DbContext.UserInfo
                            on subject.AddUserId equals user.UserId
                            where guidanceIdList.Contains(subject.GuidanceId ?? 0) && (subject.IsDelete == false || subject.IsDelete == null)
                            && (orderTypeList.Any() ? (orderTypeList.Contains(subject.SubjectType ?? 1)) : 1 == 1)
                            select user).Distinct().ToList();
            if (userList.Any())
            {
                userList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.UserId.ToString();
                    li.Text = s.RealName + "&nbsp;";
                    cblAddUserName.Items.Add(li);
                });
            }

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddSubject.aspx", false);
        }

        protected void cblAddUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectType();
            BindSubjectCategory();
        }

        protected void cblSubjectCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubjectType();
        }



        protected void cblOrderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAddUser();
            BindSubjectType();
            BindSubjectCategory();
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }
    }
}
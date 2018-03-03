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
    public partial class AddSubject : BasePage
    {
        int subjectId;
        SubjectBLL subjectBll = new SubjectBLL();
        Subject subjectModel;
        int ItemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["itemId"] != null)
            {
                ItemId = int.Parse(Request.QueryString["itemId"]);

            }
            if (!IsPostBack)
            {
                BindMyCustomerList(ddlCustomer);
                BindSBCInstallType();
                if (subjectId == 0)
                {
                    BindOrderType();
                    BindGuidanceList();
                }
                if (ItemId > 0)
                    BindSubjectType(ItemId);
                BindRegion();
                //ddlCustomer.SelectedIndex = 1;
                BindSubjectCategory();
                BindData();
                

            }
        }

        void BindRegion()
        {
            rblPriceBlong.Items.Clear();
            rblRegion.Items.Clear();
            ListItem li0 = new ListItem();
            li0.Value = "";
            li0.Text = "默认&nbsp;&nbsp;";
            li0.Selected = true;
            rblPriceBlong.Items.Add(li0);
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<string> regions = new RegionBLL().GetList(s => s.CustomerId == customerId).Select(s => s.RegionName).Distinct().ToList();

            if (regions.Any())
            {
                regions.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    rblPriceBlong.Items.Add(li);

                    rblRegion.Items.Add(li);
                });
            }
        }

        void BindGuidanceList()
        {
            ddlGuidance.Items.Clear();
            
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            DateTime now = DateTime.Now;
            var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && s.ActivityTypeId != (int)GuidanceTypeEnum.Others && (s.IsFinish == null || s.IsFinish == false) && (s.IsDelete == null || s.IsDelete == false)).OrderByDescending(s => s.ItemId).ToList();
            //if (ItemId == 0)
            //{
            //    if (subjectId == 0)
            //    {
            //        DateTime date = DateTime.Now;
            //        DateTime newDate = new DateTime(date.Year, date.Month, 1);
            //        DateTime beginDate = newDate.AddMonths(-1);
            //        DateTime endDate = newDate.AddMonths(2);
            //        list = list.Where(s => s.EndDate >= date).ToList();
            //        //list = list.Where(s => s.BeginDate >= beginDate && s.BeginDate < endDate).ToList();
            //    }

            //}
            DateTime date = DateTime.Now;
            list = list.Where(s => s.EndDate >= date).ToList();
            if (list.Any())
            {
                ddlGuidance.DataSource = list;
                ddlGuidance.DataTextField = "ItemName";
                ddlGuidance.DataValueField = "ItemId";
                ddlGuidance.DataBind();
                
                if (ItemId > 0)
                {
                    ddlGuidance.SelectedValue = ItemId.ToString();
                    ddlGuidance.Enabled = false;
                }
                if (subjectId > 0)
                    ddlGuidance.Enabled = false;
            }
            ddlGuidance.Items.Insert(0,new ListItem("请选择", "0"));
        }

        void BindOrderType()
        {
            rblSubjectType.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from corderType in CurrentContext.DbContext.CustomerOrderType
                                 join orderType in CurrentContext.DbContext.OrderType
                                 on corderType.OrderTypeId equals orderType.Id
                                 where corderType.CustomerId == customerId
                                 && (orderType.ForRegion == null || (orderType.LevelNum == 1 || orderType.LevelNum == 3))
                                 && (orderType.IsDelete==null || orderType.IsDelete==false)
                                 select orderType).ToList();



            //List<EnumEntity> list = CommonMethod.GetEnumList<SubjectTypeEnum>().Where(s => s.Desction == OrderChannelEnum.上海订单.ToString()).ToList();
            if (list.Any())
            {
                int index = 0;
                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.OrderTypeName + "&nbsp;";
                    if (index == 0)
                        li.Selected = true;
                    rblSubjectType.Items.Add(li);
                    index++;
                });
            }
        }

        void BindSubjectType(int itemId)
        {

            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(itemId);
            ddlSubjectType.Items.Clear();
            if (model != null)
            {
                if (model.BeginDate != null)
                {
                    txtBeginDate.Text = DateTime.Parse(model.BeginDate.ToString()).ToShortDateString();
                }
                if (model.EndDate != null)
                {
                    txtEndDate.Text = DateTime.Parse(model.EndDate.ToString()).ToShortDateString();
                }
                var typeList = new SubjectTypeBLL().GetList(s => s.GuidanceId == itemId && (s.IsDelete == false || s.IsDelete == null));
                if (typeList.Any())
                {
                    typeList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectTypeName;
                        ddlSubjectType.Items.Add(li);
                    });
                }
            }
            ddlSubjectType.Items.Insert(0, new ListItem("请选择", "0"));
        }

        void BindSubjectCategory()
        {
            ddlSubjectCategory.Items.Clear();
            ddlSubjectCategory.Items.Add(new ListItem("请选择","0"));
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var List = new ADSubjectCategoryBLL().GetList(s => s.CustomerId == customerId && (s.IsDelete==null || s.IsDelete==false));
            if (List.Any())
            {
                List.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.CategoryName;
                    ddlSubjectCategory.Items.Add(li);
                });
            }
        }

        void BindData()
        {
            if (subjectId > 0)
            {
                labTitel.Text = "编辑项目";
                subjectModel = subjectBll.GetModel(subjectId);
                if (subjectModel != null)
                {
                    if (subjectModel.CustomerId != null)
                    {
                        ddlCustomer.SelectedValue = subjectModel.CustomerId.ToString();
                        BindOrderType();
                        BindGuidanceList();
                        rblSubjectType.Enabled = false;
                    }
                    
                    //string name = System.Web.HttpUtility.UrlDecode(subjectModel.SubjectName, System.Text.Encoding.UTF8);
                    if (subjectModel.GuidanceId != null)
                    {
                        ddlGuidance.SelectedValue = subjectModel.GuidanceId.ToString();
                        BindSubjectType(subjectModel.GuidanceId ?? 0);
                    }
                    txtSubjectName.Text = subjectModel.SubjectName;
                    //txtOutName.Text = subjectModel.OutSubjectName;
                    txtBeginDate.Text = subjectModel.BeginDate != null ? DateTime.Parse(subjectModel.BeginDate.ToString()).ToShortDateString() : "";
                    txtEndDate.Text = subjectModel.EndDate != null ? DateTime.Parse(subjectModel.EndDate.ToString()).ToShortDateString() : "";
                    //txtContact.Text = subjectModel.Contact;
                    //txtTel.Text = subjectModel.Tel;
                   
                    //rblOrderType.SelectedValue = subjectModel.AddOrderType != null ? subjectModel.AddOrderType.ToString() : "";
                    txtRemark.Text = subjectModel.Remark;
                    if (subjectModel.SubjectTypeId != null)
                        ddlSubjectType.SelectedValue = subjectModel.SubjectTypeId.ToString();
                    if (subjectModel.SubjectCategoryId != null)
                        ddlSubjectCategory.SelectedValue = subjectModel.SubjectCategoryId.ToString();
                    //if (subjectModel.IsInstall!=null)
                    //rblIsInstall.SelectedValue = subjectModel.IsInstall.ToString();
                    if (subjectModel.SubjectType != null)
                        rblSubjectType.SelectedValue = subjectModel.SubjectType.ToString();
                    if (subjectModel.PriceBlongRegion != null)
                    {
                        rblPriceBlong.SelectedValue = subjectModel.PriceBlongRegion;

                    }
                    if ((subjectModel.IsSecondInstall ?? false))
                    {
                        cbIsSecondInstall.Checked = true;
                        rblSecondInstallType.SelectedValue = (subjectModel.SecondBasicInstallPriceType ?? 1).ToString();
                    }
                    if (subjectModel.Status == 4)
                    {
                        rblSubjectType.Enabled = false;
                        ddlSubjectCategory.Enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNext_Click(object sender, EventArgs e)
        {
            labMsg.Text = "";
            if (CheckSubject(txtSubjectName.Text.Trim(), subjectId))
            {
                labMsg.Text = "项目名称重复";
                return;
            }
            if (subjectId > 0)
            {
                subjectModel = subjectBll.GetModel(subjectId);
            }
            else
            {
                subjectModel = new Subject();
                subjectModel.AddOrderType = 3;
                subjectModel.AddUserId = CurrentUser.UserId;
                subjectModel.AddDate = DateTime.Now;
                subjectModel.IsDelete = false;
                subjectModel.Status = 1;
                subjectModel.SubjectNo = CreateSubjectNo();
                subjectModel.CompanyId = CurrentUser.CompanyId;
                subjectModel.ApproveState = 0;
            }
            subjectModel.BeginDate = DateTime.Parse(txtBeginDate.Text.Trim());
            subjectModel.CustomerId = int.Parse(ddlCustomer.SelectedValue);
            subjectModel.EndDate = DateTime.Parse(txtEndDate.Text.Trim());
            subjectModel.Remark = StringHelper.ReplaceSpecialChar(txtRemark.Text.Trim());

            //subjectModel.SubjectName = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(txtSubjectName.Text.Trim()," ");
            //subjectModel.OutSubjectName = txtOutName.Text.Trim();
            subjectModel.SubjectName = StringHelper.ReplaceSpecialChar(txtSubjectName.Text.Trim());
            subjectModel.SubjectCategoryId = int.Parse(ddlSubjectCategory.SelectedValue);
            if (ddlSubjectCategory.SelectedItem.Text == CornerTypeEnum.三叶草.ToString())
            {
                subjectModel.CornerType = CornerTypeEnum.三叶草.ToString();
            }
            else
                subjectModel.CornerType = string.Empty;
            subjectModel.SubjectTypeId = int.Parse(ddlSubjectType.SelectedValue);
            subjectModel.PriceBlongRegion = rblPriceBlong.SelectedValue;
            if (!string.IsNullOrWhiteSpace(rblSubjectType.SelectedValue))
                subjectModel.SubjectType = int.Parse(rblSubjectType.SelectedValue);
            else
                subjectModel.SubjectType = 1;//默认是pop订单
            if (cbIsSecondInstall.Checked)
            {
                subjectModel.IsSecondInstall = true;
                if (rblSecondInstallType.SelectedIndex > 0)
                    subjectModel.SecondBasicInstallPriceType = int.Parse(rblSecondInstallType.SelectedValue);
                else
                    subjectModel.SecondBasicInstallPriceType = 1;
            }
            else
            {
                subjectModel.IsSecondInstall = false;
                subjectModel.SecondBasicInstallPriceType = null;
            }

            if (subjectId > 0)
            {
                subjectBll.Update(subjectModel);
            }
            else
            {
                if (ItemId == 0)
                    ItemId = int.Parse(ddlGuidance.SelectedValue);
                subjectModel.GuidanceId = ItemId;
                subjectBll.Add(subjectModel);
            }
            if (subjectModel.SubjectType == (int)SubjectTypeEnum.二次安装)
            {
                Response.Redirect("SecondInstallFee/AddOrderDetail.aspx?subjectId=" + subjectModel.Id, false);
            }
            else if (subjectModel.SubjectType == (int)SubjectTypeEnum.费用订单)
            {
                Response.Redirect("PriceOrder/AddOrderDetail.aspx?subjectId=" + subjectModel.Id, false);
            }
            else if (subjectModel.SubjectType == (int)SubjectTypeEnum.散单)
            {
                Response.Redirect("ExtraOrder/AddOrderDetail.aspx?subjectId=" + subjectModel.Id, false);
            }
            else
                Response.Redirect("ADOrders/ImportOrder.aspx?subjectId=" + subjectModel.Id, false);
        }



        /// <summary>
        /// 检查项目名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool CheckSubject(string name, int id)
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            //var list = subjectBll.GetList(s => StringHelper.ReplaceSpace(s.SubjectName) == StringHelper.ReplaceSpace(name) && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1));
            name = StringHelper.ReplaceSpace(name);
            var list = subjectBll.GetList(s => s.SubjectName.Replace(" ", "").ToLower() == name.ToLower() && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1) && (s.IsDelete == null || s.IsDelete == false));

            return list.Any();
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
            int itemId = int.Parse((sender as DropDownList).SelectedValue);
            BindSubjectType(itemId);
        }

        /// <summary>
        /// 提交无POP订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    
                    labMsg.Text = "";
                    if (CheckSubject(txtSubjectName.Text.Trim(), subjectId))
                    {
                        labMsg.Text = "项目名称重复";
                        return;
                    }
                    if (subjectId > 0)
                    {
                        subjectModel = subjectBll.GetModel(subjectId);
                        int subjectType = subjectModel.SubjectType ?? 1;
                        switch (subjectType)
                        {
                            case (int)SubjectTypeEnum.POP订单:
                                new MergeOriginalOrderBLL().Delete(s => s.SubjectId == subjectId);
                                new POPOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                                new ListOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                                new FinalOrderDetailTempBLL().Delete(s => s.SubjectId == subjectId);
                                OrderPlanBLL planBll = new OrderPlanBLL();
                                SplitOrderPlanDetailBLL planDetailBll = new SplitOrderPlanDetailBLL();
                                var planList = planBll.GetList(s => s.SubjectId == subjectId);
                                if (planList.Any())
                                {
                                    List<int> planIdList = planList.Select(s => s.Id).ToList();
                                    planDetailBll.Delete(s=>planIdList.Contains(s.PlanId??0));
                                    planBll.Delete(s => s.SubjectId == subjectId);
                                }
                                break;
                            case (int)SubjectTypeEnum.运费:
                            case (int)SubjectTypeEnum.新开店安装费:
                                new PriceOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                                break;
                            case (int)SubjectTypeEnum.手工订单:
                                new HandMadeOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                                break;
                        }
                    }
                    else
                    {
                        subjectModel = new Subject();

                        subjectModel.AddOrderType = 3;
                        subjectModel.AddUserId = CurrentUser.UserId;
                        subjectModel.AddDate = DateTime.Now;
                        subjectModel.IsDelete = false;
                        subjectModel.SubjectNo = CreateSubjectNo();
                        subjectModel.CompanyId = CurrentUser.CompanyId;

                    }
                    subjectModel.Status = 4;
                    subjectModel.ApproveState = 1;
                    subjectModel.BeginDate = DateTime.Parse(txtBeginDate.Text.Trim());

                    subjectModel.CustomerId = int.Parse(ddlCustomer.SelectedValue);
                    subjectModel.EndDate = DateTime.Parse(txtEndDate.Text.Trim());
                    subjectModel.Remark = txtRemark.Text;

                    subjectModel.SubjectName = txtSubjectName.Text.Trim();

                    subjectModel.SubjectCategoryId = int.Parse(ddlSubjectCategory.SelectedValue);
                    if (ddlSubjectCategory.SelectedItem.Text == CornerTypeEnum.三叶草.ToString())
                    {
                        subjectModel.CornerType = CornerTypeEnum.三叶草.ToString();
                    }
                    else
                        subjectModel.CornerType = string.Empty;
                    subjectModel.SubjectTypeId = int.Parse(ddlSubjectType.SelectedValue);
                    subjectModel.RegionOrderType = 1;
                    if (!string.IsNullOrWhiteSpace(rblRegion.SelectedValue))
                        subjectModel.Region = rblRegion.SelectedValue;
                    if (cbIsSecondInstall.Checked)
                    {
                        subjectModel.IsSecondInstall = true;
                        if (rblSecondInstallType.SelectedIndex > 0)
                            subjectModel.SecondBasicInstallPriceType = int.Parse(rblSecondInstallType.SelectedValue);
                        else
                            subjectModel.SecondBasicInstallPriceType = 1;
                    }
                    else
                    {
                        subjectModel.IsSecondInstall = false;
                        subjectModel.SecondBasicInstallPriceType = null;
                    }
                    subjectModel.SubjectType = 1;//默认是pop订单
                    if (subjectId > 0)
                    {
                        subjectBll.Update(subjectModel);
                    }
                    else
                    {
                        if (ItemId == 0)
                            ItemId = int.Parse(ddlGuidance.SelectedValue);
                        subjectModel.GuidanceId = ItemId;
                        subjectBll.Add(subjectModel);
                    }
                    tran.Complete();
                    Response.Redirect("SubjectList.aspx", false);
                }
                catch
                {
                    ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "js", "<script>alert('提交失败！')</script>", true);
                }
            }
        }

        protected void cbNoOrderList_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                Panel1.Visible = true;
                Panel2.Visible = false;
                bool hasOrder = false;
                Subject model = subjectBll.GetModel(subjectId);
                if (model != null)
                {
                    int subjectType=model.SubjectType??1;
                    switch (subjectType)
                    { 
                        case (int)SubjectTypeEnum.POP订单:
                            var mergeOrderList = new MergeOriginalOrderBLL().GetList(s => s.SubjectId == subjectId);
                            hasOrder = mergeOrderList.Any();
                            break;
                        case (int)SubjectTypeEnum.运费:
                        case (int)SubjectTypeEnum.新开店安装费:
                            var priceOrderList = new PriceOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
                            hasOrder = priceOrderList.Any();
                            break;
                        case (int)SubjectTypeEnum.手工订单:
                            var handMadeOrderList = new HandMadeOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
                            hasOrder = handMadeOrderList.Any();
                            break;
                        

                    }
                }
                if (hasOrder)
                {
                    labTipsMsg.Text = "提示：该项目已有订单信息，提交后将删除原来订单，包括拆单方案";
                }
               
                
            }
            else
            {
                Panel1.Visible = false;
                Panel2.Visible = true;
                labTipsMsg.Text = "";
            }
        }

        protected void cblOrderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //bool selected = false;
            //foreach (ListItem li in cblOrderType.Items)
            //{
            //    if (li.Selected)
            //        selected = true;
            //}
            //if (selected)
            //{
            //    Panel1.Visible = true;
            //    Panel2.Visible = false;
            //}
            //else
            //{
            //    Panel1.Visible = false;
            //    Panel2.Visible = true;
            //}
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindOrderType();
            BindGuidanceList();
            BindRegion();
            BindSubjectCategory();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("SubjectList.aspx", false);
        }

        protected void rblSubjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeSubjectType();
        }

        void ChangeSubjectType()
        {
            int orderType = 0;
            if (rblSubjectType.SelectedValue != null)
            {
                orderType = int.Parse(rblSubjectType.SelectedValue);
            }
            if (orderType == (int)SubjectTypeEnum.POP订单)
            {
                cbNoOrderList.Enabled = true;
                rblRegion.Enabled = true;
                trSecondInstall.Style.Add("display","");
            }
            else
            {
                cbNoOrderList.Checked = false;
                cbNoOrderList.Enabled = false;
                if (rblRegion.SelectedItem!=null)
                    rblRegion.SelectedItem.Selected = false;
                rblRegion.Enabled = false;
                Panel1.Visible = false;
                Panel2.Visible = true;
                
                cbIsSecondInstall.Checked = false;
                if (rblSecondInstallType.SelectedItem != null)
                    rblSecondInstallType.SelectedItem.Selected = false;
                trSecondInstall.Style.Add("display", "none");
            }
            //if (orderType == (int)SubjectTypeEnum.补单)
            //{
            //    ddlSubjectName.Visible = true;
            //    txtSubjectName.Visible = false;
            //    rblPriceBlong.Enabled = false;
            //    cbNoOrderList.Enabled = false;
            //    rblRegion.Enabled = false;
            //}
            //else
            //{
            //    ddlSubjectName.Visible = false;
            //    txtSubjectName.Visible = true;
            //    rblPriceBlong.Enabled = true;
            //    cbNoOrderList.Enabled = true;
            //    rblRegion.Enabled = true;
            //}
        }

        void BindSubject()
        {
            SubjectBLL subjectBll = new SubjectBLL();
            ddlSubjectName.Items.Clear();
            ddlSubjectName.Items.Add(new ListItem("--请选择项目--", "0"));
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            List<int> submitSubjectIdList = new List<int>();//已提交的分区订单
            
            var subjectList = subjectBll.GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && s.SubjectType == (int)SubjectTypeEnum.POP订单 && s.ApproveState == 1);

            if (subjectList.Any())
            {
                subjectList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.SubjectName;
                    ddlSubjectName.Items.Add(li);
                });
            }
        }

        void BindSBCInstallType()
        {
            var list = CommonMethod.GetEnumList<SecondInstallInstallTypeEnum>();
            list.ForEach(s => {
                ListItem li = new ListItem();
                li.Value = s.Value.ToString();
                li.Text = s.Desction + "&nbsp;&nbsp;";
                rblSecondInstallType.Items.Add(li);
            });
        }

    }
}
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
using System.Text;

namespace WebApp.Subjects
{
    public partial class GuidanceDetail : BasePage
    {
        int itemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemId"] != null)
            {
                itemId = int.Parse(Request.QueryString["itemId"]);
                GetGuidanceModel();
                BindSubjectList();
                
            }
        }

        void GetGuidanceModel()
        {
            //SubjectGuidance model = new SubjectGuidanceBLL().GetModel(itemId);
            var model = (from guidance in CurrentContext.DbContext.SubjectGuidance
                         //join type1 in CurrentContext.DbContext.ADOrderActivity
                         //on guidance.ActivityTypeId equals type1.ActivityId into typeTemp
                         join priceItem1 in CurrentContext.DbContext.CustomerMaterialPriceItem
                         on guidance.PriceItemId equals priceItem1.Id into priceItemTemp
                         from priceItem in priceItemTemp.DefaultIfEmpty()
                         //from type in typeTemp.DefaultIfEmpty()
                         join customer in CurrentContext.DbContext.Customer
                         on guidance.CustomerId equals customer.Id
                         where guidance.ItemId == itemId
                         select new
                         {
                             PriceItemName=priceItem.ItemName,
                             guidance,
                             //type.ActivityName,
                             customer.CustomerName
                         }).FirstOrDefault();
            if (model != null)
            {
                labCustomerName.Text =model.CustomerName;
                labItemName.Text = model.guidance.ItemName;
                string activityTypeName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((model.guidance.ActivityTypeId ?? 1).ToString());
                if (!string.IsNullOrWhiteSpace(activityTypeName))
                {
                    
                    StringBuilder aName = new StringBuilder(activityTypeName);
                    if (!(model.guidance.HasExperssFees ?? true))
                        aName.Append("—无快递费");
                    else if (!(model.guidance.HasInstallFees ?? true))
                        aName.Append("—无安装费");
                    labActivityType.Text = aName.ToString();

                }
                if (model.guidance.BeginDate != null)
                    labBeginDate.Text = DateTime.Parse(model.guidance.BeginDate.ToString()).ToShortDateString();
                if (model.guidance.EndDate != null)
                    labEndDate.Text = DateTime.Parse(model.guidance.EndDate.ToString()).ToShortDateString();
                if (model.guidance.GuidanceYear != null && model.guidance.GuidanceMonth != null)
                {
                    string date = model.guidance.GuidanceYear + "-" + model.guidance.GuidanceMonth;
                    if (StringHelper.IsDateTime(date))
                        labGuidanceMonth.Text = DateTime.Parse(date).ToString("yyyy-MM");
                }
                //labGuidanceMonth.Text=
                labMaterialPriceItem.Text = model.PriceItemName;
                labSubjectNames.Text = model.guidance.SubjectNames;
                var typeList = new SubjectTypeBLL().GetList(s=>s.GuidanceId==itemId && (s.IsDelete==false || s.IsDelete==null));
                if (typeList.Any())
                {
                    StringBuilder str = new StringBuilder();
                    typeList.ForEach(s => {
                        str.Append(s.SubjectTypeName);
                        str.Append("，");
                    });
                    labSubjectType.Text = str.ToString().TrimEnd('，');
                }
                labRemark.Text = model.guidance.Remark;
                 string permission = GetPromissionStr();
                 List<string> permissionList = new List<string>();
                 if (!string.IsNullOrWhiteSpace(permission))
                 {
                     permissionList = permission.Split('|').ToList();
                 }
                 
                if (model.guidance.IsFinish ?? false)
                {
                    labState.Text = "已完成";
                    if (permissionList.Contains("edit"))
                    {
                        spanOpenState.Style.Add("display", "");
                        
                    }
                }
                else
                {
                    labState.Text = "进行中";
                    labState.Style.Add("color","red");
                    if (permissionList.Contains("edit"))
                    {
                        spanChangeState.Style.Add("display", "");
                        
                    }
                    
                    if (CurrentUser.RoleId == 2)
                    {
                        btnAddSubject.Visible = true;
                    }
                }
                AttachmentBLL attachBll = new AttachmentBLL();
               
                string fileType = FileTypeEnum.Files.ToString();
                
                
                string fileCode2 = ((int)FileCodeEnum.SubjectGuidanceAttach).ToString();
                var otherAttach = attachBll.GetList(s => s.FileCode == fileCode2 && s.FileType == fileType && s.SubjectId == model.guidance.ItemId && (s.IsDelete == null || s.IsDelete == false));
                if (otherAttach.Any())
                {
                    otherAttach.ForEach(a => {
                        LinkButton lb1 = new LinkButton();
                        lb1.Text = a.Title;
                        lb1.Style.Add("margin-right","15px");
                        lb1.Style.Add("text-decoration", "underline");
                        lb1.Click += (s, e) =>
                        {
                            OperateFile.DownLoadFile(a.FilePath, a.Title);
                        };
                        Panel2.Controls.Add(lb1);
                    });
                }

            }
        }


        void BindSubjectList()
        {
            
            List<int> companyList = MySonCompanyList.Select(s => s.Id).ToList();
            
            var list = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on subject.AddUserId equals user.UserId
                        where (subject.IsDelete == null || subject.IsDelete == false) //&&  companyList.Contains(subject.CompanyId ?? 0)
                        && subject.GuidanceId==itemId
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
                            user.RealName
                        }).ToList();
            if (CurrentUser.RoleId == 2)
            {
                list = list.Where(s => s.AddUserId == CurrentUser.UserId).ToList();
            }
            
            //if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            //{
            //    list = list.Where(s => s.SubjectName.Contains(txtSubjectName.Text)).ToList();
            //}
            //if (!string.IsNullOrWhiteSpace(txtSubjectNo.Text))
            //{
            //    list = list.Where(s => s.SubjectNo.Contains(txtSubjectNo.Text)).ToList();
            //}

            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();

            List<int> roleIdList = new List<int>() {2 };
            if (roleIdList.Contains(CurrentUser.RoleId))
            {
                SetOPeratePromission(ref gv);
            }
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "EditItem")
            {
                Response.Redirect(string.Format("AddSubject.aspx?itemId={0}&subjectId={1}",itemId, id), false);

            }
            if (e.CommandName == "Check")
            {
                string url = string.Format("ShowOrderDetail.aspx?subjectId={0}", id);
                //Response.Redirect(string.Format("CheckOrderDetail.aspx?subjectId={0}", id), false);
                SubjectBLL subjectBll = new SubjectBLL();
                Models.Subject model = subjectBll.GetModel(id);
                //if (model != null && !string.IsNullOrWhiteSpace(model.SupplementRegion) && model.SubjectType != (int)SubjectTypeEnum.正常单)
                //{
                //    url = string.Format("RegionSubject/CheckOrderDetail.aspx?subjectId={0}", id);
                //}
                if (model.SubjectType == (int)SubjectTypeEnum.补单)
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
                        url = string.Format("/Subjects/SecondInstallFee/CheckDetail.aspx?subjectId={0}", id);
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
                    model.IsDelete = true;
                    subjectBll.Update(model);
                    BindSubjectList();
                }
            }
            if (e.CommandName == "Export")
            {
                Response.Redirect(string.Format("ExportOrder.aspx?subjectId={0}", id), false);
            }
            if (e.CommandName == "EditOrder")
            {
                Response.Redirect(string.Format("ADOrders/EditOrderDetails.aspx?subjectId={0}", id), false);
            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object statusObj = item.GetType().GetProperty("Status").GetValue(item, null);
                    int status = statusObj != null ? int.Parse(statusObj.ToString()) : 0;
                    Label labStatus = (Label)e.Row.FindControl("labStatus");
                    LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                    LinkButton lbExport = (LinkButton)e.Row.FindControl("lbExport");
                    LinkButton lbEditOrder = (LinkButton)e.Row.FindControl("lbEditOrder");
                    lbEdit.CommandName = "EditItem";
                    lbExport.CommandName = "";
                    lbExport.Enabled = false;
                    lbExport.Style.Add("color", "#ccc");

                    lbEditOrder.CommandName = "";
                    lbEditOrder.Enabled = false;
                    lbEditOrder.Style.Add("color", "#ccc");

                    string statusMsg = string.Empty;
                    switch (status)
                    {
                        case 1:
                            statusMsg = "<span style='color:red;'>待下单</span>";
                            //lbEdit.CommandName = "EditItem";
                            break;
                        case 2:
                            statusMsg = "<span style='color:blue;'>待拆单</span>";
                            //lbEdit.CommandName = "EditItem";
                            break;
                        case 3:
                            statusMsg = "<span style='color:red;'>待提交</span>";
                            //lbEdit.CommandName = "EditItem";
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
                            //lbEditOrder.CommandName = "EditOrder";
                            //lbEditOrder.Enabled = true;
                            //lbEditOrder.Style.Add("color", "blue");
                            break;
                        case 1:
                            approveMsg = "<span style='color:green;'>审批通过</span>";
                            lbDelete.Enabled = false;
                            lbDelete.CommandName = "";
                            lbDelete.Style.Add("color", "#ccc");

                            lbEdit.Enabled = false;
                            lbEdit.CommandName = "";
                            lbEdit.Style.Add("color", "#ccc");

                            lbExport.CommandName = "Export";
                            lbExport.Enabled = true;
                            lbExport.Style.Add("color", "blue");
                            break;
                        case 2:
                            approveMsg = "<span style='color:red;'>审批不通过</span>";
                            lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                            //lbEditOrder.CommandName = "EditOrder";
                            //lbEditOrder.Enabled = true;
                            //lbEditOrder.Style.Add("color", "blue");
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
            BindSubjectList();
        }

        protected void btnAddSubject_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddSubject.aspx?itemId="+itemId, false);
        }

        protected void btnChangeState_Click(object sender, EventArgs e)
        {
            SubjectGuidanceBLL bll = new SubjectGuidanceBLL();
            SubjectGuidance model = bll.GetModel(itemId);
            if (model != null)
            {
                bool canFinish = true;
                string msg = string.Empty;
                var subjectList = new SubjectBLL().GetList(s => s.GuidanceId == itemId && (s.IsDelete == null || s.IsDelete == false));
                if (subjectList.Any())
                {
                    subjectList = subjectList.Where(s => s.ApproveState != 1).ToList();
                    if (subjectList.Any())
                    {
                        msg = "不能完成，该活动下还有未通过审批的项目！";
                        canFinish = false;
                    }
                }
                else
                {
                    msg = "不能完成，该活动下没有项目！";
                    canFinish = false;
                }
                if (canFinish)
                {
                    
                    model.IsFinish = true;
                    model.FinishDate = DateTime.Now;
                    model.FinishUserId = CurrentUser.UserId;
                    bll.Update(model);
                    Response.Redirect("GuidanceDetail.aspx?itemId=" + itemId, false);
                    
                }
                else
                {
                    LayAlert(msg);
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("GuidanceList.aspx",false);
        }

        protected void btnOpenState_Click(object sender, EventArgs e)
        {
            SubjectGuidanceBLL bll = new SubjectGuidanceBLL();
            SubjectGuidance model = bll.GetModel(itemId);
            if (model != null)
            {
                model.IsFinish = false;
                bll.Update(model);
                Response.Redirect("GuidanceDetail.aspx?itemId=" + itemId, false);
               
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BindSubjectList();
        }
    }
}
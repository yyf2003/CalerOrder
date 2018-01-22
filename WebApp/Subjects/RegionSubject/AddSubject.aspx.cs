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

namespace WebApp.Subjects.RegionSubject
{
    public partial class AddSubject : BasePage
    {
        int subjectId;
        SubjectBLL subjectBll = new SubjectBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindMyCustomerList(ddlCustomer);
                if (subjectId == 0)
                {
                    BindOrderType();
                    BindGuidanceList();
                }
                BindRegion();
                BindData();
            }
        }

        void BindData()
        {
            Subject model = new SubjectBLL().GetModel(subjectId);
            if (model != null)
            {
                if (model.CustomerId != null)
                {
                    ddlCustomer.SelectedValue = model.CustomerId.ToString();
                    BindOrderType();
                    
                    BindRegion();
                }
                if (model.SubjectType != null)
                {
                    rblSubjectType.SelectedValue = model.SubjectType.ToString();
                    
                    ChangeSubjectType();
                    rblSubjectType.Enabled = false;
                }
                BindGuidanceList();
                txtSubjectName.Text = model.SubjectName;
                
                if (model.GuidanceId != null)
                {
                    ddlGuidance.SelectedValue = model.GuidanceId.ToString();
                    BindSubject();
                    ChangeGuidance();
                }
                if (model.HandMakeSubjectId != null)
                    ddlSubjectName.SelectedValue = model.HandMakeSubjectId.ToString();
                if (model.BeginDate != null)
                    txtBeginDate.Text = DateTime.Parse(model.BeginDate.ToString()).ToShortDateString();
                if (model.EndDate != null)
                    txtEndDate.Text = DateTime.Parse(model.EndDate.ToString()).ToShortDateString();
                
                rblRegion.SelectedValue = model.SupplementRegion;
                
                txtRemark.Text = model.Remark;
            }
        }

        void BindGuidanceList()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            DateTime now = DateTime.Now;
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && (s.IsFinish == null || s.IsFinish == false) && (s.IsDelete == null || s.IsDelete == false)).OrderByDescending(s => s.ItemId).ToList();
            int orderType = 0;
            if (rblSubjectType.SelectedValue != null)
            {
                orderType = int.Parse(rblSubjectType.SelectedValue);
                if (orderType == (int)SubjectTypeEnum.分区增补 || orderType == (int)SubjectTypeEnum.新开店订单)
                {
                    list = list.Where(s => s.ActivityTypeId == (int)GuidanceTypeEnum.Others).ToList();
                }
                else
                {
                    list = list.Where(s => s.ActivityTypeId != (int)GuidanceTypeEnum.Others).ToList();
                }
            }
            //if (subjectId == 0)
            //{
                //DateTime date = DateTime.Now;
                //DateTime newDate = new DateTime(date.Year, date.Month, 1);
                //DateTime beginDate = newDate.AddMonths(-5);
                //DateTime endDate = newDate.AddMonths(2);
                //list = list.Where(s => s.EndDate >= date).ToList();
                //list = list.Where(s => s.BeginDate >= beginDate && s.BeginDate < endDate).ToList();
            //}
            DateTime date = DateTime.Now;
            list = list.Where(s => s.EndDate >= date).ToList();
            if (list.Any())
            {
                ddlGuidance.DataSource = list;
                ddlGuidance.DataTextField = "ItemName";
                ddlGuidance.DataValueField = "ItemId";
                ddlGuidance.DataBind();
                ddlGuidance.Items.Insert(0, new ListItem("请选择", "0"));

                //if (subjectId > 0)
                //    ddlGuidance.Enabled = false;
            }

            //rblSubjectType.Items.Clear();
            //List<EnumEntity> list1 = CommonMethod.GetEnumList<SubjectTypeEnum>().Where(s => s.Desction == OrderChannelEnum.分区订单.ToString()).ToList();
            //if (list1.Any())
            //{
            //    int index = 0;
            //    list1.ForEach(s =>
            //    {
            //        ListItem li = new ListItem();
            //        li.Value = s.Value.ToString();
            //        li.Text = s.Name + "&nbsp;";
            //        if (index == 0)
            //            li.Selected = true;
            //        rblSubjectType.Items.Add(li);
            //        index++;
            //    });
            //}
        }


        void BindOrderType()
        {
            rblSubjectType.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = (from corderType in CurrentContext.DbContext.CustomerOrderType
                        join orderType in CurrentContext.DbContext.OrderType
                        on corderType.OrderTypeId equals orderType.Id
                        where corderType.CustomerId == customerId
                        && ((orderType.LevelNum == 2 || orderType.LevelNum == 3))
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



        void BindSubject()
        {
            SubjectBLL subjectBll=new SubjectBLL();
            ddlSubjectName.Items.Clear();
            ddlSubjectName.Items.Add(new ListItem("--请选择项目--", "0"));
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            List<int> submitSubjectIdList = new List<int>();//已提交的分区订单
            //submitSubjectIdList = subjectBll.GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && s.SubjectType == (int)SubjectTypeEnum.正常单).Select(s=>s.HandMakeSubjectId??0).ToList();

            //var subjectList = subjectBll.GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && s.SubjectType == (int)SubjectTypeEnum.POP订单 && !submitSubjectIdList.Contains(s.Id) && s.ApproveState==1);
            var subjectList = subjectBll.GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && s.SubjectType == (int)SubjectTypeEnum.POP订单 && s.ApproveState == 1);
            
            if (subjectList.Any())
            {
                subjectList.ForEach(s => {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.SubjectName;
                    ddlSubjectName.Items.Add(li);
                });
            }
        }
        

        void BindRegion()
        {
            rblRegion.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<string> myRegions = GetResponsibleRegion;
            if (!myRegions.Any())
            {
                myRegions = new RegionBLL().GetList(s => s.CustomerId == customerId).Select(s => s.RegionName).Distinct().ToList();
            }
            if (myRegions.Any())
            {
                myRegions.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    rblRegion.Items.Add(li);
                });
            }
        }

       

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubject();
            ChangeGuidance();
        }

        void ChangeGuidance()
        {
            int itemId = int.Parse(ddlGuidance.SelectedValue);
            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(itemId);
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
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRegion();
            BindGuidanceList();
        }

      
        

        protected void btnNext_Click(object sender, EventArgs e)
        {
            labMsg.Text = "";
            Subject subjectModel;
            int realSubjectId = 0;
            int subjectType = int.Parse(rblSubjectType.SelectedValue);
            string subjectName =new System.Text.RegularExpressions.Regex("[\\s]+").Replace(txtSubjectName.Text.Trim()," ");
            if (subjectType == (int)SubjectTypeEnum.正常单)
            {
                realSubjectId = int.Parse(ddlSubjectName.SelectedValue);
                subjectName = ddlSubjectName.SelectedItem.Text +"—"+ txtRemark.Text;
            }
            else
            {
                if (CheckSubject(txtSubjectName.Text.Trim(), subjectId))
                {
                    labMsg.Text = "项目名称重复";
                    return;
                }
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
                subjectModel.Status = 3;
                
                subjectModel.SubjectNo = CreateSubjectNo();
                subjectModel.CompanyId = CurrentUser.CompanyId;
                subjectModel.ApproveState = 0;
                if (subjectType == (int)SubjectTypeEnum.正常单)
                {
                    subjectModel.Status = 1;
                    Subject oldSubjectModel = subjectBll.GetModel(realSubjectId);
                    if (oldSubjectModel != null)
                    {
                        subjectModel.ActivityId = oldSubjectModel.ActivityId;
                        subjectModel.AddOrderType = oldSubjectModel.AddOrderType;
                        subjectModel.SubjectCategoryId = oldSubjectModel.SubjectCategoryId;
                        subjectModel.SubjectTypeId = oldSubjectModel.SubjectTypeId;
                        subjectModel.CornerType = oldSubjectModel.CornerType;
                    }


                }
            }
            subjectModel.BeginDate = DateTime.Parse(txtBeginDate.Text.Trim());

            subjectModel.CustomerId = int.Parse(ddlCustomer.SelectedValue);
            subjectModel.EndDate = DateTime.Parse(txtEndDate.Text.Trim());
            subjectModel.Remark = txtRemark.Text;
            subjectModel.SubjectName = subjectName;
            
            subjectModel.SupplementRegion = rblRegion.SelectedValue;
            subjectModel.HandMakeSubjectId = realSubjectId;
            subjectModel.SubjectType = subjectType;//
            subjectModel.GuidanceId = int.Parse(ddlGuidance.SelectedValue);
            if (subjectId > 0)
            {
                subjectBll.Update(subjectModel);
            }
            else
            {
                
                subjectBll.Add(subjectModel);
            }
            string url = string.Format("AddOrderDetail.aspx?subjectId=" + subjectModel.Id);
            if (subjectType == (int)SubjectTypeEnum.二次安装)
                url = "/Subjects/SecondInstallFee/AddOrderDetail.aspx?subjectId=" + subjectModel.Id;
            if (subjectType==(int)SubjectTypeEnum.分区补单)
                url="/Subjects/SupplementByRegion/ImportOrder.aspx?subjectId=" + subjectModel.Id;
            if (subjectType == (int)SubjectTypeEnum.正常单)
                url = "/Subjects/ADOrders/ImportOrder.aspx?fromRegion=1&subjectId=" + subjectModel.Id + "&realSubjectId=" + realSubjectId;
            if (subjectType == (int)SubjectTypeEnum.分区增补 || subjectType == (int)SubjectTypeEnum.新开店订单)
                url = string.Format("AddNewShopOrder.aspx?subjectId=" + subjectModel.Id);
            if (subjectType == (int)SubjectTypeEnum.费用订单)
                url = string.Format("/Subjects/PriceOrder/AddOrderDetail.aspx?subjectId=" + subjectModel.Id);

            Response.Redirect(url, false);
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("List.aspx",false);
        }

        /// <summary>
        /// 检查项目名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool CheckSubject(string name, int id)
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            name = StringHelper.ReplaceSpace(name);
            var list = subjectBll.GetList(s => s.SubjectName.Replace(" ", "").ToLower() == name.ToLower() && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1) && (s.IsDelete == null || s.IsDelete == false));

            return list.Any();
        }

        protected void rblSubjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidanceList();
            ChangeSubjectType();
        }

        void ChangeSubjectType()
        {
            int orderType = 0;
            if (rblSubjectType.SelectedValue != null)
            {
                orderType = int.Parse(rblSubjectType.SelectedValue);
            }
            if (orderType == (int)SubjectTypeEnum.正常单)
            {
                ddlSubjectName.Visible = true;
                txtSubjectName.Visible = false;
            }
            else
            {
                ddlSubjectName.Visible = false;
                txtSubjectName.Visible = true;
            }
        }

       
    }
}
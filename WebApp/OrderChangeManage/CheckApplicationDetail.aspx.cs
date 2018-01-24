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

namespace WebApp.OrderChangeManage
{
    public partial class CheckApplicationDetail : BasePage
    {
        int applicationId;
        bool applicationIsDelete = false;
        bool applicationIsApprove = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                applicationId = int.Parse(Request.QueryString["id"]);
            }
            if (!IsPostBack)
            {
                BindAppliction();
                BindDetail();
                
            }
        }

        void BindAppliction()
        {
            var applicationModel = (from app in CurrentContext.DbContext.OrderChangeApplication
                                    join user in CurrentContext.DbContext.UserInfo
                                    on app.AddUserId equals user.UserId
                                    join guidance in CurrentContext.DbContext.SubjectGuidance
                                    on app.SubjectGuidanceId equals guidance.ItemId
                                    where app.Id == applicationId
                                    select new
                                    {
                                        app,
                                        user.RealName,
                                        guidance.ItemName
                                    }).FirstOrDefault();
            if (applicationModel != null)
            {
                
                labGuidanceName.Text = applicationModel.ItemName;
                labAddUser.Text = applicationModel.RealName;
                if (applicationModel.app.AddDate!=null)
                   labAddDate.Text = applicationModel.app.AddDate.ToString();
                applicationIsDelete=applicationModel.app.IsDelete ?? false;
                applicationIsApprove = applicationModel.app.ManagerApperoveState==1;
                if (applicationIsDelete)
                {
                    labApproveState.Text = "已删除";
                }
                GetApproveInfo();
            }
        }

        void BindDetail()
        {
            var list = (from order in CurrentContext.DbContext.OrderChangeApplicationDetail
                       join subject in CurrentContext.DbContext.Subject
                       on order.SubjectId equals subject.Id
                       join user1 in CurrentContext.DbContext.UserInfo
                       on order.FinishUserId equals user1.UserId into temp
                        from user in temp.DefaultIfEmpty()
                       where order.ApplicationId == applicationId
                       select new {
                           order,
                           subject.SubjectName,
                           order.ChangeType,
                           user.RealName,
                           order.State,
                           order.Id,
                           order.SubjectId,
                           order.Running
                       }).ToList();
            Repeater1.DataSource = list;
            Repeater1.DataBind();
        }

        /// <summary>
        /// 显示审批记录
        /// </summary>
        void GetApproveInfo()
        {
            var list = (from approve in CurrentContext.DbContext.OrderChangeApproveInfo
                        join user in CurrentContext.DbContext.UserInfo
                        on approve.AddUserId equals user.UserId
                        where approve.SubjectId == applicationId
                        select new
                        {
                            approve,
                            UserName = user.RealName,
                        }).ToList();
            if (list.Any())
            {
                Dictionary<int, string> approveStateDic = new Dictionary<int, string>();
                approveStateDic.Add(0, "待审批");
                approveStateDic.Add(1, "<span style='color:green;'>审批通过</span>");
                approveStateDic.Add(2, "<span style='color:red;'>审批不通过</span>");
                //StringBuilder tb = new StringBuilder();
                //list.ForEach(s =>
                //{
                //    int approveState = s.approve.Result ?? 0;
                //    tb.Append("<table class=\"table\" style=\"margin-bottom:6px;\">");
                //    tb.AppendFormat("<tr class=\"tr_bai\"><td style=\"width: 100px;\">审批时间</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", s.approve.AddDate);
                //    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批结果</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", approveStateDic[approveState]);
                //    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批人</td><td style=\"text-align: left; padding-left: 5px;\">{0}</td></tr>", s.UserName);
                //    tb.AppendFormat("<tr class=\"tr_bai\"><td>审批意见</td><td style=\"text-align: left; padding-left: 5px;height: 60px;\">{0}</td></tr>", s.approve.Remark);
                //    tb.Append("</table>");


                //});
                //approveInfoDiv.InnerHtml = tb.ToString();
                //Panel_ApproveInfo.Visible = true;
                labApproveState.Text = approveStateDic[(list[0].approve.Result ?? 0)];
                labApproveUserName.Text = list[0].UserName;
                labApproveDate.Text = list[0].approve.AddDate.ToString();
                labApproveRemark.Text = list[0].approve.Remark;
            }
            else
            {
                labApproveState.Text = "未审批";
            }
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object idObj = item.GetType().GetProperty("Id").GetValue(item,null);
                    int Id=idObj!=null?int.Parse(idObj.ToString()):0;
                    object subjectIdObj = item.GetType().GetProperty("SubjectId").GetValue(item, null);
                    int subjectId = subjectIdObj != null ? int.Parse(subjectIdObj.ToString()) : 0;
                    object changeTypeObj = item.GetType().GetProperty("ChangeType").GetValue(item,null);
                    string changeType = changeTypeObj != null ? changeTypeObj.ToString() : "";

                    object runningObj = item.GetType().GetProperty("Running").GetValue(item, null);
                    bool running = runningObj != null ? bool.Parse(runningObj.ToString()) : false;

                    ((Label)e.Item.FindControl("labChangeType")).Text = CommonMethod.GetEnumDescription<OrderChangeTypeEnum>(changeType);

                    object stateObj = item.GetType().GetProperty("State").GetValue(item, null);
                    int state = stateObj != null ? int.Parse(stateObj.ToString()) : 0;
                    Label labChangeState = (Label)e.Item.FindControl("labChangeState");

                    Panel panel1 = (Panel)e.Item.FindControl("Panel1");
                    Panel panel2 = (Panel)e.Item.FindControl("Panel2");
                    //Label labOperateType = (Label)e.Item.FindControl("labOperateType");
                    if (applicationIsDelete)
                    {
                        labChangeState.Text = "已删除";
                        labChangeState.ForeColor = System.Drawing.Color.Red;
                        panel1.Visible = false;
                        panel2.Visible = true;
                    }
                    else
                    {
                        if (applicationIsApprove)
                        {
                            switch (state)
                            {
                                case 1:
                                    labChangeState.Text = "进行中";
                                    labChangeState.ForeColor = System.Drawing.Color.Red;

                                    break;
                                case 2:
                                    labChangeState.Text = "完成";
                                    labChangeState.ForeColor = System.Drawing.Color.Green;
                                    panel1.Visible = false;
                                    panel2.Visible = true;

                                    break;
                                case 3:
                                    labChangeState.Text = "已撤销";
                                    labChangeState.ForeColor = System.Drawing.Color.Red;
                                    panel1.Visible = false;
                                    panel2.Visible = true;

                                    break;
                            }
                        }
                        else
                        {
                            
                            panel1.Visible = false;
                        }
                    }
                }
            }
        }

        protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "Excute")
            {
                
                OrderChangeApplicationDetail model = new OrderChangeApplicationDetailBLL().GetModel(id);
                if (model != null && model.ChangeType>0)
                {
                    int subjectId = model.SubjectId;
                    string url = string.Format("/Subjects/SubjectList.aspx?subjectId={0}&operateType={1}&applicationDetailId={2}", model.SubjectId, model.ChangeType, id);
                    Subject subjectModel = new SubjectBLL().GetModel(subjectId);
                    if (subjectModel != null)
                    {
                        if (subjectModel.SubjectType == (int)SubjectTypeEnum.HC订单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区补单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区增补 || subjectModel.SubjectType == (int)SubjectTypeEnum.新开店订单)
                        {
                            //url = "../Subjects/RegionSubject/List.aspx";
                            url = string.Format("/Subjects/RegionSubject/List.aspx?subjectId={0}&operateType={1}&applicationDetailId={2}", model.SubjectId, model.ChangeType, id);
                        }
                    }
                    
                    Response.Redirect(url,false);
                }
            }
            if (e.CommandName == "Cancel")
            { 
               OrderChangeApplicationDetail model = new OrderChangeApplicationDetailBLL().GetModel(id);
               if (model != null)
               {
                   model.State = 3;//撤销
                   model.FinishUserId = CurrentUser.UserId;
                   model.FinishDate = DateTime.Now;
                   new OrderChangeApplicationDetailBLL().Update(model);
                   BindDetail();
               }
            }
        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ApplicationList.aspx",false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using BLL;
using DAL;
using Models;
using Common;

namespace WebApp.OrderChangeManage
{
    public partial class HandleSubjectList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSubjectList();
            }
        }

        void BindSubjectList() {
            var list = (from detail in CurrentContext.DbContext.OrderChangeApplicationDetail
                       join subject in CurrentContext.DbContext.Subject
                       on detail.SubjectId equals subject.Id
                       join application in CurrentContext.DbContext.OrderChangeApplication
                       on detail.ApplicationId equals application.Id
                       join guidance in CurrentContext.DbContext.SubjectGuidance
                       on application.SubjectGuidanceId equals guidance.ItemId
                       where application.AddUserId == CurrentUser.UserId
                       && (application.IsDelete == null || application.IsDelete==false)
                       && (detail.State ?? 0) < 2
                       select new {
                           guidance,
                           application,
                           detail,
                           detail.State,
                           detail.ChangeType,
                           subject
                       }).ToList();
            if (!string.IsNullOrWhiteSpace(txtSubjectNo.Text))
            {
                string subjectNo = txtSubjectNo.Text.Trim().ToUpper();
                list = list.Where(s => s.subject.SubjectNo.ToUpper().Contains(subjectNo)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                string subjectName = txtSubjectName.Text.Trim().ToUpper();
                list = list.Where(s => s.subject.SubjectName.ToUpper().Contains(subjectName)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.subject.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindSubjectList();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindSubjectList();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "Excute")
            {

                OrderChangeApplicationDetail model = new OrderChangeApplicationDetailBLL().GetModel(id);
                if (model != null && model.ChangeType > 0)
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

                    Response.Redirect(url, false);
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
                    object ChangeTypeObj = item.GetType().GetProperty("ChangeType").GetValue(item,null);
                    string ChangeType = ChangeTypeObj != null ? ChangeTypeObj.ToString() : "";
                    ((Label)e.Row.FindControl("labChangeType")).Text = CommonMethod.GetEnumDescription<OrderChangeTypeEnum>(ChangeType);

                    object StateObj = item.GetType().GetProperty("State").GetValue(item, null);
                    int state = StateObj != null ? int.Parse(StateObj.ToString()) : 0;
                    Label labState = (Label)e.Row.FindControl("labStatus");
                    switch (state)
                    { 
                        case 0:
                            labState.Text = "待处理";
                            break;
                        case 1:
                            labState.Text = "处理中";
                            break;
                    }
                }
            }
        }
    }
}
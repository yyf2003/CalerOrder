using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Common;

namespace WebApp.Subjects
{
    public partial class ApproveList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                BindGuidance();
                BindData();

            }
        }

        void BindGuidance()
        {
            int approveState = int.Parse(rblApproveState.SelectedValue);
            List<int> customerIds = CurrentUser.Customers.Select(u => u.CustomerId).ToList();
            List<int> companyList = MySonCompanyList.Select(s => s.Id).ToList();
            var list = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join guidance in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance.ItemId
                        where subject.Status == 4 && companyList.Contains(subject.CompanyId ?? 0) && customerIds.Contains(subject.CustomerId ?? 0) && (approveState == 0 ? (subject.ApproveState == 0 || subject.ApproveState == null) : subject.ApproveState > 0) && (subject.IsDelete == null || subject.IsDelete == false)
                        select guidance).ToList();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            if (customerId > 0)
            {
                list = list.Where(s => s.CustomerId == customerId).ToList();
            }
            cblGuidanceNames.Items.Clear();
            if (list.Any())
            {
                list.Distinct().OrderBy(s=>s.ItemId).ToList().ForEach(s => {
                    ListItem li = new ListItem();
                    li.Text = s.ItemName;
                    li.Value = s.ItemId.ToString();
                    cblGuidanceNames.Items.Add(li);
                });
            }
        }

        void BindData()
        {
            //审批状态
            int approveState = int.Parse(rblApproveState.SelectedValue);
            List<int> customerIds=CurrentUser.Customers.Select(u => u.CustomerId).ToList();
            List<int> companyList = MySonCompanyList.Select(s => s.Id).ToList();
            var list = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on subject.AddUserId equals user.UserId
                        join guidance1 in CurrentContext.DbContext.SubjectGuidance
                        on subject.GuidanceId equals guidance1.ItemId into guidanceTemp
                        from guidance in guidanceTemp.DefaultIfEmpty()
                        where subject.Status == 4 && companyList.Contains(subject.CompanyId ?? 0) && customerIds.Contains(subject.CustomerId ?? 0) && (approveState == 0 ? (subject.ApproveState == 0 || subject.ApproveState == null) : subject.ApproveState > 0) && (subject.IsDelete == null || subject.IsDelete == false)
                        select new
                        {
                            subject.Id,
                            subject.AddDate,
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
                            customer.CustomerName,
                            user.RealName,
                            SubjectType=subject.SubjectType??1,
                            guidance.ItemName,
                            subject.GuidanceId,
                            subject.CustomerId
                        }).ToList();
            //cblGuidanceNames.Items.Clear();
            
            if (!IsPostBack)
            {
                HttpCookie cookie = Request.Cookies["ApproveCondition"];
                List<int> guidanceList0 = new List<int>();
                if (cookie != null)
                {
                    string condition = cookie.Value;
                    if (!string.IsNullOrWhiteSpace(condition))
                    {
                        string[] str = condition.Split('&');
                        tbSubjectName.Text = str[0].Split(':')[1];
                        tbSubjectNo.Text = str[1].Split(':')[1];
                        string guidances = str[2].Split(':')[1];
                        if (!string.IsNullOrWhiteSpace(guidances))
                        {
                            guidanceList0 = StringHelper.ToIntList(guidances, ',');

                        }
                    }
                    cookie.Expires = DateTime.Now.AddSeconds(-1);
                    Response.Cookies.Add(cookie);
                }
                //if (list.Any())
                //{
                //    List<int> guidanceIds = new List<int>();
                //    list.ForEach(s =>
                //    {
                //        if ((s.GuidanceId ?? 0) > 0 && !guidanceIds.Contains(s.GuidanceId ?? 0))
                //        {
                //            guidanceIds.Add(s.GuidanceId ?? 0);
                //            ListItem li = new ListItem();
                //            li.Value = (s.GuidanceId ?? 0).ToString();
                //            li.Text = s.ItemName + "&nbsp;&nbsp;";
                //            guidanceList0.ForEach(g => {
                //                if (g == (s.GuidanceId ?? 0))
                //                    li.Selected = true;
                //            });
                //            cblGuidanceNames.Items.Add(li);
                //        }
                //    });
                //}
            }

            int customerId = int.Parse(ddlCustomer.SelectedValue);
            if (customerId > 0)
            {
                list = list.Where(s => s.CustomerId == customerId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(txtGuidanceName.Text))
            {
                list = list.Where(s =>s.ItemName!=null && s.ItemName!="" && s.ItemName.ToLower().Contains(txtGuidanceName.Text.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(tbSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.ToLower().Contains(tbSubjectName.Text.ToLower().Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(tbSubjectNo.Text))
            {
                list = list.Where(s => s.SubjectNo.ToLower() == tbSubjectNo.Text.ToLower().Trim()).ToList();
            }
            List<int> guidanceList = new List<int>();
            foreach(ListItem item in cblGuidanceNames.Items)
            {
                if (item.Selected)
                {
                    guidanceList.Add(int.Parse(item.Value));
                }
            }
            if (guidanceList.Any())
            {
                list = list.Where(s => guidanceList.Contains(s.GuidanceId??0)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.ApproveState).ThenByDescending(s=>s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataKeyNames = new String[] { "Id", "SubjectType" };
            gv.DataBind();
            Button btn = new Button();
            SetPromission(gv);
            if (approveState == 0)
            {
                gv.Columns[gv.Columns.Count - 2].Visible = false;
                gv.Columns[gv.Columns.Count - 1].Visible = true;
            }
            else
            {
                gv.Columns[gv.Columns.Count - 2].Visible = true;
                gv.Columns[gv.Columns.Count - 1].Visible = false;
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            DataKey keys = gv.DataKeys[index];
            int id = int.Parse(keys.Values[0].ToString());
            int subjectType = int.Parse(keys.Values[1].ToString());
            if (e.CommandName == "Check")
            {
                
                string url = string.Format("ShowOrderDetail.aspx?subjectId={0}", id);
                if (subjectType == (int)SubjectTypeEnum.补单)
                    url = string.Format("HandMadeOrder/CheckDetail.aspx?subjectId={0}", id);
                else if (subjectType == (int)SubjectTypeEnum.二次安装)
                    url = string.Format("SecondInstallFee/CheckDetail.aspx?subjectId={0}", id);
                Response.Redirect(url, false);
            }
            if (e.CommandName == "Approve")
            {
                string subjectName = tbSubjectName.Text.Trim();
                string subjectNo = tbSubjectNo.Text.Trim();
                List<int> guidanceList = new List<int>();
                foreach (ListItem item in cblGuidanceNames.Items)
                {
                    if (item.Selected)
                    {
                        guidanceList.Add(int.Parse(item.Value));
                    }
                }
                bool saveCondition = !string.IsNullOrWhiteSpace(subjectName) || !string.IsNullOrWhiteSpace(subjectNo) || guidanceList.Any();
                if (saveCondition)
                {
                    string condition = string.Format("subjectName:{0}&subjectNo:{1}&guidanceId:{2}", subjectName, subjectNo, StringHelper.ListToString(guidanceList));
                    HttpCookie cookie = new HttpCookie("ApproveCondition");
                    cookie.Value = condition;
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);
                }
                
                string url = string.Format("Approve.aspx?subjectId={0}", id);
                if (subjectType==(int)SubjectTypeEnum.二次安装)
                    url = string.Format("SecondInstallFee/SubjectApprove.aspx?subjectId={0}", id);
                else if (subjectType == (int)SubjectTypeEnum.补单)
                    url = string.Format("HandMadeOrder/Approve.aspx?subjectId={0}", id);
                else if (subjectType == (int)SubjectTypeEnum.费用订单)
                    url = string.Format("PriceOrder/Approve.aspx?subjectId={0}", id);
                else if (subjectType == (int)SubjectTypeEnum.道具订单)
                    url = string.Format("/PropSubject/Approve.aspx?subjectId={0}", id);
                Response.Redirect(url, false);
            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object subjectTypeObj = item.GetType().GetProperty("SubjectType").GetValue(item, null);
                    string subjectType = subjectTypeObj != null ? subjectTypeObj.ToString() : "1";
                    Label labSubjectType = (Label)e.Row.FindControl("labSubjectType");
                    labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType);


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
                }
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }
    }
}
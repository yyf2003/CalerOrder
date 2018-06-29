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

namespace WebApp.Subjects
{
    public partial class GuidanceList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                BindAddType();
                BindActivityType();
                BindData();
            }
        }

        void BindAddType()
        {
            var enumList = CommonMethod.GetEnumList<GuidanceAddTypeEnum>();
            enumList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Text = s.Desction + "&nbsp;&nbsp;";
                li.Value = s.Value.ToString();
                cblAddType.Items.Add(li);
            });
        }

        void BindActivityType()
        {
            var enumList = CommonMethod.GetEnumList<GuidanceTypeEnum>();
            enumList.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Text = s.Desction + "&nbsp;&nbsp;";
                li.Value = s.Value.ToString();
                cblActivityType.Items.Add(li);
            });
        }

        void BindData()
        {
            List<int> curstomerList = new List<int>();
            foreach (ListItem item in ddlCustomer.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }

            List<int> addTypeList = new List<int>();
            foreach (ListItem item in cblAddType.Items)
            {
                if (item.Selected)
                    addTypeList.Add(int.Parse(item.Value));
            }

            List<int> activityTypeList = new List<int>();
            foreach (ListItem item in cblActivityType.Items)
            {
                if (item.Selected)
                    activityTypeList.Add(int.Parse(item.Value));
            }
            
            var list = (from item in CurrentContext.DbContext.SubjectGuidance
                       join user1 in CurrentContext.DbContext.UserInfo
                       on item.AddUserId equals user1.UserId into userTemp
                       join activityType1 in CurrentContext.DbContext.ADOrderActivity
                       on item.ActivityTypeId equals activityType1.ActivityId into activityTypeTemp
                       from user in userTemp.DefaultIfEmpty()
                       from activityType in activityTypeTemp.DefaultIfEmpty()
                       join customer in CurrentContext.DbContext.Customer
                       on item.CustomerId equals customer.Id
                       where (curstomerList.Any() ? (curstomerList.Contains(item.CustomerId ?? 0)) : 1 == 1)
                       select new {
                           item.AddType,
                           item.ItemId,
                           item.CustomerId,
                           item.BeginDate,
                           item.AddDate,
                           item.AddUserId,
                           item.ItemName,
                           item.Remark,
                           item.SubjectNames,
                           item.EndDate,
                           AddUserName=user.RealName,
                           activityType.ActivityName,
                           item.ActivityTypeId,
                           item.IsFinish,
                           item.IsDelete,
                           customer.CustomerName
                       }).ToList();
            if (CurrentUser.RoleId != 3)
            {
                list = list.Where(s => (s.IsDelete == null || s.IsDelete == false)).ToList();
            }
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtGuidanceName.Text.Trim()))
            {
                list = list.Where(s => s.ItemName.ToUpper().Contains(txtGuidanceName.Text.Trim().ToUpper())).ToList();
            }
            if (addTypeList.Any())
            {
                list = list.Where(s => addTypeList.Contains(s.AddType??1)).ToList();
            }
            if (activityTypeList.Any())
            {
                list = list.Where(s => activityTypeList.Contains(s.ActivityTypeId ?? 0)).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.ItemId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv);
            
        }

        protected void btnSearch_Click(object sender, EventArgs e)
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
            int itemId = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "Check")
            {
                Response.Redirect("GuidanceDetail.aspx?itemId="+itemId,false);
            }
            if (e.CommandName == "EditOrder")
            {
                Response.Redirect("AddGuidance.aspx?itemId=" + itemId, false);
            }
            if (e.CommandName == "DeleteItem")
            {
                SubjectGuidanceBLL bll = new SubjectGuidanceBLL();
                SubjectGuidance model = bll.GetModel(itemId);
                if (model != null)
                {

                    model.IsDelete = !(model.IsDelete??false);
                    model.DeleteDate = DateTime.Now;
                    model.DeleteUserId = CurrentUser.UserId;
                    bll.Update(model);
                    BindData();
                }
            }
          
        }


        SubjectBLL subjectBll = new SubjectBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object IdObj = item.GetType().GetProperty("ItemId").GetValue(item, null);
                    int Id = IdObj != null ? int.Parse(IdObj.ToString()) :0;

                    object addTypeObj = item.GetType().GetProperty("AddType").GetValue(item, null);
                    int addType = addTypeObj != null ? int.Parse(addTypeObj.ToString()) : 1;
                    Label labAddTypeName = (Label)e.Row.FindControl("labAddTypeName");
                    labAddTypeName.Text = CommonMethod.GetEnumDescription<GuidanceAddTypeEnum>(addType.ToString());


                    object typeIdObj = item.GetType().GetProperty("ActivityTypeId").GetValue(item, null);
                    int typeId = typeIdObj != null ? int.Parse(typeIdObj.ToString()) : 1;
                    //Label labInstallPrice = (Label)e.Row.FindControl("labInstallPrice");
                    Label labActivityName = (Label)e.Row.FindControl("labActivityName");
                    labActivityName.Text = CommonMethod.GetEnumDescription<GuidanceTypeEnum>(typeId.ToString());
                    
                    object IsFinishObj = item.GetType().GetProperty("IsFinish").GetValue(item, null);
                    bool IsFinish = IsFinishObj != null ? bool.Parse(IsFinishObj.ToString()) : false;
                    Label labStatus = (Label)e.Row.FindControl("labStatus");
                    if (IsFinish)
                    {
                        labStatus.Text = "已完成";
                    }
                    else
                    {
                        labStatus.Text = "进行中";
                        labStatus.Style.Add("color","green");
                    }
                    LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");
                    var subjectList = subjectBll.GetList(s => s.GuidanceId == Id && (s.IsDelete==null || s.IsDelete==false));
                    if (subjectList.Any())
                    {
                        
                        
                        lbDelete.ForeColor = System.Drawing.Color.Gray;
                        lbDelete.Enabled = false;
                        if (IsFinish)
                        {
                            lbEdit.ForeColor = System.Drawing.Color.Gray;
                            lbEdit.Enabled = false;
                        }
                    }
                    else
                    {
                        lbEdit.ForeColor = System.Drawing.Color.Blue;
                        object IsDeleteObj = item.GetType().GetProperty("IsDelete").GetValue(item, null);
                        bool IsDelete = IsDeleteObj != null ? bool.Parse(IsDeleteObj.ToString()) : false;
                        if (IsDelete)
                        {
                            labStatus.Text = "已删除";
                            labStatus.Style.Add("color", "red");
                            lbDelete.Text = "恢复";
                            lbDelete.Attributes.Add("OnClick", "return confirm('确定恢复吗？')");
                            lbDelete.ForeColor = System.Drawing.Color.Blue;
                        }
                        else
                        {
                            lbDelete.Text = "删除";
                            lbDelete.Attributes.Add("OnClick", "return confirm('确定删除吗？')");
                            lbDelete.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
        }
    }
}
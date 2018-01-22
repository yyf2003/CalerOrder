using System;
using System.Linq;
using BLL;
using DAL;
using Models;

namespace WebApp.Subjects.Supplement
{
    public partial class CheckDetail : BasePage
    {
        int itemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["supplementId"] != null)
            {
                itemId = int.Parse(Request.QueryString["supplementId"]);
            }

            if (!IsPostBack)
            {

                BindSubject();
                BindOrder();
                if (Request.QueryString["check"] != null && Request.QueryString["check"].ToString() == "1")
                {
                    PanelTitle.Visible = true;
                    PanelBackButton.Visible = true;
                }
            }
        }

        void BindSubject()
        {
            
            var model = (from supplement in CurrentContext.DbContext.SupplementItem
                         join subject in CurrentContext.DbContext.Subject
                         on supplement.SubjectId equals subject.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on supplement.AddUserId equals user.UserId
                         join user1 in CurrentContext.DbContext.UserInfo
                         on supplement.ApproveUserId equals user1.UserId into approveUser
                         from approveUserTemp in approveUser.DefaultIfEmpty()
                         where supplement.SupplementId == itemId
                         select new
                         {
                             supplement,
                             subject.SubjectName,
                             subject.SubjectNo,
                             user.RealName,
                             ApproveUserName = approveUserTemp.RealName
                         }).FirstOrDefault();
            if (model != null)
            {
                labSubjectName.Text = model.SubjectName;
                labSubjectNo.Text = model.SubjectNo;
                labPrice.Text = model.supplement.Price != null ? model.supplement.Price.ToString() : "";
                labUserName.Text = model.RealName;
                labAddDate.Text = model.supplement.AddDate != null ? model.supplement.AddDate.ToString() : "";
                if (model.supplement.ApproveState != null && model.supplement.ApproveState > 0)
                {
                    Panel1.Visible = true;
                    labApproveResult.Text = model.supplement.ApproveState == 1 ? "通过" : "不通过";
                    labRemark.Text = model.supplement.ApproveRemark;
                    labApproveUserName.Text = model.ApproveUserName;
                    labApproveDate.Text = model.supplement.ApproveDate != null ? model.supplement.ApproveDate.ToString() : "";
                }
            }
            
        }

        void BindOrder()
        {
            var list = (from order in CurrentContext.DbContext.SupplementDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.ItemId == itemId
                        select new
                        {
                            order,
                            shop
                        }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPOP.DataSource = list.OrderByDescending(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPOP.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindOrder();
        }

       
    }
}
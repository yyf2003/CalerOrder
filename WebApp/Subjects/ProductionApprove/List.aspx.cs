using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;

namespace WebApp.Subjects.ProductionApprove
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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

            var list = (from item in CurrentContext.DbContext.SubjectGuidance
                        join user1 in CurrentContext.DbContext.UserInfo
                        on item.AddUserId equals user1.UserId into userTemp
                        from user in userTemp.DefaultIfEmpty()
                        where curstomerList.Any() ? (curstomerList.Contains(item.CustomerId ?? 0)) : 1 == 1
                        select new
                        {
                            item.ItemId,
                            item.CustomerId,
                            item.BeginDate,
                            item.AddDate,
                            item.AddUserId,
                            item.ItemName,
                            item.Remark,
                            item.SubjectNames,
                            item.EndDate,
                            AddUserName = user.RealName

                        }).ToList();
            if (ddlCustomer.SelectedValue != "0")
            {
                int cid = int.Parse(ddlCustomer.SelectedValue);
                list = list.Where(s => s.CustomerId == cid).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.ItemId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();

            SetPromission(gv);

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using System.Text;
using Common;

namespace WebApp
{
    public partial class index : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                labUserName.Text = CurrentUser.LoginName+"，";
            GetMenu();
        }

        void GetMenu()
        {
            var list = (from rm in CurrentContext.DbContext.RoleInModule
                       join module in CurrentContext.DbContext.Module
                       on rm.ModuleId equals module.Id
                       join parent in CurrentContext.DbContext.Module
                       on module.ParentId equals parent.Id
                       where rm.RoleId == CurrentUser.RoleId && module.IsShowOnHome == true && module.IsShow==true && (module.IsDelete == null || module.IsDelete == false)
                       select new{module,parent} ).OrderBy(s=>s.parent.OrderNum);
            if (list.Any())
            {
                StringBuilder menu = new StringBuilder();
                menu.Append("<ul>");
                list.ToList().ForEach(s => {
                    StringBuilder div = new StringBuilder();
                    div.AppendFormat("<p><img src='{0}' width='70' height='70' alt=''/></p>", s.module.ImgUrl);
                    if (s.module.ModuleName.Contains("项目审批") || s.module.ModuleName.Contains("订单审批"))
                    {
                        List<int> customerIds = CurrentUser.Customers.Select(u => u.CustomerId).ToList();
                        List<int> companyList = MySonCompanyList.Select(m => m.Id).ToList();
                        var list1 = new SubjectBLL().GetList(sub => sub.Status == 4 && companyList.Contains(sub.CompanyId ?? 0) && customerIds.Contains(sub.CustomerId ?? 0) && (sub.ApproveState == 0 || sub.ApproveState == null) && (sub.IsDelete == null || sub.IsDelete == false)).ToList();
                        if(list1.Any())
                            div.Append("<span class='badge1'>" + list1.Count + "</span>");//
                    }
                    if (s.module.ModuleName.Contains("审批不通过"))
                    {
                        List<int> customerIds = CurrentUser.Customers.Select(u => u.CustomerId).ToList();
                        List<int> companyList = MySonCompanyList.Select(m => m.Id).ToList();
                        int userId=new BasePage().CurrentUser.UserId;
                        var list1 = new SubjectBLL().GetList(sub => sub.ApproveState == 2 && sub.AddUserId == userId && companyList.Contains(sub.CompanyId ?? 0) && customerIds.Contains(sub.CustomerId ?? 0) && (sub.IsDelete == null || sub.IsDelete == false)).ToList();
                        if (list1.Any())
                            div.Append("<span class='badge1'>" + list1.Count + "</span>");//
                    }
                    if (s.module.ModuleName.Contains("变更审批"))
                    {
                        List<int> customerIds = CurrentUser.Customers.Select(u => u.CustomerId).ToList();
                        List<int> companyList = MySonCompanyList.Select(m => m.Id).ToList();
                        var list1 = from order in CurrentContext.DbContext.OrderChangeApplication
                                    join user in CurrentContext.DbContext.UserInfo
                                    on order.AddUserId equals user.UserId
                                    where customerIds.Contains(order.CustomerId ?? 0)
                                    && companyList.Contains(user.CompanyId ?? 0)
                                    && (order.ManagerApperoveState == 0 || order.ManagerApperoveState == null)
                                    && (order.IsDelete==null || order.IsDelete==false)
                                    select order;
                        //int userId = new BasePage().CurrentUser.UserId;
                        //var list1 = new SubjectBLL().GetList(sub => sub.ApproveState == 2 && sub.AddUserId == userId && companyList.Contains(sub.CompanyId ?? 0) && customerIds.Contains(sub.CustomerId ?? 0) && (sub.IsDelete == null || sub.IsDelete == false)).ToList();
                        if (list1.Any())
                            div.Append("<span class='badge1'>" + list1.Count() + "</span>");//
                    }
                    if (s.module.ModuleName.Contains("项目变更处理"))
                    {
                        int userId = new BasePage().CurrentUser.UserId;
                        var list2 = from detail in CurrentContext.DbContext.OrderChangeApplicationDetail
                                    join application in CurrentContext.DbContext.OrderChangeApplication
                                    on detail.ApplicationId equals application.Id
                                    where application.AddUserId == CurrentUser.UserId
                                    && (application.IsDelete == null || application.IsDelete == false)
                                    && (detail.State ?? 0) < 2
                                    select detail;
                        if (list2.Any())
                            div.Append("<span class='badge1'>" + list2.Count() + "</span>");//
                    }
                    div.AppendFormat("<div style='text-align:center;'>{0}</div>",s.module.ModuleName);
                    menu.AppendFormat("<li style='position:relative;'><a href='{0}'>{1}</a></li>", s.module.Url, div.ToString());
                });
                menu.Append("</ul>");
                labMenu.Text = menu.ToString();
            }
        }
    }
}
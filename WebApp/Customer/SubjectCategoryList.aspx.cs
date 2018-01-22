using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using DAL;


namespace WebApp.Customer
{
    public partial class SubjectCategoryList :BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomerSearch);
                BindCustomerList(ref ddlCustomer);
                BindData();
            }
        }

        void BindData()
        {
            //var list = new ADSubjectCategoryBLL().GetList(s=>s.Id>0);
            List<int> curstomerList = new List<int>();

            foreach (ListItem item in ddlCustomerSearch.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }
            var list = (from category in CurrentContext.DbContext.ADSubjectCategory
                        join customer in CurrentContext.DbContext.Customer
                        on category.CustomerId equals customer.Id
                        where curstomerList.Contains(category.CustomerId ?? 0)
                        && (category.IsDelete == null || category.IsDelete==false)
                        select new
                        {
                            category.Id,
                            category.CategoryName,
                            customer.CustomerName,
                            category.CustomerId
                        }).ToList();
            int customerId1 = int.Parse(ddlCustomerSearch.SelectedValue);
            if (customerId1 > 0)
            {
                list = list.Where(s => s.CustomerId == customerId1).ToList();
            }
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderBy(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
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
            if (e.CommandName == "deleteItem")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                ADSubjectCategoryBLL bll = new ADSubjectCategoryBLL();
                ADSubjectCategory model = bll.GetModel(id);
                if (model != null)
                {
                    model.IsDelete = true;
                    bll.Update(model);
                    BindData();
                }
            }
        }
    }
}
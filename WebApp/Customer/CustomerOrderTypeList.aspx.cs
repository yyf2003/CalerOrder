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
    public partial class CustomerOrderTypeList : BasePage
    {
        int customerId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);
            }
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomerSearch);
                BindCustomerList(ref ddlCustomer);
                if (customerId > 0)
                {
                    ddlCustomerSearch.SelectedValue = customerId.ToString();
                    ddlCustomer.SelectedValue = customerId.ToString();
                }
                BindData();
                BindOrderTypeList();
            }
        }

        void BindData()
        {
            List<int> curstomerList = new List<int>();
            foreach (ListItem item in ddlCustomerSearch.Items)
            {
                int id = int.Parse(item.Value);
                if (!curstomerList.Contains(id))
                    curstomerList.Add(id);
            }
            var curstomers = (from customerO in CurrentContext.DbContext.CustomerOrderType
                                 join custmoer in CurrentContext.DbContext.Customer
                                 on customerO.CustomerId equals custmoer.Id
                                 where curstomerList.Contains(custmoer.Id)
                                 select custmoer).Distinct().ToList();
            int customerId1 = int.Parse(ddlCustomerSearch.SelectedValue);
            if (customerId1 > 0)
            {
                curstomers = curstomers.Where(s => s.Id == customerId1).ToList();
            }
            AspNetPager1.RecordCount = curstomers.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = curstomers.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }

        void BindOrderTypeList()
        {
            cblOrderType.Items.Clear();
            var list = new OrderTypeBLL().GetList(s=>s.IsDelete==false || s.IsDelete==null);
            if (list.Any())
            {
                list.ForEach(s => {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.OrderTypeName + "&nbsp;&nbsp;";
                    cblOrderType.Items.Add(li);
                });
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindData();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "deleteItem")
            {
                int customerId = int.Parse(e.CommandArgument.ToString());
                CustomerOrderTypeBll.Delete(s => s.CustomerId == customerId);
                BindData();
            }
        }


        CustomerOrderTypeBLL CustomerOrderTypeBll = new CustomerOrderTypeBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                Models.Customer cModel = e.Row.DataItem as Models.Customer;
                if (cModel != null)
                {
                    int customerId = cModel.Id;
                    var list = (from corderType in CurrentContext.DbContext.CustomerOrderType
                                join orderType in CurrentContext.DbContext.OrderType
                                on corderType.OrderTypeId equals orderType.Id
                                where corderType.CustomerId == customerId
                                select orderType).ToList();
                    if (list.Any())
                    {
                        Label labOrderType = (Label)e.Row.FindControl("labOrderType");
                        System.Text.StringBuilder str = new System.Text.StringBuilder();
                        list.ForEach(s => {
                            str.Append(s.OrderTypeName);
                            str.Append("/");
                        });
                        labOrderType.Text = str.ToString().TrimEnd('/');
                    }
                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}
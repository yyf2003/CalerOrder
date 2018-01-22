using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using DAL;
using BLL;
using Models;

namespace WebApp.OutsourcingOrder.PriceOrder
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                BindData();
            }
        }

        void BindData()
        {
            var list = (from order in CurrentContext.DbContext.OutsourcePriceOrder
                       join outsource in CurrentContext.DbContext.Company
                       on order.OutsourceId equals outsource.Id
                       join customer in CurrentContext.DbContext.Customer
                       on order.CustomerId equals customer.Id
                       join user in CurrentContext.DbContext.UserInfo
                       on order.AddUserId equals user.UserId
                       select new {
                           order,
                           order.OrderType,
                           outsource.CompanyName,
                           customer.CustomerName,
                           user.RealName
                       }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
            SetPromission(gv, new object[] { btnAdd });
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "EditItem")
            {
                Response.Redirect(string.Format("Add.aspx?subjectId={0}",id),false);
            }
            if (e.CommandName == "DeleteItem")
            { 
                OutsourcePriceOrderBLL bll=new OutsourcePriceOrderBLL();
                OutsourcePriceOrder model = bll.GetModel(id);
                if (model != null)
                {
                    bll.Delete(model);
                    BindData();
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
                    object objOrderType = item.GetType().GetProperty("OrderType").GetValue(item,null);
                    string orderType = objOrderType != null ? objOrderType.ToString() : "0";
                    ((Label)e.Row.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);
                }
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("Add.aspx",false);
        }
    }
}
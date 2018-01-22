using System;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;

namespace WebApp.Subjects.Material
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ref ddlCustomer);
                BindData();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }
        void BindData()
        {
            //OrderMaterialItemBLL materailItemBll = new OrderMaterialItemBLL();
            //var list = materailItemBll.GetList(s => 1 == 1);
            var list = (from mitem in CurrentContext.DbContext.OrderMaterialItem
                       join subject in CurrentContext.DbContext.Subject
                       on mitem.SubjectId equals subject.Id
                       select new {
                           mitem.BeginDate,
                           mitem.EndDate,
                           mitem.IsDelete,
                           mitem.ItemId,
                           mitem.SubjectId,
                           subject.SubjectName,
                           subject.CustomerId
                       }).ToList();
            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                list = list.Where(s => s.SubjectName.Contains(txtSubjectName.Text)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtBegin.Text))
            {
                if (StringHelper.IsDateTime(txtBegin.Text))
                {
                    DateTime date = DateTime.Parse(txtBegin.Text);
                    list = list.Where(s => s.BeginDate >= date).ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(txtEnd.Text))
            {
                if (StringHelper.IsDateTime(txtEnd.Text))
                {
                    DateTime date = DateTime.Parse(txtBegin.Text).AddDays(1);
                    list = list.Where(s => s.BeginDate < date).ToList();
                }
            }
            AspNetPager1.RecordCount = list.Count;
            gv.DataSource = list.OrderByDescending(s => s.ItemId).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
        }

        OrderMaterialBLL materialBll = new OrderMaterialBLL();
        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                //OrderMaterialItem model = (OrderMaterialItem)e.Row.DataItem;
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object itemIdObj = item.GetType().GetProperty("ItemId").GetValue(item, null);
                    int itemId = itemIdObj != null ? int.Parse(itemIdObj.ToString()) : 0;
                    var list = materialBll.GetList(s => s.ItemId == itemId).Select(s=>s.MaterialName).Distinct();
                    if (list.Any())
                    {
                        StringBuilder str = new StringBuilder();
                        list.ToList().ForEach(s => {
                            str.Append(s);
                            str.Append("/");
                        });
                        string str1 = str.Length > 20 ? (str.ToString().Substring(0, 20) + "...") : str.ToString().TrimEnd('/');
                        ((Label)e.Row.FindControl("labMaterial")).Text = str1;
                    }
                }
                
            }
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "deleteItem")
            {
                int itemId = int.Parse(e.CommandArgument.ToString());
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        OrderMaterialItemBLL materailItemBll = new OrderMaterialItemBLL();
                        OrderMaterialItem model = materailItemBll.GetModel(itemId);
                        if (model != null)
                        {
                            materailItemBll.Delete(model);
                            new OrderMaterialBLL().Delete(s => s.ItemId == itemId);
                        }
                        tran.Complete();
                        
                    }
                    catch (Exception ex)
                    {
                        Alert("删除失败！");
                    }
                }
                BindData();
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        
    }
}
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
using System.Data;

namespace WebApp.Subjects.ADOrders
{
    public partial class CheckOrderSuccess : System.Web.UI.Page
    {
        int checkOrderId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["checkOrderId"] != null)
            {
                checkOrderId = int.Parse(Request.QueryString["checkOrderId"]);
            }
            if (!IsPostBack)
            {
                BindCheckOrder();
                BinData();
            }
        }

        void BindCheckOrder()
        {
            var model = (from check in CurrentContext.DbContext.CheckOrder
                         join customer in CurrentContext.DbContext.Customer
                         on check.CustomerId equals customer.Id
                         where check.Id==checkOrderId
                         select new
                         {
                             check,
                             customer.CustomerName
                         }).FirstOrDefault();
            if (model != null)
            {
                labCustomer.Text = model.CustomerName;
                string begin = model.check.Begindate != null ? DateTime.Parse(model.check.Begindate.ToString()).ToShortDateString() : "";
                string end = model.check.EndDate != null ? DateTime.Parse(model.check.EndDate.ToString()).ToShortDateString() : "";
                if (!string.IsNullOrWhiteSpace(begin))
                    labDate.Text = begin + "—" + end;
                if (!string.IsNullOrWhiteSpace(model.check.SubjectId))
                {
                    List<int> sid = StringHelper.ToIntList(model.check.SubjectId, ',');
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    SubjectBLL subjectBll = new SubjectBLL();
                    Subject sModel;
                    sid.ForEach(s => {
                        sModel = subjectBll.GetModel(s);
                        if (sModel != null)
                        {
                            sb.Append(sModel.SubjectName);
                            sb.Append(',');
                        }
                       
                    });
                    labSubjectName.Text = sb.ToString().TrimEnd(',');
                }
                labAddDate.Text = model.check.AddDate != null ? DateTime.Parse(model.check.AddDate.ToString()).ToShortDateString() : "";
                
            }
        }

        void BinData()
        {
            var list = (from result in CurrentContext.DbContext.CheckOrderResult
                       join subject in CurrentContext.DbContext.Subject
                       on result.SubjectId equals subject.Id
                        where result.CheckOrderId == checkOrderId
                       select new {
                           subject.SubjectName,
                           result.BasicPositionCount,
                           result.CityId,
                           result.CityTier,
                           result.Format,
                           result.Gender,
                           result.IsInstall,
                           result.MaterialSupport,
                           result.OrderPositionCount,
                           result.PlanDetailId,
                           result.PositionId,
                           result.PositionName,
                           result.POSScale,
                           result.ProvinceId,
                           result.ProvinceName,
                           result.RegionId,
                           result.RegionNames,
                           result.SubjectId,
                           result.Id
                       }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = list.OrderByDescending(s => s.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvList.DataBind();
            lbExport.Visible = list.Any();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BinData();
        }

        protected void lbExport_Click(object sender, EventArgs e)
        {
            DataSet ds = new CheckOrderResultBLL().GetList(checkOrderId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                OperateFile.ExportExcel(ds.Tables[0], "核单结果");

            }
        }

        
    }
}
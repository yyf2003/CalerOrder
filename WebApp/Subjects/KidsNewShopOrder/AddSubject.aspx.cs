using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using Common;

namespace WebApp.Subjects.KidsNewShopOrder
{
    public partial class AddSubject : BasePage
    {
        int subjectId;
        SubjectBLL subjectBll = new SubjectBLL();
        Subject subjectModel;
        int ItemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["itemId"] != null)
            {
                ItemId = int.Parse(Request.QueryString["itemId"]);

            }
            if (!IsPostBack)
            {
                BindGuidanceList();
                BindOrderType();
                if (ItemId > 0)
                    BindSubjectType(ItemId);
                BindMyCustomerList(ddlCustomer);
                BindRegion();
                ddlCustomer.SelectedIndex = 1;
                BindSubjectCategory();
                //BindData();

            }
        }

        void BindGuidanceList()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            DateTime now = DateTime.Now;
            var list = new SubjectGuidanceBLL().GetList(s => (s.IsFinish == null || s.IsFinish == false) && (s.IsDelete == null || s.IsDelete == false)).OrderByDescending(s => s.ItemId).ToList();
            if (ItemId == 0)
            {
                if (subjectId == 0)
                {
                    DateTime date = DateTime.Now;
                    DateTime newDate = new DateTime(date.Year, date.Month, 1);
                    DateTime beginDate = newDate.AddMonths(-1);
                    DateTime endDate = newDate.AddMonths(2);
                    list = list.Where(s => s.BeginDate >= beginDate && s.BeginDate < endDate).ToList();
                }

            }
            if (list.Any())
            {
                ddlGuidance.DataSource = list;
                ddlGuidance.DataTextField = "ItemName";
                ddlGuidance.DataValueField = "ItemId";
                ddlGuidance.DataBind();
                ddlGuidance.Items.Insert(0, new ListItem("请选择", "0"));
                if (ItemId > 0)
                {
                    ddlGuidance.SelectedValue = ItemId.ToString();
                    ddlGuidance.Enabled = false;
                }
                if (subjectId > 0)
                    ddlGuidance.Enabled = false;
            }

        }

        void BindOrderType()
        {
            rblSubjectType.Items.Clear();
            List<EnumEntity> list = CommonMethod.GetEnumList<SubjectTypeEnum>().Where(s => s.Desction == OrderChannelEnum.上海订单.ToString()).ToList();
            if (list.Any())
            {
                int index = 0;
                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Value.ToString();
                    li.Text = s.Name + "&nbsp;";
                    if (index == 0)
                        li.Selected = true;
                    rblSubjectType.Items.Add(li);
                    index++;
                });
            }
        }

        void BindSubjectType(int itemId)
        {

            SubjectGuidance model = new SubjectGuidanceBLL().GetModel(itemId);
            ddlSubjectType.Items.Clear();
            if (model != null)
            {
                if (model.BeginDate != null)
                {
                    txtBeginDate.Text = DateTime.Parse(model.BeginDate.ToString()).ToShortDateString();
                }
                if (model.EndDate != null)
                {
                    txtEndDate.Text = DateTime.Parse(model.EndDate.ToString()).ToShortDateString();
                }
                var typeList = new SubjectTypeBLL().GetList(s => s.GuidanceId == itemId && (s.IsDelete == false || s.IsDelete == null));
                if (typeList.Any())
                {
                    typeList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s.Id.ToString();
                        li.Text = s.SubjectTypeName;
                        ddlSubjectType.Items.Add(li);
                    });
                }
            }
            ddlSubjectType.Items.Insert(0, new ListItem("请选择", "0"));
        }

        void BindSubjectCategory()
        {
            var List = new ADSubjectCategoryBLL().GetList(s => s.Id > 0);
            if (List.Any())
            {
                List.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.CategoryName;
                    ddlSubjectCategory.Items.Add(li);
                });
            }
        }

        void BindRegion()
        {
            rblPriceBlong.Items.Clear();
            
            ListItem li0 = new ListItem();
            li0.Value = "";
            li0.Text = "默认&nbsp;&nbsp;";
            li0.Selected = true;
            rblPriceBlong.Items.Add(li0);
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<string> regions = new RegionBLL().GetList(s => s.CustomerId == customerId).Select(s => s.RegionName).Distinct().ToList();

            if (regions.Any())
            {
                regions.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s;
                    li.Text = s + "&nbsp;&nbsp;";
                    rblPriceBlong.Items.Add(li);

                });
            }
        }

        protected void ddlGuidance_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnNext_Click(object sender, EventArgs e)
        {

        }

        
    }
}
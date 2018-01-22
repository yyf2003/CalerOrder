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
using System.Transactions;

namespace WebApp.Subjects.OutsourceMaterialOrder
{
    public partial class AddSubject : BasePage
    {
        int subjectId;
        SubjectBLL subjectBll = new SubjectBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                BindMyCustomerList(ddlCustomer);
                BindMaterialDll();
                BindMaterialList();
                BindGuidanceList();
                BindOutsourceList();
                BindData();
            }

        }

        void BindData()
        {
            Subject subjectModel = new SubjectBLL().GetModel(subjectId);
            if (subjectModel != null)
            {
                if (subjectModel.CustomerId != null)
                    ddlCustomer.SelectedValue = subjectModel.CustomerId.ToString();
                if (subjectModel.GuidanceId != null)
                    ddlGuidance.SelectedValue = subjectModel.GuidanceId.ToString();
                txtSubjectName.Text = subjectModel.SubjectName;
                if (subjectModel.OutsourceId != null)
                    ddlOutsource.SelectedValue = subjectModel.OutsourceId.ToString();
                txtRemark.Text = subjectModel.Remark;
                var detailList = new OutsourceMaterialOrderDetailBLL().GetList(s => s.OutsourceSubjectId == subjectId);
                if (detailList.Any())
                {
                    Session["OutsourceMaterial"] = detailList;
                    BindMaterialList();
                }
            }
        }

        void BindMaterialDll()
        {
            var list = new OutSourceMaterialBLL().GetList(s => s.IsDelete == null || s.IsDelete == false);
            list.ForEach(s =>
            {
                ListItem li = new ListItem();
                li.Value = s.Id.ToString();
                li.Text = s.MaterialName;
                ddlMaterial.Items.Add(li);
            });
        }

        void BindMaterialList()
        {
            List<OutsourceMaterialOrderDetail> list = new List<OutsourceMaterialOrderDetail>();
            if (Session["OutsourceMaterial"] != null)
            {
                list = Session["OutsourceMaterial"] as List<OutsourceMaterialOrderDetail>;
            }
            tbMaterialList.DataSource = list.OrderBy(s => s.MaterialName).ToList();
            tbMaterialList.DataBind();
        }

        void BindGuidanceList()
        {
            ddlGuidance.Items.Clear();

            int customerId = int.Parse(ddlCustomer.SelectedValue);
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            DateTime now = DateTime.Now;
            var list = new SubjectGuidanceBLL().GetList(s => s.CustomerId == customerId && (s.IsFinish == null || s.IsFinish == false) && (s.IsDelete == null || s.IsDelete == false)).OrderByDescending(s => s.ItemId).ToList();
            if (subjectId == 0)
            {
                string guidanceMonth = txtGuidanceMonth.Text.Trim();
                if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                {
                    DateTime date = DateTime.Parse(guidanceMonth);
                    int year = date.Year;
                    int month = date.Month;
                    list = list.Where(s => s.GuidanceYear == year && s.GuidanceMonth == month).ToList();

                }
                else
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

                if (subjectId > 0)
                {
                    txtGuidanceMonth.Enabled = false;
                    ddlGuidance.Enabled = false;
                }
            }
            ddlGuidance.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        void BindOutsourceList()
        {
            var companyList = new CompanyBLL().GetList(s => s.TypeId == (int)CompanyTypeEnum.Outsource && (s.IsDelete == null || s.IsDelete == false)).OrderBy(s => s.ProvinceId).ToList();

            if (CurrentUser.RoleId == 5)
            {
                int userId = new BasePage().CurrentUser.UserId;
                List<int> outsourceList = new OutsourceInUserBLL().GetList(s => s.UserId == userId).Select(s => s.OutsourceId ?? 0).ToList();
                companyList = companyList.Where(s => outsourceList.Contains(s.Id)).ToList();

            }
            if (companyList.Any())
            {
                companyList.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.CompanyName;
                    ddlOutsource.Items.Add(li);
                });
            }
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGuidanceMonth.Text.Trim()))
            {
                BindGuidanceList();
            }
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            int mid = int.Parse(ddlMaterial.SelectedValue);
            OutSourceMaterial model = new OutSourceMaterialBLL().GetModel(mid);
            if (model != null)
            {
                labPrice.Text = model.UnitPrice != null ? model.UnitPrice.ToString() : "";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            int customerId = int.Parse(ddlCustomer.SelectedValue);
            int guidanceId = int.Parse(ddlGuidance.SelectedValue);
            string subjectName = txtSubjectName.Text.Trim();
            int outsourceId = int.Parse(ddlOutsource.SelectedValue);
            string remark = txtRemark.Text.Trim();
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    Subject subjectModel = new Subject(); ;
                    labMsg.Text = "";
                    if (CheckSubject(txtSubjectName.Text.Trim(), subjectId, guidanceId))
                    {
                        labMsg.Text = "项目名称重复";
                        return;
                    }
                    if (subjectId > 0)
                    {
                        subjectModel = subjectBll.GetModel(subjectId);
                    }
                    subjectModel.SubjectName = txtSubjectName.Text.Trim();
                    //subjectModel.BeginDate = DateTime.Parse(txtBeginDate.Text.Trim());
                    subjectModel.CustomerId = int.Parse(ddlCustomer.SelectedValue);
                    //subjectModel.EndDate = DateTime.Parse(txtEndDate.Text.Trim());
                    subjectModel.Remark = txtRemark.Text;
                    subjectModel.SubjectType = (int)SubjectTypeEnum.外协订单;
                    subjectModel.OutsourceId = outsourceId;
                    subjectModel.GuidanceId = guidanceId;
                    subjectModel.Status = 4;
                    if (subjectId > 0)
                    {
                        subjectBll.Update(subjectModel);
                    }
                    else
                    {
                        subjectModel.AddOrderType = 3;
                        subjectModel.AddUserId = CurrentUser.UserId;
                        subjectModel.AddDate = DateTime.Now;
                        subjectModel.IsDelete = false;
                        subjectModel.SubjectNo = CreateSubjectNo();
                        subjectModel.CompanyId = CurrentUser.CompanyId;
                        subjectModel.ApproveState = 0;
                        subjectBll.Add(subjectModel);
                        subjectId = subjectModel.Id;
                    }

                    if (Session["OutsourceMaterial"] != null)
                    {
                        List<OutsourceMaterialOrderDetail> list = Session["OutsourceMaterial"] as List<OutsourceMaterialOrderDetail>;
                        if (list.Any())
                        {
                            OutsourceMaterialOrderDetailBLL detailBll = new OutsourceMaterialOrderDetailBLL();
                            detailBll.Delete(s => s.OutsourceSubjectId == subjectId);
                            list.ForEach(s =>
                            {
                                s.GuidanceId = guidanceId;
                                s.OutsourceSubjectId = subjectId;
                                detailBll.Add(s);
                            });
                        }
                    }
                    tran.Complete();
                    Response.Redirect("SubjectList.aspx", false);
                }
                catch (Exception ex)
                {
                    Alert("提交失败：" + ex.Message);
                }
            }

        }

        protected void btnAddMaterial_Click(object sender, EventArgs e)
        {
            labAddMaterialMsg.Text = "";
            int materialId = int.Parse(ddlMaterial.SelectedValue);
            string materialName = ddlMaterial.SelectedItem.Text;
            decimal price = !string.IsNullOrWhiteSpace(labPrice.Text) ? StringHelper.IsDecimal(labPrice.Text) : 0;
            int amount = !string.IsNullOrWhiteSpace(txtMaterialAmount.Text) ? StringHelper.IsInt(txtMaterialAmount.Text.Trim()) : 1;
            string remark = txtMaterialRemark.Text.Trim();
            OutsourceMaterialOrderDetail model = new OutsourceMaterialOrderDetail();
            model.MaterialId = materialId;
            model.Amount = amount;
            model.MaterialName = materialName;
            model.Remark = remark;
            model.UnitPrice = price;
            List<OutsourceMaterialOrderDetail> list = new List<OutsourceMaterialOrderDetail>();
            if (Session["OutsourceMaterial"] != null)
            {
                list = Session["OutsourceMaterial"] as List<OutsourceMaterialOrderDetail>;
                var list0 = list.Where(s => s.MaterialId == materialId);
                if (list0.Any())
                {
                    labAddMaterialMsg.Text = "重复添加";
                    return;
                }
            }
            list.Add(model);
            Session["OutsourceMaterial"] = list;
            BindMaterialList();
        }


        /// <summary>
        /// 检查项目名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool CheckSubject(string name, int id, int guidanceId)
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            name = StringHelper.ReplaceSpace(name);
            var list = subjectBll.GetList(s => s.GuidanceId == guidanceId && s.SubjectName.Replace(" ", "").ToLower() == name.ToLower() && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId && (id > 0 ? s.Id != id : 1 == 1) && (s.IsDelete == null || s.IsDelete == false));

            return list.Any();
        }

        protected void tbMaterialList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "deleteItem")
            {
                int materialId = int.Parse(e.CommandArgument.ToString());
                if (Session["OutsourceMaterial"] != null)
                {
                    List<OutsourceMaterialOrderDetail> list = Session["OutsourceMaterial"] as List<OutsourceMaterialOrderDetail>;
                    if (list.Any())
                    {
                        OutsourceMaterialOrderDetail model = list.Where(s => s.MaterialId == materialId).FirstOrDefault();
                        if (model != null)
                        {
                            list.Remove(model);
                            Session["OutsourceMaterial"] = list;
                            BindMaterialList();
                        }
                    }
                }

            }
            if (e.CommandName == "editItem")
            {
                int materialId = int.Parse(e.CommandArgument.ToString());
                List<OutsourceMaterialOrderDetail> list = Session["OutsourceMaterial"] as List<OutsourceMaterialOrderDetail>;
                if (list.Any())
                {
                    OutsourceMaterialOrderDetail model = list.Where(s => s.MaterialId == materialId).FirstOrDefault();
                    if (model != null)
                    {
                        ddlMaterial.SelectedValue = materialId.ToString();
                        labPrice.Text = model.UnitPrice != null ? model.UnitPrice.ToString() : "";
                        txtMaterialAmount.Text = model.Amount != null ? model.Amount.ToString() : "1";
                        txtMaterialRemark.Text = model.Remark;
                        btnEditMaterial.Visible = true;
                    }
                }

            }
        }

        protected void btnEditMaterial_Click(object sender, EventArgs e)
        {
            labAddMaterialMsg.Text = "";
            int materialId = int.Parse(ddlMaterial.SelectedValue);
            string materialName = ddlMaterial.SelectedItem.Text;
            decimal price = !string.IsNullOrWhiteSpace(labPrice.Text) ? StringHelper.IsDecimal(labPrice.Text) : 0;
            int amount = !string.IsNullOrWhiteSpace(txtMaterialAmount.Text) ? StringHelper.IsInt(txtMaterialAmount.Text.Trim()) : 1;
            string remark = txtMaterialRemark.Text.Trim();
            if (Session["OutsourceMaterial"] != null)
            {
                List<OutsourceMaterialOrderDetail> list = Session["OutsourceMaterial"] as List<OutsourceMaterialOrderDetail>;
                var oldModel = list.Where(s => s.MaterialId == materialId).FirstOrDefault();
                if (oldModel!=null)
                {
                    list.Remove(oldModel);
                    oldModel.Amount = amount;
                    oldModel.MaterialName = materialName;
                    oldModel.Remark = remark;
                    oldModel.UnitPrice = price;
                    list.Add(oldModel);
                    Session["OutsourceMaterial"] = list;
                    ddlMaterial.SelectedValue = "0";
                    labPrice.Text = "";
                    txtMaterialAmount.Text = "1";
                    txtMaterialRemark.Text = "";
                    btnEditMaterial.Visible = false;
                    BindMaterialList();
                }
                else
                    labAddMaterialMsg.Text = "更新失败";
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("SubjectList.aspx",false);
        }

    }
}
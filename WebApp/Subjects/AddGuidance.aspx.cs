using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using Common;
using System.Text;

namespace WebApp.Subjects
{
    public partial class AddGuidance : BasePage
    {
        int id;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["itemId"] != null)
            {
                id = int.Parse(Request.QueryString["itemId"]);
            }
            if (!IsPostBack)
            {
                BindMyCustomerList(ref ddlCustomer);
                BindGuidanceAddType();
                BindExperssPrice();
                BindActivityType();
                BindMaterialPriceItem();

            }
            BindData();
        }

        void BindGuidanceAddType()
        {
            rblAddType.Items.Clear();
            var enumList = CommonMethod.GetEnumList<GuidanceAddTypeEnum>();
            enumList.ForEach(s => {
                ListItem li = new ListItem();
                li.Text = s.Desction + "&nbsp;&nbsp;";
                li.Value = s.Value.ToString();
                if (s.Value == (int)GuidanceAddTypeEnum.POP)
                    li.Selected = true;
                rblAddType.Items.Add(li);
            });
        }

        void BindActivityType()
        {
           
            var list = CommonMethod.GetEnumList<GuidanceTypeEnum>();
            if (list.Any())
            {
                //rblActivityType.DataSource = list;
                //rblActivityType.DataTextField = "Desction";
                //rblActivityType.DataValueField = "Value";
                //rblActivityType.DataBind();
                list.ForEach(s => {
                    ListItem li = new ListItem();
                    li.Text = s.Desction + "&nbsp;&nbsp;";
                    li.Value = s.Value.ToString();
                    rblActivityType.Items.Add(li);
                });
            }
        }

        void BindExperssPrice()
        {
            var list = new ExpressPriceConfigBLL().GetList(s=>s.Id>0);
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Text = s.ReceivePrice.ToString();
                    li.Value = s.ReceivePrice.ToString();
                    ddlExperssPrice.Items.Add(li);
                });
            }
        }

        void BindMaterialPriceItem()
        {
            ddlPriceItemList.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = new CustomerMaterialPriceItemBLL().GetList(s => s.CustomerId == customerId &&(s.IsDelete == null || s.IsDelete == false)).OrderByDescending(s => s.IsTop).ThenByDescending(s=>s.Id).ToList();
            if (list.Any())
            {
                int index = 0;
                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.ItemName;
                    if (index == 0)
                        li.Selected = true;
                    ddlPriceItemList.Items.Add(li);
                    index++;
                });
            }
            else
            {
                ddlPriceItemList.Items.Add(new ListItem("--请选择--", "0"));
            }
        }

        SubjectGuidanceBLL bll = new SubjectGuidanceBLL();
        SubjectTypeBLL subjectTypeBll = new SubjectTypeBLL();
        void BindData()
        {
            SubjectGuidance model = bll.GetModel(id);
            bool flag = false;
            StringBuilder divStr = new StringBuilder();
            if (model != null)
            {
                if (!IsPostBack)
                {
                    
                    ddlCustomer.SelectedValue = model.CustomerId != null ? model.CustomerId.ToString() : "";
                    int addType = model.AddType ?? 1;
                    rblAddType.SelectedValue = addType.ToString();

                    BindMaterialPriceItem();
                    rblActivityType.SelectedValue = model.ActivityTypeId != null ? model.ActivityTypeId.ToString() : "0";
                    if (model.HasExperssFees != null)
                    {
                        int hasFee = (model.HasExperssFees ?? true) ? 1 : 0;
                        rblHasExperssFees.SelectedValue = hasFee.ToString();
                    }
                    if (model.HasInstallFees != null)
                    {
                        int hasFee = (model.HasInstallFees ?? true) ? 1 : 0;
                        rblHasInstallFees.SelectedValue = hasFee.ToString();
                    }
                    if (model.PriceItemId != null)
                    {
                        ddlPriceItemList.SelectedValue = model.PriceItemId.ToString();
                    }
                    txtItemName.Text = model.ItemName;
                    if (!string.IsNullOrWhiteSpace(model.SubjectNames))
                        //txtSubjectNames.Text = model.SubjectNames.Replace("<br/>","\r\n");
                        txtSubjectNames.Text = model.SubjectNames;
                    if (model.GuidanceYear != null && model.GuidanceMonth != null)
                    {
                        string month = model.GuidanceYear + "-" + model.GuidanceMonth;
                        if (StringHelper.IsDateTime(month))
                        {
                            txtGuidanceMonth.Text = month;
                        }
                    }
                    if (model.BeginDate != null)
                        txtBeginDate.Text = DateTime.Parse(model.BeginDate.ToString()).ToShortDateString();
                    if (model.EndDate != null)
                        txtEndDate.Text = DateTime.Parse(model.EndDate.ToString()).ToShortDateString();
                    txtRemark.Text = model.Remark;
                    if ((model.ExperssPrice ?? 0) > 0)
                        ddlExperssPrice.SelectedValue = model.ExperssPrice.ToString();
                }
                var typeList = subjectTypeBll.GetList(s => s.GuidanceId == id && (s.IsDelete == false || s.IsDelete == null));
                if (typeList.Any())
                {

                    typeList.ForEach(s =>
                    {
                        divStr.AppendFormat("<input type='text' data-typeid='{0}' name='txtTypeName' value='{1}'/><span name='deletetype' style='color:red;cursor:pointer;'>×</span>", s.Id, s.SubjectTypeName);
                    });

                    flag = true;
                }
            }
            if (!flag)
            {

                for (int i = 0; i < 8; i++)
                {
                    divStr.Append("<input type='text' data-typeid='0' name='txtTypeName' value=''/><span name='deletetype' style='color:red;cursor:pointer;'>×</span>");
                }
            }
            divStr.Append("<span style=''><a id='btnAdd' style='float: left;margin-left:-5px;margin-top:-2px;' class='easyui-linkbutton' plain='true' icon='icon-add'></a></span>");
            typeContainer.InnerHtml = divStr.ToString();
            if (id > 0)
            {
                var subjectList = new SubjectBLL().GetList(s => s.GuidanceId == id && (s.IsDelete == null || s.IsDelete == false));
                if (subjectList.Any())
                {
                    ddlCustomer.Enabled = false;
                    rblAddType.Enabled = false;
                    rblActivityType.Enabled = false;
                    ddlPriceItemList.Enabled = false;

                }
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string ItemName = txtItemName.Text.Trim();
            string BeginDate = txtBeginDate.Text;
            string EndDate = txtEndDate.Text;
            string SubjectNames = string.Empty;
            decimal expressPrice = decimal.Parse(ddlExperssPrice.SelectedValue);
            if (!string.IsNullOrWhiteSpace(txtSubjectNames.Text))
            {
                //SubjectNames = txtSubjectNames.Text.Replace("\r\n", "<br/>");
                SubjectNames = txtSubjectNames.Text;
            }
            string Remark = txtRemark.Text.Trim();
            string guidanceMonth = txtGuidanceMonth.Text.Trim();
            int priceItemId = int.Parse(ddlPriceItemList.SelectedValue);
            bool canSave = false;
            string errorMsg = string.Empty;
            if (priceItemId == 0)
            {
                canSave = false;
                errorMsg = "请选择材质价格方案";
            }
            else
            {
                if (id > 0)
                {
                    canSave = !CheckItemName(ItemName, id);
                }
                else
                    canSave = !CheckItemName(ItemName, null);
            }
            if (canSave)
            {
                try
                {
                    SubjectGuidance model = new SubjectGuidance();
                    bool hasInstallFee = false;
                    bool hasExpressFee = false;
                    bool isUpDate = false;
                    if (id > 0)
                    {
                        model = bll.GetModel(id);
                        if (model != null)
                        {
                            isUpDate = true;
                            hasInstallFee = model.HasInstallFees ?? false;
                            hasExpressFee = model.HasExperssFees ?? false;
                        }
                        else
                            model = new SubjectGuidance();
                    }
                    if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
                    {
                        DateTime month = DateTime.Parse(guidanceMonth);
                        model.GuidanceYear = month.Year;
                        model.GuidanceMonth = month.Month;
                        model.GuidanceDate = month;
                    }
                    if (!string.IsNullOrWhiteSpace(BeginDate))
                        model.BeginDate = DateTime.Parse(BeginDate);
                    if (!string.IsNullOrWhiteSpace(EndDate))
                        model.EndDate = DateTime.Parse(EndDate);
                    model.ItemName = ItemName;
                    int addType = 1;
                    if (rblAddType.SelectedValue!=null)
                        addType = int.Parse(rblAddType.SelectedValue);
                    model.AddType = addType;
                    model.Remark = Remark;
                    model.CustomerId = int.Parse(ddlCustomer.SelectedValue);
                    if (rblActivityType.SelectedValue != null)
                    {
                        int activityId=int.Parse(rblActivityType.SelectedValue);
                        model.ActivityTypeId = activityId;
                        if (activityId == (int)GuidanceTypeEnum.Promotion)
                        {
                            int hasFee = int.Parse(rblHasExperssFees.SelectedValue);
                            model.HasExperssFees = hasFee == 1;
                        }
                        else
                            model.HasExperssFees = null;
                        if (activityId == (int)GuidanceTypeEnum.Install)
                        {
                            int hasFee = int.Parse(rblHasInstallFees.SelectedValue);
                            model.HasInstallFees = hasFee == 1;
                        }
                        else
                            model.HasInstallFees = null;
                        if (activityId == (int)GuidanceTypeEnum.Delivery)
                        {
                            model.ExperssPrice = expressPrice;
                        }
                        else
                            model.ExperssPrice = 0;
                    }
                    model.SubjectNames = SubjectNames;
                    model.PriceItemId = priceItemId;

                    if (isUpDate)
                    {
                        bll.Update(model);
                        if ((model.ActivityTypeId == (int)GuidanceTypeEnum.Install) && !(model.HasInstallFees ?? false) && hasInstallFee)
                        { 
                            //如果原来有安装费，修改后没有的话，就自动删除安装费
                            new InstallPriceShopInfoBLL().Delete(s => s.GuidanceId == id);
                            new InstallPriceTempBLL().Delete(s => s.GuidanceId == id);
                            new OutsourceOrderDetailBLL().Delete(s => s.GuidanceId == id && s.OrderType==(int)OrderTypeEnum.安装费 && s.SubjectId==0);
                        }
                        else if ((model.ActivityTypeId == (int)GuidanceTypeEnum.Promotion) && !(model.HasExperssFees ?? false) && hasExpressFee)
                        {
                            //如果原来有快递费，修改后没有的话，就自动删除快递费
                            new ExpressPriceDetailBLL().Delete(s => s.GuidanceId == id);
                            new OutsourceOrderDetailBLL().Delete(s => s.GuidanceId == id && s.OrderType == (int)OrderTypeEnum.发货费 && s.SubjectId == 0);
                        }
                    }
                    else
                    {
                        model.AddDate = DateTime.Now;
                        model.AddUserId = CurrentUser.UserId;
                        bll.Add(model);
                    }
                    if (hfSubjectType.Value != "")
                    {
                        string[] arr = hfSubjectType.Value.Split('|');
                        foreach (string s in arr)
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                int typeId = int.Parse(s.Split(':')[0]);
                                string typeName = s.Split(':')[1];
                                if (typeId > 0)
                                {
                                    SubjectType stModel = subjectTypeBll.GetModel(typeId);
                                    if (stModel != null)
                                    {
                                        stModel.SubjectTypeName = typeName;
                                        subjectTypeBll.Update(stModel);
                                    }
                                }
                                else
                                {
                                    SubjectType stModel = new SubjectType() { GuidanceId = model.ItemId, SubjectTypeName = typeName };
                                    subjectTypeBll.Add(stModel);
                                }
                            }
                        }
                    }
                    //Upload(model.ItemId);
                    Alert("提交成功！", "GuidanceList.aspx");
                }
                catch (Exception ex)
                {
                    Alert("提交失败！");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(errorMsg))
                {
                    errorMsg = "该活动名称已存在";
                }
                Alert(errorMsg);
            }

        }

        bool CheckItemName(string name, int? id)
        {
            var list = bll.GetList(s => s.ItemName.ToLower() == name.ToLower() && (id != null ? (s.ItemId != id) : (1 == 1)));
            return list.Any();

        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindMaterialPriceItem();
        }

        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using BLL;
using Models;
using Common;

namespace WebApp.OutsourcingOrder.ChangeMaterialPrice
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCustomerList(ddlCustomer);
                DateTime now = DateTime.Now;
                txtGuidanceMonth.Text = now.Year + "-" + now.Month;
                BindGuidance();
                BindOrderList();
                BindPriceItem();
            }
        }

        void BindGuidance()
        {
            cblGuidanceList.Items.Clear();
            cblMaterial.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);

            var list = (from guidance in CurrentContext.DbContext.SubjectGuidance
                        join subject in CurrentContext.DbContext.Subject
                        on guidance.ItemId equals subject.GuidanceId
                        where guidance.CustomerId == customerId
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && (guidance.IsDelete == null || guidance.IsDelete == false)
                        select new
                        {
                            guidance
                            //type
                        }).Distinct().ToList();


            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                list = list.Where(s => s.guidance.GuidanceYear == year && s.guidance.GuidanceMonth == month).ToList();

            }

            if (list.Any())
            {
                list = list.OrderBy(s => s.guidance.ItemId).ToList();

                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    string ActivityName = CommonMethod.GetEnumDescription<GuidanceTypeEnum>((s.guidance.ActivityTypeId ?? 1).ToString());
                    li.Value = s.guidance.ItemId.ToString();
                    li.Text = s.guidance.ItemName + "-" + ActivityName + "&nbsp;&nbsp;";
                    cblGuidanceList.Items.Add(li);
                });
            }
            Panel_EmptyGuidance.Visible = !list.Any();
        }

        void GetMaterialList()
        {
            cblMaterial.Items.Clear();
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            if (guidanceIdList.Any())
            {
                var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 where subject.ApproveState == 1
                                 && (subject.IsDelete == null || subject.IsDelete == false)
                                 && guidanceIdList.Contains(order.GuidanceId ?? 0)
                                 && (order.OrderType == (int)OrderTypeEnum.POP || order.OrderType == (int)OrderTypeEnum.道具)
                                 && order.GraphicMaterial != null && order.GraphicMaterial != ""
                                 select order).ToList();
                if (orderList.Any())
                {
                    List<string> materialList = orderList.Select(s => s.GraphicMaterial.ToLower()).OrderBy(s => s).Distinct().ToList();

                    materialList.ForEach(s =>
                    {
                        ListItem li = new ListItem();
                        li.Value = s;
                        li.Text = s + "&nbsp;&nbsp;";
                        cblMaterial.Items.Add(li);
                    });
                }
            }
        }

        void BindOrderList()
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            List<string> selectMaterialList = new List<string>();
            foreach (ListItem li in cblMaterial.Items)
            {
                if (li.Selected)
                {
                    selectMaterialList.Add(li.Value.ToLower());
                }
            }
            
            var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on order.GuidanceId equals guidance.ItemId
                             where subject.ApproveState == 1
                             && (subject.IsDelete == null || subject.IsDelete == false)
                             && guidanceIdList.Contains(order.GuidanceId ?? 0)
                             && (order.OrderType == (int)OrderTypeEnum.POP || order.OrderType == (int)OrderTypeEnum.道具)
                             && order.GraphicMaterial != null && order.GraphicMaterial != ""
                             select new { guidance.ItemName, order }).ToList();
            if (selectMaterialList.Any())
            {
                orderList = orderList.Where(s => selectMaterialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
            }
            btnUpdatePrice.Enabled = orderList.Any();
            AspNetPager1.RecordCount = orderList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            Repeater1.DataSource = orderList.OrderBy(s => s.order.GraphicMaterial).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            Repeater1.DataBind();
        }

        void BindPriceItem()
        {
            ddlPriceItem.Items.Clear();
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            var list = new OutsourceMaterialPriceItemBLL().GetList(s => s.CustomerId == customerId).OrderByDescending(s => s.BeginDate).ToList();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    ListItem li = new ListItem();
                    li.Value = s.Id.ToString();
                    li.Text = s.ItemName;
                    ddlPriceItem.Items.Add(li);
                });
            }
            ddlPriceItem.Items.Insert(0, new ListItem("--请选择价格条目--", "0"));
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGuidance();
            BindPriceItem();
        }

        protected void txtGuidanceMonth_TextChanged(object sender, EventArgs e)
        {
            BindGuidance();
        }

        protected void lbUp_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month <= 1)
                {
                    year--;
                    month = 12;
                }
                else
                    month--;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();
            }
        }

        protected void lbDown_Click(object sender, EventArgs e)
        {
            string guidanceMonth = txtGuidanceMonth.Text;
            if (!string.IsNullOrWhiteSpace(guidanceMonth) && StringHelper.IsDateTime(guidanceMonth))
            {
                DateTime date = DateTime.Parse(guidanceMonth);
                int year = date.Year;
                int month = date.Month;
                if (month >= 12)
                {
                    year++;
                    month = 1;
                }
                else
                    month++;
                txtGuidanceMonth.Text = year + "-" + month;
                BindGuidance();

            }
        }

        protected void btnCheckAllGuidance_Click(object sender, EventArgs e)
        {
            GetMaterialList();
        }

        protected void cblGuidanceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetMaterialList();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AspNetPager1.CurrentPageIndex = 1;
            BindOrderList();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindOrderList();
        }

        protected void btnUpdatePrice_Click(object sender, EventArgs e)
        {
            int customerId = int.Parse(ddlCustomer.SelectedValue);
            List<int> guidanceIdList = new List<int>();
            foreach (ListItem li in cblGuidanceList.Items)
            {
                if (li.Selected)
                {
                    guidanceIdList.Add(int.Parse(li.Value));
                }
            }
            List<string> selectMaterialList = new List<string>();
           
            foreach (ListItem li in cblMaterial.Items)
            {
                if (li.Selected)
                {
                    selectMaterialList.Add(li.Value.ToLower());
                }
            }
            bool isOk = true;
            string errorMsg = string.Empty;

            try
            {
                var orderList = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                                 join subject in CurrentContext.DbContext.Subject
                                 on order.SubjectId equals subject.Id
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on order.GuidanceId equals guidance.ItemId
                                 where subject.ApproveState == 1
                                 && (subject.IsDelete == null || subject.IsDelete == false)
                                 && guidanceIdList.Contains(order.GuidanceId ?? 0)
                                 && (order.OrderType == (int)OrderTypeEnum.POP || order.OrderType == (int)OrderTypeEnum.道具)
                                 && order.GraphicMaterial != null && order.GraphicMaterial != ""
                                 select new { guidance.ItemName, order }).ToList();
                if (selectMaterialList.Any())
                {
                    orderList = orderList.Where(s => selectMaterialList.Contains(s.order.GraphicMaterial.ToLower())).ToList();
                }
                if (orderList.Any())
                {
                    int priceItemId = int.Parse(ddlPriceItem.SelectedValue);
                    POP popModel;
                    string unitName = string.Empty;
                    OutsourceOrderDetailBLL orderBll = new OutsourceOrderDetailBLL();
                    Dictionary<int, SubjectGuidance> guidanceDic = new Dictionary<int, SubjectGuidance>();
                    SubjectGuidanceBLL guidanceBll = new SubjectGuidanceBLL();
                    SubjectGuidance guidanceModel;
                    orderList.ForEach(s =>
                    {
                        popModel = new POP();
                        popModel.Quantity = s.order.Quantity;
                        popModel.PriceItemId = priceItemId;
                        popModel.GraphicLength = s.order.GraphicLength;
                        popModel.GraphicWidth = s.order.GraphicWidth;
                        popModel.GraphicMaterial = s.order.GraphicMaterial;
                        popModel.CustomerId = customerId;
                        int guidanceType = 0;
                        if (guidanceDic.Keys.Contains(s.order.GuidanceId ?? 0))
                        {
                            guidanceModel = guidanceDic[s.order.GuidanceId ?? 0];
                            guidanceType = guidanceModel.ActivityTypeId ?? 0;
                        }
                        else
                        {
                            guidanceModel = guidanceBll.GetModel(s.order.GuidanceId ?? 0);
                            if (guidanceModel != null)
                            {
                                guidanceDic.Add(s.order.GuidanceId ?? 0, guidanceModel);
                                guidanceType = guidanceModel.ActivityTypeId ?? 0;
                            }
                        }
                        if (guidanceType == (int)GuidanceTypeEnum.Promotion)
                        {
                            popModel.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                        }
                        else if (guidanceType == (int)GuidanceTypeEnum.Delivery)
                        {
                            if (s.order.Province == "北京")
                            {
                                popModel.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                            }
                            else
                                popModel.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
                        }
                        else
                        {
                            popModel.OutsourceType = s.order.AssignType;
                        }
                        decimal totalPrice = 0;
                        decimal unitPrice = 0;
                        GetOutsourceOrderMaterialPrice(popModel, out unitPrice, out totalPrice);
                        s.order.UnitPrice = unitPrice;
                        s.order.TotalPrice = totalPrice;
                        orderBll.Update(s.order);


                    });
                    
                }
            }
            catch (Exception ex)
            {
                isOk = false;
                errorMsg = ex.Message;
            }
            if (isOk)
            {
                //labUpdateMsg.Text = "更新成功";
                //labUpdateMsg.ForeColor = System.Drawing.Color.Blue;
            }
            else
            {
                //labUpdateMsg.Text = "更新失败：" + errorMsg;
                //labUpdateMsg.ForeColor = System.Drawing.Color.Red;
            }
            BindOrderList();
        }
    }
}
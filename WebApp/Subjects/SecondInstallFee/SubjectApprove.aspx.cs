using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using BLL;
using Common;
using DAL;
using Models;
using System.Web.UI.WebControls;
using System.Transactions;

namespace WebApp.Subjects.SecondInstallFee
{
    public partial class SubjectApprove : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                
                BindSubject();
                BindData();
            }
        }

        void BindSubject()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                         join customer in CurrentContext.DbContext.Customer
                         on subject.CustomerId equals customer.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on subject.AddUserId equals user.UserId
                         where subject.Id == subjectId
                         select new
                         {
                             subject,
                             customer.CustomerName,
                             user.RealName

                         }).FirstOrDefault();
            if (model != null)
            {
                labSubjectNo.Text = model.subject.SubjectNo;
                labSubjectName.Text = model.subject.SubjectName;
                labAddUserName.Text = model.RealName;
                labCustomerName.Text = model.CustomerName;
                int subjectType = model.subject.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                labRegion.Text = model.subject.PriceBlongRegion;
               
            }
        }

        void BindData()
        {
            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        //join subject in CurrentContext.DbContext.Subject
                        //on order.SubjectId equals subject.Id
                        where order.SubjectId == subjectId
                        && order.IsSubmit == 1
                        select new
                        {
                            shop,
                            order,
                            order.OrderType
                            //subject
                        }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            orderListRepeater.DataSource = list.OrderBy(s => s.shop.Id).ThenBy(s => s.order.OrderType).ThenBy(s => s.order.Sheet).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            orderListRepeater.DataBind();

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void orderListRepeater_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object dataItem = e.Item.DataItem;
                if (dataItem != null)
                {
                    object orderTypeObj = dataItem.GetType().GetProperty("OrderType").GetValue(dataItem, null);
                    int orderType = orderTypeObj!=null?int.Parse(orderTypeObj.ToString()):1;
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType.ToString());
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text.Trim();
            SubjectBLL subjectBll = new SubjectBLL();
            bool isApproveOk = false;
            string msg = string.Empty;
            string url = string.Empty;
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    
                    Models.Subject model = subjectBll.GetModel(subjectId);
                    if (model != null)
                    {
                        var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                                    join shop in CurrentContext.DbContext.Shop
                                    on order.ShopId equals shop.Id
                                    join subject in CurrentContext.DbContext.Subject
                                    on order.SubjectId equals subject.Id
                                    join guidance in CurrentContext.DbContext.SubjectGuidance
                                    on subject.GuidanceId equals guidance.ItemId
                                    where order.SubjectId == subjectId
                                    && order.IsSubmit == 1
                                    && order.ApproveState != 1
                                    select new
                                    {
                                        order,
                                        shop,
                                        guidance,
                                        guidance.PriceItemId
                                    }).ToList();

                        if (list.Any())
                        {
                            string unitName = string.Empty;
                            FinalOrderDetailTempBLL finalOrderTempBll = new FinalOrderDetailTempBLL();
                            
                            int materialPriceItemId = list[0].PriceItemId ?? 0;


                            FinalOrderDetailTemp finalOrderTempModel;
                            RegionOrderDetailBLL orderBll = new RegionOrderDetailBLL();
                            RegionOrderDetail orderModel;

                            list.ForEach(o =>
                            {

                                orderModel = new RegionOrderDetail();
                                orderModel = o.order;
                                orderModel.ApproveState = result;
                                orderModel.ApproveDate = DateTime.Now;
                                orderBll.Update(orderModel);

                                if (result == 1)
                                {
                                    int OrderType = o.order.OrderType ?? 1;
                                    
                                    finalOrderTempModel = new FinalOrderDetailTemp();
                                    finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                    finalOrderTempModel.AgentName = o.shop.AgentName;
                                    finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                    finalOrderTempModel.Channel = o.shop.Channel;
                                    finalOrderTempModel.City = o.shop.CityName;
                                    finalOrderTempModel.CityTier = o.shop.CityTier;
                                    finalOrderTempModel.Contact = o.shop.Contact1;
                                    finalOrderTempModel.Format = o.shop.Format;
                                    finalOrderTempModel.Gender = o.order.Gender;
                                    finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                    finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                    finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                    finalOrderTempModel.LocationType = o.shop.LocationType;
                                    finalOrderTempModel.MaterialSupport = o.order.MaterialSupport;
                                    finalOrderTempModel.OrderType = o.order.OrderType;
                                    finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                    finalOrderTempModel.POSScale = o.order.POSScale;
                                    finalOrderTempModel.InstallPricePOSScale = o.order.POSScale;
                                    finalOrderTempModel.InstallPriceMaterialSupport = o.order.MaterialSupport;

                                    finalOrderTempModel.Province = o.shop.ProvinceName;
                                    finalOrderTempModel.Quantity = o.order.Quantity;
                                    finalOrderTempModel.Region = o.shop.RegionName;
                                    finalOrderTempModel.ChooseImg = o.order.ChooseImg;
                                    finalOrderTempModel.Remark = o.order.Remark;
                                    //finalOrderTempModel.Remark = "分区补单";
                                    finalOrderTempModel.Sheet = o.order.Sheet;
                                    finalOrderTempModel.ShopId = o.shop.Id;
                                    finalOrderTempModel.ShopName = o.shop.ShopName;
                                    finalOrderTempModel.ShopNo = o.shop.ShopNo;

                                    finalOrderTempModel.SubjectId = subjectId;
                                    finalOrderTempModel.Tel = o.shop.Tel1;
                                    finalOrderTempModel.MachineFrame = o.order.MachineFrame;
                                    //finalOrderTempModel.LevelNum = o.order.LevelNum;
                                    decimal width = o.order.GraphicWidth ?? 0;
                                    decimal length = o.order.GraphicLength ?? 0;
                                    finalOrderTempModel.GraphicLength = length;
                                    finalOrderTempModel.GraphicMaterial = o.order.GraphicMaterial;
                                    finalOrderTempModel.GraphicWidth = width;
                                    //finalOrderTempModel.UnitPrice = GetMaterialPrice(o.order.GraphicMaterial);
                                    finalOrderTempModel.PositionDescription = o.order.PositionDescription;
                                    finalOrderTempModel.Area = (width * length) / 1000000;
                                    finalOrderTempModel.InstallPositionDescription = o.order.PositionDescription;
                                    
                                    finalOrderTempModel.IsValid = o.order.IsValid;
                                    decimal unitPrice = 0;
                                    decimal totalPrice = 0;
                                    if (!string.IsNullOrWhiteSpace(o.order.GraphicMaterial))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = o.order.GraphicMaterial;
                                        pop.GraphicLength = o.order.GraphicLength;
                                        pop.GraphicWidth = o.order.GraphicWidth;
                                        pop.Quantity = o.order.Quantity;
                                        pop.PriceItemId = materialPriceItemId;
                                        unitPrice = GetMaterialUnitPrice(pop, out totalPrice, out unitName);
                                    }
                                    finalOrderTempModel.UnitPrice = unitPrice;
                                    finalOrderTempModel.TotalPrice = totalPrice;
                                   
                                    finalOrderTempModel.ShopStatus = o.shop.Status;
                                    finalOrderTempModel.OrderPrice = o.order.Price;
                                    finalOrderTempModel.PayOrderPrice = o.order.PayPrice;
                                    finalOrderTempModel.GuidanceId = o.guidance.ItemId;
                                    finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                    finalOrderTempBll.Add(finalOrderTempModel);
                                }
                            });

                        }
                        model.ApproveState = result;
                        model.ApproveDate = DateTime.Now;
                        model.ApproveUserId = CurrentUser.UserId;
                        subjectBll.Update(model);
                        if (!string.IsNullOrWhiteSpace(remark))
                        {
                            remark = remark.Replace("\r\n", "<br/>");
                        }
                        ApproveInfo approveModel = new ApproveInfo() { AddDate = DateTime.Now, AddUserId = CurrentUser.UserId, Remark = remark, Result = result, SubjectId = subjectId };
                        new ApproveInfoBLL().Add(approveModel);
                        tran.Complete();
                        isApproveOk = true;
                        if (!string.IsNullOrWhiteSpace(model.SupplementRegion))
                        {
                            url = "/Subjects/RegionSubject/ApproveList.aspx";
                        }
                        else
                        {
                            url = "/Subjects/ApproveList.aspx";
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            if (isApproveOk)
            {
                Alert("审批成功！", url);
            }
            else
                Alert("审批失败！");
        }
    }
}
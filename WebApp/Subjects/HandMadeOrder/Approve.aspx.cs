using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using Common;

namespace WebApp.Subjects.HandMadeOrder
{
    public partial class Approve : BasePage
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
                BindPOPList();
                //BindHCList();
            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                join user in CurrentContext.DbContext.UserInfo
                                on subject.AddUserId equals user.UserId

                                where subject.Id == subjectId
                                select new
                                {
                                    subject,
                                    customer.CustomerName,
                                    AddUserName = user.RealName
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                labSubjectNo.Text = subjectModel.subject.SubjectNo;
                labSubjectName.Text = subjectModel.subject.SubjectName;
                labOutSubjectName.Text = subjectModel.subject.OutSubjectName;
                labBeginDate.Text = subjectModel.subject.BeginDate != null ? DateTime.Parse(subjectModel.subject.BeginDate.ToString()).ToShortDateString() : "";
                labEndDate.Text = subjectModel.subject.EndDate != null ? DateTime.Parse(subjectModel.subject.EndDate.ToString()).ToShortDateString() : "";
                labAddUserName.Text = subjectModel.AddUserName;

                labCustomerName.Text = subjectModel.CustomerName;
                int subjectType = subjectModel.subject.SubjectType ?? 1;
                //labSubjectType.Text = subjectType == 1 ? "POP订单" : subjectType == 2 ? "费用订单" : "补单";

                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                labRemark.Text = subjectModel.subject.Remark;

            }
        }

        void BindPOPList()
        {
            var list = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        //&& order.IsSubmit == 1 && (order.ApproveState == null || order.ApproveState == 0)
                        select new
                        {
                            order,
                            shop
                        }).ToList();

            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            popList.DataSource = list.OrderBy(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            popList.DataBind();
        }

        void BindHCList()
        {
            var list = (from order in CurrentContext.DbContext.HCOrderDetail
                        join pop in CurrentContext.DbContext.POP
                        on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop.ShopId, pop.GraphicNo, pop.Sheet }
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            pop,
                            shop
                        }).ToList();
            string shopNo = txtShopNo2.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPager2.RecordCount = list.Count;
            this.AspNetPager2.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager2.CurrentPageIndex, this.AspNetPager2.PageCount, this.AspNetPager2.RecordCount, this.AspNetPager2.PageSize });
            hcList.DataSource = list.OrderBy(s => s.order.ShopId).Skip((AspNetPager2.CurrentPageIndex - 1) * AspNetPager2.PageSize).Take(AspNetPager2.PageSize).ToList();

            hcList.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindPOPList();
        }


        protected void AspNetPager2_PageChanged(object sender, EventArgs e)
        {
            BindHCList();
        }

        protected void btnSreach1_Click(object sender, EventArgs e)
        {
            BindPOPList();
        }

        protected void btnSreach2_Click(object sender, EventArgs e)
        {
            BindHCList();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text.Trim();
            SubjectBLL subjectBll = new SubjectBLL();
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    Models.Subject model = subjectBll.GetModel(subjectId);
                    if (model != null)
                    {

                        

                        int materialPriceItemId = 0;
                        string subjectCategoryName = string.Empty;
                        FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                        FinalOrderDetailTemp orderModel;


                        
                        //将原项目西区的订单全部删除
                        var oldOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                            join shop in CurrentContext.DbContext.Shop
                                            on order.ShopId equals shop.Id
                                            where shop.RegionName.ToLower() == "west"
                                            && order.SubjectId == model.HandMakeSubjectId
                                            select order).ToList();
                        if (oldOrderList.Any())
                        {
                            List<int> orderIList = oldOrderList.Select(s => s.Id).ToList();
                            orderBll.Delete(s => orderIList.Contains(s.Id));
                        }

                        if (model.SubjectType == (int)SubjectTypeEnum.HC订单 || model.SubjectType == (int)SubjectTypeEnum.分区补单 || model.SubjectType == (int)SubjectTypeEnum.分区增补 || model.SubjectType == (int)SubjectTypeEnum.新开店订单)
                        {
                            orderBll.Delete(s => s.RegionSupplementId == subjectId);
                        }
                        else
                        {
                            orderBll.Delete(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0);
                        }
                        //orderBll.Delete(s => s.SubjectId == subjectId);


                        HandMadeOrderDetailBLL detailBll = new HandMadeOrderDetailBLL();
                        HandMadeOrderDetail detailModel;

                        
                        //pop手工单
                        var handOrderList = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                                             join subject in CurrentContext.DbContext.Subject
                                             on order.SubjectId equals subject.Id
                                             join shop in CurrentContext.DbContext.Shop
                                             on order.ShopId equals shop.Id
                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                             on subject.GuidanceId equals guidance.ItemId
                                             join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                             on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                                             from subjectCategory in categoryTemp.DefaultIfEmpty()
                                             where order.SubjectId == subjectId
                                             //&& order.IsSubmit == 1 && (order.ApproveState == null || order.ApproveState == 0)
                                             select new
                                             {
                                                 order,
                                                 subject,
                                                 shop,
                                                 guidance.PriceItemId,
                                                 subjectCategory
                                             }).ToList();

                        if (handOrderList.Any())
                        {
                            materialPriceItemId = handOrderList[0].PriceItemId ?? 0;
                            subjectCategoryName = handOrderList[0].subjectCategory != null ? handOrderList[0].subjectCategory.CategoryName : string.Empty;
                            string unitName = string.Empty;
                            handOrderList.ForEach(s =>
                            {
                                detailModel = s.order;
                                detailModel.ApproveState = result;
                                detailBll.Update(detailModel);
                                if (result == 1)
                                {

                                    if (!string.IsNullOrWhiteSpace(s.order.Operation) && s.order.Operation.Contains("删除"))
                                    {

                                        //orderBll.Delete(o => o.ShopId == s.order.ShopId && o.Sheet.ToLower() == s.order.Sheet.ToLower() && o.Gender == s.order.Gender && o.GraphicLength == s.order.GraphicLength && o.GraphicWidth == s.order.GraphicWidth && o.SubjectId == s.subject.HandMakeSubjectId);

                                        //查找同一项目的所有补单
                                        List<int> handOrderIdList = subjectBll.GetList(sub => sub.HandMakeSubjectId == s.subject.HandMakeSubjectId).Select(sub => sub.Id).ToList();
                                        handOrderIdList.Add(s.subject.HandMakeSubjectId ?? 0);
                                        if (handOrderIdList.Any())
                                        {
                                            orderBll.Delete(o => o.ShopId == s.order.ShopId && o.Sheet.ToLower() == s.order.Sheet.ToLower() && o.Gender == s.order.Gender && o.GraphicLength == s.order.GraphicLength && o.GraphicWidth == s.order.GraphicWidth && handOrderIdList.Contains(o.SubjectId ?? 0));

                                        }
                                    }
                                    else
                                    {
                                        orderModel = new FinalOrderDetailTemp();
                                        //orderModel.Area = s.order.Area;
                                        orderModel.ChooseImg = s.order.ChooseImg;
                                        orderModel.Gender = s.order.Gender;
                                        orderModel.GraphicLength = s.order.GraphicLength;
                                        orderModel.GraphicMaterial = s.order.GraphicMaterial;
                                        orderModel.GraphicWidth = s.order.GraphicWidth;
                                        orderModel.LevelNum = s.order.LevelNum;
                                        orderModel.OrderType = s.order.OrderType;
                                        orderModel.PositionDescription = s.order.PositionDescription;
                                        orderModel.Quantity = s.order.Quantity;
                                        orderModel.Remark = s.order.Remark;
                                        orderModel.Sheet = s.order.Sheet;
                                        orderModel.ShopId = s.order.ShopId;
                                        orderModel.ShopNo = s.shop.ShopNo;

                                        orderModel.ShopName = s.shop.ShopName;
                                        orderModel.Region = s.shop.RegionName;
                                        orderModel.Province = s.shop.ProvinceName;
                                        orderModel.City = s.shop.CityName;
                                        orderModel.CityTier = s.shop.CityTier;
                                        orderModel.IsInstall = s.shop.IsInstall;
                                        orderModel.BCSIsInstall = s.shop.BCSIsInstall;
                                        orderModel.AgentCode = s.shop.AgentCode;
                                        orderModel.AgentName = s.shop.AgentName;
                                        orderModel.POPAddress = s.shop.POPAddress;
                                        orderModel.Contact = s.shop.Contact1;
                                        orderModel.Tel = s.shop.Tel1;
                                        orderModel.Channel = s.shop.Channel;

                                        orderModel.Format = s.shop.Format;
                                        orderModel.LocationType = s.shop.LocationType;
                                        orderModel.BusinessModel = s.shop.BusinessModel;
                                        orderModel.SubjectId = subjectId;
                                        orderModel.IsPOPMaterial = 1;
                                        orderModel.MaterialSupport = s.order.MaterialSupport;

                                        decimal width = s.order.GraphicWidth ?? 0;
                                        decimal length = s.order.GraphicLength ?? 0;
                                        orderModel.Area = (width * length) / 1000000;
                                        orderModel.POSScale = s.order.POSScale;
                                        orderModel.InstallPricePOSScale = s.order.POSScale;
                                        orderModel.InstallPriceMaterialSupport = s.order.MaterialSupport;
                                        //if (!string.IsNullOrWhiteSpace(s.shop.Format) && (s.shop.Format.ToLower().Contains("kids") || s.shop.Format.ToLower().Contains("infant")))
                                        if (subjectCategoryName == "童店")
                                            orderModel.InstallPriceMaterialSupport = "Others";
                                        orderModel.InstallPositionDescription = s.order.InstallPositionDescription;
                                        orderModel.MachineFrame = s.order.MachineFrame;

                                        decimal unitPrice = 0;
                                        decimal totalPrice = 0;
                                        if (!string.IsNullOrWhiteSpace(s.order.GraphicMaterial))
                                        {
                                            POP pop = new POP();
                                            pop.GraphicMaterial = s.order.GraphicMaterial;
                                            pop.GraphicLength = s.order.GraphicLength;
                                            pop.GraphicWidth = s.order.GraphicWidth;
                                            pop.Quantity = s.order.Quantity;
                                            pop.PriceItemId = materialPriceItemId;
                                            unitPrice = GetMaterialUnitPrice(pop, out totalPrice, out unitName);
                                        }
                                        orderModel.UnitPrice = unitPrice;
                                        orderModel.TotalPrice = totalPrice;
                                        orderModel.ShopStatus = s.shop.Status;
                                        orderModel.GuidanceId = s.subject.GuidanceId;
                                        orderModel.CSUserId = s.shop.CSUserId;
                                        orderBll.Add(orderModel);
                                    }
                                }
                            });
                        }

                        model.ApproveState = result;
                        model.ApproveUserId = CurrentUser.UserId;
                        model.ApproveDate = DateTime.Now;
                        subjectBll.Update(model);
                        if (!string.IsNullOrWhiteSpace(remark))
                        {
                            remark = remark.Replace("\r\n", "<br/>");
                        }
                        ApproveInfo approveModel = new ApproveInfo() { AddDate = DateTime.Now, AddUserId = CurrentUser.UserId, Remark = remark, Result = result, SubjectId = subjectId };
                        new ApproveInfoBLL().Add(approveModel);
                        if (result == 1)
                        {
                            OrderChangeApplicationDetailBLL changeApplicationBll = new OrderChangeApplicationDetailBLL();
                            OrderChangeApplicationDetail changeModel = changeApplicationBll.GetList(s => s.SubjectId == subjectId && s.State == 1).FirstOrDefault();
                            if (changeModel != null)
                            {
                                changeModel.State = 2;
                                changeModel.FinishDate = DateTime.Now;
                                changeModel.FinishUserId = CurrentUser.UserId;
                                changeApplicationBll.Update(changeModel);
                            }
                            SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(model.GuidanceId ?? 0);
                            if (guidanceModel != null && guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install && (model.SubjectType != (int)SubjectTypeEnum.二次安装 && model.SubjectType != (int)SubjectTypeEnum.费用订单 && model.SubjectType != (int)SubjectTypeEnum.新开店安装费))
                            {
                                SaveInstallPrice(guidanceModel.ItemId, subjectId, model.SubjectType ?? 1);
                            }
                            else if (guidanceModel != null && guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Promotion && (guidanceModel.HasExperssFees ?? true) && (model.SubjectType != (int)SubjectTypeEnum.二次安装 && model.SubjectType != (int)SubjectTypeEnum.费用订单 && model.SubjectType != (int)SubjectTypeEnum.新开店安装费))
                            {
                                SaveExpressPrice(guidanceModel.ItemId, subjectId, model.SubjectType ?? 1);
                            }
                            else if (guidanceModel != null && guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Delivery && (model.SubjectType != (int)SubjectTypeEnum.二次安装 && model.SubjectType != (int)SubjectTypeEnum.费用订单 && model.SubjectType != (int)SubjectTypeEnum.新开店安装费))
                            {
                                SaveExpressPriceForDelivery(guidanceModel.ItemId, subjectId, model.SubjectType ?? 1, guidanceModel.ExperssPrice);
                            }
                            //外协自动分单
                            AutoAssignOutsourceOrder(subjectId, model.SubjectType ?? 1);
                        }
                        tran.Complete();
                        Alert("审批成功！", "/Subjects/ApproveList.aspx");
                    }
                }
                catch (Exception ex)
                {
                    Alert("审批失败：" + ex.Message);
                }
            }
        }
    }
}
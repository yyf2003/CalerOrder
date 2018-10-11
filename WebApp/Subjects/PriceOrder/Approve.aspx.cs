using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;
using System.Transactions;

namespace WebApp.Subjects.PriceOrder
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
                BindOrderData();
                
            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                join guidance in CurrentContext.DbContext.SubjectGuidance
                                on subject.GuidanceId equals guidance.ItemId
                                where subject.Id == subjectId
                                select new
                                {
                                    subject.SubjectName,
                                    subject.SubjectNo,
                                    customer.CustomerName,
                                    subject.CustomerId,
                                    subject.SubjectType,
                                    subject.GuidanceId,
                                    guidance.ItemName
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                labGuidanceName.Text = subjectModel.ItemName;
                labSubjectName.Text = subjectModel.SubjectName;
                labSubjectNo.Text = subjectModel.SubjectNo;
                labCustomer.Text = subjectModel.CustomerName;
                int subjectType = subjectModel.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());
               
            }
        }

        void BindOrderData()
        {
            var orderList = new PriceOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
            AspNetPager1.RecordCount = orderList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            orderListRepeater.DataSource = orderList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            orderListRepeater.DataBind();
            
        }

        protected void orderListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                PriceOrderDetail model = (PriceOrderDetail)e.Item.DataItem;
                if (model != null)
                {
                    string orderType = (model.OrderType ?? 0).ToString();
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text;
            SubjectBLL subjectBll = new SubjectBLL();
            bool isApproveOk = false;
            string msg = "ok";
            //int subjectType = 1;
            //using (TransactionScope tran = new TransactionScope())
            //{

                try
                {
                    Models.Subject model = subjectBll.GetModel(subjectId);
                    if (model != null)
                    {

                        if (result == 1)
                        {
                            PriceOrderDetailBLL bll = new PriceOrderDetailBLL();
                            //var list = bll.GetList(s=>s.SubjectId==subjectId);
                            var list = (from order in CurrentContext.DbContext.PriceOrderDetail
                                       join shop in CurrentContext.DbContext.Shop
                                       on order.ShopId equals shop.Id
                                       where order.SubjectId == subjectId
                                       select new {
                                         order,
                                         shop
                                       }).ToList();
                            if (list.Any())
                            {
                                
                                FinalOrderDetailTempBLL finalOrderTempBll = new FinalOrderDetailTempBLL();
                                FinalOrderDetailTemp finalOrderTempModel;
                                finalOrderTempBll.Delete(s => s.SubjectId == subjectId);
                                new QuoteOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                                list.ForEach(o =>
                                {

                                    
                                    finalOrderTempModel = new FinalOrderDetailTemp();
                                    finalOrderTempModel.AddDate = o.order.AddDate;
                                    finalOrderTempModel.AddUserId = model.AddUserId;
                                    finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                    finalOrderTempModel.AgentName = o.shop.AgentName;
                                    finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                    finalOrderTempModel.Channel = o.shop.Channel;
                                    finalOrderTempModel.City = o.shop.CityName;
                                    finalOrderTempModel.CityTier = o.shop.CityTier;
                                    finalOrderTempModel.Contact = o.shop.Contact1;
                                    finalOrderTempModel.Format = o.shop.Format;
                                    //finalOrderTempModel.Gender = o.order.Gender;
                                    //finalOrderTempModel.GraphicNo = o.order.GraphicNo;
                                    finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                    finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                    finalOrderTempModel.LocationType = o.shop.LocationType;
                                    finalOrderTempModel.MaterialSupport = "";
                                    finalOrderTempModel.OrderType = o.order.OrderType;
                                    finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                    finalOrderTempModel.POSScale = "";
                                    finalOrderTempModel.InstallPricePOSScale = "";
                                    finalOrderTempModel.InstallPriceMaterialSupport ="";
                                    finalOrderTempModel.Province = o.shop.ProvinceName;
                                    finalOrderTempModel.Quantity =o.order.Quantity;
                                    finalOrderTempModel.Region = o.shop.RegionName;
                                    finalOrderTempModel.ChooseImg = "";
                                    finalOrderTempModel.Remark = o.order.Remark;
                                    //finalOrderTempModel.Remark = "分区补单";
                                    finalOrderTempModel.Sheet = "";
                                    finalOrderTempModel.ShopId = o.shop.Id;
                                    finalOrderTempModel.ShopName = o.shop.ShopName;
                                    finalOrderTempModel.ShopNo = o.shop.ShopNo;

                                    finalOrderTempModel.SubjectId = subjectId;
                                    finalOrderTempModel.Tel = o.shop.Tel1;
                                    finalOrderTempModel.MachineFrame = "";
                                    //finalOrderTempModel.LevelNum = o.order.LevelNum;
                                    decimal width =0;
                                    decimal length = 0;
                                    finalOrderTempModel.GraphicLength = length;
                                    finalOrderTempModel.GraphicMaterial = "";
                                    finalOrderTempModel.GraphicWidth = width;
                                    //finalOrderTempModel.UnitPrice = GetMaterialPrice(o.order.GraphicMaterial);
                                    finalOrderTempModel.PositionDescription = o.order.Contents;
                                    finalOrderTempModel.Area = (width * length) / 1000000;
                                    finalOrderTempModel.InstallPositionDescription = "";
                                    finalOrderTempModel.RegionSupplementId = 0;
                                    
                                    decimal unitPrice = 0;
                                    decimal totalPrice = 0;
                                    
                                    finalOrderTempModel.UnitPrice = unitPrice;
                                    finalOrderTempModel.TotalPrice = totalPrice;
                                    finalOrderTempModel.IsFromRegion = false;
                                    finalOrderTempModel.ShopStatus = o.shop.Status;
                                    finalOrderTempModel.OrderPrice = o.order.Amount;
                                    finalOrderTempModel.PayOrderPrice = o.order.PayAmount;
                                    finalOrderTempModel.GuidanceId = o.order.GuidanceId;
                                    finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                    if ((o.order.OutsourceId ?? 0) > 0)
                                    {
                                        finalOrderTempModel.ProduceOutsourceId = o.order.OutsourceId;
                                    }
                                    else if ((model.OutsourceId ?? 0) > 0)
                                    {
                                        finalOrderTempModel.ProduceOutsourceId = model.OutsourceId;
                                    }
                                    finalOrderTempBll.Add(finalOrderTempModel);
                                    //new BasePage().SaveQuotationOrder(finalOrderTempModel, false);
                                });
                            }
                            
                        }


                        //subjectType = model.SubjectType ?? 1;
                        model.ApproveState = result;
                        model.ApproveDate = DateTime.Now;
                        model.ApproveUserId = CurrentUser.UserId;
                        subjectBll.Update(model);
                        if (!string.IsNullOrWhiteSpace(remark))
                        {
                            remark = remark.Replace("\r\n", "<br/>");
                        }
                        ApproveInfo approveModel = new ApproveInfo();
                        approveModel.AddDate = DateTime.Now;
                        approveModel.AddUserId = CurrentUser.UserId;
                        approveModel.Remark = remark;
                        approveModel.Result = result;
                        approveModel.SubjectId = subjectId;
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
                            //外协自动分单
                            AutoAssignPriceOrder(subjectId);
                            //保存报价单
                            new BasePage().SaveQuotationOrder(model.GuidanceId??0, subjectId, model.SubjectType ?? 0);
                        }
                        //tran.Complete();
                        isApproveOk = true;
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            //}
            if (isApproveOk)
            {

                string url = "/Subjects/ApproveList.aspx";
                //Alert("审批成功！", url);
                ExcuteJs("ApproveStae", msg, url);
            }
            else
            {
                //Alert("提交失败！");
                ExcuteJs("ApproveStae", msg, "");
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindOrderData();
        }
    }
}
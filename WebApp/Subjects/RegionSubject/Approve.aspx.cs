using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using Common;
using BLL;
using System.Transactions;
using Models;
using NPOI.SS.UserModel;
using System.Configuration;
using System.IO;
using System.Text;

namespace WebApp.Subjects.RegionSubject
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
                //BindData();
            }
        }

        void BindSubject()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                         join guidance in CurrentContext.DbContext.SubjectGuidance
                         on subject.GuidanceId equals guidance.ItemId
                         join customer in CurrentContext.DbContext.Customer
                         on subject.CustomerId equals customer.Id
                         join user in CurrentContext.DbContext.UserInfo
                         on subject.AddUserId equals user.UserId
                         where subject.Id == subjectId
                         select new
                         {
                             subject,
                             customer.CustomerName,
                             user.RealName,
                             guidance.ItemName

                         }).FirstOrDefault();
            if (model != null)
            {
                labGuidanceName.Text = model.ItemName;
                labSubjectNo.Text = model.subject.SubjectNo;
                labSubjectName.Text = model.subject.SubjectName;
                labAddUserName.Text = model.RealName;
                labCustomerName.Text = model.CustomerName;
                int subjectType = model.subject.SubjectType ?? 1;
                hfSubjectType.Value = subjectType.ToString();
                labSubjectType.Text = CommonMethod.GetEnumDescription<SubjectTypeEnum>(subjectType.ToString());
                labRegion.Text = model.subject.SupplementRegion;

                if (subjectType == (int)SubjectTypeEnum.分区增补 || subjectType == (int)SubjectTypeEnum.新开店订单)
                {
                    Panel1.Visible = false;
                    Panel2.Visible = true;
                    BindOrder1();
                }
                else
                {
                    Panel1.Visible = true;
                    Panel2.Visible = false;
                    BindData();
                }
            }
            else
            {
                btnSubmit.Enabled = false;
            }
        }

        void BindData()
        {
            List<Order350Model> orderList = new List<Order350Model>();
            
                var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id
                            join subject in CurrentContext.DbContext.Subject
                            on order.HandMakeSubjectId equals subject.Id
                            where order.SubjectId == subjectId
                            //&& order.IsSubmit == 1 
                            //&& (order.ApproveState == null || order.ApproveState == 0)
                            select new
                            {
                                shop,
                                order,
                                subject.SubjectName
                            }).OrderBy(s=>s.shop.Id).ToList();

                if (list.Any())
                {
                    list.ForEach(s =>
                    {
                        Order350Model model = new Order350Model();
                        model.SubjectName = s.SubjectName;
                        model.ChooseImg = s.order.ChooseImg;

                        model.OrderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                        model.Region = s.shop.RegionName;
                        model.City = s.shop.CityName;
                        model.County = s.shop.AreaName;
                        model.CityTier = s.shop.CityTier;
                        model.Format = s.shop.Format;
                        model.Gender = s.order.Gender;
                        model.GraphicLength = s.order.GraphicLength != null ? double.Parse(s.order.GraphicLength.ToString()) : 0;
                        model.GraphicMaterial = s.order.GraphicMaterial;
                        model.GraphicWidth = s.order.GraphicWidth != null ? double.Parse(s.order.GraphicWidth.ToString()) : 0; ;
                        model.POPAddress = s.shop.POPAddress;
                        model.PositionDescription = s.order.PositionDescription;
                        model.Province = s.shop.ProvinceName;
                        model.Quantity = s.order.Quantity != null ? double.Parse(s.order.Quantity.ToString()) : 1;
                        model.Sheet = s.order.Sheet;
                        model.GraphicNo = s.order.GraphicNo;
                        model.ShopName = s.shop.ShopName;
                        model.ShopNo = s.shop.ShopNo;
                        model.OtherRemark = s.order.Remark;
                        model.POSScale = s.order.POSScale;
                        model.MaterialSupport = s.order.MaterialSupport;
                        model.ReceivePrice = double.Parse((s.order.Price ?? 0).ToString());
                        model.PayPrice = double.Parse((s.order.PayPrice ?? 0).ToString());
                        orderList.Add(model);
                    });
                }

            //物料
            var materialList = (from order in CurrentContext.DbContext.OrderMaterial
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                where order.RegionSupplementId == subjectId
                                select new
                                {
                                    shop,
                                    order,
                                    subject.SubjectName
                                }).ToList();

            if (materialList.Any())
            {
                materialList.ForEach(s =>
                {
                    Order350Model model = new Order350Model();
                    //model.ChooseImg = s.order.ChooseImg;
                    StringBuilder size = new StringBuilder();
                    if (s.order.MaterialLength != null && s.order.MaterialLength > 0 && s.order.MaterialWidth != null && s.order.MaterialWidth > 0)
                    {
                        size.AppendFormat("({0}*{1}", s.order.MaterialLength, s.order.MaterialWidth);
                        if (s.order.MaterialHigh != null && s.order.MaterialHigh > 0)
                            size.AppendFormat("*{0}", s.order.MaterialHigh);
                        size.Append(")");
                    }
                    model.OrderType = "物料";
                    model.Region = s.shop.RegionName;
                    model.City = s.shop.CityName;
                    model.County = s.shop.AreaName;
                    model.CityTier = s.shop.CityTier;
                    model.Format = s.shop.Format;
                    model.POPAddress = s.shop.POPAddress;
                    model.Province = s.shop.ProvinceName;
                    model.Quantity = s.order.MaterialCount != null ? double.Parse(s.order.MaterialCount.ToString()) : 1;
                    model.Sheet = s.order.Sheet;
                    model.PositionDescription = s.order.MaterialName + size.ToString();
                    model.ShopName = s.shop.ShopName;
                    model.ShopNo = s.shop.ShopNo;
                    model.OtherRemark = s.order.Remark;
                    model.SubjectName = s.SubjectName;
                    orderList.Add(model);
                });
            }





            AspNetPager1.RecordCount = orderList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            orderListRepeater.DataSource = orderList.OrderBy(s => s.ShopId).ThenBy(s=>s.OrderType).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            orderListRepeater.DataBind();

        }

        /// <summary>
        /// 新开店或者增补的
        /// </summary>
        void BindOrder1()
        {
            List<Order350Model> orderList = new List<Order350Model>();

            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        
                        where order.SubjectId == subjectId
                        && order.IsSubmit == 1 && (order.ApproveState == null || order.ApproveState == 0)
                        select new
                        {
                            shop,
                            order,
                            
                        }).OrderBy(s => s.shop.Id).ToList();

            if (list.Any())
            {
                list.ForEach(s =>
                {
                    Order350Model model = new Order350Model();
                    model.OrderType = CommonMethod.GeEnumName<OrderTypeEnum>((s.order.OrderType ?? 1).ToString());
                    if ((s.order.OrderType ?? 1) > (int)OrderTypeEnum.物料)
                    {
                        
                        model.Region = s.shop.RegionName;
                        model.City = s.shop.CityName;
                        model.County = s.shop.AreaName;
                        model.CityTier = s.shop.CityTier;
                        model.Format = s.shop.Format;
                       
                        model.Province = s.shop.ProvinceName;
                        model.Quantity = double.Parse((s.order.Quantity ?? 1).ToString());
                        model.ReceivePrice = double.Parse((s.order.Price ?? 0).ToString());
                        model.PayPrice = double.Parse((s.order.PayPrice ?? 0).ToString());
                       
                        model.ShopName = s.shop.ShopName;
                        model.ShopNo = s.shop.ShopNo;
                        model.OtherRemark = s.order.Remark;
                       
                        orderList.Add(model);
                    }
                    else
                    {
                        model.ChooseImg = s.order.ChooseImg;
                        model.Region = s.shop.RegionName;
                        model.City = s.shop.CityName;
                        model.County = s.shop.AreaName;
                        model.CityTier = s.shop.CityTier;
                        model.Format = s.shop.Format;
                        model.Gender = s.order.Gender;
                        model.GraphicLength = s.order.GraphicLength != null ? double.Parse(s.order.GraphicLength.ToString()) : 0;
                        model.GraphicMaterial = s.order.GraphicMaterial;
                        model.GraphicWidth = s.order.GraphicWidth != null ? double.Parse(s.order.GraphicWidth.ToString()) : 0; ;
                        model.POPAddress = s.shop.POPAddress;
                        model.PositionDescription = s.order.PositionDescription;
                        model.Province = s.shop.ProvinceName;
                        model.Quantity = s.order.Quantity != null ? double.Parse(s.order.Quantity.ToString()) : 1;
                        model.Sheet = s.order.Sheet;
                        model.GraphicNo = s.order.GraphicNo;
                        model.ShopName = s.shop.ShopName;
                        model.ShopNo = s.shop.ShopNo;
                        model.OtherRemark = s.order.Remark;
                        model.POSScale = s.order.POSScale;
                        model.MaterialSupport = s.order.MaterialSupport;
                        orderList.Add(model);
                    }
                });
                //费用信息
                //var priceOrderList = (from price in CurrentContext.DbContext.RegionOrderPrice
                //                      join shop in CurrentContext.DbContext.Shop
                //                      on price.ShopId equals shop.Id
                //                      where price.SubjectId == subjectId
                //                      && price.IsSubmit == 1 && (price.ApproveState == null || price.ApproveState == 0)
                //                      select new { price, shop }).ToList();
                //if (priceOrderList.Any())
                //{
                //    priceOrderList.ForEach(s => {
                        
                //    });
                //}

            }
            AspNetPager2.RecordCount = orderList.Count;
            this.AspNetPager2.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager2.CurrentPageIndex, this.AspNetPager2.PageCount, this.AspNetPager2.RecordCount, this.AspNetPager2.PageSize });
            Repeater1.DataSource = orderList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).Skip((AspNetPager2.CurrentPageIndex - 1) * AspNetPager2.PageSize).Take(AspNetPager2.PageSize).ToList();
            Repeater1.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void AspNetPager2_PageChanged(object sender, EventArgs e)
        {
            BindOrder1();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text.Trim();
            SubjectBLL subjectBll = new SubjectBLL();
            bool isApproveOk = false;
            string msg = "ok";
            int subjectType = 1;
            int guidanceId = 0;
            Models.Subject model = subjectBll.GetModel(subjectId);
            //using (TransactionScope tran = new TransactionScope())
            //{
                try
                {
                    
                    SubjectGuidance guidanceModel = null;
                    int newSubjectId = subjectId;
                    if (model != null)
                    {
                        guidanceId = model.GuidanceId ?? 0;
                        subjectType = model.SubjectType ?? 1;
                        var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                                   join shop in CurrentContext.DbContext.Shop
                                   on order.ShopId equals shop.Id
                                   join subject in CurrentContext.DbContext.Subject
                                   on order.SubjectId equals subject.Id
                                   join guidance in CurrentContext.DbContext.SubjectGuidance
                                   on subject.GuidanceId equals guidance.ItemId
                                   
                                   where order.SubjectId == subjectId
                                   && order.IsSubmit == 1 
                                   && order.ApproveState!=1
                                   select new
                                   {
                                       order,
                                       shop,
                                       guidance,
                                       guidance.PriceItemId
                                   }).ToList();

                        if (list.Any())
                        {
                            guidanceModel = list[0].guidance;
                            FinalOrderDetailTempBLL finalOrderTempBll = new FinalOrderDetailTempBLL();
                            if (result==1)
                            {
                                //先删除旧数据
                                if (list[0].guidance.ActivityTypeId == (int)GuidanceTypeEnum.Others || model.SubjectType == (int)SubjectTypeEnum.正常单)
                                {
                                    finalOrderTempBll.Delete(s => s.SubjectId == subjectId);
                                    new QuoteOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                                }
                                else if (model.SubjectType == (int)SubjectTypeEnum.分区补单 || model.SubjectType == (int)SubjectTypeEnum.HC订单)
                                {
                                    finalOrderTempBll.Delete(s => s.RegionSupplementId == subjectId);
                                    new QuoteOrderDetailBLL().Delete(s => s.RegionSupplementId == subjectId);
                                }
                            }


                            int materialPriceItemId = list[0].PriceItemId ?? 0;
                            
                            
                            FinalOrderDetailTemp finalOrderTempModel;
                            RegionOrderDetailBLL orderBll = new RegionOrderDetailBLL();
                            RegionOrderDetail orderModel;

                            //InstallPriceShopInfoBLL installPriceBll = new InstallPriceShopInfoBLL();
                            //InstallPriceShopInfo installPriceModel;

                            string subjectCategoryName = string.Empty;
                            string unitName = string.Empty;
                            bool useQuoteSetting = true;
                            if (list[0].guidance.ActivityTypeId == (int)GuidanceTypeEnum.Others)
                            {
                                useQuoteSetting = false;
                            }
                            list.ForEach(o =>
                            {
                                
                                subjectCategoryName = string.Empty;
                                if (o.order.HandMakeSubjectId != null && o.order.HandMakeSubjectId > 0)
                                {
                                    //分区补单和HC订单
                                    newSubjectId = o.order.HandMakeSubjectId ?? 0;
                                    //Subject HandMakeSubjectModel = subjectBll.GetModel(o.order.HandMakeSubjectId??0);
                                    var HandMakeSubjectModel = (from subject in CurrentContext.DbContext.Subject
                                                                join category1 in CurrentContext.DbContext.ADSubjectCategory
                                                                on subject.SubjectCategoryId equals category1.Id into categoryTemp
                                                                from subjectCategory in categoryTemp.DefaultIfEmpty()
                                                                where subject.Id == (o.order.HandMakeSubjectId ?? 0)
                                                                select new
                                                                {
                                                                    subject,
                                                                    subjectCategory
                                                                }).FirstOrDefault();
                                    if (HandMakeSubjectModel != null && HandMakeSubjectModel.subjectCategory!=null)
                                    {
                                        subjectCategoryName = HandMakeSubjectModel.subjectCategory.CategoryName;
                                    }
                                }

                                orderModel = new RegionOrderDetail();
                                orderModel = o.order;
                                orderModel.ApproveState = result;
                                orderModel.ApproveDate = DateTime.Now;
                                orderBll.Update(orderModel);

                                if (result == 1)
                                {
                                    int OrderType=o.order.OrderType ?? 1;
                                    
                                    finalOrderTempModel = new FinalOrderDetailTemp();
                                    finalOrderTempModel.AddDate = o.order.AddDate;
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

                                    if (subjectCategoryName == "童店" || (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant") || o.shop.Format.ToLower().Contains("ya"))))
                                        finalOrderTempModel.InstallPriceMaterialSupport = "Others";
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

                                    finalOrderTempModel.SubjectId = newSubjectId;
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
                                    finalOrderTempModel.RegionSupplementId = subjectId;
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
                                    finalOrderTempModel.IsFromRegion = true;
                                    finalOrderTempModel.ShopStatus = o.shop.Status;
                                    finalOrderTempModel.OrderPrice = o.order.Price;
                                    finalOrderTempModel.PayOrderPrice = o.order.PayPrice;
                                    finalOrderTempModel.GuidanceId = o.guidance.ItemId;
                                    finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                    finalOrderTempModel.UnitName = unitName;
                                    finalOrderTempBll.Add(finalOrderTempModel);
                                    //new BasePage().SaveQuotationOrder(finalOrderTempModel, useQuoteSetting);
                                }
                            });

                        }
                        // && (model.SubjectType == (int)SubjectTypeEnum.HC订单 || model.SubjectType == (int)SubjectTypeEnum.分区补单 || model.SubjectType == (int)SubjectTypeEnum.正常单)
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

                if (result == 1 && subjectType != (int)SubjectTypeEnum.新开店安装费 && subjectType != (int)SubjectTypeEnum.运费)
                {
                    new WebApp.Base.DelegateClass().SaveOutsourceOrder(guidanceId, subjectId);
                }
                //Alert("审批成功！", "ApproveList.aspx");
                ExcuteJs("ApproveStae", msg, "ApproveList.aspx");
            }
            else
            {
                //Alert("审批失败！");
                ExcuteJs("ApproveStae", msg, "");
            }
        }



    }
}
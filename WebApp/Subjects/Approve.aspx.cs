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
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Reflection;




namespace WebApp.Subjects
{
    public partial class Approve : BasePage
    {
        int subjectId;
        Dictionary<int, string> dic = new Dictionary<int, string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            dic.Add(1, "导入订单");
            dic.Add(2, "历史数据下单");
            dic.Add(3, "数据库生成订单");
            dic.Add(4, "零单");
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (!IsPostBack)
            {
                //BindSubject();
                //BindFinalOrder();
                //hfSubjectId.Value = subjectId.ToString();
            }
        }

        //void BindSubject()
        //{
        //    var subjectModel = (from subject in CurrentContext.DbContext.Subject
        //                       join customer in CurrentContext.DbContext.Customer
        //                       on subject.CustomerId equals customer.Id
        //                        where subject.Id==subjectId
        //                       select new {
        //                           subject,
        //                           customer.CustomerName
        //                       }).FirstOrDefault();
        //    if (subjectModel != null)
        //    {
        //        labSubjectNo.Text = subjectModel.subject.SubjectNo;
        //        labSubjectName.Text = subjectModel.subject.SubjectName;
        //        labOutSubjectName.Text = subjectModel.subject.OutSubjectName;
        //        labBeginDate.Text = subjectModel.subject.BeginDate != null ? DateTime.Parse(subjectModel.subject.BeginDate.ToString()).ToShortDateString() : "";
        //        labEndDate.Text = subjectModel.subject.EndDate != null ? DateTime.Parse(subjectModel.subject.EndDate.ToString()).ToShortDateString() : "";
        //        labContact.Text = subjectModel.subject.Contact;
        //        labTel.Text = subjectModel.subject.Tel;
        //        labCustomerName.Text = subjectModel.CustomerName;
        //        labAddType.Text = dic[subjectModel.subject.AddOrderType ?? 0];
        //        labRemark.Text = subjectModel.subject.Remark;
        //        hfCustomerId.Value =subjectModel.subject.CustomerId!=null ? subjectModel.subject.CustomerId.ToString():"0";
        //        hfPlanIds.Value = subjectModel.subject.SplitPlanIds;
        //    }
        //}

        //void BindFinalOrder()
        //{


        //    var list = from order in CurrentContext.DbContext.FinalOrderDetailTemp
        //               join region1 in CurrentContext.DbContext.Region
        //               on order.RegionId equals region1.Id into regionTemp
        //               join place1 in CurrentContext.DbContext.Place
        //               on order.ProvinceId equals place1.ID into provinceTemp
        //               join place2 in CurrentContext.DbContext.Place
        //               on order.CityId equals place2.ID into cityTemp
        //               from region in regionTemp.DefaultIfEmpty()
        //               from province in provinceTemp.DefaultIfEmpty()
        //               from city in cityTemp.DefaultIfEmpty()
        //               where order.SubjectId == subjectId
        //               select new
        //               {
        //                   order,
        //                   Region = region != null ? region.RegionName : "",
        //                   Province = province != null ? province.PlaceName : "",
        //                   City = city != null ? city.PlaceName : ""
        //               };
        //    AspNetPager1.RecordCount = list.Count();
        //    this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
        //    gvPOP.DataSource = list.ToList().OrderBy(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
        //    gvPOP.DataBind();


        //}

        /// <summary>
        /// 提交审批
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int result = int.Parse(rblApproveResult.SelectedValue);
            string remark = txtRemark.Text;
            SubjectBLL subjectBll = new SubjectBLL();
            bool isApproveOk = false;
            string msg = "ok";
            int guidanceId = 0;
            int subjectType = 1;
            Models.Subject model = subjectBll.GetModel(subjectId);
            using (TransactionScope tran = new TransactionScope())
            {

                try
                {
                   
                    if (model != null)
                    {
                        guidanceId = model.GuidanceId ?? 0;
                        subjectType = model.SubjectType ?? 1;
                        
                        if (result == 1)
                        {

                            //bool hasPriceOrder = false;
                            if (subjectType != (int)SubjectTypeEnum.新开店安装费 && subjectType != (int)SubjectTypeEnum.运费)
                            {
                                var list = (from order in CurrentContext.DbContext.PriceOrderDetail
                                            join shop in CurrentContext.DbContext.Shop
                                            on order.ShopId equals shop.Id
                                            where order.SubjectId == subjectId
                                            select new
                                            {
                                                order,
                                                shop
                                            }).ToList();
                                if (list.Any())
                                {
                                    //hasPriceOrder = true;
                                    FinalOrderDetailTempBLL finalOrderTempBll = new FinalOrderDetailTempBLL();
                                    FinalOrderDetailTemp finalOrderTempModel;
                                    finalOrderTempBll.Delete(s => s.SubjectId == subjectId && (s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费 || s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.其他费用));
                                    new QuoteOrderDetailBLL().Delete(s => s.SubjectId == subjectId && (s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费 || s.OrderType == (int)OrderTypeEnum.发货费 || s.OrderType == (int)OrderTypeEnum.其他费用));
                                    list.ForEach(o =>
                                    {
                                        finalOrderTempModel = new FinalOrderDetailTemp();
                                        finalOrderTempModel.AgentCode = o.shop.AgentCode;
                                        finalOrderTempModel.AgentName = o.shop.AgentName;
                                        finalOrderTempModel.BusinessModel = o.shop.BusinessModel;
                                        finalOrderTempModel.Channel = o.shop.Channel;
                                        finalOrderTempModel.City = o.shop.CityName;
                                        finalOrderTempModel.CityTier = o.shop.CityTier;
                                        finalOrderTempModel.Contact = o.shop.Contact1;
                                        finalOrderTempModel.Format = o.shop.Format;
                                        finalOrderTempModel.IsInstall = o.shop.IsInstall;
                                        finalOrderTempModel.BCSIsInstall = o.shop.BCSIsInstall;
                                        finalOrderTempModel.LocationType = o.shop.LocationType;
                                        finalOrderTempModel.MaterialSupport = "";
                                        finalOrderTempModel.OrderType = o.order.OrderType;
                                        finalOrderTempModel.POPAddress = o.shop.POPAddress;
                                        finalOrderTempModel.POSScale = "";
                                        finalOrderTempModel.InstallPricePOSScale = "";
                                        finalOrderTempModel.InstallPriceMaterialSupport = "";
                                        finalOrderTempModel.Province = o.shop.ProvinceName;
                                        finalOrderTempModel.Quantity = 1;
                                        finalOrderTempModel.Region = o.shop.RegionName;
                                        finalOrderTempModel.ChooseImg = "";
                                        finalOrderTempModel.Remark = o.order.Remark;
                                        finalOrderTempModel.Sheet = "";
                                        finalOrderTempModel.ShopId = o.shop.Id;
                                        finalOrderTempModel.ShopName = o.shop.ShopName;
                                        finalOrderTempModel.ShopNo = o.shop.ShopNo;
                                        finalOrderTempModel.SubjectId = subjectId;
                                        finalOrderTempModel.Tel = o.shop.Tel1;
                                        finalOrderTempModel.MachineFrame = "";

                                        finalOrderTempModel.GraphicLength = 0;
                                        finalOrderTempModel.GraphicMaterial = "";
                                        finalOrderTempModel.GraphicWidth = 0;
                                        finalOrderTempModel.PositionDescription = "";
                                        finalOrderTempModel.Area = 0;
                                        finalOrderTempModel.InstallPositionDescription = "";
                                        finalOrderTempModel.RegionSupplementId = 0;

                                        decimal unitPrice = 0;
                                        decimal totalPrice = 0;

                                        finalOrderTempModel.UnitPrice = unitPrice;
                                        finalOrderTempModel.TotalPrice = totalPrice;
                                        finalOrderTempModel.IsFromRegion = true;
                                        finalOrderTempModel.ShopStatus = o.shop.Status;
                                        finalOrderTempModel.OrderPrice = o.order.Amount;
                                        finalOrderTempModel.PayOrderPrice = o.order.PayAmount;
                                        finalOrderTempModel.GuidanceId = o.order.GuidanceId;
                                        finalOrderTempModel.CSUserId = o.shop.CSUserId;
                                        finalOrderTempBll.Add(finalOrderTempModel);
                                        //new BasePage().SaveQuotationOrder(finalOrderTempModel, false);
                                    });
                                }
                            }



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
                        ApproveInfo approveModel = new ApproveInfo();
                        approveModel.AddDate = DateTime.Now;
                        approveModel.AddUserId = CurrentUser.UserId;
                        approveModel.Remark = remark;
                        approveModel.Result = result;
                        approveModel.SubjectId = subjectId;
                        new ApproveInfoBLL().Add(approveModel);
                        tran.Complete();
                        isApproveOk = true;
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            if (isApproveOk)
            {
               
                if (result == 1 && subjectType != (int)SubjectTypeEnum.新开店安装费 && subjectType != (int)SubjectTypeEnum.运费)
                {
                    new WebApp.Base.DelegateClass().SaveOutsourceOrder(guidanceId, subjectId);
                }

                string url = "ApproveList.aspx";
                if (subjectType == (int)SubjectTypeEnum.正常单)
                    url = "/Subjects/RegionSubject/ApproveList.aspx";
                //Alert("审批成功！", url);
                ExcuteJs("ApproveStae", msg, url);
            }
            else
            {
                //Alert("提交失败！");
                ExcuteJs("ApproveStae",msg,"");
            }
        }


        delegate void deleSentErp();

        void Sent()
        {
            deleSentErp sentDele = new deleSentErp(SentToERP);
            AsyncCallback callback = new AsyncCallback(CallBackMethod);
            sentDele.BeginInvoke(callback, sentDele);
        }

        void CallBackMethod(IAsyncResult ia)
        {

        }

        void SentToERP()
        {

            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        where order.SubjectId == subjectId
                        && (order.IsDelete == null || order.IsDelete == false)
                        && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || (order.OrderType == 2))
                        select new
                        {
                            order,
                            subject,
                            shop,
                            customer.CustomerCode
                        }).ToList();
            if (list.Any())
            {

                var clentList = new ErpHostBLL().GetList(s => s.IsActive == null || s.IsActive == true);
                if (clentList.Any())
                {

                    PlaceBLL placeBll = new PlaceBLL();
                    foreach (ErpHost client in clentList)
                    {
                        var list1 = list;
                        if (!string.IsNullOrWhiteSpace(client.Provinces))
                        {
                            List<int> provinceIdList = StringHelper.ToIntList(client.Provinces, ',');
                            var provinceList = placeBll.GetList(s => provinceIdList.Contains(s.ID)).Select(s => s.PlaceName).Distinct().ToList();
                            list1 = list1.Where(s => provinceList.Contains(s.shop.ProvinceName)).ToList();
                        }
                        if (!string.IsNullOrWhiteSpace(client.Cities))
                        {
                            List<int> cityIdList = StringHelper.ToIntList(client.Cities, ',');
                            var cityList = placeBll.GetList(s => cityIdList.Contains(s.ID)).Select(s => s.PlaceName).Distinct().ToList();
                            list1 = list1.Where(s => cityList.Contains(s.shop.CityName)).ToList();
                        }
                        if (list1.Any())
                        {
                            try
                            {
                                List<CalerOrderModel> orderList = new List<CalerOrderModel>();
                                CalerOrderModel model;
                                list1.ForEach(s =>
                                {
                                    model = new CalerOrderModel();
                                    model.CustomerNo = list[0].CustomerCode;
                                    model.CompanyNo = client.ClientNo;
                                    model.AddDate = s.subject.AddDate;
                                    model.Address = s.shop.POPAddress;
                                    model.AgentCode = s.shop.AgentCode;
                                    model.AgentName = s.shop.AgentName;
                                    model.BeginDate = s.subject.BeginDate;
                                    model.Category = "";
                                    model.ChooseImg = s.order.ChooseImg;
                                    model.City = s.shop.CityName;
                                    model.EndDate = s.subject.EndDate;
                                    model.Gender = s.order.Gender;
                                    model.GraphicLength = s.order.GraphicLength != null ? int.Parse(s.order.GraphicLength.ToString()) : 0;
                                    model.GraphicWidth = s.order.GraphicWidth != null ? int.Parse(s.order.GraphicWidth.ToString()) : 0;
                                    model.GuidanceName = "";
                                    model.MaterialName = s.order.GraphicMaterial;
                                    model.OrderName = s.subject.SubjectName;
                                    model.OrderNo = s.subject.SubjectNo;
                                    model.PositionDescription = s.order.PositionDescription;
                                    model.Province = s.shop.ProvinceName;
                                    model.Quantity = s.order.Quantity ?? 1;
                                    model.Sheet = s.order.Sheet;
                                    model.ShopName = s.shop.ShopName;
                                    model.ShopNo = s.shop.ShopNo;
                                    orderList.Add(model);
                                });
                                string jsonStr = JsonConvert.SerializeObject(orderList);
                                HttpClient httpClient = new HttpClient();


                                WebClient wc = new WebClient();
                                wc.Headers.Add("Content-Type", "application/json");
                                Uri uri = new Uri(client.ClientHost + "/api/Order");
                                var data = System.Text.Encoding.UTF8.GetBytes(jsonStr);
                                wc.UploadDataAsync(uri, data);
                                wc.UploadDataCompleted += (sender, e) =>
                                {
                                    string result = string.Empty;
                                    try
                                    {
                                        result = System.Text.Encoding.UTF8.GetString(e.Result);
                                    }
                                    catch (UnauthorizedAccessException ex)
                                    {
                                        result = ex.Message;
                                    }
                                    catch (SocketException ex)
                                    {
                                        result = ex.Message;
                                    }
                                    catch (WebException ex)
                                    {
                                        result = ex.Message;
                                    }
                                    catch (TargetInvocationException ex)
                                    {
                                        result = ex.InnerException.Message;
                                    }
                                    finally
                                    {
                                        SendERPResult resultModel = new SendERPResult();
                                        resultModel.AddDate = DateTime.Now;
                                        resultModel.ErpHostId = client.Id;
                                        resultModel.SubjectId = subjectId;
                                        resultModel.SendResult = result == "ok" ? 1 : 2;
                                        resultModel.CallBackMsg = "客户端：" + result;
                                        new SendERPResultBLL().Add(resultModel);
                                    }
                                };

                            }
                            catch (Exception ex)
                            {
                                SendERPResult resultModel = new SendERPResult();
                                resultModel.AddDate = DateTime.Now;
                                resultModel.ErpHostId = client.Id;
                                resultModel.SubjectId = subjectId;
                                resultModel.SendResult = 2;
                                resultModel.CallBackMsg = "主机端：" + ex.Message;
                                new SendERPResultBLL().Add(resultModel);
                            }


                        }

                    }
                }
            }
        }

    }
}
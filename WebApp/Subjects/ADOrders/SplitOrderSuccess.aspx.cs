using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using System.Collections.Generic;
using Models;

namespace WebApp.Subjects.ADOrders
{
    public partial class SplitOrderSuccess : BasePage
    {
        public int subjectId;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            
            int splitState = 0;
            if (Request.QueryString["split"] != null)
            {
                splitState = int.Parse(Request.QueryString["split"]);
                if (splitState == 1)
                {
                    labState.Visible = true;
                }
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindFinalOrder();
                //StatisticsInfo();
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
                                    AddUserName = user.UserName
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
                labSubjectType.Text = subjectType == 1 ? "POP订单" : subjectType == 2 ? "费用订单" : "补单";
                labRemark.Text = subjectModel.subject.Remark;
                

            }
        }

        void BindFinalOrder()
        {
            FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            Subject subjectModel = new SubjectBLL().GetModel(subjectId);
            if (subjectModel != null)
            {
                if (subjectModel.SubjectType == (int)SubjectTypeEnum.HC订单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区补单 || subjectModel.SubjectType == (int)SubjectTypeEnum.分区增补 || subjectModel.SubjectType == (int)SubjectTypeEnum.新开店订单)
                {
                    orderList = orderBll.GetList(s => s.RegionSupplementId == subjectId && (s.IsDelete == null || s.IsDelete == false));
                }
                else
                {
                    orderList = orderBll.GetList(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0 && (s.IsDelete == null || s.IsDelete == false));
                }
            }
            //var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
            //            where order.SubjectId == subjectId
            //            && (order.IsDelete == null || order.IsDelete == false)
            //            select order).ToList();
            if (!string.IsNullOrWhiteSpace(txtShopNo.Text.Trim()))
            {
                orderList = orderList.Where(s => s.ShopNo.ToLower() == txtShopNo.Text.Trim().ToLower()).ToList();
            }
            if (!string.IsNullOrWhiteSpace(txtShopName.Text.Trim()))
            {
                orderList = orderList.Where(s => s.ShopName.Contains(txtShopNo.Text.Trim())).ToList();
            }


            AspNetPagerFinal.RecordCount = orderList.Count;
            this.AspNetPagerFinal.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPagerFinal.CurrentPageIndex, this.AspNetPagerFinal.PageCount, this.AspNetPagerFinal.RecordCount, this.AspNetPagerFinal.PageSize });
            repeater_FinalOrder.DataSource = orderList.OrderBy(s => s.ShopId).Skip((AspNetPagerFinal.CurrentPageIndex - 1) * AspNetPagerFinal.PageSize).Take(AspNetPagerFinal.PageSize).ToList();
            repeater_FinalOrder.DataBind();
            if (!IsPostBack)
            {
                if (orderList.Any())
                {
                    List<int> shopIdList = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                    shopIdList.Remove(0);
                    labShopCount.Text = shopIdList.Count.ToString();
                    decimal totalArea = 0;
                    decimal totalPrice = 0;
                    StatisticPOPTotalPrice(orderList, out totalPrice, out totalArea);
                    if (totalArea > 0)
                    {
                        labTotalArea.Text = Math.Round(totalArea, 2) + " 平方米";
                    }
                    else
                        labTotalArea.Text = "0";
                    if (totalPrice > 0)
                    {
                        labTotalPrice.Text = Math.Round(totalPrice, 2) + " 元";
                    }
                    else
                        labTotalPrice.Text = "0";
                }
            }


        }

        protected void btnSearchFinal_Click(object sender, EventArgs e)
        {
            AspNetPagerFinal.CurrentPageIndex = 1;
            BindFinalOrder();
        }

        protected void AspNetPagerFinal_PageChanged(object sender, EventArgs e)
        {
            BindFinalOrder();
        }

        /// <summary>
        /// 确定提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool canSubmit = true;
            string area = labTotalArea.Text;
            if (area == "0")
            {
                canSubmit = false;
                ExcuteJs("SubmintFail");
            }
            else
            {
                var orderList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId);
                if (orderList.Any())
                {
                    int emptyOrderCount = orderList.Where(s=>s.OrderType == 1 && (s.GraphicLength == null || s.GraphicLength == 0 || s.GraphicWidth == null || s.GraphicWidth == 0)).ToList().Count;
                    if (emptyOrderCount > 0)
                    {
                        int orderTotalCount = orderList.Count;
                        if ((decimal.Parse(emptyOrderCount.ToString()) / decimal.Parse(orderTotalCount.ToString()) * 100) >= 50)
                        {
                            canSubmit = false;
                            ExcuteJs("SubmintWarning");
                        }
                    }
                }
                
            }
            if (canSubmit)
            {
                SubmitOrder();
            }
        }



        void SubmitOrder()
        {
            SubjectBLL subjectBll = new SubjectBLL();
            Models.Subject model = subjectBll.GetModel(subjectId);
            if (model != null)
            {
                model.Status = 4;//提交完成
                model.ApproveState = 0;
                model.SubmitUserId = CurrentUser.UserId;
                subjectBll.Update(model);
                string url = "/Subjects/SubjectList.aspx";
                if (model.RegionOrderType == 2)
                    url = "/Subjects/RegionSubject/NotFinishList.aspx";
                Alert("提交成功！", url);
            }
            else
            {
                Alert("提交失败！");
            }
        }

        /// <summary>
        /// 重新拆单，返回上一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReSplit_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("SplitOrder.aspx?subjectId={0}",subjectId),false);
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("../SubjectList.aspx", false);
        }


        //void StatisticsInfo() 
        //{
        //    var orderList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false));
            
        //}

        protected void Button1_Click(object sender, EventArgs e)
        {
            SubmitOrder();
        }

        protected void repeater_FinalOrder_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                FinalOrderDetailTemp orderModel = e.Item.DataItem as FinalOrderDetailTemp;
                if (orderModel != null)
                {
                    Label labOrderType = (Label)e.Item.FindControl("labOrderType");
                    labOrderType.Text = CommonMethod.GeEnumName<OrderTypeEnum>((orderModel.OrderType??0).ToString());
                }
            }
        }

        
    }
}
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


namespace WebApp.Subjects.Received
{
    public partial class ReceiveConfirm : BasePage
    {
        public int guidanceId;
        public int shopId;
        public string FileType = string.Empty;
        public string FileCode = string.Empty;
        public string SendFileCode = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["shopId"] != null)
            {
                shopId = int.Parse(Request.QueryString["shopId"]);
            }
            FileType = FileTypeEnum.Image.ToString();
            FileCode = FileCodeEnum.ReceiveInvoice.ToString();
            SendFileCode = FileCodeEnum.SendInvoice.ToString();
            if (!IsPostBack)
            {
                BindShop();
                BindOrders();
                BindSendData();
            }
        }

        void BindShop()
        {
            Shop model = new ShopBLL().GetModel(shopId);
            if (model != null)
            {
                labShopNo.Text = model.ShopNo;
                labShopName.Text = model.ShopName;
                labRegion.Text = model.RegionName;
                labProvince.Text = model.ProvinceName;
                labCity.Text = model.CityName;
                labCityTier.Text = model.CityTier;
                labIsInstall.Text = model.IsInstall;
                labAddress.Text = model.POPAddress;
            }
        }

        void BindOrders()
        {
            List<FinalOrderDetailTemp> orderlist = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                                    join subject in CurrentContext.DbContext.Subject
                                                    on order.SubjectId equals subject.Id
                                                    where subject.GuidanceId == guidanceId && order.ShopId == shopId
                                                    select order).ToList();
            var materialList = (from material in CurrentContext.DbContext.OrderMaterial
                                join subject in CurrentContext.DbContext.Subject
                                on material.SubjectId equals subject.Id
                                where subject.GuidanceId == guidanceId && material.ShopId == shopId
                                select material).ToList();
            materialList.ForEach(s =>
            {
                FinalOrderDetailTemp model = new FinalOrderDetailTemp();
                model.OrderType = 3;
                model.PositionDescription = s.MaterialName;
                model.Quantity = s.MaterialCount;
                model.Remark = s.Remark;
                model.Sheet = s.Sheet;
                model.SubjectId = s.SubjectId;
                orderlist.Add(model);
            });
            rep_OrderList.DataSource = orderlist;
            rep_OrderList.DataBind();

        }


        void BindSendData()
        {
            var model = new SendBLL().GetList(s => s.GuidanceId == guidanceId && s.ShopId == shopId).FirstOrDefault();
            if (model != null)
            {
                if (model.SendDate != null)
                    labSendDate.Text = DateTime.Parse(model.SendDate.ToString()).ToShortDateString();
                labSendRemark.Text = model.SendRemark;
                labSendNumber.Text = model.SendInvoiceNumber;
                hfReceiveId.Value = model.Id.ToString();
                if (model.IsReceived == 1)
                {
                    if (model.ReceiveDate != null)
                        txtReceiveDate.Text = DateTime.Parse(model.ReceiveDate.ToString()).ToShortDateString();
                    txtRemark.Text = model.ReceiveRemark;
                }
            }
        }

        protected void rep_OrderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                FinalOrderDetailTemp model = (FinalOrderDetailTemp)e.Item.DataItem;
                if (model != null)
                {
                    int orderType = model.OrderType ?? 1;
                    Label lab = (Label)e.Item.FindControl("labOrderType");
                    switch (orderType)
                    {
                        case 1:
                            lab.Text = "POP";
                            break;
                        case 2:
                            lab.Text = "道具";
                            break;
                        case 3:
                            lab.Text = "物料";
                            break;
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string receiveDate = txtReceiveDate.Text.Trim();
            string remark = txtRemark.Text;
            int receiveId = 0;
            if (!string.IsNullOrWhiteSpace(hfReceiveId.Value))
                receiveId = int.Parse(hfReceiveId.Value);
            Send sendModel = new Send();
            SendBLL sendBll = new SendBLL();
            if (receiveId > 0)
                sendModel = sendBll.GetModel(receiveId);
            sendModel.ReceiveDate = DateTime.Parse(receiveDate);
            sendModel.ReceiveRemark = remark;

            if (receiveId > 0)
            {
                sendModel.IsReceived = 1;
                sendBll.Update(sendModel);
            }
            
            if (FileUpload1.HasFile)
            {
                HttpPostedFile file = FileUpload1.PostedFile;
                AttachmentBLL attachBll = new AttachmentBLL();
                Attachment attachModel;
                attachModel = new Attachment();
                attachModel.FileType = FileType;
                attachModel.SubjectId = guidanceId;
                attachModel.SecItemId = shopId;
                attachModel.FileCode = FileCode;
                OperateFile.UpFiles(file, ref attachModel);
                attachModel.AddDate = DateTime.Now;
                attachModel.AddUserId = new BasePage().CurrentUser.UserId;
                attachModel.IsDelete = false;
                attachBll.Add(attachModel);
            }
            Alert("提交成功", "SendList.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("SendList.aspx", false);
        }
    }
}
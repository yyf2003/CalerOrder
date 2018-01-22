using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;

namespace WebApp.Subjects.Received
{
    public partial class SendConfirm : BasePage
    {
        public int guidanceId;
        public int shopId;
        public string FileType = string.Empty;
        public string FileCode = string.Empty;

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
            FileCode = FileCodeEnum.SendInvoice.ToString();
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
            materialList.ForEach(s => {
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
            var model = new SendBLL().GetList(s=>s.GuidanceId==guidanceId && s.ShopId==shopId).FirstOrDefault();
            if (model != null)
            {
                if (model.SendDate!=null)
                  txtSendDate.Text = DateTime.Parse(model.SendDate.ToString()).ToShortDateString();
                txtRemark.Text = model.SendRemark;
                hfSendId.Value = model.Id.ToString();
                txtSendInvoiceNumber.Text = model.SendInvoiceNumber;
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
            string sendDate = txtSendDate.Text.Trim();
            string remark = txtRemark.Text;
            int sendId = 0;
            if (!string.IsNullOrWhiteSpace(hfSendId.Value))
                sendId = int.Parse(hfSendId.Value);
            Send sendModel = new Send();
            SendBLL sendBll = new SendBLL();
            if (sendId > 0)
                sendModel = sendBll.GetModel(sendId);
            sendModel.SendDate = DateTime.Parse(sendDate);
            sendModel.SendRemark = remark;
            sendModel.SendInvoiceNumber = txtSendInvoiceNumber.Text.Trim();
            if (sendId > 0)
            {
                sendBll.Update(sendModel);
            }
            else
            {
                sendModel.AddDate = DateTime.Now;
                sendModel.AddUserId = CurrentUser.UserId;
                sendModel.GuidanceId = guidanceId;
                sendModel.IsSend = 1;
                sendModel.ShopId = shopId;
                sendBll.Add(sendModel);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Models;
using DAL;
using System.Transactions;
using Common;

namespace WebApp.Customer
{
    public partial class OneShopMachineFrameList : BasePage
    {
        public int shopId;
        public string url = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            url = Request.FilePath;
            if (Request.QueryString["shopId"] != null)
            {
                shopId = int.Parse(Request.QueryString["shopId"]);
            }
            if (!IsPostBack)
            {
                //string promissionStr = GetPromissionStr();
                //if (!string.IsNullOrWhiteSpace(promissionStr))
                //{
                //    hfPromission.Value = promissionStr;
                //}
                Shop model = new ShopBLL().GetModel(shopId);
                if (model != null)
                {
                    hfShopNo.Value = model.ShopNo;
                }
                BindData();
            }

        }



        void BindData()
        {
            var list = (from frame in CurrentContext.DbContext.ShopMachineFrame
                        join shop in CurrentContext.DbContext.Shop
                        on frame.ShopId equals shop.Id
                        where frame.ShopId == shopId
                        select new
                        {
                            FrameId=frame.Id,
                            frame,
                            frame.ShopId,
                            frame.MachineFrame,
                            frame.Gender,
                            frame.IsValid,
                            shop
                        }).ToList();
            
            
            gv.DataSource = list.OrderBy(s => s.frame.PositionName).ToList();
            gv.DataBind();
            
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            List<int> frameList = new List<int>();
            foreach (GridViewRow grv in gv.Rows)
            {
                CheckBox cbOne = (CheckBox)grv.FindControl("cbOne");
                if (cbOne.Checked)
                {
                    int frameId = int.Parse(gv.DataKeys[grv.RowIndex].Value.ToString());
                    frameList.Add(frameId);
                }
            }
            if (frameList.Any())
            {
                bool isOk = false;
                using (TransactionScope tran = new TransactionScope()) {
                    try
                    {
                        BaseDataChangeLogBLL changeLogBll = new BaseDataChangeLogBLL();
                        ShopMachineFrameChangeDetailBLL changeDetailBll = new ShopMachineFrameChangeDetailBLL();
                        ShopMachineFrameChangeDetail changeDetailModel;
                        //添加变更日志
                        BaseDataChangeLog logModel = new BaseDataChangeLog();
                        logModel.AddDate = DateTime.Now;
                        logModel.AddUserId = CurrentUser.UserId;
                        logModel.ItemType = (int)BaseDataChangeItemEnum.ShopMachineFrame;
                        logModel.ChangeType = (int)DataChangeTypeEnum.Delete;
                        changeLogBll.Add(logModel);

                        ShopMachineFrameBLL frameBll=new ShopMachineFrameBLL();

                        var frames = frameBll.GetList(s => frameList.Contains(s.Id));
                        if (frames.Any())
                        {
                            frames.ForEach(s => {
                                changeDetailModel = new ShopMachineFrameChangeDetail();
                                changeDetailModel.CornerType = s.CornerType;
                                changeDetailModel.Count = s.Count;
                                changeDetailModel.Gender = s.Gender;
                                changeDetailModel.LevelNum = s.LevelNum;
                                changeDetailModel.LogId = logModel.Id;
                                changeDetailModel.MachineFrame = s.MachineFrame;
                                changeDetailModel.PositionName = s.PositionName;
                                changeDetailModel.Remark = "删除";
                                changeDetailModel.ShopId = s.ShopId;
                                changeDetailModel.AddDate = DateTime.Now;
                                changeDetailModel.AddUserId = CurrentUser.UserId;
                                changeDetailBll.Add(changeDetailModel);
                                frameBll.Delete(s);
                            });
                        }
                        tran.Complete();
                        isOk = true;
                    }
                    catch (Exception ex)
                    { 
                        
                    }
                }
                if (isOk)
                    BindData();
                else
                {
                    Alert("删除失败！");
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex != -1)
            {
                object item = e.Row.DataItem;
                if (item != null)
                {
                    object isValidObj = item.GetType().GetProperty("IsValid").GetValue(item, null);
                    bool isValid = isValidObj != null ? bool.Parse(isValidObj.ToString()) : true;
                    Label labIsValid = (Label)e.Row.FindControl("labIsValid");
                    labIsValid.Text = isValid ? "生产" : "不生产";
                }
            }
        }

        
    }
}
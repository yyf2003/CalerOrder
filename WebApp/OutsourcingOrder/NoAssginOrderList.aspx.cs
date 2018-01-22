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

namespace WebApp.OutsourcingOrder
{
    public partial class NoAssginOrderList : System.Web.UI.Page
    {
        int guidanceId;
        int shopId;
        string subjectIds = string.Empty;
        string sheet = string.Empty;
        int ruanmo = 0;
        string materialAssign = string.Empty;
        string materialPlan = string.Empty;
        string otherMaterial = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            List<int> subjectIdList = new List<int>();
            List<string> sheetList = new List<string>();
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["shopId"] != null)
            {
                shopId = int.Parse(Request.QueryString["shopId"]);
            }
            if (Request.QueryString["subjectIds"] != null)
            {
                subjectIds = Request.QueryString["subjectIds"];
                if (!string.IsNullOrWhiteSpace(subjectIds))
                {
                    subjectIdList = StringHelper.ToIntList(subjectIds, ',');
                }
            }
            if (Request.QueryString["sheet"] != null)
            {
                sheet = Request.QueryString["sheet"];
                if (!string.IsNullOrWhiteSpace(sheet))
                {
                    sheetList = StringHelper.ToStringList(sheet, ',');
                }
            }
            if (Request.QueryString["ruanmo"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ruanmo"]))
            {
                ruanmo = int.Parse(Request.QueryString["ruanmo"]);
            }
            if (Request.QueryString["materialAssign"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["materialAssign"]))
            {
                materialAssign = Request.QueryString["materialAssign"];
            }
            if (Request.QueryString["materialPlan"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["materialPlan"]))
            {
                materialPlan = Request.QueryString["materialPlan"];
            }
            if (Request.QueryString["otherMaterial"] != null)
            {
                otherMaterial = Request.QueryString["otherMaterial"];
            }
            //List<FinalOrderDetailTemp> list = new FinalOrderDetailTempBLL().GetList(s => s.GuidanceId == guidanceId && s.ShopId == shopId && (s.OrderType!=(int)OrderTypeEnum.物料)).OrderBy(s => s.Sheet).ThenBy(s => s.GraphicNo).ToList();
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        where order.GuidanceId == guidanceId
                        && order.ShopId == shopId
                        && order.OrderType != (int)OrderTypeEnum.物料
                        && ((order.OrderType == 1 && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType > 1)
                        && (order.IsValid == null || order.IsValid == true)
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                        && (subject.IsDelete == null || subject.IsDelete==false)
                        && subject.ApproveState==1
                        select new
                        {
                            order,
                            order.OrderType,
                            subject.SubjectName
                        }).OrderBy(s => s.OrderType).ThenBy(s => s.order.Sheet).ThenBy(s => s.order.GraphicNo).ToList();
            if (subjectIdList.Any())
            {
                list = list.Where(s => subjectIdList.Contains(s.order.SubjectId ?? 0)).ToList();
            }
            if (sheetList.Any())
            {
                list = list.Where(s => s.order.Sheet != null && sheetList.Contains(s.order.Sheet)).ToList();
            }
            if (ruanmo > 0)
            {
                if (ruanmo == 1)
                {
                    //只查询软膜的订单
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜")).ToList();
                   

                }
                else if (ruanmo == 2)
                {
                    list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();
                   
                }
            }
            if (!string.IsNullOrWhiteSpace(materialAssign))
            {
                string material0 = "背胶PP+";
                string material1 = "雪弗板";
                string material3 = "蝴蝶支架";
                if (materialAssign == "背胶")
                {
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains(material0) && s.order.GraphicMaterial.Contains(material1) && !s.order.GraphicMaterial.Contains(material3)).ToList();
                    if (!string.IsNullOrWhiteSpace(materialPlan))
                    {
                        list = list.Where(s => s.order.GraphicMaterial.Contains(materialPlan)).ToList();
                    }
                }
                else if (materialAssign == "非背胶")
                {
                    list = list.Where(s => (s.order.GraphicMaterial != null && !(s.order.GraphicMaterial.Contains(material0) && s.order.GraphicMaterial.Contains(material1))) || s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || s.order.GraphicMaterial.Contains(material3)).ToList();
                   
                }
                else
                {
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains(materialAssign)).ToList();
                   
                }
            }
            if (!string.IsNullOrWhiteSpace(otherMaterial))
            {
                list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.ToLower().Contains(otherMaterial.ToLower())).ToList();
                
            }
            if (!IsPostBack)
            {
                if (list.Any())
                {
                    labShopNo.Text = list[0].order.ShopNo;
                    labShopName.Text = list[0].order.ShopName;
                }
            }
            rp_orderList.DataSource = list;
            rp_orderList.DataBind();
        }

        protected void rp_orderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object model = e.Item.DataItem;
                if (model != null)
                {
                    object objOrderType = model.GetType().GetProperty("OrderType").GetValue(model, null);
                    string orderType = objOrderType != null ? objOrderType.ToString() : "";
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);
                }
            }
        }
    }
}
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
    public partial class FinishAssignOrderList : System.Web.UI.Page
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
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["shopId"] != null)
            {
                shopId = int.Parse(Request.QueryString["shopId"]);
            }
            if (!IsPostBack)
            {
                BindData();
            }
        }

        void BindData()
        {
            //List<OutsourceOrderDetail> list = new OutsourceOrderDetailBLL().GetList(s=>s.GuidanceId==guidanceId && s.ShopId==shopId).OrderBy(s=>s.Sheet).ThenBy(s=>s.GraphicNo).ToList();
            List<int> subjectIdList = new List<int>();
            List<string> sheetList = new List<string>();
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



            var list = (from order in CurrentContext.DbContext.OutsourceOrderDetail
                       join outsource in CurrentContext.DbContext.Company
                       on order.OutsourceId equals outsource.Id
                       where order.GuidanceId == guidanceId
                       && order.ShopId == shopId
                       select new {
                           order,
                           order.OrderType,
                           order.AssignType,
                           outsource.CompanyName
                       }).OrderBy(s=>s.OrderType).ToList();

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
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains("软膜") && s.order.SubjectId>0).ToList();


                }
                else if (ruanmo == 2)
                {
                    list = list.Where(s => s.order.GraphicMaterial == null || s.order.GraphicMaterial == "" || (s.order.GraphicMaterial != null && !s.order.GraphicMaterial.Contains("软膜"))).ToList();

                }
            }
            if (!string.IsNullOrWhiteSpace(materialAssign))
            {
                list = list.Where(s => s.order.SubjectId > 0).ToList();
                string material0 = "背胶PP+";
                string material1 = "雪弗板";
                string material3 = "蝴蝶支架";
                if (materialAssign == "背胶")
                {
                    list = list.Where(s => s.order.OrderGraphicMaterial != null && s.order.OrderGraphicMaterial.Contains(material0) && s.order.OrderGraphicMaterial.Contains(material1) && !s.order.OrderGraphicMaterial.Contains(material3)).ToList();
                    if (!string.IsNullOrWhiteSpace(materialPlan))
                    {
                        if (materialPlan == "雪弗板")
                        {
                            list = list.Where(s =>s.order.GraphicMaterial!=null && s.order.GraphicMaterial.ToLower() == "3mmpvc").ToList();
                        }
                        else
                            list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial == materialPlan).ToList();
                    }
                }
                else if (materialAssign == "非背胶")
                {
                    list = list.Where(s => (s.order.OrderGraphicMaterial != null && !(s.order.OrderGraphicMaterial.Contains(material0) && s.order.OrderGraphicMaterial.Contains(material1))) || s.order.OrderGraphicMaterial == null || s.order.OrderGraphicMaterial == "" || s.order.OrderGraphicMaterial.Contains(material3)).ToList();

                }
                else
                {
                    list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.Contains(materialAssign)).ToList();

                }
            }
            if (!string.IsNullOrWhiteSpace(otherMaterial))
            {

                list = list.Where(s => s.order.GraphicMaterial != null && s.order.GraphicMaterial.ToLower().Contains(otherMaterial.ToLower()) && s.order.SubjectId > 0).ToList();

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
                //OutsourceOrderDetail model = e.Item.DataItem as OutsourceOrderDetail;
                object model = e.Item.DataItem;
                if (model != null)
                {
                    object orderTypeObj = model.GetType().GetProperty("OrderType").GetValue(model, null);
                    string orderType = orderTypeObj != null ? orderTypeObj.ToString() : "1";
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);

                    object outsourceNameObj = model.GetType().GetProperty("CompanyName").GetValue(model, null);
                    if (outsourceNameObj!=null)
                    {
                        string outsourceName = outsourceNameObj.ToString();
                        object assignTypeObj = model.GetType().GetProperty("AssignType").GetValue(model, null);
                        string assignType = assignTypeObj != null ? assignTypeObj.ToString() : "1";
                        assignType = CommonMethod.GetEnumDescription<OutsourceOrderTypeEnum>(assignType);
                        if (!string.IsNullOrWhiteSpace(assignType))
                        {
                            outsourceName += "(" + assignType + ")";
                        }
                        ((Label)e.Item.FindControl("labOutsoure")).Text = outsourceName;
                    }

                }
            }
        }
    }
}
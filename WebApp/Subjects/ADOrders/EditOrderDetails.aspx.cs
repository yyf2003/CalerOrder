using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Web.UI.HtmlControls;
using System.Transactions;

namespace WebApp.Subjects.ADOrders
{
    public partial class EditOrderDetails : BasePage
    {
        int subjectId;
        ListOrderDetailBLL listBll = new ListOrderDetailBLL();
        POPOrderDetailBLL popBll = new POPOrderDetailBLL();
        SupplementOrderDetailBLL supplementBll = new SupplementOrderDetailBLL();
        MergeOriginalOrderBLL mergeBll = new MergeOriginalOrderBLL();
        //Dictionary<int, string> levelDic = new Dictionary<int, string>();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            //levelDic.Add(1, "第一陈列桌");
            //levelDic.Add(2, "第二陈列桌");
            //levelDic.Add(3, "第三陈列桌");
            //levelDic.Add(4, "第四陈列桌");
            //levelDic.Add(5, "第五陈列桌");
            if (!IsPostBack)
            {
                
                
                BindProject();
                BindList();
                BindPOP();
                BindSupplement();
                BindMerge();
                BindFinalOrder();
                if (rblEditType.SelectedValue == "1")
                {
                    Panel1.Visible = true;
                    Panel2.Visible = false;
                }
                else
                {
                    Panel1.Visible = false;
                    Panel2.Visible = true;
                }
            }
        }

        void BindProject()
        {
            var model = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        where subject.Id == subjectId
                        select new { 
                           subject,
                           customer.CustomerName
                        }).FirstOrDefault();
            if (model != null)
            {
                labCustomer.Text = model.CustomerName;
                labSubjectNo.Text = model.subject.SubjectNo;
                labSubjectName.Text = model.subject.SubjectName;
            }
        }


        void BindList()
        {

            var listOrder = (from list in CurrentContext.DbContext.ListOrderDetail
                             join shop in CurrentContext.DbContext.Shop
                             on list.ShopId equals shop.Id

                             where list.SubjectId == subjectId
                             select new
                             {
                                 listIsDelete = list.IsDelete,
                                 Sheet=list.Sheet,
                                 LevelNum=list.LevelNum??0,
                                 list,
                                 shop
                             }).OrderBy(s => s.shop.CityTier).ToList();
            if (cblListRegion.Items.Count == 0)
            {
                List<string> regionList = new List<string>();
                List<string> sheetList = new List<string>();
                List<string> cityCiterList = new List<string>();
                listOrder.ForEach(s =>
                {
                    if (!regionList.Contains(s.shop.RegionName))
                    {
                        regionList.Add(s.shop.RegionName);
                        cblListRegion.Items.Add(new ListItem(s.shop.RegionName + " ", s.shop.RegionName));
                    }
                    if (!sheetList.Contains(s.list.Sheet))
                    {
                        sheetList.Add(s.list.Sheet);
                        cblListSheet.Items.Add(new ListItem(s.list.Sheet + " ", s.list.Sheet));
                    }
                    if (!cityCiterList.Contains(s.shop.CityTier))
                    {
                        cityCiterList.Add(s.shop.CityTier);
                        cblListCityTier.Items.Add(new ListItem(s.shop.CityTier + " ", s.shop.CityTier));
                    }
                });
            }
            if (!string.IsNullOrWhiteSpace(txtListShopNo.Text))
            {
                listOrder = listOrder.Where(s => s.shop.ShopNo.ToLower() == txtListShopNo.Text.Trim().ToLower()).ToList();
            }

            List<string> regionList1 = new List<string>();
            List<string> sheetList1 = new List<string>();
            List<string> cityCiterList1 = new List<string>();
            for (int i = 0; i < cblListRegion.Items.Count; i++)
            {
                if (cblListRegion.Items[i].Selected)
                {
                    regionList1.Add(cblListRegion.Items[i].Value);
                }
            }
            for (int i = 0; i < cblListSheet.Items.Count; i++)
            {
                if (cblListSheet.Items[i].Selected)
                {
                    sheetList1.Add(cblListSheet.Items[i].Value);
                }
            }
            for (int i = 0; i < cblListCityTier.Items.Count; i++)
            {
                if (cblListCityTier.Items[i].Selected)
                {
                    cityCiterList1.Add(cblListCityTier.Items[i].Value);
                }
            }
            if (regionList1.Any())
            {
                listOrder = listOrder.Where(s => regionList1.Contains(s.shop.RegionName)).ToList();
            }
            if (sheetList1.Any())
            {
                listOrder = listOrder.Where(s => sheetList1.Contains(s.list.Sheet)).ToList();
            }
            if (cityCiterList1.Any())
            {
                listOrder = listOrder.Where(s => cityCiterList1.Contains(s.shop.CityTier)).ToList();
            }
            AspNetPager1.RecordCount = listOrder.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvList.DataSource = listOrder.OrderByDescending(s => s.list.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();

            gvList.DataBind();

        }



        void BindPOP()
        {
            var popOrder = (from order in CurrentContext.DbContext.POPOrderDetail
                            join pop in CurrentContext.DbContext.POP
                            on new { order.ShopId, order.GraphicNo } equals new { pop.ShopId, pop.GraphicNo }
                            join shop in CurrentContext.DbContext.Shop
                            on order.ShopId equals shop.Id

                            where order.SubjectId == subjectId
                            select new
                            {
                                order,
                                //Region = region != null ? region.RegionName : "",
                                // Province = province != null ? province.PlaceName : "",
                                //City = city != null ? city.PlaceName : "",
                                pop,
                                shop,
                                popIsDelete = order.IsDelete,

                            }).OrderBy(s => s.shop.CityTier).ToList();
            if (cblPOPRegion.Items.Count == 0)
            {
                List<string> regionList = new List<string>();
                List<string> sheetList = new List<string>();
                List<string> cityCiterList = new List<string>();
                popOrder.ForEach(s =>
                {
                    if (!regionList.Contains(s.shop.RegionName))
                    {
                        regionList.Add(s.shop.RegionName);
                        cblPOPRegion.Items.Add(new ListItem(s.shop.RegionName + " ", s.shop.RegionName));
                    }
                    if (!sheetList.Contains(s.order.Sheet))
                    {
                        sheetList.Add(s.order.Sheet);
                        cblPOPSheet.Items.Add(new ListItem(s.order.Sheet + " ", s.order.Sheet));
                    }
                    if (!cityCiterList.Contains(s.shop.CityTier))
                    {
                        cityCiterList.Add(s.shop.CityTier);
                        cblPOPCityTier.Items.Add(new ListItem(s.shop.CityTier + " ", s.shop.CityTier));
                    }
                });
            }
            string shopNo = txtPOPShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                popOrder = popOrder.Where(s => s.shop.ShopNo == shopNo).ToList();
            }
            List<string> regionList1 = new List<string>();
            List<string> sheetList1 = new List<string>();
            List<string> cityCiterList1 = new List<string>();
            for (int i = 0; i < cblPOPRegion.Items.Count; i++)
            {
                if (cblPOPRegion.Items[i].Selected)
                {
                    regionList1.Add(cblPOPRegion.Items[i].Value);
                }
            }
            for (int i = 0; i < cblPOPSheet.Items.Count; i++)
            {
                if (cblPOPSheet.Items[i].Selected)
                {
                    sheetList1.Add(cblPOPSheet.Items[i].Value);
                }
            }
            for (int i = 0; i < cblPOPCityTier.Items.Count; i++)
            {
                if (cblPOPCityTier.Items[i].Selected)
                {
                    cityCiterList1.Add(cblPOPCityTier.Items[i].Value);
                }
            }
            if (regionList1.Any())
            {
                popOrder = popOrder.Where(s => regionList1.Contains(s.shop.RegionName)).ToList();
            }
            if (sheetList1.Any())
            {
                popOrder = popOrder.Where(s => sheetList1.Contains(s.pop.Sheet)).ToList();
            }
            if (cityCiterList1.Any())
            {
                popOrder = popOrder.Where(s => cityCiterList1.Contains(s.shop.CityTier)).ToList();
            }
            AspNetPager2.RecordCount = popOrder.Count;
            this.AspNetPager2.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager2.CurrentPageIndex, this.AspNetPager2.PageCount, this.AspNetPager2.RecordCount, this.AspNetPager2.PageSize });
            gvPOP.DataSource = popOrder.OrderByDescending(s => s.order.Id).Skip((AspNetPager2.CurrentPageIndex - 1) * AspNetPager2.PageSize).Take(AspNetPager2.PageSize).ToList();

            gvPOP.DataBind();

        }

        void BindSupplement()
        {


            //补充订单
            var supplementOrder = (from list in CurrentContext.DbContext.SupplementOrderDetail
                                   join shop in CurrentContext.DbContext.Shop
                                   on list.ShopId equals shop.Id

                                   where list.SubjectId == subjectId
                                   select new
                                   {
                                       list,
                                       shop
                                   }).OrderBy(s => s.shop.CityTier).ToList();

            if (cblBCRegion.Items.Count == 0)
            {
                List<string> regionList = new List<string>();
                List<string> sheetList = new List<string>();
                List<string> cityCiterList = new List<string>();
                supplementOrder.ForEach(s =>
                {
                    if (!regionList.Contains(s.shop.RegionName))
                    {
                        regionList.Add(s.shop.RegionName);
                        cblBCRegion.Items.Add(new ListItem(s.shop.RegionName + " ", s.shop.RegionName));
                    }
                    if (!sheetList.Contains(s.list.Sheet))
                    {
                        sheetList.Add(s.list.Sheet);
                        cblBCSheet.Items.Add(new ListItem(s.list.Sheet + " ", s.list.Sheet));
                    }
                    if (!cityCiterList.Contains(s.shop.CityTier))
                    {
                        cityCiterList.Add(s.shop.CityTier);
                        cblBCCityTier.Items.Add(new ListItem(s.shop.CityTier + " ", s.shop.CityTier));
                    }
                });
            }
            string shopNo = txtBCShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                supplementOrder = supplementOrder.Where(s => s.shop.ShopNo == shopNo).ToList();
            }
            List<string> regionList1 = new List<string>();
            List<string> sheetList1 = new List<string>();
            List<string> cityCiterList1 = new List<string>();
            for (int i = 0; i < cblBCRegion.Items.Count; i++)
            {
                if (cblBCRegion.Items[i].Selected)
                {
                    regionList1.Add(cblBCRegion.Items[i].Value);
                }
            }
            for (int i = 0; i < cblBCSheet.Items.Count; i++)
            {
                if (cblBCSheet.Items[i].Selected)
                {
                    sheetList1.Add(cblBCSheet.Items[i].Value);
                }
            }
            for (int i = 0; i < cblBCCityTier.Items.Count; i++)
            {
                if (cblBCCityTier.Items[i].Selected)
                {
                    cityCiterList1.Add(cblBCCityTier.Items[i].Value);
                }
            }
            if (regionList1.Any())
            {
                supplementOrder = supplementOrder.Where(s => regionList1.Contains(s.shop.RegionName)).ToList();
            }
            if (sheetList1.Any())
            {
                supplementOrder = supplementOrder.Where(s => sheetList1.Contains(s.list.Sheet)).ToList();
            }
            if (cityCiterList1.Any())
            {
                supplementOrder = supplementOrder.Where(s => cityCiterList1.Contains(s.shop.CityTier)).ToList();
            }
            AspNetPager3.RecordCount = supplementOrder.Count;
            this.AspNetPager3.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager3.CurrentPageIndex, this.AspNetPager3.PageCount, this.AspNetPager3.RecordCount, this.AspNetPager3.PageSize });
            gvSupplement.DataSource = supplementOrder.OrderByDescending(s => s.list.Id).Skip((AspNetPager3.CurrentPageIndex - 1) * AspNetPager3.PageSize).Take(AspNetPager3.PageSize).ToList();

            gvSupplement.DataBind();

        }

        void BindMerge()
        {
            var MergeList = (from merge in CurrentContext.DbContext.MergeOriginalOrder
                             join pop1 in CurrentContext.DbContext.POP
                             on new { merge.GraphicNo, merge.ShopId } equals new { pop1.GraphicNo, pop1.ShopId } into popTemp
                             join shop in CurrentContext.DbContext.Shop
                             on merge.ShopId equals shop.Id
                             from pop in popTemp.DefaultIfEmpty()
                             where merge.SubjectId == subjectId
                             select new
                             {
                                 merge,
                                 shop,
                                 GraphicNo = pop != null ? pop.GraphicNo : "",
                                 POPName = pop != null ? pop.POPName : "",
                                 POPType = pop != null ? pop.POPType : "",
                                 PositionDescription = pop != null ? pop.PositionDescription : "",
                                 WindowWide = pop != null && pop.WindowWide != null ? pop.WindowWide.ToString() : "",
                                 WindowHigh = pop != null && pop.WindowHigh != null ? pop.WindowHigh.ToString() : "",
                                 WindowDeep = pop != null && pop.WindowDeep != null ? pop.WindowDeep.ToString() : "",
                                 WindowSize = pop != null ? pop.WindowSize : "",
                                 GraphicWidth = pop != null && pop.GraphicWidth != null ? pop.GraphicWidth.ToString() : "",
                                 GraphicLength = pop != null && pop.GraphicLength != null ? pop.GraphicLength.ToString() : "",
                                 Area = pop != null && pop.Area != null ? pop.Area.ToString() : "",
                                 GraphicMaterial = pop != null ? pop.GraphicMaterial : "",
                                 Style = pop != null ? pop.Style : "",
                                 CornerType = pop != null ? pop.CornerType : merge.CornerType,
                                 Category = pop != null ? pop.Category : "",
                                 StandardDimension = pop != null ? pop.StandardDimension : "",
                                 Modula = pop != null ? pop.Modula : "",
                                 Frame = pop != null ? pop.Frame : "",
                                 DoubleFace = pop != null ? pop.DoubleFace : "",
                                 Glass = pop != null ? pop.Glass : "",
                                 Backdrop = pop != null ? pop.Backdrop : "",
                                 ModulaQuantityWidth = pop != null && pop.ModulaQuantityWidth != null ? pop.ModulaQuantityWidth.ToString() : "",
                                 ModulaQuantityHeight = pop != null && pop.ModulaQuantityHeight != null ? pop.ModulaQuantityHeight.ToString() : "",
                                 PlatformLength = pop != null && pop.PlatformLength != null ? pop.PlatformLength.ToString() : "",
                                 PlatformWidth = pop != null && pop.PlatformWidth != null ? pop.PlatformWidth.ToString() : "",
                                 PlatformHeight = pop != null && pop.PlatformHeight != null ? pop.PlatformHeight.ToString() : "",
                                 FixtureType = pop != null ? pop.FixtureType : "",
                                 //Gender = pop != null ? pop.Gender : "",
                                 Sheet = merge.Sheet,
                                 LevelNum = merge.LevelNum ?? 0

                             }).OrderBy(s => s.shop.CityTier).ToList();
            if (cblMergeRegion.Items.Count == 0)
            {
                List<string> regionList = new List<string>();
                List<string> sheetList = new List<string>();
                List<string> cityCiterList = new List<string>();
                MergeList.ForEach(s =>
                {
                    if (!regionList.Contains(s.shop.RegionName))
                    {
                        regionList.Add(s.shop.RegionName);
                        cblMergeRegion.Items.Add(new ListItem(s.shop.RegionName + " ", s.shop.RegionName));
                    }
                    if (!sheetList.Contains(s.merge.Sheet))
                    {
                        sheetList.Add(s.merge.Sheet);
                        cblMergeSheet.Items.Add(new ListItem(s.merge.Sheet + " ", s.merge.Sheet));
                    }
                    if (!cityCiterList.Contains(s.shop.CityTier))
                    {
                        cityCiterList.Add(s.shop.CityTier);
                        cblMergeCityTier.Items.Add(new ListItem(s.shop.CityTier + " ", s.shop.CityTier));
                    }
                });
            }
            string shopNo = txtMergeShopNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                MergeList = MergeList.Where(s => s.shop.ShopNo == shopNo).ToList();
            }
            List<string> regionList1 = new List<string>();
            List<string> sheetList1 = new List<string>();
            List<string> cityCiterList1 = new List<string>();
            for (int i = 0; i < cblMergeRegion.Items.Count; i++)
            {
                if (cblMergeRegion.Items[i].Selected)
                {
                    regionList1.Add(cblMergeRegion.Items[i].Value);
                }
            }
            for (int i = 0; i < cblMergeSheet.Items.Count; i++)
            {
                if (cblMergeSheet.Items[i].Selected)
                {
                    sheetList1.Add(cblMergeSheet.Items[i].Value);
                }
            }
            for (int i = 0; i < cblMergeCityTier.Items.Count; i++)
            {
                if (cblMergeCityTier.Items[i].Selected)
                {
                    cityCiterList1.Add(cblMergeCityTier.Items[i].Value);
                }
            }
            if (regionList1.Any())
            {
                MergeList = MergeList.Where(s => regionList1.Contains(s.shop.RegionName)).ToList();
            }
            if (sheetList1.Any())
            {
                MergeList = MergeList.Where(s => sheetList1.Contains(s.merge.Sheet)).ToList();
            }
            if (cityCiterList1.Any())
            {
                MergeList = MergeList.Where(s => cityCiterList1.Contains(s.shop.CityTier)).ToList();
            }

            AspNetPager4.RecordCount = MergeList.Count;
            this.AspNetPager4.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager4.CurrentPageIndex, this.AspNetPager4.PageCount, this.AspNetPager4.RecordCount, this.AspNetPager4.PageSize });
            gvMerge.DataSource = MergeList.OrderBy(s => s.merge.Id).Skip((AspNetPager4.CurrentPageIndex - 1) * AspNetPager4.PageSize).Take(AspNetPager4.PageSize).ToList();

            gvMerge.DataBind();

        }

        /// <summary>
        /// list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindList();
        }

        /// <summary>
        /// 新增list订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddList_Click(object sender, EventArgs e)
        {
            string POSCode = txtAddListPOSCode.Text.Trim();
            int shopid = GetShopId(POSCode);
            if (shopid == 0)
            {

                System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "alert", "<script>alert('店铺编号不存在')</script>", false);
            }
            else
            {
                string MaterialSupport = txtAddListMaterialSupport.Text.Trim();
                string POSScale = txtAddListPOSScale.Text.Trim();
                string Sheet = txtAddListSheet.Text.Trim();
                string LevelNum = ddlAddListLevelNum.SelectedValue;
                string CornerType = txtAddListCornerType.Text.Trim();
                string Gender = txtAddListGender.Text.Trim();
                string Quantity = txtAddListQuantity.Text.Trim();
                string ChooseImg = txtAddListChooseImg.Text.Trim();
                string Remarks = txtAddListRemarks.Text.Trim();

                var list1 = listBll.GetList(s => s.ShopId == shopid && s.SubjectId == subjectId && s.Sheet == Sheet && s.Gender == Gender && (s.IsDelete == null || s.IsDelete == false));
                if (list1.Any())
                {
                    //Alert("该POP订单已存在！");
                    System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "alert", "<script>alert('订单已存在')</script>", false);
                }
                else
                {
                    if (Sheet.Contains("桌") && !CheckFrameNumber(shopid, Gender, int.Parse(LevelNum == "0" ? "1" : LevelNum)))
                    {
                        System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "alert", "<script>alert('陈列桌数量不足！')</script>", false);
                    }
                    else
                    {
                        ListOrderDetail listModel = new ListOrderDetail();
                        listModel.AddDate = DateTime.Now;
                        listModel.AddUserId = CurrentUser.UserId;
                        listModel.ChooseImg = ChooseImg;
                        listModel.CornerType = CornerType;
                        listModel.Gender = Gender;
                        listModel.MaterialSupport = MaterialSupport;
                        listModel.POSScale = POSScale;
                        listModel.Quantity = !string.IsNullOrWhiteSpace(Quantity) ? int.Parse(Quantity) : 1;
                        listModel.Remark = Remarks;
                        listModel.Sheet = Sheet;
                        if (LevelNum != "0")
                            listModel.LevelNum = int.Parse(LevelNum);
                        listModel.SubjectId = subjectId;
                        listModel.ShopId = shopid;
                        listBll.Add(listModel);
                        BindList();
                    }
                }
            }
        }


        ShopBLL shopBll = new ShopBLL();
        int GetShopId(string posCode)
        {
            Shop model = shopBll.GetList(s => s.ShopNo == posCode).FirstOrDefault();
            if (model != null)
            {
                return model.Id;
            }
            else
                return 0;
        }

        POPBLL popBll1 = new POPBLL();
        bool CheckPOP(int shopid, string sheet, string graphicCode)
        {
            var list = popBll1.GetList(s => s.ShopId == shopid && s.Sheet == sheet && s.GraphicNo.ToLower() == graphicCode.ToLower());
            return list.Any();
        }

        /// <summary>
        /// 删除list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteList_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvList.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbListOne");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfListId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                idList.ForEach(id =>
                {
                    ListOrderDetail model = listBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = true;
                        listBll.Update(model);
                    }
                });
                BindList();
            }
        }

        protected void gvList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {

                    //object objSheet = item.GetType().GetProperty("Sheet").GetValue(item, null);
                    object objLevelNum = item.GetType().GetProperty("LevelNum").GetValue(item, null);
                    //string sheet = objSheet != null ? objSheet.ToString() : "";
                    int levelNum = objLevelNum != null ? int.Parse(objLevelNum.ToString()) : 0;
                    DropDownList ddlLevelNum = (DropDownList)e.Item.FindControl("ddlListLevelNum");
                    ddlLevelNum.SelectedValue = levelNum.ToString();
                    
                    object objIsDelete = item.GetType().GetProperty("listIsDelete").GetValue(item, null);
                    bool isDelete = objIsDelete != null ? bool.Parse(objIsDelete.ToString()) : false;
                    if (isDelete)
                    {
                        foreach (Control con in e.Item.Controls)
                        {
                            if (con.GetType().Name == "TextBox")
                            {
                                ((TextBox)con).Enabled = false;
                            }
                            if (con.GetType().Name == "DropDownList")
                            {
                                ((DropDownList)con).Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        protected void brnReconverList_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvList.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbListOne");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfListId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                idList.ForEach(id =>
                {
                    ListOrderDetail model = listBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = false;
                        listBll.Update(model);
                    }
                });
                BindList();
            }
        }

        /// <summary>
        /// 提交list修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveListEdit_Click(object sender, EventArgs e)
        {
            ChangeBorderColor(gvList);
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    #region
                    int failCount = 0;
                    System.Text.StringBuilder errMsg = new System.Text.StringBuilder();
                    foreach (RepeaterItem item in gvList.Items)
                    {
                        if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                        {
                            CheckBox cb = (CheckBox)item.FindControl("cbListOne");
                            if (cb.Checked)
                            {
                                bool canSave = true;
                                HiddenField hfId = (HiddenField)item.FindControl("hfListId");
                                int id = int.Parse(hfId.Value);
                                TextBox txtListMaterialSupport = (TextBox)item.FindControl("txtListMaterialSupport");
                                TextBox txtListPOSScale = (TextBox)item.FindControl("txtListPOSScale");
                                TextBox txtListCornerType = (TextBox)item.FindControl("txtListCornerType");
                                TextBox txtListSheet = (TextBox)item.FindControl("txtListSheet");
                                DropDownList ddlLevelNum = (DropDownList)item.FindControl("ddlListLevelNum");
                                TextBox txtListGender = (TextBox)item.FindControl("txtListGender");
                                TextBox txtListQuantity = (TextBox)item.FindControl("txtListQuantity");
                                TextBox txtListChooseImg = (TextBox)item.FindControl("txtListChooseImg");
                                TextBox txtListRemark = (TextBox)item.FindControl("txtListRemark");

                                string MaterialSupport = txtListMaterialSupport.Text.Trim();
                                string POSScale = txtListPOSScale.Text.Trim();
                                string CornerType = txtListCornerType.Text.Trim();
                                string Sheet = txtListSheet.Text.Trim();
                                string LevelNum = ddlLevelNum.SelectedValue;
                                string Gender = txtListGender.Text.Trim();
                                string Quantity = txtListQuantity.Text.Trim();
                                string ChooseImg = txtListChooseImg.Text.Trim();
                                string Remark = txtListRemark.Text.Trim();
                                if (string.IsNullOrWhiteSpace(Sheet))
                                {
                                    //System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "alert", "<script>alert('请填写位置')</script>", false);
                                    //txtListSheet.BorderColor = System.Drawing.Color.Red;
                                    txtListSheet.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                
                                if (string.IsNullOrWhiteSpace(Gender))
                                {
                                    //txtListGender.BorderColor = System.Drawing.Color.Red;
                                    txtListGender.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                if (string.IsNullOrWhiteSpace(Quantity))
                                {
                                    txtListQuantity.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                else
                                {
                                    int num = 0;
                                    if (!int.TryParse(Quantity, out num))
                                    {
                                        txtListQuantity.Style.Add("border", "1px solid red");
                                        canSave = false;
                                        failCount++;
                                    }
                                }
                                
                                if (canSave)
                                {
                                    ListOrderDetail model = listBll.GetModel(id);
                                    if (model != null)
                                    {
                                        int LevelNum1=int.Parse(LevelNum == "0" ? "1" : LevelNum);
                                        //if (Sheet.Contains("桌") && !CheckFrameNumber(model.ShopId ?? 0, Gender, LevelNum1))
                                        //{
                                        //    failCount++;
                                        //    Shop shopModel = shopBll.GetModel(model.ShopId??0);
                                        //    string shopno = string.Empty;
                                        //    if (shopModel != null)
                                        //        shopno = shopModel.ShopNo;
                                        //    errMsg.AppendFormat("{0}陈列桌数量不足；", shopno);
                                        //}
                                        //else
                                        //{
                                            
                                        //}
                                        model.ChooseImg = ChooseImg;
                                        model.CornerType = CornerType;
                                        model.Gender = Gender;
                                        model.MaterialSupport = MaterialSupport;
                                        model.POSScale = POSScale;
                                        model.Quantity = int.Parse(Quantity);
                                        model.Remark = Remark;
                                        model.Sheet = Sheet;
                                        model.LevelNum = LevelNum1;
                                        listBll.Update(model);
                                    }
                                }
                            }
                        }
                    }
                    if (failCount == 0)
                    {
                        tran.Complete();
                        string js = "$('#gvList').find('input[name$=cbListOne]').each(function(){this.checked=false;})";
                        System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "alert", "<script>alert('提交成功');" + js + "</script>", false);
                        BindList();
                    }
                    else
                    {
                        if (errMsg.Length > 0)
                        {

                            System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "alert", "<script>alert('" + errMsg.ToString() + "')</script>", false);
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel1, GetType(), "alert", "<script>alert('提交失败：" + ex.Message + "')</script>", false);
                }
            }


        }

        void ChangeBorderColor(Repeater ritem)
        {
            foreach (RepeaterItem item in ritem.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    foreach (Control con in item.Controls)
                    {
                        if (con.GetType().Name == "TextBox")
                        {
                            ((TextBox)con).Style.Add("border", "");
                        }
                    }
                }
            }

        }

        protected void btnSreachList_Click(object sender, EventArgs e)
        {
            BindList();
        }

        protected void AspNetPager2_PageChanged(object sender, EventArgs e)
        {
            BindPOP();
        }

        protected void btnSreachPOP_Click(object sender, EventArgs e)
        {
            AspNetPager2.CurrentPageIndex = 1;
            BindPOP();
        }

        protected void btnDeletePOP_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvPOP.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbPOPOne");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfPOPId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                idList.ForEach(id =>
                {
                    POPOrderDetail model = popBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = true;
                        popBll.Update(model);
                    }
                });
                BindPOP();
            }
        }

        protected void btnReconverPOP_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvPOP.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbPOPOne");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfPOPId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                idList.ForEach(id =>
                {
                    POPOrderDetail model = popBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = false;
                        popBll.Update(model);
                    }
                });
                BindPOP();
            }
        }

        /// <summary>
        /// 提交POP修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSavePOPEdit_Click(object sender, EventArgs e)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    int failCount = 0;
                    foreach (RepeaterItem item in gvPOP.Items)
                    {
                        if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                        {
                            CheckBox cb = (CheckBox)item.FindControl("cbPOPOne");
                            if (cb.Checked)
                            {
                                bool canSave = true;
                                HiddenField hfId = (HiddenField)item.FindControl("hfPOPId");
                                int id = int.Parse(hfId.Value);
                                HiddenField hfShopNo = (HiddenField)item.FindControl("hfShopNo");
                                string shopNo = hfShopNo.Value;
                                int shopid = GetShopId(shopNo);
                                TextBox txtPOPMaterialSupport = (TextBox)item.FindControl("txtPOPMaterialSupport");
                                TextBox txtPOPPOSScale = (TextBox)item.FindControl("txtPOPPOSScale");
                                TextBox txtPOPGraphicNo = (TextBox)item.FindControl("txtPOPGraphicNo");
                                TextBox txtPOPSheet = (TextBox)item.FindControl("txtPOPSheet");
                                TextBox txtPOPGender = (TextBox)item.FindControl("txtPOPGender");
                                TextBox txtPOPQuantity = (TextBox)item.FindControl("txtPOPQuantity");
                                TextBox txtPOPChooseImg = (TextBox)item.FindControl("txtPOPChooseImg");
                                TextBox txtPOPRemark = (TextBox)item.FindControl("txtPOPRemark");

                                string MaterialSupport = txtPOPMaterialSupport.Text.Trim();
                                string POSScale = txtPOPPOSScale.Text.Trim();
                                string GraphicNo = txtPOPGraphicNo.Text.Trim();
                                string Sheet = txtPOPSheet.Text.Trim();
                                string Gender = txtPOPGender.Text.Trim();
                                string Quantity = txtPOPQuantity.Text.Trim();
                                string ChooseImg = txtPOPChooseImg.Text.Trim();
                                string Remark = txtPOPRemark.Text.Trim();

                                if (string.IsNullOrWhiteSpace(GraphicNo))
                                {

                                    txtPOPGraphicNo.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                if (string.IsNullOrWhiteSpace(Sheet))
                                {

                                    txtPOPSheet.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                if (string.IsNullOrWhiteSpace(Gender))
                                {

                                    txtPOPGender.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                if (string.IsNullOrWhiteSpace(Quantity))
                                {
                                    txtPOPQuantity.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                else
                                {
                                    int num = 0;
                                    if (!int.TryParse(Quantity, out num))
                                    {
                                        txtPOPQuantity.Style.Add("border", "1px solid red");
                                        canSave = false;
                                        failCount++;
                                    }
                                }
                                if (canSave)
                                {
                                    if (!CheckPOP(shopid, Sheet, GraphicNo))
                                    {
                                        System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel2, GetType(), "alert", "<script>alert('该店铺不含有该POP')</script>", false);
                                    }
                                    else
                                    {
                                        POPOrderDetail model = popBll.GetModel(id);
                                        if (model != null)
                                        {
                                            model.ChooseImg = ChooseImg;
                                            model.GraphicNo = GraphicNo;
                                            model.Gender = Gender;
                                            model.MaterialSupport = MaterialSupport;
                                            model.POSScale = POSScale;
                                            model.Quantity = int.Parse(Quantity);
                                            model.Remark = Remark;
                                            model.Sheet = Sheet;
                                            popBll.Update(model);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (failCount == 0)
                    {
                        tran.Complete();
                        string js = "$('#gvPOP').find('input[name$=cbPOPOne]').each(function(){this.checked=false;})";
                        System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel2, GetType(), "alert", "<script>alert('提交成功');" + js + "</script>", false);
                        BindPOP();
                    }
                }
                catch (Exception ex)
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel2, GetType(), "alert", "<script>alert('提交失败：" + ex.Message + "')</script>", false);
                }
            }
        }

        protected void btnAddPOP_Click(object sender, EventArgs e)
        {
            string POSCode = txtAddPOPPOSCode.Text.Trim();
            int shopid = GetShopId(POSCode);
            string Sheet = txtAddPOPSheet.Text.Trim();
            string GraphicNo = txtAddPOPGraphicNo.Text.Trim();
            if (shopid == 0)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel2, GetType(), "alert", "<script>alert('店铺编号不存在')</script>", false);
            }
            else if (!CheckPOP(shopid, Sheet, GraphicNo))
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel2, GetType(), "alert", "<script>alert('该店铺不含有该POP')</script>", false);
            }
            else
            {
                string MaterialSupport = txtAddPOPMaterialSupport.Text.Trim();
                string POSScale = txtAddPOPPOSScale.Text.Trim();

                string Gender = txtAddPOPGender.Text.Trim();
                string Quantity = txtAddPOPQuantity.Text.Trim();
                string ChooseImg = txtAddPOPChooseImg.Text.Trim();
                string Remarks = txtAddPOPRemarks.Text.Trim();

                var list1 = popBll.GetList(s => s.ShopId == shopid && s.SubjectId == subjectId && s.Sheet == Sheet && s.Gender == Gender && s.GraphicNo == GraphicNo && (s.IsDelete == null || s.IsDelete == false));
                if (list1.Any())
                {
                    //Alert("该POP订单已存在！");
                    System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel2, GetType(), "alert", "<script>alert('订单已存在')</script>", false);
                }
                else
                {

                    POPOrderDetail listModel = new POPOrderDetail();
                    listModel.AddDate = DateTime.Now;
                    listModel.AddUserId = CurrentUser.UserId;
                    listModel.ChooseImg = ChooseImg;
                    listModel.GraphicNo = GraphicNo;
                    listModel.Gender = Gender;
                    listModel.MaterialSupport = MaterialSupport;
                    listModel.POSScale = POSScale;
                    listModel.Quantity = !string.IsNullOrWhiteSpace(Quantity) ? int.Parse(Quantity) : 1;
                    listModel.Remark = Remarks;
                    listModel.Sheet = Sheet;
                    listModel.SubjectId = subjectId;
                    listModel.ShopId = shopid;
                    popBll.Add(listModel);
                    BindPOP();
                }
            }
        }

        protected void gvPOP_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object objIsDelete = item.GetType().GetProperty("popIsDelete").GetValue(item, null);
                    bool isDelete = objIsDelete != null ? bool.Parse(objIsDelete.ToString()) : false;
                    if (isDelete)
                    {
                        foreach (Control con in e.Item.Controls)
                        {
                            if (con.GetType().Name == "TextBox")
                            {
                                ((TextBox)con).Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        protected void AspNetPager3_PageChanged(object sender, EventArgs e)
        {
            BindSupplement();
        }

        protected void btnAddBC_Click(object sender, EventArgs e)
        {
            string POSCode = txtAddBCPOSCode.Text.Trim();
            int shopid = GetShopId(POSCode);
            string Sheet = txtAddBCSheet.Text.Trim();

            if (shopid == 0)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel3, GetType(), "alert", "<script>alert('店铺编号不存在')</script>", false);
            }
            else
            {

                string Gender = txtAddBCGender.Text.Trim();
                string Quantity = txtAddBCQuantity.Text.Trim();
                string ChooseImg = txtAddBCChooseImg.Text.Trim();
                string Remarks = txtAddBCRemarks.Text.Trim();

                var list1 = popBll.GetList(s => s.ShopId == shopid && s.SubjectId == subjectId && s.Sheet == Sheet && s.Gender == Gender && (s.IsDelete == null || s.IsDelete == false));
                if (list1.Any())
                {
                    //Alert("该POP订单已存在！");
                    System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel3, GetType(), "alert", "<script>alert('订单已存在')</script>", false);
                }
                else
                {

                    SupplementOrderDetail listModel = new SupplementOrderDetail();
                    listModel.AddDate = DateTime.Now;
                    listModel.AddUserId = CurrentUser.UserId;
                    listModel.ChooseImg = ChooseImg;

                    listModel.Gender = Gender;

                    listModel.Quantity = !string.IsNullOrWhiteSpace(Quantity) ? int.Parse(Quantity) : 1;
                    listModel.Remark = Remarks;
                    listModel.Sheet = Sheet;
                    listModel.SubjectId = subjectId;
                    listModel.ShopId = shopid;
                    supplementBll.Add(listModel);
                    BindSupplement();
                }
            }
        }

        protected void btnSreachBC_Click(object sender, EventArgs e)
        {
            AspNetPager3.CurrentPageIndex = 1;
            BindSupplement();
        }

        protected void btnDeleteBC_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvSupplement.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbBCOne");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfBCId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                idList.ForEach(id =>
                {
                    SupplementOrderDetail model = supplementBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = true;
                        supplementBll.Update(model);
                    }
                });
                BindSupplement();
            }
        }

        protected void btnReconverBC_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvPOP.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbBCOne");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfBCId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                idList.ForEach(id =>
                {
                    SupplementOrderDetail model = supplementBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = false;
                        supplementBll.Update(model);
                    }
                });
                BindSupplement();
            }
        }

        protected void btnSaveBCEdit_Click(object sender, EventArgs e)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    int failCount = 0;
                    foreach (RepeaterItem item in gvSupplement.Items)
                    {
                        if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                        {
                            CheckBox cb = (CheckBox)item.FindControl("cbBCOne");
                            if (cb.Checked)
                            {
                                bool canSave = true;
                                HiddenField hfId = (HiddenField)item.FindControl("hfBCId");
                                int id = int.Parse(hfId.Value);

                                TextBox txtBCSheet = (TextBox)item.FindControl("txtBCSheet");
                                TextBox txtBCGender = (TextBox)item.FindControl("txtBCGender");
                                TextBox txtBCQuantity = (TextBox)item.FindControl("txtBCQuantity");
                                TextBox txtBCChooseImg = (TextBox)item.FindControl("txtBCChooseImg");
                                TextBox txtBCRemark = (TextBox)item.FindControl("txtBCRemark");


                                string Sheet = txtBCSheet.Text.Trim();
                                string Gender = txtBCGender.Text.Trim();
                                string Quantity = txtBCQuantity.Text.Trim();
                                string ChooseImg = txtBCChooseImg.Text.Trim();
                                string Remark = txtBCRemark.Text.Trim();

                                if (string.IsNullOrWhiteSpace(Sheet))
                                {

                                    txtBCSheet.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                if (string.IsNullOrWhiteSpace(Gender))
                                {

                                    txtBCGender.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                if (string.IsNullOrWhiteSpace(Quantity))
                                {
                                    txtBCQuantity.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                else
                                {
                                    int num = 0;
                                    if (!int.TryParse(Quantity, out num))
                                    {
                                        txtBCQuantity.Style.Add("border", "1px solid red");
                                        canSave = false;
                                        failCount++;
                                    }
                                }
                                if (canSave)
                                {
                                    SupplementOrderDetail model = supplementBll.GetModel(id);
                                    if (model != null)
                                    {
                                        model.ChooseImg = ChooseImg;
                                        model.Gender = Gender;
                                        model.Quantity = int.Parse(Quantity);
                                        model.Remark = Remark;
                                        model.Sheet = Sheet;
                                        supplementBll.Update(model);
                                    }
                                }
                            }
                        }
                    }
                    if (failCount == 0)
                    {
                        tran.Complete();
                        string js = "$('#gvSupplement').find('input[name$=cbBCOne]').each(function(){this.checked=false;})";
                        System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel3, GetType(), "alert", "<script>alert('提交成功');" + js + "</script>", false);
                        BindSupplement();
                    }
                }
                catch (Exception ex)
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel3, GetType(), "alert", "<script>alert('提交失败：" + ex.Message + "')</script>", false);
                }
            }
        }



        protected void btnSreachMerge_Click(object sender, EventArgs e)
        {
            AspNetPager4.CurrentPageIndex = 1;
            BindMerge();
        }

        protected void btnDeleteMerge_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvMerge.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbMergeOne");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfMergeId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                idList.ForEach(id =>
                {
                    MergeOriginalOrder model = mergeBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = true;
                        mergeBll.Update(model);
                    }
                });
                BindMerge();
            }
        }

        protected void btnReconverMerge_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvMerge.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbMergeOne");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfMergeId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                idList.ForEach(id =>
                {
                    MergeOriginalOrder model = mergeBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = false;
                        mergeBll.Update(model);
                    }
                });
                BindMerge();
            }
        }

        protected void AspNetPager4_PageChanged(object sender, EventArgs e)
        {
            BindMerge();
        }

        /// <summary>
        /// 重新生成合并订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        bool isMergeOk = false;
        protected void btnReMerge_Click(object sender, EventArgs e)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    MergeOrder();
                    tran.Complete();


                    isMergeOk = true;
                }
                catch (Exception ex)
                {

                }
            }
            if (isMergeOk)
            {
               //System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel4, GetType(), "", "<script>closeLoading();</script>", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "", "<script>closeLoading();</script>", false);
                BindMerge();
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel4, GetType(), "alert", "<script>alert('操作失败！');</script>", false);
            }
        }

        /// <summary>
        /// 合并订单
        /// </summary>
        void MergeOrder()
        {

            MergeOriginalOrder model;
            ListOrderDetail listOrderModel;
            POPOrderDetail popdetail;
            mergeBll.Delete(s => s.SubjectId == subjectId);
            var list = listBll.GetList(s => s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false));
            var pop = popBll.GetList(s => s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false));
            var supplement = supplementBll.GetList(s => s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false));
            //List<MergeOriginalOrder> MergeList = new List<MergeOriginalOrder>();
            #region old
            ////先把补充订单数据加到list里面(按店铺编码、位置、性别、选图作为条件，相同的就保存大数量的那个)
            //#region
            //supplement.ForEach(s =>
            //{
            //    var listModel = list.FirstOrDefault(l => l.Sheet == s.Sheet && l.ShopId == s.ShopId && ((l.Gender == s.Gender) || (l.Gender.Contains("男") && l.Gender.Contains("女")) || (s.Gender.Contains("男") && s.Gender.Contains("女"))) && l.ChooseImg == s.ChooseImg);
            //    if (listModel != null)
            //    {
            //        int index = list.IndexOf(listModel);
            //        if (listModel.Quantity < s.Quantity)
            //        {
            //            listModel.Quantity = s.Quantity;
            //            list[index] = listModel;
            //        }
            //    }
            //    else
            //    {
            //        listOrderModel = new ListOrderDetail()
            //        {
            //            AddDate = s.AddDate,
            //            AddUserId = s.AddUserId,
            //            ChooseImg = s.ChooseImg,
            //            Gender = s.Gender,
            //            PositionId = s.PositionId,
            //            Quantity = s.Quantity,
            //            Remark = s.Remark,
            //            Sheet = s.Sheet,
            //            ShopId = s.ShopId,
            //            SubjectId = s.SubjectId,
            //            MaterialSupport = "",
            //            POSScale = ""

            //        };
            //        list.Add(listOrderModel);
            //    }
            //});
            ////再把list合并到pop中（按店铺编码、位置、性别、选图作为条件，相同的就保存大数量的那个）

            //if (list.Any())
            //{
            //    list.ForEach(l =>
            //    {

            //        var popModel = pop.FirstOrDefault(p => p.Sheet == l.Sheet && p.ShopId == l.ShopId && ((p.Gender == l.Gender) || (p.Gender.Contains("男") && p.Gender.Contains("女")) || (l.Gender.Contains("男") && l.Gender.Contains("女"))) && p.ChooseImg == l.ChooseImg);
            //        if (popModel != null)
            //        {
            //            //如果pop表和list表有相同的，用list的数据
            //            int index = pop.IndexOf(popModel);
            //            if (!string.IsNullOrWhiteSpace(l.MaterialSupport))
            //                popModel.MaterialSupport = l.MaterialSupport;
            //            if (!string.IsNullOrWhiteSpace(l.POSScale))
            //                popModel.POSScale = l.POSScale;
            //            if (popModel.Quantity < l.Quantity)
            //            {
            //                popModel.Quantity = l.Quantity;
            //                popModel.CornerType = l.CornerType;
            //                pop[index] = popModel;
            //            }

            //        }
            //        else
            //        {

            //            popdetail = new POPOrderDetail();
            //            popdetail.AddDate = DateTime.Now;
            //            popdetail.AddUserId = CurrentUser.UserId;
            //            popdetail.ChooseImg = l.ChooseImg;
            //            popdetail.Gender = l.Gender;
            //            popdetail.MaterialSupport = l.MaterialSupport;
            //            popdetail.PositionId = l.PositionId;
            //            popdetail.POSScale = l.POSScale;
            //            popdetail.Quantity = l.Quantity;
            //            popdetail.Remark = l.Remark;
            //            popdetail.Sheet = l.Sheet;
            //            popdetail.ShopId = l.ShopId;
            //            popdetail.SubjectId = l.SubjectId;
            //            popdetail.CornerType = l.CornerType;
            //            pop.Add(popdetail);
            //        }

            //    });
            //}
            //#endregion

            ////最后保存到合并表中
            //if (pop.Any())
            //{
            //    pop.ForEach(p =>
            //    {
            //        model = new MergeOriginalOrder();
            //        model.AddDate = DateTime.Now;
            //        model.ChooseImg = p.ChooseImg;
            //        model.Gender = p.Gender;
            //        model.GraphicNo = p.GraphicNo;
            //        model.MaterialSupport = p.MaterialSupport;
            //        model.POPName = p.POPName;
            //        model.POPType = p.POPType;
            //        model.PositionId = p.PositionId;
            //        model.POSScale = p.POSScale;
            //        model.Quantity = p.Quantity;
            //        model.Remark = p.Remark;
            //        model.Sheet = p.Sheet;
            //        model.ShopId = p.ShopId;
            //        model.SubjectId = p.SubjectId;
            //        model.CornerType = p.CornerType;
            //        mergeBll.Add(model);
            //    });
            //}
            #endregion
            #region 修改后
            #region 先把补充订单数据加到list里面(按店铺编码、位置、性别作为条件，相同的就保存大数量的那个)
            supplement.ForEach(s =>
            {
                var listModel = list.FirstOrDefault(l => l.Sheet == s.Sheet && l.ShopId == s.ShopId && ((l.Gender == s.Gender) || (l.Gender.Contains("男") && l.Gender.Contains("女")) || (s.Gender.Contains("男") && s.Gender.Contains("女"))));
                if (listModel != null)
                {
                    int index = list.IndexOf(listModel);
                    if (listModel.Quantity < s.Quantity)
                    {
                        listModel.Quantity = s.Quantity;
                        list[index] = listModel;
                    }
                }
                else
                {
                    listOrderModel = new ListOrderDetail()
                    {
                        AddDate = s.AddDate,
                        AddUserId = s.AddUserId,
                        ChooseImg = s.ChooseImg,
                        Gender = s.Gender,
                        PositionId = s.PositionId,
                        Quantity = s.Quantity,
                        Remark = s.Remark,
                        Sheet = s.Sheet,
                        ShopId = s.ShopId,
                        SubjectId = s.SubjectId,
                        MaterialSupport = s.MaterialSupport,
                        POSScale = s.POSScale,
                        LevelNum = 1
                    };
                    list.Add(listOrderModel);
                }
            });
            //再把list合并到pop中（按店铺编码、位置、性别作为条件，相同的就保存大数量的那个）

            if (list.Any())
            {
                list.ForEach(l =>
                {

                    var popModel = pop.FirstOrDefault(p => p.Sheet == l.Sheet && p.ShopId == l.ShopId && ((p.Gender == l.Gender) || (p.Gender.Contains("男") && p.Gender.Contains("女")) || (l.Gender.Contains("男") && l.Gender.Contains("女"))));
                    if (popModel != null)
                    {
                        //如果pop表和list表有相同的，用list的数据
                        int index = pop.IndexOf(popModel);
                        if (!string.IsNullOrWhiteSpace(l.MaterialSupport))
                            popModel.MaterialSupport = l.MaterialSupport;
                        if (!string.IsNullOrWhiteSpace(l.POSScale))
                            popModel.POSScale = l.POSScale;
                        if (string.IsNullOrWhiteSpace(popModel.ChooseImg))
                        {
                            popModel.ChooseImg = l.ChooseImg;
                        }

                        if (popModel.Quantity < l.Quantity)
                        {
                            popModel.Quantity = l.Quantity;

                        }
                        popModel.CornerType = l.CornerType;
                        popModel.LevelNum = l.LevelNum;
                        pop[index] = popModel;
                    }
                    else
                    {

                        popdetail = new POPOrderDetail();
                        popdetail.AddDate = DateTime.Now;
                        popdetail.AddUserId = CurrentUser.UserId;
                        popdetail.ChooseImg = l.ChooseImg;
                        popdetail.Gender = l.Gender;
                        popdetail.MaterialSupport = l.MaterialSupport;
                        popdetail.PositionId = l.PositionId;
                        popdetail.POSScale = l.POSScale;
                        popdetail.Quantity = l.Quantity;
                        popdetail.Remark = l.Remark;
                        popdetail.Sheet = l.Sheet;
                        popdetail.ShopId = l.ShopId;
                        popdetail.SubjectId = l.SubjectId;
                        popdetail.CornerType = l.CornerType;
                        popdetail.LevelNum = l.LevelNum;
                        pop.Add(popdetail);
                    }

                });
            }
            #endregion

            #region 更新那些“物料支持”和“规模大小”为空的
            if (pop.Any())
            {
                var emptyList = pop.Where(s => string.IsNullOrWhiteSpace(s.MaterialSupport) == true || string.IsNullOrWhiteSpace(s.POSScale) == true).ToList();
                Dictionary<int, MergeOriginalOrder> dic = new Dictionary<int, MergeOriginalOrder>();
                emptyList.ForEach(s =>
                {
                    int index = pop.IndexOf(s);
                    if (string.IsNullOrWhiteSpace(s.MaterialSupport))
                    {
                        var model1 = pop.Where(m => string.IsNullOrWhiteSpace(m.MaterialSupport) == false && m.ShopId == s.ShopId).FirstOrDefault();
                        if (model1 != null)
                        {
                            s.MaterialSupport = model1.MaterialSupport;
                        }

                    }
                    if (string.IsNullOrWhiteSpace(s.POSScale))
                    {
                        var model1 = pop.Where(m => string.IsNullOrWhiteSpace(m.POSScale) == false && m.ShopId == s.ShopId).FirstOrDefault();
                        if (model1 != null)
                        {
                            s.POSScale = model1.POSScale;
                        }

                    }
                    pop[index] = s;
                });
                dic = null;
            }
            #endregion


            #region 最后保存到合并表中
            List<MergeOriginalOrder> MergeList = new List<MergeOriginalOrder>();
            if (pop.Any())
            {
                pop.ForEach(p =>
                {
                    model = new MergeOriginalOrder();
                    model.AddDate = DateTime.Now;
                    model.ChooseImg = p.ChooseImg;
                    model.Gender = p.Gender;
                    model.GraphicNo = p.GraphicNo;
                    model.MaterialSupport = p.MaterialSupport;
                    model.POPName = p.POPName;
                    model.POPType = p.POPType;
                    model.PositionId = p.PositionId;
                    model.POSScale = p.POSScale;
                    model.Quantity = p.Quantity;
                    model.Remark = p.Remark;
                    model.Sheet = p.Sheet;
                    model.ShopId = p.ShopId;
                    model.SubjectId = p.SubjectId;
                    model.CornerType = p.CornerType;
                    model.LevelNum = p.LevelNum;
                    mergeBll.Add(model);

                });


            }
            #endregion
            #endregion

        }

        protected void gvMerge_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {

                    //object objSheet = item.GetType().GetProperty("Sheet").GetValue(item, null);
                    //object objLevelNum = item.GetType().GetProperty("LevelNum").GetValue(item, null);
                    //string sheet = objSheet != null ? objSheet.ToString() : "";
                    //int levelNum = objLevelNum != null ? int.Parse(objLevelNum.ToString()) : 0;
                    //DropDownList ddlLevelNum = (DropDownList)e.Item.FindControl("ddlListLevelNum");
                    //Label labLevelNum = (Label)e.Item.FindControl("labLevelNum");
                    //if (sheet.Contains("桌"))
                    //{
                    //    labLevelNum.Text = levelDic[levelNum];
                    //}
                   
                    
                }
            }
        }

        /// <summary>
        /// 如果是陈列桌，检查级别和系统数量，系统数量不足就不导入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        ShopMachineFrameBLL frmaeBll = new ShopMachineFrameBLL();
        bool CheckFrameNumber(int shopid, string gender, int levelNum)
        {
            bool flag = true;
            int sysCount = frmaeBll.GetList(s => s.ShopId == shopid && s.PositionName == "陈列桌" && ((s.Gender == "") || (s.Gender == null) || (s.Gender == gender) || (s.Gender.Contains("男") && s.Gender.Contains("女")))).Sum(s => s.Count) ?? 0;
            flag = sysCount >= levelNum;
            return flag;
        }

        protected void rblEditType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int type = int.Parse(rblEditType.SelectedValue);
            if (type == 1)
            {
                Panel1.Visible = true;
                Panel2.Visible = false;
            }
            else
            {
                Panel1.Visible = false;
                Panel2.Visible = true;
                
            }
        }

        void BindFinalOrder()
        {


            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join pop1 in CurrentContext.DbContext.POP
                        on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop1.ShopId, pop1.GraphicNo, pop1.Sheet } into popTemp
                        from pop in popTemp.DefaultIfEmpty()
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            order.IsDelete,
                            Sheet = order.Sheet,
                            LevelNum = order.LevelNum,
                            shop,
                            pop
                        }).ToList();
            if (!string.IsNullOrWhiteSpace(txtShopNoF.Text.Trim()))
            {
                list = list.Where(s => s.order.ShopNo.ToLower() == txtShopNoF.Text.Trim().ToLower()).ToList();
            }
            //if (!string.IsNullOrWhiteSpace(txtShopName.Text.Trim()))
            //{
            //    list = list.Where(s => s.order.ShopName.Contains(txtShopNo.Text.Trim())).ToList();
            //}
            //List<string> regionList = new List<string>();
            //foreach (ListItem li in cblRegion.Items)
            //{
            //    if (li.Selected)
            //    {
            //        regionList.Add(li.Value.ToLower());
            //    }
            //}
            //if (regionList.Any())
            //{
            //    list = list.Where(s => regionList.Contains(s.shop.RegionName.ToLower())).ToList();
            //}
            //if (!string.IsNullOrWhiteSpace(txtKeyWord.Text.Trim()))
            //{
            //    string keyWord = txtKeyWord.Text.Trim().ToLower();
            //    list = list.Where(s => s.order.Sheet.ToLower().Contains(keyWord) || (s.order.PositionDescription.ToLower().Contains(keyWord)) || (s.order.GraphicMaterial.ToLower().Contains(keyWord)) || (s.order.MachineFrame.ToLower().Contains(keyWord))).ToList();
            //}
            AspNetPager5.RecordCount = list.Count;
            this.AspNetPager5.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager5.CurrentPageIndex, this.AspNetPager5.PageCount, this.AspNetPager5.RecordCount, this.AspNetPager5.PageSize });
            gvFinalOrder.DataSource = list.OrderBy(s => s.order.ShopId).Skip((AspNetPager5.CurrentPageIndex - 1) * AspNetPager5.PageSize).Take(AspNetPager5.PageSize).ToList();
            gvFinalOrder.DataBind();

            

        }

        protected void btnSreachF_Click(object sender, EventArgs e)
        {
            BindFinalOrder();
        }

        protected void btnDeleteF_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvFinalOrder.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbOneF");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfFinalOrderId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                FinalOrderDetailTempBLL finalOrderBll = new FinalOrderDetailTempBLL();
                idList.ForEach(id =>
                {
                    FinalOrderDetailTemp model = finalOrderBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete = true;
                        finalOrderBll.Update(model);
                    }
                });
                BindFinalOrder();
            }
        }

        protected void btnReconverF_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            foreach (RepeaterItem item in gvFinalOrder.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbOneF");
                    if (cb.Checked)
                    {
                        HiddenField hfId = (HiddenField)item.FindControl("hfFinalOrderId");
                        int id = int.Parse(hfId.Value);
                        idList.Add(id);

                    }
                }
            }
            if (idList.Any())
            {
                FinalOrderDetailTempBLL finalOrderBll = new FinalOrderDetailTempBLL();
                idList.ForEach(id =>
                {
                    FinalOrderDetailTemp model = finalOrderBll.GetModel(id);
                    if (model != null)
                    {
                        model.IsDelete =false;
                        finalOrderBll.Update(model);
                    }
                });
                BindFinalOrder();
            }
        }

        /// <summary>
        /// 最终订单提交修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveEditF_Click(object sender, EventArgs e)
        {
            //using (TransactionScope tran = new TransactionScope())
            //{
                try
                {
                    int failCount = 0;
                    FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                    foreach (RepeaterItem item in gvFinalOrder.Items)
                    {
                        if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                        {
                            CheckBox cb = (CheckBox)item.FindControl("cbOneF");
                            if (cb.Checked)
                            {
                                bool canSave = true;
                                HiddenField hfOrderId = (HiddenField)item.FindControl("hfFinalOrderId");
                                int id = int.Parse(hfOrderId.Value);

                                TextBox txtSheet = (TextBox)item.FindControl("txtSheetF");
                                TextBox txtLevelNum = (TextBox)item.FindControl("txtLevelNumF");
                                TextBox txtPositionDescription = (TextBox)item.FindControl("txtPositionDescriptionF");
                                TextBox txtGender = (TextBox)item.FindControl("txtGenderF");
                                TextBox txtQuantity = (TextBox)item.FindControl("txtQuantityF");

                                TextBox txtGraphicWidth = (TextBox)item.FindControl("txtGraphicWidthF");
                                TextBox txtGraphicLength = (TextBox)item.FindControl("txtGraphicLengthF");

                                TextBox txtChooseImg = (TextBox)item.FindControl("txtChooseImgF");
                                TextBox txtRemark = (TextBox)item.FindControl("txtRemarkF");


                                string Sheet = txtSheet.Text.Trim();
                                string LevelNum = txtLevelNum.Text.Trim();
                                string PositionDescription = txtPositionDescription.Text.Trim();
                                
                                string Gender = txtGender.Text.Trim();
                                string Quantity = txtQuantity.Text.Trim();
                                string GraphicWidth = txtGraphicWidth.Text.Trim();
                                string GraphicLength = txtGraphicLength.Text.Trim();

                                string ChooseImg = txtChooseImg.Text.Trim();
                                string Remark = txtRemark.Text.Trim();

                                if (string.IsNullOrWhiteSpace(Sheet))
                                {

                                    txtSheet.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                if (string.IsNullOrWhiteSpace(Gender))
                                {

                                    txtGender.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                if (string.IsNullOrWhiteSpace(Quantity))
                                {
                                    txtQuantity.Style.Add("border", "1px solid red");
                                    canSave = false;
                                    failCount++;
                                }
                                else
                                {
                                    int num = 0;
                                    if (!int.TryParse(Quantity, out num))
                                    {
                                        txtQuantity.Style.Add("border", "1px solid red");
                                        canSave = false;
                                        failCount++;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(LevelNum))
                                {
                                    int num = 0;
                                    if (!int.TryParse(LevelNum, out num))
                                    {
                                        txtLevelNum.Style.Add("border", "1px solid red");
                                        canSave = false;
                                        failCount++;
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(GraphicWidth))
                                {
                                    int num = 0;
                                    if (!int.TryParse(GraphicWidth, out num))
                                    {
                                        txtGraphicWidth.Style.Add("border", "1px solid red");
                                        canSave = false;
                                        failCount++;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(GraphicLength))
                                {
                                    int num = 0;
                                    if (!int.TryParse(GraphicLength, out num))
                                    {
                                        txtGraphicLength.Style.Add("border", "1px solid red");
                                        canSave = false;
                                        failCount++;
                                    }
                                }

                                if (canSave)
                                {
                                    FinalOrderDetailTemp model = orderBll.GetModel(id);
                                    if (model != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(GraphicLength))
                                            model.GraphicLength = decimal.Parse(GraphicLength);
                                        else
                                            model.GraphicLength = null;
                                        if (!string.IsNullOrWhiteSpace(GraphicWidth))
                                            model.GraphicWidth = decimal.Parse(GraphicWidth);
                                        else
                                            model.GraphicWidth = null;
                                        model.Area = ((model.GraphicLength ?? 0) * (model.GraphicWidth ?? 0)) / 1000000;
                                        model.PositionDescription = PositionDescription;
                                        model.ChooseImg = ChooseImg;
                                        model.Gender = Gender;
                                        model.Quantity = int.Parse(Quantity);
                                        model.Remark = Remark;
                                        model.Sheet = Sheet;
                                        orderBll.Update(model);
                                    }
                                }
                            }
                        }
                    }
                    if (failCount == 0)
                    {
                        //tran.Complete();
                       
                        string js = "$('#gvFinalOrder').find('input[name$=cbOneF]').each(function(){this.checked=false;})";

                        System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel5, GetType(), "alert", "<script>alert('提交成功');" + js + "</script>", false);
                        BindFinalOrder();
                    }
                }
                catch (Exception ex)
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(UpdatePanel5, GetType(), "alert", "<script>alert('提交失败：" + ex.Message + "')</script>", false);
                }
            //}
        }

        protected void gvFinalOrder_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object IsDeleteObj = item.GetType().GetProperty("IsDelete").GetValue(item,null);
                    bool isDelete = IsDeleteObj != null ? bool.Parse(IsDeleteObj.ToString()) : false;
                    if (isDelete)
                    { 
                        //gvFinalOrder.Items[e.Item.ItemIndex].
                        //HtmlTableCellCollection cells = (((HtmlTableRow)e.Item.FindControl("trId")).Cells);
                        //foreach (HtmlTableCell cell in cells)
                        //{
                        //    cell.Attributes.Add("style","color:red");
                        //}
                        ((HtmlTableRow)e.Item.FindControl("trId")).Attributes.Add("style", "color:red");
                        foreach(Control c in e.Item.Controls)
                        {
                            if (c is TextBox)
                            {
                                ((TextBox)c).Enabled = false;
                            }
                        }
                    }
                }
            }
        }
        
    }
}


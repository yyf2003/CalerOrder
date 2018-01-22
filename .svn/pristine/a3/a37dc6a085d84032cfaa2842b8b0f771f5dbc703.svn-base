using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using BLL;
using Common;
using DAL;
using Models;

namespace WebApp.Subjects.Supplement
{
    public partial class EditOrder : BasePage
    {
        public int customerId=0;
        int itemId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["supplementId"] != null)
            {
                itemId = int.Parse(Request.QueryString["supplementId"]);
            }
            if (Request.QueryString["import"] != null)
            {
                
                if (Session["fail"] != null)
                {
                    labState.Text = "部分数据导入失败！";
                    lbExportErrorMsg.Visible = true;
                }
                labState.Visible = true;
              
            }
            BindSubject();
            if (!IsPostBack)
            {
                BindOrder();
            }
        }

        void BindSubject()
        {
            //SupplementItem supplementModel = new SupplementItemBLL().GetModel(itemId);
            var supplementModel = (from supplement in CurrentContext.DbContext.SupplementItem
                        join subject in CurrentContext.DbContext.Subject
                        on supplement.SubjectId equals subject.Id
                        where supplement.SupplementId == itemId
                        select new {
                            supplement,
                            subject.CustomerId
                        }).FirstOrDefault();
            if (supplementModel != null)
            {
                if (supplementModel.supplement.IsSubmit != null && supplementModel.supplement.IsSubmit == 1)
                    Panel1.Visible = false;
                customerId = supplementModel.CustomerId ?? 0;
            }
        }

        void BindOrder()
        {
            var list = (from order in CurrentContext.DbContext.SupplementDetail
                       join shop in CurrentContext.DbContext.Shop
                       on order.ShopId equals shop.Id
                       where order.ItemId == itemId
                       select new { 
                          order,
                          shop
                       }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gvPOP.DataSource = list.OrderByDescending(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gvPOP.DataBind();
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "SupplementDetailTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile && itemId > 0)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                    OleDbConnection conn;
                    OleDbDataAdapter da;
                    DataSet ds = null;
                    path = Server.MapPath(path);
                    ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                    conn = new OleDbConnection(ExcelConnStr);
                    conn.Open();
                    string sql = "select * from [Sheet1$]";
                    da = new OleDbDataAdapter(sql, conn);
                    ds = new DataSet();
                    da.Fill(ds);
                    da.Dispose();
                    conn.Dispose();
                    conn.Close();
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        //if (!cbAdd.Checked)
                            //materialBll.Delete(s => s.ItemId == ItemId);


                        DataColumnCollection cols = ds.Tables[0].Columns;
                        DataTable errorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);
                        SupplementDetail orderModel;
                        SupplementDetailBLL orderBll = new SupplementDetailBLL();
                        int shopId = 0;
                        //int successNum=0;
                        DataTable dt1 = ds.Tables[0];
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            bool canSave = true;
                            StringBuilder msg = new StringBuilder();
                            string shopMsg = string.Empty;
                            string shopNo = string.Empty;
                            string sheet = string.Empty;
                            string levelNum = string.Empty;
                            string width = string.Empty;
                            string length = string.Empty;
                            string gender = string.Empty;
                            string count = string.Empty;
                            string material = string.Empty;
                            string unitPrice = string.Empty;
                            string chooseImg = string.Empty;
                            string positionDescription = string.Empty;
                            if (cols.Contains("POSCode"))
                            {
                                shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["POSCode"].ToString());
                            }
                            else if (cols.Contains("POS Code"))
                            {
                                shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["POS Code"].ToString());
                            }
                            else if (cols.Contains("店铺编号"))
                            {
                                shopNo = StringHelper.ReplaceSpace(dt1.Rows[i]["店铺编号"].ToString());
                            }
                            if (string.IsNullOrWhiteSpace(shopNo))
                            {
                                canSave = false;
                                msg.Append("店铺编号为空；");
                            }
                            if (!string.IsNullOrWhiteSpace(shopNo) && !CheckShop(shopNo, out shopId, out shopMsg))
                            {
                                canSave = false;
                                msg.Append(shopMsg);
                            }
                            if (cols.Contains("POP位置"))
                            {
                                sheet = StringHelper.ReplaceSpace(dt1.Rows[i]["POP位置"].ToString());
                            }
                            else if (cols.Contains("pop位置"))
                            {
                                sheet = StringHelper.ReplaceSpace(dt1.Rows[i]["pop位置"].ToString());
                            }
                            else if (cols.Contains("Sheet"))
                            {
                                sheet = StringHelper.ReplaceSpace(dt1.Rows[i]["Sheet"].ToString());
                            }
                            else if (cols.Contains("sheet"))
                            {
                                sheet = StringHelper.ReplaceSpace(dt1.Rows[i]["sheet"].ToString());
                            }
                            if (string.IsNullOrWhiteSpace(sheet))
                            {
                                canSave = false;
                                msg.Append("POP位置为空；");
                            }
                            if (cols.Contains("级别"))
                            {
                                levelNum = StringHelper.ReplaceSpace(dt1.Rows[i]["级别"].ToString());
                            }
                            if (cols.Contains("宽"))
                            {
                                width = StringHelper.ReplaceSpace(dt1.Rows[i]["宽"].ToString());
                            }
                            if (!string.IsNullOrWhiteSpace(width) && !StringHelper.IsDecimalVal(width))
                            {
                                canSave = false;
                                msg.Append("宽填写不正确；");
                            }
                            if (cols.Contains("高"))
                            {
                                length = StringHelper.ReplaceSpace(dt1.Rows[i]["高"].ToString());
                            }
                            if (!string.IsNullOrWhiteSpace(length) && !StringHelper.IsDecimalVal(length))
                            {
                                canSave = false;
                                msg.Append("高填写不正确；");
                            }
                            if (cols.Contains("性别"))
                            {
                                gender = StringHelper.ReplaceSpace(dt1.Rows[i]["性别"].ToString());
                            }
                            if (string.IsNullOrWhiteSpace(gender))
                            {
                                canSave = false;
                                msg.Append("性别为空；");
                            }
                            if (cols.Contains("数量"))
                            {
                                count = StringHelper.ReplaceSpace(dt1.Rows[i]["数量"].ToString());
                            }
                            if (string.IsNullOrWhiteSpace(count))
                            {
                                canSave = false;
                                msg.Append("数量为空；");
                            }
                            else if (!StringHelper.IsIntVal(count))
                            {
                                canSave = false;
                                msg.Append("数量填写不正确；");
                            }
                            if (cols.Contains("材质"))
                            {
                                material = StringHelper.ReplaceSpace(dt1.Rows[i]["材质"].ToString());
                            }
                            if (cols.Contains("单价"))
                            {
                                unitPrice = StringHelper.ReplaceSpace(dt1.Rows[i]["单价"].ToString());
                            }
                            if (!string.IsNullOrWhiteSpace(unitPrice) && !StringHelper.IsDecimalVal(unitPrice))
                            {
                                canSave = false;
                                msg.Append("单价填写不正确；");
                            }
                            if (cols.Contains("选图"))
                            {
                                chooseImg = StringHelper.ReplaceSpace(dt1.Rows[i]["选图"].ToString());
                            }
                            if (cols.Contains("位置说明"))
                            {
                                positionDescription = StringHelper.ReplaceSpace(dt1.Rows[i]["位置说明"].ToString());
                            }
                            if (canSave)
                            {
                                orderModel = new SupplementDetail();
                                orderModel.ChooseImg = chooseImg;
                                orderModel.Gender = gender;
                                decimal width0 = StringHelper.IsDecimal(width);
                                decimal length0 = StringHelper.IsDecimal(length);
                                orderModel.GraphicLength = length0;
                                orderModel.GraphicWidth = width0;
                                orderModel.Area = Math.Round((width0*length0)/1000000,2);
                                orderModel.GraphicMaterial = material;
                                orderModel.ItemId = itemId;
                                orderModel.PositionDescription = positionDescription;
                                orderModel.Quantity = StringHelper.IsInt(count);
                                orderModel.Sheet = sheet;
                                orderModel.ShopId = shopId;
                                orderModel.UnitPrice = StringHelper.IsDecimal(unitPrice);
                                orderBll.Add(orderModel);
                                //successNum++;
                            }
                            //
                            if (msg.Length > 0)
                            {
                                DataRow dr1 = errorTB.NewRow();
                                for (int ii = 0; ii < cols.Count; ii++)
                                {
                                    dr1["" + cols[ii] + ""] = dt1.Rows[i][ii].ToString();
                                }
                                dr1["错误信息"] = msg.ToString();
                                errorTB.Rows.Add(dr1);
                            }
                        }
                        if (errorTB != null && errorTB.Rows.Count > 0)
                        {
                            
                            Session["fail"] = errorTB;
                        }
                        else
                            Session["fail"] = null;
                        Response.Redirect("EditOrder.aspx?import=1&supplementId=" + itemId, false);
                    }
                    else
                    {
                        //Panel1.Visible = true;
                        labState.Text = "导入失败：导入文件里无有效数据";
                        return;
                    }
                }
            }
        }

        protected void btnAddOrderDetail_Click(object sender, EventArgs e)
        {
            string shopNo = txtPOSCode.Text.Trim();
            string sheet = ddlSheet.SelectedValue;
            string levelNum = ddlLevelNum.SelectedValue;
            string width = txtGraphicWidth.Text.Trim();
            string length = txtGraphicLength.Text.Trim();
            string gender = ddlGender.SelectedValue;
            string quantity = txtQuantity.Text.Trim();
            string graphicMaterial = hfMaterial.Value;
            string unitPrice = txtUnitPrice.Text.Trim();
            string chooseImg = txtChooseImg.Text.Trim();
            string positionDescription = txtPositionDescription.Text.Trim();
            int shopId = 0;
            string msg = string.Empty;
            if (!CheckShop(shopNo, out shopId, out msg))
            {
                Alert(msg);
            }
            else
            {
                SupplementDetail detailModel = new SupplementDetail();
                SupplementDetailBLL detailBll=new SupplementDetailBLL();
                
                detailModel.ChooseImg = chooseImg;
                detailModel.Gender = gender;
                decimal width1 = StringHelper.IsDecimal(width);
                decimal length1 = StringHelper.IsDecimal(length);
                detailModel.GraphicWidth = width1;
                detailModel.GraphicLength = length1;
                if (width1 > 0 && length1 > 0)
                    detailModel.Area = Math.Round((width1 * length1) / 1000000, 2);
                detailModel.GraphicMaterial = graphicMaterial;
                detailModel.ItemId = itemId;
                detailModel.PositionDescription = positionDescription;
                detailModel.Quantity = StringHelper.IsInt(quantity);
                detailModel.Sheet = sheet;
                detailModel.ShopId = shopId;
                detailModel.LevelNum = levelNum;
                detailModel.UnitPrice = StringHelper.IsDecimal(unitPrice);
                detailBll.Add(detailModel);
                BindOrder();
                txtPOSCode.Text="";
                ddlSheet.SelectedValue="";
                ddlLevelNum.SelectedValue="";
                txtGraphicWidth.Text= "";
                txtGraphicLength.Text = "";
                ddlGender.SelectedValue="";
                txtQuantity.Text = "1";
                txtGraphicMaterial.Text = "";
                hfMaterial.Value = "";
                txtUnitPrice.Text = "";
                txtChooseImg.Text = "";
                txtPositionDescription.Text = "";
            }
        }

        bool CheckShop(string shopNo, out int shopId,out string msg)
        {
            bool flag = false;
            shopId = 0;
            msg = string.Empty;
            var model = new ShopBLL().GetList(s=>s.ShopNo==shopNo).FirstOrDefault();
            if (model != null)
            {
                if (!string.IsNullOrWhiteSpace(model.Status) && model.Status != "正常")
                {
                    msg = "该店铺已经关闭";
                }
                else
                {
                    shopId = model.Id;
                    flag = true;
                }
            }
            else
                msg = "店铺不存在";

            return flag;
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindOrder();
        }

        protected void lbExportErrorMsg_Click(object sender, EventArgs e)
        {
            if (Session["fail"] != null)
            {
                DataTable dt = (DataTable)Session["fail"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void gvPOP_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName == "deleteItem")
            {
                SupplementDetailBLL detailBll = new SupplementDetailBLL();
                SupplementDetail detailModel = detailBll.GetModel(id);
                if (detailModel != null)
                {
                    detailBll.Delete(detailModel);
                    BindOrder();
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdateOrderDetail_Click(object sender, EventArgs e)
        {
            string shopNo = txtPOSCode.Text.Trim();
            string sheet = ddlSheet.SelectedValue;
            string levelNum = ddlLevelNum.SelectedValue;
            string width = txtGraphicWidth.Text.Trim();
            string length = txtGraphicLength.Text.Trim();
            string gender = ddlGender.SelectedValue;
            string quantity = txtQuantity.Text.Trim();
            string graphicMaterial = hfMaterial.Value;
            string unitPrice = txtUnitPrice.Text.Trim();
            string chooseImg = txtChooseImg.Text.Trim();
            string positionDescription = txtPositionDescription.Text.Trim();
            int shopId = 0;
            string msg = string.Empty;
            if (!CheckShop(shopNo, out shopId, out msg))
            {
                Alert(msg);
            }
            else
            {
                SupplementDetail detailModel = new SupplementDetail();
                SupplementDetailBLL detailBll = new SupplementDetailBLL();
                int id = StringHelper.IsInt(hfCurrDetailId.Value);
                
                if (id > 0)
                {
                    detailModel = detailBll.GetModel(id);
                    if (detailModel != null)
                    {
                        detailModel.ChooseImg = chooseImg;
                        detailModel.Gender = gender;
                        decimal width1 = StringHelper.IsDecimal(width);
                        decimal length1 = StringHelper.IsDecimal(length);
                        detailModel.GraphicWidth = width1;
                        detailModel.GraphicLength = length1;
                        if (width1 > 0 && length1 > 0)
                            detailModel.Area = Math.Round((width1 * length1) / 1000000, 2);
                        detailModel.GraphicMaterial = graphicMaterial;
                        detailModel.ItemId = itemId;
                        detailModel.PositionDescription = positionDescription;
                        detailModel.Quantity = StringHelper.IsInt(quantity);
                        detailModel.Sheet = sheet;
                        detailModel.ShopId = shopId;
                        detailModel.LevelNum = levelNum;
                        detailModel.UnitPrice = StringHelper.IsDecimal(unitPrice);
                        detailBll.Update(detailModel);
                        
                        txtPOSCode.Text = "";
                        ddlSheet.SelectedValue = "";
                        ddlLevelNum.SelectedValue = "";
                        txtGraphicWidth.Text = "";
                        txtGraphicLength.Text = "";
                        ddlGender.SelectedValue = "";
                        txtQuantity.Text = "1";
                        txtGraphicMaterial.Text = "";
                        hfMaterial.Value = "";
                        txtUnitPrice.Text = "";
                        txtChooseImg.Text = "";
                        txtPositionDescription.Text = "";
                        hfCurrDetailId.Value = "";
                        BindOrder();
                    }
                    else
                        msg = "更新失败";
                }
                else
                    msg = "更新失败";
                if (!string.IsNullOrWhiteSpace(msg))
                    Alert(msg);
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SupplementItem supplementModel = new SupplementItemBLL().GetModel(itemId);
            if (supplementModel != null)
            {
                supplementModel.IsSubmit = 1;
                new SupplementItemBLL().Update(supplementModel);
                ExcuteJs("FinishSubmit");
            }
        }
    }
}
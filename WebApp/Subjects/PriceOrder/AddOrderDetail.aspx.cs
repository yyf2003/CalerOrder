using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

using System.Text;
using BLL;
using Common;
using DAL;
using Models;
using System.Transactions;
using System.Data;
using System.Configuration;

namespace WebApp.Subjects.PriceOrder
{
    public partial class AddOrderDetail : BasePage
    {
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["import"] != null)
            {

                Panel1.Visible = true;
                if (Session["errorTb"] != null)
                {
                    labState.Text = "部分数据导入失败！";
                    ExportFailMsg.Style.Add("display", "block");
                }
               
                

            }
            if (!IsPostBack)
            {
                BindSubject();
                BindOrderData();
                //BindOrderType();
            }
        }

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                where subject.Id == subjectId
                                select new
                                {
                                    subject.SubjectName,
                                    subject.SubjectNo,
                                    customer.CustomerName,
                                    subject.CustomerId,
                                    subject.SubjectType,
                                    subject.GuidanceId
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                
                labSubjectName.Text = subjectModel.SubjectName;
                labSubjectNo.Text = subjectModel.SubjectNo;
                labCustomer.Text = subjectModel.CustomerName;
                int subjectType = subjectModel.SubjectType ?? 1;
                labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                hfGuidanceId.Value = (subjectModel.GuidanceId ?? 0).ToString();
            }
        }

       

        void BindOrderData()
        {
            var orderList = new PriceOrderDetailBLL().GetList(s => s.SubjectId == subjectId);
            AspNetPager1.RecordCount = orderList.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            orderListRepeater.DataSource = orderList.OrderBy(s => s.ShopId).ThenBy(s => s.OrderType).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            orderListRepeater.DataBind();
            Panel_OrderList.Visible = orderList.Any();
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    DataTable errorTB = null;
                    OleDbConnection conn;
                    OleDbDataAdapter da;
                    DataSet ds = null;
                    path = Server.MapPath(path);
                    string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                    ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                    conn = new OleDbConnection(ExcelConnStr);
                    conn.Open();
                    string sql = "select * from [Sheet1$]";
                    da = new OleDbDataAdapter(sql, conn);
                    ds = new DataSet();
                    da.Fill(ds);
                    da.Dispose();
                    
                    PriceOrderDetailBLL priceOrderBll = new PriceOrderDetailBLL();
                    PriceOrderDetail priceModel;
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        if (!cbAdd.Checked)
                        {
                            priceOrderBll.Delete(s=>s.SubjectId==subjectId);
                        }
                        List<string> orderTypeList = CommonMethod.GetEnumList<OrderTypeEnum>().Where(s => s.Desction.Contains("费用订单")).Select(s=>s.Name).ToList();
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        errorTB = CommonMethod.CreateErrorTB(cols);
                        int shopId = 0;
                        string orderType = string.Empty;
                        //店铺编号
                        string shopNo = string.Empty;
                        //应收费用金额
                        string price = string.Empty;
                        //应付费用金额
                        string payPrice = string.Empty;
                        string contents = string.Empty;
                        string remark = string.Empty;
                        
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringBuilder msg = new StringBuilder();
                            int orderTypeIndex = 0;
                            bool canSave = true;
                            if (cols.Contains("费用类型"))
                                orderType = StringHelper.ReplaceSpecialChar(dr["费用类型"].ToString().Trim());

                            if (cols.Contains("店铺编号"))
                                shopNo = StringHelper.ReplaceSpecialChar(dr["店铺编号"].ToString().Trim());
                            else if (cols.Contains("POSCode"))
                                shopNo = StringHelper.ReplaceSpecialChar(dr["POSCode"].ToString().Trim());
                            else if (cols.Contains("POS Code"))
                                shopNo = StringHelper.ReplaceSpecialChar(dr["POS Code"].ToString().Trim());

                            if (cols.Contains("费用金额"))
                                price = StringHelper.ReplaceSpecialChar(dr["费用金额"].ToString().Trim());
                            else if (cols.Contains("费用"))
                                price = StringHelper.ReplaceSpecialChar(dr["费用"].ToString().Trim());
                            else if (cols.Contains("金额"))
                                price = StringHelper.ReplaceSpecialChar(dr["金额"].ToString().Trim());
                            else if (cols.Contains("应收金额"))
                                price = StringHelper.ReplaceSpecialChar(dr["应收金额"].ToString().Trim());
                            else if (cols.Contains("应收费用金额"))
                                price = StringHelper.ReplaceSpecialChar(dr["应收费用金额"].ToString().Trim());
                            else if (cols.Contains("应收费用"))
                                price = StringHelper.ReplaceSpecialChar(dr["应收费用"].ToString().Trim());

                            if (cols.Contains("应付费用金额"))
                                payPrice = StringHelper.ReplaceSpecialChar(dr["应付费用金额"].ToString().Trim());
                            else if (cols.Contains("应付费用"))
                                payPrice = StringHelper.ReplaceSpecialChar(dr["应付费用"].ToString().Trim());
                            else if (cols.Contains("应付金额"))
                                payPrice = StringHelper.ReplaceSpecialChar(dr["应付金额"].ToString().Trim());

                            if (cols.Contains("费用内容"))
                                contents = StringHelper.ReplaceSpecialChar(dr["费用内容"].ToString().Trim());
                            if (cols.Contains("备注"))
                                remark = StringHelper.ReplaceSpecialChar(dr["备注"].ToString().Trim());
                            if (string.IsNullOrWhiteSpace(orderType))
                            {
                                canSave = false;
                                msg.Append("费用类型 为空；");
                            }
                            else if (!orderTypeList.Contains(orderType))
                            {
                                canSave = false;
                                msg.Append("费用类型 填写不正确；");
                            }
                            else
                            {
                                OrderTypeEnum otEnum = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), orderType, true);
                                orderTypeIndex = (int)otEnum;
                            }
                            if (string.IsNullOrWhiteSpace(shopNo))
                            {
                                canSave = false;
                                msg.Append("店铺编号 为空；");
                            }
                            Shop shopFromDB = null;
                            if (!string.IsNullOrWhiteSpace(shopNo) && GetShopFromDB(shopNo, ref shopFromDB))
                            {
                                if (shopFromDB != null)
                                {
                                    shopId = shopFromDB.Id;
                                }
                                else
                                {
                                    canSave = false;
                                    msg.Append("店铺编号不存在；");
                                }
                            }
                            else
                            {
                                canSave = false;
                                msg.Append("店铺编号不存在；");
                            }
                            if (string.IsNullOrWhiteSpace(price))
                            {
                                canSave = false;
                                msg.Append("应收金额 为空；");
                            }
                            else if (!StringHelper.IsDecimalVal(price))
                            {
                                canSave = false;
                                msg.Append("应收金额填写不正确；");
                            }
                            //if (string.IsNullOrWhiteSpace(payPrice))
                            //{
                            //    canSave = false;
                            //    msg.Append("应付金额 为空；");
                            //}
                            if (!string.IsNullOrWhiteSpace(payPrice) &&!StringHelper.IsDecimalVal(payPrice))
                            {
                                canSave = false;
                                msg.Append("应付金额填写不正确；");
                            }
                            if (canSave)
                            {
                                if (string.IsNullOrWhiteSpace(payPrice))
                                    payPrice = price;
                                priceModel = new PriceOrderDetail();
                                priceModel.AddDate = DateTime.Now;
                                priceModel.Address = shopFromDB.POPAddress;
                                priceModel.Amount = StringHelper.IsDecimal(price);
                                priceModel.PayAmount = StringHelper.IsDecimal(payPrice);
                                priceModel.City = shopFromDB.CityName;
                                priceModel.Contents = contents;
                                priceModel.Province = shopFromDB.ProvinceName;
                                priceModel.Region = shopFromDB.RegionName;
                                priceModel.Remark = remark;
                                priceModel.ShopId = shopId;
                                priceModel.ShopName = shopFromDB.ShopName;
                                priceModel.SubjectId = subjectId;
                                priceModel.OrderType = orderTypeIndex;
                                priceModel.GuidanceId = int.Parse(hfGuidanceId.Value);
                                priceModel.ShopNo = shopFromDB.ShopNo;
                                priceOrderBll.Add(priceModel);
                                
                            }
                            else
                            {

                                DataRow dr1 = errorTB.NewRow();
                                for (int ii = 0; ii < cols.Count; ii++)
                                {
                                    dr1["" + cols[ii] + ""] = dr[cols[ii]];
                                }
                                dr1["错误信息"] = msg.ToString();
                                errorTB.Rows.Add(dr1);
                            }
                        }
                        if (errorTB.Rows.Count > 0)
                        {
                            Session["errorTb"] = errorTB;
                        }
                        else
                        {
                            Session["errorTb"] = null;
                        }
                        conn.Dispose();
                        conn.Close();
                        Response.Redirect(string.Format("AddOrderDetail.aspx?import=1&subjectId={0}", subjectId), false);
                    }
                    else
                    {
                        Panel1.Visible = true;
                        labState.Text = "导入失败：表格里面没有数据！";
                    }
                }
            }
        }

        ShopBLL shopBll = new ShopBLL();
        Dictionary<string, Shop> shopDic = new Dictionary<string, Shop>();
        int guidanceType = 0;
        bool GetShopFromDB(string shopNo, ref Shop shopFromDb)
        {
            bool flag = false;
            shopNo = shopNo.ToUpper();
            if (shopDic.Keys.Contains(shopNo))
            {
                shopFromDb = shopDic[shopNo];
                flag = true;
            }
            else
            {
                var shopModel = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo && (s.IsDelete == null || s.IsDelete == false)).FirstOrDefault();
                if (shopModel != null)
                {
                    shopFromDb = shopModel;
                    shopDic.Add(shopNo, shopFromDb);
                    flag = true;
                }
            }
            if (guidanceType == 0)
            {
                var guidance = (from subject in CurrentContext.DbContext.Subject
                                join g in CurrentContext.DbContext.SubjectGuidance
                                on subject.GuidanceId equals g.ItemId
                                where subject.Id == subjectId
                                select g).FirstOrDefault();
                if (guidance != null)
                {
                    guidanceType = guidance.ActivityTypeId ?? 1;//活动类型：1安装，2发货，3促销
                    
                }
            }
            return flag;
        }

        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "费用订单模板";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void lbExportError_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindOrderData();
        }

        protected void orderListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                PriceOrderDetail model = (PriceOrderDetail)e.Item.DataItem;
                if (model != null)
                {
                    string orderType = (model.OrderType ?? 0).ToString();
                    ((Label)e.Item.FindControl("labOrderType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType);
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int itemCount = orderListRepeater.Items.Count;
            if (itemCount == 0)
            {
                Alert("提交失败：订单列表为空");
            }
            else
            {
                SubjectBLL subjectBll = new SubjectBLL();
                Subject model = subjectBll.GetModel(subjectId);
                if (model != null)
                {
                    model.Status = 4;
                    model.ApproveState = 0;
                    subjectBll.Update(model);
                    Alert("提交成功", "/Subjects/SubjectList.aspx");
                }
                else
                {
                    Alert("提交失败");
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/Subjects/AddSubject.aspx?subjectId={0}", subjectId), false);
        }
    }
}
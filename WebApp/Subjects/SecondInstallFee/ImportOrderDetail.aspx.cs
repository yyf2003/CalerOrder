using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.OleDb;
using Common;
using BLL;
using Models;
using System.Data;
using System.Text;
using DAL;

namespace WebApp.Subjects.SecondInstallFee
{
    public partial class ImportOrderDetail : BasePage
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
                int successNum = 0;
                if (Request.QueryString["successNum"] != null)
                    successNum = int.Parse(Request.QueryString["successNum"]);
                if (successNum > 0)
                    ExcuteJs("Change");

            }
            if (!IsPostBack)
            {
                Subject model = new SubjectBLL().GetModel(subjectId);
                if (model != null)
                {
                    int customerId = model.CustomerId ?? 0;
                    hfCustomerId.Value = customerId.ToString();
                }
            }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            #region
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    int customerId = 0;
                    if (!string.IsNullOrWhiteSpace(hfCustomerId.Value))
                    {
                        customerId = StringHelper.IsInt(hfCustomerId.Value);
                    }


                    RegionOrderDetailBLL orderDetailBll = new RegionOrderDetailBLL();

                    RegionOrderDetail orderDetailModel;
                    SubjectBLL subjectBll = new SubjectBLL();
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
                    int successNum = 0;
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        errorTB = CommonMethod.CreateErrorTB(cols);
                        if (cbDeleteOld.Checked)
                        {
                            orderDetailBll.Delete(s => s.SubjectId == subjectId);
                        }
                        int shopId = 0;
                        string orderType = string.Empty;

                        //店铺编号
                        string shopNo = string.Empty;
                        //店铺大小
                        string posScale = string.Empty;
                        //店铺级别
                        string materialSupport = string.Empty;
                        //费用金额
                        string price = string.Empty;
                        //应付费用金额
                        string payPrice = string.Empty;
                        //性别
                        string gender = string.Empty;

                        //系列/选图
                        string chooseImg = string.Empty;

                        //string category = string.Empty;

                        //string levelNum = string.Empty;
                        //pop位置
                        string sheet = string.Empty;
                        //POP位置明细
                        string positionDescription = string.Empty;
                        //数量
                        string num = string.Empty;
                        //pop材质
                        string material = string.Empty;
                        //pop宽
                        string width = string.Empty;
                        //pop高
                        string length = string.Empty;

                        //其他备注
                        string remark = string.Empty;


                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringBuilder msg = new StringBuilder();

                            if (cols.Contains("订单类型"))
                                orderType = dr["订单类型"].ToString().Trim();

                            if (cols.Contains("店铺编号"))
                                shopNo = dr["店铺编号"].ToString().Trim();
                            else if (cols.Contains("POSCode"))
                                shopNo = dr["POSCode"].ToString().Trim();
                            else if (cols.Contains("POS Code"))
                                shopNo = dr["POS Code"].ToString().Trim();

                            if (cols.Contains("店铺规模大小"))
                                posScale = dr["店铺规模大小"].ToString().Trim();
                            else if (cols.Contains("店铺规模"))
                                posScale = dr["店铺规模"].ToString().Trim();
                            else if (cols.Contains("店铺大小"))
                                posScale = dr["店铺大小"].ToString().Trim();

                            if (cols.Contains("物料支持"))
                                materialSupport = dr["物料支持"].ToString().Trim();
                            else if (cols.Contains("物料支持级别"))
                                materialSupport = dr["物料支持级别"].ToString().Trim();
                            else if (cols.Contains("店铺级别"))
                                materialSupport = dr["店铺级别"].ToString().Trim();


                            if (cols.Contains("费用金额"))
                                price = dr["费用金额"].ToString().Trim();
                            else if (cols.Contains("费用"))
                                price = dr["费用"].ToString().Trim();
                            else if (cols.Contains("应收费用"))
                                price = dr["应收费用"].ToString().Trim();
                            else if (cols.Contains("应收费用金额"))
                                price = dr["应收费用金额"].ToString().Trim();

                            if (cols.Contains("应付费用金额"))
                                payPrice = dr["应付费用金额"].ToString().Trim();
                            else if (cols.Contains("应付费用"))
                                payPrice = dr["应付费用"].ToString().Trim();

                            if (cols.Contains("M/W"))
                                gender = dr["M/W"].ToString().Trim();
                            else if (cols.Contains("Gender"))
                                gender = dr["Gender"].ToString().Trim();
                            else if (cols.Contains("性别"))
                                gender = dr["性别"].ToString().Trim();
                            else if (cols.Contains("男女"))
                                gender = dr["男女"].ToString().Trim();
                            else if (cols.Contains("男/女"))
                                gender = dr["男/女"].ToString().Trim();

                            if (cols.Contains("选图"))
                                chooseImg = dr["选图"].ToString().Trim();
                            else if (cols.Contains("系列/选图"))
                                chooseImg = dr["系列/选图"].ToString().Trim();

                            if (cols.Contains("POP位置"))
                                sheet = dr["POP位置"].ToString().Trim();
                            else if (cols.Contains("位置"))
                                sheet = dr["位置"].ToString().Trim();
                            else if (cols.Contains("Sheet"))
                                sheet = dr["Sheet"].ToString().Trim();

                            if (cols.Contains("POP位置明细"))
                                positionDescription = dr["POP位置明细"].ToString().Trim();
                            else if (cols.Contains("pop位置明细"))
                                positionDescription = dr["pop位置明细"].ToString().Trim();
                            else if (cols.Contains("位置明细"))
                                positionDescription = dr["位置明细"].ToString().Trim();
                            else if (cols.Contains("位置描述"))
                                positionDescription = dr["位置描述"].ToString().Trim();

                            //if (cols.Contains("级别"))
                            //    levelNum = dr["级别"].ToString().Trim();


                            if (cols.Contains("POP数量"))
                                num = dr["POP数量"].ToString().Trim();
                            else if (cols.Contains("数量"))
                                num = dr["数量"].ToString().Trim();
                            else if (cols.Contains("Quantity"))
                                num = dr["Quantity"].ToString().Trim();


                            if (cols.Contains("POP材质"))
                                material = dr["POP材质"].ToString().Trim();
                            else if (cols.Contains("材质"))
                                material = dr["材质"].ToString().Trim();


                            if (cols.Contains("POP宽"))
                                width = dr["POP宽"].ToString().Trim();
                            else if (cols.Contains("宽"))
                                width = dr["宽"].ToString().Trim();

                            if (cols.Contains("POP高"))
                                length = dr["POP高"].ToString().Trim();
                            else if (cols.Contains("高"))
                                length = dr["高"].ToString().Trim();




                            if (cols.Contains("备注"))
                                remark = dr["备注"].ToString().Trim();
                            else if (cols.Contains("其他备注"))
                                remark = dr["其他备注"].ToString().Trim();


                            bool canSave = true;

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
                                    if (shopFromDB.Format == null || shopFromDB.Format == "")
                                    {
                                        canSave = false;
                                        msg.Append("店铺类型为空；");
                                    }
                                    if (shopFromDB.IsInstall == null || shopFromDB.IsInstall == "")
                                    {
                                        canSave = false;
                                        msg.Append("安装级别为空；");
                                    }
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
                            int orderTypeIndex = 1;
                            bool isPriceOrder = false;
                            decimal area = 0;
                            decimal width1 = 0;
                            decimal length1 = 0;
                            if (!string.IsNullOrWhiteSpace(orderType))
                            {
                                OrderTypeEnum otEnum = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), orderType, true);
                                orderTypeIndex = (int)otEnum;
                                if (CommonMethod.GetEnumDescription<OrderTypeEnum>(orderTypeIndex.ToString()).Contains("费用订单"))
                                {
                                    isPriceOrder = true;
                                }
                            }
                            if (isPriceOrder)
                            {
                                if (string.IsNullOrWhiteSpace(price))
                                {
                                    canSave = false;
                                    msg.Append("应收费用金额 为空；");
                                }
                                else if (!StringHelper.IsDecimalVal(price))
                                {
                                    canSave = false;
                                    msg.Append("应收费用金额填写不正确；");
                                }
                                if (string.IsNullOrWhiteSpace(payPrice))
                                {
                                    canSave = false;
                                    msg.Append("应付费用金额 为空；");
                                }
                                else if (!StringHelper.IsDecimalVal(payPrice))
                                {
                                    canSave = false;
                                    msg.Append("应付费用金额填写不正确；");
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(sheet))
                                {
                                    canSave = false;
                                    msg.Append("位置 为空；");
                                }
                                sheet = sheet == "户外" ? "OOH" : sheet;
                                if (string.IsNullOrWhiteSpace(gender))
                                {
                                    canSave = false;
                                    msg.Append("性别 为空；");
                                }
                                if (string.IsNullOrWhiteSpace(num))
                                {
                                    canSave = false;
                                    msg.Append("数量 为空；");
                                }
                                else if (!StringHelper.IsIntVal(num))
                                {
                                    canSave = false;
                                    msg.Append("数量填写不正确；");
                                }
                                if (string.IsNullOrWhiteSpace(width))
                                {
                                    canSave = false;
                                    msg.Append("POP宽不能空；");
                                }
                                else if (!StringHelper.IsDecimalVal(width))
                                {
                                    canSave = false;
                                    msg.Append("POP宽填写不正确；");
                                }
                                else if (StringHelper.IsDecimal(width) == 0)
                                {
                                    canSave = false;
                                    msg.Append("POP宽必须大于0；");
                                }
                                if (string.IsNullOrWhiteSpace(length))
                                {
                                    canSave = false;
                                    msg.Append("POP高不能空；");
                                }
                                else if (!StringHelper.IsDecimalVal(length))
                                {
                                    canSave = false;
                                    msg.Append("POP高填写不正确；");
                                }
                                else if (StringHelper.IsDecimal(length) == 0)
                                {
                                    canSave = false;
                                    msg.Append("POP高必须大于0；");
                                }

                                
                                if (string.IsNullOrWhiteSpace(material))
                                {
                                    canSave = false;
                                    msg.Append("材质 为空；");
                                }
                                else if (!CheckMaterial(customerId,material))
                                {
                                    canSave = false;
                                    msg.Append("材质不存在；");
                                }
                                if (!string.IsNullOrWhiteSpace(width))
                                    width1 = StringHelper.IsDecimal(width);
                                if (!string.IsNullOrWhiteSpace(length))
                                    length1 = StringHelper.IsDecimal(length);
                                area = (width1 * length1) / 1000000;
                                //if (!CheckHandMakeOrder(shopId, sheet, width1, length1))
                                //{
                                //    canSave = false;
                                //    msg.Append("该尺寸的POP不存在，请更新数据库；");
                                //}
                            }





                            if (canSave)
                            {

                                orderDetailModel = new RegionOrderDetail();
                                orderDetailModel.OrderType = orderTypeIndex;
                                if (isPriceOrder)
                                {
                                    orderDetailModel.GraphicLength = 0;
                                    orderDetailModel.GraphicWidth = 0;
                                    orderDetailModel.Price = StringHelper.IsDecimal(price);
                                    orderDetailModel.PayPrice = StringHelper.IsDecimal(payPrice);
                                    orderDetailModel.Quantity = 1;
                                    orderDetailModel.Remark = remark;
                                    orderDetailModel.ShopId = shopId;
                                    orderDetailModel.SubjectId = subjectId;
                                    orderDetailModel.HandMakeSubjectId = 0;
                                    orderDetailModel.AddDate = DateTime.Now;
                                    orderDetailModel.AddUserId = CurrentUser.UserId;

                                    orderDetailBll.Add(orderDetailModel);
                                }
                                else
                                {
                                    //detailModel.Area = area;
                                    orderDetailModel.ChooseImg = chooseImg;
                                    orderDetailModel.Gender = gender;
                                    orderDetailModel.GraphicLength = length1;
                                    orderDetailModel.GraphicMaterial = material;
                                    orderDetailModel.GraphicWidth = width1;


                                    orderDetailModel.PositionDescription = positionDescription;
                                    orderDetailModel.Quantity = StringHelper.IsInt(num);
                                    orderDetailModel.Remark = remark;
                                    orderDetailModel.Sheet = sheet;
                                    orderDetailModel.ShopId = shopId;
                                    orderDetailModel.SubjectId = subjectId;
                                    orderDetailModel.HandMakeSubjectId = 0;

                                    orderDetailModel.MaterialSupport = materialSupport;
                                    orderDetailModel.POSScale = posScale;

                                    orderDetailModel.AddDate = DateTime.Now;
                                    orderDetailModel.AddUserId = CurrentUser.UserId;

                                    orderDetailBll.Add(orderDetailModel);
                                }
                                successNum++;
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
                        if (successNum > 0)
                        {

                            Subject model = subjectBll.GetModel(subjectId);
                            if (model != null)
                            {
                                model.ApproveState = 0;
                                subjectBll.Update(model);
                            }
                        }
                        Response.Redirect(string.Format("ImportOrderDetail.aspx?import=1&subjectId={0}&successNum={1}", subjectId, successNum), false);
                    }
                    else
                    {
                        Panel1.Visible = true;
                        labState.Text = "导入失败：表格里面没有数据！";
                    }
                }
            }
            #endregion
        }

        protected void lbDownLoadTemplate_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "NewShopOrderTemplate";
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


        Dictionary<string, Shop> shopDic = new Dictionary<string, Shop>();
        int guidanceType = 0;
        ShopBLL shopBll = new ShopBLL();
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
                    guidanceType = guidance.ActivityTypeId ?? 1;//活动类型：1安装，2发货，3促销
            }
            return flag;
        }

        
    }
}
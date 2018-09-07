using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.OleDb;
using System.Web.UI.WebControls;
using BLL;
using Common;
using DAL;
using Models;
using System.Configuration;
using System.Text;
using System.Transactions;

namespace WebApp.Subjects.SupplementByRegion
{
    public partial class ImportOrder : BasePage
    {
        int subjectId;
        SubjectBLL subjectBll = new SubjectBLL();
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
                if (Session["errorMaterialSupport"] != null)
                {
                    ErrorMaterialSupportWarning.Style.Add("display", "block");
                    hfHasErrorMaterialSupport.Value = "1";
                }
                else
                {
                    hfHasErrorMaterialSupport.Value = "";
                }
            }
            else
            {
                if (Session["errorMaterialSupport"] != null)
                {
                    ErrorMaterialSupportWarning.Style.Add("display", "block");
                    hfHasErrorMaterialSupport.Value = "1";
                }
                else
                {
                    hfHasErrorMaterialSupport.Value = "";
                }
            }
            if (!IsPostBack)
            {
                Subject model = subjectBll.GetModel(subjectId);
                if (model != null)
                {
                    int customerId=model.CustomerId??0;
                    hfCustomerId.Value = customerId.ToString();
                    hfGuidanceId.Value = (model.GuidanceId ?? 0).ToString();
                    labSubjectName.Text = model.SubjectName;
                    labSubjectNo.Text = model.SubjectNo;
                    int subjectType = model.SubjectType ?? 1;
                    labSubjectType.Text = CommonMethod.GeEnumName<SubjectTypeEnum>(subjectType.ToString());
                    if (model.IsSecondInstall ?? false)
                    {
                        labIsSecondInstall.Text = "是";
                        ViewState["IsSecondInstall"] = "1";
                    }
                    else
                    {
                        labIsSecondInstall.Text = "否";
                        ViewState["IsSecondInstall"] = "0";
                    }
               
                }
                BindOrder();
                DataTable errorTB = null;
                if (!CheckMaterialSupport(subjectId, out errorTB))
                {

                    if (errorTB != null && errorTB.Rows.Count > 0)
                    {
                        labState.Text = "警告：";
                        Session["errorMaterialSupport"] = errorTB;
                        hfHasErrorMaterialSupport.Value = "1";
                        Panel1.Visible = true;
                        ErrorMaterialSupportWarning.Style.Add("display", "block");
                    }
                    else
                    {
                        Panel1.Visible = false;
                        labState.Text = "导入完成";
                        Session["errorMaterialSupport"] = null;
                        hfHasErrorMaterialSupport.Value = "";
                    }
                }
            }
        }

       

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "RegionSupplementTemplate";
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

        /// <summary>
        /// 导出物料级别不统一警告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportErrorMaterialSupport_Click(object sender, EventArgs e)
        {
            if (Session["errorMaterialSupport"] != null)
            {
                DataTable dt = (DataTable)Session["errorMaterialSupport"];
                OperateFile.ExportExcel(dt, "物料级别不统一警告信息");
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        List<string> materialSupportList = new List<string>();
        RegionOrderDetailBLL detailBll = new RegionOrderDetailBLL();
        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                //RegionSupplementDetailBLL detailBll = new RegionSupplementDetailBLL();
                
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    
                    string MaterialSupportStr = string.Empty;
                    try
                    {
                        MaterialSupportStr = ConfigurationManager.AppSettings["MaterialSupport"];
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrWhiteSpace(MaterialSupportStr))
                    {
                        materialSupportList = StringHelper.ToStringList(MaterialSupportStr, '|', LowerUpperEnum.ToUpper);
                    }
                    if (!cbAdd.Checked)
                    {
                        detailBll.Delete(s => s.SubjectId == subjectId);
                        Subject model = subjectBll.GetModel(subjectId);
                        if (model != null)
                        {
                            model.Status = 1;
                            subjectBll.Update(model);
                        }
                    }
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
                        
                        RegionOrderDetail detailModel;
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        DataTable ErrorTB = CommonMethod.CreateErrorTB(cols);
                        int shopId = 0;
                        //项目名称
                        string subjectName = string.Empty;
                        //店铺编号
                        string shopNo = string.Empty;
                        //系列/选图
                        string chooseImg = string.Empty;
                        //性别
                        string gender = string.Empty;
                        //费用类型
                        string priceType = string.Empty;
                        //应收费用金额
                        string price = string.Empty;
                        //应付费用金额
                        string payPrice = string.Empty;

                        //pop位置
                        string sheet = string.Empty;
                        //器架名称
                        string machineFrame = string.Empty;
                        //数量
                        string num = string.Empty;
                        //pop材质
                        string material = string.Empty;
                        //pop宽
                        string width = string.Empty;
                        //pop高
                        string length = string.Empty;
                        //POP位置明细
                        string positionDescription = string.Empty;
                        //店铺大小
                        string posScale = string.Empty;
                        //店铺级别
                        string materialSupport = string.Empty;
                        //安装位置描述
                        string installPositionDescription = string.Empty;
                        //其他备注
                        string remark = string.Empty;
                        string outsourceName = string.Empty;
                        int outsourceId = 0;

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            shopId = 0;
                            //项目名称
                            subjectName = string.Empty;
                            //店铺编号
                            shopNo = string.Empty;
                            //系列/选图
                            chooseImg = string.Empty;
                            //性别
                            gender = string.Empty;
                            //费用类型
                            priceType = string.Empty;
                            //应收费用金额
                            price = string.Empty;
                            //应付费用金额
                            payPrice = string.Empty;

                            //pop位置
                            sheet = string.Empty;
                            //器架名称
                            machineFrame = string.Empty;
                            //数量
                            num = string.Empty;
                            //pop材质
                            material = string.Empty;
                            //pop宽
                            width = string.Empty;
                            //pop高
                            length = string.Empty;
                            //POP位置明细
                            positionDescription = string.Empty;
                            //店铺大小
                            posScale = string.Empty;
                            //店铺级别
                            materialSupport = string.Empty;
                            //安装位置描述
                            installPositionDescription = string.Empty;
                            //其他备注
                            remark = string.Empty;
                            outsourceName = string.Empty;
                            outsourceId = 0;


                            StringBuilder msg = new StringBuilder();
                            //是否特殊活动
                            //bool isSpecialSubject = false;
                            
                            if (cols.Contains("项目名称"))
                                subjectName = StringHelper.ReplaceSpecialChar(dr["项目名称"].ToString().Trim());
                            else if (cols.Contains("项目"))
                                subjectName = StringHelper.ReplaceSpecialChar(dr["项目"].ToString().Trim());
                            if (cols.Contains("店铺编号"))
                                shopNo = StringHelper.ReplaceSpecialChar(dr["店铺编号"].ToString().Trim());
                            else if (cols.Contains("POSCode"))
                                shopNo = StringHelper.ReplaceSpecialChar(dr["POSCode"].ToString().Trim());
                            else if (cols.Contains("POS Code"))
                                shopNo = StringHelper.ReplaceSpecialChar(dr["POS Code"].ToString().Trim());

                            if (cols.Contains("费用类型"))
                                priceType = StringHelper.ReplaceSpecialChar(dr["费用类型"].ToString().Trim());
                            if (cols.Contains("应收金额"))
                                price = StringHelper.ReplaceSpecialChar(dr["应收金额"].ToString().Trim());
                            else if (cols.Contains("应收费用"))
                                price = StringHelper.ReplaceSpecialChar(dr["应收费用"].ToString().Trim());
                            else if (cols.Contains("应收费用金额"))
                                price = StringHelper.ReplaceSpecialChar(dr["应收费用金额"].ToString().Trim());

                            if (cols.Contains("应付费用金额"))
                                payPrice = StringHelper.ReplaceSpecialChar(dr["应付费用金额"].ToString().Trim());
                            else if (cols.Contains("应付费用"))
                                payPrice = StringHelper.ReplaceSpecialChar(dr["应付费用"].ToString().Trim());
                            else if (cols.Contains("应付金额"))
                                payPrice = StringHelper.ReplaceSpecialChar(dr["应付金额"].ToString().Trim());

                            if (cols.Contains("POP位置"))
                                sheet = StringHelper.ReplaceSpecialChar(dr["POP位置"].ToString().Trim());
                            else if (cols.Contains("位置"))
                                sheet = StringHelper.ReplaceSpecialChar(dr["位置"].ToString().Trim());
                            else if (cols.Contains("Sheet"))
                                sheet = StringHelper.ReplaceSpecialChar(dr["Sheet"].ToString().Trim());

                            if (cols.Contains("器架名称"))
                                machineFrame = StringHelper.ReplaceSpecialChar(dr["器架名称"].ToString().Trim());
                            else if (cols.Contains("器架"))
                                machineFrame = StringHelper.ReplaceSpecialChar(dr["器架"].ToString().Trim());

                            if (cols.Contains("M/W"))
                                gender = StringHelper.ReplaceSpecialChar(dr["M/W"].ToString().Trim());
                            else if (cols.Contains("Gender"))
                                gender = StringHelper.ReplaceSpecialChar(dr["Gender"].ToString().Trim());
                            else if (cols.Contains("性别"))
                                gender = StringHelper.ReplaceSpecialChar(dr["性别"].ToString().Trim());
                            else if (cols.Contains("男女"))
                                gender = StringHelper.ReplaceSpecialChar(dr["男女"].ToString().Trim());
                            else if (cols.Contains("男/女"))
                                gender = StringHelper.ReplaceSpecialChar(dr["男/女"].ToString().Trim());

                            if (cols.Contains("POP数量"))
                                num = StringHelper.ReplaceSpecialChar(dr["POP数量"].ToString().Trim());
                            else if (cols.Contains("数量"))
                                num = StringHelper.ReplaceSpecialChar(dr["数量"].ToString().Trim());
                            else if (cols.Contains("Quantity"))
                                num = StringHelper.ReplaceSpecialChar(dr["Quantity"].ToString().Trim());

                            if (cols.Contains("POP宽"))
                                width = StringHelper.ReplaceSpecialChar(dr["POP宽"].ToString().Trim());
                            else if (cols.Contains("宽"))
                                width = StringHelper.ReplaceSpecialChar(dr["宽"].ToString().Trim());

                            if (cols.Contains("POP高"))
                                length = StringHelper.ReplaceSpecialChar(dr["POP高"].ToString().Trim());
                            else if (cols.Contains("高"))
                                length = StringHelper.ReplaceSpecialChar(dr["高"].ToString().Trim());

                            if (cols.Contains("POP材质"))
                                material = StringHelper.ReplaceSpecialChar(dr["POP材质"].ToString().Trim());
                            else if (cols.Contains("材质"))
                                material = StringHelper.ReplaceSpecialChar(dr["材质"].ToString().Trim());

                            if (cols.Contains("选图"))
                                chooseImg = StringHelper.ReplaceSpecialChar(dr["选图"].ToString().Trim());
                            else if (cols.Contains("系列/选图"))
                                chooseImg = StringHelper.ReplaceSpecialChar(dr["系列/选图"].ToString().Trim());

                            if (cols.Contains("POP位置明细"))
                                positionDescription = StringHelper.ReplaceSpecialChar(dr["POP位置明细"].ToString().Trim());
                            else if (cols.Contains("pop位置明细"))
                                positionDescription = StringHelper.ReplaceSpecialChar(dr["pop位置明细"].ToString().Trim());
                            else if (cols.Contains("位置明细"))
                                positionDescription = StringHelper.ReplaceSpecialChar(dr["位置明细"].ToString().Trim());
                            else if (cols.Contains("位置描述"))
                                positionDescription = StringHelper.ReplaceSpecialChar(dr["位置描述"].ToString().Trim());

                            if (cols.Contains("备注"))
                                remark = StringHelper.ReplaceSpecialChar(dr["备注"].ToString().Trim());
                            else if (cols.Contains("其他备注"))
                                remark = StringHelper.ReplaceSpecialChar(dr["其他备注"].ToString().Trim());

                            if (cols.Contains("物料支持"))
                                materialSupport = StringHelper.ReplaceSpecialChar(dr["物料支持"].ToString().Trim());
                            else if (cols.Contains("物料支持级别"))
                                materialSupport = StringHelper.ReplaceSpecialChar(dr["物料支持级别"].ToString().Trim());
                            else if (cols.Contains("店铺级别"))
                                materialSupport = StringHelper.ReplaceSpecialChar(dr["店铺级别"].ToString().Trim());

                            if (cols.Contains("店铺规模大小"))
                                posScale = StringHelper.ReplaceSpecialChar(dr["店铺规模大小"].ToString().Trim());
                            else if (cols.Contains("店铺规模"))
                                posScale = StringHelper.ReplaceSpecialChar(dr["店铺规模"].ToString().Trim());
                            else if (cols.Contains("店铺大小"))
                                posScale = StringHelper.ReplaceSpecialChar(dr["店铺大小"].ToString().Trim());

                            if (cols.Contains("安装位置描述"))
                                installPositionDescription = StringHelper.ReplaceSpecialChar(dr["安装位置描述"].ToString().Trim());

                            if (cols.Contains("外协"))
                                outsourceName = StringHelper.ReplaceSpecialChar(dr["外协"].ToString().Trim());
                            else if (cols.Contains("外协名称"))
                                outsourceName = StringHelper.ReplaceSpecialChar(dr["外协名称"].ToString().Trim());

                            bool canSave = true;
                            //int supplementSubjectId = 0;
                            Subject supplementSubject = null;
                            if (string.IsNullOrWhiteSpace(subjectName))
                            {
                                canSave = false;
                                msg.Append("项目名称 为空；");
                            }
                            else if (!CheckSubject(subjectId,subjectName, out supplementSubject))
                            {
                                canSave = false;
                                msg.Append("项目名称填写不正确；");
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
                                    if (guidanceType != (int)GuidanceTypeEnum.Promotion)//不是促销
                                    {
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

                            if (!string.IsNullOrWhiteSpace(priceType))
                            {
                                OrderTypeEnum otEnum = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), priceType, true);
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
                                    msg.Append("费用金额 为空；");
                                }
                                else if (!StringHelper.IsDecimalVal(price))
                                {
                                    canSave = false;
                                    msg.Append("费用金额填写不正确；");
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
                                if (guidanceType == (int)GuidanceTypeEnum.Install)
                                {

                                    if (string.IsNullOrWhiteSpace(materialSupport))
                                    {
                                        canSave = false;
                                        msg.Append("物料支持级别 为空；");
                                    }
                                    else if (!materialSupportList.Contains(materialSupport.ToUpper()))
                                    {
                                        canSave = false;
                                        msg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                                    }
                                }
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

                                //decimal materialPrice = 0;
                                if (string.IsNullOrWhiteSpace(material))
                                {
                                    canSave = false;
                                    msg.Append("材质 为空；");
                                }
                                else if (!CheckMaterial(material))
                                {
                                    canSave = false;
                                    msg.Append("材质不存在；");
                                }

                                if (!string.IsNullOrWhiteSpace(machineFrame) && !string.IsNullOrWhiteSpace(gender) && !string.IsNullOrWhiteSpace(sheet))
                                {
                                    if (!CheckShopMachineFrame(shopId, sheet, machineFrame, gender))
                                    {
                                        canSave = false;
                                        msg.Append("此店铺不含该器架；");
                                    }
                                }
                                
                                if (!string.IsNullOrWhiteSpace(width))
                                    width1 = StringHelper.IsDecimal(width);
                                if (!string.IsNullOrWhiteSpace(length))
                                    length1 = StringHelper.IsDecimal(length);
                                area = (width1 * length1) / 1000000;
                                if (guidanceType != (int)GuidanceTypeEnum.Promotion)
                                {

                                    if (shopFromDB != null && shopFromDB.IsInstall != null && shopFromDB.IsInstall.ToLower() == "y" && sheet.ToLower().Contains("ooh"))
                                    {
                                        if (!CheckOOH(shopId, "", width1, length1))
                                        {
                                            canSave = false;
                                            msg.Append("该尺寸的OOH POP不存在，请更新数据库；");
                                        }
                                    }
                                }
                                //暂时注释掉，
                                //if (supplementSubject != null && (supplementSubject.SubjectItemType ?? 1) == (int)SubjectItemTypeEnum.Supplement)
                                //{ 
                                //   //如果是上海增补（特殊活动），就检查增补项目位置是否存在
                                //    if (!CheckSpecialSubjectSheet(supplementSubject.SupplementSubjectId??0, sheet))
                                //    {
                                //        canSave = false;
                                //        msg.Append("活动项目不含该增补位置；");
                                //    }
                                //}

                            }

                            if (!string.IsNullOrWhiteSpace(outsourceName))
                            {
                                if (!GetOutsourceName(outsourceName, out outsourceId))
                                {
                                    canSave = false;
                                    msg.Append("外协不存在；");
                                }
                            }
                            if (canSave)
                            {

                               
                                detailModel = new RegionOrderDetail();
                                detailModel.OrderType = orderTypeIndex;
                                if (isPriceOrder && StringHelper.IsDecimal(price) > 0)
                                {
                                    if (string.IsNullOrWhiteSpace(payPrice))
                                    {
                                        payPrice = price;
                                    }
                                    detailModel.Sheet = string.Empty;
                                    detailModel.GraphicMaterial = string.Empty;
                                    detailModel.GraphicLength = 0;
                                    detailModel.GraphicWidth = 0;
                                    detailModel.Price = StringHelper.IsDecimal(price);
                                    detailModel.PayPrice = StringHelper.IsDecimal(payPrice);
                                    detailModel.Quantity = 1;
                                    detailModel.Remark =remark;
                                    detailModel.ShopId = shopId;
                                    detailModel.SubjectId = subjectId;
                                    detailModel.HandMakeSubjectId = supplementSubject.Id;
                                    detailModel.AddDate = DateTime.Now;
                                    detailModel.AddUserId = CurrentUser.UserId;
                                    detailModel.MaterialSupport = string.Empty;
                                    detailModel.IsSubmit = 1;
                                    detailModel.OutsourceId = outsourceId;
                                    detailBll.Add(detailModel);
                                }
                                else
                                {
                                    detailModel.ChooseImg = chooseImg;
                                    detailModel.Gender = gender;
                                    detailModel.GraphicLength = length1;
                                    detailModel.GraphicMaterial = material;
                                    detailModel.GraphicWidth = width1;
                                    detailModel.PositionDescription = positionDescription;
                                    detailModel.Quantity = int.Parse(num);
                                    detailModel.Remark = remark;
                                    detailModel.Sheet = sheet;
                                    detailModel.ShopId = shopId;
                                    detailModel.SubjectId = subjectId;
                                    detailModel.HandMakeSubjectId = supplementSubject.Id;
                                    detailModel.MaterialSupport = materialSupport;
                                    detailModel.POSScale = posScale;
                                    detailModel.MachineFrame = machineFrame;
                                    detailModel.AddDate = DateTime.Now;
                                    detailModel.AddUserId = CurrentUser.UserId;
                                    detailModel.IsSubmit = 1;
                                    detailModel.OutsourceId = outsourceId;
                                    detailBll.Add(detailModel);
                                }
                                successNum++;
                            }
                            else
                            {

                                DataRow dr1 = ErrorTB.NewRow();
                                for (int ii = 0; ii < cols.Count; ii++)
                                {
                                    dr1["" + cols[ii] + ""] = dr[cols[ii]];
                                }
                                dr1["错误信息"] = msg.ToString();
                                ErrorTB.Rows.Add(dr1);
                            }
                        }
                        if (ErrorTB.Rows.Count > 0)
                        {
                            Session["errorTb"] = ErrorTB;
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
                                model.Status = 2;
                                model.ApproveState = 0;
                                subjectBll.Update(model);
                            }
                        }
                        //检查物料支持级别是否统一名称
                        DataTable errorMaterialSupportTB = null;
                        if (!CheckMaterialSupport(subjectId, out errorMaterialSupportTB))
                        {
                            if (errorMaterialSupportTB != null && errorMaterialSupportTB.Rows.Count > 0)
                            {
                                Session["errorMaterialSupport"] = errorMaterialSupportTB;
                            }
                            else
                                Session["errorMaterialSupport"] = null;
                        }
                        else
                            Session["errorMaterialSupport"] = null;
                        Response.Redirect(string.Format("ImportOrder.aspx?import=1&subjectId={0}", subjectId), false);
                    }
                    else
                    {
                        Panel1.Visible = true;
                        labState.Text = "导入失败：表格里面没有数据！";
                    }
                }

            }
        }

        /*
        DataTable POPErrorTB = new DataTable();
        DataTable PriceOrderErrorTB = new DataTable();
        void SavePOP(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                RegionOrderDetailBLL detailBll = new RegionOrderDetailBLL();
                RegionOrderDetail detailModel;
                DataColumnCollection cols = ds.Tables[0].Columns;
                POPErrorTB = CommonMethod.CreateErrorTB(cols);
                int shopId = 0;
                //项目名称
                string subjectName = string.Empty;
                //店铺编号
                string shopNo = string.Empty;
                //系列/选图
                string chooseImg = string.Empty;
                //性别
                string gender = string.Empty;

                //pop位置
                string sheet = string.Empty;
                //器架名称
                string machineFrame = string.Empty;
                //数量
                string num = string.Empty;
                //pop材质
                string material = string.Empty;
                //pop宽
                string width = string.Empty;
                //pop高
                string length = string.Empty;
                //POP位置明细
                string positionDescription = string.Empty;
                //店铺大小
                string posScale = string.Empty;
                //店铺级别
                string materialSupport = string.Empty;
                //安装位置描述
                string installPositionDescription = string.Empty;
                //其他备注
                string remark = string.Empty;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StringBuilder msg = new StringBuilder();

                    if (cols.Contains("项目名称"))
                        subjectName = dr["项目名称"].ToString().Trim();
                    else if (cols.Contains("项目"))
                        subjectName = dr["项目"].ToString().Trim();
                    if (cols.Contains("店铺编号"))
                        shopNo = dr["店铺编号"].ToString().Trim();
                    else if (cols.Contains("POSCode"))
                        shopNo = dr["POSCode"].ToString().Trim();
                    else if (cols.Contains("POS Code"))
                        shopNo = dr["POS Code"].ToString().Trim();


                    if (cols.Contains("POP位置"))
                        sheet = dr["POP位置"].ToString().Trim();
                    else if (cols.Contains("位置"))
                        sheet = dr["位置"].ToString().Trim();
                    else if (cols.Contains("Sheet"))
                        sheet = dr["Sheet"].ToString().Trim();

                    if (cols.Contains("器架名称"))
                        machineFrame = dr["器架名称"].ToString().Trim();
                    else if (cols.Contains("器架"))
                        machineFrame = dr["器架"].ToString().Trim();

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

                    if (cols.Contains("POP数量"))
                        num = dr["POP数量"].ToString().Trim();
                    else if (cols.Contains("数量"))
                        num = dr["数量"].ToString().Trim();
                    else if (cols.Contains("Quantity"))
                        num = dr["Quantity"].ToString().Trim();

                    if (cols.Contains("POP宽"))
                        width = dr["POP宽"].ToString().Trim();
                    else if (cols.Contains("宽"))
                        width = dr["宽"].ToString().Trim();

                    if (cols.Contains("POP高"))
                        length = dr["POP高"].ToString().Trim();
                    else if (cols.Contains("高"))
                        length = dr["高"].ToString().Trim();

                    if (cols.Contains("POP材质"))
                        material = dr["POP材质"].ToString().Trim();
                    else if (cols.Contains("材质"))
                        material = dr["材质"].ToString().Trim();

                    if (cols.Contains("选图"))
                        chooseImg = dr["选图"].ToString().Trim();
                    else if (cols.Contains("系列/选图"))
                        chooseImg = dr["系列/选图"].ToString().Trim();

                    if (cols.Contains("POP位置明细"))
                        positionDescription = dr["POP位置明细"].ToString().Trim();
                    else if (cols.Contains("pop位置明细"))
                        positionDescription = dr["pop位置明细"].ToString().Trim();
                    else if (cols.Contains("位置明细"))
                        positionDescription = dr["位置明细"].ToString().Trim();
                    else if (cols.Contains("位置描述"))
                        positionDescription = dr["位置描述"].ToString().Trim();

                    if (cols.Contains("备注"))
                        remark = dr["备注"].ToString().Trim();
                    else if (cols.Contains("其他备注"))
                        remark = dr["其他备注"].ToString().Trim();

                    if (cols.Contains("物料支持"))
                        materialSupport = dr["物料支持"].ToString().Trim();
                    else if (cols.Contains("物料支持级别"))
                        materialSupport = dr["物料支持级别"].ToString().Trim();
                    else if (cols.Contains("店铺级别"))
                        materialSupport = dr["店铺级别"].ToString().Trim();

                    if (cols.Contains("店铺规模大小"))
                        posScale = dr["店铺规模大小"].ToString().Trim();
                    else if (cols.Contains("店铺规模"))
                        posScale = dr["店铺规模"].ToString().Trim();
                    else if (cols.Contains("店铺大小"))
                        posScale = dr["店铺大小"].ToString().Trim();

                    if (cols.Contains("安装位置描述"))
                        installPositionDescription = dr["安装位置描述"].ToString().Trim();

                    bool canSave = true;
                    int supplementSubjectId = 0;
                    if (string.IsNullOrWhiteSpace(subjectName))
                    {
                        canSave = false;
                        msg.Append("项目名称 为空；");
                    }
                    else if (!CheckSubject(subjectName, out supplementSubjectId))
                    {
                        canSave = false;
                        msg.Append("项目名称填写不正确；");
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
                            if (guidanceType != (int)GuidanceTypeEnum.Promotion)//不是促销
                            {
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

                    if (guidanceType == (int)GuidanceTypeEnum.Install)
                    {

                        if (string.IsNullOrWhiteSpace(materialSupport))
                        {
                            canSave = false;
                            msg.Append("物料支持级别 为空；");
                        }
                        else if (!materialSupportList.Contains(materialSupport.ToUpper()))
                        {
                            canSave = false;
                            msg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                        }
                    }
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

                    //decimal materialPrice = 0;
                    if (string.IsNullOrWhiteSpace(material))
                    {
                        canSave = false;
                        msg.Append("材质 为空；");
                    }
                    else if (!CheckMaterial(material))
                    {
                        canSave = false;
                        msg.Append("材质不存在；");
                    }

                    if (!string.IsNullOrWhiteSpace(machineFrame) && !string.IsNullOrWhiteSpace(gender) && !string.IsNullOrWhiteSpace(sheet))
                    {
                        if (!CheckShopMachineFrame(shopId, sheet, machineFrame, gender))
                        {
                            canSave = false;
                            msg.Append("此店铺不含该器架；");
                        }
                    }
                    decimal area = 0;
                    decimal width1 = 0;
                    decimal length1 = 0;
                    if (!string.IsNullOrWhiteSpace(width))
                        width1 = StringHelper.IsDecimal(width);
                    if (!string.IsNullOrWhiteSpace(length))
                        length1 = StringHelper.IsDecimal(length);
                    area = (width1 * length1) / 1000000;
                    if (guidanceType != (int)GuidanceTypeEnum.Promotion)
                    {

                        if (shopFromDB != null && shopFromDB.IsInstall != null && shopFromDB.IsInstall.ToLower() == "y" && sheet.ToLower().Contains("ooh"))
                        {
                            if (!CheckOOH(shopId, "", width1, length1))
                            {
                                canSave = false;
                                msg.Append("该尺寸的POP不存在，请更新数据库；");
                            }
                        }
                    }
                    if (canSave)
                    {

                        detailModel = new RegionOrderDetail();
                        detailModel.ChooseImg = chooseImg;
                        detailModel.Gender = gender;
                        detailModel.GraphicLength = length1;
                        detailModel.GraphicMaterial = material;
                        detailModel.GraphicWidth = width1;
                        detailModel.OrderType = 1;
                        detailModel.PositionDescription = positionDescription;
                        detailModel.Quantity = int.Parse(num);
                        detailModel.Remark = remark;
                        detailModel.Sheet = sheet;
                        detailModel.ShopId = shopId;
                        detailModel.SubjectId = subjectId;
                        detailModel.HandMakeSubjectId = supplementSubjectId;

                        detailModel.MaterialSupport = materialSupport;
                        detailModel.POSScale = posScale;
                        //detailModel.Category = category;
                        //detailModel.SubjectName = subjectName;
                        //detailModel.ShopNo = shopNo;
                        //detailModel.PositionDescription = installPositionDescription;
                        detailModel.MachineFrame = machineFrame;
                        detailModel.AddDate = DateTime.Now;
                        detailModel.AddUserId = CurrentUser.UserId;
                        detailModel.IsSubmit = 1;
                        detailBll.Add(detailModel);
                        
                    }
                    else
                    {

                        DataRow dr1 = POPErrorTB.NewRow();
                        for (int ii = 0; ii < cols.Count; ii++)
                        {
                            dr1["" + cols[ii] + ""] = dr[cols[ii]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        POPErrorTB.Rows.Add(dr1);
                    }
                }

            }

        }
        PriceOrderDetailBLL priceOrderBll = new PriceOrderDetailBLL();
        void SavePriceOder(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataColumnCollection cols = ds.Tables[0].Columns;
                PriceOrderErrorTB = CommonMethod.CreateErrorTB(ds.Tables[0].Columns);

                DataTable dt1 = ds.Tables[0];

                PriceOrderDetail priceModel;
                List<string> orderTypeList = CommonMethod.GetEnumList<OrderTypeEnum>().Where(s => s.Desction.Contains("费用订单")).Select(s => s.Name).ToList();
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
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    StringBuilder msg = new StringBuilder();
                    int orderTypeIndex = 0;
                    bool canSave = true;
                    if (cols.Contains("费用类型"))
                        orderType = dt1.Rows[i]["费用类型"].ToString().Trim();

                    if (cols.Contains("店铺编号"))
                        shopNo = dt1.Rows[i]["店铺编号"].ToString().Trim();
                    else if (cols.Contains("POSCode"))
                        shopNo = dt1.Rows[i]["POSCode"].ToString().Trim();
                    else if (cols.Contains("POS Code"))
                        shopNo = dt1.Rows[i]["POS Code"].ToString().Trim();

                    if (cols.Contains("费用金额"))
                        price = dt1.Rows[i]["费用金额"].ToString().Trim();
                    else if (cols.Contains("费用"))
                        price = dt1.Rows[i]["费用"].ToString().Trim();
                    else if (cols.Contains("金额"))
                        price = dt1.Rows[i]["金额"].ToString().Trim();
                    else if (cols.Contains("应收金额"))
                        price = dt1.Rows[i]["应收金额"].ToString().Trim();
                    else if (cols.Contains("应收费用金额"))
                        price = dt1.Rows[i]["应收费用金额"].ToString().Trim();
                    else if (cols.Contains("应收费用"))
                        price = dt1.Rows[i]["应收费用"].ToString().Trim();

                    if (cols.Contains("应付费用金额"))
                        payPrice = dt1.Rows[i]["应付费用金额"].ToString().Trim();
                    else if (cols.Contains("应付费用"))
                        payPrice = dt1.Rows[i]["应付费用"].ToString().Trim();
                    else if (cols.Contains("应付金额"))
                        payPrice = dt1.Rows[i]["应付金额"].ToString().Trim();
                    if (cols.Contains("备注"))
                        remark = dt1.Rows[i]["备注"].ToString().Trim();
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

                    if (!string.IsNullOrWhiteSpace(payPrice) && !StringHelper.IsDecimalVal(payPrice))
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
                        priceModel.Contents = string.Empty;
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

                        DataRow dr1 = PriceOrderErrorTB.NewRow();
                        for (int ii = 0; ii < cols.Count; ii++)
                        {
                            dr1["" + cols[ii] + ""] = dt1.Rows[i][ii].ToString();
                        }
                        dr1["错误信息"] = msg.ToString();
                        PriceOrderErrorTB.Rows.Add(dr1);

                    }
                }
            }
        }
        */ 






        ShopBLL shopBll = new ShopBLL();
        Dictionary<string, int> shopNoList = new Dictionary<string, int>();
       
        bool CheckShop(string shopNo, out int shopId)
        {
            shopId = 0;
            //msg = string.Empty;
            bool flag = true;
            if (shopNoList.Keys.Contains(shopNo.ToUpper()))
            {
                shopId = shopNoList[shopNo.ToUpper()];
            }
            else
            {
                var shop = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).FirstOrDefault();
                if (shop != null)
                {
                    shopId = shop.Id;
                    if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                        shopNoList.Add(shopNo.ToUpper(), shopId);

                }
                else
                {
                    //shopId = AddShop(dr, out msg);
                    //if (shopId > 0)
                    //{
                    //    if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                    //        shopNoList.Add(shopNo.ToUpper(), shopId);
                    //}
                    //else
                    flag = false;
                }
            }
            return flag;
        }


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
                var shopModel = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo && (s.IsDelete==null || s.IsDelete==false)).FirstOrDefault();
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


       
        /* 
        Dictionary<string, int> subjectList = new Dictionary<string, int>();
        bool CheckSubject(string subjectName, out int supplementSubjectId)
        {
            supplementSubjectId = 0;
           
            subjectName = subjectName.Trim().ToLower();
            if (subjectList.Keys.Contains(subjectName))
            {
                supplementSubjectId = subjectList[subjectName];
            }
            else
            {
                int guidanceId = -1;
                Subject subjectModel = subjectBll.GetModel(subjectId);
                if (subjectModel != null)
                    guidanceId = subjectModel.GuidanceId ?? -1;
                var model = subjectBll.GetList(s => s.SubjectName.Trim().ToLower() == subjectName && (((s.SubjectType ?? 1) == (int)SubjectTypeEnum.POP订单) || ((s.SubjectType ?? 1) == (int)SubjectTypeEnum.正常单) || ((s.SubjectType ?? 1) == (int)SubjectTypeEnum.手工订单)) && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && s.GuidanceId == guidanceId).FirstOrDefault();
                if (model != null)
                {
                    if (ViewState["IsSecondInstall"] != null && ViewState["IsSecondInstall"].ToString() == "1")
                    {
                        //判断是否二次安装项目
                        if (model.IsSecondInstall ?? false)
                        {
                            supplementSubjectId = model.Id;
                        }
                    }
                    else if (!(model.IsSecondInstall ?? false))
                    {
                        supplementSubjectId = model.Id;
                    }

                }
                
            }
            return supplementSubjectId>0;
        }
        */
        //提交
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    
                    bool isOk = false;
                    string msg = string.Empty;
                    //var list = new RegionSupplementDetailBLL().GetList(s => s.ItemId == itemId);
                    var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                               join shop in CurrentContext.DbContext.Shop
                               on order.ShopId equals shop.Id
                               where order.SubjectId == subjectId
                               select new { 
                                  order,
                                  shop
                               }).ToList();
                    if (list.Any())
                    {
                        
                        Subject model = subjectBll.GetModel(subjectId);
                        if (model != null)
                        {
                            //list.ForEach(s =>
                            //{
                            //    s.order.ApproveState = 0;
                            //    detailBll.Update(s.order);
                            //});
                            model.ApproveState = 0;
                            model.Status = 4;
                            model.SubmitDate = DateTime.Now;
                            subjectBll.Update(model);
                            isOk = true;
                        }
                        else
                        {
                            msg = "提交失败";
                        }
                    }
                    else
                    {
                        msg = "提交失败：无订单数据！";
                    }
                    if (isOk)
                    {
                        tran.Complete();
                        Alert("提交成功！", "/Subjects/RegionSubject/List.aspx");
                    }
                    else
                    {
                        Alert(msg);
                    }
                }
                catch (Exception ex)
                {
                    Alert("提交失败："+ex.Message);
                }
            }
        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Subjects/RegionSubject/AddSubject.aspx?subjectId=" + subjectId);
        }

        void BindOrder()
        {
            var list = (from order in CurrentContext.DbContext.RegionOrderDetail
                       join shop in CurrentContext.DbContext.Shop
                       on order.ShopId equals shop.Id
                        join subject in CurrentContext.DbContext.Subject
                         on order.HandMakeSubjectId equals subject.Id into subjectTemp
                        from subject1 in subjectTemp.DefaultIfEmpty()
                        where order.SubjectId == subjectId
                       select new { 
                         shop,
                         order,
                         order.OrderType,
                         subject1.SubjectName
                       }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            orderList.DataSource = list.OrderBy(s=>s.order.OrderType).OrderByDescending(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            orderList.DataBind();
            Panel2.Visible = list.Any();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindOrder();
        }

        protected void orderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                object item = e.Item.DataItem;
                if (item != null)
                {
                    object orderTypeObj = item.GetType().GetProperty("OrderType").GetValue(item, null);
                    int orderType = orderTypeObj != null ? int.Parse(orderTypeObj.ToString()) : 1;
                    if (orderType > 1)
                    {
                        ((Label)e.Item.FindControl("labPriceType")).Text = CommonMethod.GeEnumName<OrderTypeEnum>(orderType.ToString());
                    }
                }
            }
        }


        

        


       
    }
}
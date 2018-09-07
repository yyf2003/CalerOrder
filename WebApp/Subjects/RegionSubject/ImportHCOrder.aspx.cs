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

namespace WebApp.Subjects.RegionSubject
{
    public partial class ImportHCOrder : BasePage
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

        protected void lbDownLoadTemplate_Click(object sender, EventArgs e)
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

        protected void btnImport_Click(object sender, EventArgs e)
        {
            #region
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    List<string> materialSupportList = new List<string>();
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
                        if (!cbKeep.Checked)
                        {
                            orderDetailBll.Delete(s => s.SubjectId == subjectId);
                        }
                        int shopId = 0;
                        string orderType = string.Empty;

                        //项目名称
                        string subjectName = string.Empty;
                        //店铺编号
                        string shopNo = string.Empty;
                        //系列/选图
                        string chooseImg = string.Empty;
                        //性别
                        string gender = string.Empty;
                        //string category = string.Empty;

                        //string levelNum = string.Empty;
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
                            StringBuilder msg = new StringBuilder();

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
                            bool isHc = false;
                            if (!string.IsNullOrWhiteSpace(shopNo) && GetShopFromDB(shopNo, ref shopFromDB))
                            {
                                if (shopFromDB != null)
                                {
                                    shopId = shopFromDB.Id;
                                    if (!string.IsNullOrWhiteSpace(shopFromDB.Format))
                                    {
                                        string format = shopFromDB.Format.ToUpper();
                                        if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE") || format.Contains("YA"))
                                        {
                                            isHc = true;
                                        }
                                    }
                                    if (!isHc)
                                    {
                                        canSave = false;
                                        msg.Append("非HC店铺；");
                                    }
                                    else
                                    {
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
                                //canSave = false;
                                msg.Append("数量为空:系统自动改成1；");
                                num = "1";
                            }
                            else if (!StringHelper.IsIntVal(num))
                            {
                                //canSave = false;
                                msg.Append("数量填写不正确:系统自动改成1；");
                                num = "1";
                            }
                            if (StringHelper.IsInt(num) < 1)
                            {
                                msg.Append("数量小于1:系统自动改成1；");
                                num = "1";
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
                                        msg.Append("该尺寸的OOH POP不存在，请更新数据库；");
                                    }
                                }
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

                                orderDetailModel = new RegionOrderDetail();

                                //detailModel.Area = area;
                                orderDetailModel.ChooseImg = chooseImg;
                                orderDetailModel.Gender = gender;
                                orderDetailModel.GraphicLength = length1;
                                orderDetailModel.GraphicMaterial = material;
                                orderDetailModel.GraphicWidth = width1;
                                orderDetailModel.OrderType = 1;
                                orderDetailModel.PositionDescription = positionDescription;
                                orderDetailModel.Quantity = int.Parse(num);
                                orderDetailModel.Remark = remark;
                                orderDetailModel.Sheet = sheet;
                                orderDetailModel.ShopId = shopId;
                                orderDetailModel.SubjectId = subjectId;
                                orderDetailModel.HandMakeSubjectId = supplementSubjectId;
                                orderDetailModel.MaterialSupport = materialSupport;
                                orderDetailModel.POSScale = posScale;
                                orderDetailModel.MachineFrame = machineFrame;
                                orderDetailModel.AddDate = DateTime.Now;
                                orderDetailModel.AddUserId = CurrentUser.UserId;
                                orderDetailModel.IsSubmit = 0;
                                orderDetailModel.OutsourceId = outsourceId;
                                orderDetailBll.Add(orderDetailModel);
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
                        Response.Redirect(string.Format("ImportHCOrder.aspx?import=1&subjectId={0}&successNum={1}", subjectId, successNum), false);
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


        Dictionary<string, int> subjectList = new Dictionary<string, int>();
        bool CheckSubject(string subjectName, out int supplementSubjectId)
        {
            supplementSubjectId = 0;
            bool flag = true;
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
                var model = subjectBll.GetList(s => s.SubjectName.Trim().ToLower() == subjectName && (((s.SubjectType ?? 1) == (int)SubjectTypeEnum.POP订单) || ((s.SubjectType ?? 1) == (int)SubjectTypeEnum.正常单)) && (s.IsDelete == null || s.IsDelete == false) && s.ApproveState == 1 && s.GuidanceId == guidanceId).FirstOrDefault();
                if (model != null)
                {
                    supplementSubjectId = model.Id;

                }
                else
                    flag = false;
            }
            return flag;
        }
    }
}
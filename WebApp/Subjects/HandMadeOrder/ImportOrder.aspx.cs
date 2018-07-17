using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using BLL;
using Common;
using DAL;
using Models;
using System.Transactions;
using WebApp.Subjects.ADOrders;


namespace WebApp.Subjects.HandMadeOrder
{
    public partial class ImportOrder : BasePage
    {
        int subjectId;
        int hasOrder;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["import"] != null)
            {
                SubjectBLL subjectBll = new SubjectBLL();
                Subject model = subjectBll.GetModel(subjectId);
                if (model != null)
                {
                    model.Status = 3;
                    subjectBll.Update(model);
                }
                Panel1.Visible = true;
                if (Session["failMsg"] != null)
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
                BindSubject();
                BindPOPList();
                BindHCList();
                btnSubmit.Enabled = hasOrder > 0;

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

        void BindSubject()
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join customer in CurrentContext.DbContext.Customer
                                on subject.CustomerId equals customer.Id
                                where subject.Id == subjectId
                                select new
                                {
                                    CustomerId = subject.CustomerId ?? 0,
                                    subject.SubjectName,
                                    subject.SubjectNo,
                                    customer.CustomerName,
                                    subject.Remark
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                hfCustomerId.Value = subjectModel.CustomerId.ToString();
                labSubjectName.Text = subjectModel.SubjectName;
                labSubjectNo.Text = subjectModel.SubjectNo;
                labCustomer.Text = subjectModel.CustomerName;
                labRemark.Text = subjectModel.Remark;

            }
        }

        void BindPOPList()
        {
            var list = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            shop
                        }).ToList();

            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            popList.DataSource = list.OrderBy(s => s.order.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            popList.DataBind();
            if (list.Any())
                hasOrder++;
        }

        void BindHCList()
        {
            var list = (from order in CurrentContext.DbContext.HCOrderDetail
                        //join pop in CurrentContext.DbContext.POP
                        //on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop.ShopId, pop.GraphicNo, pop.Sheet }
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        where order.SubjectId == subjectId
                        select new
                        {
                            order,
                            //pop,
                            shop
                        }).ToList();
            string shopNo = txtShopNo2.Text.Trim();
            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                list = list.Where(s => s.shop.ShopNo.ToLower().Contains(shopNo.ToLower())).ToList();
            }
            AspNetPager2.RecordCount = list.Count;
            this.AspNetPager2.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager2.CurrentPageIndex, this.AspNetPager2.PageCount, this.AspNetPager2.RecordCount, this.AspNetPager2.PageSize });
            hcList.DataSource = list.OrderBy(s => s.order.ShopId).Skip((AspNetPager2.CurrentPageIndex - 1) * AspNetPager2.PageSize).Take(AspNetPager2.PageSize).ToList();

            hcList.DataBind();
            if (list.Any())
                hasOrder++;
        }

        List<ImportTables> importTableList = new List<ImportTables>();
        List<string> materialSupportList = new List<string>();
        protected void btnImport_Click(object sender, EventArgs e)
        {
            Session["failMsg"] = null;
            if (FileUpload1.HasFile)
            {
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
                    OleDbConnection conn;
                    OleDbDataAdapter da;
                    DataSet ds = null;
                    path = Server.MapPath(path);
                    string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                    ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                    conn = new OleDbConnection(ExcelConnStr);
                    conn.Open();
                    
                    DataTable dt = conn.GetSchema("Tables");
                    StringBuilder sheetMsg = new StringBuilder();
                    List<string> sheetList = new List<string>();
                    string[] sheets = new string[] { "pop$" };
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string sheetName = dt.Rows[i]["TABLE_NAME"].ToString();
                        if (sheetName.IndexOf("FilterDatabase") == -1 && sheets.Contains(sheetName.ToLower()))
                        {
                            if (!sheetList.Contains(sheetName.ToLower()))
                            {
                                sheetList.Add(sheetName.ToLower());
                            }
                            else
                            {
                                sheetMsg.Append("导入文件不能含有相同的Sheet!<br/>");
                                break;
                            }
                        }
                    }
                    if (sheetList.Count < 1)
                    {
                        sheetMsg.Append("导入文件必须含有pop表!<br/>");
                    }
                    if (sheetMsg.Length > 0)
                    {
                        Panel1.Visible = true;
                        labState.Text = "导入失败：" + sheetMsg.ToString();
                        return;

                    }
                    else
                    {
                        if (!cbAdd.Checked)
                        {
                            new HandMadeOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                            new HCOrderDetailBLL().Delete(s => s.SubjectId == subjectId);
                        }
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string sheetName = dt.Rows[i]["TABLE_NAME"].ToString();
                        if (sheetName.IndexOf("FilterDatabase") == -1)
                        {

                            string sql = "select * from [" + sheetName + "]";
                            da = new OleDbDataAdapter(sql, conn);
                            ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            if (sheetName.ToLower().Contains("pop"))
                            {
                                ImportTables tab = new ImportTables();
                                tab.TabName = "pop";
                                tab.Data = ds;
                                importTableList.Add(tab);
                                SavePOPOrder(ds);
                            }
                            //else if (sheetName.ToLower().Contains("hc"))
                            //{
                            //    ImportTables tab = new ImportTables();
                            //    tab.TabName = "hc";
                            //    tab.Data = ds;
                            //    importTableList.Add(tab);
                            //    SaveHCOrder(ds);
                            //}
                        }
                    }
                    conn.Dispose();
                    conn.Close();
                    CheckImport();
                    Response.Redirect(string.Format("ImportOrder.aspx?import=1&subjectId={0}", subjectId), false);
                }
            }
        }

        DataColumnCollection cols;
        DataTable POPErrorTB = new DataTable();
        DataTable HCErrorTB = new DataTable();
        void SavePOPOrder(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                HandMadeOrderDetailBLL handOrderBll = new HandMadeOrderDetailBLL();
                Dictionary<string, int> orderTypeDic = new Dictionary<string, int>();
                orderTypeDic.Add("pop", 1);
                orderTypeDic.Add("道具", 2);
                orderTypeDic.Add("物料", 3);

                string orderType = string.Empty;
                string shopNo = string.Empty;
                int shopId = 0;
                string sheet = string.Empty;
                string machineFrame = string.Empty;
                string levelNum = string.Empty;
                string gender = string.Empty;
                string num = string.Empty;
                string width = string.Empty;
                string length = string.Empty;
                string material = string.Empty;
                string chooseImg = string.Empty;
                string positionDescription = string.Empty;
                string remark = string.Empty;
                string operate = string.Empty;
                string posScale = string.Empty;
                string materialSupport = string.Empty;
                string category = string.Empty;
                string installPositionDescription = string.Empty;

                cols = ds.Tables[0].Columns;
                POPErrorTB = CommonMethod.CreateErrorTB(cols);
                HandMadeOrderDetail handOrderModel;
                //int successNum = 0;
                //int failNum = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StringBuilder msg = new StringBuilder();
                   
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


                    if (cols.Contains("系列"))
                        category = StringHelper.ReplaceSpecialChar(dr["系列"].ToString().Trim());
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

                    if (cols.Contains("操作"))
                        operate = dr["操作"].ToString().Trim();
                    bool canSave = true;
                    //decimal materialPrice = 0;
                   
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
                    
                    if (string.IsNullOrWhiteSpace(sheet))
                    {
                        canSave = false;
                        msg.Append("位置 为空；");
                    }
                    sheet = sheet == "户外" ? "OOH" : sheet;
                    if (sheet.ToLower() == "t-stand" || sheet.ToLower() == "tstand")
                    {
                        sheet = "中岛";
                    }
                    
                    
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
                   
                    bool IsHc = false;
                    string materialSupport1 = materialSupport;
                    if (shopFromDB != null && !string.IsNullOrWhiteSpace(shopFromDB.RegionName) && shopFromDB.RegionName.ToLower() == "north")
                    {
                       
                        if (guidanceType == (int)GuidanceTypeEnum.Install)//安装活动
                        {

                            materialSupport1 = GetShopMaterialSupport(guidanceId, shopId);
                            if (string.IsNullOrWhiteSpace(materialSupport1))
                            {
                                materialSupport1 = materialSupport;
                                if (string.IsNullOrWhiteSpace(materialSupport1))
                                {
                                    canSave = false;
                                    msg.Append("物料支持 为空；");
                                }
                                else if (!materialSupportList.Contains(materialSupport1.ToUpper()))
                                {
                                    canSave = false;
                                    msg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                                }
                            }
                            else
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
                        }
                    }
                    if (canSave)
                    {

                        handOrderModel = new HandMadeOrderDetail();
                        handOrderModel.AddDate = DateTime.Now;
                        handOrderModel.Area = area;
                        handOrderModel.ChooseImg = chooseImg;
                        handOrderModel.Gender = gender;
                        handOrderModel.GraphicLength = length1;
                        handOrderModel.GraphicMaterial = material;
                        handOrderModel.GraphicWidth = width1;
                        if (!string.IsNullOrWhiteSpace(levelNum))
                            handOrderModel.LevelNum = int.Parse(levelNum);
                        //handOrderModel.OrderType = orderTypeDic[orderType.ToLower()];
                        handOrderModel.OrderType = 1;
                        handOrderModel.PositionDescription = positionDescription;
                        handOrderModel.Quantity = int.Parse(num);
                        handOrderModel.Remark = remark;
                        handOrderModel.Sheet = sheet;
                        handOrderModel.ShopId = shopId;
                        handOrderModel.SubjectId = subjectId;
                        handOrderModel.Operation = operate;
                        handOrderModel.MaterialSupport = materialSupport1;
                        handOrderModel.Category = category;
                        handOrderModel.POSScale = posScale;
                        handOrderModel.InstallPositionDescription = installPositionDescription;
                        handOrderModel.MachineFrame = machineFrame;
                        handOrderModel.IsSubmit = 0;
                        handOrderModel.ApproveState = 0;
                        handOrderBll.Add(handOrderModel);

                    }
                    else
                    {
                        DataRow dr1 = POPErrorTB.NewRow();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            dr1["" + cols[i] + ""] = dr[cols[i]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        POPErrorTB.Rows.Add(dr1);

                    }
                }

            }
        }

        void SaveHCOrder(DataSet ds)
        {

            //if (!cbAdd.Checked)

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                HCOrderDetailBLL hcOrderBll = new HCOrderDetailBLL();

                HCOrderDetail hcOrderModel;
                cols = ds.Tables[0].Columns;
                HCErrorTB = CommonMethod.CreateErrorTB(cols);
                //Dictionary<string, int> orderTypeDic = new Dictionary<string, int>();
                //orderTypeDic.Add("pop", 1);
                //orderTypeDic.Add("道具", 2);
                //orderTypeDic.Add("物料", 3);
                //string orderType = string.Empty;
                string shopNo = string.Empty;
                string posScale = string.Empty;
                string materialSupport = string.Empty;
                int shopId = 0;
                string sheet = string.Empty;
                string machineFrame = string.Empty;
                string graphicNo = string.Empty;
                string levelNum = string.Empty;
                string gender = string.Empty;
                string num = string.Empty;
                string width = string.Empty;
                string length = string.Empty;
                string material = string.Empty;
                string chooseImg = string.Empty;
                string category = string.Empty;
                string positionDescription = string.Empty;
                string remark = string.Empty;
                string operate = string.Empty;
                string installPositionDescription = string.Empty;
                //bool isMaterialSupport = false;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StringBuilder msg = new StringBuilder();
                    //if (cols.Contains("订单类型"))
                    //{
                    //    orderType = dr["订单类型"].ToString().Trim();
                    //}
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


                    if (cols.Contains("级别"))
                        levelNum = dr["级别"].ToString().Trim();
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

                    if (cols.Contains("操作"))
                        operate = dr["操作"].ToString().Trim();
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
                    if (guidanceType == (int)GuidanceTypeEnum.Install)//安装活动
                    {
                        if (string.IsNullOrWhiteSpace(materialSupport))
                        {
                            canSave = false;
                            msg.Append("物料支持 为空；");
                        }
                        else if (!materialSupportList.Contains(materialSupport.ToUpper()))
                        {
                            canSave = false;
                            msg.Append("物料支持填写不正确，必须是Basic,Premium,MCS,VVIP四种之一；");
                        }
                    }
                    //string addShopMsg = string.Empty;
                    //bool isHC = false;
                    //if (!string.IsNullOrWhiteSpace(shopNo) && !CheckShop(shopNo, dr, out shopId, out isHC, out addShopMsg))
                    //{
                    //    canSave = false;

                    //    msg.Append("店铺不存在，添加失败：" + addShopMsg + "；");

                    //}
                    //if (string.IsNullOrWhiteSpace(materialSupport))
                    //{
                    //    canSave = false;
                    //    msg.Append("物料支持级别(店铺级别) 为空；");
                    //}
                    if (string.IsNullOrWhiteSpace(sheet))
                    {
                        canSave = false;
                        msg.Append("位置 为空；");
                    }
                    //if (string.IsNullOrWhiteSpace(graphicNo))
                    //{
                    //    canSave = false;
                    //    msg.Append("POP编号 为空；");
                    //}
                    sheet = sheet == "户外" ? "OOH" : sheet;
                    if (sheet.ToLower() == "t-stand" || sheet.ToLower() == "tstand")
                    {
                        sheet = "中岛";
                    }
                    
                    //if (IsInvalid == 0)//店铺有效（生产店铺）
                    //{
                    //    string realGender = string.Empty;
                    //    if (!string.IsNullOrWhiteSpace(shopNo) && !string.IsNullOrWhiteSpace(sheet) && !string.IsNullOrWhiteSpace(graphicNo) && !CheckGraphicNo(shopId, sheet, graphicNo, out realGender))
                    //    {
                    //        canSave = false;
                    //        msg.Append("此店铺不含有该pop，请更新基础数据；");
                    //    }
                    //}
                    //if (!string.IsNullOrWhiteSpace(levelNum) && !StringHelper.IsIntVal(levelNum))
                    //{
                    //    canSave = false;
                    //    msg.Append("级别填写不正确；");
                    //}
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

                    //if (!string.IsNullOrWhiteSpace(width) && !StringHelper.IsDecimalVal(width))
                    //{
                    //    canSave = false;
                    //    msg.Append("POP宽填写不正确；");
                    //}
                    //if (!string.IsNullOrWhiteSpace(length) && !StringHelper.IsDecimalVal(length))
                    //{
                    //    canSave = false;
                    //    msg.Append("POP高填写不正确；");
                    //}
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
                    //if (HCOrderIsExist(subjectId, shopId, sheet, gender, StringHelper.IsDecimal(width), StringHelper.IsDecimal(length)))
                    //{
                    //    canSave = false;
                    //    msg.Append("订单重复；");
                    //}
                    decimal area = 0;
                    decimal width1 = 0;
                    decimal length1 = 0;
                    if (!string.IsNullOrWhiteSpace(width))
                        width1 = StringHelper.IsDecimal(width);
                    if (!string.IsNullOrWhiteSpace(length))
                        length1 = StringHelper.IsDecimal(length);
                    area = (width1 * length1) / 1000000;
                    if (!CheckHandMakeOrder(shopId, sheet, width1, length1))
                    {
                        canSave = false;
                        msg.Append("此POP不存在,请更新数据库；");
                    }
                    if (canSave)
                    {

                        hcOrderModel = new HCOrderDetail();
                        hcOrderModel.AddDate = DateTime.Now;
                        hcOrderModel.Area = area;
                        hcOrderModel.ChooseImg = chooseImg;
                        hcOrderModel.Gender = gender;
                        hcOrderModel.GraphicLength = length1;
                        hcOrderModel.GraphicMaterial = material;
                        hcOrderModel.GraphicWidth = width1;
                        if (!string.IsNullOrWhiteSpace(levelNum))
                            hcOrderModel.LevelNum = int.Parse(levelNum);
                        hcOrderModel.OrderType = 1;
                        hcOrderModel.PositionDescription = positionDescription;
                        hcOrderModel.Quantity = int.Parse(num);
                        hcOrderModel.Remark = remark;
                        hcOrderModel.Sheet = sheet;
                        hcOrderModel.ShopId = shopId;
                        hcOrderModel.SubjectId = subjectId;
                        hcOrderModel.GraphicNo = graphicNo;
                        hcOrderModel.MaterialSupport = materialSupport;
                        hcOrderModel.POSScale = posScale;
                        hcOrderModel.Category = category;
                        hcOrderModel.Operation = operate;
                        hcOrderModel.InstallPositionDescription = installPositionDescription;
                        hcOrderModel.MachineFrame = machineFrame;
                        hcOrderBll.Add(hcOrderModel);
                        //if (materialSupportList.Contains(materialSupport.ToLower()))
                        //isMaterialSupport = true;
                    }
                    else
                    {

                        DataRow dr1 = HCErrorTB.NewRow();
                        for (int ii = 0; ii < cols.Count; ii++)
                        {
                            dr1["" + cols[ii] + ""] = dr[cols[ii]];
                        }
                        dr1["错误信息"] = msg.ToString();
                        HCErrorTB.Rows.Add(dr1);
                    }

                }
                //if (!isMaterialSupport)
                //{
                //    tipsList.Add("HC订单");
                //}
            }
        }

        /// <summary>
        /// 如果导入有失败的数据，就导出失败的信息
        /// </summary>
        void CheckImport()
        {
            bool isFail = false;
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();

            if (POPErrorTB.Rows.Count > 0 || HCErrorTB.Rows.Count > 0)
            {
                isFail = true;
                dic.Add("POP", POPErrorTB);
                dic.Add("HC", HCErrorTB);
            }

            if (isFail)
            {
                Session["failMsg"] = dic;
            }
            else
            {
                Session["failMsg"] = null;
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
        }


        /// <summary>
        /// 检查HC订单是否跟第一次下单重复
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="GraphicNo"></param>
        /// <returns></returns>
        List<FinalOrderDetailTemp> popDetailList = new List<FinalOrderDetailTemp>();
        bool HCOrderIsExist(int subjectId, int shopId, string sheet, string gender, decimal width, decimal length)
        {
            bool flag = false;
            if (!popDetailList.Any())
            {
                popDetailList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false));

            }
            if (popDetailList.Any())
            {
                var list = popDetailList.Where(s => s.ShopId == shopId && s.Sheet.ToLower() == sheet.ToLower() && (s.Gender == gender || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女"))) && s.GraphicWidth == width && s.GraphicLength == length);
                flag = list.Any();
            }
            return flag;
        }


        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "HandMadeOrderTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void lbExportError_Click(object sender, EventArgs e)
        {
            //if (Session["failMsg"] != null)
            //{
            //    DataTable dt = (DataTable)Session["failMsg"];
            //    OperateFile.ExportExcel(dt, "导入失败信息");
            //}
            if (Session["failMsg"] != null)
            {
                //List<DataTable> dts = (List<DataTable>)Session["failMsg"];
                Dictionary<string, DataTable> dts = (Dictionary<string, DataTable>)Session["failMsg"];
                OperateFile.ExportTables(dts, "导入失败信息");
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


        ShopBLL shopBll = new ShopBLL();
        Dictionary<string, int> shopNoList = new Dictionary<string, int>();
        List<string> CloseShopList = new List<string>();
        Dictionary<string, int> InvalidShops = new Dictionary<string, int>();
        List<string> HCShopList = new List<string>();
        bool CheckShop(string shopNo, DataRow dr, out int shopId,out bool IsHc, out string msg)
        {
            shopId = 0;
            IsHc = false;
            msg = string.Empty;
            bool flag = true;
            int guidanceType = 0;
            if (shopNoList.Keys.Contains(shopNo.ToUpper()))
            {
                shopId = shopNoList[shopNo.ToUpper()];
                if (HCShopList.Contains(shopNo.ToUpper()))
                    IsHc = true;
            }
            else
            {
                
                var shop = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).FirstOrDefault();
                if (shop != null)
                {
                    shopId = shop.Id;
                    if (!shopNoList.Keys.Contains(shopNo.ToUpper()))
                        shopNoList.Add(shopNo.ToUpper(), shopId);
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
                    if (guidanceType != 3)
                    {
                        if (!string.IsNullOrWhiteSpace(shop.Format))
                        {
                            //非促销的活动要检查是不是HC店铺，如果是，就不导入Homecourt
                            string format = shop.Format.ToUpper();

                            if (format.Contains("HC") || format.Contains("HOMECOURT") || format.Contains("HOMECORE"))
                            {
                                IsHc = true;
                                if (!HCShopList.Contains(shopNo.ToUpper()))
                                    HCShopList.Add(shopNo.ToUpper());
                            }
                        }
                        
                    }
                   

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
                    //    flag = false;
                    flag = false;
                }
            }
            return flag;
        }

        POPBLL popBll = new POPBLL();
        bool CheckGraphicNo(int shopId, string sheet, string graphicNo, out string realGender)
        {
            realGender = string.Empty;
            //return popBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet && s.GraphicNo.ToUpper() == graphicNo.Trim().ToUpper() && ((s.Gender=="")||(s.Gender==null)||(s.Gender == gender) || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女")))).Any();
            var model = popBll.GetList(s => s.ShopId == shopId && s.Sheet == sheet && s.GraphicNo.ToUpper() == graphicNo.Trim().ToUpper()).FirstOrDefault();
            if (model != null)
            {
                realGender = model.Gender;
                return true;
            }
            return false;

        }

       

        


        OrderMaterialMppingBLL OrderMaterialMppingBll = new OrderMaterialMppingBLL();
        List<string> materialStrList = new List<string>();
        bool CheckMaterial(string materialName)
        {

            bool flag = false;
            if (materialStrList.Contains(materialName.ToLower()))
            {
                flag = true;
            }
            else
            {
                int customerId = 0;
                if (!string.IsNullOrWhiteSpace(hfCustomerId.Value))
                    customerId = StringHelper.IsInt(hfCustomerId.Value);
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    var model = OrderMaterialMppingBll.GetList(s => s.OrderMaterialName.ToLower() == materialName.ToLower()).FirstOrDefault();
                    if (model != null)
                    {
                        flag = true;
                        materialStrList.Add(materialName.ToLower());
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
            return flag;
        }


        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    //int materialPriceItemId = 0;
                    string subjectCategoryName = string.Empty;
                    SubjectBLL subjectBll = new SubjectBLL();
                    Subject supplimentSubjectModel = subjectBll.GetModel(subjectId);
                    if (supplimentSubjectModel != null)
                    {
                        /*
                        FinalOrderDetailTempBLL orderBll = new FinalOrderDetailTempBLL();
                        FinalOrderDetailTemp orderModel;
                        //将原项目西区的订单全部删除
                        var oldOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                            join shop in CurrentContext.DbContext.Shop
                                            on order.ShopId equals shop.Id
                                            where shop.RegionName.ToLower() == "west"
                                            && order.SubjectId == supplimentSubjectModel.HandMakeSubjectId
                                            select order).ToList();
                        if (oldOrderList.Any())
                        {
                            List<int> orderIList = oldOrderList.Select(s => s.Id).ToList();
                            orderBll.Delete(s => orderIList.Contains(s.Id));
                        }

                       
                        orderBll.Delete(s => s.SubjectId == subjectId);

                        

                        //pop手工单
                        var handOrderList = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                                             join subject in CurrentContext.DbContext.Subject
                                             on order.SubjectId equals subject.Id
                                             join shop in CurrentContext.DbContext.Shop
                                             on order.ShopId equals shop.Id
                                             join guidance in CurrentContext.DbContext.SubjectGuidance
                                             on subject.GuidanceId equals guidance.ItemId
                                             join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                             on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                                             from subjectCategory in categoryTemp.DefaultIfEmpty()
                                             where order.SubjectId == subjectId
                                             select new
                                             {
                                                 order,
                                                 subject,
                                                 shop,
                                                 guidance.PriceItemId,
                                                 subjectCategory
                                             }).ToList();
                        if (handOrderList.Any())
                        {
                            materialPriceItemId = handOrderList[0].PriceItemId ?? 0;
                            subjectCategoryName = handOrderList[0].subjectCategory != null ? handOrderList[0].subjectCategory.CategoryName : string.Empty;
                            
                            handOrderList.ForEach(s =>
                            {
                                if (!string.IsNullOrWhiteSpace(s.order.Operation) && s.order.Operation.Contains("删除"))
                                {

                                    //orderBll.Delete(o => o.ShopId == s.order.ShopId && o.Sheet.ToLower() == s.order.Sheet.ToLower() && o.Gender == s.order.Gender && o.GraphicLength == s.order.GraphicLength && o.GraphicWidth == s.order.GraphicWidth && o.SubjectId == s.subject.HandMakeSubjectId);

                                    //查找同一项目的所有补单
                                    List<int> handOrderIdList = subjectBll.GetList(sub => sub.HandMakeSubjectId == s.subject.HandMakeSubjectId).Select(sub => sub.Id).ToList();
                                    handOrderIdList.Add(s.subject.HandMakeSubjectId ?? 0);
                                    if (handOrderIdList.Any())
                                    {
                                        orderBll.Delete(o => o.ShopId == s.order.ShopId && o.Sheet.ToLower() == s.order.Sheet.ToLower() && o.Gender == s.order.Gender && o.GraphicLength == s.order.GraphicLength && o.GraphicWidth == s.order.GraphicWidth && handOrderIdList.Contains(o.SubjectId ?? 0));

                                    }
                                }
                                else
                                {
                                    orderModel = new FinalOrderDetailTemp();
                                    //orderModel.Area = s.order.Area;
                                    orderModel.ChooseImg = s.order.ChooseImg;
                                    orderModel.Gender = s.order.Gender;
                                    orderModel.GraphicLength = s.order.GraphicLength;
                                    orderModel.GraphicMaterial = s.order.GraphicMaterial;
                                    orderModel.GraphicWidth = s.order.GraphicWidth;
                                    orderModel.LevelNum = s.order.LevelNum;
                                    orderModel.OrderType = s.order.OrderType;
                                    orderModel.PositionDescription = s.order.PositionDescription;
                                    orderModel.Quantity = s.order.Quantity;
                                    orderModel.Remark = s.order.Remark;
                                    orderModel.Sheet = s.order.Sheet;
                                    orderModel.ShopId = s.order.ShopId;
                                    orderModel.ShopNo = s.shop.ShopNo;

                                    orderModel.ShopName = s.shop.ShopName;
                                    orderModel.Region = s.shop.RegionName;
                                    orderModel.Province = s.shop.ProvinceName;
                                    orderModel.City = s.shop.CityName;
                                    orderModel.CityTier = s.shop.CityTier;
                                    orderModel.IsInstall = s.shop.IsInstall;
                                    orderModel.AgentCode = s.shop.AgentCode;
                                    orderModel.AgentName = s.shop.AgentName;
                                    orderModel.POPAddress = s.shop.POPAddress;
                                    orderModel.Contact = s.shop.Contact1;
                                    orderModel.Tel = s.shop.Tel1;
                                    orderModel.Channel = s.shop.Channel;

                                    orderModel.Format = s.shop.Format;
                                    orderModel.LocationType = s.shop.LocationType;
                                    orderModel.BusinessModel = s.shop.BusinessModel;
                                    


                                    orderModel.SubjectId = subjectId;
                                    orderModel.IsPOPMaterial = 1;
                                    orderModel.MaterialSupport = s.order.MaterialSupport;
                                    
                                    decimal width = s.order.GraphicWidth ?? 0;
                                    decimal length = s.order.GraphicLength ?? 0;
                                    orderModel.Area = (width * length) / 1000000;
                                    orderModel.POSScale = s.order.POSScale;
                                    orderModel.InstallPricePOSScale = s.order.POSScale;
                                    orderModel.InstallPriceMaterialSupport = s.order.MaterialSupport;
                                    //if (!string.IsNullOrWhiteSpace(s.shop.Format) && (s.shop.Format.ToLower().Contains("kids") || s.shop.Format.ToLower().Contains("infant")))
                                    if (subjectCategoryName=="童店") 
                                        orderModel.InstallPriceMaterialSupport = "Others";
                                    orderModel.InstallPositionDescription = s.order.InstallPositionDescription;
                                    orderModel.MachineFrame = s.order.MachineFrame;

                                    decimal unitPrice = 0;
                                    decimal totalPrice = 0;
                                    if (!string.IsNullOrWhiteSpace(s.order.GraphicMaterial))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = s.order.GraphicMaterial;
                                        pop.GraphicLength = s.order.GraphicLength;
                                        pop.GraphicWidth = s.order.GraphicWidth;
                                        pop.Quantity = s.order.Quantity;
                                        pop.PriceItemId = materialPriceItemId;
                                        unitPrice = GetMaterialUnitPrice(pop, out totalPrice);
                                    }
                                    orderModel.UnitPrice = unitPrice;
                                    orderModel.TotalPrice = totalPrice;
                                        

                                    orderBll.Add(orderModel);
                                }
                            });
                        }
                        //HC订单
                        var list = (from order in CurrentContext.DbContext.HCOrderDetail
                                    //join pop in CurrentContext.DbContext.POP
                                    //on new { order.ShopId, order.GraphicNo, order.Sheet } equals new { pop.ShopId, pop.GraphicNo, pop.Sheet }
                                    join shop in CurrentContext.DbContext.Shop
                                    on order.ShopId equals shop.Id
                                    join subject in CurrentContext.DbContext.Subject
                                    on order.SubjectId equals subject.Id
                                    join guidance in CurrentContext.DbContext.SubjectGuidance
                                    on subject.GuidanceId equals guidance.ItemId
                                    join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                     on subject.SubjectCategoryId equals subjectCategory1.Id into categoryTemp
                                    from subjectCategory in categoryTemp.DefaultIfEmpty()
                                    where order.SubjectId == subjectId
                                    select new
                                    {
                                        subject,
                                        order,
                                        shop,
                                        guidance.PriceItemId,
                                        subjectCategory
                                    }).ToList();
                        if (list.Any())
                        {

                            materialPriceItemId = list[0].PriceItemId ?? 0;
                            subjectCategoryName = handOrderList[0].subjectCategory != null ? handOrderList[0].subjectCategory.CategoryName : string.Empty;
                            list.ForEach(o =>
                            {
                                if (!string.IsNullOrWhiteSpace(o.order.Operation) && o.order.Operation.Contains("删除"))
                                {
                                    
                                    List<int> handOrderIdList = subjectBll.GetList(sub => sub.HandMakeSubjectId == o.subject.HandMakeSubjectId).Select(sub => sub.Id).ToList();
                                    handOrderIdList.Add(o.subject.HandMakeSubjectId ?? 0);
                                    if (handOrderIdList.Any())
                                    {
                                        orderBll.Delete(sub => sub.ShopId == o.order.ShopId && sub.Sheet.ToLower() == o.order.Sheet.ToLower() && sub.Gender == o.order.Gender && sub.GraphicLength == o.order.GraphicLength && sub.GraphicWidth == o.order.GraphicWidth && handOrderIdList.Contains(sub.SubjectId ?? 0));

                                    }
                                }
                                else
                                {
                                    orderModel = new FinalOrderDetailTemp();
                                    orderModel.AgentCode = o.shop.AgentCode;
                                    orderModel.AgentName = o.shop.AgentName;
                                    orderModel.BusinessModel = o.shop.BusinessModel;
                                    orderModel.Channel = o.shop.Channel;
                                    orderModel.City = o.shop.CityName;
                                    orderModel.CityTier = o.shop.CityTier;
                                    orderModel.Contact = o.shop.Contact1;
                                    orderModel.Format = o.shop.Format;
                                    orderModel.Gender = o.order.Gender;
                                    orderModel.GraphicNo = o.order.GraphicNo;
                                    orderModel.IsInstall = o.shop.IsInstall;
                                    orderModel.LocationType = o.shop.LocationType;
                                    orderModel.MaterialSupport = o.order.MaterialSupport;
                                    orderModel.OrderType = o.order.OrderType;
                                    orderModel.POPAddress = o.shop.POPAddress;
                                    orderModel.POSScale = o.order.POSScale;
                                    orderModel.InstallPricePOSScale = o.order.POSScale;
                                    orderModel.InstallPriceMaterialSupport = o.order.MaterialSupport;
                                    //if (!string.IsNullOrWhiteSpace(o.shop.Format) && (o.shop.Format.ToLower().Contains("kids") || o.shop.Format.ToLower().Contains("infant")))
                                    if (subjectCategoryName == "童店")    
                                        orderModel.InstallPriceMaterialSupport = "Others";
                                    orderModel.Province = o.shop.ProvinceName;
                                    orderModel.Quantity = o.order.Quantity;
                                    orderModel.Region = o.shop.RegionName;
                                    orderModel.ChooseImg = o.order.ChooseImg;
                                    
                                    orderModel.Remark = o.order.Remark;
                                    orderModel.Sheet = o.order.Sheet;
                                    orderModel.ShopId = o.shop.Id;
                                    orderModel.ShopName = o.shop.ShopName;
                                    orderModel.ShopNo = o.shop.ShopNo;

                                    orderModel.SubjectId = subjectId;
                                    orderModel.Tel = o.shop.Tel1;
                                    orderModel.MachineFrame = o.order.MachineFrame;
                                    orderModel.LevelNum = o.order.LevelNum;
                                    decimal width = o.order.GraphicWidth ?? 0;
                                    decimal length = o.order.GraphicLength ?? 0;
                                    orderModel.GraphicLength = length;
                                    orderModel.GraphicMaterial = o.order.GraphicMaterial;
                                    orderModel.GraphicWidth = width;
                                    
                                    orderModel.PositionDescription = o.order.PositionDescription;
                                    orderModel.Area = (width * length) / 1000000;
                                    orderModel.Category = o.order.Category;
                                    orderModel.IsHC = true;
                                    orderModel.InstallPositionDescription = o.order.InstallPositionDescription;

                                    decimal unitPrice = 0;
                                    decimal totalPrice = 0;
                                    if (!string.IsNullOrWhiteSpace(o.order.GraphicMaterial))
                                    {
                                        POP pop = new POP();
                                        pop.GraphicMaterial = o.order.GraphicMaterial;
                                        pop.GraphicLength = o.order.GraphicLength;
                                        pop.GraphicWidth = o.order.GraphicWidth;
                                        pop.Quantity = o.order.Quantity;
                                        pop.PriceItemId = materialPriceItemId;
                                        unitPrice = GetMaterialUnitPrice(pop, out totalPrice);
                                    }
                                    orderModel.UnitPrice = unitPrice;
                                    orderModel.TotalPrice = totalPrice;

                                    orderBll.Add(orderModel);
                                }
                            });
                        }
                        */

                        //Subject subjectModel = subjectBll.GetModel(subjectId);
                        //bool flag = false;

                        HandMadeOrderDetailBLL detailBll = new HandMadeOrderDetailBLL();
                        HandMadeOrderDetail detailModel;
                        var orderList = detailBll.GetList(s=>s.SubjectId==subjectId && (s.IsSubmit==null || s.IsSubmit==0));
                        if (orderList.Any())
                        {
                            orderList.ForEach(s =>
                            {
                                detailModel = s;
                                detailModel.IsSubmit = 1;
                                detailBll.Update(detailModel);
                            });
                            supplimentSubjectModel.Status = 4;//提交完成
                            supplimentSubjectModel.ApproveState = 0;
                            subjectBll.Update(supplimentSubjectModel);
                            tran.Complete();
                            Alert("提交成功！", "List.aspx");
                        }
                        else
                        {
                            Alert("提交失败：没有可以提交的数据！");
                        }
                        
                    }
                    else
                    {
                        Alert("提交失败！");
                    }
                }
                catch (Exception ex)
                {
                    Alert("提交失败！");
                }

            }

        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("Add.aspx?subjectId={0}", subjectId), false);
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindPOPList();
        }


        protected void AspNetPager2_PageChanged(object sender, EventArgs e)
        {
            BindHCList();
        }

        protected void btnSreach1_Click(object sender, EventArgs e)
        {
            BindPOPList();
        }

        protected void btnSreach2_Click(object sender, EventArgs e)
        {
            BindHCList();
        }


        int AddShop222(DataRow dr, out string msg)
        {
            msg = string.Empty;
            int shopId = 0;
            int provinceId = 0;
            int cityId = 0;
            int countyId = 0;
            string shopNo = string.Empty;
            string shopName = string.Empty;
            string region = string.Empty;
            string province = string.Empty;
            string city = string.Empty;
            string county = string.Empty;
            string cityLevel = string.Empty;
            string isInstall = string.Empty;
            string angentCode = string.Empty;
            string angentName = string.Empty;
            string adress = string.Empty;
            string contact1 = string.Empty;
            string tel1 = string.Empty;
            string contact2 = string.Empty;
            string tel2 = string.Empty;
            string channel = string.Empty;
            string format = string.Empty;
            string locationType = string.Empty;
            string customerModel = string.Empty;
            string shopLevel = string.Empty;
            string materialSupport = string.Empty;
            bool canSave = true;
            StringBuilder errorMsg = new StringBuilder();
            if (cols.Contains("POS Code"))
                shopNo = dr["POS Code"].ToString().Trim();
            else if (cols.Contains("POSCode"))
                shopNo = dr["POSCode"].ToString().Trim();
            else if (cols.Contains("店铺编号"))
                shopNo = dr["店铺编号"].ToString().Trim();
            if (cols.Contains("POS Name"))
                shopName = dr["POS Name"].ToString().Trim();
            else if (cols.Contains("POSName"))
                shopName = dr["POSName"].ToString().Trim();
            else if (cols.Contains("店铺名称"))
                shopName = dr["店铺名称"].ToString().Trim();
            if (cols.Contains("Region"))
                region = dr["Region"].ToString().Trim();
            else if (cols.Contains("区域"))
                region = dr["区域"].ToString().Trim();
            if (cols.Contains("Province"))
                province = dr["Province"].ToString().Trim();
            else if (cols.Contains("省份"))
                province = dr["省份"].ToString().Trim();
            if (cols.Contains("City"))
                city = dr["City"].ToString().Trim();
            else if (cols.Contains("城市"))
                city = dr["城市"].ToString().Trim();

            if (cols.Contains("县/区"))
                county = dr["县/区"].ToString().Trim();
            else if (cols.Contains("区"))
                county = dr["区"].ToString().Trim();

            if (cols.Contains("City Tier"))
                cityLevel = dr["City Tier"].ToString().Trim();
            else if (cols.Contains("CityTier"))
                cityLevel = dr["CityTier"].ToString().Trim();
            else if (cols.Contains("城市级别"))
                cityLevel = dr["城市级别"].ToString().Trim();
            if (cols.Contains("是否安装"))
                isInstall = dr["是否安装"].ToString().Trim();
            else if (cols.Contains("IsInstall"))
                isInstall = dr["IsInstall"].ToString().Trim();
            if (cols.Contains("店铺级别"))
                materialSupport = dr["店铺级别"].ToString().Trim();
            else if (cols.Contains("物料支持"))
                materialSupport = dr["物料支持"].ToString().Trim();
            else if (cols.Contains("物料支持级别"))
                materialSupport = dr["物料支持级别"].ToString().Trim();
            if (cols.Contains("Customer Code"))
                angentCode = dr["Customer Code"].ToString().Trim();
            else if (cols.Contains("CustomerCode"))
                angentCode = dr["CustomerCode"].ToString().Trim();
            else if (cols.Contains("经销商编号"))
                angentCode = dr["经销商编号"].ToString().Trim();

            if (cols.Contains("Customer Name"))
                angentName = dr["Customer Name"].ToString().Trim();
            else if (cols.Contains("CustomerName"))
                angentName = dr["CustomerName"].ToString().Trim();
            else if (cols.Contains("经销商名称"))
                angentName = dr["经销商名称"].ToString().Trim();
            if (cols.Contains("POPAddress"))
                adress = dr["POPAddress"].ToString().Trim();
            else if (cols.Contains("店铺地址"))
                adress = dr["店铺地址"].ToString().Trim();
            else if (cols.Contains("Address"))
                adress = dr["Address"].ToString().Trim();

            if (cols.Contains("popContact1"))
                contact1 = dr["popContact1"].ToString().Trim();
            else if (cols.Contains("Contact1"))
                contact1 = dr["Contact1"].ToString().Trim();
            else if (cols.Contains("Contact"))
                contact1 = dr["Contact"].ToString().Trim();
            else if (cols.Contains("联系人1"))
                contact1 = dr["联系人1"].ToString().Trim();
            else if (cols.Contains("联系人"))
                contact1 = dr["联系人"].ToString().Trim();

            if (cols.Contains("popTel1"))
                tel1 = dr["popTel1"].ToString().Trim();
            else if (cols.Contains("Tel1"))
                tel1 = dr["Tel1"].ToString().Trim();
            else if (cols.Contains("Tel"))
                tel1 = dr["Tel"].ToString().Trim();
            else if (cols.Contains("联系电话1"))
                tel1 = dr["联系电话1"].ToString().Trim();
            else if (cols.Contains("联系电话"))
                tel1 = dr["联系电话"].ToString().Trim();

            if (cols.Contains("popContact2"))
                contact2 = dr["popContact2"].ToString().Trim();
            else if (cols.Contains("Contact2"))
                contact2 = dr["Contact2"].ToString().Trim();
            else if (cols.Contains("联系人2"))
                contact2 = dr["联系人2"].ToString().Trim();

            if (cols.Contains("popTel2"))
                tel2 = dr["popTel2"].ToString().Trim();
            else if (cols.Contains("Tel2"))
                tel2 = dr["Tel2"].ToString().Trim();
            else if (cols.Contains("联系电话2"))
                tel2 = dr["联系电话2"].ToString().Trim();

            if (cols.Contains("Channel"))
                channel = dr["Channel"].ToString().Trim();
            else if (cols.Contains("产品类型"))
                channel = dr["产品类型"].ToString().Trim();

            if (cols.Contains("Format"))
                format = dr["Format"].ToString().Trim();
            else if (cols.Contains("店铺类型"))
                format = dr["店铺类型"].ToString().Trim();

            if (cols.Contains("店铺级别"))
                shopLevel = dr["店铺级别"].ToString().Trim();
            if (cols.Contains("Location Type"))
                locationType = dr["Location Type"].ToString().Trim();
            else if (cols.Contains("LocationType"))
                locationType = dr["LocationType"].ToString().Trim();
            else if (cols.Contains("店铺属性"))
                locationType = dr["店铺属性"].ToString().Trim();

            if (cols.Contains("Business Model"))
                customerModel = dr["Business Model"].ToString().Trim();
            else if (cols.Contains("BusinessModel"))
                customerModel = dr["BusinessModel"].ToString().Trim();
            else if (cols.Contains("客户类别"))
                customerModel = dr["客户类别"].ToString().Trim();
            if (string.IsNullOrWhiteSpace(shopName))
            {
                canSave = false;
                errorMsg.Append("店铺名称为空 ；");
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                canSave = false;
                errorMsg.Append("区域为空 ；");
            }
            if (!string.IsNullOrWhiteSpace(province))
            {
                provinceId = GetProvinceId(province);
                if (provinceId > 0)
                {
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        cityId = GetCityId(city, provinceId);
                        if (cityId > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(county))
                            {
                                countyId = GetCityId(county, cityId);
                                if (countyId == 0)
                                {
                                    //canSave = false;
                                    //errorMsg.Append("区县填写填写不正确 ；");
                                }
                            }
                        }
                        else
                        {
                            string newCity = string.Empty;
                            GetCity(city, out newCity, out cityId, out countyId);
                            if (cityId == 0)
                            {
                                //canSave = false;
                                //errorMsg.Append("城市填写填写不正确 ；");
                            }
                            else
                            {
                                county = city;
                                city = newCity;
                            }
                        }
                    }
                }
                else
                {
                    canSave = false;
                    errorMsg.Append("省份填写填写不正确 ；");
                }
            }
            if (string.IsNullOrWhiteSpace(cityLevel))
            {
                //canSave = false;
                //errorMsg.Append("城市级别为空 ；");
            }
            if (string.IsNullOrWhiteSpace(isInstall))
            {
                //canSave = false;
                //errorMsg.Append("是否安装为空 ；");
            }
            if (string.IsNullOrWhiteSpace(adress))
            {
                //canSave = false;
                //errorMsg.Append("店铺地址为空 ；");
            }
            if (string.IsNullOrWhiteSpace(format))
            {
                //canSave = false;
                //errorMsg.Append("店铺类型为空 ；");
            }
            if (canSave)
            {
                Shop shopModel = new Models.Shop();
                if (provinceId > 0)
                    shopModel.ProvinceId = provinceId;
                if (cityId > 0)
                    shopModel.CityId = cityId;
                if (countyId > 0)
                    shopModel.AreaId = countyId;
                shopModel.RegionName = region;
                shopModel.ProvinceName = province;
                shopModel.CityName = city;
                shopModel.AreaName = county;
                shopModel.AgentCode = angentCode;
                shopModel.AgentName = angentName;
                shopModel.BusinessModel = customerModel;
                shopModel.Channel = channel;
                shopModel.CityTier = cityLevel;
                shopModel.Contact1 = contact1;
                shopModel.Tel1 = tel1;
                shopModel.Contact2 = contact2;
                shopModel.Tel2 = tel2;
                shopModel.CustomerId = !string.IsNullOrWhiteSpace(hfCustomerId.Value) ? int.Parse(hfCustomerId.Value) : 0;
                shopModel.Format = format.Replace("（", "(").Replace("）", ")"); ;
                shopModel.IsInstall = isInstall;
                shopModel.LocationType = locationType;
                shopModel.POPAddress = adress;
                shopModel.ShopName = shopName;
                shopModel.AddDate = DateTime.Now;
                shopModel.ShopNo = shopNo.ToUpper();
                shopModel.IsDelete = false;
                shopModel.ShopLevel = shopLevel;
                shopModel.MaterialSupport = materialSupport;
                shopModel.Status = "正常";
                shopBll.Add(shopModel);
                shopId = shopModel.Id;
            }
            else
                msg = errorMsg.ToString();
            return shopId;
        }


        List<Models.Place> placeList = new List<Models.Place>();
        //RegionBLL regionBll = new RegionBLL();
        PlaceBLL placeBll = new PlaceBLL();

        /// <summary>
        /// 省份ID
        /// </summary>
        /// <param name="provinceName"></param>
        /// <returns></returns>
        int GetProvinceId(string provinceName)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => (s.PlaceName == provinceName || s.PlaceName == provinceName + "省") && s.ParentID == 0).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }
        /// <summary>
        /// 城市ID 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        int GetCityId(string cityName, int provinceId)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => s.PlaceName == cityName && s.ParentID == provinceId).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }

        /// <summary>
        /// 县ID 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        int GetCountyId(string countyName, int cityId)
        {
            int id = 0;
            if (!placeList.Any())
            {
                placeList = placeBll.GetList(s => 1 == 1);
            }
            var list = placeList.Where(s => s.PlaceName == countyName && s.ParentID == cityId).ToList();
            if (list.Any())
            {
                id = list[0].ID;
            }
            return id;
        }

        /// <summary>
        /// 通过县名称获取城市Id
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="cityId"></param>
        /// <param name="areaId"></param>
        void GetCity(string areaName, out string cityName, out int cityId, out int areaId)
        {
            cityId = 0; areaId = 0;
            cityName = string.Empty;
            Models.Place placeModel = placeBll.GetList(s => s.PlaceName == areaName).FirstOrDefault();
            if (placeModel != null)
            {
                cityId = placeModel.ParentID ?? 0;
                areaId = placeModel.ID;
                var model1 = placeBll.GetModel(cityId);
                if (model1 != null)
                    cityName = model1.PlaceName;
            }
        }

        int guidanceType = 0;
        int guidanceId = 0;
        Dictionary<string, Shop> shopDic = new Dictionary<string, Shop>();
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
                {
                    guidanceType = guidance.ActivityTypeId ?? 1;//活动类型：1安装，2发货，3促销
                    guidanceId = guidance.ItemId;
                }
            }
            return flag;
        }
    }
}
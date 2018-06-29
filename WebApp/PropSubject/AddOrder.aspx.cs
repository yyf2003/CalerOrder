using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using Models;
using System.Configuration;
using Common;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace WebApp.PropSubject
{
    public partial class AddOrder : BasePage
    {
        public int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            if (Request.QueryString["import"] != null && Request.QueryString["import"] == "1")
            {

                Panel1.Visible = true;
                if (Session["errorTb"] != null)
                {
                    labState.Text = "部分数据导入失败！";
                    ExportFailMsg.Style.Add("display", "block");
                }
            }
            else
            {
                Session["errorTb"] = null;
            }
            if (!IsPostBack)
            {
                BindSubject();
                BindOrder();
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
                                    subject.GuidanceId,
                                    subject.SubjectName,
                                    subject.SubjectNo,
                                    customer.CustomerName,
                                    subject.CustomerId,
                                    subject.SubjectType
                                }).FirstOrDefault();
            if (subjectModel != null)
            {

                labSubjectName.Text = subjectModel.SubjectName;
                labSubjectNo.Text = subjectModel.SubjectNo;
                labCustomer.Text = subjectModel.CustomerName;
                hfGuidanceId.Value = (subjectModel.GuidanceId ?? 0).ToString();
            }
        }

        void BindOrder()
        {
            List<PropOrderDetail> orderList = new List<PropOrderDetail>();
            var list = (from iOrder in CurrentContext.DbContext.PropOrderDetail
                        join oOrder in CurrentContext.DbContext.PropOutsourceOrderDetail
                        on iOrder.Id equals oOrder.PropOrderId
                        where iOrder.SubjectId == subjectId
                        select new {
                           iOrder,
                           oOrder
                       }).OrderBy(s=>s.iOrder.Id).ToList();
            if (list.Any())
            {
                int orderId = 0;
                int index = 0;
                foreach (var item in list)
                {
                    PropOrderDetail model = new PropOrderDetail();
                    if (index == 0)
                    {
                        orderId = item.iOrder.Id;
                        model.Id = item.iOrder.Id;
                        model.BOMCost = item.iOrder.BOMCost;
                        model.Dimension = item.iOrder.Dimension;
                        model.MaterialName = item.iOrder.MaterialName;
                        model.MaterialType = item.iOrder.MaterialType;
                        model.Packaging = item.iOrder.Packaging;
                        model.PackingCost = item.iOrder.PackingCost;
                        model.ProcessingCost = item.iOrder.ProcessingCost;
                        model.Quantity = item.iOrder.Quantity;
                        model.ServiceType = item.iOrder.ServiceType;
                        model.Sheet = item.iOrder.Sheet;
                        model.SubjectId = item.iOrder.SubjectId;
                        model.TransportationCost = item.iOrder.TransportationCost;
                        model.UnitName = item.iOrder.UnitName;
                        model.UnitPrice = item.iOrder.UnitPrice;
                    }
                    else
                    {
                        if (orderId == item.iOrder.Id)
                        {
                            model.Id = 0;
                            model.MaterialName = item.oOrder.MaterialName;
                            //model.BOMCost = null;
                            //model.Dimension = item.iOrder.Dimension;
                            //model.MaterialName = item.iOrder.MaterialName;
                            //model.MaterialType = item.iOrder.MaterialType;
                            //model.Packaging = item.iOrder.Packaging;
                            //model.PackingCost = item.iOrder.PackingCost;
                            //model.ProcessingCost = item.iOrder.ProcessingCost;

                            //model.Quantity = item.iOrder.Quantity;
                            //model.ServiceType = item.iOrder.ServiceType;
                            //model.Sheet = item.iOrder.Sheet;
                            //model.SubjectId = item.iOrder.SubjectId;
                            //model.TransportationCost = item.iOrder.TransportationCost;
                            //model.UnitName = item.iOrder.UnitName;
                            //model.UnitPrice = item.iOrder.UnitPrice;
                        }
                        else
                        {

                            orderId = item.iOrder.Id;
                            model.Id = item.iOrder.Id;
                            model.BOMCost = item.iOrder.BOMCost;
                            model.Dimension = item.iOrder.Dimension;
                            model.MaterialName = item.iOrder.MaterialName;
                            model.MaterialType = item.iOrder.MaterialType;
                            model.Packaging = item.iOrder.Packaging;
                            model.PackingCost = item.iOrder.PackingCost;
                            model.ProcessingCost = item.iOrder.ProcessingCost;
                            model.Quantity = item.iOrder.Quantity;
                            model.ServiceType = item.iOrder.ServiceType;
                            model.Sheet = item.iOrder.Sheet;
                            model.SubjectId = item.iOrder.SubjectId;
                            model.TransportationCost = item.iOrder.TransportationCost;
                            model.UnitName = item.iOrder.UnitName;
                            model.UnitPrice = item.iOrder.UnitPrice;
                        }
                    }
                    model.PropOrderId = item.oOrder.PropOrderId;
                    model.OutsourceName = item.oOrder.OutsourceName;
                    model.PayMaterialName = item.oOrder.MaterialName;
                    model.PayPackaging = item.oOrder.Packaging;
                    model.PayQuantity = item.oOrder.Quantity;
                    model.PayRemark = item.oOrder.Remark;
                    model.PayUnitName = item.oOrder.UnitName;
                    model.PayUnitPrice = item.oOrder.UnitPrice;

                    orderList.Add(model);
                    index++;
                }
                PanelSubmit.Visible = true;
                PanelBack.Visible = false;
            }
            else
            {
                PanelSubmit.Visible = false;
                PanelBack.Visible = true;
            }
            Repeater1.DataSource = orderList;
            Repeater1.DataBind();
            Panel2.Visible = list.Any();
        }

        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "道具订单模板";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
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
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        errorTB = CommonMethod.CreateErrorTB(cols);

                        int propOrderId = 0;
                        //道具名称
                        string materialName = string.Empty;
                        //应用位置
                        string sheet = string.Empty;
                        //材料类型
                        string materialType = string.Empty;
                        //尺寸规格
                        string dimension = string.Empty;
                        //服务描述
                        string serviceType = string.Empty;
                        //包装方式
                        string packaging = string.Empty;
                        //单位
                        string unitName = string.Empty;
                        //数量
                        string rquantity = string.Empty;
                        //材料成本
                        string BOMCost = string.Empty;
                        //加工成本
                        string processingCost = string.Empty;
                        //包装费
                        string packagingCost = string.Empty;
                        //运输费
                        string transportationCost = string.Empty;

                        //外协名称
                        string outsourceName = string.Empty;
                        //外协包装方式
                        string payPackaging = string.Empty;
                        //外协单位
                        string payUnitName = string.Empty;
                        //外协数量
                        string payQuantity = string.Empty;
                        //外协单价
                        string payUnitPrice = string.Empty;
                        //备注
                        string remark = string.Empty;


                        PropOrderDetailBLL propOrderBll = new PropOrderDetailBLL();
                        PropOrderDetail propOrderModel;
                        PropOutsourceOrderDetailBLL propOutsourceOrderBll = new PropOutsourceOrderDetailBLL();
                        PropOutsourceOrderDetail propOutsourceOrderModel;
                        if (!cbAdd.Checked)
                        {
                            propOrderBll.Delete(s => s.SubjectId == subjectId);
                            propOutsourceOrderBll.Delete(s => s.SubjectId == subjectId);
                        }
                        int successNum = 0;
                        for (int i = 2; i < ds.Tables[0].Rows.Count; i++)
                        {
                            bool canSave = true;
                            DataRow dr = ds.Tables[0].Rows[i];
                            StringBuilder msg = new StringBuilder();

                            decimal runitPrice = 0;

                            materialName = string.Empty;
                            sheet = string.Empty;
                            materialType = string.Empty;
                            dimension = string.Empty;
                            serviceType = string.Empty;
                            packaging = string.Empty;
                            unitName = string.Empty;
                            rquantity = string.Empty;
                            BOMCost = string.Empty;
                            processingCost = string.Empty;
                            packagingCost = string.Empty;
                            transportationCost = string.Empty;

                            outsourceName = string.Empty;
                            payPackaging = string.Empty;
                            payUnitName = string.Empty;
                            payQuantity = string.Empty;
                            payUnitPrice = string.Empty;
                            remark = string.Empty;

                            
                            materialName = dr[0].ToString();
                            sheet = dr[1].ToString();
                            materialType = dr[2].ToString();
                            dimension = dr[3].ToString();
                            serviceType = dr[4].ToString();
                            packaging = dr[5].ToString();
                            unitName = dr[6].ToString();
                            rquantity = dr[7].ToString();
                            BOMCost = dr[8].ToString();
                            processingCost = dr[9].ToString();
                            packagingCost = dr[10].ToString();
                            transportationCost = dr[11].ToString();

                            
                            outsourceName = dr[14].ToString();
                            payPackaging = dr[15].ToString();
                            payUnitName = dr[16].ToString();
                            payQuantity = dr[17].ToString();
                            payUnitPrice = dr[18].ToString();
                            remark = dr[20].ToString();

                            
                            if (string.IsNullOrWhiteSpace(materialName))
                            {
                                canSave = false;
                                msg.Append("道具名称不能空；");
                            }
                            if (!string.IsNullOrWhiteSpace(rquantity)&&!StringHelper.IsIntVal(rquantity))
                            {
                                canSave = false;
                                msg.Append("应收数量填写不正确；");
                            }
                            if (!string.IsNullOrWhiteSpace(BOMCost) && !StringHelper.IsDecimalVal(BOMCost))
                            {
                                canSave = false;
                                msg.Append("材料成本填写不正确；");
                            }
                            if (!string.IsNullOrWhiteSpace(processingCost) && !StringHelper.IsDecimalVal(processingCost))
                            {
                                canSave = false;
                                msg.Append("加工成本填写不正确；");
                            }
                            if (!string.IsNullOrWhiteSpace(packagingCost) && !StringHelper.IsDecimalVal(packagingCost))
                            {
                                canSave = false;
                                msg.Append("包装费填写不正确；");
                            }
                            if (!string.IsNullOrWhiteSpace(transportationCost) && !StringHelper.IsDecimalVal(transportationCost))
                            {
                                canSave = false;
                                msg.Append("运输费填写不正确；");
                            }
                            if (string.IsNullOrWhiteSpace(outsourceName))
                            {
                                canSave = false;
                                msg.Append("外协名称不能空；");
                            }
                            if (string.IsNullOrWhiteSpace(payQuantity))
                            {
                                canSave = false;
                                msg.Append("应付数量不能空；");
                            }
                            else if (!StringHelper.IsIntVal(payQuantity))
                            {
                                canSave = false;
                                msg.Append("应付数量填写不正确；");
                            }
                            if (string.IsNullOrWhiteSpace(payUnitPrice))
                            {
                                canSave = false;
                                msg.Append("应付单价不能空；");
                            }
                            else if (!StringHelper.IsDecimalVal(payUnitPrice))
                            {
                                canSave = false;
                                msg.Append("应付单价填写不正确；");
                            }
                            if (canSave)
                            {
                                propOrderModel = new PropOrderDetail();
                                runitPrice = StringHelper.IsDecimal(BOMCost) + StringHelper.IsDecimal(processingCost) + StringHelper.IsDecimal(packagingCost) + StringHelper.IsDecimal(transportationCost);
                                if (StringHelper.IsInt(rquantity) > 0 && runitPrice > 0)
                                { 
                                   //保存应收
                                    propOrderModel = new PropOrderDetail();
                                    propOrderModel.AddDate = DateTime.Now;
                                    propOrderModel.AddUserId = CurrentUser.UserId;
                                    propOrderModel.BOMCost = StringHelper.IsDecimal(BOMCost);
                                    propOrderModel.Dimension = dimension;
                                    propOrderModel.GuidanceId = StringHelper.IsInt(hfGuidanceId.Value);
                                    propOrderModel.IsDelete = false;
                                    propOrderModel.MaterialName = materialName;
                                    propOrderModel.MaterialType = materialType;
                                    propOrderModel.Packaging = packaging;
                                    propOrderModel.PackingCost = StringHelper.IsDecimal(packagingCost);
                                    propOrderModel.ProcessingCost = StringHelper.IsDecimal(processingCost);
                                    propOrderModel.Quantity = StringHelper.IsInt(rquantity);
                                    propOrderModel.ServiceType = serviceType;
                                    propOrderModel.Sheet = sheet;
                                    propOrderModel.SubjectId = subjectId;
                                    propOrderModel.TransportationCost = StringHelper.IsDecimal(transportationCost);
                                    propOrderModel.UnitName = unitName;
                                    propOrderModel.UnitPrice = runitPrice;
                                    propOrderBll.Add(propOrderModel);
                                    propOrderId = propOrderModel.Id;
                                }
                                if (StringHelper.IsInt(payQuantity) > 0 && StringHelper.IsDecimal(payUnitPrice) > 0)
                                {
                                    propOutsourceOrderModel = new PropOutsourceOrderDetail();
                                    propOutsourceOrderModel.AddDate = DateTime.Now;
                                    propOutsourceOrderModel.AddUserId = CurrentUser.UserId;
                                    propOutsourceOrderModel.GuidanceId = StringHelper.IsInt(hfGuidanceId.Value);
                                    propOutsourceOrderModel.IsDelete = false;
                                    propOutsourceOrderModel.MaterialName = materialName;
                                    propOutsourceOrderModel.OutsourceName = outsourceName;
                                    propOutsourceOrderModel.Packaging = payPackaging;
                                    propOutsourceOrderModel.PropOrderId = propOrderId;
                                    propOutsourceOrderModel.Quantity = StringHelper.IsInt(payQuantity);
                                    propOutsourceOrderModel.Remark = remark;
                                    propOutsourceOrderModel.SubjectId = subjectId;
                                    propOutsourceOrderModel.UnitName = payUnitName;
                                    propOutsourceOrderModel.UnitPrice = StringHelper.IsDecimal(payUnitPrice);
                                    propOutsourceOrderBll.Add(propOutsourceOrderModel);
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
                            SubjectBLL subjectBll = new SubjectBLL();
                            Subject model = subjectBll.GetModel(subjectId);
                            if (model != null)
                            {
                                model.Status = 3;
                                subjectBll.Update(model);
                            }
                        }
                        Response.Redirect(string.Format("AddOrder.aspx?import=1&subjectId={0}", subjectId), false);
                    }
                }
            }
        }

        /// <summary>
        /// 导出失败信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbExportError_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SubjectBLL subjectBll = new SubjectBLL();
            Subject model = subjectBll.GetModel(subjectId);
            if (model != null)
            {
                model.Status = 4;
                subjectBll.Update(model);
            }
            Alert("提交成功", "/Subjects/SubjectList.aspx");
            //Response.Redirect("/Subjects/SubjectList.aspx",false);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddSubject.aspx?subjectId="+subjectId, false);
        }
    }
}
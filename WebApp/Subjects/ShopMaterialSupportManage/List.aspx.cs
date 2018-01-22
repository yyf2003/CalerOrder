using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using BLL;
using Models;
using System.Data.OleDb;
using Common;
using System.Configuration;
using System.Data;
using System.Text;

namespace WebApp.Subjects.ShopMaterialSupportManage
{
    public partial class List : BasePage
    {
        int guidanceId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guidanceId"] != null)
            {
                guidanceId = int.Parse(Request.QueryString["guidanceId"]);
            }
            if (Request.QueryString["import"] != null)
            {
                if (Session["errorTb"] != null)
                {
                    labImportState.Text = "存在失败信息：";
                    lbExportErrorMsg.Visible = true;
                }
                else
                {
                    labImportState.Text = "导入成功";
                    lbExportErrorMsg.Visible = false;
                }
            }
            
            if (!IsPostBack)
            {
                SubjectGuidance guidanceModel = new SubjectGuidanceBLL().GetModel(guidanceId);
                if (guidanceModel != null)
                {
                    labGuidanceName.Text = guidanceModel.ItemName;
                }
                BindData();
            }
        }

        void BindData()
        {
            var list = (from ms in CurrentContext.DbContext.ShopMaterialSupport
                       join shop in CurrentContext.DbContext.Shop
                       on ms.ShopId equals shop.Id
                       where ms.GuidanceId == guidanceId
                       select new { 
                          ms.MaterialSupport,
                          shop
                       }).ToList();
            AspNetPager1.RecordCount = list.Count;
            this.AspNetPager1.CustomInfoHTML = string.Format("当前第{0}/{1}页 共{2}条记录 每页{3}条", new object[] { this.AspNetPager1.CurrentPageIndex, this.AspNetPager1.PageCount, this.AspNetPager1.RecordCount, this.AspNetPager1.PageSize });
            gv.DataSource = list.OrderByDescending(s => s.shop.Id).Skip((AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize).Take(AspNetPager1.PageSize).ToList();
            gv.DataBind();
        }


        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
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
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ShopMaterialSupportBLL bll = new ShopMaterialSupportBLL();
                        ShopMaterialSupport model;
                        if(!cbAdd.Checked)
                            bll.Delete(s=>s.GuidanceId==guidanceId);
                        string shopNo = string.Empty;
                        string materialSupport = string.Empty;
                        DataColumnCollection cols = ds.Tables[0].Columns;
                        DataTable errorTB = Common.CommonMethod.CreateErrorTB(cols);

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

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            bool canSave = true;
                            StringBuilder errorMsg = new StringBuilder();
                            int shopId = 0;
                            shopNo = string.Empty;
                            materialSupport = string.Empty;
                            if (cols.Contains("POS Code"))
                                shopNo = dr["POS Code"].ToString().Trim();
                            else if (cols.Contains("POSCode"))
                                shopNo = dr["POSCode"].ToString().Trim();
                            else if (cols.Contains("店铺编号"))
                                shopNo = dr["店铺编号"].ToString().Trim();

                            if (cols.Contains("物料支持级别"))
                                materialSupport = dr["物料支持级别"].ToString().Trim();
                            else if (cols.Contains("物料支持"))
                                materialSupport = dr["物料支持"].ToString().Trim();

                            if (string.IsNullOrWhiteSpace(shopNo))
                            {
                                canSave = false;
                                errorMsg.Append("店铺编号为空 ；");
                            }
                            else if (!CheckShop(shopNo, out shopId))
                            {
                                canSave = false;
                                errorMsg.Append("店铺编号 不存在；");
                            }
                            if (string.IsNullOrWhiteSpace(materialSupport))
                            {
                                canSave = false;
                                errorMsg.Append("物料支持级别 为空 ；");
                            }
                            else if (!materialSupportList.Contains(materialSupport.ToUpper()))
                            {
                                canSave = false;
                                errorMsg.AppendFormat("物料支持填写不正确，必须是{0}之一；", StringHelper.ListToString(materialSupportList));
                            }
                            if (canSave)
                            {
                                
                                var oldModel = bll.GetList(s => s.GuidanceId == guidanceId && s.ShopId == shopId).FirstOrDefault();
                                if (oldModel != null)
                                {
                                    if (oldModel.MaterialSupport.ToUpper() != materialSupport.ToUpper())
                                    {
                                        var subjectList = new SubjectBLL().GetList(s => s.GuidanceId == guidanceId);
                                        if (subjectList.Any())
                                        {
                                            //更新订单里面的物料支持数据
                                            var orderList1 = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                                              join subject in CurrentContext.DbContext.Subject
                                                              on order.SubjectId equals subject.Id
                                                              where subject.GuidanceId == guidanceId
                                                              && order.ShopId == shopId
                                                              select order).ToList();
                                            if (orderList1.Any())
                                            {
                                                MergeOriginalOrderBLL MergeOriginalOrderBll = new MergeOriginalOrderBLL();
                                                orderList1.ForEach(s =>
                                                {
                                                    s.MaterialSupport = materialSupport;
                                                    MergeOriginalOrderBll.Update(s);
                                                });
                                            }
                                            var handMakeOrderList = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                                                                     join subject in CurrentContext.DbContext.Subject
                                                                     on order.SubjectId equals subject.Id
                                                                     where subject.GuidanceId == guidanceId
                                                                     && order.ShopId == shopId
                                                                     select order).ToList();
                                            if (handMakeOrderList.Any())
                                            {
                                                HandMadeOrderDetailBLL HandMadeOrderDetailBll = new HandMadeOrderDetailBLL();
                                                handMakeOrderList.ForEach(s =>
                                                {
                                                    s.MaterialSupport = materialSupport;
                                                    HandMadeOrderDetailBll.Update(s);
                                                });
                                            }
                                            var RegionOrderList = (from order in CurrentContext.DbContext.RegionOrderDetail
                                                                   join subject in CurrentContext.DbContext.Subject
                                                                   on order.SubjectId equals subject.Id
                                                                   where subject.GuidanceId == guidanceId
                                                                   && order.ShopId == shopId
                                                                   select order).ToList();
                                            if (RegionOrderList.Any())
                                            {
                                                RegionOrderDetailBLL RegionOrderDetailBll = new RegionOrderDetailBLL();
                                                RegionOrderList.ForEach(s =>
                                                {
                                                    s.MaterialSupport = materialSupport;
                                                    RegionOrderDetailBll.Update(s);
                                                });
                                            }
                                            var finalOrderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                                                  join subject in CurrentContext.DbContext.Subject
                                                                  on order.SubjectId equals subject.Id
                                                                  where subject.GuidanceId == guidanceId
                                                                  && order.ShopId == shopId
                                                                  select order).ToList();
                                            if (finalOrderList.Any())
                                            {
                                                FinalOrderDetailTempBLL FinalOrderDetailTempBll = new FinalOrderDetailTempBLL();
                                                finalOrderList.ForEach(s =>
                                                {
                                                    s.MaterialSupport = materialSupport;
                                                    FinalOrderDetailTempBll.Update(s);
                                                });
                                            }
                                        }
                                    }
                                    oldModel.MaterialSupport = materialSupport;
                                    bll.Update(oldModel);
                                }
                                else
                                {
                                    model = new ShopMaterialSupport();
                                    model.AddDate = DateTime.Now;
                                    model.GuidanceId = guidanceId;
                                    model.MaterialSupport = materialSupport;
                                    model.ShopId = shopId;
                                    bll.Add(model);
                                }
                            }
                            else
                            {
                                DataRow dr1 = errorTB.NewRow();
                                for (int i = 0; i < cols.Count; i++)
                                {
                                    dr1["" + cols[i] + ""] = dr[cols[i]];
                                }
                                dr1["错误信息"] = errorMsg.ToString();
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
                        Response.Redirect(string.Format("List.aspx?import=1&guidanceId={0}", guidanceId), false);
                    }
                    else
                    {
                        labImportState.Text = "导入失败：表格没有数据";
                    }
                }
            }
        }

        protected void lbDownLoadShop_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "ShopMaterialSupportTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        ShopBLL shopBll = new ShopBLL();
        bool CheckShop(string shopNo, out int shopId)
        {
            shopId = 0;
            var list = shopBll.GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper() && (s.IsDelete == null || s.IsDelete == false));
            if (list.Any())
            {
                shopId = list[0].Id;
                return true;
            }
            return false;
        }

        protected void lbExportErrorMsg_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        }

    }
}
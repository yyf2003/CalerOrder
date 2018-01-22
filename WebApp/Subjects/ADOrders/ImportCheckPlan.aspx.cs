using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.Objects.SqlClient;
using System.Linq;
using BLL;
using DAL;
using Common;
using Models;
using System.Text;
using System.Transactions;

namespace WebApp.Subjects.ADOrders
{
    public partial class ImportCheckPlan : BasePage
    {
        int customerId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["customerId"] != null)
            {
                customerId = int.Parse(Request.QueryString["customerId"]);
            }
            int successNum = 0;
            int failNum = 0;
            if (Request.QueryString["successNum"] != null)
            {
                successNum = int.Parse(Request.QueryString["successNum"]);
            }
            if (Request.QueryString["failNum"] != null)
            {
                failNum = int.Parse(Request.QueryString["failNum"]);
            }

            if ((successNum + failNum) > 0)
            {
                Panel1.Visible = true;
                if (failNum>0)
                {
                    labState.Text = "部分数据导入失败！";
                    ExportFailMsg.Style.Add("display", "block");
                }
            }
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDownLoad_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.AppSettings["ImportTemplate"].ToString();
            string fileName = "CheckOrderPlanTemplate";
            path = path.Replace("fileName", fileName);
            OperateFile.DownLoadFile(path);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        
        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string path = OperateFile.UpLoadFile(FileUpload1.PostedFile);
                if (path != "")
                {
                    OleDbConnection conn;
                    OleDbDataAdapter da;
                    DataSet ds = null;
                    string ExcelConnStr = ConfigurationManager.ConnectionStrings["ExcelFilePath"].ToString();
                    path = Server.MapPath(path);
                    ExcelConnStr = ExcelConnStr.Replace("ExcelPath", path);
                    conn = new OleDbConnection(ExcelConnStr);
                    conn.Open();
                    string sql = "select * from [Sheet1$]";
                    da = new OleDbDataAdapter(sql, conn);
                    ds = new DataSet();
                    da.Fill(ds);
                    da.Dispose();
                    conn.Close();
                    using (TransactionScope tran = new TransactionScope())
                    {
                        try
                        {
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                DataColumnCollection cols = ds.Tables[0].Columns;
                                DataTable errorTB = CommonMethod.CreateErrorTB(cols);
                                string beginDate = txtBeginDate.Text.Trim();
                                string endDate = txtEndDate.Text.Trim();
                                Dictionary<string, List<int>> subjects = new Dictionary<string, List<int>>();
                                CheckOrderPlanDetailBLL detailBll = new CheckOrderPlanDetailBLL();
                                CheckOrderPlanDetail detailModel;
                                int successNum = 0;
                                int failNum = 0;
                                //int index = 0;
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    StringBuilder errorMsg = new StringBuilder();
                                    bool canSave = true;

                                    StringBuilder subjectIds = new StringBuilder();
                                    StringBuilder subjectNames = new StringBuilder();
                                    string region = dr["区域"].ToString();
                                    string province = dr["省份"].ToString();
                                    string city = dr["城市"].ToString();
                                    string cityTier = dr["城市级别"].ToString();
                                    string isInstall = dr["是否安装"].ToString();
                                    string format = dr["店铺类型"].ToString();
                                    string materialSupport = dr["物料支持"].ToString();
                                    string posScale = dr["店铺规模大小"].ToString();
                                    string sheet = dr["位置"].ToString();
                                    string gener = dr["性别"].ToString();
                                    string chooseImg = dr["选图"].ToString();
                                    string remark = dr["备注"].ToString();
                                    string subject = dr["活动项目"].ToString();
                                    if (string.IsNullOrWhiteSpace(sheet))
                                    {
                                        errorMsg.Append("位置 为空；");
                                        canSave = false;
                                    }
                                    if (string.IsNullOrWhiteSpace(subject))
                                    {
                                        errorMsg.Append("活动项目 为空；");
                                        canSave = false;
                                    }
                                    else
                                    {
                                        int subjectId = 0;
                                        string newName = string.Empty;
                                        List<string> subjectList = StringHelper.ToStringList(subject, ',');
                                        subjectList.ForEach(s =>
                                        {
                                            if (!checkSubject(s, beginDate, endDate, out subjectId, out newName))
                                            {
                                                errorMsg.Append("在选定的时间段中该活动项目‘" + s + "’不存在；");
                                                canSave = false;
                                            }
                                            subjectIds.Append(subjectId + ",");
                                            subjectNames.Append(newName + ",");
                                        });

                                    }
                                    if (canSave)
                                    {
                                        detailModel = new CheckOrderPlanDetail();
                                        detailModel.CityId = city;
                                        detailModel.CityTier = cityTier;
                                        detailModel.Format = format;
                                        detailModel.Gender = gener;
                                        detailModel.IsInstall = isInstall;
                                        detailModel.MaterialSupport = materialSupport;
                                        detailModel.PositionId = sheet;
                                        detailModel.POSScale = posScale;
                                        detailModel.ProvinceId = province;
                                        detailModel.RegionNames = region;
                                        detailModel.ChooseImg = chooseImg;
                                        
                                        detailBll.Add(detailModel);
                                        
                                        int detailid = detailModel.Id;
                                        string key = subjectIds.ToString().TrimEnd(',') + "|" + subjectNames.ToString().TrimEnd(',');
                                        if (subjects.ContainsKey(key))
                                        {
                                            List<int> detailIds = subjects[key];
                                            detailIds.Add(detailid);
                                            subjects[key] = detailIds;
                                        }
                                        else
                                        {
                                            List<int> detailIds = new List<int>();
                                            detailIds.Add(detailid);
                                            subjects.Add(key, detailIds);
                                        }
                                        //if (subjects.Count > 0)
                                        //{

                                            

                                        //}
                                        //else
                                        //{


                                        //    List<int> detailIds = new List<int>();
                                        //    detailIds.Add(detailid);
                                        //    subjects.Add(key, detailIds);
                                        //}
                                        successNum++;
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
                                        failNum++;
                                    }
                                   
                                }
                                if (subjects.Count > 0)
                                {
                                    OrderPlanBLL planBll = new OrderPlanBLL();
                                    OrderPlan planModel;
                                    foreach (var item in subjects)
                                    {
                                        string key = item.Key;

                                        planModel = new OrderPlan();
                                        planModel.AddDate = DateTime.Now;
                                        planModel.AddUserId = CurrentUser.UserId;
                                        planModel.BeginDate = DateTime.Parse(beginDate);
                                        planModel.CustomerId = customerId;
                                        planModel.EndDate = DateTime.Parse(endDate);
                                        planModel.PlanType = 2;
                                        planModel.ProjectId = key.Split('|')[0];
                                        planModel.ProjectName = key.Split('|')[1];
                                        planBll.Add(planModel);

                                        List<int> detailIds = item.Value;
                                        if (detailIds.Any())
                                        {
                                            StringBuilder ids = new StringBuilder();
                                            detailIds.ForEach(s =>
                                            {
                                                ids.Append(s);
                                                ids.Append(',');
                                            });
                                            detailBll.ExcuteSql(string.Format("update CheckOrderPlanDetail set PlanId={0} where Id in ({1})", planModel.Id, ids.ToString().TrimEnd(',')));
                                        }

                                    }
                                }
                                if (errorTB.Rows.Count > 0)
                                {
                                    Session["errorTb"] = errorTB;
                                }
                                Response.Redirect(string.Format("ImportCheckPlan.aspx?customerId={0}&successNum={1}&failNum={2}", customerId, successNum, failNum), false);


                            }
                            else
                            {
                                //文件内容为空

                            }
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            Alert("导入失败："+ex.Message);
                        }
                    }
                    
                }
            }
        }

        SubjectBLL subjectBll = new SubjectBLL();
        Dictionary<string, int> subjectDic = new Dictionary<string, int>();
        /// <summary>
        /// 检查活动名称是存在（根据起止时间）
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        bool checkSubject(string subjectName,string beginDate,string endDate,out int subjectId,out string newSubjectName)
        {
            subjectId = 0;
            newSubjectName = string.Empty;
            bool flag = false;
            bool isExest = false;
            foreach (var item in subjectDic.Keys)
            {
                if (StringHelper.ReplaceSpace(item).ToLower() == StringHelper.ReplaceSpace(subjectName).ToLower())
                {
                    isExest = true;
                    flag = true;
                    subjectId = subjectDic[item];
                    newSubjectName = item;
                }
            }
            if (isExest==false)
            {
                DateTime begin = DateTime.Parse(beginDate);
                DateTime end = DateTime.Parse(endDate).AddDays(1);
                var list = subjectBll.GetList(s => s.BeginDate >= begin && s.BeginDate < end && s.CustomerId == customerId && s.CompanyId == CurrentUser.CompanyId);
                
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        if (StringHelper.ReplaceSpace(item.SubjectName).ToLower() == StringHelper.ReplaceSpace(subjectName).ToLower())
                        {
                            subjectId = item.Id;
                            newSubjectName = item.SubjectName;
                            flag = true;
                            subjectDic.Add(item.SubjectName, subjectId);
                            break;
                        }
                    }
                    
                }
            }
            
            return flag;
        }

        protected void lbExportError_Click(object sender, EventArgs e)
        {
            if (Session["errorTb"] != null)
            {
                DataTable dt = (DataTable)Session["errorTb"];
                OperateFile.ExportExcel(dt, "导入失败信息");
            }
        } 


    }
}
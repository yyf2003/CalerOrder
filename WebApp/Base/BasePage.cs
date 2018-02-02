using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using System.Web.Security;
using Common;
using BLL;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Web.UI;
using System.Text;
using DAL;
using System.Data;
using System.Configuration;

namespace WebApp
{
    public class BasePage : System.Web.UI.Page
    {
        public LoginUser CurrentUser
        {
            get
            {
                LoginUser user = null;
                if (User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    FormsAuthenticationTicket ticket = identity.Ticket;
                    string userData = ticket.UserData;
                    //user = (LoginUser)SerializeObject.BinaryToObj(userData);
                    user = JsonConvert.DeserializeObject<LoginUser>(userData);
                }

                return user;
            }
        }

        public string PreviousUrl
        {
            get
            {
                string url = string.Empty;
                if (ViewState["PreviousUrl"] != null)
                    url = ViewState["PreviousUrl"].ToString();
                return url;
            }
            set
            {
                if (ViewState["PreviousUrl"] == null && HttpContext.Current.Request.UrlReferrer != null)
                {
                    ViewState["PreviousUrl"] = HttpContext.Current.Request.UrlReferrer.PathAndQuery;
                }
            }
        }

        public List<Company> MySonCompanyList
        {
            get
            {
                List<Company> list = new CompanyBLL().GetSonCompalyList(CurrentUser.CompanyId);
                return list;
            }
        }

        public GridView gv { get; set; }

        public void LoadRegion(ref DropDownList ddl, int? customerId = null)
        {
            ddl.Items.Clear();
            var list = new RegionBLL().GetList(s => s.Id > 0);
            if (customerId != null && customerId > 0)
            {
                list = list.Where(s => s.CustomerId == customerId).ToList();
            }
            ddl.DataSource = list;
            ddl.DataTextField = "RegionName";
            ddl.DataValueField = "RegionName";
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        public void LoadProvince(ref DropDownList ddl)
        {
            ddl.Items.Clear();
            var list = new PlaceBLL().GetList(s => s.ParentID == 0);
            ddl.DataSource = list;
            ddl.DataTextField = "PlaceName";
            ddl.DataValueField = "ID";
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        /// <summary>
        /// 用户负责区域
        /// </summary>
        public List<string> GetResponsibleRegion
        {
            get
            {
                List<string> list = new List<string>();
                var model = new UserInRegionBLL().GetList(s => s.UserId == CurrentUser.UserId && s.RoleId == CurrentUser.RoleId).FirstOrDefault();
                if (model != null && !string.IsNullOrWhiteSpace(model.RegionId))
                {
                    List<int> regionIdLst = StringHelper.ToIntList(model.RegionId, ',');
                    list = new RegionBLL().GetList(s => regionIdLst.Contains(s.Id)).Select(s => s.RegionName).Distinct().ToList();

                }
                return list;
            }

        }



        /// <summary>
        /// 设置gridview的权限
        /// </summary>
        /// <param name="gv"></param>
        public void SetPromission(GridView gv, params object[] btns)
        {
            string url = Request.FilePath;
            int moduleId = 0;
            var module = new ModuleBLL().GetList(s => s.Url == url).FirstOrDefault();
            if (module != null)
                moduleId = module.Id;
            int roleId = CurrentUser.RoleId;
            var promission = new RoleInModuleBLL().GetList(s => s.RoleId == roleId && s.ModuleId == moduleId).FirstOrDefault();
            if (promission != null)
            {
                List<string> promissionList = promission.OperatePermission.Split('|').ToList();
                //string[] promissionStr = promission.OperatePermission.Split('|');
                //单个用户的权限
                int userId = CurrentUser.UserId;
                var userInModuleList = new UserInModuleBLL().GetList(s => s.UserId == userId && s.RoleId == roleId && s.ModuleId == moduleId).FirstOrDefault();
                if (userInModuleList != null && userInModuleList.OperatePermission != null)
                {
                    List<string> promissionList0 = userInModuleList.OperatePermission.Split('|').ToList();
                    promissionList0.ForEach(s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s) && !promissionList.Contains(s))
                        {
                            promissionList.Add(s);
                        }
                    });
                }

                foreach (string s in promissionList)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (s == "add")
                        {
                            //btnAdd.Visible = true;
                            //if (btnImport != null)
                            //    btnImport.Visible = true;
                            if (btns.Length > 0)
                            {
                                foreach (Button btn in btns)
                                {
                                    if (btn.ID.IndexOf("Add") != -1)
                                        btn.Visible = true;
                                }
                            }
                        }
                        if (s == "edit")
                        {

                            for (int i = 0; i < gv.Columns.Count; i++)
                            {
                                if (gv.Columns[i].HeaderText.Contains("编辑") || gv.Columns[i].HeaderText.Contains("修改") || gv.Columns[i].HeaderText == "审批" || (gv.Columns[i].HeaderText.IndexOf("上传") != -1) || (gv.Columns[i].HeaderText.IndexOf("安装费") != -1) || (gv.Columns[i].HeaderText.IndexOf("权限设置") != -1) || (gv.Columns[i].HeaderText.IndexOf("店铺状态") != -1))
                                {
                                    gv.Columns[i].Visible = true;
                                }
                            }
                        }
                        if (s == "delete")
                        {

                            for (int i = 0; i < gv.Columns.Count; i++)
                            {
                                if (gv.Columns[i].HeaderText == "删除" || gv.Columns[i].HeaderText == "操作")
                                {
                                    gv.Columns[i].Visible = true
                                        ;
                                }
                            }
                        }
                        if (s == "export")
                        {
                            if (btns.Length > 0)
                            {
                                foreach (Button btn in btns)
                                {
                                    if (btn.ID.IndexOf("Export") != -1)
                                        btn.Visible = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public string GetPromissionStr(string pageUrl = null)
        {
            List<string> promissionList = new List<string>();

            string url = !string.IsNullOrWhiteSpace(pageUrl) ? pageUrl : Request.FilePath;
            int moduleId = 0;
            ModuleBLL moduleBll = new ModuleBLL();
            var module = moduleBll.GetList(s => s.Url == url).FirstOrDefault();
            if (module != null)
            {
                moduleId = module.Id;
            }
            int roleId = CurrentUser.RoleId;
            var promission = new RoleInModuleBLL().GetList(s => s.RoleId == roleId && s.ModuleId == moduleId).FirstOrDefault();
            if (promission != null)
            {
                promissionList = promission.OperatePermission.Split('|').ToList();
            }
            int userId = CurrentUser.UserId;
            var userInModuleList = new UserInModuleBLL().GetList(s => s.UserId == userId && s.RoleId == roleId && s.ModuleId == moduleId).FirstOrDefault();
            if (userInModuleList != null && userInModuleList.OperatePermission != null)
            {
                List<string> promissionList0 = userInModuleList.OperatePermission.Split('|').ToList();
                promissionList0.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s) && !promissionList.Contains(s))
                    {
                        promissionList.Add(s);
                    }
                });
            }
            if (promissionList.Any())
                return StringHelper.ListToString(promissionList, "|");
            else
                return "";
        }


        /// <summary>
        /// 开放编辑/删除权限
        /// </summary>
        /// <param name="gv"></param>
        public void SetOPeratePromission(ref GridView gv)
        {
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (gv.Columns[i].HeaderText.Contains("编辑") || gv.Columns[i].HeaderText.Contains("修改") || gv.Columns[i].HeaderText == "审批" || (gv.Columns[i].HeaderText.IndexOf("上传") != -1))
                {
                    gv.Columns[i].Visible = true;
                }
            }
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (gv.Columns[i].HeaderText == "删除" || gv.Columns[i].HeaderText == "操作")
                {
                    gv.Columns[i].Visible = true
                        ;
                }
            }
        }


        public void ExportExcel(string fileName)
        {

        }

        /// <summary>
        /// 绑定客户
        /// </summary>
        /// <param name="ddl"></param>
        protected void BindCustomerList(ref DropDownList ddl)
        {
            List<Models.Customer> cusotmerList = GetMyCustomerList();
            if (cusotmerList.Any())
            {
                ddl.DataSource = cusotmerList;
                ddl.DataTextField = "CustomerName";
                ddl.DataValueField = "Id";
                ddl.DataBind();
            }
            ddl.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        protected void BindCustomerList(ref CheckBoxList cbl)
        {
            List<Models.Customer> cusotmerList = GetMyCustomerList();
            cbl.Items.Clear();
            if (cusotmerList.Any())
            {
                cbl.DataSource = cusotmerList;
                cbl.DataTextField = "CustomerName";
                cbl.DataValueField = "Id";
                cbl.DataBind();
            }

        }

        public List<Models.Customer> GetMyCustomerList()
        {
            List<Models.Customer> cusotmerList = new CustomerBLL().GetList(s => s.IsDelete == null || s.IsDelete == false);
            //客服
            if (CurrentUser.RoleId == 2)
            {
                var myCustomerList = new UserInCustomerBLL().GetList(s => s.UserId == CurrentUser.UserId).Select(s => s.CustomerId);
                cusotmerList = cusotmerList.Where(s => myCustomerList.Contains(s.Id)).ToList();
            }
            else
            {
                //如果是其他角色，先检查有没有负责的客户，
                var myCustomerList = new UserInCustomerBLL().GetList(s => s.UserId == CurrentUser.UserId).Select(s => s.CustomerId);
                if (myCustomerList.Any())
                {
                    cusotmerList = cusotmerList.Where(s => myCustomerList.Contains(s.Id)).ToList();
                }

            }
            return cusotmerList;
        }


        protected void BindMyCustomerList(ref DropDownList ddlCustomer)
        {
            if (CurrentUser != null && CurrentUser.Customers != null && CurrentUser.Customers.Any())
            {
                ddlCustomer.DataSource = CurrentUser.Customers;
                ddlCustomer.DataTextField = "CustomerName";
                ddlCustomer.DataValueField = "CustomerId";
                ddlCustomer.DataBind();
            }
            ddlCustomer.Items.Insert(0, new ListItem("--请选择--", "0"));


        }

        protected void BindMyCustomerList(DropDownList ddlCustomer)
        {
            if (CurrentUser != null && CurrentUser.Customers != null && CurrentUser.Customers.Any())
            {
                ddlCustomer.DataSource = CurrentUser.Customers;
                ddlCustomer.DataTextField = "CustomerName";
                ddlCustomer.DataValueField = "CustomerId";
                ddlCustomer.DataBind();
            }
            else
                ddlCustomer.Items.Insert(0, new ListItem("--请选择--", "0"));


        }

        protected void BindMyCustomerList(ref CheckBoxList cbl)
        {
            cbl.Items.Clear();
            if (CurrentUser != null && CurrentUser.Customers != null && CurrentUser.Customers.Any())
            {
                cbl.DataSource = CurrentUser.Customers;
                cbl.DataTextField = "CustomerName";
                cbl.DataValueField = "CustomerId";
                cbl.DataBind();
            }


        }

        protected void BindCustomerList(DropDownList ddl)
        {
            List<Models.Customer> cusotmerList = GetMyCustomerList();
            if (cusotmerList.Any())
            {
                ddl.DataSource = cusotmerList;
                ddl.DataTextField = "CustomerName";
                ddl.DataValueField = "Id";
                ddl.DataBind();
            }
            else
                ddl.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        /// <summary>
        /// 获取不同客户的区域信息
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="ddl"></param>
        protected void BindRegionByCustomer(int customerId, ref DropDownList ddl)
        {
            ddl.Items.Clear();
            var list = new RegionBLL().GetList(s => s.CustomerId == customerId && (s.IsDelete == null || s.IsDelete == false));
            if (list.Any())
            {
                ddl.DataSource = list;
                ddl.DataTextField = "RegionName";
                ddl.DataValueField = "Id";
                ddl.DataBind();
            }
            ddl.Items.Insert(0, new ListItem("--请选择--", "0"));
        }

        protected void BindRegionByCustomer(int customerId, ref CheckBoxList cbl)
        {
            cbl.Items.Clear();
            var list = new RegionBLL().GetList(s => s.CustomerId == customerId && (s.IsDelete == null || s.IsDelete == false));
            if (list.Any())
            {
                cbl.DataSource = list;
                cbl.DataTextField = "RegionName";
                cbl.DataValueField = "Id";
                cbl.DataBind();

            }

        }

        protected void BindRegionByCustomer1(int customerId, ref CheckBoxList cbl)
        {
            cbl.Items.Clear();
            var list = new RegionBLL().GetList(s => s.CustomerId == customerId && (s.IsDelete == null || s.IsDelete == false));
            if (GetResponsibleRegion.Any())
            {
                list = list.Where(s => GetResponsibleRegion.Contains(s.RegionName)).ToList();
            }
            if (list.Any())
            {

                foreach (var item in list)
                {
                    ListItem li = new ListItem();
                    li.Value = item.RegionName;
                    li.Text = item.RegionName + "&nbsp;";
                    cbl.Items.Add(li);
                }

            }

        }

        /// <summary>
        /// 初始化查询框的值
        /// </summary>
        /// <param name="url"></param>
        /// <param name="container"></param>
        protected void SetSearchVal(string url, Panel container)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                string[] arr = url.Split('&');
                foreach (string s in arr)
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        string key = s.Split('=')[0];
                        string val = s.Split('=')[1];
                        if (!string.IsNullOrWhiteSpace(val))
                            SetControlVal(container, key, val);
                    }
                }
            }
        }


        void SetControlVal(System.Web.UI.Control container, string key, string val)
        {
            foreach (Control control in container.Controls)
            {
                if (control.ID == key)
                {
                    if (control.GetType().Name == "DropDownList" && val != "0")
                    {
                        DropDownList ddl = (DropDownList)control;
                        if (ddl.Items.Count > 0)
                            ddl.SelectedValue = val;
                    }
                    if (control.GetType().Name == "TextBox")
                    {
                        ((TextBox)control).Text = val;
                    }
                }
            }
        }


        public void Alert(string msg, string url = null)
        {
            System.Text.StringBuilder script = new System.Text.StringBuilder();
            string jump = string.Empty;
            if (!string.IsNullOrWhiteSpace(url))
            {
                jump = "window.location='" + url + "';";
            }
            script.AppendFormat("<script>alert('{0}');{1}</script>", msg, jump);
            ClientScript.RegisterClientScriptBlock(GetType(), "al", script.ToString());
        }

        public void LayAlert(string msg)
        {
            System.Text.StringBuilder script = new System.Text.StringBuilder();
            string jump = string.Empty;

            script.AppendFormat("<script>layer.confirm('{0}')</script>", msg);
            ClientScript.RegisterClientScriptBlock(GetType(), "al", script.ToString());
        }

        public void ExcuteJs(string fun, string msg = null)
        {
            string js = "<script>" + fun + "();</script>";
            if (!string.IsNullOrWhiteSpace(msg))
            {
                js = "<script>" + fun + "('" + msg + "');</script>";
            }
            ClientScript.RegisterClientScriptBlock(GetType(), "al", js);
        }


        protected string GetAttachList(string fileCode, int subjectId)
        {
            AttachmentBLL attachBll = new AttachmentBLL();
            var list = attachBll.GetList(s => s.FileCode == fileCode && s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false));
            if (list.Any())
            {
                StringBuilder attach = new StringBuilder();
                list.ForEach(s =>
                {
                    string exten = s.FilePath.Substring(s.FilePath.LastIndexOf('.') + 1);
                    string src = GetSrcString(exten, s.FileType);
                    string path = s.FilePath.Replace("..//", "//");

                    if (src == "" && s.SmallImgUrl == "")
                    {
                        src = "/Image/UpLoadFileType/Others.png";


                    }

                    if (src == "")//图片
                    {

                        src = s.SmallImgUrl.Replace("..//", "//");

                        attach.Append("<div style='width:130px;float:left;height:120px;'><a href='" + path + "' class='showimg' data-fancybox-group='" + fileCode + "'  style='border:0px;'><img style='border:0px;' src='" + src + "' width='100px' height='80px' /></a><br/>");
                    }
                    else//非图片
                    {
                        src = src.Replace("..//", "//");
                        attach.Append("<div style='width:130px;float:left; height:120px;'><img src='" + src + "' width='100px' height='80px' /><br/>");
                    }
                    string downLoadUrl = " ../Handler/Files.ashx?id=" + s.Id + "&type=download";

                    attach.Append("<table width='100%' cellpadding='0' cellspacing='0'><tr><td style='text-align:left;width:50px; height:30px;font-size:12px;border:0px;'><a href='" + downLoadUrl + "' rel='" + s.Title + "' title='" + s.Title + "'>" + (s.Title) + "</a></td></tr></table>");
                    attach.Append("</div>");
                });
                return attach.ToString();
            }
            else
                return "";
        }

        string GetSrcString(string type, string fileType)
        {
            string src = "";
            switch (type)
            {
                case "xls":
                case "xlsx":
                    src = "/image/UpLoadFileType/EXCEL.png";
                    break;
                case "docx":
                case "doc":
                    src = "/image/UpLoadFileType/WORD.png";
                    break;
                case "pptx":
                case "pptm":
                case "ppsx":
                case "ppsm":
                case "pps":
                case "ppt":
                    src = "/image/UpLoadFileType/PPT.png";
                    break;
                case "rar":
                case "zip":
                    src = "/Image/UpLoadFileType/yasuo.png";
                    break;
            }

            return src;
        }

        /// <summary>
        /// 统计订单pop的价格和面积
        /// </summary>
        /// <param name="orderList"></param>
        /// <param name="popPrice"></param>
        /// <param name="popArea"></param>
        //public void StatisticPOPPrice(IEnumerable<FinalOrderDetailTemp> orderList, out decimal popPrice, out decimal popArea)
        //{
        //    popPrice = 0;
        //    popArea = 0;
        //    if (orderList.Any())
        //    {
        //        decimal totalPrice = 0;
        //        decimal totalArea = 0;
        //        string unitName = string.Empty;
        //        orderList.ToList().ForEach(s =>
        //        {
        //            decimal newPOPArea = 0;
        //            if (s.GraphicLength != null && s.GraphicWidth != null)
        //            {
        //                newPOPArea = ((s.GraphicLength ?? 0) * (s.GraphicWidth ?? 0)) / 1000000;
        //            }
        //            if (!string.IsNullOrWhiteSpace(s.GraphicMaterial))
        //            {
        //                unitName = string.Empty;
        //                decimal defaultUnitPrice = GetMaterialPrice(s.GraphicMaterial, out unitName);//这个是材质数据库单价（会变更）
        //                defaultUnitPrice = s.UnitPrice ?? 0;//取下订单时候的单价
        //                if (s.GraphicMaterial.IndexOf("挂轴") != -1)
        //                {
        //                    string material0 = s.GraphicMaterial.Replace("+挂轴", "").Replace("+上挂轴", "").Replace("+下挂轴", "");
        //                    decimal unitPrice0 = GetMaterialPrice(material0, out unitName);
        //                    decimal unitPrice1 = GetMaterialPrice("挂轴", out unitName);
        //                    decimal popPrice0 = newPOPArea * unitPrice0 * (s.Quantity ?? 1);
        //                    decimal popPrice1 = (s.GraphicWidth ?? 0) / 1000 * 2 * unitPrice1 * (s.Quantity ?? 1);
        //                    if (s.GraphicMaterial.IndexOf("上挂轴") != -1 || s.GraphicMaterial.IndexOf("下挂轴") != -1)
        //                    {
        //                        popPrice1 = (s.GraphicWidth ?? 0) / 1000  * unitPrice1 * (s.Quantity ?? 1);
        //                    }
        //                    totalPrice += (popPrice0 + popPrice1);
        //                    totalArea += newPOPArea * (s.Quantity ?? 1);
        //                }
        //                else
        //                {
        //                    //if (s.UnitPrice != null && s.UnitPrice > 0)
        //                    //{

        //                    //    if (unitName == "个")
        //                    //    {
        //                    //        totalPrice += (s.UnitPrice ?? 0) * (s.Quantity ?? 1);
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        totalArea += (s.Area ?? 0) * (s.Quantity ?? 1);
        //                    //        totalPrice += (s.Area ?? 0) * (s.UnitPrice ?? 0) * (s.Quantity ?? 1);
        //                    //    }
        //                    //}
        //                    //else
        //                    //{


        //                    //}
        //                    if (unitName == "个")
        //                    {
        //                        totalPrice += defaultUnitPrice * (s.Quantity ?? 1);
        //                    }
        //                    else
        //                    {
        //                        totalArea += newPOPArea * (s.Quantity ?? 1);
        //                        totalPrice += newPOPArea * defaultUnitPrice * (s.Quantity ?? 1);
        //                    }

        //                }


        //            }
        //        });
        //        popPrice = totalPrice;
        //        popArea = totalArea;
        //    }
        //}


        /// <summary>
        /// 统计订单pop的价格和面积
        /// </summary>
        /// <param name="orderList"></param>
        /// <param name="popPrice"></param>
        /// <param name="popArea"></param>
        public void StatisticPOPTotalPrice(IEnumerable<FinalOrderDetailTemp> orderList, out decimal totalPrice, out decimal totalArea)
        {
            totalPrice = 0;
            totalArea = 0;

            if (orderList.Any())
            {
                decimal price = 0;
                decimal area = 0;
                orderList.ToList().ForEach(s =>
                {

                    decimal width = s.GraphicWidth ?? 0;
                    decimal length = s.GraphicLength ?? 0;
                    string GraphicMaterial = s.GraphicMaterial ?? "";
                    if (width > 0 && length > 0 && !string.IsNullOrWhiteSpace(GraphicMaterial))
                    {
                        price += (s.TotalPrice ?? 0);
                        if (GraphicMaterial.Contains("挂轴"))
                        {

                            if (GraphicMaterial.Contains("+挂轴") || GraphicMaterial.Contains("+上挂轴") || GraphicMaterial.Contains("+下挂轴"))
                            {
                                area += ((width * length) / 1000000) * (s.Quantity ?? 1);
                                if (GraphicMaterial.Contains("+挂轴"))
                                    area += (width / 1000) * 2 * (s.Quantity ?? 1);
                                else
                                    area += (width / 1000) * (s.Quantity ?? 1);
                            }
                            else
                            {
                                area += (width / 1000) * 2 * (s.Quantity ?? 1);
                            }

                        }
                        else
                        {
                            area += ((width * length) / 1000000) * (s.Quantity ?? 1);
                        }
                    }
                });
                totalPrice = price;
                totalArea = area;
            }

        }
        /// <summary>
        /// 统计报价订单pop的价格和面积
        /// </summary>
        /// <param name="orderList"></param>
        /// <param name="popPrice"></param>
        /// <param name="popArea"></param>
        public void StatisticQuotePOPTotalPrice(IEnumerable<QuoteOrderDetail> orderList, out decimal totalPrice, out decimal totalArea)
        {
            totalPrice = 0;
            totalArea = 0;

            if (orderList.Any())
            {
                decimal price = 0;
                decimal area = 0;
                orderList.ToList().ForEach(s =>
                {

                    decimal width = s.GraphicWidth ?? 0;
                    decimal length = s.GraphicLength ?? 0;
                    string GraphicMaterial = s.GraphicMaterial ?? "";
                    if (width > 0 && length > 0 && !string.IsNullOrWhiteSpace(GraphicMaterial))
                    {
                        price += (s.TotalPrice ?? 0);
                        if (GraphicMaterial.Contains("挂轴"))
                        {

                            if (GraphicMaterial.Contains("+挂轴") || GraphicMaterial.Contains("+上挂轴") || GraphicMaterial.Contains("+下挂轴"))
                            {
                                area += ((width * length) / 1000000) * (s.Quantity ?? 1);
                                if (GraphicMaterial.Contains("+挂轴"))
                                    area += (width / 1000) * 2 * (s.Quantity ?? 1);
                                else
                                    area += (width / 1000) * (s.Quantity ?? 1);
                            }
                            else
                            {
                                area += (width / 1000) * 2 * (s.Quantity ?? 1);
                            }

                        }
                        else
                        {
                            area += ((width * length) / 1000000) * (s.Quantity ?? 1);
                        }
                    }
                });
                totalPrice = price;
                totalArea = area;
            }

        }

        /// <summary>
        /// 获取材质价格和单位
        /// </summary>
        Dictionary<string, Dictionary<decimal, string>> priceDic = new Dictionary<string, Dictionary<decimal, string>>();
        public decimal GetMaterialPrice(int priceItemId, string materialName, out string unitName)
        {
            unitName = string.Empty;
            decimal price = 0;
            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (priceDic.Keys.Contains(materialName.ToLower()))
                {
                    Dictionary<decimal, string> dic = priceDic[materialName.ToLower()];
                    int index = 0;
                    foreach (KeyValuePair<decimal, string> item in dic)
                    {
                        if (index == 0)
                        {
                            unitName = item.Value;
                            price = item.Key;
                        }
                    }
                }
                else
                {
                    string name = materialName.Replace("—", "-").Replace("（", "(").Replace("）", ")").ToLower();

                    //var model = (from orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                    //             join customerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                    //             on orderMaterial.CustomerMaterialId equals customerMaterial.Id
                    //             join basicM in CurrentContext.DbContext.BasicMaterial
                    //             on customerMaterial.BasicMaterialId equals basicM.Id
                    //             join unit in CurrentContext.DbContext.UnitInfo
                    //             on basicM.UnitId equals unit.Id
                    //             where orderMaterial.OrderMaterialName.ToLower() == name
                    //             select new
                    //             {
                    //                 customerMaterial.Price,
                    //                 unit.UnitName
                    //             }).FirstOrDefault();

                    var model = (from orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                                 join customerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                                 on orderMaterial.BasicMaterialId equals customerMaterial.BasicMaterialId
                                 join basicM in CurrentContext.DbContext.BasicMaterial
                                 on customerMaterial.BasicMaterialId equals basicM.Id
                                 join unit in CurrentContext.DbContext.UnitInfo
                                 on basicM.UnitId equals unit.Id
                                 where orderMaterial.OrderMaterialName.ToLower() == name
                                 && customerMaterial.PriceItemId == priceItemId
                                 select new
                                 {
                                     customerMaterial.Price,
                                     unit.UnitName
                                 }).FirstOrDefault();


                    if (model != null)
                    {
                        price = model.Price ?? 0;
                        unitName = model.UnitName;
                        Dictionary<decimal, string> dic = new Dictionary<decimal, string>();
                        dic.Add(price, unitName);
                        priceDic.Add(materialName.ToLower(), dic);

                    }
                }
            }
            return price;
        }




        /// <summary>
        /// 获取那些没参加拆单的pop的材质单价
        /// </summary>
        /// <param name="materialName"></param>
        /// <returns></returns>
        /// 
        CustomerMaterialInfoBLL materialBll = new CustomerMaterialInfoBLL();
        Dictionary<string, decimal> mpriceDic = new Dictionary<string, decimal>();
        public decimal GetMaterialPrice(string materialName)
        {
            decimal price = 0;
            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (mpriceDic.Keys.Contains(materialName.ToLower()))
                {
                    price = mpriceDic[materialName.ToLower()];
                }
                else
                {
                    string name = materialName.Replace("—", "-").Replace("（", "(").Replace("）", ")").ToLower();
                    //var model = materialBll.GetList(s => s.MaterialName.ToLower() == name).FirstOrDefault();
                    var model = (from orderMaterial in CurrentContext.DbContext.OrderMaterialMpping
                                 join customerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                                 on orderMaterial.CustomerMaterialId equals customerMaterial.Id
                                 where orderMaterial.OrderMaterialName.ToLower() == name
                                 select new
                                 {
                                     customerMaterial.Price
                                 }).FirstOrDefault();
                    if (model != null)
                    {
                        price = model.Price ?? 0;
                        mpriceDic.Add(materialName.ToLower(), price);
                    }
                }
            }
            return price;
        }

        /// <summary>
        /// 获取材质单价，并计算总价格
        /// </summary>
        /// <param name="materialName"></param>
        /// <param name="totalPrice"></param>
        /// <returns></returns>
        public decimal GetMaterialUnitPrice(POP pop, out decimal totalPrice, out string unitName1)
        {
            decimal unitPrice = 0;
            totalPrice = 0;
            unitName1 = string.Empty;
            if (pop != null)
            {
                decimal width = pop.GraphicWidth ?? 0;
                decimal length = pop.GraphicLength ?? 0;
                int quantity = pop.Quantity ?? 1;
                string materialName = pop.GraphicMaterial;
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    materialName = materialName.Replace("—", "-").Replace("（", "(").Replace("）", ")").ToLower();
                    string unitName = string.Empty;
                    if (materialName.Contains("挂轴"))
                    {
                        if (materialName.Contains("+挂轴") || materialName.Contains("+上挂轴") || materialName.Contains("+下挂轴"))
                        {
                            decimal unitPrice1 = GetMaterialPrice(pop.PriceItemId, "挂轴", out unitName);
                            string newMaterial = materialName.Substring(0, materialName.LastIndexOf('+'));
                            unitPrice = GetMaterialPrice(pop.PriceItemId, newMaterial, out unitName);
                            decimal totalPrice0 = 0;
                            if (unitName == "平米")
                            {
                                totalPrice0 = ((width * length) / 1000000) * quantity * unitPrice;
                            }
                            else if (unitName == "米")
                            {
                                totalPrice0 = (width / 1000) * 2 * quantity * unitPrice;
                            }
                            else
                            {
                                totalPrice0 = quantity * unitPrice;
                            }
                            decimal totalPrice1 = (width / 1000) * 2 * quantity * unitPrice1;
                            if (materialName.Contains("+上挂轴") || materialName.Contains("+下挂轴"))
                            {
                                totalPrice1 = (width / 1000) * quantity * unitPrice1;
                            }
                            totalPrice = totalPrice0 + totalPrice1;
                        }
                        else
                        {
                            unitPrice = GetMaterialPrice(pop.PriceItemId, materialName, out unitName);
                            totalPrice = (width / 1000) * 2 * quantity * unitPrice;
                        }
                    }
                    else
                    {
                        unitPrice = GetMaterialPrice(pop.PriceItemId, materialName, out unitName);
                        if (unitName == "平米")
                        {
                            totalPrice = ((width * length) / 1000000) * quantity * unitPrice;
                        }
                        else if (unitName == "米")
                        {
                            totalPrice = (width / 1000) * 2 * quantity * unitPrice;
                        }
                        else
                        {
                            totalPrice = quantity * unitPrice;
                        }
                    }
                    unitName1 = unitName;
                }
            }
            return unitPrice;

        }


        /// <summary>
        /// 按照物料级别获取安装费
        /// </summary>
        /// <param name="MaterialSupport"></param>
        /// <returns></returns>
        /// 
        //List<InstallPriceConfig> installConfigList = new List<InstallPriceConfig>();
        List<NewMaterialSupport> installConfigList = new List<NewMaterialSupport>();
        /// <summary>
        /// 基本安装费
        /// </summary>
        /// <param name="MaterialSupport"></param>
        /// <returns></returns>
        public decimal GetBasicInstallPrice(string MaterialSupport)
        {
            decimal BasicInstallPrice = 0;
            if (string.IsNullOrWhiteSpace(MaterialSupport))
            {
                MaterialSupport = "Basic";
            }
            if (!installConfigList.Any())
            {
                LoadMaterialSupportInstallPrice();
            }
            var price = installConfigList.Where(s => s.NewMaterialSupportName.ToLower().Contains(MaterialSupport.ToLower())).FirstOrDefault();
            if (price != null)
            {
                BasicInstallPrice = price.BasicInstallPrice;
            }
            else
            {
                var price1 = installConfigList.Where(s => s.NewMaterialSupportName.ToLower().Contains("others")).FirstOrDefault();
                if (price1 != null)
                {
                    BasicInstallPrice = price1.BasicInstallPrice;
                }
            }
            return BasicInstallPrice;
        }



        /// <summary>
        /// 外协基本安装费
        /// </summary>
        /// <param name="MaterialSupport"></param>
        /// <returns></returns>
        public decimal GetOutsourceBasicInstallPrice(string MaterialSupport)
        {
            decimal BasicInstallPrice = 0;
            if (string.IsNullOrWhiteSpace(MaterialSupport))
            {
                MaterialSupport = "Basic";
            }
            if (!installConfigList.Any())
            {
                LoadMaterialSupportInstallPrice();
            }
            var price = installConfigList.Where(s => s.NewMaterialSupportName.ToLower().Contains(MaterialSupport.ToLower())).FirstOrDefault();
            if (price != null)
            {
                BasicInstallPrice = price.OutsourceInstallPrice;
            }
            else
            {
                var price1 = installConfigList.Where(s => s.NewMaterialSupportName.ToLower().Contains("others")).FirstOrDefault();
                if (price1 != null)
                {
                    BasicInstallPrice = price1.OutsourceInstallPrice;
                }
            }
            return BasicInstallPrice;
        }
        /// <summary>
        /// 橱窗安装费
        /// </summary>
        /// <param name="MaterialSupport"></param>
        /// <returns></returns>
        public decimal GetWindowInstallPrice(string MaterialSupport)
        {
            if (string.IsNullOrWhiteSpace(MaterialSupport))
                MaterialSupport = "Basic";
            decimal WindowInstallPrice = 0;
            if (!installConfigList.Any())
            {
                LoadMaterialSupportInstallPrice();
            }
            var price = installConfigList.Where(s => s.NewMaterialSupportName.ToLower().Contains(MaterialSupport.ToLower())).FirstOrDefault();
            if (price != null)
            {
                WindowInstallPrice = price.WindowInstallPrice;
            }
            else
            {
                var price1 = installConfigList.Where(s => s.NewMaterialSupportName.ToLower().Contains("others")).FirstOrDefault();
                if (price1 != null)
                {
                    WindowInstallPrice = price1.WindowInstallPrice;
                }
            }
            return WindowInstallPrice;
        }

        void LoadMaterialSupportInstallPrice()
        {
            installConfigList.Clear();
            var list = (from ms in CurrentContext.DbContext.NewMaterialSupport
                        join basic in CurrentContext.DbContext.BasicMaterialSupport
                        on ms.BasicMaterialSupportId equals basic.Id
                        join price in CurrentContext.DbContext.InstallPriceConfig
                        on basic.Id equals price.BasicMaterialSupportId
                        select new
                        {
                            ms,
                            price
                        }).ToList();
            if (list.Any())
            {
                list.ForEach(s =>
                {
                    NewMaterialSupport model = new NewMaterialSupport();
                    model.BasicInstallPrice = s.price.BasicInstallPrice ?? 0;
                    model.NewMaterialSupportName = s.ms.NewMaterialSupportName;
                    model.WindowInstallPrice = s.price.WindowInstallPrice ?? 0;
                    model.OutsourceInstallPrice = s.price.OutsourceBasicInstallPrice ?? 0;
                    installConfigList.Add(model);
                });
            }
        }

        /// <summary>
        /// 检查器架是否存在
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="sheet"></param>
        /// <param name="machineFrameName"></param>
        /// <returns></returns>
        protected bool CheckShopMachineFrame(int shopId, string sheet, string machineFrameName, string gender)
        {
            var list = new ShopMachineFrameBLL().GetList(s => s.ShopId == shopId && s.PositionName.ToLower() == sheet.ToLower() && s.MachineFrame.ToLower() == machineFrameName.ToLower() && (s.Gender == gender || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女"))));
            return list.Any();
        }

        public string CreateSubjectNo()
        {
            System.Text.StringBuilder code = new System.Text.StringBuilder();
            DateTime date = DateTime.Now;
            code.Append(date.Year).Append(date.Month.ToString().PadLeft(2, '0')).Append(date.Day.ToString().PadLeft(2, '0')).Append(date.Hour.ToString().PadLeft(2, '0')).Append(date.Minute.ToString().PadLeft(2, '0')).Append(date.Second.ToString().PadLeft(2, '0'));
            return code.ToString();
        }

        /// <summary>
        /// 正常单检查物料支持级别
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="errorTB"></param>
        /// <returns></returns>
        public bool IsMaterialSupportOKInSuject(int subjectId, out DataTable errorTB)
        {
            bool canSearch = false;
            bool flag = true;
            errorTB = new DataTable();
            var guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on subject.GuidanceId equals guidance.ItemId
                                 where subject.Id == subjectId
                                 select guidance).FirstOrDefault();
            if (guidanceModel != null && guidanceModel.ActivityTypeId != null && guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install)
            {
                List<MergeOriginalOrder> mergeOrderList = new List<MergeOriginalOrder>();
                MergeOriginalOrderBLL orderBll = new MergeOriginalOrderBLL();
                var orderList = orderBll.GetList(s => s.SubjectId == subjectId && s.RegionName != null && s.RegionName.ToLower() == "north").Select(s => new { s, MaterialSupport = s.MaterialSupport.ToUpper() });
                if (orderList.Any())
                {
                    canSearch = true;
                    var shopList1 = (from order in orderList
                                     group order by new
                                     {
                                         order.s.ShopId,
                                         order.MaterialSupport
                                     } into temp
                                     select new
                                     {
                                         temp.Key.ShopId,
                                         temp.Key.MaterialSupport
                                     }).ToList();
                    var shopList = from shop in shopList1
                                   group shop by shop.ShopId
                                       into g
                                       select new
                                       {
                                           ShopId = g.Key,
                                           count = g.Count()
                                       };
                    List<int> shopIdList = shopList.Where(s => s.count > 1).Select(s => s.ShopId ?? 0).ToList();
                    if (shopIdList.Any())
                    {
                        flag = false;
                        mergeOrderList = orderList.Where(s => shopIdList.Contains(s.s.ShopId ?? 0)).Select(s => s.s).OrderBy(s => s.ShopId).ToList();
                        errorTB.Columns.Add(new DataColumn("店铺编号", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("店铺名称", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("区域", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("省份", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("城市", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("店铺规模大小", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("物料支持", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP位置", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("GraphicNo", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("性别", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("数量", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP宽", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP高", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("表位置", Type.GetType("System.String")));
                        foreach (var order in mergeOrderList)
                        {
                            DataRow dr1 = errorTB.NewRow();
                            dr1["店铺编号"] = order.ShopNo;
                            dr1["店铺名称"] = order.ShopName;
                            dr1["区域"] = order.RegionName;
                            dr1["省份"] = order.ProvinceName;
                            dr1["城市"] = order.CityName;
                            dr1["店铺规模大小"] = order.POSScale;
                            dr1["物料支持"] = order.MaterialSupport;
                            dr1["POP位置"] = order.Sheet;
                            dr1["GraphicNo"] = order.GraphicNo;
                            dr1["性别"] = order.Gender;
                            dr1["数量"] = order.Quantity;
                            dr1["POP宽"] = order.GraphicWidth;
                            dr1["POP高"] = order.GraphicLength;
                            if (!string.IsNullOrWhiteSpace(order.GraphicNo))
                                dr1["表位置"] = "POP";
                            else
                                dr1["表位置"] = "List";
                            errorTB.Rows.Add(dr1);
                        }
                    }
                }
                if (flag && canSearch)
                {
                    //如果单个项目没问题就检查整个活动
                    var orderList1 = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                      join subject in CurrentContext.DbContext.Subject
                                      on order.SubjectId equals subject.Id
                                      where subject.GuidanceId == guidanceModel.ItemId
                                      && order.RegionName != null && order.RegionName.ToLower() == "north"
                                      && (subject.IsDelete == null || subject.IsDelete == false)
                                      select new
                                      {
                                          order,
                                          subject.SubjectName,
                                          MaterialSupport = order.MaterialSupport.ToUpper()
                                      }).ToList();
                    if (orderList1.Any())
                    {
                        var shopList1 = (from order in orderList1
                                         group order by new
                                         {
                                             order.order.ShopId,
                                             order.MaterialSupport
                                         } into temp
                                         select new
                                         {
                                             temp.Key.ShopId,
                                             temp.Key.MaterialSupport
                                         }).ToList();
                        var shopList = from shop in shopList1
                                       group shop by shop.ShopId
                                           into g
                                           select new
                                           {
                                               ShopId = g.Key,
                                               count = g.Count()
                                           };
                        List<int> shopIdList = shopList.Where(s => s.count > 1).Select(s => s.ShopId ?? 0).ToList();
                        if (shopIdList.Any())
                        {
                            flag = false;
                            var mergeOrderList1 = orderList1.Where(s => shopIdList.Contains(s.order.ShopId ?? 0)).Select(s => new { s.order, s.SubjectName }).OrderBy(s => s.order.ShopId).ToList();
                            errorTB.Columns.Add(new DataColumn("店铺编号", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("店铺名称", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("区域", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("省份", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("城市", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("店铺规模大小", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("物料支持", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("POP位置", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("GraphicNo", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("性别", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("数量", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("POP宽", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("POP高", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("表位置", Type.GetType("System.String")));
                            errorTB.Columns.Add(new DataColumn("项目名称", Type.GetType("System.String")));
                            foreach (var order in mergeOrderList1)
                            {
                                DataRow dr1 = errorTB.NewRow();
                                dr1["店铺编号"] = order.order.ShopNo;
                                dr1["店铺名称"] = order.order.ShopName;
                                dr1["区域"] = order.order.RegionName;
                                dr1["省份"] = order.order.ProvinceName;
                                dr1["城市"] = order.order.CityName;
                                dr1["店铺规模大小"] = order.order.POSScale;
                                dr1["物料支持"] = order.order.MaterialSupport;
                                dr1["POP位置"] = order.order.Sheet;
                                dr1["GraphicNo"] = order.order.GraphicNo;
                                dr1["性别"] = order.order.Gender;
                                dr1["数量"] = order.order.Quantity;
                                dr1["POP宽"] = order.order.GraphicWidth;
                                dr1["POP高"] = order.order.GraphicLength;
                                if (!string.IsNullOrWhiteSpace(order.order.GraphicNo))
                                    dr1["表位置"] = "POP";
                                else
                                    dr1["表位置"] = "List";
                                dr1["项目名称"] = order.SubjectName;
                                errorTB.Rows.Add(dr1);
                            }
                        }
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 手工单和上海补单检查物料支持级别
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="errorTB"></param>
        /// <returns></returns>
        public bool IsMaterialSupportOKInHandMakeOrder(int subjectId, out DataTable errorTB)
        {
            bool canSearch = false;
            bool flag = true;
            errorTB = new DataTable();
            var guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on subject.GuidanceId equals guidance.ItemId
                                 where subject.Id == subjectId
                                 select guidance).FirstOrDefault();
            if (guidanceModel != null && guidanceModel.ActivityTypeId != null && guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install)
            {
                //.GetList(s => s.SubjectId == subjectId).Select(s => new { s, MaterialSupport = s.MaterialSupport.ToUpper() });
                //var orderList = new HandMadeOrderDetailBLL().GetList(s => s.SubjectId == subjectId).Select(s => new { s, MaterialSupport = s.MaterialSupport.ToUpper() });
                var orderList = (from s in CurrentContext.DbContext.HandMadeOrderDetail
                                 join shop in CurrentContext.DbContext.Shop
                                 on s.ShopId equals shop.Id
                                 where s.SubjectId == subjectId
                                 && shop.RegionName != null && shop.RegionName.ToLower() == "north"
                                 select new
                                 {
                                     s,
                                     MaterialSupport = s.MaterialSupport.ToUpper()
                                 }).ToList();
                if (orderList.Any())
                {
                    canSearch = true;
                    var shopList1 = (from order in orderList
                                     group order by new
                                     {
                                         order.s.ShopId,
                                         order.MaterialSupport
                                     } into temp
                                     select new
                                     {
                                         temp.Key.ShopId,
                                         temp.Key.MaterialSupport
                                     }).ToList();
                    var shopList = from shop in shopList1
                                   group shop by shop.ShopId
                                       into g
                                       select new
                                       {
                                           ShopId = g.Key,
                                           count = g.Count()
                                       };
                    List<int> shopIdList = shopList.Where(s => s.count > 1).Select(s => s.ShopId ?? 0).ToList();
                    if (shopIdList.Any())
                    {
                        flag = false;
                        //var mergeOrderList = orderList.Where(s => shopIdList.Contains(s.s.ShopId ?? 0)).Select(s => s.s).ToList();
                        var mergeOrderList = (from order in orderList
                                              join shop in CurrentContext.DbContext.Shop
                                              on order.s.ShopId equals shop.Id
                                              where shopIdList.Contains(shop.Id)
                                              select new
                                              {
                                                  order = order.s,
                                                  shop
                                              }).OrderBy(s => s.shop.Id).ToList();
                        errorTB.Columns.Add(new DataColumn("店铺编号", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("店铺名称", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("区域", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("省份", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("城市", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("店铺规模大小", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("物料支持", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP位置", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("性别", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("数量", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP宽", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP高", Type.GetType("System.String")));

                        foreach (var order in mergeOrderList)
                        {
                            DataRow dr1 = errorTB.NewRow();
                            dr1["店铺编号"] = order.shop.ShopNo;
                            dr1["店铺名称"] = order.shop.ShopName;
                            dr1["区域"] = order.shop.RegionName;
                            dr1["省份"] = order.shop.ProvinceName;
                            dr1["城市"] = order.shop.CityName;
                            dr1["店铺规模大小"] = order.order.POSScale;
                            dr1["物料支持"] = order.order.MaterialSupport;
                            dr1["POP位置"] = order.order.Sheet;

                            dr1["性别"] = order.order.Gender;
                            dr1["数量"] = order.order.Quantity;
                            dr1["POP宽"] = order.order.GraphicWidth;
                            dr1["POP高"] = order.order.GraphicLength;

                            errorTB.Rows.Add(dr1);
                        }
                    }
                }
                if (flag && canSearch)
                {
                    //如果单个项目没问题就检查整个活动
                    var orderList1 = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                      join subject in CurrentContext.DbContext.Subject
                                      on order.SubjectId equals subject.Id
                                      where subject.GuidanceId == guidanceModel.ItemId
                                      && (subject.IsDelete == null || subject.IsDelete == false)
                                      && order.RegionName != null && order.RegionName.ToLower() == "north"

                                      select new
                                      {
                                          order,
                                          subject.SubjectName

                                      }).ToList();
                    var handMakeOrderList1 = (from order in CurrentContext.DbContext.HandMadeOrderDetail
                                              join subject in CurrentContext.DbContext.Subject
                                              on order.SubjectId equals subject.Id
                                              join shop in CurrentContext.DbContext.Shop
                                              on order.ShopId equals shop.Id
                                              where subject.GuidanceId == guidanceModel.ItemId
                                              && shop.RegionName != null && shop.RegionName.ToLower() == "north"
                                              && (subject.IsDelete == null || subject.IsDelete == false)
                                              select new
                                              {
                                                  order,
                                                  shop,
                                                  subject.SubjectName

                                              }).ToList();
                    List<MergeOriginalOrder> mergeOrderList = new List<MergeOriginalOrder>();
                    if (orderList1.Any())
                    {
                        orderList1.ForEach(s =>
                        {
                            MergeOriginalOrder model = new MergeOriginalOrder();
                            model.CityName = s.order.CityName;
                            model.Gender = s.order.Gender;
                            model.GraphicLength = s.order.GraphicLength;
                            model.GraphicNo = s.order.GraphicNo;
                            model.GraphicWidth = s.order.GraphicWidth;
                            model.MaterialSupport = s.order.MaterialSupport;
                            model.POSScale = s.order.POSScale;
                            model.ProvinceName = s.order.ProvinceName;
                            model.Quantity = s.order.Quantity;
                            model.RegionName = s.order.RegionName;
                            model.Sheet = s.order.Sheet;
                            model.ShopName = s.order.ShopName;
                            model.ShopNo = s.order.ShopNo;
                            model.SubjectName = s.SubjectName;
                            model.ShopId = s.order.ShopId;
                            mergeOrderList.Add(model);
                        });
                    }
                    if (handMakeOrderList1.Any())
                    {
                        handMakeOrderList1.ForEach(s =>
                        {
                            MergeOriginalOrder model = new MergeOriginalOrder();
                            model.CityName = s.shop.CityName;
                            model.Gender = s.order.Gender;
                            model.GraphicLength = s.order.GraphicLength;
                            model.GraphicNo = "";
                            model.GraphicWidth = s.order.GraphicWidth;
                            model.MaterialSupport = s.order.MaterialSupport;
                            model.POSScale = s.order.POSScale;
                            model.ProvinceName = s.shop.ProvinceName;
                            model.Quantity = s.order.Quantity;
                            model.RegionName = s.shop.RegionName;
                            model.Sheet = s.order.Sheet;
                            model.ShopName = s.shop.ShopName;
                            model.ShopNo = s.shop.ShopNo;
                            model.SubjectName = s.SubjectName;
                            model.ShopId = s.order.ShopId;
                            mergeOrderList.Add(model);
                        });
                    }
                    if (mergeOrderList.Any())
                    {
                        var orderList0 = mergeOrderList.Select(s => new { s, MaterialSupport = s.MaterialSupport.ToUpper() });
                        if (orderList0.Any())
                        {

                            var shopList1 = (from order in orderList0
                                             group order by new
                                             {
                                                 order.s.ShopId,
                                                 order.MaterialSupport
                                             } into temp
                                             select new
                                             {
                                                 temp.Key.ShopId,
                                                 temp.Key.MaterialSupport
                                             }).ToList();
                            var shopList = from shop in shopList1
                                           group shop by shop.ShopId
                                               into g
                                               select new
                                               {
                                                   ShopId = g.Key,
                                                   count = g.Count()
                                               };
                            List<int> shopIdList = shopList.Where(s => s.count > 1).Select(s => s.ShopId ?? 0).ToList();
                            if (shopIdList.Any())
                            {
                                flag = false;
                                var mergeOrderList0 = mergeOrderList.Where(s => shopIdList.Contains(s.ShopId ?? 0)).OrderBy(s => s.ShopId).ToList();

                                errorTB.Columns.Add(new DataColumn("店铺编号", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("店铺名称", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("区域", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("省份", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("城市", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("店铺规模大小", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("物料支持", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("POP位置", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("性别", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("数量", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("POP宽", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("POP高", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("项目名称", Type.GetType("System.String")));
                                foreach (var order in mergeOrderList0)
                                {
                                    DataRow dr1 = errorTB.NewRow();
                                    dr1["店铺编号"] = order.ShopNo;
                                    dr1["店铺名称"] = order.ShopName;
                                    dr1["区域"] = order.RegionName;
                                    dr1["省份"] = order.ProvinceName;
                                    dr1["城市"] = order.CityName;
                                    dr1["店铺规模大小"] = order.POSScale;
                                    dr1["物料支持"] = order.MaterialSupport;
                                    dr1["POP位置"] = order.Sheet;

                                    dr1["性别"] = order.Gender;
                                    dr1["数量"] = order.Quantity;
                                    dr1["POP宽"] = order.GraphicWidth;
                                    dr1["POP高"] = order.GraphicLength;
                                    dr1["项目名称"] = order.SubjectName;
                                    errorTB.Rows.Add(dr1);
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 分区反导检查物料支持级别
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="errorTB"></param>
        /// <returns></returns>
        public bool IsMaterialSupportOKInRegionSupplement(int subjectId, out DataTable errorTB)
        {
            bool canSearch = false;
            bool flag = true;
            errorTB = new DataTable();
            var guidanceModel = (from subject in CurrentContext.DbContext.Subject
                                 join guidance in CurrentContext.DbContext.SubjectGuidance
                                 on subject.GuidanceId equals guidance.ItemId
                                 where subject.Id == subjectId
                                 select guidance).FirstOrDefault();
            if (guidanceModel != null && guidanceModel.ActivityTypeId != null && guidanceModel.ActivityTypeId == (int)GuidanceTypeEnum.Install)
            {
                //.GetList(s => s.SubjectId == subjectId).Select(s => new { s, MaterialSupport = s.MaterialSupport.ToUpper() });
                var orderList = new RegionOrderDetailBLL().GetList(s => s.SubjectId == subjectId && s.OrderType == (int)OrderTypeEnum.POP).Select(s => new { s, MaterialSupport = s.MaterialSupport.ToUpper() });
                if (orderList.Any())
                {
                    canSearch = true;
                    var shopList1 = (from order in orderList
                                     group order by new
                                     {
                                         order.s.ShopId,
                                         order.MaterialSupport
                                     } into temp
                                     select new
                                     {
                                         temp.Key.ShopId,
                                         temp.Key.MaterialSupport
                                     }).ToList();
                    var shopList = from shop in shopList1
                                   group shop by shop.ShopId
                                       into g
                                       select new
                                       {
                                           ShopId = g.Key,
                                           count = g.Count()
                                       };
                    List<int> shopIdList = shopList.Where(s => s.count > 1).Select(s => s.ShopId ?? 0).ToList();
                    if (shopIdList.Any())
                    {
                        flag = false;
                        //var mergeOrderList = orderList.Where(s => shopIdList.Contains(s.s.ShopId ?? 0)).Select(s => s.s).ToList();
                        var supplementOrderList = (from order in orderList
                                                   join shop in CurrentContext.DbContext.Shop
                                                   on order.s.ShopId equals shop.Id
                                                   where shopIdList.Contains(shop.Id)
                                                   select new
                                                   {
                                                       order = order.s,
                                                       shop
                                                   }).OrderBy(s => s.shop.Id).ToList();
                        errorTB.Columns.Add(new DataColumn("店铺编号", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("店铺名称", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("区域", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("省份", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("城市", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("店铺规模大小", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("物料支持", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP位置", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("性别", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("数量", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP宽", Type.GetType("System.String")));
                        errorTB.Columns.Add(new DataColumn("POP高", Type.GetType("System.String")));

                        foreach (var order in supplementOrderList)
                        {
                            DataRow dr1 = errorTB.NewRow();
                            dr1["店铺编号"] = order.shop.ShopNo;
                            dr1["店铺名称"] = order.shop.ShopName;
                            dr1["区域"] = order.shop.RegionName;
                            dr1["省份"] = order.shop.ProvinceName;
                            dr1["城市"] = order.shop.CityName;
                            dr1["店铺规模大小"] = order.order.POSScale;
                            dr1["物料支持"] = order.order.MaterialSupport;
                            dr1["POP位置"] = order.order.Sheet;

                            dr1["性别"] = order.order.Gender;
                            dr1["数量"] = order.order.Quantity;
                            dr1["POP宽"] = order.order.GraphicWidth;
                            dr1["POP高"] = order.order.GraphicLength;

                            errorTB.Rows.Add(dr1);
                        }
                    }
                }
                if (flag && canSearch)
                {
                    //如果单个项目没问题就检查整个活动
                    var orderList1 = (from order in CurrentContext.DbContext.MergeOriginalOrder
                                      join subject in CurrentContext.DbContext.Subject
                                      on order.SubjectId equals subject.Id
                                      where subject.GuidanceId == guidanceModel.ItemId
                                      && (subject.IsDelete == null || subject.IsDelete == false)
                                      select new
                                      {
                                          order,
                                          subject.SubjectName

                                      }).ToList();
                    var supplementOrderList1 = (from order in CurrentContext.DbContext.RegionOrderDetail
                                                join subject in CurrentContext.DbContext.Subject
                                                on order.SubjectId equals subject.Id
                                                join shop in CurrentContext.DbContext.Shop
                                                on order.ShopId equals shop.Id
                                                where subject.GuidanceId == guidanceModel.ItemId
                                                && (subject.IsDelete == null || subject.IsDelete == false)
                                                select new
                                                {
                                                    order,
                                                    shop,
                                                    subject.SubjectName

                                                }).ToList();
                    List<MergeOriginalOrder> mergeOrderList = new List<MergeOriginalOrder>();
                    if (orderList1.Any())
                    {
                        orderList1.ForEach(s =>
                        {
                            MergeOriginalOrder model = new MergeOriginalOrder();
                            model.CityName = s.order.CityName;
                            model.Gender = s.order.Gender;
                            model.GraphicLength = s.order.GraphicLength;
                            model.GraphicNo = s.order.GraphicNo;
                            model.GraphicWidth = s.order.GraphicWidth;
                            model.MaterialSupport = s.order.MaterialSupport ?? string.Empty;
                            model.POSScale = s.order.POSScale;
                            model.ProvinceName = s.order.ProvinceName;
                            model.Quantity = s.order.Quantity;
                            model.RegionName = s.order.RegionName;
                            model.Sheet = s.order.Sheet;
                            model.ShopName = s.order.ShopName;
                            model.ShopNo = s.order.ShopNo;
                            model.SubjectName = s.SubjectName;
                            model.ShopId = s.order.ShopId;
                            mergeOrderList.Add(model);
                        });
                    }
                    if (supplementOrderList1.Any())
                    {
                        supplementOrderList1.ForEach(s =>
                        {
                            MergeOriginalOrder model = new MergeOriginalOrder();
                            model.CityName = s.shop.CityName;
                            model.Gender = s.order.Gender;
                            model.GraphicLength = s.order.GraphicLength;
                            model.GraphicNo = "";
                            model.GraphicWidth = s.order.GraphicWidth;
                            model.MaterialSupport = s.order.MaterialSupport ?? string.Empty;
                            model.POSScale = s.order.POSScale;
                            model.ProvinceName = s.shop.ProvinceName;
                            model.Quantity = s.order.Quantity;
                            model.RegionName = s.shop.RegionName;
                            model.Sheet = s.order.Sheet;
                            model.ShopName = s.shop.ShopName;
                            model.ShopNo = s.shop.ShopNo;
                            model.SubjectName = s.SubjectName;
                            model.ShopId = s.order.ShopId;
                            mergeOrderList.Add(model);
                        });
                    }
                    if (mergeOrderList.Any())
                    {
                        var orderList0 = mergeOrderList.Select(s => new { s, MaterialSupport = s.MaterialSupport.ToUpper() });
                        if (orderList0.Any())
                        {

                            var shopList1 = (from order in orderList0
                                             group order by new
                                             {
                                                 order.s.ShopId,
                                                 order.MaterialSupport
                                             } into temp
                                             select new
                                             {
                                                 temp.Key.ShopId,
                                                 temp.Key.MaterialSupport
                                             }).ToList();
                            var shopList = from shop in shopList1
                                           group shop by shop.ShopId
                                               into g
                                               select new
                                               {
                                                   ShopId = g.Key,
                                                   count = g.Count()
                                               };
                            List<int> shopIdList = shopList.Where(s => s.count > 1).Select(s => s.ShopId ?? 0).ToList();
                            if (shopIdList.Any())
                            {
                                flag = false;
                                var mergeOrderList0 = mergeOrderList.Where(s => shopIdList.Contains(s.ShopId ?? 0)).OrderBy(s => s.ShopId).ToList();

                                errorTB.Columns.Add(new DataColumn("店铺编号", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("店铺名称", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("区域", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("省份", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("城市", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("店铺规模大小", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("物料支持", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("POP位置", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("性别", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("数量", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("POP宽", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("POP高", Type.GetType("System.String")));
                                errorTB.Columns.Add(new DataColumn("项目名称", Type.GetType("System.String")));
                                foreach (var order in mergeOrderList0)
                                {
                                    DataRow dr1 = errorTB.NewRow();
                                    dr1["店铺编号"] = order.ShopNo;
                                    dr1["店铺名称"] = order.ShopName;
                                    dr1["区域"] = order.RegionName;
                                    dr1["省份"] = order.ProvinceName;
                                    dr1["城市"] = order.CityName;
                                    dr1["店铺规模大小"] = order.POSScale;
                                    dr1["物料支持"] = order.MaterialSupport;
                                    dr1["POP位置"] = order.Sheet;

                                    dr1["性别"] = order.Gender;
                                    dr1["数量"] = order.Quantity;
                                    dr1["POP宽"] = order.GraphicWidth;
                                    dr1["POP高"] = order.GraphicLength;
                                    dr1["项目名称"] = order.SubjectName;
                                    errorTB.Rows.Add(dr1);
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 导入订单的时候，检查物料支持级别的填写是否统一，（注：目前只检查北区，西区不检查，如果需要检查西区，要把区域限制去掉）
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="errorTB"></param>
        /// <returns></returns>
        public bool CheckMaterialSupport(int subjectId, out DataTable errorTB)
        {
            errorTB = new DataTable();
            Subject model = new SubjectBLL().GetModel(subjectId);
            if (model != null && model.SubjectType != null)
            {
                if (model.SubjectType == (int)SubjectTypeEnum.手工订单 || model.SubjectType == (int)SubjectTypeEnum.补单)
                {
                    return IsMaterialSupportOKInHandMakeOrder(subjectId, out errorTB);
                }
                if (model.SubjectType == (int)SubjectTypeEnum.POP订单 || model.SubjectType == (int)SubjectTypeEnum.正常单)
                {
                    return IsMaterialSupportOKInSuject(subjectId, out errorTB);
                }
                if (model.SubjectType == (int)SubjectTypeEnum.分区补单)
                {
                    return IsMaterialSupportOKInRegionSupplement(subjectId, out errorTB);
                }
            }
            return true;
        }

        POPBLL popBll = new POPBLL();
        /// <summary>
        /// 检查反导订单，手工单是否录入正确
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="sheet"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool CheckHandMakeOrder(int shopId, string sheet, decimal width, decimal length)
        {
            var list = popBll.GetList(s => s.ShopId == shopId && s.Sheet.ToUpper().Contains(sheet.ToUpper()) && s.GraphicWidth == width && s.GraphicLength == length);
            return list.Any();
        }


        public bool CheckOOH(int shopId, string popNo, decimal width, decimal height)
        {
            var popList = popBll.GetList(s => s.ShopId == shopId && s.Sheet != null && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外"));
            if (popList.Any())
            {
                if (!string.IsNullOrWhiteSpace(popNo))
                {
                    popList = popList.Where(s => s.GraphicNo != null && s.GraphicNo.ToLower() == popNo.ToLower()).ToList();
                }
                else
                {
                    popList = popList.Where(s => s.GraphicWidth == width && s.GraphicLength == height).ToList();
                }
            }
            return popList.Any();
        }


        /// <summary>
        /// 导入订单的时候，自动获取物料支持级别
        /// </summary>
        ShopMaterialSupportBLL shopMaterialSupportBll = new ShopMaterialSupportBLL();
        Dictionary<int, string> shopMaterialSupportDic = new Dictionary<int, string>();
        public string GetShopMaterialSupport(int guidanceId, int shopId)
        {
            string MaterialSupport = string.Empty;
            if (shopMaterialSupportDic.Keys.Contains(shopId))
            {
                MaterialSupport = shopMaterialSupportDic[shopId];
            }
            else
            {
                var model = shopMaterialSupportBll.GetList(s => s.GuidanceId == guidanceId && s.ShopId == shopId).FirstOrDefault();
                if (model != null)
                {
                    MaterialSupport = model.MaterialSupport;
                    shopMaterialSupportDic.Add(shopId, MaterialSupport);
                }
            }
            return MaterialSupport;
        }


        OrderMaterialMppingBLL OrderMaterialMppingBll = new OrderMaterialMppingBLL();
        List<string> materialStrList = new List<string>();
        public bool CheckMaterial(string materialName)
        {

            bool flag = false;
            if (materialStrList.Contains(materialName.ToLower()))
            {
                flag = true;
            }
            else
            {

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
        /// 计算外协订单材质费用
        /// </summary>
        /// <param name="pop"></param>
        /// <param name="totalPrice"></param>
        /// <returns></returns>
        public void GetOutsourceOrderMaterialPrice(POP pop, out decimal unitPrice, out decimal totalPrice)
        {
            unitPrice = 0;
            totalPrice = 0;
            if (pop != null)
            {
                decimal width = pop.GraphicWidth ?? 0;
                decimal length = pop.GraphicLength ?? 0;
                int quantity = pop.Quantity ?? 1;
                string materialName = pop.GraphicMaterial;//此材质已经转换成标准材质（非订单材质）
                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    materialName = materialName.Replace("—", "-").Replace("（", "(").Replace("）", ")").ToLower();
                    string unitName = string.Empty;
                    if (materialName.Contains("挂轴"))
                    {
                        if (materialName.Contains("+挂轴") || materialName.Contains("+上挂轴") || materialName.Contains("+下挂轴"))
                        {
                            decimal unitPrice1 = GetOutsourceMaterialUnitPrice(pop.CustomerId ?? 0, "挂轴", pop.OutsourceType ?? 0, out unitName);
                            string newMaterial = materialName.Substring(0, materialName.LastIndexOf('+'));
                            unitPrice = GetOutsourceMaterialUnitPrice(pop.CustomerId ?? 0, newMaterial, pop.OutsourceType ?? 0, out unitName);
                            decimal totalPrice0 = 0;
                            if (unitName == "平米")
                            {
                                totalPrice0 = ((width * length) / 1000000) * quantity * unitPrice;
                            }
                            else if (unitName == "米")
                            {
                                totalPrice0 = (width / 1000) * 2 * quantity * unitPrice;
                            }
                            else
                            {
                                totalPrice0 = quantity * unitPrice;
                            }
                            decimal totalPrice1 = (width / 1000) * 2 * quantity * unitPrice1;
                            if (materialName.Contains("+上挂轴") || materialName.Contains("+下挂轴"))
                            {
                                totalPrice1 = (width / 1000) * quantity * unitPrice1;
                            }
                            totalPrice = totalPrice0 + totalPrice1;
                        }
                        else
                        {

                            unitPrice = GetOutsourceMaterialUnitPrice(pop.CustomerId ?? 0, materialName, pop.OutsourceType ?? 0, out unitName);
                            totalPrice = (width / 1000) * 2 * quantity * unitPrice;
                        }
                    }
                    else
                    {
                        unitPrice = GetOutsourceMaterialUnitPrice(pop.CustomerId ?? 0, materialName, pop.OutsourceType ?? 0, out unitName);
                        if (unitName == "平米")
                        {
                            totalPrice = ((width * length) / 1000000) * quantity * unitPrice;
                        }
                        else if (unitName == "米")
                        {
                            totalPrice = (width / 1000) * 2 * quantity * unitPrice;
                        }
                        else
                        {
                            totalPrice = quantity * unitPrice;
                        }
                    }
                }
            }
            //return unitPrice;

        }

        /// <summary>
        /// 获取外协材质单价
        /// </summary>
        /// <param name="materialName"></param>
        /// <param name="customerId"></param>
        /// 
        Dictionary<string, Dictionary<decimal, string>> outsourcePriceDic = new Dictionary<string, Dictionary<decimal, string>>();
        decimal GetOutsourceMaterialUnitPrice(int customerId, string materialName, int outsourceType, out string unitName)
        {
            unitName = string.Empty;
            decimal price = 0;
            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (outsourcePriceDic.Keys.Contains(materialName.ToLower()))
                {
                    Dictionary<decimal, string> dic = outsourcePriceDic[materialName.ToLower()];
                    int index = 0;
                    foreach (KeyValuePair<decimal, string> item in dic)
                    {
                        if (index == 0)
                        {
                            unitName = item.Value;
                            price = item.Key;
                        }
                    }
                }
                else
                {
                    string name = materialName.Replace("—", "-").Replace("（", "(").Replace("）", ")").ToLower();
                    var model = (from osMprice in CurrentContext.DbContext.OutsourceMaterialInfo
                                 join basicM in CurrentContext.DbContext.BasicMaterial
                                 on osMprice.BasicMaterialId equals basicM.Id
                                 join unit in CurrentContext.DbContext.UnitInfo
                                 on basicM.UnitId equals unit.Id
                                 join priceItem in CurrentContext.DbContext.OutsourceMaterialPriceItem
                                 on osMprice.PriceItemId equals priceItem.Id
                                 where basicM.MaterialName.ToLower() == name
                                 && (osMprice.IsDelete == null || osMprice.IsDelete == false)
                                 && (osMprice.CustomerId == customerId)
                                 && (priceItem.IsDelete == null || priceItem.IsDelete == false)
                                 select new
                                 {
                                     osMprice.InstallPrice,
                                     osMprice.SendPrice,
                                     unit.UnitName
                                 }).FirstOrDefault();


                    if (model != null)
                    {
                        if (outsourceType == (int)OutsourceOrderTypeEnum.Install)
                        {
                            price = model.InstallPrice ?? 0;
                        }
                        else
                        {
                            price = model.SendPrice ?? 0;
                        }
                        unitName = model.UnitName;
                        Dictionary<decimal, string> dic = new Dictionary<decimal, string>();
                        dic.Add(price, unitName);
                        priceDic.Add(materialName.ToLower(), dic);

                    }
                }
            }
            return price;
        }


        /// <summary>
        /// 获取基础材质，从订单材质对照表找，如果是拆单材质就不需要做这一步
        /// </summary>
        Dictionary<string, string> BasicMaterialDic = new Dictionary<string, string>();
        public string GetBasicMaterial(string orderMaterial)
        {

            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(orderMaterial))
            {
                orderMaterial = orderMaterial.ToLower();
                if (BasicMaterialDic.Keys.Contains(orderMaterial))
                {
                    result = BasicMaterialDic[orderMaterial];
                }
                else
                {
                    var model = (from orderMM in CurrentContext.DbContext.OrderMaterialMpping
                                 //join curstomerMaterial in CurrentContext.DbContext.CustomerMaterialInfo
                                 //on orderMM.CustomerMaterialId equals curstomerMaterial.Id
                                 join basicMaterial in CurrentContext.DbContext.BasicMaterial
                                 on orderMM.BasicMaterialId equals basicMaterial.Id
                                 where orderMM.OrderMaterialName.ToLower() == orderMaterial
                                 select new
                                 {
                                     basicMaterial.MaterialName
                                 }).FirstOrDefault();
                    if (model != null)
                    {
                        result = model.MaterialName;
                        if (!BasicMaterialDic.Keys.Contains(orderMaterial))
                            BasicMaterialDic.Add(orderMaterial, result);
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 项目审批的时候保存安装费(不用)
        /// </summary>
        /// <param name="subjectId"></param>
        public void SaveInstallPrice_old(int guidanceId, int subjectId, int subjectType)
        {

            List<int> shopIdList0 = new List<int>();
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            if (subjectType == (int)SubjectTypeEnum.HC订单 || subjectType == (int)SubjectTypeEnum.分区补单)
            {
                orderList = new FinalOrderDetailTempBLL().GetList(s => s.RegionSupplementId == subjectId && (s.IsDelete == null || s.IsDelete == false) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));
            }
            else
            {
                orderList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId && (s.IsDelete == null || s.IsDelete == false) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));
            }
            if (orderList.Any())
            {
                shopIdList0 = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
            }
            List<string> cityCierList = new List<string>() { "T1", "T2", "T3" };
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        join subject in CurrentContext.DbContext.Subject
                        on order.SubjectId equals subject.Id
                        join shop in CurrentContext.DbContext.Shop
                        on order.ShopId equals shop.Id
                        join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                        on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                        from subjectCategory in categortTemp.DefaultIfEmpty()
                        join pop1 in CurrentContext.DbContext.POP
                        on new { order.ShopId, order.Sheet, order.GraphicNo } equals new { pop1.ShopId, pop1.Sheet, pop1.GraphicNo } into popTemp
                        from pop in popTemp.DefaultIfEmpty()
                        where order.GuidanceId == guidanceId
                        && shopIdList0.Contains(shop.Id)
                        && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && order.BCSIsInstall == "Y"))
                        && (order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0)
                        && (order.IsDelete == null || order.IsDelete == false)
                        && (subject.IsDelete == null || subject.IsDelete == false)
                        && subject.ApproveState == 1
                        && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                        && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                        select new
                        {
                            subject,
                            pop,
                            shop,
                            order,
                            CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                        }).ToList();
            if (list.Any())
            {
                int shopid = 0;
                //int guidanceId = list[0].subject.GuidanceId ?? 0;
                List<int> shopIdList = list.Select(s => s.shop.Id).Distinct().ToList();

                var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);

                //计算基础安装费的店铺Id,所有项目中，相同店铺只算一次
                List<int> basicInstallPriceShop = new List<int>();
                //计算橱窗安装费的店铺Id,所有项目中，相同店铺只算一次
                List<int> windowInstallPriceShop = new List<int>();
                //基础安装费
                decimal basicInstallPrice = 0;
                //橱窗安装费(同一个店，无论有多少个橱窗订单，都算一次)
                decimal windowInstallPrice = 0;
                decimal oohInstallPrice = 0;
                //decimal totalPrice = 0;
                //有橱窗订单店铺
                List<int> windowSheetShopIdList = list.Where(s => (s.order.Sheet != null && (s.order.Sheet.Contains("橱窗") || s.order.Sheet.ToLower() == "window") && (!s.CategoryName.Contains("常规-非活动")))).Select(s => s.shop.Id).Distinct().ToList();

                List<InstallPriceShopInfo> installPriceShopList = new List<InstallPriceShopInfo>();
                InstallPriceShopInfo installPriceShopModel;
                InstallPriceTempBLL installPriceTempBll = new InstallPriceTempBLL();
                InstallPriceTemp installPriceTempModel;
                List<string> BCSInstallCityTierList = new List<string>();
                decimal bcsBasicInstallPrice = 150;//三叶草t1-t3安装费
                if (!BCSInstallCityTierList.Any())
                {
                    string BCSInstallCityTier = string.Empty;
                    try
                    {
                        BCSInstallCityTier = ConfigurationManager.AppSettings["BCSBasicInstallPrice"];
                        if (!string.IsNullOrWhiteSpace(BCSInstallCityTier))
                        {
                            string[] str = BCSInstallCityTier.Split(':');
                            BCSInstallCityTierList = StringHelper.ToStringList(str[0], ',', LowerUpperEnum.ToUpper);
                            bcsBasicInstallPrice = StringHelper.IsDecimal(str[1]);
                        }
                    }
                    catch
                    {

                    }
                }
                list.ForEach(s =>
                {

                    installPriceShopModel = new InstallPriceShopInfo();
                    installPriceShopModel.ShopId = s.shop.Id;
                    installPriceShopModel.MaterialSupport = s.order.MaterialSupport;
                    installPriceShopModel.GuidanceId = guidanceId;
                    decimal basicPrice = 0;
                    decimal windowPrice = 0;
                    bool isvalid = false;
                    //基础安装费
                    if (!basicInstallPriceShop.Contains(s.shop.Id))
                    {
                        basicInstallPriceShop.Add(s.shop.Id);

                        if (s.subject.CornerType == "三叶草")
                        {
                            if ((s.shop.BCSInstallPrice ?? 0) > 0)
                            {
                                basicPrice = (s.shop.BCSInstallPrice ?? 0);
                            }
                            else if (BCSInstallCityTierList.Contains(s.order.CityTier.ToUpper()))
                            {
                                basicPrice = bcsBasicInstallPrice;
                            }
                            else
                            {
                                basicPrice = 0;
                            }
                        }
                        else if (s.CategoryName.Contains("常规-非活动"))
                        {
                            if (BCSInstallCityTierList.Contains(s.order.CityTier.ToUpper()))
                            {
                                basicPrice = bcsBasicInstallPrice;
                            }
                            else
                            {
                                basicPrice = 0;
                            }
                        }
                        else
                        {
                            if ((s.shop.BasicInstallPrice ?? 0) > 0)
                            {
                                basicPrice = (s.shop.BasicInstallPrice ?? 0);
                            }
                            else
                                basicPrice += GetBasicInstallPrice(s.order.InstallPriceMaterialSupport);
                        }
                        installPriceShopModel.BasicPrice = basicPrice;
                        basicInstallPrice += basicPrice;
                        isvalid = true;
                    }
                    //橱窗安装费
                    if (windowSheetShopIdList.Contains(s.shop.Id) && !windowInstallPriceShop.Contains(s.shop.Id))
                    {
                        windowInstallPriceShop.Add(s.shop.Id);
                        windowPrice += GetWindowInstallPrice(s.order.InstallPriceMaterialSupport);
                        windowInstallPrice += windowPrice;
                        installPriceShopModel.WindowPrice = windowPrice;
                    }
                    if (isvalid)
                        installPriceShopList.Add(installPriceShopModel);
                });
                //户外订单(同一个店，如果有2个以上的户外位置订单，按最高算)

                var oohOrderList0 = list.Where(s => (s.order.Sheet != null && (s.order.Sheet == "户外" || s.order.Sheet.ToLower() == "ooh"))).ToList();
                List<int> oohOrderShopIdList = oohOrderList0.Select(s => s.shop.Id).Distinct().ToList();
                oohPOPList = oohPOPList.Where(s => oohOrderShopIdList.Contains(s.ShopId ?? 0)).ToList();
                if (oohOrderList0.Any())
                {
                    Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                    oohOrderList0.ForEach(s =>
                    {
                        decimal price = 0;
                        if (!string.IsNullOrWhiteSpace(s.order.GraphicNo))
                        {
                            price = oohPOPList.Where(p => p.ShopId == s.shop.Id && p.GraphicNo.ToLower() == s.order.GraphicNo.ToLower()).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                        }
                        else
                            price = oohPOPList.Where(p => p.ShopId == s.shop.Id && p.GraphicLength == s.order.GraphicLength && p.GraphicWidth == s.order.GraphicWidth).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                        if (oohPriceDic.Keys.Contains(s.shop.Id))
                        {
                            if (oohPriceDic[s.shop.Id] < price)
                            {
                                oohPriceDic[s.shop.Id] = price;
                            }
                        }
                        else
                            oohPriceDic.Add(s.shop.Id, price);
                    });
                    if (oohPriceDic.Keys.Count > 0)
                    {
                        foreach (KeyValuePair<int, decimal> item in oohPriceDic)
                        {
                            var model0 = installPriceShopList.Where(sh => sh.ShopId == item.Key).FirstOrDefault();
                            if (model0 != null)
                            {
                                int index = installPriceShopList.IndexOf(model0);
                                model0.OOHPrice = item.Value;
                                installPriceShopList[index] = model0;
                                oohInstallPrice += item.Value;
                            }
                        }
                    }
                }


                if (installPriceShopList.Any())
                {
                    installPriceShopList.ForEach(s =>
                    {
                        installPriceTempModel = installPriceTempBll.GetList(i => i.GuidanceId == guidanceId && i.ShopId == s.ShopId).FirstOrDefault();
                        if (installPriceTempModel != null)
                        {
                            installPriceTempModel.BasicPrice = s.BasicPrice;
                            installPriceTempModel.OOHPrice = s.OOHPrice;
                            installPriceTempModel.WindowPrice = s.WindowPrice;
                            installPriceTempModel.TotalPrice = (s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0);
                            installPriceTempModel.AddDate = DateTime.Now;
                            installPriceTempBll.Update(installPriceTempModel);
                        }
                        else
                        {
                            installPriceTempModel = new InstallPriceTemp();
                            installPriceTempModel.GuidanceId = s.GuidanceId;
                            installPriceTempModel.ShopId = s.ShopId;
                            installPriceTempModel.BasicPrice = s.BasicPrice;
                            installPriceTempModel.OOHPrice = s.OOHPrice;
                            installPriceTempModel.WindowPrice = s.WindowPrice;
                            installPriceTempModel.TotalPrice = (s.BasicPrice ?? 0) + (s.OOHPrice ?? 0) + (s.WindowPrice ?? 0);
                            installPriceTempModel.AddDate = DateTime.Now;
                            installPriceTempBll.Add(installPriceTempModel);
                        }
                    });
                }

            }
        }

        /// <summary>
        /// 项目审批的时候保存安装费
        /// </summary>
        public void SaveInstallPrice(int guidanceId, int subjectId, int subjectType)
        {
            InstallPriceTempBLL installPriceTempBll = new InstallPriceTempBLL();

            List<int> shopIdList0 = new List<int>();
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            if (subjectType == (int)SubjectTypeEnum.HC订单 || subjectType == (int)SubjectTypeEnum.分区补单)
            {
                orderList = new FinalOrderDetailTempBLL().GetList(s => s.RegionSupplementId == subjectId && (s.IsDelete == null || s.IsDelete == false));
            }
            else
            {
                orderList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0 && (s.IsDelete == null || s.IsDelete == false));
            }
            if (orderList.Any())
            {
                shopIdList0 = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
            }
            //删除
            installPriceTempBll.Delete(s => s.GuidanceId == guidanceId && shopIdList0.Contains(s.ShopId ?? 0));
            var allOrderlist = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                from subjectCategory in categortTemp.DefaultIfEmpty()
                                where order.GuidanceId == guidanceId
                                && shopIdList0.Contains(order.ShopId ?? 0)
                                && ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && order.BCSIsInstall == "Y"))
                                && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType == (int)OrderTypeEnum.道具)
                                && (order.IsDelete == null || order.IsDelete == false)
                                && (subject.IsDelete == null || subject.IsDelete == false)
                                && subject.ApproveState == 1
                                && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                select new
                                {
                                    subject,
                                    order,
                                    shop,
                                    CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                }).ToList();
            if (allOrderlist.Any())
            {
                //List<string> cityCierList = new List<string>() { "T1", "T2", "T3" };
                List<string> BCSInstallCityTierList = new List<string>();
                decimal bcsBasicInstallPrice = 150;//三叶草t1-t3安装费
                if (!BCSInstallCityTierList.Any())
                {
                    string BCSInstallCityTier = string.Empty;
                    try
                    {
                        BCSInstallCityTier = ConfigurationManager.AppSettings["BCSBasicInstallPrice"];
                        if (!string.IsNullOrWhiteSpace(BCSInstallCityTier))
                        {
                            string[] str = BCSInstallCityTier.Split(':');
                            BCSInstallCityTierList = StringHelper.ToStringList(str[0], ',', LowerUpperEnum.ToUpper);
                            bcsBasicInstallPrice = StringHelper.IsDecimal(str[1]);
                        }
                    }
                    catch
                    {

                    }
                }
                List<Shop> shopList = allOrderlist.Select(s => s.shop).Distinct().ToList();
                List<int> shopIdList = shopList.Select(s => s.Id).ToList();
                var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);

                InstallPriceTemp installPriceTempModel;
                shopList.ForEach(shop =>
                {

                    bool isBCSSubject = true;
                    bool isGeneric = true;
                    //基础安装费
                    decimal basicInstallPrice = 0;
                    //橱窗安装费
                    decimal windowInstallPrice = 0;
                    //OOH安装费
                    decimal oohInstallPrice = 0;
                    string materialSupport = string.Empty;
                    string POSScale = string.Empty;
                    List<string> materialSupportList = new List<string>();
                    var oneShopOrderList = allOrderlist.Where(s => s.order.ShopId == shop.Id).ToList();
                    if (oneShopOrderList.Any())
                    {
                        oneShopOrderList.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(s.order.InstallPriceMaterialSupport.ToLower()))
                            {
                                materialSupportList.Add(s.order.InstallPriceMaterialSupport.ToLower());
                            }
                            if (string.IsNullOrWhiteSpace(POSScale) && !string.IsNullOrWhiteSpace(s.order.InstallPricePOSScale))
                                POSScale = s.order.InstallPricePOSScale;
                            if (s.subject.CornerType != "三叶草")
                                isBCSSubject = false;
                            if (!s.CategoryName.Contains("常规-非活动"))
                                isGeneric = false;
                        });
                        List<FinalOrderDetailTemp> oohOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower() == "ooh" || s.order.Sheet == "户外")).Select(s => s.order).ToList();
                        List<FinalOrderDetailTemp> windowOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower().Contains("橱窗") || s.order.Sheet.ToLower().Contains("window"))).Select(s => s.order).ToList();


                        #region 店内安装费
                        if (isBCSSubject)
                        {
                            if ((shop.BCSInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = (shop.BCSInstallPrice ?? 0);
                            }
                            else if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                            {
                                basicInstallPrice = bcsBasicInstallPrice;
                            }
                            else
                            {
                                basicInstallPrice = 0;
                            }
                        }
                        else if (isGeneric)
                        {
                            if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                            {
                                basicInstallPrice = bcsBasicInstallPrice;
                            }
                            else
                            {
                                basicInstallPrice = 0;
                            }
                        }
                        else
                        {
                            if ((shop.BasicInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = (shop.BasicInstallPrice ?? 0);
                            }
                            else
                            {

                                //按照级别，获取基础安装费
                                materialSupportList.ForEach(ma =>
                                {
                                    decimal basicInstallPrice0 = GetBasicInstallPrice(ma);
                                    if (basicInstallPrice0 > basicInstallPrice)
                                    {
                                        basicInstallPrice = basicInstallPrice0;
                                        materialSupport = ma;
                                    }
                                });
                            }
                        }
                        #endregion
                        #region 橱窗安装
                        if (windowOrderList.Any())
                        {
                            windowInstallPrice = GetWindowInstallPrice(materialSupport);
                        }
                        #endregion
                        #region OOH安装费
                        if (oohOrderList.Any())
                        {

                            oohOrderList.ForEach(s =>
                            {
                                decimal price = 0;
                                if (!string.IsNullOrWhiteSpace(s.GraphicNo))
                                {
                                    price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.GraphicNo.ToLower()).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                                }
                                else
                                    price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.GraphicLength && p.GraphicWidth == s.GraphicWidth).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                                if (price > oohInstallPrice)
                                {
                                    oohInstallPrice = price;
                                }
                            });
                        }
                        #endregion

                        #region 保存安装费
                        installPriceTempModel = new InstallPriceTemp();
                        installPriceTempModel.GuidanceId = guidanceId;
                        installPriceTempModel.ShopId = shop.Id;
                        installPriceTempModel.BasicPrice = basicInstallPrice;
                        installPriceTempModel.OOHPrice = oohInstallPrice;
                        installPriceTempModel.WindowPrice = windowInstallPrice;
                        installPriceTempModel.TotalPrice = basicInstallPrice + oohInstallPrice + windowInstallPrice;
                        installPriceTempModel.AddDate = DateTime.Now;
                        installPriceTempBll.Add(installPriceTempModel);
                        #endregion
                    }
                });
            }


        }


        /// <summary>
        /// 项目审批的时候保存快递费
        /// </summary>
        /// <param name="subjectId"></param>
        public void SaveExpressPrice(int guidanceId, int subjectId, int subjectType)
        {
            ExpressPriceDetailBLL expressPriceBll = new ExpressPriceDetailBLL();
            InstallPriceTempBLL installPriceTempBll = new InstallPriceTempBLL();

            //installPriceTempBll.Delete(s => s.GuidanceId == guidanceId);
            //expressPriceBll.Delete(s => s.GuidanceId == guidanceId);

            List<int> shopIdList0 = new List<int>();
            List<int> windowShopIdList = new List<int>();
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            if (subjectType == (int)SubjectTypeEnum.HC订单 || subjectType == (int)SubjectTypeEnum.分区补单)
            {
                orderList = new FinalOrderDetailTempBLL().GetList(s => s.RegionSupplementId == subjectId && (s.IsDelete == null || s.IsDelete == false) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));
            }
            else
            {
                orderList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0 && (s.IsDelete == null || s.IsDelete == false) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));
            }
            int installSubjectId = 0;
            if (orderList.Any())
            {
                installSubjectId = orderList[0].SubjectId ?? 0;
                var windowOrderList = orderList.Where(s => s.Sheet != null && (s.Sheet.Contains("橱窗") || s.Sheet.Contains("窗贴")) && s.GraphicLength > 1 && s.GraphicWidth > 1 && s.GraphicMaterial != null && s.GraphicMaterial.Contains("全透贴") && s.IsInstall == "Y").ToList();
                if (windowOrderList.Any())
                {
                    windowShopIdList = windowOrderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                }
                shopIdList0 = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                if (windowShopIdList.Any())
                {
                    shopIdList0 = shopIdList0.Except(windowShopIdList).ToList();
                }
            }

            InstallPriceTemp installPriceTempModel;
            if (shopIdList0.Any())
            {
                decimal defaultExpressPrice = 0;
                ExpressPriceConfig model = new ExpressPriceConfigBLL().GetList(s => s.IsDefault == true).FirstOrDefault();
                if (model != null)
                {
                    defaultExpressPrice = model.ReceivePrice ?? 0;
                }
                defaultExpressPrice = defaultExpressPrice > 0 ? defaultExpressPrice : 35;

                ExpressPriceDetail experssPriceModel;
                InstallPriceTemp installPriceTempModel1;
                shopIdList0.ForEach(s =>
                {
                    installPriceTempModel1 = installPriceTempBll.GetList(e => e.GuidanceId == guidanceId && e.ShopId == s && (e.BasicPrice ?? 0) > 0).FirstOrDefault();
                    if (installPriceTempModel1 == null)
                    {
                        experssPriceModel = expressPriceBll.GetList(e => e.GuidanceId == guidanceId && e.ShopId == s).FirstOrDefault();
                        if (experssPriceModel == null)
                        {
                            experssPriceModel = new ExpressPriceDetail();
                            experssPriceModel.GuidanceId = guidanceId;
                            experssPriceModel.ExpressPrice = defaultExpressPrice;
                            experssPriceModel.AddDate = DateTime.Now;
                            experssPriceModel.ShopId = s;
                            expressPriceBll.Add(experssPriceModel);
                        }
                    }
                });
            }
            if (windowShopIdList.Any())
            {


                InstallPriceShopInfoBLL InstallPriceShopInfoBll = new InstallPriceShopInfoBLL();
                InstallPriceShopInfo installPriceShopInfoModel;
                expressPriceBll.Delete(s => windowShopIdList.Contains(s.ShopId ?? 0) && s.GuidanceId == guidanceId);
                windowShopIdList.ForEach(s =>
                {
                    installPriceTempModel = installPriceTempBll.GetList(i => i.ShopId == s && i.GuidanceId == guidanceId).FirstOrDefault();
                    if (installPriceTempModel == null)
                    {
                        installPriceTempModel = new InstallPriceTemp();
                        installPriceTempModel.GuidanceId = guidanceId;
                        installPriceTempModel.ShopId = s;
                        installPriceTempModel.BasicPrice = 150;
                        installPriceTempModel.OOHPrice = 0;
                        installPriceTempModel.WindowPrice = 0;
                        installPriceTempModel.TotalPrice = 150;
                        installPriceTempModel.AddDate = DateTime.Now;
                        installPriceTempBll.Add(installPriceTempModel);
                    }
                    installPriceShopInfoModel = InstallPriceShopInfoBll.GetList(i => i.ShopId == s && i.GuidanceId == guidanceId).FirstOrDefault();
                    if (installPriceShopInfoModel == null)
                    {
                        installPriceShopInfoModel = new InstallPriceShopInfo();
                        installPriceShopInfoModel.GuidanceId = guidanceId;
                        installPriceShopInfoModel.ShopId = s;
                        installPriceShopInfoModel.BasicPrice = 150;
                        installPriceShopInfoModel.OOHPrice = 0;
                        installPriceShopInfoModel.WindowPrice = 0;
                        installPriceShopInfoModel.SubjectId = installSubjectId;
                        InstallPriceShopInfoBll.Add(installPriceShopInfoModel);
                    }
                });
            }
        }

        public void ReSaveExpressPrice(int guidanceId)
        {
            ExpressPriceDetailBLL expressPriceBll = new ExpressPriceDetailBLL();
            InstallPriceTempBLL installPriceTempBll = new InstallPriceTempBLL();

            installPriceTempBll.Delete(s => s.GuidanceId == guidanceId);
            expressPriceBll.Delete(s => s.GuidanceId == guidanceId);

            List<int> shopIdList0 = new List<int>();
            List<int> windowShopIdList = new List<int>();
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            orderList = new FinalOrderDetailTempBLL().GetList(s => s.GuidanceId == guidanceId && (s.IsDelete == null || s.IsDelete == false) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));
            int installSubjectId = 0;
            if (orderList.Any())
            {
                installSubjectId = orderList[0].SubjectId ?? 0;
                var windowOrderList = orderList.Where(s => s.Sheet != null && (s.Sheet.Contains("橱窗") || s.Sheet.Contains("窗贴")) && s.GraphicLength > 1 && s.GraphicWidth > 1 && s.GraphicMaterial != null && s.GraphicMaterial.Contains("全透贴") && s.IsInstall == "Y").ToList();
                if (windowOrderList.Any())
                {
                    windowShopIdList = windowOrderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                }
                shopIdList0 = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
                if (windowShopIdList.Any())
                {
                    shopIdList0 = shopIdList0.Except(windowShopIdList).ToList();
                }
            }

            InstallPriceTemp installPriceTempModel;
            if (shopIdList0.Any())
            {
                decimal defaultExpressPrice = 0;
                ExpressPriceConfig model = new ExpressPriceConfigBLL().GetList(s => s.IsDefault == true).FirstOrDefault();
                if (model != null)
                {
                    defaultExpressPrice = model.ReceivePrice ?? 0;
                }
                defaultExpressPrice = defaultExpressPrice > 0 ? defaultExpressPrice : 35;

                ExpressPriceDetail experssPriceModel;
                InstallPriceTemp installPriceTempModel1;
                shopIdList0.ForEach(s =>
                {
                    installPriceTempModel1 = installPriceTempBll.GetList(e => e.GuidanceId == guidanceId && e.ShopId == s && (e.BasicPrice ?? 0) > 0).FirstOrDefault();
                    if (installPriceTempModel1 == null)
                    {
                        experssPriceModel = expressPriceBll.GetList(e => e.GuidanceId == guidanceId && e.ShopId == s).FirstOrDefault();
                        if (experssPriceModel == null)
                        {
                            experssPriceModel = new ExpressPriceDetail();
                            experssPriceModel.GuidanceId = guidanceId;
                            experssPriceModel.ExpressPrice = defaultExpressPrice;
                            experssPriceModel.AddDate = DateTime.Now;
                            experssPriceModel.ShopId = s;
                            expressPriceBll.Add(experssPriceModel);
                        }
                    }
                });
            }
            if (windowShopIdList.Any())
            {


                InstallPriceShopInfoBLL InstallPriceShopInfoBll = new InstallPriceShopInfoBLL();
                InstallPriceShopInfo installPriceShopInfoModel;
                expressPriceBll.Delete(s => windowShopIdList.Contains(s.ShopId ?? 0) && s.GuidanceId == guidanceId);
                windowShopIdList.ForEach(s =>
                {
                    installPriceTempModel = installPriceTempBll.GetList(i => i.ShopId == s && i.GuidanceId == guidanceId).FirstOrDefault();
                    if (installPriceTempModel == null)
                    {
                        installPriceTempModel = new InstallPriceTemp();
                        installPriceTempModel.GuidanceId = guidanceId;
                        installPriceTempModel.ShopId = s;
                        installPriceTempModel.BasicPrice = 150;
                        installPriceTempModel.OOHPrice = 0;
                        installPriceTempModel.WindowPrice = 0;
                        installPriceTempModel.TotalPrice = 150;
                        installPriceTempModel.AddDate = DateTime.Now;
                        installPriceTempBll.Add(installPriceTempModel);
                    }
                    installPriceShopInfoModel = InstallPriceShopInfoBll.GetList(i => i.ShopId == s && i.GuidanceId == guidanceId).FirstOrDefault();
                    if (installPriceShopInfoModel == null)
                    {
                        installPriceShopInfoModel = new InstallPriceShopInfo();
                        installPriceShopInfoModel.GuidanceId = guidanceId;
                        installPriceShopInfoModel.ShopId = s;
                        installPriceShopInfoModel.BasicPrice = 150;
                        installPriceShopInfoModel.OOHPrice = 0;
                        installPriceShopInfoModel.WindowPrice = 0;
                        installPriceShopInfoModel.SubjectId = installSubjectId;
                        InstallPriceShopInfoBll.Add(installPriceShopInfoModel);
                    }
                });
            }
        }

        public void SaveExpressPriceForDelivery(int guidanceId, int subjectId, int subjectType, decimal? experssPrice = null)
        {
            ExpressPriceDetailBLL expressPriceBll = new ExpressPriceDetailBLL();
            expressPriceBll.Delete(s => s.GuidanceId == guidanceId);
            List<int> shopIdList0 = new List<int>();
            List<int> windowShopIdList = new List<int>();
            List<FinalOrderDetailTemp> orderList = new List<FinalOrderDetailTemp>();
            if (subjectType == (int)SubjectTypeEnum.HC订单 || subjectType == (int)SubjectTypeEnum.分区补单)
            {
                orderList = new FinalOrderDetailTempBLL().GetList(s => s.RegionSupplementId == subjectId && (s.IsDelete == null || s.IsDelete == false) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));
            }
            else
            {
                orderList = new FinalOrderDetailTempBLL().GetList(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0 && (s.IsDelete == null || s.IsDelete == false) && (s.OrderType == (int)OrderTypeEnum.POP || s.OrderType == (int)OrderTypeEnum.道具));
            }

            if (orderList.Any())
            {
                shopIdList0 = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
            }

            if (shopIdList0.Any())
            {
                new InstallPriceTempBLL().Delete(s => s.GuidanceId == guidanceId && shopIdList0.Contains(s.ShopId ?? 0));
                new InstallPriceShopInfoBLL().Delete(s => s.GuidanceId == guidanceId && shopIdList0.Contains(s.ShopId ?? 0));
                decimal defaultExpressPrice = experssPrice ?? 0;
                if (defaultExpressPrice == 0)
                {
                    ExpressPriceConfig model = new ExpressPriceConfigBLL().GetList(s => s.IsDefault == true).FirstOrDefault();
                    if (model != null)
                    {
                        defaultExpressPrice = model.ReceivePrice ?? 0;
                    }
                    defaultExpressPrice = defaultExpressPrice > 0 ? defaultExpressPrice : 35;
                }
                ExpressPriceDetail experssPriceModel;
                shopIdList0.ForEach(s =>
                {
                    experssPriceModel = expressPriceBll.GetList(e => e.GuidanceId == guidanceId && e.ShopId == s).FirstOrDefault();
                    if (experssPriceModel == null)
                    {
                        experssPriceModel = new ExpressPriceDetail();
                        experssPriceModel.GuidanceId = guidanceId;
                        experssPriceModel.ExpressPrice = defaultExpressPrice;
                        experssPriceModel.AddDate = DateTime.Now;
                        experssPriceModel.ShopId = s;
                        expressPriceBll.Add(experssPriceModel);
                    }

                });
            }

        }

        //项目审批同时分外协订单
        public void AutoAssignOutsourceOrder(int subjectId, int subjectType)
        {
            var subjectModel = (from subject in CurrentContext.DbContext.Subject
                                join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                from subjectCategory in categortTemp.DefaultIfEmpty()
                                join gudiance in CurrentContext.DbContext.SubjectGuidance
                                on subject.GuidanceId equals gudiance.ItemId
                                where subject.Id == subjectId
                                select new
                                {
                                    subject,
                                    gudiance,
                                    CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                }).FirstOrDefault();
            if (subjectModel != null)
            {
                int guidanceType = subjectModel.gudiance.ActivityTypeId ?? 0;
                List<FinalOrderDetailTemp> orderList0 = new List<FinalOrderDetailTemp>();
                FinalOrderDetailTempBLL OrderDetailTempBll = new FinalOrderDetailTempBLL();
                OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
                OutsourceOrderDetail outsourceOrderDetailModel;
                if (subjectType == (int)SubjectTypeEnum.HC订单 || subjectType == (int)SubjectTypeEnum.分区补单 || subjectType == (int)SubjectTypeEnum.分区增补 || subjectType == (int)SubjectTypeEnum.新开店订单)
                {
                    orderList0 = OrderDetailTempBll.GetList(s => s.RegionSupplementId == subjectId && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType > 1)) && (s.OrderType != (int)OrderTypeEnum.物料));
                    //删除旧POP订单数据
                    outsourceOrderDetailBll.Delete(s => s.RegionSupplementId == subjectId);
                }
                else
                {
                    orderList0 = OrderDetailTempBll.GetList(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0 && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true) && (s.ShopStatus == null || s.ShopStatus == "" || s.ShopStatus == ShopStatusEnum.正常.ToString()) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType > 1)) && (s.OrderType != (int)OrderTypeEnum.物料));
                    //删除旧POP订单数据
                    outsourceOrderDetailBll.Delete(s => s.SubjectId == subjectId && (s.RegionSupplementId ?? 0) == 0);
                }
                if (orderList0.Any())
                {

                    List<ExpressPriceConfig> expressPriceConfigList = new ExpressPriceConfigBLL().GetList(s => s.Id > 0);
                    ExpressPriceDetailBLL expressPriceDetailBll = new ExpressPriceDetailBLL();
                    ExpressPriceDetail expressPriceDetailModel;

                    List<OutsourceOrderAssignConfig> configList = new OutsourceOrderAssignConfigBLL().GetList(s => s.Id > 0);
                    List<Place> placeList = new PlaceBLL().GetList(s => s.ID > 0);
                    List<int> shopIdList = orderList0.Select(s => s.ShopId ?? 0).ToList();
                    List<Shop> shopList = new ShopBLL().GetList(s => shopIdList.Contains(s.Id)).ToList();
                    List<POP> oohPOPList = new POPBLL().GetList(pop => shopIdList.Contains(pop.ShopId ?? 0) && (pop.Sheet == "户外" || pop.Sheet.ToLower() == "ooh") && (pop.OOHInstallPrice ?? 0) > 0);
                    List<string> BCSCityTierList = new List<string>() { "T1", "T2", "T3" };
                    List<FinalOrderDetailTemp> totalOrderList = OrderDetailTempBll.GetList(s => s.GuidanceId == subjectModel.gudiance.ItemId && shopIdList.Contains(s.ShopId ?? 0) && ((s.OrderType == (int)OrderTypeEnum.POP && s.GraphicLength > 1 && s.GraphicWidth > 1) || (s.OrderType == (int)OrderTypeEnum.道具)) && (s.IsDelete == null || s.IsDelete == false) && (s.IsValid == null || s.IsValid == true));
                    string changePOPCountSheetStr = string.Empty;
                    string beiJingCalerOutsourceName = string.Empty;
                    int calerOutsourceId = 8;
                    List<string> ChangePOPCountSheetList = new List<string>();
                    try
                    {
                        changePOPCountSheetStr = ConfigurationManager.AppSettings["350OrderPOPCount"];
                        beiJingCalerOutsourceName = ConfigurationManager.AppSettings["CalerOutsourceName"];
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrWhiteSpace(changePOPCountSheetStr))
                    {
                        ChangePOPCountSheetList = StringHelper.ToStringList(changePOPCountSheetStr, '|');
                    }
                    if (!string.IsNullOrWhiteSpace(beiJingCalerOutsourceName))
                    {
                        Company companyModel = new CompanyBLL().GetList(s => (s.CompanyName == beiJingCalerOutsourceName || s.ShortName == beiJingCalerOutsourceName) && s.TypeId == (int)CompanyTypeEnum.Outsource).FirstOrDefault();
                        if (companyModel != null)
                            calerOutsourceId = companyModel.Id;
                    }


                    //删除安装费和发货费
                    outsourceOrderDetailBll.Delete(s => s.GuidanceId == subjectModel.gudiance.ItemId && shopIdList.Contains(s.ShopId ?? 0) && s.SubjectId == 0);
                    var assignedList = outsourceOrderDetailBll.GetList(s => s.GuidanceId == subjectModel.gudiance.ItemId && s.SubjectId > 0);
                    shopList.ForEach(shop =>
                    {

                        bool isInstallShop = shop.IsInstall == "Y";
                        //if (shop.Id == 709)
                        //{
                        //int iddd = 0;
                        //}
                        bool hasInstallPrice = false;
                        bool addInstallPrice = false;

                        List<string> materialSupportList = new List<string>();
                        string posScale = string.Empty;
                        decimal promotionInstallPrice = 0;
                        bool hasExpressPrice = subjectModel.gudiance.HasExperssFees ?? true;
                        //bool hasInstallPrice=subjectModel.gudiance.HasInstallFees ?? true;
                        var oneShopOrderList = orderList0.Where(order => order.ShopId == shop.Id).ToList();
                        //去重
                        List<FinalOrderDetailTemp> tempOrderList = new List<FinalOrderDetailTemp>();
                        oneShopOrderList.ForEach(s =>
                        {

                            bool canGo = true;

                            if (!string.IsNullOrWhiteSpace(s.GraphicNo) && !string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                            {
                                //string gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                //去掉重复的（同一个编号下多次的）
                                var checkList = tempOrderList.Where(sl => sl.Sheet == s.Sheet && sl.PositionDescription == s.PositionDescription && sl.GraphicNo == s.GraphicNo && sl.GraphicLength == s.GraphicLength && sl.GraphicWidth == s.GraphicWidth && (sl.Gender == s.Gender || sl.OrderGender == s.OrderGender)).ToList();

                                if (checkList.Any())
                                {
                                    canGo = false;
                                }
                                else
                                {
                                    var oneShopAssignedList = assignedList.Where(assign => assign.ShopId == s.ShopId).ToList();
                                    foreach (var assign in oneShopAssignedList)
                                    {
                                        if (assign.Sheet == s.Sheet && assign.PositionDescription == s.PositionDescription && assign.GraphicNo == s.GraphicNo && assign.GraphicLength == s.GraphicLength && assign.GraphicWidth == s.GraphicWidth && (assign.Gender == s.Gender || assign.Gender == s.OrderGender))
                                        {
                                            canGo = false;
                                            break;
                                        }
                                    }

                                }
                            }
                            if (canGo)
                            {
                                tempOrderList.Add(s);

                            }


                        });
                        oneShopOrderList = tempOrderList;
                        var popList = totalOrderList.Where(s => s.ShopId == shop.Id);
                        //单店全部订单
                        var totalOrderList0 = (from order in popList
                                               join subject in CurrentContext.DbContext.Subject
                                               on order.SubjectId equals subject.Id
                                               join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                              on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                               from subjectCategory in categortTemp.DefaultIfEmpty()
                                               where (subject.IsDelete == null || subject.IsDelete == false)
                                               && subject.ApproveState == 1
                                               && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                               && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                                               && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                                               select new
                                               {
                                                   order,
                                                   subject,
                                                   CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                               }).ToList();
                        bool isBCSSubject = true;
                        bool isGeneric = true;
                        totalOrderList0.ForEach(order =>
                        {
                            if (order.subject.CornerType != "三叶草")
                                isBCSSubject = false;
                            if (!order.CategoryName.Contains("常规-非活动"))
                                isGeneric = false;
                            if (!string.IsNullOrWhiteSpace(order.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(order.order.InstallPriceMaterialSupport.ToLower()))
                            {
                                materialSupportList.Add(order.order.InstallPriceMaterialSupport.ToLower());
                            }
                            if (string.IsNullOrWhiteSpace(posScale))
                                posScale = order.order.InstallPricePOSScale;
                        });
                        int assignType = 0;
                        if (shop.ProvinceName == "内蒙古" && !shop.CityName.Contains("通辽"))
                        {
                            assignType = (int)OutsourceOrderTypeEnum.Install;

                        }
                        else
                        {
                            if (guidanceType == (int)GuidanceTypeEnum.Install)
                            {
                                if (isBCSSubject)
                                {
                                    assignType = shop.BCSIsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                                    isInstallShop = shop.BCSIsInstall == "Y";
                                }
                                else
                                {
                                    assignType = shop.IsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                                }
                            }
                            else
                            {
                                assignType = shop.IsInstall == "Y" ? (int)OutsourceOrderTypeEnum.Install : (int)OutsourceOrderTypeEnum.Send;
                            }
                        }

                        if (!shop.ProvinceName.Contains("北京"))
                        {
                            #region 非北京订单
                            List<int> assignedOrderIdList = new List<int>();
                            #region 按设置好的材质
                            var materialConfig = configList.Where(s => s.ConfigTypeId == (int)OutsourceOrderConfigType.Material).ToList();
                            if (materialConfig.Any())
                            {

                                materialConfig.ForEach(config =>
                                {
                                    OutsourceOrderPlaceConfigBLL placeConfigBll = new OutsourceOrderPlaceConfigBLL();
                                    if (!string.IsNullOrWhiteSpace(config.MaterialName))
                                    {
                                        var orderList = oneShopOrderList.Where(order => !assignedOrderIdList.Contains(order.Id) && order.GraphicMaterial != null && order.GraphicMaterial.ToLower().Contains(config.MaterialName.ToLower()) && (order.OrderType == (int)OrderTypeEnum.POP || order.OrderType == (int)OrderTypeEnum.道具)).ToList();

                                        List<int> cityIdList = new List<int>();

                                        List<string> cityNameList = new List<string>();
                                        bool hasProvince = false;
                                        bool canGo = false;
                                        var placeConfigList = (from placeConfin in CurrentContext.DbContext.OutsourceOrderPlaceConfig
                                                               join place in CurrentContext.DbContext.Place
                                                               on placeConfin.PrivinceId equals place.ID
                                                               where placeConfin.ConfigId == config.Id
                                                               select new
                                                               {
                                                                   placeConfin.CityIds,
                                                                   place.PlaceName
                                                               }).ToList();
                                        if (placeConfigList.Any())
                                        {
                                            hasProvince = true;
                                            placeConfigList.ForEach(pc =>
                                            {
                                                if (pc.PlaceName == shop.ProvinceName)
                                                {
                                                    orderList = orderList.Where(order => order.Province == pc.PlaceName).ToList();
                                                    if (!string.IsNullOrWhiteSpace(pc.CityIds))
                                                    {
                                                        cityIdList = StringHelper.ToIntList(pc.CityIds, ',');
                                                        cityNameList = placeList.Where(p => cityIdList.Contains(p.ID)).Select(p => p.PlaceName).ToList();
                                                        orderList = orderList.Where(order => cityNameList.Contains(order.City)).ToList();
                                                    }
                                                    canGo = true;
                                                }
                                            });
                                        }
                                        if (hasProvince && !canGo)
                                            orderList = new List<FinalOrderDetailTemp>();
                                        if (!string.IsNullOrWhiteSpace(config.Channel))
                                        {
                                            List<string> channelList = StringHelper.ToStringList(config.Channel, ',', LowerUpperEnum.ToLower);
                                            if (channelList.Any())
                                            {
                                                orderList = orderList.Where(order => order.Channel != null && channelList.Contains(order.Channel.ToLower())).ToList();
                                            }
                                        }
                                        if (orderList.Any())
                                        {
                                            orderList.ForEach(order =>
                                            {
                                                string material0 = order.GraphicMaterial;
                                                int Quantity = order.Quantity ?? 1;
                                                if (!string.IsNullOrWhiteSpace(order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(order.Sheet.ToUpper()))
                                                {
                                                    Quantity = Quantity > 0 ? 1 : 0;
                                                }
                                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                                outsourceOrderDetailModel.AgentCode = order.AgentCode;
                                                outsourceOrderDetailModel.AgentName = order.AgentName;
                                                outsourceOrderDetailModel.Area = order.Area;
                                                outsourceOrderDetailModel.BusinessModel = order.BusinessModel;
                                                outsourceOrderDetailModel.Channel = order.Channel;
                                                outsourceOrderDetailModel.ChooseImg = order.ChooseImg;
                                                outsourceOrderDetailModel.City = order.City;
                                                outsourceOrderDetailModel.CityTier = order.CityTier;
                                                outsourceOrderDetailModel.Contact = order.Contact;
                                                outsourceOrderDetailModel.CornerType = order.CornerType;
                                                outsourceOrderDetailModel.Format = order.Format;
                                                outsourceOrderDetailModel.Gender = (order.OrderGender != null && order.OrderGender != "") ? order.OrderGender : order.Gender;
                                                outsourceOrderDetailModel.GraphicLength = order.GraphicLength;
                                                outsourceOrderDetailModel.OrderGraphicMaterial = order.GraphicMaterial;
                                                string material = string.Empty;
                                                if (!string.IsNullOrWhiteSpace(material0))
                                                    material = new BasePage().GetBasicMaterial(material0);
                                                if (string.IsNullOrWhiteSpace(material))
                                                    material = order.GraphicMaterial;
                                                outsourceOrderDetailModel.GraphicMaterial = material;
                                                outsourceOrderDetailModel.GraphicNo = order.GraphicNo;
                                                outsourceOrderDetailModel.GraphicWidth = order.GraphicWidth;
                                                outsourceOrderDetailModel.GuidanceId = order.GuidanceId;
                                                outsourceOrderDetailModel.IsInstall = order.IsInstall;
                                                outsourceOrderDetailModel.BCSIsInstall = order.BCSIsInstall;
                                                outsourceOrderDetailModel.LocationType = order.LocationType;
                                                outsourceOrderDetailModel.MachineFrame = order.MachineFrame;
                                                outsourceOrderDetailModel.MaterialSupport = order.MaterialSupport;
                                                outsourceOrderDetailModel.OrderGender = order.OrderGender;
                                                outsourceOrderDetailModel.OrderType = order.OrderType;
                                                outsourceOrderDetailModel.POPAddress = order.POPAddress;
                                                outsourceOrderDetailModel.POPName = order.POPName;
                                                outsourceOrderDetailModel.POPType = order.POPType;
                                                outsourceOrderDetailModel.PositionDescription = order.PositionDescription;
                                                outsourceOrderDetailModel.POSScale = order.POSScale;
                                                outsourceOrderDetailModel.Province = order.Province;
                                                outsourceOrderDetailModel.Quantity = Quantity;
                                                outsourceOrderDetailModel.Region = order.Region;
                                                outsourceOrderDetailModel.Remark = order.Remark;
                                                outsourceOrderDetailModel.Sheet = order.Sheet;
                                                outsourceOrderDetailModel.ShopId = order.ShopId;
                                                outsourceOrderDetailModel.ShopName = order.ShopName;
                                                outsourceOrderDetailModel.ShopNo = order.ShopNo;
                                                outsourceOrderDetailModel.ShopStatus = order.ShopStatus;
                                                outsourceOrderDetailModel.SubjectId = order.SubjectId;
                                                outsourceOrderDetailModel.Tel = order.Tel;
                                                outsourceOrderDetailModel.TotalArea = order.TotalArea;
                                                outsourceOrderDetailModel.WindowDeep = order.WindowDeep;
                                                outsourceOrderDetailModel.WindowHigh = order.WindowHigh;
                                                outsourceOrderDetailModel.WindowSize = order.WindowSize;
                                                outsourceOrderDetailModel.WindowWide = order.WindowWide;
                                                outsourceOrderDetailModel.ReceiveOrderPrice = order.OrderPrice;
                                                outsourceOrderDetailModel.PayOrderPrice = order.PayOrderPrice;
                                                outsourceOrderDetailModel.InstallPriceMaterialSupport = order.InstallPriceMaterialSupport;
                                                decimal unitPrice = 0;
                                                decimal totalPrice = 0;
                                                if (!string.IsNullOrWhiteSpace(material))
                                                {
                                                    POP pop = new POP();
                                                    pop.GraphicMaterial = material;
                                                    pop.GraphicLength = order.GraphicLength;
                                                    pop.GraphicWidth = order.GraphicWidth;
                                                    pop.Quantity = Quantity;
                                                    pop.CustomerId = subjectModel.subject.CustomerId;
                                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                    new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                    outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                    outsourceOrderDetailModel.TotalPrice = totalPrice;
                                                }
                                                outsourceOrderDetailModel.ReceiveUnitPrice = order.UnitPrice;
                                                outsourceOrderDetailModel.ReceiveTotalPrice = order.TotalPrice;
                                                outsourceOrderDetailModel.RegionSupplementId = order.RegionSupplementId;
                                                outsourceOrderDetailModel.CSUserId = order.CSUserId;
                                                outsourceOrderDetailModel.OutsourceId = config.ProductOutsourctId;
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                outsourceOrderDetailModel.FinalOrderId = order.Id;
                                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                assignedOrderIdList.Add(order.Id);
                                                //assignOrderCount++;
                                            });
                                        }
                                    }
                                });
                            }
                            #endregion

                            #region 其他材质订单
                            var orderList1 = (from order in oneShopOrderList
                                              //join subject in CurrentContext.DbContext.Subject
                                              //on order.SubjectId equals subject.Id
                                              where !assignedOrderIdList.Contains(order.Id)
                                              select new
                                              {
                                                  order
                                                  //subject

                                              }).ToList();
                            if (orderList1.Any())
                            {
                                orderList1.ForEach(s =>
                                {

                                    int Quantity = s.order.Quantity ?? 1;
                                    if (!string.IsNullOrWhiteSpace(s.order.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.order.Sheet.ToUpper()))
                                    {
                                        Quantity = Quantity > 0 ? 1 : 0;
                                    }
                                    string material0 = s.order.GraphicMaterial ?? "";
                                    if (s.order.Province == "天津")
                                    {
                                        if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)//促销或发货
                                        {


                                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                            outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                            outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                            outsourceOrderDetailModel.Area = s.order.Area;
                                            outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                            outsourceOrderDetailModel.Channel = s.order.Channel;
                                            outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                            outsourceOrderDetailModel.City = s.order.City;
                                            outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                            outsourceOrderDetailModel.Contact = s.order.Contact;
                                            outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                            outsourceOrderDetailModel.Format = s.order.Format;
                                            outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                            outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                            outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                            string material = string.Empty;
                                            if (!string.IsNullOrWhiteSpace(material0))
                                                material = new BasePage().GetBasicMaterial(material0);
                                            if (string.IsNullOrWhiteSpace(material))
                                                material = s.order.GraphicMaterial;
                                            outsourceOrderDetailModel.GraphicMaterial = material;
                                            outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                            outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                            outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                            outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                            outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                            outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                            outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                            outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                            outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                            outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                            outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                            outsourceOrderDetailModel.POPName = s.order.POPName;
                                            outsourceOrderDetailModel.POPType = s.order.POPType;
                                            outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                            outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                            outsourceOrderDetailModel.Province = s.order.Province;
                                            outsourceOrderDetailModel.Quantity = Quantity;
                                            outsourceOrderDetailModel.Region = s.order.Region;
                                            outsourceOrderDetailModel.Remark = s.order.Remark;
                                            outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                            outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                            outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                            outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                            outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                            outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                            outsourceOrderDetailModel.Tel = s.order.Tel;
                                            outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                            outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                            outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                            outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                            outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                            outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                            outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                            outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                            decimal unitPrice = 0;
                                            decimal totalPrice = 0;
                                            if (!string.IsNullOrWhiteSpace(material))
                                            {
                                                POP pop = new POP();
                                                pop.GraphicMaterial = material0;
                                                pop.GraphicLength = s.order.GraphicLength;
                                                pop.GraphicWidth = s.order.GraphicWidth;
                                                pop.Quantity = Quantity;
                                                pop.CustomerId = subjectModel.subject.CustomerId;
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                if (guidanceType == (int)GuidanceTypeEnum.Delivery)
                                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
                                                new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                outsourceOrderDetailModel.TotalPrice = totalPrice;
                                            }
                                            outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                            outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                            outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                            outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                            outsourceOrderDetailModel.OutsourceId = calerOutsourceId;//北京卡乐
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.order.Sheet != null && (s.order.Sheet.Contains("橱窗") || s.order.Sheet.Contains("窗贴")) && s.order.GraphicLength > 1 && s.order.GraphicWidth > 1 && material0.Contains("全透贴"))
                                            {

                                                promotionInstallPrice = 150;

                                            }
                                            outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);

                                        }
                                        else
                                        {

                                            string material = string.Empty;
                                            if (!string.IsNullOrWhiteSpace(material0))
                                                material = new BasePage().GetBasicMaterial(material0);
                                            if (string.IsNullOrWhiteSpace(material))
                                                material = s.order.GraphicMaterial ?? string.Empty;


                                            if (material.Contains("背胶PP+") && material.Contains("雪弗板") && !material.Contains("蝴蝶支架"))
                                            {
                                                string material1 = "背胶PP";


                                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                                outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                                outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                                outsourceOrderDetailModel.Area = s.order.Area;
                                                outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                                outsourceOrderDetailModel.Channel = s.order.Channel;
                                                outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                                outsourceOrderDetailModel.City = s.order.City;
                                                outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                                outsourceOrderDetailModel.Contact = s.order.Contact;
                                                outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                                outsourceOrderDetailModel.Format = s.order.Format;
                                                outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                                outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                                outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                                outsourceOrderDetailModel.GraphicMaterial = material1;
                                                outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                                outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                                outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                                outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                                outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                                outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                                outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                                outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                                outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                                outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                                outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                                outsourceOrderDetailModel.POPName = s.order.POPName;
                                                outsourceOrderDetailModel.POPType = s.order.POPType;
                                                outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                                outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                                outsourceOrderDetailModel.Province = s.order.Province;
                                                outsourceOrderDetailModel.Quantity = Quantity;
                                                outsourceOrderDetailModel.Region = s.order.Region;
                                                outsourceOrderDetailModel.Remark = s.order.Remark;
                                                outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                                outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                                outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                                outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                                outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                                outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                                outsourceOrderDetailModel.Tel = s.order.Tel;
                                                outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                                outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                                outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                                outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                                outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                                outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                                outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                                outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                                decimal unitPrice = 0;
                                                decimal totalPrice = 0;
                                                if (!string.IsNullOrWhiteSpace(material1))
                                                {
                                                    POP pop = new POP();
                                                    pop.GraphicMaterial = material1;
                                                    pop.GraphicLength = s.order.GraphicLength;
                                                    pop.GraphicWidth = s.order.GraphicWidth;
                                                    pop.Quantity = Quantity;
                                                    pop.CustomerId = subjectModel.subject.CustomerId;
                                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                    new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                    outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                    outsourceOrderDetailModel.TotalPrice = totalPrice;
                                                }
                                                outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                                outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                                outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                                outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                                outsourceOrderDetailModel.OutsourceId = calerOutsourceId;//北京卡乐
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                                material1 = "3mmPVC";


                                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                                outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                                outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                                outsourceOrderDetailModel.Area = s.order.Area;
                                                outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                                outsourceOrderDetailModel.Channel = s.order.Channel;
                                                outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                                outsourceOrderDetailModel.City = s.order.City;
                                                outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                                outsourceOrderDetailModel.Contact = s.order.Contact;
                                                outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                                outsourceOrderDetailModel.Format = s.order.Format;
                                                outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : s.order.Gender;
                                                outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                                outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                                outsourceOrderDetailModel.GraphicMaterial = material1;
                                                outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                                outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                                outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                                outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                                outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                                outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                                outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                                outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                                outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                                outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                                outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                                outsourceOrderDetailModel.POPName = s.order.POPName;
                                                outsourceOrderDetailModel.POPType = s.order.POPType;
                                                outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                                outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                                outsourceOrderDetailModel.Province = s.order.Province;
                                                outsourceOrderDetailModel.Quantity = Quantity;
                                                outsourceOrderDetailModel.Region = s.order.Region;
                                                outsourceOrderDetailModel.Remark = s.order.Remark;
                                                outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                                outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                                outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                                outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                                outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                                outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                                outsourceOrderDetailModel.Tel = s.order.Tel;
                                                outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                                outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                                outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                                outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                                outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                                outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                                outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                                outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                                decimal unitPrice1 = 0;
                                                decimal totalPrice1 = 0;
                                                if (!string.IsNullOrWhiteSpace(material1))
                                                {
                                                    POP pop = new POP();
                                                    pop.GraphicMaterial = material1;
                                                    pop.GraphicLength = s.order.GraphicLength;
                                                    pop.GraphicWidth = s.order.GraphicWidth;
                                                    pop.Quantity = Quantity;
                                                    pop.CustomerId = subjectModel.subject.CustomerId;
                                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                    new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                                    outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                                    outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                                }
                                                //outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                                //outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                                //不算应收（算作北京）
                                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                                outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                                outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                                outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                                //addInstallPrice = true;

                                            }
                                            else
                                            {



                                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                                outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                                outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                                outsourceOrderDetailModel.Area = s.order.Area;
                                                outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                                outsourceOrderDetailModel.Channel = s.order.Channel;
                                                outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                                outsourceOrderDetailModel.City = s.order.City;
                                                outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                                outsourceOrderDetailModel.Contact = s.order.Contact;
                                                outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                                outsourceOrderDetailModel.Format = s.order.Format;
                                                outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                                                outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                                outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                                //string material = string.Empty;
                                                //if (!string.IsNullOrWhiteSpace(material0))
                                                //    material = new BasePage().GetBasicMaterial(material0);
                                                //if (string.IsNullOrWhiteSpace(material))
                                                //    material = s.order.GraphicMaterial;
                                                outsourceOrderDetailModel.GraphicMaterial = material;
                                                outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                                outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                                outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                                outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                                outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                                outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                                outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                                outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                                outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                                outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                                outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                                outsourceOrderDetailModel.POPName = s.order.POPName;
                                                outsourceOrderDetailModel.POPType = s.order.POPType;
                                                outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                                outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                                outsourceOrderDetailModel.Province = s.order.Province;
                                                outsourceOrderDetailModel.Quantity = Quantity;
                                                outsourceOrderDetailModel.Region = s.order.Region;
                                                outsourceOrderDetailModel.Remark = s.order.Remark;
                                                outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                                outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                                outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                                outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                                outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                                outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                                outsourceOrderDetailModel.Tel = s.order.Tel;
                                                outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                                outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                                outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                                outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                                outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                                outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                                outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                                outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                                decimal unitPrice = 0;
                                                decimal totalPrice = 0;
                                                if (!string.IsNullOrWhiteSpace(material))
                                                {
                                                    POP pop = new POP();
                                                    pop.GraphicMaterial = material;
                                                    pop.GraphicLength = s.order.GraphicLength;
                                                    pop.GraphicWidth = s.order.GraphicWidth;
                                                    pop.Quantity = Quantity;
                                                    pop.CustomerId = subjectModel.subject.CustomerId;
                                                    pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                                    new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice, out totalPrice);
                                                    outsourceOrderDetailModel.UnitPrice = unitPrice;
                                                    outsourceOrderDetailModel.TotalPrice = totalPrice;
                                                }
                                                outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                                outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                                outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                                outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                                outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                                {
                                                    outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                    //if (s.order.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                                    //hasInstallPrice = true;
                                                }
                                                outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);


                                            }
                                        }
                                    }
                                    else
                                    {
                                        //非天津

                                        outsourceOrderDetailModel = new OutsourceOrderDetail();
                                        outsourceOrderDetailModel.AddDate = DateTime.Now;
                                        outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                                        outsourceOrderDetailModel.AgentName = s.order.AgentName;
                                        outsourceOrderDetailModel.Area = s.order.Area;
                                        outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                                        outsourceOrderDetailModel.Channel = s.order.Channel;
                                        outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                                        outsourceOrderDetailModel.City = s.order.City;
                                        outsourceOrderDetailModel.CityTier = s.order.CityTier;
                                        outsourceOrderDetailModel.Contact = s.order.Contact;
                                        outsourceOrderDetailModel.CornerType = s.order.CornerType;
                                        outsourceOrderDetailModel.Format = s.order.Format;
                                        outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : s.order.Gender;
                                        outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                                        outsourceOrderDetailModel.OrderGraphicMaterial = s.order.GraphicMaterial;
                                        string material = string.Empty;
                                        if (!string.IsNullOrWhiteSpace(material0))
                                            material = new BasePage().GetBasicMaterial(material0);
                                        if (string.IsNullOrWhiteSpace(material))
                                            material = s.order.GraphicMaterial;
                                        outsourceOrderDetailModel.GraphicMaterial = material;
                                        outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                                        outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                                        outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                                        outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                                        outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                                        outsourceOrderDetailModel.LocationType = s.order.LocationType;
                                        outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                                        outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                                        outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                                        outsourceOrderDetailModel.OrderType = s.order.OrderType;
                                        outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                                        outsourceOrderDetailModel.POPName = s.order.POPName;
                                        outsourceOrderDetailModel.POPType = s.order.POPType;
                                        outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                                        outsourceOrderDetailModel.POSScale = s.order.POSScale;
                                        outsourceOrderDetailModel.Province = s.order.Province;
                                        outsourceOrderDetailModel.Quantity = Quantity;
                                        outsourceOrderDetailModel.Region = s.order.Region;
                                        outsourceOrderDetailModel.Remark = s.order.Remark;
                                        outsourceOrderDetailModel.Sheet = s.order.Sheet;
                                        outsourceOrderDetailModel.ShopId = s.order.ShopId;
                                        outsourceOrderDetailModel.ShopName = s.order.ShopName;
                                        outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                                        outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                                        outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                                        outsourceOrderDetailModel.Tel = s.order.Tel;
                                        outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                                        outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                                        outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                                        outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                                        outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                                        outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                                        outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                                        outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                                        decimal unitPrice1 = 0;
                                        decimal totalPrice1 = 0;

                                        if (!string.IsNullOrWhiteSpace(material))
                                        {
                                            POP pop = new POP();
                                            pop.GraphicMaterial = material;
                                            pop.GraphicLength = s.order.GraphicLength;
                                            pop.GraphicWidth = s.order.GraphicWidth;
                                            pop.Quantity = Quantity;
                                            pop.CustomerId = subjectModel.subject.CustomerId;
                                            pop.OutsourceType = assignType;
                                            if ((!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south")) || guidanceType == (int)GuidanceTypeEnum.Promotion)
                                            {
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                            }
                                            else if (guidanceType == (int)GuidanceTypeEnum.Delivery)
                                                pop.OutsourceType = (int)OutsourceOrderTypeEnum.Send;
                                            new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                            outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                            outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                        }
                                        outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                                        outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                                        outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                                        outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                                        outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                        outsourceOrderDetailModel.AssignType = assignType;
                                        if (s.order.OrderType == (int)OrderTypeEnum.安装费 || s.order.OrderType == (int)OrderTypeEnum.测量费 || s.order.OrderType == (int)OrderTypeEnum.其他费用)
                                        {
                                            //if (s.order.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                            //hasInstallPrice = true;
                                            if (!string.IsNullOrWhiteSpace(s.order.Region) && (s.order.Region.ToLower() == "east" || s.order.Region.ToLower() == "south"))
                                            {
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                                outsourceOrderDetailModel.PayOrderPrice = 0;
                                            }
                                            else
                                            {
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                            }
                                        }
                                        if (s.order.OrderType == (int)OrderTypeEnum.发货费)
                                        {
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            if (s.order.Province == "内蒙古" && !s.order.City.Contains("通辽"))
                                                outsourceOrderDetailModel.PayOrderPrice = 0;
                                        }
                                        if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                        {
                                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                            if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.order.Sheet != null && (s.order.Sheet.Contains("橱窗") || s.order.Sheet.Contains("窗贴")) && s.order.GraphicLength > 1 && s.order.GraphicWidth > 1 && material0.Contains("全透贴"))
                                            {
                                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                                promotionInstallPrice = 150;
                                            }
                                        }
                                        outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                                        outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                    }
                                    //assignOrderCount++;
                                });
                            }
                            #endregion
                            #endregion
                        }
                        else
                        {
                            #region 北京订单
                            var orderList1 = oneShopOrderList;
                            orderList1.ForEach(s =>
                            {
                                int Quantity = s.Quantity ?? 1;
                                if (!string.IsNullOrWhiteSpace(s.Sheet) && ChangePOPCountSheetList.Any() && ChangePOPCountSheetList.Contains(s.Sheet.ToUpper()))
                                {
                                    Quantity = Quantity > 0 ? 1 : 0;
                                }
                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = s.AgentCode;
                                outsourceOrderDetailModel.AgentName = s.AgentName;
                                outsourceOrderDetailModel.Area = s.Area;
                                outsourceOrderDetailModel.BusinessModel = s.BusinessModel;
                                outsourceOrderDetailModel.Channel = s.Channel;
                                outsourceOrderDetailModel.ChooseImg = s.ChooseImg;
                                outsourceOrderDetailModel.City = s.City;
                                outsourceOrderDetailModel.CityTier = s.CityTier;
                                outsourceOrderDetailModel.Contact = s.Contact;
                                outsourceOrderDetailModel.CornerType = s.CornerType;
                                outsourceOrderDetailModel.Format = s.Format;
                                outsourceOrderDetailModel.Gender = (s.OrderGender != null && s.OrderGender != "") ? s.OrderGender : s.Gender;
                                outsourceOrderDetailModel.GraphicLength = s.GraphicLength;
                                outsourceOrderDetailModel.OrderGraphicMaterial = s.GraphicMaterial;
                                string material = string.Empty;
                                if (!string.IsNullOrWhiteSpace(s.GraphicMaterial))
                                    material = new BasePage().GetBasicMaterial(s.GraphicMaterial);
                                if (string.IsNullOrWhiteSpace(material))
                                    material = s.GraphicMaterial;
                                outsourceOrderDetailModel.GraphicMaterial = s.GraphicMaterial;
                                outsourceOrderDetailModel.GraphicNo = s.GraphicNo;
                                outsourceOrderDetailModel.GraphicWidth = s.GraphicWidth;
                                outsourceOrderDetailModel.GuidanceId = s.GuidanceId;
                                outsourceOrderDetailModel.IsInstall = s.IsInstall;
                                outsourceOrderDetailModel.BCSIsInstall = s.BCSIsInstall;
                                outsourceOrderDetailModel.LocationType = s.LocationType;
                                outsourceOrderDetailModel.MachineFrame = s.MachineFrame;
                                outsourceOrderDetailModel.MaterialSupport = s.MaterialSupport;
                                outsourceOrderDetailModel.OrderGender = s.OrderGender;
                                outsourceOrderDetailModel.OrderType = s.OrderType;
                                outsourceOrderDetailModel.POPAddress = s.POPAddress;
                                outsourceOrderDetailModel.POPName = s.POPName;
                                outsourceOrderDetailModel.POPType = s.POPType;
                                outsourceOrderDetailModel.PositionDescription = s.PositionDescription;
                                outsourceOrderDetailModel.POSScale = s.POSScale;
                                outsourceOrderDetailModel.Province = s.Province;
                                outsourceOrderDetailModel.Quantity = Quantity;
                                outsourceOrderDetailModel.Region = s.Region;
                                outsourceOrderDetailModel.Remark = s.Remark;
                                outsourceOrderDetailModel.Sheet = s.Sheet;
                                outsourceOrderDetailModel.ShopId = s.ShopId;
                                outsourceOrderDetailModel.ShopName = s.ShopName;
                                outsourceOrderDetailModel.ShopNo = s.ShopNo;
                                outsourceOrderDetailModel.ShopStatus = s.ShopStatus;
                                outsourceOrderDetailModel.SubjectId = s.SubjectId;
                                outsourceOrderDetailModel.Tel = s.Tel;
                                outsourceOrderDetailModel.TotalArea = s.TotalArea;
                                outsourceOrderDetailModel.WindowDeep = s.WindowDeep;
                                outsourceOrderDetailModel.WindowHigh = s.WindowHigh;
                                outsourceOrderDetailModel.WindowSize = s.WindowSize;
                                outsourceOrderDetailModel.WindowWide = s.WindowWide;
                                outsourceOrderDetailModel.ReceiveOrderPrice = s.OrderPrice;
                                outsourceOrderDetailModel.PayOrderPrice = s.PayOrderPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = s.InstallPriceMaterialSupport;
                                decimal unitPrice1 = 0;
                                decimal totalPrice1 = 0;
                                if (!string.IsNullOrWhiteSpace(material))
                                {
                                    POP pop = new POP();
                                    pop.GraphicMaterial = material;
                                    pop.GraphicLength = s.GraphicLength;
                                    pop.GraphicWidth = s.GraphicWidth;
                                    pop.Quantity = Quantity;
                                    pop.CustomerId = subjectModel.subject.CustomerId;
                                    pop.OutsourceType = assignType;
                                    if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                        pop.OutsourceType = (int)OutsourceOrderTypeEnum.Install;
                                    new BasePage().GetOutsourceOrderMaterialPrice(pop, out unitPrice1, out totalPrice1);
                                    outsourceOrderDetailModel.UnitPrice = unitPrice1;
                                    outsourceOrderDetailModel.TotalPrice = totalPrice1;
                                }
                                outsourceOrderDetailModel.ReceiveUnitPrice = s.UnitPrice;
                                outsourceOrderDetailModel.ReceiveTotalPrice = s.TotalPrice;
                                outsourceOrderDetailModel.RegionSupplementId = s.RegionSupplementId;
                                outsourceOrderDetailModel.CSUserId = s.CSUserId;
                                outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                outsourceOrderDetailModel.AssignType = assignType;
                                if (s.OrderType == (int)OrderTypeEnum.安装费 || s.OrderType == (int)OrderTypeEnum.测量费 || s.OrderType == (int)OrderTypeEnum.其他费用)
                                {
                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                    //if (s.OrderType == (int)OrderTypeEnum.安装费 && subjectModel.subject.SubjectType != (int)SubjectTypeEnum.费用订单)
                                    //hasInstallPrice = true;

                                }
                                if (s.OrderType == (int)OrderTypeEnum.发货费)
                                {
                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                }
                                if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                                {

                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                                    if (guidanceType == (int)GuidanceTypeEnum.Promotion && s.Sheet != null && (s.Sheet.Contains("橱窗") || s.Sheet.Contains("窗贴")) && s.GraphicLength > 1 && s.GraphicWidth > 1 && s.GraphicMaterial.Contains("全透贴"))
                                    {

                                        outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                        promotionInstallPrice = 150;
                                    }
                                }
                                outsourceOrderDetailModel.FinalOrderId = s.Id;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                //assignOrderCount++;
                            });
                            #endregion
                        }

                        #region 安装费和快递费
                        if (guidanceType == (int)GuidanceTypeEnum.Others)
                        {
                            addInstallPrice = false;
                        }
                        else if (guidanceType == (int)GuidanceTypeEnum.Promotion && assignType == (int)OutsourceOrderTypeEnum.Install)
                        {
                            if (promotionInstallPrice > 0)
                            {
                                addInstallPrice = true;
                                hasExpressPrice = false;
                            }
                            else
                            {
                                addInstallPrice = false;
                            }
                        }
                        else if (guidanceType == (int)GuidanceTypeEnum.Install && (subjectModel.gudiance.HasInstallFees ?? true) && assignType == (int)OutsourceOrderTypeEnum.Install && popList.Any())
                        {
                            addInstallPrice = true;
                        }
                        if (addInstallPrice && !hasInstallPrice && isInstallShop)
                        {
                            decimal receiveInstallPrice = 0;
                            decimal installPrice = 0;
                            string remark = "活动安装费";
                            decimal oohInstallPrice = 0;
                            decimal basicInstallPrice = 0;
                            string materialSupport = string.Empty;
                            if (promotionInstallPrice > 0)
                            {
                                installPrice = promotionInstallPrice;
                                receiveInstallPrice = promotionInstallPrice;
                                remark = "促销窗贴安装费";
                            }
                            else
                            {

                                //按照级别，获取基础安装费
                                //decimal basicInstallPrice = new BasePage().GetOutsourceBasicInstallPrice(materialSupport);

                                materialSupportList.ForEach(ma =>
                                {
                                    decimal basicInstallPrice0 = new BasePage().GetOutsourceBasicInstallPrice(ma);
                                    if (basicInstallPrice0 > basicInstallPrice)
                                    {
                                        basicInstallPrice = basicInstallPrice0;
                                        materialSupport = ma;
                                    }
                                });

                                var oohList = totalOrderList0.Where(s => (s.order.Sheet != null && (s.order.Sheet.Contains("户外") || s.order.Sheet.ToLower() == "ooh"))).ToList();
                                if (oohList.Any())
                                {

                                    Dictionary<int, decimal> oohPriceDic = new Dictionary<int, decimal>();
                                    oohList.ForEach(s =>
                                    {
                                        decimal price = 0;
                                        if (!string.IsNullOrWhiteSpace(s.order.GraphicNo))
                                        {
                                            price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.order.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                        }
                                        else
                                            price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.order.GraphicLength && p.GraphicWidth == s.order.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();
                                        if (price > oohInstallPrice)
                                        {
                                            oohInstallPrice = price;
                                        }
                                    });


                                }

                                //hasExtraInstallPrice = true;
                                //添加安装费

                                InstallPriceTempBLL installShopPriceBll = new InstallPriceTempBLL();
                                var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == subjectModel.gudiance.ItemId && sh.ShopId == shop.Id);
                                if (installShopList.Any())
                                {
                                    installShopList.ForEach(sh =>
                                    {
                                        receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                                    });
                                }
                                if ((shop.OutsourceInstallPrice ?? 0) > 0)
                                {
                                    basicInstallPrice = shop.OutsourceInstallPrice ?? 0;
                                }
                                if (isBCSSubject)
                                {
                                    if ((shop.OutsourceBCSInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = shop.OutsourceBCSInstallPrice ?? 0;
                                    }
                                    else if (BCSCityTierList.Contains(shop.CityTier.ToUpper()))
                                    {
                                        basicInstallPrice = 150;
                                    }
                                    else
                                    {
                                        basicInstallPrice = 0;
                                    }

                                }
                                if (isGeneric)
                                {

                                    if (shop.CityName == "包头市" && (shop.OutsourceInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = shop.OutsourceInstallPrice ?? 0;
                                    }
                                    else if (BCSCityTierList.Contains(shop.CityTier.ToUpper()))
                                    {
                                        basicInstallPrice = 150;
                                    }
                                    else
                                    {
                                        basicInstallPrice = 0;
                                    }
                                }
                                installPrice = oohInstallPrice + basicInstallPrice;
                            }
                            if (installPrice > 0)
                            {

                                if (oohInstallPrice > 0 && (shop.OOHInstallOutsourceId ?? 0) > 0)
                                {
                                    //如果有单独的户外安装外协
                                    outsourceOrderDetailModel = new OutsourceOrderDetail();
                                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                    outsourceOrderDetailModel.AddDate = DateTime.Now;
                                    outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                    outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                    outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                    outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                    outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                    outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                    outsourceOrderDetailModel.Contact = shop.Contact1;
                                    outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                    outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                    outsourceOrderDetailModel.GraphicNo = string.Empty;
                                    outsourceOrderDetailModel.GraphicWidth = 0;
                                    outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                    outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                    outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                    outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                    outsourceOrderDetailModel.MachineFrame = string.Empty;
                                    outsourceOrderDetailModel.MaterialSupport = string.Empty;
                                    outsourceOrderDetailModel.OrderGender = string.Empty;
                                    outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                    outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                    outsourceOrderDetailModel.POPName = string.Empty;
                                    outsourceOrderDetailModel.POPType = string.Empty;
                                    outsourceOrderDetailModel.PositionDescription = string.Empty;
                                    outsourceOrderDetailModel.POSScale = posScale;
                                    outsourceOrderDetailModel.Province = shop.ProvinceName;
                                    outsourceOrderDetailModel.Quantity = 1;
                                    outsourceOrderDetailModel.Region = shop.RegionName;
                                    outsourceOrderDetailModel.Remark = "户外安装费";
                                    outsourceOrderDetailModel.Sheet = string.Empty;
                                    outsourceOrderDetailModel.ShopId = shop.Id;
                                    outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                    outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                    outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                    outsourceOrderDetailModel.SubjectId = 0;
                                    outsourceOrderDetailModel.Tel = shop.Tel1;
                                    outsourceOrderDetailModel.TotalArea = 0;
                                    outsourceOrderDetailModel.WindowDeep = 0;
                                    outsourceOrderDetailModel.WindowHigh = 0;
                                    outsourceOrderDetailModel.WindowSize = string.Empty;
                                    outsourceOrderDetailModel.WindowWide = 0;
                                    outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                    outsourceOrderDetailModel.PayOrderPrice = oohInstallPrice;
                                    outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                    outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                    outsourceOrderDetailModel.InstallPriceMaterialSupport = string.Empty;
                                    outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                    outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                    outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                    outsourceOrderDetailModel.OutsourceId = shop.OOHInstallOutsourceId;
                                    outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                    installPrice = installPrice - oohInstallPrice;
                                    oohInstallPrice = 0;
                                }
                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                                outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                                outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                                outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                                outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                                outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                                outsourceOrderDetailModel.Contact = oneShopOrderList[0].Contact;
                                outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                outsourceOrderDetailModel.GraphicWidth = 0;
                                outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                                outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                                outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                                outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                outsourceOrderDetailModel.POPName = string.Empty;
                                outsourceOrderDetailModel.POPType = string.Empty;
                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                outsourceOrderDetailModel.POSScale = posScale;
                                outsourceOrderDetailModel.Province = shop.ProvinceName;
                                outsourceOrderDetailModel.Quantity = 1;
                                outsourceOrderDetailModel.Region = shop.RegionName;
                                outsourceOrderDetailModel.Remark = remark;
                                outsourceOrderDetailModel.Sheet = string.Empty;
                                outsourceOrderDetailModel.ShopId = shop.Id;
                                outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                                outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                                outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                                outsourceOrderDetailModel.SubjectId = 0;
                                outsourceOrderDetailModel.Tel = shop.Tel1;
                                outsourceOrderDetailModel.TotalArea = 0;
                                outsourceOrderDetailModel.WindowDeep = 0;
                                outsourceOrderDetailModel.WindowHigh = 0;
                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                outsourceOrderDetailModel.WindowWide = 0;
                                outsourceOrderDetailModel.ReceiveOrderPrice = receiveInstallPrice;
                                outsourceOrderDetailModel.PayOrderPrice = installPrice;
                                outsourceOrderDetailModel.PayBasicInstallPrice = basicInstallPrice;
                                outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                                outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                            }
                        }
                        else if ((guidanceType == (int)GuidanceTypeEnum.Promotion && hasExpressPrice) || (guidanceType == (int)GuidanceTypeEnum.Delivery) && popList.Any())
                        {
                            expressPriceDetailModel = expressPriceDetailBll.GetList(price => price.GuidanceId == subjectModel.gudiance.ItemId && price.ShopId == shop.Id).FirstOrDefault();
                            //快递费
                            decimal rExpressPrice = 0;
                            decimal payExpressPrice = 0;
                            if (expressPriceDetailModel != null && (expressPriceDetailModel.ExpressPrice ?? 0) > 0)
                            {
                                rExpressPrice = expressPriceDetailModel.ExpressPrice ?? 0;
                            }
                            else
                                rExpressPrice = 35;

                            ExpressPriceConfig eM = expressPriceConfigList.Where(price => price.ReceivePrice == rExpressPrice).FirstOrDefault();
                            if (eM != null)
                                payExpressPrice = eM.PayPrice ?? 0;
                            else
                                payExpressPrice = 22;
                            if (shop.ProvinceName == "内蒙古" && !shop.CityName.Contains("通辽"))
                            {
                                payExpressPrice = 0;
                            }

                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].AgentCode;
                            outsourceOrderDetailModel.AgentName = oneShopOrderList[0].AgentName;
                            outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].BusinessModel;
                            outsourceOrderDetailModel.Channel = oneShopOrderList[0].Channel;
                            outsourceOrderDetailModel.City = oneShopOrderList[0].City;
                            outsourceOrderDetailModel.CityTier = oneShopOrderList[0].CityTier;
                            outsourceOrderDetailModel.Contact = shop.Contact1;
                            outsourceOrderDetailModel.Format = oneShopOrderList[0].Format;
                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                            outsourceOrderDetailModel.GraphicWidth = 0;
                            outsourceOrderDetailModel.GuidanceId = subjectModel.gudiance.ItemId;
                            outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = oneShopOrderList[0].LocationType;
                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                            outsourceOrderDetailModel.MaterialSupport = string.Empty;
                            outsourceOrderDetailModel.OrderGender = string.Empty;
                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.发货费;
                            outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                            outsourceOrderDetailModel.POPName = string.Empty;
                            outsourceOrderDetailModel.POPType = string.Empty;
                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                            outsourceOrderDetailModel.POSScale = posScale;
                            outsourceOrderDetailModel.Province = shop.ProvinceName;
                            outsourceOrderDetailModel.Quantity = 1;
                            outsourceOrderDetailModel.Region = shop.RegionName;
                            outsourceOrderDetailModel.Remark = string.Empty;
                            outsourceOrderDetailModel.Sheet = string.Empty;
                            outsourceOrderDetailModel.ShopId = shop.Id;
                            outsourceOrderDetailModel.ShopName = oneShopOrderList[0].ShopName;
                            outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].ShopNo;
                            outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].ShopStatus;
                            outsourceOrderDetailModel.SubjectId = 0;
                            outsourceOrderDetailModel.Tel = shop.Tel1;
                            outsourceOrderDetailModel.TotalArea = 0;
                            outsourceOrderDetailModel.WindowDeep = 0;
                            outsourceOrderDetailModel.WindowHigh = 0;
                            outsourceOrderDetailModel.WindowSize = string.Empty;
                            outsourceOrderDetailModel.WindowWide = 0;
                            outsourceOrderDetailModel.ReceiveOrderPrice = rExpressPrice;
                            outsourceOrderDetailModel.PayOrderPrice = payExpressPrice;
                            outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                            outsourceOrderDetailModel.PayOOHInstallPrice = 0;
                            outsourceOrderDetailModel.PayExpressPrice = payExpressPrice;
                            outsourceOrderDetailModel.ReceiveExpresslPrice = rExpressPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = string.Empty;
                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                            outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].CSUserId;
                            outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                            if (shop.ProvinceName == "天津")
                                outsourceOrderDetailModel.OutsourceId = calerOutsourceId;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);

                        }
                        #endregion

                    });
                }
            }
        }

        //重新计算外协安装费
        public void ResetOutsourceInstallPrice(int guidanceId, List<int> shopIdList)
        {
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             join guidance in CurrentContext.DbContext.SubjectGuidance
                             on order.GuidanceId equals guidance.ItemId
                             join subject in CurrentContext.DbContext.Subject
                             on order.SubjectId equals subject.Id
                             join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                             on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                             from subjectCategory in categortTemp.DefaultIfEmpty()
                             where order.GuidanceId == guidanceId
                             && shopIdList.Contains(order.ShopId ?? 0)
                             && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 1 && order.GraphicWidth != null && order.GraphicWidth > 1) || order.OrderType == (int)OrderTypeEnum.道具)
                             && (order.IsValid == null || order.IsValid == true)
                             && (order.IsDelete == null || order.IsDelete == false)
                             && (order.ShopStatus == null || order.ShopStatus == "" || order.ShopStatus == ShopStatusEnum.正常.ToString())
                             && (order.IsProduce == null || order.IsProduce == true)
                             && (order.IsValidFromAssign == null || order.IsValidFromAssign == true)
                             && (subject.SubjectType != (int)SubjectTypeEnum.二次安装)
                             && subject.SubjectType != (int)SubjectTypeEnum.新开店安装费
                             select new
                             {
                                 order,
                                 shop,
                                 guidance,
                                 subject,
                                 CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                             }).ToList();
            OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
            outsourceOrderDetailBll.Delete(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0) && s.OrderType == (int)OrderTypeEnum.安装费 && s.SubjectId == 0);
            if (orderList.Any())
            {
                OutsourceOrderDetail outsourceOrderDetailModel;

                int guidanceType = orderList[0].guidance.ActivityTypeId ?? 0;
                string beiJingCalerOutsourceName = string.Empty;
                int calerOutsourceId = 8;
                try
                {
                    beiJingCalerOutsourceName = ConfigurationManager.AppSettings["CalerOutsourceName"];
                }
                catch
                {

                }
                if (!string.IsNullOrWhiteSpace(beiJingCalerOutsourceName))
                {
                    Company companyModel = new CompanyBLL().GetList(s => (s.CompanyName == beiJingCalerOutsourceName || s.ShortName == beiJingCalerOutsourceName) && s.TypeId == (int)CompanyTypeEnum.Outsource).FirstOrDefault();
                    if (companyModel != null)
                        calerOutsourceId = companyModel.Id;
                }
                List<Shop> shopList = orderList.Select(s => s.shop).Distinct().ToList();
                List<int> newShopIdList = shopList.Select(s => s.Id).ToList();
                List<POP> oohPOPList = new POPBLL().GetList(pop => newShopIdList.Contains(pop.ShopId ?? 0) && (pop.Sheet == "户外" || pop.Sheet.ToLower() == "ooh") && (pop.OOHInstallPrice ?? 0) > 0);
                List<string> BCSCityTierList = new List<string>() { "T1", "T2", "T3" };
                shopList.ForEach(shop =>
                {

                    if (guidanceType == (int)GuidanceTypeEnum.Promotion || guidanceType == (int)GuidanceTypeEnum.Delivery)
                    {
                        var orderListP = outsourceOrderDetailBll.GetList(s => s.GuidanceId == guidanceId && s.ShopId == shop.Id && s.SubjectId > 0);
                        if (!orderListP.Any())
                        {
                            outsourceOrderDetailBll.Delete(s => s.GuidanceId == guidanceId && s.ShopId == shop.Id && s.OrderType == (int)OrderTypeEnum.发货费 && s.SubjectId == 0);
                        }
                    }
                    bool isInstallShop = false;
                    //decimal promotionInstallPrice = 0;
                    List<string> materialSupportList = new List<string>();
                    string posScale = string.Empty;
                    bool isBCSSubject = true;
                    bool isGeneric = true;
                    var oneShopOrderList = orderList.Where(order => order.order.ShopId == shop.Id).ToList();
                    if (oneShopOrderList.Any())
                    {

                        decimal installPrice = 0;
                        decimal oohInstallPrice = 0;
                        decimal basicInstallPrice = 0;
                        decimal receiveInstallPrice = 0;
                        string materialSupport = string.Empty;
                        string remark = string.Empty;
                        if (guidanceType == (int)GuidanceTypeEnum.Promotion)
                        {
                            if (shop.IsInstall == "Y")
                            {
                                var windowStickOrderList = oneShopOrderList.Where(s => s.order.Sheet != null && (s.order.Sheet.Contains("橱窗") || s.order.Sheet.Contains("窗贴")) && s.order.GraphicMaterial.Contains("全透贴"));
                                if (windowStickOrderList.Any())
                                {
                                    installPrice = 150;
                                    receiveInstallPrice = 150;
                                    remark = "促销窗贴安装费";
                                }
                            }
                        }
                        else if (guidanceType == (int)GuidanceTypeEnum.Install)
                        {

                            oneShopOrderList.ForEach(order =>
                            {
                                if (order.subject.CornerType != "三叶草")
                                    isBCSSubject = false;
                                if (!order.CategoryName.Contains("常规-非活动"))
                                    isGeneric = false;

                                if (string.IsNullOrWhiteSpace(posScale) && !string.IsNullOrWhiteSpace(order.order.InstallPricePOSScale))
                                {
                                    posScale = order.order.InstallPricePOSScale;
                                }
                                if (!string.IsNullOrWhiteSpace(order.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(order.order.InstallPriceMaterialSupport.ToLower()))
                                {
                                    materialSupportList.Add(order.order.InstallPriceMaterialSupport.ToLower());
                                }
                            });
                            if (guidanceType == (int)GuidanceTypeEnum.Install)
                            {
                                if (isBCSSubject)
                                {
                                    isInstallShop = shop.BCSIsInstall == "Y";
                                }
                                else
                                {
                                    isInstallShop = shop.IsInstall == "Y";
                                }
                            }
                            else
                            {
                                isInstallShop = shop.IsInstall == "Y";
                            }
                            if (isInstallShop)
                            {

                                //按照级别，获取基础安装费

                                materialSupportList.ForEach(ma =>
                                {
                                    decimal basicInstallPrice0 = new BasePage().GetOutsourceBasicInstallPrice(ma);
                                    if (basicInstallPrice0 > basicInstallPrice)
                                    {
                                        basicInstallPrice = basicInstallPrice0;
                                        materialSupport = ma;
                                    }
                                });

                                var oohList = oneShopOrderList.Where(s => (s.order.Sheet != null && (s.order.Sheet.Contains("户外") || s.order.Sheet.ToLower() == "ooh"))).ToList();
                                if (oohList.Any())
                                {

                                    oohList.ForEach(s =>
                                    {
                                        decimal price = 0;
                                        if (!string.IsNullOrWhiteSpace(s.order.GraphicNo))
                                        {
                                            price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.order.GraphicNo.ToLower()).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                        }
                                        else
                                            price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.order.GraphicLength && p.GraphicWidth == s.order.GraphicWidth).Select(p => p.OSOOHInstallPrice ?? 0).FirstOrDefault();

                                        if (price > oohInstallPrice)
                                        {
                                            oohInstallPrice = price;
                                        }
                                    });
                                }


                                InstallPriceTempBLL installShopPriceBll = new InstallPriceTempBLL();
                                var installShopList = installShopPriceBll.GetList(sh => sh.GuidanceId == guidanceId && sh.ShopId == shop.Id);
                                if (installShopList.Any())
                                {
                                    installShopList.ForEach(sh =>
                                    {
                                        receiveInstallPrice += ((sh.BasicPrice ?? 0) + (sh.OOHPrice ?? 0) + (sh.WindowPrice ?? 0));
                                    });
                                }
                                if ((shop.OutsourceInstallPrice ?? 0) > 0)
                                {
                                    basicInstallPrice = shop.OutsourceInstallPrice ?? 0;
                                }
                                if (isBCSSubject)
                                {
                                    if ((shop.OutsourceBCSInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = shop.OutsourceBCSInstallPrice ?? 0;
                                    }
                                    else if (BCSCityTierList.Contains(shop.CityTier.ToUpper()))
                                    {
                                        basicInstallPrice = 150;
                                    }
                                    else
                                    {
                                        basicInstallPrice = 0;
                                    }

                                }
                                if (isGeneric)
                                {
                                    if (shop.CityName == "包头市" && (shop.OutsourceInstallPrice ?? 0) > 0)
                                    {
                                        basicInstallPrice = shop.OutsourceInstallPrice ?? 0;
                                    }
                                    else if (BCSCityTierList.Contains(shop.CityTier.ToUpper()))
                                    {
                                        basicInstallPrice = 150;
                                    }
                                    else
                                    {
                                        basicInstallPrice = 0;
                                    }
                                }
                                installPrice = oohInstallPrice + basicInstallPrice;
                                remark = "活动安装费";
                            }
                        }

                        if (installPrice > 0)
                        {
                            if (oohInstallPrice > 0 && (shop.OOHInstallOutsourceId ?? 0) > 0)
                            {
                                //如果有单独的户外安装外协
                                outsourceOrderDetailModel = new OutsourceOrderDetail();
                                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                                outsourceOrderDetailModel.AddDate = DateTime.Now;
                                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].order.AgentCode;
                                outsourceOrderDetailModel.AgentName = oneShopOrderList[0].order.AgentName;
                                outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].order.BusinessModel;
                                outsourceOrderDetailModel.Channel = oneShopOrderList[0].order.Channel;
                                outsourceOrderDetailModel.City = oneShopOrderList[0].order.City;
                                outsourceOrderDetailModel.CityTier = oneShopOrderList[0].order.CityTier;
                                outsourceOrderDetailModel.Contact = shop.Contact1;
                                outsourceOrderDetailModel.Format = oneShopOrderList[0].order.Format;
                                outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                                outsourceOrderDetailModel.GraphicNo = string.Empty;
                                outsourceOrderDetailModel.GraphicWidth = 0;
                                outsourceOrderDetailModel.GuidanceId = guidanceId;
                                outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].order.IsInstall;
                                outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].order.BCSIsInstall;
                                outsourceOrderDetailModel.LocationType = oneShopOrderList[0].order.LocationType;
                                outsourceOrderDetailModel.MachineFrame = string.Empty;
                                outsourceOrderDetailModel.MaterialSupport = materialSupport;
                                outsourceOrderDetailModel.OrderGender = string.Empty;
                                outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                                outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                                outsourceOrderDetailModel.POPName = string.Empty;
                                outsourceOrderDetailModel.POPType = string.Empty;
                                outsourceOrderDetailModel.PositionDescription = string.Empty;
                                outsourceOrderDetailModel.POSScale = posScale;
                                outsourceOrderDetailModel.Province = shop.ProvinceName;
                                outsourceOrderDetailModel.Quantity = 1;
                                outsourceOrderDetailModel.Region = shop.RegionName;
                                outsourceOrderDetailModel.Remark = "户外安装费";
                                outsourceOrderDetailModel.Sheet = string.Empty;
                                outsourceOrderDetailModel.ShopId = shop.Id;
                                outsourceOrderDetailModel.ShopName = oneShopOrderList[0].order.ShopName;
                                outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].order.ShopNo;
                                outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].order.ShopStatus;
                                outsourceOrderDetailModel.SubjectId = 0;
                                outsourceOrderDetailModel.Tel = shop.Tel1;
                                outsourceOrderDetailModel.TotalArea = 0;
                                outsourceOrderDetailModel.WindowDeep = 0;
                                outsourceOrderDetailModel.WindowHigh = 0;
                                outsourceOrderDetailModel.WindowSize = string.Empty;
                                outsourceOrderDetailModel.WindowWide = 0;
                                outsourceOrderDetailModel.ReceiveOrderPrice = 0;
                                outsourceOrderDetailModel.PayOrderPrice = oohInstallPrice;
                                outsourceOrderDetailModel.PayBasicInstallPrice = 0;
                                outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                                outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                                outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                                outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                                outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].order.CSUserId;
                                outsourceOrderDetailModel.OutsourceId = shop.OOHInstallOutsourceId;
                                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                                installPrice = installPrice - oohInstallPrice;
                                oohInstallPrice = 0;
                            }
                            outsourceOrderDetailModel = new OutsourceOrderDetail();
                            outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                            outsourceOrderDetailModel.AddDate = DateTime.Now;
                            outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                            outsourceOrderDetailModel.AgentCode = oneShopOrderList[0].order.AgentCode;
                            outsourceOrderDetailModel.AgentName = oneShopOrderList[0].order.AgentName;
                            outsourceOrderDetailModel.BusinessModel = oneShopOrderList[0].order.BusinessModel;
                            outsourceOrderDetailModel.Channel = oneShopOrderList[0].order.Channel;
                            outsourceOrderDetailModel.City = oneShopOrderList[0].order.City;
                            outsourceOrderDetailModel.CityTier = oneShopOrderList[0].order.CityTier;
                            outsourceOrderDetailModel.Contact = oneShopOrderList[0].order.Contact;
                            outsourceOrderDetailModel.Format = oneShopOrderList[0].order.Format;
                            outsourceOrderDetailModel.GraphicMaterial = string.Empty;
                            outsourceOrderDetailModel.GraphicNo = string.Empty;
                            outsourceOrderDetailModel.GraphicWidth = 0;
                            outsourceOrderDetailModel.GuidanceId = guidanceId;
                            outsourceOrderDetailModel.IsInstall = oneShopOrderList[0].order.IsInstall;
                            outsourceOrderDetailModel.BCSIsInstall = oneShopOrderList[0].order.BCSIsInstall;
                            outsourceOrderDetailModel.LocationType = oneShopOrderList[0].order.LocationType;
                            outsourceOrderDetailModel.MachineFrame = string.Empty;
                            outsourceOrderDetailModel.MaterialSupport = materialSupport;
                            outsourceOrderDetailModel.OrderGender = string.Empty;
                            outsourceOrderDetailModel.OrderType = (int)OrderTypeEnum.安装费;
                            outsourceOrderDetailModel.POPAddress = shop.POPAddress;
                            outsourceOrderDetailModel.POPName = string.Empty;
                            outsourceOrderDetailModel.POPType = string.Empty;
                            outsourceOrderDetailModel.PositionDescription = string.Empty;
                            outsourceOrderDetailModel.POSScale = posScale;
                            outsourceOrderDetailModel.Province = shop.ProvinceName;
                            outsourceOrderDetailModel.Quantity = 1;
                            outsourceOrderDetailModel.Region = shop.RegionName;
                            outsourceOrderDetailModel.Remark = remark;
                            outsourceOrderDetailModel.Sheet = string.Empty;
                            outsourceOrderDetailModel.ShopId = shop.Id;
                            outsourceOrderDetailModel.ShopName = oneShopOrderList[0].order.ShopName;
                            outsourceOrderDetailModel.ShopNo = oneShopOrderList[0].order.ShopNo;
                            outsourceOrderDetailModel.ShopStatus = oneShopOrderList[0].order.ShopStatus;
                            outsourceOrderDetailModel.SubjectId = 0;
                            outsourceOrderDetailModel.Tel = shop.Tel1;
                            outsourceOrderDetailModel.TotalArea = 0;
                            outsourceOrderDetailModel.WindowDeep = 0;
                            outsourceOrderDetailModel.WindowHigh = 0;
                            outsourceOrderDetailModel.WindowSize = string.Empty;
                            outsourceOrderDetailModel.WindowWide = 0;
                            outsourceOrderDetailModel.ReceiveOrderPrice = receiveInstallPrice;
                            outsourceOrderDetailModel.PayOrderPrice = installPrice;
                            outsourceOrderDetailModel.PayBasicInstallPrice = basicInstallPrice;
                            outsourceOrderDetailModel.PayOOHInstallPrice = oohInstallPrice;
                            outsourceOrderDetailModel.InstallPriceMaterialSupport = materialSupport;
                            outsourceOrderDetailModel.ReceiveUnitPrice = 0;
                            outsourceOrderDetailModel.ReceiveTotalPrice = 0;
                            outsourceOrderDetailModel.CSUserId = oneShopOrderList[0].order.CSUserId;
                            outsourceOrderDetailModel.OutsourceId = shop.OutsourceId ?? 0;
                            outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
                        }
                    }
                });
            }

        }

        //分配外协费用订单
        public void AutoAssignPriceOrder(int subjectId)
        {
            var orderList = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                             join shop in CurrentContext.DbContext.Shop
                             on order.ShopId equals shop.Id
                             where order.SubjectId == subjectId
                             && (order.IsDelete == null || order.IsDelete == false)
                             && (order.OrderType == (int)OrderTypeEnum.安装费 || order.OrderType == (int)OrderTypeEnum.测量费 || order.OrderType == (int)OrderTypeEnum.发货费 || order.OrderType == (int)OrderTypeEnum.其他费用)
                             select new
                             {
                                 order,
                                 shop
                             }).ToList();
            OutsourceOrderDetail outsourceOrderDetailModel;
            OutsourceOrderDetailBLL outsourceOrderDetailBll = new OutsourceOrderDetailBLL();
            orderList.ForEach(s =>
            {
                int Quantity = s.order.Quantity ?? 1;
                outsourceOrderDetailModel = new OutsourceOrderDetail();
                outsourceOrderDetailModel.AddDate = DateTime.Now;
                outsourceOrderDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                outsourceOrderDetailModel.AgentCode = s.order.AgentCode;
                outsourceOrderDetailModel.AgentName = s.order.AgentName;
                outsourceOrderDetailModel.Area = s.order.Area;
                outsourceOrderDetailModel.BusinessModel = s.order.BusinessModel;
                outsourceOrderDetailModel.Channel = s.order.Channel;
                outsourceOrderDetailModel.ChooseImg = s.order.ChooseImg;
                outsourceOrderDetailModel.City = s.order.City;
                outsourceOrderDetailModel.CityTier = s.order.CityTier;
                outsourceOrderDetailModel.Contact = s.order.Contact;
                outsourceOrderDetailModel.CornerType = s.order.CornerType;
                outsourceOrderDetailModel.Format = s.order.Format;
                outsourceOrderDetailModel.Gender = (s.order.OrderGender != null && s.order.OrderGender != "") ? s.order.OrderGender : (s.order.Gender ?? "");
                outsourceOrderDetailModel.GraphicLength = s.order.GraphicLength;
                outsourceOrderDetailModel.GraphicNo = s.order.GraphicNo;
                outsourceOrderDetailModel.GraphicWidth = s.order.GraphicWidth;
                outsourceOrderDetailModel.GuidanceId = s.order.GuidanceId;
                outsourceOrderDetailModel.IsInstall = s.order.IsInstall;
                outsourceOrderDetailModel.BCSIsInstall = s.order.BCSIsInstall;
                outsourceOrderDetailModel.LocationType = s.order.LocationType;
                outsourceOrderDetailModel.MachineFrame = s.order.MachineFrame;
                outsourceOrderDetailModel.MaterialSupport = s.order.MaterialSupport;
                outsourceOrderDetailModel.OrderGender = s.order.OrderGender;
                outsourceOrderDetailModel.OrderType = s.order.OrderType;
                outsourceOrderDetailModel.POPAddress = s.order.POPAddress;
                outsourceOrderDetailModel.POPName = s.order.POPName;
                outsourceOrderDetailModel.POPType = s.order.POPType;
                outsourceOrderDetailModel.PositionDescription = s.order.PositionDescription;
                outsourceOrderDetailModel.POSScale = s.order.POSScale;
                outsourceOrderDetailModel.Province = s.order.Province;
                outsourceOrderDetailModel.Quantity = Quantity;
                outsourceOrderDetailModel.Region = s.order.Region;
                outsourceOrderDetailModel.Remark = s.order.Remark;
                outsourceOrderDetailModel.Sheet = s.order.Sheet;
                outsourceOrderDetailModel.ShopId = s.order.ShopId;
                outsourceOrderDetailModel.ShopName = s.order.ShopName;
                outsourceOrderDetailModel.ShopNo = s.order.ShopNo;
                outsourceOrderDetailModel.ShopStatus = s.order.ShopStatus;
                outsourceOrderDetailModel.SubjectId = s.order.SubjectId;
                outsourceOrderDetailModel.Tel = s.order.Tel;
                outsourceOrderDetailModel.TotalArea = s.order.TotalArea;
                outsourceOrderDetailModel.WindowDeep = s.order.WindowDeep;
                outsourceOrderDetailModel.WindowHigh = s.order.WindowHigh;
                outsourceOrderDetailModel.WindowSize = s.order.WindowSize;
                outsourceOrderDetailModel.WindowWide = s.order.WindowWide;
                outsourceOrderDetailModel.ReceiveOrderPrice = s.order.OrderPrice;
                outsourceOrderDetailModel.PayOrderPrice = s.order.PayOrderPrice;
                outsourceOrderDetailModel.InstallPriceMaterialSupport = s.order.InstallPriceMaterialSupport;
                outsourceOrderDetailModel.ReceiveUnitPrice = s.order.UnitPrice;
                outsourceOrderDetailModel.ReceiveTotalPrice = s.order.TotalPrice;
                outsourceOrderDetailModel.RegionSupplementId = s.order.RegionSupplementId;
                outsourceOrderDetailModel.CSUserId = s.order.CSUserId;
                outsourceOrderDetailModel.OutsourceId = s.shop.OutsourceId ?? 0;
                outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Install;
                if (s.order.OrderType == (int)OrderTypeEnum.发货费)
                {
                    outsourceOrderDetailModel.AssignType = (int)OutsourceOrderTypeEnum.Send;
                }
                outsourceOrderDetailModel.FinalOrderId = s.order.Id;
                outsourceOrderDetailBll.Add(outsourceOrderDetailModel);
            });
        }

        //重新计算活动安装费
        public void RecountInstallPrice(int guidanceId, List<int> shopIdList)
        {
            InstallPriceTempBLL installPriceTempBll = new InstallPriceTempBLL();
            InstallPriceShopInfoBLL installPriceShopInfoBll = new InstallPriceShopInfoBLL();
            List<int> shopIdList0 = new List<int>();
            List<FinalOrderDetailTemp> orderList = new FinalOrderDetailTempBLL().GetList(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0) && (s.IsDelete == null || s.IsDelete == false));

            //if (orderList.Any())
            //{
            //    shopIdList0 = orderList.Select(s => s.ShopId ?? 0).Distinct().ToList();
            //}
            //删除
            installPriceTempBll.Delete(s => s.GuidanceId == guidanceId && shopIdList.Contains(s.ShopId ?? 0));
            var allOrderlist = (from order in orderList
                                join shop in CurrentContext.DbContext.Shop
                                on order.ShopId equals shop.Id
                                join subject in CurrentContext.DbContext.Subject
                                on order.SubjectId equals subject.Id
                                join subjectCategory1 in CurrentContext.DbContext.ADSubjectCategory
                                on subject.SubjectCategoryId equals subjectCategory1.Id into categortTemp
                                from subjectCategory in categortTemp.DefaultIfEmpty()
                                where ((order.IsInstall != null && order.IsInstall == "Y" && (subject.CornerType == null || subject.CornerType == "" || subject.CornerType != "三叶草")) || (subject.CornerType == "三叶草" && order.BCSIsInstall == "Y"))
                                && ((order.OrderType == (int)OrderTypeEnum.POP && order.GraphicLength != null && order.GraphicLength > 0 && order.GraphicWidth != null && order.GraphicWidth > 0) || order.OrderType == (int)OrderTypeEnum.道具)
                                && (order.IsDelete == null || order.IsDelete == false)
                                && (subject.IsDelete == null || subject.IsDelete == false)
                                && subject.ApproveState == 1
                                && subject.SubjectType != (int)SubjectTypeEnum.二次安装
                                && subject.SubjectType != (int)SubjectTypeEnum.费用订单
                                select new
                                {
                                    subject,
                                    order,
                                    shop,
                                    CategoryName = subjectCategory != null ? (subjectCategory.CategoryName) : ""
                                }).ToList();
            if (allOrderlist.Any())
            {
                List<string> cityCierList = new List<string>() { "T1", "T2", "T3" };
                List<string> BCSInstallCityTierList = new List<string>();
                decimal bcsBasicInstallPrice = 150;//三叶草t1-t3安装费
                if (!BCSInstallCityTierList.Any())
                {
                    string BCSInstallCityTier = string.Empty;
                    try
                    {
                        BCSInstallCityTier = ConfigurationManager.AppSettings["BCSBasicInstallPrice"];
                        if (!string.IsNullOrWhiteSpace(BCSInstallCityTier))
                        {
                            string[] str = BCSInstallCityTier.Split(':');
                            BCSInstallCityTierList = StringHelper.ToStringList(str[0], ',', LowerUpperEnum.ToUpper);
                            bcsBasicInstallPrice = StringHelper.IsDecimal(str[1]);
                        }
                    }
                    catch
                    {

                    }
                }
                List<Shop> shopList = allOrderlist.Select(s => s.shop).Distinct().ToList();
                shopIdList = shopList.Select(s => s.Id).ToList();
                var oohPOPList = new POPBLL().GetList(s => shopIdList.Contains(s.ShopId ?? 0) && (s.Sheet.ToLower() == "ooh" || s.Sheet == "户外") && (s.OOHInstallPrice ?? 0) > 0);

                InstallPriceTemp installPriceTempModel;
                shopList.ForEach(shop =>
                {
                    bool isBCSSubject = true;
                    bool isGeneric = true;
                    //基础安装费
                    decimal basicInstallPrice = 0;
                    //橱窗安装费
                    decimal windowInstallPrice = 0;
                    //OOH安装费
                    decimal oohInstallPrice = 0;
                    string materialSupport = string.Empty;
                    string POSScale = string.Empty;
                    List<string> materialSupportList = new List<string>();
                    var oneShopOrderList = allOrderlist.Where(s => s.order.ShopId == shop.Id).ToList();
                    if (oneShopOrderList.Any())
                    {
                        oneShopOrderList.ForEach(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(s.order.InstallPriceMaterialSupport) && !materialSupportList.Contains(s.order.InstallPriceMaterialSupport.ToLower()))
                            {
                                materialSupportList.Add(s.order.InstallPriceMaterialSupport.ToLower());
                            }
                            if (string.IsNullOrWhiteSpace(POSScale) && !string.IsNullOrWhiteSpace(s.order.InstallPricePOSScale))
                                POSScale = s.order.InstallPricePOSScale;
                            if (s.subject.CornerType != "三叶草")
                                isBCSSubject = false;
                            if (!s.CategoryName.Contains("常规-非活动"))
                                isGeneric = false;
                        });
                        List<FinalOrderDetailTemp> oohOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower() == "ooh" || s.order.Sheet == "户外")).Select(s => s.order).ToList();
                        List<FinalOrderDetailTemp> windowOrderList = oneShopOrderList.Where(s => s.order.ShopId == shop.Id && s.order.Sheet != null && (s.order.Sheet.ToLower().Contains("橱窗") || s.order.Sheet.ToLower().Contains("window"))).Select(s => s.order).ToList();


                        #region 店内安装费
                        if (isBCSSubject)
                        {
                            if ((shop.BCSInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = (shop.BCSInstallPrice ?? 0);
                            }
                            else if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                            {
                                basicInstallPrice = bcsBasicInstallPrice;
                            }
                            else
                            {
                                basicInstallPrice = 0;
                            }
                        }
                        else if (isGeneric)
                        {
                            if (BCSInstallCityTierList.Contains(shop.CityTier.ToUpper()))
                            {
                                basicInstallPrice = bcsBasicInstallPrice;
                            }
                            else
                            {
                                basicInstallPrice = 0;
                            }
                        }
                        else
                        {
                            if ((shop.BasicInstallPrice ?? 0) > 0)
                            {
                                basicInstallPrice = (shop.BasicInstallPrice ?? 0);
                            }
                            else
                            {

                                //按照级别，获取基础安装费
                                materialSupportList.ForEach(ma =>
                                {
                                    decimal basicInstallPrice0 = GetBasicInstallPrice(ma);
                                    if (basicInstallPrice0 > basicInstallPrice)
                                    {
                                        basicInstallPrice = basicInstallPrice0;
                                        materialSupport = ma;
                                    }
                                });
                            }
                        }
                        #endregion
                        #region 橱窗安装
                        if (windowOrderList.Any())
                        {
                            windowInstallPrice = GetWindowInstallPrice(materialSupport);
                        }
                        #endregion
                        #region OOH安装费
                        if (oohOrderList.Any())
                        {

                            oohOrderList.ForEach(s =>
                            {
                                decimal price = 0;
                                if (!string.IsNullOrWhiteSpace(s.GraphicNo))
                                {
                                    price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicNo.ToLower() == s.GraphicNo.ToLower()).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                                }
                                else
                                    price = oohPOPList.Where(p => p.ShopId == shop.Id && p.GraphicLength == s.GraphicLength && p.GraphicWidth == s.GraphicWidth).Select(p => p.OOHInstallPrice ?? 0).FirstOrDefault();

                                if (price > oohInstallPrice)
                                {
                                    oohInstallPrice = price;
                                }
                            });
                        }
                        #endregion

                        #region 保存安装费
                        installPriceTempModel = new InstallPriceTemp();
                        installPriceTempModel.GuidanceId = guidanceId;
                        installPriceTempModel.ShopId = shop.Id;
                        installPriceTempModel.BasicPrice = basicInstallPrice;
                        installPriceTempModel.OOHPrice = oohInstallPrice;
                        installPriceTempModel.WindowPrice = windowInstallPrice;
                        installPriceTempModel.TotalPrice = basicInstallPrice + oohInstallPrice + windowInstallPrice;
                        installPriceTempModel.AddDate = DateTime.Now;
                        installPriceTempBll.Add(installPriceTempModel);
                        #endregion

                        //更新安装费归类表
                        InstallPriceShopInfo installPriceModel = installPriceShopInfoBll.GetList(s => s.GuidanceId == guidanceId && s.ShopId == shop.Id && s.AddType == 1).FirstOrDefault();
                        if (installPriceModel != null)
                        {
                            installPriceModel.BasicPrice = basicInstallPrice;
                            installPriceModel.OOHPrice = oohInstallPrice;
                            installPriceModel.WindowPrice = windowInstallPrice;
                            installPriceModel.MaterialSupport = materialSupport;
                            installPriceShopInfoBll.Update(installPriceModel);
                        }
                        
                    }
                    else
                    {
                        installPriceShopInfoBll.Delete(s => s.GuidanceId == guidanceId && s.ShopId == shop.Id && s.AddType == 1);

                    }
                });
            }
        }

        /// <summary>
        /// 保存报价订单
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="useSetting"></param>

        QuoteOrderDetailBLL quoteOrderBll = new QuoteOrderDetailBLL();
        QuoteOrderDetail quoteOrderModel;
        QuoteOrderSettingBLL quoteOrderSettingBll = new QuoteOrderSettingBLL();
        List<QuoteOrderSetting> settingList = new List<QuoteOrderSetting>();
        public void SaveQuotationOrder(FinalOrderDetailTemp order, bool? orderSetting = true)
        {
            if (order != null)
            {
                decimal width = order.GraphicWidth ?? 0;
                decimal length = order.GraphicLength ?? 0;
                decimal widthAdd = 0;
                decimal lengthAdd = 0;
                decimal areaAdd = 0;

                decimal addTotalPrice = 0;

                bool useSetting = orderSetting ?? true;
                if (string.IsNullOrWhiteSpace(order.Sheet) || string.IsNullOrWhiteSpace(order.GraphicMaterial))
                {
                    useSetting = false;
                }
                if (order.UnitName != "平米" && order.UnitName != "米")
                {
                    useSetting = false;
                }
                if ((order.GraphicWidth ?? 0) <= 1 || (order.GraphicLength ?? 0) <= 1)
                {
                    useSetting = false;
                }
                if (useSetting)
                {
                    if (!settingList.Any())
                    {
                        settingList = quoteOrderSettingBll.GetList(s => s.Id > 0);
                    }
                    if (settingList.Any())
                    {
                        var setting = settingList.Where(s => s.Sheet.ToLower() == order.Sheet.ToLower()).FirstOrDefault();
                        if (setting != null)
                        {
                            if (setting.OperateType == (int)QuoteOrderSettingTypeEnum.Percent)
                            {
                                if ((setting.POPWidth ?? 0) > 0)
                                {
                                    //order.GraphicWidth += (order.GraphicWidth * setting.POPWidth);
                                    widthAdd = width * (setting.POPWidth??0);
                                }
                                if ((setting.POPLength ?? 0) > 0)
                                {
                                    //order.GraphicLength += (order.GraphicLength * setting.POPLength);
                                    lengthAdd = length * (setting.POPLength?? 0);
                                }
                            }
                            else if (setting.OperateType == (int)QuoteOrderSettingTypeEnum.Amount)
                            {
                                if ((setting.POPWidth ?? 0) > 0)
                                {
                                    //order.GraphicWidth += setting.POPWidth;
                                    widthAdd = (setting.POPWidth??0);
                                }
                                if ((setting.POPLength ?? 0) > 0)
                                {
                                    //order.GraphicLength += setting.POPLength;
                                    lengthAdd = (setting.POPLength??0);
                                }
                            }
                            //order.Area = (order.GraphicWidth * order.GraphicLength) / 1000000;
                            areaAdd = (widthAdd * lengthAdd) / 1000000;
                            if (order.UnitName == "平米")
                            {
                                //order.TotalPrice = order.Area * (order.Quantity ?? 1) * (order.UnitPrice ?? 0);
                                addTotalPrice = areaAdd * (order.Quantity ?? 1) * (order.UnitPrice ?? 0);
                            }
                            else if (order.UnitName == "米")
                            {
                                //order.TotalPrice = (order.GraphicWidth / 1000) * 2 * (order.Quantity ?? 1) * (order.UnitPrice ?? 0);
                                addTotalPrice = (widthAdd / 1000) * 2 * (order.Quantity ?? 1) * (order.UnitPrice ?? 0);
                            }
                        }
                        else
                        {
                            useSetting = false;
                        }
                    }
                    else
                    {
                        useSetting = false;
                    }
                }
                quoteOrderModel = new QuoteOrderDetail();
                quoteOrderModel.AddDate = DateTime.Now;
                quoteOrderModel.AddUserId = order.AddUserId;
                quoteOrderModel.AgentCode = order.AgentCode;
                quoteOrderModel.AgentName = order.AgentName;
                
                quoteOrderModel.BCSIsInstall = order.BCSIsInstall;
                quoteOrderModel.BusinessModel = order.BusinessModel;
                quoteOrderModel.Category = order.Category;
                quoteOrderModel.Channel = order.Channel;
                quoteOrderModel.ChooseImg = order.ChooseImg;
                quoteOrderModel.City = order.City;
                quoteOrderModel.CityTier = order.CityTier;
                quoteOrderModel.Contact = order.Contact;
                quoteOrderModel.CornerType = order.CornerType;
                quoteOrderModel.CSUserId = order.CSUserId;
                quoteOrderModel.CustomerMaterialId = order.CustomerMaterialId;
                quoteOrderModel.FinalOrderId = order.Id;
                quoteOrderModel.Gender = order.Gender;
                
                quoteOrderModel.GraphicMaterial = order.GraphicMaterial;
                if (!string.IsNullOrWhiteSpace(order.GraphicMaterial))
                    quoteOrderModel.QuoteGraphicMaterial = this.GetQuoteMaterial(order.GraphicMaterial);
                quoteOrderModel.GraphicNo = order.GraphicNo;
                
                quoteOrderModel.GuidanceId = order.GuidanceId;
                quoteOrderModel.InstallPositionDescription = order.InstallPositionDescription;
                quoteOrderModel.InstallPriceMaterialSupport = order.InstallPriceMaterialSupport;
                quoteOrderModel.InstallPricePOSScale = order.InstallPricePOSScale;
                quoteOrderModel.IsFromRegion = order.IsFromRegion;
                quoteOrderModel.IsInstall = order.IsInstall;
                quoteOrderModel.IsValid = order.IsValid;
                quoteOrderModel.LocationType = order.LocationType;
                quoteOrderModel.MachineFrame = order.MachineFrame;
                quoteOrderModel.MaterialSupport = order.MaterialSupport;
                quoteOrderModel.OrderGender = order.OrderGender;
                quoteOrderModel.OrderPrice = order.OrderPrice;
                quoteOrderModel.OrderType = order.OrderType;
                quoteOrderModel.PayOrderPrice = order.PayOrderPrice;
                quoteOrderModel.POPAddress = order.POPAddress;
                quoteOrderModel.POPName = order.POPName;
                quoteOrderModel.POPType = order.POPType;
                quoteOrderModel.PositionDescription = order.PositionDescription;
                quoteOrderModel.POSScale = order.POSScale;
                quoteOrderModel.PriceBlongRegion = order.PriceBlongRegion;
                quoteOrderModel.Province = order.Province;
                quoteOrderModel.Quantity = order.Quantity;
                quoteOrderModel.Region = order.Region;
                quoteOrderModel.RegionSupplementId = order.RegionSupplementId;
                quoteOrderModel.Remark = order.Remark;
                quoteOrderModel.RightSideStick = order.RightSideStick;
                quoteOrderModel.Sheet = order.Sheet;
                quoteOrderModel.ShopId = order.ShopId;
                quoteOrderModel.ShopName = order.ShopName;
                quoteOrderModel.ShopNo = order.ShopNo;
                quoteOrderModel.ShopStatus = order.ShopStatus;
                quoteOrderModel.SmallMaterialId = order.SmallMaterialId;
                quoteOrderModel.SubjectId = order.SubjectId;
                quoteOrderModel.Tel = order.Tel;

                //原始尺寸
                quoteOrderModel.GraphicWidth = width;
                quoteOrderModel.GraphicLength = length;
                quoteOrderModel.Area = order.Area;
                quoteOrderModel.DefaultTotalPrice = order.TotalPrice;

                //增加的尺寸
                quoteOrderModel.AutoAddGraphicWidth = widthAdd;
                quoteOrderModel.AutoAddGraphicLength = lengthAdd;
                quoteOrderModel.AutoAddArea = areaAdd;
                quoteOrderModel.AutoAddTotalPrice = addTotalPrice;
                //增加后尺寸
                quoteOrderModel.TotalGraphicWidth = width + widthAdd;
                quoteOrderModel.TotalGraphicLength = length + lengthAdd;
                quoteOrderModel.TotalArea = order.Area + areaAdd;
                quoteOrderModel.TotalPrice = order.TotalPrice + addTotalPrice;

                quoteOrderModel.UnitPrice = order.UnitPrice;
                quoteOrderModel.UnitName = order.UnitName;
                quoteOrderModel.WindowDeep = order.WindowDeep;
                quoteOrderModel.WindowHigh = order.WindowHigh;
                quoteOrderModel.WindowSize = order.WindowSize;
                quoteOrderModel.WindowStick = order.WindowStick;
                quoteOrderModel.WindowWide = order.WindowWide;
                quoteOrderBll.Add(quoteOrderModel);
            }
        }



        public void SaveQuotationOrder12(FinalOrderDetailTemp order, bool? orderSetting = true)
        { }


        Dictionary<string, string> materialDic = new Dictionary<string, string>();
        QuoteMaterialBLL quoteMaterialBll = new QuoteMaterialBLL();
        CustomerMaterialInfoBLL customerMaterialBll = new CustomerMaterialInfoBLL();
        OrderMaterialMppingBLL orderMaterialMppingBll = new OrderMaterialMppingBLL();
        /// <summary>
        /// 将订单材质转换报价材质
        /// </summary>
        /// <param name="materialName"></param>
        /// <returns></returns>
        protected string GetQuoteMaterial(string materialName)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(materialName))
            {
                materialName = materialName.ToLower();
                if (materialDic.Keys.Contains(materialName))
                {
                    result = materialDic[materialName];
                }
                else
                {
                    var orderMaterialModel = orderMaterialMppingBll.GetList(s => s.OrderMaterialName.ToLower() == materialName).FirstOrDefault();
                    if (orderMaterialModel != null)
                    {
                        var quoteMaterial = quoteMaterialBll.GetList(s => s.CustomerMaterialId == orderMaterialModel.CustomerMaterialId).FirstOrDefault();
                        if (quoteMaterial != null)
                        {
                            result = quoteMaterial.QuoteMaterialName;
                            materialDic.Add(materialName, result);
                        }
                    }
                }
            }
            return result;
        }
    }
}
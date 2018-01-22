﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using Models;
using Common;
namespace WebApp.Subjects
{
    public partial class ShowOrderDetailExport : System.Web.UI.Page
    {
        string isFilter = "false";
        string fileName = "POP订单数据(含空尺寸)";
        int subjectId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["isFilter"] != null)
            {
                isFilter = Request.QueryString["isFilter"];
                isFilter = isFilter == "1" ? "true" : "false";
            }
            if (isFilter == "true")
            {
                fileName = "POP订单数据(不含空尺寸)";
            }
            if (Request.QueryString["subjectId"] != null)
            {
                subjectId = int.Parse(Request.QueryString["subjectId"]);
            }
            Subject model = new SubjectBLL().GetModel(subjectId);
            if (model != null)
            {
                DataSet ds = new FinalOrderDetailTempBLL().GetOrderList(subjectId.ToString(), "", isFilter);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    fileName = model.SubjectName + fileName;
                    OperateFile.ExportExcel(ds.Tables[0], fileName);

                }
                else
                {
                    HttpCookie cookie = Request.Cookies["export"];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("export");
                    }
                    cookie.Value = "2";
                    cookie.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(cookie);
                }
            }
        }


    }
}
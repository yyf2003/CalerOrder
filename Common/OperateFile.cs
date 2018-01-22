﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NPOI.SS.UserModel;
using NPOI;
using NPOI.XSSF.UserModel;
namespace Common
{
    public class OperateFile
    {

        public static void UpFiles(HttpPostedFile file, ref Models.Attachment models)
        {
            //检查文件扩展名字

            HttpPostedFile postedFile = file;
            string fileName;
            fileName = System.IO.Path.GetFileName(postedFile.FileName);
            if (fileName.ToLower() != "")
            {
                string name = fileName.Substring(0, fileName.LastIndexOf("."));
                string scurTypeName = fileName.Substring(fileName.LastIndexOf(".") + 1);
                //初始化原图物理路径
                string sGuid_phy = Guid.NewGuid().ToString();

                string strFile = "/UploadFiles/" + models.FileCode + "/" + models.FileType;
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(strFile)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(strFile));
                }
                //原图 原文件
                string FliePath = strFile + "/" + sGuid_phy + "." + scurTypeName;

                //小图片路径  SmallImgFilePath
                string SmallImgFilePath = string.Empty;
                postedFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath(FliePath));
                string extents = ConfigurationManager.AppSettings["UpLoadImgType"];
                if (extents != "")
                {
                    string[] ExtentArr = extents.Split('|');
                    if (ExtentArr.Contains(scurTypeName.ToLower()))
                    {

                        SmallImgFilePath = strFile + "/Small_" + sGuid_phy + "." + scurTypeName;
                        MakeThumbnail(System.Web.HttpContext.Current.Server.MapPath(FliePath), SmallImgFilePath, 90, 100, "Cut");
                    }
                    models.SmallImgUrl = SmallImgFilePath;
                }
                models.Title = name + "." + scurTypeName;
                models.FilePath = FliePath;



            }
        }


        ///<summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "A":
                    if (originalImage.Width / originalImage.Height >= width / height)
                    {
                        if (originalImage.Width > width)
                        {
                            towidth = width;
                            toheight = (originalImage.Height * width) / originalImage.Width;
                        }
                        else
                        {
                            towidth = originalImage.Width;
                            toheight = originalImage.Height;
                        }
                    }
                    else
                    {
                        if (originalImage.Height > height)
                        {
                            toheight = height;
                            towidth = (originalImage.Width * height) / originalImage.Height;
                        }
                        else
                        {
                            towidth = originalImage.Width;
                            toheight = originalImage.Height;
                        }
                    }
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
             new Rectangle(x, y, ow, oh),
             GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(System.Web.HttpContext.Current.Server.MapPath(thumbnailPath), System.Drawing.Imaging.ImageFormat.Jpeg);
                //outthumbnailPath=thumbnailPath;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }



        public static string UpLoadFile(HttpPostedFile file, string folder = null)
        {
            //检查文件扩展名字
            string SourceFliePath = string.Empty;
            HttpPostedFile postedFile = file;
            string fileName;
            fileName = System.IO.Path.GetFileName(postedFile.FileName);
            if (fileName.ToLower() != "")
            {
                DateTime date = DateTime.Now;

                string path = "/UploadFiles/";
                if (folder == null)
                {
                    string parentDirctoty = date.Year.ToString() + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0');
                    path += parentDirctoty;
                }
                else
                    path += folder;
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(path));
                }

                string filename1 = fileName.Substring(0, fileName.LastIndexOf('.'));
                string extent = fileName.Substring(fileName.LastIndexOf('.') + 1);

                //初始化原图物理路径
                string GuidFileName = Guid.NewGuid().ToString() + filename1;
                //原图 原文件
                SourceFliePath = path + "/" + GuidFileName + "." + extent;

                SourceFliePath = SourceFliePath.Replace("\'"," ");

                postedFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath(SourceFliePath));




            }
            return SourceFliePath;
        }

        public static void DownLoadFile(DataTable dt, string fileName = null)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                GridView gv = new GridView();
                gv.DataSource = dt;
                gv.DataBind();
                string name = fileName ?? "未能导入数据列表";
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "GB2312";  //设置了类型为中文防止乱码的出现  
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8) + ".xls");

                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                gv.EnableViewState = false;
                System.Globalization.CultureInfo myCItrad = new System.Globalization.CultureInfo("ZH-CN", true);
                System.IO.StringWriter tw = new System.IO.StringWriter(myCItrad);
                System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
                gv.RenderControl(hw);

                HttpContext.Current.Response.Write("<html><head><meta http-equiv=Content-Type content=\"text/html; charset=gb2312\">");
                HttpContext.Current.Response.Write(tw.ToString());
                HttpContext.Current.Response.Write("</body></html>"); ;
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="gv">GridView</param>
        /// <param name="currentPage">当前窗体</param>
        /// <param name="filename">导出文件名</param>
        public static void ExportExcel(GridView gv, string filename)
        {

            foreach (DataControlField dcf in gv.Columns)
            {
                if (dcf.HeaderText == "编辑" || dcf.HeaderText == "操作" || dcf.HeaderText == "删除" || dcf.HeaderText == "注销" || dcf.HeaderText == "查看")//前提条件是列名要与对应列的HeadText一致
                {
                    dcf.Visible = false;
                }
            }

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "GB2312";  //设置了类型为中文防止乱码的出现  
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8) + ".xls");

            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            gv.EnableViewState = false;
            System.Globalization.CultureInfo myCItrad = new System.Globalization.CultureInfo("ZH-CN", true);
            System.IO.StringWriter tw = new System.IO.StringWriter(myCItrad);
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            System.Web.UI.Page page = new System.Web.UI.Page();
            HtmlForm form = new HtmlForm();

            page.EnableEventValidation = false;


            page.DesignerInitialize();
            page.Controls.Add(form);
            form.Controls.Add(gv);
            page.RenderControl(hw);


            HttpContext.Current.Response.Write("<html><head><meta http-equiv=Content-Type content=\"text/html; charset=gb2312\">");
            HttpContext.Current.Response.Write(tw.ToString());
            HttpContext.Current.Response.Write("</body></html>"); ;
            HttpContext.Current.Response.End();
        }


        public static void DownLoadFile(MemoryStream ms, string fileName)
        {
            byte[] data = ms.ToArray();

            long fileSize = data.Length;

            //加上设置大小下载下来的.xlsx文件打开时才没有错误
            

            HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "UTF-8";//GB2312  //设置了类型为中文防止乱码的出现  
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xlsx");
            HttpContext.Current.Response.AddHeader("Content-Length", fileSize.ToString());
            //HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.ContentType = "application/vnd-excel";
            HttpContext.Current.Response.BinaryWrite(data);
            HttpContext.Current.Response.End();

        }

        public static void DownLoadFile(string path,string fileName1=null)
        {

            if (path != "")
            {
                try
                {
                    if (!File.Exists(HttpContext.Current.Server.MapPath(path)))
                    {
                        throw new Exception("文件不存在！");
                    }

                    string fileName = path.Substring(path.LastIndexOf('/') + 1);
                    if (!string.IsNullOrWhiteSpace(fileName1))
                    {
                        //string fn = fileName1 + fileName.Substring(fileName.IndexOf('.'));
                        fileName = fileName1;
                    }
                    long len = 102400;
                    byte[] buffer = new byte[len];
                    long read = 0;
                    FileStream fs = null;
                    fs = new FileStream(HttpContext.Current.Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
                    read = fs.Length;


                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)));

                    HttpContext.Current.Response.AddHeader("Content-Length", read.ToString());
                    HttpContext.Current.Response.AddHeader("Content-Transfer-Encoding", "binary");
                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                    while (read > 0)
                    {
                        if (HttpContext.Current.Response.IsClientConnected)
                        {
                            int length = fs.Read(buffer, 0, Convert.ToInt32(len));
                            HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.Clear();
                            read -= length;
                        }
                        else
                        {
                            read = -1;
                        }
                    }

                    HttpContext.Current.Response.Flush();

                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (Exception e)
                {
                    throw e;

                }
            }

        }

        public static void ExportExcel(DataTable dt, string fileName, string sheetName = null, string cookieName = null)
        {
            
            sheetName = string.IsNullOrWhiteSpace(sheetName) ? "Sheet1" : sheetName;
            IWorkbook workBook = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet(sheetName);
            int startRow = 1;
            DataColumnCollection cols = dt.Columns;
            int colsNum = cols.Count;
            IRow row1 = sheet.CreateRow(0);
            for (int i = 0; i < colsNum; i++)
            {
                row1.CreateCell(i).SetCellValue(cols[i].ColumnName);
            }
            foreach (DataRow dr in dt.Rows)
            {

                IRow dataRow = sheet.CreateRow(startRow);
                for (int i = 0; i < colsNum; i++)
                {
                    if (StringHelper.IsIntVal(dr[i].ToString()) && dr[i].ToString().Length<11)
                        dataRow.CreateCell(i).SetCellValue(int.Parse(dr[i].ToString()));
                    else if (StringHelper.IsDecimalVal(dr[i].ToString()) && dr[i].ToString().Length < 11)
                        dataRow.CreateCell(i).SetCellValue(double.Parse(dr[i].ToString()));
                    else if (StringHelper.IsDateTime(dr[i].ToString()))
                        dataRow.CreateCell(i).SetCellValue(DateTime.Parse(dr[i].ToString()).ToShortDateString());
                    else
                        dataRow.CreateCell(i).SetCellValue(dr[i].ToString());

                }
                startRow++;

            }
            cookieName=string.IsNullOrWhiteSpace(cookieName)?"export":cookieName;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
            }
            cookie.Value = "1";
            cookie.Expires = DateTime.Now.AddMinutes(30);
            HttpContext.Current.Response.Cookies.Add(cookie);

            using (MemoryStream ms = new MemoryStream())
            {
                workBook.Write(ms);
                ms.Flush();
                sheet = null;
                workBook = null;
                //DownLoadFile(ms, "综合查询");
                byte[] data = ms.ToArray();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                //HttpContext.Current.Response.Charset = "GB2312";  //设置了类型为中文防止乱码的出现  
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");
                //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.HtmlDecode(fileName) + ".xls");

                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.BinaryWrite(data);
                HttpContext.Current.Response.End();
            }
        }

        public static void ExportExcel(DataView dv, string fileName, string sheetName = "Sheet1")
        {

            IWorkbook workBook = new NPOI.HSSF.UserModel.HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet(sheetName);
            int startRow = 1;
            DataColumnCollection cols = dv.Table.Columns;
            int colsNum = cols.Count;
            IRow row1 = sheet.CreateRow(0);
            for (int i = 0; i < colsNum; i++)
            {
                row1.CreateCell(i).SetCellValue(cols[i].ColumnName);
            }
            foreach (DataRow dr in dv.Table.Rows)
            {

                IRow dataRow = sheet.CreateRow(startRow);
                for (int i = 0; i < colsNum; i++)
                {
                    if (StringHelper.IsIntVal(dr[i].ToString()) && dr[i].ToString().Length < 11)
                        dataRow.CreateCell(i).SetCellValue(int.Parse(dr[i].ToString()));
                    else if (StringHelper.IsDecimalVal(dr[i].ToString()) && dr[i].ToString().Length < 11)
                        dataRow.CreateCell(i).SetCellValue(double.Parse(dr[i].ToString()));
                    else if (StringHelper.IsDateTime(dr[i].ToString()))
                        dataRow.CreateCell(i).SetCellValue(DateTime.Parse(dr[i].ToString()).ToShortDateString());
                    else
                        dataRow.CreateCell(i).SetCellValue(dr[i].ToString());

                }
                startRow++;

            }
            HttpCookie cookie = HttpContext.Current.Request.Cookies["export"];
            if (cookie == null)
            {
                cookie = new HttpCookie("export");
            }
            cookie.Value = "1";
            cookie.Expires = DateTime.Now.AddMinutes(30);
            HttpContext.Current.Response.Cookies.Add(cookie);

            using (MemoryStream ms = new MemoryStream())
            {
                workBook.Write(ms);
                ms.Flush();
                sheet = null;
                workBook = null;
                //DownLoadFile(ms, "综合查询");
                byte[] data = ms.ToArray();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                //HttpContext.Current.Response.Charset = "GB2312";  //设置了类型为中文防止乱码的出现  
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");
                //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.HtmlDecode(fileName) + ".xls");

                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.BinaryWrite(data);
                HttpContext.Current.Response.End();
            }
        }


        public static void ExportTables(Dictionary<string, DataTable> dts, string fileName)
        {
            if (dts.Any())
            {
                IWorkbook workBook = new NPOI.HSSF.UserModel.HSSFWorkbook();

                foreach (KeyValuePair<string, DataTable> dt in dts)
                {
                    ISheet sheet = workBook.CreateSheet(dt.Key);
                    int startRow = 1;
                    DataColumnCollection cols = dt.Value.Columns;
                    int colsNum = cols.Count;
                    IRow row1 = sheet.CreateRow(0);
                    for (int i = 0; i < colsNum; i++)
                    {
                        row1.CreateCell(i).SetCellValue(cols[i].ColumnName);
                    }
                    foreach (DataRow dr in dt.Value.Rows)
                    {

                        IRow dataRow = sheet.CreateRow(startRow);
                        for (int i = 0; i < colsNum; i++)
                        {
                            dataRow.CreateCell(i).SetCellValue(dr[i].ToString());
                        }
                        startRow++;

                    }

                }
                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.Write(ms);
                    ms.Flush();
                    workBook = null;
                    //DownLoadFile(ms, "综合查询");
                    byte[] data = ms.ToArray();
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Charset = "GB2312";  //设置了类型为中文防止乱码的出现  
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");

                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    HttpContext.Current.Response.ContentType = "application/ms-excel";
                    HttpContext.Current.Response.BinaryWrite(data);
                    HttpContext.Current.Response.End();
                }
            }


        }

        /// <summary>
        /// 检查导入Excel中的列名
        /// </summary>
        /// <param name="importCols"></param>
        /// <param name="requireCols"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CheckColumns(DataColumnCollection importCols, List<string> requireCols, out string msg)
        {

            msg = string.Empty;
            if (importCols.Count == 0)
            {
                msg = "导入文件内容为空！";
                return false;
            }
            StringBuilder msgs = new StringBuilder();
            foreach (string s in requireCols)
            {
                bool isOk = false;
                foreach (DataColumn item in importCols)
                {
                    if (item.ColumnName.Trim() == s)
                    {
                        isOk = true;
                        break;
                    }
                }
                if (!isOk)
                {
                    msgs.AppendFormat("{0}，", s);
                }
            }
            if (msgs.Length > 0)
            {
                msg = msgs.Insert(0, "导入文件中，以下列名不存在：<br/>").ToString();
                return false;
            }
            return true;
        }

    }
}

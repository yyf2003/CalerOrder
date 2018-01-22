using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL;
using Models;
using Newtonsoft.Json;
using System.Text;
using Common;
using DAL;

namespace WebApp.TableSize.handler
{
    /// <summary>
    /// HCSmallSizeList 的摘要说明
    /// </summary>
    public class HCSmallSizeList : IHttpHandler
    {
        HttpContext context1;
        public void ProcessRequest(HttpContext context)
        {
            context1 = context;
            context.Response.ContentType = "text/plain";
            string type = string.Empty;
            string result = string.Empty;
            if (context.Request.QueryString["type"] != null)
            {
                type = context.Request.QueryString["type"];
            }
            switch (type)
            {
                case "getList":
                    result = GetSizeList();
                    break;
                case "edit":
                    result = EditSize();
                    break;
                case "delete":
                    result = DeleteSize();
                    break;
                case "getFormat":
                    result = GetFormatList();
                    break;
            }
            context.Response.Write(result);
        }

        string GetSizeList1()
        {
            
            int currPage = 0;
            int pageSize = 0;
            string format = string.Empty;
            if (context1.Request.QueryString["currpage"] != null)
            {
                currPage = int.Parse(context1.Request.QueryString["currpage"]);
            }
            if (context1.Request.QueryString["pagesize"] != null)
            {
                pageSize = int.Parse(context1.Request.QueryString["pagesize"]);
            }
            if (context1.Request.QueryString["format"] != null)
            {
                format = context1.Request.QueryString["format"];
            }
            var list = new HCSmallGraphicSizeBLL().GetList(s => s.Id > 0);
            if (!string.IsNullOrWhiteSpace(format))
            {
                list = list.Where(s => s.Format == format).ToList();
            }
            if (list.Any())
            {
                int totalCount = list.Count;
                list = list.OrderBy(s => s.Format).ThenBy(s => s.BigGraphicWidth).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
                StringBuilder json = new StringBuilder();
                int index = 1;
                list.ForEach(s => {
                    json.Append("{\"rowIndex\":\"" + index + "\",\"Id\":\"" + s.Id + "\",\"Sheet\":\"" + s.Sheet + "\",\"BigGraphicWidth\":\"" + s.BigGraphicWidth + "\",\"BigGraphicLength\":\"" + s.BigGraphicLength + "\",\"SmallGraphicWidth\":\"" + s.SmallGraphicWidth + "\",\"SmallGraphicLength\":\"" + s.SmallGraphicLength + "\",\"Remark\":\"" + s.Remark + "\",\"Format\":\""+s.Format+"\"},");
                    index++;
                });
                if (json.Length > 0)
                    return "{\"total\":" + totalCount + ",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
                else
                    return "{\"total\":0,\"rows\":[] }";
            }
            else
                return "{\"total\":0,\"rows\":[] }";
        }

        StringBuilder json = new StringBuilder();
        string GetSizeList()
        {

            string format = string.Empty;
            if (context1.Request.QueryString["format"] != null)
            {
                format = context1.Request.QueryString["format"];
            }
            var list = new HCSmallGraphicSizeBLL().GetList(s => s.Id > 0).OrderBy(s=>s.Format).ToList();
            if (!string.IsNullOrWhiteSpace(format))
            {
                list = list.Where(s => s.Format == format).ToList();
            }
            if (list.Any())
            {
                //int totalCount = list.Count;
                //var list1 = (from item in list
                //            group item by new
                //            {
                //                item.Format,
                //                item.Sheet,
                //                item.Remark,
                //                item.BigGraphicWidth,
                //                item.BigGraphicLength
                //            } into g
                //            select new { 
                //               g.Key.Format,
                //               g.Key.Sheet,
                //               g.Key.Remark,
                //               g.Key.BigGraphicWidth,
                //               g.Key.BigGraphicLength
                //            }).ToList();
                var list1 = list.Where(s => s.ParentId == 0).OrderBy(s => s.Format).ThenBy(s => s.Remark).ThenBy(s => s.BigGraphicWidth).ToList();
                
                list1.ForEach(s =>
                {
                    json.Append("{\"id\":\"" + s.Id + "\",\"ParentId\":\""+s.ParentId+"\",\"Sheet\":\"" + s.Sheet + "\",\"GraphicWidth\":\"" + s.BigGraphicWidth + "\",\"GraphicLength\":\"" + s.BigGraphicLength + "\",\"Remark\":\"" + s.Remark + "\",\"text\":\"" + s.Format + "\"");
                    GetSubList(list, s.Id);
                    json.Append("},");
                    
                });
                
            }
            if (json.Length > 0)
                return "[" + json.ToString().TrimEnd(',') + "]";
            else
                return "[]";
        }


        void GetSubList(IEnumerable<HCSmallGraphicSize> list, int parentId)
        {
            var list1 = list.Where(s => s.ParentId==parentId).OrderBy(s => s.SmallGraphicWidth).ToList();
            if (list1.Any())
            {
                json.Append(" ,\"state\":\"closed\",\"children\":[");
                int index = 0;
                list1.ForEach(s =>
                {
                    if (index > 0)
                    {
                        json.Append(",");
                    }
                    json.Append("{\"id\":\"" + s.Id + "\",\"ParentId\":\"" + parentId + "\",\"Sheet\":\"" + s.Sheet + "\",\"GraphicWidth\":\"" + s.SmallGraphicWidth + "\",\"GraphicLength\":\"" + s.SmallGraphicLength + "\",\"Remark\":\"" + s.Remark + "\",\"text\":\"" + s.Format + "\"}");
                   
                    index++;
                });
                json.Append("]");
            }
        }

        string EditSize()
        {
            string result = "ok";
            string jsonStr = string.Empty;
            if (context1.Request.QueryString["jsonstr"] != null)
            {
                jsonStr = context1.Request.QueryString["jsonstr"];
            }
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                jsonStr = jsonStr.Replace("+", "%2B");
                jsonStr = HttpUtility.UrlDecode(jsonStr);
                HCSmallGraphicSizeBLL sizeBll = new HCSmallGraphicSizeBLL();
                HCSmallGraphicSize model = JsonConvert.DeserializeObject<HCSmallGraphicSize>(jsonStr);
                if (model != null)
                {

                    if (model.Id > 0)
                    {
                        HCSmallGraphicSize oldModel = sizeBll.GetModel(model.Id);
                        if (oldModel != null)
                        {
                            oldModel.Format = model.Format;
                            oldModel.Remark = model.Remark;
                            oldModel.Sheet = model.Sheet;
                            oldModel.BigGraphicWidth = model.BigGraphicWidth;
                            oldModel.BigGraphicLength = model.BigGraphicLength;
                            sizeBll.Update(oldModel);
                            sizeBll.Delete(s => s.ParentId == oldModel.Id);
                            if (!string.IsNullOrWhiteSpace(model.GraphicSizes))
                            {
                                HCSmallGraphicSize newModel = new HCSmallGraphicSize();
                                string[] sizes = model.GraphicSizes.Split('|');
                                foreach (string s in sizes)
                                {
                                    if (!string.IsNullOrWhiteSpace(s))
                                    {
                                        string widthStr = s.Split('*')[0];
                                        string lengthStr = s.Split('*')[1];
                                        newModel = new HCSmallGraphicSize();
                                        newModel.ParentId = oldModel.Id;
                                        newModel.Format = model.Format;
                                        newModel.Remark = model.Remark;
                                        newModel.Sheet = model.Sheet;
                                        newModel.SmallGraphicWidth = StringHelper.IsDecimal(widthStr);
                                        newModel.SmallGraphicLength = StringHelper.IsDecimal(lengthStr);
                                        sizeBll.Add(newModel);
                                    }
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        HCSmallGraphicSize newModel;
                        if (!string.IsNullOrWhiteSpace(model.GraphicSizes))
                        {
                            var existList = sizeBll.GetList(s=>s.BigGraphicLength==model.BigGraphicLength && s.BigGraphicWidth==model.BigGraphicWidth && s.Format==model.Format);
                            if (!existList.Any())
                            {
                                newModel = new HCSmallGraphicSize();
                                newModel.BigGraphicWidth = model.BigGraphicWidth;
                                newModel.BigGraphicLength = model.BigGraphicLength;
                                newModel.Format = model.Format;
                                newModel.Remark = model.Remark;
                                newModel.Sheet = model.Sheet;
                                newModel.ParentId = 0;
                                sizeBll.Add(newModel);
                                int parentId = newModel.Id;
                                string[] sizes = model.GraphicSizes.Split('|');
                                foreach (string s in sizes)
                                {
                                    if (!string.IsNullOrWhiteSpace(s))
                                    {
                                        string widthStr = s.Split('*')[0];
                                        string lengthStr = s.Split('*')[1];
                                        newModel = new HCSmallGraphicSize();
                                        newModel.ParentId = parentId;
                                        newModel.Format = model.Format;
                                        newModel.Remark = model.Remark;
                                        newModel.Sheet = model.Sheet;
                                        newModel.SmallGraphicWidth = StringHelper.IsDecimal(widthStr);
                                        newModel.SmallGraphicLength = StringHelper.IsDecimal(lengthStr);
                                        sizeBll.Add(newModel);
                                    }
                                }
                            }
                            else
                                result = "exist";
                        }
                    }
                }
            }
            return result;
        }

        string DeleteSize()
        {
            string result = "删除失败";
            string ids = string.Empty;
            if (context1.Request.QueryString["ids"] != null)
            {
                ids = context1.Request.QueryString["ids"];
            }
            HCSmallGraphicSizeBLL sizeBll = new HCSmallGraphicSizeBLL();
            if (!string.IsNullOrWhiteSpace(ids))
            {
                List<int> idList = StringHelper.ToIntList(ids, ',');
                sizeBll.Delete(s => idList.Contains(s.Id));
                result = "ok";
            }
            return result;
        }

        string GetFormatList()
        {
            //string result = string.Empty;
            var list = new HCSmallGraphicSizeBLL().GetList(s => s.Id > 0).Select(s=>s.Format).Distinct().OrderBy(s=>s).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"Format\":\""+s+"\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }

        //string GetModel()
        //{
        //    string result = string.Empty;
        //    int id = 0;
        //    if (context1.Request.QueryString["id"] != null)
        //    {
        //        id = int.Parse(context1.Request.QueryString["id"]);
        //    }
        //    HCSmallGraphicSizeBLL bll = new HCSmallGraphicSizeBLL();
        //    HCSmallGraphicSize model = bll.GetModel(id);
        //    if (model != null)
        //    { 
               
        //    }
        //}

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
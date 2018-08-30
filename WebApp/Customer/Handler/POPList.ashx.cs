using System;
using System.Linq;
using System.Text;
using System.Web;
using BLL;
using DAL;
using Models;
using Newtonsoft.Json;
using Common;
using System.Collections.Generic;
using System.Configuration;

namespace WebApp.Customer.Handler
{
    /// <summary>
    /// POPList 的摘要说明
    /// </summary>
    public class POPList : IHttpHandler
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
                case "getPOP":
                    int id = int.Parse(context.Request.QueryString["id"]);
                    result= GetPOP(id);
                    break;
                case "getSheetList":
                    result = GetSheetList();
                    break;
                case "getMaterialCategory":
                    result = GetMaterialCategory();
                    break;
                case "getOrderMaterial":
                    int categoryId=0;
                    if (context.Request.QueryString["categoryId"] != null)
                        categoryId = int.Parse(context.Request.QueryString["categoryId"]);
                    result = GetOrderMaterial(categoryId);
                    break;
                case "edit":
                    string jsonString = context.Request.QueryString["jsonString"];
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        jsonString = jsonString.Replace("+", "%2B");
                        jsonString = HttpUtility.UrlDecode(jsonString);
                    }
                    result = UpdatePOP(jsonString);
                    break;
                case "getCornerType":
                    string sheet0 = string.Empty;
                    int shopId0 = 0;
                    string gender0 = string.Empty;
                    if (context.Request.QueryString["sheet"] != null)
                        sheet0 = context.Request.QueryString["sheet"];
                    if (context.Request.QueryString["shopId"] != null)
                        shopId0 = int.Parse(context.Request.QueryString["shopId"]);
                    if (context.Request.QueryString["gender"] != null)
                        gender0 = context.Request.QueryString["gender"];
                    result = GetCornerTypeList(sheet0, shopId0, gender0);
                    break;
                case "getFrameList":
                    string sheet = string.Empty;
                    string cornerType = string.Empty;
                    int shopId = 0;
                    string gender = string.Empty;
                    if (context.Request.QueryString["sheet"] != null)
                        sheet = context.Request.QueryString["sheet"];
                    if (context.Request.QueryString["cornerType"] != null)
                        cornerType = context.Request.QueryString["cornerType"];
                    if (context.Request.QueryString["shopId"] != null)
                        shopId = int.Parse(context.Request.QueryString["shopId"]);
                    if (context.Request.QueryString["gender"] != null)
                        gender = context.Request.QueryString["gender"];
                    result = GetFrameNameList(sheet, cornerType, shopId, gender);
                    break;
                case "getGraphicNoPrefix":
                    int shopId1=0;
                    string sheet1=string.Empty;
                    if (context.Request.QueryString["shopId"] != null)
                        shopId1 = int.Parse(context.Request.QueryString["shopId"]);
                    if (context.Request.QueryString["sheet"] != null)
                        sheet1 = context.Request.QueryString["sheet"];
                    result=GetGraphicNoPrefix(shopId1, sheet1);
                    break;
                case "getOutsource":

                    break;
            }

            context.Response.Write(result);
        }

        string GetPOP(int popId)
        {
            var model = (from pop in CurrentContext.DbContext.POP
                         join shop in CurrentContext.DbContext.Shop
                         on pop.ShopId equals shop.Id
                         where pop.Id==popId
                         select new
                         {
                             pop,
                             shop.ShopNo
                         }
                      ).FirstOrDefault();
            if (model != null)
            {
                StringBuilder json = new StringBuilder();
                string IsValid = model.pop.IsValid == false ? "0" : "1";
                json.Append("{\"Id\":\"" + model.pop.Id + "\",\"ShopNo\":\"" + model.ShopNo + "\",\"Sheet\":\"" + model.pop.Sheet + "\",\"GraphicNo\":\"" + model.pop.GraphicNo + "\",\"Gender\":\"" + model.pop.Gender + "\",\"Quantity\":\"" + model.pop.Quantity + "\",\"WindowWide\":\"" + model.pop.WindowWide + "\",\"WindowHigh\":\"" + model.pop.WindowHigh + "\",\"WindowDeep\":\"" + model.pop.WindowDeep + "\",\"WindowSize\":\"" + model.pop.WindowSize + "\",\"GraphicWidth\":\"" + model.pop.GraphicWidth + "\",\"GraphicLength\":\"" + model.pop.GraphicLength + "\",\"DoubleFace\":\"" + model.pop.DoubleFace + "\",\"GraphicMaterial\":\"" + model.pop.GraphicMaterial + "\",\"Glass\":\"" + model.pop.Glass + "\",\"Backdrop\":\"" + model.pop.Backdrop + "\",\"PositionDescription\":\"" + model.pop.PositionDescription + "\",\"Area\":\"" + model.pop.Area + "\",\"Remark\":\"" + model.pop.Remark + "\",\"IsElectricity\":\"" + model.pop.IsElectricity + "\",\"IsHang\":\"" + model.pop.IsHang + "\",\"DoorPosition\":\"" + model.pop.DoorPosition + "\",\"IsValid\":\"" + IsValid + "\",\"OOHInstallPrice\":\"" + model.pop.OOHInstallPrice + "\",\"OSOOHInstallPrice\":\"" + model.pop.OSOOHInstallPrice + "\",\"MaterialCategoryId\":\"" + model.pop.MaterialCategoryId + "\",\"OrderGraphicMaterialId\":\"" + model.pop.OrderGraphicMaterialId + "\",\"Category\":\"" + model.pop.Category + "\",\"CornerType\":\"" + model.pop.CornerType + "\",\"FrameName\":\"" + model.pop.MachineFrameName + "\",\"LeftSideStick\":\"" + model.pop.LeftSideStick + "\",\"RightSideStick\":\"" + model.pop.RightSideStick + "\",\"Floor\":\"" + model.pop.Floor + "\",\"ProduceOutsourceId\":\"" + (model.pop.ProduceOutsourceId??0) + "\"}");
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            else
                return "";
        }

        string GetSheetList()
        {
            var list = new PositionBLL().GetList(s=>1==1);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"SheetName\":\""+s.PositionName+"\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }

        string GetCornerTypeList(string sheet,int shopId,string gender)
        {
            //List<string> list = new BasicMachineFrameBLL().GetList(s => s.Sheet == sheet).Select(s => s.CornerType).Distinct().OrderBy(s => s).ToList();
            List<ShopMachineFrame> frameList = new ShopMachineFrameBLL().GetList(s => s.ShopId == shopId && s.PositionName == sheet);
            if (!string.IsNullOrWhiteSpace(gender))
            {
                frameList = frameList.Where(s => (s.Gender == gender || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女")))).ToList();
            }
            List<string> list = new List<string>();
            if (frameList.Any())
            { 
               list=frameList.Select(s => s.CornerType).Distinct().OrderBy(s => s).ToList();
            }
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        json.Append("{\"CornerType\":\"" + s + "\"},");
                    }
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        string GetFrameNameList(string sheet, string cornerType, int shopId,string gender)
        {
            //List<BasicMachineFrame> frameList = new BasicMachineFrameBLL().GetList(s => s.Sheet == sheet);
            List<ShopMachineFrame> frameList = new ShopMachineFrameBLL().GetList(s => s.ShopId == shopId && s.PositionName == sheet);
            
            if (!string.IsNullOrWhiteSpace(cornerType))
            {
                frameList = frameList.Where(s => s.CornerType == cornerType).ToList();
            }
            else
            {
                frameList = frameList.Where(s => s.CornerType == null || s.CornerType == "").ToList();
            }
            if (!string.IsNullOrWhiteSpace(gender))
            {
                frameList = frameList.Where(s => (s.Gender == gender || (s.Gender.Contains("男") && s.Gender.Contains("女")) || (gender.Contains("男") && gender.Contains("女")))).ToList();
            }
            List<string> list = frameList.Select(s => s.MachineFrame).Distinct().OrderBy(s => s).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s =>
                {
                    json.Append("{\"FrameName\":\"" + s + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            else
                return "";
        }

        POPBLL popBll = new POPBLL();
        string UpdatePOP(string jsonStr)
        {
            string result = "提交失败！";
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                BaseDataChangeLogBLL changeLogBll = new BaseDataChangeLogBLL();
                POPChangeDetailBLL changeDetailBll = new POPChangeDetailBLL();
                POPChangeDetail changeDetailModel;
                POP model = JsonConvert.DeserializeObject<POP>(jsonStr);
                
                if (model != null)
                {
                    //int outsourceId = 0;
                    int shopId = model.ShopId ?? 0;
                    //if (shopId == 0 || (model.ProduceOutsourceId ?? 0) > 0)
                    //    shopId = GetShopId(model.ShopNo, out outsourceId);
                    if (shopId > 0)
                    {
                        //int outsourceId=GetShopOutsourceId(shopId);
                        model.ShopId = shopId;
                        if (!CheckPOP(model))
                        {
                            bool canSave = true;
                            //if ((model.ProduceOutsourceId ?? 0) > 0 && outsourceId>0 && model.ProduceOutsourceId == outsourceId)
                            //{
                            //    result = "生产外协与店铺主外协不能相同";
                            //    canSave = false;
                            //}
                            #region 保存
                            if (canSave)
                            {
                                if (model.Id > 0)
                                {
                                    //更新
                                    POP newModel = popBll.GetModel(model.Id);
                                    if (newModel != null)
                                    {

                                        model.GraphicNo = model.GraphicNo.ToUpper();
                                        popBll.Update(model);
                                        result = "ok";

                                        BaseDataChangeLog logModel = new BaseDataChangeLog();
                                        logModel.AddDate = DateTime.Now;
                                        logModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        logModel.ItemType = (int)BaseDataChangeItemEnum.POP;
                                        logModel.ChangeType = (int)DataChangeTypeEnum.Edit;
                                        logModel.ShopId = shopId;
                                        changeLogBll.Add(logModel);

                                        changeDetailModel = new POPChangeDetail();
                                        changeDetailModel.AddDate = DateTime.Now;
                                        changeDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        changeDetailModel.Category = newModel.Category;
                                        changeDetailModel.ChangeType = "修改前";
                                        changeDetailModel.CornerType = newModel.CornerType;
                                        changeDetailModel.Gender = newModel.Gender;
                                        changeDetailModel.GraphicLength = newModel.GraphicLength;
                                        changeDetailModel.GraphicMaterial = newModel.GraphicMaterial;
                                        changeDetailModel.GraphicNo = newModel.GraphicNo;
                                        changeDetailModel.GraphicWidth = newModel.GraphicWidth;
                                        changeDetailModel.IsValid = newModel.IsValid;
                                        changeDetailModel.OOHInstallPrice = newModel.OOHInstallPrice;
                                        changeDetailModel.PositionDescription = newModel.PositionDescription;
                                        changeDetailModel.Quantity = newModel.Quantity;
                                        changeDetailModel.Remark = newModel.Remark;
                                        changeDetailModel.Sheet = newModel.Sheet;
                                        changeDetailModel.ShopId = newModel.ShopId;
                                        changeDetailModel.WindowDeep = newModel.WindowDeep;
                                        changeDetailModel.WindowHigh = newModel.WindowHigh;
                                        changeDetailModel.WindowSize = newModel.WindowSize;
                                        changeDetailModel.WindowWide = newModel.WindowWide;
                                        changeDetailModel.LogId = logModel.Id;
                                        changeDetailModel.FrameName = newModel.MachineFrameName;
                                        changeDetailBll.Add(changeDetailModel);

                                        changeDetailModel = new POPChangeDetail();
                                        changeDetailModel.AddDate = DateTime.Now.AddSeconds(1);
                                        changeDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                        changeDetailModel.Category = model.Category;
                                        changeDetailModel.ChangeType = "修改后";
                                        changeDetailModel.CornerType = model.CornerType;
                                        changeDetailModel.Gender = model.Gender;
                                        changeDetailModel.GraphicLength = model.GraphicLength;
                                        changeDetailModel.GraphicMaterial = model.GraphicMaterial;
                                        changeDetailModel.GraphicNo = model.GraphicNo;
                                        changeDetailModel.GraphicWidth = model.GraphicWidth;
                                        changeDetailModel.IsValid = model.IsValid;
                                        changeDetailModel.OOHInstallPrice = model.OOHInstallPrice;
                                        changeDetailModel.PositionDescription = model.PositionDescription;
                                        changeDetailModel.Quantity = model.Quantity;
                                        changeDetailModel.Remark = model.Remark;
                                        changeDetailModel.Sheet = model.Sheet;
                                        changeDetailModel.ShopId = model.ShopId;
                                        changeDetailModel.WindowDeep = model.WindowDeep;
                                        changeDetailModel.WindowHigh = model.WindowHigh;
                                        changeDetailModel.WindowSize = model.WindowSize;
                                        changeDetailModel.WindowWide = model.WindowWide;
                                        changeDetailModel.LogId = logModel.Id;

                                        changeDetailModel.FrameName = model.MachineFrameName;
                                        changeDetailBll.Add(changeDetailModel);
                                    }
                                }
                                else
                                {

                                    model.GraphicNo = model.GraphicNo.ToUpper();
                                    popBll.Add(model);

                                    BaseDataChangeLog logModel = new BaseDataChangeLog();
                                    logModel.AddDate = DateTime.Now;
                                    logModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    logModel.ItemType = (int)BaseDataChangeItemEnum.POP;
                                    logModel.ChangeType = (int)DataChangeTypeEnum.Add;
                                    logModel.ShopId = shopId;
                                    changeLogBll.Add(logModel);

                                    changeDetailModel = new POPChangeDetail();
                                    changeDetailModel.AddDate = DateTime.Now;
                                    changeDetailModel.AddUserId = new BasePage().CurrentUser.UserId;
                                    changeDetailModel.Category = model.Category;
                                    changeDetailModel.ChangeType = "新增";
                                    changeDetailModel.CornerType = model.CornerType;
                                    changeDetailModel.Gender = model.Gender;
                                    changeDetailModel.GraphicLength = model.GraphicLength;
                                    changeDetailModel.GraphicMaterial = model.GraphicMaterial;
                                    changeDetailModel.GraphicNo = model.GraphicNo;
                                    changeDetailModel.GraphicWidth = model.GraphicWidth;
                                    changeDetailModel.IsValid = model.IsValid;
                                    changeDetailModel.OOHInstallPrice = model.OOHInstallPrice;
                                    changeDetailModel.PositionDescription = model.PositionDescription;
                                    changeDetailModel.Quantity = model.Quantity;
                                    changeDetailModel.Remark = model.Remark;
                                    changeDetailModel.Sheet = model.Sheet;
                                    changeDetailModel.ShopId = model.ShopId;
                                    changeDetailModel.WindowDeep = model.WindowDeep;
                                    changeDetailModel.WindowHigh = model.WindowHigh;
                                    changeDetailModel.WindowSize = model.WindowSize;
                                    changeDetailModel.WindowWide = model.WindowWide;
                                    changeDetailModel.LogId = logModel.Id;
                                    changeDetailBll.Add(changeDetailModel);
                                    result = "ok";

                                }
                            }
                            #endregion

                        }
                        else
                            result = "该POP已存在";
                    }
                    else
                        result = "该店铺不存在";
                }
            }
            return result;
        }

        bool CheckPOP(POP model)
        {
            var list = popBll.GetList(s=>s.ShopId==model.ShopId && s.Sheet==model.Sheet && s.GraphicNo.ToUpper()==model.GraphicNo.ToUpper());
            if (model.Id > 0)
                list = list.Where(s => s.Id != model.Id).ToList();
            return list.Any();
        }

        int GetShopId(string shopNo,out int outsourceId)
        {
            outsourceId = 0;
            var model = new ShopBLL().GetList(s => s.ShopNo.ToUpper() == shopNo.ToUpper()).FirstOrDefault();
            if (model != null)
            {
                outsourceId = model.OutsourceId ?? 0;
                return model.Id;
            }
            else
                return 0;
        }

        int GetShopOutsourceId(int shopId)
        {
            var model = new ShopBLL().GetModel(shopId);
            if (model != null)
            {
                return (model.OutsourceId ?? 0);
            }
            else
                return 0;
        }

        string GetMaterialCategory()
        {
            var list = new MaterialCategoryBLL().GetList(s=>s.IsDelete==false ||s.IsDelete==null);
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"CategoryName\":\""+s.CategoryName+"\"},");
                });
                return "["+json.ToString().TrimEnd(',')+"]";
            }
            return "";
        }

        string GetOrderMaterial(int categoryId)
        {
            var list = new OrderMaterialMppingBLL().GetList(s => s.BasicCategoryId == categoryId).OrderBy(s => s.OrderMaterialName).ToList();
            if (list.Any())
            {
                StringBuilder json = new StringBuilder();
                list.ForEach(s => {
                    json.Append("{\"Id\":\"" + s.Id + "\",\"OrderMaterialName\":\"" + s.OrderMaterialName + "\"},");
                });
                return "[" + json.ToString().TrimEnd(',') + "]";
            }
            return "";
        }

       


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        string GetGraphicNoPrefix(int shopId,string sheet)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(sheet))
            {
                sheet = sheet.ToUpper();
                string str = string.Empty;
                try
                {
                    str = ConfigurationManager.AppSettings["GraphicNoPrefix"];
                }
                catch
                {

                }
                if (!string.IsNullOrWhiteSpace(str))
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    List<string> list = StringHelper.ToStringList(str,'|');
                    if (list.Any())
                    {
                        list.ForEach(s => {
                            string[] arr = s.Split(':');
                            if (!dic.Keys.Contains(arr[0]))
                            {
                                dic.Add(arr[0], arr[1]);
                            }
                        });
                        
                        StringBuilder POPNo = new StringBuilder();
                        POPNo.Append(dic[sheet]);
                        if (POPNo.Length>0)
                        {
                            POPNo.Append("-#");
                            POPBLL popBll = new POPBLL();
                            int popCount = popBll.GetList(s => s.ShopId == shopId && sheet.Contains(s.Sheet.ToUpper())).Count;
                            
                            string No = (++popCount).ToString().PadLeft(2, '0');
                            result = POPNo.ToString().Replace("#", No);
                            var poplist = popBll.GetList(s => s.ShopId == shopId && s.GraphicNo.ToUpper() == result.ToUpper());
                            while (poplist.Any())
                            {
                                No = (++popCount).ToString().PadLeft(2, '0');
                                result = POPNo.ToString().Replace("#", No);
                                poplist = popBll.GetList(s => s.ShopId == shopId && s.GraphicNo.ToUpper() == result.ToUpper());
                            }
                        }
                    }

                }
            }
            return result;
        }
    }
}
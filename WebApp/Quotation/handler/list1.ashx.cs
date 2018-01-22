using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using BLL;
using DAL;
using Models;
using System.Text;
using Common;

namespace WebApp.Quotation.handler
{
    /// <summary>
    /// list1 的摘要说明
    /// </summary>
    public class list1 : IHttpHandler
    {
        QuotationsBLL quotaltionBll = new QuotationsBLL();
        ADSubjectPriceBLL adPriceBll = new ADSubjectPriceBLL();
        AttachmentBLL attachBll = new AttachmentBLL();
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
            if (context.Request.Form["type"] != null)
            {
                type = context.Request.Form["type"];
            }
            string url = context.Request.Url.PathAndQuery;
            switch (type)
            { 
                case "getlist":
                    
                    int pageSize = int.Parse(context.Request.Form["rows"]);
                    int currPage = int.Parse(context.Request.Form["page"]);
                    
                    result = GetList(pageSize, currPage);
                    break;
                case "sublist":
                    int subjectid=int.Parse(context.Request.QueryString["subjectid"]);
                    result=GetSubList(subjectid);
                    break;
            }
            context.Response.Write(result);
        }

        string GetList(int pageSize, int currPage)
        {
            List<int> curstomerList = new List<int>();
            List<int> companyList = new BasePage().MySonCompanyList.Select(s => s.Id).ToList();
            
            var list = (from subject in CurrentContext.DbContext.Subject
                        join customer in CurrentContext.DbContext.Customer
                        on subject.CustomerId equals customer.Id
                        join user in CurrentContext.DbContext.UserInfo
                        on subject.AddUserId equals user.UserId
                        join supplement1 in CurrentContext.DbContext.AccountCheckSupplement
                        on subject.Id equals supplement1.SubjectId into temp
                        from supplement in temp.DefaultIfEmpty()
                        where (subject.IsDelete == null || subject.IsDelete == false) && (curstomerList.Any() ? (curstomerList.Contains(subject.CustomerId ?? 0)) : 1 == 1) && companyList.Contains(subject.CompanyId ?? 0)
                        select new
                        {
                            subject.Id,
                            subject.AddDate,
                            subject.ApproveState,
                            subject.ApproveUserId,
                            subject.BeginDate,
                            subject.Contact,
                            subject.EndDate,
                            subject.IsDelete,
                            subject.OutSubjectName,
                            subject.Remark,
                            subject.Status,
                            subject.SubjectName,
                            subject.SubjectNo,
                            subject.Tel,
                            subject.CustomerId,
                            customer.CustomerName,
                            user.RealName,
                            supplement.CRNumber,
                            supplement.PONumber,
                            subject.AddUserId,
                            subject.GuidanceId
                        }).ToList();
            int customerId = int.Parse(context1.Request.Form["customerId"]);
            int guidanceId = int.Parse(context1.Request.Form["guidanceId"]);
            //string region = context1.Request.Form["region"];
            //string province = context1.Request.Form["province"];
            //string city = context1.Request.Form["city"];
            var subjectName = context1.Request.Form["subjectName"];
            var subjectNo = context1.Request.Form["subjectNo"];
            var crNumber = context1.Request.Form["crNumber"];
            var poNumber = context1.Request.Form["poNumber"];
            int isSubmitPrice = int.Parse(context1.Request.Form["isSubmitPrice"]);
            var adContacts = context1.Request.Form["adContact"];
            int invoiceState = int.Parse(context1.Request.Form["isInvoice"]);

            string beginDate = context1.Request.Form["beginDate"];
            string endDate = context1.Request.Form["endDate"];
            //if (new BasePage().CurrentUser.RoleId == 2)
            //{
            //    list = list.Where(s => s.AddUserId == new BasePage().CurrentUser.UserId).ToList();
            //}
            if (customerId > 0)
            {
                list = list.Where(s => s.CustomerId == customerId).ToList();
            }
            if (guidanceId > -1)
            {
                list = list.Where(s => s.GuidanceId == guidanceId).ToList();
            }
            //if (!string.IsNullOrWhiteSpace(region))
            //{
            //    List<string> regionList = StringHelper.ToStringList(region, ',', LowerUpperEnum.ToLower);
                
            //}
            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                list = list.Where(s => s.SubjectName.Contains(subjectName.Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(subjectNo))
            {
                list = list.Where(s => s.SubjectNo.Contains(subjectNo.Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(beginDate))
            {
                DateTime begin = DateTime.Parse(beginDate);
                list = list.Where(s => s.BeginDate >= begin).ToList();
            }
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                DateTime end = DateTime.Parse(endDate).AddDays(1);
                list = list.Where(s => s.BeginDate <end).ToList();
            }
            if (!string.IsNullOrWhiteSpace(crNumber))
            {
                list = list.Where(s => s.CRNumber != null && s.CRNumber.Contains(crNumber.Trim())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(poNumber))
            {
                list = list.Where(s => s.PONumber != null && s.PONumber.Contains(poNumber.Trim())).ToList();
            }
            if (isSubmitPrice > 0)
            {
                List<int> subjectIdList = list.Select(s => s.Id).ToList();
                var quList = quotaltionBll.GetList(q => subjectIdList.Contains(q.SubjectId ?? 0));
                if (quList.Any())
                {
                    List<int> submitList = quList.Select(q => q.SubjectId ?? 0).Distinct().ToList();
                    if (isSubmitPrice == 1)
                    {
                        list = list.Where(s => submitList.Contains(s.Id)).ToList();
                    }
                    else
                    {
                        list = list.Where(s => !submitList.Contains(s.Id)).ToList();
                    }
                }
                else
                {
                    list.Clear();
                }
            }

            
            if (!string.IsNullOrWhiteSpace(adContacts))
            {
                string[] contactArr = adContacts.TrimEnd(',').Split(',');
                if (adContacts.Any())
                {
                    List<int> subjectIds = quotaltionBll.GetList(s => contactArr.Contains(s.AdidasContact)).Select(s => s.SubjectId ?? 0).ToList();
                    list = list.Where(s => subjectIds.Contains(s.Id)).ToList();
                }
            }
            StringBuilder json = new StringBuilder();
            decimal total = Math.Ceiling((decimal)list.Count / pageSize);
            int records = list.Count;
            list = list.OrderByDescending(s => s.Id).Skip((currPage - 1) * pageSize).Take(pageSize).ToList();
            list.ForEach(s => {
                string adContact = string.Empty;
                string totalMoney = string.Empty;
                int fileNum = 0;
                string isInvoice = string.Empty;
                GetInfo(s.Id, out adContact, out totalMoney, out fileNum, out isInvoice);
                string price=GetMaterialPrice(s.Id);
                string projectName = s.SubjectName;
                string dateTime=string.Empty;
                if(s.BeginDate!=null && s.EndDate!=null)
                   dateTime = DateTime.Parse(s.BeginDate.ToString()).ToShortDateString() + "至" + DateTime.Parse(s.EndDate.ToString()).ToShortDateString();
                json.Append("{\"Id\":\"" + s.Id + "\",\"CustomerName\":\"" + s.CustomerName + "\",\"SubjectNo\":\"" + s.SubjectNo + "\",\"SubjectName\":\"" + projectName + "\",\"Contact\":\"" + s.Contact + "\",\"ADContact\":\"" + adContact + "\",\"CRNumber\":\"" + s.CRNumber + "\",\"PONumber\":\"" + s.PONumber + "\",\"TotalMoney\":\"" + totalMoney + "\",\"FileNum\":\"" + fileNum + "\",\"IsInvoice\":\"" + isInvoice + "\",\"MateialPrice\":\"" + price + "\",\"SubjectDate\":\"" + dateTime + "\"},");
                
            });
            if (json.Length > 0)
            {
                return "{\"page\":\"" + currPage + "\",\"records\":\"" + records + "\",\"total\":\"" + total + "\",\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
            }
            else
               return "";

        }

        string fileCode = ((int)FileCodeEnum.SubjectQuotation).ToString();
        ADInvoiceBLL invoiceBll = new ADInvoiceBLL();
        void GetInfo(int subjectId, out string adContact, out string totalMoney,out int fileNum,out string isInvoice)
        {
            adContact = string.Empty;
            totalMoney = string.Empty;
            fileNum = 0;
            isInvoice = "否";
            //var list = attachBll.GetList(s => s.SubjectId == subjectId && s.FileCode == fileCode && (s.IsDelete == null || s.IsDelete == false));
            //fileNum = list.Count;


            List<string> contactList = new List<string>();
            System.Text.StringBuilder adContacts = new System.Text.StringBuilder();
            var qList = quotaltionBll.GetList(s => s.SubjectId == subjectId);
            if (qList.Any())
            {
                decimal total = 0;
                qList.ForEach(s =>
                {
                    total += ((s.OfferPrice ?? 0)+(s.OtherPrice??0));
                    if (!contactList.Contains(s.AdidasContact))
                    {
                        contactList.Add(s.AdidasContact);
                    }
                });
               totalMoney= total > 0 ? total.ToString("0.00") : "0";
            }
            if (contactList.Any())
            {
                contactList.ForEach(s =>
                {
                    adContacts.Append(s + "/");
                });
                adContact= adContacts.ToString().TrimEnd('/');
            }
            var invoiceModel = invoiceBll.GetList(s => s.SubjectId == subjectId).FirstOrDefault();
            if (invoiceModel != null)
                isInvoice = "是";
        }

        string GetSubList(int subjectId)
        {
            string r = string.Empty;
            CreateTB();
            var list = (from quotation in CurrentContext.DbContext.Quotations
                        join price in CurrentContext.DbContext.ADSubjectPrice
                       on quotation.Id equals price.QuotationsId
                       where quotation.SubjectId == subjectId
                       select new {
                           quotation.Category,
                           quotation.Id,
                           price.PriceType,
                           price.PriceName,
                           price.Price
                       }).ToList();
            if (list.Any())
            {
                List<int> quotationIdList = new List<int>();
                list.ForEach(s =>
                {
                    if (!quotationIdList.Contains(s.Id))
                    {
                        quotationIdList.Add(s.Id);
                    }

                });
                if (quotationIdList.Any())
                {

                    
                    quotationIdList.ForEach(s =>
                    {
                        //分摊金额
                        var FTList = list.Where(l => l.Id == s && l.PriceType == 1).ToList();
                        //并入金额
                        var BRList = list.Where(l => l.Id == s && l.PriceType == 2).ToList();
                        int FTCount = FTList.Count;
                        int BRCount = BRList.Count;
                        int rowCount = FTCount > BRCount ? FTCount : BRCount;
                        for (int i = 0; i < rowCount; i++)
                        {
                            DataRow dr = priceTB.NewRow();

                            dr["类型"] = FTList[0].Category;
                            if (i < FTCount)
                            {
                                dr["分摊金额名称"] = FTList[i].PriceName;
                                dr["分摊金额"] = FTList[i].Price != null ? FTList[i].Price.ToString() : "";
                            }
                            if (i < BRCount)
                            {
                                dr["并入金额名称"] = BRList[i].PriceName;
                                dr["并入金额"] = BRList[i].Price != null ? BRList[i].Price.ToString() : "";
                            }
                            priceTB.Rows.Add(dr);

                        }
                    });
                }
                
            }
            else
            {
                DataRow dr = priceTB.NewRow();

                dr["类型"] = "无";

                dr["分摊金额名称"] = "无";
                dr["分摊金额"] = "0";

                dr["并入金额名称"] = "无";
                dr["并入金额"] = "0";

                priceTB.Rows.Add(dr);
            }
            if (priceTB.Rows.Count > 0)
            {
                StringBuilder json = new StringBuilder();
                foreach (DataRow dr in priceTB.Rows)
                {
                    json.Append("{\"Category\":\"" + dr["类型"].ToString() + "\",\"FTPriceName\":\"" + dr["分摊金额名称"].ToString() + "\",\"FTPrice\":\"" + dr["分摊金额"].ToString() + "\",\"BRPriceName\":\"" + dr["并入金额名称"].ToString() + "\",\"BRPrice\":\"" + dr["并入金额"].ToString() + "\"},");
                }
                r = "{\"rows\":[" + json.ToString().TrimEnd(',') + "]}";
            }
            return r;
        }

        DataTable priceTB = null;
        void CreateTB()
        {
            priceTB = new DataTable();
            priceTB.Columns.AddRange(new DataColumn[]{
               new DataColumn("类型",Type.GetType("System.String")),
               new DataColumn("分摊金额名称",Type.GetType("System.String")),
               new DataColumn("分摊金额",Type.GetType("System.String")),
               new DataColumn("并入金额名称",Type.GetType("System.String")),
               new DataColumn("并入金额",Type.GetType("System.String"))
            });

        }

        string GetMaterialPrice(int subjectId)
        {
            var list = (from order in CurrentContext.DbContext.FinalOrderDetailTemp
                        where order.SubjectId == subjectId
                        group order by order.GraphicMaterial into g
                        select new
                        {
                            Price = g.Sum(s => (s.Area ?? 0) * (s.UnitPrice ?? 0)*(s.Quantity??1))
                        }).ToList();
            if (list.Any())
            {
                decimal price = 0;
                list.ForEach(s => {
                    price += s.Price;
                });
                return Math.Round(price,2).ToString();
            }
            return "0";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
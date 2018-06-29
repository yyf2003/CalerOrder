using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;
using IDAL;
using System.Data;

namespace BLL
{
    public class QuoteOrderDetailBLL : BaseDAL<QuoteOrderDetail>
    {
        private readonly IQuoteOrderDetailDAL dal = FactoryClass.DataAccess.CreateQuoteOrderDetail();

        public void UpdateRate(string sheet,string subjectId, decimal rate, int quoteItemId)
        {
            dal.UpdateRate(sheet,subjectId, rate, quoteItemId);
        }

        public void UpdateQuoteItemId(string guidanceId, string subjectId, int quoteItemId,string type)
        {
            dal.UpdateQuoteItemId(guidanceId, subjectId, quoteItemId, type);
        }
    }
}

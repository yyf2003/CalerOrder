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

        public void UpdateRate(string subjectId, decimal rate)
        {
            dal.UpdateRate(subjectId, rate);
        }
    }
}

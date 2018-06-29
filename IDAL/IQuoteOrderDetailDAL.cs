using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDAL
{
    public interface IQuoteOrderDetailDAL
    {
        void UpdateRate(string sheet,string subjectId, decimal rate, int quoteItemId);

        /// <summary>
        /// 更新报价单Id
        /// </summary>
        /// <param name="guidanceId"></param>
        /// <param name="subjectId"></param>
        /// <param name="QuoteItemId"></param>
        void UpdateQuoteItemId(string guidanceId, string subjectId, int quoteItemId,string type);
    }
}

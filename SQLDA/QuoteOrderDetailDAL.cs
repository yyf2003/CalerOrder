using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using IDAL;

namespace SQLDAL
{
    public class QuoteOrderDetailDAL:IQuoteOrderDetailDAL
    {
        #region IQuoteOrderDetailDAL 成员

        public void UpdateRate(string sheet,string subjectId, decimal rate,int quoteItemId)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@sheet",sheet),
               new SqlParameter("@subjectId",subjectId),
               new SqlParameter("@rate",rate),
               new SqlParameter("@quoteItemId",quoteItemId)
            };
            SQLHelper.RunProcedure("prop_AddRateToQuoteOrder", paras, "QuoteOrderDetail");
            
        }

        public void UpdateQuoteItemId(string guidanceId, string subjectId, int quoteItemId,string type)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@GuidanceId",guidanceId),
               new SqlParameter("@SubjectId",subjectId),
               new SqlParameter("@QuoteItemId",quoteItemId),
               new SqlParameter("@type",type)
            };
            SQLHelper.RunProcedure("prop_UpdateQuoteItemId", paras, "QuoteOrderDetail");
        }

        #endregion
    }
}

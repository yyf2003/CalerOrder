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

        public void UpdateRate(string subjectId, decimal rate)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@subjectId",subjectId),
               new SqlParameter("@rate",rate)
            };
            SQLHelper.RunProcedure("prop_AddRateToQuoteOrder", paras, "QuoteOrderDetail");
            
        }

        #endregion
    }
}

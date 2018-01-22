using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using IDAL;
using System.Data.SqlClient;

namespace SQLDAL
{
    public class CheckOrderResultDAL:ICheckOrderResult
    {
        public DataSet GetList(int checkOrderId)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@CheckOrderId",checkOrderId),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_GetCheckOrderResultList", paras, "results");
            return ds;
        }
    }
}

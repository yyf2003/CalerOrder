using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IDAL;

namespace SQLDAL
{
    public class MergeOriginalOrderDAL:IMergeOriginalOrder
    {
        public DataSet GetOrderList(int subjectId)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@SubjectId",subjectId),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportMergeOrder", paras, "orders");
            return ds;
        }
    }
}

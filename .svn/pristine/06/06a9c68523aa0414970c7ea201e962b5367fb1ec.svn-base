using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDAL;
using System.Data;
using System.Data.SqlClient;

namespace SQLDAL
{
    public class SupplementOrderDAL:ISupplementOrder
    {
        public DataSet GetOrderList(int subjectId)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@SubjectId",subjectId),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportSupplementOrder", paras, "orders");
            return ds;
        }
    }
}

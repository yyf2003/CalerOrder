using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDAL;
using System.Data;
using System.Data.SqlClient;
using Models;

namespace SQLDAL
{
    public class QuoteMaterialDAL : IQuoteMaterialDAL
    {
        #region IQuoteMaterialDAL 成员

        public DataSet Export(string whereStr)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@where",whereStr),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportQuoteMaterial", paras, "table");
            return ds;
        }

        #endregion
    }
}

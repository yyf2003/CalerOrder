using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IDAL;

namespace SQLDAL
{
    public class BasicMaterialDAL:IBasicMaterialDAL
    {
        public DataSet GetDataList(string where)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@where",where),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportBasicMateial", paras, "table");
            return ds;
        }
    }
}

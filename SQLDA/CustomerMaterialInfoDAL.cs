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
    public class CustomerMaterialInfoDAL:ICustomerMaterialInfoDAL
    {
        #region ICustomerMaterialInfoDAL 成员

        public DataSet GetDataList(string whereStr)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@where",whereStr),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportCustomerMaterial", paras, "table");
            return ds;
        }

        #endregion
    }
}

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
    public class OrderMaterialMappingDAL : IOrderMaterialMapping
    {
        #region IOrderMaterialMapping 成员

        public DataSet GetDataList(string whereStr)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@where",whereStr),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportOrderMaterial", paras, "table");
            return ds;
        }

        #endregion
    }
}

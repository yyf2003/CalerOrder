using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IDAL;

namespace SQLDAL
{
    public class ShopDAL:IShop
    {

        public DataSet GetShopList(string where)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@where",where),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportShop", paras, "shops");
            return ds;
        }

        public DataSet GetShopAndPOPList(string where)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@where",where),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportShopAndPOP", paras, "shops");
            return ds;
        }
    }
}

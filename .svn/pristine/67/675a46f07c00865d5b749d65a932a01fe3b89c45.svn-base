using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IDAL;

namespace SQLDAL
{
    public class ShopMachineFrameDAL:IShopMachineFrame
    {
        #region IShopMachineFrame 成员

        public DataSet GetDataList(string where)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@where",where),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportShopMachineFrame", paras, "ShopMachineFrame");
            return ds;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDAL;
using System.Data;
using System.Data.SqlClient;

namespace SQLDAL
{
    public class BasicMachineFrameDAL:IBasicMachineFrameDAL
    {
        #region IBasicMachineFrameDAL 成员

        public DataSet Export()
        {
            DataSet ds = SQLHelper.RunProcedure("pro_ExportBasicMachineFrame", null, "table");
            return ds;
        }

        #endregion
    }
}

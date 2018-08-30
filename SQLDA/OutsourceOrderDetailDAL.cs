using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using IDAL;

namespace SQLDAL
{
    public class OutsourceOrderDetailDAL:IOutsourceOrderDetailDAL
    {
        #region IOutsourceOrderDetailDAL 成员

        public void DeleteByOrderType(int subjectId, int isRegionOrder)
        {
            SqlParameter[] paras = new SqlParameter[] { 
              new SqlParameter("@subjectId",subjectId),
              new SqlParameter("@isRegionOrder",isRegionOrder)
            };
            SQLHelper.RunProcedure("prop_DeleteOutsourceOrder", paras);
        }

        

        public void DeleteOutsourceActivityOrderPrice(int guidanceId, string shopId, string orderType)
        {
            SqlParameter[] paras = new SqlParameter[] { 
              new SqlParameter("@guidanceId",guidanceId),
              new SqlParameter("@shopId",shopId),
              new SqlParameter("@orderType",orderType)
            };
            SQLHelper.RunProcedure("prop_DeleteOutsourceActivityOrderPrice", paras);
        }

        #endregion
    }
}

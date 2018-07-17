using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IDAL;

namespace SQLDAL
{
    public class FinalOrderDetailTempDAL:IFinalOrderDetailTemp
    {
        public DataSet GetOrderList(string subjectIds,string whereSql,string isFilter)
        {
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@SubjectIds",subjectIds),
               new SqlParameter("@WhereSql",whereSql),
               new SqlParameter("@IsFilter",isFilter)
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportFinalOrderTemp", paras,"orders");
            return ds;
        }
        /// <summary>
        /// 导出外协订单
        /// </summary>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        public DataSet ExportOutsourceOrderList(string whereSql)
        {
            SqlParameter[] paras = new SqlParameter[] { 
              
               new SqlParameter("@WhereSql",whereSql)
              
            };
            DataSet ds = SQLHelper.RunProcedure("pro_ExportOutsourceOrder", paras, "orders");
            return ds;
        }

       public void UpdateAddDate(int subjectId, string addDate, int type)
        {
            SqlParameter[] paras = new SqlParameter[] { 
              
               new SqlParameter("@subjectId",subjectId),
               new SqlParameter("@addDate",addDate),
               new SqlParameter("@type",type)
              
            };
            SQLHelper.RunProcedure("prop_UpdateOrderAddDate", paras, "orders");
        }
    }
}

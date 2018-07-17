using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;
using FactoryClass;
using IDAL;
using System.Data;

namespace BLL
{
    public class FinalOrderDetailTempBLL : BaseDAL<FinalOrderDetailTemp>
    {
        private readonly IFinalOrderDetailTemp dal = DataAccess.CreateFinalOrderDetailTemp();

        public DataSet GetOrderList(string subjectIds,string whereSql,string isFilter)
        {
            return dal.GetOrderList(subjectIds, whereSql, isFilter);
        }

        public DataSet ExportOutsourceOrderList(string whereSql)
        {
            return dal.ExportOutsourceOrderList(whereSql);
        }

        public void UpdateAddDate(int subjectId, string addDate, int type)
        {
            dal.UpdateAddDate(subjectId, addDate, type);
        }
    }
}

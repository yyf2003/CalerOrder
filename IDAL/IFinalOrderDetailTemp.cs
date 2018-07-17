using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace IDAL
{
    public interface IFinalOrderDetailTemp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="isFilter">是否过滤pop尺寸为空的数据</param>
        /// <returns></returns>
        DataSet GetOrderList(string subjectIds,string whereSql,string isFilter);

        DataSet ExportOutsourceOrderList(string whereSql);

        void UpdateAddDate(int subjectId, string addDate, int type);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDAL
{
    public interface IOutsourceOrderDetailDAL
    {
        void DeleteByOrderType(int subjectId,int isRegionOrder);
        
        /// <summary>
        /// 删除外协活动安装费或快递费
        /// </summary>
        /// <param name="guidanceId"></param>
        /// <param name="shopId"></param>
        /// <param name="orderType"></param>
        void DeleteOutsourceActivityOrderPrice(int guidanceId, string shopId,string orderType);
    }
}

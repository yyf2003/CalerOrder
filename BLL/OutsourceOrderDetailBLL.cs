using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDAL;
using DAL;
using Models;
using FactoryClass;

namespace BLL
{
    public class OutsourceOrderDetailBLL : BaseDAL<OutsourceOrderDetail>
    {
        private readonly IOutsourceOrderDetailDAL dal = DataAccess.CreateOutsourceOrderDetail();

        public void DeleteByOrderType(int subjectId, int isRegionOrder)
        {
            dal.DeleteByOrderType(subjectId, isRegionOrder);
        }

        public void DeleteOutsourceActivityOrderPrice(int guidanceId, string shopId, string orderType)
        {
            dal.DeleteOutsourceActivityOrderPrice(guidanceId, shopId, orderType);
        }
    }
}

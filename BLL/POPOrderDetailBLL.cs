using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;
using System.Data;
using IDAL;

namespace BLL
{
    public class POPOrderDetailBLL : BaseDAL<POPOrderDetail>
    {
        private readonly IPOPOrder dal = FactoryClass.DataAccess.CreatePOPOrder();

        public DataSet GetOrderList(int subjectId)
        {
            return dal.GetOrderList(subjectId);
        }
    }
}

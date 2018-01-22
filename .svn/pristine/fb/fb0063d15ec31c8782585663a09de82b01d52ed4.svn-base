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
    public class SupplementOrderDetailBLL : BaseDAL<SupplementOrderDetail>
    {
        private readonly ISupplementOrder dal = FactoryClass.DataAccess.CreateSupplementOrder();

        public DataSet GetOrderList(int subjectId)
        {
            return dal.GetOrderList(subjectId);
        }
    }
}

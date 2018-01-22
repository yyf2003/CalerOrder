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
    public class ListOrderDetailBLL : BaseDAL<ListOrderDetail>
    {
        private readonly IListOrder dal = FactoryClass.DataAccess.CreateListOrder();

        public DataSet GetOrderList(int subjectId)
        {
            return dal.GetOrderList(subjectId);
        }
    }
}

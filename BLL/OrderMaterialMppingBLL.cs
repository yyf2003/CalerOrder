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
    public class OrderMaterialMppingBLL : BaseDAL<OrderMaterialMpping>
    {
        private readonly IOrderMaterialMapping dal = FactoryClass.DataAccess.CreateOrderMaterialMapping();

        public DataSet GetDataList(string where)
        {
            return dal.GetDataList(where);
        }
    }
}

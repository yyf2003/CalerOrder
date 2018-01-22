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
    public class CustomerMaterialInfoBLL : BaseDAL<CustomerMaterialInfo>
    {
        private readonly ICustomerMaterialInfoDAL dal = FactoryClass.DataAccess.CreateCustomerMaterialInfo();

        public DataSet GetDataList(string where)
        {
            return dal.GetDataList(where);
        }
    }
}

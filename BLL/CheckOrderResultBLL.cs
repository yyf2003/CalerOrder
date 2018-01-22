using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;
using IDAL;
using System.Data;

namespace BLL
{
    public class CheckOrderResultBLL : BaseDAL<CheckOrderResult>
    {
        private readonly ICheckOrderResult dal = FactoryClass.DataAccess.CreateCheckOrderResult();
        public DataSet GetList(int checkOrderId)
        {
            return dal.GetList(checkOrderId);
        }
    }
}

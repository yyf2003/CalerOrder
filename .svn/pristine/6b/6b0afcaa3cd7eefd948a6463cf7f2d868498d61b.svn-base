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
    public class QuoteMaterialBLL : BaseDAL<QuoteMaterial>
    {
        private readonly IQuoteMaterialDAL dal = FactoryClass.DataAccess.CreateQuoteMaterial();

        public DataSet Export(string where)
        {
            return dal.Export(where);
        }
    }
}

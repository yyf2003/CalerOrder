using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;
using IDAL;
using Models;

namespace BLL
{
    public class BasicMaterialBLL : BaseDAL<BasicMaterial>
    {
        private readonly IBasicMaterialDAL dal = FactoryClass.DataAccess.CreateBasicMaterial();

        public DataSet GetDataList(string where)
        {
            return dal.GetDataList(where);
        }
    }
}

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
    public class ShopBLL : BaseDAL<Shop>
    {
        private readonly IShop dal = FactoryClass.DataAccess.CreateShop();

        public DataSet GetShopList(string where)
        {
            return dal.GetShopList(where);
        }

        public DataSet GetShopAndPOPList(string where)
        {
            return dal.GetShopAndPOPList(where);
        }
    }
}

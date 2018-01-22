using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;
using Models;
using IDAL;

namespace BLL
{
    public class ShopMachineFrameBLL : BaseDAL<ShopMachineFrame>
    {
        private readonly IShopMachineFrame dal = FactoryClass.DataAccess.CreateShopMachineFrame();

        public DataSet GetDataList(string where)
        {
            return dal.GetDataList(where);
        }
    }
}

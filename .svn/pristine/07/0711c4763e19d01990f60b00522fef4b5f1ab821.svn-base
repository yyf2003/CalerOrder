using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;
using System.Data.SqlClient;
using IDAL;

namespace BLL
{
    public class ModuleBLL : BaseDAL<Module>
    {
        private readonly IModule dal = FactoryClass.DataAccess.CreateModule();
        public List<Module> GetModulesById(int id)
        {
            return dal.GetModulesById(id);
        }
    }
}

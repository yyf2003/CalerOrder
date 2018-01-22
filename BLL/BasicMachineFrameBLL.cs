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
    public class BasicMachineFrameBLL : BaseDAL<BasicMachineFrame>
    {
        private readonly IBasicMachineFrameDAL dal = FactoryClass.DataAccess.CreateBasicMachineFrame();

        public DataSet Export()
        {
            return dal.Export();
        }
    }
}

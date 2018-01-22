using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace IDAL
{
    public interface IModule
    {
        List<Module> GetModulesById(int id);
    }
}

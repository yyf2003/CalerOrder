using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDAL
{
    public interface IBaseDAL<T> : IRepository<T> where T : class
    {
    }
}

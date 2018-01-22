using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace IDAL
{
    public interface IRepository<T> where T : class
    {
        T Add(T entity);
        bool Update(T entity);
        
        bool Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetModel(object key);
        List<T> GetList(Expression<Func<T, bool>> where);
        int ExcuteSql(string sqlStr);
    }
}

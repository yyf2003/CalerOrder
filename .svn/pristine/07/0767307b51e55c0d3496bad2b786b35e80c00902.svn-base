using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using IDAL;
using Models;

namespace DAL
{
    public class BaseDAL<T> : IBaseDAL<T> where T : class
    {
        
        public T Add(T entity)
        {
            using (KalerOrderDBEntities db = new KalerOrderDBEntities())
            {
                db.Entry<T>(entity).State = EntityState.Added;
                db.SaveChanges();
                return entity;
            }
            
        }

        public bool Update(T entity)
        {
            using (KalerOrderDBEntities db = new KalerOrderDBEntities())
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
        }

        public bool Delete(T entity)
        {
            using (KalerOrderDBEntities db = new KalerOrderDBEntities())
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Deleted;
                return db.SaveChanges() > 0;
            }
        }

        public void Delete(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            List<T> list = GetList(where);
            if (list.Any())
            {
                foreach (T entity in list)
                {
                    Delete(entity);
                }
            }
        }

        public T GetModel(object key)
        {
            using (KalerOrderDBEntities db = new KalerOrderDBEntities())
            {
                return db.Set<T>().Find(key);
            }
        }

        public List<T> GetList(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            using (KalerOrderDBEntities db = new KalerOrderDBEntities())
            {
                return db.Set<T>().Where<T>(where).AsNoTracking().ToList();
            }
        }

        public virtual void ExportWithTemplate(List<T> list, string path) { }

        public int ExcuteSql(string sqlStr)
        {
            int row = 0;
            using (KalerOrderDBEntities db = new KalerOrderDBEntities())
            {
                row= db.Database.ExecuteSqlCommand(sqlStr);
            }

            return row;
        }
    }
}

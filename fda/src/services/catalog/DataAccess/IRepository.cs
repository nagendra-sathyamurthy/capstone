using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace catalog.DataAccess
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(string id);
        void Insert(T entity);
        void InsertMany(IEnumerable<T> entities);
        void Update(string id, T entity);
        void Delete(string id);
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);
    }
}

using System.Linq.Expressions;
namespace Crm.DataAccess
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(string id);
        void Insert(T entity);
        void Update(string id, T entity);
        void Delete(string id);
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);
    }
}

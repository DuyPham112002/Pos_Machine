using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_DBAccess.Repository.Base
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T> GetAsync(string id);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null);
        Task Remove(string id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
       
    }
}

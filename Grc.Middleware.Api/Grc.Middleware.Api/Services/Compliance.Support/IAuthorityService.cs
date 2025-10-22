using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Support {
    public interface IAuthorityService : IBaseService {
        int Count();
        int Count(Expression<Func<Authority, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Authority, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Authority, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<Authority, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<Authority, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<Authority, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Authority Get(long id, bool includeDeleted = false);
        Task<Authority> GetAsync(long id, bool includeDeleted = false);
        Authority Get(Expression<Func<Authority, bool>> where, bool includeDeleted = false);
        Authority Get(Expression<Func<Authority, bool>> where, bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes);
        Task<Authority> GetAsync(Expression<Func<Authority, bool>> where, bool includeDeleted = false);
        Task<Authority> GetAsync(Expression<Func<Authority, bool>> where, bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes);
        IQueryable<Authority> GetAll(bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes);
        IList<Authority> GetAll(bool includeDeleted = false);
        Task<IList<Authority>> GetAllAsync(bool includeDeleted = false);
        IList<Authority> GetAll(Expression<Func<Authority, bool>> where, bool includeDeleted);
        Task<IList<Authority>> GetAllAsync(Expression<Func<Authority, bool>> where, bool includeDeleted);
        Task<IList<Authority>> GetAllAsync(Expression<Func<Authority, bool>> where, bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes);
        Task<IList<Authority>> GetAllAsync(bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes);
        Task<IList<Authority>> GetTopAsync(Expression<Func<Authority, bool>> where, int top, bool includeDeleted = false);
        bool Insert(Authority authority);
        Task<bool> InsertAsync(Authority authority);
        bool Update(Authority authority, bool includeDeleted = false);
        Task<bool> UpdateAsync(Authority authority, bool includeDeleted = false);
        bool Delete(Authority authority, bool markAsDeleted = false);
        Task<bool> DeleteAsync(Authority authority, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<Authority> authorities, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(Authority[] authorities);
        Task<bool> BulkyUpdateAsync(Authority[] authorities);
        Task<bool> BulkyUpdateAsync(Authority[] authorities, params Expression<Func<Authority, object>>[] propertySelectors);
        Task<PagedResult<Authority>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Authority, object>>[] includes);
        Task<PagedResult<Authority>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Authority, object>>[] includes);
        Task<PagedResult<Authority>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Authority, bool>> where = null);
        Task<PagedResult<Authority>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Authority, bool>> where = null, bool includeDeleted = false);
    }
}

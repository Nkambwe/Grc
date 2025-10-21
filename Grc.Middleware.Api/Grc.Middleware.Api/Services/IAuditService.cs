using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public interface IAuditService {
        int Count();
        int Count(Expression<Func<Audit, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Audit, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Audit, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<Audit, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<Audit, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<Audit, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Audit Get(long id, bool includeDeleted = false);
        Task<Audit> GetAsync(long id, bool includeDeleted = false);
        Audit Get(Expression<Func<Audit, bool>> where, bool includeDeleted = false);
        Audit Get(Expression<Func<Audit, bool>> where, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes);
        Task<Audit> GetAsync(Expression<Func<Audit, bool>> where, bool includeDeleted = false);
        Task<Audit> GetAsync(Expression<Func<Audit, bool>> where, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes);
        IQueryable<Audit> GetAll(bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes);
        IList<Audit> GetAll(bool includeDeleted = false);
        Task<IList<Audit>> GetAllAsync(bool includeDeleted = false);
        IList<Audit> GetAll(Expression<Func<Audit, bool>> where, bool includeDeleted);
        Task<IList<Audit>> GetAllAsync(Expression<Func<Audit, bool>> where, bool includeDeleted);
        Task<IList<Audit>> GetAllAsync(Expression<Func<Audit, bool>> where, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes);
        Task<IList<Audit>> GetAllAsync(bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes);
        Task<IList<Audit>> GetTopAsync(Expression<Func<Audit, bool>> where, int top, bool includeDeleted = false);
        bool Insert(Audit audit);
        Task<bool> InsertAsync(Audit audit);
        bool Update(Audit audit, bool includeDeleted = false);
        Task<bool> UpdateAsync(Audit audit, bool includeDeleted = false);
        bool Delete(Audit audit, bool markAsDeleted = false);
        Task<bool> DeleteAsync(Audit audit, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<Audit> audits, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(Audit[] audits);
        Task<bool> BulkyUpdateAsync(Audit[] audits);
        Task<bool> BulkyUpdateAsync(Audit[] audits, params Expression<Func<Audit, object>>[] propertySelectors);
        Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Audit, object>>[] includes);
        Task<PagedResult<Audit>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Audit, object>>[] includes);
        Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Audit, bool>> where = null);
        Task<PagedResult<Audit>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Audit, bool>> where = null, bool includeDeleted = false);
    }
}

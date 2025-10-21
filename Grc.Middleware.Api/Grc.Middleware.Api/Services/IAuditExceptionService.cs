using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {

    public interface IAuditExceptionService {
        int Count();
        int Count(Expression<Func<AuditException, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<AuditException, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<AuditException, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditException, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        AuditException Get(long id, bool includeDeleted = false);
        Task<AuditException> GetAsync(long id, bool includeDeleted = false);
        AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false);
        AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false);
        Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        IQueryable<AuditException> GetAll(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        IList<AuditException> GetAll(bool includeDeleted = false);
        Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false);
        IList<AuditException> GetAll(Expression<Func<AuditException, bool>> where, bool includeDeleted);
        Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted);
        Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        Task<IList<AuditException>> GetTopAsync(Expression<Func<AuditException, bool>> where, int top, bool includeDeleted = false);
        bool Insert(AuditException exception);
        Task<bool> InsertAsync(AuditException exception);
        bool Update(AuditException exception, bool includeDeleted = false);
        Task<bool> UpdateAsync(AuditException exception, bool includeDeleted = false);
        bool Delete(AuditException exception, bool markAsDeleted = false);
        Task<bool> DeleteAsync(AuditException exception, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<AuditException> exceptions, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(AuditException[] exceptions);
        Task<bool> BulkyUpdateAsync(AuditException[] exceptions);
        Task<bool> BulkyUpdateAsync(AuditException[] exceptions, params Expression<Func<AuditException, object>>[] propertySelectors);
        Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes);
        Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes);
        Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditException, bool>> where = null);
        Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditException, bool>> where = null, bool includeDeleted = false);
    }
}

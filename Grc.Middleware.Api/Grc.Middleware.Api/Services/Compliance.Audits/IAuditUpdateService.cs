using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Audits {
    public interface IAuditUpdateService {
        int Count();
        int Count(Expression<Func<AuditUpdate, bool>> predicate);
        Task<int> CountAsync(Expression<Func<AuditUpdate, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<AuditUpdate, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<AuditUpdate, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<AuditUpdate> GetAsync(long id, bool includeDeleted = false);
        Task<AuditUpdate> GetAsync(Expression<Func<AuditUpdate, bool>> where, bool includeDeleted = false);
        Task<AuditUpdate> GetAsync(Expression<Func<AuditUpdate, bool>> where, bool includeDeleted = false, params Expression<Func<AuditUpdate, object>>[] includes);
        Task<IList<AuditUpdate>> GetAllAsync(Expression<Func<AuditUpdate, bool>> where, bool includeDeleted);
        Task<IList<AuditUpdate>> GetAllAsync(Expression<Func<AuditUpdate, bool>> where, bool includeDeleted = false, params Expression<Func<AuditUpdate, object>>[] includes);
        Task<IList<AuditUpdate>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditUpdate, object>>[] includes);
        Task<IList<AuditUpdate>> GetAllAsync(Expression<Func<AuditUpdate, bool>> where, bool includeDeleted = false, Func<IQueryable<AuditUpdate>, IQueryable<AuditUpdate>> includeFunc = null);
        Task<bool> InsertAsync(AuditUpdateRequest entity, string username);
        Task<bool> UpdateAsync(AuditUpdateRequest entity, string username, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<PagedResult<AuditUpdate>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditUpdate, object>>[] includes);
        Task<PagedResult<AuditUpdate>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditUpdate, bool>> predicate = null);
        Task<PagedResult<AuditUpdateResponse>> PageLookupAsync<AuditUpdateResponse>(int page, int size, bool includeDeleted, Expression<Func<AuditUpdate, AuditUpdateResponse>> selector);
    }


}

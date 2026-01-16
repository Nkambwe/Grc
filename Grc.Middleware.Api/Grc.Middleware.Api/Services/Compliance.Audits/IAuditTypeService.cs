
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Audits {
    public interface IAuditTypeService {
        int Count();
        int Count(Expression<Func<AuditType, bool>> predicate);
        Task<int> CountAsync(Expression<Func<AuditType, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<AuditType, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<AuditType, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<AuditType> GetAsync(long id, bool includeDeleted = false);
        Task<AuditType> GetAsync(Expression<Func<AuditType, bool>> where, bool includeDeleted = false);
        Task<AuditType> GetAsync(Expression<Func<AuditType, bool>> where, bool includeDeleted = false, params Expression<Func<AuditType, object>>[] includes);
        Task<IList<AuditType>> GetAllAsync(Expression<Func<AuditType, bool>> where, bool includeDeleted);
        Task<IList<AuditType>> GetAllAsync(Expression<Func<AuditType, bool>> where, bool includeDeleted = false, params Expression<Func<AuditType, object>>[] includes);
        Task<IList<AuditType>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditType, object>>[] includes);
        Task<IList<AuditType>> GetAllAsync(Expression<Func<AuditType, bool>> where, bool includeDeleted = false, Func<IQueryable<AuditType>, IQueryable<AuditType>> includeFunc = null);
        Task<bool> InsertAsync(AuditTypeRequest entity, string username);
        Task<bool> UpdateAsync(AuditTypeRequest entity, string username, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<PagedResult<AuditType>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditType, object>>[] includes);
        Task<PagedResult<AuditType>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditType, bool>> predicate = null);
        Task<PagedResult<AuditTypeResponse>> PageLookupAsync<AuditTypeResponse>(int page, int size, bool includeDeleted, Expression<Func<AuditType, AuditTypeResponse>> selector);
    }

}

using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public interface IAuditReportService {
        int Count();
        int Count(Expression<Func<AuditReport, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<AuditReport, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<AuditReport, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<AuditReport, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<AuditReport, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditReport, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        AuditReport Get(long id, bool includeDeleted = false);
        Task<AuditReport> GetAsync(long id, bool includeDeleted = false);
        AuditReport Get(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false);
        AuditReport Get(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes);
        Task<AuditReport> GetAsync(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false);
        Task<AuditReport> GetAsync(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes);
        IQueryable<AuditReport> GetAll(bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes);
        IList<AuditReport> GetAll(bool includeDeleted = false);
        Task<IList<AuditReport>> GetAllAsync(bool includeDeleted = false);
        IList<AuditReport> GetAll(Expression<Func<AuditReport, bool>> where, bool includeDeleted);
        Task<IList<AuditReport>> GetAllAsync(Expression<Func<AuditReport, bool>> where, bool includeDeleted);
        Task<IList<AuditReport>> GetAllAsync(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes);
        Task<IList<AuditReport>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes);
        Task<IList<AuditReport>> GetTopAsync(Expression<Func<AuditReport, bool>> where, int top, bool includeDeleted = false);
        bool Insert(AuditReport auditReport);
        Task<bool> InsertAsync(AuditReport auditReport);
        bool Update(AuditReport auditReport, bool includeDeleted = false);
        Task<bool> UpdateAsync(AuditReport auditReport, bool includeDeleted = false);
        bool Delete(AuditReport auditReport, bool markAsDeleted = false);
        Task<bool> DeleteAsync(AuditReport auditReport, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<AuditReport> auditReports, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(AuditReport[] auditReports);
        Task<bool> BulkyUpdateAsync(AuditReport[] auditReports);
        Task<bool> BulkyUpdateAsync(AuditReport[] auditReports, params Expression<Func<AuditReport, object>>[] propertySelectors);
        Task<PagedResult<AuditReport>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditReport, object>>[] includes);
        Task<PagedResult<AuditReport>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditReport, object>>[] includes);
        Task<PagedResult<AuditReport>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditReport, bool>> where = null);
        Task<PagedResult<AuditReport>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditReport, bool>> where = null, bool includeDeleted = false);
    }
}

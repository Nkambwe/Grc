using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Audits {
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
        bool Insert(AuditReportRequest auditReport, string username);
        Task<bool> InsertAsync(AuditReportRequest auditReport, string username);
        bool Update(AuditReportRequest AuditReportRequest, string username, bool includeDeleted = false);
        Task<bool> UpdateAsync(AuditReportRequest auditReport, string username, bool includeDeleted = false);
        bool Delete(IdRequest auditReport);
        Task<bool> DeleteAsync(IdRequest auditReport);
        Task<bool> DeleteAllAsync(IList<long> entityIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(AuditReportRequest[] auditReports);
        Task<bool> BulkyUpdateAsync(AuditReportRequest[] auditReports);
        Task<bool> BulkyUpdateAsync(AuditReportRequest[] auditReports, params Expression<Func<AuditReport, object>>[] propertySelectors);
        Task<PagedResult<AuditReport>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditReport, object>>[] includes);
        Task<PagedResult<AuditReport>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditReport, object>>[] includes);
        Task<PagedResult<AuditReport>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditReport, bool>> where = null);
        Task<PagedResult<AuditReport>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditReport, bool>> where = null, params Expression<Func<AuditReport, object>>[] includes);
        Task<PagedResult<AuditReport>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditReport, bool>> where = null, bool includeDeleted = false);
    }
}

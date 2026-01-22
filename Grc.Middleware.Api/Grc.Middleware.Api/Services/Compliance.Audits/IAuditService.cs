using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Audits {
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
        bool Insert(AuditRequest audit, string username);
        Task<bool> InsertAsync(AuditRequest audit, string username);
        bool Update(AuditRequest audit, string username, bool includeDeleted = false);
        Task<bool> UpdateAsync(AuditRequest audit, string username, bool includeDeleted = false);
        bool Delete(IdRequest audit);
        Task<bool> DeleteAsync(IdRequest audit);
        Task<bool> DeleteAllAsync(IList<long> auditIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(AuditRequest[] audits);
        Task<bool> BulkyUpdateAsync(AuditRequest[] audits);
        Task<bool> BulkyUpdateAsync(AuditRequest[] audits, params Expression<Func<Audit, object>>[] propertySelectors);
        Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Audit, object>>[] includes);
        Task<PagedResult<Audit>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Audit, object>>[] includes);
        Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Audit, bool>> where = null);
        Task<PagedResult<Audit>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Audit, bool>> where = null, bool includeDeleted = false);
        Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Audit, bool>> predicate = null, params Expression<Func<Audit, object>>[] includes);
        Task<AuditDashboardResponse> GetAuditDashboardStatisticsAsync(bool includeDeletes);
        Task<AuditMiniReportResponse> GetAuditMiniStatisticsAsync(long recordId, bool includeDeleted);
        Task<AuditExtensionStatistics> GetPeriodStatisticsAsync(string period, bool includeDeleted);
        Task<List<AuditMiniReportResponse>> GetMiniPeriodStatisticsAsync(string period, bool includeDeleted);
        Task<AuditSupportResponse> GetAuditSupportItemsAsync(bool includeDeleted);
    }
}

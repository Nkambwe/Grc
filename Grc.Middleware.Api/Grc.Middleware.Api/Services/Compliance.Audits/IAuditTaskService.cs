using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Audits {
    public interface IAuditTaskService {
        int Count();
        int Count(Expression<Func<AuditTask, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<AuditTask, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<AuditTask, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<AuditTask, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<AuditTask, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditTask, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        AuditTask Get(long id, bool includeDeleted = false);
        Task<AuditTask> GetAsync(long id, bool includeDeleted = false);
        AuditTask Get(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false);
        AuditTask Get(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes);
        Task<AuditTask> GetAsync(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false);
        Task<AuditTask> GetAsync(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes);
        IQueryable<AuditTask> GetAll(bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes);
        IList<AuditTask> GetAll(bool includeDeleted = false);
        Task<IList<AuditTask>> GetAllAsync(bool includeDeleted = false);
        IList<AuditTask> GetAll(Expression<Func<AuditTask, bool>> where, bool includeDeleted);
        Task<IList<AuditTask>> GetAllAsync(Expression<Func<AuditTask, bool>> where, bool includeDeleted);
        Task<IList<AuditTask>> GetAllAsync(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes);
        Task<IList<AuditTask>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes);
        Task<IList<AuditTask>> GetTopAsync(Expression<Func<AuditTask, bool>> where, int top, bool includeDeleted = false);
        bool Insert(AuditTaskRequest auditTask);
        Task<bool> InsertAsync(AuditTaskRequest auditTask);
        bool Update(AuditTaskRequest auditTask, bool includeDeleted = false);
        Task<bool> UpdateAsync(AuditTaskRequest auditTask, bool includeDeleted = false);
        bool Delete(IdRequest auditTask);
        Task<bool> DeleteAsync(IdRequest auditTask);
        Task<bool> DeleteAllAsync(IList<long> auditTasks, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(AuditTaskRequest[] auditTasks);
        Task<bool> BulkyUpdateAsync(AuditTaskRequest[] auditTasks);
        Task<bool> BulkyUpdateAsync(AuditTaskRequest[] auditTasks, params Expression<Func<AuditTask, object>>[] propertySelectors);
        Task<PagedResult<AuditTask>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditTask, object>>[] includes);
        Task<PagedResult<AuditTask>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditTask, object>>[] includes);
        Task<PagedResult<AuditTask>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditTask, bool>> where = null);
        Task<PagedResult<AuditTask>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditTask, bool>> where = null, bool includeDeleted = false);
    }
}

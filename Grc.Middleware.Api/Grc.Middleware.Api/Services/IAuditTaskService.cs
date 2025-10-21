using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
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
        bool Insert(AuditTask auditTask);
        Task<bool> InsertAsync(AuditTask auditTask);
        bool Update(AuditTask auditTask, bool includeDeleted = false);
        Task<bool> UpdateAsync(AuditTask auditTask, bool includeDeleted = false);
        bool Delete(AuditTask auditTask, bool markAsDeleted = false);
        Task<bool> DeleteAsync(AuditTask auditTask, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<AuditTask> auditTasks, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(AuditTask[] auditTasks);
        Task<bool> BulkyUpdateAsync(AuditTask[] auditTasks);
        Task<bool> BulkyUpdateAsync(AuditTask[] auditTasks, params Expression<Func<AuditTask, object>>[] propertySelectors);
        Task<PagedResult<AuditTask>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditTask, object>>[] includes);
        Task<PagedResult<AuditTask>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditTask, object>>[] includes);
        Task<PagedResult<AuditTask>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditTask, bool>> where = null);
        Task<PagedResult<AuditTask>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditTask, bool>> where = null, bool includeDeleted = false);
    }
}

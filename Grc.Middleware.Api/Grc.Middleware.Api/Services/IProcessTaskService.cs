using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public interface IProcessTaskService
    {
        int Count();
        int Count(Expression<Func<ProcessTask, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessTask, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessTask, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ProcessTask, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ProcessTask, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessTask, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ProcessTask Get(long id, bool includeDeleted = false);
        Task<ProcessTask> GetAsync(long id, bool includeDeleted = false);
        ProcessTask Get(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false);
        ProcessTask Get(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes);
        Task<ProcessTask> GetAsync(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false);
        Task<ProcessTask> GetAsync(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes);
        IQueryable<ProcessTask> GetAll(bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes);
        IList<ProcessTask> GetAll(bool includeDeleted = false);
        Task<IList<ProcessTask>> GetAllAsync(bool includeDeleted = false);
        IList<ProcessTask> GetAll(Expression<Func<ProcessTask, bool>> where, bool includeDeleted);
        Task<IList<ProcessTask>> GetAllAsync(Expression<Func<ProcessTask, bool>> where, bool includeDeleted);
        Task<IList<ProcessTask>> GetAllAsync(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes);
        Task<IList<ProcessTask>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes);
        Task<IList<ProcessTask>> GetTopAsync(Expression<Func<ProcessTask, bool>> where, int top, bool includeDeleted = false);
        bool Insert(ProcessTask processTask);
        Task<bool> InsertAsync(ProcessTask processTask);
        bool Update(ProcessTask processTask, bool includeDeleted = false);
        Task<bool> UpdateAsync(ProcessTask processTask, bool includeDeleted = false);
        bool Delete(ProcessTask processTask, bool markAsDeleted = false);
        Task<bool> DeleteAsync(ProcessTask processTask, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<ProcessTask> processTasks, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ProcessTask[] processTasks);
        Task<bool> BulkyUpdateAsync(ProcessTask[] processTasks);
        Task<bool> BulkyUpdateAsync(ProcessTask[] processTasks, params Expression<Func<ProcessTask, object>>[] propertySelectors);
        Task<PagedResult<ProcessTask>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessTask, object>>[] includes);
        Task<PagedResult<ProcessTask>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessTask, object>>[] includes);
        Task<PagedResult<ProcessTask>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessTask, bool>> where = null);
        Task<PagedResult<ProcessTask>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessTask, bool>> where = null, bool includeDeleted = false);
    }
}

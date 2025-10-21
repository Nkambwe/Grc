
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public interface IProcessGroupService
    {
        int Count();
        int Count(Expression<Func<ProcessGroup, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessGroup, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessGroup, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ProcessGroup, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ProcessGroup, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessGroup, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ProcessGroup Get(long id, bool includeDeleted = false);
        Task<ProcessGroup> GetAsync(long id, bool includeDeleted = false);
        ProcessGroup Get(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false);
        ProcessGroup Get(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<ProcessGroup> GetAsync(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false);
        Task<ProcessGroup> GetAsync(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        IQueryable<ProcessGroup> GetAll(bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        IList<ProcessGroup> GetAll(bool includeDeleted = false);
        Task<IList<ProcessGroup>> GetAllAsync(bool includeDeleted = false);
        IList<ProcessGroup> GetAll(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted);
        Task<IList<ProcessGroup>> GetAllAsync(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted);
        Task<IList<ProcessGroup>> GetAllAsync(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<IList<ProcessGroup>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<IList<ProcessGroup>> GetTopAsync(Expression<Func<ProcessGroup, bool>> where, int top, bool includeDeleted = false);
        bool Insert(ProcessGroup processGroup);
        Task<bool> InsertAsync(ProcessGroup processGroup);
        bool Update(ProcessGroup processGroup, bool includeDeleted = false);
        Task<bool> UpdateAsync(ProcessGroup processGroup, bool includeDeleted = false);
        bool Delete(ProcessGroup processGroup, bool markAsDeleted = false);
        Task<bool> DeleteAsync(ProcessGroup processGroup, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<ProcessGroup> processGroups, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ProcessGroup[] processGroups);
        Task<bool> BulkyUpdateAsync(ProcessGroup[] processGroups);
        Task<bool> BulkyUpdateAsync(ProcessGroup[] processGroups, params Expression<Func<ProcessGroup, object>>[] propertySelectors);
        Task<PagedResult<ProcessGroup>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<PagedResult<ProcessGroup>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<PagedResult<ProcessGroup>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessGroup, bool>> where = null);
        Task<PagedResult<ProcessGroup>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessGroup, bool>> where = null, bool includeDeleted = false);
    }
}

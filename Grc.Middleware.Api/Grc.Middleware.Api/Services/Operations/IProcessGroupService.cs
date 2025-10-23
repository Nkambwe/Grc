using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Operations {

    public interface IProcessGroupService
    {
        int Count();
        int Count(Expression<Func<ProcessGroup, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessGroup, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessGroup, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ProcessGroup, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ProcessGroup, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessGroup, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ProcessGroup Get(long id, bool includeDeleted = false);
        Task<ProcessGroup> GetAsync(long id, bool includeDeleted = false);
        ProcessGroup Get(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false);
        ProcessGroup Get(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<ProcessGroup> GetAsync(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false);
        Task<ProcessGroup> GetAsync(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        IQueryable<ProcessGroup> GetAll(bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        IList<ProcessGroup> GetAll(bool includeDeleted = false);
        Task<IList<ProcessGroup>> GetAllAsync(bool includeDeleted = false);
        IList<ProcessGroup> GetAll(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted);
        Task<IList<ProcessGroup>> GetAllAsync(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted);
        Task<IList<ProcessGroup>> GetAllAsync(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<IList<ProcessGroup>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<IList<ProcessGroup>> GetTopAsync(Expression<Func<ProcessGroup, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(ProcessGroupRequest request);
        Task<bool> InsertAsync(ProcessGroupRequest request);
        bool Update(ProcessGroupRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ProcessGroupRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ProcessGroupRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ProcessGroupRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ProcessGroupRequest[] requestItems, params Expression<Func<ProcessGroup, object>>[] propertySelectors);
        Task<PagedResult<ProcessGroup>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<PagedResult<ProcessGroup>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessGroup, object>>[] includes);
        Task<PagedResult<ProcessGroup>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessGroup, bool>> predicate = null);
        Task<PagedResult<ProcessGroup>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessGroup, bool>> predicate = null, bool includeDeleted = false);
    }
}

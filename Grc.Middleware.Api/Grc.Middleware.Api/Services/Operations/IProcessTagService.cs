using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Operations {

    public interface IProcessTagService
    {
        int Count();
        int Count(Expression<Func<ProcessTag, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessTag, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessTag, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ProcessTag, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ProcessTag, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessTag, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ProcessTag Get(long id, bool includeDeleted = false);
        Task<ProcessTag> GetAsync(long id, bool includeDeleted = false);
        ProcessTag Get(Expression<Func<ProcessTag, bool>> predicate, bool includeDeleted = false);
        ProcessTag Get(Expression<Func<ProcessTag, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes);
        Task<ProcessTag> GetAsync(Expression<Func<ProcessTag, bool>> predicate, bool includeDeleted = false);
        Task<ProcessTag> GetAsync(Expression<Func<ProcessTag, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes);
        IQueryable<ProcessTag> GetAll(bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes);
        IList<ProcessTag> GetAll(bool includeDeleted = false);
        Task<IList<ProcessTag>> GetAllAsync(bool includeDeleted = false);
        IList<ProcessTag> GetAll(Expression<Func<ProcessTag, bool>> predicate, bool includeDeleted);
        Task<IList<ProcessTag>> GetAllAsync(Expression<Func<ProcessTag, bool>> predicate, bool includeDeleted);
        Task<IList<ProcessTag>> GetAllAsync(Expression<Func<ProcessTag, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes);
        Task<IList<ProcessTag>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes);
        Task<IList<ProcessTag>> GetTopAsync(Expression<Func<ProcessTag, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(ProcessTagRequest request);
        Task<bool> InsertAsync(ProcessTagRequest request);
        bool Update(ProcessTagRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ProcessTagRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ProcessTagRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ProcessTagRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ProcessTagRequest[] requestItems, params Expression<Func<ProcessTag, object>>[] propertySelectors);
        Task<PagedResult<ProcessTag>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessTag, object>>[] includes);
        Task<PagedResult<ProcessTag>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessTag, object>>[] includes);
        Task<PagedResult<ProcessTag>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessTag, bool>> predicate = null);
        Task<PagedResult<ProcessTag>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessTag, bool>> predicate = null, bool includeDeleted = false);
    }
}

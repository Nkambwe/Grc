using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Operations {
    public interface IProcessActivityService {
        int Count();
        int Count(Expression<Func<ProcessActivity, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessActivity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessActivity, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ProcessActivity, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ProcessActivity, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessActivity, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ProcessActivity Get(long id, bool includeDeleted = false);
        Task<ProcessActivity> GetAsync(long id, bool includeDeleted = false);
        ProcessActivity Get(Expression<Func<ProcessActivity, bool>> predicate, bool includeDeleted = false);
        ProcessActivity Get(Expression<Func<ProcessActivity, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes);
        Task<ProcessActivity> GetAsync(Expression<Func<ProcessActivity, bool>> predicate, bool includeDeleted = false);
        Task<ProcessActivity> GetAsync(Expression<Func<ProcessActivity, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes);
        IQueryable<ProcessActivity> GetAll(bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes);
        IList<ProcessActivity> GetAll(bool includeDeleted = false);
        Task<IList<ProcessActivity>> GetAllAsync(bool includeDeleted = false);
        IList<ProcessActivity> GetAll(Expression<Func<ProcessActivity, bool>> predicate, bool includeDeleted);
        Task<IList<ProcessActivity>> GetAllAsync(Expression<Func<ProcessActivity, bool>> predicate, bool includeDeleted);
        Task<IList<ProcessActivity>> GetAllAsync(Expression<Func<ProcessActivity, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes);
        Task<IList<ProcessActivity>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes);
        Task<IList<ProcessActivity>> GetTopAsync(Expression<Func<ProcessActivity, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(ProcessActivityRequest request);
        Task<bool> InsertAsync(ProcessActivityRequest request);
        bool Update(ProcessActivityRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ProcessActivityRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ProcessActivityRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ProcessActivityRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ProcessActivityRequest[] requestItems, params Expression<Func<ProcessActivity, object>>[] propertySelectors);
        Task<PagedResult<ProcessActivity>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessActivity, object>>[] includes);
        Task<PagedResult<ProcessActivity>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessActivity, object>>[] includes);
        Task<PagedResult<ProcessActivity>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessActivity, bool>> predicate = null);
        Task<PagedResult<ProcessActivity>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessActivity, bool>> predicate = null, bool includeDeleted = false);
    }
}

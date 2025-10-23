using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Support {
    public interface IResponsebilityService : IBaseService
    {
        int Count();
        int Count(Expression<Func<Responsebility, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Responsebility, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Responsebility, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<Responsebility, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<Responsebility, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<Responsebility, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Responsebility Get(long id, bool includeDeleted = false);
        Task<Responsebility> GetAsync(long id, bool includeDeleted = false);
        Responsebility Get(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false);
        Responsebility Get(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes);
        Task<Responsebility> GetAsync(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false);
        Task<Responsebility> GetAsync(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes);
        IQueryable<Responsebility> GetAll(bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes);
        IList<Responsebility> GetAll(bool includeDeleted = false);
        Task<IList<Responsebility>> GetAllAsync(bool includeDeleted = false);
        IList<Responsebility> GetAll(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted);
        Task<IList<Responsebility>> GetAllAsync(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted);
        Task<IList<Responsebility>> GetAllAsync(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes);
        Task<IList<Responsebility>> GetAllAsync(bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes);
        Task<IList<Responsebility>> GetTopAsync(Expression<Func<Responsebility, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(ResponsebilityRequest request);
        Task<bool> InsertAsync(ResponsebilityRequest request);
        bool Update(ResponsebilityRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ResponsebilityRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ResponsebilityRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ResponsebilityRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ResponsebilityRequest[] requestItems, params Expression<Func<Responsebility, object>>[] propertySelectors);
        Task<PagedResult<Responsebility>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Responsebility, object>>[] includes);
        Task<PagedResult<Responsebility>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Responsebility, object>>[] includes);
        Task<PagedResult<Responsebility>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Responsebility, bool>> predicate = null);
        Task<PagedResult<Responsebility>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Responsebility, bool>> predicate = null, bool includeDeleted = false);
    }
}

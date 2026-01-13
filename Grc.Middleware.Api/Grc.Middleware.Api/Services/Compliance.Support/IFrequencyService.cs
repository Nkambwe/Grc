using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Support {
    public interface IFrequencyService : IBaseService
    {
        int Count();
        int Count(Expression<Func<Frequency, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Frequency, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Frequency, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<Frequency, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<Frequency, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<Frequency, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Frequency Get(long id, bool includeDeleted = false);
        Task<Frequency> GetAsync(long id, bool includeDeleted = false);
        Frequency Get(Expression<Func<Frequency, bool>> predicate, bool includeDeleted = false);
        Frequency Get(Expression<Func<Frequency, bool>> predicate, bool includeDeleted = false, params Expression<Func<Frequency, object>>[] includes);
        Task<Frequency> GetAsync(Expression<Func<Frequency, bool>> predicate, bool includeDeleted = false);
        Task<Frequency> GetAsync(Expression<Func<Frequency, bool>> predicate, bool includeDeleted = false, params Expression<Func<Frequency, object>>[] includes);
        IQueryable<Frequency> GetAll(bool includeDeleted = false, params Expression<Func<Frequency, object>>[] includes);
        IList<Frequency> GetAll(bool includeDeleted = false);
        Task<IList<Frequency>> GetAllAsync(bool includeDeleted = false);
        IList<Frequency> GetAll(Expression<Func<Frequency, bool>> predicate, bool includeDeleted);
        Task<IList<Frequency>> GetAllAsync(Expression<Func<Frequency, bool>> predicate, bool includeDeleted);
        Task<IList<Frequency>> GetAllAsync(Expression<Func<Frequency, bool>> predicate, bool includeDeleted = false, params Expression<Func<Frequency, object>>[] includes);
        Task<IList<Frequency>> GetAllAsync(bool includeDeleted = false, params Expression<Func<Frequency, object>>[] includes);
        Task<IList<Frequency>> GetTopAsync(Expression<Func<Frequency, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(FrequencyRequest request);
        Task<bool> InsertAsync(FrequencyRequest request);
        bool Update(FrequencyRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(FrequencyRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(FrequencyRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(FrequencyRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(FrequencyRequest[] requestItems, params Expression<Func<Frequency, object>>[] propertySelectors);
        Task<PagedResult<Frequency>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Frequency, object>>[] includes);
        Task<PagedResult<Frequency>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Frequency, object>>[] includes);
        Task<PagedResult<Frequency>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Frequency, bool>> predicate = null);
        Task<PagedResult<Frequency>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Frequency, bool>> predicate = null, bool includeDeleted = false);
        Task<PagedResult<FrequencyResponse>> PageLookupAsync<FrequencyResponse>(int page, int size, bool includeDeleted, Expression<Func<Frequency, FrequencyResponse>> selector);
    }
}

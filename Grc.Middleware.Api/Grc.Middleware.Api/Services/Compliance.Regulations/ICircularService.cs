using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public interface ICircularService {
        int Count();
        int Count(Expression<Func<Circular, bool>> predicate);
        Task<int> CountAsync(Expression<Func<Circular, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<Circular, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<Circular, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Circular> GetAsync(long id, bool includeDeleted = false);
        Task<Circular> GetAsync(Expression<Func<Circular, bool>> predicate, bool includeDeleted = false);
        Task<Circular> GetAsync(Expression<Func<Circular, bool>> predicate, bool includeDeleted = false, params Expression<Func<Circular, object>>[] includes);
        Task<IList<Circular>> GetAllAsync(Expression<Func<Circular, bool>> predicate, bool includeDeleted);
        Task<IList<Circular>> GetAllAsync(Expression<Func<Circular, bool>> predicate, bool includeDeleted = false, params Expression<Func<Circular, object>>[] includes);
        bool Insert(CircularRequest request, string username);
        Task<bool> InsertAsync(CircularRequest request, string username);
        bool Update(CircularRequest request, string username, bool includeDeleted = false);
        Task<bool> UpdateAsync(CircularRequest request, string username, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<PagedResult<CircularResponse>> PageLookupAsync<CircularResponse>(int page, int size, bool includeDeleted, Expression<Func<Circular, CircularResponse>> selector);
        Task<PagedResult<Circular>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Circular, object>>[] includes);
        Task<PagedResult<Circular>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Circular, bool>> predicate = null);
        Task<bool> UpdateSubmissionAsync(CircularSubmissionRequest request, string username);
    }
}

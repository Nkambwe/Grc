using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public interface ICircularIssueService {
        int Count();
        int Count(Expression<Func<CircularIssue, bool>> predicate);
        Task<int> CountAsync(Expression<Func<CircularIssue, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<CircularIssue, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<CircularIssue, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<CircularIssue> GetAsync(long id, bool includeDeleted = false);
        Task<CircularIssue> GetAsync(Expression<Func<CircularIssue, bool>> predicate, bool includeDeleted = false);
        Task<CircularIssue> GetAsync(Expression<Func<CircularIssue, bool>> predicate, bool includeDeleted = false, params Expression<Func<CircularIssue, object>>[] includes);
        Task<IList<CircularIssue>> GetAllAsync(Expression<Func<CircularIssue, bool>> predicate, bool includeDeleted);
        Task<IList<CircularIssue>> GetAllAsync(Expression<Func<CircularIssue, bool>> predicate, bool includeDeleted = false, params Expression<Func<CircularIssue, object>>[] includes);
        bool Insert(CircularIssueRequest request);
        Task<bool> InsertAsync(CircularIssueRequest request);
        bool Update(CircularIssueRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(CircularIssueRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<PagedResult<CircularIssueResponse>> PageLookupAsync<CircularIssueResponse>(int page, int size, bool includeDeleted, Expression<Func<CircularIssue, CircularIssueResponse>> selector);
        Task<PagedResult<CircularIssue>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<CircularIssue, object>>[] includes);
        Task<PagedResult<CircularIssue>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<CircularIssue, bool>> predicate = null);
    }
}

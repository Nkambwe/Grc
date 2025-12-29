using System.Linq.Expressions;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public interface IComplianceIssueService {
        int Count();
        int Count(Expression<Func<ComplianceIssue, bool>> predicate);
        Task<int> CountAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ComplianceIssue, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<ComplianceIssue> GetAsync(long id, bool includeDeleted = false);
        Task<ComplianceIssue> GetAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool includeDeleted = false);
        Task<ComplianceIssue> GetAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool includeDeleted = false, params Expression<Func<ComplianceIssue, object>>[] includes);
        Task<IList<ComplianceIssue>> GetAllAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool includeDeleted);
        Task<IList<ComplianceIssue>> GetAllAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool includeDeleted = false, params Expression<Func<ComplianceIssue, object>>[] includes);
        bool Insert(ComplianceIssueRequest request);
        Task<bool> InsertAsync(ComplianceIssueRequest request);
        bool Update(ComplianceIssueRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ComplianceIssueRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<PagedResult<TResponse>> PageLookupAsync<TResponse>(int page, int size, bool includeDeleted, Expression<Func<ComplianceIssue, TResponse>> selector);
        Task<PagedResult<ComplianceIssue>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ComplianceIssue, object>>[] includes);
        Task<PagedResult<ComplianceIssue>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ComplianceIssue, bool>> predicate = null);
    }

}

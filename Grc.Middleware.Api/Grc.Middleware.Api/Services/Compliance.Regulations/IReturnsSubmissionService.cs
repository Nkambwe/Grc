using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public interface IReturnsSubmissionService {

        #region Queries
        int Count();
        int Count(Expression<Func<ReturnSubmission, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ReturnSubmission, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ReturnSubmission, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ReturnSubmission, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ReturnSubmission, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ReturnSubmission, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ReturnSubmission Get(long id, bool includeDeleted = false);
        Task<ReturnSubmission> GetAsync(long id, bool includeDeleted = false);
        ReturnSubmission Get(Expression<Func<ReturnSubmission, bool>> where, bool includeDeleted = false);
        ReturnSubmission Get(Expression<Func<ReturnSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes);
        Task<ReturnSubmission> GetAsync(Expression<Func<ReturnSubmission, bool>> where, bool includeDeleted = false);
        Task<ReturnSubmission> GetAsync(Expression<Func<ReturnSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes);
        IQueryable<ReturnSubmission> GetAll(bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes);
        IList<ReturnSubmission> GetAll(bool includeDeleted = false);
        Task<IList<ReturnSubmission>> GetAllAsync(bool includeDeleted = false);
        IList<ReturnSubmission> GetAll(Expression<Func<ReturnSubmission, bool>> where, bool includeDeleted);
        Task<IList<ReturnSubmission>> GetAllAsync(Expression<Func<ReturnSubmission, bool>> where, bool includeDeleted);
        Task<IList<ReturnSubmission>> GetAllAsync(Expression<Func<ReturnSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes);
        Task<IList<ReturnSubmission>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes);
        Task<IList<ReturnSubmission>> GetTopAsync(Expression<Func<ReturnSubmission, bool>> where, int top, bool includeDeleted = false);
        bool Insert(ReturnSubmissionRequest submission);
        Task<bool> InsertAsync(ReturnSubmissionRequest submission);
        bool Update(ReturnSubmissionRequest submission, bool includeDeleted = false);
        Task<bool> UpdateAsync(ReturnSubmissionRequest submission, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ReturnSubmissionRequest[] curcularSubmissions);
        Task<bool> BulkyUpdateAsync(ReturnSubmissionRequest[] curcularSubmissions);
        Task<bool> BulkyUpdateAsync(ReturnSubmissionRequest[] curcularSubmissions, params Expression<Func<ReturnSubmission, object>>[] propertySelectors);
        Task<PagedResult<ReturnSubmission>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ReturnSubmission, object>>[] includes);
        Task<PagedResult<ReturnSubmission>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ReturnSubmission, object>>[] includes);
        Task<PagedResult<ReturnSubmission>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ReturnSubmission, bool>> where = null);
        Task<PagedResult<ReturnSubmission>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ReturnSubmission, bool>> where = null, bool includeDeleted = false);
        #endregion

        #region Background Service

        Task<bool> UpdateAsync(SubmissionRequest submission, string username);

        Task GenerateMissingSubmissionsAsync(DateTime today, CancellationToken ct);

        #endregion

    }
}

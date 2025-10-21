using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public interface ICircularSubmissionService {
        int Count();
        int Count(Expression<Func<CircularSubmission, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<CircularSubmission, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<CircularSubmission, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<CircularSubmission, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<CircularSubmission, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<CircularSubmission, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        CircularSubmission Get(long id, bool includeDeleted = false);
        Task<CircularSubmission> GetAsync(long id, bool includeDeleted = false);
        CircularSubmission Get(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false);
        CircularSubmission Get(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes);
        Task<CircularSubmission> GetAsync(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false);
        Task<CircularSubmission> GetAsync(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes);
        IQueryable<CircularSubmission> GetAll(bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes);
        IList<CircularSubmission> GetAll(bool includeDeleted = false);
        Task<IList<CircularSubmission>> GetAllAsync(bool includeDeleted = false);
        IList<CircularSubmission> GetAll(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted);
        Task<IList<CircularSubmission>> GetAllAsync(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted);
        Task<IList<CircularSubmission>> GetAllAsync(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes);
        Task<IList<CircularSubmission>> GetAllAsync(bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes);
        Task<IList<CircularSubmission>> GetTopAsync(Expression<Func<CircularSubmission, bool>> where, int top, bool includeDeleted = false);
        bool Insert(CircularSubmission circularSubmission);
        Task<bool> InsertAsync(CircularSubmission circularSubmission);
        bool Update(CircularSubmission circularSubmission, bool includeDeleted = false);
        Task<bool> UpdateAsync(CircularSubmission circularSubmission, bool includeDeleted = false);
        bool Delete(CircularSubmission circularSubmission, bool markAsDeleted = false);
        Task<bool> DeleteAsync(CircularSubmission circularSubmission, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<CircularSubmission> curcularSubmissions, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(CircularSubmission[] curcularSubmissions);
        Task<bool> BulkyUpdateAsync(CircularSubmission[] curcularSubmissions);
        Task<bool> BulkyUpdateAsync(CircularSubmission[] curcularSubmissions, params Expression<Func<CircularSubmission, object>>[] propertySelectors);
        Task<PagedResult<CircularSubmission>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<CircularSubmission, object>>[] includes);
        Task<PagedResult<CircularSubmission>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<CircularSubmission, object>>[] includes);
        Task<PagedResult<CircularSubmission>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<CircularSubmission, bool>> where = null);
        Task<PagedResult<CircularSubmission>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<CircularSubmission, bool>> where = null, bool includeDeleted = false);
    }
}

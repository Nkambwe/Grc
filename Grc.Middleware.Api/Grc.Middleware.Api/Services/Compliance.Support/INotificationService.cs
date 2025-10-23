using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Support {
    public interface INotificationService : IBaseService
    {
        int Count();
        int Count(Expression<Func<SubmissionNotification, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<SubmissionNotification, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<SubmissionNotification, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<SubmissionNotification, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<SubmissionNotification, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<SubmissionNotification, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        SubmissionNotification Get(long id, bool includeDeleted = false);
        Task<SubmissionNotification> GetAsync(long id, bool includeDeleted = false);
        SubmissionNotification Get(Expression<Func<SubmissionNotification, bool>> predicate, bool includeDeleted = false);
        SubmissionNotification Get(Expression<Func<SubmissionNotification, bool>> predicate, bool includeDeleted = false, params Expression<Func<SubmissionNotification, object>>[] includes);
        Task<SubmissionNotification> GetAsync(Expression<Func<SubmissionNotification, bool>> predicate, bool includeDeleted = false);
        Task<SubmissionNotification> GetAsync(Expression<Func<SubmissionNotification, bool>> predicate, bool includeDeleted = false, params Expression<Func<SubmissionNotification, object>>[] includes);
        IQueryable<SubmissionNotification> GetAll(bool includeDeleted = false, params Expression<Func<SubmissionNotification, object>>[] includes);
        IList<SubmissionNotification> GetAll(bool includeDeleted = false);
        Task<IList<SubmissionNotification>> GetAllAsync(bool includeDeleted = false);
        IList<SubmissionNotification> GetAll(Expression<Func<SubmissionNotification, bool>> predicate, bool includeDeleted);
        Task<IList<SubmissionNotification>> GetAllAsync(Expression<Func<SubmissionNotification, bool>> predicate, bool includeDeleted);
        Task<IList<SubmissionNotification>> GetAllAsync(Expression<Func<SubmissionNotification, bool>> predicate, bool includeDeleted = false, params Expression<Func<SubmissionNotification, object>>[] includes);
        Task<IList<SubmissionNotification>> GetAllAsync(bool includeDeleted = false, params Expression<Func<SubmissionNotification, object>>[] includes);
        Task<IList<SubmissionNotification>> GetTopAsync(Expression<Func<SubmissionNotification, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(NotificationRequest request);
        Task<bool> InsertAsync(NotificationRequest request);
        bool Update(NotificationRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(NotificationRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(NotificationRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(NotificationRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(NotificationRequest[] requestItems, params Expression<Func<SubmissionNotification, object>>[] propertySelectors);
        Task<PagedResult<SubmissionNotification>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<SubmissionNotification, object>>[] includes);
        Task<PagedResult<SubmissionNotification>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<SubmissionNotification, object>>[] includes);
        Task<PagedResult<SubmissionNotification>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<SubmissionNotification, bool>> predicate = null);
        Task<PagedResult<SubmissionNotification>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<SubmissionNotification, bool>> predicate = null, bool includeDeleted = false);
    }
}

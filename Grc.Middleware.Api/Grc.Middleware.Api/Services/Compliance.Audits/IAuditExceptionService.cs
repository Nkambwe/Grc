using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Audits {

    public interface IAuditExceptionService {

        #region Queries
        int Count();
        int Count(Expression<Func<AuditException, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<AuditException, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<AuditException, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditException, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        AuditException Get(long id, bool includeDeleted = false);
        Task<AuditException> GetAsync(long id, bool includeDeleted = false);
        AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false);
        AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false);
        Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        IQueryable<AuditException> GetAll(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        IList<AuditException> GetAll(bool includeDeleted = false);
        Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false);
        IList<AuditException> GetAll(Expression<Func<AuditException, bool>> where, bool includeDeleted);
        Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted);
        Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes);
        Task<IList<AuditException>> GetTopAsync(Expression<Func<AuditException, bool>> where, int top, bool includeDeleted = false);
        bool Insert(AuditExceptionRequest exception, string username);
        Task<bool> InsertAsync(AuditExceptionRequest exception, string username);
        bool Update(AuditExceptionRequest exception, string username, bool includeDeleted = false);
        Task<bool> UpdateAsync(AuditExceptionRequest exception, string username, bool includeDeleted = false);
        bool Delete(IdRequest exception);
        Task<bool> DeleteAsync(IdRequest exception);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(AuditExceptionRequest[] exceptions);
        Task<bool> BulkyUpdateAsync(AuditExceptionRequest[] exceptions);
        Task<bool> BulkyUpdateAsync(AuditExceptionRequest[] exceptions, params Expression<Func<AuditException, object>>[] propertySelectors);
        Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes);
        Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes);
        Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditException, bool>> where = null);
        Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditException, bool>> where = null, bool includeDeleted = false);
        Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditException, bool>> predicate = null, params Expression<Func<AuditException, object>>[] includes);
        Task<PagedResult<AuditExceptionResponse>> PageLookupAsync<AuditExceptionResponse>(int page, int size, bool includeDeleted, Expression<Func<AuditException, AuditExceptionResponse>> selector);

        #endregion

        #region Reports
        Task<List<ExceptionReport>> GetExceptionSummaryReportAsync(bool includeDeleted);
        #endregion
    }
}

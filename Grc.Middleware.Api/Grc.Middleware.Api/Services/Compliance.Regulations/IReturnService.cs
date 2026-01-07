using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public interface IReturnService : IBaseService
    {
        int Count();
        int Count(Expression<Func<ReturnReport, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ReturnReport, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ReturnReport, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ReturnReport, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ReturnReport, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ReturnReport, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ReturnReport Get(long id, bool includeDeleted = false);
        Task<ReturnReport> GetAsync(long id, bool includeDeleted = false);
        ReturnReport Get(Expression<Func<ReturnReport, bool>> where, bool includeDeleted = false);
        ReturnReport Get(Expression<Func<ReturnReport, bool>> where, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes);
        Task<ReturnReport> GetAsync(Expression<Func<ReturnReport, bool>> where, bool includeDeleted = false);
        Task<ReturnReport> GetAsync(Expression<Func<ReturnReport, bool>> where, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes);
        IQueryable<ReturnReport> GetAll(bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes);
        IList<ReturnReport> GetAll(bool includeDeleted = false);
        Task<IList<ReturnReport>> GetAllAsync(bool includeDeleted = false);
        IList<ReturnReport> GetAll(Expression<Func<ReturnReport, bool>> where, bool includeDeleted);
        Task<IList<ReturnReport>> GetAllAsync(Expression<Func<ReturnReport, bool>> where, bool includeDeleted);
        Task<IList<ReturnReport>> GetAllAsync(Expression<Func<ReturnReport, bool>> where, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes);
        Task<IList<ReturnReport>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes);
        Task<IList<ReturnReport>> GetTopAsync(Expression<Func<ReturnReport, bool>> where, int top, bool includeDeleted = false);
        bool Insert(ReturnRequest submission);
        Task<bool> InsertAsync(ReturnRequest submission);
        bool Update(ReturnRequest submission, bool includeDeleted = false);
        Task<bool> UpdateAsync(ReturnRequest submission, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ReturnRequest[] request);
        Task<bool> BulkyUpdateAsync(ReturnRequest[] request);
        Task<bool> BulkyUpdateAsync(ReturnRequest[] request, params Expression<Func<ReturnReport, object>>[] propertySelectors);
        Task<PagedResult<ReturnReport>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ReturnReport, object>>[] includes);
        Task<PagedResult<ReturnReport>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ReturnReport, object>>[] includes);
        Task<PagedResult<ReturnReport>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ReturnReport, bool>> where = null);
        Task<PagedResult<ReturnReport>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ReturnReport, bool>> where = null, bool includeDeleted = false);


        Task<ComplianceStatisticsResponse> GetComplianceStatisticsAsync(bool includeDeleted);
        Task<PolicyDashboardResponse> GetPolicyStatisticsAsync(bool includeDeleted, PolicyStatus status);
        Task<ReturnDashboardResponse> GetReturnStatisticsAsync(bool includeDeleted, ReportPeriod period);
        Task<CircularDashboardResponse> GetCircularStatisticsAsync(bool includeDeleted, string authority);
    }
}

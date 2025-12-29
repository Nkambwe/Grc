using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Support {
    public interface IRegulatoryCategoryService : IBaseService
    {
        int Count();
        int Count(Expression<Func<RegulatoryCategory, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryCategory, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<RegulatoryCategory, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<RegulatoryCategory, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        RegulatoryCategory Get(long id, bool includeDeleted = false);
        Task<RegulatoryCategory> GetAsync(long id, bool includeDeleted = false);
        RegulatoryCategory Get(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false);
        RegulatoryCategory Get(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes);
        Task<RegulatoryCategory> GetAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false);
        Task<RegulatoryCategory> GetAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes);
        IQueryable<RegulatoryCategory> GetAll(bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes);
        IList<RegulatoryCategory> GetAll(bool includeDeleted = false);
        Task<IList<RegulatoryCategory>> GetAllAsync(bool includeDeleted = false);
        IList<RegulatoryCategory> GetAll(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted);
        Task<IList<RegulatoryCategory>> GetAllAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted);
        Task<IList<RegulatoryCategory>> GetAllAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes);
        Task<IList<RegulatoryCategory>> GetAllAsync(bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes);
        Task<IList<RegulatoryCategory>> GetTopAsync(Expression<Func<RegulatoryCategory, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(RegulatoryCategoryRequest request);
        Task<bool> InsertAsync(RegulatoryCategoryRequest request);
        bool Update(RegulatoryCategoryRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(RegulatoryCategoryRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(RegulatoryCategoryRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(RegulatoryCategoryRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(RegulatoryCategoryRequest[] requestItems, params Expression<Func<RegulatoryCategory, object>>[] propertySelectors);
        Task<PagedResult<RegulatoryCategoryResponse>> PageLookupAsync<RegulatoryCategoryResponse>(int page, int size, bool includeDeleted, Expression<Func<RegulatoryCategory, RegulatoryCategoryResponse>> selector);
        Task<PagedResult<RegulatoryCategory>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<RegulatoryCategory, object>>[] includes);
        Task<PagedResult<RegulatoryCategory>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<RegulatoryCategory, object>>[] includes);
        Task<PagedResult<RegulatoryCategory>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<RegulatoryCategory, bool>> predicate = null);
        Task<PagedResult<RegulatoryCategory>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<RegulatoryCategory, bool>> predicate = null, bool includeDeleted = false);
        Task<PagedResult<ObligaionResponse>> GetObligationsAsync(int pageIndex, int pageSize, bool isDeleted);
    }
}

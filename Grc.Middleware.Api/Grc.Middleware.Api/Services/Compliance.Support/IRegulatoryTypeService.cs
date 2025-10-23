using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Support {
    public interface IRegulatoryTypeService : IBaseService
    {
        int Count();
        int Count(Expression<Func<RegulatoryType, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryType, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryType, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<RegulatoryType, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<RegulatoryType, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<RegulatoryType, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        RegulatoryType Get(long id, bool includeDeleted = false);
        Task<RegulatoryType> GetAsync(long id, bool includeDeleted = false);
        RegulatoryType Get(Expression<Func<RegulatoryType, bool>> predicate, bool includeDeleted = false);
        RegulatoryType Get(Expression<Func<RegulatoryType, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryType, object>>[] includes);
        Task<RegulatoryType> GetAsync(Expression<Func<RegulatoryType, bool>> predicate, bool includeDeleted = false);
        Task<RegulatoryType> GetAsync(Expression<Func<RegulatoryType, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryType, object>>[] includes);
        IQueryable<RegulatoryType> GetAll(bool includeDeleted = false, params Expression<Func<RegulatoryType, object>>[] includes);
        IList<RegulatoryType> GetAll(bool includeDeleted = false);
        Task<IList<RegulatoryType>> GetAllAsync(bool includeDeleted = false);
        IList<RegulatoryType> GetAll(Expression<Func<RegulatoryType, bool>> predicate, bool includeDeleted);
        Task<IList<RegulatoryType>> GetAllAsync(Expression<Func<RegulatoryType, bool>> predicate, bool includeDeleted);
        Task<IList<RegulatoryType>> GetAllAsync(Expression<Func<RegulatoryType, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryType, object>>[] includes);
        Task<IList<RegulatoryType>> GetAllAsync(bool includeDeleted = false, params Expression<Func<RegulatoryType, object>>[] includes);
        Task<IList<RegulatoryType>> GetTopAsync(Expression<Func<RegulatoryType, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(RegulatoryTypeRequest request);
        Task<bool> InsertAsync(RegulatoryTypeRequest request);
        bool Update(RegulatoryTypeRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(RegulatoryTypeRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(RegulatoryTypeRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(RegulatoryTypeRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(RegulatoryTypeRequest[] requestItems, params Expression<Func<RegulatoryType, object>>[] propertySelectors);
        Task<PagedResult<RegulatoryType>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<RegulatoryType, object>>[] includes);
        Task<PagedResult<RegulatoryType>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<RegulatoryType, object>>[] includes);
        Task<PagedResult<RegulatoryType>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<RegulatoryType, bool>> predicate = null);
        Task<PagedResult<RegulatoryType>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<RegulatoryType, bool>> predicate = null, bool includeDeleted = false);
    }
}

using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Support {

    public interface IReturnTypeService : IBaseService {
        int Count();
        int Count(Expression<Func<ReturnType, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ReturnType, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ReturnType, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ReturnType, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ReturnType, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ReturnType, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ReturnType Get(long id, bool includeDeleted = false);
        Task<ReturnType> GetAsync(long id, bool includeDeleted = false);
        ReturnType Get(Expression<Func<ReturnType, bool>> predicate, bool includeDeleted = false);
        ReturnType Get(Expression<Func<ReturnType, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnType, object>>[] includes);
        Task<ReturnType> GetAsync(Expression<Func<ReturnType, bool>> predicate, bool includeDeleted = false);
        Task<ReturnType> GetAsync(Expression<Func<ReturnType, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnType, object>>[] includes);
        IQueryable<ReturnType> GetAll(bool includeDeleted = false, params Expression<Func<ReturnType, object>>[] includes);
        IList<ReturnType> GetAll(bool includeDeleted = false);
        Task<IList<ReturnType>> GetAllAsync(bool includeDeleted = false);
        IList<ReturnType> GetAll(Expression<Func<ReturnType, bool>> predicate, bool includeDeleted);
        Task<IList<ReturnType>> GetAllAsync(Expression<Func<ReturnType, bool>> predicate, bool includeDeleted);
        Task<IList<ReturnType>> GetAllAsync(Expression<Func<ReturnType, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnType, object>>[] includes);
        Task<IList<ReturnType>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ReturnType, object>>[] includes);
        Task<IList<ReturnType>> GetTopAsync(Expression<Func<ReturnType, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(ReturnTypeRequest request);
        Task<bool> InsertAsync(ReturnTypeRequest request);
        bool Update(ReturnTypeRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ReturnTypeRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ReturnTypeRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ReturnTypeRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ReturnTypeRequest[] requestItems, params Expression<Func<ReturnType, object>>[] propertySelectors);
        Task<PagedResult<ReturnType>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ReturnType, object>>[] includes);
        Task<PagedResult<ReturnType>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ReturnType, object>>[] includes);
        Task<PagedResult<ReturnType>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ReturnType, bool>> predicate = null);
        Task<PagedResult<ReturnType>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ReturnType, bool>> predicate = null, bool includeDeleted = false);
    }

}

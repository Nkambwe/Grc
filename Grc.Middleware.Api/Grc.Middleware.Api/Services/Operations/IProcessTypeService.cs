using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Operations {
    public interface IProcessTypeService
    {
        int Count();
        int Count(Expression<Func<ProcessType, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessType, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ProcessType, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ProcessType, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ProcessType, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessType, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ProcessType Get(long id, bool includeDeleted = false);
        Task<ProcessType> GetAsync(long id, bool includeDeleted = false);
        ProcessType Get(Expression<Func<ProcessType, bool>> predicate, bool includeDeleted = false);
        ProcessType Get(Expression<Func<ProcessType, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        Task<ProcessType> GetAsync(Expression<Func<ProcessType, bool>> predicate, bool includeDeleted = false);
        Task<ProcessType> GetAsync(Expression<Func<ProcessType, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        IQueryable<ProcessType> GetAll(bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        IList<ProcessType> GetAll(bool includeDeleted = false);
        Task<IList<ProcessType>> GetAllAsync(bool includeDeleted = false);
        IList<ProcessType> GetAll(Expression<Func<ProcessType, bool>> predicate, bool includeDeleted);
        Task<IList<ProcessType>> GetAllAsync(Expression<Func<ProcessType, bool>> predicate, bool includeDeleted);
        Task<IList<ProcessType>> GetAllAsync(Expression<Func<ProcessType, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        Task<IList<ProcessType>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        Task<IList<ProcessType>> GetTopAsync(Expression<Func<ProcessType, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(ProcessTypeRequest request);
        Task<bool> InsertAsync(ProcessTypeRequest request);
        bool Update(ProcessTypeRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ProcessTypeRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ProcessTypeRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ProcessTypeRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(ProcessTypeRequest[] requestItems, params Expression<Func<ProcessType, object>>[] propertySelectors);
        Task<PagedResult<ProcessType>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessType, object>>[] includes);
        Task<PagedResult<ProcessType>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessType, object>>[] includes);
        Task<PagedResult<ProcessType>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessType, bool>> predicate = null);
        Task<PagedResult<ProcessType>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessType, bool>> predicate = null, bool includeDeleted = false);
    }
}

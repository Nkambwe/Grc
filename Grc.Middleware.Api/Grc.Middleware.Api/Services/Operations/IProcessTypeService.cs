using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
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
        bool Exists(Expression<Func<ProcessType, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ProcessType, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessType, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ProcessType Get(long id, bool includeDeleted = false);
        Task<ProcessType> GetAsync(long id, bool includeDeleted = false);
        ProcessType Get(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false);
        ProcessType Get(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        Task<ProcessType> GetAsync(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false);
        Task<ProcessType> GetAsync(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        IQueryable<ProcessType> GetAll(bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        IList<ProcessType> GetAll(bool includeDeleted = false);
        Task<IList<ProcessType>> GetAllAsync(bool includeDeleted = false);
        IList<ProcessType> GetAll(Expression<Func<ProcessType, bool>> where, bool includeDeleted);
        Task<IList<ProcessType>> GetAllAsync(Expression<Func<ProcessType, bool>> where, bool includeDeleted);
        Task<IList<ProcessType>> GetAllAsync(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        Task<IList<ProcessType>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes);
        Task<IList<ProcessType>> GetTopAsync(Expression<Func<ProcessType, bool>> where, int top, bool includeDeleted = false);
        bool Insert(ProcessType processType);
        Task<bool> InsertAsync(ProcessType processType);
        bool Update(ProcessType processType, bool includeDeleted = false);
        Task<bool> UpdateAsync(ProcessType processType, bool includeDeleted = false);
        bool Delete(ProcessType processType, bool markAsDeleted = false);
        Task<bool> DeleteAsync(ProcessType processType, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<ProcessType> processTypes, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ProcessType[] processTypes);
        Task<bool> BulkyUpdateAsync(ProcessType[] processTypes);
        Task<bool> BulkyUpdateAsync(ProcessType[] processTypes, params Expression<Func<ProcessType, object>>[] propertySelectors);
        Task<PagedResult<ProcessType>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessType, object>>[] includes);
        Task<PagedResult<ProcessType>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessType, object>>[] includes);
        Task<PagedResult<ProcessType>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessType, bool>> where = null);
        Task<PagedResult<ProcessType>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessType, bool>> where = null, bool includeDeleted = false);
    }
}

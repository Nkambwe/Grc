using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Operations {
    public interface IOperationProcessService {
        int Count();
        int Count(Expression<Func<OperationProcess, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<OperationProcess, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<OperationProcess, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<OperationProcess, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<OperationProcess, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<OperationProcess, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        OperationProcess Get(long id, bool includeDeleted = false);
        Task<OperationProcess> GetAsync(long id, bool includeDeleted = false);
        OperationProcess Get(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false);
        OperationProcess Get(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes);
        Task<OperationProcess> GetAsync(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false);
        Task<OperationProcess> GetAsync(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes);
        IQueryable<OperationProcess> GetAll(bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes);
        IList<OperationProcess> GetAll(bool includeDeleted = false);
        Task<IList<OperationProcess>> GetAllAsync(bool includeDeleted = false);
        IList<OperationProcess> GetAll(Expression<Func<OperationProcess, bool>> where, bool includeDeleted);
        Task<IList<OperationProcess>> GetAllAsync(Expression<Func<OperationProcess, bool>> where, bool includeDeleted);
        Task<IList<OperationProcess>> GetAllAsync(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes);
        Task<IList<OperationProcess>> GetAllAsync(bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes);
        Task<IList<OperationProcess>> GetTopAsync(Expression<Func<OperationProcess, bool>> where, int top, bool includeDeleted = false);
        bool Insert(OperationProcess process);
        Task<bool> InsertAsync(OperationProcess process);
        bool Update(OperationProcess process, bool includeDeleted = false);
        Task<bool> UpdateAsync(OperationProcess process, bool includeDeleted = false);
        bool Delete(OperationProcess process, bool markAsDeleted = false);
        Task<bool> DeleteAsync(OperationProcess process, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<OperationProcess> processes, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(OperationProcess[] processes);
        Task<bool> BulkyUpdateAsync(OperationProcess[] processes);
        Task<bool> BulkyUpdateAsync(OperationProcess[] processes, params Expression<Func<OperationProcess, object>>[] propertySelectors);
        Task<PagedResult<OperationProcess>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes);
        Task<PagedResult<OperationProcess>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes);
        Task<PagedResult<OperationProcess>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<OperationProcess, bool>> where = null);
        Task<PagedResult<OperationProcess>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<OperationProcess, bool>> where = null, bool includeDeleted = false);
    }
}

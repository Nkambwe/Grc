using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public class OperationProcessService : BaseService, IOperationProcessService
    {
        public OperationProcessService(IServiceLoggerFactory loggerFactory, 
                                       IUnitOfWorkFactory uowFactory, 
                                       IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(OperationProcess[] processes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(OperationProcess[] processes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(OperationProcess[] processes, params Expression<Func<OperationProcess, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<OperationProcess, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<OperationProcess, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<OperationProcess, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(OperationProcess process, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<OperationProcess> processes, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(OperationProcess process, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<OperationProcess, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<OperationProcess, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<OperationProcess, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public OperationProcess Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public OperationProcess Get(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public OperationProcess Get(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<OperationProcess> GetAll(bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<OperationProcess> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<OperationProcess> GetAll(Expression<Func<OperationProcess, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<OperationProcess>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<OperationProcess>> GetAllAsync(Expression<Func<OperationProcess, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<OperationProcess>> GetAllAsync(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<OperationProcess>> GetAllAsync(bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<OperationProcess> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<OperationProcess> GetAsync(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<OperationProcess> GetAsync(Expression<Func<OperationProcess, bool>> where, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<OperationProcess>> GetTopAsync(Expression<Func<OperationProcess, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(OperationProcess process)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(OperationProcess process)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<OperationProcess>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<OperationProcess>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<OperationProcess>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<OperationProcess, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<OperationProcess>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<OperationProcess, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(OperationProcess process, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(OperationProcess process, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}

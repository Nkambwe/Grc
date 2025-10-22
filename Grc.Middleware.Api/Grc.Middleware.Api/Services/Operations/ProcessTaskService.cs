using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Operations {
    public class ProcessTaskService : BaseService, IProcessTaskService
    {
        public ProcessTaskService(IServiceLoggerFactory loggerFactory, 
                                  IUnitOfWorkFactory uowFactory, 
                                  IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(ProcessTask[] processTasks)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessTask[] processTasks)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessTask[] processTasks, params Expression<Func<ProcessTask, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<ProcessTask, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<ProcessTask, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<ProcessTask, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ProcessTask processTask, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<ProcessTask> processTasks, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ProcessTask processTask, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<ProcessTask, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<ProcessTask, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessTask, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ProcessTask Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessTask Get(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessTask Get(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProcessTask> GetAll(bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessTask> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessTask> GetAll(Expression<Func<ProcessTask, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTask>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTask>> GetAllAsync(Expression<Func<ProcessTask, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTask>> GetAllAsync(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTask>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessTask> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessTask> GetAsync(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessTask> GetAsync(Expression<Func<ProcessTask, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTask>> GetTopAsync(Expression<Func<ProcessTask, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(ProcessTask processTask)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(ProcessTask processTask)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessTask>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessTask>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessTask>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessTask, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessTask>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessTask, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(ProcessTask processTask, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ProcessTask processTask, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}

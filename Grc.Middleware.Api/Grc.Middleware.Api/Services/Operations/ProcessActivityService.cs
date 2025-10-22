using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Operations {
    public class ProcessActivityService : BaseService, IProcessActivityService
    {
        public ProcessActivityService(IServiceLoggerFactory loggerFactory,
                                      IUnitOfWorkFactory uowFactory,
                                      IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(ProcessActivity[] activities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessActivity[] activities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessActivity[] activities, params Expression<Func<ProcessActivity, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<ProcessActivity, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<ProcessActivity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<ProcessActivity, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ProcessActivity activity, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<ProcessActivity> activities, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ProcessActivity activity, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<ProcessActivity, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<ProcessActivity, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessActivity, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ProcessActivity Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessActivity Get(Expression<Func<ProcessActivity, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessActivity Get(Expression<Func<ProcessActivity, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProcessActivity> GetAll(bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessActivity> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessActivity> GetAll(Expression<Func<ProcessActivity, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessActivity>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessActivity>> GetAllAsync(Expression<Func<ProcessActivity, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessActivity>> GetAllAsync(Expression<Func<ProcessActivity, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessActivity>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessActivity> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessActivity> GetAsync(Expression<Func<ProcessActivity, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessActivity> GetAsync(Expression<Func<ProcessActivity, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessActivity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessActivity>> GetTopAsync(Expression<Func<ProcessActivity, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(ProcessActivity activity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(ProcessActivity activity)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessActivity>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessActivity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessActivity>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessActivity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessActivity>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessActivity, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessActivity>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessActivity, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(ProcessActivity activity, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ProcessActivity activity, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }

}

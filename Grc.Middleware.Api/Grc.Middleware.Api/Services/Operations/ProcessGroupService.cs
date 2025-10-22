using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Operations {
    public class ProcessGroupService : BaseService, IProcessGroupService
    {
        public ProcessGroupService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory,
            IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(ProcessGroup[] processGroups)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessGroup[] processGroups)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessGroup[] processGroups, params Expression<Func<ProcessGroup, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<ProcessGroup, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<ProcessGroup, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<ProcessGroup, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ProcessGroup processGroup, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<ProcessGroup> processGroups, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ProcessGroup processGroup, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<ProcessGroup, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<ProcessGroup, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessGroup, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ProcessGroup Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessGroup Get(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessGroup Get(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProcessGroup> GetAll(bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessGroup> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessGroup> GetAll(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessGroup>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessGroup>> GetAllAsync(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessGroup>> GetAllAsync(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessGroup>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessGroup> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessGroup> GetAsync(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessGroup> GetAsync(Expression<Func<ProcessGroup, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessGroup>> GetTopAsync(Expression<Func<ProcessGroup, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(ProcessGroup processGroup)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(ProcessGroup processGroup)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessGroup>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessGroup>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessGroup>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessGroup, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessGroup>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessGroup, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(ProcessGroup processGroup, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ProcessGroup processGroup, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}

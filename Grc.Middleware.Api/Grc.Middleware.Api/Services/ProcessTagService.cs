using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public class ProcessTagService : BaseService, IProcessTagService
    {
        public ProcessTagService(IServiceLoggerFactory loggerFactory, 
                                 IUnitOfWorkFactory uowFactory, 
                                 IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(ProcessTag[] processTags)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessTag[] processTags)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessTag[] processTags, params Expression<Func<ProcessTag, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<ProcessTag, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<ProcessTag, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<ProcessTag, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ProcessTag circularSubmission, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<ProcessTag> processTags, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ProcessTag circularSubmission, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<ProcessTag, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<ProcessTag, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessTag, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ProcessTag Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessTag Get(Expression<Func<ProcessTag, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessTag Get(Expression<Func<ProcessTag, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProcessTag> GetAll(bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessTag> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessTag> GetAll(Expression<Func<ProcessTag, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTag>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTag>> GetAllAsync(Expression<Func<ProcessTag, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTag>> GetAllAsync(Expression<Func<ProcessTag, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTag>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessTag> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessTag> GetAsync(Expression<Func<ProcessTag, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessTag> GetAsync(Expression<Func<ProcessTag, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessTag, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessTag>> GetTopAsync(Expression<Func<ProcessTag, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(ProcessTag circularSubmission)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(ProcessTag circularSubmission)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessTag>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessTag, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessTag>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessTag, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessTag>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessTag, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessTag>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessTag, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(ProcessTag circularSubmission, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ProcessTag circularSubmission, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}

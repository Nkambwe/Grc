using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public class ProcessTypeService : BaseService, IProcessTypeService
    {
        public ProcessTypeService(IServiceLoggerFactory loggerFactory, 
                                  IUnitOfWorkFactory uowFactory, 
                                  IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(ProcessType[] processTypes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessType[] processTypes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(ProcessType[] processTypes, params Expression<Func<ProcessType, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<ProcessType, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<ProcessType, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<ProcessType, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ProcessType processType, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<ProcessType> processTypes, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ProcessType processType, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<ProcessType, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<ProcessType, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessType, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ProcessType Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessType Get(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public ProcessType Get(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProcessType> GetAll(bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessType> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<ProcessType> GetAll(Expression<Func<ProcessType, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessType>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessType>> GetAllAsync(Expression<Func<ProcessType, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessType>> GetAllAsync(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessType>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessType> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessType> GetAsync(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessType> GetAsync(Expression<Func<ProcessType, bool>> where, bool includeDeleted = false, params Expression<Func<ProcessType, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProcessType>> GetTopAsync(Expression<Func<ProcessType, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(ProcessType processType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(ProcessType processType)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessType>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessType, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessType>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessType, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessType>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessType, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProcessType>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessType, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(ProcessType processType, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ProcessType processType, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}

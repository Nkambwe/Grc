using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services
{
    public class CircularSubmissionService : BaseService, ICircularSubmissionService
    {
        public CircularSubmissionService(IServiceLoggerFactory loggerFactory, 
                                         IUnitOfWorkFactory uowFactory, 
                                         IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<CircularSubmission, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<CircularSubmission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<CircularSubmission, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<CircularSubmission, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<CircularSubmission, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<CircularSubmission, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public CircularSubmission Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public CircularSubmission Get(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public CircularSubmission Get(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CircularSubmission> GetAll(bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<CircularSubmission> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<CircularSubmission> GetAll(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CircularSubmission>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CircularSubmission>> GetAllAsync(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CircularSubmission>> GetAllAsync(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CircularSubmission>> GetAllAsync(bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<CircularSubmission> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<CircularSubmission> GetAsync(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<CircularSubmission> GetAsync(Expression<Func<CircularSubmission, bool>> where, bool includeDeleted = false, params Expression<Func<CircularSubmission, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CircularSubmission>> GetTopAsync(Expression<Func<CircularSubmission, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(CircularSubmission circularSubmission)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(CircularSubmission circularSubmission)
        {
            throw new NotImplementedException();
        }

        public bool Update(CircularSubmission circularSubmission, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(CircularSubmission circularSubmission, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Delete(CircularSubmission circularSubmission, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<CircularSubmission> curcularSubmissions, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(CircularSubmission circularSubmission, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyInsertAsync(CircularSubmission[] curcularSubmissions)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(CircularSubmission[] curcularSubmissions)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(CircularSubmission[] curcularSubmissions, params Expression<Func<CircularSubmission, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CircularSubmission>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<CircularSubmission, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CircularSubmission>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<CircularSubmission, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CircularSubmission>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<CircularSubmission, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CircularSubmission>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<CircularSubmission, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

    }
}

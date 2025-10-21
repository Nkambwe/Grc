using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public class AuthorityService : BaseService, IAuthorityService
    {
        public AuthorityService(IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory uowFactory, 
            IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Authority, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<Authority, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<Authority, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<Authority, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Authority, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<Authority, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Authority Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Authority Get(Expression<Func<Authority, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Authority Get(Expression<Func<Authority, bool>> where, bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Authority> GetAll(bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<Authority> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<Authority> GetAll(Expression<Func<Authority, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Authority>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Authority>> GetAllAsync(Expression<Func<Authority, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Authority>> GetAllAsync(Expression<Func<Authority, bool>> where, bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Authority>> GetAllAsync(bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<Authority> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<Authority> GetAsync(Expression<Func<Authority, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<Authority> GetAsync(Expression<Func<Authority, bool>> where, bool includeDeleted = false, params Expression<Func<Authority, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Authority>> GetTopAsync(Expression<Func<Authority, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Authority authority)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(Authority authority)
        {
            throw new NotImplementedException();
        }

        public bool Update(Authority authority, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Authority authority, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Authority authority, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<Authority> authorities, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Authority authority, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyInsertAsync(Authority[] authorities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(Authority[] authorities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(Authority[] authorities, params Expression<Func<Authority, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Authority>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Authority, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Authority>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Authority, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Authority>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Authority, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Authority>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Authority, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }


    }
}

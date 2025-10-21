using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public class AuditExceptionService : BaseService, IAuditExceptionService
    {
        public AuditExceptionService(IServiceLoggerFactory loggerFactory, 
                                     IUnitOfWorkFactory uowFactory, 
                                     IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(AuditException[] exceptions)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(AuditException[] exceptions)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(AuditException[] exceptions, params Expression<Func<AuditException, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<AuditException, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AuditException exception, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<AuditException> exceptions, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(AuditException exception, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<AuditException, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<AuditException, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditException, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public AuditException Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<AuditException> GetAll(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<AuditException> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<AuditException> GetAll(Expression<Func<AuditException, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<AuditException> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditException>> GetTopAsync(Expression<Func<AuditException, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(AuditException exception)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(AuditException exception)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditException, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditException, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(AuditException exception, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(AuditException exception, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}

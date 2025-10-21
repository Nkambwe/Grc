using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public class AuditService : BaseService, IAuditService
    {
        public AuditService(IServiceLoggerFactory loggerFactory, 
                            IUnitOfWorkFactory uowFactory, 
                            IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Audit, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<Audit, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<Audit, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<Audit, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Audit, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<Audit, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Audit Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Audit Get(Expression<Func<Audit, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Audit Get(Expression<Func<Audit, bool>> where, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Audit> GetAll(bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<Audit> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<Audit> GetAll(Expression<Func<Audit, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Audit>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Audit>> GetAllAsync(Expression<Func<Audit, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Audit>> GetAllAsync(Expression<Func<Audit, bool>> where, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Audit>> GetAllAsync(bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<Audit> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<Audit> GetAsync(Expression<Func<Audit, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<Audit> GetAsync(Expression<Func<Audit, bool>> where, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Audit>> GetTopAsync(Expression<Func<Audit, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Audit audit)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(Audit audit)
        {
            throw new NotImplementedException();
        }

        public bool Update(Audit audit, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Audit audit, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Audit audit, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<Audit> audits, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Audit audit, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyInsertAsync(Audit[] audits)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(Audit[] audits)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(Audit[] audits, params Expression<Func<Audit, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Audit, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Audit>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Audit, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Audit, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Audit>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Audit, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

    }
}

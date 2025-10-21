using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public class AuditTaskService : BaseService, IAuditTaskService
    {
        public AuditTaskService(IServiceLoggerFactory loggerFactory, 
                                IUnitOfWorkFactory uowFactory, 
                                IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(AuditTask[] auditTasks)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(AuditTask[] auditTasks)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(AuditTask[] auditTasks, params Expression<Func<AuditTask, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<AuditTask, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<AuditTask, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<AuditTask, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AuditTask auditTask, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<AuditTask> auditTasks, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(AuditTask auditTask, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<AuditTask, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<AuditTask, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditTask, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public AuditTask Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public AuditTask Get(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public AuditTask Get(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<AuditTask> GetAll(bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<AuditTask> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<AuditTask> GetAll(Expression<Func<AuditTask, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditTask>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditTask>> GetAllAsync(Expression<Func<AuditTask, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditTask>> GetAllAsync(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditTask>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<AuditTask> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<AuditTask> GetAsync(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<AuditTask> GetAsync(Expression<Func<AuditTask, bool>> where, bool includeDeleted = false, params Expression<Func<AuditTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditTask>> GetTopAsync(Expression<Func<AuditTask, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(AuditTask auditTask)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(AuditTask auditTask)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditTask>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditTask>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditTask, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditTask>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditTask, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditTask>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditTask, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(AuditTask auditTask, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(AuditTask auditTask, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}

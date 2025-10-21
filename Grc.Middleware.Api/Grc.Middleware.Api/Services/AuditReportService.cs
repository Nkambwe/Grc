using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public class AuditReportService : BaseService, IAuditReportService
    {
        public AuditReportService(IServiceLoggerFactory loggerFactory, 
                                  IUnitOfWorkFactory uowFactory, 
                                  IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public Task<bool> BulkyInsertAsync(AuditReport[] auditReports)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(AuditReport[] auditReports)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(AuditReport[] auditReports, params Expression<Func<AuditReport, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<AuditReport, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<AuditReport, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<AuditReport, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AuditReport auditReport, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<AuditReport> auditReports, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(AuditReport auditReport, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<AuditReport, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<AuditReport, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditReport, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public AuditReport Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public AuditReport Get(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public AuditReport Get(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<AuditReport> GetAll(bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<AuditReport> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<AuditReport> GetAll(Expression<Func<AuditReport, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditReport>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditReport>> GetAllAsync(Expression<Func<AuditReport, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditReport>> GetAllAsync(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditReport>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<AuditReport> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<AuditReport> GetAsync(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<AuditReport> GetAsync(Expression<Func<AuditReport, bool>> where, bool includeDeleted = false, params Expression<Func<AuditReport, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuditReport>> GetTopAsync(Expression<Func<AuditReport, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(AuditReport auditReport)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(AuditReport auditReport)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditReport>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditReport, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditReport>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditReport, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditReport>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditReport, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditReport>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditReport, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Update(AuditReport auditReport, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(AuditReport auditReport, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}

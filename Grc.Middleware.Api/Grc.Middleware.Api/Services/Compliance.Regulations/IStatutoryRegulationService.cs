using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public interface IStatutoryRegulationService : IBaseService
    {
        int Count();
        int Count(Expression<Func<StatutoryRegulation, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<StatutoryRegulation, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<StatutoryRegulation, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<StatutoryRegulation, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<StatutoryRegulation, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<StatutoryRegulation, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        StatutoryRegulation Get(long id, bool includeDeleted = false);
        Task<StatutoryRegulation> GetAsync(long id, bool includeDeleted = false);
        StatutoryRegulation Get(Expression<Func<StatutoryRegulation, bool>> where, bool includeDeleted = false);
        StatutoryRegulation Get(Expression<Func<StatutoryRegulation, bool>> where, bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes);
        Task<StatutoryRegulation> GetAsync(Expression<Func<StatutoryRegulation, bool>> where, bool includeDeleted = false);
        Task<StatutoryRegulation> GetAsync(Expression<Func<StatutoryRegulation, bool>> where, bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes);
        IQueryable<StatutoryRegulation> GetAll(bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes);
        IList<StatutoryRegulation> GetAll(bool includeDeleted = false);
        Task<IList<StatutoryRegulation>> GetAllAsync(bool includeDeleted = false);
        IList<StatutoryRegulation> GetAll(Expression<Func<StatutoryRegulation, bool>> where, bool includeDeleted);
        Task<IList<StatutoryRegulation>> GetAllAsync(Expression<Func<StatutoryRegulation, bool>> where, bool includeDeleted);
        Task<IList<StatutoryRegulation>> GetAllAsync(Expression<Func<StatutoryRegulation, bool>> where, bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes);
        Task<IList<StatutoryRegulation>> GetAllAsync(bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes);
        Task<IList<StatutoryRegulation>> GetTopAsync(Expression<Func<StatutoryRegulation, bool>> where, int top, bool includeDeleted = false);
        bool Insert(StatutoryRegulationRequest auditTask);
        Task<bool> InsertAsync(StatutoryRegulationRequest auditTask);
        bool Update(StatutoryRegulationRequest auditTask, bool includeDeleted = false);
        Task<bool> UpdateAsync(StatutoryRegulationRequest auditTask, bool includeDeleted = false);
        bool Delete(IdRequest idRequest);
        Task<bool> DeleteAsync(IdRequest idRequest);
        Task<bool> DeleteAllAsync(IList<long> requestItems, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(StatutoryRegulationRequest[] auditTasks);
        Task<bool> BulkyUpdateAsync(StatutoryRegulationRequest[] auditTasks);
        Task<bool> BulkyUpdateAsync(StatutoryRegulationRequest[] auditTasks, params Expression<Func<StatutoryRegulation, object>>[] propertySelectors);
        Task<PagedResult<StatutoryRegulation>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<StatutoryRegulation, bool>> predicate = null, params Expression<Func<StatutoryRegulation, object>>[] includes);
        Task<PagedResult<StatutoryRegulation>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<StatutoryRegulation, object>>[] includes);
        Task<PagedResult<StatutoryRegulation>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<StatutoryRegulation, bool>> where = null);
        Task<PagedResult<StatutoryRegulation>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<StatutoryRegulation, bool>> where = null, bool includeDeleted = false);
        Task<PolicySupportResponse> GetSupportItemsAsync(bool includeDeleted);
        Task<StatuteSupportResponse> GetStatuteSupportItemsAsync(bool includeDeleted);
    }
}

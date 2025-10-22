using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public interface IRegulatoryReturnService : IBaseService
    {
        int Count();
        int Count(Expression<Func<RegulatoryReturn, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryReturn, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryReturn, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<RegulatoryReturn, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<RegulatoryReturn, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<RegulatoryReturn, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        RegulatoryReturn Get(long id, bool includeDeleted = false);
        Task<RegulatoryReturn> GetAsync(long id, bool includeDeleted = false);
        RegulatoryReturn Get(Expression<Func<RegulatoryReturn, bool>> where, bool includeDeleted = false);
        RegulatoryReturn Get(Expression<Func<RegulatoryReturn, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryReturn, object>>[] includes);
        Task<RegulatoryReturn> GetAsync(Expression<Func<RegulatoryReturn, bool>> where, bool includeDeleted = false);
        Task<RegulatoryReturn> GetAsync(Expression<Func<RegulatoryReturn, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryReturn, object>>[] includes);
        IQueryable<RegulatoryReturn> GetAll(bool includeDeleted = false, params Expression<Func<RegulatoryReturn, object>>[] includes);
        IList<RegulatoryReturn> GetAll(bool includeDeleted = false);
        Task<IList<RegulatoryReturn>> GetAllAsync(bool includeDeleted = false);
        IList<RegulatoryReturn> GetAll(Expression<Func<RegulatoryReturn, bool>> where, bool includeDeleted);
        Task<IList<RegulatoryReturn>> GetAllAsync(Expression<Func<RegulatoryReturn, bool>> where, bool includeDeleted);
        Task<IList<RegulatoryReturn>> GetAllAsync(Expression<Func<RegulatoryReturn, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryReturn, object>>[] includes);
        Task<IList<RegulatoryReturn>> GetAllAsync(bool includeDeleted = false, params Expression<Func<RegulatoryReturn, object>>[] includes);
        Task<IList<RegulatoryReturn>> GetTopAsync(Expression<Func<RegulatoryReturn, bool>> where, int top, bool includeDeleted = false);
        bool Insert(RegulatoryReturnRequest submission);
        Task<bool> InsertAsync(RegulatoryReturnRequest submission);
        bool Update(RegulatoryReturnRequest submission, bool includeDeleted = false);
        Task<bool> UpdateAsync(RegulatoryReturnRequest submission, bool includeDeleted = false);
        bool Delete(IdRequest request, bool markAsDeleted = false);
        Task<bool> DeleteAsync(IdRequest request, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(RegulatoryReturnRequest[] request);
        Task<bool> BulkyUpdateAsync(RegulatoryReturnRequest[] request);
        Task<bool> BulkyUpdateAsync(RegulatoryReturnRequest[] request, params Expression<Func<RegulatoryReturn, object>>[] propertySelectors);
        Task<PagedResult<RegulatoryReturn>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<RegulatoryReturn, object>>[] includes);
        Task<PagedResult<RegulatoryReturn>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<RegulatoryReturn, object>>[] includes);
        Task<PagedResult<RegulatoryReturn>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<RegulatoryReturn, bool>> where = null);
        Task<PagedResult<RegulatoryReturn>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<RegulatoryReturn, bool>> where = null, bool includeDeleted = false);
    }
}

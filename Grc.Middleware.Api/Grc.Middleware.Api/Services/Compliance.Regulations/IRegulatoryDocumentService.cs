using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations
{
    public interface IRegulatoryDocumentService
    {
        int Count();
        int Count(Expression<Func<RegulatoryDocument, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryDocument, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryDocument, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<RegulatoryDocument, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<RegulatoryDocument, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<RegulatoryDocument, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        RegulatoryDocument Get(long id, bool includeDeleted = false);
        Task<RegulatoryDocument> GetAsync(long id, bool includeDeleted = false);
        RegulatoryDocument Get(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false);
        RegulatoryDocument Get(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes);
        Task<RegulatoryDocument> GetAsync(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false);
        Task<RegulatoryDocument> GetAsync(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes);
        IQueryable<RegulatoryDocument> GetAll(bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes);
        IList<RegulatoryDocument> GetAll(bool includeDeleted = false);
        Task<IList<RegulatoryDocument>> GetAllAsync(bool includeDeleted = false);
        IList<RegulatoryDocument> GetAll(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted);
        Task<IList<RegulatoryDocument>> GetAllAsync(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted);
        Task<IList<RegulatoryDocument>> GetAllAsync(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes);
        Task<IList<RegulatoryDocument>> GetAllAsync(bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes);
        Task<IList<RegulatoryDocument>> GetTopAsync(Expression<Func<RegulatoryDocument, bool>> where, int top, bool includeDeleted = false);
        bool Insert(RegulatoryDocumentRequest document);
        Task<bool> InsertAsync(RegulatoryDocumentRequest document);
        bool Update(RegulatoryDocumentRequest document, bool includeDeleted = false);
        Task<bool> UpdateAsync(RegulatoryDocumentRequest document, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(RegulatoryDocumentRequest[] request);
        Task<bool> BulkyUpdateAsync(RegulatoryDocumentRequest[] request);
        Task<bool> BulkyUpdateAsync(RegulatoryDocumentRequest[] request, params Expression<Func<RegulatoryDocument, object>>[] propertySelectors);
        Task<PagedResult<RegulatoryDocument>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<RegulatoryDocument, object>>[] includes);
        Task<PagedResult<RegulatoryDocument>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<RegulatoryDocument, object>>[] includes);
        Task<PagedResult<RegulatoryDocument>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<RegulatoryDocument, bool>> where = null);
        Task<PagedResult<RegulatoryDocument>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<RegulatoryDocument, bool>> where = null, bool includeDeleted = false);
    }
}

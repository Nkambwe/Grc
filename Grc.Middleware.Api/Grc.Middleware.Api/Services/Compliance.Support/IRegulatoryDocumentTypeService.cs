using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Support
{
    public interface IRegulatoryDocumentTypeService: IBaseService
    {
        int Count();
        int Count(Expression<Func<RegulatoryDocumentType, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryDocumentType, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<RegulatoryDocumentType, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        RegulatoryDocumentType Get(long id, bool includeDeleted = false);
        Task<RegulatoryDocumentType> GetAsync(long id, bool includeDeleted = false);
        RegulatoryDocumentType Get(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool includeDeleted = false);
        RegulatoryDocumentType Get(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryDocumentType, object>>[] includes);
        Task<RegulatoryDocumentType> GetAsync(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool includeDeleted = false);
        Task<RegulatoryDocumentType> GetAsync(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryDocumentType, object>>[] includes);
        IQueryable<RegulatoryDocumentType> GetAll(bool includeDeleted = false, params Expression<Func<RegulatoryDocumentType, object>>[] includes);
        IList<RegulatoryDocumentType> GetAll(bool includeDeleted = false);
        Task<IList<RegulatoryDocumentType>> GetAllAsync(bool includeDeleted = false);
        IList<RegulatoryDocumentType> GetAll(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool includeDeleted);
        Task<IList<RegulatoryDocumentType>> GetAllAsync(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool includeDeleted);
        Task<IList<RegulatoryDocumentType>> GetAllAsync(Expression<Func<RegulatoryDocumentType, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryDocumentType, object>>[] includes);
        Task<IList<RegulatoryDocumentType>> GetAllAsync(bool includeDeleted = false, params Expression<Func<RegulatoryDocumentType, object>>[] includes);
        Task<IList<RegulatoryDocumentType>> GetTopAsync(Expression<Func<RegulatoryDocumentType, bool>> predicate, int top, bool includeDeleted = false);
        bool Insert(DocumentTypeRequest request);
        Task<bool> InsertAsync(DocumentTypeRequest request);
        bool Update(DocumentTypeRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(DocumentTypeRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(DocumentTypeRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(DocumentTypeRequest[] requestItems);
        Task<bool> BulkyUpdateAsync(DocumentTypeRequest[] requestItems, params Expression<Func<RegulatoryDocumentType, object>>[] propertySelectors);
        Task<PagedResult<RegulatoryDocumentType>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<RegulatoryDocumentType, object>>[] includes);
        Task<PagedResult<RegulatoryDocumentType>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<RegulatoryDocumentType, object>>[] includes);
        Task<PagedResult<RegulatoryDocumentType>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<RegulatoryDocumentType, bool>> predicate = null);
        Task<PagedResult<RegulatoryDocumentType>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<RegulatoryDocumentType, bool>> predicate = null, bool includeDeleted = false);
    }
}

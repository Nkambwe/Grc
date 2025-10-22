using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations
{
    public class RegulatoryDocumentService : BaseService, IRegulatoryDocumentService
    {
        public RegulatoryDocumentService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper)
            : base(loggerFactory, uowFactory, mapper)
        {
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<RegulatoryDocument, bool>> predicate)
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

        public Task<int> CountAsync(Expression<Func<RegulatoryDocument, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<RegulatoryDocument, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<RegulatoryDocument, bool>> where, bool excludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<RegulatoryDocument, bool>> where, bool excludeDeleted = false, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<RegulatoryDocument, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public RegulatoryDocument Get(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public RegulatoryDocument Get(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public RegulatoryDocument Get(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<RegulatoryDocument> GetAll(bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<RegulatoryDocument> GetAll(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IList<RegulatoryDocument> GetAll(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<RegulatoryDocument>> GetAllAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<RegulatoryDocument>> GetAllAsync(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<IList<RegulatoryDocument>> GetAllAsync(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<RegulatoryDocument>> GetAllAsync(bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<RegulatoryDocument> GetAsync(long id, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<RegulatoryDocument> GetAsync(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<RegulatoryDocument> GetAsync(Expression<Func<RegulatoryDocument, bool>> where, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<IList<RegulatoryDocument>> GetTopAsync(Expression<Func<RegulatoryDocument, bool>> where, int top, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Insert(RegulatoryDocumentRequest document)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(RegulatoryDocumentRequest document)
        {
            throw new NotImplementedException();
        }

        public bool Update(RegulatoryDocumentRequest document, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(RegulatoryDocumentRequest document, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Delete(IdRequest request, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(IdRequest request, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyInsertAsync(RegulatoryDocumentRequest[] request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(RegulatoryDocumentRequest[] request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(RegulatoryDocumentRequest[] request, params Expression<Func<RegulatoryDocument, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<RegulatoryDocument>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<RegulatoryDocument>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<RegulatoryDocument>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<RegulatoryDocument, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<RegulatoryDocument>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<RegulatoryDocument, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

    }
}

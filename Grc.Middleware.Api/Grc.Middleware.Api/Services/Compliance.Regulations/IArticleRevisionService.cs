using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public interface IArticleRevisionService : IBaseService {
        int Count();
        int Count(Expression<Func<ArticleRevision, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ArticleRevision, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<ArticleRevision, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ArticleRevision, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ArticleRevision, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ArticleRevision, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        ArticleRevision Get(long id, bool includeDeleted = false);
        Task<ArticleRevision> GetAsync(long id, bool includeDeleted = false);
        ArticleRevision Get(Expression<Func<ArticleRevision, bool>> where, bool includeDeleted = false);
        ArticleRevision Get(Expression<Func<ArticleRevision, bool>> where, bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes);
        Task<ArticleRevision> GetAsync(Expression<Func<ArticleRevision, bool>> where, bool includeDeleted = false);
        Task<ArticleRevision> GetAsync(Expression<Func<ArticleRevision, bool>> where, bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes);
        IQueryable<ArticleRevision> GetAll(bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes);
        IList<ArticleRevision> GetAll(bool includeDeleted = false);
        Task<IList<ArticleRevision>> GetAllAsync(bool includeDeleted = false);
        IList<ArticleRevision> GetAll(Expression<Func<ArticleRevision, bool>> where, bool includeDeleted);
        Task<IList<ArticleRevision>> GetAllAsync(Expression<Func<ArticleRevision, bool>> where, bool includeDeleted);
        Task<IList<ArticleRevision>> GetAllAsync(Expression<Func<ArticleRevision, bool>> where, bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes);
        Task<IList<ArticleRevision>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes);
        Task<IList<ArticleRevision>> GetTopAsync(Expression<Func<ArticleRevision, bool>> where, int top, bool includeDeleted = false);
        bool Insert(ArticleRevisionRequest article);
        Task<bool> InsertAsync(ArticleRevisionRequest article);
        bool Update(ArticleRevisionRequest article, bool includeDeleted = false);
        Task<bool> UpdateAsync(ArticleRevisionRequest article, bool includeDeleted = false);
        bool Delete(IdRequest idRequest);
        Task<bool> DeleteAsync(IdRequest idRequest);
        Task<bool> DeleteAllAsync(IList<long> requestItems, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(ArticleRevisionRequest[] article);
        Task<bool> BulkyUpdateAsync(ArticleRevisionRequest[] article);
        Task<bool> BulkyUpdateAsync(ArticleRevisionRequest[] article, params Expression<Func<ArticleRevision, object>>[] propertySelectors);
        Task<PagedResult<ArticleRevision>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ArticleRevision, object>>[] includes);
        Task<PagedResult<ArticleRevision>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ArticleRevision, object>>[] includes);
        Task<PagedResult<ArticleRevision>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ArticleRevision, bool>> where = null);
        Task<PagedResult<ArticleRevision>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ArticleRevision, bool>> where = null, bool includeDeleted = false);
    }
}

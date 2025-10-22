
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public interface IStatutoryArticleService : IBaseService
    {
        int Count();
        int Count(Expression<Func<StatutoryArticle, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<StatutoryArticle, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<StatutoryArticle, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<StatutoryArticle, bool>> where, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<StatutoryArticle, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<StatutoryArticle, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        StatutoryArticle Get(long id, bool includeDeleted = false);
        Task<StatutoryArticle> GetAsync(long id, bool includeDeleted = false);
        StatutoryArticle Get(Expression<Func<StatutoryArticle, bool>> where, bool includeDeleted = false);
        StatutoryArticle Get(Expression<Func<StatutoryArticle, bool>> where, bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes);
        Task<StatutoryArticle> GetAsync(Expression<Func<StatutoryArticle, bool>> where, bool includeDeleted = false);
        Task<StatutoryArticle> GetAsync(Expression<Func<StatutoryArticle, bool>> where, bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes);
        IQueryable<StatutoryArticle> GetAll(bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes);
        IList<StatutoryArticle> GetAll(bool includeDeleted = false);
        Task<IList<StatutoryArticle>> GetAllAsync(bool includeDeleted = false);
        IList<StatutoryArticle> GetAll(Expression<Func<StatutoryArticle, bool>> where, bool includeDeleted);
        Task<IList<StatutoryArticle>> GetAllAsync(Expression<Func<StatutoryArticle, bool>> where, bool includeDeleted);
        Task<IList<StatutoryArticle>> GetAllAsync(Expression<Func<StatutoryArticle, bool>> where, bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes);
        Task<IList<StatutoryArticle>> GetAllAsync(bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes);
        Task<IList<StatutoryArticle>> GetTopAsync(Expression<Func<StatutoryArticle, bool>> where, int top, bool includeDeleted = false);
        bool Insert(StatutoryArticleRequest article);
        Task<bool> InsertAsync(StatutoryArticleRequest article);
        bool Update(StatutoryArticleRequest article, bool includeDeleted = false);
        Task<bool> UpdateAsync(StatutoryArticleRequest article, bool includeDeleted = false);
        bool Delete(IdRequest idRequest, bool markAsDeleted = false);
        Task<bool> DeleteAsync(IdRequest idRequest, bool markAsDeleted = false);
        Task<bool> DeleteAllAsync(IList<long> requestItems, bool markAsDeleted = false);
        Task<bool> BulkyInsertAsync(StatutoryArticleRequest[] article);
        Task<bool> BulkyUpdateAsync(StatutoryArticleRequest[] article);
        Task<bool> BulkyUpdateAsync(StatutoryArticleRequest[] article, params Expression<Func<StatutoryArticle, object>>[] propertySelectors);
        Task<PagedResult<StatutoryArticle>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<StatutoryArticle, object>>[] includes);
        Task<PagedResult<StatutoryArticle>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<StatutoryArticle, object>>[] includes);
        Task<PagedResult<StatutoryArticle>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<StatutoryArticle, bool>> where = null);
        Task<PagedResult<StatutoryArticle>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<StatutoryArticle, bool>> where = null, bool includeDeleted = false);
    }
}

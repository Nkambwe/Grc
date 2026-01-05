using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public interface IControlItemService {
        int Count();
        int Count(Expression<Func<ControlItem, bool>> predicate);
        Task<int> CountAsync(Expression<Func<ControlItem, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ControlItem, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ControlItem, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<ControlItem> GetAsync(long id, bool includeDeleted = false);
        Task<ControlItem> GetAsync(Expression<Func<ControlItem, bool>> predicate, bool includeDeleted = false);
        Task<ControlItem> GetAsync(Expression<Func<ControlItem, bool>> predicate, bool includeDeleted = false, params Expression<Func<ControlItem, object>>[] includes);
        Task<IList<ControlItem>> GetAllAsync(Expression<Func<ControlItem, bool>> predicate, bool includeDeleted);
        Task<IList<ControlItem>> GetAllAsync(Expression<Func<ControlItem, bool>> predicate, bool includeDeleted = false, params Expression<Func<ControlItem, object>>[] includes);
        bool Insert(ControlItemRequest request);
        Task<bool> InsertAsync(ControlItemRequest request);
        bool Update(ControlItemRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ControlItemRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<PagedResult<ControlItemResponse>> PageLookupAsync<ControlItemResponse>(int page, int size, bool includeDeleted, Expression<Func<ControlItem, ControlItemResponse>> selector);
        Task<PagedResult<ControlItem>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ControlItem, object>>[] includes);
        Task<PagedResult<ControlItem>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ControlItem, bool>> predicate = null);
    }
}

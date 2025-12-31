using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public interface IControlCategoryService {
        int Count();
        int Count(Expression<Func<ControlCategory, bool>> predicate);
        Task<int> CountAsync(Expression<Func<ControlCategory, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);
        bool Exists(Expression<Func<ControlCategory, bool>> predicate, bool excludeDeleted = false);
        Task<bool> ExistsAsync(Expression<Func<ControlCategory, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<ControlCategoryResponse> GetAsync(long id, bool includeDeleted = false);
        Task<ControlCategoryResponse> GetAsync(Expression<Func<ControlCategory, bool>> predicate, bool includeDeleted = false);
        Task<ControlCategoryResponse> GetAsync(Expression<Func<ControlCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<ControlCategory, object>>[] includes);
        Task<IList<ControlCategory>> GetAllAsync(Expression<Func<ControlCategory, bool>> predicate, bool includeDeleted);
        Task<IList<ControlCategory>> GetAllAsync(Expression<Func<ControlCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<ControlCategory, object>>[] includes);
        bool Insert(ControlCategoryRequest request);
        Task<bool> InsertAsync(ControlCategoryRequest request);
        bool Update(ControlCategoryRequest request, bool includeDeleted = false);
        Task<bool> UpdateAsync(ControlCategoryRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<PagedResult<ControlCategoryResponse>> PageLookupAsync<ControlCategoryResponse>(int page, int size, bool includeDeleted, Expression<Func<ControlCategory, ControlCategoryResponse>> selector);
        Task<PagedResult<ControlCategory>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ControlCategory, object>>[] includes);
        Task<PagedResult<ControlCategory>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ControlCategory, bool>> predicate = null);
        Task<ControlSupportResponse> GetSupportItemsAsync(bool includeDeleted);
        Task<bool> GenerateMapAsync(ComplianceItemMapRequest request);
    }

}

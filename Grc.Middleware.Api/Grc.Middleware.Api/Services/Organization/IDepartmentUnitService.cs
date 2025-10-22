using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services.Organization {
    public interface IDepartmentUnitService: IBaseService {
        Task<DepartmentUnit> GetUnitByIdAsync(long id, bool includeDeleted=false);
        Task<DepartmentUnit> GetUnitByNameAsync(string name, bool includeDeleted=false);
        Task<DepartmentUnit> GetUnitByCodeAsync(string code, bool includeDeleted=false);
        Task<IList<DepartmentUnit>> GetAllAsync(bool includeDeleted = false);
        Task<PagedResult<DepartmentUnit>> GetPagedUnitsAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, int pageIndex = 1, int pageSize = 20, bool includeDeleted=false);
        Task<bool> InsertUnitAsync(DepartmentUnitRequest request);
        Task<bool> UpdateUnitAsync(DepartmentUnitRequest request);
        Task<bool> DeleteUnitAsync(IdRequest request);
        Task<bool> ExistsByIdAsync(long id);
        Task<bool> ExistsAsync(DepartmentUnitRequest request);
        
    }

}

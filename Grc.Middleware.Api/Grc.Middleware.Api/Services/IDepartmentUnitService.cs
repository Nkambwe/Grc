using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Helpers;

namespace Grc.Middleware.Api.Services {
    public interface IDepartmentUnitService: IBaseService {
        Task<DepartmentUnit> GetUnitByIdAsync(long id, bool includeDeleted=false);
        Task<DepartmentUnit> GetUnitByNameAsync(string name, bool includeDeleted=false);
        Task<DepartmentUnit> GetUnitByCodeAsync(string code, bool includeDeleted=false);
        Task<PagedResult<DepartmentUnit>> GetPagedUnitsAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, int pageIndex = 1, int pageSize = 20, bool includeDeleted=false);
        Task<bool> InsertUnitAsync(DepartmentUnit department);
        Task<bool> DeleteUnitAsync(DepartmentUnit department, bool includeDeleted);
    }

}

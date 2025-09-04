using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Helpers;

namespace Grc.Middleware.Api.Services {
    public interface IDepartmentsService: IBaseService {
        Task<Department> GetDepartmentByIdAsync(long id, bool includeDeleted=false);
        Task<Department> GetDepartmentByNameAsync(string name, bool includeDeleted=false);
        Task<Department> GetDepartmentByCodeAsync(string code, bool includeDeleted=false);
        Task<PagedResult<Department>> GetPagedDepartmentsAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, int pageIndex = 1, int pageSize = 20, bool includeDeleted=false);
        Task<bool> InsertDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(Department department, bool includeDeleted);
    }

}

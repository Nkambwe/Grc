using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services {
    public interface IDepartmentsService: IBaseService {
        Task<Department> GetDepartmentByIdAsync(long id, bool includeDeleted=false);
        Task<Department> GetDepartmentByNameAsync(string name, bool includeDeleted=false);
        Task<Department> GetDepartmentByCodeAsync(string code, bool includeDeleted=false);
        Task<IList<Department>> GetAllAsync(bool includeDeleted=false);
        Task<PagedResult<Department>> GetPagedDepartmentsAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, int pageIndex = 1, int pageSize = 20, bool includeDeleted=false);
        Task<bool> InsertDepartmentAsync(DepartmentRequest request);
        Task<bool> UpdateDepartmentAsync(DepartmentRequest request);
        Task<bool> ExistsByIdAsync(long id);
        Task<bool> ExistsAsync(DepartmentRequest request); 
        Task<bool> DeleteDepartmentAsync(IdRequst request);
        
    }

}

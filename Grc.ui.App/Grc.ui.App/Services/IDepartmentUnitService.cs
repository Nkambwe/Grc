using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IDepartmentUnitService : IGrcBaseService {
        Task<GrcResponse<DepartmentUnitModel>> GetUnitById(GrcIdRequst request);
        Task<GrcResponse<List<DepartmentUnitModel>>> GetUnitsAsync(GrcRequest request);
        Task<GrcResponse<PagedResponse<DepartmentUnitModel>>> GetDepartmentUnitsAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> InsertDepartmentUnitAsync(GrcInsertRequest<DepartmentUnitRequest> data);
        Task<GrcResponse<ServiceResponse>> UpdateDepartmentUnitAsync(GrcInsertRequest<DepartmentUnitRequest> data);
        Task<GrcResponse<ServiceResponse>> DeleteDepartmentUnitAsync(long id, long userId, string ipAddress = null);
        
    }
}

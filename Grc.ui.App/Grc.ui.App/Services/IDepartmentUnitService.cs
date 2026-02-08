using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IDepartmentUnitService : IGrcBaseService {
        Task<GrcResponse<GrcDepartmentUnitResponse>> GetUnitById(GrcIdRequest request);
        Task<GrcResponse<List<GrcDepartmentUnitResponse>>> GetUnitsAsync(GrcRequest request);
        Task<GrcResponse<PagedResponse<GrcDepartmentUnitResponse>>> GetDepartmentUnitsAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> InsertDepartmentUnitAsync(DepartmentFullUnitModel data, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateDepartmentUnitAsync(DepartmentFullUnitModel data, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteDepartmentUnitAsync(long id, long userId, string ipAddress = null);
        
    }
}

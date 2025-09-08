using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IDepartmentUnitService : IGrcBaseService {
        Task<GrcResponse<PagedResponse<DepartmentUnitModel>>> GetDepartmentUnitsAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> InsertDepartmentUnitAsync(DepartmentUnitModel model, long userId, string ipAddress = null);
        Task<GrcResponse<ServiceResponse>> UpdateDepartmentUnitAsync(DepartmentUnitModel model, long userId, string ipAddress = null);
        Task<GrcResponse<ServiceResponse>> DeleteDepartmentUnitAsync(long id, long userId, string ipAddress = null);
    }
}

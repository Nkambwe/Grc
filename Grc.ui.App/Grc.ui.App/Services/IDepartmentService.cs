using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IDepartmentService : IGrcBaseService {
        Task<GrcResponse<PagedResponse<DepartmentModel>>> GetDepartmentsAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> InsertDepartmentAsync(DepartmentModel model, long userId, string ipAddress = null);
        Task<GrcResponse<ServiceResponse>> UpdateDepartmentAsync(DepartmentModel model, long userId, string ipAddress = null);
        Task<GrcResponse<ServiceResponse>> DeleteDepartmentAsync(long id, long userId, string ipAddress = null);
    }
}

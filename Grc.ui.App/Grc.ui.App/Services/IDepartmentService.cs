using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IDepartmentService : IGrcBaseService {
        Task<GrcResponse<DepartmentModel>> GetDepartmentById(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<DepartmentModel>>> GetAllDepartmentsAsync(TableListRequest request);
        Task<GrcResponse<List<DepartmentModel>>> GetDepartmentsAsync(GrcRequest request);
        Task<GrcResponse<ServiceResponse>> InsertDepartmentAsync(DepartmentModel model, long userId, string ipAddress = null);
        Task<GrcResponse<ServiceResponse>> UpdateDepartmentAsync(DepartmentModel model, long userId, string ipAddress = null);
        Task<GrcResponse<ServiceResponse>> DeleteDepartmentAsync(long id, long userId, string ipAddress = null);
    }
}

using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IDepartmentService : IGrcBaseService {
        Task<GrcResponse<GrcDepartmentRecordResponse>> GetDepartmentById(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcDepartmentRecordResponse>>> GetAllDepartmentsAsync(TableListRequest request);
        Task<GrcResponse<List<GrcDepartmentRecordResponse>>> GetDepartmentsAsync(GrcRequest request);
        Task<GrcResponse<ServiceResponse>> InsertDepartmentAsync(DepartmentFullModel model, long userId, string ipAddress = null);
        Task<GrcResponse<ServiceResponse>> UpdateDepartmentAsync(DepartmentFullModel model, long userId, string ipAddress = null);
        Task<GrcResponse<ServiceResponse>> DeleteDepartmentAsync(long id, long userId, string ipAddress = null);
    }
}

using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IResponsibilityService : IGrcBaseService {
        Task<GrcResponse<OwnerResponse>> GetOwnerAsync(GrcIdRequest getRequest);
        Task<GrcResponse<List<OwnerResponse>>> GetAllAsync(GrcRequest getRequest);
        Task<GrcResponse<PagedResponse<OwnerResponse>>> GetAllOwners(TableListRequest request);
        Task<GrcResponse<OwnerResponse>> CreateOwnerAsync(OwnerViewModel request);
        Task<GrcResponse<OwnerResponse>> UpdateOwnerAsync(OwnerViewModel request);
        Task<GrcResponse<ServiceResponse>> DeleteOwnerAsync(GrcIdRequest deleteRequest);
    }
}

using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IRegulatonAuthorityService : IGrcBaseService
    {
        Task<GrcResponse<GrcRegulatoryAuthorityResponse>> CreateAuthorityAsync(RegulatoryAuthorityRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteAuthorityAsync(GrcIdRequest deleteRequest);
        Task<GrcResponse<PagedResponse<GrcRegulatoryAuthorityResponse>>> GetAllRegulatoryAuthorities(TableListRequest request);
        Task<GrcResponse<GrcRegulatoryAuthorityResponse>> GetAuthorityAsync(GrcIdRequest getRequest);
        Task<GrcResponse<GrcRegulatoryAuthorityResponse>> UpdateAuthorityAsync(RegulatoryAuthorityRequest request);
    }
}

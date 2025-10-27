using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IRegulatonAuthorityService : IGrcBaseService
    {
        Task<GrcResponse<RegulatoryAuthorityResponse>> CreateAuthorityAsync(RegulatoryAuthorityRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteAuthorityAsync(GrcIdRequest deleteRequest);
        Task<GrcResponse<PagedResponse<RegulatoryAuthorityResponse>>> GetAllRegulatoryAuthorities(TableListRequest request);
        Task<GrcResponse<RegulatoryAuthorityResponse>> GetAuthorityAsync(GrcIdRequest getRequest);
        Task<GrcResponse<RegulatoryAuthorityResponse>> UpdateAuthorityAsync(RegulatoryAuthorityRequest request);
    }
}

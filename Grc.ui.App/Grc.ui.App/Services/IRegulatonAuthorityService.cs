using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IRegulatonAuthorityService : IGrcBaseService
    {
        Task<GrcResponse<RegulatoryAuthorityResponse>> CreateAuthorityAsync(RegulatoryAuthorityRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteAuthorityAsync(GrcIdRequst deleteRequest);
        Task<GrcResponse<PagedResponse<RegulatoryAuthorityResponse>>> GetAllRegulatoryAuthorities(TableListRequest request);
        Task<GrcResponse<RegulatoryAuthorityResponse>> GetAuthorityAsync(GrcIdRequst getRequest);
        Task<GrcResponse<RegulatoryAuthorityResponse>> UpdateAuthorityAsync(RegulatoryAuthorityRequest request);
    }
}

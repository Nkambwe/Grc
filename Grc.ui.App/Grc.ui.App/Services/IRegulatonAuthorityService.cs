using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public interface IRegulatonAuthorityService : IGrcBaseService {
        Task<GrcResponse<PagedResponse<GrcRegulatoryAuthorityResponse>>> GetPagedAuthoritiesAsync(TableListRequest request);
        Task<GrcResponse<GrcRegulatoryAuthorityResponse>> GetAuthorityAsync(GrcIdRequest getRequest);
        Task<GrcResponse<ServiceResponse>> CreateAuthorityAsync(RegulatoryAuthorityRequest request);
        Task<GrcResponse<ServiceResponse>> UpdateAuthorityAsync(RegulatoryAuthorityRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteAuthorityAsync(GrcIdRequest request);
    }

}

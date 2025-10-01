using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IRegulatonTypeService : IGrcBaseService
    {
        Task<GrcResponse<RegulatoryTypeResponse>> CreateTypeAsync(RegulatoryTypeRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequst deleteRequest);
        Task<GrcResponse<PagedResponse<RegulatoryTypeResponse>>> GetAllRegulatoryTypes(TableListRequest request);
        Task<GrcResponse<RegulatoryTypeResponse>> GetTypeAsync(GrcIdRequst getRequest);
        Task<GrcResponse<RegulatoryTypeResponse>> UpdateTypeAsync(RegulatoryTypeRequest request);
    }
}

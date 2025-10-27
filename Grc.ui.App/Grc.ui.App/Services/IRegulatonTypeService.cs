using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IRegulatonTypeService : IGrcBaseService {
        Task<GrcResponse<RegulatoryTypeResponse>> CreateTypeAsync(RegulatoryViewModel request);
        Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequest deleteRequest);
        Task<GrcResponse<PagedResponse<RegulatoryTypeResponse>>> GetAllRegulatoryTypes(TableListRequest request);
        Task<GrcResponse<RegulatoryTypeResponse>> GetTypeAsync(GrcIdRequest getRequest);
        Task<GrcResponse<RegulatoryTypeResponse>> UpdateTypeAsync(RegulatoryViewModel request);
    }
}

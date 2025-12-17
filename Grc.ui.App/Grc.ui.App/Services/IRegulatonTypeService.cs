using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IRegulatonTypeService : IGrcBaseService {
        Task<GrcResponse<GrcRegulatoryTypeResponse>> GetTypeAsync(GrcIdRequest getRequest);
        Task<GrcResponse<PagedResponse<GrcRegulatoryTypeResponse>>> GetPagedTypesAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> CreateTypeAsync(RegulatoryViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateTypeAsync(RegulatoryViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequest request);
    }

}

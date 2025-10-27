using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IRegulatoryActService : IGrcBaseService
    {
        Task<GrcResponse<RegulatoryActResponse>> CreateRegulatoryActAsync(RegulatoryActViewModel request);
        Task<GrcResponse<ServiceResponse>> DeleteRegulatoryActAsync(GrcIdRequest deleteRequest);
        Task<GrcResponse<List<RegulatoryActResponse>>> GetAllAsync(GrcRequest request);
        Task<GrcResponse<PagedResponse<RegulatoryActResponse>>> GetPagedRegulatoryActAsync(TableListRequest request);
        Task<GrcResponse<RegulatoryActResponse>> GetRegulatoryActAsyncAsync(GrcIdRequest getRequest);
        Task<GrcResponse<RegulatoryActResponse>> UpdateRegulatoryActAsync(RegulatoryActViewModel request);
    }
}

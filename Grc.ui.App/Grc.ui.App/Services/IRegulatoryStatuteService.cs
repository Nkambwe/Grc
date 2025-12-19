using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public interface IRegulatoryStatuteService {
        Task<GrcResponse<GrcStatutoryLawResponse>> GetStatuteAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcStatutoryLawResponse>>> GetCategoryStatutes(StatueListRequest request, long userId, string ipAddress);
        Task<GrcResponse<PagedResponse<GrcStatutoryLawResponse>>> GetPagedStatutesAsync(StatueListRequest request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> CreateStatuteAsync(GrcStatutoryLawRequest request);
        Task<GrcResponse<ServiceResponse>> UpdateStatuteAsync(GrcStatutoryLawRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteStatuteAsync(GrcIdRequest request);
    }
}

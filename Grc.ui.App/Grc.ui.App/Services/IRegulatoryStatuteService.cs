using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IRegulatoryStatuteService {
        Task<GrcResponse<GrcStatuteSupportResponse>> GetStatuteSupportItemsAsync(GrcRequest request);
        Task<GrcResponse<GrcAuditSupportResponse>> GetAuditSupportItemsAsync(GrcRequest request);
        Task<GrcResponse<GrcStatutoryLawResponse>> GetStatuteAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcStatutoryLawResponse>>> GetCategoryStatutes(StatueListRequest request, long userId, string ipAddress);
        Task<GrcResponse<PagedResponse<GrcStatutoryLawResponse>>> GetPagedStatutesAsync(StatueListRequest request, long userId, string ipAddress);
        Task<GrcResponse<PagedResponse<GrcObligationResponse>>> GetStatutoryObligations(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> CreateStatuteAsync(StatuteViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateStatuteAsync(StatuteViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteStatuteAsync(GrcIdRequest request);
        
    }
}

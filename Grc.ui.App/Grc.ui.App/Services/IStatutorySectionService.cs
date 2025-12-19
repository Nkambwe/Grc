using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IStatutorySectionService : IGrcBaseService {
        Task<GrcResponse<GrcStatutorySectionResponse>> GetSectionAsyncAsync(GrcIdRequest getRequest);
        Task<GrcResponse<PagedResponse<GrcStatutorySectionResponse>>> GetLawSectionsAsync(StatueListRequest request, long userId, string ipAddress);
        Task<GrcResponse<PagedResponse<GrcStatutorySectionResponse>>> GetPagedSectionsAsync(StatueListRequest request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> CreateSectionAsync(StatuteSectionViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateSectionAsync(StatuteSectionViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteSectionAsync(GrcIdRequest deleteRequest);
    }
}

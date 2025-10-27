using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IDocumentTypeService : IGrcBaseService
    {
        Task<GrcResponse<DocumentTypeResponse>> GetTypeAsync(GrcIdRequest getRequest);
        Task<GrcResponse<List<DocumentTypeResponse>>> GetAllAsync(GrcRequest getRequest);
        Task<GrcResponse<PagedResponse<DocumentTypeResponse>>> GetAllDocumentTypes(TableListRequest request);
        Task<GrcResponse<DocumentTypeResponse>> CreateTypeAsync(DocumentTypeViewModel request);
        Task<GrcResponse<DocumentTypeResponse>> UpdateTypeAsync(DocumentTypeViewModel request);
        Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequest deleteRequest);
        
        
    }
}

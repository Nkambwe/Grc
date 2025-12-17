using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IDocumentTypeService : IGrcBaseService
    {
        Task<GrcResponse<DocumentTypeResponse>> GetDocumentTypeAsync(GrcIdRequest request);
        Task<GrcResponse<List<DocumentTypeResponse>>> GetDocumentListAsync(GrcRequest request);
        Task<GrcResponse<PagedResponse<DocumentTypeResponse>>> GetPagedDocumentTypesAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> CreateTypeAsync(DocumentTypeViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateTypeAsync(DocumentTypeViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequest request);
        
        
    }
}

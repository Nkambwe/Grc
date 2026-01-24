using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IPolicyService : IGrcBaseService {
        Task<GrcResponse<PagedResponse<PolicyDocumentResponse>>> GetPagedDocumentsAsync(TableListRequest request);
        Task<GrcResponse<PolicyDocumentResponse>> GetPolicyDocumentAsync(GrcIdRequest getRequest);
        Task<GrcResponse<List<PolicyDocumentResponse>>> GetDocumentListAsync(GrcRequest request);
        Task<GrcResponse<GrcPolicySupportResponse>> GetPolicySupportItemsAsync(GrcRequest request);
        Task<GrcResponse<ServiceResponse>> CreateDocumentAsync(PolicyViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateDocumentAsync(PolicyViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> LockDocumentAsync(PolicyLockViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeletePolicyAsync(GrcIdRequest deleteRequest);
        Task<GrcResponse<ServiceResponse>> LockPolicyAsync(GrcIdRequest request);

    }

}

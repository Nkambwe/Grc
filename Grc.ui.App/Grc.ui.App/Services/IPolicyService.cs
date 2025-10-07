using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IPolicyService : IGrcBaseService
    {
        Task<GrcResponse<PolicyRegisterResponse>> CreatePolicyAsync(PolicyViewModel request);
        Task<GrcResponse<ServiceResponse>> DeletePolicyAsync(GrcIdRequst deleteRequest);
        Task<GrcResponse<List<PolicyRegisterResponse>>> GetAllAsync(GrcRequest request);
        Task<GrcResponse<PagedResponse<PolicyRegisterResponse>>> GetAllPolicies(TableListRequest request);
        Task<GrcResponse<PolicyRegisterResponse>> GetPolicyAsync(GrcIdRequst getRequest);
        Task<GrcResponse<PolicyRegisterResponse>> UpdatePolicyAsync(PolicyViewModel request);
    }
}

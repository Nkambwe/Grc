using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IPolicyTaskService : IGrcBaseService
    {
        Task<GrcResponse<PolicyTaskResponse>> CreatePolicyTaskAsync(PolicyTaskViewModel request);
        Task<GrcResponse<ServiceResponse>> DeletePolicyTaskAsync(GrcIdRequest deleteRequest);
        Task<GrcResponse<List<PolicyTaskResponse>>> GetAllAsync(GrcRequest request);
        Task<GrcResponse<PagedResponse<PolicyTaskResponse>>> GetAllPolicyTasks(TableListRequest request);
        Task<GrcResponse<PolicyTaskResponse>> GetPolicyTaskAsync(GrcIdRequest getRequest);
        Task<GrcResponse<PolicyTaskResponse>> UpdatePolicyTaskAsync(PolicyTaskViewModel request);
    }
}

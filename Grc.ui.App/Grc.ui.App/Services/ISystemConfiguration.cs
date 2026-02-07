using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public interface ISystemConfiguration {
        Task<GrcResponse<GrcConfigurationResponse>> GetConfigurationAsync(long userId, string iPAddress);
        Task<GrcResponse<GrcBooleanConfigurationResponse>> GetIncludeDeletedRecordAsync(GrcConfigurationParamRequest request);
        Task<GrcResponse<ServiceResponse>> SaveGeneralConfigurationsAsync(GrcGeneralConfigurationsRequest request);
        Task<GrcResponse<ServiceResponse>> SavePolicyConfigurationsAsync(GrcPolicyConfigurationsRequest request);
        Task<GrcResponse<ServiceResponse>> UpdateConfigurationAsync(GrcSystemConfigurationRequest request);
        Task<GrcResponse<GrcBugResponse>> GetErrorAsync(long id, long userId, string iPAddress);
        Task<GrcResponse<PagedResponse<GrcBugItemResponse>>> GetBugListAsync(BugListView request, long id, string ipAddress);
        Task<GrcResponse<ListResponse<GrcBugResponse>>> GetNewBugsAsync(GrcRequest request);
        Task<GrcResponse<PagedResponse<GrcBugResponse>>> GetStatusBugsAsync(GrcBugStatusListRequest request);
        Task<GrcResponse<ServiceResponse>> CreateErrorAsync(GrcBugRequest request);
        Task<GrcResponse<ServiceResponse>> ChangeErrorStausAsync(GrcBugStatusRequest request);
        Task<GrcResponse<ServiceResponse>> UpdateErrorAsync(GrcBugRequest request);
    }
}

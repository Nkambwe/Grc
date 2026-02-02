using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public interface ISystemConfiguration {

        Task<GrcResponse<GrcConfigurationResponse>> GetConfigurationAsync(long userId, string iPAddress);

        Task<GrcResponse<GrcBooleanConfigurationResponse>> GetIncludeDeletedRecordAsync(GrcConfigurationParamRequest request);
        Task<GrcResponse<ServiceResponse>> SaveGeneralConfigurationsAsync(GrcGeneralConfigurationsRequest request);
        Task<GrcResponse<ServiceResponse>> SavePolicyConfigurationsAsync(GrcPolicyConfigurationsRequest request);
        Task<GrcResponse<ServiceResponse>> UpdateConfigurationAsync(GrcSystemConfigurationRequest request);
    }
}

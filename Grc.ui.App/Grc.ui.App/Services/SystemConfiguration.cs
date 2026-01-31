using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {
    public class SystemConfiguration : GrcBaseService, ISystemConfiguration {
        public SystemConfiguration(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, 
            IMapper mapper, 
            IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) : 
            base(loggerFactory, httpHandler, environment, endpointType, 
                mapper, webHelper, sessionManager, errorFactory, errorService) {
        }

        public async Task<GrcResponse<GrcConfigurationResponse>> GetConfigurationAsync(long userId, string iPAddress) {
            try {
                var request = new GrcRequest() {
                    UserId = userId,
                    IPAddress = iPAddress,
                    Action = "Get application settings"
                };

                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/settings-all";
                return await HttpHandler.PostAsync<GrcRequest, GrcConfigurationResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcConfigurationResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcBooleanConfigurationResponse>> GetIncludeDeletedRecordAsync(GrcConfigurationParamRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/include-deleted";
                return await HttpHandler.PostAsync<GrcConfigurationParamRequest, GrcBooleanConfigurationResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcBooleanConfigurationResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateConfigurationAsync(GrcSystemConfigurationRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/settings-update";
                return await HttpHandler.PostAsync<GrcSystemConfigurationRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }
    }
}

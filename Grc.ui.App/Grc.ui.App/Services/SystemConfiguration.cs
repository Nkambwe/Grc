using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;
using System.Net;

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
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
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
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
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
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> SaveGeneralConfigurationsAsync(GrcGeneralConfigurationsRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/general-configurations";
                return await HttpHandler.PostAsync<GrcGeneralConfigurationsRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> SavePolicyConfigurationsAsync(GrcPolicyConfigurationsRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/policy-configurations";
                return await HttpHandler.PostAsync<GrcPolicyConfigurationsRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> SavePasswordPolicyConfigurationsAsync(GrcPasswordConfigurationsRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/password-policy-configuration";
                return await HttpHandler.PostAsync<GrcPasswordConfigurationsRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcBugResponse>> GetErrorAsync(long id, long userId, string iPAddress) {
            try {
                var request = new GrcIdRequest() {
                    RecordId = id,
                    UserId = userId,
                    IPAddress = iPAddress,
                    Action = "Get system error record"
                };

                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/bug-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcBugResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcBugResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcBugItemResponse>>> GetBugListAsync(BugListView model, long id, string ipAddress) {
            try {

                var request = new GrcBugListRequest() {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    SortBy = model.SortBy,
                    SortDirection = model.SortDirection,
                    Filters = model.Filters,
                    SearchTerm = model.SearchTerm,
                    UserId = id,
                    IPAddress = ipAddress,
                    Action = "Retrieve a list of system bugs"
                };

                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/bug-list";
                return await HttpHandler.PostAsync<GrcBugListRequest, PagedResponse<GrcBugItemResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcBugItemResponse>>(error);
            }
        }

        public async Task<GrcResponse<ListResponse<GrcBugResponse>>> GetNewBugsAsync(GrcRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/bug-export";
                return await HttpHandler.PostAsync<GrcRequest, ListResponse<GrcBugResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ListResponse<GrcBugResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcBugResponse>>> GetStatusBugsAsync(GrcBugStatusListRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/bug-status";
                return await HttpHandler.PostAsync<GrcBugStatusListRequest, PagedResponse<GrcBugResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcBugResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateErrorAsync(GrcBugRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/create-bug";
                return await HttpHandler.PostAsync<GrcBugRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> ChangeErrorStausAsync(GrcBugStatusRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/change-bug-status";
                return await HttpHandler.PostAsync<GrcBugStatusRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateErrorAsync(GrcBugRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/update-bug";
                return await HttpHandler.PostAsync<GrcBugRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcPasswordChangeResponse>> GetPasswordSettingAsync(long userId, string iPAddress) {
            try {
                var request = new GrcRequest() {
                    UserId = userId,
                    IPAddress = iPAddress,
                    Action = "Get password settings",
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/password-settings";
                return await HttpHandler.PostAsync<GrcRequest, GrcPasswordChangeResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcPasswordChangeResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> SaveUserAccountConfigurationsAsync(GrcUserAccountConfigurationsRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/user-account-configurations";
                return await HttpHandler.PostAsync<GrcUserAccountConfigurationsRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

    }
}

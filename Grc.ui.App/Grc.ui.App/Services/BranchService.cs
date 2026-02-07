using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {
    public class BranchService : GrcBaseService, IBranchService {
        public BranchService(IApplicationLoggerFactory loggerFactory, 
                             IHttpHandler httpHandler, 
                             IEnvironmentProvider environment, 
                             IEndpointTypeProvider endpointType, 
                             IMapper mapper,
                            IWebHelper webHelper,
                            SessionManager sessionManager,
                            IGrcErrorFactory errorFactory,
                            IErrorService errorService) 
        : base(loggerFactory, httpHandler, environment, endpointType, mapper,webHelper,sessionManager,errorFactory,errorService) {
        }

        public async Task<GrcResponse<BranchResponse>> GetBranchAsync(long recordId, long userId, string ipAddress) {

            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST,"Branch ID is required","Invalid Branch request");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<BranchResponse>(error);
                }

                var request = new GrcIdRequest() {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = "Retrieve branch record",
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/branches-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, BranchResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve branch record for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve branch.", ex);
            }
        }

        public async Task<GrcResponse<PagedResponse<BranchResponse>>> GetAllBranchesAsync(TableListRequest request) {
            Logger.LogActivity($"Get all branches data", "INFO");

            try{
               var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/branches-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<BranchResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving list all branches: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message,"BRANCH-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR,"Error retrieving all branches",ex.Message);
                return new GrcResponse<PagedResponse<BranchResponse>>(error);
            }
        }

        public async Task<GrcResponse<List<BranchResponse>>> GetBranchesAsync(GrcRequest request) {
            Logger.LogActivity($"Get a list of branches data", "INFO");

            try{
               var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/branches-all";
                return await HttpHandler.PostAsync<GrcRequest, List<BranchResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving a list of branches: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message,"BRANCH-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR,"Error retrieving a list of branches", ex.Message);
                return new GrcResponse<List<BranchResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateBranchAsync(BranchModel model, long userId, string ipAddress) {
            Logger.LogActivity($"Create new branch", "INFO");

            try {
                var request = new GrcBranchRequest() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Create new branch",
                    SolId = model.SolId,
                    BranchName = model.BranchName,
                    CompanyId = model.CompanyId,
                    IsDeleted = false
                };
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/create-branch";
                return await HttpHandler.PostAsync<GrcBranchRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving list all branches: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message, "BRANCH-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "Error retrieving all branches", ex.Message);
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateBranchAsync(BranchModel model, long userId, string ipAddress) {
            Logger.LogActivity($"Update branch", "INFO");

            try {
                var request = new GrcBranchRequest() {
                    Id = model.Id,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Update branch",
                    SolId = model.SolId,
                    BranchName = model.BranchName,
                    CompanyId = model.CompanyId,
                    IsDeleted = false
                };
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/update-branch";
                return await HttpHandler.PostAsync<GrcBranchRequest, ServiceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving list all branches: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message, "BRANCH-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "Error retrieving all branches", ex.Message);
                return new GrcResponse<ServiceResponse>(error);
            }
        }
        
        public async Task<GrcResponse<ServiceResponse>> DeleteBranchAsync(GrcIdRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Branch record cannot be null", "Invalid Branch record");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"DELETE BRANCH REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Organization.OrganizationBase}/delete-branch";
                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<WorkspaceResponse>> GetWorkspaceAsync(long userId, long requestingUserId, string ipAddress) {
            
            if(userId == 0) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "User ID is required",
                    "Invalid user request"
                );
        
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<WorkspaceResponse>(error);
            }

            try {
                
                var request = new UserByIdRequest() {
                    UserId = userId,
                    RecordId = requestingUserId,
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/getworkspace";
                return await HttpHandler.PostAsync<UserByIdRequest, WorkspaceResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user workspace info {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"BRANCH-SERVICE" , ex.StackTrace);
                throw new GRCException("Uanble to retrieve user workspace info", ex);
            }
        }

        
    }
}

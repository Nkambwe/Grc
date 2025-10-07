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

        public async Task<GrcResponse<PagedResponse<BranchResponse>>> GetAllBranchesAsync(TableListRequest request) {
            Logger.LogActivity($"Get all branches data", "INFO");

            try{
               var endpoint = $"{EndpointProvider.Organization.AllBranches}";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<BranchResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving list all branches: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message,"BRANCH-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving all branches",
                    ex.Message
                );

                return new GrcResponse<PagedResponse<BranchResponse>>(error);
            }
        }

        public async Task<GrcResponse<List<BranchResponse>>> GetBranchesAsync(GrcRequest request) {
            Logger.LogActivity($"Get a list of branches data", "INFO");

            try{
               var endpoint = $"{EndpointProvider.Organization.GetBranches}";
                return await HttpHandler.PostAsync<GrcRequest, List<BranchResponse>>(endpoint, request);

                //var branches = new List<BranchResponse> {
                //    new() {
                //        Id = 1,
                //        BranchName = "Main Branch",
                //        CompanyName = "Pearl Bank Uganda",
                //        SolId = "MAIN",
                //        IsDeleted = false,
                //    },
                //    new() {
                //        Id = 1,
                //        BranchName = "Kampala Road",
                //        CompanyName = "Pearl Bank Uganda",
                //        SolId = "1002",
                //        IsDeleted = false,
                //    },
                //    new() {
                //        Id = 1,
                //        BranchName = "City Branch",
                //        CompanyName = "Pearl Bank Uganda",
                //        SolId = "025",
                //        IsDeleted = false,
                //    },
                //};
                //return await Task.FromResult(new GrcResponse<List<BranchResponse>>());
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving a list of branches: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message,"BRANCH-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving a list of branches",
                    ex.Message
                );

                return new GrcResponse<List<BranchResponse>>(error);
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

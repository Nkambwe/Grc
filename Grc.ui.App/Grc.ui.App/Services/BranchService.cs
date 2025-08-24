using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {
    public class BranchService : GrcBaseService, IBranchService {
        public BranchService(IApplicationLoggerFactory loggerFactory, 
                             IHttpHandler httpHandler, 
                             IEnvironmentProvider environment, 
                             IEndpointTypeProvider endpointType, IMapper mapper)
            : base(loggerFactory, httpHandler, environment, endpointType, mapper) {
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
                throw new GRCException("Uanble to retrieve user workspace info", ex);
            }
        }
    }
}

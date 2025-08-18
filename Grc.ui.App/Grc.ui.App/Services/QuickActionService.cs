using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {
    public class QuickActionService : GrcBaseService, IQuickActionService  {

        public QuickActionService(
                    IApplicationLoggerFactory loggerFactory, 
                    IHttpHandler httpHandler, 
                    IEnvironmentProvider environment, 
                    IEndpointTypeProvider endpointType, 
                    IMapper mapper) 
                    : base(loggerFactory, httpHandler, environment, endpointType, mapper) {
        }

        public async Task<GrcResponse<IList<QuickAction>>> GetQuickActionsync(long userId, string ipAddress) {
            Logger.LogActivity($"Get current user quick action menu items", "INFO");

            try{
                var endpoint = $"{EndpointProvider.Sam.Users}/getQuickActions";
            
                    //..generate request body
                    var model = new UserByIdRequest() {
                        UserId = userId,
                        RecordId = userId,
                        IPAddress = ipAddress,
                        EncryptFields = Array.Empty<string>(),
                        DecryptFields = Array.Empty<string>(),
                    };
                    return await HttpHandler.PostAsync<UserByIdRequest, IList<QuickAction>>(endpoint,model);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving user quick action items: {ex.Message}", "Error");
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving user quick action items",
                    ex.Message
                );

                return new GrcResponse<IList<QuickAction>>(error);
            }
        }
    }
}

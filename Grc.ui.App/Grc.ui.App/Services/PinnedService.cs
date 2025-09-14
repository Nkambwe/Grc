using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {
    public class PinnedService: GrcBaseService, IPinnedService {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PinnedService(
                    IHttpContextAccessor httpContextAccessor,
                    IApplicationLoggerFactory loggerFactory, 
                    IHttpHandler httpHandler, 
                    IEnvironmentProvider environment, 
                    IEndpointTypeProvider endpointType, 
                    IMapper mapper,
                    IWebHelper webHelper,
                    SessionManager sessionManager,
                    IGrcErrorFactory errorFactory,
                    IErrorService errorService) 
        : base(loggerFactory, httpHandler, environment, endpointType, mapper,webHelper,sessionManager,errorFactory,errorService) {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GrcResponse<IList<PinnedItem>>> GetPinnedItemAsync(long userId, string ipAddress) {
            Logger.LogActivity($"Get Current user pinned menu items", "INFO");

            try{
                var endpoint = $"{EndpointProvider.Sam.Users}/getPinnedItems";
            
                    //..generate request body
                    var model = new UserByIdRequest() {
                        UserId = userId,
                        RecordId = userId,
                        IPAddress = ipAddress,
                        EncryptFields = Array.Empty<string>(),
                        DecryptFields = Array.Empty<string>(),
                    };
                    return await HttpHandler.PostAsync<UserByIdRequest, IList<PinnedItem>>(endpoint,model);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving user pinned menu items: {ex.Message}", "Error");
                 await ProcessErrorAsync(ex.Message,"PINNED-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving user pinned menu items",
                    ex.Message
                );

                return new GrcResponse<IList<PinnedItem>>(error);
            }
        }
    }
}

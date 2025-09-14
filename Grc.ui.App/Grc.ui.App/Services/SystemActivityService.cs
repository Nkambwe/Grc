using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class SystemActivityService : GrcBaseService, ISystemActivityService {
        public SystemActivityService(IApplicationLoggerFactory loggerFactory, 
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

        public async Task<GrcResponse<PagedResponse<ActivityModel>>> GetActivityLogsAsync(TableListRequest model) {
            Logger.LogActivity($"Get a list of all activities", "INFO");

            try{
               var endpoint = $"{EndpointProvider.ActivityLog.AllActivities}";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<ActivityModel>>(endpoint, model);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving all activities: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message,"SYSTEMACTIVITY-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving list of activities",
                    ex.Message
                );

                return new GrcResponse<PagedResponse<ActivityModel>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> InsertActivityAsync(long userId, string activity, string comment, string systemKeyword=null, string entityName=null, string ipAddress = null)  {

            try {
                //..build request model
                var request = new AcivityRequest() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Activity = activity,
                    Comment = comment,
                    SystemKeyword = systemKeyword,
                    EntityName = entityName
                };

                //..map request
                Logger.LogActivity($"ACTIVITY REQUEST : {JsonSerializer.Serialize(request)}");
                
                //..build endpoint
                var endpoint = $"{EndpointProvider.ActivityLog.InsertActivity}";
                Logger.LogActivity($"Endpoint: {endpoint}");
        
                return await HttpHandler.PostAsync<AcivityRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message,"SYSTEMACTIVITY-SERVICE" , httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message,"SYSTEMACTIVITY-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

    }

}

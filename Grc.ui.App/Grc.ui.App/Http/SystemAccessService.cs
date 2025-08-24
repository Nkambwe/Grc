using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace Grc.ui.App.Http {

    public class SystemAccessService : GrcBaseService, ISystemAccessService {
        
        public SystemAccessService(IApplicationLoggerFactory loggerFactory, 
                                   IHttpHandler httpHandler,
                                   IEnvironmentProvider environment, 
                                   IEndpointTypeProvider endpointType,
                                   IMapper mapper)
            :base(loggerFactory, httpHandler, environment, endpointType, mapper){
            
        }

        public async Task<RecordCountResponse> CountActiveUsersAsync(long requestingUserId, string ipAddress) {
            try {
                Logger.LogActivity($"Retrieve count for active users", "INFO");
                var request = new GrcRequest() {
                    UserId = requestingUserId,
                    Action = Activity.COUNTUSERS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/countActiveUsers";
                var response = await HttpHandler.PostAsync<GrcRequest,RecordCountResponse>(endpoint, request);
                if(response.HasError) { 
                    
                }

                return response.Data;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user record count for active users: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to retrieve user count", ex);
            }
        }

        public async Task<RecordCountResponse> CountAllUsersAsync(long requestingUserId, string ipAddress) {
            try {
                Logger.LogActivity($"Retrieve count for all users", "INFO");
                var request = new GrcRequest() {
                    UserId = requestingUserId,
                    Action = Activity.COUNTUSERS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/countUsers";
                var response = await HttpHandler.PostAsync<GrcRequest,RecordCountResponse>(endpoint, request);
                if(response.HasError) { 
                    return new(){ Count = 0};
                }

                return response.Data;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user record count for all users: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to retrieve user count", ex);
            }
        }

        public async Task<AdminCountResponse> StatisticAsync(long requestingUserId, string ipAddress) {
            try {
                Logger.LogActivity($"Retrieve count for all users", "INFO");
                var request = new GrcRequest() {
                    UserId = requestingUserId,
                    Action = Activity.STATISTICS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/statistics";
                var response = await HttpHandler.PostAsync<GrcRequest,AdminCountResponse>(endpoint, request);
                if(response.HasError) { 
                    return new(){
                        TotalUsers = 0,
                        ActiveUsers = 0,
                        DeactivatedUsers= 0,
                        UnApprovedUsers= 0,
                        UnverifiedUsers = 0,
                        DeletedUsers= 0,
                        TotalBugs = 0,
                        NewBugs = 0,
                        BugFixes = 0,
                        BugProgress = 0,
                        UserReportedBugs = 0
                    };
                }

                return response.Data;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user statistics: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to retrieve user statistics", ex);
            }
        }

        public async Task<GrcResponse<UserModel>>  GetUserByEmailAsync(string email, long requestingUserId, string ipAddress) {
            
            if(string.IsNullOrWhiteSpace(email)) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "User Email is required",
                    "Invalid user request"
                );
        
                Logger.LogActivity($"BAD REQUEST : {JsonSerializer.Serialize(error)}");
                return new GrcResponse<UserModel>(error);
            }

            try {
                
                var request = new UserByEmailRequet() {
                    UserId = requestingUserId,
                    Email = email,
                    Action = Activity.RETRIVEUSERBYEMAIL.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/getByEmail";
                return await HttpHandler.PostAsync<UserByEmailRequet, UserModel>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user record for User Email {email}: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task<GrcResponse<UserModel>> GetUserByIdAsync(long userId, long requestingUserId, string ipAddress) {
            
            if(userId == 0) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "User ID is required",
                    "Invalid user request"
                );
        
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<UserModel>(error);
            }

            try {
                
                var request = new UserByIdRequest() {
                    UserId = userId,
                    RecordId = requestingUserId,
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/getById";
                return await HttpHandler.PostAsync<UserByIdRequest, UserModel>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user record for User ID {userId}: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task<GrcResponse<UserModel>> GetUserByUsernameAsync(string username, long requestingUserId, string ipAddress) {

            if(string.IsNullOrWhiteSpace(username)) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Username is required",
                    "Invalid user request"
                );
        
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<UserModel>(error);
            }

            try {
                
                var request = new UserByNameRequest() {
                    UserId = requestingUserId,
                    Username = username,
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/getByUsername";
                return await HttpHandler.PostAsync<UserByNameRequest, UserModel>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user record for {username}: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task UpdateLoggedInStatusAsync(long userId, bool isLoggedIn, string ipAddress) {
            try {
                //..create request
                var model = new LogoutRequest(){ 
                    UserId = userId,
                    IsLoggedOut = isLoggedIn,
                    IPAddress = ipAddress,
                    Action = Activity.LOGIN.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };
                 
                //..map endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/logout";

                //..post request
                await HttpHandler.PostAsync<LogoutRequest>(endpoint, model);
            } catch (Exception ex) {
                Logger.LogActivity($"Logout failed: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                var error = new GrcResponseError(
                    (int)GrcStatusCodes.SERVERERROR,
                    "User loggout failed, an error occurred",
                    ex.Message
                ); 
                Logger.LogActivity($"SYSTEM ACCESS RESPONSE: {JsonSerializer.Serialize(error)}");
             }
        }

        public async Task<GrcResponse<UsernameValidationResponse>> ValidateUsernameAsync(UsernameValidationModel model) {

            try {
                //..map request
                var request = Mapper.Map<UsernameValidationRequest>(model);

                //..map endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/validate-username";

                //..post request
                return await HttpHandler.PostAsync<UsernameValidationRequest, UsernameValidationResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Authentication failed: {ex.Message}", "ERROR");
                var error = new GrcResponseError(
                    (int)GrcStatusCodes.SERVERERROR,
                    "Username validation failed, an error occurred",
                    ex.Message
                ); 
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
               return new GrcResponse<UsernameValidationResponse>(error);
            }
        }
    }

}

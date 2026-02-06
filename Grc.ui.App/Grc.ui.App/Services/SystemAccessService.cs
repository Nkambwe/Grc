using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Net;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class SystemAccessService : GrcBaseService, ISystemAccessService {

        public SystemAccessService(IApplicationLoggerFactory loggerFactory,
                                   IHttpHandler httpHandler,
                                   IEnvironmentProvider environment,
                                   IEndpointTypeProvider endpointType,
                                   IMapper mapper,
                                   IWebHelper webHelper,
                                   SessionManager sessionManager,
                                   IGrcErrorFactory errorFactory,
                                   IErrorService errorService)
             : base(loggerFactory, httpHandler, environment, endpointType, mapper,webHelper, sessionManager,errorFactory,errorService) {

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
                var response = await HttpHandler.PostAsync<GrcRequest, RecordCountResponse>(endpoint, request);
                if (response.HasError) {

                }

                return response.Data;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user record count for active users: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
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
                var response = await HttpHandler.PostAsync<GrcRequest, RecordCountResponse>(endpoint, request);
                if (response.HasError) {
                    return new() { Count = 0 };
                }

                return response.Data;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user record count for all users: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
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
                var response = await HttpHandler.PostAsync<GrcRequest, AdminCountResponse>(endpoint, request);
                if (response.HasError) {
                    return new() {
                        TotalUsers = 0,
                        ActiveUsers = 0,
                        DeactivatedUsers = 0,
                        UnApprovedUsers = 0,
                        UnverifiedUsers = 0,
                        DeletedUsers = 0,
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
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                throw new GRCException("Uanble to retrieve user statistics", ex);
            }
        }

        #region System Activity

        public async Task<GrcResponse<GrcSystemActivityResponse>> GetSystemActivityIdAsync(long userId, long recordId, string ipAddress) {

            if (recordId == 0) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Record ID is required",
                    "Invalid activity request"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<GrcSystemActivityResponse>(error);
            }

            try {

                var request = new GrcIdRequest() {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.RETRIEVEALLSYSTEMACTIVITIES.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/getactivity";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcSystemActivityResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system activity with ID {recordId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "DEPARTMENT-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve system activity.", ex);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcSystemActivityResponse>>> GetPagedActivitiesAsync(TableListRequest request) {
            if (request == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid Request object",
                    "Request object cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<GrcSystemActivityResponse>>(error);
            }

            try {
                var endpoint = $"{EndpointProvider.Sam.Users}/getactivities";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcSystemActivityResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcSystemActivityResponse>>(error);
            }
        }

        #endregion

        #region Users

        public async Task<GrcResponse<GrcUserSupportResponse>> GetUserSupportAsync(long userId, string ipAddress) {
            try {

                var request = new GrcRequest() {
                    UserId = userId,
                    Action = Activity.RETRIVEUSERBYEMAIL.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/user-support";
                return await HttpHandler.PostAsync<GrcRequest, GrcUserSupportResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user support items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve user support items.", ex);
            }
        }

        public async Task<GrcResponse<List<RoleGroupItemResponse>>> GetSelectedRoleGroupsAsync(long userId, long id, string ipAddress) {
            try {

                var request = new GrcIdRequest() {
                    RecordId = id,
                    UserId = userId,
                    Action = Activity.RETRIVEUSERBYEMAIL.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/role-rolegroup-list";
                return await HttpHandler.PostAsync<GrcIdRequest, List<RoleGroupItemResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve department units: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve role groups.", ex);
            }
        }

        public async Task<GrcResponse<List<UnitItemResponse>>> GetSelectedUnitsAsync(long userId, long id, string ipAddress) {
            try {

                var request = new GrcIdRequest() {
                    RecordId = id,
                    UserId = userId,
                    Action = Activity.RETRIVEUSERBYEMAIL.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/department-units-mini-list";
                return await HttpHandler.PostAsync<GrcIdRequest, List<UnitItemResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve department units: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve department units.", ex);
            }
        }


        public async Task<GrcResponse<UserResponse>> GetUserByEmailAsync(string email, long requestingUserId, string ipAddress)
        {

            if (string.IsNullOrWhiteSpace(email))
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "User Email is required",
                    "Invalid user request"
                );

                Logger.LogActivity($"BAD REQUEST : {JsonSerializer.Serialize(error)}");
                return new GrcResponse<UserResponse>(error);
            }

            try
            {

                var request = new UserByEmailRequet()
                {
                    UserId = requestingUserId,
                    Email = email,
                    Action = Activity.RETRIVEUSERBYEMAIL.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/getByEmail";
                return await HttpHandler.PostAsync<UserByEmailRequet, UserResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user record for User Email {email}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "DEPARTMENT-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task<GrcResponse<UserResponse>> GetUserByIdAsync(long userId, long recordId, string ipAddress)
        {

            if (recordId == 0)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "User ID is required",
                    "Invalid user request"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<UserResponse>(error);
            }

            try
            {

                var request = new UserByIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.RETRIVEUSERBYID.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/getById";
                return await HttpHandler.PostAsync<UserByIdRequest, UserResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user record for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "DEPARTMENT-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task<GrcResponse<UserResponse>> GetUserByUsernameAsync(string username, long requestingUserId, string ipAddress)
        {

            if (string.IsNullOrWhiteSpace(username))
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Username is required",
                    "Invalid user request"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<UserResponse>(error);
            }

            try
            {

                var request = new UserByNameRequest()
                {
                    UserId = requestingUserId,
                    Username = username,
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/getByUsername";
                return await HttpHandler.PostAsync<UserByNameRequest, UserResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user record for {username}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "DEPARTMENT-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task<GrcResponse<ListResponse<UserResponse>>> GetUsersAsync(GrcRequest request)
        {

            if (request == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid request",
                    "Request body cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ListResponse<UserResponse>>(error);
            }

            try
            {
                var endpoint = $"{EndpointProvider.Sam.Users}/getusers";
                return await HttpHandler.PostAsync<GrcRequest, ListResponse<UserResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user records: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SYSTEM-ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task<GrcResponse<PagedResponse<UserResponse>>> GetPagedUsersAsync(TableListRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }

            try {
                var endpoint = $"{EndpointProvider.Sam.Users}/pagedusers";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<UserResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<UserResponse>>> GetUnapprovedUsersAsync(TableListRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }

            try {
                var endpoint = $"{EndpointProvider.Sam.Users}/unapproved";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<UserResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<UserResponse>>> GetUnverifiedUsersAsync(TableListRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }

            try {
                var endpoint = $"{EndpointProvider.Sam.Users}/unverified-users";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<UserResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<UserResponse>>> GetDeletedUsersAsync(TableListRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }

            try {
                var endpoint = $"{EndpointProvider.Sam.Users}/deleted-users";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<UserResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<UserResponse>>> GetLockedUsersAsync(TableListRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }

            try {
                var endpoint = $"{EndpointProvider.Sam.Users}/locked-accounts";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<UserResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<UserResponse>>(error);
            }
        }
        
        public async Task<GrcResponse<ServiceResponse>> LockUserAsync(GrcIdRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST,"User record cannot be null","Invalid user record");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"LOCK USER REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/lock-user";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred",httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }

        }

        public async Task<GrcResponse<ServiceResponse>> UnlockUserAsync(GrcIdRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST,"User record cannot be null","Invalid user record");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"UNLOCK USER REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/unlock-user";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred",httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }

        }

        public async Task<GrcResponse<ServiceResponse>> RestoreUserAsync(GrcIdRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST,"User record cannot be null","Invalid user record");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"RESTORE USER REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/restore-user";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred",httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> PasswordResetAsync(GrcIdRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "User record cannot be null", "Invalid user record");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"PASSWORD RESET REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/password-reset";
                Logger.LogActivity($"Endpoint: {endpoint}");

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

        public async Task<GrcResponse<ServiceResponse>> ChangePasswordAsync(GrcPasswordChangeRequest request) {
            if (request == null) {
                var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "User record cannot be null", "Invalid user record");
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"PASSWORD CHANGE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/password-change";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcPasswordChangeRequest, ServiceResponse>(endpoint, request);
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

        public async Task<GrcResponse<ServiceResponse>> CreateUserAsync(UserViewModel userRecord, long userId, string ipAddress)
        {
            if (userRecord == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "User record cannot be null",
                    "Invalid user record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try
            {
                //..build request model
                var request = Mapper.Map<UserModel>(userRecord);
                request.UserId = userId;
                request.IPAddress = ipAddress;
                request.Action = Activity.USER_ADDED.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE USER REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/createuser";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<UserModel, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateUserAsync(UserViewModel userRecord, long userId, string ipAddress)
        {
            if (userRecord == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "User record cannot be null",
                    "Invalid user record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try
            {
                //..build request model
                var request = Mapper.Map<UserModel>(userRecord);
                request.UserId = userId;
                request.IPAddress = ipAddress;
                request.Action = Activity.USER_EDITED.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE USER REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/updateuser";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<UserModel, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteUserAsync(GrcIdRequest request)
        {
            if (request == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "User record cannot be null",
                    "Invalid user record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try
            {
                //..map request
                Logger.LogActivity($"DELETE USER REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/deleteuser";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }

        }

        public async Task UpdateLoggedInStatusAsync(long userId, bool isLoggedIn, string ipAddress)
        {
            try
            {
                //..create request
                var model = new LogoutRequest()
                {
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
                await HttpHandler.PostAsync(endpoint, model);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Logout failed: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "DEPARTMENT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    (int)GrcStatusCodes.SERVERERROR,
                    "User loggout failed, an error occurred",
                    ex.Message
                );
                Logger.LogActivity($"SYSTEM ACCESS RESPONSE: {JsonSerializer.Serialize(error)}");
            }
        }

        public async Task<GrcResponse<UsernameValidationResponse>> ValidateUsernameAsync(UsernameValidationModel model)
        {

            try
            {
                //..map request
                var request = Mapper.Map<UsernameValidationRequest>(model);

                //..map endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/validate-username";

                //..post request
                return await HttpHandler.PostAsync<UsernameValidationRequest, UsernameValidationResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Authentication failed: {ex.Message}", "ERROR");
                var error = new GrcResponseError(
                    (int)GrcStatusCodes.SERVERERROR,
                    "Username validation failed, an error occurred",
                    ex.Message
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                await ProcessErrorAsync(ex.Message, "SYSTEM-ACCESS-SERVICE", ex.StackTrace);
                return new GrcResponse<UsernameValidationResponse>(error);
            }
        }
        
        public async Task<GrcResponse<ServiceResponse>> ApproveUserAsync(ApproveUserViewModel model, long userId, string ipAddress) {
            if (model == null) {
                var error = new GrcResponseError( GrcStatusCodes.BADREQUEST, "Request record cannot be null","Invalid user record");

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..build request model
                var request = new GrcApproveUserRequest(){ 
                    Id = model.Id,
                    IsApproved = model.IsApproved,
                    IsVerified = model.IsVerified,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Approve user account"
                };

                //..map request
                Logger.LogActivity($"APPROVE ACCOUNT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/approve-user";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcApproveUserRequest, ServiceResponse>(endpoint, request);
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

        public async  Task<GrcResponse<ServiceResponse>> VerifyUserAsync(ApproveUserViewModel model, long userId, string ipAddress) {
            if (model == null) {
                var error = new GrcResponseError( GrcStatusCodes.BADREQUEST, "Request record cannot be null","Invalid user record");

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..build request model
                var request = new GrcApproveUserRequest(){ 
                    Id = model.Id,
                    IsApproved = model.IsApproved,
                    IsVerified = model.IsVerified,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Verify User account"
                };

                //..map request
                Logger.LogActivity($"VERIFY USER REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/verify-user";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcApproveUserRequest, ServiceResponse>(endpoint, request);
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

        #endregion

        #region System Permissions

        public async Task<GrcResponse<ListResponse<GrcPermissionResponse>>> GetPermissionsAsync(GrcRequest request) {
            if (request == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid request",
                    "Request body cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ListResponse<GrcPermissionResponse>>(error);
            }

            try
            {
                var endpoint = $"{EndpointProvider.Sam.Permissions}/get-permissions";
                return await HttpHandler.PostAsync<GrcRequest, ListResponse<GrcPermissionResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system permissions: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SYSTEM-ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve system permissions.", ex);
            }
        }

        #endregion

        #region Roles

        public async Task<GrcResponse<GrcRoleResponse>> GetRoleByIdAsync(long recordId, long userId, string ipAddress) {
           
            try
            {
                if (recordId == 0)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Role ID is required",
                        "Invalid Role request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcRoleResponse>(error);
                }


                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.RETRIVEROLEBYID.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Roles}/getrole-by-id";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcRoleResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role record for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve role.", ex);
            }
        }

        public async Task<GrcResponse<GrcRoleResponse>> GetRoleWithUsersAsync(long id, long userId, string ipAddress)
        {
            if (id == 0)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role ID is required",
                    "Invalid Role request"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<GrcRoleResponse>(error);
            }

            try
            {
                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = id,
                    IPAddress = ipAddress,
                    Action = Activity.RETRIVEROLEBYID.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Roles}/getrole-users";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcRoleResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role record for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve role.", ex);
            }
        }

        public async Task<GrcResponse<GrcRoleResponse>> GetRoleWithPermissionsAsync(long id, long userId, string ipAddress)
        {
            if (id == 0)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role ID is required",
                    "Invalid Role request"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<GrcRoleResponse>(error);
            }

            try
            {
                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = id,
                    IPAddress = ipAddress,
                    Action = Activity.RETRIVEROLEBYID.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Permissions}/getrole-permissions";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcRoleResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role record for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve role.", ex);
            }
        }

        public async Task<GrcResponse<GrcRoleResponse>> GetRoleWithPermissionSetsAsync(long id, long userId, string ipAddress)
        {
            if (id == 0)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role ID is required",
                    "Invalid Role request"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<GrcRoleResponse>(error);
            }

            try
            {
                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = id,
                    IPAddress = ipAddress,
                    Action = Activity.RETRIVEROLEBYID.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Permissions}/getrole-permissionsets";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcRoleResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role record for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve role.", ex);
            }
        }

        public async Task<GrcResponse<ListResponse<GrcRoleResponse>>> GetRolesAsync(GrcRequest request) {

            if (request == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid request",
                    "Request body cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ListResponse<GrcRoleResponse>>(error);
            }

            try
            {
                var endpoint = $"{EndpointProvider.Sam.Roles}/getroles";
                return await HttpHandler.PostAsync<GrcRequest, ListResponse<GrcRoleResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve roles records: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SYSTEM-ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve system role.", ex);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcRoleResponse>>> GetPagedRolesAsync(TableListRequest request) {
            if (request == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid Request object",
                    "Request object cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<GrcRoleResponse>>(error);
            }

            try
            {
                var endpoint = $"{EndpointProvider.Sam.Roles}/pagedroles";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcRoleResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcRoleResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateRoleAsync(RoleViewModel roleRecord, long userId, string ipAddress)
        {
            if (roleRecord == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role record cannot be null",
                    "Invalid role record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try
            {
                //..build request object
                var request = Mapper.Map<GrcRoleRequest>(roleRecord);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.ROLE_ADDED.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE ROLE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Roles}/createrole";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcRoleRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateRoleAsync(RoleViewModel roleRecord, long userId, string ipAddress)
        {
            if (roleRecord == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role record cannot be null",
                    "Invalid role record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try
            {
                //..build request object
                var request = Mapper.Map<GrcRoleRequest>(roleRecord);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.ROLE_EDITED.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE ROLE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Roles}/updaterole";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcRoleRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteRoleAsync(GrcIdRequest request) {
            if (request == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role record cannot be null",
                    "Invalid Role record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try
            {
                //..map request
                Logger.LogActivity($"DELETE SYSTEM ROLE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Roles}/deleterole";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Role Groups

        public async Task<GrcResponse<GrcRoleGroupResponse>> GetRoleGroupWithRolesByIdAsync(long recordId, long userId, string ipAddress) {
            if (recordId == 0) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role Group ID is required",
                    "Invalid Role Group request"
                ); 

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<GrcRoleGroupResponse>(error);
            }

            try {
                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.RETRIVEROLEBYID.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Roles}/getrolegroup-by-id";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcRoleGroupResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role group for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SYSTEM-ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve role group.", ex);
            }
        }

        public async Task<GrcResponse<GrcRoleGroupResponse>> GetRoleGroupWithPermissionSetsByIdAsync(long recordId, long userId, string ipAddress)
        {
            if (recordId == 0)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role Group ID is required",
                    "Invalid Role Group request"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<GrcRoleGroupResponse>(error);
            }

            try
            {
                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.RETRIVEROLEBYID.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Roles}/getrolegroupwithpermissionsets";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcRoleGroupResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role group for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SYSTEM-ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve role group.", ex);
            }
        }

        public async Task<GrcResponse<ListResponse<GrcRoleGroupResponse>>> GetRoleGroupsAsync(GrcRequest request) {

            if (request == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid request",
                    "Request body cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ListResponse<GrcRoleGroupResponse>>(error);
            }

            try
            {
                var endpoint = $"{EndpointProvider.Sam.Roles}/getrolegroups";
                return await HttpHandler.PostAsync<GrcRequest, ListResponse<GrcRoleGroupResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve roles groups: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SYSTEM-ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve system role groups.", ex);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcRoleGroupResponse>>> GetPagedRoleGroupsAsync(TableListRequest request) {
            if (request == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid Request object",
                    "Request object cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<GrcRoleGroupResponse>>(error);
            }

            try {
                var endpoint = $"{EndpointProvider.Sam.Roles}/pagedrolegroups";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcRoleGroupResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcRoleGroupResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcRoleGroupResponse>>> GetPagedRoleGroupWithPermissionSetsAsync(TableListRequest request) {
            if (request == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid Request object",
                    "Request object cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<GrcRoleGroupResponse>>(error);
            }

            try
            {
                var endpoint = $"{EndpointProvider.Sam.Roles}/pagedrolegroupsWithPermissionsets";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcRoleGroupResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcRoleGroupResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateRoleGroupAsync(RoleGroupViewModel roleRecord, long userId, string ipAddress) {
            if (roleRecord == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role group record cannot be null",
                    "Invalid role group record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..build request object
                var request = Mapper.Map<GrcRoleGroupRequest>(roleRecord);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.ROLE_GROUP_ADDED.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE ROLE GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Roles}/createrolegroup";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcRoleGroupRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateRoleGroupAsync(RoleGroupViewModel roleRecord, long userId, string ipAddress) {
            if (roleRecord == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role group record cannot be null",
                    "Invalid role group record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..build request object
                var request = Mapper.Map<GrcRoleGroupRequest>(roleRecord);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.ROLE_GROUP_EDITED.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE ROLE GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Roles}/updaterolegroup";
                Logger.LogActivity($"Endpoint: {endpoint}");
                return await HttpHandler.PostAsync<GrcRoleGroupRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }

        }

        public async Task<GrcResponse<ServiceResponse>> DeleteRoleGroupAsync(GrcIdRequest request) {
            if (request == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role Group cannot be null",
                    "Invalid Role Group record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"DELETE SYSTEM ROLE GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Roles}/deleterolegroup";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }

        }

        public async Task<GrcResponse<ServiceResponse>> UpdateRoleGroupWithPermissionsAsync(RoleGroupViewModel groupModel, long userId, string ipAddress) {

            if (groupModel == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role Group record cannot be null",
                    "Invalid Role Group record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..build request object
                var request = Mapper.Map<GrcRoleGroupRequest>(groupModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.ROLE_GROUP_ADDED.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE ROLE GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.RoleGroups}/update-rolegroup-permissionsets";
                Logger.LogActivity($"Endpoint: {endpoint}");
                return await HttpHandler.PostAsync<GrcRoleGroupRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateRoleGroupWithPermissionsAsync(RoleGroupViewModel groupModel, long userId, string ipAddress) {
            if (groupModel == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Role group record cannot be null",
                    "Invalid role group record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {

                //..build request object
                var request = Mapper.Map<GrcRoleGroupRequest>(groupModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.ROLE_GROUP_ADDED.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE ROLE GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Roles}/create-rolegroup-permissionsets";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcRoleGroupRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Permission Sets

        public async Task<GrcResponse<GrcPermissionSetResponse>> GetPermissionSetIdAsync(long recordId, long userId, string ipAddress) {
            if (recordId == 0)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Permission Set ID is required",
                    "Invalid Permission Set request"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<GrcPermissionSetResponse>(error);
            }

            try {
                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PERMISSION_SET_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Permissions}/retrieve-permission-set";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcPermissionSetResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve permission set record for User ID {userId}: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve permission set.", ex);
            }
        }

        public async Task<GrcResponse<ListResponse<GrcPermissionSetResponse>>> GetPermissionSetAsync(GrcRequest request) {
            if (request == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid request",
                    "Request body cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ListResponse<GrcPermissionSetResponse>>(error);
            }

            try
            {
                var endpoint = $"{EndpointProvider.Sam.Permissions}/get-permissionsets";
                return await HttpHandler.PostAsync<GrcRequest, ListResponse<GrcPermissionSetResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve permission sets: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SYSTEM-ACCESS-SERVICE", ex.StackTrace);
                throw new GRCException("Uanble to retrieve permission sets.", ex);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcPermissionSetResponse>>> GetPagedPermissionSetAsync(TableListRequest request) {
            if (request == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Invalid Request object",
                    "Request object cannot be null"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<PagedResponse<GrcPermissionSetResponse>>(error);
            }

            try
            {
                var endpoint = $"{EndpointProvider.Sam.Permissions}/pagedsets";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcPermissionSetResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcPermissionSetResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreatePermissionSetAsync(GrcPermissionSetViewModel roleRecord, long userId, string ipAddress) {
            if (roleRecord == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Permission Set record cannot be null",
                    "Invalid Permission Set record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try
            {
                //..build request object
                var request = Mapper.Map<GrcPermissionSetRequest>(roleRecord);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PERMISSION_SET_ADDED.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE PERMISSION SET REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Permissions}/create-permissionset";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcPermissionSetRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdatePermissionSetAsync(GrcPermissionSetViewModel setRecord, long userId, string ipAddress) {
            if (setRecord == null)
            {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Permission Set record cannot be null",
                    "Invalid Permission Set record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try
            {
                //..build request object
                var request = Mapper.Map<GrcPermissionSetRequest>(setRecord);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PERMISSION_SET_RETRIVED.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE PERMISSION SET REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Permissions}/update-permissionsets";
                Logger.LogActivity($"Endpoint: {endpoint}");
                return await HttpHandler.PostAsync<GrcPermissionSetRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeletePermissionSetAsync(GrcIdRequest request) {
            if (request == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Permission set cannot be null",
                    "Invalid permission set record"
                );

                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"DELETE PERMISSION SET REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Sam.Permissions}/delete-permissionset";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "SYSTEM-ACCESS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "SYSTEM_ACCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

    }

}

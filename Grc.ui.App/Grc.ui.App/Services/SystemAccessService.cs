using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
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
             : base(loggerFactory, httpHandler, environment, 
                                  endpointType, mapper,webHelper,
                                  sessionManager,errorFactory,errorService) {

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

        public async Task<GrcResponse<UserModel>> GetUserByEmailAsync(string email, long requestingUserId, string ipAddress) {

            if (string.IsNullOrWhiteSpace(email)) {
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
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task<GrcResponse<UserModel>> GetUserByIdAsync(long userId, long requestingUserId, string ipAddress) {

            if (userId == 0) {
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
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }

        public async Task<GrcResponse<UserModel>> GetUserByUsernameAsync(string username, long requestingUserId, string ipAddress) {

            if (string.IsNullOrWhiteSpace(username)) {
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
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                throw new GRCException("Uanble to retrieve user.", ex);
            }
        }


        public async Task<GrcResponse<List<UserModel>>> GetUsersAsync(GrcRequest request)
        {
            var users = new List<UserModel>
            {
                new() {
                    UserId = 1,
                    FirstName = "Wendi",
                    LastName = "Mukasa",
                    MiddleName = "Allan",
                    UserName = "wmukasa",
                    EmailAddress = "wendi.mukasa@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "Wendi A. Mukasa",
                    PhoneNumber = "+256700000001",
                    PFNumber = "PF001",
                    SolId = "001",
                    RoleId = 1,
                    RoleName = "Administrator",
                    RoleGroup = "System Admins",
                    DepartmentId = 10,
                    UnitCode = "HR01",
                    IsActive = true,
                    IsVerified = true,
                    IsLogged = false,
                    RequiresPasswordChange = false,
                    LastLoginIpAddress = "192.168.1.10",
                    CreatedOn = DateTime.Now.AddDays(-30),
                    CreatedBy = "system",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "system"
                },
                new() {
                    UserId = 2,
                    FirstName = "Sarah",
                    LastName = "Nabunya",
                    MiddleName = "Hope",
                    UserName = "snabunya",
                    EmailAddress = "sarah.nabunya@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "Sarah H. Nabunya",
                    PhoneNumber = "+256700000002",
                    PFNumber = "PF002",
                    SolId = "002",
                    RoleId = 2,
                    RoleName = "Manager",
                    RoleGroup = "Operations",
                    DepartmentId = 20,
                    UnitCode = "OPS01",
                    IsActive = true,
                    IsVerified = true,
                    IsLogged = false,
                    RequiresPasswordChange = false,
                    LastLoginIpAddress = "192.168.1.11",
                    CreatedOn = DateTime.Now.AddDays(-25),
                    CreatedBy = "system",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "system"
                },
                new() {
                    UserId = 3,
                    FirstName = "John",
                    LastName = "Kato",
                    MiddleName = "Michael",
                    UserName = "jkato",
                    EmailAddress = "john.kato@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "John M. Kato",
                    PhoneNumber = "+256700000003",
                    PFNumber = "PF003",
                    SolId = "003",
                    RoleId = 3,
                    RoleName = "Auditor",
                    RoleGroup = "Compliance",
                    DepartmentId = 30,
                    UnitCode = "COMP01",
                    IsActive = true,
                    IsVerified = false,
                    IsLogged = false,
                    RequiresPasswordChange = true,
                    LastLoginIpAddress = "192.168.1.12",
                    CreatedOn = DateTime.Now.AddDays(-20),
                    CreatedBy = "admin",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "admin"
                },
                new UserModel
                {
                    UserId = 4,
                    FirstName = "Paul",
                    LastName = "Okello",
                    MiddleName = "James",
                    UserName = "pokello",
                    EmailAddress = "paul.okello@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "Paul J. Okello",
                    PhoneNumber = "+256700000004",
                    PFNumber = "PF004",
                    SolId = "004",
                    RoleId = 2,
                    RoleName = "Manager",
                    RoleGroup = "Finance",
                    DepartmentId = 40,
                    UnitCode = "FIN01",
                    IsActive = true,
                    IsVerified = true,
                    IsLogged = false,
                    RequiresPasswordChange = false,
                    LastLoginIpAddress = "192.168.1.13",
                    CreatedOn = DateTime.Now.AddDays(-15),
                    CreatedBy = "system",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "system"
                },
                new UserModel
                {
                    UserId = 5,
                    FirstName = "Grace",
                    LastName = "Nambi",
                    MiddleName = "Lydia",
                    UserName = "gnambi",
                    EmailAddress = "grace.nambi@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "Grace L. Nambi",
                    PhoneNumber = "+256700000005",
                    PFNumber = "PF005",
                    SolId = "005",
                    RoleId = 4,
                    RoleName = "Analyst",
                    RoleGroup = "Finance",
                    DepartmentId = 40,
                    UnitCode = "FIN02",
                    IsActive = true,
                    IsVerified = true,
                    IsLogged = false,
                    RequiresPasswordChange = false,
                    LastLoginIpAddress = "192.168.1.14",
                    CreatedOn = DateTime.Now.AddDays(-10),
                    CreatedBy = "system",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "system"
                },
                new UserModel
                {
                    UserId = 6,
                    FirstName = "Peter",
                    LastName = "Mugisha",
                    MiddleName = "David",
                    UserName = "pmugisha",
                    EmailAddress = "peter.mugisha@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "Peter D. Mugisha",
                    PhoneNumber = "+256700000006",
                    PFNumber = "PF006",
                    SolId = "006",
                    RoleId = 3,
                    RoleName = "Auditor",
                    RoleGroup = "Compliance",
                    DepartmentId = 30,
                    UnitCode = "COMP02",
                    IsActive = false,
                    IsVerified = true,
                    IsLogged = false,
                    RequiresPasswordChange = true,
                    LastLoginIpAddress = "192.168.1.15",
                    CreatedOn = DateTime.Now.AddDays(-12),
                    CreatedBy = "admin",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "admin"
                },
                new UserModel
                {
                    UserId = 7,
                    FirstName = "Mary",
                    LastName = "Namugerwa",
                    MiddleName = "Agnes",
                    UserName = "mnamugerwa",
                    EmailAddress = "mary.namugerwa@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "Mary A. Namugerwa",
                    PhoneNumber = "+256700000007",
                    PFNumber = "PF007",
                    SolId = "007",
                    RoleId = 5,
                    RoleName = "Clerk",
                    RoleGroup = "Operations",
                    DepartmentId = 20,
                    UnitCode = "OPS02",
                    IsActive = true,
                    IsVerified = true,
                    IsLogged = false,
                    RequiresPasswordChange = false,
                    LastLoginIpAddress = "192.168.1.16",
                    CreatedOn = DateTime.Now.AddDays(-8),
                    CreatedBy = "system",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "system"
                },
                new UserModel
                {
                    UserId = 8,
                    FirstName = "David",
                    LastName = "Muwonge",
                    MiddleName = "Isaac",
                    UserName = "dmuwonge",
                    EmailAddress = "david.muwonge@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "David I. Muwonge",
                    PhoneNumber = "+256700000008",
                    PFNumber = "PF008",
                    SolId = "008",
                    RoleId = 2,
                    RoleName = "Manager",
                    RoleGroup = "HR",
                    DepartmentId = 50,
                    UnitCode = "HR02",
                    IsActive = true,
                    IsVerified = false,
                    IsLogged = false,
                    RequiresPasswordChange = true,
                    LastLoginIpAddress = "192.168.1.17",
                    CreatedOn = DateTime.Now.AddDays(-5),
                    CreatedBy = "admin",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "admin"
                },
                new UserModel
                {
                    UserId = 9,
                    FirstName = "Agnes",
                    LastName = "Nakato",
                    MiddleName = "Joyce",
                    UserName = "anakato",
                    EmailAddress = "agnes.nakato@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "Agnes J. Nakato",
                    PhoneNumber = "+256700000009",
                    PFNumber = "PF009",
                    SolId = "009",
                    RoleId = 4,
                    RoleName = "Analyst",
                    RoleGroup = "Finance",
                    DepartmentId = 40,
                    UnitCode = "FIN03",
                    IsActive = true,
                    IsVerified = true,
                    IsLogged = false,
                    RequiresPasswordChange = false,
                    LastLoginIpAddress = "192.168.1.18",
                    CreatedOn = DateTime.Now.AddDays(-3),
                    CreatedBy = "system",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "system"
                },
                new UserModel
                {
                    UserId = 10,
                    FirstName = "Robert",
                    LastName = "Lule",
                    MiddleName = "Brian",
                    UserName = "rlule",
                    EmailAddress = "robert.lule@example.com",
                    Password = "Password@123",
                    ConfirmPassword = "Password@123",
                    DisplayName = "Robert B. Lule",
                    PhoneNumber = "+256700000010",
                    PFNumber = "PF010",
                    SolId = "010",
                    RoleId = 6,
                    RoleName = "Supervisor",
                    RoleGroup = "Operations",
                    DepartmentId = 20,
                    UnitCode = "OPS03",
                    IsActive = true,
                    IsVerified = true,
                    IsLogged = true,
                    RequiresPasswordChange = false,
                    LastLoginIpAddress = "192.168.1.19",
                    CreatedOn = DateTime.Now.AddDays(-1),
                    CreatedBy = "system",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "system"
                }
            };
            
            return await Task.FromResult(new GrcResponse<List<UserModel>>(users));
        }
        public async Task UpdateLoggedInStatusAsync(long userId, bool isLoggedIn, string ipAddress) {
            try {
                //..create request
                var model = new LogoutRequest() {
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
            } catch (Exception ex) {
                Logger.LogActivity($"Logout failed: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
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
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                return new GrcResponse<UsernameValidationResponse>(error);
            }
        }

    }

}

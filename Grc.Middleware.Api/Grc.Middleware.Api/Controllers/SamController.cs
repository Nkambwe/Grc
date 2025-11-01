using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Security;
using System.Text.Json;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class SamController : GrcControllerBase {
        private readonly ISystemAccessService _accessService;
        private readonly IQuickActionService _quickActionService;
        private readonly IPinnedItemService _pinnedItemService;
        public SamController(IObjectCypher cypher, 
                            IServiceLoggerFactory loggerFactory, 
                            IMapper mapper, 
                            ICompanyService companyService,
                            IEnvironmentProvider environment,
                            ISystemAccessService accessService,
                            IQuickActionService quickActionService,
                            IPinnedItemService pinnedItemService,
                            IErrorNotificationService errorService,
                            ISystemErrorService systemErrorService) 
                            : base(cypher, loggerFactory, mapper, companyService, environment,
                                  errorService, systemErrorService) {
            _accessService = accessService;
            _quickActionService = quickActionService;
            _pinnedItemService = pinnedItemService;
        }

        #region Authentication

        [HttpPost("sam/users/validate-username")] 
        public async Task<IActionResult> ValidateUsername([FromBody] UsernameValidationRequest request) {

            try {
                Logger.LogActivity("Validate username at login", "INFO");
                if (request == null) { 
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
        
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                var response = await _accessService.ValidateUsernameAsync(request.Username);
                if(response != null){  
                    //..decrypt firstName
                    if(!string.IsNullOrEmpty(response.DisplayName)){ 
                        request.DecryptFields = new string[] { "DisplayName"};
                        response = Cypher.DecryptProperties(response, request.DecryptFields);
                    }
                    
                    Logger.LogActivity($"MIDDLEWARE-SAM RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<UsernameValidationResponse>(response));
                } else { 
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        $"Oops! Something thing went wrong",
                        "Failed to validate username. An error occurrred"
                    ); 
                    Logger.LogActivity($"MIDDLEWARE-SAM RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UsernameValidationResponse>(error));
                }
            } catch (Exception ex) { 
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"GRC-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );
        
                Logger.LogActivity($"MIDDLEWARE-SAM RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<UsernameValidationResponse>(error));
            }

        }

        [HttpPost("sam/users/auth")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] LoginRequest request) {
            try {
                Logger.LogActivity("Authenticate user at login", "INFO");
        
                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)) { 
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Username and password are required",
                        "Invalid login credentials"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuthenticationResponse>(error));
                }

                Logger.LogActivity($"Authentication Request >> {JsonSerializer.Serialize(new { Username = request.Username, HasPassword = !string.IsNullOrEmpty(request.Password) })}", "INFO");
        
                var response = await _accessService.AuthenticateUserAsync(request.Username);
                if(response != null) {  
                    //..decrypt sensitive fields 
                    if(!string.IsNullOrEmpty(response.Username) || !string.IsNullOrEmpty(response.EmailAddress)){ 
                        request.DecryptFields = new string[] { "PFNumber", "EmailAddress", "FirstName", "LastName", "MiddleName", "PhoneNumber", "Password" };
                        response = Cypher.DecryptProperties(response, request.DecryptFields);
                    }

                    //..authenticate user
                    if(response.IsActive && !response.IsDeleted) { 
                        response.IsAuthenticated = ExtendedHashMapper.VerifyPassword(request.Password, response.Password);
                    }

                    if (response.IsAuthenticated) {
                        //..update the last login for user
                         await _accessService.UpdateLastLoginAsync(response.UserId, DateTime.Now);
                    } else { 
                        //..lock account if attempts completed
                         await _accessService.LockUserAccountAsync(response.UserId);
                    }

                    Logger.LogActivity($"AUTHENTICATION SUCCESS: User {request.Username} authenticated successfully");
                    return Ok(new GrcResponse<AuthenticationResponse>(response));
                } else { 
                    var error = new ResponseError(
                        ResponseCodes.UNAUTHORIZED,
                        "Invalid username or password",
                        "Authentication failed - please check your credentials"
                    ); 
            
                    Logger.LogActivity($"AUTHENTICATION FAILED: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuthenticationResponse>(error));
                }
            } catch (Exception ex) { 
                Logger.LogActivity($"Authentication Error: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-CONTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.SERVERERROR,
                    "Authentication service temporarily unavailable",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE-SAM RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<AuthenticationResponse>(error));
            }
        }

        [HttpPost("sam/users/logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest) {
            try {
                Logger.LogActivity($"Action - {logoutRequest.Action} on IP Address {logoutRequest.IPAddress}", "INFO");
                var status = await _accessService.UpdateLoginStatusAsync(logoutRequest.UserId, DateTime.Now);
                if(!status){
                    var error = new ResponseError(
                        ResponseCodes.NOTUPDATE,
                        "Login status not updated",
                        "An error occurred! could not update login status");
                     return Ok(new GrcResponse<StatusResponse>(error));
                } 
               return Ok(new GrcResponse<StatusResponse>(new StatusResponse(){Status = status}));
            } catch (Exception ex) {
                Logger.LogActivity($"Error updating logged_in status for user {logoutRequest.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return StatusCode(500, "An error occurred while logging out.");
            }
        }
        
        #endregion

        #region Dashboard

        [HttpPost("sam/users/statistics")]
        public async Task<IActionResult> Statistics([FromBody] GeneralRequest request) {
                try {
                    Logger.LogActivity($"{request.Action}", "INFO");

                    if (request == null) {
                        var error = new ResponseError(
                            ResponseCodes.BADREQUEST,
                            "Request record cannot be empty",
                            "Invalid request body"
                        );
                        Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<AdminCountResponse>(error));
                    }

                    Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                    var statistics = await _accessService.GetAdminiDashboardStatisticsAsync();
                    //..map response
                    statistics ??= new AdminCountResponse() {
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

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(statistics)}");
                    return Ok(new GrcResponse<AdminCountResponse>(statistics));
                } catch (Exception ex) {
                    Logger.LogActivity($"{ex.Message}", "ERROR");
                    Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                    
                    var conpany = await CompanyService.GetDefaultCompanyAsync();
                    long companyId = conpany != null ? conpany.Id : 1;
                    SystemError errorObj = new(){ 
                        ErrorMessage = ex.Message,
                        ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                        StackTrace = ex.StackTrace,
                        Severity = "CRITICAL",
                        ReportedOn = DateTime.Now,
                        CompanyId = companyId
                    };

                    //..save error object to the database
                    var result = await SystemErrorService.SaveErrorAsync(errorObj);
                    var response = new GeneralResponse();
                    if(result){
                        response.Status = true;
                        response.StatusCode = (int)ResponseCodes.SUCCESS;
                        response.Message = "Error captured and saved successfully";  
                        Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                    } else { 
                        response.Status = true;
                        response.StatusCode = (int)ResponseCodes.FAILED;
                        response.Message = "Failed to capture error to database. An error occurrred";  
                        Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                    }

                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Oops! Something went wrong",
                        $"System Error - {ex.Message}"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AdminCountResponse>(error));
                }
            }
        
        [HttpPost("sam/users/getworkspace")]
        public async Task<IActionResult> Workspace([FromBody] UserRequest request) {
                try {
                    Logger.LogActivity($"{request.Action}", "INFO");

                    if (request == null) {
                        var error = new ResponseError(
                            ResponseCodes.BADREQUEST,
                            "Request record cannot be empty",
                            "Invalid request body"
                        );
                        Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<WorkspaceResponse>(error));
                    }

                    Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                    var workspace = await _accessService.GetWorkspaceAsync(request.RecordId, request.IPAddress);
                    workspace ??= new WorkspaceResponse() {
                        CurrentUser = null,
                        Permissions = new List<string>(),
                        UserViews = new List<UserViewResponse>()
                    };

                    if(workspace.CurrentUser != null){ 
                        string[] decrypt = new string[] { "PersonnelFileNumber", "Email", "FirstName", "LastName" };
                        workspace.CurrentUser = Cypher.DecryptProperties(workspace.CurrentUser, decrypt);
                    }

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(workspace)}");
                    return Ok(new GrcResponse<WorkspaceResponse>(workspace));
                } catch (Exception ex) {
                    Logger.LogActivity($"{ex.Message}", "ERROR");
                    Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                    
                    var conpany = await CompanyService.GetDefaultCompanyAsync();
                    long companyId = conpany != null ? conpany.Id : 1;
                    SystemError errorObj = new(){ 
                        ErrorMessage = ex.Message,
                        ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                        StackTrace = ex.StackTrace,
                        Severity = "CRITICAL",
                        ReportedOn = DateTime.Now,
                        CompanyId = companyId
                    };

                    //..save error object to the database
                    var result = await SystemErrorService.SaveErrorAsync(errorObj);
                    var response = new GeneralResponse();
                    if(result){
                        response.Status = true;
                        response.StatusCode = (int)ResponseCodes.SUCCESS;
                        response.Message = "Error captured and saved successfully";  
                        Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                    } else { 
                        response.Status = true;
                        response.StatusCode = (int)ResponseCodes.FAILED;
                        response.Message = "Failed to capture error to database. An error occurrred";  
                        Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                    }

                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Oops! Something went wrong",
                        $"System Error - {ex.Message}"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<WorkspaceResponse>(error));
                }
            }

        #endregion

        #region User Records

        [HttpPost("sam/users/getById")]
        public async Task<IActionResult> GetUserByIdAsync([FromBody] UserRequest request) {
            try {
                Logger.LogActivity("Get user by ID", "INFO");

                if (request == null){
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }

                if (request.RecordId == 0){
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request User ID is required",
                        "Invalid request User ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = await _accessService.GetByIdAsync(request.RecordId);
                if (response != null) {
                    //..map response
                    var userRecord = Mapper.Map<UserResponse>(response);
                    request.DecryptFields = new string[] { "FirstName", "LastName", "MiddleName", "EmailAddress", "PhoneNumber", "PFNumber" };
                    userRecord = Cypher.DecryptProperties(userRecord, request.DecryptFields);

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<UserResponse>(userRecord));
                } else {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "User not found",
                        "No user matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                    var conpany = await CompanyService.GetDefaultCompanyAsync();
                    long companyId = conpany != null ? conpany.Id : 1;
                    SystemError errorObj = new(){ 
                        ErrorMessage = ex.Message,
                        ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                        StackTrace = ex.StackTrace,
                        Severity = "CRITICAL",
                        ReportedOn = DateTime.Now,
                        CompanyId = companyId
                    };

                    //..save error object to the database
                    var result = await SystemErrorService.SaveErrorAsync(errorObj);
                    var response = new GeneralResponse();
                    if(result){
                        response.Status = true;
                        response.StatusCode = (int)ResponseCodes.SUCCESS;
                        response.Message = "Error captured and saved successfully";  
                        Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                    } else { 
                        response.Status = true;
                        response.StatusCode = (int)ResponseCodes.FAILED;
                        response.Message = "Failed to capture error to database. An error occurrred";  
                        Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                    }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<UserResponse>(error));
            }
        }

        [HttpPost("sam/users/getByUsername")]
        public async Task<IActionResult> GetByUsername([FromBody] UserRequest request) {
            try {
                Logger.LogActivity("Get user by Username", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.Username)) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request username is required",
                        "Invalid request username"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = await _accessService.GetUserByUsernameAsync(request.Username);
                if (response != null) {
                    //..map response
                    var userRecord = Mapper.Map<UserResponse>(response);
                    request.DecryptFields = new string[] { "FirstName", "LastName", "MiddleName", "EmailAddress", "PhoneNumber", "PFNumber" };
                    userRecord = Cypher.DecryptProperties(userRecord, request.DecryptFields);

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<UserResponse>(userRecord));
                } else {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "User not found",
                        "No user matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<UserResponse>(error));
            }
        }

        [HttpPost("sam/users/getByEmail")]
        public async Task<IActionResult> GetByEmail([FromBody] UserRequest request) {
            try {
                Logger.LogActivity("Get user by Email", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }
                
                if (string.IsNullOrWhiteSpace(request.Email)) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request Email Address is required",
                        "Invalid request Email Address"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = await _accessService.GetUserByEmailAsync(request.Email);
                if (response != null) {
                    //..map response
                    var userRecord = Mapper.Map<UserResponse>(response);
                    request.DecryptFields = new string[] { "FirstName", "LastName", "MiddleName", "EmailAddress", "PhoneNumber", "PFNumber" };
                    userRecord = Cypher.DecryptProperties(userRecord, request.DecryptFields);

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<UserResponse>(userRecord));
                } else {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "User not found",
                        "No user matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<UserResponse>(error));
            }
        }
        
        [HttpPost("sam/users/countUsers")]
        public async Task<IActionResult> CountUsers([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RecordCountResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var count = await _accessService.GetTotalUsersCountAsync();
                //..map response
                var countResponse = new RecordCountResponse() {
                    Count = count
                };
                
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(countResponse)}");
                return Ok(new GrcResponse<RecordCountResponse>(countResponse));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<RecordCountResponse>(error));
            }
        }

        [HttpPost("sam/users/countActiveUsers")]
        public async Task<IActionResult> CountActiveUsers([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RecordCountResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var count = await _accessService.GetActiveUsersCountAsync();
                //..map response
                var countResponse = new RecordCountResponse() {
                    Count = count
                };
                
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(countResponse)}");
                return Ok(new GrcResponse<RecordCountResponse>(countResponse));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<RecordCountResponse>(error));
            }
        }
        
        [HttpPost("sam/users/getQuickActions")]
        public async Task<IActionResult> GetQuickActions([FromBody] UserRequest request) {
            try {
                Logger.LogActivity("Get user Quick Action menu", "INFO");

                if (request == null){
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<IList<QuickActionResponse>>(error));
                }

                if (request.RecordId == 0){
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request User ID is required",
                        "Invalid request User ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<IList<QuickActionResponse>>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var actions = await _quickActionService.GetUserQuickActionAsync(request.RecordId);
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(actions)}");
                return Ok(new GrcResponse<IList<QuickActionResponse>>(actions));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<IList<QuickActionResponse>>(error));
            }
        }
        
        [HttpPost("sam/users/getPinnedItems")]
        public async Task<IActionResult> GetPinnedItems([FromBody] UserRequest request) {
            try {
                Logger.LogActivity("Get user Pinned item menu", "INFO");

                if (request == null){
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<IList<PinnedItemResponse>>(error));
                }

                if (request.RecordId == 0){
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request User ID is required",
                        "Invalid request User ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<IList<PinnedItemResponse>>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var actions = await _pinnedItemService.GetUserPinnedItemsAsync(request.RecordId);
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(actions)}");
                return Ok(new GrcResponse<IList<PinnedItemResponse>>(actions));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<IList<PinnedItemResponse>>(error));
            }
        }

        [HttpPost("sam/users/createuser")]
        public async Task<IActionResult> CreateUser([FromBody] UserRecordRequest request) {
            try {
                Logger.LogActivity("Creating new user record", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The user record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UserResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                //..get activity type
                SystemUser user = null;
                if (!string.IsNullOrWhiteSpace(request.UserName)) {
                    user = await _accessService.GetUserByUsernameAsync(request.UserName);
                    if (user != null)
                    {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another user found with same username"
                        );

                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<UserResponse>(error));
                    }
                }

                //..create company
                var userRecord = Mapper.Map<SystemUser>(request);

                //..hash the password
                userRecord.PasswordHash = ExtendedHashMapper.HashPassword(userRecord.PasswordHash);
                //..encrypt fields
                Cypher.EncryptProperties(userRecord, request.EncryptFields);

                //..create company
                var result = await _accessService.InsertUserAsync(userRecord);

                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "User saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save user record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("sam/users/updateuser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserRecordRequest request)
        {
            try
            {
                Logger.LogActivity("Update system role", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The user record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _accessService.UserExistsAsync(r => r.Id == request.Id))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "User record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }

                //..encrypt fields
                Cypher.EncryptProperties(request, request.EncryptFields);

                //..update role
                var result = await _accessService.UpdateUserAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "User record updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update user record record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("sam/users/deleteuser")]
        public async Task<IActionResult> DeleteUser([FromBody] IdRequest request)
        {
            try
            {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _accessService.UserExistsAsync(r => r.Id == request.RecordId))
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No User record found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete system user
                var status = await _accessService.DeleteRoleAsync(request);
                if (!status)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete system user",
                        "An error occurred! could delete system user");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting system user by user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while deleting unit.");
            }
        }

        [HttpPost("sam/users/getusers")]
        public async Task<IActionResult> GetPagedUsers([FromBody] GeneralRequest request) {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<UserResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var result = await _accessService.GetAllUsersAsync();

                if (result == null || !result.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No user records found"
                    );

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ListResponse<UserResponse>>(error));
                }

                //..decrypt fields
                var fieldsToDecrypt = new[] { "FirstName", "LastName", "MiddleName", "EmailAddress", "PhoneNumber", "PFNumber" };

                //..decrypt in parallel for better performance with large datasets
                var decryptedUsers = result
                    .AsParallel()
                    .AsOrdered()
                    .Select(user => Cypher.DecryptProperties(user, fieldsToDecrypt))
                    .ToList();

                //..map to UserResponse
                var users = decryptedUsers.Select(Mapper.Map<UserResponse>).ToList();

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: Retrieved {users.Count} users (IDs: {string.Join(", ", users.Select(u => u.Id))})");
                return Ok(new GrcResponse<ListResponse<UserResponse>>(new ListResponse<UserResponse>() {
                    Data = users
                }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<UserResponse>>(error));
            }
        }

        [HttpPost("sam/users/pagedusers")]
        public async Task<IActionResult> GetPagedUsers([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<UserResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _accessService.PagedUsersAsync(request.PageIndex, request.PageSize, true);

                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No user records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<UserResponse>>(new PagedResponse<UserResponse>(
                        new List<UserResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                // Map to UserResponse (removed unnecessary null! and AsQueryable)
                var users = pageResult.Entities.Select(Mapper.Map<UserResponse>).ToList();

                // Decrypt fields - define once outside loop
                var fieldsToDecrypt = new[] { "FirstName", "LastName", "MiddleName", "EmailAddress", "PhoneNumber", "PFNumber" };

                // Decrypt in parallel for better performance with large datasets
                var decryptedUsers = users
                    .AsParallel()
                    .AsOrdered()
                    .Select(user => Cypher.DecryptProperties(user, fieldsToDecrypt))
                    .ToList();

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();

                    decryptedUsers = decryptedUsers.Where(u =>
                        (u.Username?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.FirstName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.MiddleName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.LastName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.DepartmentCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.DepartmentName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.EmailAddress?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.PFNumber?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.RoleName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(decryptedUsers)}");
                return Ok(new GrcResponse<PagedResponse<UserResponse>>(new PagedResponse<UserResponse>(
                    decryptedUsers,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<UserResponse>>(error));
            }
        }

        #endregion

        #region System Roles

        [HttpPost("sam/roles/getrole-by-id")]
        public async Task<IActionResult> GetRoleById([FromBody] IdRequest request) {
            try
            {
                Logger.LogActivity("Get role by ID", "INFO");

                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }

                if (request.RecordId == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request Role ID is required",
                        "Invalid request Role ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = await _accessService.GetRoleByIdAsync(request);
                if (response != null)
                {
                    //..map response
                    var roleRecord = Mapper.Map<RoleResponse>(response);
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<RoleResponse>(roleRecord));
                }
                else
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Role not found",
                        "No role matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<UserResponse>(error));
            }
        }

        [HttpPost("sam/roles/getroles")]
        public async Task<IActionResult> GetPagedRoles([FromBody] GeneralRequest request)
        {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<RoleResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var result = await _accessService.GetAllRolesAsync();

                if (result == null || !result.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No system role records found"
                    );

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ListResponse<RoleResponse>>(error));
                }


                //..map to RoleResponse
                var roles = result.Select(Mapper.Map<RoleResponse>).ToList();

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: Retrieved {roles.Count} roles (IDs: {string.Join(", ", roles.Select(u => u.Id))})");
                return Ok(new GrcResponse<ListResponse<RoleResponse>>(new ListResponse<RoleResponse>()
                {
                    Data = roles
                }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<RoleResponse>>(error));
            }
        }

        [HttpPost("sam/roles/pagedroles")]
        public async Task<IActionResult> GetPagedRoles([FromBody] ListRequest request)
        {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<RoleResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _accessService.PagedRolesAsync(request.PageIndex, request.PageSize, true);

                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No system role records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<RoleResponse>>(new PagedResponse<RoleResponse>(
                        new List<RoleResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                var roles = pageResult.Entities.Select(Mapper.Map<RoleResponse>).ToList();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();

                    roles = roles.Where(u =>
                        (u.RoleName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(roles)}");
                return Ok(new GrcResponse<PagedResponse<RoleResponse>>(new PagedResponse<RoleResponse>(
                    roles,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<RoleResponse>>(error));
            }
        }

        [HttpPost("sam/roles/createrole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequest request)
        {
            try
            {
                Logger.LogActivity("Creating new system role", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The system role record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                if (!string.IsNullOrWhiteSpace(request.RoleName))
                {
                    if (await _accessService.RoleExistsAsync(r => r.RoleName == request.RoleName))
                    {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another Role found with same role name"
                        );

                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                }

                //..create role
                var result = await _accessService.InsertRoleAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Role saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save system role record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("sam/roles/updaterole")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleRequest request)
        {
            try
            {
                Logger.LogActivity("Update system role", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The system role record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _accessService.RoleExistsAsync(r => r.Id == request.Id))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "System Role record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..update role
                var result = await _accessService.UpdateRoleAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "System Role updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update system role record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("sam/roles/deleterole")]
        public async Task<IActionResult> DeleteRole([FromBody] IdRequest request) {
            try
            {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _accessService.RoleExistsAsync(r => r.Id == request.RecordId))
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No Role record found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete system role
                var status = await _accessService.DeleteRoleAsync(request);
                if (!status)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete system role",
                        "An error occurred! could delete system role");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting system role by user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while deleting unit.");
            }
        }

        #endregion

        #region System Role Groups

        [HttpPost("sam/roles/getrolegroup-by-id")]
        public async Task<IActionResult> GetRoleGroupById([FromBody] IdRequest request)
        {
            try
            {
                Logger.LogActivity("Get role group by ID", "INFO");

                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleGroupResponse>(error));
                }

                if (request.RecordId == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request Role Group ID is required",
                        "Invalid request Role Group ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleGroupResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = await _accessService.GetRoleGroupByIdAsync(request);
                if (response != null)
                {
                    //..map response
                    var roleRecord = Mapper.Map<RoleGroupResponse>(response);
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<RoleGroupResponse>(roleRecord));
                }
                else
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Role Group not found",
                        "No role Group matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleGroupResponse>(error));
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<RoleGroupResponse>(error));
            }
        }

        [HttpPost("sam/roles/getrolegroups")]
        public async Task<IActionResult> GetPagedRoleGroups([FromBody] GeneralRequest request)
        {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<RoleGroupResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var result = await _accessService.GetAllRoleGroupsAsync();

                if (result == null || !result.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No Role group records found"
                    );

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ListResponse<RoleGroupResponse>>(error));
                }

                //..map to RoleGroupResponse
                var groups = result.Select(Mapper.Map<RoleGroupResponse>).ToList();

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: Retrieved {groups.Count} role groups (IDs: {string.Join(", ", groups.Select(u => u.Id))})");
                return Ok(new GrcResponse<ListResponse<RoleGroupResponse>>(new ListResponse<RoleGroupResponse>()
                {
                    Data = groups
                }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<RoleGroupResponse>>(error));
            }
        }

        [HttpPost("sam/roles/pagedrolegroups")]
        public async Task<IActionResult> GetPagedRoleGroups([FromBody] ListRequest request)
        {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<RoleGroupResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _accessService.PagedRoleGroupsAsync(request.PageIndex, request.PageSize, true);

                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No role groups records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<RoleGroupResponse>>(new PagedResponse<RoleGroupResponse>(
                        new List<RoleGroupResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                var groups = pageResult.Entities.Select(Mapper.Map<RoleGroupResponse>).ToList();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    groups = groups.Where(u =>
                        (u.GroupName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<RoleGroupResponse>>(new PagedResponse<RoleGroupResponse>(
                    groups,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<RoleGroupResponse>>(error));
            }
        }

        [HttpPost("sam/roles/createrolegroup")]
        public async Task<IActionResult> CreateRoleGroup([FromBody] RoleGroupRequest request)
        {
            try
            {
                Logger.LogActivity("Creating new role group", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The role group record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.GroupName))
                {
                    if (await _accessService.RoleGroupExistsAsync(r => r.GroupName == request.GroupName))
                    {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another Role Group found with same group name"
                        );

                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                }

                //..create role group
                var result = await _accessService.InsertRoleGroupAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Role Group saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save role group record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("sam/roles/updaterolegroup")]
        public async Task<IActionResult> UpdateRoleGroup([FromBody] RoleGroupRequest request)
        {
            try
            {
                Logger.LogActivity("Update role group", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The role group record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _accessService.RoleExistsAsync(r => r.Id == request.Id))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "Role Group record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..update role group
                var result = await _accessService.UpdateRoleGroupAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Role Group updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update role group record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("sam/roles/deleterolegroup")]
        public async Task<IActionResult> DeleteRoleGroup([FromBody] IdRequest request)
        {
            try
            {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _accessService.RoleExistsAsync(r => r.Id == request.RecordId))
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No Role record found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete system role
                var status = await _accessService.DeleteRoleAsync(request);
                if (!status)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete system role",
                        "An error occurred! could delete system role");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting system role by user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while deleting unit.");
            }
        }

        #endregion

        #region System Permissions

        [HttpPost("sam/permissions/get-permissions")]
        public async Task<IActionResult> GetAllPermissions([FromBody] GeneralRequest request)
        {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var permissions = await _accessService.GetAllPermissionsAsync();

                if (permissions == null || !permissions.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No System Permissions found"
                    );

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ListResponse<PermissionResponse>>(error));
                }

                //..map to PermissionResponse
                var permissionData = permissions.Select(Mapper.Map<PermissionResponse>).ToList();

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: Retrieved {permissionData.Count} system permissions (IDs: {string.Join(", ", permissionData.Select(u => u.Id))})");
                return Ok(new GrcResponse<ListResponse<PermissionResponse>>(new ListResponse<PermissionResponse>()
                {
                    Data = permissionData
                }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<PermissionResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/paged-permissions")]
        public async Task<IActionResult> GetPagedPermissions([FromBody] ListRequest request)
        {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<PermissionResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _accessService.PagedPermissionsAsync(request.PageIndex, request.PageSize, true);

                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No system permissions found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<PermissionResponse>>(new PagedResponse<PermissionResponse>(
                        new List<PermissionResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                var groups = pageResult.Entities.Select(Mapper.Map<PermissionResponse>).ToList();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    groups = groups.Where(u =>
                        (u.PermissionName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.PermissionDescription?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(groups)}");
                return Ok(new GrcResponse<PagedResponse<PermissionResponse>>(new PagedResponse<PermissionResponse>(
                    groups,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<PermissionResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/getrole-permissions")]
        public async Task<IActionResult> GetRolePermissions([FromBody] RolePermissionRequest request) {
            try
            {
                Logger.LogActivity("Get role permissions", "INFO");

                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionResponse>>(error));
                }

                if (request.RoleId == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request Role ID is required",
                        "Invalid request Role ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionResponse>>(error));
                }

                //..check if role exist
                if (!await _accessService.RoleExistsAsync(r => r.Id == request.RoleId))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "System Role record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionResponse>>(error));
                }

                //..log request
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                //..get permissions
                var data = await _accessService.GetRolePermissionsAsync(request);
                if (data != null)
                {
                    //..map response
                    var permissions = data.Select(Mapper.Map<PermissionResponse>).ToList();
                    return Ok(new GrcResponse<List<PermissionResponse>>(permissions));
                }
                else
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Role not found",
                        "No role matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionResponse>>(error));
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<List<PermissionResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/updaterole-permissions")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] RolePermissionRequest request) {
            try
            {
                Logger.LogActivity("Get role permissions", "INFO");

                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (request.RoleId == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request Role ID is required",
                        "Invalid request Role ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..check if role exist
                if (!await _accessService.RoleExistsAsync(r => r.Id == request.RoleId))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "System Role record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (request.Permissions == null || request.Permissions.Count == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "No Role Permissions",
                        "List of role permissions is empty"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..log request
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                //..get permissions
                var ids = request.Permissions.Select(p => p.Id).ToList();
                var result = await _accessService.UpdateRolePermissionSetsAsync(request.RoleId, ids);

                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Role permissions updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update role permissions. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<List<PermissionResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/getrole-group-permissions")]
        public async Task<IActionResult> GetRoleGroupPermissions([FromBody] RoleGroupPermissionRequest request) {
            try {
                Logger.LogActivity("Get role group permission sets", "INFO");

                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionSetResponse>>(error));
                }

                if (request.RoleGroupId == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request Role Group ID is required",
                        "Invalid request Role Group ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionSetResponse>>(error));
                }

                //..check if role group exist
                if (!await _accessService.RoleGroupExistsAsync(r => r.Id == request.RoleGroupId))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "System Role Group record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionSetResponse>>(error));
                }

                //..log request
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                //..get permissions
                var data = await _accessService.GetRoleGroupPermissionSetsAsync(request.RoleGroupId);
                if (data != null)
                {
                    //..map response
                    var permissions = data.Select(Mapper.Map<PermissionSetResponse>).ToList();
                    return Ok(new GrcResponse<List<PermissionSetResponse>>(permissions));
                }
                else
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Not Found",
                        "Not Role Group permissions found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionSetResponse>>(error));
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<List<PermissionSetResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/updaterole-group-permissions")]
        public async Task<IActionResult> UpdateRoleGroupPermissions([FromBody] RoleGroupPermissionRequest request) {
            try {
                Logger.LogActivity("Get role group permissions", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (request.RoleGroupId == 0) {
                    var error = new ResponseError (
                        ResponseCodes.BADREQUEST,
                        "Request Role  Group ID is required",
                        "Invalid request Role Group ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..check if role group exist
                if (!await _accessService.RoleGroupExistsAsync(r => r.Id == request.RoleGroupId)) {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "System Role Group not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (request.PermissionSets == null || request.PermissionSets.Count == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "No Role Permission Sets",
                        "List of role permission sets is empty"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..log request
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                //..get permissions
                var ids = request.PermissionSets.Select(p => p.Id).ToList();
                var result = await _accessService.UpdateRolePermissionSetsAsync(request.RoleGroupId, ids);

                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Role Group permission sets updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update role permission sets. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new() {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<List<PermissionResponse>>(error));
            }
        }

        #endregion

        #region Permission Sets

        [HttpPost("sam/permissions/retrieve-permission-set")]
        public async Task<IActionResult> GetPermissionSetId([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get permission set by ID", "INFO");

                if (request == null || request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Invalid request",
                        "Permission Set ID is required"
                    );
                    return Ok(new GrcResponse<PermissionSetResponse>(error));
                }

                //...get the permission set with assigned permissions
                var set = await _accessService.GetPermissionSetAsync(request);
                if (set == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Permission Set not found",
                        "No permission set matched the provided ID"
                    );
                    return Ok(new GrcResponse<PermissionSetResponse>(error));
                }

                //..get all system permissions
                var permissionData = await _accessService.GetAllPermissionsAsync();
                var permissionList = permissionData ?? new List<SystemPermission>();

                //..map assigned permissions for easy lookup
                var assignedIds = set.Permissions.Select(p => p.PermissionId).ToHashSet();

                //merge mark isAssigned = true if in assignedIds
                var mergedPermissions = permissionList
                    .Select(p => new PermissionResponse
                    {
                        Id = p.Id,
                        PermissionName = p.PermissionName,
                        PermissionDescription = p.Description,
                        IsAssigned = assignedIds.Contains(p.Id)
                    })
                    .OrderBy(p => p.PermissionName)
                    .ToList();

                //..return full set data
                var setRecord = new PermissionSetResponse
                {
                    Id = set.Id,
                    SetName = set.SetName,
                    SetDescription = set.Description,
                    CreatedOn = set.CreatedOn,
                    CreatedBy = set.CreatedBy ?? string.Empty,
                    ModifiedOn = set.LastModifiedOn,
                    ModifiedBy = set.LastModifiedBy ?? string.Empty,
                    Permissions = mergedPermissions
                };

                return Ok(new GrcResponse<PermissionSetResponse>(setRecord));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<PermissionSetResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/get-permissionsets")]
        public async Task<IActionResult> GetPermissionSets([FromBody] GeneralRequest request)
        {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PermissionSetResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var permissionSets = await _accessService.GetAllPermissionSetsAsync();
                if (permissionSets == null || !permissionSets.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No System Permission sets found"
                    );

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ListResponse<PermissionSetResponse>>(error));
                }

                //..map to PermissionSetResponse
                var permissionData = permissionSets.Select(Mapper.Map<PermissionSetResponse>).ToList();

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: Retrieved {permissionData.Count} system permissions (IDs: {string.Join(", ", permissionData.Select(u => u.Id))})");
                return Ok(new GrcResponse<ListResponse<PermissionSetResponse>>(new ListResponse<PermissionSetResponse>()
                {
                    Data = permissionData
                }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<PermissionSetResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/pagedsets")]
        public async Task<IActionResult> GetPagedPermissionSets([FromBody] ListRequest request)
        {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<PermissionSetResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _accessService.PagedPermissionSetAsync(request.PageIndex, request.PageSize, true);

                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No permission set records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<PermissionSetResponse>>(new PagedResponse<PermissionSetResponse>(
                        new List<PermissionSetResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                var permissionData = pageResult.Entities;
                var sets = permissionData.Select(Mapper.Map<PermissionSetResponse>).ToList();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    sets = sets.Where(u =>
                        (u.SetName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.SetDescription?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<PermissionSetResponse>>(new PagedResponse<PermissionSetResponse>(
                    sets,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<RoleGroupResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/create-permissionset")]
        public async Task<IActionResult> CreatePermissionSet([FromBody] PermissionSetRequest request) {
            try
            {
                Logger.LogActivity("Creating new permission set", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The permission set record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.SetName)) {
                    if (await _accessService.PermissionSetExistsAsync(r => r.SetName == request.SetName || r.Description == request.Description)) { 
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another Permission Set found with same setName or Description"
                        );

                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                }

                //..create permission set
                var result = await _accessService.InsertPermissionSetAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Permission Set saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save permission set record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("sam/permissions/update-permissionsets")]
        public async Task<IActionResult> UpdatePermissionSets([FromBody] PermissionSetRequest request)
        {
            try
            {
                Logger.LogActivity("Get permission sets", "INFO");

                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (request.Id == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request Permission Set ID is required",
                        "Invalid request Permission Set ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..check if permission set exist
                if (!await _accessService.PermissionSetExistsAsync(r => r.Id == request.Id))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "System Role record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..check for duplicate name or description
                if (await _accessService.PermissionSetExistsAsync(r => r.Id != request.Id && (r.SetName == request.SetName || r.Description == request.Description) ))
                {
                    var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another Permission Set found with same setName or Set Description"
                        );

                    Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (request.Permissions == null || request.Permissions.Count == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "No Permission sets",
                        "List of permission sets is empty"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..log request
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _accessService.UpdatePermissionSetAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Permission sets updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update permission sets. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<List<PermissionResponse>>(error));
            }
        }

        [HttpPost("sam/permissions/delete-permissionset")]
        public async Task<IActionResult> DeletePermissionSet([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _accessService.PermissionSetExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found! No permission set record found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete permission set
                var status = await _accessService.DeletePermissionSetAsync(request);
                if (!status)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete permission set",
                        "An error occurred! could delete permission set");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting permission set by user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while deleting unit.");
            }
        }

        #endregion

        #region System Activities

        [HttpPost("sam/users/getactivity")]
        public async Task<IActionResult> GetSystemActivityId([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get system activity by ID", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<SystemActivityResponse>(error));
                }

                if (request.RecordId == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request System Activity ID is required",
                        "Invalid request System Activity ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<SystemActivityResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = await _accessService.GetActivityLogAsync(request);
                if (response != null)
                {
                    //..map response
                    var activity = Mapper.Map<SystemActivityResponse>(response);
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<SystemActivityResponse>(activity));
                }
                else
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Record not found",
                        "No System Activity matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<SystemActivityResponse>(error));
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-SAM-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE-SAM-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<SystemActivityResponse>(error));
            }
        }

        [HttpPost("sam/users/getactivities")]
        public async Task<IActionResult> GetPagedActivities([FromBody] ListRequest request) {
            try
            {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<SystemActivityResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _accessService.GetPagedActivityLogAsync(request.PageIndex, request.PageSize, true);

                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No System Activity records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<SystemActivityResponse>>(new PagedResponse<SystemActivityResponse>(
                        new List<SystemActivityResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                //..fields to decrypt
                var fieldsToDecrypt = new[] { "FirstName", "LastName", "MiddleName" };

                //..decrypt user info
                var decryptedUserIds = new ConcurrentDictionary<long, bool>(); // thread-safe
                Parallel.ForEach(pageResult.Entities, activity =>
                {
                    var user = activity.User;
                    if (user != null && decryptedUserIds.TryAdd(user.Id, true))
                    {
                        activity.User = Cypher.DecryptProperties(user, fieldsToDecrypt);
                    }
                });

                //..filter before mapping
                IEnumerable<ActivityLog> filteredEntities = pageResult.Entities;

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();

                    filteredEntities = filteredEntities.Where(a =>
                        (a.Comment?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (a.EntityName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    );
                }

                //..map to response DTOs
                var activities = filteredEntities
                    .Select(Mapper.Map<SystemActivityResponse>)
                    .ToList();

                return Ok(new GrcResponse<PagedResponse<SystemActivityResponse>>(new PagedResponse<SystemActivityResponse>(
                    activities,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<SystemActivityResponse>>(error));
            }
        }

        #endregion
    }
}

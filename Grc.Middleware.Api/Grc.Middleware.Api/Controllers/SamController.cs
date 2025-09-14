﻿using AutoMapper;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
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

                var response = await _accessService.GetByUsernameAsync(request.Username);
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

                var response = await _accessService.GetByEmailAsync(request.Email);
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

        #endregion

    }
}

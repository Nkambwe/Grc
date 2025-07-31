using AutoMapper;
using Azure.Core;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Grc.Middleware.Api.Controllers {


    [ApiController]
    [Route("grc")]
    public class SamController : GrcControllerBase {
        private readonly ISystemAccessService _accessService;
        public SamController(IObjectCypher cypher, 
                            IServiceLoggerFactory loggerFactory, 
                            IMapper mapper, 
                            IEnvironmentProvider environment,
                            ISystemAccessService accessService) 
                            : base(cypher, loggerFactory, mapper, environment) {
            _accessService = accessService;
        }

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
                    
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<UsernameValidationResponse>(response));
                } else { 
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        $"Oops! Something thing went wrong",
                        "Failed to validate username. An error occurrred"
                    ); 
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<UsernameValidationResponse>(error));
                }
            } catch (Exception ex) { 
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );
        
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
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
        
                var error = new ResponseError(
                    ResponseCodes.SERVERERROR,
                    "Authentication service temporarily unavailable",
                    $"System Error - {ex.Message}"
                );

                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<AuthenticationResponse>(error));
            }
        }

        //[HttpGet("sam/users/signin")]
        //public async Task<IActionResult> Signin([FromBody] LoginRequest loginRequest) {

        //}
        
        [Authorize]
        [HttpGet("sam/users/current-user")]
        public async Task<IActionResult> GetCurrentUser() {
            try {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.Identity.Name;

                if (string.IsNullOrWhiteSpace(userId)) { 
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Current user record not found",
                        "User session might be invalidated"
                    );
        
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                 Logger.LogActivity($"User Claims :: User ID >{userId}, Email {email}", "INFO");
                if(!long.TryParse(userId, out long currentId)) { 
                    currentId = 0;
                }
                SystemUser user = await _accessService.GetByIdAsync(currentId);
                if(user == null) { 
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Current user record not found",
                        "User session might be invalidated"
                    );
        
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                
                //..decrypt user record
                Cypher.EncryptProperties(user, ExtendedHashMapper.GetEncryptedUserFields());
                
                //..map user record to response
                var record = Mapper.Map<AuthenticationResponse>(user);
                record.SolId = user.BranchSolId;
                record.RoleId = user.RoleId;
                record.DepartmentId = user.DepartmentId;
                record.Favourites = new();
                record.Views = new();

                return Ok(new GrcResponse<AuthenticationResponse>(record));
            } catch (Exception ex) { 
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );
        
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<AuthenticationResponse>(error));
            }
            
        }

    }
}

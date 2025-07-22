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
                var record = Mapper.Map<UserResponse>(user);
                record.SolId = user.BranchSolId;
                record.RoleId = user.RoleId;
                record.DepartmentId = user.DepartmentId;
                record.Favourites = new();
                record.Views = new();

                return Ok(new GrcResponse<UserResponse>(record));
            } catch (Exception ex) { 
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );
        
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<UserResponse>(error));
            }
            
        }

        //[HttpGet("sam/users/signin")]
        //public async Task<IActionResult> Signin([FromBody] LoginRequest loginRequest) {

        //}

    }
}

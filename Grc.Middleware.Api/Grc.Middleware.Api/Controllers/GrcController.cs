using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Factories;
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
    public class GrcController : GrcControllerBase {

        private readonly ICompanyService _companyService;

        public GrcController(IObjectCypher cypher,
            IServiceLoggerFactory loggerFactory, 
                             IEnvironmentProvider environment,
                             ICompanyService companyService,
                             IMapper mapper) 
            : base(cypher, loggerFactory, mapper, environment) {
            _companyService = companyService;
        }

        [HttpGet]
        public IActionResult Index() { 
            return Ok("Welcome to GRC Suite! ");    
        }

        [HttpPost("registration/register")]
        [Produces("application/json")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request) {

            try {
                Logger.LogActivity("Process company record for persistance", "INFO");
                if (request == null) { 
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The company registration model cannot be null"
                    );
        
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                //..create company
                var company = Mapper.Map<Company>(request);
                var admin = Mapper.Map<SystemUser>(request);
                //..hash the password
                admin.PasswordHash = ExtendedHashMapper.HashPassword(admin.PasswordHash);
                //..encrypt fields
                Cypher.EncryptProperties(admin, request.EncryptFields);

                //..create branch
                List<Branch> branches = new() { new BranchFactory().CreateMainBranch(admin) };
                company.Branches = branches;
                //..create company
                var result = await _companyService.CreateCompanyAsync(company);

                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Registration completed successfully";    
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to complete regiatration. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) { 
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
    }
}

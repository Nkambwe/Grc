using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class GrcController : GrcControllerBase {

        private readonly ICompanyService _companyService;

        public GrcController(IServiceLoggerFactory loggerFactory, 
                             IEnvironmentProvider environment,
                             ICompanyService companyService) 
            : base(loggerFactory, environment) {
            _companyService = companyService;
        }

        [HttpGet]
        public IActionResult Index() { 
            return Ok("Welcome to GRC Suite! ");    
        }

        [HttpPost("registration/register")]
        [Produces("application/json")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request) { 
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

            var company = new Company();
            var result = await Task.FromResult(company); //_companyService.CreateCompanyAsync(company);

            var response = new GeneralResponse(){ 
                Status = true,
                StatusCode = (int)ResponseCodes.SUCCESS,
                Message = "Registration completed successfully"    
            };
        
            Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
            return Ok(response);    
        }
    }
}

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
        
        public GrcController(IObjectCypher cypher,
            IServiceLoggerFactory loggerFactory, 
                             IEnvironmentProvider environment,
                             ICompanyService companyService,
                             IMapper mapper,
                             IErrorNotificationService errorService,
                             ISystemErrorService systemErrorService) 
            : base(cypher, loggerFactory, mapper, companyService, environment, 
                  errorService, systemErrorService) {
        }

        [HttpGet]
        public IActionResult Index() { 
            return Ok("Welcome to GRC Suite! ");    
        }

        [HttpPost("errors/saveerror")] 
        public async Task<IActionResult> SaveError([FromBody] ErrorRequest request) {
            try{
                Logger.LogActivity("Saving error to the database", "INFO");
                if (request == null) { 
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid error object"
                    );
        
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                //..create system error object
                var errorObj = Mapper.Map<SystemError>(request);
                errorObj.Severity = DetermineSeverity(request.Message);

                //..send real-time notification
                await ErrorService.NotifyNewErrorAsync(errorObj);

                //..get updated counts and notify
                var errorCounts = await SystemErrorService.GetErrorCountsAsync(request.CompanyId);
                await ErrorService.NotifyCountAsync(
                    errorCounts.TotalBugs, 
                    errorCounts.NewBugs
                );

                //..save error object to the database
                var result = await SystemErrorService.SaveErrorAsync(errorObj);
                var response = new GeneralResponse();
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                Logger.LogActivity($"ERROR CAPTURE: {JsonSerializer.Serialize(response)}");
                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) { 
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-GRC-COTROLLER",
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
                    Logger.LogActivity($"GRC-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );
        
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }    
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
                var result = await CompanyService.CreateCompanyAsync(company);

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

                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "MIDDLEWARE-GRC-COTROLLER",
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
                    Logger.LogActivity($"GRC-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - {ex.Message}"
                );
        
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }    
        }

        private string DetermineSeverity(string message) {
            if (message.Contains("Critical") || message.Contains("Fatal"))
                return "CRITICAL";
            if (message.Contains("Warning"))
                return "WARNING";
            return "ERROR";
        }
    }
}

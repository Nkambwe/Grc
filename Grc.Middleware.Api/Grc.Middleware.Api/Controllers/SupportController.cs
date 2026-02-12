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
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class SupportController : GrcControllerBase {

        private readonly IActivityLogService _activityLogService;
        private readonly IActivityTypeService _activityTypeService;
        private readonly IActivityLogSettingService _activitySettingService;
        private readonly IBranchService _branchService;
        private readonly IDepartmentsService _departmentsService;
        private readonly IDepartmentUnitService _departmentUnitService;
        private readonly ISystemAccessService _accessService;
        private readonly ISystemConfigurationService _configService;
        private readonly IBugService _bugService;
        public SupportController(
            IActivityLogService activityLogService,
            IActivityTypeService activityTypeService,
            IActivityLogSettingService activitySettingService,
            IDepartmentsService departmentsService,
            IDepartmentUnitService departmentUnitService,
            IObjectCypher cypher, 
            IServiceLoggerFactory loggerFactory, 
            IMapper mapper, 
            ICompanyService companyService,
            IEnvironmentProvider environment,
            IErrorNotificationService errorService, 
            ISystemErrorService systemErrorService,
            ISystemConfigurationService configService,
            ISystemAccessService accessService,
            IBugService bugService,
            IBranchService branchService)
            : base(cypher, loggerFactory, mapper, companyService, environment, 
                  errorService, systemErrorService) {
            _activityLogService = activityLogService;
            _activityTypeService = activityTypeService;
            _activitySettingService = activitySettingService;
            _departmentsService = departmentsService;
            _departmentUnitService = departmentUnitService;
            _branchService = branchService;
            _configService = configService;
            _accessService = accessService;
            _bugService = bugService;
        }

        #region Activity logging
        
        [HttpPost("activities/allActivities")]
        public async Task<IActionResult> AllActivities([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ActivityLogResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var pageResult = await _activityLogService.GetPagedActivitiesAsync(
                    ipAddress:request.IPAddress, 
                    pageIndex:request.PageIndex, 
                    pageSize:request.PageSize);

                //..map response
                List<ActivityLogResponse> data = new();
                if(pageResult.Entities != null && pageResult.Entities.Any()) { 
                    data = pageResult.Entities.Select(Mapper.Map<ActivityLogResponse>).ToList();

                    //..generate period
                    data = data.Select(entity => {
                                entity.Period = GetElapsedTime(entity.CreatedOn);
                                return entity;
                            }).ToList();
                }
                    
                //..decrypt user fields
                    
                if(data.Any()){
                    List<ActivityLogResponse> decryptedData = new();
                    request.DecryptFields = new string[] { "UserFirstName", "UserLastName", "UserEmail"};
                    foreach(var entity in data){ 
                        var decEntity = Cypher.DecryptProperties(entity, request.DecryptFields);
                        decryptedData.Add(decEntity);
                    }
                        
                    data = decryptedData.ToList();
                }

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(data)}");
                return Ok(new GrcResponse<PagedResponse<ActivityLogResponse>>(new PagedResponse<ActivityLogResponse>(
                            data,
                            pageResult.Count,
                            pageResult.Page,
                            pageResult.Size
                        )));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                    
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
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
                return Ok(new GrcResponse<PagedResponse<ActivityLogResponse>>(error));
            }
        }
        
        [HttpPost("activities/saveActivity")]
        public async Task<IActionResult> SaveActivity([FromBody] AcivityLogRequest request) {

            try {

                Logger.LogActivity("Process activity log record for persistance", "INFO");
                if (request == null) { 
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The activity record cannot be null"
                    );
        
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                //..get activity type
                ActivityType type = null;
                if (!string.IsNullOrWhiteSpace(request.Activity)) {
                    type = await _activityTypeService.GetActivityTypeByNameAsync(request.Activity);
                }

                if(type == null && !string.IsNullOrWhiteSpace(request.SystemKeyword)) { 
                    type = await _activityTypeService.GetActivityTypeBySystemKeywordAsync(request.SystemKeyword);
                }

                //..create company
                var result = await _activityLogService.InsertActivityAsync(type,request.Comment, request.UserId, request.IPAddress, request.EntityName);

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

        #endregion

        #region Branches

        [HttpPost("organization/branches-retrieve")]
        public async Task<IActionResult> GetBranch([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get System Error", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request Branch ID is required", "Invalid request Role ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var branch = await _branchService.GetByIdAsync(request.RecordId, true);
                if (branch != null) {
                    var bug = new BranchResponse() {
                        Id = branch.Id,
                        CompanyId = branch.CompanyId,
                        CompanyName = branch.Company != null ? branch.Company.CompanyName : string.Empty,
                        BranchName = branch.BranchName,
                        SolId = branch.SolId,
                        IsDeleted = branch.IsDeleted,
                        CreatedBy = branch.CreatedBy,
                        CreatedOn = branch.CreatedOn,
                        LastModifiedBy = branch.LastModifiedBy,
                        LastModifiedOn = branch.LastModifiedOn
                    };
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(bug)}");
                    return Ok(new GrcResponse<BranchResponse>(bug));
                } else {
                    var error = new ResponseError(ResponseCodes.FAILED, "System Error not found", "No System Error matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<BranchResponse>(error));
                }
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<BranchResponse>(error));
            }
        }

        [HttpPost("organization/branches-all")]
        public async Task<IActionResult> GetBranches([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Request record cannot be empty","Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<BranchResponse>>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var dataList = await _branchService.GetAllAsync();

                //..map response
                List<BranchResponse> data = new();
                if(dataList != null && dataList.Any()) { 
                    data = dataList.Select(b=> new BranchResponse() {
                        Id = b.Id,
                        CompanyId = b.CompanyId,
                        CompanyName = b.Company != null ? b.Company.CompanyName : string.Empty,
                        BranchName = b.BranchName,
                        SolId = b.SolId,
                        IsDeleted = b.IsDeleted,
                        CreatedBy = b.CreatedBy,
                        CreatedOn = b.CreatedOn,
                        LastModifiedBy = b.LastModifiedBy,
                        LastModifiedOn = b.LastModifiedOn
                    }).ToList();
                }
                    
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(data)}");
                return Ok(new GrcResponse<List<BranchResponse>>(data));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                   
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
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
                return Ok(new GrcResponse<List<BranchResponse>>(error));
            }
        }

        [HttpPost("organization/branches-list")]
        public async Task<IActionResult> AllBranches([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<BranchResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var pageResult = await _branchService.GetPagedBranchesAsync(
                    pageIndex:request.PageIndex, 
                    pageSize:request.PageSize,
                    includeDeleted:request.IncludeDeleted);

                //..map response
                List<BranchResponse> data = new();
                if(pageResult.Entities != null && pageResult.Entities.Any()) {
                    data = pageResult.Entities.Select(b => new BranchResponse() {
                        Id = b.Id,
                        CompanyId = b.CompanyId,
                        CompanyName = b.Company != null ? b.Company.CompanyName : string.Empty,
                        BranchName = b.BranchName,
                        SolId = b.SolId,
                        IsDeleted = b.IsDeleted,
                        CreatedBy = b.CreatedBy,
                        CreatedOn = b.CreatedOn,
                        LastModifiedBy = b.LastModifiedBy,
                        LastModifiedOn = b.LastModifiedOn
                    }).ToList();
                }
                    
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(data)}");
                return Ok(new GrcResponse<PagedResponse<BranchResponse>>(new PagedResponse<BranchResponse>(
                            data,
                            pageResult.Count,
                            pageResult.Page,
                            pageResult.Size
                        )));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                   
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
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
                return Ok(new GrcResponse<PagedResponse<BranchResponse>>(error));
            }
        }

        [HttpPost("organization/create-branch")]
        public async Task<IActionResult> SaveBranch([FromBody] BranchRequest request) {
            try {

                Logger.LogActivity("Process unit data record for persistance", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "No data found to save, process has been cancelled"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = new GeneralResponse();

                //..check if record exists
                bool exists = await _branchService.ExistsAsync(request);
                if (exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.DUPLICATE;
                    response.Message = "Duplicate! Another branch found with either the same branch name or Sol ID";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..save branch record
                var result = await _branchService.InsertBranchAsync(request);
                
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Branch saved successfully";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save branch. An error occurrred";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                   
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - An unexpected error occurred while saving branch"
                );

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("organization/update-branch")]
        public async Task<IActionResult> UpdateBranch([FromBody] BranchRequest request) {
            try {
                var response = new GeneralResponse();
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                bool exists = await _branchService.ExistsByIdAsync(request.Id);
                if (!exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No branch found with ID '{request.Id}'";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..update branch record
                var status = await _branchService.UpdateBranchAsync(request);
                if(!status){
                    var error = new ResponseError(
                        ResponseCodes.NOTUPDATE,
                        "Failed to update department unit",
                        "An unexpected error occurred while trying to update department unit");
                     return Ok(new GrcResponse<GeneralResponse>(error));
                } 

               return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse(){Status = status}));
            } catch (Exception ex) {
                Logger.LogActivity($"Error updating department unit for user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while updating unit.");
            }
        }

        [HttpPost("organization/delete-branch")]
        public async Task<IActionResult> DeleteBranch([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                bool exists = await _branchService.ExistsByIdAsync(request.RecordId);
                if (!exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No branch found with ID '{request.RecordId}'";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete branch record
                var status = await _branchService.DeleteBranchAsync(request);
                if(!status){
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to delete branch", "An error occurred! could delete branch");
                     return Ok(new GrcResponse<GeneralResponse>(error));
                } 
               return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse(){Status = status}));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting unit by user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while deleting unit.");
            }
        }

        #endregion

        #region Configurations

        [HttpPost("organization/settings-all")]
        public async Task<IActionResult> GetConfiguration([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Request record cannot be empty","Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ConfigurationResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var response = await _configService.GetAllConfigurationAsync();
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                return Ok(new GrcResponse<ConfigurationResponse>(response));
            } catch (Exception ex) {
                return Ok(new GrcResponse<ConfigurationResponse>(await ResponseErrorAsync(ex)));
            }
        }

        [HttpPost("organization/include-deleted")]
        public async Task<IActionResult> GetIncludeDeletedRecord([FromBody] ConfigurationParamRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<BooleanConfigurationResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                if (await _configService.ExistsAsync(s=>s.ParameterName == request.ParamName)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND,
                        "Configuration Parameter not found", $"No configuration found with parameter name '{request.ParamName}'");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<BooleanConfigurationResponse>(error));
                }

                var response = await _configService.GetConfigurationAsync<bool>(request.ParamName);
                if(response == null){
                    var error = new ResponseError(ResponseCodes.FAILED, "System Error", $"Could not retrieve configuration. An error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<BooleanConfigurationResponse>(error));
                }

                var result = new BooleanConfigurationResponse(){
                    ParameterName = response.ParameterName,
                    ParameterValue = response.Value
                };
                Logger.LogActivity($"SETTINGS-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(result)}");
                return Ok(new GrcResponse<BooleanConfigurationResponse>(result));
            } catch (Exception ex) {
                return Ok(new GrcResponse<BooleanConfigurationResponse>(await ResponseErrorAsync(ex)));
            }
        }

        [HttpPost("organization/settings-update")]
        public async Task<IActionResult> UpdateConfigurationParameter([FromBody] SystemConfigurationRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                if (await _configService.ExistsAsync(s => s.ParameterName == request.ParamName)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND,
                        "Configuration Parameter not found", $"No configuration found with parameter name '{request.ParamName}'");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser == null) {
                    var error = new ResponseError(ResponseCodes.RESTRICTED, "Authentication Error", "User ID could not be verified");
                    Logger.LogActivity($"RESTRICTED: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                string username = currentUser != null ? currentUser.Username : "SYSTEM";    
                var response = await _configService.UpdateConfigurationAsync(request, username);
                if (!response) {
                    var error = new ResponseError(ResponseCodes.FAILED, "System Error", $"Could not retrieve configuration. An error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                var result = new GeneralResponse() {
                    Status = true,
                    StatusCode = 200,
                    Message = "Configuration parameter updated successfully"
                };
                Logger.LogActivity($"SETTINGS-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(result)}");
                return Ok(new GrcResponse<GeneralResponse>(result));
            } catch (Exception ex) {
                return Ok(new GrcResponse<GeneralResponse>(await ResponseErrorAsync(ex)));
            }
        }

        [HttpPost("organization/general-configurations")]
        public async Task<IActionResult> SaveGeneralConfigurations([FromBody] GeneralConfigurationsRequest request) {
            try {

                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<BooleanConfigurationResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser == null) {
                    var error = new ResponseError(ResponseCodes.RESTRICTED, "Authentication Error", "User ID could not be verified");
                    Logger.LogActivity($"RESTRICTED: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                string username = currentUser != null ? currentUser.Username : "SYSTEM";
                var successful = await _configService.SaveGeneralConfigurationsAsync(request, username);
                if (!successful) {
                    var error = new ResponseError(ResponseCodes.FAILED, "System Error! Could not save configuration. An error occurred", $"Could not save configuration. An error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                var result = new GeneralResponse() {
                    Status = true,
                    Message = "Settings Saved successfully",
                    StatusCode = 200
                };
                Logger.LogActivity($"SETTINGS-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(result)}");
                return Ok(new GrcResponse<GeneralResponse>(result));
            } catch (Exception ex) {
                return Ok(new GrcResponse<GeneralResponse>(await ResponseErrorAsync(ex)));
            }
        }

        [HttpPost("organization/policy-configurations")]
        public async Task<IActionResult> SavePolicyConfigurations([FromBody] PolicyConfigurationsRequest request) {
            try {

                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<BooleanConfigurationResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser == null) {
                    var error = new ResponseError(ResponseCodes.RESTRICTED, "Authentication Error", "User ID could not be verified");
                    Logger.LogActivity($"RESTRICTED: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                string username = currentUser != null ? currentUser.Username : "SYSTEM";
                var successful = await _configService.SavePolicyConfigurationsAsync(request, username);
                if (!successful) {
                    var error = new ResponseError(ResponseCodes.FAILED, "System Error! Could not save configuration. An error occurred", $"Could not save configuration. An error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                var result = new GeneralResponse() {
                    Status = true,
                    Message = "Settings Saved successfully",
                    StatusCode = 200
                };
                Logger.LogActivity($"SETTINGS-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(result)}");
                return Ok(new GrcResponse<GeneralResponse>(result));
            } catch (Exception ex) {
                return Ok(new GrcResponse<GeneralResponse>(await ResponseErrorAsync(ex)));
            }
        }
        
        [HttpPost("organization/password-policy-configuration")]
        public async Task<IActionResult> SavePasswordPolicyConfigurations([FromBody] PasswordConfigurationsRequest request) {
            try {

                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<BooleanConfigurationResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser == null) {
                    var error = new ResponseError(ResponseCodes.RESTRICTED, "Authentication Error", "User ID could not be verified");
                    Logger.LogActivity($"RESTRICTED: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                string username = currentUser != null ? currentUser.Username : "SYSTEM";
                var successful = await _configService.SavePasswordPolicyConfigurationsAsync(request, username);
                if (!successful) {
                    var error = new ResponseError(ResponseCodes.FAILED, "System Error! Could not save configuration. An error occurred", $"Could not save configuration. An error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                var result = new GeneralResponse() {
                    Status = true,
                    Message = "Settings Saved successfully",
                    StatusCode = 200
                };
                Logger.LogActivity($"SETTINGS-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(result)}");
                return Ok(new GrcResponse<GeneralResponse>(result));
            } catch (Exception ex) {
                return Ok(new GrcResponse<GeneralResponse>(await ResponseErrorAsync(ex)));
            }
        }

        [HttpPost("organization/password-settings")]
        public async Task<IActionResult> GetPasswordSetting([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PasswordChangeResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var response = await _configService.GetPasswordSettingAsync();
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                return Ok(new GrcResponse<PasswordChangeResponse>(response));
            } catch (Exception ex) {
                return Ok(new GrcResponse<PasswordChangeResponse>(await ResponseErrorAsync(ex)));
            }
        }

        #endregion

        #region Bugs

        [HttpPost("organization/bug-retrieve")]
        public async Task<IActionResult> GetSystemError([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get System Error", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Request System Error ID is required", "Invalid request Role ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RoleResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = await _bugService.GetBugAsync(request.RecordId);
                if (response != null) {
                    var bug = new BugResponse() {
                        Id = response.Id,
                        Error = response.ErrorMessage,
                        Severity = response.Severity,
                        Status = response.FixStatus,
                        Source = response.ErrorSource,
                        AssignedTo = response.AssignedTo,
                        StackTrace = response.StackTrace
                    };
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(bug)}");
                    return Ok(new GrcResponse<BugResponse>(bug));
                } else {
                    var error = new ResponseError(ResponseCodes.FAILED, "System Error not found", "No System Error matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<BugResponse>(error));
                }
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<BugResponse>(error));
            }
        }

        [HttpPost("organization/bug-list")]
        public async Task<IActionResult> GetAllBugs([FromBody] BugListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<BugItemResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var pageResult = await _bugService.GetBugsAsync(request);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No bug list found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<BugItemResponse>>(new PagedResponse<BugItemResponse>(new List<BugItemResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<BugItemResponse> bugs = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(bug => bugs.Add(new() {
                        Id = bug.Id,
                        Error = bug.ErrorMessage,
                        Severity = bug.Severity,
                        Status = bug.FixStatus,
                        CreatedOn = bug.CreatedOn,
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    bugs = bugs.Where(u =>
                        (u.Error?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Severity?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Status?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<BugItemResponse>>(
                    new PagedResponse<BugItemResponse>(
                    bugs,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                var error = new ResponseError(ResponseCodes.BADREQUEST,"Oops! Something went wrong",$"System Error - {ex.Message}");
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<BugItemResponse>>(error));
            }
        }

        [HttpPost("organization/bug-export")]
        public async Task<IActionResult> GetSystemErrorList([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<BugResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var permissionSets = await _bugService.GetAllAsync(b=> b.FixStatus == "OPEN", false);
                if (permissionSets == null || !permissionSets.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS,"No data", "No System erros found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ListResponse<BugResponse>>(error));
                }

                var bugs = permissionSets.Select(b => new BugResponse() { 
                    Id = b.Id,
                    Error = b.ErrorMessage,
                    Severity = b.Severity,
                    Status = b.FixStatus,
                    Source = b.ErrorSource,
                    StackTrace = b.StackTrace,
                    AssignedTo = b.AssignedTo,
                    CreatedOn = b.CreatedOn
                }).ToList();
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: Retrieved {bugs.Count} system errors (IDs: {string.Join(", ", bugs.Select(u => u.Id))})");
                return Ok(new GrcResponse<ListResponse<BugResponse>>(new ListResponse<BugResponse>() {
                    Data = bugs
                }));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<BugResponse>>(error));
            }
        }

        [HttpPost("organization/bug-status")]
        public async Task<IActionResult> GetStatusBugs([FromBody] BugStatusListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Request record cannot be empty","Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<BugResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _bugService.PageAllAsync(request.PageIndex, request.PageSize, true, b =>b.FixStatus == request.Status);

                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS,"No data","No permission set records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<BugResponse>>(new PagedResponse<BugResponse>(
                        new List<BugResponse>(),0,pageResult.Page,pageResult.Size)));
                }

                var permissionData = pageResult.Entities;
                var sets = permissionData.Select(Mapper.Map<BugResponse>).ToList();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    sets = sets.Where(u =>
                        (u.Error?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Source?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Severity?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<BugResponse>>(new PagedResponse<BugResponse>(
                    sets,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<RoleGroupResponse>>(error));
            }
        }

        [HttpPost("organization/create-bug")]
        public async Task<IActionResult> CreateError([FromBody] BugRequest request) {
            try {
                Logger.LogActivity("Creating new system error record", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Error record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                //..get username

                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username = currentUser != null ? currentUser.Username : $"{request.UserId}";

                //..create system error
                var result = await _bugService.InsertSystemErrorAsync(request, username);
                var response = new GeneralResponse();
                if (result) {
                    //..set response
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "System Error saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save system error record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("organization/change-bug-status")]
        public async Task<IActionResult> ChangeBugStatus([FromBody] BugStatusRequest request) {
            try {
                Logger.LogActivity("Close system error", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The user record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _accessService.UserExistsAsync(r => r.Id == request.RecordId)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "User record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username = currentUser != null ? currentUser.Username : $"{request.UserId}";

                //..update system error
                await _bugService.UpdateStatusAsync(request.RecordId, request.Status, username);
                var response = new GeneralResponse { Status = true, StatusCode = (int)ResponseCodes.SUCCESS, Message = "User record updated successfully" };
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("organization/update-bug")]
        public async Task<IActionResult> UpdateSystemError([FromBody] BugRequest request) {
            try {
                Logger.LogActivity("Update system error", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The system error record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _bugService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "System Error record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username = currentUser != null ? currentUser.Username : $"{request.UserId}";

                //..update system error
                var result = await _bugService.UpdateErrorAsync(request, username);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "System Error record updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update system error record record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Departments

        [HttpPost("departments/getDepartments")]
        public async Task<IActionResult> GetDepartments([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<DepartmentResponse>>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var dataList = await _departmentsService.GetAllAsync();

                //..map response
                List<DepartmentResponse> data = new();
                if(dataList != null && dataList.Any()) { 
                    data = dataList.Select(Mapper.Map<DepartmentResponse>).ToList();
                }
                    
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(data)}");
                return Ok(new GrcResponse<List<DepartmentResponse>>(data));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                   
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
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
                return Ok(new GrcResponse<List<DepartmentResponse>>(error));
            }
        }

        [HttpPost("departments/allDepartments")]
        public async Task<IActionResult> AllDepartments([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DepartmentResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var pageResult = await _departmentsService.GetPagedDepartmentsAsync(
                    pageIndex:request.PageIndex, 
                    pageSize:request.PageSize,
                    includeDeleted:request.IncludeDeleted);

                //..map response
                List<DepartmentResponse> data = new();
                if(pageResult.Entities != null && pageResult.Entities.Any()) { 
                    data = pageResult.Entities.Select(Mapper.Map<DepartmentResponse>).ToList();
                }
                    
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(data)}");
                return Ok(new GrcResponse<PagedResponse<DepartmentResponse>>(new PagedResponse<DepartmentResponse>(
                            data,
                            pageResult.Count,
                            pageResult.Page,
                            pageResult.Size
                        )));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<DepartmentResponse>>(error));
            }
        }
        
        [HttpPost("departments/saveDepartment")]
        public async Task<IActionResult> SaveDepartment([FromBody] DepartmentRequest request) {

            try {

                Logger.LogActivity("Process department data record for persistance", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "No data found to save, process has been cancelled"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = new GeneralResponse();

                //..check if record exists
                bool exists = await _departmentsService.ExistsAsync(request);
                if (exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.DUPLICATE;
                    response.Message = "Duplicate! Another department found with either the same department name or department code";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..get current user record
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                var username = currentUser != null ? $"{currentUser.Username}": $"{request.UserId}";
                //..save department
                var result = await _departmentsService.InsertDepartmentAsync(request, username);
                
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Department saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save department. An error occurrred";
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
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
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
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - An unexpected error occurred while saving unit"
                );

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("departments/updateDepartment")]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                bool exists = await _departmentsService.ExistsAsync(request);
                if (!exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No department found with ID '{request.Id}'";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..get current user record
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                var username = currentUser != null ? $"{currentUser.Username}": $"{request.UserId}";

                //..update department
                var status = await _departmentsService.UpdateDepartmentAsync(request,username);
                if(!status){
                    var error = new ResponseError(
                        ResponseCodes.NOTUPDATE,
                        "Failed to update department",
                        "An unexpected error occurred while trying to update department");
                        return Ok(new GrcResponse<StatusResponse>(error));
                } 
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse(){Status = status}));
            } catch (Exception ex) {
                Logger.LogActivity($"Error updating department for user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while updating department.");
            }
        }
        
        [HttpPost("departments/deleteDepartment")]
        public async Task<IActionResult> DeleteDepartment([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");
                
                //..check if record exists
                var response = new GeneralResponse();
                bool exists = await _departmentsService.ExistsByIdAsync(request.RecordId);
                if (!exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No department found with ID '{request.RecordId}'";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete department
                var status = await _departmentsService.DeleteDepartmentAsync(request);
                if(!status){
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete department",
                        "An error occurred! could delete department");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                } 
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse(){Status = status}));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting department by user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while deleting department.");
            }
        }
        
        [HttpPost("departments/getDepartmentById")]
        public async Task<IActionResult> GetDepartmentById([FromBody] IdRequest request) { 
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                ResponseError error = null;
                if (request == null) {
                    error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DepartmentResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var dataRecord = await _departmentsService.GetDepartmentByIdAsync(request.RecordId, true);

                //..map response
                DepartmentResponse result;
                if(dataRecord != null) { 
                    //..get head details
                    var head = await _departmentUnitService.GetDepartmentHeadAsync($"HOD - {dataRecord.DepartmentName}");

                    result = new DepartmentResponse() { 
                        Id = dataRecord.Id,
                        BranchId = dataRecord.BranchId,
                        Branch = dataRecord.Branch != null ? dataRecord.Branch.BranchName : string.Empty,
                        DepartmentCode = dataRecord.DepartmentCode,
                        DepartmentName = dataRecord.DepartmentName,
                        HeadFullName = head?.ContactName ?? string.Empty,
                        HeadEmail = head?.ContactEmail ?? string.Empty,
                        HeadContact = head?.ContactPhone ?? string.Empty,
                        HeadDesignation = head?.ContactPosition ?? string.Empty,
                        HeadComment = head?.Description ?? string.Empty,
                        DepartmentAlias = dataRecord.Alias,
                        DepartmentUnits = dataRecord.Units != null && dataRecord.Units.Any() ?
                        dataRecord.Units.Select(u => new DepartmentUnitResponse() {
                            Id = u.Id,
                            DepartmentId = u.DepartmentId,
                            UnitCode = u.UnitCode,
                            UnitName = u.UnitName,
                            Department = dataRecord.DepartmentName

                        }).ToList() :
                        new List<DepartmentUnitResponse>(),
                    };
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(result)}");
                    return Ok(new GrcResponse<DepartmentResponse>(result));
                }
                    
                error = new ResponseError(ResponseCodes.FAILED, "An error occurred while retriving unit", "Get help from your administrtor");
                return Ok(new GrcResponse<DepartmentResponse>(error));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                   
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
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
                return Ok(new GrcResponse<DepartmentResponse>(error));
            }
        }
        
        [HttpPost("departments/getUnitById")]
        public async Task<IActionResult> GetUnitById([FromBody] IdRequest request) { 
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                ResponseError error = null;
                if (request == null) {
                    error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DepartmentUnitResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var dataRecord = await _departmentUnitService.GetUnitByIdAsync(request.RecordId, true);

                //..map response
                DepartmentUnitResponse result;
                if(dataRecord != null) { 
                    //..get head details
                    var head = await _departmentUnitService.GetDepartmentHeadAsync($"Unit Head - {dataRecord.UnitName}");

                    result = new DepartmentUnitResponse() { 
                        Id = dataRecord.Id,
                        DepartmentId = dataRecord.DepartmentId,
                        UnitCode = dataRecord.UnitCode,
                        UnitName = dataRecord.UnitName,
                        UnitHead = head?.ContactName ?? string.Empty,
                        UnitContactEmail = head?.ContactEmail ?? string.Empty,
                        UnitContactNumber = head?.ContactPhone ?? string.Empty,
                        UnitHeadDesignation = head?.ContactPosition ?? string.Empty
                    };
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(result)}");
                    return Ok(new GrcResponse<DepartmentUnitResponse>(result));
                }
                    
                error = new ResponseError(ResponseCodes.FAILED, "An error occurred while retriving unit", "Get help from your administrtor");
                return Ok(new GrcResponse<DepartmentUnitResponse>(error));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                   
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
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
                return Ok(new GrcResponse<DepartmentResponse>(error));
            }
        }

        [HttpPost("departments/getUnits")]
        public async Task<IActionResult> GetUnits([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<DepartmentUnitResponse>>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var dataList = await _departmentUnitService.GetAllAsync();

                //..map response
                List<DepartmentUnitResponse> data = new();
                if(dataList != null && dataList.Any()) { 
                    data = dataList.Select(Mapper.Map<DepartmentUnitResponse>).ToList();
                }
                    
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(data)}");
                return Ok(new GrcResponse<List<DepartmentUnitResponse>>(data));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                   
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
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
                return Ok(new GrcResponse<List<DepartmentUnitResponse>>(error));
            }
        }

        [HttpPost("departments/allUnits")]
        public async Task<IActionResult> AllUnits([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DepartmentUnitResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var pageResult = await _departmentUnitService.GetPagedUnitsAsync(
                    pageIndex:request.PageIndex, 
                    pageSize:request.PageSize,
                    includeDeleted:request.IncludeDeleted);

                //..map response
                List<DepartmentUnitResponse> data = new();
                if(pageResult.Entities != null && pageResult.Entities.Any()) { 
                    data = pageResult.Entities.Select(Mapper.Map<DepartmentUnitResponse>).ToList();
                }
                    
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(data)}");
                return Ok(new GrcResponse<PagedResponse<DepartmentUnitResponse>>(new PagedResponse<DepartmentUnitResponse>(
                            data,
                            pageResult.Count,
                            pageResult.Page,
                            pageResult.Size
                        )));
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                       
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    "Oops! Something went wrong",
                    $"System Error - {ex.Message}"
                );
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<PagedResponse<DepartmentUnitResponse>>(error));
            }
        }
        
        [HttpPost("departments/saveUnit")]
        public async Task<IActionResult> SaveUnit([FromBody] DepartmentUnitRequest request) {
            try {

                Logger.LogActivity("Process unit data record for persistance", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "No data found to save, process has been cancelled"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = new GeneralResponse();

                //..check if record exists
                bool exists = await _departmentUnitService.ExistsAsync(request);
                if (exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.DUPLICATE;
                    response.Message = "Duplicate! Another unit found with either the same unit name or unit code";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..get current user record
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                var username = currentUser != null ? $"{currentUser.Username}": $"{request.UserId}";
                //..save department unit
                var result = await _departmentUnitService.InsertUnitAsync(request, currentUser.Username);
                
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Unit saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save department unit. An error occurrred";
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
                    ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
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
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                var error = new ResponseError(
                    ResponseCodes.BADREQUEST,
                    $"Oops! Something thing went wrong",
                    $"System Error - An unexpected error occurred while saving unit"
                );

                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("departments/updateUnit")]
        public async Task<IActionResult> UpdateUnit([FromBody] DepartmentUnitRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");
                
                //..check if record exists
                var response = new GeneralResponse();
                bool exists = await _departmentUnitService.ExistsAsync(request);
                if (!exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No unit found with ID '{request.Id}'";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..get current user record
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                var username = currentUser != null ? $"{currentUser.Username}": $"{request.UserId}";
                //..update unit
                var status = await _departmentUnitService.UpdateUnitAsync(request, currentUser.Username);
                if(!status){
                    var error = new ResponseError(
                        ResponseCodes.NOTUPDATE,
                        "Failed to update department unit",
                        "An unexpected error occurred while trying to update department unit");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                } 
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse(){Status = status}));
            } catch (Exception ex) {
                Logger.LogActivity($"Error updating department unit for user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while updating unit.");
            }
        }
        
        [HttpPost("departments/deleteUnit")]
        public async Task<IActionResult> DeleteUnit([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");
                
                //..check if record exists
                var response = new GeneralResponse();
                bool exists = await _departmentUnitService.ExistsByIdAsync(request.RecordId);
                if (!exists) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Not Found!! No unit found with ID '{request.RecordId}'";
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete unit
                var status = await _departmentUnitService.DeleteUnitAsync(request);
                if(!status){
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete department unit",
                        "An error occurred! could delete unit");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                } 
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse(){Status = status}));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting unit by user {request.UserId}: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = await CompanyService.GetDefaultCompanyAsync();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
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
                if(result){
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Error captured and saved successfully";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else { 
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to capture error to database. An error occurrred";  
                    Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return StatusCode(500, "An error occurred while deleting unit.");
            }
        }

        #endregion

        #region Private Members

        protected async Task<ResponseError> ResponseErrorAsync (Exception ex) {
            Logger.LogActivity($"{ex.Message}", "ERROR");
            Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

            var conpany = await CompanyService.GetDefaultCompanyAsync();
            long companyId = conpany != null ? conpany.Id : 1;
            SystemError errorObj = new() {
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
            if (result) {
                response.Status = true;
                response.StatusCode = (int)ResponseCodes.SUCCESS;
                response.Message = "Error captured and saved successfully";
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
            } else {
                response.Status = true;
                response.StatusCode = (int)ResponseCodes.FAILED;
                response.Message = "Failed to capture error to database. An error occurrred";
                Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
            }

            var error = new ResponseError(ResponseCodes.BADREQUEST, "Oops! Something went wrong", $"System Error - {ex.Message}");
            Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

            return error;
        }
        #endregion

    }
}

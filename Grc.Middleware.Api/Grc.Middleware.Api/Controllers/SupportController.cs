using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Logging;
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
    public class SupportController : GrcControllerBase {

        private readonly IActivityLogService _activityLogService;
        private readonly IActivityTypeService _activityTypeService;
        private readonly IActivityLogSettingService _activitySettingService;
        private readonly IBranchService _branchService;
        private readonly IDepartmentsService _departmentsService;
        private readonly IDepartmentUnitService _departmentUnitService;

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
            IBranchService branchService)
            : base(cypher, loggerFactory, mapper, companyService, environment, 
                  errorService, systemErrorService) {
            _activityLogService = activityLogService;
            _activityTypeService = activityTypeService;
            _activitySettingService = activitySettingService;
            _departmentsService = departmentsService;
            _departmentUnitService = departmentUnitService;
            _branchService = branchService;
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
                        "The company registration model cannot be null"
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

        [HttpPost("organization/getBranches")]
        public async Task<IActionResult> GetBranches([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<BranchResponse>>(error));
                }

                Logger.LogActivity($"REQUEST >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                var dataList = await _branchService.GetAllAsync();

                //..map response
                List<BranchResponse> data = new();
                if(dataList != null && dataList.Any()) { 
                    data = dataList.Select(Mapper.Map<BranchResponse>).ToList();
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

        [HttpPost("organization/allBranches")]
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

                var pageResult = await _departmentsService.GetPagedDepartmentsAsync(
                    pageIndex:request.PageIndex, 
                    pageSize:request.PageSize,
                    includeDeleted:request.IncludeDeleted);

                //..map response
                List<BranchResponse> data = new();
                if(pageResult.Entities != null && pageResult.Entities.Any()) { 
                    data = pageResult.Entities.Select(Mapper.Map<BranchResponse>).ToList();
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

        [HttpPost("organization/saveBranch")]
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

        [HttpPost("organization/updateBranch")]
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

        [HttpPost("organization/deleteBranch")]
        public async Task<IActionResult> DeleteBranch([FromBody] DeleteRequst request) {
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

        #region Departments
        
        [HttpPost("departments/getDepartments")]
        public async Task<IActionResult> GetDepartments([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");

                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
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

                //..save department
                var result = await _departmentsService.InsertDepartmentAsync(request);
                
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

                //..update department
                var status = await _departmentsService.UpdateDepartmentAsync(request);
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
        public async Task<IActionResult> DeleteDepartment([FromBody] DeleteRequst request) {
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

                //..save department unit
                var result = await _departmentUnitService.InsertUnitAsync(request);
                
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

                //..update unit
                var status = await _departmentUnitService.UpdateUnitAsync(request);
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
        public async Task<IActionResult> DeleteUnit([FromBody] DeleteRequst request) {
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

    }
}

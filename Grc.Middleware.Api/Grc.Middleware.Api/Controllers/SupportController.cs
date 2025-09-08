using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Logging;
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
            IMapper mapper, IEnvironmentProvider environment,
            IErrorNotificationService errorService, 
            ISystemErrorService systemErrorService)
            : base(cypher, loggerFactory, mapper, environment, 
                  errorService, systemErrorService) {
            _activityLogService = activityLogService;
            _activityTypeService = activityTypeService;
            _activitySettingService = activitySettingService;
            _departmentsService = departmentsService;
            _departmentUnitService = departmentUnitService;
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

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(data)}");
                    return Ok(new GrcResponse<PagedResponse<ActivityLogResponse>>(new PagedResponse<ActivityLogResponse>(
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
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
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

        #region departments

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
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DepartmentResponse>>(error));
                }
            }
        
        [HttpPost("departments/saveDepartment")]
        public async Task<IActionResult> SaveDepartment([FromBody] DepartmentRequest request) {

            //try {

            //    Logger.LogActivity("Process activity log record for persistance", "INFO");
            //    if (request == null) { 
            //        var error = new ResponseError(
            //            ResponseCodes.BADREQUEST,
            //            "Request record cannot be empty",
            //            "The company registration model cannot be null"
            //        );
        
            //        Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
            //        return Ok(new GrcResponse<GeneralResponse>(error));
            //    }

            //    Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

            //    //..get activity type
            //    ActivityType type = null;
            //    if (!string.IsNullOrWhiteSpace(request.Activity)) {
            //        type = await _activityTypeService.GetActivityTypeByNameAsync(request.Activity);
            //    }

            //    if(type == null && !string.IsNullOrWhiteSpace(request.SystemKeyword)) { 
            //        type = await _activityTypeService.GetActivityTypeBySystemKeywordAsync(request.SystemKeyword);
            //    }

            //    //..create company
            //    var result = await _activityLogService.InsertActivityAsync(type,request.Comment, request.UserId, request.IPAddress, request.EntityName);

            //    var response = new GeneralResponse();
            //    if(result){
            //        response.Status = true;
            //        response.StatusCode = (int)ResponseCodes.SUCCESS;
            //        response.Message = "Registration completed successfully";    
            //        Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
            //    } else { 
            //        response.Status = true;
            //        response.StatusCode = (int)ResponseCodes.FAILED;
            //        response.Message = "Failed to complete regiatration. An error occurrred";  
            //        Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
            //    }

            //    return Ok(new GrcResponse<GeneralResponse>(response));
            //} catch (Exception ex) { 
            //    Logger.LogActivity($"{ex.Message}", "ERROR");
            //    Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

            //    var error = new ResponseError(
            //        ResponseCodes.BADREQUEST,
            //        $"Oops! Something thing went wrong",
            //        $"System Error - {ex.Message}"
            //    );
        
            //    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
            //    return Ok(new GrcResponse<GeneralResponse>(error));
            //}
            //
            return null;
        }

        [HttpPost("departments/updateDepartment")]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentRequest request) {
            return null;
        }
        
        [HttpPost("departments/deleteDepartment")]
        public async Task<IActionResult> DeleteDepartment([FromBody] DepartmentRequest request) {
            return null;
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
        public async Task<IActionResult> SaveUnit([FromBody] DepartmentRequest request) {

            return null;
        }

        [HttpPost("departments/updateUnit")]
        public async Task<IActionResult> UpdateUnit([FromBody] DepartmentRequest request) {
            return null;
        }
        
        [HttpPost("departments/deleteUnit")]
        public async Task<IActionResult> DeleteUnit([FromBody] DepartmentRequest request) {
            return null;
        }

        #endregion

    }
}

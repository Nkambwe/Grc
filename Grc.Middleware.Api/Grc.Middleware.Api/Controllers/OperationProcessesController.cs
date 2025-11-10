using AutoMapper;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Operations;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Text.Json;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class OperationProcessesController : GrcControllerBase {

        private readonly IOperationProcessService _processService;
        private readonly IProcessTagService _tagService;
        private readonly IProcessTypeService _typeService;
        private readonly IDepartmentsService _departmentService;

        public OperationProcessesController(IObjectCypher cypher, 
                                            IServiceLoggerFactory loggerFactory, 
                                            IMapper mapper, 
                                            ICompanyService companyService, 
                                            IEnvironmentProvider environment, 
                                            IErrorNotificationService errorService, 
                                            ISystemErrorService systemErrorService,
                                            IOperationProcessService processService,
                                            IProcessTagService tagService,
                                            IProcessTypeService typeService,
                                            IDepartmentsService departmentService
                                            ) 
                                            : base(cypher, 
                                                  loggerFactory, 
                                                  mapper, 
                                                  companyService, 
                                                  environment, 
                                                  errorService, 
                                                  systemErrorService) {
            _processService = processService;
            _tagService = tagService;
            _typeService = typeService;
            _departmentService = departmentService;

        }

        #region Process Register Endpoints

        [HttpPost("processes/support-items")]
        public async Task<IActionResult> GetProcessSupportItems([FromBody] GeneralRequest request)
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
                    return Ok(new GrcResponse<ProcessSupportResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                //..get support data
                var _supportItemsList = await _processService.GetSupportItemsAsync(false);
                return Ok(new GrcResponse<ProcessSupportResponse>(_supportItemsList));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ProcessSupportResponse>(error));
            }
        }

        [HttpPost("processes/register")]
        public async Task<IActionResult> GetProcessRegister([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get operations process by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessRegisterResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request operations process ID is required",
                        "Invalid request operations process ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessRegisterResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var register = await _processService.GetAsync(p => p.Id == request.RecordId, true, 
                                                              p => p.Unit,
                                                              p => p.Owner,
                                                              p => p.Responsible,
                                                              p => p.ProcessType);
                if (register == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Operations process not found",
                        "No operations process matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessRegisterResponse>(error));
                }

                //..return process register data
                var registerRecord = new ProcessRegisterResponse {
                    Id = register.Id,
                    ProcessName = register.ProcessName ?? string.Empty,
                    Description = register.Description ?? string.Empty,
                    CurrentVersion = register.CurrentVersion ?? string.Empty,
                    EffectiveDate = register.EffectiveDate,
                    LastUpdated = register.LastUpdated,
                    FileName = register.FileName ?? string.Empty,
                    OriginalOnFile = register.OriginalOnFile,
                    ProcessStatus = register.ProcessStatus ?? string.Empty,
                    Comments = register.Comments ?? string.Empty,
                    IsLockProcess = register.IsLockProcess,
                    NeedsBranchReview = register.NeedsBranchReview,
                    NeedsCreditReview = register.NeedsCreditReview,
                    NeedsTreasuryReview = register.NeedsTreasuryReview,
                    NeedsFintechReview = register.NeedsFintechReview,
                    OnholdReason = register.ReasonOnhold ?? string.Empty,
                    TypeId = register.TypeId,
                    TypeName = register.ProcessType != null ? register.ProcessType.TypeName : string.Empty,
                    UnitId = register.UnitId,
                    UnitName = register.Unit != null ? register.Unit.UnitName : string.Empty,
                    OwnerId = register.ResponsibilityId,
                    OwnerName = register.Owner != null ? register.Owner.ContactName : string.Empty,
                    IsDeleted = register.IsDeleted,
                    CreatedOn = register.CreatedOn,
                    CreatedBy = register.CreatedBy ?? "System",
                    ModifiedOn = register.LastModifiedOn ?? register.CreatedOn,
                    ModifiedBy = register.LastModifiedBy ?? string.Empty
                };

                return Ok(new GrcResponse<ProcessRegisterResponse>(registerRecord));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ProcessRegisterResponse>(error));
            }
        }

        [HttpPost("processes/registers-all")]
        public async Task<IActionResult> GetProcessRegisters([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _processService.PageAllAsync(request.PageIndex, request.PageSize, true, 
                                                                    p => p.Unit,
                                                                    p => p.Owner,
                                                                    p => p.Responsible,
                                                                    p => p.ProcessType);

                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No operation process records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(new PagedResponse<ProcessRegisterResponse>(
                        new List<ProcessRegisterResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                List<ProcessRegisterResponse> processes = new();
                var records = pageResult.Entities;
                if (records != null && records.Any()) {
                    records.ForEach(register => processes.Add(new() {
                        Id = register.Id,
                        ProcessName = register.ProcessName ?? string.Empty,
                        Description = register.Description ?? string.Empty,
                        CurrentVersion = register.CurrentVersion ?? string.Empty,
                        EffectiveDate = register.EffectiveDate,
                        LastUpdated = register.LastUpdated,
                        FileName = register.FileName ?? string.Empty,
                        OriginalOnFile = register.OriginalOnFile,
                        ProcessStatus = register.ProcessStatus ?? string.Empty,
                        Comments = register.Comments ?? string.Empty,
                        OnholdReason = register.ReasonOnhold ?? string.Empty,
                        TypeId = register.TypeId,
                        TypeName = register.ProcessType != null ? register.ProcessType.TypeName : string.Empty,
                        UnitId = register.UnitId,
                        UnitName = register.Unit != null ? register.Unit.UnitName : string.Empty,
                        OwnerId = register.OwnerId,
                        OwnerName = register.Owner != null ? register.Owner.ContactPosition : string.Empty,
                        ResponsibilityId = register.ResponsibilityId,
                        Responsibile = register.Responsible != null ? register.Responsible.ContactPosition : string.Empty,
                        IsLockProcess = register.IsLockProcess,
                        NeedsBranchReview = register.NeedsBranchReview,
                        NeedsCreditReview = register.NeedsCreditReview,
                        NeedsTreasuryReview = register.NeedsTreasuryReview,
                        NeedsFintechReview = register.NeedsFintechReview,
                        IsDeleted = register.IsDeleted,
                        CreatedOn = register.CreatedOn,
                        CreatedBy = register.CreatedBy ?? string.Empty,
                        ModifiedOn = register.LastModifiedOn ?? register.CreatedOn,
                        ModifiedBy = register.LastModifiedBy ?? string.Empty
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    processes = processes.Where(u =>
                        (u.ProcessName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(
                    new PagedResponse<ProcessRegisterResponse>(
                    processes,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(error));
            }
        }

        [HttpPost("processes/create-process")]
        public async Task<IActionResult> CreateProcessRegister([FromBody] ProcessRequest request) {
            try
            {
                Logger.LogActivity("Creating new operations process", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The operations process record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.ProcessName) && !string.IsNullOrWhiteSpace(request.Description)) {
                    if (await _processService.ExistsAsync(r => r.ProcessName == request.ProcessName || r.Description == request.Description)) {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another Process found with similar process name or description"
                        );
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                }
                else
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The process name and description are required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..create process register
                var result = await _processService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Operation processes saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save operation process record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("processes/update-process")]
        public async Task<IActionResult> UpdateProcessRegister([FromBody] ProcessRequest request) {
            try {
                Logger.LogActivity("Update operation process", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The operations process record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _processService.ExistsAsync(r => r.Id == request.Id))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "Operations Process record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..update operations process
                var result = await _processService.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Operations Processes updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update operations process record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("processes/delete-process")]
        public async Task<IActionResult> DeleteProcessRegister([FromBody] IdRequest request) {
            try
            {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _processService.ExistsAsync(r => r.Id == request.RecordId))
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Process Not Found!! No Role record found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete system role
                var status = await _processService.DeleteAsync(request);
                if (!status)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete operation process",
                        "An error occurred! could delete operation process");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting operation process by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

    }
}

using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Extensions;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Compliance.Support;
using Grc.Middleware.Api.Services.Operations;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class OperationProcessesController : GrcControllerBase {

        private readonly IOperationProcessService _processService;
        private readonly IProcessTagService _tagService;
        private readonly IProcessGroupService _groupService;
        private readonly ISystemAccessService _accessService;
        private readonly IProcessApprovalService _approvalService;
        private readonly IMailService _mailService;
        private readonly IResponsebilityService _officersService;

        public OperationProcessesController(IObjectCypher cypher, 
                                            IServiceLoggerFactory loggerFactory, 
                                            IMapper mapper, 
                                            ICompanyService companyService, 
                                            IEnvironmentProvider environment, 
                                            IErrorNotificationService errorService, 
                                            ISystemErrorService systemErrorService,
                                            IOperationProcessService processService,
                                            IProcessTagService tagService,
                                            IProcessGroupService groupService,
                                            IProcessApprovalService approvalService,
                                            ISystemAccessService accessService,
                                            IMailService mailService,
                                            IResponsebilityService officersService
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
            _groupService = groupService;
            _mailService = mailService;
            _accessService = accessService;
            _approvalService = approvalService;
            _officersService = officersService;
        }

        #region Statistics Endpoints

        [HttpPost("processes/unit-statistics")]
        public async Task<IActionResult> GetUnitStatistics([FromBody] StatisticRequest request) {
            try {
                Logger.LogActivity("Retrieve Operations unit statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ServiceOperationUnitCountResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _processService.GetOperationUnitStatisticsAsync(false);
                return Ok(new GrcResponse<ServiceOperationUnitCountResponse>(result));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ProcessRegisterResponse>(error));
            }
        }

        [HttpPost("processes/unit-extensions-statistics")]
        public async Task<IActionResult> GetUnitStatisticExtensions([FromBody] UnitStatisticRequest request) {
            try {
                Logger.LogActivity("Retrieve Operations unit statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ServiceUnitExtensionCountResponse> (error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                if (string.IsNullOrWhiteSpace(request.UnitName)) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Unit name cannot be null or empty",
                        "Invalid request body! Unit name is required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ServiceUnitExtensionCountResponse>(error));
                }

                var statistic = await _processService.GetUnitStatisticExtensionsAsync(request.UnitName, false);
                return Ok(new GrcResponse<ServiceUnitExtensionCountResponse>(statistic));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ServiceUnitExtensionCountResponse>(error));
            }
        }

        [HttpPost("processes/category-statistics")]
        public async Task<IActionResult> GetCategoryStatistics([FromBody] StatisticRequest request) {
            try {
                Logger.LogActivity("Retrieve Operations unit statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ServiceCategoriesCountResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _processService.GetProcessCategoryStatisticsAsync();
                return Ok(new GrcResponse<ServiceCategoriesCountResponse>(result));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ServiceCategoriesCountResponse>(error));
            }
        }

        [HttpPost("processes/category-extensions-statistics")]
        public async Task<IActionResult> GetCategoryStatisticExtensions([FromBody] CategoryStatisticRequest request) {
            try {
                Logger.LogActivity("Retrieve Operations unit statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ServiceCategoryExtensionCountResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                if (string.IsNullOrWhiteSpace(request.Category)) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Process category is required",
                        "Invalid request body. Process category is not provided"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ServiceCategoryExtensionCountResponse>(error));

                }
                var categories = await _processService.GetCategoryStatisticExtensionsAsync(request.Category, false);
               return Ok(new GrcResponse<ServiceCategoryExtensionCountResponse>(categories));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ServiceCategoryExtensionCountResponse>(error));
            }
        }

        [HttpPost("processes/total-process-statistics")]
        public async Task<IActionResult> GetProcessTotalStatistics([FromBody] GeneralRequest request) {

            try {
                Logger.LogActivity("Retrieve Operations unit statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<ServiceStatisticTotalResponse>>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var totalStats = await _processService.GetProcessTotalStatisticsAsync(false);
                if (totalStats == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "An error occurred",
                        "Could not retrieve statistics. A system error occurred"
                    );
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<List<ServiceStatisticTotalResponse>>(error));
                }

                return Ok(new GrcResponse<List<ServiceStatisticTotalResponse>>(totalStats));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<List<ServiceStatisticTotalResponse>>(error));
            }
        }

        #endregion

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

        [HttpPost("processes/process-list")]
        public async Task<IActionResult> GetAllProcesses([FromBody] GeneralRequest request) {
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
                    return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var processes = await _processService.GetAllAsync(false,p => p.Unit, p => p.Owner, p => p.Responsible, p => p.ProcessType);

                if (processes == null || !processes.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No operation process records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<ProcessRegisterResponse>>(new List<ProcessRegisterResponse>()));
                }

                List<ProcessRegisterResponse> processList = new();
                var records = processes.Where(p => !p.ProcessStatus.Equals("DRAFT") && !p.ProcessStatus.Equals("INREVIEW")).ToList();
                records.ForEach(register => processList.Add(new()
                {
                    Id = register.Id,
                    ProcessName = register.ProcessName ?? string.Empty,
                    Description = register.Description ?? string.Empty,
                    CurrentVersion = register.CurrentVersion ?? string.Empty,
                    EffectiveDate = register.EffectiveDate,
                    LastUpdated = register.LastUpdated,
                    FileName = register.FileName ?? string.Empty,
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
                    UnlockReason = register.UnlockReason,
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

                return Ok(new GrcResponse<List<ProcessRegisterResponse>>(processList));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<List<ProcessRegisterResponse>>(error));
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
                    ProcessStatus = register.ProcessStatus ?? string.Empty,
                    Comments = register.Comments ?? string.Empty,
                    IsLockProcess = register.IsLockProcess,
                    UnlockReason = register.UnlockReason,
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
                    ResponsibilityId = register.ResponsibilityId,
                    Responsibile = register.Responsible != null ? register.Responsible.ContactName : string.Empty,
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
                    return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _processService.PageAllAsync(request.PageIndex, request.PageSize, false,
                                                                    p => p.Unit,
                                                                    p => p.Owner,
                                                                    p => p.Responsible,
                                                                    p => p.ProcessType);

                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
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
                var records = pageResult.Entities.Where(p => !p.ProcessStatus.Equals("DRAFT") && !p.ProcessStatus.Equals("INREVIEW")).ToList(); 
                if (records != null && records.Any()) {
                    records.ForEach(register => processes.Add(new() {
                        Id = register.Id,
                        ProcessName = register.ProcessName ?? string.Empty,
                        Description = register.Description ?? string.Empty,
                        CurrentVersion = register.CurrentVersion ?? string.Empty,
                        EffectiveDate = register.EffectiveDate,
                        LastUpdated = register.LastUpdated,
                        FileName = register.FileName ?? string.Empty,
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
                        UnlockReason = register.UnlockReason,
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
                        (u.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TypeName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.UnitName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.OwnerName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Responsibile?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
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

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null)
                {
                    request.CreatedBy = currentUser.Username;
                    request.ModifiedBy = currentUser.Username;
                }
                else
                {
                    request.CreatedBy = $"{request.UserId}";
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.CreatedOn = DateTime.Now;
                request.ModifiedOn = DateTime.Now;

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

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null)
                {
                    request.ModifiedBy = currentUser.Username;
                }
                else
                {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.ModifiedOn = DateTime.Now;

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
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _processService.ExistsAsync(r => r.Id == request.RecordId))
                {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Process Not Found!! No Process record found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete operation process
                var status = await _processService.DeleteAsync(request);
                if (!status) {
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

        #region Process Groups Endpoints

        [HttpPost("processes/group")]
        public async Task<IActionResult> GetProcessGroup([FromBody] IdRequest request) {
            try
            {
                Logger.LogActivity("Get operations process by ID", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessGroupResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request process group ID is required",
                        "Invalid request process group ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessGroupResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var group = await _groupService.GetAsync(p => p.Id == request.RecordId, false, p => p.Processes);
                if (group == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Process group not found",
                        "No process group matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessRegisterResponse>(error));
                }

                //..map assigned processes for easy lookup
                var assignedIds = group.Processes != null ?
                                  group.Processes.Select(p => p.ProcessId).ToHashSet() :
                                  new HashSet<long>();

                //..get all processes
                var processData = await _processService.GetAllAsync(true);
                var processList = processData ?? new List<OperationProcess>();

                //merge processes isAssigned = true if in assignedIds
                var allProcesses = processList
                    .Select(p => new ProcessMinResponse
                    {
                        Id = p.Id,
                        ProcessName = p.ProcessName ?? string.Empty,
                        Description = p.Description ?? string.Empty,
                        IsAssigned = assignedIds.Contains(p.Id)
                    }).OrderBy(p => p.ProcessName).ToList();

                //..return process group data
                var groupRecord = new ProcessGroupResponse {
                    Id = group.Id,
                    GroupName = group.GroupName ?? string.Empty,
                    GroupDescription = group.Description ?? string.Empty,
                    IsDeleted = group.IsDeleted,
                    CreatedOn = group.CreatedOn,
                    CreatedBy = group.CreatedBy ?? "SYSTEM",
                    ModifiedOn = group.LastModifiedOn ?? group.CreatedOn,
                    ModifiedBy = group.LastModifiedBy ?? string.Empty,
                    Processes = allProcesses
                };

                return Ok(new GrcResponse<ProcessGroupResponse>(groupRecord));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ProcessGroupResponse>(error));
            }
        }

        [HttpPost("processes/groups-all")]
        public async Task<IActionResult> GetProcessGroups([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ProcessGroupResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _groupService.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No process group records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<ProcessGroupResponse>>(new PagedResponse<ProcessGroupResponse>(
                        new List<ProcessGroupResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                List<ProcessGroupResponse> groups = new();
                var records = pageResult.Entities;
                if (records != null && records.Any()) {
                    records.ForEach(group => groups.Add(new() {
                        Id = group.Id,
                        GroupName = group.GroupName ?? string.Empty,
                        GroupDescription = group.Description ?? string.Empty,
                        IsDeleted = group.IsDeleted,
                        CreatedOn = group.CreatedOn,
                        CreatedBy = group.CreatedBy ?? string.Empty,
                        ModifiedOn = group.LastModifiedOn ?? group.CreatedOn,
                        ModifiedBy = group.LastModifiedBy ?? string.Empty,
                        Processes = new List<ProcessMinResponse>()
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    groups = groups.Where(u =>
                        (u.GroupName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.GroupDescription?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<ProcessGroupResponse>>(
                    new PagedResponse<ProcessGroupResponse>(
                    groups,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ProcessGroupResponse>>(error));
            }
        }

        [HttpPost("processes/create-group")]
        public async Task<IActionResult> CreateProcessGroup([FromBody] ProcessGroupRequest request) {
            try {
                Logger.LogActivity("Creating new process group", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The process group record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.GroupName) && !string.IsNullOrWhiteSpace(request.GroupDescription)) {
                    if (await _groupService.ExistsAsync(r => r.GroupName == request.GroupName || r.Description == request.GroupDescription)) {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another Group found with similar name or description"
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
                        "The group name and description are required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.CreatedBy = currentUser.Username;
                    request.ModifiedBy = currentUser.Username;
                }
                else {
                    request.CreatedBy = $"{request.UserId}";
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.CreatedOn = DateTime.Now;
                request.ModifiedOn = DateTime.Now;

                //..create process group
                var result = await _groupService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Process group saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save process group record. An error occurrred";
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

        [HttpPost("processes/update-group")]
        public async Task<IActionResult> UpdateProcessGroup([FromBody] ProcessGroupRequest request) {
            try {
                Logger.LogActivity("Update process group", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The process record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _processService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "Process group record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null)
                {
                    request.ModifiedBy = currentUser.Username;
                }
                else
                {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.ModifiedOn = DateTime.Now;

                //..update process group
                var result = await _groupService.UpdateAsync(request, true);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Process group updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update process group record. An error occurrred";
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

        [HttpPost("processes/delete-group")]
        public async Task<IActionResult> DeleteProcessGroup([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _groupService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Notot Found!! Process group record you're looking for was not found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete process group
                var status = await _groupService.DeleteAsync(request);
                if (!status)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete process group",
                        "An error occurred! could delete process group");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting process group by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Process Tags Endpoints

        [HttpPost("processes/tag")]
        public async Task<IActionResult> GetProcessTag([FromBody] IdRequest request) {
            try
            {
                Logger.LogActivity("Get process tag by ID", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessTagResponse>(error));
                }

                if (request.RecordId == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request process tag ID is required",
                        "Invalid request process tag ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessTagResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var tag = await _tagService.GetAsync(p => p.Id == request.RecordId, false, p => p.Processes);
                if (tag == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Process tag not found",
                        "No process tag matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessTagResponse>(error));
                }

                //..map assigned processes for easy lookup
                var assignedIds = tag.Processes != null ?
                                  tag.Processes.Select(p => p.ProcessId).ToHashSet() :
                                  new HashSet<long>();

                //..get all processes
                var processData = await _processService.GetAllAsync(true);
                var processList = processData ?? new List<OperationProcess>();

                //merge processes isAssigned = true if in assignedIds
                var allProcesses = processList
                    .Select(p => new ProcessRegisterResponse {
                        Id = p.Id,
                        ProcessName = p.ProcessName ?? string.Empty,
                        Description = p.Description ?? string.Empty,
                        IsAssigned = assignedIds.Contains(p.Id)
                    }).OrderBy(p => p.ProcessName).ToList();

                //..return process tag data
                var tagRecord = new ProcessTagResponse {
                    Id = tag.Id,
                    TagName = tag.TagName ?? string.Empty,
                    TagDescription = tag.Description ?? string.Empty,
                    IsDeleted = tag.IsDeleted,
                    CreatedOn = tag.CreatedOn,
                    CreatedBy = tag.CreatedBy ?? "SYSTEM",
                    ModifiedOn = tag.LastModifiedOn ?? tag.CreatedOn,
                    ModifiedBy = tag.LastModifiedBy ?? "SYSTEM",
                    Processes = allProcesses
                };

                return Ok(new GrcResponse<ProcessTagResponse>(tagRecord));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ProcessTagResponse>(error));
            }
        }

        [HttpPost("processes/tags-all")]
        public async Task<IActionResult> GetProcessTags([FromBody] ListRequest request) {
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
                    return Ok(new GrcResponse<PagedResponse<ProcessTagResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _tagService.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No process group records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<ProcessTagResponse>>(new PagedResponse<ProcessTagResponse>(
                        new List<ProcessTagResponse>(),
                        0,
                        pageResult.Page,
                        pageResult.Size
                    )));
                }

                List<ProcessTagResponse> tags = new();
                var records = pageResult.Entities;
                if (records != null && records.Any()) {
                    records.ForEach(tag => tags.Add(new() {
                        Id = tag.Id,
                        TagName = tag.TagName ?? string.Empty,
                        TagDescription = tag.Description ?? string.Empty,
                        IsDeleted = tag.IsDeleted,
                        CreatedOn = tag.CreatedOn,
                        CreatedBy = tag.CreatedBy ?? string.Empty,
                        ModifiedOn = tag.LastModifiedOn ?? tag.CreatedOn,
                        ModifiedBy = tag.LastModifiedBy ?? string.Empty,
                        Processes = new List<ProcessRegisterResponse>()
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    tags = tags.Where(u =>
                        (u.TagName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TagDescription?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<ProcessTagResponse>>(
                    new PagedResponse<ProcessTagResponse>(
                    tags,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ProcessTagResponse>>(error));
            }
        }

        [HttpPost("processes/create-tag")]
        public async Task<IActionResult> CreateProcessTag([FromBody] ProcessTagRequest request) {
            try {
                Logger.LogActivity("Creating new process tag", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The process tag record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.TagName) && !string.IsNullOrWhiteSpace(request.TagDescription))
                {
                    if (await _tagService.ExistsAsync(r => r.TagName == request.TagName || r.Description == request.TagDescription))
                    {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record!",
                            "Another process tag found with similar name or description"
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
                        "The process tag name and description are required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null)
                {
                    request.CreatedBy = currentUser.Username;
                    request.ModifiedBy = currentUser.Username;
                }
                else
                {
                    request.CreatedBy = $"{request.UserId}";
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.CreatedOn = DateTime.Now;
                request.ModifiedOn = DateTime.Now;

                //..create process tag
                var result = await _tagService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Processes tag saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save process tag record. An error occurrred";
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

        [HttpPost("processes/update-tag")]
        public async Task<IActionResult> UpdateProcessTag([FromBody] ProcessTagRequest request) {
            try {
                Logger.LogActivity("Update process tag", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The process tag record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _processService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "Process tag record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null)
                {
                    request.ModifiedBy = currentUser.Username;
                }
                else
                {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.ModifiedOn = DateTime.Now;

                //..update process tag
                var result = await _tagService.UpdateAsync(request, true);
                var response = new GeneralResponse();
                if (result)
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Process tag updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update process tag record. An error occurrred";
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

        [HttpPost("processes/delete-tag")]
        public async Task<IActionResult> DeleteProcessTag([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _tagService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Record Notot Found!! Process tag record you're looking for was not found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete process tag
                var status = await _tagService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete process tag",
                        "An error occurred! could delete process tag");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting process tag by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Process TAT Endpoints

        [HttpPost("processes/tat")]
        public async Task<IActionResult> GetProcessTat([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get process TAT by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessTATResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request operations process ID is required",
                        "Invalid request operations process ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessTATResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var approval = await _approvalService.GetAsync(p => p.Id == request.RecordId, true, p => p.Process);
                if (approval == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Operations process not found",
                        "No operations process matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessTATResponse>(error));
                }

                //..return process register data
                var hod = CalculateDays(approval.HeadOfDepartmentStart, approval.HeadOfDepartmentEnd);
                var risk = CalculateDays(approval.RiskStart, approval.RiskEnd);
                var compliance = CalculateDays(approval.ComplianceStart, approval.ComplianceEnd);
                var bop = CalculateDays(approval.BranchOperationsStatusStart, approval.BranchOperationsStatusEnd);
                var credit = CalculateDays(approval.CreditStart, approval.CreditEnd);
                var treasury = CalculateDays(approval.TreasuryStart, approval.TreasuryEnd);
                var fintech = CalculateDays(approval.FintechStart, approval.FintechEnd);
                var tatRecord = new ProcessTATResponse {
                    Id = approval.Id,
                    ProcessId = approval.ProcessId,
                    RequestDate = approval.RequestDate,
                    ProcessName = approval.Process.ProcessName ?? string.Empty,
                    ProcessStatus = GetStatus(approval),
                    HodCount = hod,
                    HodStatus = approval.HeadOfDepartmentStatus,
                    HodComment = approval.HeadOfDepartmentComment ?? string.Empty,
                    HodEnddate = approval.HeadOfDepartmentEnd,
                    RiskCount = risk,
                    RiskStatus = approval.RiskStatus ?? string.Empty,
                    RiskComment = approval.RiskComment ?? string.Empty,
                    RiskEnddate = approval.RiskEnd,
                    ComplianceCount = compliance,
                    ComplianceStatus = approval.ComplianceStatus ?? string.Empty,
                    ComplianceComment = approval.ComplianceComment ?? string.Empty,
                    ComplianceEnddate = approval.ComplianceEnd,
                    BopCount = bop,
                    BropStatus = approval.BranchOperationsStatus ?? string.Empty,
                    BropComment = approval.BranchManagerComment ?? string.Empty,
                    BropEnddate = approval.BranchOperationsStatusEnd,
                    CreditCount = credit,
                    CreditStatus = approval.CreditStatus ?? string.Empty,
                    CreditComment = approval.CreditComment ?? string.Empty,
                    CreditEnddate = approval.CreditEnd,
                    TreasuryCount = treasury,
                    TreasuryStatus = approval.TreasuryStatus ?? string.Empty,
                    TreasuryComment = approval.TreasuryComment ?? string.Empty,
                    TreasuryEnddate = approval.TreasuryEnd,
                    FintechCount = fintech,
                    FintechStatus = approval.FintechStatus ?? string.Empty,
                    FintechComment = approval.FintechComment ?? string.Empty,
                    FintechEnddate = approval.FintechEnd,
                    TotalCount = hod + risk + compliance + bop + credit + treasury + fintech
                };

                return Ok(new GrcResponse<ProcessTATResponse>(tatRecord));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ProcessTATResponse>(error));
            }
        }

        [HttpPost("processes/tat-all")]
        public async Task<IActionResult> GetProcessTat([FromBody] ListRequest request) {
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
                    return Ok(new GrcResponse<PagedResponse<ProcessTATResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _approvalService.PageProcessApprovalStatusAsync(request.PageIndex, request.PageSize, true, p => p.Process);
                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No review process records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<ProcessTATResponse>>(
                        new PagedResponse<ProcessTATResponse>(
                        new List<ProcessTATResponse>(),
                        0, pageResult.Page, pageResult.Size)));
                }

                List<ProcessTATResponse> approvals = new();
                var records = pageResult.Entities;
                if (records != null && records.Any()) {
                    records.ForEach(approval => {
                        var hod = CalculateDays(approval.RequestDate, approval.HeadOfDepartmentEnd);
                        var risk = CalculateDays(approval.RiskStart, approval.RiskEnd);
                        var compliance = CalculateDays(approval.ComplianceStart, approval.ComplianceEnd);
                        var bop = CalculateDays(approval.BranchOperationsStatusStart, approval.BranchOperationsStatusEnd);
                        var credit = CalculateDays(approval.CreditStart, approval.CreditEnd);
                        var treasury = CalculateDays(approval.TreasuryStart, approval.TreasuryEnd);
                        var fintech = CalculateDays(approval.FintechStart, approval.FintechEnd);
                        approvals.Add(new ProcessTATResponse {
                            Id = approval.Id,
                            ProcessId = approval.ProcessId,
                            RequestDate = approval.RequestDate,
                            ProcessName = approval.Process.ProcessName ?? string.Empty,
                            ProcessStatus = GetStatus(approval),
                            HodCount = hod,
                            HodStatus = approval.HeadOfDepartmentStatus,
                            HodComment = approval.HeadOfDepartmentComment ?? string.Empty,
                            RiskCount = risk,
                            RiskStatus = approval.RiskStatus ?? string.Empty,
                            RiskComment = approval.RiskComment ?? string.Empty,
                            ComplianceCount = compliance,
                            ComplianceStatus = approval.ComplianceStatus ?? string.Empty,
                            ComplianceComment = approval.ComplianceComment ?? string.Empty,
                            BopCount = bop,
                            BropStatus = approval.BranchOperationsStatus ?? string.Empty,
                            BropComment = approval.BranchManagerComment ?? string.Empty,
                            CreditCount = credit,
                            CreditStatus = approval.CreditStatus ?? string.Empty,
                            CreditComment = approval.CreditComment ?? string.Empty,
                            TreasuryCount = treasury,
                            TreasuryStatus = approval.TreasuryStatus ?? string.Empty,
                            TreasuryComment = approval.TreasuryComment ?? string.Empty,
                            FintechCount = fintech,
                            FintechStatus = approval.FintechStatus ?? string.Empty,
                            FintechComment = approval.FintechComment ?? string.Empty,
                            TotalCount = hod + risk + compliance + bop + credit + treasury + fintech
                        });
                    });

                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    approvals = approvals.Where(u =>
                        (u.ProcessName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.RiskStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.BropStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.ComplianceStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.CreditStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TreasuryStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.FintechStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<ProcessTATResponse>>(
                    new PagedResponse<ProcessTATResponse>(
                    approvals,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ProcessTATResponse>>(error));
            }
        }

        [HttpPost("processes/tat-report")]
        public async Task<IActionResult> GetTatReport([FromBody] GeneralRequest request)
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
                    return Ok(new GrcResponse<PagedResponse<ProcessTATResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var approvals = await _approvalService.GetAllAsync(true, p => p.Process);
                if (approvals == null || !approvals.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No review process records found"
                    );

                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<ProcessTATResponse>>(new List<ProcessTATResponse>()));
                }

                List<ProcessTATResponse> tatApprovals = new();
                var records = approvals.ToList();
                records.ForEach(approval => {
                    var hod = CalculateDays(approval.HeadOfDepartmentStart, approval.HeadOfDepartmentEnd);
                    var risk = CalculateDays(approval.RiskStart, approval.RiskEnd);
                    var compliance = CalculateDays(approval.ComplianceStart, approval.ComplianceEnd);
                    var bop = CalculateDays(approval.BranchOperationsStatusStart, approval.BranchOperationsStatusEnd);
                    var credit = CalculateDays(approval.CreditStart, approval.CreditEnd);
                    var treasury = CalculateDays(approval.TreasuryStart, approval.TreasuryEnd);
                    var fintech = CalculateDays(approval.FintechStart, approval.FintechEnd);
                    tatApprovals.Add(new ProcessTATResponse {
                        Id = approval.Id,
                        ProcessId = approval.ProcessId,
                        RequestDate = approval.RequestDate,
                        ProcessName = approval.Process.ProcessName ?? string.Empty,
                        ProcessStatus = GetStatus(approval),
                        HodCount = hod,
                        HodStatus = approval.HeadOfDepartmentStatus,
                        HodComment = approval.HeadOfDepartmentComment ?? string.Empty,
                        RiskCount = risk,
                        RiskStatus = approval.RiskStatus ?? string.Empty,
                        RiskComment = approval.RiskComment ?? string.Empty,
                        ComplianceCount = compliance,
                        ComplianceStatus = approval.ComplianceStatus ?? string.Empty,
                        ComplianceComment = approval.ComplianceComment ?? string.Empty,
                        BopCount = bop,
                        BropStatus = approval.BranchOperationsStatus ?? string.Empty,
                        BropComment = approval.BranchManagerComment ?? string.Empty,
                        CreditCount = credit,
                        CreditStatus = approval.CreditStatus ?? string.Empty,
                        CreditComment = approval.CreditComment ?? string.Empty,
                        TreasuryCount = treasury,
                        TreasuryStatus = approval.TreasuryStatus ?? string.Empty,
                        TreasuryComment = approval.TreasuryComment ?? string.Empty,
                        FintechCount = fintech,
                        FintechStatus = approval.FintechStatus ?? string.Empty,
                        FintechComment = approval.FintechComment ?? string.Empty,
                        TotalCount = hod + risk + compliance + bop + credit + treasury + fintech
                    });
                });

                return Ok(new GrcResponse<List<ProcessTATResponse>>(tatApprovals));
            }
            catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<List<ProcessTATResponse>>(error));
            }
        }

        #endregion

        #region New Process Endpoints

        [HttpPost("processes/processes-new")]
        public async Task<IActionResult> GetNewProcess([FromBody] ListRequest request)
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
                    return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _processService.PageNewProcessesAsync(request.PageIndex, 
                    request.PageSize, false, p => p.Unit, p => p.Owner, p => p.Responsible, p => p.ProcessType);
                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No new process records found"
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
                if (records != null && records.Any())
                {
                    records.ForEach(register => processes.Add(new()
                    {
                        Id = register.Id,
                        ProcessName = register.ProcessName ?? string.Empty,
                        Description = register.Description ?? string.Empty,
                        CurrentVersion = register.CurrentVersion ?? string.Empty,
                        EffectiveDate = register.EffectiveDate,
                        LastUpdated = register.LastUpdated,
                        FileName = register.FileName ?? string.Empty,
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
                        UnlockReason = register.UnlockReason,
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

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    processes = processes.Where(u =>
                        (u.ProcessName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TypeName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.UnitName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.OwnerName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Responsibile?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(
                    new PagedResponse<ProcessRegisterResponse>(
                    processes,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(error));
            }
        }

        [HttpPost("processes/new-approval-retrieve")]
        public async Task<IActionResult> GetNewProcessApproval([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get new process approval by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessApprovalResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request new process approval ID is required",
                        "Invalid request process approval ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessApprovalResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var process = await _processService.GetAsync(p => p.Id == request.RecordId, false);
                if (process == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Operations process not found",
                        "No operations process matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessApprovalResponse>(error));
                }

                var approval = await _approvalService.GetAsync(a => a.ProcessId == request.RecordId, false);
                ProcessApprovalResponse approvalRecord;
                if (approval == null) {
                    approvalRecord = new ProcessApprovalResponse {
                        Id = 0,
                        ProcessId = process.Id,
                        RequestDate = DateTime.Now,
                        ProcessName = process.ProcessName ?? string.Empty,
                        ProcessDescription = process.Description ?? string.Empty,
                        HeadOfDepartmentStart = DateTime.Now,
                        HeadOfDepartmentEnd = null,
                        HeadOfDepartmentStatus = null,
                        HeadOfDepartmentComment = "Approval record not found",
                        RiskStart = null,
                        RiskEnd = null,
                        RiskStatus = "UNCLASSIFIED",
                        RiskComment = "Approval record not found",
                        ComplianceStart = null,
                        ComplianceEnd = null,
                        ComplianceStatus = "UNCLASSIFIED",
                        ComplianceComment = "Approval record not found",
                        BranchOperationsStatusStart = null,
                        BranchOperationsStatusEnd = null,
                        BranchOperationsStatus = "UNCLASSIFIED",
                        BranchManagerComment = "Approval record not found",
                        CreditStart = null,
                        CreditEnd = null,
                        CreditStatus = "UNCLASSIFIED",
                        CreditComment = "Approval record not found",
                        TreasuryStart = null,
                        TreasuryEnd = null,
                        TreasuryStatus = "UNCLASSIFIED",
                        TreasuryComment = "Approval record not found",
                        FintechStart = null,
                        FintechEnd = null,
                        FintechStatus = "UNCLASSIFIED",
                        FintechComment = "Approval record not found",
                        IsDeleted = false
                    };
                } else {
                    approvalRecord = new ProcessApprovalResponse {
                        Id = approval.Id,
                        ProcessId = process.Id,
                        RequestDate = approval.RequestDate,
                        ProcessName = process.ProcessName ?? string.Empty,
                        ProcessDescription = process.Description ?? string.Empty,
                        HeadOfDepartmentStart = approval.HeadOfDepartmentStart,
                        HeadOfDepartmentEnd = approval.HeadOfDepartmentEnd,
                        HeadOfDepartmentStatus = approval.HeadOfDepartmentStatus ?? string.Empty,
                        HeadOfDepartmentComment = approval.HeadOfDepartmentComment ?? string.Empty,
                        RiskStart = approval.RiskStart,
                        RiskEnd = approval.RiskEnd,
                        RiskStatus = approval.RiskStatus ?? string.Empty,
                        RiskComment = approval.RiskComment ?? string.Empty,
                        ComplianceStart = approval.ComplianceStart,
                        ComplianceEnd = approval.ComplianceEnd,
                        ComplianceStatus = approval.ComplianceStatus ?? string.Empty,
                        ComplianceComment = approval.ComplianceComment ?? string.Empty,
                        BranchOperationsStatusStart = approval.BranchOperationsStatusStart,
                        BranchOperationsStatusEnd = approval.BranchOperationsStatusEnd,
                        BranchOperationsStatus = approval.BranchOperationsStatus ?? string.Empty,
                        BranchManagerComment = approval.BranchManagerComment ?? string.Empty,
                        CreditStart = approval.CreditStart,
                        CreditEnd = approval.CreditEnd,
                        CreditStatus = approval.CreditStatus ?? string.Empty,
                        CreditComment = approval.CreditComment ?? string.Empty,
                        TreasuryStart = approval.TreasuryStart,
                        TreasuryEnd = approval.TreasuryEnd,
                        TreasuryStatus = approval.TreasuryStatus ?? string.Empty,
                        TreasuryComment = approval.TreasuryComment ?? string.Empty,
                        FintechStart = approval.FintechStart,
                        FintechEnd = approval.FintechEnd,
                        FintechStatus = approval.FintechStatus ?? string.Empty,
                        FintechComment = approval.FintechComment ?? string.Empty,
                        IsDeleted = approval.IsDeleted
                    };
                }

                return Ok(new GrcResponse<ProcessApprovalResponse>(approvalRecord));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ProcessTATResponse>(error));
            }
        }

        #endregion

        #region Review Process Endpoints

        [HttpPost("processes/processes-reviews")]
        public async Task<IActionResult> GetReviewProcess([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _processService.PageReviewProcessesAsync(request.PageIndex, 
                    request.PageSize, true, p => p.Unit, p => p.Owner, p => p.Responsible,p => p.ProcessType);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No review process records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(
                        new PagedResponse<ProcessRegisterResponse>(
                        new List<ProcessRegisterResponse>(),
                        0,pageResult.Page, pageResult.Size)));
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
                        UnlockReason = register.UnlockReason,
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

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    processes = processes.Where(u =>
                        (u.ProcessName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TypeName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.UnitName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.OwnerName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Responsibile?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(
                    new PagedResponse<ProcessRegisterResponse>(
                    processes,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ProcessRegisterResponse>>(error));
            }
        }

        [HttpPost("processes/initiate-review")]
        public async Task<IActionResult> InitiateReview([FromBody] InitiateReviewRequest request) {
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

                var initiate = new InitiateRequest() {
                    Id = request.Id,
                    ProcessStatus = request.ProcessStatus,
                    UnlockReason = request.UnlockReason,
                };

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null)
                {
                    initiate.ModifiedBy = currentUser.Username;
                }
                else
                {
                    initiate.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                initiate.ModifiedOn = DateTime.Now;
                var response = new GeneralResponse();

                //..initiate operations process review
                var isInitiated = await _processService.InitiateReviewAsync(initiate);
                if (!isInitiated) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Unable to initiate review.";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                string msg = "Processes sent for review";
                var mailSettings = await _mailService.GetMailSettingsAsync();
                if (mailSettings is null) {
                    msg += ". Mail settings not found. No mail sent";
                } else {

                    //..get Head of operations details
                    var hod = await _officersService.GetAsync(o => o.ContactPosition == "Head of Operation & Services");
                    if (hod is null) {
                        msg += ". Head Of Operations Contacts not found. Mail not sent";
                    } else {
                        var hodName = (hod.ContactEmail ?? string.Empty).Trim();
                        var hodEmail = (hod.ContactEmail ?? string.Empty).Trim();
                        if (!string.IsNullOrEmpty(hodName) && !string.IsNullOrEmpty(hodEmail)) {
                            var (sent, subject, mail) = MailHandler.GenerateMail(Logger, mailSettings.MailSender, hodName, hodEmail, mailSettings.CopyTo, request.ProcessName, mailSettings.NetworkPort, mailSettings.SystemPassword);
                            if (sent) {
                                await _mailService.InsertMailAsync(new Data.Entities.System.MailRecord() {
                                    SentToEmail = hodEmail,
                                    CCMail = mailSettings.CopyTo,
                                    Subject = subject,
                                    Mail = mail,
                                    ApprovalId = request.Id,
                                    IsDeleted = false,
                                    CreatedBy = "SYSTEM",
                                    CreatedOn = DateTime.Now,
                                    LastModifiedBy = "SYSTEM",
                                    LastModifiedOn = DateTime.Now,
                                });
                            }
                        } else {
                            msg += ". Head Of Operations Contacts not found. Mail not sent";
                        }
                    }
                }

                response.Status = true;
                response.StatusCode = (int)ResponseCodes.SUCCESS;
                response.Message = msg;
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                return Ok(new GrcResponse<GeneralResponse>(response));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("processes/hold-review")]
        public async Task<IActionResult> HoldProcessReview([FromBody] HoldProcessRequest request) {
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
                if (!await _processService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "Operations Process record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                var initiate = new HoldRequest() {
                    Id = request.Id,
                    ProcessId = request.ProcessId,
                    ProcessStatus = request.ProcessStatus,
                    HoldReason = request.HoldReason,
                };

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    initiate.ModifiedBy = currentUser.Username;
                } else {
                    initiate.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                initiate.ModifiedOn = DateTime.Now;
                var response = new GeneralResponse();

                //..initiate operations process review
                var isInitiated = await _processService.HoldProcessReviewAsync(initiate);
                if (!isInitiated) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Unable to hold process review.";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                response.Status = true;
                response.StatusCode = (int)ResponseCodes.SUCCESS;
                response.Message = $"Process '{request.ProcessName}'has been put on hold";
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("processes/approval-request")]
        public async Task<IActionResult> RequestProcessApproval([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _processService.ExistsAsync(r => r.Id == request.RecordId && !r.IsDeleted)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Process Not Found!! No Process record found";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..get username
                string requestedBy;
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    requestedBy = currentUser.Username;
                } else {
                    requestedBy = $"{request.UserId}";
                }

                //..request process approval
                var (status, processName, processId) = await _processService.RequestApprovalAsync(request.RecordId, requestedBy);
                if (!status) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to submit process approval",
                        "An error occurred! could not submit process approval");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..send mail to HOD
                string msg = "Processes sent for approval";
                var mailSettings = await _mailService.GetMailSettingsAsync();
                if (mailSettings is null) {
                    msg += ". Mail settings not found. No mail sent";
                } else {

                    //..get Head of operations details
                    var hod = await _officersService.GetAsync(o => o.ContactPosition == "Head of Operation & Services");
                    if (hod is null) {
                        msg += ". Head Of Operations Contacts not found. Mail not sent";
                    } else {
                        var hodName = (hod.ContactEmail ?? string.Empty).Trim();
                        var hodEmail = (hod.ContactEmail ?? string.Empty).Trim();
                        if (!string.IsNullOrEmpty(hodName) && !string.IsNullOrEmpty(hodEmail)) {
                            var (sent, subject, mail) = MailHandler.GenerateMail(Logger, mailSettings.MailSender, hodName, hodEmail, mailSettings.CopyTo, processName, mailSettings.NetworkPort, mailSettings.SystemPassword);
                            if (sent) {
                                await _mailService.InsertMailAsync(new MailRecord() {
                                    SentToEmail = hodEmail,
                                    CCMail = mailSettings.CopyTo,
                                    Subject = subject,
                                    Mail = mail,
                                    ApprovalId = request.RecordId,
                                    IsDeleted = false,
                                    CreatedBy = "SYSTEM",
                                    CreatedOn = DateTime.Now,
                                    LastModifiedBy = "SYSTEM",
                                    LastModifiedOn = DateTime.Now,
                                });
                            }
                        } else {
                            msg += ". Head Of Operations Contacts not found. Mail not sent";
                        }
                    }
                }

                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status, Message=msg }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error submitting process approval by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Process Approvals Endpoints

        [HttpPost("processes/approval-status")]
        public async Task<IActionResult> GetProcessApprovalStatus([FromBody] ListRequest request) {

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
                    return Ok(new GrcResponse<PagedResponse<ProcessApprovalResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _approvalService.PageProcessApprovalStatusAsync(request.PageIndex, request.PageSize, true, p => p.Process);
                if (pageResult.Entities == null || !pageResult.Entities.Any())
                {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No review process records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");

                    return Ok(new GrcResponse<PagedResponse<ProcessApprovalResponse>>(
                        new PagedResponse<ProcessApprovalResponse>(
                        new List<ProcessApprovalResponse>(),
                        0, pageResult.Page, pageResult.Size)));
                }

                List<ProcessApprovalResponse> approvals = new();

                var records = pageResult.Entities;
                if (records is not null && records.Any()) {
                    foreach (var approval in records) {
                        var process = approval.Process;
                        if (process is null) {
                            continue;
                        }
                        
                        //..check if proces required reviews
                        bool bopRequired = process.NeedsBranchReview ?? false;
                        bool treasRequired = process.NeedsTreasuryReview ?? false;
                        bool creditRequired = process.NeedsCreditReview ?? false;
                        bool fintechRequired = process.NeedsFintechReview ?? false;

                        //..check if not fully approved
                        bool notApproved = string.IsNullOrWhiteSpace(approval.HeadOfDepartmentStatus) || !approval.HeadOfDepartmentStatus.Equals("APPROVED") ||
                                           string.IsNullOrWhiteSpace(approval.RiskStatus) || !approval.RiskStatus.Equals("APPROVED") ||
                                           string.IsNullOrWhiteSpace(approval.ComplianceStatus) || !approval.ComplianceStatus.Equals("APPROVED");

                        //..check if any review still required
                        bool extraReviewRequired = bopRequired || treasRequired || creditRequired || fintechRequired;

                        //..skip if nothing required
                        if (!notApproved && !extraReviewRequired) {
                            continue;
                        }
                        
                        approvals.Add(MapToResponse(approval));
                    }
                }


                //..filter approvals
                if (approvals.Any() && !string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    string search = request.SearchTerm.ToLower();

                    approvals = approvals.Where(u =>
                        (u.ProcessName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.HeadOfDepartmentStatus?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.RiskStatus?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.ComplianceStatus?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.BranchOperationsStatus?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.CreditStatus?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TreasuryStatus?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.FintechStatus?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }


                //..response
                return Ok(new GrcResponse<PagedResponse<ProcessApprovalResponse>>(
                    new PagedResponse<ProcessApprovalResponse>(
                        approvals,
                        pageResult.Count,
                        pageResult.Page,
                        pageResult.Size
                    )
                ));

            } catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ProcessApprovalResponse>>(error));
            }
        }

        [HttpPost("processes/approval-retrieve")]
        public async Task<IActionResult> GetProcessApproval([FromBody] IdRequest request)
        {
            try
            {
                Logger.LogActivity("Get process approval by ID", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessApprovalResponse>(error));
                }

                if (request.RecordId == 0)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request process approval ID is required",
                        "Invalid request process approval ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessApprovalResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var approval = await _approvalService.GetAsync(p => p.Id == request.RecordId, true, p => p.Process);
                if (approval == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Operations process not found",
                        "No operations process matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ProcessApprovalResponse>(error));
                }

                var tatRecord = new ProcessApprovalResponse
                {
                    Id = approval.Id,
                    ProcessId = approval.ProcessId,
                    RequestDate = approval.RequestDate,
                    ProcessName = approval.Process.ProcessName ?? string.Empty,
                    ProcessDescription = approval.Process.Description ?? string.Empty,
                    HeadOfDepartmentStart = approval.HeadOfDepartmentStart,
                    HeadOfDepartmentEnd = approval.HeadOfDepartmentEnd,
                    HeadOfDepartmentStatus = approval.HeadOfDepartmentStatus ?? string.Empty,
                    HeadOfDepartmentComment = approval.HeadOfDepartmentComment ?? string.Empty,
                    RiskStart = approval.RiskStart,
                    RiskEnd = approval.RiskEnd,
                    RiskStatus = approval.RiskStatus ?? string.Empty,
                    RiskComment = approval.RiskComment ?? string.Empty,
                    ComplianceStart = approval.ComplianceStart,
                    ComplianceEnd = approval.ComplianceEnd,
                    ComplianceStatus = approval.ComplianceStatus ?? string.Empty,
                    ComplianceComment = approval.ComplianceComment ?? string.Empty,
                    BranchOperationsStatusStart = approval.BranchOperationsStatusStart,
                    BranchOperationsStatusEnd = approval.BranchOperationsStatusEnd,
                    BranchOperationsStatus = approval.BranchOperationsStatus ?? string.Empty,
                    BranchManagerComment = approval.BranchManagerComment ?? string.Empty,
                    CreditStart = approval.CreditStart,
                    CreditEnd = approval.CreditEnd,
                    CreditStatus = approval.CreditStatus ?? string.Empty,
                    CreditComment = approval.CreditComment ?? string.Empty,
                    TreasuryStart = approval.TreasuryStart,
                    TreasuryEnd = approval.TreasuryEnd,
                    TreasuryStatus = approval.TreasuryStatus ?? string.Empty,
                    TreasuryComment = approval.TreasuryComment ?? string.Empty,
                    FintechStart = approval.FintechStart,
                    FintechEnd = approval.FintechEnd,
                    FintechStatus = approval.FintechStatus ?? string.Empty,
                    FintechComment = approval.FintechComment ?? string.Empty,
                    IsDeleted = approval.IsDeleted
                };

                return Ok(new GrcResponse<ProcessApprovalResponse>(tatRecord));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ProcessTATResponse>(error));
            }
        }

        [HttpPost("processes/update-approval")]
        public async Task<IActionResult> UpdateApproval([FromBody] ApprovalRequest request)
        {
            try
            {
                Logger.LogActivity("Update process approval", "INFO");
                if (request == null)
                {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The process approval record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _approvalService.ExistsAsync(r => r.Id == request.Id))
                {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "Process approval record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null)
                {
                    request.ModifiedBy = currentUser.Username;
                }
                else
                {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..update process approval
                var (isApproved, stage) = await _approvalService.ApproveProcessAsync(request, true);
                var response = new GeneralResponse();
                if (isApproved) {
                    string msg = $"Processes has passed stage {(int)stage} approval";

                    var mailSettings = await _mailService.GetMailSettingsAsync();
                    if (mailSettings is null) {
                        msg += ". Mail settings not found. No mail sent";
                    } else {
                        //..get resposible manager
                        var (receiverName, receiverMail) = await GetMailReceiverInfo(stage, _officersService);
                        if (!string.IsNullOrEmpty(receiverName) && !string.IsNullOrEmpty(receiverMail)) {
                            var hodName = (receiverName ?? string.Empty).Trim();
                            var hodEmail = (receiverMail ?? string.Empty).Trim();
                            if (!string.IsNullOrEmpty(hodName) && !string.IsNullOrEmpty(hodEmail)) {
                                var (sent, subject, mail) = MailHandler.GenerateMail(Logger, mailSettings.MailSender, hodName, hodEmail, mailSettings.CopyTo, request.ProcessName, mailSettings.NetworkPort, mailSettings.SystemPassword);
                                if (sent) {
                                    await _mailService.InsertMailAsync(new MailRecord() {
                                        SentToEmail = hodEmail,
                                        CCMail = mailSettings.CopyTo,
                                        Subject = subject,
                                        Mail = mail,
                                        ApprovalId = request.Id,
                                        IsDeleted = false,
                                        CreatedBy = "SYSTEM",
                                        CreatedOn = DateTime.Now,
                                        LastModifiedBy = "SYSTEM",
                                        LastModifiedOn = DateTime.Now,
                                    });
                                }
                            } else {
                                msg += $". Mail has not been sent to {stage.GetDescription()}, you need to send the mail manually";
                            }
                        } else {
                            msg += $". Mail has not been sent to {stage.GetDescription()}, you need to send the mail manually";
                        }
                    }

                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = msg;
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update process tag record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Private Methods

        private static async Task<(string, string)> GetMailReceiverInfo(ApprovalStage stage, IResponsebilityService respService) {
            Responsebility responsible = stage switch {
                ApprovalStage.HOD => await respService.GetAsync(o => o.ContactPosition == "Head of Operation & Services"),
                ApprovalStage.RISK => await respService.GetAsync(o => o.ContactPosition == "Head Risk"),
                ApprovalStage.COMP => await respService.GetAsync(o => o.ContactPosition == "Head Compliance"),
                ApprovalStage.BOM => await respService.GetAsync(o => o.ContactPosition == "Branch Operations"),
                ApprovalStage.TREA => await respService.GetAsync(o => o.ContactPosition == "Head Treasury"),
                ApprovalStage.CRT => await respService.GetAsync(o => o.ContactPosition == "Head Of Department Credit"),
                ApprovalStage.FIN => await respService.GetAsync(o => o.ContactPosition == "Head Of Department Fintech"),
                _ => null,
            };

            if (responsible == null) {
                return (string.Empty, string.Empty);
            }

            return (responsible.ContactName, responsible.ContactEmail);
        }

        private static string GetStatus(ProcessApproval approval) {
            string status = "Pending";
            if (approval.FintechStatus == "REJECTED" ||
                approval.TreasuryStatus == "REJECTED" ||
                approval.CreditStatus == "REJECTED" ||
                approval.BranchOperationsStatus == "REJECTED" ||
                approval.ComplianceStatus == "REJECTED" ||
                approval.RiskStatus == "REJECTED" ||
                approval.HeadOfDepartmentStatus == "REJECTED") {
                status = "Rejected";
            }
            else if (approval.FintechStatus == "APPROVED" &&
                     approval.TreasuryStatus == "APPROVED" &&
                     approval.CreditStatus == "APPROVED" &&
                     approval.BranchOperationsStatus == "APPROVED" &&
                     approval.ComplianceStatus == "APPROVED" &&
                     approval.RiskStatus == "APPROVED" &&
                     approval.HeadOfDepartmentStatus == "APPROVED") {
                status = "Approved";
            }
            else if (approval.FintechStatus == "ONHOLD" &&
                     approval.TreasuryStatus == "ONHOLD" &&
                     approval.CreditStatus == "ONHOLD" &&
                     approval.BranchOperationsStatus == "ONHOLD" &&
                     approval.ComplianceStatus == "ONHOLD" &&
                     approval.RiskStatus == "ONHOLD" &&
                     approval.HeadOfDepartmentStatus == "ONHOLD")
            {
                status = "Onhold";
            }

            return status;
        }

        private static int CalculateDays(DateTime? startDate, DateTime? endDate) {
            if (startDate is null)
                return 0;

            DateTime finalDate = endDate ?? DateTime.Now;
            return Math.Max(0, (finalDate - startDate.Value).Days);
        }

        private static ProcessApprovalResponse MapToResponse(ProcessApproval approval) {
            return new ProcessApprovalResponse {
                Id = approval.Id,
                ProcessId = approval.ProcessId,
                RequestDate = approval.RequestDate,
                ProcessName = approval.Process?.ProcessName ?? string.Empty,
                ProcessDescription = approval.Process?.Description ?? string.Empty,

                HeadOfDepartmentStart = approval.HeadOfDepartmentStart,
                HeadOfDepartmentEnd = approval.HeadOfDepartmentEnd,
                HeadOfDepartmentStatus = approval.HeadOfDepartmentStatus ?? string.Empty,
                HeadOfDepartmentComment = approval.HeadOfDepartmentComment ?? string.Empty,

                RiskStart = approval.RiskStart,
                RiskEnd = approval.RiskEnd,
                RiskStatus = approval.RiskStatus ?? string.Empty,
                RiskComment = approval.RiskComment ?? string.Empty,

                ComplianceStart = approval.ComplianceStart,
                ComplianceEnd = approval.ComplianceEnd,
                ComplianceStatus = approval.ComplianceStatus ?? string.Empty,
                ComplianceComment = approval.ComplianceComment ?? string.Empty,

                BranchOperationsStatusStart = approval.BranchOperationsStatusStart,
                BranchOperationsStatusEnd = approval.BranchOperationsStatusEnd,
                BranchOperationsStatus = approval.BranchOperationsStatus ?? string.Empty,
                BranchManagerComment = approval.BranchManagerComment ?? string.Empty,

                CreditStart = approval.CreditStart,
                CreditEnd = approval.CreditEnd,
                CreditStatus = approval.CreditStatus ?? string.Empty,
                CreditComment = approval.CreditComment ?? string.Empty,

                TreasuryStart = approval.TreasuryStart,
                TreasuryEnd = approval.TreasuryEnd,
                TreasuryStatus = approval.TreasuryStatus ?? string.Empty,
                TreasuryComment = approval.TreasuryComment ?? string.Empty,

                FintechStart = approval.FintechStart,
                FintechEnd = approval.FintechEnd,
                FintechStatus = approval.FintechStatus ?? string.Empty,
                FintechComment = approval.FintechComment ?? string.Empty,

                IsDeleted = approval.IsDeleted
            };
        }
        
        #endregion

    }
}

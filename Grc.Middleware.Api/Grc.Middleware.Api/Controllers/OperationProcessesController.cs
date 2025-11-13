using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
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
        private readonly IProcessTypeService _typeService;
        private readonly IDepartmentsService _departmentService;
        private readonly ISystemAccessService _accessService;
        private readonly IProcessApprovalService _approvalService;

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
                                            IProcessTypeService typeService,
                                            IProcessApprovalService approvalService,
                                            ISystemAccessService accessService,
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
            _groupService = groupService;
            _typeService = typeService;
            _departmentService = departmentService;
            _accessService = accessService;
            _approvalService = approvalService;
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

                var group = await _groupService.GetAsync(p => p.Id == request.RecordId, true, p => p.Processes);
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
                    .Select(p => new ProcessRegisterResponse
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
                var pageResult = await _groupService.PageAllAsync(request.PageIndex, request.PageSize, true);
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
                        Processes = new List<ProcessRegisterResponse>()
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

                var tag = await _tagService.GetAsync(p => p.Id == request.RecordId, true, p => p.Processes);
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
                var pageResult = await _tagService.PageAllAsync(request.PageIndex, request.PageSize, true);
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
        public async Task<IActionResult> GetProcessTat([FromBody] IdRequest request)
        {
            return Ok();
        }

        [HttpPost("processes/tat-all")]
        public async Task<IActionResult> GetProcessTat([FromBody] ListRequest request)
        {
            return Ok();
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
                    request.PageSize, true, p => p.Unit, p => p.Owner, p => p.Responsible, p => p.ProcessType);
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

        #endregion

        #region Process Approvals Endpoints

        [HttpPost("processes/approval-status")]
        public async Task<IActionResult> GetProcessApprovalStatus([FromBody] ListRequest request)
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
                if (records != null && records.Any())
                {
                    records.ForEach(approval => approvals.Add(new ProcessApprovalResponse()
                    {
                        Id = approval.Id,
                        ProcessName = approval.Process.ProcessName ?? string.Empty,
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
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    approvals = approvals.Where(u =>
                        (u.ProcessName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<ProcessApprovalResponse>>(
                    new PagedResponse<ProcessApprovalResponse>(
                    approvals,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            }
            catch (Exception ex)
            {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ProcessApprovalResponse>>(error));
            }
        }

        #endregion

    }
}

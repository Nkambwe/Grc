using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Compliance.Regulations;
using Grc.Middleware.Api.Services.Compliance.Support;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Text.Json;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class RegulatoryController : GrcControllerBase {

       
        private readonly ISystemAccessService _accessService;
        private readonly IRegulatoryDocumentTypeService _documentTypeService;
        private readonly IRegulatoryDocumentService _regulatoryDocuments;
        private readonly IFrequencyService _frequencyService;
        private readonly IRegulatoryCategoryService _categoryService;
        private readonly IRegulatoryReturnService _returnsService;
        private readonly IRegulatoryTypeService _regulatoryType;
        private readonly IAuthorityService _authorityService;
        private readonly IResponsebilityService _responsibilityService;
        private readonly IReturnTypeService _returnTypeService;
        private readonly IStatutoryArticleService _articleService;
        private readonly IStatutoryRegulationService _regulatoryService;
        public RegulatoryController(IObjectCypher cypher,
            IServiceLoggerFactory loggerFactory, 
            IMapper mapper, 
            ICompanyService companyService,
            IAuthorityService authorityService,
            ISystemAccessService accessService,
            IFrequencyService frequencyService,
            IRegulatoryCategoryService categoryService,
            IRegulatoryReturnService returnsService,
            IRegulatoryTypeService regulatoryType,
            IRegulatoryDocumentService regulatoryDocuments,
            IRegulatoryDocumentTypeService documentTypeService,
            IResponsebilityService responsibilityService,
            IReturnTypeService returnTypeService,
            IStatutoryArticleService articleService,
            IStatutoryRegulationService regulatoryService,
            IEnvironmentProvider environment, 
            IErrorNotificationService errorService, 
            ISystemErrorService systemErrorService) 
            : base(cypher, loggerFactory, mapper, companyService, environment, errorService, systemErrorService) {
            _authorityService = authorityService;
            _accessService = accessService;
            _frequencyService = frequencyService;
            _categoryService = categoryService;
            _returnsService = returnsService;
            _returnTypeService = returnTypeService;
            _regulatoryType = regulatoryType;
            _regulatoryDocuments = regulatoryDocuments;
            _documentTypeService = documentTypeService;
            _articleService = articleService;
            _regulatoryService = regulatoryService;
            _responsibilityService = responsibilityService;
        }

        #region Policy Documents

        [HttpPost("registers/support-items")]
        public async Task<IActionResult> GetDocumentSupportItems([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PolicySupportResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                //..get support data
                var _supportItemsList = await _regulatoryService.GetSupportItemsAsync(false);
                return Ok(new GrcResponse<PolicySupportResponse>(_supportItemsList));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PolicySupportResponse>(error));
            }
        }

        [HttpPost("registers/document-retrieve")]
        public async Task<IActionResult> GetDocument([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get Regulatory policy by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DocumentResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request regulation ID is required",
                        "Invalid regulation request ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DocumentResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var register = await _regulatoryDocuments.GetAsync(d => d.Id == request.RecordId, false, d => d.Owner, d => d.DocumentType, d => d.Frequency);
                if (register == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Statutory Document not found",
                        "No statutory document matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DocumentResponse>(error));
                }

                //..return process register data
                var registerRecord = new DocumentResponse {
                    Id = register.Id,
                    DocumentName = register.DocumentName ?? string.Empty,
                    Status = register.Status ?? string.Empty,
                    IsAligned = register.PolicyAligned,
                    FrequencyId = register.FrequencyId,
                    IsLocked = register.IsLocked ?? false,
                    FrequencyName = register.Frequency?.FrequencyName ?? string.Empty,
                    DocumentTypeId = register.DocumentTypeId,
                    DocumentTypeName = register.DocumentType?.DocumentType ?? string.Empty,
                    ResponsibilityId = register.ResponsibilityId,
                    ResponsibilityName = register.Owner?.ContactPosition ?? string.Empty,
                    DepartmentId = register.Owner?.DepartmentId ?? 0,
                    DepartmentName = register.Owner?.Department?.DepartmentName ?? string.Empty,
                    Comments = register.Comments ?? string.Empty,
                    ApprovedBy = register.ApprovedBy ?? string.Empty,
                    LastRevisionDate = register.LastRevisionDate,
                    NextRevisionDate = register.NextRevisionDate
                };

                return Ok(new GrcResponse<DocumentResponse>(registerRecord));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<DocumentResponse>(error));
            }
        }

        [HttpPost("registers/document-list")]
        public async Task<IActionResult> GetDocumentList([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DocumentResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var documents = await _regulatoryDocuments.GetAllAsync(false, d => d.Owner, d => d.DocumentType, d => d.Frequency);

                if (documents == null || !documents.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No statutory document records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<DocumentResponse>>(new List<DocumentResponse>()));
                }

                List<DocumentResponse> documentList = new();
                documents.ToList().ForEach(register => documentList.Add(new DocumentResponse() {
                    Id = register.Id,
                    DocumentName = register.DocumentName ?? string.Empty,
                    Status = register.Status ?? string.Empty,
                    IsAligned = register.PolicyAligned,
                    IsLocked = register.IsLocked ?? false,
                    IsDeleted = register.IsDeleted,
                    FrequencyId = register.FrequencyId,
                    FrequencyName = register.Frequency?.FrequencyName ?? string.Empty,
                    DocumentTypeId = register.DocumentTypeId,
                    DocumentTypeName = register.DocumentType?.DocumentType ?? string.Empty,
                    ResponsibilityId = register.ResponsibilityId,
                    ResponsibilityName = register.Owner?.ContactPosition ?? string.Empty,
                    DepartmentId = register.Owner?.DepartmentId ?? 0,
                    DepartmentName = register.Owner?.Department?.DepartmentName ?? string.Empty,
                    Comments = register.Comments ?? string.Empty,
                    ApprovedBy = register.ApprovedBy ?? string.Empty,
                    ApprovalDate = register.ApprovalDate ?? DateTime.MinValue,
                    LastRevisionDate = register.LastRevisionDate,
                    NextRevisionDate = register.NextRevisionDate
                }));

                return Ok(new GrcResponse<List<DocumentResponse>>(documentList));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<List<DocumentResponse>>(error));
            }
        }

        [HttpPost("registers/paged-document-list")]
        public async Task<IActionResult> GetPagedDocumentList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DocumentResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _regulatoryDocuments.PageAllAsync(request.PageIndex, 
                                                                         request.PageSize, 
                                                                         false, 
                                                                         d => d.Owner,
                                                                         d => d.Owner.Department, 
                                                                         d => d.DocumentType, 
                                                                         d => d.Frequency);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No operation process records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DocumentResponse>>(new PagedResponse<DocumentResponse>(new List<DocumentResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<DocumentResponse> documents = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(register => documents.Add(new() {
                        Id = register.Id,
                        DocumentName = register.DocumentName ?? string.Empty,
                        Status = register.Status ?? string.Empty,
                        IsAligned = register.PolicyAligned,
                        IsLocked = register.IsLocked ?? false,
                        FrequencyId = register.FrequencyId,
                        FrequencyName = register.Frequency?.FrequencyName ?? string.Empty,
                        DocumentTypeId = register.DocumentTypeId,
                        DocumentTypeName = register.DocumentType?.DocumentType ?? string.Empty,
                        ResponsibilityId = register.ResponsibilityId,
                        ResponsibilityName = register.Owner?.ContactPosition ?? string.Empty,
                        DepartmentId = register.Owner?.DepartmentId ?? 0,
                        DepartmentName = register.Owner?.Department?.DepartmentName ?? string.Empty,
                        Comments = register.Comments ?? string.Empty,
                        ApprovedBy = register.ApprovedBy ?? string.Empty,
                        ApprovalDate = register.ApprovalDate ?? DateTime.MinValue,
                        LastRevisionDate = register.LastRevisionDate,
                        NextRevisionDate = register.NextRevisionDate
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    documents = documents.Where(u =>
                        (u.DocumentName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Status?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<DocumentResponse>>(
                    new PagedResponse<DocumentResponse>(
                    documents,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<DocumentResponse>>(error));
            }
        }

        [HttpPost("registers/create-document")]
        public async Task<IActionResult> CreateDocument([FromBody] PolicyDocumentRequest request) {
            try {
                Logger.LogActivity("Creating new policy document", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The policy document record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.DocumentName) && !string.IsNullOrWhiteSpace(request.DocumentStatus)) {
                    if (await _regulatoryDocuments.ExistsAsync(r => r.DocumentName == request.DocumentName)) {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another Policy document found with similar name"
                        );
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The document name is required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.CreatedBy = currentUser.Username;
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.CreatedBy = $"{request.UserId}";
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.CreatedOn = DateTime.Now;
                request.ModifiedOn = DateTime.Now;

                //..create policy document
                var result = await _regulatoryDocuments.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Policy document saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save policy document record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-document")]
        public async Task<IActionResult> UpdateDocument([FromBody] PolicyDocumentRequest request) {
            try {
                Logger.LogActivity("Update policy document", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The policy document record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _regulatoryDocuments.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(
                        ResponseCodes.NOTFOUND,
                        "Record Not Found",
                        "Policy document record not found in the database"
                    );

                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.ModifiedOn = DateTime.Now;

                //..update policy document
                var result = await _regulatoryDocuments.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Policy document updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update policy document record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-document")]
        public async Task<IActionResult> DeleteDocument([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _regulatoryDocuments.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Policy document Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete policy document
                var status = await _regulatoryDocuments.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to delete policy document",
                        "An error occurred! could delete policy document");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting policy document by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/lock-document")]
        public async Task<IActionResult> LockDocument([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                var record = await _regulatoryDocuments.GetAsync(r => r.Id == request.RecordId);   
                if (record == null) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Policy document Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                if (record.Status.Equals("UPTODATE") || record.Status.Equals("NA")) {
                    //..lock policy document
                    var status = await _regulatoryDocuments.LockAsync(request);
                    if (!status) {
                        var error = new ResponseError(
                            ResponseCodes.FAILED,
                            "Failed to lock policy document",
                            "An error occurred! could lock policy document");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                    return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
                } else {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.POLICYVIOLATION;
                    response.Message = $"Policy Violation! You cannot lock a document that is not uptodate";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting policy document by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Regulatory Types

        [HttpPost("registers/regulatory-types-retrieve")]
        public async Task<IActionResult> GetRegulatoryType([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get regulatory type by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RegulatoryTypeResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request regulatory type ID is required",
                        "Invalid regulatory type request ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RegulatoryTypeResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var typeRecord = await _regulatoryType.GetAsync(d => d.Id == request.RecordId, false);
                if (typeRecord == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Regulatory type not found",
                        "No regulatory type matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RegulatoryTypeResponse>(error));
                }

                //..return regulatory type
                var type = new RegulatoryTypeResponse {
                    Id = typeRecord.Id,
                    TypeName = typeRecord.TypeName ?? string.Empty,
                    IsDeleted = typeRecord.IsDeleted,
                    CreatedOn = typeRecord.CreatedOn,
                    CreatedBy = typeRecord.CreatedBy ?? string.Empty,
                    UpdatedOn = typeRecord.LastModifiedOn ?? typeRecord.CreatedOn
                };

                return Ok(new GrcResponse<RegulatoryTypeResponse>(type));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<RegulatoryTypeResponse>(error));
            }
        }

        [HttpPost("registers/paged-regulatory-type-list")]
        public async Task<IActionResult> GetPagedRegulatoryTypeList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<RegulatoryTypeResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _regulatoryType.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No regulatory type records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<RegulatoryTypeResponse>>(new PagedResponse<RegulatoryTypeResponse>(new List<RegulatoryTypeResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<RegulatoryTypeResponse> types = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(type => types.Add(new() {
                        Id = type.Id,
                        TypeName = type.TypeName ?? string.Empty,
                        IsDeleted = type.IsDeleted,
                        CreatedOn = type.CreatedOn,
                        CreatedBy = type.CreatedBy ?? string.Empty,
                        UpdatedOn = type.LastModifiedOn ?? type.CreatedOn
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    types = types.Where(u =>
                        (u.TypeName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<RegulatoryTypeResponse>>(
                    new PagedResponse<RegulatoryTypeResponse>(
                    types,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<RegulatoryTypeResponse>>(error));
            }
        }

        [HttpPost("registers/create-regulatory-type")]
        public async Task<IActionResult> CreateRegulatoryType([FromBody] RegulatoryTypeRequest request) {
            try {
                Logger.LogActivity("Creating new regulatory type", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The regulatory type record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.TypeName)) {
                    if (await _regulatoryType.ExistsAsync(t => t.TypeName == request.TypeName)) {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another regulatory type found with similar name"
                        );
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The type name is required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.CreatedBy = currentUser.Username;
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.CreatedBy = $"{request.UserId}";
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.CreatedOn = DateTime.Now;
                request.ModifiedOn = DateTime.Now;

                //..create regulatory type
                var result = await _regulatoryType.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory type saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save regulatory type record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-regulatory-type")]
        public async Task<IActionResult> UpdateRegulatoryType([FromBody] RegulatoryTypeRequest request) {
            try {
                Logger.LogActivity("Update regulatory type", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The regulatory type record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _regulatoryType.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found","Regulatory type record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.ModifiedOn = DateTime.Now;

                //..update regulatory type
                var result = await _regulatoryType.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory type updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update regulatory type record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-regulatory-type")]
        public async Task<IActionResult> DeleteRegulatoryType([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _regulatoryType.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Regulatory type Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete regulatory type
                var status = await _regulatoryType.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to regulatory type document",
                        "An error occurred! could delete regulatory type");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting regulatory type by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Document Types

        [HttpPost("registers/document-type-retrieve")]
        public async Task<IActionResult> GetDocumentType([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get document type by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DocumentTypeResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request document type ID is required",
                        "Invalid document type request ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DocumentTypeResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var register = await _documentTypeService.GetAsync(d => d.Id == request.RecordId, false);
                if (register == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Document type not found",
                        "No document type matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DocumentTypeResponse>(error));
                }

                //..return document type
                var type = new DocumentTypeResponse {
                    Id = register.Id,
                    TypeName = register.DocumentType ?? string.Empty,
                    IsDeleted = register.IsDeleted,
                    CreatedOn = register.CreatedOn,
                    CreatedBy = register.CreatedBy ?? string.Empty,
                    UpdatedOn = register.LastModifiedOn ?? register.CreatedOn
                };

                return Ok(new GrcResponse<DocumentTypeResponse>(type));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<DocumentTypeResponse>(error));
            }
        }

        [HttpPost("registers/document-type-list")]
        public async Task<IActionResult> GetDocumentTypeList([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DocumentTypeResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var documents = await _documentTypeService.GetAllAsync(false);

                if (documents == null || !documents.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No document type records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<DocumentTypeResponse>>(new List<DocumentTypeResponse>()));
                }

                List<DocumentTypeResponse> documentList = new();
                documents.ToList().ForEach(register => documentList.Add(new DocumentTypeResponse() {
                    Id = register.Id,
                    TypeName = register.DocumentType ?? string.Empty,
                    IsDeleted = register.IsDeleted,
                    CreatedOn = register.CreatedOn,
                    CreatedBy = register.CreatedBy ?? string.Empty,
                    UpdatedOn = register.LastModifiedOn ?? register.CreatedOn
                }));

                return Ok(new GrcResponse<List<DocumentTypeResponse>>(documentList));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<List<DocumentTypeResponse>>(error));
            }
        }

        [HttpPost("registers/paged-document-type-list")]
        public async Task<IActionResult> GetPagedDocumentTypeList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DocumentTypeResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _documentTypeService.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No document type records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<DocumentTypeResponse>>(new PagedResponse<DocumentTypeResponse>(new List<DocumentTypeResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<DocumentTypeResponse> types = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(type => types.Add(new() {
                        Id = type.Id,
                        TypeName = type.DocumentType ?? string.Empty,
                        IsDeleted = type.IsDeleted,
                        CreatedOn = type.CreatedOn,
                        CreatedBy = type.CreatedBy ?? string.Empty,
                        UpdatedOn = type.LastModifiedOn ?? type.CreatedOn
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    types = types.Where(u =>
                        (u.TypeName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<DocumentTypeResponse>>(
                    new PagedResponse<DocumentTypeResponse>(
                    types,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<DocumentTypeResponse>>(error));
            }
        }

        [HttpPost("registers/create-document-type")]
        public async Task<IActionResult> CreateDocumentType([FromBody] DocumentTypeRequest request) {
            try {
                Logger.LogActivity("Creating new document type", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The document type record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.DocumentType)) {
                    if (await _documentTypeService.ExistsAsync(t => t.DocumentType == request.DocumentType)) {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another document type found with similar name"
                        );
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The type name is required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.CreatedBy = currentUser.Username;
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.CreatedBy = $"{request.UserId}";
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.CreatedOn = DateTime.Now;
                request.ModifiedOn = DateTime.Now;

                //..create document type
                var result = await _documentTypeService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Document type saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save document type record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-document-type")]
        public async Task<IActionResult> UpdateDocumentType([FromBody] DocumentTypeRequest request) {
            try {
                Logger.LogActivity("Update document type", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The document type record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _documentTypeService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Document type record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.ModifiedOn = DateTime.Now;

                //..update document type
                var result = await _documentTypeService.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Document type updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update document type record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-document-type")]
        public async Task<IActionResult> DeleteDocumentType([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _documentTypeService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Regulatory document Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete document type
                var status = await _documentTypeService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to regulatory document document",
                        "An error occurred! could delete regulatory document");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting regulatory document by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Categories

        [HttpPost("registers/regulatory-categories-retrieve")]
        public async Task<IActionResult> GetCategory([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get regulatory category by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RegulatoryCategoryResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request regulatory category ID is required",
                        "Invalid regulatory category request ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RegulatoryCategoryResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var register = await _categoryService.GetAsync(d => d.Id == request.RecordId, false);
                if (register == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Regulatory category not found",
                        "No regulatory category matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<RegulatoryCategoryResponse>(error));
                }

                //..return regulatory category
                var type = new RegulatoryCategoryResponse {
                    Id = register.Id,
                    CategoryName = register.CategoryName ?? string.Empty,
                    IsDeleted = register.IsDeleted,
                    CreatedOn = register.CreatedOn,
                    CreatedBy = register.CreatedBy ?? string.Empty,
                    UpdatedOn = register.LastModifiedOn ?? register.CreatedOn
                };

                return Ok(new GrcResponse<RegulatoryCategoryResponse>(type));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<RegulatoryCategoryResponse>(error));
            }
        }

        [HttpPost("registers/category-list")]
        public async Task<IActionResult> GetCategoryList([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var documents = await _categoryService.GetAllAsync(false);

                if (documents == null || !documents.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No category records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<RegulatoryCategoryResponse>>(new List<RegulatoryCategoryResponse>()));
                }

                List<RegulatoryCategoryResponse> categories = new();
                documents.ToList().ForEach(category => categories.Add(new RegulatoryCategoryResponse() {
                    Id = category.Id,
                    CategoryName = category.CategoryName ?? string.Empty,
                    IsDeleted = category.IsDeleted,
                    CreatedOn = category.CreatedOn,
                    CreatedBy = category.CreatedBy ?? string.Empty,
                    UpdatedOn = category.LastModifiedOn ?? category.CreatedOn
                }));

                return Ok(new GrcResponse<List<RegulatoryCategoryResponse>>(categories));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<List<RegulatoryCategoryResponse>>(error));
            }
        }

        [HttpPost("registers/paged-categories-list")]
        public async Task<IActionResult> GetPagedCategoryList([FromBody] ListRequest request) {

            try {
                //Logger.LogActivity($"{request.Action}", "INFO");
                //if (request == null) {
                //    var error = new ResponseError(
                //        ResponseCodes.BADREQUEST,
                //        "Request record cannot be empty",
                //        "Invalid request body"
                //    );
                //    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                //    return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(error));
                //}

                //Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                //var pageResult = await _categoryService.PageAllAsync(request.PageIndex, request.PageSize, false);
                //if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                //    var error = new ResponseError(
                //        ResponseCodes.SUCCESS,
                //        "No data",
                //        "No regulatory category records found"
                //    );
                //    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                //    return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(new PagedResponse<RegulatoryCategoryResponse>(new List<RegulatoryCategoryResponse>(), 0, pageResult.Page, pageResult.Size)));
                //}

                //List<RegulatoryCategoryResponse> categories = new();
                //var records = pageResult.Entities.ToList();
                //if (records != null && records.Any()) {
                //    records.ForEach(category => categories.Add(new() {
                //        Id = category.Id,
                //        CategoryName = category.CategoryName ?? string.Empty,
                //        IsDeleted = category.IsDeleted,
                //        CreatedOn = category.CreatedOn,
                //        CreatedBy = category.CreatedBy ?? string.Empty,
                //        UpdatedOn = category.LastModifiedOn ?? category.CreatedOn
                //    }));
                //}

                //if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                //    var searchTerm = request.SearchTerm.ToLower();
                //    categories = categories.Where(u =>
                //        (u.CategoryName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                //    ).ToList();
                //}

                var categories = new List<RegulatoryCategoryResponse>() {
                        new () {
                            Id = 1,
                            CategoryName = "Financial Institutions Business",
                            IsDeleted = false,
                            CreatedOn = DateTime.Now.AddMonths(-5),
                            CreatedBy = "System",
                            Statutes = new List<StatutoryRegulationResponse>() {
                                new () {
                                    Id = 1,
                                    StatutoryTypeId = 1,
                                    StatutoryType = "Regulatory Act",
                                    AuthorityId = 1,
                                    AuthorityName = "Bank of Uganda",
                                    StatutoryLawCode = "FIA-2004",
                                    StatutoryLawName ="FINANCIAL INSTITUTIONS ACT ,2004 as Amended by the FINANCIAL INSTITUTIONS ACT 2016",
                                    IsDeleted = false,
                                    CreatedBy = "system",
                                    ModifiedBy = "system"
                                },
                                new () {
                                    Id = 2,
                                    StatutoryTypeId = 2,
                                    StatutoryType = "Regulation",
                                    AuthorityId = 6,
                                    AuthorityName = "Financial Intelligence Authority",
                                    StatutoryLawCode = "FIR-2005",
                                    StatutoryLawName ="FINANCIAL INSTITUTIONS (LICENSING) REGULATIONS, 2005",
                                    IsDeleted = false,
                                    CreatedBy = "system",
                                    ModifiedBy = "system"
                                },
                                new () {
                                    Id = 3,
                                    StatutoryTypeId = 2,
                                    StatutoryType = "Regulation",
                                    AuthorityId = 6,
                                    AuthorityName = "Financial Intelligence Authority",
                                    StatutoryLawCode = "FIR-CAR",
                                    StatutoryLawName ="FINANCIAL INSTITUTIONS (CAPITAL ADEQUACY REQUIREMENTS) REGULATIONS, 2018 as Amended by the Revision_of_minimum_Capital_Requirements_Instrument_2022",
                                    IsDeleted = false,
                                    CreatedBy = "system",
                                    ModifiedBy = "system"
                                },
                                new () {
                                    Id = 4,
                                    StatutoryTypeId = 2,
                                    StatutoryType = "Regulation",
                                    AuthorityId = 6,
                                    AuthorityName = "Financial Intelligence Authority",
                                    StatutoryLawCode = "FIR-CCAP",
                                    StatutoryLawName ="THE FINANCIAL INSTITUTIONS (CREDIT CLASSIFICATION AND PROVISIONING) REGULATIONS, 2005",
                                    IsDeleted = false,
                                    CreatedBy = "system",
                                    ModifiedBy = "system"
                                }
                            }
                        },
                        new () {
                            Id = 2,
                            CategoryName = "Liquidity Regulations",
                            CreatedOn = DateTime.Now.AddMonths(-20),
                            CreatedBy = "System",
                            Statutes = new List<StatutoryRegulationResponse>()
                        },
                        new () {
                            Id = 3,
                            CategoryName = "Payment Systems",
                            CreatedOn = DateTime.Now.AddMonths(-50),
                            CreatedBy = "System",
                            Statutes = new List<StatutoryRegulationResponse>() {
                                new () {
                                    Id = 5,
                                    StatutoryTypeId = 1,
                                    StatutoryType = "Act",
                                    AuthorityId = 1,
                                    AuthorityName = "Bank of Uganda",
                                    StatutoryLawCode = "NPS-2020",
                                    StatutoryLawName ="THE NATIONAL PAYMENTS SYSTEMS ACT, 2020",
                                    IsDeleted = false,
                                    CreatedBy = "system",
                                    ModifiedBy = "system"
                                },
                                new () {
                                    Id = 6,
                                    StatutoryTypeId = 2,
                                    StatutoryType = "Regulation",
                                    AuthorityId = 1,
                                    AuthorityName = "Bank of Uganda",
                                    StatutoryLawCode = "NPS-2021",
                                    StatutoryLawName ="THE NATIONAL PAYMENTS SYSTEMS REGULATION 2021 S.I No.18 OF 2021",
                                    IsDeleted = false,
                                    CreatedBy = "system",
                                    ModifiedBy = "system"
                                },
                                new () {
                                    Id = 7,
                                    StatutoryTypeId = 2,
                                    StatutoryType = "Regulation",
                                    AuthorityId = 1,
                                    AuthorityName = "Bank of Uganda",
                                    StatutoryLawCode = "NPS-AGENT",
                                    StatutoryLawName ="THE NATIONAL PAYMENTS SYSTEMS (AGENT) REGULATION 2021",
                                    IsDeleted = false,
                                    CreatedBy = "system",
                                    ModifiedBy = "system"
                                }
                            }
                        }
                    };
                return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(new PagedResponse<RegulatoryCategoryResponse>(
                    categories,
                    20, //pageResult.Count,
                    2, //pageResult.Page,
                    5 //pageResult.Size
                )));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(error));
            }
        }

        [HttpPost("registers/create-category")]
        public async Task<IActionResult> CreateCategory([FromBody] RegulatoryCategoryRequest request) {
            try {
                Logger.LogActivity("Creating new regulatory category", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The regulatory category record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.CategoryName)) {
                    if (await _categoryService.ExistsAsync(t => t.CategoryName == request.CategoryName)) {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another regulatory category found with similar name"
                        );
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The regulatory category name is required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.CreatedBy = currentUser.Username;
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.CreatedBy = $"{request.UserId}";
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.CreatedOn = DateTime.Now;
                request.ModifiedOn = DateTime.Now;

                //..create document type
                var result = await _categoryService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory category saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save regulatory category record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-category")]
        public async Task<IActionResult> UpdateCategory([FromBody] RegulatoryCategoryRequest request) {
            try {
                Logger.LogActivity("Update regulatory category", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The regulatory category record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.CategoryName)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid Record, Category name is required", "Category name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _categoryService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Regulatory category record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.ModifiedOn = DateTime.Now;

                //..update regulatory category
                var result = await _categoryService.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory category updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update regulatory category record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-category")]
        public async Task<IActionResult> DeleteCategory([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _categoryService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Regulatory category Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete document type
                var status = await _categoryService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Failed to regulatory category",
                        "An error occurred! could delete regulatory category");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting regulatory category by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Statutories

        [HttpPost("registers/statute-retrieve")]
        public async Task<IActionResult> GetStatute([FromBody] IdRequest request) {
            return Ok();
        }

        [HttpPost("registers/category-statute-list")]
        public async Task<IActionResult> GetCategoryStatues([FromBody] StatuteListRequest request) {

            if (request != null) {
                var categoryId = request.ActivityTypeId.Value;

                var categoryList = new List<StatutoryRegulationResponse>() {
                    new () {
                        Id = 1,
                        CategoryId = 1,
                        CategoryName = "Financial Institutions Business",
                        StatutoryTypeId = 1,
                        StatutoryType = "Regulatory Act",
                        AuthorityId = 1,
                        AuthorityName = "Bank of Uganda",
                        StatutoryLawCode = "FIA-2004",
                        StatutoryLawName ="FINANCIAL INSTITUTIONS ACT ,2004 as Amended by the FINANCIAL INSTITUTIONS ACT 2016",
                        IsDeleted = false,
                        CreatedBy = "system",
                        ModifiedBy = "system"
                    },
                    new () {
                        Id = 2,
                        CategoryId = 1,
                        CategoryName = "Financial Institutions Business",
                        StatutoryTypeId = 2,
                        StatutoryType = "Regulation",
                        AuthorityId = 6,
                        AuthorityName = "Financial Intelligence Authority",
                        StatutoryLawCode = "FIR-2005",
                        StatutoryLawName ="FINANCIAL INSTITUTIONS (LICENSING) REGULATIONS, 2005",
                        IsDeleted = false,
                        CreatedBy = "system",
                        ModifiedBy = "system"
                    },
                    new () {
                        Id = 3,
                        CategoryId = 1,
                        CategoryName = "Financial Institutions Business",
                        StatutoryTypeId = 2,
                        StatutoryType = "Regulation",
                        AuthorityId = 6,
                        AuthorityName = "Financial Intelligence Authority",
                        StatutoryLawCode = "FIR-CAR",
                        StatutoryLawName ="FINANCIAL INSTITUTIONS (CAPITAL ADEQUACY REQUIREMENTS) REGULATIONS, 2018 as Amended by the Revision_of_minimum_Capital_Requirements_Instrument_2022",
                        IsDeleted = false,
                        CreatedBy = "system",
                        ModifiedBy = "system"
                    },
                    new () {
                        Id = 4,
                        CategoryId = 1,
                        CategoryName = "Financial Institutions Business",
                        StatutoryTypeId = 2,
                        StatutoryType = "Regulation",
                        AuthorityId = 6,
                        AuthorityName = "Financial Intelligence Authority",
                        StatutoryLawCode = "FIR-CCAP",
                        StatutoryLawName ="THE FINANCIAL INSTITUTIONS (CREDIT CLASSIFICATION AND PROVISIONING) REGULATIONS, 2005",
                        IsDeleted = false,
                        CreatedBy = "system",
                        ModifiedBy = "system"
                    },
                    new () {
                        Id = 5,
                        CategoryId = 2,
                        CategoryName = "Liquidity Regulations",
                        StatutoryTypeId = 2,
                        StatutoryType = "Regulation",
                        AuthorityId = 6,
                        AuthorityName = "Financial Intelligence Authority",
                        StatutoryLawCode = "FIR-LIQUIDITY",
                        StatutoryLawName ="THE FINANCIAL INSTITUTIONS (LIQUIDITY) REGULATIONS, 2023",
                        IsDeleted = false,
                        CreatedBy = "system",
                        ModifiedBy = "system"
                    },
                    new () {
                        Id = 6,
                        CategoryId = 3,
                        CategoryName = "Payment Systems",
                        StatutoryTypeId = 1,
                        StatutoryType = "Act",
                        AuthorityId = 1,
                        AuthorityName = "Bank of Uganda",
                        StatutoryLawCode = "NPS-2020",
                        StatutoryLawName ="THE NATIONAL PAYMENTS SYSTEMS ACT, 2020",
                        IsDeleted = false,
                        CreatedBy = "system",
                        ModifiedBy = "system"
                    },
                    new () {
                        Id = 7,
                        CategoryId = 3,
                        CategoryName = "Payment Systems",
                        StatutoryTypeId = 2,
                        StatutoryType = "Regulation",
                        AuthorityId = 1,
                        AuthorityName = "Bank of Uganda",
                        StatutoryLawCode = "NPS-2021",
                        StatutoryLawName ="THE NATIONAL PAYMENTS SYSTEMS REGULATION 2021 S.I No.18 OF 2021",
                        IsDeleted = false,
                        CreatedBy = "system",
                        ModifiedBy = "system"
                    },
                    new () {
                        Id = 8,
                        CategoryId = 3,
                        CategoryName = "Payment Systems",
                        StatutoryTypeId = 2,
                        StatutoryType = "Regulation",
                        AuthorityId = 1,
                        AuthorityName = "Bank of Uganda",
                        StatutoryLawCode = "NPS-AGENT",
                        StatutoryLawName ="THE NATIONAL PAYMENTS SYSTEMS (AGENT) REGULATION 2021",
                        IsDeleted = false,
                        CreatedBy = "system",
                        ModifiedBy = "system"
                    }
                };

                var statutues = await Task.FromResult(categoryList.Where(l => l.CategoryId == categoryId).ToList());
                return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(new PagedResponse<StatutoryRegulationResponse>(
                  statutues,
                  20, //pageResult.Count,
                  2, //pageResult.Page,
                  5 //pageResult.Size
              )));
            }

            return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(new PagedResponse<StatutoryRegulationResponse>(null,0,0,0)));
        }

        [HttpPost("registers/paged-statute-list")]
        public async Task<IActionResult> GetPagedStatues([FromBody] ListRequest request) {
            return Ok();
        }

        [HttpPost("registers/create-statute")]
        public async Task<IActionResult> CreateStatute([FromBody] StatutoryRegulationRequest request) {
            return Ok();
        }

        [HttpPost("registers/update-statute")]
        public async Task<IActionResult> UpdateStatute([FromBody] StatutoryRegulationRequest request) {
            return Ok();
        }

        [HttpPost("registers/delete-statute")]
        public async Task<IActionResult> DeleteStatute([FromBody] IdRequest request) {
            return Ok();
        }

        #endregion

        #region Statute Section

        [HttpPost("registers/statute-section-retrieve")]
        public async Task<IActionResult> GetStatuteSection([FromBody] IdRequest request) {
            return Ok();
        }

        [HttpPost("registers/statute-sections-list")]
        public async Task<IActionResult> GetLawSection([FromBody] StatuteListRequest request) {

            if (request != null) {
                if (request.ActivityTypeId.HasValue) {
                    var statuteId = request.ActivityTypeId.Value;
                    var statuteList = new List<StatuteSectionResponse>() {
                        new () {
                            Id = 1,
                            StatutoryId = 1,
                            Section = "R.7(1)",
                            Summery = "Core and total capital",
                            Obligation = "The bank shall at all times maintain a core capital of not less than 10% and a total capital of not less than 12% of the total risk adjusted assets plus risk adjusted off balance sheet",
                            ComplianceAssurance = 10,
                            IsMandatory = true
                        },
                        new () {
                            Id = 2,
                            StatutoryId = 1,
                            Section = "R.7(2)",
                            Summery = "Core capital to be equal too minimum capital funds",
                            Obligation = "The core capital of the bank shall at all times be equivalent to at least the minimum capital funds set out by Law and shall be reduced by goodwill and similar intangible assets, investments in unconsolidated financial subsidiaries and future income tax benefits.",
                            ComplianceAssurance = 40,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 1,
                            Section = "R.8(1)",
                            Summery = "Maintenance of Records",
                            Obligation = "The bank shall at all times maintain adequate records, including daily balance sheets and periodic statements of income and expense to enable proper computation of its capital adequacy.",
                            ComplianceAssurance = 40,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 2,
                            Section = "R.6 (1)",
                            Summery = "When credit facility with pre-established repayment schedule is  considered Non performing",
                            Obligation = "Consider a credit facility with a pre-established repayment schedule  non-performing if—\r\n(a) the principal or interest is due and unpaid for 90 days or more; or\r\n(b) the principal or interest payments equal to 90 days interest or more have been capitalised, refinanced, renegotiated, restructured or rolled over.",
                            ComplianceAssurance = 30,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 2,
                            Section = "R.6 (2)",
                            Summery = "When credit facility with pre-established repayment schedule is  considered Non performing",
                            Obligation = "Consider a credit facility without a fixed repayment program, e.g  overdrafts or other forms of open-ended credit,  non-performing if—\r\n(a) It exceeds the customer’s established borrowing limit for 90 days or more\r\n(b) The customer’s borrowing line has expired for 90 consecutive days or more;\r\n(c) interest is due and unpaid for 90 days or more; or\r\n(d) the overdraft or account is inactive,",
                            ComplianceAssurance = 50,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 4,
                            Section = "R.8A(1)",
                            Summery = "Ascertainment that foreign currency is legal tender",
                            Obligation = "The bank has a duty to cross check and satisfy itself that the foreign currency exchanged is legal tender.",
                            ComplianceAssurance = 3,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 5,
                            Section = "5",
                            Summery = "Liquidity risk appetite",
                            Obligation = "(1) A financial institution shall set its liquidity risk appetite.\r\n(2) The liquidity risk appetite set in subregulation (1) shall—\r\n(a) define the level of liquidity risk that the financial institution is willing to take on;\r\n(b) match the business strategy of the financial institution and role in the financial system of the financial institution;\r\n(c) reflect the financial condition and funding capacity of the financial institution;\r\n(d) manage the liquidity of the financial institution efficiently in normal times so as to be able to withstand prolonged periods of stress; and\r\n(e) reflect a clear understanding of the trade-off between risk and return at all levels of management.\r\n(3) The liquidity risk appetite shall be reviewed periodically and adjusted in line with changes in the business strategy and operating conditions of the financial institutions.",
                            ComplianceAssurance = 3,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 5,
                            Section = "6(a)",
                            Summery = "Liquidity management framework",
                            Obligation = "(1) A financial institution, with the approval of the board of directors of the financial institution, shall formulate a liquidity risk management framework including—\r\n(a) liquidity management policy; and\r\n(b) liquidity management strategy.\r\n(2) The liquidity management strategy shall prescribe—\r\n(a) a general approach to liquidity risk management in line with the liquidity risk appetite of the financial institution;\r\n(b) measures to maintain sufficient liquidity of the financial institution including a cushion of unencumbered high quality liquid assets and the loss or impairment of both unsecured and secured funding sources; and(c) measures to manage the assets, liabilities and off-balance sheet items of the financial institution for the purpose of meeting the contractual commitments of the financial institution at a reasonable cost as and when they fall due.",
                            ComplianceAssurance = 3,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 5,
                            Section = "6(b)",
                            Summery = "Liquidity management policy",
                            Obligation = "A financial institution shall have a well-articulated liquidity management policy indicating crucial elements of sound liquidity management including—\r\n(a) the appropriate mix and composition of assets and liabilities required to maintain sufficient liquidity;an effective process for identifying, measuring, monitoring and controlling liquidity risk considering the forecasting of cash inflow and outflows over a suitable time horizon;\r\n(c) active monitoring of liquidity risk exposures and funding needs across all business lines, subsidiaries and currencies;\r\n(d) good management information systems;\r\n(e) central liquidity control;\r\n(f) a funding strategy that provides effective diversification and stability of funding sources; \r\n(g) active management of liquidity positions within the same day in order to meet payment system obligations in a timely manner under both normal and stressed conditions;\r\n(h) ongoing analysis and stress testing of net funding requirements under alternative scenarios to manage long term and temporary liquidity disruptions; and\r\n(i) monitoring and reporting of the liquidity metrics in these Regulations.",
                            ComplianceAssurance = 3,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 5,
                            Section = "7(a)",
                            Summery = "Review of the Liquidity management policy",
                            Obligation = "(4) The financial institution shall review the liquidity management policy on an annual basis.\r\n(5) The financial institution shall review the liquidity management strategy as part of the overall strategy of the financial institution.",
                            ComplianceAssurance = 3,
                            IsMandatory = true
                        },
                        new () {
                            Id = 3,
                            StatutoryId = 5,
                            Section = "7(b)",
                            Summery = "Liquidity contingency plan",
                            Obligation = "(1) A financial institution shall formulate a liquidity contingency plan.\r\n(2) The liquidity contingency plan shall prescribe a strategy for addressing liquidity shortages and the procedures for managing cash flow deficits in emergency situations.\r\n(3) The liquidity contingency plan shall identify the range of events that may trigger a liquidity shortfall, outline mechanisms to facilitate monitoring of these trigger events, establish clear lines of responsibility of monitoring and approval procedures of remedying the liquidity shortfall.\r\n(4) The financial institution shall review, test and update the liquidity contingency plan annually and documentation of the results for the purpose of verifying that the liquidity contingency plan is operationally robust.",
                            ComplianceAssurance = 3,
                            IsMandatory = true
                        },
                    };


                    var statutues = await Task.FromResult(statuteList.Where(l => l.StatutoryId == statuteId).ToList());
                    return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(new PagedResponse<StatuteSectionResponse>(
                      statutues,
                      20, //pageResult.Count,
                      2, //pageResult.Page,
                      5 //pageResult.Size
                  )));
                }

            }

            return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(new PagedResponse<StatuteSectionResponse>(null, 0, 0, 0)));
        }

        [HttpPost("paged-statute-sections-list")]
        public async Task<IActionResult> GetPagedSections([FromBody] ListRequest request) {
            return Ok();
        }

        [HttpPost("registers/create-statute-section")]
        public async Task<IActionResult> CreateSection([FromBody] StatutoryRegulationRequest request) {
            return Ok();
        }

        [HttpPost("registers/update-statute-section")]
        public async Task<IActionResult> UpdateSection([FromBody] StatutoryRegulationRequest request) {
            return Ok();
        }

        [HttpPost("registers/delete-statute-section")]
        public async Task<IActionResult> DeleteSection([FromBody] IdRequest request) {
            return Ok();
        }

        #endregion

        #region Authorities

        [HttpPost("registers/authorities-retrieve")]
        public async Task<IActionResult> GetAuthority([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get authority by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuthorityResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request authority ID is required",
                        "Invalid authority request ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuthorityResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var authorityRecord = await _authorityService.GetAsync(d => d.Id == request.RecordId, false);
                if (authorityRecord == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Authority not found",
                        "No authority matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuthorityResponse>(error));
                }

                //..return authority
                var authority = new AuthorityResponse {
                    Id = authorityRecord.Id,
                    AuthorityName = authorityRecord.AuthorityName ?? string.Empty,
                    AuthorityAlias = authorityRecord.AuthorityAlias ?? string.Empty,
                    IsDeleted = authorityRecord.IsDeleted,
                    CreatedOn = authorityRecord.CreatedOn,
                    CreatedBy = authorityRecord.CreatedBy ?? string.Empty,
                    UpdatedOn = authorityRecord.LastModifiedOn ?? authorityRecord.CreatedOn
                };

                return Ok(new GrcResponse<AuthorityResponse>(authority));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuthorityResponse>(error));
            }
        }

        [HttpPost("registers/paged-authorities-list")]
        public async Task<IActionResult> GetPagedAuthorityList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuthorityResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _authorityService.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No authority records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuthorityResponse>>(new PagedResponse<AuthorityResponse>(new List<AuthorityResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<AuthorityResponse> authorities = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(authority => authorities.Add(new() {
                        Id = authority.Id,
                        AuthorityName = authority.AuthorityName ?? string.Empty,
                        AuthorityAlias = authority.AuthorityAlias ?? string.Empty,
                        IsDeleted = authority.IsDeleted,
                        CreatedOn = authority.CreatedOn,
                        CreatedBy = authority.CreatedBy ?? string.Empty,
                        UpdatedOn = authority.LastModifiedOn ?? authority.CreatedOn
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    authorities = authorities.Where(u =>
                        (u.AuthorityName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)||
                        (u.AuthorityAlias?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<AuthorityResponse>>(new PagedResponse<AuthorityResponse>(
                    authorities,
                    pageResult.Count,
                    pageResult.Page,
                    pageResult.Size
                )));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<AuthorityResponse>>(error));
            }
        }

        [HttpPost("registers/create-authority")]
        public async Task<IActionResult> CreateAuthority([FromBody] AuthorityRequest request) {
            try {
                Logger.LogActivity("Creating new authority", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The authority record cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.AuthorityName) || !string.IsNullOrWhiteSpace(request.AuthorityAlias)) {
                    if (await _authorityService.ExistsAsync(a => a.AuthorityName == request.AuthorityName || a.AuthorityAlias == request.AuthorityAlias)) {
                        var error = new ResponseError(
                            ResponseCodes.DUPLICATE,
                            "Duplicate Record",
                            "Another authority found with similar name or alias"
                        );
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "The authority name and alias are required"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.CreatedBy = currentUser.Username;
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.CreatedBy = $"{request.UserId}";
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.CreatedOn = DateTime.Now;
                request.ModifiedOn = DateTime.Now;

                //..create regulatory authority
                var result = await _authorityService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory authority saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save regulatory authority record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-authority")]
        public async Task<IActionResult> UpdateAuthority([FromBody] AuthorityRequest request) {
            try {
                Logger.LogActivity("Update regulatory authority", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The regulatory authority record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _authorityService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Regulatory authority record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.ModifiedBy = currentUser.Username;
                } else {
                    request.ModifiedBy = $"{request.UserId}";
                }

                //..add dates
                request.ModifiedOn = DateTime.Now;

                //..update regulatory authority
                var result = await _authorityService.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory authority updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update regulatory authority record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-authority")]
        public async Task<IActionResult> DeleteAuthority([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _authorityService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Regulatory authority Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete authority
                var status = await _authorityService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to regulatory authority", "An error occurred! could delete regulatory authority");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting regulatory authority by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

    }
}

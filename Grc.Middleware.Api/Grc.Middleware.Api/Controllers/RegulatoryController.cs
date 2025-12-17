using AutoMapper;
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
using System.Text.Json;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class RegulatoryController : GrcControllerBase {

        private readonly IAuthorityService _authorityService;
        private readonly ISystemAccessService _accessService;
        private readonly IFrequencyService _frequencyService;
        private readonly IRegulatoryCategoryService _categoryService;
        private readonly IRegulatoryReturnService _returnsService;
        private readonly IRegulatoryTypeService _regulatoryType;
        private readonly IRegulatoryDocumentService _regulatoryDocuments;
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
            return Ok();
        }

        [HttpPost("registers/paged-regulatory-type-list")]
        public async Task<IActionResult> GetPagedRegulatoryTypeList([FromBody] ListRequest request) {
            return Ok();
        }

        [HttpPost("registers/create-regulatory-type")]
        public async Task<IActionResult> CreateRegulatoryType([FromBody] PolicyDocumentRequest request) {
            return Ok();
        }

        [HttpPost("registers/update-regulatory-type")]
        public async Task<IActionResult> UpdateRegulatoryType([FromBody] PolicyDocumentRequest request) {
            return Ok();
        }

        [HttpPost("registers/delete-regulatory-type")]
        public async Task<IActionResult> DeleteRegulatoryType([FromBody] IdRequest request) {
            return Ok();
        }

        #endregion

        #region Document Types

        [HttpPost("registers/document-type-retrieve")]
        public async Task<IActionResult> GetDocumentType([FromBody] IdRequest request) {
            return Ok();
        }

        [HttpPost("registers/document-type-list")]
        public async Task<IActionResult> GetDocumentTypeList([FromBody] GeneralRequest request) {
            return Ok();
        }

        [HttpPost("registers/paged-document-type-list")]
        public async Task<IActionResult> GetPagedDocumentTypeList([FromBody] ListRequest request) {
            return Ok();
        }

        [HttpPost("registers/create-document-type")]
        public async Task<IActionResult> CreateDocumentType([FromBody] PolicyDocumentRequest request) {
            return Ok();
        }

        [HttpPost("registers/update-document-type")]
        public async Task<IActionResult> UpdateDocumentType([FromBody] PolicyDocumentRequest request) {
            return Ok();
        }

        [HttpPost("registers/delete-document-type")]
        public async Task<IActionResult> DeleteDocumentType([FromBody] IdRequest request) {
            return Ok();
        }

        #endregion

        #region Categories

        [HttpPost("registers/regulatory-categories-retrieve")]
        public async Task<IActionResult> GetCategory([FromBody] IdRequest request) {
            return Ok();
        }

        [HttpPost("registers/category-list")]
        public async Task<IActionResult> GetCategoryList([FromBody] GeneralRequest request) {
            return Ok();
        }

        [HttpPost("registers/paged-categories-list")]
        public async Task<IActionResult> GetPagedCategoryList([FromBody] ListRequest request) {
            return Ok();
        }

        [HttpPost("registers/create-category")]
        public async Task<IActionResult> CreateCategory([FromBody] PolicyDocumentRequest request) {
            return Ok();
        }

        [HttpPost("registers/update-category")]
        public async Task<IActionResult> UpdateCategory([FromBody] PolicyDocumentRequest request) {
            return Ok();
        }

        [HttpPost("registers/delete-category")]
        public async Task<IActionResult> DeleteCategory([FromBody] IdRequest request) {
            return Ok();
        }

        #endregion

        #region Authorities

        [HttpPost("registers/authorities-retrieve")]
        public async Task<IActionResult> GetAuthority([FromBody] IdRequest request) {
            return Ok();
        }

        [HttpPost("registers/paged-authorities-list")]
        public async Task<IActionResult> GetPagedAuthorityList([FromBody] ListRequest request) {
            return Ok();
        }

        [HttpPost("registers/create-authority")]
        public async Task<IActionResult> CreateAuthority([FromBody] PolicyDocumentRequest request) {
            return Ok();
        }

        [HttpPost("registers/update-authority")]
        public async Task<IActionResult> UpdateAuthority([FromBody] PolicyDocumentRequest request) {
            return Ok();
        }

        [HttpPost("registers/delete-authority")]
        public async Task<IActionResult> DeleteAuthority([FromBody] IdRequest request) {
            return Ok();
        }

        #endregion

        #region Articles
        #endregion

        #region Statutories
        #endregion


    }
}

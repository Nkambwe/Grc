using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Compliance.Audits;
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
        private readonly ISystemAccessService _accessService;
        private readonly IRegulatoryDocumentTypeService _documentTypeService;
        private readonly IRegulatoryDocumentService _regulatoryDocuments;
        private readonly IRegulatoryCategoryService _categoryService;
        private readonly IRegulatoryTypeService _regulatoryType;
        private readonly IAuthorityService _authorityService;
        private readonly IStatutoryArticleService _articleService;
        private readonly IStatutoryRegulationService _regulatoryService;
        private readonly IControlCategoryService _controlCategoryService;
        private readonly IComplianceIssueService _issueService;
        private readonly IControlItemService _itemService;
        private readonly IReturnService _returnService;
        private readonly IMailService _mailService;
        private readonly ICircularService _circularService;
        private readonly IResponsebilityService _ownerService;
        private readonly ICircularIssueService _circularIssueService;
        private readonly IReturnsSubmissionService _submissionService;
        private readonly IFrequencyService _frequencyService;
        private readonly IAuditService _auditService;
        private readonly IAuditTypeService _auditTypeService;
        private readonly IAuditReportService _auditReportService;
        private readonly IAuditExceptionService _auditExceptionService;
        private readonly IAuditTaskService _auditTaskService;
        public RegulatoryController(IObjectCypher cypher,
            IServiceLoggerFactory loggerFactory, 
            IMapper mapper, 
            ICompanyService companyService,
            IAuthorityService authorityService,
            ISystemAccessService accessService,
            IRegulatoryCategoryService categoryService,
            IRegulatoryTypeService regulatoryType,
            IRegulatoryDocumentService regulatoryDocuments,
            IRegulatoryDocumentTypeService documentTypeService,
            IResponsebilityService ownerService,
            IStatutoryArticleService articleService,
            IStatutoryRegulationService regulatoryService,
            IControlCategoryService controlCategoryService,
            IComplianceIssueService issueService,
            IControlItemService itemService,
            ICircularIssueService circularIssueService,
            IReturnService returnService,
            IFrequencyService frequencyService,
            ICircularService circularService,
            IEnvironmentProvider environment,
            IMailService mailService,
            IReturnsSubmissionService submissionService,
            IAuditService auditService,
            IAuditTypeService auditTypeService,
            IAuditReportService auditReportService,
            IAuditExceptionService auditExceptionService,
            IAuditTaskService auditTaskService,
            IErrorNotificationService errorService, 
            ISystemErrorService systemErrorService) 
            : base(cypher, loggerFactory, mapper, companyService, environment, errorService, systemErrorService) {
            _authorityService = authorityService;
            _accessService = accessService;
            _categoryService = categoryService;
            _regulatoryType = regulatoryType;
            _regulatoryDocuments = regulatoryDocuments;
            _documentTypeService = documentTypeService;
            _articleService = articleService;
            _regulatoryService = regulatoryService;
            _controlCategoryService = controlCategoryService;
            _issueService = issueService;
            _itemService = itemService;
            _returnService = returnService;
            _circularService = circularService;
            _circularIssueService = circularIssueService;
            _submissionService = submissionService;
            _ownerService = ownerService;
            _mailService = mailService;
            _frequencyService = frequencyService;
            _auditService = auditService;
            _auditTypeService = auditTypeService;
            _auditExceptionService = auditExceptionService;
            _auditReportService = auditReportService;
            _auditTaskService = auditTaskService;
        }

        #region Compliance Statistics

        [HttpPost("returns/dashboard-statistics")]
        public async Task<IActionResult> GetComplianceStatistics([FromBody] GeneralRequest request) {

            try {
                Logger.LogActivity("Retrieve compliance statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ComplianceStatisticsResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _returnService.GetComplianceStatisticsAsync(false);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could compliance statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<ComplianceStatisticsResponse>>(error));
                }

                return Ok(new GrcResponse<ComplianceStatisticsResponse>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ComplianceStatisticsResponse>(error));
            }
        }

        [HttpPost("returns/policy-statistics")]
        public async Task<IActionResult> GetPolicyStatistics([FromBody] StatusStatisticRequest request) {

            try {
                Logger.LogActivity("Retrieve policy status statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PolicyDashboardResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _returnService.GetPolicyStatisticsAsync(false, GetStatusEnumValue(request.Status));
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could policy status statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<PolicyDashboardResponse>>(error));
                }

                return Ok(new GrcResponse<PolicyDashboardResponse>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PolicyDashboardResponse>(error));
            }
        }

        [HttpPost("returns/all-statistics")]
        public async Task<IActionResult> GetComplianceStatistics([FromBody] StatusStatisticRequest request) {

            try {
                Logger.LogActivity("Retrieve returns statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ReturnsStatisticsResponses>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _returnService.GetReturnDashboardStatisticsAsync(false);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could not retrieve statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<ReturnsStatisticsResponses>>(error));
                }

                return Ok(new GrcResponse<ReturnsStatisticsResponses>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ReturnsStatisticsResponses>(error));
            }
        }

        [HttpPost("returns/period-returns")]
        public async Task<IActionResult> GetReturnsStatistics([FromBody] PeriodStatisticRequest request) {

            try {
                Logger.LogActivity("Retrieve returns statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ReturnDashboardResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _returnService.GetReturnStatisticsAsync(false, GetPeriodEnumValue(request.Period));
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could not retrieve returns statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ReturnDashboardResponse>(error));
                }

                return Ok(new GrcResponse<ReturnDashboardResponse>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ReturnDashboardResponse>(error));
            }
        }

        [HttpPost("returns/returns-extension")]
        public async Task<IActionResult> GetReturnExtensionStatistics([FromBody] PeriodStatisticRequest request) {

            try {
                Logger.LogActivity("Retrieve returns statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ReturnExtensionResponse>(error));
                }
                Logger.LogActivity($"ACTION >> {request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _returnService.GetReturnExtensionStatisticsAsync(false, GetPeriodEnumValue(request.Period));
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could not retrieve returns statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ReturnExtensionResponse>(error));
                }

                return Ok(new GrcResponse<ReturnExtensionResponse>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ReturnExtensionResponse>(error));
            }
        }

        [HttpPost("circulars/circular-authorities")]
        public async Task<IActionResult> GetCircularStatistics([FromBody] CircularStatisticRequest request) {

            try {
                Logger.LogActivity("Retrieve circular statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<CircularDashboardResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _returnService.GetCircularStatisticsAsync(false, request.Authority);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could circular statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<CircularDashboardResponse>>(error));
                }

                return Ok(new GrcResponse<CircularDashboardResponse>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<CircularDashboardResponse>(error));
            }
        }

        [HttpPost("circulars/all-statistics")]
        public async Task<IActionResult> GetCircularDashboardStatistics([FromBody] StatusStatisticRequest request) {

            try {
                Logger.LogActivity("Retrieve circular dashboard statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<CircularStatisticsResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _returnService.GetCircularDashboardStatisticsAsync(false);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could compliance statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<CircularStatisticsResponse>>(error));
                }

                return Ok(new GrcResponse<CircularStatisticsResponse>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<CircularStatisticsResponse>(error));
            }
        }

        [HttpPost("circulars/status-statistics")]
        public async Task<IActionResult> GetCircularExtensionStatistics([FromBody] CircularStatisticRequest request) {

            try {
                Logger.LogActivity("Retrieve circular statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<CircularExtensionResponses>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _returnService.GetCircularExtensionStatisticsAsync(false, request.Authority);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could circular statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<CircularExtensionResponses>>(error));
                }

                return Ok(new GrcResponse<CircularExtensionResponses>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<CircularExtensionResponses>(error));
            }
        }

        #endregion

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

        #region Compliance Obligations

        [HttpPost("registers/paged-obligations-list")]
        public async Task<IActionResult> GetPagedObligationsList([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ObligaionResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _categoryService.GetObligationsAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS,"No data", "No obligation records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ObligaionResponse>>(
                              new PagedResponse<ObligaionResponse>(
                              new List<ObligaionResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                var categories = pageResult.Entities;
                return Ok(new GrcResponse<PagedResponse<ObligaionResponse>>(
                          new PagedResponse<ObligaionResponse>(
                              categories, pageResult.Count, pageResult.Page,pageResult.Size))
                    );
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ObligaionResponse>>(error));
            }
        }

        [HttpPost("registers/obligation-retrieve")]
        public async Task<IActionResult> GetObligation([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get Obligation/requirement by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ObligationResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request obligation ID is required", "Invalid regulation request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ObligationResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var obligation = await _articleService.GetObligationAsync(a => a.Id == request.RecordId, true);
                if (obligation == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Obligation/Requirement not found", "No obligation/Requirement matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ObligationResponse>(error));
                }
                return Ok(new GrcResponse<ObligationResponse>(obligation));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ObligationResponse>(error));
            }
        }

        [HttpPost("registers/create-compliance-map")]
        public async Task<IActionResult> CreateComplianceMap([FromBody] ObligationMapRequest request) {
            return Ok(new { success = true, message = "Success" });
        }

        [HttpPost("registers/paged-maps-list")]
        public async Task<IActionResult> GetPagedMapList([FromBody] ListRequest request) {
            try {
                //Logger.LogActivity($"{request.Action}", "INFO");
                //if (request == null) {
                //    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                //    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                //    return Ok(new GrcResponse<PagedResponse<ObligaionResponse>>(error));
                //}

                //Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                //var pageResult = await _categoryService.GetObligationsAsync(request.PageIndex, request.PageSize, false);
                //if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                //    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No obligation records found");
                //    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                //    return Ok(new GrcResponse<PagedResponse<ObligaionResponse>>(
                //              new PagedResponse<ObligaionResponse>(
                //              new List<ObligaionResponse>(), 0, pageResult.Page, pageResult.Size)));
                //}

                //var categories = pageResult.Entities;
                //return Ok(new GrcResponse<PagedResponse<ObligaionResponse>>(
                //          new PagedResponse<ObligaionResponse>(
                //              categories, pageResult.Count, pageResult.Page, pageResult.Size))
                //    );

                var maps = new List<ComplianceMapResponse>() {
                    new() {
                        Id = 1,
                        MapControl = "Access Control",
                        Include = true,
                        Owner = "Head Business Technology",
                        ControlMaps = new List<ControlMapResponse>() {
                            new() {
                                Id=1,
                                ParentId = 1,
                                Description = "Password Policy",
                                Notes = "Pearl Bank maintains a password cycle managed by Microsoft ADC for all company devices",
                                Include = false
                            },
                            new() {
                                Id=2,
                                ParentId = 1,
                                Description = "MFA Enforcement",
                                Notes = "Pearl Bank uses Entrust System to manage Two-Factor authentication",
                                Include = false
                            }
                        }
                    },
                    new() {
                        Id = 2,
                        MapControl = "Incident Logging",
                        Include = true,
                        Owner = "Head Business Technology",
                        ControlMaps = new()
                    },
                    new() {
                        Id = 3,
                        MapControl = "System Security",
                        Include = true,
                        Owner = "Head Security",
                        ControlMaps = new List<ControlMapResponse>() { 
                            new() {
                                Id=3,
                                ParentId = 3,
                                Description = "Premises security",
                                Notes = "Pearl Bank applys bio-metric systems for access, secures premises with armed security",
                                Include = false
                            },
                            new() {
                                Id=4,
                                ParentId = 3,
                                Description = "Customer Protection",
                                Notes = "Maintains customer physical property and secure access points",
                                Include = false
                            },
                            new() {
                                Id=5,
                                ParentId = 3,
                                Description = "Data Security",
                                Notes = "Some notes on data security",
                                Include = false
                            }
                        }
                    }
                };

                return Ok(new GrcResponse<PagedResponse<ComplianceMapResponse>>(new PagedResponse<ComplianceMapResponse>(maps, 2, 1, 2)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ObligaionResponse>>(error));
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
                    Comments = register.Comments ?? string.Empty,
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
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var documents = await _categoryService.GetAllAsync(false);

                if (documents == null || !documents.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No category records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<RegulatoryCategoryResponse>>(new List<RegulatoryCategoryResponse>()));
                }

                List<RegulatoryCategoryResponse> categories = new();
                documents.ToList().ForEach(category => categories.Add(new RegulatoryCategoryResponse() {
                    Id = category.Id,
                    CategoryName = category.CategoryName ?? string.Empty,
                    Comments = category.Comments ?? string.Empty,
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
                var pageResult = await _categoryService.PageLookupAsync(request.PageIndex, request.PageSize, false,
                    category => new RegulatoryCategoryResponse {
                        Id = category.Id,
                        CategoryName = category.CategoryName ?? string.Empty,
                        Comments = category.Comments ?? string.Empty,
                        CreatedOn = category.CreatedOn,
                        CreatedBy = category.CreatedBy,
                        Statutes = category.Regulations.Select(law => new StatutoryRegulationResponse() {
                            Id = law.Id,
                            CategoryId = law.CategoryId,
                            CategoryName = category.CategoryName ?? string.Empty,
                            StatutoryTypeId = law.TypeId,
                            StatutoryType = law.RegulationType.TypeName ?? string.Empty,
                            AuthorityId = law.AuthorityId,
                            AuthorityName = law.Authority.AuthorityName ?? string.Empty,
                            StatutoryLawCode = law.Code ?? string.Empty,
                            StatutoryLawName = law.RegulatoryName ?? string.Empty,
                            IsDeleted = law.IsDeleted,
                            CreatedBy = law.CreatedBy ?? string.Empty,
                            ModifiedBy = law.LastModifiedBy ?? string.Empty
                        }).ToList()
                    });

                    if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                        var error = new ResponseError(
                            ResponseCodes.SUCCESS,
                            "No data",
                            "No regulatory statute records found"
                        );
                        Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(new PagedResponse<RegulatoryCategoryResponse>(
                            new List<RegulatoryCategoryResponse>(), 0, pageResult.Page, pageResult.Size))
                            );
                    }

                    List<RegulatoryCategoryResponse> categories = new();
                    var records = pageResult.Entities ?? new();
                    if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                        var searchTerm = request.SearchTerm.ToLower();
                        categories = records.Where(u => u.CategoryName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
                    } else {
                        categories = records;
                    }

                    return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(
                        new PagedResponse<RegulatoryCategoryResponse>(categories, pageResult.Count, pageResult.Page, pageResult.Size))
                        );
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

        [HttpPost("registers/law-support-items")]
        public async Task<IActionResult> GetLawSupportItems([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatuteSupportResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                //..get support data
                var _supportItemsList = await _regulatoryService.GetStatuteSupportItemsAsync(false);
                return Ok(new GrcResponse<StatuteSupportResponse>(_supportItemsList));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<StatuteSupportResponse>(error));
            }
        }

        [HttpPost("registers/statute-retrieve")]
        public async Task<IActionResult> GetStatute([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get statute by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request record cannot be empty",
                        "Invalid request body"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatutoryRegulationResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(
                        ResponseCodes.BADREQUEST,
                        "Request ID is required",
                        "Invalid Statute request ID"
                    );
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatutoryRegulationResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var statuteRecord = await _regulatoryService.GetAsync(s => s.Id == request.RecordId, false, s => s.Category, s => s.Authority, s => s.RegulationType);
                if (statuteRecord == null) {
                    var error = new ResponseError(
                        ResponseCodes.FAILED,
                        "Statute not found",
                        "No regulatory statute matched the provided ID"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatutoryRegulationResponse>(error));
                }

                //..return regulatory statute
                var result = new StatutoryRegulationResponse {
                    Id = statuteRecord.Id,
                    CategoryId = statuteRecord.CategoryId,
                    CategoryName = statuteRecord.Category?.CategoryName ?? string.Empty,
                    StatutoryTypeId = statuteRecord.TypeId,
                    StatutoryType = statuteRecord.RegulationType?.TypeName ?? string.Empty,
                    AuthorityId = statuteRecord.AuthorityId,
                    AuthorityName = statuteRecord.Authority?.AuthorityName ?? string.Empty,
                    StatutoryLawCode = statuteRecord.Code ?? string.Empty,
                    StatutoryLawName = statuteRecord.RegulatoryName ?? string.Empty,
                    IsDeleted = statuteRecord.IsDeleted,
                    CreatedBy = statuteRecord.CreatedBy ?? string.Empty,
                    ModifiedBy = statuteRecord.LastModifiedBy ?? string.Empty
                };

                return Ok(new GrcResponse<StatutoryRegulationResponse>(result));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<StatutoryRegulationResponse>(error));
            }
        }

        [HttpPost("registers/category-statute-list")]
        public async Task<IActionResult> GetCategoryStatues([FromBody] StatuteListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(error));
                }

                int pageCount = 0;
                int pages = 0;
                int pageSize = 0;
                List<StatutoryRegulationResponse> statutes = new();
                if (request.ActivityTypeId.HasValue) {

                    //..log request
                    Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                    //..retrieve statutes
                    var pageResult = await _regulatoryService.PageAllAsync(request.PageIndex, request.PageSize, false, 
                                       c => c.CategoryId == request.ActivityTypeId, c => c.Authority, c => c.RegulationType, c => c.Category);
                    if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                        var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No Statutes records found");
                        Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(new PagedResponse<StatutoryRegulationResponse>(
                                new List<StatutoryRegulationResponse>(), 0, pageResult.Page, pageResult.Size))
                            );
                    }

                    //..map to response type
                    var records = pageResult.Entities.ToList();
                    if (records != null && records.Any()) {
                        pageCount = pageResult.Count;
                        pages = pageResult.Page;
                        pageSize = pageResult.Size;
                        records.ForEach(statute => statutes.Add(new() {
                            Id = statute.Id,
                            StatutoryTypeId = statute.TypeId,
                            StatutoryType = statute.RegulationType?.TypeName??string.Empty,
                            AuthorityId = statute.AuthorityId,
                            AuthorityName = statute.Authority?.AuthorityName?? string.Empty,
                            StatutoryLawCode = statute.Code ?? string.Empty,
                            StatutoryLawName = statute.RegulatoryName?? string.Empty,
                            IsDeleted = false,
                            CreatedBy = statute.CreatedBy ?? string.Empty,
                            ModifiedBy = statute.LastModifiedBy ?? string.Empty
                        }));
                    }

                    if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                        var searchTerm = request.SearchTerm.ToLower();
                        statutes = statutes.Where(u =>
                            (u.StatutoryType?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.AuthorityName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.StatutoryLawName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
                    }

                }

                return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(new PagedResponse<StatutoryRegulationResponse>(statutes, pageCount, pages, pageSize)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(error));
            }
        }

        [HttpPost("registers/paged-statute-list")]
        public async Task<IActionResult> GetPagedStatues([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _regulatoryService.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS,"No data","No statutes records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(new PagedResponse<StatutoryRegulationResponse>(new List<StatutoryRegulationResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<StatutoryRegulationResponse> statutes = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(statute => statutes.Add(new() {
                        Id = statute.Id,
                        StatutoryTypeId = statute.TypeId,
                        StatutoryType = statute.RegulationType?.TypeName ?? string.Empty,
                        AuthorityId = statute.AuthorityId,
                        AuthorityName = statute.Authority?.AuthorityName ?? string.Empty,
                        StatutoryLawCode = statute.Code ?? string.Empty,
                        StatutoryLawName = statute.RegulatoryName ?? string.Empty,
                        IsDeleted = false,
                        CreatedBy = statute.CreatedBy ?? string.Empty,
                        ModifiedBy = statute.LastModifiedBy ?? string.Empty
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    statutes = statutes.Where(u =>
                            (u.StatutoryType?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.AuthorityName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.StatutoryLawName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(new PagedResponse<StatutoryRegulationResponse>(statutes, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(error));
            }
        }

        [HttpPost("registers/create-statute")]
        public async Task<IActionResult> CreateStatute([FromBody] StatutoryRegulationRequest request) {
            try {
                Logger.LogActivity("Creating new regulatory statute", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Request record cannot be empty", "The regulatory statute record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.RegulatoryName) && !string.IsNullOrWhiteSpace(request.Code)) {
                    if (await _regulatoryService.ExistsAsync(s => s.RegulatoryName == request.RegulatoryName || s.Code == request.Code)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another record found with similar name or code");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Invalid Data, Statute name and code are required", "The Statute name and code are required");
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
                var result = await _regulatoryService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory statute saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save regulatory statute record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-statute")]
        public async Task<IActionResult> UpdateStatute([FromBody] StatutoryRegulationRequest request) {
            try {
                Logger.LogActivity("Update regulatory statute", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The regulatory statute record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.RegulatoryName) || string.IsNullOrWhiteSpace(request.Code)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid Record, Statute name and code are required", "Statute name and code are required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _regulatoryService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Regulatory statute record not found in the database");
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

                //..update regulatory statute
                var result = await _regulatoryService.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory statute updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update regulatory statute record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-statute")]
        public async Task<IActionResult> DeleteStatute([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _categoryService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Regulatory statute Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete statute
                var status = await _regulatoryService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to regulatory statute", "An error occurred! could delete regulatory statute");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting regulatory statute by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Statute Section

        [HttpPost("registers/statute-section-retrieve")]
        public async Task<IActionResult> GetStatuteSection([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get statute act/section by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatuteSectionResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request ID is required", "Invalid Statute act/section request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatuteSectionResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var section = await _articleService.GetAsync(a => a.Id == request.RecordId, false, a => a.Frequency);
                if (section == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Statute act/section not found", "No Statute act/section matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatuteSectionResponse>(error));
                }

                //..return regulatory statute act/section
                var result = new StatuteSectionResponse {
                    Id = section.Id,
                    Section = section.Article ?? string.Empty,
                    Summery = section.Summery ?? string.Empty,
                    Obligation = section.ObligationOrRequirement ?? string.Empty,
                    StatutoryId = section.StatuteId,
                    IsMandatory = section.IsMandatory,
                    ExcludeFromCompliance = section.ExcludeFromCompliance,
                    ComplianceAssurance = section.ComplianceAssurance,
                    IsCovered = section.IsCovered,
                    FrequencyId = section.FrequencyId ?? 0,
                    Coverage = section.Coverage,
                    ObligationFrequency = section.Frequency?.FrequencyName ?? string.Empty,
                    OwnerId = section.OwnerId ?? 0,
                    Owner = section.Owner?.ContactPosition ?? string.Empty,
                    Comments = section.Comments ?? string.Empty,
                    IsDeleted = false,
                    CreatedBy = section.CreatedBy ?? string.Empty,
                    ModifiedBy = section.LastModifiedBy ?? string.Empty,
                    Revisions = new(),
                    ComplianceIssues = new(),
                    
                };

                return Ok(new GrcResponse<StatuteSectionResponse>(result));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<StatuteSectionResponse>(error));
            }
        }

        [HttpPost("registers/statute-sections-list")]
        public async Task<IActionResult> GetLawSection([FromBody] StatuteListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<StatutoryRegulationResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _articleService.PageAllAsync(request.PageIndex, request.PageSize, false, a => a.StatuteId == request.ActivityTypeId, a => a.Frequency);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No statute acts/sections records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(new PagedResponse<StatuteSectionResponse>(
                            new List<StatuteSectionResponse>(), 0, pageResult.Page, pageResult.Size))
                        );
                }

                List<StatuteSectionResponse> sections = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(statute => sections.Add(new() {
                        Id = statute.Id,
                        Section = statute.Article ?? string.Empty,
                        Summery = statute.Summery ?? string.Empty,
                        Obligation = statute.ObligationOrRequirement ?? string.Empty,
                        StatutoryId = statute.StatuteId,
                        IsMandatory = statute.IsMandatory,
                        ExcludeFromCompliance = statute.ExcludeFromCompliance,
                        ComplianceAssurance = statute.ComplianceAssurance,
                        IsCovered = statute.IsCovered,
                        FrequencyId = statute.FrequencyId ?? 0,
                        Coverage = statute.Coverage,
                        ObligationFrequency = statute.Frequency?.FrequencyName ?? string.Empty,
                        OwnerId = statute.OwnerId ?? 0,
                        Owner = statute.Owner?.ContactPosition ?? string.Empty,
                        Comments = statute.Comments ?? string.Empty,
                        IsDeleted = false,
                        CreatedBy = statute.CreatedBy ?? string.Empty,
                        ModifiedBy = statute.LastModifiedBy ?? string.Empty,
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    sections = sections.Where(u =>
                            (u.Section?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.Summery?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.Obligation?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(new PagedResponse<StatuteSectionResponse>(
                        sections, pageResult.Count, pageResult.Page, pageResult.Size))
                    );
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(error));
            }
        }

        [HttpPost("paged-statute-sections-list")]
        public async Task<IActionResult> GetPagedSections([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _articleService.PageAllAsync(request.PageIndex, request.PageSize, false, null, a => a.Statute);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No statute acts/sections found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(new PagedResponse<StatuteSectionResponse>(
                        new List<StatuteSectionResponse>(), 0, pageResult.Page, pageResult.Size))
                        );
                }

                List<StatuteSectionResponse> sections = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(section => sections.Add(new() {
                        Id = section.Id,
                        Section = section.Article ?? string.Empty,
                        Summery = section.Summery ?? string.Empty,
                        Obligation = section.ObligationOrRequirement ?? string.Empty,
                        StatutoryId = section.StatuteId,
                        IsMandatory = section.IsMandatory, 
                        ExcludeFromCompliance = section.ExcludeFromCompliance,
                        ComplianceAssurance = section.ComplianceAssurance,
                        IsCovered = section.IsCovered,
                        FrequencyId = section.FrequencyId ?? 0,
                        Coverage = section.Coverage,
                        ObligationFrequency = section.Frequency?.FrequencyName ?? string.Empty,
                        OwnerId = section.OwnerId ?? 0,
                        Owner = section.Owner?.ContactPosition ?? string.Empty,
                        Comments = section.Comments ?? string.Empty,
                        IsDeleted = false,
                        CreatedBy = section.CreatedBy ?? string.Empty,
                        ModifiedBy = section.LastModifiedBy ?? string.Empty
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    sections = sections.Where(u =>
                            (u.Section?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.Summery?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.Obligation?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(new PagedResponse<StatuteSectionResponse>(
                        sections, pageResult.Count, pageResult.Page, pageResult.Size))
                    );
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<StatuteSectionResponse>>(error));
            }
        }

        [HttpPost("registers/create-statute-section")]
        public async Task<IActionResult> CreateSection([FromBody] StatutoryArticleRequest request) {
            try {
                Logger.LogActivity("Creating new regulatory act/section", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The regulatory act/section record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.Section) && !string.IsNullOrWhiteSpace(request.Summery)) {
                    if (await _articleService.ExistsAsync(s => s.Article == request.Section || s.Summery == request.Summery)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another record found with similar act/section, summery");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid Data, act/section, summery and obligation are required", "The act/section, summery and obligation are required");
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
                var result = await _articleService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Regulatory statute act/section saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save regulatory statute act/section record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-statute-section")]
        public async Task<IActionResult> UpdateSection([FromBody] StatutoryArticleRequest request) {
            try {
                Logger.LogActivity("Update regulatory statute", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The regulatory statute record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.Section) || string.IsNullOrWhiteSpace(request.Summery) || string.IsNullOrWhiteSpace(request.Obligation)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid Record, Act/Section, summery and obligation are required", "Act/Section, summery and obligation are required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _articleService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Statute Act/Section record not found in the database");
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

                //..update regulatory statute section
                var result = await _articleService.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Statute Act/Section updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update Statute Act/Section record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-statute-section")]
        public async Task<IActionResult> DeleteSection([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _articleService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Regulatory statute section Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete statute
                var status = await _articleService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to regulatory statute section", "An error occurred! could delete regulatory statute section");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting regulatory statute section by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
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

        #region Compliance Control Categories

        [HttpPost("registers/control-category-retrieve")]
        public async Task<IActionResult> GetControlCategory([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get Control Category by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty","Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlCategoryResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request Control Category ID is required", "Invalid Control Category request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlCategoryResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var response = await _controlCategoryService.GetAsync(d => d.Id == request.RecordId, true, d => d.ControlItems);
                if (response == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Control Category not found", "No control category matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlCategoryResponse>(error));
                }

                return Ok(new GrcResponse<ControlCategoryResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ControlCategoryResponse>(error));
            }
        }

        [HttpPost("registers/compliance-support")]
        public async Task<IActionResult> GetComplianceSupport([FromBody] GeneralRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError( ResponseCodes.BADREQUEST,"Request record cannot be empty","Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlSupportResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");

                //..get support data
                var _supportItemsList = await _controlCategoryService.GetSupportItemsAsync(false);
                return Ok(new GrcResponse<ControlSupportResponse>(_supportItemsList));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ControlSupportResponse>(error));
            }
        }

        [HttpPost("registers/paged-control-categories")]
        public async Task<IActionResult> GetPagedControlCategories([FromBody] ListRequest request) {
            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ControlCategoryResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _controlCategoryService.PageLookupAsync(request.PageIndex, request.PageSize, false,
                    category => new ControlCategoryResponse {
                        Id = category.Id,
                        CategoryName = category.CategoryName ?? string.Empty,
                        Comments = category.Notes ?? string.Empty,
                        Exclude = category.Exclude,
                        IsDeleted = category.IsDeleted,
                        ControlItems = category.ControlItems.Select(item => new ControlItemResponse() {
                            Id = item.Id,
                            CategoryId = item.ControlCategoryId,
                            ItemName = item.ItemName ?? string.Empty,
                            Comments = item.Notes ?? string.Empty,
                            Exclude = item.Exclude,
                            IsDeleted = item.IsDeleted
                        }).ToList()
                    });

                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No control categories found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(new PagedResponse<RegulatoryCategoryResponse>(
                        new List<RegulatoryCategoryResponse>(), 0, pageResult.Page, pageResult.Size))
                        );
                }

                List<ControlCategoryResponse> categories = new();
                var records = pageResult.Entities ?? new();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    categories = records.Where(u => u.CategoryName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
                } else {
                    categories = records;
                }

                return Ok(new GrcResponse<PagedResponse<ControlCategoryResponse>>(
                    new PagedResponse<ControlCategoryResponse>(categories, pageResult.Count, pageResult.Page, pageResult.Size))
                    );
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ControlCategoryResponse>>(error));
            }

        }

        [HttpPost("registers/create-control-category")]
        public async Task<IActionResult> CreateControlCategory([FromBody] ControlCategoryRequest request) {
            try {
                Logger.LogActivity("Creating new control category", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The Control Category record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.CategoryName)) {
                    if (await _controlCategoryService.ExistsAsync(t => t.CategoryName == request.CategoryName)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another Control Category found with similar name");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The Control Category name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.UserName = currentUser.Username;
                }

                //..create control category
                var result = await _controlCategoryService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Control category saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save control category record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-control-category")]
        public async Task<IActionResult> UpdateControlCategory([FromBody] ControlCategoryRequest request) {
            try {
                Logger.LogActivity("Update control category", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The control category record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.CategoryName)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid Record, Category name is required", "Category name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _controlCategoryService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Control category record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.UserName = currentUser.Username;
                } 
                //..update regulatory category
                var result = await _controlCategoryService.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Control category updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update control category record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/control-item-retrieve")]
        public async Task<IActionResult> GetControlItem([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get Control Item by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlItemResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request Control Item ID is required", "Invalid Control Item request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlItemResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _itemService.GetAsync(d => d.Id == request.RecordId, true);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Control Category not found", "No control item matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlItemResponse>(error));
                }

                //..map response
                var response = new ControlItemResponse {
                    Id = result.Id,
                    CategoryId = result.ControlCategoryId,
                    ItemName = result.ItemName ?? string.Empty,
                    Comments = result.Notes ?? string.Empty,
                    IsDeleted = result.IsDeleted,
                    Exclude = result.Exclude
                };

                return Ok(new GrcResponse<ControlItemResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ControlItemResponse>(error));
            }
        }

        [HttpPost("registers/create-control-item")]
        public async Task<IActionResult> CreateControlItemy([FromBody] ControlItemRequest request) {
            try {
                Logger.LogActivity("Creating new control item", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The Control item record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.ItemName)) {
                    if (await _itemService.ExistsAsync(t => t.ItemName == request.ItemName)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another Control item found with similar name");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The Control item name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.UserName = currentUser.Username;
                }

                //..create control category
                var result = await _itemService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Control item saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save control item record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-control-item")]
        public async Task<IActionResult> UpdateControlItem([FromBody] ControlItemRequest request) {
            try {
                Logger.LogActivity("Update control item", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The control item record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.ItemName)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid Record, Item name is required", "Item name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _controlCategoryService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Control item record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                if (currentUser != null) {
                    request.UserName = currentUser.Username;
                }
                //..update regulatory category
                var result = await _itemService.UpdateAsync(request, true);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Control item updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update control item record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-control-item")]
        public async Task<IActionResult> DeleteControlItem([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _itemService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Control item Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete document type
                var status = await _itemService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to control item", "An error occurred! could delete control item");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting control item by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/create-map")]
        public async Task<IActionResult> GetCreatMapping([FromBody] ComplianceItemMapRequest request) {
            try {
                Logger.LogActivity("Create compliance mapping by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlCategoryResponse>(error));
                }

                if (request.Items.Count == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Bad Request", "No items to map");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ControlCategoryResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _controlCategoryService.GenerateMapAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Compliance mapping completed successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to complete mapping. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Compliance Issues

        [HttpPost("registers/compliance-issue-retrieve")]
        public async Task<IActionResult> GetComplianceIssue([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get Compliance Issue by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ComplianceIssueResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request Compliance Issue ID is required", "Invalid Control Item request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ComplianceIssueResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _issueService.GetAsync(d => d.Id == request.RecordId, true);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Compliance Issue not found", "No compliance issue matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ComplianceIssueResponse>(error));
                }

                //..map response
                var response = new ComplianceIssueResponse {
                    Id = result.Id,
                    ArticleId = result.StatutoryArticleId,
                    Description = result.Description ?? string.Empty,
                    Comments = result.Notes ?? string.Empty,
                    IsClosed = result.IsClosed,
                    IsDeleted = result.IsDeleted
                };

                return Ok(new GrcResponse<ComplianceIssueResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ComplianceIssueResponse>(error));
            }
        }

        [HttpPost("registers/article-issues")]
        public async Task<IActionResult> GetArticleIssues([FromBody] IssueListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ListResponse<ComplianceIssueResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var result = await _issueService.GetAllAsync(i => i.StatutoryArticleId == request.ArticleId);
                if (result == null || !result.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No article issues found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<ComplianceIssueResponse>>(new List<ComplianceIssueResponse>()));
                }

                List<ComplianceIssueResponse> issues = new();
                result.ToList().ForEach(issue => issues.Add(new ComplianceIssueResponse() {
                    Id = issue.Id,
                    ArticleId = issue.StatutoryArticleId,
                    Description = issue.Description ?? string.Empty,
                    Comments = issue.Notes ?? string.Empty,
                    IsClosed = issue.IsClosed,
                    IsDeleted = issue.IsDeleted
                }));

                return Ok(new GrcResponse<List<ComplianceIssueResponse>>(issues));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<ListResponse<ComplianceIssueResponse>>(error));
            }
        }

        [HttpPost("registers/create-article-issue")]
        public async Task<IActionResult> CreateArticleIssue([FromBody] ComplianceIssueRequest request) {
            try {
                Logger.LogActivity("Creating new compliance issue", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The compliance issue record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.Description)) {
                    if (await _issueService.ExistsAsync(i => i.Description == request.Description)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another issue found with similar description");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Issue description cannot be empty","The description is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.Comments)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Issue notes cannot be empty", "The notes is required");
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
                request.UserName = currentUser.Username;

                //..create document type
                var result = await _issueService.InsertAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Issue saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save issuerecord. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/update-article-issue")]
        public async Task<IActionResult> UpdateArticleIssue([FromBody] ComplianceIssueRequest request) {
            try {
                Logger.LogActivity("Update Compliance issue", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The Compliance issue record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _issueService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Compliance issue record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.Description)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Issue description cannot be empty", "The description is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.Comments)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Issue notes cannot be empty", "The notes is required");
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
                request.UserName = currentUser.Username;

                //..update compliance issue
                var result = await _issueService.UpdateAsync(request);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Compliance issue updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update compliance issue record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("registers/delete-article-issue")]
        public async Task<IActionResult> DeleteArticleIssue([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _issueService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Compliance issue Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete issue
                var status = await _issueService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to compliance issue", "An error occurred! could delete compliance issue");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting compliance issue by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Returns

        [HttpPost("returns/return-retrieve")]
        public async Task<IActionResult> GetReturn([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get Return by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<ComplianceIssueResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request Return ID is required", "Invalid Return request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatutoryReturnReportResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _returnService.GetAsync(d => d.Id == request.RecordId, true, r => r.Authority, r => r.Article, r => r.Frequency, r => r.Department, r => r.ReturnType);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Return/Report not found", "No return/report matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<StatutoryReturnReportResponse>(error));
                }

                //..map response
                var response = new StatutoryReturnReportResponse {
                    Id = result.Id,
                    StatuteId = result.Article?.Id ?? 0,
                    Statute = result.Article?.Article ?? string.Empty,
                    ReturnName = result.ReturnName ?? string.Empty,
                    SendReminder = result.SendReminder,
                    Interval = result.Interval,
                    IntervalType = result.IntervalType,
                    Reminder = result.Reminder,
                    TypeId = result.ReturnType?.Id ?? 0,
                    Type = result.ReturnType?.TypeName ?? string.Empty,
                    DepartmentId = result.Department?.Id ?? 0,
                    Department = result.Department?.DepartmentName ?? string.Empty,
                    AuthorityId = result.Authority?.Id ?? 0,
                    Authority = result.Authority?.AuthorityName ?? string.Empty,
                    FrequencyId = result.Frequency?.Id ?? 0,
                    Frequency = result.Frequency?.FrequencyName ?? string.Empty,
                    Comments = result.Comments ?? string.Empty,
                    Risk = result.Risk ?? string.Empty,
                    IsDeleted = result.IsDeleted
                };

                return Ok(new GrcResponse<StatutoryReturnReportResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<StatutoryReturnReportResponse>(error));
            }
        }

        [HttpPost("returns/create-return")]
        public async Task<IActionResult> CreateReturn([FromBody] StatutoryReturnReportRequest request) {
            try {
                Logger.LogActivity("Creating new return/report", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The return/report record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.ReturnName)) {
                    if (await _returnService.ExistsAsync(i => i.ReturnName == request.ReturnName)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another return/report found with similar name");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Return/report cannot be empty", "The name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.Comments)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Return/report notes cannot be empty", "The notes is required");
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

                var record = new ReturnRequest() {
                    ArticleId = request.StatuteId,
                    ReturnName = request.ReturnName,
                    TypeId = request.TypeId,
                    AuthorityId = request.AuthorityId,
                    FrequencyId = request.FrequencyId,
                    DepartmentId = request.DepartmentId,
                    UserName = currentUser.Username,
                    Action = request.Action,
                    IpAddress = request.IPAddress,
                    Comments = request.Comments,
                    IsDeleted = request.IsDeleted,
                };

                //..create return/report
                var result = await _returnService.InsertAsync(record);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Issue saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save issuerecord. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("returns/update-return")]
        public async Task<IActionResult> UpdateReturn([FromBody] StatutoryReturnReportRequest request) {
            try {
                Logger.LogActivity("Update Return/report", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The return/report record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _issueService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Return/Report record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.ReturnName)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Report/return name cannot be empty", "The name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                if (string.IsNullOrWhiteSpace(request.Comments)) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Report/return notes cannot be empty", "The notes is required");
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

                var record = new ReturnRequest() {
                    Id = request.Id,
                    ArticleId = request.StatuteId,
                    ReturnName = request.ReturnName,
                    TypeId = request.TypeId,
                    AuthorityId = request.AuthorityId,
                    FrequencyId = request.FrequencyId,
                    DepartmentId = request.DepartmentId,
                    UserName = currentUser.Username,
                    Action = request.Action,
                    IpAddress = request.IPAddress,
                    Comments = request.Comments,
                    IsDeleted = request.IsDeleted,
                };

                //..update report
                var result = await _returnService.UpdateAsync(record);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Return/report updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update report/return record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("returns/delete-return")]
        public async Task<IActionResult> DeleteReturn([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _returnService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Return/report Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete return/report
                var status = await _returnService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to return/report", "An error occurred! could delete return/report");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting report/return by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("returns/returns-list")]
        public async Task<IActionResult> GetReturnsList([FromBody] RecordListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ReturnsResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _returnService.PageAllAsync(request.PageIndex, request.PageSize, false, 
                    r=> r.FrequencyId == request.RecordId, r=>r.Frequency, r=> r.Department,r=> r.ReturnType, r=>r.Authority);

                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS,"No data", "No return/report records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ReturnsResponse>>(new PagedResponse<ReturnsResponse>(new List<ReturnsResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<ReturnsResponse> returns = new();
                var records = pageResult.Entities ?? new();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    records = records.Where(u => u.ReturnName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
                }

                returns = records.Select(rec => new ReturnsResponse() {
                    Id = rec.Id,
                    ReportName = rec.ReturnName ?? string.Empty,
                    Article = $"{rec.Article?.Article ?? string.Empty}{rec.Article?.Summery ?? string.Empty}",
                    Frequency = rec.Frequency?.FrequencyName ?? string.Empty,
                    Department = rec.Department?.DepartmentName ?? string.Empty,
                    Type = rec.ReturnType?.TypeName ?? string.Empty,
                    IsDeleted = rec.IsDeleted,
                    Risk = rec.Risk ??string.Empty,
                    Authority = rec.Authority?.AuthorityName ?? string.Empty,
                    Comments = rec.Comments??string.Empty

                }).ToList();

                return Ok(new GrcResponse<PagedResponse<ReturnsResponse>>(new PagedResponse<ReturnsResponse>(returns, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ReturnsResponse>>(error));
            }
        }

        [HttpPost("returns/returns-submissions-list")]
        public async Task<IActionResult> GetReturnSubmissions([FromBody] RecordListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ReturnSubmissionResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _submissionService.PageAllAsync(request.PageIndex, request.PageSize, false, 
                                                         r => r.ReturnId == request.RecordId, 
                                                         r => r.RegulatoryReturn,
                                                         r => r.RegulatoryReturn.Article,
                                                         r => r.RegulatoryReturn.Department);

                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No submissions records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ReturnSubmissionResponse>>(new PagedResponse<ReturnSubmissionResponse>(new List<ReturnSubmissionResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<ReturnSubmissionResponse> returns = new();
                var records = pageResult.Entities ?? new();
                if (records.Count > 0) {
                    records.ForEach(s => {
                        returns.Add(new ReturnSubmissionResponse() {
                            Id = s.Id,
                            Report = s.RegulatoryReturn.ReturnName,
                            Title = s.RegulatoryReturn.ReturnName,
                            Article = $"{s.RegulatoryReturn?.Article.Article ?? string.Empty} {s.RegulatoryReturn?.Article.Summery ?? string.Empty}",
                            PeriodEnd = s.PeriodEnd,
                            PeriodStart = s.PeriodStart,
                            Status = s.IsBreached ? "BREACHED" : s.Status,
                            BreachReason = s.BreachReason,
                            SubmittedOn = s.SubmissionDate,
                            SubmittedBy = s.SubmittedBy,
                            Risk = s.RegulatoryReturn.Risk,
                            Comment = s.Comments,
                            IsBreached = s.IsBreached,
                            IsDeleted = s.IsDeleted,
                            Department = s.RegulatoryReturn?.Department?.DepartmentName ?? string.Empty

                        });
                    });
                }

                return Ok(new GrcResponse<PagedResponse<ReturnSubmissionResponse>>(new PagedResponse<ReturnSubmissionResponse>(returns, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ReturnSubmissionResponse>>(error));
            }
        }

        [HttpPost("returns/paged-returns-list")]
        public async Task<IActionResult> GetPagedReturnsList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<ReturnsResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _categoryService.PageLookupAsync(request.PageIndex, request.PageSize, false,
                    category => new RegulatoryCategoryResponse {
                        Id = category.Id,
                        CategoryName = category.CategoryName ?? string.Empty,
                        Comments = category.Comments ?? string.Empty,
                        CreatedOn = category.CreatedOn,
                        CreatedBy = category.CreatedBy,
                        Statutes = category.Regulations.Select(law => new StatutoryRegulationResponse() {
                            Id = law.Id,
                            CategoryId = law.CategoryId,
                            CategoryName = category.CategoryName ?? string.Empty,
                            StatutoryTypeId = law.TypeId,
                            StatutoryType = law.RegulationType.TypeName ?? string.Empty,
                            AuthorityId = law.AuthorityId,
                            AuthorityName = law.Authority.AuthorityName ?? string.Empty,
                            StatutoryLawCode = law.Code ?? string.Empty,
                            StatutoryLawName = law.RegulatoryName ?? string.Empty,
                            IsDeleted = law.IsDeleted,
                            CreatedBy = law.CreatedBy ?? string.Empty,
                            ModifiedBy = law.LastModifiedBy ?? string.Empty
                        }).ToList()
                    });

                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(
                        ResponseCodes.SUCCESS,
                        "No data",
                        "No regulatory statute records found"
                    );
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(new PagedResponse<RegulatoryCategoryResponse>(
                        new List<RegulatoryCategoryResponse>(), 0, pageResult.Page, pageResult.Size))
                        );
                }

                List<RegulatoryCategoryResponse> categories = new();
                var records = pageResult.Entities ?? new();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    categories = records.Where(u => u.CategoryName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
                } else {
                    categories = records;
                }

                return Ok(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(
                    new PagedResponse<RegulatoryCategoryResponse>(categories, pageResult.Count, pageResult.Page, pageResult.Size))
                    );
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<ReturnsResponse>>(error));
            }
        }

        [HttpPost("returns/frequency-returns")]
        public async Task<IActionResult> GetFrequencyReturnsList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<FrequencyResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _frequencyService.PageLookupAsync(request.PageIndex, request.PageSize, false,
                    frequencies => new FrequencyResponse {
                        Id = frequencies.Id,
                        FrequencyName = frequencies.FrequencyName ?? string.Empty,
                        IsDeleted = frequencies.IsDeleted,
                        Returns = frequencies.Returns.Select(report => new ReturnReportResponse() {
                            Id = report.Id,
                            Title = report.ReturnName ?? string.Empty,
                            BreachRisk = report.Risk ?? string.Empty,
                            SendReminder = report.SendReminder,
                            Interval = report.Interval,
                            IntervalType = report.IntervalType,
                            Reminder = report.Reminder,
                            DepartmentId = report.DepartmentId,
                            Department = report.Department.DepartmentName ?? string.Empty,
                            ArticleNo = report.Article.Article ?? string.Empty,
                            ArticleSummery = report.Article.Summery ?? string.Empty,
                            AuthorityId = report.AuthorityId,
                            Authority = report.Authority.AuthorityAlias ?? string.Empty,
                        }).ToList()
                    });

                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No Frequency reports records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<FrequencyResponse>>(new PagedResponse<FrequencyResponse>(new List<FrequencyResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<FrequencyResponse> frquencies = new();
                var records = pageResult.Entities ?? new();
                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    frquencies = records.Where(u => u.FrequencyName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
                } else {
                    frquencies = records;
                }

                return Ok(new GrcResponse<PagedResponse<FrequencyResponse>>(new PagedResponse<FrequencyResponse>(frquencies, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<FrequencyResponse>>(error));
            }
        }

        #endregion

        #region Circulars

        [HttpPost("circulars/paged-circulars-list")]
        public async Task<IActionResult> GetPagedCircularList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<CircularsResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _circularService.PageAllAsync(request.PageIndex, request.PageSize, false,
                            c => c.Department, c => c.Frequency, r => r.Authority);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS,"No data", "No circular records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<CircularsResponse>>(
                              new PagedResponse<CircularsResponse>(new List<CircularsResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<CircularsResponse> circulars = new();

                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(circular => circulars.Add(new CircularsResponse {
                        Id = circular.Id,
                        CircularTitle = circular.CircularTitle ?? string.Empty,
                        Requirement = circular.Requirement ?? string.Empty,
                        FilePath = circular.FilePath ?? string.Empty,
                        RefNumber = circular.Reference ?? string.Empty,
                        Authority = circular.Authority?.AuthorityAlias ?? string.Empty,
                        Frequency = circular.Frequency?.FrequencyName ?? "N/A",
                        Department = circular.Department?.DepartmentName ?? string.Empty,
                        Status = circular.Status ?? string.Empty,
                        IsBreached = circular.IsBreached,
                        BreachReason = circular.BreachReason,
                        BreachRisk = circular.BreachRisk,
                        Comments = circular.Comments ?? string.Empty,
                        RecievedOn = circular.RecievedOn,
                        DeadlineOn = circular.DeadlineOn,
                        SubmissionDate = circular.SubmissionDate,
                        SubmittedBy = circular.SubmittedBy ?? string.Empty,
                        Issues = circular.Issues != null && circular.Issues.Count > 0
                            ? circular.Issues.Select(issue => new CircularIssueResponse {
                                Id = issue.Id,
                                IssueDescription = issue.IssueDescription,
                                Resolution = issue.Resolution,
                                Status = issue.Status,
                            }).ToList()
                            : new List<CircularIssueResponse>()
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    circulars = circulars.Where(u =>
                        (u.CircularTitle?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Department?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Authority?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) 
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<CircularsResponse>>(
                          new PagedResponse<CircularsResponse>(circulars, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<CircularsResponse>>(error));
            }
        }

        #endregion

        #region Submissions

        [HttpPost("returns/submission-retrieve")]
        public async Task<IActionResult> GetSubmission([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get submission by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<SubmissionResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request Submission ID is required", "Invalid Submission request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<SubmissionResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _submissionService.GetAsync(d => d.Id == request.RecordId, true, r => r.RegulatoryReturn, r => r.RegulatoryReturn.Department, r => r.RegulatoryReturn.Department);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "submission/Report not found", "No submission/report matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<SubmissionResponse>(error));
                }

                //..map response
                var response = new SubmissionResponse {
                    Id = result.Id,
                    Report = result.RegulatoryReturn?.ReturnName ?? string.Empty,
                    Title = result.RegulatoryReturn?.ReturnName ?? string.Empty,
                    PeriodStart = result.PeriodStart,
                    PeriodEnd = result.PeriodEnd,
                    Status = result.Status ?? string.Empty,
                    OwnerId = result.RegulatoryReturn?.Department?.Id ?? 0,
                    Department = result.RegulatoryReturn?.Department?.DepartmentName,
                    IsDeleted = result.IsDeleted,
                    IsBreached = result.IsBreached,
                    BreachReason = result.BreachReason ?? string.Empty,
                    Risk = result.RegulatoryReturn?.Risk ?? string.Empty
                };

                return Ok(new GrcResponse<SubmissionResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<SubmissionResponse>(error));
            }
        }

        [HttpPost("returns/update-submission")]
        public async Task<IActionResult> UpdateSubmission([FromBody] SubmissionRequest request) {
            try {
                Logger.LogActivity("Update submission", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The submission record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _submissionService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Submission record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username = string.Empty;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..update document type
                var result = await _submissionService.UpdateAsync(request, username);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Return submission updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update Return submission record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("circulars/submission-retrieve")]
        public async Task<IActionResult> GetCircularSubmission([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get submission by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<CircularSubmissionResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request Submission ID is required", "Invalid Submission request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<CircularSubmissionResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _circularService.GetAsync(c => c.Id == request.RecordId, true, c => c.Authority, c => c.Frequency, c => c.Department, c => c.Issues);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "submission/Report not found", "No submission/report matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<CircularSubmissionResponse>(error));
                }

                //..map response
                var response = new CircularSubmissionResponse {
                    Id = result.Id,
                    Reference = result.Reference?? string.Empty,
                    CircularTitle = result.CircularTitle ?? string.Empty,
                    CircularRequirement = result.Requirement ?? string.Empty,
                    RecievedOn = result.RecievedOn,
                    Deadline = result.DeadlineOn,
                    AuthorityId = result?.Authority?.Id?? 0,
                    Authority = result?.Authority?.AuthorityAlias?? string.Empty,
                    FrequencyId = result?.Frequency?.Id ?? 0,
                    Frequency = result?.Frequency?.FrequencyName ?? string.Empty,
                    CircularStatus = result.Status ?? string.Empty,
                    OwnerId = result.Department?.Id ?? 0,
                    Department = result.Department?.DepartmentName,
                    FilePath = result.FilePath ?? string.Empty,
                    IsDeleted = result.IsDeleted,
                    IsBreached = result.IsBreached,
                    BreachReason = result.BreachReason ?? string.Empty,
                    BreachRisk = result.BreachRisk ?? string.Empty,
                    Comments = result.Comments ?? string.Empty,
                    SubmittedOn = result.SubmissionDate,
                    SubmittedBy = result.SubmittedBy ?? string.Empty,
                    Issues = result.Issues != null && result.Issues.Count > 0
                            ? result.Issues.Select(issue => new CircularIssueResponse {
                                Id = issue.Id,
                                IssueDescription = issue.IssueDescription,
                                Resolution = issue.Resolution,
                                Status = issue.Status,
                            }).ToList()
                            : new List<CircularIssueResponse>()
                };

                return Ok(new GrcResponse<CircularSubmissionResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<CircularSubmissionResponse>(error));
            }
        }

        [HttpPost("circulars/update-submission")]
        public async Task<IActionResult> UpdateCircularSubmission([FromBody] CircularSubmissionRequest request) {
            try {
                Logger.LogActivity("Update submission", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The submission record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _circularService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Submission record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username = string.Empty;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..update document type
                var result = await _circularService.UpdateSubmissionAsync(request, username);
                var response = new GeneralResponse();
                if (result) {

                    //..send submission messages
                    var owners = await _ownerService.GetAllAsync(d => d.Id == request.DepartmentId);
                    var emails = new Dictionary<string, string>();
                    if (owners != null && owners.Any()) {
                        foreach (var o in owners) {
                            if (emails.ContainsKey(o.ContactName))
                                continue;
                           emails[o.ContactName] = o.ContactEmail;
                        }

                    }

                    string msg = "Circular submission updated successfully";
                    if (emails.Any()) {
                        
                        var mailSettings = await _mailService.GetMailSettingsAsync();
                        if (mailSettings is null) {
                            msg += ". Mail settings not found. No mail sent";
                        } else {
                            foreach (var kv in emails) {
                                var sendToName = kv.Key;
                                var email = kv.Value;
                                string title = "";
                                var (sent, subject, mail) = MailHandler.GenerateSubmissionMail(Logger, mailSettings.MailSender, email, 
                                    sendToName, mailSettings.CopyTo, "CURCULAR", title, mailSettings.NetworkPort, mailSettings.SystemPassword);
                                if (sent) {
                                    await _mailService.InsertMailAsync(new Data.Entities.System.MailRecord() {
                                        SentToEmail = email,
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
                                } else {
                                    msg += ". Notification email not sent. Something went erong";
                                }
                            }
                                
                        }

                    }

                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = msg;
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to Circular submission record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Audit-statistics

        [HttpPost("audits/audit-dashboard-statistics")]
        public async Task<IActionResult> GetAuditStatistic([FromBody] GeneralRequest request) {

            try {
                Logger.LogActivity("Retrieve compliance statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditDashboardResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _auditService.GetAuditDashboardStatisticsAsync(false);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could audit dashboard statistics. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<AuditDashboardResponse>>(error));
                }

                return Ok(new GrcResponse<AuditDashboardResponse>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuditDashboardResponse>(error));
            }
        }

        [HttpPost("audits/exception-report")]
        public async Task<IActionResult> GetExceptionStatistic([FromBody] IdRequest request) {

            try {
                Logger.LogActivity("Retrieve compliance statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditMiniReportResponse>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IPAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _auditService.GetAuditMiniStatisticsAsync(request.RecordId, false);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could audit mini report. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditMiniReportResponse>(error));
                }

                return Ok(new GrcResponse<AuditMiniReportResponse>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuditMiniReportResponse>(error));
            }
        }

        [HttpPost("audits/exception-extension-report")]
        public async Task<IActionResult> GetAuditExtensionStatistic([FromBody] AuditExtensionExceptionRequest request) {
            try {

                Logger.LogActivity("Retrieve compliance statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<AuditMiniReportResponse>>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IpAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _auditService.GetPeriodStatisticsAsync(request.Period, false);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could audit mini report. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditExtensionStatistics>(error));
                }

                return Ok(new GrcResponse<AuditExtensionStatistics>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuditExtensionStatistics>(error));
            }
        }

        [HttpPost("audits/audit-mini-report")]
        public async Task<IActionResult> GetAuditMiniReport([FromBody] AuditListRequest request) {
            try {

                Logger.LogActivity("Retrieve compliance statistics", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<AuditMiniReportResponse>>(error));
                }
                Logger.LogActivity($"ACTION >>{request.Action}:: IPADDRESS >> {request.IpAddress}", "INFO");
                Logger.LogActivity($"REQUEST BODY >> {JsonSerializer.Serialize(request)}", "INFO");

                var statistics = await _auditService.GetMiniPeriodStatisticsAsync(request.Period, false);
                if (statistics == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "An error occurred", "Could audit mini report. A system error occurred");
                    Logger.LogActivity($"SYSTEM ERROR: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<List<AuditMiniReportResponse>>(error));
                }

                return Ok(new GrcResponse<List<AuditMiniReportResponse>>(statistics));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<List<AuditMiniReportResponse>>(error));
            }
        }

        #endregion

        #region Audit

        [HttpPost("audits/audit-retrieve")]
        public async Task<IActionResult> GetAudit([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get audit by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request audit ID is required", "Invalid audit request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _auditService.GetAsync(d => d.Id == request.RecordId, false, a => a.Authority, a => a.AuditType);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Audit not found", "No audit matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditResponse>(error));
                }

                //..return audit
                var status = new AuditResponse {
                    Id = result.Id,
                    AuditName = result.AuditName ?? string.Empty,
                    Notes = result.Notes ?? string.Empty,
                    Authority = result.Authority?.AuthorityAlias ?? string.Empty,
                    AuthorityId = result.AuthorityId,
                    TypeId = result.AuditTypeId,
                    TypeName = result.AuditType.TypeName ?? string.Empty,
                    IsDeleted = result.IsDeleted
                };

                return Ok(new GrcResponse<AuditResponse>(status));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuditResponse>(error));
            }
        }

        [HttpPost("audits/paged-audit-list")]
        public async Task<IActionResult> GetPagedAuditList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _auditService.PageAllAsync(request.PageIndex, request.PageSize, false, a => a.Authority, a => a.AuditType);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No audit records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditResponse>>(
                        new PagedResponse<AuditResponse>(
                        new List<AuditResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<AuditResponse> audits = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(audit => audits.Add(new AuditResponse() {
                        Id = audit.Id,
                        AuditName = audit.AuditName ?? string.Empty,
                        Notes = audit.Notes ?? string.Empty,
                        Authority = audit.Authority?.AuthorityAlias ?? string.Empty,
                        AuthorityId = audit.AuthorityId,
                        TypeId = audit.AuditTypeId,
                        TypeName = audit.AuditType.TypeName ?? string.Empty,
                        IsDeleted = audit.IsDeleted
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    audits = audits.Where(u =>
                        (u.AuditName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Authority?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TypeName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) 
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<AuditResponse>>(new PagedResponse<AuditResponse>(audits, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<AuditResponse>>(error));
            }
        }

        [HttpPost("audits/create-audit")]
        public async Task<IActionResult> CreateAudit([FromBody] AuditRequest request) {
            try {
                Logger.LogActivity("Creating new audit", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.AuditName)) {
                    if (await _auditService.ExistsAsync(t => t.AuditName == request.AuditName)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another audit found with similar name");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The Audit name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..create audit
                var result = await _auditService.InsertAsync(request, username);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit task saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save audit record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/update-audit")]
        public async Task<IActionResult> UpdateAudit([FromBody] AuditRequest request) {
            try {
                Logger.LogActivity("Update auidt", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _auditService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Audit record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..update document type
                var result = await _auditService.UpdateAsync(request, username, true);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update audit record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/delete-audit")]
        public async Task<IActionResult> DeleteAudit([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _auditService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Audi Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete audit type
                var status = await _auditService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to regulatory audit", "An error occurred! could delete audit");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting audit by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Audit-Reports

        [HttpPost("audits/report-retrieve")]
        public async Task<IActionResult> GetAuditReport([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get audit report by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<DocumentTypeResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request audit report ID is required", "Invalid audit type request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditReportResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var register = await _auditReportService.GetAsync(r => r.Id == request.RecordId, false, r => r.AuditExceptions, r => r.AuditUpdates);
                if (register == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Audit report not found", "No audit report matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditReportResponse>(error));
                }

                //..return audit report
                var report = new AuditReportResponse {
                    Id = register.Id,
                    Reference = register.Reference ?? string.Empty,
                    ReportName = register.ReportName ?? string.Empty,
                    Summery = register.Summery ?? string.Empty,
                    ReportStatus = register.Status ?? string.Empty,
                    ReportDate = register.AuditedOn,
                    ExceptionCount = register.ExceptionCount,
                    ResponseDate = register.RespondedOn,
                    ManagementComments = register.ManagementComment ?? string.Empty,
                    AdditionalNotes = register.AdditionalNotes ?? string.Empty,
                    AuditType = register.AdditionalNotes ?? string.Empty,
                    IsDeleted = register.IsDeleted,
                    Findings = register.AuditExceptions != null && register.AuditExceptions.Any() ?
                               register.AuditExceptions.Select(exec=> new AuditExceptionResponse() { 
                                Id = exec.Id,
                                ReportId = exec.AuditReportId,
                                Finding = exec.AuditFinding ?? string.Empty,
                                ProposedAction = exec.RemediationPlan ?? string.Empty,
                                CorrectiveAction =exec.CorrectiveAction ?? string.Empty,
                                Notes = exec.ExceptionNotes,
                                TargetDate = exec.TargetDate,
                                RiskRating = exec.RiskRating,
                                RiskStatement = exec.RiskAssessment,
                                ResponsibleId = exec.ResponsabilityId,
                                Responsible = exec.Responseability?.ContactPosition ?? string.Empty,
                                Executioner = exec.Executioner ?? string.Empty,
                                Status = exec.Status ?? string.Empty,
                                IsDeleted = exec.IsDeleted
                               }).ToList():
                               new List<AuditExceptionResponse>(),
                    Updates = register.AuditUpdates != null && register.AuditUpdates.Any() ?
                               register.AuditUpdates.Select(update => new AuditUpdateResponse() {
                                   Id = update.Id,
                                   ReportId = update.ReportId,
                                   UpdateNotes = update.Notes ?? string.Empty,
                                   ReminderMessage = update.ReminderMessage ?? string.Empty,
                                   SendToEmails = update.SendTo ?? string.Empty,
                                   SendReminders = update.SendReminder,
                                   SendDate = update.SendReminderOn,
                                   AddedBy = update.AddedBy ?? string.Empty,
                                   IsDeleted = update.IsDeleted
                               }).ToList() :
                               new List<AuditUpdateResponse>(),
                };

                return Ok(new GrcResponse<AuditReportResponse>(report));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuditReportResponse>(error));
            }
        }

        [HttpPost("audits/paged-reports-list")]
        public async Task<IActionResult> GetPagedAuditReportList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditReportResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _auditReportService.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No audit report records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditReportResponse>>(
                        new PagedResponse<AuditReportResponse>(
                        new List<AuditReportResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<AuditReportResponse> reports = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(report => reports.Add(new() {
                        Id = report.Id,
                        Reference = report.Reference ?? string.Empty,
                        ReportName = report.ReportName ?? string.Empty,
                        Summery = report.Summery ?? string.Empty,
                        ReportStatus = report.Status ?? string.Empty,
                        ReportDate = report.AuditedOn,
                        ExceptionCount = report.ExceptionCount,
                        ResponseDate = report.RespondedOn,
                        ManagementComments = report.ManagementComment ?? string.Empty,
                        AdditionalNotes = report.AdditionalNotes ?? string.Empty,
                        AuditType = report.AdditionalNotes ?? string.Empty,
                        IsDeleted = report.IsDeleted
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    reports = reports.Where(u =>
                        (u.Reference?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)||
                        (u.ReportName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)||
                        (u.AuditType?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.ReportStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<AuditReportResponse>>(
                    new PagedResponse<AuditReportResponse>(reports, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<AuditReportResponse>>(error));
            }
        }

        [HttpPost("audits/create-report")]
        public async Task<IActionResult> CreateAuditReport([FromBody] AuditReportRequest request) {
            try {
                Logger.LogActivity("Creating new audit report", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit report record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.ReportName)) {
                    if (await _auditReportService.ExistsAsync(t => t.ReportName == request.ReportName)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another audit report found with similar name");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The report name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..create audit report
                var result = await _auditReportService.InsertAsync(request, username);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit report saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save audit report record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/update-report")]
        public async Task<IActionResult> UpdateAuditRepor([FromBody] AuditReportRequest request) {
            try {
                Logger.LogActivity("Update auidt report", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit report record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} >> ${request.IpAddress}", "INFO");
                if (!await _auditTypeService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Audit report record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..update document type
                var result = await _auditReportService.UpdateAsync(request, username, true);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit report updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update audit report record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/delete-report")]
        public async Task<IActionResult> DeleteAuditReport([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _auditReportService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Audi report Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete audit report
                var status = await _auditReportService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to regulatory audit report", "An error occurred! could delete audit report");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting audit report by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Audit-Excptions

        [HttpPost("audits/exception-retrieve")]
        public async Task<IActionResult> GetAuditException([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get audit exception by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditExceptionResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request audit result ID is required", "Invalid audit exception request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditExceptionResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _auditExceptionService.GetAsync(d => d.Id == request.RecordId, false, ex=> ex.AuditTasks, ex=> ex.Responseability);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Audit result not found", "No audit exception matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditExceptionResponse>(error));
                }

                //..return audit exception
                var exception = new AuditExceptionResponse {
                    Id = result.Id,
                    Finding = result.AuditFinding,
                    ProposedAction = result.RemediationPlan,
                    CorrectiveAction = result.CorrectiveAction,
                    Notes = result.ExceptionNotes,
                    TargetDate = result.TargetDate,
                    RiskStatement = result.RiskAssessment,
                    RiskRating = result.RiskRating,
                    ResponsibleId = result.ResponsabilityId,
                    Responsible = result.Responseability?.ContactName ?? string.Empty,
                    Executioner = result.Executioner,
                    Status = result.Status,
                    ReportId = result.AuditReportId,
                    IsDeleted = result.IsDeleted,
                    Tasks = result.AuditTasks != null && result.AuditTasks.Any()?
                        result.AuditTasks.Select(task => new AuditTaskResponse() {
                            Id = task.Id,
                            TaskName = task.TaskName ?? string.Empty,
                            TaskDescription = task.Description ?? string.Empty,
                            Duedate = task.DueDate,
                            TaskStatus = task.Status ?? string.Empty,
                            OwnerId = task.OwnerId,
                            ExceptionId = task.ExceptionId,
                            IsDeleted = task.IsDeleted

                        }).ToList():
                        new List<AuditTaskResponse>()
                };

                return Ok(new GrcResponse<AuditExceptionResponse>(exception));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuditExceptionResponse>(error));
            }
        }

        [HttpPost("audits/report-exceptions")]
        public async Task<IActionResult> GetPagedExceptionList([FromBody] AuditCategoryRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditExceptionResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _auditExceptionService.PageAllAsync(request.PageIndex, request.PageSize, false, e => e.Responseability, e => e.AuditReportId == request.ReportId);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No audit exception records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditExceptionResponse>>(
                        new PagedResponse<AuditExceptionResponse>(
                        new List<AuditExceptionResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<AuditExceptionResponse> exceptions = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(exception => exceptions.Add(new() {
                        Id = exception.Id,
                        Finding = exception.AuditFinding,
                        ProposedAction  = exception.RemediationPlan,
                        CorrectiveAction = exception.CorrectiveAction,
                        Notes = exception.ExceptionNotes,
                        TargetDate = exception.TargetDate,
                        RiskStatement = exception.RiskAssessment,
                        RiskRating = exception.RiskRating,
                        ResponsibleId = exception.ResponsabilityId,
                        Responsible = exception.Responseability?.ContactName ?? string.Empty,
                        Executioner = exception.Executioner,
                        Status = exception.Status,
                        ReportId = exception.AuditReportId,
                        IsDeleted = exception.IsDeleted
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    exceptions = exceptions.Where(u =>
                        (u.Finding?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)||
                        (u.Responsible?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)||
                        (u.ProposedAction?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)||
                        (u.CorrectiveAction?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Executioner?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<AuditExceptionResponse>>(
                          new PagedResponse<AuditExceptionResponse>(exceptions, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<AuditTypeResponse>>(error));
            }
        }

        [HttpPost("audits/create-exception")]
        public async Task<IActionResult> CreateAuditException([FromBody] AuditExceptionRequest request) {
            try {
                Logger.LogActivity("Creating new audit exception", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit exception record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.Findings)) {
                    if (await _auditExceptionService.ExistsAsync(e => e.AuditFinding == request.Findings && e.Id == request.ReportId)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another finding with same description found on the same report");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Exception findings cannot be empty", "The exception name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..create audit exception
                var result = await _auditExceptionService.InsertAsync(request, username);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit exception saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save audit exception record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/update-exception")]
        public async Task<IActionResult> UpdateAuditException([FromBody] AuditExceptionRequest request) {
            try {
                Logger.LogActivity("Update auidt exception", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit type record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _auditExceptionService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Audit exception record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..update document type
                var result = await _auditExceptionService.UpdateAsync(request, username, true);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit exception updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update audit exception record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/delete-exception")]
        public async Task<IActionResult> DeleteAuditException([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _auditExceptionService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Audi exception Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete audit exception
                var status = await _auditExceptionService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to regulatory audit exception", "An error occurred! could delete audit exception");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting audit exception by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Audit-Types

        [HttpPost("audits/type-retrieve")]
        public async Task<IActionResult> GetAuditType([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get audit type by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty","Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditTypeResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request audit type ID is required","Invalid audit type request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditTypeResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _auditTypeService.GetAsync(d => d.Id == request.RecordId, false);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Audit type not found", "No audit type matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditTypeResponse>(error));
                }

                //..return audit type
                var type = new AuditTypeResponse {
                    Id = result.Id,
                    TypeCode = result.TypeCode ?? string.Empty,
                    TypeName = result.TypeName ?? string.Empty,
                    Description = result.Description ?? string.Empty,
                    IsDeleted = result.IsDeleted
                };

                return Ok(new GrcResponse<AuditTypeResponse>(type));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuditTypeResponse>(error));
            }
        }

        [HttpPost("audits/paged-types-list")]
        public async Task<IActionResult> GetPagedAuditTypeList([FromBody] ListRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty","Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditTypeResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _auditTypeService.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No audit type records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditTypeResponse>>(new PagedResponse<AuditTypeResponse>(new List<AuditTypeResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<AuditTypeResponse> types = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(type => types.Add(new() {
                        Id = type.Id,
                        TypeCode = type.TypeCode ?? string.Empty,
                        TypeName = type.TypeName ?? string.Empty,
                        Description = type.Description ?? string.Empty,
                        IsDeleted = type.IsDeleted
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    types = types.Where(u =>
                        (u.TypeName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<AuditTypeResponse>>(new PagedResponse<AuditTypeResponse>(types, pageResult.Count,pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<AuditTypeResponse>>(error));
            }
        }

        [HttpPost("audits/create-type")]
        public async Task<IActionResult> CreateAuditType([FromBody] AuditTypeRequest request) {
            try {
                Logger.LogActivity("Creating new audit type", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST,"Request record cannot be empty", "The audit type record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.TypeName)) {
                    if (await _auditTypeService.ExistsAsync(t => t.TypeName == request.TypeName)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE,"Duplicate Record","Another audit type found with similar name");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty","The type name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..create audit type
                var result = await _auditTypeService.InsertAsync(request, username);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit type saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save audit type record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/update-type")]
        public async Task<IActionResult> UpdateAuditType([FromBody] AuditTypeRequest request) {
            try {
                Logger.LogActivity("Update auidt type", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit type record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _auditTypeService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Audit type record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..update document type
                var result = await _auditTypeService.UpdateAsync(request, username, true);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit type updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update audit type record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/delete-type")]
        public async Task<IActionResult> DeleteAuditType([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _auditTypeService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Audi type Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete audit type
                var status = await _auditTypeService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to regulatory audit type","An error occurred! could delete audit type");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting audit type by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Audit-Tasks

        [HttpPost("audits/audit-task-retrieve")]
        public async Task<IActionResult> GetAuditTask([FromBody] IdRequest request) {
            try {
                Logger.LogActivity("Get audit task by ID", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditTaskResponse>(error));
                }

                if (request.RecordId == 0) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request audit task ID is required", "Invalid audit type request ID");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditTaskResponse>(error));
                }
                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");

                var result = await _auditTaskService.GetAsync(d => d.Id == request.RecordId, false);
                if (result == null) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Audit task not found", "No audit task matched the provided ID");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<AuditTaskResponse>(error));
                }

                //..return audit type
                var status = new AuditTaskResponse {
                    Id = result.Id,
                    TaskName = result.TaskName ?? string.Empty,
                    TaskStatus = result.Status ?? string.Empty,
                    TaskDescription = result.Description ?? string.Empty,
                    Duedate = result.DueDate,
                    OwnerId = result.OwnerId,
                    Owner = result.ActionOwner.ContactPosition ?? string.Empty,
                    ExceptionId = result.ExceptionId,
                    Interval = result.Interval ?? string.Empty,
                    IntervalType = result.IntervalType ?? string.Empty,
                    SendReminder = result.SendReminder,
                    Message = result.Reminder ?? string.Empty,
                    IsDeleted = result.IsDeleted
                };

                return Ok(new GrcResponse<AuditTaskResponse>(status));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<AuditTaskResponse>(error));
            }
        }

        [HttpPost("audits/audit-tasks-list")]
        public async Task<IActionResult> GetPagedAuditTasksList([FromBody] AuditMiniTaskRequest request) {

            try {
                Logger.LogActivity($"{request.Action}", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "Invalid request body");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditTaskResponse>>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)} from IP Address {request.IPAddress}", "INFO");
                var pageResult = await _auditTaskService.PageAllAsync(request.PageIndex, request.PageSize, false);
                if (pageResult.Entities == null || !pageResult.Entities.Any()) {
                    var error = new ResponseError(ResponseCodes.SUCCESS, "No data", "No audit task records found");
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<PagedResponse<AuditTaskResponse>>(
                        new PagedResponse<AuditTaskResponse>(
                        new List<AuditTaskResponse>(), 0, pageResult.Page, pageResult.Size)));
                }

                List<AuditTaskResponse> tasks = new();
                var records = pageResult.Entities.ToList();
                if (records != null && records.Any()) {
                    records.ForEach(task => tasks.Add(new AuditTaskResponse() {
                        Id = task.Id,
                        TaskName = task.TaskName ?? string.Empty,
                        TaskStatus = task.Status ?? string.Empty,
                        TaskDescription = task.Description ?? string.Empty,
                        Duedate = task.DueDate,
                        OwnerId = task.OwnerId,
                        Owner = task.ActionOwner.ContactPosition ?? string.Empty,
                        ExceptionId = task.ExceptionId,
                        Interval = task.Interval ?? string.Empty,
                        IntervalType = task.IntervalType ?? string.Empty,
                        SendReminder = task.SendReminder,
                        Message = task.Reminder ?? string.Empty,
                        IsDeleted = task.IsDeleted
                    }));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                    var searchTerm = request.SearchTerm.ToLower();
                    tasks = tasks.Where(u =>
                        (u.TaskName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TaskStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.TaskDescription?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Owner?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(new GrcResponse<PagedResponse<AuditTaskResponse>>(new PagedResponse<AuditTaskResponse>(tasks, pageResult.Count, pageResult.Page, pageResult.Size)));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<PagedResponse<AuditTaskResponse>>(error));
            }
        }

        [HttpPost("audits/create-task")]
        public async Task<IActionResult> CreateAuditTask([FromBody] AuditTaskRequest request) {
            try {
                Logger.LogActivity("Creating new audit task", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit task record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!string.IsNullOrWhiteSpace(request.TaskName)) {
                    if (await _auditTaskService.ExistsAsync(t => t.TaskName == request.TaskName)) {
                        var error = new ResponseError(ResponseCodes.DUPLICATE, "Duplicate Record", "Another audit task found with similar name");
                        Logger.LogActivity($"DUPLICATE RECORD: {JsonSerializer.Serialize(error)}");
                        return Ok(new GrcResponse<GeneralResponse>(error));
                    }
                } else {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The task name is required");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..create audit type
                var result = await _auditTaskService.InsertAsync(request, username);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit task saved successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to save audit task record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/update-task")]
        public async Task<IActionResult> UpdateAuditTask([FromBody] AuditTaskRequest request) {
            try {
                Logger.LogActivity("Update auidt task", "INFO");
                if (request == null) {
                    var error = new ResponseError(ResponseCodes.BADREQUEST, "Request record cannot be empty", "The audit task record cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                Logger.LogActivity($"Request >> {JsonSerializer.Serialize(request)}", "INFO");
                if (!await _auditTaskService.ExistsAsync(r => r.Id == request.Id)) {
                    var error = new ResponseError(ResponseCodes.NOTFOUND, "Record Not Found", "Audit task record not found in the database");
                    Logger.LogActivity($"RECORD NOT FOUND: {JsonSerializer.Serialize(error)}");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }

                //..get username
                var currentUser = await _accessService.GetByIdAsync(request.UserId);
                string username;
                if (currentUser != null) {
                    username = currentUser.Username;
                } else {
                    username = $"{request.UserId}";
                }

                //..update document type
                var result = await _auditTaskService.UpdateAsync(request, username, true);
                var response = new GeneralResponse();
                if (result) {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.SUCCESS;
                    response.Message = "Audit task updated successfully";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                } else {
                    response.Status = true;
                    response.StatusCode = (int)ResponseCodes.FAILED;
                    response.Message = "Failed to update audit task record. An error occurrred";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                }

                return Ok(new GrcResponse<GeneralResponse>(response));
            } catch (Exception ex) {
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        [HttpPost("audits/delete-task")]
        public async Task<IActionResult> DeleteAuditTask([FromBody] IdRequest request) {
            try {
                Logger.LogActivity($"ACTION - {request.Action} on IP Address {request.IPAddress}", "INFO");

                //..check if record exists
                var response = new GeneralResponse();
                if (!await _auditTypeService.ExistsAsync(r => r.Id == request.RecordId)) {
                    response.Status = false;
                    response.StatusCode = (int)ResponseCodes.NOTFOUND;
                    response.Message = $"Audi task Not Found!";
                    Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
                    return Ok(new GrcResponse<GeneralResponse>(response));
                }

                //..delete audit type
                var status = await _auditTaskService.DeleteAsync(request);
                if (!status) {
                    var error = new ResponseError(ResponseCodes.FAILED, "Failed to regulatory audit task", "An error occurred! could delete audit task");
                    return Ok(new GrcResponse<GeneralResponse>(error));
                }
                return Ok(new GrcResponse<GeneralResponse>(new GeneralResponse() { Status = status }));
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting audit task by user {request.UserId}: {ex.Message}", "ERROR");
                var error = await HandleErrorAsync(ex);
                return Ok(new GrcResponse<GeneralResponse>(error));
            }
        }

        #endregion

        #region Protected methods

        protected static PolicyStatus GetStatusEnumValue(string status) {
            if (Enum.TryParse<PolicyStatus>(status, ignoreCase: true, out var enumVal)) {
                return enumVal;
            }

            return PolicyStatus.TOTALS;
        }

        protected static ReportPeriod GetPeriodEnumValue(string period) {
            if (Enum.TryParse<ReportPeriod>(period, ignoreCase: true, out var enumVal)) {
                return enumVal;
            }

            return ReportPeriod.NA;
        }
        
        #endregion

    }
}

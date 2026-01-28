using ClosedXML.Excel;
using Grc.ui.App.Defaults;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Controllers {
    public class RegisterController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly IStatutorySectionService _sectionService;
        private readonly IRegulatoryStatuteService _statuteService;
        private readonly IComplianceControlService _controlsService;
        private readonly IRegulatonCategoryService _regulatoryCategoryService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IResponsibilityService _responsibilityService;
        private readonly IRegulatonTypeService _regulatoryTypeService;
        private readonly IRegulatonAuthorityService _regulatoryAuthorityService;
        public RegisterController(IApplicationLoggerFactory loggerFactory,
                                  IEnvironmentProvider environment,
                                  IWebHelper webHelper,
                                  ILocalizationService localizationService,
                                  IErrorService errorService,
                                  IAuthenticationService authService,
                                  IStatutorySectionService regulatoryActService,
                                  IRegulatoryStatuteService statuteService,
                                  IComplianceControlService controlsService,
                                  IRegulatonCategoryService regulatoryService,
                                  IDocumentTypeService documentTypeService,
                                  IResponsibilityService responsibilityService,
                                  IRegulatonTypeService regulatoryTypeService,
                                  IRegulatonAuthorityService regulatoryAuthorityService,
                                  IGrcErrorFactory errorFactory,
                                  SessionManager sessionManager)
                                : base(loggerFactory, environment, webHelper,
                                      localizationService, errorService,
                                      errorFactory, sessionManager) {
            Logger.Channel = $"REGISTER-{DateTime.Now:yyyyMMddHHmmss}";
            _authService = authService;
            _sectionService = regulatoryActService;
            _statuteService = statuteService;
            _controlsService = controlsService;
            _regulatoryCategoryService = regulatoryService;
            _documentTypeService = documentTypeService;
            _responsibilityService = responsibilityService;
            _regulatoryTypeService = regulatoryTypeService;
            _regulatoryAuthorityService = regulatoryAuthorityService;
        }

        #region Law Registers
        public async Task<IActionResult> GetLawList() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Acts and legal Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "REGISTER-LAWS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        [LogActivityResult("Retrieve Law/Regulation", "User retrieved law/regulationt", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_ACTS, "RegulatoryLaw")]
        public async Task<IActionResult> GetLaw(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Law/Regul;ation Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.LAW_RETRIVE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _statuteService.GetStatuteAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving law/regulation";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var lawRecord = new {
                    id = response.Id,
                    categoryId = response.CategoryId,
                    lawCode = response.StatutoryLawCode ?? string.Empty,
                    lawName = response.StatutoryLawName ?? string.Empty,
                    typeId = response.StatutoryTypeId,
                    authorityId = response.AuthorityId,
                    isActive = response.IsDeleted
                };

                return Ok(new { success = true, data = lawRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving regulatory: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        [HttpPost]
        [LogActivityResult("Add Law/Regulation", "User added law/regulation", ActivityTypeDefaults.COMPLIANCE_CREATE_ACT, "RegulatoryLaw")]
        public async Task<IActionResult> CreateLaw([FromBody] StatuteViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _statuteService.CreateStatuteAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create statutory section" });

                var created = result.Data;
                return Ok(new { success = true, message = "Category created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating regulatory act: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        [HttpPost]
        [LogActivityResult("Update Law/Regulation", "User updated law/regulation", ActivityTypeDefaults.COMPLIANCE_EDITED_ACT, "RegulatoryLaw")]
        public async Task<IActionResult> UpdateLaw([FromBody] StatuteViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _statuteService.UpdateStatuteAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update Statute section" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Statute section created successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating task: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, data = new { } });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Law/Regulation", "User delete law/regulation", ActivityTypeDefaults.COMPLIANCE_DELETED_ACT, "RegulatoryLaw")]
        public async Task<IActionResult> DeleteLaw(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Task Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_ACTS.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _statuteService.DeleteStatuteAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete Statute section" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting Statute section: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetRegulatoryLaws([FromBody] StatueListRequest request) {

            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                }

                var currentUser = userResponse.Data;
                var result = await _statuteService.GetCategoryStatutes(request, currentUser.UserId, ipAddress);
                PagedResponse<GrcStatutoryLawResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to retrieve category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"CATEGORY STATUTES DATA - {list.TotalCount}");
                }

                var statutes = list.Entities ?? new();
                if (statutes.Any()) {
                    var laws = statutes.Select(l => new {
                        id = l.Id,
                        lawName = l.StatutoryLawName,
                        lawCode = l.StatutoryLawCode,
                        status = l.IsDeleted ? "Inactive" : "Active"
                    }).ToList();
                    return Ok(new { last_page = 1, total_records = laws.Count, data = laws });
                }

                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory acts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetPagedRegulatoryLaws([FromBody] StatueListRequest request) {
            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                }

                var currentUser = userResponse.Data;
                var result = await _sectionService.GetLawSectionsAsync(request, currentUser.UserId, ipAddress);
                PagedResponse<GrcStatutorySectionResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"SECTION DATA ERROR: Failed to retrieve category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"SECTION STATUTES DATA - {list.TotalCount}");
                }

                var statutes = list.Entities ?? new();
                if (statutes.Any()) {
                    var laws = statutes.Select(l => new {
                        id = l.Id,
                        sectionNumber = l.Section,
                        title = l.Summery,
                        assurance = l.ComplianceAssurance,
                        isMandatory = l.IsMandatory,
                    }).ToList();
                    return Ok(new { last_page = 1, total_records = laws.Count, data = laws });
                }

                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory acts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-ACTS", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Acts Registers

        public async Task<IActionResult> RegulationList() {
            try {
                if (User.Identity?.IsAuthenticated == true)  {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        Statistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Acts and legal Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "REGISTER-ACTS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        [LogActivityResult("Retrieve Act", "User retrieved legal act", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_ACTS, "RegulatoryAct")]
        public async Task<IActionResult> GetRegulatoryAct(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Legal Act Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.SECTION_RETRIVE_SECTION.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _sectionService.GetSectionAsyncAsync(request);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving legacl Act";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var actRecord = new {
                    id = response.Id,
                    statutoryId = response.StatutoryId,
                    section = response.Section ?? string.Empty,
                    summery = response.Summery ?? string.Empty,
                    obligation = response.Obligation ?? string.Empty,
                    frequencyId = response.FrequencyId,
                    ownerId = response.OwnerId,
                    isMandatory = response.IsMandatory,
                    exclude = response.ExcludeFromCompliance,
                    coverage = response.Coverage,
                    isCovered = response.IsCovered,
                    isActive = response.IsDeleted,
                    comments = response.Comments ?? string.Empty,
                    assurance = response.ComplianceAssurance
                };

                return Ok(new { success = true, data = actRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error retrieving regulatory act: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message="Failed to retrieve regulatory acts, an Unexpected error occurred", data = new { }});
            }
        }

        [HttpPost]
        [LogActivityResult("Add Act", "User added legal act", ActivityTypeDefaults.COMPLIANCE_CREATE_ACT, "RegulatoryAct")]
        public async Task<IActionResult> CreateRegulatoryAct([FromBody] StatuteSectionViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {success = false,message = msg,data = new { }});
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _sectionService.CreateSectionAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create statutory section" });

                var created = result.Data;
                return Ok(new{success = true,message = "Regulatory Act created successfully",data = new { }});

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating regulatory act: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message="Failed to create regulatory act, an Unexpected error occurred", data = new { } });
            }
        }

        [HttpPost]
        [LogActivityResult("Update Act", "User updated legal act", ActivityTypeDefaults.COMPLIANCE_EDITED_ACT, "RegulatoryAct")]
        public async Task<IActionResult> UpdateRegulatoryAct([FromBody] StatuteSectionViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _sectionService.UpdateSectionAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update Statute section" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Statute section created successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating status section: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, data = new { } });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Act", "User delete legal act", ActivityTypeDefaults.COMPLIANCE_DELETED_ACT, "RegulatoryAct")]
        public async Task<IActionResult> DeleteRegulatory(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Section Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_ACTS.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _sectionService.DeleteSectionAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete Statute section" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting Statute section: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetRegulatoryActs([FromBody] StatueListRequest request) {
            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                }

                var currentUser = userResponse.Data;
                var result = await _sectionService.GetLawSectionsAsync(request, currentUser.UserId, ipAddress);
                PagedResponse<GrcStatutorySectionResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"SECTION DATA ERROR: Failed to retrieve category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"SECTION STATUTES DATA - {list.TotalCount}");
                }

                var statutes = list.Entities ?? new();
                if (statutes.Any()) {
                    var laws = statutes.Select(l => new {
                        id = l.Id,
                        sectionNumber = l.Section,
                        title = l.Summery,
                        coverage = l.Coverage,
                        isCovered = l.IsCovered,
                        assurance = l.ComplianceAssurance,
                        isMandatory = l.IsMandatory,
                    }).ToList();
                    return Ok(new { last_page = 1, total_records = laws.Count, data = laws });
                }

                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory acts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-ACTS", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Obligations

        public async Task<IActionResult> RegulationObligations() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Acts obligations view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "REGISTER-OBLIGATION", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> GetObligationList([FromBody] TableListRequest request) {

            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = userResponse.Data;
                if (currentUser == null) {
                    Logger.LogActivity($"User record id null - {JsonSerializer.Serialize(userResponse)}");
                    //..session has expired
                    return Ok(new { last_page = 0, data = new List<object>() });
                }
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.PageIndex = request.PageIndex;
                request.PageSize = request.PageSize;

                var result = await _statuteService.GetStatutoryObligations(request);
                PagedResponse<GrcObligationResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"OBLIGATIONS DATA ERROR: Failed to retrieve category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"OBLIGATIONS RECORD COUNT - {list.TotalCount}");
                }

                var categoryData = list.Entities ?? new();
                if (categoryData.Any()) {
                    var categories = categoryData.Select(category => new {
                        categoryName = category.CategoryName,
                        coverage = category.Coverage,
                        isCovered = category.IsCovered,
                        assurance = category.Assurance,
                        issues = category.Issues,
                        laws = category.Laws.Select(law => new {
                            lawName = law.LawName,
                            coverage = law.Coverage,
                            isCovered = law.IsCovered,
                            assurance = law.Assurance,
                            issues = law.Issues,
                            sections = law.Sections.Select(section => new {
                                sectionId = section.Id,
                                section = section.Section,
                                requirement = section.Requirement,
                                coverage = section.Coverage,
                                isCovered = section.IsCovered,
                                assurance = section.Assurance,
                                issues = section.Issues
                            })
                        }),
                    }).ToList();

                    return Ok(new {
                        last_page = list.TotalPages,
                        total_records = list.TotalCount,
                        data = categories
                    });
                }
                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory obligations: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-OBLIGATIONS", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [LogActivityResult("Retrieve Act", "User retrieved legal act", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_ACTS, "RegulatoryAct")]
        public async Task<IActionResult> GetObligation(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Legal Act Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.SECTION_RETRIVE_SECTION.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _sectionService.GetObligationAsyncAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving legacl Act";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var obligation = new {
                    id = response.Id,
                    category = response.Category ?? string.Empty,
                    statute = response.Statute ?? string.Empty,
                    section = response.Section ?? string.Empty,
                    summery = response.Summery ?? string.Empty,
                    obligation = response.Obligation ?? string.Empty,
                    isMandatory = response.IsMandatory,
                    exclude = response.Exclude,
                    coverage = response.Coverage,
                    isCovered = response.IsCovered,
                    rationale = response.ComplianceReason ?? string.Empty,
                    assurance = response.Assurance,
                    complianceMaps = response.ComplianceMaps,
                    issues = response.Issues,
                    revisions = response.Revisions,
                };

                return Ok(new { success = true, data = obligation });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving regulatory act: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        public async Task<IActionResult> CreateComplianceMap([FromBody] ObligationMapViewModel request) {
            try {
                Logger.LogActivity("Create Obligation Compliance Map", "INFO");
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _sectionService.CreateMapAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create compliance map, an error occurred" });

                var created = result.Data;
                if (!created.Status)
                    return Ok(new { success = created.Status, message = created.Message ?? "Failed to create compliance map, an error occurred", data = new { } });

                return Ok(new { success = false, message = "Compliance map saved successfully"});
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error occurred while saving compliance map: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message="Could not save map, an unexpected error occurred"});
            }
        }

        [HttpPost()]
        public async Task<IActionResult> GetObligationSummary() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Action = Activity.OBLIGATION_REPORT_DATA.GetDescription()
            };

            var result = await _statuteService.GetObligationReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve obligations data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Obligations Summary");
            var kpiSheet = workbook.Worksheets.Add("Summary KPIs");

            var data = result.Data;

            //..grouped headers
            ws.Cell(1, 1).Value = "NO";
            ws.Cell(1, 2).Value = "STATUTE";
            ws.Cell(1, 3).Value = "OBLIGATIONS";

            ws.Range(1, 4, 1, 5).Merge().Value = "COMPLIANT";
            ws.Range(1, 6, 1, 7).Merge().Value = "NON-COMPLIANT";

            ws.Cell(1, 8).Value = "COMPLIANCE %";

            //..sub headers
            ws.Cell(2, 4).Value = "COUNT";
            ws.Cell(2, 5).Value = "%";
            ws.Cell(2, 6).Value = "COUNT";
            ws.Cell(2, 7).Value = "%";

            //..header styling
            var headerRange = ws.Range(1, 1, 2, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Row(1).Height = 28;
            ws.Row(2).Height = 22;

            //..freez header
            ws.SheetView.FreezeRows(2);

            int row = 3;
            int no = 1;

            foreach (var s in data) {
                ws.Cell(row, 1).Value = no;
                ws.Cell(row, 2).Value = s.Statute;
                ws.Cell(row, 3).Value = s.Obligations;

                ws.Cell(row, 4).Value = s.CompliantCount;
                ws.Cell(row, 5).Value = s.CompliantPercentage / 100;

                ws.Cell(row, 6).Value = s.NonCompliantCount;
                ws.Cell(row, 7).Value = s.NonCompliantPercentage / 100;

                ws.Cell(row, 8).Value = s.CompliantPercentage / 100;

                row++;
                no++;
            }

            int lastDataRow = row - 1;
            // Percentage formats
            ws.Range(3, 5, lastDataRow, 5).Style.NumberFormat.Format = "0%";
            ws.Range(3, 7, lastDataRow, 7).Style.NumberFormat.Format = "0%";
            ws.Range(3, 8, lastDataRow, 8).Style.NumberFormat.Format = "0%";

            //..column shading
            ws.Range(3, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
            ws.Range(3, 2, lastDataRow, 2).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
            ws.Range(3, 3, lastDataRow, 3).Style.Fill.BackgroundColor = XLColor.AliceBlue;
            ws.Range(3, 4, lastDataRow, 5).Style.Fill.BackgroundColor = XLColor.Honeydew;
            ws.Range(3, 6, lastDataRow, 7).Style.Fill.BackgroundColor = XLColor.MistyRose;

            //..zebra striping
            for (int r = 3; r <= lastDataRow; r++) {
                if (r % 2 == 0)
                    ws.Range(r, 1, r, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#F9FAFB");
            }

            var complianceRange = ws.Range(3, 8, lastDataRow, 8);
            complianceRange.AddConditionalFormat().WhenBetween(0.9, 1).Fill.SetBackgroundColor(XLColor.Green);
            complianceRange.AddConditionalFormat().WhenBetween(0.75, 0.8999).Fill.SetBackgroundColor(XLColor.Orange);
            complianceRange.AddConditionalFormat().WhenLessThan(0.75).Fill.SetBackgroundColor(XLColor.Red);
            complianceRange.Style.Font.Bold = true;

            //..boarders and resize
            ws.Range(3, 1, lastDataRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(3, 3, lastDataRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(1, 1, lastDataRow, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Widths
            ws.Column(1).Width = 6;
            ws.Column(2).Width = 35;
            ws.Column(3).Width = 14;
            for (int c = 4; c <= 8; c++) ws.Column(c).Width = 12;

            int totalRow = lastDataRow + 1;

            ws.Cell(totalRow, 1).Value = "TOTAL";
            ws.Range(totalRow, 1, totalRow, 2).Merge();

            ws.Cell(totalRow, 3).FormulaA1 = $"=SUM(C3:C{lastDataRow})";
            ws.Cell(totalRow, 4).FormulaA1 = $"=SUM(D3:D{lastDataRow})";
            ws.Cell(totalRow, 6).FormulaA1 = $"=SUM(F3:F{lastDataRow})";

            ws.Cell(totalRow, 5).FormulaA1 = $"=IF(C{totalRow}=0,0,D{totalRow}/C{totalRow})";
            ws.Cell(totalRow, 7).FormulaA1 = $"=IF(C{totalRow}=0,0,F{totalRow}/C{totalRow})";
            ws.Cell(totalRow, 8).FormulaA1 = $"=IF(C{totalRow}=0,0,D{totalRow}/C{totalRow})";

            var totalRange = ws.Range(totalRow, 1, totalRow, 8);
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#D1D5DB");
            totalRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
            totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            totalRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Row(totalRow).Height = 26;

            ws.Range(3, 5, totalRow, 5).Style.NumberFormat.Format = "0%";
            ws.Range(3, 7, totalRow, 7).Style.NumberFormat.Format = "0%";
            ws.Range(3, 9, totalRow, 9).Style.NumberFormat.Format = "0%";
            ws.Range(3, 10, totalRow, 10).Style.NumberFormat.Format = "0%";

            //..add KPI Sheet
            kpiSheet.Cell(2, 2).Value = "Total Obligations";
            kpiSheet.Cell(2, 3).FormulaA1 = $"='Obligations Summary'!C{totalRow}";

            kpiSheet.Cell(3, 2).Value = "Compliant";
            kpiSheet.Cell(3, 3).FormulaA1 = $"='Obligations Summary'!D{totalRow}";

            kpiSheet.Cell(4, 2).Value = "Non-Compliant";
            kpiSheet.Cell(4, 3).FormulaA1 = $"='Obligations Summary'!F{totalRow}";

            kpiSheet.Cell(5, 2).Value = "Compliance Rate";
            kpiSheet.Cell(5, 3).FormulaA1 = $"='Obligations Summary'!H{totalRow}";
            kpiSheet.Cell(5, 3).Style.NumberFormat.Format = "0%";

            var kpiRange = kpiSheet.Range(2, 2, 5, 3);
            kpiRange.Style.Font.Bold = true;
            kpiRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            kpiRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            kpiSheet.Column(2).Width = 30;
            kpiSheet.Column(3).Width = 16;
            kpiSheet.Range(2, 3, 5, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Center totals row
            ws.Range(totalRow, 1, totalRow, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(totalRow, 1, totalRow, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range(totalRow, 1, totalRow, 10).Style.Font.FontSize = 12;

            kpiSheet.Range(5, 3, 5, 3).AddConditionalFormat().WhenBetween(0.9, 1).Fill.SetBackgroundColor(XLColor.Green);
            kpiSheet.Range(5, 3, 5, 3).AddConditionalFormat().WhenLessThan(0.9).Fill.SetBackgroundColor(XLColor.Orange);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"CIRCULAR-SUMMERY-{DateTime.Today:MM-yyyy}.xlsx");
        }

        #endregion

        #region Compliance Issues

        [LogActivityResult("Retrieve Compliance Issue", "User retrieved compliance issue", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_ISSUE, "ComplianceIssue")]
        public async Task<IActionResult> GetIssue(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Issue Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_ISSUE_RETRIEVE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _controlsService.GetIssueAsyncAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving compliance issue";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var actRecord = new {
                    id = response.Id,
                    articleId = response.ArticleId,
                    description = response.Description ?? string.Empty,
                    isClosed = response.IsClosed,
                    isActive = response.IsDeleted,
                    comments = response.Comments ?? string.Empty
                };

                return Ok(new { success = true, data = actRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving regulatory act: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to retrieve regulatory acts, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Create Compliance Issue", "User added new compliance issue", ActivityTypeDefaults.COMPLIANCE_CREATE_ISSUE, "ComplianceIssue")]
        public async Task<IActionResult> CreateIssue([FromBody] IssueViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _controlsService.CreateIssueAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create issue" });

                var created = result.Data;
                if (!created.Status)
                    return Ok(new { success = created.Status, message = created.Message ?? "Failed to update issue", data = new { } });

                return Ok(new { success = true, message = "Issue created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating issue: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to create issue, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Update Compliance issue", "User updated compliance issue", ActivityTypeDefaults.COMPLIANCE_EDITED_ISSUE, "ComplianceIssue")]
        public async Task<IActionResult> UpdateIssue([FromBody] IssueViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _controlsService.UpdateIssueAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update issue" });

                var updated = result.Data;
                if (!updated.Status)
                    return Ok(new { success = updated.Status, message = updated.Message ?? "Failed to update issue", data = new { } });

                return Ok(new { success = true, message = "Issue created successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating issue: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, data = new { } });
            }
        }

        [LogActivityResult("Delete Compliance Issue", "User deleted control item", ActivityTypeDefaults.COMPLIANCE_DELETED_ISSUE, "ControlItem")]
        public async Task<IActionResult> DeleteIssue(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Issue Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_ISSUE_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _controlsService.DeleteIssueAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete issue" });
                
                var resultData = result.Data;
                if (!resultData.Status)
                    return Ok(new { success = resultData.Status, message = resultData.Message ?? "Failed to delete issue", data = new { } });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting issue: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        public async Task<IActionResult> ComplianceMapping([FromBody] ComplianceMapViewModel model) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (model == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _controlsService.CreatMappAsync(model, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to complete mapping" });

                var created = result.Data;
                if(!created.Status)
                    return Ok(new { success = created.Status, message = created.Message ?? "Failed to complete mapping", data = new { } });

                return Ok(new { success = true, message = "Compliance mapping completed successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating issue: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to create issue, an Unexpected error occurred", data = new { } });
            }
        }

        #endregion

        #region Compliance Controls

        public async Task<IActionResult> ComplianceControl() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Acts obligations view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "REGISTER-OBLIGATION", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> GetCategoryControlList([FromBody] TableListRequest request) {
            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = userResponse.Data;
                if (currentUser == null) {
                    Logger.LogActivity($"User record id null - {JsonSerializer.Serialize(userResponse)}");
                    //..session has expired
                    return Ok(new { last_page = 0, data = new List<object>() });
                }
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.PageIndex = request.PageIndex;
                request.PageSize = request.PageSize;

                var result = await _controlsService.GetControlCategoriesAsync(request);
                PagedResponse<GrcControlCategoryResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"CONTROL CATEGORY DATA ERROR: Failed to control category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"CONTROL CATEGORY RECORD COUNT - {list.TotalCount}");
                }

                var categoryData = list.Entities ?? new();
                if (categoryData.Any()) {
                    var categories = categoryData.Select(category => new {
                        categoryId = category.Id,
                        categoryName = category.CategoryName,
                        isExcluded = category.Exclude,
                        isDeleted = category.IsDeleted,
                        notes = category.Comments ?? string.Empty,
                        controls = category.ControlItems.Select(control => new {
                            itemId = control.Id,
                            controlName = control.ItemName,
                            isExcluded = control.Exclude,
                            isDeleted = control.IsDeleted,
                            notes = control.Comments ?? string.Empty,
                        }),
                    }).ToList();

                    return Ok(new { last_page = list.TotalPages, total_records = list.TotalCount, data = categories });
                }

                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving control category: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLS", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [LogActivityResult("Retrieve Compliance control", "User retrieved compliance control", ActivityTypeDefaults.COMPLIANCE_CONTROLCATEGORY_RETRIVE, "ControlCategory")]
        public async Task<IActionResult> GetControlCategory(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Control category Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_CATEGORY_RETRIVE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _controlsService.GetCategoryAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving control category";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var categoryRecord = new {
                    categoryId = response.Id,
                    categoryName = response.CategoryName ?? string.Empty,
                    isCategoryExcluded = response.Exclude,
                    isCategoryDeleted = response.IsDeleted,
                    categoryComments = response.Comments ?? string.Empty,
                    items = response.ControlItems.Select(item => new {
                        itemId = item.Id,
                        itemName = item.ItemName ?? string.Empty,
                        isItemExcluded = item.Exclude,
                        isItemDeleted = item.IsDeleted,
                        itemComments = item.Comments ?? string.Empty
                    })
                };

                return Ok(new { success = true, data = categoryRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving control categopry: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-CONTROL", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to retrieve control categopry, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Add Compliance control", "User added new compliance control", ActivityTypeDefaults.COMPLIANCE_CONTROLCATEGORY_CREATE, "ControlCategory")]
        public async Task<IActionResult> CreateControlCategory([FromBody] ControlCategoryViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _controlsService.CreateControlCategoryAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create control category" });

                var created = result.Data;
                return Ok(new { success = true, message = "Category created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating category: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to create category, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Update Compliance control", "User updated compliance control", ActivityTypeDefaults.COMPLIANCE_CONTROLCATEGORY_UPDATE, "ControlCategory")]
        public async Task<IActionResult> UpdateControlCategory([FromBody] ControlCategoryViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _controlsService.UpdateCategoryAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update control category" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Control category created successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating control category: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, data = new { } });
            }
        }

        [LogActivityResult("Retrieve control item", "User retrieved control item", ActivityTypeDefaults.COMPLIANCE_CONTROLCATEGORY_RETRIVE, "ControlItem")]
        public async Task<IActionResult> GetControlItem(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Legal Act Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_CONTROL_RETRIVE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _controlsService.GetItemAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving legacl Act";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var actRecord = new {
                    itemId = response.Id,
                    categoryId = response.Id,
                    parentId = response.CategoryId,
                    itemName = response.ItemName ?? string.Empty,
                    isItemExcluded = response.Exclude,
                    isItemDeleted = response.IsDeleted,
                    itemComments = response.Comments ?? string.Empty
                };

                return Ok(new { success = true, data = actRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving regulatory act: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to retrieve regulatory acts, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Add control item", "User retrieved added new control item", ActivityTypeDefaults.COMPLIANCE_CONTROLITEM_CREATE, "ControlItem")]
        public async Task<IActionResult> CreateControlItem([FromBody] ItemViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _controlsService.CreateItemAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create control item" });

                var created = result.Data;
                return Ok(new { success = true, message = "Item created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating item: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to create item, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Update control item", "User modified control item", ActivityTypeDefaults.COMPLIANCE_CONTROLITEM_UPDATE, "ControlItem")]
        public async Task<IActionResult> UpdateControlItem([FromBody] ItemViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _controlsService.UpdateItemAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update control item" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Control item created successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating item: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, data = new { } });
            }
        }

        [LogActivityResult("Delete control item", "User deleted control item", ActivityTypeDefaults.COMPLIANCE_CONTROLITEM_DELETE, "ControlItem")]
        public async Task<IActionResult> DeleteControlItem(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Item Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_ITEM.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _controlsService.DeleteItemAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete Item" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting item: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-ITEM", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        #endregion

        #region support - Regulatory Categories

        public async Task<IActionResult> ComplianceRegulatoryCategories() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        TempData["UiResponse"] = JsonSerializer.Serialize(
                            UiResponse.Fail("Your session expired. Please login again.")
                        );
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Compliance Regulatory Categories view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                TempData["UiResponse"] = JsonSerializer.Serialize(
                           UiResponse.Fail("Error loading Compliance Regulatory types view")
                       );
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }

        [LogActivityResult("Retrieve Category", "User retrieved regulatory category", ActivityTypeDefaults.COMPLIACE_RETRIVE_CATEGORY, "Category")]
        public async Task<IActionResult> GetRegulatoryCategory(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Category Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequest getRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_CATEGORY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryCategoryService.GetCategoryAsync(getRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while deleting category";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        message = errMsg,
                        data = new { }
                    });
                }

                GrcRegulatoryCategoryResponse response = result.Data;
                var categoryRecord = new {
                    id = response.Id,
                    categoryName = response.CategoryName,
                    comments = response.Comments,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    addedon = response.CreatedOn.ToString("dd-MM-yyyy"),
                };

                return Ok(new {
                    success = true,
                    message = "Category created successfully",
                    data = categoryRecord
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting category: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add Category", "User added regulatory category", ActivityTypeDefaults.COMPLIACE_CREATE_CATEGORY, "Category")]
        public async Task<IActionResult> CreateRegulatoryCategory([FromBody] RegulatoryCategoryRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.CREATEREGULATORYCATEGORY.GetDescription();

                var result = await _regulatoryCategoryService.CreateCategoryAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to create category";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var created = result.Data;
                return Ok(new { success = true, message = "Category created successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating category: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Update Category", "User modified regulatory category", ActivityTypeDefaults.COMPLIANCE_EDITED_CATEGORY, "Category")]
        public async Task<IActionResult> UpdateRegulatoryCategory([FromBody] RegulatoryCategoryRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_CATEGORY.GetDescription();

                var result = await _regulatoryCategoryService.UpdateCategoryAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to update category";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Category updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating category: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Delete Category", "User deleted regulatory category", ActivityTypeDefaults.COMPLIANCE_DELETED_CATEGORY, "Category")]
        public async Task<IActionResult> DeleteRegulatoryCategory(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Category Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequest deleteRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_CATEGORY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryCategoryService.DeleteCategoryAsync(deleteRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while deleting category";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        message = errMsg,
                        data = new { }
                    });
                }

                ServiceResponse response = result.Data;
                return Ok(new {
                    message = response.Message,
                    success = response.Status,
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting category: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [LogActivityResult("Export Categories", "User exported regulatory categories to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_CATEGORY, "Category")]
        public IActionResult ExportToExcel([FromBody] List<GrcRegulatoryCategoryResponse> data) {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Regulation Categories");

            //..headers
            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 2).Value = "Category";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var item in data) {
                worksheet.Cell(row, 1).Value = item.Id;
                worksheet.Cell(row, 2).Value = item.CategoryName;
                worksheet.Cell(row, 3).Value = item.IsDeleted ? "Inactive" : "Active";
                worksheet.Cell(row, 4).Value = item.CreatedOn.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryCategories.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export Categories", "User exported regulatory categories to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_CATEGORY, "Category")]
        public async Task<IActionResult> ExportAllCategories() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null) {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_CATEGORY.GetDescription()
            };

            var categoryData = await _regulatoryCategoryService.GetPagedCategoriesAsync(request);
            if (categoryData.HasError || categoryData.Data == null) {
                var errMsg = categoryData.Error?.Message ?? "Failed to retrieve categories";
                Logger.LogActivity(errMsg);
                return Ok(new {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Categories");

            //..headers
            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Category";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var cat in categoryData.Data.Entities) {
                ws.Cell(row, 1).Value = cat.Id;
                ws.Cell(row, 2).Value = cat.CategoryName;
                ws.Cell(row, 3).Value = cat.IsDeleted ? "Inactive" : "Active";
                ws.Cell(row, 4).Value = cat.CreatedOn.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryCategories.xlsx"
            );
        }

        public async Task<IActionResult> GetRegulatoryCategories() {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"REGULATORY CATEGORY LIST ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new() {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIEVEREGULATORYCATEGORIES.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all regulatory categories
                var categoryData = await _regulatoryCategoryService.GetRegulatoryCategories(request);

                List<GrcRegulatoryCategoryResponse> categories;
                if (categoryData.HasError) {
                    categories = new();
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to retrieve category items - {JsonSerializer.Serialize(categoryData)}");
                } else {
                    categories = categoryData.Data;
                    Logger.LogActivity($"REGULATORY CATEGORY DATA - {JsonSerializer.Serialize(categories)}");
                }

                //..transform data for Tabulator
                var tabulatorData = categories.Select(cat => new {
                    id = cat.Id,
                    startTab = "",
                    category = cat.CategoryName,
                    comments = cat.Comments,
                    status = cat.IsDeleted,
                    addedon = cat.CreatedOn.ToString("dd-MM-yyyy"),
                    endTab = ""
                }).ToList();

                return Json(new { success = true, data = tabulatorData });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory categories: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-CONTROLLER", ex.StackTrace);
                return Json(new { success = false, data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPagedCategories([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_AUTHORITY.GetDescription();

                //..map to ajax object
                var categoryData = await _regulatoryCategoryService.GetPagedCategoriesAsync(request);
                PagedResponse<GrcRegulatoryCategoryResponse> categoryList = new();

                if (categoryData.HasError) {
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to retrieve Regulation categories - {JsonSerializer.Serialize(categoryData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    categoryList = categoryData.Data;
                    Logger.LogActivity($"REGULATORY CATEGORY DATA - {JsonSerializer.Serialize(categoryList)}");
                }

                var pagedEntities = categoryList.Entities
                    .Select(category => new {
                        id = category.Id,
                        category = category.CategoryName,
                        comments = category.Comments,
                        status = category.IsDeleted ? "Pending" : "Active",
                        addedon = category.CreatedOn.ToString("dd-MM-yyyy"),
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)categoryList.TotalCount / categoryList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = categoryList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory categories: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-SETTINGS-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region support - Regulatory Types

        public async Task<IActionResult> ComplianceRegulatoryTypes() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        TempData["UiResponse"] = JsonSerializer.Serialize(
                            UiResponse.Fail("Your session expired. Please login again.")
                        );
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }

            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Compliance Regulatory types view: {ex.Message}", "ERROR");
                TempData["UiResponse"] = JsonSerializer.Serialize(
                            UiResponse.Fail("Error loading Compliance Regulatory types view")
                        );
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }

        [LogActivityResult("Retrieve Type", "User retrieved regulatory type", ActivityTypeDefaults.COMPLIACE_RETRIVE_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> GetRegulatoryType(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Type Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequest getRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_TYPE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryTypeService.GetTypeAsync(getRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while retrieving type";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        succuss = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                GrcRegulatoryTypeResponse response = result.Data;
                var type = new {
                    id = response.Id,
                    typeName = response.TypeName,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    addedBy = response.CreatedBy ?? string.Empty,
                    addedon = response.CreatedOn.ToString("dd-MM-yyyy"),
                };

                return Ok(new {success = true, data = type });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add Type", "User added regulatory type", ActivityTypeDefaults.COMPLIACE_CREATE_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> CreateRegulatoryType([FromBody] RegulatoryViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _regulatoryTypeService.CreateTypeAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to create type";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var created = result.Data;
                return Ok(new {
                    success = true,
                    message = "Type created successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Update type", "User modified regulatory type", ActivityTypeDefaults.COMPLIANCE_EDITED_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> UpdateRegulatoryType([FromBody] RegulatoryViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _regulatoryTypeService.UpdateTypeAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to update type";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Type updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Delete type", "User deleted regulatory type", ActivityTypeDefaults.COMPLIANCE_DELETED_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> DeleteRegulatoryType(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Type Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequest deleteRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_TYPE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryTypeService.DeleteTypeAsync(deleteRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to delete regulatory type";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg ?? "" });
                }

                ServiceResponse response = result.Data;
                return Ok(new { success = true, message = response.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [LogActivityResult("Export types", "User exported regulatory types to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_TYPE, "Regulatory_Type")]
        public IActionResult ExcelExportTypes([FromBody] List<GrcRegulatoryTypeResponse> data) {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Regulation Types");

            //..headers
            worksheet.Cell(1, 2).Value = "Type";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var item in data) {
                worksheet.Cell(row, 2).Value = item.TypeName;
                worksheet.Cell(row, 3).Value = item.IsDeleted ? "Inactive" : "Active";
                worksheet.Cell(row, 4).Value = item.CreatedOn.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryTypes.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export types", "User exported regulatory types to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> ExcelExportAllTypes() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null) {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_TYPES.GetDescription()
            };

            GrcResponse<PagedResponse<GrcRegulatoryTypeResponse>> typeData = await _regulatoryTypeService.GetPagedTypesAsync(request);
            if (typeData.HasError || typeData.Data == null) {
                var errMsg = typeData.Error?.Message ?? "Failed to retrieve types";
                Logger.LogActivity(errMsg);
                return Ok(new {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Types");

            //..headers
            ws.Cell(1, 2).Value = "Type";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var cat in typeData.Data.Entities) {
                ws.Cell(row, 2).Value = cat.TypeName;
                ws.Cell(row, 3).Value = cat.IsDeleted ? "Inactive" : "Active";
                ws.Cell(row, 4).Value = cat.CreatedOn.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryTypes.xlsx"
            );
        }

        public async Task<IActionResult> AllRegulatoryTypes([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"REGULATORY TYPES DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIEVEREGULATORYTYPES.GetDescription();

                //..get regulatory type data
                var typeData = await _regulatoryTypeService.GetPagedTypesAsync(request);
                PagedResponse<GrcRegulatoryTypeResponse> typeList = new();
                if (typeData.HasError) {
                    Logger.LogActivity($"REGULATORY TYPE DATA ERROR: Failed to retrieve type items - {JsonSerializer.Serialize(typeData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    typeList = typeData.Data;
                    Logger.LogActivity($"REGULATORY TYPE DATA - {JsonSerializer.Serialize(typeList)}");
                }

                typeList.Entities ??= new();
                var pagedEntities = typeList.Entities
                    .Select(t => new {
                        id = t.Id,
                        typeName = t.TypeName,
                        status = t.IsDeleted ? "Inactive" : "Active",
                        addedBy = t.CreatedBy ?? string.Empty,
                        addedon = t.CreatedOn.ToString("dd-MM-yyyy"),
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)typeList.TotalCount / typeList.Size);
                return Ok(new { last_page = totalPages, total_records = typeList.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory type items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-TYPE-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region support - Authorities

        public async Task<IActionResult> ComplianceAuthorities() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Compliance Regulatory Authorities view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }

        [LogActivityResult("Retrieve Authority", "User retrieved regulatory authority", ActivityTypeDefaults.COMPLIACE_RETRIVE_AUTHORITY, "Authorities")]
        public async Task<IActionResult> GetRegulatoryAuthority(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Authority Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequest getRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_AUTHORITY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryAuthorityService.GetAuthorityAsync(getRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while retrieving authority";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        succuss = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var response = result.Data;
                var authorityRecord = new {
                    id = response.Id,
                    authorityName = response.AuthorityName,
                    authorityAlias = response.AuthorityAlias,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    addedon = response.CreatedOn.ToString("dd-MM-yyyy"),
                    addedBy = response.CreatedBy ?? string.Empty,
                };

                return Ok(new {
                    success = true,
                    message = "",
                    data = authorityRecord
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        public async Task<IActionResult> AllRegulatoryAuthorities([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"REGULATORY AUTHORITIES DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_AUTHORITY.GetDescription();

                //..get regulatory authorities data
                var authoritiesData = await _regulatoryAuthorityService.GetPagedAuthoritiesAsync(request);
                PagedResponse<GrcRegulatoryAuthorityResponse> authoritiesList = new();

                if (authoritiesData.HasError) {
                    Logger.LogActivity($"REGULATORY AUTHORITY DATA ERROR: Failed to retrieve authority items - {JsonSerializer.Serialize(authoritiesData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    authoritiesList = authoritiesData.Data;
                    Logger.LogActivity($"REGULATORY AUTHORITY DATA - {JsonSerializer.Serialize(authoritiesList)}");
                }
                authoritiesList.Entities ??= new();
                var pagedEntities = authoritiesList.Entities
                    .Select(t => new {
                        id = t.Id,
                        authorityName = t.AuthorityName,
                        authorityAlias = t.AuthorityAlias,
                        status = t.IsDeleted ? "Inactive" : "Active",
                        addedon = t.CreatedOn.ToString("dd-MM-yyyy"),
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)authoritiesList.TotalCount / authoritiesList.Size);
                return Ok(new { last_page = totalPages, total_records = authoritiesList.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory authority items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-AUTHGORITY-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add Authority", "User added regulatory authority", ActivityTypeDefaults.COMPLIACE_CREATE_AUTHORITY, "Authorities")]
        public async Task<IActionResult> CreateRegulatoryAuthority([FromBody] RegulatoryAuthorityRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_CREATE_AUTHORITY.GetDescription();

                var result = await _regulatoryAuthorityService.CreateAuthorityAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to create authority";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var created = result.Data;
                return Ok(new {
                    success = true,
                    message = "Authority created successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating authority: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Update Authority", "User modified regulatory authority", ActivityTypeDefaults.COMPLIANCE_EDITED_AUTHORITY, "Authorities")]
        public async Task<IActionResult> UpdateRegulatoryAuthority([FromBody] RegulatoryAuthorityRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_AUTHORITY.GetDescription();

                var result = await _regulatoryAuthorityService.UpdateAuthorityAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to update authority";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Authority updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating authority: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Delete Authority", "User deleted regulatory authority", ActivityTypeDefaults.COMPLIANCE_DELETED_AUTHORITY, "Authorities")]
        public async Task<IActionResult> DeleteRegulatoryAuthority(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var errMsg = userResponse.Error?.Message ?? "Your session has expired. Please login";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Authority Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequest deleteRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_AUTHORITY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryAuthorityService.DeleteAuthorityAsync(deleteRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while deleting authority";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var response = result.Data;
                return Ok(new {
                    message = response.Message,
                    success = response.Status,
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting authority: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [LogActivityResult("Export authorities", "User exported regulatory authorities to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_AUTHORITY, "Authorities")]
        public IActionResult ExcelExportAuthorities([FromBody] List<GrcRegulatoryAuthorityResponse> data) {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Regulation Authorities");

            //..headers
            worksheet.Cell(1, 2).Value = "Authority";
            worksheet.Cell(1, 2).Value = "Alias";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var item in data) {
                worksheet.Cell(row, 2).Value = item.AuthorityName;
                worksheet.Cell(row, 2).Value = item.AuthorityAlias;
                worksheet.Cell(row, 3).Value = item.IsDeleted ? "Inactive" : "Active";
                worksheet.Cell(row, 4).Value = item.CreatedOn.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryAuthorities.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export authorities", "User exported regulatory authorities to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_AUTHORITY, "Authorities")]
        public async Task<IActionResult> ExcelExportAllAuthorities() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null) {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_AUTHORITIES.GetDescription()
            };

            var typeData = await _regulatoryAuthorityService.GetPagedAuthoritiesAsync(request);
            if (typeData.HasError || typeData.Data == null) {
                var errMsg = typeData.Error?.Message ?? "Failed to retrieve authorities";
                Logger.LogActivity(errMsg);
                return Ok(new {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Authorities");

            //..headers
            ws.Cell(1, 2).Value = "Authority";
            ws.Cell(1, 2).Value = "Alias";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var cat in typeData.Data.Entities) {
                ws.Cell(row, 2).Value = cat.AuthorityName;
                ws.Cell(row, 2).Value = cat.AuthorityAlias;
                ws.Cell(row, 3).Value = cat.IsDeleted ? "Inactive" : "Active";
                ws.Cell(row, 4).Value = cat.CreatedOn.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryAuthorities.xlsx"
            );
        }

        #endregion

        #region support - Document Types

        public async Task<IActionResult> ComplianceDocumentType() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        TempData["UiResponse"] = JsonSerializer.Serialize(
                            UiResponse.Fail("Your session expired. Please login again.")
                        );
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Compliance Document Types view: {ex.Message}", "ERROR");
                TempData["UiResponse"] = JsonSerializer.Serialize(
                    UiResponse.Fail("Error loading Compliance Document Types view")
                );
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentTypes() {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"DOCUMENT TYPE LIST ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new() {
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_RETRIEVE_DOCTYPE.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all document types
                var doctypeData = await _documentTypeService.GetDocumentListAsync(request);

                List<DocumentTypeResponse> documentTypes;
                if (doctypeData.HasError) {
                    documentTypes = new();
                    Logger.LogActivity($"DOCUMENT TYPE DATA ERROR: Failed to retrieve type items - {JsonSerializer.Serialize(doctypeData)}");
                } else {
                    documentTypes = doctypeData.Data;
                    Logger.LogActivity($"DOCUMENT TYPES DATA - {JsonSerializer.Serialize(documentTypes)}");
                }

                //..get ajax data
                List<object> select2Data = new();
                if (documentTypes.Any()) {
                    select2Data = documentTypes.Select(type => new {
                        id = type.Id,
                        text = type.TypeName,
                        isDeleted = type.IsDeleted,
                        addedBy = type.CreatedBy ?? string.Empty,
                        addedon = type.CreatedOn.ToString("dd-MM-yyyy")
                    }).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentType(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                if (id == 0)
                    return BadRequest(new { success = false, message = "Type Id is required" });

                var currentUser = userResponse.Data;
                var request = new GrcIdRequest {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_GET_DOCTYPE.GetDescription(),
                    IsDeleted = false
                };

                var result = await _documentTypeService.GetDocumentTypeAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to retrieve document type" });

                var response = result.Data;
                var record = new {
                    id = response.Id,
                    typeName = response.TypeName,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    addedon = response.CreatedOn.ToString("dd-MM-yyyy")
                };

                return Ok(new { success = true, data = record });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return StatusCode(500, new {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add document type", "User added document type", ActivityTypeDefaults.COMPLIANCE_CREATE_DOCTYPE, "Document")]
        public async Task<IActionResult> CreateDocumentType([FromBody] DocumentTypeViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Invalid request" });

                var currentUser = userResponse.Data;
                var result = await _documentTypeService.CreateTypeAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create document type" });

                var created = result.Data;
                return Ok(new { success = true, message = "Document type created successfully", data = new { } });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error creating document type" });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Modify document type", "User modified document type", ActivityTypeDefaults.COMPLIANCE_EDITED_DOCTYPE, "Document")]
        public async Task<IActionResult> UpdateDocumentType([FromBody] DocumentTypeViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Invalid request" });

                var currentUser = userResponse.Data;
                var result = await _documentTypeService.UpdateTypeAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update document type" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Document type updated successfully", data = new { } });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error updating document type" });
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Export document types", "User exported document types to excel", ActivityTypeDefaults.COMPLIANCE_DELETED_DOCTYPE, "Document")]
        public async Task<IActionResult> DeleteDocumentType(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var errMsg = userResponse.Error?.Message ?? "Unable to resolve current user";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg });
                }

                if (id == 0) {
                    var errMsg = "Type Id is required";
                    Logger.LogActivity(errMsg);
                    return BadRequest(new { success = false, message = errMsg });
                }

                var currentUser = userResponse.Data;
                var request = new GrcIdRequest {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_DELETED_DOCTYPE.GetDescription(),
                    IsDeleted = true
                };

                var result = await _documentTypeService.DeleteTypeAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to delete document type";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg });
                }

                return Ok(new { success = true, message = result.Data.Message });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error deleting document type" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AllDocumentTypes([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { last_page = 0, data = new List<object>() });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_DOCTYPE.GetDescription();

                var typeData = await _documentTypeService.GetPagedDocumentTypesAsync(request);
                PagedResponse<DocumentTypeResponse> docList = new();

                if (typeData.HasError) {
                    Logger.LogActivity($"DOCUMENT TYPES DATA ERROR: Failed to retrieve authority items - {JsonSerializer.Serialize(docList)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    docList = typeData.Data;
                    Logger.LogActivity($"DOCUMENT TYPES DATA - {JsonSerializer.Serialize(docList)}");
                }

                docList.Entities ??= new();
                var pagedEntities = docList.Entities
                    .Select(t => new {
                        id = t.Id,
                        startTab = "",
                        typeName = t.TypeName,
                        status = t.IsDeleted ? "Inactive" : "Active",
                        addedon = t.CreatedOn.ToString("dd-MM-yyyy"),
                        endTab = ""
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)docList.TotalCount / docList.Size);
                return Ok(new { last_page = totalPages, total_records = docList.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Export document types", "User exported document types to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_AUTHORITY, "Document")]
        public IActionResult ExcelExportDoctypes([FromBody] List<DocumentTypeResponse> data) {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Document Types");

            //..headers
            worksheet.Cell(1, 2).Value = "Document Type";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var item in data) {
                worksheet.Cell(row, 2).Value = item.TypeName;
                worksheet.Cell(row, 3).Value = item.IsDeleted ? "Inactive" : "Active";
                worksheet.Cell(row, 4).Value = item.CreatedOn.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Document_types.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export document types", "User exported document types to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_AUTHORITY, "Document")]
        public async Task<IActionResult> ExcelExportAllDoctypes() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null) {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_DOCTYPES.GetDescription()
            };

            var typeData = await _documentTypeService.GetPagedDocumentTypesAsync(request);
            if (typeData.HasError || typeData.Data == null) {
                var errMsg = typeData.Error?.Message ?? "Failed to retrieve document types";
                Logger.LogActivity(errMsg);
                return Ok(new {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Document_Types");

            //..headers
            ws.Cell(1, 2).Value = "Document Type";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var cat in typeData.Data.Entities) {
                ws.Cell(row, 2).Value = cat.TypeName;
                ws.Cell(row, 3).Value = cat.IsDeleted ? "Inactive" : "Active";
                ws.Cell(row, 4).Value = cat.CreatedOn.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Document_Types.xlsx"
            );
        }

        #endregion

        #region support - Departments

        public async Task<IActionResult> ComplianceDepartments() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Compliance Departments view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }

        #endregion

        #region support - Responsibilities

        public async Task<IActionResult> ComplianceResponsibilities() {
            try {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        Statistics = new()
                    };

                    return View(userDashboard);
                }
                else {
                    return RedirectToAction("Login", "Application");
                }

            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Compliance Regulatory Categories view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetResponsibilities() {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"DOCUMENT OWNER LIST ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new() {
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_RETRIEVE_DOCOWNERS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all owners
                var ownerData = await _responsibilityService.GetAllAsync(request);

                List<OwnerResponse> owners;
                if (ownerData.HasError) {
                    owners = new();
                    Logger.LogActivity($"DOCUMENT OWNER DATA ERROR: Failed to retrieve owners - {JsonSerializer.Serialize(ownerData)}");
                } else {
                    owners = ownerData.Data;
                    Logger.LogActivity($"DOCUMENT OWNERS DATA - {JsonSerializer.Serialize(owners)}");
                }

                //..get ajax data
                List<object> select2Data = new();
                if (owners.Any()) {
                    select2Data = owners.Select(type => new {
                        id = type.Id,
                        ownerName = type.Name,
                        ownerPhone = type.Phone,
                        ownerEmail = type.Email,
                        ownerPosition = type.Position,
                        ownerComment = type.Comment,
                        departmentId = type.DepartmentId,
                        department = type.Department
                    }).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-OWNER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetResponsibility(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                if (id == 0)
                    return BadRequest(new { success = false, message = "Owner Id is required" });

                var currentUser = userResponse.Data;
                var request = new GrcIdRequest {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_GET_DOCOWNER.GetDescription(),
                    IsDeleted = false
                };

                var result = await _responsibilityService.GetOwnerAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to retrieve document owner" });

                var response = result.Data;
                var record = new {
                    id = response.Id,
                    ownerName = response.Name,
                    ownerPhone = response.Phone,
                    ownerEmail = response.Email,
                    ownerPosition = response.Position,
                    ownerComment = response.Comment,
                    departmentId = response.DepartmentId,
                    department = response.Department,
                    isActive = response.IsDeleted ? "Inactive" : "Active",
                    addedon = response.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new { success = true, data = record });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error retrieving document type" });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add document owner", "User added document owner", ActivityTypeDefaults.COMPLIANCE_CREATE_DOCOWNER, "Responsibility")]
        public async Task<IActionResult> CreateResponsibility([FromBody] OwnerViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current owner" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Invalid request" });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_CREATE_DOCOWNER.GetDescription();

                var result = await _responsibilityService.CreateOwnerAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create document owner" });

                var created = result.Data;
                var record = new {
                    id = created.Id,
                    ownerName = created.Name,
                    ownerPhone = created.Phone,
                    ownerEmail = created.Email,
                    ownerPosition = created.Position,
                    ownerComment = created.Comment,
                    departmentId = created.DepartmentId,
                    department = created.Department,
                    isActive = created.IsDeleted ? "Inactive" : "Active",
                    addedon = created.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new { success = true, message = "Document owner created successfully", data = record });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-OWNER", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error creating document owner" });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Modify document owner", "User modified document owner", ActivityTypeDefaults.COMPLIANCE_EDITED_DOCOWNER, "Responsibility")]
        public async Task<IActionResult> UpdateResponsibility([FromBody] OwnerViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Invalid request" });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_DOCOWNER.GetDescription();

                var result = await _responsibilityService.UpdateOwnerAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update document owner" });

                var updated = result.Data;
                var record = new {
                    id = updated.Id,
                    ownerName = updated.Name,
                    ownerPhone = updated.Phone,
                    ownerEmail = updated.Email,
                    ownerPosition = updated.Position,
                    ownerComment = updated.Comment,
                    departmentId = updated.DepartmentId,
                    department = updated.Department,
                    isActive = updated.IsDeleted ? "Inactive" : "Active",
                    addedon = updated.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new { success = true, message = "Document owner updated successfully", data = record });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-OWNER", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error updating document owner" });
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Export document owner", "User exported document owners to excel", ActivityTypeDefaults.COMPLIANCE_DELETED_DOCOWNER, "Responsibility")]
        public async Task<IActionResult> DeleteResponsibility(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var errMsg = userResponse.Error?.Message ?? "Unable to resolve current user";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg });
                }

                if (id == 0) {
                    var errMsg = "Owner Id is required";
                    Logger.LogActivity(errMsg);
                    return BadRequest(new { success = false, message = errMsg });
                }

                var currentUser = userResponse.Data;
                var request = new GrcIdRequest {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_DELETED_DOCOWNER.GetDescription(),
                    IsDeleted = true
                };

                var result = await _responsibilityService.DeleteOwnerAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to delete document owner";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg });
                }

                return Ok(new { success = true, message = result.Data.Message });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-OWNER", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error deleting document owner" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AllResponsibilities([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { last_page = 0, data = new List<object>() });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_DOCOWNERS.GetDescription();

                var ownerData = await _responsibilityService.GetAllOwners(request);
                PagedResponse<OwnerResponse> ownerList = new();

                if (ownerData.HasError) {
                    Logger.LogActivity($"DOCUMENT OWNERS DATA ERROR: Failed to retrieve authority items - {JsonSerializer.Serialize(ownerList)}");
                } else {
                    ownerList = ownerData.Data;
                    Logger.LogActivity($"DOCUMENT OWNERS DATA - {JsonSerializer.Serialize(ownerList)}");
                }
                ownerList.Entities ??= new();

                var pagedEntities = ownerList.Entities
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(t => new {
                        id = t.Id,
                        startTab = "",
                        ownerName = t.Name,
                        ownerPhone = t.Phone,
                        ownerEmail = t.Email,
                        ownerPosition = t.Position,
                        ownerComment = t.Comment,
                        departmentId = t.DepartmentId,
                        department = t.Department,
                        isActive = t.IsDeleted ? "Inactive" : "Active",
                        endTab = ""
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)ownerList.TotalCount / ownerList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = ownerList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-OWNERS", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Export document owners", "User exported document owners to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_DOCOWNER, "Responsibility")]
        public IActionResult ExcelExportResponsibilities([FromBody] List<OwnerResponse> data) {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Document Owners");

            //..headers
            worksheet.Cell(1, 2).Value = "ONWER NAME";
            worksheet.Cell(1, 3).Value = "PHONE NUMBER";
            worksheet.Cell(1, 4).Value = "EMAIL ADDRESS";
            worksheet.Cell(1, 4).Value = "DEPARTMENT";
            worksheet.Cell(1, 4).Value = "POSITION";

            int row = 2;
            foreach (var item in data) {
                worksheet.Cell(row, 2).Value = item.Name;
                worksheet.Cell(row, 2).Value = item.Phone;
                worksheet.Cell(row, 2).Value = item.Email;
                worksheet.Cell(row, 2).Value = item.Department;
                worksheet.Cell(row, 2).Value = item.Position;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryAuthorities.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export document owners", "User exported document owners to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_DOCOWNER, "Responsibility")]
        public async Task<IActionResult> ExcelExportAllResponsibilities() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null) {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_DOCOWNERS.GetDescription()
            };

            var ownerData = await _responsibilityService.GetAllOwners(request);
            if (ownerData.HasError || ownerData.Data == null) {
                var errMsg = ownerData.Error?.Message ?? "Failed to retrieve document owners";
                Logger.LogActivity(errMsg);
                return Ok(new {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Document_Types");

            //..headers
            ws.Cell(1, 2).Value = "ONWER NAME";
            ws.Cell(1, 3).Value = "PHONE NUMBER";
            ws.Cell(1, 4).Value = "EMAIL ADDRESS";
            ws.Cell(1, 4).Value = "DEPARTMENT";
            ws.Cell(1, 4).Value = "POSITION";

            int row = 2;
            foreach (var cat in ownerData.Data.Entities) {
                ws.Cell(row, 2).Value = cat.Name;
                ws.Cell(row, 2).Value = cat.Phone;
                ws.Cell(row, 2).Value = cat.Email;
                ws.Cell(row, 2).Value = cat.Department;
                ws.Cell(row, 2).Value = cat.Position;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Document_Owners.xlsx"
            );
        }

        #endregion

        #region Users
        public async Task<IActionResult> ComplianceUsers() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }

            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Compliance Regulatory Categories view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }

        #endregion

        #region Delegation
        public async Task<IActionResult> ComplianceDelegation() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        Statistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }


            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Compliance Regulatory Categories view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }

        #endregion


    }
}

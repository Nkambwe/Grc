using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using Grc.ui.App.Defaults;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Helpers;
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
        public RegisterController(IApplicationLoggerFactory loggerFactory,
                                  IEnvironmentProvider environment,
                                  IWebHelper webHelper,
                                  ILocalizationService localizationService,
                                  IErrorService errorService,
                                  IAuthenticationService authService,
                                  IStatutorySectionService regulatoryActService,
                                  IRegulatoryStatuteService statuteService,
                                  IComplianceControlService controlsService,
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
                        DashboardStatistics = new()
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
                        DashboardStatistics = new()
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
                return Ok(new{success = true,message = "Category created successfully",data = new { }});

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

                if (id == 0) return BadRequest(new { success = false, message = "Task Id is required" });

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
                        DashboardStatistics = new()
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
                    complianceMaps = response.ComplianceMaps
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

                return Ok(new { success = false, message = "Compliance map saved successfully"});
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error occurred while saving compliance map: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message="Could not save map, an unexpected error occurred"});
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
                        DashboardStatistics = new()
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

        [LogActivityResult("Add Compliance control", "User retrieved added new compliance control", ActivityTypeDefaults.COMPLIANCE_CONTROLCATEGORY_CREATE, "ControlCategory")]
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

        [LogActivityResult("Update Compliance control", "User retrieved updated compliance control", ActivityTypeDefaults.COMPLIANCE_CONTROLCATEGORY_UPDATE, "ControlCategory")]
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

        [LogActivityResult("Delete control item", "User retrieved deleted control item", ActivityTypeDefaults.COMPLIANCE_CONTROLITEM_DELETE, "ControlItem")]
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

        #region Regulatory Returns
        public async Task<IActionResult> RegulationReturns() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        #endregion

        #region Circular Obligations
        public async Task<IActionResult> RegulationCirculars() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel() {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        #endregion

        #region Manage Regulations
        public async Task<IActionResult> ManageRegulations()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        #endregion

        #region Regulatory Maps
        public IActionResult RegulationMaps() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        #endregion

    }
}

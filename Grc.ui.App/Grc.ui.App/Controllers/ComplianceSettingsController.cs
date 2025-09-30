using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
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
    public class ComplianceSettingsController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IRegulatonServiceService _regulatoryService;

        public ComplianceSettingsController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, 
            IWebHelper webHelper, 
            ILocalizationService localizationService, 
            IErrorService errorService, 
            IAuthenticationService authService,
            IGrcErrorFactory errorFactory,
            IRegulatonServiceService regulatoryService,
            SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, 
                  localizationService, errorService,
                  errorFactory, sessionManager) {

            Logger.Channel = $"COMP-SETTINGS-{DateTime.Now:yyyyMMddHHmmss}";
             _authService = authService;
            _regulatoryService = regulatoryService;
        }

        public async Task<IActionResult> ComplianceRegulatoryTypes()
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

        public async Task<IActionResult> ComplianceRegulatoryCategories()
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

        public async Task<IActionResult> ComplianceAuthorities()
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

        public async Task<IActionResult> ComplianceDocumentType() {
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

        public async Task<IActionResult> ComplianceUsers() {
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

        public async Task<IActionResult> ComplianceDelegation()
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

        public async Task<IActionResult> ComplianceDepartments()
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

        public async Task<IActionResult> ComplianceResponsibilities()
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

        #region Data Actions
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
                var categoryData = await _regulatoryService.GetRegulatoryCategories(request);

                List<RegulatoryCategoryResponse> categories;
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
                    status = cat.IsDeleted,
                    addedon = cat.CreatedAt.ToString("dd-MM-yyyy"),
                    endTab = ""
                }).ToList();

                return Json(new { success = true, data = tabulatorData });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory categories: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-CONTROLLER", ex.StackTrace);
                return Json(new { success = false, data = new List<object>() });
            }
        }

        public async Task<IActionResult> AllRegulatoryCategories([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIEVEREGULATORYCATEGORIES.GetDescription();

                //..get regulatory category data
                var categoryData = await _regulatoryService.GetAllRegulatoryCategories(request);
                PagedResponse<RegulatoryCategoryResponse> categoryList = new();

                if (categoryData.HasError) {
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to retrieve category items - {JsonSerializer.Serialize(categoryData)}");
                } else {
                    categoryList = categoryData.Data;
                    Logger.LogActivity($"REGULATORY CATEGORY DATA - {JsonSerializer.Serialize(categoryList)}");
                } 
                categoryList.Entities ??= new();
                //..use this for final code from DB
                //var tabulatorData = categoryList.Entities.Select(cat => new
                //{
                //    id = cat.Id,
                //    startTab = "",
                //    category = cat.CategoryName,
                //    status = cat.IsDeleted ? "Inactive" : "Active",  
                //    addedon = cat.CreatedAt.ToString("dd-MM-yyyy"),
                //    endTab = ""
                //}).ToList();

                ////..response
                //return Ok(new
                //{
                //    last_page = categoryList.TotalPages,
                //    data = tabulatorData,
                //    total_records = categoryList.TotalCount  
                //});

                

                var pagedEntities = categoryList.Entities
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(cat => new {
                        id = cat.Id,
                        startTab = "",
                        category = cat.CategoryName,
                        status = cat.IsDeleted ? "Inactive" : "Active",
                        addedon = cat.CreatedAt.ToString("dd-MM-yyyy"),
                        endTab = ""
                    }).ToList();

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    pagedEntities = pagedEntities.Where(c => c.category.ToLower().Contains(request.SearchTerm.ToLower())).ToList();
                }

                //..apply sorting
                //if (!string.IsNullOrEmpty(request.SortBy))
                //{
                //    if (request.SortDirection == "Ascending")
                //        pagedEntities = pagedEntities.OrderBy(a => a.category == request.SortBy).ToList();
                //    else
                //        pagedEntities = pagedEntities.OrderByDescending(a => a.category == request.SortBy).ToList();
                //}

                var totalPages = (int)Math.Ceiling((double)categoryList.TotalCount / categoryList.Size);

                return Ok(new
                {
                    last_page = totalPages,
                    total_records = categoryList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory category items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-CONTROLLER", ex.StackTrace);
                return Ok(new
                {
                    last_page = 0,
                    data = new List<object>()
                });
            }
        }

        #endregion
    }
}

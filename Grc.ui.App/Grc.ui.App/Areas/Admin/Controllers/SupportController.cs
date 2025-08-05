using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Areas.Admin.Controllers {

    [Area("Admin")]
    public class SupportController : AdminBaseController {
        private readonly ISystemAccessService _accessService;
        private readonly IAuthenticationService _authService;
        public SupportController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment, 
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISystemAccessService accessService,
                                 IAuthenticationService authService) 
            : base(loggerFactory, environment, webHelper, localizationService) {
           _accessService = accessService;
            _authService = authService;
        }

        public async Task<IActionResult> Index(){
            var model = new AdminDashboardModel();
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    return HandleLoginErrors(grcResponse.Error.Message, model);
                }

                var currentUser = grcResponse.Data;
                model.WelcomeMessage = $"{LocalizationService.GetLocalizedLabel("App.Label.Welcome")}, {currentUser?.FirstName}!";
                model.TotalUsers = (await _accessService.CountAllUsersAsync(currentUser.Id, ipAddress)).Count;
                model.ActiveUsers = (await _accessService.CountActiveUsersAsync(currentUser.Id, ipAddress)).Count;
                model.Initials =$"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}";

                model.QuickActions = new List<QuickActionModel> {
                    new() { 
                        Label = "App.Menu.Users", 
                        IconClass = "mdi mdi-account-outline", 
                        Controller = "Support",
                        Action = "Users",
                        Area = "Admin",
                        CssClass = "" 
                    },
                    new() { 
                        Label = "App.Menu.Departments", 
                        IconClass = "mdi mdi-share-all-outline", 
                        Controller = "Support",
                        Action = "Departments",
                        Area = "Admin",
                        CssClass = ""
                    },
                    new() { 
                        Label = "App.Menu.Permissions.Assign", 
                        IconClass = "mdi mdi-shield-check-outline", 
                        Controller = "Support",
                        Action = "AssignPermissions",
                        Area = "Admin",
                        CssClass = "" 
                    }
                    // Load from DB or user prefs in future
                };
                
                model.Recents = new List<RecentModel> {
                    new() {
                        Label = "App.Menu.Users", 
                        IconClass = "mdi mdi-account-outline", 
                        Controller = "Support",
                        Action = "Users",
                        Area = "Admin",
                        CssClass = "" 
                    },
                    new() { 
                        Label = "App.Menu.Departments", 
                        IconClass = "mdi mdi-share-all-outline", 
                        Controller = "Support",
                        Action = "Departments",
                        Area = "Admin",
                        CssClass = ""
                    },
                    new() { 
                        Label = "App.Menu.Permissions.Assign", 
                        IconClass = "mdi mdi-shield-check-outline", 
                        Controller = "Support",
                        Action = "AssignPermissions",
                        Area = "Admin",
                        CssClass = "" 
                    },
                    new() { 
                        Label = "App.Menu.Configurations.Data", 
                        IconClass = "mdi mdi-account-details-outline", 
                        Controller = "Configuration",
                        Action = "UserData",
                        Area = "Admin",
                        CssClass = "" 
                    },
                    new() { 
                        Label = "App.Menu.Configurations.Groups", 
                        IconClass = "mdi mdi-account-group-outline", 
                        Controller = "Configuration",
                        Action = "UserGroups",
                        Area = "Admin",
                        CssClass = "" 
                    }
                    // Load from session
                };

                model.PinnedItems = new List<PinnedModel> {
                    new() { 
                        Label = "App.Menu.Configurations.Data", 
                        IconClass = "mdi mdi-account-details-outline", 
                        Controller = "Configuration",
                        Action = "UserData",
                        Area = "Admin",
                        CssClass = "" 
                    },
                    new() { 
                        Label = "App.Menu.Configurations.Groups", 
                        IconClass = "mdi mdi-account-group-outline", 
                        Controller = "Configuration",
                        Action = "UserGroups",
                        Area = "Admin",
                        CssClass = "" 
                    }
                };

                model.LastLogin = DateTime.UtcNow;
            } catch(Exception ex){ 
                Logger.LogActivity($"Username validation error: {ex.Message}", "ERROR");
                return HandleLoginErrors(LocalizationService.GetLocalizedLabel("Error.Service.Unavailable"), model);
            }

            return View(model);
        }

        public async Task<IActionResult> Departments() {
            return View();
        }

        public async Task<IActionResult> Users() {
            return View();
        }
        
        public async Task<IActionResult> Roles() {
             return View();
        }

        public async Task<IActionResult> PermissionSets() {
            return View();
        }
        
        public async Task<IActionResult> AssignPermissions() {
            return View();
        }

        public async Task<IActionResult> PermissionDelegation() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout() {
            try {
                var username = User.Identity?.Name;
                Logger.LogActivity($"Admin user initiating logout: {username}", "INFO");
        
                if (WebHelper.IsAjaxRequest(Request)) {
                    //..for AJAX requests, return the logout URL
                    return Json(new { 
                        success = true, 
                        redirectUrl = Url.Action("Logout", "Application", new { area = "" }),
                        message = "Logging out..."
                    });
                }
        
                //..for non-AJAX, redirect directly
                return RedirectToAction("Logout", "Application", new { area = "" });
            } catch (Exception ex) {
                Logger.LogActivity($"Error during admin logout: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                if (WebHelper.IsAjaxRequest(Request)) {
                    return Json(new { 
                        success = false, 
                        message = "Logout failed. Please try again." 
                    });
                }
        
                return RedirectToAction("Index", "Support");
            }
        }

        private IActionResult HandleLoginErrors(string error, AdminDashboardModel model) {
            //..TODO redirect to login
            return View(model);
        }

        public void Notify(string message, string title = "GRC NOTIFICATION", NotificationType type = NotificationType.Success) {
            var notificationMessage = new NotificationMessage() {
                Title = title,
                Message = message,
                Icon = type.GetEnumMemberValue(),
                Type  = type.GetEnumMemberValue()
            };

            TempData["Message"] = JsonSerializer.Serialize(notificationMessage); 
        }

    }
}

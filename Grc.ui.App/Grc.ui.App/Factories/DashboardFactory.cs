using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {
    public class DashboardFactory : IDashboardFactory {
        private readonly ILocalizationService _localizationService;
        private readonly IQuickActionService _quickActionService;
        public DashboardFactory(ILocalizationService localizationService,
                                IQuickActionService quickActionService) {  
            _localizationService = localizationService;
            _quickActionService = quickActionService;
        }

        public async Task<UserDashboardModel>  PrepareUserDashboardModelAsync(UserModel currentUser) {
            var model = new UserDashboardModel {
                WelcomeMessage = $"{_localizationService.GetLocalizedLabel("App.Label.Welcome")}, {currentUser?.FirstName}!",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = new List<QuickActionModel> {
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
                },

                Recents = new List<RecentModel> {
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
                },

                PinnedItems = new List<PinnedModel> {
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
                },

                LastLogin = DateTime.UtcNow
            };
            return await Task.FromResult(model);
        }

    }
}

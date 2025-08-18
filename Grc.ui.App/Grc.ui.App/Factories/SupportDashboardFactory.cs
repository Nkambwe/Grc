using AutoMapper;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class SupportDashboardFactory : ISupportDashboardFactory {

        private readonly ILocalizationService _localizationService;
        private readonly IPinnedService _pinnedService;
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        public SupportDashboardFactory(ILocalizationService localizationService,
                                       IPinnedService pinnedService, 
                                       IQuickActionService quickActionService,
                                       IMapper mapper) {  
            _localizationService = localizationService;
            _pinnedService = pinnedService;
            _quickActionService = quickActionService;
            _mapper = mapper;
        }

        public async Task<AdminDashboardModel> PrepareAdminDashboardModelAsync(UserModel currentUser) {

            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.LastLoginIpAddress);
            var quickActions = new List<QuickActionModel>();
            if(!quicksData.HasError){ 
                var quickies = quicksData.Data;
                if(quickies.Count > 0){ 
                    foreach(var action in quickies){ 
                        quickActions.Add(_mapper.Map<QuickActionModel>(action));
                    }
                }
            }

            //..get pinned items
            var pinData = await _pinnedService.GetPinnedItemAsync(currentUser.UserId, currentUser.LastLoginIpAddress);
            var pins = new List<PinnedModel>();
            if(!pinData.HasError){ 
                var pinItems = pinData.Data;
                if(pinItems.Count > 0){ 
                    foreach(var pin in pinItems){ 
                        pins.Add(_mapper.Map<PinnedModel>(pin));
                    }
                }
            }
            var model = new AdminDashboardModel {
                WelcomeMessage = $"{_localizationService.GetLocalizedLabel("App.Label.Welcome")}, {currentUser?.FirstName}!",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                PinnedItems = pins,
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

                LastLogin = DateTime.UtcNow
            };
            return await Task.FromResult(model);
        }
    }
}

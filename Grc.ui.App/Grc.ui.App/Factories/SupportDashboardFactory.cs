using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Factories {

    public class SupportDashboardFactory : ISupportDashboardFactory {

       
        private readonly ISystemAccessService _accessService;
        private readonly IPinnedService _pinnedService;
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly SessionManager _sessionManager;
        private readonly IConfiguration _configuration;
        public SupportDashboardFactory(IPinnedService pinnedService, 
                                       ISystemActivityService activityService,
                                       ISystemAccessService accessService,
                                       IQuickActionService quickActionService,
                                       IMapper mapper,
                                       SessionManager session,
                                       IConfiguration configuration) {  
            _pinnedService = pinnedService;
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
            _configuration = configuration;
            _accessService = accessService;
            
        }

        public async Task<AdminDashboardModel> PrepareAdminDashboardModelAsync(UserModel currentUser) {
            // Get recents from session
            var recents = _sessionManager.Get<List<RecentModel>>(SessionKeys.RecentItems.GetDescription()) ?? new List<RecentModel>();

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

            //..get base url
            var middlewareOptions = _configuration.GetSection("MiddlewareOptions").Get<MiddlewareOptions>();
            var envOptions = _configuration.GetSection("EnvironmentOptions").Get<EnvironmentOptions>();
            string baseUrl = !(bool)envOptions?.IsLive ? (middlewareOptions?.BaseUrl?.TrimEnd('/')) :
                (middlewareOptions?.ProdBaseUrl?.TrimEnd('/'));

            //..get dashboard statistics
            var stats = (await _accessService.StatisticAsync(currentUser.UserId, currentUser.LastLoginIpAddress));
            
            //..generate dashboard model
            return new AdminDashboardModel {
                MiddlwareUrl = baseUrl,
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}!",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                PinnedItems = pins,
                Recents = recents,
                LastLogin = DateTime.UtcNow,
                //..set workspace into seession
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                TotalUsers = stats.TotalUsers,
                ActiveUsers = stats.ActiveUsers,
                DeactivatedUsers = stats.DeactivatedUsers,
                UnApprovedUsers = stats.UnApprovedUsers,
                UnverifiedUsers = stats.UnverifiedUsers,
                DeletedUsers = stats.DeletedUsers,
                TotalBugs = stats.TotalBugs,
                NewBugs = stats.NewBugs,
                BugFixes = stats.BugFixes,
                BugProgress = stats.BugProgress,
                UserReportedBugs = stats.UserReportedBugs,
            };
        }

        public async Task<AdminDashboardModel> PrepareDefaultModelAsync(UserModel currentUser) {
            // Get recents from session
            var recents = _sessionManager.Get<List<RecentModel>>(SessionKeys.RecentItems.GetDescription()) ?? new List<RecentModel>();

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
            
            return await Task.FromResult(new AdminDashboardModel {
                PinnedItems = pins,
                Recents = recents,
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                //..set workspace into seession
                Workspace = _sessionManager.GetWorkspace(),
            });
        }
    }
}

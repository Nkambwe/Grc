using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Factories {

    public class SupportDashboardFactory : ISupportDashboardFactory {

        private readonly ILocalizationService _localizationService;
        private readonly IPinnedService _pinnedService;
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly SessionManager _sessionManager;
        public SupportDashboardFactory(ILocalizationService localizationService,
                                       IPinnedService pinnedService, 
                                       IQuickActionService quickActionService,
                                       IMapper mapper,
                                       SessionManager session) {  
            _localizationService = localizationService;
            _pinnedService = pinnedService;
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
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
            var model = new AdminDashboardModel {
                WelcomeMessage = $"{_localizationService.GetLocalizedLabel("App.Label.Welcome")}, {currentUser?.FirstName}!",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                PinnedItems = pins,
                Recents = recents,
                LastLogin = DateTime.UtcNow
            };
            return await Task.FromResult(model);
        }
    }
}

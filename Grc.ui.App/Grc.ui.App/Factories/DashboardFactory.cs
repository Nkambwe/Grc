using AutoMapper;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Factories {
    public class DashboardFactory : IDashboardFactory {
        private readonly ILocalizationService _localizationService;
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly SessionManager _sessionManager;
        private readonly IRegistersService _registersService;
        public DashboardFactory(ILocalizationService localizationService,
                                IMapper mapper,
                                IQuickActionService quickActionService,
                                IRegistersService registersService,
                                SessionManager session) {  
            _localizationService = localizationService;
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
            _registersService = registersService;
        }

        public async Task<UserDashboardModel>  PrepareUserDashboardModelAsync(UserModel currentUser) {
            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.LastLoginIpAddress);
            var quickActions = new List<QuickActionModel>();
            if (!quicksData.HasError) {
                var quickies = quicksData.Data;
                if (quickies.Count > 0) {
                    foreach (var action in quickies)
                    {
                        quickActions.Add(_mapper.Map<QuickActionModel>(action));
                    }
                }
            }

            var stats = await _registersService.StatisticAsync(currentUser.UserId, currentUser.LastLoginIpAddress);
            var model = new UserDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

    }
}

using AutoMapper;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Factories {

    public class OperationsDashboardFactory : IOperationsDashboardFactory {
        private readonly ISystemAccessService _accessService;
        private readonly IProcessesService _processesService;
        private readonly IPinnedService _pinnedService;
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly SessionManager _sessionManager;
        private readonly IConfiguration _configuration;

        public OperationsDashboardFactory(IPinnedService pinnedService,
                                       ISystemActivityService activityService,
                                       ISystemAccessService accessService,
                                       IQuickActionService quickActionService,
                                       IProcessesService processesService,
                                       IMapper mapper,
                                       SessionManager session,
                                       IConfiguration configuration) {
            _pinnedService = pinnedService;
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
            _configuration = configuration;
            _accessService = accessService;
            _processesService = processesService;
        }

        public async Task<OperationsDashboardModel> PrepareOperationsDashboardModelAsync(UserModel currentUser) {

            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.LastLoginIpAddress);
            var quickActions = new List<QuickActionModel>();
            if (!quicksData.HasError)
            {
                var quickies = quicksData.Data;
                if (quickies.Count > 0)
                {
                    foreach (var action in quickies)
                    {
                        quickActions.Add(_mapper.Map<QuickActionModel>(action));
                    }
                }
            }

            //..get base url
            var middlewareOptions = _configuration.GetSection("MiddlewareOptions").Get<MiddlewareOptions>();
            var envOptions = _configuration.GetSection("EnvironmentOptions").Get<EnvironmentOptions>();
            string baseUrl = !(bool)envOptions?.IsLive ? (middlewareOptions?.BaseUrl?.TrimEnd('/')) :
                (middlewareOptions?.ProdBaseUrl?.TrimEnd('/'));

            //..get dashboard statistics
            var stats = (await _processesService.StatisticAsync(currentUser.UserId, currentUser.LastLoginIpAddress));

            //..generate dashboard model
            return new OperationsDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}!",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                //..set workspace into session
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                ProposedProcesses = stats.ProposedProcesses,
                CompletedProcesses = stats.CompletedProcesses,
                UnchangedProcesses = stats.UnchangedProcesses,
                ProcessesDueForReview = stats.ProcessesDueForReview,
                DormantProcesses = stats.DormantProcesses,
                CancelledProcesses = stats.CancelledProcesses,
                UnitTotalProcesses = stats.UnitTotalProcesses
            };
        }

        public async Task<OperationsDashboardModel> PrepareDefaultOperationsModelAsync(UserModel currentUser) {
            
            return await Task.FromResult(new OperationsDashboardModel {
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                //..set workspace into seession
                Workspace = _sessionManager.GetWorkspace(),
            });
        }
    }
}

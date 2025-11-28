using AutoMapper;
using Grc.ui.App.Areas.Operations.Helpers;
using Grc.ui.App.Dtos;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Grc.ui.App.Factories {

    public class OperationsDashboardFactory : IOperationsDashboardFactory {
        private readonly IProcessesService _processesService;
        private readonly ILocalizationService _localizationService;
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly SessionManager _sessionManager;

        public OperationsDashboardFactory(IQuickActionService quickActionService,
                                       IProcessesService processesService,
                                       ILocalizationService localizationService,
                                       IMapper mapper,
                                       SessionManager session) {
            _localizationService = localizationService;
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
            _processesService = processesService;
        }

        public async Task<OperationsDashboardModel> PrepareOperationsDashboardModelAsync(UserModel currentUser) {

            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
            var quickActions = new List<QuickActionModel>();
            if (!quicksData.HasError) {
                var quickies = quicksData.Data;
                if (quickies.Count > 0) {
                    foreach (var action in quickies) {
                        quickActions.Add(_mapper.Map<QuickActionModel>(action));
                    }
                }
            }

            //..get dashboard statistics
            var grcResponse = await _processesService.UnitStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            OperationsUnitCountResponse stats;
            if (grcResponse.HasError) {
                stats = new OperationsUnitCountResponse {
                    UnitProcesses = new OperationsUnitStatisticsResponse(),
                    ProcessCategories = new ProcessCategoryStatisticsResponse()
                };
            } else {                
                stats = grcResponse.Data;
            }

            var model = new OperationsDashboardModel {
                    WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}  - Operations Processes",
                    Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                    QuickActions = quickActions,
                    Workspace = _sessionManager.GetWorkspace(),
                    DashboardStatistics = stats
                };

            //..create dashboard cards
            var chart = DashboardCardGenerator.CreateCard(_localizationService, stats);
            model.ChartViewModel = chart;
            return model;
        }

        public async Task<OperationsDashboardModel> PrepareDefaultOperationsModelAsync(UserModel currentUser) {

            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
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

            return await Task.FromResult(new OperationsDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
            });
        }

        public async Task<OperationsDashboardModel> PrepareUnitStatisticsModelAsync(UserModel currentUser, string unit) {

            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
            var quickActions = new List<QuickActionModel>();
            if (!quicksData.HasError) {
                var quickies = quicksData.Data;
                if (quickies.Count > 0) {
                    quickActions.AddRange(from action in quickies select _mapper.Map<QuickActionModel>(action));
                }
            }

            var grcResponse = await _processesService.CategoryCountAsync(currentUser.UserId, currentUser.IPAddress, unit);
            CategoriesCountResponse stats;
            if (grcResponse.HasError) {
                stats = new ();
            } else {
                stats = grcResponse.Data;
            }
            return new OperationsDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                UnitStatistics = stats
            };
        }

        public async Task<TotalExtensionModel> PrepareDefaultTotalExtensionsModelAsync(UserModel currentUser) {

            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
            var quickActions = new List<QuickActionModel>();
            if (!quicksData.HasError) {
                var quickies = quicksData.Data;
                if (quickies.Count > 0) {
                    quickActions.AddRange(from action in quickies select _mapper.Map<QuickActionModel>(action));
                }
            }

            var grcResponse = await _processesService.TotalExtensionsCountAsync(currentUser.UserId, currentUser.IPAddress);
            List<StatisticTotalResponse> charts = new();
            if (grcResponse.HasError) {
                if (charts.Count > 0) {
                    charts.ForEach(c => c.Categories = new());
                }
            } else {
                charts = grcResponse.Data;
            }

            return new TotalExtensionModel
            {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - Processes Categories Per Unit",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                Charts = charts
            };
        }

        public async Task<CategoryExtensionResponse> PrepareCategoryExtensionsModelAsync(UserModel currentUser, string category)
        {
            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
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

            var grcResponse = await _processesService.CategoryExtensionsCountAsync(category, currentUser.UserId, currentUser.IPAddress);
            CategoryExtensionResponse record;
            if (grcResponse.HasError) {
                record = new() {
                    CategoryProcesses = new()
                };
            } else {
                record = grcResponse.Data;
            }
            record.WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - {category} Processes breakdown";
            record.Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}";
            record.QuickActions = quickActions;
            record.Workspace = _sessionManager.GetWorkspace();
            return record;
        }

        public async Task<CategoryExtensionResponse> PrepareDefaultExtensionCategoryErrorModelAsync(UserModel currentUser)
        {
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
            var quickActions = new List<QuickActionModel>();
            if (!quicksData.HasError) {
                var quickies = quicksData.Data;
                if (quickies.Count > 0) {
                    quickActions.AddRange(from action in quickies select _mapper.Map<QuickActionModel>(action));
                }
            }

            //..get dashboard statistics
            var model = new CategoryExtensionResponse
            {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                CategoryProcesses = new(),
                Workspace = _sessionManager.GetWorkspace(),
            };
            return model;
        }

        public async Task<TotalExtensionModel> PrepareExtensionCategoryErrorModelAsync(UserModel currentUser)
        {
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
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

            //..get dashboard statistics
            var stats = await _processesService.UnitStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new TotalExtensionModel
            {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Charts = new(),
                //..set workspace into session
                Workspace = _sessionManager.GetWorkspace(),
            };
            return model;
        }

        public async Task<OperationsDashboardModel> PrepareErrorOperationsDashboardModelAsync(UserModel currentUser) {
            
            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
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

            //..get dashboard statistics
            var grcResponse = await _processesService.UnitStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            OperationsUnitCountResponse stats;
            if (grcResponse.HasError) {
                stats = new OperationsUnitCountResponse {
                    UnitProcesses = new OperationsUnitStatisticsResponse(),
                    ProcessCategories = new ProcessCategoryStatisticsResponse()
                };
            } else {
                stats = grcResponse.Data;
            }

            var model = new OperationsDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}  - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                //..set workspace into session
                Workspace = _sessionManager.GetWorkspace(),
                DashboardStatistics = stats,
                ChartViewModel = new DashboardChartViewModel()
            };
            return model;
        }

        public async Task<UnitExtensionCountResponse> PrepareUnitExtensionsModelAsync(UserModel currentUser, string unit)
        {
            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
            var quickActions = new List<QuickActionModel>();
            if (!quicksData.HasError)
            {
                var quickies = quicksData.Data;
                if (quickies.Count > 0) {
                    quickActions.AddRange(from action in quickies select _mapper.Map<QuickActionModel>(action));
                }
            }

            UnitExtensionCountResponse record = new(); 
            var grcResponse = await _processesService.UnitExtensionsCountAsync(unit, currentUser.UserId, currentUser.IPAddress);

            //..check if we have any server errors
            if (grcResponse.HasError) {
                record.UnitProcesses = new();
            } else {
                record = grcResponse.Data;
            }

            //..initialize other properties
            record.WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - {unit} Processes breakdown";
            record.Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}";
            record.QuickActions = quickActions;
            record.Workspace = _sessionManager.GetWorkspace();
            return record;
        }

        public async Task<UnitExtensionCountResponse> PrepareDefaultExtensionUnitErrorModelAsync(UserModel currentUser)
        {
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
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

            //..get dashboard statistics
            var model = new UnitExtensionCountResponse
            {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}  - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                UnitProcesses = new(),
                Workspace = _sessionManager.GetWorkspace(),
            };
            return model;
        }
    }
}

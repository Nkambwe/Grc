using AutoMapper;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

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
            var stats = await _processesService.StatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new OperationsDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}  - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                //..set workspace into session
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            //..create dashboard cards
            var chart = new DashboardChartViewModel();

            //..process cards
            chart.ProcessCards.Add(new StatCardViewModel {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.TotalProcesses"),
                Value = stats.UnitProcesses.TotalUnitProcess.TotalProcesses,
                CssClass = "stat-separator-default",
                Controller = "OperationDashboard",
                Action = "TotalProcesses"
            });

            chart.ProcessCards.Add(new StatCardViewModel {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Proposed"),
                Value = stats.UnitProcesses.ProposedProcesses.TotalProcesses,
                CssClass = "stat-separator-primary",
                Controller = "OperationDashboard",
                Action = "Proposed"
            });
            chart.ProcessCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Unchanged"),
                Value = stats.UnitProcesses.UnchangedProcesses.TotalProcesses,
                CssClass = "stat-separator-nuetral",
                Controller = "OperationDashboard",
                Action = "Unchanged"
            });
            chart.ProcessCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.DueForReview"),
                Value = stats.UnitProcesses.ProcessesDueForReview.TotalProcesses,
                CssClass = "stat-separator-danger",
                Controller = "OperationDashboard",
                Action = "Due"
            });
            chart.ProcessCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Dormant"),
                Value = stats.UnitProcesses.DormantProcesses.TotalProcesses,
                CssClass = "stat-separator-warning",
                Controller = "OperationDashboard",
                Action = "Dormant"
            });
            chart.ProcessCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Cancelled"),
                Value = stats.UnitProcesses.CancelledProcesses.TotalProcesses,
                CssClass = "stat-separator-cancelled",
                Controller = "OperationDashboard",
                Action = "Cancelled"
            });

            chart.ProcessCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Completed"),
                Value = stats.UnitProcesses.CompletedProcesses.TotalProcesses,
                CssClass = "stat-separator-cancelled",
                Controller = "OperationDashboard",
                Action = "Completed"
            });


            //..unit cards
            chart.UnitCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.AccountServices"),
                Value = stats.ProcessCategories.AccountServiceProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "AccountServices"
            });
            chart.UnitCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Cash"),
                Value = stats.ProcessCategories.CashProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Cash"
            });
            chart.UnitCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Channels"),
                Value = stats.ProcessCategories.ChannelProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Channels"
            });
            chart.UnitCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Payments"),
                Value = stats.ProcessCategories.PaymentProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Payments"
            });
            chart.UnitCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Wallets"),
                Value = stats.ProcessCategories.WalletProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Wallets"
            });
            chart.UnitCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Records"),
                Value = stats.ProcessCategories.RecordsMgtProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "RecordsManagement"
            });
            chart.UnitCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.CustomerExperience"),
                Value = stats.ProcessCategories.CustomerExperienceProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "CustomerExperience"
            });
            chart.UnitCards.Add(new StatCardViewModel
            {
                Title = _localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Reconciliation"),
                Value = stats.ProcessCategories.ReconciliationProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Reconciliation"
            });

            //..doughnut for category totals
            chart.CategoryTotals = new Dictionary<OperationUnit, int> {
                { OperationUnit.AccountServices, stats.ProcessCategories.AccountServiceProcesses.Total },
                { OperationUnit.Cash, stats.ProcessCategories.CashProcesses.Total },
                { OperationUnit.Channels, stats.ProcessCategories.ChannelProcesses.Total },
                { OperationUnit.Payments, stats.ProcessCategories.PaymentProcesses.Total },
                { OperationUnit.Wallets, stats.ProcessCategories.WalletProcesses.Total },
                { OperationUnit.RecordsMgt, stats.ProcessCategories.RecordsMgtProcesses.Total },
                { OperationUnit.CustomerExp, stats.ProcessCategories.CustomerExperienceProcesses.Total },
                { OperationUnit.Reconciliation, stats.ProcessCategories.ReconciliationProcesses.Total }
            };

            //..stacked bar for status summaries
            chart.StatusSummaries = new List<ProcessSummary>
            {
                new() {
                    Status = ProcessCategories.UpToDate,
                    Categories = new Dictionary<OperationUnit, int> {
                        { OperationUnit.AccountServices,stats.UnitProcesses.CompletedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.CompletedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.CompletedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.CompletedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.CompletedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.CompletedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.CompletedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.CompletedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Proposed,
                    Categories = new Dictionary<OperationUnit, int> {
                        {  OperationUnit.AccountServices, stats.UnitProcesses.ProposedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.ProposedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.ProposedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.ProposedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.ProposedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.ProposedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.ProposedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.ProposedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Unchanged,
                    Categories = new Dictionary<OperationUnit, int> {
                        { OperationUnit.AccountServices,stats.UnitProcesses.CompletedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.CompletedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.CompletedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.CompletedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.CompletedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.CompletedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.CompletedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.CompletedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Due,
                    Categories = new Dictionary<OperationUnit, int> {
                        {  OperationUnit.AccountServices, stats.UnitProcesses.ProposedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.ProposedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.ProposedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.ProposedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.ProposedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.ProposedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.ProposedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.ProposedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Cancelled,
                    Categories = new Dictionary<OperationUnit, int> {
                        { OperationUnit.AccountServices,stats.UnitProcesses.CompletedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.CompletedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.CompletedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.CompletedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.CompletedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.CompletedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.CompletedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.CompletedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.UnitTotal,
                    Categories = new Dictionary<OperationUnit, int> {
                        {  OperationUnit.AccountServices, stats.UnitProcesses.ProposedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.ProposedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.ProposedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.ProposedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.ProposedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.ProposedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.ProposedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.ProposedProcesses.ReconciliationProcesses }
                    }
                }

            };

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

            var unitStatistics = await _processesService.UnitCountAsync(currentUser.UserId, currentUser.IPAddress, unit);
            return new OperationsDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                UnitStatistics = unitStatistics
            };
        }

        public async Task<TotalExtensionModel> PrepareDefaultTotalExtensionsModelAsync(UserModel currentUser) {

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

            var charts = await _processesService.TotalExtensionsCountAsync(currentUser.UserId, currentUser.IPAddress);
            return new TotalExtensionModel
            {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - Processes Categories Per Unit",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                Charts = charts
            };
        }

        public async Task<CategoryExtensionModel> PrepareCategoryExtensionsModelAsync(UserModel currentUser, string category)
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

            CategoryExtensionModel record = await _processesService.CategoryExtensionsCountAsync(category, currentUser.UserId, currentUser.IPAddress);
            if (record != null) {
                record.WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - {category} Processes breakdown";
                record.Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}";
                record.QuickActions = quickActions;
                record.Workspace = _sessionManager.GetWorkspace();
            } else
            {
                record = new()
                {
                    WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - {category} Processes breakdown",
                    Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                    QuickActions = quickActions,
                    Workspace = _sessionManager.GetWorkspace(),
                    CategoryProcesses = new()
                };
            }

            return record;
        }

        public async Task<CategoryExtensionModel> PrepareDefaultExtensionCategoryErrorModelAsync(UserModel currentUser)
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
            //var stats = await _processesService.StatisticAsync(currentUser.UserId, currentUser.LastLoginIpAddress);
            var model = new CategoryExtensionModel
            {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                CategoryProcesses = new(),
                //..set workspace into session
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
            var stats = await _processesService.StatisticAsync(currentUser.UserId, currentUser.IPAddress);
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

        public async Task<OperationsDashboardModel> PrepareErrorOperationsDashboardModelAsync(UserModel currentUser)
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

            //..get dashboard statistics
            var stats = await _processesService.StatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new OperationsDashboardModel
            {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}  - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                //..set workspace into session
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats,
                //..create dashboard cards
                ChartViewModel = new DashboardChartViewModel()
            };
            return model;
        }

        public async Task<UnitExtensionModel> PrepareUnitExtensionsModelAsync(UserModel currentUser, string unit)
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

            UnitExtensionModel record = await _processesService.UnitExtensionsCountAsync(unit, currentUser.UserId, currentUser.IPAddress);
            if (record != null)
            {
                record.WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - {unit} Processes breakdown";
                record.Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}";
                record.QuickActions = quickActions;
                record.Workspace = _sessionManager.GetWorkspace();
            }
            else
            {
                record = new()
                {
                    WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName} - {unit} Processes breakdown",
                    Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                    QuickActions = quickActions,
                    Workspace = _sessionManager.GetWorkspace(),
                    UnitProcesses = new()
                };
            }

            return record;
        }

        public async Task<UnitExtensionModel> PrepareDefaultExtensionUnitErrorModelAsync(UserModel currentUser)
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
            //var stats = await _processesService.StatisticAsync(currentUser.UserId, currentUser.LastLoginIpAddress);
            var model = new UnitExtensionModel
            {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}  - Operations Processes",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                UnitProcesses = new(),
                //..set workspace into session
                Workspace = _sessionManager.GetWorkspace(),
            };
            return model;
        }
    }
}

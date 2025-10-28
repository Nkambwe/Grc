using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Factories {

    public class SupportDashboardFactory : ISupportDashboardFactory {

        private readonly ILocalizationService _localizationService;
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
                                       ILocalizationService localizationService,
                                       IMapper mapper,
                                       SessionManager session,
                                       IConfiguration configuration) {  
            _pinnedService = pinnedService;
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
            _configuration = configuration;
            _accessService = accessService;
            _localizationService = localizationService;
        }

        public async Task<AdminDashboardModel> PrepareAdminDashboardModelAsync(UserModel currentUser) {

            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
            var quickActions = new List<QuickActionModel>();
            if(!quicksData.HasError){ 
                var quickies = quicksData.Data;
                if(quickies.Count > 0){ 
                    foreach(var action in quickies){ 
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
            var stats = (await _accessService.StatisticAsync(currentUser.UserId, currentUser.IPAddress));

            //..populate dashboard data
            AdminDashboardChartViewModel statistics = new() {
                Users = new() {
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.SystemUsers"),
                        Value = stats.TotalUsers,
                        CssClass = "stat-separator-default",
                        Controller = "Support",
                        Action = "Users"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.ActiveUsers"),
                        Value = stats.ActiveUsers,
                        CssClass = "stat-separator-primary",
                        Controller = "Support",
                        Action = "ActiveUsers"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.DisabledUsers"),
                        Value = stats.DeactivatedUsers,
                        CssClass = "stat-separator-danger",
                        Controller = "Support",
                        Action = "LockedUsers"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.PendingApproval"),
                        Value = stats.UnApprovedUsers,
                        CssClass = "stat-separator-nuetral",
                        Controller = "Support",
                        Action = "UnapprovedUsers"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.PendingVerification"),
                        Value = stats.UnverifiedUsers,
                        CssClass = "stat-separator-primary",
                        Controller = "Support",
                        Action = "UnverifiedUser"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.DeletedAccounts"),
                        Value = stats.DeletedUsers,
                        CssClass = "stat-separator-danger",
                        Controller = "Support",
                        Action = "DeletedUsers"
                    }
                },
                Bugs = new()
                {
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.TotalBugs"),
                        Value = stats.TotalBugs,
                        CssClass = "stat-separator-colored-pearl-orange",
                        Controller = "Support",
                        Action = "Bugs"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.NewBugs"),
                        Value = stats.NewBugs,
                        CssClass = "stat-separator-colored-pearl-orange",
                        Controller = "Support",
                        Action = "NewBugs"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.FixingBugs"),
                        Value = stats.BugProgress,
                        CssClass = "stat-separator-colored-pearl-orange",
                        Controller = "Support",
                        Action = "BugProgress"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.Fixed"),
                        Value = stats.BugFixes,
                        CssClass = "stat-separator-colored-pearl-orange",
                        Controller = "Support",
                        Action = "bugFixing"
                    },
                    new(){
                        Title = _localizationService.GetLocalizedLabel("App.Admin.Dashboard.Labels.UserReported"),
                        Value = stats.UserReportedBugs,
                        CssClass = "stat-separator-colored-pearl-orange",
                        Controller = "Support",
                        Action = "UserReportedBugs"
                    }
                }
            };

            //..generate dashboard model
            return new AdminDashboardModel {
                MiddlwareUrl = baseUrl,
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}!",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                LastLogin = DateTime.UtcNow,
                Workspace = _sessionManager.GetWorkspace(),
                Statistics = statistics
            };
        }

        public async Task<AdminDashboardModel> PrepareDefaultModelAsync(UserModel currentUser) {
            // Get recents from session
            var recents = _sessionManager.Get<List<RecentModel>>(SessionKeys.RecentItems.GetDescription()) ?? new List<RecentModel>();

            //..get pinned items
            var pinData = await _pinnedService.GetPinnedItemAsync(currentUser.UserId, currentUser.IPAddress);
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

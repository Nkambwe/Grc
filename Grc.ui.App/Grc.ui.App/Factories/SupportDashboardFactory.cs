using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Factories {

    public class SupportDashboardFactory : ISupportDashboardFactory {

        private readonly ILocalizationService _localizationService;
        private readonly ISystemAccessService _accessService;
        private readonly IProcessesService _processService;
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
                                       IProcessesService processService,
                                       IConfiguration configuration) {  
                _pinnedService = pinnedService;
                _quickActionService = quickActionService;
                _mapper = mapper;
                _sessionManager = session;
                _configuration = configuration;
                _accessService = accessService;
                _processService = processService;
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

        public async Task<UserSupportViewModel> PrepareUserSupportModelAsync(UserModel currentUser) {
            // Get recents from session
            var recents = _sessionManager.Get<List<RecentModel>>(SessionKeys.RecentItems.GetDescription()) ?? new List<RecentModel>();

            //..get pinned items
            var response = await _accessService.GetUserSupportAsync(currentUser.UserId, currentUser.IPAddress);
            GrcUserSupportResponse data;
            if (response.HasError) {
                data = new GrcUserSupportResponse() {
                    Branches = new(),
                    Roles = new(),
                    Departments = new()
                };
            } else {
                data = response.Data;
            }

            return new UserSupportViewModel() {
                Branches = data.Branches.Any()?
                           data.Branches.Select(b => new BranchItemViewModel(){ 
                               Id = b.Id,
                               SolId = b.SolId,
                               Name = b.Name}
                           ).ToList(): new(),
                Roles = data.Roles.Any()?
                        data.Roles.Select(b => new RoleItemViewModel(){ 
                               Id = b.Id,
                               Name = b.Name}).ToList(): new(),
                Departments = data.Departments.Any()?
                              data.Departments.Select(b => new DepartmentItemViewModel(){ 
                               Id = b.Id,
                               Name = b.Name}).ToList(): new(),
            };
        }

        public async Task<RoleGroupListModel> PrepareRoleGroupListModelAsync(UserModel currentUser) {
            RoleGroupListModel roleGroupModel = new();
            GrcRequest request = new()
            {
                UserId = currentUser.UserId,
                Action = Activity.RETRIVEROLEGROUPS.GetDescription(),
                IPAddress = currentUser.IPAddress,
                EncryptFields = Array.Empty<string>(),
                DecryptFields = Array.Empty<string>()
            };
            //..get list of all role groups
            var rolesData = await _accessService.GetRoleGroupsAsync(request);
            List<GrcRoleGroupResponse> roleGroups;

            List<RoleGroupItemResponse> items = new();
            if (!rolesData.HasError) {
                roleGroups = rolesData.Data.Data;
            } else {
                roleGroups = new();
            }

            if(roleGroups.Any()){ 
                foreach(var roleGroup in roleGroups){
                    items.Add(new RoleGroupItemResponse
                    {
                        Id = roleGroup.Id,
                        GroupName = roleGroup.GroupName
                    });
                }

                roleGroupModel.RoleGroups = items;
            }

            return roleGroupModel;
        }

        public async Task<OperationProcessViewModel> PrepareProcessViewModelAsync(UserModel currentUser) {
            OperationProcessViewModel processModel = new()
            {
                ProcessStatuses = new List<ProcessStatusViewModel> {
                    new() { Id=(int)ProcessCategories.Draft, StatusName = ProcessCategories.Draft.GetDescription()},
                    new() { Id=(int)ProcessCategories.UpToDate, StatusName = ProcessCategories.UpToDate.GetDescription()},
                    new() { Id=(int)ProcessCategories.Unchanged, StatusName = ProcessCategories.Unchanged.GetDescription()},
                    new() { Id=(int)ProcessCategories.Proposed, StatusName = ProcessCategories.Proposed.GetDescription()},
                    new() { Id=(int)ProcessCategories.Due, StatusName = ProcessCategories.Due.GetDescription()},
                    new() { Id=(int)ProcessCategories.Dormant, StatusName = ProcessCategories.Dormant.GetDescription()},
                    new() { Id=(int)ProcessCategories.Cancelled, StatusName = ProcessCategories.Cancelled.GetDescription()},
                    new() { Id=(int)ProcessCategories.OnHold, StatusName = ProcessCategories.OnHold.GetDescription()}
                },
                ProcessTypes = new List<ProcessTypeViewModel>(),
                Units = new List<UnitViewModel>(),
                Responsibilities = new List<ResponsibilityViewModel>()
            };

            GrcRequest request = new()
            {
                UserId = currentUser.UserId,
                IPAddress = currentUser.IPAddress,
                Action = Activity.RETRIVEPROCESSUPPORTITEMS.GetDescription(),
                EncryptFields = Array.Empty<string>(),
                DecryptFields = Array.Empty<string>()
            };

            //..get lists of process support items  
            var response = await _processService.GetProcessSupportItemsAsync(request);
            if (response.HasError) {
                return processModel;
                
            }
            
            var supportItems = response.Data;
            if(supportItems == null) {
                return processModel;
            }

            //..get process types
            if (supportItems.ProcessTypes != null && supportItems.ProcessTypes.Any())  {
                processModel.ProcessTypes.AddRange(
                    from type in supportItems.ProcessTypes
                    select new ProcessTypeViewModel
                    {
                        Id = type.Id,
                        TypeName = type.TypeName
                    }
                );
            }


            //..get process units
            if (supportItems.Units != null && supportItems.Units.Any())
            {
                processModel.Units.AddRange(
                    from unit in supportItems.Units
                    select new UnitViewModel
                    {
                        Id = unit.Id,
                        UnitName = unit.UnitName
                    }
                );
            }

            //..get responsibilities
            if (supportItems.Responsibilities != null && supportItems.Responsibilities.Any())
            {
                processModel.Responsibilities.AddRange(
                    from owner in supportItems.Responsibilities
                    select new ResponsibilityViewModel
                    {
                        Id = owner.Id,
                        DepartmentName = owner.DepartmentName,
                        ResponsibleRole = owner.ResponsibilityRole
                    }
                );
            }

            return processModel;
        }

    }
}

using AutoMapper;
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
    public class DashboardFactory : IDashboardFactory {
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly SessionManager _sessionManager;
        private readonly IReturnsService _returnsService;
        private readonly IPolicyService _policyService;
        public DashboardFactory(IMapper mapper,
                                IQuickActionService quickActionService,
                                IReturnsService returnsService,
                                IPolicyService policyService,
                                SessionManager session) {  
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
            _returnsService = returnsService;
            _policyService = policyService;
        }

        public async Task<UserDashboardModel>  PrepareUserDashboardModelAsync(UserModel currentUser) {
            //..get quick items
            var quicksData = await _quickActionService.GetQuickActionsync(currentUser.UserId, currentUser.IPAddress);
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

            var grcResponse = await _returnsService.GetAllReturnsStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            ComplianceGeneralStatistic stats = new();
            if (grcResponse.HasError) {
                stats.Circulars = new();
                stats.Returns = new();
                stats.Tasks = new();
            } else {
                var data = grcResponse.Data;
                stats.Circulars = data.CircularStatuses;
                stats.Returns = data.ReturnStatuses;
                stats.Tasks = data.TaskStatuses;
                stats.Policies = data.Policies;
            }

            var model = new UserDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                Statistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<UserDashboardModel> PrepareUserModelAsync(UserModel currentUser) {
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

            var model = new UserDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Statistics = null
            };

            return await Task.FromResult(model);
        }

        public async Task<PolicyRegisterViewModel> PrepareReturnSupportItemsModelAsync(UserModel currentUser) {
            PolicyRegisterViewModel policyModel = new() {
                Authorities = new(),
                Responsibilities = new(),
                Departments = new(),
                Frequencies = new(),
                RegulatoryTypes = new()
            };

            GrcRequest request = new() {
                UserId = currentUser.UserId,
                IPAddress = currentUser.IPAddress,
                Action = Activity.RETRIEVEPOLICYSUPPORT.GetDescription(),
                EncryptFields = Array.Empty<string>(),
                DecryptFields = Array.Empty<string>()
            };

            //..get lists of process support items  
            var response = await _policyService.GetPolicySupportItemsAsync(request);
            if (response.HasError) {
                return policyModel;
            }

            var supportItems = response.Data;
            if (supportItems == null) {
                return policyModel;
            }

            //..get departments
            if (supportItems.Departments != null && supportItems.Departments.Any()) {
                policyModel.Departments.AddRange(
                    from department in supportItems.Departments
                    select new DepartmentViewModel {
                        Id = department.Id,
                        DepartmentName = department.DepartmentName
                    }
                );
            }

            //..get responsibilities
            if (supportItems.Responsibilities != null && supportItems.Responsibilities.Any()) {
                policyModel.Responsibilities.AddRange(
                    from owner in supportItems.Responsibilities
                    select new ResponsibilityViewModel {
                        Id = owner.Id,
                        DepartmentName = owner.DepartmentName,
                        ResponsibleRole = owner.ResponsibilityRole
                    }
                );
            }

            //..get frequency
            if (supportItems.Frequencies != null && supportItems.Frequencies.Any()) {
                policyModel.Frequencies.AddRange(
                    from frequency in supportItems.Frequencies
                    select new FrequencyViewModel {
                        Id = frequency.Id,
                        FrequencyName = frequency.FrequencyName
                    }
                );
            }

            //..get authorities
            if (supportItems.Authorities != null && supportItems.Authorities.Any()) {
                policyModel.Authorities.AddRange(
                    from authority in supportItems.Authorities
                    select new AuthorityViewModel {
                        Id = authority.Id,
                        AuthorityName = authority.AuthorityName
                    }
                );
            }

            //..get regulatory types
            if (supportItems.RegulatoryTypes != null && supportItems.RegulatoryTypes.Any()) {
                policyModel.RegulatoryTypes.AddRange(
                    from type in supportItems.RegulatoryTypes
                    select new RegulatoryTypeViewModel {
                        Id = type.Id,
                        TypeName = type.TypeName
                    }
                );
            }

            return policyModel;
        }

        public async Task<PolicyDashboardModel> PreparePolicyMinModelAsync(UserModel currentUser, string status) {
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

            var grcResponse = await _returnsService.GetPolicyCountAsync(currentUser.UserId, currentUser.IPAddress, status);
            PolicyDashboardStatistic policyData = new();
            if (grcResponse.HasError) {
                policyData.Policies = new();
                policyData.Statistics = new();
            } else {
                var data = grcResponse.Data;
                policyData.Policies = data.Policies.Select(policy => new PolicyDocument {
                    Id = policy.Id,
                    Title = policy.Title,
                    OwnerId = policy.OwnerId,
                    Department = policy.Department,
                    ReviewDate = policy.ReviewDate
                }).ToList();
                policyData.Statistics = data.Statistics;
            }

            var model = new PolicyDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                PolicyData = policyData
            };

            return await Task.FromResult(model);

        }

        #region Circulars
        public async Task<CircularDashboardModel> PrepareCircularDashboardModelAsync(UserModel currentUser) {
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

            var grcResponse = await _returnsService.GetCircularStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            CircularDashboardResponses circulars = new();
            if (grcResponse.HasError) {
                circulars.Authorities = new();
                circulars.Statuses = new();
            } else {
                var data = grcResponse.Data;
                circulars.Authorities = data.Authorities;
                circulars.Statuses = data.Statuses;
            }
            var model = new CircularDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Circulars = new() {
                    Authorities = circulars.Authorities,
                    Statuses = circulars.Statuses
                }
            };

            return await Task.FromResult(model);

        }

        public async Task<CircularMiniStatisticViewModel> PrepareCircularAuthorityDashboardModelAsync(UserModel currentUser, string authority) {
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

            var grcResponse = await _returnsService.GetAuthorityCircularCountAsync(currentUser.UserId, currentUser.IPAddress, authority);
            CircularDashboardStatistic circulars = new();
            if (grcResponse.HasError) {
                circulars.Statistics = new();
                circulars.Reports = new();
            } else {
                var data = grcResponse.Data;
                circulars.Statistics = data.Statistics;
                circulars.Reports = data.Circulars!= null && data.Circulars.Any() ?
                    data.Circulars.Select(circular => new CircularReport() { 
                        Id = circular.Id,
                        Title = circular.Title,
                        Status = circular.Status,
                        Authority = circular.Authority,
                        AuthorityAlias = circular.AuthorityAlias,
                        Department = circular.Department
                    }).ToList(): 
                    new List<CircularReport>();
            }
            var model = new CircularMiniStatisticViewModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Circulars = circulars
            };

            return await Task.FromResult(model);

        }

        #endregion

        #region Returns
        
        public async Task<ComplianceGeneralStatisticViewModel> PrepareGeneralReturnsDashboardModelAsync(UserModel currentUser) {
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

            var grcResponse = await _returnsService.GetAllReturnsStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            ComplianceGeneralStatistic stats = new();
            if (grcResponse.HasError) {
                stats.Circulars = new();
                stats.Returns = new();
                stats.Tasks = new();
            } else {
                var data = grcResponse.Data;
                stats.Circulars = data.CircularStatuses;
                stats.Returns = data.ReturnStatuses;
                stats.Tasks = data.TaskStatuses;
                stats.Policies = data.Policies;
            }
   
            var model = new ComplianceGeneralStatisticViewModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Statistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<ComplianceReturnStatisticViewModel> PrepareReturnsDashboardModelAsync(UserModel currentUser) {
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

            var grcResponse = await _returnsService.GetReturnStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            ComplianceReturnStatistic returns = new();
            if (grcResponse.HasError) {
                returns.Periods = new();
                returns.Statuses = new();
            } else {
                var data = grcResponse.Data;
                returns.Periods = data.Periods;
                returns.Statuses = data.Statuses;
            }

            var model = new ComplianceReturnStatisticViewModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Statistics = new() {
                    Periods = returns.Periods,
                    Statuses = returns.Statuses   
                }
            };

            return await Task.FromResult(model);
        }

        public async Task<ReturnMiniStatisticViewModel> PrepareReturnPeriodDashboardModelAsync(UserModel currentUser, string period) {
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

            var grcResponse = await _returnsService.GrcReturnDashboardResponseAsync(currentUser.UserId, currentUser.IPAddress, period);
            ReturnDashboardStatistic returns = new();
            if (grcResponse.HasError) {
                returns.Statistics = new();
                returns.Reports = new();
            } else {
                var data = grcResponse.Data;
                returns.Statistics = data.Statistics;
                returns.Reports = data.Reports.Select(report => new ReturnReport() {
                    Id = report.Id,
                    Department = report.Department,
                    Title = report.Title,
                    Type = report.Type
                }).ToList();
            }

            var model = new ReturnMiniStatisticViewModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                Returns = returns
            };

            return await Task.FromResult(model);
        }

        #endregion

        #region Tasks
        public async Task<TaskDashboardModel> PrepareTasksDashboardModelAsync(UserModel currentUser) {
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

            var grcResponse = await _returnsService.GetTaskStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            TaskDashboardResponses task = new();
            if (grcResponse.HasError) {
                task.Total = new();
                task.Open = new();
                task.Closed = new();
                task.Breached = new();
            } else {
                var data = grcResponse.Data;
                task.Total = data.Totals;
                task.Open = data.Open;
                task.Closed = data.Closed;
                task.Breached = data.Breached;
            }

            var model = new TaskDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Tasks = new() {
                    Total = task.Total,
                    Open = task.Open,
                    Closed = task.Closed,
                    Breached = task.Breached
                }
            };

            return await Task.FromResult(model);
        }

        public async Task<TaskMinDashboardModel> PrepareMinTaskDashboardStatisticModelAsync(UserModel currentUser, string status) {
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

            var grcResponse = await _returnsService.GetMiniTaskStatisticAsync(currentUser.UserId, currentUser.IPAddress, status);
            TaskMinDashboardModel task = new();
            if (grcResponse.HasError) {
                task.Tasks = new();
            } else {
                var data = grcResponse.Data;
                task.Tasks = data.Tasks;
            }

            var model = new TaskMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Tasks = task.Tasks
            };

            return await Task.FromResult(model);
        }

        #endregion
    }
}

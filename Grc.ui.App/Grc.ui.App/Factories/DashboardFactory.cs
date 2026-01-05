using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Factories {
    public class DashboardFactory : IDashboardFactory {
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly SessionManager _sessionManager;
        private readonly IRegistersService _registersService;
        private readonly IReturnsService _returnsService;
        private readonly IPolicyService _policyService;
        public DashboardFactory(IMapper mapper,
                                IQuickActionService quickActionService,
                                IRegistersService registersService,
                                IReturnsService returnsService,
                                IPolicyService policyService,
                                SessionManager session) {  
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
            _returnsService = returnsService;
            _policyService = policyService;
            _registersService = registersService;
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

            var stats = await _registersService.StatisticAsync(currentUser.UserId, currentUser.IPAddress);
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
                DashboardStatistics = null
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

            var stats = await _returnsService.GetCircularStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new CircularDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);

        }

        public async Task<CircularMinDashboardModel> PrepareCircularBreachStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetBreachCircularStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new CircularMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<CircularMinDashboardModel> PrepareCircularClosedStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetClosedCircularStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new CircularMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<CircularMinDashboardModel> PrepareCircularOpenStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetOpenCircularStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new CircularMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<CircularMinDashboardModel> PrepareCircularReceivedStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetReceivedCircularStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new CircularMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        #endregion

        #region Returns

        public async Task<ReturnsDashboardModel> PrepareReturnsDashboardModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetReturnStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new ReturnsDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<ReturnsMinDashboardModel> PrepareReturnReceivedStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetReturnReceivedStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new ReturnsMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<ReturnsMinDashboardModel> PrepareReturnTotalStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetReturnTotalStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new ReturnsMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<ReturnsMinDashboardModel> PrepareReturnOpenStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetReturnOpenStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new ReturnsMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<ReturnsMinDashboardModel> PrepareReturnSubmittedStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetReturnSubmittedStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new ReturnsMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<ReturnsMinDashboardModel> PrepareReturnBreachStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetReturnBreachStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new ReturnsMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
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

            var stats = await _returnsService.GetTaskStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new TaskDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<TaskMinDashboardModel> PrepareTotalTaskStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetTotalTaskStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new TaskMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<TaskMinDashboardModel> PrepareOpenTaskStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetOpenTaskStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new TaskMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<TaskMinDashboardModel> PrepareClosedTaskStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetClosedTaskStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new TaskMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<TaskMinDashboardModel> PrepareFailedTaskStatisticModelAsync(UserModel currentUser) {
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

            var stats = await _returnsService.GetFailedTaskStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            var model = new TaskMinDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                DashboardStatistics = stats
            };

            return await Task.FromResult(model);
        }

        #endregion
    }
}

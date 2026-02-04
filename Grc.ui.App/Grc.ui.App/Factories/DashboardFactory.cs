using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure.AppSettings;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using NuGet.Configuration;

namespace Grc.ui.App.Factories {
    public class DashboardFactory : IDashboardFactory {
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly SessionManager _sessionManager;
        private readonly IReturnsService _returnsService;
        private readonly IPolicyService _policyService;
        private readonly IAuditService _auditService;
        public DashboardFactory(IMapper mapper,
                                IQuickActionService quickActionService,
                                IReturnsService returnsService,
                                IPolicyService policyService,
                                IAuditService auditService,
                                SessionManager session) {  
            _quickActionService = quickActionService;
            _mapper = mapper;
            _sessionManager = session;
            _returnsService = returnsService;
            _policyService = policyService;
            _auditService = auditService;
        }

        public async Task<UserDashboardModel>  PrepareUserDashboardModelAsync(UserModel currentUser) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

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
            var quickActions = GetComplianceQuickActions();

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
                EnforcementLaws= new(),
                Departments = new(),
                Frequencies = new(),
                RegulatoryTypes = new(),
                ReturnTypes = new()
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

            //..get enforcement laws
            if (supportItems.EnforcementLaws != null && supportItems.EnforcementLaws.Any()) {
                policyModel.EnforcementLaws.AddRange(
                    from type in supportItems.EnforcementLaws
                    select new StatuteMinViewModel {
                        Id = type.Id,
                        Section = type.Section,
                        Requirement = type.Requirement
                    }
                );
            }

            //..get return types
            if (supportItems.ReturnTypes != null && supportItems.ReturnTypes.Any()) {
                policyModel.ReturnTypes.AddRange(
                    from type in supportItems.ReturnTypes
                    select new ReturnTypeViewModel {
                        Id = type.Id,
                        TypeName = type.TypeName
                    }
                );
            }

            return policyModel;
        }

        public async Task<PolicyDashboardModel> PreparePolicyMinModelAsync(UserModel currentUser, string status) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

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

        public async Task<ComplianceGeneralStatisticViewModel> PrepareGeneralReturnsDashboardModelAsync(UserModel currentUser) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

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

        #region Audits
        public async Task<AuditDashboardViewModel> PrepareAuditDashboardModelAsync(UserModel currentUser) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

            var grcResponse = await _auditService.GetAuditStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            AuditStatistic stats = new();
            if (grcResponse.HasError) {
                stats.Findings = new();
                stats.Completions = new();
                stats.BarChart = new();
            } else {
                var data = grcResponse.Data;
                stats.Findings = data.Findings;
                stats.Completions = data.Completions;
                stats.BarChart = data.BarChart;
            }

            var model = new AuditDashboardViewModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                Statistics = stats
            };

            return await Task.FromResult(model);
        }

        public async Task<AuditExtensionDashboardModel> PrepareAuditExtensionDashboardModelAsync(UserModel currentUser, string period) {

            var grcResponse = await _auditService.GetAuditExtensionStatisticAsync(currentUser.UserId, currentUser.IPAddress, period);
            AuditExtensionStatistics stats = new();
            if (grcResponse.HasError) {
                stats.Statuses = new();
                stats.Reports = new();
            } else {
                var data = grcResponse.Data;
                stats.Statuses = data.Statuses;
                stats.Reports = data.Reports;
            }

            var model = new AuditExtensionDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = new(),
                Workspace = _sessionManager.GetWorkspace(),
                Statistics = stats
            };

            return await Task.FromResult(model);
        }

        #endregion

        #region Circulars

        public async Task<CircularDashboardModel> PrepareCircularDashboardModelAsync(UserModel currentUser) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

            var grcResponse = await _returnsService.GetCircularStatisticAsync(currentUser.UserId, currentUser.IPAddress);
            CircularDashboardResponses circulars = new();
            if (grcResponse.HasError) {
                circulars.Authorities = new();
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

        public async Task<CircularExtensionDashboardModel> PrepareCircularExtensionDashboardModelAsync(UserModel currentUser, string authority) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

            var grcResponse = await _returnsService.GetCircularExtensionStatisticAsync(currentUser.UserId, currentUser.IPAddress, authority);
            CircularExtensionDashboardResponses circulars = new();
            if (grcResponse.HasError) {
                circulars.Reports = new();
                circulars.Statuses = new();
            } else {
                var data = grcResponse.Data;
                circulars.Reports = data.Reports;
                circulars.Statuses = data.Statuses;
            }

            var model = new CircularExtensionDashboardModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Circulars = new() {
                    Statuses = circulars.Statuses,
                    Reports = circulars.Reports != null && circulars.Reports.Any() ?
                              circulars.Reports.Select(report => new CircularReport() {
                                  Id= report.Id,
                                  Title = report.Title ?? string.Empty,
                                  Status = report.Status ?? string.Empty,
                                  RequiredDate = report.DueDate.HasValue ? report.DueDate.Value.ToString("MMM dd") : string.Empty,
                                  BreachRisk = report.BreachRisk ?? string.Empty,
                                  Authority = report.Authority ?? string.Empty,
                                  AuthorityAlias = report.AuthorityAlias ?? string.Empty,
                                  Department = report.Department ?? string.Empty

                              }).ToList() :
                              new List<CircularReport>()
                }
            };

            return await Task.FromResult(model);

        }

        public async Task<CircularMiniStatisticViewModel> PrepareCircularAuthorityDashboardModelAsync(UserModel currentUser, string authority) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

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
                        Title = circular.Title ?? string.Empty,
                        Status = circular.Status ?? string.Empty,
                        BreachRisk = circular.BreachRisk ?? string.Empty,
                        Authority = circular.Authority ?? string.Empty,
                        AuthorityAlias = circular.AuthorityAlias ?? string.Empty,
                        Department = circular.Department ?? string.Empty
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
        
        public async Task<ComplianceReturnStatisticViewModel> PrepareReturnsDashboardModelAsync(UserModel currentUser) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

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
                Returns = new() {
                    Periods = returns.Periods,
                    Statuses = returns.Statuses   
                }
            };

            return await Task.FromResult(model);
        }

        public async Task<ComplianceExtensionReturnStatisticViewModel> PrepareReturnExtensionDashboardModelAsync(UserModel currentUser, string period) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

            var grcResponse = await _returnsService.GetReturnExtensionStatisticAsync(currentUser.UserId, currentUser.IPAddress, period);
            ComplianceExtensionReturnStatistic returns = new();
            if (grcResponse.HasError) {
                returns.Reports = new();
                returns.Periods = new();
            } else {
                var data = grcResponse.Data;
                returns.Reports = data.Reports.Select(report => new ReturnSubmission() {
                    Id = report.Id,
                    Title = report.Title ?? string.Empty,
                    Period =report.RequiredSubmissionDate.HasValue ? report.RequiredSubmissionDate.Value.ToString("MMM dd"): "",
                    Status = report.Status ?? string.Empty,
                    Department = report.Department ?? string.Empty,
                    Risk = report.Risk ?? string.Empty,
                }).ToList();
                returns.Periods = data.Periods;
            }

            var model = new ComplianceExtensionReturnStatisticViewModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                QuickActions = quickActions,
                Workspace = _sessionManager.GetWorkspace(),
                //..statistics
                Returns = new() {
                    Periods = returns.Periods,
                    Reports = returns.Reports
                }
            };

            return await Task.FromResult(model);
        }

        public async Task<ReturnMiniStatisticViewModel> PrepareReturnPeriodDashboardModelAsync(UserModel currentUser, string period) {
            //..get quick items
            var quickActions = GetComplianceQuickActions();

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
                    Department = report.Department ?? string.Empty,
                    Title = report.Title ?? string.Empty,
                    Type = report.Type ?? string.Empty
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

        #region Private Methods
        private static string GetReportDate(DateTime date) {
            var today = DateTime.Today;

            string report;
            if (date == today) {
                report = "TODAY";
            } else if (date == today.AddDays(-1)) {
                report = "YESTERDAY";
            } else {
                report = date.ToString("yyyy-MM-dd");
            }
            return report;

        }

        private static List<QuickActionModel> GetComplianceQuickActions() {
            return new List<QuickActionModel>() {
                new() {
                    Label = "App.Menu.Home",
                    IconClass = "mdi mdi-home-outline",
                    Controller = "Application",
                    Action = "Dashboard",
                    Area = "",
                    CssClass = "is-primary"
                },
                new() {
                    Label = "App.Menu.Registers.Policies",
                    IconClass = "mdi mdi-file-replace-outline",
                    Controller = "CompliancePolicy",
                    Action = "PoliciesRegisters",
                    Area = "",
                    CssClass = "is-primary"
                },
                new() {
                    Label = "App.Menu.Circulars",
                    IconClass = "mdi mdi-file-rotate-right-outline",
                    Controller = "ComplianceReturn",
                    Action = "CircularHome",
                    Area = "",
                    CssClass = "is-primary"
                },
                new() {
                    Label = "App.Menu.Returns",
                    IconClass = "mdi mdi-file-table-box-multiple-outline",
                    Controller = "ComplianceReturn",
                    Action = "ReturnsHome",
                    Area = "",
                    CssClass = "is-primary"
                },
                new() {
                    Label = "App.Menu.Compliance.Audits.AuditExceptions",
                    IconClass = "mdi mdi-marker-check",
                    Controller = "ComplianceAudit",
                    Action = "AuditDashboard",
                    Area = "",
                    CssClass = "is-primary"
                }
            };
        }

        #endregion

    }
}

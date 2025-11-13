using AutoMapper;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Diagnostics;
using System.Text.Json;
using Activity = Grc.ui.App.Enums.Activity;

namespace Grc.ui.App.Services {

    public class ProcessesService : GrcBaseService, IProcessesService {

        public ProcessesService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, IMapper mapper, 
            IWebHelper webHelper, SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType, 
                  mapper, webHelper, sessionManager, errorFactory, errorService) {
        }

        #region Statistics
        public async Task<OperationsUnitCountResponse> StatisticAsync(long userId, string ipAddress) {

            var stats = new OperationsUnitCountResponse()
            {
                UnitProcesses = new OperationsUnitStatisticsResponse
                {
                    TotalUnitProcess = new()
                    {
                        CashProcesses = 36,
                        AccountServiceProcesses = 23,
                        ChannelProcesses = 33,
                        PaymentProcesses = 17,
                        WalletProcesses = 18,
                        RecordsManagementProcesses = 3,
                        CustomerExperienceProcesses = 17,
                        ReconciliationProcesses = 12,
                        TotalProcesses = 159
                    },
                    CompletedProcesses = new UnitCountResponse()
                    {
                        AccountServiceProcesses = 9,
                        CashProcesses = 3,
                        ChannelProcesses = 9,
                        CustomerExperienceProcesses = 12,
                        ReconciliationProcesses = 4,
                        RecordsManagementProcesses = 3,
                        PaymentProcesses = 9,
                        WalletProcesses = 15,
                        TotalProcesses = 62

                    },
                    ProposedProcesses = new UnitCountResponse()
                    {
                        AccountServiceProcesses = 0,
                        CashProcesses = 0,
                        ChannelProcesses = 3,
                        CustomerExperienceProcesses = 0,
                        ReconciliationProcesses = 0,
                        RecordsManagementProcesses = 0,
                        PaymentProcesses = 5,
                        WalletProcesses = 2,
                        TotalProcesses = 10
                    },
                    UnchangedProcesses = new UnitCountResponse()
                    {
                        AccountServiceProcesses = 0,
                        CashProcesses = 9,
                        ChannelProcesses = 14,
                        CustomerExperienceProcesses = 2,
                        ReconciliationProcesses = 8,
                        RecordsManagementProcesses = 0,
                        PaymentProcesses = 6,
                        WalletProcesses = 0,
                        TotalProcesses = 30
                    },
                    ProcessesDueForReview = new UnitCountResponse()
                    {
                        AccountServiceProcesses = 0,
                        CashProcesses = 1,
                        ChannelProcesses = 7,
                        CustomerExperienceProcesses = 3,
                        ReconciliationProcesses = 0,
                        RecordsManagementProcesses = 0,
                        PaymentProcesses = 5,
                        WalletProcesses = 1,
                        TotalProcesses = 17
                    },
                    DormantProcesses = new UnitCountResponse()
                    {
                        AccountServiceProcesses = 0,
                        CashProcesses = 0,
                        ChannelProcesses = 1,
                        CustomerExperienceProcesses = 0,
                        ReconciliationProcesses = 0,
                        RecordsManagementProcesses = 0,
                        PaymentProcesses = 2,
                        WalletProcesses = 1,
                        TotalProcesses = 4

                    },
                    CancelledProcesses = new UnitCountResponse()
                    {
                        AccountServiceProcesses = 4,
                        CashProcesses = 0,
                        ChannelProcesses = 6,
                        CustomerExperienceProcesses = 0,
                        ReconciliationProcesses = 0,
                        RecordsManagementProcesses = 1,
                        PaymentProcesses = 0,
                        WalletProcesses = 0,
                        TotalProcesses = 11
                    }

                },

                ProcessCategories = new ProcessCategoryStatisticsResponse() {

                    CashProcesses = new CategoriesCountResponse() {
                        Unclassified = 0,
                        UpToDate = 9,
                        Unchanged = 6,
                        Proposed = 3,
                        Due = 0,
                        Dormant = 1,
                        Cancelled = 2,
                        Total = 21
                    },
                    AccountServiceProcesses = new CategoriesCountResponse() {
                        Unclassified = 0,
                        UpToDate = 9,
                        Unchanged = 0,
                        Proposed = 0,
                        Due = 0,
                        Dormant = 1,
                        Cancelled = 3,
                        Total = 13
                    },
                    RecordsMgtProcesses = new CategoriesCountResponse() {
                        Unclassified = 5,
                        UpToDate = 3,
                        Unchanged = 0,
                        Proposed = 0,
                        Due = 0,
                        Dormant = 1,
                        Cancelled = 0,
                        Total = 9
                    },
                    ChannelProcesses = new CategoriesCountResponse() {
                        Unclassified = 0,
                        UpToDate = 9,
                        Unchanged = 14,
                        Proposed = 7,
                        Due = 2,
                        Dormant = 5,
                        Cancelled = 12,
                        Total = 49
                    },
                    PaymentProcesses = new CategoriesCountResponse() {
                        Unclassified = 2,
                        UpToDate = 9,
                        Unchanged = 5,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 84
                    },
                    WalletProcesses = new CategoriesCountResponse() {
                        Unclassified = 3,
                        UpToDate = 15,
                        Unchanged = 2,
                        Proposed = 0,
                        Due = 1,
                        Dormant = 6,
                        Cancelled = 7,
                        Total = 35
                    },
                    CustomerExperienceProcesses = new CategoriesCountResponse() {
                        Unclassified = 2,
                        UpToDate = 12,
                        Unchanged = 2,
                        Proposed = 3,
                        Due = 3,
                        Dormant = 6,
                        Cancelled = 0,
                        Total = 28
                    },
                    ReconciliationProcesses = new CategoriesCountResponse() {
                        Unclassified = 5,
                        UpToDate = 4,
                        Unchanged = 0,
                        Proposed = 0,
                        Due = 0,
                        Dormant = 8,
                        Cancelled = 0,
                        Total = 12
                    },
                    TotalCategoryProcesses = new CategoriesCountResponse() {
                        Unclassified = 40,
                        UpToDate = 64,
                        Unchanged = 39,
                        Proposed = 10,
                        Due = 17,
                        Dormant = 2,
                        Cancelled = 4,
                        Total = 172
                    },
                }
            };

            return await Task.FromResult(stats);
        }

        public async Task<CategoriesCountResponse> UnitCountAsync(long userId, string ipAddress, string unit)
        {
            var categories = new CategoriesCountResponse()
            {
                Unclassified = 9,
                UpToDate = 5,
                Unchanged = 12,
                Proposed = 7,
                Due = 3,
                Dormant = 8,
                Cancelled = 15,
                Total = 59
            };

            return await Task.FromResult(categories);
        }

        public async Task<List<DashboardRecord>> TotalExtensionsCountAsync(long userId, string ipAddress)
        {
            return await Task.FromResult(new List<DashboardRecord>()
            {
                new(){
                    Banner = ProcessCategories.UpToDate.GetDescription(),
                    Categories = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),9 },
                        { OperationUnit.Cash.GetDescription(), 3 },
                        { OperationUnit.Channels.GetDescription(), 9 },
                        { OperationUnit.CustomerExp.GetDescription(), 12 },
                        { OperationUnit.Reconciliation.GetDescription(), 4 },
                        { OperationUnit.RecordsMgt.GetDescription(), 3 },
                        { OperationUnit.Payments.GetDescription(), 9 },
                        { OperationUnit.Wallets.GetDescription(), 15 },
                        { OperationUnit.CategoryTotal.GetDescription(), 62 }
                    }
                },
                new()
                {
                    Banner = ProcessCategories.Proposed.GetDescription(),
                    Categories = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),0 },
                        { OperationUnit.Cash.GetDescription(), 0 },
                        { OperationUnit.Channels.GetDescription(), 3 },
                        { OperationUnit.CustomerExp.GetDescription(), 0 },
                        { OperationUnit.Reconciliation.GetDescription(), 0 },
                        { OperationUnit.RecordsMgt.GetDescription(), 0 },
                        { OperationUnit.Payments.GetDescription(), 5 },
                        { OperationUnit.Wallets.GetDescription(), 2 },
                        { OperationUnit.CategoryTotal.GetDescription(), 10 }
                    }
                },
                new()
                {
                    Banner = ProcessCategories.Unchanged.GetDescription(),
                    Categories = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),0 },
                        { OperationUnit.Cash.GetDescription(), 9 },
                        { OperationUnit.Channels.GetDescription(), 14 },
                        { OperationUnit.CustomerExp.GetDescription(), 2 },
                        { OperationUnit.Reconciliation.GetDescription(), 8 },
                        { OperationUnit.RecordsMgt.GetDescription(), 0 },
                        { OperationUnit.Payments.GetDescription(), 6 },
                        { OperationUnit.Wallets.GetDescription(), 0 },
                        { OperationUnit.CategoryTotal.GetDescription(), 30 }
                    },
                },
                new()
                {
                    Banner = ProcessCategories.Due.GetDescription(),
                    Categories = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),0 },
                        { OperationUnit.Cash.GetDescription(), 1 },
                        { OperationUnit.Channels.GetDescription(), 7 },
                        { OperationUnit.CustomerExp.GetDescription(), 3 },
                        { OperationUnit.Reconciliation.GetDescription(), 0 },
                        { OperationUnit.RecordsMgt.GetDescription(), 0 },
                        { OperationUnit.Payments.GetDescription(), 5 },
                        { OperationUnit.Wallets.GetDescription(), 1 },
                        { OperationUnit.CategoryTotal.GetDescription(), 17 }
                    },
                },
                new()
                {
                    Banner = ProcessCategories.Dormant.GetDescription(),
                    Categories = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),0 },
                        { OperationUnit.Cash.GetDescription(), 0 },
                        { OperationUnit.Channels.GetDescription(), 1 },
                        { OperationUnit.CustomerExp.GetDescription(), 0 },
                        { OperationUnit.Reconciliation.GetDescription(), 0 },
                        { OperationUnit.RecordsMgt.GetDescription(), 0 },
                        { OperationUnit.Payments.GetDescription(), 2 },
                        { OperationUnit.Wallets.GetDescription(), 1 },
                        { OperationUnit.CategoryTotal.GetDescription(), 4 }
                    },
                },
                new()
                {
                    Banner = ProcessCategories.Cancelled.GetDescription(),
                    Categories = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),4 },
                        { OperationUnit.Cash.GetDescription(), 0 },
                        { OperationUnit.Channels.GetDescription(), 6 },
                        { OperationUnit.CustomerExp.GetDescription(), 0 },
                        { OperationUnit.Reconciliation.GetDescription(), 0 },
                        { OperationUnit.RecordsMgt.GetDescription(), 1 },
                        { OperationUnit.Payments.GetDescription(), 0 },
                        { OperationUnit.Wallets.GetDescription(), 0 },
                        { OperationUnit.CategoryTotal.GetDescription(), 11 }
                    },
                }
            });
        }

        public async Task<CategoryExtensionModel> CategoryExtensionsCountAsync(string category, long userId, string lastLoginIpAddress)
        {
            var processes = new List<CategoryExtensionModel>()
            {
                new(){
                    Banner = ProcessCategories.UpToDate.GetDescription(),
                    CategoryProcesses = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),9 },
                        { OperationUnit.Cash.GetDescription(), 3 },
                        { OperationUnit.Channels.GetDescription(), 9 },
                        { OperationUnit.CustomerExp.GetDescription(), 12 },
                        { OperationUnit.Reconciliation.GetDescription(), 4 },
                        { OperationUnit.RecordsMgt.GetDescription(), 3 },
                        { OperationUnit.Payments.GetDescription(), 9 },
                        { OperationUnit.Wallets.GetDescription(), 15 }
                    }
                },
                new()
                {
                    Banner = ProcessCategories.Proposed.GetDescription(),
                    CategoryProcesses = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),0 },
                        { OperationUnit.Cash.GetDescription(), 0 },
                        { OperationUnit.Channels.GetDescription(), 3 },
                        { OperationUnit.CustomerExp.GetDescription(), 0 },
                        { OperationUnit.Reconciliation.GetDescription(), 0 },
                        { OperationUnit.RecordsMgt.GetDescription(), 0 },
                        { OperationUnit.Payments.GetDescription(), 5 },
                        { OperationUnit.Wallets.GetDescription(), 2 }
                    }
                },
                new()
                {
                    Banner = ProcessCategories.Unchanged.GetDescription(),
                    CategoryProcesses = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),0 },
                        { OperationUnit.Cash.GetDescription(), 9 },
                        { OperationUnit.Channels.GetDescription(), 14 },
                        { OperationUnit.CustomerExp.GetDescription(), 2 },
                        { OperationUnit.Reconciliation.GetDescription(), 8 },
                        { OperationUnit.RecordsMgt.GetDescription(), 0 },
                        { OperationUnit.Payments.GetDescription(), 6 },
                        { OperationUnit.Wallets.GetDescription(), 0 }
                    },
                },
                new()
                {
                    Banner = ProcessCategories.Due.GetDescription(),
                    CategoryProcesses = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),0 },
                        { OperationUnit.Cash.GetDescription(), 1 },
                        { OperationUnit.Channels.GetDescription(), 7 },
                        { OperationUnit.CustomerExp.GetDescription(), 3 },
                        { OperationUnit.Reconciliation.GetDescription(), 0 },
                        { OperationUnit.RecordsMgt.GetDescription(), 0 },
                        { OperationUnit.Payments.GetDescription(), 5 },
                        { OperationUnit.Wallets.GetDescription(), 1 }
                    },
                },
                new()
                {
                    Banner = ProcessCategories.Dormant.GetDescription(),
                    CategoryProcesses = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),0 },
                        { OperationUnit.Cash.GetDescription(), 0 },
                        { OperationUnit.Channels.GetDescription(), 1 },
                        { OperationUnit.CustomerExp.GetDescription(), 0 },
                        { OperationUnit.Reconciliation.GetDescription(), 0 },
                        { OperationUnit.RecordsMgt.GetDescription(), 0 },
                        { OperationUnit.Payments.GetDescription(), 2 },
                        { OperationUnit.Wallets.GetDescription(), 1 }
                    },
                },
                new()
                {
                    Banner = ProcessCategories.Cancelled.GetDescription(),
                    CategoryProcesses = new Dictionary<string, int> {
                        { OperationUnit.AccountServices.GetDescription(),4 },
                        { OperationUnit.Cash.GetDescription(), 0 },
                        { OperationUnit.Channels.GetDescription(), 6 },
                        { OperationUnit.CustomerExp.GetDescription(), 0 },
                        { OperationUnit.Reconciliation.GetDescription(), 0 },
                        { OperationUnit.RecordsMgt.GetDescription(), 1 },
                        { OperationUnit.Payments.GetDescription(), 0 },
                        { OperationUnit.Wallets.GetDescription(), 0 }
                    },
                }
            };
            var obj = processes.FirstOrDefault(d => d.Banner.Trim().Equals(category?.Trim(), StringComparison.CurrentCultureIgnoreCase));
            return await Task.FromResult(obj);
        }

        public async Task<UnitExtensionModel> UnitExtensionsCountAsync(string unit, long userId, string ipAddress)
        {
            var processes = new List<UnitExtensionModel>()
            {
                new(){
                    Banner =  OperationUnit.AccountServices.GetDescription(),
                    UnitProcesses = new Dictionary<string, int> {
                        { ProcessCategories.UpToDate.GetDescription(), 9 },
                        { ProcessCategories.Proposed.GetDescription(),0 },
                        { ProcessCategories.Unchanged.GetDescription(), 0 },
                        { ProcessCategories.Due.GetDescription(), 0 },
                        { ProcessCategories.Dormant.GetDescription(), 0 },
                        { ProcessCategories.Cancelled.GetDescription(), 4 }
                    }
                },
                new()
                {
                    Banner = OperationUnit.Cash.GetDescription(),
                    UnitProcesses = new Dictionary<string, int> {
                        { ProcessCategories.UpToDate.GetDescription(), 3 },
                        { ProcessCategories.Proposed.GetDescription(),0 },
                        { ProcessCategories.Unchanged.GetDescription(), 9 },
                        { ProcessCategories.Due.GetDescription(), 1 },
                        { ProcessCategories.Dormant.GetDescription(), 0 },
                        { ProcessCategories.Cancelled.GetDescription(), 0 }
                    }
                },
                new()
                {
                    Banner = OperationUnit.Channels.GetDescription(),
                    UnitProcesses = new Dictionary<string, int> {
                        { ProcessCategories.UpToDate.GetDescription(), 9 },
                        { ProcessCategories.Proposed.GetDescription(),3 },
                        { ProcessCategories.Unchanged.GetDescription(), 14 },
                        { ProcessCategories.Due.GetDescription(), 7 },
                        { ProcessCategories.Dormant.GetDescription(), 1 },
                        { ProcessCategories.Cancelled.GetDescription(), 6 }
                    }
                },
                new()
                {
                    Banner = OperationUnit.CustomerExp.GetDescription(),
                    UnitProcesses = new Dictionary<string, int> {
                        { ProcessCategories.UpToDate.GetDescription(), 12 },
                        { ProcessCategories.Proposed.GetDescription(),0 },
                        { ProcessCategories.Unchanged.GetDescription(), 2 },
                        { ProcessCategories.Due.GetDescription(), 3 },
                        { ProcessCategories.Dormant.GetDescription(), 0 },
                        { ProcessCategories.Cancelled.GetDescription(), 0 }
                    }
                },
                new()
                {
                    Banner = OperationUnit.Reconciliation.GetDescription(),
                    UnitProcesses = new Dictionary<string, int> {
                        { ProcessCategories.UpToDate.GetDescription(), 4 },
                        { ProcessCategories.Proposed.GetDescription(),0 },
                        { ProcessCategories.Unchanged.GetDescription(), 8 },
                        { ProcessCategories.Due.GetDescription(), 0 },
                        { ProcessCategories.Dormant.GetDescription(), 0 },
                        { ProcessCategories.Cancelled.GetDescription(), 0 }
                    }
                },
                new()
                {
                    Banner = OperationUnit.RecordsMgt.GetDescription(),
                    UnitProcesses = new Dictionary<string, int> {
                        { ProcessCategories.UpToDate.GetDescription(), 3 },
                        { ProcessCategories.Proposed.GetDescription(),0 },
                        { ProcessCategories.Unchanged.GetDescription(), 0 },
                        { ProcessCategories.Due.GetDescription(), 0 },
                        { ProcessCategories.Dormant.GetDescription(), 0 },
                        { ProcessCategories.Cancelled.GetDescription(), 1 }
                    }
                },
                new()
                {
                    Banner = OperationUnit.Payments.GetDescription(),
                    UnitProcesses = new Dictionary<string, int> {
                        { ProcessCategories.UpToDate.GetDescription(), 9 },
                        { ProcessCategories.Proposed.GetDescription(),5 },
                        { ProcessCategories.Unchanged.GetDescription(), 6 },
                        { ProcessCategories.Due.GetDescription(), 5 },
                        { ProcessCategories.Dormant.GetDescription(), 2 },
                        { ProcessCategories.Cancelled.GetDescription(), 0 }
                    }
                },
                new()
                {
                    Banner = OperationUnit.Wallets.GetDescription(),
                    UnitProcesses = new Dictionary<string, int> {
                        { ProcessCategories.UpToDate.GetDescription(), 15 },
                        { ProcessCategories.Proposed.GetDescription(),2 },
                        { ProcessCategories.Unchanged.GetDescription(), 0 },
                        { ProcessCategories.Due.GetDescription(), 1 },
                        { ProcessCategories.Dormant.GetDescription(), 1 },
                        { ProcessCategories.Cancelled.GetDescription(), 0 }
                    },
                }
            };
            var obj = processes.FirstOrDefault(d => d.Banner.Trim().Equals(unit?.Trim(), StringComparison.CurrentCultureIgnoreCase));
            return await Task.FromResult(obj);
        }

        #endregion

        #region Process Registers

        public async Task<GrcResponse<GrcProcessSupportResponse>> GetProcessSupportItemsAsync(GrcRequest request) {
            try
            {
                if (request == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessSupportResponse>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/support-items";
                return await HttpHandler.PostAsync<GrcRequest, GrcProcessSupportResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<GrcProcessSupportResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetProcessRegistersAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/registers-all";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessRegisterResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetNewProcessAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/processes-new";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessRegisterResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetReviewProcessAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/processes-reviews";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessRegisterResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
            }
        }

        public async Task<GrcResponse<GrcProcessRegisterResponse>> GetProcessRegisterAsync(long recordId, long userId, string ipAddress) {

            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process ID is required",
                        "Invalid Process request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessRegisterResponse>(error);
                }

                var request = new GrcIdRequest() {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESSES_RETRIEVE_PROCESS.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/register";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessRegisterResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessRegisterResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateProcessAsync(ProcessViewModel processModel, long userId, string ipAddress) {
            
            try
            {
                if (processModel == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process record cannot be null",
                        "Invalid process record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = Mapper.Map<GrcProcessRegisterRequest>(processModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESSES_CREATE_PROCESS.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE Process REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/create-process";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessRegisterRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateProcessAsync(ProcessViewModel processModel, long userId, string ipAddress) {
            
            try
            {
                if (processModel == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process record cannot be null",
                        "Invalid process record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = Mapper.Map<GrcProcessRegisterRequest>(processModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESSES_EDITED_PROCESS.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE PROCESS REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/update-process";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessRegisterRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteProcessAsync(GrcIdRequest request) {
           
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process record cannot be null",
                        "Invalid process record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE PROCESS REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/delete-process";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Process Groups

        public async Task<GrcResponse<GrcProcessGroupResponse>> GetProcessGroupAsync(long recordId, long userId, string ipAddress) {

            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Group ID is required",
                        "Invalid Process request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessGroupResponse>(error);
                }

                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESS_GROUP_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/group";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessGroupResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessGroupResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessGroupResponse>>> GetProcessGroupsAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessGroupResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/groups-all";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessGroupResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessGroupResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateProcessGroupAsync(ProcessGroupViewModel groupModel, long userId, string ipAddress) {

            try {

                if (groupModel == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Process record cannot be null", "Invalid role record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcProcessGroupRequest {
                    Id = groupModel.Id,
                    GroupName = groupModel.GroupName,
                    GroupDescription = groupModel.GroupDescription,
                    CreatedBy = "SYSTEM",
                    CreatedOn = DateTime.UtcNow,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.PROCESS_GROUP_CREATE.GetDescription(),
                };

                //..map request
                Logger.LogActivity($"CREATE PROCESS GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/create-group";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessGroupRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }

        }

        public async Task<GrcResponse<ServiceResponse>> UpdateProcessGroupAsync(ProcessGroupViewModel groupModel, long userId, string ipAddress) {
            
            try {
                if (groupModel == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Group record cannot be null",
                        "Invalid process group record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = Mapper.Map<GrcProcessRegisterRequest>(groupModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESS_GROUP_UPDATE.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE PROCESS GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/update-group";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessRegisterRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteProcessGroupAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Group cannot be null",
                        "Invalid process group record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE PROCESS GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/delete-group";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Process Tags

        public async Task<GrcResponse<GrcProcessTagResponse>> GetProcessTagAsync(long recordId, long userId, string ipAddress) {

            try
            {
                if (recordId == 0)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Group ID is required",
                        "Invalid Process request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessTagResponse>(error);
                }

                var request = new GrcIdRequest() {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESS_TAG_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tag";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessTagResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessTagResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessTagResponse>>> GetProcessTagsAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessTagResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tags-all";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessTagResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessTagResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateProcessTagAsync(ProcessTagViewModel tagModel, long userId, string ipAddress) {
            try {

                if (tagModel == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Process record cannot be null", "Invalid role record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcProcessTagRequest {
                    Id = tagModel.Id,
                    TagName = tagModel.TagName,
                    TagDescription = tagModel.TagDescription,
                    CreatedBy = "SYSTEM",
                    CreatedOn = DateTime.UtcNow,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.PROCESS_TAG_CREATE.GetDescription(),
                };

                //..map request
                Logger.LogActivity($"CREATE PROCESS TAG REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/create-tag";
                Logger.LogActivity($"Endpoint: {endpoint}");
                return await HttpHandler.PostAsync<GrcProcessTagRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateProcessTagAsync(ProcessTagViewModel tagModel, long userId, string ipAddress)
        {
            try
            {
                if (tagModel == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Tag record cannot be null",
                        "Invalid process tag record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = Mapper.Map<GrcProcessRegisterRequest>(tagModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESS_TAG_UPDATE.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE PROCESS GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/update-tag";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessRegisterRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteProcessTagAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Tag cannot be null",
                        "Invalid process tag record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE PROCESS TAG REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/delete-tag";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Process TAT

        public async Task<GrcResponse<GrcProcessTatResponse>> GetProcessTatAsync(long recordId, long userId, string ipAddress) {

            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Tag ID is required",
                        "Invalid Process request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessTatResponse>(error);
                }

                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESS_TAG_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tat";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessTatResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessTatResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessTatResponse>>> GetProcessTatAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessTatResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tat-all";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessTatResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessTatResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessApprovalStatusResponse>>> GetProcessApprovalStatusAsync(TableListRequest request) {
            try
            {
                if (request == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessApprovalStatusResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/approval-status";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessApprovalStatusResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessApprovalStatusResponse>>(error);
            }
        }

        #endregion

    }

}
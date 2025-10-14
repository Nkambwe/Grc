using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;

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
    }

}
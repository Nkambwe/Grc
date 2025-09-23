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

                ProcessCategories = new ProcessCategoryStatisticsResponse()
                {

                    CashProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
                    },
                    AccountServiceProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
                    },
                    RecordsMgtProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
                    },
                    ChannelProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
                    },
                    PaymentProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
                    },
                    WalletProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
                    },
                    CustomerExperienceProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
                    },
                    ReconciliationProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
                    },
                    TotalCategoryProcesses = new CategoriesCountResponse()
                    {
                        Unclassified = 10,
                        UpToDate = 13,
                        Unchanged = 16,
                        Proposed = 39,
                        Due = 17,
                        Dormant = 8,
                        Cancelled = 4,
                        Total = 114
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
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

        public async Task<OperationsCountResponse> StatisticAsync(long userId, string ipAddress) {
            var countResponse = new OperationsCountResponse
            {
                CompletedProcesses = new Dictionary<OperationUnit, int> {
                    {OperationUnit.AccountServices, 9 },
                    {OperationUnit.Cash, 3 },
                    {OperationUnit.Channels, 9 },
                    {OperationUnit.CustomerExp, 12 },
                    {OperationUnit.Reconciliation, 4 },
                    {OperationUnit.RecordsMgt, 3 },
                    {OperationUnit.Payments, 9 },
                    {OperationUnit.Wallets, 15 },
                    {OperationUnit.CategoryTotal, 64 }
                },
                ProposedProcesses = new Dictionary<OperationUnit, int> {
                    {OperationUnit.AccountServices, 0 },
                    {OperationUnit.Cash, 0 },
                    {OperationUnit.Channels, 3},
                    {OperationUnit.CustomerExp, 0 },
                    {OperationUnit.Reconciliation, 0 },
                    {OperationUnit.RecordsMgt, 0 },
                    {OperationUnit.Payments, 5 },
                    {OperationUnit.Wallets, 2 },
                    {OperationUnit.CategoryTotal, 10 }
                },
                UnchangedProcesses = new Dictionary<OperationUnit, int> {
                    {OperationUnit.AccountServices, 0 },
                    {OperationUnit.Cash, 9 },
                    {OperationUnit.Channels, 14 },
                    {OperationUnit.CustomerExp, 2 },
                    {OperationUnit.Reconciliation, 8 },
                    {OperationUnit.RecordsMgt, 0 },
                    {OperationUnit.Payments, 6 },
                    {OperationUnit.Wallets, 0 },
                    {OperationUnit.CategoryTotal, 39 }
                },
                ProcessesDueForReview = new Dictionary<OperationUnit, int> {
                    {OperationUnit.AccountServices, 0 },
                    {OperationUnit.Cash, 1 },
                    {OperationUnit.Channels, 7 },
                    {OperationUnit.CustomerExp, 3 },
                    {OperationUnit.Reconciliation, 0 },
                    {OperationUnit.RecordsMgt, 0 },
                    {OperationUnit.Payments, 5 },
                    {OperationUnit.Wallets, 1 },
                    {OperationUnit.CategoryTotal, 17 }
                },
                DormantProcesses = new Dictionary<OperationUnit, int> {
                    {OperationUnit.AccountServices, 0 },
                    {OperationUnit.Cash, 0 },
                    {OperationUnit.Channels, 0 },
                    {OperationUnit.CustomerExp, 0 },
                    {OperationUnit.Reconciliation, 0 },
                    {OperationUnit.RecordsMgt, 0 },
                    {OperationUnit.Payments, 0 },
                    {OperationUnit.Wallets, 0 },
                    {OperationUnit.CategoryTotal, 64 }
                },
                CancelledProcesses = new Dictionary<OperationUnit, int> {
                    {OperationUnit.AccountServices, 4 },
                    {OperationUnit.Cash, 0 },
                    {OperationUnit.Channels, 6 },
                    {OperationUnit.CustomerExp, 0 },
                    {OperationUnit.Reconciliation, 0 },
                    {OperationUnit.RecordsMgt, 1 },
                    {OperationUnit.Payments, 0 },
                    {OperationUnit.Wallets, 0 },
                    {OperationUnit.CategoryTotal, 11 }
                },
                UnitTotalProcesses = new Dictionary<ProcessCategories, int> {
                    {ProcessCategories.UpToDate, 13 },
                    {ProcessCategories.Proposed, 16 },
                    {ProcessCategories.Unchanged, 39 },
                    {ProcessCategories.Due, 17 },
                    {ProcessCategories.Dormant, 8 },
                    {ProcessCategories.Cancelled, 4 },
                    {ProcessCategories.UnitTotal, 141 }
                },
            };

            return await Task.FromResult(countResponse);
        }
    }

}
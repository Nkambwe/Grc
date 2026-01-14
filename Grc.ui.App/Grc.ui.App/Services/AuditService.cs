using AutoMapper;
using Grc.ui.App.Dtos;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {
    public class AuditService : GrcBaseService, IAuditService {
        public AuditService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, 
            IMapper mapper, 
            IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType, mapper, 
                  webHelper, sessionManager, errorFactory, errorService) {
        }

        public async Task<GrcResponse<AuditDashboardResponse>> GetAuditStatisticAsync(long userId, string iPAddress) {
            return await Task.FromResult(new GrcResponse<AuditDashboardResponse>() {
                HasError = false,
                Data = new AuditDashboardResponse() { 
                    Findings = new() {
                        {"Total", 80 },
                        {"< 1 Month", 10 },
                        {"1 to 2 Months", 50 },
                        {"> 3 Months", 20 }
                    },
                    Completions = new() {
                        {"Closed", 70 },
                        {"Open", 60 },
                        {"Over Due", 30 }
                    },
                    BarChart = new() {
                        {"BOU", new Dictionary<string, int>(){
                            {"Closed", 20 },
                            {"Open", 30 },
                            {"Over Due", 10 }
                        } },
                        {"KPMG",  new Dictionary<string, int>(){
                            {"Closed", 20 },
                            {"Open", 40 },
                            {"Over Due",15 }
                        } },
                        {"IRA",  new Dictionary<string, int>(){
                            {"Closed", 30 },
                            {"Open", 10 },
                            {"Over Due", 30 }
                        } },
                        {"ER",  new Dictionary<string, int>(){
                            {"Closed", 8 },
                            {"Open", 12 },
                            {"Over Due", 5 }
                        } }
                    }
                }
            });
        }

        public async Task<GrcResponse<AuditExtensionStatistics>> GetAuditExtensionStatisticAsync(long userId, string iPAddress, string period) {
            var statuses = new Dictionary<string, int>() { {"Total", 80 }, {"Open", 10 }, {"Close", 50 },{"Due", 20 }};

            return await Task.FromResult(new GrcResponse<AuditExtensionStatistics>() {
                Data = new AuditExtensionStatistics() {
                    Statuses = statuses,
                    Reports = new List<GrcAuditResponse>() {
                        new () { 
                            Id =  1,
                            Reference = "BOU OUT/1",
                            ReportName = "Sample BOU Report",
                            Count = 10,
                            AuditedOn = new DateTime(2024,2,15),
                            Status = "OPEN"
                        },
                        new () {
                            Id =  3,
                            Reference = "KMGT EXT/1",
                            ReportName = "Sample KMGT Report",
                            Count = 10,
                            AuditedOn = new DateTime(2024,6,2),
                            Status = "CLOSE"
                        },
                        new () {
                            Id =  3,
                            Reference = "IR EXT/1",
                            ReportName = "Sample IR Report",
                            Count = 10,
                            AuditedOn = new DateTime(2024,2,15),
                            Status = "OVER DUE"
                        }
                    }
                }
            });
        }
    }

}

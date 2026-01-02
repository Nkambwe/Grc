using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public class RegistersService : IRegistersService {

        public async Task<ComplianceStatistics> StatisticAsync(long userId, string ipAddress) {
            var data = new ComplianceStatistics {
                CircularTotals = new CircularTotalsStatistics {
                    Period = DateTime.Now.ToString("MMM yyyy").ToUpper(),
                    Total = 31,
                    Submitted = 28,
                    Closed = 3,
                    Breach = 4,
                },
                ReturnTotals = new ReturnTotalsStatistics {
                    Period = DateTime.Now.ToString("MMM yyyy").ToUpper(),
                    Total = 52,
                    Submitted = 20,
                    Open = 32,
                    Breach = 4
                },
                TaskTotals = new TaskTotalsStatistics {
                    Period = DateTime.Now.ToString("MMM yyyy").ToUpper(),
                    Total = 51,
                    Open = 8,
                    Closed = 43,
                    Failed = 8
                },
                Circulars = new List<CircularAuthorityStatistics>() {
                    new() {
                        Authority = "BOU",
                        Received = 120,
                        Open = 64,
                        Closed = 56
                    }
                },
                Regulations = new List<RegulationStatistics>() {
                    new() {
                        Regulation = "Anti Money Laudering",
                        Applicable = 0,
                        Gaps = 0,
                        Compliant = 0,
                        Issues = 0
                    }
                },
                Tasks = new List<TaskTotalsStatistics>() {
                    new() {
                        Period = "TODAY",
                        Open = 2,
                        Closed = 4,
                        Failed = 0
                    },
                    new() {
                        Period = "THIS WEEK",
                        Open = 8,
                        Closed = 12,
                        Failed = 0
                    },
                    new() {
                        Period = "LAST WEEK",
                        Open = 2,
                        Closed = 20,
                        Failed = 2
                    },
                    new() {
                        Period =DateTime.Now.ToString("MMM yyyy").ToUpper(),
                        Open = 80,
                        Closed = 20,
                        Failed = 2
                    }
                },
            };
            return await Task.FromResult(data);
        }
    }
}

using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public class RegistersService : IRegistersService {

        public async Task<ComplianceStatistics> StatisticAsync(long userId, string ipAddress) {
            var data = new ComplianceStatistics {
                CircularTotals = new CircularTotalsStatistics {
                    Received = 431,
                    Open = 231,
                    Closed = 200
                },
                RegulationsTotals = new RegulationsTotalsStatistics {
                    Applicable = 3940,
                    Gaps = 132,
                    Covered = 3010,
                    NotApplicable = 320,
                    Issues = 828
                },
                CircularByAuthority = new List<CircularAuthorityStatistics>() {
                    new() {
                        Authority = "BOU",
                        Received = 120,
                        Open = 64,
                        Closed = 56
                    }
                },
                RegulationStatistics = new List<RegulationStatistics>() {
                    new() {
                        Regulation = "Anti Money Laudering",
                        Applicable = 0,
                        Gaps = 0,
                        Covered = 0,
                        Issues = 0
                    }
                }
            };
            return await Task.FromResult(data);
        }
    }
}

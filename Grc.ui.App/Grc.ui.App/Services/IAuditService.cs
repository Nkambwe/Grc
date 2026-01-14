
using Grc.ui.App.Dtos;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public interface IAuditService : IGrcBaseService {
        Task<GrcResponse<AuditExtensionStatistics>> GetAuditExtensionStatisticAsync(long userId, string iPAddress, string period);
        Task<GrcResponse<AuditDashboardResponse>> GetAuditStatisticAsync(long userId, string iPAddress);
    }

}

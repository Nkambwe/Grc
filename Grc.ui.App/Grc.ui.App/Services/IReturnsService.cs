using Grc.ui.App.Dtos;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IReturnsService {

        #region Policy Statistics
        Task<GrcResponse<GrcPolicyDashboardResponse>> GetPolicyCountAsync(long userId, string ipAddress, string status);
        Task<GrcResponse<GeneralComplianceReturnResponse>> GetAllReturnsStatisticAsync(long userId, string ipAddress);
        #endregion

        #region Returns Statistics

        Task<GrcResponse<ReturnsDashboardResponses>> GetReturnStatisticAsync(long userId, string ipAddress);
        Task<GrcResponse<ComplianceExtensionReturnResponse>> GetReturnExtensionStatisticAsync(long userId, string iPAddress, string period);
        Task<GrcResponse<GrcReturnDashboardResponse>> GrcReturnDashboardResponseAsync(long userId, string ipAddress, string period);
        
        #endregion

        #region Circular Statistics

        Task<GrcResponse<CircularDashboardResponses>> GetCircularStatisticAsync(long userId, string ipAddress);
        Task<GrcResponse<CircularExtensionDashboardResponses>> GetCircularExtensionStatisticAsync(long userId, string iPAddress, string authority);
        Task<GrcResponse<CircularMiniDashboardResponses>> GetAuthorityCircularCountAsync(long userId, string ipAddress, string authority);

        #endregion

        #region Tasks Statistics
        Task<GrcResponse<ComplianceTaskStatisticResponse>> GetTaskStatisticAsync(long userId, string ipAddress);
        Task<GrcResponse<ComplianceTaskMinStatisticResponse>> GetMiniTaskStatisticAsync(long userId, string iPAddress, string status);

        #endregion

        #region Returns Queries
        Task<GrcResponse<PagedResponse<GrcReturnsResponse>>> GetPagedReturnsAsync(TableListRequest request);

        #endregion

        #region Circular Queries
        Task<GrcResponse<PagedResponse<GrcCircularsResponse>>> GetPagedCircularAsync(TableListRequest request);
        #endregion
    }
}

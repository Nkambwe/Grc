using Grc.ui.App.Dtos;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IReturnsService {

        #region Returns Statistics

        Task<GrcResponse<GeneralComplianceReturnResponse>> GetAllReturnsStatisticAsync(long userId, string ipAddress);
        Task<GrcResponse<ReturnsDashboardResponses>> GetReturnStatisticAsync(long userId, string ipAddress);
        Task<GrcResponse<ReturnsMiniDashboardResponses>> GetStatusReturnStatisticAsync(long userId, string ipAddress, string status);
        Task<GrcResponse<ReturnsMiniDashboardResponses>> GetPeriodReturnStatisticAsync(long userId, string ipAddress, string period);
        
        #endregion

        #region Circular Statistics

        Task<GrcResponse<CircularDashboardResponses>> GetCircularStatisticAsync(long userId, string ipAddress);
        Task<GrcResponse<CircularMiniDashboardResponses>> GetStatusCircularCountAsync(long userId, string ipAddress, string status);
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

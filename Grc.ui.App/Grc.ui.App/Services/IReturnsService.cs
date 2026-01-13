using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

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

        #region Submissions

        Task<GrcResponse<GrcReturnSubmissionResponse>> GetSubmissionAsync(GrcIdRequest request);

        Task<GrcResponse<ServiceResponse>> UpdateSubmissionAsync(SubmissionViewModel submission, long userId, string ipAddress);

        Task<GrcResponse<GrcCircularSubmissionResponse>> GetCircularSubmissionAsync(GrcIdRequest request);

        Task<GrcResponse<ServiceResponse>> UpdateCircularSubmissionAsync(CircularSubmissionViewModel submission, long userId, string ipAddress);

        #endregion

        #region Returns Queries
        Task<GrcResponse<GrcStatutoryReturnReportResponse>> GetReturnAsync(GrcIdRequest request);
        Task<GrcResponse<ServiceResponse>> CreateReturnAsync(StatutoryReturnViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateReturnAsync(StatutoryReturnViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteReturnAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcReturnSubmissionResponse>>> GetReturnSubmissionsAsync(TableListRequest model);
        Task<GrcResponse<PagedResponse<GrcReturnsResponse>>> GetPagedReturnsAsync(TableListRequest request);
        Task<GrcResponse<PagedResponse<GrcReturnsResponse>>> GetReturnsListAsync(TableListRequest request);
        Task<GrcResponse<PagedResponse<GrcFrequencyResponse>>> GetFrequencyReturnsAsync(TableListRequest request);
        #endregion

        #region Circular Queries
        Task<GrcResponse<PagedResponse<GrcCircularsResponse>>> GetPagedCircularAsync(TableListRequest request);
        

        #endregion
    }
}

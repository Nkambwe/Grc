
using Grc.ui.App.Dtos;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IAuditService : IGrcBaseService {

        #region Statistics
        Task<GrcResponse<GrcAuditMiniReportResponse>> GetAuditExceptionReportAsync(GrcIdRequest request);
        Task<GrcResponse<AuditExtensionStatistics>> GetAuditExtensionStatisticAsync(long userId, string iPAddress, string period);
        Task<GrcResponse<List<GrcAuditMiniReportResponse>>> GetAuditMiniReportAsync(Models.AuditListViewModel request, long userId, string ipAddress);
        Task<GrcResponse<GrcAuditDashboardResponse>> GetAuditStatisticAsync(long userId, string iPAddress);

        #endregion

        #region Audits
        Task<GrcResponse<GrcAuditResponse>> GetAuditAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditResponse>>> GetAuditsAsync(TableListRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditResponse>>> GetTypeAuditsAsync(AuditListViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> CreateAuditAsync(AuditViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateAuditAsync(AuditViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteAuditAsync(GrcIdRequest request);

        #endregion

        #region Audit Types
        Task<GrcResponse<GrcAuditTypeResponse>> GetAuditTypeAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditTypeResponse>>> GetAuditTypesAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> CreateAuditTypeAsync(AuditTypeViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateAuditTypeAsync(AuditTypeViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequest request);

        #endregion

        #region Audit Report
        Task<GrcResponse<GrcAuditReportResponse>> GetAuditReportAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditReportResponse>>> GetAuditReportsAsync(TableListRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditReportResponse>>> GetAuditTentativeReportsAsync(AuditListViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> CreateAuditReportAsync(AuditReportViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateAuditReportAsync(AuditReportViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteReportAsync(GrcIdRequest request);
        #endregion

        #region Audit Exception
        Task<GrcResponse<GrcAuditExceptionResponse>> GetAuditExceptionAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditExceptionResponse>>> GetOpenExceptionsAsync(TableListRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditExceptionResponse>>> GetAuditExceptionsAsync(AuditCategoryViewModel request, long userId, string ipAddress, string action);
        Task<GrcResponse<ServiceResponse>> CreateAuditExceptionAsync(AuditExceptionViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateAuditExceptionAsync(AuditExceptionViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteExceptionAsync(GrcIdRequest request);

        #endregion

        #region Audit Updates

        Task<GrcResponse<GrcAuditUpdateResponse>> GetAuditUpdateAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditUpdateResponse>>> GetReportNotesAsync(GrcAuditMiniUpdateRequest request);
        Task<GrcResponse<ServiceResponse>> CreateAuditUpdateAsync(AuditUpdateViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateAuditUpdateAsync(AuditUpdateViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteAuditUpdateAsync(GrcIdRequest request);

        #endregion

        #region Audit Tasks

        Task<GrcResponse<GrcAuditTaskResponse>> GetAuditTaskAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcAuditTaskResponse>>> GetExceptionTasksAsync(GrcExceptionTaskViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> CreateExceptionTaskAsync(GrcAuditTaskViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateExceptionTaskAsync(GrcAuditTaskViewModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteExceptionTaskAsync(GrcIdRequest request);

        #endregion

        #region Reports
        Task<GrcResponse<List<GrcExceptionReport>>> GetExceptionReportAsync(GrcRequest request);
        #endregion
    }

}

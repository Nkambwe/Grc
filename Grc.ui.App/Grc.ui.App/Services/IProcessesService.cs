using Grc.ui.App.Dtos;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IProcessesService : IGrcBaseService  {

        #region Statistics
        Task<OperationsUnitCountResponse> StatisticAsync(long userId, string ipAddress);

        Task<CategoriesCountResponse> UnitCountAsync(long userId, string ipAddress, string unit);

        Task<List<DashboardRecord>> TotalExtensionsCountAsync(long userId, string ipAddress);

        Task<CategoryExtensionModel> CategoryExtensionsCountAsync(string category, long userId, string ipAddress);

        Task<UnitExtensionModel> UnitExtensionsCountAsync(string unit, long userId, string ipAddress);

        #endregion

        #region Process Registers

        Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetProcessRegistersAsync(TableListRequest request);
        
        Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetNewProcessAsync(TableListRequest request);

        Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetReviewProcessAsync(TableListRequest request);

        Task<GrcResponse<GrcProcessRegisterResponse>> GetProcessRegisterAsync(long id, long userId, string ipAddress);

        Task<GrcResponse<GrcProcessSupportResponse>> GetProcessSupportItemsAsync(GrcRequest request);

        Task<GrcResponse<ServiceResponse>> CreateProcessAsync(ProcessViewModel request, long userId, string ipAddress);

        Task<GrcResponse<ServiceResponse>> UpdateProcessAsync(ProcessViewModel request, long userId, string ipAddress);

        Task<GrcResponse<ServiceResponse>> DeleteProcessAsync(GrcIdRequest request);

        #endregion

        #region Process Groups
        Task<GrcResponse<GrcProcessGroupResponse>> GetProcessGroupAsync(long id, long userId, string ipAddress);

        Task<GrcResponse<PagedResponse<GrcProcessGroupResponse>>> GetProcessGroupsAsync(TableListRequest request);

        Task<GrcResponse<ServiceResponse>> CreateProcessGroupAsync(ProcessGroupViewModel request, long userId, string ipAddress);

        Task<GrcResponse<ServiceResponse>> UpdateProcessGroupAsync(ProcessGroupViewModel request, long userId, string ipAddress);

        Task<GrcResponse<ServiceResponse>> DeleteProcessGroupAsync(GrcIdRequest request);

        #endregion

        #region Process Tags

        Task<GrcResponse<GrcProcessTagResponse>> GetProcessTagAsync(long id, long userId, string ipAddress);

        Task<GrcResponse<PagedResponse<GrcProcessTagResponse>>> GetProcessTagsAsync(TableListRequest request);

        Task<GrcResponse<ServiceResponse>> CreateProcessTagAsync(ProcessTagViewModel request, long userId, string ipAddress);

        Task<GrcResponse<ServiceResponse>> UpdateProcessTagAsync(ProcessTagViewModel request, long userId, string ipAddress);

        Task<GrcResponse<ServiceResponse>> DeleteProcessTagAsync(GrcIdRequest request);

        #endregion

        #region Process TAT

        Task<GrcResponse<GrcProcessTatResponse>> GetProcessTatAsync(long id, long userId, string ipAddress);

        Task<GrcResponse<PagedResponse<GrcProcessTatResponse>>> GetProcessTatAsync(TableListRequest request);
        
        Task<GrcResponse<PagedResponse<GrcProcessApprovalStatusResponse>>> GetProcessApprovalStatusAsync(TableListRequest request);

        #endregion

    }

}
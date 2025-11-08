using Grc.ui.App.Dtos;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IProcessesService : IGrcBaseService  {

        /// <summary>
        /// Get all process statistics
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="userId">User ID of the requesting user</param>
        /// <returns>Task containing user dashboard statistics/returns>
        Task<OperationsUnitCountResponse> StatisticAsync(long userId, string ipAddress);

        /// <summary>
        ///  Get count of count by unit for all processes
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="userId">User ID of the requesting user</param>
        /// <param name="unit">name of unit to count for</param>
        /// <returns>Task containing user dashboard statistics/returns>
        Task<CategoriesCountResponse> UnitCountAsync(long userId, string ipAddress, string unit);

        /// <summary>
        ///  Get count of process categories for all processes
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="userId">User ID of the requesting user</param>
        /// <returns>Task containing user dashboard statistics/returns>
        Task<List<DashboardRecord>> TotalExtensionsCountAsync(long userId, string ipAddress);

        /// <summary>
        /// Get count of processes per category
        /// </summary>
        /// <param name="category">Process category</param>
        /// <param name="userId">Current user id</param>
        /// <param name="ipAddress">Current user IP Address</param>
        /// <returns></returns>
        Task<CategoryExtensionModel> CategoryExtensionsCountAsync(string category, long userId, string ipAddress);

        /// <summary>
        /// Get count of processes per unit
        /// </summary>
        /// <param name="unit">Operations Unit</param>
        /// <param name="userId">Current user id</param>
        /// <param name="ipAddress">Current user IP Address</param>
        /// <returns></returns>
        Task<UnitExtensionModel> UnitExtensionsCountAsync(string unit, long userId, string ipAddress);

        Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetProcessRegistersAsync(TableListRequest request);

        Task<GrcResponse<GrcProcessRegisterResponse>> GetProcessRegisterAsync(long id, long userId, string ipAddress);

        Task<GrcResponse<GrcProcessSupportResponse>> GetProcessSupportItemsAsync(GrcRequest request);
    }

}
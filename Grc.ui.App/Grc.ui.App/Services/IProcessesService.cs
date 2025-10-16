using Grc.ui.App.Dtos;
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
        /// <param name="category"></param>
        /// <param name="userId"></param>
        /// <param name="lastLoginIpAddress"></param>
        /// <returns></returns>
        Task<CategoryExtensionModel> CategoryExtensionsCountAsync(string category, long userId, string lastLoginIpAddress);
    }

}
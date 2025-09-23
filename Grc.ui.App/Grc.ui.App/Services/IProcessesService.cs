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


    }

}
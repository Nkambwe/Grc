using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IRegistersService {
        /// <summary>
        /// Get all process statistics
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="userId">User ID of the requesting user</param>
        /// <returns>Task containing user dashboard statistics/returns>
        Task<ComplianceStatistics> StatisticAsync(long userId, string ipAddress);
    }
}

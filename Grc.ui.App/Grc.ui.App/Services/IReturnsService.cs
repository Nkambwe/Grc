using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IReturnsService {
        /// <summary>
        /// Get all returns statistics
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="userId">User ID of the requesting user</param>
        /// <returns>Task containing all returns dashboard statistics/returns>
        Task<GeneralComplianceReturnStatistics> GetAllStatisticAsync(long userId, string ipAddress);
        /// <summary>
        /// Get returns statistics
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="userId">User ID of the requesting user</param>
        /// <returns>Task containing return dashboard statistics/returns>
        Task<ComplianceReturnStatistics> GetReturnStatisticAsync(long userId, string ipAddress);
        Task<ComplianceMinReturnStatistics> GetReturnReceivedStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinReturnStatistics> GetReturnTotalStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinReturnStatistics> GetReturnOpenStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinReturnStatistics> GetReturnSubmittedStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinReturnStatistics> GetReturnBreachStatisticAsync(long userId, string iPAddress);
        /// <summary>
        /// Get circular returns statistics
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="userId">User ID of the requesting user</param>
        /// <returns>Task containing circular return dashboard statistics/returns>
        Task<ComplianceCircularStatistics> GetCircularStatisticAsync(long userId, string ipAddress);
        Task<ComplianceMinCircularStatistics> GetClosedCircularStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinCircularStatistics> GetOpenCircularStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinCircularStatistics> GetReceivedCircularStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinCircularStatistics> GetBreachCircularStatisticAsync(long userId, string iPAddress);
        /// <summary>
        /// Get return tasks statistics
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="userId">User ID of the requesting user</param>
        /// <returns>Task containing return task dashboard statistics/returns>
        Task<ComplianceTaskStatistics> GetTaskStatisticAsync(long userId, string ipAddress);
        Task<ComplianceMinStatistics> GetTotalTaskStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinStatistics> GetOpenTaskStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinStatistics> GetClosedTaskStatisticAsync(long userId, string iPAddress);
        Task<ComplianceMinStatistics> GetFailedTaskStatisticAsync(long userId, string iPAddress);

    }
}

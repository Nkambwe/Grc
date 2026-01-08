using Grc.ui.App.Dtos;
using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IDashboardFactory {

        Task<UserDashboardModel> PrepareUserDashboardModelAsync(UserModel currentUser);
        Task<UserDashboardModel> PrepareUserModelAsync(UserModel currentUser);
        Task<PolicyRegisterViewModel> PrepareReturnSupportItemsModelAsync(UserModel data);
        Task<PolicyDashboardModel> PreparePolicyMinModelAsync(UserModel data, string status);
        Task<ComplianceGeneralStatisticViewModel> PrepareGeneralReturnsDashboardModelAsync(UserModel currentUser);

        #region Returns
        Task<ComplianceReturnStatisticViewModel> PrepareReturnsDashboardModelAsync(UserModel currentUser);
        Task<ComplianceExtensionReturnStatisticViewModel> PrepareReturnExtensionDashboardModelAsync(UserModel currentUser, string period);
        Task<ReturnMiniStatisticViewModel> PrepareReturnPeriodDashboardModelAsync(UserModel currentUser, string period);

        #endregion

        #region Circulars
        Task<CircularDashboardModel> PrepareCircularDashboardModelAsync(UserModel data);
        Task<CircularExtensionDashboardModel> PrepareCircularExtensionDashboardModelAsync(UserModel currentUser, string authority);
        Task<CircularMiniStatisticViewModel> PrepareCircularAuthorityDashboardModelAsync(UserModel currentUser, string authority);
        #endregion

        #region Tasks
        Task<TaskDashboardModel> PrepareTasksDashboardModelAsync(UserModel data);
        Task<TaskMinDashboardModel> PrepareMinTaskDashboardStatisticModelAsync(UserModel currentUser, string status);

        #endregion
    }
}

using Grc.ui.App.Dtos;
using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IDashboardFactory {

        Task<UserDashboardModel> PrepareUserDashboardModelAsync(UserModel currentUser);
        Task<UserDashboardModel> PrepareUserModelAsync(UserModel currentUser);
        Task<PolicyRegisterViewModel> PrepareReturnSupportItemsModelAsync(UserModel data);

        #region Circulars
        Task<CircularDashboardModel> PrepareCircularDashboardModelAsync(UserModel data);
        Task<CircularExtensionModel> PrepareCircularAuthorityDashboardModelAsync(UserModel currentUser, string authority);
        Task<CircularExtensionModel> PrepareCircularStatusDashboardModelAsync(UserModel currentUser, string status);
        #endregion

        #region Returns
        Task<ComplianceGeneralStatisticViewModel> PrepareGeneralReturnsDashboardModelAsync(UserModel currentUser);
        Task<ComplianceReturnStatisticViewModel> PrepareReturnsDashboardModelAsync(UserModel currentUser);
        Task<ComplianceMiniReturnStatisticViewModel> PrepareReturnPeriodDashboardModelAsync(UserModel currentUser, string period);
        Task<ComplianceMiniReturnStatisticViewModel> PrepareReturnStatusDashboardModelAsync(UserModel currentUser, string status);

        #endregion

        #region Tasks
        Task<TaskDashboardModel> PrepareTasksDashboardModelAsync(UserModel data);
        Task<TaskMinDashboardModel> PrepareMinTaskDashboardStatisticModelAsync(UserModel currentUser, string status);

        #endregion
    }
}

using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IDashboardFactory {
        Task<CircularDashboardModel> PrepareCircularDashboardModelAsync(UserModel data);
        Task<CircularMinDashboardModel> PrepareCircularBreachStatisticModelAsync(UserModel data);
        Task<CircularMinDashboardModel> PrepareCircularClosedStatisticModelAsync(UserModel data);
        Task<CircularMinDashboardModel> PrepareCircularOpenStatisticModelAsync(UserModel data);
        Task<CircularMinDashboardModel> PrepareCircularReceivedStatisticModelAsync(UserModel data);
        Task<ReturnsDashboardModel> PrepareReturnsDashboardModelAsync(UserModel data);
        Task<ReturnsMinDashboardModel> PrepareReturnReceivedStatisticModelAsync(UserModel data);
        Task<ReturnsMinDashboardModel> PrepareReturnTotalStatisticModelAsync(UserModel data);
        Task<ReturnsMinDashboardModel> PrepareReturnOpenStatisticModelAsync(UserModel data);
        Task<ReturnsMinDashboardModel> PrepareReturnSubmittedStatisticModelAsync(UserModel data);
        Task<ReturnsMinDashboardModel> PrepareReturnBreachStatisticModelAsync(UserModel data);
        Task<TaskDashboardModel> PrepareTasksDashboardModelAsync(UserModel data);
        Task<TaskMinDashboardModel> PrepareTotalTaskStatisticModelAsync(UserModel data);
        Task<TaskMinDashboardModel> PrepareOpenTaskStatisticModelAsync(UserModel data);
        Task<TaskMinDashboardModel> PrepareClosedTaskStatisticModelAsync(UserModel data);
        Task<TaskMinDashboardModel> PrepareFailedTaskStatisticModelAsync(UserModel data);
        Task<UserDashboardModel> PrepareUserDashboardModelAsync(UserModel model);
        Task<UserDashboardModel> PrepareUserModelAsync(UserModel model);
        Task<PolicyRegisterViewModel> PrepareReturnSupportItemsModelAsync(UserModel data);
    }
}
